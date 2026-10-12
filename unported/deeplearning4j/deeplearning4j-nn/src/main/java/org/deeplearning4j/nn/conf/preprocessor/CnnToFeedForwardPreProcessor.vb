Imports System
Imports Data = lombok.Data
Imports val = lombok.val
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports JsonCreator = org.nd4j.shade.jackson.annotation.JsonCreator
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.conf.preprocessor


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class CnnToFeedForwardPreProcessor implements org.deeplearning4j.nn.conf.InputPreProcessor
	<Serializable>
	Public Class CnnToFeedForwardPreProcessor
		Implements InputPreProcessor

		Protected Friend inputHeight As Long
		Protected Friend inputWidth As Long
		Protected Friend numChannels As Long
		Protected Friend format As CNN2DFormat = CNN2DFormat.NCHW 'Default for legacy JSON deserialization

		''' <param name="inputHeight"> the columns </param>
		''' <param name="inputWidth"> the rows </param>
		''' <param name="numChannels"> the channels </param>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public CnnToFeedForwardPreProcessor(@JsonProperty("inputHeight") long inputHeight, @JsonProperty("inputWidth") long inputWidth, @JsonProperty("numChannels") long numChannels, @JsonProperty("format") org.deeplearning4j.nn.conf.CNN2DFormat format)
		Public Sub New(ByVal inputHeight As Long, ByVal inputWidth As Long, ByVal numChannels As Long, ByVal format As CNN2DFormat)
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = numChannels
			If format <> Nothing Then
				Me.format = format
			End If
		End Sub

		Public Sub New(ByVal inputHeight As Long, ByVal inputWidth As Long)
			Me.New(inputHeight, inputWidth, 1, CNN2DFormat.NCHW)
		End Sub

		Public Sub New(ByVal inputHeight As Long, ByVal inputWidth As Long, ByVal numChannels As Long)
			Me.New(inputHeight, inputWidth, numChannels, CNN2DFormat.NCHW)
		End Sub

		Public Sub New()
		End Sub

		Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
			If input.rank() = 2 Then
				Return input 'Should usually never happen
			End If

			Dim chDim As Integer = 1
			Dim hDim As Integer = 2
			Dim wDim As Integer = 3
			If format = CNN2DFormat.NHWC Then
				chDim = 3
				hDim = 1
				wDim = 2
			End If

			If inputHeight = 0 AndAlso inputWidth = 0 AndAlso numChannels = 0 Then
				Me.inputHeight = input.size(hDim)
				Me.inputWidth = input.size(wDim)
				Me.numChannels = input.size(chDim)
			End If

			If input.size(chDim) <> numChannels OrElse input.size(hDim) <> inputHeight OrElse input.size(wDim) <> inputWidth Then
				Throw New System.InvalidOperationException("Invalid input, does not match configuration: expected " & (If(format = CNN2DFormat.NCHW, "[minibatch, numChannels=" & numChannels & ", inputHeight=" & inputHeight & ", inputWidth=" & inputWidth & "] ", "[minibatch, inputHeight=" & inputHeight & ", inputWidth=" & inputWidth & ", numChannels=" & numChannels & "]")) & " but got input array of shape " & Arrays.toString(input.shape()))
			End If

			'Check input: nchw format
			If input.size(chDim) <> numChannels OrElse input.size(hDim) <> inputHeight OrElse input.size(wDim) <> inputWidth Then
				Throw New System.InvalidOperationException("Invalid input array: expected shape [minibatch, channels, height, width] = " & "[minibatch, " & numChannels & ", " & inputHeight & ", " & inputWidth & "] - got " & Arrays.toString(input.shape()))
			End If

			'Assume input is standard rank 4 activations out of CNN layer
			'First: we require input to be in c order. But c order (as declared in array order) isn't enough; also need strides to be correct
			If input.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(input) Then
				input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "c"c)
			End If

			'Note that to match Tensorflow/Keras, we do a simple "c order reshape" for both NCHW and NHWC

			Dim inShape As val = input.shape() '[miniBatch,depthOut,outH,outW]
			Dim outShape As val = New Long(){inShape(0), inShape(1) * inShape(2) * inShape(3)}

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input.reshape("c"c, outShape)) 'Should be zero copy reshape
		End Function

		Public Overridable Function backprop(ByVal epsilons As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
			'Epsilons from layer above should be 2d, with shape [miniBatchSize, depthOut*outH*outW]
			If epsilons.ordering() <> "c"c OrElse Not Shape.strideDescendingCAscendingF(epsilons) Then
				epsilons = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilons, "c"c)
			End If

			If epsilons.rank() = 4 Then
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilons) 'Should never happen
			End If

			If epsilons.columns() <> inputWidth * inputHeight * numChannels Then
				Throw New System.ArgumentException("Invalid input: expect output columns must be equal to rows " & inputHeight & " x columns " & inputWidth & " x channels " & numChannels & " but was instead " & Arrays.toString(epsilons.shape()))
			End If

			Dim ret As INDArray
			If format = CNN2DFormat.NCHW Then
				ret = epsilons.reshape("c"c, epsilons.size(0), numChannels, inputHeight, inputWidth)
			Else
				ret = epsilons.reshape("c"c, epsilons.size(0), inputHeight, inputWidth, numChannels)
			End If

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, ret) 'Move if required to specified workspace
		End Function

		Public Overridable Function clone() As CnnToFeedForwardPreProcessor
			Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim clone_Conflict As CnnToFeedForwardPreProcessor = CType(MyBase.clone(), CnnToFeedForwardPreProcessor)
				Return clone_Conflict
			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input type: Expected input of type CNN, got " & inputType)
			End If

			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Dim outSize As val = c.getChannels() * c.getHeight() * c.getWidth()
			'h=2,w=1,c=5 pre processor: 0,0,NCHW (broken)
			'h=2,w=2,c=3, cnn=2,2,3, NCHW
			Return InputType.feedForward(outSize)
		End Function


		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
			If maskArray Is Nothing OrElse maskArray.rank() = 2 Then
				Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
			End If

			If maskArray.rank() <> 4 OrElse maskArray.size(2) <> 1 OrElse maskArray.size(3) <> 1 Then
				Throw New System.NotSupportedException("Expected rank 4 mask array for 2D CNN layer activations. Got rank " & maskArray.rank() & " mask array (shape " & Arrays.toString(maskArray.shape()) & ")  - when used in conjunction with input data of shape" & " [batch,channels,h,w] 4d masks passing through CnnToFeedForwardPreProcessor should have shape" & " [batchSize,1,1,1]")
			End If

			Return New Pair(Of INDArray, MaskState)(maskArray.reshape(maskArray.ordering(), maskArray.size(0), maskArray.size(1)), currentMaskState)
		End Function
	End Class

End Namespace