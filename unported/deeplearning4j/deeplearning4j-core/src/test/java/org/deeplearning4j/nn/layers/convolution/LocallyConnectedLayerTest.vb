Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.layers.convolution

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Locally Connected Layer Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class LocallyConnectedLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class LocallyConnectedLayerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void before()
		Friend Overridable Sub before()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Nd4j.factory().DType = DataType.DOUBLE
			Nd4j.EPS_THRESHOLD = 1e-4
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test 2 d Forward") void test2dForward()
		Friend Overridable Sub test2dForward()
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(123).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).l2(2e-4).updater(New Nesterovs(0.9)).dropOut(0.5).list().layer((New LocallyConnected2D.Builder()).kernelSize(8, 8).nIn(3).stride(4, 4).nOut(16).dropOut(0.5).convolutionMode(ConvolutionMode.Strict).setInputSize(28, 28).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)).nOut(10).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 3))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Dim input As INDArray = Nd4j.ones(10, 3, 28, 28)
			Dim output As INDArray = network.output(input, False)
			assertArrayEquals(New Long() { 10, 10 }, output.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test 1 d Forward") void test1dForward()
		Friend Overridable Sub test1dForward()
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(123).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).l2(2e-4).updater(New Nesterovs(0.9)).dropOut(0.5).list().layer((New LocallyConnected1D.Builder()).kernelSize(4).nIn(3).stride(1).nOut(16).dropOut(0.5).convolutionMode(ConvolutionMode.Strict).setInputSize(28).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.SQUARED_LOSS)).nOut(10).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.recurrent(3, 8))
			Dim conf As MultiLayerConfiguration = builder.build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Dim input As INDArray = Nd4j.ones(10, 3, 8)
			Dim output As INDArray = network.output(input, False)

			For i As Integer = 0 To 99
				' TODO: this falls flat for 1000 iterations on my machine
				output = network.output(input, False)
			Next i
			assertArrayEquals(New Long() { (8 - 4 + 1) * 10, 10 }, output.shape())
			network.fit(input, output)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Locally Connected") void testLocallyConnected()
		Friend Overridable Sub testLocallyConnected()
			For Each globalDtype As DataType In New DataType() { DataType.DOUBLE, DataType.FLOAT, DataType.HALF }
				Nd4j.setDefaultDataTypes(globalDtype, globalDtype)
				For Each networkDtype As DataType In New DataType() { DataType.DOUBLE, DataType.FLOAT, DataType.HALF }
					assertEquals(globalDtype, Nd4j.dataType())
					assertEquals(globalDtype, Nd4j.defaultFloatingPointType())
					For test As Integer = 0 To 1
						Dim msg As String = "Global dtype: " & globalDtype & ", network dtype: " & networkDtype & ", test=" & test
						Dim b As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).dataType(networkDtype).seed(123).updater(New NoOp()).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).graphBuilder()
						Dim [in]() As INDArray
						Dim label As INDArray
						Select Case test
							Case 0
								b.addInputs("in").addLayer("1", (New LSTM.Builder()).nOut(5).build(), "in").addLayer("2", (New LocallyConnected1D.Builder()).kernelSize(2).nOut(4).build(), "1").addLayer("out", (New RnnOutputLayer.Builder()).nOut(10).build(), "2").setOutputs("out").setInputTypes(InputType.recurrent(5, 4))
								[in] = New INDArray() { Nd4j.rand(networkDtype, 2, 5, 4) }
								label = TestUtils.randomOneHotTimeSeries(2, 10, 4).castTo(networkDtype)
							Case 1
								b.addInputs("in").addLayer("1", (New ConvolutionLayer.Builder()).kernelSize(2, 2).nOut(5).convolutionMode(ConvolutionMode.Same).build(), "in").addLayer("2", (New LocallyConnected2D.Builder()).kernelSize(2, 2).nOut(5).build(), "1").addLayer("out", (New OutputLayer.Builder()).nOut(10).build(), "2").setOutputs("out").setInputTypes(InputType.convolutional(8, 8, 1))
								[in] = New INDArray() { Nd4j.rand(networkDtype, 2, 1, 8, 8) }
								label = TestUtils.randomOneHot(2, 10).castTo(networkDtype)
							Case Else
								Throw New Exception()
						End Select
						Dim net As New ComputationGraph(b.build())
						net.init()
						Dim [out] As INDArray = net.outputSingle([in])
						assertEquals(networkDtype, [out].dataType(),msg)
						Dim ff As IDictionary(Of String, INDArray) = net.feedForward([in], False)
						For Each e As KeyValuePair(Of String, INDArray) In ff.SetOfKeyValuePairs()
							If e.Key.Equals("in") Then
								Continue For
							End If
							Dim s As String = msg & " - layer: " & e.Key
							assertEquals(networkDtype, e.Value.dataType(),s)
						Next e
						net.Inputs = [in]
						net.Labels = label
						net.computeGradientAndScore()
						net.fit(New MultiDataSet([in], New INDArray() { label }))
					Next test
				Next networkDtype
			Next globalDtype
		End Sub
	End Class

End Namespace