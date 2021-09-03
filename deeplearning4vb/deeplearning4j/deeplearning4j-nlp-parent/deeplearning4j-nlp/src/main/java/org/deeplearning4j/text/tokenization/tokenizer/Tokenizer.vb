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

Namespace org.deeplearning4j.text.tokenization.tokenizer


	Public Interface Tokenizer

		''' <summary>
		''' An iterator for tracking whether
		''' more tokens are left in the iterator not </summary>
		''' <returns> whether there is anymore tokens
		''' to iterate over </returns>
		Function hasMoreTokens() As Boolean

		''' <summary>
		''' The number of tokens in the tokenizer </summary>
		''' <returns> the number of tokens </returns>
		Function countTokens() As Integer

		''' <summary>
		''' The next token (word usually) in the string </summary>
		''' <returns> the next token in the string if any </returns>
		Function nextToken() As String

		''' <summary>
		''' Returns a list of all the tokens </summary>
		''' <returns> a list of all the tokens </returns>
		ReadOnly Property Tokens As IList(Of String)

		''' <summary>
		''' Set the token pre process </summary>
		''' <param name="tokenPreProcessor"> the token pre processor to set </param>
		WriteOnly Property TokenPreProcessor As TokenPreProcess



	End Interface

End Namespace