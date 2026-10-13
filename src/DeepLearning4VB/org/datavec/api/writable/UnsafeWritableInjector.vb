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

Namespace org.datavec.api.writable

    ''' <summary>
    ''' Wraps a plain value (number, string, byte, Nothing or an existing writable) in the matching
    ''' <see cref="Writable"/>. The Java check that rejects <c>INDArray</c> arguments is not ported
    ''' (the tensor API does not exist yet); such values fail with the generic "wrong argument" error.
    ''' </summary>
    Public Class UnsafeWritableInjector
        Public Shared Function inject(Of T)(ByVal x As T) As Writable
            Dim o As Object = x
            If o Is Nothing Then
                Return NullWritable.INSTANCE
            ElseIf TypeOf o Is Writable Then
                Return DirectCast(o, Writable)
            ElseIf TypeOf o Is Integer Then
                Return New IntWritable(CInt(o))
            ElseIf TypeOf o Is Long Then
                Return New LongWritable(CLng(o))
            ElseIf TypeOf o Is Single Then
                Return New FloatWritable(CSng(o))
            ElseIf TypeOf o Is Double Then
                Return New DoubleWritable(CDbl(o))
            ElseIf TypeOf o Is String Then
                Return New Text(CStr(o))
            ElseIf TypeOf o Is SByte Then
                Return New ByteWritable(CSByte(o))
            ElseIf TypeOf o Is Byte Then
                ' an unsigned .NET byte is taken as the same bit pattern (Java bytes are signed)
                Dim b As Integer = CInt(CByte(o))
                Return New ByteWritable(CSByte(If(b > 127, b - 256, b)))
            Else
                Throw New ArgumentException("Wrong argument type for writable conversion " & o.GetType().FullName)
            End If
        End Function
    End Class

End Namespace
