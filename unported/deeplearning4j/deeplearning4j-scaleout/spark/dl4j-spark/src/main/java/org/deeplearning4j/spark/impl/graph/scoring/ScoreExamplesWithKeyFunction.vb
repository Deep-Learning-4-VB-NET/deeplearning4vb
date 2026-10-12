Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports PairFlatMapFunction = org.apache.spark.api.java.function.PairFlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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

Namespace org.deeplearning4j.spark.impl.graph.scoring


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ScoreExamplesWithKeyFunction<K> implements org.apache.spark.api.java.function.PairFlatMapFunction<java.util.Iterator<scala.Tuple2<K, org.nd4j.linalg.dataset.api.MultiDataSet>>, K, Double>
	Public Class ScoreExamplesWithKeyFunction(Of K)
		Implements PairFlatMapFunction(Of IEnumerator(Of Tuple2(Of K, MultiDataSet)), K, Double)

		Private ReadOnly params As Broadcast(Of INDArray)
		Private ReadOnly jsonConfig As Broadcast(Of String)
		Private ReadOnly addRegularization As Boolean
		Private ReadOnly batchSize As Integer

		''' <param name="params"> ComputationGraph parameters </param>
		''' <param name="jsonConfig"> ComputationGraphConfiguration, as json </param>
		''' <param name="addRegularizationTerms"> if true: add regularization terms (l1/l2) if applicable; false: don't add regularization terms </param>
		''' <param name="batchSize"> Batch size to use when scoring examples </param>
		Public Sub New(ByVal params As Broadcast(Of INDArray), ByVal jsonConfig As Broadcast(Of String), ByVal addRegularizationTerms As Boolean, ByVal batchSize As Integer)
			Me.params = params
			Me.jsonConfig = jsonConfig
			Me.addRegularization = addRegularizationTerms
			Me.batchSize = batchSize
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<K, Double>> call(java.util.Iterator<scala.Tuple2<K, org.nd4j.linalg.dataset.api.MultiDataSet>> iterator) throws Exception
		Public Overrides Function [call](ByVal iterator As IEnumerator(Of Tuple2(Of K, MultiDataSet))) As IEnumerator(Of Tuple2(Of K, Double))
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

			Dim ret As IList(Of Tuple2(Of K, Double)) = New List(Of Tuple2(Of K, Double))()

			Dim collect As IList(Of MultiDataSet) = New List(Of MultiDataSet)(batchSize)
			Dim collectKey As IList(Of K) = New List(Of K)(batchSize)
			Dim totalCount As Integer = 0
			Do While iterator.MoveNext()
				collect.Clear()
				collectKey.Clear()
				Dim nExamples As Integer = 0
				Do While iterator.MoveNext() AndAlso nExamples < batchSize
					Dim t2 As Tuple2(Of K, MultiDataSet) = iterator.Current
					Dim ds As MultiDataSet = t2._2()
					Dim n As val = ds.getFeatures(0).size(0)
					If n <> 1 Then
						Throw New System.InvalidOperationException("Cannot score examples with one key per data set if " & "data set contains more than 1 example (numExamples: " & n & ")")
					End If
					collect.Add(ds)
					collectKey.Add(t2._1())
					nExamples += n
				Loop
				totalCount += nExamples

				Dim data As MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet.merge(collect)


				Dim scores As INDArray = network.scoreExamples(data, addRegularization)
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