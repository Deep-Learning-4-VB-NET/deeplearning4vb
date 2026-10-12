Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric
Imports PrecisionRecallCurve = org.nd4j.evaluation.curves.PrecisionRecallCurve
Imports RocCurve = org.nd4j.evaluation.curves.RocCurve
Imports ROCArraySerializer = org.nd4j.evaluation.serde.ROCArraySerializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class ROCMultiClass extends org.nd4j.evaluation.BaseEvaluation<ROCMultiClass>
	Public Class ROCMultiClass
		Inherits BaseEvaluation(Of ROCMultiClass)

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
					Return GetType(ROCMultiClass)
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

		Private thresholdSteps As Integer
		Private rocRemoveRedundantPts As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.evaluation.serde.ROCArraySerializer.class) private ROC[] underlying;
		Private underlying() As ROC
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
			'Default to exact
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


		Public Overrides Sub reset()
			underlying = Nothing
		End Sub


		Public Overrides Function stats() As String
			Return stats(DEFAULT_STATS_PRECISION)
		End Function

		Public Overridable Overloads Function stats(ByVal printPrecision As Integer) As String

			Dim sb As New StringBuilder()

			Dim maxLabelsLength As Integer = 15
			If labels IsNot Nothing Then
				For Each s As String In labels
					maxLabelsLength = Math.Max(s.Length, maxLabelsLength)
				Next s
			End If

			Dim patternHeader As String = "%-" & (maxLabelsLength + 5) & "s%-12s%-10s%-10s"
			Dim header As String = String.format(patternHeader, "Label", "AUC", "# Pos", "# Neg")

			Dim pattern As String = "%-" & (maxLabelsLength + 5) & "s" & "%-12." & printPrecision & "f" & "%-10d%-10d" 'Count pos, count neg

			sb.Append(header)

			If underlying IsNot Nothing Then
				For i As Integer = 0 To underlying.Length - 1
					Dim auc As Double = calculateAUC(i)

					Dim label As String = (If(labels Is Nothing, i.ToString(), labels(i)))

					sb.Append(vbLf).Append(String.format(pattern, label, auc, getCountActualPositive(i), getCountActualNegative(i)))
				Next i

'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				sb.Append("Average AUC: ").Append(String.Format("%-12." & printPrecision & "f", calculateAverageAUC()))

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

		''' <summary>
		''' Evaluate the network, with optional metadata
		''' </summary>
		''' <param name="labels">   Data labels </param>
		''' <param name="predictions">        Network predictions </param>
		''' <param name="recordMetaData"> Optional; may be null. If not null, should have size equal to the number of outcomes/guesses
		'''  </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public void eval(org.nd4j.linalg.api.ndarray.INDArray labels, org.nd4j.linalg.api.ndarray.INDArray predictions, org.nd4j.linalg.api.ndarray.INDArray mask, final java.util.List<? extends java.io.Serializable> recordMetaData)
		Public Overrides Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal mask As INDArray, ByVal recordMetaData As IList(Of T1))

			Dim p As Triple(Of INDArray, INDArray, INDArray) = BaseEvaluation.reshapeAndExtractNotMasked(labels, predictions, mask, axis_Conflict)
			If p Is Nothing Then
				'All values masked out; no-op
				Return
			End If

			Dim labels2d As INDArray = p.getFirst()
			Dim predictions2d As INDArray = p.getSecond()
			Dim maskArray As INDArray = p.getThird()
			Preconditions.checkState(maskArray Is Nothing, "Per-output masking for ROCMultiClass is not supported")


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

			If underlying.Length <> labels2d.size(1) Then
				Throw New System.ArgumentException("Cannot evaluate data: number of label classes does not match previous call. " & "Got " & labels2d.size(1) & " labels (from array shape " & Arrays.toString(labels2d.shape()) & ")" & " vs. expected number of label classes = " & underlying.Length)
			End If

			For i As Integer = 0 To n - 1
				Dim prob As INDArray = predictions2d.getColumn(i, True) 'Probability of class i
				Dim label As INDArray = labels2d.getColumn(i, True)
				'Workaround for: https://github.com/eclipse/deeplearning4j/issues/7305
				If prob.rank() = 0 Then
					prob = prob.reshape(ChrW(1), 1)
				End If
				If label.rank() = 0 Then
					label = label.reshape(ChrW(1), 1)
				End If
				underlying(i).eval(label, prob)
			Next i
		End Sub

		''' <summary>
		''' Get the (one vs. all) ROC curve for the specified class </summary>
		''' <param name="classIdx"> Class index to get the ROC curve for </param>
		''' <returns> ROC curve for the given class </returns>
		Public Overridable Function getRocCurve(ByVal classIdx As Integer) As RocCurve
			assertIndex(classIdx)
			Return underlying(classIdx).RocCurve
		End Function

		''' <summary>
		''' Get the (one vs. all) Precision-Recall curve for the specified class </summary>
		''' <param name="classIdx"> Class to get the P-R curve for </param>
		''' <returns>  Precision recall curve for the given class </returns>
		Public Overridable Function getPrecisionRecallCurve(ByVal classIdx As Integer) As PrecisionRecallCurve
			assertIndex(classIdx)
			Return underlying(classIdx).PrecisionRecallCurve
		End Function

		''' <summary>
		''' Calculate the AUC - Area Under ROC Curve<br>
		''' Utilizes trapezoidal integration internally
		''' </summary>
		''' <returns> AUC </returns>
		Public Overridable Function calculateAUC(ByVal classIdx As Integer) As Double
			assertIndex(classIdx)
			Return underlying(classIdx).calculateAUC()
		End Function

		''' <summary>
		''' Calculate the AUPRC - Area Under Curve Precision Recall <br>
		''' Utilizes trapezoidal integration internally
		''' </summary>
		''' <returns> AUC </returns>
		Public Overridable Function calculateAUCPR(ByVal classIdx As Integer) As Double
			assertIndex(classIdx)
			Return underlying(classIdx).calculateAUCPR()
		End Function

		''' <summary>
		''' Calculate the macro-average (one-vs-all) AUC for all classes
		''' </summary>
		Public Overridable Function calculateAverageAUC() As Double
			assertIndex(0)

			Dim sum As Double = 0.0
			For i As Integer = 0 To underlying.Length - 1
				sum += calculateAUC(i)
			Next i

			Return sum / underlying.Length
		End Function

		''' <summary>
		''' Calculate the macro-average (one-vs-all) AUCPR (area under precision recall curve) for all classes
		''' </summary>
		Public Overridable Function calculateAverageAUCPR() As Double
			Dim sum As Double = 0.0
			For i As Integer = 0 To underlying.Length - 1
				sum += calculateAUCPR(i)
			Next i

			Return sum / underlying.Length
		End Function

		''' <summary>
		''' Get the actual positive count (accounting for any masking) for  the specified class
		''' </summary>
		''' <param name="outputNum"> Index of the class </param>
		Public Overridable Function getCountActualPositive(ByVal outputNum As Integer) As Long
			assertIndex(outputNum)
			Return underlying(outputNum).getCountActualPositive()
		End Function

		''' <summary>
		''' Get the actual negative count (accounting for any masking) for  the specified output/column
		''' </summary>
		''' <param name="outputNum"> Index of the class </param>
		Public Overridable Function getCountActualNegative(ByVal outputNum As Integer) As Long
			assertIndex(outputNum)
			Return underlying(outputNum).getCountActualNegative()
		End Function

		''' <summary>
		''' Merge this ROCMultiClass instance with another.
		''' This ROCMultiClass instance is modified, by adding the stats from the other instance.
		''' </summary>
		''' <param name="other"> ROCMultiClass instance to combine with this one </param>
		Public Overrides Sub merge(ByVal other As ROCMultiClass)
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

		Public Overridable ReadOnly Property NumClasses As Integer
			Get
				If underlying Is Nothing Then
					Return -1
				End If
				Return underlying.Length
			End Get
		End Property


		Private Sub assertIndex(ByVal classIdx As Integer)
			If underlying Is Nothing Then
				Throw New System.InvalidOperationException("Cannot get results: no data has been collected")
			End If
			If classIdx < 0 OrElse classIdx >= underlying.Length Then
				Throw New System.ArgumentException("Invalid class index (" & classIdx & "): must be in range 0 to numClasses = " & underlying.Length)
			End If
		End Sub

		Public Shared Function fromJson(ByVal json As String) As ROCMultiClass
			Return fromJson(json, GetType(ROCMultiClass))
		End Function

		Public Overridable Function scoreForMetric(ByVal metric As Metric, ByVal idx As Integer) As Double
			assertIndex(idx)
			Select Case metric.innerEnumValue
				Case org.nd4j.evaluation.classification.ROCMultiClass.Metric.InnerEnum.AUROC
					Return calculateAUC(idx)
				Case org.nd4j.evaluation.classification.ROCMultiClass.Metric.InnerEnum.AUPRC
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
					Return calculateAverageAUC()
				Else
					Throw New System.InvalidOperationException("Can't get value for non-ROC Metric " & metric)
				End If
			Else
				Throw New System.InvalidOperationException("Can't get value for non-ROC Metric " & metric)
			End If
		End Function

		Public Overrides Function newInstance() As ROCMultiClass
			Return New ROCMultiClass(axis_Conflict, thresholdSteps, rocRemoveRedundantPts, labels)
		End Function
	End Class

End Namespace