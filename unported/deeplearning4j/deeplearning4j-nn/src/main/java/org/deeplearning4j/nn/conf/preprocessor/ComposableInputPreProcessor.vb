Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = false) public class ComposableInputPreProcessor extends BaseInputPreProcessor
	<Serializable>
	Public Class ComposableInputPreProcessor
		Inherits BaseInputPreProcessor

		Private inputPreProcessors() As InputPreProcessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonCreator public ComposableInputPreProcessor(@JsonProperty("inputPreProcessors") org.deeplearning4j.nn.conf.InputPreProcessor... inputPreProcessors)
		Public Sub New(ParamArray ByVal inputPreProcessors() As InputPreProcessor)
			Me.inputPreProcessors = inputPreProcessors
		End Sub

		Public Overrides Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			For Each preProcessor As InputPreProcessor In inputPreProcessors
				input = preProcessor.preProcess(input, miniBatchSize, workspaceMgr)
			Next preProcessor
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input)
		End Function

		Public Overrides Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			'Apply input preprocessors in opposite order for backprop (compared to forward pass)
			'For example, CNNtoFF + FFtoRNN, need to do backprop in order of FFtoRNN + CNNtoFF
			For i As Integer = inputPreProcessors.Length - 1 To 0 Step -1
				output = inputPreProcessors(i).backprop(output, miniBatchSize, workspaceMgr)
			Next i
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, output)
		End Function

		Public Overrides Function clone() As ComposableInputPreProcessor
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As ComposableInputPreProcessor = CType(MyBase.clone(), ComposableInputPreProcessor)
			If clone_Conflict.inputPreProcessors IsNot Nothing Then
				Dim processors(clone_Conflict.inputPreProcessors.Length - 1) As InputPreProcessor
				For i As Integer = 0 To clone_Conflict.inputPreProcessors.Length - 1
					processors(i) = clone_Conflict.inputPreProcessors(i).clone()
				Next i
				clone_Conflict.inputPreProcessors = processors
			End If
			Return clone_Conflict
		End Function

		Public Overrides Function getOutputType(ByVal inputType As InputType) As InputType
			For Each p As InputPreProcessor In inputPreProcessors
				inputType = p.getOutputType(inputType)
			Next p
			Return inputType
		End Function

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			For Each preproc As InputPreProcessor In inputPreProcessors
				Dim p As Pair(Of INDArray, MaskState) = preproc.feedForwardMaskArray(maskArray, currentMaskState, minibatchSize)
				maskArray = p.First
				currentMaskState = p.Second
			Next preproc
			Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
		End Function
	End Class

End Namespace