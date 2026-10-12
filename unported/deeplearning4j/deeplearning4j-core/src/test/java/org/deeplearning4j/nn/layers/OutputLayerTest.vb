Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports org.deeplearning4j.nn.conf.layers
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports RnnOutputLayer = org.deeplearning4j.nn.layers.recurrent.RnnOutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
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
Namespace org.deeplearning4j.nn.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Output Layer Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class OutputLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class OutputLayerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Params") void testSetParams()
		Friend Overridable Sub testSetParams()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).updater(New Sgd(1e-1)).layer((New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.ZERO).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim l As OutputLayer = CType(conf.getLayer().instantiate(conf, Collections.singletonList(Of TrainingListener)(New ScoreIterationListener(1)), 0, params, True, params.dataType()), OutputLayer)
			params = l.params()
			l.Params = params
			assertEquals(params, l.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Output Layers Rnn Forward Pass") void testOutputLayersRnnForwardPass()
		Friend Overridable Sub testOutputLayersRnnForwardPass()
			' Test output layer with RNNs (
			' Expect all outputs etc. to be 2d
			Dim nIn As Integer = 2
			Dim nOut As Integer = 5
			Dim layerSize As Integer = 4
			Dim timeSeriesLength As Integer = 6
			Dim miniBatchSize As Integer = 3
			Dim r As New Random(12345L)
			Dim input As INDArray = Nd4j.zeros(miniBatchSize, nIn, timeSeriesLength)
			For i As Integer = 0 To miniBatchSize - 1
				For j As Integer = 0 To nIn - 1
					For k As Integer = 0 To timeSeriesLength - 1
						input.putScalar(New Integer() { i, j, k }, r.NextDouble() - 0.5)
					Next k
				Next j
			Next i
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).inputPreProcessor(1, New RnnToFeedForwardPreProcessor()).build()
			Dim mln As New MultiLayerNetwork(conf)
			mln.init()
			Dim out2d As INDArray = mln.feedForward(input)(2)
			assertArrayEquals(out2d.shape(), New Long() { miniBatchSize * timeSeriesLength, nOut })
			Dim [out] As INDArray = mln.output(input)
			assertArrayEquals([out].shape(), New Long() { miniBatchSize * timeSeriesLength, nOut })
			Dim preout As INDArray = mln.output(input)
			assertArrayEquals(preout.shape(), New Long() { miniBatchSize * timeSeriesLength, nOut })
			' As above, but for RnnOutputLayer. Expect all activations etc. to be 3d
			Dim confRnn As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).build()
			Dim mlnRnn As New MultiLayerNetwork(confRnn)
			mln.init()
			Dim out3d As INDArray = mlnRnn.feedForward(input)(2)
			assertArrayEquals(out3d.shape(), New Long() { miniBatchSize, nOut, timeSeriesLength })
			Dim outRnn As INDArray = mlnRnn.output(input)
			assertArrayEquals(outRnn.shape(), New Long() { miniBatchSize, nOut, timeSeriesLength })
			Dim preoutRnn As INDArray = mlnRnn.output(input)
			assertArrayEquals(preoutRnn.shape(), New Long() { miniBatchSize, nOut, timeSeriesLength })
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Rnn Output Layer Inc Edge Cases") void testRnnOutputLayerIncEdgeCases()
		Friend Overridable Sub testRnnOutputLayerIncEdgeCases()
			' Basic test + test edge cases: timeSeriesLength==1, miniBatchSize==1, both
			Dim tsLength() As Integer = { 5, 1, 5, 1 }
			Dim miniBatch() As Integer = { 7, 7, 1, 1 }
			Dim nIn As Integer = 3
			Dim nOut As Integer = 6
			Dim layerSize As Integer = 4
			Dim proc As New FeedForwardToRnnPreProcessor()
			For t As Integer = 0 To tsLength.Length - 1
				Nd4j.Random.setSeed(12345)
				Dim timeSeriesLength As Integer = tsLength(t)
				Dim miniBatchSize As Integer = miniBatch(t)
				Dim r As New Random(12345L)
				Dim input As INDArray = Nd4j.zeros(miniBatchSize, nIn, timeSeriesLength)
				For i As Integer = 0 To miniBatchSize - 1
					For j As Integer = 0 To nIn - 1
						For k As Integer = 0 To timeSeriesLength - 1
							input.putScalar(New Integer() { i, j, k }, r.NextDouble() - 0.5)
						Next k
					Next j
				Next i
				Dim labels3d As INDArray = Nd4j.zeros(miniBatchSize, nOut, timeSeriesLength)
				For i As Integer = 0 To miniBatchSize - 1
					For j As Integer = 0 To timeSeriesLength - 1
						Dim idx As Integer = r.Next(nOut)
						labels3d.putScalar(New Integer() { i, idx, j }, 1.0f)
					Next j
				Next i
				Dim labels2d As INDArray = proc.backprop(labels3d, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).inputPreProcessor(1, New RnnToFeedForwardPreProcessor()).build()
				Dim mln As New MultiLayerNetwork(conf)
				mln.init()
				Dim out2d As INDArray = mln.feedForward(input)(2)
				Dim out3d As INDArray = proc.preProcess(out2d, miniBatchSize, LayerWorkspaceMgr.noWorkspaces())
				Dim confRnn As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).updater(New NoOp()).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).build()
				Dim mlnRnn As New MultiLayerNetwork(confRnn)
				mlnRnn.init()
				Dim outRnn As INDArray = mlnRnn.feedForward(input)(2)
				mln.Labels = labels2d
				mlnRnn.Labels = labels3d
				mln.computeGradientAndScore()
				mlnRnn.computeGradientAndScore()
				' score is average over all examples.
				' However: OutputLayer version has miniBatch*timeSeriesLength "examples" (after reshaping)
				' RnnOutputLayer has miniBatch examples
				' Hence: expect difference in scores by factor of timeSeriesLength
				Dim score As Double = mln.score() * timeSeriesLength
				Dim scoreRNN As Double = mlnRnn.score()
				assertTrue(Not Double.IsNaN(score))
				assertTrue(Not Double.IsNaN(scoreRNN))
				Dim relError As Double = Math.Abs(score - scoreRNN) / (Math.Abs(score) + Math.Abs(scoreRNN))
				Console.WriteLine(relError)
				assertTrue(relError < 1e-6)
				' Check labels and inputs for output layer:
				Dim ol As OutputLayer = DirectCast(mln.OutputLayer, OutputLayer)
				assertArrayEquals(ol.Input.shape(), New Long() { miniBatchSize * timeSeriesLength, layerSize })
				assertArrayEquals(ol.Labels.shape(), New Long() { miniBatchSize * timeSeriesLength, nOut })
				Dim rnnol As RnnOutputLayer = DirectCast(mlnRnn.OutputLayer, RnnOutputLayer)
				' assertArrayEquals(rnnol.getInput().shape(),new int[]{miniBatchSize,layerSize,timeSeriesLength});
				' Input may be set by BaseLayer methods. Thus input may end up as reshaped 2d version instead of original 3d version.
				' Not ideal, but everything else works.
				assertArrayEquals(rnnol.Labels.shape(), New Long() { miniBatchSize, nOut, timeSeriesLength })
				' Check shapes of output for both:
				assertArrayEquals(out2d.shape(), New Long() { miniBatchSize * timeSeriesLength, nOut })
				Dim [out] As INDArray = mln.output(input)
				assertArrayEquals([out].shape(), New Long() { miniBatchSize * timeSeriesLength, nOut })
				Dim preout As INDArray = mln.output(input)
				assertArrayEquals(preout.shape(), New Long() { miniBatchSize * timeSeriesLength, nOut })
				Dim outFFRnn As INDArray = mlnRnn.feedForward(input)(2)
				assertArrayEquals(outFFRnn.shape(), New Long() { miniBatchSize, nOut, timeSeriesLength })
				Dim outRnn2 As INDArray = mlnRnn.output(input)
				assertArrayEquals(outRnn2.shape(), New Long() { miniBatchSize, nOut, timeSeriesLength })
				Dim preoutRnn As INDArray = mlnRnn.output(input)
				assertArrayEquals(preoutRnn.shape(), New Long() { miniBatchSize, nOut, timeSeriesLength })
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Compare Rnn Output Rnn Loss") void testCompareRnnOutputRnnLoss()
		Friend Overridable Sub testCompareRnnOutputRnnLoss()
			Nd4j.Random.setSeed(12345)
			Dim timeSeriesLength As Integer = 4
			Dim nIn As Integer = 5
			Dim layerSize As Integer = 6
			Dim nOut As Integer = 6
			Dim miniBatchSize As Integer = 3
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).updater(New NoOp()).list().layer((New LSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()).layer((New DenseLayer.Builder()).nIn(layerSize).nOut(nOut).activation(Activation.IDENTITY).build()).layer((New RnnLossLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).build()).build()
			Dim mln As New MultiLayerNetwork(conf1)
			mln.init()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).updater(New NoOp()).list().layer((New LSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()).layer((New org.deeplearning4j.nn.conf.layers.RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).build()).build()
			Dim mln2 As New MultiLayerNetwork(conf2)
			mln2.init()
			mln2.Params = mln.params()
			Dim [in] As INDArray = Nd4j.rand(New Integer() { miniBatchSize, nIn, timeSeriesLength })
			Dim out1 As INDArray = mln.output([in])
			Dim out2 As INDArray = mln.output([in])
			assertEquals(out1, out2)
			Dim r As New Random(12345)
			Dim labels As INDArray = Nd4j.create(miniBatchSize, nOut, timeSeriesLength)
			For i As Integer = 0 To miniBatchSize - 1
				For j As Integer = 0 To timeSeriesLength - 1
					labels.putScalar(i, r.Next(nOut), j, 1.0)
				Next j
			Next i
			mln.Input = [in]
			mln.Labels = labels
			mln2.Input = [in]
			mln2.Labels = labels
			mln.computeGradientAndScore()
			mln2.computeGradientAndScore()
			assertEquals(mln.gradient().gradient(), mln2.gradient().gradient())
			assertEquals(mln.score(), mln2.score(), 1e-6)
			TestUtils.testModelSerialization(mln)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn Loss Layer") void testCnnLossLayer()
		Friend Overridable Sub testCnnLossLayer()
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("*** Testing workspace: " & ws)
				For Each a As Activation In New Activation() { Activation.TANH, Activation.SELU }
					' Check that (A+identity) is equal to (identity+A), for activation A
					' i.e., should get same output and weight gradients for both
					Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).updater(New NoOp()).convolutionMode(ConvolutionMode.Same).inferenceWorkspaceMode(ws).trainingWorkspaceMode(ws).list().layer((New ConvolutionLayer.Builder()).nIn(3).nOut(4).activation(Activation.IDENTITY).kernelSize(2, 2).stride(1, 1).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()).layer((New CnnLossLayer.Builder(LossFunctions.LossFunction.MSE)).activation(a).build()).build()
					Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).updater(New NoOp()).convolutionMode(ConvolutionMode.Same).inferenceWorkspaceMode(ws).trainingWorkspaceMode(ws).list().layer((New ConvolutionLayer.Builder()).nIn(3).nOut(4).activation(a).kernelSize(2, 2).stride(1, 1).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()).layer((New CnnLossLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).build()).build()
					Dim mln As New MultiLayerNetwork(conf1)
					mln.init()
					Dim mln2 As New MultiLayerNetwork(conf2)
					mln2.init()
					mln2.Params = mln.params()
					Dim [in] As INDArray = Nd4j.rand(New Integer() { 3, 3, 5, 5 })
					Dim out1 As INDArray = mln.output([in])
					Dim out2 As INDArray = mln2.output([in])
					assertEquals(out1, out2)
					Dim labels As INDArray = Nd4j.rand(out1.shape())
					mln.Input = [in]
					mln.Labels = labels
					mln2.Input = [in]
					mln2.Labels = labels
					mln.computeGradientAndScore()
					mln2.computeGradientAndScore()
					assertEquals(mln.score(), mln2.score(), 1e-6)
					assertEquals(mln.gradient().gradient(), mln2.gradient().gradient())
					' Also check computeScoreForExamples
					Dim in2a As INDArray = Nd4j.rand(New Integer() { 1, 3, 5, 5 })
					Dim labels2a As INDArray = Nd4j.rand(New Integer() { 1, 4, 5, 5 })
					Dim in2 As INDArray = Nd4j.concat(0, in2a, in2a)
					Dim labels2 As INDArray = Nd4j.concat(0, labels2a, labels2a)
					Dim s As INDArray = mln.scoreExamples(New DataSet(in2, labels2), False)
					assertArrayEquals(New Long() { 2, 1 }, s.shape())
					assertEquals(s.getDouble(0), s.getDouble(1), 1e-6)
					TestUtils.testModelSerialization(mln)
				Next a
			Next ws
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn Loss Layer Comp Graph") void testCnnLossLayerCompGraph()
		Friend Overridable Sub testCnnLossLayerCompGraph()
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				log.info("*** Testing workspace: " & ws)
				For Each a As Activation In New Activation() { Activation.TANH, Activation.SELU }
					' Check that (A+identity) is equal to (identity+A), for activation A
					' i.e., should get same output and weight gradients for both
					Dim conf1 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).updater(New NoOp()).convolutionMode(ConvolutionMode.Same).inferenceWorkspaceMode(ws).trainingWorkspaceMode(ws).graphBuilder().addInputs("in").addLayer("0", (New ConvolutionLayer.Builder()).nIn(3).nOut(4).activation(Activation.IDENTITY).kernelSize(2, 2).stride(1, 1).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build(), "in").addLayer("1", (New CnnLossLayer.Builder(LossFunctions.LossFunction.MSE)).activation(a).build(), "0").setOutputs("1").build()
					Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).updater(New NoOp()).convolutionMode(ConvolutionMode.Same).inferenceWorkspaceMode(ws).trainingWorkspaceMode(ws).graphBuilder().addInputs("in").addLayer("0", (New ConvolutionLayer.Builder()).nIn(3).nOut(4).activation(a).kernelSize(2, 2).stride(1, 1).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build(), "in").addLayer("1", (New CnnLossLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).build(), "0").setOutputs("1").build()
					Dim graph As New ComputationGraph(conf1)
					graph.init()
					Dim graph2 As New ComputationGraph(conf2)
					graph2.init()
					graph2.Params = graph.params()
					Dim [in] As INDArray = Nd4j.rand(New Integer() { 3, 3, 5, 5 })
					Dim out1 As INDArray = graph.outputSingle([in])
					Dim out2 As INDArray = graph2.outputSingle([in])
					assertEquals(out1, out2)
					Dim labels As INDArray = Nd4j.rand(out1.shape())
					graph.setInput(0, [in])
					graph.Labels = labels
					graph2.setInput(0, [in])
					graph2.Labels = labels
					graph.computeGradientAndScore()
					graph2.computeGradientAndScore()
					assertEquals(graph.score(), graph2.score(), 1e-6)
					assertEquals(graph.gradient().gradient(), graph2.gradient().gradient())
					' Also check computeScoreForExamples
					Dim in2a As INDArray = Nd4j.rand(New Integer() { 1, 3, 5, 5 })
					Dim labels2a As INDArray = Nd4j.rand(New Integer() { 1, 4, 5, 5 })
					Dim in2 As INDArray = Nd4j.concat(0, in2a, in2a)
					Dim labels2 As INDArray = Nd4j.concat(0, labels2a, labels2a)
					Dim s As INDArray = graph.scoreExamples(New DataSet(in2, labels2), False)
					assertArrayEquals(New Long() { 2, 1 }, s.shape())
					assertEquals(s.getDouble(0), s.getDouble(1), 1e-6)
					TestUtils.testModelSerialization(graph)
				Next a
			Next ws
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cnn Output Layer Softmax") void testCnnOutputLayerSoftmax()
		Friend Overridable Sub testCnnOutputLayerSoftmax()
			' Check that softmax is applied channels-wise
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).updater(New NoOp()).convolutionMode(ConvolutionMode.Same).list().layer((New ConvolutionLayer.Builder()).nIn(3).nOut(4).activation(Activation.IDENTITY).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()).layer((New CnnLossLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim [in] As INDArray = Nd4j.rand(New Integer() { 2, 3, 4, 5 })
			Dim [out] As INDArray = net.output([in])
			Dim min As Double = [out].minNumber().doubleValue()
			Dim max As Double = [out].maxNumber().doubleValue()
			assertTrue(min >= 0 AndAlso max <= 1.0)
			Dim sum As INDArray = [out].sum(1)
			assertEquals(Nd4j.ones(DataType.FLOAT, 2, 4, 5), sum)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Output Layer Defaults") void testOutputLayerDefaults()
		Friend Overridable Sub testOutputLayerDefaults()
			Call (New NeuralNetConfiguration.Builder()).list().layer((New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder()).nIn(10).nOut(10).build()).build()
			Call (New NeuralNetConfiguration.Builder()).list().layer((New org.deeplearning4j.nn.conf.layers.LossLayer.Builder()).build()).build()
			Call (New NeuralNetConfiguration.Builder()).list().layer((New CnnLossLayer.Builder()).build()).build()
			Call (New NeuralNetConfiguration.Builder()).list().layer((New CenterLossOutputLayer.Builder()).build()).build()
		End Sub
	End Class

End Namespace