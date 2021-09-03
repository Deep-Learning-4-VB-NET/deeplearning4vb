Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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

Namespace org.deeplearning4j.nn.updater.custom

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.CUSTOM_FUNCTIONALITY) public class TestCustomUpdater extends org.deeplearning4j.BaseDL4JTest
	Public Class TestCustomUpdater
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomUpdater()
		Public Overridable Sub testCustomUpdater()

			'Create a simple custom updater, equivalent to SGD updater

			Dim lr As Double = 0.03

			Nd4j.Random.setSeed(12345)
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).activation(Activation.TANH).updater(New CustomIUpdater(lr)).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder()).nIn(10).nOut(10).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

			Nd4j.Random.setSeed(12345)
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).activation(Activation.TANH).updater(New Sgd(lr)).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder()).nIn(10).nOut(10).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

			'First: Check updater config
			assertTrue(TypeOf (CType(conf1.getConf(0).getLayer(), BaseLayer)).getIUpdater() Is CustomIUpdater)
			assertTrue(TypeOf (CType(conf1.getConf(1).getLayer(), BaseLayer)).getIUpdater() Is CustomIUpdater)
			assertTrue(TypeOf (CType(conf2.getConf(0).getLayer(), BaseLayer)).getIUpdater() Is Sgd)
			assertTrue(TypeOf (CType(conf2.getConf(1).getLayer(), BaseLayer)).getIUpdater() Is Sgd)

			Dim u0_0 As CustomIUpdater = CType((CType(conf1.getConf(0).getLayer(), BaseLayer)).getIUpdater(), CustomIUpdater)
			Dim u0_1 As CustomIUpdater = CType((CType(conf1.getConf(1).getLayer(), BaseLayer)).getIUpdater(), CustomIUpdater)
			assertEquals(lr, u0_0.getLearningRate(), 1e-6)
			assertEquals(lr, u0_1.getLearningRate(), 1e-6)

			Dim u1_0 As Sgd = CType((CType(conf2.getConf(0).getLayer(), BaseLayer)).getIUpdater(), Sgd)
			Dim u1_1 As Sgd = CType((CType(conf2.getConf(1).getLayer(), BaseLayer)).getIUpdater(), Sgd)
			assertEquals(lr, u1_0.getLearningRate(), 1e-6)
			assertEquals(lr, u1_1.getLearningRate(), 1e-6)


			'Second: check JSON
			Dim asJson As String = conf1.toJson()
			Dim fromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(asJson)
			assertEquals(conf1, fromJson)

			Nd4j.Random.setSeed(12345)
			Dim net1 As New MultiLayerNetwork(conf1)
			net1.init()

			Nd4j.Random.setSeed(12345)
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()


			'Third: check gradients are equal
			Dim [in] As INDArray = Nd4j.rand(5, 10)
			Dim labels As INDArray = Nd4j.rand(5, 10)

			net1.Input = [in]
			net2.Input = [in]

			net1.Labels = labels
			net2.Labels = labels

			net1.computeGradientAndScore()
			net2.computeGradientAndScore()

			assertEquals(net1.getFlattenedGradients(), net2.getFlattenedGradients())
		End Sub

	End Class

End Namespace