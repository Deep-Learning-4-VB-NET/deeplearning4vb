Imports System
Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Row = org.apache.spark.sql.Row
Imports GenericRowWithSchema = org.apache.spark.sql.catalyst.expressions.GenericRowWithSchema
Imports StructType = org.apache.spark.sql.types.StructType
Imports Schema = org.datavec.api.transform.schema.Schema
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DataFrames = org.datavec.spark.transform.DataFrames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.datavec.spark.transform.sparkfunction



	Public Class ToRow
		Implements [Function](Of IList(Of Writable), Row)

		Private schema As Schema
		Private structType As StructType

		Public Sub New(ByVal schema As Schema)
			Me.schema = schema
			structType = DataFrames.fromSchema(schema)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.apache.spark.sql.Row call(java.util.List<org.datavec.api.writable.Writable> v1) throws Exception
		Public Overrides Function [call](ByVal v1 As IList(Of Writable)) As Row
			If v1.Count = 1 AndAlso TypeOf v1(0) Is NDArrayWritable Then
				Dim writable As NDArrayWritable = DirectCast(v1(0), NDArrayWritable)
				Dim arr As INDArray = writable.get()
				If arr.columns() <> schema.numColumns() Then
					Throw New System.InvalidOperationException("Illegal record of size " & v1 & ". Should have been " & schema.numColumns())
				End If
				Dim values(arr.columns() - 1) As Object
				For i As Integer = 0 To values.Length - 1
					Select Case schema.getColumnTypes()(i)
						Case Double?
							values(i) = arr.getDouble(i)
						Case Integer?
							values(i) = CInt(Math.Truncate(arr.getDouble(i)))
						Case Long?
							values(i) = CLng(Math.Truncate(arr.getDouble(i)))
						Case Single?
							values(i) = CSng(arr.getDouble(i))
						Case Else
							Throw New System.InvalidOperationException("This api should not be used with strings , binary data or ndarrays. This is only for columnar data")
					End Select
				Next i

				Dim row As Row = New GenericRowWithSchema(values, structType)
				Return row
			Else
				If v1.Count <> schema.numColumns() Then
					Throw New System.InvalidOperationException("Illegal record of size " & v1 & ". Should have been " & schema.numColumns())
				End If
				Dim values(v1.Count - 1) As Object
				For i As Integer = 0 To values.Length - 1
					Select Case schema.getColumnTypes()(i)
						Case Double?
							values(i) = v1(i).toDouble()
						Case Integer?
							values(i) = v1(i).toInt()
						Case Long?
							values(i) = v1(i).toLong()
						Case Single?
							values(i) = v1(i).toFloat()
						Case Else
							Throw New System.InvalidOperationException("This api should not be used with strings , binary data or ndarrays. This is only for columnar data")
					End Select
				Next i

				Dim row As Row = New GenericRowWithSchema(values, structType)
				Return row
			End If

		End Function
	End Class

End Namespace