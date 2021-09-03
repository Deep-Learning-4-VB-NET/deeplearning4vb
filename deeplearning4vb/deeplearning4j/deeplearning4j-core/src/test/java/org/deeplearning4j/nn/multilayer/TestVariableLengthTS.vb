Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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

Namespace org.deeplearning4j.nn.multilayer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestVariableLengthTS extends org.deeplearning4j.BaseDL4JTest
	Public Class TestVariableLengthTS
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

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).seed(12345).list().layer(0, (New GravesLSTM.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build()).layer(1, (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(2).nOut(1).activation(Activation.TANH).build()).build()

				Dim net As New MultiLayerNetwork(conf)
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


				net.Input = in1
				net.Labels = labels1
				net.computeGradientAndScore()
				Dim score1 As Double = net.score()
				Dim g1 As Gradient = net.gradient()

				net.Input = in2
				net.Labels = labels2
				net.setLayerMaskArrays(Nothing, labelMask)
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
					assertEquals(g1s, g2s,s)
				Next s

				'Finally: check that the values at the masked outputs don't actually make any differente to:
				' (a) score, (b) gradients
				For i As Integer = 0 To nExamples - 1
					For j As Integer = 0 To nOut - 1
						Dim d As Double = r.NextDouble()
						labels2.putScalar(New Integer() {i, j, 4}, d)
					Next j
					net.Labels = labels2
					net.computeGradientAndScore()
					Dim score2a As Double = net.score()
					Dim g2a As Gradient = net.gradient()
					assertEquals(score2, score2a, 1e-6)
					For Each s As String In g2map.Keys
						Dim g2s As INDArray = g2map(s)
						Dim g2sa As INDArray = g2a.getGradientFor(s)
						assertEquals(g2s, g2sa,s)
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
			Dim r As New Random(12345)

			For Each nExamples As Integer In miniBatchSizes
				Nd4j.Random.setSeed(1234)

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).seed(12345).list().layer(0, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build()).layer(2, (New LSTM.Builder()).activation(Activation.TANH).nIn(2).nOut(2).build()).layer(3, (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MEAN_ABSOLUTE_ERROR).nIn(2).nOut(1).activation(Activation.TANH).build()).inputPreProcessor(0, New RnnToFeedForwardPreProcessor()).inputPreProcessor(2, New FeedForwardToRnnPreProcessor()).setInputType(InputType.recurrent(2,-1, RNNFormat.NCW)).build()

				Dim net As New MultiLayerNetwork(conf)
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


				net.Input = in1
				net.Labels = labels1
				net.computeGradientAndScore()
				Dim score1 As Double = net.score()
				Dim g1 As Gradient = net.gradient()
				Dim map1 As IDictionary(Of String, INDArray) = g1.gradientForVariable()
				For Each s As String In map1.Keys
					map1(s) = map1(s).dup() 'Note: gradients are a view normally -> second computeGradientAndScore would have modified the original gradient map values...
				Next s

				net.Input = in2
				net.Labels = labels2
				net.setLayerMaskArrays(inputMask, Nothing)
				net.computeGradientAndScore()
				Dim score2 As Double = net.score()
				Dim g2 As Gradient = net.gradient()

				net.Input = in2
				net.Labels = labels2
				net.setLayerMaskArrays(inputMask, Nothing)
				Dim activations2 As IList(Of INDArray) = net.feedForward()

				'Scores should differ here: masking the input, not the output. Therefore 4 vs. 5 time step outputs
				assertNotEquals(score1, score2, 0.005)

				Dim g1map As IDictionary(Of String, INDArray) = g1.gradientForVariable()
				Dim g2map As IDictionary(Of String, INDArray) = g2.gradientForVariable()

				For Each s As String In g1map.Keys
					Dim g1s As INDArray = g1map(s)
					Dim g2s As INDArray = g2map(s)

	'                System.out.println("-------");
	'                System.out.println("Variable: " + s);
	'                System.out.println(Arrays.toString(g1s.dup().data().asFloat()));
	'                System.out.println(Arrays.toString(g2s.dup().data().asFloat()));
					assertNotEquals(g1s, g2s,s)
				Next s

				'Modify the values at the masked time step, and check that neither the gradients, score or activations change
				For j As Integer = 0 To nExamples - 1
					For k As Integer = 0 To nIn - 1
						in2.putScalar(New Integer() {j, k, 4}, r.NextDouble())
					Next k
					net.Input = in2
					net.setLayerMaskArrays(inputMask, Nothing)
					net.computeGradientAndScore()
					Dim score2a As Double = net.score()
					Dim g2a As Gradient = net.gradient()
					assertEquals(score2, score2a, 1e-12)
					For Each s As String In g2.gradientForVariable().Keys
						assertEquals(g2.getGradientFor(s), g2a.getGradientFor(s))
					Next s

					Dim activations2a As IList(Of INDArray) = net.feedForward()
					For k As Integer = 1 To activations2.Count - 1
						assertEquals(activations2(k), activations2a(k))
					Next k
				Next j

				'Finally: check that the activations for the first two (dense) layers are zero at the appropriate time step
				Dim temp As New FeedForwardToRnnPreProcessor()
				Dim l0Before As INDArray = activations2(1)
				Dim l1Before As INDArray = activations2(2)
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
			'Idea: check magnitude of scores, with differeing number of values masked out
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

							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(5).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).layer(1, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(5).nOut(nOut).weightInit(WeightInit.ZERO).updater(New NoOp()).build()).build()
							Dim mln As New MultiLayerNetwork(conf)
							mln.init()

							'MSE loss function: 1/n * sum(squaredErrors)... but sum(squaredErrors) = n * (1-0) here -> sum(squaredErrors)
							Dim expScore As Double = (tsLength - nToMask) 'Sum over minibatches, then divide by minibatch size

							mln.setLayerMaskArrays(Nothing, labelMaskArray)
							mln.Input = input
							mln.Labels = labels

							mln.computeGradientAndScore()
							Dim score As Double = mln.score()

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

							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(5).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).layer(1, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(5).nOut(nOut).weightInit(WeightInit.XAVIER).updater(New NoOp()).build()).build()
							Dim mln As New MultiLayerNetwork(conf)
							mln.init()

							Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).list().layer(0, (New GravesLSTM.Builder()).nIn(nIn).nOut(5).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).layer(1, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(5).nOut(nOut).weightInit(WeightInit.XAVIER).updater(New NoOp()).build()).build()
							Dim mln2 As New MultiLayerNetwork(conf2)
							mln2.init()

							mln.setLayerMaskArrays(Nothing, labelMaskArray)
							mln2.setLayerMaskArrays(Nothing, labelMaskArray)


							Dim [out] As INDArray = mln.output(input)
							Dim out2 As INDArray = mln2.output(input)
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingBidirectionalRnn()
		Public Overridable Sub testMaskingBidirectionalRnn()
			'Idea: mask some of the time steps, like [1,1,1,0,0]. We expect the activations for the first 3 time steps
			' to be the same as if we'd just fed in [1,1,1] for that example

			Nd4j.Random.setSeed(12345)

			Dim nIn As Integer = 4
			Dim layerSize As Integer = 3
			Dim nOut As Integer = 3

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).list().layer(0, (New GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(layerSize).build()).layer(1, (New GravesBidirectionalLSTM.Builder()).nIn(layerSize).nOut(layerSize).build()).layer(2, (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(layerSize).nOut(nOut).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()


			Dim tsLength As Integer = 5
			Dim minibatch As Integer = 3

			Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, nIn, tsLength})
			Dim labels As INDArray = Nd4j.rand(New Integer() {minibatch, nOut, tsLength})
			Dim featuresMask As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 1, 1, 1, 1},
				New Double() {1, 1, 1, 1, 0},
				New Double() {1, 1, 1, 0, 0}
			})

			Dim labelsMask As INDArray = featuresMask.dup()

			net.setLayerMaskArrays(featuresMask, labelsMask)
			Dim outMasked As INDArray = net.output(input)

			net.clearLayerMaskArrays()

			'Check forward pass:
			For i As Integer = 0 To minibatch - 1
				Dim idx() As INDArrayIndex = {NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.interval(0, tsLength - i)}
				Dim expExampleOut As INDArray = net.output(input.get(idx))
				Dim actualExampleOut As INDArray = outMasked.get(idx)
				'            System.out.println(i);
				assertEquals(expExampleOut, actualExampleOut)
			Next i

			'Also: check the score examples method...
			Dim ds As New DataSet(input, labels, featuresMask, labelsMask)
			Dim exampleScores As INDArray = net.scoreExamples(ds, False)
			assertArrayEquals(New Long() {minibatch, 1}, exampleScores.shape()) 'One score per time series (added over each time step)

			For i As Integer = 0 To minibatch - 1
				Dim idx() As INDArrayIndex = {NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.interval(0, tsLength - i)}
				Dim dsSingle As New DataSet(input.get(idx), labels.get(idx))

				Dim exampleSingleScore As INDArray = net.scoreExamples(dsSingle, False)
				Dim exp As Double = exampleSingleScore.getDouble(0)
				Dim act As Double = exampleScores.getDouble(i)

				'            System.out.println(i + "\t" + exp + "\t" + act);
				assertEquals(exp, act, 1e-6)
			Next i
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingLstmAndBidirectionalLstmGlobalPooling()
		Public Overridable Sub testMaskingLstmAndBidirectionalLstmGlobalPooling()
			'Idea: mask some of the time steps, like [1,1,1,0,0]. We expect the activations out of the global pooling
			' to be the same as if we'd just fed in the in the present (1s) time steps only

			Nd4j.Random.setSeed(12345)

			Dim nIn As Integer = 2
			Dim layerSize As Integer = 4
			Dim nOut As Integer = 3

			Dim poolingTypes() As PoolingType = {PoolingType.SUM, PoolingType.AVG, PoolingType.MAX}

			Dim isBidirectional() As Boolean = {False, True}

			For Each bidirectional As Boolean In isBidirectional
				For Each pt As PoolingType In poolingTypes

	'                System.out.println("Starting test: bidirectional = " + bidirectional + ", poolingType = " + pt);

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).list().layer(0,If(bidirectional, (New GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(layerSize).build(), (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).build())).layer(1,If(bidirectional, (New GravesBidirectionalLSTM.Builder()).nIn(layerSize).nOut(layerSize).build(), (New GravesLSTM.Builder()).nIn(layerSize).nOut(layerSize).build())).layer(2, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(3, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(layerSize).nOut(nOut).build()).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()


					Dim tsLength As Integer = 5
					Dim minibatch As Integer = 3

					Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, nIn, tsLength})
					Dim labels As INDArray = Nd4j.rand(New Integer() {minibatch, nOut})
					Dim featuresMask As INDArray = Nd4j.create(New Double()() {
						New Double() {1, 1, 1, 1, 1},
						New Double() {1, 1, 1, 1, 0},
						New Double() {1, 1, 1, 0, 0}
					})


					net.setLayerMaskArrays(featuresMask, Nothing)
					Dim outMasked As INDArray = net.output(input)
					net.clearLayerMaskArrays()

					For i As Integer = 0 To minibatch - 1
						Dim idx() As INDArrayIndex = {NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.interval(0, tsLength - i)}
						Dim inputSubset As INDArray = input.get(idx)
						Dim expExampleOut As INDArray = net.output(inputSubset)
						Dim actualExampleOut As INDArray = outMasked.getRow(i, True)
						'                    System.out.println(i);
						assertEquals(expExampleOut, actualExampleOut)
					Next i

					'Also: check the score examples method...
					Dim ds As New DataSet(input, labels, featuresMask, Nothing)
					Dim exampleScores As INDArray = net.scoreExamples(ds, False)
					For i As Integer = 0 To minibatch - 1
						Dim idx() As INDArrayIndex = {NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.interval(0, tsLength - i)}
						Dim dsSingle As New DataSet(input.get(idx), labels.getRow(i,True))

						Dim exampleSingleScore As INDArray = net.scoreExamples(dsSingle, False)
						Dim exp As Double = exampleSingleScore.getDouble(0)
						Dim act As Double = exampleScores.getDouble(i)

						'                    System.out.println(i + "\t" + exp + "\t" + act);
						assertEquals(exp, act, 1e-6)
					Next i
				Next pt
			Next bidirectional
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReverse()
		Public Overridable Sub testReverse()

			For Each c As Char In New Char(){"f"c, "c"c}

				Dim [in] As INDArray = Nd4j.linspace(1, 3 * 5 * 10, 3 * 5 * 10, Nd4j.dataType()).reshape("f"c, 3, 5, 10).dup(c)
				Dim inMask As INDArray = Nd4j.linspace(1, 30, 30, Nd4j.dataType()).reshape("f"c, 3, 10).dup(c) 'Minibatch, TS length

				Dim inReverseExp As INDArray = reverseTimeSeries([in])
				Dim inMaskReverseExp As INDArray = Nd4j.create(inMask.shape())
				Dim i As Integer = 0
				Do While i < inMask.size(1)
					inMaskReverseExp.putColumn(i, inMask.getColumn(inMask.size(1) - i - 1))
					i += 1
				Loop

				Dim inReverse As INDArray = TimeSeriesUtils.reverseTimeSeries([in], LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT)
				Dim inMaskReverse As INDArray = TimeSeriesUtils.reverseTimeSeriesMask(inMask, LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT)

				assertEquals(inReverseExp, inReverse)
				assertEquals(inMaskReverseExp, inMaskReverse)
			Next c
		End Sub



		''' <summary>
		''' CPU ONLY VERSION FOR TESTING
		''' </summary>
		Public Shared Function reverseTimeSeries(ByVal [in] As INDArray) As INDArray
			If [in] Is Nothing Then
				Return Nothing
			End If
			Dim [out] As INDArray = Nd4j.createUninitialized([in].shape(), "f"c)
			Dim op As CustomOp = DynamicCustomOp.builder("reverse").addIntegerArguments(2).addInputs([in]).addOutputs([out]).callInplace(False).build()
			Nd4j.Executioner.exec(op)
			Return [out]
		End Function
	End Class

End Namespace