Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Updater = org.deeplearning4j.nn.conf.Updater
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports FrozenLayerWithBackprop = org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports MaskLayer = org.deeplearning4j.nn.conf.layers.util.MaskLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class UtilLayerGradientChecks extends org.deeplearning4j.BaseDL4JTest
	Public Class UtilLayerGradientChecks
		Inherits BaseDL4JTest

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMaskLayer()
		Public Overridable Sub testMaskLayer()
			Nd4j.Random.setSeed(12345)
			Dim tsLength As Integer = 3

			For Each minibatch As Integer In New Integer(){1, 3}
				For Each inputRank As Integer In New Integer(){2, 3, 4}
					For Each inputMask As Boolean In New Boolean(){False, True}
						Dim maskType As String = (If(inputMask, "inputMask", "none"))

						Dim inMask As INDArray = Nothing
						If inputMask Then
							Select Case inputRank
								Case 2
									If minibatch = 1 Then
										inMask = Nd4j.ones(1,1)
									Else
										inMask = Nd4j.create(DataType.DOUBLE, minibatch, 1)
										Nd4j.Executioner.exec(New BernoulliDistribution(inMask, 0.5))
										Dim count As Integer = inMask.sumNumber().intValue()
										assertTrue(count >= 0 AndAlso count <= minibatch) 'Sanity check on RNG seed
									End If
								Case 4
									'Per-example mask (broadcast along all channels/x/y)
									If minibatch = 1 Then
										inMask = Nd4j.ones(DataType.DOUBLE, 1,1, 1, 1)
									Else
										inMask = Nd4j.create(DataType.DOUBLE, minibatch, 1, 1, 1)
										Nd4j.Executioner.exec(New BernoulliDistribution(inMask, 0.5))
										Dim count As Integer = inMask.sumNumber().intValue()
										assertTrue(count >= 0 AndAlso count <= minibatch) 'Sanity check on RNG seed
									End If
								Case 3
									inMask = Nd4j.ones(DataType.DOUBLE, minibatch, tsLength)
									For i As Integer = 0 To minibatch - 1
										For j As Integer = i+1 To tsLength - 1
											inMask.putScalar(i,j,0.0)
										Next j
									Next i
								Case Else
									Throw New Exception()
							End Select
						End If

						Dim inShape() As Integer
						Dim labelShape() As Integer
						Select Case inputRank
							Case 2
								inShape = New Integer(){minibatch, 3}
								labelShape = inShape
							Case 3
								inShape = New Integer(){minibatch, 3, tsLength}
								labelShape = inShape
							Case 4
								inShape = New Integer(){minibatch, 1, 5, 5}
								labelShape = New Integer(){minibatch, 5}
							Case Else
								Throw New Exception()
						End Select
						Dim input As INDArray = Nd4j.rand(inShape).muli(100)
						Dim label As INDArray = Nd4j.rand(labelShape)

						Dim name As String = "mb=" & minibatch & ", maskType=" & maskType & ", inputRank=" & inputRank
						Console.WriteLine("*** Starting test: " & name)

						Dim l1 As Layer
						Dim l2 As Layer
						Dim l3 As Layer
						Dim it As InputType
						Select Case inputRank
							Case 2
								l1 = (New DenseLayer.Builder()).nOut(3).build()
								l2 = (New DenseLayer.Builder()).nOut(3).build()
								l3 = (New OutputLayer.Builder()).nOut(3).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.TANH).build()
								it = InputType.feedForward(3)
							Case 3
								l1 = (New SimpleRnn.Builder()).nIn(3).nOut(3).activation(Activation.TANH).build()
								l2 = (New SimpleRnn.Builder()).nIn(3).nOut(3).activation(Activation.TANH).build()
								l3 = (New RnnOutputLayer.Builder()).nIn(3).nOut(3).lossFunction(LossFunctions.LossFunction.SQUARED_LOSS).activation(Activation.IDENTITY).build()
								it = InputType.recurrent(3)
							Case 4
								l1 = (New ConvolutionLayer.Builder()).nOut(5).convolutionMode(ConvolutionMode.Truncate).stride(1,1).kernelSize(2,2).padding(0,0).build()
								l2 = (New ConvolutionLayer.Builder()).nOut(5).convolutionMode(ConvolutionMode.Truncate).stride(1,1).kernelSize(2,2).padding(0,0).build()
								l3 = (New OutputLayer.Builder()).nOut(5).lossFunction(LossFunctions.LossFunction.SQUARED_LOSS).activation(Activation.IDENTITY).build()
								it = InputType.convolutional(5,5,1)
							Case Else
								Throw New Exception()

						End Select

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).activation(Activation.TANH).dataType(DataType.DOUBLE).dist(New NormalDistribution(0,2)).list().layer(l1).layer(New MaskLayer()).layer(l2).layer(l3).setInputType(it).build()


						Dim net As New MultiLayerNetwork(conf)
						net.init()

						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).minAbsoluteError(1e-6).input(input).labels(label).inputMask(inMask))
						assertTrue(gradOK)

						TestUtils.testModelSerialization(net)
					Next inputMask
				Next inputRank
			Next minibatch
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFrozenWithBackprop()
		Public Overridable Sub testFrozenWithBackprop()

			For Each minibatch As Integer In New Integer(){1, 5}

				Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).updater(Updater.NONE).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(New FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build())).layer(New FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build())).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
				Dim net As New MultiLayerNetwork(conf2)
				net.init()

				Dim [in] As INDArray = Nd4j.rand(minibatch, 10)
				Dim labels As INDArray = TestUtils.randomOneHot(minibatch, 10)

				Dim excludeParams As ISet(Of String) = New HashSet(Of String)()
				excludeParams.addAll(Arrays.asList("1_W", "1_b", "2_W", "2_b"))

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).excludeParams(excludeParams))
				assertTrue(gradOK)

				TestUtils.testModelSerialization(net)


				'Test ComputationGraph equivalent:
				Dim g As ComputationGraph = net.toComputationGraph()

				Dim gradOKCG As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(g).minAbsoluteError(1e-6).inputs(New INDArray(){[in]}).labels(New INDArray(){labels}).excludeParams(excludeParams))
				assertTrue(gradOKCG)

				TestUtils.testModelSerialization(g)
			Next minibatch

		End Sub
	End Class

End Namespace