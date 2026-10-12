Imports System
Imports System.Collections.Generic
Imports System.IO
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator

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

Namespace org.deeplearning4j.text.sentenceiterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public class FileSentenceIterator extends BaseSentenceIterator
	Public Class FileSentenceIterator
		Inherits BaseSentenceIterator

	'    
	'     * Used as a pair for when
	'     * the number of sentences is not known
	'     
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile java.util.Iterator<java.io.File> fileIterator;
		Protected Friend fileIterator As IEnumerator(Of File)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile java.util.Queue<String> cache;
		Protected Friend cache As LinkedList(Of String)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile org.apache.commons.io.LineIterator currLineIterator;
		Protected Friend currLineIterator As LineIterator
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile java.io.File file;
		Protected Friend file As File
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile java.io.File currentFile;
		Protected Friend currentFile As File

		''' <summary>
		''' Takes a single file or directory
		''' </summary>
		''' <param name="preProcessor"> the sentence pre processor </param>
		''' <param name="file">         the file or folder to iterate over </param>
		Public Sub New(ByVal preProcessor As SentencePreProcessor, ByVal file As File)
			MyBase.New(preProcessor)
			Me.file = file
			cache = New java.util.concurrent.ConcurrentLinkedDeque(Of String)()
			If file.isDirectory() Then
				fileIterator = FileUtils.iterateFiles(file, Nothing, True)
			Else
				fileIterator = Arrays.asList(file).GetEnumerator()
			End If
		End Sub

		Public Sub New(ByVal dir As File)
			Me.New(Nothing, dir)
		End Sub


		Public Overrides Function nextSentence() As String
			Dim ret As String = Nothing
			If cache.Count > 0 Then
				ret = cache.RemoveFirst()
				If preProcessor_Conflict IsNot Nothing Then
					ret = preProcessor_Conflict.preProcess(ret)
				End If
				Return ret
			Else

				If currLineIterator Is Nothing OrElse Not currLineIterator.hasNext() Then
					nextLineIter()
				End If

				For i As Integer = 0 To 99999
					If currLineIterator IsNot Nothing AndAlso currLineIterator.hasNext() Then
						Dim line As String = currLineIterator.nextLine()
						If line IsNot Nothing Then
							cache.AddLast(line)
						Else
							Exit For
						End If
					Else
						Exit For
					End If
				Next i

				If cache.Count > 0 Then
					ret = cache.RemoveFirst()
					If preProcessor_Conflict IsNot Nothing Then
						ret = preProcessor_Conflict.preProcess(ret)
					End If
					Return ret
				End If

			End If


			If cache.Count > 0 Then
				ret = cache.RemoveFirst()
			End If
			Return ret

		End Function


		Private Sub nextLineIter()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If fileIterator.hasNext() Then
				Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim [next] As File = fileIterator.next()
					currentFile = [next]
					If [next].getAbsolutePath().EndsWith(".gz") Then
						If currLineIterator IsNot Nothing Then
							currLineIterator.close()
						End If
						currLineIterator = IOUtils.lineIterator(New BufferedInputStream(New GZIPInputStream(New FileStream([next], FileMode.Open, FileAccess.Read))), "UTF-8")

					Else
						If currLineIterator IsNot Nothing Then
							currLineIterator.close()
						End If
						currLineIterator = FileUtils.lineIterator([next])

					End If
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End If
		End Sub

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return currLineIterator IsNot Nothing AndAlso currLineIterator.hasNext() OrElse fileIterator.hasNext() OrElse cache.Count > 0
		End Function


		Public Overrides Sub reset()
			If file.isFile() Then
				fileIterator = Arrays.asList(file).GetEnumerator()
			Else
				fileIterator = FileUtils.iterateFiles(file, Nothing, True)
			End If


		End Sub


	End Class

End Namespace