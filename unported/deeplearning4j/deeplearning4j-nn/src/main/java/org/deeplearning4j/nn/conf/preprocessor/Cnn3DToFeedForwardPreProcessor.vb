Imports System
Imports Data = lombok.Data
Imports val = lombok.val
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports JsonCreator = org.nd4j.shade.jackson.annotation.JsonCreator
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
import static org.nd4j.linalg.api.shape.Shape.hasDefaultStridesForShape

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
'ORIGINAL LINE: @Data public class Cnn3DToFeedForwardPreProcessor implements org.deeplearning4j.nn.conf.InputPreProcessor
	<Serializable>
	Public Class Cnn3DToFeedForwardPreProcessor
		Implements InputPreProcessor

		Protected Friend inputDepth As Long
		Protected Friend inputHeight As Long
		Protected Friend inputWidth As Long
		Protected Friend numChannels As Long
		Protected Friend isNCDHW As Boolean = True ' channels first ordering by default

		''' <param name="inputDepth">  input channels </param>
		''' <param name="inputHeight"> input height </param>
		''' <param name="inputWidth">  input width </param>
		''' <param name="numChannels"> input channels </param>
		''' <param name="isNCDHW">     boolean to indicate data format, i.e. channels first (NCDHW) vs. channels last (NDHWC) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public Cnn3DToFeedForwardPreProcessor(@JsonProperty("inputDepth") long inputDepth, @JsonProperty("inputHeight") long inputHeight, @JsonProperty("inputWidth") long inputWidth, @JsonProperty("numChannels") long numChannels, @JsonProperty("isNCDHW") boolean isNCDHW)
		Public Sub New(ByVal inputDepth As Long, ByVal inputHeight As Long, ByVal inputWidth As Long, ByVal numChannels As Long, ByVal isNCDHW As Boolean)
			Me.inputDepth = inputDepth
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = numChannels
			Me.isNCDHW = isNCDHW
		End Sub

		Public Sub New(ByVal inputDepth As Integer, ByVal inputHeight As Integer, ByVal inputWidth As Integer)
			Me.inputDepth = inputDepth
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = 1
		End Sub

		Public Sub New(ByVal inputDepth As Integer, ByVal inputHeight As Integer, ByVal inputWidth As Integer, ByVal numChannels As Integer, ByVal dataFormat As Convolution3D.DataFormat)
			Me.New(inputDepth, inputHeight, inputWidth, numChannels, dataFormat = Convolution3D.DataFormat.NCDHW)
		End Sub

		Public Sub New()
		End Sub

		Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
			If input.rank() = 2 Then
				Return input ' Pass-through feed-forward input
			End If

			' We expect either NCDHW or NDHWC format
			If (isNCDHW AndAlso input.size(1) <> numChannels) OrElse (Not isNCDHW AndAlso input.size(4) <> numChannels) Then
				Throw New System.InvalidOperationException("Invalid input array: expected shape in format " & "[minibatch, channels, channels, height, width] or " & "[minibatch, channels, height, width, channels]" & " for numChannels: " & numChannels & ", inputDepth " & inputDepth & ", inputHeight " & inputHeight & " and inputWidth " & inputWidth & ", but got " & Arrays.toString(input.shape()))
			End If

			If Not hasDefaultStridesForShape(input) Then
				input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "c"c)
			End If

			Dim inShape As val = input.shape()
			Dim outShape As val = New Long(){inShape(0), inShape(1) * inShape(2) * inShape(3) * inShape(4)}

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input.reshape("c"c, outShape))
		End Function

		Public Overridable Function backprop(ByVal epsilons As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
			'Epsilons are 2d, with shape [miniBatchSize, outChannels*outD*outH*outW]

			If Not hasDefaultStridesForShape(epsilons) Then
				epsilons = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilons, "c"c)
			End If

			If epsilons.rank() = 5 Then
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilons) 'Should never happen
			End If

			If epsilons.columns() <> inputDepth * inputWidth * inputHeight * numChannels Then
				Throw New System.ArgumentException("Invalid input: expect output to have depth: " & inputDepth & ", height: " & inputHeight & ", width: " & inputWidth & " and channels: " & numChannels & ", i.e. [" & epsilons.rows() & ", " & inputDepth * inputHeight * inputWidth * numChannels & "] but was instead " & Arrays.toString(epsilons.shape()))
			End If

			Dim ret As INDArray
			If isNCDHW Then
				ret = epsilons.reshape("c"c, epsilons.size(0), numChannels, inputDepth, inputHeight, inputWidth)
			Else
				ret = epsilons.reshape("c"c, epsilons.size(0), inputDepth, inputHeight, inputWidth, numChannels)
			End If

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, ret) 'Move to specified workspace if required

		End Function

		Public Overridable Function clone() As Cnn3DToFeedForwardPreProcessor
			Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim clone_Conflict As Cnn3DToFeedForwardPreProcessor = CType(MyBase.clone(), Cnn3DToFeedForwardPreProcessor)
				Return clone_Conflict
			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN3D Then
				Throw New System.InvalidOperationException("Invalid input type: Expected input of type CNN3D, got " & inputType)
			End If

			Dim c As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)
			Dim outSize As val = c.getChannels() * c.getDepth() * c.getHeight() * c.getWidth()
			Return InputType.feedForward(outSize)
		End Function


		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
			'Pass-through, unmodified (assuming here that it's a 1d mask array - one value per example)
			Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
		End Function
	End Class

End Namespace