Imports System
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports WritableType = DeepLearning4VB.org.datavec.api.writable.WritableType

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

Namespace org.datavec.api.formats.input

    ''' <summary>
    ''' Base class for input formats: <c>createReader(split)</c> delegates to
    ''' <c>createReader(split, Nothing)</c>; the <see cref="writable.Writable"/> members are no-ops or unsupported.
    '''
    ''' @author Adam Gibson
    ''' </summary>
    <Serializable>
    Public MustInherit Class BaseInputFormat
        Implements InputFormat

        Public MustOverride Function createReader(ByVal split As InputSplit, ByVal conf As Configuration) As RecordReader Implements InputFormat.createReader

        Public Overridable Function createReader(ByVal split As InputSplit) As RecordReader Implements InputFormat.createReader
            Return createReader(split, Nothing)
        End Function

        Public Overridable Sub write(ByVal [out] As DataOutput) Implements writable.Writable.write

        End Sub

        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements writable.Writable.readFields

        End Sub

        Public Overridable Function toDouble() As Double Implements writable.Writable.toDouble
            Throw New NotSupportedException()
        End Function

        Public Overridable Function toFloat() As Single Implements writable.Writable.toFloat
            Throw New NotSupportedException()
        End Function

        Public Overridable Function toInt() As Integer Implements writable.Writable.toInt
            Throw New NotSupportedException()
        End Function

        Public Overridable Function toLong() As Long Implements writable.Writable.toLong
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements writable.Writable.writeType
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function [getType]() As WritableType Implements writable.Writable.getType
            Throw New NotSupportedException()
        End Function

    End Class

End Namespace
