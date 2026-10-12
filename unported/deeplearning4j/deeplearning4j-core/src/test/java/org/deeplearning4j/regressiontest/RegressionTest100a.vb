Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports GravesLSTM = org.deeplearning4j.nn.conf.layers.GravesLSTM
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports ActivationLReLU = org.nd4j.linalg.activations.impl.ActivationLReLU
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
Imports Resources = org.nd4j.common.resources.Resources
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

Namespace org.deeplearning4j.regressiontest


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @NativeTag @Tag(TagNames.DL4J_OLD_API) public class RegressionTest100a extends org.deeplearning4j.BaseDL4JTest
	Public Class RegressionTest100a
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L 'Most tests should be fast, but slow download may cause timeout on slow connections
			End Get
		End Property

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCustomLayer()
			'We dropped support for 1.0.0-alpha and earlier custom layers due to the maintenance overhead for a rarely used feature
			'An upgrade path exists as a workaround - load in beta to beta4 and re-save
			'All built-in layers can be loaded going back to 0.5.0

			Dim f As File = Resources.asFile("regression_testing/100a/CustomLayerExample_100a.bin")

			Try
				MultiLayerNetwork.load(f, True)
				fail("Expected exception")
			Catch e As Exception
				Dim msg As String = e.Message
				assertTrue(msg.Contains("custom") AndAlso msg.Contains("1.0.0-beta") AndAlso msg.Contains("saved again"), msg)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGravesLSTM() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGravesLSTM()

			Dim f As File = Resources.asFile("regression_testing/100a/GravesLSTMCharModelingExample_100a.bin")
			Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)

			Dim l0 As GravesLSTM = CType(net.getLayer(0).conf().getLayer(), GravesLSTM)
			assertEquals(New ActivationTanH(), l0.getActivationFn())
			assertEquals(200, l0.getNOut())
			assertEquals(New WeightInitXavier(), l0.getWeightInitFn())
			assertEquals(New WeightDecay(0.001, False), TestUtils.getWeightDecayReg(l0))
			assertEquals(New RmsProp(0.1), l0.getIUpdater())

			Dim l1 As GravesLSTM = CType(net.getLayer(1).conf().getLayer(), GravesLSTM)
			assertEquals(New ActivationTanH(), l1.getActivationFn())
			assertEquals(200, l1.getNOut())
			assertEquals(New WeightInitXavier(), l1.getWeightInitFn())
			assertEquals(New WeightDecay(0.001, False), TestUtils.getWeightDecayReg(l1))
			assertEquals(New RmsProp(0.1), l1.getIUpdater())

			Dim l2 As RnnOutputLayer = CType(net.getLayer(2).conf().getLayer(), RnnOutputLayer)
			assertEquals(New ActivationSoftmax(), l2.getActivationFn())
			assertEquals(77, l2.getNOut())
			assertEquals(New WeightInitXavier(), l2.getWeightInitFn())
			assertEquals(New WeightDecay(0.001, False), TestUtils.getWeightDecayReg(l0))
			assertEquals(New RmsProp(0.1), l0.getIUpdater())

			assertEquals(BackpropType.TruncatedBPTT, net.LayerWiseConfigurations.getBackpropType())
			assertEquals(50, net.LayerWiseConfigurations.getTbpttBackLength())
			assertEquals(50, net.LayerWiseConfigurations.getTbpttFwdLength())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100a/GravesLSTMCharModelingExample_Output_100a.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100a/GravesLSTMCharModelingExample_Input_100a.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			Dim outAct As INDArray = net.output([in])

			assertEquals(outExp, outAct)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVae() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVae()

			Dim f As File = Resources.asFile("regression_testing/100a/VaeMNISTAnomaly_100a.bin")
			Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)

			Dim l0 As VariationalAutoencoder = CType(net.getLayer(0).conf().getLayer(), VariationalAutoencoder)
			assertEquals(New ActivationLReLU(), l0.getActivationFn())
			assertEquals(32, l0.getNOut())
			assertArrayEquals(New Integer(){256, 256}, l0.getEncoderLayerSizes())
			assertArrayEquals(New Integer(){256, 256}, l0.getDecoderLayerSizes())
					assertEquals(New WeightInitXavier(), l0.getWeightInitFn())
			assertEquals(New WeightDecay(1e-4, False), TestUtils.getWeightDecayReg(l0))
			assertEquals(New Adam(0.05), l0.getIUpdater())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100a/VaeMNISTAnomaly_Output_100a.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100a/VaeMNISTAnomaly_Input_100a.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			Dim outAct As INDArray = net.output([in])

			assertEquals(outExp, outAct)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/23 - Failing on linux-x86_64-cuda-9.2 - see issue #7657") public void testYoloHouseNumber() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testYoloHouseNumber()

			Dim f As File = Resources.asFile("regression_testing/100a/HouseNumberDetection_100a.bin")
			Dim net As ComputationGraph = ComputationGraph.load(f, True)

			Dim nBoxes As Integer = 5
			Dim nClasses As Integer = 10

			Dim cl As ConvolutionLayer = CType((CType(net.Configuration.getVertices().get("convolution2d_9"), LayerVertex)).getLayerConf().getLayer(), ConvolutionLayer)
			assertEquals(nBoxes * (5 + nClasses), cl.getNOut())
			assertEquals(New ActivationIdentity(), cl.getActivationFn())
			assertEquals(ConvolutionMode.Same, cl.getConvolutionMode())
			assertEquals(New WeightInitXavier(), cl.getWeightInitFn())
			assertArrayEquals(New Integer(){1, 1}, cl.getKernelSize())
			assertArrayEquals(New Integer(){1, 1}, cl.getKernelSize())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100a/HouseNumberDetection_Output_100a.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100a/HouseNumberDetection_Input_100a.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			'Minor bug in 1.0.0-beta and earlier: not adding epsilon value to forward pass for batch norm
			'Which means: the record output doesn't have this. To account for this, we'll manually set eps to 0.0 here
			'https://github.com/eclipse/deeplearning4j/issues/5836#issuecomment-405526228
			For Each l As Layer In net.Layers
				If TypeOf l.conf().getLayer() Is BatchNormalization Then
					Dim bn As BatchNormalization = CType(l.conf().getLayer(), BatchNormalization)
					bn.setEps(0.0)
				End If
			Next l

			Dim outAct As INDArray = net.outputSingle([in]).castTo(outExp.dataType())

			Dim eq As Boolean = outExp.equalsWithEps(outAct, 1e-4)
			If Not eq Then
				log.info("Expected: {}", outExp)
				log.info("Actual: {}", outAct)
			End If
			assertTrue(eq, "Output not equal")
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Ignoring due to new set input types changes. Loading a network isn't a problem, but we need to set the input types yet.") public void testUpsampling2d() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUpsampling2d()

			Dim f As File = Resources.asFile("regression_testing/100a/upsampling/net.bin")
			Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)

			Dim [in] As INDArray
			Dim fIn As File = Resources.asFile("regression_testing/100a/upsampling/in.bin")
			Using dis As New java.io.DataInputStream(New FileStream(fIn, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using


			Dim label As INDArray
			Dim fLabels As File = Resources.asFile("regression_testing/100a/upsampling/labels.bin")
			Using dis As New java.io.DataInputStream(New FileStream(fLabels, FileMode.Open, FileAccess.Read))
				label = Nd4j.read(dis)
			End Using

			Dim outExp As INDArray
			Dim fOutExp As File = Resources.asFile("regression_testing/100a/upsampling/out.bin")
			Using dis As New java.io.DataInputStream(New FileStream(fOutExp, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim gradExp As INDArray
			Dim fGradExp As File = Resources.asFile("regression_testing/100a/upsampling/gradient.bin")
			Using dis As New java.io.DataInputStream(New FileStream(fGradExp, FileMode.Open, FileAccess.Read))
				gradExp = Nd4j.read(dis)
			End Using

			Dim [out] As INDArray = net.output([in], False)
			assertEquals(outExp, [out])

			net.Input = [in]
			net.Labels = label
			net.computeGradientAndScore()

			Dim grad As INDArray = net.getFlattenedGradients()
			assertEquals(gradExp, grad)
		End Sub


	End Class

End Namespace