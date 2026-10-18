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
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit
Imports TimeUnitT = DeepLearning4VB.JavaCompat.TimeUnit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.split

    ''' <summary>
    ''' Split a sequence into a number of smaller sequences, if the time difference between subsequent time steps
    ''' exceeds the specified amount.<br/>
    ''' Example: suppose we have a sequence, and want to split it into multiple sequences if the time between
    ''' subsequent time steps exceeds 1 minute: use <c>New SequenceSplitTimeSeparation("time", 1, TimeUnit.MINUTES)</c>.
    ''' Equality (Lombok <c>@EqualsAndHashCode(exclude = {"separationMilliseconds", "timeColumnIdx", "schema"})</c>)
    ''' compares the time column, the quantity and the unit.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"separationMilliseconds", "timeColumnIdx", "schema"})
    <JsonIgnoreProperties("separationMilliseconds", "timeColumnIdx", "schema")>
    <Serializable>
    Public Class SequenceSplitTimeSeparation
        Implements SequenceSplitT

        Private ReadOnly timeColumn As String
        Private ReadOnly timeQuantity As Long
        Private ReadOnly timeUnit As TimeUnitT
        Private ReadOnly separationMilliseconds As Long
        Private timeColumnIdx As Integer = -1
        Private schemaField As SchemaT

        ''' <param name="timeColumn">      Time column to consider when splitting </param>
        ''' <param name="timeQuantity">    Value/amount (of the specified TimeUnit) </param>
        ''' <param name="timeUnit">        The unit of time </param>
        <JsonConstructor>
        Public Sub New(ByVal timeColumn As String, ByVal timeQuantity As Long, ByVal timeUnit As TimeUnitT)
            Me.timeColumn = timeColumn
            Me.timeQuantity = timeQuantity
            Me.timeUnit = timeUnit

            Me.separationMilliseconds = TimeUnitT.MILLISECONDS.convert(timeQuantity, timeUnit)
        End Sub

        ''' <summary>Java <c>getTimeColumn()</c> (Lombok).</summary>
        Public Function getTimeColumn() As String
            Return timeColumn
        End Function

        ''' <summary>Java <c>getTimeQuantity()</c> (Lombok).</summary>
        Public Function getTimeQuantity() As Long
            Return timeQuantity
        End Function

        ''' <summary>Java <c>getTimeUnit()</c> (Lombok).</summary>
        Public Function getTimeUnit() As TimeUnitT
            Return timeUnit
        End Function

        ''' <summary>Java <c>getSeparationMilliseconds()</c> (Lombok).</summary>
        Public Function getSeparationMilliseconds() As Long
            Return separationMilliseconds
        End Function

        ''' <summary>Java <c>getTimeColumnIdx()</c> (Lombok).</summary>
        Public Function getTimeColumnIdx() As Integer
            Return timeColumnIdx
        End Function

        ''' <summary>
        ''' Splits the sequence wherever the time between two subsequent time steps exceeds the separation. As in Java,
        ''' an empty sequence gives a list holding one Nothing.
        ''' </summary>
        Public Overridable Function split(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of IList(Of WritableT))) Implements SequenceSplitT.split

            Dim [out] As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()

            Dim lastTimeStepTime As Long = Long.MinValue
            Dim currentSplit As IList(Of IList(Of WritableT)) = Nothing

            For Each timeStep As IList(Of WritableT) In sequence
                Dim currStepTime As Long = timeStep(timeColumnIdx).toLong()
                If lastTimeStepTime = Long.MinValue OrElse (currStepTime - lastTimeStepTime) > separationMilliseconds Then
                    'New split
                    If currentSplit IsNot Nothing Then
                        [out].Add(currentSplit)
                    End If
                    currentSplit = New List(Of IList(Of WritableT))()
                End If
                currentSplit.Add(timeStep)
                lastTimeStepTime = currStepTime
            Next timeStep

            'Add the final split to the output...
            [out].Add(currentSplit)

            Return [out]
        End Function

        ''' <summary>The input schema; the time column must exist and be a Time column.</summary>
        ''' <exception cref="InvalidOperationException">if the column is missing or not a time column</exception>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements SequenceSplitT.InputSchema
            Get
                Return schemaField
            End Get
            Set(ByVal value As SchemaT)
                If Not value.hasColumn(timeColumn) Then
                    Throw New InvalidOperationException("Invalid state: schema does not have column " & "with name """ & timeColumn & """")
                End If
                If value.getMetaData(timeColumn).ColumnType <> ColumnType.Time Then
                    Throw New InvalidOperationException("Invalid input schema: schema column """ & timeColumn & """ is not a time column." & " (Is type: " & value.getMetaData(timeColumn).ColumnType.ToString() & ")")
                End If

                Me.timeColumnIdx = value.getIndexOfColumn(timeColumn)
                Me.schemaField = value
            End Set
        End Property

        ''' <summary>Java <c>getSchema()</c> (Lombok).</summary>
        Public Function getSchema() As SchemaT
            Return schemaField
        End Function

        Public Overrides Function ToString() As String
            Return "SequenceSplitTimeSeparation(timeColumn=""" & timeColumn & """,timeQuantity=" & timeQuantity & ",timeUnit=" & timeUnit.ToString() & ")"
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As SequenceSplitTimeSeparation = DirectCast(obj, SequenceSplitTimeSeparation)
            Return String.Equals(timeColumn, other.timeColumn) AndAlso timeQuantity = other.timeQuantity AndAlso timeUnit = other.timeUnit
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(timeColumn, timeQuantity, timeUnit)
        End Function

    End Class

End Namespace
