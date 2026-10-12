Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports FileUtils = org.apache.commons.io.FileUtils
Imports JavaType = org.nd4j.shade.jackson.databind.JavaType
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.nd4j.common.validation


	Public Class Nd4jCommonValidator

		Private Sub New()
		End Sub

		''' <summary>
		''' Validate whether the specified file is a valid file (must exist and be non-empty)
		''' </summary>
		''' <param name="f"> File to check </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static ValidationResult isValidFile(@NonNull File f)
		Public Shared Function isValidFile(ByVal f As File) As ValidationResult
			Dim vr As ValidationResult = isValidFile(f, "File", False)
			If vr IsNot Nothing Then
				Return vr
			End If
			Return ValidationResult.builder().valid(True).formatType("File").path(getPath(f)).build()
		End Function

		''' <summary>
		''' Validate whether the specified file is a valid file
		''' </summary>
		''' <param name="f">          File to check </param>
		''' <param name="formatType"> Name of the file format to include in validation results </param>
		''' <param name="allowEmpty"> If true: allow empty files to pass. False: empty files will fail validation </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static ValidationResult isValidFile(@NonNull File f, String formatType, boolean allowEmpty)
		Public Shared Function isValidFile(ByVal f As File, ByVal formatType As String, ByVal allowEmpty As Boolean) As ValidationResult
			Dim path As String
			Try
				path = f.getAbsolutePath() 'Very occasionally: getAbsolutePath not possible (files in JARs etc)
			Catch t As Exception
				path = f.getPath()
			End Try

			If f.exists() AndAlso Not f.isFile() Then
				Return ValidationResult.builder().valid(False).formatType(formatType).path(path).issues(Collections.singletonList(If(f.isDirectory(), "Specified path is a directory", "Specified path is not a file"))).build()
			End If

			If Not f.exists() OrElse Not f.isFile() Then
				Return ValidationResult.builder().valid(False).formatType(formatType).path(path).issues(Collections.singletonList("File does not exist")).build()
			End If

			If Not allowEmpty AndAlso f.length() <= 0 Then
				Return ValidationResult.builder().valid(False).formatType(formatType).path(path).issues(Collections.singletonList("File is empty (length 0)")).build()
			End If

			Return Nothing 'OK
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static ValidationResult isValidJsonUTF8(@NonNull File f)
		Public Shared Function isValidJsonUTF8(ByVal f As File) As ValidationResult
			Return isValidJson(f, StandardCharsets.UTF_8)
		End Function

		''' <summary>
		''' Validate whether the specified file is a valid JSON file. Note that this does not match the JSON content against a specific schema
		''' </summary>
		''' <param name="f">       File to check </param>
		''' <param name="charset"> Character set for file </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static ValidationResult isValidJson(@NonNull File f, java.nio.charset.Charset charset)
		Public Shared Function isValidJson(ByVal f As File, ByVal charset As Charset) As ValidationResult

			Dim vr As ValidationResult = isValidFile(f, "JSON", False)
			If vr IsNot Nothing Then
				Return vr
			End If

			Dim content As String
			Try
				content = FileUtils.readFileToString(f, charset)
			Catch e As IOException
				Return ValidationResult.builder().valid(False).formatType("JSON").path(getPath(f)).issues(Collections.singletonList("Unable to read file (IOException)")).exception(e).build()
			End Try


			Return isValidJson(content, f)
		End Function

		''' <summary>
		''' Validate whether the specified String is valid JSON. Note that this does not match the JSON content against a specific schema
		''' </summary>
		''' <param name="s"> JSON String to check </param>
		''' <returns> Result of validation </returns>
		Public Shared Function isValidJSON(ByVal s As String) As ValidationResult
			Return isValidJson(s, Nothing)
		End Function


		Protected Friend Shared Function isValidJson(ByVal content As String, ByVal f As File) As ValidationResult
			Try
				Dim om As New ObjectMapper()
				Dim javaType As JavaType = om.getTypeFactory().constructMapType(GetType(System.Collections.IDictionary), GetType(String), GetType(Object))
				om.readValue(content, javaType) 'Don't care about result, just that it can be parsed successfully
			Catch t As Exception
				'Jackson should tell us specifically where error occurred also
				Return ValidationResult.builder().valid(False).formatType("JSON").path(getPath(f)).issues(Collections.singletonList("File does not appear to be valid JSON")).exception(t).build()
			End Try


			Return ValidationResult.builder().valid(True).formatType("JSON").path(getPath(f)).build()
		End Function


		''' <summary>
		''' Validate whether the specified file is a valid Zip file
		''' </summary>
		''' <param name="f">          File to check </param>
		''' <param name="allowEmpty"> If true: allow empty zip files to pass validation. False: empty zip files will fail validation. </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static ValidationResult isValidZipFile(@NonNull File f, boolean allowEmpty)
		Public Shared Function isValidZipFile(ByVal f As File, ByVal allowEmpty As Boolean) As ValidationResult
			Return isValidZipFile(f, allowEmpty, DirectCast(Nothing, IList(Of String)))
		End Function

		''' <summary>
		''' Validate whether the specified file is a valid Zip file
		''' </summary>
		''' <param name="f">          File to check </param>
		''' <param name="allowEmpty"> If true: allow empty zip files to pass validation. False: empty zip files will fail validation. </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static ValidationResult isValidZipFile(@NonNull File f, boolean allowEmpty, String... requiredEntries)
		Public Shared Function isValidZipFile(ByVal f As File, ByVal allowEmpty As Boolean, ParamArray ByVal requiredEntries() As String) As ValidationResult
			Return isValidZipFile(f, allowEmpty,If(requiredEntries Is Nothing, Nothing, java.util.Arrays.asList(requiredEntries)))
		End Function

		''' <summary>
		''' Validate whether the specified file is a valid Zip file, and contains all of the required entries
		''' </summary>
		''' <param name="f">               File to check </param>
		''' <param name="allowEmpty">      If true: allow empty zip files to pass validation. False: empty zip files will fail validation. </param>
		''' <param name="requiredEntries"> If non-null, all of the specified entries must be present for the file to pass validation </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static ValidationResult isValidZipFile(@NonNull File f, boolean allowEmpty, List<String> requiredEntries)
		Public Shared Function isValidZipFile(ByVal f As File, ByVal allowEmpty As Boolean, ByVal requiredEntries As IList(Of String)) As ValidationResult
			Dim vr As ValidationResult = isValidFile(f, "Zip File", False)
			If vr IsNot Nothing Then
				Return vr
			End If

			Dim zf As ZipFile
			Try
				zf = New ZipFile(f)
			Catch e As Exception
				Return ValidationResult.builder().valid(False).formatType("Zip File").path(getPath(f)).issues(Collections.singletonList("File does not appear to be valid zip file (not a zip file or content is corrupt)")).exception(e).build()
			End Try

			Try
				Dim numEntries As Integer = zf.size()
				If Not allowEmpty AndAlso numEntries <= 0 Then
					Return ValidationResult.builder().valid(False).formatType("Zip File").path(getPath(f)).issues(Collections.singletonList("Zip file is empty")).build()
				End If

				If requiredEntries IsNot Nothing AndAlso requiredEntries.Count > 0 Then
					Dim missing As IList(Of String) = Nothing
					For Each s As String In requiredEntries
						Dim ze As ZipEntry = zf.getEntry(s)
						If ze Is Nothing Then
							If missing Is Nothing Then
								missing = New List(Of String)()
							End If
							missing.Add(s)
						End If
					Next s

					If missing IsNot Nothing Then
						Dim s As String = "Zip file is missing " & missing.Count & " of " & requiredEntries.Count & " required entries: " & missing
						Return ValidationResult.builder().valid(False).formatType("Zip File").path(getPath(f)).issues(Collections.singletonList(s)).build()
					End If
				End If

			Catch t As Exception
				Return ValidationResult.builder().valid(False).formatType("Zip File").path(getPath(f)).issues(Collections.singletonList("Error reading zip file")).exception(t).build()
			Finally
				Try
					zf.close()
				Catch e As IOException
				End Try 'Ignore, can't do anything about it...
			End Try

			Return ValidationResult.builder().valid(True).formatType("Zip File").path(getPath(f)).build()
		End Function


		''' <summary>
		''' Null-safe and "no absolute path exists" safe method for getting the path of a file for validation purposes
		''' </summary>
		Public Shared Function getPath(ByVal f As File) As String
			If f Is Nothing Then
				Return Nothing
			End If
			Try
				Return f.getAbsolutePath() 'Very occasionally: getAbsolutePath not possible (files in JARs etc)
			Catch t As Exception
				Return f.getPath()
			End Try
		End Function


	End Class

End Namespace