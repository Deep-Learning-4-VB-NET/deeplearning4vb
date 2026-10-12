Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Cropping2D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping2D
Imports NASNet = org.deeplearning4j.zoo.model.NASNet
Imports Activation = org.nd4j.linalg.activations.Activation
Imports org.nd4j.common.primitives

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


	Public Class NASNetHelper


		Public Shared Function sepConvBlock(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal filters As Integer, ByVal kernelSize As Integer, ByVal stride As Integer, ByVal blockId As String, ByVal input As String) As String
			Dim prefix As String = "sepConvBlock" & blockId

			graphBuilder.addLayer(prefix & "_act", New ActivationLayer(Activation.RELU), input).addLayer(prefix & "_sepconv1", (New SeparableConvolution2D.Builder(kernelSize, kernelSize)).stride(stride, stride).nOut(filters).hasBias(False).convolutionMode(ConvolutionMode.Same).build(), prefix & "_act").addLayer(prefix & "_conv1_bn", (New BatchNormalization.Builder()).eps(1e-3).gamma(0.9997).build(), prefix & "_sepconv1").addLayer(prefix & "_act2", New ActivationLayer(Activation.RELU), prefix & "_conv1_bn").addLayer(prefix & "_sepconv2", (New SeparableConvolution2D.Builder(kernelSize, kernelSize)).stride(stride, stride).nOut(filters).hasBias(False).convolutionMode(ConvolutionMode.Same).build(), prefix & "_act2").addLayer(prefix & "_conv2_bn", (New BatchNormalization.Builder()).eps(1e-3).gamma(0.9997).build(), prefix & "_sepconv2")

			Return prefix & "_conv2_bn"
		End Function

		Public Shared Function adjustBlock(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal filters As Integer, ByVal blockId As String, ByVal input As String) As String
			Return adjustBlock(graphBuilder, filters, blockId, input, Nothing)
		End Function

		Public Shared Function adjustBlock(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal filters As Integer, ByVal blockId As String, ByVal input As String, ByVal inputToMatch As String) As String
			Dim prefix As String = "adjustBlock" & blockId
			Dim outputName As String = input

			If inputToMatch Is Nothing Then
				inputToMatch = input
			End If
			Dim layerActivationTypes As IDictionary(Of String, InputType) = graphBuilder.getLayerActivationTypes()
			Dim shapeToMatch As val = layerActivationTypes(inputToMatch).getShape()
			Dim inputShape As val = layerActivationTypes(input).getShape()

			If shapeToMatch(1) <> inputShape(1) Then
				graphBuilder.addLayer(prefix & "_relu1", New ActivationLayer(Activation.RELU), input).addLayer(prefix & "_avgpool1", (New SubsamplingLayer.Builder(PoolingType.AVG)).kernelSize(1,1).stride(2,2).convolutionMode(ConvolutionMode.Truncate).build(), prefix & "_relu1").addLayer(prefix & "_conv1", (New ConvolutionLayer.Builder(1,1)).stride(1,1).nOut(CInt(Math.Floor(filters \ 2))).hasBias(False).convolutionMode(ConvolutionMode.Same).build(), prefix & "_avg_pool_1").addLayer(prefix & "_zeropad1", New ZeroPaddingLayer(0,1), prefix & "_relu1").addLayer(prefix & "_crop1", New Cropping2D(1,0), prefix & "_zeropad_1").addLayer(prefix & "_avgpool2", (New SubsamplingLayer.Builder(PoolingType.AVG)).kernelSize(1,1).stride(2,2).convolutionMode(ConvolutionMode.Truncate).build(), prefix & "_crop1").addLayer(prefix & "_conv2", (New ConvolutionLayer.Builder(1,1)).stride(1,1).nOut(CInt(Math.Floor(filters \ 2))).hasBias(False).convolutionMode(ConvolutionMode.Same).build(), prefix & "_avgpool2").addVertex(prefix & "_concat1", New MergeVertex(), prefix & "_conv1", prefix & "_conv2").addLayer(prefix & "_bn1", (New BatchNormalization.Builder()).eps(1e-3).gamma(0.9997).build(), prefix & "_concat1")

				outputName = prefix & "_bn1"
			End If

			If inputShape(3) <> filters Then
				graphBuilder.addLayer(prefix & "_projection_relu", New ActivationLayer(Activation.RELU), outputName).addLayer(prefix & "_projection_conv", (New ConvolutionLayer.Builder(1,1)).stride(1,1).nOut(filters).hasBias(False).convolutionMode(ConvolutionMode.Same).build(), prefix & "_projection_relu").addLayer(prefix & "_projection_bn", (New BatchNormalization.Builder()).eps(1e-3).gamma(0.9997).build(), prefix & "_projection_conv")
				outputName = prefix & "_projection_bn"
			End If

			Return outputName
		End Function

		Public Shared Function normalA(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal filters As Integer, ByVal blockId As String, ByVal inputX As String, ByVal inputP As String) As Pair(Of String, String)
			Dim prefix As String = "normalA" & blockId

			Dim topAdjust As String = adjustBlock(graphBuilder, filters, prefix, inputP, inputX)

			' top block
			graphBuilder.addLayer(prefix & "_relu1", New ActivationLayer(Activation.RELU), topAdjust).addLayer(prefix & "_conv1", (New ConvolutionLayer.Builder(1,1)).stride(1,1).nOut(filters).hasBias(False).convolutionMode(ConvolutionMode.Same).build(), prefix & "_relu1").addLayer(prefix & "_bn1", (New BatchNormalization.Builder()).eps(1e-3).gamma(0.9997).build(), prefix & "_conv1")

			' block 1
			Dim left1 As String = sepConvBlock(graphBuilder, filters, 5, 1, prefix & "_left1", prefix & "_bn1")
			Dim right1 As String = sepConvBlock(graphBuilder, filters, 3, 1, prefix & "_right1", topAdjust)
			graphBuilder.addVertex(prefix & "_add1", New ElementWiseVertex(ElementWiseVertex.Op.Add), left1, right1)

			' block 2
			Dim left2 As String = sepConvBlock(graphBuilder, filters, 5, 1, prefix & "_left2", topAdjust)
			Dim right2 As String = sepConvBlock(graphBuilder, filters, 3, 1, prefix & "_right2", topAdjust)
			graphBuilder.addVertex(prefix & "_add2", New ElementWiseVertex(ElementWiseVertex.Op.Add), left2, right2)

			' block 3
			graphBuilder.addLayer(prefix & "_left3", (New SubsamplingLayer.Builder(PoolingType.AVG)).kernelSize(3,3).stride(1,1).convolutionMode(ConvolutionMode.Same).build(), prefix & "_bn1").addVertex(prefix & "_add3", New ElementWiseVertex(ElementWiseVertex.Op.Add), prefix & "_left3", topAdjust)

			' block 4
			graphBuilder.addLayer(prefix & "_left4", (New SubsamplingLayer.Builder(PoolingType.AVG)).kernelSize(3,3).stride(1,1).convolutionMode(ConvolutionMode.Same).build(), topAdjust).addLayer(prefix & "_right4", (New SubsamplingLayer.Builder(PoolingType.AVG)).kernelSize(3,3).stride(1,1).convolutionMode(ConvolutionMode.Same).build(), topAdjust).addVertex(prefix & "_add4", New ElementWiseVertex(ElementWiseVertex.Op.Add), prefix & "_left4", prefix & "_right4")

			' block 5
			Dim left5 As String = sepConvBlock(graphBuilder, filters, 3, 1, prefix & "_left5", topAdjust)
			graphBuilder.addVertex(prefix & "_add5", New ElementWiseVertex(ElementWiseVertex.Op.Add), prefix & "_left5", prefix & "_bn1")

			' output
			graphBuilder.addVertex(prefix, New MergeVertex(), topAdjust, prefix & "_add1", prefix & "_add2", prefix & "_add3", prefix & "_add4", prefix & "_add5")

			Return New Pair(Of String, String)(prefix, inputX)

		End Function

		Public Shared Function reductionA(ByVal graphBuilder As ComputationGraphConfiguration.GraphBuilder, ByVal filters As Integer, ByVal blockId As String, ByVal inputX As String, ByVal inputP As String) As Pair(Of String, String)
			Dim prefix As String = "reductionA" & blockId

			Dim topAdjust As String = adjustBlock(graphBuilder, filters, prefix, inputP, inputX)

			' top block
			graphBuilder.addLayer(prefix & "_relu1", New ActivationLayer(Activation.RELU), topAdjust).addLayer(prefix & "_conv1", (New ConvolutionLayer.Builder(1,1)).stride(1,1).nOut(filters).hasBias(False).convolutionMode(ConvolutionMode.Same).build(), prefix & "_relu1").addLayer(prefix & "_bn1", (New BatchNormalization.Builder()).eps(1e-3).gamma(0.9997).build(), prefix & "_conv1")

			' block 1
			Dim left1 As String = sepConvBlock(graphBuilder, filters, 5, 2, prefix & "_left1", prefix & "_bn1")
			Dim right1 As String = sepConvBlock(graphBuilder, filters, 7, 2, prefix & "_right1", topAdjust)
			graphBuilder.addVertex(prefix & "_add1", New ElementWiseVertex(ElementWiseVertex.Op.Add), left1, right1)

			' block 2
			graphBuilder.addLayer(prefix & "_left2", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3,3).stride(2,2).convolutionMode(ConvolutionMode.Same).build(), prefix & "_bn1")
			Dim right2 As String = sepConvBlock(graphBuilder, filters, 3, 1, prefix & "_right2", topAdjust)
			graphBuilder.addVertex(prefix & "_add2", New ElementWiseVertex(ElementWiseVertex.Op.Add), prefix & "_left2", right2)

			' block 3
			graphBuilder.addLayer(prefix & "_left3", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.AVG)).kernelSize(3,3).stride(2,2).convolutionMode(ConvolutionMode.Same).build(), prefix & "_bn1")
			Dim right3 As String = sepConvBlock(graphBuilder, filters, 5, 2, prefix & "_right3", topAdjust)
			graphBuilder.addVertex(prefix & "_add3", New ElementWiseVertex(ElementWiseVertex.Op.Add), prefix & "_left3", right3)

			' block 4
			graphBuilder.addLayer(prefix & "_left4", (New SubsamplingLayer.Builder(PoolingType.AVG)).kernelSize(3,3).stride(1,1).convolutionMode(ConvolutionMode.Same).build(), prefix & "_add1").addVertex(prefix & "_add4", New ElementWiseVertex(ElementWiseVertex.Op.Add), prefix & "_add2", prefix & "_left4")

			' block 5
			Dim left5 As String = sepConvBlock(graphBuilder, filters, 3, 2, prefix & "_left5", prefix & "_add1")
			graphBuilder.addLayer(prefix & "_right5", (New SubsamplingLayer.Builder(PoolingType.MAX)).kernelSize(3,3).stride(2,2).convolutionMode(ConvolutionMode.Same).build(), prefix & "_bn1").addVertex(prefix & "_add5", New ElementWiseVertex(ElementWiseVertex.Op.Add), left5, prefix & "_right5")

			' output
			graphBuilder.addVertex(prefix, New MergeVertex(), prefix & "_add2", prefix & "_add3", prefix & "_add4", prefix & "_add5")

			Return New Pair(Of String, String)(prefix, inputX)


		End Function

	End Class

End Namespace