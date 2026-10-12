Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports org.deeplearning4j.datasets.iterator.impl
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Updater = org.deeplearning4j.nn.conf.Updater
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BatchNormalizationParamInitializer = org.deeplearning4j.nn.params.BatchNormalizationParamInitializer
Imports FineTuneConfiguration = org.deeplearning4j.nn.transferlearning.FineTuneConfiguration
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports MultiLayerUpdater = org.deeplearning4j.nn.updater.MultiLayerUpdater
Imports UpdaterBlock = org.deeplearning4j.nn.updater.UpdaterBlock
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports org.junit.jupiter.api
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastAddOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAddOp
Imports BroadcastDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastDivOp
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports BroadcastSubOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastSubOp
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOpUpdater = org.nd4j.linalg.learning.NoOpUpdater
Imports RmsPropUpdater = org.nd4j.linalg.learning.RmsPropUpdater
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports org.junit.jupiter.api.Assertions
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.layers.normalization

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Batch Normalization Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) @Disabled("C++ error on gpu with bacprop") class BatchNormalizationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class BatchNormalizationTest
		Inherits BaseDL4JTest

		Shared Sub New()
			' Force Nd4j initialization, then set data type to double:
			Nd4j.zeros(1)
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
		End Sub

		Protected Friend dnnInput As INDArray = Nd4j.linspace(0, 31, 32, Nd4j.dataType()).reshape(ChrW(2), 16)

		Protected Friend dnnEpsilon As INDArray = Nd4j.linspace(0, 31, 32, Nd4j.dataType()).reshape(ChrW(2), 16)

		Protected Friend cnnInput As INDArray = Nd4j.linspace(0, 63, 64, Nd4j.dataType()).reshape(ChrW(2), 2, 4, 4)

		Protected Friend cnnEpsilon As INDArray = Nd4j.linspace(0, 63, 64, Nd4j.dataType()).reshape(ChrW(2), 2, 4, 4)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void doBefore()
		Friend Overridable Sub doBefore()
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dnn Forward Pass") void testDnnForwardPass()
		Friend Overridable Sub testDnnForwardPass()
			Dim nOut As Integer = 10
			Dim l As Layer = getLayer(nOut, 0.0, False, -1, -1)
			' Gamma, beta, global mean, global var
			assertEquals(4 * nOut, l.numParams())
			Dim randInput As INDArray = Nd4j.rand(100, nOut)
			Dim output As INDArray = l.activate(randInput, True, LayerWorkspaceMgr.noWorkspaces())
			Dim mean As INDArray = output.mean(0)
			Dim stdev As INDArray = output.std(False, 0)
			' System.out.println(Arrays.toString(mean.data().asFloat()));
			assertArrayEquals(New Single(nOut - 1){}, mean.data().asFloat(), 1e-6f)
			assertEquals(Nd4j.ones(nOut), stdev)
			' If we fix gamma/beta: expect different mean and variance...
			Dim gamma As Double = 2.0
			Dim beta As Double = 3.0
			l = getLayer(nOut, 0.0, True, gamma, beta)
			' Should have only global mean/var parameters
			assertEquals(2 * nOut, l.numParams())
			output = l.activate(randInput, True, LayerWorkspaceMgr.noWorkspaces())
			mean = output.mean(0)
			stdev = output.std(False, 0)
			assertEquals(Nd4j.valueArrayOf(mean.shape(), beta), mean)
			assertEquals(Nd4j.valueArrayOf(stdev.shape(), gamma), stdev)
		End Sub

		Protected Friend Shared Function getLayer(ByVal nOut As Integer, ByVal epsilon As Double, ByVal lockGammaBeta As Boolean, ByVal gamma As Double, ByVal beta As Double) As Layer
			Dim b As BatchNormalization.Builder = (New BatchNormalization.Builder()).nOut(nOut).eps(epsilon)
			If lockGammaBeta Then
				b.lockGammaBeta(True).gamma(gamma).beta(beta)
			End If
			Dim bN As BatchNormalization = b.build()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer(bN).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nothing
			If numParams > 0 Then
				params = Nd4j.create(1, numParams)
			End If
			Dim layer As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True,If(params Is Nothing, Nd4j.defaultFloatingPointType(), params.dataType()))
			If numParams > 0 Then
				layer.BackpropGradientsViewArray = Nd4j.create(1, numParams)
			End If
			Return layer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dnn Forward Backward") void testDnnForwardBackward()
		Friend Overridable Sub testDnnForwardBackward()
			Dim eps As Double = 1e-5
			Dim nIn As Integer = 4
			Dim minibatch As Integer = 2
			Nd4j.Random.setSeed(12345)
			Dim input As INDArray = Nd4j.rand("c"c, New Integer() { minibatch, nIn })
			' TODO: other values for gamma/beta
			Dim gamma As INDArray = Nd4j.ones(1, nIn)
			Dim beta As INDArray = Nd4j.zeros(1, nIn)
			Dim l As Layer = getLayer(nIn, eps, False, -1, -1)
			Dim mean As INDArray = input.mean(0)
			Dim var As INDArray = input.var(False, 0)
			Dim xHat As INDArray = input.subRowVector(mean).divRowVector(Transforms.sqrt(var.add(eps), True))
			Dim outExpected As INDArray = xHat.mulRowVector(gamma).addRowVector(beta)
			Dim [out] As INDArray = l.activate(input, True, LayerWorkspaceMgr.noWorkspaces())
			' System.out.println(Arrays.toString(outExpected.data().asDouble()));
			' System.out.println(Arrays.toString(out.data().asDouble()));
			assertEquals(outExpected, [out])
			' -------------------------------------------------------------
			' Check backprop
			' dL/dy
			Dim epsilon As INDArray = Nd4j.rand(minibatch, nIn)
			Dim dldgammaExp As INDArray = epsilon.mul(xHat).sum(True, 0)
			Dim dldbetaExp As INDArray = epsilon.sum(True, 0)
			Dim dldxhat As INDArray = epsilon.mulRowVector(gamma)
			Dim dldvar As INDArray = dldxhat.mul(input.subRowVector(mean)).mul(-0.5).mulRowVector(Transforms.pow(var.add(eps), -3.0 / 2.0, True)).sum(0)
			Dim dldmu As INDArray = dldxhat.mulRowVector(Transforms.pow(var.add(eps), -1.0 / 2.0, True)).neg().sum(0).add(dldvar.mul(input.subRowVector(mean).mul(-2.0).sum(0).div(minibatch)))
			Dim dldinExp As INDArray = dldxhat.mulRowVector(Transforms.pow(var.add(eps), -1.0 / 2.0, True)).add(input.subRowVector(mean).mul(2.0 / minibatch).mulRowVector(dldvar)).addRowVector(dldmu.mul(1.0 / minibatch))
			Dim p As Pair(Of Gradient, INDArray) = l.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			Dim dldgamma As INDArray = p.First.getGradientFor("gamma")
			Dim dldbeta As INDArray = p.First.getGradientFor("beta")
			assertEquals(dldgammaExp, dldgamma)
			assertEquals(dldbetaExp, dldbeta)
			' System.out.println("EPSILONS");
			' System.out.println(Arrays.toString(dldinExp.data().asDouble()));
			' System.out.println(Arrays.toString(p.getSecond().dup().data().asDouble()));
			assertEquals(dldinExp, p.Second)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn Forward Pass") void testCnnForwardPass()
		Friend Overridable Sub testCnnForwardPass()
			Dim nOut As Integer = 10
			Dim l As Layer = getLayer(nOut, 0.0, False, -1, -1)
			' Gamma, beta, global mean, global var
			assertEquals(4 * nOut, l.numParams())
			Dim hw As Integer = 15
			Nd4j.Random.setSeed(12345)
			Dim randInput As INDArray = Nd4j.rand(New Integer() { 100, nOut, hw, hw })
			Dim output As INDArray = l.activate(randInput, True, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(4, output.rank())
			Dim mean As INDArray = output.mean(0, 2, 3)
			Dim stdev As INDArray = output.std(False, 0, 2, 3)
			assertArrayEquals(New Single(nOut - 1){}, mean.data().asFloat(), 1e-6f)
			assertArrayEquals(Nd4j.ones(1, nOut).data().asFloat(), stdev.data().asFloat(), 1e-6f)
			' If we fix gamma/beta: expect different mean and variance...
			Dim gamma As Double = 2.0
			Dim beta As Double = 3.0
			l = getLayer(nOut, 0.0, True, gamma, beta)
			' Should have only global mean/var parameters
			assertEquals(2 * nOut, l.numParams())
			output = l.activate(randInput, True, LayerWorkspaceMgr.noWorkspaces())
			mean = output.mean(0, 2, 3)
			stdev = output.std(False, 0, 2, 3)
			assertEquals(Nd4j.valueArrayOf(mean.shape(), beta), mean)
			assertEquals(Nd4j.valueArrayOf(stdev.shape(), gamma), stdev)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test 2 d Vs 4 d") void test2dVs4d()
		Friend Overridable Sub test2dVs4d()
			' Idea: 2d and 4d should be the same...
			Nd4j.Random.setSeed(12345)
			Dim m As Integer = 2
			Dim h As Integer = 3
			Dim w As Integer = 3
			Dim nOut As Integer = 2
			Dim [in] As INDArray = Nd4j.rand("c"c, m * h * w, nOut)
			Dim in4 As INDArray = [in].dup()
			in4 = Shape.newShapeNoCopy(in4, New Integer() { m, h, w, nOut }, False)
			assertNotNull(in4)
			in4 = in4.permute(0, 3, 1, 2).dup()
			Dim arr As INDArray = Nd4j.rand(1, m * h * w * nOut).reshape("f"c, h, w, m, nOut).permute(2, 3, 1, 0)
			in4 = arr.assign(in4)
			Dim l1 As Layer = getLayer(nOut)
			Dim l2 As Layer = getLayer(nOut)
			Dim out2d As INDArray = l1.activate([in].dup(), True, LayerWorkspaceMgr.noWorkspaces())
			Dim out4d As INDArray = l2.activate(in4.dup(), True, LayerWorkspaceMgr.noWorkspaces())
			Dim out4dAs2 As INDArray = out4d.permute(0, 2, 3, 1).dup("c"c)
			out4dAs2 = Shape.newShapeNoCopy(out4dAs2, New Integer() { m * h * w, nOut }, False)
			assertEquals(out2d, out4dAs2)
			' Test backprop:
			Dim epsilons2d As INDArray = Nd4j.rand("c"c, m * h * w, nOut)
			Dim epsilons4d As INDArray = epsilons2d.dup()
			epsilons4d = Shape.newShapeNoCopy(epsilons4d, New Integer() { m, h, w, nOut }, False)
			assertNotNull(epsilons4d)
			epsilons4d = epsilons4d.permute(0, 3, 1, 2).dup()
			Dim b2d As Pair(Of Gradient, INDArray) = l1.backpropGradient(epsilons2d, LayerWorkspaceMgr.noWorkspaces())
			Dim b4d As Pair(Of Gradient, INDArray) = l2.backpropGradient(epsilons4d, LayerWorkspaceMgr.noWorkspaces())
			Dim e4dAs2d As INDArray = b4d.Second.permute(0, 2, 3, 1).dup("c"c)
			e4dAs2d = Shape.newShapeNoCopy(e4dAs2d, New Integer() { m * h * w, nOut }, False)
			assertEquals(b2d.Second, e4dAs2d)
		End Sub

		Protected Friend Shared Function getLayer(ByVal nOut As Integer) As Layer
			Return getLayer(nOut, Nd4j.EPS_THRESHOLD, False, -1, -1)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn Forward Backward") void testCnnForwardBackward()
		Friend Overridable Sub testCnnForwardBackward()
			Dim eps As Double = 1e-5
			Dim nIn As Integer = 4
			Dim hw As Integer = 3
			Dim minibatch As Integer = 2
			Nd4j.Random.setSeed(12345)
			Dim input As INDArray = Nd4j.rand("c"c, New Integer() { minibatch, nIn, hw, hw })
			' TODO: other values for gamma/beta
			Dim gamma As INDArray = Nd4j.ones(1, nIn)
			Dim beta As INDArray = Nd4j.zeros(1, nIn)
			Dim l As Layer = getLayer(nIn, eps, False, -1, -1)
			Dim mean As INDArray = input.mean(0, 2, 3)
			Dim var As INDArray = input.var(False, 0, 2, 3)
			Dim xHat As INDArray = Nd4j.Executioner.exec(New BroadcastSubOp(input, mean, input.dup(), 1))
			Nd4j.Executioner.exec(New BroadcastDivOp(xHat, Transforms.sqrt(var.add(eps), True), xHat, 1))
			Dim outExpected As INDArray = Nd4j.Executioner.exec(New BroadcastMulOp(xHat, gamma, xHat.dup(), 1))
			Nd4j.Executioner.exec(New BroadcastAddOp(outExpected, beta, outExpected, 1))
			Dim [out] As INDArray = l.activate(input, True, LayerWorkspaceMgr.noWorkspaces())
			' System.out.println(Arrays.toString(outExpected.data().asDouble()));
			' System.out.println(Arrays.toString(out.data().asDouble()));
			assertEquals(outExpected, [out])
			' -------------------------------------------------------------
			' Check backprop
			' dL/dy
			Dim epsilon As INDArray = Nd4j.rand("c"c, New Integer() { minibatch, nIn, hw, hw })
			Dim effectiveMinibatch As Integer = minibatch * hw * hw
			Dim dldgammaExp As INDArray = epsilon.mul(xHat).sum(0, 2, 3)
			dldgammaExp = dldgammaExp.reshape(ChrW(1), dldgammaExp.length())
			Dim dldbetaExp As INDArray = epsilon.sum(0, 2, 3)
			dldbetaExp = dldbetaExp.reshape(ChrW(1), dldbetaExp.length())
			' epsilon.mulRowVector(gamma);
			Dim dldxhat As INDArray = Nd4j.Executioner.exec(New BroadcastMulOp(epsilon, gamma, epsilon.dup(), 1))
			Dim inputSubMean As INDArray = Nd4j.Executioner.exec(New BroadcastSubOp(input, mean, input.dup(), 1))
			Dim dldvar As INDArray = dldxhat.mul(inputSubMean).mul(-0.5)
			dldvar = Nd4j.Executioner.exec(New BroadcastMulOp(dldvar, Transforms.pow(var.add(eps), -3.0 / 2.0, True), dldvar.dup(), 1))
			dldvar = dldvar.sum(0, 2, 3)
			Dim dldmu As INDArray = Nd4j.Executioner.exec(New BroadcastMulOp(dldxhat, Transforms.pow(var.add(eps), -1.0 / 2.0, True), dldxhat.dup(), 1)).neg().sum(0, 2, 3)
			dldmu = dldmu.add(dldvar.mul(inputSubMean.mul(-2.0).sum(0, 2, 3).div(effectiveMinibatch)))
			Dim dldinExp As INDArray = Nd4j.Executioner.exec(New BroadcastMulOp(dldxhat, Transforms.pow(var.add(eps), -1.0 / 2.0, True), dldxhat.dup(), 1))
			dldinExp = dldinExp.add(Nd4j.Executioner.exec(New BroadcastMulOp(inputSubMean.mul(2.0 / effectiveMinibatch), dldvar, inputSubMean.dup(), 1)))
			dldinExp = Nd4j.Executioner.exec(New BroadcastAddOp(dldinExp, dldmu.mul(1.0 / effectiveMinibatch), dldinExp.dup(), 1))
			Dim p As Pair(Of Gradient, INDArray) = l.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			Dim dldgamma As INDArray = p.First.getGradientFor("gamma")
			Dim dldbeta As INDArray = p.First.getGradientFor("beta")
			assertEquals(dldgammaExp, dldgamma)
			assertEquals(dldbetaExp, dldbeta)
			' System.out.println("EPSILONS");
			' System.out.println(Arrays.toString(dldinExp.data().asDouble()));
			' System.out.println(Arrays.toString(p.getSecond().dup().data().asDouble()));
			assertEquals(dldinExp, p.Second)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test DBNBN Multi Layer") void testDBNBNMultiLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testDBNBNMultiLayer()
			Dim iter As DataSetIterator = New MnistDataSetIterator(2, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			' Run with separate activation layer
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28).nOut(10).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New BatchNormalization.Builder()).nOut(10).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			network.Input = [next].Features
			Dim activationsActual As INDArray = network.output([next].Features)
			assertEquals(10, activationsActual.shape()(1), 1e-2)
			network.fit([next])
			Dim actualGammaParam As INDArray = network.getLayer(1).getParam(BatchNormalizationParamInitializer.GAMMA)
			Dim actualBetaParam As INDArray = network.getLayer(1).getParam(BatchNormalizationParamInitializer.BETA)
			assertTrue(actualGammaParam IsNot Nothing)
			assertTrue(actualBetaParam IsNot Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNNBN Activation Combo") void testCNNBNActivationCombo() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCNNBNActivationCombo()
			Dim iter As DataSetIterator = New MnistDataSetIterator(2, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New ConvolutionLayer.Builder()).nIn(1).nOut(6).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			network.fit([next])
			assertNotEquals(Nothing, network.getLayer(0).getParam("W"))
			assertNotEquals(Nothing, network.getLayer(0).getParam("b"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Check Serialization") void checkSerialization() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub checkSerialization()
			' Serialize the batch norm network (after training), and make sure we get same activations out as before
			' i.e., make sure state is properly stored
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New ConvolutionLayer.Builder()).nIn(1).nOut(6).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.LEAKYRELU).build()).layer(3, (New DenseLayer.Builder()).nOut(10).activation(Activation.LEAKYRELU).build()).layer(4, (New BatchNormalization.Builder()).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim iter As DataSetIterator = New MnistDataSetIterator(16, True, 12345)
			For i As Integer = 0 To 19
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				net.fit(iter.next())
			Next i
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [in] As INDArray = iter.next().getFeatures()
			Dim [out] As INDArray = net.output([in], False)
			Dim out2 As INDArray = net.output([in], False)
			assertEquals([out], out2)
			Dim net2 As MultiLayerNetwork = TestUtils.testModelSerialization(net)
			Dim outDeser As INDArray = net2.output([in], False)
			assertEquals([out], outDeser)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient And Updaters") void testGradientAndUpdaters() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGradientAndUpdaters()
			' Global mean/variance are part of the parameter vector. Expect 0 gradient, and no-op updater for these
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(Updater.RMSPROP).seed(12345).list().layer(0, (New ConvolutionLayer.Builder()).nIn(1).nOut(6).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.LEAKYRELU).build()).layer(3, (New DenseLayer.Builder()).nOut(10).activation(Activation.LEAKYRELU).build()).layer(4, (New BatchNormalization.Builder()).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim iter As DataSetIterator = New MnistDataSetIterator(16, True, 12345)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			net.Input = ds.Features
			net.Labels = ds.Labels
			net.computeGradientAndScore()
			Dim g As Gradient = net.gradient()
			Dim map As IDictionary(Of String, INDArray) = g.gradientForVariable()
			Dim u As org.deeplearning4j.nn.api.Updater = net.Updater
			Dim mlu As MultiLayerUpdater = DirectCast(u, MultiLayerUpdater)
			Dim l As IList(Of UpdaterBlock) = mlu.getUpdaterBlocks()
			assertNotNull(l)
			' Conv+bn (RMSProp), No-op (bn), RMSProp (dense, bn), no-op (bn), RMSProp (out)
			assertEquals(5, l.Count)
			For Each ub As UpdaterBlock In l
				Dim list As IList(Of UpdaterBlock.ParamState) = ub.getLayersAndVariablesInBlock()
				For Each v As UpdaterBlock.ParamState In list
					If BatchNormalizationParamInitializer.GLOBAL_MEAN.Equals(v.getParamName()) OrElse BatchNormalizationParamInitializer.GLOBAL_VAR.Equals(v.getParamName()) OrElse BatchNormalizationParamInitializer.GLOBAL_LOG_STD.Equals(v.getParamName()) Then
						assertTrue(TypeOf ub.GradientUpdater Is NoOpUpdater)
					Else
						assertTrue(TypeOf ub.GradientUpdater Is RmsPropUpdater)
					End If
				Next v
			Next ub
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Check Mean Variance Estimate") void checkMeanVarianceEstimate() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub checkMeanVarianceEstimate()
			Nd4j.Random.setSeed(12345)
			' Check that the internal global mean/variance estimate is approximately correct
			For Each useLogStd As Boolean In New Boolean() { True, False }
				' First, Mnist data as 2d input (NOT taking into account convolution property)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(Updater.RMSPROP).seed(12345).list().layer(0, (New BatchNormalization.Builder()).nIn(10).nOut(10).eps(1e-5).decay(0.95).useLogStd(useLogStd).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).nIn(10).nOut(10).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim minibatch As Integer = 32
				Dim list As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 199
					list.Add(New DataSet(Nd4j.rand(minibatch, 10), Nd4j.rand(minibatch, 10)))
				Next i
				Dim iter As DataSetIterator = New ListDataSetIterator(list)
				Dim expMean As INDArray = Nd4j.valueArrayOf(New Integer() { 1, 10 }, 0.5)
				' Expected variance of U(0,1) distribution: 1/12 * (1-0)^2 = 0.0833
				Dim expVar As INDArray = Nd4j.valueArrayOf(New Integer() { 1, 10 }, 1 / 12.0)
				For i As Integer = 0 To 9
					iter.reset()
					net.fit(iter)
				Next i
				Dim estMean As INDArray = net.getLayer(0).getParam(BatchNormalizationParamInitializer.GLOBAL_MEAN)
				Dim estVar As INDArray
				If useLogStd Then
					Dim log10std As INDArray = net.getLayer(0).getParam(BatchNormalizationParamInitializer.GLOBAL_LOG_STD)
					estVar = Nd4j.valueArrayOf(log10std.shape(), 10.0).castTo(log10std.dataType())
					' stdev = 10^(log10(stdev))
					Transforms.pow(estVar, log10std, False)
					estVar.muli(estVar)
				Else
					estVar = net.getLayer(0).getParam(BatchNormalizationParamInitializer.GLOBAL_VAR)
				End If
				Dim fMeanExp() As Single = expMean.data().asFloat()
				Dim fMeanAct() As Single = estMean.data().asFloat()
				Dim fVarExp() As Single = expVar.data().asFloat()
				Dim fVarAct() As Single = estVar.data().asFloat()
				' System.out.println("Mean vs. estimated mean:");
				' System.out.println(Arrays.toString(fMeanExp));
				' System.out.println(Arrays.toString(fMeanAct));
				' 
				' System.out.println("Var vs. estimated var:");
				' System.out.println(Arrays.toString(fVarExp));
				' System.out.println(Arrays.toString(fVarAct));
				assertArrayEquals(fMeanExp, fMeanAct, 0.02f)
				assertArrayEquals(fVarExp, fVarAct, 0.02f)
			Next useLogStd
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Check Mean Variance Estimate CNN") void checkMeanVarianceEstimateCNN() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub checkMeanVarianceEstimateCNN()
			For Each useLogStd As Boolean In New Boolean() { True, False }
				Nd4j.Random.setSeed(12345)
				' Check that the internal global mean/variance estimate is approximately correct
				' First, Mnist data as 2d input (NOT taking into account convolution property)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(Updater.RMSPROP).seed(12345).list().layer(0, (New BatchNormalization.Builder()).nIn(3).nOut(3).eps(1e-5).decay(0.95).useLogStd(useLogStd).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).nOut(10).build()).setInputType(InputType.convolutional(5, 5, 3)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim minibatch As Integer = 32
				Dim list As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 99
					list.Add(New DataSet(Nd4j.rand(New Integer() { minibatch, 3, 5, 5 }), Nd4j.rand(minibatch, 10)))
				Next i
				Dim iter As DataSetIterator = New ListDataSetIterator(list)
				Dim expMean As INDArray = Nd4j.valueArrayOf(New Integer() { 1, 3 }, 0.5)
				' Expected variance of U(0,1) distribution: 1/12 * (1-0)^2 = 0.0833
				Dim expVar As INDArray = Nd4j.valueArrayOf(New Integer() { 1, 3 }, 1 / 12.0)
				For i As Integer = 0 To 9
					iter.reset()
					net.fit(iter)
				Next i
				Dim estMean As INDArray = net.getLayer(0).getParam(BatchNormalizationParamInitializer.GLOBAL_MEAN)
				Dim estVar As INDArray
				If useLogStd Then
					Dim log10std As INDArray = net.getLayer(0).getParam(BatchNormalizationParamInitializer.GLOBAL_LOG_STD)
					estVar = Nd4j.valueArrayOf(log10std.shape(), 10.0).castTo(log10std.dataType())
					' stdev = 10^(log10(stdev))
					Transforms.pow(estVar, log10std, False)
					estVar.muli(estVar)
				Else
					estVar = net.getLayer(0).getParam(BatchNormalizationParamInitializer.GLOBAL_VAR)
				End If
				Dim fMeanExp() As Single = expMean.data().asFloat()
				Dim fMeanAct() As Single = estMean.data().asFloat()
				Dim fVarExp() As Single = expVar.data().asFloat()
				Dim fVarAct() As Single = estVar.data().asFloat()
				' System.out.println("Mean vs. estimated mean:");
				' System.out.println(Arrays.toString(fMeanExp));
				' System.out.println(Arrays.toString(fMeanAct));
				' 
				' System.out.println("Var vs. estimated var:");
				' System.out.println(Arrays.toString(fVarExp));
				' System.out.println(Arrays.toString(fVarAct));
				assertArrayEquals(fMeanExp, fMeanAct, 0.01f)
				assertArrayEquals(fVarExp, fVarAct, 0.01f)
			Next useLogStd
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Check Mean Variance Estimate CNN Compare Modes") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) void checkMeanVarianceEstimateCNNCompareModes() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub checkMeanVarianceEstimateCNNCompareModes()
			Nd4j.Random.setSeed(12345)
			' Check that the internal global mean/variance estimate is approximately correct
			' First, Mnist data as 2d input (NOT taking into account convolution property)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(Updater.RMSPROP).seed(12345).list().layer(0, (New BatchNormalization.Builder()).nIn(3).nOut(3).eps(1e-5).decay(0.95).useLogStd(False).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).nOut(10).build()).setInputType(InputType.convolutional(5, 5, 3)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Nd4j.Random.setSeed(12345)
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(Updater.RMSPROP).seed(12345).list().layer(0, (New BatchNormalization.Builder()).nIn(3).nOut(3).eps(1e-5).decay(0.95).useLogStd(True).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).nOut(10).build()).setInputType(InputType.convolutional(5, 5, 3)).build()
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()
			Dim minibatch As Integer = 32
			For i As Integer = 0 To 9
				Dim ds As New DataSet(Nd4j.rand(New Integer() { minibatch, 3, 5, 5 }), Nd4j.rand(minibatch, 10))
				net.fit(ds)
				net2.fit(ds)
				Dim globalVar As INDArray = net.getParam("0_" & BatchNormalizationParamInitializer.GLOBAL_VAR)
				Dim log10std As INDArray = net2.getParam("0_" & BatchNormalizationParamInitializer.GLOBAL_LOG_STD)
				Dim globalVar2 As INDArray = Nd4j.valueArrayOf(log10std.shape(), 10.0).castTo(log10std.dataType())
				' stdev = 10^(log10(stdev))
				Transforms.pow(globalVar2, log10std, False)
				globalVar2.muli(globalVar2)
				assertEquals(globalVar, globalVar2)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Batch Norm") void testBatchNorm() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBatchNorm()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(1e-3)).activation(Activation.TANH).list().layer((New ConvolutionLayer.Builder()).nOut(5).kernelSize(2, 2).build()).layer(New BatchNormalization()).layer((New ConvolutionLayer.Builder()).nOut(5).kernelSize(2, 2).build()).layer((New OutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim iter As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(32, True, 12345), 10)
			net.fit(iter)
			Dim net2 As MultiLayerNetwork = (New TransferLearning.Builder(net)).fineTuneConfiguration(FineTuneConfiguration.builder().updater(New AdaDelta()).build()).removeOutputLayer().addLayer((New BatchNormalization.Builder()).nOut(3380).build()).addLayer((New OutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(3380).nOut(10).build()).build()
			net2.fit(iter)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Batch Norm Recurrent Cnn 1 d") void testBatchNormRecurrentCnn1d()
		Friend Overridable Sub testBatchNormRecurrentCnn1d()
			' Simple sanity check on CNN1D and RNN layers
			For Each rnn As Boolean In New Boolean() { True, False }
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).list().layer(If(rnn, (New LSTM.Builder()).nOut(3).build(), (New Convolution1DLayer.Builder()).kernelSize(3).stride(1).nOut(3).build())).layer(New BatchNormalization()).layer((New RnnOutputLayer.Builder()).nOut(3).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build()).setInputType(InputType.recurrent(3)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim [in] As INDArray = Nd4j.rand(New Integer() { 1, 3, 5 })
				Dim label As INDArray = Nd4j.rand(New Integer() { 1, 3, 5 })
				Dim [out] As INDArray = net.output([in])
				assertArrayEquals(New Long() { 1, 3, 5 }, [out].shape())
				net.fit([in], label)
				log.info("OK: {}", (If(rnn, "rnn", "cnn1d")))
			Next rnn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Validation") void testInputValidation()
		Friend Overridable Sub testInputValidation()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New BatchNormalization.Builder()).nIn(10).nOut(10).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim in1 As INDArray = Nd4j.create(1, 10)
			Dim in2 As INDArray = Nd4j.create(1, 5)
			Dim out1 As INDArray = net.output(in1)
			Try
				Dim out2 As INDArray = net.output(in2)
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("expected input"))
			End Try
		End Sub
	End Class

End Namespace