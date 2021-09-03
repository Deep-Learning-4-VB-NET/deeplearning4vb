Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils

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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BertWordPieceStreamTokenizer extends BertWordPieceTokenizer
	Public Class BertWordPieceStreamTokenizer
		Inherits BertWordPieceTokenizer


		Public Sub New(ByVal tokens As Stream, ByVal encoding As Charset, ByVal vocab As NavigableMap(Of String, Integer), ByVal preTokenizePreProcessor As TokenPreProcess, ByVal tokenPreProcess As TokenPreProcess)
			MyBase.New(readAndClose(tokens, encoding), vocab, preTokenizePreProcessor, tokenPreProcess)
		End Sub


		Public Shared Function readAndClose(ByVal [is] As Stream, ByVal encoding As Charset) As String
			Try
				Return IOUtils.toString([is], encoding)
			Catch e As IOException
				Throw New Exception("Error reading from stream", e)
			Finally
				IOUtils.closeQuietly([is])
			End Try
		End Function
	End Class

End Namespace