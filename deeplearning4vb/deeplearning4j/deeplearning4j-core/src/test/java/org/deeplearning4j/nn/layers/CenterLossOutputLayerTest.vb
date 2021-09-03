Imports System
Imports System.Threading
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports CenterLossOutputLayer = org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
import static org.junit.jupiter.api.Assertions.assertNotEquals
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
Namespace org.deeplearning4j.nn.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Center Loss Output Layer Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class CenterLossOutputLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CenterLossOutputLayerTest
		Inherits BaseDL4JTest

		Private Function getGraph(ByVal numLabels As Integer, ByVal lambda As Double) As ComputationGraph
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input1").addLayer("l1", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.RELU).build(), "input1").addLayer("lossLayer", (New CenterLossOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(5).nOut(numLabels).lambda(lambda).activation(Activation.SOFTMAX).build(), "l1").setOutputs("lossLayer").build()
			Dim graph As New ComputationGraph(conf)
			graph.init()
			Return graph
		End Function

		Public Overridable ReadOnly Property CNNMnistConfig As ComputationGraph
			Get
				' Number of input channels
				Dim nChannels As Integer = 1
				' The number of possible outcomes
				Dim outputNum As Integer = 10
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l2(0.0005).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).graphBuilder().addInputs("input").setInputTypes(InputType.convolutionalFlat(28, 28, 1)).addLayer("0", (New ConvolutionLayer.Builder(5, 5)).nIn(nChannels).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build(), "input").addLayer("1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build(), "0").addLayer("2", (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build(), "1").addLayer("3", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build(), "2").addLayer("4", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(500).build(), "3").addLayer("output", (New CenterLossOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nOut(outputNum).activation(Activation.SOFTMAX).build(), "4").setOutputs("output").build()
				Dim graph As New ComputationGraph(conf)
				graph.init()
				Return graph
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Lambda Conf") void testLambdaConf()
		Friend Overridable Sub testLambdaConf()
			Dim lambdas() As Double = { 0.1, 0.01 }
			Dim results(1) As Double
			Dim numClasses As Integer = 2
			Dim input As INDArray = Nd4j.rand(150, 4)
			Dim labels As INDArray = Nd4j.zeros(150, numClasses)
			Dim r As New Random(12345)
			For i As Integer = 0 To 149
				labels.putScalar(i, r.Next(numClasses), 1.0)
			Next i
			Dim graph As ComputationGraph
			For i As Integer = 0 To lambdas.Length - 1
				graph = getGraph(numClasses, lambdas(i))
				graph.setInput(0, input)
				graph.setLabel(0, labels)
				graph.computeGradientAndScore()
				results(i) = graph.score()
			Next i
			assertNotEquals(results(0), results(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @DisplayName("Test MNIST Config") void testMNISTConfig() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMNISTConfig()
			' Test batch size
			Dim batchSize As Integer = 64
			Dim mnistTrain As DataSetIterator = New MnistDataSetIterator(batchSize, True, 12345)
			Dim net As ComputationGraph = CNNMnistConfig
			net.init()
			net.setListeners(New ScoreIterationListener(1))
			For i As Integer = 0 To 49
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				net.fit(mnistTrain.next())
				Thread.Sleep(1000)
			Next i
			Thread.Sleep(100000)
		End Sub
	End Class

End Namespace