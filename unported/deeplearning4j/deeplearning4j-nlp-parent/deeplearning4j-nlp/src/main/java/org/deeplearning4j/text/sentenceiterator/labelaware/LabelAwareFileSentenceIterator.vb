Imports System.Collections.Generic
Imports FileSentenceIterator = org.deeplearning4j.text.sentenceiterator.FileSentenceIterator
Imports SentencePreProcessor = org.deeplearning4j.text.sentenceiterator.SentencePreProcessor

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

Namespace org.deeplearning4j.text.sentenceiterator.labelaware


	Public Class LabelAwareFileSentenceIterator
		Inherits FileSentenceIterator
		Implements LabelAwareSentenceIterator

		''' <summary>
		''' Takes a single file or directory
		''' </summary>
		''' <param name="preProcessor"> the sentence pre processor </param>
		''' <param name="file">         the file or folder to iterate over </param>
		Public Sub New(ByVal preProcessor As SentencePreProcessor, ByVal file As File)
			MyBase.New(preProcessor, file)
		End Sub

		Public Sub New(ByVal dir As File)
			MyBase.New(dir)
		End Sub

		Public Overridable Function currentLabel() As String Implements LabelAwareSentenceIterator.currentLabel
			Return currentFile.getParentFile().getName()
		End Function

		Public Overridable Function currentLabels() As IList(Of String) Implements LabelAwareSentenceIterator.currentLabels
			Return New List(Of String) From {currentFile.getParentFile().getName()}
		End Function
	End Class

End Namespace