Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement

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

Namespace org.deeplearning4j.models.embeddings.learning.impl.elements


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BatchSequences<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement>
	Public Class BatchSequences(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)

		Private batches As Integer

		Friend buffer As IList(Of BatchItem(Of T)) = New List(Of BatchItem(Of T))()

		Public Sub New(ByVal batches As Integer)
			Me.batches = batches
		End Sub

		Public Overridable Sub put(ByVal word As T, ByVal lastWord As T, ByVal randomValue As Long, ByVal alpha As Double)
			Dim newItem As New BatchItem(Of T)(word, lastWord, randomValue, alpha)
			buffer.Add(newItem)
		End Sub

		Public Overridable Sub put(ByVal word As T, ByVal windowWords() As Integer, ByVal wordStatuses() As Boolean, ByVal randomValue As Long, ByVal alpha As Double)
			Dim newItem As New BatchItem(Of T)(word, windowWords, wordStatuses, randomValue, alpha)
			buffer.Add(newItem)
		End Sub

		Public Overridable Sub put(ByVal word As T, ByVal windowWords() As Integer, ByVal wordStatuses() As Boolean, ByVal randomValue As Long, ByVal alpha As Double, ByVal numLabels As Integer)
			Dim newItem As New BatchItem(Of T)(word, windowWords, wordStatuses, randomValue, alpha, numLabels)
			buffer.Add(newItem)
		End Sub

		Public Overridable Function get(ByVal chunkNo As Integer) As IList(Of BatchItem(Of T))
			Dim retVal As IList(Of BatchItem(Of T)) = New List(Of BatchItem(Of T))()

			Dim i As Integer = 0 + chunkNo * batches
			Do While (i < batches + chunkNo * batches) AndAlso (i < buffer.Count)
				Dim value As BatchItem(Of T) = buffer(i)
				retVal.Add(value)
				i += 1
			Loop
			Return retVal
		End Function

		Public Overridable Function size() As Integer
			Return buffer.Count
		End Function

		Public Overridable Sub clear()
			buffer.Clear()
		End Sub


	End Class

End Namespace