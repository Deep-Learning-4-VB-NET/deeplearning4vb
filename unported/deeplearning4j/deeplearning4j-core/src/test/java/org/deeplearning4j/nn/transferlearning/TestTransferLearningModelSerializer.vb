Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.nn.transferlearning


	Public Class TestTransferLearningModelSerializer
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testModelSerializerFrozenLayers() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelSerializerFrozenLayers()

			Dim finetune As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).build()

			Dim nIn As Integer = 6
			Dim nOut As Integer = 3

			Dim origConf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.TANH).dropOut(0.5).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(5).build()).layer(1, (New DenseLayer.Builder()).nIn(5).nOut(4).build()).layer(2, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(3, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(nOut).build()).build()
			Dim origModel As New MultiLayerNetwork(origConf)
			origModel.init()

			Dim withFrozen As MultiLayerNetwork = (New TransferLearning.Builder(origModel)).fineTuneConfiguration(finetune).setFeatureExtractor(1).build()

			assertTrue(TypeOf withFrozen.getLayer(0) Is FrozenLayer)
			assertTrue(TypeOf withFrozen.getLayer(1) Is FrozenLayer)

			assertTrue(TypeOf withFrozen.LayerWiseConfigurations.getConf(0).getLayer() Is org.deeplearning4j.nn.conf.layers.misc.FrozenLayer)
			assertTrue(TypeOf withFrozen.LayerWiseConfigurations.getConf(1).getLayer() Is org.deeplearning4j.nn.conf.layers.misc.FrozenLayer)

			Dim restored As MultiLayerNetwork = TestUtils.testModelSerialization(withFrozen)

			assertTrue(TypeOf restored.getLayer(0) Is FrozenLayer)
			assertTrue(TypeOf restored.getLayer(1) Is FrozenLayer)
			assertFalse(TypeOf restored.getLayer(2) Is FrozenLayer)
			assertFalse(TypeOf restored.getLayer(3) Is FrozenLayer)

			Dim [in] As INDArray = Nd4j.rand(3, nIn)
			Dim [out] As INDArray = withFrozen.output([in])
			Dim out2 As INDArray = restored.output([in])

			assertEquals([out], out2)

			'Sanity check on train mode:
			[out] = withFrozen.output([in], True)
			out2 = restored.output([in], True)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testModelSerializerFrozenLayersCompGraph() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testModelSerializerFrozenLayersCompGraph()
			Dim finetune As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).build()

			Dim nIn As Integer = 6
			Dim nOut As Integer = 3

			Dim origConf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(5).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(5).nOut(4).build(), "0").addLayer("2", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "1").addLayer("3", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(nOut).build(), "2").setOutputs("3").build()
			Dim origModel As New ComputationGraph(origConf)
			origModel.init()

			Dim withFrozen As ComputationGraph = (New TransferLearning.GraphBuilder(origModel)).fineTuneConfiguration(finetune).setFeatureExtractor("1").build()

			assertTrue(TypeOf withFrozen.getLayer(0) Is FrozenLayer)
			assertTrue(TypeOf withFrozen.getLayer(1) Is FrozenLayer)

			Dim m As IDictionary(Of String, GraphVertex) = withFrozen.Configuration.getVertices()
			Dim l0 As Layer = DirectCast(m("0"), LayerVertex).getLayerConf().getLayer()
			Dim l1 As Layer = DirectCast(m("1"), LayerVertex).getLayerConf().getLayer()
			assertTrue(TypeOf l0 Is org.deeplearning4j.nn.conf.layers.misc.FrozenLayer)
			assertTrue(TypeOf l1 Is org.deeplearning4j.nn.conf.layers.misc.FrozenLayer)

			Dim restored As ComputationGraph = TestUtils.testModelSerialization(withFrozen)

			assertTrue(TypeOf restored.getLayer(0) Is FrozenLayer)
			assertTrue(TypeOf restored.getLayer(1) Is FrozenLayer)
			assertFalse(TypeOf restored.getLayer(2) Is FrozenLayer)
			assertFalse(TypeOf restored.getLayer(3) Is FrozenLayer)

			Dim [in] As INDArray = Nd4j.rand(3, nIn)
			Dim [out] As INDArray = withFrozen.outputSingle([in])
			Dim out2 As INDArray = restored.outputSingle([in])

			assertEquals([out], out2)

			'Sanity check on train mode:
			[out] = withFrozen.outputSingle(True, [in])
			out2 = restored.outputSingle(True, [in])
		End Sub
	End Class

End Namespace