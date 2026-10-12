Imports System.IO
Imports NGramTokenizer = org.deeplearning4j.text.tokenization.tokenizer.NGramTokenizer
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
	''' @author sonali
	''' </summary>
	Public Class NGramTokenizerFactory
		Implements TokenizerFactory

		Private preProcess As TokenPreProcess
		Private minN As Integer? = 1
		Private maxN As Integer? = 1
		Private tokenizerFactory As TokenizerFactory

		Public Sub New(ByVal tokenizerFactory As TokenizerFactory, ByVal minN As Integer?, ByVal maxN As Integer?)
			Me.tokenizerFactory = tokenizerFactory
			Me.minN = minN
			Me.maxN = maxN
		End Sub

		Public Overridable Function create(ByVal toTokenize As String) As Tokenizer Implements TokenizerFactory.create
			Dim t1 As Tokenizer = tokenizerFactory.create(toTokenize)
			t1.TokenPreProcessor = preProcess
			Dim ret As Tokenizer = New NGramTokenizer(t1, minN, maxN)
			Return ret
		End Function

		Public Overridable Function create(ByVal toTokenize As Stream) As Tokenizer Implements TokenizerFactory.create
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Property TokenPreProcessor Implements TokenizerFactory.setTokenPreProcessor As TokenPreProcess
			Set(ByVal preProcessor As TokenPreProcess)
				Me.preProcess = preProcessor
			End Set
			Get
				Return preProcess
			End Get
		End Property

	End Class

End Namespace