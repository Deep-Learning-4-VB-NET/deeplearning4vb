Imports System
Imports Data = lombok.Data
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"ratio", "inputSchema", "columnNumber"}) public class MinMaxNormalizer extends BaseDoubleTransform
	<Serializable>
	Public Class MinMaxNormalizer
		Inherits BaseDoubleTransform

		Protected Friend ReadOnly min As Double
		Protected Friend ReadOnly max As Double
		Protected Friend ReadOnly newMin As Double
		Protected Friend ReadOnly newMax As Double
		Protected Friend ReadOnly ratio As Double

		Public Sub New(ByVal columnName As String, ByVal min As Double, ByVal max As Double)
			Me.New(columnName, min, max, 0, 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MinMaxNormalizer(@JsonProperty("columnName") String columnName, @JsonProperty("min") double min, @JsonProperty("max") double max, @JsonProperty("newMin") double newMin, @JsonProperty("newMax") double newMax)
		Public Sub New(ByVal columnName As String, ByVal min As Double, ByVal max As Double, ByVal newMin As Double, ByVal newMax As Double)
			MyBase.New(columnName)
			Me.min = min
			Me.max = max
			Me.newMin = newMin
			Me.newMax = newMax
			Me.ratio = (newMax - newMin) / (max - min)
		End Sub

		Public Overrides Function map(ByVal writable As Writable) As Writable
			Dim val As Double = writable.toDouble()
			If Double.IsNaN(val) Then
				Return New DoubleWritable(0)
			End If
			Return New DoubleWritable(ratio * (val - min) + newMin)
		End Function

		Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnMeta As ColumnMetaData) As ColumnMetaData
			Return New DoubleMetaData(newColumnName, newMin, newMax)
		End Function

		Public Overrides Function ToString() As String
			Return "MinMaxNormalizer(min=" & min & ",max=" & max & ",newMin=" & newMin & ",newMax=" & newMax & ")"
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
			Return ratio * (val - min) + newMin
		End Function
	End Class

End Namespace