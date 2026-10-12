Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports EvaluationAveraging = org.nd4j.evaluation.EvaluationAveraging
Imports EvaluationUtils = org.nd4j.evaluation.EvaluationUtils
Imports org.nd4j.evaluation
Imports IMetric = org.nd4j.evaluation.IMetric
Imports Prediction = org.nd4j.evaluation.meta.Prediction
Imports ConfusionMatrixDeserializer = org.nd4j.evaluation.serde.ConfusionMatrixDeserializer
Imports ConfusionMatrixSerializer = org.nd4j.evaluation.serde.ConfusionMatrixSerializer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports NDArrayTextDeSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer
Imports NDArrayTextSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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
'ORIGINAL LINE: @Slf4j @EqualsAndHashCode(callSuper = true) @Getter @Setter @JsonIgnoreProperties({"confusionMatrixMetaData"}) public class Evaluation extends org.nd4j.evaluation.BaseEvaluation<Evaluation>
	Public Class Evaluation
		Inherits BaseEvaluation(Of Evaluation)

		Public NotInheritable Class Metric Implements IMetric
			Public Shared ReadOnly ACCURACY As New Metric("ACCURACY", InnerEnum.ACCURACY)
			Public Shared ReadOnly F1 As New Metric("F1", InnerEnum.F1)
			Public Shared ReadOnly PRECISION As New Metric("PRECISION", InnerEnum.PRECISION)
			Public Shared ReadOnly RECALL As New Metric("RECALL", InnerEnum.RECALL)
			Public Shared ReadOnly GMEASURE As New Metric("GMEASURE", InnerEnum.GMEASURE)
			Public Shared ReadOnly MCC As New Metric("MCC", InnerEnum.MCC)

			Private Shared ReadOnly valueList As New List(Of Metric)()

			Shared Sub New()
				valueList.Add(ACCURACY)
				valueList.Add(F1)
				valueList.Add(PRECISION)
				valueList.Add(RECALL)
				valueList.Add(GMEASURE)
				valueList.Add(MCC)
			End Sub

			Public Enum InnerEnum
				ACCURACY
				F1
				PRECISION
				RECALL
				GMEASURE
				MCC
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
					Return GetType(Evaluation)
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

		'What to output from the precision/recall function when we encounter an edge case
		Protected Friend Const DEFAULT_EDGE_VALUE As Double = 0.0

		Protected Friend Const CONFUSION_PRINT_MAX_CLASSES As Integer = 20

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode.Exclude protected int axis = 1;
'JAVA TO VB CONVERTER NOTE: The field axis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend axis_Conflict As Integer = 1
		Protected Friend binaryPositiveClass As Integer? = 1 'Used *only* for binary classification; default value here to 1 for legacy JSON loading
		Protected Friend ReadOnly topN As Integer
'JAVA TO VB CONVERTER NOTE: The field topNCorrectCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend topNCorrectCount_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field topNTotalCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend topNTotalCount_Conflict As Integer = 0 'Could use topNCountCorrect / (double)getNumRowCounter() - except for eval(int,int), hence separate counters
'JAVA TO VB CONVERTER NOTE: The field truePositives was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend truePositives_Conflict As New Counter(Of Integer)()
'JAVA TO VB CONVERTER NOTE: The field falsePositives was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend falsePositives_Conflict As New Counter(Of Integer)()
'JAVA TO VB CONVERTER NOTE: The field trueNegatives was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend trueNegatives_Conflict As New Counter(Of Integer)()
'JAVA TO VB CONVERTER NOTE: The field falseNegatives was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend falseNegatives_Conflict As New Counter(Of Integer)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.evaluation.serde.ConfusionMatrixSerializer.class) @JsonDeserialize(using = org.nd4j.evaluation.serde.ConfusionMatrixDeserializer.class) protected ConfusionMatrix<Integer> confusion;
'JAVA TO VB CONVERTER NOTE: The field confusion was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend confusion_Conflict As ConfusionMatrix(Of Integer)
'JAVA TO VB CONVERTER NOTE: The field numRowCounter was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend numRowCounter_Conflict As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected List<String> labelsList = new ArrayList<>();
		Protected Friend labelsList As IList(Of String) = New List(Of String)()

		Protected Friend binaryDecisionThreshold As Double?
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) protected org.nd4j.linalg.api.ndarray.INDArray costArray;
		Protected Friend costArray As INDArray

		Protected Friend confusionMatrixMetaData As IDictionary(Of Pair(Of Integer, Integer), IList(Of Object)) 'Pair: (Actual,Predicted)

		''' <summary>
		''' For stats(): When classes are excluded from precision/recall, what is the maximum number we should print?
		''' If this is set to a high value, the output (potentially thousands of classes) can become unreadable.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int maxWarningClassesToPrint = 16;
		Protected Friend maxWarningClassesToPrint As Integer = 16

		Protected Friend Sub New(ByVal axis As Integer, ByVal binaryPositiveClass As Integer?, ByVal topN As Integer, ByVal labelsList As IList(Of String), ByVal binaryDecisionThreshold As Double?, ByVal costArray As INDArray, ByVal maxWarningClassesToPrint As Integer)
			Me.axis_Conflict = axis
			Me.binaryPositiveClass = binaryPositiveClass
			Me.topN = topN
			Me.labelsList = labelsList
			Me.binaryDecisionThreshold = binaryDecisionThreshold
			Me.costArray = costArray
			Me.maxWarningClassesToPrint = maxWarningClassesToPrint
		End Sub

		' Empty constructor
		Public Sub New()
			Me.topN = 1
			Me.binaryPositiveClass = 1
		End Sub

		''' <summary>
		''' The number of classes to account for in the evaluation </summary>
		''' <param name="numClasses"> the number of classes to account for in the evaluation </param>
		Public Sub New(ByVal numClasses As Integer)
			Me.New(numClasses, (If(numClasses = 2, 1, Nothing)))
		End Sub

		''' <summary>
		''' Constructor for specifying the number of classes, and optionally the positive class for binary classification.
		''' See Evaluation javadoc for more details on evaluation in the binary case
		''' </summary>
		''' <param name="numClasses">          The number of classes for the evaluation. Must be 2, if binaryPositiveClass is non-null </param>
		''' <param name="binaryPositiveClass"> If non-null, the positive class (0 or 1). </param>
		Public Sub New(ByVal numClasses As Integer, ByVal binaryPositiveClass As Integer?)
			Me.New(createLabels(numClasses), 1)
			If binaryPositiveClass IsNot Nothing Then
				Preconditions.checkArgument(binaryPositiveClass = 0 OrElse binaryPositiveClass = 1, "Only 0 and 1 are valid inputs for binaryPositiveClass; got " & binaryPositiveClass)
				Preconditions.checkArgument(numClasses = 2, "Cannot set binaryPositiveClass argument " & "when number of classes is not equal to 2 (got: numClasses=" & numClasses & ")")
			End If
			Me.binaryPositiveClass = binaryPositiveClass
		End Sub


		''' <summary>
		''' The labels to include with the evaluation.
		''' This constructor can be used for
		''' generating labeled output rather than just
		''' numbers for the labels </summary>
		''' <param name="labels"> the labels to use
		'''               for the output </param>
		Public Sub New(ByVal labels As IList(Of String))
			Me.New(labels, 1)
		End Sub

		''' <summary>
		''' Use a map to generate labels
		''' Pass in a label index with the actual label
		''' you want to use for output </summary>
		''' <param name="labels"> a map of label index to label value </param>
		Public Sub New(ByVal labels As IDictionary(Of Integer, String))
			Me.New(createLabelsFromMap(labels), 1)
		End Sub

		''' <summary>
		''' Constructor to use for top N accuracy
		''' </summary>
		''' <param name="labels"> Labels for the classes (may be null) </param>
		''' <param name="topN">   Value to use for top N accuracy calculation (<=1: standard accuracy). Note that with top N
		'''               accuracy, an example is considered 'correct' if the probability for the true class is one of the
		'''               highest N values </param>
		Public Sub New(ByVal labels As IList(Of String), ByVal topN As Integer)
			Me.labelsList = labels
			If labels IsNot Nothing Then
				createConfusion(labels.Count)
			End If

			Me.topN = topN
			If labels IsNot Nothing AndAlso labels.Count = 2 Then
				Me.binaryPositiveClass = 1
			End If
		End Sub

		''' <summary>
		''' Create an evaluation instance with a custom binary decision threshold. Note that binary decision thresholds can
		''' only be used with binary classifiers.<br>
		''' Defaults to class 1 for the positive class - see class javadoc, and use <seealso cref="Evaluation(Double, Integer)"/> to
		''' change this.
		''' </summary>
		''' <param name="binaryDecisionThreshold"> Decision threshold to use for binary predictions </param>
		Public Sub New(ByVal binaryDecisionThreshold As Double)
			Me.New(binaryDecisionThreshold, 1)
		End Sub

		''' <summary>
		''' Create an evaluation instance with a custom binary decision threshold. Note that binary decision thresholds can
		''' only be used with binary classifiers.<br>
		''' This constructor also allows the user to specify the positive class for binary classification. See class javadoc
		''' for more details.
		''' </summary>
		''' <param name="binaryDecisionThreshold"> Decision threshold to use for binary predictions </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Evaluation(double binaryDecisionThreshold, @NonNull Integer binaryPositiveClass)
		Public Sub New(ByVal binaryDecisionThreshold As Double, ByVal binaryPositiveClass As Integer)
			If binaryPositiveClass <> Nothing Then
				Preconditions.checkArgument(binaryPositiveClass = 0 OrElse binaryPositiveClass = 1, "Only 0 and 1 are valid inputs for binaryPositiveClass; got " & binaryPositiveClass)
			End If
			Me.binaryDecisionThreshold = binaryDecisionThreshold
			Me.topN = 1
			Me.binaryPositiveClass = binaryPositiveClass
		End Sub

		''' <summary>
		'''  Created evaluation instance with the specified cost array. A cost array can be used to bias the multi class
		'''  predictions towards or away from certain classes. The predicted class is determined using argMax(cost * probability)
		'''  instead of argMax(probability) when no cost array is present.
		''' </summary>
		''' <param name="costArray"> Row vector cost array. May be null </param>
		Public Sub New(ByVal costArray As INDArray)
			Me.New(Nothing, costArray)
		End Sub

		''' <summary>
		'''  Created evaluation instance with the specified cost array. A cost array can be used to bias the multi class
		'''  predictions towards or away from certain classes. The predicted class is determined using argMax(cost * probability)
		'''  instead of argMax(probability) when no cost array is present.
		''' </summary>
		''' <param name="labels"> Labels for the output classes. May be null </param>
		''' <param name="costArray"> Row vector cost array. May be null </param>
		Public Sub New(ByVal labels As IList(Of String), ByVal costArray As INDArray)
			If costArray IsNot Nothing AndAlso Not costArray.RowVectorOrScalar Then
				Throw New System.ArgumentException("Invalid cost array: must be a row vector (got shape: " & java.util.Arrays.toString(costArray.shape()) & ")")
			End If
			If costArray IsNot Nothing AndAlso costArray.minNumber().doubleValue() < 0.0 Then
				Throw New System.ArgumentException("Invalid cost array: Cost array values must be positive")
			End If
			Me.labelsList = labels
			Me.costArray = If(costArray Is Nothing, Nothing, costArray.castTo(DataType.FLOAT))
			Me.topN = 1
		End Sub

		Protected Friend Overridable Function numClasses() As Integer
			If labelsList IsNot Nothing Then
				Return labelsList.Count
			End If
			Return confusion().getClasses().Count
		End Function

		Public Overrides Sub reset()
			confusion_Conflict = Nothing
			truePositives_Conflict = New Counter(Of Integer)()
			falsePositives_Conflict = New Counter(Of Integer)()
			trueNegatives_Conflict = New Counter(Of Integer)()
			falseNegatives_Conflict = New Counter(Of Integer)()

			topNCorrectCount_Conflict = 0
			topNTotalCount_Conflict = 0
			numRowCounter_Conflict = 0
		End Sub

		Private Function confusion() As ConfusionMatrix(Of Integer)
			Return confusion_Conflict
		End Function

		Private Shared Function createLabels(ByVal numClasses As Integer) As IList(Of String)
			If numClasses = 1 Then
				numClasses = 2 'Binary (single output variable) case...
			End If
			Dim list As IList(Of String) = New List(Of String)(numClasses)
			For i As Integer = 0 To numClasses - 1
				list.Add(i.ToString())
			Next i
			Return list
		End Function

		Private Shared Function createLabelsFromMap(ByVal labels As IDictionary(Of Integer, String)) As IList(Of String)
			Dim size As Integer = labels.Count
			Dim labelsList As IList(Of String) = New List(Of String)(size)
			For i As Integer = 0 To size - 1
				Dim str As String = labels(i)
				If str Is Nothing Then
					Throw New System.ArgumentException("Invalid labels map: missing key for class " & i & " (expect integers 0 to " & (size - 1) & ")")
				End If
				labelsList.Add(str)
			Next i
			Return labelsList
		End Function

		Private Sub createConfusion(ByVal nClasses As Integer)
			Dim classes As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To nClasses - 1
				classes.Add(i)
			Next i

			confusion_Conflict = New ConfusionMatrix(Of Integer)(classes)
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



		''' <summary>
		''' Collects statistics on the real outcomes vs the
		''' guesses. This is for logistic outcome matrices.
		''' <para>
		''' Note that an IllegalArgumentException is thrown if the two passed in
		''' matrices aren't the same length.
		''' 
		''' </para>
		''' </summary>
		''' <param name="realOutcomes"> the real outcomes (labels - usually binary) </param>
		''' <param name="guesses">      the guesses/prediction (usually a probability vector) </param>
		Public Overrides Sub eval(ByVal realOutcomes As INDArray, ByVal guesses As INDArray)
			eval(realOutcomes, guesses, DirectCast(Nothing, IList(Of Serializable)))
		End Sub

		''' <summary>
		''' Evaluate the network, with optional metadata
		''' </summary>
		''' <param name="labels">   Data labels </param>
		''' <param name="predictions">        Network predictions </param>
		''' <param name="recordMetaData"> Optional; may be null. If not null, should have size equal to the number of outcomes/guesses
		'''  </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public void eval(org.nd4j.linalg.api.ndarray.INDArray labels, org.nd4j.linalg.api.ndarray.INDArray predictions, org.nd4j.linalg.api.ndarray.INDArray mask, final List<? extends java.io.Serializable> recordMetaData)
		Public Overrides Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal mask As INDArray, ByVal recordMetaData As IList(Of T1))
			Dim p As Triple(Of INDArray, INDArray, INDArray) = BaseEvaluation.reshapeAndExtractNotMasked(labels, predictions, mask, axis_Conflict)
			If p Is Nothing Then
				'All values masked out; no-op
				Return
			End If

			Dim labels2d As INDArray = p.getFirst()
			Dim predictions2d As INDArray = p.getSecond()
			Dim maskArray As INDArray = p.getThird()
			Preconditions.checkState(maskArray Is Nothing, "Per-output masking for Evaluation is not supported")

			'Check for NaNs in predictions - without this, evaulation could silently be intepreted as class 0 prediction due to argmax
			Dim count As Long = Nd4j.Executioner.execAndReturn(New MatchCondition(predictions2d, Conditions.Nan)).getFinalResult().longValue()
			Preconditions.checkState(count = 0, "Cannot perform evaluation with NaNs present in predictions:" & " %s NaNs present in predictions INDArray", count)

			' Add the number of rows to numRowCounter
			numRowCounter_Conflict += labels2d.size(0)

			If labels2d.dataType() <> predictions2d.dataType() Then
				labels2d = labels2d.castTo(predictions2d.dataType())
			End If

			' If confusion is null, then Evaluation was instantiated without providing the classes -> infer # classes from
			If confusion_Conflict Is Nothing Then
				Dim nClasses As Integer = labels2d.columns()
				If nClasses = 1 Then
					nClasses = 2 'Binary (single output variable) case
				End If
				If labelsList Is Nothing OrElse labelsList.Count = 0 Then
					labelsList = New List(Of String)(nClasses)
					For i As Integer = 0 To nClasses - 1
						labelsList.Add(i.ToString())
					Next i
				End If
				createConfusion(nClasses)
			End If

			' Length of real labels must be same as length of predicted labels
			If Not labels2d.shape().SequenceEqual(predictions2d.shape()) Then
				Throw New System.ArgumentException("Unable to evaluate. Predictions and labels arrays are not same shape." & " Predictions shape: " & java.util.Arrays.toString(predictions2d.shape()) & ", Labels shape: " & java.util.Arrays.toString(labels2d.shape()))
			End If

			' For each row get the most probable label (column) from prediction and assign as guessMax
			' For each row get the column of the true label and assign as currMax

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int nCols = labels2d.columns();
			Dim nCols As Integer = labels2d.columns()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int nRows = labels2d.rows();
			Dim nRows As Integer = labels2d.rows()

			If nCols = 1 Then
				Dim binaryGuesses As INDArray = predictions2d.gt(If(binaryDecisionThreshold Is Nothing, 0.5, binaryDecisionThreshold)).castTo(predictions.dataType())

				Dim notLabel As INDArray = labels2d.rsub(1.0) 'Invert entries (assuming 1 and 0)
				Dim notGuess As INDArray = binaryGuesses.rsub(1.0)
				'tp: predicted = 1, actual = 1
				Dim tp As Integer = labels2d.mul(binaryGuesses).castTo(DataType.INT).sumNumber().intValue()
				'fp: predicted = 1, actual = 0
				Dim fp As Integer = notLabel.mul(binaryGuesses).castTo(DataType.INT).sumNumber().intValue()
				'fn: predicted = 0, actual = 1
				Dim fn As Integer = notGuess.mul(labels2d).castTo(DataType.INT).sumNumber().intValue()
				Dim tn As Integer = nRows - tp - fp - fn

				confusion().add(1, 1, tp)
				confusion().add(1, 0, fn)
				confusion().add(0, 1, fp)
				confusion().add(0, 0, tn)

				truePositives_Conflict.incrementCount(1, tp)
				falsePositives_Conflict.incrementCount(1, fp)
				falseNegatives_Conflict.incrementCount(1, fn)
				trueNegatives_Conflict.incrementCount(1, tn)

				truePositives_Conflict.incrementCount(0, tn)
				falsePositives_Conflict.incrementCount(0, fn)
				falseNegatives_Conflict.incrementCount(0, fp)
				trueNegatives_Conflict.incrementCount(0, tp)

				If recordMetaData IsNot Nothing Then
					Dim i As Integer = 0
					Do While i < binaryGuesses.size(0)
						If i >= recordMetaData.Count Then
							Exit Do
						End If
						Dim actual As Integer = If(labels2d.getDouble(0) = 0.0, 0, 1)
						Dim predicted As Integer = If(binaryGuesses.getDouble(0) = 0.0, 0, 1)
						addToMetaConfusionMatrix(actual, predicted, recordMetaData(i))
						i += 1
					Loop
				End If

			Else
				Dim guessIndex As INDArray
				If binaryDecisionThreshold IsNot Nothing Then
					If nCols <> 2 Then
						Throw New System.InvalidOperationException("Binary decision threshold is set, but number of columns for " & "predictions is " & nCols & ". Binary decision threshold can only be used for binary " & "prediction cases")
					End If

					Dim pClass1 As INDArray = predictions2d.getColumn(1)
					guessIndex = pClass1.gt(binaryDecisionThreshold)
				ElseIf costArray IsNot Nothing Then
					'With a cost array: do argmax(cost * probability) instead of just argmax(probability)
					guessIndex = Nd4j.argMax(predictions2d.mulRowVector(costArray.castTo(predictions2d.dataType())), 1)
				Else
					'Standard case: argmax
					guessIndex = Nd4j.argMax(predictions2d, 1)
				End If
				Dim realOutcomeIndex As INDArray = Nd4j.argMax(labels2d, 1)
				Dim nExamples As val = guessIndex.length()

				For i As Integer = 0 To nExamples - 1
					Dim actual As Integer = CInt(Math.Truncate(realOutcomeIndex.getDouble(i)))
					Dim predicted As Integer = CInt(Math.Truncate(guessIndex.getDouble(i)))
					confusion().add(actual, predicted)

					If recordMetaData IsNot Nothing AndAlso recordMetaData.Count > i Then
						Dim m As Object = recordMetaData(i)
						addToMetaConfusionMatrix(actual, predicted, m)
					End If

					' instead of looping through each label for confusion
					' matrix, instead infer those values by determining if true/false negative/positive,
					' then just add across matrix

					' if actual == predicted, then it's a true positive, assign true negative to every other label
					If actual = predicted Then
						truePositives_Conflict.incrementCount(actual, 1)
						For col As Integer = 0 To nCols - 1
							If col = actual Then
								Continue For
							End If
							trueNegatives_Conflict.incrementCount(col, 1) ' all cols prior
						Next col
					Else
						falsePositives_Conflict.incrementCount(predicted, 1)
						falseNegatives_Conflict.incrementCount(actual, 1)

						' first determine intervals for adding true negatives
						Dim lesserIndex, greaterIndex As Integer
						If actual < predicted Then
							lesserIndex = actual
							greaterIndex = predicted
						Else
							lesserIndex = predicted
							greaterIndex = actual
						End If

						' now loop through intervals
						For col As Integer = 0 To lesserIndex - 1
							trueNegatives_Conflict.incrementCount(col, 1) ' all cols prior
						Next col
						For col As Integer = lesserIndex + 1 To greaterIndex - 1
							trueNegatives_Conflict.incrementCount(col, 1) ' all cols after
						Next col
						For col As Integer = greaterIndex + 1 To nCols - 1
							trueNegatives_Conflict.incrementCount(col, 1) ' all cols after
						Next col
					End If
				Next i
			End If

			If nCols > 1 AndAlso topN > 1 Then
				'Calculate top N accuracy
				'TODO: this could be more efficient
				Dim realOutcomeIndex As INDArray = Nd4j.argMax(labels2d, 1)
				Dim nExamples As val = realOutcomeIndex.length()
				For i As Integer = 0 To nExamples - 1
					Dim labelIdx As Integer = CInt(Math.Truncate(realOutcomeIndex.getDouble(i)))
					Dim prob As Double = predictions2d.getDouble(i, labelIdx)
					Dim row As INDArray = predictions2d.getRow(i)
					Dim countGreaterThan As Integer = CInt(Math.Truncate(Nd4j.Executioner.exec(New MatchCondition(row, Conditions.greaterThan(prob))).getDouble(0)))
					If countGreaterThan < topN Then
						'For example, for top 3 accuracy: can have at most 2 other probabilities larger
						topNCorrectCount_Conflict += 1
					End If
					topNTotalCount_Conflict += 1
				Next i
			End If
		End Sub

		''' <summary>
		''' Evaluate a single prediction (one prediction at a time)
		''' </summary>
		''' <param name="predictedIdx"> Index of class predicted by the network </param>
		''' <param name="actualIdx">    Index of actual class </param>
		Public Overridable Overloads Sub eval(ByVal predictedIdx As Integer, ByVal actualIdx As Integer)
			' Add the number of rows to numRowCounter
			numRowCounter_Conflict += 1

			' If confusion is null, then Evaluation is instantiated without providing the classes
			If confusion_Conflict Is Nothing Then
				Throw New System.NotSupportedException("Cannot evaluate single example without initializing confusion matrix first")
			End If

			addToConfusion(actualIdx, predictedIdx)

			' If they are equal
			If predictedIdx = actualIdx Then
				' Then add 1 to True Positive
				' (For a particular label)
				incrementTruePositives(predictedIdx)

				' And add 1 for each negative class that is accurately predicted (True Negative)
				'(For a particular label)
				For Each clazz As Integer? In confusion().getClasses()
					If clazz <> predictedIdx Then
						trueNegatives_Conflict.incrementCount(clazz, 1.0f)
					End If
				Next clazz
			Else
				' Otherwise the real label is predicted as negative (False Negative)
				incrementFalseNegatives(actualIdx)
				' Otherwise the prediction is predicted as falsely positive (False Positive)
				incrementFalsePositives(predictedIdx)
				' Otherwise true negatives
				For Each clazz As Integer? In confusion().getClasses()
					If clazz <> predictedIdx AndAlso clazz <> actualIdx Then
						trueNegatives_Conflict.incrementCount(clazz, 1.0f)
					End If

				Next clazz
			End If
		End Sub

		''' <summary>
		''' Report the classification statistics as a String </summary>
		''' <returns> Classification statistics as a String </returns>
		Public Overrides Function stats() As String
			Return stats(False)
		End Function

		''' <summary>
		''' Method to obtain the classification report as a String
		''' </summary>
		''' <param name="suppressWarnings"> whether or not to output warnings related to the evaluation results </param>
		''' <returns> A (multi-line) String with accuracy, precision, recall, f1 score etc </returns>
		Public Overridable Overloads Function stats(ByVal suppressWarnings As Boolean) As String
			Return stats(suppressWarnings, numClasses() <= CONFUSION_PRINT_MAX_CLASSES, numClasses() > CONFUSION_PRINT_MAX_CLASSES)
		End Function

		''' <summary>
		''' Method to obtain the classification report as a String
		''' </summary>
		''' <param name="suppressWarnings"> whether or not to output warnings related to the evaluation results </param>
		''' <param name="includeConfusion"> whether the confusion matrix should be included it the returned stats or not </param>
		''' <returns> A (multi-line) String with accuracy, precision, recall, f1 score etc </returns>
		Public Overridable Overloads Function stats(ByVal suppressWarnings As Boolean, ByVal includeConfusion As Boolean) As String
			Return stats(suppressWarnings, includeConfusion, False)
		End Function

		Private Overloads Function stats(ByVal suppressWarnings As Boolean, ByVal includeConfusion As Boolean, ByVal logConfusionSizeWarning As Boolean) As String
			If numRowCounter_Conflict = 0 Then
				Return "Evaluation: No data available (no evaluation has been performed)"
			End If

			Dim builder As StringBuilder = (New StringBuilder()).Append(vbLf)
			Dim warnings As New StringBuilder()
			Dim confusion As ConfusionMatrix(Of Integer) = Me.confusion()
			If confusion Is Nothing Then
				confusion = New ConfusionMatrix(Of Integer)() 'Empty
			End If
			Dim classes As IList(Of Integer) = confusion.getClasses()

			Dim falsePositivesWarningClasses As IList(Of Integer) = New List(Of Integer)()
			Dim falseNegativesWarningClasses As IList(Of Integer) = New List(Of Integer)()
			For Each clazz As Integer? In classes
				'Output possible warnings regarding precision/recall calculation
				If Not suppressWarnings AndAlso truePositives_Conflict.getCount(clazz) = 0 Then
					If falsePositives_Conflict.getCount(clazz) = 0 Then
						falsePositivesWarningClasses.Add(clazz)
					End If
					If falseNegatives_Conflict.getCount(clazz) = 0 Then
						falseNegativesWarningClasses.Add(clazz)
					End If
				End If
			Next clazz

			If falsePositivesWarningClasses.Count > 0 Then
				warningHelper(warnings, falsePositivesWarningClasses, "precision")
			End If
			If falseNegativesWarningClasses.Count > 0 Then
				warningHelper(warnings, falseNegativesWarningClasses, "recall")
			End If

			Dim nClasses As Integer = confusion.getClasses().Count
			Dim df As New DecimalFormat("0.0000")
			Dim acc As Double = accuracy()
			Dim precision As Double = Me.precision() 'Macro precision for N>2, or binary class 1 (only) precision by default
			Dim recall As Double = Me.recall() 'Macro recall for N>2, or binary class 1 (only) precision by default
			Dim f1 As Double = Me.f1() 'Macro F1 for N>2, or binary class 1 (only) precision by default
			builder.Append(vbLf & "========================Evaluation Metrics========================")
			builder.Append(vbLf & " # of classes:    ").Append(nClasses)
			builder.Append(vbLf & " Accuracy:        ").Append(format(df, acc))
			If topN > 1 Then
				Dim topNAcc As Double = topNAccuracy()
				builder.Append(vbLf & " Top ").Append(topN).Append(" Accuracy:  ").Append(format(df, topNAcc))
			End If
			builder.Append(vbLf & " Precision:       ").Append(format(df, precision))
			If nClasses > 2 AndAlso averagePrecisionNumClassesExcluded() > 0 Then
				Dim ex As Integer = averagePrecisionNumClassesExcluded()
				builder.Append(vbTab & "(").Append(ex).Append(" class")
				If ex > 1 Then
					builder.Append("es")
				End If
				builder.Append(" excluded from average)")
			End If
			builder.Append(vbLf & " Recall:          ").Append(format(df, recall))
			If nClasses > 2 AndAlso averageRecallNumClassesExcluded() > 0 Then
				Dim ex As Integer = averageRecallNumClassesExcluded()
				builder.Append(vbTab & "(").Append(ex).Append(" class")
				If ex > 1 Then
					builder.Append("es")
				End If
				builder.Append(" excluded from average)")
			End If
			builder.Append(vbLf & " F1 Score:        ").Append(format(df, f1))
			If nClasses > 2 AndAlso averageF1NumClassesExcluded() > 0 Then
				Dim ex As Integer = averageF1NumClassesExcluded()
				builder.Append(vbTab & "(").Append(ex).Append(" class")
				If ex > 1 Then
					builder.Append("es")
				End If
				builder.Append(" excluded from average)")
			End If
			If nClasses > 2 OrElse binaryPositiveClass Is Nothing Then
				builder.Append(vbLf & "Precision, recall & F1: macro-averaged (equally weighted avg. of ").Append(nClasses).Append(" classes)")
			End If
			If nClasses = 2 AndAlso binaryPositiveClass IsNot Nothing Then
				builder.Append(vbLf & "Precision, recall & F1: reported for positive class (class ").Append(binaryPositiveClass)
				If labelsList IsNot Nothing Then
					builder.Append(" - """).Append(labelsList(binaryPositiveClass)).Append("""")
				End If
				builder.Append(") only")
			End If
			If binaryDecisionThreshold IsNot Nothing Then
				builder.Append(vbLf & "Binary decision threshold: ").Append(binaryDecisionThreshold)
			End If
			If costArray IsNot Nothing Then
				builder.Append(vbLf & "Cost array: ").Append(java.util.Arrays.toString(costArray.dup().data().asFloat()))
			End If
			'Note that we could report micro-averaged too - but these are the same as accuracy
			'"Note that for “micro-averaging in a multiclass setting with all labels included will produce equal precision, recall and F,"
			'http://scikit-learn.org/stable/modules/model_evaluation.html

			builder.Append(vbLf & vbLf)
			builder.Append(warnings)

			If includeConfusion Then
				builder.Append(vbLf & "=========================Confusion Matrix=========================" & vbLf)
				builder.Append(confusionMatrix())
			ElseIf logConfusionSizeWarning Then
				builder.Append(vbLf & vbLf & "Note: Confusion matrix not generated due to space requirements for ").Append(nClasses).Append(" classes." & vbLf).Append("Use stats(false,true) to generate anyway")
			End If

			builder.Append(vbLf & "==================================================================")
			Return builder.ToString()
		End Function

		''' <summary>
		''' Get the confusion matrix as a String </summary>
		''' <returns> Confusion matrix as a String </returns>
		Public Overridable Function confusionMatrix() As String
			Dim nClasses As Integer = numClasses()

			If confusion_Conflict Is Nothing Then
				Return "Confusion matrix: <no data>"
			End If

			'First: work out the maximum count
			Dim classes As IList(Of Integer) = confusion_Conflict.getClasses()
			Dim maxCount As Integer = 1
			For Each i As Integer? In classes
				For Each j As Integer? In classes
					Dim count As Integer = confusion().getCount(i, j)
					maxCount = Math.Max(maxCount, count)
				Next j
			Next i
			maxCount = Math.Max(maxCount, nClasses) 'Include this as header might be bigger than actual values

			Dim numDigits As Integer = CInt(Math.Truncate(Math.Ceiling(Math.Log10(maxCount))))
			If numDigits < 1 Then
				numDigits = 1
			End If
			Dim digitFormat As String = "%" & (numDigits+1) & "d"

			Dim sb As New StringBuilder()
			'Build header:
			For i As Integer = 0 To nClasses - 1
				sb.Append(String.format(digitFormat, i))
			Next i
			sb.Append(vbLf)
			Dim numDividerChars As Integer = (numDigits+1) * nClasses + 1
			For i As Integer = 0 To numDividerChars - 1
				sb.Append("-")
			Next i
			sb.Append(vbLf)

			'Build each row:
			For actual As Integer = 0 To nClasses - 1
				Dim actualName As String = resolveLabelForClass(actual)
				For predicted As Integer = 0 To nClasses - 1
					Dim count As Integer = confusion_Conflict.getCount(actual, predicted)
					sb.Append(String.format(digitFormat, count))
				Next predicted
				sb.Append(" | ").Append(actual).Append(" = ").Append(actualName).Append(vbLf)
			Next actual

			sb.Append(vbLf & "Confusion matrix format: Actual (rowClass) predicted as (columnClass) N times")

			Return sb.ToString()
		End Function

		Private Shared Function format(ByVal f As DecimalFormat, ByVal num As Double) As String
			If Double.IsNaN(num) OrElse Double.IsInfinity(num) Then
				Return num.ToString()
			End If
			Return f.format(num)
		End Function

		Private Function resolveLabelForClass(ByVal clazz As Integer?) As String
			If labelsList IsNot Nothing AndAlso labelsList.Count > clazz Then
				Return labelsList(clazz)
			End If
			Return clazz.ToString()
		End Function

		Private Sub warningHelper(ByVal warnings As StringBuilder, ByVal list As IList(Of Integer), ByVal metric As String)
			warnings.Append("Warning: ").Append(list.Count).Append(" class")
			Dim wasWere As String
			If list.Count = 1 Then
				wasWere = "was"
			Else
				wasWere = "were"
				warnings.Append("es")
			End If
			warnings.Append(" ").Append(wasWere)
			warnings.Append(" never predicted by the model and ").Append(wasWere).Append(" excluded from average ").Append(metric)
			If list.Count <= maxWarningClassesToPrint Then
				warnings.Append(vbLf & "Classes excluded from average ").Append(metric).Append(": ").Append(list).Append(vbLf)
			End If
		End Sub

		''' <summary>
		''' Returns the precision for a given class label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <returns> the precision for the label </returns>
		Public Overridable Function precision(ByVal classLabel As Integer?) As Double
			Return precision(classLabel, DEFAULT_EDGE_VALUE)
		End Function

		''' <summary>
		''' Returns the precision for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <param name="edgeCase">   What to output in case of 0/0 </param>
		''' <returns> the precision for the label </returns>
		Public Overridable Function precision(ByVal classLabel As Integer?, ByVal edgeCase As Double) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get precision: no evaluation has been performed")
			Dim tpCount As Double = truePositives_Conflict.getCount(classLabel)
			Dim fpCount As Double = falsePositives_Conflict.getCount(classLabel)
			Return EvaluationUtils.precision(CLng(Math.Truncate(tpCount)), CLng(Math.Truncate(fpCount)), edgeCase)
		End Function

		''' <summary>
		''' Precision based on guesses so far.<br>
		''' Note: value returned will differ depending on number of classes and settings.<br>
		''' 1. For binary classification, if the positive class is set (via default value of 1, via constructor,
		'''    or via <seealso cref="setBinaryPositiveClass(Integer)"/>), the returned value will be for the specified positive class
		'''    only.<br>
		''' 2. For the multi-class case, or when <seealso cref="getBinaryPositiveClass()"/> is null, the returned value is macro-averaged
		'''    across all classes. i.e., is macro-averaged precision, equivalent to {@code precision(EvaluationAveraging.Macro)}<br>
		''' </summary>
		''' <returns> the total precision based on guesses so far </returns>
		Public Overridable Function precision() As Double
			If binaryPositiveClass IsNot Nothing AndAlso numClasses() = 2 Then
				Return precision(binaryPositiveClass)
			End If
			Return precision(EvaluationAveraging.Macro)
		End Function

		''' <summary>
		''' Calculate the average precision for all classes. Can specify whether macro or micro averaging should be used
		''' NOTE: if any classes have tp=0 and fp=0, (precision=0/0) these are excluded from the average
		''' </summary>
		''' <param name="averaging"> Averaging method - macro or micro </param>
		''' <returns> Average precision </returns>
		Public Overridable Function precision(ByVal averaging As EvaluationAveraging) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get precision: no evaluation has been performed")
			Dim nClasses As Integer = confusion().getClasses().Count
			If averaging = EvaluationAveraging.Macro Then
				Dim macroPrecision As Double = 0.0
				Dim count As Integer = 0
				For i As Integer = 0 To nClasses - 1
					Dim thisClassPrec As Double = precision(i, -1)
					If thisClassPrec <> -1 Then
						macroPrecision += thisClassPrec
						count += 1
					End If
				Next i
				macroPrecision /= count
				Return macroPrecision
			ElseIf averaging = EvaluationAveraging.Micro Then
				Dim tpCount As Long = 0
				Dim fpCount As Long = 0
				For i As Integer = 0 To nClasses - 1
					tpCount += truePositives_Conflict.getCount(i)
					fpCount += falsePositives_Conflict.getCount(i)
				Next i
				Return EvaluationUtils.precision(tpCount, fpCount, DEFAULT_EDGE_VALUE)
			Else
				Throw New System.NotSupportedException("Unknown averaging approach: " & averaging)
			End If
		End Function

		''' <summary>
		''' When calculating the (macro) average precision, how many classes are excluded from the average due to
		''' no predictions - i.e., precision would be the edge case of 0/0
		''' </summary>
		''' <returns> Number of classes excluded from the  average precision </returns>
		Public Overridable Function averagePrecisionNumClassesExcluded() As Integer
			Return numClassesExcluded("precision")
		End Function

		''' <summary>
		''' When calculating the (macro) average Recall, how many classes are excluded from the average due to
		''' no predictions - i.e., recall would be the edge case of 0/0
		''' </summary>
		''' <returns> Number of classes excluded from the average recall </returns>
		Public Overridable Function averageRecallNumClassesExcluded() As Integer
			Return numClassesExcluded("recall")
		End Function

		''' <summary>
		''' When calculating the (macro) average F1, how many classes are excluded from the average due to
		''' no predictions - i.e., F1 would be calculated from a precision or recall of 0/0
		''' </summary>
		''' <returns> Number of classes excluded from the average F1 </returns>
		Public Overridable Function averageF1NumClassesExcluded() As Integer
			Return numClassesExcluded("f1")
		End Function

		''' <summary>
		''' When calculating the (macro) average FBeta, how many classes are excluded from the average due to
		''' no predictions - i.e., FBeta would be calculated from a precision or recall of 0/0
		''' </summary>
		''' <returns> Number of classes excluded from the average FBeta </returns>
		Public Overridable Function averageFBetaNumClassesExcluded() As Integer
			Return numClassesExcluded("fbeta")
		End Function

		Private Function numClassesExcluded(ByVal metric As String) As Integer
			Dim countExcluded As Integer = 0
			Dim nClasses As Integer = confusion().getClasses().Count

			For i As Integer = 0 To nClasses - 1
				Dim d As Double
				Select Case metric.ToLower()
					Case "precision"
						d = precision(i, -1)
					Case "recall"
						d = recall(i, -1)
					Case "f1", "fbeta"
						d = fBeta(1.0, i, -1)
					Case Else
						Throw New Exception("Unknown metric: " & metric)
				End Select

				If d = -1 Then
					countExcluded += 1
				End If
			Next i
			Return countExcluded
		End Function

		''' <summary>
		''' Returns the recall for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <returns> Recall rate as a double </returns>
		Public Overridable Function recall(ByVal classLabel As Integer) As Double
			Return recall(classLabel, DEFAULT_EDGE_VALUE)
		End Function

		''' <summary>
		''' Returns the recall for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <param name="edgeCase">   What to output in case of 0/0 </param>
		''' <returns> Recall rate as a double </returns>
		Public Overridable Function recall(ByVal classLabel As Integer, ByVal edgeCase As Double) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get recall: no evaluation has been performed")
			Dim tpCount As Double = truePositives_Conflict.getCount(classLabel)
			Dim fnCount As Double = falseNegatives_Conflict.getCount(classLabel)

			Return EvaluationUtils.recall(CLng(Math.Truncate(tpCount)), CLng(Math.Truncate(fnCount)), edgeCase)
		End Function

		''' <summary>
		''' Recall based on guesses so far<br>
		''' Note: value returned will differ depending on number of classes and settings.<br>
		''' 1. For binary classification, if the positive class is set (via default value of 1, via constructor,
		'''    or via <seealso cref="setBinaryPositiveClass(Integer)"/>), the returned value will be for the specified positive class
		'''    only.<br>
		''' 2. For the multi-class case, or when <seealso cref="getBinaryPositiveClass()"/> is null, the returned value is macro-averaged
		'''    across all classes. i.e., is macro-averaged recall, equivalent to {@code recall(EvaluationAveraging.Macro)}<br>
		''' </summary>
		''' <returns> the recall for the outcomes </returns>
		Public Overridable Function recall() As Double
			If binaryPositiveClass IsNot Nothing AndAlso numClasses() = 2 Then
				Return recall(binaryPositiveClass)
			End If
			Return recall(EvaluationAveraging.Macro)
		End Function

		''' <summary>
		''' Calculate the average recall for all classes - can specify whether macro or micro averaging should be used
		''' NOTE: if any classes have tp=0 and fn=0, (recall=0/0) these are excluded from the average
		''' </summary>
		''' <param name="averaging"> Averaging method - macro or micro </param>
		''' <returns> Average recall </returns>
		Public Overridable Function recall(ByVal averaging As EvaluationAveraging) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get recall: no evaluation has been performed")
			Dim nClasses As Integer = confusion().getClasses().Count
			If averaging = EvaluationAveraging.Macro Then
				Dim macroRecall As Double = 0.0
				Dim count As Integer = 0
				For i As Integer = 0 To nClasses - 1
					Dim thisClassRecall As Double = recall(i, -1)
					If thisClassRecall <> -1 Then
						macroRecall += thisClassRecall
						count += 1
					End If
				Next i
				macroRecall /= count
				Return macroRecall
			ElseIf averaging = EvaluationAveraging.Micro Then
				Dim tpCount As Long = 0
				Dim fnCount As Long = 0
				For i As Integer = 0 To nClasses - 1
					tpCount += truePositives_Conflict.getCount(i)
					fnCount += falseNegatives_Conflict.getCount(i)
				Next i
				Return EvaluationUtils.recall(tpCount, fnCount, DEFAULT_EDGE_VALUE)
			Else
				Throw New System.NotSupportedException("Unknown averaging approach: " & averaging)
			End If
		End Function


		''' <summary>
		''' Returns the false positive rate for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <returns> fpr as a double </returns>
		Public Overridable Function falsePositiveRate(ByVal classLabel As Integer) As Double
			Return falsePositiveRate(classLabel, DEFAULT_EDGE_VALUE)
		End Function

		''' <summary>
		''' Returns the false positive rate for a given label
		''' </summary>
		''' <param name="classLabel"> the label </param>
		''' <param name="edgeCase">   What to output in case of 0/0 </param>
		''' <returns> fpr as a double </returns>
		Public Overridable Function falsePositiveRate(ByVal classLabel As Integer, ByVal edgeCase As Double) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get false positive rate: no evaluation has been performed")
			Dim fpCount As Double = falsePositives_Conflict.getCount(classLabel)
			Dim tnCount As Double = trueNegatives_Conflict.getCount(classLabel)

			Return EvaluationUtils.falsePositiveRate(CLng(Math.Truncate(fpCount)), CLng(Math.Truncate(tnCount)), edgeCase)
		End Function

		''' <summary>
		''' False positive rate based on guesses so far<br>
		''' Note: value returned will differ depending on number of classes and settings.<br>
		''' 1. For binary classification, if the positive class is set (via default value of 1, via constructor,
		'''    or via <seealso cref="setBinaryPositiveClass(Integer)"/>), the returned value will be for the specified positive class
		'''    only.<br>
		''' 2. For the multi-class case, or when <seealso cref="getBinaryPositiveClass()"/> is null, the returned value is macro-averaged
		'''    across all classes. i.e., is macro-averaged false positive rate, equivalent to
		'''    {@code falsePositiveRate(EvaluationAveraging.Macro)}<br>
		''' </summary>
		''' <returns> the fpr for the outcomes </returns>
		Public Overridable Function falsePositiveRate() As Double
			If binaryPositiveClass IsNot Nothing AndAlso numClasses() = 2 Then
				Return falsePositiveRate(binaryPositiveClass)
			End If
			Return falsePositiveRate(EvaluationAveraging.Macro)
		End Function

		''' <summary>
		''' Calculate the average false positive rate across all classes. Can specify whether macro or micro averaging should be used
		''' </summary>
		''' <param name="averaging"> Averaging method - macro or micro </param>
		''' <returns> Average false positive rate </returns>
		Public Overridable Function falsePositiveRate(ByVal averaging As EvaluationAveraging) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get false positive rate: no evaluation has been performed")
			Dim nClasses As Integer = confusion().getClasses().Count
			If averaging = EvaluationAveraging.Macro Then
				Dim macroFPR As Double = 0.0
				For i As Integer = 0 To nClasses - 1
					macroFPR += falsePositiveRate(i)
				Next i
				macroFPR /= nClasses
				Return macroFPR
			ElseIf averaging = EvaluationAveraging.Micro Then
				Dim fpCount As Long = 0
				Dim tnCount As Long = 0
				For i As Integer = 0 To nClasses - 1
					fpCount += falsePositives_Conflict.getCount(i)
					tnCount += trueNegatives_Conflict.getCount(i)
				Next i
				Return EvaluationUtils.falsePositiveRate(fpCount, tnCount, DEFAULT_EDGE_VALUE)
			Else
				Throw New System.NotSupportedException("Unknown averaging approach: " & averaging)
			End If
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
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get false negative rate: no evaluation has been performed")
			Dim fnCount As Double = falseNegatives_Conflict.getCount(classLabel)
			Dim tpCount As Double = truePositives_Conflict.getCount(classLabel)

			Return EvaluationUtils.falseNegativeRate(CLng(Math.Truncate(fnCount)), CLng(Math.Truncate(tpCount)), edgeCase)
		End Function

		''' <summary>
		''' False negative rate based on guesses so far
		''' Note: value returned will differ depending on number of classes and settings.<br>
		''' 1. For binary classification, if the positive class is set (via default value of 1, via constructor,
		'''    or via <seealso cref="setBinaryPositiveClass(Integer)"/>), the returned value will be for the specified positive class
		'''    only.<br>
		''' 2. For the multi-class case, or when <seealso cref="getBinaryPositiveClass()"/> is null, the returned value is macro-averaged
		'''    across all classes. i.e., is macro-averaged false negative rate, equivalent to
		'''    {@code falseNegativeRate(EvaluationAveraging.Macro)}<br>
		''' </summary>
		''' <returns> the fnr for the outcomes </returns>
		Public Overridable Function falseNegativeRate() As Double
			If binaryPositiveClass IsNot Nothing AndAlso numClasses() = 2 Then
				Return falseNegativeRate(binaryPositiveClass)
			End If
			Return falseNegativeRate(EvaluationAveraging.Macro)
		End Function

		''' <summary>
		''' Calculate the average false negative rate for all classes - can specify whether macro or micro averaging should be used
		''' </summary>
		''' <param name="averaging"> Averaging method - macro or micro </param>
		''' <returns> Average false negative rate </returns>
		Public Overridable Function falseNegativeRate(ByVal averaging As EvaluationAveraging) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get false negative rate: no evaluation has been performed")
			Dim nClasses As Integer = confusion().getClasses().Count
			If averaging = EvaluationAveraging.Macro Then
				Dim macroFNR As Double = 0.0
				For i As Integer = 0 To nClasses - 1
					macroFNR += falseNegativeRate(i)
				Next i
				macroFNR /= nClasses
				Return macroFNR
			ElseIf averaging = EvaluationAveraging.Micro Then
				Dim fnCount As Long = 0
				Dim tnCount As Long = 0
				For i As Integer = 0 To nClasses - 1
					fnCount += falseNegatives_Conflict.getCount(i)
					tnCount += trueNegatives_Conflict.getCount(i)
				Next i
				Return EvaluationUtils.falseNegativeRate(fnCount, tnCount, DEFAULT_EDGE_VALUE)
			Else
				Throw New System.NotSupportedException("Unknown averaging approach: " & averaging)
			End If
		End Function

		''' <summary>
		''' False Alarm Rate (FAR) reflects rate of misclassified to classified records
		''' <a href="http://ro.ecu.edu.au/cgi/viewcontent.cgi?article=1058&context=isw">http://ro.ecu.edu.au/cgi/viewcontent.cgi?article=1058&context=isw</a><br>
		''' Note: value returned will differ depending on number of classes and settings.<br>
		''' 1. For binary classification, if the positive class is set (via default value of 1, via constructor,
		'''    or via <seealso cref="setBinaryPositiveClass(Integer)"/>), the returned value will be for the specified positive class
		'''    only.<br>
		''' 2. For the multi-class case, or when <seealso cref="getBinaryPositiveClass()"/> is null, the returned value is macro-averaged
		'''    across all classes. i.e., is macro-averaged false alarm rate)
		''' </summary>
		''' <returns> the fpr for the outcomes </returns>
		Public Overridable Function falseAlarmRate() As Double
			If binaryPositiveClass IsNot Nothing AndAlso numClasses() = 2 Then
				Return (falsePositiveRate(binaryPositiveClass) + falseNegativeRate(binaryPositiveClass)) / 2.0
			End If
			Return (falsePositiveRate() + falseNegativeRate()) / 2.0
		End Function

		''' <summary>
		''' Calculate f1 score for a given class
		''' </summary>
		''' <param name="classLabel"> the label to calculate f1 for </param>
		''' <returns> the f1 score for the given label </returns>
		Public Overridable Function f1(ByVal classLabel As Integer) As Double
			Return fBeta(1.0, classLabel)
		End Function

		''' <summary>
		''' Calculate the f_beta for a given class, where f_beta is defined as:<br>
		''' (1+beta^2) * (precision * recall) / (beta^2 * precision + recall).<br>
		''' F1 is a special case of f_beta, with beta=1.0
		''' </summary>
		''' <param name="beta">       Beta value to use </param>
		''' <param name="classLabel"> Class label </param>
		''' <returns> F_beta </returns>
		Public Overridable Function fBeta(ByVal beta As Double, ByVal classLabel As Integer) As Double
			Return fBeta(beta, classLabel, 0.0)
		End Function

		''' <summary>
		''' Calculate the f_beta for a given class, where f_beta is defined as:<br>
		''' (1+beta^2) * (precision * recall) / (beta^2 * precision + recall).<br>
		''' F1 is a special case of f_beta, with beta=1.0
		''' </summary>
		''' <param name="beta">       Beta value to use </param>
		''' <param name="classLabel"> Class label </param>
		''' <param name="defaultValue"> Default value to use when precision or recall is undefined (0/0 for prec. or recall) </param>
		''' <returns> F_beta </returns>
		Public Overridable Function fBeta(ByVal beta As Double, ByVal classLabel As Integer, ByVal defaultValue As Double) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get fBeta score: no evaluation has been performed")
			Dim precision As Double = Me.precision(classLabel, -1)
			Dim recall As Double = Me.recall(classLabel, -1)
			If precision = -1 OrElse recall = -1 Then
				Return defaultValue
			End If
			Return EvaluationUtils.fBeta(beta, precision, recall)
		End Function

		''' <summary>
		''' Calculate the F1 score<br>
		''' F1 score is defined as:<br>
		''' TP: true positive<br>
		''' FP: False Positive<br>
		''' FN: False Negative<br>
		''' F1 score: 2 * TP / (2TP + FP + FN)<br>
		''' <br>
		''' Note: value returned will differ depending on number of classes and settings.<br>
		''' 1. For binary classification, if the positive class is set (via default value of 1, via constructor,
		'''    or via <seealso cref="setBinaryPositiveClass(Integer)"/>), the returned value will be for the specified positive class
		'''    only.<br>
		''' 2. For the multi-class case, or when <seealso cref="getBinaryPositiveClass()"/> is null, the returned value is macro-averaged
		'''    across all classes. i.e., is macro-averaged f1, equivalent to {@code f1(EvaluationAveraging.Macro)}<br>
		''' </summary>
		''' <returns> the f1 score or harmonic mean of precision and recall based on current guesses </returns>
		Public Overridable Function f1() As Double
			If binaryPositiveClass IsNot Nothing AndAlso numClasses() = 2 Then
				Return f1(binaryPositiveClass)
			End If
			Return f1(EvaluationAveraging.Macro)
		End Function

		''' <summary>
		''' Calculate the average F1 score across all classes, using macro or micro averaging
		''' </summary>
		''' <param name="averaging"> Averaging method to use </param>
		Public Overridable Function f1(ByVal averaging As EvaluationAveraging) As Double
			Return fBeta(1.0, averaging)
		End Function

		''' <summary>
		''' Calculate the average F_beta score across all classes, using macro or micro averaging
		''' </summary>
		''' <param name="beta"> Beta value to use </param>
		''' <param name="averaging"> Averaging method to use </param>
		Public Overridable Function fBeta(ByVal beta As Double, ByVal averaging As EvaluationAveraging) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get fBeta score: no evaluation has been performed")
			Dim nClasses As Integer = confusion().getClasses().Count

			If nClasses = 2 Then
				Return EvaluationUtils.fBeta(beta, CLng(Math.Truncate(truePositives_Conflict.getCount(1))), CLng(Math.Truncate(falsePositives_Conflict.getCount(1))), CLng(Math.Truncate(falseNegatives_Conflict.getCount(1))))
			End If

			If averaging = EvaluationAveraging.Macro Then
				Dim macroFBeta As Double = 0.0
				Dim count As Integer = 0
				For i As Integer = 0 To nClasses - 1
					Dim thisFBeta As Double = fBeta(beta, i, -1)
					If thisFBeta <> -1 Then
						macroFBeta += thisFBeta
						count += 1
					End If
				Next i
				macroFBeta /= count
				Return macroFBeta
			ElseIf averaging = EvaluationAveraging.Micro Then
				Dim tpCount As Long = 0
				Dim fpCount As Long = 0
				Dim fnCount As Long = 0
				For i As Integer = 0 To nClasses - 1
					tpCount += truePositives_Conflict.getCount(i)
					fpCount += falsePositives_Conflict.getCount(i)
					fnCount += falseNegatives_Conflict.getCount(i)
				Next i
				Return EvaluationUtils.fBeta(beta, tpCount, fpCount, fnCount)
			Else
				Throw New System.NotSupportedException("Unknown averaging approach: " & averaging)
			End If
		End Function

		''' <summary>
		''' Calculate the G-measure for the given output
		''' </summary>
		''' <param name="output"> The specified output </param>
		''' <returns> The G-measure for the specified output </returns>
		Public Overridable Function gMeasure(ByVal output As Integer) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get gMeasure: no evaluation has been performed")
			Dim precision As Double = Me.precision(output)
			Dim recall As Double = Me.recall(output)
			Return EvaluationUtils.gMeasure(precision, recall)
		End Function

		''' <summary>
		''' Calculates the average G measure for all outputs using micro or macro averaging
		''' </summary>
		''' <param name="averaging"> Averaging method to use </param>
		''' <returns> Average G measure </returns>
		Public Overridable Function gMeasure(ByVal averaging As EvaluationAveraging) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get gMeasure: no evaluation has been performed")
			Dim nClasses As Integer = confusion().getClasses().Count
			If averaging = EvaluationAveraging.Macro Then
				Dim macroGMeasure As Double = 0.0
				For i As Integer = 0 To nClasses - 1
					macroGMeasure += gMeasure(i)
				Next i
				macroGMeasure /= nClasses
				Return macroGMeasure
			ElseIf averaging = EvaluationAveraging.Micro Then
				Dim tpCount As Long = 0
				Dim fpCount As Long = 0
				Dim fnCount As Long = 0
				For i As Integer = 0 To nClasses - 1
					tpCount += truePositives_Conflict.getCount(i)
					fpCount += falsePositives_Conflict.getCount(i)
					fnCount += falseNegatives_Conflict.getCount(i)
				Next i
				Dim precision As Double = EvaluationUtils.precision(tpCount, fpCount, DEFAULT_EDGE_VALUE)
				Dim recall As Double = EvaluationUtils.recall(tpCount, fnCount, DEFAULT_EDGE_VALUE)
				Return EvaluationUtils.gMeasure(precision, recall)
			Else
				Throw New System.NotSupportedException("Unknown averaging approach: " & averaging)
			End If
		End Function

		''' <summary>
		''' Accuracy:
		''' (TP + TN) / (P + N)
		''' </summary>
		''' <returns> the accuracy of the guesses so far </returns>
		Public Overridable Function accuracy() As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get accuracy: no evaluation has been performed")
			'Accuracy: sum the counts on the diagonal of the confusion matrix, divide by total
			Dim nClasses As Integer = confusion().getClasses().Count
			Dim countCorrect As Integer = 0
			For i As Integer = 0 To nClasses - 1
				countCorrect += confusion().getCount(i, i)
			Next i

			Return countCorrect / CDbl(NumRowCounter)
		End Function

		''' <summary>
		''' Top N accuracy of the predictions so far. For top N = 1 (default), equivalent to <seealso cref="accuracy()"/> </summary>
		''' <returns> Top N accuracy </returns>
		Public Overridable Function topNAccuracy() As Double
			If topN <= 1 Then
				Return accuracy()
			End If
			If topNTotalCount_Conflict = 0 Then
				Return 0.0
			End If
			Return topNCorrectCount_Conflict / CDbl(topNTotalCount_Conflict)
		End Function

		''' <summary>
		''' Calculate the binary Mathews correlation coefficient, for the specified class.<br>
		''' MCC = (TP*TN - FP*FN) / sqrt((TP+FP)(TP+FN)(TN+FP)(TN+FN))<br>
		''' </summary>
		''' <param name="classIdx"> Class index to calculate Matthews correlation coefficient for </param>
		Public Overridable Function matthewsCorrelation(ByVal classIdx As Integer) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get Matthews correlation: no evaluation has been performed")
			Return EvaluationUtils.matthewsCorrelation(CLng(Math.Truncate(truePositives_Conflict.getCount(classIdx))), CLng(Math.Truncate(falsePositives_Conflict.getCount(classIdx))), CLng(Math.Truncate(falseNegatives_Conflict.getCount(classIdx))), CLng(Math.Truncate(trueNegatives_Conflict.getCount(classIdx))))
		End Function

		''' <summary>
		''' Calculate the average binary Mathews correlation coefficient, using macro or micro averaging.<br>
		''' MCC = (TP*TN - FP*FN) / sqrt((TP+FP)(TP+FN)(TN+FP)(TN+FN))<br>
		''' Note: This is NOT the same as the multi-class Matthews correlation coefficient
		''' </summary>
		''' <param name="averaging"> Averaging approach </param>
		''' <returns> Average </returns>
		Public Overridable Function matthewsCorrelation(ByVal averaging As EvaluationAveraging) As Double
			Preconditions.checkState(numRowCounter_Conflict > 0, "Cannot get Matthews correlation: no evaluation has been performed")
			Dim nClasses As Integer = confusion().getClasses().Count
			If averaging = EvaluationAveraging.Macro Then
				Dim macroMatthewsCorrelation As Double = 0.0
				For i As Integer = 0 To nClasses - 1
					macroMatthewsCorrelation += matthewsCorrelation(i)
				Next i
				macroMatthewsCorrelation /= nClasses
				Return macroMatthewsCorrelation
			ElseIf averaging = EvaluationAveraging.Micro Then
				Dim tpCount As Long = 0
				Dim fpCount As Long = 0
				Dim fnCount As Long = 0
				Dim tnCount As Long = 0
				For i As Integer = 0 To nClasses - 1
					tpCount += truePositives_Conflict.getCount(i)
					fpCount += falsePositives_Conflict.getCount(i)
					fnCount += falseNegatives_Conflict.getCount(i)
					tnCount += trueNegatives_Conflict.getCount(i)
				Next i
				Return EvaluationUtils.matthewsCorrelation(tpCount, fpCount, fnCount, tnCount)
			Else
				Throw New System.NotSupportedException("Unknown averaging approach: " & averaging)
			End If
		End Function

		''' <summary>
		''' True positives: correctly rejected
		''' </summary>
		''' <returns> the total true positives so far </returns>
		Public Overridable Function truePositives() As IDictionary(Of Integer, Integer)
			Return convertToMap(truePositives_Conflict, confusion().getClasses().Count)
		End Function

		''' <summary>
		''' True negatives: correctly rejected
		''' </summary>
		''' <returns> the total true negatives so far </returns>
		Public Overridable Function trueNegatives() As IDictionary(Of Integer, Integer)
			Return convertToMap(trueNegatives_Conflict, confusion().getClasses().Count)
		End Function

		''' <summary>
		''' False positive: wrong guess
		''' </summary>
		''' <returns> the count of the false positives </returns>
		Public Overridable Function falsePositives() As IDictionary(Of Integer, Integer)
			Return convertToMap(falsePositives_Conflict, confusion().getClasses().Count)
		End Function

		''' <summary>
		''' False negatives: correctly rejected
		''' </summary>
		''' <returns> the total false negatives so far </returns>
		Public Overridable Function falseNegatives() As IDictionary(Of Integer, Integer)
			Return convertToMap(falseNegatives_Conflict, confusion().getClasses().Count)
		End Function

		''' <summary>
		''' Total negatives true negatives + false negatives
		''' </summary>
		''' <returns> the overall negative count </returns>
		Public Overridable Function negative() As IDictionary(Of Integer, Integer)
			Return addMapsByKey(trueNegatives(), falsePositives())
		End Function

		''' <summary>
		''' Returns all of the positive guesses:
		''' true positive + false negative
		''' </summary>
		Public Overridable Function positive() As IDictionary(Of Integer, Integer)
			Return addMapsByKey(truePositives(), falseNegatives())
		End Function

		Private Function convertToMap(ByVal counter As Counter(Of Integer), ByVal maxCount As Integer) As IDictionary(Of Integer, Integer)
			Dim map As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			For i As Integer = 0 To maxCount - 1
				map(i) = CInt(Math.Truncate(counter.getCount(i)))
			Next i
			Return map
		End Function

		Private Function addMapsByKey(ByVal first As IDictionary(Of Integer, Integer), ByVal second As IDictionary(Of Integer, Integer)) As IDictionary(Of Integer, Integer)
			Dim [out] As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim keys As ISet(Of Integer) = New HashSet(Of Integer)(first.Keys)
			keys.addAll(second.Keys)

			For Each i As Integer? In keys
				Dim f As Integer? = first(i)
				Dim s As Integer? = second(i)
				If f Is Nothing Then
					f = 0
				End If
				If s Is Nothing Then
					s = 0
				End If

				[out](i) = f.Value + s.Value
			Next i

			Return [out]
		End Function


		' Incrementing counters
		Public Overridable Sub incrementTruePositives(ByVal classLabel As Integer?)
			truePositives_Conflict.incrementCount(classLabel, 1.0f)
		End Sub

		Public Overridable Sub incrementTrueNegatives(ByVal classLabel As Integer?)
			trueNegatives_Conflict.incrementCount(classLabel, 1.0f)
		End Sub

		Public Overridable Sub incrementFalseNegatives(ByVal classLabel As Integer?)
			falseNegatives_Conflict.incrementCount(classLabel, 1.0f)
		End Sub

		Public Overridable Sub incrementFalsePositives(ByVal classLabel As Integer?)
			falsePositives_Conflict.incrementCount(classLabel, 1.0f)
		End Sub


		' Other misc methods

		''' <summary>
		''' Adds to the confusion matrix
		''' </summary>
		''' <param name="real">  the actual guess </param>
		''' <param name="guess"> the system guess </param>
		Public Overridable Sub addToConfusion(ByVal real As Integer?, ByVal guess As Integer?)
			confusion().add(real, guess)
		End Sub

		''' <summary>
		''' Returns the number of times the given label
		''' has actually occurred
		''' </summary>
		''' <param name="clazz"> the label </param>
		''' <returns> the number of times the label
		''' actually occurred </returns>
		Public Overridable Function classCount(ByVal clazz As Integer?) As Integer
			Return confusion().getActualTotal(clazz)
		End Function

		Public Overridable ReadOnly Property NumRowCounter As Integer
			Get
				Return numRowCounter_Conflict
			End Get
		End Property

		''' <summary>
		''' Return the number of correct predictions according to top N value. For top N = 1 (default) this is equivalent to
		''' the number of correct predictions </summary>
		''' <returns> Number of correct top N predictions </returns>
		Public Overridable ReadOnly Property TopNCorrectCount As Integer
			Get
				If confusion_Conflict Is Nothing Then
					Return 0
				End If
				If topN <= 1 Then
					Dim nClasses As Integer = confusion().getClasses().Count
					Dim countCorrect As Integer = 0
					For i As Integer = 0 To nClasses - 1
						countCorrect += confusion().getCount(i, i)
					Next i
					Return countCorrect
				End If
				Return topNCorrectCount_Conflict
			End Get
		End Property

		''' <summary>
		''' Return the total number of top N evaluations. Most of the time, this is exactly equal to <seealso cref="getNumRowCounter()"/>,
		''' but may differ in the case of using <seealso cref="eval(Integer, Integer)"/> as top N accuracy cannot be calculated in that case
		''' (i.e., requires the full probability distribution, not just predicted/actual indices) </summary>
		''' <returns> Total number of top N predictions </returns>
		Public Overridable ReadOnly Property TopNTotalCount As Integer
			Get
				If topN <= 1 Then
					Return NumRowCounter
				End If
				Return topNTotalCount_Conflict
			End Get
		End Property

		Public Overridable Function getClassLabel(ByVal clazz As Integer?) As String
			Return resolveLabelForClass(clazz)
		End Function

		''' <summary>
		''' Returns the confusion matrix variable
		''' </summary>
		''' <returns> confusion matrix variable for this evaluation </returns>
		Public Overridable ReadOnly Property ConfusionMatrix As ConfusionMatrix(Of Integer)
			Get
				Return confusion_Conflict
			End Get
		End Property

		''' <summary>
		''' Merge the other evaluation object into this one. The result is that this Evaluation instance contains the counts
		''' etc from both
		''' </summary>
		''' <param name="other"> Evaluation object to merge into this one. </param>
		Public Overrides Sub merge(ByVal other As Evaluation)
			If other Is Nothing Then
				Return
			End If

			truePositives_Conflict.incrementAll(other.truePositives_Conflict)
			falsePositives_Conflict.incrementAll(other.falsePositives_Conflict)
			trueNegatives_Conflict.incrementAll(other.trueNegatives_Conflict)
			falseNegatives_Conflict.incrementAll(other.falseNegatives_Conflict)

			If confusion_Conflict Is Nothing Then
				If other.confusion_Conflict IsNot Nothing Then
					confusion_Conflict = New ConfusionMatrix(Of Integer)(other.confusion_Conflict)
				End If
			Else
				If other.confusion_Conflict IsNot Nothing Then
					confusion().add(other.confusion_Conflict)
				End If
			End If
			numRowCounter_Conflict += other.numRowCounter_Conflict
			If labelsList.Count = 0 Then
				CType(labelsList, List(Of String)).AddRange(other.labelsList)
			End If

			If topN <> other.topN Then
				log.warn("Different topN values ({} vs {}) detected during Evaluation merging. Top N accuracy may not be accurate.", topN, other.topN)
			End If
			Me.topNCorrectCount_Conflict += other.topNCorrectCount_Conflict
			Me.topNTotalCount_Conflict += other.topNTotalCount_Conflict
		End Sub

		''' <summary>
		''' Get a String representation of the confusion matrix
		''' </summary>
		Public Overridable Function confusionToString() As String
			Dim nClasses As Integer = confusion().getClasses().Count

			'First: work out the longest label size
			Dim maxLabelSize As Integer = 0
			For Each s As String In labelsList
				maxLabelSize = Math.Max(maxLabelSize, s.Length)
			Next s

			'Build the formatting for the rows:
			Dim labelSize As Integer = Math.Max(maxLabelSize + 5, 10)
			Dim sb As New StringBuilder()
			sb.Append("%-3d")
			sb.Append("%-")
			sb.Append(labelSize)
			sb.Append("s | ")

			Dim headerFormat As New StringBuilder()
			headerFormat.Append("   %-").Append(labelSize).Append("s   ")

			For i As Integer = 0 To nClasses - 1
				sb.Append("%7d")
				headerFormat.Append("%7d")
			Next i
			Dim rowFormat As String = sb.ToString()


			Dim [out] As New StringBuilder()
			'First: header row
			Dim headerArgs(nClasses) As Object
			headerArgs(0) = "Predicted:"
			For i As Integer = 0 To nClasses - 1
				headerArgs(i + 1) = i
			Next i
			[out].Append(String.format(headerFormat.ToString(), headerArgs)).Append(vbLf)

			'Second: divider rows
			[out].Append("   Actual:" & vbLf)

			'Finally: data rows
			For i As Integer = 0 To nClasses - 1

				Dim args(nClasses + 1) As Object
				args(0) = i
				args(1) = labelsList(i)
				For j As Integer = 0 To nClasses - 1
					args(j + 2) = confusion().getCount(i, j)
				Next j
				[out].Append(String.format(rowFormat, args))
				[out].Append(vbLf)
			Next i

			Return [out].ToString()
		End Function


		Private Sub addToMetaConfusionMatrix(ByVal actual As Integer, ByVal predicted As Integer, ByVal metaData As Object)
			If confusionMatrixMetaData Is Nothing Then
				confusionMatrixMetaData = New Dictionary(Of Pair(Of Integer, Integer), IList(Of Object))()
			End If

			Dim p As New Pair(Of Integer, Integer)(actual, predicted)
			Dim list As IList(Of Object) = confusionMatrixMetaData(p)
			If list Is Nothing Then
				list = New List(Of Object)()
				confusionMatrixMetaData(p) = list
			End If

			list.Add(metaData)
		End Sub

		''' <summary>
		''' Get a list of prediction errors, on a per-record basis<br>
		''' <para>
		''' <b>Note</b>: Prediction errors are ONLY available if the "evaluate with metadata"  method is used: <seealso cref="eval(INDArray, INDArray, List)"/>
		''' Otherwise (if the metadata hasn't been recorded via that previously mentioned eval method), there is no value in
		''' splitting each prediction out into a separate Prediction object - instead, use the confusion matrix to get the counts,
		''' via <seealso cref="getConfusionMatrix()"/>
		''' 
		''' </para>
		''' </summary>
		''' <returns> A list of prediction errors, or null if no metadata has been recorded </returns>
		Public Overridable ReadOnly Property PredictionErrors As IList(Of Prediction)
			Get
				If Me.confusionMatrixMetaData Is Nothing Then
					Return Nothing
				End If
    
				Dim list As IList(Of Prediction) = New List(Of Prediction)()
    
				Dim sorted As IList(Of KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object))) = New List(Of KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object)))(confusionMatrixMetaData.SetOfKeyValuePairs())
				sorted.Sort(New ComparatorAnonymousInnerClass(Me))
    
				For Each entry As KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object)) In sorted
					Dim p As Pair(Of Integer, Integer) = entry.Key
					If p.First.Equals(p.Second) Then
						'predicted = actual -> not an error -> skip
						Continue For
					End If
					For Each m As Object In entry.Value
						list.Add(New Prediction(p.First, p.Second, m))
					Next m
				Next entry
    
				Return list
			End Get
		End Property

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object)))

			Private ReadOnly outerInstance As Evaluation

			Public Sub New(ByVal outerInstance As Evaluation)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object)), ByVal o2 As KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object))) As Integer Implements IComparer(Of KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object))).Compare
				Dim p1 As Pair(Of Integer, Integer) = o1.Key
				Dim p2 As Pair(Of Integer, Integer) = o2.Key
				Dim order As Integer = Integer.compare(p1.First, p2.First)
				If order <> 0 Then
					Return order
				End If
				order = Integer.compare(p1.Second, p2.Second)
				Return order
			End Function
		End Class

		''' <summary>
		''' Get a list of predictions, for all data with the specified <i>actual</i> class, regardless of the predicted
		''' class.
		''' <para>
		''' <b>Note</b>: Prediction errors are ONLY available if the "evaluate with metadata"  method is used: <seealso cref="eval(INDArray, INDArray, List)"/>
		''' Otherwise (if the metadata hasn't been recorded via that previously mentioned eval method), there is no value in
		''' splitting each prediction out into a separate Prediction object - instead, use the confusion matrix to get the counts,
		''' via <seealso cref="getConfusionMatrix()"/>
		''' 
		''' </para>
		''' </summary>
		''' <param name="actualClass"> Actual class to get predictions for </param>
		''' <returns> List of predictions, or null if the "evaluate with metadata" method was not used </returns>
		Public Overridable Function getPredictionsByActualClass(ByVal actualClass As Integer) As IList(Of Prediction)
			If confusionMatrixMetaData Is Nothing Then
				Return Nothing
			End If

			Dim [out] As IList(Of Prediction) = New List(Of Prediction)()
			For Each entry As KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object)) In confusionMatrixMetaData.SetOfKeyValuePairs() 'Entry Pair: (Actual,Predicted)
				If entry.Key.getFirst() = actualClass Then
					Dim actual As Integer = entry.Key.getFirst()
					Dim predicted As Integer = entry.Key.getSecond()
					For Each m As Object In entry.Value
						[out].Add(New Prediction(actual, predicted, m))
					Next m
				End If
			Next entry
			Return [out]
		End Function

		''' <summary>
		''' Get a list of predictions, for all data with the specified <i>predicted</i> class, regardless of the actual data
		''' class.
		''' <para>
		''' <b>Note</b>: Prediction errors are ONLY available if the "evaluate with metadata"  method is used: <seealso cref="eval(INDArray, INDArray, List)"/>
		''' Otherwise (if the metadata hasn't been recorded via that previously mentioned eval method), there is no value in
		''' splitting each prediction out into a separate Prediction object - instead, use the confusion matrix to get the counts,
		''' via <seealso cref="getConfusionMatrix()"/>
		''' 
		''' </para>
		''' </summary>
		''' <param name="predictedClass"> Actual class to get predictions for </param>
		''' <returns> List of predictions, or null if the "evaluate with metadata" method was not used </returns>
		Public Overridable Function getPredictionByPredictedClass(ByVal predictedClass As Integer) As IList(Of Prediction)
			If confusionMatrixMetaData Is Nothing Then
				Return Nothing
			End If

			Dim [out] As IList(Of Prediction) = New List(Of Prediction)()
			For Each entry As KeyValuePair(Of Pair(Of Integer, Integer), IList(Of Object)) In confusionMatrixMetaData.SetOfKeyValuePairs() 'Entry Pair: (Actual,Predicted)
				If entry.Key.getSecond() = predictedClass Then
					Dim actual As Integer = entry.Key.getFirst()
					Dim predicted As Integer = entry.Key.getSecond()
					For Each m As Object In entry.Value
						[out].Add(New Prediction(actual, predicted, m))
					Next m
				End If
			Next entry
			Return [out]
		End Function

		''' <summary>
		''' Get a list of predictions in the specified confusion matrix entry (i.e., for the given actua/predicted class pair)
		''' </summary>
		''' <param name="actualClass">    Actual class </param>
		''' <param name="predictedClass"> Predicted class </param>
		''' <returns> List of predictions that match the specified actual/predicted classes, or null if the "evaluate with metadata" method was not used </returns>
		Public Overridable Function getPredictions(ByVal actualClass As Integer, ByVal predictedClass As Integer) As IList(Of Prediction)
			If confusionMatrixMetaData Is Nothing Then
				Return Nothing
			End If

			Dim [out] As IList(Of Prediction) = New List(Of Prediction)()
			Dim list As IList(Of Object) = confusionMatrixMetaData(New Pair(Of Object)(actualClass, predictedClass))
			If list Is Nothing Then
				Return [out]
			End If

			For Each meta As Object In list
				[out].Add(New Prediction(actualClass, predictedClass, meta))
			Next meta
			Return [out]
		End Function

		Public Overridable Function scoreForMetric(ByVal metric As Metric) As Double
			Select Case metric.innerEnumValue
				Case org.nd4j.evaluation.classification.Evaluation.Metric.InnerEnum.ACCURACY
					Return accuracy()
				Case org.nd4j.evaluation.classification.Evaluation.Metric.InnerEnum.F1
					Return f1()
				Case org.nd4j.evaluation.classification.Evaluation.Metric.InnerEnum.PRECISION
					Return precision()
				Case org.nd4j.evaluation.classification.Evaluation.Metric.InnerEnum.RECALL
					Return recall()
				Case org.nd4j.evaluation.classification.Evaluation.Metric.InnerEnum.GMEASURE
					Return gMeasure(EvaluationAveraging.Macro)
				Case org.nd4j.evaluation.classification.Evaluation.Metric.InnerEnum.MCC
					Return matthewsCorrelation(EvaluationAveraging.Macro)
				Case Else
					Throw New System.InvalidOperationException("Unknown metric: " & metric)
			End Select
		End Function


		Public Shared Function fromJson(ByVal json As String) As Evaluation
			Return fromJson(json, GetType(Evaluation))
		End Function

		Public Shared Function fromYaml(ByVal yaml As String) As Evaluation
			Return fromYaml(yaml, GetType(Evaluation))
		End Function

		Public Overrides Function getValue(ByVal metric As IMetric) As Double
			If TypeOf metric Is Metric Then
				Return scoreForMetric(DirectCast(metric, Metric))
			Else
				Throw New System.InvalidOperationException("Can't get value for non-evaluation Metric " & metric)
			End If
		End Function

		Public Overrides Function newInstance() As Evaluation
			Return New Evaluation(axis_Conflict, binaryPositiveClass, topN, labelsList, binaryDecisionThreshold, costArray, maxWarningClassesToPrint)
		End Function
	End Class

End Namespace