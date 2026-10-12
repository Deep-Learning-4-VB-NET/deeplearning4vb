Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports org.deeplearning4j.nn.conf.layers
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
Imports LossSparseMCXENT = org.nd4j.linalg.lossfunctions.impl.LossSparseMCXENT
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class OutputLayerGradientChecks extends org.deeplearning4j.BaseDL4JTest
	Public Class OutputLayerGradientChecks
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRnnLossLayer()
		Public Overridable Sub testRnnLossLayer()
			Nd4j.Random.Seed = 12345L

			Dim timeSeriesLength As Integer = 4
			Dim nIn As Integer = 2
			Dim layerSize As Integer = 2
			Dim nOut As Integer = 2
			Dim miniBatchSize As Integer = 3

			Dim lfs() As ILossFunction = {
				New LossMSE(),
				New LossMCXENT(),
				New LossSparseMCXENT()
			}

			For maskType As Integer = 0 To 2

				Dim r As New Random(12345L)
				Dim input As INDArray = Nd4j.rand(New Integer(){miniBatchSize, nIn, timeSeriesLength})

				Dim labelMask As INDArray
				Dim mt As String
				Select Case maskType
					Case 0
						'No masking
						labelMask = Nothing
						mt = "none"
					Case 1
						'Per time step masking
						labelMask = Nd4j.createUninitialized(DataType.DOUBLE, miniBatchSize, timeSeriesLength)
						Nd4j.Executioner.exec(New BernoulliDistribution(labelMask, 0.5))
						mt = "PerTimeStep"
					Case 2
						'Per output masking:
						labelMask = Nd4j.createUninitialized(DataType.DOUBLE, miniBatchSize, nOut, timeSeriesLength)
						Nd4j.Executioner.exec(New BernoulliDistribution(labelMask, 0.5))
						mt = "PerOutput"
					Case Else
						Throw New Exception()
				End Select

				For Each lf As ILossFunction In lfs

					Dim labels As INDArray
					If TypeOf lf Is LossSparseMCXENT Then
						labels = Nd4j.zeros(miniBatchSize, 1, timeSeriesLength)
						For i As Integer = 0 To miniBatchSize - 1
							For j As Integer = 0 To timeSeriesLength - 1
								Dim idx As Integer = r.Next(nOut)
								labels.putScalar(New Integer(){i, 0, j}, idx)
							Next j
						Next i
					Else
						labels = Nd4j.zeros(miniBatchSize, nOut, timeSeriesLength)
						For i As Integer = 0 To miniBatchSize - 1
							For j As Integer = 0 To timeSeriesLength - 1
								Dim idx As Integer = r.Next(nOut)
								labels.putScalar(New Integer(){i, idx, j}, 1.0)
							Next j
						Next i
					End If


					Dim oa As Activation = If(maskType = 2, Activation.SIGMOID, Activation.SOFTMAX)

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).dataType(DataType.DOUBLE).updater(New NoOp()).list().layer((New LSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()).layer((New RnnLossLayer.Builder(lf)).activation(oa).build()).validateOutputLayerConfig(False).build()

					Dim mln As New MultiLayerNetwork(conf)
					mln.init()

					Dim testName As String = "testRnnLossLayer(lf=" & lf & ", maskType=" & mt & ", outputActivation = " & oa & ")"
					If PRINT_RESULTS Then
						Console.WriteLine(testName)
	'                    for (int j = 0; j < mln.getnLayers(); j++)
	'                        System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
					End If

					Console.WriteLine("Starting test: " & testName)
					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).labelMask(labelMask))

					assertTrue(gradOK, testName)
					TestUtils.testModelSerialization(mln)
				Next lf
			Next maskType
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnLossLayer()
		Public Overridable Sub testCnnLossLayer()
			Nd4j.Random.Seed = 12345L

			Dim dIn As Integer = 2
			Dim layerSize As Integer = 2
			Dim dOut As Integer = 2
			Dim miniBatchSize As Integer = 3

			Dim lfs() As ILossFunction = {
				New LossMSE(),
				New LossMCXENT()
			}

			Dim heights() As Integer = {4, 4, 5}
			Dim widths() As Integer = {4, 5, 6}

			For i As Integer = 0 To heights.Length - 1
				Dim h As Integer = heights(i)
				Dim w As Integer = widths(i)

				For maskType As Integer = 0 To 3

					Dim r As New Random(12345L)
					Dim input As INDArray = Nd4j.rand(New Integer(){miniBatchSize, dIn, h, w})

					Dim labelMask As INDArray
					Dim mt As String
					Select Case maskType
						Case 0
							'No masking
							labelMask = Nothing
							mt = "none"
						Case 1
							'Per example masking (2d mask, shape [minibatch, 1]
							labelMask = Nd4j.createUninitialized(miniBatchSize, 1)
							Nd4j.Executioner.exec(New BernoulliDistribution(labelMask, 0.5))
							mt = "PerTimeStep"
						Case 2
							'Per x/y masking (3d mask, shape [minibatch, h, w])
							labelMask = Nd4j.createUninitialized(New Integer(){miniBatchSize, h, w})
							Nd4j.Executioner.exec(New BernoulliDistribution(labelMask, 0.5))
							mt = "PerXY"
						Case 3
							'Per output masking (4d mask, same shape as output [minibatch, c, h, w])
							labelMask = Nd4j.createUninitialized(New Integer(){miniBatchSize, dOut, h, w})
							Nd4j.Executioner.exec(New BernoulliDistribution(labelMask, 0.5))
							mt = "PerOutput"
						Case Else
							Throw New Exception()
					End Select

					For Each lf As ILossFunction In lfs

						Dim labels As INDArray
						If TypeOf lf Is LossMSE Then
							labels = Nd4j.rand(New Integer(){miniBatchSize, dOut, h, w})
						Else
							labels = Nd4j.zeros(miniBatchSize, dOut, h, w)
							For mb As Integer = 0 To miniBatchSize - 1
								For x As Integer = 0 To w - 1
									For y As Integer = 0 To h - 1
										Dim idx As Integer = r.Next(dOut)
										labels.putScalar(New Integer(){mb, idx, y, x}, 1.0)
									Next y
								Next x
							Next mb
						End If

						Dim oa As Activation = If(maskType = 3, Activation.SIGMOID, Activation.SOFTMAX)

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).dataType(DataType.DOUBLE).updater(New NoOp()).convolutionMode(ConvolutionMode.Same).list().layer((New ConvolutionLayer.Builder()).nIn(dIn).nOut(dOut).activation(Activation.TANH).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()).layer((New CnnLossLayer.Builder(lf)).activation(oa).build()).validateOutputLayerConfig(False).build()

						Dim mln As New MultiLayerNetwork(conf)
						mln.init()

						Dim testName As String = "testCnnLossLayer(lf=" & lf & ", maskType=" & mt & ", outputActivation = " & oa & ")"
						If PRINT_RESULTS Then
							Console.WriteLine(testName)
	'                        for (int j = 0; j < mln.getnLayers(); j++)
	'                            System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
						End If

						Console.WriteLine("Starting test: " & testName)
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).labelMask(labelMask))

						assertTrue(gradOK, testName)
						TestUtils.testModelSerialization(mln)
					Next lf
				Next maskType
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnn3dLossLayer()
		Public Overridable Sub testCnn3dLossLayer()
			Nd4j.Random.Seed = 12345L

			Dim chIn As Integer = 2
			Dim layerSize As Integer = 2
			Dim chOut As Integer = 2
			Dim miniBatchSize As Integer = 3

			Dim lfs() As ILossFunction = {
				New LossMSE(),
				New LossMCXENT()
			}

			Dim heights() As Integer = {4, 4, 5}
			Dim widths() As Integer = {4, 5, 6}

			For Each dataFormat As Convolution3D.DataFormat In System.Enum.GetValues(GetType(Convolution3D.DataFormat))
				For i As Integer = 0 To heights.Length - 1
					Dim h As Integer = heights(i)
					Dim w As Integer = widths(i)
					Dim d As Integer = h

					For maskType As Integer = 0 To 3

						Dim r As New Random(12345L)
						Dim input As INDArray
						If dataFormat = Convolution3D.DataFormat.NCDHW Then
							input = Nd4j.rand(New Integer(){miniBatchSize, chIn, d, h, w})
						Else
							input = Nd4j.rand(New Integer(){miniBatchSize, d, h, w, chIn})
						End If

						Dim labelMask As INDArray
						Dim mt As String
						Select Case maskType
							Case 0
								'No masking
								labelMask = Nothing
								mt = "none"
							Case 1
								'Per example masking (shape [minibatch, 1, 1, 1, 1]
								labelMask = Nd4j.createUninitialized(New Integer(){miniBatchSize, 1, 1, 1, 1})
								Nd4j.Executioner.exec(New BernoulliDistribution(labelMask, 0.5))
								mt = "PerExample"
							Case 2
								'Per channel masking (5d mask, shape [minibatch, d, 1, 1, 1] or [minibatch, 1, 1, 1, d])
								If dataFormat = Convolution3D.DataFormat.NCDHW Then
									labelMask = Nd4j.createUninitialized(New Integer(){miniBatchSize, chOut, 1, 1, 1})
								Else
									labelMask = Nd4j.createUninitialized(New Integer(){miniBatchSize, 1, 1, 1, chOut})
								End If
								Nd4j.Executioner.exec(New BernoulliDistribution(labelMask, 0.5))
								mt = "PerChannel"
							Case 3
								'Per output masking (5d mask, same shape as output [minibatch, c, h, w])
								If dataFormat = Convolution3D.DataFormat.NCDHW Then
									labelMask = Nd4j.createUninitialized(New Integer(){miniBatchSize, chOut, d, h, w})
								Else
									labelMask = Nd4j.createUninitialized(New Integer(){miniBatchSize, d, h, w, chOut})
								End If
								Nd4j.Executioner.exec(New BernoulliDistribution(labelMask, 0.5))
								mt = "PerOutput"
							Case Else
								Throw New Exception()
						End Select

						For Each lf As ILossFunction In lfs

							If (mt.Equals("PerOutput") OrElse mt.Equals("PerChannel")) AndAlso TypeOf lf Is LossMCXENT Then
								'Per-output masking + MCXENT: not supported
								Continue For
							End If

							Dim labels As INDArray
							If TypeOf lf Is LossMSE Then
								If dataFormat = Convolution3D.DataFormat.NCDHW Then
									labels = Nd4j.rand(New Integer(){miniBatchSize, chOut, d, h, w})
								Else
									labels = Nd4j.rand(New Integer(){miniBatchSize, d, h, w, chOut})
								End If
							Else
								If dataFormat = Convolution3D.DataFormat.NCDHW Then
									labels = Nd4j.zeros(miniBatchSize, chOut, d, h, w)
									For mb As Integer = 0 To miniBatchSize - 1
										For d2 As Integer = 0 To d - 1
											For x As Integer = 0 To w - 1
												For y As Integer = 0 To h - 1
													Dim idx As Integer = r.Next(chOut)
													labels.putScalar(New Integer(){mb, idx, d2, y, x}, 1.0)
												Next y
											Next x
										Next d2
									Next mb
								Else
									labels = Nd4j.zeros(miniBatchSize, d, h, w, chOut)
									For mb As Integer = 0 To miniBatchSize - 1
										For d2 As Integer = 0 To d - 1
											For x As Integer = 0 To w - 1
												For y As Integer = 0 To h - 1
													Dim idx As Integer = r.Next(chOut)
													labels.putScalar(New Integer(){mb, d2, y, x, idx}, 1.0)
												Next y
											Next x
										Next d2
									Next mb
								End If
							End If

							Dim oa As Activation = If(maskType = 1, Activation.SOFTMAX, Activation.SIGMOID)

							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).dataType(DataType.DOUBLE).updater(New NoOp()).convolutionMode(ConvolutionMode.Same).list().layer((New Convolution3D.Builder()).nIn(chIn).nOut(chOut).activation(Activation.TANH).dist(New NormalDistribution(0, 1.0)).dataFormat(dataFormat).updater(New NoOp()).build()).layer((New Cnn3DLossLayer.Builder(dataFormat)).lossFunction(lf).activation(oa).build()).validateOutputLayerConfig(False).build()

							Dim mln As New MultiLayerNetwork(conf)
							mln.init()

							Dim testName As String = "testCnn3dLossLayer(dataFormat=" & dataFormat & ",lf=" & lf & ", maskType=" & mt & ", outputActivation = " & oa & ")"
							If PRINT_RESULTS Then
								Console.WriteLine(testName)
	'                            for (int j = 0; j < mln.getnLayers(); j++)
	'                                System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
							End If

							Console.WriteLine("Starting test: " & testName)
							Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).labelMask(labelMask))

							assertTrue(gradOK, testName)
							TestUtils.testModelSerialization(mln)
						Next lf
					Next maskType
				Next i
			Next dataFormat
		End Sub
	End Class

End Namespace