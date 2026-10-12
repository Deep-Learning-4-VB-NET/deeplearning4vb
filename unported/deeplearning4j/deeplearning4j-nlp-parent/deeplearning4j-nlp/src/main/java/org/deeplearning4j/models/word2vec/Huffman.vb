Imports System
Imports System.Collections.Generic
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.models.word2vec



	''' <summary>
	''' Huffman tree builder
	''' @author Adam Gibson
	''' 
	''' </summary>
	Public Class Huffman

		Public ReadOnly MAX_CODE_LENGTH As Integer
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile boolean buildTrigger = false;
		Private buildTrigger As Boolean = False

		Private logger As Logger = LoggerFactory.getLogger(GetType(Huffman))

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public Huffman(Collection<? extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> words)
		Public Sub New(ByVal words As ICollection(Of SequenceElement))
			Me.New(words, 40)
		End Sub

		''' <summary>
		''' Builds Huffman tree for collection of SequenceElements, with defined CODE_LENGTH
		''' Default CODE_LENGTH is 40
		''' </summary>
		''' <param name="words"> </param>
		''' <param name="CODE_LENGTH"> CODE_LENGTH defines maximum length of code path, and effectively limits vocabulary size. </param>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public Huffman(Collection<? extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> words, int CODE_LENGTH)
		Public Sub New(ByVal words As ICollection(Of SequenceElement), ByVal CODE_LENGTH As Integer)
			Me.MAX_CODE_LENGTH = CODE_LENGTH
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: this.words = new ArrayList<>(words);
			Me.words = New List(Of SequenceElement)(words)
			Me.words.Sort(New ComparatorAnonymousInnerClass(Me))
		End Sub

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of SequenceElement)

			Private ReadOnly outerInstance As Huffman

			Public Sub New(ByVal outerInstance As Huffman)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As SequenceElement, ByVal o2 As SequenceElement) As Integer Implements IComparer(Of SequenceElement).Compare
				Return o2.getElementFrequency().CompareTo(o1.getElementFrequency())
			End Function

		End Class

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private List<? extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> words;
		Private words As IList(Of SequenceElement)

		Public Overridable Sub build()
			buildTrigger = True
			Dim count(words.Count * 2) As Long
			Dim binary(words.Count * 2) As SByte
			Dim code(MAX_CODE_LENGTH - 1) As SByte
			Dim point(MAX_CODE_LENGTH - 1) As Integer
			Dim parentNode(words.Count * 2) As Integer
			Dim a As Integer = 0

			Do While a < words.Count
				count(a) = CLng(Math.Truncate(words(a).getElementFrequency()))
				a += 1
			Loop

			a = words.Count

			Do While a < words.Count * 2
				count(a) = Integer.MaxValue
				a += 1
			Loop

			Dim pos1 As Integer = words.Count - 1
			Dim pos2 As Integer = words.Count

			Dim min1i As Integer
			Dim min2i As Integer

			a = 0
			' Following algorithm constructs the Huffman tree by adding one node at a time
			For a = 0 To words.Count - 2
				' First, find two smallest nodes 'min1, min2'
				If pos1 >= 0 Then
					If count(pos1) < count(pos2) Then
						min1i = pos1
						pos1 -= 1
					Else
						min1i = pos2
						pos2 += 1
					End If
				Else
					min1i = pos2
					pos2 += 1
				End If
				If pos1 >= 0 Then
					If count(pos1) < count(pos2) Then
						min2i = pos1
						pos1 -= 1
					Else
						min2i = pos2
						pos2 += 1
					End If
				Else
					min2i = pos2
					pos2 += 1
				End If

				count(words.Count + a) = count(min1i) + count(min2i)
				parentNode(min1i) = words.Count + a
				parentNode(min2i) = words.Count + a
				binary(min2i) = 1
			Next a
			' Now assign binary code to each vocabulary word
			Dim i As Integer
			Dim b As Integer
			' Now assign binary code to each vocabulary word
			a = 0
			Do While a < words.Count
				b = a
				i = 0
				Do
					code(i) = binary(b)
					point(i) = b
					i += 1
					b = parentNode(b)

				Loop While b <> words.Count * 2 - 2 AndAlso i < 39


				words(a).setCodeLength(CShort(i))
				words(a).getPoints().add(words.Count - 2)

				For b = 0 To i - 1
					Try
						words(a).getCodes().set(i - b - 1, code(b))
						words(a).getPoints().set(i - b, point(b) - words.Count)
					Catch e As Exception
						logger.info("Words size: [" & words.Count & "], a: [" & a & "], b: [" & b & "], i: [" & i & "], points size: [" & words(a).getPoints().size() & "]")
						Throw New Exception(e)
					End Try
				Next b

				a += 1
			Loop


		End Sub

		''' <summary>
		''' This method updates VocabCache and all it's elements with Huffman indexes
		''' Please note: it should be the same VocabCache as was used for Huffman tree initialization
		''' </summary>
		''' <param name="cache"> VocabCache to be updated. </param>
		Public Overridable Sub applyIndexes(Of T1 As SequenceElement)(ByVal cache As VocabCache(Of T1))
			If Not buildTrigger Then
				build()
			End If

			Dim a As Integer = 0
			Do While a < words.Count
				If words(a).getLabel() IsNot Nothing Then
					cache.addWordToIndex(a, words(a).getLabel())
				Else
					cache.addWordToIndex(a, words(a).getStorageId())
				End If

				words(a).setIndex(a)
				a += 1
			Loop
		End Sub
	End Class

End Namespace