Imports System
Imports System.Collections.Generic
Imports Indexed = com.codepoetics.protonpack.Indexed
Imports StreamUtils = com.codepoetics.protonpack.StreamUtils
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BufferAllocator = org.apache.arrow.memory.BufferAllocator
Imports RootAllocator = org.apache.arrow.memory.RootAllocator
Imports FieldVector = org.apache.arrow.vector.FieldVector
Imports DataAction = org.datavec.api.transform.DataAction
Imports Transform = org.datavec.api.transform.Transform
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Filter = org.datavec.api.transform.filter.Filter
Imports Join = org.datavec.api.transform.join.Join
Imports org.datavec.api.transform.ops
Imports CalculateSortedRank = org.datavec.api.transform.rank.CalculateSortedRank
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports ConvertToSequence = org.datavec.api.transform.sequence.ConvertToSequence
Imports SequenceSplit = org.datavec.api.transform.sequence.SequenceSplit
Imports org.datavec.api.writable
Imports ArrowConverter = org.datavec.arrow.ArrowConverter
Imports EmptyRecordFunction = org.datavec.local.transforms.functions.EmptyRecordFunction
Imports ExecuteJoinFromCoGroupFlatMapFunction = org.datavec.local.transforms.join.ExecuteJoinFromCoGroupFlatMapFunction
Imports ExtractKeysFunction = org.datavec.local.transforms.join.ExtractKeysFunction
Imports ColumnAsKeyPairFunction = org.datavec.local.transforms.misc.ColumnAsKeyPairFunction
Imports UnzipForCalculateSortedRankFunction = org.datavec.local.transforms.rank.UnzipForCalculateSortedRankFunction
Imports MapToPairForReducerFunction = org.datavec.local.transforms.reduce.MapToPairForReducerFunction
Imports org.datavec.local.transforms.sequence
Imports LocalTransformFunction = org.datavec.local.transforms.transform.LocalTransformFunction
Imports SequenceSplitFunction = org.datavec.local.transforms.transform.SequenceSplitFunction
Imports LocalFilterFunction = org.datavec.local.transforms.transform.filter.LocalFilterFunction
Imports org.nd4j.common.function
Imports FunctionalUtils = org.nd4j.common.function.FunctionalUtils
Imports org.nd4j.common.primitives

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

Namespace org.datavec.local.transforms



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LocalTransformExecutor
	Public Class LocalTransformExecutor
		'a boolean jvm argument that when the system property is true
		'will cause some functions to invoke a try catch block and just log errors
		'returning empty records
		Public Const LOG_ERROR_PROPERTY As String = "org.datavec.spark.transform.logerrors"

		Private Shared bufferAllocator As BufferAllocator = New RootAllocator(Long.MaxValue)

		''' <summary>
		''' Execute the specified TransformProcess with the given input data<br>
		''' Note: this method can only be used if the TransformProcess returns non-sequence data. For TransformProcesses
		''' that return a sequence, use <seealso cref="executeToSequence(List, TransformProcess)"/>
		''' </summary>
		''' <param name="inputWritables">   Input data to process </param>
		''' <param name="transformProcess"> TransformProcess to execute </param>
		''' <returns> Processed data </returns>
		Public Shared Function execute(ByVal inputWritables As IList(Of IList(Of Writable)), ByVal transformProcess As TransformProcess) As IList(Of IList(Of Writable))
			If TypeOf transformProcess.FinalSchema Is SequenceSchema Then
				Throw New System.InvalidOperationException("Cannot return sequence data with this method")

			End If

			Dim filteredSequence As IList(Of IList(Of Writable)) = inputWritables.Where(Function(input) input.size() = transformProcess.getInitialSchema().numColumns()).ToList()
			If filteredSequence.Count <> inputWritables.Count Then
				log.warn("Filtered out " & (inputWritables.Count - filteredSequence.Count) & " values")
			End If
			Return execute(filteredSequence, Nothing, transformProcess).First
		End Function

		''' <summary>
		''' Execute the specified TransformProcess with the given input data<br>
		''' Note: this method can only be used if the TransformProcess
		''' starts with non-sequential data,
		''' but returns <it>sequence</it>
		''' data (after grouping or converting to a sequence as one of the steps)
		''' </summary>
		''' <param name="inputWritables">   Input data to process </param>
		''' <param name="transformProcess"> TransformProcess to execute </param>
		''' <returns> Processed (sequence) data </returns>
		Public Shared Function executeToSequence(ByVal inputWritables As IList(Of IList(Of Writable)), ByVal transformProcess As TransformProcess) As IList(Of IList(Of IList(Of Writable)))
			If Not (TypeOf transformProcess.FinalSchema Is SequenceSchema) Then
				Throw New System.InvalidOperationException("Cannot return non-sequence data with this method")
			End If

			Return execute(inputWritables, Nothing, transformProcess).Second
		End Function

		''' <summary>
		''' Execute the specified TransformProcess with the given <i>sequence</i> input data<br>
		''' Note: this method can only be used if the TransformProcess starts with sequence data, but returns <i>non-sequential</i>
		''' data (after reducing or converting sequential data to individual examples)
		''' </summary>
		''' <param name="inputSequence">    Input sequence data to process </param>
		''' <param name="transformProcess"> TransformProcess to execute </param>
		''' <returns> Processed (non-sequential) data </returns>
		Public Shared Function executeSequenceToSeparate(ByVal inputSequence As IList(Of IList(Of IList(Of Writable))), ByVal transformProcess As TransformProcess) As IList(Of IList(Of Writable))
			If TypeOf transformProcess.FinalSchema Is SequenceSchema Then
				Throw New System.InvalidOperationException("Cannot return sequence data with this method")
			End If

			Return execute(Nothing, inputSequence, transformProcess).First
		End Function

		''' <summary>
		''' Execute the specified TransformProcess with the given <i>sequence</i> input data<br>
		''' Note: this method can only be used if the TransformProcess starts with sequence data, and also returns sequence data
		''' </summary>
		''' <param name="inputSequence">    Input sequence data to process </param>
		''' <param name="transformProcess"> TransformProcess to execute </param>
		''' <returns> Processed (non-sequential) data </returns>
		Public Shared Function executeSequenceToSequence(ByVal inputSequence As IList(Of IList(Of IList(Of Writable))), ByVal transformProcess As TransformProcess) As IList(Of IList(Of IList(Of Writable)))
			If Not (TypeOf transformProcess.FinalSchema Is SequenceSchema) Then
				Dim ret As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))(inputSequence.Count)
				For Each timeStep As IList(Of IList(Of Writable)) In inputSequence
					ret.Add(execute(timeStep,Nothing, transformProcess).First)
				Next timeStep

				Return ret
			End If

			Return execute(Nothing, inputSequence, transformProcess).Second
		End Function



		''' <summary>
		''' Convert a string time series to
		''' the proper writable set based on the schema.
		''' Note that this does not use arrow.
		''' This just uses normal writable objects.
		''' </summary>
		''' <param name="stringInput"> the string input </param>
		''' <param name="schema"> the schema to use </param>
		''' <returns> the converted records </returns>
		Public Shared Function convertWritableInputToString(ByVal stringInput As IList(Of IList(Of Writable)), ByVal schema As Schema) As IList(Of IList(Of String))
			Dim ret As IList(Of IList(Of String)) = New List(Of IList(Of String))()
			Dim timeStepAdd As IList(Of IList(Of String)) = New List(Of IList(Of String))()
			For j As Integer = 0 To stringInput.Count - 1
				Dim record As IList(Of Writable) = stringInput(j)
				Dim recordAdd As IList(Of String) = New List(Of String)()
				For k As Integer = 0 To record.Count - 1
					recordAdd.Add(record(k).ToString())
				Next k

				timeStepAdd.Add(recordAdd)
			Next j


			Return ret
		End Function


		''' <summary>
		''' Convert a string time series to
		''' the proper writable set based on the schema.
		''' Note that this does not use arrow.
		''' This just uses normal writable objects.
		''' </summary>
		''' <param name="stringInput"> the string input </param>
		''' <param name="schema"> the schema to use </param>
		''' <returns> the converted records </returns>
		Public Shared Function convertStringInput(ByVal stringInput As IList(Of IList(Of String)), ByVal schema As Schema) As IList(Of IList(Of Writable))
			Dim ret As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim timeStepAdd As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For j As Integer = 0 To stringInput.Count - 1
				Dim record As IList(Of String) = stringInput(j)
				Dim recordAdd As IList(Of Writable) = New List(Of Writable)()
				For k As Integer = 0 To record.Count - 1
					Select Case schema.getType(k)
						Case Double?
							recordAdd.Add(New DoubleWritable(Double.Parse(record(k))))
						Case Single?
							recordAdd.Add(New FloatWritable(Single.Parse(record(k))))
						Case Integer?
							recordAdd.Add(New IntWritable(Integer.Parse(record(k))))
						Case Long?
							recordAdd.Add(New LongWritable(Long.Parse(record(k))))
						Case String
							recordAdd.Add(New Text(record(k)))
						Case Time
							recordAdd.Add(New LongWritable(Long.Parse(record(k))))

					End Select
				Next k

				timeStepAdd.Add(recordAdd)
			Next j


			Return ret
		End Function




		''' <summary>
		''' Convert a string time series to
		''' the proper writable set based on the schema.
		''' Note that this does not use arrow.
		''' This just uses normal writable objects.
		''' </summary>
		''' <param name="stringInput"> the string input </param>
		''' <param name="schema"> the schema to use </param>
		''' <returns> the converted records </returns>
		Public Shared Function convertWritableInputToStringTimeSeries(ByVal stringInput As IList(Of IList(Of IList(Of Writable))), ByVal schema As Schema) As IList(Of IList(Of IList(Of String)))
			Dim ret As IList(Of IList(Of IList(Of String))) = New List(Of IList(Of IList(Of String)))()
			For i As Integer = 0 To stringInput.Count - 1
				Dim currInput As IList(Of IList(Of Writable)) = stringInput(i)
				Dim timeStepAdd As IList(Of IList(Of String)) = New List(Of IList(Of String))()
				For j As Integer = 0 To currInput.Count - 1
					Dim record As IList(Of Writable) = currInput(j)
					Dim recordAdd As IList(Of String) = New List(Of String)()
					For k As Integer = 0 To record.Count - 1
						recordAdd.Add(record(k).ToString())
					Next k

					timeStepAdd.Add(recordAdd)
				Next j

				ret.Add(timeStepAdd)
			Next i

			Return ret
		End Function


		''' <summary>
		''' Convert a string time series to
		''' the proper writable set based on the schema.
		''' Note that this does not use arrow.
		''' This just uses normal writable objects.
		''' </summary>
		''' <param name="stringInput"> the string input </param>
		''' <param name="schema"> the schema to use </param>
		''' <returns> the converted records </returns>
		Public Shared Function convertStringInputTimeSeries(ByVal stringInput As IList(Of IList(Of IList(Of String))), ByVal schema As Schema) As IList(Of IList(Of IList(Of Writable)))
			Dim ret As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			For i As Integer = 0 To stringInput.Count - 1
				Dim currInput As IList(Of IList(Of String)) = stringInput(i)
				Dim timeStepAdd As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
				For j As Integer = 0 To currInput.Count - 1
					Dim record As IList(Of String) = currInput(j)
					Dim recordAdd As IList(Of Writable) = New List(Of Writable)()
					For k As Integer = 0 To record.Count - 1
						Select Case schema.getType(k)
							Case Double?
								recordAdd.Add(New DoubleWritable(Double.Parse(record(k))))
							Case Single?
								recordAdd.Add(New FloatWritable(Single.Parse(record(k))))
							Case Integer?
								recordAdd.Add(New IntWritable(Integer.Parse(record(k))))
							Case Long?
								recordAdd.Add(New LongWritable(Long.Parse(record(k))))
							Case String
								recordAdd.Add(New Text(record(k)))
							Case Time
								recordAdd.Add(New LongWritable(Long.Parse(record(k))))

						End Select
					Next k

					timeStepAdd.Add(recordAdd)
				Next j

				ret.Add(timeStepAdd)
			Next i

			Return ret
		End Function

		''' <summary>
		''' Returns true if the executor
		''' is in try catch mode.
		''' @return
		''' </summary>
		Public Shared ReadOnly Property TryCatch As Boolean
			Get
				Return Boolean.getBoolean(LOG_ERROR_PROPERTY)
			End Get
		End Property

		Private Shared Function execute(ByVal inputWritables As IList(Of IList(Of Writable)), ByVal inputSequence As IList(Of IList(Of IList(Of Writable))), ByVal sequence As TransformProcess) As Pair(Of IList(Of IList(Of Writable)), IList(Of IList(Of IList(Of Writable))))
			Dim currentWritables As IList(Of IList(Of Writable)) = inputWritables
			Dim currentSequence As IList(Of IList(Of IList(Of Writable))) = inputSequence

			Dim dataActions As IList(Of DataAction) = sequence.getActionList()
			If inputWritables IsNot Nothing Then
				Dim first As IList(Of Writable) = inputWritables(0)
				If first.Count <> sequence.getInitialSchema().numColumns() Then
					Throw New System.InvalidOperationException("Input data number of columns (" & first.Count & ") does not match the number of columns for the transform process (" & sequence.getInitialSchema().numColumns() & ")")
				End If
			Else
				Dim firstSeq As IList(Of IList(Of Writable)) = inputSequence(0)
				If firstSeq.Count > 0 AndAlso firstSeq(0).Count <> sequence.getInitialSchema().numColumns() Then
					Throw New System.InvalidOperationException("Input sequence data number of columns (" & firstSeq(0).Count & ") does not match the number of columns for the transform process (" & sequence.getInitialSchema().numColumns() & ")")
				End If
			End If


			For Each d As DataAction In dataActions
				'log.info("Starting execution of stage {} of {}", count, dataActions.size());     //

				If d.getTransform() IsNot Nothing Then
					Dim t As Transform = d.getTransform()
					If currentWritables IsNot Nothing Then
						Dim [function] As [Function](Of IList(Of Writable), IList(Of Writable)) = New LocalTransformFunction(t)
						If TryCatch Then
							currentWritables = currentWritables.Select(Function(input) [function].apply(input)).Where(Function(input) (New EmptyRecordFunction()).apply(input)).ToList()
						Else
							currentWritables = currentWritables.Select(Function(input) [function].apply(input)).ToList()
						End If
					Else
						Dim [function] As [Function](Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable))) = New LocalSequenceTransformFunction(t)
						If TryCatch Then
							currentSequence = currentSequence.Select(Function(input) [function].apply(input)).Where(Function(input) (New SequenceEmptyRecordFunction()).apply(input)).ToList()
						Else
							currentSequence = currentSequence.Select(Function(input) [function].apply(input)).ToList()
						End If


					End If
				ElseIf d.getFilter() IsNot Nothing Then
					'Filter
					Dim f As Filter = d.getFilter()
					If currentWritables IsNot Nothing Then
						Dim localFilterFunction As New LocalFilterFunction(f)
						currentWritables = currentWritables.Where(Function(input) localFilterFunction.apply(input)).ToList()
					Else
						Dim localSequenceFilterFunction As New LocalSequenceFilterFunction(f)
						currentSequence = currentSequence.Where(Function(input) localSequenceFilterFunction.apply(input)).ToList()
					End If

				ElseIf d.getConvertToSequence() IsNot Nothing Then
					'Convert to a sequence...
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.datavec.api.transform.sequence.ConvertToSequence cts = d.getConvertToSequence();
					Dim cts As ConvertToSequence = d.getConvertToSequence()

					If cts.isSingleStepSequencesMode() Then
						Dim convertToSequenceLengthOne As New ConvertToSequenceLengthOne()
						'Edge case: create a sequence from each example, by treating each value as a sequence of length 1
						currentSequence = currentWritables.Select(Function(input) convertToSequenceLengthOne.apply(input)).ToList()
						currentWritables = Nothing
					Else
						'Standard case: join by key
						'First: convert to PairRDD
						Dim schema As Schema = cts.getInputSchema()
						Dim colIdxs() As Integer = schema.getIndexOfColumns(cts.getKeyColumns())
						Dim localMapToPairByMultipleColumnsFunction As New LocalMapToPairByMultipleColumnsFunction(colIdxs)
						Dim withKey As IList(Of Pair(Of IList(Of Writable), IList(Of Writable))) = currentWritables.Select(Function(inputSequence2) localMapToPairByMultipleColumnsFunction.apply(inputSequence2)).ToList()


						Dim collect As IDictionary(Of IList(Of Writable), IList(Of IList(Of Writable))) = FunctionalUtils.groupByKey(withKey)
						Dim localGroupToSequenceFunction As New LocalGroupToSequenceFunction(cts.getComparator())
						'Now: convert to a sequence...
						currentSequence = collect.SetOfKeyValuePairs().Select(Function(input) input.getValue()).Select(Function(input) localGroupToSequenceFunction.apply(input)).ToList()

						currentWritables = Nothing
					End If
				ElseIf d.getConvertFromSequence() IsNot Nothing Then
					'Convert from sequence...

					If currentSequence Is Nothing Then
						Throw New System.InvalidOperationException("Cannot execute ConvertFromSequence operation: current sequence is null")
					End If

					currentWritables = currentSequence.stream().flatMap(Function(input) input.stream()).collect(toList())
					currentSequence = Nothing
				ElseIf d.getSequenceSplit() IsNot Nothing Then
					Dim sequenceSplit As SequenceSplit = d.getSequenceSplit()
					If currentSequence Is Nothing Then
						Throw New System.InvalidOperationException("Error during execution of SequenceSplit: currentSequence is null")
					End If
					Dim sequenceSplitFunction As New SequenceSplitFunction(sequenceSplit)

					currentSequence = currentSequence.stream().flatMap(Function(input) sequenceSplitFunction.call(input).stream()).collect(toList())
				ElseIf d.getReducer() IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.datavec.api.transform.reduce.IAssociativeReducer reducer = d.getReducer();
					Dim reducer As IAssociativeReducer = d.getReducer()

					If currentWritables Is Nothing Then
						Throw New System.InvalidOperationException("Error during execution of reduction: current writables are null. " & "Trying to execute a reduce operation on a sequence?")
					End If
					Dim mapToPairForReducerFunction As New MapToPairForReducerFunction(reducer)
					Dim pair As IList(Of Pair(Of String, IList(Of Writable))) = currentWritables.Select(Function(input) mapToPairForReducerFunction.apply(input)).ToList()


					'initial op
					Dim resultPerKey As IDictionary(Of String, IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable))) = New Dictionary(Of String, IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)))()

					Dim groupedByKey As val = FunctionalUtils.groupByKey(pair)
					Dim aggregated As val = StreamUtils.aggregate(groupedByKey.entrySet().stream(), Function(stringListEntry As KeyValuePair(Of String, IList(Of IList(Of Writable))), stringListEntry2 As KeyValuePair(Of String, IList(Of IList(Of Writable))))
					Return stringListEntry.Key.Equals(stringListEntry2.Key)
					End Function).collect(Collectors.toList())


					aggregated.ForEach(Sub(input As IList(Of KeyValuePair(Of String, IList(Of IList(Of Writable)))))
					For Each entry As KeyValuePair(Of String, IList(Of IList(Of Writable))) In input
						If Not resultPerKey.ContainsKey(entry.Key) Then
							Dim reducer2 As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()
							resultPerKey(entry.Key) = reducer2
							For Each value As IList(Of Writable) In entry.Value
								reducer2.accept(value)
							Next value
						End If
					Next entry
					End Sub)




					currentWritables = resultPerKey.SetOfKeyValuePairs().Select(Function(input) input.getValue().get()).ToList()



				ElseIf d.getCalculateSortedRank() IsNot Nothing Then
					Dim csr As CalculateSortedRank = d.getCalculateSortedRank()

					If currentWritables Is Nothing Then
						Throw New System.InvalidOperationException("Error during execution of CalculateSortedRank: current writables are null. " & "Trying to execute a CalculateSortedRank operation on a sequence? (not currently supported)")
					End If

					Dim comparator As IComparer(Of Writable) = csr.getComparator()
					Dim sortColumn As String = csr.getSortOnColumn()
					Dim sortColumnIdx As Integer = csr.InputSchema.getIndexOfColumn(sortColumn)
					Dim ascending As Boolean = csr.isAscending()
					'NOTE: this likely isn't the most efficient implementation.
					Dim pairRDD As IList(Of Pair(Of Writable, IList(Of Writable))) = currentWritables.Select(Function(input) (New ColumnAsKeyPairFunction(sortColumnIdx)).apply(input)).ToList()
					pairRDD = pairRDD.OrderBy(New ComparatorAnonymousInnerClass(comparator, ascending)).ToList()

					Dim zipped As IList(Of Indexed(Of Pair(Of Writable, IList(Of Writable)))) = StreamUtils.zipWithIndex(pairRDD.stream()).collect(toList())
					currentWritables = zipped.Select(Function(input) (New UnzipForCalculateSortedRankFunction()).apply(Pair.of(input.getValue(),input.getIndex()))).ToList()
				Else
					Throw New Exception("Unknown/not implemented action: " & d)
				End If

			Next d

			'log.info("Completed {} of {} execution steps", count - 1, dataActions.size());       //Lazy execution means this can be printed before anything has actually happened...
			If currentSequence IsNot Nothing Then
				Dim allSameLength As Boolean = True
				Dim length As Integer? = Nothing
				For Each record As IList(Of IList(Of Writable)) In currentSequence
					If length Is Nothing Then
						length = record.Count
					ElseIf record.Count <> length Then
						allSameLength = False
					End If
				Next record

				If allSameLength Then
					Dim arrowColumns As IList(Of FieldVector) = ArrowConverter.toArrowColumnsTimeSeries(bufferAllocator, sequence.FinalSchema, currentSequence)
					 Dim timeSeriesLength As Integer = currentSequence(0).Count * currentSequence(0)(0).Count
	'                 if(currentSequence.get(0).get(0).size() == 1) {
	'                     timeSeriesLength = 1;
	'                 }
					Dim writablesConvert As IList(Of IList(Of IList(Of Writable))) = ArrowConverter.toArrowWritablesTimeSeries(arrowColumns, sequence.FinalSchema, timeSeriesLength)
					currentSequence = writablesConvert
				End If


				Return Pair.of(Nothing, currentSequence)

			Else

				Return New Pair(Of IList(Of IList(Of Writable)), IList(Of IList(Of IList(Of Writable))))(ArrowConverter.toArrowWritables(ArrowConverter.toArrowColumns(bufferAllocator, sequence.FinalSchema, currentWritables),sequence.FinalSchema), Nothing)
			End If

		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Pair(Of Writable, IList(Of Writable)))

			Private comparator As IComparer(Of Writable)
			Private ascending As Boolean

			Public Sub New(ByVal comparator As IComparer(Of Writable), ByVal ascending As Boolean)
				Me.comparator = comparator
				Me.ascending = ascending
			End Sub

			Public Function Compare(ByVal writableListPair As Pair(Of Writable, IList(Of Writable)), ByVal t1 As Pair(Of Writable, IList(Of Writable))) As Integer Implements IComparer(Of Pair(Of Writable, IList(Of Writable))).Compare
				Dim result As Integer = comparator.Compare(writableListPair.First,t1.First)
				If ascending Then
					Return result
				Else
					Return -result
				End If
			End Function
		End Class




		''' <summary>
		''' Execute a join on the specified data
		''' </summary>
		''' <param name="join">  Join to execute </param>
		''' <param name="left">  Left data for join </param>
		''' <param name="right"> Right data for join </param>
		''' <returns> Joined data </returns>
		Public Shared Function executeJoin(ByVal join As Join, ByVal left As IList(Of IList(Of Writable)), ByVal right As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))

			Dim leftColumnNames() As String = join.getJoinColumnsLeft()
			Dim leftColumnIndexes(leftColumnNames.Length - 1) As Integer
			For i As Integer = 0 To leftColumnNames.Length - 1
				leftColumnIndexes(i) = join.getLeftSchema().getIndexOfColumn(leftColumnNames(i))
			Next i
			Dim extractKeysFunction1 As New ExtractKeysFunction(leftColumnIndexes)

			Dim leftJV As IList(Of Pair(Of IList(Of Writable), IList(Of Writable))) = left.Where(Function(input) input.size() <> leftColumnNames.Length).Select(Function(input) extractKeysFunction1.apply(input)).ToList()

			Dim rightColumnNames() As String = join.getJoinColumnsRight()
			Dim rightColumnIndexes(rightColumnNames.Length - 1) As Integer
			For i As Integer = 0 To rightColumnNames.Length - 1
				rightColumnIndexes(i) = join.getRightSchema().getIndexOfColumn(rightColumnNames(i))
			Next i

			Dim extractKeysFunction As New ExtractKeysFunction(rightColumnIndexes)
			Dim rightJV As IList(Of Pair(Of IList(Of Writable), IList(Of Writable))) = right.Where(Function(input) input.size() <> rightColumnNames.Length).Select(Function(input) extractKeysFunction.apply(input)).ToList()

			Dim cogroupedJV As IDictionary(Of IList(Of Writable), Pair(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))) = FunctionalUtils.cogroup(leftJV, rightJV)
			Dim executeJoinFromCoGroupFlatMapFunction As New ExecuteJoinFromCoGroupFlatMapFunction(join)
			Dim ret As IList(Of IList(Of Writable)) = cogroupedJV.SetOfKeyValuePairs().stream().flatMap(Function(input) executeJoinFromCoGroupFlatMapFunction.call(Pair.of(input.getKey(),input.getValue())).stream()).collect(toList())

			Dim retSchema As Schema = join.OutputSchema
			Return ArrowConverter.toArrowWritables(ArrowConverter.toArrowColumns(bufferAllocator,retSchema,ret),retSchema)

		End Function


	End Class

End Namespace