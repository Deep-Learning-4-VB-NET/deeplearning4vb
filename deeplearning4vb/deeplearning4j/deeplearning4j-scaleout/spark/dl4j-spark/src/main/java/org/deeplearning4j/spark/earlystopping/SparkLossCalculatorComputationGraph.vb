Imports SparkContext = org.apache.spark.SparkContext
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports org.deeplearning4j.earlystopping.scorecalc
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.deeplearning4j.spark.earlystopping

	Public Class SparkLossCalculatorComputationGraph
		Implements ScoreCalculator(Of ComputationGraph)

		Private data As JavaRDD(Of MultiDataSet)
		Private average As Boolean
		Private sc As SparkContext

		''' <summary>
		''' Calculate the score (loss function value) on a given data set (usually a test set)
		''' </summary>
		''' <param name="data">    Data set to calculate the score for </param>
		''' <param name="average"> Whether to return the average (sum of loss / N) or just (sum of loss) </param>
		Public Sub New(ByVal data As JavaRDD(Of MultiDataSet), ByVal average As Boolean, ByVal sc As SparkContext)
			Me.data = data
			Me.average = average
			Me.sc = sc
		End Sub


		Public Overridable Function calculateScore(ByVal network As ComputationGraph) As Double
			Dim net As New SparkComputationGraph(sc, network, Nothing)
			Return net.calculateScoreMultiDataSet(data, average)
		End Function

		Public Overridable Function minimizeScore() As Boolean Implements ScoreCalculator(Of ComputationGraph).minimizeScore
			Return True 'Minimize loss
		End Function

	End Class

End Namespace