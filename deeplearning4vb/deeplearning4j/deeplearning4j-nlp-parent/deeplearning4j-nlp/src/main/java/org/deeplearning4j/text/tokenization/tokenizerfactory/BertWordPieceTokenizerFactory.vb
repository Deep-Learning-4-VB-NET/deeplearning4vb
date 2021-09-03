Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports BertWordPieceStreamTokenizer = org.deeplearning4j.text.tokenization.tokenizer.BertWordPieceStreamTokenizer
Imports BertWordPieceTokenizer = org.deeplearning4j.text.tokenization.tokenizer.BertWordPieceTokenizer
Imports TokenPreProcess = org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports BertWordPiecePreProcessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.BertWordPiecePreProcessor

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


	Public Class BertWordPieceTokenizerFactory
		Implements TokenizerFactory

		Private ReadOnly vocab As NavigableMap(Of String, Integer)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess preTokenizePreProcessor;
		Private preTokenizePreProcessor As TokenPreProcess
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess tokenPreProcessor;
		Private tokenPreProcessor As TokenPreProcess
		Private charset As Charset

		''' <param name="vocab">                   Vocabulary, as a navigable map </param>
		''' <param name="lowerCaseOnly"> If true: tokenization should convert all characters to lower case </param>
		''' <param name="stripAccents">  If true: strip accents off characters. Usually same as lower case. Should be true when using "uncased" official BERT TensorFlow models </param>
		Public Sub New(ByVal vocab As NavigableMap(Of String, Integer), ByVal lowerCaseOnly As Boolean, ByVal stripAccents As Boolean)
			Me.New(vocab, New BertWordPiecePreProcessor(lowerCaseOnly, stripAccents, vocab))
		End Sub

		''' <param name="vocab">                   Vocabulary, as a navigable map </param>
		''' <param name="preTokenizePreProcessor"> The preprocessor that should be used on the raw strings, before splitting </param>
		Public Sub New(ByVal vocab As NavigableMap(Of String, Integer), ByVal preTokenizePreProcessor As TokenPreProcess)
			Me.vocab = vocab
			Me.preTokenizePreProcessor = preTokenizePreProcessor
		End Sub

		''' <summary>
		''' Create a BertWordPieceTokenizerFactory, load the vocabulary from the specified file.<br>
		''' The expected format is a \n seperated list of tokens for vocab entries
		''' </summary>
		''' <param name="pathToVocab">   Path to vocabulary file </param>
		''' <param name="lowerCaseOnly"> If true: tokenization should convert all characters to lower case </param>
		''' <param name="stripAccents">  If true: strip accents off characters. Usually same as lower case. Should be true when using "uncased" official BERT TensorFlow models </param>
		''' <param name="charset">       Character set for the file </param>
		''' <exception cref="IOException"> If an error occurs reading the vocab file </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BertWordPieceTokenizerFactory(File pathToVocab, boolean lowerCaseOnly, boolean stripAccents, @NonNull Charset charset) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Sub New(ByVal pathToVocab As File, ByVal lowerCaseOnly As Boolean, ByVal stripAccents As Boolean, ByVal charset As Charset)
			Me.New(loadVocab(pathToVocab, charset), lowerCaseOnly, stripAccents)
			Me.charset = charset
		End Sub

		''' <summary>
		''' Create a BertWordPieceTokenizerFactory, load the vocabulary from the specified input stream.<br>
		''' The expected format for vocabulary is a \n seperated list of tokens for vocab entries </summary>
		''' <param name="vocabInputStream"> Input stream to load vocabulary </param>
		''' <param name="lowerCaseOnly"> If true: tokenization should convert all characters to lower case </param>
		''' <param name="stripAccents">  If true: strip accents off characters. Usually same as lower case. Should be true when using "uncased" official BERT TensorFlow models </param>
		''' <param name="charset">       Character set for the vocab stream </param>
		''' <exception cref="IOException">  If an error occurs reading the vocab stream </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BertWordPieceTokenizerFactory(InputStream vocabInputStream, boolean lowerCaseOnly, boolean stripAccents, @NonNull Charset charset) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Sub New(ByVal vocabInputStream As Stream, ByVal lowerCaseOnly As Boolean, ByVal stripAccents As Boolean, ByVal charset As Charset)
			Me.New(loadVocab(vocabInputStream, charset), lowerCaseOnly, stripAccents)
			Me.charset = charset
		End Sub

		Public Overridable Function create(ByVal toTokenize As String) As Tokenizer Implements TokenizerFactory.create
			Dim t As Tokenizer = New BertWordPieceTokenizer(toTokenize, vocab, preTokenizePreProcessor, tokenPreProcessor)
			Return t
		End Function

		Public Overridable Function create(ByVal toTokenize As Stream) As Tokenizer
			Dim t As Tokenizer = New BertWordPieceStreamTokenizer(toTokenize, charset, vocab, preTokenizePreProcessor, tokenPreProcessor)
			Return t
		End Function

		Public Overridable ReadOnly Property Vocab As IDictionary(Of String, Integer)
			Get
				Return Collections.unmodifiableMap(vocab)
			End Get
		End Property

		''' <summary>
		''' The expected format is a \n seperated list of tokens for vocab entries
		''' 
		''' <code>
		'''     foo
		'''     bar
		'''     baz
		''' </code>
		''' 
		''' the tokens should <b>not</b> have any whitespace on either of their sides
		''' </summary>
		''' <param name="is"> InputStream </param>
		''' <returns> A vocab map with the popper sort order for fast traversal </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.NavigableMap<String, Integer> loadVocab(InputStream is, java.nio.charset.Charset charset) throws IOException
		Public Shared Function loadVocab(ByVal [is] As Stream, ByVal charset As Charset) As NavigableMap(Of String, Integer)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.TreeMap<String, Integer> map = new java.util.TreeMap<>(java.util.Collections.reverseOrder());
			Dim map As New SortedDictionary(Of String, Integer)(Collections.reverseOrder())

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: try (final BufferedReader reader = new BufferedReader(new InputStreamReader(is, charset)))
			Using reader As New StreamReader([is], charset)
				Dim token As String
				Dim i As Integer = 0
				token = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((token = reader.readLine()) != null)
				Do While token IsNot Nothing
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: map.put(token, i++);
					map(token) = i
						i += 1
						token = reader.ReadLine()
				Loop
			End Using

			Return map
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.NavigableMap<String, Integer> loadVocab(File vocabFile, java.nio.charset.Charset charset) throws IOException
		Public Shared Function loadVocab(ByVal vocabFile As File, ByVal charset As Charset) As NavigableMap(Of String, Integer)
			Return loadVocab(New FileStream(vocabFile, FileMode.Open, FileAccess.Read), charset)
		End Function

	End Class

End Namespace