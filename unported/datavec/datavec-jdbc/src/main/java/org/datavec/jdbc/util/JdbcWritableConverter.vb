Imports BooleanWritable = org.datavec.api.writable.BooleanWritable
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports FloatWritable = org.datavec.api.writable.FloatWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports NullWritable = org.datavec.api.writable.NullWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.jdbc.util


	Public Class JdbcWritableConverter

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.datavec.api.writable.Writable convert(final Object columnValue, final int columnType)
		Public Shared Function convert(ByVal columnValue As Object, ByVal columnType As Integer) As Writable
			If columnValue Is Nothing Then
				Return New NullWritable()
			End If

			Select Case columnType
				Case Types.BOOLEAN
					Return New BooleanWritable(DirectCast(columnValue, Boolean))

				Case Types.DATE, Types.TIME, Types.TIMESTAMP, Types.CHAR, Types.LONGVARCHAR, Types.LONGNVARCHAR, Types.NCHAR, Types.NVARCHAR, Types.VARCHAR
					Return New Text(columnValue.ToString())

				Case Types.FLOAT
					Return New FloatWritable(DirectCast(columnValue, Single))

				Case Types.REAL
					Return If(TypeOf columnValue Is Single?, New FloatWritable(DirectCast(columnValue, Single)), New DoubleWritable(DirectCast(columnValue, Double)))

				Case Types.DECIMAL, Types.NUMERIC
					Return New DoubleWritable(DirectCast(columnValue, Decimal).doubleValue()) '!\ This may overflow

				Case Types.DOUBLE
					Return New DoubleWritable(DirectCast(columnValue, Double))

				Case Types.INTEGER, Types.SMALLINT, Types.TINYINT
					Return New IntWritable(DirectCast(columnValue, Integer))

				Case Types.BIT
					Return New BooleanWritable(DirectCast(columnValue, Boolean))

				Case Types.BIGINT
					Return New LongWritable(DirectCast(columnValue, Long))

				Case Else
					Throw New System.ArgumentException("Column type unknown")
			End Select
		End Function

		Private Sub New()
		End Sub
	End Class

End Namespace