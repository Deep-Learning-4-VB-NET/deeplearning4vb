Imports MultiDataSetWrapperIterator = org.deeplearning4j.datasets.iterator.MultiDataSetWrapperIterator
Imports SingletonDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonDataSetIterator
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.listener
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

	Public Class EarlyStoppingTrainer
		Inherits BaseEarlyStoppingTrainer(Of MultiLayerNetwork)

		Private net As MultiLayerNetwork
		Private isMultiEpoch As Boolean = False


		Public Sub New(ByVal earlyStoppingConfiguration As EarlyStoppingConfiguration(Of MultiLayerNetwork), ByVal configuration As MultiLayerConfiguration, ByVal train As DataSetIterator)
			Me.New(earlyStoppingConfiguration, New MultiLayerNetwork(configuration), train)
			net.init()
		End Sub

		Public Sub New(ByVal esConfig As EarlyStoppingConfiguration(Of MultiLayerNetwork), ByVal net As MultiLayerNetwork, ByVal train As DataSetIterator)
			Me.New(esConfig, net, train, Nothing)
		End Sub

		Public Sub New(ByVal esConfig As EarlyStoppingConfiguration(Of MultiLayerNetwork), ByVal net As MultiLayerNetwork, ByVal train As DataSetIterator, ByVal listener As EarlyStoppingListener(Of MultiLayerNetwork))
			MyBase.New(esConfig, net, train, Nothing, listener)
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
			net.pretrain(New MultiDataSetWrapperIterator(New SingletonMultiDataSetIterator(mds)))
		End Sub
	End Class

End Namespace