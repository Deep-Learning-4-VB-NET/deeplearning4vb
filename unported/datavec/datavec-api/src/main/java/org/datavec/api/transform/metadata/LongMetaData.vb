Imports System
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports LongWritable = org.datavec.api.writable.LongWritable
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class LongMetaData extends BaseColumnMetaData
	<Serializable>
	Public Class LongMetaData
		Inherits BaseColumnMetaData

		'minAllowedValue/maxAllowedValue are nullable: null -> no restriction on minAllowedValue/maxAllowedValue values
		Private ReadOnly minAllowedValue As Long?
		Private ReadOnly maxAllowedValue As Long?

		Public Sub New(ByVal name As String)
			Me.New(name, Nothing, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LongMetaData(@JsonProperty("name") String name, @JsonProperty("minAllowedValue") System.Nullable<Long> minAllowedValue, @JsonProperty("maxAllowedValue") System.Nullable<Long> maxAllowedValue)
		Public Sub New(ByVal name As String, ByVal minAllowedValue As Long?, ByVal maxAllowedValue As Long?)
			MyBase.New(name)
			Me.minAllowedValue = minAllowedValue
			Me.maxAllowedValue = maxAllowedValue
		End Sub

		Public Overrides ReadOnly Property ColumnType As ColumnType
			Get
				Return ColumnType.Long
			End Get
		End Property

		Public Overrides Function isValid(ByVal writable As Writable) As Boolean
			Dim value As Long
			If TypeOf writable Is IntWritable OrElse TypeOf writable Is LongWritable Then
				value = writable.toLong()
			Else
				Try
					value = Long.Parse(writable.ToString())
				Catch e As System.FormatException
					Return False
				End Try
			End If
			If minAllowedValue IsNot Nothing AndAlso value < minAllowedValue Then
				Return False
			End If
			If maxAllowedValue IsNot Nothing AndAlso value > maxAllowedValue Then
				Return False
			End If

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
			Dim value As Long
			Try
				value = Long.Parse(input.ToString())
			Catch e As System.FormatException
				Return False
			End Try

			If minAllowedValue IsNot Nothing AndAlso value < minAllowedValue Then
				Return False
			End If
			If maxAllowedValue IsNot Nothing AndAlso value > maxAllowedValue Then
				Return False
			End If

			Return True
		End Function

		Public Overrides Function clone() As LongMetaData
			Return New LongMetaData(name_Conflict, minAllowedValue, maxAllowedValue)
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("LongMetaData(name=""").Append(name_Conflict).Append(""",")
			If minAllowedValue IsNot Nothing Then
				sb.Append("minAllowed=").Append(minAllowedValue)
			End If
			If maxAllowedValue IsNot Nothing Then
				If minAllowedValue IsNot Nothing Then
					sb.Append(",")
				End If
				sb.Append("maxAllowed=").Append(maxAllowedValue)
			End If
			sb.Append(")")
			Return sb.ToString()
		End Function
	End Class

End Namespace