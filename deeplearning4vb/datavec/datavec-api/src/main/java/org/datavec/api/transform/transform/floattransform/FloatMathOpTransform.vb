Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports MathOp = org.datavec.api.transform.MathOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports FloatMetaData = org.datavec.api.transform.metadata.FloatMetaData
Imports BaseColumnTransform = org.datavec.api.transform.transform.BaseColumnTransform
Imports FloatColumnsMathOpTransform = org.datavec.api.transform.transform.floattransform.FloatColumnsMathOpTransform
Imports FloatWritable = org.datavec.api.writable.FloatWritable
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

Namespace org.datavec.api.transform.transform.floattransform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class FloatMathOpTransform extends org.datavec.api.transform.transform.BaseColumnTransform
	<Serializable>
	Public Class FloatMathOpTransform
		Inherits BaseColumnTransform

		Private ReadOnly mathOp As MathOp
		Private ReadOnly scalar As Single

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FloatMathOpTransform(@JsonProperty("columnName") String columnName, @JsonProperty("mathOp") org.datavec.api.transform.MathOp mathOp, @JsonProperty("scalar") float scalar)
		Public Sub New(ByVal columnName As String, ByVal mathOp As MathOp, ByVal scalar As Single)
			MyBase.New(columnName)
			Me.mathOp = mathOp
			Me.scalar = scalar
		End Sub

		Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			If Not (TypeOf oldColumnType Is FloatMetaData) Then
				Throw New System.InvalidOperationException("Column is not an float column")
			End If
			Dim meta As FloatMetaData = DirectCast(oldColumnType, FloatMetaData)
			Dim minValue As Single? = meta.getMinAllowedValue()
			Dim maxValue As Single? = meta.getMaxAllowedValue()
			If minValue IsNot Nothing Then
				minValue = doOp(minValue)
			End If
			If maxValue IsNot Nothing Then
				maxValue = doOp(maxValue)
			End If
			If minValue IsNot Nothing AndAlso maxValue IsNot Nothing AndAlso minValue > maxValue Then
				'Consider rsub 1, with original min/max of 0 and 1: (1-0) -> 1 and (1-1) -> 0
				'Or multiplication by -1: (0 to 1) -> (-1 to 0)
				'Need to swap min/max here...
				Dim temp As Single? = minValue
				minValue = maxValue
				maxValue = temp
			End If
			Return New FloatMetaData(newColumnName, minValue, maxValue)
		End Function

		Private Function doOp(ByVal input As Single) As Single
			Select Case mathOp
				Case MathOp.Add
					Return input + scalar
				Case MathOp.Subtract
					Return input - scalar
				Case MathOp.Multiply
					Return input * scalar
				Case MathOp.Divide
					Return input / scalar
				Case MathOp.Modulus
					Return input Mod scalar
				Case MathOp.ReverseSubtract
					Return scalar - input
				Case MathOp.ReverseDivide
					Return scalar / input
				Case MathOp.ScalarMin
					Return Math.Min(input, scalar)
				Case MathOp.ScalarMax
					Return Math.Max(input, scalar)
				Case Else
					Throw New System.InvalidOperationException("Unknown or not implemented math op: " & mathOp)
			End Select
		End Function

		Public Overrides Function map(ByVal columnWritable As Writable) As Writable
			Return New FloatWritable(doOp(columnWritable.toFloat()))
		End Function

		Public Overrides Function ToString() As String
			Return "FloatMathOpTransform(mathOp=" & mathOp & ",scalar=" & scalar & ")"
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Overloads Function map(ByVal input As Object) As Object
			If TypeOf input Is Number Then
				Dim number As Number = DirectCast(input, Number)
				Return doOp(number.floatValue())
			End If
			Throw New System.ArgumentException("Input must be a number")
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<?> list = (java.util.List<?>) sequence;
			Dim list As IList(Of Object) = DirectCast(sequence, IList(Of Object))
			Dim ret As IList(Of Object) = New List(Of Object)()
			For Each o As Object In list
				ret.Add(map(o))
			Next o
			Return ret
		End Function
	End Class

End Namespace