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

Namespace org.deeplearning4j.ui.api


	Public Interface I18N

		''' <summary>
		''' Get the specified message in the default language (according to <seealso cref="getDefaultLanguage()"/>
		''' </summary>
		''' <param name="key"> Key value </param>
		''' <returns> Message for the given key, or null if none is found/available </returns>
		Function getMessage(ByVal key As String) As String

		''' <summary>
		''' Get the specified message for the specified language
		''' </summary>
		''' <param name="langCode"> ISO 639-1 language code: "en", "ja", etc </param>
		''' <param name="key">      Key value for the message to retrieve </param>
		''' <returns> Message for the given key/language pair, or null if none is found </returns>
		Function getMessage(ByVal langCode As String, ByVal key As String) As String

		''' <summary>
		''' Get the currently set default language as an ISO 639-1 code
		''' </summary>
		''' <returns> The current default language </returns>
		Property DefaultLanguage As String


		''' <summary>
		''' Get all internationalization messages for the specified language code </summary>
		''' <param name="langCode"> Language code </param>
		''' <returns> Messages </returns>
		Function getMessages(ByVal langCode As String) As IDictionary(Of String, String)

	End Interface

End Namespace