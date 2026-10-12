Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports EvaluationUtils = org.nd4j.evaluation.EvaluationUtils
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastGreaterThan = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastGreaterThan
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports org.nd4j.common.primitives
Imports NDArrayTextDeSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer
Imports NDArrayTextSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer
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
'ORIGINAL LINE: @NoArgsConstructor @EqualsAndHashCode(callSuper = true) @Data public class EvaluationBinary extends org.nd4j.evaluation.BaseEvaluation<EvaluationBinary>
	Public Class EvaluationBinary
		Inherits BaseEvaluation(Of EvaluationBinary)

		Public NotInheritable Class Metric Implements IMetric
			Public Shared ReadOnly ACCURACY As New Metric("ACCURACY", InnerEnum.ACCURACY)
			Public Shared ReadOnly F1 As New Metric("F1", InnerEnum.F1)
			Public Shared ReadOnly PRECISION As New Metric("PRECISION", InnerEnum.PRECISION)
			Public Shared ReadOnly RECALL As New Metric("RECALL", InnerEnum.RECALL)
			Public Shared ReadOnly GMEASURE As New Metric("GMEASURE", InnerEnum.GMEASURE)
			Public Shared ReadOnly MCC As New Metric("MCC", InnerEnum.MCC)
			Public Shared ReadOnly FAR As New Metric("FAR", InnerEnum.FAR)

			Private Shared ReadOnly valueList As New List(Of Metric)()

			Shared Sub New()
				valueList.Add(ACCURACY)
				valueList.Add(F1)
				valueList.Add(PRECISION)
				valueList.Add(RECALL)
				valueList.Add(GMEASURE)
				valueList.Add(MCC)
				valueList.Add(FAR)
			End Sub

			Public Enum InnerEnum
				ACCURACY
				F1
				PRECISION
				RECALL
				GMEASURE
				MCC
				FAR
			End Enum

			Public ReadOnly innerEnumValue As InnerEnum
			Private ReadOnly nameValue As String
			Private ReadOnly ordinalValue As Integer
			Private Shared nextOrdinal As Integer = 0

			Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
				nameValue = name
				ordinalValue = nextOrdinal
				nextOrdinal += 1
				innerEnumValue = thisInnerEnumValue
			End Sub

			Public ReadOnly Property EvaluationClass As Type Implements IMetric.getEvaluationClass
				Get
					Return GetType(EvaluationBinary)
				End Get
			End Property

			Public Function minimize() As Boolean Implements IMetric.minimize
				Return False
			End Function

			Public Shared Function values() As Metric()
				Return valueList.ToArray()
			End Function

			Public Function ordinal() As Integer
				Return ordinalValue
			End Function

			Public Overrides Function ToString() As String
				Return nameValue
			End Function

			Public Shared Operator =(ByVal one As Metric, ByVal two As Metric) As Boolean
				Return one.innerEnumValue = two.innerEnumValue
			End Operator

			Public Shared Operator <>(ByVal one As Metric, ByVal two As Metric) As Boolean
				Return one.innerEnumValue <> two.innerEnumValue
			End Operator

			Public Shared Function valueOf(ByVal name As String) As Metric
				For Each enumInstance As Metric In Metric.valueList
					If enumInstance.nameValue = name Then
						Return enumInstance
					End If
				Next
				Throw New System.ArgumentException(name)
			End Function
		End Class

		Public Const DEFAULT_PRECISION As Integer = 4
		Public Const DEFAULT_EDGE_VALUE As Double = 0.0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode.Exclude protected int axis = 1;
'JAVA TO VB CONVERTER NOTE: The field axis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend axis_Conflict As Integer = 1

		'Because we want evaluation to work for large numbers of examples - and with low precision (FP16), we won't
		'use INDArrays to store the counts
		Private countTruePositive() As Integer 'P=1, Act=1
		Private countFalsePositive() As Integer 'P=1, Act=0
		Private countTrueNegative() As Integer 'P=0, Act=0
		Private countFalseNegative() As Integer 'P=0, Act=1
'JAVA TO VB CONVERTER NOTE: The field rocBinary was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private rocBinary_Conflict As ROCBinary

		Private labels As IList(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray decisionThreshold;
		Private decisionThreshold As INDArray

		Protected Friend Sub New(ByVal axis As Integer, ByVal rocBinary As ROCBinary, ByVal labels As IList(Of String), ByVal decisionThreshold As INDArray)
			Me.axis_Conflict = axis
			Me.rocBinary_Conflict = rocBinary
			Me.labels = labels
			Me.decisionThreshold = decisionThreshold
		End Sub

		''' <summary>
		''' Create an EvaulationBinary instance with an optional decision threshold array.
		''' </summary>
		''' <param name="decisionThreshold"> Decision threshold for each output; may be null. Should be a row vector with length
		'''                          equal to the number of outputs, with values in range 0 to 1. An array of 0.5 values is
		'''                          equivalent to the default (no manually specified decision threshold). </param>
		Public Sub New(ByVal decisionThreshold As INDArray)
			If decisionThreshold IsNot Nothing Then
				If Not decisionThreshold.RowVectorOrScalar Then
					Throw New System.ArgumentException("Decision threshold array must be a row vector; got array with shape " & Arrays.toString(decisionThreshold.shape()))
				End If
				If decisionThreshold.minNumber().doubleValue() < 0.0 Then
					Throw New System.ArgumentException("Invalid decision threshold array: minimum value is less than 0")
				End If
				If decisionThreshold.maxNumber().doubleValue() > 1.0 Then
					Throw New System.ArgumentException("invalid decision threshold array: maximum value is greater than 1.0")
				End If

				Me.decisionThreshold = decisionThreshold
			End If
		End Sub

		''' <summary>
		''' This constructor allows for ROC to be calculated in addition to the standard evaluation metrics, when the
		''' rocBinarySteps arg is non-null. See <seealso cref="ROCBinary"/> for more details
		''' </summary>
		''' <param name="size">           Number of outputs </param>
		''' <param name="rocBinarySteps"> Constructor arg for <seealso cref="ROCBinary.ROCBinary(Integer)"/> </param>
		Public Sub New(ByVal size As Integer, ByVal rocBinarySteps As Integer?)
			countTruePositive = New Integer(size - 1){}
			countFalsePositive = New Integer(size - 1){}
			countTrueNegative = New Integer(size - 1){}
			countFalseNegative = New Integer(size - 1){}
			If rocBinarySteps IsNot Nothing Then
				rocBinary_Conflict = New ROCBinary(rocBinarySteps)
			End If
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


		Public Overrides Sub eval(ByVal labels As INDArray, ByVal networkPredictions As INDArray)
			eval(labels, networkPredictions, DirectCast(Nothing, INDArray))
		End Sub

		Public Overrides Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray, ByVal recordMetaData As IList(Of T1))
			eval(labels, networkPredictions, maskArray)
		End Sub

		Public Overrides Sub eval(ByVal labelsArr As INDArray, ByVal predictionsArr As INDArray, ByVal maskArr As INDArray)

			'Check for NaNs in predictions - without this, evaulation could silently be intepreted as class 0 prediction due to argmax
			Dim count As Long = Nd4j.Executioner.execAndReturn(New MatchCondition(predictionsArr, Conditions.Nan)).getFinalResult().longValue()
			Preconditions.checkState(count = 0, "Cannot perform evaluation with NaNs present in predictions:" & " %s NaNs present in predictions INDArray", count)

			If countTruePositive IsNot Nothing AndAlso countTruePositive.Length <> labelsArr.size(axis_Conflict) Then
				Throw New System.InvalidOperationException("Labels array does not match stored state size. Expected labels array with " & "size " & countTruePositive.Length & ", got labels array with size " & labelsArr.size(axis_Conflict) & " for axis " & axis_Conflict)
			End If

			Dim p As Triple(Of INDArray, INDArray, INDArray) = BaseEvaluation.reshapeAndExtractNotMasked(labelsArr, predictionsArr, maskArr, axis_Conflict)
			Dim labels As INDArray = p.getFirst()
			Dim predictions As INDArray = p.getSecond()
			Dim maskArray As INDArray = p.getThird()

			If labels.dataType() <> predictions.dataType() Then
				labels = labels.castTo(predictions.dataType())
			End If

			If decisionThreshold IsNot Nothing AndAlso decisionThreshold.dataType() <> predictions.dataType() Then
				decisionThreshold = decisionThreshold.castTo(predictions.dataType())
			End If

			'First: binarize the network prediction probabilities, threshold 0.5 unless otherwise specified
			'This gives us 3 binary arrays: labels, predictions, masks
			Dim classPredictions As INDArray
			If decisionThreshold IsNot Nothing Then
				classPredictions = Nd4j.createUninitialized(DataType.BOOL, predictions.shape())
				Nd4j.Executioner.exec(New BroadcastGreaterThan(predictions, decisionThreshold, classPredictions, 1))
			Else
				classPredictions = predictions.gt(0.5)
			End If
			classPredictions = classPredictions.castTo(predictions.dataType())

			Dim notLabels As INDArray = labels.rsub(1.0) 'If labels are 0 or 1, then rsub(1) swaps
			Dim notClassPredictions As INDArray = classPredictions.rsub(1.0)

			Dim truePositives As INDArray = classPredictions.mul(labels) '1s where predictions are 1, and labels are 1. 0s elsewhere
			Dim trueNegatives As INDArray = notClassPredictions.mul(notLabels) '1s where predictions are 0, and labels are 0. 0s elsewhere
			Dim falsePositives As INDArray = classPredictions.mul(notLabels) '1s where predictions are 1, labels are 0
			Dim falseNegatives As INDArray = notClassPredictions.mul(labels) '1s where predictions are 0, labels are 1

			If maskArray IsNot Nothing Then
				'By multiplying by mask, we keep only those 1s that are actually present
				maskArray = maskArray.castTo(truePositives.dataType())
				truePositives.muli(maskArray)
				trueNegatives.muli(maskArray)
				falsePositives.muli(maskArray)
				falseNegatives.muli(maskArray)
			End If

			Dim tpCount() As Integer = truePositives.sum(0).data().asInt()
			Dim tnCount() As Integer = trueNegatives.sum(0).data().asInt()
			Dim fpCount() As Integer = falsePositives.sum(0).data().asInt()
			Dim fnCount() As Integer = falseNegatives.sum(0).data().asInt()

			If countTruePositive Is Nothing Then
				Dim l As Integer = tpCount.Length
				countTruePositive = New Integer(l - 1){}
				countFalsePositive = New Integer(l - 1){}
				countTrueNegative = New Integer(l - 1){}
				countFalseNegative = New Integer(l - 1){}
			End If

			addInPlace(countTruePositive, tpCount)
			addInPlace(countFalsePositive, fpCount)
			addInPlace(countTrueNegative, tnCount)
			addInPlace(countFalseNegative, fnCount)

			If rocBinary_Conflict IsNot Nothing Then
				rocBinary_Conflict.eval(labels, predictions, maskArray)
			End If
		End Sub

		''' <summary>
		''' Merge the other evaluation object into this one. The result is that this <seealso cref="EvaluationBinary"/>  instance contains the counts
		''' etc from both
		''' </summary>
		''' <param name="other"> EvaluationBinary object to merge into this one. </param>
		Public Overrides Sub merge(ByVal other As EvaluationBinary)
			If other.countTruePositive Is Nothing Then
				'Other is empty - no op
				Return
			End If

			If countTruePositive Is Nothing Then
				'This evaluation is empty -> take results from other
				Me.countTruePositive = other.countTruePositive
				Me.countFalsePositive = other.countFalsePositive
				Me.countTrueNegative = other.countTrueNegative
				Me.countFalseNegative = other.countFalseNegative
				Me.rocBinary_Conflict = other.rocBinary_Conflict
			Else
				If Me.countTruePositive.Length <> other.countTruePositive.Length Then
					Throw New System.InvalidOperationException("Cannot merge EvaluationBinary instances with different sizes. This " & "size: " & Me.countTruePositive.Length & ", other size: " & other.countTruePositive.Length)
				End If

				'Both have stats
				addInPlace(Me.countTruePositive, other.countTruePositive)
				addInPlace(Me.countTrueNegative, other.countTrueNegative)
				addInPlace(Me.countFalsePositive, other.countFalsePositive)
				addInPlace(Me.countFalseNegative, other.countFalseNegative)

				If Me.rocBinary_Conflict IsNot Nothing Then
					Me.rocBinary_Conflict.merge(other.rocBinary_Conflict)
				End If
			End If
		End Sub

		Public Overrides Sub reset()
			countTruePositive = Nothing
		End Sub

		Private Shared Sub addInPlace(ByVal addTo() As Integer, ByVal toAdd() As Integer)
			For i As Integer = 0 To addTo.Length - 1
				addTo(i) += toAdd(i)
			Next i
		End Sub

		''' <summary>
		''' Returns the number of labels - (i.e., size of the prediction/labels arrays) - if known. Returns -1 otherwise
		''' </summary>
		Public Overridable Function numLabels() As Integer
			If countTruePositive Is Nothing Then
				Return -1
			End If

			Return countTruePositive.Length
		End Function

		''' <summary>
		''' Set the label names, for printing via <seealso cref="stats()"/>
		''' </summary>
		Public Overridable WriteOnly Property LabelNames As IList(Of String)
			Set(ByVal labels As IList(Of String))
				If labels Is Nothing Then
					Me.labels = Nothing
					Return
				End If
				Me.labels = New List(Of String)(labels)
			End Set
		End Property

		''' <summary>
		''' Get the total number of values for the specified column, accounting for any masking
		''' </summary>
		Public Overridable Function totalCount(ByVal outputNum As Integer) As Integer
			assertIndex(outputNum)
			Return countTruePositive(outputNum) + countTrueNegative(outputNum) + countFalseNegative(outputNum) + countFalsePositive(outputNum)
		End Function



		''' <summary>
		''' Get the true positives count for the specified output
		''' </summary>
		Public Overridable Function truePositives(ByVal outputNum As Integer) As Integer
			assertIndex(outputNum)
			Return countTruePositive(outputNum)
		End Function

		''' <summary>
		''' Get the true negatives count for the specified output
		''' </summary>
		Public Overridable Function trueNegatives(ByVal outputNum As Integer) As Integer
			assertIndex(outputNum)
			Return countTrueNegative(outputNum)
		End Function

		''' <summary>
		''' Get the false positives count for the specified output
		''' </summary>
		Public Overridable Function falsePositives(ByVal outputNum As Integer) As Integer
			assertIndex(outputNum)
			Return countFalsePositive(outputNum)
		End Function

		''' <summary>
		''' Get the false negatives count for the specified output
		''' </summary>
		Public Overridable Function falseNegatives(ByVal outputNum As Integer) As Integer
			assertIndex(outputNum)
			Return countFalseNegative(outputNum)
		End Function

		Public Overridable Function averageAccuracy() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += accuracy(i)
				i += 1
			Loop

			ret /= CDbl(numLabels())
			Return ret
		End Function

		''' <summary>
		''' Get the accuracy for the specified output
		''' </summary>
		Public Overridable Function accuracy(ByVal outputNum As Integer) As Double
			assertIndex(outputNum)
			Return (countTruePositive(outputNum) + countTrueNegative(outputNum)) / CDbl(totalCount(outputNum))
		End Function

		Public Overridable Function averagePrecision() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += precision(i)
				i += 1
			Loop

			ret /= CDbl(numLabels())
			Return ret
		End Function

		''' <summary>
		''' Get the precision (tp / (tp + fp)) for the specified output
		''' </summary>
		Public Overridable Function precision(ByVal outputNum As Integer) As Double
			assertIndex(outputNum)
			'double precision = tp / (double) (tp + fp);
			Return countTruePositive(outputNum) / CDbl(countTruePositive(outputNum) + countFalsePositive(outputNum))
		End Function


		Public Overridable Function averageRecall() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += recall(i)
				i += 1
			Loop

			ret /= CDbl(numLabels())
			Return ret
		End Function

		''' <summary>
		''' Get the recall (tp / (tp + fn)) for the specified output
		''' </summary>
		Public Overridable Function recall(ByVal outputNum As Integer) As Double
			assertIndex(outputNum)
			Return countTruePositive(outputNum) / CDbl(countTruePositive(outputNum) + countFalseNegative(outputNum))
		End Function


		Public Overridable Function averageF1() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += f1(i)
				i += 1
			Loop

			ret /= CDbl(numLabels())
			Return ret
		End Function

		''' <summary>
		''' Calculate the F-beta value for the given output
		''' </summary>
		''' <param name="beta">      Beta value to use </param>
		''' <param name="outputNum"> Output number </param>
		''' <returns> F-beta for the given output </returns>
		Public Overridable Function fBeta(ByVal beta As Double, ByVal outputNum As Integer) As Double
			assertIndex(outputNum)
			Dim precision As Double = Me.precision(outputNum)
			Dim recall As Double = Me.recall(outputNum)
			Return EvaluationUtils.fBeta(beta, precision, recall)
		End Function

		''' <summary>
		''' Get the F1 score for the specified output
		''' </summary>
		Public Overridable Function f1(ByVal outputNum As Integer) As Double
			Return fBeta(1.0, outputNum)
		End Function

		''' <summary>
		''' Calculate the Matthews correlation coefficient for the specified output
		''' </summary>
		''' <param name="outputNum"> Output number </param>
		''' <returns> Matthews correlation coefficient </returns>
		Public Overridable Function matthewsCorrelation(ByVal outputNum As Integer) As Double
			assertIndex(outputNum)

			Return EvaluationUtils.matthewsCorrelation(truePositives(outputNum), falsePositives(outputNum), falseNegatives(outputNum), trueNegatives(outputNum))
		End Function

		''' <summary>
		''' Macro average of the Matthews correlation coefficient (MCC) (see <seealso cref="matthewsCorrelation(Integer)"/>) for all labels.
		''' </summary>
		''' <returns> The macro average of the MCC for all labels. </returns>
		Public Overridable Function averageMatthewsCorrelation() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += matthewsCorrelation(i)
				i += 1
			Loop

			ret /= CDbl(numLabels())
			Return ret
		End Function

		''' <summary>
		''' Calculate the macro average G-measure for the given output
		''' </summary>
		''' <param name="output"> The specified output </param>
		''' <returns> The macro average of the G-measure for the specified output </returns>
		Public Overridable Function gMeasure(ByVal output As Integer) As Double
			Dim precision As Double = Me.precision(output)
			Dim recall As Double = Me.recall(output)
			Return EvaluationUtils.gMeasure(precision, recall)
		End Function

		''' <summary>
		''' Average G-measure (see <seealso cref="gMeasure(Integer)"/>) for all labels.
		''' </summary>
		''' <returns> The G-measure for all labels. </returns>
		Public Overridable Function averageGMeasure() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += gMeasure(i)
				i += 1
			Loop

			ret /= CDbl(numLabels())
			Return ret
		End Function

		''' <summary>
		''' Returns the false positive rate for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <returns> fpr as a double </returns>
		Public Overridable Function falsePositiveRate(ByVal classLabel As Integer) As Double
			assertIndex(classLabel)
			Return falsePositiveRate(classLabel, DEFAULT_EDGE_VALUE)
		End Function

		''' <summary>
		''' Returns the false positive rate for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <param name="edgeCase">   What to output in case of 0/0 </param>
		''' <returns> fpr as a double </returns>
		Public Overridable Function falsePositiveRate(ByVal classLabel As Integer, ByVal edgeCase As Double) As Double
			Dim fpCount As Double = falsePositives(classLabel)
			Dim tnCount As Double = trueNegatives(classLabel)

			Return EvaluationUtils.falsePositiveRate(CLng(Math.Truncate(fpCount)), CLng(Math.Truncate(tnCount)), edgeCase)
		End Function

		''' <summary>
		''' Returns the false negative rate for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <returns> fnr as a double </returns>
		Public Overridable Function falseNegativeRate(ByVal classLabel As Integer?) As Double
			Return falseNegativeRate(classLabel, DEFAULT_EDGE_VALUE)
		End Function

		''' <summary>
		''' Returns the false negative rate for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <param name="edgeCase">   What to output in case of 0/0 </param>
		''' <returns> fnr as a double </returns>
		Public Overridable Function falseNegativeRate(ByVal classLabel As Integer?, ByVal edgeCase As Double) As Double
			Dim fnCount As Double = falseNegatives(classLabel)
			Dim tpCount As Double = truePositives(classLabel)

			Return EvaluationUtils.falseNegativeRate(CLng(Math.Truncate(fnCount)), CLng(Math.Truncate(tpCount)), edgeCase)
		End Function

		''' <summary>
		''' Returns the <seealso cref="ROCBinary"/> instance, if present
		''' </summary>
		Public Overridable ReadOnly Property ROCBinary As ROCBinary
			Get
				Return rocBinary_Conflict
			End Get
		End Property

		Private Sub assertIndex(ByVal outputNum As Integer)
			If countTruePositive Is Nothing Then
				Throw New System.NotSupportedException("EvaluationBinary does not have any stats: eval must be called first")
			End If
			If outputNum < 0 OrElse outputNum >= countTruePositive.Length Then
				Throw New System.ArgumentException("Invalid input: output number must be between 0 and " & (outputNum - 1) & ". Got index: " & outputNum)
			End If
		End Sub

		''' <summary>
		''' Average False Alarm Rate (FAR) (see <seealso cref="falseAlarmRate(Integer)"/>) for all labels.
		''' </summary>
		''' <returns> The FAR for all labels. </returns>
		Public Overridable Function averageFalseAlarmRate() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += falseAlarmRate(i)
				i += 1
			Loop

			ret /= CDbl(numLabels())
			Return ret
		End Function

		''' <summary>
		''' False Alarm Rate (FAR) reflects rate of misclassified to classified records
		''' <a href="http://ro.ecu.edu.au/cgi/viewcontent.cgi?article=1058&context=isw">http://ro.ecu.edu.au/cgi/viewcontent.cgi?article=1058&context=isw</a><br>
		''' </summary>
		''' <param name="outputNum"> Class index to calculate False Alarm Rate (FAR) </param>
		''' <returns> The FAR for the outcomes </returns>
		Public Overridable Function falseAlarmRate(ByVal outputNum As Integer) As Double
			assertIndex(outputNum)

			Return (falsePositiveRate(outputNum) + falseNegativeRate(outputNum)) / 2.0
		End Function

		''' <summary>
		''' Get a String representation of the EvaluationBinary class, using the default precision
		''' </summary>
		Public Overrides Function stats() As String
			Return stats(DEFAULT_PRECISION)
		End Function

		''' <summary>
		''' Get a String representation of the EvaluationBinary class, using the specified precision
		''' </summary>
		''' <param name="printPrecision"> The precision (number of decimal places) for the accuracy, f1, etc. </param>
		Public Overridable Overloads Function stats(ByVal printPrecision As Integer) As String

			Dim sb As New StringBuilder()

			'Report: Accuracy, precision, recall, F1. Then: confusion matrix

			Dim maxLabelsLength As Integer = 15
			If labels IsNot Nothing Then
				For Each s As String In labels
					maxLabelsLength = Math.Max(s.Length, maxLabelsLength)
				Next s
			End If

			Dim subPattern As String = "%-12." & printPrecision & "f"
			Dim pattern As String = "%-" & (maxLabelsLength + 5) & "s" & subPattern & subPattern & subPattern & subPattern & "%-8d%-7d%-7d%-7d%-7d" 'Total count, TP, TN, FP, FN

			Dim patternHeader As String = "%-" & (maxLabelsLength + 5) & "s%-12s%-12s%-12s%-12s%-8s%-7s%-7s%-7s%-7s"



			Dim headerNames As IList(Of String) = New List(Of String) From {"Label", "Accuracy", "F1", "Precision", "Recall", "Total", "TP", "TN", "FP", "FN"}

			If rocBinary_Conflict IsNot Nothing Then
				patternHeader &= "%-12s"
				pattern &= subPattern

				headerNames = New List(Of String)(headerNames)
				headerNames.Add("AUC")
			End If

			Dim header As String = String.format(patternHeader, headerNames.ToArray())


			sb.Append(header)

			If countTrueNegative IsNot Nothing Then

				For i As Integer = 0 To countTrueNegative.Length - 1
					Dim totalCount As Integer = Me.totalCount(i)

					Dim acc As Double = accuracy(i)
					Dim f1 As Double = Me.f1(i)
					Dim precision As Double = Me.precision(i)
					Dim recall As Double = Me.recall(i)

					Dim label As String = (If(labels Is Nothing, i.ToString(), labels(i)))

					Dim args As IList(Of Object) = New List(Of Object) From {Of Object}
					If rocBinary_Conflict IsNot Nothing Then
						args = New List(Of Object)(args)
						args.Add(rocBinary_Conflict.calculateAUC(i))
					End If

					sb.Append(vbLf).Append(String.format(pattern, args.ToArray()))
				Next i

				If decisionThreshold IsNot Nothing Then
					sb.Append(vbLf & "Per-output decision thresholds: ").Append(Arrays.toString(decisionThreshold.dup().data().asFloat()))
				End If
			Else
				'Empty evaluation
				sb.Append(vbLf & "-- No Data --" & vbLf)
			End If

			Return sb.ToString()
		End Function

		''' <summary>
		''' Calculate specific metric (see <seealso cref="Metric"/>) for a given label.
		''' </summary>
		''' <param name="metric"> The Metric to calculate. </param>
		''' <param name="outputNum"> Class index to calculate.
		''' </param>
		''' <returns> Calculated metric. </returns>
		Public Overridable Function scoreForMetric(ByVal metric As Metric, ByVal outputNum As Integer) As Double
			Select Case metric.innerEnumValue
				Case org.nd4j.evaluation.classification.EvaluationBinary.Metric.InnerEnum.ACCURACY
					Return accuracy(outputNum)
				Case org.nd4j.evaluation.classification.EvaluationBinary.Metric.InnerEnum.F1
					Return f1(outputNum)
				Case org.nd4j.evaluation.classification.EvaluationBinary.Metric.InnerEnum.PRECISION
					Return precision(outputNum)
				Case org.nd4j.evaluation.classification.EvaluationBinary.Metric.InnerEnum.RECALL
					Return recall(outputNum)
				Case org.nd4j.evaluation.classification.EvaluationBinary.Metric.InnerEnum.GMEASURE
					Return gMeasure(outputNum)
				Case org.nd4j.evaluation.classification.EvaluationBinary.Metric.InnerEnum.MCC
					Return matthewsCorrelation(outputNum)
				Case org.nd4j.evaluation.classification.EvaluationBinary.Metric.InnerEnum.FAR
					Return falseAlarmRate(outputNum)
				Case Else
					Throw New System.InvalidOperationException("Unknown metric: " & metric)
			End Select
		End Function

		Public Shared Function fromJson(ByVal json As String) As EvaluationBinary
			Return fromJson(json, GetType(EvaluationBinary))
		End Function

		Public Shared Function fromYaml(ByVal yaml As String) As EvaluationBinary
			Return fromYaml(yaml, GetType(EvaluationBinary))
		End Function

		Public Overrides Function getValue(ByVal metric As IMetric) As Double
			If TypeOf metric Is Metric Then
				Select Case DirectCast(metric, Metric)
					Case ACCURACY
						Return averageAccuracy()
					Case F1
						Return averageF1()
					Case PRECISION
						Return averagePrecision()
					Case RECALL
						Return averageRecall()
					Case GMEASURE
						Return averageGMeasure()
					Case MCC
						Return averageMatthewsCorrelation()
					Case FAR
						Return averageFalseAlarmRate()
					Case Else
						Throw New System.InvalidOperationException("Can't get value for non-binary evaluation Metric " & metric)
				End Select
			Else
				Throw New System.InvalidOperationException("Can't get value for non-binary evaluation Metric " & metric)
			End If
		End Function

		Public Overrides Function newInstance() As EvaluationBinary
			If rocBinary_Conflict IsNot Nothing Then
				Return New EvaluationBinary(axis_Conflict, rocBinary_Conflict.newInstance(), labels, decisionThreshold)
			Else
				Return New EvaluationBinary(axis_Conflict, Nothing, labels, decisionThreshold)
			End If
		End Function
	End Class

End Namespace