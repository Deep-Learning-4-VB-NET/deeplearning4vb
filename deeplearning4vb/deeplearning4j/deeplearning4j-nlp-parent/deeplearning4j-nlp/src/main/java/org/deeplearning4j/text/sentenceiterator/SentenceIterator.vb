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

	Public Interface SentenceIterator

		''' <summary>
		''' Gets the next sentence or null
		''' if there's nothing left (Do yourself a favor and
		''' check hasNext() )
		''' </summary>
		''' <returns> the next sentence in the iterator </returns>
		Function nextSentence() As String

		''' <summary>
		''' Same idea as <seealso cref="System.Collections.IEnumerator"/> </summary>
		''' <returns> whether there's anymore sentences left </returns>
		Function hasNext() As Boolean

		''' <summary>
		''' Resets the iterator to the beginning
		''' </summary>
		Sub reset()

		''' <summary>
		''' Allows for any finishing (closing of input streams or the like)
		''' </summary>
		Sub finish()


		Property PreProcessor As SentencePreProcessor



	End Interface

End Namespace