Imports System
Imports System.Collections.Generic
Imports Predicates = org.nd4j.shade.guava.base.Predicates
Imports Collections2 = org.nd4j.shade.guava.collect.Collections2
Imports ImmutableMap = org.nd4j.shade.guava.collect.ImmutableMap
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports Getter = lombok.Getter
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric

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
'ORIGINAL LINE: @Getter public class EvaluationRecord
	Public Class EvaluationRecord

'JAVA TO VB CONVERTER NOTE: The field evaluations was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private evaluations_Conflict As IDictionary(Of String, IList(Of IEvaluation))
		Private classEvaluations As IDictionary(Of Type, IEvaluation) = New Dictionary(Of Type, IEvaluation)()
		Private isEmpty As Boolean = True

		Public Sub New(ByVal evaluations As IDictionary(Of String, IList(Of IEvaluation)))
			Me.evaluations_Conflict = Collections.unmodifiableMap(evaluations)

			For Each le As IList(Of IEvaluation) In evaluations.Values
				For Each e As IEvaluation In le
					isEmpty = False
					If classEvaluations.ContainsKey(e.GetType()) Then
						classEvaluations.Remove(e.GetType())
					Else
						classEvaluations(e.GetType()) = e
					End If
				Next e
			Next le
		End Sub

		Private Sub New()

		End Sub

		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return isEmpty
			End Get
		End Property

		''' <summary>
		''' Get all evaluations
		''' </summary>
		Public Overridable Function evaluations() As IDictionary(Of String, IList(Of IEvaluation))
			Return evaluations_Conflict
		End Function

		''' <summary>
		''' Get evaluations for a given param/variable
		''' </summary>
		''' <param name="param"> The target param/variable </param>
		Public Overridable Function evaluations(ByVal param As String) As IList(Of IEvaluation)
			Preconditions.checkArgument(evaluations_Conflict.ContainsKey(param), "No evaluations for %s.", param)

			Return evaluations(param)
		End Function

		''' <summary>
		''' Get evaluations for a given param/variable
		''' </summary>
		''' <param name="param"> The target param/variable </param>
		Public Overridable Function evaluations(ByVal param As SDVariable) As IList(Of IEvaluation)
			Return evaluations(param.name())
		End Function

		''' <summary>
		''' Get the evaluation for param at the specified index
		''' </summary>
		Public Overridable Function evaluation(ByVal param As String, ByVal index As Integer) As IEvaluation
			Return evaluations(param)(index)
		End Function

		''' <summary>
		''' Get the evaluation for param at the specified index
		''' </summary>
		Public Overridable Function evaluation(ByVal param As SDVariable, ByVal index As Integer) As IEvaluation
			Return evaluation(param.name(), index)
		End Function

		''' <summary>
		''' Get the evaluation for a given param/variable
		''' <para>
		''' Will throw an exception if there are more than one or no evaluations for the param
		''' 
		''' </para>
		''' </summary>
		''' <param name="param"> The target param/variable </param>
		Public Overridable Function evaluation(Of T As IEvaluation)(ByVal param As String) As T
			Preconditions.checkArgument(evaluations_Conflict.ContainsKey(param), "No evaluations for %s.", param)
			Preconditions.checkArgument(evaluations(param).Count = 1, "Multiple evaluations for %s.  Use evaluations().", param)

			Return CType(evaluations(param)(0), T)
		End Function

		''' <summary>
		''' Get the evaluation for a given param/variable
		''' <para>
		''' Will throw an exception if there are more than one or no evaluations for the param
		''' 
		''' </para>
		''' </summary>
		''' <param name="param"> The target param/variable </param>
		Public Overridable Function evaluation(Of T As IEvaluation)(ByVal param As SDVariable) As T
			Return evaluation(param.name())
		End Function

		''' <summary>
		''' Get the evaluation of a given type
		''' <para>
		''' Will throw an exception if there are more than one or no evaluations of that type
		''' 
		''' </para>
		''' </summary>
		''' <param name="evalClass"> The type of evaluation to look for </param>
		Public Overridable Function evaluation(Of T As IEvaluation(Of T))(ByVal evalClass As Type(Of T)) As T
			Preconditions.checkArgument(classEvaluations.ContainsKey(evalClass), "Can't get evaluation for %s.  Either no evaluations with that class are present, or more than one are.", evalClass)

			Return CType(classEvaluations(evalClass), T)
		End Function

		''' <summary>
		''' Get the evaluation of a given type, for a given param/variable
		''' <para>
		''' Will throw an exception if there are more than one or no evaluations of that type for the given param
		''' 
		''' </para>
		''' </summary>
		''' <param name="param">     The target param/variable </param>
		''' <param name="evalClass"> The type of evaluation to look for </param>
		Public Overridable Function evaluation(Of T As IEvaluation(Of T))(ByVal param As String, ByVal evalClass As Type(Of T)) As T
			Dim evals As ICollection(Of IEvaluation) = Collections2.filter(evaluations(param), Predicates.instanceOf(evalClass))

			Preconditions.checkArgument(evals.Count = 1, "Multiple or no evaluations of type %s for param %s.", evalClass, param)

			Return CType(evals.GetEnumerator().next(), T)
		End Function

		''' <summary>
		''' Get the evaluation of a given type, for a given param/variable
		''' <para>
		''' Will throw an exception if there are more than one or no evaluations of that type for the given param
		''' 
		''' </para>
		''' </summary>
		''' <param name="param">     The target param/variable </param>
		''' <param name="evalClass"> The type of evaluation to look for </param>
		Public Overridable Function evaluation(Of T As IEvaluation(Of T))(ByVal param As SDVariable, ByVal evalClass As Type(Of T)) As T
			Return evaluation(param.name(), evalClass)
		End Function

		''' <summary>
		''' Get the metric's value for the evaluation of the metric's type
		''' <para>
		''' Will throw an exception if there are more than one or no evaluations of that type
		''' 
		''' </para>
		''' </summary>
		''' <param name="metric"> The metric to calculate </param>
		Public Overridable Function getValue(ByVal metric As IMetric) As Double
			Return evaluation(metric.EvaluationClass).getValue(metric)
		End Function

		''' <summary>
		''' Get the metric's value for the evaluation of the metric's type, for a given param/variable
		''' <para>
		''' Will throw an exception if there are more than one or no evaluations of that type for the given param
		''' 
		''' </para>
		''' </summary>
		''' <param name="param">  The target param/variable </param>
		''' <param name="metric"> The metric to calculate </param>
		Public Overridable Function getValue(ByVal param As String, ByVal metric As IMetric) As Double
			Return evaluation(param, metric.EvaluationClass).getValue(metric)
		End Function

		''' <summary>
		''' Get the metric's value for the evaluation of the metric's type, for a given param/variable
		''' <para>
		''' Will throw an exception if there are more than one or no evaluations of that type for the given param
		''' 
		''' </para>
		''' </summary>
		''' <param name="param">  The target param/variable </param>
		''' <param name="metric"> The metric to calculate </param>
		Public Overridable Function getValue(ByVal param As SDVariable, ByVal metric As IMetric) As Double
			Return getValue(param.name(), metric)
		End Function

		''' <summary>
		''' Get the metric's value for the evaluation for a given param/variable at the given index
		''' <para>
		''' Will throw an exception if the target evaluation doesn't support the given metric
		''' 
		''' </para>
		''' </summary>
		''' <param name="param">  The target param/variable </param>
		''' <param name="index">  The index of the target evaluation on the param </param>
		''' <param name="metric"> The metric to calculate </param>
		Public Overridable Function getValue(ByVal param As String, ByVal index As Integer, ByVal metric As IMetric) As Double
			Return evaluation(param, index).getValue(metric)
		End Function

		''' <summary>
		''' Get the metric's value for the evaluation for a given param/variable at the given index
		''' <para>
		''' Will throw an exception if the target evaluation doesn't support the given metric
		''' 
		''' </para>
		''' </summary>
		''' <param name="param">  The target param/variable </param>
		''' <param name="index">  The index of the target evaluation on the param </param>
		''' <param name="metric"> The metric to calculate </param>
		Public Overridable Function getValue(ByVal param As SDVariable, ByVal index As Integer, ByVal metric As IMetric) As Double
			Return getValue(param.name(), index, metric)
		End Function

	End Class

End Namespace