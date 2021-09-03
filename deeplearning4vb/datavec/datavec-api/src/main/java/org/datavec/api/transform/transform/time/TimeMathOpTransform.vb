Imports System
Imports Data = lombok.Data
Imports MathOp = org.datavec.api.transform.MathOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports TimeMetaData = org.datavec.api.transform.metadata.TimeMetaData
Imports BaseColumnTransform = org.datavec.api.transform.transform.BaseColumnTransform
Imports LongWritable = org.datavec.api.writable.LongWritable
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

Namespace org.datavec.api.transform.transform.time


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "columnNumber", "asMilliseconds"}) @Data public class TimeMathOpTransform extends org.datavec.api.transform.transform.BaseColumnTransform
	<Serializable>
	Public Class TimeMathOpTransform
		Inherits BaseColumnTransform

		Private ReadOnly mathOp As MathOp
		Private ReadOnly timeQuantity As Long
		Private ReadOnly timeUnit As TimeUnit
		Private ReadOnly asMilliseconds As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TimeMathOpTransform(@JsonProperty("columnName") String columnName, @JsonProperty("mathOp") org.datavec.api.transform.MathOp mathOp, @JsonProperty("timeQuantity") long timeQuantity, @JsonProperty("timeUnit") java.util.concurrent.TimeUnit timeUnit)
		Public Sub New(ByVal columnName As String, ByVal mathOp As MathOp, ByVal timeQuantity As Long, ByVal timeUnit As TimeUnit)
			MyBase.New(columnName)
			If mathOp <> MathOp.Add AndAlso mathOp <> MathOp.Subtract AndAlso mathOp <> MathOp.ScalarMin AndAlso mathOp <> MathOp.ScalarMax Then
				Throw New System.ArgumentException("Invalid MathOp: only Add/Subtract/ScalarMin/ScalarMax supported")
			End If
			If (mathOp = MathOp.ScalarMin OrElse mathOp = MathOp.ScalarMax) AndAlso timeUnit <> TimeUnit.MILLISECONDS Then
				Throw New System.ArgumentException("Only valid time unit for ScalarMin/Max is Milliseconds (i.e., timestamp format)")
			End If

			Me.mathOp = mathOp
			Me.timeQuantity = timeQuantity
			Me.timeUnit = timeUnit
			Me.asMilliseconds = TimeUnit.MILLISECONDS.convert(timeQuantity, timeUnit)
		End Sub

		Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			If Not (TypeOf oldColumnType Is TimeMetaData) Then
				Throw New System.InvalidOperationException("Cannot execute TimeMathOpTransform on column with type " & oldColumnType)
			End If

			Return New TimeMetaData(newName, DirectCast(oldColumnType, TimeMetaData).getTimeZone())
		End Function

		Public Overrides Function map(ByVal columnWritable As Writable) As Writable
			Dim currTime As Long = columnWritable.toLong()
			Select Case mathOp
				Case MathOp.Add
					Return New LongWritable(currTime + asMilliseconds)
				Case MathOp.Subtract
					Return New LongWritable(currTime - asMilliseconds)
				Case MathOp.ScalarMax
					Return New LongWritable(Math.Max(asMilliseconds, currTime))
				Case MathOp.ScalarMin
					Return New LongWritable(Math.Min(asMilliseconds, currTime))
				Case Else
					Throw New Exception("Invalid MathOp for TimeMathOpTransform: " & mathOp)
			End Select
		End Function

		Public Overrides Function ToString() As String
			Return "TimeMathOpTransform(mathOp=" & mathOp & "," & timeQuantity & "-" & timeUnit & ")"
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Overloads Function map(ByVal input As Object) As Object
			Return Nothing
		End Function
	End Class

End Namespace