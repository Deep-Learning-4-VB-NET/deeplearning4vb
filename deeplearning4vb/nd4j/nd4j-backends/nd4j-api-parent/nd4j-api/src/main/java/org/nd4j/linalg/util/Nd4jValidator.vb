Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports FlatGraph = org.nd4j.graph.FlatGraph
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jCommonValidator = org.nd4j.common.validation.Nd4jCommonValidator
Imports ValidationResult = org.nd4j.common.validation.ValidationResult

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

Namespace org.nd4j.linalg.util


	Public Class Nd4jValidator

		Private Sub New()
		End Sub

		''' <summary>
		''' Validate whether the file represents a valid INDArray (of any data type) saved previously with <seealso cref="Nd4j.saveBinary(INDArray, File)"/>
		''' to be read with <seealso cref="Nd4j.readBinary(File)"/>
		''' </summary>
		''' <param name="f"> File that should represent an INDArray saved with Nd4j.saveBinary </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateINDArrayFile(@NonNull File f)
		Public Shared Function validateINDArrayFile(ByVal f As File) As ValidationResult
			Return validateINDArrayFile(f, DirectCast(Nothing, DataType()))
		End Function

		''' <summary>
		''' Validate whether the file represents a valid INDArray (of one of the allowed/specified data types) saved previously
		''' with <seealso cref="Nd4j.saveBinary(INDArray, File)"/> to be read with <seealso cref="Nd4j.readBinary(File)"/>
		''' </summary>
		''' <param name="f">                  File that should represent an INDArray saved with Nd4j.saveBinary </param>
		''' <param name="allowableDataTypes"> May be null. If non-null, the file must represent one of the specified data types </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateINDArrayFile(@NonNull File f, org.nd4j.linalg.api.buffer.DataType... allowableDataTypes)
		Public Shared Function validateINDArrayFile(ByVal f As File, ParamArray ByVal allowableDataTypes() As DataType) As ValidationResult

			Dim vr As ValidationResult = Nd4jCommonValidator.isValidFile(f, "INDArray File", False)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				vr.setFormatClass(GetType(INDArray))
				Return vr
			End If

			'TODO let's do this without reading the whole thing into memory - check header + length...
			Try 'Using the fact that INDArray.close() exists -> deallocate memory as soon as reading is done
					Using arr As INDArray = Nd4j.readBinary(f)
					If allowableDataTypes IsNot Nothing Then
						ArrayUtils.contains(allowableDataTypes, arr.dataType())
					End If
					End Using
			Catch e As IOException
				Return ValidationResult.builder().valid(False).formatType("INDArray File").formatClass(GetType(INDArray)).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("Unable to read file (IOException)")).exception(e).build()
			Catch t As Exception
				If TypeOf t Is System.OutOfMemoryException OrElse t.getMessage().ToLower().Contains("failed to allocate") Then
					'This is a memory exception during reading... result is indeterminant (might be valid, might not be, can't tell here)
					Return ValidationResult.builder().valid(True).formatType("INDArray File").formatClass(GetType(INDArray)).path(Nd4jCommonValidator.getPath(f)).build()
				End If

				Return ValidationResult.builder().valid(False).formatType("INDArray File").formatClass(GetType(INDArray)).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("File may be corrupt or is not a binary INDArray file")).exception(t).build()
			End Try

			Return ValidationResult.builder().valid(True).formatType("INDArray File").formatClass(GetType(INDArray)).path(Nd4jCommonValidator.getPath(f)).build()
		End Function

		''' <summary>
		''' Validate whether the file represents a valid INDArray text file (of any data type) saved previously with
		''' <seealso cref="Nd4j.writeTxt(INDArray, String)"/> to be read with <seealso cref="Nd4j.readTxt(String)"/> }
		''' </summary>
		''' <param name="f"> File that should represent an INDArray saved with Nd4j.writeTxt </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateINDArrayTextFile(@NonNull File f)
		Public Shared Function validateINDArrayTextFile(ByVal f As File) As ValidationResult

			Dim vr As ValidationResult = Nd4jCommonValidator.isValidFile(f, "INDArray Text File", False)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				vr.setFormatClass(GetType(INDArray))
				Return vr
			End If

			'TODO let's do this without reading the whole thing into memory - check header + length...
			Try 'Using the fact that INDArray.close() exists -> deallocate memory as soon as reading is done
					Using arr As INDArray = Nd4j.readTxt(f.getPath())
					Console.WriteLine()
					End Using
			Catch t As Exception
				If TypeOf t Is System.OutOfMemoryException OrElse t.getMessage().ToLower().Contains("failed to allocate") Then
					'This is a memory exception during reading... result is indeterminant (might be valid, might not be, can't tell here)
					Return ValidationResult.builder().valid(True).formatType("INDArray Text File").formatClass(GetType(INDArray)).path(Nd4jCommonValidator.getPath(f)).build()
				End If

				Return ValidationResult.builder().valid(False).formatType("INDArray Text File").formatClass(GetType(INDArray)).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("File may be corrupt or is not a text INDArray file")).exception(t).build()
			End Try

			Return ValidationResult.builder().valid(True).formatType("INDArray Text File").formatClass(GetType(INDArray)).path(Nd4jCommonValidator.getPath(f)).build()
		End Function

		''' <summary>
		''' Validate whether the file represents a valid Numpy .npy file to be read with <seealso cref="Nd4j.createFromNpyFile(File)"/> }
		''' </summary>
		''' <param name="f"> File that should represent a Numpy .npy file written with Numpy save method </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateNpyFile(@NonNull File f)
		Public Shared Function validateNpyFile(ByVal f As File) As ValidationResult

			Dim vr As ValidationResult = Nd4jCommonValidator.isValidFile(f, "Numpy .npy File", False)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				Return vr
			End If

			'TODO let's do this without reading whole thing into memory
			Try 'Using the fact that INDArray.close() exists -> deallocate memory as soon as reading is done
					Using arr As INDArray = Nd4j.createFromNpyFile(f)
					End Using
			Catch t As Exception
				If TypeOf t Is System.OutOfMemoryException OrElse t.getMessage().ToLower().Contains("failed to allocate") Then
					'This is a memory exception during reading... result is indeterminant (might be valid, might not be, can't tell here)
					Return ValidationResult.builder().valid(True).formatType("Numpy .npy File").path(Nd4jCommonValidator.getPath(f)).build()
				End If

				Return ValidationResult.builder().valid(False).formatType("Numpy .npy File").path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("File may be corrupt or is not a Numpy .npy file")).exception(t).build()
			End Try

			Return ValidationResult.builder().valid(True).formatType("Numpy .npy File").path(Nd4jCommonValidator.getPath(f)).build()
		End Function

		''' <summary>
		''' Validate whether the file represents a valid Numpy .npz file to be read with <seealso cref="Nd4j.createFromNpyFile(File)"/> }
		''' </summary>
		''' <param name="f"> File that should represent a Numpy .npz file written with Numpy savez method </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateNpzFile(@NonNull File f)
		Public Shared Function validateNpzFile(ByVal f As File) As ValidationResult
			Dim vr As ValidationResult = Nd4jCommonValidator.isValidFile(f, "Numpy .npz File", False)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				Return vr
			End If

			Dim m As IDictionary(Of String, INDArray) = Nothing
			Try
				m = Nd4j.createFromNpzFile(f)
			Catch t As Exception
				Return ValidationResult.builder().valid(False).formatType("Numpy .npz File").path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("File may be corrupt or is not a Numpy .npz file")).exception(t).build()
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

			Return ValidationResult.builder().valid(True).formatType("Numpy .npz File").path(Nd4jCommonValidator.getPath(f)).build()
		End Function

		''' <summary>
		''' Validate whether the file represents a valid Numpy text file (written using numpy.savetxt) to be read with
		''' <seealso cref="Nd4j.readNumpy(String)"/> }
		''' </summary>
		''' <param name="f"> File that should represent a Numpy text file written with Numpy savetxt method </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateNumpyTxtFile(@NonNull File f, @NonNull String delimiter, @NonNull Charset charset)
		Public Shared Function validateNumpyTxtFile(ByVal f As File, ByVal delimiter As String, ByVal charset As Charset) As ValidationResult
			Dim vr As ValidationResult = Nd4jCommonValidator.isValidFile(f, "Numpy text file", False)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				Return vr
			End If

			Dim s As String
			Try
				s = FileUtils.readFileToString(f, charset)
			Catch t As Exception
				Return ValidationResult.builder().valid(False).formatType("Numpy text file").path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("File may be corrupt or is not a Numpy text file")).exception(t).build()
			End Try

			Dim lines() As String = s.Split(vbLf, True)
			Dim countPerLine As Integer = 0
			For i As Integer = 0 To lines.Length - 1
				Dim lineSplit() As String = lines(i).Split(delimiter, True)
				If i = 0 Then
					countPerLine = lineSplit.Length
				ElseIf lines(i).Length > 0 Then
					If countPerLine <> lineSplit.Length Then
						Return ValidationResult.builder().valid(False).formatType("Numpy text file").path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("Number of values in each line is not the same for all lines: File may be corrupt, is not a Numpy text file, or delimiter """ & delimiter & """ is incorrect")).build()
					End If
				End If

				For j As Integer = 0 To lineSplit.Length - 1
					Try
						Double.Parse(lineSplit(j))
					Catch e As System.FormatException
						Return ValidationResult.builder().valid(False).formatType("Numpy text file").path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("File may be corrupt, is not a Numpy text file, or delimiter """ & delimiter & """ is incorrect")).exception(e).build()
					End Try
				Next j
			Next i
			Return ValidationResult.builder().valid(True).formatType("Numpy text file").path(Nd4jCommonValidator.getPath(f)).build()
		End Function


		''' <summary>
		''' Validate whether the file represents a valid SameDiff FlatBuffers file, previously saved with <seealso cref="org.nd4j.autodiff.samediff.SameDiff.asFlatFile(File)"/> )
		''' to be read with <seealso cref="org.nd4j.autodiff.samediff.SameDiff.fromFlatFile(File)"/> }
		''' </summary>
		''' <param name="f"> File that should represent a SameDiff FlatBuffers file </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateSameDiffFlatBuffers(@NonNull File f)
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
	End Class

End Namespace