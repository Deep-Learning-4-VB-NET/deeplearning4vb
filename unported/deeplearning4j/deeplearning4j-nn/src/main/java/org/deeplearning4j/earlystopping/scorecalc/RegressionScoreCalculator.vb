Imports org.deeplearning4j.earlystopping.scorecalc.base
Imports Model = org.deeplearning4j.nn.api.Model
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports Metric = org.nd4j.evaluation.regression.RegressionEvaluation.Metric
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

	Public Class RegressionScoreCalculator
		Inherits BaseIEvaluationScoreCalculator(Of Model, RegressionEvaluation)

		Protected Friend ReadOnly metric As RegressionEvaluation.Metric

		Public Sub New(ByVal metric As RegressionEvaluation.Metric, ByVal iterator As DataSetIterator)
			MyBase.New(iterator)
			Me.metric = metric
		End Sub

		Protected Friend Overrides Function newEval() As RegressionEvaluation
			Return New RegressionEvaluation()
		End Function

		Protected Friend Overrides Function finalScore(ByVal eval As RegressionEvaluation) As Double
			Return eval.scoreForMetric(metric)
		End Function

		Public Overrides Function minimizeScore() As Boolean
			Return metric.minimize()
		End Function
	End Class

End Namespace