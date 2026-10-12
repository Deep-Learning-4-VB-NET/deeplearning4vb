Imports System
Imports System.Globalization
Imports System.IO
Imports System.Text

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

    ''' <summary>
    ''' System Information Saving: writes time-stamped diagnostic lines both to a console
    ''' writer and to a uniquely named text file. The Java <c>PrintStream</c> parameters are
    ''' <see cref="TextWriter"/> here (pass <c>Console.Out</c>/<c>Console.Error</c>).
    ''' </summary>
    Public Class SIS
        Private ReadOnly baseModuleCode As String = "SIS"
        Private moduleCode As String = "?"
        Private [out] As TextWriter
        Private err As TextWriter
        Private fullFileName As String = "?"
        Private wasOpenedFile As Boolean = False
        Private wasClosedFile As Boolean = False
        Private sis_File As String
        Private sis_Writer As StreamWriter
        Private writerErrorInfoCount As Integer = 0
        Private closedFileInfoCount As Integer = 0
        Private charsCount As Long = 0

        Private Sub OutFormat(ByVal format As String, ByVal ParamArray args() As Object)
            [out].Write(JavaString.format(format, args))
        End Sub

        ''' <summary>Initialises console output only (no file is created).</summary>
        Public Overridable Sub initValues(ByVal mtLv As Integer, ByVal superiorModuleCode As String, ByVal [out] As TextWriter, ByVal err As TextWriter)
            mtLv += 1
            moduleCode = superiorModuleCode & "." & baseModuleCode
            Me.out = If([out], TextWriter.Null)
            Me.err = If(err, TextWriter.Null)
        End Sub

        ''' <summary>Initialises console output and creates the log file in <paramref name="fileDrcS"/>.</summary>
        Public Overridable Sub initValues(ByVal mtLv As Integer, ByVal superiorModuleCode As String, ByVal [out] As TextWriter, ByVal err As TextWriter, ByVal fileDrcS As String, ByVal base_FileCode As String, ByVal spc_FileCode As String, ByVal ShowBriefInfo As Boolean, ByVal ShowFullInfo As Boolean)
            mtLv += 1
            moduleCode = superiorModuleCode & "." & baseModuleCode
            Dim methodName As String = moduleCode & "." & "initValues"
            Me.out = If([out], TextWriter.Null)
            Me.err = If(err, TextWriter.Null)
            If ShowBriefInfo OrElse ShowFullInfo Then
                OutFormat("")
                OutFormat(BTools.getMtLvESS(mtLv))
                OutFormat(methodName & ": ")
                OutFormat("fileDrcS: " & fileDrcS & "; ")
                OutFormat("base_FileCode: " & base_FileCode & "; ")
                OutFormat("spc_FileCode: " & spc_FileCode & "; ")
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
            End If
            initFile(mtLv, fileDrcS, base_FileCode, spc_FileCode, ShowBriefInfo, ShowFullInfo)
        End Sub

        Private Sub initFile(ByVal mtLv As Integer, ByVal fileDrcS As String, ByVal base_FileCode As String, ByVal spc_FileCode As String, ByVal ShowBriefInfo As Boolean, ByVal ShowFullInfo As Boolean)
            mtLv += 1
            Dim oinfo As String = ""
            Dim methodName As String = moduleCode & "." & "initFile"
            If ShowBriefInfo OrElse ShowFullInfo Then
                OutFormat("")
                OutFormat(BTools.getMtLvESS(mtLv))
                OutFormat(methodName & ": ")
                OutFormat("fileDrcS: " & fileDrcS & "; ")
                OutFormat("base_FileCode: " & base_FileCode & "; ")
                OutFormat("spc_FileCode: " & spc_FileCode & "; ")
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
            End If
            spc_FileCode = spc_FileCode.Replace(":", "")
            spc_FileCode = spc_FileCode.Replace("/", "")
            spc_FileCode = spc_FileCode.Replace(".", "")
            If Not Directory.Exists(fileDrcS) Then
                Directory.CreateDirectory(fileDrcS)
                OutFormat("")
                OutFormat(BTools.getMtLvESS(mtLv))
                OutFormat(methodName & ": ")
                OutFormat("fileDrcS: %s; ", fileDrcS)
                OutFormat("Directory was created; ")
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
            End If
            Dim TimeS As String = DateTime.Now.ToString("yyyyMMdd'_'HHmmss.fff", CultureInfo.InvariantCulture)
            fullFileName = "Z" & TimeS & "_" & base_FileCode & "_" & spc_FileCode & ".txt"
            sis_File = Path.Combine(fileDrcS, fullFileName)
            If File.Exists(sis_File) Then
                If ShowBriefInfo OrElse ShowFullInfo Then
                    OutFormat("")
                    OutFormat(BTools.getMtLvESS(mtLv))
                    OutFormat(BTools.MtLvISS)
                    OutFormat("delete File; ")
                    OutFormat("%s", BTools.SLcDtTm)
                    OutFormat("%n")
                End If
                File.Delete(sis_File)
            End If
            Try
                Using New FileStream(sis_File, FileMode.CreateNew, FileAccess.Write)
                End Using
            Catch Exc As Exception
                OutFormat("===")
                OutFormat(methodName & ": ")
                OutFormat("create New File error !!! ")
                OutFormat("Exception: %s; ", Exc.Message)
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
                OutFormat("===")
                OutFormat(BTools.MtLvISS)
                OutFormat("fileDrcS: " & fileDrcS & "; ")
                OutFormat("fullFileName: " & fullFileName & "; ")
                OutFormat("%n")
                Return
            End Try
            If ShowFullInfo Then
                OutFormat("")
                OutFormat(BTools.getMtLvESS(mtLv))
                OutFormat(BTools.MtLvISS)
                OutFormat("fullFileName: " & fullFileName & "; ")
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
            End If
            Try
                sis_Writer = New StreamWriter(sis_File, False, New UTF8Encoding(False))
            Catch Exc As Exception
                OutFormat("===")
                OutFormat(methodName & ": ")
                OutFormat("create New Writer: ")
                OutFormat("Exception: %s; ", Exc.Message)
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
                Return
            End Try
            wasOpenedFile = True
            If ShowFullInfo Then
                oinfo = ""
                oinfo &= BTools.getMtLvESS(mtLv)
                oinfo &= methodName & ": "
                oinfo &= "fullFileName: " & fullFileName & "; "
                OutFormat("%s", BTools.SLcDtTm)
                info(oinfo)
            End If
        End Sub

        ''' <summary>The name (without directory) of the log file, or "?" before initialisation.</summary>
        Public Overridable Function getfullFileName() As String
            Return fullFileName
        End Function

        ''' <summary>The full path of the log file, or Nothing before initialisation.</summary>
        Public Overridable Function getFullFilePath() As String
            Return sis_File
        End Function

        ''' <summary>Writes a line to the console writer and, time-stamped, to the log file.</summary>
        Public Overridable Sub info(ByVal oinfo As String)
            Dim methodName As String = moduleCode & "." & "info"
            OutFormat("%s%n", oinfo)
            charsCount += oinfo.Length
            Dim FOInfo As String = getFullInfoString(oinfo)
            If Not isFileOpen(methodName) Then
                Return
            End If
            outFile(FOInfo)
            flushFile()
        End Sub

        ''' <summary>Total number of characters passed to <see cref="info"/>.</summary>
        Public Overridable Function getcharsCount() As Long
            Return charsCount
        End Function

        Private Function getFullInfoString(ByVal oinfo As String) As String
            Dim TimeS As String = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
            Return TimeS & ": " & oinfo & Environment.NewLine
        End Function

        Private Function isFileOpen(ByVal SourceMethodName As String) As Boolean
            If Not wasOpenedFile Then
                Return False
            End If
            If Not wasClosedFile Then
                Return True
            End If
            Dim methodName As String = moduleCode & "." & "isFileOpen"
            closedFileInfoCount += 1
            If closedFileInfoCount <= 3 Then
                OutFormat("===")
                OutFormat(methodName & "(from " & SourceMethodName & "): ")
                OutFormat("File is closed !!!; ")
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
            End If
            Return False
        End Function

        Private Sub outFile(ByVal FOInfo As String)
            Dim methodName As String = moduleCode & "." & "outFile"
            Try
                sis_Writer.Write(FOInfo)
            Catch Exc As Exception
                If writerErrorInfoCount < 2 Then
                    writerErrorInfoCount += 1
                    OutFormat("===")
                    OutFormat(methodName & ": ")
                    OutFormat("Writer.write error !!!; ")
                    OutFormat("Exception: %s; ", Exc.Message)
                    OutFormat("%s", BTools.SLcDtTm)
                    OutFormat("%n")
                End If
            End Try
        End Sub

        Private Sub flushFile()
            Dim methodName As String = moduleCode & "." & "flushFile"
            Try
                sis_Writer.Flush()
            Catch Exc As Exception
                OutFormat("===")
                OutFormat(methodName & ": ")
                OutFormat("Writer.flush error !!!; ")
                OutFormat("Exception: %s; ", Exc.Message)
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
            End Try
        End Sub

        ''' <summary>Writes a final line and closes the log file.</summary>
        Public Overridable Sub onStop(ByVal mtLv As Integer)
            mtLv += 1
            Dim methodName As String = moduleCode & "." & "onStop"
            Dim oinfo As String = ""
            oinfo &= BTools.getMtLvESS(mtLv)
            oinfo &= methodName & ": "
            oinfo &= BTools.SLcDtTm
            info(oinfo)
            closeFile()
        End Sub

        Private Sub closeFile()
            Dim methodName As String = moduleCode & "." & "closeFile"
            If sis_Writer Is Nothing Then
                wasClosedFile = True
                Return
            End If
            flushFile()
            Try
                sis_Writer.Close()
            Catch Exc As Exception
                OutFormat("===")
                OutFormat(methodName & ": ")
                OutFormat("Writer.close error !!!; ")
                OutFormat("Exception: %s; ", Exc.Message)
                OutFormat("%s", BTools.SLcDtTm)
                OutFormat("%n")
            End Try
            wasClosedFile = True
        End Sub

    End Class

End Namespace
