Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports FineTuneConfiguration = org.deeplearning4j.nn.transferlearning.FineTuneConfiguration
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports FileStatsStorage = org.deeplearning4j.ui.model.storage.FileStatsStorage
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd

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

Namespace org.deeplearning4j.ui.stats

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestTransferStatsCollection extends org.deeplearning4j.BaseDL4JTest
	Public Class TestTransferStatsCollection
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90_000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void test() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub test()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder()).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()


			Dim net2 As MultiLayerNetwork = (New TransferLearning.Builder(net)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).updater(New Sgd(0.01)).build()).setFeatureExtractor(0).build()

			net2.setListeners(New StatsListener(New InMemoryStatsStorage()))

			'Previosuly: failed on frozen layers
			net2.fit(New DataSet(Nd4j.rand(8, 10), Nd4j.rand(8, 10)))
		End Sub
	End Class

End Namespace