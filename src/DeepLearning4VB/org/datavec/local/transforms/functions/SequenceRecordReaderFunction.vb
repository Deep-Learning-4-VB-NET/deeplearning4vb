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
Imports System.IO
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports SequenceRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.functions

    ''' <summary>
    ''' Reads one sequence from a (path, stream) pair with <c>SequenceRecordReader.sequenceRecord(uri, stream)</c>.
    ''' The path becomes the sequence's URI; the stream is closed afterwards.
    ''' </summary>
    Public Class SequenceRecordReaderFunction
        Implements [Function](Of Pair(Of String, Stream), IList(Of IList(Of WritableT)))

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(SequenceRecordReaderFunction))

        Protected Friend sequenceRecordReader As SequenceRecordReaderT

        Public Sub New(ByVal sequenceRecordReader As SequenceRecordReaderT)
            Me.sequenceRecordReader = sequenceRecordReader
        End Sub

        ''' <exception cref="InvalidOperationException">if reading fails with an I/O error (the error is logged)</exception>
        Public Overridable Function apply(ByVal value As Pair(Of String, Stream)) As IList(Of IList(Of WritableT)) Implements [Function](Of Pair(Of String, Stream), IList(Of IList(Of WritableT))).apply
            Dim uri As New Uri(value.First, UriKind.RelativeOrAbsolute)
            Try
                Using dis As New DataInputStream(value.Second)
                    Return sequenceRecordReader.sequenceRecord(uri, dis)
                End Using
            Catch e As IOException
                log.error("", e)
                Throw New InvalidOperationException("Something went wrong", e)
            End Try
        End Function

    End Class

End Namespace
