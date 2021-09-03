Imports System
Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j

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
'ORIGINAL LINE: @Slf4j public class BasicLineIterator implements SentenceIterator, Iterable<String>
	Public Class BasicLineIterator
		Implements SentenceIterator, IEnumerable(Of String)

		Private reader As StreamReader
		Private backendStream As Stream
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As SentencePreProcessor
		Private internal As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BasicLineIterator(@NonNull File file) throws FileNotFoundException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Sub New(ByVal file As File)
			Me.New(New FileStream(file, FileMode.Open, FileAccess.Read))
			Me.internal = True
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BasicLineIterator(@NonNull InputStream stream)
		Public Sub New(ByVal stream As Stream)
			Me.backendStream = stream
			reader = New StreamReader(New BufferedInputStream(backendStream, 10 * 1024 * 1024))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BasicLineIterator(@NonNull String filePath) throws FileNotFoundException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Sub New(ByVal filePath As String)
			Me.New(New FileStream(filePath, FileMode.Open, FileAccess.Read))
			Me.internal = True
		End Sub

		Public Overridable Function nextSentence() As String Implements SentenceIterator.nextSentence
			SyncLock Me
				Try
					Return If(preProcessor_Conflict IsNot Nothing, Me.preProcessor_Conflict.preProcess(reader.ReadLine()), reader.ReadLine())
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End SyncLock
		End Function

		Public Overridable Function hasNext() As Boolean Implements SentenceIterator.hasNext
			SyncLock Me
				Try
					Return reader.ready()
				Catch e As Exception
					Return False
				End Try
			End SyncLock
		End Function

		Public Overridable Sub reset() Implements SentenceIterator.reset
			SyncLock Me
				Try
					If TypeOf backendStream Is FileStream Then
						CType(backendStream, FileStream).getChannel().position(0)
					Else
						backendStream.reset()
					End If
					reader = New StreamReader(New BufferedInputStream(backendStream, 10 * 10 * 1024))
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End SyncLock
		End Sub

		Public Overridable Sub finish() Implements SentenceIterator.finish
			Try
				If Me.internal AndAlso backendStream IsNot Nothing Then
					backendStream.Close()
				End If
				If reader IsNot Nothing Then
					reader.Close()
				End If
			Catch e As Exception
				' do nothing here
			End Try
		End Sub

		Public Overridable Property PreProcessor As SentencePreProcessor Implements SentenceIterator.getPreProcessor
			Get
				Return preProcessor_Conflict
			End Get
			Set(ByVal preProcessor As SentencePreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override protected void finalize() throws Throwable
		Protected Overrides Sub Finalize()
			Try
				If Me.internal AndAlso backendStream IsNot Nothing Then
					backendStream.Close()
				End If
				If reader IsNot Nothing Then
					reader.Close()
				End If
			Catch e As Exception
				' do nothing here
				log.error("",e)
			End Try
			MyBase.Finalize()
		End Sub

		''' <summary>
		''' Implentation for Iterable interface.
		''' Please note: each call for iterator() resets underlying SentenceIterator to the beginning;
		''' 
		''' @return
		''' </summary>
		Public Overridable Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerator(Of String).GetEnumerator
			Me.reset()
			Dim ret As IEnumerator(Of String) = New IteratorAnonymousInnerClass(Me)

			Return ret
		End Function

		Private Class IteratorAnonymousInnerClass
			Implements IEnumerator(Of String)

			Private ReadOnly outerInstance As BasicLineIterator

			Public Sub New(ByVal outerInstance As BasicLineIterator)
				Me.outerInstance = outerInstance
			End Sub

			Public Function hasNext() As Boolean
				Return outerInstance.hasNext()
			End Function

			Public Function [next]() As String
				Return outerInstance.nextSentence()
			End Function

			Public Sub remove()
				Throw New System.NotSupportedException()
			End Sub
		End Class
	End Class

End Namespace