Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports Nd4vbCommonValidator = DeepLearning4VB.org.nd4vb.common.validation.Nd4vbCommonValidator
Imports ValidationResult = DeepLearning4VB.org.nd4vb.common.validation.ValidationResult

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

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' Checks whether files hold valid ND4VB arrays: binary and text INDArray files, numpy <c>.npy</c>/<c>.npz</c> files
    ''' and numpy text files (Java <c>Nd4jValidator</c>).
    ''' <para>
    ''' Differences from Java: files are path strings (<c>java.io.File</c>) and charsets are <see cref="Encoding"/>s; numbers
    ''' in numpy text files are parsed with the invariant culture; Java's <c>Throwable</c> catch blocks catch
    ''' <see cref="System.Exception"/>. <c>validateSameDiffFlatBuffers</c> is fenced with <c>#If FLATBUFFERS</c>
    ''' (milestone M5); without the fence it throws <see cref="NotSupportedException"/>.
    ''' </para>
    ''' </summary>
    ''' @author Alex Black
    Public Class Nd4vbValidator

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Validate whether the file represents a valid INDArray (of any data type) saved previously with
        ''' <c>Nd4vb.saveBinary</c>, to be read with <c>Nd4vb.readBinary</c>.
        ''' </summary>
        ''' <param name="f"> File that should represent an INDArray </param>
        ''' <returns> Result of validation </returns>
        Public Shared Function validateINDArrayFile(ByVal f As String) As ValidationResult
            Return validateINDArrayFile(f, DirectCast(Nothing, DataType()))
        End Function

        ''' <summary>
        ''' Validate whether the file represents a valid INDArray saved previously with <c>Nd4vb.saveBinary</c>, to be
        ''' read with <c>Nd4vb.readBinary</c>. As in Java, the allowable data types are not enforced.
        ''' </summary>
        ''' <param name="f">                  File that should represent an INDArray </param>
        ''' <param name="allowableDataTypes"> May be <c>Nothing</c>. If non-null, the file must have one of the specified data types </param>
        ''' <returns> Result of validation </returns>
        Public Shared Function validateINDArrayFile(ByVal f As String, ParamArray ByVal allowableDataTypes() As DataType) As ValidationResult
            Dim vr As ValidationResult = Nd4vbCommonValidator.isValidFile(f, "INDArray File", False)
            If vr IsNot Nothing AndAlso Not vr.isValid() Then
                vr.FormatClass = GetType(INDArray)
                Return vr
            End If

            'TODO let's do this without reading the whole thing into memory - check header + length...
            Try 'Using the fact that INDArray.close() exists -> deallocate memory as soon as reading is done
                Using arr As INDArray = Nd4vbT.readBinary(f)
                    If allowableDataTypes IsNot Nothing Then
                        Array.IndexOf(allowableDataTypes, arr.dataType())
                    End If
                End Using
            Catch e As IOException
                Return ValidationResult.builder().valid(False).formatType("INDArray File").formatClass(GetType(INDArray)).path(Nd4vbCommonValidator.getPath(f)).issues(New List(Of String) From {"Unable to read file (IOException)"}).exception(e).build()
            Catch t As System.Exception
                If isMemoryException(t) Then
                    'This is a memory exception during reading... result is indeterminant (might be valid, might not be, can't tell here)
                    Return ValidationResult.builder().valid(True).formatType("INDArray File").formatClass(GetType(INDArray)).path(Nd4vbCommonValidator.getPath(f)).build()
                End If

                Return ValidationResult.builder().valid(False).formatType("INDArray File").formatClass(GetType(INDArray)).path(Nd4vbCommonValidator.getPath(f)).issues(New List(Of String) From {"File may be corrupt or is not a binary INDArray file"}).exception(t).build()
            End Try

            Return ValidationResult.builder().valid(True).formatType("INDArray File").formatClass(GetType(INDArray)).path(Nd4vbCommonValidator.getPath(f)).build()
        End Function

        ''' <summary>
        ''' Validate whether the file represents a valid INDArray text file (any data type) saved with
        ''' <c>Nd4vb.writeTxt</c>, to be read with <c>Nd4vb.readTxt</c>.
        ''' </summary>
        ''' <param name="f"> File that should represent an INDArray text file </param>
        ''' <returns> Result of validation </returns>
        Public Shared Function validateINDArrayTextFile(ByVal f As String) As ValidationResult
            Dim vr As ValidationResult = Nd4vbCommonValidator.isValidFile(f, "INDArray Text File", False)
            If vr IsNot Nothing AndAlso Not vr.isValid() Then
                vr.FormatClass = GetType(INDArray)
                Return vr
            End If

            'TODO let's do this without reading the whole thing into memory - check header + length...
            Try 'Using the fact that INDArray.close() exists -> deallocate memory as soon as reading is done
                Using arr As INDArray = Nd4vbT.readTxt(f)
                End Using
            Catch t As System.Exception
                If isMemoryException(t) Then
                    'This is a memory exception during reading... result is indeterminant (might be valid, might not be, can't tell here)
                    Return ValidationResult.builder().valid(True).formatType("INDArray Text File").formatClass(GetType(INDArray)).path(Nd4vbCommonValidator.getPath(f)).build()
                End If

                Return ValidationResult.builder().valid(False).formatType("INDArray Text File").formatClass(GetType(INDArray)).path(Nd4vbCommonValidator.getPath(f)).issues(New List(Of String) From {"File may be corrupt or is not a text INDArray file"}).exception(t).build()
            End Try

            Return ValidationResult.builder().valid(True).formatType("INDArray Text File").formatClass(GetType(INDArray)).path(Nd4vbCommonValidator.getPath(f)).build()
        End Function

        ''' <summary>
        ''' Validate whether the file represents a valid Numpy .npy file to be read with <c>Nd4vb.createFromNpyFile</c>.
        ''' </summary>
        ''' <param name="f"> File that should represent a Numpy .npy file </param>
        ''' <returns> Result of validation </returns>
        Public Shared Function validateNpyFile(ByVal f As String) As ValidationResult
            Dim vr As ValidationResult = Nd4vbCommonValidator.isValidFile(f, "Numpy .npy File", False)
            If vr IsNot Nothing AndAlso Not vr.isValid() Then
                Return vr
            End If

            'TODO let's do this without reading whole thing into memory
            Try 'Using the fact that INDArray.close() exists -> deallocate memory as soon as reading is done
                Using arr As INDArray = Nd4vbT.createFromNpyFile(f)
                End Using
            Catch t As System.Exception
                If isMemoryException(t) Then
                    'This is a memory exception during reading... result is indeterminant (might be valid, might not be, can't tell here)
                    Return ValidationResult.builder().valid(True).formatType("Numpy .npy File").path(Nd4vbCommonValidator.getPath(f)).build()
                End If

                Return ValidationResult.builder().valid(False).formatType("Numpy .npy File").path(Nd4vbCommonValidator.getPath(f)).issues(New List(Of String) From {"File may be corrupt or is not a Numpy .npy file"}).exception(t).build()
            End Try

            Return ValidationResult.builder().valid(True).formatType("Numpy .npy File").path(Nd4vbCommonValidator.getPath(f)).build()
        End Function

        ''' <summary>
        ''' Validate whether the file represents a valid Numpy .npz file to be read with <c>Nd4vb.createFromNpzFile</c>.
        ''' </summary>
        ''' <param name="f"> File that should represent a Numpy .npz file </param>
        ''' <returns> Result of validation </returns>
        Public Shared Function validateNpzFile(ByVal f As String) As ValidationResult
            Dim vr As ValidationResult = Nd4vbCommonValidator.isValidFile(f, "Numpy .npz File", False)
            If vr IsNot Nothing AndAlso Not vr.isValid() Then
                Return vr
            End If

            Dim m As IDictionary(Of String, INDArray) = Nothing
            Try
                m = Nd4vbT.createFromNpzFile(f)
            Catch t As System.Exception
                Return ValidationResult.builder().valid(False).formatType("Numpy .npz File").path(Nd4vbCommonValidator.getPath(f)).issues(New List(Of String) From {"File may be corrupt or is not a Numpy .npz file"}).exception(t).build()
            Finally
                'Deallocate immediately
                If m IsNot Nothing Then
                    For Each arr As INDArray In m.Values
                        If arr IsNot Nothing Then
                            arr.close()
                        End If
                    Next arr
                End If
            End Try

            Return ValidationResult.builder().valid(True).formatType("Numpy .npz File").path(Nd4vbCommonValidator.getPath(f)).build()
        End Function

        ''' <summary>
        ''' Basic validation for a Numpy text file (e.g. written with <c>numpy.savetxt</c>): every line has the same number
        ''' of values, separated by <paramref name="delimiter"/> (a regular expression, as in Java), and every value parses as a number.
        ''' </summary>
        ''' <param name="f">         File that should represent a Numpy text file </param>
        ''' <param name="delimiter"> The delimiter between values </param>
        ''' <param name="charset">   The character set of the file </param>
        ''' <returns> Result of validation </returns>
        Public Shared Function validateNumpyTxtFile(ByVal f As String, ByVal delimiter As String, ByVal charset As Encoding) As ValidationResult
            If delimiter Is Nothing Then
                Throw New ArgumentNullException(NameOf(delimiter), "delimiter is marked non-null but is null")
            End If
            If charset Is Nothing Then
                Throw New ArgumentNullException(NameOf(charset), "charset is marked non-null but is null")
            End If
            Dim vr As ValidationResult = Nd4vbCommonValidator.isValidFile(f, "Numpy text file", False)
            If vr IsNot Nothing AndAlso Not vr.isValid() Then
                Return vr
            End If

            Dim s As String
            Try
                s = File.ReadAllText(f, charset)
            Catch t As System.Exception
                Return ValidationResult.builder().valid(False).formatType("Numpy text file").path(Nd4vbCommonValidator.getPath(f)).issues(New List(Of String) From {"File may be corrupt or is not a Numpy text file"}).exception(t).build()
            End Try

            Dim lines() As String = javaSplit(s, vbLf)
            Dim countPerLine As Integer = 0
            For i As Integer = 0 To lines.Length - 1
                Dim lineSplit() As String = javaSplit(lines(i), delimiter)
                If i = 0 Then
                    countPerLine = lineSplit.Length
                ElseIf lines(i).Length > 0 Then
                    If countPerLine <> lineSplit.Length Then
                        Return ValidationResult.builder().valid(False).formatType("Numpy text file").path(Nd4vbCommonValidator.getPath(f)).issues(New List(Of String) From {"Number of values in each line is not the same for all lines: File may be corrupt, is not a Numpy text file, or delimiter """ & delimiter & """ is incorrect"}).build()
                    End If
                End If

                For j As Integer = 0 To lineSplit.Length - 1
                    Try
                        Double.Parse(lineSplit(j), NumberStyles.Float, CultureInfo.InvariantCulture)
                    Catch e As FormatException
                        Return ValidationResult.builder().valid(False).formatType("Numpy text file").path(Nd4vbCommonValidator.getPath(f)).issues(New List(Of String) From {"File may be corrupt, is not a Numpy text file, or delimiter """ & delimiter & """ is incorrect"}).exception(e).build()
                    End Try
                Next j
            Next i
            Return ValidationResult.builder().valid(True).formatType("Numpy text file").path(Nd4vbCommonValidator.getPath(f)).build()
        End Function

#If FLATBUFFERS Then
		Public Shared Function validateSameDiffFlatBuffers(ByVal f As File) As ValidationResult
			Dim vr As ValidationResult = Nd4jCommonValidator.isValidFile(f, "SameDiff FlatBuffers file", False)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				Return vr
			End If

			Try
				Dim bytes() As SByte
				Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
					bytes = IOUtils.toByteArray([is])
				End Using

				Dim bbIn As ByteBuffer = ByteBuffer.wrap(bytes)

				Dim fg As FlatGraph = FlatGraph.getRootAsFlatGraph(bbIn)
				Dim vl As Integer = fg.variablesLength()
				Dim ol As Integer = fg.nodesLength()
				Console.WriteLine()
			Catch t As Exception
				Return ValidationResult.builder().valid(False).formatType("SameDiff FlatBuffers file").formatClass(GetType(SameDiff)).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("File may be corrupt or is not a SameDiff file in FlatBuffers format")).exception(t).build()
			End Try

			Return ValidationResult.builder().valid(True).formatType("SameDiff FlatBuffers file").formatClass(GetType(SameDiff)).path(Nd4jCommonValidator.getPath(f)).build()
		End Function
#Else
        ''' <summary>
        ''' Validate whether the file represents a valid SameDiff FlatBuffers file. Needs FlatBuffers (phase 2 milestone M5).
        ''' </summary>
        ''' <exception cref="NotSupportedException">Always.</exception>
        Public Shared Function validateSameDiffFlatBuffers(ByVal f As String) As ValidationResult
            Throw New NotSupportedException("validateSameDiffFlatBuffers needs FlatBuffers (phase 2 milestone M5)")
        End Function
#End If

        ''' <summary>Java's test for a memory failure while reading: an out-of-memory error or "failed to allocate".</summary>
        Private Shared Function isMemoryException(ByVal t As System.Exception) As Boolean
            Return TypeOf t Is OutOfMemoryException OrElse (t.Message IsNot Nothing AndAlso t.Message.ToLowerInvariant().Contains("failed to allocate"))
        End Function

        ''' <summary>Java's <c>String.split(regex)</c>: the separator is a regular expression and trailing empty strings are removed.</summary>
        Private Shared Function javaSplit(ByVal s As String, ByVal separator As String) As String()
            Dim parts As New List(Of String)(Regex.Split(s, separator))
            Do While parts.Count > 1 AndAlso parts(parts.Count - 1).Length = 0
                parts.RemoveAt(parts.Count - 1)
            Loop
            If parts.Count = 1 AndAlso parts(0).Length = 0 AndAlso s.Length > 0 Then
                parts.Clear()
            End If
            Return parts.ToArray()
        End Function

    End Class

End Namespace
