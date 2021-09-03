Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeEx = org.nd4j.linalg.api.buffer.DataTypeEx
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports Flatten = org.nd4j.linalg.api.ops.custom.Flatten
Imports Concat = org.nd4j.linalg.api.ops.impl.shape.Concat
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = org.nd4j.linalg.api.shape.options.ArrayType
Imports CompressionUtils = org.nd4j.linalg.compression.CompressionUtils
Imports org.nd4j.linalg.jcublas.buffer
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports org.nd4j.common.primitives
Imports org.bytedeco.javacpp
Imports CudaConstants = org.nd4j.jita.allocator.enums.CudaConstants
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports TADManager = org.nd4j.linalg.cache.TADManager
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor
Imports CompressionType = org.nd4j.linalg.compression.CompressionType
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.linalg.jcublas.blas
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports org.nd4j.nativeblas

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

Namespace org.nd4j.linalg.jcublas


	''' <summary>
	''' Jcublas ndarray factory. Handles creation of
	''' jcuda.jcublas ndarrays.
	''' 
	''' @author mjk
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JCublasNDArrayFactory extends BaseNativeNDArrayFactory
	Public Class JCublasNDArrayFactory
		Inherits BaseNativeNDArrayFactory


		Public Sub New()
		End Sub

		Public Sub New(ByVal dtype As DataType, ByVal order As Char?)
			MyBase.New(dtype, order)
		End Sub

		Public Sub New(ByVal dtype As DataType, ByVal order As Char)
			MyBase.New(dtype, order)
			AtomicAllocator.Instance
		End Sub

		Public Overrides Sub createBlas()
			blas_Conflict = New CudaBlas()
			Dim functions As New PointerPointer(13)
			functions.put(0, Loader.addressof("cublasSgemv_v2"))
			functions.put(1, Loader.addressof("cublasDgemv_v2"))
			functions.put(2, Loader.addressof("cublasHgemm"))
			functions.put(3, Loader.addressof("cublasSgemm_v2"))
			functions.put(4, Loader.addressof("cublasDgemm_v2"))
			functions.put(5, Loader.addressof("cublasSgemmEx"))
			functions.put(6, Loader.addressof("cublasHgemmBatched"))
			functions.put(7, Loader.addressof("cublasSgemmBatched"))
			functions.put(8, Loader.addressof("cublasDgemmBatched"))
			functions.put(9, Loader.addressof("cusolverDnSgesvd_bufferSize"))
			functions.put(10, Loader.addressof("cusolverDnDgesvd_bufferSize"))
			functions.put(11, Loader.addressof("cusolverDnSgesvd"))
			functions.put(12, Loader.addressof("cusolverDnDgesvd"))
			nativeOps.initializeFunctions(functions)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If
	'
	'        val major = new int[1];
	'        val minor = new int[1];
	'        val build = new int[1];
	'        org.bytedeco.cuda.global.cublas.cublasGetProperty(0, major);
	'        org.bytedeco.cuda.global.cublas.cublasGetProperty(1, minor);
	'        org.bytedeco.cuda.global.cublas.cublasGetProperty(2, build);
	'
	'        val pew = new int[100];
	'        org.bytedeco.cuda.global.cudart.cudaDriverGetVersion(pew);
	'
	'        nativeOps.isBlasVersionMatches(major[0], minor[0], build[0]);
	'
	'        if (nativeOps.lastErrorCode() != 0)
	'            throw new RuntimeException(nativeOps.lastErrorMessage());
	'        
		End Sub

		Public Overrides Sub createLevel1()
			level1_Conflict = New JcublasLevel1()
		End Sub

		Public Overrides Sub createLevel2()
			level2_Conflict = New JcublasLevel2()
		End Sub

		Public Overrides Sub createLevel3()
			level3_Conflict = New JcublasLevel3()
		End Sub

		Public Overrides Sub createLapack()
			lapack_Conflict = New JcublasLapack()
		End Sub

		Public Overrides Function create(ByVal shape() As Integer, ByVal buffer As DataBuffer) As INDArray
			Return New JCublasNDArray(shape, buffer)
		End Function

		''' <summary>
		''' Create an ndarray with the given data layout
		''' </summary>
		''' <param name="data"> the data to create the ndarray with </param>
		''' <returns> the ndarray with the given data layout </returns>
		Public Overrides Function create(ByVal data()() As Double) As INDArray
			Return New JCublasNDArray(data)
		End Function

		Public Overrides Function create(ByVal data()() As Double, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, ordering)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer) As INDArray
			Return New JCublasNDArray(data)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray
			' FIXME: int cast
			Return New JCublasNDArray(data, New Long() {rows, columns}, ArrayUtil.toLongArray(stride), Nd4j.order(), data.dataType())
		End Function

		Public Overrides Function create(ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(shape, ordering)
		End Function

		Public Overrides Function createUninitialized(ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(shape, Nd4j.getStrides(shape, ordering), 0, ordering, False)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal newShape() As Integer, ByVal newStride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, newShape, newStride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long, ByVal order As Char?) As INDArray
			Return New JCublasNDArray(data, shape, offset, order)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, New Long() {rows, columns}, ArrayUtil.toLongArray(stride), offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, shape, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, shape, ordering)
		End Function

		Public Overrides Function create(ByVal strings As ICollection(Of String), ByVal shape() As Long, ByVal order As Char) As INDArray
			Dim pairShape As val = Nd4j.ShapeInfoProvider.createShapeInformation(shape, order, DataType.UTF8)
			Dim buffer As val = New CudaUtf8Buffer(strings)
			Dim list As val = New List(Of String)(strings)
			Return Nd4j.createArrayFromShapeBuffer(buffer, pairShape)
		End Function

		Public Overrides Function create(ByVal list As IList(Of INDArray), ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(list, shape, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, shape, ChrW(offset))
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset, ordering)
		End Function

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset)
		End Function

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Integer) As INDArray
			Return New JCublasNDArray(data, shape)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, ArrayUtil.toLongArray(shape), ArrayUtil.toLongArray(stride), Nd4j.order(), data.dataType())
		End Function

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="list"> </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overrides Function create(ByVal list As IList(Of INDArray), ByVal shape() As Integer) As INDArray
			If order_Conflict = FORTRAN Then
				Return New JCublasNDArray(list, shape, ArrayUtil.calcStridesFortran(shape))
			Else
				Return New JCublasNDArray(list, shape)
			End If
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, shape, offset)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType, workspace), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Integer, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Short, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As SByte, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Boolean, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal floats()() As Single) As INDArray
			Return New JCublasNDArray(floats)
		End Function

		Public Overrides Function create(ByVal data()() As Single, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(buffer, shape, offset)
		End Function


		Public Overrides Function toFlattened(ByVal matrices As ICollection(Of INDArray)) As INDArray
			Return Me.toFlattened(AscW(order()), matrices)
		End Function

		Public Overrides Function toFlattened(ByVal order As Char, ByVal matrices As ICollection(Of INDArray)) As INDArray
			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			Return Nd4j.exec(New Flatten(order, matrices.ToArray()))(0)
		End Function

		Public Overrides Function concat(ByVal dimension As Integer, ParamArray ByVal toConcat() As INDArray) As INDArray
			Nd4j.Executioner.push()

			Return Nd4j.exec(New Concat(dimension, toConcat))(0)
		End Function


		Public Overrides Function specialConcat(ByVal dimension As Integer, ParamArray ByVal toConcat() As INDArray) As INDArray
			If toConcat.Length = 1 Then
				Return toConcat(0)
			End If

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			Dim shapeInfoPointers As New PointerPointer(toConcat.Length)
			Dim dataPointers As New PointerPointer(toConcat.Length)

			Dim allocator As AtomicAllocator = AtomicAllocator.Instance
			Dim context As val = allocator.DeviceContext


			Dim sumAlongDim As Integer = 0

			Dim outputShape As val = ArrayUtil.copy(toConcat(0).shape())


			For i As Integer = 0 To toConcat.Length - 1
				DirectCast(toConcat(i).data(), BaseCudaDataBuffer).lazyAllocateHostPointer()

				If toConcat(i).Compressed Then
					Nd4j.Compressor.decompressi(toConcat(i))
				End If

				allocator.synchronizeHostData(toConcat(i))
				shapeInfoPointers.put(i, allocator.getHostPointer(toConcat(i).shapeInfoDataBuffer()))
				dataPointers.put(i, allocator.getHostPointer(toConcat(i).data()))
				sumAlongDim += toConcat(i).size(dimension)

				Dim j As Integer = 0
				Do While j < toConcat(i).rank()
					If j <> dimension AndAlso toConcat(i).size(j) <> outputShape(j) Then
						Throw New System.ArgumentException("Illegal concatenation at array " & i & " and shape element " & j)
					End If
					j += 1
				Loop
			Next i

			outputShape(dimension) = sumAlongDim


			Dim ret As val = Nd4j.createUninitialized(toConcat(0).dataType(), outputShape, Nd4j.order())

			CType(ret.data(), BaseCudaDataBuffer).lazyAllocateHostPointer()

			nativeOps.specialConcat(Nothing, dimension, toConcat.Length, dataPointers, shapeInfoPointers, ret.data().addressPointer(), CType(ret.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			Dim point As AllocationPoint = allocator.getAllocationPoint(ret)

			Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

			nativeOps.memcpyAsync(point.DevicePointer, point.HostPointer, ret.length() * Nd4j.sizeOfDataType(ret.data().dataType()), CudaConstants.cudaMemcpyHostToDevice, context.getSpecialStream())
			context.getSpecialStream().synchronize()

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			PerformanceTracker.Instance.helperRegisterTransaction(point.DeviceId, perfD, point.NumberOfBytes, MemcpyDirection.HOST_TO_DEVICE)

			point.tickHostRead()
			point.tickDeviceWrite()

			Return ret
		End Function



		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source">          source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes">         indexes from source array
		''' @return </param>
		Public Overrides Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer) As INDArray
			Return pullRows(source, sourceDimension, indexes, Nd4j.order())
		End Function

		Public Overrides Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Long) As INDArray
			Return pullRows(source, sourceDimension, ArrayUtil.toInts(indexes))
		End Function

		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source">          source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes">         indexes from source array
		''' @return </param>
		Public Overrides Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer, ByVal order As Char) As INDArray
			If indexes Is Nothing OrElse indexes.Length < 1 Then
				Throw New System.InvalidOperationException("Indexes can't be null or zero-length")
			End If


			Dim shape() As Long
			If source.rank() = 1 Then
				shape = New Long(){indexes.Length}
			ElseIf sourceDimension = 1 Then
				shape = New Long() {indexes.Length, source.shape()(sourceDimension)}
			ElseIf sourceDimension = 0 Then
				shape = New Long() {source.shape()(sourceDimension), indexes.Length}
			Else
				Throw New System.NotSupportedException("2D input is expected")
			End If

			Return pullRows(source, Nd4j.createUninitialized(source.dataType(), shape, order), sourceDimension, indexes)
		End Function

		Public Overrides Function pullRows(ByVal source As INDArray, ByVal destination As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer) As INDArray
			Nd4j.Executioner.push()

			If indexes Is Nothing OrElse indexes.Length < 1 Then
				Throw New System.InvalidOperationException("Indexes can't be null or zero-length")
			End If

			Preconditions.checkArgument(source.dataType() = destination.dataType(), "Source and Destination data types must be the same")

			Dim shape() As Long = Nothing
			If source.rank() = 1 Then
				shape = New Long(){indexes.Length}
			ElseIf sourceDimension = 1 Then
				shape = New Long() {indexes.Length, source.shape()(sourceDimension)}
			ElseIf sourceDimension = 0 Then
				shape = New Long() {source.shape()(sourceDimension), indexes.Length}
			Else
				Throw New System.NotSupportedException("2D input is expected")
			End If

			Dim ret As INDArray = destination
			If ret Is Nothing Then
				ret = Nd4j.createUninitialized(source.dataType(), shape, order_Conflict)
			Else
				If Not shape.SequenceEqual(destination.shape()) Then
					Throw New System.InvalidOperationException("Cannot pull rows into destination array: expected destination array of" & " shape " & java.util.Arrays.toString(shape) & " but got destination array of shape " & java.util.Arrays.toString(destination.shape()))
				End If
			End If

			Dim allocator As AtomicAllocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareAction(ret, source)

			Dim x As val = DirectCast(source.data(), BaseCudaDataBuffer).OpaqueDataBuffer
			Dim z As val = DirectCast(ret.data(), BaseCudaDataBuffer).OpaqueDataBuffer
			Dim xShape As Pointer = AtomicAllocator.Instance.getPointer(source.shapeInfoDataBuffer(), context)
			Dim zShape As Pointer = AtomicAllocator.Instance.getPointer(ret.shapeInfoDataBuffer(), context)

			Dim extras As New PointerPointer(AddressRetriever.retrieveHostPointer(ret.shapeInfoDataBuffer()), context.getOldStream(), allocator.DeviceIdPointer)

			Dim tempIndexes As val = New CudaLongDataBuffer(indexes.Length)
			AtomicAllocator.Instance.memcpyBlocking(tempIndexes, New LongPointer(ArrayUtil.toLongArray(indexes)), indexes.Length * 8, 0)

			Dim pIndex As Pointer = AtomicAllocator.Instance.getPointer(tempIndexes, context)

			Dim tadManager As TADManager = Nd4j.Executioner.TADManager

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager.getTADOnlyShapeInfo(source, New Integer() {sourceDimension})
			Dim zTadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager.getTADOnlyShapeInfo(ret, New Integer() {sourceDimension})

			Dim tadShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(tadBuffers.First, context)
			Dim zTadShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(zTadBuffers.First, context)

			Dim offsets As DataBuffer = tadBuffers.Second
			Dim tadOffsets As Pointer = AtomicAllocator.Instance.getPointer(offsets, context)

			Dim zTadOffsets As Pointer = AtomicAllocator.Instance.getPointer(zTadBuffers.Second, context)


			nativeOps.pullRows(extras, x, CType(source.shapeInfoDataBuffer().addressPointer(), LongPointer), CType(xShape, LongPointer), z, CType(ret.shapeInfoDataBuffer().addressPointer(), LongPointer), CType(zShape, LongPointer), indexes.Length, CType(pIndex, LongPointer), CType(tadShapeInfo, LongPointer), New LongPointerWrapper(tadOffsets), CType(zTadShapeInfo, LongPointer), New LongPointerWrapper(zTadOffsets))

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			allocator.registerAction(context, ret, source)

			Return ret
		End Function

		Public Overrides Function accumulate(ByVal target As INDArray, ParamArray ByVal arrays() As INDArray) As INDArray
			If arrays Is Nothing OrElse arrays.Length = 0 Then
				Throw New Exception("Input arrays are missing")
			End If

			If arrays.Length = 1 Then
				Return target.assign(arrays(0))
			End If

			' we do averaging on GPU only if ALL devices have p2p links
			If True Then
				Nd4j.Executioner.push()

				Dim len As Long = target.length()

				Dim allocator As AtomicAllocator = AtomicAllocator.Instance

				Dim context As CudaContext = allocator.FlowController.prepareAction(target, arrays)

				Dim extras As New PointerPointer(Nothing, context.getOldStream(), allocator.DeviceIdPointer, New CudaPointer(0))


				Dim z As Pointer = AtomicAllocator.Instance.getPointer(target, context)

				Dim xPointers(arrays.Length - 1) As Long

				For i As Integer = 0 To arrays.Length - 1
					If arrays(i).elementWiseStride() <> 1 Then
						Throw New ND4JIllegalStateException("Native averaging is applicable only to continuous INDArrays")
					End If

					If arrays(i).length() <> len Then
						Throw New ND4JIllegalStateException("All arrays should have equal length for averaging")
					End If

					Dim point As AllocationPoint = allocator.getAllocationPoint(arrays(i))
					xPointers(i) = point.DevicePointer.address()
					point.tickDeviceWrite()
				Next i

				Dim tempX As New CudaDoubleDataBuffer(arrays.Length)

				allocator.memcpyBlocking(tempX, New LongPointer(xPointers), xPointers.Length * 8, 0)

				Dim x As New PointerPointer(AtomicAllocator.Instance.getPointer(tempX, context))

				nativeOps.accumulate(extras, Nothing, CType(arrays(0).shapeInfoDataBuffer().addressPointer(), LongPointer), x, Nothing, Nothing, CType(allocator.getHostPointer(target.shapeInfoDataBuffer()), LongPointer), z, CType(allocator.getPointer(target.shapeInfoDataBuffer()), LongPointer), arrays.Length, len)

				If nativeOps.lastErrorCode() <> 0 Then
					Throw New Exception(nativeOps.lastErrorMessage())
				End If

				allocator.FlowController.registerAction(context, target, arrays)

				Return target
			Else
				Dim len As Long = target.length()

				Nd4j.Executioner.commit()

				Dim context As val = CType(AtomicAllocator.Instance.DeviceContext, CudaContext)

				Dim dataPointers As val = New PointerPointer(arrays.Length)
				Dim extras As val = New PointerPointer(Nothing, context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, New CudaPointer(1))

				For i As Integer = 0 To arrays.Length - 1
					Nd4j.Compressor.autoDecompress(arrays(i))

					If arrays(i).elementWiseStride() <> 1 Then
						Throw New ND4JIllegalStateException("Native averaging is applicable only to continuous INDArrays")
					End If

					If arrays(i).length() <> len Then
						Throw New ND4JIllegalStateException("All arrays should have equal length for averaging")
					End If

					DirectCast(arrays(i).data(), BaseCudaDataBuffer).lazyAllocateHostPointer()

					dataPointers.put(i, AtomicAllocator.Instance.getHostPointer(arrays(i)))
				Next i

				If target IsNot Nothing Then
					DirectCast(target.data(), BaseCudaDataBuffer).lazyAllocateHostPointer()
				End If

				nativeOps.accumulate(extras, dataPointers, CType(arrays(0).shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing,If(target Is Nothing, Nothing, AtomicAllocator.Instance.getHostPointer(target)),If(target Is Nothing, Nothing, CType(AtomicAllocator.Instance.getHostPointer(target.shapeInfoDataBuffer()), LongPointer)), Nothing, Nothing, arrays.Length, len)

				If nativeOps.lastErrorCode() <> 0 Then
					Throw New Exception(nativeOps.lastErrorMessage())
				End If

				AtomicAllocator.Instance.getAllocationPoint(target).tickHostWrite()


				Return target
			End If

		End Function

		Public Overrides Function average(ByVal target As INDArray, ByVal arrays() As INDArray) As INDArray
			If arrays Is Nothing OrElse arrays.Length = 0 Then
				Throw New Exception("Input arrays are missing")
			End If

			If arrays.Length = 1 Then
				'Edge case - average 1 array - no op
				If target Is Nothing Then
					Return Nothing
				End If
				Return target.assign(arrays(0))
			End If

			' we do averaging on GPU only if ALL devices have p2p links
			If nativeOps.P2PAvailable AndAlso CudaEnvironment.Instance.Configuration.isCrossDeviceAccessAllowed() Then

				Nd4j.Executioner.push()

				Dim len As Long = If(target IsNot Nothing, target.length(), arrays(0).length())

				Dim allocator As AtomicAllocator = AtomicAllocator.Instance

				Dim context As CudaContext = allocator.FlowController.prepareAction(target, arrays)

				Dim extras As New PointerPointer(Nothing, context.getOldStream(), allocator.DeviceIdPointer, New CudaPointer(0))


				Dim z As Pointer = If(target Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(target, context))

				Dim xPointers(arrays.Length - 1) As Long

				For i As Integer = 0 To arrays.Length - 1
					If arrays(i).elementWiseStride() <> 1 Then
						Throw New ND4JIllegalStateException("Native averaging is applicable only to continuous INDArrays")
					End If

					If arrays(i).length() <> len Then
						Throw New ND4JIllegalStateException("All arrays should have equal length for averaging")
					End If

					Dim point As AllocationPoint = allocator.getAllocationPoint(arrays(i))
					xPointers(i) = point.DevicePointer.address()
					point.tickDeviceWrite()
				Next i

				Dim tempX As New CudaDoubleDataBuffer(arrays.Length)

				allocator.memcpyBlocking(tempX, New LongPointer(xPointers), xPointers.Length * 8, 0)

				Dim x As New PointerPointer(AtomicAllocator.Instance.getPointer(tempX, context))

				nativeOps.average(extras, Nothing, CType(arrays(0).shapeInfoDataBuffer().addressPointer(), LongPointer), x, Nothing, Nothing, CType(If(target Is Nothing, Nothing, target.shapeInfoDataBuffer().addressPointer()), LongPointer),If(target Is Nothing, Nothing, z), Nothing, arrays.Length, len, True)

				If nativeOps.lastErrorCode() <> 0 Then
					Throw New Exception(nativeOps.lastErrorMessage())
				End If

				allocator.FlowController.registerAction(context, target, arrays)

				Return target
			Else
				' otherwise we do averging on CPU side
				''' <summary>
				''' We expect all operations are complete at this point
				''' </summary>
				Dim len As Long = If(target Is Nothing, arrays(0).length(), target.length())

				Dim context As val = CType(AtomicAllocator.Instance.DeviceContext, CudaContext)

				Dim dataPointers As val = New PointerPointer(arrays.Length)
				Dim extras As val = New PointerPointer(Nothing, context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, New CudaPointer(1))

				For i As Integer = 0 To arrays.Length - 1
					Nd4j.Compressor.autoDecompress(arrays(i))

					If arrays(i).elementWiseStride() <> 1 Then
						Throw New ND4JIllegalStateException("Native averaging is applicable only to continuous INDArrays")
					End If

					If arrays(i).length() <> len Then
						Throw New ND4JIllegalStateException("All arrays should have equal length for averaging")
					End If

					DirectCast(arrays(i).data(), BaseCudaDataBuffer).lazyAllocateHostPointer()

					dataPointers.put(i, AtomicAllocator.Instance.getHostPointer(arrays(i)))
				Next i

				If target IsNot Nothing Then
					DirectCast(target.data(), BaseCudaDataBuffer).lazyAllocateHostPointer()
				End If

				nativeOps.average(extras, dataPointers, CType(arrays(0).shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing,If(target Is Nothing, Nothing, target.data().addressPointer()), CType(If(target Is Nothing, Nothing, target.shapeInfoDataBuffer().addressPointer()), LongPointer), Nothing, Nothing, arrays.Length, len, True)

				If nativeOps.lastErrorCode() <> 0 Then
					Throw New Exception(nativeOps.lastErrorMessage())
				End If

				If target IsNot Nothing Then
					AtomicAllocator.Instance.getAllocationPoint(target).tickHostWrite()
				End If

				' TODO: make propagation optional maybe?
				If True Then
					For i As Integer = 0 To arrays.Length - 1
						AtomicAllocator.Instance.getAllocationPoint(arrays(i)).tickHostWrite()
					Next i
				End If

				Return target
			End If
		End Function

		Public Overrides Function average(ByVal arrays As ICollection(Of INDArray)) As INDArray
			Return average(arrays.ToArray())
		End Function


		''' <summary>
		''' This method averages input arrays, and returns averaged array
		''' </summary>
		''' <param name="arrays">
		''' @return </param>
		Public Overrides Function average(ByVal arrays() As INDArray) As INDArray
			If arrays Is Nothing OrElse arrays.Length = 0 Then
				Throw New Exception("Input arrays are missing")
			End If

			' we assume all arrays have equal length,
			Dim ret As INDArray = Nd4j.createUninitialized(arrays(0).dataType(), arrays(0).shape(), arrays(0).ordering())

			Return average(ret, arrays)
		End Function

		''' <summary>
		''' This method averages input arrays, and returns averaged array
		''' </summary>
		''' <param name="target"> </param>
		''' <param name="arrays">
		''' @return </param>
		Public Overrides Function average(ByVal target As INDArray, ByVal arrays As ICollection(Of INDArray)) As INDArray
			Return average(target, arrays.ToArray())
		End Function

		''' <summary>
		''' In place shuffle of an ndarray
		''' along a specified set of dimensions
		''' </summary>
		''' <param name="array">     the ndarray to shuffle </param>
		''' <param name="dimension"> the dimension to do the shuffle
		''' @return </param>
		Public Overrides Sub shuffle(ByVal array As INDArray, ByVal rnd As Random, ParamArray ByVal dimension() As Integer)
			shuffle(Collections.singletonList(array), rnd, dimension)
		End Sub

		''' <summary>
		''' Symmetric in place shuffle of an ndarray
		''' along a specified set of dimensions. Each array in list should have it's own dimension at the same index of dimensions array
		''' </summary>
		''' <param name="arrays">      the ndarrays to shuffle </param>
		''' <param name="dimensions"> the dimensions to do the shuffle
		''' @return </param>
		Public Overrides Sub shuffle(ByVal arrays As IList(Of INDArray), ByVal rnd As Random, ByVal dimensions As IList(Of Integer()))
			' no dimension - no shuffle
			If dimensions Is Nothing OrElse dimensions.Count = 0 Then
				Throw New Exception("Dimension can't be null or 0-length")
			End If

			If arrays Is Nothing OrElse arrays.Count = 0 Then
				Throw New Exception("No input arrays provided")
			End If

			If dimensions.Count > 1 AndAlso arrays.Count <> dimensions.Count Then
				Throw New System.InvalidOperationException("Number of dimensions do not match number of arrays to shuffle")
			End If

			Nd4j.Executioner.push()

			' first we build TAD for input array and dimensions

			Dim allocator As AtomicAllocator = AtomicAllocator.Instance

			Dim context As CudaContext = Nothing

			For x As Integer = 0 To arrays.Count - 1
				context = allocator.FlowController.prepareAction(arrays(x))
			Next x

			Dim zero As val = arrays(0)
			Dim tadLength As Integer = 1
			If zero.rank() > 1 Then
				Dim i As Integer = 0
				Do While i < dimensions(0).Length
					tadLength *= zero.shape()(dimensions(0)(i))
					i += 1
				Loop
			End If

			Dim numTads As val = zero.length() \ tadLength

			Dim map As val = ArrayUtil.buildInterleavedVector(rnd, CInt(numTads))

			Dim shuffle As val = New CudaIntDataBuffer(map)

			Dim shuffleMap As val = allocator.getPointer(shuffle, context)

			Dim extras As val = New PointerPointer(Nothing, context.getOldStream(), allocator.DeviceIdPointer)


			Dim hPointers(arrays.Count - 1) As Long
			Dim xPointers(arrays.Count - 1) As Long
			Dim xShapes(arrays.Count - 1) As Long
			Dim tadShapes(arrays.Count - 1) As Long
			Dim tadOffsets(arrays.Count - 1) As Long

			For i As Integer = 0 To arrays.Count - 1
				Dim array As val = arrays(i)

				Dim x As val = AtomicAllocator.Instance.getPointer(array, context)
				Dim xShapeInfo As val = AtomicAllocator.Instance.getPointer(array.shapeInfoDataBuffer(), context)


				Dim tadManager As val = Nd4j.Executioner.TADManager

				Dim dimension() As Integer = If(dimensions.Count > 1, dimensions(i), dimensions(0))

				Dim tadBuffers As val = tadManager.getTADOnlyShapeInfo(array, dimension)

	'            log.info("Original shape: {}; dimension: {}; TAD shape: {}", array.shapeInfoDataBuffer().asInt(), dimension, tadBuffers.getFirst().asInt());

				Dim tadShapeInfo As val = AtomicAllocator.Instance.getPointer(tadBuffers.getFirst(), context)

				Dim offsets As val = tadBuffers.getSecond()

				If zero.rank() <> 1 AndAlso offsets.length() <> numTads Then
					Throw New ND4JIllegalStateException("Can't symmetrically shuffle arrays with non-equal number of TADs")
				End If

				Dim tadOffset As val = AtomicAllocator.Instance.getPointer(offsets, context)

				hPointers(i) = AtomicAllocator.Instance.getHostPointer(array.shapeInfoDataBuffer()).address()
				xPointers(i) = x.address()
				xShapes(i) = xShapeInfo.address()
				tadShapes(i) = tadShapeInfo.address()
				tadOffsets(i) = tadOffset.address()
			Next i


			Dim hostPointers As val = New LongPointer(hPointers)
			Dim hosthost As val = New PointerPointerWrapper(hostPointers)
			Dim tempX As val = New CudaDoubleDataBuffer(arrays.Count)
			Dim tempShapes As val = New CudaDoubleDataBuffer(arrays.Count)
			Dim tempTAD As val = New CudaDoubleDataBuffer(arrays.Count)
			Dim tempOffsets As val = New CudaDoubleDataBuffer(arrays.Count)

			AtomicAllocator.Instance.memcpyBlocking(tempX, New LongPointer(xPointers), xPointers.Length * 8, 0)
			AtomicAllocator.Instance.memcpyBlocking(tempShapes, New LongPointer(xShapes), xPointers.Length * 8, 0)
			AtomicAllocator.Instance.memcpyBlocking(tempTAD, New LongPointer(tadShapes), xPointers.Length * 8, 0)
			AtomicAllocator.Instance.memcpyBlocking(tempOffsets, New LongPointer(tadOffsets), xPointers.Length * 8, 0)

			nativeOps.shuffle(extras, Nothing, hosthost, New PointerPointer(allocator.getPointer(tempX, context)), New PointerPointer(allocator.getPointer(tempShapes, context)), Nothing, Nothing, New PointerPointer(allocator.getPointer(tempX, context)), New PointerPointer(allocator.getPointer(tempShapes, context)), arrays.Count, CType(shuffleMap, IntPointer), New PointerPointer(allocator.getPointer(tempTAD, context)), New PointerPointer(allocator.getPointer(tempOffsets, context)))

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			For f As Integer = 0 To arrays.Count - 1
				allocator.FlowController.registerAction(context, arrays(f))
			Next f


			' just to keep reference
			'shuffle.address();
			'hostPointers.address();

			tempX.dataType()
			tempShapes.dataType()
			tempOffsets.dataType()
			tempTAD.dataType()
		End Sub

		''' <summary>
		''' Symmetric in place shuffle of an ndarray
		''' along a specified set of dimensions. All arrays
		''' </summary>
		''' <param name="sourceArrays">     the ndarray to shuffle </param>
		''' <param name="dimension"> the dimension to do the shuffle
		''' @return </param>
		Public Overrides Sub shuffle(ByVal sourceArrays As ICollection(Of INDArray), ByVal rnd As Random, ParamArray ByVal dimension() As Integer)
			shuffle(New List(Of INDArray)(sourceArrays), rnd, Collections.singletonList(dimension))
		End Sub

	'    
	'    public DataBuffer convertToHalfs(DataBuffer buffer) {
	'        DataBuffer halfsBuffer = new CudaHalfDataBuffer(buffer.length());
	'    
	'        AtomicAllocator allocator = AtomicAllocator.getInstance();
	'    
	'        AllocationPoint pointSrc = allocator.getAllocationPoint(buffer);
	'        AllocationPoint pointDst = allocator.getAllocationPoint(halfsBuffer);
	'    
	'        CudaContext context =  allocator.getFlowController().prepareAction(pointDst, pointSrc);
	'    
	'        PointerPointer extras = new PointerPointer(
	'                null, // not used for conversion
	'                context.getOldStream(),
	'                AtomicAllocator.getInstance().getDeviceIdPointer());
	'    
	'        Pointer x = AtomicAllocator.getInstance().getPointer(buffer, context);
	'        Pointer z = AtomicAllocator.getInstance().getPointer(halfsBuffer, context);
	'    
	'        if (buffer.dataType() == DataType.FLOAT) {
	'            NativeOpsHolder.getInstance().getDeviceNativeOps().convertFloatsToHalfs(extras, x, (int) buffer.length(), z);
	'            pointDst.tickDeviceWrite();
	'        } else if (buffer.dataType() == DataType.DOUBLE) {
	'            NativeOpsHolder.getInstance().getDeviceNativeOps().convertDoublesToHalfs(extras, x, (int) buffer.length(), z);
	'            pointDst.tickDeviceWrite();
	'        } else if (buffer.dataType() == DataType.HALF) {
	'            log.info("Buffer is already HALF-precision");
	'            return buffer;
	'        } else {
	'            throw new UnsupportedOperationException("Conversion INT->HALF isn't supported yet.");
	'        }
	'    
	'        allocator.getFlowController().registerAction(context, pointDst, pointSrc);
	'    
	'        return halfsBuffer;
	'    }
	'    
	'    public DataBuffer restoreFromHalfs(DataBuffer buffer) {
	'        if (buffer.dataType() != DataType.HALF)
	'            throw new IllegalStateException("Input DataBuffer should contain Halfs");
	'    
	'        DataBuffer outputBuffer = null;
	'    
	'    
	'    
	'        if (Nd4j.dataType() == DataType.FLOAT) {
	'            outputBuffer = new CudaFloatDataBuffer(buffer.length());
	'    
	'        } else if (Nd4j.dataType() == DataType.DOUBLE) {
	'            outputBuffer = new CudaDoubleDataBuffer(buffer.length());
	'    
	'        } else throw new UnsupportedOperationException("DataType ["+Nd4j.dataType()+"] isn't supported yet");
	'    
	'        AtomicAllocator allocator = AtomicAllocator.getInstance();
	'    
	'        AllocationPoint pointSrc = allocator.getAllocationPoint(buffer);
	'        AllocationPoint pointDst = allocator.getAllocationPoint(outputBuffer);
	'    
	'        CudaContext context =  allocator.getFlowController().prepareAction(pointDst, pointSrc);
	'    
	'        PointerPointer extras = new PointerPointer(
	'                null, // not used for conversion
	'                context.getOldStream(),
	'                AtomicAllocator.getInstance().getDeviceIdPointer());
	'    
	'        Pointer x = AtomicAllocator.getInstance().getPointer(buffer, context);
	'        Pointer z = AtomicAllocator.getInstance().getPointer(outputBuffer, context);
	'    
	'        if (Nd4j.dataType() == DataType.FLOAT) {
	'            NativeOpsHolder.getInstance().getDeviceNativeOps().convertHalfsToFloats(extras, x, (int) buffer.length(), z);
	'            pointDst.tickDeviceWrite();
	'        } else if (Nd4j.dataType() == DataType.DOUBLE) {
	'            NativeOpsHolder.getInstance().getDeviceNativeOps().convertHalfsToDoubles(extras, x, (int) buffer.length(), z);
	'            pointDst.tickDeviceWrite();
	'        } else if (Nd4j.dataType() == DataType.HALF) {
	'            log.info("Buffer is already HALF-precision");
	'            return buffer;
	'        }
	'    
	'        allocator.getFlowController().registerAction(context, pointDst, pointSrc);
	'    
	'        return outputBuffer;
	'    }
	'    

		''' <summary>
		''' This method converts Single/Double precision databuffer to Half-precision databuffer
		''' </summary>
		''' <param name="typeSrc"> </param>
		''' <param name="source"> </param>
		''' <param name="typeDst"> @return </param>
		Public Overrides Function convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As INDArray, ByVal typeDst As DataTypeEx) As INDArray
			If source.View Then
				Throw New System.NotSupportedException("Impossible to compress View. Consider using dup() before. ")
			End If

			Dim buffer As DataBuffer = convertDataEx(typeSrc, source.data(), typeDst)
			source.Data = buffer

			If TypeOf buffer Is CompressedDataBuffer Then
				source.markAsCompressed(True)
			Else
				source.markAsCompressed(False)
			End If

			Return source
		End Function



		Public Overrides Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As Pointer, ByVal typeDst As DataTypeEx, ByVal target As Pointer, ByVal length As Long)
			Dim stream As val = AtomicAllocator.Instance.DeviceContext.getOldStream()

			Dim p As val = New PointerPointer(Of )(New Pointer(){Nothing, stream})

			nativeOps.convertTypes(p, typeSrc.ordinal(), source, length, typeDst.ordinal(), target)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If
		End Sub

		Public Overrides Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As Pointer, ByVal typeDst As DataTypeEx, ByVal buffer As DataBuffer)
			Dim srcPtr As Pointer = Nothing
			Dim dstPtr As Pointer = Nothing
			Dim size As Long = 0
			Dim ssize As Long = 0
			Dim stream As val = AtomicAllocator.Instance.DeviceContext.getOldStream()
			If TypeOf buffer Is CompressedDataBuffer Then
				' compressing
				size = DirectCast(buffer, CompressedDataBuffer).getCompressionDescriptor().getCompressedLength()
				ssize = DirectCast(buffer, CompressedDataBuffer).getCompressionDescriptor().getOriginalLength()

				srcPtr = nativeOps.mallocDevice(ssize, 0, 0)
				dstPtr = nativeOps.mallocDevice(size, 0, 0)

				If nativeOps.lastErrorCode() <> 0 Then
					Throw New Exception(nativeOps.lastErrorMessage())
				End If

				nativeOps.memcpyAsync(srcPtr, source, ssize, CudaConstants.cudaMemcpyHostToDevice, stream)

				If nativeOps.lastErrorCode() <> 0 Then
					Throw New Exception(nativeOps.lastErrorMessage())
				End If
			Else
				' decompressing
				Throw New System.NotSupportedException()
			End If

			convertDataEx(typeSrc, srcPtr, typeDst, dstPtr, buffer.length())
			nativeOps.memcpyAsync(buffer.addressPointer(), dstPtr, size, CudaConstants.cudaMemcpyHostToHost, stream)

			stream.synchronize()

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			If TypeOf buffer Is CompressedDataBuffer Then
				nativeOps.freeDevice(srcPtr, 0)
				nativeOps.freeDevice(dstPtr, 0)

				If nativeOps.lastErrorCode() <> 0 Then
					Throw New Exception(nativeOps.lastErrorMessage())
				End If
			End If
		End Sub

		Public Overrides Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As DataBuffer, ByVal typeDst As DataTypeEx, ByVal target As DataBuffer)

			Dim stream As val = AtomicAllocator.Instance.DeviceContext.getOldStream()
			Dim srcPtr As Pointer = Nothing
			Dim dstPtr As Pointer = Nothing

			' we have to replace pointer here, temporary
			If Nd4j.WorkspaceManager.anyWorkspaceActiveForCurrentThread() Then
				Dim ws As val = Nd4j.MemoryManager.CurrentWorkspace
				' if true - we're decompressing from host memory
				If TypeOf source Is CompressedDataBuffer Then
					Dim size As val = DirectCast(source, CompressedDataBuffer).getCompressionDescriptor().getCompressedLength()
					srcPtr = ws.alloc(size, MemoryKind.DEVICE, DataType.HALF, False)
					nativeOps.memcpyAsync(srcPtr, source.addressPointer(), size, CudaConstants.cudaMemcpyHostToHost, stream)

					If nativeOps.lastErrorCode() <> 0 Then
						Throw New Exception(nativeOps.lastErrorMessage())
					End If
				End If

				' if true - we're compressing into host memory
				If TypeOf target Is CompressedDataBuffer Then
					Dim size As val = DirectCast(target, CompressedDataBuffer).getCompressionDescriptor().getCompressedLength()
					dstPtr = ws.alloc(size, MemoryKind.DEVICE, DataType.HALF, False)
				End If
			Else
				' if true - we're decompressing from host memory
				If TypeOf source Is CompressedDataBuffer Then
					log.info("Replacing source ptr")
					Dim size As val = DirectCast(source, CompressedDataBuffer).getCompressionDescriptor().getCompressedLength()
					srcPtr = nativeOps.mallocDevice(size, 0, 0)
					nativeOps.memcpyAsync(srcPtr, source.addressPointer(), size, CudaConstants.cudaMemcpyHostToHost, stream)
					stream.synchronize()

					If nativeOps.lastErrorCode() <> 0 Then
						Throw New Exception(nativeOps.lastErrorMessage())
					End If
				Else
					srcPtr = AtomicAllocator.Instance.getPointer(source)
				End If

				' if true - we're compressing into host memory
				If TypeOf target Is CompressedDataBuffer Then
					log.info("Replacing target ptr")
					Dim size As val = DirectCast(target, CompressedDataBuffer).getCompressionDescriptor().getCompressedLength()
					dstPtr = nativeOps.mallocDevice(size, 0, 0)

					If nativeOps.lastErrorCode() <> 0 Then
						Throw New Exception(nativeOps.lastErrorMessage())
					End If
				Else
					dstPtr = AtomicAllocator.Instance.getPointer(target)
				End If
			End If


			convertDataEx(typeSrc, srcPtr, typeDst, dstPtr, target.length())

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			Nd4j.Executioner.commit()


			' we were compressing something into temporary buffer
			If TypeOf target Is CompressedDataBuffer Then
				nativeOps.memcpyAsync(target.addressPointer(), dstPtr, target.capacity(), CudaConstants.cudaMemcpyHostToHost, stream)

				If Nd4j.WorkspaceManager.anyWorkspaceActiveForCurrentThread() Then
					' no-op, workspace was used
				Else
					nativeOps.freeDevice(dstPtr, 0)
				End If
			End If

			' we were decompressing something from host memory
			If TypeOf source Is CompressedDataBuffer Then
				If Nd4j.WorkspaceManager.anyWorkspaceActiveForCurrentThread() Then
					' no-op, workspace was used
				Else
					nativeOps.freeDevice(srcPtr, 0)
				End If

			End If

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			Nd4j.Executioner.commit()
		End Sub

		Public Overrides Function convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As DataBuffer, ByVal typeDst As DataTypeEx) As DataBuffer
			Dim elementSize As Integer = 0
			If typeDst.ordinal() <= 2 Then
				elementSize = 1
			ElseIf typeDst.ordinal() <= 5 Then
				elementSize = 2
			ElseIf typeDst.ordinal() = 6 Then
				elementSize = 4
			ElseIf typeDst.ordinal() = 7 Then
				elementSize = 8
			Else
				Throw New System.NotSupportedException("Unknown target TypeEx: " & typeDst.ToString())
			End If

			' flushQueue should be blocking here, because typeConversion happens on cpu side
			Nd4j.Executioner.commit()

			Dim buffer As DataBuffer = Nothing

			If Not (TypeOf source Is CompressedDataBuffer) Then
				AtomicAllocator.Instance.synchronizeHostData(source)
			End If

			If CompressionUtils.goingToCompress(typeSrc, typeDst) Then
				' all types below 8 are compression modes
				Dim pointer As Pointer = New BytePointer(source.length() * elementSize)
				Dim descriptor As New CompressionDescriptor(source, typeDst.ToString())
				descriptor.setCompressionType(CompressionType.LOSSY)
				descriptor.setCompressedLength(source.length() * elementSize)
				buffer = New CompressedDataBuffer(pointer, descriptor)
			Else
				Dim compressed As CompressedDataBuffer = DirectCast(source, CompressedDataBuffer)
				Dim descriptor As CompressionDescriptor = compressed.getCompressionDescriptor()
				' decompression mode
				buffer = Nd4j.createBuffer(descriptor.getNumberOfElements(), False)

				Dim point As AllocationPoint = AtomicAllocator.Instance.getAllocationPoint(buffer)
				point.tickDeviceWrite()
			End If

			convertDataEx(typeSrc, source, typeDst, buffer)

			Return buffer
		End Function


		Public Overrides Function tear(ByVal tensor As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray()
			If tensor.Compressed Then
				Nd4j.Compressor.decompressi(tensor)
			End If

			Array.Sort(dimensions)

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(tensor, dimensions)

			Dim tadLength As Long = 1
			Dim shape As val = New Long(dimensions.Length - 1){}
			For i As Integer = 0 To dimensions.Length - 1
				tadLength *= tensor.shape()(dimensions(i))
				shape(i) = tensor.shape()(dimensions(i))
			Next i


			Dim numTads As Integer = CInt(tensor.length() \ tadLength)
			Dim result(numTads - 1) As INDArray

			Dim xPointers(numTads - 1) As Long

			Dim context As CudaContext = AtomicAllocator.Instance.FlowController.prepareAction(Nothing, tensor)

			For x As Integer = 0 To numTads - 1
				result(x) = Nd4j.createUninitialized(shape)

				context = AtomicAllocator.Instance.FlowController.prepareAction(result(x))

				xPointers(x) = AtomicAllocator.Instance.getPointer(result(x), context).address()
			Next x

			Dim tempX As New CudaDoubleDataBuffer(numTads)

			AtomicAllocator.Instance.memcpyBlocking(tempX, New LongPointer(xPointers), xPointers.Length * 8, 0)

			Dim extraz As New PointerPointer(Nothing, context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer)

			Dim x As val = DirectCast(tensor.data(), BaseCudaDataBuffer).OpaqueDataBuffer


			nativeOps.tear(extraz, x, CType(tensor.shapeInfoDataBuffer().addressPointer(), LongPointer), CType(AtomicAllocator.Instance.getPointer(tensor.shapeInfoDataBuffer(), context), LongPointer), New PointerPointer(AtomicAllocator.Instance.getPointer(tempX, context)), CType(AtomicAllocator.Instance.getPointer(result(0).shapeInfoDataBuffer(), context), LongPointer), CType(AtomicAllocator.Instance.getPointer(tadBuffers.First, context), LongPointer), New LongPointerWrapper(AtomicAllocator.Instance.getPointer(tadBuffers.Second, context)))

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			AtomicAllocator.Instance.FlowController.registerActionAllWrite(context, result)
			AtomicAllocator.Instance.FlowController.registerAction(context,Nothing, result)

			Return result
		End Function


		Public Overrides Function sort(ByVal x As INDArray, ByVal descending As Boolean) As INDArray
			If x.Scalar Then
				Return x
			End If

			Nd4j.Executioner.push()

			Dim context As CudaContext = AtomicAllocator.Instance.FlowController.prepareAction(x)

			Dim ptr As Pointer = AtomicAllocator.Instance.getHostPointer(x.shapeInfoDataBuffer())

			Dim extraz As New PointerPointer(ptr, context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, Nothing, context.getBufferReduction(), context.getBufferScalar(), Nothing, ptr, AtomicAllocator.Instance.getHostPointer(x.shapeInfoDataBuffer()), ptr, ptr, ptr, ptr, ptr, ptr, ptr, ptr, ptr, New CudaPointer(0))

			' we're sending > 10m elements to radixSort
			Dim isRadix As Boolean = Not x.View AndAlso (x.length() > 1024 * 1024 * 10)
			Dim tmpX As INDArray = x

			' we need to guarantee all threads are finished here
			If isRadix Then
				Nd4j.Executioner.commit()
			End If


			nativeOps.sort(extraz, Nothing, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), AtomicAllocator.Instance.getPointer(tmpX, context), CType(AtomicAllocator.Instance.getPointer(tmpX.shapeInfoDataBuffer(), context), LongPointer), descending)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			AtomicAllocator.Instance.FlowController.registerAction(context, x)

			Return x
		End Function

		Public Overrides Function empty(ByVal type As DataType) As INDArray
			Dim extras As Long = ArrayOptionsHelper.setOptionBit(0L, ArrayType.EMPTY)
			extras = ArrayOptionsHelper.setOptionBit(extras, type)
			Dim shape As val = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){}, New Long(){}, 1, "c"c, extras)
			Return New JCublasNDArray(Nothing, CType(shape.getFirst(), CudaLongDataBuffer), shape.getSecond())
		End Function


		Public Overrides Function sort(ByVal x As INDArray, ByVal descending As Boolean, ParamArray ByVal dimension() As Integer) As INDArray
			If x.Scalar Then
				Return x
			End If

			Array.Sort(dimension)

			Nd4j.Executioner.push()

			Dim tadBuffers As val = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(x, dimension)

			Dim context As val = AtomicAllocator.Instance.FlowController.prepareAction(x)

			Dim extraz As val = New PointerPointer(AtomicAllocator.Instance.getHostPointer(x.shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer)


			Dim dimensionPointer As val = AtomicAllocator.Instance.getHostPointer(AtomicAllocator.Instance.getConstantBuffer(dimension))


			nativeOps.sortTad(extraz, Nothing, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), AtomicAllocator.Instance.getPointer(x, context), CType(AtomicAllocator.Instance.getPointer(x.shapeInfoDataBuffer(), context), LongPointer), CType(dimensionPointer, IntPointer), dimension.Length, CType(AtomicAllocator.Instance.getPointer(tadBuffers.getFirst(), context), LongPointer), New LongPointerWrapper(AtomicAllocator.Instance.getPointer(tadBuffers.getSecond(), context)), descending)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			AtomicAllocator.Instance.FlowController.registerAction(context, x)

			Return x
		End Function


		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset, Nd4j.order())
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset, Nd4j.order())
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType, workspace), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType, workspace), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Integer, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Short, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As SByte, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Boolean, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createTypedBuffer(data, dataType, workspace), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Long) As INDArray
			Return New JCublasNDArray(data, shape)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset, Nd4j.order(), data.dataType())
		End Function

		Public Overrides Function create(ByVal list As IList(Of INDArray), ByVal shape() As Long) As INDArray
			Return New JCublasNDArray(list, shape)
		End Function

		Public Overrides Function create(ByVal rows As Long, ByVal columns As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
			Return create(New Long() {rows, columns}, stride, offset, Nd4j.order())
		End Function

		Public Overrides Function create(ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(shape, 0, ordering)
		End Function

		Public Overrides Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
			Return create(dataType, shape, Nd4j.getStrides(shape, ordering), ordering, workspace)
		End Function

		Public Overrides Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal strides() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createBuffer(dataType, Shape.lengthOf(shape), True, workspace), shape, strides, ordering, dataType)
		End Function

		Public Overrides Function createUninitialized(ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(shape, Nd4j.getStrides(shape, ordering), 0, ordering, False)
		End Function

		Public Overrides Function createUninitialized(ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(Nd4j.createBuffer(dataType, Shape.lengthOf(shape), False), shape, Nd4j.getStrides(shape, ordering), ordering, dataType)
		End Function

		Public Overrides Function createUninitializedDetached(ByVal dataType As DataType, ByVal ordering As Char, ParamArray ByVal shape() As Long) As INDArray
			Return New JCublasNDArray(Nd4j.createBufferDetached(shape, dataType), shape, Nd4j.getStrides(shape, order_Conflict), order_Conflict, dataType)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, newShape, newStride, offset, ordering, data.dataType())
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, newShape, newStride, offset, ews, ordering, data.dataType())
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal dataType As DataType) As INDArray
			'if (data.dataType() != dataType  && data.dataType() != DataType.COMPRESSED)
			'    throw new ND4JIllegalStateException("Data types mismatch: [" + data.dataType() + "] vs [" + dataType + "]");

			Return New JCublasNDArray(data, newShape, newStride, offset, ordering, dataType)
		End Function

		Public Overrides Function create(ByVal list As IList(Of INDArray), ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(list, shape, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal offset As Long) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset, order)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, shape, stride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal offset As Long, ByVal order As Char?) As INDArray
			Return New JCublasNDArray(data, shape, Nd4j.getStrides(shape, order), offset, order)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal offset As Long, ByVal order As Char?) As INDArray
			Return New JCublasNDArray(data, shape, Nd4j.getStrides(shape, order), offset, order)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return New JCublasNDArray(data, shape, Nd4j.getStrides(shape, order_Conflict), 0, ordering)
		End Function

		'//////////////////////////////////////////////////////////////////////////////////////////////////////////////

		Public Overrides Function sortCooIndices(ByVal x As INDArray) As INDArray
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal paddings() As Long, ByVal paddingOffsets() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
			Return New JCublasNDArray(dataType, shape, paddings, paddingOffsets, ordering, workspace)
		End Function
	End Class

End Namespace