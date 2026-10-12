Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SameDiffMSELossLayer = org.deeplearning4j.nn.layers.samediff.testlayers.SameDiffMSELossLayer
Imports SameDiffMSEOutputLayer = org.deeplearning4j.nn.layers.samediff.testlayers.SameDiffMSEOutputLayer
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
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.nn.layers.samediff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) public class TestSameDiffOutput extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSameDiffOutput
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutputMSELossLayer()
		Public Overridable Sub testOutputMSELossLayer()
			Nd4j.Random.setSeed(12345)

			Dim confSD As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(0.01)).list().layer((New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer(New SameDiffMSELossLayer()).build()

			Dim confStd As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(0.01)).list().layer((New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer((New LossLayer.Builder()).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

			Dim netSD As New MultiLayerNetwork(confSD)
			netSD.init()

			Dim netStd As New MultiLayerNetwork(confStd)
			netStd.init()

			Dim [in] As INDArray = Nd4j.rand(3, 5)
			Dim label As INDArray = Nd4j.rand(3,5)

			Dim outSD As INDArray = netSD.output([in])
			Dim outStd As INDArray = netStd.output([in])
			assertEquals(outStd, outSD)

			Dim ds As New DataSet([in], label)
			Dim scoreSD As Double = netSD.score(ds)
			Dim scoreStd As Double = netStd.score(ds)
			assertEquals(scoreStd, scoreSD, 1e-6)

			For i As Integer = 0 To 2
				netSD.fit(ds)
				netStd.fit(ds)

				assertEquals(netStd.params(), netSD.params())
				assertEquals(netStd.getFlattenedGradients(), netSD.getFlattenedGradients())
			Next i

			'Test fit before output:
			Dim net As New MultiLayerNetwork(confSD.clone())
			net.init()
			net.fit(ds)

			'Sanity check on different minibatch sizes:
			Dim newIn As INDArray = Nd4j.vstack([in], [in])
			Dim outMbsd As INDArray = netSD.output(newIn)
			Dim outMb As INDArray = netStd.output(newIn)
			assertEquals(outMb, outMbsd)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMSEOutputLayer()
		Public Overridable Sub testMSEOutputLayer() 'Faliing 2019/04/17 - https://github.com/eclipse/deeplearning4j/issues/7560
			Nd4j.Random.setSeed(12345)

			For Each a As Activation In New Activation(){Activation.IDENTITY, Activation.TANH, Activation.SOFTMAX}
				log.info("Starting test: " & a)

				Dim confSD As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(0.01)).list().layer((New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer(New SameDiffMSEOutputLayer(5, 5, a, WeightInit.XAVIER)).build()

				Dim confStd As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(0.01)).list().layer((New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer((New OutputLayer.Builder()).nIn(5).nOut(5).activation(a).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

				Dim netSD As New MultiLayerNetwork(confSD)
				netSD.init()

				Dim netStd As New MultiLayerNetwork(confStd)
				netStd.init()

				netSD.params().assign(netStd.params())

				assertEquals(netStd.paramTable(), netSD.paramTable())

				Dim minibatch As Integer = 2
				Dim [in] As INDArray = Nd4j.rand(minibatch, 5)
				Dim label As INDArray = Nd4j.rand(minibatch, 5)

				Dim outSD As INDArray = netSD.output([in])
				Dim outStd As INDArray = netStd.output([in])
				assertEquals(outStd, outSD)

				Dim ds As New DataSet([in], label)
				Dim scoreSD As Double = netSD.score(ds)
				Dim scoreStd As Double = netStd.score(ds)
				assertEquals(scoreStd, scoreSD, 1e-6)

				netSD.Input = [in]
				netSD.Labels = label

				netStd.Input = [in]
				netStd.Labels = label

				'System.out.println(((SameDiffOutputLayer) netSD.getLayer(1)).sameDiff.summary());

				netSD.computeGradientAndScore()
				netStd.computeGradientAndScore()

				assertEquals(netStd.getFlattenedGradients(), netSD.getFlattenedGradients())

				For i As Integer = 0 To 2
					netSD.fit(ds)
					netStd.fit(ds)
					Dim s As String = i.ToString()
					assertEquals(netStd.params(), netSD.params(), s)
					assertEquals(netStd.getFlattenedGradients(), netSD.getFlattenedGradients(), s)
				Next i

				'Test fit before output:
				Dim net As New MultiLayerNetwork(confSD.clone())
				net.init()
				net.fit(ds)

				'Sanity check on different minibatch sizes:
				Dim newIn As INDArray = Nd4j.vstack([in], [in])
				Dim outMbsd As INDArray = netSD.output(newIn)
				Dim outMb As INDArray = netStd.output(newIn)
				assertEquals(outMb, outMbsd)
			Next a
		End Sub

	End Class

End Namespace