Imports System
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.spark.models.embeddings.word2vec


	<Serializable>
	Public Class NegativeHolder
		Private Shared ourInstance As New NegativeHolder()

		Public Shared ReadOnly Property Instance As NegativeHolder
			Get
				Return ourInstance
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private volatile org.nd4j.linalg.api.ndarray.INDArray syn1Neg;
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private syn1Neg As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private volatile org.nd4j.linalg.api.ndarray.INDArray table;
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private table As INDArray

		<NonSerialized>
		Private wasInit As New AtomicBoolean(False)
		<NonSerialized>
		Private vocab As VocabCache(Of VocabWord)

		Private Sub New()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void initHolder(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache, double[] expTable, int layerSize)
		Public Overridable Sub initHolder(ByVal vocabCache As VocabCache(Of VocabWord), ByVal expTable() As Double, ByVal layerSize As Integer)
			SyncLock Me
				If Not wasInit.get() Then
					Me.vocab = vocabCache
					Me.syn1Neg = Nd4j.zeros(vocabCache.numWords(), layerSize)
					makeTable(Math.Max(expTable.Length, 100000), 0.75)
					wasInit.set(True)
				End If
			End SyncLock
		End Sub

		Protected Friend Overridable Sub makeTable(ByVal tableSize As Integer, ByVal power As Double)
			Dim vocabSize As Integer = vocab.numWords()
			table = Nd4j.create(DataType.FLOAT, tableSize)
			Dim trainWordsPow As Double = 0.0
			For Each word As String In vocab.words()
				trainWordsPow += Math.Pow(vocab.wordFrequency(word), power)
			Next word

			Dim wordIdx As Integer = 0
			Dim word As String = vocab.wordAtIndex(wordIdx)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim d1 As Double = Math.Pow(vocab.wordFrequency(word), power) / trainWordsPow
			For i As Integer = 0 To tableSize - 1
				table.putScalar(i, wordIdx)
				Dim mul As Double = i * 1.0 / CDbl(tableSize)
				If mul > d1 Then
					If wordIdx < vocabSize - 1 Then
						wordIdx += 1
					End If
					word = vocab.wordAtIndex(wordIdx)
					Dim wordAtIndex As String = vocab.wordAtIndex(wordIdx)
					If word Is Nothing Then
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					d1 += Math.Pow(vocab.wordFrequency(wordAtIndex), power) / trainWordsPow
				End If
			Next i
		End Sub


	End Class

End Namespace