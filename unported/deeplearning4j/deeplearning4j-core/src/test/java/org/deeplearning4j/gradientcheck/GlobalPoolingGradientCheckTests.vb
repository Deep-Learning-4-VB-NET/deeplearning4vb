Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class GlobalPoolingGradientCheckTests extends org.deeplearning4j.BaseDL4JTest
	Public Class GlobalPoolingGradientCheckTests
		Inherits BaseDL4JTest

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRNNGlobalPoolingBasicMultiLayer()
		Public Overridable Sub testRNNGlobalPoolingBasicMultiLayer()
			'Basic test of global pooling w/ LSTM
			Nd4j.Random.Seed = 12345L

			Dim timeSeriesLength As Integer = 5
			Dim nIn As Integer = 5
			Dim layerSize As Integer = 4
			Dim nOut As Integer = 2

			Dim minibatchSizes() As Integer = {1, 3}
			Dim poolingTypes() As PoolingType = {PoolingType.AVG, PoolingType.SUM, PoolingType.MAX, PoolingType.PNORM}

			For Each miniBatchSize As Integer In minibatchSizes
				For Each pt As PoolingType In poolingTypes

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1.0)).seed(12345L).list().layer(0, (New SimpleRnn.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).build()).build()

					Dim mln As New MultiLayerNetwork(conf)
					mln.init()

					Dim r As New Random(12345L)
					Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, miniBatchSize, nIn, timeSeriesLength).subi(0.5)

					Dim labels As INDArray = TestUtils.randomOneHot(miniBatchSize, nOut).castTo(DataType.DOUBLE)

					If PRINT_RESULTS Then
						Console.WriteLine("testLSTMGlobalPoolingBasicMultiLayer() - " & pt & ", minibatch = " & miniBatchSize)
	'                    for (int j = 0; j < mln.getnLayers(); j++)
	'                        System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

					assertTrue(gradOK)
					TestUtils.testModelSerialization(mln)
				Next pt
			Next miniBatchSize
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnGlobalPoolingBasicMultiLayer()
		Public Overridable Sub testCnnGlobalPoolingBasicMultiLayer()
			'Basic test of global pooling w/ CNN
			Nd4j.Random.Seed = 12345L

			For Each nchw As Boolean In New Boolean(){True, False}

				Dim inputDepth As Integer = 3
				Dim inputH As Integer = 5
				Dim inputW As Integer = 4
				Dim layerDepth As Integer = 4
				Dim nOut As Integer = 2

				Dim minibatchSizes() As Integer = {1, 3}
				Dim poolingTypes() As PoolingType = {PoolingType.AVG, PoolingType.SUM, PoolingType.MAX, PoolingType.PNORM}

				For Each miniBatchSize As Integer In minibatchSizes
					For Each pt As PoolingType In poolingTypes

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1.0)).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).dataFormat(If(nchw, CNN2DFormat.NCHW, CNN2DFormat.NHWC)).nOut(layerDepth).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(inputH, inputW, inputDepth,If(nchw, CNN2DFormat.NCHW, CNN2DFormat.NHWC))).build()

						Dim mln As New MultiLayerNetwork(conf)
						mln.init()

						Dim r As New Random(12345L)
						Dim inShape() As Long = If(nchw, New Long(){miniBatchSize, inputDepth, inputH, inputW}, New Long()){miniBatchSize, inputH, inputW, inputDepth}
						Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, inShape).subi(0.5)

						Dim labels As INDArray = Nd4j.zeros(miniBatchSize, nOut)
						For i As Integer = 0 To miniBatchSize - 1
							Dim idx As Integer = r.Next(nOut)
							labels.putScalar(i, idx, 1.0)
						Next i

						If PRINT_RESULTS Then
							Console.WriteLine("testCnnGlobalPoolingBasicMultiLayer() - " & pt & ", minibatch = " & miniBatchSize & " - " & (If(nchw, "NCHW", "NHWC")))
	'                    for (int j = 0; j < mln.getnLayers(); j++)
	'                        System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
						End If

						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

						assertTrue(gradOK)
						TestUtils.testModelSerialization(mln)
					Next pt
				Next miniBatchSize
			Next nchw
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTMWithMasking()
		Public Overridable Sub testLSTMWithMasking()
			'Basic test of LSTM layer
			Nd4j.Random.Seed = 12345L

			Dim timeSeriesLength As Integer = 5
			Dim nIn As Integer = 4
			Dim layerSize As Integer = 3
			Dim nOut As Integer = 2

			Dim miniBatchSize As Integer = 3
			Dim poolingTypes() As PoolingType = {PoolingType.AVG, PoolingType.SUM, PoolingType.MAX, PoolingType.PNORM}

			For Each pt As PoolingType In poolingTypes

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1.0)).seed(12345L).list().layer(0, (New LSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).build()).build()

				Dim mln As New MultiLayerNetwork(conf)
				mln.init()

				Dim r As New Random(12345L)
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, miniBatchSize, nIn, timeSeriesLength).subi(0.5)

				Dim featuresMask As INDArray = Nd4j.create(miniBatchSize, timeSeriesLength)
				For i As Integer = 0 To miniBatchSize - 1
					Dim [to] As Integer = timeSeriesLength - i
					For j As Integer = 0 To [to] - 1
						featuresMask.putScalar(i, j, 1.0)
					Next j
				Next i

				Dim labels As INDArray = TestUtils.randomOneHot(miniBatchSize, nOut)
				mln.setLayerMaskArrays(featuresMask, Nothing)

				If PRINT_RESULTS Then
					Console.WriteLine("testLSTMGlobalPoolingBasicMultiLayer() - " & pt & ", minibatch = " & miniBatchSize)
	'                for (int j = 0; j < mln.getnLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).inputMask(featuresMask))

				assertTrue(gradOK)
				TestUtils.testModelSerialization(mln)
			Next pt
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnGlobalPoolingMasking()
		Public Overridable Sub testCnnGlobalPoolingMasking()
			'Global pooling w/ CNN + masking, where mask is along dimension 2, then separately test along dimension 3
			Nd4j.Random.Seed = 12345L

			Dim inputDepth As Integer = 2
			Dim inputH As Integer = 5
			Dim inputW As Integer = 5
			Dim layerDepth As Integer = 3
			Dim nOut As Integer = 2

			For maskDim As Integer = 2 To 3

				Dim minibatchSizes() As Integer = {1, 3}
				Dim poolingTypes() As PoolingType = {PoolingType.AVG, PoolingType.SUM, PoolingType.MAX, PoolingType.PNORM}

				For Each miniBatchSize As Integer In minibatchSizes
					For Each pt As PoolingType In poolingTypes

						Dim kernel() As Integer
						Dim stride() As Integer
						If maskDim = 2 Then
							'"time" (variable length) dimension is dimension 2
							kernel = New Integer() {2, inputW}
							stride = New Integer() {1, inputW}
						Else
							kernel = New Integer() {inputH, 2}
							stride = New Integer() {inputH, 1}
						End If

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).dist(New NormalDistribution(0, 1.0)).convolutionMode(ConvolutionMode.Same).seed(12345L).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(kernel).stride(stride).nOut(layerDepth).build()).layer(1, (New GlobalPoolingLayer.Builder()).poolingType(pt).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(nOut).build()).setInputType(InputType.convolutional(inputH, inputW, inputDepth)).build()

						Dim mln As New MultiLayerNetwork(conf)
						mln.init()

						Dim r As New Random(12345L)
						Dim input As INDArray = Nd4j.rand(New Integer() {miniBatchSize, inputDepth, inputH, inputW}).subi(0.5)

						Dim inputMask As INDArray
						If miniBatchSize = 1 Then
							inputMask = Nd4j.create(New Double() {1, 1, 1, 1, 0}).reshape(ChrW(1), 1, (If(maskDim = 2, inputH, 1)), (If(maskDim = 3, inputW, 1)))
						ElseIf miniBatchSize = 3 Then
							inputMask = Nd4j.create(New Double()() {
								New Double() {1, 1, 1, 1, 1},
								New Double() {1, 1, 1, 1, 0},
								New Double() {1, 1, 1, 0, 0}
							}).reshape(ChrW(miniBatchSize), 1, (If(maskDim = 2, inputH, 1)), (If(maskDim = 3, inputW, 1)))
						Else
							Throw New Exception()
						End If


						Dim labels As INDArray = Nd4j.zeros(miniBatchSize, nOut)
						For i As Integer = 0 To miniBatchSize - 1
							Dim idx As Integer = r.Next(nOut)
							labels.putScalar(i, idx, 1.0)
						Next i

						If PRINT_RESULTS Then
							Console.WriteLine("testCnnGlobalPoolingBasicMultiLayer() - " & pt & ", minibatch = " & miniBatchSize)
	'                        for (int j = 0; j < mln.getnLayers(); j++)
	'                            System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
						End If

						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).inputMask(inputMask))

						assertTrue(gradOK)
						TestUtils.testModelSerialization(mln)
					Next pt
				Next miniBatchSize
			Next maskDim
		End Sub
	End Class

End Namespace