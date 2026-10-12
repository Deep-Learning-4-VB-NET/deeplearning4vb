Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceComparator = org.datavec.api.transform.sequence.SequenceComparator
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties

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
'ORIGINAL LINE: @EqualsAndHashCode(exclude = {"schema", "columnIdx"}) @JsonIgnoreProperties({"schema", "columnIdx"}) @Data public abstract class BaseColumnComparator implements org.datavec.api.transform.sequence.SequenceComparator, org.datavec.api.transform.ColumnOp
	<Serializable>
	Public MustInherit Class BaseColumnComparator
		Implements SequenceComparator, ColumnOp

'JAVA TO VB CONVERTER NOTE: The field schema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend schema_Conflict As Schema

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly columnName_Conflict As String
		Protected Friend columnIdx As Integer = -1

		Protected Friend Sub New(ByVal columnName As String)
			Me.columnName_Conflict = columnName
		End Sub

		Public Overridable WriteOnly Property Schema Implements SequenceComparator.setSchema As Schema
			Set(ByVal sequenceSchema As Schema)
				Me.schema_Conflict = sequenceSchema
				Me.columnIdx = sequenceSchema.getIndexOfColumn(columnName_Conflict)
			End Set
		End Property

		''' <summary>
		''' Get the output schema for this transformation, given an input schema
		''' </summary>
		''' <param name="inputSchema"> </param>
		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Return inputSchema
		End Function

		''' <summary>
		''' Set the input schema.
		''' </summary>
		''' <param name="inputSchema"> </param>
		Public Overridable Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.schema_Conflict = inputSchema
			End Set
			Get
				Return schema_Conflict
			End Get
		End Property


		Public Overridable Function Compare(ByVal o1 As IList(Of Writable), ByVal o2 As IList(Of Writable)) As Integer
			Return Compare(get(o1, columnIdx), get(o2, columnIdx))
		End Function

		Private Shared Function get(ByVal c As IList(Of Writable), ByVal idx As Integer) As Writable
			Return c(idx)
		End Function

		Protected Friend MustOverride Function compare(ByVal w1 As Writable, ByVal w2 As Writable) As Integer

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
			Return columnName()
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
			Return columnNames()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
			Return New String() {columnName_Conflict}
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String Implements ColumnOp.columnName
			Return columnNames()(0)
		End Function
	End Class

End Namespace