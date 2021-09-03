Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Darknet19 = org.deeplearning4j.zoo.model.Darknet19
Imports TinyYOLO = org.deeplearning4j.zoo.model.TinyYOLO
Imports YOLO2 = org.deeplearning4j.zoo.model.YOLO2
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationLReLU = org.nd4j.linalg.activations.impl.ActivationLReLU

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

Namespace org.deeplearning4j.zoo.model.helper

	Public Class DarknetHelper

		''' <summary>
		''' Returns {@code inputShape[1] / 32}, where {@code inputShape[1]} should be a multiple of 32. </summary>
		Public Shared Function getGridWidth(ByVal inputShape() As Integer) As Integer
			Return inputShape(1) \ 32
		End Function

		''' <summary>
		''' Returns {@code inputShape[2] / 32}, where {@code inputShape[2]} should be a multiple of 32. </summary>
		Public Shared Function getGridHeight(ByVal inputShape() As Integer) As Integer
			Return inputShape(2) \ 32
		End Function

		Public Shared Function addLayers(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal layerNumber As Integer, ByVal filterSize As Integer, ByVal nIn As Integer, ByVal nOut As Integer, ByVal poolSize As Integer) As ComputationGraphConfiguration.GraphBuilder
			Return addLayers(graphBuilder, layerNumber, filterSize, nIn, nOut, poolSize, poolSize)
		End Function

		Public Shared Function addLayers(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal layerNumber As Integer, ByVal filterSize As Integer, ByVal nIn As Integer, ByVal nOut As Integer, ByVal poolSize As Integer, ByVal poolStride As Integer) As ComputationGraphConfiguration.GraphBuilder
			Dim input As String = "maxpooling2d_" & (layerNumber - 1)
			If Not graphBuilder.getVertices().containsKey(input) Then
				input = "activation_" & (layerNumber - 1)
			End If
			If Not graphBuilder.getVertices().containsKey(input) Then
				input = "concatenate_" & (layerNumber - 1)
			End If
			If Not graphBuilder.getVertices().containsKey(input) Then
				input = "input"
			End If

			Return addLayers(graphBuilder, layerNumber, input, filterSize, nIn, nOut, poolSize, poolStride)
		End Function

		Public Shared Function addLayers(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal layerNumber As Integer, ByVal input As String, ByVal filterSize As Integer, ByVal nIn As Integer, ByVal nOut As Integer, ByVal poolSize As Integer, ByVal poolStride As Integer) As ComputationGraphConfiguration.GraphBuilder
			graphBuilder.addLayer("convolution2d_" & layerNumber, (New ConvolutionLayer.Builder(filterSize,filterSize)).nIn(nIn).nOut(nOut).weightInit(WeightInit.XAVIER).convolutionMode(ConvolutionMode.Same).hasBias(False).stride(1,1).activation(Activation.IDENTITY).build(), input).addLayer("batchnormalization_" & layerNumber, (New BatchNormalization.Builder()).nIn(nOut).nOut(nOut).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).build(), "convolution2d_" & layerNumber).addLayer("activation_" & layerNumber, (New ActivationLayer.Builder()).activation(New ActivationLReLU(0.1)).build(), "batchnormalization_" & layerNumber)
			If poolSize > 0 Then
				graphBuilder.addLayer("maxpooling2d_" & layerNumber, (New SubsamplingLayer.Builder()).kernelSize(poolSize, poolSize).stride(poolStride, poolStride).convolutionMode(ConvolutionMode.Same).build(), "activation_" & layerNumber)
			End If

			Return graphBuilder
		End Function

	End Class

End Namespace