Imports System
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports BaseInputFormat = DeepLearning4VB.org.datavec.api.formats.input.BaseInputFormat
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports LineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.LineRecordReader
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

    ''' <summary>Input format creating a <see cref="LineRecordReader"/>.</summary>
    <Serializable>
    Public Class LineInputFormat
        Inherits BaseInputFormat

        Public Overrides Function createReader(ByVal split As InputSplit, ByVal conf As Configuration) As RecordReader
            Return createReader(split)
        End Function

        Public Overrides Function createReader(ByVal split As InputSplit) As RecordReader
            Dim ret As New LineRecordReader()
            ret.initialize(split)
            Return ret
        End Function

        Public Overrides Sub write(ByVal [out] As DataOutput)

        End Sub

        Public Overrides Sub readFields(ByVal [in] As DataInput)

        End Sub

    End Class

End Namespace
