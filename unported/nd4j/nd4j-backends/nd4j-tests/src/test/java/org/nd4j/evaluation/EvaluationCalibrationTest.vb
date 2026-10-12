Imports System
Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports EvaluationCalibration = org.nd4j.evaluation.classification.EvaluationCalibration
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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
'ORIGINAL LINE: @Tag(TagNames.EVAL_METRICS) @NativeTag public class EvaluationCalibrationTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class EvaluationCalibrationTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReliabilityDiagram(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReliabilityDiagram(ByVal backend As Nd4jBackend)

			Dim dtypeBefore As DataType = Nd4j.defaultFloatingPointType()
			Dim first As EvaluationCalibration = Nothing
			Dim sFirst As String = Nothing
			Try
				For Each globalDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.INT}
					Nd4j.setDefaultDataTypes(globalDtype,If(globalDtype.isFPType(), globalDtype, DataType.DOUBLE))
					For Each lpDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}


						'Test using 5 bins - format: binary softmax-style output
						'Note: no values fall in fourth bin

						'[0, 0.2)
						Dim bin0Probs As INDArray = Nd4j.create(New Double()(){
							New Double() {1.0, 0.0},
							New Double() {0.9, 0.1},
							New Double() {0.85, 0.15}
						}).castTo(lpDtype)
						Dim bin0Labels As INDArray = Nd4j.create(New Double()(){
							New Double() {1.0, 0.0},
							New Double() {1.0, 0.0},
							New Double() {0.0, 1.0}
						}).castTo(lpDtype)

						'[0.2, 0.4)
						Dim bin1Probs As INDArray = Nd4j.create(New Double()(){
							New Double() {0.80, 0.20},
							New Double() {0.7, 0.3},
							New Double() {0.65, 0.35}
						}).castTo(lpDtype)
						Dim bin1Labels As INDArray = Nd4j.create(New Double()(){
							New Double() {1.0, 0.0},
							New Double() {0.0, 1.0},
							New Double() {1.0, 0.0}
						}).castTo(lpDtype)

						'[0.4, 0.6)
						Dim bin2Probs As INDArray = Nd4j.create(New Double()(){
							New Double() {0.59, 0.41},
							New Double() {0.5, 0.5},
							New Double() {0.45, 0.55}
						}).castTo(lpDtype)
						Dim bin2Labels As INDArray = Nd4j.create(New Double()(){
							New Double() {1.0, 0.0},
							New Double() {0.0, 1.0},
							New Double() {0.0, 1.0}
						}).castTo(lpDtype)

						'[0.6, 0.8)
						'Empty

						'[0.8, 1.0]
						Dim bin4Probs As INDArray = Nd4j.create(New Double()(){
							New Double() {0.0, 1.0},
							New Double() {0.1, 0.9}
						}).castTo(lpDtype)
						Dim bin4Labels As INDArray = Nd4j.create(New Double()(){
							New Double() {0.0, 1.0},
							New Double() {0.0, 1.0}
						}).castTo(lpDtype)


						Dim probs As INDArray = Nd4j.vstack(bin0Probs, bin1Probs, bin2Probs, bin4Probs)
						Dim labels As INDArray = Nd4j.vstack(bin0Labels, bin1Labels, bin2Labels, bin4Labels)

						Dim ec As New EvaluationCalibration(5, 5)
						ec.eval(labels, probs)

						For i As Integer = 0 To 0
							Dim avgBinProbsClass() As Double
							Dim fracPos() As Double
							If i = 0 Then
								'Class 0: needs to be handled a little differently, due to threshold/edge cases (0.8, etc)
								avgBinProbsClass = New Double(){0.05, (0.59 + 0.5 + 0.45) / 3, (0.65 + 0.7) / 2.0, (0.8 + 0.85 + 0.9 + 1.0) / 4}
								fracPos = New Double(){0.0 / 2.0, 1.0 / 3, 1.0 / 2, 3.0 / 4}
							Else
								avgBinProbsClass = New Double(){bin0Probs.getColumn(i).meanNumber().doubleValue(), bin1Probs.getColumn(i).meanNumber().doubleValue(), bin2Probs.getColumn(i).meanNumber().doubleValue(), bin4Probs.getColumn(i).meanNumber().doubleValue()}

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
								fracPos = New Double(){bin0Labels.getColumn(i).sumNumber().doubleValue() / bin0Labels.size(0), bin1Labels.getColumn(i).sumNumber().doubleValue() / bin1Labels.size(0), bin2Labels.getColumn(i).sumNumber().doubleValue() / bin2Labels.size(0), bin4Labels.getColumn(i).sumNumber().doubleValue() / bin4Labels.size(0)}
							End If

							Dim rd As org.nd4j.evaluation.curves.ReliabilityDiagram = ec.getReliabilityDiagram(i)

							Dim x() As Double = rd.getMeanPredictedValueX()
							Dim y() As Double = rd.getFractionPositivesY()

							assertArrayEquals(avgBinProbsClass, x, 1e-3)
							assertArrayEquals(fracPos, y, 1e-3)

							Dim s As String = ec.stats()
							If first Is Nothing Then
								first = ec
								sFirst = s
							Else
	'                            assertEquals(first, ec);
								assertEquals(sFirst, s)
								assertTrue(first.getRDiagBinPosCount().equalsWithEps(ec.getRDiagBinPosCount(),If(lpDtype = DataType.HALF, 1e-3, 1e-5))) 'Lower precision due to fload
								assertTrue(first.getRDiagBinTotalCount().equalsWithEps(ec.getRDiagBinTotalCount(),If(lpDtype = DataType.HALF, 1e-3, 1e-5)))
								assertTrue(first.getRDiagBinSumPredictions().equalsWithEps(ec.getRDiagBinSumPredictions(),If(lpDtype = DataType.HALF, 1e-3, 1e-5)))
								assertArrayEquals(first.LabelCountsEachClass, ec.LabelCountsEachClass)
								assertArrayEquals(first.PredictionCountsEachClass, ec.PredictionCountsEachClass)
								assertTrue(first.getProbHistogramOverall().equalsWithEps(ec.getProbHistogramOverall(),If(lpDtype = DataType.HALF, 1e-3, 1e-5)))
								assertTrue(first.getProbHistogramByLabelClass().equalsWithEps(ec.getProbHistogramByLabelClass(),If(lpDtype = DataType.HALF, 1e-3, 1e-5)))
							End If
						Next i
					Next lpDtype
				Next globalDtype
			Finally
				Nd4j.setDefaultDataTypes(dtypeBefore, dtypeBefore)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLabelAndPredictionCounts(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLabelAndPredictionCounts(ByVal backend As Nd4jBackend)

			Dim minibatch As Integer = 50
			Dim nClasses As Integer = 3

			Dim arr As INDArray = Nd4j.rand(minibatch, nClasses)
			arr.diviColumnVector(arr.sum(1))
			Dim labels As INDArray = Nd4j.zeros(minibatch, nClasses)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nClasses), 1.0)
			Next i

			Dim ec As New EvaluationCalibration(5, 5)
			ec.eval(labels, arr)

			Dim expLabelCounts() As Integer = labels.sum(0).data().asInt()
			Dim expPredictionCount(CInt(labels.size(1)) - 1) As Integer
			Dim argmax As INDArray = Nd4j.argMax(arr, 1)
			For i As Integer = 0 To argmax.length() - 1
				expPredictionCount(argmax.getInt(i)) += 1
			Next i

			assertArrayEquals(expLabelCounts, ec.LabelCountsEachClass)
			assertArrayEquals(expPredictionCount, ec.PredictionCountsEachClass)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testResidualPlots(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testResidualPlots(ByVal backend As Nd4jBackend)

			Dim minibatch As Integer = 50
			Dim nClasses As Integer = 3

			Dim arr As INDArray = Nd4j.rand(minibatch, nClasses)
			arr.diviColumnVector(arr.sum(1))
			Dim labels As INDArray = Nd4j.zeros(minibatch, nClasses)
			Dim r As New Random(12345)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, r.Next(nClasses), 1.0)
			Next i

			Dim numBins As Integer = 5
			Dim ec As New EvaluationCalibration(numBins, numBins)
			ec.eval(labels, arr)

			Dim absLabelSubProb As INDArray = Transforms.abs(labels.sub(arr))
			Dim argmaxLabels As INDArray = Nd4j.argMax(labels, 1)

			Dim countsAllClasses(numBins - 1) As Integer
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim countsByClass[][] As Integer = new Integer[nClasses][numBins] 'Histogram count of |label[x] - p(x)|; rows x are over classes
			Dim countsByClass()() As Integer = RectangularArrays.RectangularIntegerArray(nClasses, numBins) 'Histogram count of |label[x] - p(x)|; rows x are over classes
			Dim binSize As Double = 1.0 / numBins

			For i As Integer = 0 To minibatch - 1
				Dim actualClassIdx As Integer = argmaxLabels.getInt(i)
				For j As Integer = 0 To nClasses - 1
					Dim labelSubProb As Double = absLabelSubProb.getDouble(i, j)
					For k As Integer = 0 To numBins - 1
						Dim binLower As Double = k * binSize
						Dim binUpper As Double = (k + 1) * binSize
						If k = numBins - 1 Then
							binUpper = 1.0
						End If

						If labelSubProb >= binLower AndAlso labelSubProb < binUpper Then
							countsAllClasses(k) += 1
							If j = actualClassIdx Then
								countsByClass(j)(k) += 1
							End If
						End If
					Next k
				Next j
			Next i

			'Check residual plot - all classes/predictions
			Dim rpAllClasses As org.nd4j.evaluation.curves.Histogram = ec.ResidualPlotAllClasses
			Dim rpAllClassesBinCounts() As Integer = rpAllClasses.BinCounts
			assertArrayEquals(countsAllClasses, rpAllClassesBinCounts)

			'Check residual plot - split by labels for each class
			' i.e., histogram of |label[x] - p(x)| only for those examples where label[x] == 1
			For i As Integer = 0 To nClasses - 1
				Dim rpCurrClass As org.nd4j.evaluation.curves.Histogram = ec.getResidualPlot(i)
				Dim rpCurrClassCounts() As Integer = rpCurrClass.BinCounts

				'            System.out.println(Arrays.toString(countsByClass[i]));
				'            System.out.println(Arrays.toString(rpCurrClassCounts));

				assertArrayEquals(countsByClass(i), rpCurrClassCounts,"Class: " & i)
			Next i



			'Check overall probability distribution
			Dim probCountsAllClasses(numBins - 1) As Integer
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim probCountsByClass[][] As Integer = new Integer[nClasses][numBins] 'Histogram count of |label[x] - p(x)|; rows x are over classes
			Dim probCountsByClass()() As Integer = RectangularArrays.RectangularIntegerArray(nClasses, numBins) 'Histogram count of |label[x] - p(x)|; rows x are over classes
			For i As Integer = 0 To minibatch - 1
				Dim actualClassIdx As Integer = argmaxLabels.getInt(i)
				For j As Integer = 0 To nClasses - 1
					Dim prob As Double = arr.getDouble(i, j)
					For k As Integer = 0 To numBins - 1
						Dim binLower As Double = k * binSize
						Dim binUpper As Double = (k + 1) * binSize
						If k = numBins - 1 Then
							binUpper = 1.0
						End If

						If prob >= binLower AndAlso prob < binUpper Then
							probCountsAllClasses(k) += 1
							If j = actualClassIdx Then
								probCountsByClass(j)(k) += 1
							End If
						End If
					Next k
				Next j
			Next i

			Dim allProb As org.nd4j.evaluation.curves.Histogram = ec.ProbabilityHistogramAllClasses
			Dim actProbCountsAllClasses() As Integer = allProb.BinCounts

			assertArrayEquals(probCountsAllClasses, actProbCountsAllClasses)

			'Check probability distribution - for each label class
			For i As Integer = 0 To nClasses - 1
				Dim probCurrClass As org.nd4j.evaluation.curves.Histogram = ec.getProbabilityHistogram(i)
				Dim actProbCurrClass() As Integer = probCurrClass.BinCounts

				assertArrayEquals(probCountsByClass(i), actProbCurrClass)
			Next i
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

				Dim e2d As New EvaluationCalibration()
				Dim e4d As New EvaluationCalibration()

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

					Dim e As New EvaluationCalibration()
					e.Axis = i

					e.eval(labels, predictions)
					assertEquals(e2d, e)
				Next i
			Next c
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationCalibration3d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationCalibration3d(ByVal backend As Nd4jBackend)
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

			Dim e3d As New EvaluationCalibration()
			Dim e2d As New EvaluationCalibration()

			e3d.eval(label, prediction)
			e2d.eval(l2d, p2d)

			Console.WriteLine(e2d.stats())

			assertEquals(e2d, e3d)

			assertEquals(e2d.stats(), e3d.stats())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEvaluationCalibration3dMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEvaluationCalibration3dMasking(ByVal backend As Nd4jBackend)
			Dim prediction As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10)
			Dim label As INDArray = Nd4j.rand(DataType.FLOAT, 2, 3, 10)

			Dim rowsP As IList(Of INDArray) = New List(Of INDArray)()
			Dim rowsL As IList(Of INDArray) = New List(Of INDArray)()

			'Check "DL4J-style" 2d per timestep masking [minibatch, seqLength] mask shape
			Dim mask2d As INDArray = Nd4j.randomBernoulli(0.5, 2, 10)
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

			Dim e3d_m2d As New EvaluationCalibration()
			Dim e2d_m2d As New EvaluationCalibration()
			e3d_m2d.eval(label, prediction, mask2d)
			e2d_m2d.eval(l2d, p2d)

			assertEquals(e3d_m2d, e2d_m2d)
		End Sub
	End Class

End Namespace