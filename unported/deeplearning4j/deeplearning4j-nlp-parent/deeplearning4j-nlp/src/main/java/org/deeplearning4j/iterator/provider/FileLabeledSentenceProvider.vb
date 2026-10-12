Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports FileUtils = org.apache.commons.io.FileUtils
Imports LabeledSentenceProvider = org.deeplearning4j.iterator.LabeledSentenceProvider
Imports CompactHeapStringList = org.nd4j.common.collection.CompactHeapStringList
Imports org.nd4j.common.primitives
Imports MathUtils = org.nd4j.common.util.MathUtils

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

Namespace org.deeplearning4j.iterator.provider


	Public Class FileLabeledSentenceProvider
		Implements LabeledSentenceProvider

		Private ReadOnly totalCount As Integer
		Private ReadOnly filePaths As IList(Of String)
		Private ReadOnly fileLabelIndexes() As Integer
		Private ReadOnly rng As Random
		Private ReadOnly order() As Integer
'JAVA TO VB CONVERTER NOTE: The field allLabels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly allLabels_Conflict As IList(Of String)

		Private cursor As Integer = 0

		''' <param name="filesByLabel"> Key: label. Value: list of files for that label </param>
		Public Sub New(ByVal filesByLabel As IDictionary(Of String, IList(Of File)))
			Me.New(filesByLabel, New Random())
		End Sub

		''' 
		''' <param name="filesByLabel"> Key: label. Value: list of files for that label </param>
		''' <param name="rng">          Random number generator. May be null. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FileLabeledSentenceProvider(@NonNull Map<String, List<java.io.File>> filesByLabel, Random rng)
		Public Sub New(ByVal filesByLabel As IDictionary(Of String, IList(Of File)), ByVal rng As Random)
			Dim totalCount As Integer = 0
			For Each l As IList(Of File) In filesByLabel.Values
				totalCount += l.Count
			Next l
			Me.totalCount = totalCount

			Me.rng = rng
			If rng Is Nothing Then
				order = Nothing
			Else
				order = New Integer(totalCount - 1){}
				For i As Integer = 0 To totalCount - 1
					order(i) = i
				Next i

				MathUtils.shuffleArray(order, rng)
			End If

			allLabels_Conflict = New List(Of String)(filesByLabel.Keys)
			allLabels_Conflict.Sort()

			Dim labelsToIdx As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			For i As Integer = 0 To allLabels_Conflict.Count - 1
				labelsToIdx(allLabels(i)) = i
			Next i

			filePaths = New CompactHeapStringList()
			fileLabelIndexes = New Integer(totalCount - 1){}
			Dim position As Integer = 0
			For Each entry As KeyValuePair(Of String, IList(Of File)) In filesByLabel.SetOfKeyValuePairs()
				Dim labelIdx As Integer = labelsToIdx(entry.Key)
				For Each f As File In entry.Value
					filePaths.Add(f.getPath())
					fileLabelIndexes(position) = labelIdx
					position += 1
				Next f
			Next entry
		End Sub

		Public Overridable Function hasNext() As Boolean Implements LabeledSentenceProvider.hasNext
			Return cursor < totalCount
		End Function

		Public Overridable Function nextSentence() As Pair(Of String, String) Implements LabeledSentenceProvider.nextSentence
			Dim idx As Integer
			If rng Is Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idx = cursor++;
				idx = cursor
					cursor += 1
			Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idx = order[cursor++];
				idx = order(cursor)
					cursor += 1
			End If
			Dim f As New File(filePaths(idx))
			Dim label As String = allLabels(fileLabelIndexes(idx))

			Dim sentence As String
			Try
				sentence = FileUtils.readFileToString(f)
			Catch e As IOException
				Throw New Exception(e)
			End Try

			Return New Pair(Of String, String)(sentence, label)
		End Function

		Public Overridable Sub reset() Implements LabeledSentenceProvider.reset
			cursor = 0
			If rng IsNot Nothing Then
				MathUtils.shuffleArray(order, rng)
			End If
		End Sub

		Public Overridable Function totalNumSentences() As Integer Implements LabeledSentenceProvider.totalNumSentences
			Return totalCount
		End Function

		Public Overridable Function allLabels() As IList(Of String)
			Return allLabels_Conflict
		End Function

		Public Overridable Function numLabelClasses() As Integer Implements LabeledSentenceProvider.numLabelClasses
			Return allLabels_Conflict.Count
		End Function
	End Class

End Namespace