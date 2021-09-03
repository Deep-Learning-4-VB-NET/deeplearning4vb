Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports AttentionVertex = org.deeplearning4j.nn.conf.graph.AttentionVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
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
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertThrows
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName

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
'ORIGINAL LINE: @Disabled @DisplayName("Attention Layer Test") @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) class AttentionLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class AttentionLayerTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Self Attention Layer") void testSelfAttentionLayer()
		Friend Overridable Sub testSelfAttentionLayer()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 2
			Dim tsLength As Integer = 4
			Dim layerSize As Integer = 4
			For Each mb As Integer In New Integer() { 1, 3 }
				For Each inputMask As Boolean In New Boolean() { False, True }
					For Each projectInput As Boolean In New Boolean() { False, True }
						Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer() { mb, nIn, tsLength })
						Dim labels As INDArray = TestUtils.randomOneHot(mb, nOut)
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
						Dim name As String = "testSelfAttentionLayer() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType & ", projectInput = " & projectInput
						Console.WriteLine("Starting test: " & name)
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nOut(layerSize).build()).layer(If(projectInput, (New SelfAttentionLayer.Builder()).nOut(4).nHeads(2).projectInput(True).build(), (New SelfAttentionLayer.Builder()).nHeads(1).projectInput(False).build())).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.MAX).build()).layer((New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(nIn)).build()
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).inputMask(inMask).subset(True).maxPerParam(100))
						assertTrue(gradOK,name)
					Next projectInput
				Next inputMask
			Next mb
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Learned Self Attention Layer") void testLearnedSelfAttentionLayer()
		Friend Overridable Sub testLearnedSelfAttentionLayer()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 2
			Dim tsLength As Integer = 4
			Dim layerSize As Integer = 4
			Dim numQueries As Integer = 3
			For Each inputMask As Boolean In New Boolean() { False, True }
				For Each mb As Integer In New Integer() { 3, 1 }
					For Each projectInput As Boolean In New Boolean() { False, True }
						Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer() { mb, nIn, tsLength })
						Dim labels As INDArray = TestUtils.randomOneHot(mb, nOut)
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
						Dim name As String = "testLearnedSelfAttentionLayer() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType & ", projectInput = " & projectInput
						Console.WriteLine("Starting test: " & name)
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nOut(layerSize).build()).layer(If(projectInput, (New LearnedSelfAttentionLayer.Builder()).nOut(4).nHeads(2).nQueries(numQueries).projectInput(True).build(), (New LearnedSelfAttentionLayer.Builder()).nHeads(1).nQueries(numQueries).projectInput(False).build())).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.MAX).build()).layer((New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(nIn)).build()
						Dim net As New MultiLayerNetwork(conf)
						net.init()
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).inputMask(inMask).subset(True).maxPerParam(100))
						assertTrue(gradOK,name)
					Next projectInput
				Next mb
			Next inputMask
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Learned Self Attention Layer _ different Mini Batch Sizes") void testLearnedSelfAttentionLayer_differentMiniBatchSizes()
		Friend Overridable Sub testLearnedSelfAttentionLayer_differentMiniBatchSizes()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 2
			Dim tsLength As Integer = 4
			Dim layerSize As Integer = 4
			Dim numQueries As Integer = 3
			Dim r As New Random(12345)
			For Each inputMask As Boolean In New Boolean() { False, True }
				For Each projectInput As Boolean In New Boolean() { False, True }
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nOut(layerSize).build()).layer(If(projectInput, (New LearnedSelfAttentionLayer.Builder()).nOut(4).nHeads(2).nQueries(numQueries).projectInput(True).build(), (New LearnedSelfAttentionLayer.Builder()).nHeads(1).nQueries(numQueries).projectInput(False).build())).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.MAX).build()).layer((New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(nIn)).build()
					Dim net As New MultiLayerNetwork(conf)
					net.init()
					For Each mb As Integer In New Integer() { 3, 1 }
						Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer() { mb, nIn, tsLength })
						Dim labels As INDArray = TestUtils.randomOneHot(mb, nOut)
						Dim maskType As String = (If(inputMask, "inputMask", "none"))
						Dim inMask As INDArray = Nothing
						If inputMask Then
							inMask = Nd4j.ones(DataType.INT, mb, tsLength)
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
						Dim name As String = "testLearnedSelfAttentionLayer() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType & ", projectInput = " & projectInput
						Console.WriteLine("Starting test: " & name)
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).inputMask(inMask).subset(True).maxPerParam(100))
						assertTrue(gradOK,name)
					Next mb
				Next projectInput
			Next inputMask
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Recurrent Attention Layer _ differing Time Steps") void testRecurrentAttentionLayer_differingTimeSteps()
		Friend Overridable Sub testRecurrentAttentionLayer_differingTimeSteps()
		   assertThrows(GetType(System.ArgumentException), Sub()
		   Dim nIn As Integer = 9
		   Dim nOut As Integer = 5
		   Dim layerSize As Integer = 8
		   Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.IDENTITY).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nOut(layerSize).build()).layer((New RecurrentAttentionLayer.Builder()).nIn(layerSize).nOut(layerSize).nHeads(1).projectInput(False).hasBias(False).build()).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.AVG).build()).layer((New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(nIn)).build()
		   Dim net As New MultiLayerNetwork(conf)
		   net.init()
		   Dim initialInput As INDArray = Nd4j.rand(New Integer() { 8, nIn, 7 })
		   Dim goodNextInput As INDArray = Nd4j.rand(New Integer() { 8, nIn, 7 })
		   Dim badNextInput As INDArray = Nd4j.rand(New Integer() { 8, nIn, 12 })
		   Dim labels As INDArray = Nd4j.rand(New Integer() { 8, nOut })
		   net.fit(initialInput, labels)
		   net.fit(goodNextInput, labels)
		   net.fit(badNextInput, labels)
		   End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Recurrent Attention Layer") void testRecurrentAttentionLayer()
		Friend Overridable Sub testRecurrentAttentionLayer()
			Dim nIn As Integer = 4
			Dim nOut As Integer = 2
			Dim tsLength As Integer = 3
			Dim layerSize As Integer = 3
			For Each mb As Integer In New Integer() { 3, 1 }
				For Each inputMask As Boolean In New Boolean() { True, False }
					Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer() { mb, nIn, tsLength })
					Dim labels As INDArray = TestUtils.randomOneHot(mb, nOut)
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
					Dim name As String = "testRecurrentAttentionLayer() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType
					Console.WriteLine("Starting test: " & name)
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.IDENTITY).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nOut(layerSize).build()).layer((New RecurrentAttentionLayer.Builder()).nIn(layerSize).nOut(layerSize).nHeads(1).projectInput(False).hasBias(False).build()).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.AVG).build()).layer((New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(nIn)).build()
					Dim net As New MultiLayerNetwork(conf)
					net.init()
					' System.out.println("Original");
					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input([in]).labels(labels).inputMask(inMask).subset(True).maxPerParam(100))
					assertTrue(gradOK,name)
				Next inputMask
			Next mb
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Attention Vertex") void testAttentionVertex()
		Friend Overridable Sub testAttentionVertex()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 2
			Dim tsLength As Integer = 3
			Dim layerSize As Integer = 3
			Dim r As New Random(12345)
			For Each inputMask As Boolean In New Boolean() { False, True }
				For Each mb As Integer In New Integer() { 3, 1 }
					For Each projectInput As Boolean In New Boolean() { False, True }
						Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer() { mb, nIn, tsLength })
						Dim labels As INDArray = TestUtils.randomOneHot(mb, nOut)
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
						Dim name As String = "testAttentionVertex() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType & ", projectInput = " & projectInput
						Console.WriteLine("Starting test: " & name)
						Dim graph As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("input").addLayer("rnnKeys", (New SimpleRnn.Builder()).nOut(layerSize).build(), "input").addLayer("rnnQueries", (New SimpleRnn.Builder()).nOut(layerSize).build(), "input").addLayer("rnnValues", (New SimpleRnn.Builder()).nOut(layerSize).build(), "input").addVertex("attention",If(projectInput, (New AttentionVertex.Builder()).nOut(4).nHeads(2).projectInput(True).nInQueries(layerSize).nInKeys(layerSize).nInValues(layerSize).build(), (New AttentionVertex.Builder()).nOut(3).nHeads(1).projectInput(False).nInQueries(layerSize).nInKeys(layerSize).nInValues(layerSize).build()), "rnnQueries", "rnnKeys", "rnnValues").addLayer("pooling", (New GlobalPoolingLayer.Builder()).poolingType(PoolingType.MAX).build(), "attention").addLayer("output", (New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "pooling").setOutputs("output").setInputTypes(InputType.recurrent(nIn)).build()
						Dim net As New ComputationGraph(graph)
						net.init()
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(net).inputs(New INDArray() { [in] }).labels(New INDArray() { labels }).inputMask(If(inMask IsNot Nothing, New INDArray() { inMask }, Nothing)).subset(True).maxPerParam(100))
						assertTrue(gradOK,name)
					Next projectInput
				Next mb
			Next inputMask
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Attention Vertex Same Input") void testAttentionVertexSameInput()
		Friend Overridable Sub testAttentionVertexSameInput()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 2
			Dim tsLength As Integer = 4
			Dim layerSize As Integer = 4
			Dim r As New Random(12345)
			For Each inputMask As Boolean In New Boolean() { False, True }
				For Each mb As Integer In New Integer() { 3, 1 }
					For Each projectInput As Boolean In New Boolean() { False, True }
						Dim [in] As INDArray = Nd4j.rand(New Integer() { mb, nIn, tsLength })
						Dim labels As INDArray = TestUtils.randomOneHot(mb, nOut)
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
						Dim name As String = "testAttentionVertex() - mb=" & mb & ", tsLength = " & tsLength & ", maskType=" & maskType & ", projectInput = " & projectInput
						Console.WriteLine("Starting test: " & name)
						Dim graph As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).updater(New NoOp()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("input").addLayer("rnn", (New SimpleRnn.Builder()).activation(Activation.TANH).nOut(layerSize).build(), "input").addVertex("attention",If(projectInput, (New AttentionVertex.Builder()).nOut(4).nHeads(2).projectInput(True).nInQueries(layerSize).nInKeys(layerSize).nInValues(layerSize).build(), (New AttentionVertex.Builder()).nOut(4).nHeads(1).projectInput(False).nInQueries(layerSize).nInKeys(layerSize).nInValues(layerSize).build()), "rnn", "rnn", "rnn").addLayer("pooling", (New GlobalPoolingLayer.Builder()).poolingType(PoolingType.MAX).build(), "attention").addLayer("output", (New OutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "pooling").setOutputs("output").setInputTypes(InputType.recurrent(nIn)).build()
						Dim net As New ComputationGraph(graph)
						net.init()
						Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(net).inputs(New INDArray() { [in] }).labels(New INDArray() { labels }).inputMask(If(inMask IsNot Nothing, New INDArray() { inMask }, Nothing)))
						assertTrue(gradOK,name)
					Next projectInput
				Next mb
			Next inputMask
		End Sub
	End Class

End Namespace