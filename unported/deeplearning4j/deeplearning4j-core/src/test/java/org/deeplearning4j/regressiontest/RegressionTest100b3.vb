Imports System.Collections.Generic
Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports CustomLayer = org.deeplearning4j.regressiontest.customlayer100a.CustomLayer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.linalg.activations.impl
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
'ORIGINAL LINE: @Disabled @NativeTag @Tag(TagNames.DL4J_OLD_API) public class RegressionTest100b3 extends org.deeplearning4j.BaseDL4JTest
	Public Class RegressionTest100b3
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

			For i As Integer = 1 To 1
				Dim dtype As String = (If(i = 0, "float", "double"))
				Dim dt As DataType = (If(i = 0, DataType.FLOAT, DataType.DOUBLE))

				Dim f As File = Resources.asFile("regression_testing/100b3/CustomLayerExample_100b3_" & dtype & ".bin")
				MultiLayerNetwork.load(f, True)

				Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)
	'            net = net.clone();

				Dim l0 As DenseLayer = CType(net.getLayer(0).conf().getLayer(), DenseLayer)
				assertEquals(New ActivationTanH(), l0.getActivationFn())
				assertEquals(New WeightDecay(0.03, False), TestUtils.getWeightDecayReg(l0))
				assertEquals(New RmsProp(0.95), l0.getIUpdater())

				Dim l1 As CustomLayer = CType(net.getLayer(1).conf().getLayer(), CustomLayer)
				assertEquals(New ActivationTanH(), l1.getActivationFn())
				assertEquals(New ActivationSigmoid(), l1.SecondActivationFunction)
				assertEquals(New RmsProp(0.95), l1.getIUpdater())


				Dim outExp As INDArray
				Dim f2 As File = Resources.asFile("regression_testing/100b3/CustomLayerExample_Output_100b3_" & dtype & ".bin")
				Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
					outExp = Nd4j.read(dis)
				End Using

				Dim [in] As INDArray
				Dim f3 As File = Resources.asFile("regression_testing/100b3/CustomLayerExample_Input_100b3_" & dtype & ".bin")
				Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
					[in] = Nd4j.read(dis)
				End Using

				assertEquals(dt, [in].dataType())
				assertEquals(dt, outExp.dataType())
				assertEquals(dt, net.params().dataType())
				assertEquals(dt, net.getFlattenedGradients().dataType())
				assertEquals(dt, net.Updater.StateViewArray.dataType())

				'System.out.println(Arrays.toString(net.params().data().asFloat()));

				Dim outAct As INDArray = net.output([in])
				assertEquals(dt, outAct.dataType())

				Dim activations As IList(Of INDArray) = net.feedForward([in])

				assertEquals(dt, net.LayerWiseConfigurations.getDataType())
				assertEquals(dt, net.params().dataType())
				assertEquals(outExp, outAct, dtype)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTM() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLSTM()

			Dim f As File = Resources.asFile("regression_testing/100b3/GravesLSTMCharModelingExample_100b3.bin")
			Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)

			Dim l0 As LSTM = CType(net.getLayer(0).conf().getLayer(), LSTM)
			assertEquals(New ActivationTanH(), l0.getActivationFn())
			assertEquals(200, l0.getNOut())
			assertEquals(New WeightInitXavier(), l0.getWeightInitFn())
			assertEquals(New WeightDecay(0.0001, False), TestUtils.getWeightDecayReg(l0))
			assertEquals(New Adam(0.005), l0.getIUpdater())

			Dim l1 As LSTM = CType(net.getLayer(1).conf().getLayer(), LSTM)
			assertEquals(New ActivationTanH(), l1.getActivationFn())
			assertEquals(200, l1.getNOut())
			assertEquals(New WeightInitXavier(), l1.getWeightInitFn())
			assertEquals(New WeightDecay(0.0001, False), TestUtils.getWeightDecayReg(l1))
			assertEquals(New Adam(0.005), l1.getIUpdater())

			Dim l2 As RnnOutputLayer = CType(net.getLayer(2).conf().getLayer(), RnnOutputLayer)
			assertEquals(New ActivationSoftmax(), l2.getActivationFn())
			assertEquals(77, l2.getNOut())
			assertEquals(New WeightInitXavier(), l2.getWeightInitFn())
			assertEquals(New WeightDecay(0.0001, False), TestUtils.getWeightDecayReg(l0))
			assertEquals(New Adam(0.005), l0.getIUpdater())

			assertEquals(BackpropType.TruncatedBPTT, net.LayerWiseConfigurations.getBackpropType())
			assertEquals(50, net.LayerWiseConfigurations.getTbpttBackLength())
			assertEquals(50, net.LayerWiseConfigurations.getTbpttFwdLength())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100b3/GravesLSTMCharModelingExample_Output_100b3.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100b3/GravesLSTMCharModelingExample_Input_100b3.bin")
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

			Dim f As File = Resources.asFile("regression_testing/100b3/VaeMNISTAnomaly_100b3.bin")
			Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)

			Dim l0 As VariationalAutoencoder = CType(net.getLayer(0).conf().getLayer(), VariationalAutoencoder)
			assertEquals(New ActivationLReLU(), l0.getActivationFn())
			assertEquals(32, l0.getNOut())
			assertArrayEquals(New Integer(){256, 256}, l0.getEncoderLayerSizes())
			assertArrayEquals(New Integer(){256, 256}, l0.getDecoderLayerSizes())
					assertEquals(New WeightInitXavier(), l0.getWeightInitFn())
			assertEquals(New WeightDecay(1e-4, False), TestUtils.getWeightDecayReg(l0))
			assertEquals(New Adam(1e-3), l0.getIUpdater())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100b3/VaeMNISTAnomaly_Output_100b3.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100b3/VaeMNISTAnomaly_Input_100b3.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			Dim outAct As INDArray = net.output([in])

			assertEquals(outExp, outAct)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testYoloHouseNumber() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testYoloHouseNumber()

			Dim f As File = Resources.asFile("regression_testing/100b3/HouseNumberDetection_100b3.bin")
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
			Dim f2 As File = Resources.asFile("regression_testing/100b3/HouseNumberDetection_Output_100b3.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100b3/HouseNumberDetection_Input_100b3.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			Dim outAct As INDArray = net.outputSingle([in])

			Dim eq As Boolean = outExp.equalsWithEps(outAct.castTo(outExp.dataType()), 1e-3)
			assertTrue(eq)
		End Sub
	End Class

End Namespace