Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports BaseInputPreProcessor = org.deeplearning4j.nn.conf.preprocessor.BaseInputPreProcessor
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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

Namespace org.deeplearning4j.nn.modelimport.keras.preprocessors


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j @EqualsAndHashCode(callSuper = false) @JsonIgnoreProperties({"hasLeadingDimension"}) public class PermutePreprocessor extends org.deeplearning4j.nn.conf.preprocessor.BaseInputPreProcessor
	<Serializable>
	Public Class PermutePreprocessor
		Inherits BaseInputPreProcessor

		Private permutationIndices() As Integer
		Private hasLeadingDimension As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PermutePreprocessor(@JsonProperty("permutationIndices") int... permutationIndices)
		Public Sub New(ParamArray ByVal permutationIndices() As Integer)
			Me.permutationIndices = permutationIndices
		End Sub


		Private Shared Function prependZero(ByVal shape() As Integer) As Integer()
			Dim shapeLength As Integer = shape.Length
			Dim augmentedShape(shapeLength) As Integer
			For i As Integer = 0 To augmentedShape.Length - 1
				If i = 0 Then
					augmentedShape(i) = 0
				Else
					augmentedShape(i) = shape(i - 1)
				End If
			Next i
			Return augmentedShape
		End Function

		Public Overrides Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If permutationIndices.Length + 1 = input.shape().Length Then
				permutationIndices = prependZero(permutationIndices)
				Me.hasLeadingDimension = True
			End If
			If input.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(input) Then
				input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "c"c)
			End If
			Dim output As INDArray = workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input.permute(Me.permutationIndices))
			Return output
		End Function

		Public Overrides Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If output.ordering() <> "c"c OrElse Not Shape.hasDefaultStridesForShape(output) Then
				output = workspaceMgr.dup(ArrayType.ACTIVATIONS, output, "c"c)
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, output.permute(permutationIndices))

		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType inputType) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal inputType As InputType) As InputType
			If TypeOf inputType Is InputType.InputTypeConvolutional Then
				Dim it As InputType.InputTypeConvolutional = DirectCast(inputType, InputType.InputTypeConvolutional)
				Return InputType.convolutional(it.getWidth(), it.getHeight(), it.getChannels())
			ElseIf TypeOf inputType Is InputType.InputTypeRecurrent Then
				Dim it As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
				Return InputType.recurrent(it.getTimeSeriesLength(), it.getSize())
			ElseIf TypeOf inputType Is InputType.InputTypeFeedForward OrElse TypeOf inputType Is InputType.InputTypeConvolutional3D Then
				Return inputType
			Else
				Throw New InvalidInputTypeException("Unsupported Input type " & inputType)
			End If
		End Function
	End Class
End Namespace