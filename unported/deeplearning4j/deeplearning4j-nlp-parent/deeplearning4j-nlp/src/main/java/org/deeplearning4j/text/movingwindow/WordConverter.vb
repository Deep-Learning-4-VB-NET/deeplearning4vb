Imports System.Collections.Generic
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil

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



	Public Class WordConverter

		Private sentences As IList(Of String) = New List(Of String)()
		Private vec As Word2Vec
'JAVA TO VB CONVERTER NOTE: The variable windows was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private windows_Conflict As IList(Of Window)

		Public Sub New(ByVal sentences As IList(Of String), ByVal vec As Word2Vec)
			Me.sentences = sentences
			Me.vec = vec
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter windows was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function toInputMatrix(ByVal windows_Conflict As IList(Of Window), ByVal vec As Word2Vec) As INDArray
			Dim columns As Integer = vec.lookupTable().layerSize() * vec.getWindow()
			Dim rows As Integer = windows_Conflict.Count
			Dim ret As INDArray = Nd4j.create(rows, columns)
			For i As Integer = 0 To rows - 1
				ret.putRow(i, WindowConverter.asExampleMatrix(windows(i), vec))
			Next i
			Return ret
		End Function


		Public Overridable Function toInputMatrix() As INDArray
'JAVA TO VB CONVERTER NOTE: The variable windows was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim windows_Conflict As IList(Of Window) = allWindowsForAllSentences()
			Return toInputMatrix(windows_Conflict, vec)
		End Function



'JAVA TO VB CONVERTER NOTE: The parameter windows was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function toLabelMatrix(ByVal labels As IList(Of String), ByVal windows_Conflict As IList(Of Window)) As INDArray
			Dim columns As Integer = labels.Count
			Dim ret As INDArray = Nd4j.create(windows_Conflict.Count, columns)
			Dim i As Integer = 0
			Do While i < ret.rows()
				ret.putRow(i, FeatureUtil.toOutcomeVector(labels.IndexOf(windows(i).getLabel()), labels.Count))
				i += 1
			Loop
			Return ret
		End Function

		Public Overridable Function toLabelMatrix(ByVal labels As IList(Of String)) As INDArray
'JAVA TO VB CONVERTER NOTE: The variable windows was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim windows_Conflict As IList(Of Window) = allWindowsForAllSentences()
			Return toLabelMatrix(labels, windows_Conflict)
		End Function

		Private Function allWindowsForAllSentences() As IList(Of Window)
			If windows_Conflict IsNot Nothing Then
				Return windows_Conflict
			End If
			windows_Conflict = New List(Of Window)()
			For Each s As String In sentences
				If s.Length > 0 Then
					CType(windows_Conflict, List(Of Window)).AddRange(Windows.windows(s))
				End If
			Next s
			Return windows_Conflict
		End Function



	End Class

End Namespace