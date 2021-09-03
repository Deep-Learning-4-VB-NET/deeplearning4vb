Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Row = org.apache.spark.sql.Row
Imports Schema = org.datavec.api.transform.schema.Schema
Imports org.datavec.api.writable

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class ToRecord implements org.apache.spark.api.java.function.@Function<org.apache.spark.sql.Row, java.util.List<Writable>>
	Public Class ToRecord
		Implements [Function](Of Row, IList(Of Writable))

		Private schema As Schema

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<Writable> call(org.apache.spark.sql.Row v1) throws Exception
		Public Overrides Function [call](ByVal v1 As Row) As IList(Of Writable)
			Dim ret As IList(Of Writable) = New List(Of Writable)()
			If v1.size() <> schema.numColumns() Then
				Throw New System.ArgumentException("Invalid number of columns for row " & v1.size() & " should have matched schema columns " & schema.numColumns())
			End If
			For i As Integer = 0 To v1.size() - 1
				If v1.get(i) Is Nothing Then
					Throw New System.InvalidOperationException("Row item " & i & " is null")
				End If
				Select Case schema.getType(i)
					Case Double?
						ret.Add(New DoubleWritable(v1.getDouble(i)))
					Case Single?
						ret.Add(New FloatWritable(v1.getFloat(i)))
					Case Integer?
						ret.Add(New IntWritable(v1.getInt(i)))
					Case Long?
						ret.Add(New LongWritable(v1.getLong(i)))
					Case Else
						Throw New System.InvalidOperationException("Illegal type")
				End Select

			Next i
			Return ret
		End Function
	End Class

End Namespace