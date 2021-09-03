Imports System
Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.nd4j.linalg.dataset.api.iterator.fetcher


	<Serializable>
	Public MustInherit Class BaseDataFetcher
		Implements DataSetFetcher

		Public MustOverride Sub fetch(ByVal numExamples As Integer)

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BaseDataFetcher))
		''' 
		Private Const serialVersionUID As Long = -859588773699432365L
'JAVA TO VB CONVERTER NOTE: The field cursor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend cursor_Conflict As Integer = 0
		Protected Friend numOutcomes As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field inputColumns was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputColumns_Conflict As Integer = -1
		Protected Friend curr As DataSet
'JAVA TO VB CONVERTER NOTE: The field totalExamples was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend totalExamples_Conflict As Integer

		''' <summary>
		''' Creates a feature vector
		''' </summary>
		''' <param name="numRows"> the number of examples </param>
		''' <returns> a feature vector </returns>
		Protected Friend Overridable Function createInputMatrix(ByVal numRows As Integer) As INDArray
			Return Nd4j.create(numRows, inputColumns_Conflict)
		End Function

		''' <summary>
		''' Creates an output label matrix
		''' </summary>
		''' <param name="outcomeLabel"> the outcome label to use </param>
		''' <returns> a binary vector where 1 is transform to the
		''' index specified by outcomeLabel </returns>
		Protected Friend Overridable Function createOutputVector(ByVal outcomeLabel As Integer) As INDArray
			Return FeatureUtil.toOutcomeVector(outcomeLabel, numOutcomes)
		End Function

		Protected Friend Overridable Function createOutputMatrix(ByVal numRows As Integer) As INDArray
			Return Nd4j.create(numRows, numOutcomes)
		End Function

		''' <summary>
		''' Initializes this data transform fetcher from the passed in datasets
		''' </summary>
		''' <param name="examples"> the examples to use </param>
		Protected Friend Overridable Sub initializeCurrFromList(ByVal examples As IList(Of DataSet))

			If examples.Count = 0 Then
				log.warn("Warning: empty dataset from the fetcher")
			End If

			Dim inputs As INDArray = createInputMatrix(examples.Count)
			Dim labels As INDArray = createOutputMatrix(examples.Count)
			For i As Integer = 0 To examples.Count - 1
				inputs.putRow(i, examples(i).getFeatures())
				labels.putRow(i, examples(i).getLabels())
			Next i
			curr = New DataSet(inputs, labels)

		End Sub

		Public Overridable Function hasMore() As Boolean Implements DataSetFetcher.hasMore
			Return cursor_Conflict < totalExamples_Conflict
		End Function

		Public Overridable Function [next]() As DataSet Implements DataSetFetcher.next
			Return curr
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetFetcher.totalOutcomes
			Return numOutcomes
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetFetcher.inputColumns
			Return inputColumns_Conflict
		End Function

		Public Overridable Function totalExamples() As Integer Implements DataSetFetcher.totalExamples
			Return totalExamples_Conflict
		End Function

		Public Overridable Sub reset() Implements DataSetFetcher.reset
			cursor_Conflict = 0
		End Sub

		Public Overridable Function cursor() As Integer Implements DataSetFetcher.cursor
			Return cursor_Conflict
		End Function


	End Class

End Namespace