Imports System
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports BaseInputPreProcessor = org.deeplearning4j.nn.conf.preprocessor.BaseInputPreProcessor
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
'ORIGINAL LINE: @Slf4j @Data public class KerasFlattenRnnPreprocessor extends org.deeplearning4j.nn.conf.preprocessor.BaseInputPreProcessor
	<Serializable>
	Public Class KerasFlattenRnnPreprocessor
		Inherits BaseInputPreProcessor

		Private tsLength As Long
		Private depth As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public KerasFlattenRnnPreprocessor(@JsonProperty("depth") long depth, @JsonProperty("tsLength") long tsLength)
		Public Sub New(ByVal depth As Long, ByVal tsLength As Long)
			MyBase.New()
			Me.tsLength = Math.Abs(tsLength)
			Me.depth = depth
		End Sub

		Public Overrides Function preProcess(ByVal input As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim output As INDArray = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "c"c)
			Return output.reshape(ChrW(input.size(0)), depth * tsLength)
		End Function

		Public Overrides Function backprop(ByVal epsilons As INDArray, ByVal miniBatchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilons, "c"c).reshape(ChrW(miniBatchSize), depth, tsLength)
		End Function

		Public Overrides Function clone() As KerasFlattenRnnPreprocessor
			Return CType(MyBase.clone(), KerasFlattenRnnPreprocessor)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType inputType) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
		Public Overrides Function getOutputType(ByVal inputType As InputType) As InputType

			Return InputType.feedForward(depth * tsLength)

		End Function
	End Class

End Namespace