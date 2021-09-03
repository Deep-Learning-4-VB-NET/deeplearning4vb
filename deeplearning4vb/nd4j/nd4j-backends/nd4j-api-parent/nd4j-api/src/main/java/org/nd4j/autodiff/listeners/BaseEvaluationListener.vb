Imports System.Collections.Generic
Imports EvaluationRecord = org.nd4j.autodiff.listeners.records.EvaluationRecord
Imports LossCurve = org.nd4j.autodiff.listeners.records.LossCurve
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports org.nd4j.evaluation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.nd4j.autodiff.listeners

	Public MustInherit Class BaseEvaluationListener
		Inherits BaseListener

		Private trainingEvaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
		Private validationEvaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()

		''' <summary>
		''' Return the requested evaluations.  New instances of these evaluations will be made each time they are used
		''' </summary>
		Public MustOverride Function evaluations() As ListenerEvaluations

		Public NotOverridable Overrides Function requiredVariables(ByVal sd As SameDiff) As ListenerVariables
			Return evaluations().requiredVariables().merge(otherRequiredVariables(sd))
		End Function

		''' <summary>
		''' Return any requested variables that are not part of the evaluations
		''' </summary>
		Public Overridable Function otherRequiredVariables(ByVal sd As SameDiff) As ListenerVariables
			Return ListenerVariables.empty()
		End Function


'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public NotOverridable Overrides Sub epochStart(ByVal sd As SameDiff, ByVal at_Conflict As At)
			trainingEvaluations = New Dictionary(Of String, IList(Of IEvaluation))()
			For Each entry As KeyValuePair(Of String, IList(Of IEvaluation)) In evaluations().trainEvaluations().SetOfKeyValuePairs()

				Dim evals As IList(Of IEvaluation) = New List(Of IEvaluation)()
				For Each ie As IEvaluation In entry.Value
					evals.Add(ie.newInstance())
				Next ie

				trainingEvaluations(entry.Key) = evals
			Next entry
			validationEvaluations = New Dictionary(Of String, IList(Of IEvaluation))()
			For Each entry As KeyValuePair(Of String, IList(Of IEvaluation)) In evaluations().validationEvaluations().SetOfKeyValuePairs()

				Dim evals As IList(Of IEvaluation) = New List(Of IEvaluation)()
				For Each ie As IEvaluation In entry.Value
					evals.Add(ie.newInstance())
				Next ie

				validationEvaluations(entry.Key) = evals
			Next entry

			epochStartEvaluations(sd, at_Conflict)
		End Sub

		''' <summary>
		''' See <seealso cref="Listener.epochStart(SameDiff, At)"/>
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub epochStartEvaluations(ByVal sd As SameDiff, ByVal at_Conflict As At)
			'No op
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public NotOverridable Overrides Function epochEnd(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal lossCurve As LossCurve, ByVal epochTimeMillis As Long) As ListenerResponse
			Return epochEndEvaluations(sd, at_Conflict, lossCurve, epochTimeMillis, New EvaluationRecord(trainingEvaluations))
		End Function

		''' <summary>
		''' See <seealso cref="Listener.epochEnd(SameDiff, At, LossCurve, Long)"/>, also provided the requested evaluations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function epochEndEvaluations(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal lossCurve As LossCurve, ByVal epochTimeMillis As Long, ByVal evaluations As EvaluationRecord) As ListenerResponse
			'No op
			Return ListenerResponse.CONTINUE
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public NotOverridable Overrides Function validationDone(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal validationTimeMillis As Long) As ListenerResponse
			Return validationDoneEvaluations(sd, at_Conflict, validationTimeMillis, New EvaluationRecord(validationEvaluations))
		End Function

		''' <summary>
		''' See <seealso cref="Listener.validationDone(SameDiff, At, Long)"/>, also provided the requested evaluations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function validationDoneEvaluations(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal validationTimeMillis As Long, ByVal evaluations As EvaluationRecord) As ListenerResponse
			'No op
			Return ListenerResponse.CONTINUE
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public NotOverridable Overrides Sub activationAvailable(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal varName As String, ByVal activation As INDArray)
			If at_Conflict.operation() = Operation.TRAINING Then
				If trainingEvaluations.ContainsKey(varName) Then
					Dim labels As INDArray = batch.getLabels(evaluations().trainEvaluationLabels()(varName))
					Dim mask As INDArray = batch.getLabelsMaskArray(evaluations().trainEvaluationLabels()(varName))

					For Each e As IEvaluation In trainingEvaluations(varName)
						e.eval(labels, activation, mask)
					Next e
				End If
			ElseIf at_Conflict.operation() = Operation.TRAINING_VALIDATION Then
				If validationEvaluations.ContainsKey(varName) Then
					Dim labels As INDArray = batch.getLabels(evaluations().validationEvaluationLabels()(varName))
					Dim mask As INDArray = batch.getLabelsMaskArray(evaluations().validationEvaluationLabels()(varName))

					For Each e As IEvaluation In validationEvaluations(varName)
						e.eval(labels, activation, mask)
					Next e
				End If
			End If

			activationAvailableEvaluations(sd, at_Conflict, batch, op, varName, activation)
		End Sub

		''' <summary>
		''' See <seealso cref="Listener.activationAvailable(SameDiff, At, MultiDataSet, SameDiffOp, String, INDArray)"/>
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub activationAvailableEvaluations(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal varName As String, ByVal activation As INDArray)
			'No op
		End Sub

	End Class

End Namespace