Imports System.IO
Imports DefaultStreamTokenizer = org.deeplearning4j.text.tokenization.tokenizer.DefaultStreamTokenizer
Imports DefaultTokenizer = org.deeplearning4j.text.tokenization.tokenizer.DefaultTokenizer
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
	''' Default tokenizer based on string tokenizer or stream tokenizer
	''' @author Adam Gibson
	''' </summary>
	Public Class DefaultTokenizerFactory
		Implements TokenizerFactory

		Private tokenPreProcess As TokenPreProcess

		Public Overridable Function create(ByVal toTokenize As String) As Tokenizer Implements TokenizerFactory.create
			Dim t As New DefaultTokenizer(toTokenize)
			t.TokenPreProcessor = tokenPreProcess
			Return t
		End Function

		Public Overridable Function create(ByVal toTokenize As Stream) As Tokenizer Implements TokenizerFactory.create
			Dim t As Tokenizer = New DefaultStreamTokenizer(toTokenize)
			t.TokenPreProcessor = tokenPreProcess
			Return t
		End Function

		Public Overridable Property TokenPreProcessor Implements TokenizerFactory.setTokenPreProcessor As TokenPreProcess
			Set(ByVal preProcessor As TokenPreProcess)
				Me.tokenPreProcess = preProcessor
			End Set
			Get
				Return tokenPreProcess
			End Get
		End Property



	End Class

End Namespace