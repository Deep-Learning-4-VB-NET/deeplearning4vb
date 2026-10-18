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
Imports System.IO
Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports BytesWritableT = DeepLearning4VB.org.datavec.api.writable.BytesWritable
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.local.transforms.functions.data

    ''' <summary>
    ''' Reads the whole content of a (path, stream) pair into a (<c>Text</c> path, <c>BytesWritable</c> content) pair.
    ''' The stream is read to its end but not closed (as with Java's <c>IOUtils.toByteArray</c>).
    ''' </summary>
    Public Class FilesAsBytesFunction
        Implements [Function](Of Pair(Of String, Stream), Pair(Of TextT, BytesWritableT))

        ''' <exception cref="InvalidOperationException">if reading fails with an I/O error</exception>
        Public Overridable Function apply(ByVal [in] As Pair(Of String, Stream)) As Pair(Of TextT, BytesWritableT) Implements [Function](Of Pair(Of String, Stream), Pair(Of TextT, BytesWritableT)).apply
            Try
                Using buffer As New MemoryStream()
                    [in].Second.CopyTo(buffer)
                    Return Pair.of(New TextT([in].First), New BytesWritableT(buffer.ToArray()))
                End Using
            Catch e As IOException
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Function

    End Class

End Namespace
