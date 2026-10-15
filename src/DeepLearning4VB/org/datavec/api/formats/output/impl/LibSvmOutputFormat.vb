Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports OutputFormat = DeepLearning4VB.org.datavec.api.formats.output.OutputFormat
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter
Imports LibSvmRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter

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

Namespace org.datavec.api.formats.output.impl

    ''' <summary>
    ''' Output format creating a <see cref="LibSvmRecordWriter"/>.
    '''
    ''' @author Adam Gibson
    ''' </summary>
    Public Class LibSvmOutputFormat
        Implements OutputFormat

        Public Overridable Function createWriter(ByVal conf As Configuration) As RecordWriter Implements OutputFormat.createWriter
            Dim writer As RecordWriter = New LibSvmRecordWriter()
            writer.Conf = conf
            Return writer
        End Function

    End Class

End Namespace
