Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports ImageObject = org.datavec.image.recordreader.objdetect.ImageObject
Imports ImageObjectLabelProvider = org.datavec.image.recordreader.objdetect.ImageObjectLabelProvider

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

Namespace org.datavec.image.recordreader.objdetect.impl


	Public Class VocLabelProvider
		Implements ImageObjectLabelProvider

		Private Const OBJECT_START_TAG As String = "<object>"
		Private Const OBJECT_END_TAG As String = "</object>"
		Private Const NAME_TAG As String = "<name>"
		Private Const XMIN_TAG As String = "<xmin>"
		Private Const YMIN_TAG As String = "<ymin>"
		Private Const XMAX_TAG As String = "<xmax>"
		Private Const YMAX_TAG As String = "<ymax>"

		Private annotationsDir As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VocLabelProvider(@NonNull String baseDirectory)
		Public Sub New(ByVal baseDirectory As String)
			Me.annotationsDir = FilenameUtils.concat(baseDirectory, "Annotations")

			If Not Directory.Exists(annotationsDir) OrElse File.Exists(annotationsDir) Then
				Throw New System.InvalidOperationException("Annotations directory does not exist. Annotation files should be " & "present at baseDirectory/Annotations/nnnnnn.xml. Expected location: " & annotationsDir)
			End If
		End Sub

		Public Overridable Function getImageObjectsForPath(ByVal path As String) As IList(Of ImageObject) Implements ImageObjectLabelProvider.getImageObjectsForPath
			Dim idx As Integer = path.LastIndexOf("/"c)
			idx = Math.Max(idx, path.LastIndexOf("\"c))

			Dim filename As String = path.Substring(idx+1, (path.Length-4) - (idx+1)) '-4: ".jpg"
			Dim xmlPath As String = FilenameUtils.concat(annotationsDir, filename & ".xml")
			Dim xmlFile As New File(xmlPath)
			If Not xmlFile.exists() Then
				Throw New System.InvalidOperationException("Could not find XML file for image " & path & "; expected at " & xmlPath)
			End If

			Dim xmlContent As String
			Try
				xmlContent = FileUtils.readFileToString(xmlFile)
			Catch e As IOException
				Throw New Exception(e)
			End Try

			'Normally we'd use Jackson to parse XML, but Jackson has real trouble with multiple XML elements with
			'  the same name. However, the structure is simple and we can parse it manually (even though it's not
			' the most elegant thing to do :)
			Dim lines() As String = xmlContent.Split(vbLf, True)

			Dim [out] As IList(Of ImageObject) = New List(Of ImageObject)()
			For i As Integer = 0 To lines.Length - 1
				If Not lines(i).Contains(OBJECT_START_TAG) Then
					Continue For
				End If
				Dim name As String = Nothing
				Dim xmin As Integer = Integer.MinValue
				Dim ymin As Integer = Integer.MinValue
				Dim xmax As Integer = Integer.MinValue
				Dim ymax As Integer = Integer.MinValue
				Do While Not lines(i).Contains(OBJECT_END_TAG)
					If name Is Nothing AndAlso lines(i).Contains(NAME_TAG) Then
						Dim idxStartName As Integer = lines(i).IndexOf(">"c) + 1
						Dim idxEndName As Integer = lines(i).LastIndexOf("<"c)
						name = lines(i).Substring(idxStartName, idxEndName - idxStartName)
						i += 1
						Continue Do
					End If
					If xmin = Integer.MinValue AndAlso lines(i).Contains(XMIN_TAG) Then
						xmin = extractAndParse(lines(i))
						i += 1
						Continue Do
					End If
					If ymin = Integer.MinValue AndAlso lines(i).Contains(YMIN_TAG) Then
						ymin = extractAndParse(lines(i))
						i += 1
						Continue Do
					End If
					If xmax = Integer.MinValue AndAlso lines(i).Contains(XMAX_TAG) Then
						xmax = extractAndParse(lines(i))
						i += 1
						Continue Do
					End If
					If ymax = Integer.MinValue AndAlso lines(i).Contains(YMAX_TAG) Then
						ymax = extractAndParse(lines(i))
						i += 1
						Continue Do
					End If

					i += 1
				Loop

				If name Is Nothing Then
					Throw New System.InvalidOperationException("Invalid object format: no name tag found for object in file " & xmlPath)
				End If
				If xmin = Integer.MinValue OrElse ymin = Integer.MinValue OrElse xmax = Integer.MinValue OrElse ymax = Integer.MinValue Then
					Throw New System.InvalidOperationException("Invalid object format: did not find all of xmin/ymin/xmax/ymax tags in " & xmlPath)
				End If

				[out].Add(New ImageObject(xmin, ymin, xmax, ymax, name))
			Next i

			Return [out]
		End Function

		Private Function extractAndParse(ByVal line As String) As Integer
			Dim idxStartName As Integer = line.IndexOf(">"c) + 1
			Dim idxEndName As Integer = line.LastIndexOf("<"c)
			Dim substring As String = line.Substring(idxStartName, idxEndName - idxStartName)
			Return Integer.Parse(substring)
		End Function

		Public Overridable Function getImageObjectsForPath(ByVal uri As URI) As IList(Of ImageObject) Implements ImageObjectLabelProvider.getImageObjectsForPath
			Return getImageObjectsForPath(uri.ToString())
		End Function

	End Class

End Namespace