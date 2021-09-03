Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
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
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
import static org.junit.jupiter.api.Assertions.assertEquals
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class NoBiasGradientCheckTests extends org.deeplearning4j.BaseDL4JTest
	Public Class NoBiasGradientCheckTests
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
'ORIGINAL LINE: @Test public void testGradientNoBiasDenseOutput()
		Public Overridable Sub testGradientNoBiasDenseOutput()

			Dim nIn As Integer = 5
			Dim nOut As Integer = 3
			Dim layerSize As Integer = 6

			For Each minibatch As Integer In New Integer(){1, 4}
				Dim input As INDArray = Nd4j.rand(minibatch, nIn)
				Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
				For i As Integer = 0 To minibatch - 1
					labels.putScalar(i, i Mod nOut, 1.0)
				Next i

				For Each denseHasBias As Boolean In New Boolean(){True, False}
					For Each outHasBias As Boolean In New Boolean(){True, False}

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(layerSize).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).hasBias(True).build()).layer(1, (New DenseLayer.Builder()).nIn(layerSize).nOut(layerSize).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).hasBias(denseHasBias).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).hasBias(outHasBias).build()).build()

						Dim mln As New MultiLayerNetwork(conf)
						mln.init()

						If denseHasBias Then
							assertEquals(layerSize * layerSize + layerSize, mln.getLayer(1).numParams())
						Else
							assertEquals(layerSize * layerSize, mln.getLayer(1).numParams())
						End If

						If outHasBias Then
							assertEquals(layerSize * nOut + nOut, mln.getLayer(2).numParams())
						Else
							assertEquals(layerSize * nOut, mln.getLayer(2).numParams())
						End If

						Dim msg As String = "testGradientNoBiasDenseOutput(), minibatch = " & minibatch & ", denseHasBias = " & denseHasBias & ", outHasBias = " & outHasBias & ")"

						If PRINT_RESULTS Then
							Console.WriteLine(msg)
						End If

						Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
						assertTrue(gradOK, msg)

						TestUtils.testModelSerialization(mln)
					Next outHasBias
				Next denseHasBias
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientNoBiasRnnOutput()
		Public Overridable Sub testGradientNoBiasRnnOutput()

			Dim nIn As Integer = 5
			Dim nOut As Integer = 3
			Dim tsLength As Integer = 3
			Dim layerSize As Integer = 6

			For Each minibatch As Integer In New Integer(){1, 4}
				Dim input As INDArray = Nd4j.rand(New Integer(){minibatch, nIn, tsLength})
				Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(minibatch, nOut, tsLength)

				For Each rnnOutHasBias As Boolean In New Boolean(){True, False}

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).seed(12345L).list().layer(0, (New LSTM.Builder()).nIn(nIn).nOut(layerSize).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).build()).layer(1, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).hasBias(rnnOutHasBias).build()).build()

					Dim mln As New MultiLayerNetwork(conf)
					mln.init()

					If rnnOutHasBias Then
						assertEquals(layerSize * nOut + nOut, mln.getLayer(1).numParams())
					Else
						assertEquals(layerSize * nOut, mln.getLayer(1).numParams())
					End If

					Dim msg As String = "testGradientNoBiasRnnOutput(), minibatch = " & minibatch & ", rnnOutHasBias = " & rnnOutHasBias & ")"

					If PRINT_RESULTS Then
						Console.WriteLine(msg)
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
					assertTrue(gradOK, msg)

					TestUtils.testModelSerialization(mln)
				Next rnnOutHasBias
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientNoBiasEmbedding()
		Public Overridable Sub testGradientNoBiasEmbedding()

			Dim nIn As Integer = 5
			Dim nOut As Integer = 3
			Dim layerSize As Integer = 6

			For Each minibatch As Integer In New Integer(){1, 4}
				Dim input As INDArray = Nd4j.zeros(minibatch, 1)
				For i As Integer = 0 To minibatch - 1
					input.putScalar(i, 0, i Mod layerSize)
				Next i
				Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
				For i As Integer = 0 To minibatch - 1
					labels.putScalar(i, i Mod nOut, 1.0)
				Next i

				For Each embeddingHasBias As Boolean In New Boolean(){True, False}

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).seed(12345L).list().layer(0, (New EmbeddingLayer.Builder()).nIn(nIn).nOut(layerSize).dist(New NormalDistribution(0, 1)).activation(Activation.TANH).hasBias(embeddingHasBias).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).build()).build()

					Dim mln As New MultiLayerNetwork(conf)
					mln.init()

					If embeddingHasBias Then
						assertEquals(nIn * layerSize + layerSize, mln.getLayer(0).numParams())
					Else
						assertEquals(nIn * layerSize, mln.getLayer(0).numParams())
					End If

					Dim msg As String = "testGradientNoBiasEmbedding(), minibatch = " & minibatch & ", embeddingHasBias = " & embeddingHasBias & ")"

					If PRINT_RESULTS Then
						Console.WriteLine(msg)
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
					assertTrue(gradOK, msg)

					TestUtils.testModelSerialization(mln)
				Next embeddingHasBias
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnWithSubsamplingNoBias()
		Public Overridable Sub testCnnWithSubsamplingNoBias()
			Dim nOut As Integer = 4

			Dim minibatchSizes() As Integer = {1, 3}
			Dim width As Integer = 5
			Dim height As Integer = 5
			Dim inputDepth As Integer = 1

			Dim kernel() As Integer = {2, 2}
			Dim stride() As Integer = {1, 1}
			Dim padding() As Integer = {0, 0}
			Dim pNorm As Integer = 3

			For Each minibatchSize As Integer In minibatchSizes
				Dim input As INDArray = Nd4j.rand(minibatchSize, width * height * inputDepth)
				Dim labels As INDArray = Nd4j.zeros(minibatchSize, nOut)
				For i As Integer = 0 To minibatchSize - 1
					labels.putScalar(New Integer(){i, i Mod nOut}, 1.0)
				Next i

				For Each cnnHasBias As Boolean In New Boolean(){True, False}

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1)).list().layer((New ConvolutionLayer.Builder(kernel, stride, padding)).nIn(inputDepth).hasBias(False).nOut(3).build()).layer((New SubsamplingLayer.Builder(PoolingType.MAX)).kernelSize(kernel).stride(stride).padding(padding).pnorm(pNorm).build()).layer((New ConvolutionLayer.Builder(kernel, stride, padding)).hasBias(cnnHasBias).nOut(2).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(4).build()).setInputType(InputType.convolutionalFlat(height, width, inputDepth)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					If cnnHasBias Then
						assertEquals(3 * 2 * kernel(0) * kernel(1) + 2, net.getLayer(2).numParams())
					Else
						assertEquals(3 * 2 * kernel(0) * kernel(1), net.getLayer(2).numParams())
					End If

					Dim msg As String = "testCnnWithSubsamplingNoBias(), minibatch = " & minibatchSize & ", cnnHasBias = " & cnnHasBias
					Console.WriteLine(msg)

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

					assertTrue(gradOK, msg)

					TestUtils.testModelSerialization(net)
				Next cnnHasBias
			Next minibatchSize
		End Sub

	End Class

End Namespace