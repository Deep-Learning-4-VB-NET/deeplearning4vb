Imports System
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone
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
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class TimeMetaData extends BaseColumnMetaData
	<Serializable>
	Public Class TimeMetaData
		Inherits BaseColumnMetaData

		Private ReadOnly timeZone As DateTimeZone
		Private ReadOnly minValidTime As Long?
		Private ReadOnly maxValidTime As Long?

		''' <summary>
		''' Create a TimeMetaData column with no restrictions and UTC timezone.
		''' </summary>
		Public Sub New(ByVal name As String)
			Me.New(name, DateTimeZone.UTC, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Create a TimeMetaData column with no restriction on the allowable times
		''' </summary>
		''' <param name="timeZone"> Timezone for this column. Typically used for parsing </param>
		Public Sub New(ByVal name As String, ByVal timeZone As TimeZone)
			Me.New(name, timeZone, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Create a TimeMetaData column with no restriction on the allowable times
		''' </summary>
		''' <param name="timeZone"> Timezone for this column. </param>
		Public Sub New(ByVal name As String, ByVal timeZone As DateTimeZone)
			Me.New(name, timeZone, Nothing, Nothing)
		End Sub

		''' <param name="timeZone">     Timezone for this column. Typically used for parsing and some transforms </param>
		''' <param name="minValidTime"> Minimum valid time, in milliseconds (timestamp format). If null: no restriction </param>
		''' <param name="maxValidTime"> Maximum valid time, in milliseconds (timestamp format). If null: no restriction </param>
		Public Sub New(ByVal name As String, ByVal timeZone As TimeZone, ByVal minValidTime As Long?, ByVal maxValidTime As Long?)
			MyBase.New(name)
			Me.timeZone = DateTimeZone.forTimeZone(timeZone)
			Me.minValidTime = minValidTime
			Me.maxValidTime = maxValidTime
		End Sub

		''' <param name="timeZone">     Timezone for this column. Typically used for parsing and some transforms </param>
		''' <param name="minValidTime"> Minimum valid time, in milliseconds (timestamp format). If null: no restriction </param>
		''' <param name="maxValidTime"> Maximum valid time, in milliseconds (timestamp format). If null: no restriction </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TimeMetaData(@JsonProperty("name") String name, @JsonProperty("timeZone") org.joda.time.DateTimeZone timeZone, @JsonProperty("minValidTime") System.Nullable<Long> minValidTime, @JsonProperty("maxValidTime") System.Nullable<Long> maxValidTime)
		Public Sub New(ByVal name As String, ByVal timeZone As DateTimeZone, ByVal minValidTime As Long?, ByVal maxValidTime As Long?)
			MyBase.New(name)
			Me.timeZone = timeZone
			Me.minValidTime = minValidTime
			Me.maxValidTime = maxValidTime
		End Sub

		Public Overrides ReadOnly Property ColumnType As ColumnType
			Get
				Return ColumnType.Time
			End Get
		End Property

		Public Overrides Function isValid(ByVal writable As Writable) As Boolean
			Dim epochMillisec As Long

			If TypeOf writable Is LongWritable Then
				epochMillisec = writable.toLong()
			Else
				Try
					epochMillisec = Long.Parse(writable.ToString())
				Catch e As System.FormatException
					Return False
				End Try
			End If
			If minValidTime IsNot Nothing AndAlso epochMillisec < minValidTime Then
				Return False
			End If
			Return Not (maxValidTime IsNot Nothing AndAlso epochMillisec > maxValidTime)
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
			Dim epochMillisec As Long
			Try
				epochMillisec = Long.Parse(input.ToString())
			Catch e As System.FormatException
				Return False
			End Try

			If minValidTime IsNot Nothing AndAlso epochMillisec < minValidTime Then
				Return False
			End If
			Return Not (maxValidTime IsNot Nothing AndAlso epochMillisec > maxValidTime)
		End Function

		Public Overrides Function clone() As TimeMetaData
			Return New TimeMetaData(name_Conflict, timeZone, minValidTime, maxValidTime)
		End Function


		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("TimeMetaData(name=""").Append(name_Conflict).Append(""",timeZone=").Append(timeZone.getID())
			If minValidTime IsNot Nothing Then
				sb.Append("minValidTime=").Append(minValidTime)
			End If
			If maxValidTime IsNot Nothing Then
				If minValidTime IsNot Nothing Then
					sb.Append(",")
				End If
				sb.Append("maxValidTime=").Append(maxValidTime)
			End If
			sb.Append(")")
			Return sb.ToString()
		End Function
	End Class

End Namespace