Imports System
Imports System.Collections.Generic
Imports System.IO
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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


	''' <summary>
	''' Iterate over files
	''' @author Adam Gibson
	''' 
	''' </summary>
	<Serializable>
	Public Class FileDocumentIterator
		Implements DocumentIterator

		Private iter As IEnumerator(Of File)
		Private lineIterator As LineIterator
		Private rootDir As File
		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(FileDocumentIterator))

		Public Sub New(ByVal path As String)
			Me.New(New File(path))
		End Sub


		Public Sub New(ByVal path As File)
			If path.isFile() Then
				Preconditions.checkState(path.exists(), "File %s does not exist", path)
				Preconditions.checkState(path.length() > 0, "Cannot iterate over empty file: %s", path)
				iter = Collections.singletonList(path).GetEnumerator()
				Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					lineIterator = FileUtils.lineIterator(iter.next())
				Catch e As IOException
					Throw New Exception(e)
				End Try
				Me.rootDir = path
			Else
				Dim fileList As ICollection(Of File) = FileUtils.listFiles(path, Nothing, True)
				Dim nonEmpty As IList(Of File) = New List(Of File)()
				For Each f As File In fileList
					If f.length() > 0 Then
						nonEmpty.Add(f)
					End If
				Next f
				Preconditions.checkState(nonEmpty.Count > 0, "No (non-empty) files were found at path %s", path)
				iter = nonEmpty.GetEnumerator()
				Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					lineIterator = FileUtils.lineIterator(iter.next())
				Catch e As IOException
					Throw New Exception(e)
				End Try
				Me.rootDir = path
			End If


		End Sub

		Public Overridable Function nextDocument() As Stream Implements DocumentIterator.nextDocument
			SyncLock Me
				Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If lineIterator IsNot Nothing AndAlso Not lineIterator.hasNext() AndAlso iter.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim [next] As File = iter.next()
						lineIterator.close()
						lineIterator = FileUtils.lineIterator([next])
						Do While Not lineIterator.hasNext()
							lineIterator.close()
							lineIterator = FileUtils.lineIterator([next])
						Loop
					End If
        
					If lineIterator IsNot Nothing AndAlso lineIterator.hasNext() Then
						Return New BufferedInputStream(IOUtils.toInputStream(lineIterator.nextLine()))
					End If
				Catch e As Exception
					log.warn("Error reading input stream...this is just a warning..Going to return", e)
					Return Nothing
				End Try
        
				Return Nothing
			End SyncLock
		End Function

		Public Overridable Function hasNext() As Boolean Implements DocumentIterator.hasNext
			SyncLock Me
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return iter.hasNext() OrElse lineIterator IsNot Nothing AndAlso lineIterator.hasNext()
			End SyncLock
		End Function

		Public Overridable Sub reset() Implements DocumentIterator.reset
			If rootDir.isDirectory() Then
				iter = FileUtils.iterateFiles(rootDir, Nothing, True)
			Else
				iter = java.util.Arrays.asList(rootDir).GetEnumerator()
			End If

		End Sub

	End Class

End Namespace