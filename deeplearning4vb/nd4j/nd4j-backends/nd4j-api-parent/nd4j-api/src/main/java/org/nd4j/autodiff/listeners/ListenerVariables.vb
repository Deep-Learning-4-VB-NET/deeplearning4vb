Imports System.Collections.Generic
Imports Sets = org.nd4j.shade.guava.collect.Sets
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports RequiredArgsConstructor = lombok.RequiredArgsConstructor
Imports Setter = lombok.Setter
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @RequiredArgsConstructor @Getter public class ListenerVariables
	Public Class ListenerVariables

		Public Shared Function empty() As ListenerVariables
			Return ListenerVariables.builder().build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.@Set<String> trainingVariables;
'JAVA TO VB CONVERTER NOTE: The field trainingVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private trainingVariables_Conflict As ISet(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.@Set<String> validationVariables;
'JAVA TO VB CONVERTER NOTE: The field validationVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private validationVariables_Conflict As ISet(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.@Set<String> evaluationVariables;
'JAVA TO VB CONVERTER NOTE: The field evaluationVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private evaluationVariables_Conflict As ISet(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.@Set<String> inferenceVariables;
'JAVA TO VB CONVERTER NOTE: The field inferenceVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inferenceVariables_Conflict As ISet(Of String)

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		''' <summary>
		''' Get required training variables
		''' </summary>
		Public Overridable Function trainingVariables() As ISet(Of String)
			Return trainingVariables_Conflict
		End Function

		''' <summary>
		''' Get required validation variables
		''' </summary>
		Public Overridable Function validationVariables() As ISet(Of String)
			Return validationVariables_Conflict
		End Function

		''' <summary>
		''' Get required evaluation variables
		''' </summary>
		Public Overridable Function evaluationVariables() As ISet(Of String)
			Return evaluationVariables_Conflict
		End Function

		''' <summary>
		''' Get required inference variables
		''' </summary>
		Public Overridable Function inferenceVariables() As ISet(Of String)
			Return inferenceVariables_Conflict
		End Function

		''' <summary>
		''' Get required variables for specified op
		''' </summary>
		Public Overridable Function requiredVariables(ByVal op As Operation) As ISet(Of String)
			Select Case op.innerEnumValue
				Case org.nd4j.autodiff.listeners.Operation.InnerEnum.TRAINING
					Return trainingVariables_Conflict
				Case org.nd4j.autodiff.listeners.Operation.InnerEnum.TRAINING_VALIDATION
					Return validationVariables_Conflict
				Case org.nd4j.autodiff.listeners.Operation.InnerEnum.INFERENCE
					Return inferenceVariables_Conflict
				Case org.nd4j.autodiff.listeners.Operation.InnerEnum.EVALUATION
					Return evaluationVariables_Conflict
			End Select
			Throw New System.ArgumentException("Unknown operation " & op)
		End Function

		Private Sub New()

		End Sub

		''' <summary>
		''' Return a new ListenerVariables that contains the variables of this ListenerVariables and of other
		''' </summary>
		Public Overridable Function merge(ByVal other As ListenerVariables) As ListenerVariables
			Return New ListenerVariables(Sets.newHashSet(Sets.union(trainingVariables_Conflict, other.trainingVariables_Conflict)), Sets.newHashSet(Sets.union(validationVariables_Conflict, other.validationVariables_Conflict)), Sets.newHashSet(Sets.union(evaluationVariables_Conflict, other.evaluationVariables_Conflict)), Sets.newHashSet(Sets.union(inferenceVariables_Conflict, other.inferenceVariables_Conflict)))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder
		Public Class Builder
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.@Set<String> trainingVariables = new java.util.HashSet<>();
'JAVA TO VB CONVERTER NOTE: The field trainingVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend trainingVariables_Conflict As ISet(Of String) = New HashSet(Of String)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.@Set<String> validationVariables = new java.util.HashSet<>();
'JAVA TO VB CONVERTER NOTE: The field validationVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend validationVariables_Conflict As ISet(Of String) = New HashSet(Of String)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.@Set<String> evaluationVariables = new java.util.HashSet<>();
'JAVA TO VB CONVERTER NOTE: The field evaluationVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend evaluationVariables_Conflict As ISet(Of String) = New HashSet(Of String)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.@Set<String> inferenceVariables = new java.util.HashSet<>();
'JAVA TO VB CONVERTER NOTE: The field inferenceVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inferenceVariables_Conflict As ISet(Of String) = New HashSet(Of String)()

			''' <summary>
			''' Add required variables for the specified op
			''' </summary>
			''' <param name="op"> The op to require the variable for </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder requireVariables(@NonNull Operation op, @NonNull String... variables)
			Public Overridable Function requireVariables(ByVal op As Operation, ParamArray ByVal variables() As String) As Builder
				Select Case op.innerEnumValue
					Case org.nd4j.autodiff.listeners.Operation.InnerEnum.TRAINING
						trainingVariables_Conflict.addAll(Arrays.asList(variables))
					Case org.nd4j.autodiff.listeners.Operation.InnerEnum.TRAINING_VALIDATION
						validationVariables_Conflict.addAll(Arrays.asList(variables))
					Case org.nd4j.autodiff.listeners.Operation.InnerEnum.INFERENCE
						inferenceVariables_Conflict.addAll(Arrays.asList(variables))
					Case org.nd4j.autodiff.listeners.Operation.InnerEnum.EVALUATION
						evaluationVariables_Conflict.addAll(Arrays.asList(variables))
				End Select

				Return Me
			End Function

			''' <summary>
			''' Add required variables for the specified op
			''' </summary>
			''' <param name="op"> The op to require the variable for </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder requireVariables(@NonNull Operation op, @NonNull SDVariable... variables)
			Public Overridable Function requireVariables(ByVal op As Operation, ParamArray ByVal variables() As SDVariable) As Builder
				Dim names(variables.Length - 1) As String

				For i As Integer = 0 To variables.Length - 1
					names(i) = variables(i).name()
				Next i

				Return requireVariables(op, names)
			End Function

			''' <summary>
			''' Add required variables for training
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainingVariables(@NonNull String... variables)
			Public Overridable Function trainingVariables(ParamArray ByVal variables() As String) As Builder
				Return requireVariables(Operation.TRAINING, variables)
			End Function

			''' <summary>
			''' Add required variables for training
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainingVariables(@NonNull SDVariable... variables)
			Public Overridable Function trainingVariables(ParamArray ByVal variables() As SDVariable) As Builder
				Return requireVariables(Operation.TRAINING, variables)
			End Function

			''' <summary>
			''' Add required variables for validation
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder validationVariables(@NonNull String... variables)
			Public Overridable Function validationVariables(ParamArray ByVal variables() As String) As Builder
				Return requireVariables(Operation.TRAINING_VALIDATION, variables)
			End Function

			''' <summary>
			''' Add required variables for validation
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder validationVariables(@NonNull SDVariable... variables)
			Public Overridable Function validationVariables(ParamArray ByVal variables() As SDVariable) As Builder
				Return requireVariables(Operation.TRAINING_VALIDATION, variables)
			End Function

			''' <summary>
			''' Add required variables for inference
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder inferenceVariables(@NonNull String... variables)
			Public Overridable Function inferenceVariables(ParamArray ByVal variables() As String) As Builder
				Return requireVariables(Operation.INFERENCE, variables)
			End Function

			''' <summary>
			''' Add required variables for inference
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder inferenceVariables(@NonNull SDVariable... variables)
			Public Overridable Function inferenceVariables(ParamArray ByVal variables() As SDVariable) As Builder
				Return requireVariables(Operation.INFERENCE, variables)
			End Function

			''' <summary>
			''' Add required variables for evaluation
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder evaluationVariables(@NonNull String... variables)
			Public Overridable Function evaluationVariables(ParamArray ByVal variables() As String) As Builder
				Return requireVariables(Operation.EVALUATION, variables)
			End Function

			''' <summary>
			''' Add required variables for evaluation
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder evaluationVariables(@NonNull SDVariable... variables)
			Public Overridable Function evaluationVariables(ParamArray ByVal variables() As SDVariable) As Builder
				Return requireVariables(Operation.EVALUATION, variables)
			End Function

			Public Overridable Function build() As ListenerVariables
				Return New ListenerVariables(trainingVariables_Conflict, validationVariables_Conflict, evaluationVariables_Conflict, inferenceVariables_Conflict)
			End Function
		End Class

	End Class

End Namespace