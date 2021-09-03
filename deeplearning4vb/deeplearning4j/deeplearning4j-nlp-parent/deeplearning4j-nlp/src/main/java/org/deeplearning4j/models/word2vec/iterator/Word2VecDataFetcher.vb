Imports System
Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports Window = org.deeplearning4j.text.movingwindow.Window
Imports WindowConverter = org.deeplearning4j.text.movingwindow.WindowConverter
Imports Windows = org.deeplearning4j.text.movingwindow.Windows
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetFetcher = org.nd4j.linalg.dataset.api.iterator.fetcher.DataSetFetcher
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
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

Namespace org.deeplearning4j.models.word2vec.iterator


	''' 
	<Serializable>
	Public Class Word2VecDataFetcher
		Implements DataSetFetcher

		''' 
		Private Const serialVersionUID As Long = 3245955804749769475L
		<NonSerialized>
		Private files As IEnumerator(Of File)
'JAVA TO VB CONVERTER NOTE: The field vec was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private vec_Conflict As Word2Vec
'JAVA TO VB CONVERTER NOTE: The field begin was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared begin_Conflict As Pattern = Pattern.compile("<[A-Z]+>")
'JAVA TO VB CONVERTER NOTE: The field end was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared end_Conflict As Pattern = Pattern.compile("</[A-Z]+>")
		Private labels As IList(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER NOTE: The field batch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private batch_Conflict As Integer
		Private cache As IList(Of Window) = New List(Of Window)()
		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(Word2VecDataFetcher))
'JAVA TO VB CONVERTER NOTE: The field totalExamples was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private totalExamples_Conflict As Integer
		Private path As String

		Public Sub New(ByVal path As String, ByVal vec As Word2Vec, ByVal labels As IList(Of String))
			If vec Is Nothing OrElse labels Is Nothing OrElse labels.Count = 0 Then
				Throw New System.ArgumentException("Unable to initialize due to missing argument or empty label applyTransformToDestination")
			End If
			Me.vec_Conflict = vec
			Me.labels = labels
			Me.path = path
		End Sub



		Private Function fromCache() As DataSet
			Dim outcomes As INDArray = Nothing
			Dim input As INDArray = Nothing
			input = Nd4j.create(batch_Conflict, vec_Conflict.lookupTable().layerSize() * vec_Conflict.getWindow())
			outcomes = Nd4j.create(batch_Conflict, labels.Count)
			For i As Integer = 0 To batch_Conflict - 1
				input.putRow(i, WindowConverter.asExampleMatrix(cache(i), vec_Conflict))
				Dim idx As Integer = labels.IndexOf(cache(i).getLabel())
				If idx < 0 Then
					idx = 0
				End If
				outcomes.putRow(i, FeatureUtil.toOutcomeVector(idx, labels.Count))
			Next i
			Return New DataSet(input, outcomes)

		End Function

		Public Overridable Function [next]() As DataSet Implements DataSetFetcher.next
			'pop from cache when possible, or when there's nothing left
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If cache.Count >= batch_Conflict OrElse Not files.hasNext() Then
				Return fromCache()
			End If



'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim f As File = files.next()
			Try
				Dim lines As LineIterator = FileUtils.lineIterator(f)
				Dim outcomes As INDArray = Nothing
				Dim input As INDArray = Nothing

				Do While lines.hasNext()
					Dim windows As IList(Of Window) = Windows.windows(lines.nextLine())
					If windows.Count = 0 AndAlso lines.hasNext() Then
						Continue Do
					End If

					If windows.Count < batch_Conflict Then
						input = Nd4j.create(windows.Count, vec_Conflict.lookupTable().layerSize() * vec_Conflict.getWindow())
						outcomes = Nd4j.create(batch_Conflict, labels.Count)
						For i As Integer = 0 To windows.Count - 1
							input.putRow(i, WindowConverter.asExampleMatrix(cache(i), vec_Conflict))
							Dim idx As Integer = labels.IndexOf(windows(i).getLabel())
							If idx < 0 Then
								idx = 0
							End If
							Dim outcomeRow As INDArray = FeatureUtil.toOutcomeVector(idx, labels.Count)
							outcomes.putRow(i, outcomeRow)
						Next i
						Return New DataSet(input, outcomes)


					Else
						input = Nd4j.create(batch_Conflict, vec_Conflict.lookupTable().layerSize() * vec_Conflict.getWindow())
						outcomes = Nd4j.create(batch_Conflict, labels.Count)
						For i As Integer = 0 To batch_Conflict - 1
							input.putRow(i, WindowConverter.asExampleMatrix(cache(i), vec_Conflict))
							Dim idx As Integer = labels.IndexOf(windows(i).getLabel())
							If idx < 0 Then
								idx = 0
							End If
							Dim outcomeRow As INDArray = FeatureUtil.toOutcomeVector(idx, labels.Count)
							outcomes.putRow(i, outcomeRow)
						Next i
						'add left over to cache; need to ensure that only batch rows are returned
	'                    
	'                     * Note that I'm aware of possible concerns for sentence sequencing.
	'                     * This is a hack right now in place of something
	'                     * that will be way more elegant in the future.
	'                     
						If windows.Count > batch_Conflict Then
							Dim leftOvers As IList(Of Window) = windows.subList(batch_Conflict, windows.Count)
							CType(cache, List(Of Window)).AddRange(leftOvers)
						End If
						Return New DataSet(input, outcomes)
					End If

				Loop
			Catch e As IOException
				Throw New Exception(e)
			End Try

			Return Nothing
		End Function



		Public Overridable Function totalExamples() As Integer Implements DataSetFetcher.totalExamples
			Return totalExamples_Conflict
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetFetcher.inputColumns
			Return vec_Conflict.lookupTable().layerSize() * vec_Conflict.getWindow()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetFetcher.totalOutcomes
			Return labels.Count

		End Function

		Public Overridable Sub reset() Implements DataSetFetcher.reset
			files = FileUtils.iterateFiles(New File(path), Nothing, True)
			cache.Clear()

		End Sub



		Public Overridable Function cursor() As Integer Implements DataSetFetcher.cursor
			Return 0

		End Function



		Public Overridable Function hasMore() As Boolean Implements DataSetFetcher.hasMore
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return files.hasNext() OrElse cache.Count > 0
		End Function

		Public Overridable Sub fetch(ByVal numExamples As Integer) Implements DataSetFetcher.fetch
			Me.batch_Conflict = numExamples
		End Sub

		Public Overridable ReadOnly Property Files As IEnumerator(Of File)
			Get
				Return files
			End Get
		End Property

		Public Overridable ReadOnly Property Vec As Word2Vec
			Get
				Return vec_Conflict
			End Get
		End Property

		Public Shared ReadOnly Property Begin As Pattern
			Get
				Return begin_Conflict
			End Get
		End Property

		Public Shared ReadOnly Property End As Pattern
			Get
				Return end_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				Return labels
			End Get
		End Property

		Public Overridable ReadOnly Property Batch As Integer
			Get
				Return batch_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Cache As IList(Of Window)
			Get
				Return cache
			End Get
		End Property



	End Class

End Namespace