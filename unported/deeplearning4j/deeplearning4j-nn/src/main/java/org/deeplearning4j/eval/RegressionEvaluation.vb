Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

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

Namespace org.deeplearning4j.eval


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated @Data @EqualsAndHashCode(callSuper = true) public class RegressionEvaluation extends org.nd4j.evaluation.regression.RegressionEvaluation implements IEvaluation<org.nd4j.evaluation.regression.RegressionEvaluation>
	<Obsolete>
	Public Class RegressionEvaluation
		Inherits org.nd4j.evaluation.regression.RegressionEvaluation
		Implements IEvaluation(Of org.nd4j.evaluation.regression.RegressionEvaluation)

		<Obsolete("Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.regression.RegressionEvaluation.Metric""/>")>
		Public NotInheritable Class Metric
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
			Public Function minimize() As Boolean
				Return outerInstance.toNd4j().minimize()
			End Function

			Public Function toNd4j() As org.nd4j.evaluation.regression.RegressionEvaluation.Metric
				Select Case Me
					Case MSE
						Return org.nd4j.evaluation.regression.RegressionEvaluation.Metric.MSE
					Case MAE
						Return org.nd4j.evaluation.regression.RegressionEvaluation.Metric.MAE
					Case RMSE
						Return org.nd4j.evaluation.regression.RegressionEvaluation.Metric.RMSE
					Case RSE
						Return org.nd4j.evaluation.regression.RegressionEvaluation.Metric.RSE
					Case PC
						Return org.nd4j.evaluation.regression.RegressionEvaluation.Metric.PC
					Case R2
						Return org.nd4j.evaluation.regression.RegressionEvaluation.Metric.R2
					Case Else
						Throw New System.InvalidOperationException("Unknown enum: " & Me)
				End Select
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

		''' @deprecated Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.regression.RegressionEvaluation"/> 
		<Obsolete("Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.regression.RegressionEvaluation""/>")>
		Public Sub New()
		End Sub
		<Obsolete("Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.regression.RegressionEvaluation""/>")>
		Public Sub New(ByVal nColumns As Long)
			MyBase.New(nColumns)
		End Sub

		''' @deprecated Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.regression.RegressionEvaluation"/> 
		<Obsolete("Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.regression.RegressionEvaluation""/>")>
		Public Sub New(ByVal nColumns As Long, ByVal precision As Long)
			MyBase.New(nColumns, precision)
		End Sub

		''' @deprecated Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.regression.RegressionEvaluation"/> 
		<Obsolete("Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.regression.RegressionEvaluation""/>")>
		Public Sub New(ParamArray ByVal columnNames() As String)
			MyBase.New(columnNames)
		End Sub

		''' @deprecated Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.regression.RegressionEvaluation"/> 
		<Obsolete("Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.regression.RegressionEvaluation""/>")>
		Public Sub New(ByVal columnNames As IList(Of String))
			MyBase.New(columnNames)
		End Sub

		''' @deprecated Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.regression.RegressionEvaluation"/> 
		<Obsolete("Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.regression.RegressionEvaluation""/>")>
		Public Sub New(ByVal columnNames As IList(Of String), ByVal precision As Long)
			MyBase.New(columnNames, precision)
		End Sub

		''' @deprecated Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref="org.nd4j.evaluation.regression.RegressionEvaluation"/> 
		<Obsolete("Use ND4J RegressionEvaluation class, which has the same interface: <seealso cref=""org.nd4j.evaluation.regression.RegressionEvaluation""/>")>
		Public Overrides Function scoreForMetric(ByVal metric As Metric) As Double
			Return scoreForMetric(metric.toNd4j())
		End Function
	End Class

End Namespace