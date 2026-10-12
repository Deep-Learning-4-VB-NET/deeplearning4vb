Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports MathOp = org.datavec.api.transform.MathOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports FloatMetaData = org.datavec.api.transform.metadata.FloatMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseColumnsMathOpTransform = org.datavec.api.transform.transform.BaseColumnsMathOpTransform
Imports FloatMathOpTransform = org.datavec.api.transform.transform.floattransform.FloatMathOpTransform
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
'ORIGINAL LINE: @Data public class FloatColumnsMathOpTransform extends org.datavec.api.transform.transform.BaseColumnsMathOpTransform
	<Serializable>
	Public Class FloatColumnsMathOpTransform
		Inherits BaseColumnsMathOpTransform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FloatColumnsMathOpTransform(@JsonProperty("newColumnName") String newColumnName, @JsonProperty("mathOp") org.datavec.api.transform.MathOp mathOp, @JsonProperty("columns") java.util.List<String> columns)
		Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ByVal columns As IList(Of String))
			Me.New(newColumnName, mathOp, CType(columns, List(Of String)).ToArray())
		End Sub

		Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columns() As String)
			MyBase.New(newColumnName, mathOp, columns)
		End Sub

		Protected Friend Overrides Function derivedColumnMetaData(ByVal newColumnName As String, ByVal inputSchema As Schema) As ColumnMetaData
			Return New FloatMetaData(newColumnName)
		End Function

		Protected Friend Overrides Function doOp(ParamArray ByVal input() As Writable) As Writable
			Select Case mathOp
				Case MathOp.Add
					Dim sum As Single? = 0f
					For Each w As Writable In input
						sum += w.toFloat()
					Next w
					Return New FloatWritable(sum)
				Case MathOp.Subtract
					Return New FloatWritable(input(0).toFloat() - input(1).toFloat())
				Case MathOp.Multiply
					Dim product As Single = 1.0f
					For Each w As Writable In input
						product *= w.toFloat()
					Next w
					Return New FloatWritable(product)
				Case MathOp.Divide
					Return New FloatWritable(input(0).toFloat() / input(1).toFloat())
				Case MathOp.Modulus
					Return New FloatWritable(input(0).toFloat() Mod input(1).toFloat())
				Case Else
					Throw New Exception("Invalid mathOp: " & mathOp) 'Should never happen
			End Select
		End Function

		Public Overrides Function ToString() As String
			Return "FloatColumnsMathOpTransform(newColumnName=""" & newColumnName & """,mathOp=" & mathOp & ",columns=" & Arrays.toString(columns) & ")"
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Dim row As IList(Of Single) = DirectCast(input, IList(Of Single))
			Select Case mathOp
				Case MathOp.Add
					Dim sum As Single = 0f
					For Each w As Single? In row
						sum += w
					Next w
					Return sum
				Case MathOp.Subtract
					Return row(0) - row(1)
				Case MathOp.Multiply
					Dim product As Single = 1.0f
					For Each w As Single? In row
						product *= w
					Next w
					Return product
				Case MathOp.Divide
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Return row(0) / row(1)
				Case MathOp.Modulus
					Return row(0) Mod row(1)
				Case Else
					Throw New Exception("Invalid mathOp: " & mathOp) 'Should never happen
			End Select
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Dim seq As IList(Of IList(Of Single)) = DirectCast(sequence, IList(Of IList(Of Single)))
			Dim ret As IList(Of Single) = New List(Of Single)()
			For Each [step] As IList(Of Single) In seq
				ret.Add(DirectCast(map([step]), Single?))
			Next [step]
			Return ret
		End Function
	End Class

End Namespace