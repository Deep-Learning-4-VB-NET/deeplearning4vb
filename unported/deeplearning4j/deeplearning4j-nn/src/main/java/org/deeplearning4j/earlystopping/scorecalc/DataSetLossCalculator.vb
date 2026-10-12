Imports System
Imports org.deeplearning4j.earlystopping.scorecalc.base
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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

	Public Class DataSetLossCalculator
		Inherits BaseScoreCalculator(Of Model)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty private boolean average;
		Private average As Boolean

		''' <summary>
		''' Calculate the score (loss function value) on a given data set (usually a test set)
		''' </summary>
		''' <param name="dataSetIterator"> Data set to calculate the score for </param>
		''' <param name="average">         Whether to return the average (sum of loss / N) or just (sum of loss) </param>
		Public Sub New(ByVal dataSetIterator As DataSetIterator, ByVal average As Boolean)
			MyBase.New(dataSetIterator)
			Me.average = average
		End Sub

		''' <summary>
		'''Calculate the score (loss function value) on a given data set (usually a test set)
		''' </summary>
		''' <param name="dataSetIterator"> Data set to calculate the score for </param>
		''' <param name="average"> Whether to return the average (sum of loss / N) or just (sum of loss) </param>
		Public Sub New(ByVal dataSetIterator As MultiDataSetIterator, ByVal average As Boolean)
			MyBase.New(dataSetIterator)
			Me.average = average
		End Sub

		Public Overrides Function ToString() As String
			Return "DataSetLossCalculator(average=" & average & ")"
		End Function

		Protected Friend Overrides Sub reset()
			scoreSum = 0
			minibatchCount = 0
			exampleCount = 0
		End Sub

		Protected Friend Overrides Function output(ByVal network As Model, ByVal input As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray) As INDArray
			Return output(network, arr(input), arr(fMask), arr(lMask))(0)
		End Function

		Protected Friend Overrides Function output(ByVal network As Model, ByVal input() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray) As INDArray()
			If TypeOf network Is MultiLayerNetwork Then
				Dim [out] As INDArray = DirectCast(network, MultiLayerNetwork).output(input(0), False, get0(fMask), get0(lMask))
				Return New INDArray(){[out]}
			ElseIf TypeOf network Is ComputationGraph Then
				Return DirectCast(network, ComputationGraph).output(False, input, fMask, lMask)
			Else
				Throw New Exception("Unknown model type: " & network.GetType())
			End If
		End Function

		Protected Friend Overrides Function scoreMinibatch(ByVal network As Model, ByVal features() As INDArray, ByVal labels() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray, ByVal output() As INDArray) As Double
			If TypeOf network Is MultiLayerNetwork Then
				Return DirectCast(network, MultiLayerNetwork).score(New DataSet(get0(features), get0(labels), get0(fMask), get0(lMask)), False) * features(0).size(0)
			ElseIf TypeOf network Is ComputationGraph Then
				Return DirectCast(network, ComputationGraph).score(New MultiDataSet(features, labels, fMask, lMask)) * features(0).size(0)
			Else
				Throw New Exception("Unknown model type: " & network.GetType())
			End If
		End Function

		Protected Friend Overrides Function finalScore(ByVal scoreSum As Double, ByVal minibatchCount As Integer, ByVal exampleCount As Integer) As Double
			If average Then
				Return scoreSum / exampleCount
			Else
				Return scoreSum
			End If
		End Function

		Public Overrides Function minimizeScore() As Boolean
			Return True 'Minimize loss
		End Function
	End Class

End Namespace