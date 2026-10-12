Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports org.deeplearning4j.nn.conf.layers
Imports ElementWiseMultiplicationLayer = org.deeplearning4j.nn.conf.layers.misc.ElementWiseMultiplicationLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.deeplearning4j.gradientcheck.GradientCheckUtil.checkGradients
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

Namespace org.deeplearning4j.gradientcheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class GradientCheckTests extends org.deeplearning4j.BaseDL4JTest
	Public Class GradientCheckTests
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMinibatchApplication()
		Public Overridable Sub testMinibatchApplication()
			Dim iter As New IrisDataSetIterator(30, 150)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).miniBatch(False).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New NoOp()).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build()

			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			assertEquals(1,mln.InputMiniBatchSize)

			Dim scaler As DataNormalization = New NormalizerMinMaxScaler()
			scaler.fit(iter)
			iter.PreProcessor = scaler
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()

			Dim doLearningFirst As Boolean = True
			Dim outputActivation As String = "tanh"
			Dim afn As String = outputActivation
			Dim lf As String = "negativeloglikelihood"
			If doLearningFirst Then
				'Run a number of iterations of learning
				mln.Input = ds.Features
				mln.Labels = ds.Labels
				mln.computeGradientAndScore()
				Dim scoreBefore As Double = mln.score()
				For j As Integer = 0 To 9
					mln.fit(ds)
				Next j
				mln.computeGradientAndScore()
				Dim scoreAfter As Double = mln.score()
				'Can't test in 'characteristic mode of operation' if not learning
				Dim msg As String = "testMinibatchApplication() - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
			End If

			If PRINT_RESULTS Then
				Console.WriteLine("testMinibatchApplication() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst)
	'            for (int j = 0; j < mln.getnLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, ds.Features, ds.Labels)

			Dim msg As String = "testMinibatchApplication() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst
			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(mln)
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientMLP2LayerIrisSimple()
		Public Overridable Sub testGradientMLP2LayerIrisSimple()
			'Parameterized test, testing combinations of:
			' (a) activation function
			' (b) Whether to test at random initialization, or after some learning (i.e., 'characteristic mode of operation')
			' (c) Loss function (with specified output activations)
			Dim activFns() As Activation = {Activation.SIGMOID, Activation.TANH, Activation.MISH}
			Dim characteristic() As Boolean = {False, True} 'If true: run some backprop steps first

			Dim lossFunctions() As LossFunctions.LossFunction = {LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.MSE}
			Dim outputActivations() As Activation = {Activation.SOFTMAX, Activation.TANH} 'i.e., lossFunctions[i] used with outputActivations[i] here
			Dim scaler As DataNormalization = New NormalizerMinMaxScaler()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			scaler.fit(iter)
			iter.PreProcessor = scaler
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()

			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels

			For Each afn As Activation In activFns
				For Each doLearningFirst As Boolean In characteristic
					For i As Integer = 0 To lossFunctions.Length - 1
						Dim lf As LossFunctions.LossFunction = lossFunctions(i)
						Dim outputActivation As Activation = outputActivations(i)

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).updater(New NoOp()).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).dist(New NormalDistribution(0, 1)).activation(afn).build()).layer(1, (New OutputLayer.Builder(lf)).activation(outputActivation).nIn(3).nOut(3).dist(New NormalDistribution(0, 1)).build()).build()

						Dim mln As New MultiLayerNetwork(conf)
						mln.init()

						If doLearningFirst Then
							'Run a number of iterations of learning
							mln.Input = ds.Features
							mln.Labels = ds.Labels
							mln.computeGradientAndScore()
							Dim scoreBefore As Double = mln.score()
							For j As Integer = 0 To 9
								mln.fit(ds)
							Next j
							mln.computeGradientAndScore()
							Dim scoreAfter As Double = mln.score()
							'Can't test in 'characteristic mode of operation' if not learning
							Dim msg As String = "testGradMLP2LayerIrisSimple() - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
							'assertTrue(msg, scoreAfter < 0.8 * scoreBefore);
						End If

						If PRINT_RESULTS Then
							Console.WriteLine("testGradientMLP2LayerIrisSimpleRandom() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst)
	'                        for (int j = 0; j < mln.getnLayers(); j++)
	'                            System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
						End If

						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

						Dim msg As String = "testGradMLP2LayerIrisSimple() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst
						assertTrue(gradOK, msg)
						TestUtils.testModelSerialization(mln)
					Next i
				Next doLearningFirst
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientMLP2LayerIrisL1L2Simple()
		Public Overridable Sub testGradientMLP2LayerIrisL1L2Simple()
			'As above (testGradientMLP2LayerIrisSimple()) but with L2, L1, and both L2/L1 applied
			'Need to run gradient through updater, so that L2 can be applied

			Dim activFns() As Activation = {Activation.SIGMOID, Activation.TANH, Activation.THRESHOLDEDRELU}
			Dim characteristic() As Boolean = {False, True} 'If true: run some backprop steps first

			Dim lossFunctions() As LossFunctions.LossFunction = {LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.MSE}
			Dim outputActivations() As Activation = {Activation.SOFTMAX, Activation.TANH} 'i.e., lossFunctions[i] used with outputActivations[i] here

			Dim scaler As DataNormalization = New NormalizerMinMaxScaler()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			scaler.fit(iter)
			iter.PreProcessor = scaler
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()

			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels

			'use l2vals[i] with l1vals[i]
			Dim l2vals() As Double = {0.4, 0.0, 0.4, 0.4}
			Dim l1vals() As Double = {0.0, 0.0, 0.5, 0.0}
			Dim biasL2() As Double = {0.0, 0.0, 0.0, 0.2}
			Dim biasL1() As Double = {0.0, 0.0, 0.6, 0.0}

			For Each afn As Activation In activFns
				For Each doLearningFirst As Boolean In characteristic
					For i As Integer = 0 To lossFunctions.Length - 1
						For k As Integer = 0 To l2vals.Length - 1
							Dim lf As LossFunctions.LossFunction = lossFunctions(i)
							Dim outputActivation As Activation = outputActivations(i)
							Dim l2 As Double = l2vals(k)
							Dim l1 As Double = l1vals(k)

							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(l2).l1(l1).dataType(DataType.DOUBLE).l2Bias(biasL2(k)).l1Bias(biasL1(k)).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(afn).build()).layer(1, (New OutputLayer.Builder(lf)).nIn(3).nOut(3).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(outputActivation).build()).build()

							Dim mln As New MultiLayerNetwork(conf)
							mln.init()
							doLearningFirst = False
							If doLearningFirst Then
								'Run a number of iterations of learning
								mln.Input = ds.Features
								mln.Labels = ds.Labels
								mln.computeGradientAndScore()
								Dim scoreBefore As Double = mln.score()
								For j As Integer = 0 To 9
									mln.fit(ds)
								Next j
								mln.computeGradientAndScore()
								Dim scoreAfter As Double = mln.score()
								'Can't test in 'characteristic mode of operation' if not learning
								Dim msg As String = "testGradMLP2LayerIrisSimple() - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", l2=" & l2 & ", l1=" & l1 & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
								assertTrue(scoreAfter < 0.8 * scoreBefore, msg)
							End If

							If PRINT_RESULTS Then
								Console.WriteLine("testGradientMLP2LayerIrisSimpleRandom() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", l2=" & l2 & ", l1=" & l1)
	'                            for (int j = 0; j < mln.getnLayers(); j++)
	'                                System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
							End If

							Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

							Dim msg As String = "testGradMLP2LayerIrisSimple() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", l2=" & l2 & ", l1=" & l1
							assertTrue(gradOK, msg)
							TestUtils.testModelSerialization(mln)
						Next k
					Next i
				Next doLearningFirst
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEmbeddingLayerPreluSimple()
		Public Overridable Sub testEmbeddingLayerPreluSimple()
			Dim r As New Random(12345)
			Dim nExamples As Integer = 5
			Dim input As INDArray = Nd4j.zeros(nExamples, 1)
			Dim labels As INDArray = Nd4j.zeros(nExamples, 3)
			For i As Integer = 0 To nExamples - 1
				input.putScalar(i, r.Next(4))
				labels.putScalar(New Integer() {i, r.Next(3)}, 1.0)
			Next i

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.2).l1(0.1).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(12345L).list().layer((New EmbeddingLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).updater(New NoOp()).build()).layer((New PReLULayer.Builder()).inputShape(3).sharedAxes(1).updater(New NoOp()).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(3).weightInit(WeightInit.XAVIER).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(Activation.SOFTMAX).build()).build()

			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			If PRINT_RESULTS Then
				Console.WriteLine("testEmbeddingLayerSimple")
	'            for (int j = 0; j < mln.getnLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

			Dim msg As String = "testEmbeddingLayerSimple"
			assertTrue(gradOK, msg)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEmbeddingLayerSimple()
		Public Overridable Sub testEmbeddingLayerSimple()
			Dim r As New Random(12345)
			Dim nExamples As Integer = 5
			Dim input As INDArray = Nd4j.zeros(nExamples, 1)
			Dim labels As INDArray = Nd4j.zeros(nExamples, 3)
			For i As Integer = 0 To nExamples - 1
				input.putScalar(i, r.Next(4))
				labels.putScalar(New Integer() {i, r.Next(3)}, 1.0)
			Next i

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.2).l1(0.1).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(12345L).list().layer(0, (New EmbeddingLayer.Builder()).nIn(4).nOut(3).weightInit(WeightInit.XAVIER).updater(New NoOp()).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(3).weightInit(WeightInit.XAVIER).updater(New NoOp()).activation(Activation.SOFTMAX).build()).build()

			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			If PRINT_RESULTS Then
				Console.WriteLine("testEmbeddingLayerSimple")
	'            for (int j = 0; j < mln.getnLayers(); j++)
	'                System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

			Dim msg As String = "testEmbeddingLayerSimple"
			assertTrue(gradOK, msg)
			TestUtils.testModelSerialization(mln)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAutoEncoder()
		Public Overridable Sub testAutoEncoder()
			'As above (testGradientMLP2LayerIrisSimple()) but with L2, L1, and both L2/L1 applied
			'Need to run gradient through updater, so that L2 can be applied

			Dim activFns() As Activation = {Activation.SIGMOID, Activation.TANH}
			Dim characteristic() As Boolean = {False, True} 'If true: run some backprop steps first

			Dim lossFunctions() As LossFunctions.LossFunction = {LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.MSE}
			Dim outputActivations() As Activation = {Activation.SOFTMAX, Activation.TANH}

			Dim scaler As DataNormalization = New NormalizerMinMaxScaler()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			scaler.fit(iter)
			iter.PreProcessor = scaler
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels

			Dim norm As New NormalizerStandardize()
			norm.fit(ds)
			norm.transform(ds)

			Dim l2vals() As Double = {0.2, 0.0, 0.2}
			Dim l1vals() As Double = {0.0, 0.3, 0.3} 'i.e., use l2vals[i] with l1vals[i]

			For Each afn As Activation In activFns
				For Each doLearningFirst As Boolean In characteristic
					For i As Integer = 0 To lossFunctions.Length - 1
						For k As Integer = 0 To l2vals.Length - 1
							Dim lf As LossFunctions.LossFunction = lossFunctions(i)
							Dim outputActivation As Activation = outputActivations(i)
							Dim l2 As Double = l2vals(k)
							Dim l1 As Double = l1vals(k)

							Nd4j.Random.setSeed(12345)
							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).l2(l2).l1(l1).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).seed(12345L).dist(New NormalDistribution(0, 1)).list().layer(0, (New AutoEncoder.Builder()).nIn(4).nOut(3).activation(afn).build()).layer(1, (New OutputLayer.Builder(lf)).nIn(3).nOut(3).activation(outputActivation).build()).build()

							Dim mln As New MultiLayerNetwork(conf)
							mln.init()

							Dim msg As String
							If doLearningFirst Then
								'Run a number of iterations of learning
								mln.Input = ds.Features
								mln.Labels = ds.Labels
								mln.computeGradientAndScore()
								Dim scoreBefore As Double = mln.score()
								For j As Integer = 0 To 9
									mln.fit(ds)
								Next j
								mln.computeGradientAndScore()
								Dim scoreAfter As Double = mln.score()
								'Can't test in 'characteristic mode of operation' if not learning
								msg = "testGradMLP2LayerIrisSimple() - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", l2=" & l2 & ", l1=" & l1 & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
								assertTrue(scoreAfter < scoreBefore, msg)
							End If

							msg = "testGradMLP2LayerIrisSimple() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", l2=" & l2 & ", l1=" & l1
							If PRINT_RESULTS Then
								Console.WriteLine(msg)
	'                            for (int j = 0; j < mln.getnLayers(); j++)
	'                                System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
							End If

							Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
							assertTrue(gradOK, msg)
							TestUtils.testModelSerialization(mln)
						Next k
					Next i
				Next doLearningFirst
			Next afn
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void elementWiseMultiplicationLayerTest()
		Public Overridable Sub elementWiseMultiplicationLayerTest()

			For Each a As Activation In New Activation(){Activation.IDENTITY, Activation.TANH}

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).updater(New NoOp()).seed(12345L).weightInit(New UniformDistribution(0, 1)).graphBuilder().addInputs("features").addLayer("dense", (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build(), "features").addLayer("elementWiseMul", (New ElementWiseMultiplicationLayer.Builder()).nIn(4).nOut(4).activation(a).build(), "dense").addLayer("loss", (New LossLayer.Builder(LossFunctions.LossFunction.COSINE_PROXIMITY)).activation(Activation.IDENTITY).build(), "elementWiseMul").setOutputs("loss").build()

				Dim netGraph As New ComputationGraph(conf)
				netGraph.init()

				log.info("params before learning: " & netGraph.getLayer(1).paramTable())

				'Run a number of iterations of learning manually make some pseudo data
				'the ides is simple: since we do a element wise multiplication layer (just a scaling), we want the cos sim
				' is mainly decided by the fourth value, if everything runs well, we will get a large weight for the fourth value

				Dim features As INDArray = Nd4j.create(New Double()(){
					New Double() {1, 2, 3, 4},
					New Double() {1, 2, 3, 1},
					New Double() {1, 2, 3, 0}
				})
				Dim labels As INDArray = Nd4j.create(New Double()(){
					New Double() {1, 1, 1, 8},
					New Double() {1, 1, 1, 2},
					New Double() {1, 1, 1, 1}
				})

				netGraph.Inputs = features
				netGraph.Labels = labels
				netGraph.computeGradientAndScore()
				Dim scoreBefore As Double = netGraph.score()

				Dim msg As String
				For epoch As Integer = 0 To 4
					netGraph.fit(New INDArray(){features}, New INDArray(){labels})
				Next epoch
				netGraph.computeGradientAndScore()
				Dim scoreAfter As Double = netGraph.score()
				'Can't test in 'characteristic mode of operation' if not learning
				msg = "elementWiseMultiplicationLayerTest() - score did not (sufficiently) decrease during learning - activationFn=" & "Id" & ", lossFn=" & "Cos-sim" & ", outputActivation=" & "Id" & ", doLearningFirst=" & "true" & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
				assertTrue(scoreAfter < 0.8 * scoreBefore, msg)

	'        expectation in case linear regression(with only element wise multiplication layer): large weight for the fourth weight
				log.info("params after learning: " & netGraph.getLayer(1).paramTable())

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(netGraph).inputs(New INDArray(){features}).labels(New INDArray(){labels}))

				msg = "elementWiseMultiplicationLayerTest() - activationFn=" & "ID" & ", lossFn=" & "Cos-sim" & ", outputActivation=" & "Id" & ", doLearningFirst=" & "true"
				assertTrue(gradOK, msg)

				TestUtils.testModelSerialization(netGraph)
			Next a
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEmbeddingSequenceLayer()
		Public Overridable Sub testEmbeddingSequenceLayer()
			Nd4j.Random.setSeed(12345)

			For Each seqOutputFormat As RNNFormat In System.Enum.GetValues(GetType(RNNFormat))
				For Each maskArray As Boolean In New Boolean(){False, True}
					For Each inputRank As Integer In New Integer(){2, 3}

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).updater(New NoOp()).weightInit(New NormalDistribution(0, 1)).list().layer((New EmbeddingSequenceLayer.Builder()).nIn(8).nOut(4).outputDataFormat(seqOutputFormat).build()).layer((New RnnOutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.TANH).dataFormat(seqOutputFormat).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						Dim ncw As Boolean = seqOutputFormat = RNNFormat.NCW

						Dim [in] As INDArray = Transforms.floor(Nd4j.rand(3, 6).muli(8)) 'Integers 0 to 7 inclusive
						Dim label As INDArray = Nd4j.rand(DataType.FLOAT,If(ncw, New Integer(){3, 3, 6}, New Integer()){3,6,3})

						If inputRank = 3 Then
							'Reshape from [3,6] to [3,1,6]
							[in] = [in].reshape("c"c, 3, 1, 6)
						End If

						Dim fMask As INDArray = Nothing
						If maskArray Then
							fMask = Nd4j.create(New Double()(){
								New Double() {1, 1, 1, 1, 1, 1},
								New Double() {1, 1, 0, 0, 0, 0},
								New Double() {1, 0, 0, 0, 0, 0}
							})

						End If

						Dim msg As String = "mask=" & maskArray & ", inputRank=" & inputRank
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(label).inputMask(fMask))
						assertTrue(gradOK, msg)
						TestUtils.testModelSerialization(net)


						'Also: if mask is present, double check that the masked steps don't impact score
						If maskArray Then
							Dim ds As New DataSet([in], label, fMask, Nothing)
							Dim score As Double = net.score(ds)
							If inputRank = 2 Then
								[in].putScalar(1, 2, 0)
								[in].putScalar(2, 1, 0)
								[in].putScalar(2, 2, 0)
							Else
								[in].putScalar(1, 0, 2, 0)
								[in].putScalar(2, 0, 1, 0)
								[in].putScalar(2, 0, 2, 0)
							End If
							Dim score2 As Double = net.score(ds)
							assertEquals(score, score2, 1e-6)
							If inputRank = 2 Then
								[in].putScalar(1, 2, 1)
								[in].putScalar(2, 1, 1)
								[in].putScalar(2, 2, 1)
							Else
								[in].putScalar(1, 0, 2, 1)
								[in].putScalar(2, 0, 1, 1)
								[in].putScalar(2, 0, 2, 1)
							End If
							Dim score3 As Double = net.score(ds)
							assertEquals(score, score3, 1e-6)
						End If
					Next inputRank
				Next maskArray
			Next seqOutputFormat
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientWeightDecay()
		Public Overridable Sub testGradientWeightDecay()

			Dim activFns() As Activation = {Activation.SIGMOID, Activation.TANH, Activation.THRESHOLDEDRELU}
			Dim characteristic() As Boolean = {False, True} 'If true: run some backprop steps first

			Dim lossFunctions() As LossFunctions.LossFunction = {LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.MSE}
			Dim outputActivations() As Activation = {Activation.SOFTMAX, Activation.TANH} 'i.e., lossFunctions[i] used with outputActivations[i] here

			Dim scaler As DataNormalization = New NormalizerMinMaxScaler()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			scaler.fit(iter)
			iter.PreProcessor = scaler
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()

			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels

			'use l2vals[i] with l1vals[i]
			Dim l2vals() As Double = {0.4, 0.0, 0.4, 0.4, 0.0, 0.0}
			Dim l1vals() As Double = {0.0, 0.0, 0.5, 0.0, 0.5, 0.0}
			Dim biasL2() As Double = {0.0, 0.0, 0.0, 0.2, 0.0, 0.0}
			Dim biasL1() As Double = {0.0, 0.0, 0.6, 0.0, 0.0, 0.5}
			Dim wdVals() As Double = {0.0, 0.0, 0.0, 0.0, 0.4, 0.0}
			Dim wdBias() As Double = {0.0, 0.0, 0.0, 0.0, 0.0, 0.4}

			For Each afn As Activation In activFns
				For i As Integer = 0 To lossFunctions.Length - 1
					For k As Integer = 0 To l2vals.Length - 1
						Dim lf As LossFunctions.LossFunction = lossFunctions(i)
						Dim outputActivation As Activation = outputActivations(i)
						Dim l2 As Double = l2vals(k)
						Dim l1 As Double = l1vals(k)

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(l2).l1(l1).dataType(DataType.DOUBLE).l2Bias(biasL2(k)).l1Bias(biasL1(k)).weightDecay(wdVals(k)).weightDecayBias(wdBias(k)).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(afn).build()).layer(1, (New OutputLayer.Builder(lf)).nIn(3).nOut(3).dist(New NormalDistribution(0, 1)).updater(New NoOp()).activation(outputActivation).build()).build()

						Dim mln As New MultiLayerNetwork(conf)
						mln.init()

						Dim gradOK1 As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

						Dim msg As String = "testGradientWeightDecay() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", l2=" & l2 & ", l1=" & l1
						assertTrue(gradOK1, msg)

						TestUtils.testModelSerialization(mln)
					Next k
				Next i
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/06/24 - Ignored to get to all passing baseline to prevent regressions via CI - see issue #7912") public void testGradientMLP2LayerIrisLayerNorm()
		Public Overridable Sub testGradientMLP2LayerIrisLayerNorm()
			'Parameterized test, testing combinations of:
			' (a) activation function
			' (b) Whether to test at random initialization, or after some learning (i.e., 'characteristic mode of operation')
			' (c) Loss function (with specified output activations)
			' (d) Layer Normalization enabled / disabled
			Dim activFns() As Activation = {Activation.SIGMOID, Activation.TANH}
			Dim characteristic() As Boolean = {True, False} 'If true: run some backprop steps first

			Dim lossFunctions() As LossFunctions.LossFunction = {LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.MSE}
			Dim outputActivations() As Activation = {Activation.SOFTMAX, Activation.TANH} 'i.e., lossFunctions[i] used with outputActivations[i] here
			Dim scaler As DataNormalization = New NormalizerMinMaxScaler()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			scaler.fit(iter)
			iter.PreProcessor = scaler
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()

			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels

			For Each afn As Activation In activFns
				For Each doLearningFirst As Boolean In characteristic
					For i As Integer = 0 To lossFunctions.Length - 1
						For Each layerNorm As Boolean In New Boolean(){True, False}
							Dim lf As LossFunctions.LossFunction = lossFunctions(i)
							Dim outputActivation As Activation = outputActivations(i)

							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).updater(New NoOp()).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).dist(New NormalDistribution(0, 1)).hasLayerNorm(layerNorm).activation(afn).build()).layer(1, (New OutputLayer.Builder(lf)).activation(outputActivation).nIn(3).nOut(3).dist(New NormalDistribution(0, 1)).build()).build()

							Dim mln As New MultiLayerNetwork(conf)
							mln.init()

							If doLearningFirst Then
								'Run a number of iterations of learning
								mln.Input = ds.Features
								mln.Labels = ds.Labels
								mln.computeGradientAndScore()
								Dim scoreBefore As Double = mln.score()
								For j As Integer = 0 To 9
									mln.fit(ds)
								Next j
								mln.computeGradientAndScore()
								Dim scoreAfter As Double = mln.score()
								'Can't test in 'characteristic mode of operation' if not learning
								Dim msg As String = "testGradMLP2LayerIrisSimple() - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", layerNorm=" & layerNorm & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
								'assertTrue(msg, scoreAfter < 0.8 * scoreBefore);
							End If

							If PRINT_RESULTS Then
								Console.WriteLine("testGradientMLP2LayerIrisSimpleRandom() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", layerNorm=" & layerNorm)
	'                            for (int j = 0; j < mln.getnLayers(); j++)
	'                                System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
							End If

							Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

							Dim msg As String = "testGradMLP2LayerIrisSimple() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & ", layerNorm=" & layerNorm
							assertTrue(gradOK, msg)
							TestUtils.testModelSerialization(mln)
						Next layerNorm
					Next i
				Next doLearningFirst
			Next afn
		End Sub
	End Class

End Namespace