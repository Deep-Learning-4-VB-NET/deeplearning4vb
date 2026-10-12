Imports System

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


	'B = Base
	Public Class BTools
		'

		''' <summary>
		''' <b>getMtLvESS</b><br>
		''' public static String getMtLvESS( int mtLv )<br>
		''' Returns string. String length create indentation(shift) of other text.<br>
		''' Indentation depends on method level - great method level, great indentation.<br>
		''' Main method has method level 0.<br>
		''' Other called method has method level 1, 2,...N.<br> </summary>
		''' <param name="mtLv"> - method level </param>
		''' <returns> method level external shift string </returns>
		Public Shared Function getMtLvESS(ByVal mtLv As Integer) As String
			'  MtLvESS = Method Level External Shift String 
			'
			If mtLv < 0 Then
				Return "?"
			End If
			'
			Dim Result As String = ""
			'
		'	String LvS = ". ";
			Dim LvS As String = "."
			'
			For K As Integer = 1 To mtLv
				'
				Result = Result & LvS
			Next K
			'
			Return Result
		End Function

		''' <summary>
		''' <b>getMtLvISS</b><br>
		''' public static String getMtLvISS()<br>
		''' Returns string. String create indentation(shift)<br>
		'''   internal text to start text of method.<br>
		''' </summary>
		''' <returns> method level internal shift string </returns>
		Public Shared ReadOnly Property MtLvISS As String
			Get
				'  MtLvISS = Method Level Intern Shift String 
				'
			'	String Result = "..";
			'	String Result = "~";
				Dim Result As String = " "
				'
				Return Result
			End Get
		End Property

		''' <summary>
		''' <b>getSpaces</b><br>
		''' public static String getSpaces( int SpacesCount )<br>
		''' Returns asked count of spaces.<br>
		''' If count of spaces is < 0 returns '?'. </summary>
		''' <param name="SpacesCount"> = spaces count </param>
		''' <returns> spaces </returns>
		Public Shared Function getSpaces(ByVal SpacesCount As Integer) As String
			'
			If SpacesCount < 0 Then
				Return "?"
			End If
			'
			Dim Info As String = ""
			'
			For K As Integer = 1 To SpacesCount
				Info &= " "
			Next K
			'
			'
			Return Info
		End Function

		''' <summary>
		''' <b>getSBln</b><br>
		''' public static String getSBln( boolean... blnA )<br>
		''' Returns boolean(s) converted to char (true = 'T'; false = 'F')<br>
		''' If blnA.length is > 1 returns chars without separator.<br>
		''' If blnA is '{ true, false, true }' returns 'TFT'.<br>
		''' If blnA is null returns '?'.<br>
		''' If blnA.length is 0 returns '?'.<br> </summary>
		''' <param name="blnA"> </param>
		''' <returns> boolean(s) as string </returns>
		Public Shared Function getSBln(ParamArray ByVal blnA() As Boolean) As String
			'
			Dim Info As String = ""
			'
			If blnA Is Nothing Then
				Return "?"
			End If
			If blnA.Length = 0 Then
				Return "?"
			End If
			'
			For K As Integer = 0 To blnA.Length - 1
				'
				Info &= If(blnA(K), "T", "F")
			Next K
			'
			Return Info
		End Function

		''' <summary>
		''' <b>getSDbl</b><br>
		''' public static String getSDbl( double Value, int DecPrec )<br>
		''' Returns double converted to string.<br>
		''' If Value is Double.NaN returns "NaN".<br>
		''' If DecPrec is < 0 is DecPrec set 0.<br>
		''' </summary>
		''' <param name="Value"> - value </param>
		''' <param name="DecPrec"> - decimal precision </param>
		''' <returns> double as string </returns>
		Public Shared Function getSDbl(ByVal Value As Double, ByVal DecPrec As Integer) As String
			'
			Dim Result As String = ""
			'
			If Double.IsNaN(Value) Then
				Return "NaN"
			End If
			'
			If DecPrec < 0 Then
				DecPrec = 0
			End If
			'
			Dim DFS As String = "###,###,##0"
			'
			If DecPrec > 0 Then
				Dim idx As Integer = 0
				DFS &= "."
				Do While idx < DecPrec
					DFS = DFS & "0"
					idx += 1
					If idx > 100 Then
						Exit Do
					End If
				Loop
			End If
			'
	'		Locale locale  = new Locale("en", "UK");
			'
			Dim DcmFrmSmb As New DecimalFormatSymbols(Locale.getDefault())
			DcmFrmSmb.setDecimalSeparator("."c)
			DcmFrmSmb.setGroupingSeparator(" "c)
			'
			Dim DcmFrm As DecimalFormat
			'
			DcmFrm = New DecimalFormat(DFS, DcmFrmSmb)
			'
		'	DcmFrm.setGroupingSize( 3 );
			'
			Result = DcmFrm.format(Value)
			'
			Return Result
		End Function

		''' <summary>
		''' <b>getSDbl</b><br>
		''' public static String getSDbl( double Value, int DecPrec, boolean ShowPlusSign )<br>
		''' Returns double converted to string.<br>
		''' If Value is Double.NaN returns "NaN".<br>
		''' If DecPrec is < 0 is DecPrec set 0.<br>
		''' If ShowPlusSign is true:<br>
		'''   - If Value is > 0 sign is '+'.<br>
		'''   - If Value is 0 sign is ' '.<br> </summary>
		''' <param name="Value"> - value </param>
		''' <param name="DecPrec"> - decimal precision </param>
		''' <param name="ShowPlusSign"> - show plus sign </param>
		''' <returns> double as string </returns>
		Public Shared Function getSDbl(ByVal Value As Double, ByVal DecPrec As Integer, ByVal ShowPlusSign As Boolean) As String
			'
			Dim PlusSign As String = ""
			'
			If ShowPlusSign AndAlso Value > 0 Then
				PlusSign = "+"
			End If
			If ShowPlusSign AndAlso Value = 0 Then
				PlusSign = " "
			End If
			'
			Return PlusSign & getSDbl(Value, DecPrec)
		End Function

		''' <summary>
		''' <b>getSDbl</b><br>
		''' public static String getSDbl( double Value, int DecPrec, boolean ShowPlusSign, int StringLength )<br>
		''' Returns double converted to string.<br>
		''' If Value is Double.NaN returns "NaN".<br>
		''' If DecPrec is < 0 is DecPrec set 0.<br>
		''' If ShowPlusSign is true:<br>
		'''   - If Value is > 0 sign is '+'.<br>
		'''   - If Value is 0 sign is ' '.<br>
		''' If StringLength is > base double string length<br>
		'''   before base double string adds relevant spaces.<br>
		''' If StringLength is <= base double string length<br>
		'''   returns base double string.<br> </summary>
		''' <param name="Value"> - value </param>
		''' <param name="DecPrec"> - decimal precision </param>
		''' <param name="ShowPlusSign"> - show plus sign </param>
		''' <param name="StringLength"> - string length </param>
		''' <returns> double as string </returns>
		Public Shared Function getSDbl(ByVal Value As Double, ByVal DecPrec As Integer, ByVal ShowPlusSign As Boolean, ByVal StringLength As Integer) As String
			'
			Dim Info As String = ""
			'
			Dim SDbl As String = getSDbl(Value, DecPrec, ShowPlusSign)
			'
			If SDbl.Length >= StringLength Then
				Return SDbl
			End If
			'
	'		String SpacesS = "            ";
			Dim SpacesS As String = getSpaces(StringLength)
			'
			Info = SpacesS.Substring(0, StringLength - SDbl.Length) & SDbl
			'
			Return Info
		End Function

		''' <summary>
		''' <b>getSInt</b><br>
		''' public static String getSInt( int Value, int CharsCount )<br>
		''' Returns int converted to string.<br>
		''' If CharsCount > base int string length<br>
		'''   before base int string adds relevant spaces.<br>
		''' If CharsCount <= base int string length<br>
		'''   returns base int string.<br> </summary>
		''' <param name="Value"> - value </param>
		''' <param name="CharsCount"> - chars count </param>
		''' <returns> int as string </returns>
		Public Shared Function getSInt(ByVal Value As Integer, ByVal CharsCount As Integer) As String
			'
			Return getSInt(Value, CharsCount, " "c)
		End Function

		''' <summary>
		''' <b>getSInt</b><br>
		''' public static String getSInt( int Value, int CharsCount, char LeadingChar )<br>
		''' Returns int converted to string.<br>
		''' If CharsCount > base int string length<br>
		'''   before base int string adds relevant leading chars.<br>
		''' If CharsCount <= base int string length<br>
		'''   returns base int string.<br>
		''' </summary>
		''' <param name="Value"> - value </param>
		''' <param name="CharsCount"> - chars count </param>
		''' <param name="LeadingChar"> - leading char </param>
		''' <returns> int as string </returns>
		Public Shared Function getSInt(ByVal Value As Integer, ByVal CharsCount As Integer, ByVal LeadingChar As Char) As String
			'
			Dim Result As String = ""
			'
			If CharsCount <= 0 Then
				Return getSInt(Value)
			End If
			'
			Dim FormatS As String = ""
			If LeadingChar = "0"c Then
				FormatS = "%" & LeadingChar & Convert.ToString(CharsCount) & "d"
			Else
				FormatS = "%" & Convert.ToString(CharsCount) & "d"
			End If
			'
			Result = String.format(FormatS, Value)
			'
			Return Result
		End Function

		''' <summary>
		''' <b>getSInt</b><br>
		''' public static String getSInt( int Value )<br>
		''' Returns int converted to string.<br> </summary>
		''' <param name="Value"> </param>
		''' <returns> int as string </returns>
		Public Shared Function getSInt(ByVal Value As Integer) As String
			'
			Dim Result As String = ""
			'
			Result = String.Format("{0:D}", Value)
			'
			Return Result
		End Function

		''' <summary>
		''' <b>getSIntA</b><br>
		''' public static String getSIntA( int... intA )<br>
		''' Returns intA converted to string.<br>
		''' Strings are separated with ", ".<br>
		''' If intA is null returns '?'.<br>
		''' If intA.length is 0 returns '?'.<br> </summary>
		''' <param name="intA"> - int value(s) (one or more) </param>
		''' <returns> int... as string </returns>
	'	public static String getSIntA( int[] intA ) {
		Public Shared Function getSIntA(ParamArray ByVal intA() As Integer) As String
			'
			Dim Info As String = ""
			'
			If intA Is Nothing Then
				Return "?"
			End If
			If intA.Length = 0 Then
				Return "?"
			End If
			'
			For K As Integer = 0 To intA.Length - 1
				'
				Info &= If(Info.Length = 0, "", ", ")
				Info &= BTools.getSInt(intA(K))
			Next K
			'
			Return Info
		End Function

		''' <summary>
		''' <b>getIndexCharsCount</b><br>
		''' public static int getIndexCharsCount( int MaxIndex )<br>
		''' Returns chars count for max value of index.<br>
		''' Example: Max value of index is 150 and chars count is 3.<br>
		''' It is important for statement of indexed values.<br>
		''' Index columns can have the same width for all rouws.<br> </summary>
		''' <param name="MaxIndex"> - max value of index </param>
		''' <returns> chars count for max value of index </returns>
		Public Shared Function getIndexCharsCount(ByVal MaxIndex As Integer) As Integer
			'
			Dim CharsCount As Integer = 1
			'
			If MaxIndex <= 0 Then
				Return 1
			End If
			'
			CharsCount = CInt(Math.Truncate(Math.Log10(MaxIndex))) + 1
			'
			Return CharsCount
		End Function

		''' <summary>
		''' <b>getSLcDtTm</b><br>
		''' public static String getSLcDtTm()<br>
		''' Returns local datetime as string.<br>
		''' Datetime format is "mm:ss.SSS".<br> </summary>
		''' <returns> local datetime as string </returns>
		Public Shared ReadOnly Property SLcDtTm As String
			Get
				'
				Return getSLcDtTm("mm:ss.SSS")
			End Get
		End Property

		''' <summary>
		''' <b>getSLcDtTm</b><br>
		''' public static String getSLcDtTm( String FormatS )<br>
		''' Returns local datetime as string.<br>
		''' Datetime format is param.<br> </summary>
		''' <param name="FormatS"> datetime format </param>
		''' <returns> local datetime as string </returns>
		Public Shared Function getSLcDtTm(ByVal FormatS As String) As String
			'
			Dim Result As String = "?"
			'
			Dim LDT As DateTime = DateTime.Now
			'
			Result = "LDTm: " & LDT.format(DateTimeFormatter.ofPattern(FormatS))
			'
			Return Result
		End Function






	End Class
End Namespace