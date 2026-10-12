Imports System
Imports System.Linq
Imports Data = lombok.Data
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports MathOp = org.datavec.api.transform.MathOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports NDArrayMetaData = org.datavec.api.transform.metadata.NDArrayMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseColumnsMathOpTransform = org.datavec.api.transform.transform.BaseColumnsMathOpTransform
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.datavec.api.transform.ndarray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class NDArrayColumnsMathOpTransform extends org.datavec.api.transform.transform.BaseColumnsMathOpTransform
	<Serializable>
	Public Class NDArrayColumnsMathOpTransform
		Inherits BaseColumnsMathOpTransform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayColumnsMathOpTransform(@JsonProperty("newColumnName") String newColumnName, @JsonProperty("mathOp") org.datavec.api.transform.MathOp mathOp, @JsonProperty("columns") String... columns)
		Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columns() As String)
			MyBase.New(newColumnName, mathOp, columns)
		End Sub

		Protected Friend Overrides Function derivedColumnMetaData(ByVal newColumnName As String, ByVal inputSchema As Schema) As ColumnMetaData
			'Check types

			For i As Integer = 0 To columns.Length - 1
				If inputSchema.getMetaData(columns(i)).ColumnType <> ColumnType.NDArray Then
					Throw New Exception("Column " & columns(i) & " is not an NDArray column")
				End If
			Next i

			'Check shapes
			Dim meta As NDArrayMetaData = DirectCast(inputSchema.getMetaData(columns(0)), NDArrayMetaData)
			For i As Integer = 1 To columns.Length - 1
				Dim meta2 As NDArrayMetaData = DirectCast(inputSchema.getMetaData(columns(i)), NDArrayMetaData)
				If Not meta.getShape().SequenceEqual(meta2.getShape()) Then
					Throw New System.NotSupportedException("Cannot perform NDArray operation on columns with different shapes: " & "Columns """ & columns(0) & """ and """ & columns(i) & """ have shapes: " & Arrays.toString(meta.getShape()) & " and " & Arrays.toString(meta2.getShape()))
				End If
			Next i

			Return New NDArrayMetaData(newColumnName, meta.getShape())
		End Function

		Protected Friend Overrides Function doOp(ParamArray ByVal input() As Writable) As Writable
			Dim [out] As INDArray = DirectCast(input(0), NDArrayWritable).get().dup()

			Select Case mathOp
				Case MathOp.Add
					For i As Integer = 1 To input.Length - 1
						[out].addi(DirectCast(input(i), NDArrayWritable).get())
					Next i
				Case MathOp.Subtract
					[out].subi(DirectCast(input(1), NDArrayWritable).get())
				Case MathOp.Multiply
					For i As Integer = 1 To input.Length - 1
						[out].muli(DirectCast(input(i), NDArrayWritable).get())
					Next i
				Case MathOp.Divide
					[out].divi(DirectCast(input(1), NDArrayWritable).get())
				Case MathOp.ReverseSubtract
					[out].rsubi(DirectCast(input(1), NDArrayWritable).get())
				Case MathOp.ReverseDivide
					[out].rdivi(DirectCast(input(1), NDArrayWritable).get())
				Case MathOp.Modulus, ScalarMin, ScalarMax
					Throw New System.ArgumentException("Invalid MathOp: cannot use " & mathOp & " with NDArrayColumnsMathOpTransform")
				Case Else
					Throw New Exception("Unknown MathOp: " & mathOp)
			End Select

			'To avoid threading issues...
			Nd4j.Executioner.commit()

			Return New NDArrayWritable([out])
		End Function

		Public Overrides Function ToString() As String
			Return "NDArrayColumnsMathOpTransform(newColumnName=""" & newColumnName & """,mathOp=" & mathOp & ",columns=" & Arrays.toString(columns) & ")"
		End Function

		Public Overrides Function map(ByVal input As Object) As Object
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Throw New System.NotSupportedException("Not yet implemented")
		End Function
	End Class

End Namespace