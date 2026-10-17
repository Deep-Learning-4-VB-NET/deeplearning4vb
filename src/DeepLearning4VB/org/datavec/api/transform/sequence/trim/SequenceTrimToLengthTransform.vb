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
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.trim

    ''' <summary>
    ''' Trim or pad the sequence to the specified length (number of sequence steps). It supports 2 modes:<br/>
    ''' TRIM: Sequences longer than the specified maximum will be trimmed to exactly the maximum. Shorter sequences will not be modified.<br/>
    ''' TRIM_OR_PAD: Sequences longer than the specified maximum will be trimmed to exactly the maximum. Shorter sequences will be
    ''' padded with as many copies of the "pad" array to make the sequence length equal the specified maximum.<br/>
    ''' Note that the 'pad' list (i.e., values to pad when using TRIM_OR_PAD mode) must be equal in length to the number of columns (values per time step)
    ''' Equality (Lombok <c>@EqualsAndHashCode(exclude = {"schema"})</c>) compares the length, the mode and the padding.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"schema"})
    <Serializable>
    Public Class SequenceTrimToLengthTransform
        Implements TransformT

        ''' <summary>
        ''' Mode. See <seealso cref="SequenceTrimToLengthTransform"/>. Serializes by name;
        ''' <see cref="SequenceTrimToLengthTransformExtensions.ModeValueOf"/> is Java's <c>Mode.valueOf</c>.
        ''' </summary>
        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum Mode
            TRIM
            TRIM_OR_PAD
        End Enum

        Private maxLength As Integer
        Private modeField As Mode
        Private pad As IList(Of WritableT)
        Private schemaField As SchemaT

        ''' <param name="maxLength"> maximum sequence length. Must be positive. </param>
        ''' <param name="mode">      Mode - trim or trim/pad </param>
        ''' <param name="pad">       Padding value. Only used with Mode.TRIM_OR_PAD. Must be equal in length to the number of columns (values per time step) </param>
        ''' <exception cref="InvalidOperationException">if the length is not positive, or the padding is missing for TRIM_OR_PAD</exception>
        <JsonConstructor>
        Public Sub New(ByVal maxLength As Integer, ByVal mode As Mode, ByVal pad As IList(Of WritableT))
            Preconditions.checkState(maxLength > 0, "Maximum length must be > 0, got %s", maxLength)
            Preconditions.checkState(mode = SequenceTrimToLengthTransform.Mode.TRIM OrElse pad IsNot Nothing, "If mode == Mode.TRIM_OR_PAD ")
            Me.maxLength = maxLength
            Me.modeField = mode
            Me.pad = pad
        End Sub

        ''' <summary>Java <c>getMaxLength()</c> (Lombok).</summary>
        Public Function getMaxLength() As Integer
            Return maxLength
        End Function

        ''' <summary>Java <c>getMode()</c> (Lombok).</summary>
        Public Function getMode() As Mode
            Return modeField
        End Function

        ''' <summary>Java <c>getPad()</c> (Lombok).</summary>
        Public Function getPad() As IList(Of WritableT)
            Return pad
        End Function

        ''' <summary>Java <c>getSchema()</c> (Lombok).</summary>
        Public Function getSchema() As SchemaT
            Return schemaField
        End Function

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Throw New NotSupportedException("SequenceTrimToLengthTransform cannot be applied to non-sequence values")
        End Function

        ''' <summary>
        ''' Trims (and in TRIM_OR_PAD mode pads) the sequence to the maximum length. Sequences that already have the
        ''' right length (TRIM: at most the maximum) are returned as they are.
        ''' </summary>
        ''' <exception cref="InvalidOperationException">when padding, if the padding length differs from the number of values per time step</exception>
        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            If modeField = SequenceTrimToLengthTransform.Mode.TRIM Then
                If sequence.Count <= maxLength Then
                    Return sequence
                End If
                Return New List(Of IList(Of WritableT))(sequence.Take(maxLength))
            Else
                'Trim or pad
                If sequence.Count = maxLength Then
                    Return sequence
                ElseIf sequence.Count > maxLength Then
                    Return New List(Of IList(Of WritableT))(sequence.Take(maxLength))
                Else
                    'Need to pad
                    ' (Java evaluated sequence.get(0) for the message even for an empty sequence, which threw there)
                    If sequence.Count > 0 Then
                        Preconditions.checkState(sequence(0).Count = pad.Count, "Invalid padding values: %s padding " & "values were provided, but data has %s values per time step (columns)", pad.Count, sequence(0).Count)
                    End If

                    Dim [out] As New List(Of IList(Of WritableT))(maxLength)
                    [out].AddRange(sequence)
                    Do While [out].Count < maxLength
                        [out].Add(pad)
                    Loop
                    Return [out]
                End If
            End If
        End Function

        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException()
        End Function

        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException()
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Return inputSchema
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
            Return Nothing
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As SequenceTrimToLengthTransform = DirectCast(obj, SequenceTrimToLengthTransform)
            If maxLength <> other.maxLength OrElse modeField <> other.modeField Then Return False
            If pad Is Nothing OrElse other.pad Is Nothing Then Return pad Is Nothing AndAlso other.pad Is Nothing
            If pad.Count <> other.pad.Count Then Return False
            For i As Integer = 0 To pad.Count - 1
                If Not Object.Equals(pad(i), other.pad(i)) Then Return False
            Next i
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = (59 + maxLength) * 59 + CInt(modeField)
            If pad IsNot Nothing Then
                For Each w As WritableT In pad
                    h = 31 * h + Objects.hashCode(w)
                Next w
            End If
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "SequenceTrimToLengthTransform(maxLength=" & maxLength & ", mode=" & modeField.ToString() & ", pad=" &
                If(pad Is Nothing, "null", "[" & String.Join(", ", pad.Select(Function(w) Objects.toString(w))) & "]") & ")"
        End Function

    End Class

    ''' <summary>The Java static methods of the <see cref="SequenceTrimToLengthTransform.Mode"/> enum.</summary>
    Public Module SequenceTrimToLengthTransformExtensions

        ''' <summary>Java <c>Mode.values()</c>.</summary>
        Public Function ModeValues() As SequenceTrimToLengthTransform.Mode()
            Return DirectCast([Enum].GetValues(GetType(SequenceTrimToLengthTransform.Mode)), SequenceTrimToLengthTransform.Mode())
        End Function

        ''' <summary>Parses the enum constant name (Java <c>Mode.valueOf</c>).</summary>
        ''' <exception cref="ArgumentException">for an unknown name (including numeric strings)</exception>
        Public Function ModeValueOf(ByVal name As String) As SequenceTrimToLengthTransform.Mode
            Dim result As SequenceTrimToLengthTransform.Mode
            ' Enum.TryParse also accepts numeric strings ("1"); Java valueOf only accepts the constant names
            If name IsNot Nothing AndAlso name.Length > 0 AndAlso Char.IsLetter(name(0)) AndAlso [Enum].TryParse(name, False, result) AndAlso [Enum].IsDefined(GetType(SequenceTrimToLengthTransform.Mode), result) Then
                Return result
            End If
            Throw New ArgumentException("No enum constant org.datavec.api.transform.sequence.trim.SequenceTrimToLengthTransform.Mode." & name)
        End Function

    End Module

End Namespace
