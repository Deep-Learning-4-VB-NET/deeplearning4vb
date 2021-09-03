Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports org.datavec.api.writable
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

Namespace org.datavec.api.transform.schema


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class SequenceSchema extends Schema
	<Serializable>
	Public Class SequenceSchema
		Inherits Schema

		Private ReadOnly minSequenceLength As Integer?
		Private ReadOnly maxSequenceLength As Integer?

		Public Sub New(ByVal columnMetaData As IList(Of ColumnMetaData))
			Me.New(columnMetaData, Nothing, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceSchema(@JsonProperty("columns") java.util.List<org.datavec.api.transform.metadata.ColumnMetaData> columnMetaData, @JsonProperty("minSequenceLength") System.Nullable<Integer> minSequenceLength, @JsonProperty("maxSequenceLength") System.Nullable<Integer> maxSequenceLength)
		Public Sub New(ByVal columnMetaData As IList(Of ColumnMetaData), ByVal minSequenceLength As Integer?, ByVal maxSequenceLength As Integer?)
			MyBase.New(columnMetaData)
			Me.minSequenceLength = minSequenceLength
			Me.maxSequenceLength = maxSequenceLength
		End Sub

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.minSequenceLength = builder.minSequenceLength_Conflict
			Me.maxSequenceLength = builder.maxSequenceLength_Conflict
		End Sub

		Public Overrides Function newSchema(ByVal columnMetaData As IList(Of ColumnMetaData)) As SequenceSchema
			Return New SequenceSchema(columnMetaData, minSequenceLength, maxSequenceLength)
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			Dim nCol As Integer = numColumns()

			Dim maxNameLength As Integer = 0
			For Each s As String In getColumnNames()
				maxNameLength = Math.Max(maxNameLength, s.Length)
			Next s

			'Header:
			sb.Append("SequenceSchema(")

			If minSequenceLength IsNot Nothing Then
				sb.Append("minSequenceLength=").Append(minSequenceLength)
			End If
			If maxSequenceLength IsNot Nothing Then
				If minSequenceLength IsNot Nothing Then
					sb.Append(",")
				End If
				sb.Append("maxSequenceLength=").Append(maxSequenceLength)
			End If

			sb.Append(")" & vbLf)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			sb.Append(String.Format("{0,-6}", "idx")).Append(String.Format("%-" & (maxNameLength + 8) & "s", "name")).Append(String.Format("{0,-15}", "type")).Append("meta data").Append(vbLf)

			For i As Integer = 0 To nCol - 1
				Dim colName As String = getName(i)
				Dim type As ColumnType = [getType](i)
				Dim meta As ColumnMetaData = getMetaData(i)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Dim paddedName As String = String.Format("%-" & (maxNameLength + 8) & "s", """" & colName & """")
				sb.Append(String.Format("{0,-6:D}", i)).Append(paddedName).Append(String.Format("{0,-15}", type)).Append(meta).Append(vbLf)
			Next i

			Return sb.ToString()
		End Function

		Public Class Builder
			Inherits Schema.Builder

'JAVA TO VB CONVERTER NOTE: The field minSequenceLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minSequenceLength_Conflict As Integer?
'JAVA TO VB CONVERTER NOTE: The field maxSequenceLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend maxSequenceLength_Conflict As Integer?

'JAVA TO VB CONVERTER NOTE: The parameter minSequenceLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minSequenceLength(ByVal minSequenceLength_Conflict As Integer) As Builder
				Me.minSequenceLength_Conflict = minSequenceLength_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter maxSequenceLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maxSequenceLength(ByVal maxSequenceLength_Conflict As Integer) As Builder
				Me.maxSequenceLength_Conflict = maxSequenceLength_Conflict
				Return Me
			End Function


			Public Overrides Function build() As SequenceSchema
				Return New SequenceSchema(Me)
			End Function


		End Class


		''' <summary>
		''' Infers a sequence schema based
		''' on the record </summary>
		''' <param name="record"> the record to infer the schema based on </param>
		''' <returns> the inferred sequence schema
		'''  </returns>
		Public Shared Function inferSequenceMulti(ByVal record As IList(Of IList(Of IList(Of Writable)))) As SequenceSchema
			Dim builder As New SequenceSchema.Builder()
			Dim minSequenceLength As Integer = record(0).Count
			Dim maxSequenceLength As Integer = record(0).Count
			For i As Integer = 0 To record.Count - 1
				If TypeOf record(i) Is DoubleWritable Then
					builder.addColumnDouble(i.ToString())
				ElseIf TypeOf record(i) Is IntWritable Then
					builder.addColumnInteger(i.ToString())
				ElseIf TypeOf record(i) Is LongWritable Then
					builder.addColumnLong(i.ToString())
				ElseIf TypeOf record(i) Is FloatWritable Then
					builder.addColumnFloat(i.ToString())

				Else
					Throw New System.InvalidOperationException("Illegal writable for inferring schema of type " & record(i).GetType().ToString() & " with record " & record(0))
				End If
				builder.minSequenceLength(Math.Min(record(i).Count, minSequenceLength))
				builder.maxSequenceLength(Math.Max(record(i).Count, maxSequenceLength))
			Next i


			Return builder.build()
		End Function

		''' <summary>
		''' Infers a sequence schema based
		''' on the record </summary>
		''' <param name="record"> the record to infer the schema based on </param>
		''' <returns> the inferred sequence schema
		'''  </returns>
		Public Shared Function inferSequence(ByVal record As IList(Of IList(Of Writable))) As SequenceSchema
			Dim builder As New SequenceSchema.Builder()
			For i As Integer = 0 To record.Count - 1
				If TypeOf record(i) Is DoubleWritable Then
					builder.addColumnDouble(i.ToString())
				ElseIf TypeOf record(i) Is IntWritable Then
					builder.addColumnInteger(i.ToString())
				ElseIf TypeOf record(i) Is LongWritable Then
					builder.addColumnLong(i.ToString())
				ElseIf TypeOf record(i) Is FloatWritable Then
					builder.addColumnFloat(i.ToString())

				Else
					Throw New System.InvalidOperationException("Illegal writable for infering schema of type " & record(i).GetType().ToString())
				End If
			Next i

			builder.minSequenceLength(record.Count)
			builder.maxSequenceLength(record.Count)
			Return builder.build()
		End Function
	End Class

End Namespace