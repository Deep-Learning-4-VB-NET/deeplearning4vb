Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports PairFlatMapFunction = org.apache.spark.api.java.function.PairFlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VariationalAutoencoder = org.deeplearning4j.nn.layers.variational.VariationalAutoencoder
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.spark.impl.common.score


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseVaeScoreWithKeyFunction<K> implements org.apache.spark.api.java.function.PairFlatMapFunction<java.util.Iterator<scala.Tuple2<K, org.nd4j.linalg.api.ndarray.INDArray>>, K, Double>
	Public MustInherit Class BaseVaeScoreWithKeyFunction(Of K)
		Implements PairFlatMapFunction(Of IEnumerator(Of Tuple2(Of K, INDArray)), K, Double)

		Protected Friend ReadOnly params As Broadcast(Of INDArray)
		Protected Friend ReadOnly jsonConfig As Broadcast(Of String)
		Private ReadOnly batchSize As Integer


		''' <param name="params">                 MultiLayerNetwork parameters </param>
		''' <param name="jsonConfig">             MultiLayerConfiguration, as json </param>
		''' <param name="batchSize">              Batch size to use when scoring </param>
		Public Sub New(ByVal params As Broadcast(Of INDArray), ByVal jsonConfig As Broadcast(Of String), ByVal batchSize As Integer)
			Me.params = params
			Me.jsonConfig = jsonConfig
			Me.batchSize = batchSize
		End Sub

		Public MustOverride ReadOnly Property VaeLayer As VariationalAutoencoder

		Public MustOverride Function computeScore(ByVal vae As VariationalAutoencoder, ByVal toScore As INDArray) As INDArray


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<K, Double>> call(java.util.Iterator<scala.Tuple2<K, org.nd4j.linalg.api.ndarray.INDArray>> iterator) throws Exception
		Public Overrides Function [call](ByVal iterator As IEnumerator(Of Tuple2(Of K, INDArray))) As IEnumerator(Of Tuple2(Of K, Double))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterator.hasNext() Then
				Return Collections.emptyIterator()
			End If

			Dim vae As VariationalAutoencoder = VaeLayer

			Dim ret As IList(Of Tuple2(Of K, Double)) = New List(Of Tuple2(Of K, Double))()

			Dim collect As IList(Of INDArray) = New List(Of INDArray)(batchSize)
			Dim collectKey As IList(Of K) = New List(Of K)(batchSize)
			Dim totalCount As Integer = 0
			Do While iterator.MoveNext()
				collect.Clear()
				collectKey.Clear()
				Dim nExamples As Integer = 0
				Do While iterator.MoveNext() AndAlso nExamples < batchSize
					Dim t2 As Tuple2(Of K, INDArray) = iterator.Current
					Dim features As INDArray = t2._2()
					Dim n As val = features.size(0)
					If n <> 1 Then
						Throw New System.InvalidOperationException("Cannot score examples with one key per data set if " & "data set contains more than 1 example (numExamples: " & n & ")")
					End If
					collect.Add(features)
					collectKey.Add(t2._1())
					nExamples += n
				Loop
				totalCount += nExamples

				Dim toScore As INDArray = Nd4j.vstack(collect)
				Dim scores As INDArray = computeScore(vae, toScore)

				Dim doubleScores() As Double = scores.data().asDouble()

				For i As Integer = 0 To doubleScores.Length - 1
					ret.Add(New Tuple2(Of K, Double)(collectKey(i), doubleScores(i)))
				Next i
			Loop

			Nd4j.Executioner.commit()

			If log.isDebugEnabled() Then
				log.debug("Scored {} examples ", totalCount)
			End If

			Return ret.GetEnumerator()
		End Function
	End Class

End Namespace