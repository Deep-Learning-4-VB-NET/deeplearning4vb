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
Imports RecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports TransformProcessRecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.impl.transform.TransformProcessRecordReader
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess

Namespace org.datavec.local.transforms

    ''' <summary>
    ''' A record reader that applies a transform process to each record of another record reader, skipping the records
    ''' removed by filters (see <c>TransformProcessRecordReader</c>, which does the work).
    ''' </summary>
    <Serializable>
    Public Class LocalTransformProcessRecordReader
        Inherits TransformProcessRecordReaderT

        ''' <summary>
        ''' Initialize with the internal record reader
        ''' and the transform process. </summary>
        ''' <param name="recordReader"> the reader of the input records </param>
        ''' <param name="transformProcess"> the transform process to apply to each record </param>
        Public Sub New(ByVal recordReader As RecordReaderT, ByVal transformProcess As TransformProcessT)
            MyBase.New(recordReader, transformProcess)
        End Sub

    End Class

End Namespace
