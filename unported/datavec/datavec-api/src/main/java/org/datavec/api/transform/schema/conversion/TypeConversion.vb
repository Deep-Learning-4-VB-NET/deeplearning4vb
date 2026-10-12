Imports System
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.schema.conversion

	Public Class TypeConversion

		Private Shared SINGLETON As New TypeConversion()

		Private Sub New()
		End Sub

		Public Shared ReadOnly Property Instance As TypeConversion
			Get
				Return SINGLETON
			End Get
		End Property

		Public Overridable Function convertInt(ByVal o As Object) As Integer
			If TypeOf o Is Writable Then
				Dim writable As Writable = DirectCast(o, Writable)
				Return convertInt(writable)
			Else
				Return convertInt(o.ToString())
			End If
		End Function



		Public Overridable Function convertInt(ByVal writable As Writable) As Integer
			Return writable.toInt()
		End Function

		Public Overridable Function convertInt(ByVal o As String) As Integer
			Return CInt(Math.Truncate(Double.Parse(o)))
		End Function

		Public Overridable Function convertDouble(ByVal writable As Writable) As Double
			Return writable.toDouble()
		End Function

		Public Overridable Function convertDouble(ByVal o As String) As Double
			Return Double.Parse(o)
		End Function

		Public Overridable Function convertDouble(ByVal o As Object) As Double
			If TypeOf o Is Writable Then
				Dim writable As Writable = DirectCast(o, Writable)
				Return convertDouble(writable)
			Else
				Return convertDouble(o.ToString())
			End If
		End Function


		Public Overridable Function convertFloat(ByVal writable As Writable) As Single
			Return writable.toFloat()
		End Function

		Public Overridable Function convertFloat(ByVal o As String) As Single
			Return Single.Parse(o)
		End Function

		Public Overridable Function convertFloat(ByVal o As Object) As Single
			If TypeOf o Is Writable Then
				Dim writable As Writable = DirectCast(o, Writable)
				Return convertFloat(writable)
			Else
				Return convertFloat(o.ToString())
			End If
		End Function

		Public Overridable Function convertLong(ByVal writable As Writable) As Long
			Return writable.toLong()
		End Function


		Public Overridable Function convertLong(ByVal o As String) As Long
			Return Long.Parse(o)
		End Function

		Public Overridable Function convertLong(ByVal o As Object) As Long
			If TypeOf o Is Writable Then
				Dim writable As Writable = DirectCast(o, Writable)
				Return convertLong(writable)
			Else
				Return convertLong(o.ToString())
			End If
		End Function

		Public Overridable Function convertString(ByVal writable As Writable) As String
			Return writable.ToString()
		End Function

		Public Overridable Function convertString(ByVal s As String) As String
			Return s
		End Function

		Public Overridable Function convertString(ByVal o As Object) As String
			If TypeOf o Is Writable Then
				Dim writable As Writable = DirectCast(o, Writable)
				Return convertString(writable)
			Else
				Return convertString(o.ToString())
			End If
		End Function

	End Class

End Namespace