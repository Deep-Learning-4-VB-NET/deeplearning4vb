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

Namespace org.deeplearning4j.models.sequencevectors.interfaces

	Public Interface SequenceElementFactory(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		''' <summary>
		''' This method builds object from provided JSON
		''' </summary>
		''' <param name="json"> JSON for restored object </param>
		''' <returns> restored object </returns>
		Function deserialize(ByVal json As String) As T

		''' <summary>
		''' This method serializaes object  into JSON string </summary>
		''' <param name="element">
		''' @return </param>
		Function serialize(ByVal element As T) As String
	End Interface

End Namespace