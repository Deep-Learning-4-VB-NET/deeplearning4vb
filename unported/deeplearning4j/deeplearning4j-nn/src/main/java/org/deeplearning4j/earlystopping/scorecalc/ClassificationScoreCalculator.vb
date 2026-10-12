Imports org.deeplearning4j.earlystopping.scorecalc.base
Imports Model = org.deeplearning4j.nn.api.Model
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

	Public Class ClassificationScoreCalculator
		Inherits BaseIEvaluationScoreCalculator(Of Model, Evaluation)

		Protected Friend ReadOnly metric As Evaluation.Metric

		Public Sub New(ByVal metric As Evaluation.Metric, ByVal iterator As DataSetIterator)
			MyBase.New(iterator)
			Me.metric = metric
		End Sub

		Public Sub New(ByVal metric As Evaluation.Metric, ByVal iterator As MultiDataSetIterator)
			MyBase.New(iterator)
			Me.metric = metric
		End Sub

		Protected Friend Overrides Function newEval() As Evaluation
			Return New Evaluation()
		End Function

		Protected Friend Overrides Function finalScore(ByVal e As Evaluation) As Double
			Return e.scoreForMetric(metric)
		End Function

		Public Overrides Function minimizeScore() As Boolean
			'All classification metrics should be maximized: ACCURACY, F1, PRECISION, RECALL, GMEASURE, MCC
			Return False
		End Function
	End Class

End Namespace