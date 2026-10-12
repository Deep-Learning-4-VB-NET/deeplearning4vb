Imports System
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
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
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.linalg.lossfunctions.impl
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.deeplearning4j.gradientcheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class GradientCheckTestsMasking extends org.deeplearning4j.BaseDL4JTest
	Public Class GradientCheckTestsMasking
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

		Private Class GradientCheckSimpleScenario
			Friend ReadOnly lf As ILossFunction
			Friend ReadOnly act As Activation
			Friend ReadOnly nOut As Integer
			Friend ReadOnly labelWidth As Integer

			Friend Sub New(ByVal lf As ILossFunction, ByVal act As Activation, ByVal nOut As Integer, ByVal labelWidth As Integer)
				Me.lf = lf
				Me.act = act
				Me.nOut = nOut
				Me.labelWidth = labelWidth
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void gradientCheckMaskingOutputSimple()
		Public Overridable Sub gradientCheckMaskingOutputSimple()

			Dim timeSeriesLength As Integer = 5
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim mask[][] As Boolean = new Boolean[5][0]
			Dim mask()() As Boolean = RectangularArrays.RectangularBooleanArray(5, 0)
			mask(0) = New Boolean() {True, True, True, True, True} 'No masking
			mask(1) = New Boolean() {False, True, True, True, True} 'mask first output time step
			mask(2) = New Boolean() {False, False, False, False, True} 'time series classification: mask all but last
			mask(3) = New Boolean() {False, False, True, False, True} 'time series classification w/ variable length TS
			mask(4) = New Boolean() {True, True, True, False, True} 'variable length TS

			Dim nIn As Integer = 3
			Dim layerSize As Integer = 3

			Dim scenarios() As GradientCheckSimpleScenario = {
				New GradientCheckSimpleScenario(LossFunctions.LossFunction.MCXENT.getILossFunction(), Activation.SOFTMAX, 2, 2),
				New GradientCheckSimpleScenario(LossMixtureDensity.builder().gaussians(2).labelWidth(3).build(), Activation.TANH, 10, 3),
				New GradientCheckSimpleScenario(LossMixtureDensity.builder().gaussians(2).labelWidth(4).build(), Activation.IDENTITY, 12, 4)
			}

			For Each s As GradientCheckSimpleScenario In scenarios

				Dim r As New Random(12345L)
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, 1, nIn, timeSeriesLength).subi(0.5)

				Dim labels As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, s.labelWidth, timeSeriesLength)
				For m As Integer = 0 To 0
					For j As Integer = 0 To timeSeriesLength - 1
						Dim idx As Integer = r.Next(s.labelWidth)
						labels.putScalar(New Integer() {m, idx, j}, 1.0f)
					Next j
				Next m

				For i As Integer = 0 To mask.Length - 1

					'Create mask array:
					Dim maskArr As INDArray = Nd4j.create(1, timeSeriesLength)
					Dim j As Integer = 0
					Do While j < mask(i).Length
						maskArr.putScalar(New Integer() {0, j},If(mask(i)(j), 1.0, 0.0))
						j += 1
					Loop

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).dataType(DataType.DOUBLE).updater(New NoOp()).list().layer(0, (New SimpleRnn.Builder()).nIn(nIn).nOut(layerSize).weightInit(New NormalDistribution(0, 1)).build()).layer(1, (New RnnOutputLayer.Builder(s.lf)).activation(s.act).nIn(layerSize).nOut(s.nOut).weightInit(New NormalDistribution(0, 1)).build()).build()
					Dim mln As New MultiLayerNetwork(conf)
					mln.init()

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).labelMask(maskArr))

					Dim msg As String = "gradientCheckMaskingOutputSimple() - timeSeriesLength=" & timeSeriesLength & ", miniBatchSize=" & 1
					assertTrue(gradOK,msg)
					TestUtils.testModelSerialization(mln)
				Next i
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBidirectionalLSTMMasking()
		Public Overridable Sub testBidirectionalLSTMMasking()
			Nd4j.Random.Seed = 12345L

			Dim timeSeriesLength As Integer = 5
			Dim nIn As Integer = 3
			Dim layerSize As Integer = 3
			Dim nOut As Integer = 2

			Dim miniBatchSize As Integer = 2

			Dim masks() As INDArray = {
				Nd4j.create(New Double()() {
					New Double() {1, 1, 1, 1, 1},
					New Double() {1, 1, 1, 0, 0}
			}), Nd4j.create(New Double()() {
				New Double() {1, 1, 1, 1, 1},
				New Double() {0, 1, 1, 1, 1}
			})}

			Dim testNum As Integer = 0
			For Each mask As INDArray In masks

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1.0)).seed(12345L).list().layer(0, (New SimpleRnn.Builder()).nIn(nIn).nOut(2).activation(Activation.TANH).build()).layer(1, (New GravesBidirectionalLSTM.Builder()).nIn(2).nOut(layerSize).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).build()).build()

				Dim mln As New MultiLayerNetwork(conf)
				mln.init()

				Dim input As INDArray = Nd4j.rand(New Integer(){miniBatchSize, nIn, timeSeriesLength}, "f"c).subi(0.5)

				Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(miniBatchSize, nOut, timeSeriesLength)

				If PRINT_RESULTS Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: System.out.println("testBidirectionalLSTMMasking() - testNum = " + testNum++);
					Console.WriteLine("testBidirectionalLSTMMasking() - testNum = " & testNum)
						testNum += 1
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).inputMask(mask).labelMask(mask).subset(True).maxPerParam(12))

				assertTrue(gradOK)
				TestUtils.testModelSerialization(mln)
			Next mask
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPerOutputMaskingMLP()
		Public Overridable Sub testPerOutputMaskingMLP()
			Dim nIn As Integer = 6
			Dim layerSize As Integer = 4

			Dim mask1 As INDArray = Nd4j.create(New Double() {1, 0, 0, 1, 0}).reshape(ChrW(1), -1)
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


					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1)).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder()).nIn(layerSize).nOut(nOut).lossFunction(lf).activation(a).build()).validateOutputLayerConfig(False).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim fl() As INDArray = LossFunctionGradientCheck.getFeaturesAndLabels(lf, minibatch, nIn, nOut, 12345)
					Dim features As INDArray = fl(0)
					Dim labels As INDArray = fl(1)

					Dim msg As String = "testPerOutputMaskingMLP(): maskShape = " & Arrays.toString(labelMask.shape()) & ", loss function = " & lf & ", activation = " & a

					Console.WriteLine(msg)

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(features).labels(labels).labelMask(labelMask))

					assertTrue(gradOK,msg)
					TestUtils.testModelSerialization(net)
				Next i
			Next labelMask
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPerOutputMaskingRnn()
		Public Overridable Sub testPerOutputMaskingRnn()
			'For RNNs: per-output masking uses 3d masks (same shape as output/labels), as compared to the standard
			' 2d masks (used for per *example* masking)

			Dim nIn As Integer = 3
			Dim layerSize As Integer = 3
			Dim nOut As Integer = 2

			'1 example, TS length 3
			Dim mask1 As INDArray = Nd4j.create(New Double() {1, 0, 0, 1, 0, 1}, New Integer() {1, nOut, 3}, "f"c)
			'1 example, TS length 1
			Dim mask2 As INDArray = Nd4j.create(New Double() {1, 1}, New Integer() {1, nOut, 1}, "f"c)
			'3 examples, TS length 3
			Dim mask3 As INDArray = Nd4j.create(New Double() { 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1}, New Integer() {3, nOut, 3}, "f"c)
			Dim labelMasks() As INDArray = {mask1, mask2, mask3}

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
				Dim tsLength As val = labelMask.size(2)

				For i As Integer = 0 To lossFunctions.Length - 1
					Dim lf As ILossFunction = lossFunctions(i)
					Dim a As Activation = act(i)

					Nd4j.Random.setSeed(12345)
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1)).seed(12345).list().layer(0, (New SimpleRnn.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build()).layer(1, (New RnnOutputLayer.Builder()).nIn(layerSize).nOut(nOut).lossFunction(lf).activation(a).build()).validateOutputLayerConfig(False).setInputType(InputType.recurrent(nIn,tsLength, RNNFormat.NCW)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim fl() As INDArray = LossFunctionGradientCheck.getFeaturesAndLabels(lf, New Long() {minibatch, nIn, tsLength}, New Long() {minibatch, nOut, tsLength}, 12345)
					Dim features As INDArray = fl(0)
					Dim labels As INDArray = fl(1)

					Dim msg As String = "testPerOutputMaskingRnn(): maskShape = " & Arrays.toString(labelMask.shape()) & ", loss function = " & lf & ", activation = " & a

					Console.WriteLine(msg)

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(features).labels(labels).labelMask(labelMask))

					assertTrue(gradOK,msg)


					'Check the equivalent compgraph:
					Nd4j.Random.setSeed(12345)
					Dim cg As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 2)).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New SimpleRnn.Builder()).nOut(layerSize).activation(Activation.TANH).build(), "in").addLayer("1", (New RnnOutputLayer.Builder()).nIn(layerSize).nOut(nOut).lossFunction(lf).activation(a).build(), "0").setOutputs("1").validateOutputLayerConfig(False).setInputTypes(InputType.recurrent(nIn,tsLength,RNNFormat.NCW)).build()

					Dim graph As New ComputationGraph(cg)
					graph.init()

					gradOK = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(graph).inputs(New INDArray(){features}).labels(New INDArray(){labels}).labelMask(New INDArray(){labelMask}))

					assertTrue(gradOK,msg & " (compgraph)")
					TestUtils.testModelSerialization(graph)
				Next i
			Next labelMask
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutputLayerMasking()
		Public Overridable Sub testOutputLayerMasking()
			Nd4j.Random.setSeed(12345)
			'Idea: RNN input, global pooling, OutputLayer - with "per example" mask arrays

			Dim mb As Integer = 4
			Dim tsLength As Integer = 5
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).weightInit(New NormalDistribution(0,2)).updater(New NoOp()).list().layer((New LSTM.Builder()).nIn(3).nOut(3).build()).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.AVG).build()).layer((New OutputLayer.Builder()).nIn(3).nOut(3).activation(Activation.SOFTMAX).build()).setInputType(InputType.recurrent(3)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim f As INDArray = Nd4j.rand(New Integer(){mb, 3, tsLength})
			Dim l As INDArray = TestUtils.randomOneHot(mb, 3)
			Dim lm As INDArray = TestUtils.randomBernoulli(mb, 1)

			Dim attempts As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while(attempts++ < 1000 && lm.sumNumber().intValue() == 0)
			Do While attempts < 1000 AndAlso lm.sumNumber().intValue() = 0
					attempts += 1
				lm = TestUtils.randomBernoulli(mb, 1)
			Loop
				attempts += 1
			assertTrue(lm.sumNumber().intValue() > 0,"Could not generate non-zero mask after " & attempts & " attempts")

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(f).labels(l).labelMask(lm))
			assertTrue(gradOK)

			'Also ensure score doesn't depend on masked feature or label values
			Dim score As Double = net.score(New DataSet(f,l,Nothing,lm))

			For i As Integer = 0 To mb - 1
				If lm.getDouble(i) <> 0.0 Then
					Continue For
				End If

				Dim fView As INDArray = f.get(interval(i,i,True), all(),all())
				fView.assign(Nd4j.rand(fView.shape()))

				Dim lView As INDArray = l.get(interval(i,i,True), all())
				lView.assign(TestUtils.randomOneHot(1, lView.size(1)))

				Dim score2 As Double = net.score(New DataSet(f,l,Nothing,lm))

				assertEquals(score, score2, 1e-8,i.ToString())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOutputLayerMaskingCG()
		Public Overridable Sub testOutputLayerMaskingCG()
			Nd4j.Random.setSeed(12345)
			'Idea: RNN input, global pooling, OutputLayer - with "per example" mask arrays

			Dim mb As Integer = 10
			Dim tsLength As Integer = 5
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).weightInit(New NormalDistribution(0,2)).updater(New NoOp()).graphBuilder().addInputs("in").layer("0", (New LSTM.Builder()).nIn(3).nOut(3).build(), "in").layer("1", (New GlobalPoolingLayer.Builder()).poolingType(PoolingType.AVG).build(), "0").layer("out", (New OutputLayer.Builder()).nIn(3).nOut(3).activation(Activation.SOFTMAX).build(), "1").setOutputs("out").setInputTypes(InputType.recurrent(3)).build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim f As INDArray = Nd4j.rand(New Integer(){mb, 3, tsLength})
			Dim l As INDArray = TestUtils.randomOneHot(mb, 3)
			Dim lm As INDArray = TestUtils.randomBernoulli(mb, 1)

			Dim attempts As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while(attempts++ < 1000 && lm.sumNumber().intValue() == 0)
			Do While attempts < 1000 AndAlso lm.sumNumber().intValue() = 0
					attempts += 1
				lm = TestUtils.randomBernoulli(mb, 1)
			Loop
				attempts += 1
			assertTrue(lm.sumNumber().intValue() > 0,"Could not generate non-zero mask after " & attempts & " attempts")

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(net).inputs(New INDArray(){f}).labels(New INDArray(){l}).labelMask(New INDArray(){lm}))
			assertTrue(gradOK)

			'Also ensure score doesn't depend on masked feature or label values
			Dim score As Double = net.score(New DataSet(f,l,Nothing,lm))

			For i As Integer = 0 To mb - 1
				If lm.getDouble(i) <> 0.0 Then
					Continue For
				End If

				Dim fView As INDArray = f.get(interval(i,i,True), all(),all())
				fView.assign(Nd4j.rand(fView.shape()))

				Dim lView As INDArray = l.get(interval(i,i,True), all())
				lView.assign(TestUtils.randomOneHot(1, lView.size(1)))

				Dim score2 As Double = net.score(New DataSet(f,l,Nothing,lm))

				assertEquals(score, score2, 1e-8,i.ToString())
			Next i
		End Sub
	End Class

End Namespace