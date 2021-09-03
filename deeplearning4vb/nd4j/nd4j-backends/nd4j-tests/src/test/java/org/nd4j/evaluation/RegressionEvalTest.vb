Imports System
Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports Metric = org.nd4j.evaluation.regression.RegressionEvaluation.Metric
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertThrows
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
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) @NativeTag public class RegressionEvalTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class RegressionEvalTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvalParameters(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvalParameters(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.InvalidOperationException),Sub()
			Dim specCols As Integer = 5
			Dim labels As INDArray = Nd4j.ones(3)
			Dim preds As INDArray = Nd4j.ones(6)
			Dim eval As New RegressionEvaluation(specCols)
			eval.eval(labels, preds)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPerfectPredictions(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPerfectPredictions(ByVal backend As Nd4jBackend)

			Dim nCols As Integer = 5
			Dim nTestArrays As Integer = 100
			Dim valuesPerTestArray As Integer = 3
			Dim eval As New RegressionEvaluation(nCols)

			For i As Integer = 0 To nTestArrays - 1
				Dim rand As INDArray = Nd4j.rand(DataType.DOUBLE,valuesPerTestArray, nCols)
				eval.eval(rand, rand)
			Next i

	'        System.out.println(eval.stats());
			eval.stats()

			For i As Integer = 0 To nCols - 1
				assertEquals(0.0, eval.meanSquaredError(i), 1e-6)
				assertEquals(0.0, eval.meanAbsoluteError(i), 1e-6)
				assertEquals(0.0, eval.rootMeanSquaredError(i), 1e-6)
				assertEquals(0.0, eval.relativeSquaredError(i), 1e-6)
				assertEquals(1.0, eval.correlationR2(i), 1e-6)
				assertEquals(1.0, eval.pearsonCorrelation(i), 1e-6)
				assertEquals(1.0, eval.rSquared(i), 1e-6)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKnownValues(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKnownValues(ByVal backend As Nd4jBackend)

			Dim dtypeBefore As DataType = Nd4j.defaultFloatingPointType()
			Dim first As RegressionEvaluation = Nothing
			Dim sFirst As String = Nothing
			Try
				For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT}
					Nd4j.setDefaultDataTypes(globalDtype,If(globalDtype.isFPType(), globalDtype, DataType.DOUBLE))
					For Each lpDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}

						Dim labelsD()() As Double = {
							New Double() {1, 2, 3},
							New Double() {0.1, 0.2, 0.3},
							New Double() {6, 5, 4}
						}
						Dim predictedD()() As Double = {
							New Double() {2.5, 3.2, 3.8},
							New Double() {2.15, 1.3, -1.2},
							New Double() {7, 4.5, 3}
						}

						Dim expMSE() As Double = {2.484166667, 0.966666667, 1.296666667}
						Dim expMAE() As Double = {1.516666667, 0.933333333, 1.1}
						Dim expRSE() As Double = {0.368813923, 0.246598639, 0.530937216}
						Dim expCorrs() As Double = {0.997013483, 0.968619605, 0.915603032}
						Dim expR2() As Double = {0.63118608, 0.75340136, 0.46906278}

						Dim labels As INDArray = Nd4j.create(labelsD).castTo(lpDtype)
						Dim predicted As INDArray = Nd4j.create(predictedD).castTo(lpDtype)

						Dim eval As New RegressionEvaluation(3)

						For xe As Integer = 0 To 1
							eval.eval(labels, predicted)

							For col As Integer = 0 To 2
								assertEquals(expMSE(col), eval.meanSquaredError(col),If(lpDtype = DataType.HALF, 1e-2, 1e-4))
								assertEquals(expMAE(col), eval.meanAbsoluteError(col),If(lpDtype = DataType.HALF, 1e-2, 1e-4))
								assertEquals(Math.Sqrt(expMSE(col)), eval.rootMeanSquaredError(col),If(lpDtype = DataType.HALF, 1e-2, 1e-4))
								assertEquals(expRSE(col), eval.relativeSquaredError(col),If(lpDtype = DataType.HALF, 1e-2, 1e-4))
								assertEquals(expCorrs(col), eval.pearsonCorrelation(col),If(lpDtype = DataType.HALF, 1e-2, 1e-4))
								assertEquals(expR2(col), eval.rSquared(col),If(lpDtype = DataType.HALF, 1e-2, 1e-4))
							Next col

							Dim s As String = eval.stats()
							If first Is Nothing Then
								first = eval
								sFirst = s
							ElseIf lpDtype <> DataType.HALF Then 'Precision issues with FP16
								assertEquals(sFirst, s)
								assertEquals(first, eval)
							End If

							eval = New RegressionEvaluation(3)
						Next xe
					Next lpDtype
				Next globalDtype
			Finally
				Nd4j.setDefaultDataTypes(dtypeBefore, dtypeBefore)
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRegressionEvaluationMerging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRegressionEvaluationMerging(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim nRows As Integer = 20
			Dim nCols As Integer = 3

			Dim numMinibatches As Integer = 5
			Dim nEvalInstances As Integer = 4

			Dim list As IList(Of RegressionEvaluation) = New List(Of RegressionEvaluation)()

			Dim [single] As New RegressionEvaluation(nCols)

			For i As Integer = 0 To nEvalInstances - 1
				list.Add(New RegressionEvaluation(nCols))
				For j As Integer = 0 To numMinibatches - 1
					Dim p As INDArray = Nd4j.rand(DataType.DOUBLE,nRows, nCols)
					Dim act As INDArray = Nd4j.rand(DataType.DOUBLE,nRows, nCols)

					[single].eval(act, p)

					list(i).eval(act, p)
				Next j
			Next i

			Dim merged As RegressionEvaluation = list(0)
			For i As Integer = 1 To nEvalInstances - 1
				merged.merge(list(i))
			Next i

			Dim prec As Double = 1e-5
			For i As Integer = 0 To nCols - 1
				assertEquals([single].correlationR2(i), merged.correlationR2(i), prec)
				assertEquals([single].meanAbsoluteError(i), merged.meanAbsoluteError(i), prec)
				assertEquals([single].meanSquaredError(i), merged.meanSquaredError(i), prec)
				assertEquals([single].relativeSquaredError(i), merged.relativeSquaredError(i), prec)
				assertEquals([single].rootMeanSquaredError(i), merged.rootMeanSquaredError(i), prec)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRegressionEvalPerOutputMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRegressionEvalPerOutputMasking(ByVal backend As Nd4jBackend)

			Dim l As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2, 3},
				New Double() {10, 20, 30},
				New Double() {-5, -10, -20}
			})

			Dim predictions As INDArray = Nd4j.zeros(l.shape())

			Dim mask As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1, 1},
				New Double() {1, 1, 0},
				New Double() {0, 1, 0}
			})


			Dim re As New RegressionEvaluation()

			re.eval(l, predictions, mask)

			Dim mse() As Double = {(10 * 10) / 1.0, (2 * 2 + 20 * 20 + 10 * 10) \ 3, (3 * 3) / 1.0}

			Dim mae() As Double = {10.0, (2 + 20 + 10) / 3.0, 3.0}

			Dim rmse() As Double = {10.0, Math.Sqrt((2 * 2 + 20 * 20 + 10 * 10) / 3.0), 3.0}

			For i As Integer = 0 To 2
				assertEquals(mse(i), re.meanSquaredError(i), 1e-6)
				assertEquals(mae(i), re.meanAbsoluteError(i), 1e-6)
				assertEquals(rmse(i), re.rootMeanSquaredError(i), 1e-6)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRegressionEvalTimeSeriesSplit()
		Public Overridable Sub testRegressionEvalTimeSeriesSplit()

			Dim out1 As INDArray = Nd4j.rand(New Integer(){3, 5, 20})
			Dim outSub1 As INDArray = out1.get(all(), all(), interval(0,10))
			Dim outSub2 As INDArray = out1.get(all(), all(), interval(10, 20))

			Dim label1 As INDArray = Nd4j.rand(New Integer(){3, 5, 20})
			Dim labelSub1 As INDArray = label1.get(all(), all(), interval(0,10))
			Dim labelSub2 As INDArray = label1.get(all(), all(), interval(10, 20))

			Dim e1 As New RegressionEvaluation()
			Dim e2 As New RegressionEvaluation()

			e1.eval(label1, out1)

			e2.eval(labelSub1, outSub1)
			e2.eval(labelSub2, outSub2)

			assertEquals(e1, e2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRegressionEval3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRegressionEval3d(ByVal backend As Nd4jBackend)
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

			Dim e3d As New RegressionEvaluation()
			Dim e2d As New RegressionEvaluation()

			e3d.eval(label, prediction)
			e2d.eval(l2d, p2d)

			For Each m As RegressionEvaluation.Metric In RegressionEvaluation.Metric.values()
				Dim d1 As Double = e3d.scoreForMetric(m)
				Dim d2 As Double = e2d.scoreForMetric(m)
				assertEquals(d2, d1, 1e-6,m.ToString())
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRegressionEval4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRegressionEval4d(ByVal backend As Nd4jBackend)
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

			Dim e4d As New RegressionEvaluation()
			Dim e2d As New RegressionEvaluation()

			e4d.eval(label, prediction)
			e2d.eval(l2d, p2d)

			For Each m As RegressionEvaluation.Metric In RegressionEvaluation.Metric.values()
				Dim d1 As Double = e4d.scoreForMetric(m)
				Dim d2 As Double = e2d.scoreForMetric(m)
				assertEquals(d2, d1, 1e-5,m.ToString())
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRegressionEval3dMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRegressionEval3dMasking(ByVal backend As Nd4jBackend)
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

			Dim e3d_m2d As New RegressionEvaluation()
			Dim e2d_m2d As New RegressionEvaluation()
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

			Dim e4d_m2 As New RegressionEvaluation()
			Dim e2d_m2 As New RegressionEvaluation()
			e4d_m2.eval(label, prediction, perOutMask)
			e2d_m2.eval(l2d, p2d, m2d)
			For Each m As RegressionEvaluation.Metric In RegressionEvaluation.Metric.values()
				Dim d1 As Double = e4d_m2.scoreForMetric(m)
				Dim d2 As Double = e2d_m2.scoreForMetric(m)
				assertEquals(d2, d1, 1e-5,m.ToString())
			Next m
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRegressionEval4dMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRegressionEval4dMasking(ByVal backend As Nd4jBackend)
			Dim prediction As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10, 10)
			Dim label As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10, 10)

			Dim rowsP As IList(Of INDArray) = New List(Of INDArray)()
			Dim rowsL As IList(Of INDArray) = New List(Of INDArray)()

			'Check per-example masking:
			Dim mask1dPerEx As INDArray = Nd4j.createFromArray(1, 0).castTo(DataType.FLOAT)

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

			Dim e4d_m1 As New RegressionEvaluation()
			Dim e2d_m1 As New RegressionEvaluation()
			e4d_m1.eval(label, prediction, mask1dPerEx)
			e2d_m1.eval(l2d, p2d)
			For Each m As RegressionEvaluation.Metric In RegressionEvaluation.Metric.values()
				Dim d1 As Double = e4d_m1.scoreForMetric(m)
				Dim d2 As Double = e2d_m1.scoreForMetric(m)
				assertEquals(d2, d1, 1e-5,m.ToString())
			Next m

			'Check per-output masking:
			Dim perOutMask As INDArray = Nd4j.randomBernoulli(0.5, label.shape()).castTo(DataType.FLOAT)
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

			Dim e4d_m2 As New RegressionEvaluation()
			Dim e2d_m2 As New RegressionEvaluation()
			e4d_m2.eval(label, prediction, perOutMask)
			e2d_m2.eval(l2d, p2d, m2d)
			For Each m As RegressionEvaluation.Metric In RegressionEvaluation.Metric.values()
				Dim d1 As Double = e4d_m2.scoreForMetric(m)
				Dim d2 As Double = e2d_m2.scoreForMetric(m)
				assertEquals(d2, d1, 1e-5,m.ToString())
			Next m
		End Sub
	End Class

End Namespace