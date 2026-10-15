Imports System
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports BaseInputFormat = DeepLearning4VB.org.datavec.api.formats.input.BaseInputFormat
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports LibSvmRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.LibSvmRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit

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

Namespace org.datavec.api.formats.input.impl

    ''' <summary>
    ''' Lib svm input format
    '''
    ''' @author Adam Gibson
    ''' </summary>
    <Serializable>
    Public Class LibSvmInputFormat
        Inherits BaseInputFormat

        Public Overrides Function createReader(ByVal split As InputSplit, ByVal conf As Configuration) As RecordReader
            Dim reader As RecordReader = New LibSvmRecordReader()
            reader.initialize(conf, split)
            Return reader
        End Function

    End Class

End Namespace
