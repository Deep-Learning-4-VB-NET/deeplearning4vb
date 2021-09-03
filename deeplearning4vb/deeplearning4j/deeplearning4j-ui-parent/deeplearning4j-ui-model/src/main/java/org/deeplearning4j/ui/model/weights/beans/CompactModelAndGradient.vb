Imports System
Imports System.Collections.Generic

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

Namespace org.deeplearning4j.ui.model.weights.beans



	''' <summary>
	''' Slightly modified version of ModelAndGradient, with binned params/gradients, suitable for fast network transfers for HistogramIterationListener
	''' 
	''' @author Adam Gibson
	''' </summary>

	<Serializable>
	Public Class CompactModelAndGradient
'JAVA TO VB CONVERTER NOTE: The field lastUpdateTime was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lastUpdateTime_Conflict As Long = -1L
'JAVA TO VB CONVERTER NOTE: The field parameters was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private parameters_Conflict As IDictionary(Of String, System.Collections.IDictionary)
'JAVA TO VB CONVERTER NOTE: The field gradients was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private gradients_Conflict As IDictionary(Of String, System.Collections.IDictionary)
'JAVA TO VB CONVERTER NOTE: The field score was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private score_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field scores was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private scores_Conflict As IList(Of Double) = New List(Of Double)()
'JAVA TO VB CONVERTER NOTE: The field updateMagnitudes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private updateMagnitudes_Conflict As IList(Of IDictionary(Of String, IList(Of Double))) = New List(Of IDictionary(Of String, IList(Of Double)))()
'JAVA TO VB CONVERTER NOTE: The field paramMagnitudes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private paramMagnitudes_Conflict As IList(Of IDictionary(Of String, IList(Of Double))) = New List(Of IDictionary(Of String, IList(Of Double)))()
'JAVA TO VB CONVERTER NOTE: The field layerNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private layerNames_Conflict As IList(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER NOTE: The field path was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private path_Conflict As String


		Public Sub New()
			parameters_Conflict = New Dictionary(Of String, System.Collections.IDictionary)()
			gradients_Conflict = New Dictionary(Of String, System.Collections.IDictionary)()
		End Sub


		Public Overridable Property LastUpdateTime As Long
			Set(ByVal lastUpdateTime As Long)
				Me.lastUpdateTime_Conflict = lastUpdateTime
			End Set
			Get
				Return lastUpdateTime_Conflict
			End Get
		End Property


		Public Overridable Property Score As Double
			Get
				Return score_Conflict
			End Get
			Set(ByVal score As Double)
				Me.score_Conflict = score
			End Set
		End Property



		Public Overridable Property Parameters As IDictionary(Of String, System.Collections.IDictionary)
			Get
				Return parameters_Conflict
			End Get
			Set(ByVal parameters As IDictionary(Of String, System.Collections.IDictionary))
				Me.parameters_Conflict = parameters
			End Set
		End Property



		Public Overridable Property Gradients As IDictionary(Of String, System.Collections.IDictionary)
			Get
				Return gradients_Conflict
			End Get
			Set(ByVal gradients As IDictionary(Of String, System.Collections.IDictionary))
				Me.gradients_Conflict = gradients
			End Set
		End Property


		Public Overridable Property Scores As IList(Of Double)
			Set(ByVal scores As IList(Of Double))
				Me.scores_Conflict = scores
			End Set
			Get
				Return scores_Conflict
			End Get
		End Property

		Public Overridable Property Path As String
			Set(ByVal path As String)
				Me.path_Conflict = path
			End Set
			Get
				Return path_Conflict
			End Get
		End Property



		Public Overridable Property UpdateMagnitudes As IList(Of IDictionary(Of String, IList(Of Double)))
			Set(ByVal updateMagnitudes As IList(Of IDictionary(Of String, IList(Of Double))))
				Me.updateMagnitudes_Conflict = updateMagnitudes
			End Set
			Get
				Return updateMagnitudes_Conflict
			End Get
		End Property


		Public Overridable Property ParamMagnitudes As IList(Of IDictionary(Of String, IList(Of Double)))
			Set(ByVal paramMagnitudes As IList(Of IDictionary(Of String, IList(Of Double))))
				Me.paramMagnitudes_Conflict = paramMagnitudes
			End Set
			Get
				Return paramMagnitudes_Conflict
			End Get
		End Property


		Public Overridable Property LayerNames As IList(Of String)
			Set(ByVal layerNames As IList(Of String))
				Me.layerNames_Conflict = layerNames
			End Set
			Get
				Return layerNames_Conflict
			End Get
		End Property


		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As CompactModelAndGradient = DirectCast(o, CompactModelAndGradient)

			If that.score_Conflict.CompareTo(score_Conflict) <> 0 Then
				Return False
			End If
			If If(parameters_Conflict IsNot Nothing, Not parameters_Conflict.Equals(that.parameters_Conflict), that.parameters_Conflict IsNot Nothing) Then
				Return False
			End If
			Return Not (If(gradients_Conflict IsNot Nothing, Not gradients_Conflict.Equals(that.gradients_Conflict), that.gradients_Conflict IsNot Nothing))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer
			Dim temp As Long
			result = If(parameters_Conflict IsNot Nothing, parameters_Conflict.GetHashCode(), 0)
			result = 31 * result + (If(gradients_Conflict IsNot Nothing, gradients_Conflict.GetHashCode(), 0))
			temp = System.BitConverter.DoubleToInt64Bits(score_Conflict)
			result = 31 * result + CInt(temp Xor (CLng(CULng(temp) >> 32)))
			Return result
		End Function
	End Class

End Namespace