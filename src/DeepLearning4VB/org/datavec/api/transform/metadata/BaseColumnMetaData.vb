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
Imports DeepLearning4VB.JavaCompat
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.metadata

    ''' <summary>
    ''' Base class of the column metadata implementations: holds the column name and the name-based equality
    ''' (Lombok <c>@EqualsAndHashCode</c> in the Java original); subclasses add their own fields to it.
    ''' </summary>
    <Serializable>
    Public MustInherit Class BaseColumnMetaData
        Implements ColumnMetaData

        Protected Friend Sub New(ByVal name As String)
            Me.Name = name
        End Sub

        Public Overridable Property Name As String Implements ColumnMetaData.Name

        Public Overridable Function getName() As String Implements ColumnMetaData.getName
            Return Name
        End Function

        Public Overridable Sub setName(ByVal name As String) Implements ColumnMetaData.setName
            Me.Name = name
        End Sub

        Public MustOverride ReadOnly Property ColumnType As ColumnTypeEnum Implements ColumnMetaData.ColumnType

        Public Overridable Function getColumnType() As ColumnTypeEnum Implements ColumnMetaData.getColumnType
            Return ColumnType
        End Function

        Public MustOverride Overloads Function isValid(ByVal writable As writable.Writable) As Boolean Implements ColumnMetaData.isValid

        Public MustOverride Overloads Function isValid(ByVal input As Object) As Boolean Implements ColumnMetaData.isValid

        Public MustOverride Function clone() As ColumnMetaData Implements ColumnMetaData.clone

        ''' <summary>Equal when the other object is of the same class and has the same name (subclasses add their fields).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Return String.Equals(Name, DirectCast(obj, BaseColumnMetaData).Name)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(Name)
        End Function

    End Class

End Namespace
