Imports System
Imports System.Collections.Generic
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports RequiredArgsConstructor = lombok.RequiredArgsConstructor
Imports org.nd4j.evaluation
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.evaluation.custom

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class CustomEvaluation<T> extends org.nd4j.evaluation.BaseEvaluation<CustomEvaluation>
	Public Class CustomEvaluation(Of T)
		Inherits BaseEvaluation(Of CustomEvaluation)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @RequiredArgsConstructor public static class Metric<T> implements org.nd4j.evaluation.IMetric
		Public Class Metric(Of T)
			Implements IMetric

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @NonNull private ResultLambda<T> getResult;
			Friend getResult As ResultLambda(Of T)

'JAVA TO VB CONVERTER NOTE: The field minimize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minimize_Conflict As Boolean = False

			Public Overridable ReadOnly Property EvaluationClass As Type Implements IMetric.getEvaluationClass
				Get
					Return GetType(CustomEvaluation)
				End Get
			End Property

			Public Overridable Function minimize() As Boolean Implements IMetric.minimize
				Return minimize_Conflict
			End Function

			''' <summary>
			''' A metric that takes the average of a list of doubles
			''' </summary>
			Public Shared Function doubleAverage(ByVal minimize As Boolean) As Metric(Of Double)
				Return New Metric(Of Double)(New ResultLambdaAnonymousInnerClass()
			   , minimize)
			End Function

			Private Class ResultLambdaAnonymousInnerClass
				Implements ResultLambda(Of Double)

				Public Function toResult(ByVal data As IList(Of Double)) As Double Implements ResultLambda(Of Double).toResult
					Dim count As Integer = 0
					Dim sum As Double = 0
					For Each d As Double? In data
						count += 1
						sum += d
					Next d
					Return sum / count
				End Function
			End Class


			''' <summary>
			''' A metric that takes the max of a list of doubles
			''' </summary>
			Public Shared Function doubleMax(ByVal minimize As Boolean) As Metric(Of Double)
				Return New Metric(Of Double)(New ResultLambdaAnonymousInnerClass2()
			   , minimize)
			End Function

			Private Class ResultLambdaAnonymousInnerClass2
				Implements ResultLambda(Of Double)

				Public Function toResult(ByVal data As IList(Of Double)) As Double Implements ResultLambda(Of Double).toResult
					Dim max As Double = 0
					For Each d As Double? In data
						If d > max Then
							max = d
						End If
					Next d
					Return max
				End Function
			End Class


			''' <summary>
			''' A metric that takes the min of a list of doubles
			''' </summary>
			Public Shared Function doubleMin(ByVal minimize As Boolean) As Metric(Of Double)
				Return New Metric(Of Double)(New ResultLambdaAnonymousInnerClass3()
			   , minimize)
			End Function

			Private Class ResultLambdaAnonymousInnerClass3
				Implements ResultLambda(Of Double)

				Public Function toResult(ByVal data As IList(Of Double)) As Double Implements ResultLambda(Of Double).toResult
					Dim max As Double = 0
					For Each d As Double? In data
						If d < max Then
							max = d
						End If
					Next d
					Return max
				End Function
			End Class
		End Class

		''' <summary>
		''' A MergeLambda that merges by concatenating the two lists
		''' </summary>
		Public Shared Function mergeConcatenate(Of R)() As MergeLambda(Of R)
			Return New MergeLambdaAnonymousInnerClass()
		End Function

		Private Class MergeLambdaAnonymousInnerClass
			Implements MergeLambda(Of R)

			Public Function merge(ByVal a As IList(Of R), ByVal b As IList(Of R)) As IList(Of R) Implements MergeLambda(Of R).merge
				Dim res As IList(Of R) = Lists.newArrayList(a)
				CType(res, List(Of R)).AddRange(b)
				Return res
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private EvaluationLambda<T> evaluationLambda;
		Private evaluationLambda As EvaluationLambda(Of T)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private MergeLambda<T> mergeLambda;
		Private mergeLambda As MergeLambda(Of T)

		Private evaluations As IList(Of T) = New List(Of T)()

		Public Overrides Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray, ByVal recordMetaData As IList(Of T1))
			evaluations.Add(evaluationLambda.eval(labels, networkPredictions, maskArray, recordMetaData))
		End Sub

		Public Overrides Sub merge(ByVal other As CustomEvaluation)
			evaluations = mergeLambda.merge(evaluations, other.evaluations)
		End Sub

		Public Overrides Sub reset()
			evaluations = New List(Of T)()
		End Sub

		Public Overrides Function stats() As String
			Return ""
		End Function

		Public Overrides Function getValue(ByVal metric As IMetric) As Double
			If TypeOf metric Is Metric Then
				Return DirectCast(metric, Metric(Of T)).getGetResult().toResult(evaluations)
			Else
				Throw New System.InvalidOperationException("Can't get value for non-regression Metric " & metric)
			End If
		End Function

		Public Overrides Function newInstance() As CustomEvaluation(Of T)
			Return New CustomEvaluation(Of T)(evaluationLambda, mergeLambda)
		End Function
	End Class

End Namespace