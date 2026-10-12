Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports lombok
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric
Imports PrecisionRecallCurve = org.nd4j.evaluation.curves.PrecisionRecallCurve
Imports RocCurve = org.nd4j.evaluation.curves.RocCurve
Imports ROCSerializer = org.nd4j.evaluation.serde.ROCSerializer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Op = org.nd4j.linalg.api.ops.Op
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports CompareAndSet = org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndSet
Imports MulOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize
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

Namespace org.nd4j.evaluation.classification


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true, exclude = {"auc", "auprc", "probAndLabel", "exactAllocBlockSize", "rocCurve", "prCurve", "axis"}) @Data @JsonIgnoreProperties({"probAndLabel", "exactAllocBlockSize"}) @JsonSerialize(using = ROCSerializer.class) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY) public class ROC extends org.nd4j.evaluation.BaseEvaluation<ROC>
	Public Class ROC
		Inherits BaseEvaluation(Of ROC)

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
					Return GetType(ROC)
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

		Private Const DEFAULT_EXACT_ALLOC_BLOCK_SIZE As Integer = 2048
		Private ReadOnly counts As IDictionary(Of Double, CountsForThreshold) = New LinkedHashMap(Of Double, CountsForThreshold)()
		Private thresholdSteps As Integer
		Private countActualPositive As Long
		Private countActualNegative As Long
'JAVA TO VB CONVERTER NOTE: The field auc was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private auc_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field auprc was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private auprc_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field rocCurve was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private rocCurve_Conflict As RocCurve
		Private prCurve As PrecisionRecallCurve

		Private isExact As Boolean
		Private probAndLabel As INDArray
		Private exampleCount As Integer = 0
		Private rocRemoveRedundantPts As Boolean
		Private exactAllocBlockSize As Integer
'JAVA TO VB CONVERTER NOTE: The field axis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend axis_Conflict As Integer = 1



		Public Sub New(ByVal thresholdSteps As Integer, ByVal rocRemoveRedundantPts As Boolean, ByVal exactAllocBlockSize As Integer, ByVal axis As Integer)
			Me.New(thresholdSteps, rocRemoveRedundantPts, exactAllocBlockSize)
			Me.axis_Conflict = axis
		End Sub

		Public Sub New()
			'Default to exact
			Me.New(0)
		End Sub

		''' <param name="thresholdSteps"> Number of threshold steps to use for the ROC calculation. If set to 0: use exact calculation </param>
		Public Sub New(ByVal thresholdSteps As Integer)
			Me.New(thresholdSteps, True)
		End Sub

		''' <param name="thresholdSteps">        Number of threshold steps to use for the ROC calculation. If set to 0: use exact calculation </param>
		''' <param name="rocRemoveRedundantPts"> Usually set to true. If true,  remove any redundant points from ROC and P-R curves </param>
		Public Sub New(ByVal thresholdSteps As Integer, ByVal rocRemoveRedundantPts As Boolean)
			Me.New(thresholdSteps, rocRemoveRedundantPts, DEFAULT_EXACT_ALLOC_BLOCK_SIZE)
		End Sub

		''' <param name="thresholdSteps">        Number of threshold steps to use for the ROC calculation. If set to 0: use exact calculation </param>
		''' <param name="rocRemoveRedundantPts"> Usually set to true. If true,  remove any redundant points from ROC and P-R curves </param>
		''' <param name="exactAllocBlockSize">   if using exact mode, the block size relocation. Users can likely use the default
		'''                              setting in almost all cases </param>
		Public Sub New(ByVal thresholdSteps As Integer, ByVal rocRemoveRedundantPts As Boolean, ByVal exactAllocBlockSize As Integer)


			If thresholdSteps > 0 Then
				Me.thresholdSteps = thresholdSteps

				Dim [step] As Double = 1.0 / thresholdSteps
				For i As Integer = 0 To thresholdSteps
					Dim currThreshold As Double = i * [step]
					counts(currThreshold) = New CountsForThreshold(currThreshold)
				Next i

				isExact = False
			Else
				'Exact

				isExact = True
			End If
			Me.rocRemoveRedundantPts = rocRemoveRedundantPts
			Me.exactAllocBlockSize = exactAllocBlockSize
		End Sub

		Public Shared Function fromJson(ByVal json As String) As ROC
			Return fromJson(json, GetType(ROC))
		End Function

		''' <summary>
		''' Set the axis for evaluation - this should be a size 1 dimension
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




		Private ReadOnly Property Auc As Double
			Get
				If auc_Conflict IsNot Nothing Then
					Return auc_Conflict
				End If
				auc_Conflict = calculateAUC()
				Return auc_Conflict
			End Get
		End Property

		''' <summary>
		''' Calculate the AUROC - Area Under ROC Curve<br>
		''' Utilizes trapezoidal integration internally
		''' </summary>
		''' <returns> AUC </returns>
		Public Overridable Function calculateAUC() As Double
			If auc_Conflict IsNot Nothing Then
				Return auc_Conflict
			End If

			Preconditions.checkState(exampleCount > 0, "Unable to calculate AUC: no evaluation has been performed (no examples)")

			Me.auc_Conflict = RocCurve.calculateAUC()
			Return auc_Conflict
		End Function

		''' <summary>
		''' Get the ROC curve, as a set of (threshold, falsePositive, truePositive) points
		''' </summary>
		''' <returns> ROC curve </returns>
		Public Overridable ReadOnly Property RocCurve As RocCurve
			Get
				If rocCurve_Conflict IsNot Nothing Then
					Return rocCurve_Conflict
				End If
    
				Preconditions.checkState(exampleCount > 0, "Unable to get ROC curve: no evaluation has been performed (no examples)")
    
				If isExact Then
					'Sort ascending. As we decrease threshold, more are predicted positive.
					'if(prob <= threshold> predict 0, otherwise predict 1
					'So, as we iterate from i=0..length, first 0 to i (inclusive) are predicted class 1, all others are predicted class 0
					Dim pl As INDArray = ProbAndLabelUsed
					Dim sorted As INDArray = Nd4j.sortRows(pl, 0, False)
					Dim isPositive As INDArray = sorted.getColumn(1,True)
					Dim isNegative As INDArray = sorted.getColumn(1,True).rsub(1.0)
    
					Dim cumSumPos As INDArray = isPositive.cumsum(-1)
					Dim cumSumNeg As INDArray = isNegative.cumsum(-1)
					Dim length As val = sorted.size(0)
    
					Dim t As INDArray = Nd4j.create(DataType.DOUBLE, length + 2, 1)
					t.put(New INDArrayIndex(){interval(1, length + 1), all()}, sorted.getColumn(0,True))
    
					Dim fpr As INDArray = Nd4j.create(DataType.DOUBLE, length + 2, 1)
					fpr.put(New INDArrayIndex(){interval(1, length + 1), all()}, cumSumNeg.div(countActualNegative))
    
					Dim tpr As INDArray = Nd4j.create(DataType.DOUBLE, length + 2, 1)
					tpr.put(New INDArrayIndex(){interval(1, length + 1), all()}, cumSumPos.div(countActualPositive))
    
					'Edge cases
					t.putScalar(0, 0, 1.0)
					fpr.putScalar(0, 0, 0.0)
					tpr.putScalar(0, 0, 0.0)
					fpr.putScalar(length + 1, 0, 1.0)
					tpr.putScalar(length + 1, 0, 1.0)
    
    
					Dim x_fpr_out() As Double = fpr.data().asDouble()
					Dim y_tpr_out() As Double = tpr.data().asDouble()
					Dim tOut() As Double = t.data().asDouble()
    
					'Note: we can have multiple FPR for a given TPR, and multiple TPR for a given FPR
					'These can be omitted, without changing the area (as long as we keep the edge points)
					If rocRemoveRedundantPts Then
						Dim p As Pair(Of Double()(), Integer()()) = removeRedundant(tOut, x_fpr_out, y_tpr_out, Nothing, Nothing, Nothing)
						Dim temp()() As Double = p.First
						tOut = temp(0)
						x_fpr_out = temp(1)
						y_tpr_out = temp(2)
					End If
    
					Me.rocCurve_Conflict = New RocCurve(tOut, x_fpr_out, y_tpr_out)
    
					Return rocCurve_Conflict
				Else
    
					Dim [out]()() As Double = { New Double(thresholdSteps){}, New Double(thresholdSteps){}, New Double(thresholdSteps){} }
					Dim i As Integer = 0
					For Each entry As KeyValuePair(Of Double, CountsForThreshold) In counts.SetOfKeyValuePairs()
						Dim c As CountsForThreshold = entry.Value
	'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim tpr As Double = c.getCountTruePositive() / (CDbl(countActualPositive))
	'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim fpr As Double = c.getCountFalsePositive() / (CDbl(countActualNegative))
    
						[out](0)(i) = c.getThreshold()
						[out](1)(i) = fpr
						[out](2)(i) = tpr
						i += 1
					Next entry
					Return New RocCurve([out](0), [out](1), [out](2))
				End If
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property ProbAndLabelUsed As INDArray
			Get
				If probAndLabel Is Nothing OrElse exampleCount = 0 Then
					Return Nothing
				End If
				Return probAndLabel.get(interval(0, exampleCount), all())
			End Get
		End Property

		Private Shared Function removeRedundant(ByVal threshold() As Double, ByVal x() As Double, ByVal y() As Double, ByVal tpCount() As Integer, ByVal fpCount() As Integer, ByVal fnCount() As Integer) As Pair(Of Double()(), Integer()())
			Dim t_compacted(threshold.Length - 1) As Double
			Dim x_compacted(x.Length - 1) As Double
			Dim y_compacted(y.Length - 1) As Double
			Dim tp_compacted() As Integer = Nothing
			Dim fp_compacted() As Integer = Nothing
			Dim fn_compacted() As Integer = Nothing
			Dim hasInts As Boolean = False
			If tpCount IsNot Nothing Then
				tp_compacted = New Integer(tpCount.Length - 1){}
				fp_compacted = New Integer(fpCount.Length - 1){}
				fn_compacted = New Integer(fnCount.Length - 1){}
				hasInts = True
			End If
			Dim lastOutPos As Integer = -1
			For i As Integer = 0 To threshold.Length - 1

				Dim keep As Boolean
				If i = 0 OrElse i = threshold.Length - 1 Then
					keep = True
				Else
					Dim ommitSameY As Boolean = y(i - 1) = y(i) AndAlso y(i) = y(i + 1)
					Dim ommitSameX As Boolean = x(i - 1) = x(i) AndAlso x(i) = x(i + 1)
					keep = Not ommitSameX AndAlso Not ommitSameY
				End If

				If keep Then
					lastOutPos += 1
					t_compacted(lastOutPos) = threshold(i)
					y_compacted(lastOutPos) = y(i)
					x_compacted(lastOutPos) = x(i)
					If hasInts Then
						tp_compacted(lastOutPos) = tpCount(i)
						fp_compacted(lastOutPos) = fpCount(i)
						fn_compacted(lastOutPos) = fnCount(i)
					End If
				End If
			Next i

			If lastOutPos < x.Length - 1 Then
				t_compacted = Arrays.CopyOfRange(t_compacted, 0, lastOutPos + 1)
				x_compacted = Arrays.CopyOfRange(x_compacted, 0, lastOutPos + 1)
				y_compacted = Arrays.CopyOfRange(y_compacted, 0, lastOutPos + 1)
				If hasInts Then
					tp_compacted = Arrays.CopyOfRange(tp_compacted, 0, lastOutPos + 1)
					fp_compacted = Arrays.CopyOfRange(fp_compacted, 0, lastOutPos + 1)
					fn_compacted = Arrays.CopyOfRange(fn_compacted, 0, lastOutPos + 1)
				End If
			End If

			Return New Pair(Of Double()(), Integer()())(New Double()(){t_compacted, x_compacted, y_compacted},If(hasInts, New Integer()(){tp_compacted, fp_compacted, fn_compacted}, Nothing))
		End Function

		Private ReadOnly Property Auprc As Double
			Get
				If auprc_Conflict IsNot Nothing Then
					Return auprc_Conflict
				End If
				auprc_Conflict = calculateAUCPR()
				Return auprc_Conflict
			End Get
		End Property

		''' <summary>
		''' Calculate the area under the precision/recall curve - aka AUCPR
		''' 
		''' @return
		''' </summary>
		Public Overridable Function calculateAUCPR() As Double
			If auprc_Conflict IsNot Nothing Then
				Return auprc_Conflict
			End If

			Preconditions.checkState(exampleCount > 0, "Unable to calculate AUPRC: no evaluation has been performed (no examples)")

			auprc_Conflict = PrecisionRecallCurve.calculateAUPRC()
			Return auprc_Conflict
		End Function

		''' <summary>
		''' Get the precision recall curve as array.
		''' return[0] = threshold array<br>
		''' return[1] = precision array<br>
		''' return[2] = recall array<br>
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property PrecisionRecallCurve As PrecisionRecallCurve
			Get
    
				If prCurve IsNot Nothing Then
					Return prCurve
				End If
    
				Preconditions.checkState(exampleCount > 0, "Unable to get PR curve: no evaluation has been performed (no examples)")
    
				Dim thresholdOut() As Double
				Dim precisionOut() As Double
				Dim recallOut() As Double
				Dim tpCountOut() As Integer
				Dim fpCountOut() As Integer
				Dim fnCountOut() As Integer
    
				If isExact Then
					Dim pl As INDArray = ProbAndLabelUsed
					Dim sorted As INDArray = Nd4j.sortRows(pl, 0, False)
					Dim isPositive As INDArray = sorted.getColumn(1,True)
    
					Dim cumSumPos As INDArray = isPositive.cumsum(-1)
					Dim length As val = sorted.size(0)
    
		'            
		'            Sort descending. As we iterate: decrease probability threshold T... all values <= T are predicted
		'            as class 0, all others are predicted as class 1
		'
		'            Precision:  sum(TP) / sum(predicted pos at threshold)
		'            Recall:     sum(TP) / total actual positives
		'
		'            predicted positive at threshold: # values <= threshold, i.e., just i
		'             
    
					Dim t As INDArray = Nd4j.create(DataType.DOUBLE, length + 2, 1)
					t.put(New INDArrayIndex(){interval(1, length + 1), all()}, sorted.getColumn(0,True))
    
					Dim linspace As INDArray = Nd4j.linspace(1, length, length, DataType.DOUBLE)
					Dim precision As INDArray = cumSumPos.castTo(DataType.DOUBLE).div(linspace.reshape(cumSumPos.shape()))
					Dim prec As INDArray = Nd4j.create(DataType.DOUBLE, length + 2, 1)
					prec.put(New INDArrayIndex(){interval(1, length + 1), all()}, precision)
    
					'Recall/TPR
					Dim rec As INDArray = Nd4j.create(DataType.DOUBLE, length + 2, 1)
					rec.put(New INDArrayIndex(){interval(1, length + 1), all()}, cumSumPos.div(countActualPositive))
    
					'Edge cases
					t.putScalar(0, 0, 1.0)
					prec.putScalar(0, 0, 1.0)
					rec.putScalar(0, 0, 0.0)
					prec.putScalar(length + 1, 0, cumSumPos.getDouble(cumSumPos.length() - 1) / length)
					rec.putScalar(length + 1, 0, 1.0)
    
					thresholdOut = t.data().asDouble()
					precisionOut = prec.data().asDouble()
					recallOut = rec.data().asDouble()
    
					'Counts. Note the edge cases
					tpCountOut = New Integer(thresholdOut.Length - 1){}
					fpCountOut = New Integer(thresholdOut.Length - 1){}
					fnCountOut = New Integer(thresholdOut.Length - 1){}
    
					For i As Integer = 1 To tpCountOut.Length - 2
						tpCountOut(i) = cumSumPos.getInt(i - 1)
						fpCountOut(i) = i - tpCountOut(i) 'predicted positive - true positive
						fnCountOut(i) = CInt(countActualPositive) - tpCountOut(i)
					Next i
    
					'Edge cases: last idx -> threshold of 0.0, all predicted positive
					tpCountOut(tpCountOut.Length - 1) = CInt(countActualPositive)
					fpCountOut(tpCountOut.Length - 1) = CInt(exampleCount - countActualPositive)
					fnCountOut(tpCountOut.Length - 1) = 0
					'Edge case: first idx -> threshold of 1.0, all predictions negative
					tpCountOut(0) = 0
					fpCountOut(0) = 0 '(int)(exampleCount - countActualPositive);  //All negatives are predicted positive
					fnCountOut(0) = CInt(countActualPositive)
    
					'Finally: 2 things to do
					'(a) Reverse order: lowest to highest threshold
					'(b) remove unnecessary/rendundant points (doesn't affect graph or AUPRC)
    
					ArrayUtils.reverse(thresholdOut)
					ArrayUtils.reverse(precisionOut)
					ArrayUtils.reverse(recallOut)
					ArrayUtils.reverse(tpCountOut)
					ArrayUtils.reverse(fpCountOut)
					ArrayUtils.reverse(fnCountOut)
    
					If rocRemoveRedundantPts Then
						Dim pair As Pair(Of Double()(), Integer()()) = removeRedundant(thresholdOut, precisionOut, recallOut, tpCountOut, fpCountOut, fnCountOut)
						Dim temp()() As Double = pair.First
						Dim temp2()() As Integer = pair.Second
						thresholdOut = temp(0)
						precisionOut = temp(1)
						recallOut = temp(2)
						tpCountOut = temp2(0)
						fpCountOut = temp2(1)
						fnCountOut = temp2(2)
					End If
				Else
					thresholdOut = New Double(counts.Count - 1){}
					precisionOut = New Double(counts.Count - 1){}
					recallOut = New Double(counts.Count - 1){}
					tpCountOut = New Integer(counts.Count - 1){}
					fpCountOut = New Integer(counts.Count - 1){}
					fnCountOut = New Integer(counts.Count - 1){}
    
					Dim i As Integer = 0
					For Each entry As KeyValuePair(Of Double, CountsForThreshold) In counts.SetOfKeyValuePairs()
						Dim t As Double = entry.Key
						Dim c As CountsForThreshold = entry.Value
						Dim tpCount As Long = c.getCountTruePositive()
						Dim fpCount As Long = c.getCountFalsePositive()
						'For edge cases: http://stats.stackexchange.com/questions/1773/what-are-correct-values-for-precision-and-recall-in-edge-cases
						'precision == 1 when FP = 0 -> no incorrect positive predictions
						'recall == 1 when no dataset positives are present (got all 0 of 0 positives)
						Dim precision As Double
						If tpCount = 0 AndAlso fpCount = 0 Then
							'At this threshold: no predicted positive cases
							precision = 1.0
						Else
							precision = tpCount / CDbl(tpCount + fpCount)
						End If
    
						Dim recall As Double
						If countActualPositive = 0 Then
							recall = 1.0
						Else
							recall = tpCount / (CDbl(countActualPositive))
						End If
    
						thresholdOut(i) = c.getThreshold()
						precisionOut(i) = precision
						recallOut(i) = recall
    
						tpCountOut(i) = CInt(tpCount)
						fpCountOut(i) = CInt(fpCount)
						fnCountOut(i) = CInt(countActualPositive - tpCount)
						i += 1
					Next entry
				End If
    
				prCurve = New PrecisionRecallCurve(thresholdOut, precisionOut, recallOut, tpCountOut, fpCountOut, fnCountOut, exampleCount)
				Return prCurve
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @NoArgsConstructor public static class CountsForThreshold implements java.io.Serializable, Cloneable
		<Serializable>
		Public Class CountsForThreshold
			Implements ICloneable

			Friend threshold As Double
			Friend countTruePositive As Long
			Friend countFalsePositive As Long

			Public Sub New(ByVal threshold As Double)
				Me.New(threshold, 0, 0)
			End Sub

			Public Overrides Function clone() As CountsForThreshold
				Return New CountsForThreshold(threshold, countTruePositive, countFalsePositive)
			End Function

			Public Overridable Sub incrementFalsePositive(ByVal count As Long)
				countFalsePositive += count
			End Sub

			Public Overridable Sub incrementTruePositive(ByVal count As Long)
				countTruePositive += count
			End Sub
		End Class

		''' <summary>
		''' Evaluate (collect statistics for) the given minibatch of data.
		''' For time series (3 dimensions) use <seealso cref="evalTimeSeries(INDArray, INDArray)"/> or <seealso cref="evalTimeSeries(INDArray, INDArray, INDArray)"/>
		''' </summary>
		''' <param name="labels">      Labels / true outcomes </param>
		''' <param name="predictions"> Predictions </param>
		Public Overrides Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal mask As INDArray, ByVal recordMetaData As IList(Of T1))

			Dim p As Triple(Of INDArray, INDArray, INDArray) = BaseEvaluation.reshapeAndExtractNotMasked(labels, predictions, mask, axis_Conflict)
			If p Is Nothing Then
				'All values masked out; no-op
				Return
			End If
			Dim labels2d As INDArray = p.getFirst()
			Dim predictions2d As INDArray = p.getSecond()

			If labels2d.rank() = 3 AndAlso predictions2d.rank() = 3 Then
				'Assume time series input -> reshape to 2d
				evalTimeSeries(labels2d, predictions2d)
			End If
			If labels2d.rank() > 2 OrElse predictions2d.rank() > 2 OrElse labels2d.size(1) <> predictions2d.size(1) OrElse labels2d.size(1) > 2 Then
				Throw New System.ArgumentException("Invalid input data shape: labels shape = " & Arrays.toString(labels2d.shape()) & ", predictions shape = " & Arrays.toString(predictions2d.shape()) & "; require rank 2 array with size(1) == 1 or 2")
			End If

			If labels2d.dataType() <> predictions2d.dataType() Then
				labels2d = labels2d.castTo(predictions2d.dataType())
			End If

			'Check for NaNs in predictions - without this, evaulation could silently be intepreted as class 0 prediction due to argmax
			Dim count As Long = Nd4j.Executioner.execAndReturn(New MatchCondition(predictions2d, Conditions.Nan)).getFinalResult().longValue()
			Preconditions.checkState(count = 0, "Cannot perform evaluation with NaN(s) present:" & " %s NaN(s) present in predictions INDArray", count)

			Dim [step] As Double = 1.0 / thresholdSteps
			Dim singleOutput As Boolean = labels2d.size(1) = 1

			If isExact Then
				'Exact approach: simply add them to the storage for later computation/use

				If probAndLabel Is Nothing Then
					'Do initial allocation
					Dim initialSize As val = Math.Max(labels2d.size(0), exactAllocBlockSize)
					probAndLabel = Nd4j.create(DataType.DOUBLE, New Long(){initialSize, 2}, "c"c) 'First col: probability of class 1. Second col: "is class 1"
				End If

				'Allocate a larger array if necessary
				If exampleCount + labels2d.size(0) >= probAndLabel.size(0) Then
					Dim newSize As val = probAndLabel.size(0) + Math.Max(exactAllocBlockSize, labels2d.size(0))
					Dim newProbAndLabel As INDArray = Nd4j.create(DataType.DOUBLE, New Long(){newSize, 2}, "c"c)
					If exampleCount > 0 Then
						'If statement to handle edge case: no examples, but we need to re-allocate right away
						newProbAndLabel.get(interval(0, exampleCount), all()).assign(probAndLabel.get(interval(0, exampleCount), all()))
					End If
					probAndLabel = newProbAndLabel
				End If

				'put values
				Dim probClass1 As INDArray
				Dim labelClass1 As INDArray
				If singleOutput Then
					probClass1 = predictions2d
					labelClass1 = labels2d
				Else
					probClass1 = predictions2d.getColumn(1,True)
					labelClass1 = labels2d.getColumn(1,True)
				End If
				Dim currMinibatchSize As val = labels2d.size(0)
				probAndLabel.get(interval(exampleCount, exampleCount + currMinibatchSize), NDArrayIndex.point(0)).assign(probClass1)

				probAndLabel.get(interval(exampleCount, exampleCount + currMinibatchSize), NDArrayIndex.point(1)).assign(labelClass1)

				Dim countClass1CurrMinibatch As Integer = labelClass1.sumNumber().intValue()
				countActualPositive += countClass1CurrMinibatch
				countActualNegative += labels2d.size(0) - countClass1CurrMinibatch
			Else
				'Thresholded approach
				Dim positivePredictedClassColumn As INDArray
				Dim positiveActualClassColumn As INDArray
				Dim negativeActualClassColumn As INDArray

				If singleOutput Then
					'Single binary variable case
					positiveActualClassColumn = labels2d
					negativeActualClassColumn = labels2d.rsub(1.0) '1.0 - label
					positivePredictedClassColumn = predictions2d
				Else
					'Standard case - 2 output variables (probability distribution)
					positiveActualClassColumn = labels2d.getColumn(1,True)
					negativeActualClassColumn = labels2d.getColumn(0,True)
					positivePredictedClassColumn = predictions2d.getColumn(1,True)
				End If

				'Increment global counts - actual positive/negative observed
				countActualPositive += positiveActualClassColumn.sumNumber().intValue()
				countActualNegative += negativeActualClassColumn.sumNumber().intValue()

				'Here: calculate true positive rate (TPR) vs. false positive rate (FPR) at different threshold

				Dim ppc As INDArray = Nothing
				Dim itp As INDArray = Nothing
				Dim ifp As INDArray = Nothing
				For i As Integer = 0 To thresholdSteps
					Dim currThreshold As Double = i * [step]

					'Work out true/false positives - do this by replacing probabilities (predictions) with 1 or 0 based on threshold
					Dim condGeq As Condition = Conditions.greaterThanOrEqual(currThreshold)
					Dim condLeq As Condition = Conditions.lessThanOrEqual(currThreshold)

					If ppc Is Nothing Then
						ppc = positivePredictedClassColumn.dup(positiveActualClassColumn.ordering())
					Else
						ppc.assign(positivePredictedClassColumn)
					End If
					Dim op As Op = New CompareAndSet(ppc, 1.0, condGeq)
					Dim predictedClass1 As INDArray = Nd4j.Executioner.exec(op)
					op = New CompareAndSet(predictedClass1, 0.0, condLeq)
					predictedClass1 = Nd4j.Executioner.exec(op)


					'True positives: occur when positive predicted class and actual positive actual class...
					'False positive occurs when positive predicted class, but negative actual class
					Dim isTruePositive As INDArray ' = predictedClass1.mul(positiveActualClassColumn); //If predicted == 1 and actual == 1 at this threshold: 1x1 = 1. 0 otherwise
					Dim isFalsePositive As INDArray ' = predictedClass1.mul(negativeActualClassColumn); //If predicted == 1 and actual == 0 at this threshold: 1x1 = 1. 0 otherwise
					If i = 0 Then
						isTruePositive = predictedClass1.mul(positiveActualClassColumn)
						isFalsePositive = predictedClass1.mul(negativeActualClassColumn)
						itp = isTruePositive
						ifp = isFalsePositive
					Else
						isTruePositive = Nd4j.Executioner.exec(New MulOp(predictedClass1, positiveActualClassColumn, itp))(0)
						isFalsePositive = Nd4j.Executioner.exec(New MulOp(predictedClass1, negativeActualClassColumn, ifp))(0)
					End If

					'Counts for this batch:
					Dim truePositiveCount As Integer = isTruePositive.sumNumber().intValue()
					Dim falsePositiveCount As Integer = isFalsePositive.sumNumber().intValue()

					'Increment counts for this thold
					Dim thresholdCounts As CountsForThreshold = counts(currThreshold)
					thresholdCounts.incrementTruePositive(truePositiveCount)
					thresholdCounts.incrementFalsePositive(falsePositiveCount)
				Next i
			End If

			exampleCount += labels2d.size(0)
			auc_Conflict = Nothing
			auprc_Conflict = Nothing
			rocCurve_Conflict = Nothing
			prCurve = Nothing
		End Sub

		''' <summary>
		''' Merge this ROC instance with another.
		''' This ROC instance is modified, by adding the stats from the other instance.
		''' </summary>
		''' <param name="other"> ROC instance to combine with this one </param>
		Public Overrides Sub merge(ByVal other As ROC)
			If Me.thresholdSteps <> other.thresholdSteps Then
				Throw New System.NotSupportedException("Cannot merge ROC instances with different numbers of threshold steps (" & Me.thresholdSteps & " vs. " & other.thresholdSteps & ")")
			End If
			Me.countActualPositive += other.countActualPositive
			Me.countActualNegative += other.countActualNegative
			Me.auc_Conflict = Nothing
			Me.auprc_Conflict = Nothing
			Me.rocCurve_Conflict = Nothing
			Me.prCurve = Nothing


			If isExact Then
				If other.exampleCount = 0 Then
					Return
				End If

				If Me.exampleCount = 0 Then
					Me.exampleCount = other.exampleCount
					Me.probAndLabel = other.probAndLabel
					Return
				End If

				If Me.exampleCount + other.exampleCount > Me.probAndLabel.size(0) Then
					'Allocate new array
					Dim newSize As val = Me.probAndLabel.size(0) + Math.Max(other.probAndLabel.size(0), exactAllocBlockSize)
					Dim newProbAndLabel As INDArray = Nd4j.create(DataType.DOUBLE, newSize, 2)
					newProbAndLabel.put(New INDArrayIndex(){interval(0, exampleCount), all()}, probAndLabel.get(interval(0, exampleCount), all()))
					probAndLabel = newProbAndLabel
				End If

				Dim toPut As INDArray = other.probAndLabel.get(interval(0, other.exampleCount), all())
				probAndLabel.put(New INDArrayIndex(){ interval(exampleCount, exampleCount + other.exampleCount), all()}, toPut)
			Else
				For Each d As Double? In Me.counts.Keys
					Dim cft As CountsForThreshold = Me.counts(d)
					Dim otherCft As CountsForThreshold = other.counts(d)
					cft.countTruePositive += otherCft.countTruePositive
					cft.countFalsePositive += otherCft.countFalsePositive
				Next d
			End If

			Me.exampleCount += other.exampleCount
		End Sub

		Public Overrides Sub reset()
			countActualPositive = 0L
			countActualNegative = 0L
			counts.Clear()

			If isExact Then
				probAndLabel = Nothing
			Else
				Dim [step] As Double = 1.0 / thresholdSteps
				For i As Integer = 0 To thresholdSteps
					Dim currThreshold As Double = i * [step]
					counts(currThreshold) = New CountsForThreshold(currThreshold)
				Next i
			End If

			exampleCount = 0
			auc_Conflict = Nothing
			auprc_Conflict = Nothing
		End Sub

		Public Overrides Function stats() As String
			If Me.exampleCount = 0 Then
				Return "ROC: No data available (no data has been performed)"
			End If

			Dim sb As New StringBuilder()
			sb.Append("AUC (Area under ROC Curve):                ").Append(calculateAUC()).Append(vbLf)
			sb.Append("AUPRC (Area under Precision/Recall Curve): ").Append(calculateAUCPR())
			If Not isExact Then
				sb.Append(vbLf)
				sb.Append("[Note: Thresholded AUC/AUPRC calculation used with ").Append(thresholdSteps).Append(" steps); accuracy may reduced compared to exact mode]")
			End If
			Return sb.ToString()
		End Function

		Public Overrides Function ToString() As String
			Return stats()
		End Function

		Public Overridable Function scoreForMetric(ByVal metric As Metric) As Double
			Select Case metric.innerEnumValue
				Case org.nd4j.evaluation.classification.ROC.Metric.InnerEnum.AUROC
					Return calculateAUC()
				Case org.nd4j.evaluation.classification.ROC.Metric.InnerEnum.AUPRC
					Return calculateAUCPR()
				Case Else
					Throw New System.InvalidOperationException("Unknown metric: " & metric)
			End Select
		End Function

		Public Overrides Function getValue(ByVal metric As IMetric) As Double
			If TypeOf metric Is Metric Then
				Return scoreForMetric(DirectCast(metric, Metric))
			Else
				Throw New System.InvalidOperationException("Can't get value for non-ROC Metric " & metric)
			End If
		End Function

		Public Overrides Function newInstance() As ROC
			Return New ROC(thresholdSteps, rocRemoveRedundantPts, exactAllocBlockSize, axis_Conflict)
		End Function
	End Class

End Namespace