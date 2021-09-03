Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports val = lombok.val
Imports org.nd4j.evaluation
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric
Imports PrecisionRecallCurve = org.nd4j.evaluation.curves.PrecisionRecallCurve
Imports RocCurve = org.nd4j.evaluation.curves.RocCurve
Imports ROCArraySerializer = org.nd4j.evaluation.serde.ROCArraySerializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class ROCBinary extends org.nd4j.evaluation.BaseEvaluation<ROCBinary>
	Public Class ROCBinary
		Inherits BaseEvaluation(Of ROCBinary)

		Public Const DEFAULT_STATS_PRECISION As Integer = 4

		''' <summary>
		''' AUROC: Area under ROC curve<br>
		''' AUPRC: Area under Precision-Recall Curve
		''' </summary>
		Public NotInheritable Class Metric Implements IMetric
			Public Shared ReadOnly AUROC As New Metric("AUROC", InnerEnum.AUROC)
			Public Shared ReadOnly AUPRC As New Metric("AUPRC", InnerEnum.AUPRC)

			Private Shared ReadOnly valueList As New List(Of Metric)()

			Shared Sub New()
				valueList.Add(AUROC)
				valueList.Add(AUPRC)
			End Sub

			Public Enum InnerEnum
				AUROC
				AUPRC
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
					Return GetType(ROCBinary)
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.evaluation.serde.ROCArraySerializer.class) private ROC[] underlying;
		Private underlying() As ROC

		Private thresholdSteps As Integer
		Private rocRemoveRedundantPts As Boolean
		Private labels As IList(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode.Exclude protected int axis = 1;
'JAVA TO VB CONVERTER NOTE: The field axis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend axis_Conflict As Integer = 1

		Protected Friend Sub New(ByVal axis As Integer, ByVal thresholdSteps As Integer, ByVal rocRemoveRedundantPts As Boolean, ByVal labels As IList(Of String))
			Me.thresholdSteps = thresholdSteps
			Me.rocRemoveRedundantPts = rocRemoveRedundantPts
			Me.axis_Conflict = axis
			Me.labels = labels
		End Sub

		Public Sub New()
			Me.New(0)
		End Sub

		''' <param name="thresholdSteps"> Number of threshold steps to use for the ROC calculation. Set to 0 for exact ROC calculation </param>
		Public Sub New(ByVal thresholdSteps As Integer)
			Me.New(thresholdSteps, True)
		End Sub

		''' <param name="thresholdSteps"> Number of threshold steps to use for the ROC calculation. If set to 0: use exact calculation </param>
		''' <param name="rocRemoveRedundantPts"> Usually set to true. If true,  remove any redundant points from ROC and P-R curves </param>
		Public Sub New(ByVal thresholdSteps As Integer, ByVal rocRemoveRedundantPts As Boolean)
			Me.thresholdSteps = thresholdSteps
			Me.rocRemoveRedundantPts = rocRemoveRedundantPts
		End Sub

		''' <summary>
		''' Set the axis for evaluation - this is the dimension along which the probability (and label independent binary classes) are present.<br>
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


		Public Overrides Sub reset()
			underlying = Nothing
		End Sub

		Public Overrides Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal mask As INDArray, ByVal recordMetaData As IList(Of T1))
			Dim p As Triple(Of INDArray, INDArray, INDArray) = BaseEvaluation.reshapeAndExtractNotMasked(labels, predictions, mask, axis_Conflict)
			Dim labels2d As INDArray = p.getFirst()
			Dim predictions2d As INDArray = p.getSecond()
			Dim maskArray As INDArray = p.getThird()

			If underlying IsNot Nothing AndAlso underlying.Length <> labels2d.size(1) Then
				Throw New System.InvalidOperationException("Labels array does not match stored state size. Expected labels array with " & "size " & underlying.Length & ", got labels array with size " & labels2d.size(1))
			End If

			If labels2d.rank() = 3 Then
				evalTimeSeries(labels2d, predictions2d, maskArray)
				Return
			End If

			If labels2d.dataType() <> predictions2d.dataType() Then
				labels2d = labels2d.castTo(predictions2d.dataType())
			End If

			Dim n As Integer = CInt(labels2d.size(1))
			If underlying Is Nothing Then
				underlying = New ROC(n - 1){}
				For i As Integer = 0 To n - 1
					underlying(i) = New ROC(thresholdSteps, rocRemoveRedundantPts)
				Next i
			End If

			Dim perExampleNonMaskedIdxs() As Integer = Nothing
			For i As Integer = 0 To n - 1
				Dim prob As INDArray = predictions2d.getColumn(i).reshape(ChrW(predictions2d.size(0)), 1)
				Dim label As INDArray = labels2d.getColumn(i).reshape(ChrW(labels2d.size(0)), 1)
				If maskArray IsNot Nothing Then
					'If mask array is present, pull out the non-masked rows only
					Dim m As INDArray
					Dim perExampleMasking As Boolean = False
					If maskArray.ColumnVectorOrScalar Then
						'Per-example masking
						m = maskArray
						perExampleMasking = True
					Else
						'Per-output masking
						m = maskArray.getColumn(i)
					End If
					Dim rowsToPull() As Integer

					If perExampleNonMaskedIdxs IsNot Nothing Then
						'Reuse, per-example masking
						rowsToPull = perExampleNonMaskedIdxs
					Else
						Dim nonMaskedCount As Integer = m.sumNumber().intValue()
						rowsToPull = New Integer(nonMaskedCount - 1){}
						Dim maskSize As val = m.size(0)
						Dim used As Integer = 0
						For j As Integer = 0 To maskSize - 1
							If m.getDouble(j) <> 0.0 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: rowsToPull[used++] = j;
								rowsToPull(used) = j
									used += 1
							End If
						Next j
						If perExampleMasking Then
							perExampleNonMaskedIdxs = rowsToPull
						End If
					End If

					'TODO Temporary workaround for: https://github.com/eclipse/deeplearning4j/issues/7102
					If prob.View Then
						prob = prob.dup()
					End If
					If label.View Then
						label = label.dup()
					End If

					prob = Nd4j.pullRows(prob, 1, rowsToPull) '1: tensor along dim 1
					label = Nd4j.pullRows(label, 1, rowsToPull)
				End If

				underlying(i).eval(label, prob)
			Next i
		End Sub

		Public Overrides Sub merge(ByVal other As ROCBinary)
			If Me.underlying Is Nothing Then
				Me.underlying = other.underlying
				Return
			ElseIf other.underlying Is Nothing Then
				Return
			End If

			'Both have data
			If underlying.Length <> other.underlying.Length Then
				Throw New System.NotSupportedException("Cannot merge ROCBinary: this expects " & underlying.Length & "outputs, other expects " & other.underlying.Length & " outputs")
			End If
			For i As Integer = 0 To underlying.Length - 1
				Me.underlying(i).merge(other.underlying(i))
			Next i
		End Sub

		Private Sub assertIndex(ByVal outputNum As Integer)
			If underlying Is Nothing Then
				Throw New System.NotSupportedException("ROCBinary does not have any stats: eval must be called first")
			End If
			If outputNum < 0 OrElse outputNum >= underlying.Length Then
				Throw New System.ArgumentException("Invalid input: output number must be between 0 and " & (outputNum - 1))
			End If
		End Sub

		''' <summary>
		''' Returns the number of labels - (i.e., size of the prediction/labels arrays) - if known. Returns -1 otherwise
		''' </summary>
		Public Overridable Function numLabels() As Integer
			If underlying Is Nothing Then
				Return -1
			End If

			Return underlying.Length
		End Function

		''' <summary>
		''' Get the actual positive count (accounting for any masking) for  the specified output/column
		''' </summary>
		''' <param name="outputNum"> Index of the output (0 to <seealso cref="numLabels()"/>-1) </param>
		Public Overridable Function getCountActualPositive(ByVal outputNum As Integer) As Long
			assertIndex(outputNum)
			Return underlying(outputNum).getCountActualPositive()
		End Function

		''' <summary>
		''' Get the actual negative count (accounting for any masking) for  the specified output/column
		''' </summary>
		''' <param name="outputNum"> Index of the output (0 to <seealso cref="numLabels()"/>-1) </param>
		Public Overridable Function getCountActualNegative(ByVal outputNum As Integer) As Long
			assertIndex(outputNum)
			Return underlying(outputNum).getCountActualNegative()
		End Function

		''' <summary>
		''' Get the ROC object for the specific column </summary>
		''' <param name="outputNum"> Column (output number) </param>
		''' <returns> The underlying ROC object for this specific column </returns>
		Public Overridable Function getROC(ByVal outputNum As Integer) As ROC
			assertIndex(outputNum)
			Return underlying(outputNum)
		End Function

		''' <summary>
		''' Get the ROC curve for the specified output </summary>
		''' <param name="outputNum"> Number of the output to get the ROC curve for </param>
		''' <returns> ROC curve </returns>
		Public Overridable Function getRocCurve(ByVal outputNum As Integer) As RocCurve
			assertIndex(outputNum)

			Return underlying(outputNum).RocCurve
		End Function

		''' <summary>
		''' Get the Precision-Recall curve for the specified output </summary>
		''' <param name="outputNum"> Number of the output to get the P-R curve for </param>
		''' <returns>  Precision recall curve </returns>
		Public Overridable Function getPrecisionRecallCurve(ByVal outputNum As Integer) As PrecisionRecallCurve
			assertIndex(outputNum)
			Return underlying(outputNum).PrecisionRecallCurve
		End Function


		''' <summary>
		''' Macro-average AUC for all outcomes </summary>
		''' <returns> the (macro-)average AUC for all outcomes. </returns>
		Public Overridable Function calculateAverageAuc() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += calculateAUC(i)
				i += 1
			Loop

			Return ret / CDbl(numLabels())
		End Function

		''' <returns> the (macro-)average AUPRC (area under precision recall curve) </returns>
		Public Overridable Function calculateAverageAUCPR() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numLabels()
				ret += calculateAUCPR(i)
				i += 1
			Loop

			Return ret / CDbl(numLabels())
		End Function

		''' <summary>
		''' Calculate the AUC - Area Under (ROC) Curve<br>
		''' Utilizes trapezoidal integration internally
		''' </summary>
		''' <param name="outputNum"> Output number to calculate AUC for </param>
		''' <returns> AUC </returns>
		Public Overridable Function calculateAUC(ByVal outputNum As Integer) As Double
			assertIndex(outputNum)
			Return underlying(outputNum).calculateAUC()
		End Function

		''' <summary>
		''' Calculate the AUCPR - Area Under Curve - Precision Recall<br>
		''' Utilizes trapezoidal integration internally
		''' </summary>
		''' <param name="outputNum"> Output number to calculate AUCPR for </param>
		''' <returns> AUCPR </returns>
		Public Overridable Function calculateAUCPR(ByVal outputNum As Integer) As Double
			assertIndex(outputNum)
			Return underlying(outputNum).calculateAUCPR()
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

		Public Overrides Function stats() As String
			Return stats(DEFAULT_STATS_PRECISION)
		End Function

		Public Overridable Overloads Function stats(ByVal printPrecision As Integer) As String
			'Calculate AUC and also print counts, for each output

			Dim sb As New StringBuilder()

			Dim maxLabelsLength As Integer = 15
			If labels IsNot Nothing Then
				For Each s As String In labels
					maxLabelsLength = Math.Max(s.Length, maxLabelsLength)
				Next s
			End If

			Dim patternHeader As String = "%-" & (maxLabelsLength + 5) & "s%-12s%-12s%-10s%-10s"
			Dim header As String = String.format(patternHeader, "Label", "AUC", "AUPRC", "# Pos", "# Neg")

			Dim pattern As String = "%-" & (maxLabelsLength + 5) & "s" & "%-12." & printPrecision & "f" & "%-12." & printPrecision & "f" & "%-10d%-10d" 'Count pos, count neg

			sb.Append(header)

			If underlying IsNot Nothing Then
				For i As Integer = 0 To underlying.Length - 1
					Dim auc As Double = calculateAUC(i)
					Dim auprc As Double = calculateAUCPR(i)

					Dim label As String = (If(labels Is Nothing, i.ToString(), labels(i)))

					sb.Append(vbLf).Append(String.format(pattern, label, auc, auprc, getCountActualPositive(i), getCountActualNegative(i)))
				Next i

				If thresholdSteps > 0 Then
					sb.Append(vbLf)
					sb.Append("[Note: Thresholded AUC/AUPRC calculation used with ").Append(thresholdSteps).Append(" steps); accuracy may reduced compared to exact mode]")
				End If

			Else
				'Empty evaluation
				sb.Append(vbLf & "-- No Data --" & vbLf)
			End If

			Return sb.ToString()
		End Function

		Public Shared Function fromJson(ByVal json As String) As ROCBinary
			Return fromJson(json, GetType(ROCBinary))
		End Function

		Public Overridable Function scoreForMetric(ByVal metric As Metric, ByVal idx As Integer) As Double
			assertIndex(idx)
			Select Case metric.innerEnumValue
				Case org.nd4j.evaluation.classification.ROCBinary.Metric.InnerEnum.AUROC
					Return calculateAUC(idx)
				Case org.nd4j.evaluation.classification.ROCBinary.Metric.InnerEnum.AUPRC
					Return calculateAUCPR(idx)
				Case Else
					Throw New System.InvalidOperationException("Unknown metric: " & metric)
			End Select
		End Function

		Public Overrides Function getValue(ByVal metric As IMetric) As Double
			If TypeOf metric Is Metric Then
				If metric = Metric.AUPRC Then
					Return calculateAverageAUCPR()
				ElseIf metric = Metric.AUROC Then
					Return calculateAverageAuc()
				Else
					Throw New System.InvalidOperationException("Can't get value for non-binary ROC Metric " & metric)
				End If
			Else
				Throw New System.InvalidOperationException("Can't get value for non-binary ROC Metric " & metric)
			End If
		End Function

		Public Overrides Function newInstance() As ROCBinary
			Return New ROCBinary(axis_Conflict, thresholdSteps, rocRemoveRedundantPts, labels)
		End Function
	End Class

End Namespace