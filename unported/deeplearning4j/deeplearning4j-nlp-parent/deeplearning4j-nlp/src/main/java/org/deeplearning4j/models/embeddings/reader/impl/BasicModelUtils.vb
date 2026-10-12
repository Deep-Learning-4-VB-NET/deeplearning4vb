Imports System
Imports System.Collections.Generic
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.reader
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports MathUtils = org.nd4j.common.util.MathUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports SetUtils = org.nd4j.common.util.SetUtils

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

Namespace org.deeplearning4j.models.embeddings.reader.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BasicModelUtils<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.deeplearning4j.models.embeddings.reader.ModelUtils<T>
	Public Class BasicModelUtils(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements ModelUtils(Of T)

		Public Const EXISTS As String = "exists"
		Public Const CORRECT As String = "correct"
		Public Const WRONG As String = "wrong"
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile org.deeplearning4j.models.word2vec.wordstore.VocabCache<T> vocabCache;
		Protected Friend vocabCache As VocabCache(Of T)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile org.deeplearning4j.models.embeddings.WeightLookupTable<T> lookupTable;
		Protected Friend lookupTable As WeightLookupTable(Of T)

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile boolean normalized = false;
		Protected Friend normalized As Boolean = False


		Public Sub New()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull WeightLookupTable<T> lookupTable)
		Public Overridable Sub init(ByVal lookupTable As WeightLookupTable(Of T))
			Me.vocabCache = lookupTable.getVocabCache()
			Me.lookupTable = lookupTable

			' reset normalization trigger on init call
			Me.normalized = False
		End Sub

		''' <summary>
		''' Returns the similarity of 2 words. Result value will be in range [-1,1], where -1.0 is exact opposite similarity, i.e. NO similarity, and 1.0 is total match of two word vectors.
		''' However, most of time you'll see values in range [0,1], but that's something depends of training corpus.
		''' 
		''' Returns NaN if any of labels not exists in vocab, or any label is null
		''' </summary>
		''' <param name="label1"> the first word </param>
		''' <param name="label2"> the second word </param>
		''' <returns> a normalized similarity (cosine similarity) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public double similarity(@NonNull String label1, @NonNull String label2)
		Public Overridable Function similarity(ByVal label1 As String, ByVal label2 As String) As Double Implements ModelUtils(Of T).similarity
			If label1 Is Nothing OrElse label2 Is Nothing Then
				log.debug("LABELS: " & label1 & ": " & (If(label1 Is Nothing, "null", EXISTS)) & ";" & label2 & " vec2:" & (If(label2 Is Nothing, "null", EXISTS)))
				Return Double.NaN
			End If

			If Not vocabCache.hasToken(label1) Then
				log.debug("Unknown token 1 requested: [{}]", label1)
				Return Double.NaN
			End If

			If Not vocabCache.hasToken(label2) Then
				log.debug("Unknown token 2 requested: [{}]", label2)
				Return Double.NaN
			End If

			Dim vec1 As INDArray = lookupTable.vector(label1).dup()
			Dim vec2 As INDArray = lookupTable.vector(label2).dup()


			If vec1 Is Nothing OrElse vec2 Is Nothing Then
				log.debug(label1 & ": " & (If(vec1 Is Nothing, "null", EXISTS)) & ";" & label2 & " vec2:" & (If(vec2 Is Nothing, "null", EXISTS)))
				Return Double.NaN
			End If

			If label1.Equals(label2) Then
				Return 1.0
			End If

			Return Transforms.cosineSim(vec1, vec2)
		End Function


		Public Overridable Function wordsNearest(ByVal label As String, ByVal n As Integer) As ICollection(Of String)
			Dim collection As IList(Of String) = New List(Of String)(wordsNearest(java.util.Arrays.asList(label), New List(Of String)(), n + 1))
			If collection.Contains(label) Then
				collection.Remove(label)
			End If

			Do While collection.Count > n
				collection.RemoveAt(collection.Count - 1)
			Loop

			Return collection
		End Function

		''' <summary>
		''' Accuracy based on questions which are a space separated list of strings
		''' where the first word is the query word, the next 2 words are negative,
		''' and the last word is the predicted word to be nearest </summary>
		''' <param name="questions"> the questions to ask </param>
		''' <returns> the accuracy based on these questions </returns>
		Public Overridable Function accuracy(ByVal questions As IList(Of String)) As IDictionary(Of String, Double)
'JAVA TO VB CONVERTER NOTE: The local variable accuracy was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim accuracy_Conflict As IDictionary(Of String, Double) = New Dictionary(Of String, Double)()
			Dim right As New Counter(Of String)()
			Dim analogyType As String = ""
			For Each s As String In questions
				If s.StartsWith(":", StringComparison.Ordinal) Then
'JAVA TO VB CONVERTER NOTE: The variable correct was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim correct_Conflict As Double = right.getCount(CORRECT)
'JAVA TO VB CONVERTER NOTE: The variable wrong was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim wrong_Conflict As Double = right.getCount(WRONG)
					If analogyType.Length = 0 Then
						analogyType = s
						Continue For
					End If
					Dim accuracyRet As Double = 100.0 * correct_Conflict / (correct_Conflict + wrong_Conflict)
					Me.accuracy(analogyType) = accuracyRet
					analogyType = s
					right.clear()
				Else
					Dim split() As String = s.Split(" ", True)
					Dim positive As IList(Of String) = New List(Of String) From {split(1), split(2)}
					Dim negative As IList(Of String) = New List(Of String) From {split(0)}
					Dim predicted As String = split(3)
					Dim w As String = wordsNearest(positive, negative, 1).GetEnumerator().next()
					If predicted.Equals(w) Then
						right.incrementCount(CORRECT, 1.0f)
					Else
						right.incrementCount(WRONG, 1.0f)
					End If

				End If
			Next s
			If analogyType.Length > 0 Then
'JAVA TO VB CONVERTER NOTE: The variable correct was renamed since Visual Basic does not handle local variables named the same as class members well:
				Dim correct_Conflict As Double = right.getCount(CORRECT)
'JAVA TO VB CONVERTER NOTE: The variable wrong was renamed since Visual Basic does not handle local variables named the same as class members well:
				Dim wrong_Conflict As Double = right.getCount(WRONG)
				Dim accuracyRet As Double = 100.0 * correct_Conflict / (correct_Conflict + wrong_Conflict)
				Me.accuracy(analogyType) = accuracyRet
			End If
			Return accuracy_Conflict
		End Function

		''' <summary>
		''' Find all words with a similar characters
		''' in the vocab </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="accuracy"> the accuracy: 0 to 1 </param>
		''' <returns> the list of words that are similar in the vocab </returns>
		Public Overridable Function similarWordsInVocabTo(ByVal word As String, ByVal accuracy As Double) As IList(Of String)
			Dim ret As IList(Of String) = New List(Of String)()
			For Each s As String In vocabCache.words()
				If MathUtils.stringSimilarity(word, s) >= accuracy Then
					ret.Add(s)
				End If
			Next s
			Return ret
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Collection<String> wordsNearest(@NonNull Collection<String> positive, @NonNull Collection<String> negative, int top)
		Public Overridable Function wordsNearest(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)
			' Check every word is in the model
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (String p : org.nd4j.common.util.SetUtils.union(new HashSet<>(positive), new HashSet<>(negative)))
			For Each p As String In SetUtils.union(New HashSet(Of T)(positive), New HashSet(Of T)(negative))
				If Not vocabCache.containsWord(p) Then
					Return New List(Of String)()
				End If
			Next p

			Dim words As INDArray = Nd4j.create(positive.Count + negative.Count, lookupTable.layerSize())
			Dim row As Integer = 0
			'Set<String> union = SetUtils.union(new HashSet<>(positive), new HashSet<>(negative));
			For Each s As String In positive
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: words.putRow(row++, lookupTable.vector(s));
				words.putRow(row, lookupTable.vector(s))
					row += 1
			Next s

			For Each s As String In negative
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: words.putRow(row++, lookupTable.vector(s).mul(-1));
				words.putRow(row, lookupTable.vector(s).mul(-1))
					row += 1
			Next s

			Dim mean As INDArray = If(words.Matrix, words.mean(0).reshape(ChrW(1), words.size(1)), words)
			Dim tempRes As ICollection(Of String) = wordsNearest(mean, top + positive.Count + negative.Count)
			Dim realResults As IList(Of String) = New List(Of String)()

			For Each word As String In tempRes
				If Not positive.Contains(word) AndAlso Not negative.Contains(word) AndAlso realResults.Count < top Then
					realResults.Add(word)
				End If
			Next word

			Return realResults
		End Function

		''' <summary>
		''' Get the top n words most similar to the given word </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="n"> the n to get </param>
		''' <returns> the top n words </returns>
		Public Overridable Function wordsNearestSum(ByVal word As String, ByVal n As Integer) As ICollection(Of String)
			'INDArray vec = Transforms.unitVec(this.lookupTable.vector(word));
			Dim vec As INDArray = Me.lookupTable.vector(word)
			Return wordsNearestSum(vec, n)
		End Function

		Protected Friend Overridable Function adjustRank(ByVal words As INDArray) As INDArray
			If TypeOf lookupTable Is InMemoryLookupTable Then
				Dim l As InMemoryLookupTable = CType(lookupTable, InMemoryLookupTable)

				Dim syn0 As INDArray = l.getSyn0()
				If Not words.dataType().Equals(syn0.dataType()) Then
					Return words.castTo(syn0.dataType())
				End If
				If words.rank() = 0 OrElse words.rank() > 2 Then
					Throw New System.InvalidOperationException("Invalid rank for wordsNearest method")
				ElseIf words.rank() = 1 Then
					Return words.reshape(ChrW(1), -1)
				End If
			End If
			Return words
		End Function
		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' * <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearest(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)
			words = adjustRank(words)

			If TypeOf lookupTable Is InMemoryLookupTable Then
				Dim l As InMemoryLookupTable = CType(lookupTable, InMemoryLookupTable)

				Dim syn0 As INDArray = l.getSyn0()

				If Not normalized Then
					SyncLock Me
						If Not normalized Then
							syn0.diviColumnVector(syn0.norm2(1))
							normalized = True
						End If
					End SyncLock
				End If

				Dim similarity As INDArray = Transforms.unitVec(words).mmul(syn0.transpose())

				Dim highToLowSimList As IList(Of Double) = getTopN(similarity, top + 20)

				Dim result As IList(Of WordSimilarity) = New List(Of WordSimilarity)()

				For i As Integer = 0 To highToLowSimList.Count - 1
					Dim word As String = vocabCache.wordAtIndex(highToLowSimList(i).intValue())
					If word IsNot Nothing AndAlso Not word.Equals("UNK") AndAlso Not word.Equals("STOP") Then
						Dim otherVec As INDArray = lookupTable.vector(word)
						Dim sim As Double = Transforms.cosineSim(words, otherVec)

						result.Add(New WordSimilarity(word, sim))
					End If
				Next i

				result.Sort(New SimilarityComparator())

				Return getLabels(result, top)
			End If

			Dim distances As New Counter(Of String)()

			For Each s As String In vocabCache.words()
				Dim otherVec As INDArray = lookupTable.vector(s)
				Dim sim As Double = Transforms.cosineSim(words, otherVec)
				distances.incrementCount(s, CSng(sim))
			Next s


			distances.keepTopNElements(top)
			Return distances.keySet()


		End Function

		''' <summary>
		''' Get top N elements
		''' </summary>
		''' <param name="vec"> the vec to extract the top elements from </param>
		''' <param name="N"> the number of elements to extract </param>
		''' <returns> the indices and the sorted top N elements </returns>
		Private Function getTopN(ByVal vec As INDArray, ByVal N As Integer) As IList(Of Double)
			Dim comparator As New ArrayComparator()
			Dim queue As New PriorityQueue(Of Double())(vec.rows(), comparator)

			For j As Integer = 0 To vec.length() - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final System.Nullable<Double>[] pair = new System.Nullable<Double>[] {vec.getDouble(j), (double) j};
				Dim pair() As Double? = {vec.getDouble(j), CDbl(j)}
				If queue.size() < N Then
					queue.add(pair)
				Else
					Dim head() As Double? = queue.peek()
					If comparator.Compare(pair, head) > 0 Then
						queue.poll()
						queue.add(pair)
					End If
				End If
			Next j

			Dim lowToHighSimLst As IList(Of Double) = New List(Of Double)()

			Do While Not queue.isEmpty()
				Dim ind As Double = queue.poll()(1)
				lowToHighSimLst.Add(ind)
			Loop
			Return Lists.reverse(lowToHighSimLst)
		End Function

		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' * <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearestSum(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)

			If TypeOf lookupTable Is InMemoryLookupTable Then
				Dim l As InMemoryLookupTable = CType(lookupTable, InMemoryLookupTable)
				Dim syn0 As INDArray = l.getSyn0()
				Dim temp As INDArray = syn0.norm2(0).rdivi(1).reshape(words.shape())
				Dim weights As INDArray = temp.muli(words)
				Dim distances As INDArray = syn0.mulRowVector(weights).sum(1)
				Dim sorted() As INDArray = Nd4j.sortWithIndices(distances, 0, False)
				Dim sort As INDArray = sorted(0)
				Dim ret As IList(Of String) = New List(Of String)()

				If top > sort.length() Then
					top = CInt(sort.length())
				End If
				'there will be a redundant word
				Dim [end] As Integer = top
				For i As Integer = 0 To [end] - 1
					Dim add As String = vocabCache.wordAtIndex(sort.getInt(i))
					If add Is Nothing OrElse add.Equals("UNK") OrElse add.Equals("STOP") Then
						[end] += 1
						If [end] >= sort.length() Then
							Exit For
						End If
						Continue For
					End If
					ret.Add(vocabCache.wordAtIndex(sort.getInt(i)))
				Next i
				Return ret
			End If

			Dim distances As New Counter(Of String)()

			For Each s As String In vocabCache.words()
				Dim otherVec As INDArray = lookupTable.vector(s)
				Dim sim As Double = Transforms.cosineSim(words, otherVec)
				distances.incrementCount(s, CSng(sim))
			Next s

			distances.keepTopNElements(top)
			Return distances.keySet()
		End Function

		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' <param name="positive"> the positive words </param>
		''' <param name="negative"> the negative words </param>
		''' <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearestSum(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)
			Dim words As INDArray = Nd4j.create(lookupTable.layerSize())
			'    Set<String> union = SetUtils.union(new HashSet<>(positive), new HashSet<>(negative));
			For Each s As String In positive
				words.addi(lookupTable.vector(s))
			Next s


			For Each s As String In negative
				words.addi(lookupTable.vector(s).mul(-1))
			Next s

			Return wordsNearestSum(words, top)
		End Function


		Public Class SimilarityComparator
			Implements IComparer(Of WordSimilarity)

			Public Overridable Function Compare(ByVal o1 As WordSimilarity, ByVal o2 As WordSimilarity) As Integer Implements IComparer(Of WordSimilarity).Compare
				If Double.IsNaN(o1.getSimilarity()) AndAlso Double.IsNaN(o2.getSimilarity()) Then
					Return 0
				ElseIf Double.IsNaN(o1.getSimilarity()) AndAlso Not Double.IsNaN(o2.getSimilarity()) Then
					Return -1
				ElseIf Not Double.IsNaN(o1.getSimilarity()) AndAlso Double.IsNaN(o2.getSimilarity()) Then
					Return 1
				End If
				Return o2.getSimilarity().CompareTo(o1.getSimilarity())
			End Function
		End Class

		Public Class ArrayComparator
			Implements IComparer(Of Double())

			Public Overridable Function Compare(ByVal o1() As Double?, ByVal o2() As Double?) As Integer Implements IComparer(Of Double()).Compare
				If Double.IsNaN(o1(0)) AndAlso Double.IsNaN(o2(0)) Then
					Return 0
				ElseIf Double.IsNaN(o1(0)) AndAlso Not Double.IsNaN(o2(0)) Then
					Return -1
				ElseIf Not Double.IsNaN(o1(0)) AndAlso Double.IsNaN(o2(0)) Then
					Return 1
				End If
				Return o1(0).CompareTo(o2(0))
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor public static class WordSimilarity
		Public Class WordSimilarity
			Friend word As String
			Friend similarity As Double
		End Class

		Public Shared Function getLabels(ByVal results As IList(Of WordSimilarity), ByVal limit As Integer) As IList(Of String)
			Dim result As IList(Of String) = New List(Of String)()
			For x As Integer = 0 To results.Count - 1
				result.Add(results(x).getWord())
				If result.Count >= limit Then
					Exit For
				End If
			Next x

			Return result
		End Function
	End Class

End Namespace