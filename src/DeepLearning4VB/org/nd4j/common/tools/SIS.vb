Imports System
Imports System.IO
Imports Microsoft.VisualBasic

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




	Public Class SIS
		' System Informations Saving
		'
		Private baseModuleCode As String = "SIS"
		Private moduleCode As String = "?"
		'
		Private [out] As PrintStream
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") private java.io.PrintStream err;
		Private err As PrintStream
		'
		Private fullFileName As String = "?"
		'
		Private wasOpenedFile As Boolean = False
		Private wasClosedFile As Boolean = False
		'
		Private sis_File As File
		Private sis_Writer As Writer
		'
		Private writerErrorInfoCount As Integer = 0
		Private closedFileInfoCount As Integer = 0
		'
		Private charsCount As Long = 0
		'

		''' <summary>
		''' <b>initValues</b><br>
		''' public void initValues( int mtLv, String superiorModuleCode,<br>
		'''    PrintStream out, PrintStream err )<br>
		''' Initialize values for console - not file.<br> </summary>
		''' <param name="mtLv"> - method level </param>
		''' <param name="superiorModuleCode"> - superior module code </param>
		''' <param name="out"> - console standard output </param>
		''' <param name="err"> - console error output (not used) </param>
		Public Overridable Sub initValues(ByVal mtLv As Integer, ByVal superiorModuleCode As String, ByVal [out] As PrintStream, ByVal err As PrintStream)
			'
			mtLv += 1
			'
			moduleCode = superiorModuleCode & "." & baseModuleCode
			'
			Me.out = [out]
			Me.err = err
			'
		End Sub

		''' <summary>
		''' <b>initValues</b><br>
		''' public void initValues( int mtLv, String superiorModuleCode,<br>
		'''   PrintStream out, PrintStream err, String fileDrcS,<br>
		'''   String base_FileCode, String spc_FileCode,<br>
		'''   boolean ShowBriefInfo, boolean ShowFullInfo )<br>
		''' Initialize values for console and file.<br>
		''' 	fullFileName =<br>
		'''    "Z" +<br>
		'''	  TimeS + "_" +<br>
		'''    base_FileCode + "_" +<br>
		'''    spc_FileCode +<br>
		'''    ".txt";<br>
		''' TimeS (time string) format: "yyyyMMdd'_'HHmmss.SSS"<br> </summary>
		''' <param name="mtLv"> - method level </param>
		''' <param name="superiorModuleCode"> - superior module code </param>
		''' <param name="out"> - console standard output </param>
		''' <param name="err"> - console error output (not used) </param>
		''' <param name="fileDrcS"> - file directory as string </param>
		''' <param name="base_FileCode"> - base part of file code </param>
		''' <param name="spc_FileCode"> - specifying part of file code </param>
		''' <param name="ShowBriefInfo"> - show brief informations </param>
		''' <param name="ShowFullInfo"> - show full informations </param>
		Public Overridable Sub initValues(ByVal mtLv As Integer, ByVal superiorModuleCode As String, ByVal [out] As PrintStream, ByVal err As PrintStream, ByVal fileDrcS As String, ByVal base_FileCode As String, ByVal spc_FileCode As String, ByVal ShowBriefInfo As Boolean, ByVal ShowFullInfo As Boolean)
			'
			mtLv += 1
			'
			moduleCode = superiorModuleCode & "." & baseModuleCode
			'
			Dim methodName As String = moduleCode & "." & "initValues"
			'
			Me.out = [out]
			Me.err = err
			'
			If ShowBriefInfo OrElse ShowFullInfo Then
				[out].format("")
				[out].format(BTools.getMtLvESS(mtLv))
				[out].format(methodName & ": ")
				[out].format("fileDrcS: " & fileDrcS & "; ")
				[out].format("base_FileCode: " & base_FileCode & "; ")
				[out].format("spc_FileCode: " & spc_FileCode & "; ")
	'			out.format( "STm: %s; ", Tools.getSDatePM( System.currentTimeMillis(), "HH:mm:ss" ) + "; " );
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
			End If
			'
			initFile(mtLv, fileDrcS, base_FileCode, spc_FileCode, ShowBriefInfo, ShowFullInfo)
			'
		End Sub

		Private Sub initFile(ByVal mtLv As Integer, ByVal fileDrcS As String, ByVal base_FileCode As String, ByVal spc_FileCode As String, ByVal ShowBriefInfo As Boolean, ByVal ShowFullInfo As Boolean)
			'
			mtLv += 1
			'
			Dim oinfo As String = ""
			'
			Dim methodName As String = moduleCode & "." & "initFile"
			'
			If ShowBriefInfo OrElse ShowFullInfo Then
				[out].format("")
				[out].format(BTools.getMtLvESS(mtLv))
				[out].format(methodName & ": ")
				[out].format("fileDrcS: " & fileDrcS & "; ")
				[out].format("base_FileCode: " & base_FileCode & "; ")
				[out].format("spc_FileCode: " & spc_FileCode & "; ")
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
			End If
			'
			spc_FileCode = spc_FileCode.Replace(":", "")
			spc_FileCode = spc_FileCode.Replace("/", "")
			spc_FileCode = spc_FileCode.Replace(".", "")
			'
			Dim fileDrc As New File(fileDrcS)
			'
			If Not fileDrc.exists() Then
				fileDrc.mkdirs()
				'
				[out].format("")
				[out].format(BTools.getMtLvESS(mtLv))
				[out].format(methodName & ": ")
				[out].format("fileDrcS: %s; ", fileDrcS)
				[out].format("Directory was created; ")
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
			End If
			'
			Dim LDT As DateTime = DateTime.Now
			'
			Dim TimeS As String = LDT.format(DateTimeFormatter.ofPattern("yyyyMMdd'_'HHmmss.SSS"))
			'
			fullFileName = "Z" & TimeS & "_" & base_FileCode & "_" & spc_FileCode & ".txt"
			'
			sis_File = New File(fileDrcS, fullFileName)
			'
			sis_File.setReadable(True)
			'
			If sis_File.exists() Then
				If ShowBriefInfo OrElse ShowFullInfo Then
					[out].format("")
					[out].format(BTools.getMtLvESS(mtLv))
					[out].format(BTools.MtLvISS)
					[out].format("delete File; ")
					[out].format("%s", BTools.SLcDtTm)
					[out].format("%n")
				End If
				sis_File.delete()
			End If
			'
			Try
				sis_File.createNewFile()
			Catch Exc As Exception
			'	Exc.printStackTrace( Err_PS );
				[out].format("===")
				[out].format(methodName & ": ")
				[out].format("create New File error !!! ")
				[out].format("Exception: %s; ", Exc.Message)
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
				[out].format("===")
				[out].format(BTools.MtLvISS)
				[out].format("fileDrcS: " & fileDrcS & "; ")
				[out].format("fullFileName: " & fullFileName & "; ")
				[out].format("%n")
				'
				Return
			End Try
			'
			If ShowFullInfo Then
				[out].format("")
				[out].format(BTools.getMtLvESS(mtLv))
				[out].format(BTools.MtLvISS)
				[out].format("fullFileName: " & fullFileName & "; ")
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
			End If
			'
			Try
				sis_Writer = New StreamWriter(sis_File)
			Catch Exc As Exception
				[out].format("===")
				[out].format(methodName & ": ")
				[out].format("create New Writer: ")
				[out].format("Exception: %s; ", Exc.Message)
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
				'
				Return
			End Try
			'
			wasOpenedFile = True
			'
			If ShowFullInfo Then
				oinfo = ""
				oinfo &= BTools.getMtLvESS(mtLv)
				oinfo &= methodName & ": "
				oinfo &= "fullFileName: " & fullFileName & "; "
				[out].format("%s", BTools.SLcDtTm)
				info(oinfo)
			End If
			'
		End Sub

		''' <summary>
		''' <b>getfullFileName</b><br>
		''' public String getfullFileName()<br>
		''' Returns full file name<br> </summary>
		''' <returns> full file name </returns>
		Public Overridable Function getfullFileName() As String
			'
			Return fullFileName
		End Function

		''' <summary>
		''' <b>info</b><br>
		''' public void info( String oinfo )<br>
		''' This method is input for informations.<br>
		''' Informations are showed in console and saved in file.<br> </summary>
		''' <param name="oinfo"> - information </param>
		Public Overridable Sub info(ByVal oinfo As String)
			'
			Dim methodName As String = moduleCode & "." & "info"
			'
			[out].format("%s%n", oinfo)
			'
			charsCount += oinfo.Length
			'
			Dim FOInfo As String = getFullInfoString(oinfo)
			'
			If Not isFileOpen(methodName) Then
				Return
			End If
			'
			outFile(FOInfo)
			'
			flushFile()
			'
		End Sub

		''' <summary>
		''' <b>getcharsCount</b><br>
		''' public long getcharsCount()<br>
		''' Returns chars count counted from SIS creating.<br> </summary>
		''' <returns> chars count </returns>
		Public Overridable Function getcharsCount() As Long
			'
			Return charsCount
		End Function

		Private Function getFullInfoString(ByVal oinfo As String) As String
			'
			Dim Result As String = ""
			'
			Dim LDT As DateTime = DateTime.Now
			'
			Dim TimeS As String = LDT.format(DateTimeFormatter.ofPattern("yyyy.MM.dd HH:mm:ss.SSS"))
			'
			Result = TimeS & ": " & oinfo & vbCrLf & ""
			'
			Return Result
		End Function

		Private Function isFileOpen(ByVal SourceMethodName As String) As Boolean
			'
			If Not wasOpenedFile Then
				Return False
			End If
			If Not wasClosedFile Then
				Return True
			End If
			'
			Dim methodName As String = moduleCode & "." & "isFileOpen"
			'
			closedFileInfoCount += 1
			If closedFileInfoCount <= 3 Then
				[out].format("===")
	'			out.format( methodName + ": " );
				[out].format(methodName & "(from " & SourceMethodName & "): ")
				[out].format("File is closed !!!; ")
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
			End If
			'
			Return False
		End Function

		Private Sub outFile(ByVal FOInfo As String)
			'
			Dim methodName As String = moduleCode & "." & "outFile"
			'
			Try
				sis_Writer.write(FOInfo)
			Catch Exc As Exception
				If writerErrorInfoCount < 2 Then
					writerErrorInfoCount += 1
					[out].format("===")
					[out].format(methodName & ": ")
					[out].format("Writer.write error !!!; ")
					[out].format("Exception: %s; ", Exc.Message)
					[out].format("%s", BTools.SLcDtTm)
					[out].format("%n")
				End If
				'
			End Try
			'
		End Sub

		Private Sub flushFile()
			'
			Dim methodName As String = moduleCode & "." & "flushFile"
			'
			Try
				sis_Writer.flush()
			Catch Exc As Exception
				[out].format("===")
				[out].format(methodName & ": ")
				[out].format("Writer.flush error !!!; ")
				[out].format("Exception: %s; ", Exc.Message)
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
			End Try
			'
		End Sub

		''' <summary>
		''' <b>onStop</b><br>
		''' public void onStop( int mtLv )<br>
		''' This method should be called at the end of program.<br>
		''' Close file.<br> </summary>
		''' <param name="mtLv"> - method level </param>
		Public Overridable Sub onStop(ByVal mtLv As Integer)
			'
			mtLv += 1
			'
			Dim oinfo As String = ""
			'
			Dim methodName As String = moduleCode & "." & "onStop"
			'
			oinfo = ""
			oinfo &= BTools.getMtLvESS(mtLv)
			oinfo &= methodName & ": "
			oinfo &= BTools.SLcDtTm
			info(oinfo)
			'
			closeFile()
			'
		End Sub


		Private Sub closeFile()
			'
			Dim methodName As String = moduleCode & "." & "closeFile"
			'
			flushFile()
			'
			Try
				sis_Writer.close()
			Catch Exc As Exception
				[out].format("===")
				[out].format(methodName & ": ")
				[out].format("Writer.close error !!!; ")
				[out].format("Exception: %s; ", Exc.Message)
				[out].format("%s", BTools.SLcDtTm)
				[out].format("%n")
			End Try
			'
			wasClosedFile = True
			'
		End Sub






	End Class
End Namespace