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
Imports System.Runtime.CompilerServices

Namespace org.datavec.api.writable

    ''' <summary>
    ''' The kinds of <see cref="Writable"/>. The ordinal values are part of the serialized format
    ''' (<see cref="WritableTypeExtensions.typeIdx"/>), so new types may only be appended.
    ''' </summary>
    Public Enum WritableType
        [Boolean] = 0
        [Byte] = 1
        [Double] = 2
        Float = 3
        Int = 4
        [Long] = 5
        Null = 6
        Text = 7
        NDArray = 8
        Image = 9
        Arrow = 10
        Bytes = 11
    End Enum

    ''' <summary>The Java instance methods of the <c>WritableType</c> enum, as extension methods.</summary>
    Public Module WritableTypeExtensions

        ''' <returns> True if Writable is defined in datavec-api, false otherwise </returns>
        <Extension>
        Public Function isCoreWritable(ByVal type As WritableType) As Boolean
            Select Case type
                Case WritableType.Image, WritableType.Arrow
                    Return False
                Case Else
                    Return True
            End Select
        End Function

        ''' <summary>
        ''' Return a unique type index for the given writable
        ''' </summary>
        ''' <returns> Type index for the writable </returns>
        <Extension>
        Public Function typeIdx(ByVal type As WritableType) As Short
            Return CShort(ordinal(type))
        End Function

        ''' <summary>The Java <c>ordinal()</c> of the value.</summary>
        <Extension>
        Public Function ordinal(ByVal type As WritableType) As Integer
            Return CInt(type)
        End Function

        ''' <summary>
        ''' Return the class of the implementation corresponding to each WritableType.
        ''' Note that if <seealso cref="isCoreWritable"/> returns false, null will be returned by this method.
        ''' <c>NDArray</c> also returns Nothing until <c>NDArrayWritable</c> is ported (it needs the ND4VB tensor API).
        ''' Like the Java original, <c>Bytes</c> maps to <see cref="ByteWritable"/> (not <see cref="BytesWritable"/>).
        ''' </summary>
        ''' <returns> Class for the given WritableType </returns>
        <Extension>
        Public Function getWritableClass(ByVal type As WritableType) As Type
            Select Case type
                Case WritableType.Boolean
                    Return GetType(BooleanWritable)
                Case WritableType.Byte
                    Return GetType(ByteWritable)
                Case WritableType.Double
                    Return GetType(DoubleWritable)
                Case WritableType.Float
                    Return GetType(FloatWritable)
                Case WritableType.Int
                    Return GetType(IntWritable)
                Case WritableType.Long
                    Return GetType(LongWritable)
                Case WritableType.Null
                    Return GetType(NullWritable)
                Case WritableType.Text
                    Return GetType(Text)
                Case WritableType.NDArray
                    Return Nothing
                Case WritableType.Bytes
                    Return GetType(ByteWritable)
                Case Else
                    Return Nothing
            End Select
        End Function

        ''' <summary>All values in ordinal order (Java <c>values()</c>).</summary>
        Public Function values() As WritableType()
            Return DirectCast([Enum].GetValues(GetType(WritableType)), WritableType())
        End Function

        ''' <summary>Java <c>valueOf</c>: the value with exactly this name; throws <see cref="ArgumentException"/> otherwise.</summary>
        Public Function valueOf(ByVal name As String) As WritableType
            For Each v As WritableType In values()
                If v.ToString() = name Then Return v
            Next v
            Throw New ArgumentException(name)
        End Function

    End Module

End Namespace
