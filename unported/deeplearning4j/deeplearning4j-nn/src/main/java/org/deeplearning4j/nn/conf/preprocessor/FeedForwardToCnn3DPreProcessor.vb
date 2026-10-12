Imports System
Imports lombok
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"shape"}) public class FeedForwardToCnn3DPreProcessor implements org.deeplearning4j.nn.conf.InputPreProcessor
	<Serializable>
	Public Class FeedForwardToCnn3DPreProcessor
		Implements InputPreProcessor

		Private inputDepth As Integer
		Private inputHeight As Integer
		Private inputWidth As Integer
		Private numChannels As Integer
		Private isNCDHW As Boolean = True

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private long[] shape;
		Private shape() As Long

		''' <param name="inputDepth">  input channels </param>
		''' <param name="inputHeight"> input height </param>
		''' <param name="inputWidth">  input width </param>
		''' <param name="numChannels"> input channels </param>
		''' <param name="isNCDHW">     boolean to indicate data format, i.e. channels first (NCDHW) vs. channels last (NDHWC) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public FeedForwardToCnn3DPreProcessor(@JsonProperty("inputDepth") int inputDepth, @JsonProperty("inputHeight") int inputHeight, @JsonProperty("inputWidth") int inputWidth, @JsonProperty("numChannels") int numChannels, @JsonProperty("isNCDHW") boolean isNCDHW)
		Public Sub New(ByVal inputDepth As Integer, ByVal inputHeight As Integer, ByVal inputWidth As Integer, ByVal numChannels As Integer, ByVal isNCDHW As Boolean)
			Me.inputDepth = inputDepth
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = numChannels
			Me.isNCDHW = isNCDHW
		End Sub

		Public Sub New(ByVal inputDepth As Integer, ByVal inputWidth As Integer, ByVal inputHeight As Integer)
			Me.inputDepth = inputDepth
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = 1
		End Sub

		Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
			Me.shape = input.shape()

			If shape.Length = 5 Then
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input)
			End If

			If Not hasDefaultStridesForShape(input) Then
				input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "c"c)
			End If

			If input.columns() <> inputDepth * inputWidth * inputHeight * numChannels Then
				Throw New System.ArgumentException("Invalid input: expect output columns must be equal to channels " & inputDepth & " times height " & inputWidth & "times width " & inputWidth & " times channels " & numChannels & " but was instead " & Arrays.toString(input.shape()))
			End If

			Dim ret As INDArray
			If isNCDHW Then
				ret = input.reshape("c"c, input.size(0), numChannels, inputDepth, inputHeight, inputWidth)
			Else
				ret = input.reshape("c"c, input.size(0), inputDepth, inputHeight, inputWidth, numChannels)
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, ret)
		End Function

		Public Overridable Function backprop(ByVal epsilons As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
			If Not hasDefaultStridesForShape(epsilons) Then
				epsilons = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilons, "c"c)
			End If

			If shape Is Nothing OrElse ArrayUtil.prod(shape) <> epsilons.length() Then
				Dim ret As INDArray = epsilons.reshape("c"c, epsilons.size(0),inputDepth * inputHeight * inputWidth * numChannels)
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, ret)
			End If

			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilons.reshape("c"c, shape))
		End Function


		Public Overridable Function clone() As FeedForwardToCnn3DPreProcessor
			Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim clone_Conflict As FeedForwardToCnn3DPreProcessor = CType(MyBase.clone(), FeedForwardToCnn3DPreProcessor)
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
					Dim expSize As Integer = inputDepth * inputHeight * inputWidth * numChannels
					If c.getSize() <> expSize Then
						Throw New System.InvalidOperationException("Invalid input: expected FeedForward input of size " & expSize & " = (d=" & numChannels & " * w=" & inputWidth & " * h=" & inputHeight & "), got " & inputType)
					End If
					Return InputType.convolutional3D(inputDepth, inputHeight, inputWidth, numChannels)
				Case CNN
					Dim c2 As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)

					If c2.getChannels() <> numChannels OrElse c2.getHeight() <> inputHeight OrElse c2.getWidth() <> inputWidth Then
						Throw New System.InvalidOperationException("Invalid input: Got CNN input type with (c,w,h)=(" & c2.getChannels() & "," & c2.getWidth() & "," & c2.getHeight() & ") but expected (" & numChannels & "," & inputHeight & "," & inputWidth & ")")
					End If
					Return InputType.convolutional3D(1, c2.getHeight(), c2.getWidth(), c2.getChannels())
				Case CNN3D
					Dim c3 As InputType.InputTypeConvolutional3D = DirectCast(inputType, InputType.InputTypeConvolutional3D)

					If c3.getChannels() <> numChannels OrElse c3.getDepth() <> inputDepth OrElse c3.getHeight() <> inputHeight OrElse c3.getWidth() <> inputWidth Then
						Throw New System.InvalidOperationException("Invalid input: Got CNN input type with (c, d,w,h)=(" & c3.getChannels() & "," & c3.getDepth() & "," & c3.getWidth() & "," & c3.getHeight() & ") but expected (" & numChannels & "," & inputDepth & "," & inputHeight & "," & inputWidth & ")")
					End If
					Return c3
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