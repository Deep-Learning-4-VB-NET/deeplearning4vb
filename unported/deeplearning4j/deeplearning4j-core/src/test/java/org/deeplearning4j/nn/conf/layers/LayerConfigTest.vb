Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports MapSchedule = org.nd4j.linalg.schedule.MapSchedule
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName
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
Namespace org.deeplearning4j.nn.conf.layers

	'
	'    @Test
	'    public void testLearningRatePolicyExponential() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().learningRate(lr)
	'                        .updater(Updater.SGD)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Exponential).lrPolicyDecayRate(lrDecayRate).list()
	'                        .layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Exponential, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Exponential, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'    }
	'
	'    @Test
	'    public void testLearningRatePolicyInverse() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        double power = 3;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().iterations(iterations).learningRate(lr)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Inverse).lrPolicyDecayRate(lrDecayRate)
	'                        .lrPolicyPower(power).list().layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Inverse, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Inverse, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(power, conf.getConf(0).getLrPolicyPower(), 0.0);
	'        assertEquals(power, conf.getConf(1).getLrPolicyPower(), 0.0);
	'    }
	'
	'
	'    @Test
	'    public void testLearningRatePolicySteps() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        double steps = 4;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().iterations(iterations).learningRate(lr)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Step).lrPolicyDecayRate(lrDecayRate)
	'                        .lrPolicySteps(steps).list().layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Step, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Step, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(steps, conf.getConf(0).getLrPolicySteps(), 0.0);
	'        assertEquals(steps, conf.getConf(1).getLrPolicySteps(), 0.0);
	'    }
	'
	'    @Test
	'    public void testLearningRatePolicyPoly() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        double power = 3;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().iterations(iterations).learningRate(lr)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Poly).lrPolicyDecayRate(lrDecayRate)
	'                        .lrPolicyPower(power).list().layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Poly, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Poly, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(power, conf.getConf(0).getLrPolicyPower(), 0.0);
	'        assertEquals(power, conf.getConf(1).getLrPolicyPower(), 0.0);
	'    }
	'
	'    @Test
	'    public void testLearningRatePolicySigmoid() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        double steps = 4;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().iterations(iterations).learningRate(lr)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Sigmoid).lrPolicyDecayRate(lrDecayRate)
	'                        .lrPolicySteps(steps).list().layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Sigmoid, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Sigmoid, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(steps, conf.getConf(0).getLrPolicySteps(), 0.0);
	'        assertEquals(steps, conf.getConf(1).getLrPolicySteps(), 0.0);
	'    }
	'
	'
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Layer Config Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class LayerConfigTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class LayerConfigTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Layer Name") void testLayerName()
		Friend Overridable Sub testLayerName()
			Dim name1 As String = "genisys"
			Dim name2 As String = "bill"
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).name(name1).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).name(name2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(name1, conf.getConf(0).getLayer().getLayerName())
			assertEquals(name2, conf.getConf(1).getLayer().getLayerName())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Activation Layerwise Override") void testActivationLayerwiseOverride()
		Friend Overridable Sub testActivationLayerwiseOverride()
			' Without layerwise override:
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.RELU).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(CType(conf.getConf(0).getLayer(), BaseLayer).getActivationFn().ToString(), "relu")
			assertEquals(CType(conf.getConf(1).getLayer(), BaseLayer).getActivationFn().ToString(), "relu")
			' With
			conf = (New NeuralNetConfiguration.Builder()).activation(Activation.RELU).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).activation(Activation.TANH).build()).build()
			net = New MultiLayerNetwork(conf)
			net.init()
			assertEquals(CType(conf.getConf(0).getLayer(), BaseLayer).getActivationFn().ToString(), "relu")
			assertEquals(CType(conf.getConf(1).getLayer(), BaseLayer).getActivationFn().ToString(), "tanh")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Weight Bias Init Layerwise Override") void testWeightBiasInitLayerwiseOverride()
		Friend Overridable Sub testWeightBiasInitLayerwiseOverride()
			' Without layerwise override:
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.distribution.Distribution defaultDistribution = new org.deeplearning4j.nn.conf.distribution.NormalDistribution(0, 1.0);
			Dim defaultDistribution As Distribution = New NormalDistribution(0, 1.0)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dist(defaultDistribution).biasInit(1).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(New WeightInitDistribution(defaultDistribution), CType(conf.getConf(0).getLayer(), BaseLayer).getWeightInitFn())
			assertEquals(New WeightInitDistribution(defaultDistribution), CType(conf.getConf(1).getLayer(), BaseLayer).getWeightInitFn())
			assertEquals(1, CType(conf.getConf(0).getLayer(), BaseLayer).getBiasInit(), 0.0)
			assertEquals(1, CType(conf.getConf(1).getLayer(), BaseLayer).getBiasInit(), 0.0)
			' With:
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.distribution.Distribution overriddenDistribution = new org.deeplearning4j.nn.conf.distribution.UniformDistribution(0, 1);
			Dim overriddenDistribution As Distribution = New UniformDistribution(0, 1)
			conf = (New NeuralNetConfiguration.Builder()).dist(defaultDistribution).biasInit(1).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).dist(overriddenDistribution).biasInit(0).build()).build()
			net = New MultiLayerNetwork(conf)
			net.init()
			assertEquals(New WeightInitDistribution(defaultDistribution), CType(conf.getConf(0).getLayer(), BaseLayer).getWeightInitFn())
			assertEquals(New WeightInitDistribution(overriddenDistribution), CType(conf.getConf(1).getLayer(), BaseLayer).getWeightInitFn())
			assertEquals(1, CType(conf.getConf(0).getLayer(), BaseLayer).getBiasInit(), 0.0)
			assertEquals(0, CType(conf.getConf(1).getLayer(), BaseLayer).getBiasInit(), 0.0)
		End Sub

	'    
	'    @Test
	'    public void testLrL1L2LayerwiseOverride() {
	'        //Idea: Set some common values for all layers. Then selectively override
	'        // the global config, and check they actually work.
	'
	'        //Learning rate without layerwise override:
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().learningRate(0.3).list()
	'                        .layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(0.3, ((BaseLayer) conf.getConf(0).getLayer()).getLearningRate(), 0.0);
	'        assertEquals(0.3, ((BaseLayer) conf.getConf(1).getLayer()).getLearningRate(), 0.0);
	'
	'        //With:
	'        conf = new NeuralNetConfiguration.Builder().learningRate(0.3).list()
	'                        .layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).learningRate(0.2).build()).build();
	'
	'        net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(0.3, ((BaseLayer) conf.getConf(0).getLayer()).getLearningRate(), 0.0);
	'        assertEquals(0.2, ((BaseLayer) conf.getConf(1).getLayer()).getLearningRate(), 0.0);
	'
	'        //L1 and L2 without layerwise override:
	'        conf = new NeuralNetConfiguration.Builder().l1(0.1).l2(0.2).list()
	'                        .layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(0.1, ((BaseLayer) conf.getConf(0).getLayer()).getL1(), 0.0);
	'        assertEquals(0.1, ((BaseLayer) conf.getConf(1).getLayer()).getL1(), 0.0);
	'        assertEquals(0.2, ((BaseLayer) conf.getConf(0).getLayer()).getL2(), 0.0);
	'        assertEquals(0.2, ((BaseLayer) conf.getConf(1).getLayer()).getL2(), 0.0);
	'
	'        //L1 and L2 with layerwise override:
	'        conf = new NeuralNetConfiguration.Builder().l1(0.1).l2(0.2).list()
	'                        .layer(0, new DenseLayer.Builder().nIn(2).nOut(2).l1(0.9).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).l2(0.8).build()).build();
	'        net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(0.9, ((BaseLayer) conf.getConf(0).getLayer()).getL1(), 0.0);
	'        assertEquals(0.1, ((BaseLayer) conf.getConf(1).getLayer()).getL1(), 0.0);
	'        assertEquals(0.2, ((BaseLayer) conf.getConf(0).getLayer()).getL2(), 0.0);
	'        assertEquals(0.8, ((BaseLayer) conf.getConf(1).getLayer()).getL2(), 0.0);
	'    }
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dropout Layerwise Override") void testDropoutLayerwiseOverride()
		Friend Overridable Sub testDropoutLayerwiseOverride()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(1.0).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(New Dropout(1.0), conf.getConf(0).getLayer().getIDropout())
			assertEquals(New Dropout(1.0), conf.getConf(1).getLayer().getIDropout())
			conf = (New NeuralNetConfiguration.Builder()).dropOut(1.0).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).dropOut(2.0).build()).build()
			net = New MultiLayerNetwork(conf)
			net.init()
			assertEquals(New Dropout(1.0), conf.getConf(0).getLayer().getIDropout())
			assertEquals(New Dropout(2.0), conf.getConf(1).getLayer().getIDropout())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Momentum Layerwise Override") void testMomentumLayerwiseOverride()
		Friend Overridable Sub testMomentumLayerwiseOverride()
			Dim testMomentumAfter As IDictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()
			testMomentumAfter(0) = 0.1
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Nesterovs(1.0, New MapSchedule(ScheduleType.ITERATION, testMomentumAfter))).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(0.1, CType(CType(conf.getConf(0).getLayer(), BaseLayer).getIUpdater(), Nesterovs).getMomentumISchedule().valueAt(0, 0), 0.0)
			assertEquals(0.1, CType(CType(conf.getConf(1).getLayer(), BaseLayer).getIUpdater(), Nesterovs).getMomentumISchedule().valueAt(0, 0), 0.0)
			Dim testMomentumAfter2 As IDictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()
			testMomentumAfter2(0) = 0.2
			conf = (New NeuralNetConfiguration.Builder()).updater(New Nesterovs(1.0, New MapSchedule(ScheduleType.ITERATION, testMomentumAfter))).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).updater(New Nesterovs(1.0, New MapSchedule(ScheduleType.ITERATION, testMomentumAfter2))).build()).build()
			net = New MultiLayerNetwork(conf)
			net.init()
			assertEquals(0.1, CType(CType(conf.getConf(0).getLayer(), BaseLayer).getIUpdater(), Nesterovs).getMomentumISchedule().valueAt(0, 0), 0.0)
			assertEquals(0.2, CType(CType(conf.getConf(1).getLayer(), BaseLayer).getIUpdater(), Nesterovs).getMomentumISchedule().valueAt(0, 0), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Updater Rho Rms Decay Layerwise Override") void testUpdaterRhoRmsDecayLayerwiseOverride()
		Friend Overridable Sub testUpdaterRhoRmsDecayLayerwiseOverride()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New AdaDelta(0.5, 0.9)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).updater(New AdaDelta(0.01, 0.9)).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertTrue(TypeOf (CType(conf.getConf(0).getLayer(), BaseLayer)).getIUpdater() Is AdaDelta)
			assertTrue(TypeOf (CType(conf.getConf(1).getLayer(), BaseLayer)).getIUpdater() Is AdaDelta)
			assertEquals(0.5, CType(CType(conf.getConf(0).getLayer(), BaseLayer).getIUpdater(), AdaDelta).getRho(), 0.0)
			assertEquals(0.01, CType(CType(conf.getConf(1).getLayer(), BaseLayer).getIUpdater(), AdaDelta).getRho(), 0.0)
			conf = (New NeuralNetConfiguration.Builder()).updater(New RmsProp(1.0, 2.0, RmsProp.DEFAULT_RMSPROP_EPSILON)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).updater(New RmsProp(1.0, 1.0, RmsProp.DEFAULT_RMSPROP_EPSILON)).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).updater(New AdaDelta(0.5, AdaDelta.DEFAULT_ADADELTA_EPSILON)).build()).build()
			net = New MultiLayerNetwork(conf)
			net.init()
			assertTrue(TypeOf (CType(conf.getConf(0).getLayer(), BaseLayer)).getIUpdater() Is RmsProp)
			assertTrue(TypeOf (CType(conf.getConf(1).getLayer(), BaseLayer)).getIUpdater() Is AdaDelta)
			assertEquals(1.0, CType(CType(conf.getConf(0).getLayer(), BaseLayer).getIUpdater(), RmsProp).getRmsDecay(), 0.0)
			assertEquals(0.5, CType(CType(conf.getConf(1).getLayer(), BaseLayer).getIUpdater(), AdaDelta).getRho(), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Updater Adam Params Layerwise Override") void testUpdaterAdamParamsLayerwiseOverride()
		Friend Overridable Sub testUpdaterAdamParamsLayerwiseOverride()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1.0, 0.5, 0.5, 1e-8)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).updater(New Adam(1.0, 0.6, 0.7, 1e-8)).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(0.5, CType(CType(conf.getConf(0).getLayer(), BaseLayer).getIUpdater(), Adam).getBeta1(), 0.0)
			assertEquals(0.6, CType(CType(conf.getConf(1).getLayer(), BaseLayer).getIUpdater(), Adam).getBeta1(), 0.0)
			assertEquals(0.5, CType(CType(conf.getConf(0).getLayer(), BaseLayer).getIUpdater(), Adam).getBeta2(), 0.0)
			assertEquals(0.7, CType(CType(conf.getConf(1).getLayer(), BaseLayer).getIUpdater(), Adam).getBeta2(), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Gradient Normalization Layerwise Override") void testGradientNormalizationLayerwiseOverride()
		Friend Overridable Sub testGradientNormalizationLayerwiseOverride()
			' Learning rate without layerwise override:
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, CType(conf.getConf(0).getLayer(), BaseLayer).GradientNormalization)
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, CType(conf.getConf(1).getLayer(), BaseLayer).GradientNormalization)
			assertEquals(10, CType(conf.getConf(0).getLayer(), BaseLayer).GradientNormalizationThreshold, 0.0)
			assertEquals(10, CType(conf.getConf(1).getLayer(), BaseLayer).GradientNormalizationThreshold, 0.0)
			' With:
			conf = (New NeuralNetConfiguration.Builder()).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).gradientNormalization(GradientNormalization.None).gradientNormalizationThreshold(2.5).build()).build()
			net = New MultiLayerNetwork(conf)
			net.init()
			assertEquals(GradientNormalization.ClipElementWiseAbsoluteValue, CType(conf.getConf(0).getLayer(), BaseLayer).GradientNormalization)
			assertEquals(GradientNormalization.None, CType(conf.getConf(1).getLayer(), BaseLayer).GradientNormalization)
			assertEquals(10, CType(conf.getConf(0).getLayer(), BaseLayer).GradientNormalizationThreshold, 0.0)
			assertEquals(2.5, CType(conf.getConf(1).getLayer(), BaseLayer).GradientNormalizationThreshold, 0.0)
		End Sub
	'    
	'    @Test
	'    public void testLearningRatePolicyExponential() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().learningRate(lr)
	'                        .updater(Updater.SGD)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Exponential).lrPolicyDecayRate(lrDecayRate).list()
	'                        .layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Exponential, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Exponential, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'    }
	'
	'    @Test
	'    public void testLearningRatePolicyInverse() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        double power = 3;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().iterations(iterations).learningRate(lr)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Inverse).lrPolicyDecayRate(lrDecayRate)
	'                        .lrPolicyPower(power).list().layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Inverse, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Inverse, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(power, conf.getConf(0).getLrPolicyPower(), 0.0);
	'        assertEquals(power, conf.getConf(1).getLrPolicyPower(), 0.0);
	'    }
	'
	'
	'    @Test
	'    public void testLearningRatePolicySteps() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        double steps = 4;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().iterations(iterations).learningRate(lr)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Step).lrPolicyDecayRate(lrDecayRate)
	'                        .lrPolicySteps(steps).list().layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Step, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Step, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(steps, conf.getConf(0).getLrPolicySteps(), 0.0);
	'        assertEquals(steps, conf.getConf(1).getLrPolicySteps(), 0.0);
	'    }
	'
	'    @Test
	'    public void testLearningRatePolicyPoly() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        double power = 3;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().iterations(iterations).learningRate(lr)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Poly).lrPolicyDecayRate(lrDecayRate)
	'                        .lrPolicyPower(power).list().layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Poly, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Poly, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(power, conf.getConf(0).getLrPolicyPower(), 0.0);
	'        assertEquals(power, conf.getConf(1).getLrPolicyPower(), 0.0);
	'    }
	'
	'    @Test
	'    public void testLearningRatePolicySigmoid() {
	'        double lr = 2;
	'        double lrDecayRate = 5;
	'        double steps = 4;
	'        int iterations = 1;
	'        MultiLayerConfiguration conf = new NeuralNetConfiguration.Builder().iterations(iterations).learningRate(lr)
	'                        .learningRateDecayPolicy(LearningRatePolicy.Sigmoid).lrPolicyDecayRate(lrDecayRate)
	'                        .lrPolicySteps(steps).list().layer(0, new DenseLayer.Builder().nIn(2).nOut(2).build())
	'                        .layer(1, new DenseLayer.Builder().nIn(2).nOut(2).build()).build();
	'        MultiLayerNetwork net = new MultiLayerNetwork(conf);
	'        net.init();
	'
	'        assertEquals(LearningRatePolicy.Sigmoid, conf.getConf(0).getLearningRatePolicy());
	'        assertEquals(LearningRatePolicy.Sigmoid, conf.getConf(1).getLearningRatePolicy());
	'        assertEquals(lrDecayRate, conf.getConf(0).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(lrDecayRate, conf.getConf(1).getLrPolicyDecayRate(), 0.0);
	'        assertEquals(steps, conf.getConf(0).getLrPolicySteps(), 0.0);
	'        assertEquals(steps, conf.getConf(1).getLrPolicySteps(), 0.0);
	'    }
	'
	'
	End Class

End Namespace