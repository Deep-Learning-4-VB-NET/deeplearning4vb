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
Imports DeepLearning4VB.org.nd4vb.common.function
Imports RecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports StringSplitT = DeepLearning4VB.org.datavec.api.split.StringSplit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.functions

    ''' <summary>
    ''' Parses one line of text into a record with a line-based record reader (for example a
    ''' <c>CSVRecordReader</c>): the reader is initialized with a <c>StringSplit</c> holding the line and its first
    ''' record is returned. The reader is reused, so an instance must not be shared between threads.
    ''' </summary>
    Public Class LineRecordReaderFunction
        Implements [Function](Of String, IList(Of WritableT))

        Private ReadOnly recordReader As RecordReaderT

        Public Sub New(ByVal recordReader As RecordReaderT)
            Me.recordReader = recordReader
        End Sub

        ''' <exception cref="InvalidOperationException">if the reader cannot be initialized with the line</exception>
        Public Overridable Function apply(ByVal s As String) As IList(Of WritableT) Implements [Function](Of String, IList(Of WritableT)).apply
            Try
                recordReader.initialize(New StringSplitT(s))
            Catch e As Exception
                Throw New InvalidOperationException(e.Message, e)
            End Try
            Return recordReader.next()
        End Function

    End Class

End Namespace
