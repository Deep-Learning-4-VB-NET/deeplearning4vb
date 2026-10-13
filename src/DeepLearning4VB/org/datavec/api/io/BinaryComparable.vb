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

Namespace org.datavec.api.io

    ''' <summary>
    ''' A value whose ordering, equality and hash are defined by a byte array
    ''' (<see cref="Bytes"/>, valid up to <see cref="Length"/>).
    ''' </summary>
    Public MustInherit Class BinaryComparable
        Implements IComparable(Of BinaryComparable), IComparable

        ''' <summary>
        ''' Return n st bytes 0..n-1 from <see cref="Bytes"/> are valid.
        ''' </summary>
        Public MustOverride ReadOnly Property Length As Integer

        ''' <summary>
        ''' Return representative byte array for this instance.
        ''' </summary>
        Public MustOverride ReadOnly Property Bytes As Byte()

        Public Function getLength() As Integer
            Return Length
        End Function

        Public Function getBytes() As Byte()
            Return Bytes
        End Function

        ''' <summary>
        ''' Compare bytes from <see cref="Bytes"/>. </summary>
        ''' <seealso cref="WritableComparator.compareBytes"/>
        Public Overridable Function CompareTo(ByVal other As BinaryComparable) As Integer Implements IComparable(Of BinaryComparable).CompareTo
            If Me Is other Then
                Return 0
            End If
            Return WritableComparator.compareBytes(Bytes, 0, Length, other.Bytes, 0, other.Length)
        End Function

        ''' <summary>
        ''' Compare bytes from <see cref="Bytes"/> to those provided.
        ''' </summary>
        Public Overridable Function CompareTo(ByVal other() As Byte, ByVal off As Integer, ByVal len As Integer) As Integer
            Return WritableComparator.compareBytes(Bytes, 0, Length, other, off, len)
        End Function

        Public Overridable Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            Return CompareTo(DirectCast(obj, BinaryComparable))
        End Function

        ''' <summary>
        ''' Return true if bytes from <see cref="Bytes"/> match.
        ''' </summary>
        Public Overrides Function Equals(ByVal other As Object) As Boolean
            If Not (TypeOf other Is BinaryComparable) Then
                Return False
            End If
            Dim that As BinaryComparable = DirectCast(other, BinaryComparable)
            If Me.Length <> that.Length Then
                Return False
            End If
            Return Me.CompareTo(that) = 0
        End Function

        ''' <summary>
        ''' Return a hash of the bytes returned from <see cref="Bytes"/>. </summary>
        ''' <seealso cref="WritableComparator.hashBytes(Byte(), Integer)"/>
        Public Overrides Function GetHashCode() As Integer
            Return WritableComparator.hashBytes(Bytes, Length)
        End Function

    End Class

End Namespace
