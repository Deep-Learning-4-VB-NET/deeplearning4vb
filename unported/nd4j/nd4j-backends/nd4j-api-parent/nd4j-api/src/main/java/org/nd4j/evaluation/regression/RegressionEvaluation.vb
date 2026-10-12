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
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ASum = org.nd4j.linalg.api.ops.impl.reduce.same.ASum
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.evaluation.regression


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class RegressionEvaluation extends org.nd4j.evaluation.BaseEvaluation<RegressionEvaluation>
	Public Class RegressionEvaluation
		Inherits BaseEvaluation(Of RegressionEvaluation)

		Public NotInheritable Class Metric Implements IMetric
			Public Shared ReadOnly MSE As New Metric("MSE", InnerEnum.MSE)
			Public Shared ReadOnly MAE As New Metric("MAE", InnerEnum.MAE)
			Public Shared ReadOnly RMSE As New Metric("RMSE", InnerEnum.RMSE)
			Public Shared ReadOnly RSE As New Metric("RSE", InnerEnum.RSE)
			Public Shared ReadOnly PC As New Metric("PC", InnerEnum.PC)
			Public Shared ReadOnly R2 As New Metric("R2", InnerEnum.R2)

			Private Shared ReadOnly valueList As New List(Of Metric)()

			Shared Sub New()
				valueList.Add(MSE)
				valueList.Add(MAE)
				valueList.Add(RMSE)
				valueList.Add(RSE)
				valueList.Add(PC)
				valueList.Add(R2)
			End Sub

			Public Enum InnerEnum
				MSE
				MAE
				RMSE
				RSE
				PC
				R2
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
					Return GetType(RegressionEvaluation)
				End Get
			End Property

			''' <returns> True if the metric should be minimized, or false if the metric should be maximized.
			''' For example, MSE of 0 is best, but R^2 of 1.0 is best </returns>
			Public Function minimize() As Boolean Implements IMetric.minimize
				If Me = R2 OrElse Me = PC Then
					Return False
				End If
				Return True
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

		Public Const DEFAULT_PRECISION As Integer = 5

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode.Exclude protected int axis = 1;
'JAVA TO VB CONVERTER NOTE: The field axis was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend axis_Conflict As Integer = 1
		Private initialized As Boolean
		Private columnNames As IList(Of String)
		Private precision As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray exampleCountPerColumn;
		Private exampleCountPerColumn As INDArray 'Necessary to account for per-output masking
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray labelsSumPerColumn;
		Private labelsSumPerColumn As INDArray 'sum(actual) per column -> used to calculate mean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray sumSquaredErrorsPerColumn;
		Private sumSquaredErrorsPerColumn As INDArray '(predicted - actual)^2
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray sumAbsErrorsPerColumn;
		Private sumAbsErrorsPerColumn As INDArray 'abs(predicted-actial)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray currentMean;
		Private currentMean As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray currentPredictionMean;
		Private currentPredictionMean As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray sumOfProducts;
		Private sumOfProducts As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray sumSquaredLabels;
		Private sumSquaredLabels As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray sumSquaredPredicted;
		Private sumSquaredPredicted As INDArray
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private org.nd4j.linalg.api.ndarray.INDArray sumLabels;
		Private sumLabels As INDArray

		Protected Friend Sub New(ByVal axis As Integer, ByVal columnNames As IList(Of String), ByVal precision As Long)
			Me.axis_Conflict = axis
			Me.columnNames = columnNames
			Me.precision = precision
		End Sub

		Public Sub New()
			Me.New(Nothing, DEFAULT_PRECISION)
		End Sub

		''' <summary>
		''' Create a regression evaluation object with the specified number of columns, and default precision
		''' for the stats() method. </summary>
		''' <param name="nColumns"> Number of columns </param>
		Public Sub New(ByVal nColumns As Long)
			Me.New(createDefaultColumnNames(nColumns), DEFAULT_PRECISION)
		End Sub

		''' <summary>
		''' Create a regression evaluation object with the specified number of columns, and specified precision
		''' for the stats() method. </summary>
		''' <param name="nColumns"> Number of columns </param>
		Public Sub New(ByVal nColumns As Long, ByVal precision As Long)
			Me.New(createDefaultColumnNames(nColumns), precision)
		End Sub

		''' <summary>
		''' Create a regression evaluation object with default precision for the stats() method </summary>
		''' <param name="columnNames"> Names of the columns </param>
		Public Sub New(ParamArray ByVal columnNames() As String)
			Me.New(If(columnNames Is Nothing OrElse columnNames.Length = 0, Nothing, Arrays.asList(columnNames)), DEFAULT_PRECISION)
		End Sub

		''' <summary>
		''' Create a regression evaluation object with default precision for the stats() method </summary>
		''' <param name="columnNames"> Names of the columns </param>
		Public Sub New(ByVal columnNames As IList(Of String))
			Me.New(columnNames, DEFAULT_PRECISION)
		End Sub

		''' <summary>
		''' Create a regression evaluation object with specified precision for the stats() method </summary>
		''' <param name="columnNames"> Names of the columns </param>
		Public Sub New(ByVal columnNames As IList(Of String), ByVal precision As Long)
			Me.precision = precision

			If columnNames Is Nothing OrElse columnNames.Count = 0 Then
				initialized = False
			Else
				Me.columnNames = columnNames
				initialize(columnNames.Count)
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


		Public Overrides Sub reset()
			initialized = False
		End Sub

		Private Sub initialize(ByVal n As Integer)
			If columnNames Is Nothing OrElse columnNames.Count <> n Then
				columnNames = createDefaultColumnNames(n)
			End If
			exampleCountPerColumn = Nd4j.zeros(DataType.DOUBLE, n)
			labelsSumPerColumn = Nd4j.zeros(DataType.DOUBLE, n)
			sumSquaredErrorsPerColumn = Nd4j.zeros(DataType.DOUBLE, n)
			sumAbsErrorsPerColumn = Nd4j.zeros(DataType.DOUBLE, n)
			currentMean = Nd4j.zeros(DataType.DOUBLE, n)

			currentPredictionMean = Nd4j.zeros(DataType.DOUBLE, n)
			sumOfProducts = Nd4j.zeros(DataType.DOUBLE, n)
			sumSquaredLabels = Nd4j.zeros(DataType.DOUBLE, n)
			sumSquaredPredicted = Nd4j.zeros(DataType.DOUBLE, n)
			sumLabels = Nd4j.zeros(DataType.DOUBLE, n)

			initialized = True
		End Sub

		Private Shared Function createDefaultColumnNames(ByVal nColumns As Long) As IList(Of String)
			Dim list As IList(Of String) = New List(Of String)(CInt(nColumns))
			For i As Integer = 0 To nColumns - 1
				list.Add("col_" & i)
			Next i
			Return list
		End Function

		Public Overrides Sub eval(ByVal labels As INDArray, ByVal predictions As INDArray)
			eval(labels, predictions, DirectCast(Nothing, INDArray))
		End Sub

		Public Overrides Sub eval(Of T1 As Serializable)(ByVal labels As INDArray, ByVal networkPredictions As INDArray, ByVal maskArray As INDArray, ByVal recordMetaData As IList(Of T1))
			eval(labels, networkPredictions, maskArray)
		End Sub

		Public Overrides Sub eval(ByVal labelsArr As INDArray, ByVal predictionsArr As INDArray, ByVal maskArr As INDArray)
			Dim p As Triple(Of INDArray, INDArray, INDArray) = BaseEvaluation.reshapeAndExtractNotMasked(labelsArr, predictionsArr, maskArr, axis_Conflict)
			Dim labels As INDArray = p.getFirst()
			Dim predictions As INDArray = p.getSecond()
			Dim maskArray As INDArray = p.getThird()

			If labels.dataType() <> predictions.dataType() Then
				labels = labels.castTo(predictions.dataType())
			End If

			If Not initialized Then
				initialize(CInt(labels.size(1)))
			End If
			'References for the calculations is this section:
			'https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Online_algorithm
			'https://en.wikipedia.org/wiki/Pearson_product-moment_correlation_coefficient#For_a_sample
			'Doing online calculation of means, sum of squares, etc.

			If columnNames.Count <> labels.size(1) OrElse columnNames.Count <> predictions.size(1) Then
				Throw New System.ArgumentException("Number of the columns of labels and predictions must match specification (" & columnNames.Count & "). Got " & labels.size(1) & " and " & predictions.size(1))
			End If

			If maskArray IsNot Nothing Then
				'Handle per-output masking. We are assuming *binary* masks here
				labels = labels.mul(maskArray)
				predictions = predictions.mul(maskArray)
			End If

			labelsSumPerColumn.addi(labels.sum(0).castTo(labelsSumPerColumn.dataType()))

			Dim [error] As INDArray = predictions.sub(labels)
			Dim absErrorSum As INDArray = Nd4j.Executioner.exec(New ASum([error], 0))
			Dim squaredErrorSum As INDArray = [error].mul([error]).sum(0)

			sumAbsErrorsPerColumn.addi(absErrorSum.castTo(labelsSumPerColumn.dataType()))
			sumSquaredErrorsPerColumn.addi(squaredErrorSum.castTo(labelsSumPerColumn.dataType()))

			sumOfProducts.addi(labels.mul(predictions).sum(0).castTo(labelsSumPerColumn.dataType()))

			sumSquaredLabels.addi(labels.mul(labels).sum(0).castTo(labelsSumPerColumn.dataType()))
			sumSquaredPredicted.addi(predictions.mul(predictions).sum(0).castTo(labelsSumPerColumn.dataType()))


			Dim nRows As val = labels.size(0)

			Dim newExampleCountPerColumn As INDArray
			If maskArray Is Nothing Then
				newExampleCountPerColumn = exampleCountPerColumn.add(nRows)
			Else
				newExampleCountPerColumn = exampleCountPerColumn.add(maskArray.sum(0).castTo(labelsSumPerColumn.dataType()))
			End If
			currentMean.muliRowVector(exampleCountPerColumn).addi(labels.sum(0).castTo(labelsSumPerColumn.dataType())).diviRowVector(newExampleCountPerColumn)
			currentPredictionMean.muliRowVector(exampleCountPerColumn).addi(predictions.sum(0).castTo(labelsSumPerColumn.dataType())).divi(newExampleCountPerColumn)
			exampleCountPerColumn = newExampleCountPerColumn

			sumLabels.addi(labels.sum(0).castTo(labelsSumPerColumn.dataType()))
		End Sub

		Public Overrides Sub merge(ByVal other As RegressionEvaluation)

			If other.labelsSumPerColumn Is Nothing Then
				'Other RegressionEvaluation is empty -> no op
				Return

			ElseIf labelsSumPerColumn Is Nothing Then
				'This RegressionEvaluation is empty -> just copy over from the other one...
				Me.columnNames = other.columnNames
				Me.precision = other.precision
				Me.exampleCountPerColumn = other.exampleCountPerColumn
				Me.labelsSumPerColumn = other.labelsSumPerColumn.dup()
				Me.sumSquaredErrorsPerColumn = other.sumSquaredErrorsPerColumn.dup()
				Me.sumAbsErrorsPerColumn = other.sumAbsErrorsPerColumn.dup()
				Me.currentMean = other.currentMean.dup()
				Me.currentPredictionMean = other.currentPredictionMean.dup()
				Me.sumOfProducts = other.sumOfProducts.dup()
				Me.sumSquaredLabels = other.sumSquaredLabels.dup()
				Me.sumSquaredPredicted = other.sumSquaredPredicted.dup()

				Return
			End If

			Me.labelsSumPerColumn.addi(other.labelsSumPerColumn)
			Me.sumSquaredErrorsPerColumn.addi(other.sumSquaredErrorsPerColumn)
			Me.sumAbsErrorsPerColumn.addi(other.sumAbsErrorsPerColumn)
			Me.currentMean.muliRowVector(exampleCountPerColumn).addi(other.currentMean.mulRowVector(other.exampleCountPerColumn)).diviRowVector(exampleCountPerColumn.add(other.exampleCountPerColumn))
			Me.currentPredictionMean.muliRowVector(exampleCountPerColumn).addi(other.currentPredictionMean.mulRowVector(other.exampleCountPerColumn)).diviRowVector(exampleCountPerColumn.add(other.exampleCountPerColumn))
			Me.sumOfProducts.addi(other.sumOfProducts)
			Me.sumSquaredLabels.addi(other.sumSquaredLabels)
			Me.sumSquaredPredicted.addi(other.sumSquaredPredicted)

			Me.exampleCountPerColumn.addi(other.exampleCountPerColumn)
		End Sub

		Public Overrides Function stats() As String
			If Not initialized Then
				Return "RegressionEvaluation: No Data"
			Else

				If columnNames Is Nothing Then
					columnNames = createDefaultColumnNames(numColumns())
				End If
				Dim maxLabelLength As Integer = 0
				For Each s As String In columnNames
					maxLabelLength = Math.Max(maxLabelLength, s.Length)
				Next s

				Dim labelWidth As Integer = maxLabelLength + 5
				Dim columnWidth As Long = precision + 10

				Dim resultFormat As String = "%-" & labelWidth & "s" & "%-" & columnWidth & "." & precision & "e" & "%-" & columnWidth & "." & precision & "e" & "%-" & columnWidth & "." & precision & "e" & "%-" & columnWidth & "." & precision & "e" & "%-" & columnWidth & "." & precision & "e" & "%-" & columnWidth & "." & precision & "e" 'R2

				'Print header:
				Dim sb As New StringBuilder()
				Dim headerFormat As String = "%-" & labelWidth & "s" & "%-" & columnWidth & "s" & "%-" & columnWidth & "s" & "%-" & columnWidth & "s" & "%-" & columnWidth & "s" & "%-" & columnWidth & "s" & "%-" & columnWidth & "s" ' R2

				sb.Append(String.format(headerFormat, "Column", "MSE", "MAE", "RMSE", "RSE", "PC", "R^2"))
				sb.Append(vbLf)

				'Print results for each column:
				For i As Integer = 0 To columnNames.Count - 1
					Dim name As String = columnNames(i)
					Dim mse As Double = meanSquaredError(i)
					Dim mae As Double = meanAbsoluteError(i)
					Dim rmse As Double = rootMeanSquaredError(i)
					Dim rse As Double = relativeSquaredError(i)
					Dim corr As Double = pearsonCorrelation(i)
					Dim r2 As Double = rSquared(i)

					sb.Append(String.format(resultFormat, name, mse, mae, rmse, rse, corr, r2))
					sb.Append(vbLf)
				Next i

				Return sb.ToString()
			End If
		End Function

		Public Overridable Function numColumns() As Integer
			If columnNames Is Nothing Then
				If exampleCountPerColumn Is Nothing Then
					Return 0
				End If
				Return CInt(exampleCountPerColumn.size(1))
			End If
			Return columnNames.Count
		End Function

		Public Overridable Function meanSquaredError(ByVal column As Integer) As Double
			'mse per column: 1/n * sum((predicted-actual)^2)
			Return sumSquaredErrorsPerColumn.getDouble(column) / exampleCountPerColumn.getDouble(column)
		End Function

		Public Overridable Function meanAbsoluteError(ByVal column As Integer) As Double
			'mse per column: 1/n * |predicted-actual|
			Return sumAbsErrorsPerColumn.getDouble(column) / exampleCountPerColumn.getDouble(column)
		End Function

		Public Overridable Function rootMeanSquaredError(ByVal column As Integer) As Double
			'rmse per column: sqrt(1/n * sum((predicted-actual)^2)
			Return Math.Sqrt(sumSquaredErrorsPerColumn.getDouble(column) / exampleCountPerColumn.getDouble(column))
		End Function

		''' <summary>
		''' Legacy method for the correlation score.
		''' </summary>
		''' <param name="column"> Column to evaluate </param>
		''' <returns> Pearson Correlation for the given column </returns>
		''' <seealso cref= <seealso cref="pearsonCorrelation(Integer)"/> </seealso>
		''' @deprecated Use <seealso cref="pearsonCorrelation(Integer)"/> instead.
		''' For the R2 score use <seealso cref="rSquared(Integer)"/>. 
		<Obsolete("Use <seealso cref=""pearsonCorrelation(Integer)""/> instead.")>
		Public Overridable Function correlationR2(ByVal column As Integer) As Double
			Return pearsonCorrelation(column)
		End Function

		''' <summary>
		''' Pearson Correlation Coefficient for samples
		''' </summary>
		''' <param name="column"> Column to evaluate </param>
		''' <returns> Pearson Correlation Coefficient for column with index {@code column} </returns>
		''' <seealso cref= <a href="https://en.wikipedia.org/wiki/Pearson_correlation_coefficient#For_a_sample">Wikipedia</a> </seealso>
		Public Overridable Function pearsonCorrelation(ByVal column As Integer) As Double
			Dim sumxiyi As Double = sumOfProducts.getDouble(column)
			Dim predictionMean As Double = currentPredictionMean.getDouble(column)
			Dim labelMean As Double = currentMean.getDouble(column)

			Dim sumSquaredLabels As Double = Me.sumSquaredLabels.getDouble(column)
			Dim sumSquaredPredicted As Double = Me.sumSquaredPredicted.getDouble(column)

			Dim exampleCount As Double = exampleCountPerColumn.getDouble(column)
			Dim r As Double = sumxiyi - exampleCount * predictionMean * labelMean
			r /= Math.Sqrt(sumSquaredLabels - exampleCount * labelMean * labelMean) * Math.Sqrt(sumSquaredPredicted - exampleCount * predictionMean * predictionMean)

			Return r
		End Function

		''' <summary>
		''' Coefficient of Determination (R^2 Score)
		''' </summary>
		''' <param name="column"> Column to evaluate </param>
		''' <returns> R^2 score for column with index {@code column} </returns>
		''' <seealso cref= <a href="https://en.wikipedia.org/wiki/Coefficient_of_determination">Wikipedia</a> </seealso>
		Public Overridable Function rSquared(ByVal column As Integer) As Double
			'ss_tot = sum_i (label_i - mean(labels))^2
			'       = (sum_i label_i^2) + mean(labels) * (n * mean(labels) - 2 * sum_i label_i)
			Dim sumLabelSquared As Double = sumSquaredLabels.getDouble(column)
			Dim meanLabel As Double = currentMean.getDouble(column)
			Dim sumLabel As Double = sumLabels.getDouble(column)
			Dim n As Double = exampleCountPerColumn.getDouble(column)
			Dim sstot As Double = sumLabelSquared + meanLabel * (n * meanLabel - 2 * sumLabel)
			Dim ssres As Double = sumSquaredErrorsPerColumn.getDouble(column)
			Return (sstot - ssres) / sstot
		End Function

		Public Overridable Function relativeSquaredError(ByVal column As Integer) As Double
			' RSE: sum(predicted-actual)^2 / sum(actual-labelsMean)^2
			' (sum(predicted^2) - 2 * sum(predicted * actual) + sum(actual ^ 2)) / (sum(actual ^ 2) - n * actualMean)
			Dim numerator As Double = sumSquaredPredicted.getDouble(column) - 2 * sumOfProducts.getDouble(column) + sumSquaredLabels.getDouble(column)
			Dim denominator As Double = sumSquaredLabels.getDouble(column) - exampleCountPerColumn.getDouble(column) * currentMean.getDouble(column) * currentMean.getDouble(column)

			If Math.Abs(denominator) > Nd4j.EPS_THRESHOLD Then
				Return numerator / denominator
			Else
				Return Double.PositiveInfinity
			End If
		End Function


		''' <summary>
		''' Average MSE across all columns
		''' @return
		''' </summary>
		Public Overridable Function averageMeanSquaredError() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numColumns()
				ret += meanSquaredError(i)
				i += 1
			Loop

			Return ret / CDbl(numColumns())
		End Function

		''' <summary>
		''' Average MAE across all columns
		''' @return
		''' </summary>
		Public Overridable Function averageMeanAbsoluteError() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numColumns()
				ret += meanAbsoluteError(i)
				i += 1
			Loop

			Return ret / CDbl(numColumns())
		End Function

		''' <summary>
		''' Average RMSE across all columns
		''' @return
		''' </summary>
		Public Overridable Function averagerootMeanSquaredError() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numColumns()
				ret += rootMeanSquaredError(i)
				i += 1
			Loop

			Return ret / CDbl(numColumns())
		End Function


		''' <summary>
		''' Average RSE across all columns
		''' @return
		''' </summary>
		Public Overridable Function averagerelativeSquaredError() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numColumns()
				ret += relativeSquaredError(i)
				i += 1
			Loop

			Return ret / CDbl(numColumns())
		End Function


		''' <summary>
		''' Legacy method for the correlation average across all columns.
		''' </summary>
		''' <returns> Pearson Correlation averaged over all columns </returns>
		''' <seealso cref= <seealso cref="averagePearsonCorrelation()"/> </seealso>
		''' @deprecated Use <seealso cref="averagePearsonCorrelation()"/> instead.
		''' For the R2 score use <seealso cref="averageRSquared()"/>. 
		<Obsolete("Use <seealso cref=""averagePearsonCorrelation()""/> instead.")>
		Public Overridable Function averagecorrelationR2() As Double
			Return averagePearsonCorrelation()
		End Function

		''' <summary>
		''' Average Pearson Correlation Coefficient across all columns
		''' </summary>
		''' <returns> Pearson Correlation Coefficient across all columns </returns>
		Public Overridable Function averagePearsonCorrelation() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numColumns()
				ret += pearsonCorrelation(i)
				i += 1
			Loop

			Return ret / CDbl(numColumns())
		End Function

		''' <summary>
		''' Average R2 across all columns
		''' </summary>
		''' <returns> R2 score accross all columns </returns>
		Public Overridable Function averageRSquared() As Double
			Dim ret As Double = 0.0
			Dim i As Integer = 0
			Do While i < numColumns()
				ret += rSquared(i)
				i += 1
			Loop

			Return ret / CDbl(numColumns())
		End Function

		Public Overrides Function getValue(ByVal metric As IMetric) As Double
			If TypeOf metric Is Metric Then
				Return scoreForMetric(DirectCast(metric, Metric))
			Else
				Throw New System.InvalidOperationException("Can't get value for non-regression Metric " & metric)
			End If
		End Function

		Public Overridable Function scoreForMetric(ByVal metric As Metric) As Double
			Select Case metric.innerEnumValue
				Case org.nd4j.evaluation.regression.RegressionEvaluation.Metric.InnerEnum.MSE
					Return averageMeanSquaredError()
				Case org.nd4j.evaluation.regression.RegressionEvaluation.Metric.InnerEnum.MAE
					Return averageMeanAbsoluteError()
				Case org.nd4j.evaluation.regression.RegressionEvaluation.Metric.InnerEnum.RMSE
					Return averagerootMeanSquaredError()
				Case org.nd4j.evaluation.regression.RegressionEvaluation.Metric.InnerEnum.RSE
					Return averagerelativeSquaredError()
				Case org.nd4j.evaluation.regression.RegressionEvaluation.Metric.InnerEnum.PC
					Return averagePearsonCorrelation()
				Case org.nd4j.evaluation.regression.RegressionEvaluation.Metric.InnerEnum.R2
					Return averageRSquared()
				Case Else
					Throw New System.InvalidOperationException("Unknown metric: " & metric)
			End Select
		End Function

		Public Shared Function fromJson(ByVal json As String) As RegressionEvaluation
			Return fromJson(json, GetType(RegressionEvaluation))
		End Function

		Public Overrides Function newInstance() As RegressionEvaluation
			Return New RegressionEvaluation(axis_Conflict, columnNames, precision)
		End Function
	End Class

End Namespace