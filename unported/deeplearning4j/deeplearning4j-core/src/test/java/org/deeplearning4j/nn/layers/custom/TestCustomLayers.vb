Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports CustomLayer = org.deeplearning4j.nn.layers.custom.testclasses.CustomLayer
Imports CustomOutputLayer = org.deeplearning4j.nn.layers.custom.testclasses.CustomOutputLayer
Imports CustomOutputLayerImpl = org.deeplearning4j.nn.layers.custom.testclasses.CustomOutputLayerImpl
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports AnnotatedClass = org.nd4j.shade.jackson.databind.introspect.AnnotatedClass
Imports NamedType = org.nd4j.shade.jackson.databind.jsontype.NamedType
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.nn.layers.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.CUSTOM_FUNCTIONALITY) public class TestCustomLayers extends org.deeplearning4j.BaseDL4JTest
	Public Class TestCustomLayers
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJsonMultiLayerNetwork()
		Public Overridable Sub testJsonMultiLayerNetwork()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, New CustomLayer(3.14159)).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()

			Dim json As String = conf.toJson()
			Dim yaml As String = conf.toYaml()

	'        System.out.println(json);

			Dim confFromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
			assertEquals(conf, confFromJson)

			Dim confFromYaml As MultiLayerConfiguration = MultiLayerConfiguration.fromYaml(yaml)
			assertEquals(conf, confFromYaml)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJsonComputationGraph()
		Public Overridable Sub testJsonComputationGraph()
			'ComputationGraph with a custom layer; check JSON and YAML config actually works...

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", New CustomLayer(3.14159), "0").addLayer("2", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "1").setOutputs("2").build()

			Dim json As String = conf.toJson()
			Dim yaml As String = conf.toYaml()

	'        System.out.println(json);

			Dim confFromJson As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(conf, confFromJson)

			Dim confFromYaml As ComputationGraphConfiguration = ComputationGraphConfiguration.fromYaml(yaml)
			assertEquals(conf, confFromYaml)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void checkInitializationFF()
		Public Overridable Sub checkInitializationFF()
			'Actually create a network with a custom layer; check initialization and forward pass

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(9).nOut(10).build()).layer(1, New CustomLayer(3.14159)).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(11).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			assertEquals(9 * 10 + 10, net.getLayer(0).numParams())
			assertEquals(10 * 10 + 10, net.getLayer(1).numParams())
			assertEquals(10 * 11 + 11, net.getLayer(2).numParams())

			'Check for exceptions...
			net.output(Nd4j.rand(1, 9))
			net.fit(New DataSet(Nd4j.rand(1, 9), Nd4j.rand(1, 11)))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomOutputLayerMLN()
		Public Overridable Sub testCustomOutputLayerMLN()
			'Second: let's create a MultiLayerCofiguration with one, and check JSON and YAML config actually works...
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New CustomOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()

			Dim json As String = conf.toJson()
			Dim yaml As String = conf.toYaml()

	'        System.out.println(json);

			Dim confFromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
			assertEquals(conf, confFromJson)

			Dim confFromYaml As MultiLayerConfiguration = MultiLayerConfiguration.fromYaml(yaml)
			assertEquals(conf, confFromYaml)

			'Third: check initialization
			Nd4j.Random.setSeed(12345)
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			assertTrue(TypeOf net.getLayer(1) Is CustomOutputLayerImpl)

			'Fourth: compare to an equivalent standard output layer (should be identical)
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Nd4j.Random.setSeed(12345)
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()

			assertEquals(net2.params(), net.params())

			Dim testFeatures As INDArray = Nd4j.rand(1, 10)
			Dim testLabels As INDArray = Nd4j.zeros(1, 10)
			testLabels.putScalar(0, 3, 1.0)
			Dim ds As New DataSet(testFeatures, testLabels)

			assertEquals(net2.output(testFeatures), net.output(testFeatures))
			assertEquals(net2.score(ds), net.score(ds), 1e-6)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomOutputLayerCG()
		Public Overridable Sub testCustomOutputLayerCG()
			'Create a ComputationGraphConfiguration with custom output layer, and check JSON and YAML config actually works...
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", (New CustomOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			Dim json As String = conf.toJson()
			Dim yaml As String = conf.toYaml()

	'        System.out.println(json);

			Dim confFromJson As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(conf, confFromJson)

			Dim confFromYaml As ComputationGraphConfiguration = ComputationGraphConfiguration.fromYaml(yaml)
			assertEquals(conf, confFromYaml)

			'Third: check initialization
			Nd4j.Random.setSeed(12345)
			Dim net As New ComputationGraph(conf)
			net.init()

			assertTrue(TypeOf net.getLayer(1) Is CustomOutputLayerImpl)

			'Fourth: compare to an equivalent standard output layer (should be identical)
			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()
			Nd4j.Random.setSeed(12345)
			Dim net2 As New ComputationGraph(conf2)
			net2.init()

			assertEquals(net2.params(), net.params())

			Dim testFeatures As INDArray = Nd4j.rand(1, 10)
			Dim testLabels As INDArray = Nd4j.zeros(1, 10)
			testLabels.putScalar(0, 3, 1.0)
			Dim ds As New DataSet(testFeatures, testLabels)

			assertEquals(net2.output(testFeatures)(0), net.output(testFeatures)(0))
			assertEquals(net2.score(ds), net.score(ds), 1e-6)
		End Sub
	End Class

End Namespace