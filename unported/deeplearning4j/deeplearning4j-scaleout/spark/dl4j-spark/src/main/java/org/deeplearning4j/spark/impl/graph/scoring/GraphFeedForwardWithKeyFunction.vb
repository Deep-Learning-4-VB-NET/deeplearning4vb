Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports PairFlatMapFunction = org.apache.spark.api.java.function.PairFlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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

Namespace org.deeplearning4j.spark.impl.graph.scoring


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @AllArgsConstructor public class GraphFeedForwardWithKeyFunction<K> implements org.apache.spark.api.java.function.PairFlatMapFunction<java.util.Iterator<scala.Tuple2<K, org.nd4j.linalg.api.ndarray.INDArray[]>>, K, org.nd4j.linalg.api.ndarray.INDArray[]>
	Public Class GraphFeedForwardWithKeyFunction(Of K)
		Implements PairFlatMapFunction(Of IEnumerator(Of Tuple2(Of K, INDArray())), K, INDArray())

		Private ReadOnly params As Broadcast(Of INDArray)
		Private ReadOnly jsonConfig As Broadcast(Of String)
		Private ReadOnly batchSize As Integer


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<K, org.nd4j.linalg.api.ndarray.INDArray[]>> call(java.util.Iterator<scala.Tuple2<K, org.nd4j.linalg.api.ndarray.INDArray[]>> iterator) throws Exception
		Public Overrides Function [call](ByVal iterator As IEnumerator(Of Tuple2(Of K, INDArray()))) As IEnumerator(Of Tuple2(Of K, INDArray()))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterator.hasNext() Then
				Return Collections.emptyIterator()
			End If

			Dim network As New ComputationGraph(ComputationGraphConfiguration.fromJson(jsonConfig.getValue()))
			network.init()
			Dim val As INDArray = params.value().unsafeDuplication()
			If val.length() <> network.numParams(False) Then
				Throw New System.InvalidOperationException("Network did not have same number of parameters as the broadcast set parameters")
			End If
			network.Params = val

			'Issue: for 2d data (MLPs etc) we can just stack the examples.
			'But: for 3d and 4d: in principle the data sizes could be different
			'We could handle that with mask arrays - but it gets messy. The approach used here is simpler but less efficient

			Dim featuresList As IList(Of INDArray()) = New List(Of INDArray())(batchSize)
			Dim keyList As IList(Of K) = New List(Of K)(batchSize)
			Dim origSizeList As IList(Of Long) = New List(Of Long)()

			Dim firstShapes()() As Long = Nothing
			Dim sizesDiffer As Boolean = False
			Dim tupleCount As Integer = 0
			Do While iterator.MoveNext()
				Dim t2 As Tuple2(Of K, INDArray()) = iterator.Current
				If firstShapes Is Nothing Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: firstShapes = new Long[t2._2().length][0]
					firstShapes = RectangularArrays.RectangularLongArray(t2._2().length, 0)
					For i As Integer = 0 To firstShapes.Length - 1
						firstShapes(i) = t2._2()(i).shape()
					Next i
				ElseIf Not sizesDiffer Then
					For i As Integer = 0 To firstShapes.Length - 1
						Dim j As Integer = 1
						Do While j < firstShapes(i).Length
							If firstShapes(i)(j) <> featuresList(tupleCount - 1)(i).size(j) Then
								sizesDiffer = True
								Exit Do
							End If
							j += 1
						Loop
					Next i
				End If
				featuresList.Add(t2._2())
				keyList.Add(t2._1())

				origSizeList.Add(t2._2()(0).size(0))
				tupleCount += 1
			Loop

			If tupleCount = 0 Then
				Return Collections.emptyIterator()
			End If

			Dim output As IList(Of Tuple2(Of K, INDArray())) = New List(Of Tuple2(Of K, INDArray()))(tupleCount)
			Dim currentArrayIndex As Integer = 0

			Do While currentArrayIndex < featuresList.Count
				Dim firstIdx As Integer = currentArrayIndex
				Dim nextIdx As Integer = currentArrayIndex
				Dim examplesInBatch As Integer = 0
				Dim toMerge As IList(Of INDArray()) = New List(Of INDArray())()
				firstShapes = Nothing
				Do While nextIdx < featuresList.Count AndAlso examplesInBatch < batchSize
					Dim f() As INDArray = featuresList(nextIdx)
					If firstShapes Is Nothing Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: firstShapes = new Long[f.Length][0]
						firstShapes = RectangularArrays.RectangularLongArray(f.Length, 0)
						For i As Integer = 0 To firstShapes.Length - 1
							firstShapes(i) = f(i).shape()
						Next i
					ElseIf sizesDiffer Then
						Dim breakWhile As Boolean = False
						For i As Integer = 0 To firstShapes.Length - 1
							Dim j As Integer = 1
							Do While j < firstShapes(i).Length
								If firstShapes(i)(j) <> featuresList(nextIdx)(i).size(j) Then
									'Next example has a different size. So: don't add it to the current batch, just process what we have
									breakWhile = True
									Exit Do
								End If
								j += 1
							Loop
						Next i
						If breakWhile Then
							Exit Do
						End If
					End If

					toMerge.Add(f)
					examplesInBatch += f(0).size(0)
					nextIdx += 1
				Loop

				Dim batchFeatures((toMerge(0).Length) - 1) As INDArray
				For i As Integer = 0 To batchFeatures.Length - 1
					Dim tempArr(toMerge.Count - 1) As INDArray
					For j As Integer = 0 To tempArr.Length - 1
						tempArr(j) = toMerge(j)(i)
					Next j
					batchFeatures(i) = Nd4j.concat(0, tempArr)
				Next i


				Dim [out]() As INDArray = network.output(False, batchFeatures)

				examplesInBatch = 0
				For i As Integer = firstIdx To nextIdx - 1
					Dim numExamples As Long = origSizeList(i)
					Dim outSubset([out].Length - 1) As INDArray
					For j As Integer = 0 To [out].Length - 1
						outSubset(j) = getSubset(examplesInBatch, examplesInBatch + numExamples, [out](j))
					Next j
					examplesInBatch += numExamples

					output.Add(New Tuple2(Of K, INDArray())(keyList(i), outSubset))
				Next i

				currentArrayIndex += (nextIdx - firstIdx)
			Loop

			Nd4j.Executioner.commit()

			Return output.GetEnumerator()
		End Function

		Private Function getSubset(ByVal exampleStart As Long, ByVal exampleEnd As Long, ByVal from As INDArray) As INDArray
			Select Case from.rank()
				Case 2
					Return from.get(NDArrayIndex.interval(exampleStart, exampleEnd), NDArrayIndex.all())
				Case 3
					Return from.get(NDArrayIndex.interval(exampleStart, exampleEnd), NDArrayIndex.all(), NDArrayIndex.all())
				Case 4
					Return from.get(NDArrayIndex.interval(exampleStart, exampleEnd), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
				Case Else
					Throw New Exception("Invalid rank: " & from.rank())
			End Select
		End Function
	End Class

End Namespace