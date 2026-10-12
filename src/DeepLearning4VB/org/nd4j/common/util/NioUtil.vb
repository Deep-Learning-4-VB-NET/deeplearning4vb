Imports System

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

Namespace org.nd4j.common.util

    ''' <summary>
    ''' Strided element copies between raw byte buffers (the Java version worked on
    ''' <c>java.nio.ByteBuffer</c> views). Offsets and strides are in elements, not bytes, and the
    ''' buffers use the platform's native byte order.
    ''' </summary>
    Public NotInheritable Class NioUtil

        Private Sub New()
        End Sub

        Public Enum BufferType
            INT
            FLOAT
            [DOUBLE]
        End Enum

        ''' <summary>Copies <paramref name="n"/> elements from one buffer to another with independent strides.</summary>
        Public Shared Sub copyAtStride(ByVal n As Integer, ByVal bufferType As BufferType, ByVal from() As Byte, ByVal fromOffset As Integer, ByVal fromStride As Integer, ByVal [to]() As Byte, ByVal toOffset As Integer, ByVal toStride As Integer)
            If from Is Nothing Then Throw New ArgumentNullException(NameOf(from))
            If [to] Is Nothing Then Throw New ArgumentNullException(NameOf([to]))
            Dim elementSize As Integer
            Select Case bufferType
                Case BufferType.INT, BufferType.FLOAT
                    elementSize = 4
                Case BufferType.DOUBLE
                    elementSize = 8
                Case Else
                    Throw New ArgumentException("Only ints, floats and doubles supported")
            End Select
            For i As Integer = 0 To n - 1
                Dim src As Integer = (fromOffset + i * fromStride) * elementSize
                Dim dst As Integer = (toOffset + i * toStride) * elementSize
                Buffer.BlockCopy(from, src, [to], dst, elementSize)
            Next i
        End Sub

    End Class

End Namespace
