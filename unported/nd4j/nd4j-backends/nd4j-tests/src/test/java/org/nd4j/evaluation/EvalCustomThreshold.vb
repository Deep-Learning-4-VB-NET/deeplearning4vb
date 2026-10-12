Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports EvaluationBinary = org.nd4j.evaluation.classification.EvaluationBinary
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ScalarMin = org.nd4j.linalg.api.ops.impl.scalar.ScalarMin
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) public class EvalCustomThreshold extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class EvalCustomThreshold
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationCustomBinaryThreshold(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationCustomBinaryThreshold(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			'Sanity checks: 0.5 threshold for 1-output and 2-output binary cases
			Dim e As New Evaluation()
			Dim e05 As New Evaluation(0.5)
			Dim e05v2 As New Evaluation(0.5)

			Dim nExamples As Integer = 20
			Dim nOut As Integer = 2
			Dim probs As INDArray = Nd4j.rand(nExamples, nOut)
			probs.diviColumnVector(probs.sum(1))
			Dim labels As INDArray = Nd4j.create(nExamples, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To nExamples - 1
				labels.putScalar(i, r.Next(2), 1.0)
			Next i

			e.eval(labels, probs)
			e05.eval(labels, probs)
			e05v2.eval(labels.getColumn(1, True), probs.getColumn(1, True)) '"single output binary" case

			For Each e2 As Evaluation In New Evaluation() {e05, e05v2}
				assertEquals(e.accuracy(), e2.accuracy(), 1e-6)
				assertEquals(e.f1(), e2.f1(), 1e-6)
				assertEquals(e.precision(), e2.precision(), 1e-6)
				assertEquals(e.recall(), e2.recall(), 1e-6)
				assertEquals(e.getConfusionMatrix(), e2.getConfusionMatrix())
			Next e2

			'Check with decision threshold of 0.25
			'In this test, we'll cheat a bit: multiply class 1 probabilities by 2 (max of 1.0); this should give an
			' identical result to a threshold of 0.5 vs. no multiplication and threshold of 0.25

			Dim p2 As INDArray = probs.dup()
			Dim p2c As INDArray = p2.getColumn(1)
			p2c.muli(2.0)
			Nd4j.Executioner.exec(New ScalarMin(p2c, Nothing, p2c, 1.0))
			p2.getColumn(0).assign(p2.getColumn(1).rsub(1.0))

			Dim e025 As New Evaluation(0.25)
			e025.eval(labels, probs)

			Dim ex2 As New Evaluation()
			ex2.eval(labels, p2)

			assertEquals(ex2.accuracy(), e025.accuracy(), 1e-6)
			assertEquals(ex2.f1(), e025.f1(), 1e-6)
			assertEquals(ex2.precision(), e025.precision(), 1e-6)
			assertEquals(ex2.recall(), e025.recall(), 1e-6)
			assertEquals(ex2.getConfusionMatrix(), e025.getConfusionMatrix())


			'Check the same thing, but the single binary output case:

			Dim e025v2 As New Evaluation(0.25)
			e025v2.eval(labels.getColumn(1, True), probs.getColumn(1, True))

			assertEquals(ex2.accuracy(), e025v2.accuracy(), 1e-6)
			assertEquals(ex2.f1(), e025v2.f1(), 1e-6)
			assertEquals(ex2.precision(), e025v2.precision(), 1e-6)
			assertEquals(ex2.recall(), e025v2.recall(), 1e-6)
			assertEquals(ex2.getConfusionMatrix(), e025v2.getConfusionMatrix())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationCostArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationCostArray(ByVal backend As Nd4jBackend)


			Dim nExamples As Integer = 20
			Dim nOut As Integer = 3
			Nd4j.Random.setSeed(12345)
			Dim probs As INDArray = Nd4j.rand(nExamples, nOut)
			probs.diviColumnVector(probs.sum(1))
			Dim labels As INDArray = Nd4j.create(nExamples, nOut)
			Dim r As New Random(12345)
			For j As Integer = 0 To nExamples - 1
				labels.putScalar(j, r.Next(2), 1.0)
			Next j

			Dim e As New Evaluation()
			e.eval(labels, probs)

			'Sanity check: "all equal" cost array - equal to no cost array
			For i As Integer = 1 To 3
				Dim e2 As New Evaluation(Nd4j.valueArrayOf(New Integer() {1, nOut}, i))
				e2.eval(labels, probs)

				assertEquals(e.accuracy(), e2.accuracy(), 1e-6)
				assertEquals(e.f1(), e2.f1(), 1e-6)
				assertEquals(e.precision(), e2.precision(), 1e-6)
				assertEquals(e.recall(), e2.recall(), 1e-6)
				assertEquals(e.getConfusionMatrix(), e2.getConfusionMatrix())
			Next i

			'Manual checks:
			Dim costArray As INDArray = Nd4j.create(New Double() {5, 2, 1})
			labels = Nd4j.create(New Double()() {
				New Double() {1, 0, 0},
				New Double() {0, 1, 0},
				New Double() {0, 0, 1}
			})
			probs = Nd4j.create(New Double()() {
				New Double() {0.2, 0.3, 0.5},
				New Double() {0.1, 0.4, 0.5},
				New Double() {0.1, 0.1, 0.8}
			})

			'With no cost array: only last example is predicted correctly
			e = New Evaluation()
			e.eval(labels, probs)
			assertEquals(1.0 / 3, e.accuracy(), 1e-6)

			'With cost array: all examples predicted correctly
			Dim e2 As New Evaluation(costArray)
			e2.eval(labels, probs)
			assertEquals(1.0, e2.accuracy(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinaryCustomThreshold(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinaryCustomThreshold(ByVal backend As Nd4jBackend)

			'Sanity check: same results for 0.5 threshold vs. default (no threshold)
			Dim nExamples As Integer = 20
			Dim nOut As Integer = 2
			Dim probs As INDArray = Nd4j.rand(nExamples, nOut)
			Dim labels As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(nExamples, nOut), 0.5))

			Dim eStd As New EvaluationBinary()
			eStd.eval(labels, probs)

			Dim eb05 As New EvaluationBinary(Nd4j.create(New Double() {0.5, 0.5}, New Long(){1, 2}))
			eb05.eval(labels, probs)

			Dim eb05v2 As New EvaluationBinary(Nd4j.create(New Double() {0.5, 0.5}, New Long(){1, 2}))
			For i As Integer = 0 To nExamples - 1
				eb05v2.eval(labels.getRow(i, True), probs.getRow(i, True))
			Next i

			For Each eb2 As EvaluationBinary In New EvaluationBinary() {eb05, eb05v2}
				assertArrayEquals(eStd.getCountTruePositive(), eb2.getCountTruePositive())
				assertArrayEquals(eStd.getCountFalsePositive(), eb2.getCountFalsePositive())
				assertArrayEquals(eStd.getCountTrueNegative(), eb2.getCountTrueNegative())
				assertArrayEquals(eStd.getCountFalseNegative(), eb2.getCountFalseNegative())

				For j As Integer = 0 To nOut - 1
					assertEquals(eStd.accuracy(j), eb2.accuracy(j), 1e-6)
					assertEquals(eStd.f1(j), eb2.f1(j), 1e-6)
				Next j
			Next eb2


			'Check with decision threshold of 0.25 and 0.125 (for different outputs)
			'In this test, we'll cheat a bit: multiply probabilities by 2 (max of 1.0) and threshold of 0.25 should give
			' an identical result to a threshold of 0.5
			'Ditto for 4x and 0.125 threshold

			Dim probs2 As INDArray = probs.mul(2)
			probs2 = Transforms.min(probs2, 1.0)

			Dim probs4 As INDArray = probs.mul(4)
			probs4 = Transforms.min(probs4, 1.0)

			Dim ebThreshold As New EvaluationBinary(Nd4j.create(New Double() {0.25, 0.125}))
			ebThreshold.eval(labels, probs)

			Dim ebStd2 As New EvaluationBinary()
			ebStd2.eval(labels, probs2)

			Dim ebStd4 As New EvaluationBinary()
			ebStd4.eval(labels, probs4)

			assertEquals(ebThreshold.truePositives(0), ebStd2.truePositives(0))
			assertEquals(ebThreshold.trueNegatives(0), ebStd2.trueNegatives(0))
			assertEquals(ebThreshold.falsePositives(0), ebStd2.falsePositives(0))
			assertEquals(ebThreshold.falseNegatives(0), ebStd2.falseNegatives(0))

			assertEquals(ebThreshold.truePositives(1), ebStd4.truePositives(1))
			assertEquals(ebThreshold.trueNegatives(1), ebStd4.trueNegatives(1))
			assertEquals(ebThreshold.falsePositives(1), ebStd4.falsePositives(1))
			assertEquals(ebThreshold.falseNegatives(1), ebStd4.falseNegatives(1))
		End Sub

	End Class

End Namespace