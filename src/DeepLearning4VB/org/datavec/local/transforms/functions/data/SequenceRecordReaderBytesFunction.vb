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
Imports BytesWritableT = DeepLearning4VB.org.datavec.api.writable.BytesWritable
Imports SequenceRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.functions.data

    ''' <summary>
    ''' Reads one sequence from a (<c>Text</c> path, <c>BytesWritable</c> content) pair, as produced by
    ''' <see cref="FilesAsBytesFunction"/>, with <c>SequenceRecordReader.sequenceRecord(uri, stream)</c>.
    ''' </summary>
    Public Class SequenceRecordReaderBytesFunction
        Implements [Function](Of Pair(Of TextT, BytesWritableT), IList(Of IList(Of WritableT)))

        Private ReadOnly recordReader As SequenceRecordReaderT

        Public Sub New(ByVal recordReader As SequenceRecordReaderT)
            Me.recordReader = recordReader
        End Sub

        ''' <exception cref="InvalidOperationException">if reading fails with an I/O error</exception>
        Public Overridable Function apply(ByVal v1 As Pair(Of TextT, BytesWritableT)) As IList(Of IList(Of WritableT)) Implements [Function](Of Pair(Of TextT, BytesWritableT), IList(Of IList(Of WritableT))).apply
            Dim uri As New Uri(v1.First.ToString(), UriKind.RelativeOrAbsolute)
            Dim dis As New DataInputStream(New MemoryStream(v1.Second.getContent()))
            Try
                Return recordReader.sequenceRecord(uri, dis)
            Catch e As IOException
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Function

    End Class

End Namespace
