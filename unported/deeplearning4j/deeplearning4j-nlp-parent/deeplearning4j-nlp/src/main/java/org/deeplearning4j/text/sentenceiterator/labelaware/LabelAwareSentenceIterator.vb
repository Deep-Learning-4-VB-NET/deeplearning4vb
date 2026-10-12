Imports System.Collections.Generic
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator

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


	Public Interface LabelAwareSentenceIterator
		Inherits SentenceIterator

		''' <summary>
		''' Returns the current label for nextSentence() </summary>
		''' <returns> the label for nextSentence() </returns>
		Function currentLabel() As String


		''' <summary>
		''' For multi label problems
		''' @return
		''' </summary>
		Function currentLabels() As IList(Of String)

	End Interface

End Namespace