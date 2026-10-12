Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
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

Namespace org.datavec.api.transform.transform.column


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class AddConstantColumnTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class AddConstantColumnTransform
		Implements Transform

		Private ReadOnly newColumnName As String
		Private ReadOnly newColumnType As ColumnType
		Private ReadOnly fixedValue As Writable

'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AddConstantColumnTransform(@JsonProperty("newColumnName") String newColumnName, @JsonProperty("newColumnType") org.datavec.api.transform.ColumnType newColumnType, @JsonProperty("fixedValue") org.datavec.api.writable.Writable fixedValue)
		Public Sub New(ByVal newColumnName As String, ByVal newColumnType As ColumnType, ByVal fixedValue As Writable)
			Me.newColumnName = newColumnName
			Me.newColumnType = newColumnType
			Me.fixedValue = fixedValue
		End Sub


		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Dim outMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.getColumnMetaData())

			Dim newColMeta As ColumnMetaData = newColumnType.newColumnMetaData(newColumnName)
			outMeta.Add(newColMeta)
			Return inputSchema.newSchema(outMeta)
		End Function

		Public Overridable Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function outputColumnName() As String
			Return newColumnName
		End Function

		Public Overridable Function outputColumnNames() As String()
			Return New String() {outputColumnName()}
		End Function

		Public Overridable Function columnNames() As String()
			Return New String(){}
		End Function

		Public Overridable Function columnName() As String
			Return newColumnName
		End Function

		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Dim [out] As IList(Of Writable) = New List(Of Writable)(writables.Count + 1)
			CType([out], List(Of Writable)).AddRange(writables)
			[out].Add(fixedValue)
			Return [out]
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			Dim outSeq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(sequence.Count)
			For Each l As IList(Of Writable) In sequence
				outSeq.Add(map(l))
			Next l
			Return outSeq
		End Function

		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace