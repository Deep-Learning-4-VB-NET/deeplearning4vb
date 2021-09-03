Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class ListenerEvaluations
	Public Class ListenerEvaluations
'JAVA TO VB CONVERTER NOTE: The field trainEvaluations was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private trainEvaluations_Conflict As IDictionary(Of String, IList(Of IEvaluation))
'JAVA TO VB CONVERTER NOTE: The field trainEvaluationLabels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private trainEvaluationLabels_Conflict As IDictionary(Of String, Integer)

'JAVA TO VB CONVERTER NOTE: The field validationEvaluations was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private validationEvaluations_Conflict As IDictionary(Of String, IList(Of IEvaluation))
'JAVA TO VB CONVERTER NOTE: The field validationEvaluationLabels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private validationEvaluationLabels_Conflict As IDictionary(Of String, Integer)

		Public Sub New(ByVal trainEvaluations As IDictionary(Of String, IList(Of IEvaluation)), ByVal trainEvaluationLabels As IDictionary(Of String, Integer), ByVal validationEvaluations As IDictionary(Of String, IList(Of IEvaluation)), ByVal validationEvaluationLabels As IDictionary(Of String, Integer))
			Me.trainEvaluations_Conflict = trainEvaluations
			Me.trainEvaluationLabels_Conflict = trainEvaluationLabels
			Me.validationEvaluations_Conflict = validationEvaluations
			Me.validationEvaluationLabels_Conflict = validationEvaluationLabels

			Preconditions.checkArgument(trainEvaluations.Keys.Equals(trainEvaluationLabels.Keys), "Must specify a label index for each train evaluation.  Expected: %s, got: %s", trainEvaluations.Keys, trainEvaluationLabels.Keys)

			Preconditions.checkArgument(validationEvaluations.Keys.Equals(validationEvaluationLabels.Keys), "Must specify a label index for each validation evaluation.  Expected: %s, got: %s", validationEvaluations.Keys, validationEvaluationLabels.Keys)
		End Sub

		Private Sub New()

		End Sub

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		''' <summary>
		''' Get the requested training evaluations
		''' </summary>
		Public Overridable Function trainEvaluations() As IDictionary(Of String, IList(Of IEvaluation))
			Return trainEvaluations_Conflict
		End Function

		''' <summary>
		''' Get the label indices for the requested training evaluations
		''' </summary>
		Public Overridable Function trainEvaluationLabels() As IDictionary(Of String, Integer)
			Return trainEvaluationLabels_Conflict
		End Function

		''' <summary>
		''' Get the requested validation evaluations
		''' </summary>
		Public Overridable Function validationEvaluations() As IDictionary(Of String, IList(Of IEvaluation))
			Return validationEvaluations_Conflict
		End Function

		''' <summary>
		''' Get the label indices for the requested validation evaluations
		''' </summary>
		Public Overridable Function validationEvaluationLabels() As IDictionary(Of String, Integer)
			Return validationEvaluationLabels_Conflict
		End Function

		''' <summary>
		''' Get the required variables for these evaluations
		''' </summary>
		Public Overridable Function requiredVariables() As ListenerVariables
			Return New ListenerVariables(trainEvaluations_Conflict.Keys, validationEvaluations_Conflict.Keys, New HashSet(Of String)(), New HashSet(Of String)())
		End Function

		''' <returns> true if there are no requested evaluations </returns>
		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return trainEvaluations_Conflict.Count = 0 AndAlso validationEvaluations_Conflict.Count = 0
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder
		Public Class Builder
			Friend trainEvaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
			Friend trainEvaluationLabels As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

			Friend validationEvaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
			Friend validationEvaluationLabels As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private void addEvaluations(boolean validation, @NonNull Map<String, java.util.List<org.nd4j.evaluation.IEvaluation>> evaluationMap, @NonNull Map<String, Integer> labelMap, @NonNull String variableName, int labelIndex, @NonNull IEvaluation... evaluations)
			Friend Overridable Sub addEvaluations(ByVal validation As Boolean, ByVal evaluationMap As IDictionary(Of String, IList(Of IEvaluation)), ByVal labelMap As IDictionary(Of String, Integer), ByVal variableName As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation)
				If evaluationMap.containsKey(variableName) AndAlso labelMap.get(variableName) <> labelIndex Then
					Dim s As String

					If validation Then
						s = "This ListenerEvaluations.Builder already has validation evaluations for "
					Else
						s = "This ListenerEvaluations.Builder already has train evaluations for "
					End If

					Throw New System.ArgumentException(s & "variable " & variableName & " with label index " & labelIndex & ".  You can't add " & " evaluations with a different label index.  Got label index " & labelIndex)
				End If

				If evaluationMap.containsKey(variableName) Then
					evaluationMap.get(variableName).addAll(Arrays.asList(evaluations))
				Else
					evaluationMap.put(variableName, Arrays.asList(evaluations))
					labelMap.put(variableName, labelIndex)
				End If
			End Sub

			''' <summary>
			''' Add requested training evaluations for a parm/variable
			''' </summary>
			''' <param name="variableName"> The variable to evaluate </param>
			''' <param name="labelIndex">   The index of the label to evaluate against </param>
			''' <param name="evaluations">  The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainEvaluation(@NonNull String variableName, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function trainEvaluation(ByVal variableName As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				addEvaluations(False, Me.trainEvaluations, Me.trainEvaluationLabels, variableName, labelIndex, evaluations)
				Return Me
			End Function

			''' <summary>
			''' Add requested training evaluations for a parm/variable
			''' </summary>
			''' <param name="variable">    The variable to evaluate </param>
			''' <param name="labelIndex">  The index of the label to evaluate against </param>
			''' <param name="evaluations"> The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainEvaluation(@NonNull SDVariable variable, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function trainEvaluation(ByVal variable As SDVariable, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				Return trainEvaluation(variable.name(), labelIndex, evaluations)
			End Function

			''' <summary>
			''' Add requested validation evaluations for a parm/variable
			''' </summary>
			''' <param name="variableName"> The variable to evaluate </param>
			''' <param name="labelIndex">   The index of the label to evaluate against </param>
			''' <param name="evaluations">  The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder validationEvaluation(@NonNull String variableName, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function validationEvaluation(ByVal variableName As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				addEvaluations(True, Me.validationEvaluations, Me.validationEvaluationLabels, variableName, labelIndex, evaluations)
				Return Me
			End Function

			''' <summary>
			''' Add requested validation evaluations for a parm/variable
			''' </summary>
			''' <param name="variable">    The variable to evaluate </param>
			''' <param name="labelIndex">  The index of the label to evaluate against </param>
			''' <param name="evaluations"> The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder validationEvaluation(@NonNull SDVariable variable, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function validationEvaluation(ByVal variable As SDVariable, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				Return validationEvaluation(variable.name(), labelIndex, evaluations)
			End Function

			''' <summary>
			''' Add requested evaluations for a parm/variable, for either training or validation
			''' </summary>
			''' <param name="validation">   Whether to add these evaluations as validation or training </param>
			''' <param name="variableName"> The variable to evaluate </param>
			''' <param name="labelIndex">   The index of the label to evaluate against </param>
			''' <param name="evaluations">  The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addEvaluations(boolean validation, @NonNull String variableName, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function addEvaluations(ByVal validation As Boolean, ByVal variableName As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				If validation Then
					Return validationEvaluation(variableName, labelIndex, evaluations)
				Else
					Return trainEvaluation(variableName, labelIndex, evaluations)
				End If
			End Function

			Public Overridable Function build() As ListenerEvaluations
				Return New ListenerEvaluations(trainEvaluations, trainEvaluationLabels, validationEvaluations, validationEvaluationLabels)
			End Function
		End Class
	End Class

End Namespace