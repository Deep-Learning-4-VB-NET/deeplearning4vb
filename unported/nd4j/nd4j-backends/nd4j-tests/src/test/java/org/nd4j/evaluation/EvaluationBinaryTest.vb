Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports EvaluationBinary = org.nd4j.evaluation.classification.EvaluationBinary
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
import static org.junit.jupiter.api.Assertions.assertEquals
Imports org.nd4j.evaluation.classification.EvaluationBinary.Metric

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
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) @NativeTag public class EvaluationBinaryTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class EvaluationBinaryTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinary(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinary(ByVal backend As Nd4jBackend)
			'Compare EvaluationBinary to Evaluation class
			Dim dtypeBefore As DataType = Nd4j.defaultFloatingPointType()
			Dim first As EvaluationBinary = Nothing
			Dim sFirst As String = Nothing
			Try
				For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT}
					Nd4j.setDefaultDataTypes(globalDtype,If(globalDtype.isFPType(), globalDtype, DataType.DOUBLE))
					For Each lpDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}

						Nd4j.Random.setSeed(12345)

						Dim nExamples As Integer = 50
						Dim nOut As Integer = 4
						Dim shape() As Long = {nExamples, nOut}

						Dim labels As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(lpDtype, shape), 0.5))

						Dim predicted As INDArray = Nd4j.rand(lpDtype, shape)
						Dim binaryPredicted As INDArray = predicted.gt(0.5)

						Dim eb As New EvaluationBinary()
						eb.eval(labels, predicted)

						'System.out.println(eb.stats());

						Dim eps As Double = 1e-6
						For i As Integer = 0 To nOut - 1
							Dim lCol As INDArray = labels.getColumn(i,True)
							Dim pCol As INDArray = predicted.getColumn(i,True)
							Dim bpCol As INDArray = binaryPredicted.getColumn(i,True)

							Dim countCorrect As Integer = 0
							Dim tpCount As Integer = 0
							Dim tnCount As Integer = 0
							For j As Integer = 0 To lCol.length() - 1
								If lCol.getDouble(j) = bpCol.getDouble(j) Then
									countCorrect += 1
									If lCol.getDouble(j) = 1 Then
										tpCount += 1
									Else
										tnCount += 1
									End If
								End If
							Next j
							Dim acc As Double = countCorrect / CDbl(lCol.length())

							Dim e As New Evaluation()
							e.eval(lCol, pCol)

							assertEquals(acc, eb.accuracy(i), eps)
							assertEquals(e.accuracy(), eb.scoreForMetric(EvaluationBinary.Metric.ACCURACY, i), eps)
							assertEquals(e.precision(1), eb.scoreForMetric(EvaluationBinary.Metric.PRECISION, i), eps)
							assertEquals(e.recall(1), eb.scoreForMetric(EvaluationBinary.Metric.RECALL, i), eps)
							assertEquals(e.f1(1), eb.scoreForMetric(EvaluationBinary.Metric.F1, i), eps)
							assertEquals(e.falseAlarmRate(), eb.scoreForMetric(EvaluationBinary.Metric.FAR, i), eps)
							assertEquals(e.falsePositiveRate(1), eb.falsePositiveRate(i), eps)


							assertEquals(tpCount, eb.truePositives(i))
							assertEquals(tnCount, eb.trueNegatives(i))

							assertEquals(CInt(e.truePositives()(1)), eb.truePositives(i))
							assertEquals(CInt(e.trueNegatives()(1)), eb.trueNegatives(i))
							assertEquals(CInt(e.falsePositives()(1)), eb.falsePositives(i))
							assertEquals(CInt(e.falseNegatives()(1)), eb.falseNegatives(i))

							assertEquals(nExamples, eb.totalCount(i))

							Dim s As String = eb.stats()
							If first Is Nothing Then
								first = eb
								sFirst = s
							Else
								assertEquals(first, eb)
								assertEquals(sFirst, s)
							End If
						Next i
					Next lpDtype
				Next globalDtype
			Finally
				Nd4j.setDefaultDataTypes(dtypeBefore, dtypeBefore)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinaryMerging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinaryMerging(ByVal backend As Nd4jBackend)
			Dim nOut As Integer = 4
			Dim shape1() As Integer = {30, nOut}
			Dim shape2() As Integer = {50, nOut}

			Nd4j.Random.setSeed(12345)
			Dim l1 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(shape1), 0.5))
			Dim l2 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(shape2), 0.5))
			Dim p1 As INDArray = Nd4j.rand(shape1)
			Dim p2 As INDArray = Nd4j.rand(shape2)

			Dim eb As New EvaluationBinary()
			eb.eval(l1, p1)
			eb.eval(l2, p2)

			Dim eb1 As New EvaluationBinary()
			eb1.eval(l1, p1)

			Dim eb2 As New EvaluationBinary()
			eb2.eval(l2, p2)

			eb1.merge(eb2)

			assertEquals(eb.stats(), eb1.stats())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinaryPerOutputMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinaryPerOutputMasking(ByVal backend As Nd4jBackend)

			'Provide a mask array: "ignore" the masked steps

			Dim mask As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 1, 0},
				New Double() {1, 0, 0},
				New Double() {1, 1, 0},
				New Double() {1, 0, 0},
				New Double() {1, 1, 1}
			})

			Dim labels As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 1, 1},
				New Double() {0, 0, 0},
				New Double() {1, 1, 1},
				New Double() {0, 1, 1},
				New Double() {1, 0, 1}
			})

			Dim predicted As INDArray = Nd4j.create(New Double()() {
				New Double() {0.9, 0.9, 0.9},
				New Double() {0.7, 0.7, 0.7},
				New Double() {0.6, 0.6, 0.6},
				New Double() {0.4, 0.4, 0.4},
				New Double() {0.1, 0.1, 0.1}
			})

			'Correct?
			'      Y Y m
			'      N m m
			'      Y Y m
			'      Y m m
			'      N Y N

			Dim eb As New EvaluationBinary()
			eb.eval(labels, predicted, mask)

			assertEquals(0.6, eb.accuracy(0), 1e-6)
			assertEquals(1.0, eb.accuracy(1), 1e-6)
			assertEquals(0.0, eb.accuracy(2), 1e-6)

			assertEquals(2, eb.truePositives(0))
			assertEquals(2, eb.truePositives(1))
			assertEquals(0, eb.truePositives(2))

			assertEquals(1, eb.trueNegatives(0))
			assertEquals(1, eb.trueNegatives(1))
			assertEquals(0, eb.trueNegatives(2))

			assertEquals(1, eb.falsePositives(0))
			assertEquals(0, eb.falsePositives(1))
			assertEquals(0, eb.falsePositives(2))

			assertEquals(1, eb.falseNegatives(0))
			assertEquals(0, eb.falseNegatives(1))
			assertEquals(1, eb.falseNegatives(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTimeSeriesEval(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTimeSeriesEval(ByVal backend As Nd4jBackend)

			Dim shape() As Integer = {2, 4, 3}
			Nd4j.Random.setSeed(12345)
			Dim labels As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(shape), 0.5))
			Dim predicted As INDArray = Nd4j.rand(shape)
			Dim mask As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(shape), 0.5))

			Dim eb1 As New EvaluationBinary()
			eb1.eval(labels, predicted, mask)

			Dim eb2 As New EvaluationBinary()
			Dim i As Integer = 0
			Do While i < shape(2)
				Dim l As INDArray = labels.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i))
				Dim p As INDArray = predicted.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i))
				Dim m As INDArray = mask.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i))

				eb2.eval(l, p, m)
				i += 1
			Loop

			assertEquals(eb2.stats(), eb1.stats())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinaryWithROC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinaryWithROC(ByVal backend As Nd4jBackend)
			'Simple test for nested ROCBinary in EvaluationBinary

			Nd4j.Random.setSeed(12345)
			Dim l1 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(New Integer() {50, 4}), 0.5))
			Dim p1 As INDArray = Nd4j.rand(50, 4)

			Dim eb As New EvaluationBinary(4, 30)
			eb.eval(l1, p1)

	'        System.out.println(eb.stats());
			eb.stats()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinary3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinary3d(ByVal backend As Nd4jBackend)
			Dim prediction As INDArray = Nd4j.rand(DataType.FLOAT, 2, 5, 10)
			Dim label As INDArray = Nd4j.rand(DataType.FLOAT, 2, 5, 10)


			Dim rowsP As IList(Of INDArray) = New List(Of INDArray)()
			Dim rowsL As IList(Of INDArray) = New List(Of INDArray)()
			Dim iter As New NdIndexIterator(2, 10)
			Do While iter.MoveNext()
				Dim idx() As Long = iter.Current
				Dim idxs() As INDArrayIndex = {NDArrayIndex.point(idx(0)), NDArrayIndex.all(), NDArrayIndex.point(idx(1))}
				rowsP.Add(prediction.get(idxs))
				rowsL.Add(label.get(idxs))
			Loop

			Dim p2d As INDArray = Nd4j.vstack(rowsP)
			Dim l2d As INDArray = Nd4j.vstack(rowsL)

			Dim e3d As New EvaluationBinary()
			Dim e2d As New EvaluationBinary()

			e3d.eval(label, prediction)
			e2d.eval(l2d, p2d)

			For Each m As EvaluationBinary.Metric In EvaluationBinary.Metric.values()
				For i As Integer = 0 To 4
					Dim d1 As Double = e3d.scoreForMetric(m, i)
					Dim d2 As Double = e2d.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinary4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinary4d(ByVal backend As Nd4jBackend)
			Dim prediction As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10, 10)
			Dim label As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10, 10)


			Dim rowsP As IList(Of INDArray) = New List(Of INDArray)()
			Dim rowsL As IList(Of INDArray) = New List(Of INDArray)()
			Dim iter As New NdIndexIterator(2, 10, 10)
			Do While iter.MoveNext()
				Dim idx() As Long = iter.Current
				Dim idxs() As INDArrayIndex = {NDArrayIndex.point(idx(0)), NDArrayIndex.all(), NDArrayIndex.point(idx(1)), NDArrayIndex.point(idx(2))}
				rowsP.Add(prediction.get(idxs))
				rowsL.Add(label.get(idxs))
			Loop

			Dim p2d As INDArray = Nd4j.vstack(rowsP)
			Dim l2d As INDArray = Nd4j.vstack(rowsL)

			Dim e4d As New EvaluationBinary()
			Dim e2d As New EvaluationBinary()

			e4d.eval(label, prediction)
			e2d.eval(l2d, p2d)

			For Each m As EvaluationBinary.Metric In EvaluationBinary.Metric.values()
				For i As Integer = 0 To 2
					Dim d1 As Double = e4d.scoreForMetric(m, i)
					Dim d2 As Double = e2d.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinary3dMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinary3dMasking(ByVal backend As Nd4jBackend)
			Dim prediction As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10)
			Dim label As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10)

			Dim rowsP As IList(Of INDArray) = New List(Of INDArray)()
			Dim rowsL As IList(Of INDArray) = New List(Of INDArray)()

			'Check "DL4J-style" 2d per timestep masking [minibatch, seqLength] mask shape
			Dim mask2d As INDArray = Nd4j.randomBernoulli(0.5, 2, 10)
			rowsP.Clear()
			rowsL.Clear()
			Dim iter As New NdIndexIterator(2, 10)
			Do While iter.MoveNext()
				Dim idx() As Long = iter.Current
				If mask2d.getDouble(idx(0), idx(1)) <> 0.0 Then
					Dim idxs() As INDArrayIndex = {NDArrayIndex.point(idx(0)), NDArrayIndex.all(), NDArrayIndex.point(idx(1))}
					rowsP.Add(prediction.get(idxs))
					rowsL.Add(label.get(idxs))
				End If
			Loop
			Dim p2d As INDArray = Nd4j.vstack(rowsP)
			Dim l2d As INDArray = Nd4j.vstack(rowsL)

			Dim e3d_m2d As New EvaluationBinary()
			Dim e2d_m2d As New EvaluationBinary()
			e3d_m2d.eval(label, prediction, mask2d)
			e2d_m2d.eval(l2d, p2d)



			'Check per-output masking:
			Dim perOutMask As INDArray = Nd4j.randomBernoulli(0.5, label.shape())
			rowsP.Clear()
			rowsL.Clear()
			Dim rowsM As IList(Of INDArray) = New List(Of INDArray)()
			iter = New NdIndexIterator(2, 10)
			Do While iter.MoveNext()
				Dim idx() As Long = iter.Current
				Dim idxs() As INDArrayIndex = {NDArrayIndex.point(idx(0)), NDArrayIndex.all(), NDArrayIndex.point(idx(1))}
				rowsP.Add(prediction.get(idxs))
				rowsL.Add(label.get(idxs))
				rowsM.Add(perOutMask.get(idxs))
			Loop
			p2d = Nd4j.vstack(rowsP)
			l2d = Nd4j.vstack(rowsL)
			Dim m2d As INDArray = Nd4j.vstack(rowsM)

			Dim e4d_m2 As New EvaluationBinary()
			Dim e2d_m2 As New EvaluationBinary()
			e4d_m2.eval(label, prediction, perOutMask)
			e2d_m2.eval(l2d, p2d, m2d)
			For Each m As EvaluationBinary.Metric In EvaluationBinary.Metric.values()
				For i As Integer = 0 To 2
					Dim d1 As Double = e4d_m2.scoreForMetric(m, i)
					Dim d2 As Double = e2d_m2.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationBinary4dMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationBinary4dMasking(ByVal backend As Nd4jBackend)
			Dim prediction As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10, 10)
			Dim label As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10, 10)

			Dim rowsP As IList(Of INDArray) = New List(Of INDArray)()
			Dim rowsL As IList(Of INDArray) = New List(Of INDArray)()

			'Check per-example masking:
			Dim mask1dPerEx As INDArray = Nd4j.createFromArray(1, 0)

			Dim iter As New NdIndexIterator(2, 10, 10)
			Do While iter.MoveNext()
				Dim idx() As Long = iter.Current
				If mask1dPerEx.getDouble(idx(0)) <> 0.0 Then
					Dim idxs() As INDArrayIndex = {NDArrayIndex.point(idx(0)), NDArrayIndex.all(), NDArrayIndex.point(idx(1)), NDArrayIndex.point(idx(2))}
					rowsP.Add(prediction.get(idxs))
					rowsL.Add(label.get(idxs))
				End If
			Loop

			Dim p2d As INDArray = Nd4j.vstack(rowsP)
			Dim l2d As INDArray = Nd4j.vstack(rowsL)

			Dim e4d_m1 As New EvaluationBinary()
			Dim e2d_m1 As New EvaluationBinary()
			e4d_m1.eval(label, prediction, mask1dPerEx)
			e2d_m1.eval(l2d, p2d)
			For Each m As EvaluationBinary.Metric In EvaluationBinary.Metric.values()
				For i As Integer = 0 To 2
					Dim d1 As Double = e4d_m1.scoreForMetric(m, i)
					Dim d2 As Double = e2d_m1.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m

			'Check per-output masking:
			Dim perOutMask As INDArray = Nd4j.randomBernoulli(0.5, label.shape())
			rowsP.Clear()
			rowsL.Clear()
			Dim rowsM As IList(Of INDArray) = New List(Of INDArray)()
			iter = New NdIndexIterator(2, 10, 10)
			Do While iter.MoveNext()
				Dim idx() As Long = iter.Current
				Dim idxs() As INDArrayIndex = {NDArrayIndex.point(idx(0)), NDArrayIndex.all(), NDArrayIndex.point(idx(1)), NDArrayIndex.point(idx(2))}
				rowsP.Add(prediction.get(idxs))
				rowsL.Add(label.get(idxs))
				rowsM.Add(perOutMask.get(idxs))
			Loop
			p2d = Nd4j.vstack(rowsP)
			l2d = Nd4j.vstack(rowsL)
			Dim m2d As INDArray = Nd4j.vstack(rowsM)

			Dim e3d_m2 As New EvaluationBinary()
			Dim e2d_m2 As New EvaluationBinary()
			e3d_m2.eval(label, prediction, perOutMask)
			e2d_m2.eval(l2d, p2d, m2d)
			For Each m As EvaluationBinary.Metric In EvaluationBinary.Metric.values()
				For i As Integer = 0 To 2
					Dim d1 As Double = e3d_m2.scoreForMetric(m, i)
					Dim d2 As Double = e2d_m2.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m
		End Sub
	End Class

End Namespace