Imports System
Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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


	Public Class EvaluationUtils

		''' <summary>
		''' Calculate the precision from true positive and false positive counts
		''' </summary>
		''' <param name="tpCount">  True positive count </param>
		''' <param name="fpCount">  False positive count </param>
		''' <param name="edgeCase"> Edge case value use to avoid 0/0 </param>
		''' <returns> Precision </returns>
		Public Shared Function precision(ByVal tpCount As Long, ByVal fpCount As Long, ByVal edgeCase As Double) As Double
			'Edge case
			If tpCount = 0 AndAlso fpCount = 0 Then
				Return edgeCase
			End If

			Return tpCount / CDbl(tpCount + fpCount)
		End Function

		''' <summary>
		''' Calculate the recall from true positive and false negative counts
		''' </summary>
		''' <param name="tpCount">  True positive count </param>
		''' <param name="fnCount">  False negative count </param>
		''' <param name="edgeCase"> Edge case values used to avoid 0/0 </param>
		''' <returns> Recall </returns>
		Public Shared Function recall(ByVal tpCount As Long, ByVal fnCount As Long, ByVal edgeCase As Double) As Double
			'Edge case
			If tpCount = 0 AndAlso fnCount = 0 Then
				Return edgeCase
			End If

			Return tpCount / CDbl(tpCount + fnCount)
		End Function

		''' <summary>
		''' Calculate the false positive rate from the false positive count and true negative count
		''' </summary>
		''' <param name="fpCount">  False positive count </param>
		''' <param name="tnCount">  True negative count </param>
		''' <param name="edgeCase"> Edge case values are used to avoid 0/0 </param>
		''' <returns> False positive rate </returns>
		Public Shared Function falsePositiveRate(ByVal fpCount As Long, ByVal tnCount As Long, ByVal edgeCase As Double) As Double
			'Edge case
			If fpCount = 0 AndAlso tnCount = 0 Then
				Return edgeCase
			End If
			Return fpCount / CDbl(fpCount + tnCount)
		End Function

		''' <summary>
		''' Calculate the false negative rate from the false negative counts and true positive count
		''' </summary>
		''' <param name="fnCount">  False negative count </param>
		''' <param name="tpCount">  True positive count </param>
		''' <param name="edgeCase"> Edge case value to use to avoid 0/0 </param>
		''' <returns> False negative rate </returns>
		Public Shared Function falseNegativeRate(ByVal fnCount As Long, ByVal tpCount As Long, ByVal edgeCase As Double) As Double
			'Edge case
			If fnCount = 0 AndAlso tpCount = 0 Then
				Return edgeCase
			End If

			Return fnCount / CDbl(fnCount + tpCount)
		End Function

		''' <summary>
		''' Calculate the F beta value from counts
		''' </summary>
		''' <param name="beta"> Beta of value to use </param>
		''' <param name="tp">   True positive count </param>
		''' <param name="fp">   False positive count </param>
		''' <param name="fn">   False negative count </param>
		''' <returns> F beta </returns>
		Public Shared Function fBeta(ByVal beta As Double, ByVal tp As Long, ByVal fp As Long, ByVal fn As Long) As Double
			Dim prec As Double = tp / (CDbl(tp) + fp)
			Dim recall As Double = tp / (CDbl(tp) + fn)
			Return fBeta(beta, prec, recall)
		End Function

		''' <summary>
		''' Calculate the F-beta value from precision and recall
		''' </summary>
		''' <param name="beta">      Beta value to use </param>
		''' <param name="precision"> Precision </param>
		''' <param name="recall">    Recall </param>
		''' <returns> F-beta value </returns>
		Public Shared Function fBeta(ByVal beta As Double, ByVal precision As Double, ByVal recall As Double) As Double
			If precision = 0.0 OrElse recall = 0.0 Then
				Return 0
			End If

			Dim numerator As Double = (1 + beta * beta) * precision * recall
			Dim denominator As Double = beta * beta * precision + recall

			Return numerator / denominator
		End Function

		''' <summary>
		''' Calculate the G-measure from precision and recall
		''' </summary>
		''' <param name="precision"> Precision value </param>
		''' <param name="recall">    Recall value </param>
		''' <returns> G-measure </returns>
		Public Shared Function gMeasure(ByVal precision As Double, ByVal recall As Double) As Double
			Return Math.Sqrt(precision * recall)
		End Function

		''' <summary>
		''' Calculate the binary Matthews correlation coefficient from counts
		''' </summary>
		''' <param name="tp"> True positive count </param>
		''' <param name="fp"> False positive counts </param>
		''' <param name="fn"> False negative counts </param>
		''' <param name="tn"> True negative count </param>
		''' <returns> Matthews correlation coefficient </returns>
		Public Shared Function matthewsCorrelation(ByVal tp As Long, ByVal fp As Long, ByVal fn As Long, ByVal tn As Long) As Double
			Dim numerator As Double = (CDbl(tp)) * tn - (CDbl(fp)) * fn
			Dim denominator As Double = Math.Sqrt((CDbl(tp) + fp) * (tp + fn) * (tn + fp) * (tn + fn))
			Return numerator / denominator
		End Function


		Public Shared Function reshapeTimeSeriesTo2d(ByVal labels As INDArray) As INDArray
			Dim labelsShape As val = labels.shape()
			Dim labels2d As INDArray
			If labelsShape(0) = 1 Then
				labels2d = labels.tensorAlongDimension(0, 1, 2).permutei(1, 0) 'Edge case: miniBatchSize==1
			ElseIf labelsShape(2) = 1 Then
				labels2d = labels.tensorAlongDimension(0, 1, 0) 'Edge case: timeSeriesLength=1
			Else
				labels2d = labels.permute(0, 2, 1)
				labels2d = labels2d.reshape("f"c, labelsShape(0) * labelsShape(2), labelsShape(1))
			End If
			Return labels2d
		End Function

		Public Shared Function extractNonMaskedTimeSteps(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal outputMask As INDArray) As Pair(Of INDArray, INDArray)
			If labels.rank() <> 3 OrElse predicted.rank() <> 3 Then
				Throw New System.ArgumentException("Invalid data: expect rank 3 arrays. Got arrays with shapes labels=" & Arrays.toString(labels.shape()) & ", predictions=" & Arrays.toString(predicted.shape()))
			End If

			'Reshaping here: basically RnnToFeedForwardPreProcessor...
			'Dup to f order, to ensure consistent buffer for reshaping
			labels = labels.dup("f"c)
			predicted = predicted.dup("f"c)

			Dim labels2d As INDArray = EvaluationUtils.reshapeTimeSeriesTo2d(labels)
			Dim predicted2d As INDArray = EvaluationUtils.reshapeTimeSeriesTo2d(predicted)

			If outputMask Is Nothing Then
				Return New Pair(Of INDArray, INDArray)(labels2d, predicted2d)
			End If

			Dim oneDMask As INDArray = reshapeTimeSeriesMaskToVector(outputMask)
			Dim f() As Single = oneDMask.dup().data().asFloat()
			Dim rowsToPull(f.Length - 1) As Integer
			Dim usedCount As Integer = 0
			For i As Integer = 0 To f.Length - 1
				If f(i) = 1.0f Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: rowsToPull[usedCount++] = i;
					rowsToPull(usedCount) = i
						usedCount += 1
				End If
			Next i
			If usedCount = 0 Then
				'Edge case: all time steps are masked -> nothing to extract
				Return Nothing
			End If
			rowsToPull = Arrays.CopyOfRange(rowsToPull, 0, usedCount)

			labels2d = Nd4j.pullRows(labels2d, 1, rowsToPull)
			predicted2d = Nd4j.pullRows(predicted2d, 1, rowsToPull)

			Return New Pair(Of INDArray, INDArray)(labels2d, predicted2d)
		End Function

		''' <summary>
		''' Reshape time series mask arrays. This should match the assumptions (f order, etc) in RnnOutputLayer </summary>
		''' <param name="timeSeriesMask">    Mask array to reshape to a column vector </param>
		''' <returns>                  Mask array as a column vector </returns>
		Public Shared Function reshapeTimeSeriesMaskToVector(ByVal timeSeriesMask As INDArray) As INDArray
			If timeSeriesMask.rank() <> 2 Then
				Throw New System.ArgumentException("Cannot reshape mask: rank is not 2")
			End If

			If timeSeriesMask.ordering() <> "f"c Then
				timeSeriesMask = timeSeriesMask.dup("f"c)
			End If

			Return timeSeriesMask.reshape("f"c, timeSeriesMask.length(), 1)
		End Function
	End Class

End Namespace