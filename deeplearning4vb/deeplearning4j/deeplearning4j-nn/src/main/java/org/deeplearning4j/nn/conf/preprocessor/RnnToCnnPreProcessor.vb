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
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"product"}) public class RnnToCnnPreProcessor implements org.deeplearning4j.nn.conf.InputPreProcessor
	<Serializable>
	Public Class RnnToCnnPreProcessor
		Implements InputPreProcessor

		Private inputHeight As Integer
		Private inputWidth As Integer
		Private numChannels As Integer
		Private rnnDataFormat As RNNFormat = RNNFormat.NCW
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter(AccessLevel.NONE) private int product;
		Private product As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RnnToCnnPreProcessor(@JsonProperty("inputHeight") int inputHeight, @JsonProperty("inputWidth") int inputWidth, @JsonProperty("numChannels") int numChannels, @JsonProperty("rnnDataFormat") org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat)
		Public Sub New(ByVal inputHeight As Integer, ByVal inputWidth As Integer, ByVal numChannels As Integer, ByVal rnnDataFormat As RNNFormat)
			Me.inputHeight = inputHeight
			Me.inputWidth = inputWidth
			Me.numChannels = numChannels
			Me.product = inputHeight * inputWidth * numChannels
			Me.rnnDataFormat = rnnDataFormat
		End Sub

		Public Sub New(ByVal inputHeight As Integer, ByVal inputWidth As Integer, ByVal numChannels As Integer)
			Me.New(inputHeight, inputWidth, numChannels, RNNFormat.NCW)
		End Sub

		Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
			If input.ordering() <> "f"c OrElse Not Shape.hasDefaultStridesForShape(input) Then
				input = input.dup("f"c)
			End If
			'Input: 3d activations (RNN)
			'Output: 4d activations (CNN)
			If rnnDataFormat = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
			End If
			Dim shape As val = input.shape()
			Dim in2d As INDArray
			If shape(0) = 1 Then
				'Edge case: miniBatchSize = 1
				in2d = input.tensorAlongDimension(0, 1, 2).permutei(1, 0)
			ElseIf shape(2) = 1 Then
				'Edge case: time series length = 1
				in2d = input.tensorAlongDimension(0, 1, 0)
			Else
				Dim permuted As INDArray = input.permute(0, 2, 1) 'Permute, so we get correct order after reshaping
				in2d = permuted.reshape("f"c, shape(0) * shape(2), shape(1))
			End If

			Return workspaceMgr.dup(ArrayType.ACTIVATIONS, in2d, "c"c).reshape("c"c, shape(0) * shape(2), numChannels, inputHeight, inputWidth)
		End Function

		Public Overridable Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
			'Input: 4d epsilons (CNN)
			'Output: 3d epsilons (RNN)
			If output.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(output) Then
				output = output.dup("c"c)
			End If
			Dim shape As val = output.shape()
			'First: reshape 4d to 2d
			Dim twod As INDArray = output.reshape("c"c, output.size(0), ArrayUtil.prod(output.shape()) \ output.size(0))
			'Second: reshape 2d to 3d
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim reshaped As INDArray = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, twod, "f"c).reshape("f"c, miniBatchSize, shape(0) / miniBatchSize, product)
			If rnnDataFormat = RNNFormat.NCW Then
				reshaped = reshaped.permute(0, 2, 1)
			End If
			Return reshaped
		End Function

		Public Overridable Function clone() As RnnToCnnPreProcessor
			Return New RnnToCnnPreProcessor(inputHeight, inputWidth, numChannels, rnnDataFormat)
		End Function

		Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input type: Expected input of type RNN, got " & inputType)
			End If

			Dim c As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim expSize As Integer = inputHeight * inputWidth * numChannels
			If c.getSize() <> expSize Then
				Throw New System.InvalidOperationException("Invalid input: expected RNN input of size " & expSize & " = (d=" & numChannels & " * w=" & inputWidth & " * h=" & inputHeight & "), got " & inputType)
			End If

			Return InputType.convolutional(inputHeight, inputWidth, numChannels)
		End Function

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
			'Assume mask array is 2d for time series (1 value per time step)
			If maskArray Is Nothing Then
				Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
			ElseIf maskArray.rank() = 2 Then
				'Need to reshape mask array from [minibatch,timeSeriesLength] to 4d minibatch format: [minibatch*timeSeriesLength, 1, 1, 1]
				Return New Pair(Of INDArray, MaskState)(TimeSeriesUtils.reshapeTimeSeriesMaskToCnn4dMask(maskArray, LayerWorkspaceMgr.noWorkspacesImmutable(), ArrayType.INPUT), currentMaskState)
			Else
				Throw New System.ArgumentException("Received mask array of rank " & maskArray.rank() & "; expected rank 2 mask array. Mask array shape: " & Arrays.toString(maskArray.shape()))
			End If
		End Function
	End Class

End Namespace