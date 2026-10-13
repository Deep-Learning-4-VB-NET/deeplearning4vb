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
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports DeepLearning4VB.org.datavec.api.writable
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.schema

    ''' <summary>
    ''' A <see cref="Schema"/> for sequence data, with optional minimum and maximum sequence lengths.
    ''' VB has no covariant return types, so <see cref="newSchema"/> and <see cref="Builder.build"/> are typed as
    ''' <see cref="Schema"/> (they return a SequenceSchema); use <see cref="Builder.buildSequence"/> for a typed result.
    ''' </summary>
    <Serializable>
    Public Class SequenceSchema
        Inherits Schema

        Public Sub New(ByVal columnMetaData As IList(Of ColumnMetaData))
            Me.New(columnMetaData, Nothing, Nothing)
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal columnMetaData As IList(Of ColumnMetaData), ByVal minSequenceLength As Integer?, ByVal maxSequenceLength As Integer?)
            MyBase.New(columnMetaData)
            Me.MinSequenceLength = minSequenceLength
            Me.MaxSequenceLength = maxSequenceLength
        End Sub

        Private Sub New(ByVal builder As Builder)
            MyBase.New(builder)
            Me.MinSequenceLength = builder.minSequenceLengthField
            Me.MaxSequenceLength = builder.maxSequenceLengthField
        End Sub

        Public ReadOnly Property MinSequenceLength As Integer?
        Public ReadOnly Property MaxSequenceLength As Integer?

        Public Function getMinSequenceLength() As Integer?
            Return MinSequenceLength
        End Function

        Public Function getMaxSequenceLength() As Integer?
            Return MaxSequenceLength
        End Function

        ''' <summary>A new SequenceSchema with the given metadata and this schema's length restrictions (typed as Schema).</summary>
        Public Overrides Function newSchema(ByVal columnMetaData As IList(Of ColumnMetaData)) As Schema
            Return New SequenceSchema(columnMetaData, MinSequenceLength, MaxSequenceLength)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other = DirectCast(obj, SequenceSchema)
            Return Nullable.Equals(MinSequenceLength, other.MinSequenceLength) AndAlso Nullable.Equals(MaxSequenceLength, other.MaxSequenceLength)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), MinSequenceLength, MaxSequenceLength)
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            Dim nCol As Integer = numColumns()

            Dim maxNameLength As Integer = 0
            For Each s As String In getColumnNames()
                maxNameLength = Math.Max(maxNameLength, s.Length)
            Next s

            'Header:
            sb.Append("SequenceSchema(")

            If MinSequenceLength.HasValue Then
                sb.Append("minSequenceLength=").Append(MinSequenceLength.Value)
            End If
            If MaxSequenceLength.HasValue Then
                If MinSequenceLength.HasValue Then
                    sb.Append(",")
                End If
                sb.Append("maxSequenceLength=").Append(MaxSequenceLength.Value)
            End If

            sb.Append(")" & ControlChars.Lf)
            sb.Append(padRight("idx", 6)).Append(padRight("name", maxNameLength + 8)).Append(padRight("type", 15)).Append("meta data").Append(ControlChars.Lf)

            For i As Integer = 0 To nCol - 1
                Dim colName As String = getName(i)
                Dim type As ColumnType = [getType](i)
                Dim meta As ColumnMetaData = getMetaData(i)
                Dim paddedName As String = padRight("""" & colName & """", maxNameLength + 8)
                sb.Append(padRight(i.ToString(), 6)).Append(paddedName).Append(padRight(type.ToString(), 15)).Append(meta.ToString()).Append(ControlChars.Lf)
            Next i

            Return sb.ToString()
        End Function

        ''' <summary>Fluent builder for a <see cref="SequenceSchema"/>.</summary>
        Public Class Builder
            Inherits Schema.Builder

            Friend minSequenceLengthField As Integer?
            Friend maxSequenceLengthField As Integer?

            Public Overridable Function minSequenceLength(ByVal minLength As Integer) As Builder
                Me.minSequenceLengthField = minLength
                Return Me
            End Function

            Public Overridable Function maxSequenceLength(ByVal maxLength As Integer) As Builder
                Me.maxSequenceLengthField = maxLength
                Return Me
            End Function

            ''' <summary>Builds the SequenceSchema (typed as Schema: VB has no covariant return types).</summary>
            Public Overrides Function build() As Schema
                Return New SequenceSchema(Me)
            End Function

            ''' <summary>Builds the SequenceSchema with its own static type.</summary>
            Public Function buildSequence() As SequenceSchema
                Return New SequenceSchema(Me)
            End Function

        End Class

        ''' <summary>
        ''' Infers a sequence schema based
        ''' on the record </summary>
        ''' <param name="record"> the record to infer the schema based on </param>
        ''' <returns> the inferred sequence schema
        '''  </returns>
        Public Shared Function inferSequenceMulti(ByVal record As IList(Of IList(Of IList(Of writable.Writable)))) As SequenceSchema
            ' Ported as in Java, which tests the (list-typed) elements against the writable types and therefore
            ' throws for every non-empty input.
            Dim builder As New SequenceSchema.Builder()
            Dim minSequenceLength As Integer = record(0).Count
            Dim maxSequenceLength As Integer = record(0).Count
            For i As Integer = 0 To record.Count - 1
                Dim element As Object = record(i)
                If TypeOf element Is DoubleWritable Then
                    builder.addColumnDouble(i.ToString())
                ElseIf TypeOf element Is IntWritable Then
                    builder.addColumnInteger(i.ToString())
                ElseIf TypeOf element Is LongWritable Then
                    builder.addColumnLong(i.ToString())
                ElseIf TypeOf element Is FloatWritable Then
                    builder.addColumnFloat(i.ToString())
                Else
                    Throw New InvalidOperationException("Illegal writable for inferring schema of type " & record(i).GetType().ToString() & " with record " & record(0).ToString())
                End If
                builder.minSequenceLength(Math.Min(record(i).Count, minSequenceLength))
                builder.maxSequenceLength(Math.Max(record(i).Count, maxSequenceLength))
            Next i

            Return builder.buildSequence()
        End Function

        ''' <summary>
        ''' Infers a sequence schema based
        ''' on the record </summary>
        ''' <param name="record"> the record to infer the schema based on </param>
        ''' <returns> the inferred sequence schema
        '''  </returns>
        Public Shared Function inferSequence(ByVal record As IList(Of IList(Of writable.Writable))) As SequenceSchema
            ' Ported as in Java, which tests the (list-typed) elements against the writable types and therefore
            ' throws for every non-empty input.
            Dim builder As New SequenceSchema.Builder()
            For i As Integer = 0 To record.Count - 1
                Dim element As Object = record(i)
                If TypeOf element Is DoubleWritable Then
                    builder.addColumnDouble(i.ToString())
                ElseIf TypeOf element Is IntWritable Then
                    builder.addColumnInteger(i.ToString())
                ElseIf TypeOf element Is LongWritable Then
                    builder.addColumnLong(i.ToString())
                ElseIf TypeOf element Is FloatWritable Then
                    builder.addColumnFloat(i.ToString())
                Else
                    Throw New InvalidOperationException("Illegal writable for infering schema of type " & record(i).GetType().ToString())
                End If
            Next i

            builder.minSequenceLength(record.Count)
            builder.maxSequenceLength(record.Count)
            Return builder.buildSequence()
        End Function
    End Class

End Namespace
