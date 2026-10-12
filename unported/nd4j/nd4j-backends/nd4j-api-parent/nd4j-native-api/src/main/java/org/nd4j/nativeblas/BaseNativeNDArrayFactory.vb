Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports BaseNDArrayFactory = org.nd4j.linalg.factory.BaseNDArrayFactory
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection

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

Namespace org.nd4j.nativeblas


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseNativeNDArrayFactory extends org.nd4j.linalg.factory.BaseNDArrayFactory
	Public MustInherit Class BaseNativeNDArrayFactory
		Inherits BaseNDArrayFactory

		Protected Friend nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()

		Public Sub New(ByVal dtype As DataType, ByVal order As Char?)
			MyBase.New(dtype, order)
		End Sub

		Public Sub New(ByVal dtype As DataType, ByVal order As Char)
			MyBase.New(dtype, order)
		End Sub

		Public Sub New()
		End Sub



		Public Overrides Function convertToNumpy(ByVal array As INDArray) As Pointer
			Dim size As val = New LongPointer(1)
			Dim header As Pointer = NativeOpsHolder.Instance.getDeviceNativeOps().numpyHeaderForNd4j(array.data().pointer(), array.shapeInfoDataBuffer().pointer(), array.data().ElementSize,size)

			Dim headerSize As val = size.get() - 1
			header.capacity(headerSize)
			header.position(0)



			Dim bytePointer As New BytePointer(CInt(headerSize + (array.data().ElementSize * array.data().length())))
			Dim headerCast As New BytePointer(header)
			Dim indexer As val = ByteIndexer.create(headerCast)
			Dim pos As Integer = 0
			bytePointer.position(pos)
			Pointer.memcpy(bytePointer, headerCast,headerCast.capacity())
			pos += (headerCast.capacity())
			bytePointer.position(pos)

			' make sure data is copied to the host memory
			Nd4j.AffinityManager.ensureLocation(array, AffinityManager.Location.HOST)

			Pointer.memcpy(bytePointer,array.data().pointer(),(array.data().ElementSize * array.data().length()))
			bytePointer.position(0)
			Return bytePointer
		End Function

		''' <summary>
		''' Create from an in memory numpy pointer.
		''' Note that this is heavily used
		''' in our python library jumpy.
		''' </summary>
		''' <param name="pointer"> the pointer to the
		'''                numpy array </param>
		''' <returns> an ndarray created from the in memory
		''' numpy pointer </returns>
		Public Overrides Function createFromNpyPointer(ByVal pointer As Pointer) As INDArray
			Dim dataPointer As Pointer = nativeOps.dataPointForNumpy(pointer)
			Dim dataBufferElementSize As Integer = nativeOps.elementSizeForNpyArray(pointer)
			Dim data As DataBuffer = Nothing
			Dim shapeBufferPointer As Pointer = nativeOps.shapeBufferForNumpy(pointer)
			Dim length As Integer = nativeOps.lengthForShapeBufferPointer(shapeBufferPointer)
			shapeBufferPointer.capacity(8 * length)
			shapeBufferPointer.limit(8 * length)
			shapeBufferPointer.position(0)


			Dim intPointer As val = New LongPointer(shapeBufferPointer)
			Dim newPointer As val = New LongPointer(length)

			Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

			Pointer.memcpy(newPointer, intPointer, shapeBufferPointer.limit())

			PerformanceTracker.Instance.helperRegisterTransaction(0, perfD, shapeBufferPointer.limit(), MemcpyDirection.HOST_TO_HOST)

			Dim shapeBuffer As DataBuffer = Nd4j.createBuffer(newPointer, DataType.LONG, length, LongIndexer.create(newPointer))

			dataPointer.position(0)
			dataPointer.limit(dataBufferElementSize * Shape.length(shapeBuffer))
			dataPointer.capacity(dataBufferElementSize * Shape.length(shapeBuffer))

			Dim jvmShapeInfo As val = shapeBuffer.asLong()
			Dim dtype As val = ArrayOptionsHelper.dataType(jvmShapeInfo)

			Select Case dtype
				Case BOOL
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New BooleanPointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), BooleanIndexer.create(dPointer))
				Case UBYTE
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New BytePointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), UByteIndexer.create(dPointer))
				Case [BYTE]
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New BytePointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), ByteIndexer.create(dPointer))
				Case UINT64, [LONG]
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New LongPointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), LongIndexer.create(dPointer))
				Case UINT32, INT
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New IntPointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), IntIndexer.create(dPointer))
				Case UINT16
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New ShortPointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), UShortIndexer.create(dPointer))
				Case [SHORT]
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New ShortPointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), ShortIndexer.create(dPointer))
				Case BFLOAT16, HALF
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New ShortPointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), HalfIndexer.create(dPointer))
				Case FLOAT
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New FloatPointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), FloatIndexer.create(dPointer))
				Case [DOUBLE]
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New DoublePointer(dataPointer.limit() / dataBufferElementSize)
					Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), DoubleIndexer.create(dPointer))
			End Select

			Dim ret As INDArray = Nd4j.create(data, Shape.shape(shapeBuffer), Shape.strideArr(shapeBuffer), 0, Shape.order(shapeBuffer))

			Nd4j.AffinityManager.tagLocation(ret, AffinityManager.Location.DEVICE)

			Return ret
		End Function

		Public Overrides Function createFromNpyHeaderPointer(ByVal pointer As Pointer) As INDArray
			Dim dtype As val = DataType.fromInt(nativeOps.dataTypeFromNpyHeader(pointer))

			Dim dataPointer As Pointer = nativeOps.dataPointForNumpyHeader(pointer)
			Dim dataBufferElementSize As Integer = nativeOps.elementSizeForNpyArrayHeader(pointer)
			Dim data As DataBuffer = Nothing
			Dim shapeBufferPointer As Pointer = nativeOps.shapeBufferForNumpyHeader(pointer)
			Dim length As Integer = nativeOps.lengthForShapeBufferPointer(shapeBufferPointer)
			shapeBufferPointer.capacity(8 * length)
			shapeBufferPointer.limit(8 * length)
			shapeBufferPointer.position(0)


			Dim intPointer As val = New LongPointer(shapeBufferPointer)
			Dim newPointer As val = New LongPointer(length)

			Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

			Pointer.memcpy(newPointer, intPointer, shapeBufferPointer.limit())

			PerformanceTracker.Instance.helperRegisterTransaction(0, perfD, shapeBufferPointer.limit(), MemcpyDirection.HOST_TO_HOST)

			Dim shapeBuffer As DataBuffer = Nd4j.createBuffer(newPointer, DataType.LONG, length, LongIndexer.create(newPointer))

			dataPointer.position(0)
			dataPointer.limit(dataBufferElementSize * Shape.length(shapeBuffer))
			dataPointer.capacity(dataBufferElementSize * Shape.length(shapeBuffer))

			Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

			Select Case dtype
				Case [BYTE]
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New BytePointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), ByteIndexer.create(dPointer))
				Case [SHORT]
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New ShortPointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), ShortIndexer.create(dPointer))
				Case INT
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New IntPointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), IntIndexer.create(dPointer))
				Case [LONG]
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New LongPointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), LongIndexer.create(dPointer))
				Case UBYTE
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New BytePointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), UByteIndexer.create(dPointer))
				Case UINT16
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New ShortPointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), UShortIndexer.create(dPointer))
				Case UINT32
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New IntPointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), IntIndexer.create(dPointer))
				Case UINT64
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New LongPointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), LongIndexer.create(dPointer))
				Case HALF
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim dPointer As val = New ShortPointer(dataPointer.limit() / dataBufferElementSize)
						'dPointer.limit(dataPointer.limit() / dataBufferElementSize);
						'dPointer.capacity(dataPointer.limit() / dataBufferElementSize);
						Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

						data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), HalfIndexer.create(dPointer))
				Case FLOAT
					' TODO: we might want to skip copy, and use existing pointer/data here
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New FloatPointer(dataPointer.limit() / dataBufferElementSize)
					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), FloatIndexer.create(dPointer))
				Case [DOUBLE]
					' TODO: we might want to skip copy, and use existing pointer/data here
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As val = New DoublePointer(dataPointer.limit() / dataBufferElementSize)
					Pointer.memcpy(dPointer, dataPointer, dataPointer.limit())

					data = Nd4j.createBuffer(dPointer, dtype, Shape.length(shapeBuffer), DoubleIndexer.create(dPointer))
				Case Else
					Throw New Exception("Unsupported data type: [" & dtype & "]")
			End Select

			PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, dataPointer.limit(), MemcpyDirection.HOST_TO_HOST)

			Dim ret As INDArray = Nd4j.create(data, Shape.shape(shapeBuffer), Shape.strideArr(shapeBuffer), 0, Shape.order(shapeBuffer))

			Return ret
		End Function


		''' <summary>
		''' Create from a given numpy file.
		''' </summary>
		''' <param name="file"> the file to create the ndarray from </param>
		''' <returns> the created ndarray </returns>
		Public Overrides Function createFromNpyFile(ByVal file As File) As INDArray
			Dim pathBytes() As SByte = file.getAbsolutePath().getBytes(Charset.forName("UTF-8"))
			Dim directBuffer As ByteBuffer = ByteBuffer.allocateDirect(pathBytes.Length).order(ByteOrder.nativeOrder())
			directBuffer.put(pathBytes)
			CType(directBuffer, Buffer).rewind()
			CType(directBuffer, Buffer).position(0)
			Dim pointer As Pointer = nativeOps.numpyFromFile(New BytePointer(directBuffer))

			Dim result As INDArray = createFromNpyPointer(pointer)

			' releasing original pointer here
			nativeOps.releaseNumpy(pointer)
			Return result
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> createFromNpzFile(java.io.File file) throws Exception
		Public Overrides Function createFromNpzFile(ByVal file As File) As IDictionary(Of String, INDArray)

			' TODO error checks
			Dim map As New Dictionary(Of String, INDArray)()
			Dim [is] As Stream = New FileStream(file, FileMode.Open, FileAccess.Read)
			Do
				Dim localHeader(29) As SByte
				[is].Read(localHeader, 0, localHeader.Length)
				If CInt(localHeader(2)) <> 3 OrElse CInt(localHeader(3)) <> 4 Then
					If map.Count = 0 Then
						Throw New System.InvalidOperationException("Found malformed NZP file header: File is not a npz file? " & file.getPath())
					Else
						Exit Do
					End If
				End If
				Dim fNameLength As Integer = localHeader(26)
				Dim fNameBytes(fNameLength - 1) As SByte
				[is].Read(fNameBytes, 0, fNameBytes.Length)
				Dim fName As String = ""
				Dim i As Integer=0
				Do While i < fNameLength - 4
					fName &= ChrW(fNameBytes(i))
					i += 1
				Loop
				Dim extraFieldLength As Integer = localHeader(28)
				If extraFieldLength > 0 Then
					[is].Read(New SByte(extraFieldLength - 1){}, 0, New SByte(extraFieldLength - 1){}.Length)
				End If
				[is].Read(New SByte(10){}, 0, New SByte(10){}.Length)

				Dim headerStr As String = ""
				Dim b As Integer
				b = [is].Read()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while((b = is.read()) != ((int)ControlChars.Lf))
				Do While b <> (CInt(Math.Truncate(ControlChars.Lf)))
					headerStr &= ChrW(b)
						b = [is].Read()
				Loop

				Dim idx As Integer
				Dim typeStr As String
				If headerStr.Contains("<") Then
					idx = headerStr.IndexOf("'<", StringComparison.Ordinal) + 2
				Else
					idx = headerStr.IndexOf("'|", StringComparison.Ordinal) + 2
				End If
				typeStr = headerStr.Substring(idx, 2)

				Dim elemSize As Integer
				Dim dt As DataType
				If typeStr.Equals("f8") Then
					elemSize = 8
					dt = DataType.DOUBLE
				ElseIf typeStr.Equals("f4") Then
					elemSize = 4
					dt = DataType.FLOAT
				ElseIf typeStr.Equals("f2") Then
					elemSize = 2
					dt = DataType.HALF
				ElseIf typeStr.Equals("i8") Then
					elemSize = 8
					dt = DataType.LONG
				ElseIf typeStr.Equals("i4") Then
					elemSize = 4
					dt = DataType.INT
				ElseIf typeStr.Equals("i2") Then
					elemSize = 2
					dt = DataType.SHORT
				ElseIf typeStr.Equals("i1") Then
					elemSize = 1
					dt = DataType.BYTE
				ElseIf typeStr.Equals("u1") Then
					elemSize = 1
					dt = DataType.UBYTE
				Else
					Throw New Exception("Unsupported data type: " & typeStr)
				End If
				idx = headerStr.IndexOf("'fortran_order': ", StringComparison.Ordinal)
				Dim order As Char = If(headerStr.Chars(idx & "'fortran_order': ".Length) = "F"c, "c"c, "f"c)

								Dim tempVar = headerStr.indexOf("(") + 1
				Dim shapeStr As String = headerStr.Substring(tempVar, headerStr.IndexOf(")", StringComparison.Ordinal) - (tempVar))

				shapeStr = shapeStr.Replace(" ", "")
				Dim dims() As String = shapeStr.Split(",", True)
				Dim shape(dims.Length - 1) As Long
				Dim size As Long = 1
				For i As Integer = 0 To dims.Length - 1
					Dim d As Long = Long.Parse(dims(i))
					shape(i) = d
					size *= d
				Next i


				' TODO support long shape

				Dim numBytes As Integer = CInt(size * elemSize)
				Dim data(numBytes - 1) As SByte
				[is].Read(data, 0, data.Length)
				Dim bb As ByteBuffer = ByteBuffer.wrap(data)

				If dt = DataType.DOUBLE Then
					Dim doubleData(CInt(size) - 1) As Double
					For i As Integer = 0 To size - 1
						Dim l As Long = bb.getLong(8*i)
						l = Long.reverseBytes(l)
						doubleData(i) = Double.longBitsToDouble(l)
					Next i
					map(fName) = Nd4j.create(doubleData, shape, order)
				ElseIf dt = DataType.FLOAT Then
					Dim floatData(CInt(size) - 1) As Single
					For i As Integer = 0 To size - 1
						Dim i2 As Integer = bb.getInt(4*i)
						i2 = Integer.reverseBytes(i2)
						Dim f As Single = Float.intBitsToFloat(i2)
						floatData(i) = f
					Next i
					map(fName) = Nd4j.create(floatData, shape, order)
				ElseIf dt = DataType.HALF Then
					Dim arr As INDArray = Nd4j.create(DataType.HALF, size)
					Dim bb2 As ByteBuffer = arr.data().pointer().asByteBuffer()
					For i As Integer = 0 To size - 1
						Dim s As Short = bb.getShort(2*i)
						bb2.put(CSByte((s >> 8) And &Hff))
						bb2.put(CSByte(s And &Hff))
					Next i
					Nd4j.AffinityManager.tagLocation(arr, AffinityManager.Location.HOST)
					map(fName) = arr.reshape(order, shape)
				ElseIf dt = DataType.LONG Then
					Dim d(CInt(size) - 1) As Long
					For i As Integer = 0 To size - 1
						Dim l As Long = bb.getLong(8*i)
						l = Long.reverseBytes(l)
						d(i) = l
					Next i
					map(fName) = Nd4j.createFromArray(d).reshape(order, shape)
				ElseIf dt = DataType.INT Then
					Dim d(CInt(size) - 1) As Integer
					For i As Integer = 0 To size - 1
						Dim l As Integer = bb.getInt(4*i)
						l = Integer.reverseBytes(l)
						d(i) = l
					Next i
					map(fName) = Nd4j.createFromArray(d).reshape(order, shape)
				ElseIf dt = DataType.SHORT Then
					Dim d(CInt(size) - 1) As Short
					For i As Integer = 0 To size - 1
						Dim l As Short = bb.getShort(2*i)
						l = Short.reverseBytes(l)
						d(i) = l
					Next i
					map(fName) = Nd4j.createFromArray(d).reshape(order, shape)
				ElseIf dt = DataType.BYTE Then
					map(fName) = Nd4j.createFromArray(data).reshape(order, shape)
				ElseIf dt = DataType.UBYTE Then
					Dim d(CInt(size) - 1) As Short
					For i As Integer = 0 To size - 1
						Dim l As Short = (CShort(bb.get(i) And CShort(&Hff)))
						d(i) = l
					Next i
					map(fName) = Nd4j.createFromArray(d).reshape(order, shape).castTo(DataType.UBYTE)
				End If

			Loop

			Return map

		End Function
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> _createFromNpzFile(java.io.File file) throws Exception
		Public Overridable Function _createFromNpzFile(ByVal file As File) As IDictionary(Of String, INDArray)

			' TODO: Fix libnd4j implementation
			Dim pathBytes() As SByte = file.getAbsolutePath().getBytes(Charset.forName("UTF-8"))
			Dim directBuffer As ByteBuffer = ByteBuffer.allocateDirect(pathBytes.Length).order(ByteOrder.nativeOrder())
			directBuffer.put(pathBytes)
			CType(directBuffer, Buffer).rewind()
			CType(directBuffer, Buffer).position(0)
			Dim pointer As Pointer = nativeOps.mapFromNpzFile(New BytePointer(directBuffer))
			Dim n As Integer = nativeOps.getNumNpyArraysInMap(pointer)
			Dim map As New Dictionary(Of String, INDArray)()

			For i As Integer = 0 To n - 1
				'pre allocate 255 chars, only use up to null terminated
				'create a null terminated string buffer pre allocated for use
				'with the buffer
				Dim buffer(254) As SByte
				For j As Integer = 0 To buffer.Length - 1
					buffer(j) = ControlChars.NullChar
				Next j

				Dim charPointer As New BytePointer(buffer)
				Dim arrName As String = nativeOps.getNpyArrayNameFromMap(pointer, i,charPointer)
				Dim arrPtr As Pointer = nativeOps.getNpyArrayFromMap(pointer, i)
				Dim ndim As Integer = nativeOps.getNpyArrayRank(arrPtr)
				Dim shape(ndim - 1) As Long
				Dim shapePtr As LongPointer = nativeOps.getNpyArrayShape(arrPtr)

				Dim length As Long = 1
				For j As Integer = 0 To ndim - 1
					shape(j) = shapePtr.get(j)
					length *= shape(j)
				Next j

				Dim numBytes As Integer = nativeOps.getNpyArrayElemSize(arrPtr)

				Dim elemSize As Integer = numBytes * 8

				Dim order As Char = nativeOps.getNpyArrayOrder(arrPtr)

				Dim dataPointer As Pointer = nativeOps.dataPointForNumpyStruct(arrPtr)


				dataPointer.position(0)

				Dim size As Long = elemSize * length
				dataPointer.limit(size)
				dataPointer.capacity(size)

				Dim arr As INDArray
				If elemSize = (Len(New Single()) * 8) Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As New FloatPointer(dataPointer.limit() / elemSize)
					Dim data As DataBuffer = Nd4j.createBuffer(dPointer, DataType.FLOAT, length, FloatIndexer.create(dPointer))

					arr = Nd4j.create(data, shape, Nd4j.getStrides(shape, order), 0, order, DataType.FLOAT)

				ElseIf elemSize = (Len(New Double()) * 8) Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim dPointer As New DoublePointer(dataPointer.limit() / elemSize)
					Dim data As DataBuffer = Nd4j.createBuffer(dPointer, DataType.DOUBLE, length, DoubleIndexer.create(dPointer))
					arr = Nd4j.create(data, shape, Nd4j.getStrides(shape, order), 0, order, DataType.DOUBLE)

				Else
					Throw New Exception("Unsupported data type: " & elemSize.ToString())
				End If


				map(arrName) = arr
			Next i

			Return map

		End Function

	End Class

End Namespace