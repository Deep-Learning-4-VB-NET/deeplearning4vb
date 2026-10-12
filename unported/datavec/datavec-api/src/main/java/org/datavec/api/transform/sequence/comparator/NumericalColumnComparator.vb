Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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

Namespace org.datavec.api.transform.sequence.comparator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"columnType", "schema", "columnIdx"}) @EqualsAndHashCode(callSuper = true, exclude = {"columnType"}) @Data public class NumericalColumnComparator extends BaseColumnComparator
	<Serializable>
	Public Class NumericalColumnComparator
		Inherits BaseColumnComparator

		Private columnType As ColumnType
		Private ascending As Boolean

		Public Sub New(ByVal columnName As String)
			Me.New(columnName, True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NumericalColumnComparator(@JsonProperty("columnName") String columnName, @JsonProperty("ascending") boolean ascending)
		Public Sub New(ByVal columnName As String, ByVal ascending As Boolean)
			MyBase.New(columnName)
			Me.ascending = ascending
		End Sub

		Public Overrides WriteOnly Property Schema As Schema
			Set(ByVal sequenceSchema As Schema)
				MyBase.Schema = sequenceSchema
				Me.columnType = sequenceSchema.getType(Me.columnIdx)
				Select Case columnType.innerEnumValue
					Case Integer?, System.Nullable<Long>, System.Nullable<Double>, Time
						'All ok. Time column uses LongWritables too...
					Case Else
						Throw New System.InvalidOperationException("Cannot apply numerical column comparator on column of type " & columnType)
				End Select
			End Set
		End Property

		Protected Friend Overrides Function Compare(ByVal w1 As Writable, ByVal w2 As Writable) As Integer
'JAVA TO VB CONVERTER NOTE: The local variable compare was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim compare_Conflict As Integer
			Select Case columnType.innerEnumValue
				Case Integer?
					compare_Conflict = Integer.compare(w1.toInt(), w2.toInt())
				Case ColumnType.InnerEnum.Time, System.Nullable<Long>
					compare_Conflict = Long.compare(w1.toLong(), w2.toLong())
				Case Double?
					compare_Conflict = w1.toDouble().CompareTo(w2.toDouble())
				Case Else
					'Should never happen...
					Throw New Exception("Cannot apply numerical column comparator on column of type " & columnType)
			End Select

			If ascending Then
				Return compare_Conflict
			End If
			Return -compare_Conflict
		End Function

		Public Overrides Function ToString() As String
			Return "NumericalColumnComparator(columnName=""" & columnName_Conflict & """,ascending=" & ascending & ")"
		End Function
	End Class

End Namespace