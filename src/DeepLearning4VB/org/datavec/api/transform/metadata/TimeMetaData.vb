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

Imports System
Imports System.Text
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone

Namespace org.datavec.api.transform.metadata

    ''' <summary>
    ''' Metadata for a time column: values are epoch milliseconds (longs) in the given time zone, with optional
    ''' min/max restrictions. <c>java.util.TimeZone</c> is <see cref="TimeZoneInfo"/>.
    ''' </summary>
    <Serializable>
    Public Class TimeMetaData
        Inherits BaseColumnMetaData

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
        Public Sub New(ByVal name As String, ByVal timeZone As TimeZoneInfo)
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
        Public Sub New(ByVal name As String, ByVal timeZone As TimeZoneInfo, ByVal minValidTime As Long?, ByVal maxValidTime As Long?)
            Me.New(name, DateTimeZone.forTimeZone(timeZone), minValidTime, maxValidTime)
        End Sub

        ''' <param name="timeZone">     Timezone for this column. Typically used for parsing and some transforms </param>
        ''' <param name="minValidTime"> Minimum valid time, in milliseconds (timestamp format). If null: no restriction </param>
        ''' <param name="maxValidTime"> Maximum valid time, in milliseconds (timestamp format). If null: no restriction </param>
        <JsonConstructor>
        Public Sub New(ByVal name As String, ByVal timeZone As DateTimeZone, ByVal minValidTime As Long?, ByVal maxValidTime As Long?)
            MyBase.New(name)
            Me.TimeZone = timeZone
            Me.MinValidTime = minValidTime
            Me.MaxValidTime = maxValidTime
        End Sub

        Public ReadOnly Property TimeZone As DateTimeZone
        Public ReadOnly Property MinValidTime As Long?
        Public ReadOnly Property MaxValidTime As Long?

        Public Function getTimeZone() As DateTimeZone
            Return TimeZone
        End Function

        Public Function getMinValidTime() As Long?
            Return MinValidTime
        End Function

        Public Function getMaxValidTime() As Long?
            Return MaxValidTime
        End Function

        Public Overrides ReadOnly Property ColumnType As ColumnTypeEnum
            Get
                Return ColumnTypeEnum.Time
            End Get
        End Property

        Public Overrides Function isValid(ByVal writable As writable.Writable) As Boolean
            Dim epochMillisec As Long

            If TypeOf writable Is LongWritable Then
                epochMillisec = writable.toLong()
            ElseIf Not JavaNumbers.tryParseLong(writable.ToString(), epochMillisec) Then
                Return False
            End If
            Return check(epochMillisec)
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
            If Not JavaNumbers.tryParseLong(input.ToString(), epochMillisec) Then
                Return False
            End If
            Return check(epochMillisec)
        End Function

        Private Function check(ByVal epochMillisec As Long) As Boolean
            If MinValidTime.HasValue AndAlso epochMillisec < MinValidTime.Value Then
                Return False
            End If
            Return Not (MaxValidTime.HasValue AndAlso epochMillisec > MaxValidTime.Value)
        End Function

        Public Overrides Function clone() As ColumnMetaData
            Return New TimeMetaData(Name, TimeZone, MinValidTime, MaxValidTime)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other = DirectCast(obj, TimeMetaData)
            Return Objects.equals(TimeZone, other.TimeZone) AndAlso Nullable.Equals(MinValidTime, other.MinValidTime) AndAlso Nullable.Equals(MaxValidTime, other.MaxValidTime)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(Name, TimeZone, MinValidTime, MaxValidTime)
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("TimeMetaData(name=""").Append(Name).Append(""",timeZone=").Append(TimeZone.getID())
            If MinValidTime.HasValue Then
                sb.Append("minValidTime=").Append(MinValidTime.Value)
            End If
            If MaxValidTime.HasValue Then
                If MinValidTime.HasValue Then
                    sb.Append(",")
                End If
                sb.Append("maxValidTime=").Append(MaxValidTime.Value)
            End If
            sb.Append(")")
            Return sb.ToString()
        End Function
    End Class

End Namespace
