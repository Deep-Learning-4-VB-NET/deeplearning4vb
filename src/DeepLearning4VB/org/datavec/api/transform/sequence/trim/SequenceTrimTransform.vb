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
Imports System.Text.Json.Serialization
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.trim

    ''' <summary>
    ''' SequenceTrimTransform removes the first or last N values in a sequence. Note that the resulting sequence
    ''' may be of length 0, if the input number of values is less than or equal to N.
    ''' Equality (Lombok <c>@EqualsAndHashCode(exclude = {"schema"})</c>) compares the number of steps and the side.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"schema"})
    <Serializable>
    Public Class SequenceTrimTransform
        Implements TransformT

        Private numStepsToTrim As Integer
        Private trimFromStart As Boolean
        Private schemaField As SchemaT

        ''' <param name="numStepsToTrim"> Number of time steps to trim from the sequence </param>
        ''' <param name="trimFromStart">  If true: Trim values from the start of the sequence. If false: trim values from the end. </param>
        <JsonConstructor>
        Public Sub New(ByVal numStepsToTrim As Integer, ByVal trimFromStart As Boolean)
            Me.numStepsToTrim = numStepsToTrim
            Me.trimFromStart = trimFromStart
        End Sub

        ''' <summary>Java <c>getNumStepsToTrim()</c> (Lombok).</summary>
        Public Function getNumStepsToTrim() As Integer
            Return numStepsToTrim
        End Function

        ''' <summary>Java <c>isTrimFromStart()</c> (Lombok).</summary>
        Public Function isTrimFromStart() As Boolean
            Return trimFromStart
        End Function

        ''' <summary>Java <c>getSchema()</c> (Lombok).</summary>
        Public Function getSchema() As SchemaT
            Return schemaField
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Return inputSchema 'No op
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Get
                Return schemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.schemaField = value
            End Set
        End Property

        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return Nothing
        End Function

        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New List(Of String)(schemaField.getColumnNames()).ToArray()
        End Function

        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return outputColumnNames()
        End Function

        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return outputColumnName()
        End Function

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Throw New NotSupportedException("SequenceTrimTransform cannot be applied to non-sequence values")
        End Function

        ''' <summary>The sequence without its first (or last) <c>numStepsToTrim</c> time steps (empty if too short).</summary>
        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim start As Integer = 0
            Dim [end] As Integer = sequence.Count
            If trimFromStart Then
                start += numStepsToTrim
            Else
                [end] -= numStepsToTrim
            End If

            If [end] < start Then
                Return JavaCollections.emptyList(Of IList(Of WritableT))()
            End If

            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))([end] - start)

            For i As Integer = start To [end] - 1
                [out].Add(sequence(i))
            Next i

            Return [out]
        End Function

        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("SequenceTrimTransform cannot be applied to non-sequence values")
        End Function

        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("Not yet implemented")
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As SequenceTrimTransform = DirectCast(obj, SequenceTrimTransform)
            Return numStepsToTrim = other.numStepsToTrim AndAlso trimFromStart = other.trimFromStart
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (59 + numStepsToTrim) * 59 + If(trimFromStart, 79, 97)
        End Function

        Public Overrides Function ToString() As String
            Return "SequenceTrimTransform(numStepsToTrim=" & numStepsToTrim & ", trimFromStart=" & If(trimFromStart, "true", "false") & ")"
        End Function

    End Class

End Namespace
