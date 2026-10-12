Imports System
Imports System.Collections.Generic
Imports StringUtils = org.apache.commons.lang3.StringUtils

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

Namespace org.deeplearning4j.text.movingwindow



	<Serializable>
	Public Class Window
		''' 
		Private Const serialVersionUID As Long = 6359906393699230579L
'JAVA TO VB CONVERTER NOTE: The field words was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private words_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field label was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private label_Conflict As String = "NONE"
'JAVA TO VB CONVERTER NOTE: The field beginLabel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private beginLabel_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field endLabel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private endLabel_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field windowSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private windowSize_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field median was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private median_Conflict As Integer
		Private Shared BEGIN_LABEL As String = "<([A-Z]+|\d+)>"
		Private Shared END_LABEL As String = "</([A-Z]+|\d+)>"
'JAVA TO VB CONVERTER NOTE: The field begin was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field end was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private begin_Conflict, end_Conflict As Integer

		''' <summary>
		''' Creates a window with a context of size 3 </summary>
		''' <param name="words"> a collection of strings of size 3 </param>
		Public Sub New(ByVal words As ICollection(Of String), ByVal begin As Integer, ByVal [end] As Integer)
			Me.New(words, 5, begin, [end])

		End Sub

		Public Overridable Function asTokens() As String
			Return StringUtils.join(words_Conflict, " ")
		End Function


		''' <summary>
		''' Initialize a window with the given size </summary>
		''' <param name="words"> the words to use </param>
		''' <param name="windowSize"> the size of the window </param>
		''' <param name="begin"> the begin index for the window </param>
		''' <param name="end"> the end index for the window </param>
		Public Sub New(ByVal words As ICollection(Of String), ByVal windowSize As Integer, ByVal begin As Integer, ByVal [end] As Integer)
			If words Is Nothing Then
				Throw New System.ArgumentException("Words must be a list of size 3")
			End If

			Me.words_Conflict = New List(Of String)(words)
			Me.windowSize_Conflict = windowSize
			Me.begin_Conflict = begin
			Me.end_Conflict = [end]
			initContext()
		End Sub


		Private Sub initContext()
			Dim median As Integer = CInt(Math.Floor(words_Conflict.Count \ 2))
			Dim begin As IList(Of String) = words_Conflict.subList(0, median)
			Dim after As IList(Of String) = words_Conflict.subList(median + 1, words_Conflict.Count)


			For Each s As String In begin
				If s.matches(BEGIN_LABEL) Then
					Me.label_Conflict = s.replaceAll("(<|>)", "").Replace("/", "")
					beginLabel_Conflict = True
				ElseIf s.matches(END_LABEL) Then
					endLabel_Conflict = True
					Me.label_Conflict = s.replaceAll("(<|>|/)", "").Replace("/", "")

				End If

			Next s

			For Each s1 As String In after

				If s1.matches(BEGIN_LABEL) Then
					Me.label_Conflict = s1.replaceAll("(<|>)", "").Replace("/", "")
					beginLabel_Conflict = True
				End If

				If s1.matches(END_LABEL) Then
					endLabel_Conflict = True
					Me.label_Conflict = s1.replaceAll("(<|>)", "")

				End If
			Next s1
			Me.median_Conflict = median

		End Sub



		Public Overrides Function ToString() As String
			Return words_Conflict.ToString()
		End Function

		Public Overridable Property Words As IList(Of String)
			Get
				Return words_Conflict
			End Get
			Set(ByVal words As IList(Of String))
				Me.words_Conflict = words
			End Set
		End Property


		Public Overridable Function getWord(ByVal i As Integer) As String
			Return words(i)
		End Function

		Public Overridable ReadOnly Property FocusWord As String
			Get
				Return words(median_Conflict)
			End Get
		End Property

		Public Overridable ReadOnly Property BeginLabel As Boolean
			Get
				Return Not label_Conflict.Equals("NONE") AndAlso beginLabel_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property EndLabel As Boolean
			Get
				Return Not label_Conflict.Equals("NONE") AndAlso endLabel_Conflict
			End Get
		End Property

		Public Overridable Property Label As String
			Get
				Return label_Conflict.Replace("/", "")
			End Get
			Set(ByVal label As String)
				Me.label_Conflict = label
			End Set
		End Property

		Public Overridable ReadOnly Property WindowSize As Integer
			Get
				Return words_Conflict.Count
			End Get
		End Property

		Public Overridable ReadOnly Property Median As Integer
			Get
				Return median_Conflict
			End Get
		End Property


		Public Overridable Property Begin As Integer
			Get
				Return begin_Conflict
			End Get
			Set(ByVal begin As Integer)
				Me.begin_Conflict = begin
			End Set
		End Property


		Public Overridable Property End As Integer
			Get
				Return end_Conflict
			End Get
			Set(ByVal [end] As Integer)
				Me.end_Conflict = [end]
			End Set
		End Property



	End Class

End Namespace