Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IteratorDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorDataSetIterator
Imports IteratorMultiDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorMultiDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports GlobalPoolingLayer = org.deeplearning4j.nn.conf.layers.GlobalPoolingLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers.recurrent
Imports GravesLSTM = org.deeplearning4j.nn.layers.recurrent.GravesLSTM
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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

Namespace org.deeplearning4j.nn.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.DL4J_OLD_API) public class ComputationGraphTestRNN extends org.deeplearning4j.BaseDL4JTest
	Public Class ComputationGraphTestRNN
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnTimeStepGravesLSTM()
		Public Overridable Sub testRnnTimeStepGravesLSTM()
			Nd4j.Random.setSeed(12345)
			Dim timeSeriesLength As Integer = 12

			'4 layer network: 2 GravesLSTM + DenseLayer + RnnOutputLayer. Hence also tests preprocessors.
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(5).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "0").addLayer("2", (New DenseLayer.Builder()).nIn(8).nOut(9).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "1").addLayer("3", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(9).nOut(4).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build(), "2").setOutputs("3").inputPreProcessor("2", New RnnToFeedForwardPreProcessor()).inputPreProcessor("3", New FeedForwardToRnnPreProcessor()).build()
			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim input As INDArray = Nd4j.rand(New Integer() {3, 5, timeSeriesLength})

			Dim allOutputActivations As IDictionary(Of String, INDArray) = graph.feedForward(input, True)
			Dim fullOutL0 As INDArray = allOutputActivations("0")
			Dim fullOutL1 As INDArray = allOutputActivations("1")
			Dim fullOutL3 As INDArray = allOutputActivations("3")

			assertArrayEquals(New Long() {3, 7, timeSeriesLength}, fullOutL0.shape())
			assertArrayEquals(New Long() {3, 8, timeSeriesLength}, fullOutL1.shape())
			assertArrayEquals(New Long() {3, 4, timeSeriesLength}, fullOutL3.shape())

			Dim inputLengths() As Integer = {1, 2, 3, 4, 6, 12}

			'Do steps of length 1, then of length 2, ..., 12
			'Should get the same result regardless of step size; should be identical to standard forward pass
			For i As Integer = 0 To inputLengths.Length - 1
				Dim inLength As Integer = inputLengths(i)
				Dim nSteps As Integer = timeSeriesLength \ inLength 'each of length inLength

				graph.rnnClearPreviousState()

				For j As Integer = 0 To nSteps - 1
					Dim startTimeRange As Integer = j * inLength
					Dim endTimeRange As Integer = startTimeRange + inLength

					Dim inputSubset As INDArray = input.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeRange, endTimeRange))
					If inLength > 1 Then
						assertTrue(inputSubset.size(2) = inLength)
					End If

					Dim outArr() As INDArray = graph.rnnTimeStep(inputSubset)
					assertEquals(1, outArr.Length)
					Dim [out] As INDArray = outArr(0)

					Dim expOutSubset As INDArray
					If inLength = 1 Then
						Dim sizes As val = New Long() {fullOutL3.size(0), fullOutL3.size(1), 1}
						expOutSubset = Nd4j.create(DataType.FLOAT, sizes)
						expOutSubset.tensorAlongDimension(0, 1, 0).assign(fullOutL3.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(startTimeRange)))
					Else
						expOutSubset = fullOutL3.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeRange, endTimeRange))
					End If

					assertEquals(expOutSubset, [out])

					Dim currL0State As IDictionary(Of String, INDArray) = graph.rnnGetPreviousState("0")
					Dim currL1State As IDictionary(Of String, INDArray) = graph.rnnGetPreviousState("1")

					Dim lastActL0 As INDArray = currL0State(GravesLSTM.STATE_KEY_PREV_ACTIVATION)
					Dim lastActL1 As INDArray = currL1State(GravesLSTM.STATE_KEY_PREV_ACTIVATION)

					Dim expLastActL0 As INDArray = fullOutL0.tensorAlongDimension(endTimeRange - 1, 1, 0)
					Dim expLastActL1 As INDArray = fullOutL1.tensorAlongDimension(endTimeRange - 1, 1, 0)

					assertEquals(expLastActL0, lastActL0)
					assertEquals(expLastActL1, lastActL1)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnTimeStep2dInput()
		Public Overridable Sub testRnnTimeStep2dInput()
			Nd4j.Random.setSeed(12345)
			Dim timeSeriesLength As Integer = 6

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(5).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "0").addLayer("2", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(4).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build(), "1").setOutputs("2").build()
			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim input3d As INDArray = Nd4j.rand(New Integer() {3, 5, timeSeriesLength})
			Dim out3d As INDArray = graph.rnnTimeStep(input3d)(0)
			assertArrayEquals(out3d.shape(), New Long() {3, 4, timeSeriesLength})

			graph.rnnClearPreviousState()
			For i As Integer = 0 To timeSeriesLength - 1
				Dim input2d As INDArray = input3d.tensorAlongDimension(i, 1, 0)
				Dim out2d As INDArray = graph.rnnTimeStep(input2d)(0)

				assertArrayEquals(out2d.shape(), New Long() {3, 4})

				Dim expOut2d As INDArray = out3d.tensorAlongDimension(i, 1, 0)
				assertEquals(out2d, expOut2d)
			Next i

			'Check same but for input of size [3,5,1]. Expect [3,4,1] out
			graph.rnnClearPreviousState()
			For i As Integer = 0 To timeSeriesLength - 1
				Dim temp As INDArray = Nd4j.create(New Integer() {3, 5, 1})
				temp.tensorAlongDimension(0, 1, 0).assign(input3d.tensorAlongDimension(i, 1, 0))
				Dim out3dSlice As INDArray = graph.rnnTimeStep(temp)(0)
				assertArrayEquals(out3dSlice.shape(), New Long() {3, 4, 1})

				assertTrue(out3dSlice.tensorAlongDimension(0, 1, 0).Equals(out3d.tensorAlongDimension(i, 1, 0)))
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnTimeStepMultipleInOut()
		Public Overridable Sub testRnnTimeStepMultipleInOut()
			'Test rnnTimeStep functionality with multiple inputs and outputs...

			Nd4j.Random.setSeed(12345)
			Dim timeSeriesLength As Integer = 12

			'4 layer network: 2 GravesLSTM + DenseLayer + RnnOutputLayer. Hence also tests preprocessors.
			'Network architecture: lstm0 -> Dense -> RnnOutputLayer0
			' and lstm1 -> Dense -> RnnOutputLayer1
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in0", "in1").addLayer("lstm0", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(5).nOut(6).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "in0").addLayer("lstm1", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(4).nOut(5).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "in1").addLayer("dense", (New DenseLayer.Builder()).nIn(6 + 5).nOut(9).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "lstm0", "lstm1").addLayer("out0", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(9).nOut(3).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build(), "dense").addLayer("out1", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(9).nOut(4).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build(), "dense").setOutputs("out0", "out1").inputPreProcessor("dense", New RnnToFeedForwardPreProcessor()).inputPreProcessor("out0", New FeedForwardToRnnPreProcessor()).inputPreProcessor("out1", New FeedForwardToRnnPreProcessor()).build()
			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim input0 As INDArray = Nd4j.rand(New Integer() {3, 5, timeSeriesLength})
			Dim input1 As INDArray = Nd4j.rand(New Integer() {3, 4, timeSeriesLength})

			Dim allOutputActivations As IDictionary(Of String, INDArray) = graph.feedForward(New INDArray() {input0, input1}, True)
			Dim fullActLSTM0 As INDArray = allOutputActivations("lstm0")
			Dim fullActLSTM1 As INDArray = allOutputActivations("lstm1")
			Dim fullActOut0 As INDArray = allOutputActivations("out0")
			Dim fullActOut1 As INDArray = allOutputActivations("out1")

			assertArrayEquals(New Long() {3, 6, timeSeriesLength}, fullActLSTM0.shape())
			assertArrayEquals(New Long() {3, 5, timeSeriesLength}, fullActLSTM1.shape())
			assertArrayEquals(New Long() {3, 3, timeSeriesLength}, fullActOut0.shape())
			assertArrayEquals(New Long() {3, 4, timeSeriesLength}, fullActOut1.shape())

			Dim inputLengths() As Integer = {1, 2, 3, 4, 6, 12}

			'Do steps of length 1, then of length 2, ..., 12
			'Should get the same result regardless of step size; should be identical to standard forward pass
			For i As Integer = 0 To inputLengths.Length - 1
				Dim inLength As Integer = inputLengths(i)
				Dim nSteps As Integer = timeSeriesLength \ inLength 'each of length inLength

				graph.rnnClearPreviousState()

				For j As Integer = 0 To nSteps - 1
					Dim startTimeRange As Integer = j * inLength
					Dim endTimeRange As Integer = startTimeRange + inLength

					Dim inputSubset0 As INDArray = input0.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeRange, endTimeRange))
					If inLength > 1 Then
						assertTrue(inputSubset0.size(2) = inLength)
					End If

					Dim inputSubset1 As INDArray = input1.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeRange, endTimeRange))
					If inLength > 1 Then
						assertTrue(inputSubset1.size(2) = inLength)
					End If

					Dim outArr() As INDArray = graph.rnnTimeStep(inputSubset0, inputSubset1)
					assertEquals(2, outArr.Length)
					Dim out0 As INDArray = outArr(0)
					Dim out1 As INDArray = outArr(1)

					Dim expOutSubset0 As INDArray
					If inLength = 1 Then
						Dim sizes As val = New Long() {fullActOut0.size(0), fullActOut0.size(1), 1}
						expOutSubset0 = Nd4j.create(DataType.FLOAT, sizes)
						expOutSubset0.tensorAlongDimension(0, 1, 0).assign(fullActOut0.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(startTimeRange)))
					Else
						expOutSubset0 = fullActOut0.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeRange, endTimeRange))
					End If

					Dim expOutSubset1 As INDArray
					If inLength = 1 Then
						Dim sizes As val = New Long() {fullActOut1.size(0), fullActOut1.size(1), 1}
						expOutSubset1 = Nd4j.create(DataType.FLOAT, sizes)
						expOutSubset1.tensorAlongDimension(0, 1, 0).assign(fullActOut1.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(startTimeRange)))
					Else
						expOutSubset1 = fullActOut1.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeRange, endTimeRange))
					End If

					assertEquals(expOutSubset0, out0)
					assertEquals(expOutSubset1, out1)

					Dim currLSTM0State As IDictionary(Of String, INDArray) = graph.rnnGetPreviousState("lstm0")
					Dim currLSTM1State As IDictionary(Of String, INDArray) = graph.rnnGetPreviousState("lstm1")

					Dim lastActL0 As INDArray = currLSTM0State(GravesLSTM.STATE_KEY_PREV_ACTIVATION)
					Dim lastActL1 As INDArray = currLSTM1State(GravesLSTM.STATE_KEY_PREV_ACTIVATION)

					Dim expLastActL0 As INDArray = fullActLSTM0.tensorAlongDimension(endTimeRange - 1, 1, 0)
					Dim expLastActL1 As INDArray = fullActLSTM1.tensorAlongDimension(endTimeRange - 1, 1, 0)

					assertEquals(expLastActL0, lastActL0)
					assertEquals(expLastActL1, lastActL1)
				Next j
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

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(WorkspaceMode.NONE).inferenceWorkspaceMode(WorkspaceMode.NONE).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "0").addLayer("out", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build(), "1").setInputTypes(InputType.recurrent(nIn,timeSeriesLength,RNNFormat.NCW)).setOutputs("out").build()
			assertEquals(BackpropType.Standard, conf.getBackpropType())

			Dim confTBPTT As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(WorkspaceMode.NONE).inferenceWorkspaceMode(WorkspaceMode.NONE).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "0").addLayer("out", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build(), "1").setOutputs("out").backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(timeSeriesLength).tBPTTBackwardLength(timeSeriesLength).setInputTypes(InputType.recurrent(nIn,timeSeriesLength,RNNFormat.NCW)).build()
			assertEquals(BackpropType.TruncatedBPTT, confTBPTT.getBackpropType())

			Nd4j.Random.setSeed(12345)
			Dim graph As New ComputationGraph(conf)
			graph.init()

			Nd4j.Random.setSeed(12345)
			Dim graphTBPTT As New ComputationGraph(confTBPTT)
			graphTBPTT.init()
			graphTBPTT.clearTbpttState = False

			assertEquals(BackpropType.TruncatedBPTT, graphTBPTT.Configuration.getBackpropType())
			assertEquals(timeSeriesLength, graphTBPTT.Configuration.getTbpttFwdLength())
			assertEquals(timeSeriesLength, graphTBPTT.Configuration.getTbpttBackLength())

			Dim inputData As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, timeSeriesLength})
			Dim labels As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nOut, timeSeriesLength})

			graph.setInput(0, inputData)
			graph.setLabel(0, labels)

			graphTBPTT.setInput(0, inputData)
			graphTBPTT.setLabel(0, labels)

			graph.computeGradientAndScore()
			graphTBPTT.computeGradientAndScore()

			Dim graphPair As Pair(Of Gradient, Double) = graph.gradientAndScore()
			Dim graphTbpttPair As Pair(Of Gradient, Double) = graphTBPTT.gradientAndScore()

			assertEquals(graphPair.First.gradientForVariable(), graphTbpttPair.First.gradientForVariable())
			assertEquals(graphPair.Second, graphTbpttPair.Second, 1e-8)

			'Check states: expect stateMap to be empty but tBpttStateMap to not be
			Dim l0StateMLN As IDictionary(Of String, INDArray) = graph.rnnGetPreviousState(0)
			Dim l0StateTBPTT As IDictionary(Of String, INDArray) = graphTBPTT.rnnGetPreviousState(0)
			Dim l1StateMLN As IDictionary(Of String, INDArray) = graph.rnnGetPreviousState(0)
			Dim l1StateTBPTT As IDictionary(Of String, INDArray) = graphTBPTT.rnnGetPreviousState(0)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> l0TBPTTState = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) graph.getLayer(0)).rnnGetTBPTTState();
			Dim l0TBPTTState As IDictionary(Of String, INDArray) = CType(graph.getLayer(0), BaseRecurrentLayer(Of Object)).rnnGetTBPTTState()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> l0TBPTTStateTBPTT = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) graphTBPTT.getLayer(0)).rnnGetTBPTTState();
			Dim l0TBPTTStateTBPTT As IDictionary(Of String, INDArray) = DirectCast(graphTBPTT.getLayer(0), BaseRecurrentLayer(Of Object)).rnnGetTBPTTState()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> l1TBPTTState = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) graph.getLayer(1)).rnnGetTBPTTState();
			Dim l1TBPTTState As IDictionary(Of String, INDArray) = CType(graph.getLayer(1), BaseRecurrentLayer(Of Object)).rnnGetTBPTTState()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> l1TBPTTStateTBPTT = ((org.deeplearning4j.nn.layers.recurrent.BaseRecurrentLayer<?>) graphTBPTT.getLayer(1)).rnnGetTBPTTState();
			Dim l1TBPTTStateTBPTT As IDictionary(Of String, INDArray) = DirectCast(graphTBPTT.getLayer(1), BaseRecurrentLayer(Of Object)).rnnGetTBPTTState()

			assertTrue(l0StateMLN.Count = 0)
			assertTrue(l0StateTBPTT.Count = 0)
			assertTrue(l1StateMLN.Count = 0)
			assertTrue(l1StateTBPTT.Count = 0)

			assertTrue(l0TBPTTState.Count = 0)
			assertEquals(2, l0TBPTTStateTBPTT.Count)
			assertTrue(l1TBPTTState.Count = 0)
			assertEquals(2, l1TBPTTStateTBPTT.Count)

			Dim tbpttActL0 As INDArray = l0TBPTTStateTBPTT(GravesLSTM.STATE_KEY_PREV_ACTIVATION)
			Dim tbpttActL1 As INDArray = l1TBPTTStateTBPTT(GravesLSTM.STATE_KEY_PREV_ACTIVATION)

			Dim activations As IDictionary(Of String, INDArray) = graph.feedForward(inputData, True)
			Dim l0Act As INDArray = activations("0")
			Dim l1Act As INDArray = activations("1")
			Dim expL0Act As INDArray = l0Act.tensorAlongDimension(timeSeriesLength - 1, 1, 0)
			Dim expL1Act As INDArray = l1Act.tensorAlongDimension(timeSeriesLength - 1, 1, 0)
			assertEquals(tbpttActL0, expL0Act)
			assertEquals(tbpttActL1, expL1Act)
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

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "0").addLayer("out", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build(), "1").setOutputs("out").backpropType(BackpropType.TruncatedBPTT).setInputTypes(InputType.recurrent(nIn,timeSeriesLength,RNNFormat.NCW)).tBPTTBackwardLength(timeSeriesLength).tBPTTForwardLength(timeSeriesLength).build()

			Nd4j.Random.setSeed(12345)
			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim inputLong As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, nTimeSlices * timeSeriesLength})
			Dim labelsLong As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nOut, nTimeSlices * timeSeriesLength})

			graph.fit(New INDArray() {inputLong}, New INDArray() {labelsLong})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTBPTTLongerThanTS()
		Public Overridable Sub testTBPTTLongerThanTS()
			Dim tbpttLength As Integer = 100
			Dim timeSeriesLength As Integer = 20
			Dim miniBatchSize As Integer = 7
			Dim nIn As Integer = 5
			Dim nOut As Integer = 4

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(nIn).nOut(7).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.GravesLSTM.Builder()).nIn(7).nOut(8).activation(Activation.TANH).dist(New NormalDistribution(0, 0.5)).build(), "0").addLayer("out", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(8).nOut(nOut).activation(Activation.SOFTMAX).dist(New NormalDistribution(0, 0.5)).build(), "1").setOutputs("out").backpropType(BackpropType.TruncatedBPTT).tBPTTBackwardLength(tbpttLength).tBPTTForwardLength(tbpttLength).setInputTypes(InputType.recurrent(nIn,timeSeriesLength, RNNFormat.NCW)).build()

			Nd4j.Random.setSeed(12345)
			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim inputLong As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nIn, timeSeriesLength})
			Dim labelsLong As INDArray = Nd4j.rand(New Integer() {miniBatchSize, nOut, timeSeriesLength})

			Dim initialParams As INDArray = graph.params().dup()
			graph.fit(New INDArray() {inputLong}, New INDArray() {labelsLong})
			Dim afterParams As INDArray = graph.params()

			assertNotEquals(initialParams, afterParams)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTbpttMasking()
		Public Overridable Sub testTbpttMasking()
			'Simple "does it throw an exception" type test...
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("out", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(1).nOut(1).build(), "in").setOutputs("out").backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(8).setInputTypes(InputType.recurrent(1,1,RNNFormat.NCW)).tBPTTBackwardLength(8).build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim data As New MultiDataSet(New INDArray() {Nd4j.linspace(1, 10, 10, Nd4j.dataType()).reshape(ChrW(1), 1, 10)}, New INDArray() {Nd4j.linspace(2, 20, 10, Nd4j.dataType()).reshape(ChrW(1), 1, 10)}, Nothing, New INDArray() {Nd4j.ones(1, 10)})

			net.fit(data)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void checkMaskArrayClearance()
		Public Overridable Sub checkMaskArrayClearance()
			For Each tbptt As Boolean In New Boolean() {True, False}
				'Simple "does it throw an exception" type test...
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("out", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(1).nOut(1).build(), "in").setOutputs("out").backpropType(If(tbptt, BackpropType.TruncatedBPTT, BackpropType.Standard)).tBPTTForwardLength(8).tBPTTBackwardLength(8).build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim data As New MultiDataSet(New INDArray() {Nd4j.linspace(1, 10, 10, Nd4j.dataType()).reshape(ChrW(1), 1, 10)}, New INDArray() {Nd4j.linspace(2, 20, 10, Nd4j.dataType()).reshape(ChrW(1), 1, 10)}, New INDArray() {Nd4j.ones(1, 10)}, New INDArray() {Nd4j.ones(1, 10)})

				net.fit(data)
				assertNull(net.InputMaskArrays)
				assertNull(net.LabelMaskArrays)
				For Each l As Layer In net.Layers
					assertNull(l.MaskArray)
				Next l

				Dim ds As New DataSet(data.getFeatures(0), data.getLabels(0), data.getFeaturesMaskArray(0), data.getLabelsMaskArray(0))
				net.fit(ds)
				assertNull(net.InputMaskArrays)
				assertNull(net.LabelMaskArrays)
				For Each l As Layer In net.Layers
					assertNull(l.MaskArray)
				Next l

				net.fit(data.Features, data.Labels, data.FeaturesMaskArrays, data.LabelsMaskArrays)
				assertNull(net.InputMaskArrays)
				assertNull(net.LabelMaskArrays)
				For Each l As Layer In net.Layers
					assertNull(l.MaskArray)
				Next l

				Dim iter As MultiDataSetIterator = New IteratorMultiDataSetIterator(Collections.singletonList(DirectCast(data, org.nd4j.linalg.dataset.api.MultiDataSet)).GetEnumerator(), 1)
				net.fit(iter)
				assertNull(net.InputMaskArrays)
				assertNull(net.LabelMaskArrays)
				For Each l As Layer In net.Layers
					assertNull(l.MaskArray)
				Next l

				Dim iter2 As DataSetIterator = New IteratorDataSetIterator(Collections.singletonList(ds).GetEnumerator(), 1)
				net.fit(iter2)
				assertNull(net.InputMaskArrays)
				assertNull(net.LabelMaskArrays)
				For Each l As Layer In net.Layers
					assertNull(l.MaskArray)
				Next l
			Next tbptt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInvalidTPBTT()
		Public Overridable Sub testInvalidTPBTT()
			Dim nIn As Integer = 8
			Dim nOut As Integer = 25
			Dim nHiddenUnits As Integer = 17

			Try
				Call (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New org.deeplearning4j.nn.conf.layers.LSTM.Builder()).nIn(nIn).nOut(nHiddenUnits).build(), "in").layer("1", New GlobalPoolingLayer(), "0").layer("2", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(nHiddenUnits).nOut(nOut).activation(Activation.TANH).build(), "1").setOutputs("2").backpropType(BackpropType.TruncatedBPTT).build()
				fail("Exception expected")
			Catch e As System.InvalidOperationException
				log.error("",e)
				assertTrue(e.Message.contains("TBPTT") AndAlso e.Message.contains("validateTbpttConfig"))
			End Try
		End Sub

	End Class

End Namespace