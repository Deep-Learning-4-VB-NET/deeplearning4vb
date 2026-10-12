Imports System
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives

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

	''' <summary>
	''' @author Adam Gibson
	''' </summary>

	<Serializable>
	Public MustInherit Class BaseInputPreProcessor
		Implements InputPreProcessor

		Public MustOverride Function getOutputType(ByVal inputType As org.deeplearning4j.nn.conf.inputs.InputType) As org.deeplearning4j.nn.conf.inputs.InputType Implements InputPreProcessor.getOutputType
		Public MustOverride Function backprop(ByVal output As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As org.deeplearning4j.nn.workspace.LayerWorkspaceMgr) As INDArray
		Public MustOverride Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As org.deeplearning4j.nn.workspace.LayerWorkspaceMgr) As INDArray
		Public Overridable Function clone() As BaseInputPreProcessor
			Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim clone_Conflict As BaseInputPreProcessor = CType(MyBase.clone(), BaseInputPreProcessor)
				Return clone_Conflict
			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function


		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements InputPreProcessor.feedForwardMaskArray
			'Default: pass-through, unmodified
			Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
		End Function
	End Class

End Namespace