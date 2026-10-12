Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ToString = lombok.ToString
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @ToString(exclude = "referencedContent") public class LabelledDocument
	Public Class LabelledDocument

		' optional field
		Private id As String

		' initial text representation of current document
		Private content As String


		Private labels As IList(Of String) = New List(Of String)()


	'    
	'        as soon as sentence was parsed for vocabulary words, there's no need to hold string representation, and we can just stick to references to those VocabularyWords
	'      
		Private referencedContent As IList(Of VocabWord)

		''' <summary>
		''' This method returns first label for the document.
		''' 
		''' PLEASE NOTE: This method is here only for backward compatibility purposes, getLabels() should be used instead.
		''' 
		''' @return
		''' </summary>
		<Obsolete>
		Public Overridable Property Label As String
			Get
				Return labels(0)
			End Get
			Set(ByVal label As String)
				If labels.Count > 0 Then
					labels(0) = label
				Else
					labels.Add(label)
				End If
			End Set
		End Property


		Public Overridable Sub addLabel(ByVal label As String)
			labels.Add(label)
		End Sub

	End Class

End Namespace