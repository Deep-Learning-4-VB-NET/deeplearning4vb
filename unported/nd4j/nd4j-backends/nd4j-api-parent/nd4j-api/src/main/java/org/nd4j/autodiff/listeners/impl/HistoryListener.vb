Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseEvaluationListener = org.nd4j.autodiff.listeners.BaseEvaluationListener
Imports EvaluationRecord = org.nd4j.autodiff.listeners.records.EvaluationRecord
Imports History = org.nd4j.autodiff.listeners.records.History
Imports ListenerEvaluations = org.nd4j.autodiff.listeners.ListenerEvaluations
Imports ListenerResponse = org.nd4j.autodiff.listeners.ListenerResponse
Imports LossCurve = org.nd4j.autodiff.listeners.records.LossCurve
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig

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

Namespace org.nd4j.autodiff.listeners.impl


	Public Class HistoryListener
		Inherits BaseEvaluationListener

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.autodiff.listeners.ListenerEvaluations evaluations;
'JAVA TO VB CONVERTER NOTE: The field evaluations was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private evaluations_Conflict As ListenerEvaluations

		Private trainingHistory As IList(Of EvaluationRecord) = New List(Of EvaluationRecord)()
		Private validationHistory As IList(Of EvaluationRecord) = New List(Of EvaluationRecord)()
		Private loss As LossCurve = Nothing

		Private startTime As Long
		Private endTime As Long

		Private validationTimes As IList(Of Long) = New List(Of Long)()
		Private validationStartTime As Long


		Public Sub New(ByVal tc As TrainingConfig)
			Me.evaluations_Conflict = New ListenerEvaluations(tc.getTrainEvaluations(), tc.getTrainEvaluationLabels(), tc.getValidationEvaluations(), tc.getValidationEvaluationLabels())
		End Sub

		Public Sub New(ByVal evaluations As ListenerEvaluations)
			Me.evaluations_Conflict = evaluations
		End Sub

		Public Overridable Function newInstance() As HistoryListener
			Return New HistoryListener(evaluations_Conflict)
		End Function

		Public Overrides Function evaluations() As ListenerEvaluations
			Return evaluations_Conflict
		End Function

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return operation.isTrainingPhase()
		End Function

		Public Overrides Function epochEndEvaluations(ByVal sd As SameDiff, ByVal at As At, ByVal lossCurve As LossCurve, ByVal epochTimeMillis As Long, ByVal evaluations As EvaluationRecord) As ListenerResponse
			trainingHistory.Add(evaluations)
			loss = lossCurve

			Return ListenerResponse.CONTINUE
		End Function

		Public Overrides Function validationDoneEvaluations(ByVal sd As SameDiff, ByVal at As At, ByVal validationTimeMillis As Long, ByVal evaluations As EvaluationRecord) As ListenerResponse
			validationHistory.Add(evaluations)
			Return ListenerResponse.CONTINUE
		End Function

		Public Overrides Sub operationStart(ByVal sd As SameDiff, ByVal op As Operation)
			If op = Operation.TRAINING Then
				startTime = DateTimeHelper.CurrentUnixTimeMillis()
			ElseIf op = Operation.TRAINING_VALIDATION Then
				validationStartTime = DateTimeHelper.CurrentUnixTimeMillis()
			End If
		End Sub

		Public Overrides Sub operationEnd(ByVal sd As SameDiff, ByVal op As Operation)
			If op = Operation.TRAINING Then
				endTime = DateTimeHelper.CurrentUnixTimeMillis()
			ElseIf op = Operation.TRAINING_VALIDATION Then
				validationTimes.Add(DateTimeHelper.CurrentUnixTimeMillis() - validationStartTime)
			End If
		End Sub

		Public Overridable ReadOnly Property Report As History
			Get
				Return New History(trainingHistory, validationHistory, loss, endTime - startTime, validationTimes)
			End Get
		End Property

	End Class

End Namespace