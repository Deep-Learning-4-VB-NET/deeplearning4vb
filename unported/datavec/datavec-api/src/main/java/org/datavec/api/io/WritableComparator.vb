Imports System
Imports System.Collections.Generic
Imports ReflectionUtils = org.datavec.api.util.ReflectionUtils
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.io




	Public Class WritableComparator
		Implements RawComparator

		Private Shared comparators As New Dictionary(Of Type, WritableComparator)() ' registry

		''' <summary>
		''' Get a comparator for a <seealso cref="WritableComparable"/> implementation. </summary>
		Public Shared Function get(ByVal c As Type) As WritableComparator
			SyncLock GetType(WritableComparator)
				Dim comparator As WritableComparator = comparators(c)
				If comparator Is Nothing Then
					' force the static initializers to run
					forceInit(c)
					' look to see if it is defined now
					comparator = comparators(c)
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
		''' Force initialization of the static members.
		''' As of Java 5, referencing a class doesn't force it to initialize. Since
		''' this class requires that the classes be initialized to declare their
		''' comparators, we force that initialization to happen. </summary>
		''' <param name="cls"> the class to initialize </param>
		Private Shared Sub forceInit(ByVal cls As Type)
			Try
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Type.GetType(cls.FullName, True, cls.getClassLoader())
			Catch e As ClassNotFoundException
				Throw New System.ArgumentException("Can't initialize class " & cls, e)
			End Try
		End Sub

		''' <summary>
		''' Register an optimized comparator for a <seealso cref="WritableComparable"/>
		''' implementation. 
		''' </summary>
		Public Shared Sub define(ByVal c As Type, ByVal comparator As WritableComparator)
			SyncLock GetType(WritableComparator)
				comparators(c) = comparator
			End SyncLock
		End Sub


'JAVA TO VB CONVERTER NOTE: The field keyClass was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly keyClass_Conflict As Type
		Private ReadOnly key1 As WritableComparable
		Private ReadOnly key2 As WritableComparable
		Private ReadOnly buffer As DataInputBuffer

		''' <summary>
		''' Construct for a <seealso cref="WritableComparable"/> implementation. </summary>
		Protected Friend Sub New(ByVal keyClass As Type)
			Me.New(keyClass, False)
		End Sub

		Protected Friend Sub New(ByVal keyClass As Type, ByVal createInstances As Boolean)
			Me.keyClass_Conflict = keyClass
			If createInstances Then
				key1 = newKey()
				key2 = newKey()
				buffer = New DataInputBuffer()
			Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: key1 = key2 = null;
				key2 = Nothing
					key1 = key2
				buffer = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns the WritableComparable implementation class. </summary>
		Public Overridable ReadOnly Property KeyClass As Type
			Get
				Return keyClass_Conflict
			End Get
		End Property

		''' <summary>
		''' Construct a new <seealso cref="WritableComparable"/> instance. </summary>
		Public Overridable Function newKey() As WritableComparable
			Return ReflectionUtils.newInstance(keyClass_Conflict, Nothing)
		End Function

		''' <summary>
		''' Optimization hook.  Override this to make SequenceFile.Sorter's scream.
		''' 
		''' <para>The default implementation reads the data into two {@link
		''' WritableComparable}s (using {@link
		''' Writable#readFields(DataInput)}, then calls {@link
		''' #compare(WritableComparable,WritableComparable)}.
		''' </para>
		''' </summary>
		Public Overridable Function compare(ByVal b1() As SByte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As SByte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
			Try
				buffer.reset(b1, s1, l1) ' parse key1
				key1.readFields(buffer)

				buffer.reset(b2, s2, l2) ' parse key2
				key2.readFields(buffer)

			Catch e As IOException
				Throw New Exception(e)
			End Try

			Return compare(key1, key2) ' compare them
		End Function

		''' <summary>
		''' Compare two WritableComparables.
		''' 
		''' <para> The default implementation uses the natural ordering, calling {@link
		''' Comparable#compareTo(Object)}. 
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public int compare(WritableComparable a, WritableComparable b)
		Public Overridable Function compare(ByVal a As WritableComparable, ByVal b As WritableComparable) As Integer
			Return a.compareTo(b)
		End Function

		Public Overridable Function compare(ByVal a As Object, ByVal b As Object) As Integer
			Return compare(DirectCast(a, WritableComparable), DirectCast(b, WritableComparable))
		End Function

		''' <summary>
		''' Lexicographic order of binary data. </summary>
		Public Shared Function compareBytes(ByVal b1() As SByte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As SByte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
			Dim end1 As Integer = s1 + l1
			Dim end2 As Integer = s2 + l2
			Dim i As Integer = s1
			Dim j As Integer = s2
			Do While i < end1 AndAlso j < end2
				Dim a As Integer = (b1(i) And &Hff)
				Dim b As Integer = (b2(j) And &Hff)
				If a <> b Then
					Return a - b
				End If
				i += 1
				j += 1
			Loop
			Return l1 - l2
		End Function

		''' <summary>
		''' Compute hash for binary data. </summary>
		Public Shared Function hashBytes(ByVal bytes() As SByte, ByVal offset As Integer, ByVal length As Integer) As Integer
			Dim hash As Integer = 1
			Dim i As Integer = offset
			Do While i < offset + length
				hash = (31 * hash) + CInt(bytes(i))
				i += 1
			Loop
			Return hash
		End Function

		''' <summary>
		''' Compute hash for binary data. </summary>
		Public Shared Function hashBytes(ByVal bytes() As SByte, ByVal length As Integer) As Integer
			Return hashBytes(bytes, 0, length)
		End Function

		''' <summary>
		''' Parse an unsigned short from a byte array. </summary>
		Public Shared Function readUnsignedShort(ByVal bytes() As SByte, ByVal start As Integer) As Integer
			Return (((bytes(start) And &Hff) << 8) + ((bytes(start + 1) And &Hff)))
		End Function

		''' <summary>
		''' Parse an integer from a byte array. </summary>
		Public Shared Function readInt(ByVal bytes() As SByte, ByVal start As Integer) As Integer
			Return (((bytes(start) And &Hff) << 24) + ((bytes(start + 1) And &Hff) << 16) + ((bytes(start + 2) And &Hff) << 8) + ((bytes(start + 3) And &Hff)))

		End Function

		''' <summary>
		''' Parse a float from a byte array. </summary>
		Public Shared Function readFloat(ByVal bytes() As SByte, ByVal start As Integer) As Single
			Return Float.intBitsToFloat(readInt(bytes, start))
		End Function

		''' <summary>
		''' Parse a long from a byte array. </summary>
		Public Shared Function readLong(ByVal bytes() As SByte, ByVal start As Integer) As Long
			Return (CLng(readInt(bytes, start)) << 32) + (readInt(bytes, start + 4) And &HFFFFFFFFL)
		End Function

		''' <summary>
		''' Parse a double from a byte array. </summary>
		Public Shared Function readDouble(ByVal bytes() As SByte, ByVal start As Integer) As Double
			Return Double.longBitsToDouble(readLong(bytes, start))
		End Function

		''' <summary>
		''' Reads a zero-compressed encoded long from a byte array and returns it. </summary>
		''' <param name="bytes"> byte array with decode long </param>
		''' <param name="start"> starting index </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> deserialized long </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static long readVLong(byte[] bytes, int start) throws java.io.IOException
		Public Shared Function readVLong(ByVal bytes() As SByte, ByVal start As Integer) As Long
			Dim len As Integer = bytes(start)
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
				i = i Or (bytes(start + 1 + idx) And &HFF)
			Next idx
			Return (If(isNegative, (Not i), i))
		End Function

		''' <summary>
		''' Reads a zero-compressed encoded integer from a byte array and returns it. </summary>
		''' <param name="bytes"> byte array with the encoded integer </param>
		''' <param name="start"> start index </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> deserialized integer </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int readVInt(byte[] bytes, int start) throws java.io.IOException
		Public Shared Function readVInt(ByVal bytes() As SByte, ByVal start As Integer) As Integer
			Return CInt(readVLong(bytes, start))
		End Function
	End Class

End Namespace