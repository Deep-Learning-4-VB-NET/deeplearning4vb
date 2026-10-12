Imports System.IO
Imports TokenPreProcess = org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer

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

Namespace org.deeplearning4j.text.tokenization.tokenizerfactory


	''' <summary>
	''' Generates a tokenizer for a given string
	''' @author Adam Gibson
	''' 
	''' </summary>
	Public Interface TokenizerFactory

		''' <summary>
		''' The tokenizer to createComplex </summary>
		''' <param name="toTokenize"> the string to createComplex the tokenizer with </param>
		''' <returns> the new tokenizer </returns>
		Function create(ByVal toTokenize As String) As Tokenizer

		''' <summary>
		''' Create a tokenizer based on an input stream </summary>
		''' <param name="toTokenize">
		''' @return </param>
		Function create(ByVal toTokenize As Stream) As Tokenizer

		''' <summary>
		''' Sets a token pre processor to be used
		''' with every tokenizer </summary>
		''' <param name="preProcessor"> the token pre processor to use </param>
		Property TokenPreProcessor As TokenPreProcess

	End Interface

End Namespace