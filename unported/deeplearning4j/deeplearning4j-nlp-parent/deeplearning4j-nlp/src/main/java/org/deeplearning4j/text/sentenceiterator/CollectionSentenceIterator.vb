Imports System.Collections.Generic

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


	Public Class CollectionSentenceIterator
		Inherits BaseSentenceIterator

		Private iter As IEnumerator(Of String)
		Private coll As ICollection(Of String)

		Public Sub New(ByVal preProcessor As SentencePreProcessor, ByVal coll As ICollection(Of String))
			MyBase.New(preProcessor)
			Me.coll = coll
			iter = coll.GetEnumerator()
		End Sub

		Public Sub New(ByVal coll As ICollection(Of String))
			Me.New(Nothing, coll)
		End Sub

		Public Overrides Function nextSentence() As String
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ret As String = iter.next()
			If Me.PreProcessor IsNot Nothing Then
				ret = Me.PreProcessor.preProcess(ret)
			End If
			Return ret
		End Function

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iter.hasNext()
		End Function


		Public Overrides Sub reset()
			iter = coll.GetEnumerator()
		End Sub



	End Class

End Namespace