Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports MathOp = org.datavec.api.transform.MathOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports LongMetaData = org.datavec.api.transform.metadata.LongMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseColumnsMathOpTransform = org.datavec.api.transform.transform.BaseColumnsMathOpTransform
Imports DoubleColumnsMathOpTransform = org.datavec.api.transform.transform.doubletransform.DoubleColumnsMathOpTransform
Imports LongWritable = org.datavec.api.writable.LongWritable
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

Namespace org.datavec.api.transform.transform.longtransform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class LongColumnsMathOpTransform extends org.datavec.api.transform.transform.BaseColumnsMathOpTransform
	<Serializable>
	Public Class LongColumnsMathOpTransform
		Inherits BaseColumnsMathOpTransform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LongColumnsMathOpTransform(@JsonProperty("newColumnName") String newColumnName, @JsonProperty("mathOp") org.datavec.api.transform.MathOp mathOp, @JsonProperty("columns") String... columns)
		Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columns() As String)
			MyBase.New(newColumnName, mathOp, columns)
		End Sub

		Protected Friend Overrides Function derivedColumnMetaData(ByVal newColumnName As String, ByVal inputSchema As Schema) As ColumnMetaData
			Return New LongMetaData(newColumnName)
		End Function

		Protected Friend Overrides Function doOp(ParamArray ByVal input() As Writable) As Writable
			Select Case mathOp
				Case MathOp.Add
					Dim sum As Long = 0
					For Each w As Writable In input
						sum += w.toLong()
					Next w
					Return New LongWritable(sum)
				Case MathOp.Subtract
					Return New LongWritable(input(0).toLong() - input(1).toLong())
				Case MathOp.Multiply
					Dim product As Long = 1
					For Each w As Writable In input
						product *= w.toLong()
					Next w
					Return New LongWritable(product)
				Case MathOp.Divide
					Return New LongWritable(input(0).toLong() \ input(1).toLong())
				Case MathOp.Modulus
					Return New LongWritable(input(0).toLong() Mod input(1).toLong())
				Case Else
					Throw New Exception("Invalid mathOp: " & mathOp) 'Should never happen
			End Select
		End Function

		Public Overrides Function ToString() As String
			Return "LongColumnsMathOpTransform(newColumnName=""" & newColumnName & """,mathOp=" & mathOp & ",columns=" & Arrays.toString(columns) & ")"
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Dim list As IList(Of Long) = DirectCast(input, IList(Of Long))
			Select Case mathOp
				Case MathOp.Add
					Dim sum As Long = 0
					For Each w As Long? In list
						sum += w
					Next w
					Return New LongWritable(sum)
				Case MathOp.Subtract
					Return list(0) - list(1)
				Case MathOp.Multiply
					Dim product As Long = 1
					For Each w As Long? In list
						product *= w
					Next w
					Return product
				Case MathOp.Divide
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Return list(0) / list(1)
				Case MathOp.Modulus
					Return list(0) Mod list(1)
				Case Else
					Throw New Exception("Invalid mathOp: " & mathOp) 'Should never happen
			End Select
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Dim seq As IList(Of IList(Of Long)) = DirectCast(sequence, IList(Of IList(Of Long)))
			Dim ret As IList(Of Long) = New List(Of Long)()
			For Each l As IList(Of Long) In seq
				ret.Add(DirectCast(map(l), Long?))
			Next l
			Return ret
		End Function
	End Class

End Namespace