Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports Model = org.deeplearning4j.nn.api.Model
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener

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

Namespace org.deeplearning4j.optimize.listeners


	Public Class CollectScoresIterationListener
		Inherits BaseTrainingListener

		Private frequency As Integer
		Private iterationCount As Integer = 0
		'private List<Pair<Integer, Double>> scoreVsIter = new ArrayList<>();

		Public Class ScoreStat
			Public Const BUCKET_LENGTH As Integer = 10000

			Friend position As Integer = 0
			Friend bucketNumber As Integer = 1
			Friend indexes As IList(Of Long())
			Friend scores As IList(Of Double())

			Public Sub New()
				indexes = New List(Of Long())(1)
				indexes.Add(New Long(BUCKET_LENGTH - 1){})
				scores = New List(Of Double())(1)
				scores.Add(New Double(BUCKET_LENGTH - 1){})
			End Sub

			Public Overridable ReadOnly Property Indexes As IList(Of Long())
				Get
					Return indexes
				End Get
			End Property

			Public Overridable ReadOnly Property Scores As IList(Of Double())
				Get
					Return scores
				End Get
			End Property

			Public Overridable ReadOnly Property EffectiveIndexes As Long()
				Get
					Return Arrays.CopyOfRange(indexes(0), 0, position)
				End Get
			End Property

			Public Overridable ReadOnly Property EffectiveScores As Double()
				Get
					Return Arrays.CopyOfRange(scores(0), 0, position)
				End Get
			End Property


	'        
	'            Originally scores array is initialized with BUCKET_LENGTH size.
	'            When data doesn't fit there - arrays size is increased for BUCKET_LENGTH,
	'            old data is copied and bucketNumber (counter of reallocations) being incremented.
	'
	'            If we got more score points than MAX_VALUE - they are put to another item of scores list.
	'         
			Friend Overridable Sub reallocateGuard()
				If position >= BUCKET_LENGTH * bucketNumber Then

					Dim fullLength As Long = CLng(BUCKET_LENGTH) * bucketNumber

					If position = Integer.MaxValue OrElse fullLength >= Integer.MaxValue Then
						position = 0
						Dim newIndexes(BUCKET_LENGTH - 1) As Long
						Dim newScores(BUCKET_LENGTH - 1) As Double
						indexes.Add(newIndexes)
						scores.Add(newScores)
					Else
						Dim newIndexes((CInt(fullLength) + BUCKET_LENGTH) - 1) As Long
						Dim newScores((CInt(fullLength) + BUCKET_LENGTH) - 1) As Double
						Array.Copy(indexes(indexes.Count - 1), 0, newIndexes, 0, CInt(fullLength))
						Array.Copy(scores(scores.Count - 1), 0, newScores, 0, CInt(fullLength))
						scores.RemoveAt(scores.Count - 1)
						indexes.RemoveAt(indexes.Count - 1)
						Dim lastIndex As Integer = If(scores.Count = 0, 0, scores.Count - 1)
						scores.Insert(lastIndex, newScores)
						indexes.Insert(lastIndex, newIndexes)
					End If
					bucketNumber += 1
				End If
			End Sub

			Public Overridable Sub addScore(ByVal index As Long, ByVal score As Double)
				reallocateGuard()
				scores(scores.Count - 1)(position) = score
				indexes(scores.Count - 1)(position) = index
				position += 1
			End Sub
		End Class

'JAVA TO VB CONVERTER NOTE: The field scoreVsIter was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Friend scoreVsIter_Conflict As New ScoreStat()

		''' <summary>
		''' Constructor for collecting scores with default saving frequency of 1
		''' </summary>
		Public Sub New()
			Me.New(1)
		End Sub

		''' <summary>
		''' Constructor for collecting scores with the specified frequency. </summary>
		''' <param name="frequency">    Frequency with which to collect/save scores </param>
		Public Sub New(ByVal frequency As Integer)
			If frequency <= 0 Then
				frequency = 1
			End If
			Me.frequency = frequency
		End Sub

		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			iterationCount += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (++iterationCount % frequency == 0)
			If iterationCount Mod frequency = 0 Then
				Dim score As Double = model.score()
				scoreVsIter_Conflict.reallocateGuard()
				scoreVsIter_Conflict.addScore(iteration, score)
			End If
		End Sub

		Public Overridable ReadOnly Property ScoreVsIter As ScoreStat
			Get
				Return scoreVsIter_Conflict
			End Get
		End Property

		''' <summary>
		''' Export the scores in tab-delimited (one per line) UTF-8 format.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void exportScores(java.io.OutputStream outputStream) throws java.io.IOException
		Public Overridable Sub exportScores(ByVal outputStream As Stream)
			exportScores(outputStream, vbTab)
		End Sub

		''' <summary>
		''' Export the scores in delimited (one per line) UTF-8 format with the specified delimiter
		''' </summary>
		''' <param name="outputStream"> Stream to write to </param>
		''' <param name="delimiter">    Delimiter to use </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void exportScores(java.io.OutputStream outputStream, String delimiter) throws java.io.IOException
		Public Overridable Sub exportScores(ByVal outputStream As Stream, ByVal delimiter As String)
			Dim sb As New StringBuilder()
			sb.Append("Iteration").Append(delimiter).Append("Score")
			Dim largeBuckets As Integer = scoreVsIter_Conflict.indexes.Count
			For j As Integer = 0 To largeBuckets - 1
				Dim indexes() As Long = scoreVsIter_Conflict.indexes(j)
				Dim scores() As Double = scoreVsIter_Conflict.scores(j)

				Dim effectiveLength As Integer = If(j < largeBuckets -1, indexes.Length, scoreVsIter_Conflict.position)

				For i As Integer = 0 To effectiveLength - 1
					sb.Append(vbLf).Append(indexes(i)).Append(delimiter).Append(scores(i))
				Next i
			Next j
			outputStream.WriteByte(sb.ToString().GetBytes(Encoding.UTF8))
		End Sub

		''' <summary>
		''' Export the scores to the specified file in delimited (one per line) UTF-8 format, tab delimited
		''' </summary>
		''' <param name="file"> File to write to </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void exportScores(java.io.File file) throws java.io.IOException
		Public Overridable Sub exportScores(ByVal file As File)
			exportScores(file, vbTab)
		End Sub

		''' <summary>
		''' Export the scores to the specified file in delimited (one per line) UTF-8 format, using the specified delimiter
		''' </summary>
		''' <param name="file">      File to write to </param>
		''' <param name="delimiter"> Delimiter to use for writing scores </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void exportScores(java.io.File file, String delimiter) throws java.io.IOException
		Public Overridable Sub exportScores(ByVal file As File, ByVal delimiter As String)
			Using fos As New FileStream(file, FileMode.Create, FileAccess.Write)
				exportScores(fos, delimiter)
			End Using
		End Sub

	End Class

End Namespace