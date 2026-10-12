Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports var = lombok.var
Imports org.bytedeco.javacpp
Imports LongIndexer = org.bytedeco.javacpp.indexer.LongIndexer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports DeviceTADManager = org.nd4j.jita.allocator.tad.DeviceTADManager
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports Nd4jEnvironment = org.nd4j.linalg.api.environment.Nd4jEnvironment
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayStatistics = org.nd4j.linalg.api.ndarray.INDArrayStatistics
Imports org.nd4j.linalg.api.ops
Imports Aggregate = org.nd4j.linalg.api.ops.aggregates.Aggregate
Imports org.nd4j.linalg.api.ops.aggregates
Imports DefaultOpExecutioner = org.nd4j.linalg.api.ops.executioner.DefaultOpExecutioner
Imports OpStatus = org.nd4j.linalg.api.ops.executioner.OpStatus
Imports ScatterUpdate = org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports BaseRandomOp = org.nd4j.linalg.api.ops.random.BaseRandomOp
Imports Random = org.nd4j.linalg.api.rng.Random
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports TadPack = org.nd4j.linalg.api.shape.TadPack
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = org.nd4j.linalg.api.shape.options.ArrayType
Imports TADManager = org.nd4j.linalg.cache.TADManager
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AddressRetriever = org.nd4j.linalg.jcublas.buffer.AddressRetriever
Imports BaseCudaDataBuffer = org.nd4j.linalg.jcublas.buffer.BaseCudaDataBuffer
Imports CudaLongDataBuffer = org.nd4j.linalg.jcublas.buffer.CudaLongDataBuffer
Imports CudaUtf8Buffer = org.nd4j.linalg.jcublas.buffer.CudaUtf8Buffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports org.nd4j.common.primitives
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

Namespace org.nd4j.linalg.jcublas.ops.executioner




	''' <summary>
	''' JCuda executioner.
	''' <p/>
	''' Runs ops directly on the gpu
	''' 
	''' If requested Op doesn't exist within GPU context, DefaultOpExecutioner will be used, with arrays/buffers updated after that.
	''' 
	''' @author Adam Gibson
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudaExecutioner extends org.nd4j.linalg.api.ops.executioner.DefaultOpExecutioner
	Public Class CudaExecutioner
		Inherits DefaultOpExecutioner

'JAVA TO VB CONVERTER NOTE: The field nativeOps was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shared nativeOps_Conflict As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()

		'    private static final Allocator allocator = AtomicAllocator.getInstance();

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected static org.nd4j.linalg.cache.TADManager tadManager = new org.nd4j.jita.allocator.tad.DeviceTADManager();
'JAVA TO VB CONVERTER NOTE: The field tadManager was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shared tadManager_Conflict As TADManager = New DeviceTADManager()
		Protected Friend extraz As New ThreadLocal(Of PointerPointer)()
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile transient Properties properties;
		<NonSerialized>
		Protected Friend properties As Properties

'JAVA TO VB CONVERTER NOTE: The field lastOp was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend lastOp_Conflict As New ThreadLocal(Of String)()

		Protected Friend customOps As IDictionary(Of String, CustomOpDescriptor) = Nothing

'JAVA TO VB CONVERTER NOTE: The field experimentalMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend experimentalMode_Conflict As New AtomicBoolean(False)

		Public Sub New()
			experimentalMode_Conflict.set(nativeOps_Conflict.isExperimentalEnabled())
		End Sub

		Public Overridable ReadOnly Property NativeOps As NativeOps
			Get
				Return nativeOps_Conflict
			End Get
		End Property

		Public Overrides ReadOnly Property LastOp As String
			Get
				Return lastOp_Conflict.get()
			End Get
		End Property

		Public Overrides Function exec(ByVal op As BroadcastOp) As INDArray
			Dim st As Long = profilingConfigurableHookIn(op)

			checkForCompression(op)

			Dim dimension As val = op.dimensions().toIntVector()

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			Dim hostYShapeInfo As Pointer = If(op.y() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.y().shapeInfoDataBuffer()))
			Dim hostZShapeInfo As Pointer = If(op.z() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.z().shapeInfoDataBuffer()))

			Dim x As val = If(op.x() Is Nothing, Nothing, DirectCast(op.x().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim y As val = If(op.y() Is Nothing, Nothing, DirectCast(op.y().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim z As val = If(op.z() Is Nothing, Nothing, DirectCast(op.z().data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			Dim xShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(op.x().shapeInfoDataBuffer(), context)

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.x(), dimension)

			Dim hostTadShapeInfo As Pointer = AddressRetriever.retrieveHostPointer(tadBuffers.First)
			Dim devTadShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(tadBuffers.First, context)

			Dim offsets As DataBuffer = tadBuffers.Second
			Dim devTadOffsets As Pointer = AtomicAllocator.Instance.getPointer(offsets, context)

			Dim devTadShapeInfoZ As Pointer = Nothing
			Dim devTadOffsetsZ As Pointer = Nothing

			' that's the place where we're going to have second TAD in place
			Dim tadBuffersZ As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.z(), dimension)

			devTadShapeInfoZ = AtomicAllocator.Instance.getPointer(tadBuffersZ.First, context)
			devTadOffsetsZ = AtomicAllocator.Instance.getPointer(tadBuffersZ.Second, context)
			'        }

			' extraz.get().put
			' new PointerPointer
			Dim xShapeInfoHostPointer As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(op.x().shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, hostTadShapeInfo, devTadShapeInfo, devTadOffsets, devTadShapeInfoZ, devTadOffsetsZ)

			'Pointer dimensionPointer = AtomicAllocator.getInstance().getPointer(Nd4j.createBuffer(dimension), context);
			Dim dimensionPointer As Pointer = AtomicAllocator.Instance.getPointer(AtomicAllocator.Instance.getConstantBuffer(dimension), context)

			Select Case op.OpType
				Case BROADCAST
					nativeOps_Conflict.execBroadcast(xShapeInfoHostPointer, op.opNum(), x, CType(AtomicAllocator.Instance.getHostPointer(op.x().shapeInfoDataBuffer()), LongPointer), CType(xShapeInfo, LongPointer), y, CType(AtomicAllocator.Instance.getHostPointer(op.y().shapeInfoDataBuffer()), LongPointer), CType(AtomicAllocator.Instance.getPointer(op.y().shapeInfoDataBuffer(),context), LongPointer), z, CType(AtomicAllocator.Instance.getHostPointer(op.z().shapeInfoDataBuffer()), LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(AtomicAllocator.Instance.getHostPointer(op.dimensions().shapeInfoDataBuffer()), LongPointer), CType(AtomicAllocator.Instance.getPointer(op.dimensions().shapeInfoDataBuffer(), context), LongPointer))
				Case BROADCAST_BOOL
					nativeOps_Conflict.execBroadcastBool(xShapeInfoHostPointer, op.opNum(), x, CType(AtomicAllocator.Instance.getHostPointer(op.x().shapeInfoDataBuffer()), LongPointer), CType(xShapeInfo, LongPointer), y, CType(AtomicAllocator.Instance.getHostPointer(op.y().shapeInfoDataBuffer()), LongPointer), CType(AtomicAllocator.Instance.getPointer(op.y().shapeInfoDataBuffer(),context), LongPointer), z, CType(AtomicAllocator.Instance.getHostPointer(op.z().shapeInfoDataBuffer()), LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(AtomicAllocator.Instance.getHostPointer(op.dimensions().shapeInfoDataBuffer()), LongPointer), CType(AtomicAllocator.Instance.getPointer(op.dimensions().shapeInfoDataBuffer(), context), LongPointer))
				Case Else
					Throw New System.NotSupportedException("Unknown op type: " & op.OpType)
			End Select

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, Nothing, st)

			Return op.z()
		End Function

		''' 
		''' <param name="op"> </param>
		''' <param name="dimension">
		''' @return </param>
		Protected Friend Overridable Function naiveExec(ByVal op As ReduceOp, ParamArray ByVal dimension() As Integer) As INDArray
			Dim st As Long = profilingConfigurableHookIn(op)

			If TypeOf op Is BaseReduceOp AndAlso DirectCast(op, BaseReduceOp).isEmptyReduce() Then
				'Edge case for TF import compatibility: [x,y].reduce(empty) = [x,y]
				'Note that "empty" axis is NOT the same as length 0, as in INDArray.sum(new int[0]), which means "all dimensions"
				If op.z() IsNot Nothing Then
					Preconditions.checkState(op.x().equalShapes(op.z()), "For empty reductions, result (z) array must have same shape as x shape." & " Got: x=%ndShape, z=%ndShape", op.x(), op.z())
					op.z().assign(op.x())
					Return op.z()
				Else
					op.Z = op.x().dup()
					Return op.z()
				End If
			End If

			Dim ret As INDArray = op.z()

			checkForCompression(op)
			op.validateDataTypes(Nothing)
			'validateDataType(Nd4j.dataType(), op);

			Dim i As Integer = 0
			Do While i < dimension.Length
				If dimension(i) >= op.x().rank() AndAlso dimension(i) <> Integer.MaxValue Then
					Throw New ND4JIllegalStateException("Op target dimension " & java.util.Arrays.toString(dimension) & " contains element that higher then rank of op.X: [" & op.x().rank() & "]")
				End If
				i += 1
			Loop

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			Dim hostXShapeInfo As val = If(op.x() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.x().shapeInfoDataBuffer()))
			Dim hostYShapeInfo As val = If(op.y() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.y().shapeInfoDataBuffer()))
			Dim hostZShapeInfo As val = If(op.z() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.z().shapeInfoDataBuffer()))

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.x(), dimension)

			Dim hostTadShapeInfo As Pointer = AddressRetriever.retrieveHostPointer(tadBuffers.First)
			Dim devTadShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(tadBuffers.First, context)

			Dim offsets As DataBuffer = tadBuffers.Second
			Dim devTadOffsets As Pointer = If(offsets Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(offsets, context))

			Dim xShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(op.x().shapeInfoDataBuffer(), context)

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim xShapeInfoHostPointer As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(op.x().shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, hostTadShapeInfo, devTadShapeInfo, devTadOffsets)

			Dim yDevTadOffsets As Pointer = Nothing
			Dim yDevTadShapeInfo As Pointer = Nothing

			If op.y() IsNot Nothing Then
				If dimension.Length = 0 OrElse (dimension.Length = 1 AndAlso dimension(0) = Integer.MaxValue) OrElse op.x().tensorAlongDimension(0, dimension).length() <> op.y().length() Then
					If Not op.ComplexAccumulation AndAlso op.x().length() <> op.y().length() Then
						Throw New ND4JIllegalStateException("Op.X [" & op.x().length() & "] and Op.Y [" & op.y().length() & "] lengths should match")
					End If

					If Not op.z().Scalar Then
						Dim yTadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.y(), dimension)

						yDevTadShapeInfo = AtomicAllocator.Instance.getPointer(yTadBuffers.First, context)

						Dim yOffsets As DataBuffer = yTadBuffers.Second
						yDevTadOffsets = If(yOffsets Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(yOffsets, context))

						xShapeInfoHostPointer.put(12, yDevTadShapeInfo)
						xShapeInfoHostPointer.put(13, yDevTadOffsets)
					End If
				Else
					' TAD vs full array code branch
					Dim fakeOffsets As val = Nd4j.ConstantHandler.getConstantBuffer(New Integer() {0, 0}, DataType.LONG)
					yDevTadOffsets = If(fakeOffsets Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(fakeOffsets, context))

					yDevTadShapeInfo = AtomicAllocator.Instance.getPointer(op.y().shapeInfoDataBuffer(), context)

					xShapeInfoHostPointer.put(12, AtomicAllocator.Instance.getPointer(op.y().shapeInfoDataBuffer(), context))
					xShapeInfoHostPointer.put(13, Nothing)
				End If
			End If

			Dim argsType As DataType
			Select Case op.OpType
				Case REDUCE_LONG, REDUCE_BOOL
					argsType = op.x().dataType()
				Case Else
					argsType = op.z().dataType()
			End Select

			Dim extraArgs As Pointer = If(op.extraArgs() IsNot Nothing, AtomicAllocator.Instance.getPointer(op.extraArgsDataBuff(argsType), context), Nothing)
			Dim dimensionPointer As Pointer = AtomicAllocator.Instance.getPointer(AtomicAllocator.Instance.getConstantBuffer(dimension), context) 'AtomicAllocator.getInstance().getPointer(Nd4j.createBuffer(dimension), context);

			Dim x As val = If(op.x() Is Nothing, Nothing, DirectCast(op.x().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim y As val = If(op.y() Is Nothing, Nothing, DirectCast(op.y().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim z As val = If(op.z() Is Nothing, Nothing, DirectCast(op.z().data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			If TypeOf op Is Variance Then
				If ret.Scalar Then
					nativeOps_Conflict.execSummaryStatsScalar(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer()), LongPointer), DirectCast(op, Variance).BiasCorrected)
				Else
					nativeOps_Conflict.execSummaryStatsTad(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op, Variance).BiasCorrected, CType(devTadShapeInfo, LongPointer), CType(devTadOffsets, LongPointer))
				End If
			ElseIf op.y() IsNot Nothing Then
				If op.ComplexAccumulation Then

					Dim dT As val = New LongPointerWrapper(devTadOffsets)
					Dim yT As val = New LongPointerWrapper(yDevTadOffsets)

					nativeOps_Conflict.execReduce3All(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, y, CType(hostYShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.y().shapeInfoDataBuffer(),context), LongPointer), z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, CType(devTadShapeInfo, LongPointer), dT, CType(yDevTadShapeInfo, LongPointer), yT)
				ElseIf ret.Scalar Then
					nativeOps_Conflict.execReduce3Scalar(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, y, CType(hostYShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.y().shapeInfoDataBuffer(), context), LongPointer), z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer))
				Else
					nativeOps_Conflict.execReduce3Tad(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, y, CType(hostYShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.y().shapeInfoDataBuffer(), context), LongPointer), z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, CType(devTadShapeInfo, LongPointer), CType(devTadOffsets, LongPointer), CType(yDevTadShapeInfo, LongPointer), CType(yDevTadOffsets, LongPointer))
				End If
			Else
					If ret.Scalar Then
						Select Case op.OpType
							Case REDUCE_FLOAT
								nativeOps_Conflict.execReduceFloat(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer),CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer()), LongPointer))
							Case REDUCE_BOOL
								nativeOps_Conflict.execReduceBool(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer()), LongPointer))
							Case REDUCE_LONG
								nativeOps_Conflict.execReduceLong(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer()), LongPointer))
							Case REDUCE_SAME
								nativeOps_Conflict.execReduceSame(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer),CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer()), LongPointer))
							Case Else
								Throw New System.NotSupportedException()
						End Select
					Else
						Select Case op.OpType
							Case REDUCE_FLOAT
								nativeOps_Conflict.execReduceFloat2(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
							Case REDUCE_BOOL
								nativeOps_Conflict.execReduceBool2(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
							Case REDUCE_SAME
								nativeOps_Conflict.execReduceSame2(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
							Case REDUCE_LONG
								nativeOps_Conflict.execReduceLong2(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context), LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
							Case Else
								Throw New System.NotSupportedException()
						End Select
					End If
			End If

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, Nothing, st)

			Return op.z()
		End Function

		Public Overrides Function exec(ByVal op As Variance) As INDArray
			Return exec(DirectCast(op, ReduceOp))
		End Function

		Public Overrides Function exec(ByVal op As ReduceOp) As INDArray
			checkForCompression(op)

			If TypeOf op Is BaseReduceOp AndAlso DirectCast(op, BaseReduceOp).isEmptyReduce() Then
				'Edge case for TF import compatibility: [x,y].reduce(empty) = [x,y]
				'Note that "empty" axis is NOT the same as length 0, as in INDArray.sum(new int[0]), which means "all dimensions"
				If op.z() IsNot Nothing Then
					Preconditions.checkState(op.x().equalShapes(op.z()), "For empty reductions, result (z) array must have same shape as x shape." & " Got: x=%ndShape, z=%ndShape", op.x(), op.z())
					op.z().assign(op.x())
					Return op.z()
				Else
					op.Z = op.x().dup()
					Return op.z()
				End If
			End If

			Dim dimension As val = op.dimensions().toIntVector()

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim maxShape As val = Shape.getMaxShape(op.x(),op.y())

			Dim wholeDims As val = Shape.wholeArrayDimension(dimension) OrElse op.x().rank() = dimension.length OrElse dimension.length = 0
			Dim retShape As val = Shape.reductionShape(If(op.y() Is Nothing, op.x(), If(op.x().length() > op.y().length(), op.x(), op.y())), dimension, True, op.KeepDims)

			If op.x().Vector AndAlso op.x().length() = ArrayUtil.prod(retShape) AndAlso ArrayUtil.prodLong(retShape) > 1 AndAlso op.y() Is Nothing Then
				Return op.noOp()
			End If

			Dim dtype As val = op.resultType()
			Dim ret As INDArray = Nothing
			If op.z() Is Nothing OrElse op.z() Is op.x() Then
				If op.ComplexAccumulation Then
					Dim xT As val = op.x().tensorsAlongDimension(dimension)
					Dim yT As val = op.y().tensorsAlongDimension(dimension)

					' we intentionally want to set it to 0.0
					ret = Nd4j.createUninitialized(dtype, New Long() {xT, yT})
				Else
					If op.y() IsNot Nothing Then
						'2 options here: either pairwise, equal sizes - OR every X TAD vs. entirety of Y
						If op.x().length() = op.y().length() Then
							'Pairwise
							If Not wholeDims AndAlso op.x().tensorsAlongDimension(dimension) <> op.y().tensorsAlongDimension(dimension) Then
								Throw New ND4JIllegalStateException("Number of TADs along dimension don't match: (x shape = " & java.util.Arrays.toString(op.x().shape()) & ", y shape = " & java.util.Arrays.toString(op.y().shape()) & ", dimension = " & java.util.Arrays.toString(dimension) & ")")
							End If
						Else
							If dimension.length = 0 Then
								Throw New ND4JIllegalStateException("TAD vs TAD comparison requires dimension (or other comparison mode was supposed to be used?)")
							End If

							'Every X TAD vs. entirety of Y
							Dim xTADSize As val = op.x().length() \ op.x().tensorsAlongDimension(dimension)

							If xTADSize <> op.y().length() Then
								Throw New ND4JIllegalStateException("Size of TADs along dimension don't match for pairwise execution:" & " (x TAD size = " & xTADSize & ", y size = " & op.y().length())
							End If
						End If
					End If

					' in case of regular accumulation we don't care about array state before op
					ret = Nd4j.create(dtype, retShape)
				End If
				op.Z = ret
			Else
				' compare length

				If op.z().length() <> (If(retShape.length = 0, 1, ArrayUtil.prodLong(retShape))) Then
					Throw New ND4JIllegalStateException("Shape of target array for reduction [" & java.util.Arrays.toString(op.z().shape()) & "] doesn't match expected [" & java.util.Arrays.toString(retShape) & "]")
				End If
			End If

			Dim st As Long = profilingConfigurableHookIn(op)
			naiveExec(op, dimension)

			profilingConfigurableHookOut(op, Nothing, st)

			Return op.z()
		End Function

		Public Overrides Function exec(ByVal op As IndexAccumulation) As INDArray
			Dim dimension As val = Shape.normalizeAxis(op.x().rank(), op.dimensions().toIntVector())

			If op.x().Empty Then
				For Each d As val In dimension
					Preconditions.checkArgument(op.x().shape()(d) <> 0, "IndexReduce can't be issued along axis with 0 in shape")
				Next d
			End If

			If op.z() Is Nothing Then
				Dim retShape As val = Shape.reductionShape(op.x(), dimension, True, op.KeepDims)
				op.Z = Nd4j.createUninitialized(DataType.LONG, retShape)
			End If

			Dim st As Long = profilingConfigurableHookIn(op)

			checkForCompression(op)

			'validateDataType(Nd4j.dataType(), op);

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			If op.x().Vector AndAlso op.x().length() = op.z().length() Then
				Return op.x()
			End If

			If op.z().Empty Then
				Return op.z()
			End If

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			Dim hostXShapeInfo As val = If(op.x() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.x().shapeInfoDataBuffer()))
			Dim hostYShapeInfo As val = If(op.y() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.y().shapeInfoDataBuffer()))
			Dim hostZShapeInfo As val = If(op.z() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.z().shapeInfoDataBuffer()))

			Dim xShapeInfo As val = AtomicAllocator.Instance.getPointer(op.x().shapeInfoDataBuffer(), context)
			Dim zShapeInfo As val = AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context)

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.x(), dimension)

			Dim hostTadShapeInfo As val = AddressRetriever.retrieveHostPointer(tadBuffers.First)
			Dim devTadShapeInfo As val = AtomicAllocator.Instance.getPointer(tadBuffers.First, context)

			Dim offsets As val = tadBuffers.Second
			Dim devTadOffsets As val = If(offsets Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(offsets, context))

			Dim xShapeInfoHostPointer As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(op.x().shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, hostTadShapeInfo, devTadShapeInfo, devTadOffsets)
			Dim extraArgs As Pointer = If(op.extraArgs() IsNot Nothing, AtomicAllocator.Instance.getPointer(op.extraArgsDataBuff(op.x().dataType()), context), Nothing)
			'Pointer dimensionPointer = AtomicAllocator.getInstance().getPointer(Nd4j.createBuffer(dimension), context);
			Dim dimensionPointer As Pointer = AtomicAllocator.Instance.getPointer(AtomicAllocator.Instance.getConstantBuffer(dimension), context)

			Dim x As val = If(op.x() Is Nothing, Nothing, DirectCast(op.x().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim y As val = If(op.y() Is Nothing, Nothing, DirectCast(op.y().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim z As val = If(op.z() Is Nothing, Nothing, DirectCast(op.z().data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			nativeOps_Conflict.execIndexReduce(xShapeInfoHostPointer, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, z, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, Nothing, st)

			Return op.z()
		End Function


		Public Overrides Function exec(ByVal op As Op) As INDArray
			Return exec(op, Nothing)
		End Function

		Public Overrides Function exec(ByVal op As Op, ByVal oc As OpContext) As INDArray
			checkForCompression(op)

	'        
	'        //TODO this never would have worked
	'        //linear views and oblong offsets can't be handled by the gpu (due to the way the buffers are interpreted as vectors)
	'        if ( op instanceof CopyOp) {
	'            // we dont' care about op.Z sync state, since it'll be overwritten
	'            if (op.x() != null)
	'                AtomicAllocator.getInstance().synchronizeHostData(op.x());
	'            if (op.y() != null)
	'                AtomicAllocator.getInstance().synchronizeHostData(op.y());
	'
	'            super.exec(op);
	'
	'            if (op.z() != null)
	'                throw new UnsupportedOperationException("Pew-pew");
	'                //AtomicAllocator.getInstance().tickHostWrite(op.z());
	'
	'            return null;
	'        }

			If TypeOf op Is TransformOp Then
				Dim t As TransformOp = DirectCast(op, TransformOp)
				invoke(t, oc)
			ElseIf TypeOf op Is ReduceOp Then
				Dim acc As ReduceOp = DirectCast(op, ReduceOp)
				invoke(acc, oc, acc.dimensions().toIntVector())
			ElseIf TypeOf op Is ScalarOp Then
				Dim sc As ScalarOp = DirectCast(op, ScalarOp)
				invoke(sc, oc)
			ElseIf TypeOf op Is BroadcastOp Then
				Dim broadcastOp As BroadcastOp = DirectCast(op, BroadcastOp)
				invoke(broadcastOp, oc)
			ElseIf TypeOf op Is IndexAccumulation Then
				Dim indexAccumulation As IndexAccumulation = DirectCast(op, IndexAccumulation)
				invoke(indexAccumulation, oc, indexAccumulation.dimensions().toIntVector())
			ElseIf TypeOf op Is RandomOp Then
				exec(DirectCast(op, RandomOp), oc, Nd4j.Random)
			ElseIf TypeOf op Is CustomOp Then
				exec(DirectCast(op, CustomOp), oc)
			End If


			Return op.z()
		End Function


		Public Overrides Function execAndReturn(ByVal op As TransformOp) As TransformOp
			checkForCompression(op)
			invoke(op, Nothing)
			Return op
		End Function



		Protected Friend Overridable Function invoke(ByVal op As BroadcastOp, ByVal oc As OpContext) As CudaContext
			Dim st As Long = profilingConfigurableHookIn(op)

			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

			checkForCompression(op)

			'validateDataType(Nd4j.dataType(), op);

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			Dim xShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(x.shapeInfoDataBuffer(), context)


			Dim hostXShapeInfo As val = If(x Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()))
			Dim hostYShapeInfo As val = If(y Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(y.shapeInfoDataBuffer()))
			Dim hostZShapeInfo As val = If(z Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(z.shapeInfoDataBuffer()))

			Dim tadBuffers As val = tadManager_Conflict.getTADOnlyShapeInfo(x, op.Dimension)

			Dim hostTadShapeInfo As val = AddressRetriever.retrieveHostPointer(tadBuffers.getFirst())
			Dim devTadShapeInfo As val = AtomicAllocator.Instance.getPointer(tadBuffers.getFirst(), context)

			Dim offsets As val = tadBuffers.getSecond()
			Dim devTadOffsets As val = AtomicAllocator.Instance.getPointer(offsets, context)

			Dim devTadShapeInfoZ As Pointer = Nothing
			Dim devTadOffsetsZ As Pointer = Nothing

			' that's the place where we're going to have second TAD in place
			Dim tadBuffersZ As val = tadManager_Conflict.getTADOnlyShapeInfo(z, op.Dimension)

			devTadShapeInfoZ = AtomicAllocator.Instance.getPointer(tadBuffersZ.getFirst(), context)
			devTadOffsetsZ = AtomicAllocator.Instance.getPointer(tadBuffersZ.getSecond(), context)

			Dim xShapeInfoHostPointer As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, hostTadShapeInfo, devTadShapeInfo, devTadOffsets, devTadShapeInfoZ, devTadOffsetsZ) ' 13

			Dim yShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(y.shapeInfoDataBuffer(), context)

			Dim zShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(z.shapeInfoDataBuffer(), context)
			Dim dimensionPointer As Pointer = AtomicAllocator.Instance.getPointer(AtomicAllocator.Instance.getConstantBuffer(op.Dimension), context)

			Dim xb As val = If(x Is Nothing, Nothing, DirectCast(x.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim yb As val = If(y Is Nothing, Nothing, DirectCast(y.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim zb As val = If(z Is Nothing, Nothing, DirectCast(z.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			'log.info("X: {}; Y: {}; Z: {}; dTS: {}, dTO: {}; dTSz: {}; dTOz: {};", x.address(), y.address(), z.address(), devTadShapeInfo.address(), devTadOffsets.address(), devTadShapeInfoZ.address(), devTadOffsetsZ.address());

			Select Case op.OpType
				Case BROADCAST
					nativeOps_Conflict.execBroadcast(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), yb, CType(hostYShapeInfo, LongPointer), CType(yShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
				Case BROADCAST_BOOL
					nativeOps_Conflict.execBroadcastBool(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), yb, CType(hostYShapeInfo, LongPointer), CType(yShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
				Case Else
					Throw New System.NotSupportedException("Unknown opType: " & op.OpType)
			End Select

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)

			Return Nothing
		End Function



		Protected Friend Overridable Function invoke(ByVal op As IndexAccumulation, ByVal oc As OpContext, ByVal dimension() As Integer) As CudaContext
			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

			dimension = Shape.normalizeAxis(x.rank(), dimension)
			If dimension Is Nothing OrElse (dimension.Length = 1 AndAlso dimension(0) = Integer.MaxValue) Then
				If z Is x OrElse z Is Nothing Then
					z = Nd4j.createUninitialized(DataType.LONG, New Long(){}, "c"c)
					setZ(z, op, oc)
				End If
			End If

			Dim keepDims As Boolean = op.KeepDims
			Dim retShape() As Long = Shape.reductionShape(x, dimension, True, keepDims)

			If z Is Nothing OrElse x Is z Then
				Dim ret As val = Nd4j.createUninitialized(DataType.LONG, retShape)

				setZ(ret, op, oc)
				z = ret
			ElseIf Not retShape.SequenceEqual(z.shape()) Then
				Throw New System.InvalidOperationException("Z array shape does not match expected return type for op " & op & ": expected shape " & java.util.Arrays.toString(retShape) & ", z.shape()=" & java.util.Arrays.toString(z.shape()))
			End If

			Dim st As Long = profilingConfigurableHookIn(op)

			checkForCompression(op)

			'validateDataType(Nd4j.dataType(), op);

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If
			CudaEnvironment.Instance.Configuration.enableDebug(True)
			If dimension IsNot Nothing Then
				Dim i As Integer = 0
				Do While i < dimension.Length
					If dimension(i) >= x.rank() AndAlso dimension(i) <> Integer.MaxValue Then
						Throw New ND4JIllegalStateException("Op target dimension " & java.util.Arrays.toString(dimension) & " contains element that higher then rank of op.X: [" & x.rank() & "]")
					End If
					i += 1
				Loop
			End If

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			Dim xShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(x.shapeInfoDataBuffer(), context)
			Dim extraArgs As Pointer = If(op.extraArgs() IsNot Nothing, AtomicAllocator.Instance.getPointer(op.extraArgsDataBuff(x.dataType()), context), Nothing)

			Dim hostXShapeInfo As val = If(x Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()))
			Dim hostYShapeInfo As val = If(y Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(y.shapeInfoDataBuffer()))
			Dim hostZShapeInfo As val = If(z Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(z.shapeInfoDataBuffer()))

			Dim fdimension() As Integer = dimension
			If fdimension Is Nothing Then
				fdimension = New Integer() {0}
			End If

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(x, fdimension)

			Dim hostTadShapeInfo As Pointer = AddressRetriever.retrieveHostPointer(tadBuffers.First)
			Dim devTadShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(tadBuffers.First, context)

			Dim offsets As DataBuffer = tadBuffers.Second
			Dim devTadOffsets As Pointer = If(offsets Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(offsets, context))
			Dim zShapeInfo As val = AtomicAllocator.Instance.getPointer(z.shapeInfoDataBuffer(), context)

			Dim xb As val = If(x Is Nothing, Nothing, DirectCast(x.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim zb As val = If(z Is Nothing, Nothing, DirectCast(z.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			Dim xShapeInfoHostPointer As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, hostTadShapeInfo, devTadShapeInfo, devTadOffsets)

			If z.Scalar OrElse dimension Is Nothing OrElse dimension(0) = Integer.MaxValue Then
					nativeOps_Conflict.execIndexReduceScalar(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer))
			Else
				If dimension IsNot Nothing AndAlso dimension.Length > 1 Then
					Array.Sort(dimension)
				End If

				'long dimensionPointer = AtomicAllocator.getInstance().getPointer(Nd4j.createBuffer(dimension), context);
				Dim dimensionPointer As Pointer = AtomicAllocator.Instance.getHostPointer(AtomicAllocator.Instance.getConstantBuffer(dimension))

				nativeOps_Conflict.execIndexReduce(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
			End If

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)

			Return Nothing

		End Function


		Protected Friend Overridable Function invoke(ByVal op As ReduceOp, ByVal oc As OpContext, ByVal dimension() As Integer) As CudaContext
			Dim context As val = AtomicAllocator.Instance.DeviceContext

			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

			If TypeOf op Is BaseReduceOp AndAlso DirectCast(op, BaseReduceOp).isEmptyReduce() Then
				'Edge case for TF import compatibility: [x,y].reduce(empty) = [x,y]
				'Note that "empty" axis is NOT the same as length 0, as in INDArray.sum(new int[0]), which means "all dimensions"
				If z IsNot Nothing Then
					Preconditions.checkState(x.equalShapes(z), "For empty reductions, result (z) array must have same shape as x shape." & " Got: x=%ndShape, z=%ndShape", x, z)
					z.assign(x)
					Return context
				Else
					op.Z = x.dup()
					Return context
				End If
			End If

			' FIXME: this should be moved down to C++ on per-op basis
			' reduce to scalar case, ReduceBool ops require special treatment
			If TypeOf op Is BaseReduceBoolOp AndAlso x.Empty AndAlso (dimension Is Nothing OrElse (dimension.Length = 1 AndAlso dimension(0) = Integer.MaxValue)) Then
				If z Is Nothing Then
					op.Z = Nd4j.scalar(DirectCast(op, BaseReduceBoolOp).emptyValue())
				Else
					z.assign(DirectCast(op, BaseReduceBoolOp).emptyValue())
				End If

				Return context
			End If

			Dim st As Long = profilingConfigurableHookIn(op)

			checkForCompression(op)

			dimension = Shape.normalizeAxis(x.rank(), dimension)

			'validateDataType(Nd4j.dataType(), op);

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			' dimension is ALWAYS null here.
			If dimension Is Nothing Then
				dimension = New Integer() {Integer.MaxValue}
			End If

			If dimension IsNot Nothing AndAlso dimension.Length > 1 Then
				Array.Sort(dimension)
			End If

			Dim i As Integer = 0
			Do While i < dimension.Length
				If dimension(i) >= x.rank() AndAlso dimension(i) <> Integer.MaxValue Then
					Throw New ND4JIllegalStateException("Op target dimension " & java.util.Arrays.toString(dimension) & " contains element that higher then rank of op.X: [" & x.rank() & "]")
				End If
				i += 1
			Loop

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			Dim tadBuffers As val = If(x.Empty, Pair.makePair(Of DataBuffer, DataBuffer)(x.data(), Nothing), tadManager_Conflict.getTADOnlyShapeInfo(x, dimension))

			Dim hostTadShapeInfo As val = AddressRetriever.retrieveHostPointer(tadBuffers.getFirst())
			Dim devTadShapeInfo As val = AtomicAllocator.Instance.getPointer(tadBuffers.getFirst(), context)

			Dim offsets As val = If(x.Empty, Nothing, tadBuffers.getSecond())
			Dim devTadOffsets As val = If(offsets Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(offsets, context))

			Dim xShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(x.shapeInfoDataBuffer(), context)

			Dim retShape() As Long = Shape.reductionShape(x, dimension, True, op.KeepDims)

			If y IsNot Nothing Then
				'2 options here: either pairwise, equal sizes - OR every X TAD vs. entirety of Y
				If x.length() = y.length() Then
					'Pairwise
					If x.tensorsAlongDimension(dimension) <> y.tensorsAlongDimension(dimension) Then
						Throw New ND4JIllegalStateException("Number of TADs along dimension don't match: (x shape = " & java.util.Arrays.toString(x.shape()) & ", y shape = " & java.util.Arrays.toString(y.shape()) & ", dimension = " & java.util.Arrays.toString(dimension) & ")")
					End If
				Else
					'Every X TAD vs. entirety of Y
					Dim xTADSize As val = x.length() \ x.tensorsAlongDimension(dimension)

					If xTADSize <> y.length() Then
						Throw New ND4JIllegalStateException("Size of TADs along dimension don't match for pairwise execution:" & " (x TAD size = " & xTADSize & ", y size = " & y.length())
					End If
				End If
			End If

			'if (x.isVector() && x.length() == ArrayUtil.prod(retShape)) {
			'    return null;
			'}

			Dim dataType As val = If(oc IsNot Nothing, op.resultType(oc), op.resultType())

			If z Is Nothing Then
				Dim ret As val = Nd4j.createUninitialized(dataType, retShape)
				setZ(ret, op, oc)
				z = ret
			ElseIf z.dataType() <> dataType OrElse Not retShape.SequenceEqual(z.shape()) Then
				Throw New ND4JIllegalStateException("Output array for op " & op.GetType().Name & " should have type " & dataType & " and shape " & java.util.Arrays.toString(retShape) & " but has datatype " & z.dataType() & " and shape " & java.util.Arrays.toString(z.shape()))
			End If

			Dim eb As val = op.extraArgsDataBuff(If(z.dataType() = DataType.BOOL OrElse op.OpType = Op.Type.REDUCE_LONG, x.dataType(), z.dataType()))
			Dim extraArgs As Pointer = If(op.extraArgs() IsNot Nothing, AtomicAllocator.Instance.getPointer(eb, context), Nothing)

			Dim hostXShapeInfo As val = If(x Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()))
			Dim hostYShapeInfo As val = If(y Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(y.shapeInfoDataBuffer()))
			Dim hostZShapeInfo As val = If(z Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(z.shapeInfoDataBuffer()))

			Dim xShapeInfoHostPointer As val = extraz.get().put(AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, hostTadShapeInfo, devTadShapeInfo, devTadOffsets)

			Dim yTadBuffers As val = If(y Is Nothing, Nothing, tadManager_Conflict.getTADOnlyShapeInfo(y, dimension))

			Dim yDevTadShapeInfo As val = If(y Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(yTadBuffers.getFirst(), context))
			Dim yOffsets As val = If(y Is Nothing, Nothing, yTadBuffers.getSecond())
			Dim yDevTadOffsets As val = If(yOffsets Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(yOffsets, context))

			If y IsNot Nothing Then
				xShapeInfoHostPointer.put(12, yDevTadShapeInfo)
				xShapeInfoHostPointer.put(13, yDevTadOffsets)
			End If

			Dim zShapeInfo As val = AtomicAllocator.Instance.getPointer(z.shapeInfoDataBuffer(), context)

			Dim xb As val = If(x Is Nothing, Nothing, DirectCast(x.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim yb As val = If(y Is Nothing, Nothing, DirectCast(y.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim zb As val = If(z Is Nothing, Nothing, DirectCast(z.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			op.validateDataTypes(Nothing)

			If z.Scalar Then
				If TypeOf op Is Variance Then
					nativeOps_Conflict.execSummaryStatsScalar(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op, Variance).BiasCorrected)
				ElseIf y IsNot Nothing Then
					Dim yShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(y.shapeInfoDataBuffer(), context)
						nativeOps_Conflict.execReduce3Scalar(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, yb, CType(hostYShapeInfo, LongPointer), CType(yShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer))
				Else
					Select Case op.OpType
						Case REDUCE_FLOAT
							nativeOps_Conflict.execReduceFloat(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer))
						Case REDUCE_BOOL
							nativeOps_Conflict.execReduceBool(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer))
						Case REDUCE_SAME
							nativeOps_Conflict.execReduceSame(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer))
						Case REDUCE_LONG
							nativeOps_Conflict.execReduceLong(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer))
						Case Else
							Throw New System.NotSupportedException()
					End Select
				End If
			Else
				Dim dimensionPointer As val = AtomicAllocator.Instance.getPointer(AtomicAllocator.Instance.getConstantBuffer(dimension), context) 'AtomicAllocator.getInstance().getPointer(Nd4j.createBuffer(dimension), context);

				If y IsNot Nothing Then
					Dim yShapeInfo As val = AtomicAllocator.Instance.getPointer(y.shapeInfoDataBuffer(), context)
					nativeOps_Conflict.execReduce3Tad(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, yb, CType(hostYShapeInfo, LongPointer), CType(yShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, CType(devTadShapeInfo, LongPointer), CType(devTadOffsets, LongPointer), CType(yDevTadShapeInfo, LongPointer), CType(yDevTadOffsets, LongPointer))
				Else
					If TypeOf op Is Variance Then
						nativeOps_Conflict.execSummaryStatsTad(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op, Variance).BiasCorrected, CType(devTadShapeInfo, LongPointer), CType(devTadOffsets, LongPointer))
					Else
						Select Case op.OpType
							Case REDUCE_FLOAT
								nativeOps_Conflict.execReduceFloat2(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
							Case REDUCE_SAME
								nativeOps_Conflict.execReduceSame2(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
							Case REDUCE_BOOL
								nativeOps_Conflict.execReduceBool2(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
							Case REDUCE_LONG
								nativeOps_Conflict.execReduceLong2(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), extraArgs, zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
							Case Else
								Throw New System.NotSupportedException()
						End Select
					End If
				End If
			End If

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)

			Nd4j.Executioner.commit()

			Return context
		End Function


		Protected Friend Overridable Function intercept(ByVal op As ScalarOp, ByVal dimension() As Integer) As CudaContext
			Dim st As Long = profilingConfigurableHookIn(op)

			If dimension IsNot Nothing AndAlso dimension.Length > 1 Then
				Array.Sort(dimension)
			End If

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			Dim hostXShapeInfo As val = If(op.x() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.x().shapeInfoDataBuffer()))
			Dim hostYShapeInfo As val = If(op.y() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.y().shapeInfoDataBuffer()))
			Dim hostZShapeInfo As val = If(op.z() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.z().shapeInfoDataBuffer()))

			Dim xShapeInfo As val = AtomicAllocator.Instance.getPointer(op.x().shapeInfoDataBuffer(), context)
			Dim yShapeInfo As val = AtomicAllocator.Instance.getPointer(op.y().shapeInfoDataBuffer(), context)
			Dim zShapeInfo As val = AtomicAllocator.Instance.getPointer(op.z().shapeInfoDataBuffer(), context)

			Dim tadBuffers As val = tadManager_Conflict.getTADOnlyShapeInfo(op.x(), dimension)

			Dim hostTadShapeInfo As val = AddressRetriever.retrieveHostPointer(tadBuffers.getFirst())
			Dim devTadShapeInfo As val = AtomicAllocator.Instance.getPointer(tadBuffers.getFirst(), context)

			Dim offsets As val = tadBuffers.getSecond()
			Dim devTadOffsets As val = AtomicAllocator.Instance.getPointer(offsets, context)

			Dim devTadShapeInfoZ As Pointer = Nothing
			Dim devTadOffsetsZ As Pointer = Nothing

			Dim tadBuffersZ As val = tadManager_Conflict.getTADOnlyShapeInfo(op.z(), dimension)

			devTadShapeInfoZ = AtomicAllocator.Instance.getPointer(tadBuffersZ.getFirst(), context)
			devTadOffsetsZ = AtomicAllocator.Instance.getPointer(tadBuffersZ.getSecond(), context)


			Dim extraPointers As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(op.x().shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, hostTadShapeInfo, devTadShapeInfo, devTadOffsets, devTadShapeInfoZ, devTadOffsetsZ)

			Dim extraArgs As val = If(op.extraArgs() IsNot Nothing, AtomicAllocator.Instance.getPointer(op.extraArgsDataBuff(op.z().dataType()), context), Nothing)

			Dim dimensionPointer As val = AtomicAllocator.Instance.getPointer(AtomicAllocator.Instance.getConstantBuffer(dimension), context)

			Dim x As val = If(op.x() Is Nothing, Nothing, DirectCast(op.x().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim y As val = If(op.y() Is Nothing, Nothing, DirectCast(op.y().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim z As val = If(op.z() Is Nothing, Nothing, DirectCast(op.z().data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			Select Case op.OpType
				Case SCALAR
					nativeOps_Conflict.execScalarTad(extraPointers, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), z, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), y, CType(hostYShapeInfo, LongPointer), CType(yShapeInfo, LongPointer), extraArgs, DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, CType(devTadShapeInfo, LongPointer), CType(devTadOffsets, LongPointer), CType(devTadShapeInfoZ, LongPointer), CType(devTadOffsetsZ, LongPointer))
				Case SCALAR_BOOL
					nativeOps_Conflict.execScalarBoolTad(extraPointers, op.opNum(), x, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), z, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), y, CType(hostYShapeInfo, LongPointer), CType(yShapeInfo, LongPointer), extraArgs, DirectCast(op.dimensions().data(), BaseCudaDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, CType(devTadShapeInfo, LongPointer), CType(devTadOffsets, LongPointer), CType(devTadShapeInfoZ, LongPointer), CType(devTadOffsetsZ, LongPointer))
				Case Else
					Throw New System.NotSupportedException()
			End Select

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, Nothing, st)

			Return Nothing
		End Function

		Public Overrides Function exec(ByVal op As ScalarOp) As INDArray
			invoke(op, Nothing)
			Return op.z()
		End Function

		Protected Friend Overridable Function invoke(ByVal op As ScalarOp, ByVal oc As OpContext) As CudaContext
			Dim st As Long = profilingConfigurableHookIn(op)

			checkForCompression(op)

			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

	'        validateDataType(Nd4j.dataType(), op);

			If z Is Nothing Then
				Select Case op.OpType
					Case SCALAR
						z = x.ulike()
						setZ(x.ulike(), op, oc)
					Case SCALAR_BOOL
						z = Nd4j.createUninitialized(DataType.BOOL, x.shape())
						setZ(z, op, oc)
					Case Else
						Throw New ND4JIllegalStateException("Unknown op type: [" & op.OpType & "]")
				End Select
			End If

			If x.length() <> z.length() Then
				Throw New ND4JIllegalStateException("op.X length should be equal to op.Y length: [" & java.util.Arrays.toString(x.shapeInfoDataBuffer().asInt()) & "] != [" & java.util.Arrays.toString(z.shapeInfoDataBuffer().asInt()) & "]")
			End If

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			If op.dimensions() IsNot Nothing Then
				intercept(op, op.dimensions().toIntVector())
				Return Nothing
			End If

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			Dim hostXShapeInfo As val = If(x Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()))
			Dim hostYShapeInfo As val = If(op.scalar() Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(op.scalar().shapeInfoDataBuffer()))
			Dim hostZShapeInfo As val = If(z Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(z.shapeInfoDataBuffer()))

			Dim xShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(x.shapeInfoDataBuffer(), context)
			Dim extraArgs As Pointer = If(op.extraArgs() IsNot Nothing, AtomicAllocator.Instance.getPointer(op.extraArgsDataBuff(If(op.OpType = Op.Type.SCALAR_BOOL, x.dataType(), z.dataType())), context), Nothing)

			Dim zShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(z.shapeInfoDataBuffer(), context)

			Dim xShapeInfoHostPointer As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, Nothing, Nothing)

			Dim xb As val = If(x Is Nothing, Nothing, DirectCast(x.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim yb As val = If(op.scalar() Is Nothing, Nothing, DirectCast(op.scalar().data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim zb As val = If(z Is Nothing, Nothing, DirectCast(z.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			Select Case op.OpType
				Case SCALAR_BOOL
					nativeOps_Conflict.execScalarBool(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), yb, CType(hostYShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.scalar().shapeInfoDataBuffer(), context), LongPointer), extraArgs)
				Case SCALAR
					nativeOps_Conflict.execScalar(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), yb, CType(hostYShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(op.scalar().shapeInfoDataBuffer(), context), LongPointer), extraArgs)
				Case Else
					Throw New System.NotSupportedException("Unknown op type: " & op.OpType)
			End Select

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)

			Return Nothing
		End Function

		Protected Friend Overridable Function invoke(ByVal op As TransformOp, ByVal oc As OpContext) As CudaContext
			Dim st As Long = profilingConfigurableHookIn(op)

			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

			checkForCompression(op)

			'validateDataType(Nd4j.dataType(), op);

			Dim allocator As AtomicAllocator = AtomicAllocator.Instance

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim context As val = allocator.DeviceContext

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			' special temp array for IsMax along dimension
			Dim ret As INDArray = Nothing

			Dim xShapeInfo As Pointer = allocator.getPointer(x.shapeInfoDataBuffer(), context)


			Dim dimensionDevPointer As Pointer = Nothing
			Dim dimensionHostPointer As Pointer = Nothing
			Dim retPointer As Pointer = Nothing
			Dim retHostShape As Pointer = Nothing
			Dim dimension() As Integer = Nothing

			Dim hostXShapeInfo As val = If(x Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()))
			Dim hostYShapeInfo As var = If(y Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(y.shapeInfoDataBuffer()))


			If z Is Nothing Then
				ret = Nd4j.createUninitialized(op.resultType(), x.shape(), x.ordering())
				setZ(ret, op, oc)
				z = ret
			End If

			Dim extraArgs As var = If(op.extraArgs() IsNot Nothing, allocator.getPointer(op.extraArgsDataBuff(If(op.OpType = Op.Type.TRANSFORM_BOOL OrElse op.OpType = Op.Type.PAIRWISE_BOOL, x.dataType(), z.dataType())), context), Nothing)
			Dim hostZShapeInfo As val = If(z Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(z.shapeInfoDataBuffer()))

			Dim hostTadShapeInfo As Pointer = Nothing
			Dim devTadShapeInfo As Pointer = Nothing

			Dim hostMaxTadShapeInfo As Pointer = Nothing
			Dim devMaxTadShapeInfo As Pointer = Nothing

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer)
			Dim tadMaxBuffers As Pair(Of DataBuffer, DataBuffer)

			Dim devTadOffsets As Pointer = Nothing
			Dim devMaxTadOffsets As Pointer = Nothing

			op.validateDataTypes(oc, experimentalMode_Conflict.get())

			Dim zShapeInfo As Pointer = allocator.getPointer(z.shapeInfoDataBuffer(), context)


			Dim xShapeInfoHostPointer As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()), context.getOldStream(), allocator.DeviceIdPointer, context.getBufferAllocation(), context.getBufferReduction(), context.getBufferScalar(), context.getBufferSpecial(), hostYShapeInfo, hostZShapeInfo, hostTadShapeInfo, devTadShapeInfo, devTadOffsets, hostMaxTadShapeInfo, devMaxTadShapeInfo, devMaxTadOffsets, dimensionDevPointer, dimensionHostPointer, retPointer, New CudaPointer(If(dimension Is Nothing, 0, dimension.Length)), retHostShape)


			Dim xb As val = If(x Is Nothing, Nothing, DirectCast(x.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim yb As val = If(y Is Nothing, Nothing, DirectCast(y.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim zb As val = If(z Is Nothing, Nothing, DirectCast(z.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			If y IsNot Nothing Then
				Dim yShapeInfo As Pointer = allocator.getPointer(y.shapeInfoDataBuffer(), context)

				If x.length() <> y.length() OrElse x.length() <> z.length() Then
					Throw New ND4JIllegalStateException("X, Y and Z arguments should have the same length for PairwiseTransform")
				End If

				Select Case op.OpType
					Case TRANSFORM_BOOL, PAIRWISE_BOOL
						nativeOps_Conflict.execPairwiseTransformBool(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), yb, CType(hostYShapeInfo, LongPointer), CType(yShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), extraArgs)
					Case Else
						nativeOps_Conflict.execPairwiseTransform(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), yb, CType(hostYShapeInfo, LongPointer), CType(yShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), extraArgs)
				End Select
			Else
				Select Case op.OpType
					Case TRANSFORM_ANY
						nativeOps_Conflict.execTransformAny(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), extraArgs)
					Case TRANSFORM_FLOAT
						nativeOps_Conflict.execTransformFloat(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), extraArgs)
					Case TRANSFORM_BOOL
						nativeOps_Conflict.execTransformBool(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), extraArgs)
					Case TRANSFORM_SAME
						nativeOps_Conflict.execTransformSame(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), extraArgs)
					Case TRANSFORM_STRICT
						nativeOps_Conflict.execTransformStrict(xShapeInfoHostPointer, op.opNum(), xb, CType(hostXShapeInfo, LongPointer), CType(xShapeInfo, LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(zShapeInfo, LongPointer), extraArgs)
					Case Else
						Throw New System.NotSupportedException()
				End Select
			End If

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			If extraArgs IsNot Nothing Then
				extraArgs.address()
			End If

			If ret IsNot Nothing Then
				ret.elementWiseStride()
			End If

			profilingConfigurableHookOut(op, oc, st)

			Return Nothing
		End Function

		Protected Friend Overridable Function getBuffer(Of T As Aggregate)(ByVal batch As Batch(Of T)) As DataBuffer
			Dim buffer As DataBuffer = Nd4j.DataBufferFactory.createInt(batch.getSample().getRequiredBatchMemorySize() * 4, False)
			batch.setParamsSurface(buffer)
			Return buffer
		End Function

		Public Overrides Sub exec(Of T As Aggregate)(ByVal batch As Batch(Of T))
			Throw New System.NotSupportedException("Pew-pew")
		End Sub

		Public Overrides Sub exec(ByVal batch As IList(Of Aggregate))
			If batch.Count = 0 Then
				Return
			End If

			Dim batches As IList(Of Batch(Of Aggregate)) = Batch.getBatches(batch, 8192)
			For Each [single] As Batch(Of Aggregate) In batches
				Me.exec([single])
			Next [single]

			Dim context As val = AtomicAllocator.Instance.DeviceContext
			context.syncOldStream()
		End Sub

		Public Overrides Sub exec(ByVal op As Aggregate)
			Throw New System.NotSupportedException("Pew-pew")
		End Sub

		''' <summary>
		''' This method executes specified RandomOp using default RNG available via Nd4j.getRandom()
		''' </summary>
		''' <param name="op"> </param>
		Public Overrides Function exec(ByVal op As RandomOp) As INDArray
			Return exec(op, Nd4j.Random)
		End Function


		Public Overrides Function exec(ByVal op As RandomOp, ByVal rng As Random) As INDArray
			Return exec(op, Nothing, rng)
		End Function

		Public Overridable Overloads Function exec(ByVal op As RandomOp, ByVal oc As OpContext, ByVal rng As Random) As INDArray
			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

			If TypeOf op Is BaseRandomOp AndAlso DirectCast(op, BaseRandomOp).TripleArgRngOp AndAlso z IsNot Nothing AndAlso x Is Nothing AndAlso y Is Nothing Then
				'Ugly hack to ensure the triple arg call occurs
				'See GaussianDistribution.setZ etc
				x = z
				y = z
			End If

			Dim st As Long = profilingConfigurableHookIn(op)

			checkForCompression(op)

			'validateDataType(Nd4j.dataType(), op);

			If rng.StatePointer Is Nothing Then
				Throw New System.InvalidOperationException("You should use one of NativeRandom classes for NativeOperations execution")
			End If

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			If CudaEnvironment.Instance.Configuration.isDebug() Then
				lastOp_Conflict.set(op.opName())
			End If

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			Dim extraZZ As PointerPointer = extraz.get().put(AddressRetriever.retrieveHostPointer(z.shapeInfoDataBuffer()), context.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer)

			Dim hostXShapeInfo As val = If(x Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(x.shapeInfoDataBuffer()))
			Dim hostYShapeInfo As val = If(y Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(y.shapeInfoDataBuffer()))
			Dim hostZShapeInfo As val = If(z Is Nothing, Nothing, AddressRetriever.retrieveHostPointer(z.shapeInfoDataBuffer()))

			Dim xb As val = If(x Is Nothing, Nothing, DirectCast(x.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim yb As val = If(y Is Nothing, Nothing, DirectCast(y.data(), BaseCudaDataBuffer).OpaqueDataBuffer)
			Dim zb As val = If(z Is Nothing, Nothing, DirectCast(z.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			If x IsNot Nothing AndAlso y IsNot Nothing AndAlso z IsNot Nothing Then
				' triple arg call
				nativeOps_Conflict.execRandom3(extraZZ, op.opNum(), rng.StatePointer, xb, CType(hostXShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(x.shapeInfoDataBuffer(), context), LongPointer), yb, CType(hostYShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(y.shapeInfoDataBuffer(), context), LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(z.shapeInfoDataBuffer(), context), LongPointer), AtomicAllocator.Instance.getPointer(op.extraArgsDataBuff(z.dataType()), context))

			ElseIf x IsNot Nothing AndAlso z IsNot Nothing Then
				'double arg call
				nativeOps_Conflict.execRandom2(extraZZ, op.opNum(), rng.StatePointer, xb, CType(hostXShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(x.shapeInfoDataBuffer(), context), LongPointer), zb, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(z.shapeInfoDataBuffer(), context), LongPointer), AtomicAllocator.Instance.getPointer(op.extraArgsDataBuff(z.dataType()),context))


			Else
				' single arg call
				nativeOps_Conflict.execRandom(extraZZ, op.opNum(), rng.StatePointer, zb, CType(hostZShapeInfo, LongPointer), CType(AtomicAllocator.Instance.getPointer(z.shapeInfoDataBuffer(), context), LongPointer), AtomicAllocator.Instance.getPointer(op.extraArgsDataBuff(z.dataType()), context))
			End If

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)

			Return z
		End Function

		''' <summary>
		''' This method return set of key/value
		''' and key/key/value objects,
		''' describing current environment
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property EnvironmentInformation As Properties
			Get
				SyncLock Me
					If properties Is Nothing Then
						Dim props As Properties = MyBase.EnvironmentInformation
            
						Dim devicesList As IList(Of IDictionary(Of String, Object)) = New List(Of IDictionary(Of String, Object))()
            
						' fill with per-device information: name, memory, versions
						Dim i As Integer = 0
						Do While i < nativeOps_Conflict.getAvailableDevices()
							Dim deviceProps As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
            
							deviceProps(Nd4jEnvironment.CUDA_DEVICE_NAME_KEY) = nativeOps_Conflict.getDeviceName(i)
							deviceProps(Nd4jEnvironment.CUDA_FREE_MEMORY_KEY) = nativeOps_Conflict.getDeviceFreeMemory(i)
							deviceProps(Nd4jEnvironment.CUDA_TOTAL_MEMORY_KEY) = nativeOps_Conflict.getDeviceTotalMemory(i)
							deviceProps(Nd4jEnvironment.CUDA_DEVICE_MAJOR_VERSION_KEY) = CLng(Math.Truncate(nativeOps_Conflict.getDeviceMajor(i)))
							deviceProps(Nd4jEnvironment.CUDA_DEVICE_MINOR_VERSION_KEY) = CLng(Math.Truncate(nativeOps_Conflict.getDeviceMinor(i)))
            
							devicesList.Insert(i, deviceProps)
							i += 1
						Loop
            
						' fill with basic general info
						props.put(Nd4jEnvironment.BACKEND_KEY, "CUDA")
						props.put(Nd4jEnvironment.CUDA_NUM_GPUS_KEY, nativeOps_Conflict.getAvailableDevices())
						props.put(Nd4jEnvironment.CUDA_DEVICE_INFORMATION_KEY, devicesList)
						props.put(Nd4jEnvironment.BLAS_VENDOR_KEY, (Nd4j.factory().blas()).BlasVendor.ToString())
						props.put(Nd4jEnvironment.HOST_FREE_MEMORY_KEY, Pointer.maxBytes() - Pointer.totalBytes())
            
						' fill bandwidth information
						props.put(Nd4jEnvironment.MEMORY_BANDWIDTH_KEY, PerformanceTracker.Instance.getCurrentBandwidth())
            
						properties = props
					Else
            
						Dim devicesList As IList(Of IDictionary(Of String, Object)) = CType(properties.get(Nd4jEnvironment.CUDA_DEVICE_INFORMATION_KEY), IList(Of IDictionary(Of String, Object)))
            
						' just update information that might change over time
						Dim i As Integer = 0
						Do While i < nativeOps_Conflict.getAvailableDevices()
							Dim dev As IDictionary(Of String, Object) = devicesList(i)
            
							dev(Nd4jEnvironment.CUDA_FREE_MEMORY_KEY) = nativeOps_Conflict.getDeviceFreeMemory(i)
							dev(Nd4jEnvironment.CUDA_TOTAL_MEMORY_KEY) = nativeOps_Conflict.getDeviceTotalMemory(i)
							i += 1
						Loop
            
						properties.put(Nd4jEnvironment.CUDA_DEVICE_INFORMATION_KEY, devicesList)
						properties.put(Nd4jEnvironment.HOST_FREE_MEMORY_KEY, Pointer.maxBytes() - Pointer.totalBytes())
            
						' fill bandwidth information
						properties.put(Nd4jEnvironment.MEMORY_BANDWIDTH_KEY, PerformanceTracker.Instance.getCurrentBandwidth())
					End If
					Return properties
				End SyncLock
			End Get
		End Property

		Public Overrides ReadOnly Property TADManager As TADManager
			Get
				Return tadManager_Conflict
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void printEnvironmentInformation()
		Public Overrides Sub printEnvironmentInformation()
			MyBase.printEnvironmentInformation()
		End Sub

		Public Overrides Sub commit()
			Dim ctx As val = AtomicAllocator.Instance.DeviceContext
			ctx.syncOldStream()
			ctx.syncSpecialStream()
		End Sub

		Public Overrides ReadOnly Property CustomOperations As IDictionary(Of String, CustomOpDescriptor)
			Get
				SyncLock Me
					If customOps Is Nothing Then
						Dim list As String = nativeOps_Conflict.getAllCustomOps()
            
						If list Is Nothing OrElse list.Length = 0 Then
							log.warn("No customs ops available!")
							customOps = java.util.Collections.emptyMap()
							Return customOps
						End If
            
						Dim map As val = New Dictionary(Of String, CustomOpDescriptor)()
            
						Dim split() As String = list.Split(";", True)
						For Each op As String In split
							If op Is Nothing OrElse op.Length = 0 Then
								Continue For
							End If
            
							Dim another() As String = op.Split(":", True)
            
							Dim descriptor As CustomOpDescriptor = CustomOpDescriptor.builder().hash(Convert.ToInt64(another(1))).numInputs(Convert.ToInt32(another(2))).numOutputs(Convert.ToInt32(another(3))).allowsInplace(Convert.ToInt32(another(4)) = 1).numTArgs(Convert.ToInt32(another(5))).numIArgs(Convert.ToInt32(another(6))).build()
            
							map.put(another(0), descriptor)
						Next op
            
						customOps = Collections.unmodifiableMap(map)
					End If
            
					Return customOps
				End SyncLock
			End Get
		End Property



		Protected Friend Overridable Function getShapeFromPointer(ByVal ptr As LongPointer) As LongShapeDescriptor
			Dim rank As val = CInt(Math.Truncate(ptr.get(0)))

			Dim shape As val = New Long(rank * 2 + 3){}
			For i As Integer = 0 To shape.length - 1
				shape(i) = ptr.get(i)
			Next i

			'val extras = ptr.get(Shape.shapeInfoLength(rank) - 3);
			Dim t As val = ArrayOptionsHelper.arrayType(shape)
			Return LongShapeDescriptor.fromShape(Shape.shape(shape), Shape.stride(shape), Shape.elementWiseStride(shape), Shape.order(shape), ArrayOptionsHelper.dataType(shape), t = ArrayType.EMPTY)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public List<org.nd4j.linalg.api.shape.LongShapeDescriptor> calculateOutputShape(@NonNull CustomOp op)
		Public Overrides Function calculateOutputShape(ByVal op As CustomOp) As IList(Of LongShapeDescriptor)
			Return calculateOutputShape(op, Nothing)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public List<org.nd4j.linalg.api.shape.LongShapeDescriptor> calculateOutputShape(@NonNull CustomOp op, OpContext opContext)
		Public Overrides Function calculateOutputShape(ByVal op As CustomOp, ByVal opContext As OpContext) As IList(Of LongShapeDescriptor)

			Nd4j.Executioner.commit()

			Dim lc As val = op.opName().ToLower()
			Dim hash As val = op.opHash()

			Dim result As val = New List(Of LongShapeDescriptor)()
			Dim nIn As Integer = If(opContext IsNot Nothing, opContext.numInputArguments(), op.numInputArguments())
			If nIn = 0 AndAlso op.Descriptor.getNumInputs() >= 1 Then
				If log.isTraceEnabled() Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.trace("Could not calculate output shape for op {}: number of input args was 0", op.GetType().FullName)
				End If
				Return java.util.Collections.emptyList()
			End If

			Dim inputBuffers As val = New PointerPointer(Of )(nIn * 2)
			Dim inputShapes As val = New PointerPointer(Of )(nIn)

			Dim inputArgs As val = If(opContext IsNot Nothing, opContext.getInputArrays(), op.inputArguments())
			Dim cnt As Integer= 0
			For Each [in] As val In inputArgs
				' TODO: once we implement Context-based shape function call this method should be removed
				Dim loc As val = Nd4j.AffinityManager.getActiveLocation([in])
				If loc <> AffinityManager.Location.DEVICE AndAlso loc <> AffinityManager.Location.EVERYWHERE Then
					Nd4j.AffinityManager.ensureLocation([in], AffinityManager.Location.DEVICE)
				End If

				' NOT A TYPO: shape functions work on host side only
				If Not [in].isEmpty() Then
					inputBuffers.put(cnt, [in].data().addressPointer())
					inputBuffers.put(cnt + nIn, AtomicAllocator.Instance.getPointer([in].data()))
				End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: inputShapes.put(cnt++, in.shapeInfoDataBuffer().addressPointer());
				inputShapes.put(cnt, [in].shapeInfoDataBuffer().addressPointer())
					cnt += 1
			Next [in]


			Dim nIArgs As Integer = If(opContext IsNot Nothing, opContext.numIArguments(), op.numIArguments())
			Dim iArgs As val = If(nIArgs > 0, New LongPointer(nIArgs), Nothing)
			cnt = 0
			If opContext IsNot Nothing Then
				For Each i As val In opContext.getIArguments()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: iArgs.put(cnt++, i);
					iArgs.put(cnt, i)
						cnt += 1
				Next i
			Else
				For Each i As val In op.iArgs()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: iArgs.put(cnt++, i);
					iArgs.put(cnt, i)
						cnt += 1
				Next i
			End If


			Dim nTArgs As Integer = If(opContext IsNot Nothing, opContext.numTArguments(), op.numTArguments())
			Dim tArgs As val = If(nTArgs > 0, New DoublePointer(nTArgs), Nothing)

			Dim nBArgs As Integer = If(opContext IsNot Nothing, opContext.numBArguments(), op.numBArguments())
			Dim bArgs As val = If(nBArgs > 0, New BooleanPointer(nBArgs), Nothing)

			Dim nDArgs As Integer = If(opContext IsNot Nothing, opContext.numDArguments(), op.numDArguments())
			Dim dArgs As val = If(nDArgs > 0, New IntPointer(nDArgs), Nothing)

			cnt = 0
			If opContext IsNot Nothing Then
				For Each b As val In opContext.getBArguments()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: bArgs.put(cnt++, b);
					bArgs.put(cnt, b)
						cnt += 1
				Next b
			Else
				For Each b As val In op.bArgs()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: bArgs.put(cnt++, b);
					bArgs.put(cnt, b)
						cnt += 1
				Next b
			End If


			cnt = 0
			If opContext IsNot Nothing Then
				For Each b As val In opContext.getTArguments()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: tArgs.put(cnt++, b);
					tArgs.put(cnt, b)
						cnt += 1
				Next b
			Else
				For Each b As val In op.tArgs()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: tArgs.put(cnt++, b);
					tArgs.put(cnt, b)
						cnt += 1
				Next b
			End If

			cnt = 0
			If opContext IsNot Nothing Then
				For Each b As val In opContext.getDArguments()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: dArgs.put(cnt++, b.toInt());
					dArgs.put(cnt, b.toInt())
						cnt += 1
				Next b
			Else
				For Each b As val In op.dArgs()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: dArgs.put(cnt++, b.toInt());
					dArgs.put(cnt, b.toInt())
						cnt += 1
				Next b
			End If

			Dim ptrptr As OpaqueShapeList = nativeOps_Conflict.calculateOutputShapes2(Nothing, hash, inputBuffers, inputShapes, nIn, tArgs, nTArgs, iArgs, nIArgs, bArgs, nBArgs, dArgs, nDArgs)
	'        OpaqueShapeList ptrptr = nativeOps.calculateOutputShapes2(null, hash, inputBuffers, inputShapes, op.inputArguments().size(), tArgs, op.tArgs().length, iArgs, op.iArgs().length, bArgs, op.numBArguments(), dArgs, op.numDArguments());

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			If ptrptr Is Nothing Then
				Throw New Exception()
			End If

			Dim e As Integer = 0
			Do While e < nativeOps_Conflict.getShapeListSize(ptrptr)
				result.add(getShapeFromPointer((New PagedPointer(nativeOps_Conflict.getShape(ptrptr, e))).asLongPointer()))
				e += 1
			Loop

			nativeOps_Conflict.deleteShapeList(ptrptr)


			Return result
		End Function

		''' <summary>
		''' This method executes given CustomOp
		''' 
		''' PLEASE NOTE: You're responsible for input/output validation
		''' PLEASE NOTE: right now this operations are executing on CPU </summary>
		''' <param name="op"> </param>
		Public Overrides Function exec(ByVal op As CustomOp) As INDArray()

			Nd4j.Executioner.commit()

			Dim shapeOverride As Boolean = False
			If op.numOutputArguments() = 0 AndAlso Not op.InplaceCall Then
				Try
					Dim list As val = Me.calculateOutputShape(op)
					If list.isEmpty() Then
						Throw New ND4JIllegalStateException("Op name " & op.opName() & " failed to execute. You can't execute non-inplace CustomOp without outputs being specified")
					End If

					For Each shape As val In list
						op.addOutputArgument(Nd4j.create(shape, False))
					Next shape

					shapeOverride = True
				Catch e As Exception
					Throw New ND4JIllegalStateException("Op name " & op.opName() & " - no output arrays were provided and calculateOutputShape failed to execute", e)
				End Try
			End If

			Dim ctx As val = AtomicAllocator.Instance.DeviceContext

			Dim name As val = op.opName()
			Try
					Using context As val = DirectCast(buildContext(), CudaOpContext)
        
					' optionally skip shape validation on op execution
					If shapeOverride Then
						context.shapeFunctionOverride(True)
					End If
        
					context.markInplace(op.InplaceCall)
        
					' transferring rng state
					context.setRngStates(Nd4j.Random.rootState(), Nd4j.Random.nodeState())
        
					'transferring input/output arrays
					context.setInputArrays(op.inputArguments())
					context.setOutputArrays(op.outputArguments())
        
					' transferring static args
					context.setBArguments(op.bArgs())
					context.setIArguments(op.iArgs())
					context.setTArguments(op.tArgs())
					context.setDArguments(op.dArgs())
        
					Dim result As val = exec(op, context)
					Dim states As val = context.getRngStates()
        
					' check if input && output needs update
					For Each [in] As val In op.inputArguments()
						If Not [in].isEmpty() Then
							CType([in].data(), BaseCudaDataBuffer).actualizePointerAndIndexer()
						End If
					Next [in]
        
					For Each [out] As val In op.outputArguments()
						If Not [out].isEmpty() Then
							CType([out].data(), BaseCudaDataBuffer).actualizePointerAndIndexer()
						End If
					Next [out]
        
					' pulling states back
					Nd4j.Random.setStates(states.getFirst(), states.getSecond())
        
					Return result
					End Using
			Catch e As ND4JOpProfilerException
				Throw e
			Catch e As Exception
				Throw New Exception("Op [" & name & "] execution failed", e)
			End Try
		End Function

		Public Overrides Sub enableDebugMode(ByVal reallyEnable As Boolean)
			debug_Conflict.set(reallyEnable)
			nativeOps_Conflict.enableDebugMode(reallyEnable)
		End Sub

		Public Overrides Sub enableVerboseMode(ByVal reallyEnable As Boolean)
			verbose_Conflict.set(reallyEnable)
			nativeOps_Conflict.enableVerboseMode(reallyEnable)
		End Sub

		Public Overrides Sub registerGraph(ByVal id As Long, ByVal graph As Pointer)
			nativeOps_Conflict.registerGraph(Nothing, id, graph)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Map<String, org.nd4j.linalg.api.ndarray.INDArray> executeGraph(long id, @NonNull Map<String, org.nd4j.linalg.api.ndarray.INDArray> map, @NonNull Map<String, Integer> reverseMap)
		Public Overrides Function executeGraph(ByVal id As Long, ByVal map As IDictionary(Of String, INDArray), ByVal reverseMap As IDictionary(Of String, Integer)) As IDictionary(Of String, INDArray)

			Nd4j.Executioner.commit()

			Dim ptrBuffers As val = New PointerPointer(map.Count * 2)
			Dim ptrShapes As val = New PointerPointer(map.Count * 2)
			Dim ptrIndices As val = New IntPointer(map.Count)

			Dim cnt As Integer = 0
			Dim keySet As val = New List(Of String)(map.Keys)
			For Each key As val In keySet
				Dim array As val = map(key)

				ptrBuffers.put(cnt, AtomicAllocator.Instance.getHostPointer(array))
				ptrShapes.put(cnt, AtomicAllocator.Instance.getHostPointer(array.shapeInfoDataBuffer()))
				ptrIndices.put(cnt, reverseMap(key))

				cnt += 1
			Next key

			Dim newMap As val = New LinkedHashMap(Of String, INDArray)()

			Dim result As OpaqueVariablesSet = nativeOps_Conflict.executeStoredGraph(Nothing, id, ptrBuffers, ptrShapes, ptrIndices, map.Count)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim status As OpStatus = OpStatus.byNumber(nativeOps_Conflict.getVariablesSetStatus(result))

			If status <> OpStatus.ND4J_STATUS_OK Then
				Throw New ND4JIllegalStateException("Op execution failed: " & status)
			End If

			Dim e As Integer = 0
			Do While e < nativeOps_Conflict.getVariablesSetSize(result)
				Dim var As OpaqueVariable = nativeOps_Conflict.getVariable(result, e)
				Dim nodeId As Integer = nativeOps_Conflict.getVariableId(var)
				Dim index As Integer = nativeOps_Conflict.getVariableIndex(var)
				Dim shapeInfo As LongPointer = nativeOps_Conflict.getVariableShape(var)
				Dim buffer As Pointer = nativeOps_Conflict.getVariableBuffer(var)

				Dim rank As val = CInt(Math.Truncate(shapeInfo.get(0)))
				Dim jshape As val = New Long(rank * 2 + 3){}
				For i As Integer = 0 To jshape.length - 1
					jshape(i) = shapeInfo.get(i)
				Next i

				Dim shapeOf As val = Shape.shapeOf(jshape)
				Dim stridesOf As val = Shape.stridesOf(jshape)
				Dim order As val = Shape.order(jshape)
				Dim array As val = Nd4j.create(shapeOf, stridesOf, 0, order)

				Pointer.memcpy(AtomicAllocator.Instance.getHostPointer(array), buffer, ArrayUtil.prod(shapeOf) * Nd4j.sizeOfDataType())
				'AtomicAllocator.getInstance().getAllocationPoint(array).tickHostWrite();
				If 1 > 0 Then
					Throw New System.NotSupportedException("Pew-pew")
				End If

				Dim nodeName As String = nativeOps_Conflict.getVariableName(var)
				newMap.put(nodeName, array)
				e += 1
			Loop

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			nativeOps_Conflict.deleteVariablesSet(result)

			Return newMap
		End Function

		Public Overrides Sub forgetGraph(ByVal id As Long)
			nativeOps_Conflict.unregisterGraph(Nothing, id)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If
		End Sub

		''' <summary>
		''' This method allows to set desired number of elements per thread, for performance optimization purposes.
		''' I.e. if array contains 2048 elements, and threshold is set to 1024, 2 threads will be used for given op execution.
		''' <para>
		''' Default value: 1024
		''' 
		''' </para>
		''' </summary>
		''' <param name="threshold"> </param>
		Public Overrides WriteOnly Property ElementsThreshold As Integer
			Set(ByVal threshold As Integer)
				nativeOps_Conflict.setElementThreshold(threshold)
			End Set
		End Property

		''' <summary>
		''' This method allows to set desired number of sub-arrays per thread, for performance optimization purposes.
		''' I.e. if matrix has shape of 64 x 128, and threshold is set to 8, each thread will be processing 8 sub-arrays (sure, if you have 8 core cpu).
		''' If your cpu has, say, 4, cores, only 4 threads will be spawned, and each will process 16 sub-arrays
		''' <para>
		''' Default value: 8
		''' 
		''' </para>
		''' </summary>
		''' <param name="threshold"> </param>
		Public Overrides WriteOnly Property TadThreshold As Integer
			Set(ByVal threshold As Integer)
				nativeOps_Conflict.setTADThreshold(threshold)
			End Set
		End Property


		Public Overrides Function type() As ExecutionerType
			Return ExecutionerType.CUDA
		End Function

		Public Overrides Function getString(ByVal buffer As DataBuffer, ByVal index As Long) As String
			Preconditions.checkArgument(TypeOf buffer Is CudaUtf8Buffer, "Expected Utf8Buffer")

			Dim addr As val = CType(buffer.indexer(), LongIndexer).get(index)
			Dim ptr As val = New PagedPointer(addr)
			Dim str As val = New Nd4jCuda.utf8string(ptr)
			Return str._buffer().capacity(str._length()).getString()
		End Function

		Public Overrides ReadOnly Property ExperimentalMode As Boolean
			Get
				Return experimentalMode_Conflict.get()
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void scatterUpdate(org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate.UpdateOp op, @NonNull INDArray array, @NonNull INDArray indices, @NonNull INDArray updates, @NonNull int[] axis)
		Public Overrides Sub scatterUpdate(ByVal op As ScatterUpdate.UpdateOp, ByVal array As INDArray, ByVal indices As INDArray, ByVal updates As INDArray, ByVal axis() As Integer)
			Dim context As val = AtomicAllocator.Instance.DeviceContext

			Dim tadX As val = tadManager_Conflict.getTADOnlyShapeInfo(array, axis)
			Dim tadY As val = tadManager_Conflict.getTADOnlyShapeInfo(updates, axis)

			If tadY.getSecond().length() <> indices.length() Then
				Throw New System.InvalidOperationException("Number of updates doesn't match number of indices. Bad dimensions used?")
			End If

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim stuff As val = extraz.get().put(Nothing, context.getOldStream())

			nativeOps_Conflict.scatterUpdate(stuff, op.ordinal(), CInt(indices.length()), Nothing, CType(AtomicAllocator.Instance.getHostPointer(tadX.getFirst()), LongPointer), Nothing, AtomicAllocator.Instance.getPointer(array, context), CType(AtomicAllocator.Instance.getPointer(tadX.getFirst()), LongPointer), CType(AtomicAllocator.Instance.getPointer(tadX.getSecond()), LongPointer), Nothing, CType(AtomicAllocator.Instance.getHostPointer(tadY.getFirst()), LongPointer), Nothing, AtomicAllocator.Instance.getPointer(updates, context), CType(AtomicAllocator.Instance.getPointer(tadY.getFirst()), LongPointer), CType(AtomicAllocator.Instance.getPointer(tadY.getSecond()), LongPointer), AtomicAllocator.Instance.getHostPointer(indices), CType(AtomicAllocator.Instance.getHostPointer(indices.shapeInfoDataBuffer()), LongPointer), AtomicAllocator.Instance.getPointer(indices, context), CType(AtomicAllocator.Instance.getPointer(indices.shapeInfoDataBuffer(), context), LongPointer))

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If
		End Sub

		Public Overrides Function buildContext() As OpContext
			Return New CudaOpContext()
		End Function

		Public Overrides Function exec(ByVal op As CustomOp, ByVal context As OpContext) As INDArray()
			Dim st As Long = profilingConfigurableHookIn(op, context)

			Dim ctx As val = AtomicAllocator.Instance.DeviceContext
			DirectCast(context, CudaOpContext).setCudaStream(ctx.getOldStream(), ctx.getBufferReduction(), ctx.getBufferAllocation())

			Dim status As val = nativeOps_Conflict.execCustomOp2(Nothing, op.opHash(), context.contextPointer())
			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			If status <> 0 Then
				Throw New Exception("Op [" & op.opName() & "] execution failed")
			End If

			profilingConfigurableHookOut(op, context, st)

			If context.getOutputArrays().Count = 0 Then
				Return New INDArray(){}
			Else
				Return CType(context.getOutputArrays(), List(Of INDArray)).ToArray()
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArrayStatistics inspectArray(@NonNull INDArray array)
		Public Overrides Function inspectArray(ByVal array As INDArray) As INDArrayStatistics
			Dim debugInfo As val = New Nd4jCuda.DebugInfo()
			Dim ctx As val = AtomicAllocator.Instance.DeviceContext
			AtomicAllocator.Instance.synchronizeHostData(array)

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim extras As val = extraz.get().put(Nothing, ctx.getOldStream(), AtomicAllocator.Instance.DeviceIdPointer, ctx.getBufferAllocation(), ctx.getBufferReduction(), ctx.getBufferScalar(), ctx.getBufferSpecial())


			nativeOps_Conflict.inspectArray(extras, AtomicAllocator.Instance.getHostPointer(array), CType(AtomicAllocator.Instance.getHostPointer(array.shapeInfoDataBuffer()), LongPointer), AtomicAllocator.Instance.getPointer(array, ctx), CType(AtomicAllocator.Instance.getPointer(array.shapeInfoDataBuffer()), LongPointer), debugInfo)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Return INDArrayStatistics.builder().minValue(debugInfo._minValue()).maxValue(debugInfo._maxValue()).meanValue(debugInfo._meanValue()).stdDevValue(debugInfo._stdDevValue()).countInf(debugInfo._infCount()).countNaN(debugInfo._nanCount()).countNegative(debugInfo._negativeCount()).countPositive(debugInfo._positiveCount()).countZero(debugInfo._zeroCount()).build()
		End Function


		Public Overrides Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal empty As Boolean) As DataBuffer
			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim dbf As val = nativeOps_Conflict.shapeBuffer(shape.Length, New LongPointer(shape), New LongPointer(stride), dtype.toInt(), order, elementWiseStride, empty)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim result As val = New CudaLongDataBuffer(nativeOps_Conflict.getConstantShapeBufferPrimary(dbf), nativeOps_Conflict.getConstantShapeBufferSpecial(dbf), Shape.shapeInfoLength(shape.Length))

			nativeOps_Conflict.deleteConstantShapeBuffer(dbf)

			Return result
		End Function

		Public Overrides Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal extras As Long) As DataBuffer
			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim dbf As val = nativeOps_Conflict.shapeBufferEx(shape.Length, New LongPointer(shape), New LongPointer(stride), dtype.toInt(), order, elementWiseStride, extras)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim result As val = New CudaLongDataBuffer(nativeOps_Conflict.getConstantShapeBufferPrimary(dbf), nativeOps_Conflict.getConstantShapeBufferSpecial(dbf), Shape.shapeInfoLength(shape.Length))

			nativeOps_Conflict.deleteConstantShapeBuffer(dbf)

			Return result
		End Function

		Public Overrides Function tadShapeInfoAndOffsets(ByVal array As INDArray, ByVal dimension() As Integer) As TadPack
			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim pack As OpaqueTadPack = nativeOps_Conflict.tadOnlyShapeInfo(CType(array.shapeInfoDataBuffer().addressPointer(), LongPointer), New IntPointer(dimension), dimension.Length)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim tadShape As val = New CudaLongDataBuffer(nativeOps_Conflict.getPrimaryShapeInfo(pack), nativeOps_Conflict.getSpecialShapeInfo(pack), nativeOps_Conflict.getShapeInfoLength(pack))
			Dim tadOffsets As val = New CudaLongDataBuffer(nativeOps_Conflict.getPrimaryOffsets(pack), nativeOps_Conflict.getSpecialOffsets(pack), nativeOps_Conflict.getNumberOfTads(pack))

			nativeOps_Conflict.deleteTadPack(pack)

			Return New TadPack(tadShape, tadOffsets)
		End Function

		Public Overrides Function createConstantBuffer(ByVal values() As Long, ByVal desiredType As DataType) As DataBuffer
			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim dbf As val = nativeOps_Conflict.constantBufferLong(desiredType.toInt(), New LongPointer(values), values.Length)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim buffer As val = Nd4j.createBuffer(nativeOps_Conflict.getConstantDataBufferPrimary(dbf), nativeOps_Conflict.getConstantDataBufferSpecial(dbf), values.Length, desiredType)
			buffer.setConstant(True)

			Return buffer
		End Function

		Public Overrides Function createConstantBuffer(ByVal values() As Double, ByVal desiredType As DataType) As DataBuffer
			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim dbf As val = nativeOps_Conflict.constantBufferDouble(desiredType.toInt(), New DoublePointer(values), values.Length)

			If nativeOps_Conflict.lastErrorCode() <> 0 Then
				Throw New Exception(nativeOps_Conflict.lastErrorMessage())
			End If

			Dim buffer As val = Nd4j.createBuffer(nativeOps_Conflict.getConstantDataBufferPrimary(dbf), nativeOps_Conflict.getConstantDataBufferSpecial(dbf), values.Length, desiredType)
			buffer.setConstant(True)

			Return buffer
		End Function


	End Class



End Namespace