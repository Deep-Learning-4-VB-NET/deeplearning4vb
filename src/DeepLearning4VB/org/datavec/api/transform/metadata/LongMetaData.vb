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
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable

Namespace org.datavec.api.transform.metadata

    ''' <summary>Metadata for a long column with optional min/max bounds.</summary>
    <Serializable>
    Public Class LongMetaData
        Inherits BaseColumnMetaData

        Public Sub New(ByVal name As String)
            Me.New(name, Nothing, Nothing)
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal name As String, ByVal minAllowedValue As Long?, ByVal maxAllowedValue As Long?)
            MyBase.New(name)
            Me.MinAllowedValue = minAllowedValue
            Me.MaxAllowedValue = maxAllowedValue
        End Sub

        'minAllowedValue/maxAllowedValue are nullable: null -> no restriction on minAllowedValue/maxAllowedValue values
        Public ReadOnly Property MinAllowedValue As Long?
        Public ReadOnly Property MaxAllowedValue As Long?

        Public Function getMinAllowedValue() As Long?
            Return MinAllowedValue
        End Function

        Public Function getMaxAllowedValue() As Long?
            Return MaxAllowedValue
        End Function

        Public Overrides ReadOnly Property ColumnType As ColumnTypeEnum
            Get
                Return ColumnTypeEnum.Long
            End Get
        End Property

        Public Overrides Function isValid(ByVal writable As writable.Writable) As Boolean
            Dim value As Long
            If TypeOf writable Is IntWritable OrElse TypeOf writable Is LongWritable Then
                value = writable.toLong()
            ElseIf Not JavaNumbers.tryParseLong(writable.ToString(), value) Then
                Return False
            End If
            Return check(value)
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
            If Not JavaNumbers.tryParseLong(input.ToString(), value) Then
                Return False
            End If
            Return check(value)
        End Function

        Private Function check(ByVal value As Long) As Boolean
            If MinAllowedValue.HasValue AndAlso value < MinAllowedValue.Value Then
                Return False
            End If
            If MaxAllowedValue.HasValue AndAlso value > MaxAllowedValue.Value Then
                Return False
            End If

            Return True
        End Function

        Public Overrides Function clone() As ColumnMetaData
            Return New LongMetaData(Name, MinAllowedValue, MaxAllowedValue)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other = DirectCast(obj, LongMetaData)
            Return Nullable.Equals(MinAllowedValue, other.MinAllowedValue) AndAlso Nullable.Equals(MaxAllowedValue, other.MaxAllowedValue)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(Name, MinAllowedValue, MaxAllowedValue)
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("LongMetaData(name=""").Append(Name).Append(""",")
            If MinAllowedValue.HasValue Then
                sb.Append("minAllowed=").Append(MinAllowedValue.Value)
            End If
            If MaxAllowedValue.HasValue Then
                If MinAllowedValue.HasValue Then
                    sb.Append(",")
                End If
                sb.Append("maxAllowed=").Append(MaxAllowedValue.Value)
            End If
            sb.Append(")")
            Return sb.ToString()
        End Function
    End Class

End Namespace
