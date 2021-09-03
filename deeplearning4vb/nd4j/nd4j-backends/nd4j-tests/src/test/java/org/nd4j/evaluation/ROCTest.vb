Imports System
Imports System.Collections.Generic
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCBinary = org.nd4j.evaluation.classification.ROCBinary
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports PrecisionRecallCurve = org.nd4j.evaluation.curves.PrecisionRecallCurve
Imports RocCurve = org.nd4j.evaluation.curves.RocCurve
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) @NativeTag public class ROCTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ROCTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		Private Shared expTPR As IDictionary(Of Double, Double)
		Private Shared expFPR As IDictionary(Of Double, Double)

		Shared Sub New()
			expTPR = New Dictionary(Of Double, Double)()
			Dim totalPositives As Double = 5.0
			expTPR(0 / 10.0) = 5.0 / totalPositives 'All 10 predicted as class 1, of which 5 of 5 are correct
			expTPR(1 / 10.0) = 5.0 / totalPositives
			expTPR(2 / 10.0) = 5.0 / totalPositives
			expTPR(3 / 10.0) = 5.0 / totalPositives
			expTPR(4 / 10.0) = 5.0 / totalPositives
			expTPR(5 / 10.0) = 5.0 / totalPositives
			expTPR(6 / 10.0) = 4.0 / totalPositives 'Threshold: 0.4 -> last 4 predicted; last 5 actual
			expTPR(7 / 10.0) = 3.0 / totalPositives
			expTPR(8 / 10.0) = 2.0 / totalPositives
			expTPR(9 / 10.0) = 1.0 / totalPositives
			expTPR(10 / 10.0) = 0.0 / totalPositives

			expFPR = New Dictionary(Of Double, Double)()
			Dim totalNegatives As Double = 5.0
			expFPR(0 / 10.0) = 5.0 / totalNegatives 'All 10 predicted as class 1, but all 5 true negatives are predicted positive
			expFPR(1 / 10.0) = 4.0 / totalNegatives '1 true negative is predicted as negative; 4 false positives
			expFPR(2 / 10.0) = 3.0 / totalNegatives '2 true negatives are predicted as negative; 3 false positives
			expFPR(3 / 10.0) = 2.0 / totalNegatives
			expFPR(4 / 10.0) = 1.0 / totalNegatives
			expFPR(5 / 10.0) = 0.0 / totalNegatives
			expFPR(6 / 10.0) = 0.0 / totalNegatives
			expFPR(7 / 10.0) = 0.0 / totalNegatives
			expFPR(8 / 10.0) = 0.0 / totalNegatives
			expFPR(9 / 10.0) = 0.0 / totalNegatives
			expFPR(10 / 10.0) = 0.0 / totalNegatives
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRocBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRocBasic(ByVal backend As Nd4jBackend)
			'2 outputs here - probability distribution over classes (softmax)
			Dim predictions As INDArray = Nd4j.create(New Double()() {
				New Double() {1.0, 0.001},
				New Double() {0.899, 0.101},
				New Double() {0.799, 0.201},
				New Double() {0.699, 0.301},
				New Double() {0.599, 0.401},
				New Double() {0.499, 0.501},
				New Double() {0.399, 0.601},
				New Double() {0.299, 0.701},
				New Double() {0.199, 0.801},
				New Double() {0.099, 0.901}
			})

			Dim actual As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1}
			})

			Dim roc As New ROC(10)
			roc.eval(actual, predictions)

			Dim rocCurve As RocCurve = roc.RocCurve

			assertEquals(11, rocCurve.getThreshold().length) '0 + 10 steps
			For i As Integer = 0 To 10
				Dim expThreshold As Double = i / 10.0
				assertEquals(expThreshold, rocCurve.getThreshold(i), 1e-5)

				'            System.out.println("t=" + expThreshold + "\t" + v.getFalsePositiveRate() + "\t" + v.getTruePositiveRate());

				Dim efpr As Double = expFPR(expThreshold)
				Dim afpr As Double = rocCurve.getFalsePositiveRate(i)
				assertEquals(efpr, afpr, 1e-5)

				Dim etpr As Double = expTPR(expThreshold)
				Dim atpr As Double = rocCurve.getTruePositiveRate(i)
				assertEquals(etpr, atpr, 1e-5)
			Next i


			'Expect AUC == 1.0 here
			Dim auc As Double = roc.calculateAUC()
			assertEquals(1.0, auc, 1e-6)

			' testing reset now
			roc.reset()
			roc.eval(actual, predictions)
			auc = roc.calculateAUC()
			assertEquals(1.0, auc, 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRocBasicSingleClass(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRocBasicSingleClass(ByVal backend As Nd4jBackend)
			'1 output here - single probability value (sigmoid)

			'add 0.001 to avoid numerical/rounding issues (float vs. double, etc)
			Dim predictions As INDArray = Nd4j.create(New Double() {0.001, 0.101, 0.201, 0.301, 0.401, 0.501, 0.601, 0.701, 0.801, 0.901}, New Integer() {10, 1})

			Dim actual As INDArray = Nd4j.create(New Double() {0, 0, 0, 0, 0, 1, 1, 1, 1, 1}, New Integer() {10, 1})

			Dim roc As New ROC(10)
			roc.eval(actual, predictions)

			Dim rocCurve As RocCurve = roc.RocCurve

			assertEquals(11, rocCurve.getThreshold().length) '0 + 10 steps
			For i As Integer = 0 To 10
				Dim expThreshold As Double = i / 10.0
				assertEquals(expThreshold, rocCurve.getThreshold(i), 1e-5)

				'            System.out.println("t=" + expThreshold + "\t" + v.getFalsePositiveRate() + "\t" + v.getTruePositiveRate());

				Dim efpr As Double = expFPR(expThreshold)
				Dim afpr As Double = rocCurve.getFalsePositiveRate(i)
				assertEquals(efpr, afpr, 1e-5)

				Dim etpr As Double = expTPR(expThreshold)
				Dim atpr As Double = rocCurve.getTruePositiveRate(i)
				assertEquals(etpr, atpr, 1e-5)
			Next i

			'Expect AUC == 1.0 here
			Dim auc As Double = roc.calculateAUC()
			assertEquals(1.0, auc, 1e-6)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRoc(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRoc(ByVal backend As Nd4jBackend)
			'Previous tests allowed for a perfect classifier with right threshold...

			Dim labels As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0}
			})

			Dim prediction As INDArray = Nd4j.create(New Double()() {
				New Double() {0.199, 0.801},
				New Double() {0.499, 0.501},
				New Double() {0.399, 0.601},
				New Double() {0.799, 0.201},
				New Double() {0.899, 0.101}
			})

			Dim expTPR As IDictionary(Of Double, Double) = New Dictionary(Of Double, Double)()
			Dim totalPositives As Double = 2.0
			expTPR(0.0) = 2.0 / totalPositives 'All predicted class 1 -> 2 true positives / 2 total positives
			expTPR(0.1) = 2.0 / totalPositives
			expTPR(0.2) = 2.0 / totalPositives
			expTPR(0.3) = 2.0 / totalPositives
			expTPR(0.4) = 2.0 / totalPositives
			expTPR(0.5) = 2.0 / totalPositives
			expTPR(0.6) = 1.0 / totalPositives 'At threshold of 0.6, only 1 of 2 positives are predicted positive
			expTPR(0.7) = 1.0 / totalPositives
			expTPR(0.8) = 1.0 / totalPositives
			expTPR(0.9) = 0.0 / totalPositives 'At threshold of 0.9, 0 of 2 positives are predicted positive
			expTPR(1.0) = 0.0 / totalPositives

			Dim expFPR As IDictionary(Of Double, Double) = New Dictionary(Of Double, Double)()
			Dim totalNegatives As Double = 3.0
			expFPR(0.0) = 3.0 / totalNegatives 'All predicted class 1 -> 3 false positives / 3 total negatives
			expFPR(0.1) = 3.0 / totalNegatives
			expFPR(0.2) = 2.0 / totalNegatives 'At threshold of 0.2: 1 true negative, 2 false positives
			expFPR(0.3) = 1.0 / totalNegatives 'At threshold of 0.3: 2 true negative, 1 false positive
			expFPR(0.4) = 1.0 / totalNegatives
			expFPR(0.5) = 1.0 / totalNegatives
			expFPR(0.6) = 1.0 / totalNegatives
			expFPR(0.7) = 0.0 / totalNegatives 'At threshold of 0.7: 3 true negatives, 0 false positives
			expFPR(0.8) = 0.0 / totalNegatives
			expFPR(0.9) = 0.0 / totalNegatives
			expFPR(1.0) = 0.0 / totalNegatives

			Dim expTPs() As Integer = {2, 2, 2, 2, 2, 2, 1, 1, 1, 0, 0}
			Dim expFPs() As Integer = {3, 3, 2, 1, 1, 1, 1, 0, 0, 0, 0}
			Dim expFNs(10) As Integer
			Dim expTNs(10) As Integer
			For i As Integer = 0 To 10
				expFNs(i) = CInt(Math.Truncate(totalPositives)) - expTPs(i)
				expTNs(i) = 5 - expTPs(i) - expFPs(i) - expFNs(i)
			Next i

			Dim roc As New ROC(10)
			roc.eval(labels, prediction)

			Dim rocCurve As RocCurve = roc.RocCurve

			assertEquals(11, rocCurve.getThreshold().length)
			assertEquals(11, rocCurve.getFpr().length)
			assertEquals(11, rocCurve.getTpr().length)

			For i As Integer = 0 To 10
				Dim expThreshold As Double = i / 10.0
				assertEquals(expThreshold, rocCurve.getThreshold(i), 1e-5)

				Dim efpr As Double = expFPR(expThreshold)
				Dim afpr As Double = rocCurve.getFalsePositiveRate(i)
				assertEquals(efpr, afpr, 1e-5)

				Dim etpr As Double = expTPR(expThreshold)
				Dim atpr As Double = rocCurve.getTruePositiveRate(i)
				assertEquals(etpr, atpr, 1e-5)
			Next i

			'AUC: expected values are based on plotting the ROC curve and manually calculating the area
			Dim expAUC As Double = 0.5 * 1.0 / 3.0 + (1 - 1 / 3.0) * 1.0
			Dim actAUC As Double = roc.calculateAUC()

			assertEquals(expAUC, actAUC, 1e-6)

			Dim prc As PrecisionRecallCurve = roc.PrecisionRecallCurve
			For i As Integer = 0 To 10
				Dim c As PrecisionRecallCurve.Confusion = prc.getConfusionMatrixAtThreshold(i * 0.1)
				assertEquals(expTPs(i), c.getTpCount())
				assertEquals(expFPs(i), c.getFpCount())
				assertEquals(expFPs(i), c.getFpCount())
				assertEquals(expTNs(i), c.getTnCount())
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRocTimeSeriesNoMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRocTimeSeriesNoMasking(ByVal backend As Nd4jBackend)
			'Same as first test...

			'2 outputs here - probability distribution over classes (softmax)
			Dim predictions2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1.0, 0.001},
				New Double() {0.899, 0.101},
				New Double() {0.799, 0.201},
				New Double() {0.699, 0.301},
				New Double() {0.599, 0.401},
				New Double() {0.499, 0.501},
				New Double() {0.399, 0.601},
				New Double() {0.299, 0.701},
				New Double() {0.199, 0.801},
				New Double() {0.099, 0.901}
			})

			Dim actual2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1}
			})

			Dim predictions3d As INDArray = Nd4j.create(2, 2, 5)
			Dim firstTSp As INDArray = predictions3d.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all()).transpose()
			assertArrayEquals(New Long() {5, 2}, firstTSp.shape())
			firstTSp.assign(predictions2d.get(NDArrayIndex.interval(0, 5), NDArrayIndex.all()))

			Dim secondTSp As INDArray = predictions3d.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all()).transpose()
			assertArrayEquals(New Long() {5, 2}, secondTSp.shape())
			secondTSp.assign(predictions2d.get(NDArrayIndex.interval(5, 10), NDArrayIndex.all()))

			Dim labels3d As INDArray = Nd4j.create(2, 2, 5)
			Dim firstTS As INDArray = labels3d.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.all()).transpose()
			assertArrayEquals(New Long() {5, 2}, firstTS.shape())
			firstTS.assign(actual2d.get(NDArrayIndex.interval(0, 5), NDArrayIndex.all()))

			Dim secondTS As INDArray = labels3d.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all()).transpose()
			assertArrayEquals(New Long() {5, 2}, secondTS.shape())
			secondTS.assign(actual2d.get(NDArrayIndex.interval(5, 10), NDArrayIndex.all()))

			For Each steps As Integer In New Integer() {10, 0} '0 steps: exact
				'            System.out.println("Steps: " + steps);
				Dim rocExp As New ROC(steps)
				rocExp.eval(actual2d, predictions2d)

				Dim rocAct As New ROC(steps)
				rocAct.evalTimeSeries(labels3d, predictions3d)

				assertEquals(rocExp.calculateAUC(), rocAct.calculateAUC(), 1e-6)
				assertEquals(rocExp.calculateAUCPR(), rocAct.calculateAUCPR(), 1e-6)

				assertEquals(rocExp.RocCurve, rocAct.RocCurve)
			Next steps
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRocTimeSeriesMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRocTimeSeriesMasking(ByVal backend As Nd4jBackend)
			'2 outputs here - probability distribution over classes (softmax)
			Dim predictions2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1.0, 0.001},
				New Double() {0.899, 0.101},
				New Double() {0.799, 0.201},
				New Double() {0.699, 0.301},
				New Double() {0.599, 0.401},
				New Double() {0.499, 0.501},
				New Double() {0.399, 0.601},
				New Double() {0.299, 0.701},
				New Double() {0.199, 0.801},
				New Double() {0.099, 0.901}
			})

			Dim actual2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {1, 0},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1},
				New Double() {0, 1}
			})


			'Create time series data... first time series: length 4. Second time series: length 6
			Dim predictions3d As INDArray = Nd4j.create(2, 2, 6)
			Dim tad As INDArray = predictions3d.tensorAlongDimension(0, 1, 2).transpose()
			tad.get(NDArrayIndex.interval(0, 4), NDArrayIndex.all()).assign(predictions2d.get(NDArrayIndex.interval(0, 4), NDArrayIndex.all()))

			tad = predictions3d.tensorAlongDimension(1, 1, 2).transpose()
			tad.assign(predictions2d.get(NDArrayIndex.interval(4, 10), NDArrayIndex.all()))


			Dim labels3d As INDArray = Nd4j.create(2, 2, 6)
			tad = labels3d.tensorAlongDimension(0, 1, 2).transpose()
			tad.get(NDArrayIndex.interval(0, 4), NDArrayIndex.all()).assign(actual2d.get(NDArrayIndex.interval(0, 4), NDArrayIndex.all()))

			tad = labels3d.tensorAlongDimension(1, 1, 2).transpose()
			tad.assign(actual2d.get(NDArrayIndex.interval(4, 10), NDArrayIndex.all()))


			Dim mask As INDArray = Nd4j.zeros(2, 6)
			mask.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 4)).assign(1)
			mask.get(NDArrayIndex.point(1), NDArrayIndex.all()).assign(1)


			For Each steps As Integer In New Integer() {20, 0} '0 steps: exact
				Dim rocExp As New ROC(steps)
				rocExp.eval(actual2d, predictions2d)

				Dim rocAct As New ROC(steps)
				rocAct.evalTimeSeries(labels3d, predictions3d, mask)

				assertEquals(rocExp.calculateAUC(), rocAct.calculateAUC(), 1e-6)

				assertEquals(rocExp.RocCurve, rocAct.RocCurve)
			Next steps
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCompareRocAndRocMultiClass(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCompareRocAndRocMultiClass(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			'For 2 class case: ROC and Multi-class ROC should be the same...
			Dim nExamples As Integer = 200
			Dim predictions As INDArray = Nd4j.rand(nExamples, 2)
			Dim tempSum As INDArray = predictions.sum(1)
			predictions.diviColumnVector(tempSum)

			Dim labels As INDArray = Nd4j.create(nExamples, 2)
			Dim r As New Random(12345)
			For i As Integer = 0 To nExamples - 1
				labels.putScalar(i, r.Next(2), 1.0)
			Next i

			For Each numSteps As Integer In New Integer() {30, 0} 'Steps = 0: exact
				Dim roc As New ROC(numSteps)
				roc.eval(labels, predictions)

				Dim rocMultiClass As New ROCMultiClass(numSteps)
				rocMultiClass.eval(labels, predictions)

				Dim auc As Double = roc.calculateAUC()
				Dim auc1 As Double = rocMultiClass.calculateAUC(1)

				assertEquals(auc, auc1, 1e-6)
			Next numSteps
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCompare2Vs3Classes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCompare2Vs3Classes(ByVal backend As Nd4jBackend)

			'ROC multi-class: 2 vs. 3 classes should be the same, if we add two of the classes together...
			'Both methods implement one vs. all ROC/AUC in different ways

			Dim nExamples As Integer = 200
			Dim predictions3 As INDArray = Nd4j.rand(nExamples, 3)
			Dim tempSum As INDArray = predictions3.sum(1)
			predictions3.diviColumnVector(tempSum)

			Dim labels3 As INDArray = Nd4j.create(nExamples, 3)
			Dim r As New Random(12345)
			For i As Integer = 0 To nExamples - 1
				labels3.putScalar(i, r.Next(3), 1.0)
			Next i

			Dim predictions2 As INDArray = Nd4j.zeros(nExamples, 2)
			predictions2.getColumn(0).assign(predictions3.getColumn(0))
			predictions2.getColumn(0).addi(predictions3.getColumn(1))
			predictions2.getColumn(1).addi(predictions3.getColumn(2))

			Dim labels2 As INDArray = Nd4j.zeros(nExamples, 2)
			labels2.getColumn(0).assign(labels3.getColumn(0))
			labels2.getColumn(0).addi(labels3.getColumn(1))
			labels2.getColumn(1).addi(labels3.getColumn(2))

			For Each numSteps As Integer In New Integer() {30, 0} 'Steps = 0: exact

				Dim rocMultiClass3 As New ROCMultiClass(numSteps)
				Dim rocMultiClass2 As New ROCMultiClass(numSteps)

				rocMultiClass3.eval(labels3, predictions3)
				rocMultiClass2.eval(labels2, predictions2)

				Dim auc3 As Double = rocMultiClass3.calculateAUC(2)
				Dim auc2 As Double = rocMultiClass2.calculateAUC(1)

				assertEquals(auc2, auc3, 1e-6)

				Dim c3 As RocCurve = rocMultiClass3.getRocCurve(2)
				Dim c2 As RocCurve = rocMultiClass2.getRocCurve(1)

				assertArrayEquals(c2.getThreshold(), c3.getThreshold(), 1e-6)
				assertArrayEquals(c2.getFpr(), c3.getFpr(), 1e-6)
				assertArrayEquals(c2.getTpr(), c3.getTpr(), 1e-6)
			Next numSteps
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testROCMerging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCMerging(ByVal backend As Nd4jBackend)
			Dim nArrays As Integer = 10
			Dim minibatch As Integer = 64
			Dim nROCs As Integer = 3

			For Each steps As Integer In New Integer() {0, 20} '0 steps: exact, 20 steps: thresholded

				Nd4j.Random.setSeed(12345)
				Dim r As New Random(12345)

				Dim rocList As IList(Of ROC) = New List(Of ROC)()
				For i As Integer = 0 To nROCs - 1
					rocList.Add(New ROC(steps))
				Next i

				Dim [single] As New ROC(steps)
				For i As Integer = 0 To nArrays - 1
					Dim p As INDArray = Nd4j.rand(minibatch, 2)
					p.diviColumnVector(p.sum(1))

					Dim l As INDArray = Nd4j.zeros(minibatch, 2)
					For j As Integer = 0 To minibatch - 1
						l.putScalar(j, r.Next(2), 1.0)
					Next j

					[single].eval(l, p)

					Dim other As ROC = rocList(i Mod rocList.Count)
					other.eval(l, p)
				Next i

				Dim first As ROC = rocList(0)
				For i As Integer = 1 To nROCs - 1
					first.merge(rocList(i))
				Next i

				Dim singleAUC As Double = [single].calculateAUC()
				assertTrue(singleAUC >= 0.0 AndAlso singleAUC <= 1.0)
				assertEquals(singleAUC, first.calculateAUC(), 1e-6)

				assertEquals([single].RocCurve, first.RocCurve)
			Next steps
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testROCMerging2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCMerging2(ByVal backend As Nd4jBackend)
			Dim nArrays As Integer = 10
			Dim minibatch As Integer = 64
			Dim exactAllocBlockSize As Integer = 10
			Dim nROCs As Integer = 3
			Dim steps As Integer = 0 'Exact

			Nd4j.Random.setSeed(12345)
			Dim r As New Random(12345)

			Dim rocList As IList(Of ROC) = New List(Of ROC)()
			For i As Integer = 0 To nROCs - 1
				rocList.Add(New ROC(steps, True, exactAllocBlockSize))
			Next i

			Dim [single] As New ROC(steps)
			For i As Integer = 0 To nArrays - 1
				Dim p As INDArray = Nd4j.rand(minibatch, 2)
				p.diviColumnVector(p.sum(1))

				Dim l As INDArray = Nd4j.zeros(minibatch, 2)
				For j As Integer = 0 To minibatch - 1
					l.putScalar(j, r.Next(2), 1.0)
				Next j

				[single].eval(l, p)

				Dim other As ROC = rocList(i Mod rocList.Count)
				other.eval(l, p)
			Next i

			Dim first As ROC = rocList(0)
			For i As Integer = 1 To nROCs - 1
				first.merge(rocList(i))
			Next i

			Dim singleAUC As Double = [single].calculateAUC()
			assertTrue(singleAUC >= 0.0 AndAlso singleAUC <= 1.0)
			assertEquals(singleAUC, first.calculateAUC(), 1e-6)

			assertEquals([single].RocCurve, first.RocCurve)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testROCMultiMerging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCMultiMerging(ByVal backend As Nd4jBackend)

			Dim nArrays As Integer = 10
			Dim minibatch As Integer = 64
			Dim nROCs As Integer = 3
			Dim nClasses As Integer = 3

			For Each steps As Integer In New Integer() {20, 0} '0 steps: exact
				'            int steps = 20;

				Nd4j.Random.setSeed(12345)
				Dim r As New Random(12345)

				Dim rocList As IList(Of ROCMultiClass) = New List(Of ROCMultiClass)()
				For i As Integer = 0 To nROCs - 1
					rocList.Add(New ROCMultiClass(steps))
				Next i

				Dim [single] As New ROCMultiClass(steps)
				For i As Integer = 0 To nArrays - 1
					Dim p As INDArray = Nd4j.rand(minibatch, nClasses)
					p.diviColumnVector(p.sum(1))

					Dim l As INDArray = Nd4j.zeros(minibatch, nClasses)
					For j As Integer = 0 To minibatch - 1
						l.putScalar(j, r.Next(nClasses), 1.0)
					Next j

					[single].eval(l, p)

					Dim other As ROCMultiClass = rocList(i Mod rocList.Count)
					other.eval(l, p)
				Next i

				Dim first As ROCMultiClass = rocList(0)
				For i As Integer = 1 To nROCs - 1
					first.merge(rocList(i))
				Next i

				For i As Integer = 0 To nClasses - 1
					assertEquals([single].calculateAUC(i), first.calculateAUC(i), 1e-6)

					assertEquals([single].getRocCurve(i), first.getRocCurve(i))
				Next i
			Next steps
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAUCPrecisionRecall(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAUCPrecisionRecall(ByVal backend As Nd4jBackend)
			'Assume 2 positive examples, at 0.33 and 0.66 predicted, 1 negative example at 0.25 prob
			'at threshold 0 to 0.24999: tp=2, fp=1, fn=0, tn=0 prec=2/(2+1)=0.666, recall=2/2=1.0
			'at threshold 0.25 to 0.33: tp=2, fp=0, fn=0, tn=1 prec=2/2=1, recall=2/2=1
			'at threshold 0.331 to 0.66: tp=1, fp=0, fn=1, tn=1 prec=1/1=1, recall=1/2=0.5
			'at threshold 0.661 to 1.0:  tp=0, fp=0, fn=2, tn=1 prec=0/0=1, recall=0/2=0

			For Each steps As Integer In New Integer() {10, 0} '0 steps = exact
				'        for (int steps : new int[] {0}) { //0 steps = exact
				Dim msg As String = "Steps = " & steps
				'area: 1.0
				Dim r As New ROC(steps)
				Dim zero As INDArray = Nd4j.zeros(1,1)
				Dim one As INDArray = Nd4j.ones(1,1)
				r.eval(zero, Nd4j.create(New Double() {0.25}).reshape(ChrW(1), 1))
				r.eval(one, Nd4j.create(New Double() {0.33}).reshape(ChrW(1), 1))
				r.eval(one, Nd4j.create(New Double() {0.66}).reshape(ChrW(1), 1))

				Dim prc As PrecisionRecallCurve = r.PrecisionRecallCurve

				Dim auprc As Double = r.calculateAUCPR()
				assertEquals(1.0, auprc, 1e-6,msg)

				'Assume 2 positive examples, at 0.33 and 0.66 predicted, 1 negative example at 0.5 prob
				'at threshold 0 to 0.33: tp=2, fp=1, fn=0, tn=0 prec=2/(2+1)=0.666, recall=2/2=1.0
				'at threshold 0.331 to 0.5: tp=1, fp=1, fn=1, tn=0 prec=1/2=0.5, recall=1/2=0.5
				'at threshold 0.51 to 0.66: tp=1, fp=0, fn=1, tn=1 prec=1/1=1, recall=1/2=0.5
				'at threshold 0.661 to 1.0:  tp=0, fp=0, fn=2, tn=1 prec=0/0=1, recall=0/2=0
				'Area: 0.5 + 0.25 + 0.5*0.5*(0.66666-0.5) = 0.5+0.25+0.04165 = 0.7916666666667
				'But, we use 10 steps so the calculation might not match this exactly, but should be close
				r = New ROC(steps)
				r.eval(one, Nd4j.create(New Double() {0.33}))
				r.eval(zero, Nd4j.create(New Double() {0.5}))
				r.eval(one, Nd4j.create(New Double() {0.66}))

				Dim precision As Double
				If steps = 0 Then
					precision = 1e-8
				Else
					precision = 1e-4
				End If
				assertEquals(0.7916666666667, r.calculateAUCPR(), precision,msg)
			Next steps
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRocAucExact(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRocAucExact(ByVal backend As Nd4jBackend)

			'Check the implementation vs. Scikitlearn
	'        
	'        np.random.seed(12345)
	'        prob = np.random.rand(30,1)
	'        label = np.random.randint(0,2,(30,1))
	'        positiveClass = 1;
	'        
	'        fpr, tpr, thr = sklearn.metrics.roc_curve(label, prob, positiveClass, None, False)
	'        auc = sklearn.metrics.auc(fpr, tpr)
	'        
	'        #PR curve
	'        p, r, t = precision_recall_curve(label, prob, positiveClass)
	'        
	'        #sklearn.metrics.average_precision_score: http://scikit-learn.org/stable/modules/generated/sklearn.metrics.average_precision_score.html
	'        # "This score corresponds to the area under the precision-recall curve."
	'        auprc = sklearn.metrics.average_precision_score(label, prob)
	'        print(auprc)
	'        
	'        fpr
	'        [ 0.          0.15789474  0.15789474  0.31578947  0.31578947  0.52631579
	'          0.52631579  0.68421053  0.68421053  0.84210526  0.84210526  0.89473684
	'          0.89473684  1.        ]
	'        tpr
	'        [ 0.09090909  0.09090909  0.18181818  0.18181818  0.36363636  0.36363636
	'          0.45454545  0.45454545  0.72727273  0.72727273  0.90909091  0.90909091
	'          1.          1.        ]
	'        threshold
	'        [ 0.99401459  0.96130674  0.92961609  0.79082252  0.74771481  0.67687371
	'          0.65641118  0.64247533  0.46759901  0.31637555  0.20456028  0.18391881
	'          0.17091426  0.0083883 ]
	'        
	'        p, r, t = precision_recall_curve(label, prob)
	'        
	'        Precision
	'        [ 0.39285714  0.37037037  0.38461538  0.36        0.33333333  0.34782609
	'          0.36363636  0.38095238  0.35        0.31578947  0.27777778  0.29411765
	'          0.3125      0.33333333  0.28571429  0.30769231  0.33333333  0.36363636
	'          0.4         0.33333333  0.25        0.28571429  0.33333333  0.4         0.25
	'          0.33333333  0.5         1.          1.        ]
	'        Recall
	'        [ 1.          0.90909091  0.90909091  0.81818182  0.72727273  0.72727273
	'          0.72727273  0.72727273  0.63636364  0.54545455  0.45454545  0.45454545
	'          0.45454545  0.45454545  0.36363636  0.36363636  0.36363636  0.36363636
	'          0.36363636  0.27272727  0.18181818  0.18181818  0.18181818  0.18181818
	'          0.09090909  0.09090909  0.09090909  0.09090909  0.        ]
	'        Threshold
	'        [ 0.17091426  0.18391881  0.20456028  0.29870371  0.31637555  0.32558468
	'          0.43964461  0.46759901  0.56772503  0.5955447   0.64247533  0.6531771
	'          0.65356987  0.65641118  0.67687371  0.71745362  0.72368535  0.72968908
	'          0.74771481  0.74890664  0.79082252  0.80981255  0.87217591  0.92961609
	'          0.96130674  0.96451452  0.9646476   0.99401459]
	'        
	'        AUPRC
	'        0.398963619227
	'         

			Dim p() As Double = {0.92961609, 0.31637555, 0.18391881, 0.20456028, 0.56772503, 0.5955447, 0.96451452, 0.6531771, 0.74890664, 0.65356987, 0.74771481, 0.96130674, 0.0083883, 0.10644438, 0.29870371, 0.65641118, 0.80981255, 0.87217591, 0.9646476, 0.72368535, 0.64247533, 0.71745362, 0.46759901, 0.32558468, 0.43964461, 0.72968908, 0.99401459, 0.67687371, 0.79082252, 0.17091426}

			Dim l() As Double = {1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1}

			Dim fpr_skl() As Double = {0.0, 0.0, 0.15789474, 0.15789474, 0.31578947, 0.31578947, 0.52631579, 0.52631579, 0.68421053, 0.68421053, 0.84210526, 0.84210526, 0.89473684, 0.89473684, 1.0}
			Dim tpr_skl() As Double = {0.0, 0.09090909, 0.09090909, 0.18181818, 0.18181818, 0.36363636, 0.36363636, 0.45454545, 0.45454545, 0.72727273, 0.72727273, 0.90909091, 0.90909091, 1.0, 1.0}
			'Note the change to the last value: same TPR and FPR at 0.0083883 and 0.0 -> we add the 0.0 threshold edge case + combine with the previous one. Same result
			Dim thr_skl() As Double = {1.0, 0.99401459, 0.96130674, 0.92961609, 0.79082252, 0.74771481, 0.67687371, 0.65641118, 0.64247533, 0.46759901, 0.31637555, 0.20456028, 0.18391881, 0.17091426, 0.0}

			Dim prob As INDArray = Nd4j.create(p, New Integer() {30, 1})
			Dim label As INDArray = Nd4j.create(l, New Integer() {30, 1})

			Dim roc As New ROC(0)
			roc.eval(label, prob)

			Dim rocCurve As RocCurve = roc.RocCurve

			'        System.out.println("Thr: " + Arrays.toString(rocCurve[0]));
			'        System.out.println("FPR: " + Arrays.toString(rocCurve[1]));
			'        System.out.println("TPR: " + Arrays.toString(rocCurve[2]));
			'        System.out.println("AUC: " + roc.calculateAUC());

			assertArrayEquals(thr_skl, rocCurve.getThreshold(), 1e-6)
			assertArrayEquals(fpr_skl, rocCurve.getFpr(), 1e-6)
			assertArrayEquals(tpr_skl, rocCurve.getTpr(), 1e-6)

			Dim auc As Double = roc.calculateAUC()
			Dim aucExpSKL As Double = 0.459330143541
			assertEquals(aucExpSKL, auc, 1e-6)

			roc = New ROC(0, False)
			roc.eval(label, prob)
			assertEquals(aucExpSKL, roc.calculateAUC(), 1e-6)



			'Check area under PR curve
			roc = New ROC(0, True)
			roc.eval(label, prob)

			'Unfortunately some of the sklearn points are redundant... and they are missing the edge cases.
			' so a direct element-by-element comparison is not possible, unlike in the ROC case

			Dim auprcExp As Double = 0.398963619227
			Dim auprcAct As Double = roc.calculateAUCPR()
			assertEquals(auprcExp, auprcAct, 1e-8)

			roc = New ROC(0, False)
			roc.eval(label, prob)
			assertEquals(auprcExp, roc.calculateAUCPR(), 1e-8)


			'Check precision recall curve counts etc
			Dim prc As PrecisionRecallCurve = roc.PrecisionRecallCurve
			For i As Integer = 0 To thr_skl.Length - 1
				Dim threshold As Double = thr_skl(i) - 1e-6 'Subtract a bit, so we get the correct point (rounded up on the get op)
				threshold = Math.Max(0.0, threshold)
				Dim c As PrecisionRecallCurve.Confusion = prc.getConfusionMatrixAtThreshold(threshold)
				Dim tp As Integer = c.getTpCount()
				Dim fp As Integer = c.getFpCount()
				Dim tn As Integer = c.getTnCount()
				Dim fn As Integer = c.getFnCount()

				assertEquals(30, tp + fp + tn + fn)

				Dim prec As Double = tp / CDbl(tp + fp)
				Dim rec As Double = tp / CDbl(tp + fn)
				Dim fpr As Double = fp / 19.0

				If c.getPoint().getThreshold() = 0.0 Then
					rec = 1.0
					prec = 11.0 / 30 '11 positives, 30 total
				ElseIf c.getPoint().getThreshold() = 1.0 Then
					rec = 0.0
					prec = 1.0
				End If

				'            System.out.println(i + "\t" + threshold);
				assertEquals(tpr_skl(i), rec, 1e-6)
				assertEquals(fpr_skl(i), fpr, 1e-6)

				assertEquals(rec, c.getPoint().getRecall(), 1e-6)
				assertEquals(prec, c.getPoint().getPrecision(), 1e-6)
			Next i


			'Check edge case: perfect classifier
			prob = Nd4j.create(New Double() {0.1, 0.2, 0.5, 0.9}, New Integer() {4, 1})
			label = Nd4j.create(New Double() {0, 0, 1, 1}, New Integer() {4, 1})
			roc = New ROC(0)
			roc.eval(label, prob)
			assertEquals(1.0, roc.calculateAUC(), 1e-8)

			assertEquals(1.0, roc.calculateAUCPR(), 1e-8)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void rocExactEdgeCaseReallocation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub rocExactEdgeCaseReallocation(ByVal backend As Nd4jBackend)

			'Set reallocation block size to say 20, but then evaluate a 100-length array

			Dim roc As New ROC(0, True, 50)

			roc.eval(Nd4j.rand(100, 1), Nd4j.ones(100, 1))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPrecisionRecallCurveGetPointMethods(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPrecisionRecallCurveGetPointMethods(ByVal backend As Nd4jBackend)
			Dim threshold(100) As Double
			Dim precision() As Double = threshold
			Dim recall(100) As Double
			Dim i As Integer = 0
			For d As Double = 0 To 1 Step 0.01
				threshold(i) = d
				recall(i) = 1.0 - d
				i += 1
			Next d


			Dim prc As New PrecisionRecallCurve(threshold, precision, recall, Nothing, Nothing, Nothing, -1)

			Dim points() As PrecisionRecallCurve.Point = { prc.getPointAtThreshold(0.05), prc.getPointAtPrecision(0.05), prc.getPointAtRecall(1 - 0.05), prc.getPointAtThreshold(0.0495), prc.getPointAtPrecision(0.0495), prc.getPointAtRecall(1 - 0.0505)}



			For Each p As PrecisionRecallCurve.Point In points
				assertEquals(5, p.getIdx())
				assertEquals(0.05, p.getThreshold(), 1e-6)
				assertEquals(0.05, p.getPrecision(), 1e-6)
				assertEquals(1 - 0.05, p.getRecall(), 1e-6)
			Next p
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPrecisionRecallCurveConfusion(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPrecisionRecallCurveConfusion(ByVal backend As Nd4jBackend)
			'Sanity check: values calculated from the confusion matrix should match the PR curve values

			For Each removeRedundantPts As Boolean In New Boolean() {True, False}
				Dim r As New ROC(0, removeRedundantPts)

				Dim labels As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(DataType.DOUBLE,100, 1), 0.5))
				Dim probs As INDArray = Nd4j.rand(100, 1)

				r.eval(labels, probs)

				Dim prc As PrecisionRecallCurve = r.PrecisionRecallCurve
				Dim nPoints As Integer = prc.numPoints()

				For i As Integer = 0 To nPoints - 1
					Dim c As PrecisionRecallCurve.Confusion = prc.getConfusionMatrixAtPoint(i)
					Dim p As PrecisionRecallCurve.Point = c.getPoint()

					Dim tp As Integer = c.getTpCount()
					Dim fp As Integer = c.getFpCount()
					Dim fn As Integer = c.getFnCount()

					Dim prec As Double = tp / CDbl(tp + fp)
					Dim rec As Double = tp / CDbl(tp + fn)

					'Handle edge cases:
					If tp = 0 AndAlso fp = 0 Then
						prec = 1.0
					End If

					assertEquals(p.getPrecision(), prec, 1e-6)
					assertEquals(p.getRecall(), rec, 1e-6)
				Next i
			Next removeRedundantPts
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRocMerge()
		Public Overridable Sub testRocMerge()
			Nd4j.Random.setSeed(12345)

			Dim roc As New ROC()
			Dim roc1 As New ROC()
			Dim roc2 As New ROC()

			Dim nOut As Integer = 2

			Dim r As New Random(12345)
			For i As Integer = 0 To 9
				Dim labels As INDArray = Nd4j.zeros(3, nOut)
				For j As Integer = 0 To 2
					labels.putScalar(j, r.Next(nOut), 1.0)
				Next j
				Dim [out] As INDArray = Nd4j.rand(3, nOut)
				[out].diviColumnVector([out].sum(1))

				roc.eval(labels, [out])
				If i Mod 2 = 0 Then
					roc1.eval(labels, [out])
				Else
					roc2.eval(labels, [out])
				End If
			Next i

			roc1.calculateAUC()
			roc1.calculateAUCPR()
			roc2.calculateAUC()
			roc2.calculateAUCPR()

			roc1.merge(roc2)

			Dim aucExp As Double = roc.calculateAUC()
			Dim auprc As Double = roc.calculateAUCPR()

			Dim aucAct As Double = roc1.calculateAUC()
			Dim auprcAct As Double = roc1.calculateAUCPR()

			assertEquals(aucExp, aucAct, 1e-6)
			assertEquals(auprc, auprcAct, 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRocMultiMerge()
		Public Overridable Sub testRocMultiMerge()
			Nd4j.Random.setSeed(12345)

			Dim roc As New ROCMultiClass()
			Dim roc1 As New ROCMultiClass()
			Dim roc2 As New ROCMultiClass()

			Dim nOut As Integer = 5

			Dim r As New Random(12345)
			For i As Integer = 0 To 9
				Dim labels As INDArray = Nd4j.zeros(3, nOut)
				For j As Integer = 0 To 2
					labels.putScalar(j, r.Next(nOut), 1.0)
				Next j
				Dim [out] As INDArray = Nd4j.rand(3, nOut)
				[out].diviColumnVector([out].sum(1))

				roc.eval(labels, [out])
				If i Mod 2 = 0 Then
					roc1.eval(labels, [out])
				Else
					roc2.eval(labels, [out])
				End If
			Next i

			For i As Integer = 0 To nOut - 1
				roc1.calculateAUC(i)
				roc1.calculateAUCPR(i)
				roc2.calculateAUC(i)
				roc2.calculateAUCPR(i)
			Next i

			roc1.merge(roc2)

			For i As Integer = 0 To nOut - 1

				Dim aucExp As Double = roc.calculateAUC(i)
				Dim auprc As Double = roc.calculateAUCPR(i)

				Dim aucAct As Double = roc1.calculateAUC(i)
				Dim auprcAct As Double = roc1.calculateAUCPR(i)

				assertEquals(aucExp, aucAct, 1e-6)
				assertEquals(auprc, auprcAct, 1e-6)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testRocBinaryMerge(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRocBinaryMerge(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim roc As New ROCBinary()
			Dim roc1 As New ROCBinary()
			Dim roc2 As New ROCBinary()

			Dim nOut As Integer = 5

			For i As Integer = 0 To 9
				Dim labels As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(3, nOut),0.5))
				Dim [out] As INDArray = Nd4j.rand(3, nOut)
				[out].diviColumnVector([out].sum(1))

				roc.eval(labels, [out])
				If i Mod 2 = 0 Then
					roc1.eval(labels, [out])
				Else
					roc2.eval(labels, [out])
				End If
			Next i

			For i As Integer = 0 To nOut - 1
				roc1.calculateAUC(i)
				roc1.calculateAUCPR(i)
				roc2.calculateAUC(i)
				roc2.calculateAUCPR(i)
			Next i

			roc1.merge(roc2)

			For i As Integer = 0 To nOut - 1

				Dim aucExp As Double = roc.calculateAUC(i)
				Dim auprc As Double = roc.calculateAUCPR(i)

				Dim aucAct As Double = roc1.calculateAUC(i)
				Dim auprcAct As Double = roc1.calculateAUCPR(i)

				assertEquals(aucExp, aucAct, 1e-6)
				assertEquals(auprc, auprcAct, 1e-6)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSegmentationBinary()
		Public Overridable Sub testSegmentationBinary()
			For Each c As Integer In New Integer(){4, 1} 'c=1 should be treated as binary classification case
				Nd4j.Random.setSeed(12345)
				Dim mb As Integer = 3
				Dim h As Integer = 3
				Dim w As Integer = 2

				'NCHW
				Dim labels As INDArray = Nd4j.create(DataType.FLOAT, mb, c, h, w)
				Nd4j.exec(New BernoulliDistribution(labels, 0.5))

				Dim predictions As INDArray = Nd4j.rand(DataType.FLOAT, mb, c, h, w)

				Dim e2d As New ROCBinary()
				Dim e4d As New ROCBinary()

				Dim r2d As New ROC()
				e4d.eval(labels, predictions)

				For i As Integer = 0 To mb - 1
					For j As Integer = 0 To h - 1
						For k As Integer = 0 To w - 1
							Dim rowLabel As INDArray = labels.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(j), NDArrayIndex.point(k))
							Dim rowPredictions As INDArray = predictions.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(j), NDArrayIndex.point(k))
							rowLabel = rowLabel.reshape(ChrW(1), rowLabel.length())
							rowPredictions = rowPredictions.reshape(ChrW(1), rowLabel.length())

							e2d.eval(rowLabel, rowPredictions)
							If c = 1 Then
								r2d.eval(rowLabel, rowPredictions)
							End If
						Next k
					Next j
				Next i

				assertEquals(e2d, e4d)

				If c = 1 Then
					Dim r4d As New ROC()
					r4d.eval(labels, predictions)
					assertEquals(r2d, r4d)
				End If


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

					Dim e As New ROCBinary()
					e.Axis = i

					e.eval(labels, predictions)
					assertEquals(e2d, e)

					If c = 1 Then
						Dim r2 As New ROC()
						r2.Axis = i
						r2.eval(labels, predictions)
						assertEquals(r2d, r2)
					End If
				Next i
			Next c
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

				Dim e2d As New ROCMultiClass()
				Dim e4d As New ROCMultiClass()

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

					Dim e As New ROCMultiClass()
					e.Axis = i

					e.eval(labels, predictions)
					assertEquals(e2d, e)
				Next i
			Next c
		End Sub
	End Class

End Namespace