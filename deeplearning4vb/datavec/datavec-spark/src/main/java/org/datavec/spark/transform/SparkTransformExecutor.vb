Imports System
Imports System.Collections.Generic
Imports Pair = org.apache.commons.math3.util.Pair
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Function2 = org.apache.spark.api.java.function.Function2
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
Imports Writable = org.datavec.api.writable.Writable
Imports SequenceEmptyRecordFunction = org.datavec.spark.SequenceEmptyRecordFunction
Imports EmptyRecordFunction = org.datavec.spark.functions.EmptyRecordFunction
Imports SequenceFlatMapFunction = org.datavec.spark.transform.analysis.SequenceFlatMapFunction
Imports SparkFilterFunction = org.datavec.spark.transform.filter.SparkFilterFunction
Imports ExecuteJoinFromCoGroupFlatMapFunction = org.datavec.spark.transform.join.ExecuteJoinFromCoGroupFlatMapFunction
Imports ExtractKeysFunction = org.datavec.spark.transform.join.ExtractKeysFunction
Imports ColumnAsKeyPairFunction = org.datavec.spark.transform.misc.ColumnAsKeyPairFunction
Imports UnzipForCalculateSortedRankFunction = org.datavec.spark.transform.rank.UnzipForCalculateSortedRankFunction
Imports MapToPairForReducerFunction = org.datavec.spark.transform.reduce.MapToPairForReducerFunction
Imports org.datavec.spark.transform.sequence
Imports SequenceSplitFunction = org.datavec.spark.transform.transform.SequenceSplitFunction
Imports SparkTransformFunction = org.datavec.spark.transform.transform.SparkTransformFunction
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports Tuple2 = scala.Tuple2

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

Namespace org.datavec.spark.transform


	Public Class SparkTransformExecutor

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(SparkTransformExecutor))
		'a boolean jvm argument that when the system property is true
		'will cause some functions to invoke a try catch block and just log errors
		'returning empty records
		Public Const LOG_ERROR_PROPERTY As String = "org.datavec.spark.transform.logerrors"

		''' @deprecated Use static methods instead of instance methods on SparkTransformExecutor 
		<Obsolete("Use static methods instead of instance methods on SparkTransformExecutor")>
		Public Sub New()

		End Sub

		''' <summary>
		''' Execute the specified TransformProcess with the given input data<br>
		''' Note: this method can only be used if the TransformProcess returns non-sequence data. For TransformProcesses
		''' that return a sequence, use <seealso cref="executeToSequence(JavaRDD, TransformProcess)"/>
		''' </summary>
		''' <param name="inputWritables">   Input data to process </param>
		''' <param name="transformProcess"> TransformProcess to execute </param>
		''' <returns> Processed data </returns>
		Public Shared Function execute(ByVal inputWritables As JavaRDD(Of IList(Of Writable)), ByVal transformProcess As TransformProcess) As JavaRDD(Of IList(Of Writable))
			If TypeOf transformProcess.FinalSchema Is SequenceSchema Then
				Throw New System.InvalidOperationException("Cannot return sequence data with this method")
			End If

			Return execute(inputWritables, Nothing, transformProcess).getFirst()
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
		Public Shared Function executeToSequence(ByVal inputWritables As JavaRDD(Of IList(Of Writable)), ByVal transformProcess As TransformProcess) As JavaRDD(Of IList(Of IList(Of Writable)))
			If Not (TypeOf transformProcess.FinalSchema Is SequenceSchema) Then
				Throw New System.InvalidOperationException("Cannot return non-sequence data with this method")
			End If

			Return execute(inputWritables, Nothing, transformProcess).getSecond()
		End Function

		''' <summary>
		''' Execute the specified TransformProcess with the given <i>sequence</i> input data<br>
		''' Note: this method can only be used if the TransformProcess starts with sequence data, but returns <i>non-sequential</i>
		''' data (after reducing or converting sequential data to individual examples)
		''' </summary>
		''' <param name="inputSequence">    Input sequence data to process </param>
		''' <param name="transformProcess"> TransformProcess to execute </param>
		''' <returns> Processed (non-sequential) data </returns>
		Public Shared Function executeSequenceToSeparate(ByVal inputSequence As JavaRDD(Of IList(Of IList(Of Writable))), ByVal transformProcess As TransformProcess) As JavaRDD(Of IList(Of Writable))
			If TypeOf transformProcess.FinalSchema Is SequenceSchema Then
				Throw New System.InvalidOperationException("Cannot return sequence data with this method")
			End If

			Return execute(Nothing, inputSequence, transformProcess).getFirst()
		End Function

		''' <summary>
		''' Execute the specified TransformProcess with the given <i>sequence</i> input data<br>
		''' Note: this method can only be used if the TransformProcess starts with sequence data, and also returns sequence data
		''' </summary>
		''' <param name="inputSequence">    Input sequence data to process </param>
		''' <param name="transformProcess"> TransformProcess to execute </param>
		''' <returns> Processed (non-sequential) data </returns>
		Public Shared Function executeSequenceToSequence(ByVal inputSequence As JavaRDD(Of IList(Of IList(Of Writable))), ByVal transformProcess As TransformProcess) As JavaRDD(Of IList(Of IList(Of Writable)))
			If Not (TypeOf transformProcess.FinalSchema Is SequenceSchema) Then
				Throw New System.InvalidOperationException("Cannot return non-sequence data with this method")
			End If

			Return execute(Nothing, inputSequence, transformProcess).getSecond()
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

		Private Shared Function execute(ByVal inputWritables As JavaRDD(Of IList(Of Writable)), ByVal inputSequence As JavaRDD(Of IList(Of IList(Of Writable))), ByVal sequence As TransformProcess) As Pair(Of JavaRDD(Of IList(Of Writable)), JavaRDD(Of IList(Of IList(Of Writable))))
			Dim currentWritables As JavaRDD(Of IList(Of Writable)) = inputWritables
			Dim currentSequence As JavaRDD(Of IList(Of IList(Of Writable))) = inputSequence

			Dim dataActions As IList(Of DataAction) = sequence.getActionList()
			If inputWritables IsNot Nothing Then
				Dim first As IList(Of Writable) = inputWritables.first()
				If first.Count <> sequence.getInitialSchema().numColumns() Then
					Throw New System.InvalidOperationException("Input data number of columns (" & first.Count & ") does not match the number of columns for the transform process (" & sequence.getInitialSchema().numColumns() & ")")
				End If
			Else
				Dim firstSeq As IList(Of IList(Of Writable)) = inputSequence.first()
				If firstSeq.Count > 0 AndAlso firstSeq(0).Count <> sequence.getInitialSchema().numColumns() Then
					Throw New System.InvalidOperationException("Input sequence data number of columns (" & firstSeq(0).Count & ") does not match the number of columns for the transform process (" & sequence.getInitialSchema().numColumns() & ")")
				End If
			End If


			Dim count As Integer = 1
			For Each d As DataAction In dataActions
				'log.info("Starting execution of stage {} of {}", count, dataActions.size());     //

				If d.getTransform() IsNot Nothing Then
					Dim t As Transform = d.getTransform()
					If currentWritables IsNot Nothing Then
						Dim [function] As [Function](Of IList(Of Writable), IList(Of Writable)) = New SparkTransformFunction(t)
						If TryCatch Then
							currentWritables = currentWritables.map([function]).filter(New EmptyRecordFunction())
						Else
							currentWritables = currentWritables.map([function])
						End If
					Else
						Dim [function] As [Function](Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable))) = New SparkSequenceTransformFunction(t)
						If TryCatch Then
							currentSequence = currentSequence.map([function]).filter(New SequenceEmptyRecordFunction())
						Else
							currentSequence = currentSequence.map([function])
						End If


					End If
				ElseIf d.getFilter() IsNot Nothing Then
					'Filter
					Dim f As Filter = d.getFilter()
					If currentWritables IsNot Nothing Then
						currentWritables = currentWritables.filter(New SparkFilterFunction(f))
					Else
						currentSequence = currentSequence.filter(New SparkSequenceFilterFunction(f))
					End If

				ElseIf d.getConvertToSequence() IsNot Nothing Then
					'Convert to a sequence...
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.datavec.api.transform.sequence.ConvertToSequence cts = d.getConvertToSequence();
					Dim cts As ConvertToSequence = d.getConvertToSequence()

					If cts.isSingleStepSequencesMode() Then
						'Edge case: create a sequence from each example, by treating each value as a sequence of length 1
						currentSequence = currentWritables.map(New ConvertToSequenceLengthOne())
						currentWritables = Nothing
					Else
						'Standard case: join by key
						'First: convert to PairRDD
						Dim schema As Schema = cts.getInputSchema()
						Dim colIdxs() As Integer = schema.getIndexOfColumns(cts.getKeyColumns())
						Dim withKey As JavaPairRDD(Of IList(Of Writable), IList(Of Writable)) = currentWritables.mapToPair(New SparkMapToPairByMultipleColumnsFunction(colIdxs))
						Dim grouped As JavaPairRDD(Of IList(Of Writable), IEnumerable(Of IList(Of Writable))) = withKey.groupByKey()

						'Now: convert to a sequence...
						currentSequence = grouped.mapValues(New SparkGroupToSequenceFunction(cts.getComparator())).values()
						currentWritables = Nothing
					End If
				ElseIf d.getConvertFromSequence() IsNot Nothing Then
					'Convert from sequence...

					If currentSequence Is Nothing Then
						Throw New System.InvalidOperationException("Cannot execute ConvertFromSequence operation: current sequence is null")
					End If

					currentWritables = currentSequence.flatMap(New SequenceFlatMapFunction())
					currentSequence = Nothing
				ElseIf d.getSequenceSplit() IsNot Nothing Then
					Dim sequenceSplit As SequenceSplit = d.getSequenceSplit()
					If currentSequence Is Nothing Then
						Throw New System.InvalidOperationException("Error during execution of SequenceSplit: currentSequence is null")
					End If
					currentSequence = currentSequence.flatMap(New SequenceSplitFunction(sequenceSplit))
				ElseIf d.getReducer() IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.datavec.api.transform.reduce.IAssociativeReducer reducer = d.getReducer();
					Dim reducer As IAssociativeReducer = d.getReducer()

					If currentWritables Is Nothing Then
						Throw New System.InvalidOperationException("Error during execution of reduction: current writables are null. " & "Trying to execute a reduce operation on a sequence?")
					End If
					Dim pair As JavaPairRDD(Of String, IList(Of Writable)) = currentWritables.mapToPair(New MapToPairForReducerFunction(reducer))


					currentWritables = pair.aggregateByKey(reducer.aggregableReducer(), New Function2AnonymousInnerClass()
								   , New Function2AnonymousInnerClass2()
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
									).mapValues(New org.apache.spark.api.java.function.Function<org.datavec.api.transform.ops.IAggregableReduceOp<java.util.List<org.datavec.api.writable.Writable>, java.util.List<org.datavec.api.writable.Writable>>, java.util.List<org.datavec.api.writable.Writable>>()
									If True Then

										public IList(Of Writable) [call](IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) listIAggregableReduceOp) throws Exception
										If True Then
											Return listIAggregableReduceOp.get()
										End If
									End If
									).values()

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
					Dim pairRDD As JavaPairRDD(Of Writable, IList(Of Writable)) = currentWritables.mapToPair(New ColumnAsKeyPairFunction(sortColumnIdx))
					pairRDD = pairRDD.sortByKey(comparator, ascending)

					Dim zipped As JavaPairRDD(Of Tuple2(Of Writable, IList(Of Writable)), Long) = pairRDD.zipWithIndex()
					currentWritables = zipped.map(New UnzipForCalculateSortedRankFunction())
				Else
					Throw New Exception("Unknown/not implemented action: " & d)
				End If

				count += 1
			Next d

			'log.info("Completed {} of {} execution steps", count - 1, dataActions.size());       //Lazy execution means this can be printed before anything has actually happened...

			Return New Pair(Of JavaRDD(Of IList(Of Writable)), JavaRDD(Of IList(Of IList(Of Writable))))(currentWritables, currentSequence)
		End Function

		Private Class Function2AnonymousInnerClass
			Inherits Function2(Of IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)), IList(Of Writable), IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.transform.ops.IAggregableReduceOp<java.util.List<org.datavec.api.writable.Writable>, java.util.List<org.datavec.api.writable.Writable>> call(org.datavec.api.transform.ops.IAggregableReduceOp<java.util.List<org.datavec.api.writable.Writable>, java.util.List<org.datavec.api.writable.Writable>> iAggregableReduceOp, java.util.List<org.datavec.api.writable.Writable> writables) throws Exception
			Public Overrides Function [call](ByVal iAggregableReduceOp As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)), ByVal writables As IList(Of Writable)) As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable))
				iAggregableReduceOp.accept(writables)
				Return iAggregableReduceOp
			End Function
		End Class

		Private Class Function2AnonymousInnerClass2
			Inherits Function2(Of IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)), IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)), IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.transform.ops.IAggregableReduceOp<java.util.List<org.datavec.api.writable.Writable>, java.util.List<org.datavec.api.writable.Writable>> call(org.datavec.api.transform.ops.IAggregableReduceOp<java.util.List<org.datavec.api.writable.Writable>, java.util.List<org.datavec.api.writable.Writable>> iAggregableReduceOp, org.datavec.api.transform.ops.IAggregableReduceOp<java.util.List<org.datavec.api.writable.Writable>, java.util.List<org.datavec.api.writable.Writable>> iAggregableReduceOp2) throws Exception
			Public Overrides Function [call](ByVal iAggregableReduceOp As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)), ByVal iAggregableReduceOp2 As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable))) As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable))
				iAggregableReduceOp.combine(iAggregableReduceOp2)
				Return iAggregableReduceOp
			End Function
		End Class

		''' <summary>
		''' Execute a join on the specified data
		''' </summary>
		''' <param name="join">  Join to execute </param>
		''' <param name="left">  Left data for join </param>
		''' <param name="right"> Right data for join </param>
		''' <returns> Joined data </returns>
		Public Shared Function executeJoin(ByVal join As Join, ByVal left As JavaRDD(Of IList(Of Writable)), ByVal right As JavaRDD(Of IList(Of Writable))) As JavaRDD(Of IList(Of Writable))

			Dim leftColumnNames() As String = join.getJoinColumnsLeft()
			Dim leftColumnIndexes(leftColumnNames.Length - 1) As Integer
			For i As Integer = 0 To leftColumnNames.Length - 1
				leftColumnIndexes(i) = join.getLeftSchema().getIndexOfColumn(leftColumnNames(i))
			Next i

			Dim leftJV As JavaPairRDD(Of IList(Of Writable), IList(Of Writable)) = left.mapToPair(New ExtractKeysFunction(leftColumnIndexes))

			Dim rightColumnNames() As String = join.getJoinColumnsRight()
			Dim rightColumnIndexes(rightColumnNames.Length - 1) As Integer
			For i As Integer = 0 To rightColumnNames.Length - 1
				rightColumnIndexes(i) = join.getRightSchema().getIndexOfColumn(rightColumnNames(i))
			Next i

			Dim rightJV As JavaPairRDD(Of IList(Of Writable), IList(Of Writable)) = right.mapToPair(New ExtractKeysFunction(rightColumnIndexes))

			Dim cogroupedJV As JavaPairRDD(Of IList(Of Writable), Tuple2(Of IEnumerable(Of IList(Of Writable)), IEnumerable(Of IList(Of Writable)))) = leftJV.cogroup(rightJV)

			Return cogroupedJV.flatMap(New ExecuteJoinFromCoGroupFlatMapFunction(join))
		End Function
	End Class

End Namespace