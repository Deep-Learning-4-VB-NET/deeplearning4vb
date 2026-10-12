Imports System
Imports System.Collections.Generic
Imports PairFlatMapFunction = org.apache.spark.api.java.function.PairFlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.spark.impl.multilayer.scoring


	Public Class FeedForwardWithKeyFunction(Of K)
		Implements PairFlatMapFunction(Of IEnumerator(Of Tuple2(Of K, Tuple2(Of INDArray, INDArray))), K, INDArray)

		Protected Friend Shared log As Logger = LoggerFactory.getLogger(GetType(FeedForwardWithKeyFunction))

		Private ReadOnly params As Broadcast(Of INDArray)
		Private ReadOnly jsonConfig As Broadcast(Of String)
		Private ReadOnly batchSize As Integer

		''' <param name="params">     MultiLayerNetwork parameters </param>
		''' <param name="jsonConfig"> MultiLayerConfiguration, as json </param>
		''' <param name="batchSize">  Batch size to use for forward pass (use > 1 for efficiency) </param>
		Public Sub New(ByVal params As Broadcast(Of INDArray), ByVal jsonConfig As Broadcast(Of String), ByVal batchSize As Integer)
			Me.params = params
			Me.jsonConfig = jsonConfig
			Me.batchSize = batchSize
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<K, org.nd4j.linalg.api.ndarray.INDArray>> call(java.util.Iterator<scala.Tuple2<K, scala.Tuple2<org.nd4j.linalg.api.ndarray.INDArray,org.nd4j.linalg.api.ndarray.INDArray>>> iterator) throws Exception
		Public Overrides Function [call](ByVal iterator As IEnumerator(Of Tuple2(Of K, Tuple2(Of INDArray, INDArray)))) As IEnumerator(Of Tuple2(Of K, INDArray))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterator.hasNext() Then
				Return Collections.emptyIterator()
			End If

			Dim network As New MultiLayerNetwork(MultiLayerConfiguration.fromJson(jsonConfig.getValue()))
			network.init()
			Dim val As INDArray = params.value().unsafeDuplication()
			If val.length() <> network.numParams(False) Then
				Throw New System.InvalidOperationException("Network did not have same number of parameters as the broadcasted set parameters")
			End If
			network.Parameters = val

			'Issue: for 2d data (MLPs etc) we can just stack the examples.
			'But: for 3d and 4d: in principle the data sizes could be different
			'We could handle that with mask arrays - but it gets messy. The approach used here is simpler but less efficient

			Dim featuresList As IList(Of INDArray) = New List(Of INDArray)(batchSize)
			Dim fMaskList As IList(Of INDArray) = New List(Of INDArray)(batchSize)
			Dim keyList As IList(Of K) = New List(Of K)(batchSize)
			Dim origSizeList As IList(Of Integer) = New List(Of Integer)()

			Dim firstShape() As Long = Nothing
			Dim sizesDiffer As Boolean = False
			Dim tupleCount As Integer = 0
			Do While iterator.MoveNext()
				Dim t2 As Tuple2(Of K, Tuple2(Of INDArray, INDArray)) = iterator.Current

				If firstShape Is Nothing Then
					firstShape = t2._2()._1().shape()
				ElseIf Not sizesDiffer Then
					For i As Integer = 1 To firstShape.Length - 1
						If firstShape(i) <> featuresList(tupleCount - 1).size(i) Then
							sizesDiffer = True
							Exit For
						End If
					Next i
				End If
				featuresList.Add(t2._2()._1())
				fMaskList.Add(t2._2()._2())
				keyList.Add(t2._1())

				origSizeList.Add(CInt(t2._2()._1().size(0)))
				tupleCount += 1
			Loop

			If tupleCount = 0 Then
				Return Collections.emptyIterator()
			End If

			Dim output As IList(Of Tuple2(Of K, INDArray)) = New List(Of Tuple2(Of K, INDArray))(tupleCount)
			Dim currentArrayIndex As Integer = 0

			Do While currentArrayIndex < featuresList.Count
				Dim firstIdx As Integer = currentArrayIndex
				Dim nextIdx As Integer = currentArrayIndex
				Dim examplesInBatch As Integer = 0
				Dim toMerge As IList(Of INDArray) = New List(Of INDArray)()
				Dim toMergeMask As IList(Of INDArray) = New List(Of INDArray)()
				firstShape = Nothing
				Do While nextIdx < featuresList.Count AndAlso examplesInBatch < batchSize
					If firstShape Is Nothing Then
						firstShape = featuresList(nextIdx).shape()
					ElseIf sizesDiffer Then
						Dim breakWhile As Boolean = False
						For i As Integer = 1 To firstShape.Length - 1
							If firstShape(i) <> featuresList(nextIdx).size(i) Then
								'Next example has a different size. So: don't add it to the current batch, just process what we have
								breakWhile = True
								Exit For
							End If
						Next i
						If breakWhile Then
							Exit Do
						End If
					End If

					Dim f As INDArray = featuresList(nextIdx)
					Dim fm As INDArray = fMaskList(nextIdx)
					nextIdx += 1
					toMerge.Add(f)
					toMergeMask.Add(fm)
					examplesInBatch += f.size(0)
				Loop

				Dim p As Pair(Of INDArray, INDArray) = DataSetUtil.mergeFeatures(CType(toMerge, List(Of INDArray)).ToArray(), CType(toMergeMask, List(Of INDArray)).ToArray())
	'            INDArray batchFeatures = Nd4j.concat(0, toMerge.toArray(new INDArray[toMerge.size()]));
				Dim [out] As INDArray = network.output(p.First, False, p.Second, Nothing)

				examplesInBatch = 0
				For i As Integer = firstIdx To nextIdx - 1
					Dim numExamples As Integer = origSizeList(i)
					Dim outputSubset As INDArray = getSubset(examplesInBatch, examplesInBatch + numExamples, [out])
					examplesInBatch += numExamples

					output.Add(New Tuple2(Of K, INDArray)(keyList(i), outputSubset))
				Next i

				currentArrayIndex += (nextIdx - firstIdx)
			Loop

			Nd4j.Executioner.commit()

			Return output.GetEnumerator()
		End Function

		Private Function getSubset(ByVal exampleStart As Integer, ByVal exampleEnd As Integer, ByVal from As INDArray) As INDArray
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