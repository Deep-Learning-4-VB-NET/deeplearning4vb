Imports System
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports InputFormat = DeepLearning4VB.org.datavec.api.formats.input.InputFormat
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports ListStringRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.ListStringRecordReader
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

Namespace org.datavec.api.formats.input.impl

    ''' <summary>Input format creating a <see cref="ListStringRecordReader"/>.</summary>
    <Serializable>
    Public Class ListStringInputFormat
        Implements InputFormat

        ''' <summary>
        ''' Creates a reader from an input split
        ''' </summary>
        ''' <param name="split"> the split to read </param>
        ''' <param name="conf"> </param>
        ''' <returns> the reader from the given input split </returns>
        Public Overridable Function createReader(ByVal split As InputSplit, ByVal conf As Configuration) As RecordReader Implements InputFormat.createReader
            Dim reader As RecordReader = New ListStringRecordReader()
            reader.initialize(conf, split)
            Return reader
        End Function

        ''' <summary>
        ''' Creates a reader from an input split
        ''' </summary>
        ''' <param name="split"> the split to read </param>
        ''' <returns> the reader from the given input split </returns>
        Public Overridable Function createReader(ByVal split As InputSplit) As RecordReader Implements InputFormat.createReader
            Dim reader As RecordReader = New ListStringRecordReader()
            reader.initialize(split)
            Return reader
        End Function

        ''' <summary>
        ''' Serialize the fields of this object to <code>out</code>.
        ''' </summary>
        ''' <param name="out"> <code>DataOuput</code> to serialize this object into. </param>
        Public Overridable Sub write(ByVal [out] As DataOutput) Implements writable.Writable.write

        End Sub

        ''' <summary>
        ''' Deserialize the fields of this object from <code>in</code>.
        ''' <para>For efficiency, implementations should attempt to re-use storage in the
        ''' existing object where possible.</para>
        ''' </summary>
        ''' <param name="in"> <code>DataInput</code> to deseriablize this object from. </param>
        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements writable.Writable.readFields

        End Sub

        ''' <summary>
        ''' Convert Writable to double. Whether this is supported depends on the specific writable.
        ''' </summary>
        Public Overridable Function toDouble() As Double Implements writable.Writable.toDouble
            Return 0
        End Function

        ''' <summary>
        ''' Convert Writable to float. Whether this is supported depends on the specific writable.
        ''' </summary>
        Public Overridable Function toFloat() As Single Implements writable.Writable.toFloat
            Return 0
        End Function

        ''' <summary>
        ''' Convert Writable to int. Whether this is supported depends on the specific writable.
        ''' </summary>
        Public Overridable Function toInt() As Integer Implements writable.Writable.toInt
            Return 0
        End Function

        ''' <summary>
        ''' Convert Writable to long. Whether this is supported depends on the specific writable.
        ''' </summary>
        Public Overridable Function toLong() As Long Implements writable.Writable.toLong
            Return 0
        End Function

        Public Overridable Function [getType]() As WritableType Implements writable.Writable.getType
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements writable.Writable.writeType
            Throw New NotSupportedException()
        End Sub

    End Class

End Namespace
