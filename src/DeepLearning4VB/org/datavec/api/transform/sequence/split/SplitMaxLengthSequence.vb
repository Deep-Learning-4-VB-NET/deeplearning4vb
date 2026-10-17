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
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.split

    ''' <summary>
    ''' Split a sequence into a number of smaller sequences, if the length of the sequence exceeds the specified
    ''' maximum sequence length.<br/>
    ''' With <c>equalSplits</c> the sequence is split into the smallest possible number of sequences of at most
    ''' <c>maxSequenceLength</c> time steps whose lengths differ by at most one (the earlier ones are the longer ones);
    ''' the Java code used the number of splits as the split size, which produced sequences longer than the maximum.
    ''' Without <c>equalSplits</c>: n sequences of <c>maxSequenceLength</c> and (if necessary) one shorter one.
    ''' Equality (Lombok <c>@EqualsAndHashCode(exclude = {"inputSchema"})</c>) compares the maximum length and the mode.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"})
    <Serializable>
    Public Class SplitMaxLengthSequence
        Implements SequenceSplitT

        Private ReadOnly maxSequenceLength As Integer
        Private ReadOnly equalSplits As Boolean
        Private inputSchemaField As SchemaT

        ''' <param name="maxSequenceLength"> max length of sequences </param>
        ''' <param name="equalSplits">       if true: split larger sequences into equal sized subsequences. If false: split into
        '''                          n maxSequenceLength sequences, and (if necessary) 1 with 1 &lt;= length &lt; maxSequenceLength </param>
        <JsonConstructor>
        Public Sub New(ByVal maxSequenceLength As Integer, ByVal equalSplits As Boolean)
            Me.maxSequenceLength = maxSequenceLength
            Me.equalSplits = equalSplits
        End Sub

        ''' <summary>Java <c>getMaxSequenceLength()</c> (Lombok).</summary>
        Public Function getMaxSequenceLength() As Integer
            Return maxSequenceLength
        End Function

        ''' <summary>Java <c>isEqualSplits()</c> (Lombok).</summary>
        Public Function isEqualSplits() As Boolean
            Return equalSplits
        End Function

        Public Overridable Function split(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of IList(Of WritableT))) Implements SequenceSplitT.split
            Dim n As Integer = sequence.Count
            If n <= maxSequenceLength Then
                Return JavaCollections.singletonList(sequence)
            End If

            Dim [out] As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            If equalSplits Then
                Dim numSplits As Integer = n \ maxSequenceLength
                If n Mod maxSequenceLength <> 0 Then
                    numSplits += 1
                End If
                Dim baseSize As Integer = n \ numSplits
                Dim numLonger As Integer = n Mod numSplits
                Dim pos As Integer = 0
                For s As Integer = 0 To numSplits - 1
                    Dim size As Integer = baseSize + If(s < numLonger, 1, 0)
                    Dim current As New List(Of IList(Of WritableT))(size)
                    For i As Integer = pos To pos + size - 1
                        current.Add(sequence(i))
                    Next i
                    pos += size
                    [out].Add(current)
                Next s
                Return [out]
            End If

            Dim splitSize As Integer = maxSequenceLength
            Dim currentSplit As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(splitSize)
            For Each [step] As IList(Of WritableT) In sequence
                If currentSplit.Count >= splitSize Then
                    [out].Add(currentSplit)
                    currentSplit = New List(Of IList(Of WritableT))(splitSize)
                End If
                currentSplit.Add([step])
            Next [step]
            [out].Add(currentSplit)

            Return [out]
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements SequenceSplitT.InputSchema
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return "SplitMaxLengthSequence(maxSequenceLength=" & maxSequenceLength & ",equalSplits=" & If(equalSplits, "true", "false") & ")"
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As SplitMaxLengthSequence = DirectCast(obj, SplitMaxLengthSequence)
            Return maxSequenceLength = other.maxSequenceLength AndAlso equalSplits = other.equalSplits
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (59 + maxSequenceLength) * 59 + If(equalSplits, 79, 97)
        End Function

    End Class

End Namespace
