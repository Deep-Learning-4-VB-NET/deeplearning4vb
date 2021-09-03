Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Cropping2D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping2D
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.deeplearning4j.nn.conf.ConvolutionMode.Same
import static org.deeplearning4j.nn.conf.ConvolutionMode.Truncate
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports Lists = org.nd4j.shade.guava.collect.Lists

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
'ORIGINAL LINE: @DisplayName("Cnn Gradient Check Test") @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) @Disabled("Fails on GPU to be revisited") class CNNGradientCheckTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CNNGradientCheckTest
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True

		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False

		Private Const DEFAULT_EPS As Double = 1e-6

		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3

		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub



		Public Shared Function params() As Stream(Of Arguments)
			Dim args As IList(Of Arguments) = New List(Of Arguments)()
			For Each nd4jBackend As Nd4jBackend In BaseNd4jTestWithBackends.BACKENDS
				For Each format As CNN2DFormat In CNN2DFormat.values()
					args.Add(Arguments.of(format,nd4jBackend))
				Next format
			Next nd4jBackend
			Return args.stream()
		End Function

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 999990000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Gradient CNNMLN") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") public void testGradientCNNMLN(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGradientCNNMLN(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			If format <> CNN2DFormat.NCHW Then
				Return
			End If
			' Parameterized test, testing combinations of:
			' (a) activation function
			' (b) Whether to test at random initialization, or after some learning (i.e., 'characteristic mode of operation')
			' (c) Loss function (with specified output activations)
			Dim activFns() As Activation = { Activation.SIGMOID, Activation.TANH }
			' If true: run some backprop steps first
			Dim characteristic() As Boolean = { False, True }
			Dim lossFunctions() As LossFunctions.LossFunction = { LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD, LossFunctions.LossFunction.MSE }
			' i.e., lossFunctions[i] used with outputActivations[i] here
			Dim outputActivations() As Activation = { Activation.SOFTMAX, Activation.TANH }
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = (New IrisDataSetIterator(150, 150)).next()
			ds.normalizeZeroMeanZeroUnitVariance()
			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels
			For Each afn As Activation In activFns
				For Each doLearningFirst As Boolean In characteristic
					For i As Integer = 0 To lossFunctions.Length - 1
						Dim lf As LossFunctions.LossFunction = lossFunctions(i)
						Dim outputActivation As Activation = outputActivations(i)
						Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).updater(New NoOp()).weightInit(WeightInit.XAVIER).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder(1, 1)).nOut(6).activation(afn).build()).layer(1, (New OutputLayer.Builder(lf)).activation(outputActivation).nOut(3).build()).setInputType(InputType.convolutionalFlat(1, 4, 1))
						Dim conf As MultiLayerConfiguration = builder.build()
						Dim mln As New MultiLayerNetwork(conf)
						mln.init()
						Dim name As String = (New ObjectAnonymousInnerClass(Me)).GetType().getEnclosingMethod().getName()
						If doLearningFirst Then
							' Run a number of iterations of learning
							mln.Input = ds.Features
							mln.Labels = ds.Labels
							mln.computeGradientAndScore()
							Dim scoreBefore As Double = mln.score()
							For j As Integer = 0 To 9
								mln.fit(ds)
							Next j
							mln.computeGradientAndScore()
							Dim scoreAfter As Double = mln.score()
							' Can't test in 'characteristic mode of operation' if not learning
							Dim msg As String = name & " - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst= " & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
							assertTrue(scoreAfter < 0.9 * scoreBefore,msg)
						End If
						If PRINT_RESULTS Then
							Console.WriteLine(name & " - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst)
							' for (int j = 0; j < mln.getnLayers(); j++)
							' System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
						End If
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
						assertTrue(gradOK)
						TestUtils.testModelSerialization(mln)
					Next i
				Next doLearningFirst
			Next afn
		End Sub

		Private Class ObjectAnonymousInnerClass
			Inherits Object

			Private ReadOnly outerInstance As CNNGradientCheckTest

			Public Sub New(ByVal outerInstance As CNNGradientCheckTest)
				Me.outerInstance = outerInstance
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") @DisplayName("Test Gradient CNNL 1 L 2 MLN") void testGradientCNNL1L2MLN(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testGradientCNNL1L2MLN(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			If format <> CNN2DFormat.NCHW Then
				Return
			End If
			' Parameterized test, testing combinations of:
			' (a) activation function
			' (b) Whether to test at random initialization, or after some learning (i.e., 'characteristic mode of operation')
			' (c) Loss function (with specified output activations)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = (New IrisDataSetIterator(150, 150)).next()
			ds.normalizeZeroMeanZeroUnitVariance()
			Dim input As INDArray = ds.Features
			Dim labels As INDArray = ds.Labels
			' use l2vals[i] with l1vals[i]
			Dim l2vals() As Double = { 0.4, 0.0, 0.4, 0.4 }
			Dim l1vals() As Double = { 0.0, 0.0, 0.5, 0.0 }
			Dim biasL2() As Double = { 0.0, 0.0, 0.0, 0.2 }
			Dim biasL1() As Double = { 0.0, 0.0, 0.6, 0.0 }
			Dim activFns() As Activation = { Activation.SIGMOID, Activation.TANH, Activation.ELU, Activation.SOFTPLUS }
			' If true: run some backprop steps first
			Dim characteristic() As Boolean = { False, True, False, True }
			Dim lossFunctions() As LossFunctions.LossFunction = { LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD, LossFunctions.LossFunction.MSE, LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD, LossFunctions.LossFunction.MSE }
			' i.e., lossFunctions[i] used with outputActivations[i] here
			Dim outputActivations() As Activation = { Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.IDENTITY }
			For i As Integer = 0 To l2vals.Length - 1
				Dim afn As Activation = activFns(i)
				Dim doLearningFirst As Boolean = characteristic(i)
				Dim lf As LossFunctions.LossFunction = lossFunctions(i)
				Dim outputActivation As Activation = outputActivations(i)
				Dim l2 As Double = l2vals(i)
				Dim l1 As Double = l1vals(i)
				Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).l2(l2).l1(l1).l2Bias(biasL2(i)).l1Bias(biasL1(i)).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder(New Integer() { 1, 1 })).nIn(1).nOut(6).weightInit(WeightInit.XAVIER).activation(afn).updater(New NoOp()).build()).layer(1, (New OutputLayer.Builder(lf)).activation(outputActivation).nOut(3).weightInit(WeightInit.XAVIER).updater(New NoOp()).build()).setInputType(InputType.convolutionalFlat(1, 4, 1))
				Dim conf As MultiLayerConfiguration = builder.build()
				Dim mln As New MultiLayerNetwork(conf)
				mln.init()
				Dim testName As String = (New ObjectAnonymousInnerClass2(Me)).GetType().getEnclosingMethod().getName()
				If doLearningFirst Then
					' Run a number of iterations of learning
					mln.Input = ds.Features
					mln.Labels = ds.Labels
					mln.computeGradientAndScore()
					Dim scoreBefore As Double = mln.score()
					For j As Integer = 0 To 9
						mln.fit(ds)
					Next j
					mln.computeGradientAndScore()
					Dim scoreAfter As Double = mln.score()
					' Can't test in 'characteristic mode of operation' if not learning
					Dim msg As String = testName & "- score did not (sufficiently) decrease during learning - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
					assertTrue(scoreAfter < 0.8 * scoreBefore,msg)
				End If
				If PRINT_RESULTS Then
					Console.WriteLine(testName & "- activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", doLearningFirst=" & doLearningFirst)
					' for (int j = 0; j < mln.getnLayers(); j++)
					' System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				End If
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
				assertTrue(gradOK)
				TestUtils.testModelSerialization(mln)
			Next i
		End Sub

		Private Class ObjectAnonymousInnerClass2
			Inherits Object

			Private ReadOnly outerInstance As CNNGradientCheckTest

			Public Sub New(ByVal outerInstance As CNNGradientCheckTest)
				Me.outerInstance = outerInstance
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") @DisplayName("Test Cnn With Space To Depth") void testCnnWithSpaceToDepth(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnWithSpaceToDepth(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nOut As Integer = 4
			Dim minibatchSize As Integer = 2
			Dim width As Integer = 5
			Dim height As Integer = 5
			Dim inputDepth As Integer = 1
			Dim kernel() As Integer = { 2, 2 }
			Dim blocks As Integer = 2
			Dim activations() As String = { "sigmoid" }
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG, SubsamplingLayer.PoolingType.PNORM }
			For Each afn As String In activations
				For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
					Dim input As INDArray = Nd4j.rand(minibatchSize, width * height * inputDepth)
					Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
					For i As Integer = 0 To minibatchSize - 1
						labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
					Next i
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1)).list().layer((New ConvolutionLayer.Builder(kernel)).nIn(inputDepth).hasBias(False).nOut(1).build()).layer((New SpaceToDepthLayer.Builder(blocks, SpaceToDepthLayer.DataFormat.NCHW)).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(2 * 2 * 4).nOut(nOut).build()).setInputType(InputType.convolutionalFlat(height, width, inputDepth)).build()
					Dim net As New MultiLayerNetwork(conf)
					net.init()
					Dim msg As String = "PoolingType=" & poolingType & ", minibatch=" & minibatchSize & ", activationFn=" & afn
					If PRINT_RESULTS Then
						Console.WriteLine(msg)
						' for (int j = 0; j < net.getnLayers(); j++)
						' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
					End If
					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
					assertTrue(gradOK,msg)
					TestUtils.testModelSerialization(net)
				Next poolingType
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn With Space To Batch") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") public void testCnnWithSpaceToBatch(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCnnWithSpaceToBatch(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nOut As Integer = 4
			Dim minibatchSizes() As Integer = { 2, 4 }
			Dim width As Integer = 5
			Dim height As Integer = 5
			Dim inputDepth As Integer = 1
			Dim kernel() As Integer = { 2, 2 }
			Dim blocks() As Integer = { 2, 2 }
			Dim activations() As String = { "sigmoid", "tanh" }
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG, SubsamplingLayer.PoolingType.PNORM }
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For Each afn As String In activations
				For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
					For Each minibatchSize As Integer In minibatchSizes
						Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
						Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
						Dim labels As INDArray = Nd4j.zeros(4 * minibatchSize, nOut)
						Dim i As Integer = 0
						Do While i < 4 * minibatchSize
							labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
							i += 1
						Loop
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(New NormalDistribution(0, 1)).list().layer((New ConvolutionLayer.Builder(kernel)).nIn(inputDepth).nOut(3).dataFormat(format).build()).layer((New SpaceToBatchLayer.Builder(blocks)).dataFormat(format).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim msg As String = format & " - poolingType=" & poolingType & ", minibatch=" & minibatchSize & ", activationFn=" & afn
						If PRINT_RESULTS Then
							Console.WriteLine(msg)
							' for (int j = 0; j < net.getnLayers(); j++)
							' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
						End If
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
						assertTrue(gradOK,msg)
						' Also check compgraph:
						Dim cg As ComputationGraph = net.toComputationGraph()
						gradOK = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(cg).inputs(New INDArray() { input }).labels(New INDArray() { labels }))
						assertTrue(gradOK,msg & " - compgraph")
						TestUtils.testModelSerialization(net)
					Next minibatchSize
				Next poolingType
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn With Upsampling") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnWithUpsampling(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnWithUpsampling(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nOut As Integer = 4
			Dim minibatchSizes() As Integer = { 1, 3 }
			Dim width As Integer = 5
			Dim height As Integer = 5
			Dim inputDepth As Integer = 1
			Dim kernel() As Integer = { 2, 2 }
			Dim stride() As Integer = { 1, 1 }
			Dim padding() As Integer = { 0, 0 }
			Dim size As Integer = 2
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For Each minibatchSize As Integer In minibatchSizes
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = TestUtils.randomOneHot(minibatchSize, nOut)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1)).list().layer((New ConvolutionLayer.Builder(kernel, stride, padding)).nIn(inputDepth).dataFormat(format).nOut(3).build()).layer((New Upsampling2D.Builder()).size(size).dataFormat(format).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(8 * 8 * 3).nOut(4).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim msg As String = "Upsampling - minibatch=" & minibatchSize
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
					' for (int j = 0; j < net.getnLayers(); j++)
					' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
				End If
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next minibatchSize
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn With Subsampling") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnWithSubsampling(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnWithSubsampling(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nOut As Integer = 4
			Dim minibatchSizes() As Integer = { 1, 3 }
			Dim width As Integer = 5
			Dim height As Integer = 5
			Dim inputDepth As Integer = 1
			Dim kernel() As Integer = { 2, 2 }
			Dim stride() As Integer = { 1, 1 }
			Dim padding() As Integer = { 0, 0 }
			Dim pnorm As Integer = 2
			Dim activations() As Activation = { Activation.SIGMOID, Activation.TANH }
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG, SubsamplingLayer.PoolingType.PNORM }
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For Each afn As Activation In activations
				For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
					For Each minibatchSize As Integer In minibatchSizes
						Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
						Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
						Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
						For i As Integer = 0 To minibatchSize - 1
							labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
						Next i
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1)).list().layer(0, (New ConvolutionLayer.Builder(kernel, stride, padding)).nIn(inputDepth).dataFormat(format).nOut(3).build()).layer(1, (New SubsamplingLayer.Builder(poolingType)).dataFormat(format).kernelSize(kernel).stride(stride).padding(padding).pnorm(pnorm).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3 * 3 * 3).nOut(4).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim msg As String = format & " - poolingType=" & poolingType & ", minibatch=" & minibatchSize & ", activationFn=" & afn
						If PRINT_RESULTS Then
							Console.WriteLine(msg)
							' for (int j = 0; j < net.getnLayers(); j++)
							' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
						End If
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
						assertTrue(gradOK,msg)
						TestUtils.testModelSerialization(net)
					Next minibatchSize
				Next poolingType
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn With Subsampling V 2") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnWithSubsamplingV2(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnWithSubsamplingV2(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nOut As Integer = 4
			Dim minibatchSizes() As Integer = { 1, 3 }
			Dim width As Integer = 5
			Dim height As Integer = 5
			Dim inputDepth As Integer = 1
			Dim kernel() As Integer = { 2, 2 }
			Dim stride() As Integer = { 1, 1 }
			Dim padding() As Integer = { 0, 0 }
			Dim pNorm As Integer = 3
			Dim activations() As Activation = { Activation.SIGMOID, Activation.TANH }
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG, SubsamplingLayer.PoolingType.PNORM }
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For Each afn As Activation In activations
				For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
					For Each minibatchSize As Integer In minibatchSizes
						Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
						Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
						Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
						For i As Integer = 0 To minibatchSize - 1
							labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
						Next i
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1)).list().layer(0, (New ConvolutionLayer.Builder(kernel, stride, padding)).nIn(inputDepth).dataFormat(format).nOut(3).build()).layer(1, (New SubsamplingLayer.Builder(poolingType)).dataFormat(format).kernelSize(kernel).stride(stride).padding(padding).pnorm(pNorm).build()).layer(2, (New ConvolutionLayer.Builder(kernel, stride, padding)).dataFormat(format).nIn(3).nOut(2).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(2 * 2 * 2).nOut(4).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim msg As String = "PoolingType=" & poolingType & ", minibatch=" & minibatchSize & ", activationFn=" & afn
						Console.WriteLine(msg)
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
						assertTrue(gradOK,msg)
						TestUtils.testModelSerialization(net)
					Next minibatchSize
				Next poolingType
			Next afn
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn Locally Connected 2 D") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnLocallyConnected2D(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnLocallyConnected2D(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 3
			Dim width As Integer = 5
			Dim height As Integer = 5
			Nd4j.Random.setSeed(12345)
			Dim inputDepths() As Integer = { 1, 2, 4 }
			Dim activations() As Activation = { Activation.SIGMOID, Activation.TANH, Activation.SOFTPLUS }
			Dim minibatch() As Integer = { 2, 1, 3 }
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For i As Integer = 0 To inputDepths.Length - 1
				Dim inputDepth As Integer = inputDepths(i)
				Dim afn As Activation = activations(i)
				Dim minibatchSize As Integer = minibatch(i)
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = TestUtils.randomOneHot(minibatchSize, nOut)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New NoOp()).dataType(DataType.DOUBLE).activation(afn).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).dataFormat(format).padding(0, 0).nIn(inputDepth).nOut(2).build()).layer(1, (New LocallyConnected2D.Builder()).nIn(2).nOut(7).kernelSize(2, 2).dataFormat(format).setInputSize(4, 4).convolutionMode(ConvolutionMode.Strict).hasBias(False).stride(1, 1).padding(0, 0).build()).layer(2, (New ConvolutionLayer.Builder()).nIn(7).nOut(2).kernelSize(2, 2).dataFormat(format).stride(1, 1).padding(0, 0).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(2 * 2 * 2).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
				assertEquals(ConvolutionMode.Truncate, CType(conf.getConf(0).getLayer(), ConvolutionLayer).getConvolutionMode())
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim msg As String = "Minibatch=" & minibatchSize & ", activationFn=" & afn
				Console.WriteLine(msg)
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn Multi Layer") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnMultiLayer(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnMultiLayer(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 2
			Dim minibatchSizes() As Integer = { 1, 2, 5 }
			Dim width As Integer = 5
			Dim height As Integer = 5
			Dim inputDepths() As Integer = { 1, 2, 4 }
			Dim activations() As Activation = { Activation.SIGMOID, Activation.TANH }
			Dim poolingTypes() As SubsamplingLayer.PoolingType = { SubsamplingLayer.PoolingType.MAX, SubsamplingLayer.PoolingType.AVG }
			Nd4j.Random.setSeed(12345)
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For Each inputDepth As Integer In inputDepths
				For Each afn As Activation In activations
					For Each poolingType As SubsamplingLayer.PoolingType In poolingTypes
						For Each minibatchSize As Integer In minibatchSizes
							Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
							Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
							Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
							For i As Integer = 0 To minibatchSize - 1
								labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
							Next i
							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New NoOp()).dataType(DataType.DOUBLE).activation(afn).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).dataFormat(format).padding(0, 0).nIn(inputDepth).nOut(2).build()).layer(1, (New ConvolutionLayer.Builder()).nIn(2).nOut(2).kernelSize(2, 2).dataFormat(format).stride(1, 1).padding(0, 0).build()).layer(2, (New ConvolutionLayer.Builder()).nIn(2).nOut(2).kernelSize(2, 2).dataFormat(format).stride(1, 1).padding(0, 0).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(2 * 2 * 2).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
							assertEquals(ConvolutionMode.Truncate, CType(conf.getConf(0).getLayer(), ConvolutionLayer).getConvolutionMode())
							Dim net As New MultiLayerNetwork(conf)
							net.init()
							For i As Integer = 0 To 3
								Console.WriteLine("nParams, layer " & i & ": " & net.getLayer(i).numParams())
							Next i
							Dim msg As String = "PoolingType=" & poolingType & ", minibatch=" & minibatchSize & ", activationFn=" & afn
							Console.WriteLine(msg)
							Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
							assertTrue(gradOK,msg)
							TestUtils.testModelSerialization(net)
						Next minibatchSize
					Next poolingType
				Next afn
			Next inputDepth
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn Same Padding Mode") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnSamePaddingMode(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnSamePaddingMode(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 2
			Dim minibatchSizes() As Integer = { 1, 3, 3, 2, 1, 2 }
			' Same padding mode: insensitive to exact input size...
			Dim heights() As Integer = { 4, 5, 6, 5, 4, 4 }
			Dim kernelSizes() As Integer = { 2, 3, 2, 3, 2, 3 }
			Dim inputDepths() As Integer = { 1, 2, 4, 3, 2, 3 }
			Dim width As Integer = 5
			Nd4j.Random.setSeed(12345)
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For i As Integer = 0 To minibatchSizes.Length - 1
				Dim inputDepth As Integer = inputDepths(i)
				Dim minibatchSize As Integer = minibatchSizes(i)
				Dim height As Integer = heights(i)
				Dim k As Integer = kernelSizes(i)
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = TestUtils.randomOneHot(minibatchSize, nOut)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).updater(New NoOp()).activation(Activation.SIGMOID).convolutionMode(Same).list().layer(0, (New ConvolutionLayer.Builder()).name("layer 0").kernelSize(k, k).dataFormat(format).stride(1, 1).padding(0, 0).nIn(inputDepth).nOut(2).build()).layer(1, (New SubsamplingLayer.Builder()).dataFormat(format).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(k, k).stride(1, 1).padding(0, 0).build()).layer(2, (New ConvolutionLayer.Builder()).nIn(2).nOut(2).kernelSize(k, k).dataFormat(format).stride(1, 1).padding(0, 0).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim j As Integer = 0
				Do While j < net.Layers.Length
					Console.WriteLine("nParams, layer " & j & ": " & net.getLayer(j).numParams())
					j += 1
				Loop
				Dim msg As String = "Minibatch=" & minibatchSize & ", inDepth=" & inputDepth & ", height=" & height & ", kernelSize=" & k
				Console.WriteLine(msg)
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn Same Padding Mode Strided") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnSamePaddingModeStrided(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnSamePaddingModeStrided(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 2
			Dim minibatchSizes() As Integer = { 1, 3 }
			Dim width As Integer = 16
			Dim height As Integer = 16
			Dim kernelSizes() As Integer = { 2, 3 }
			Dim strides() As Integer = { 1, 2, 3 }
			Dim inputDepths() As Integer = { 1, 3 }
			Nd4j.Random.setSeed(12345)
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For Each inputDepth As Integer In inputDepths
				For Each minibatchSize As Integer In minibatchSizes
					For Each stride As Integer In strides
						For Each k As Integer In kernelSizes
							For Each convFirst As Boolean In New Boolean() { True, False }
								Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
								Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
								Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
								For i As Integer = 0 To minibatchSize - 1
									labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
								Next i
								Dim convLayer As Layer = (New ConvolutionLayer.Builder()).name("layer 0").kernelSize(k, k).dataFormat(format).stride(stride, stride).padding(0, 0).nIn(inputDepth).nOut(2).build()
								Dim poolLayer As Layer = (New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(k, k).dataFormat(format).stride(stride, stride).padding(0, 0).build()
								Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).updater(New NoOp()).activation(Activation.TANH).convolutionMode(Same).list().layer(0,If(convFirst, convLayer, poolLayer)).layer(1,If(convFirst, poolLayer, convLayer)).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
								Dim net As New MultiLayerNetwork(conf)
								net.init()
								Dim i As Integer = 0
								Do While i < net.Layers.Length
									Console.WriteLine("nParams, layer " & i & ": " & net.getLayer(i).numParams())
									i += 1
								Loop
								Dim msg As String = "Minibatch=" & minibatchSize & ", inDepth=" & inputDepth & ", height=" & height & ", kernelSize=" & k & ", stride = " & stride & ", convLayer first = " & convFirst
								Console.WriteLine(msg)
								Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(128))
								assertTrue(gradOK,msg)
								TestUtils.testModelSerialization(net)
							Next convFirst
						Next k
					Next stride
				Next minibatchSize
			Next inputDepth
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn Zero Padding Layer") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnZeroPaddingLayer(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnZeroPaddingLayer(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nOut As Integer = 4
			Dim width As Integer = 6
			Dim height As Integer = 6
			Dim kernel() As Integer = { 2, 2 }
			Dim stride() As Integer = { 1, 1 }
			Dim padding() As Integer = { 0, 0 }
			Dim minibatchSizes() As Integer = { 1, 3, 2 }
			Dim inputDepths() As Integer = { 1, 3, 2 }
			Dim zeroPadLayer()() As Integer = {
				New Integer() { 0, 0, 0, 0 },
				New Integer() { 1, 1, 0, 0 },
				New Integer() { 2, 2, 2, 2 }
			}
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For i As Integer = 0 To minibatchSizes.Length - 1
				Dim minibatchSize As Integer = minibatchSizes(i)
				Dim inputDepth As Integer = inputDepths(i)
				Dim zeroPad() As Integer = zeroPadLayer(i)
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = TestUtils.randomOneHot(minibatchSize, nOut)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1)).list().layer(0, (New ConvolutionLayer.Builder(kernel, stride, padding)).dataFormat(format).nIn(inputDepth).nOut(3).build()).layer(1, (New ZeroPaddingLayer.Builder(zeroPad)).dataFormat(format).build()).layer(2, (New ConvolutionLayer.Builder(kernel, stride, padding)).nIn(3).nOut(3).dataFormat(format).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(4).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				' Check zero padding activation shape
				Dim zpl As org.deeplearning4j.nn.layers.convolution.ZeroPaddingLayer = DirectCast(net.getLayer(1), org.deeplearning4j.nn.layers.convolution.ZeroPaddingLayer)
				Dim expShape() As Long
				If nchw Then
					expShape = New Long() { minibatchSize, inputDepth, height + zeroPad(0) + zeroPad(1), width + zeroPad(2) + zeroPad(3) }
				Else
					expShape = New Long() { minibatchSize, height + zeroPad(0) + zeroPad(1), width + zeroPad(2) + zeroPad(3), inputDepth }
				End If
				Dim [out] As INDArray = zpl.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(expShape, [out].shape())
				Dim msg As String = "minibatch=" & minibatchSize & ", channels=" & inputDepth & ", zeroPad = " & Arrays.toString(zeroPad)
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
					' for (int j = 0; j < net.getnLayers(); j++)
					' System.out.println("Layer " + j + " # params: " + net.getLayer(j).numParams());
				End If
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Deconvolution 2 D") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testDeconvolution2D(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testDeconvolution2D(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 2
			Dim minibatchSizes() As Integer = { 1, 3, 3, 1, 3 }
			Dim kernelSizes() As Integer = { 1, 1, 1, 3, 3 }
			Dim strides() As Integer = { 1, 1, 2, 2, 2 }
			Dim dilation() As Integer = { 1, 2, 1, 2, 2 }
			Dim activations() As Activation = { Activation.SIGMOID, Activation.TANH, Activation.SIGMOID, Activation.SIGMOID, Activation.SIGMOID }
			Dim cModes() As ConvolutionMode = { Same, Same, Truncate, Truncate, Truncate }
			Dim width As Integer = 7
			Dim height As Integer = 7
			Dim inputDepth As Integer = 3
			Nd4j.Random.setSeed(12345)
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For i As Integer = 0 To minibatchSizes.Length - 1
				Dim minibatchSize As Integer = minibatchSizes(i)
				Dim k As Integer = kernelSizes(i)
				Dim s As Integer = strides(i)
				Dim d As Integer = dilation(i)
				Dim cm As ConvolutionMode = cModes(i)
				Dim act As Activation = activations(i)
				Dim w As Integer = d * width
				Dim h As Integer = d * height
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, h, w }, New Long()){ minibatchSize, h, w, inputDepth }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
				For j As Integer = 0 To minibatchSize - 1
					labels.putScalar(New Integer() { j, j Mod nOut }, 1.0)
				Next j
				Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).updater(New NoOp()).activation(act).list().layer((New Deconvolution2D.Builder()).name("deconvolution_2D_layer").kernelSize(k, k).stride(s, s).dataFormat(format).dilation(d, d).convolutionMode(cm).nIn(inputDepth).nOut(nOut).build())
				Dim conf As MultiLayerConfiguration = b.layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(h, w, inputDepth, format)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim j As Integer = 0
				Do While j < net.Layers.Length
					Console.WriteLine("nParams, layer " & j & ": " & net.getLayer(j).numParams())
					j += 1
				Loop
				Dim msg As String = " - mb=" & minibatchSize & ", k=" & k & ", s=" & s & ", d=" & d & ", cm=" & cm
				Console.WriteLine(msg)
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(100))
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Separable Conv 2 D") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testSeparableConv2D(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testSeparableConv2D(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 2
			Dim width As Integer = 6
			Dim height As Integer = 6
			Dim inputDepth As Integer = 3
			Nd4j.Random.setSeed(12345)
			Dim ks() As Integer = { 1, 3, 3, 1, 3 }
			Dim ss() As Integer = { 1, 1, 1, 2, 2 }
			Dim ds() As Integer = { 1, 1, 2, 2, 2 }
			Dim cms() As ConvolutionMode = { Truncate, Truncate, Truncate, Truncate, Truncate }
			Dim mb() As Integer = { 1, 1, 1, 3, 3 }
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For t As Integer = 0 To ks.Length - 1
				Dim k As Integer = ks(t)
				Dim s As Integer = ss(t)
				Dim d As Integer = ds(t)
				Dim cm As ConvolutionMode = cms(t)
				Dim minibatchSize As Integer = mb(t)
				' Use larger input with larger dilation values (to avoid invalid config)
				Dim w As Integer = d * width
				Dim h As Integer = d * height
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, h, w }, New Long()){ minibatchSize, h, w, inputDepth }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
				For i As Integer = 0 To minibatchSize - 1
					labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
				Next i
				Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).updater(New NoOp()).activation(Activation.TANH).convolutionMode(cm).list().layer((New SeparableConvolution2D.Builder()).name("Separable conv 2D layer").kernelSize(k, k).stride(s, s).dilation(d, d).depthMultiplier(3).dataFormat(format).nIn(inputDepth).nOut(2).build())
				Dim conf As MultiLayerConfiguration = b.layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(h, w, inputDepth, format)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim i As Integer = 0
				Do While i < net.Layers.Length
					Console.WriteLine("nParams, layer " & i & ": " & net.getLayer(i).numParams())
					i += 1
				Loop
				Dim msg As String = " - mb=" & minibatchSize & ", k=" & k & ", s=" & s & ", d=" & d & ", cm=" & cm
				Console.WriteLine(msg)
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(50))
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cnn Dilated") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCnnDilated(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCnnDilated(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 2
			Dim minibatchSize As Integer = 2
			Dim width As Integer = 8
			Dim height As Integer = 8
			Dim inputDepth As Integer = 2
			Nd4j.Random.setSeed(12345)
			Dim [sub]() As Boolean = { True, True, False, True, False }
			Dim stride() As Integer = { 1, 1, 1, 2, 2 }
			Dim kernel() As Integer = { 2, 3, 3, 3, 3 }
			Dim ds() As Integer = { 2, 2, 3, 3, 2 }
			Dim cms() As ConvolutionMode = { Same, Truncate, Truncate, Same, Truncate }
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For t As Integer = 0 To [sub].Length - 1
				Dim subsampling As Boolean = [sub](t)
				Dim s As Integer = stride(t)
				Dim k As Integer = kernel(t)
				Dim d As Integer = ds(t)
				Dim cm As ConvolutionMode = cms(t)
				' Use larger input with larger dilation values (to avoid invalid config)
				Dim w As Integer = d * width
				Dim h As Integer = d * height
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, h, w }, New Long()){ minibatchSize, h, w, inputDepth }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
				For i As Integer = 0 To minibatchSize - 1
					labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
				Next i
				Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).updater(New NoOp()).activation(Activation.TANH).convolutionMode(cm).list().layer((New ConvolutionLayer.Builder()).name("layer 0").kernelSize(k, k).stride(s, s).dilation(d, d).dataFormat(format).nIn(inputDepth).nOut(2).build())
				If subsampling Then
					b.layer((New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(k, k).stride(s, s).dilation(d, d).dataFormat(format).build())
				Else
					b.layer((New ConvolutionLayer.Builder()).nIn(2).nOut(2).kernelSize(k, k).stride(s, s).dilation(d, d).dataFormat(format).build())
				End If
				Dim conf As MultiLayerConfiguration = b.layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(h, w, inputDepth, format)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim i As Integer = 0
				Do While i < net.Layers.Length
					Console.WriteLine("nParams, layer " & i & ": " & net.getLayer(i).numParams())
					i += 1
				Loop
				Dim msg As String = (If(subsampling, "subsampling", "conv")) & " - mb=" & minibatchSize & ", k=" & k & ", s=" & s & ", d=" & d & ", cm=" & cm
				Console.WriteLine(msg)
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Cropping 2 D Layer") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testCropping2DLayer(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Friend Overridable Sub testCropping2DLayer(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nOut As Integer = 2
			Dim width As Integer = 12
			Dim height As Integer = 11
			Dim kernel() As Integer = { 2, 2 }
			Dim stride() As Integer = { 1, 1 }
			Dim padding() As Integer = { 0, 0 }
			Dim cropTestCases()() As Integer = {
				New Integer() { 0, 0, 0, 0 },
				New Integer() { 1, 1, 0, 0 },
				New Integer() { 2, 2, 2, 2 },
				New Integer() { 1, 2, 3, 4 }
			}
			Dim inputDepths() As Integer = { 1, 2, 3, 2 }
			Dim minibatchSizes() As Integer = { 2, 1, 3, 2 }
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For i As Integer = 0 To cropTestCases.Length - 1
				Dim inputDepth As Integer = inputDepths(i)
				Dim minibatchSize As Integer = minibatchSizes(i)
				Dim crop() As Integer = cropTestCases(i)
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, inputDepth, height, width }, New Long()){ minibatchSize, height, width, inputDepth }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = TestUtils.randomOneHot(minibatchSize, nOut)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).convolutionMode(ConvolutionMode.Same).weightInit(New NormalDistribution(0, 1)).list().layer((New ConvolutionLayer.Builder(kernel, stride, padding)).dataFormat(format).nIn(inputDepth).nOut(2).build()).layer((New Cropping2D.Builder(crop)).dataFormat(format).build()).layer((New ConvolutionLayer.Builder(kernel, stride, padding)).dataFormat(format).nIn(2).nOut(2).build()).layer((New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG)).kernelSize(3, 3).stride(3, 3).dataFormat(format).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, inputDepth, format)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				' Check cropping activation shape
				Dim cl As org.deeplearning4j.nn.layers.convolution.Cropping2DLayer = DirectCast(net.getLayer(1), org.deeplearning4j.nn.layers.convolution.Cropping2DLayer)
				Dim expShape() As Long
				If nchw Then
					expShape = New Long() { minibatchSize, inputDepth, height - crop(0) - crop(1), width - crop(2) - crop(3) }
				Else
					expShape = New Long() { minibatchSize, height - crop(0) - crop(1), width - crop(2) - crop(3), inputDepth }
				End If
				Dim [out] As INDArray = cl.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
				assertArrayEquals(expShape, [out].shape())
				Dim msg As String = format & " - minibatch=" & minibatchSize & ", channels=" & inputDepth & ", zeroPad = " & Arrays.toString(crop)
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
				End If
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(160))
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Depthwise Conv 2 D") @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.CNNGradientCheckTest#params") void testDepthwiseConv2D(org.deeplearning4j.nn.conf.CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backendt)
		Friend Overridable Sub testDepthwiseConv2D(ByVal format As CNN2DFormat, ByVal backendt As Nd4jBackend)
			Dim nIn As Integer = 3
			Dim depthMultiplier As Integer = 2
			Dim nOut As Integer = nIn * depthMultiplier
			Dim width As Integer = 5
			Dim height As Integer = 5
			Nd4j.Random.setSeed(12345)
			Dim ks() As Integer = { 1, 3, 3, 1, 3 }
			Dim ss() As Integer = { 1, 1, 1, 2, 2 }
			Dim cms() As ConvolutionMode = { Truncate, Truncate, Truncate, Truncate, Truncate }
			Dim mb() As Integer = { 1, 1, 1, 3, 3 }
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			For t As Integer = 0 To ks.Length - 1
				Dim k As Integer = ks(t)
				Dim s As Integer = ss(t)
				Dim cm As ConvolutionMode = cms(t)
				Dim minibatchSize As Integer = mb(t)
				Dim inShape() As Long = If(nchw, New Long() { minibatchSize, nIn, height, width }, New Long()){ minibatchSize, height, width, nIn }
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape)
				Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
				For i As Integer = 0 To minibatchSize - 1
					labels.putScalar(New Integer() { i, i Mod nOut }, 1.0)
				Next i
				Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).updater(New NoOp()).activation(Activation.TANH).convolutionMode(cm).list().layer((New Convolution2D.Builder()).kernelSize(1, 1).stride(1, 1).nIn(nIn).nOut(nIn).dataFormat(format).build()).layer((New DepthwiseConvolution2D.Builder()).name("depth-wise conv 2D layer").cudnnAllowFallback(False).kernelSize(k, k).stride(s, s).depthMultiplier(depthMultiplier).nIn(nIn).build())
				Dim conf As MultiLayerConfiguration = b.layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, nIn, format)).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim i As Integer = 0
				Do While i < net.Layers.Length
					Console.WriteLine("nParams, layer " & i & ": " & net.getLayer(i).numParams())
					i += 1
				Loop
				Dim msg As String = " - mb=" & minibatchSize & ", k=" & k & ", nIn=" & nIn & ", depthMul=" & depthMultiplier & ", s=" & s & ", cm=" & cm
				Console.WriteLine(msg)
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).labels(labels).subset(True).maxPerParam(256))
				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next t
		End Sub
	End Class

End Namespace