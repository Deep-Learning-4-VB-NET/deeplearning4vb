Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement

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

Namespace org.deeplearning4j.models.sequencevectors.transformers

	Public Interface SequenceTransformer(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement, V As Object)

		''' <summary>
		''' Returns Vocabulary derived from underlying data source.
		''' In default implementations this method heavily relies on transformToSequence() method.
		''' 
		''' @return
		''' </summary>
		'VocabCache<T> derivedVocabulary();

		''' <summary>
		''' This is generic method for transformation data from any format to Sequence of SequenceElement.
		''' It will be used both in Vocab building, and in training process
		''' </summary>
		''' <param name="object"> - Object to be transformed into Sequence
		''' @return </param>
		Function transformToSequence(ByVal [object] As V) As Sequence(Of T)


		Sub reset()
	End Interface

End Namespace