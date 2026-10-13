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

    ''' <summary>Metadata for a float column: optional min/max bounds and whether NaN/infinite values are allowed.</summary>
    <Serializable>
    Public Class FloatMetaData
        Inherits BaseColumnMetaData

        Public Sub New(ByVal name As String)
            Me.New(name, Nothing, Nothing, False, False)
        End Sub

        ''' <param name="minAllowedValue"> Min allowed value. If null: no restriction on minAllowedValue value in this column </param>
        ''' <param name="maxAllowedValue"> Max allowed value. If null: no restriction on maxAllowedValue value in this column </param>
        Public Sub New(ByVal name As String, ByVal minAllowedValue As Single?, ByVal maxAllowedValue As Single?)
            Me.New(name, minAllowedValue, maxAllowedValue, False, False)
        End Sub

        ''' <param name="minAllowedValue"> Min allowed value. If null: no restriction on minAllowedValue value in this column </param>
        ''' <param name="maxAllowedValue"> Max allowed value. If null: no restriction on maxAllowedValue value in this column </param>
        ''' <param name="allowNaN">      Are NaN values ok? </param>
        ''' <param name="allowInfinite"> Are +/- infinite values ok? </param>
        <JsonConstructor>
        Public Sub New(ByVal name As String, ByVal minAllowedValue As Single?, ByVal maxAllowedValue As Single?, ByVal allowNaN As Boolean, ByVal allowInfinite As Boolean)
            MyBase.New(name)
            Me.MinAllowedValue = minAllowedValue
            Me.MaxAllowedValue = maxAllowedValue
            Me.AllowNaN = allowNaN
            Me.AllowInfinite = allowInfinite
        End Sub

        'minAllowedValue/maxAllowedValue are nullable: null -> no restriction on minAllowedValue/maxAllowedValue values
        Public ReadOnly Property MinAllowedValue As Single?
        Public ReadOnly Property MaxAllowedValue As Single?
        Public ReadOnly Property AllowNaN As Boolean
        Public ReadOnly Property AllowInfinite As Boolean

        Public Function getMinAllowedValue() As Single?
            Return MinAllowedValue
        End Function

        Public Function getMaxAllowedValue() As Single?
            Return MaxAllowedValue
        End Function

        Public Function isAllowNaN() As Boolean
            Return AllowNaN
        End Function

        Public Function isAllowInfinite() As Boolean
            Return AllowInfinite
        End Function

        Public Overrides ReadOnly Property ColumnType As ColumnTypeEnum
            Get
                Return ColumnTypeEnum.Float
            End Get
        End Property

        Public Overrides Function isValid(ByVal writable As writable.Writable) As Boolean
            Dim d As Single
            Try
                d = writable.toFloat()
            Catch e As Exception
                Return False
            End Try
            Return check(d)
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
            ' Java: d = (Float) input, so anything that is not a Float is invalid
            If Not (TypeOf input Is Single) Then
                Return False
            End If
            Return check(CSng(input))
        End Function

        Private Function check(ByVal d As Single) As Boolean
            If AllowNaN AndAlso Single.IsNaN(d) Then
                Return True
            End If
            If AllowInfinite AndAlso Single.IsInfinity(d) Then
                Return True
            End If

            If MinAllowedValue.HasValue AndAlso d < MinAllowedValue.Value Then
                Return False
            End If
            If MaxAllowedValue.HasValue AndAlso d > MaxAllowedValue.Value Then
                Return False
            End If

            Return True
        End Function

        Public Overrides Function clone() As ColumnMetaData
            Return New FloatMetaData(Name, MinAllowedValue, MaxAllowedValue, AllowNaN, AllowInfinite)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other = DirectCast(obj, FloatMetaData)
            Return Nullable.Equals(MinAllowedValue, other.MinAllowedValue) AndAlso Nullable.Equals(MaxAllowedValue, other.MaxAllowedValue) AndAlso
                AllowNaN = other.AllowNaN AndAlso AllowInfinite = other.AllowInfinite
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(Name, MinAllowedValue, MaxAllowedValue, AllowNaN, AllowInfinite)
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("FloatMetaData(name=""").Append(Name).Append(""",")
            Dim needComma As Boolean = False
            If MinAllowedValue.HasValue Then
                sb.Append("minAllowed=").Append(Arrays.JavaFloatToString(MinAllowedValue.Value))
                needComma = True
            End If
            If MaxAllowedValue.HasValue Then
                If needComma Then
                    sb.Append(",")
                End If
                sb.Append("maxAllowed=").Append(Arrays.JavaFloatToString(MaxAllowedValue.Value))
                needComma = True
            End If
            If needComma Then
                sb.Append(",")
            End If
            sb.Append("allowNaN=").Append(If(AllowNaN, "true", "false")).Append(",allowInfinite=").Append(If(AllowInfinite, "true", "false")).Append(")")
            Return sb.ToString()
        End Function
    End Class

End Namespace
