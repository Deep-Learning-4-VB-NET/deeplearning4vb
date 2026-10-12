Imports System
Imports System.Text
Imports Microsoft.VisualBasic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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

Namespace org.nd4j.linalg.string


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public class NDArrayStrings
	Public Class NDArrayStrings

		Public Const EMPTY_ARRAY_STR As String = "[]"

		Private Shared ReadOnly OPEN_BRACKETS() As String = {"", "[", "[[", "[[[", "[[[[", "[[[[[", "[[[[[[", "[[[[[[[", "[[[[[[[["}
		Private Shared ReadOnly CLOSE_BRACKETS() As String = {"", "]", "]]", "]]]", "]]]]", "]]]]]", "]]]]]]", "]]]]]]]", "]]]]]]]]"}

		''' <summary>
		''' The default number of elements for printing INDArrays (via NDArrayStrings or INDArray.toString)
		''' </summary>
		Public Const DEFAULT_MAX_PRINT_ELEMENTS As Long = 1000
		''' <summary>
		''' The maximum number of elements to print by default for INDArray.toString()
		''' Default value is 1000 - given by <seealso cref="DEFAULT_MAX_PRINT_ELEMENTS"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter private static long maxPrintElements = DEFAULT_MAX_PRINT_ELEMENTS;
		Private Shared maxPrintElements As Long = DEFAULT_MAX_PRINT_ELEMENTS

		Private localMaxPrintElements As Long = maxPrintElements
		Private colSep As String = ","
		Private newLineSep As String = ","
		Private padding As Integer = 7
		Private precision As Integer = 4
		Private minToPrintWithoutSwitching As Double
		Private maxToPrintWithoutSwitching As Double
		Private scientificFormat As String = ""
		Private decimalFormat As DecimalFormat
		Private dontOverrideFormat As Boolean = False

		Public Sub New()
			Me.New(",", 4)
		End Sub

		Public Sub New(ByVal colSep As String)
			Me.New(colSep, 4)
		End Sub

		''' <summary>
		''' Specify the number of digits after the decimal point to include </summary>
		''' <param name="precision"> </param>
		Public Sub New(ByVal precision As Integer)
			Me.New(",", precision)
		End Sub

		Public Sub New(ByVal maxElements As Long, ByVal precision As Integer)
			Me.New(",", precision)
			Me.localMaxPrintElements = maxElements
		End Sub

		Public Sub New(ByVal maxElements As Long)
			Me.New()
			Me.localMaxPrintElements = maxElements
		End Sub

		Public Sub New(ByVal maxElements As Long, ByVal forceSummarize As Boolean, ByVal precision As Integer)
			Me.New(",", precision)
			If forceSummarize Then
				localMaxPrintElements = 0
			Else
				localMaxPrintElements = maxElements
			End If
		End Sub

		Public Sub New(ByVal forceSummarize As Boolean, ByVal precision As Integer)
			Me.New(",", precision)
			If forceSummarize Then
				localMaxPrintElements = 0
			End If
		End Sub



		Public Sub New(ByVal forceSummarize As Boolean)
			Me.New(",", 4)
			If forceSummarize Then
				localMaxPrintElements = 0
			End If
		End Sub


		''' <summary>
		''' Specify a delimiter for elements in columns for 2d arrays (or in the rank-1th dimension in higher order arrays)
		''' Separator in elements in remaining dimensions defaults to ",\n"
		''' </summary>
		''' <param name="colSep">    field separating columns; </param>
		''' <param name="precision"> digits after decimal point </param>
		Public Sub New(ByVal colSep As String, ByVal precision As Integer)
			Me.colSep = colSep
			If Not colSep.replaceAll("\s", "").Equals(",") Then
				Me.newLineSep = ""
			End If
			Dim decFormatNum As New StringBuilder("0.")

			Dim prec As Integer = Math.Abs(precision)
			Me.precision = prec
			Dim useHash As Boolean = precision < 0

			Do While prec > 0
				decFormatNum.Append(If(useHash, "#", "0"))
				prec -= 1
			Loop
			Me.decimalFormat = localeIndifferentDecimalFormat(decFormatNum.ToString())
		End Sub

		''' <summary>
		''' Specify a col separator and a decimal format string </summary>
		''' <param name="colSep"> </param>
		''' <param name="decFormat"> </param>
		Public Sub New(ByVal colSep As String, ByVal decFormat As String)
			Me.colSep = colSep
			Me.decimalFormat = localeIndifferentDecimalFormat(decFormat)
			If decFormat.ToUpper().Contains("E") Then
				Me.padding = decFormat.Length + 3
			Else
				Me.padding = decFormat.Length + 1
			End If
			Me.dontOverrideFormat = True
		End Sub

		''' 
		''' <param name="arr"> </param>
		''' <returns> String representation of the array adhering to options provided in the constructor </returns>
		Public Overridable Function format(ByVal arr As INDArray) As String
			Return format(arr, True)
		End Function

		''' <summary>
		''' Format the given ndarray as a string
		''' </summary>
		''' <param name="arr">       the array to format </param>
		''' <param name="summarize"> If true and the number of elements in the array is greater than > 1000 only the first three and last elements in any dimension will print </param>
		''' <returns> the formatted array </returns>
		Public Overridable Function format(ByVal arr As INDArray, ByVal summarize As Boolean) As String
			If arr.Empty Then
				Return EMPTY_ARRAY_STR
			End If
			Me.scientificFormat = "0."
			Dim addPrecision As Integer = Me.precision
			Do While addPrecision > 0
				Me.scientificFormat &= "#"
				addPrecision -= 1
			Loop
			Me.scientificFormat = Me.scientificFormat & "E0"
			If Me.scientificFormat.Length + 2 > Me.padding Then
				Me.padding = Me.scientificFormat.Length + 2
			End If
			Me.maxToPrintWithoutSwitching = Math.Pow(10,Me.precision)
			Me.minToPrintWithoutSwitching = 1.0/(Me.maxToPrintWithoutSwitching)
			Return format(arr, 0, summarize AndAlso arr.length() > localMaxPrintElements)
		End Function

		Private Function format(ByVal arr As INDArray, ByVal offset As Integer, ByVal summarize As Boolean) As String
			Dim rank As Integer = arr.rank()
			If arr.Scalar OrElse arr.length() = 1 Then
				Dim fRank As Integer = Math.Min(rank, OPEN_BRACKETS.Length-1)
				If arr.R Then
					Dim arrElement As Double = arr.getDouble(0)
					If Not dontOverrideFormat AndAlso ((Math.Abs(arrElement) < Me.minToPrintWithoutSwitching AndAlso arrElement <> 0) OrElse (Math.Abs(arrElement) >= Me.maxToPrintWithoutSwitching)) Then
						'switch to scientific notation
						Dim asString As String = localeIndifferentDecimalFormat(scientificFormat).format(arrElement)
						'from E to small e
						asString = asString.Replace("E"c, "e"c)
						Return OPEN_BRACKETS(fRank) + asString & CLOSE_BRACKETS(fRank)
					Else
						If arr.getDouble(0) = 0 Then
							Return OPEN_BRACKETS(fRank) & "0" & CLOSE_BRACKETS(fRank)
						End If
						Return OPEN_BRACKETS(fRank) + decimalFormat.format(arr.getDouble(0)) & CLOSE_BRACKETS(fRank)
					End If
				ElseIf arr.Z Then
					Dim arrElement As Long = arr.getLong(0)
					Return OPEN_BRACKETS(fRank) + arrElement & CLOSE_BRACKETS(fRank)
				ElseIf arr.B Then
					Dim arrElement As Long = arr.getLong(0)
					Return OPEN_BRACKETS(fRank) + (If(arrElement = 0, "false", "true")) & CLOSE_BRACKETS(fRank)
				ElseIf arr.dataType() = DataType.UTF8 Then
					Dim s As String = arr.getString(0)
					Return OPEN_BRACKETS(fRank) & """" & s.replaceAll(vbLf,"\n") & """" & CLOSE_BRACKETS(fRank)
				Else
					Throw New ND4JIllegalStateException()
				End If
			ElseIf rank = 1 Then
				'true vector
				Return vectorToString(arr, summarize)
			ElseIf arr.RowVector Then
				'a slice from a higher dim array
				If offset = 0 Then
					Dim sb As New StringBuilder()
					sb.Append("[")
					sb.Append(vectorToString(arr, summarize))
					sb.Append("]")
					Return sb.ToString()
				End If
				Return vectorToString(arr, summarize)
			Else
				offset += 1
				Dim sb As New StringBuilder()
				sb.Append("[")
				Dim nSlices As Long = arr.slices()
				For i As Integer = 0 To nSlices - 1
					If summarize AndAlso i > 2 AndAlso i < nSlices - 3 Then
						sb.Append(" ...")
						sb.Append(newLineSep).Append(" " & vbLf)
						sb.Append(StringUtils.repeat(vbLf, rank - 2))
						sb.Append(StringUtils.repeat(" ", offset))
						' immediately jump to the last slices so we only print ellipsis once
						i = Math.Max(i, CInt(nSlices) - 4)
					Else
						If arr.rank() = 3 AndAlso arr.slice(i).RowVector Then
							sb.Append("[")
						End If
						'hack fix for slice issue with 'f' order
						If arr.ordering() = "f"c AndAlso arr.rank() > 2 AndAlso arr.size(arr.rank() - 1) = 1 Then
							sb.Append(format(arr.dup("c"c).slice(i), offset, summarize))
	'                    else if(arr.rank() <= 1 || arr.length() == 1) {
	'                        sb.append(format(Nd4j.scalar(arr.getDouble(0)),offset,summarize));
	'                    }
						Else
							sb.Append(format(arr.slice(i), offset, summarize))
						End If
						If i <> nSlices - 1 Then
							If arr.rank() = 3 AndAlso arr.slice(i).RowVector Then
								sb.Append("]")
							End If
							sb.Append(newLineSep).Append(" " & vbLf)
							sb.Append(StringUtils.repeat(vbLf, rank - 2))
							sb.Append(StringUtils.repeat(" ", offset))
						Else
							If arr.rank() = 3 AndAlso arr.slice(i).RowVector Then
								sb.Append("]")
							End If
						End If
					End If
				Next i
				sb.Append("]")
				Return sb.ToString()
			End If
		End Function

		Private Function vectorToString(ByVal arr As INDArray, ByVal summarize As Boolean) As String
			Dim sb As New StringBuilder()
			sb.Append("[")
			Dim l As Long = arr.length()
			For i As Integer = 0 To l - 1
				If summarize AndAlso i > 2 AndAlso i < l - 3 Then
					sb.Append("  ...")
					' immediately jump to the last elements so we only print ellipsis once
					i = Math.Max(i, CInt(l) - 4)
				Else
					If arr.R Then
						Dim arrElement As Double = arr.getDouble(i)
						If Not dontOverrideFormat AndAlso ((Math.Abs(arrElement) < Me.minToPrintWithoutSwitching AndAlso arrElement <> 0) OrElse (Math.Abs(arrElement) >= Me.maxToPrintWithoutSwitching)) Then
							'switch to scientific notation
							Dim asString As String = localeIndifferentDecimalFormat(scientificFormat).format(arrElement)
							'from E to small e
							asString = asString.Replace("E"c, "e"c)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
							sb.Append(String.Format("%1$" & padding & "s", asString))
						Else
							If arrElement = 0 Then
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
								sb.Append(String.Format("%1$" & padding & "s", 0))
							Else
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
								sb.Append(String.Format("%1$" & padding & "s", decimalFormat.format(arrElement)))
							End If
						End If
					ElseIf arr.Z Then
						Dim arrElement As Long = arr.getLong(i)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
						sb.Append(String.Format("%1$" & padding & "s", arrElement))
					ElseIf arr.B Then
						Dim arrElement As Long = arr.getLong(i)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
						sb.Append(String.Format("%1$" & padding & "s",If(arrElement = 0, "false", "true")))
					ElseIf arr.dataType() = DataType.UTF8 Then
						Dim s As String = arr.getString(i)
						s = """" & s.replaceAll(vbLf, "\n") & """"
						sb.Append(s)
					End If
				End If
				If i < l - 1 Then
					If Not summarize OrElse i <= 2 OrElse i >= l - 3 OrElse (summarize AndAlso l = 6) Then
						sb.Append(colSep)
					End If
				End If
			Next i
			sb.Append("]")
			Return sb.ToString()
		End Function


		Private Function localeIndifferentDecimalFormat(ByVal pattern As String) As DecimalFormat
			Return New DecimalFormat(pattern, DecimalFormatSymbols.getInstance(Locale.US))
		End Function
	End Class

End Namespace