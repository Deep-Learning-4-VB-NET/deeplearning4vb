Imports System
Imports System.IO
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DocumentIterator = org.deeplearning4j.text.documentiterator.DocumentIterator
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

Namespace org.deeplearning4j.text.sentenceiterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class StreamLineIterator implements SentenceIterator
	Public Class StreamLineIterator
		Implements SentenceIterator

		Private iterator As DocumentIterator
		Private linesToFetch As Integer
		Private ReadOnly buffer As New LinkedBlockingQueue(Of String)()
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As SentencePreProcessor

		Private currentReader As StreamReader

		Protected Friend logger As Logger = LoggerFactory.getLogger(GetType(StreamLineIterator))

		Private Sub New(ByVal iterator As DocumentIterator)
			Me.iterator = iterator
		End Sub

		Private Sub fetchLines(ByVal linesToFetch As Integer)
			Dim line As String = ""
			Dim cnt As Integer = 0
			Try
				line = currentReader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while (cnt < linesToFetch && (line = currentReader.readLine()) != null)
				Do While cnt < linesToFetch AndAlso line IsNot Nothing
					buffer.add(line)
					cnt += 1
						line = currentReader.ReadLine()
				Loop

				' in this case we nullify currentReader as sign of finished reading
				If line Is Nothing Then
					currentReader.Close()
					currentReader = Nothing
				End If
			Catch e As IOException
				log.error("",e)
				Throw New Exception(e)
			End Try
		End Sub

		Public Overridable Function nextSentence() As String Implements SentenceIterator.nextSentence
			If buffer.size() < linesToFetch Then
				' prefetch
				If currentReader IsNot Nothing Then
					fetchLines(linesToFetch)
				ElseIf Me.iterator.hasNext() Then
					currentReader = New StreamReader(iterator.nextDocument())
					fetchLines(linesToFetch)
				End If
			End If

			' actually its the same. You get string or you get null as result of poll, if buffer is empty after prefetch try
			If buffer.isEmpty() Then
				Return Nothing
			Else
				Return buffer.poll()
			End If
		End Function

		Public Overridable Function hasNext() As Boolean Implements SentenceIterator.hasNext
			Try
				Return Not buffer.isEmpty() OrElse iterator.hasNext() OrElse (currentReader IsNot Nothing AndAlso currentReader.ready())
			Catch e As IOException
				' this exception is possible only at currentReader.ready(), so it means that it's definitely NOT ready
				Return False
			End Try
		End Function

		Public Overridable Sub reset() Implements SentenceIterator.reset
			iterator.reset()
		End Sub

		Public Overridable Sub finish() Implements SentenceIterator.finish
			buffer.clear()
		End Sub

		Public Overridable Property PreProcessor As SentencePreProcessor Implements SentenceIterator.getPreProcessor
			Get
				Return Me.preProcessor_Conflict
			End Get
			Set(ByVal preProcessor As SentencePreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property


		Public Class Builder
			Friend iterator As DocumentIterator
			Friend linesToFetch As Integer = 50
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend preProcessor_Conflict As SentencePreProcessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull final java.io.InputStream stream)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
			Public Sub New(ByVal stream As Stream)
				Me.New(New DocumentIteratorAnonymousInnerClass(Me, stream))
			End Sub

			Private Class DocumentIteratorAnonymousInnerClass
				Implements DocumentIterator

				Private ReadOnly outerInstance As Builder

				Private stream As Stream

				Public Sub New(ByVal outerInstance As Builder, ByVal stream As Stream)
					Me.outerInstance = outerInstance
					Me.stream = stream
					onlyStream = stream
					isConsumed = New AtomicBoolean(False)
				End Sub

				Private ReadOnly onlyStream As Stream
				Private isConsumed As AtomicBoolean

				Public Function hasNext() As Boolean Implements DocumentIterator.hasNext
					Return Not isConsumed.get()
				End Function

				Public Function nextDocument() As Stream Implements DocumentIterator.nextDocument
					isConsumed.set(True)
					Return Me.onlyStream
				End Function

				Public Sub reset() Implements DocumentIterator.reset
					isConsumed.set(False)
					Try
						Me.onlyStream.reset()
					Catch e As IOException
						log.error("",e)
						Throw New Exception(e)
					End Try
				End Sub
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull DocumentIterator iterator)
			Public Sub New(ByVal iterator As DocumentIterator)
				Me.iterator = iterator
			End Sub

			Public Overridable Function setFetchSize(ByVal linesToFetch As Integer) As Builder
				Me.linesToFetch = linesToFetch
				Return Me
			End Function

			Public Overridable Function setPreProcessor(ByVal preProcessor As SentencePreProcessor) As Builder
				Me.preProcessor_Conflict = preProcessor
				Return Me
			End Function

			Public Overridable Function build() As StreamLineIterator
				Dim lineIterator As New StreamLineIterator(Me.iterator)
				lineIterator.linesToFetch = linesToFetch
				lineIterator.PreProcessor = preProcessor_Conflict

				Return lineIterator
			End Function
		End Class
	End Class

End Namespace