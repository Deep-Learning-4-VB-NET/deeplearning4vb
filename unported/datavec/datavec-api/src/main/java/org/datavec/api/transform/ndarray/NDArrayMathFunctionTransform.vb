Imports System
Imports Data = lombok.Data
Imports MathFunction = org.datavec.api.transform.MathFunction
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
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
'ORIGINAL LINE: @Data public class NDArrayMathFunctionTransform extends org.datavec.api.transform.transform.BaseColumnTransform
	<Serializable>
	Public Class NDArrayMathFunctionTransform
		Inherits BaseColumnTransform

		'Can't guarantee that the writable won't be re-used, for example in different Spark ops on the same RDD
		Private Const DUP As Boolean = True

		Private ReadOnly mathFunction As MathFunction

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayMathFunctionTransform(@JsonProperty("columnName") String columnName, @JsonProperty("mathFunction") org.datavec.api.transform.MathFunction mathFunction)
		Public Sub New(ByVal columnName As String, ByVal mathFunction As MathFunction)
			MyBase.New(columnName)
			Me.mathFunction = mathFunction
		End Sub

		Public Overrides Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			Dim m As ColumnMetaData = oldColumnType.clone()
			m.Name = newName
			Return m
		End Function

		Public Overrides Function map(ByVal w As Writable) As NDArrayWritable
			Dim n As NDArrayWritable = DirectCast(w, NDArrayWritable)
			Dim i As INDArray = n.get()
			If i Is Nothing Then
				Return n
			End If

			Dim o As NDArrayWritable
			Select Case mathFunction
				Case MathFunction.ABS
					o = New NDArrayWritable(Transforms.abs(i, DUP))
				Case MathFunction.ACOS
					o = New NDArrayWritable(Transforms.acos(i, DUP))
				Case MathFunction.ASIN
					o = New NDArrayWritable(Transforms.asin(i, DUP))
				Case MathFunction.ATAN
					o = New NDArrayWritable(Transforms.atan(i, DUP))
				Case MathFunction.CEIL
					o = New NDArrayWritable(Transforms.ceil(i, DUP))
				Case MathFunction.COS
					o = New NDArrayWritable(Transforms.cos(i, DUP))
				Case MathFunction.COSH
					'No cosh operation in ND4J
					Throw New System.NotSupportedException("sinh operation not yet supported for NDArray columns")
				Case MathFunction.EXP
					o = New NDArrayWritable(Transforms.exp(i, DUP))
				Case MathFunction.FLOOR
					o = New NDArrayWritable(Transforms.floor(i, DUP))
				Case MathFunction.LOG
					o = New NDArrayWritable(Transforms.log(i, DUP))
				Case MathFunction.LOG10
					o = New NDArrayWritable(Transforms.log(i, 10.0, DUP))
				Case MathFunction.SIGNUM
					o = New NDArrayWritable(Transforms.sign(i, DUP))
				Case MathFunction.SIN
					o = New NDArrayWritable(Transforms.sin(i, DUP))
				Case MathFunction.SINH
					'No sinh op in ND4J
					Throw New System.NotSupportedException("sinh operation not yet supported for NDArray columns")
				Case MathFunction.SQRT
					o = New NDArrayWritable(Transforms.sqrt(i, DUP))
				Case MathFunction.TAN
					'No tan op in ND4J yet - but tan(x) = sin(x)/cos(x)
					Dim sinx As INDArray = Transforms.sin(i, True)
					Dim cosx As INDArray = Transforms.cos(i, True)
					o = New NDArrayWritable(sinx.divi(cosx))
				Case MathFunction.TANH
					o = New NDArrayWritable(Transforms.tanh(i, DUP))
				Case Else
					Throw New Exception("Unknown function: " & mathFunction)
			End Select

			'To avoid threading issues...
			Nd4j.Executioner.commit()

			Return o

		End Function

		Public Overrides Function ToString() As String
			Return "NDArrayMathFunctionTransform(column=" & columnName_Conflict & ",function=" & mathFunction & ")"
		End Function

		Public Overridable Overloads Function map(ByVal input As Object) As Object
			If TypeOf input Is NDArrayWritable Then
				Return map(DirectCast(input, NDArrayWritable))
			ElseIf TypeOf input Is INDArray Then
				Return map(New NDArrayWritable(DirectCast(input, INDArray))).get()
			Else
				Throw New System.NotSupportedException("Unknown object type: " & (If(input Is Nothing, Nothing, input.GetType())))
			End If
		End Function
	End Class

End Namespace