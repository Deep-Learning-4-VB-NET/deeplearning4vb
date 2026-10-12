Imports System
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports val = lombok.val
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports JsonIgnore = org.nd4j.shade.jackson.annotation.JsonIgnore
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.earlystopping.scorecalc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Deprecated public class DataSetLossCalculatorCG implements ScoreCalculator<org.deeplearning4j.nn.graph.ComputationGraph>
	<Obsolete>
	Public Class DataSetLossCalculatorCG
		Implements ScoreCalculator(Of ComputationGraph)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore private org.nd4j.linalg.dataset.api.iterator.DataSetIterator dataSetIterator;
		Private dataSetIterator As DataSetIterator
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore private org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator multiDataSetIterator;
		Private multiDataSetIterator As MultiDataSetIterator
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty private boolean average;
		Private average As Boolean

		''' <summary>
		'''Calculate the score (loss function value) on a given data set (usually a test set)
		''' </summary>
		''' <param name="dataSetIterator"> Data set to calculate the score for </param>
		''' <param name="average"> Whether to return the average (sum of loss / N) or just (sum of loss) </param>
		Public Sub New(ByVal dataSetIterator As DataSetIterator, ByVal average As Boolean)
			Me.dataSetIterator = dataSetIterator
			Me.average = average
		End Sub

		''' <summary>
		'''Calculate the score (loss function value) on a given data set (usually a test set)
		''' </summary>
		''' <param name="dataSetIterator"> Data set to calculate the score for </param>
		''' <param name="average"> Whether to return the average (sum of loss / N) or just (sum of loss) </param>
		Public Sub New(ByVal dataSetIterator As MultiDataSetIterator, ByVal average As Boolean)
			Me.multiDataSetIterator = dataSetIterator
			Me.average = average
		End Sub

		Public Overridable Function calculateScore(ByVal network As ComputationGraph) As Double
			Dim lossSum As Double = 0.0
			Dim exCount As Integer = 0

			If dataSetIterator IsNot Nothing Then
				dataSetIterator.reset()

				Do While dataSetIterator.MoveNext()
					Dim dataSet As DataSet = dataSetIterator.Current
					Dim nEx As val = dataSet.Features.size(0)
					lossSum += network.score(dataSet) * nEx
					exCount += nEx
				Loop
			Else
				multiDataSetIterator.reset()

				Do While multiDataSetIterator.MoveNext()
					Dim dataSet As MultiDataSet = multiDataSetIterator.Current
					Dim nEx As val = dataSet.getFeatures(0).size(0)
					lossSum += network.score(dataSet) * nEx
					exCount += nEx
				Loop
			End If

			If average Then
				Return lossSum / exCount
			Else
				Return lossSum
			End If
		End Function

		Public Overridable Function minimizeScore() As Boolean Implements ScoreCalculator(Of ComputationGraph).minimizeScore
			Return True
		End Function

		Public Overrides Function ToString() As String
			Return "DataSetLossCalculatorCG(" & dataSetIterator & ",average=" & average & ")"
		End Function
	End Class

End Namespace