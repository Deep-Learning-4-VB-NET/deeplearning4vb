Imports System.Collections.Generic
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCBinary = org.nd4j.evaluation.classification.ROCBinary
Imports PrecisionRecallCurve = org.nd4j.evaluation.curves.PrecisionRecallCurve
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
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) @NativeTag public class ROCBinaryTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ROCBinaryTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Disabled public void testROCBinary(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCBinary(ByVal backend As Nd4jBackend)
			'Compare ROCBinary to ROC class

			Dim dtypeBefore As DataType = Nd4j.defaultFloatingPointType()
			Dim first30 As ROCBinary = Nothing
			Dim first0 As ROCBinary = Nothing
			Dim sFirst30 As String = Nothing
			Dim sFirst0 As String = Nothing
			Try
				For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT}
	'            for (DataType globalDtype : new DataType[]{DataType.HALF}) {
					Nd4j.setDefaultDataTypes(globalDtype,If(globalDtype.isFPType(), globalDtype, DataType.DOUBLE))
					For Each lpDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
						Dim msg As String = "globalDtype=" & globalDtype & ", labelPredictionsDtype=" & lpDtype

						Dim nExamples As Integer = 50
						Dim nOut As Integer = 4
						Dim shape() As Long = {nExamples, nOut}

						For Each thresholdSteps As Integer In New Integer(){30, 0} '0 == exact

							Nd4j.Random.setSeed(12345)
							Dim labels As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(DataType.DOUBLE, shape), 0.5)).castTo(lpDtype)

							Nd4j.Random.setSeed(12345)
							Dim predicted As INDArray = Nd4j.rand(DataType.DOUBLE, shape).castTo(lpDtype)

							Dim rb As New ROCBinary(thresholdSteps)

							For xe As Integer = 0 To 1
								rb.eval(labels, predicted)

								'System.out.println(rb.stats());

								Dim eps As Double = If(lpDtype = DataType.HALF, 1e-2, 1e-6)
								For i As Integer = 0 To nOut - 1
									Dim lCol As INDArray = labels.getColumn(i, True)
									Dim pCol As INDArray = predicted.getColumn(i, True)


									Dim r As New ROC(thresholdSteps)
									r.eval(lCol, pCol)

									Dim aucExp As Double = r.calculateAUC()
									Dim auc As Double = rb.calculateAUC(i)

									assertEquals(aucExp, auc, eps,msg)

									Dim apExp As Long = r.getCountActualPositive()
									Dim ap As Long = rb.getCountActualPositive(i)
									assertEquals(ap, apExp,msg)

									Dim anExp As Long = r.getCountActualNegative()
									Dim an As Long = rb.getCountActualNegative(i)
									assertEquals(anExp, an)

									Dim pExp As PrecisionRecallCurve = r.PrecisionRecallCurve
									Dim p As PrecisionRecallCurve = rb.getPrecisionRecallCurve(i)

									assertEquals(pExp, p,msg)
								Next i

								Dim s As String = rb.stats()

								If thresholdSteps = 0 Then
									If first0 Is Nothing Then
										first0 = rb
										sFirst0 = s
									ElseIf lpDtype <> DataType.HALF Then 'Precision issues with FP16
										assertEquals(msg, sFirst0, s)
										assertEquals(first0, rb)
									End If
								Else
									If first30 Is Nothing Then
										first30 = rb
										sFirst30 = s
									ElseIf lpDtype <> DataType.HALF Then 'Precision issues with FP16
										assertEquals(msg, sFirst30, s)
										assertEquals(first30, rb)
									End If
								End If

	'                            rb.reset();
								rb = New ROCBinary(thresholdSteps)
							Next xe
						Next thresholdSteps
					Next lpDtype
				Next globalDtype
			Finally
				Nd4j.setDefaultDataTypes(dtypeBefore, dtypeBefore)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRocBinaryMerging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRocBinaryMerging(ByVal backend As Nd4jBackend)
			For Each nSteps As Integer In New Integer(){30, 0} '0 == exact
				Dim nOut As Integer = 4
				Dim shape1() As Integer = {30, nOut}
				Dim shape2() As Integer = {50, nOut}

				Nd4j.Random.setSeed(12345)
				Dim l1 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(shape1), 0.5))
				Dim l2 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(shape2), 0.5))
				Dim p1 As INDArray = Nd4j.rand(shape1)
				Dim p2 As INDArray = Nd4j.rand(shape2)

				Dim rb As New ROCBinary(nSteps)
				rb.eval(l1, p1)
				rb.eval(l2, p2)

				Dim rb1 As New ROCBinary(nSteps)
				rb1.eval(l1, p1)

				Dim rb2 As New ROCBinary(nSteps)
				rb2.eval(l2, p2)

				rb1.merge(rb2)

				assertEquals(rb.stats(), rb1.stats())
			Next nSteps
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testROCBinaryPerOutputMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCBinaryPerOutputMasking(ByVal backend As Nd4jBackend)

			For Each nSteps As Integer In New Integer(){30, 0} '0 == exact

				'Here: we'll create a test array, then insert some 'masked out' values, and ensure we get the same results
				Dim mask As INDArray = Nd4j.create(New Double()(){
					New Double() {1, 1, 1},
					New Double() {0, 1, 1},
					New Double() {1, 0, 1},
					New Double() {1, 1, 0},
					New Double() {1, 1, 1}
				})

				Dim labels As INDArray = Nd4j.create(New Double()(){
					New Double() {0, 1, 0},
					New Double() {1, 1, 0},
					New Double() {0, 1, 1},
					New Double() {0, 0, 1},
					New Double() {1, 1, 1}
				})

				'Remove the 1 masked value for each column
				Dim labelsExMasked As INDArray = Nd4j.create(New Double()(){
					New Double() {0, 1, 0},
					New Double() {0, 1, 0},
					New Double() {0, 0, 1},
					New Double() {1, 1, 1}
				})

				Dim predicted As INDArray = Nd4j.create(New Double()(){
					New Double() {0.9, 0.4, 0.6},
					New Double() {0.2, 0.8, 0.4},
					New Double() {0.6, 0.1, 0.1},
					New Double() {0.3, 0.7, 0.2},
					New Double() {0.8, 0.6, 0.6}
				})

				Dim predictedExMasked As INDArray = Nd4j.create(New Double()(){
					New Double() {0.9, 0.4, 0.6},
					New Double() {0.6, 0.8, 0.4},
					New Double() {0.3, 0.7, 0.1},
					New Double() {0.8, 0.6, 0.6}
				})

				Dim rbMasked As New ROCBinary(nSteps)
				rbMasked.eval(labels, predicted, mask)

				Dim rb As New ROCBinary(nSteps)
				rb.eval(labelsExMasked, predictedExMasked)

				Dim s1 As String = rb.stats()
				Dim s2 As String = rbMasked.stats()
				assertEquals(s1, s2)

				For i As Integer = 0 To 2
					Dim pExp As PrecisionRecallCurve = rb.getPrecisionRecallCurve(i)
					Dim p As PrecisionRecallCurve = rbMasked.getPrecisionRecallCurve(i)

					assertEquals(pExp, p)
				Next i
			Next nSteps
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testROCBinary3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCBinary3d(ByVal backend As Nd4jBackend)
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

			Dim e3d As New ROCBinary()
			Dim e2d As New ROCBinary()

			e3d.eval(label, prediction)
			e2d.eval(l2d, p2d)

			For Each m As ROCBinary.Metric In ROCBinary.Metric.values()
				For i As Integer = 0 To 4
					Dim d1 As Double = e3d.scoreForMetric(m, i)
					Dim d2 As Double = e2d.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testROCBinary4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCBinary4d(ByVal backend As Nd4jBackend)
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

			Dim e4d As New ROCBinary()
			Dim e2d As New ROCBinary()

			e4d.eval(label, prediction)
			e2d.eval(l2d, p2d)

			For Each m As ROCBinary.Metric In ROCBinary.Metric.values()
				For i As Integer = 0 To 2
					Dim d1 As Double = e4d.scoreForMetric(m, i)
					Dim d2 As Double = e2d.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testROCBinary3dMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCBinary3dMasking(ByVal backend As Nd4jBackend)
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

			Dim e3d_m2d As New ROCBinary()
			Dim e2d_m2d As New ROCBinary()
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

			Dim e4d_m2 As New ROCBinary()
			Dim e2d_m2 As New ROCBinary()
			e4d_m2.eval(label, prediction, perOutMask)
			e2d_m2.eval(l2d, p2d, m2d)
			For Each m As ROCBinary.Metric In ROCBinary.Metric.values()
				For i As Integer = 0 To 2
					Dim d1 As Double = e4d_m2.scoreForMetric(m, i)
					Dim d2 As Double = e2d_m2.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testROCBinary4dMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testROCBinary4dMasking(ByVal backend As Nd4jBackend)
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

			Dim e4d_m1 As New ROCBinary()
			Dim e2d_m1 As New ROCBinary()
			e4d_m1.eval(label, prediction, mask1dPerEx)
			e2d_m1.eval(l2d, p2d)
			For Each m As ROCBinary.Metric In ROCBinary.Metric.values()
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

			Dim e3d_m2 As New ROCBinary()
			Dim e2d_m2 As New ROCBinary()
			e3d_m2.eval(label, prediction, perOutMask)
			e2d_m2.eval(l2d, p2d, m2d)
			For Each m As ROCBinary.Metric In ROCBinary.Metric.values()
				For i As Integer = 0 To 2
					Dim d1 As Double = e3d_m2.scoreForMetric(m, i)
					Dim d2 As Double = e2d_m2.scoreForMetric(m, i)
					assertEquals(d2, d1, 1e-6,m.ToString())
				Next i
			Next m
		End Sub
	End Class

End Namespace