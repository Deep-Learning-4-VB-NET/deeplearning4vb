Imports System
Imports System.Collections.Generic
Imports System.Text
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.collection
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

Namespace org.deeplearning4j.text.movingwindow


	Public Class ContextLabelRetriever


		Private Shared BEGIN_LABEL As String = "<([A-Za-z]+|\d+)>"
		Private Shared END_LABEL As String = "</([A-Za-z]+|\d+)>"


		Private Sub New()
		End Sub


		''' <summary>
		''' Returns a stripped sentence with the indices of words
		''' with certain kinds of labels. </summary>
		''' <param name="sentence"> the sentence to process </param>
		''' <returns> a pair of a post processed sentence
		''' with labels stripped and the spans of
		''' the labels </returns>
		Public Shared Function stringWithLabels(ByVal sentence As String, ByVal tokenizerFactory As TokenizerFactory) As Pair(Of String, MultiDimensionalMap(Of Integer, Integer, String))
			Dim map As MultiDimensionalMap(Of Integer, Integer, String) = MultiDimensionalMap.newHashBackedMap()
			Dim t As Tokenizer = tokenizerFactory.create(sentence)
			Dim currTokens As IList(Of String) = New List(Of String)()
			Dim currLabel As String = Nothing
			Dim endLabel As String = Nothing
			Dim tokensWithSameLabel As IList(Of Pair(Of String, IList(Of String))) = New List(Of Pair(Of String, IList(Of String)))()
			Do While t.hasMoreTokens()
				Dim token As String = t.nextToken()
				If token.matches(BEGIN_LABEL) Then
					If endLabel IsNot Nothing Then
						Throw New System.InvalidOperationException("Tried parsing sentence; found an end label when the begin label has not been cleared")
					End If
					currLabel = token

					'no labels; add these as NONE and begin the new label
					If currTokens.Count > 0 Then
						tokensWithSameLabel.Add(New Pair(Of String, IList(Of String))("NONE", CType(New List(Of String, IList(Of String))(currTokens), IList(Of String))))
						currTokens.Clear()

					End If

				ElseIf token.matches(END_LABEL) Then
					If currLabel Is Nothing Then
						Throw New System.InvalidOperationException("Found an ending label with no matching begin label")
					End If
					endLabel = token
				Else
					currTokens.Add(token)
				End If

				If currLabel IsNot Nothing AndAlso endLabel IsNot Nothing Then
					currLabel = currLabel.replaceAll("[<>/]", "")
					endLabel = endLabel.replaceAll("[<>/]", "")
					Preconditions.checkState(currLabel.Length > 0, "Current label is empty!")
					Preconditions.checkState(endLabel.Length > 0, "End label is empty!")
					Preconditions.checkState(currLabel.Equals(endLabel), "Current label begin and end did not match for the parse. Was: %s ending with %s", currLabel, endLabel)

					tokensWithSameLabel.Add(New Pair(Of String, IList(Of String))(currLabel, CType(New List(Of String, IList(Of String))(currTokens), IList(Of String))))
					currTokens.Clear()

					'clear out the tokens
					currLabel = Nothing
					endLabel = Nothing
				End If
			Loop

			'no labels; add these as NONE and begin the new label
			If currTokens.Count > 0 Then
				tokensWithSameLabel.Add(New Pair(Of String, IList(Of String))("none", CType(New List(Of String, IList(Of String))(currTokens), IList(Of String))))
				currTokens.Clear()

			End If

			'now join the output
			Dim strippedSentence As New StringBuilder()
			For Each tokensWithLabel As Pair(Of String, IList(Of String)) In tokensWithSameLabel
				Dim joinedSentence As String = StringUtils.join(tokensWithLabel.Second, " ")
				'spaces between separate parts of the sentence
				If Not (strippedSentence.Length < 1) Then
					strippedSentence.Append(" ")
				End If
				strippedSentence.Append(joinedSentence)
				Dim begin As Integer = strippedSentence.ToString().IndexOf(joinedSentence, StringComparison.Ordinal)
				Dim [end] As Integer = begin + joinedSentence.Length
				map.put(begin, [end], tokensWithLabel.First)
			Next tokensWithLabel

			Return New Pair(Of String, MultiDimensionalMap(Of Integer, Integer, String))(strippedSentence.ToString(), map)
		End Function
	End Class

End Namespace