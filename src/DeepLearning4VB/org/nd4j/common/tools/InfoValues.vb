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

Namespace org.nd4j.common.tools


	Public Class InfoValues
		'
		Public Sub New(ParamArray ByVal titleA() As String)
			'
			For i As Integer = 0 To Me.titleA.Length - 1
				Me.titleA(i) = ""
			Next i
			'
			Dim Max_K As Integer = Math.Min(Me.titleA.Length - 1, titleA.Length - 1)
			'
			If Max_K + 1 >= 0 Then
				Array.Copy(titleA, 0, Me.titleA, 0, Max_K + 1)
			End If
			'
		End Sub
		'
		''' <summary>
		''' Title array.<br>
		''' </summary>
		Public titleA(5) As String
		'
		' VS = Values String
		''' <summary>
		''' Values string list.<br>
		''' </summary>
		Public vsL As IList(Of String) = New List(Of String)()
		'

		''' <summary>
		''' Returns values.<br>
		''' This method use class InfoLine.<br>
		''' This method is not intended for external use.<br>
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Values As String
			Get
				'
				Dim info As String = ""
				'
				For i As Integer = 0 To vsL.Count - 1
					'
					info &= vsL(i) & "|"
				Next i
				'
				Return info
			End Get
		End Property

	End Class
End Namespace