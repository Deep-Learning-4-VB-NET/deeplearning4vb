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
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.metadata

    ''' <summary>Metadata for a column of arbitrary bytes. Every value is considered valid (as in Java).</summary>
    <Serializable>
    Public Class BinaryMetaData
        Inherits BaseColumnMetaData

        <JsonConstructor>
        Public Sub New(ByVal name As String)
            MyBase.New(name)
        End Sub

        Public Overrides ReadOnly Property ColumnType As ColumnTypeEnum
            Get
                Return ColumnTypeEnum.Bytes
            End Get
        End Property

        Public Overrides Function isValid(ByVal writable As writable.Writable) As Boolean
            ' Java: Boolean.parseBoolean(writable.toString()) never throws, so the value is always valid
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
            Return True
        End Function

        Public Overrides Function clone() As ColumnMetaData
            Return New BinaryMetaData(Name)
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("BinaryMetaData(name=""").Append(Name).Append(""",")
            sb.Append(")")
            Return sb.ToString()
        End Function
    End Class

End Namespace
