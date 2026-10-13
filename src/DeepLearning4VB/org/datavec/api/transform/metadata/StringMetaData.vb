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

Namespace org.datavec.api.transform.metadata

    ''' <summary>Metadata for a string column: optional regex and min/max length restrictions.</summary>
    <Serializable>
    Public Class StringMetaData
        Inherits BaseColumnMetaData

        Public Sub New()
            Me.New(Nothing, Nothing, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' Default constructor with no restrictions on allowable strings
        ''' </summary>
        Public Sub New(ByVal name As String)
            Me.New(name, Nothing, Nothing, Nothing)
        End Sub

        ''' <param name="regex"> Nullable. If not null: this is a regex that each string must match in order for the entry
        '''                       to be considered valid. </param>
        ''' <param name="minLength">      Min allowable String length. If null: no restriction on min String length </param>
        ''' <param name="maxLength">      Max allowable String length. If null: no restriction on max String length </param>
        <JsonConstructor>
        Public Sub New(ByVal name As String, ByVal regex As String, ByVal minLength As Integer?, ByVal maxLength As Integer?)
            MyBase.New(name)
            Me.Regex = regex
            Me.MinLength = minLength
            Me.MaxLength = maxLength
        End Sub

        'regex + min/max length are nullable: null -> no restrictions on these
        Public ReadOnly Property Regex As String
        Public ReadOnly Property MinLength As Integer?
        Public ReadOnly Property MaxLength As Integer?

        Public Function getRegex() As String
            Return Regex
        End Function

        Public Function getMinLength() As Integer?
            Return MinLength
        End Function

        Public Function getMaxLength() As Integer?
            Return MaxLength
        End Function

        Public Overrides ReadOnly Property ColumnType As ColumnTypeEnum
            Get
                Return ColumnTypeEnum.String
            End Get
        End Property

        Public Overrides Function isValid(ByVal writable As writable.Writable) As Boolean
            Return isValidText(writable.ToString())
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
            Return isValidText(input.ToString())
        End Function

        Private Function isValidText(ByVal str As String) As Boolean
            Dim len As Integer = str.Length
            If MinLength.HasValue AndAlso len < MinLength.Value Then
                Return False
            End If
            If MaxLength.HasValue AndAlso len > MaxLength.Value Then
                Return False
            End If

            Return Regex Is Nothing OrElse JavaStrings.matches(str, Regex)
        End Function

        Public Overrides Function clone() As ColumnMetaData
            Return New StringMetaData(Name, Regex, MinLength, MaxLength)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other = DirectCast(obj, StringMetaData)
            Return String.Equals(Regex, other.Regex) AndAlso Nullable.Equals(MinLength, other.MinLength) AndAlso Nullable.Equals(MaxLength, other.MaxLength)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(Name, Regex, MinLength, MaxLength)
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("StringMetaData(name=""").Append(Name).Append(""",")
            If MinLength.HasValue Then
                sb.Append("minLengthAllowed=").Append(MinLength.Value)
            End If
            If MaxLength.HasValue Then
                If MinLength.HasValue Then
                    sb.Append(",")
                End If
                sb.Append("maxLengthAllowed=").Append(MaxLength.Value)
            End If
            If Regex IsNot Nothing Then
                If MinLength.HasValue OrElse MaxLength.HasValue Then
                    sb.Append(",")
                End If
                sb.Append("regex=").Append(Regex)
            End If
            sb.Append(")")
            Return sb.ToString()
        End Function

    End Class

End Namespace
