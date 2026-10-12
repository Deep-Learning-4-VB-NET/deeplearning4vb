Imports System
Imports Microsoft.VisualBasic
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
Imports Histogram = org.nd4j.evaluation.curves.Histogram
Imports PrecisionRecallCurve = org.nd4j.evaluation.curves.PrecisionRecallCurve
Imports RocCurve = org.nd4j.evaluation.curves.RocCurve
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) @NativeTag public class EvalJsonTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class EvalJsonTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerdeEmpty(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSerdeEmpty(ByVal backend As Nd4jBackend)
			Dim print As Boolean = False

			Dim arr() As IEvaluation = {
				New Evaluation(),
				New EvaluationBinary(),
				New ROCBinary(10),
				New ROCMultiClass(10),
				New RegressionEvaluation(3),
				New RegressionEvaluation(),
				New EvaluationCalibration()
			}

			For Each e As IEvaluation In arr
				Dim json As String = e.toJson()
				Dim stats As String = e.stats()
				If print Then
					Console.WriteLine(e.GetType() & vbLf & json & vbLf & vbLf)
				End If

				Dim fromJson As IEvaluation = BaseEvaluation.fromJson(json, GetType(BaseEvaluation))
				assertEquals(e.toJson(), fromJson.toJson())
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerde(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSerde(ByVal backend As Nd4jBackend)
			Dim print As Boolean = False
			Nd4j.Random.setSeed(12345)

			Dim evaluation As New Evaluation()
			Dim evaluationBinary As New EvaluationBinary()
			Dim roc As New ROC(2)
			Dim roc2 As New ROCBinary(2)
			Dim roc3 As New ROCMultiClass(2)
			Dim regressionEvaluation As New RegressionEvaluation()
			Dim ec As New EvaluationCalibration()


			Dim arr() As IEvaluation = {evaluation, evaluationBinary, roc, roc2, roc3, regressionEvaluation, ec}

			Dim evalLabel As INDArray = Nd4j.create(10, 3)
			For i As Integer = 0 To 9
				evalLabel.putScalar(i, i Mod 3, 1.0)
			Next i
			Dim evalProb As INDArray = Nd4j.rand(10, 3)
			evalProb.diviColumnVector(evalProb.sum(1))
			evaluation.eval(evalLabel, evalProb)
			roc3.eval(evalLabel, evalProb)
			ec.eval(evalLabel, evalProb)

			evalLabel = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(10, 3), 0.5))
			evalProb = Nd4j.rand(10, 3)
			evaluationBinary.eval(evalLabel, evalProb)
			roc2.eval(evalLabel, evalProb)

			evalLabel = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(10, 1), 0.5))
			evalProb = Nd4j.rand(10, 1)
			roc.eval(evalLabel, evalProb)

			regressionEvaluation.eval(Nd4j.rand(10, 3), Nd4j.rand(10, 3))

			For Each e As IEvaluation In arr
				Dim json As String = e.toJson()
				If print Then
					Console.WriteLine(e.GetType() & vbLf & json & vbLf & vbLf)
				End If

				Dim fromJson As IEvaluation = BaseEvaluation.fromJson(json, GetType(BaseEvaluation))
				assertEquals(e.toJson(), fromJson.toJson())
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerdeExactRoc(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSerdeExactRoc(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim print As Boolean = False

			Dim roc As New ROC(0)
			Dim roc2 As New ROCBinary(0)
			Dim roc3 As New ROCMultiClass(0)


			Dim arr() As IEvaluation = {roc, roc2, roc3}

			Dim evalLabel As INDArray = Nd4j.create(100, 3)
			For i As Integer = 0 To 99
				evalLabel.putScalar(i, i Mod 3, 1.0)
			Next i
			Dim evalProb As INDArray = Nd4j.rand(100, 3)
			evalProb.diviColumnVector(evalProb.sum(1))
			roc3.eval(evalLabel, evalProb)

			evalLabel = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(100, 3), 0.5))
			evalProb = Nd4j.rand(100, 3)
			roc2.eval(evalLabel, evalProb)

			evalLabel = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(100, 1), 0.5))
			evalProb = Nd4j.rand(100, 1)
			roc.eval(evalLabel, evalProb)

			For Each e As IEvaluation In arr
				Console.WriteLine(e.GetType())
				Dim json As String = e.toJson()
				Dim stats As String = e.stats()
				If print Then
					Console.WriteLine(json & vbLf & vbLf)
				End If
				Dim fromJson As IEvaluation = BaseEvaluation.fromJson(json, GetType(BaseEvaluation))
				assertEquals(e, fromJson)

				If TypeOf fromJson Is ROC Then
					'Shouldn't have probAndLabel, but should have stored AUC and AUPRC
					assertNull(CType(fromJson, ROC).getProbAndLabel())
					assertTrue(CType(fromJson, ROC).calculateAUC() > 0.0)
					assertTrue(CType(fromJson, ROC).calculateAUCPR() > 0.0)

					assertEquals(CType(e, ROC).RocCurve, CType(fromJson, ROC).RocCurve)
					assertEquals(CType(e, ROC).PrecisionRecallCurve, CType(fromJson, ROC).PrecisionRecallCurve)
				ElseIf TypeOf e Is ROCBinary Then
					Dim rocs() As ROC = CType(fromJson, ROCBinary).getUnderlying()
					Dim origRocs() As ROC = CType(e, ROCBinary).getUnderlying()
					'                for(ROC r : rocs ){
					For i As Integer = 0 To origRocs.Length - 1
						Dim r As ROC = rocs(i)
						Dim origR As ROC = origRocs(i)
						'Shouldn't have probAndLabel, but should have stored AUC and AUPRC, AND stored curves
						assertNull(r.getProbAndLabel())
						assertEquals(origR.calculateAUC(), origR.calculateAUC(), 1e-6)
						assertEquals(origR.calculateAUCPR(), origR.calculateAUCPR(), 1e-6)
						assertEquals(origR.RocCurve, origR.RocCurve)
						assertEquals(origR.PrecisionRecallCurve, origR.PrecisionRecallCurve)
					Next i

				ElseIf TypeOf e Is ROCMultiClass Then
					Dim rocs() As ROC = CType(fromJson, ROCMultiClass).getUnderlying()
					Dim origRocs() As ROC = CType(e, ROCMultiClass).getUnderlying()
					For i As Integer = 0 To origRocs.Length - 1
						Dim r As ROC = rocs(i)
						Dim origR As ROC = origRocs(i)
						'Shouldn't have probAndLabel, but should have stored AUC and AUPRC, AND stored curves
						assertNull(r.getProbAndLabel())
						assertEquals(origR.calculateAUC(), origR.calculateAUC(), 1e-6)
						assertEquals(origR.calculateAUCPR(), origR.calculateAUCPR(), 1e-6)
						assertEquals(origR.RocCurve, origR.RocCurve)
						assertEquals(origR.PrecisionRecallCurve, origR.PrecisionRecallCurve)
					Next i
				End If
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJsonYamlCurves(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJsonYamlCurves(ByVal backend As Nd4jBackend)
			Dim roc As New ROC(0)

			Dim evalLabel As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(100, 1), 0.5))
			Dim evalProb As INDArray = Nd4j.rand(100, 1)
			roc.eval(evalLabel, evalProb)

			Dim c As RocCurve = roc.RocCurve
			Dim prc As PrecisionRecallCurve = roc.PrecisionRecallCurve

			Dim json1 As String = c.toJson()
			Dim json2 As String = prc.toJson()

			Dim c2 As RocCurve = RocCurve.fromJson(json1)
			Dim prc2 As PrecisionRecallCurve = PrecisionRecallCurve.fromJson(json2)

			assertEquals(c, c2)
			assertEquals(prc, prc2)

			'        System.out.println(json1);

			'Also test: histograms

			Dim ec As New EvaluationCalibration()

			evalLabel = Nd4j.create(10, 3)
			For i As Integer = 0 To 9
				evalLabel.putScalar(i, i Mod 3, 1.0)
			Next i
			evalProb = Nd4j.rand(10, 3)
			evalProb.diviColumnVector(evalProb.sum(1))
			ec.eval(evalLabel, evalProb)

			Dim histograms() As Histogram = {ec.ResidualPlotAllClasses, ec.getResidualPlot(0), ec.getResidualPlot(1), ec.ProbabilityHistogramAllClasses, ec.getProbabilityHistogram(0), ec.getProbabilityHistogram(1)}

			For Each h As Histogram In histograms
				Dim json As String = h.toJson()
				Dim yaml As String = h.toYaml()

				Dim h2 As Histogram = Histogram.fromJson(json)
				Dim h3 As Histogram = Histogram.fromYaml(yaml)

				assertEquals(h, h2)
				assertEquals(h2, h3)
			Next h

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJsonWithCustomThreshold(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJsonWithCustomThreshold(ByVal backend As Nd4jBackend)

			'Evaluation - binary threshold
			Dim e As New Evaluation(0.25)
			Dim json As String = e.toJson()
			Dim yaml As String = e.toYaml()

			Dim eFromJson As Evaluation = Evaluation.fromJson(json)
			Dim eFromYaml As Evaluation = Evaluation.fromYaml(yaml)

			assertEquals(0.25, eFromJson.getBinaryDecisionThreshold(), 1e-6)
			assertEquals(0.25, eFromYaml.getBinaryDecisionThreshold(), 1e-6)


			'Evaluation: custom cost array
			Dim costArray As INDArray = Nd4j.create(New Double() {1.0, 2.0, 3.0})
			Dim e2 As New Evaluation(costArray)

			json = e2.toJson()
			yaml = e2.toYaml()

			eFromJson = Evaluation.fromJson(json)
			eFromYaml = Evaluation.fromYaml(yaml)

			assertEquals(e2.getCostArray(), eFromJson.getCostArray())
			assertEquals(e2.getCostArray(), eFromYaml.getCostArray())



			'EvaluationBinary - per-output binary threshold
			Dim threshold As INDArray = Nd4j.create(New Double() {1.0, 0.5, 0.25})
			Dim eb As New EvaluationBinary(threshold)

			json = eb.toJson()
			yaml = eb.toYaml()

			Dim ebFromJson As EvaluationBinary = EvaluationBinary.fromJson(json)
			Dim ebFromYaml As EvaluationBinary = EvaluationBinary.fromYaml(yaml)

			assertEquals(threshold, ebFromJson.getDecisionThreshold())
			assertEquals(threshold, ebFromYaml.getDecisionThreshold())

		End Sub

	End Class

End Namespace