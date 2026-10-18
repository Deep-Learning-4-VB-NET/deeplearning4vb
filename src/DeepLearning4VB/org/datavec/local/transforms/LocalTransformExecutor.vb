'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports DeepLearning4VB.org.datavec.local.transforms.functions
Imports DeepLearning4VB.org.datavec.local.transforms.join
Imports DeepLearning4VB.org.datavec.local.transforms.misc
Imports DeepLearning4VB.org.datavec.local.transforms.rank
Imports DeepLearning4VB.org.datavec.local.transforms.reduce
Imports DeepLearning4VB.org.datavec.local.transforms.sequence
Imports DeepLearning4VB.org.datavec.local.transforms.transform
Imports DeepLearning4VB.org.datavec.local.transforms.transform.filter
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports BooleanWritableT = DeepLearning4VB.org.datavec.api.writable.BooleanWritable
Imports CalculateSortedRankT = DeepLearning4VB.org.datavec.api.transform.rank.CalculateSortedRank
Imports ColumnTypeT = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConvertToSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertToSequence
Imports DataActionT = DeepLearning4VB.org.datavec.api.transform.DataAction
Imports DoubleWritableT = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports FloatWritableT = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports IAssociativeReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports IntWritableT = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports JoinT = DeepLearning4VB.org.datavec.api.transform.join.Join
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports LongWritableT = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NullWritableT = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableComparatorT = DeepLearning4VB.org.datavec.api.writable.comparator.WritableComparator
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms

    ''' <summary>
    ''' Executes a <see cref="TransformProcessT"/> on in-memory data: lists of records (<c>IList(Of IList(Of Writable))</c>)
    ''' or lists of sequences (<c>IList(Of IList(Of IList(Of Writable)))</c>), including the steps that need the whole
    ''' data set at once (conversion to sequences by key, reductions, sorted ranks) and joins of two data sets.
    ''' <para>
    ''' Differences from the Java executor, which ran the steps on Java streams and converted the output through
    ''' Apache Arrow columns:
    ''' </para>
    ''' <list type="bullet">
    ''' <item>The steps run sequentially and the output is deterministic: records keep their input order through
    ''' transforms and filters; groups (conversion to sequences, reductions, join keys) come out in the order in which
    ''' their key first appears in the input (Java: hash map order); within a group, records keep their input order before
    ''' the sequence comparator's stable sort; the sorted rank uses a stable sort, so ties keep their input order.</item>
    ''' <item>The Arrow round trip is replaced by a direct conversion of every output value to the writable type of its
    ''' column in the final schema (<c>IntWritable</c> for Integer, <c>LongWritable</c> for Long and Time,
    ''' <c>FloatWritable</c>, <c>DoubleWritable</c>, <c>BooleanWritable</c>, <c>Text</c> for String and Categorical;
    ''' bytes values are kept; a <c>NullWritable</c> stays null and a value that cannot be converted becomes a
    ''' <c>NullWritable</c> with a warning, as with Arrow). Java converted sequences only when they all had the same
    ''' length; the port converts them all. The results are plain lists (Java returned Arrow-backed lists).</item>
    ''' <item>Empty input gives empty output (Java threw an <c>IndexOutOfBoundsException</c>).</item>
    ''' <item>In try/catch mode (<see cref="LOG_ERROR_PROPERTY"/>) the records whose transform failed are dropped; the
    ''' Java filter kept only those records.</item>
    ''' <item><see cref="executeJoin"/> keeps every input record; Java dropped the records whose number of values
    ''' equals the number of join columns.</item>
    ''' </list>
    ''' </summary>
    Public Class LocalTransformExecutor

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(LocalTransformExecutor))

        ''' <summary>
        ''' The name of a boolean "system property" (kept by the internal <c>JavaSystem</c> helper, Java's
        ''' <c>System.getProperty</c>): when it is "true", transform errors are logged and the failing records dropped
        ''' instead of the exception being thrown.
        ''' </summary>
        Public Const LOG_ERROR_PROPERTY As String = "org.datavec.spark.transform.logerrors"

        ''' <summary>
        ''' Execute the specified TransformProcess with the given input data<br/>
        ''' Note: this method can only be used if the TransformProcess returns non-sequence data. For TransformProcesses
        ''' that return a sequence, use <see cref="executeToSequence"/>. Records whose number of values differs from the
        ''' number of columns of the initial schema are dropped (with a warning).
        ''' </summary>
        ''' <param name="inputWritables">   Input data to process </param>
        ''' <param name="transformProcess"> TransformProcess to execute </param>
        ''' <returns> Processed data </returns>
        Public Shared Function execute(ByVal inputWritables As IList(Of IList(Of WritableT)), ByVal transformProcess As TransformProcessT) As IList(Of IList(Of WritableT))
            If TypeOf transformProcess.getFinalSchema() Is SequenceSchemaT Then
                Throw New InvalidOperationException("Cannot return sequence data with this method")
            End If

            Dim numColumns As Integer = transformProcess.getInitialSchema().numColumns()
            Dim filteredSequence As IList(Of IList(Of WritableT)) = inputWritables.Where(Function(input) input.Count = numColumns).ToList()
            If filteredSequence.Count <> inputWritables.Count Then
                log.warn("Filtered out " & (inputWritables.Count - filteredSequence.Count) & " values")
            End If
            Return executeInternal(filteredSequence, Nothing, transformProcess).First
        End Function

        ''' <summary>
        ''' Execute the specified TransformProcess with the given input data<br/>
        ''' Note: this method can only be used if the TransformProcess
        ''' starts with non-sequential data,
        ''' but returns <i>sequence</i>
        ''' data (after grouping or converting to a sequence as one of the steps)
        ''' </summary>
        ''' <param name="inputWritables">   Input data to process </param>
        ''' <param name="transformProcess"> TransformProcess to execute </param>
        ''' <returns> Processed (sequence) data </returns>
        Public Shared Function executeToSequence(ByVal inputWritables As IList(Of IList(Of WritableT)), ByVal transformProcess As TransformProcessT) As IList(Of IList(Of IList(Of WritableT)))
            If Not (TypeOf transformProcess.getFinalSchema() Is SequenceSchemaT) Then
                Throw New InvalidOperationException("Cannot return non-sequence data with this method")
            End If

            Return executeInternal(inputWritables, Nothing, transformProcess).Second
        End Function

        ''' <summary>
        ''' Execute the specified TransformProcess with the given <i>sequence</i> input data<br/>
        ''' Note: this method can only be used if the TransformProcess starts with sequence data, but returns <i>non-sequential</i>
        ''' data (after reducing or converting sequential data to individual examples)
        ''' </summary>
        ''' <param name="inputSequence">    Input sequence data to process </param>
        ''' <param name="transformProcess"> TransformProcess to execute </param>
        ''' <returns> Processed (non-sequential) data </returns>
        Public Shared Function executeSequenceToSeparate(ByVal inputSequence As IList(Of IList(Of IList(Of WritableT))), ByVal transformProcess As TransformProcessT) As IList(Of IList(Of WritableT))
            If TypeOf transformProcess.getFinalSchema() Is SequenceSchemaT Then
                Throw New InvalidOperationException("Cannot return sequence data with this method")
            End If

            Return executeInternal(Nothing, inputSequence, transformProcess).First
        End Function

        ''' <summary>
        ''' Execute the specified TransformProcess with the given <i>sequence</i> input data<br/>
        ''' Note: this method can only be used if the TransformProcess starts with sequence data, and also returns sequence data.
        ''' If the final schema is not a sequence schema, each sequence is processed as a separate list of records.
        ''' </summary>
        ''' <param name="inputSequence">    Input sequence data to process </param>
        ''' <param name="transformProcess"> TransformProcess to execute </param>
        ''' <returns> Processed (sequence) data </returns>
        Public Shared Function executeSequenceToSequence(ByVal inputSequence As IList(Of IList(Of IList(Of WritableT))), ByVal transformProcess As TransformProcessT) As IList(Of IList(Of IList(Of WritableT)))
            If Not (TypeOf transformProcess.getFinalSchema() Is SequenceSchemaT) Then
                Dim ret As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))(inputSequence.Count)
                For Each timeStep As IList(Of IList(Of WritableT)) In inputSequence
                    ret.Add(executeInternal(timeStep, Nothing, transformProcess).First)
                Next timeStep

                Return ret
            End If

            Return executeInternal(Nothing, inputSequence, transformProcess).Second
        End Function

        ''' <summary>
        ''' Convert records of writables to records of strings (the <c>ToString()</c> of each value).
        ''' (Java returned an empty list: it filled a second list and returned the first.)
        ''' </summary>
        ''' <param name="stringInput"> the input records </param>
        ''' <param name="schema"> the schema of the records (not used) </param>
        ''' <returns> the converted records </returns>
        Public Shared Function convertWritableInputToString(ByVal stringInput As IList(Of IList(Of WritableT)), ByVal schema As SchemaT) As IList(Of IList(Of String))
            Dim ret As IList(Of IList(Of String)) = New List(Of IList(Of String))(stringInput.Count)
            For Each record As IList(Of WritableT) In stringInput
                ret.Add(recordToStrings(record))
            Next record
            Return ret
        End Function

        ''' <summary>
        ''' Convert records of strings to records of writables, based on the column types of the schema: Double, Float,
        ''' Integer and Long columns are parsed (as by <c>Text.toDouble</c> and friends), Time columns are parsed as epoch
        ''' milliseconds into a <c>LongWritable</c>, String and Categorical values become <c>Text</c> and Boolean values a
        ''' <c>BooleanWritable</c> ("true", ignoring case, is true). This does not use Arrow.
        ''' (Java returned an empty list, and silently dropped the Categorical and Boolean columns.)
        ''' </summary>
        ''' <param name="stringInput"> the string input </param>
        ''' <param name="schema"> the schema to use </param>
        ''' <returns> the converted records </returns>
        ''' <exception cref="NotSupportedException">for a Bytes or NDArray column</exception>
        Public Shared Function convertStringInput(ByVal stringInput As IList(Of IList(Of String)), ByVal schema As SchemaT) As IList(Of IList(Of WritableT))
            Dim ret As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(stringInput.Count)
            For Each record As IList(Of String) In stringInput
                ret.Add(stringsToRecord(record, schema))
            Next record
            Return ret
        End Function

        ''' <summary>
        ''' Convert sequences of writables to sequences of strings (the <c>ToString()</c> of each value).
        ''' </summary>
        ''' <param name="stringInput"> the input sequences </param>
        ''' <param name="schema"> the schema of the sequences (not used) </param>
        ''' <returns> the converted sequences </returns>
        Public Shared Function convertWritableInputToStringTimeSeries(ByVal stringInput As IList(Of IList(Of IList(Of WritableT))), ByVal schema As SchemaT) As IList(Of IList(Of IList(Of String)))
            Dim ret As IList(Of IList(Of IList(Of String))) = New List(Of IList(Of IList(Of String)))(stringInput.Count)
            For Each currInput As IList(Of IList(Of WritableT)) In stringInput
                Dim timeStepAdd As IList(Of IList(Of String)) = New List(Of IList(Of String))(currInput.Count)
                For Each record As IList(Of WritableT) In currInput
                    timeStepAdd.Add(recordToStrings(record))
                Next record
                ret.Add(timeStepAdd)
            Next currInput

            Return ret
        End Function

        ''' <summary>
        ''' Convert sequences of strings to sequences of writables, based on the column types of the schema (see
        ''' <see cref="convertStringInput"/>). This does not use Arrow.
        ''' </summary>
        ''' <param name="stringInput"> the string input </param>
        ''' <param name="schema"> the schema to use </param>
        ''' <returns> the converted sequences </returns>
        ''' <exception cref="NotSupportedException">for a Bytes or NDArray column</exception>
        Public Shared Function convertStringInputTimeSeries(ByVal stringInput As IList(Of IList(Of IList(Of String))), ByVal schema As SchemaT) As IList(Of IList(Of IList(Of WritableT)))
            Dim ret As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))(stringInput.Count)
            For Each currInput As IList(Of IList(Of String)) In stringInput
                Dim timeStepAdd As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(currInput.Count)
                For Each record As IList(Of String) In currInput
                    timeStepAdd.Add(stringsToRecord(record, schema))
                Next record
                ret.Add(timeStepAdd)
            Next currInput

            Return ret
        End Function

        ''' <summary>
        ''' Returns true if the executor is in try/catch mode: the "system property" <see cref="LOG_ERROR_PROPERTY"/> is
        ''' "true" (ignoring case), as Java's <c>Boolean.getBoolean</c> checks.
        ''' </summary>
        Public Shared Function isTryCatch() As Boolean
            Dim value As String = JavaSystem.getProperty(LOG_ERROR_PROPERTY)
            Return value IsNot Nothing AndAlso value.Equals("true", StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>True if the executor is in try/catch mode (see <see cref="isTryCatch"/>).</summary>
        Public Shared ReadOnly Property TryCatch As Boolean
            Get
                Return isTryCatch()
            End Get
        End Property

        Private Shared Function executeInternal(ByVal inputWritables As IList(Of IList(Of WritableT)), ByVal inputSequence As IList(Of IList(Of IList(Of WritableT))), ByVal transformProcess As TransformProcessT) As Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of IList(Of WritableT))))
            Dim currentWritables As IList(Of IList(Of WritableT)) = inputWritables
            Dim currentSequence As IList(Of IList(Of IList(Of WritableT))) = inputSequence

            Dim dataActions As IList(Of DataActionT) = transformProcess.getActionList()
            Dim numColumns As Integer = transformProcess.getInitialSchema().numColumns()
            If inputWritables IsNot Nothing Then
                If inputWritables.Count > 0 Then
                    Dim first As IList(Of WritableT) = inputWritables(0)
                    If first.Count <> numColumns Then
                        Throw New InvalidOperationException("Input data number of columns (" & first.Count & ") does not match the number of columns for the transform process (" & numColumns & ")")
                    End If
                End If
            Else
                If inputSequence.Count > 0 Then
                    Dim firstSeq As IList(Of IList(Of WritableT)) = inputSequence(0)
                    If firstSeq.Count > 0 AndAlso firstSeq(0).Count <> numColumns Then
                        Throw New InvalidOperationException("Input sequence data number of columns (" & firstSeq(0).Count & ") does not match the number of columns for the transform process (" & numColumns & ")")
                    End If
                End If
            End If

            For Each d As DataActionT In dataActions
                If d.getTransform() IsNot Nothing Then
                    Dim t As TransformT = d.getTransform()
                    If currentWritables IsNot Nothing Then
                        Dim fn As New LocalTransformFunction(t)
                        If isTryCatch() Then
                            Dim emptyRecord As New EmptyRecordFunction()
                            currentWritables = currentWritables.Select(Function(input) fn.apply(input)).Where(Function(output) Not emptyRecord.apply(output)).ToList()
                        Else
                            currentWritables = currentWritables.Select(Function(input) fn.apply(input)).ToList()
                        End If
                    Else
                        Dim fn As New LocalSequenceTransformFunction(t)
                        If isTryCatch() Then
                            Dim emptySequence As New SequenceEmptyRecordFunction()
                            currentSequence = currentSequence.Select(Function(input) fn.apply(input)).Where(Function(output) Not emptySequence.apply(output)).ToList()
                        Else
                            currentSequence = currentSequence.Select(Function(input) fn.apply(input)).ToList()
                        End If
                    End If

                ElseIf d.getFilter() IsNot Nothing Then
                    Dim f As FilterT = d.getFilter()
                    If currentWritables IsNot Nothing Then
                        Dim localFilterFunction As New LocalFilterFunction(f)
                        currentWritables = currentWritables.Where(Function(input) localFilterFunction.apply(input)).ToList()
                    Else
                        Dim localSequenceFilterFunction As New LocalSequenceFilterFunction(f)
                        currentSequence = currentSequence.Where(Function(input) localSequenceFilterFunction.apply(input)).ToList()
                    End If

                ElseIf d.getConvertToSequence() IsNot Nothing Then
                    Dim cts As ConvertToSequenceT = d.getConvertToSequence()
                    If currentWritables Is Nothing Then
                        Throw New InvalidOperationException("Cannot execute ConvertToSequence operation: current writables are null (data is already a sequence)")
                    End If

                    If cts.isSingleStepSequencesMode() Then
                        'Edge case: create a sequence from each example, by treating each value as a sequence of length 1
                        Dim convertToSequenceLengthOne As New ConvertToSequenceLengthOne()
                        currentSequence = currentWritables.Select(Function(input) convertToSequenceLengthOne.apply(input)).ToList()
                        currentWritables = Nothing
                    Else
                        'Standard case: group by key, then sort each group into a sequence
                        Dim schema As SchemaT = cts.getInputSchema()
                        Dim colIdxs() As Integer = schema.getIndexOfColumns(cts.getKeyColumns())
                        Dim localMapToPairByMultipleColumnsFunction As New LocalMapToPairByMultipleColumnsFunction(colIdxs)
                        Dim withKey As IList(Of Pair(Of IList(Of WritableT), IList(Of WritableT))) = currentWritables.Select(Function(input) localMapToPairByMultipleColumnsFunction.apply(input)).ToList()

                        Dim localGroupToSequenceFunction As New LocalGroupToSequenceFunction(cts.getComparator())
                        currentSequence = withKey.GroupBy(Function(p) p.First, Function(p) p.Second, WritableListComparer.Instance) _
                            .Select(Function(group) localGroupToSequenceFunction.apply(group.ToList())).ToList()
                        currentWritables = Nothing
                    End If

                ElseIf d.getConvertFromSequence() IsNot Nothing Then
                    If currentSequence Is Nothing Then
                        Throw New InvalidOperationException("Cannot execute ConvertFromSequence operation: current sequence is null")
                    End If

                    currentWritables = currentSequence.SelectMany(Function(input) input).ToList()
                    currentSequence = Nothing

                ElseIf d.getSequenceSplit() IsNot Nothing Then
                    Dim sequenceSplit As SequenceSplitT = d.getSequenceSplit()
                    If currentSequence Is Nothing Then
                        Throw New InvalidOperationException("Error during execution of SequenceSplit: currentSequence is null")
                    End If

                    Dim sequenceSplitFunction As New SequenceSplitFunction(sequenceSplit)
                    currentSequence = currentSequence.SelectMany(Function(input) sequenceSplitFunction.call(input)).ToList()

                ElseIf d.getReducer() IsNot Nothing Then
                    Dim reducer As IAssociativeReducerT = d.getReducer()
                    If currentWritables Is Nothing Then
                        Throw New InvalidOperationException("Error during execution of reduction: current writables are null. " & "Trying to execute a reduce operation on a sequence?")
                    End If

                    Dim mapToPairForReducerFunction As New MapToPairForReducerFunction(reducer)
                    Dim pairs As IList(Of Pair(Of String, IList(Of WritableT))) = currentWritables.Select(Function(input) mapToPairForReducerFunction.apply(input)).ToList()

                    'One aggregation per key, fed with the records of that key in input order
                    Dim reduced As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                    For Each group As IGrouping(Of String, IList(Of WritableT)) In pairs.GroupBy(Function(p) p.First, Function(p) p.Second, StringComparer.Ordinal)
                        Dim op As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()
                        For Each value As IList(Of WritableT) In group
                            op.accept(value)
                        Next value
                        reduced.Add(op.get())
                    Next group
                    currentWritables = reduced

                ElseIf d.getCalculateSortedRank() IsNot Nothing Then
                    Dim csr As CalculateSortedRankT = d.getCalculateSortedRank()
                    If currentWritables Is Nothing Then
                        Throw New InvalidOperationException("Error during execution of CalculateSortedRank: current writables are null. " & "Trying to execute a CalculateSortedRank operation on a sequence? (not currently supported)")
                    End If

                    Dim comparator As WritableComparatorT = csr.getComparator()
                    Dim sortColumn As String = csr.getSortOnColumn()
                    Dim sortColumnIdx As Integer = csr.InputSchema.getIndexOfColumn(sortColumn)
                    Dim ascending As Boolean = csr.isAscending()
                    'NOTE: this likely isn't the most efficient implementation.
                    Dim columnAsKeyPairFunction As New ColumnAsKeyPairFunction(sortColumnIdx)
                    Dim pairRDD As IList(Of Pair(Of WritableT, IList(Of WritableT))) = currentWritables.Select(Function(input) columnAsKeyPairFunction.apply(input)).ToList()
                    'Stable sorts (like Java's sorted stream): ties keep their input order
                    If ascending Then
                        pairRDD = pairRDD.OrderBy(Function(p) p.First, comparator).ToList()
                    Else
                        pairRDD = pairRDD.OrderByDescending(Function(p) p.First, comparator).ToList()
                    End If

                    Dim unzip As New UnzipForCalculateSortedRankFunction()
                    currentWritables = pairRDD.Select(Function(p, index) unzip.apply(Pair.of(p, CLng(index)))).ToList()
                Else
                    Throw New Exception("Unknown/not implemented action: " & d.ToString())
                End If
            Next d

            Dim finalSchema As SchemaT = transformProcess.getFinalSchema()
            If currentSequence IsNot Nothing Then
                Return New Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of IList(Of WritableT))))(Nothing, toSchemaTypesSequences(currentSequence, finalSchema))
            Else
                Return New Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of IList(Of WritableT))))(toSchemaTypes(currentWritables, finalSchema), Nothing)
            End If
        End Function

        ''' <summary>
        ''' Execute a join on the specified data. Joined records come out grouped by key, in the order in which the keys
        ''' first appear in the left data and then (keys present only on the right) in the right data; within a key, in
        ''' the order of the left records and then of the right records.
        ''' </summary>
        ''' <param name="join">  Join to execute </param>
        ''' <param name="left">  Left data for join </param>
        ''' <param name="right"> Right data for join </param>
        ''' <returns> Joined data </returns>
        Public Shared Function executeJoin(ByVal join As JoinT, ByVal left As IList(Of IList(Of WritableT)), ByVal right As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT))

            Dim leftColumnNames() As String = join.getJoinColumnsLeft()
            Dim leftColumnIndexes(leftColumnNames.Length - 1) As Integer
            For i As Integer = 0 To leftColumnNames.Length - 1
                leftColumnIndexes(i) = join.getLeftSchema().getIndexOfColumn(leftColumnNames(i))
            Next i
            Dim extractKeysFunction1 As New ExtractKeysFunction(leftColumnIndexes)

            ' Java also dropped every record with as many values as there are join columns
            ' (filter(input -> input.size() != leftColumnNames.length)), which loses all the records of a side that
            ' holds only its key columns; the Spark executor has no such filter, and neither has this port.
            Dim leftJV As IList(Of Pair(Of IList(Of WritableT), IList(Of WritableT))) = left.Select(Function(input) extractKeysFunction1.apply(input)).ToList()

            Dim rightColumnNames() As String = join.getJoinColumnsRight()
            Dim rightColumnIndexes(rightColumnNames.Length - 1) As Integer
            For i As Integer = 0 To rightColumnNames.Length - 1
                rightColumnIndexes(i) = join.getRightSchema().getIndexOfColumn(rightColumnNames(i))
            Next i

            Dim extractKeysFunction As New ExtractKeysFunction(rightColumnIndexes)
            Dim rightJV As IList(Of Pair(Of IList(Of WritableT), IList(Of WritableT))) = right.Select(Function(input) extractKeysFunction.apply(input)).ToList()

            Dim cogroupedJV As IList(Of Pair(Of IList(Of WritableT), Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT))))) = cogroup(leftJV, rightJV)
            Dim executeJoinFromCoGroupFlatMapFunction As New ExecuteJoinFromCoGroupFlatMapFunction(join)
            Dim ret As IList(Of IList(Of WritableT)) = cogroupedJV.SelectMany(Function(input) executeJoinFromCoGroupFlatMapFunction.call(input)).ToList()

            Return toSchemaTypes(ret, join.OutputSchema)
        End Function

        ''' <summary>
        ''' Groups the (key, record) pairs of both sides by key (compared by content, like Java's <c>List.equals</c>):
        ''' one (key, (left records, right records)) entry per key, in the order in which the keys first appear in the left
        ''' and then in the right pairs (Java: <c>FunctionalUtils.cogroup</c>, hash map order).
        ''' </summary>
        Private Shared Function cogroup(ByVal left As IList(Of Pair(Of IList(Of WritableT), IList(Of WritableT))), ByVal right As IList(Of Pair(Of IList(Of WritableT), IList(Of WritableT)))) As IList(Of Pair(Of IList(Of WritableT), Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT)))))
            Dim ret As New List(Of Pair(Of IList(Of WritableT), Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT)))))()
            Dim byKey As New Dictionary(Of IList(Of WritableT), Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT))))(WritableListComparer.Instance)
            For side As Integer = 0 To 1
                For Each p As Pair(Of IList(Of WritableT), IList(Of WritableT)) In If(side = 0, left, right)
                    Dim lists As Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT))) = Nothing
                    If Not byKey.TryGetValue(p.First, lists) Then
                        lists = New Pair(Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT)))(New List(Of IList(Of WritableT))(), New List(Of IList(Of WritableT))())
                        byKey.Add(p.First, lists)
                        ret.Add(Pair.of(p.First, lists))
                    End If
                    If side = 0 Then
                        lists.First.Add(p.Second)
                    Else
                        lists.Second.Add(p.Second)
                    End If
                Next p
            Next side
            Return ret
        End Function

        ''' <summary>The values of a record as strings.</summary>
        Private Shared Function recordToStrings(ByVal record As IList(Of WritableT)) As IList(Of String)
            Dim recordAdd As IList(Of String) = New List(Of String)(record.Count)
            For Each w As WritableT In record
                recordAdd.Add(w.ToString())
            Next w
            Return recordAdd
        End Function

        ''' <summary>Parses the strings of a record into writables of the schema's column types.</summary>
        Private Shared Function stringsToRecord(ByVal record As IList(Of String), ByVal schema As SchemaT) As IList(Of WritableT)
            Dim recordAdd As IList(Of WritableT) = New List(Of WritableT)(record.Count)
            For k As Integer = 0 To record.Count - 1
                Dim value As New TextT(record(k))
                Dim columnType As ColumnTypeT = schema.getType(k)
                Select Case columnType
                    Case ColumnTypeT.Double
                        recordAdd.Add(New DoubleWritableT(value.toDouble()))
                    Case ColumnTypeT.Float
                        recordAdd.Add(New FloatWritableT(value.toFloat()))
                    Case ColumnTypeT.Integer
                        recordAdd.Add(New IntWritableT(value.toInt()))
                    Case ColumnTypeT.Long, ColumnTypeT.Time
                        recordAdd.Add(New LongWritableT(value.toLong()))
                    Case ColumnTypeT.String, ColumnTypeT.Categorical
                        recordAdd.Add(value)
                    Case ColumnTypeT.Boolean
                        recordAdd.Add(New BooleanWritableT(String.Equals(record(k), "true", StringComparison.OrdinalIgnoreCase)))
                    Case Else
                        Throw New NotSupportedException("Cannot convert a string to a value of a " & columnType.ToString() & " column (column " & k & ")")
                End Select
            Next k
            Return recordAdd
        End Function

        ''' <summary>
        ''' Converts every value of the records to the writable type of its column (what Java's round trip through Arrow
        ''' columns did). Values past the last column of the schema are dropped.
        ''' </summary>
        Private Shared Function toSchemaTypes(ByVal records As IList(Of IList(Of WritableT)), ByVal schema As SchemaT) As IList(Of IList(Of WritableT))
            Dim columnTypes As IList(Of ColumnTypeT) = schema.getColumnTypes()
            Dim ret As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(records.Count)
            Dim row As Integer = 0
            For Each record As IList(Of WritableT) In records
                ret.Add(toSchemaTypes(record, columnTypes, row))
                row += 1
            Next record
            Return ret
        End Function

        ''' <summary>
        ''' Converts every value of the sequences to the writable type of its column (what Java's round trip through
        ''' Arrow columns did for sequences of equal length).
        ''' </summary>
        Private Shared Function toSchemaTypesSequences(ByVal sequences As IList(Of IList(Of IList(Of WritableT))), ByVal schema As SchemaT) As IList(Of IList(Of IList(Of WritableT)))
            Dim columnTypes As IList(Of ColumnTypeT) = schema.getColumnTypes()
            Dim ret As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))(sequences.Count)
            Dim row As Integer = 0
            For Each sequence As IList(Of IList(Of WritableT)) In sequences
                Dim converted As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(sequence.Count)
                For Each record As IList(Of WritableT) In sequence
                    converted.Add(toSchemaTypes(record, columnTypes, row))
                    row += 1
                Next record
                ret.Add(converted)
            Next sequence
            Return ret
        End Function

        Private Shared Function toSchemaTypes(ByVal record As IList(Of WritableT), ByVal columnTypes As IList(Of ColumnTypeT), ByVal row As Integer) As IList(Of WritableT)
            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(columnTypes.Count)
            For j As Integer = 0 To columnTypes.Count - 1
                [out].Add(toColumnType(record(j), columnTypes(j), row))
            Next j
            Return [out]
        End Function

        ''' <summary>
        ''' Converts one value to the writable type of a column: <c>IntWritable</c> for Integer (<c>toInt()</c>),
        ''' <c>LongWritable</c> for Long and Time (<c>toLong()</c>), <c>FloatWritable</c>, <c>DoubleWritable</c>,
        ''' <c>BooleanWritable</c> (<c>toInt() &lt;&gt; 0</c>) and <c>Text</c> for String and Categorical (<c>ToString()</c>).
        ''' Bytes and NDArray values are returned unchanged. A null value stays a <c>NullWritable</c>; a value that cannot be
        ''' converted becomes a <c>NullWritable</c> and a warning is logged, as Arrow's conversion did.
        ''' </summary>
        Private Shared Function toColumnType(ByVal w As WritableT, ByVal columnType As ColumnTypeT, ByVal row As Integer) As WritableT
            If w Is Nothing OrElse TypeOf w Is NullWritableT Then
                Return NullWritableT.INSTANCE
            End If
            Try
                Select Case columnType
                    Case ColumnTypeT.Integer
                        Return New IntWritableT(w.toInt())
                    Case ColumnTypeT.Long, ColumnTypeT.Time
                        Return New LongWritableT(w.toLong())
                    Case ColumnTypeT.Float
                        Return New FloatWritableT(w.toFloat())
                    Case ColumnTypeT.Double
                        Return New DoubleWritableT(w.toDouble())
                    Case ColumnTypeT.Boolean
                        Return New BooleanWritableT(w.toInt() <> 0)
                    Case ColumnTypeT.String, ColumnTypeT.Categorical
                        Return New TextT(w.ToString())
                    Case Else
                        Return w
                End Select
            Catch e As Exception
                log.warn("Unable to set value at row " & row & ": " & e.Message)
                Return NullWritableT.INSTANCE
            End Try
        End Function

        ''' <summary>
        ''' Compares lists of writables by content (Java's <c>List.equals</c>/<c>hashCode</c>), so that lists can be used as
        ''' dictionary and grouping keys.
        ''' </summary>
        Private NotInheritable Class WritableListComparer
            Implements IEqualityComparer(Of IList(Of WritableT))

            Public Shared ReadOnly Instance As New WritableListComparer()

            Public Overloads Function Equals(ByVal x As IList(Of WritableT), ByVal y As IList(Of WritableT)) As Boolean Implements IEqualityComparer(Of IList(Of WritableT)).Equals
                If x Is y Then
                    Return True
                End If
                If x Is Nothing OrElse y Is Nothing OrElse x.Count <> y.Count Then
                    Return False
                End If
                For i As Integer = 0 To x.Count - 1
                    If Not Object.Equals(x(i), y(i)) Then
                        Return False
                    End If
                Next i
                Return True
            End Function

            Public Overloads Function GetHashCode(ByVal obj As IList(Of WritableT)) As Integer Implements IEqualityComparer(Of IList(Of WritableT)).GetHashCode
                Dim hash As Integer = 1
                For Each w As WritableT In obj
                    hash = 31 * hash + If(w Is Nothing, 0, w.GetHashCode())
                Next w
                Return hash
            End Function

        End Class

    End Class

End Namespace
