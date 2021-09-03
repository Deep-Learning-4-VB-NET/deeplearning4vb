Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports GravesLSTM = org.deeplearning4j.nn.conf.layers.GravesLSTM
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.deeplearning4j.nn.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestVariableLengthTSCG extends org.deeplearning4j.BaseDL4JTest
	Public Class TestVariableLengthTSCG
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVariableLengthSimple()
		Public Overridable Sub testVariableLengthSimple()

			'Test: Simple RNN layer + RNNOutputLayer
			'Length of 4 for standard
			'Length of 5 with last time step output mask set to 0
			'Expect the same gradients etc in both cases...

			Dim miniBatchSizes() As Integer = {1, 2, 5}
			Dim nOut As Integer = 1
			Dim r As New Random(12345)

			For Each nExamples As Integer In miniBatchSizes
				Nd4j.Random.setSeed(12345)

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New GravesLSTM.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build(), "in").addLayer("1", (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(2).nOut(1).activation(Activation.TANH).build(), "0").setInputTypes(InputType.recurrent(2,5,RNNFormat.NCW)).setOutputs("1").build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim in1 As INDArray = Nd4j.rand(New Integer() {nExamples, 2, 4})
				Dim in2 As INDArray = Nd4j.rand(New Integer() {nExamples, 2, 5})
				in2.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 3, True)}, in1)

				assertEquals(in1, in2.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)))

				Dim labels1 As INDArray = Nd4j.rand(New Integer() {nExamples, 1, 4})
				Dim labels2 As INDArray = Nd4j.create(nExamples, 1, 5)
				labels2.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 3, True)}, labels1)
				assertEquals(labels1, labels2.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)))

				Dim labelMask As INDArray = Nd4j.ones(nExamples, 5)
				For j As Integer = 0 To nExamples - 1
					labelMask.putScalar(New Integer() {j, 4}, 0)
				Next j


				net.setInput(0, in1)
				net.setLabel(0, labels1)
				net.computeGradientAndScore()
				Dim score1 As Double = net.score()
				Dim g1 As Gradient = net.gradient()

				net.setInput(0, in2)
				net.setLabel(0, labels2)
				net.setLayerMaskArrays(Nothing, New INDArray() {labelMask})
				net.computeGradientAndScore()
				Dim score2 As Double = net.score()
				Dim g2 As Gradient = net.gradient()

				'Scores and gradients should be identical for two cases (given mask array)
				assertEquals(score1, score2, 1e-6)

				Dim g1map As IDictionary(Of String, INDArray) = g1.gradientForVariable()
				Dim g2map As IDictionary(Of String, INDArray) = g2.gradientForVariable()

				For Each s As String In g1map.Keys
					Dim g1s As INDArray = g1map(s)
					Dim g2s As INDArray = g2map(s)
					assertEquals(g1s, g2s, s)
				Next s

				'Finally: check that the values at the masked outputs don't actually make any difference to:
				' (a) score, (b) gradients
				For i As Integer = 0 To nExamples - 1
					For j As Integer = 0 To nOut - 1
						Dim d As Double = r.NextDouble()
						labels2.putScalar(New Integer() {i, j, 4}, d)
					Next j
					net.setLabel(0, labels2)
					net.computeGradientAndScore()
					Dim score2a As Double = net.score()
					Dim g2a As Gradient = net.gradient()
					assertEquals(score2, score2a, 1e-6)
					For Each s As String In g2map.Keys
						Dim g2s As INDArray = g2map(s)
						Dim g2sa As INDArray = g2a.getGradientFor(s)
						assertEquals(g2s, g2sa, s)
					Next s
				Next i
			Next nExamples
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInputMasking()
		Public Overridable Sub testInputMasking()
			'Idea: have masking on the input with 2 dense layers on input
			'Ensure that the parameter gradients for the inputs don't depend on the inputs when inputs are masked

			Dim miniBatchSizes() As Integer = {1, 2, 5}
			Dim nIn As Integer = 2
			Dim r As New Random(1234)

			For Each nExamples As Integer In miniBatchSizes
				Nd4j.Random.setSeed(12345)

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(New NormalDistribution(0,2)).updater(New Sgd(0.1)).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build(), "in").addLayer("1", (New DenseLayer.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build(), "0").addLayer("2", (New GravesLSTM.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build(), "1").addLayer("3", (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(2).nOut(1).activation(Activation.TANH).build(), "2").setOutputs("3").inputPreProcessor("0", New RnnToFeedForwardPreProcessor()).inputPreProcessor("2", New FeedForwardToRnnPreProcessor()).setInputTypes(InputType.recurrent(2,5, RNNFormat.NCW)).build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim in1 As INDArray = Nd4j.rand(New Integer() {nExamples, 2, 4})
				Dim in2 As INDArray = Nd4j.rand(New Integer() {nExamples, 2, 5})
				in2.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 3, True)}, in1)

				assertEquals(in1, in2.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)))

				Dim labels1 As INDArray = Nd4j.rand(New Integer() {nExamples, 1, 4})
				Dim labels2 As INDArray = Nd4j.create(nExamples, 1, 5)
				labels2.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 3, True)}, labels1)
				assertEquals(labels1, labels2.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)))

				Dim inputMask As INDArray = Nd4j.ones(nExamples, 5)
				For j As Integer = 0 To nExamples - 1
					inputMask.putScalar(New Integer() {j, 4}, 0)
				Next j


				net.setInput(0, in1)
				net.setLabel(0, labels1)
				net.computeGradientAndScore()
				Dim score1 As Double = net.score()
				Dim g1 As Gradient = net.gradient()
				Dim map As IDictionary(Of String, INDArray) = g1.gradientForVariable()
				For Each s As String In map.Keys
					map(s) = map(s).dup() 'Gradients are views; need to dup otherwise they will be modified by next computeGradientAndScore
				Next s

				net.setInput(0, in2)
				net.setLabel(0, labels2)
				net.setLayerMaskArrays(New INDArray() {inputMask}, Nothing)
				net.computeGradientAndScore()
				Dim score2 As Double = net.score()
				Dim g2 As Gradient = net.gradient()
				Dim activations2 As IDictionary(Of String, INDArray) = net.feedForward()

				'Scores should differ here: masking the input, not the output. Therefore 4 vs. 5 time step outputs
				assertNotEquals(score1, score2, 0.001)

				Dim g1map As IDictionary(Of String, INDArray) = g1.gradientForVariable()
				Dim g2map As IDictionary(Of String, INDArray) = g2.gradientForVariable()

				For Each s As String In g1map.Keys
					Dim g1s As INDArray = g1map(s)
					Dim g2s As INDArray = g2map(s)

					assertNotEquals(g1s, g2s, s)
				Next s

				'Modify the values at the masked time step, and check that neither the gradients, score or activations change
				For j As Integer = 0 To nExamples - 1
					For k As Integer = 0 To nIn - 1
						in2.putScalar(New Integer() {j, k, 4}, r.NextDouble())
					Next k
					net.setInput(0, in2)
					net.setLayerMaskArrays(New INDArray(){inputMask}, Nothing)
					net.computeGradientAndScore()
					Dim score2a As Double = net.score()
					Dim g2a As Gradient = net.gradient()
					assertEquals(score2, score2a, 1e-12)
					For Each s As String In g2.gradientForVariable().Keys
						assertEquals(g2.getGradientFor(s), g2a.getGradientFor(s))
					Next s

					Dim activations2a As IDictionary(Of String, INDArray) = net.feedForward()
					For Each s As String In activations2.Keys
						assertEquals(activations2(s), activations2a(s))
					Next s
				Next j

				'Finally: check that the activations for the first two (dense) layers are zero at the appropriate time step
				Dim temp As New FeedForwardToRnnPreProcessor()
				Dim l0Before As INDArray = activations2("0")
				Dim l1Before As INDArray = activations2("1")
				Dim l0After As INDArray = temp.preProcess(l0Before, nExamples, LayerWorkspaceMgr.noWorkspaces())
				Dim l1After As INDArray = temp.preProcess(l1Before, nExamples, LayerWorkspaceMgr.noWorkspaces())

				For j As Integer = 0 To nExamples - 1
					For k As Integer = 0 To nIn - 1
						assertEquals(0.0, l0After.getDouble(j, k, 4), 0.0)
						assertEquals(0.0, l1After.getDouble(j, k, 4), 0.0)
					Next k
				Next j
			Next nExamples
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutputMaskingScoreMagnitudes()
		Public Overridable Sub testOutputMaskingScoreMagnitudes()
			'Idea: check magnitude of scores, with differing number of values masked out
			'i.e., MSE with zero weight init and 1.0 labels: know what to expect in terms of score

			Dim nIn As Integer = 3
			Dim timeSeriesLengths() As Integer = {3, 10}
			Dim outputSizes() As Integer = {1, 2, 5}
			Dim miniBatchSizes() As Integer = {1, 4}

			Dim r As New Random(12345)

			For Each tsLength As Integer In timeSeriesLengths
				For Each nOut As Integer In outputSizes
					For Each miniBatch As Integer In miniBatchSizes
						Dim nToMask As Integer = 0
						Do While nToMask < tsLength - 1
							Dim msg As String = "tsLen=" & tsLength & ", nOut=" & nOut & ", miniBatch=" & miniBatch

							Dim labelMaskArray As INDArray = Nd4j.ones(miniBatch, tsLength)
							For i As Integer = 0 To miniBatch - 1
								'For each example: select which outputs to mask...
								Dim nMasked As Integer = 0
								Do While nMasked < nToMask
									Dim tryIdx As Integer = r.Next(tsLength)
									If labelMaskArray.getDouble(i, tryIdx) = 0.0 Then
										Continue Do
									End If
									labelMaskArray.putScalar(New Integer() {i, tryIdx}, 0.0)
									nMasked += 1
								Loop
							Next i

							Dim input As INDArray = Nd4j.rand(New Integer() {miniBatch, nIn, tsLength})
							Dim labels As INDArray = Nd4j.ones(miniBatch, nOut, tsLength)

							Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).graphBuilder().addInputs("in").addLayer("0", (New GravesLSTM.Builder()).nIn(nIn).nOut(5).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build(), "in").addLayer("1", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(5).nOut(nOut).weightInit(WeightInit.ZERO).updater(New NoOp()).build(), "0").setOutputs("1").setInputTypes(InputType.recurrent(nIn,tsLength,RNNFormat.NCW)).build()
							Dim net As New ComputationGraph(conf)
							net.init()

							'MSE loss function: 1/n * sum(squaredErrors)... but sum(squaredErrors) = n * (1-0) here -> sum(squaredErrors)
							Dim expScore As Double = tsLength - nToMask 'Sum over minibatches, then divide by minibatch size

							net.setLayerMaskArrays(Nothing, New INDArray() {labelMaskArray})
							net.setInput(0, input)
							net.setLabel(0, labels)

							net.computeGradientAndScore()
							Dim score As Double = net.score()
							assertEquals(expScore, score, 0.1,msg)
							nToMask += 1
						Loop
					Next miniBatch
				Next nOut
			Next tsLength
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutputMasking()
		Public Overridable Sub testOutputMasking()
			'If labels are masked: want zero outputs for that time step.

			Dim nIn As Integer = 3
			Dim timeSeriesLengths() As Integer = {3, 10}
			Dim outputSizes() As Integer = {1, 2, 5}
			Dim miniBatchSizes() As Integer = {1, 4}

			Dim r As New Random(12345)

			For Each tsLength As Integer In timeSeriesLengths
				For Each nOut As Integer In outputSizes
					For Each miniBatch As Integer In miniBatchSizes
						Dim nToMask As Integer = 0
						Do While nToMask < tsLength - 1
							Dim labelMaskArray As INDArray = Nd4j.ones(miniBatch, tsLength)
							For i As Integer = 0 To miniBatch - 1
								'For each example: select which outputs to mask...
								Dim nMasked As Integer = 0
								Do While nMasked < nToMask
									Dim tryIdx As Integer = r.Next(tsLength)
									If labelMaskArray.getDouble(i, tryIdx) = 0.0 Then
										Continue Do
									End If
									labelMaskArray.putScalar(New Integer() {i, tryIdx}, 0.0)
									nMasked += 1
								Loop
							Next i

							Dim input As INDArray = Nd4j.rand(New Integer() {miniBatch, nIn, tsLength})

							Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).graphBuilder().addInputs("in").addLayer("0", (New GravesLSTM.Builder()).nIn(nIn).nOut(5).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build(), "in").addLayer("1", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(5).nOut(nOut).weightInit(WeightInit.XAVIER).updater(New NoOp()).build(), "0").setOutputs("1").build()
							Dim net As New ComputationGraph(conf)
							net.init()

							Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).graphBuilder().addInputs("in").addLayer("0", (New GravesLSTM.Builder()).nIn(nIn).nOut(5).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build(), "in").addLayer("1", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.XENT)).activation(Activation.SIGMOID).nIn(5).nOut(nOut).weightInit(WeightInit.XAVIER).updater(New NoOp()).build(), "0").setOutputs("1").build()
							Dim net2 As New ComputationGraph(conf2)
							net2.init()

							net.setLayerMaskArrays(Nothing, New INDArray() {labelMaskArray})
							net2.setLayerMaskArrays(Nothing, New INDArray() {labelMaskArray})


							Dim [out] As INDArray = net.output(input)(0)
							Dim out2 As INDArray = net2.output(input)(0)
							For i As Integer = 0 To miniBatch - 1
								For j As Integer = 0 To tsLength - 1
									Dim m As Double = labelMaskArray.getDouble(i, j)
									If m = 0.0 Then
										'Expect outputs to be exactly 0.0
										Dim outRow As INDArray = [out].get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(j))
										Dim outRow2 As INDArray = out2.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(j))
										For k As Integer = 0 To nOut - 1
											assertEquals(0.0, outRow.getDouble(k), 0.0)
											assertEquals(0.0, outRow2.getDouble(k), 0.0)
										Next k
									End If
								Next j
							Next i
							nToMask += 1
						Loop
					Next miniBatch
				Next nOut
			Next tsLength
		End Sub

	End Class

End Namespace