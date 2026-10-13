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
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.slf4j
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter

Namespace org.datavec.api.records.listener.impl

    ''' <summary>A record listener that logs each record read or written.</summary>
    <Serializable>
    Public Class LogRecordListener
        Implements RecordListener

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(LogRecordListener))
        ''' <summary>The invoked flag (the field is called <c>invoked</c> in Java; renamed because of the <c>invoked()</c> method).</summary>
        Private invokedFlag As Boolean = False

        Public Overridable Function invoked() As Boolean Implements RecordListener.invoked
            Return invokedFlag
        End Function

        Public Overridable Sub invoke() Implements RecordListener.invoke
            Me.invokedFlag = True
        End Sub

        Public Overridable Sub recordRead(ByVal reader As RecordReader, ByVal record As Object) Implements RecordListener.recordRead
            invoke()
            log.info("Reading " & Objects.toString(record))
        End Sub

        Public Overridable Sub recordWrite(ByVal writer As RecordWriter, ByVal record As Object) Implements RecordListener.recordWrite
            invoke()
            log.info("Writing " & Objects.toString(record))
        End Sub
    End Class

End Namespace
