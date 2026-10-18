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
Imports System.Collections.Generic
Imports ColumnOpT = DeepLearning4VB.org.datavec.api.transform.ColumnOp
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.serde.testClasses

    ''' <summary>A user-defined filter (Java: <c>@AllArgsConstructor @NoArgsConstructor @Data</c>).</summary>
    <Serializable>
    Public Class CustomFilter
        Implements FilterT

        Private someFilterArg As Long

        Public Sub New()
        End Sub

        Public Sub New(ByVal someFilterArg As Long)
            Me.someFilterArg = someFilterArg
        End Sub

        Public Function getSomeFilterArg() As Long
            Return someFilterArg
        End Function

        Public Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Global.DeepLearning4VB.org.datavec.api.transform.Operation(Of SchemaT, SchemaT).transform
            Return Nothing
        End Function

        Public Function outputColumnName() As String Implements ColumnOpT.outputColumnName
            Return Nothing
        End Function

        Public Function outputColumnNames() As String() Implements ColumnOpT.outputColumnNames
            Return New String() {}
        End Function

        Public Function columnNames() As String() Implements ColumnOpT.columnNames
            Return New String() {}
        End Function

        Public Function columnName() As String Implements ColumnOpT.columnName
            Return Nothing
        End Function

        Public Function removeExample(ByVal example As Object) As Boolean Implements FilterT.removeExample
            Return False
        End Function

        Public Function removeSequence(ByVal sequence As Object) As Boolean Implements FilterT.removeSequence
            Return False
        End Function

        Public Function removeExample(ByVal writables As IList(Of WritableT)) As Boolean Implements FilterT.removeExample
            Return False
        End Function

        Public Function removeSequence(ByVal sequence As IList(Of IList(Of WritableT))) As Boolean Implements FilterT.removeSequence
            Return False
        End Function

        Public Property InputSchema As SchemaT Implements ColumnOpT.InputSchema
            Get
                Return Nothing
            End Get
            Set(ByVal value As SchemaT)
            End Set
        End Property

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As CustomFilter = TryCast(obj, CustomFilter)
            Return other IsNot Nothing AndAlso someFilterArg = other.someFilterArg
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return someFilterArg.GetHashCode()
        End Function
    End Class

End Namespace
