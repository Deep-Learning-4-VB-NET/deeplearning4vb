Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports EvaluationRecord = org.nd4j.autodiff.listeners.records.EvaluationRecord
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
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

Namespace org.nd4j.autodiff.samediff.config

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public class EvaluationConfig
	Public Class EvaluationConfig
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.Map<String, java.util.List<org.nd4j.evaluation.IEvaluation>> evaluations = new java.util.HashMap<>();
		Private evaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.Map<String, Integer> labelIndices = new java.util.HashMap<>();
		Private labelIndices As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

'JAVA TO VB CONVERTER NOTE: The field data was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private data_Conflict As MultiDataSetIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.List<org.nd4j.autodiff.listeners.Listener> listeners = new java.util.ArrayList<>();
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listeners_Conflict As IList(Of Listener) = New List(Of Listener)()

		Private singleInput As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.NONE) private org.nd4j.autodiff.samediff.SameDiff sd;
		Private sd As SameDiff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig(@NonNull SameDiff sd)
		Public Sub New(ByVal sd As SameDiff)
			Me.sd = sd
		End Sub

		''' <summary>
		''' Add evaluations to be preformed on a specified variable, and set that variable's label index.
		''' 
		''' Setting a label index is required if using a MultiDataSetIterator.
		''' </summary>
		''' <param name="param">     The param to evaluate </param>
		''' <param name="labelIndex"> The label index of that parameter </param>
		''' <param name="evaluations"> The evaluations to preform </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig evaluate(@NonNull String param, int labelIndex, @NonNull IEvaluation... evaluations)
		Public Overridable Function evaluate(ByVal param As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As EvaluationConfig
			Return evaluate(param, evaluations).labelIndex(param, labelIndex)
		End Function

		''' <summary>
		''' See <seealso cref="evaluate(String, Integer, IEvaluation[])"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig evaluate(@NonNull SDVariable variable, int labelIndex, @NonNull IEvaluation... evaluations)
		Public Overridable Function evaluate(ByVal variable As SDVariable, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As EvaluationConfig
			Return evaluate(variable.name(), labelIndex, evaluations)
		End Function

		''' <summary>
		''' Add evaluations to be preformed on a specified variable, without setting a label index.
		''' 
		''' Setting a label index (which is not done here) is required if using a MultiDataSetIterator.
		''' </summary>
		''' <param name="param">     The param to evaluate </param>
		''' <param name="evaluations"> The evaluations to preform </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig evaluate(@NonNull String param, @NonNull IEvaluation... evaluations)
		Public Overridable Function evaluate(ByVal param As String, ParamArray ByVal evaluations() As IEvaluation) As EvaluationConfig
			If Me.evaluations(param) Is Nothing Then
				Me.evaluations(param) = New List(Of IEvaluation)()
			End If

			CType(Me.evaluations(param), List(Of IEvaluation)).AddRange(New List(Of IEvaluation) From {evaluations})
			Return Me
		End Function


		''' <summary>
		''' See <seealso cref="evaluate(String, IEvaluation[])"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig evaluate(@NonNull SDVariable variable, @NonNull IEvaluation... evaluations)
		Public Overridable Function evaluate(ByVal variable As SDVariable, ParamArray ByVal evaluations() As IEvaluation) As EvaluationConfig
			Return evaluate(variable.name(), evaluations)
		End Function

		''' <summary>
		''' Set the label index for a parameter
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig labelIndex(@NonNull String param, int labelIndex)
'JAVA TO VB CONVERTER NOTE: The parameter labelIndex was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function labelIndex(ByVal param As String, ByVal labelIndex_Conflict As Integer) As EvaluationConfig
			If Me.labelIndices(param) <> Nothing Then
				Dim existingIndex As Integer = Me.labelIndices(param)
				Preconditions.checkArgument(existingIndex = labelIndex_Conflict, "Different label index already specified for param %s.  Already specified: %s, given: %s", param, existingIndex, labelIndex_Conflict)
			End If

			labelIndices(param) = labelIndex_Conflict

			Return Me
		End Function

		''' <summary>
		''' See <seealso cref="labelIndex(String, Integer)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig labelIndex(@NonNull SDVariable variable, int labelIndex)
'JAVA TO VB CONVERTER NOTE: The parameter labelIndex was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function labelIndex(ByVal variable As SDVariable, ByVal labelIndex_Conflict As Integer) As EvaluationConfig
			Return labelIndex(variable.name(), labelIndex_Conflict)
		End Function

		''' <summary>
		''' Add listeners for this operation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig listeners(@NonNull Listener... listeners)
'JAVA TO VB CONVERTER NOTE: The parameter listeners was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function listeners(ParamArray ByVal listeners_Conflict() As Listener) As EvaluationConfig
			CType(Me.listeners_Conflict, List(Of Listener)).AddRange(New List(Of Listener) From {listeners_Conflict})
			Return Me
		End Function

		''' <summary>
		''' Set the data to evaluate on.
		''' 
		''' Setting a label index for each variable to evaluate is required
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig data(@NonNull MultiDataSetIterator data)
'JAVA TO VB CONVERTER NOTE: The parameter data was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function data(ByVal data_Conflict As MultiDataSetIterator) As EvaluationConfig
			Me.data_Conflict = data_Conflict
			singleInput = False
			Return Me
		End Function

		''' <summary>
		''' Set the data to evaluate on.
		''' 
		''' Setting a label index for each variable to evaluate is NOT required (since there is only one input)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationConfig data(@NonNull DataSetIterator data)
'JAVA TO VB CONVERTER NOTE: The parameter data was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function data(ByVal data_Conflict As DataSetIterator) As EvaluationConfig
			Me.data_Conflict = New MultiDataSetIteratorAdapter(data_Conflict)
			singleInput = True
			Return Me
		End Function

		Private Sub validateConfig()
			Preconditions.checkNotNull(data_Conflict, "Must specify data.  It may not be null.")

			If Not singleInput Then
				For Each param As String In Me.evaluations.Keys
					Preconditions.checkState(labelIndices.ContainsKey(param), "Using multiple input dataset iterator without specifying a label index for %s", param)
				Next param
			End If

			For Each param As String In Me.evaluations.Keys
				Preconditions.checkState(sd.variableMap().ContainsKey(param), "Parameter %s not present in this SameDiff graph", param)
			Next param
		End Sub

		''' <summary>
		''' Run the evaluation.
		''' 
		''' Note that the evaluations in the returned <seealso cref="EvaluationRecord"/> are the evaluations set using <seealso cref="evaluate(String, Integer, IEvaluation[])"/>,
		''' it does not matter which you use to access results.
		''' </summary>
		''' <returns> The specified listeners, in an <seealso cref="EvaluationRecord"/> for easy access. </returns>
		Public Overridable Function exec() As EvaluationRecord
			validateConfig()

			If singleInput Then
				For Each param As String In Me.evaluations.Keys
					labelIndices(param) = 0
				Next param
			End If

			sd.evaluate(data_Conflict, evaluations, labelIndices, CType(listeners_Conflict, List(Of Listener)).ToArray())
			Return New EvaluationRecord(evaluations)
		End Function

	End Class

End Namespace