Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports IteratorDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
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

Namespace org.deeplearning4j.spark.impl.multilayer.scoring


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @AllArgsConstructor public class ScoreFlatMapFunction implements org.apache.spark.api.java.function.FlatMapFunction<java.util.Iterator<org.nd4j.linalg.dataset.DataSet>, scala.Tuple2<Integer, Double>>
	Public Class ScoreFlatMapFunction
		Implements FlatMapFunction(Of IEnumerator(Of DataSet), Tuple2(Of Integer, Double))

		Private json As String
		Private params As Broadcast(Of INDArray)
		Private minibatchSize As Integer

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<Integer, Double>> call(java.util.Iterator<org.nd4j.linalg.dataset.DataSet> dataSetIterator) throws Exception
		Public Overrides Function [call](ByVal dataSetIterator As IEnumerator(Of DataSet)) As IEnumerator(Of Tuple2(Of Integer, Double))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not dataSetIterator.hasNext() Then
				Return Collections.singletonList(New Tuple2(Of )(0, 0.0)).GetEnumerator()
			End If

			Dim iter As DataSetIterator = New IteratorDataSetIterator(dataSetIterator, minibatchSize) 'Does batching where appropriate

			Dim network As New MultiLayerNetwork(MultiLayerConfiguration.fromJson(json))
			network.init()
			Dim val As INDArray = params.value().unsafeDuplication() '.value() object will be shared by all executors on each machine -> OK, as params are not modified by score function
			If val.length() <> network.numParams(False) Then
				Throw New System.InvalidOperationException("Network did not have same number of parameters as the broadcast set parameters")
			End If
			network.Parameters = val

			Dim [out] As IList(Of Tuple2(Of Integer, Double)) = New List(Of Tuple2(Of Integer, Double))()
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				Dim score As Double = network.score(ds, False)

				Dim numExamples As val = CInt(ds.Features.size(0))
				[out].Add(New Tuple2(Of Integer, Double)(numExamples, score * numExamples))
			Loop

			Nd4j.Executioner.commit()

			Return [out].GetEnumerator()
		End Function
	End Class

End Namespace