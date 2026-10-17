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
Imports System.Linq
Imports System.Text.Json.Serialization
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Convert a set of values to a sequence: the records with the same value(s) in the key column(s) form one
    ''' sequence, whose time steps are sorted with the comparator. In "single step sequences mode" every record becomes
    ''' a sequence of length 1 (no key, no comparator). This class only describes the conversion; grouping and sorting
    ''' are done by the executor. Equality (Lombok <c>@EqualsAndHashCode(exclude = {"inputSchema"})</c>) compares the
    ''' mode, the key columns and the comparator.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"})
    Public Class ConvertToSequence

        Private singleStepSequencesMode As Boolean
        Private ReadOnly keyColumns() As String
        Private ReadOnly comparator As SequenceComparator 'For sorting values within collected (unsorted) sequence
        Private inputSchemaField As SchemaT

        ''' <param name="keyColumn"> The value to use as the key for inferring which examples belong to what sequence </param>
        ''' <param name="comparator"> The comparator to use when deciding the order of each possible time step in the sequence </param>
        Public Sub New(ByVal keyColumn As String, ByVal comparator As SequenceComparator)
            Me.New(False, New String() {keyColumn}, comparator)
        End Sub

        ''' <param name="keyColumns"> The value or values to use as the key (multiple values: compound key)  for inferring which
        '''                   examples belong to what sequence </param>
        ''' <param name="comparator"> The comparator to use when deciding the order of each possible time step in the sequence </param>
        Public Sub New(ByVal keyColumns As ICollection(Of String), ByVal comparator As SequenceComparator)
            Me.New(False, keyColumns.ToArray(), comparator)
        End Sub

        ''' <param name="singleStepSequencesMode"> If true: every record becomes a sequence of length 1 </param>
        ''' <param name="keyColumns"> The value or values to use as the key (multiple values: compound key)  for inferring which
        '''                   examples belong to what sequence (JSON property "keyColumn") </param>
        ''' <param name="comparator"> The comparator to use when deciding the order of each possible time step in the sequence </param>
        <JsonConstructor>
        Public Sub New(ByVal singleStepSequencesMode As Boolean, ByVal keyColumns() As String, ByVal comparator As SequenceComparator)
            Me.singleStepSequencesMode = singleStepSequencesMode
            Me.keyColumns = keyColumns
            Me.comparator = comparator
        End Sub

        ''' <summary>Java <c>isSingleStepSequencesMode()</c> (Lombok).</summary>
        Public Function isSingleStepSequencesMode() As Boolean
            Return singleStepSequencesMode
        End Function

        ''' <summary>Java <c>setSingleStepSequencesMode()</c> (Lombok).</summary>
        Public Sub setSingleStepSequencesMode(ByVal singleStepSequencesMode As Boolean)
            Me.singleStepSequencesMode = singleStepSequencesMode
        End Sub

        ''' <summary>Java <c>getKeyColumns()</c> (Lombok).</summary>
        Public Function getKeyColumns() As String()
            Return keyColumns
        End Function

        ''' <summary>Java <c>getComparator()</c> (Lombok).</summary>
        Public Function getComparator() As SequenceComparator
            Return comparator
        End Function

        ''' <summary>The sequence schema with the same columns as the input schema.</summary>
        Public Overridable Function transform(ByVal schema As SchemaT) As SequenceSchemaT
            Return New SequenceSchemaT(schema.getColumnMetaData())
        End Function

        ''' <summary>
        ''' The input schema. Setting it (outside single step sequences mode) also gives the comparator the sequence schema.
        ''' </summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
                If Not singleStepSequencesMode Then
                    comparator.Schema = transform(value)
                End If
            End Set
        End Property

        ''' <summary>Java <c>getInputSchema()</c> (Lombok).</summary>
        Public Function getInputSchema() As SchemaT
            Return inputSchemaField
        End Function

        Public Overrides Function ToString() As String
            If singleStepSequencesMode Then
                Return "ConvertToSequence()"
            ElseIf keyColumns.Length = 1 Then
                Return "ConvertToSequence(keyColumn=""" & keyColumns(0) & """,comparator=" & Objects.toString(comparator) & ")"
            Else
                Return "ConvertToSequence(keyColumns=""" & Arrays.toString(keyColumns) & """,comparator=" & Objects.toString(comparator) & ")"
            End If
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As ConvertToSequence = DirectCast(obj, ConvertToSequence)
            Return singleStepSequencesMode = other.singleStepSequencesMode AndAlso Arrays.equals(keyColumns, other.keyColumns) AndAlso
                Object.Equals(comparator, other.comparator)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(singleStepSequencesMode, Arrays.hashCode(keyColumns), comparator)
        End Function

    End Class

End Namespace
