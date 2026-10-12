Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
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

Namespace org.datavec.api.transform.metadata


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"stateNamesSet"}) @EqualsAndHashCode @Data public class CategoricalMetaData extends BaseColumnMetaData
	<Serializable>
	Public Class CategoricalMetaData
		Inherits BaseColumnMetaData

		Private ReadOnly stateNames As IList(Of String)
		Private ReadOnly stateNamesSet As ISet(Of String) 'For fast lookup

		Public Sub New(ByVal name As String, ParamArray ByVal stateNames() As String)
			Me.New(name, Arrays.asList(stateNames))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CategoricalMetaData(@JsonProperty("name") String name, @JsonProperty("stateNames") java.util.List<String> stateNames)
		Public Sub New(ByVal name As String, ByVal stateNames As IList(Of String))
			MyBase.New(name)
			Me.stateNames = stateNames
			stateNamesSet = New HashSet(Of String)(stateNames)
		End Sub

		Public Overrides ReadOnly Property ColumnType As ColumnType
			Get
				Return ColumnType.Categorical
			End Get
		End Property

		Public Overrides Function isValid(ByVal writable As Writable) As Boolean
			Return stateNamesSet.Contains(writable.ToString())
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
			Return stateNamesSet.Contains(input.ToString())
		End Function

		Public Overrides Function clone() As CategoricalMetaData
			Return New CategoricalMetaData(name_Conflict, stateNames)
		End Function

		Public Overridable ReadOnly Property StateNames As IList(Of String)
			Get
				Return stateNames
			End Get
		End Property

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("CategoricalMetaData(name=""").Append(name_Conflict).Append(""",stateNames=[")
			Dim first As Boolean = True
			For Each s As String In stateNamesSet
				If Not first Then
					sb.Append(",")
				End If
				sb.Append("""").Append(s).Append("""")
				first = False
			Next s
			sb.Append("])")
			Return sb.ToString()
		End Function
	End Class

End Namespace