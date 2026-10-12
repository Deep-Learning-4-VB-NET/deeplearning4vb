Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InputTypeRecurrent = org.deeplearning4j.nn.conf.inputs.InputType.InputTypeRecurrent
Imports Type = org.deeplearning4j.nn.conf.inputs.InputType.Type
Imports SameDiffLambdaLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff

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

Namespace org.deeplearning4j.nn.conf.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class CapsuleStrengthLayer extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
	<Serializable>
	Public Class CapsuleStrengthLayer
		Inherits SameDiffLambdaLayer

		Public Sub New(ByVal builder As Builder)
			MyBase.New()
		End Sub

		Public Overrides Function defineLayer(ByVal SD As SameDiff, ByVal layerInput As SDVariable) As SDVariable
			Return SD.norm2("caps_strength", layerInput, 2)
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType

			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input for Capsule Strength layer (layer name = """ & layerName & """): expect RNN input.  Got: " & inputType)
			End If

			Dim ri As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Return InputType.feedForward(ri.getSize())
		End Function

		Public Class Builder
			Inherits SameDiffLambdaLayer.Builder(Of Builder)

			Public Overrides Function build(Of E As Layer)() As E
				Return CType(New CapsuleStrengthLayer(Me), E)
			End Function
		End Class
	End Class

End Namespace