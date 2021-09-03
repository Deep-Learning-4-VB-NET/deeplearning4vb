Imports System.Collections.Generic
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.iterator


	Public Interface LabeledPairSentenceProvider

		''' <summary>
		''' Are there more sentences/documents available?
		''' </summary>
		Function hasNext() As Boolean

		''' <returns> Triple: two sentence/document texts and label </returns>
		Function nextSentencePair() As Triple(Of String, String, String)

		''' <summary>
		''' Reset the iterator - including shuffling the order, if necessary/appropriate
		''' </summary>
		Sub reset()

		''' <summary>
		''' Return the total number of sentences, or -1 if not available
		''' </summary>
		Function totalNumSentences() As Integer

		''' <summary>
		''' Return the list of labels - this also defines the class/integer label assignment order
		''' </summary>
		Function allLabels() As IList(Of String)

		''' <summary>
		''' Equivalent to allLabels().size()
		''' </summary>
		Function numLabelClasses() As Integer

	End Interface



End Namespace