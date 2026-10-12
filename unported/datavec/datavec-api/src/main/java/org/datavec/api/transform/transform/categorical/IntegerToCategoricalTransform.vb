Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports BaseColumnTransform = org.datavec.api.transform.transform.BaseColumnTransform
Imports Text = org.datavec.api.writable.Text
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

Namespace org.datavec.api.transform.transform.categorical


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "columnNumber"}) @Data public class IntegerToCategoricalTransform extends org.datavec.api.transform.transform.BaseColumnTransform
	<Serializable>
	Public Class IntegerToCategoricalTransform
		Inherits BaseColumnTransform

'JAVA TO VB CONVERTER NOTE: The field map was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly map_Conflict As IDictionary(Of Integer, String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public IntegerToCategoricalTransform(@JsonProperty("columnName") String columnName, @JsonProperty("map") Map<Integer, String> map)
		Public Sub New(ByVal columnName As String, ByVal map As IDictionary(Of Integer, String))
			MyBase.New(columnName)
			Me.map_Conflict = map
		End Sub

		Public Sub New(ByVal columnName As String, ByVal list As IList(Of String))
			MyBase.New(columnName)
			Me.map_Conflict = New LinkedHashMap(Of Integer, String)()
			Dim i As Integer = 0
			For Each s As String In list
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: map.put(i++, s);
				map(i) = s
					i += 1
			Next s
		End Sub

		Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
			Return New CategoricalMetaData(newColumnName, New List(Of )(map_Conflict.Values))
		End Function

		Public Overrides Function map(ByVal columnWritable As Writable) As Writable
			Return New Text(map(columnWritable.toInt()))
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("IntegerToCategoricalTransform(map=[")
			Dim list As IList(Of Integer) = New List(Of Integer)(map_Conflict.Keys)
			list.Sort()
			Dim first As Boolean = True
			For Each i As Integer? In list
				If Not first Then
					sb.Append(",")
				End If
				sb.Append(i).Append("=""").Append(map(i)).Append("""")
				first = False
			Next i
			sb.Append("])")
			Return sb.ToString()
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If
			If Not MyBase.Equals(o) Then
				Return False
			End If

			Dim o2 As IntegerToCategoricalTransform = DirectCast(o, IntegerToCategoricalTransform)

			Return If(map_Conflict IsNot Nothing, map_Conflict.Equals(o2.map_Conflict), o2.map_Conflict Is Nothing)

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = MyBase.GetHashCode()
			result = 31 * result + (If(map_Conflict IsNot Nothing, map_Conflict.GetHashCode(), 0))
			Return result
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Overloads Function map(ByVal input As Object) As Object
			Return New Text(map(input.ToString()))
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: List<?> values = (List<?>) sequence;
			Dim values As IList(Of Object) = DirectCast(sequence, IList(Of Object))
			Dim ret As IList(Of IList(Of Integer)) = New List(Of IList(Of Integer))()
			For Each obj As Object In values
				ret.Add(DirectCast(map(obj), IList(Of Integer)))
			Next obj
			Return ret
		End Function
	End Class

End Namespace