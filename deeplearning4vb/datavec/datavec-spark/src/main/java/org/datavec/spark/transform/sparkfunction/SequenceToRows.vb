Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Row = org.apache.spark.sql.Row
Imports GenericRowWithSchema = org.apache.spark.sql.catalyst.expressions.GenericRowWithSchema
Imports StructType = org.apache.spark.sql.types.StructType
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports DataFrames = org.datavec.spark.transform.DataFrames

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


	Public Class SequenceToRows
		Implements FlatMapFunction(Of IList(Of IList(Of Writable)), Row)

		Private schema As Schema
		Private structType As StructType

		Public Sub New(ByVal schema As Schema)
			Me.schema = schema
			structType = DataFrames.fromSchemaSequence(schema)
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Iterator<org.apache.spark.sql.Row> call(List<List<org.datavec.api.writable.Writable>> sequence) throws Exception
		Public Overrides Function [call](ByVal sequence As IList(Of IList(Of Writable))) As IEnumerator(Of Row)
			If sequence.Count = 0 Then
				Return Collections.emptyIterator()
			End If

			Dim sequenceUUID As String = System.Guid.randomUUID().ToString()

			Dim [out] As IList(Of Row) = New List(Of Row)(sequence.Count)

			Dim stepCount As Integer = 0
			For Each [step] As IList(Of Writable) In sequence
				Dim values([step].Count + 1) As Object
				values(0) = sequenceUUID
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: values[1] = stepCount++;
				values(1) = stepCount
					stepCount += 1
				For i As Integer = 0 To [step].Count - 1
					Select Case schema.getColumnTypes()(i)
						Case Double?
							values(i + 2) = [step](i).toDouble()
						Case Integer?
							values(i + 2) = [step](i).toInt()
						Case Long?
							values(i + 2) = [step](i).toLong()
						Case Single?
							values(i + 2) = [step](i).toFloat()
						Case Else
							Throw New System.InvalidOperationException("This api should not be used with strings , binary data or ndarrays. This is only for columnar data")
					End Select
				Next i

				Dim row As Row = New GenericRowWithSchema(values, structType)
				[out].Add(row)
			Next [step]

			Return [out].GetEnumerator()
		End Function
	End Class

End Namespace