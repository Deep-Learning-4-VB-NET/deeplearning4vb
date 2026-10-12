Imports System
Imports lombok
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"product"}) public class CnnToRnnPreProcessor implements org.deeplearning4j.nn.conf.InputPreProcessor
	<Serializable>
	Public Class CnnToRnnPreProcessor
		Implements InputPreProcessor

		Private inputHeight As Long
		Private inputWidth As Long
		Private numChannels As Long
		Private rnnDataFormat As RNNFormat = RNNFormat.NCW

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private long product;
		Private product As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public CnnToRnnPreProcessor(@JsonProperty("inputHeight") long inputHeight, @JsonProperty("inputWidth") long inputWidth, @JsonProperty("numChannels") long numChannels, @JsonProperty("rnnDataFormat") org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat)
		Public Sub New(ByVal inputHeight As Long, ByVal inputWidth As Long, ByVal numChannels As Long, ByVal rnnDataFormat As RNNFormat)
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = numChannels
			Me.product = inputHeight * inputWidth * numChannels
			Me.rnnDataFormat = rnnDataFormat
		End Sub

		Public Sub New(ByVal inputHeight As Long, ByVal inputWidth As Long, ByVal numChannels As Long)
			Me.New(inputHeight, inputWidth, numChannels, RNNFormat.NCW)
		End Sub

		Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
			If input.rank() <> 4 Then
				Throw New System.ArgumentException("Invalid input: expect CNN activations with rank 4 (received input with shape " & Arrays.toString(input.shape()) & ")")
			End If
			If input.size(1) <> numChannels OrElse input.size(2) <> inputHeight OrElse input.size(3) <> inputWidth Then
				Throw New System.InvalidOperationException("Invalid input, does not match configuration: expected [minibatch, numChannels=" & numChannels & ", inputHeight=" & inputHeight & ", inputWidth=" & inputWidth & "] but got input array of" & "shape " & Arrays.toString(input.shape()))
			End If
			'Input: 4d activations (CNN)
			'Output: 3d activations (RNN)

			If input.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(input) Then
				input = input.dup("c"c)
			End If

			Dim shape As val = input.shape() '[timeSeriesLength*miniBatchSize, numChannels, inputHeight, inputWidth]

			'First: reshape 4d to 2d, as per CnnToFeedForwardPreProcessor
			Dim twod As INDArray = input.reshape("c"c, input.size(0), ArrayUtil.prod(input.shape()) \ input.size(0))
			'Second: reshape 2d to 3d, as per FeedForwardToRnnPreProcessor
			Dim reshaped As INDArray = workspaceMgr.dup(ArrayType.ACTIVATIONS, twod, "f"c)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			reshaped = reshaped.reshape("f"c, miniBatchSize, shape(0) / miniBatchSize, product)
			If rnnDataFormat = RNNFormat.NCW Then
				Return reshaped.permute(0, 2, 1)
			End If
			Return reshaped
		End Function

		Public Overridable Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
			If output.ordering() = "c"c OrElse Not Shape.hasDefaultStridesForShape(output) Then
				output = output.dup("f"c)
			End If
			If rnnDataFormat = RNNFormat.NWC Then
				output = output.permute(0, 2, 1)
			End If
			Dim shape As val = output.shape()
			Dim output2d As INDArray
			If shape(0) = 1 Then
				'Edge case: miniBatchSize = 1
				output2d = output.tensorAlongDimension(0, 1, 2).permutei(1, 0)
			ElseIf shape(2) = 1 Then
				'Edge case: timeSeriesLength = 1
				output2d = output.tensorAlongDimension(0, 1, 0)
			Else
				'As per FeedForwardToRnnPreprocessor
				Dim permuted3d As INDArray = output.permute(0, 2, 1)
				output2d = permuted3d.reshape("f"c, shape(0) * shape(2), shape(1))
			End If

			If shape(1) <> product Then
				Throw New System.ArgumentException("Invalid input: expected output size(1)=" & shape(1) & " must be equal to " & inputHeight & " x columns " & inputWidth & " x channels " & numChannels & " = " & product & ", received: " & shape(1))
			End If
			Dim ret As INDArray = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, output2d, "c"c)
			Return ret.reshape("c"c, output2d.size(0), numChannels, inputHeight, inputWidth)
		End Function

		Public Overridable Function clone() As CnnToRnnPreProcessor
			Return New CnnToRnnPreProcessor(inputHeight, inputWidth, numChannels, rnnDataFormat)
		End Function

		Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.CNN Then
				Throw New System.InvalidOperationException("Invalid input type: Expected input of type CNN, got " & inputType)
			End If

			Dim c As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
			Dim outSize As val = c.getChannels() * c.getHeight() * c.getWidth()
			Return InputType.recurrent(outSize, rnnDataFormat)
		End Function

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
			'Assume mask array is 4d - a mask array that has been reshaped from [minibatch,timeSeriesLength] to [minibatch*timeSeriesLength, 1, 1, 1]
			If maskArray Is Nothing Then
				Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
			Else
				'Need to reshape mask array from [minibatch*timeSeriesLength, 1, 1, 1] to [minibatch,timeSeriesLength]
				Return New Pair(Of INDArray, MaskState)(TimeSeriesUtils.reshapeCnnMaskToTimeSeriesMask(maskArray, minibatchSize),currentMaskState)
			End If
		End Function
	End Class

End Namespace