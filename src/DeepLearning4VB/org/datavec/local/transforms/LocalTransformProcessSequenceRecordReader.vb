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
Imports SequenceRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports TransformProcessSequenceRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.impl.transform.TransformProcessSequenceRecordReader
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms

    ''' <summary>
    ''' A sequence record reader that applies a transform process to each sequence of another sequence record reader,
    ''' with <see cref="LocalTransformExecutor.executeSequenceToSequence"/> (so sequence splits and the other whole-data
    ''' steps work on the sequence too).
    ''' </summary>
    <Serializable>
    Public Class LocalTransformProcessSequenceRecordReader
        Inherits TransformProcessSequenceRecordReaderT

        Public Sub New(ByVal sequenceRecordReader As SequenceRecordReaderT, ByVal transformProcess As TransformProcessT)
            MyBase.New(sequenceRecordReader, transformProcess)
        End Sub

        ''' <summary>
        ''' Reads the next sequence and returns it transformed (the first resulting sequence, as in Java).
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">if the transform process removed the sequence (Java threw an
        ''' <c>IndexOutOfBoundsException</c>)</exception>
        Public Overrides Function sequenceRecord() As IList(Of IList(Of WritableT))
            Dim input As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT))) From {sequenceRecordReader.nextSequence().getSequenceRecord()}
            Return LocalTransformExecutor.executeSequenceToSequence(input, transformProcess)(0)
        End Function

    End Class

End Namespace
