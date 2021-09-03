Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.interval

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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.EVAL_METRICS) public class EvalTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class EvalTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEval(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEval(ByVal backend As Nd4jBackend)
			Dim classNum As Integer = 5
			Dim eval As New Evaluation(classNum)

			' Testing the edge case when some classes do not have true positive
			Dim trueOutcome As INDArray = FeatureUtil.toOutcomeVector(0, 5) '[1,0,0,0,0]
			Dim predictedOutcome As INDArray = FeatureUtil.toOutcomeVector(0, 5) '[1,0,0,0,0]
			eval.eval(trueOutcome, predictedOutcome)
			assertEquals(1, eval.classCount(0))
			assertEquals(1.0, eval.f1(), 1e-1)

			' Testing more than one sample. eval() does not reset the Evaluation instance
			Dim trueOutcome2 As INDArray = FeatureUtil.toOutcomeVector(1, 5) '[0,1,0,0,0]
			Dim predictedOutcome2 As INDArray = FeatureUtil.toOutcomeVector(0, 5) '[1,0,0,0,0]
			eval.eval(trueOutcome2, predictedOutcome2)
			' Verified with sklearn in Python
			' from sklearn.metrics import classification_report
			' classification_report(['a', 'a'], ['a', 'b'], labels=['a', 'b', 'c', 'd', 'e'])
			assertEquals(eval.f1(), 0.6, 1e-1)
			' The first entry is 0 label
			assertEquals(1, eval.classCount(0))
			' The first entry is 1 label
			assertEquals(1, eval.classCount(1))
			' Class 0: one positive, one negative -> (one true positive, one false positive); no true/false negatives
			assertEquals(1, eval.positive()(0), 0)
			assertEquals(1, eval.negative()(0), 0)
			assertEquals(1, eval.truePositives()(0), 0)
			assertEquals(1, eval.falsePositives()(0), 0)
			assertEquals(0, eval.trueNegatives()(0), 0)
			assertEquals(0, eval.falseNegatives()(0), 0)


			' The rest are negative
			assertEquals(1, eval.negative()(0), 0)
			' 2 rows and only the first is correct
			assertEquals(0.5, eval.accuracy(), 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEval2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEval2(ByVal backend As Nd4jBackend)

			Dim dtypeBefore As DataType = Nd4j.defaultFloatingPointType()
			Dim first As Evaluation = Nothing
			Dim sFirst As String = Nothing
			Try
				For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT}
					Nd4j.setDefaultDataTypes(globalDtype,If(globalDtype.isFPType(), globalDtype, DataType.DOUBLE))
					For Each lpDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}

						'Confusion matrix:
						'actual 0      20      3
						'actual 1      10      5

						Dim evaluation As New Evaluation(java.util.Arrays.asList("class0", "class1"))
						Dim predicted0 As INDArray = Nd4j.create(New Double(){1, 0}, New Long(){1, 2}).castTo(lpDtype)
						Dim predicted1 As INDArray = Nd4j.create(New Double(){0, 1}, New Long(){1, 2}).castTo(lpDtype)
						Dim actual0 As INDArray = Nd4j.create(New Double(){1, 0}, New Long(){1, 2}).castTo(lpDtype)
						Dim actual1 As INDArray = Nd4j.create(New Double(){0, 1}, New Long(){1, 2}).castTo(lpDtype)
						For i As Integer = 0 To 19
							evaluation.eval(actual0, predicted0)
						Next i

						For i As Integer = 0 To 2
							evaluation.eval(actual0, predicted1)
						Next i

						For i As Integer = 0 To 9
							evaluation.eval(actual1, predicted0)
						Next i

						For i As Integer = 0 To 4
							evaluation.eval(actual1, predicted1)
						Next i

						assertEquals(20, evaluation.truePositives()(0), 0)
						assertEquals(3, evaluation.falseNegatives()(0), 0)
						assertEquals(10, evaluation.falsePositives()(0), 0)
						assertEquals(5, evaluation.trueNegatives()(0), 0)

						assertEquals((20.0 + 5) / (20 + 3 + 10 + 5), evaluation.accuracy(), 1e-6)

						Dim s As String = evaluation.stats()

						If first Is Nothing Then
							first = evaluation
							sFirst = s
						Else
							assertEquals(first, evaluation)
							assertEquals(sFirst, s)
						End If
					Next lpDtype
				Next globalDtype
			Finally
				Nd4j.setDefaultDataTypes(dtypeBefore, dtypeBefore)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStringListLabels(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStringListLabels(ByVal backend As Nd4jBackend)
			Dim trueOutcome As INDArray = FeatureUtil.toOutcomeVector(0, 2)
			Dim predictedOutcome As INDArray = FeatureUtil.toOutcomeVector(0, 2)

			Dim labelsList As IList(Of String) = New List(Of String)()
			labelsList.Add("hobbs")
			labelsList.Add("cal")

			Dim eval As New Evaluation(labelsList)

			eval.eval(trueOutcome, predictedOutcome)
			assertEquals(1, eval.classCount(0))
			assertEquals(labelsList(0), eval.getClassLabel(0))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStringHashLabels(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStringHashLabels(ByVal backend As Nd4jBackend)
			Dim trueOutcome As INDArray = FeatureUtil.toOutcomeVector(0, 2)
			Dim predictedOutcome As INDArray = FeatureUtil.toOutcomeVector(0, 2)

			Dim labelsMap As IDictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
			labelsMap(0) = "hobbs"
			labelsMap(1) = "cal"

			Dim eval As New Evaluation(labelsMap)

			eval.eval(trueOutcome, predictedOutcome)
			assertEquals(1, eval.classCount(0))
			assertEquals(labelsMap(0), eval.getClassLabel(0))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvalMasking(ByVal backend As Nd4jBackend)
			Dim miniBatch As Integer = 5
			Dim nOut As Integer = 3
			Dim tsLength As Integer = 6

			Dim labels As INDArray = Nd4j.zeros(miniBatch, nOut, tsLength)
			Dim predicted As INDArray = Nd4j.zeros(miniBatch, nOut, tsLength)

			Nd4j.Random.setSeed(12345)
			Dim r As New Random(12345)
			For i As Integer = 0 To miniBatch - 1
				For j As Integer = 0 To tsLength - 1
					Dim rand As INDArray = Nd4j.rand(1, nOut)
					rand.divi(rand.sumNumber())
					predicted.put(New INDArrayIndex() {NDArrayIndex.point(i), all(), NDArrayIndex.point(j)}, rand)
					Dim idx As Integer = r.Next(nOut)
					labels.putScalar(New Integer() {i, idx, j}, 1.0)
				Next j
			Next i

			'Create a longer labels/predicted with mask for first and last time step
			'Expect masked evaluation to be identical to original evaluation
			Dim labels2 As INDArray = Nd4j.zeros(miniBatch, nOut, tsLength + 2)
			labels2.put(New INDArrayIndex() {all(), all(), interval(1, tsLength + 1)}, labels)
			Dim predicted2 As INDArray = Nd4j.zeros(miniBatch, nOut, tsLength + 2)
			predicted2.put(New INDArrayIndex() {all(), all(), interval(1, tsLength + 1)}, predicted)

			Dim labelsMask As INDArray = Nd4j.ones(miniBatch, tsLength + 2)
			For i As Integer = 0 To miniBatch - 1
				labelsMask.putScalar(New Integer() {i, 0}, 0.0)
				labelsMask.putScalar(New Integer() {i, tsLength + 1}, 0.0)
			Next i

			Dim evaluation As New Evaluation()
			evaluation.evalTimeSeries(labels, predicted)

			Dim evaluation2 As New Evaluation()
			evaluation2.evalTimeSeries(labels2, predicted2, labelsMask)

	'        System.out.println(evaluation.stats());
	'        System.out.println(evaluation2.stats());
			evaluation.stats()
			evaluation2.stats()

			assertEquals(evaluation.accuracy(), evaluation2.accuracy(), 1e-12)
			assertEquals(evaluation.f1(), evaluation2.f1(), 1e-12)

			assertMapEquals(evaluation.falsePositives(), evaluation2.falsePositives())
			assertMapEquals(evaluation.falseNegatives(), evaluation2.falseNegatives())
			assertMapEquals(evaluation.truePositives(), evaluation2.truePositives())
			assertMapEquals(evaluation.trueNegatives(), evaluation2.trueNegatives())

			For i As Integer = 0 To nOut - 1
				assertEquals(evaluation.classCount(i), evaluation2.classCount(i))
			Next i
		End Sub

		Private Shared Sub assertMapEquals(ByVal first As IDictionary(Of Integer, Integer), ByVal second As IDictionary(Of Integer, Integer))
			assertEquals(first.Keys, second.Keys)
			For Each i As Integer? In first.Keys
				assertEquals(first(i), second(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFalsePerfectRecall(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFalsePerfectRecall(ByVal backend As Nd4jBackend)
			Dim testSize As Integer = 100
			Dim numClasses As Integer = 5
			Dim winner As Integer = 1
			Dim seed As Integer = 241

			Dim labels As INDArray = Nd4j.zeros(testSize, numClasses)
			Dim predicted As INDArray = Nd4j.zeros(testSize, numClasses)

			Nd4j.Random.setSeed(seed)
			Dim r As New Random(seed)

			'Modelling the situation when system predicts the same class every time
			For i As Integer = 0 To testSize - 1
				'Generating random prediction but with a guaranteed winner
				Dim rand As INDArray = Nd4j.rand(1, numClasses)
				rand.put(0, winner, rand.sumNumber())
				rand.divi(rand.sumNumber())
				predicted.put(New INDArrayIndex() {NDArrayIndex.point(i), all()}, rand)
				'Generating random label
				Dim label As Integer = r.Next(numClasses)
				labels.putScalar(New Integer() {i, label}, 1.0)
			Next i

			'Explicitly specify the amount of classes
			Dim eval As New Evaluation(numClasses)
			eval.eval(labels, predicted)

			'For sure we shouldn't arrive at 100% recall unless we guessed everything right for every class
			assertNotEquals(1.0, eval.recall())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationMerging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationMerging(ByVal backend As Nd4jBackend)

			Dim nRows As Integer = 20
			Dim nCols As Integer = 3

			Dim r As New Random(12345)
			Dim actual As INDArray = Nd4j.create(nRows, nCols)
			Dim predicted As INDArray = Nd4j.create(nRows, nCols)
			For i As Integer = 0 To nRows - 1
				Dim x1 As Integer = r.Next(nCols)
				Dim x2 As Integer = r.Next(nCols)
				actual.putScalar(New Integer() {i, x1}, 1.0)
				predicted.putScalar(New Integer() {i, x2}, 1.0)
			Next i

			Dim evalExpected As New Evaluation()
			evalExpected.eval(actual, predicted)


			'Now: split into 3 separate evaluation objects -> expect identical values after merging
			Dim eval1 As New Evaluation()
			eval1.eval(actual.get(interval(0, 5), all()), predicted.get(interval(0, 5), all()))

			Dim eval2 As New Evaluation()
			eval2.eval(actual.get(interval(5, 10), all()), predicted.get(interval(5, 10), all()))

			Dim eval3 As New Evaluation()
			eval3.eval(actual.get(interval(10, nRows), all()), predicted.get(interval(10, nRows), all()))

			eval1.merge(eval2)
			eval1.merge(eval3)

			checkEvaluationEquality(evalExpected, eval1)


			'Next: check evaluation merging with empty, and empty merging with non-empty
			eval1 = New Evaluation()
			eval1.eval(actual.get(interval(0, 5), all()), predicted.get(interval(0, 5), all()))

			Dim evalInitiallyEmpty As New Evaluation()
			evalInitiallyEmpty.merge(eval1)
			evalInitiallyEmpty.merge(eval2)
			evalInitiallyEmpty.merge(eval3)
			checkEvaluationEquality(evalExpected, evalInitiallyEmpty)

			eval1.merge(New Evaluation())
			eval1.merge(eval2)
			eval1.merge(New Evaluation())
			eval1.merge(eval3)
			checkEvaluationEquality(evalExpected, eval1)
		End Sub

		Private Shared Sub checkEvaluationEquality(ByVal evalExpected As Evaluation, ByVal evalActual As Evaluation)
			assertEquals(evalExpected.accuracy(), evalActual.accuracy(), 1e-3)
			assertEquals(evalExpected.f1(), evalActual.f1(), 1e-3)
			assertEquals(evalExpected.NumRowCounter, evalActual.NumRowCounter, 1e-3)
			assertMapEquals(evalExpected.falseNegatives(), evalActual.falseNegatives())
			assertMapEquals(evalExpected.falsePositives(), evalActual.falsePositives())
			assertMapEquals(evalExpected.trueNegatives(), evalActual.trueNegatives())
			assertMapEquals(evalExpected.truePositives(), evalActual.truePositives())
			assertEquals(evalExpected.precision(), evalActual.precision(), 1e-3)
			assertEquals(evalExpected.recall(), evalActual.recall(), 1e-3)
			assertEquals(evalExpected.falsePositiveRate(), evalActual.falsePositiveRate(), 1e-3)
			assertEquals(evalExpected.falseNegativeRate(), evalActual.falseNegativeRate(), 1e-3)
			assertEquals(evalExpected.falseAlarmRate(), evalActual.falseAlarmRate(), 1e-3)
			assertEquals(evalExpected.getConfusionMatrix(), evalActual.getConfusionMatrix())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSingleClassBinaryClassification(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSingleClassBinaryClassification(ByVal backend As Nd4jBackend)

			Dim eval As New Evaluation(1)

			For xe As Integer = 0 To 2
				Dim zero As INDArray = Nd4j.create(1,1)
				Dim one As INDArray = Nd4j.ones(1,1)

				'One incorrect, three correct
				eval.eval(one, zero)
				eval.eval(one, one)
				eval.eval(one, one)
				eval.eval(zero, zero)

	'            System.out.println(eval.stats());
				eval.stats()

				assertEquals(0.75, eval.accuracy(), 1e-6)
				assertEquals(4, eval.NumRowCounter)

				assertEquals(1, CInt(eval.truePositives()(0)))
				assertEquals(2, CInt(eval.truePositives()(1)))
				assertEquals(1, CInt(eval.falseNegatives()(1)))

				eval.reset()
			Next xe
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalInvalid(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvalInvalid(ByVal backend As Nd4jBackend)
			Dim e As New Evaluation(5)
			e.eval(0, 1)
			e.eval(1, 0)
			e.eval(1, 1)

	'        System.out.println(e.stats());
			e.stats()

			assertFalse(e.stats().Contains(ChrW(&HFFFD).ToString()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalMethods(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvalMethods(ByVal backend As Nd4jBackend)
			'Check eval(int,int) vs. eval(INDArray,INDArray)

			Dim e1 As New Evaluation(4)
			Dim e2 As New Evaluation(4)

			Dim i0 As INDArray = Nd4j.create(New Double() {1, 0, 0, 0}, New Long(){1, 4})
			Dim i1 As INDArray = Nd4j.create(New Double() {0, 1, 0, 0}, New Long(){1, 4})
			Dim i2 As INDArray = Nd4j.create(New Double() {0, 0, 1, 0}, New Long(){1, 4})
			Dim i3 As INDArray = Nd4j.create(New Double() {0, 0, 0, 1}, New Long(){1, 4})

			e1.eval(i0, i0) 'order: actual, predicted
			e2.eval(0, 0) 'order: predicted, actual
			e1.eval(i0, i2)
			e2.eval(2, 0)
			e1.eval(i0, i2)
			e2.eval(2, 0)
			e1.eval(i1, i2)
			e2.eval(2, 1)
			e1.eval(i3, i3)
			e2.eval(3, 3)
			e1.eval(i3, i0)
			e2.eval(0, 3)
			e1.eval(i3, i0)
			e2.eval(0, 3)

			Dim cm As org.nd4j.evaluation.classification.ConfusionMatrix(Of Integer) = e1.getConfusionMatrix()
			assertEquals(1, cm.getCount(0, 0)) 'Order: actual, predicted
			assertEquals(2, cm.getCount(0, 2))
			assertEquals(1, cm.getCount(1, 2))
			assertEquals(1, cm.getCount(3, 3))
			assertEquals(2, cm.getCount(3, 0))

	'        System.out.println(e1.stats());
	'        System.out.println(e2.stats());
			e1.stats()
			e2.stats()

			assertEquals(e1.stats(), e2.stats())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTopNAccuracy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTopNAccuracy(ByVal backend As Nd4jBackend)

			Dim e As New Evaluation(Nothing, 3)

			Dim i0 As INDArray = Nd4j.create(New Double() {1, 0, 0, 0, 0}, New Long(){1, 5})
			Dim i1 As INDArray = Nd4j.create(New Double() {0, 1, 0, 0, 0}, New Long(){1, 5})

			Dim p0_0 As INDArray = Nd4j.create(New Double() {0.8, 0.05, 0.05, 0.05, 0.05}, New Long(){1, 5}) 'class 0: highest prob
			Dim p0_1 As INDArray = Nd4j.create(New Double() {0.4, 0.45, 0.05, 0.05, 0.05}, New Long(){1, 5}) 'class 0: 2nd highest prob
			Dim p0_2 As INDArray = Nd4j.create(New Double() {0.1, 0.45, 0.35, 0.05, 0.05}, New Long(){1, 5}) 'class 0: 3rd highest prob
			Dim p0_3 As INDArray = Nd4j.create(New Double() {0.1, 0.40, 0.30, 0.15, 0.05}, New Long(){1, 5}) 'class 0: 4th highest prob

			Dim p1_0 As INDArray = Nd4j.create(New Double() {0.05, 0.80, 0.05, 0.05, 0.05}, New Long(){1, 5}) 'class 1: highest prob
			Dim p1_1 As INDArray = Nd4j.create(New Double() {0.45, 0.40, 0.05, 0.05, 0.05}, New Long(){1, 5}) 'class 1: 2nd highest prob
			Dim p1_2 As INDArray = Nd4j.create(New Double() {0.35, 0.10, 0.45, 0.05, 0.05}, New Long(){1, 5}) 'class 1: 3rd highest prob
			Dim p1_3 As INDArray = Nd4j.create(New Double() {0.40, 0.10, 0.30, 0.15, 0.05}, New Long(){1, 5}) 'class 1: 4th highest prob


			'                                              Correct     TopNCorrect     Total
			e.eval(i0, p0_0) '  1           1               1
			assertEquals(1.0, e.accuracy(), 1e-6)
			assertEquals(1.0, e.topNAccuracy(), 1e-6)
			assertEquals(1, e.TopNCorrectCount)
			assertEquals(1, e.TopNTotalCount)
			e.eval(i0, p0_1) '  1           2               2
			assertEquals(0.5, e.accuracy(), 1e-6)
			assertEquals(1.0, e.topNAccuracy(), 1e-6)
			assertEquals(2, e.TopNCorrectCount)
			assertEquals(2, e.TopNTotalCount)
			e.eval(i0, p0_2) '  1           3               3
			assertEquals(1.0 / 3, e.accuracy(), 1e-6)
			assertEquals(1.0, e.topNAccuracy(), 1e-6)
			assertEquals(3, e.TopNCorrectCount)
			assertEquals(3, e.TopNTotalCount)
			e.eval(i0, p0_3) '  1           3               4
			assertEquals(0.25, e.accuracy(), 1e-6)
			assertEquals(0.75, e.topNAccuracy(), 1e-6)
			assertEquals(3, e.TopNCorrectCount)
			assertEquals(4, e.TopNTotalCount)

			e.eval(i1, p1_0) '  2           4               5
			assertEquals(2.0 / 5, e.accuracy(), 1e-6)
			assertEquals(4.0 / 5, e.topNAccuracy(), 1e-6)
			e.eval(i1, p1_1) '  2           5               6
			assertEquals(2.0 / 6, e.accuracy(), 1e-6)
			assertEquals(5.0 / 6, e.topNAccuracy(), 1e-6)
			e.eval(i1, p1_2) '  2           6               7
			assertEquals(2.0 / 7, e.accuracy(), 1e-6)
			assertEquals(6.0 / 7, e.topNAccuracy(), 1e-6)
			e.eval(i1, p1_3) '  2           6               8
			assertEquals(2.0 / 8, e.accuracy(), 1e-6)
			assertEquals(6.0 / 8, e.topNAccuracy(), 1e-6)
			assertEquals(6, e.TopNCorrectCount)
			assertEquals(8, e.TopNTotalCount)

	'        System.out.println(e.stats());
			e.stats()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTopNAccuracyMerging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTopNAccuracyMerging(ByVal backend As Nd4jBackend)

			Dim e1 As New Evaluation(Nothing, 3)
			Dim e2 As New Evaluation(Nothing, 3)

			Dim i0 As INDArray = Nd4j.create(New Double() {1, 0, 0, 0, 0}, New Long(){1, 5})
			Dim i1 As INDArray = Nd4j.create(New Double() {0, 1, 0, 0, 0}, New Long(){1, 5})

			Dim p0_0 As INDArray = Nd4j.create(New Double() {0.8, 0.05, 0.05, 0.05, 0.05}, New Long(){1, 5}) 'class 0: highest prob
			Dim p0_1 As INDArray = Nd4j.create(New Double() {0.4, 0.45, 0.05, 0.05, 0.05}, New Long(){1, 5}) 'class 0: 2nd highest prob
			Dim p0_2 As INDArray = Nd4j.create(New Double() {0.1, 0.45, 0.35, 0.05, 0.05}, New Long(){1, 5}) 'class 0: 3rd highest prob
			Dim p0_3 As INDArray = Nd4j.create(New Double() {0.1, 0.40, 0.30, 0.15, 0.05}, New Long(){1, 5}) 'class 0: 4th highest prob

			Dim p1_0 As INDArray = Nd4j.create(New Double() {0.05, 0.80, 0.05, 0.05, 0.05}, New Long(){1, 5}) 'class 1: highest prob
			Dim p1_1 As INDArray = Nd4j.create(New Double() {0.45, 0.40, 0.05, 0.05, 0.05}, New Long(){1, 5}) 'class 1: 2nd highest prob
			Dim p1_2 As INDArray = Nd4j.create(New Double() {0.35, 0.10, 0.45, 0.05, 0.05}, New Long(){1, 5}) 'class 1: 3rd highest prob
			Dim p1_3 As INDArray = Nd4j.create(New Double() {0.40, 0.10, 0.30, 0.15, 0.05}, New Long(){1, 5}) 'class 1: 4th highest prob


			'                                              Correct     TopNCorrect     Total
			e1.eval(i0, p0_0) '  1           1               1
			e1.eval(i0, p0_1) '  1           2               2
			e1.eval(i0, p0_2) '  1           3               3
			e1.eval(i0, p0_3) '  1           3               4
			assertEquals(0.25, e1.accuracy(), 1e-6)
			assertEquals(0.75, e1.topNAccuracy(), 1e-6)
			assertEquals(3, e1.TopNCorrectCount)
			assertEquals(4, e1.TopNTotalCount)

			e2.eval(i1, p1_0) '  1           1               1
			e2.eval(i1, p1_1) '  1           2               2
			e2.eval(i1, p1_2) '  1           3               3
			e2.eval(i1, p1_3) '  1           3               4
			assertEquals(1.0 / 4, e2.accuracy(), 1e-6)
			assertEquals(3.0 / 4, e2.topNAccuracy(), 1e-6)
			assertEquals(3, e2.TopNCorrectCount)
			assertEquals(4, e2.TopNTotalCount)

			e1.merge(e2)

			assertEquals(8, e1.NumRowCounter)
			assertEquals(8, e1.TopNTotalCount)
			assertEquals(6, e1.TopNCorrectCount)
			assertEquals(2.0 / 8, e1.accuracy(), 1e-6)
			assertEquals(6.0 / 8, e1.topNAccuracy(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBinaryCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBinaryCase(ByVal backend As Nd4jBackend)
			Dim ones10 As INDArray = Nd4j.ones(10, 1)
			Dim ones4 As INDArray = Nd4j.ones(4, 1)
			Dim zeros4 As INDArray = Nd4j.zeros(4, 1)
			Dim ones3 As INDArray = Nd4j.ones(3, 1)
			Dim zeros3 As INDArray = Nd4j.zeros(3, 1)
			Dim zeros2 As INDArray = Nd4j.zeros(2, 1)

			Dim e As New Evaluation()
			e.eval(ones10, ones10) '10 true positives
			e.eval(ones3, zeros3) '3 false negatives
			e.eval(zeros4, ones4) '4 false positives
			e.eval(zeros2, zeros2) '2 true negatives


			assertEquals((10 + 2) / CDbl(10 + 3 + 4 + 2), e.accuracy(), 1e-6)
			assertEquals(10, CInt(e.truePositives()(1)))
			assertEquals(3, CInt(e.falseNegatives()(1)))
			assertEquals(4, CInt(e.falsePositives()(1)))
			assertEquals(2, CInt(e.trueNegatives()(1)))

			'If we switch the label around: tp becomes tn, fp becomes fn, etc
			assertEquals(10, CInt(e.trueNegatives()(0)))
			assertEquals(3, CInt(e.falsePositives()(0)))
			assertEquals(4, CInt(e.falseNegatives()(0)))
			assertEquals(2, CInt(e.truePositives()(0)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testF1FBeta_MicroMacroAveraging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testF1FBeta_MicroMacroAveraging(ByVal backend As Nd4jBackend)
			'Confusion matrix: rows = actual, columns = predicted
			'[3, 1, 0]
			'[2, 2, 1]
			'[0, 3, 4]

			Dim zero As INDArray = Nd4j.create(New Double() {1, 0, 0}, New Long(){1, 3})
			Dim one As INDArray = Nd4j.create(New Double() {0, 1, 0}, New Long(){1, 3})
			Dim two As INDArray = Nd4j.create(New Double() {0, 0, 1}, New Long(){1, 3})

			Dim e As New Evaluation()
			apply(e, 3, zero, zero)
			apply(e, 1, one, zero)
			apply(e, 2, zero, one)
			apply(e, 2, one, one)
			apply(e, 1, two, one)
			apply(e, 3, one, two)
			apply(e, 4, two, two)

			assertEquals(3, e.getConfusionMatrix().getCount(0, 0))
			assertEquals(1, e.getConfusionMatrix().getCount(0, 1))
			assertEquals(0, e.getConfusionMatrix().getCount(0, 2))
			assertEquals(2, e.getConfusionMatrix().getCount(1, 0))
			assertEquals(2, e.getConfusionMatrix().getCount(1, 1))
			assertEquals(1, e.getConfusionMatrix().getCount(1, 2))
			assertEquals(0, e.getConfusionMatrix().getCount(2, 0))
			assertEquals(3, e.getConfusionMatrix().getCount(2, 1))
			assertEquals(4, e.getConfusionMatrix().getCount(2, 2))

			Dim beta As Double = 3.5
			Dim prec(2) As Double
			Dim rec(2) As Double
			For i As Integer = 0 To 2
				prec(i) = e.truePositives()(i) / CDbl(e.truePositives()(i) + e.falsePositives()(i))
				rec(i) = e.truePositives()(i) / CDbl(e.truePositives()(i) + e.falseNegatives()(i))
			Next i

			'Binarized confusion
			'class 0:
			' [3, 1]       [tp fn]
			' [2, 10]      [fp tn]
			assertEquals(3, CInt(e.truePositives()(0)))
			assertEquals(1, CInt(e.falseNegatives()(0)))
			assertEquals(2, CInt(e.falsePositives()(0)))
			assertEquals(10, CInt(e.trueNegatives()(0)))

			'class 1:
			' [2, 3]       [tp fn]
			' [4, 7]       [fp tn]
			assertEquals(2, CInt(e.truePositives()(1)))
			assertEquals(3, CInt(e.falseNegatives()(1)))
			assertEquals(4, CInt(e.falsePositives()(1)))
			assertEquals(7, CInt(e.trueNegatives()(1)))

			'class 2:
			' [4, 3]       [tp fn]
			' [1, 8]       [fp tn]
			assertEquals(4, CInt(e.truePositives()(2)))
			assertEquals(3, CInt(e.falseNegatives()(2)))
			assertEquals(1, CInt(e.falsePositives()(2)))
			assertEquals(8, CInt(e.trueNegatives()(2)))

			Dim fBeta(2) As Double
			Dim f1(2) As Double
			Dim mcc(2) As Double
			For i As Integer = 0 To 2
				fBeta(i) = (1 + beta * beta) * prec(i) * rec(i) / (beta * beta * prec(i) + rec(i))
				f1(i) = 2 * prec(i) * rec(i) / (prec(i) + rec(i))
				assertEquals(fBeta(i), e.fBeta(beta, i), 1e-6)
				assertEquals(f1(i), e.f1(i), 1e-6)

				Dim gmeasure As Double = Math.Sqrt(prec(i) * rec(i))
				assertEquals(gmeasure, e.gMeasure(i), 1e-6)

				Dim tp As Double = e.truePositives()(i)
				Dim tn As Double = e.trueNegatives()(i)
				Dim fp As Double = e.falsePositives()(i)
				Dim fn As Double = e.falseNegatives()(i)
				mcc(i) = (tp * tn - fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn))
				assertEquals(mcc(i), e.matthewsCorrelation(i), 1e-6)
			Next i

			'Test macro and micro averaging:
			Dim tp As Integer = 0
			Dim fn As Integer = 0
			Dim fp As Integer = 0
			Dim tn As Integer = 0
			Dim macroPrecision As Double = 0.0
			Dim macroRecall As Double = 0.0
			Dim macroF1 As Double = 0.0
			Dim macroFBeta As Double = 0.0
			Dim macroMcc As Double = 0.0
			For i As Integer = 0 To 2
				tp += e.truePositives()(i)
				fn += e.falseNegatives()(i)
				fp += e.falsePositives()(i)
				tn += e.trueNegatives()(i)

				macroPrecision += prec(i)
				macroRecall += rec(i)
				macroF1 += f1(i)
				macroFBeta += fBeta(i)
				macroMcc += mcc(i)
			Next i
			macroPrecision /= 3
			macroRecall /= 3
			macroF1 /= 3
			macroFBeta /= 3
			macroMcc /= 3

			Dim microPrecision As Double = tp / CDbl(tp + fp)
			Dim microRecall As Double = tp / CDbl(tp + fn)
			Dim microFBeta As Double = (1 + beta * beta) * microPrecision * microRecall / (beta * beta * microPrecision + microRecall)
			Dim microF1 As Double = 2 * microPrecision * microRecall / (microPrecision + microRecall)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim microMcc As Double = (tp * tn - fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn))

			assertEquals(microPrecision, e.precision(EvaluationAveraging.Micro), 1e-6)
			assertEquals(microRecall, e.recall(EvaluationAveraging.Micro), 1e-6)
			assertEquals(macroPrecision, e.precision(EvaluationAveraging.Macro), 1e-6)
			assertEquals(macroRecall, e.recall(EvaluationAveraging.Macro), 1e-6)

			assertEquals(microFBeta, e.fBeta(beta, EvaluationAveraging.Micro), 1e-6)
			assertEquals(macroFBeta, e.fBeta(beta, EvaluationAveraging.Macro), 1e-6)

			assertEquals(microF1, e.f1(EvaluationAveraging.Micro), 1e-6)
			assertEquals(macroF1, e.f1(EvaluationAveraging.Macro), 1e-6)

			assertEquals(microMcc, e.matthewsCorrelation(EvaluationAveraging.Micro), 1e-6)
			assertEquals(macroMcc, e.matthewsCorrelation(EvaluationAveraging.Macro), 1e-6)

		End Sub

		Private Shared Sub apply(ByVal e As Evaluation, ByVal nTimes As Integer, ByVal predicted As INDArray, ByVal actual As INDArray)
			For i As Integer = 0 To nTimes - 1
				e.eval(actual, predicted)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConfusionMatrixStats(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConfusionMatrixStats(ByVal backend As Nd4jBackend)

			Dim e As New Evaluation()

			Dim c0 As INDArray = Nd4j.create(New Double() {1, 0, 0}, New Long(){1, 3})
			Dim c1 As INDArray = Nd4j.create(New Double() {0, 1, 0}, New Long(){1, 3})
			Dim c2 As INDArray = Nd4j.create(New Double() {0, 0, 1}, New Long(){1, 3})

			apply(e, 3, c2, c0) 'Predicted class 2 when actually class 0, 3 times
			apply(e, 2, c0, c1) 'Predicted class 0 when actually class 1, 2 times

			Dim s1 As String = " 0 0 3 | 0 = 0" 'First row: predicted 2, actual 0 - 3 times
			Dim s2 As String = " 2 0 0 | 1 = 1" 'Second row: predicted 0, actual 1 - 2 times

			Dim stats As String = e.stats()
			assertTrue(stats.Contains(s1),stats)
			assertTrue(stats.Contains(s2),stats)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalBinaryMetrics()
		Public Overridable Sub testEvalBinaryMetrics()

			Dim ePosClass1_nOut2 As New Evaluation(2, 1)
			Dim ePosClass0_nOut2 As New Evaluation(2, 0)
			Dim ePosClass1_nOut1 As New Evaluation(2, 1)
			Dim ePosClass0_nOut1 As New Evaluation(2, 0)
			Dim ePosClassNull_nOut2 As New Evaluation(2, Nothing)
			Dim ePosClassNull_nOut1 As New Evaluation(2, Nothing)

			Dim evals() As Evaluation = {ePosClass1_nOut2, ePosClass0_nOut2, ePosClass1_nOut1, ePosClass0_nOut1}
			Dim posClass() As Integer = {1, 0, 1, 0, -1, -1}


			'Correct, actual positive class -> TP
			Dim p1_1 As INDArray = Nd4j.create(New Double(){0.3, 0.7}, New Long(){1, 2})
			Dim l1_1 As INDArray = Nd4j.create(New Double(){0, 1}, New Long(){1, 2})
			Dim p1_0 As INDArray = Nd4j.create(New Double(){0.7, 0.3}, New Long(){1, 2})
			Dim l1_0 As INDArray = Nd4j.create(New Double(){1, 0}, New Long(){1, 2})

			'Incorrect, actual positive class -> FN
			Dim p2_1 As INDArray = Nd4j.create(New Double(){0.6, 0.4}, New Long(){1, 2})
			Dim l2_1 As INDArray = Nd4j.create(New Double(){0, 1}, New Long(){1, 2})
			Dim p2_0 As INDArray = Nd4j.create(New Double(){0.4, 0.6}, New Long(){1, 2})
			Dim l2_0 As INDArray = Nd4j.create(New Double(){1, 0}, New Long(){1, 2})

			'Correct, actual negative class -> TN
			Dim p3_1 As INDArray = Nd4j.create(New Double(){0.8, 0.2}, New Long(){1, 2})
			Dim l3_1 As INDArray = Nd4j.create(New Double(){1, 0}, New Long(){1, 2})
			Dim p3_0 As INDArray = Nd4j.create(New Double(){0.2, 0.8}, New Long(){1, 2})
			Dim l3_0 As INDArray = Nd4j.create(New Double(){0, 1}, New Long(){1, 2})

			'Incorrect, actual negative class -> FP
			Dim p4_1 As INDArray = Nd4j.create(New Double(){0.45, 0.55}, New Long(){1, 2})
			Dim l4_1 As INDArray = Nd4j.create(New Double(){1, 0}, New Long(){1, 2})
			Dim p4_0 As INDArray = Nd4j.create(New Double(){0.55, 0.45}, New Long(){1, 2})
			Dim l4_0 As INDArray = Nd4j.create(New Double(){0, 1}, New Long(){1, 2})

			Dim tp As Integer = 7
			Dim fn As Integer = 5
			Dim tn As Integer = 3
			Dim fp As Integer = 1
			For i As Integer = 0 To tp - 1
				ePosClass1_nOut2.eval(l1_1, p1_1)
				ePosClass1_nOut1.eval(l1_1.getColumn(1).reshape(ChrW(1), -1), p1_1.getColumn(1).reshape(ChrW(1), -1))
				ePosClass0_nOut2.eval(l1_0, p1_0)
				ePosClass0_nOut1.eval(l1_0.getColumn(1).reshape(ChrW(1), -1), p1_0.getColumn(1).reshape(ChrW(1), -1)) 'label 0 = instance of positive class

				ePosClassNull_nOut2.eval(l1_1, p1_1)
				ePosClassNull_nOut1.eval(l1_0.getColumn(0).reshape(ChrW(1), -1), p1_0.getColumn(0).reshape(ChrW(1), -1))
			Next i
			For i As Integer = 0 To fn - 1
				ePosClass1_nOut2.eval(l2_1, p2_1)
				ePosClass1_nOut1.eval(l2_1.getColumn(1).reshape(ChrW(1), -1), p2_1.getColumn(1).reshape(ChrW(1), -1))
				ePosClass0_nOut2.eval(l2_0, p2_0)
				ePosClass0_nOut1.eval(l2_0.getColumn(1).reshape(ChrW(1), -1), p2_0.getColumn(1).reshape(ChrW(1), -1))

				ePosClassNull_nOut2.eval(l2_1, p2_1)
				ePosClassNull_nOut1.eval(l2_0.getColumn(0).reshape(ChrW(1), -1), p2_0.getColumn(0).reshape(ChrW(1), -1))
			Next i
			For i As Integer = 0 To tn - 1
				ePosClass1_nOut2.eval(l3_1, p3_1)
				ePosClass1_nOut1.eval(l3_1.getColumn(1).reshape(ChrW(1), -1), p3_1.getColumn(1).reshape(ChrW(1), -1))
				ePosClass0_nOut2.eval(l3_0, p3_0)
				ePosClass0_nOut1.eval(l3_0.getColumn(1).reshape(ChrW(1), -1), p3_0.getColumn(1).reshape(ChrW(1), -1))

				ePosClassNull_nOut2.eval(l3_1, p3_1)
				ePosClassNull_nOut1.eval(l3_0.getColumn(0).reshape(ChrW(1), -1), p3_0.getColumn(0).reshape(ChrW(1), -1))
			Next i
			For i As Integer = 0 To fp - 1
				ePosClass1_nOut2.eval(l4_1, p4_1)
				ePosClass1_nOut1.eval(l4_1.getColumn(1).reshape(ChrW(1), -1), p4_1.getColumn(1).reshape(ChrW(1), -1))
				ePosClass0_nOut2.eval(l4_0, p4_0)
				ePosClass0_nOut1.eval(l4_0.getColumn(1).reshape(ChrW(1), -1), p4_0.getColumn(1).reshape(ChrW(1), -1))

				ePosClassNull_nOut2.eval(l4_1, p4_1)
				ePosClassNull_nOut1.eval(l4_0.getColumn(0).reshape(ChrW(1), -1), p4_0.getColumn(0).reshape(ChrW(1), -1))
			Next i

			For i As Integer = 0 To 3
				Dim positiveClass As Integer = posClass(i)
				Dim m As String = i.ToString()
				Dim tpAct As Integer = evals(i).truePositives()(positiveClass)
				Dim tnAct As Integer = evals(i).trueNegatives()(positiveClass)
				Dim fpAct As Integer = evals(i).falsePositives()(positiveClass)
				Dim fnAct As Integer = evals(i).falseNegatives()(positiveClass)

				'System.out.println(evals[i].stats());

				assertEquals(tp, tpAct,m)
				assertEquals(tn, tnAct,m)
				assertEquals(fp, fpAct,m)
				assertEquals(fn, fnAct,m)
			Next i

			Dim acc As Double = (tp+tn) / CDbl(tp+fn+tn+fp)
			Dim rec As Double = tp / CDbl(tp+fn)
			Dim prec As Double = tp / CDbl(tp+fp)
			Dim f1 As Double = 2 * (prec * rec) / (prec + rec)

			For i As Integer = 0 To evals.Length - 1
				Dim m As String = i.ToString()
				assertEquals(acc, evals(i).accuracy(), 1e-5,m)
				assertEquals(prec, evals(i).precision(), 1e-5,m)
				assertEquals(rec, evals(i).recall(), 1e-5,m)
				assertEquals(f1, evals(i).f1(), 1e-5,m)
			Next i

			'Also check macro-averaged versions (null positive class):
			assertEquals(acc, ePosClassNull_nOut2.accuracy(), 1e-6)
			assertEquals(ePosClass1_nOut2.recall(EvaluationAveraging.Macro), ePosClassNull_nOut2.recall(), 1e-6)
			assertEquals(ePosClass1_nOut2.precision(EvaluationAveraging.Macro), ePosClassNull_nOut2.precision(), 1e-6)
			assertEquals(ePosClass1_nOut2.f1(EvaluationAveraging.Macro), ePosClassNull_nOut2.f1(), 1e-6)

			assertEquals(acc, ePosClassNull_nOut1.accuracy(), 1e-6)
			assertEquals(ePosClass1_nOut2.recall(EvaluationAveraging.Macro), ePosClassNull_nOut1.recall(), 1e-6)
			assertEquals(ePosClass1_nOut2.precision(EvaluationAveraging.Macro), ePosClassNull_nOut1.precision(), 1e-6)
			assertEquals(ePosClass1_nOut2.f1(EvaluationAveraging.Macro), ePosClassNull_nOut1.f1(), 1e-6)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConfusionMatrixString()
		Public Overridable Sub testConfusionMatrixString()

			Dim e As New Evaluation(java.util.Arrays.asList("a","b","c"))

			Dim class0 As INDArray = Nd4j.create(New Double(){1, 0, 0}, New Long(){1, 3})
			Dim class1 As INDArray = Nd4j.create(New Double(){0, 1, 0}, New Long(){1, 3})
			Dim class2 As INDArray = Nd4j.create(New Double(){0, 0, 1}, New Long(){1, 3})

			'Predicted class 0, actual class 1 x2
			e.eval(class0, class1)
			e.eval(class0, class1)

			e.eval(class2, class2)
			e.eval(class2, class2)
			e.eval(class2, class2)

			Dim s As String = e.confusionMatrix()
	'        System.out.println(s);

			Dim exp As String = " 0 1 2" & vbLf & "-------" & vbLf & " 0 2 0 | 0 = a" & vbLf & " 0 0 0 | 1 = b" & vbLf & " 0 0 3 | 2 = c" & vbLf & vbLf & "Confusion matrix format: Actual (rowClass) predicted as (columnClass) N times"

			assertEquals(exp, s)

	'        System.out.println("============================");
	'        System.out.println(e.stats());
			e.stats()

	'        System.out.println("\n\n\n\n");

			'Test with 21 classes (> threshold)
			e = New Evaluation()
			class0 = Nd4j.create(1, 31)
			class0.putScalar(0, 1)

			e.eval(class0, class0)
	'        System.out.println(e.stats());
			e.stats()

	'        System.out.println("\n\n\n\n");
	'        System.out.println(e.stats(false, true));
			e.stats(False, True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationNaNs()
		Public Overridable Sub testEvaluationNaNs()

			Dim e As New Evaluation()
			Dim predictions As INDArray = Nd4j.create(New Double(){0.1, Double.NaN, 0.3}, New Long(){1, 3})
			Dim labels As INDArray = Nd4j.create(New Double(){0, 0, 1}, New Long(){1, 3})

			Try
				e.eval(labels, predictions)
			Catch ex As System.InvalidOperationException
				assertTrue(ex.Message.contains("NaN"))
			End Try

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentation()
		Public Overridable Sub testSegmentation()
			For Each c As Integer In New Integer(){4, 1} 'c=1 should be treated as binary classification case
				Nd4j.Random.setSeed(12345)
				Dim mb As Integer = 3
				Dim h As Integer = 3
				Dim w As Integer = 2

				'NCHW
				Dim labels As INDArray = Nd4j.create(DataType.FLOAT, mb, c, h, w)
				Dim r As New Random(12345)
				For i As Integer = 0 To mb - 1
					For j As Integer = 0 To h - 1
						For k As Integer = 0 To w - 1
							If c = 1 Then
								labels.putScalar(i, 0, j, k, r.Next(2))
							Else
								Dim classIdx As Integer = r.Next(c)
								labels.putScalar(i, classIdx, j, k, 1.0)
							End If
						Next k
					Next j
				Next i

				Dim predictions As INDArray = Nd4j.rand(DataType.FLOAT, mb, c, h, w)
				If c > 1 Then
					Dim op As DynamicCustomOp = DynamicCustomOp.builder("softmax").addInputs(predictions).addOutputs(predictions).callInplace(True).addIntegerArguments(1).build()
					Nd4j.exec(op)
				End If

				Dim e2d As New Evaluation()
				Dim e4d As New Evaluation()

				e4d.eval(labels, predictions)

				For i As Integer = 0 To mb - 1
					For j As Integer = 0 To h - 1
						For k As Integer = 0 To w - 1
							Dim rowLabel As INDArray = labels.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(j), NDArrayIndex.point(k))
							Dim rowPredictions As INDArray = predictions.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(j), NDArrayIndex.point(k))
							rowLabel = rowLabel.reshape(ChrW(1), rowLabel.length())
							rowPredictions = rowPredictions.reshape(ChrW(1), rowLabel.length())

							e2d.eval(rowLabel, rowPredictions)
						Next k
					Next j
				Next i

				assertEquals(e2d, e4d)


				'NHWC, etc
				Dim lOrig As INDArray = labels
				Dim fOrig As INDArray = predictions
				For i As Integer = 0 To 3
					Select Case i
						Case 0
							'CNHW - Never really used
							labels = lOrig.permute(1, 0, 2, 3).dup()
							predictions = fOrig.permute(1, 0, 2, 3).dup()
						Case 1
							'NCHW
							labels = lOrig
							predictions = fOrig
						Case 2
							'NHCW - Never really used...
							labels = lOrig.permute(0, 2, 1, 3).dup()
							predictions = fOrig.permute(0, 2, 1, 3).dup()
						Case 3
							'NHWC
							labels = lOrig.permute(0, 2, 3, 1).dup()
							predictions = fOrig.permute(0, 2, 3, 1).dup()
						Case Else
							Throw New Exception()
					End Select

					Dim e As New Evaluation()
					e.Axis = i

					e.eval(labels, predictions)
					assertEquals(e2d, e)
				Next i
			Next c
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLabelReset()
		Public Overridable Sub testLabelReset()

			Dim m As IDictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
			m(0) = "False"
			m(1) = "True"

			Dim e1 As New Evaluation(m)
			Dim zero As INDArray = Nd4j.create(New Double(){1, 0}).reshape(ChrW(1), 2)
			Dim one As INDArray = Nd4j.create(New Double(){0, 1}).reshape(ChrW(1), 2)

			e1.eval(zero, zero)
			e1.eval(zero, zero)
			e1.eval(one, zero)
			e1.eval(one, one)
			e1.eval(one, one)
			e1.eval(one, one)

			Dim s1 As String = e1.stats()
	'        System.out.println(s1);

			e1.reset()
			e1.eval(zero, zero)
			e1.eval(zero, zero)
			e1.eval(one, zero)
			e1.eval(one, one)
			e1.eval(one, one)
			e1.eval(one, one)

			Dim s2 As String = e1.stats()
			assertEquals(s1, s2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalStatsBinaryCase()
		Public Overridable Sub testEvalStatsBinaryCase()
			'Make sure we report class 1 precision/recall/f1 not macro averaged, for binary case

			Dim e As New Evaluation()

			Dim l0 As INDArray = Nd4j.createFromArray(New Double(){1, 0}).reshape(ChrW(1), 2)
			Dim l1 As INDArray = Nd4j.createFromArray(New Double(){0, 1}).reshape(ChrW(1), 2)

			e.eval(l1, l1)
			e.eval(l1, l1)
			e.eval(l1, l1)
			e.eval(l0, l0)
			e.eval(l1, l0)
			e.eval(l1, l0)
			e.eval(l0, l1)

			Dim tp As Double = 3
			Dim fp As Double = 1
			Dim fn As Double = 2

			Dim prec As Double = tp / (tp + fp)
			Dim rec As Double = tp / (tp + fn)
			Dim f1 As Double = 2 * prec * rec / (prec + rec)

			assertEquals(prec, e.precision(), 1e-6)
			assertEquals(rec, e.recall(), 1e-6)

			Dim df As New DecimalFormat("0.0000")

			Dim stats As String = e.stats()
			'System.out.println(stats);

			Dim stats2 As String = stats.replaceAll("( )+", " ")

			Dim recS As String = " Recall: " & df.format(rec)
			Dim preS As String = " Precision: " & df.format(prec)
			Dim f1S As String = "F1 Score: " & df.format(f1)

			assertTrue(stats2.Contains(recS),stats2)
			assertTrue(stats2.Contains(preS),stats2)
			assertTrue(stats2.Contains(f1S),stats2)
		End Sub
	End Class

End Namespace