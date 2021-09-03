Imports System
Imports System.Collections.Generic
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.iterator.bert


	Public Class BertMaskedLMMasker
		Implements BertSequenceMasker

		Public Const DEFAULT_MASK_PROB As Double = 0.15
		Public Const DEFAULT_MASK_TOKEN_PROB As Double = 0.8
		Public Const DEFAULT_RANDOM_WORD_PROB As Double = 0.1

		Protected Friend ReadOnly r As Random
		Protected Friend ReadOnly maskProb As Double
		Protected Friend ReadOnly maskTokenProb As Double
		Protected Friend ReadOnly randomTokenProb As Double

		''' <summary>
		''' Create a BertMaskedLMMasker with all default probabilities
		''' </summary>
		Public Sub New()
			Me.New(New Random(), DEFAULT_MASK_PROB, DEFAULT_MASK_TOKEN_PROB, DEFAULT_RANDOM_WORD_PROB)
		End Sub

		''' <summary>
		''' See: <seealso cref="BertMaskedLMMasker"/> for details. </summary>
		''' <param name="r">                 Random number generator </param>
		''' <param name="maskProb">          Probability of masking each token </param>
		''' <param name="maskTokenProb">     Probability of replacing a selected token with the mask token </param>
		''' <param name="randomTokenProb">    Probability of replacing a selected token with a random token </param>
		Public Sub New(ByVal r As Random, ByVal maskProb As Double, ByVal maskTokenProb As Double, ByVal randomTokenProb As Double)
			Preconditions.checkArgument(maskProb > 0 AndAlso maskProb < 1, "Probability must be beteen 0 and 1, got %s", maskProb)
			Preconditions.checkState(maskTokenProb >=0 AndAlso maskTokenProb <= 1.0, "Mask token probability must be between 0 and 1, got %s", maskTokenProb)
			Preconditions.checkState(randomTokenProb >=0 AndAlso randomTokenProb <= 1.0, "Random token probability must be between 0 and 1, got %s", randomTokenProb)
			Preconditions.checkState(maskTokenProb + randomTokenProb <= 1.0, "Sum of maskTokenProb (%s) and randomTokenProb (%s) must be <= 1.0, got sum is %s", maskTokenProb, randomTokenProb, (maskTokenProb + randomTokenProb))
			Me.r = r
			Me.maskProb = maskProb
			Me.maskTokenProb = maskTokenProb
			Me.randomTokenProb = randomTokenProb
		End Sub

		Public Overridable Function maskSequence(ByVal input As IList(Of String), ByVal maskToken As String, ByVal vocabWords As IList(Of String)) As Pair(Of IList(Of String), Boolean()) Implements BertSequenceMasker.maskSequence
			Dim [out] As IList(Of String) = New List(Of String)(input.Count)
			Dim masked(input.Count - 1) As Boolean
			For i As Integer = 0 To input.Count - 1
				If r.NextDouble() < maskProb Then
					'Mask
					Dim d As Double = r.NextDouble()
					If d < maskTokenProb Then
						[out].Add(maskToken)
					ElseIf d < maskTokenProb + randomTokenProb Then
						'Randomly select a token...
						Dim random As String = vocabWords(r.Next(vocabWords.Count))
						[out].Add(random)
					Else
						'Keep existing token
						[out].Add(input(i))
					End If
					masked(i) = True
				Else
					'No change, keep existing
					[out].Add(input(i))
				End If
			Next i
			Return New Pair(Of IList(Of String), Boolean())([out], masked)
		End Function
	End Class

End Namespace