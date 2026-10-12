Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports GlobalPoolingLayer = org.deeplearning4j.nn.conf.layers.GlobalPoolingLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports FrozenLayer = org.deeplearning4j.nn.conf.layers.misc.FrozenLayer
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports org.deeplearning4j.nn.layers.recurrent
Imports GravesLSTM = org.deeplearning4j.nn.layers.recurrent.GravesLSTM
Imports LSTM = org.deeplearning4j.nn.layers.recurrent.LSTM
Imports SimpleRnn = org.deeplearning4j.nn.layers.recurrent.SimpleRnn
Imports GravesLSTMParamInitializer = org.deeplearning4j.nn.params.GravesLSTMParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports org.nd4j.common.primitives
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
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.DL4J_OLD_API) public class MultiLayerTestRNN extends org.deeplearning4j.BaseDL4JTest
	Public Class MultiLayerTestRNN
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGravesLSTMInit()
		Public Overridable Sub testGravesLSTMInit()
			Dim nIn As Integer = 8
			Dim nOut As Integer = 25
			Dim nHiddenUnits As Integer = 17
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(nHiddenUnits).activation(Activation.TANH).build()).layer(1, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(nHiddenUnits).nOut(nOut).activation(Activation.TANH).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()

			'Ensure that we have the correct number weights and biases, that these have correct shape etc.
			Dim layer As Layer = network.getLayer(0)
			assertTrue(TypeOf layer Is GravesLSTM)

			Dim paramTable As IDictionary(Of String, INDArray) = layer.paramTable()
			assertTrue(paramTable.Count = 3) '2 sets of weights, 1 set of biases

			Dim recurrentWeights As INDArray = paramTable(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY)
			assertArrayEquals(recurrentWeights.shape(), New Long() {nHiddenUnits, 4 * nHiddenUnits + 3}) 'Should be shape: [layerSize,4*layerSize+3]
			Dim inputWeights As INDArray = paramTable(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY)
			assertArrayEquals(inputWeights.shape(), New Long() {nIn, 4 * nHiddenUnits}) 'Should be shape: [nIn,4*layerSize]
			Dim biases As INDArray = paramTable(GravesLSTMParamInitializer.BIAS_KEY)
			assertArrayEquals(biases.shape(), New Long() {1, 4 * nHiddenUnits}) 'Should be shape: [1,4*layerSize]

			'Want forget gate biases to be initialized to > 0. See parameter initializer for details
			Dim forgetGateBiases As INDArray = biases.get(NDArrayIndex.point(0), NDArrayIndex.interval(nHiddenUnits, 2 * nHiddenUnits))
			Dim gt As INDArray = forgetGateBiases.gt(0)
			Dim gtSum As INDArray = gt.castTo(DataType.INT).sum(Integer.MaxValue)
			Dim count As Integer = gtSum.getInt(0)
			assertEquals(nHiddenUnits, count)

			Dim nParams As val = recurrentWeights.length() + inputWeights.length() + biases.length()
			assertTrue(nParams = layer.numParams())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGravesTLSTMInitStacked()
		Public Overridable Sub testGravesTLSTMInitStacked()
			Dim nIn As Integer = 8
			Dim nOut As Integer = 25
			Dim nHiddenUnits() As Integer = {17, 19, 23}
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(17).activation(Activation.TANH).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(17).nOut(19).activation(Activation.TANH).build()).layer(2, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(19).nOut(23).activation(Activation.TANH).build()).layer(3, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(23).nOut(nOut).activation(Activation.TANH).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()

			'Ensure that we have the correct number weights and biases, that these have correct shape etc. for each layer
			For i As Integer = 0 To nHiddenUnits.Length - 1
				Dim layer As Layer = network.getLayer(i)
				assertTrue(TypeOf layer Is GravesLSTM)

				Dim paramTable As IDictionary(Of String, INDArray) = layer.paramTable()
				assertTrue(paramTable.Count = 3) '2 sets of weights, 1 set of biases

				Dim layerNIn As Integer = (If(i = 0, nIn, nHiddenUnits(i - 1)))

				Dim recurrentWeights As INDArray = paramTable(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY)
				assertArrayEquals(recurrentWeights.shape(), New Long() {nHiddenUnits(i), 4 * nHiddenUnits(i) + 3}) 'Should be shape: [layerSize,4*layerSize+3]
				Dim inputWeights As INDArray = paramTable(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY)
				assertArrayEquals(inputWeights.shape(), New Long() {layerNIn, 4 * nHiddenUnits(i)}) 'Should be shape: [nIn,4*layerSize]
				Dim biases As INDArray = paramTable(GravesLSTMParamInitializer.BIAS_KEY)
				assertArrayEquals(biases.shape(), New Long() {1, 4 * nHiddenUnits(i)}) 'Should be shape: [1,4*layerSize]

				'Want forget gate biases to be initialized to > 0. See parameter initializer for details
				Dim forgetGateBiases As INDArray = biases.get(NDArrayIndex.point(0), NDArrayIndex.interval(nHiddenUnits(i), 2 * nHiddenUnits(i)))
				Dim gt As INDArray = forgetGateBiases.gt(0).castTo(DataType.INT)
				Dim gtSum As INDArray = gt.sum(Integer.MaxValue)
				Dim count As Double = gtSum.getDouble(0)
				assertEquals(nHiddenUnits(i), CInt(Math.Truncate(count)))

				Dim nParams As val = recurrentWeights.length() + inputWeights.length() + biases.length()
				assertTrue(nParams = layer.numParams())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnStateMethods()
		Public Overridable Sub testRnnStateMethods()
			Nd4j.Random.setSeed(12345)
			Dim timeSeriesLength As Integer = 6

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(5).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(4).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build()).build()
			Dim mln As New MultiLayerNetwork(conf)

			Dim input As INDArray = Nd4j.rand(New Integer() {3, 5, timeSeriesLength})

			Dim allOutputActivations As IList(Of INDArray) = mln.feedForward(input, True)
			Dim outAct As INDArray = allOutputActivations(3)

			Dim outRnnTimeStep As INDArray = mln.rnnTimeStep(input)

			assertTrue(outAct.Equals(outRnnTimeStep)) 'Should be identical here

			Dim currStateL0 As IDictionary(Of String, INDArray) = mln.rnnGetPreviousState(0)
			Dim currStateL1 As IDictionary(Of String, INDArray) = mln.rnnGetPreviousState(1)

			assertTrue(currStateL0.Count = 2)
			assertTrue(currStateL1.Count = 2)

			Dim lastActL0 As INDArray = currStateL0(GravesLSTM.STATE_KEY_PREV_ACTIVATION)
			Dim lastMemL0 As INDArray = currStateL0(GravesLSTM.STATE_KEY_PREV_MEMCELL)
			assertTrue(lastActL0 IsNot Nothing AndAlso lastMemL0 IsNot Nothing)

			Dim lastActL1 As INDArray = currStateL1(GravesLSTM.STATE_KEY_PREV_ACTIVATION)
			Dim lastMemL1 As INDArray = currStateL1(GravesLSTM.STATE_KEY_PREV_MEMCELL)
			assertTrue(lastActL1 IsNot Nothing AndAlso lastMemL1 IsNot Nothing)

			Dim expectedLastActL0 As INDArray = allOutputActivations(1).tensorAlongDimension(timeSeriesLength - 1, 1, 0)
			assertTrue(expectedLastActL0.Equals(lastActL0))

			Dim expectedLastActL1 As INDArray = allOutputActivations(2).tensorAlongDimension(timeSeriesLength - 1, 1, 0)
			assertTrue(expectedLastActL1.Equals(lastActL1))

			'Check clearing and setting of state:
			mln.rnnClearPreviousState()
			assertTrue(mln.rnnGetPreviousState(0).Count = 0)
			assertTrue(mln.rnnGetPreviousState(1).Count = 0)

			mln.rnnSetPreviousState(0, currStateL0)
			assertTrue(mln.rnnGetPreviousState(0).Count = 2)
			mln.rnnSetPreviousState(1, currStateL1)
			assertTrue(mln.rnnGetPreviousState(1).Count = 2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnTimeStepLayers()
		Public Overridable Sub testRnnTimeStepLayers()

			For layerType As Integer = 0 To 2
				Dim l0 As org.deeplearning4j.nn.conf.layers.Layer
				Dim l1 As org.deeplearning4j.nn.conf.layers.Layer
				Dim lastActKey As String

				If layerType = 0 Then
					l0 = (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(5).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()
					l1 = (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()
					lastActKey = GravesLSTM.STATE_KEY_PREV_ACTIVATION
				ElseIf layerType = 1 Then
					l0 = (New org.deeplearning4j.nn.conf.layers.LSTM.Builder()).nIn(5).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()
					l1 = (New org.deeplearning4j.nn.conf.layers.LSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()
					lastActKey = LSTM.STATE_KEY_PREV_ACTIVATION
				Else
					l0 = (New org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn.Builder()).nIn(5).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()
					l1 = (New org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()
					lastActKey = SimpleRnn.STATE_KEY_PREV_ACTIVATION
				End If

				log.info("Starting test for layer type: {}", l0.GetType().Name)


				Nd4j.Random.setSeed(12345)
				Dim timeSeriesLength As Integer = 12

				'4 layer network: 2 GravesLSTM + DenseLayer + RnnOutputLayer. Hence also tests preprocessors.
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, l0).layer(1, l1).layer(2, (New DenseLayer.Builder()).nIn(8).nOut(9).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(3, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(9).nOut(4).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build()).inputPreProcessor(2, New RnnToFeedForwardPreProcessor()).inputPreProcessor(3, New FeedForwardToRnnPreProcessor()).build()
				Dim mln As New MultiLayerNetwork(conf)

				Dim input As INDArray = Nd4j.rand(New Integer(){3, 5, timeSeriesLength})

				Dim allOutputActivations As IList(Of INDArray) = mln.feedForward(input, True)
				Dim fullOutL0 As INDArray = allOutputActivations(1)
				Dim fullOutL1 As INDArray = allOutputActivations(2)
				Dim fullOutL3 As INDArray = allOutputActivations(4)

				Dim inputLengths() As Integer = {1, 2, 3, 4, 6, 12}

				'Do steps of length 1, then of length 2, ..., 12
				'Should get the same result regardless of step size; should be identical to standard forward pass
				For i As Integer = 0 To inputLengths.Length - 1
					Dim inLength As Integer = inputLengths(i)
					Dim nSteps As Integer = timeSeriesLength \ inLength 'each of length inLength

					mln.rnnClearPreviousState()
					mln.InputMiniBatchSize = 1 'Reset; should be set by rnnTimeStep method

					For j As Integer = 0 To nSteps - 1
						Dim startTimeRange As Integer = j * inLength
						Dim endTimeRange As Integer = startTimeRange + inLength

						Dim inputSubset As INDArray
						If inLength = 1 Then 'Workaround to nd4j bug
							Dim sizes As val = New Long(){input.size(0), input.size(1), 1}
							inputSubset = Nd4j.create(sizes)
							inputSubset.tensorAlongDimension(0, 1, 0).assign(input.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(startTimeRange)))
						Else
							inputSubset = input.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeRange, endTimeRange))
						End If
						If inLength > 1 Then
							assertTrue(inputSubset.size(2) = inLength)
						End If

						Dim [out] As INDArray = mln.rnnTimeStep(inputSubset)

						Dim expOutSubset As INDArray
						If inLength = 1 Then
							Dim sizes As val = New Long(){fullOutL3.size(0), fullOutL3.size(1), 1}
							expOutSubset = Nd4j.create(DataType.FLOAT, sizes)
							expOutSubset.tensorAlongDimension(0, 1, 0).assign(fullOutL3.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(startTimeRange)))
						Else
							expOutSubset = fullOutL3.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeRange, endTimeRange))
						End If

						assertEquals(expOutSubset, [out])

						Dim currL0State As IDictionary(Of String, INDArray) = mln.rnnGetPreviousState(0)
						Dim currL1State As IDictionary(Of String, INDArray) = mln.rnnGetPreviousState(1)

						Dim lastActL0 As INDArray = currL0State(lastActKey)
						Dim lastActL1 As INDArray = currL1State(lastActKey)

						Dim expLastActL0 As INDArray = fullOutL0.tensorAlongDimension(endTimeRange - 1, 1, 0)
						Dim expLastActL1 As INDArray = fullOutL1.tensorAlongDimension(endTimeRange - 1, 1, 0)

						assertEquals(expLastActL0, lastActL0)
						assertEquals(expLastActL1, lastActL1)
					Next j
				Next i
			Next layerType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnTimeStep2dInput()
		Public Overridable Sub testRnnTimeStep2dInput()
			Nd4j.Random.setSeed(12345)
			Dim timeSeriesLength As Integer = 6

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(5).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(4).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build()).build()
			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			Dim input3d As INDArray = Nd4j.rand(New Long() {3, 5, timeSeriesLength})
			Dim out3d As INDArray = mln.rnnTimeStep(input3d)
			assertArrayEquals(out3d.shape(), New Long() {3, 4, timeSeriesLength})

			mln.rnnClearPreviousState()
			For i As Integer = 0 To timeSeriesLength - 1
				Dim input2d As INDArray = input3d.tensorAlongDimension(i, 1, 0)
				Dim out2d As INDArray = mln.rnnTimeStep(input2d)

				assertArrayEquals(out2d.shape(), New Long() {3, 4})

				Dim expOut2d As INDArray = out3d.tensorAlongDimension(i, 1, 0)
				assertEquals(out2d, expOut2d)
			Next i

			'Check same but for input of size [3,5,1]. Expect [3,4,1] out
			mln.rnnClearPreviousState()
			For i As Integer = 0 To timeSeriesLength - 1
				Dim temp As INDArray = Nd4j.create(New Integer() {3, 5, 1})
				temp.tensorAlongDimension(0, 1, 0).assign(input3d.tensorAlongDimension(i, 1, 0))
				Dim out3dSlice As INDArray = mln.rnnTimeStep(temp)
				assertArrayEquals(out3dSlice.shape(), New Long() {3, 4, 1})

				assertTrue(out3dSlice.tensorAlongDimension(0, 1, 0).Equals(out3d.tensorAlongDimension(i, 1, 0)))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTruncatedBPTTVsBPTT()
		Public Overridable Sub testTruncatedBPTTVsBPTT()
			'Under some (limited) circumstances, we expect BPTT and truncated BPTT to be identical
			'Specifically TBPTT over entire data vector

			Dim timeSeriesLength As Integer = 12
			Dim miniBatchSize As Integer = 7
			Dim nIn As Integer = 5
			Dim nOut As Integer = 4

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(WorkspaceMode.NONE).inferenceWorkspaceMode(WorkspaceMode.NONE).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build()).build()
			assertEquals(BackpropType.Standard, conf.getBackpropType())

			Dim confTBPTT As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(WorkspaceMode.NONE).inferenceWorkspaceMode(WorkspaceMode.NONE).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build()).backpropType(BackpropType.TruncatedBPTT).tBPTTBackwardLength(timeSeriesLength).tBPTTForwardLength(timeSeriesLength).build()

			Nd4j.Random.setSeed(12345)
			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			Nd4j.Random.setSeed(12345)
			Dim mlnTBPTT As New MultiLayerNetwork(confTBPTT)
			mlnTBPTT.init()

			mlnTBPTT.clearTbpttState = False

			assertEquals(BackpropType.TruncatedBPTT, mlnTBPTT.LayerWiseConfigurations.getBackpropType())
			assertEquals(timeSeriesLength, mlnTBPTT.LayerWiseConfigurations.getTbpttFwdLength())
			assertEquals(timeSeriesLength, mlnTBPTT.LayerWiseConfigurations.getTbpttBackLength())

			Dim inputData As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, timeSeriesLength})
			Dim labels As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nOut, timeSeriesLength})

			mln.Input = inputData
			mln.Labels = labels

			mlnTBPTT.Input = inputData
			mlnTBPTT.Labels = labels

			mln.computeGradientAndScore()
			mlnTBPTT.computeGradientAndScore()

			Dim mlnPair As Pair(Of Gradient, Double) = mln.gradientAndScore()
			Dim tbpttPair As Pair(Of Gradient, Double) = mlnTBPTT.gradientAndScore()

			assertEquals(mlnPair.First.gradientForVariable(), tbpttPair.First.gradientForVariable())
			assertEquals(mlnPair.Second, tbpttPair.Second, 1e-8)

			'Check states: expect stateMap to be empty but tBpttStateMap to not be
			Dim l0StateMLN As IDictionary(Of String, INDArray) = mln.rnnGetPreviousState(0)
			Dim l0StateTBPTT As IDictionary(Of String, INDArray) = mlnTBPTT.rnnGetPreviousState(0)
			Dim l1StateMLN As IDictionary(Of String, INDArray) = mln.rnnGetPreviousState(0)
			Dim l1StateTBPTT As IDictionary(Of String, INDArray) = mlnTBPTT.rnnGetPreviousState(0)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> l0TBPTTStateMLN = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) mln.getLayer(0)).rnnGetTBPTTState();
			Dim l0TBPTTStateMLN As IDictionary(Of String, INDArray) = DirectCast(mln.getLayer(0), BaseRecurrentLayer(Of Object)).rnnGetTBPTTState()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> l0TBPTTStateTBPTT = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) mlnTBPTT.getLayer(0)).rnnGetTBPTTState();
			Dim l0TBPTTStateTBPTT As IDictionary(Of String, INDArray) = DirectCast(mlnTBPTT.getLayer(0), BaseRecurrentLayer(Of Object)).rnnGetTBPTTState()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> l1TBPTTStateMLN = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) mln.getLayer(1)).rnnGetTBPTTState();
			Dim l1TBPTTStateMLN As IDictionary(Of String, INDArray) = DirectCast(mln.getLayer(1), BaseRecurrentLayer(Of Object)).rnnGetTBPTTState()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> l1TBPTTStateTBPTT = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) mlnTBPTT.getLayer(1)).rnnGetTBPTTState();
			Dim l1TBPTTStateTBPTT As IDictionary(Of String, INDArray) = DirectCast(mlnTBPTT.getLayer(1), BaseRecurrentLayer(Of Object)).rnnGetTBPTTState()

			assertTrue(l0StateMLN.Count = 0)
			assertTrue(l0StateTBPTT.Count = 0)
			assertTrue(l1StateMLN.Count = 0)
			assertTrue(l1StateTBPTT.Count = 0)

			assertTrue(l0TBPTTStateMLN.Count = 0)
			assertEquals(2, l0TBPTTStateTBPTT.Count)
			assertTrue(l1TBPTTStateMLN.Count = 0)
			assertEquals(2, l1TBPTTStateTBPTT.Count)

			Dim tbpttActL0 As INDArray = l0TBPTTStateTBPTT(GravesLSTM.STATE_KEY_PREV_ACTIVATION)
			Dim tbpttActL1 As INDArray = l1TBPTTStateTBPTT(GravesLSTM.STATE_KEY_PREV_ACTIVATION)

			Dim activations As IList(Of INDArray) = mln.feedForward(inputData, True)
			Dim l0Act As INDArray = activations(1)
			Dim l1Act As INDArray = activations(2)
			Dim expL0Act As INDArray = l0Act.tensorAlongDimension(timeSeriesLength - 1, 1, 0)
			Dim expL1Act As INDArray = l1Act.tensorAlongDimension(timeSeriesLength - 1, 1, 0)
			assertEquals(tbpttActL0, expL0Act)
			assertEquals(tbpttActL1, expL1Act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnActivateUsingStoredState()
		Public Overridable Sub testRnnActivateUsingStoredState()
			Dim timeSeriesLength As Integer = 12
			Dim miniBatchSize As Integer = 7
			Dim nIn As Integer = 5
			Dim nOut As Integer = 4

			Dim nTimeSlices As Integer = 5

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build()).build()

			Nd4j.Random.setSeed(12345)
			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			Dim inputLong As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, nTimeSlices * timeSeriesLength})
			Dim input As INDArray = inputLong.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, timeSeriesLength))

			Dim outStandard As IList(Of INDArray) = mln.feedForward(input, True)
			Dim outRnnAct As IList(Of INDArray) = mln.rnnActivateUsingStoredState(input, True, True)

			'As initially state is zeros: expect these to be the same
			assertEquals(outStandard, outRnnAct)

			'Furthermore, expect multiple calls to this function to be the same:
			For i As Integer = 0 To 2
				assertEquals(outStandard, mln.rnnActivateUsingStoredState(input, True, True))
			Next i

			Dim outStandardLong As IList(Of INDArray) = mln.feedForward(inputLong, True)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?> l0 = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) mln.getLayer(0));
			Dim l0 As BaseRecurrentLayer(Of Object) = (DirectCast(mln.getLayer(0), BaseRecurrentLayer(Of Object)))
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?> l1 = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) mln.getLayer(1));
			Dim l1 As BaseRecurrentLayer(Of Object) = (DirectCast(mln.getLayer(1), BaseRecurrentLayer(Of Object)))

			For i As Integer = 0 To nTimeSlices - 1
				Dim inSlice As INDArray = inputLong.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(i * timeSeriesLength, (i + 1) * timeSeriesLength))
				Dim outSlice As IList(Of INDArray) = mln.rnnActivateUsingStoredState(inSlice, True, True)
				Dim expOut As IList(Of INDArray) = New List(Of INDArray)()
				For Each temp As INDArray In outStandardLong
					expOut.Add(temp.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(i * timeSeriesLength, (i + 1) * timeSeriesLength)))
				Next temp

				For j As Integer = 0 To expOut.Count - 1
					Dim exp As INDArray = expOut(j)
					Dim act As INDArray = outSlice(j)
	'                System.out.println(j);
	'                System.out.println(exp.sub(act));
					assertEquals(exp, act)
				Next j

				assertEquals(expOut, outSlice)

				'Again, expect multiple calls to give the same output
				For j As Integer = 0 To 2
					outSlice = mln.rnnActivateUsingStoredState(inSlice, True, True)
					assertEquals(expOut, outSlice)
				Next j

				l0.rnnSetPreviousState(l0.rnnGetTBPTTState())
				l1.rnnSetPreviousState(l1.rnnGetTBPTTState())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTruncatedBPTTSimple()
		Public Overridable Sub testTruncatedBPTTSimple()
			'Extremely simple test of the 'does it throw an exception' variety
			Dim timeSeriesLength As Integer = 12
			Dim miniBatchSize As Integer = 7
			Dim nIn As Integer = 5
			Dim nOut As Integer = 4

			Dim nTimeSlices As Integer = 20

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build()).backpropType(BackpropType.TruncatedBPTT).tBPTTBackwardLength(timeSeriesLength).tBPTTForwardLength(timeSeriesLength).build()

			Nd4j.Random.setSeed(12345)
			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			Dim inputLong As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, nTimeSlices * timeSeriesLength})
			Dim labelsLong As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nOut, nTimeSlices * timeSeriesLength})

			mln.fit(inputLong, labelsLong)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTruncatedBPTTWithMasking()
		Public Overridable Sub testTruncatedBPTTWithMasking()
			'Extremely simple test of the 'does it throw an exception' variety
			Dim timeSeriesLength As Integer = 100
			Dim tbpttLength As Integer = 10
			Dim miniBatchSize As Integer = 7
			Dim nIn As Integer = 5
			Dim nOut As Integer = 4

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build()).backpropType(BackpropType.TruncatedBPTT).tBPTTBackwardLength(tbpttLength).tBPTTForwardLength(tbpttLength).build()

			Nd4j.Random.setSeed(12345)
			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			Dim features As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, timeSeriesLength})
			Dim labels As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nOut, timeSeriesLength})

			Dim maskArrayInput As INDArray = Nd4j.ones(miniBatchSize, timeSeriesLength)
			Dim maskArrayOutput As INDArray = Nd4j.ones(miniBatchSize, timeSeriesLength)

			Dim ds As New DataSet(features, labels, maskArrayInput, maskArrayOutput)

			mln.fit(ds)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnTimeStepWithPreprocessor()
		Public Overridable Sub testRnnTimeStepWithPreprocessor()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(10).nOut(10).activation(Activation.TANH).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(10).nOut(10).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).inputPreProcessor(0, New FeedForwardToRnnPreProcessor()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim [in] As INDArray = Nd4j.rand(1, 10)
			net.rnnTimeStep([in])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnTimeStepWithPreprocessorGraph()
		Public Overridable Sub testRnnTimeStepWithPreprocessorGraph()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(10).nOut(10).activation(Activation.TANH).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(10).nOut(10).activation(Activation.TANH).build(), "0").addLayer("2", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "1").setOutputs("2").inputPreProcessor("0", New FeedForwardToRnnPreProcessor()).build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim [in] As INDArray = Nd4j.rand(1, 10)
			net.rnnTimeStep([in])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTBPTTLongerThanTS()
		Public Overridable Sub testTBPTTLongerThanTS()
			'Extremely simple test of the 'does it throw an exception' variety
			Dim timeSeriesLength As Integer = 20
			Dim tbpttLength As Integer = 1000
			Dim miniBatchSize As Integer = 7
			Dim nIn As Integer = 5
			Dim nOut As Integer = 4

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.XAVIER).list().layer(0, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(8).nOut(nOut).activation(Activation.IDENTITY).build()).backpropType(BackpropType.TruncatedBPTT).tBPTTBackwardLength(tbpttLength).tBPTTForwardLength(tbpttLength).build()

			Nd4j.Random.setSeed(12345)
			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			Dim features As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, timeSeriesLength})
			Dim labels As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nOut, timeSeriesLength})

			Dim maskArrayInput As INDArray = Nd4j.ones(miniBatchSize, timeSeriesLength)
			Dim maskArrayOutput As INDArray = Nd4j.ones(miniBatchSize, timeSeriesLength)

			Dim ds As New DataSet(features, labels, maskArrayInput, maskArrayOutput)

			Dim initialParams As INDArray = mln.params().dup()
			mln.fit(ds)
			Dim afterParams As INDArray = mln.params()
			assertNotEquals(initialParams, afterParams)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInvalidTPBTT()
		Public Overridable Sub testInvalidTPBTT()
			Dim nIn As Integer = 8
			Dim nOut As Integer = 25
			Dim nHiddenUnits As Integer = 17

			Try
				Call (New NeuralNetConfiguration.Builder()).list().layer((New org.deeplearning4j.nn.conf.layers.LSTM.Builder()).nIn(nIn).nOut(nHiddenUnits).build()).layer(New GlobalPoolingLayer()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(nHiddenUnits).nOut(nOut).activation(Activation.TANH).build()).backpropType(BackpropType.TruncatedBPTT).build()
				fail("Exception expected")
			Catch e As System.InvalidOperationException
				log.info(e.ToString())
				assertTrue(e.Message.contains("TBPTT") AndAlso e.Message.contains("validateTbpttConfig"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWrapperLayerGetPreviousState()
		Public Overridable Sub testWrapperLayerGetPreviousState()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(New FrozenLayer((New org.deeplearning4j.nn.conf.layers.LSTM.Builder()).nIn(5).nOut(5).build())).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim [in] As INDArray = Nd4j.create(1, 5, 2)
			net.rnnTimeStep([in])

			Dim m As IDictionary(Of String, INDArray) = net.rnnGetPreviousState(0)
			assertNotNull(m)
			assertEquals(2, m.Count) 'activation and cell state

			net.rnnSetPreviousState(0, m)

			Dim cg As ComputationGraph = net.toComputationGraph()
			cg.rnnTimeStep([in])
			m = cg.rnnGetPreviousState(0)
			assertNotNull(m)
			assertEquals(2, m.Count) 'activation and cell state
			cg.rnnSetPreviousState(0, m)
		End Sub
	End Class

End Namespace