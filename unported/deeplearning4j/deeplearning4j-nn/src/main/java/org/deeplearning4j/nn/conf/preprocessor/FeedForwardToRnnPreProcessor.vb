Imports System
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
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
'ORIGINAL LINE: @Data @NoArgsConstructor public class FeedForwardToRnnPreProcessor implements org.deeplearning4j.nn.conf.InputPreProcessor
	<Serializable>
	Public Class FeedForwardToRnnPreProcessor
		Implements InputPreProcessor

		Private rnnDataFormat As RNNFormat = RNNFormat.NCW

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FeedForwardToRnnPreProcessor(@JsonProperty("rnnDataFormat") org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat)
		Public Sub New(ByVal rnnDataFormat As RNNFormat)
			If rnnDataFormat <> Nothing Then
				Me.rnnDataFormat = rnnDataFormat
			End If
		End Sub
		Public Overridable Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.preProcess
			'Need to reshape FF activations (2d) activations to 3d (for input into RNN layer)
			If input.rank() <> 2 Then
				If input.rank() < 2 Then
					input = input.reshape(ChrW(1), input.length())
				ElseIf input.rank() = 2 Then
					'just continue
				Else
					Throw New System.ArgumentException("Invalid input: expect NDArray with rank 2 (i.e., activations for FF layer)")
				End If
			End If

			If input.ordering() <> "f"c OrElse Not Shape.hasDefaultStridesForShape(input) Then
				input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "f"c)
			End If

			Dim shape As val = input.shape()
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim reshaped As INDArray = input.reshape("f"c, miniBatchSize, shape(0) / miniBatchSize, shape(1))
			If rnnDataFormat = RNNFormat.NCW Then
				reshaped = reshaped.permute(0, 2, 1)
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, reshaped)
		End Function

		Public Overridable Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements InputPreProcessor.backprop
			'Need to reshape RNN epsilons (3d) to 2d (for use in FF layer backprop calculations)
			If output.rank() <> 3 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 3 (i.e., epsilons from RNN layer)")
			End If
			If output.ordering() <> "f"c OrElse Not Shape.hasDefaultStridesForShape(output) Then
				output = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, output, "f"c)
			End If
			If rnnDataFormat = RNNFormat.NWC Then
				output = output.permute(0, 2, 1)
			End If
			Dim shape As val = output.shape()

			Dim ret As INDArray
			If shape(0) = 1 Then
				ret = output.tensorAlongDimension(0, 1, 2).permutei(1, 0) 'Edge case: miniBatchSize==1
			ElseIf shape(2) = 1 Then
				Return output.tensorAlongDimension(0, 1, 0) 'Edge case: timeSeriesLength=1
			Else
				Dim permuted As INDArray = output.permute(0, 2, 1) 'Permute, so we get correct order after reshaping
				ret = permuted.reshape("f"c, shape(0) * shape(2), shape(1))
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, ret)
		End Function

		Public Overridable Function clone() As FeedForwardToRnnPreProcessor
			Return New FeedForwardToRnnPreProcessor(rnnDataFormat)
		End Function

		Public Overridable Function getOutputType(ByVal inputType As InputType) As InputType Implements InputPreProcessor.getOutputType
			If inputType Is Nothing OrElse (inputType.getType() <> InputType.Type.FF AndAlso inputType.getType() <> InputType.Type.CNNFlat) Then
				Throw New System.InvalidOperationException("Invalid input: expected input of type FeedForward, got " & inputType)
			End If

			If inputType.getType() = InputType.Type.FF Then
				Dim ff As InputType.InputTypeFeedForward = DirectCast(inputType, InputType.InputTypeFeedForward)
				Return InputType.recurrent(ff.getSize(), rnnDataFormat)
			Else
				Dim cf As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
				Return InputType.recurrent(cf.FlattenedSize, rnnDataFormat)
			End If
		End Function

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
			'Assume mask array is 1d - a mask array that has been reshaped from [minibatch,timeSeriesLength] to [minibatch*timeSeriesLength, 1]
			If maskArray Is Nothing Then
				Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
			ElseIf maskArray.Vector Then
				'Need to reshape mask array from [minibatch*timeSeriesLength, 1] to [minibatch,timeSeriesLength]
				Return New Pair(Of INDArray, MaskState)(TimeSeriesUtils.reshapeVectorToTimeSeriesMask(maskArray, minibatchSize), currentMaskState)
			Else
				Throw New System.ArgumentException("Received mask array with shape " & Arrays.toString(maskArray.shape()) & "; expected vector.")
			End If
		End Function
	End Class

End Namespace