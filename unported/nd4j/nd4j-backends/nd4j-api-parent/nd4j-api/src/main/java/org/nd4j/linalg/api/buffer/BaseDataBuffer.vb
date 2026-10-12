Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports AtomicDouble = org.nd4j.common.primitives.AtomicDouble
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.buffer



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseDataBuffer implements DataBuffer
	<Serializable>
	Public MustInherit Class BaseDataBuffer
		Implements DataBuffer

		''' @deprecated Use <seealso cref="ND4JSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS"/> 
		Public Shared TO_STRING_MAX_ELEMENTS As String = ND4JSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS
		Private Shared TO_STRING_MAX As Integer
		Shared Sub New()
			Dim s As String = System.getProperty(ND4JSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS)
			If s IsNot Nothing Then
				Try
					TO_STRING_MAX = Integer.Parse(s)
				Catch e As System.FormatException
					log.warn("Invalid value for key {}: ""{}""", ND4JSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS, s)
					TO_STRING_MAX = 1000
				End Try
			Else
				TO_STRING_MAX = 1000
			End If
		End Sub

		Protected Friend type As DataType
'JAVA TO VB CONVERTER NOTE: The field length was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend length_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field underlyingLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend underlyingLength_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field offset was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend offset_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field elementSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend elementSize_Conflict As SByte
		'protected transient ByteBuffer wrappedBuffer;
		<NonSerialized>
		Protected Friend wrappedDataBuffer As DataBuffer
		<NonSerialized>
		Protected Friend workspaceGenerationId As Long = 0L

'JAVA TO VB CONVERTER NOTE: The field allocationMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend allocationMode_Conflict As AllocationMode

'JAVA TO VB CONVERTER NOTE: The field indexer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend indexer_Conflict As Indexer = Nothing
'JAVA TO VB CONVERTER NOTE: The field pointer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend pointer_Conflict As Pointer = Nothing

'JAVA TO VB CONVERTER NOTE: The field attached was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend attached_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field parentWorkspace was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend parentWorkspace_Conflict As MemoryWorkspace

		' Allocator-related stuff. Moved down here to avoid opType casting.
		<NonSerialized>
		Protected Friend originalBuffer As DataBuffer
'JAVA TO VB CONVERTER NOTE: The field originalOffset was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend originalOffset_Conflict As Long = 0

'JAVA TO VB CONVERTER NOTE: The field constant was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend constant_Conflict As Boolean = False
		<NonSerialized>
		Protected Friend released As Boolean = False

		<NonSerialized>
		Protected Friend referenced As New AtomicBoolean(False)
		'protected transient Collection<WeakReference<BaseDataBuffer>> references = new ArrayList<>();

		Public Sub New()
		End Sub

		''' <summary>
		''' Initialize the opType of this buffer
		''' </summary>
		Protected Friend MustOverride Sub initTypeAndSize()

		Public Overridable ReadOnly Property ElementSize As Integer Implements DataBuffer.getElementSize
			Get
				Return elementSize_Conflict
			End Get
		End Property


		Public Overridable ReadOnly Property GenerationId As Long Implements DataBuffer.getGenerationId
			Get
				If parentWorkspace_Conflict IsNot Nothing Then
					Return workspaceGenerationId
				ElseIf wrappedDataBuffer IsNot Nothing AndAlso wrappedDataBuffer.Attached Then
					Return wrappedDataBuffer.GenerationId
				ElseIf originalBuffer IsNot Nothing AndAlso originalBuffer.Attached Then
					Return originalBuffer.GenerationId
				End If
				Return workspaceGenerationId
			End Get
		End Property

		''' 
		''' <summary>
		''' Meant for creating another view of a buffer </summary>
		''' <param name="pointer"> the underlying buffer to create a view from </param>
		''' <param name="indexer"> the indexer for the pointer </param>
		''' <param name="length"> the length of the view </param>
		Public Sub New(ByVal pointer As Pointer, ByVal indexer As Indexer, ByVal length As Long)
			If length < 0 Then
				Throw New System.ArgumentException("Length must be >= 0")
			End If

			initTypeAndSize()
			Me.length_Conflict = length
			Me.allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES
			Me.underlyingLength_Conflict = length
			Me.wrappedDataBuffer = Me

			If length > 0 Then
				Me.pointer_Conflict = pointer
				Me.Indexer = indexer
			End If
		End Sub


		Protected Friend Overridable WriteOnly Property Indexer As Indexer
			Set(ByVal indexer As Indexer)
				Me.indexer_Conflict = indexer
			End Set
		End Property

		Protected Friend Overridable Sub pickReferent(ByVal referent As BaseDataBuffer)
			referenced.compareAndSet(False, True)
			'references.add(new WeakReference<BaseDataBuffer>(this));
		End Sub

		''' 
		''' <summary>
		''' Meant for creating another view of a buffer </summary>
		''' <param name="underlyingBuffer"> the underlying buffer to create a view from </param>
		''' <param name="length"> the length of the view </param>
		''' <param name="offset"> the offset for the view </param>
		Protected Friend Sub New(ByVal underlyingBuffer As DataBuffer, ByVal length As Long, ByVal offset As Long)
			If length < 0 Then
				Throw New System.ArgumentException("Length must be >= 0")
			End If

			If length = 0 Then
				length = 1
			End If



			initTypeAndSize()
			Me.length_Conflict = length
			Me.offset_Conflict = offset
			Me.allocationMode_Conflict = underlyingBuffer.allocationMode()
			Me.elementSize_Conflict = CSByte(underlyingBuffer.ElementSize)
			Me.underlyingLength_Conflict = underlyingBuffer.underlyingLength()
			Me.wrappedDataBuffer = underlyingBuffer

			' we're not referencing constant buffers
			If Not underlyingBuffer.Constant Then
				DirectCast(underlyingBuffer, BaseDataBuffer).pickReferent(Me)
			End If


			' Adding link to original databuffer
			If underlyingBuffer.originalDataBuffer() Is Nothing Then
				Me.originalBuffer = underlyingBuffer
				Me.originalOffset_Conflict = offset
			Else

				Me.originalBuffer = underlyingBuffer.originalDataBuffer()

				' FIXME: please don't remove this comment, since there's probably a bug in current offset() impl,
				' and this line will change originalOffset accroding to proper offset() impl
				' FIXME: raver119@gmail.com
				Me.originalOffset_Conflict = offset ' + underlyingBuffer.originalOffset();
			End If

			pointer_Conflict = underlyingBuffer.pointer()
			Indexer = underlyingBuffer.indexer()
		End Sub

		''' <summary>
		''' Original DataBuffer.
		''' In case if we have a view derived from another view, derived from some other view, original DataBuffer will point to the originating DataBuffer, where all views come from.
		''' </summary>
		Public Overridable Function originalDataBuffer() As DataBuffer Implements DataBuffer.originalDataBuffer
			Return originalBuffer
		End Function


		'sets the nio wrapped buffer (allows to be overridden for other use cases like cuda)
		Protected Friend Overridable Sub setNioBuffer()
			If elementSize_Conflict * length_Conflict >= Integer.MaxValue Then
				Throw New System.ArgumentException("Unable to create buffer of length " & length_Conflict)
			End If
			'wrappedBuffer = pointer().asByteBuffer();

		End Sub

		''' <summary>
		''' Returns the indexer for the buffer
		''' 
		''' @return
		''' </summary>
		Public Overridable Function indexer() As Indexer
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Return indexer_Conflict
		End Function

		Public Overridable Function pointer() As Pointer
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			If underlyingDataBuffer() IsNot Nothing AndAlso underlyingDataBuffer() IsNot Me Then
				If underlyingDataBuffer().wasClosed() Then
					Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
				End If

				Return underlyingDataBuffer().pointer()
			Else
				If underlyingDataBuffer() IsNot Nothing Then
					If DirectCast(underlyingDataBuffer(), BaseDataBuffer).released Then
						Throw New System.InvalidOperationException("Underlying buffer was released via close() call")
					End If
				End If

				If released Then
					Throw New System.InvalidOperationException("This buffer was already released via close() call")
				End If

				Return pointer_Conflict
			End If
		End Function

		Public Overridable Function underlyingDataBuffer() As DataBuffer Implements DataBuffer.underlyingDataBuffer
			Return wrappedDataBuffer
		End Function

		Public Overridable Function offset() As Long Implements DataBuffer.offset
			Return offset_Conflict
		End Function

		Public Overridable Function allocationMode() As AllocationMode Implements DataBuffer.allocationMode
			Return allocationMode_Conflict
		End Function

		<Obsolete>
		Public Overridable Sub persist() Implements DataBuffer.persist
			Throw New System.NotSupportedException()
		End Sub

		<Obsolete>
		Public Overridable ReadOnly Property Persist As Boolean Implements DataBuffer.isPersist
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		<Obsolete>
		Public Overridable Sub unPersist() Implements DataBuffer.unPersist
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overridable Sub fillPointerWithZero()
			Pointer.memset(Me.pointer(), 0, ElementSize * length())
		End Sub


		Public Overridable Sub copyAtStride(ByVal buf As DataBuffer, ByVal n As Long, ByVal stride As Long, ByVal yStride As Long, ByVal offset As Long, ByVal yOffset As Long) Implements DataBuffer.copyAtStride
			If dataType() = DataType.FLOAT Then
				For i As Integer = 0 To n - 1
					put(offset + i * stride, buf.getFloat(yOffset + i * yStride))
				Next i
			Else
				For i As Integer = 0 To n - 1
					put(offset + i * stride, buf.getDouble(yOffset + i * yStride))
				Next i
			End If

		End Sub

		<Obsolete>
		Public Overridable Sub removeReferencing(ByVal id As String) Implements DataBuffer.removeReferencing
			'referencing.remove(id);
		End Sub

		<Obsolete>
		Public Overridable Function references() As ICollection(Of String) Implements DataBuffer.references
			Throw New System.NotSupportedException()
			'return referencing;
		End Function

		Public MustOverride Function addressPointer() As Pointer

	'    
	'    @Override
	'    public Pointer addressPointer() {
	'        if (released)
	'            throw new IllegalStateException("You can't use DataBuffer once it was released");
	'
	'        if (offset() > 0) {
	'            Pointer ret;
	'            // offset is accounted at native side
	'            final long retAddress = pointer().address();
	'            // directly set address at construction since Pointer.address has not setter.
	'            if (dataType() == DataType.DOUBLE) {
	'                ret = new DoublePointer(pointer()) {
	'                    {
	'                        address = retAddress;
	'                    }
	'                };
	'            } else if (dataType() == DataType.FLOAT) {
	'                ret = new FloatPointer(pointer()) {
	'                    {
	'                        address = retAddress;
	'                    }
	'                };
	'            } else if (dataType() == DataType.INT) {
	'                ret = new IntPointer(pointer()) {
	'                    {
	'                        address = retAddress;
	'                    }
	'                };
	'            } else if (dataType() == DataType.LONG) {
	'                ret = new LongPointer(pointer()) {
	'                    {
	'                        address = retAddress;
	'                    }
	'                };
	'            } else {
	'                ret = new Pointer(pointer()) {
	'                    {
	'                        address = retAddress;
	'                    }
	'                };
	'            }
	'            ret.limit(ret.limit() - offset());
	'            ret.capacity(ret.capacity() - offset());
	'            return ret;
	'        }
	'        return pointer();
	'    }
	'    

		Public Overridable Function address() As Long Implements DataBuffer.address
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Return pointer().address()
		End Function

		<Obsolete>
		Public Overridable Sub addReferencing(ByVal id As String) Implements DataBuffer.addReferencing
			'referencing.add(id);
		End Sub

		Public Overridable Sub assign(ByVal indices() As Long, ByVal data() As Single, ByVal contiguous As Boolean, ByVal inc As Long) Implements DataBuffer.assign
			If indices.Length <> data.Length Then
				Throw New System.ArgumentException("Indices and data length must be the same")
			End If
			If indices.Length > length() Then
				Throw New System.ArgumentException("More elements than space to assign. This buffer is of length " & length() & " where the indices are of length " & data.Length)
			End If
			For i As Integer = 0 To indices.Length - 1
				put(indices(i), data(i))
			Next i
		End Sub



		Public Overridable WriteOnly Property Data Implements DataBuffer.setData As Integer()
			Set(ByVal data() As Integer)
				For i As Integer = 0 To data.Length - 1
					put(i, data(i))
				Next i
			End Set
		End Property

		Public Overridable WriteOnly Property Data Implements DataBuffer.setData As Single()
			Set(ByVal data() As Single)
				For i As Integer = 0 To data.Length - 1
					put(i, data(i))
				Next i
			End Set
		End Property

		Public Overridable WriteOnly Property Data Implements DataBuffer.setData As Double()
			Set(ByVal data() As Double)
				For i As Integer = 0 To data.Length - 1
					put(i, data(i))
				Next i
			End Set
		End Property

		Public Overridable WriteOnly Property Data Implements DataBuffer.setData As Long()
			Set(ByVal data() As Long)
				For i As Integer = 0 To data.Length - 1
					put(i, data(i))
				Next i
			End Set
		End Property

		Public Overridable WriteOnly Property Data Implements DataBuffer.setData As SByte()
			Set(ByVal data() As SByte)
				For i As Integer = 0 To data.Length - 1
					put(i, data(i))
				Next i
			End Set
		End Property

		Public Overridable WriteOnly Property Data Implements DataBuffer.setData As Short()
			Set(ByVal data() As Short)
				For i As Integer = 0 To data.Length - 1
					put(i, data(i))
				Next i
			End Set
		End Property

		Public Overridable WriteOnly Property Data Implements DataBuffer.setData As Boolean()
			Set(ByVal data() As Boolean)
				For i As Integer = 0 To data.Length - 1
					put(i, data(i))
				Next i
			End Set
		End Property

		Public Overridable Sub assign(ByVal indices() As Long, ByVal data() As Double, ByVal contiguous As Boolean, ByVal inc As Long) Implements DataBuffer.assign
			If indices.Length <> data.Length Then
				Throw New System.ArgumentException("Indices and data length must be the same")
			End If
			If indices.Length > length() Then
				Throw New System.ArgumentException("More elements than space to assign. This buffer is of length " & length() & " where the indices are of length " & data.Length)
			End If
			For i As Integer = 0 To indices.Length - 1 Step inc
				put(indices(i), data(i))
			Next i
		End Sub

		Public Overridable Sub assign(ByVal data As DataBuffer) Implements DataBuffer.assign
			If data.length() <> length() Then
				Throw New System.ArgumentException("Unable to assign buffer of length " & data.length() & " to this buffer of length " & length())
			End If

			For i As Integer = 0 To data.length() - 1
				put(i, data.getDouble(i))
			Next i
		End Sub

		Public Overridable Sub assign(ByVal indices() As Long, ByVal data() As Single, ByVal contiguous As Boolean) Implements DataBuffer.assign
			assign(indices, data, contiguous, 1)
		End Sub

		Public Overridable Sub assign(ByVal indices() As Long, ByVal data() As Double, ByVal contiguous As Boolean) Implements DataBuffer.assign
			assign(indices, data, contiguous, 1)
		End Sub

		Public Overridable Function underlyingLength() As Long Implements DataBuffer.underlyingLength
			Return underlyingLength_Conflict
		End Function

		Public Overridable Function length() As Long Implements DataBuffer.length
			Return length_Conflict
		End Function

		Public Overridable Sub assign(ByVal value As Number) Implements DataBuffer.assign
			assign(value, 0)
		End Sub

		Public Overridable Function getDoublesAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Double() Implements DataBuffer.getDoublesAt
			If offset + length > Me.length() Then
				length -= offset
			End If

			Dim ret(length - 1) As Double
			For i As Integer = 0 To length - 1
				ret(i) = getDouble(i * inc + offset)
			Next i
			Return ret
		End Function

		Public Overridable Function getDoublesAt(ByVal offset As Long, ByVal length As Integer) As Double() Implements DataBuffer.getDoublesAt
			Return getDoublesAt(offset, 1, length)
		End Function

		Public Overridable Function getFloatsAt(ByVal offset As Long, ByVal length As Integer) As Single() Implements DataBuffer.getFloatsAt
			Return getFloatsAt(offset, 1, length)
		End Function

		Public Overridable Function getFloatsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Single() Implements DataBuffer.getFloatsAt
			If offset + length > Me.length() Then
				length -= offset
			End If
			Dim ret(length - 1) As Single
			For i As Integer = 0 To length - 1
				ret(i) = getFloat(i * inc + offset)
			Next i
			Return ret
		End Function

		Public Overridable Function getLongsAt(ByVal offset As Long, ByVal length As Integer) As Long() Implements DataBuffer.getLongsAt
			Return getLongsAt(offset, 1, length)
		End Function

		Public Overridable Function getLongsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Long() Implements DataBuffer.getLongsAt
			If offset + length > Me.length() Then
				length -= offset
			End If
			Dim ret(length - 1) As Long
			For i As Integer = 0 To length - 1
				ret(i) = getLong(i * inc + offset)
			Next i
			Return ret
		End Function

		Public Overridable Function getIntsAt(ByVal offset As Long, ByVal length As Integer) As Integer() Implements DataBuffer.getIntsAt
			Return getIntsAt(offset, 1, length)
		End Function

		Public Overridable Function getIntsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Integer() Implements DataBuffer.getIntsAt
			If offset + length > Me.length() Then
				length -= offset
			End If
			Dim ret(length - 1) As Integer
			For i As Integer = 0 To length - 1
				ret(i) = getInt(i * inc + offset)
			Next i
			Return ret
		End Function

		Public Overridable Function dup() As DataBuffer Implements DataBuffer.dup
			Dim ret As DataBuffer = create(length_Conflict)
			Dim i As Integer = 0
			Do While i < ret.length()
				ret.put(i, getDouble(i))
				i += 1
			Loop

			Return ret
		End Function



		''' <summary>
		''' Create with length </summary>
		''' <param name="length"> a databuffer of the same opType as
		'''               this with the given length </param>
		''' <returns> a data buffer with the same length and datatype as this one </returns>
		Protected Friend MustOverride Function create(ByVal length As Long) As DataBuffer


		''' <summary>
		''' Create the data buffer
		''' with respect to the given byte buffer </summary>
		''' <param name="data"> the buffer to create </param>
		''' <returns> the data buffer based on the given buffer </returns>
		Public MustOverride Function create(ByVal data() As Double) As DataBuffer

		''' <summary>
		''' Create the data buffer
		''' with respect to the given byte buffer </summary>
		''' <param name="data"> the buffer to create </param>
		''' <returns> the data buffer based on the given buffer </returns>
		Public MustOverride Function create(ByVal data() As Single) As DataBuffer

		''' <summary>
		''' Create the data buffer
		''' with respect to the given byte buffer </summary>
		''' <param name="data"> the buffer to create </param>
		''' <returns> the data buffer based on the given buffer </returns>
		Public MustOverride Function create(ByVal data() As Integer) As DataBuffer


		Public Overridable Sub assign(ByVal offsets() As Long, ByVal strides() As Long, ParamArray ByVal buffers() As DataBuffer) Implements DataBuffer.assign
			assign(offsets, strides, length(), buffers)
		End Sub

		Public Overridable Function asBytes() As SByte() Implements DataBuffer.asBytes
			'NOTE: DataOutputStream is big endian
			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			Dim dataType As val = Me.dataType()
			Select Case dataType
				Case [DOUBLE]
					Try
						Dim i As Integer = 0
						Do While i < length()
							dos.writeDouble(getDouble(i))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case FLOAT
					Try
						Dim i As Integer = 0
						Do While i < length()
							dos.writeFloat(getFloat(i))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case HALF
					Try
						Dim i As Integer = 0
						Do While i < length()
							dos.writeShort(HalfIndexer.fromFloat(getFloat(i)))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case BOOL
					Try
						Dim i As Integer = 0
						Do While i < length()
							dos.writeByte(If(getInt(i) = 0, CSByte(0), CSByte(1)))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case [BYTE]
					Try
						Dim i As Integer = 0
						Do While i < length()
							dos.writeByte(CSByte(getShort(i)))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case UBYTE
					Try
						Dim u As UByteIndexer = CType(indexer_Conflict, UByteIndexer)
						Dim i As Integer = 0
						Do While i < length()
							dos.writeByte(u.get(i))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case [SHORT]
					Try
						Dim i As Integer = 0
						Do While i < length()
								dos.writeShort(getShort(i))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case INT
					Try
						Dim i As Integer = 0
						Do While i < length()
							dos.writeInt(getInt(i))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case [LONG]
					Try
						Dim i As Integer = 0
						Do While i < length()
							dos.writeLong(getLong(i))
							i += 1
						Loop
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case BFLOAT16, UINT16
					'Treat BFloat16 and UINT16 as bytes
					Dim temp(CInt(2*length_Conflict) - 1) As SByte
					asNio().get(temp)
					Try
						If ByteOrder.nativeOrder().Equals(ByteOrder.LITTLE_ENDIAN) Then
							'Switch endianness to big endian
							For i As Integer = 0 To (temp.Length \ 2) - 1
								dos.write(temp(2 * i + 1))
								dos.write(temp(2 * i))
							Next i
						Else
							'Keep as big endian
							dos.write(temp)
						End If
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case UINT64
					'Treat unsigned long (UINT64) as 8 bytes
					Dim temp2(CInt(8*length_Conflict) - 1) As SByte
					asNio().get(temp2)
					Try
						If ByteOrder.nativeOrder().Equals(ByteOrder.LITTLE_ENDIAN) Then
							'Switch endianness to big endian
							For i As Integer = 0 To (temp2.Length \ 8) - 1
								For j As Integer = 0 To 7
									dos.write(temp2(8 * i + (7-j)))
								Next j
							Next i
						Else
							'Keep as big endian
							dos.write(temp2)
						End If
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case UINT32
					'Treat unsigned integer (UINT32) as 4 bytes
					Dim temp3(CInt(4*length_Conflict) - 1) As SByte
					asNio().get(temp3)
					Try
						If ByteOrder.nativeOrder().Equals(ByteOrder.LITTLE_ENDIAN) Then
							'Switch endianness to big endian
							For i As Integer = 0 To (temp3.Length \ 4) - 1
								For j As Integer = 0 To 3
									dos.write(temp3(4 * i + (3-j)))
								Next j
							Next i
						Else
							'Keep as big endian
							dos.write(temp3)
						End If
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case UTF8
					Dim temp4(CInt(length_Conflict) - 1) As SByte
					asNio().get(temp4)
					Try
						dos.write(temp4)
					Catch e As IOException
						Throw New Exception(e)
					End Try
				Case Else
					Throw New System.NotSupportedException("Unknown data type: [" & dataType & "]")
			End Select
			Return bos.toByteArray()
		End Function

		Public Overridable Function asFloat() As Single() Implements DataBuffer.asFloat
			If length_Conflict >= Integer.MaxValue Then
				Throw New System.ArgumentException("Unable to create array of length " & length_Conflict)
			End If
			Dim ret(CInt(length_Conflict) - 1) As Single
			For i As Integer = 0 To length_Conflict - 1
				ret(i) = getFloatUnsynced(i)
			Next i
			Return ret
		End Function

		Public Overridable Function asDouble() As Double() Implements DataBuffer.asDouble
			If length_Conflict >= Integer.MaxValue Then
				Throw New System.ArgumentException("Unable to create array of length " & length_Conflict)
			End If
			Dim ret(CInt(length_Conflict) - 1) As Double
			For i As Integer = 0 To length_Conflict - 1
				ret(i) = getDoubleUnsynced(i)
			Next i
			Return ret
		End Function

		Public Overridable Function asInt() As Integer() Implements DataBuffer.asInt
			If length_Conflict >= Integer.MaxValue Then
				Throw New System.ArgumentException("Unable to create array of length " & length_Conflict)
			End If
			Dim ret(CInt(length_Conflict) - 1) As Integer
			For i As Integer = 0 To length_Conflict - 1
				ret(i) = getIntUnsynced(i)
			Next i
			Return ret
		End Function

		Public Overridable Function asLong() As Long() Implements DataBuffer.asLong
			If length_Conflict >= Integer.MaxValue Then
				Throw New System.ArgumentException("Unable to create array of length " & length_Conflict)
			End If
			Dim ret(CInt(length_Conflict) - 1) As Long
			For i As Integer = 0 To length_Conflict - 1
				ret(i) = getLongUnsynced(i)
			Next i
			Return ret
		End Function

		Public Overridable Function getDouble(ByVal i As Long) As Double Implements DataBuffer.getDouble
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			If indexer_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Indexer must never be null")
			End If
			Select Case dataType()
				Case FLOAT
					Return CType(indexer_Conflict, FloatIndexer).get(i)
				Case UINT32
					Return CType(indexer_Conflict, UIntIndexer).get(i)
				Case INT
					Return CType(indexer_Conflict, IntIndexer).get(i)
				Case BFLOAT16
					Return CType(indexer_Conflict, Bfloat16Indexer).get(i)
				Case HALF
					Return CType(indexer_Conflict, HalfIndexer).get(i)
				Case UINT16
					Return CType(indexer_Conflict, UShortIndexer).get(i)
				Case [SHORT]
					Return CType(indexer_Conflict, ShortIndexer).get(i)
				Case UINT64, [LONG]
					Return CType(indexer_Conflict, LongIndexer).get(i)
				Case BOOL
					Return If(CType(indexer_Conflict, BooleanIndexer).get(i), 1.0, 0.0)
				Case [DOUBLE]
					Return CType(indexer_Conflict, DoubleIndexer).get(i)
				Case [BYTE]
					Return CType(indexer_Conflict, ByteIndexer).get(i)
				Case UBYTE
					Return CType(indexer_Conflict, UByteIndexer).get(i)
				Case Else
					Throw New System.NotSupportedException("Cannot get double value from buffer of type " & dataType())
			End Select
		End Function

		Public Overridable Function getLong(ByVal i As Long) As Long Implements DataBuffer.getLong
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case FLOAT
					Return CLng(Math.Truncate(CType(indexer_Conflict, FloatIndexer).get(i)))
				Case [DOUBLE]
					Return CLng(Math.Truncate(CType(indexer_Conflict, DoubleIndexer).get(i)))
				Case BFLOAT16
					Return CLng(Math.Truncate(CType(indexer_Conflict, Bfloat16Indexer).get(i)))
				Case HALF
					Return CLng(Math.Truncate(CType(indexer_Conflict, HalfIndexer).get(i)))
				Case UINT64, [LONG] 'Fall through
					Return CType(indexer_Conflict, LongIndexer).get(i)
				Case UINT32
					Return CLng(Math.Truncate(CType(indexer_Conflict, UIntIndexer).get(i)))
				Case INT
					Return CLng(Math.Truncate(CType(indexer_Conflict, IntIndexer).get(i)))
				Case UINT16
					Return CLng(Math.Truncate(CType(indexer_Conflict, UShortIndexer).get(i)))
				Case [SHORT]
					Return CLng(Math.Truncate(CType(indexer_Conflict, ShortIndexer).get(i)))
				Case [BYTE]
					Return CLng(Math.Truncate(CType(indexer_Conflict, ByteIndexer).get(i)))
				Case UBYTE
					Return CLng(Math.Truncate(CType(indexer_Conflict, UByteIndexer).get(i)))
				Case BOOL
					Return If(CType(indexer_Conflict, BooleanIndexer).get(i), 1L, 0L)
				Case Else
					Throw New System.NotSupportedException("Cannot get long value from buffer of type " & dataType())
			End Select
		End Function

		''' <summary>
		''' Special method for </summary>
		''' <param name="i">
		''' @return </param>
		Protected Friend Overridable Function getShort(ByVal i As Long) As Short
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case [DOUBLE]
					Return CShort(Math.Truncate(CType(indexer_Conflict, DoubleIndexer).get(i)))
				Case BFLOAT16
					Return CShort(Math.Truncate(CType(indexer_Conflict, Bfloat16Indexer).get(i)))
				Case HALF
					Return CShort(Math.Truncate(CType(indexer_Conflict, HalfIndexer).get(i)))
				Case BOOL
					Return CShort(If(CType(indexer_Conflict, BooleanIndexer).get(i), 1, 0))
				Case UINT32
					Return CShort(Math.Truncate(CType(indexer_Conflict, UIntIndexer).get(i)))
				Case INT
					Return CShort(Math.Truncate(CType(indexer_Conflict, IntIndexer).get(i)))
				Case UINT16, [SHORT]
					Return CType(indexer_Conflict, ShortIndexer).get(i)
				Case [BYTE]
					Return CShort(Math.Truncate(CType(indexer_Conflict, ByteIndexer).get(i)))
				Case UINT64, [LONG]
					Return CShort(Math.Truncate(CType(indexer_Conflict, LongIndexer).get(i)))
				Case FLOAT
					Return CShort(Math.Truncate(CType(indexer_Conflict, FloatIndexer).get(i)))
				Case Else
					Throw New System.NotSupportedException("Cannot get short value from buffer of type " & dataType())
			End Select
		End Function

		''' 
		''' <param name="v">
		''' @return </param>
		Public Shared Function fromFloat(ByVal v As Single) As Short
			Return ArrayUtil.fromFloat(v)
		End Function

		Public Overridable Function getFloat(ByVal i As Long) As Single Implements DataBuffer.getFloat
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case [DOUBLE]
					Return CSng(CType(indexer_Conflict, DoubleIndexer).get(i))
				Case BOOL
					Return If(CType(indexer_Conflict, BooleanIndexer).get(i), 1.0f, 0.0f)
				Case UINT32
					Return CSng(CType(indexer_Conflict, UIntIndexer).get(i))
				Case INT
					Return CSng(CType(indexer_Conflict, IntIndexer).get(i))
				Case UINT16
					Return CType(indexer_Conflict, UShortIndexer).get(i)
				Case [SHORT]
					Return CSng(CType(indexer_Conflict, ShortIndexer).get(i))
				Case BFLOAT16
					Return CType(indexer_Conflict, Bfloat16Indexer).get(i)
				Case HALF
					Return CType(indexer_Conflict, HalfIndexer).get(i)
				Case UBYTE
					Return CSng(CType(indexer_Conflict, UByteIndexer).get(i))
				Case [BYTE]
					Return CSng(CType(indexer_Conflict, ByteIndexer).get(i))
				Case UINT64, [LONG] 'Fall through
					Return CSng(CType(indexer_Conflict, LongIndexer).get(i))
				Case FLOAT
					Return CType(indexer_Conflict, FloatIndexer).get(i)
				Case Else
					Throw New System.NotSupportedException("Cannot get float value from buffer of type " & dataType())
			End Select
		End Function

		Public Overridable Function getInt(ByVal i As Long) As Integer Implements DataBuffer.getInt
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case [DOUBLE]
					Return CInt(Math.Truncate(CType(indexer_Conflict, DoubleIndexer).get(i)))
				Case BOOL
					Return If(CType(indexer_Conflict, BooleanIndexer).get(i), 1, 0)
				Case UINT32
					Return CInt(Math.Truncate(CType(indexer_Conflict, UIntIndexer).get(i)))
				Case INT
					Return CType(indexer_Conflict, IntIndexer).get(i)
				Case BFLOAT16
					Return CInt(Math.Truncate(CType(indexer_Conflict, Bfloat16Indexer).get(i)))
				Case HALF
					Return CInt(Math.Truncate(CType(indexer_Conflict, HalfIndexer).get(i)))
				Case UINT16
					Return CType(indexer_Conflict, UShortIndexer).get(i)
				Case [SHORT]
					Return CType(indexer_Conflict, ShortIndexer).get(i)
				Case UBYTE
					Return CType(indexer_Conflict, UByteIndexer).get(i)
				Case [BYTE]
					Return CType(indexer_Conflict, ByteIndexer).get(i)
				Case UINT64, [LONG] 'Fall through
					Return CInt(Math.Truncate(CType(indexer_Conflict, LongIndexer).get(i)))
				Case FLOAT
					Return CInt(Math.Truncate(CType(indexer_Conflict, FloatIndexer).get(i)))
				Case Else
					Throw New System.NotSupportedException("Cannot get integer value from buffer of type " & dataType())
			End Select
		End Function

		Public Overridable Function getNumber(ByVal i As Long) As Number Implements DataBuffer.getNumber
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			If dataType() = DataType.DOUBLE Then
				Return getDouble(i)
			ElseIf dataType() = DataType.INT Then
				Return getInt(i)
			ElseIf dataType() = DataType.LONG Then
				Return getLong(i)
			End If
			Return getFloat(i)
		End Function

		Public MustOverride Sub pointerIndexerByCurrentType(ByVal currentType As DataType)

		Public Overridable Sub putByDestinationType(ByVal i As Long, ByVal element As Number, ByVal globalType As DataType)
			If globalType = DataType.INT OrElse type = DataType.INT OrElse globalType = DataType.UINT16 OrElse globalType = DataType.UBYTE OrElse globalType = DataType.SHORT OrElse globalType = DataType.BYTE OrElse globalType = DataType.BOOL Then
				Dim anElement As Integer = element.intValue()
				put(i, anElement)
			ElseIf globalType = DataType.LONG OrElse type = DataType.LONG OrElse globalType = DataType.UINT32 OrElse globalType = DataType.UINT64 Then
				Dim anElement As Long = element.longValue()
				put(i, anElement)
			ElseIf globalType = DataType.FLOAT OrElse globalType = DataType.HALF OrElse globalType = DataType.BFLOAT16 Then
				Dim anElement As Single = element.floatValue()
				put(i, anElement)
			ElseIf globalType = DataType.DOUBLE Then
				Dim anElement As Double = element.doubleValue()
				put(i, anElement)
			Else
				Throw New System.InvalidOperationException("Unknown type: " & globalType)
			End If
		End Sub

		Public Overridable Sub put(ByVal i As Long, ByVal element As Single) Implements DataBuffer.put
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case BOOL
					CType(indexer_Conflict, BooleanIndexer).put(i,If(element = 0.0, False, True))
				Case [BYTE]
					CType(indexer_Conflict, ByteIndexer).put(i, CSByte(Math.Truncate(element)))
				Case UBYTE
					CType(indexer_Conflict, UByteIndexer).put(i, CInt(Math.Truncate(element)))
				Case UINT16
					CType(indexer_Conflict, UShortIndexer).put(i, CInt(Math.Truncate(element)))
				Case [SHORT]
					CType(indexer_Conflict, ShortIndexer).put(i, CShort(Math.Truncate(element)))
				Case UINT32
					CType(indexer_Conflict, UIntIndexer).put(i, CLng(Math.Truncate(element)))
				Case INT
					CType(indexer_Conflict, IntIndexer).put(i, CInt(Math.Truncate(element)))
				Case UINT64, [LONG]
					CType(indexer_Conflict, LongIndexer).put(i, CLng(Math.Truncate(element)))
				Case BFLOAT16
					CType(indexer_Conflict, Bfloat16Indexer).put(i, element)
				Case HALF
					CType(indexer_Conflict, HalfIndexer).put(i, element)
				Case FLOAT
					CType(indexer_Conflict, FloatIndexer).put(i, element)
				Case [DOUBLE]
					CType(indexer_Conflict, DoubleIndexer).put(i, element)
				Case Else
					Throw New System.InvalidOperationException("Unsupported type: " & dataType())
			End Select
		End Sub

		Public Overridable Sub put(ByVal i As Long, ByVal element As Double) Implements DataBuffer.put
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case BOOL
					CType(indexer_Conflict, BooleanIndexer).put(i, element > 0.0)
				Case [BYTE]
					CType(indexer_Conflict, ByteIndexer).put(i, CSByte(Math.Truncate(element)))
				Case UBYTE
					CType(indexer_Conflict, UByteIndexer).put(i, CShort(Math.Truncate(element)))
				Case UINT16
					CType(indexer_Conflict, UShortIndexer).put(i, CInt(Math.Truncate(element)))
				Case [SHORT]
					CType(indexer_Conflict, ShortIndexer).put(i, CShort(Math.Truncate(element)))
				Case UINT32
					CType(indexer_Conflict, UIntIndexer).put(i, CLng(Math.Truncate(element)))
				Case INT
					CType(indexer_Conflict, IntIndexer).put(i, CInt(Math.Truncate(element)))
				Case UINT64, [LONG]
					CType(indexer_Conflict, LongIndexer).put(i, CLng(Math.Truncate(element)))
				Case BFLOAT16
					CType(indexer_Conflict, Bfloat16Indexer).put(i, CSng(element))
				Case HALF
					CType(indexer_Conflict, HalfIndexer).put(i, CSng(element))
				Case FLOAT
					CType(indexer_Conflict, FloatIndexer).put(i, CSng(element))
				Case [DOUBLE]
					CType(indexer_Conflict, DoubleIndexer).put(i, element)
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType())
			End Select
		End Sub

		Public Overridable Sub put(ByVal i As Long, ByVal element As Integer) Implements DataBuffer.put
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case BOOL
					CType(indexer_Conflict, BooleanIndexer).put(i,If(element = 0, False, True))
				Case [BYTE]
					CType(indexer_Conflict, ByteIndexer).put(i, CSByte(element))
				Case UBYTE
					CType(indexer_Conflict, UByteIndexer).put(i, element)
				Case UINT16
					CType(indexer_Conflict, UShortIndexer).put(i, element)
				Case [SHORT]
					CType(indexer_Conflict, ShortIndexer).put(i, CShort(element))
				Case UINT32
					CType(indexer_Conflict, UIntIndexer).put(i, element)
				Case INT
					CType(indexer_Conflict, IntIndexer).put(i, element)
				Case UINT64, [LONG] 'Fall through
					CType(indexer_Conflict, LongIndexer).put(i, element)
				Case BFLOAT16
					CType(indexer_Conflict, Bfloat16Indexer).put(i, element)
				Case HALF
					CType(indexer_Conflict, HalfIndexer).put(i, element)
				Case FLOAT
					CType(indexer_Conflict, FloatIndexer).put(i, element)
				Case [DOUBLE]
					CType(indexer_Conflict, DoubleIndexer).put(i, element)
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType())
			End Select
		End Sub

		Public Overridable Sub put(ByVal i As Long, ByVal element As Boolean) Implements DataBuffer.put
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case BOOL
					CType(indexer_Conflict, BooleanIndexer).put(i, element)
				Case [BYTE]
					CType(indexer_Conflict, ByteIndexer).put(i,If(element, CSByte(1), CSByte(0)))
				Case UBYTE
					CType(indexer_Conflict, UByteIndexer).put(i,If(element, CSByte(1), CSByte(0)))
				Case UINT16
					CType(indexer_Conflict, UShortIndexer).put(i,If(element, 1, 0))
				Case [SHORT]
					CType(indexer_Conflict, ShortIndexer).put(i,If(element, CShort(1), CShort(0)))
				Case UINT32
					CType(indexer_Conflict, UIntIndexer).put(i,If(element, 1, 0))
				Case INT
					CType(indexer_Conflict, IntIndexer).put(i,If(element, 1, 0))
				Case UINT64, [LONG]
					CType(indexer_Conflict, LongIndexer).put(i,If(element, 1, 0))
				Case BFLOAT16
					CType(indexer_Conflict, Bfloat16Indexer).put(i,If(element, 1.0f, 0.0f))
				Case HALF
					CType(indexer_Conflict, HalfIndexer).put(i,If(element, 1.0f, 0.0f))
				Case FLOAT
					CType(indexer_Conflict, FloatIndexer).put(i,If(element, 1.0f, 0.0f))
				Case [DOUBLE]
					CType(indexer_Conflict, DoubleIndexer).put(i,If(element, 1.0, 0.0))
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType())
			End Select
		End Sub

		Public Overridable Sub put(ByVal i As Long, ByVal element As Long) Implements DataBuffer.put
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Select Case dataType()
				Case BOOL
					CType(indexer_Conflict, BooleanIndexer).put(i,If(element = 0, False, True))
				Case [BYTE]
					CType(indexer_Conflict, ByteIndexer).put(i, CSByte(element))
				Case UBYTE
					CType(indexer_Conflict, UByteIndexer).put(i, CShort(element))
				Case UINT16
					CType(indexer_Conflict, UShortIndexer).put(i, CInt(element))
				Case [SHORT]
					CType(indexer_Conflict, ShortIndexer).put(i, CShort(element))
				Case UINT32
					CType(indexer_Conflict, UIntIndexer).put(i, element)
				Case INT
					CType(indexer_Conflict, IntIndexer).put(i, CInt(element))
				Case UINT64, [LONG]
					CType(indexer_Conflict, LongIndexer).put(i, element)
				Case BFLOAT16
					CType(indexer_Conflict, Bfloat16Indexer).put(i, CSng(element))
				Case HALF
					CType(indexer_Conflict, HalfIndexer).put(i, CSng(element))
				Case FLOAT
					CType(indexer_Conflict, FloatIndexer).put(i, CSng(element))
				Case [DOUBLE]
					CType(indexer_Conflict, DoubleIndexer).put(i, CDbl(element))
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType())
			End Select
		End Sub

		<Obsolete>
		Public Overridable Function dirty() As Boolean Implements DataBuffer.dirty
			Return False
		End Function

		Public Overridable Function sameUnderlyingData(ByVal buffer As DataBuffer) As Boolean Implements DataBuffer.sameUnderlyingData
			Return pointer() = buffer.pointer()
		End Function

		Protected Friend Overridable Function wrappedBuffer() As ByteBuffer
			Return pointer().asByteBuffer()
		End Function

		Public Overridable Function asNioInt() As IntBuffer
			If offset() >= Integer.MaxValue Then
				Throw New System.InvalidOperationException("Index out of bounds " & offset())
			End If

			If offset() = 0 Then
				Return wrappedBuffer().asIntBuffer()
			Else
				Return CType(wrappedBuffer().asIntBuffer().position(CInt(offset())), IntBuffer)
			End If
		End Function

		Public Overridable Function asNioLong() As LongBuffer
			If offset() >= Integer.MaxValue Then
				Throw New System.InvalidOperationException("Index out of bounds " & offset())
			End If

			If offset() = 0 Then
				Return wrappedBuffer().asLongBuffer()
			Else
				Return CType(wrappedBuffer().asLongBuffer().position(CInt(offset())), LongBuffer)
			End If
		End Function

		Public Overridable Function asNioDouble() As DoubleBuffer
			If offset() >= Integer.MaxValue Then
				Throw New System.InvalidOperationException("Index out of bounds " & offset())
			End If

			If offset() = 0 Then
				Return wrappedBuffer().asDoubleBuffer()
			Else
				Return CType(wrappedBuffer().asDoubleBuffer().position(CInt(offset())), DoubleBuffer)
			End If
		End Function

		Public Overridable Function asNioFloat() As FloatBuffer
			If offset() >= Integer.MaxValue Then
				Throw New System.InvalidOperationException("Index out of bounds " & offset())
			End If

			If offset() = 0 Then
				Return wrappedBuffer().asFloatBuffer()
			Else
				Return CType(wrappedBuffer().asFloatBuffer().position(CInt(offset())), FloatBuffer)
			End If

		End Function

		Public Overridable Function asNio() As ByteBuffer
			Return wrappedBuffer()
		End Function

		Public Overridable Sub assign(ByVal value As Number, ByVal offset As Long) Implements DataBuffer.assign
			'note here that the final put will take care of the offset
			Dim i As Long = offset
			Do While i < length()
				put(i, value.doubleValue())
				i += 1
			Loop
		End Sub

		Public Overridable Sub write(ByVal dos As Stream) Implements DataBuffer.write
			If TypeOf dos Is DataOutputStream Then
				Try
					write(CType(dos, DataOutputStream))
				Catch e As IOException
					Throw New System.InvalidOperationException("IO Exception writing buffer", e)
				End Try
			Else
				Dim dos2 As New DataOutputStream(dos)
				Try

					write(dos2)
				Catch e As IOException
					Throw New System.InvalidOperationException("IO Exception writing buffer", e)
				End Try
			End If

		End Sub

		Public Overridable Sub read(ByVal [is] As Stream, ByVal allocationMode As AllocationMode, ByVal length As Long, ByVal dataType As DataType) Implements DataBuffer.read
			If TypeOf [is] Is DataInputStream Then
				read(CType([is], DataInputStream), allocationMode, length, dataType)

			Else
				Dim dis2 As New DataInputStream([is])
				read(dis2, allocationMode, length, dataType)
			End If
		End Sub

		Public Overridable Sub flush() Implements DataBuffer.flush

		End Sub

		Public Overridable Sub assign(ByVal offsets() As Long, ByVal strides() As Long, ByVal n As Long, ParamArray ByVal buffers() As DataBuffer) Implements DataBuffer.assign
			If offsets.Length <> strides.Length OrElse strides.Length <> buffers.Length Then
				Throw New System.ArgumentException("Unable to assign buffers, please specify equal lengths strides, offsets, and buffers")
			End If
			Dim count As Integer = 0
			For i As Integer = 0 To buffers.Length - 1
				'note here that the final put will take care of the offset
				Dim j As Long = offsets(i)
				Do While j < buffers(i).length()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: put(count++, buffers[i].getDouble(j));
					put(count, buffers(i).getDouble(j))
						count += 1
					j += strides(i)
				Loop
			Next i

			If count <> n Then
				Throw New System.ArgumentException("Strides and offsets didn't match up to length " & n)
			End If

		End Sub

		Public Overridable Sub assign(ParamArray ByVal buffers() As DataBuffer) Implements DataBuffer.assign
			Dim offsets(buffers.Length - 1) As Long
			Dim strides(buffers.Length - 1) As Long
			For i As Integer = 0 To strides.Length - 1
				strides(i) = 1
			Next i
			assign(offsets, strides, buffers)
		End Sub


		Public Overridable Sub destroy() Implements DataBuffer.destroy

		End Sub

		''' <summary>
		''' The data opType of the buffer
		''' </summary>
		''' <returns> the data opType of the buffer </returns>
		Public Overridable Function dataType() As DataType Implements DataBuffer.dataType
			Return type
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			' FIXME: this is BAD. it takes too long to work, and it breaks general equals contract
			If TypeOf o Is DataBuffer Then
				Dim d As DataBuffer = DirectCast(o, DataBuffer)
				If d.length() <> length() Then
					Return False
				End If
				Dim i As Integer = 0
				Do While i < length()
					Dim eps As Double = Math.Abs(getDouble(i) - d.getDouble(i))
					If eps > 1e-12 Then
						Return False
					End If
					i += 1
				Loop
			End If

			Return True
		End Function

		Private Sub readObject(ByVal s As ObjectInputStream)
			doReadObject(s)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(ObjectOutputStream out) throws IOException
		Private Sub writeObject(ByVal [out] As ObjectOutputStream)
			[out].defaultWriteObject()
			write([out])
		End Sub


		Protected Friend Overridable Sub doReadObject(ByVal s As ObjectInputStream)
			Try
				s.defaultReadObject()
				Dim header As val = BaseDataBuffer.readHeader(s)
				read(s, header.getLeft(), header.getMiddle(), header.getRight())
			Catch e As Exception
				Throw New Exception(e)
			End Try


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Triple<AllocationMode, Long, DataType> readHeader(@NonNull InputStream is)
		Public Shared Function readHeader(ByVal [is] As Stream) As Triple(Of AllocationMode, Long, DataType)
			Try
				Dim dis As DataInputStream = If(TypeOf [is] Is DataInputStream, CType([is], DataInputStream), New DataInputStream([is]))
				Dim alloc As val = System.Enum.Parse(GetType(AllocationMode), dis.readUTF())
				Dim length As Long = 0
				If alloc.ordinal() < 3 Then
					length = dis.readInt()
				Else
					length = dis.readLong()
				End If
				Dim type As val = DataType.valueOf(dis.readUTF())

				Return Triple.tripleOf(alloc, length, type)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void read(DataInputStream s, @NonNull AllocationMode allocMode, long len, @NonNull DataType dtype)
		Public Overridable Sub read(ByVal s As DataInputStream, ByVal allocMode As AllocationMode, ByVal len As Long, ByVal dtype As DataType) Implements DataBuffer.read
			Try
				'referencing = Collections.synchronizedSet(new HashSet<String>());
				Dim savedMode As val = allocMode
				Me.allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES
				type = dtype
				length_Conflict = len

				' old AllocationMode values are: DIRECT, HEAP, JAVACPP. Just using legacy here
				If savedMode.ordinal() < 3 Then
					'Do an implicit conversion: keep current buffer data type unchanged, and convert values from source type
					length_Conflict = len
					Dim sourceType As DataType = dtype
					pointerIndexerByCurrentType(type) 'also updates indexer based on newly set length

					If sourceType <> DataType.COMPRESSED Then
						Dim thisType As DataType = dataType()
						readContent(s, sourceType, thisType)
					End If

					' we should switch types here

					'wrappedBuffer = pointer().asByteBuffer();

				ElseIf savedMode.Equals(AllocationMode.LONG_SHAPE) Then
					length_Conflict = len
					Dim currentType As val = dtype
					type = currentType

					If currentType = DataType.LONG Then
						elementSize_Conflict = 8
					ElseIf currentType = DataType.DOUBLE AndAlso currentType <> DataType.INT Then
						elementSize_Conflict = 8
					ElseIf currentType = DataType.FLOAT OrElse currentType = DataType.INT Then
						elementSize_Conflict = 4
					ElseIf currentType = DataType.HALF AndAlso currentType <> DataType.INT Then
						elementSize_Conflict = 2
					End If

					pointerIndexerByCurrentType(currentType)

					If currentType <> DataType.COMPRESSED Then
						readContent(s, currentType, currentType)
					End If
				ElseIf allocationMode_Conflict.Equals(AllocationMode.MIXED_DATA_TYPES) Then
					Select Case type.innerEnumValue
						Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64, [LONG], [DOUBLE]
							elementSize_Conflict = 8
						Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32, FLOAT, INT
							elementSize_Conflict = 4
						Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16, [SHORT], HALF, BFLOAT16
							elementSize_Conflict = 2
						Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL, [BYTE], UBYTE, UTF8
							elementSize_Conflict = 1
						Case Else
							Throw New System.NotSupportedException()
					End Select

					pointerIndexerByCurrentType(type)

					If type <> DataType.COMPRESSED Then
						readContent(s, type, type)
					End If
				End If
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		Protected Friend Overridable Sub readContent(ByVal s As DataInputStream, ByVal sourceType As DataType, ByVal thisType As DataType)
			Try
				'Use AtomicX as a mutable Number class to reduce garbage vs. auto boxing to Double/Float etc classes
				If sourceType = DataType.DOUBLE Then
					Dim aDbl As New AtomicDouble()
					Dim i As Long = 0
					Do While i < length()
						aDbl.set(s.readDouble())
						putByDestinationType(i, aDbl, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.FLOAT Then
					'TODO no AtomicFloat to use here?
					Dim i As Long = 0
					Do While i < length()
						putByDestinationType(i, s.readFloat(), thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.COMPRESSED Then
					Dim compressionAlgorithm As String = s.readUTF()
					Dim compressedLength As Long = s.readLong()
					Dim originalLength As Long = s.readLong()
					Dim numberOfElements As Long = s.readLong()

					pointer_Conflict = New BytePointer(compressedLength)
					type = DataType.COMPRESSED
					Dim tp As val = CType(pointer_Conflict, BytePointer)
					Dim ti As val = ByteIndexer.create(tp)

					For i As Long = 0 To compressedLength - 1
						ti.put(i, s.readByte())
					Next i

				ElseIf sourceType = DataType.HALF Then
					Dim aInt As New AtomicInteger()
					Dim i As Long = 0
					Do While i < length()
						aInt.set(s.readShort())
						putByDestinationType(i, HalfIndexer.toFloat(aInt.get()), thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.BFLOAT16 Then
					Dim aInt As New AtomicInteger()
					Dim i As Long = 0
					Do While i < length()
						aInt.set(s.readShort())
						putByDestinationType(i, Bfloat16Indexer.toFloat(aInt.get()), thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.UINT64 Then
					Dim aLong As New AtomicLong()
					Dim i As Long = 0
					Do While i < length()
						aLong.set(s.readLong())
						putByDestinationType(i, aLong, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.LONG Then
					Dim aLong As New AtomicLong()
					Dim i As Long = 0
					Do While i < length()
						aLong.set(s.readLong())
						putByDestinationType(i, aLong, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.UINT32 Then
					Dim aLong As New AtomicLong()
					Dim i As Long = 0
					Do While i < length()
						aLong.set(s.readInt())
						putByDestinationType(i, aLong, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.INT Then
					Dim aInt As New AtomicInteger()
					Dim i As Long = 0
					Do While i < length()
						aInt.set(s.readInt())
						putByDestinationType(i, aInt, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.UINT16 Then
					Dim aInt As New AtomicInteger()
					Dim i As Long = 0
					Do While i < length()
						aInt.set(s.readShort())
						putByDestinationType(i, aInt, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.SHORT Then
					Dim aInt As New AtomicInteger()
					Dim i As Long = 0
					Do While i < length()
						aInt.set(s.readShort())
						putByDestinationType(i, aInt, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.UBYTE Then
					Dim aInt As New AtomicInteger()
					Dim i As Long = 0
					Do While i < length()
						aInt.set(s.readByte())
						putByDestinationType(i, aInt, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.BYTE Then
					Dim aInt As New AtomicInteger()
					Dim i As Long = 0
					Do While i < length()
						aInt.set(s.readByte())
						putByDestinationType(i, aInt, thisType)
						i += 1
					Loop
				ElseIf sourceType = DataType.BOOL Then
					Dim aInt As New AtomicInteger()
					Dim i As Long = 0
					Do While i < length()
						aInt.set(s.readByte())
						putByDestinationType(i, aInt, thisType)
						i += 1
					Loop
				Else
					Throw New System.NotSupportedException("Cannot read type: " & sourceType & " to " & thisType)
				End If
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End Sub

		Protected Friend MustOverride Function getDoubleUnsynced(ByVal index As Long) As Double
		Protected Friend MustOverride Function getFloatUnsynced(ByVal index As Long) As Single
		Protected Friend MustOverride Function getLongUnsynced(ByVal index As Long) As Long
		Protected Friend MustOverride Function getIntUnsynced(ByVal index As Long) As Integer

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(DataOutputStream out) throws IOException
		Public Overridable Sub write(ByVal [out] As DataOutputStream) Implements DataBuffer.write
			[out].writeUTF(allocationMode_Conflict.ToString())
			[out].writeLong(length())
			[out].writeUTF(dataType().name())
			Select Case dataType()
				Case [DOUBLE]
					Dim i As Long = 0
					Do While i < length()
						[out].writeDouble(getDoubleUnsynced(i))
						i += 1
					Loop
				Case UINT64, [LONG]
					Dim i As Long = 0
					Do While i < length()
						[out].writeLong(getLongUnsynced(i))
						i += 1
					Loop
				Case UINT32, INT
					Dim i As Long = 0
					Do While i < length()
						[out].writeInt(getIntUnsynced(i))
						i += 1
					Loop
				Case UINT16, [SHORT]
					Dim i As Long = 0
					Do While i < length()
						[out].writeShort(CShort(getIntUnsynced(i)))
						i += 1
					Loop
				Case UBYTE, [BYTE]
					Dim i As Long = 0
					Do While i < length()
						[out].writeByte(CSByte(getIntUnsynced(i)))
						i += 1
					Loop
				Case BOOL
					Dim i As Long = 0
					Do While i < length()
						[out].writeByte(If(getIntUnsynced(i) = 0, CSByte(0), CSByte(1)))
						i += 1
					Loop
				Case BFLOAT16
					Dim i As Long = 0
					Do While i < length()
						[out].writeShort(CShort(Math.Truncate(Bfloat16Indexer.fromFloat(getFloatUnsynced(i)))))
						i += 1
					Loop
				Case HALF
					Dim i As Long = 0
					Do While i < length()
						[out].writeShort(CShort(Math.Truncate(HalfIndexer.fromFloat(getFloatUnsynced(i)))))
						i += 1
					Loop
				Case FLOAT
					Dim i As Long = 0
					Do While i < length()
						[out].writeFloat(getFloatUnsynced(i))
						i += 1
					Loop
			End Select
		End Sub

		Public Overridable Function toFloat(ByVal hbits As Integer) As Single
			Dim mant As Integer = hbits And &H3ff ' 10 bits mantissa
			Dim exp As Integer = hbits And &H7c00 ' 5 bits exponent
			If exp = &H7c00 Then ' NaN/Inf
				exp = &H3fc00 ' -> NaN/Inf
			ElseIf exp <> 0 Then ' normalized value
				exp += &H1c000 ' exp - 15 + 127
				' "smooth transition" is nonstandard behavior
				'            if( mant == 0 && exp > 0x1c400 )  // smooth transition
				'                return Float.intBitsToFloat( ( hbits & 0x8000 ) << 16
				'                                                | exp << 13 | 0x3ff );
			ElseIf mant <> 0 Then ' && exp==0 -> subnormal
				exp = &H1c400 ' make it normal
				Do
					mant <<= 1 ' mantissa * 2
					exp -= &H400 ' decrease exp by 1
				Loop While (mant And &H400) = 0 ' while not normal
				mant = mant And &H3ff ' discard subnormal bit
			End If ' else +/-0 -> +/-0
			Return Float.intBitsToFloat((hbits And &H8000) << 16 Or (exp Or mant) << 13) ' value << ( 23 - 10 )
		End Function


		Public Overridable Function array() As Object Implements DataBuffer.array
			Return Nothing
		End Function

		Public Overrides Function ToString() As String
			Dim ret As New StringBuilder()
			ret.Append("[")

			Dim max As Integer
			If TO_STRING_MAX >= 0 Then
				max = CInt(Math.Min(length(), TO_STRING_MAX))
			Else
				max = CInt(Math.Min(length(), Integer.MaxValue))
			End If

			For i As Integer = 0 To max - 1
				Select Case dataType()
					Case UBYTE, [BYTE], INT, [SHORT], [LONG]
						ret.Append(getNumber(i).intValue())
					Case BOOL
						ret.Append(If(getNumber(i).intValue() = 0, " false", " true"))
					Case UTF8
						Throw New System.NotSupportedException()
					Case Else
						ret.Append(getNumber(i).floatValue())
				End Select
				If i < max - 1 Then
					ret.Append(",")
				End If
			Next i
			If max < length() Then
				ret.Append(",<").Append(length()-max).Append(" more elements>")
			End If
			ret.Append("]")

			Return ret.ToString()
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = CInt(length_Conflict)
			'result = 31 * result + (referencing != null ? referencing.hashCode() : 0);
			'result = 31 * result + (isPersist ? 1 : 0);
			result = 31 * result + (If(allocationMode_Conflict <> Nothing, allocationMode_Conflict.GetHashCode(), 0))
			Return result
		End Function

		''' <summary>
		''' Returns the offset of the buffer relative to originalDataBuffer
		''' 
		''' @return
		''' </summary>
		Public Overridable Function originalOffset() As Long Implements DataBuffer.originalOffset
			Return originalOffset_Conflict
		End Function

		''' <summary>
		''' This method returns whether this DataBuffer is constant, or not.
		''' Constant buffer means that it modified only during creation time, and then it stays the same for all lifecycle. I.e. used in shape info databuffers.
		''' 
		''' @return
		''' </summary>
		Public Overridable Property Constant As Boolean Implements DataBuffer.isConstant
			Get
				Return constant_Conflict
			End Get
			Set(ByVal reallyConstant As Boolean)
				Me.constant_Conflict = reallyConstant
			End Set
		End Property


		''' <summary>
		''' This method returns True, if this DataBuffer is attached to some workspace. False otherwise
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Attached As Boolean Implements DataBuffer.isAttached
			Get
				Return attached_Conflict
			End Get
		End Property


		''' <summary>
		''' This method checks, if given attached INDArray is still in scope of its parent Workspace
		''' <para>
		''' PLEASE NOTE: if this INDArray isn't attached to any Workspace, this method will return true
		''' 
		''' @return
		''' </para>
		''' </summary>
		Public Overridable ReadOnly Property InScope As Boolean Implements DataBuffer.isInScope
			Get
				If Not Attached Then
					Return True
				End If
    
				Return parentWorkspace_Conflict.ScopeActive
			End Get
		End Property


		Public Overridable ReadOnly Property ParentWorkspace As MemoryWorkspace Implements DataBuffer.getParentWorkspace
			Get
				If parentWorkspace_Conflict IsNot Nothing Then
					Return parentWorkspace_Conflict
				End If
				If wrappedDataBuffer IsNot Nothing AndAlso wrappedDataBuffer.Attached AndAlso wrappedDataBuffer.ParentWorkspace IsNot Nothing Then
					Return wrappedDataBuffer.ParentWorkspace
				End If
				If originalBuffer IsNot Nothing AndAlso originalBuffer.Attached AndAlso originalBuffer.ParentWorkspace IsNot Nothing Then
					Return originalBuffer.ParentWorkspace
				End If
				Return Nothing
			End Get
		End Property

		Public MustOverride Function reallocate(ByVal length As Long) As DataBuffer Implements DataBuffer.reallocate

		''' <returns> the capacity of the buffer
		'''  </returns>
		Public Overridable Function capacity() As Long Implements DataBuffer.capacity
			Return pointer().capacity()
		End Function

		Public Overridable Function closeable() As Boolean Implements DataBuffer.closeable
			If released OrElse Attached OrElse Constant Then
				Return False
			End If

			If wrappedDataBuffer IsNot Nothing AndAlso wrappedDataBuffer IsNot Me Then
				Return False
			End If

			Return True
		End Function

		Protected Friend Overridable Sub markReleased()
			Me.released = True
	'
	'        for (val r:references) {
	'            val b = r.get();
	'
	'            if (b != null)
	'                b.markReleased();
	'        }
	'        
		End Sub

		Public Overridable Sub close() Implements DataBuffer.close
			If Not closeable() Then
				Throw New System.InvalidOperationException("Can't release this data buffer")
			End If

			' notifying other databuffers that their underlying
	'        
	'        for (val r:references) {
	'
	'            val b = r.get();
	'
	'            if (b != null)
	'                b.markReleased();
	'        }
	'         

			release()
		End Sub

		Protected Friend Overridable Sub release()
			Me.released = True
			Me.indexer_Conflict = Nothing
			Me.pointer_Conflict = Nothing
		End Sub

		Public Overridable Function platformAddress() As Long Implements DataBuffer.platformAddress
			Return address()
		End Function


		Public Overridable Function wasClosed() As Boolean Implements DataBuffer.wasClosed
			If wrappedDataBuffer IsNot Nothing AndAlso wrappedDataBuffer IsNot Me Then
				Return wrappedDataBuffer.wasClosed()
			End If

			Return released
		End Function


		''' <summary>
		''' This method synchronizes host memory
		''' </summary>
		Public MustOverride Sub syncToPrimary()

		''' <summary>
		''' This method synchronizes device memory
		''' </summary>
		Public MustOverride Sub syncToSpecial()


	End Class

End Namespace