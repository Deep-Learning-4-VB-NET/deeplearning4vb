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
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.CompilerServices
Imports DeepLearning4VB.JavaCompat
#Disable Warning BC40000 ' ReflectionUtils is marked obsolete, as @Deprecated in Java
Imports ReflectionUtils = DeepLearning4VB.org.datavec.api.util.ReflectionUtils
#Enable Warning BC40000

Namespace org.datavec.api.io

    ''' <summary>
    ''' A comparator for <see cref="WritableComparable"/> values that can also compare their serialized
    ''' form. Optimized comparators are registered per class with <see cref="define"/> (the writables do
    ''' this in their shared constructors); <see cref="[get]"/> falls back to a generic comparator that
    ''' deserializes both keys.
    ''' </summary>
    Public Class WritableComparator
        Implements RawComparator(Of Object)

        Private Shared ReadOnly comparators As New Dictionary(Of Type, WritableComparator)() ' registry

        ''' <summary>
        ''' Get a comparator for a <seealso cref="WritableComparable"/> implementation. </summary>
        Public Shared Function [get](ByVal c As Type) As WritableComparator
            SyncLock comparators
                Dim comparator As WritableComparator = comparators.GetValueOrNull(c)
                If comparator Is Nothing Then
                    ' force the static initializers to run
                    forceInit(c)
                    ' look to see if it is defined now
                    comparator = comparators.GetValueOrNull(c)
                    ' if not, use the generic one
                    If comparator Is Nothing Then
                        comparator = New WritableComparator(c, True)
                        comparators(c) = comparator
                    End If
                End If
                Return comparator
            End SyncLock
        End Function

        ''' <summary>
        ''' Force initialization of the static members. Referencing a class does not run its shared
        ''' constructor; since the writables register their comparators there, we force it to run.
        ''' </summary>
        ''' <param name="cls"> the class to initialize </param>
        Private Shared Sub forceInit(ByVal cls As Type)
            Try
                RuntimeHelpers.RunClassConstructor(cls.TypeHandle)
            Catch e As TypeInitializationException
                Throw New ArgumentException("Can't initialize class " & cls.ToString(), e)
            End Try
        End Sub

        ''' <summary>
        ''' Register an optimized comparator for a <seealso cref="WritableComparable"/>
        ''' implementation.
        ''' </summary>
        Public Shared Sub define(ByVal c As Type, ByVal comparator As WritableComparator)
            SyncLock comparators
                comparators(c) = comparator
            End SyncLock
        End Sub

        Private ReadOnly keyClassField As Type
        Private ReadOnly key1 As WritableComparable
        Private ReadOnly key2 As WritableComparable
        Private ReadOnly buffer As DataInputBuffer

        ''' <summary>
        ''' Construct for a <seealso cref="WritableComparable"/> implementation. </summary>
        Protected Sub New(ByVal keyClass As Type)
            Me.New(keyClass, False)
        End Sub

        Protected Sub New(ByVal keyClass As Type, ByVal createInstances As Boolean)
            Me.keyClassField = keyClass
            If createInstances Then
                key1 = newKey()
                key2 = newKey()
                buffer = New DataInputBuffer()
            Else
                key1 = Nothing
                key2 = Nothing
                buffer = Nothing
            End If
        End Sub

        ''' <summary>
        ''' Returns the WritableComparable implementation class. </summary>
        Public Overridable ReadOnly Property KeyClass As Type
            Get
                Return keyClassField
            End Get
        End Property

        Public Function getKeyClass() As Type
            Return KeyClass
        End Function

        ''' <summary>
        ''' Construct a new <seealso cref="WritableComparable"/> instance. </summary>
        Public Overridable Function newKey() As WritableComparable
            Return ReflectionUtils.newInstance(Of WritableComparable)(keyClassField, Nothing)
        End Function

        ''' <summary>
        ''' Optimization hook.  Override this to make SequenceFile.Sorter's scream.
        '''
        ''' <para>The default implementation reads the data into two <see cref="WritableComparable"/>s
        ''' (using <see cref="org.datavec.api.writable.Writable.readFields"/>), then calls
        ''' <see cref="compare(WritableComparable, WritableComparable)"/>.
        ''' </para>
        ''' </summary>
        Public Overridable Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer Implements RawComparator(Of Object).compare
            Try
                buffer.reset(b1, s1, l1) ' parse key1
                key1.readFields(buffer)

                buffer.reset(b2, s2, l2) ' parse key2
                key2.readFields(buffer)

            Catch e As IOException
                Throw New InvalidOperationException(e.Message, e)
            End Try

            Return compare(key1, key2) ' compare them
        End Function

        ''' <summary>
        ''' Compare two WritableComparables.
        '''
        ''' <para> The default implementation uses the natural ordering, calling
        ''' <see cref="IComparable.CompareTo"/>.
        ''' </para>
        ''' </summary>
        Public Overridable Function compare(ByVal a As WritableComparable, ByVal b As WritableComparable) As Integer
            Return a.CompareTo(b)
        End Function

        Public Overridable Function compare(ByVal a As Object, ByVal b As Object) As Integer Implements IComparer(Of Object).Compare
            Return compare(DirectCast(a, WritableComparable), DirectCast(b, WritableComparable))
        End Function

        ''' <summary>
        ''' Lexicographic order of binary data. </summary>
        Public Shared Function compareBytes(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
            Dim end1 As Integer = s1 + l1
            Dim end2 As Integer = s2 + l2
            Dim i As Integer = s1
            Dim j As Integer = s2
            Do While i < end1 AndAlso j < end2
                Dim a As Integer = b1(i)
                Dim b As Integer = b2(j)
                If a <> b Then
                    Return a - b
                End If
                i += 1
                j += 1
            Loop
            Return l1 - l2
        End Function

        ''' <summary>
        ''' Compute hash for binary data (the Java hash, which uses the signed byte values). </summary>
        Public Shared Function hashBytes(ByVal bytes() As Byte, ByVal offset As Integer, ByVal length As Integer) As Integer
            Dim hash As Integer = 1
            Dim i As Integer = offset
            Do While i < offset + length
                hash = (31 * hash) + SignedByte(bytes(i))
                i += 1
            Loop
            Return hash
        End Function

        ''' <summary>
        ''' Compute hash for binary data. </summary>
        Public Shared Function hashBytes(ByVal bytes() As Byte, ByVal length As Integer) As Integer
            Return hashBytes(bytes, 0, length)
        End Function

        ''' <summary>
        ''' Parse an unsigned short from a byte array. </summary>
        Public Shared Function readUnsignedShort(ByVal bytes() As Byte, ByVal start As Integer) As Integer
            Return (CInt(bytes(start)) << 8) + CInt(bytes(start + 1))
        End Function

        ''' <summary>
        ''' Parse an integer from a byte array. </summary>
        Public Shared Function readInt(ByVal bytes() As Byte, ByVal start As Integer) As Integer
            Return (CInt(bytes(start)) << 24) + (CInt(bytes(start + 1)) << 16) + (CInt(bytes(start + 2)) << 8) + CInt(bytes(start + 3))
        End Function

        ''' <summary>
        ''' Parse a float from a byte array. </summary>
        Public Shared Function readFloat(ByVal bytes() As Byte, ByVal start As Integer) As Single
            Return BitConverter.ToSingle(BitConverter.GetBytes(readInt(bytes, start)), 0)
        End Function

        ''' <summary>
        ''' Parse a long from a byte array. </summary>
        Public Shared Function readLong(ByVal bytes() As Byte, ByVal start As Integer) As Long
            Return (CLng(readInt(bytes, start)) << 32) + (CLng(readInt(bytes, start + 4)) And &HFFFFFFFFL)
        End Function

        ''' <summary>
        ''' Parse a double from a byte array. </summary>
        Public Shared Function readDouble(ByVal bytes() As Byte, ByVal start As Integer) As Double
            Return BitConverter.Int64BitsToDouble(readLong(bytes, start))
        End Function

        ''' <summary>
        ''' Reads a zero-compressed encoded long from a byte array and returns it. </summary>
        ''' <param name="bytes"> byte array with decode long </param>
        ''' <param name="start"> starting index </param>
        ''' <exception cref="IOException"> </exception>
        ''' <returns> deserialized long </returns>
        Public Shared Function readVLong(ByVal bytes() As Byte, ByVal start As Integer) As Long
            Dim len As Integer = SignedByte(bytes(start))
            If len >= -112 Then
                Return len
            End If
            Dim isNegative As Boolean = (len < -120)
            len = If(isNegative, -(len + 120), -(len + 112))
            If start + 1 + len > bytes.Length Then
                Throw New IOException("Not enough number of bytes for a zero-compressed integer")
            End If
            Dim i As Long = 0
            For idx As Integer = 0 To len - 1
                i = i << 8
                i = i Or CLng(bytes(start + 1 + idx))
            Next idx
            Return (If(isNegative, (Not i), i))
        End Function

        ''' <summary>
        ''' Reads a zero-compressed encoded integer from a byte array and returns it. </summary>
        ''' <param name="bytes"> byte array with the encoded integer </param>
        ''' <param name="start"> start index </param>
        ''' <exception cref="IOException"> </exception>
        ''' <returns> deserialized integer </returns>
        Public Shared Function readVInt(ByVal bytes() As Byte, ByVal start As Integer) As Integer
            Return CInt(readVLong(bytes, start))
        End Function

        ''' <summary>The Java (signed) value of a byte.</summary>
        Private Shared Function SignedByte(ByVal b As Byte) As Integer
            Return If(b > 127, CInt(b) - 256, CInt(b))
        End Function
    End Class

End Namespace
