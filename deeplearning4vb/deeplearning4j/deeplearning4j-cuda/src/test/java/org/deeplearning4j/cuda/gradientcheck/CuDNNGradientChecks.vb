Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.cuda.TestUtils
Imports GradientCheckUtil = org.deeplearning4j.gradientcheck.GradientCheckUtil
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports IDropout = org.deeplearning4j.nn.conf.dropout.IDropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ConvolutionHelper = org.deeplearning4j.nn.layers.convolution.ConvolutionHelper
Imports CudnnConvolutionHelper = org.deeplearning4j.cuda.convolution.CudnnConvolutionHelper
Imports SubsamplingHelper = org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingHelper
Imports CudnnDropoutHelper = org.deeplearning4j.cuda.dropout.CudnnDropoutHelper
Imports BatchNormalizationHelper = org.deeplearning4j.nn.layers.normalization.BatchNormalizationHelper
Imports CudnnBatchNormalizationHelper = org.deeplearning4j.cuda.normalization.CudnnBatchNormalizationHelper
Imports CudnnLocalResponseNormalizationHelper = org.deeplearning4j.cuda.normalization.CudnnLocalResponseNormalizationHelper
Imports LocalResponseNormalizationHelper = org.deeplearning4j.nn.layers.normalization.LocalResponseNormalizationHelper
Imports CudnnLSTMHelper = org.deeplearning4j.cuda.recurrent.CudnnLSTMHelper
Imports LSTMHelper = org.deeplearning4j.nn.layers.recurrent.LSTMHelper
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Test = org.junit.jupiter.api.Test
Imports org.nd4j.common.function
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertNotNull
import static org.junit.jupiter.api.Assertions.assertTrue

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda.gradientcheck


	''' <summary>
	''' Created by Alex on 09/09/2016.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class CuDNNGradientChecks extends org.deeplearning4j.BaseDL4JTest
	Public Class CuDNNGradientChecks
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-5
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-2
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-6

		Shared Sub New()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvolutional() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConvolutional()

			'Parameterized test, testing combinations of:
			' (a) activation function
			' (b) Whether to test at random initialization, or after some learning (i.e., 'characteristic mode of operation')
			' (c) Loss function (with specified output activations)
			Dim activFns() As Activation = {Activation.SIGMOID, Activation.TANH}
			Dim characteristic() As Boolean = {False, True} 'If true: run some backprop steps first

			Dim minibatchSizes() As Integer = {1, 4}
			Dim width As Integer = 6
			Dim height As Integer = 6
			Dim inputDepth As Integer = 2
			Dim nOut As Integer = 3

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.convolution.ConvolutionLayer).getDeclaredField("helper")
			f.setAccessible(True)

			Dim r As New Random(12345)
			For Each afn As Activation In activFns
				For Each doLearningFirst As Boolean In characteristic
					For Each minibatchSize As Integer In minibatchSizes

						Dim input As INDArray = Nd4j.rand(New Integer() {minibatchSize, inputDepth, height, width})
						Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
						For i As Integer = 0 To minibatchSize - 1
							labels.putScalar(i, r.Next(nOut), 1.0)
						Next i

						Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).dist(New UniformDistribution(-1, 1)).updater(New NoOp()).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).padding(1, 1).nOut(3).activation(afn).build()).layer(1, (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).padding(0, 0).nOut(3).activation(afn).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, inputDepth))

						Dim conf As MultiLayerConfiguration = builder.build()

						Dim mln As New MultiLayerNetwork(conf)
						mln.init()

						Dim c0 As org.deeplearning4j.nn.layers.convolution.ConvolutionLayer = DirectCast(mln.getLayer(0), org.deeplearning4j.nn.layers.convolution.ConvolutionLayer)
						Dim ch0 As ConvolutionHelper = DirectCast(f.get(c0), ConvolutionHelper)
						assertTrue(TypeOf ch0 Is CudnnConvolutionHelper)

						Dim c1 As org.deeplearning4j.nn.layers.convolution.ConvolutionLayer = DirectCast(mln.getLayer(1), org.deeplearning4j.nn.layers.convolution.ConvolutionLayer)
						Dim ch1 As ConvolutionHelper = DirectCast(f.get(c1), ConvolutionHelper)
						assertTrue(TypeOf ch1 Is CudnnConvolutionHelper)

						'-------------------------------
						'For debugging/comparison to no-cudnn case: set helper field to null
						'                    f.set(c0, null);
						'                    f.set(c1, null);
						'                    assertNull(f.get(c0));
						'                    assertNull(f.get(c1));
						'-------------------------------


						Dim name As String = (New ObjectAnonymousInnerClass(Me)).GetType().getEnclosingMethod().getName()

						If doLearningFirst Then
							'Run a number of iterations of learning
							mln.Input = input
							mln.Labels = labels
							mln.computeGradientAndScore()
							Dim scoreBefore As Double = mln.score()
							For j As Integer = 0 To 9
								mln.fit(input, labels)
							Next j
							mln.computeGradientAndScore()
							Dim scoreAfter As Double = mln.score()
							'Can't test in 'characteristic mode of operation' if not learning
							Dim msg As String = name & " - score did not (sufficiently) decrease during learning - activationFn=" & afn & ", doLearningFirst= " & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
							assertTrue(scoreAfter < 0.8 * scoreBefore, msg)
						End If

						If PRINT_RESULTS Then
							Console.WriteLine(name & " - activationFn=" & afn & ", doLearningFirst=" & doLearningFirst)
							Dim j As Integer = 0
							Do While j < mln.getnLayers()
								Console.WriteLine("Layer " & j & " # params: " & mln.getLayer(j).numParams())
								j += 1
							Loop
						End If

						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

						assertTrue(gradOK)
					Next minibatchSize
				Next doLearningFirst
			Next afn
		End Sub

		Private Class ObjectAnonymousInnerClass
			Inherits Object

			Private ReadOnly outerInstance As CuDNNGradientChecks

			Public Sub New(ByVal outerInstance As CuDNNGradientChecks)
				Me.outerInstance = outerInstance
			End Sub

		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConvolutionalNoBias() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConvolutionalNoBias()
			Dim minibatchSizes() As Integer = {1, 4}
			Dim width As Integer = 6
			Dim height As Integer = 6
			Dim inputDepth As Integer = 2
			Dim nOut As Integer = 3

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.convolution.ConvolutionLayer).getDeclaredField("helper")
			f.setAccessible(True)

			Dim r As New Random(12345)
			For Each minibatchSize As Integer In minibatchSizes
				For Each convHasBias As Boolean In New Boolean(){True, False}

					Dim input As INDArray = Nd4j.rand(New Integer(){minibatchSize, inputDepth, height, width})
					Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
					For i As Integer = 0 To minibatchSize - 1
						labels.putScalar(i, r.Next(nOut), 1.0)
					Next i

					Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).dist(New UniformDistribution(-1, 1)).updater(New NoOp()).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).padding(1, 1).nOut(3).hasBias(convHasBias).activation(Activation.TANH).build()).layer(1, (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).padding(0, 0).nOut(3).hasBias(convHasBias).activation(Activation.TANH).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(height, width, inputDepth))

					Dim conf As MultiLayerConfiguration = builder.build()

					Dim mln As New MultiLayerNetwork(conf)
					mln.init()

					Dim c0 As org.deeplearning4j.nn.layers.convolution.ConvolutionLayer = DirectCast(mln.getLayer(0), org.deeplearning4j.nn.layers.convolution.ConvolutionLayer)
					Dim ch0 As ConvolutionHelper = DirectCast(f.get(c0), ConvolutionHelper)
					assertTrue(TypeOf ch0 Is CudnnConvolutionHelper)

					Dim c1 As org.deeplearning4j.nn.layers.convolution.ConvolutionLayer = DirectCast(mln.getLayer(1), org.deeplearning4j.nn.layers.convolution.ConvolutionLayer)
					Dim ch1 As ConvolutionHelper = DirectCast(f.get(c1), ConvolutionHelper)
					assertTrue(TypeOf ch1 Is CudnnConvolutionHelper)


					Dim name As String = (New ObjectAnonymousInnerClass2(Me)).GetType().getEnclosingMethod().getName() & ", minibatch = " & minibatchSize & ", convHasBias = " & convHasBias

					If PRINT_RESULTS Then
						Console.WriteLine(name)
						Dim j As Integer = 0
						Do While j < mln.getnLayers()
							Console.WriteLine("Layer " & j & " # params: " & mln.getLayer(j).numParams())
							j += 1
						Loop
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

					assertTrue(gradOK, name)
				Next convHasBias
			Next minibatchSize
		End Sub

		Private Class ObjectAnonymousInnerClass2
			Inherits Object

			Private ReadOnly outerInstance As CuDNNGradientChecks

			Public Sub New(ByVal outerInstance As CuDNNGradientChecks)
				Me.outerInstance = outerInstance
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBatchNormCnn() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBatchNormCnn()
			'Note: CuDNN batch norm supports 4d only, as per 5.1 (according to api reference documentation)
			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim depth As Integer = 1
			Dim hw As Integer = 4
			Dim nOut As Integer = 4
			Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, depth, hw, hw})
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nOut), 1.0)
			Next i

			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).nIn(depth).nOut(2).activation(Activation.IDENTITY).build()).layer(1, (New BatchNormalization.Builder()).build()).layer(2, (New ActivationLayer.Builder()).activation(Activation.TANH).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(hw, hw, depth))

			Dim mln As New MultiLayerNetwork(builder.build())
			mln.init()

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.normalization.BatchNormalization).getDeclaredField("helper")
			f.setAccessible(True)

			Dim b As org.deeplearning4j.nn.layers.normalization.BatchNormalization = DirectCast(mln.getLayer(1), org.deeplearning4j.nn.layers.normalization.BatchNormalization)
			Dim bn As BatchNormalizationHelper = DirectCast(f.get(b), BatchNormalizationHelper)
			assertTrue(TypeOf bn Is CudnnBatchNormalizationHelper)

			'-------------------------------
			'For debugging/comparison to no-cudnn case: set helper field to null
			'        f.set(b, null);
			'        assertNull(f.get(b));
			'-------------------------------

			If PRINT_RESULTS Then
				Dim j As Integer = 0
				Do While j < mln.getnLayers()
					Console.WriteLine("Layer " & j & " # params: " & mln.getLayer(j).numParams())
					j += 1
				Loop
			End If

			'Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
			'i.e., runningMean = decay * runningMean + (1-decay) * batchMean
			'However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
			Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "1_log10stdev"))
			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels, Nothing, Nothing, False, -1, excludeParams, Nothing)

			assertTrue(gradOK)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLRN() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLRN()

			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim depth As Integer = 6
			Dim hw As Integer = 5
			Dim nOut As Integer = 4
			Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, depth, hw, hw})
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nOut), 1.0)
			Next i

			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New ConvolutionLayer.Builder()).nOut(6).kernelSize(2, 2).stride(1, 1).activation(Activation.TANH).build()).layer(1, (New LocalResponseNormalization.Builder()).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(hw, hw, depth))

			Dim mln As New MultiLayerNetwork(builder.build())
			mln.init()

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization).getDeclaredField("helper")
			f.setAccessible(True)

			Dim l As org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization = DirectCast(mln.getLayer(1), org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization)
			Dim lrn As LocalResponseNormalizationHelper = DirectCast(f.get(l), LocalResponseNormalizationHelper)
			assertTrue(TypeOf lrn Is CudnnLocalResponseNormalizationHelper)

			'-------------------------------
			'For debugging/comparison to no-cudnn case: set helper field to null
			'        f.set(l, null);
			'        assertNull(f.get(l));
			'-------------------------------

			If PRINT_RESULTS Then
				Dim j As Integer = 0
				Do While j < mln.getnLayers()
					Console.WriteLine("Layer " & j & " # params: " & mln.getLayer(j).numParams())
					j += 1
				Loop
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

			assertTrue(gradOK)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTM() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLSTM()

			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 4
			Dim inputSize As Integer = 3
			Dim lstmLayerSize As Integer = 4
			Dim timeSeriesLength As Integer = 3
			Dim nOut As Integer = 4
			Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, inputSize, timeSeriesLength})
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut, timeSeriesLength)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				For j As Integer = 0 To timeSeriesLength - 1
					labels.putScalar(i, r.Next(nOut), j, 1.0)
				Next j
			Next i

			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New LSTM.Builder()).nIn(input.size(1)).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(1, (New LSTM.Builder()).nIn(lstmLayerSize).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(lstmLayerSize).nOut(nOut).build())

			Dim mln As New MultiLayerNetwork(builder.build())
			mln.init()

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.recurrent.LSTM).getDeclaredField("helper")
			f.setAccessible(True)

			Dim l As org.deeplearning4j.nn.layers.recurrent.LSTM = DirectCast(mln.getLayer(1), org.deeplearning4j.nn.layers.recurrent.LSTM)
			Dim helper As LSTMHelper = DirectCast(f.get(l), LSTMHelper)
			assertTrue(TypeOf helper Is CudnnLSTMHelper)

			'-------------------------------
			'For debugging/comparison to no-cudnn case: set helper field to null
			'        f.set(l, null);
			'        assertNull(f.get(l));
			'-------------------------------

			If PRINT_RESULTS Then
				Dim j As Integer = 0
				Do While j < mln.getnLayers()
					Console.WriteLine("Layer " & j & " # params: " & mln.getLayer(j).numParams())
					j += 1
				Loop
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels, Nothing, Nothing, True, 32, Nothing, Nothing)

			assertTrue(gradOK)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTM2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLSTM2()

			Nd4j.Random.setSeed(12345)
			Dim minibatch As Integer = 10
			Dim inputSize As Integer = 3
			Dim lstmLayerSize As Integer = 4
			Dim timeSeriesLength As Integer = 3
			Dim nOut As Integer = 2
			Dim input As INDArray = Nd4j.rand(New Integer() {minibatch, inputSize, timeSeriesLength})
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut, timeSeriesLength)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				For j As Integer = 0 To timeSeriesLength - 1
					labels.putScalar(i, r.Next(nOut), j, 1.0)
				Next j
			Next i

			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).seed(12345L).dist(New NormalDistribution(0, 2)).list().layer(0, (New LSTM.Builder()).nIn(input.size(1)).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(1, (New LSTM.Builder()).nIn(lstmLayerSize).nOut(lstmLayerSize).gateActivationFunction(Activation.SIGMOID).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(lstmLayerSize).nOut(nOut).build())

			Dim mln As New MultiLayerNetwork(builder.build())
			mln.init()

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.recurrent.LSTM).getDeclaredField("helper")
			f.setAccessible(True)

			Dim l As org.deeplearning4j.nn.layers.recurrent.LSTM = DirectCast(mln.getLayer(1), org.deeplearning4j.nn.layers.recurrent.LSTM)
			Dim helper As LSTMHelper = DirectCast(f.get(l), LSTMHelper)
			assertTrue(TypeOf helper Is CudnnLSTMHelper)

			'-------------------------------
			'For debugging/comparison to no-cudnn case: set helper field to null
			'        f.set(l, null);
			'        assertNull(f.get(l));
			'-------------------------------

			If PRINT_RESULTS Then
				Dim j As Integer = 0
				Do While j < mln.getnLayers()
					Console.WriteLine("Layer " & j & " # params: " & mln.getLayer(j).numParams())
					j += 1
				Loop
			End If

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

			assertTrue(gradOK)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnDilated() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnnDilated()
			Dim nOut As Integer = 2

			Dim minibatchSize As Integer = 3
			Dim width As Integer = 8
			Dim height As Integer = 8
			Dim inputDepth As Integer = 3


			Nd4j.Random.setSeed(12345)

			Dim f As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.convolution.ConvolutionLayer).getDeclaredField("helper")
			f.setAccessible(True)

			Dim f2 As System.Reflection.FieldInfo = GetType(org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer).getDeclaredField("helper")
			f2.setAccessible(True)

			Dim kernelSizes() As Integer = {2, 3, 2}
			Dim strides() As Integer = {1, 2, 2}
			Dim dilation() As Integer = {2, 3, 2}
			Dim cModes() As ConvolutionMode = {ConvolutionMode.Truncate, ConvolutionMode.Same, ConvolutionMode.Truncate}

			For Each subsampling As Boolean In New Boolean(){False, True}
				For t As Integer = 0 To kernelSizes.Length - 1
					Dim k As Integer = kernelSizes(t)
					Dim s As Integer = strides(t)
					Dim d As Integer = dilation(t)
					Dim cm As ConvolutionMode = cModes(t)

					'Use larger input with larger dilation values (to avoid invalid config)
					Dim w As Integer = d * width
					Dim h As Integer = d * height

					Dim input As INDArray = Nd4j.rand(minibatchSize, w * h * inputDepth)
					Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
					For i As Integer = 0 To minibatchSize - 1
						labels.putScalar(New Integer(){i, i Mod nOut}, 1.0)
					Next i

					Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).updater(New NoOp()).activation(Activation.TANH).convolutionMode(cm).list().layer((New ConvolutionLayer.Builder()).name("layer 0").kernelSize(k, k).stride(s, s).dilation(d, d).nIn(inputDepth).nOut(2).build())
					If subsampling Then
						b.layer((New SubsamplingLayer.Builder()).poolingType(SubsamplingLayer.PoolingType.MAX).kernelSize(k, k).stride(s, s).dilation(d, d).build())
					Else
						b.layer((New ConvolutionLayer.Builder()).nIn(2).nOut(2).kernelSize(k, k).stride(s, s).dilation(d, d).build())
					End If

					Dim conf As MultiLayerConfiguration = b.layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutionalFlat(h, w, inputDepth)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim c0 As org.deeplearning4j.nn.layers.convolution.ConvolutionLayer = DirectCast(net.getLayer(0), org.deeplearning4j.nn.layers.convolution.ConvolutionLayer)
					Dim ch0 As ConvolutionHelper = DirectCast(f.get(c0), ConvolutionHelper)
					assertTrue(TypeOf ch0 Is CudnnConvolutionHelper)

					If subsampling Then
						Dim s1 As org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer = DirectCast(net.getLayer(1), org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer)
						Dim sh1 As SubsamplingHelper = DirectCast(f2.get(s1), SubsamplingHelper)
						assertTrue(TypeOf sh1 Is SubsamplingHelper)
					Else
						Dim c1 As org.deeplearning4j.nn.layers.convolution.ConvolutionLayer = DirectCast(net.getLayer(1), org.deeplearning4j.nn.layers.convolution.ConvolutionLayer)
						Dim ch1 As ConvolutionHelper = DirectCast(f.get(c1), ConvolutionHelper)
						assertTrue(TypeOf ch1 Is CudnnConvolutionHelper)
					End If

					Dim i As Integer = 0
					Do While i < net.Layers.Length
						Console.WriteLine("nParams, layer " & i & ": " & net.getLayer(i).numParams())
						i += 1
					Loop

					Dim msg As String = (If(subsampling, "subsampling", "conv")) & " - mb=" & minibatchSize & ", k=" & k & ", s=" & s & ", d=" & d & ", cm=" & cm
					Console.WriteLine(msg)

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

					assertTrue(gradOK, msg)
				Next t
			Next subsampling
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDropout()
		Public Overridable Sub testDropout()
			Dim minibatch As Integer = 2

			For Each cnn As Boolean In New Boolean(){False, True}
				Nd4j.Random.setSeed(12345)
				Dim dropout As IDropout = New Dropout(0.6)

				Dim builder As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1)).convolutionMode(ConvolutionMode.Same).dropOut(dropout).activation(Activation.TANH).updater(New NoOp()).list()

				If cnn Then
					builder.layer((New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(2, 2).nOut(2).build())
					builder.layer((New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(2, 2).nOut(2).build())
					builder.InputType = InputType.convolutional(8, 8, 2)
				Else
					builder.layer((New DenseLayer.Builder()).nOut(8).build())
					builder.layer((New DenseLayer.Builder()).nOut(8).build())
					builder.InputType = InputType.feedForward(6)
				End If
				builder.layer((New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build())
				Dim conf As MultiLayerConfiguration = builder.build()

				Dim mln As New MultiLayerNetwork(conf)
				mln.init()

				Dim f As INDArray
				If cnn Then
					f = Nd4j.rand(New Integer(){minibatch, 2, 8, 8}).muli(10).subi(5)
				Else
					f = Nd4j.rand(minibatch, 6).muli(10).subi(5)
				End If
				Dim l As INDArray = TestUtils.randomOneHot(minibatch, 3)

				mln.output(f, True)

				For Each layer As Layer In mln.Layers
					Dim d As Dropout = CType(layer.conf().getLayer().getIDropout(), Dropout)
					assertNotNull(d)
					Dim h As CudnnDropoutHelper = CType(d.getHelper(), CudnnDropoutHelper)
					assertNotNull(h)
				Next layer

				Dim msg As String = (If(cnn, "CNN", "Dense")) & ": " & dropout.GetType().Name

				'Consumer function to enforce CuDNN RNG repeatability - otherwise will fail due to randomness (inconsistent
				' dropout mask between forward passes)
				Dim c As Consumer(Of MultiLayerNetwork) = New ConsumerAnonymousInnerClass(Me)

				log.info("*** Starting test: " & msg & " ***")
				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).epsilon(DEFAULT_EPS).maxRelError(DEFAULT_MAX_REL_ERROR).minAbsoluteError(DEFAULT_MIN_ABS_ERROR).print(If(PRINT_RESULTS, GradientCheckUtil.PrintMode.ZEROS, GradientCheckUtil.PrintMode.FAILURES_ONLY)).exitOnFirstError(RETURN_ON_FIRST_FAILURE).input(f).labels(l).callEachIter(c))

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(mln)
			Next cnn
		End Sub

		Private Class ConsumerAnonymousInnerClass
			Implements Consumer(Of MultiLayerNetwork)

			Private ReadOnly outerInstance As CuDNNGradientChecks

			Public Sub New(ByVal outerInstance As CuDNNGradientChecks)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub accept(ByVal net As MultiLayerNetwork)
				Nd4j.Random.setSeed(12345)
				For Each l As Layer In net.Layers
					Dim d As Dropout = CType(l.conf().getLayer().getIDropout(), Dropout)
					If d IsNot Nothing Then
						CType(d.getHelper(), CudnnDropoutHelper).setMask(Nothing)
						CType(d.getHelper(), CudnnDropoutHelper).setRngStates(Nothing)
					End If
				Next l
			End Sub
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDenseBatchNorm()
		Public Overridable Sub testDenseBatchNorm()


			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).weightInit(WeightInit.XAVIER).updater(New NoOp()).list().layer((New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer((New BatchNormalization.Builder()).nOut(5).build()).layer((New OutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim [in] As INDArray = Nd4j.rand(3, 5)
			Dim labels As INDArray = TestUtils.randomOneHot(3, 5)

			'Mean and variance vars are not gradient checkable; mean/variance "gradient" is used to implement running mean/variance calc
			'i.e., runningMean = decay * runningMean + (1-decay) * batchMean
			'However, numerical gradient will be 0 as forward pass doesn't depend on this "parameter"
			Dim excludeParams As ISet(Of String) = New HashSet(Of String)(Arrays.asList("1_mean", "1_var", "1_log10stdev"))
			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, [in], labels, Nothing, Nothing, False, -1, excludeParams, Nothing)

			assertTrue(gradOK)

			TestUtils.testModelSerialization(net)
		End Sub
	End Class

End Namespace