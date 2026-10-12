Imports System
Imports System.Collections.Generic
Imports Schema = org.datavec.api.transform.schema.Schema
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

Namespace org.datavec.api.transform.filter


	<Serializable>
	Public MustInherit Class BaseColumnFilter
		Implements Filter

		Public MustOverride Function columnName() As String
		Public MustOverride Function columnNames() As String()
		Public MustOverride Function outputColumnNames() As String()
		Public MustOverride Function outputColumnName() As String
		Public MustOverride ReadOnly Property InputSchema As Schema Implements Filter.getInputSchema
		Public MustOverride Function removeSequence(ByVal sequence As Object) As Boolean Implements Filter.removeSequence
		Public MustOverride Function removeExample(ByVal example As Object) As Boolean Implements Filter.removeExample

		Protected Friend schema As Schema
		Protected Friend ReadOnly column As String
		Protected Friend columnIdx As Integer

		Protected Friend Sub New(ByVal column As String)
			Me.column = column
		End Sub

		Public Overridable Function removeExample(ByVal writables As IList(Of Writable)) As Boolean Implements Filter.removeExample
			Return removeExample(writables(columnIdx))
		End Function

		Public Overridable Function removeSequence(ByVal sequence As IList(Of IList(Of Writable))) As Boolean Implements Filter.removeSequence
			For Each c As IList(Of Writable) In sequence
				If removeExample(c) Then
					Return True
				End If
			Next c
			Return False
		End Function

		Public Overridable WriteOnly Property InputSchema Implements Filter.setInputSchema As Schema
			Set(ByVal schema As Schema)
				Me.schema = schema
				Me.columnIdx = schema.getIndexOfColumn(column)
			End Set
		End Property

		''' <summary>
		''' Should the example or sequence be removed, based on the values from the specified column? </summary>
		Public MustOverride Function removeExample(ByVal writable As Writable) As Boolean
	End Class

End Namespace