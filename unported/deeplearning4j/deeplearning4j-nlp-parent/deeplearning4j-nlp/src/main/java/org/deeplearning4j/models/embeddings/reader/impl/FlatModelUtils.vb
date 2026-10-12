Imports System.Collections.Generic
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.models.embeddings.reader.impl


	Public Class FlatModelUtils(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Inherits BasicModelUtils(Of T)

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(FlatModelUtils))

		Public Sub New()

		End Sub

		''' <summary>
		''' This method does full scan against whole vocabulary, building descending list of similar words </summary>
		''' <param name="label"> </param>
		''' <param name="n">
		''' @return </param>
		Public Overrides Function wordsNearest(ByVal label As String, ByVal n As Integer) As ICollection(Of String)
			Dim collection As ICollection(Of String) = wordsNearest(lookupTable.vector(label), n)
			If collection.Contains(label) Then
				collection.remove(label)
			End If
			Return collection
		End Function

		''' <summary>
		''' This method does full scan against whole vocabulary, building descending list of similar words
		''' </summary>
		''' <param name="words"> </param>
		''' <param name="top"> </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overrides Function wordsNearest(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)
			Dim distances As New Counter(Of String)()

			words = adjustRank(words)

			For Each s As String In vocabCache.words()
				Dim otherVec As INDArray = lookupTable.vector(s)
				Dim sim As Double = Transforms.cosineSim(Transforms.unitVec(words.dup()), Transforms.unitVec(otherVec.dup()))
				distances.incrementCount(s, CSng(sim))
			Next s

			distances.keepTopNElements(top)
			Return distances.keySetSorted()
		End Function
	End Class

End Namespace