Imports System
Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports EmptyParamInitializer = org.deeplearning4j.nn.params.EmptyParamInitializer
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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
'ORIGINAL LINE: @NoArgsConstructor public abstract class NoParamLayer extends Layer
	<Serializable>
	Public MustInherit Class NoParamLayer
		Inherits Layer

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
		End Sub

		Public Overrides Function initializer() As ParamInitializer
			Return EmptyParamInitializer.Instance
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op in most no param layers
		End Sub

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			'No parameters -> no regularization of parameters
			Return Nothing
		End Function

		Public Overrides ReadOnly Property GradientNormalization As GradientNormalization
			Get
				Return GradientNormalization.None
			End Get
		End Property

		Public Overrides ReadOnly Property GradientNormalizationThreshold As Double
			Get
				Return 0
			End Get
		End Property

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Throw New System.NotSupportedException(Me.GetType().Name & " does not contain parameters")
		End Function
	End Class

End Namespace