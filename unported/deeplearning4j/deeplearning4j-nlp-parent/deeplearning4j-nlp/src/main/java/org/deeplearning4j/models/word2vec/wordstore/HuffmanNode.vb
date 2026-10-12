Imports Data = lombok.Data
Imports NonNull = lombok.NonNull

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

Namespace org.deeplearning4j.models.word2vec.wordstore

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class HuffmanNode
	Public Class HuffmanNode
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private byte[] code;
		Private code() As SByte

		Private Class SentencePreProcessorAnonymousInnerClass
			Implements org.deeplearning4j.text.sentenceiterator.SentencePreProcessor

			Private ReadOnly outerInstance As Word2VecDataSetIterator

			Public Sub New(ByVal outerInstance As Word2VecDataSetIterator)
				Me.outerInstance = outerInstance
			End Sub

			Public Function preProcess(ByVal sentence As String) As String Implements org.deeplearning4j.text.sentenceiterator.SentencePreProcessor.preProcess
				Dim label As String = Word2VecDataSetIterator.this.iter.currentLabel()
				Dim ret As String = "<" & label & "> " & (New InputHomogenization(sentence)).transform() & " </" & label & ">"
				Return ret
			End Function
		End Class

		Private Class SentencePreProcessorAnonymousInnerClass2
			Implements org.deeplearning4j.text.sentenceiterator.SentencePreProcessor

			Private ReadOnly outerInstance As Word2VecDataSetIterator

			Public Sub New(ByVal outerInstance As Word2VecDataSetIterator)
				Me.outerInstance = outerInstance
			End Sub

			Public Function preProcess(ByVal sentence As String) As String Implements org.deeplearning4j.text.sentenceiterator.SentencePreProcessor.preProcess
				Dim label As String = Word2VecDataSetIterator.this.iter.currentLabel()
				Dim ret As String = "<" & label & ">" & sentence & "</" & label & ">"
				Return ret
			End Function
		End Class

		Private Class SentencePreProcessorAnonymousInnerClass3
			Implements org.deeplearning4j.text.sentenceiterator.SentencePreProcessor

			Private ReadOnly outerInstance As Word2VecDataSetIterator

			Public Sub New(ByVal outerInstance As Word2VecDataSetIterator)
				Me.outerInstance = outerInstance
			End Sub

			Public Function preProcess(ByVal sentence As String) As String Implements org.deeplearning4j.text.sentenceiterator.SentencePreProcessor.preProcess
				Dim ret As String = (New InputHomogenization(sentence)).transform()
				Return ret
			End Function
		End Class
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private int[] point;
		Private point() As Integer
		Private idx As Integer
		Private length As SByte

		Public Sub New()

		End Sub

		Public Sub New(ByVal code() As SByte, ByVal point() As Integer, ByVal index As Integer, ByVal length As SByte)
			Me.code = code
			Me.point = point
			Me.idx = index
			Me.length = length
		End Sub
	End Class

End Namespace