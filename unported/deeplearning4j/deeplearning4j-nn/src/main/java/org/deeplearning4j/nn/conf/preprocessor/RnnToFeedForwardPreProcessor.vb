Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports org.nd4j.common.primitives
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
'ORIGINAL LINE: @Data @Slf4j @NoArgsConstructor public class RnnToFeedForwardPreProcessor implements org.deeplearning4j.nn.conf.InputPreProcessor
	<Serializable>
	Public Class RnnToFeedForwardPreProcessor
		Implements InputPreProcessor

		Private rnnDataFormat As RNNFormat = RNNFormat.NCW

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RnnToFeedForwardPreProcessor(@JsonProperty("rnnDataFormat") org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat)
		Public Sub New(ByVal rnnDataFormat As RNNFormat)
			If rnnDataFormat <> Nothing Then
				Me.rnnDataFormat = rnnDataFormat
			End If
		End Sub
		Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
			'Need to reshape RNN activations (3d) activations to 2d (for input into feed forward layer)
			If input.rank() <> 3 Then
				If input.rank() = 2 Then
					log.trace("Input rank was already 2. This can happen when an RNN like layer (such as GlobalPooling) is hooked up to an OutputLayer.")
					Return input
				Else
					Throw New System.ArgumentException("Invalid input: expect NDArray with rank 3 (i.e., activations for RNN layer)")
				End If
			End If
			If input.ordering() <> "f"c OrElse Not Shape.hasDefaultStridesForShape(input) Then
				input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "f"c)
			End If

			If rnnDataFormat = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
			End If
			Dim shape As val = input.shape()
			Dim ret As INDArray
			If shape(0) = 1 Then
				ret = input.tensorAlongDimension(0, 1, 2).permute(1, 0) 'Edge case: miniBatchSize==1
			ElseIf shape(2) = 1 Then
				ret = input.tensorAlongDimension(0, 1, 0) 'Edge case: timeSeriesLength=1
			Else
				Dim permuted As INDArray = input.permute(0, 2, 1) 'Permute, so we get correct order after reshaping
				ret = permuted.reshape("f"c, shape(0) * shape(2), shape(1))
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, ret)
		End Function

		Public Overridable Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
			If output Is Nothing Then
				Return Nothing 'In a few cases: output may be null, and this is valid. Like time series data -> embedding layer
			End If
			'Need to reshape FeedForward layer epsilons (2d) to 3d (for use in RNN layer backprop calculations)
			If output.rank() <> 2 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 2 (i.e., epsilons from feed forward layer)")
			End If
			If output.ordering() <> "f"c OrElse Not Shape.hasDefaultStridesForShape(output) Then
				output = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, output, "f"c)
			End If

			Dim shape As val = output.shape()
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim reshaped As INDArray = output.reshape("f"c, miniBatchSize, shape(0) / miniBatchSize, shape(1))
			If rnnDataFormat = RNNFormat.NCW Then
				reshaped = reshaped.permute(0, 2, 1)
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, reshaped)
		End Function

		Public Overridable Function clone() As RnnToFeedForwardPreProcessor
			Return New RnnToFeedForwardPreProcessor(rnnDataFormat)
		End Function

		Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input: expected input of type RNN, got " & inputType)
			End If

			Dim rnn As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Return InputType.feedForward(rnn.getSize(), rnn.getFormat())
		End Function

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
			'Assume mask array is 2d for time series (1 value per time step)
			If maskArray Is Nothing Then
				Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
			ElseIf maskArray.rank() = 2 Then
				'Need to reshape mask array from [minibatch,timeSeriesLength] to [minibatch*timeSeriesLength, 1]
				Return New Pair(Of INDArray, MaskState)(TimeSeriesUtils.reshapeTimeSeriesMaskToVector(maskArray, LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT), currentMaskState)
			Else
				Throw New System.ArgumentException("Received mask array of rank " & maskArray.rank() & "; expected rank 2 mask array. Mask array shape: " & Arrays.toString(maskArray.shape()))
			End If
		End Function
	End Class

End Namespace