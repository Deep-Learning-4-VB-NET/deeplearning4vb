Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.nn.misc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestNetConversion extends org.deeplearning4j.BaseDL4JTest
	Public Class TestNetConversion
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMlnToCompGraph()
		Public Overridable Sub testMlnToCompGraph()
			Nd4j.Random.setSeed(12345)

			For i As Integer = 0 To 2
				Dim n As MultiLayerNetwork
				Select Case i
					Case 0
						n = getNet1(False)
					Case 1
						n = getNet1(True)
					Case 2
						n = Net2
					Case Else
						Throw New Exception()
				End Select
				Dim [in] As INDArray = (If(i <= 1, Nd4j.rand(New Integer(){8, 3, 10, 10}), Nd4j.rand(New Integer(){8, 5, 10})))
				Dim labels As INDArray = (If(i <= 1, Nd4j.rand(New Integer(){8, 10}), Nd4j.rand(New Integer(){8, 10, 10})))

				Dim cg As ComputationGraph = n.toComputationGraph()

				Dim out1 As INDArray = n.output([in])
				Dim out2 As INDArray = cg.outputSingle([in])
				assertEquals(out1, out2)


				n.Input = [in]
				n.Labels = labels

				cg.Inputs = [in]
				cg.Labels = labels

				n.computeGradientAndScore()
				cg.computeGradientAndScore()

				assertEquals(n.score(), cg.score(), 1e-6)

				assertEquals(n.gradient().gradient(), cg.gradient().gradient())

				n.fit([in], labels)
				cg.fit(New INDArray(){[in]}, New INDArray(){labels})

				assertEquals(n.params(), cg.params())
			Next i
		End Sub

		Private Function getNet1(ByVal train As Boolean) As MultiLayerNetwork

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(New Sgd(0.1)).list().layer((New ConvolutionLayer.Builder()).nIn(3).nOut(5).kernelSize(2, 2).stride(1, 1).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2, 2).stride(1, 1).build()).layer((New DenseLayer.Builder()).nOut(32).build()).layer((New OutputLayer.Builder()).nOut(10).lossFunction(LossFunctions.LossFunction.MSE).build()).setInputType(InputType.convolutional(10, 10, 3)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			If train Then
				For i As Integer = 0 To 2
					Dim f As INDArray = Nd4j.rand(New Integer(){8, 3, 10, 10})
					Dim l As INDArray = Nd4j.rand(8, 10)

					net.fit(f, l)
				Next i
			End If

			Return net
		End Function

		Private ReadOnly Property Net2 As MultiLayerNetwork
			Get
    
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(New Sgd(0.1)).list().layer((New GravesLSTM.Builder()).nOut(8).build()).layer((New LSTM.Builder()).nOut(8).build()).layer((New RnnOutputLayer.Builder()).nOut(10).lossFunction(LossFunctions.LossFunction.MSE).build()).setInputType(InputType.recurrent(5)).build()
    
				Dim net As New MultiLayerNetwork(conf)
				net.init()
    
				For i As Integer = 0 To 2
					Dim f As INDArray = Nd4j.rand(New Integer(){8, 5, 10})
					Dim l As INDArray = Nd4j.rand(New Integer(){8, 10, 10})
    
					net.fit(f, l)
				Next i
    
				Return net
    
			End Get
		End Property

	End Class

End Namespace