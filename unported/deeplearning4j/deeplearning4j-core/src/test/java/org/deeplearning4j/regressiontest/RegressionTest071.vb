Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports org.deeplearning4j.nn.conf.layers
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports WeightInitRelu = org.deeplearning4j.nn.weights.WeightInitRelu
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports ActivationLReLU = org.nd4j.linalg.activations.impl.ActivationLReLU
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
Imports LossNegativeLogLikelihood = org.nd4j.linalg.lossfunctions.impl.LossNegativeLogLikelihood
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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class RegressionTest071 extends org.deeplearning4j.BaseDL4JTest
	Public Class RegressionTest071
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
'ORIGINAL LINE: @Test public void regressionTestMLP1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub regressionTestMLP1()

			Dim f As File = Resources.asFile("regression_testing/071/071_ModelSerializer_Regression_MLP_1.zip")

			Dim net As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(f, True)

			Dim conf As MultiLayerConfiguration = net.LayerWiseConfigurations
			assertEquals(2, conf.getConfs().size())

			Dim l0 As DenseLayer = CType(conf.getConf(0).getLayer(), DenseLayer)
			assertEquals("relu", l0.getActivationFn().ToString())
			assertEquals(3, l0.getNIn())
			assertEquals(4, l0.getNOut())
			assertEquals(New WeightInitXavier(), l0.getWeightInitFn())
			assertEquals(New Nesterovs(0.15, 0.9), l0.getIUpdater())
			assertEquals(0.15, CType(l0.getIUpdater(), Nesterovs).getLearningRate(), 1e-6)

			Dim l1 As OutputLayer = CType(conf.getConf(1).getLayer(), OutputLayer)
			assertEquals("softmax", l1.getActivationFn().ToString())
			assertTrue(TypeOf l1.getLossFn() Is LossMCXENT)
			assertEquals(4, l1.getNIn())
			assertEquals(5, l1.getNOut())
			assertEquals(New WeightInitXavier(), l1.getWeightInitFn())
			assertEquals(0.9, CType(l1.getIUpdater(), Nesterovs).getMomentum(), 1e-6)
			assertEquals(0.9, CType(l1.getIUpdater(), Nesterovs).getMomentum(), 1e-6)
			assertEquals(0.15, CType(l1.getIUpdater(), Nesterovs).getLearningRate(), 1e-6)

			Dim numParams As Long = CInt(net.numParams())
			assertEquals(Nd4j.linspace(1, numParams, numParams).reshape(ChrW(1), numParams), net.params())
			Dim updaterSize As Integer = CInt((New Nesterovs()).stateSize(numParams))
			assertEquals(Nd4j.linspace(1, updaterSize, updaterSize).reshape(ChrW(1), numParams), net.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void regressionTestMLP2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub regressionTestMLP2()

			Dim f As File = Resources.asFile("regression_testing/071/071_ModelSerializer_Regression_MLP_2.zip")

			Dim net As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(f, True)

			Dim conf As MultiLayerConfiguration = net.LayerWiseConfigurations
			assertEquals(2, conf.getConfs().size())

			Dim l0 As DenseLayer = CType(conf.getConf(0).getLayer(), DenseLayer)
			assertTrue(TypeOf l0.getActivationFn() Is ActivationLReLU)
			assertEquals(3, l0.getNIn())
			assertEquals(4, l0.getNOut())
			assertEquals(New WeightInitDistribution(New NormalDistribution(0.1, 1.2)), l0.getWeightInitFn())
			assertEquals(New RmsProp(0.15, 0.96, RmsProp.DEFAULT_RMSPROP_EPSILON), l0.getIUpdater())
			assertEquals(0.15, CType(l0.getIUpdater(), RmsProp).getLearningRate(), 1e-6)
			assertEquals(New Dropout(0.6), l0.getIDropout())
			assertEquals(0.1, TestUtils.getL1(l0), 1e-6)
			assertEquals(New WeightDecay(0.2,False), TestUtils.getWeightDecayReg(l0))
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, l0.GradientNormalization)
			assertEquals(1.5, l0.GradientNormalizationThreshold, 1e-5)

			Dim l1 As OutputLayer = CType(conf.getConf(1).getLayer(), OutputLayer)
			assertTrue(TypeOf l1.getActivationFn() Is ActivationIdentity)
			assertTrue(TypeOf l1.getLossFn() Is LossMSE)
			assertEquals(4, l1.getNIn())
			assertEquals(5, l1.getNOut())
			assertEquals(New WeightInitDistribution(New NormalDistribution(0.1, 1.2)), l0.getWeightInitFn())
			assertEquals(New RmsProp(0.15, 0.96, RmsProp.DEFAULT_RMSPROP_EPSILON), l1.getIUpdater())
			assertEquals(0.15, CType(l0.getIUpdater(), RmsProp).getLearningRate(), 1e-6)
			assertEquals(New Dropout(0.6), l1.getIDropout())
			assertEquals(0.1, TestUtils.getL1(l1), 1e-6)
			assertEquals(New WeightDecay(0.2,False), TestUtils.getWeightDecayReg(l1))
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, l1.GradientNormalization)
			assertEquals(1.5, l1.GradientNormalizationThreshold, 1e-5)

			Dim numParams As Long = net.numParams()
			assertEquals(Nd4j.linspace(1, numParams, numParams).reshape(ChrW(1), numParams), net.params())
			Dim updaterSize As Integer = CInt((New RmsProp()).stateSize(numParams))
			assertEquals(Nd4j.linspace(1, updaterSize, updaterSize).reshape(ChrW(1), numParams), net.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void regressionTestCNN1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub regressionTestCNN1()

			Dim f As File = Resources.asFile("regression_testing/071/071_ModelSerializer_Regression_CNN_1.zip")

			Dim net As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(f, True)

			Dim conf As MultiLayerConfiguration = net.LayerWiseConfigurations
			assertEquals(3, conf.getConfs().size())

			Dim l0 As ConvolutionLayer = CType(conf.getConf(0).getLayer(), ConvolutionLayer)
			assertEquals("tanh", l0.getActivationFn().ToString())
			assertEquals(3, l0.getNIn())
			assertEquals(3, l0.getNOut())
			assertEquals(New WeightInitRelu(), l0.getWeightInitFn())
			assertEquals(New RmsProp(0.15, 0.96, RmsProp.DEFAULT_RMSPROP_EPSILON), l0.getIUpdater())
			assertEquals(0.15, CType(l0.getIUpdater(), RmsProp).getLearningRate(), 1e-6)
			assertArrayEquals(New Integer() {2, 2}, l0.getKernelSize())
			assertArrayEquals(New Integer() {1, 1}, l0.getStride())
			assertArrayEquals(New Integer() {0, 0}, l0.getPadding())
			assertEquals(ConvolutionMode.Same, l0.getConvolutionMode())

			Dim l1 As SubsamplingLayer = CType(conf.getConf(1).getLayer(), SubsamplingLayer)
			assertArrayEquals(New Integer() {2, 2}, l1.getKernelSize())
			assertArrayEquals(New Integer() {1, 1}, l1.getStride())
			assertArrayEquals(New Integer() {0, 0}, l1.getPadding())
			assertEquals(PoolingType.MAX, l1.getPoolingType())
			assertEquals(l1.getConvolutionMode(), ConvolutionMode.Same)

			Dim l2 As OutputLayer = CType(conf.getConf(2).getLayer(), OutputLayer)
			assertEquals("sigmoid", l2.getActivationFn().ToString())
			assertTrue(TypeOf l2.getLossFn() Is LossNegativeLogLikelihood) 'TODO
			assertEquals(26 * 26 * 3, l2.getNIn())
			assertEquals(5, l2.getNOut())
			assertEquals(New WeightInitRelu(), l0.getWeightInitFn())
			assertEquals(New RmsProp(0.15, 0.96, RmsProp.DEFAULT_RMSPROP_EPSILON), l0.getIUpdater())
			assertEquals(0.15, CType(l0.getIUpdater(), RmsProp).getLearningRate(), 1e-6)

			assertTrue(TypeOf conf.getInputPreProcess(2) Is CnnToFeedForwardPreProcessor)

			Dim numParams As Long = net.numParams()
			assertEquals(Nd4j.linspace(1, numParams, numParams).reshape(ChrW(1), numParams), net.params())
			Dim updaterSize As Integer = CInt((New RmsProp()).stateSize(numParams))
			assertEquals(Nd4j.linspace(1, updaterSize, updaterSize).reshape(ChrW(1), numParams), net.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void regressionTestLSTM1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub regressionTestLSTM1()

			Dim f As File = Resources.asFile("regression_testing/071/071_ModelSerializer_Regression_LSTM_1.zip")

			Dim net As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(f, True)

			Dim conf As MultiLayerConfiguration = net.LayerWiseConfigurations
			assertEquals(3, conf.getConfs().size())

			Dim l0 As GravesLSTM = CType(conf.getConf(0).getLayer(), GravesLSTM)
			assertEquals("tanh", l0.getActivationFn().ToString())
			assertEquals(3, l0.getNIn())
			assertEquals(4, l0.getNOut())
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, l0.GradientNormalization)
			assertEquals(1.5, l0.GradientNormalizationThreshold, 1e-5)

			Dim l1 As GravesBidirectionalLSTM = CType(conf.getConf(1).getLayer(), GravesBidirectionalLSTM)
			assertEquals("softsign", l1.getActivationFn().ToString())
			assertEquals(4, l1.getNIn())
			assertEquals(4, l1.getNOut())
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, l1.GradientNormalization)
			assertEquals(1.5, l1.GradientNormalizationThreshold, 1e-5)

			Dim l2 As RnnOutputLayer = CType(conf.getConf(2).getLayer(), RnnOutputLayer)
			assertEquals(4, l2.getNIn())
			assertEquals(5, l2.getNOut())
			assertEquals("softmax", l2.getActivationFn().ToString())
			assertTrue(TypeOf l2.getLossFn() Is LossMCXENT)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void regressionTestCGLSTM1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub regressionTestCGLSTM1()
			Dim f As File = Resources.asFile("regression_testing/071/071_ModelSerializer_Regression_CG_LSTM_1.zip")

			Dim net As ComputationGraph = ModelSerializer.restoreComputationGraph(f, True)

			Dim conf As ComputationGraphConfiguration = net.Configuration
			assertEquals(3, conf.getVertices().size())

			Dim l0 As GravesLSTM = CType((CType(conf.getVertices().get("0"), LayerVertex)).getLayerConf().getLayer(), GravesLSTM)
			assertEquals("tanh", l0.getActivationFn().ToString())
			assertEquals(3, l0.getNIn())
			assertEquals(4, l0.getNOut())
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, l0.GradientNormalization)
			assertEquals(1.5, l0.GradientNormalizationThreshold, 1e-5)

			Dim l1 As GravesBidirectionalLSTM = CType((CType(conf.getVertices().get("1"), LayerVertex)).getLayerConf().getLayer(), GravesBidirectionalLSTM)
			assertEquals("softsign", l1.getActivationFn().ToString())
			assertEquals(4, l1.getNIn())
			assertEquals(4, l1.getNOut())
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, l1.GradientNormalization)
			assertEquals(1.5, l1.GradientNormalizationThreshold, 1e-5)

			Dim l2 As RnnOutputLayer = CType((CType(conf.getVertices().get("2"), LayerVertex)).getLayerConf().getLayer(), RnnOutputLayer)
			assertEquals(4, l2.getNIn())
			assertEquals(5, l2.getNOut())
			assertEquals("softmax", l2.getActivationFn().ToString())
			assertTrue(TypeOf l2.getLossFn() Is LossMCXENT)
		End Sub
	End Class

End Namespace