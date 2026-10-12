Imports MultiDataSetWrapperIterator = org.deeplearning4j.datasets.iterator.MultiDataSetWrapperIterator
Imports SingletonDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonDataSetIterator
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.listener
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.earlystopping.trainer

	Public Class EarlyStoppingGraphTrainer
		Inherits BaseEarlyStoppingTrainer(Of ComputationGraph)
 'implements IEarlyStoppingTrainer<ComputationGraph> {
		Private net As ComputationGraph

		''' <param name="esConfig"> Configuration </param>
		''' <param name="net"> Network to train using early stopping </param>
		''' <param name="train"> DataSetIterator for training the network </param>
		Public Sub New(ByVal esConfig As EarlyStoppingConfiguration(Of ComputationGraph), ByVal net As ComputationGraph, ByVal train As DataSetIterator)
			Me.New(esConfig, net, train, Nothing)
		End Sub

		''' <summary>
		'''Constructor for training using a <seealso cref="DataSetIterator"/> </summary>
		''' <param name="esConfig"> Configuration </param>
		''' <param name="net"> Network to train using early stopping </param>
		''' <param name="train"> DataSetIterator for training the network </param>
		''' <param name="listener"> Early stopping listener. May be null. </param>
		Public Sub New(ByVal esConfig As EarlyStoppingConfiguration(Of ComputationGraph), ByVal net As ComputationGraph, ByVal train As DataSetIterator, ByVal listener As EarlyStoppingListener(Of ComputationGraph))
			MyBase.New(esConfig, net, train, Nothing, listener)
			If net.NumInputArrays <> 1 OrElse net.NumOutputArrays <> 1 Then
				Throw New System.InvalidOperationException("Cannot do early stopping training on ComputationGraph with DataSetIterator: graph does not have 1 input and 1 output array")
			End If
			Me.net = net
		End Sub

		''' <summary>
		'''Constructor for training using a <seealso cref="MultiDataSetIterator"/> </summary>
		''' <param name="esConfig"> Configuration </param>
		''' <param name="net"> Network to train using early stopping </param>
		''' <param name="train"> DataSetIterator for training the network </param>
		''' <param name="listener"> Early stopping listener. May be null. </param>
		Public Sub New(ByVal esConfig As EarlyStoppingConfiguration(Of ComputationGraph), ByVal net As ComputationGraph, ByVal train As MultiDataSetIterator, ByVal listener As EarlyStoppingListener(Of ComputationGraph))
			MyBase.New(esConfig, net, Nothing, train, listener)
			Me.net = net
		End Sub

		Protected Friend Overrides Sub fit(ByVal ds As DataSet)
			net.fit(ds)
		End Sub

		Protected Friend Overrides Sub fit(ByVal mds As MultiDataSet)
			net.fit(mds)
		End Sub

		Protected Friend Overrides Sub pretrain(ByVal ds As DataSet)
			net.pretrain(New SingletonDataSetIterator(ds))
		End Sub

		Protected Friend Overrides Sub pretrain(ByVal mds As MultiDataSet)
			net.pretrain(New SingletonMultiDataSetIterator(mds))
		End Sub
	End Class

End Namespace