Imports System
Imports lombok
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer

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

	''' <summary>
	''' Upsampling base layer
	''' 
	''' @author Max Pumperla
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public abstract class BaseUpsamplingLayer extends NoParamLayer
	<Serializable>
	Public MustInherit Class BaseUpsamplingLayer
		Inherits NoParamLayer

		Protected Friend size() As Integer

		Protected Friend Sub New(ByVal builder As UpsamplingBuilder)
			MyBase.New(builder)
			Me.size = builder.size
		End Sub

		Public Overrides Function clone() As BaseUpsamplingLayer
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As BaseUpsamplingLayer = CType(MyBase.clone(), BaseUpsamplingLayer)
			Return clone_Conflict
		End Function

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input for Upsampling layer (layer name=""" & getLayerName() & """): input is null")
			End If
			Return InputTypeUtil.getPreProcessorForInputTypeCnnLayers(inputType, getLayerName())
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter protected static abstract class UpsamplingBuilder<T extends UpsamplingBuilder<T>> extends Builder<T>
		Protected Friend MustInherit Class UpsamplingBuilder(Of T As UpsamplingBuilder(Of T))
			Inherits Builder(Of T)

			''' <summary>
			''' An int array to specify upsampling dimensions, the length of which has to equal to the number of spatial
			''' dimensions (e.g. 2 for Upsampling2D etc.)
			''' 
			''' </summary>
			Protected Friend size() As Integer = {1}

			''' <summary>
			''' A single size integer is used for upsampling in all spatial dimensions
			''' </summary>
			''' <param name="size"> int for upsampling </param>
			Protected Friend Sub New(ByVal size As Integer)
				Me.setSize(New Integer() {size})
			End Sub

			''' <summary>
			''' An int array to specify upsampling dimensions, the length of which has to equal to the number of spatial
			''' dimensions (e.g. 2 for Upsampling2D etc.)
			''' </summary>
			''' <param name="size"> int for upsampling </param>
			Protected Friend Sub New(ByVal size() As Integer)
				Me.setSize(size)
			End Sub
		End Class

	End Class

End Namespace