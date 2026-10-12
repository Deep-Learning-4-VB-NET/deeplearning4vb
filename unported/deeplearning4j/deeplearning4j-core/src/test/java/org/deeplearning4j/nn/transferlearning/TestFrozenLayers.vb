Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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

Namespace org.deeplearning4j.nn.transferlearning


	Public Class TestFrozenLayers
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFrozenMLN()
		Public Overridable Sub testFrozenMLN()
			Dim orig As MultiLayerNetwork = getOriginalNet(12345)


			For Each l1 As Double In New Double(){0.0, 0.3}
				For Each l2 As Double In New Double(){0.0, 0.4}
					Dim msg As String = "l1=" & l1 & ", l2=" & l2

					Dim ftc As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.5)).l1(l1).l2(l2).build()

					Dim transfer As MultiLayerNetwork = (New TransferLearning.Builder(orig)).fineTuneConfiguration(ftc).setFeatureExtractor(4).removeOutputLayer().addLayer((New OutputLayer.Builder()).nIn(64).nOut(10).lossFunction(LossFunctions.LossFunction.MEAN_ABSOLUTE_ERROR).build()).build()

					assertEquals(6, transfer.getnLayers())
					For i As Integer = 0 To 4
						assertTrue(TypeOf transfer.getLayer(i) Is FrozenLayer)
					Next i

					Dim paramsBefore As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
					For Each entry As KeyValuePair(Of String, INDArray) In transfer.paramTable().SetOfKeyValuePairs()
						paramsBefore(entry.Key) = entry.Value.dup()
					Next entry

					For i As Integer = 0 To 19
						Dim f As INDArray = Nd4j.rand(New Integer(){16, 1, 28, 28})
						Dim l As INDArray = Nd4j.rand(New Integer(){16, 10})
						transfer.fit(f,l)
					Next i

					For Each entry As KeyValuePair(Of String, INDArray) In transfer.paramTable().SetOfKeyValuePairs()
						Dim s As String = msg & " - " & entry.Key
						If entry.Key.StartsWith("5_") Then
							'Non-frozen layer
							assertNotEquals(paramsBefore(entry.Key), entry.Value, s)
						Else
							assertEquals(paramsBefore(entry.Key), entry.Value, s)
						End If
					Next entry
				Next l2
			Next l1
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFrozenCG()
		Public Overridable Sub testFrozenCG()
			Dim orig As ComputationGraph = getOriginalGraph(12345)


			For Each l1 As Double In New Double(){0.0, 0.3}
				For Each l2 As Double In New Double(){0.0, 0.4}
					Dim msg As String = "l1=" & l1 & ", l2=" & l2

					Dim ftc As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.5)).l1(l1).l2(l2).build()

					Dim transfer As ComputationGraph = (New TransferLearning.GraphBuilder(orig)).fineTuneConfiguration(ftc).setFeatureExtractor("4").removeVertexAndConnections("5").addLayer("5", (New OutputLayer.Builder()).nIn(64).nOut(10).lossFunction(LossFunctions.LossFunction.MEAN_ABSOLUTE_ERROR).build(), "4").setOutputs("5").build()

					assertEquals(6, transfer.NumLayers)
					For i As Integer = 0 To 4
						assertTrue(TypeOf transfer.getLayer(i) Is FrozenLayer)
					Next i

					Dim paramsBefore As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
					For Each entry As KeyValuePair(Of String, INDArray) In transfer.paramTable().SetOfKeyValuePairs()
						paramsBefore(entry.Key) = entry.Value.dup()
					Next entry

					For i As Integer = 0 To 19
						Dim f As INDArray = Nd4j.rand(New Integer(){16, 1, 28, 28})
						Dim l As INDArray = Nd4j.rand(New Integer(){16, 10})
						transfer.fit(New INDArray(){f},New INDArray(){l})
					Next i

					For Each entry As KeyValuePair(Of String, INDArray) In transfer.paramTable().SetOfKeyValuePairs()
						Dim s As String = msg & " - " & entry.Key
						If entry.Key.StartsWith("5_") Then
							'Non-frozen layer
							assertNotEquals(paramsBefore(entry.Key), entry.Value, s)
						Else
							assertEquals(paramsBefore(entry.Key), entry.Value, s)
						End If
					Next entry
				Next l2
			Next l1
		End Sub

		Public Shared Function getOriginalNet(ByVal seed As Integer) As MultiLayerNetwork
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).weightInit(WeightInit.XAVIER).activation(Activation.TANH).convolutionMode(ConvolutionMode.Same).updater(New Sgd(0.3)).list().layer((New ConvolutionLayer.Builder()).nOut(3).kernelSize(2,2).stride(1,1).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2,2).stride(1,1).build()).layer((New ConvolutionLayer.Builder()).nIn(3).nOut(3).kernelSize(2,2).stride(1,1).build()).layer((New DenseLayer.Builder()).nOut(64).build()).layer((New DenseLayer.Builder()).nIn(64).nOut(64).build()).layer((New OutputLayer.Builder()).nIn(64).nOut(10).lossFunction(LossFunctions.LossFunction.MSE).build()).setInputType(InputType.convolutionalFlat(28,28,1)).build()


			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Return net
		End Function

		Public Shared Function getOriginalGraph(ByVal seed As Integer) As ComputationGraph
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).weightInit(WeightInit.XAVIER).activation(Activation.TANH).convolutionMode(ConvolutionMode.Same).updater(New Sgd(0.3)).graphBuilder().addInputs("in").layer("0", (New ConvolutionLayer.Builder()).nOut(3).kernelSize(2,2).stride(1,1).build(), "in").layer("1", (New SubsamplingLayer.Builder()).kernelSize(2,2).stride(1,1).build(), "0").layer("2", (New ConvolutionLayer.Builder()).nIn(3).nOut(3).kernelSize(2,2).stride(1,1).build(), "1").layer("3", (New DenseLayer.Builder()).nOut(64).build(), "2").layer("4", (New DenseLayer.Builder()).nIn(64).nOut(64).build(), "3").layer("5", (New OutputLayer.Builder()).nIn(64).nOut(10).lossFunction(LossFunctions.LossFunction.MSE).build(), "4").setOutputs("5").setInputTypes(InputType.convolutionalFlat(28,28,1)).build()


			Dim net As New ComputationGraph(conf)
			net.init()
			Return net
		End Function

	End Class

End Namespace