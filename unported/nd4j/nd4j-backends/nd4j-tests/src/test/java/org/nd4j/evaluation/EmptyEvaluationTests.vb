Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports EvaluationBinary = org.nd4j.evaluation.classification.EvaluationBinary
Imports EvaluationCalibration = org.nd4j.evaluation.classification.EvaluationCalibration
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCBinary = org.nd4j.evaluation.classification.ROCBinary
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports Metric = org.nd4j.evaluation.regression.RegressionEvaluation.Metric
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assertions.fail

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

Namespace org.nd4j.evaluation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.SAMEDIFF) public class EmptyEvaluationTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class EmptyEvaluationTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyEvaluation(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testEmptyEvaluation(ByVal backend As Nd4jBackend)
			Dim e As New Evaluation()
			Console.WriteLine(e.stats())

			For Each m As Evaluation.Metric In Evaluation.Metric.values()
				Try
					e.scoreForMetric(m)
					fail("Expected exception")
				Catch t As Exception
					assertTrue(t.getMessage().contains("no evaluation has been performed"),t.getMessage())
				End Try
			Next m
		  End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyRegressionEvaluation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyRegressionEvaluation(ByVal backend As Nd4jBackend)
			Dim re As New RegressionEvaluation()
			re.stats()

			For Each m As RegressionEvaluation.Metric In RegressionEvaluation.Metric.values()
				Try
					re.scoreForMetric(m)
				Catch t As Exception
					assertTrue(t.getMessage().contains("eval must be called"),t.getMessage())
				End Try
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyEvaluationBinary(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyEvaluationBinary(ByVal backend As Nd4jBackend)
			Dim eb As New EvaluationBinary()
			eb.stats()

			For Each m As EvaluationBinary.Metric In EvaluationBinary.Metric.values()
				Try
					eb.scoreForMetric(m, 0)
					fail("Expected exception")
				Catch t As Exception
					assertTrue(t.getMessage().contains("eval must be called"),t.getMessage())
				End Try
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyROC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyROC(ByVal backend As Nd4jBackend)
			Dim roc As New ROC()
			roc.stats()

			For Each m As ROC.Metric In ROC.Metric.values()
				Try
					roc.scoreForMetric(m)
					fail("Expected exception")
				Catch t As Exception
					assertTrue(t.getMessage().contains("no evaluation"),t.getMessage())
				End Try
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyROCBinary(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyROCBinary(ByVal backend As Nd4jBackend)
			Dim rb As New ROCBinary()
			rb.stats()

			For Each m As ROCBinary.Metric In ROCBinary.Metric.values()
				Try
					rb.scoreForMetric(m, 0)
					fail("Expected exception")
				Catch t As Exception
					assertTrue(t.getMessage().contains("eval must be called"),t.getMessage())
				End Try
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyROCMultiClass(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyROCMultiClass(ByVal backend As Nd4jBackend)
			Dim r As New ROCMultiClass()
			r.stats()

			For Each m As ROCMultiClass.Metric In ROCMultiClass.Metric.values()
				Try
					r.scoreForMetric(m, 0)
					fail("Expected exception")
				Catch t As Exception
					assertTrue(t.getMessage().contains("no data"),t.getMessage())
				End Try
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEmptyEvaluationCalibration(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEmptyEvaluationCalibration(ByVal backend As Nd4jBackend)
			Dim ec As New EvaluationCalibration()
			ec.stats()

			Try
				ec.getResidualPlot(0)
				fail("Expected exception")
			Catch t As Exception
				assertTrue(t.getMessage().contains("no data"),t.getMessage())
			End Try
			Try
				ec.getProbabilityHistogram(0)
				fail("Expected exception")
			Catch t As Exception
				assertTrue(t.getMessage().contains("no data"),t.getMessage())
			End Try
			Try
				ec.getReliabilityDiagram(0)
				fail("Expected exception")
			Catch t As Exception
				assertTrue(t.getMessage().contains("no data"),t.getMessage())
			End Try
		End Sub

	End Class

End Namespace