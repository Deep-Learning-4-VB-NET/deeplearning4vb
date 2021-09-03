Imports System
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
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

Namespace org.datavec.api.transform.metadata

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class BooleanMetaData extends BaseColumnMetaData
	<Serializable>
	Public Class BooleanMetaData
		Inherits BaseColumnMetaData


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BooleanMetaData(@JsonProperty("name") String name)
		Public Sub New(ByVal name As String)
			MyBase.New(name)
		End Sub



		Public Overrides ReadOnly Property ColumnType As ColumnType
			Get
				Return ColumnType.Boolean
			End Get
		End Property

		Public Overrides Function isValid(ByVal writable As Writable) As Boolean
			Dim value As Boolean
			Try
				value = Boolean.Parse(writable.ToString())
			Catch e As System.FormatException
				Return False
			End Try


			Return True
		End Function

		''' <summary>
		''' Is the given object valid for this column,
		''' given the column type and any
		''' restrictions given by the
		''' ColumnMetaData object?
		''' </summary>
		''' <param name="input"> object to check </param>
		''' <returns> true if value, false if invalid </returns>
		Public Overrides Function isValid(ByVal input As Object) As Boolean
			Dim value As Boolean
			Try
				value = Boolean.Parse(input.ToString())
			Catch e As System.FormatException
				Return False
			End Try


			Return True
		End Function

		Public Overrides Function clone() As BooleanMetaData
			Return New BooleanMetaData(name_Conflict)
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("BooleanMetaData(name=""").Append(name_Conflict).Append(""",")
			sb.Append(")")
			Return sb.ToString()
		End Function
	End Class

End Namespace