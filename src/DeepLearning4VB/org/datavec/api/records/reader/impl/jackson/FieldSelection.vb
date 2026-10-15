Imports System
Imports System.Collections.Generic
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

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

Namespace org.datavec.api.records.reader.impl.jackson

    ''' <summary>
    ''' The fields a Jackson-style reader extracts from each document: a list of paths (each a sequence of
    ''' property names) and, for each, the value to use when the path is missing.
    ''' </summary>
    <Serializable>
    Public Class FieldSelection

        Public Shared ReadOnly DEFAULT_MISSING_VALUE As writable.Writable = New Text("")

        Private ReadOnly fieldPathList As IList(Of String())
        Private ReadOnly valueIfMissingList As IList(Of writable.Writable)

        Private Sub New(ByVal builder As Builder)
            Me.fieldPathList = builder.fieldPaths
            Me.valueIfMissingList = builder.valueIfMissing
        End Sub

        Public Overridable Function getFieldPaths() As IList(Of String())
            Return fieldPathList
        End Function

        Public Overridable Function getValueIfMissing() As IList(Of writable.Writable)
            Return valueIfMissingList
        End Function

        Public Overridable Function getNumFields() As Integer
            Return fieldPathList.Count
        End Function

        Public Overridable ReadOnly Property FieldPaths As IList(Of String())
            Get
                Return fieldPathList
            End Get
        End Property

        Public Overridable ReadOnly Property ValueIfMissing As IList(Of writable.Writable)
            Get
                Return valueIfMissingList
            End Get
        End Property

        Public Overridable ReadOnly Property NumFields As Integer
            Get
                Return fieldPathList.Count
            End Get
        End Property

        Public Class Builder

            Friend fieldPaths As IList(Of String()) = New List(Of String())()
            Friend valueIfMissing As IList(Of writable.Writable) = New List(Of writable.Writable)()

            ''' <param name="fieldPath">    Path to the field. For example, {"a", "b", "c"} would be a field named c, in an object b,
            '''                     where b is in an object a </param>
            Public Overridable Function addField(ByVal ParamArray fieldPath() As String) As Builder
                Return addField(DEFAULT_MISSING_VALUE, fieldPath)
            End Function

            Public Overridable Function addField(ByVal valueIfMissing As writable.Writable, ByVal ParamArray fieldPath() As String) As Builder
                fieldPaths.Add(fieldPath)
                Me.valueIfMissing.Add(valueIfMissing)
                Return Me
            End Function

            Public Overridable Function build() As FieldSelection
                Return New FieldSelection(Me)
            End Function

        End Class

    End Class

End Namespace
