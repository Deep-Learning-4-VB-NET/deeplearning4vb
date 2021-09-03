Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric
Imports Histogram = org.nd4j.evaluation.curves.Histogram
Imports ReliabilityDiagram = org.nd4j.evaluation.curves.ReliabilityDiagram
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports LossUtil = org.nd4j.linalg.lossfunctions.LossUtil
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports NDArrayDeSerializer = org.nd4j.serde.jackson.shaded.NDArrayDeSerializer
Imports NDArraySerializer = org.nd4j.serde.jackson.shaded.NDArraySerializer
Imports NDArrayTextDeSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer
Imports NDArrayTextSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize

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

Namespace org.nd4j.evaluation.classification


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @EqualsAndHashCode public class EvaluationCalibration extends org.nd4j.evaluation.BaseEvaluation<EvaluationCalibration>
	Public Class EvaluationCalibration
		Inherits BaseEvaluation(Of EvaluationCalibration)

		Public Const DEFAULT_RELIABILITY_DIAG_NUM_BINS As Integer = 10
		Public Const DEFAULT_HISTOGRAM_NUM_BINS As Integer = 50

		Private ReadOnly reliabilityDiagNumBins As Integer
		Private ReadOnly histogramNumBins As Integer
		Private ReadOnly excludeEmptyBins As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode.Exclude protected int axis = 1;
'JAVA TO VB CONVERTER NOTE: The field axis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend axis_Conflict As Integer = 1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArraySerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray rDiagBinPosCount;
		Private rDiagBinPosCount As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArraySerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray rDiagBinTotalCount;
		Private rDiagBinTotalCount As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArraySerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray rDiagBinSumPredictions;
		Private rDiagBinSumPredictions As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray labelCountsEachClass;
'JAVA TO VB CONVERTER NOTE: The field labelCountsEachClass was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private labelCountsEachClass_Conflict As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray predictionCountsEachClass;
'JAVA TO VB CONVERTER NOTE: The field predictionCountsEachClass was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private predictionCountsEachClass_Conflict As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray residualPlotOverall;
		Private residualPlotOverall As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArraySerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray residualPlotByLabelClass;
		Private residualPlotByLabelClass As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray probHistogramOverall;
		Private probHistogramOverall As INDArray 'Simple histogram over all probabilities
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArraySerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray probHistogramByLabelClass;
		Private probHistogramByLabelClass As INDArray 'Histogram - for each label class separately

		Protected Friend Sub New(ByVal axis As Integer, ByVal reliabilityDiagNumBins As Integer, ByVal histogramNumBins As Integer, ByVal excludeEmptyBins As Boolean)
			Me.axis_Conflict = axis
			Me.reliabilityDiagNumBins = reliabilityDiagNumBins
			Me.histogramNumBins = histogramNumBins
			Me.excludeEmptyBins = excludeEmptyBins
		End Sub

		''' <summary>
		''' Create an EvaluationCalibration instance with the default number of bins
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_RELIABILITY_DIAG_NUM_BINS, DEFAULT_HISTOGRAM_NUM_BINS, True)
		End Sub

		''' <summary>
		''' Create an EvaluationCalibration instance with the specified number of bins
		''' </summary>
		''' <param name="reliabilityDiagNumBins"> Number of bins for the reliability diagram (usually 10) </param>
		''' <param name="histogramNumBins">       Number of bins for the histograms </param>
		Public Sub New(ByVal reliabilityDiagNumBins As Integer, ByVal histogramNumBins As Integer)
			Me.New(reliabilityDiagNumBins, histogramNumBins, True)
		End Sub

		''' <summary>
		''' Create an EvaluationCalibration instance with the specified number of bins
		''' </summary>
		''' <param name="reliabilityDiagNumBins"> Number of bins for the reliability diagram (usually 10) </param>
		''' <param name="histogramNumBins">       Number of bins for the histograms </param>
		''' <param name="excludeEmptyBins">       For the reliability diagram,  whether empty bins should be excluded </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluationCalibration(@JsonProperty("reliabilityDiagNumBins") int reliabilityDiagNumBins, @JsonProperty("histogramNumBins") int histogramNumBins, @JsonProperty("excludeEmptyBins") boolean excludeEmptyBins)
		Public Sub New(ByVal reliabilityDiagNumBins As Integer, ByVal histogramNumBins As Integer, ByVal excludeEmptyBins As Boolean)
			Me.reliabilityDiagNumBins = reliabilityDiagNumBins
			Me.histogramNumBins = histogramNumBins
			Me.excludeEmptyBins = excludeEmptyBins
		End Sub

		''' <summary>
		''' Set the axis for evaluation - this is the dimension along which the probability (and label classes) are present.<br>
		''' For DL4J, this can be left as the default setting (axis = 1).<br>
		''' Axis should be set as follows:<br>
		''' For 2D (OutputLayer), shape [minibatch, numClasses] - axis = 1<br>
		''' For 3D, RNNs/CNN1D (DL4J RnnOutputLayer), NCW format, shape [minibatch, numClasses, sequenceLength] - axis = 1<br>
		''' For 3D, RNNs/CNN1D (DL4J RnnOutputLayer), NWC format, shape [minibatch, sequenceLength, numClasses] - axis = 2<br>
		''' For 4D, CNN2D (DL4J CnnLossLayer), NCHW format, shape [minibatch, channels, height, width] - axis = 1<br>
		''' For 4D, CNN2D, NHWC format, shape [minibatch, height, width, channels] - axis = 3<br>
		''' </summary>
		''' <param name="axis"> Axis to use for evaluation </param>
		Public Overridable Property Axis As Integer
			Set(ByVal axis As Integer)
				Me.axis_Conflict = axis
			End Set
			Get
				Return axis_Conflict
			End Get
		End Property


		Public Overrides Sub eval(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal mask As INDArray)

			Dim triple As Triple(Of INDArray, INDArray, INDArray) = BaseEvaluation.reshapeAndExtractNotMasked(labels, predictions, mask, axis_Conflict)
			If triple Is Nothing Then
				'All values masked out; no-op
				Return
			End If

			Dim labels2d As INDArray = triple.getFirst()
			Dim predictions2d As INDArray = triple.getSecond()
			Dim maskArray As INDArray = triple.getThird()
			Preconditions.checkState(maskArray Is Nothing, "Per-output masking for EvaluationCalibration is not supported")

			'Stats for the reliability diagram: one reliability diagram for each class
			' For each bin, we need: (a) the number of positive cases AND total cases, (b) the average probability

			Dim nClasses As val = labels2d.size(1)

			If rDiagBinPosCount Is Nothing Then
				Dim dt As DataType = DataType.DOUBLE
				'Initialize
				rDiagBinPosCount = Nd4j.create(DataType.LONG, reliabilityDiagNumBins, nClasses)
				rDiagBinTotalCount = Nd4j.create(DataType.LONG, reliabilityDiagNumBins, nClasses)
				rDiagBinSumPredictions = Nd4j.create(dt, reliabilityDiagNumBins, nClasses)

				labelCountsEachClass_Conflict = Nd4j.create(DataType.LONG, 1, nClasses)
				predictionCountsEachClass_Conflict = Nd4j.create(DataType.LONG, 1, nClasses)

				residualPlotOverall = Nd4j.create(dt, 1, histogramNumBins)
				residualPlotByLabelClass = Nd4j.create(dt, histogramNumBins, nClasses)

				probHistogramOverall = Nd4j.create(dt, 1, histogramNumBins)
				probHistogramByLabelClass = Nd4j.create(dt, histogramNumBins, nClasses)
			End If


			'First: loop over classes, determine positive count and total count - for each bin
			Dim histogramBinSize As Double = 1.0 / histogramNumBins
			Dim reliabilityBinSize As Double = 1.0 / reliabilityDiagNumBins

			Dim p As INDArray = predictions2d
			Dim l As INDArray = labels2d

			If maskArray IsNot Nothing Then
				'2 options: per-output masking, or
				If maskArray.ColumnVectorOrScalar Then
					'Per-example masking
					l = l.mulColumnVector(maskArray)
				Else
					l = l.mul(maskArray)
				End If
			End If

			For j As Integer = 0 To reliabilityDiagNumBins - 1
				Dim geqBinLower As INDArray = p.gte(j * reliabilityBinSize).castTo(predictions2d.dataType())
				Dim ltBinUpper As INDArray
				If j = reliabilityDiagNumBins - 1 Then
					'Handle edge case
					ltBinUpper = p.lte(1.0).castTo(predictions2d.dataType())
				Else
					ltBinUpper = p.lt((j + 1) * reliabilityBinSize).castTo(predictions2d.dataType())
				End If

				'Calculate bit-mask over each entry - whether that entry is in the current bin or not
				Dim currBinBitMask As INDArray = geqBinLower.muli(ltBinUpper)
				If maskArray IsNot Nothing Then
					If maskArray.ColumnVectorOrScalar Then
						currBinBitMask.muliColumnVector(maskArray)
					Else
						currBinBitMask.muli(maskArray)
					End If
				End If

				Dim isPosLabelForBin As INDArray = l.mul(currBinBitMask)
				Dim maskedProbs As INDArray = predictions2d.mul(currBinBitMask)

				Dim numPredictionsCurrBin As INDArray = currBinBitMask.sum(0)

				rDiagBinSumPredictions.getRow(j).addi(maskedProbs.sum(0).castTo(rDiagBinSumPredictions.dataType()))
				rDiagBinPosCount.getRow(j).addi(isPosLabelForBin.sum(0).castTo(rDiagBinPosCount.dataType()))
				rDiagBinTotalCount.getRow(j).addi(numPredictionsCurrBin.castTo(rDiagBinTotalCount.dataType()))
			Next j


			'Second, we want histograms of:
			'(a) Distribution of label classes: label counts for each class
			'(b) Distribution of prediction classes: prediction counts for each class
			'(c) residual plots, for each class - (i) all instances, (ii) positive instances only, (iii) negative only
			'(d) Histograms of probabilities, for each class

			labelCountsEachClass_Conflict.addi(labels2d.sum(0).castTo(labelCountsEachClass_Conflict.dataType()))
			'For prediction counts: do an IsMax op, but we need to take masking into account...
			Dim isPredictedClass As INDArray = Nd4j.Executioner.exec(New IsMax(p, p.ulike(), 1))(0)
			If maskArray IsNot Nothing Then
				LossUtil.applyMask(isPredictedClass, maskArray)
			End If
			predictionCountsEachClass_Conflict.addi(isPredictedClass.sum(0).castTo(predictionCountsEachClass_Conflict.dataType()))



			'Residual plots: want histogram of |labels - predicted prob|

			'ND4J's histogram op: dynamically calculates the bin positions, which is not what I want here...
			Dim labelsSubPredicted As INDArray = labels2d.sub(predictions2d)
			Dim maskedProbs As INDArray = predictions2d.dup()
			Transforms.abs(labelsSubPredicted, False)

			'if masking: replace entries with < 0 to effectively remove them
			If maskArray IsNot Nothing Then
				'Assume per-example masking
				Dim newMask As INDArray = maskArray.mul(-10)
				labelsSubPredicted.addiColumnVector(newMask)
				maskedProbs.addiColumnVector(newMask)
			End If

			For j As Integer = 0 To histogramNumBins - 1
				Dim geqBinLower As INDArray = labelsSubPredicted.gte(j * histogramBinSize).castTo(predictions2d.dataType())
				Dim ltBinUpper As INDArray
				Dim geqBinLowerProbs As INDArray = maskedProbs.gte(j * histogramBinSize).castTo(predictions2d.dataType())
				Dim ltBinUpperProbs As INDArray
				If j = histogramNumBins - 1 Then
					'Handle edge case
					ltBinUpper = labelsSubPredicted.lte(1.0).castTo(predictions2d.dataType())
					ltBinUpperProbs = maskedProbs.lte(1.0).castTo(predictions2d.dataType())
				Else
					ltBinUpper = labelsSubPredicted.lt((j + 1) * histogramBinSize).castTo(predictions2d.dataType())
					ltBinUpperProbs = maskedProbs.lt((j + 1) * histogramBinSize).castTo(predictions2d.dataType())
				End If

				Dim currBinBitMask As INDArray = geqBinLower.muli(ltBinUpper)
				Dim currBinBitMaskProbs As INDArray = geqBinLowerProbs.muli(ltBinUpperProbs)

				Dim newTotalCount As Integer = residualPlotOverall.getInt(0, j) + currBinBitMask.sumNumber().intValue()
				residualPlotOverall.putScalar(0, j, newTotalCount)

				'Counts for positive class only: values are in the current bin AND it's a positive label
				Dim isPosLabelForBin As INDArray = l.mul(currBinBitMask)

				residualPlotByLabelClass.getRow(j).addi(isPosLabelForBin.sum(0).castTo(residualPlotByLabelClass.dataType()))

				Dim probNewTotalCount As Integer = probHistogramOverall.getInt(0, j) + currBinBitMaskProbs.sumNumber().intValue()
				probHistogramOverall.putScalar(0, j, probNewTotalCount)

				Dim isPosLabelForBinProbs As INDArray = l.mul(currBinBitMaskProbs)
				Dim temp As INDArray = isPosLabelForBinProbs.sum(0)
				probHistogramByLabelClass.getRow(j).addi(temp.castTo(probHistogramByLabelClass.dataType()))
			Next j
		End Sub

		Public Overrides Sub eval(ByVal labels As INDArray, ByVal networkPredictions As INDArray)
			eval(labels, networkPredictions, DirectCast(Nothing, INDArray))
		End Sub

		Public Overrides Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray, ByVal recordMetaData As IList(Of T1))
			eval(labels, networkPredictions, maskArray)
		End Sub

		Public Overrides Sub merge(ByVal other As EvaluationCalibration)
			If reliabilityDiagNumBins <> other.reliabilityDiagNumBins Then
				Throw New System.NotSupportedException("Cannot merge EvaluationCalibration instances with different numbers of bins")
			End If

			If other.rDiagBinPosCount Is Nothing Then
				Return
			End If

			If rDiagBinPosCount Is Nothing Then
				Me.rDiagBinPosCount = other.rDiagBinPosCount
				Me.rDiagBinTotalCount = other.rDiagBinTotalCount
				Me.rDiagBinSumPredictions = other.rDiagBinSumPredictions
			End If

			Me.rDiagBinPosCount.addi(other.rDiagBinPosCount)
			Me.rDiagBinTotalCount.addi(other.rDiagBinTotalCount)
			Me.rDiagBinSumPredictions.addi(other.rDiagBinSumPredictions)
		End Sub

		Public Overrides Sub reset()
			rDiagBinPosCount = Nothing
			rDiagBinTotalCount = Nothing
			rDiagBinSumPredictions = Nothing
		End Sub

		Public Overrides Function stats() As String
			Return "EvaluationCalibration(nBins=" & reliabilityDiagNumBins & ")"
		End Function

		Public Overridable Function numClasses() As Integer
			If rDiagBinTotalCount Is Nothing Then
				Return -1
			End If

			Return CInt(rDiagBinTotalCount.size(1))
		End Function

		''' <summary>
		''' Get the reliability diagram for the specified class
		''' </summary>
		''' <param name="classIdx"> Index of the class to get the reliability diagram for </param>
		Public Overridable Function getReliabilityDiagram(ByVal classIdx As Integer) As ReliabilityDiagram
			Preconditions.checkState(rDiagBinPosCount IsNot Nothing, "Unable to get reliability diagram: no evaluation has been performed (no data)")
			Dim totalCountBins As INDArray = rDiagBinTotalCount.getColumn(classIdx)
			Dim countPositiveBins As INDArray = rDiagBinPosCount.getColumn(classIdx)

			Dim meanPredictionBins() As Double = rDiagBinSumPredictions.getColumn(classIdx).castTo(DataType.DOUBLE).div(totalCountBins.castTo(DataType.DOUBLE)).data().asDouble()

			Dim fracPositives() As Double = countPositiveBins.castTo(DataType.DOUBLE).div(totalCountBins.castTo(DataType.DOUBLE)).data().asDouble()

			If excludeEmptyBins Then
				Dim condition As val = New MatchCondition(totalCountBins, Conditions.equals(0))
				Dim numZeroBins As Integer = Nd4j.Executioner.exec(condition).getInt(0)
				If numZeroBins <> 0 Then
					Dim mpb() As Double = meanPredictionBins
					Dim fp() As Double = fracPositives

					meanPredictionBins = New Double(CInt(totalCountBins.length() - numZeroBins) - 1){}
					fracPositives = New Double(meanPredictionBins.Length - 1){}
					Dim j As Integer = 0
					For i As Integer = 0 To mpb.Length - 1
						If totalCountBins.getDouble(i) <> 0 Then
							meanPredictionBins(j) = mpb(i)
							fracPositives(j) = fp(i)
							j += 1
						End If
					Next i
				End If
			End If
			Dim title As String = "Reliability Diagram: Class " & classIdx
			Return New ReliabilityDiagram(title, meanPredictionBins, fracPositives)
		End Function

		''' <returns> The number of observed labels for each class. For N classes, be returned array is of length N, with
		''' out[i] being the number of labels of class i </returns>
		Public Overridable ReadOnly Property LabelCountsEachClass As Integer()
			Get
				Return If(labelCountsEachClass_Conflict Is Nothing, Nothing, labelCountsEachClass_Conflict.data().asInt())
			End Get
		End Property

		''' <returns> The number of network predictions for each class. For N classes, be returned array is of length N, with
		''' out[i] being the number of predicted values (max probability) for class i </returns>
		Public Overridable ReadOnly Property PredictionCountsEachClass As Integer()
			Get
				Return If(predictionCountsEachClass_Conflict Is Nothing, Nothing, predictionCountsEachClass_Conflict.data().asInt())
			End Get
		End Property

		''' <summary>
		''' Get the residual plot for all classes combined. The residual plot is defined as a histogram of<br>
		''' |label_i - prob(class_i | input)| for all classes i and examples.<br>
		''' In general, small residuals indicate a superior classifier to large residuals.
		''' </summary>
		''' <returns> Residual plot (histogram) - all predictions/classes </returns>
		Public Overridable ReadOnly Property ResidualPlotAllClasses As Histogram
			Get
				Dim title As String = "Residual Plot - All Predictions and Classes"
				Dim counts() As Integer = residualPlotOverall.data().asInt()
				Return New Histogram(title, 0.0, 1.0, counts)
			End Get
		End Property

		''' <summary>
		''' Get the residual plot, only for examples of the specified class.. The residual plot is defined as a histogram of<br>
		''' |label_i - prob(class_i | input)| for all and examples; for this particular method, only predictions where
		''' i == labelClassIdx are included.<br>
		''' In general, small residuals indicate a superior classifier to large residuals.
		''' </summary>
		''' <param name="labelClassIdx"> Index of the class to get the residual plot for </param>
		''' <returns> Residual plot (histogram) - all predictions/classes </returns>
		Public Overridable Function getResidualPlot(ByVal labelClassIdx As Integer) As Histogram
			Preconditions.checkState(rDiagBinPosCount IsNot Nothing, "Unable to get residual plot: no evaluation has been performed (no data)")
			Dim title As String = "Residual Plot - Predictions for Label Class " & labelClassIdx
			Dim counts() As Integer = residualPlotByLabelClass.getColumn(labelClassIdx).dup().data().asInt()
			Return New Histogram(title, 0.0, 1.0, counts)
		End Function

		''' <summary>
		''' Return a probability histogram for all predictions/classes.
		''' </summary>
		''' <returns> Probability histogram </returns>
		Public Overridable ReadOnly Property ProbabilityHistogramAllClasses As Histogram
			Get
				Dim title As String = "Network Probabilities Histogram - All Predictions and Classes"
				Dim counts() As Integer = probHistogramOverall.data().asInt()
				Return New Histogram(title, 0.0, 1.0, counts)
			End Get
		End Property

		''' <summary>
		''' Return a probability histogram of the specified label class index. That is, for label class index i,
		''' a histogram of P(class_i | input) is returned, only for those examples that are labelled as class i.
		''' </summary>
		''' <param name="labelClassIdx"> Index of the label class to get the histogram for </param>
		''' <returns> Probability histogram </returns>
		Public Overridable Function getProbabilityHistogram(ByVal labelClassIdx As Integer) As Histogram
			Preconditions.checkState(rDiagBinPosCount IsNot Nothing, "Unable to get probability histogram: no evaluation has been performed (no data)")
			Dim title As String = "Network Probabilities Histogram - P(class " & labelClassIdx & ") - Data Labelled Class " & labelClassIdx & " Only"
			Dim counts() As Integer = probHistogramByLabelClass.getColumn(labelClassIdx).dup().data().asInt()
			Return New Histogram(title, 0.0, 1.0, counts)
		End Function

		Public Shared Function fromJson(ByVal json As String) As EvaluationCalibration
			Return fromJson(json, GetType(EvaluationCalibration))
		End Function

		Public Overrides Function getValue(ByVal metric As IMetric) As Double
			Throw New System.InvalidOperationException("Can't get value for non-calibration Metric " & metric)
		End Function

		Public Overrides Function newInstance() As EvaluationCalibration
			Return New EvaluationCalibration(axis_Conflict, reliabilityDiagNumBins, histogramNumBins, excludeEmptyBins)
		End Function
	End Class

End Namespace