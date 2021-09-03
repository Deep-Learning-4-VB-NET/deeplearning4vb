Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports TimeDistributed = org.deeplearning4j.nn.conf.layers.recurrent.TimeDistributed
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class RnnGradientChecks extends org.deeplearning4j.BaseDL4JTest
	Public Class RnnGradientChecks
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
'ORIGINAL LINE: @Test @Disabled("AB 2019/06/24 - Ignored to get to all passing baseline to prevent regressions via CI - see issue #7912") public void testBidirectionalWrapper()
		Public Overridable Sub testBidirectionalWrapper()

			Dim nIn As Integer = 3
			Dim nOut As Integer = 5
			Dim tsLength As Integer = 4

			Dim modes() As Bidirectional.Mode = {Bidirectional.Mode.CONCAT, Bidirectional.Mode.ADD, Bidirectional.Mode.AVERAGE, Bidirectional.Mode.MUL}

			Dim r As New Random(12345)
			For Each mb As Integer In New Integer(){1, 3}
				For Each inputMask As Boolean In New Boolean(){False, True}
					For Each simple As Boolean In New Boolean(){False, True}
						For Each hasLayerNorm As Boolean In New Boolean(){True, False}
							If Not simple AndAlso hasLayerNorm Then
								Continue For
							End If

							Dim [in] As INDArray = Nd4j.rand(New Integer(){mb, nIn, tsLength})
							Dim labels As INDArray = Nd4j.create(mb, nOut, tsLength)
							For i As Integer = 0 To mb - 1
								For j As Integer = 0 To tsLength - 1
									labels.putScalar(i, r.Next(nOut), j, 1.0)
								Next j
							Next i
							Dim maskType As String = (If(inputMask, "inputMask", "none"))

							Dim inMask As INDArray = Nothing
							If inputMask Then
								inMask = Nd4j.ones(mb, tsLength)
								For i As Integer = 0 To mb - 1
									Dim firstMaskedStep As Integer = tsLength - 1 - i
									If firstMaskedStep = 0 Then
										firstMaskedStep = tsLength
									End If
									For j As Integer = firstMaskedStep To tsLength - 1
										inMask.putScalar(i, j, 1.0)
									Next j
								Next i
							End If

							For Each m As Bidirectional.Mode In modes
								'Skip 3 of 4 test cases: from 64 to 16, which still should be good coverage
								'Note RNG seed - deterministic run-to-run
								If r.Next(4) <> 0 Then
									Continue For
								End If

								Dim name As String = "mb=" & mb & ", maskType=" & maskType & ", mode=" & m & ", hasLayerNorm=" & hasLayerNorm & ", rnnType=" & (If(simple, "SimpleRnn", "LSTM"))

								Console.WriteLine("Starting test: " & name)

								Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nIn(nIn).nOut(3).build()).layer(New Bidirectional(m, (If(simple, (New SimpleRnn.Builder()).nIn(3).nOut(3).hasLayerNorm(hasLayerNorm).build(), (New LSTM.Builder()).nIn(3).nOut(3).build())))).layer((New RnnOutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).build()).build()


								Dim net As New MultiLayerNetwork(conf)
								net.init()


								Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).inputMask(inMask))
								assertTrue(gradOK)


								TestUtils.testModelSerialization(net)
							Next m
						Next hasLayerNorm
					Next simple
				Next inputMask
			Next mb
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/06/24 - Ignored to get to all passing baseline to prevent regressions via CI - see issue #7912") public void testSimpleRnn()
		Public Overridable Sub testSimpleRnn()
			Dim nOut As Integer = 5

			Dim l1s() As Double = {0.0, 0.4}
			Dim l2s() As Double = {0.0, 0.6}

			Dim r As New Random(12345)
			For Each mb As Integer In New Integer(){1, 3}
				For Each tsLength As Integer In New Integer(){1, 4}
					For Each nIn As Integer In New Integer(){3, 1}
						For Each layerSize As Integer In New Integer(){4, 1}
							For Each inputMask As Boolean In New Boolean(){False, True}
								For Each hasLayerNorm As Boolean In New Boolean(){True, False}
									For l As Integer = 0 To l1s.Length - 1
										'Only run 1 of 5 (on average - note RNG seed for deterministic testing) - 25 of 128 test cases (to minimize test time)
										If r.Next(5) <> 0 Then
											Continue For
										End If

										Dim [in] As INDArray = Nd4j.rand(New Integer(){mb, nIn, tsLength})
										Dim labels As INDArray = Nd4j.create(mb, nOut, tsLength)
										For i As Integer = 0 To mb - 1
											For j As Integer = 0 To tsLength - 1
												labels.putScalar(i, r.Next(nOut), j, 1.0)
											Next j
										Next i
										Dim maskType As String = (If(inputMask, "inputMask", "none"))

										Dim inMask As INDArray = Nothing
										If inputMask Then
											inMask = Nd4j.ones(mb, tsLength)
											For i As Integer = 0 To mb - 1
												Dim firstMaskedStep As Integer = tsLength - 1 - i
												If firstMaskedStep = 0 Then
													firstMaskedStep = tsLength
												End If
												For j As Integer = firstMaskedStep To tsLength - 1
													inMask.putScalar(i, j, 0.0)
												Next j
											Next i
										End If

										Dim name As String = "testSimpleRnn() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType & ", l1=" & l1s(l) & ", l2=" & l2s(l) & ", hasLayerNorm=" & hasLayerNorm

										Console.WriteLine("Starting test: " & name)

										Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).updater(New NoOp()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).l1(l1s(l)).l2(l2s(l)).list().layer((New SimpleRnn.Builder()).nIn(nIn).nOut(layerSize).hasLayerNorm(hasLayerNorm).build()).layer((New SimpleRnn.Builder()).nIn(layerSize).nOut(layerSize).hasLayerNorm(hasLayerNorm).build()).layer((New RnnOutputLayer.Builder()).nIn(layerSize).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

										Dim net As New MultiLayerNetwork(conf)
										net.init()


										Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).inputMask(inMask))
										assertTrue(gradOK)
										TestUtils.testModelSerialization(net)
									Next l
								Next hasLayerNorm
							Next inputMask
						Next layerSize
					Next nIn
				Next tsLength
			Next mb
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/06/24 - Ignored to get to all passing baseline to prevent regressions via CI - see issue #7912") public void testLastTimeStepLayer()
		Public Overridable Sub testLastTimeStepLayer()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 5
			Dim tsLength As Integer = 4
			Dim layerSize As Integer = 8

			Dim r As New Random(12345)
			For Each mb As Integer In New Integer(){1, 3}
				For Each inputMask As Boolean In New Boolean(){False, True}
					For Each simple As Boolean In New Boolean(){False, True}
						For Each hasLayerNorm As Boolean In New Boolean(){True, False}
							If Not simple AndAlso hasLayerNorm Then
								Continue For
							End If


							Dim [in] As INDArray = Nd4j.rand(New Integer(){mb, nIn, tsLength})
							Dim labels As INDArray = Nd4j.create(mb, nOut)
							For i As Integer = 0 To mb - 1
								labels.putScalar(i, r.Next(nOut), 1.0)
							Next i
							Dim maskType As String = (If(inputMask, "inputMask", "none"))

							Dim inMask As INDArray = Nothing
							If inputMask Then
								inMask = Nd4j.ones(mb, tsLength)
								For i As Integer = 0 To mb - 1
									Dim firstMaskedStep As Integer = tsLength - 1 - i
									If firstMaskedStep = 0 Then
										firstMaskedStep = tsLength
									End If
									For j As Integer = firstMaskedStep To tsLength - 1
										inMask.putScalar(i, j, 0.0)
									Next j
								Next i
							End If

							Dim name As String = "testLastTimeStepLayer() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType & ", hasLayerNorm=" & hasLayerNorm & ", rnnType=" & (If(simple, "SimpleRnn", "LSTM"))
							If PRINT_RESULTS Then
								Console.WriteLine("Starting test: " & name)
							End If

							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer(If(simple, (New SimpleRnn.Builder()).nOut(layerSize).hasLayerNorm(hasLayerNorm).build(), (New LSTM.Builder()).nOut(layerSize).build())).layer(New LastTimeStep(If(simple, (New SimpleRnn.Builder()).nOut(layerSize).hasLayerNorm(hasLayerNorm).build(), (New LSTM.Builder()).nOut(layerSize).build()))).layer((New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(nIn)).build()

							Dim net As New MultiLayerNetwork(conf)
							net.init()

							Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).inputMask(inMask).subset(True).maxPerParam(16))
							assertTrue(gradOK, name)
							TestUtils.testModelSerialization(net)
						Next hasLayerNorm
					Next simple
				Next inputMask
			Next mb
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeDistributedDense()
		Public Overridable Sub testTimeDistributedDense()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 5
			Dim tsLength As Integer = 4
			Dim layerSize As Integer = 8

			Dim r As New Random(12345)
			For Each mb As Integer In New Integer(){1, 3}
				For Each inputMask As Boolean In New Boolean(){False, True}


					Dim [in] As INDArray = Nd4j.rand(New Integer(){mb, nIn, tsLength})
					Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(mb, nOut, tsLength)
					Dim maskType As String = (If(inputMask, "inputMask", "none"))

					Dim inMask As INDArray = Nothing
					If inputMask Then
						inMask = Nd4j.ones(mb, tsLength)
						For i As Integer = 0 To mb - 1
							Dim firstMaskedStep As Integer = tsLength - 1 - i
							If firstMaskedStep = 0 Then
								firstMaskedStep = tsLength
							End If
							For j As Integer = firstMaskedStep To tsLength - 1
								inMask.putScalar(i, j, 0.0)
							Next j
						Next i
					End If

					Dim name As String = "testLastTimeStepLayer() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType
					If PRINT_RESULTS Then
						Console.WriteLine("Starting test: " & name)
					End If

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nOut(layerSize).build()).layer(New TimeDistributed((New DenseLayer.Builder()).nOut(layerSize).activation(Activation.SOFTMAX).build())).layer((New RnnOutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(nIn)).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).inputMask(inMask).subset(True).maxPerParam(16))
					assertTrue(gradOK, name)
					TestUtils.testModelSerialization(net)
				Next inputMask
			Next mb
		End Sub
	End Class

End Namespace