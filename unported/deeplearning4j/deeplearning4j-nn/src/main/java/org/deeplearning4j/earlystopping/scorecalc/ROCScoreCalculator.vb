Imports org.deeplearning4j.earlystopping.scorecalc.base
Imports Model = org.deeplearning4j.nn.api.Model
Imports org.nd4j.evaluation
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCBinary = org.nd4j.evaluation.classification.ROCBinary
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
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

	Public Class ROCScoreCalculator
		Inherits BaseIEvaluationScoreCalculator(Of Model, IEvaluation)

		Public Enum ROCType
			ROC
			BINARY
			MULTICLASS
		End Enum
		Public Enum Metric
			AUC
			AUPRC

		End Enum
		Protected Friend ReadOnly type As ROCType
		Protected Friend ReadOnly metric As Metric

		Public Sub New(ByVal type As ROCType, ByVal iterator As DataSetIterator)
			Me.New(type, Metric.AUC, iterator)
		End Sub

		Public Sub New(ByVal type As ROCType, ByVal iterator As MultiDataSetIterator)
			Me.New(type, Metric.AUC, iterator)
		End Sub

		Public Sub New(ByVal type As ROCType, ByVal metric As Metric, ByVal iterator As DataSetIterator)
			MyBase.New(iterator)
			Me.type = type
			Me.metric = metric
		End Sub

		Public Sub New(ByVal type As ROCType, ByVal metric As Metric, ByVal iterator As MultiDataSetIterator)
			MyBase.New(iterator)
			Me.type = type
			Me.metric = metric
		End Sub


		Protected Friend Overrides Function newEval() As IEvaluation
			Select Case type
				Case ROC
					Return New ROC()
				Case org.deeplearning4j.earlystopping.scorecalc.ROCScoreCalculator.ROCType.BINARY
					Return New ROCBinary()
				Case org.deeplearning4j.earlystopping.scorecalc.ROCScoreCalculator.ROCType.MULTICLASS
					Return New ROCMultiClass()
				Case Else
					Throw New System.InvalidOperationException("Unknown type: " & type)
			End Select
		End Function

		Protected Friend Overrides Function finalScore(ByVal eval As IEvaluation) As Double
			Select Case type
				Case ROC
					Dim r As ROC = CType(eval, ROC)
					Return If(metric = Metric.AUC, r.calculateAUC(), r.calculateAUCPR())
				Case org.deeplearning4j.earlystopping.scorecalc.ROCScoreCalculator.ROCType.BINARY
					Dim r2 As ROCBinary = CType(eval, ROCBinary)
					Return If(metric = Metric.AUC, r2.calculateAverageAuc(), r2.calculateAverageAUCPR())
				Case org.deeplearning4j.earlystopping.scorecalc.ROCScoreCalculator.ROCType.MULTICLASS
					Dim r3 As ROCMultiClass = CType(eval, ROCMultiClass)
					Return If(metric = Metric.AUC, r3.calculateAverageAUC(), r3.calculateAverageAUCPR())
				Case Else
					Throw New System.InvalidOperationException("Unknown type: " & type)
			End Select
		End Function

		Public Overrides Function minimizeScore() As Boolean
			Return False 'Maximize AUC, AUPRC
		End Function
	End Class

End Namespace