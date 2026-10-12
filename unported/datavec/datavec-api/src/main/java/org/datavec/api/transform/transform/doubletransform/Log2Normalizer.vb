Imports System
Imports Data = lombok.Data
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Writable = org.datavec.api.writable.Writable
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.transform.doubletransform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class Log2Normalizer extends BaseDoubleTransform
	<Serializable>
	Public Class Log2Normalizer
		Inherits BaseDoubleTransform

'JAVA TO VB CONVERTER NOTE: The field log2 was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shared ReadOnly log2_Conflict As Double = Math.Log(2)
		Protected Friend ReadOnly columnMean As Double
		Protected Friend ReadOnly columnMin As Double
		Protected Friend ReadOnly scalingFactor As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Log2Normalizer(@JsonProperty("columnName") String columnName, @JsonProperty("columnMean") double columnMean, @JsonProperty("columnMin") double columnMin, @JsonProperty("scalingFactor") double scalingFactor)
		Public Sub New(ByVal columnName As String, ByVal columnMean As Double, ByVal columnMin As Double, ByVal scalingFactor As Double)
			MyBase.New(columnName)
			If Double.IsNaN(columnMean) OrElse Double.IsInfinity(columnMean) Then
				Throw New System.ArgumentException("Invalid input: column mean cannot be null/infinite (is: " & columnMean & ")")
			End If
			Me.columnMean = columnMean
			Me.columnMin = columnMin
			Me.scalingFactor = scalingFactor
		End Sub

		Public Overrides Function map(ByVal writable As Writable) As Writable
			Dim val As Double = writable.toDouble()
			If Double.IsNaN(val) Then
				Return New DoubleWritable(0)
			End If
			Return New DoubleWritable(normMean(val))
		End Function

		Private Function log2(ByVal x As Double) As Double
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Return Math.Log(x) / log2_Conflict
		End Function

		Private Function normMean(ByVal [in] As Double) As Double
			Return scalingFactor * log2(([in] - columnMin) / (columnMean - columnMin) + 1)
		End Function

		Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnMeta As ColumnMetaData) As ColumnMetaData
			Return New DoubleMetaData(newColumnName, 0.0, Nothing)
		End Function

		Public Overrides Function ToString() As String
			Return "Log2Normalizer(columnMean=" & columnMean & ",columnMin=" & columnMin & ",scalingFactor=" & scalingFactor & ")"
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Dim n As Number = DirectCast(input, Number)
			Dim val As Double = n.doubleValue()
			If Double.IsNaN(val) Then
				Return New DoubleWritable(0)
			End If
			Return normMean(val)
		End Function

	End Class

End Namespace