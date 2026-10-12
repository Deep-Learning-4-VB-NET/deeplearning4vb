Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports EvaluationBinary = org.deeplearning4j.eval.EvaluationBinary
Imports LossFunctionGradientCheck = org.deeplearning4j.gradientcheck.LossFunctionGradientCheck
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports org.deeplearning4j.nn.conf
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports StackVertex = org.deeplearning4j.nn.conf.graph.StackVertex
Imports UnstackVertex = org.deeplearning4j.nn.conf.graph.UnstackVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports CnnToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToRnnPreProcessor
Imports RnnToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToCnnPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.linalg.lossfunctions.impl
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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestMasking extends org.deeplearning4j.BaseDL4JTest
	Public Class TestMasking
		Inherits BaseDL4JTest

		Shared Sub New()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void checkMaskArrayClearance()
		Public Overridable Sub checkMaskArrayClearance()
			For Each tbptt As Boolean In New Boolean() {True, False}
				'Simple "does it throw an exception" type test...
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(1).nOut(1).build()).backpropType(If(tbptt, BackpropType.TruncatedBPTT, BackpropType.Standard)).tBPTTForwardLength(8).tBPTTBackwardLength(8).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim data As New DataSet(Nd4j.linspace(1, 10, 10).reshape(ChrW(1), 1, 10), Nd4j.linspace(2, 20, 10).reshape(ChrW(1), 1, 10), Nd4j.ones(1, 10), Nd4j.ones(1, 10))

				net.fit(data)
				For Each l As Layer In net.Layers
					assertNull(l.MaskArray)
				Next l


				net.fit(data.Features, data.Labels, data.FeaturesMaskArray, data.LabelsMaskArray)
				For Each l As Layer In net.Layers
					assertNull(l.MaskArray)
				Next l

				Dim iter As DataSetIterator = New ExistingDataSetIterator(Collections.singletonList(data).GetEnumerator())
				net.fit(iter)
				For Each l As Layer In net.Layers
					assertNull(l.MaskArray)
				Next l
			Next tbptt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPerOutputMaskingMLN()
		Public Overridable Sub testPerOutputMaskingMLN()
			'Idea: for per-output masking, the contents of the masked label entries should make zero difference to either
			' the score or the gradients

			Dim nIn As Integer = 6
			Dim layerSize As Integer = 4

			Dim mask1 As INDArray = Nd4j.create(New Double() {1, 0, 0, 1, 0}, New Long(){1, 5})
			Dim mask3 As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 1, 1, 1, 1},
				New Double() {0, 1, 0, 1, 0},
				New Double() {1, 0, 0, 1, 1}
			})
			Dim labelMasks() As INDArray = {mask1, mask3}

			Dim lossFunctions() As ILossFunction = {
				New LossBinaryXENT(),
				New LossHinge(),
				New LossKLD(),
				New LossKLD(),
				New LossL1(),
				New LossL2(),
				New LossMAE(),
				New LossMAE(),
				New LossMAPE(),
				New LossMAPE(),
				New LossMCXENT(),
				New LossMSE(),
				New LossMSE(),
				New LossMSLE(),
				New LossMSLE(),
				New LossNegativeLogLikelihood(),
				New LossPoisson(),
				New LossSquaredHinge()
			}

			Dim act() As Activation = {Activation.SIGMOID, Activation.TANH, Activation.SIGMOID, Activation.SOFTMAX, Activation.TANH, Activation.TANH, Activation.TANH, Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.SIGMOID, Activation.TANH, Activation.SOFTMAX, Activation.SIGMOID, Activation.SOFTMAX, Activation.SIGMOID, Activation.SIGMOID, Activation.TANH }

			For Each labelMask As INDArray In labelMasks

				Dim minibatch As val = labelMask.size(0)
				Dim nOut As val = labelMask.size(1)

				For i As Integer = 0 To lossFunctions.Length - 1
					Dim lf As ILossFunction = lossFunctions(i)
					Dim a As Activation = act(i)


					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dist(New NormalDistribution(0, 1)).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder()).nIn(layerSize).nOut(nOut).lossFunction(lf).activation(a).build()).validateOutputLayerConfig(False).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					net.setLayerMaskArrays(Nothing, labelMask)
					Dim fl() As INDArray = LossFunctionGradientCheck.getFeaturesAndLabels(lf, minibatch, nIn, nOut, 12345)
					Dim features As INDArray = fl(0)
					Dim labels As INDArray = fl(1)

					net.Input = features
					net.Labels = labels

					net.computeGradientAndScore()
					Dim score1 As Double = net.score()
					Dim grad1 As INDArray = net.gradient().gradient()

					'Now: change the label values for the masked steps. The

					Dim maskZeroLocations As INDArray = labelMask.rsub(1.0) 'rsub(1): swap 0s and 1s
					Dim rand As INDArray = Nd4j.rand(maskZeroLocations.shape()).muli(0.5)

					Dim newLabels As INDArray = labels.add(rand.muli(maskZeroLocations)) 'Only the masked values are changed

					net.Labels = newLabels
					net.computeGradientAndScore()

					assertNotEquals(labels, newLabels)

					Dim score2 As Double = net.score()
					Dim grad2 As INDArray = net.gradient().gradient()

					assertEquals(score1, score2, 1e-6)
					assertEquals(grad1, grad2)



					'Do the same for CompGraph
					Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dist(New NormalDistribution(0, 1)).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build(), "in").addLayer("1", (New OutputLayer.Builder()).nIn(layerSize).nOut(nOut).lossFunction(lf).activation(a).build(), "0").setOutputs("1").validateOutputLayerConfig(False).build()

					Dim graph As New ComputationGraph(conf2)
					graph.init()

					graph.setLayerMaskArrays(Nothing, New INDArray() {labelMask})

					graph.Inputs = features
					graph.Labels = labels
					graph.computeGradientAndScore()

					Dim gScore1 As Double = graph.score()
					Dim gGrad1 As INDArray = graph.gradient().gradient()

					graph.setLayerMaskArrays(Nothing, New INDArray() {labelMask})
					graph.Inputs = features
					graph.Labels = newLabels
					graph.computeGradientAndScore()

					Dim gScore2 As Double = graph.score()
					Dim gGrad2 As INDArray = graph.gradient().gradient()

					assertEquals(gScore1, gScore2, 1e-6)
					assertEquals(gGrad1, gGrad2)
				Next i
			Next labelMask
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompGraphEvalWithMask()
		Public Overridable Sub testCompGraphEvalWithMask()
			Dim minibatch As Integer = 3
			Dim layerSize As Integer = 6
			Dim nIn As Integer = 5
			Dim nOut As Integer = 4

			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dist(New NormalDistribution(0, 1)).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build(), "in").addLayer("1", (New OutputLayer.Builder()).nIn(layerSize).nOut(nOut).lossFunction(LossFunctions.LossFunction.XENT).activation(Activation.SIGMOID).build(), "0").setOutputs("1").build()

			Dim graph As New ComputationGraph(conf2)
			graph.init()

			Dim f As INDArray = Nd4j.create(minibatch, nIn)
			Dim l As INDArray = Nd4j.create(minibatch, nOut)
			Dim lMask As INDArray = Nd4j.ones(minibatch, nOut)

			Dim ds As New DataSet(f, l, Nothing, lMask)
			Dim iter As DataSetIterator = New ExistingDataSetIterator(Collections.singletonList(ds).GetEnumerator())

			Dim eb As New EvaluationBinary()
			graph.doEvaluation(iter, eb)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnCnnMaskingSimple()
		Public Overridable Sub testRnnCnnMaskingSimple()
			Dim kernelSize1 As Integer = 2
			Dim padding As Integer = 0
			Dim cnnStride1 As Integer = 1
			Dim channels As Integer = 1

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).graphBuilder().addInputs("inputs").addLayer("cnn1", (New ConvolutionLayer.Builder(New Integer() { kernelSize1, kernelSize1 }, New Integer() { cnnStride1, cnnStride1 }, New Integer() { padding, padding })).nIn(channels).nOut(2).build(), "inputs").addLayer("lstm1", (New LSTM.Builder()).nIn(7 * 7 * 2).nOut(2).build(), "cnn1").addLayer("output", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.RELU).nIn(2).nOut(2).build(), "lstm1").setOutputs("output").setInputTypes(InputType.recurrent(7*7, 1)).inputPreProcessor("cnn1", New RnnToCnnPreProcessor(7, 7, channels)).inputPreProcessor("lstm1", New CnnToRnnPreProcessor(7, 7, 2)).build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			cg.fit(New DataSet(Nd4j.create(1, 7*7, 5), Nd4j.create(1, 2, 5), Nd4j.ones(1, 5), Nd4j.ones(1, 5)))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskingStackUnstack()
		Public Overridable Sub testMaskingStackUnstack()

			Dim nnConfig As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(2e-2)).graphBuilder().setInputTypes(InputType.recurrent(3), InputType.recurrent(3)).addInputs("m1", "m2").addVertex("stack", New StackVertex(), "m1", "m2").addLayer("lastUnStacked", New LastTimeStep((New LSTM.Builder()).nIn(3).nOut(1).activation(Activation.TANH).build()), "stack").addVertex("unstacked1", New UnstackVertex(0, 2), "lastUnStacked").addVertex("unstacked2", New UnstackVertex(1, 2), "lastUnStacked").addVertex("restacked", New StackVertex(), "unstacked1", "unstacked2").addVertex("un1", New UnstackVertex(0, 2), "restacked").addVertex("un2", New UnstackVertex(1, 2), "restacked").addVertex("q", New MergeVertex(), "un1", "un2").addLayer("probability", (New OutputLayer.Builder()).nIn(2).nOut(6).lossFunction(LossFunctions.LossFunction.MEAN_ABSOLUTE_ERROR).build(), "q").setOutputs("probability").build()

			Dim cg As New ComputationGraph(nnConfig)
			cg.init()

			Dim i1 As INDArray = Nd4j.create(1, 3, 5)
			Dim i2 As INDArray = Nd4j.create(1, 3, 5)
			Dim fm1 As INDArray = Nd4j.ones(1, 5)
			Dim fm2 As INDArray = Nd4j.ones(1, 5)

			'First: check no masks case
			Dim o1 As INDArray = cg.output(False, New INDArray(){i1, i2}, Nothing)(0)

			'Second: check null mask arrays case
			Dim o2 As INDArray = cg.output(False, New INDArray(){i1, i2}, New INDArray(){Nothing, Nothing})(0)

			'Third: masks present case
			Dim o3 As INDArray = cg.output(False, New INDArray(){i1, i2}, New INDArray(){fm1, fm2})(0)

			assertEquals(o1, o2)
			assertEquals(o1, o3)
		End Sub
	End Class

End Namespace