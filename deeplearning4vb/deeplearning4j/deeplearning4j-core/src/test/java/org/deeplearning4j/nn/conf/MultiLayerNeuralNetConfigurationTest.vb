Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports DropConnect = org.deeplearning4j.nn.conf.weightnoise.DropConnect
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions
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
Namespace org.deeplearning4j.nn.conf

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Multi Layer Neural Net Configuration Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class MultiLayerNeuralNetConfigurationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class MultiLayerNeuralNetConfigurationTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Json") void testJson() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJson()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).dist(New NormalDistribution(1, 1e-1)).build()).inputPreProcessor(0, New CnnToFeedForwardPreProcessor()).build()
			Dim json As String = conf.toJson()
			Dim from As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
			assertEquals(conf.getConf(0), from.getConf(0))
			Dim props As New Properties()
			props.put("json", json)
			Dim key As String = props.getProperty("json")
			assertEquals(json, key)
			Dim f As File = testDir.resolve("props").toFile()
			f.deleteOnExit()
			Dim bos As New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write))
			props.store(bos, "")
			bos.flush()
			bos.close()
			Dim bis As New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
			Dim props2 As New Properties()
			props2.load(bis)
			bis.close()
			assertEquals(props2.getProperty("json"), props.getProperty("json"))
			Dim json2 As String = props2.getProperty("json")
			Dim conf3 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json2)
			assertEquals(conf.getConf(0), conf3.getConf(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Convnet Json") void testConvnetJson()
		Friend Overridable Sub testConvnetJson()
			Const numRows As Integer = 76
			Const numColumns As Integer = 76
			Dim nChannels As Integer = 3
			Dim outputNum As Integer = 6
			Dim seed As Integer = 123
			' setup the network
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).l1(1e-1).l2(2e-4).weightNoise(New DropConnect(0.5)).miniBatch(True).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nOut(5).dropOut(0.5).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).build()).layer(2, (New ConvolutionLayer.Builder(3, 3)).nOut(10).dropOut(0.5).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(3, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).build()).layer(4, (New DenseLayer.Builder()).nOut(100).activation(Activation.RELU).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(numRows, numColumns, nChannels))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim json As String = conf.toJson()
			Dim conf2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
			assertEquals(conf, conf2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Upsampling Convnet Json") void testUpsamplingConvnetJson()
		Friend Overridable Sub testUpsamplingConvnetJson()
			Const numRows As Integer = 76
			Const numColumns As Integer = 76
			Dim nChannels As Integer = 3
			Dim outputNum As Integer = 6
			Dim seed As Integer = 123
			' setup the network
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).l1(1e-1).l2(2e-4).dropOut(0.5).miniBatch(True).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).list().layer((New ConvolutionLayer.Builder(5, 5)).nOut(5).dropOut(0.5).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer((New Upsampling2D.Builder()).size(2).build()).layer(2, (New ConvolutionLayer.Builder(3, 3)).nOut(10).dropOut(0.5).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer((New Upsampling2D.Builder()).size(2).build()).layer(4, (New DenseLayer.Builder()).nOut(100).activation(Activation.RELU).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(numRows, numColumns, nChannels))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim json As String = conf.toJson()
			Dim conf2 As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
			assertEquals(conf, conf2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Global Pooling Json") void testGlobalPoolingJson()
		Friend Overridable Sub testGlobalPoolingJson()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dist(New NormalDistribution(0, 1.0)).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nOut(5).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(PoolingType.PNORM).pnorm(3).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(3).build()).setInputType(InputType.convolutional(32, 32, 1)).build()
			Dim str As String = conf.toJson()
			Dim fromJson As MultiLayerConfiguration = conf.fromJson(str)
			assertEquals(conf, fromJson)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Yaml") void testYaml() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testYaml()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).dist(New NormalDistribution(1, 1e-1)).build()).inputPreProcessor(0, New CnnToFeedForwardPreProcessor()).build()
			Dim json As String = conf.toYaml()
			Dim from As MultiLayerConfiguration = MultiLayerConfiguration.fromYaml(json)
			assertEquals(conf.getConf(0), from.getConf(0))
			Dim props As New Properties()
			props.put("json", json)
			Dim key As String = props.getProperty("json")
			assertEquals(json, key)
			Dim f As File = testDir.resolve("props").toFile()
			f.deleteOnExit()
			Dim bos As New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write))
			props.store(bos, "")
			bos.flush()
			bos.close()
			Dim bis As New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
			Dim props2 As New Properties()
			props2.load(bis)
			bis.close()
			assertEquals(props2.getProperty("json"), props.getProperty("json"))
			Dim yaml As String = props2.getProperty("json")
			Dim conf3 As MultiLayerConfiguration = MultiLayerConfiguration.fromYaml(yaml)
			assertEquals(conf.getConf(0), conf3.getConf(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Clone") void testClone()
		Friend Overridable Sub testClone()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).build()).inputPreProcessor(1, New CnnToFeedForwardPreProcessor()).build()
			Dim conf2 As MultiLayerConfiguration = conf.clone()
			assertEquals(conf, conf2)
			assertNotSame(conf, conf2)
			assertNotSame(conf.getConfs(), conf2.getConfs())
			Dim i As Integer = 0
			Do While i < conf.getConfs().size()
				assertNotSame(conf.getConf(i), conf2.getConf(i))
				i += 1
			Loop
			assertNotSame(conf.getInputPreProcessors(), conf2.getInputPreProcessors())
			For Each layer As Integer? In conf.getInputPreProcessors().keySet()
				assertNotSame(conf.getInputPreProcess(layer), conf2.getInputPreProcess(layer))
			Next layer
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Random Weight Init") void testRandomWeightInit()
		Friend Overridable Sub testRandomWeightInit()
			Dim model1 As New MultiLayerNetwork(Conf)
			model1.init()
			Nd4j.Random.Seed = 12345L
			Dim model2 As New MultiLayerNetwork(Conf)
			model2.init()
			Dim p1() As Single = model1.params().data().asFloat()
			Dim p2() As Single = model2.params().data().asFloat()
			Console.WriteLine(Arrays.toString(p1))
			Console.WriteLine(Arrays.toString(p2))
			assertArrayEquals(p1, p2, 0.0f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Training Listener") void testTrainingListener()
		Friend Overridable Sub testTrainingListener()
			Dim model1 As New MultiLayerNetwork(Conf)
			model1.init()
			model1.addListeners(New ScoreIterationListener(1))
			Dim model2 As New MultiLayerNetwork(Conf)
			model2.addListeners(New ScoreIterationListener(1))
			model2.init()
			Dim l1() As Layer = model1.Layers
			For i As Integer = 0 To l1.Length - 1
				assertTrue(l1(i).getListeners() IsNot Nothing AndAlso l1(i).getListeners().Count = 1)
			Next i
			Dim l2() As Layer = model2.Layers
			For i As Integer = 0 To l2.Length - 1
				assertTrue(l2(i).getListeners() IsNot Nothing AndAlso l2(i).getListeners().Count = 1)
			Next i
		End Sub

		Private Shared ReadOnly Property Conf As MultiLayerConfiguration
			Get
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345l).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).dist(New NormalDistribution(0, 1)).build()).layer(1, (New OutputLayer.Builder()).nIn(2).nOut(1).activation(Activation.TANH).dist(New NormalDistribution(0, 1)).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
				Return conf
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Config") void testInvalidConfig()
		Friend Overridable Sub testInvalidConfig()
			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK
				log.error("", e)
			Catch e As Exception
				log.error("", e)
				fail("Unexpected exception thrown for invalid config")
			End Try
			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(1, (New DenseLayer.Builder()).nIn(3).nOut(4).build()).layer(2, (New OutputLayer.Builder()).nIn(4).nOut(5).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK
				log.info(e.ToString())
			Catch e As Exception
				log.error("", e)
				fail("Unexpected exception thrown for invalid config")
			End Try
			Try
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(3).nOut(4).build()).layer(2, (New OutputLayer.Builder()).nIn(4).nOut(5).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK
				log.info(e.ToString())
			Catch e As Exception
				log.error("", e)
				fail("Unexpected exception thrown for invalid config")
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test List Overloads") void testListOverloads()
		Friend Overridable Sub testListOverloads()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(3).nOut(4).build()).layer(1, (New OutputLayer.Builder()).nIn(4).nOut(5).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim dl As DenseLayer = CType(conf.getConf(0).getLayer(), DenseLayer)
			assertEquals(3, dl.getNIn())
			assertEquals(4, dl.getNOut())
			Dim ol As OutputLayer = CType(conf.getConf(1).getLayer(), OutputLayer)
			assertEquals(4, ol.getNIn())
			assertEquals(5, ol.getNOut())
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(3).nOut(4).build()).layer(1, (New OutputLayer.Builder()).nIn(4).nOut(5).activation(Activation.SOFTMAX).build()).build()
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()
			Dim conf3 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list((New DenseLayer.Builder()).nIn(3).nOut(4).build(), (New OutputLayer.Builder()).nIn(4).nOut(5).activation(Activation.SOFTMAX).build()).build()
			Dim net3 As New MultiLayerNetwork(conf3)
			net3.init()
			assertEquals(conf, conf2)
			assertEquals(conf, conf3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Bias Lr") void testBiasLr()
		Friend Overridable Sub testBiasLr()
			' setup the network
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(1e-2)).biasUpdater(New Adam(0.5)).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nOut(5).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New DenseLayer.Builder()).nOut(100).activation(Activation.RELU).build()).layer(2, (New DenseLayer.Builder()).nOut(100).activation(Activation.RELU).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(10).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(28, 28, 1)).build()
			Dim l0 As BaseLayer = CType(conf.getConf(0).getLayer(), BaseLayer)
			Dim l1 As BaseLayer = CType(conf.getConf(1).getLayer(), BaseLayer)
			Dim l2 As BaseLayer = CType(conf.getConf(2).getLayer(), BaseLayer)
			Dim l3 As BaseLayer = CType(conf.getConf(3).getLayer(), BaseLayer)
			assertEquals(0.5, DirectCast(l0.getUpdaterByParam("b"), Adam).getLearningRate(), 1e-6)
			assertEquals(1e-2, DirectCast(l0.getUpdaterByParam("W"), Adam).getLearningRate(), 1e-6)
			assertEquals(0.5, DirectCast(l1.getUpdaterByParam("b"), Adam).getLearningRate(), 1e-6)
			assertEquals(1e-2, DirectCast(l1.getUpdaterByParam("W"), Adam).getLearningRate(), 1e-6)
			assertEquals(0.5, DirectCast(l2.getUpdaterByParam("b"), Adam).getLearningRate(), 1e-6)
			assertEquals(1e-2, DirectCast(l2.getUpdaterByParam("W"), Adam).getLearningRate(), 1e-6)
			assertEquals(0.5, DirectCast(l3.getUpdaterByParam("b"), Adam).getLearningRate(), 1e-6)
			assertEquals(1e-2, DirectCast(l3.getUpdaterByParam("W"), Adam).getLearningRate(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Output Layer") void testInvalidOutputLayer()
		Friend Overridable Sub testInvalidOutputLayer()
	'        
	'        Test case (invalid configs)
	'        1. nOut=1 + softmax
	'        2. mcxent + tanh
	'        3. xent + softmax
	'        4. xent + relu
	'        5. mcxent + sigmoid
	'         
			Dim lf() As LossFunctions.LossFunction = { LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.XENT, LossFunctions.LossFunction.XENT, LossFunctions.LossFunction.MCXENT }
			Dim nOut() As Integer = { 1, 3, 3, 3, 3 }
			Dim activations() As Activation = { Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.RELU, Activation.SIGMOID }
			For i As Integer = 0 To lf.Length - 1
				For Each lossLayer As Boolean In New Boolean() { False, True }
					For Each validate As Boolean In New Boolean() { True, False }
						Dim s As String = "nOut=" & nOut(i) & ",lossFn=" & lf(i) & ",lossLayer=" & lossLayer & ",validate=" & validate
						If nOut(i) = 1 AndAlso lossLayer Then
							' nOuts are not availabel in loss layer, can't expect it to detect this case
							Continue For
						End If
						Try
							Call (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(If(Not lossLayer, (New OutputLayer.Builder()).nIn(10).nOut(nOut(i)).activation(activations(i)).lossFunction(lf(i)).build(), (New LossLayer.Builder()).activation(activations(i)).lossFunction(lf(i)).build())).validateOutputLayerConfig(validate).build()
							If validate Then
								fail("Expected exception: " & s)
							End If
						Catch e As DL4JInvalidConfigException
							If validate Then
								assertTrue(e.Message.ToLower().Contains("invalid output"),s)
							Else
								fail("Validation should not be enabled")
							End If
						End Try
					Next validate
				Next lossLayer
			Next i
		End Sub
	End Class

End Namespace