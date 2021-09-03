Imports System
Imports lombok
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"shape"}) public class FeedForwardToCnnPreProcessor implements org.deeplearning4j.nn.conf.InputPreProcessor
	<Serializable>
	Public Class FeedForwardToCnnPreProcessor
		Implements InputPreProcessor

		Private inputHeight As Long
		Private inputWidth As Long
		Private numChannels As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private long[] shape;
		Private shape() As Long

		''' <summary>
		''' Reshape to a channels x rows x columns tensor
		''' </summary>
		''' <param name="inputHeight"> the columns </param>
		''' <param name="inputWidth">  the rows </param>
		''' <param name="numChannels"> the channels </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public FeedForwardToCnnPreProcessor(@JsonProperty("inputHeight") long inputHeight, @JsonProperty("inputWidth") long inputWidth, @JsonProperty("numChannels") long numChannels)
		Public Sub New(ByVal inputHeight As Long, ByVal inputWidth As Long, ByVal numChannels As Long)
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = numChannels
		End Sub

		Public Sub New(ByVal inputWidth As Long, ByVal inputHeight As Long)
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = 1
		End Sub

		Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
			Me.shape = input.shape()
			If input.rank() = 4 Then
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input)
			End If

			If input.columns() <> inputWidth * inputHeight * numChannels Then
				Throw New System.ArgumentException("Invalid input: expect output columns must be equal to rows " & inputHeight & " x columns " & inputWidth & " x channels " & numChannels & " but was instead " & Arrays.toString(input.shape()))
			End If

			If input.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(input) Then
				input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "c"c)
			End If

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input.reshape("c"c, input.size(0), numChannels, inputHeight, inputWidth))
		End Function

		Public Overridable Function backprop(ByVal epsilons As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
			If epsilons.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(epsilons) Then
				epsilons = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilons, "c"c)
			End If

			If shape Is Nothing OrElse ArrayUtil.prod(shape) <> epsilons.length() Then
				If epsilons.rank() = 2 Then
					Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilons) 'should never happen
				End If

				Return epsilons.reshape("c"c, epsilons.size(0), numChannels, inputHeight, inputWidth)
			End If

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilons.reshape("c"c, shape))
		End Function


		Public Overridable Function clone() As FeedForwardToCnnPreProcessor
			Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim clone_Conflict As FeedForwardToCnnPreProcessor = CType(MyBase.clone(), FeedForwardToCnnPreProcessor)
				If clone_Conflict.shape IsNot Nothing Then
					clone_Conflict.shape = CType(clone_Conflict.shape.Clone(), Long())
				End If
				Return clone_Conflict
			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType

			Select Case inputType.getType()
				Case FF
					Dim c As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
					Dim expSize As val = inputHeight * inputWidth * numChannels
					If c.getSize() <> expSize Then
						Throw New System.InvalidOperationException("Invalid input: expected FeedForward input of size " & expSize & " = (d=" & numChannels & " * w=" & inputWidth & " * h=" & inputHeight & "), got " & inputType)
					End If
					Return InputType.convolutional(inputHeight, inputWidth, numChannels)
				Case CNN
					Dim c2 As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)

					If c2.getChannels() <> numChannels OrElse c2.getHeight() <> inputHeight OrElse c2.getWidth() <> inputWidth Then
						Throw New System.InvalidOperationException("Invalid input: Got CNN input type with (d,w,h)=(" & c2.getChannels() & "," & c2.getWidth() & "," & c2.getHeight() & ") but expected (" & numChannels & "," & inputHeight & "," & inputWidth & ")")
					End If
					Return c2
				Case CNNFlat
					Dim c3 As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
					If c3.getDepth() <> numChannels OrElse c3.getHeight() <> inputHeight OrElse c3.getWidth() <> inputWidth Then
						Throw New System.InvalidOperationException("Invalid input: Got CNN input type with (d,w,h)=(" & c3.getDepth() & "," & c3.getWidth() & "," & c3.getHeight() & ") but expected (" & numChannels & "," & inputHeight & "," & inputWidth & ")")
					End If
					Return c3.UnflattenedType
				Case Else
					Throw New System.InvalidOperationException("Invalid input type: got " & inputType)
			End Select
		End Function

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
			'Pass-through, unmodified (assuming here that it's a 1d mask array - one value per example)
			Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
		End Function

	End Class

End Namespace