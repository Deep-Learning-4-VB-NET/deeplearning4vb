﻿Imports System
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.deeplearning4j.models.sequencevectors.serialization


	Public Class VocabWordFactory
		Implements SequenceElementFactory(Of VocabWord)

		''' <summary>
		''' This method builds object from provided JSON
		''' </summary>
		''' <param name="json"> JSON for restored object </param>
		''' <returns> restored object </returns>
		Public Overridable Function deserialize(ByVal json As String) As VocabWord
			Dim mapper As ObjectMapper = SequenceElement.mapper()
			Try
				Dim ret As VocabWord = mapper.readValue(json, GetType(VocabWord))
				Return ret
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' This method serializaes object  into JSON string
		''' </summary>
		''' <param name="element">
		''' @return </param>
		Public Overridable Function serialize(ByVal element As VocabWord) As String
			Return element.toJSON()
		End Function
	End Class

End Namespace