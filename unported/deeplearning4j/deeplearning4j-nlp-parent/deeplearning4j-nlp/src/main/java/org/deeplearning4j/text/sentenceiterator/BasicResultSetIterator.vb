Imports System

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


	Public Class BasicResultSetIterator
		Implements SentenceIterator

		Private rs As ResultSet
		Private columnName As String

'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As SentencePreProcessor

		Private nextCalled As Boolean ' we use this to ensure that next is only called once by hasNext() to ensure we don't skip over data
		Private resultOfNext As Boolean

		Public Sub New(ByVal rs As ResultSet, ByVal columnName As String)
			Me.rs = rs
			Me.columnName = columnName

			Me.nextCalled = False
			Me.resultOfNext = False
		End Sub

		Public Overridable Function nextSentence() As String Implements SentenceIterator.nextSentence
			SyncLock Me
				Try
					If Not nextCalled Then ' move onto the next row if we haven't yet
						rs.next()
					Else
						nextCalled = False ' reset that next has been called for next time we call nextSentence() or hasNext()
					End If
					Return If(preProcessor_Conflict IsNot Nothing, Me.preProcessor_Conflict.preProcess(rs.getString(columnName)), rs.getString(columnName))
				Catch e As SQLException
					Throw New Exception(e)
				End Try
			End SyncLock
		End Function

		Public Overridable Function hasNext() As Boolean Implements SentenceIterator.hasNext
			SyncLock Me
				Try
					If Not nextCalled Then
						resultOfNext = rs.next()
						nextCalled = True
					End If
					Return resultOfNext
				Catch e As SQLException
					Return False
				End Try
			End SyncLock
		End Function

		Public Overridable Sub reset() Implements SentenceIterator.reset
			SyncLock Me
				Try
					rs.beforeFirst()
					nextCalled = False
				Catch e As SQLException
					Throw New Exception(e)
				End Try
			End SyncLock
		End Sub

		Public Overridable Sub finish() Implements SentenceIterator.finish
			Try
				rs.close()
			Catch e As SQLException
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

	End Class

End Namespace