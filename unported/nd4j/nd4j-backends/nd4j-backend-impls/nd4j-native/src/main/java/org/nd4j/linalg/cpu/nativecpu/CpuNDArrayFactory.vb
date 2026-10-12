Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports org.nd4j.linalg.api.buffer
Imports Flatten = org.nd4j.linalg.api.ops.custom.Flatten
Imports Concat = org.nd4j.linalg.api.ops.impl.shape.Concat
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = org.nd4j.linalg.api.shape.options.ArrayType
Imports CompressionUtils = org.nd4j.linalg.compression.CompressionUtils
Imports BaseCpuDataBuffer = org.nd4j.linalg.cpu.nativecpu.buffer.BaseCpuDataBuffer
Imports LongBuffer = org.nd4j.linalg.cpu.nativecpu.buffer.LongBuffer
Imports Utf8Buffer = org.nd4j.linalg.cpu.nativecpu.buffer.Utf8Buffer
Imports org.nd4j.common.primitives
Imports org.bytedeco.javacpp
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor
Imports CompressionType = org.nd4j.linalg.compression.CompressionType
Imports org.nd4j.linalg.cpu.nativecpu.blas
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports BaseNativeNDArrayFactory = org.nd4j.nativeblas.BaseNativeNDArrayFactory
Imports LongPointerWrapper = org.nd4j.nativeblas.LongPointerWrapper
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.nd4j.linalg.cpu.nativecpu



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CpuNDArrayFactory extends org.nd4j.nativeblas.BaseNativeNDArrayFactory
	Public Class CpuNDArrayFactory
		Inherits BaseNativeNDArrayFactory

		Protected Friend extrazA As New ThreadLocal(Of PointerPointer)()
		Protected Friend extrazB As New ThreadLocal(Of PointerPointer)()
		Protected Friend extrazSize As New ThreadLocal(Of Integer)()

		Public Sub New()
		End Sub

		Shared Sub New()
			'invoke the override
			Nd4j.BlasWrapper
		End Sub


		Public Sub New(ByVal dtype As DataType, ByVal order As Char?)
			MyBase.New(dtype, order)
		End Sub

		Public Sub New(ByVal dtype As DataType, ByVal order As Char)
			MyBase.New(dtype, order)
		End Sub

		Public Overrides Sub createBlas()
			' we'll check hardware support first
			If Not nativeOps.MinimalRequirementsMet Then
				' this means cpu binary was built for some arch support, we don't have on this box

				Dim binaryLevel As val = nativeOps.binaryLevel()
				Dim optimalLevel As val = nativeOps.optimalLevel()

				Dim binLevel As String = cpuBinaryLevelToName(binaryLevel)
				Dim optLevel As String = cpuBinaryLevelToName(optimalLevel)

				log.warn("*********************************** CPU Feature Check Failed ***********************************")
				log.error("Error initializing ND4J: Attempting to use " & binLevel & " ND4J binary on a CPU with only " & optLevel & " support")
				log.error(binLevel & " binaries cannot be run on a CPU without these instructions. See deeplearning4j.org/cpu for more details")
				log.error("ND4J will now exit.")
				log.warn("************************************************************************************************")
				Environment.Exit(1)
			End If

			Dim binaryLevel As val = nativeOps.binaryLevel()
			Dim optimalLevel As val = nativeOps.optimalLevel()

			Dim binLevel As String = cpuBinaryLevelToName(binaryLevel)
			Dim optLevel As String = cpuBinaryLevelToName(optimalLevel)
			log.info("Binary level " & binLevel & " optimization level " & optLevel)
			blas_Conflict = New CpuBlas()

			' TODO: add batched gemm here

			Dim functions As New PointerPointer(10)
			functions.put(0, Loader.addressof("cblas_sgemv"))
			functions.put(1, Loader.addressof("cblas_dgemv"))
			functions.put(2, Loader.addressof("cblas_sgemm"))
			functions.put(3, Loader.addressof("cblas_dgemm"))
			functions.put(4, Loader.addressof("cblas_sgemm_batch"))
			functions.put(5, Loader.addressof("cblas_dgemm_batch"))
			functions.put(6, Loader.addressof("LAPACKE_sgesvd"))
			functions.put(7, Loader.addressof("LAPACKE_dgesvd"))
			functions.put(8, Loader.addressof("LAPACKE_sgesdd"))
			functions.put(9, Loader.addressof("LAPACKE_dgesdd"))
			nativeOps.initializeFunctions(functions)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If
		End Sub

		Private Shared Function cpuBinaryLevelToName(ByVal level As Integer) As String
			Select Case level
				Case 3
					Return "AVX512"
				Case 2
					Return "AVX/AVX2"
				Case Else
					Return "Generic x86"
			End Select
		End Function

		Public Overrides Sub createLevel1()
			level1_Conflict = New CpuLevel1()
		End Sub

		Public Overrides Sub createLevel2()
			level2_Conflict = New CpuLevel2()
		End Sub

		Public Overrides Sub createLevel3()
			level3_Conflict = New CpuLevel3()
		End Sub

		Public Overrides Sub createLapack()
			lapack_Conflict = New CpuLapack()
		End Sub

		Public Overrides Function create(ByVal shape() As Integer, ByVal buffer As DataBuffer) As INDArray
			Return New NDArray(shape, buffer)
		End Function

		''' <summary>
		''' Create an ndarray with the given data layout
		''' </summary>
		''' <param name="data"> the data to create the ndarray with </param>
		''' <returns> the ndarray with the given data layout </returns>
		Public Overrides Function create(ByVal data()() As Double) As INDArray
			Return New NDArray(data)
		End Function

		Public Overrides Function create(ByVal data()() As Double, ByVal ordering As Char) As INDArray
			Return New NDArray(data, ordering)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer) As INDArray
			Return New NDArray(data)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long) As INDArray
			Return create(data, New Long(){rows, columns}, ArrayUtil.toLongArray(stride), offset)
		End Function

		Public Overrides Function create(ByVal rows As Long, ByVal columns As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
			Return create(New Long(){rows, columns}, stride, offset)
		End Function

		Public Overrides Function create(ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return New NDArray(shape, Nd4j.getStrides(shape, ordering), 0, ordering)
		End Function

		Public Overrides Function create(ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(shape, Nd4j.getStrides(shape, ordering), 0, ordering)
		End Function

		Public Overrides Function createUninitialized(ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return New NDArray(shape, Nd4j.getStrides(shape, ordering), 0, ordering, False)
		End Function

		Public Overrides Function createUninitialized(ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(shape, Nd4j.getStrides(shape, ordering), 0, ordering, False)
		End Function

		Public Overrides Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(dataType, shape, Nd4j.getStrides(shape, ordering), 0, ordering, workspace)
		End Function

		Public Overrides Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal strides() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(dataType, shape, strides, 0, ordering)
		End Function

		Public Overrides Function createUninitialized(ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(dataType, shape, Nd4j.getStrides(shape, ordering), 0, ordering, False, workspace)
		End Function

		Public Overrides Function createUninitializedDetached(ByVal dataType As DataType, ByVal ordering As Char, ParamArray ByVal shape() As Long) As INDArray
			Dim workspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			Nd4j.MemoryManager.CurrentWorkspace = Nothing
			Dim ret As INDArray = New NDArray(dataType, shape, Nd4j.getStrides(shape, ordering), 0, ordering, False)
			Nd4j.MemoryManager.CurrentWorkspace = workspace
			Return ret
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal newShape() As Integer, ByVal newStride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(data, newShape, newStride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long, ByVal order As Char?) As INDArray
			Return New NDArray(data, shape, offset, order)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal offset As Long, ByVal order As Char?) As INDArray
			Return New NDArray(data, shape, offset, order)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal rows As Long, ByVal columns As Long, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return create(data, New Long(){rows, columns}, ArrayUtil.toLongArray(stride), offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return New NDArray(Nd4j.createBuffer(data), shape, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return create(data, shape, CType(ordering, Char?))
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return create(data, shape, CType(ordering, Char?))
		End Function

		Public Overrides Function create(ByVal list As IList(Of INDArray), ByVal shape() As Integer, ByVal ordering As Char) As INDArray
			Return New NDArray(list, shape, ordering)
		End Function



		Public Overrides Function create(ByVal list As IList(Of INDArray), ByVal shape() As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(list, shape, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal offset As Long) As INDArray
			Return New NDArray(Nd4j.createBuffer(data), shape, offset)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal offset As Long, ByVal order As Char?) As INDArray
			Return New NDArray(data, shape, offset, order.Value)
		End Function



		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, DataType.DOUBLE), shape, stride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, DataType.DOUBLE), shape, stride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, DataType.FLOAT), shape, stride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
			Return New NDArray(data, shape, stride, offset, Nd4j.order())
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType) As INDArray
			Return New NDArray(data, shape, stride, 0, order)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
			Return New NDArray(data, shape, stride, offset, Nd4j.order())
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType, workspace)
		End Function

		Public Overrides Function create(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Integer, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Short, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As SByte, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Boolean, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, order, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Long, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Integer, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Short, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As Boolean, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data() As SByte, ByVal shape() As Long, ByVal stride() As Long, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(Nd4j.createTypedBuffer(data, dataType), shape, stride, Nd4j.order(), dataType)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Long) As INDArray
			Return New NDArray(data, shape)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long) As INDArray
			Return create(data, shape, stride, offset, Nd4j.order())
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(data, shape, stride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(data, shape, stride, offset, ews, ordering)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal dataType As DataType) As INDArray
			Return New NDArray(data, shape, stride, offset, ordering, dataType)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal order As Char, ByVal offset As Long) As INDArray
			Return New NDArray(data, shape, stride, offset, order)
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
			Return New NDArray(data, shape, stride, offset)
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
			Return New NDArray(data, shape, stride, offset)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Integer) As INDArray
			Return New NDArray(data, shape)
		End Function

		Public Overrides Function create(ByVal data As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long) As INDArray
			Return New NDArray(data, shape, stride, offset, Nd4j.order())
		End Function

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="list"> </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the instance </returns>
		Public Overrides Function create(ByVal list As IList(Of INDArray), ByVal shape() As Integer) As INDArray
			Return New NDArray(list, shape, Nd4j.getStrides(shape))
		End Function

		Public Overrides Function create(ByVal list As IList(Of INDArray), ByVal shape() As Long) As INDArray
			Return New NDArray(list, shape, Nd4j.getStrides(shape))
		End Function

		Public Overrides Function empty(ByVal type As DataType) As INDArray
			Dim extras As Long = ArrayOptionsHelper.setOptionBit(0L, ArrayType.EMPTY)
			extras = ArrayOptionsHelper.setOptionBit(extras, type)
			Dim shape As val = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){}, New Long(){},1,"c"c, extras)
			Return New NDArray(Nothing, CType(shape.getFirst(), LongBuffer), shape.getSecond())
		End Function

		Public Overrides Function create(ByVal floats()() As Single) As INDArray
			Return New NDArray(floats)
		End Function

		Public Overrides Function create(ByVal data()() As Single, ByVal ordering As Char) As INDArray
			Return New NDArray(data, ordering)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char) As INDArray
			Return New NDArray(data, shape, stride, offset, ordering)
		End Function

		Public Overrides Function create(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long) As INDArray
			Return New NDArray(buffer, shape, Nd4j.getStrides(shape), offset)
		End Function

		Public Overrides Function create(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long) As INDArray
			Return New NDArray(data, shape, offset)
		End Function

		Public Overrides Function toFlattened(ByVal order As Char, ByVal matrices As ICollection(Of INDArray)) As INDArray
			Preconditions.checkArgument(matrices.Count > 0, "toFlattened expects > 0 operands")

			Return Nd4j.exec(New Flatten(order, matrices.ToArray()))(0)
		End Function

		Public Overrides Function tear(ByVal tensor As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray()
			If tensor.Compressed Then
				Nd4j.Compressor.decompressi(tensor)
			End If

			Array.Sort(dimensions)

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(tensor, dimensions)

			Dim tadLength As Long = 1
			Dim shape(dimensions.Length - 1) As Long
			For i As Integer = 0 To dimensions.Length - 1
				tadLength *= tensor.shape()(dimensions(i))
				shape(i) = tensor.shape()(dimensions(i))
			Next i



			Dim numTads As Integer = CInt(tensor.length() \ tadLength)
			Dim result(numTads - 1) As INDArray

			Dim targets As New PointerPointer(numTads)

			For x As Integer = 0 To numTads - 1
				result(x) = Nd4j.createUninitialized(shape)

				targets.put(x, result(x).data().pointer())
			Next x

				nativeOps.tear(Nothing, DirectCast(tensor.data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(tensor.shapeInfoDataBuffer().pointer(), LongPointer), Nothing, targets, CType(result(0).shapeInfoDataBuffer().pointer(), LongPointer), CType(tadBuffers.First.pointer(), LongPointer), New LongPointerWrapper(tadBuffers.Second.pointer()))

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			Return result
		End Function

		''' <summary>
		''' concatenate ndarrays along a dimension
		''' </summary>
		''' <param name="dimension"> the dimension to concatenate along </param>
		''' <param name="toConcat">  the ndarrays to concatenate </param>
		''' <returns> the concatenate ndarrays </returns>
		Public Overrides Function concat(ByVal dimension As Integer, ParamArray ByVal toConcat() As INDArray) As INDArray
			If toConcat Is Nothing OrElse toConcat.Length = 0 Then
				Throw New ND4JIllegalStateException("Can't concatenate 0 arrays")
			End If

			If toConcat.Length = 1 Then
				Return toConcat(0)
			End If

			Return Nd4j.exec(New Concat(dimension, toConcat))(0)
		End Function


		''' <summary>
		''' For CPU backend this method is equal to concat()
		''' </summary>
		''' <param name="dimension"> the dimension to concatneate along </param>
		''' <param name="toConcat">  the ndarrays to concateneate
		''' @return </param>
		Public Overrides Function specialConcat(ByVal dimension As Integer, ParamArray ByVal toConcat() As INDArray) As INDArray
			Return concat(dimension, toConcat)
		End Function

		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source">          source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes">         indexes from source array
		''' @return </param>
		Public Overrides Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer) As INDArray
			Return pullRows(source, sourceDimension, ArrayUtil.toLongArray(indexes))
		End Function

		Public Overrides Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Long) As INDArray
			Return pullRows(source, sourceDimension, indexes, Nd4j.order())
		End Function

		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source">          source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes">         indexes from source array
		''' @return </param>

		Public Overridable Overloads Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Long, ByVal order As Char) As INDArray
			If indexes Is Nothing OrElse indexes.Length < 1 Then
				Throw New System.InvalidOperationException("Indexes can't be null or zero-length")
			End If

			Dim shape() As Long
			If sourceDimension = 1 Then
				shape = New Long() {indexes.Length, source.shape()(sourceDimension)}
			ElseIf sourceDimension = 0 Then
				shape = New Long() {source.shape()(sourceDimension), indexes.Length}
			Else
				Throw New System.NotSupportedException("2D input is expected")
			End If

			Return pullRows(source, Nd4j.createUninitialized(source.dataType(), shape, order), sourceDimension, indexes)
		End Function

		Public Overrides Function pullRows(ByVal source As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer, ByVal order As Char) As INDArray
			Return pullRows(source, sourceDimension, ArrayUtil.toLongArray(indexes), order)
		End Function

		Public Overrides Function pullRows(ByVal source As INDArray, ByVal destination As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Integer) As INDArray
			Return pullRows(source, destination, sourceDimension, ArrayUtil.toLongArray(indexes))
		End Function

		Public Overridable Overloads Function pullRows(ByVal source As INDArray, ByVal destination As INDArray, ByVal sourceDimension As Integer, ByVal indexes() As Long) As INDArray
			If indexes Is Nothing OrElse indexes.Length < 1 Then
				Throw New System.InvalidOperationException("Indexes can't be null or zero-length")
			End If

			Dim shape() As Long = Nothing
			If sourceDimension = 1 Then
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

			Nd4j.Compressor.autoDecompress(source)

			Dim dummy As val = New PointerPointer(New Pointer() {Nothing})

			Dim tadManager As val = Nd4j.Executioner.TADManager

			Dim tadBuffers As val = tadManager.getTADOnlyShapeInfo(source, New Integer() {sourceDimension})

			Dim zTadBuffers As val = tadManager.getTADOnlyShapeInfo(ret, New Integer() {sourceDimension})

			Dim hostTadShapeInfo As val = tadBuffers.getFirst().addressPointer()

			Dim zTadShapeInfo As val = zTadBuffers.getFirst().addressPointer()

			Dim pIndex As val = New LongPointer(indexes)

			Dim offsets As val = tadBuffers.getSecond()
			Dim hostTadOffsets As val = If(offsets Is Nothing, Nothing, offsets.addressPointer())

			Dim zOffsets As val = zTadBuffers.getSecond()

			Dim zTadOffsets As val = If(zOffsets Is Nothing, Nothing, zOffsets.addressPointer())


			nativeOps.pullRows(dummy, DirectCast(source.data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(source.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(ret.data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(ret.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, indexes.Length, pIndex, CType(hostTadShapeInfo, LongPointer), New LongPointerWrapper(hostTadOffsets), CType(zTadShapeInfo, LongPointer), New LongPointerWrapper(zTadOffsets))

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			Return ret
		End Function

		Public Overrides Function accumulate(ByVal target As INDArray, ParamArray ByVal arrays() As INDArray) As INDArray

			If arrays Is Nothing OrElse arrays.Length = 0 Then
				Throw New Exception("Input arrays are missing")
			End If

			If arrays.Length = 1 Then
				Return target.addi(arrays(0))
			End If

			Dim len As Long = target.length()

			Dim dataPointers As New PointerPointer(arrays.Length)

			For i As Integer = 0 To arrays.Length - 1
				Nd4j.Compressor.autoDecompress(arrays(i))

				If arrays(i).elementWiseStride() <> 1 Then
					Throw New ND4JIllegalStateException("Native accumulation is applicable only to continuous INDArrays")
				End If

				If arrays(i).length() <> len Then
					Throw New ND4JIllegalStateException("All arrays should have equal length for accumulation")
				End If

				dataPointers.put(i, arrays(i).data().addressPointer())
			Next i


			nativeOps.accumulate(Nothing, dataPointers, CType(arrays(0).shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing, target.data().addressPointer(), CType(target.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing, arrays.Length, len)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			Return target
		End Function

		''' <summary>
		''' This method averages input arrays, and returns averaged array
		''' </summary>
		''' <param name="target"> </param>
		''' <param name="arrays">
		''' @return </param>
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

			Dim len As Long = If(target IsNot Nothing, target.length(), arrays(0).length())

			Dim dataPointers As New PointerPointer(arrays.Length)
			Dim firstType As val = arrays(0).dataType()

			For i As Integer = 0 To arrays.Length - 1
				Nd4j.Compressor.autoDecompress(arrays(i))

				Preconditions.checkArgument(arrays(i).dataType() = firstType, "All arrays must have the same data type")

				If arrays(i).elementWiseStride() <> 1 Then
					Throw New ND4JIllegalStateException("Native averaging is applicable only to continuous INDArrays")
				End If

				If arrays(i).length() <> len Then
					Throw New ND4JIllegalStateException("All arrays should have equal length for averaging")
				End If

				dataPointers.put(i, arrays(i).data().addressPointer())
			Next i


			nativeOps.average(Nothing, dataPointers, CType(arrays(0).shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing,If(target Is Nothing, Nothing, target.data().addressPointer()),If(target Is Nothing, Nothing, CType(target.shapeInfoDataBuffer().addressPointer(), LongPointer)), Nothing, Nothing, arrays.Length, len, True)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			Return target
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

		Public Overrides Function average(ByVal arrays() As INDArray) As INDArray
			If arrays Is Nothing OrElse arrays.Length = 0 Then
				Throw New Exception("Input arrays are missing")
			End If

			Dim ret As INDArray = Nd4j.createUninitialized(arrays(0).dataType(), arrays(0).shape(), arrays(0).ordering())

			Return average(ret, arrays)
		End Function

		Public Overrides Function average(ByVal arrays As ICollection(Of INDArray)) As INDArray
			Return average(arrays.ToArray())
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
		''' along a specified set of dimensions. All arrays
		''' </summary>
		''' <param name="array">     the ndarray to shuffle </param>
		''' <param name="dimension"> the dimension to do the shuffle
		''' @return </param>
		Public Overrides Sub shuffle(ByVal array As ICollection(Of INDArray), ByVal rnd As Random, ParamArray ByVal dimension() As Integer)
			shuffle(New List(Of INDArray)(array), rnd, Collections.singletonList(dimension))
		End Sub

		''' <summary>
		''' Symmetric in place shuffle of an ndarray
		''' along a specified set of dimensions. Each array in list should have it's own dimension at the same index of dimensions array
		''' </summary>
		''' <param name="arrays">      the ndarrays to shuffle </param>
		''' <param name="dimensions"> the dimensions to do the shuffle
		''' @return </param>
		Public Overrides Sub shuffle(ByVal arrays As IList(Of INDArray), ByVal rnd As Random, ByVal dimensions As IList(Of Integer()))
			If dimensions Is Nothing OrElse dimensions.Count = 0 Then
				Throw New Exception("Dimension can't be null or 0-length")
			End If

			If arrays Is Nothing OrElse arrays.Count = 0 Then
				Throw New Exception("No input arrays provided")
			End If

			If dimensions.Count > 1 AndAlso arrays.Count <> dimensions.Count Then
				Throw New System.InvalidOperationException("Number of dimensions do not match number of arrays to shuffle")
			End If

			Dim zero As val = arrays(0)
			Dim tadLength As Integer = 1
			If zero.rank() > 1 Then
				Dim i As Integer = 0
				Do While i < dimensions(0).Length
					tadLength *= zero.shape()(dimensions(0)(i))
					i += 1
				Loop
			End If

			Dim numTads As Long = zero.length() \ tadLength

			Dim map As val = ArrayUtil.buildInterleavedVector(rnd, CInt(numTads))

			Dim dataPointers As val = New PointerPointer(arrays.Count)
			Dim shapePointers As val = New PointerPointer(arrays.Count)
			Dim tadPointers As val = New PointerPointer(arrays.Count)
			Dim offsetPointers As val = New PointerPointer(arrays.Count)

			Dim dummy As val = New PointerPointer(New Pointer() {Nothing})

			Dim list As IList(Of Pair(Of DataBuffer, DataBuffer)) = New List(Of Pair(Of DataBuffer, DataBuffer))()

			Dim tadManager As val = Nd4j.Executioner.TADManager

			Dim ptrMap As val = New IntPointer(map)

			Dim ptrs(arrays.Count - 1) As Long


			For i As Integer = 0 To arrays.Count - 1
				Dim array As val = arrays(i)

				Nd4j.Compressor.autoDecompress(array)

				Dim dimension As val = If(dimensions.Count > 1, dimensions(i), dimensions(0))

				Dim tadBuffers As val = tadManager.getTADOnlyShapeInfo(array, dimension)
				list.Add(tadBuffers)

				Dim hostTadShapeInfo As val = tadBuffers.getFirst().addressPointer()

				Dim offsets As val = tadBuffers.getSecond()

				If array.rank() <> 1 AndAlso offsets.length() <> numTads Then
					Throw New ND4JIllegalStateException("Can't symmetrically shuffle arrays with non-equal number of TADs")
				End If

				If offsets Is Nothing Then
					Throw New ND4JIllegalStateException("Offsets for shuffle can't be null")
				End If

				dataPointers.put(i, array.data().addressPointer())
				shapePointers.put(i, array.shapeInfoDataBuffer().addressPointer())
				offsetPointers.put(i, offsets.addressPointer())
				tadPointers.put(i, tadBuffers.getFirst().addressPointer())
			Next i


			nativeOps.shuffle(dummy, dataPointers, shapePointers, Nothing, Nothing, dataPointers, shapePointers, Nothing, Nothing, arrays.Count, ptrMap, tadPointers, offsetPointers)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			dataPointers.address()
			shapePointers.address()
			tadPointers.address()
			offsetPointers.address()
		End Sub

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
			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			source.Data = buffer

			If TypeOf buffer Is CompressedDataBuffer Then
				source.markAsCompressed(True)
			Else
				source.markAsCompressed(False)
			End If

			Return source
		End Function

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

			Dim buffer As DataBuffer = Nothing


			If CompressionUtils.goingToCompress(typeSrc, typeDst) Then
				' all types below 6 are compression modes
				Dim pointer As New BytePointer(source.length() * elementSize)
				Dim descriptor As New CompressionDescriptor(source, typeDst.ToString())
				descriptor.setCompressionType(CompressionType.LOSSY)
				descriptor.setCompressedLength(source.length() * elementSize)
				buffer = New CompressedDataBuffer(pointer, descriptor)
			Else
				Dim compressed As CompressedDataBuffer = DirectCast(source, CompressedDataBuffer)
				Dim descriptor As CompressionDescriptor = compressed.getCompressionDescriptor()

				' decompression mode
				buffer = Nd4j.createBuffer(descriptor.getNumberOfElements(), True)
			End If

			convertDataEx(typeSrc, source, typeDst, buffer)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If

			Return buffer
		End Function

		Public Overrides Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As Pointer, ByVal typeDst As DataTypeEx, ByVal target As Pointer, ByVal length As Long)
			nativeOps.convertTypes(Nothing, typeSrc.ordinal(), source, length, typeDst.ordinal(), target)

			If nativeOps.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps.lastErrorMessage())
			End If
		End Sub

		Public Overrides Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As Pointer, ByVal typeDst As DataTypeEx, ByVal buffer As DataBuffer)
			convertDataEx(typeSrc, source, typeDst, buffer.addressPointer(), buffer.length())
		End Sub

		Public Overrides Sub convertDataEx(ByVal typeSrc As DataTypeEx, ByVal source As DataBuffer, ByVal typeDst As DataTypeEx, ByVal target As DataBuffer)
			convertDataEx(typeSrc, source.addressPointer(), typeDst, target.addressPointer(), target.length())
		End Sub

		Public Overrides Function sort(ByVal x As INDArray, ByVal descending As Boolean) As INDArray
			If x.Scalar Then
				Return x
			End If


			NativeOpsHolder.Instance.getDeviceNativeOps().sort(Nothing, x.data().addressPointer(), CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing, descending)

			Return x
		End Function

		Public Overrides Function sort(ByVal x As INDArray, ByVal descending As Boolean, ParamArray ByVal dimension() As Integer) As INDArray
			If x.Scalar Then
				Return x
			End If

			Array.Sort(dimension)
			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(x, dimension)


			NativeOpsHolder.Instance.getDeviceNativeOps().sortTad(Nothing, x.data().addressPointer(), CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing, CType(Nd4j.ConstantHandler.getConstantBuffer(dimension, DataType.INT).addressPointer(), IntPointer), dimension.Length, CType(tadBuffers.First.addressPointer(), LongPointer), New LongPointerWrapper(tadBuffers.Second.addressPointer()), descending)


			Return x
		End Function

		Public Overrides Function sortCooIndices(ByVal x As INDArray) As INDArray
			Throw New System.NotSupportedException("Not an COO ndarray")
		End Function


		Public Overrides Function create(ByVal strings As ICollection(Of String), ByVal shape() As Long, ByVal order As Char) As INDArray
			Dim pairShape As val = Nd4j.ShapeInfoProvider.createShapeInformation(shape, order, DataType.UTF8)
			Dim buffer As val = New Utf8Buffer(strings)
			Dim list As val = New List(Of )(strings)
			Return Nd4j.createArrayFromShapeBuffer(buffer, pairShape)
		End Function

		Public Overrides Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal paddings() As Long, ByVal paddingOffsets() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace) As INDArray
			Return New NDArray(dataType, shape, paddings, paddingOffsets, ordering, workspace)
		End Function
	End Class

End Namespace