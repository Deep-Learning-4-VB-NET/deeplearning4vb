Imports System
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports EvaluationAveraging = org.nd4j.evaluation.EvaluationAveraging
Imports org.nd4j.evaluation
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

Namespace org.deeplearning4j.eval


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Deprecated public class Evaluation extends org.nd4j.evaluation.classification.Evaluation implements org.deeplearning4j.eval.IEvaluation<org.nd4j.evaluation.classification.Evaluation>
	<Obsolete>
	Public Class Evaluation
		Inherits org.nd4j.evaluation.classification.Evaluation
		Implements org.deeplearning4j.eval.IEvaluation(Of org.nd4j.evaluation.classification.Evaluation)

		<Obsolete>
		Public NotInheritable Class Metric
			Public Shared ReadOnly ACCURACY As New Metric("ACCURACY", InnerEnum.ACCURACY)
			Public Shared ReadOnly F1 As New Metric("F1", InnerEnum.F1)
			Public Shared ReadOnly PRECISION As New Metric("PRECISION", InnerEnum.PRECISION)
			Public Shared ReadOnly RECALL As New Metric("RECALL", InnerEnum.RECALL)
			Public Shared ReadOnly GMEASURE As New Metric("GMEASURE", InnerEnum.GMEASURE)
			Public Shared ReadOnly MCC As New Metric("MCC", InnerEnum.MCC)

			Private Shared ReadOnly valueList As New List(Of Metric)()

			Shared Sub New()
				valueList.Add(ACCURACY)
				valueList.Add(F1)
				valueList.Add(PRECISION)
				valueList.Add(RECALL)
				valueList.Add(GMEASURE)
				valueList.Add(MCC)
			End Sub

			Public Enum InnerEnum
				ACCURACY
				F1
				PRECISION
				RECALL
				GMEASURE
				MCC
			End Enum

			Public ReadOnly innerEnumValue As InnerEnum
			Private ReadOnly nameValue As String
			Private ReadOnly ordinalValue As Integer
			Private Shared nextOrdinal As Integer = 0

			Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
				nameValue = name
				ordinalValue = nextOrdinal
				nextOrdinal += 1
				innerEnumValue = thisInnerEnumValue
			End Sub
			Public Function toNd4j() As org.nd4j.evaluation.classification.Evaluation.Metric
				Select Case Me
					Case ACCURACY
						Return org.nd4j.evaluation.classification.Evaluation.Metric.ACCURACY
					Case F1
						Return org.nd4j.evaluation.classification.Evaluation.Metric.F1
					Case PRECISION
						Return org.nd4j.evaluation.classification.Evaluation.Metric.PRECISION
					Case RECALL
						Return org.nd4j.evaluation.classification.Evaluation.Metric.RECALL
					Case GMEASURE
						Return org.nd4j.evaluation.classification.Evaluation.Metric.GMEASURE
					Case MCC
						Return org.nd4j.evaluation.classification.Evaluation.Metric.MCC
					Case Else
						Throw New System.InvalidOperationException("Unknown enum state: " & Me)
				End Select
			End Function

			Public Shared Function values() As Metric()
				Return valueList.ToArray()
			End Function

			Public Function ordinal() As Integer
				Return ordinalValue
			End Function

			Public Overrides Function ToString() As String
				Return nameValue
			End Function

			Public Shared Operator =(ByVal one As Metric, ByVal two As Metric) As Boolean
				Return one.innerEnumValue = two.innerEnumValue
			End Operator

			Public Shared Operator <>(ByVal one As Metric, ByVal two As Metric) As Boolean
				Return one.innerEnumValue <> two.innerEnumValue
			End Operator

			Public Shared Function valueOf(ByVal name As String) As Metric
				For Each enumInstance As Metric In Metric.valueList
					If enumInstance.nameValue = name Then
						Return enumInstance
					End If
				Next
				Throw New System.ArgumentException(name)
			End Function
		End Class

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New()
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal numClasses As Integer)
			MyBase.New(numClasses)
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal numClasses As Integer, ByVal binaryPositiveClass As Integer?)
			MyBase.New(numClasses, binaryPositiveClass)
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal labels As IList(Of String))
			MyBase.New(labels)
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal labels As IDictionary(Of Integer, String))
			MyBase.New(labels)
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal labels As IList(Of String), ByVal topN As Integer)
			MyBase.New(labels, topN)
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal binaryDecisionThreshold As Double)
			MyBase.New(binaryDecisionThreshold)
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>") public Evaluation(double binaryDecisionThreshold, @NonNull Integer binaryPositiveClass)
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal binaryDecisionThreshold As Double, ByVal binaryPositiveClass As Integer)
			MyBase.New(binaryDecisionThreshold, binaryPositiveClass)
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal costArray As INDArray)
			MyBase.New(costArray)
		End Sub

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Sub New(ByVal labels As IList(Of String), ByVal costArray As INDArray)
			MyBase.New(labels, costArray)
		End Sub

		<Obsolete>
		Public Overridable Overloads Function precision(ByVal averaging As org.deeplearning4j.eval.EvaluationAveraging) As Double
			Return precision(averaging.toNd4j())
		End Function

		<Obsolete>
		Public Overridable Overloads Function recall(ByVal averaging As org.deeplearning4j.eval.EvaluationAveraging) As Double
			Return recall(averaging.toNd4j())
		End Function

		Public Overridable Overloads Function falsePositiveRate(ByVal averaging As org.deeplearning4j.eval.EvaluationAveraging) As Double
			Return falsePositiveRate(averaging.toNd4j())
		End Function

		Public Overridable Overloads Function falseNegativeRate(ByVal averaging As org.deeplearning4j.eval.EvaluationAveraging) As Double
			Return falseNegativeRate(averaging.toNd4j())
		End Function

		Public Overridable Overloads Function f1(ByVal averaging As org.deeplearning4j.eval.EvaluationAveraging) As Double
			Return f1(averaging.toNd4j())
		End Function

		Public Overridable Overloads Function fBeta(ByVal beta As Double, ByVal averaging As org.deeplearning4j.eval.EvaluationAveraging) As Double
			Return fBeta(beta, averaging.toNd4j())
		End Function

		Public Overridable Overloads Function gMeasure(ByVal averaging As org.deeplearning4j.eval.EvaluationAveraging) As Double
			Return gMeasure(averaging.toNd4j())
		End Function

		Public Overridable Overloads Function matthewsCorrelation(ByVal averaging As org.deeplearning4j.eval.EvaluationAveraging) As Double
			Return matthewsCorrelation(averaging.toNd4j())
		End Function

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		Public Overridable Overloads Function scoreForMetric(ByVal metric As Metric) As Double
			Return scoreForMetric(metric.toNd4j())
		End Function

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Shared Function fromJson(ByVal json As String) As Evaluation
			Return fromJson(json, GetType(Evaluation))
		End Function

		''' @deprecated Use ND4J Evaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.classification.Evaluation.Metric"/> 
		<Obsolete("Use ND4J Evaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.classification.Evaluation.Metric""/>")>
		Public Shared Function fromYaml(ByVal yaml As String) As Evaluation
			Return fromYaml(yaml, GetType(Evaluation))
		End Function
	End Class

End Namespace