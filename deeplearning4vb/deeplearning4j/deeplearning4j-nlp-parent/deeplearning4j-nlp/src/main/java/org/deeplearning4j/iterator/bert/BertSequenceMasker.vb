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

Namespace org.deeplearning4j.iterator.bert


	Public Interface BertSequenceMasker

		''' 
		''' <param name="input">         Input sequence of tokens </param>
		''' <param name="maskToken">     Token to use for masking - usually something like "[MASK]" </param>
		''' <param name="vocabWords">    Vocabulary, as a list </param>
		''' <returns> Pair: The new input tokens (after masking out), along with a boolean[] for whether the token is
		''' masked or not (same length as number of tokens). boolean[i] is true if token i was masked. </returns>
		Function maskSequence(ByVal input As IList(Of String), ByVal maskToken As String, ByVal vocabWords As IList(Of String)) As Pair(Of IList(Of String), Boolean())

	End Interface

End Namespace