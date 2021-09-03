Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports IteratorMultiDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorMultiDataSetIterator
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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


	Public Class ScoreFlatMapFunctionCGMultiDataSet
		Implements FlatMapFunction(Of IEnumerator(Of MultiDataSet), Tuple2(Of Long, Double))

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ScoreFlatMapFunctionCGMultiDataSet))
		Private json As String
		Private params As Broadcast(Of INDArray)
		Private minibatchSize As Integer


		Public Sub New(ByVal json As String, ByVal params As Broadcast(Of INDArray), ByVal minibatchSize As Integer)
			Me.json = json
			Me.params = params
			Me.minibatchSize = minibatchSize
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<scala.Tuple2<Long, Double>> call(java.util.Iterator<org.nd4j.linalg.dataset.api.MultiDataSet> dataSetIterator) throws Exception
		Public Overrides Function [call](ByVal dataSetIterator As IEnumerator(Of MultiDataSet)) As IEnumerator(Of Tuple2(Of Long, Double))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not dataSetIterator.hasNext() Then
				Return Collections.singletonList(New Tuple2(Of )(0L, 0.0)).GetEnumerator()
			End If

			Dim iter As MultiDataSetIterator = New IteratorMultiDataSetIterator(dataSetIterator, minibatchSize) 'Does batching where appropriate


			Dim network As New ComputationGraph(ComputationGraphConfiguration.fromJson(json))
			network.init()
			Dim val As INDArray = params.value().unsafeDuplication() '.value() is shared by all executors on single machine -> OK, as params are not changed in score function
			If val.length() <> network.numParams(False) Then
				Throw New System.InvalidOperationException("Network did not have same number of parameters as the broadcast set parameters")
			End If
			network.Params = val

			Dim [out] As IList(Of Tuple2(Of Long, Double)) = New List(Of Tuple2(Of Long, Double))()
			Do While iter.MoveNext()
				Dim ds As MultiDataSet = iter.Current
				Dim score As Double = network.score(ds, False)

				Dim numExamples As Long = ds.getFeatures(0).size(0)
				[out].Add(New Tuple2(Of Long, Double)(numExamples, score * numExamples))
			Loop

			Nd4j.Executioner.commit()

			Return [out].GetEnumerator()
		End Function
	End Class

End Namespace