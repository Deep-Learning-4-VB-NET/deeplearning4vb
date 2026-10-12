Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports org.deeplearning4j.nn.conf.layers
Imports Cropping2D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping2D
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MergeVertex = org.deeplearning4j.nn.graph.vertex.impl.MergeVertex
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
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports LossMAE = org.nd4j.linalg.lossfunctions.impl.LossMAE
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
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
'ORIGINAL LINE: @Disabled @NativeTag @Tag(TagNames.DL4J_OLD_API) public class RegressionTest100b6 extends org.deeplearning4j.BaseDL4JTest
	Public Class RegressionTest100b6
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L 'Most tests should be fast, but slow download may cause timeout on slow connections
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCustomLayer()

			For Each dtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}

				Dim dtypeName As String = dtype.ToString().ToLower()

				Dim f As File = Resources.asFile("regression_testing/100b6/CustomLayerExample_100b6_" & dtypeName & ".bin")
				MultiLayerNetwork.load(f, True)

				Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)
	'            net = net.clone();

				Dim l0 As DenseLayer = CType(net.getLayer(0).conf().getLayer(), DenseLayer)
				assertEquals(New ActivationTanH(), l0.getActivationFn())
				assertEquals(New L2Regularization(0.03), TestUtils.getL2Reg(l0))
				assertEquals(New RmsProp(0.95), l0.getIUpdater())

				Dim l1 As CustomLayer = CType(net.getLayer(1).conf().getLayer(), CustomLayer)
				assertEquals(New ActivationTanH(), l1.getActivationFn())
				assertEquals(New ActivationSigmoid(), l1.SecondActivationFunction)
				assertEquals(New RmsProp(0.95), l1.getIUpdater())

				Dim outExp As INDArray
				Dim f2 As File = Resources.asFile("regression_testing/100b6/CustomLayerExample_Output_100b6_" & dtypeName & ".bin")
				Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
					outExp = Nd4j.read(dis)
				End Using

				Dim [in] As INDArray
				Dim f3 As File = Resources.asFile("regression_testing/100b6/CustomLayerExample_Input_100b6_" & dtypeName & ".bin")
				Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
					[in] = Nd4j.read(dis)
				End Using

				assertEquals(dtype, [in].dataType())
				assertEquals(dtype, outExp.dataType())
				assertEquals(dtype, net.params().dataType())
				assertEquals(dtype, net.getFlattenedGradients().dataType())
				assertEquals(dtype, net.Updater.StateViewArray.dataType())

				'System.out.println(Arrays.toString(net.params().data().asFloat()));

				Dim outAct As INDArray = net.output([in])
				assertEquals(dtype, outAct.dataType())

				assertEquals(dtype, net.LayerWiseConfigurations.getDataType())
				assertEquals(dtype, net.params().dataType())
				Dim eq As Boolean = outExp.equalsWithEps(outAct, 0.01)
				assertTrue(eq, "Test for dtype: " & dtypeName & " - " & outExp & " vs " & outAct)
			Next dtype
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTM() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLSTM()

			Dim f As File = Resources.asFile("regression_testing/100b6/GravesLSTMCharModelingExample_100b6.bin")
			Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)

			Dim l0 As LSTM = CType(net.getLayer(0).conf().getLayer(), LSTM)
			assertEquals(New ActivationTanH(), l0.getActivationFn())
			assertEquals(200, l0.getNOut())
			assertEquals(New WeightInitXavier(), l0.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l0))
			assertEquals(New Adam(0.005), l0.getIUpdater())

			Dim l1 As LSTM = CType(net.getLayer(1).conf().getLayer(), LSTM)
			assertEquals(New ActivationTanH(), l1.getActivationFn())
			assertEquals(200, l1.getNOut())
			assertEquals(New WeightInitXavier(), l1.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l1))
			assertEquals(New Adam(0.005), l1.getIUpdater())

			Dim l2 As RnnOutputLayer = CType(net.getLayer(2).conf().getLayer(), RnnOutputLayer)
			assertEquals(New ActivationSoftmax(), l2.getActivationFn())
			assertEquals(77, l2.getNOut())
			assertEquals(New WeightInitXavier(), l2.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l2))
			assertEquals(New Adam(0.005), l2.getIUpdater())

			assertEquals(BackpropType.TruncatedBPTT, net.LayerWiseConfigurations.getBackpropType())
			assertEquals(50, net.LayerWiseConfigurations.getTbpttBackLength())
			assertEquals(50, net.LayerWiseConfigurations.getTbpttFwdLength())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100b6/GravesLSTMCharModelingExample_Output_100b6.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100b6/GravesLSTMCharModelingExample_Input_100b6.bin")
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

			Dim f As File = Resources.asFile("regression_testing/100b6/VaeMNISTAnomaly_100b6.bin")
			Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)

			Dim l0 As VariationalAutoencoder = CType(net.getLayer(0).conf().getLayer(), VariationalAutoencoder)
			assertEquals(New ActivationLReLU(), l0.getActivationFn())
			assertEquals(32, l0.getNOut())
			assertArrayEquals(New Integer(){256, 256}, l0.getEncoderLayerSizes())
			assertArrayEquals(New Integer(){256, 256}, l0.getDecoderLayerSizes())
			assertEquals(New WeightInitXavier(), l0.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l0))
			assertEquals(New Adam(1e-3), l0.getIUpdater())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100b6/VaeMNISTAnomaly_Output_100b6.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100b6/VaeMNISTAnomaly_Input_100b6.bin")
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

			Dim f As File = Resources.asFile("regression_testing/100b6/HouseNumberDetection_100b6.bin")
			Dim net As ComputationGraph = ComputationGraph.load(f, True)

			Dim nBoxes As Integer = 5
			Dim nClasses As Integer = 10

			Dim cl As ConvolutionLayer = CType((CType(net.Configuration.getVertices().get("convolution2d_9"), LayerVertex)).getLayerConf().getLayer(), ConvolutionLayer)
			assertEquals(nBoxes * (5 + nClasses), cl.getNOut())
			assertEquals(New ActivationIdentity(), cl.getActivationFn())
			assertEquals(ConvolutionMode.Same, cl.getConvolutionMode())
			assertEquals(New WeightInitXavier(), cl.getWeightInitFn())
			assertArrayEquals(New Integer(){1, 1}, cl.getKernelSize())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100b6/HouseNumberDetection_Output_100b6.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100b6/HouseNumberDetection_Input_100b6.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			Dim outAct As INDArray = net.outputSingle([in])

			Dim eq As Boolean = outExp.equalsWithEps(outAct.castTo(outExp.dataType()), 1e-3)
			assertTrue(eq)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSyntheticCNN() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSyntheticCNN()

			Dim f As File = Resources.asFile("regression_testing/100b6/SyntheticCNN_100b6.bin")
			Dim net As MultiLayerNetwork = MultiLayerNetwork.load(f, True)

			Dim l0 As ConvolutionLayer = CType(net.getLayer(0).conf().getLayer(), ConvolutionLayer)
			assertEquals(New ActivationReLU(), l0.getActivationFn())
			assertEquals(4, l0.getNOut())
			assertEquals(New WeightInitXavier(), l0.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l0))
			assertEquals(New Adam(0.005), l0.getIUpdater())
			assertArrayEquals(New Integer(){3, 3}, l0.getKernelSize())
			assertArrayEquals(New Integer(){2, 1}, l0.getStride())
			assertArrayEquals(New Integer(){1, 1}, l0.getDilation())
			assertArrayEquals(New Integer(){0, 0}, l0.getPadding())

			Dim l1 As SeparableConvolution2D = CType(net.getLayer(1).conf().getLayer(), SeparableConvolution2D)
			assertEquals(New ActivationReLU(), l1.getActivationFn())
			assertEquals(8, l1.getNOut())
			assertEquals(New WeightInitXavier(), l1.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l1))
			assertEquals(New Adam(0.005), l1.getIUpdater())
			assertArrayEquals(New Integer(){3, 3}, l1.getKernelSize())
			assertArrayEquals(New Integer(){1, 1}, l1.getStride())
			assertArrayEquals(New Integer(){1, 1}, l1.getDilation())
			assertArrayEquals(New Integer(){0, 0}, l1.getPadding())
			assertEquals(ConvolutionMode.Same, l1.getConvolutionMode())
			assertEquals(1, l1.getDepthMultiplier())

			Dim l2 As SubsamplingLayer = CType(net.getLayer(2).conf().getLayer(), SubsamplingLayer)
			assertArrayEquals(New Integer(){3, 3}, l2.getKernelSize())
			assertArrayEquals(New Integer(){2, 2}, l2.getStride())
			assertArrayEquals(New Integer(){1, 1}, l2.getDilation())
			assertArrayEquals(New Integer(){0, 0}, l2.getPadding())
			assertEquals(PoolingType.MAX, l2.getPoolingType())

			Dim l3 As ZeroPaddingLayer = CType(net.getLayer(3).conf().getLayer(), ZeroPaddingLayer)
			assertArrayEquals(New Integer(){4, 4, 4, 4}, l3.getPadding())

			Dim l4 As Upsampling2D = CType(net.getLayer(4).conf().getLayer(), Upsampling2D)
			assertArrayEquals(New Integer(){3, 3}, l4.getSize())

			Dim l5 As DepthwiseConvolution2D = CType(net.getLayer(5).conf().getLayer(), DepthwiseConvolution2D)
			assertEquals(New ActivationReLU(), l5.getActivationFn())
			assertEquals(16, l5.getNOut())
			assertEquals(New WeightInitXavier(), l5.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l5))
			assertEquals(New Adam(0.005), l5.getIUpdater())
			assertArrayEquals(New Integer(){3, 3}, l5.getKernelSize())
			assertArrayEquals(New Integer(){1, 1}, l5.getStride())
			assertArrayEquals(New Integer(){1, 1}, l5.getDilation())
			assertArrayEquals(New Integer(){0, 0}, l5.getPadding())
			assertEquals(2, l5.getDepthMultiplier())

			Dim l6 As SubsamplingLayer = CType(net.getLayer(6).conf().getLayer(), SubsamplingLayer)
			assertArrayEquals(New Integer(){2, 2}, l6.getKernelSize())
			assertArrayEquals(New Integer(){2, 2}, l6.getStride())
			assertArrayEquals(New Integer(){1, 1}, l6.getDilation())
			assertArrayEquals(New Integer(){0, 0}, l6.getPadding())
			assertEquals(PoolingType.MAX, l6.getPoolingType())

			Dim l7 As Cropping2D = CType(net.getLayer(7).conf().getLayer(), Cropping2D)
			assertArrayEquals(New Integer(){3, 3, 2, 2}, l7.getCropping())

			Dim l8 As ConvolutionLayer = CType(net.getLayer(8).conf().getLayer(), ConvolutionLayer)
			assertEquals(4, l8.getNOut())
			assertEquals(New WeightInitXavier(), l8.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l8))
			assertEquals(New Adam(0.005), l8.getIUpdater())
			assertArrayEquals(New Integer(){4, 4}, l8.getKernelSize())
			assertArrayEquals(New Integer(){1, 1}, l8.getStride())
			assertArrayEquals(New Integer(){1, 1}, l8.getDilation())
			assertArrayEquals(New Integer(){0, 0}, l8.getPadding())

			Dim l9 As CnnLossLayer = CType(net.getLayer(9).conf().getLayer(), CnnLossLayer)
			assertEquals(New WeightInitXavier(), l9.getWeightInitFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l9))
			assertEquals(New Adam(0.005), l9.getIUpdater())
			assertEquals(New LossMAE(), l9.getLossFn())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100b6/SyntheticCNN_Output_100b6.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100b6/SyntheticCNN_Input_100b6.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			Dim outAct As INDArray = net.output([in])

			'19 layers - CPU vs. GPU difference accumulates notably, but appears to be correct
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			If Nd4j.Backend.GetType().FullName.ToLower().contains("native") Then
				assertEquals(outExp, outAct)
			Else
				Dim eq As Boolean = outExp.equalsWithEps(outAct, 0.1)
				assertTrue(eq)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSyntheticBidirectionalRNNGraph() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSyntheticBidirectionalRNNGraph()

			Dim f As File = Resources.asFile("regression_testing/100b6/SyntheticBidirectionalRNNGraph_100b6.bin")
			Dim net As ComputationGraph = ComputationGraph.load(f, True)

			Dim l0 As Bidirectional = CType(net.getLayer("rnn1").conf().getLayer(), Bidirectional)

			Dim l1 As LSTM = CType(l0.getFwd(), LSTM)
			assertEquals(16, l1.getNOut())
			assertEquals(New ActivationReLU(), l1.getActivationFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l1))

			Dim l2 As LSTM = CType(l0.getBwd(), LSTM)
			assertEquals(16, l2.getNOut())
			assertEquals(New ActivationReLU(), l2.getActivationFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l2))

			Dim l3 As Bidirectional = CType(net.getLayer("rnn2").conf().getLayer(), Bidirectional)

			Dim l4 As SimpleRnn = CType(l3.getFwd(), SimpleRnn)
			assertEquals(16, l4.getNOut())
			assertEquals(New ActivationReLU(), l4.getActivationFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l4))

			Dim l5 As SimpleRnn = CType(l3.getBwd(), SimpleRnn)
			assertEquals(16, l5.getNOut())
			assertEquals(New ActivationReLU(), l5.getActivationFn())
			assertEquals(New L2Regularization(0.0001), TestUtils.getL2Reg(l5))

			Dim mv As MergeVertex = DirectCast(net.getVertex("concat"), MergeVertex)

			Dim gpl As GlobalPoolingLayer = CType(net.getLayer("pooling").conf().getLayer(), GlobalPoolingLayer)
			assertEquals(PoolingType.MAX, gpl.getPoolingType())
			assertArrayEquals(New Integer(){2}, gpl.getPoolingDimensions())
			assertTrue(gpl.isCollapseDimensions())

			Dim outl As OutputLayer = CType(net.getLayer("out").conf().getLayer(), OutputLayer)
			assertEquals(3, outl.getNOut())
			assertEquals(New LossMCXENT(), outl.getLossFn())

			Dim outExp As INDArray
			Dim f2 As File = Resources.asFile("regression_testing/100b6/SyntheticBidirectionalRNNGraph_Output_100b6.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f2, FileMode.Open, FileAccess.Read))
				outExp = Nd4j.read(dis)
			End Using

			Dim [in] As INDArray
			Dim f3 As File = Resources.asFile("regression_testing/100b6/SyntheticBidirectionalRNNGraph_Input_100b6.bin")
			Using dis As New java.io.DataInputStream(New FileStream(f3, FileMode.Open, FileAccess.Read))
				[in] = Nd4j.read(dis)
			End Using

			Dim outAct As INDArray = net.output([in])(0)

			assertEquals(outExp, outAct)
		End Sub
	End Class

End Namespace