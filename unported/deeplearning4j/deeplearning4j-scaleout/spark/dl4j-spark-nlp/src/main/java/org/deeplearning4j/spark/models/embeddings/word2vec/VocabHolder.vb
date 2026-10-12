Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
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
	Public Class VocabHolder
		Private Shared ourInstance As New VocabHolder()

		Private indexSyn0VecMap As IDictionary(Of VocabWord, INDArray) = New ConcurrentDictionary(Of VocabWord, INDArray)()
		Private pointSyn1VecMap As IDictionary(Of Integer, INDArray) = New ConcurrentDictionary(Of Integer, INDArray)()
		Private workers As HashSet(Of Long) = New LinkedHashSet(Of Long)()

		Private seed As New AtomicLong(0)
		Private vectorLength As New AtomicInteger(0)

		Public Shared ReadOnly Property Instance As VocabHolder
			Get
				Return ourInstance
			End Get
		End Property

		Private Sub New()
		End Sub

		Public Overridable Sub setSeed(ByVal seed As Long, ByVal vectorLength As Integer)
			Me.seed.set(seed)
			Me.vectorLength.set(vectorLength)
		End Sub

		Public Overridable Function getSyn0Vector(ByVal wordIndex As Integer?, ByVal vocabCache As VocabCache(Of VocabWord)) As INDArray
			If Not workers.Contains(Thread.CurrentThread.getId()) Then
				workers.Add(Thread.CurrentThread.getId())
			End If

			Dim word As VocabWord = vocabCache.elementAtIndex(wordIndex)

			If Not indexSyn0VecMap.ContainsKey(word) Then
				SyncLock Me
					If Not indexSyn0VecMap.ContainsKey(word) Then
						indexSyn0VecMap(word) = getRandomSyn0Vec(vectorLength.get(), wordIndex)
					End If
				End SyncLock
			End If

			Return indexSyn0VecMap(word)
		End Function

		Public Overridable Function getSyn1Vector(ByVal point As Integer?) As INDArray

			If Not pointSyn1VecMap.ContainsKey(point) Then
				SyncLock Me
					If Not pointSyn1VecMap.ContainsKey(point) Then
						pointSyn1VecMap(point) = Nd4j.zeros(1, vectorLength.get())
					End If
				End SyncLock
			End If

			Return pointSyn1VecMap(point)
		End Function

		Private Function getRandomSyn0Vec(ByVal vectorLength As Integer, ByVal lseed As Long) As INDArray
	'        
	'            we use wordIndex as part of seed here, to guarantee that during word syn0 initialization on dwo distinct nodes, initial weights will be the same for the same word
	'         
			Return Nd4j.rand(New Integer() {1, vectorLength}, lseed * seed.get()).subi(0.5).divi(vectorLength)
		End Function

		Public Overridable Function getSplit(ByVal vocabCache As VocabCache(Of VocabWord)) As IEnumerable(Of KeyValuePair(Of VocabWord, INDArray))
			Dim set As ISet(Of KeyValuePair(Of VocabWord, INDArray)) = New HashSet(Of KeyValuePair(Of VocabWord, INDArray))()
			Dim cnt As Integer = 0
			For Each entry As KeyValuePair(Of VocabWord, INDArray) In indexSyn0VecMap.SetOfKeyValuePairs()
				set.Add(entry)
				cnt += 1
				If cnt > 10 Then
					Exit For
				End If
			Next entry

			Console.WriteLine("Returning set: " & set.Count)

			Return set
		End Function
	End Class

End Namespace