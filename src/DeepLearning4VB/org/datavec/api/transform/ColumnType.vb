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
Imports System.Runtime.CompilerServices
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports WritableType = DeepLearning4VB.org.datavec.api.writable.WritableType

Namespace org.datavec.api.transform

    ''' <summary>
    ''' The kinds of column a <see cref="schema.Schema"/> can hold. The Java instance methods
    ''' (<c>newColumnMetaData</c>, <c>getWritableType</c>) are the extension methods in
    ''' <see cref="ColumnTypeExtensions"/>. Serializes by name.
    ''' </summary>
    <JsonConverter(GetType(JsonStringEnumConverter))>
    Public Enum ColumnType
        [String]
        [Integer]
        [Long]
        [Double]
        Float
        Categorical
        Time
        ''' <summary>Arbitrary byte[] data</summary>
        Bytes
        [Boolean]
        ''' <summary>Kept for ordinal/JSON compatibility; the NDArray metadata is not ported yet (needs INDArray).</summary>
        NDArray
    End Enum

    ''' <summary>The Java instance methods of the <c>ColumnType</c> enum, as extension methods.</summary>
    Public Module ColumnTypeExtensions

        ''' <summary>Creates the default metadata for a new column of this type.</summary>
        <Extension>
        Public Function newColumnMetaData(ByVal type As ColumnType, ByVal columnName As String) As ColumnMetaData
            Select Case type
                Case ColumnType.String
                    Return New StringMetaData(columnName)
                Case ColumnType.Integer
                    Return New IntegerMetaData(columnName)
                Case ColumnType.Long
                    Return New LongMetaData(columnName)
                Case ColumnType.Double
                    Return New DoubleMetaData(columnName)
                Case ColumnType.Float
                    Return New FloatMetaData(columnName)
                Case ColumnType.Time
                    Return New TimeMetaData(columnName)
                Case ColumnType.Boolean
                    Return New CategoricalMetaData(columnName, "true", "false")
                Case ColumnType.Categorical
                    Throw New NotSupportedException("Cannot create new categorical column using this method: categorical state names would be unknown")
                Case ColumnType.NDArray
                    ' NDArrayMetaData is not ported (it needs INDArray); Java throws here as well.
                    Throw New NotSupportedException("Cannot create new NDArray column using this method: shape information would be unknown")
                Case Else 'And Bytes
                    Throw New NotSupportedException("Unknown or not supported column type: " & type.ToString())
            End Select
        End Function

        ''' <returns> The type of writable for this column </returns>
        <Extension>
        Public Function getWritableType(ByVal type As ColumnType) As WritableType
            Select Case type
                Case ColumnType.String
                    Return WritableType.Text
                Case ColumnType.Integer
                    Return WritableType.Int
                Case ColumnType.Long
                    Return WritableType.Long
                Case ColumnType.Double
                    Return WritableType.Double
                Case ColumnType.Float
                    Return WritableType.Float
                Case ColumnType.Categorical
                    Return WritableType.Text
                Case ColumnType.Time
                    Return WritableType.Long
                Case ColumnType.Bytes
                    Return WritableType.Byte
                Case ColumnType.Boolean
                    Return WritableType.Boolean
                Case ColumnType.NDArray
                    ' As in Java (the NDArray writable itself is not ported yet).
                    Return WritableType.Image
                Case Else
                    Throw New InvalidOperationException("Unknown writable type for column type: " & type.ToString())
            End Select
        End Function

        ''' <summary>Java <c>ColumnType.values()</c>.</summary>
        Public Function ColumnTypeValues() As ColumnType()
            Return DirectCast([Enum].GetValues(GetType(ColumnType)), ColumnType())
        End Function

        ''' <summary>Parses the enum constant name (Java <c>ColumnType.valueOf</c>).</summary>
        Public Function ColumnTypeValueOf(ByVal name As String) As ColumnType
            Dim result As ColumnType
            ' Enum.TryParse also accepts numeric strings ("3"); Java valueOf only accepts the constant names
            If name IsNot Nothing AndAlso name.Length > 0 AndAlso Char.IsLetter(name(0)) AndAlso [Enum].TryParse(name, False, result) AndAlso [Enum].IsDefined(GetType(ColumnType), result) Then
                Return result
            End If
            Throw New ArgumentException("No enum constant ColumnType." & name)
        End Function

    End Module

End Namespace
