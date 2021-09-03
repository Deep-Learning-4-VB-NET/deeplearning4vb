Imports System
Imports Data = lombok.Data
Imports MathOp = org.datavec.api.transform.MathOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports NDArrayMetaData = org.datavec.api.transform.metadata.NDArrayMetaData
Imports BaseColumnTransform = org.datavec.api.transform.transform.BaseColumnTransform
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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
'ORIGINAL LINE: @Data public class NDArrayScalarOpTransform extends org.datavec.api.transform.transform.BaseColumnTransform
	<Serializable>
	Public Class NDArrayScalarOpTransform
		Inherits BaseColumnTransform

		Private ReadOnly mathOp As MathOp
		Private ReadOnly scalar As Double

		''' 
		''' <param name="columnName"> Name of the column to perform the operation on </param>
		''' <param name="mathOp">     Operation to perform </param>
		''' <param name="scalar">     Scalar value for the operation </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayScalarOpTransform(@JsonProperty("columnName") String columnName, @JsonProperty("mathOp") org.datavec.api.transform.MathOp mathOp, @JsonProperty("scalar") double scalar)
		Public Sub New(ByVal columnName As String, ByVal mathOp As MathOp, ByVal scalar As Double)
			MyBase.New(columnName)
			Me.mathOp = mathOp
			Me.scalar = scalar
		End Sub

		Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			If Not (TypeOf oldColumnType Is NDArrayMetaData) Then
				Throw New System.InvalidOperationException("Column " & newName & " is not a NDArray column")
			End If

			Dim oldMeta As NDArrayMetaData = DirectCast(oldColumnType, NDArrayMetaData)
			Dim newMeta As NDArrayMetaData = oldMeta.clone()
			newMeta.Name = newName

			Return newMeta
		End Function

		Public Overrides Function map(ByVal w As Writable) As NDArrayWritable
			If Not (TypeOf w Is NDArrayWritable) Then
				Throw New System.ArgumentException("Input writable is not an NDArrayWritable: is " & w.GetType())
			End If

			'Make a copy - can't always assume that the original INDArray won't be used again in the future
			Dim n As NDArrayWritable = (DirectCast(w, NDArrayWritable))
			Dim a As INDArray = n.get().dup()
			Select Case mathOp
				Case MathOp.Add
					a.addi(scalar)
				Case MathOp.Subtract
					a.subi(scalar)
				Case MathOp.Multiply
					a.muli(scalar)
				Case MathOp.Divide
					a.divi(scalar)
				Case MathOp.Modulus
					a.fmodi(scalar)
				Case MathOp.ReverseSubtract
					a.rsubi(scalar)
				Case MathOp.ReverseDivide
					a.rdivi(scalar)
				Case MathOp.ScalarMin
					Transforms.min(a, scalar, False)
				Case MathOp.ScalarMax
					Transforms.max(a, scalar, False)
				Case Else
					Throw New System.NotSupportedException("Unknown or not supported op: " & mathOp)
			End Select

			'To avoid threading issues...
			Nd4j.Executioner.commit()

			Return New NDArrayWritable(a)
		End Function

		Public Overrides Function ToString() As String
			Return "NDArrayScalarOpTransform(mathOp=" & mathOp & ",scalar=" & scalar & ")"
		End Function

		Public Overridable Overloads Function map(ByVal input As Object) As Object
			If TypeOf input Is INDArray Then
				Return map(New NDArrayWritable(DirectCast(input, INDArray))).get()
			End If
			Throw New Exception("Unsupported class: " & input.GetType())
		End Function
	End Class

End Namespace