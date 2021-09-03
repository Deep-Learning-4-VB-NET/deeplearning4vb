Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports NonNull = lombok.NonNull

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

Namespace org.deeplearning4j.text.documentiterator


	Public Class FilenamesLabelAwareIterator
		Implements LabelAwareIterator

		Protected Friend files As IList(Of File)
		Protected Friend position As New AtomicInteger(0)
'JAVA TO VB CONVERTER NOTE: The field labelsSource was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend labelsSource_Conflict As LabelsSource
		Protected Friend absPath As Boolean = False

	'    
	'        Please keep this method protected, it's used in tests
	'     
		Protected Friend Sub New()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected FilenamesLabelAwareIterator(@NonNull List<java.io.File> files, @NonNull LabelsSource source)
		Protected Friend Sub New(ByVal files As IList(Of File), ByVal source As LabelsSource)
			Me.files = files
			Me.labelsSource_Conflict = source
		End Sub

		Public Overridable Function hasNextDocument() As Boolean Implements LabelAwareIterator.hasNextDocument
			Return position.get() < files.Count
		End Function


		Public Overridable Function nextDocument() As LabelledDocument Implements LabelAwareIterator.nextDocument
			Dim fileToRead As File = files(position.getAndIncrement())
			Dim label As String = If(absPath, fileToRead.getAbsolutePath(), fileToRead.getName())
			labelsSource_Conflict.storeLabel(label)
			Try
				Dim document As New LabelledDocument()
				Dim reader As New StreamReader(fileToRead)
				Dim builder As New StringBuilder()
				Dim line As String = ""
				line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
				Do While line IsNot Nothing
					builder.Append(line).Append(" ")
						line = reader.ReadLine()
				Loop

				document.setContent(builder.ToString())
				document.addLabel(label)

				Try
					reader.Close()
				Catch e As Exception
					Throw New Exception(e)
				End Try

				Return document
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Overrides Function hasNext() As Boolean
			Return hasNextDocument()
		End Function

		Public Overrides Function [next]() As LabelledDocument
			Return nextDocument()
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub

		Public Overridable Sub shutdown() Implements LabelAwareIterator.shutdown
			' no-op
		End Sub

		Public Overridable Sub reset() Implements LabelAwareIterator.reset
			position.set(0)
		End Sub

		Public Overridable ReadOnly Property LabelsSource As LabelsSource Implements LabelAwareIterator.getLabelsSource
			Get
				Return labelsSource_Conflict
			End Get
		End Property

		Public Class Builder
			Protected Friend foldersToScan As IList(Of File) = New List(Of File)()

			Friend fileList As IList(Of File) = New List(Of File)()
			Friend labels As IList(Of String) = New List(Of String)()
			Friend absPath As Boolean = False

			Public Sub New()

			End Sub

			''' <summary>
			''' Root folder for labels -> documents.
			''' Each subfolder name will be presented as label, and contents of this folder will be represented as LabelledDocument, with label attached
			''' </summary>
			''' <param name="folder"> folder to be scanned for labels and files
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addSourceFolder(@NonNull File folder)
			Public Overridable Function addSourceFolder(ByVal folder As File) As Builder
				foldersToScan.Add(folder)
				Return Me
			End Function

			Public Overridable Function useAbsolutePathAsLabel(ByVal reallyUse As Boolean) As Builder
				Me.absPath = reallyUse
				Return Me
			End Function

			Friend Overridable Sub scanFolder(ByVal folderToScan As File)
				Dim files() As File = folderToScan.listFiles()
				If files Is Nothing OrElse files.Length = 0 Then
					Return
				End If


				For Each fileLabel As File In files
					If fileLabel.isDirectory() Then
						scanFolder(fileLabel)
					Else
						fileList.Add(fileLabel)
					End If
				Next fileLabel
			End Sub

			Public Overridable Function build() As FilenamesLabelAwareIterator
				' search for all files in all folders provided


				For Each file As File In foldersToScan
					If Not file.isDirectory() Then
						Continue For
					End If
					scanFolder(file)
				Next file

				Dim source As New LabelsSource(labels)
				Dim iterator As New FilenamesLabelAwareIterator(fileList, source)
				iterator.absPath = Me.absPath

				Return iterator
			End Function
		End Class
	End Class

End Namespace