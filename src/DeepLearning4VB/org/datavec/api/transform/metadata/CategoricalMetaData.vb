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
Imports System.Text
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.metadata

    ''' <summary>
    ''' Metadata for a categorical column: a value is valid if it is one of the state names.
    ''' Equality includes the column name (the Java Lombok <c>@EqualsAndHashCode</c> without <c>callSuper</c>
    ''' compared the state names only).
    ''' </summary>
    <JsonIgnoreProperties("stateNamesSet")>
    <Serializable>
    Public Class CategoricalMetaData
        Inherits BaseColumnMetaData

        Private ReadOnly stateNamesSet As ISet(Of String) 'For fast lookup

        Public Sub New(ByVal name As String, ByVal ParamArray stateNames() As String)
            Me.New(name, Arrays.asList(stateNames))
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal name As String, ByVal stateNames As IList(Of String))
            MyBase.New(name)
            Me.StateNames = stateNames
            stateNamesSet = New HashSet(Of String)(stateNames)
        End Sub

        Public Overrides ReadOnly Property ColumnType As ColumnTypeEnum
            Get
                Return ColumnTypeEnum.Categorical
            End Get
        End Property

        Public Overrides Function isValid(ByVal writable As writable.Writable) As Boolean
            Return stateNamesSet.Contains(writable.ToString())
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
            Return stateNamesSet.Contains(input.ToString())
        End Function

        Public Overrides Function clone() As ColumnMetaData
            Return New CategoricalMetaData(Name, StateNames)
        End Function

        ''' <summary>The allowable states of this column, in order.</summary>
        Public ReadOnly Property StateNames As IList(Of String)

        Public Overridable Function getStateNames() As IList(Of String)
            Return StateNames
        End Function

        ''' <summary>The allowable states as a set (Java: ignored by JSON).</summary>
        Public Overridable Function getStateNamesSet() As ISet(Of String)
            Return stateNamesSet
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other = DirectCast(obj, CategoricalMetaData)
            Return Arrays.equals(New List(Of String)(StateNames).ToArray(), New List(Of String)(other.StateNames).ToArray())
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 31 * MyBase.GetHashCode() + Arrays.hashCode(New List(Of String)(StateNames).ToArray())
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("CategoricalMetaData(name=""").Append(Name).Append(""",stateNames=[")
            Dim first As Boolean = True
            For Each s As String In stateNamesSet
                If Not first Then
                    sb.Append(",")
                End If
                sb.Append("""").Append(s).Append("""")
                first = False
            Next s
            sb.Append("])")
            Return sb.ToString()
        End Function
    End Class

End Namespace
