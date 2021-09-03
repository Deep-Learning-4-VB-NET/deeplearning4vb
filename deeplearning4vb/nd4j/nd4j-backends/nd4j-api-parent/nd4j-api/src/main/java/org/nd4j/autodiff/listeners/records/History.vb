Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric
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

Namespace org.nd4j.autodiff.listeners.records


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class History
	Public Class History

		Private trainingHistory As IList(Of EvaluationRecord)
		Private validationHistory As IList(Of EvaluationRecord)

'JAVA TO VB CONVERTER NOTE: The field lossCurve was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lossCurve_Conflict As LossCurve

'JAVA TO VB CONVERTER NOTE: The field trainingTimeMillis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private trainingTimeMillis_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field validationTimesMillis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private validationTimesMillis_Conflict As IList(Of Long)

		Public Sub New(ByVal training As IList(Of EvaluationRecord), ByVal validation As IList(Of EvaluationRecord), ByVal loss As LossCurve, ByVal trainingTimeMillis As Long, ByVal validationTimesMillis As IList(Of Long))
			trainingHistory = Collections.unmodifiableList(training)
			validationHistory = Collections.unmodifiableList(validation)
			Me.lossCurve_Conflict = loss
			Me.trainingTimeMillis_Conflict = trainingTimeMillis
			Me.validationTimesMillis_Conflict = Collections.unmodifiableList(validationTimesMillis)
		End Sub

		''' <summary>
		''' Get the training evaluations
		''' </summary>
		Public Overridable Function trainingEval() As IList(Of EvaluationRecord)
			Return trainingHistory
		End Function

		''' <summary>
		''' Get the validation evaluations
		''' </summary>
		Public Overridable Function validationEval() As IList(Of EvaluationRecord)
			Return validationHistory
		End Function

		''' <summary>
		''' Get the loss curve
		''' </summary>
		Public Overridable Function lossCurve() As LossCurve
			Return lossCurve_Conflict
		End Function

		''' <summary>
		''' Get the total training time, in milliseconds
		''' </summary>
		Public Overridable Function trainingTimeMillis() As Long
			Return trainingTimeMillis_Conflict
		End Function

		''' <summary>
		''' Get the total validation time, in milliseconds
		''' </summary>
		Public Overridable Function validationTimesMillis() As IList(Of Long)
			Return validationTimesMillis_Conflict
		End Function

		''' <summary>
		''' Get the number of epochs trained for
		''' </summary>
		Public Overridable Function trainingEpochs() As Integer
			Return trainingHistory.Count
		End Function

		''' <summary>
		''' Get the number of epochs validation was ran on
		''' </summary>
		Public Overridable Function validationEpochs() As Integer
			Return validationHistory.Count
		End Function

		''' <summary>
		''' Get the results of a training evaluation on a given parameter for a given metric
		''' 
		''' Only works if there is only one evaluation with the given metric for param
		''' </summary>
		Public Overridable Function trainingEval(ByVal param As String, ByVal metric As IMetric) As IList(Of Double)
			Dim data As IList(Of Double) = New List(Of Double)()
			For Each er As EvaluationRecord In trainingHistory
				data.Add(er.getValue(param, metric))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a training evaluation on a given parameter for a given metric
		''' 
		''' Only works if there is only one evaluation with the given metric for param
		''' </summary>
		Public Overridable Function trainingEval(ByVal param As SDVariable, ByVal metric As IMetric) As IList(Of Double)
			Return trainingEval(param.name(), metric)
		End Function

		''' <summary>
		''' Get the results of a training evaluation on a given parameter at a given index, for a given metric
		''' 
		''' Note that it returns all recorded evaluations.
		''' Index determines the evaluation used not the epoch's results to return.
		''' </summary>
		Public Overridable Function trainingEval(ByVal param As String, ByVal index As Integer, ByVal metric As IMetric) As IList(Of Double)
			Dim data As IList(Of Double) = New List(Of Double)()
			For Each er As EvaluationRecord In trainingHistory
				data.Add(er.getValue(param, index, metric))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a training evaluation on a given parameter at a given index, for a given metric
		''' 
		''' Note that it returns all recorded evaluations.
		''' Index determines the evaluation used not the epoch's results to return.
		''' </summary>
		Public Overridable Function trainingEval(ByVal param As SDVariable, ByVal index As Integer, ByVal metric As IMetric) As IList(Of Double)
			Return trainingEval(param.name(), index, metric)
		End Function

		''' <summary>
		''' Get the results of a training evaluation for a given metric
		''' 
		''' Only works if there is only one evaluation with the given metric
		''' </summary>
		Public Overridable Function trainingEval(ByVal metric As IMetric) As IList(Of Double)
			Dim data As IList(Of Double) = New List(Of Double)()
			For Each er As EvaluationRecord In trainingHistory
				data.Add(er.getValue(metric))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a training evaluation on a given parameter
		''' 
		''' Only works if there is only one evaluation for param.
		''' </summary>
		Public Overridable Function trainingEval(ByVal param As String) As IList(Of IEvaluation)
			Dim data As IList(Of IEvaluation) = New List(Of IEvaluation)()
			For Each er As EvaluationRecord In trainingHistory
				data.Add(er.evaluation(param))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a training evaluation on a given parameter
		''' 
		''' Only works if there is only one evaluation for param.
		''' </summary>
		Public Overridable Function trainingEval(ByVal param As SDVariable) As IList(Of IEvaluation)
			Return trainingEval(param.name())
		End Function

		''' <summary>
		''' Get the results of a training evaluation on a given parameter at a given index
		''' 
		''' Note that it returns all recorded evaluations.
		''' Index determines the evaluation used not the epoch's results to return.
		''' </summary>
		Public Overridable Function trainingEval(ByVal param As String, ByVal index As Integer) As IList(Of IEvaluation)
			Dim data As IList(Of IEvaluation) = New List(Of IEvaluation)()
			For Each er As EvaluationRecord In trainingHistory
				data.Add(er.evaluation(param, index))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a training evaluation on a given parameter at a given index
		''' 
		''' Note that it returns all recorded evaluations.
		''' Index determines the evaluation used not the epoch's results to return.
		''' </summary>
		Public Overridable Function trainingEval(ByVal param As SDVariable, ByVal index As Integer) As IList(Of IEvaluation)
			Return trainingEval(param.name(), index)
		End Function

		''' <summary>
		''' Get the results of a validation evaluation on a given parameter for a given metric
		''' 
		''' Only works if there is only one evaluation with the given metric for param
		''' </summary>
		Public Overridable Function validationEval(ByVal param As String, ByVal metric As IMetric) As IList(Of Double)
			Dim data As IList(Of Double) = New List(Of Double)()
			For Each er As EvaluationRecord In validationHistory
				data.Add(er.getValue(param, metric))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a validation evaluation on a given parameter for a given metric
		''' 
		''' Only works if there is only one evaluation with the given metric for param
		''' </summary>
		Public Overridable Function validationEval(ByVal param As SDVariable, ByVal metric As IMetric) As IList(Of Double)
			Return validationEval(param.name(), metric)
		End Function

		''' <summary>
		''' Get the results of a validation evaluation on a given parameter at a given index, for a given metric
		''' 
		''' Note that it returns all recorded evaluations.
		''' Index determines the evaluation used not the epoch's results to return.
		''' </summary>
		Public Overridable Function validationEval(ByVal param As String, ByVal index As Integer, ByVal metric As IMetric) As IList(Of Double)
			Dim data As IList(Of Double) = New List(Of Double)()
			For Each er As EvaluationRecord In validationHistory
				data.Add(er.getValue(param, index, metric))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a validation evaluation on a given parameter at a given index, for a given metric
		''' 
		''' Note that it returns all recorded evaluations.
		''' Index determines the evaluation used not the epoch's results to return.
		''' </summary>
		Public Overridable Function validationEval(ByVal param As SDVariable, ByVal index As Integer, ByVal metric As IMetric) As IList(Of Double)
			Return validationEval(param.name(), index, metric)
		End Function

		''' <summary>
		''' Get the results of a validation evaluation for a given metric
		''' 
		''' Only works if there is only one evaluation with the given metric
		''' </summary>
		Public Overridable Function validationEval(ByVal metric As IMetric) As IList(Of Double)
			Dim data As IList(Of Double) = New List(Of Double)()
			For Each er As EvaluationRecord In validationHistory
				data.Add(er.getValue(metric))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a validation evaluation on a given parameter
		''' 
		''' Only works if there is only one evaluation for param.
		''' </summary>
		Public Overridable Function validationEval(ByVal param As String) As IList(Of IEvaluation)
			Dim data As IList(Of IEvaluation) = New List(Of IEvaluation)()
			For Each er As EvaluationRecord In validationHistory
				data.Add(er.evaluation(param))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a validation evaluation on a given parameter
		''' 
		''' Only works if there is only one evaluation for param.
		''' </summary>
		Public Overridable Function validationEval(ByVal param As SDVariable) As IList(Of IEvaluation)
			Return validationEval(param.name())
		End Function

		''' <summary>
		''' Get the results of a validation evaluation on a given parameter at a given index
		''' 
		''' Note that it returns all recorded evaluations.
		''' Index determines the evaluation used not the epoch's results to return.
		''' </summary>
		Public Overridable Function validationEval(ByVal param As String, ByVal index As Integer) As IList(Of IEvaluation)
			Dim data As IList(Of IEvaluation) = New List(Of IEvaluation)()
			For Each er As EvaluationRecord In validationHistory
				data.Add(er.evaluation(param, index))
			Next er

			Return data
		End Function

		''' <summary>
		''' Get the results of a validation evaluation on a given parameter at a given index
		''' 
		''' Note that it returns all recorded evaluations.
		''' Index determines the evaluation used not the epoch's results to return.
		''' </summary>
		Public Overridable Function validationEval(ByVal param As SDVariable, ByVal index As Integer) As IList(Of IEvaluation)
			Return validationEval(param.name(), index)
		End Function

		''' <summary>
		''' Gets the training evaluations ran during the last epoch
		''' </summary>
		Public Overridable Function finalTrainingEvaluations() As EvaluationRecord
			Preconditions.checkState(trainingHistory.Count > 0, "Cannot get final training evaluation - history is empty")
			Return trainingHistory(trainingHistory.Count - 1)
		End Function

		''' <summary>
		''' Gets the validation evaluations ran during the last epoch
		''' </summary>
		Public Overridable Function finalValidationEvaluations() As EvaluationRecord
			Preconditions.checkState(validationHistory.Count > 0, "Cannot get final validation evaluation - history is empty")
			Return validationHistory(validationHistory.Count - 1)
		End Function

		''' <summary>
		''' Gets the evaluation record for a given epoch. </summary>
		''' <param name="epoch"> The epoch to get results for.  If negative, returns results for the epoch that many epochs from the end. </param>
		Public Overridable Function trainingEvaluations(ByVal epoch As Integer) As EvaluationRecord
			If epoch >= 0 Then
				Return trainingHistory(epoch)
			Else
				Return trainingHistory(trainingHistory.Count - epoch)
			End If
		End Function

		''' <summary>
		''' Gets the evaluation record for a given epoch. </summary>
		''' <param name="epoch"> The epoch to get results for.  If negative, returns results for the epoch that many epochs from the end. </param>
		Public Overridable Function validationEvaluations(ByVal epoch As Integer) As EvaluationRecord
			If epoch >= 0 Then
				Return trainingHistory(epoch)
			Else
				Return validationHistory(validationHistory.Count - epoch)
			End If
		End Function

	End Class

End Namespace