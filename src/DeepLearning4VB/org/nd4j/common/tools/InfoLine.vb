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



	Public Class InfoLine
		'
		Public Sub New()
			'
		End Sub
		'
		Public ivL As IList(Of InfoValues) = New List(Of InfoValues)()
		'

		''' <summary>
		''' Returns titles line as string appointed by title index (0..5).<br>
		''' Columns are separated with char '|'.<br>
		''' If title index is < 0 returns "?".<br> 
		''' If title index is > 5 returns "?".<br> </summary>
		''' <param name="mtLv"> - method level </param>
		''' <param name="title_I"> - title index </param>
		''' <returns> titles line as string </returns>
		Public Overridable Function getTitleLine(ByVal mtLv As Integer, ByVal title_I As Integer) As String
			'
			Dim info As String = ""
			'
			If title_I < 0 Then
				Return "?"
			End If
			If title_I > 5 Then
				Return "?"
			End If
			'
			info = ""
			info &= BTools.getMtLvESS(mtLv)
			info &= BTools.MtLvISS
			info &= "|"
			'
			Dim i_IV As InfoValues
			'
			Dim i_ValuesS As String = ""
			'
			Dim i_VSLen As Integer = -1
			'
			Dim i_TitleS As String = ""
			'
			For i As Integer = 0 To ivL.Count - 1
				'
				i_IV = ivL(i)
				'
				i_ValuesS = i_IV.Values
				'
				i_VSLen = i_ValuesS.Length
				'
				i_TitleS = If(title_I < i_IV.titleA.Length, i_IV.titleA(title_I), "")
				'
				i_TitleS = i_TitleS & BTools.getSpaces(i_VSLen)
				'
				info &= i_TitleS.Substring(0, i_VSLen - 1)
				'
				info &= "|"
			Next i
			'
			Return info
		End Function

		''' <summary>
		''' Returns values line as string.<br>
		''' Columns are separated with char '|'.<br> </summary>
		''' <param name="mtLv"> - method level </param>
		''' <returns> values line as string </returns>
		Public Overridable Function getValuesLine(ByVal mtLv As Integer) As String
			'
			Dim info As String = ""
			'
			info &= BTools.getMtLvESS(mtLv)
			info &= BTools.MtLvISS
			info &= "|"
			'
			Dim i_IV As InfoValues
			'
			For i As Integer = 0 To ivL.Count - 1
				'
				i_IV = ivL(i)
				'
				info &= i_IV.Values
			Next i
			'
			Return info
		End Function



	End Class
End Namespace