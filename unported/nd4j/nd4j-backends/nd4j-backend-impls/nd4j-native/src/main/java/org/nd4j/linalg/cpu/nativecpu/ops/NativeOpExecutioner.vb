Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.bytedeco.javacpp
Imports LongIndexer = org.bytedeco.javacpp.indexer.LongIndexer
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports FlatBuffersMapper = org.nd4j.autodiff.samediff.serde.FlatBuffersMapper
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports org.nd4j.linalg.api.buffer
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
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports BaseRandomOp = org.nd4j.linalg.api.ops.random.BaseRandomOp
Imports Random = org.nd4j.linalg.api.rng.Random
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports TadPack = org.nd4j.linalg.api.shape.TadPack
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = org.nd4j.linalg.api.shape.options.ArrayType
Imports ConstantHandler = org.nd4j.linalg.cache.ConstantHandler
Imports TADManager = org.nd4j.linalg.cache.TADManager
Imports CpuTADManager = org.nd4j.linalg.cpu.nativecpu.CpuTADManager
Imports BaseCpuDataBuffer = org.nd4j.linalg.cpu.nativecpu.buffer.BaseCpuDataBuffer
Imports LongBuffer = org.nd4j.linalg.cpu.nativecpu.buffer.LongBuffer
Imports Utf8Buffer = org.nd4j.linalg.cpu.nativecpu.buffer.Utf8Buffer
Imports CpuNativeRandom = org.nd4j.linalg.cpu.nativecpu.rng.CpuNativeRandom
Imports ND4JIllegalArgumentException = org.nd4j.linalg.exception.ND4JIllegalArgumentException
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports org.nd4j.common.primitives
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
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.linalg.cpu.nativecpu.ops



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NativeOpExecutioner extends org.nd4j.linalg.api.ops.executioner.DefaultOpExecutioner
	Public Class NativeOpExecutioner
		Inherits DefaultOpExecutioner

		Private [loop] As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
		Private constantHandler As ConstantHandler = Nd4j.ConstantHandler
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.cpu.nativecpu.CpuTADManager tadManager = new org.nd4j.linalg.cpu.nativecpu.CpuTADManager();
'JAVA TO VB CONVERTER NOTE: The field tadManager was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private tadManager_Conflict As New CpuTADManager()

		'thread locals for custom op inputs and outputs to prevent allocations
		'every time exec(CustomOp) is called
		Private inputShapes As New ThreadLocal(Of IDictionary(Of Integer, PointerPointer))()
		Private inputBuffers As New ThreadLocal(Of IDictionary(Of Integer, PointerPointer))()
		Private outputShapes As New ThreadLocal(Of IDictionary(Of Integer, PointerPointer))()
		Private outputBuffers As New ThreadLocal(Of IDictionary(Of Integer, PointerPointer))()
		Private iArgsPointer As New ThreadLocal(Of IDictionary(Of Integer, LongPointer))()
		Private tArgsPointer As New ThreadLocal(Of IDictionary(Of Integer, DoublePointer))()
		Private bArgsPointer As New ThreadLocal(Of IDictionary(Of Integer, BooleanPointer))()
		Private halfArgsPointer As New ThreadLocal(Of IDictionary(Of Integer, ShortPointer))()

		Protected Friend customOps As IDictionary(Of String, CustomOpDescriptor) = Nothing

		Protected Friend extraz As New ThreadLocal(Of PointerPointer)()

'JAVA TO VB CONVERTER NOTE: The field experimentalMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend experimentalMode_Conflict As New AtomicBoolean(False)

		Protected Friend mklOverrides As IDictionary(Of String, Boolean) = New Dictionary(Of String, Boolean)()

		''' <summary>
		''' Instead of allocating new memory chunks for each batch invocation, we reuse them on thread/opNum basis
		''' Since for NativeOpExecutioner all executions are synchronous
		''' </summary>
		Private batchPointers As New ThreadLocal(Of IDictionary(Of Integer, Pointer))()
		Private memoryBlocks As New ThreadLocal(Of IDictionary(Of Integer, AggregateMemoryBlock))()

		Public Sub New()
			tadManager_Conflict.init([loop], constantHandler)

			experimentalMode_Conflict.set([loop].isExperimentalEnabled())

			' filling vars for possible overrides
			Dim env As val = Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_MKL_FALLBACK)
			If env IsNot Nothing Then
				' in this case we just disable mkl-dnn globally
				If env.equalsIgnoreCase("true") Then
					Nd4jCpu.Environment.getInstance().setUseMKLDNN(False)
				Else
					Dim split As val = env.toLowerCase().Split(",", True)
					For Each name As val In split
						mklOverrides(name) = New Boolean?(True)
					Next name
				End If
			End If
		End Sub

		Public Overrides Function exec(ByVal op As Op) As INDArray
			Return exec(op, Nothing)
		End Function

		Public Overrides Function exec(ByVal op As Op, ByVal opContext As OpContext) As INDArray
			checkForCompression(op)

			If TypeOf op Is ScalarOp Then
				Dim s As ScalarOp = DirectCast(op, ScalarOp)
				exec(s, opContext)
			ElseIf TypeOf op Is TransformOp Then
				Dim t As TransformOp = DirectCast(op, TransformOp)
				exec(t, opContext)
			ElseIf TypeOf op Is ReduceOp Then
				Dim ac As ReduceOp = DirectCast(op, ReduceOp)
				exec(ac, opContext)
			ElseIf TypeOf op Is IndexAccumulation Then
				Dim iac As IndexAccumulation = DirectCast(op, IndexAccumulation)
				exec(iac, opContext) 'Currently using DefaultOpExecutioner
			ElseIf TypeOf op Is BroadcastOp Then
				Dim broadcastOp As BroadcastOp = DirectCast(op, BroadcastOp)
				exec(broadcastOp, opContext)
			ElseIf TypeOf op Is RandomOp Then
				Dim rngOp As RandomOp = DirectCast(op, RandomOp)
				exec(rngOp, opContext, Nd4j.Random)
			End If

			Return op.z()
		End Function


		Public Overrides Function exec(ByVal op As IndexAccumulation) As INDArray
			Return exec(op, Nothing)
		End Function

		Public Overridable Overloads Function exec(ByVal op As IndexAccumulation, ByVal oc As OpContext) As INDArray
			checkForCompression(op)

			Dim x As INDArray = getX(op, oc)
			Dim z As INDArray = getZ(op, oc)

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim dimension As val = Shape.normalizeAxis(x.rank(), op.dimensions().toIntVector())

			If x.Empty Then
				For Each d As val In dimension
					Preconditions.checkArgument(x.shape()(d) <> 0, "IndexReduce can't be issued along axis with 0 in shape")
				Next d
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

			op.validateDataTypes()

			Dim dimensionAddress As Pointer = constantHandler.getConstantBuffer(dimension, DataType.INT).addressPointer()

			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(x, dimension)

			Dim hostTadShapeInfo As Pointer = tadBuffers.First.addressPointer()

			Dim offsets As DataBuffer = tadBuffers.Second
			Dim hostTadOffsets As Pointer = If(offsets Is Nothing, Nothing, offsets.addressPointer())

			Dim dummy As PointerPointer = extraz.get().put(hostTadShapeInfo, hostTadOffsets)

			Dim st As Long = profilingConfigurableHookIn(op, tadBuffers.First)

			Dim xb As val = DirectCast(x.data(), BaseCpuDataBuffer).OpaqueDataBuffer
			Dim zb As val = DirectCast(z.data(), BaseCpuDataBuffer).OpaqueDataBuffer

			If z.Scalar Then
				[loop].execIndexReduceScalar(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, x.dataType()), zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
			Else
				[loop].execIndexReduce(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, x.dataType()), zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
			End If

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)
			Return getZ(op, oc)
		End Function

		Public Overrides Function exec(ByVal op As Variance) As INDArray
			Return exec(DirectCast(op, ReduceOp))
		End Function

		Public Overrides Function exec(ByVal op As ReduceOp) As INDArray
			Return exec(op, Nothing)
		End Function

		Public Overridable Overloads Function exec(ByVal op As ReduceOp, ByVal oc As OpContext) As INDArray
			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)
			Preconditions.checkNotNull(x, "Op.x() cannot be null: Was null for op %s", op)
			op.validateDataTypes(oc)

			If TypeOf op Is BaseReduceOp AndAlso DirectCast(op, BaseReduceOp).isEmptyReduce() Then
				'Edge case for TF import compatibility: [x,y].reduce(empty) = [x,y]
				'Note that "empty" axis is NOT the same as length 0, as in INDArray.sum(new int[0]), which means "all dimensions"
				If z IsNot Nothing Then
					Preconditions.checkState(x.equalShapes(z), "For empty reductions, result (z) array must have same shape as x shape." & " Got: x=%ndShape, z=%ndShape", x, z)
					z.assign(x)
					Return z
				Else
					setZ(x.dup(), op, oc)
					Return z
				End If
			End If

			' FIXME: this should be moved down to C++ on per-op basis
			Dim dimension As val = Shape.normalizeAxis(x.rank(),If(op.dimensions() IsNot Nothing, op.dimensions().toIntVector(), Nothing))
			' reduce to scalar case, ReduceBool ops require special treatment
			If TypeOf op Is BaseReduceBoolOp AndAlso x.Empty AndAlso (dimension Is Nothing OrElse (dimension.length = 1 AndAlso dimension(0) = Integer.MaxValue)) Then
				If z Is Nothing Then
					setZ(Nd4j.scalar(DirectCast(op, BaseReduceBoolOp).emptyValue()), op, oc)
				Else
					z.assign(DirectCast(op, BaseReduceBoolOp).emptyValue())
				End If

				Return z
			End If

			'validateDataType(Nd4j.dataType(), op);

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim keepDims As Boolean = op.KeepDims
			Dim retShape() As Long = Shape.reductionShape(x, dimension, True, keepDims)


			If x.Vector AndAlso x.length() = ArrayUtil.prod(retShape) AndAlso ArrayUtil.prodLong(retShape) > 1 AndAlso y Is Nothing Then
				Return op.noOp()
			End If

			''' <summary>
			''' This is the result array.
			''' We create it only if we hadn't provided it before
			''' </summary>
			Dim ret As INDArray
			If z Is Nothing OrElse z Is x Then
				If op.ComplexAccumulation Then
					Dim xT As Long = x.tensorsAlongDimension(dimension)
					Dim yT As Long = y.tensorsAlongDimension(dimension)

					ret = Nd4j.create(op.resultType(), New Long(){xT, yT})
				Else
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

					Dim dt As DataType = If(oc IsNot Nothing, op.resultType(oc), op.resultType())
					ret = Nd4j.create(dt, retShape)

				End If
				setZ(ret, op, oc)
				z = ret
			Else
				' compare length
				Dim shapeProduct As Long = (If(retShape.Length = 0, 1, ArrayUtil.prodLong(retShape)))
				If Not op.ComplexAccumulation AndAlso z.length() <> shapeProduct Then
					If Not (x.Empty AndAlso op.KeepDims) Then
						'Empty reductions are special case: [1,0].sum(0,1,keep=true) -> shape [1,1]
						Throw New ND4JIllegalStateException("Shape of target array for reduction [" & java.util.Arrays.toString(z.shape()) & "] doesn't match expected [" & java.util.Arrays.toString(retShape) & "]")
					End If
				ElseIf op.ComplexAccumulation Then
					Dim xT As Long = x.tensorsAlongDimension(dimension)
					Dim yT As Long = y.tensorsAlongDimension(dimension)

					If z.length() <> xT * yT Then
						Throw New ND4JIllegalStateException("Shape of target array for reduction [" & java.util.Arrays.toString(z.shape()) & "] doesn't match expected [" & (xT * yT) & "]")
					End If
				End If

				ret = z
			End If

			'log.info("X dtype: {}; Z dtype: {}", x.dataType(), z.dataType());

			''' <summary>
			''' Returns the <seealso cref="Shape.createShapeInformation(Integer[], Integer[], Integer, Integer, Char)"/>
			''' and the associated offsets for each <seealso cref="INDArray.tensorAlongDimension(Integer, Integer...)"/>
			''' The first item is the shape information. The second one is the offsets.
			''' </summary>
			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = If(x.Empty, Pair.makePair(Of DataBuffer, DataBuffer)(x.data(), Nothing), tadManager_Conflict.getTADOnlyShapeInfo(x, dimension))
			Dim yTadBuffers As Pair(Of DataBuffer, DataBuffer) = Nothing
			''' <summary>
			''' Note that we use addresses in libnd4j.
			''' We use reinterpret cast in c to take the long
			''' we pass to JNI. This manages overhead.
			''' </summary>
			Dim hostTadShapeInfo As Pointer = If(x.Empty, x.shapeInfoDataBuffer().addressPointer(), tadBuffers.First.addressPointer())

			Dim offsets As DataBuffer = If(x.Empty, Nothing, tadBuffers.Second)
			Dim hostTadOffsets As Pointer = If(offsets Is Nothing, Nothing, offsets.addressPointer())

			' we're going to check, if that's TAD vs TAD comparison or TAD vs full array. if later - we're going slightly different route
			Dim tvf As Boolean = False
			If y IsNot Nothing Then
				If x.tensorAlongDimension(0, dimension).length() = y.length() Then
					tvf = True
				End If
			End If

			If op.ComplexAccumulation Then
				yTadBuffers = tadManager_Conflict.getTADOnlyShapeInfo(y, dimension)

				If x.tensorAlongDimension(0, dimension).length() <> y.tensorAlongDimension(0, dimension).length() Then
					Throw New ND4JIllegalStateException("Impossible to issue AllDistances operation: TAD lengths mismatch along given dimension: " & "x TAD length = " & x.tensorAlongDimension(0, dimension).length() & ", y TAD length " & y.tensorAlongDimension(0, dimension).length())
				End If
			End If

			''' <summary>
			''' This is a pointer to a pointer in c.
			''' </summary>
			'  FIXME: we need something better then 3rd element being non-null here...
			'PointerPointer dummy = extraz.get().put(hostTadShapeInfo, hostTadOffsets, tvf ? hostTadOffsets : null);

			Dim st As Long = profilingConfigurableHookIn(op, tadBuffers.First)

			''' <summary>
			''' Note because dimension arrays don't change,
			''' we use an <seealso cref="ConstantHandler"/> which knows how to reserve memory
			''' for immutable buffers for the dimensions.
			''' This gives us a pointer which is passed around in libnd4j.
			''' </summary>
			Dim dimensionAddress As Pointer = constantHandler.getConstantBuffer(dimension, DataType.INT).addressPointer()
			Dim xb As val = DirectCast(x.data(), BaseCpuDataBuffer).OpaqueDataBuffer
			Dim zb As val = DirectCast(z.data(), BaseCpuDataBuffer).OpaqueDataBuffer

			If TypeOf op Is Variance Then
				If ret.Scalar Then
					[loop].execSummaryStatsScalar(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()), zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op, Variance).BiasCorrected)
				Else
					Dim var As Variance = DirectCast(op, Variance)
					Try
						[loop].execSummaryStatsTad(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()), zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, var.BiasCorrected, Nothing, Nothing)
					Catch t As Exception
						Dim str As String = opInfoString(op, [Optional].of(dimension))
						Throw New Exception("Native AccumulationOp execution (double) failed: " & str, t)
					End Try
				End If

			'pairwise reduction like similarity of two arrays
			ElseIf y IsNot Nothing AndAlso op.OpType = Op.Type.REDUCE3 Then
				Dim yb As val = DirectCast(y.data(), BaseCpuDataBuffer).OpaqueDataBuffer
				If op.ComplexAccumulation Then
					Try
						[loop].execReduce3All(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()), yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, CType(tadBuffers.First.addressPointer(), LongPointer), New LongPointerWrapper(tadBuffers.Second.addressPointer()), CType(yTadBuffers.First.addressPointer(), LongPointer), New LongPointerWrapper(yTadBuffers.Second.addressPointer()))
					Catch t As Exception
						Dim str As String = opInfoString(op, [Optional].of(dimension))
						Throw New Exception("Native AccumulationOp execution (double) failed: " & str, t)
					End Try
				ElseIf ret.Scalar Then
					[loop].execReduce3Scalar(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()), yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(ret.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
				Else
					Try
						[loop].execReduce3Tad(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()), yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing, Nothing, Nothing, Nothing)
					Catch t As Exception
						Dim str As String = opInfoString(op, [Optional].of(dimension))
						Throw New Exception("Native AccumulationOp execution (double) failed: " & str, t)
					End Try
				End If

			Else
				If ret.Scalar Then
					Select Case op.OpType
						Case REDUCE_FLOAT
							[loop].execReduceFloat(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()), zb, CType(ret.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
						Case REDUCE_BOOL
							[loop].execReduceBool(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, x.dataType()), zb, CType(ret.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
						Case REDUCE_SAME
							[loop].execReduceSame(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, x.dataType()), zb, CType(ret.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
						Case REDUCE_LONG
							[loop].execReduceLong(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, x.dataType()), zb, CType(ret.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
						Case Else
							Throw New System.NotSupportedException("Unsupported op used in reduce: " & op.OpType)
					End Select
				Else
					Select Case op.OpType
						Case REDUCE_FLOAT
							[loop].execReduceFloat2(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()), zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
						Case REDUCE_LONG
							[loop].execReduceLong2(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, x.dataType()), zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
						Case REDUCE_SAME
							[loop].execReduceSame2(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()), zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
						Case REDUCE_BOOL
							[loop].execReduceBool2(Nothing, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, x.dataType()), zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
						Case Else
							Throw New System.NotSupportedException("Unsupported op used in reduce: " & op.OpType)
					End Select
				End If
			End If

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			Return getZ(op, oc)
		End Function

		''' <summary>
		''' ScalarOp execution </summary>
		''' <param name="op"> Op to execute </param>
		Private Sub invokeScalarAlongDimension(ByVal op As ScalarOp)
			invokeScalarAlongDimension(op, Nothing)
		End Sub

		Private Sub invokeScalarAlongDimension(ByVal op As ScalarOp, ByVal oc As OpContext)
			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

			Dim dimension As val = op.dimensions().toIntVector()
			'dimension = Shape.normalizeAxis(op.x().rank(), dimension);
			' do tad magic
			''' <summary>
			''' Returns the <seealso cref="Shape.createShapeInformation(Integer[], Integer[], Integer, Integer, Char)"/>
			''' and the associated offsets for each <seealso cref="INDArray.tensorAlongDimension(Integer, Integer...)"/>
			''' The first item is the shape information. The second one is the offsets.
			''' </summary>
			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.x(), dimension)

			Dim hostTadShapeInfo As Pointer = tadBuffers.First.addressPointer()
			Dim hostTadOffsets As Pointer = tadBuffers.Second.addressPointer()

			Dim devTadShapeInfoZ As Pointer = Nothing
			Dim devTadOffsetsZ As Pointer = Nothing
			''' <summary>
			''' Returns the <seealso cref="Shape.createShapeInformation(Integer[], Integer[], Integer, Integer, Char)"/>
			''' and the associated offsets for each <seealso cref="INDArray.tensorAlongDimension(Integer, Integer...)"/>
			''' The first item is the shape information. The second one is the offsets.
			''' 
			''' Note that this is the *result* TAD information. An op is always input (x) and output (z)
			''' for result.
			''' This is for assigning the result to of the operation along
			''' the proper dimension.
			''' </summary>
			Dim tadBuffersZ As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.z(), dimension)

			devTadShapeInfoZ = tadBuffersZ.First.addressPointer()
			devTadOffsetsZ = tadBuffersZ.Second.addressPointer()

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim xb As val = DirectCast(x.data(), BaseCpuDataBuffer).OpaqueDataBuffer
			Dim yb As val = DirectCast(y.data(), BaseCpuDataBuffer).OpaqueDataBuffer
			Dim zb As val = DirectCast(z.data(), BaseCpuDataBuffer).OpaqueDataBuffer

			Select Case op.OpType
				Case SCALAR
					[loop].execScalarTad(Nothing, op.opNum(), xb, CType(op.x().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(op.z().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, op.z().dataType()), DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer),Nothing, CType(hostTadShapeInfo, LongPointer), CType(hostTadOffsets, LongPointer), CType(devTadShapeInfoZ, LongPointer), CType(devTadOffsetsZ, LongPointer))
				Case SCALAR_BOOL
					[loop].execScalarBoolTad(Nothing, op.opNum(), xb, CType(op.x().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(op.z().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, yb, CType(op.y().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, op.z().dataType()), DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, CType(hostTadShapeInfo, LongPointer), CType(hostTadOffsets, LongPointer), CType(devTadShapeInfoZ, LongPointer), CType(devTadOffsetsZ, LongPointer))
				Case Else
					Throw New System.NotSupportedException()
			End Select

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If
		End Sub

		Public Overrides Function exec(ByVal op As ScalarOp) As INDArray
			Return exec(op, Nothing)
		End Function

		Public Overridable Overloads Function exec(ByVal op As ScalarOp, ByVal oc As OpContext) As INDArray
			Dim st As Long = profilingConfigurableHookIn(op)

			'validateDataType(Nd4j.dataType(), op);

			If (oc IsNot Nothing AndAlso oc.getOutputArray(0) Is Nothing) OrElse getZ(op, oc) Is Nothing Then
				Select Case op.OpType
					Case SCALAR
						setZ(getX(op, oc).ulike(), op, oc)
	'                    op.setZ(op.x().ulike());
					Case SCALAR_BOOL
	'                    op.setZ(Nd4j.createUninitialized(DataType.BOOL, op.x().shape()));
						setZ(Nd4j.createUninitialized(DataType.BOOL, getX(op, oc).shape()), op, oc)
					Case Else
						Throw New ND4JIllegalStateException("Unknown op type: [" & op.OpType & "]")
				End Select
			End If

	'        if (op.x().length() != op.z().length())
			If getX(op, oc).length() <> getZ(op, oc).length() Then
				Throw New ND4JIllegalStateException("op.X length should be equal to op.Z length: " & "x.length()=" & getX(op, oc).length() & ", z.length()=" & getZ(op, oc).length() & " - x shape info = [" & java.util.Arrays.toString(getX(op, oc).shapeInfoDataBuffer().asInt()) & "], z shape info = [" & java.util.Arrays.toString(getZ(op, oc).shapeInfoDataBuffer().asInt()) & "]")
			End If

			If op.dimensions() IsNot Nothing Then
				invokeScalarAlongDimension(op)
				Return getZ(op, oc)
			End If

			Dim x As val = DirectCast(getX(op, oc).data(), BaseCpuDataBuffer).OpaqueDataBuffer
			Dim scalar As val = DirectCast(op.scalar().data(), BaseCpuDataBuffer).OpaqueDataBuffer
			Dim z As val = DirectCast(getZ(op, oc).data(), BaseCpuDataBuffer).OpaqueDataBuffer


			Select Case op.OpType
				Case SCALAR
					[loop].execScalar(Nothing, op.opNum(), x, CType(getX(op, oc).shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, z, CType(getZ(op, oc).shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, scalar, CType(op.scalar().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, getZ(op, oc).dataType()))
				Case SCALAR_BOOL
					[loop].execScalarBool(Nothing, op.opNum(), x, CType(getX(op, oc).shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, z, CType(getZ(op, oc).shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, scalar, CType(op.scalar().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, getX(op, oc).dataType()))
				Case Else
					Throw New ND4JIllegalStateException("Unknown op type: [" & op.OpType & "]")
			End Select

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)

			Return getZ(op, oc)
		End Function

		Private Function getPointerForExtraArgs(ByVal op As Op, ByVal type As DataType) As Pointer
			If op.extraArgs() IsNot Nothing Then
				Dim eadb As val = op.extraArgsDataBuff(type)
				If eadb IsNot Nothing Then
					Return eadb.addressPointer()
				Else
					Return Nothing
				End If
			End If

			Return Nothing
		End Function

		Private Overloads Sub exec(ByVal op As TransformOp)
			exec(op, Nothing)
		End Sub

		Private Overloads Sub exec(ByVal op As TransformOp, ByVal oc As OpContext)
			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

			Dim st As Long = 0

	'        validateDataType(Nd4j.dataType(), op);

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim dummy As PointerPointer = extraz.get()

			' Pow operations might be special
			If op.opNum() = 31 Then
				If y IsNot Nothing AndAlso y.Scalar Then
	'                op.setY(Nd4j.valueArrayOf(op.x().shape(), op.y().getDouble(0)));
					setY(Nd4j.valueArrayOf(x.shape(), y.getDouble(0)), op, oc)
				End If
			End If

			''' <summary>
			''' This is the <seealso cref="IsMax"/>
			''' operation.
			''' </summary>
			''' <seealso cref= <seealso cref="Op.extraArgs()"/>
			''' for what an extra argument is in an op.
			''' 
			''' The extra argument in the op here is the <seealso cref="IsMax.IsMax(INDArray, Integer...)"/>
			''' dimension to do the ismax along </seealso>
			If op.opName().Equals("ismax", StringComparison.OrdinalIgnoreCase) AndAlso op.extraArgs() IsNot Nothing AndAlso op.extraArgs().Length > 0 Then
				Dim dimension(DirectCast(op.extraArgs()(0), Integer) - 1) As Integer

				For i As Integer = 0 To dimension.Length - 1
					dimension(i) = DirectCast(op.extraArgs()(i + 1), Integer)
				Next i


				''' <summary>
				''' Returns the <seealso cref="Shape.createShapeInformation(Integer[], Integer[], Integer, Integer, Char)"/>
				''' and the associated offsets for each <seealso cref="INDArray.tensorAlongDimension(Integer, Integer...)"/>
				''' The first item is the shape information. The second one is the offsets.
				''' </summary>
				Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.z(), dimension)


				Dim tad As Pointer = tadBuffers.First.addressPointer()

				Dim offsets As DataBuffer = tadBuffers.Second
				Dim off As Pointer = If(offsets Is Nothing, Nothing, offsets.addressPointer())
				dummy.put(0, tad)
				dummy.put(1, off)

				st = profilingConfigurableHookIn(op, tadBuffers.First)
			Else
				st = profilingConfigurableHookIn(op)
			End If

			If y IsNot Nothing Then

				If z Is Nothing Then
					setZ(Nd4j.create(op.resultType(), x.shape()), op, oc)
					z = getZ(op, oc)
				End If


				op.validateDataTypes(oc, experimentalMode_Conflict.get())

				'log.info("X type: {}; Y type: {}; Z type: {}; OpNum: {}", op.x().dataType(), op.y().dataType(), op.z().dataType(), op.opNum());


				Dim xb As val = DirectCast(x.data(), BaseCpuDataBuffer).OpaqueDataBuffer
				Dim yb As val = DirectCast(y.data(), BaseCpuDataBuffer).OpaqueDataBuffer
				Dim zb As val = DirectCast(z.data(), BaseCpuDataBuffer).OpaqueDataBuffer

				Select Case op.OpType
					Case TRANSFORM_ANY, TRANSFORM_FLOAT, TRANSFORM_STRICT, TRANSFORM_SAME
						If Not experimentalMode_Conflict.get() Then
							Preconditions.checkArgument(x.dataType() = y.dataType() OrElse y.dataType() = DataType.BOOL, "Op.X and Op.Y must have the same data type, but got %s vs. %s", x.dataType(), y.dataType())
						End If

						[loop].execPairwiseTransform(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, z.dataType()))
					Case TRANSFORM_BOOL, PAIRWISE_BOOL
						[loop].execPairwiseTransformBool(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, getPointerForExtraArgs(op, x.dataType()))
				End Select
			Else

				If z Is Nothing Then
					setZ(Nd4j.createUninitialized((If(oc IsNot Nothing, op.resultType(oc), op.resultType())), x.shape()), op, oc)
					z = getZ(op, oc)
				End If

				op.validateDataTypes(oc, experimentalMode_Conflict.get())

				Dim xb As val = DirectCast(x.data(), BaseCpuDataBuffer).OpaqueDataBuffer
				Dim zb As val = DirectCast(z.data(), BaseCpuDataBuffer).OpaqueDataBuffer

				Select Case op.OpType
					Case TRANSFORM_FLOAT
						Dim xtraz As val = getPointerForExtraArgs(op, z.dataType())



						[loop].execTransformFloat(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, xtraz)
					Case TRANSFORM_STRICT
						Dim xtraz As val = getPointerForExtraArgs(op, z.dataType())

						[loop].execTransformStrict(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, xtraz)
					Case TRANSFORM_SAME
						Dim xtraz As val = getPointerForExtraArgs(op, z.dataType())

						[loop].execTransformSame(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, xtraz)
					Case TRANSFORM_ANY
						Dim xtraz As val = getPointerForExtraArgs(op, x.dataType())
						Dim opNum As val = op.opNum()

						[loop].execTransformAny(dummy, opNum, xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, xtraz)
					Case TRANSFORM_BOOL
						Dim xtraz As val = getPointerForExtraArgs(op, x.dataType())
						Dim opNum As val = op.opNum()

						[loop].execTransformBool(dummy, opNum, xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, xtraz)
					Case Else
						Throw New System.NotSupportedException("Unknown transform type: [" & op.OpType & "]")
				End Select

			End If

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)
		End Sub

		Public Overrides Function exec(ByVal op As BroadcastOp) As INDArray
			Return exec(op, Nothing)
		End Function

		Public Overridable Overloads Function exec(ByVal op As BroadcastOp, ByVal oc As OpContext) As INDArray
			Dim x As INDArray = getX(op, oc)
			Dim y As INDArray = getY(op, oc)
			Dim z As INDArray = getZ(op, oc)

			Dim st As Long = profilingConfigurableHookIn(op)

			op.validateDataTypes(experimentalMode_Conflict.get())

			Dim dimension As val = op.dimensions().toIntVector()

			''' <summary>
			''' Returns the <seealso cref="Shape.createShapeInformation(Integer[], Integer[], Integer, Integer, Char)"/>
			''' and the associated offsets for each <seealso cref="INDArray.tensorAlongDimension(Integer, Integer...)"/>
			''' The first item is the shape information. The second one is the offsets.
			''' </summary>
			Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(x, dimension)

			Dim hostTadShapeInfo As Pointer = tadBuffers.First.addressPointer()
			Dim hostTadOffsets As Pointer = tadBuffers.Second.addressPointer()

			Dim devTadShapeInfoZ As Pointer = Nothing
			Dim devTadOffsetsZ As Pointer = Nothing

			'        if (!Arrays.equals(x.shape(),z.shape()) || !Arrays.equals(x.stride(),z.stride()) || x.ordering() != z.ordering()) {
			' that's the place where we're going to have second TAD in place
			Dim tadBuffersZ As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(z, dimension)

			devTadShapeInfoZ = tadBuffersZ.First.addressPointer()
			devTadOffsetsZ = tadBuffersZ.Second.addressPointer()
	'        
	'        log.info("Broascast dimension: {}", Arrays.toString(dimension));
	'        log.info("x shape: {}; x TAD: {}; comp TAD: {}", Arrays.toString(x.shapeInfoDataBuffer().asInt()), Arrays.toString(tadBuffers.getFirst().asInt()), Arrays.toString(x.tensorAlongDimension(0, dimension).shapeInfoDataBuffer().asInt()));
	'        log.info("z shape: {}; z TAD: {}", Arrays.toString(z.shapeInfoDataBuffer().asInt()), Arrays.toString(tadBuffersZ.getFirst().asInt()));
	'        log.info("y shape: {}", Arrays.toString(y.shapeInfoDataBuffer().asInt()));
	'        log.info("-------------");
	'        

			If extraz.get() Is Nothing Then
				extraz.set(New PointerPointer(32))
			End If

			Dim dummy As PointerPointer = extraz.get().put(hostTadShapeInfo, hostTadOffsets, devTadShapeInfoZ, devTadOffsetsZ)

			Dim dimensionAddress As Pointer = constantHandler.getConstantBuffer(dimension, DataType.INT).addressPointer()

			Dim xb As val = DirectCast(x.data(), BaseCpuDataBuffer).OpaqueDataBuffer
			Dim yb As val = DirectCast(y.data(), BaseCpuDataBuffer).OpaqueDataBuffer
			Dim zb As val = DirectCast(z.data(), BaseCpuDataBuffer).OpaqueDataBuffer

			Select Case op.OpType
				Case BROADCAST
					[loop].execBroadcast(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
				Case BROADCAST_BOOL
					[loop].execBroadcastBool(dummy, op.opNum(), xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing, DirectCast(op.dimensions().data(), BaseCpuDataBuffer).OpaqueDataBuffer, CType(op.dimensions().shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing)
				Case Else
					Throw New System.NotSupportedException("Unknown operation type: [" & op.OpType & "]")
			End Select

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			Return z
		End Function


		Protected Friend Overridable Function getPointer(Of T As Aggregate)(ByVal batch As Batch(Of T)) As Pointer
			If batchPointers.get() Is Nothing Then
				batchPointers.set(New Dictionary(Of Integer, Pointer)())
			End If

			If Not batchPointers.get().containsKey(batch.opNum()) Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim pointer As val = New IntPointer(batch.getSample().getRequiredBatchMemorySize() / 4)
				batchPointers.get().put(batch.opNum(), pointer)
				Return pointer
			End If

			Return batchPointers.get().get(batch.opNum())
		End Function


		''' <summary>
		''' This method executes previously built batch
		''' </summary>
		''' <param name="batch"> </param>
		Public Overrides Sub exec(Of T As Aggregate)(ByVal batch As Batch(Of T))
			'profilingHookIn(batch);

			Dim pointer As IntPointer = CType(getPointer(batch), IntPointer)

			Dim maxTypes As Integer = 5

			Dim maxIntArrays As Integer = batch.getSample().maxIntArrays()

			Dim maxArraySize As Integer = batch.getSample().maxIntArraySize()


			Dim indexPos As Integer = maxTypes * Batch.getBatchLimit()
			Dim intArraysPos As Integer = indexPos + (batch.getSample().maxIndexArguments() * Batch.getBatchLimit())
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim realPos As Integer = (intArraysPos + (maxIntArrays * maxArraySize * Batch.getBatchLimit())) / (If(Nd4j.dataType() = DataType.DOUBLE, 2, 1))
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim argsPos As Integer = (realPos + ((batch.getSample().maxRealArguments() * Batch.getBatchLimit()))) / (If(Nd4j.dataType() = DataType.DOUBLE, 1, 2))
			Dim shapesPos As Integer = argsPos + (batch.getSample().maxArguments() * Batch.getBatchLimit())
			Dim dataType As DataType = Nothing
			Dim i As Integer = 0
			Do While i < batch.getNumAggregates()
				Dim op As T = batch.getAggregates().get(i)

				If i = 0 Then
					dataType = op.getArguments().get(0).dataType()
				End If

				' put num arguments
				Dim idx As Integer = i * maxTypes
				pointer.put(idx, op.getArguments().size())
				pointer.put(idx + 1, op.getShapes().size())
				pointer.put(idx + 2, op.getIndexingArguments().size())
				pointer.put(idx + 3, op.getRealArguments().size())
				pointer.put(idx + 4, op.getIntArrayArguments().size())


				' putting indexing arguments
				Dim e As Integer = 0
				Do While e < op.getIndexingArguments().size()
					idx = indexPos + i * batch.getSample().maxIndexArguments()
					pointer.put(idx + e, op.getIndexingArguments().get(e))
					e += 1
				Loop

				' putting intArray values
				Dim bsize As Integer = maxIntArrays * maxArraySize
				e = 0
				Do While e < op.getIntArrayArguments().size()
					Dim [step] As Integer = (i * bsize) + (e * maxArraySize)
					If op.getIntArrayArguments().get(e) IsNot Nothing Then
						Dim x As Integer = 0
						Do While x < op.getIntArrayArguments().get(e).length
							idx = intArraysPos + [step] + x
							pointer.put(idx, op.getIntArrayArguments().get(e)(x))
							x += 1
						Loop
					End If
					e += 1
				Loop

				' TODO: variable datatype should be handled here
				' putting real arguments

				Select Case dataType.innerEnumValue
					Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
						Dim fPtr As New FloatPointer(pointer)
						e = 0
						Do While e < op.getRealArguments().size()
							idx = realPos + i * op.maxRealArguments()
							fPtr.put(idx + e, op.getRealArguments().get(e).floatValue())
							e += 1
						Loop
					Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
						Dim dPtr As New DoublePointer(pointer)
						e = 0
						Do While e < op.getRealArguments().size()
							idx = realPos + (i * op.maxRealArguments())
							dPtr.put(idx + e, op.getRealArguments().get(e).doubleValue())
							e += 1
						Loop
					Case Else
						Throw New ND4JIllegalArgumentException("Only FLOAT and DOUBLE datatypes are supported")
				End Select

				If extraz.get() Is Nothing Then
					extraz.set(New PointerPointer(32))
				End If

				' putting arguments pointers

				Dim ptrPtr As New PointerPointer(pointer) 'extraz.get().put(pointer);

				e = 0
				Do While e < op.getArguments().size()
					idx = argsPos + i * batch.getSample().maxArguments()

					If op.getArguments().get(e) IsNot Nothing Then
						ptrPtr.put(idx + e, op.getArguments().get(e).data().addressPointer())
					End If
					e += 1
				Loop


				' putting shape pointers
				e = 0
				Do While e < op.getShapes().size()
					idx = shapesPos + i * batch.getSample().maxShapes()

					If op.getShapes().get(e) IsNot Nothing Then
						ptrPtr.put(idx + e, op.getShapes().get(e).addressPointer())
					End If
					e += 1
				Loop
				i += 1
			Loop

			[loop].execAggregateBatch(Nothing, batch.getNumAggregates(), batch.opNum(), batch.getSample().maxArguments(), batch.getSample().maxShapes(), batch.getSample().maxIntArrays(), batch.getSample().maxIntArraySize(), batch.getSample().maxIndexArguments(), batch.getSample().maxRealArguments(), pointer, FlatBuffersMapper.getDataTypeAsByte(dataType))

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

		End Sub

		''' <summary>
		''' This method takes arbitrary
		''' sized list of <seealso cref="Aggregate"/>,
		''' and packs them into batches
		''' Note here that this is mainly used for random number generation
		''' for <seealso cref="RandomOp"/> and things like <seealso cref="org.nd4j.linalg.api.rng.distribution.Distribution"/> </summary>
		''' <param name="batch"> the list of <seealso cref="Aggregate"/> to
		'''              execute upon </param>
		Public Overrides Sub exec(ByVal batch As IList(Of Aggregate))
			If batch.Count = 0 Then
				Return
			End If

			Dim batches As IList(Of Batch(Of Aggregate)) = Batch.getBatches(batch)
			For Each [single] As Batch(Of Aggregate) In batches
				Me.exec([single])
			Next [single]
		End Sub

		''' <summary>
		''' This method takes arbitrary
		''' sized list of <seealso cref="Aggregate"/>,
		''' and packs them into batches
		''' Note here that this is mainly used for random number generation
		''' for <seealso cref="RandomOp"/> and things like <seealso cref="org.nd4j.linalg.api.rng.distribution.Distribution"/> </summary>
		''' <param name="op"> the list of <seealso cref="Aggregate"/> to
		'''              execute upon </param>
		Public Overrides Sub exec(ByVal op As Aggregate)
			' long st = profilingHookIn(op);

			If memoryBlocks.get() Is Nothing Then
				memoryBlocks.set(New Dictionary(Of Integer, AggregateMemoryBlock)())
			End If

			If memoryBlocks.get().get(op.opNum()) Is Nothing Then
				memoryBlocks.get().put(op.opNum(), New AggregateMemoryBlock(op))
			End If

			Dim block As AggregateMemoryBlock = memoryBlocks.get().get(op.opNum())

			Dim numArguments As Integer = op.getArguments().Count
			Dim numIndexArguments As Integer = op.getIndexingArguments().Count
			Dim numRealArguments As Integer = op.getRealArguments().Count
			Dim numShapes As Integer = op.getShapes().Count
			Dim numIntArrays As Integer = op.getIntArrayArguments().Count

			Dim arguments As PointerPointer = block.getArgumentsPointer() 'new PointerPointer(numArguments);
			Dim pointers As IList(Of IntPointer) = New List(Of IntPointer)()
			Dim intArrays As PointerPointer = block.getArraysPointer() 'new PointerPointer(numIntArrays);
			Dim dataType As val = op.getArguments()(0).dataType()

			For x As Integer = 0 To numArguments - 1
				arguments.put(x,If(op.getArguments()(x) Is Nothing, Nothing, op.getArguments()(x).data().addressPointer()))
			Next x

			Dim shapes As PointerPointer = block.getShapesPointer() 'new PointerPointer(numShapes);

			For x As Integer = 0 To numShapes - 1
				If op.getShapes()(x).dataType() <> DataType.LONG Then
					Throw New Exception("ShapeBuffers should have LONG data opType")
				End If

				shapes.put(x,If(op.getShapes()(x) Is Nothing, Nothing, op.getShapes()(x).addressPointer()))
			Next x

			'int[] indexes = new int[numIndexArguments];
			Dim pointer As IntPointer = block.getIndexingPointer()
			For x As Integer = 0 To numIndexArguments - 1
				pointer.put(x, op.getIndexingArguments()(x))
			Next x

			'IntPointer pointer = new IntPointer(indexes);

			Dim reals(numRealArguments - 1) As Double
			For x As Integer = 0 To numRealArguments - 1
				'reals[x] = op.getRealArguments().get(x).doubleValue();
				Select Case dataType
					Case FLOAT
						CType(block.getRealArgumentsPointer(), FloatPointer).put(x, op.getRealArguments()(x).floatValue())
					Case [DOUBLE]
						CType(block.getRealArgumentsPointer(), DoublePointer).put(x, op.getRealArguments()(x).doubleValue())
					Case Else
						Throw New ND4JIllegalArgumentException("Only FLOAT and DOUBLE datatypes are supported")
				End Select
			Next x

			For x As Integer = 0 To numIntArrays - 1
				Dim intPtr As IntPointer = block.getIntArrays().get(x) 'new IntPointer(op.getIntArrayArguments().get(x));
				intPtr.put(op.getIntArrayArguments()(x), 0, op.getIntArrayArguments()(x).Length)
				intArrays.put(x, intPtr)
				pointers.Add(intPtr)
			Next x

			'INDArray realsBuffer = Nd4j.create(reals);



			[loop].execAggregate(Nothing, op.opNum(), arguments, numArguments, shapes, numShapes, pointer, numIndexArguments, intArrays, numIntArrays, block.getRealArgumentsPointer(), numRealArguments, FlatBuffersMapper.getDataTypeAsByte(dataType))

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If
		End Sub

		''' <summary>
		''' This method return set of key/value and
		''' key/key/value objects,
		''' describing current environment
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property EnvironmentInformation As Properties
			Get
				Dim properties As Properties = MyBase.EnvironmentInformation
				properties.put(Nd4jEnvironment.BACKEND_KEY, "CPU")
				properties.put(Nd4jEnvironment.OMP_THREADS_KEY, [loop].ompGetMaxThreads())
				properties.put(Nd4jEnvironment.BLAS_THREADS_KEY, Nd4j.factory().blas().MaxThreads)
				properties.put(Nd4jEnvironment.BLAS_VENDOR_KEY, (Nd4j.factory().blas()).BlasVendor.ToString())
				properties.put(Nd4jEnvironment.HOST_FREE_MEMORY_KEY, Pointer.maxBytes() - Pointer.totalBytes())
    
				' fill bandwidth information
		'        
		'        Note: Environment information is logged as part of ND4J initialization... but PerformanceTracker required
		'        ND4J init to be completed before it can be initialized. Hence we can get a null PerformanceTracker when
		'        OpExecutioner.printEnvironmentInformation() is called as part of ND4J class initialization - even
		'        though PerformanceTracker.getInstance() refers to a static final field (as it may not yet be initialized)
		'         
				If PerformanceTracker.Instance IsNot Nothing Then
					properties.put(Nd4jEnvironment.MEMORY_BANDWIDTH_KEY, PerformanceTracker.Instance.getCurrentBandwidth())
				End If
    
				Return properties
			End Get
		End Property

		''' <summary>
		''' This method executes specified RandomOp using default RNG available via Nd4j.getRandom()
		''' </summary>
		''' <param name="op"> </param>
		Public Overrides Function exec(ByVal op As RandomOp) As INDArray
			Return exec(op, Nd4j.Random)
		End Function

		''' <summary>
		''' This method executes specific
		''' RandomOp against specified RNG
		''' </summary>
		''' <param name="op"> </param>
		''' <param name="rng"> </param>
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

			If Not (TypeOf rng Is CpuNativeRandom) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.InvalidOperationException("You should use one of NativeRandom classes for NativeOperations execution. Op class: " & op.GetType().FullName)
			End If

			Dim st As Long = profilingConfigurableHookIn(op)

			'validateDataType(Nd4j.dataType(), op);

			Preconditions.checkArgument(z.R, "Op.Z must have one of floating point types")

			Dim xb As val = If(x Is Nothing, Nothing, DirectCast(x.data(), BaseCpuDataBuffer).OpaqueDataBuffer)
			Dim yb As val = If(y Is Nothing, Nothing, DirectCast(y.data(), BaseCpuDataBuffer).OpaqueDataBuffer)
			Dim zb As val = If(z Is Nothing, Nothing, DirectCast(z.data(), BaseCpuDataBuffer).OpaqueDataBuffer)

			If x IsNot Nothing AndAlso y IsNot Nothing AndAlso z IsNot Nothing Then
				Dim dataBuffer As DataBuffer = op.extraArgsDataBuff(z.dataType())
				' triple arg call
				[loop].execRandom3(Nothing, op.opNum(), rng.StatePointer, xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, yb, CType(y.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing,If(dataBuffer IsNot Nothing, dataBuffer.addressPointer(), Nothing))
			ElseIf x IsNot Nothing AndAlso z IsNot Nothing Then
				Dim dataBuffer As DataBuffer = op.extraArgsDataBuff(z.dataType())
				'double arg call
				[loop].execRandom2(Nothing, op.opNum(), rng.StatePointer, xb, CType(x.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing,If(dataBuffer IsNot Nothing, dataBuffer.addressPointer(), Nothing))
			Else
				' single arg call
				[loop].execRandom(Nothing, op.opNum(), rng.StatePointer, zb, CType(z.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, op.extraArgsDataBuff(z.dataType()).addressPointer())
			End If

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			profilingConfigurableHookOut(op, oc, st)

			Return z
		End Function

		Public Overrides ReadOnly Property TADManager As TADManager
			Get
				Return tadManager_Conflict
			End Get
		End Property

		''' <summary>
		''' This class holds memory chunks required for single specific Aggregate op.
		''' Can be used together with ThreadLocal variables
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data private static class AggregateMemoryBlock
		Private Class AggregateMemoryBlock
			Friend intArrays As IList(Of IntPointer) = New List(Of IntPointer)()
			Friend indexingPointer As IntPointer
			Friend realArgumentsPointer As Pointer
			Friend shapesPointer As PointerPointer
			Friend argumentsPointer As PointerPointer
			Friend arraysPointer As PointerPointer

			Friend ReadOnly opNum As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private AggregateMemoryBlock(@NonNull Aggregate op)
			Friend Sub New(ByVal op As Aggregate)

				opNum = op.opNum()

				' creating IntArrays
				Dim i As Integer = 0
				Do While i < op.maxIntArrays()
					intArrays.Add(New IntPointer(op.maxIntArraySize()))
					i += 1
				Loop

				' allocating chunk for IndexingArguments
				indexingPointer = New IntPointer(op.maxIndexArguments())

				' allocating chunk for RealArguments
				realArgumentsPointer = If(Nd4j.dataType() = DataType.DOUBLE, New DoublePointer(op.maxRealArguments()), New FloatPointer(op.maxRealArguments()))

				' allocating chunk for shapesPointer
				shapesPointer = New PointerPointer(op.maxShapes())

				' allocating chunk for argumentsPointer
				argumentsPointer = New PointerPointer(op.maxArguments())

				' chunk for intArrays
				arraysPointer = New PointerPointer(op.maxIntArrays())
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Me Is o Then
					Return True
				End If
				If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
					Return False
				End If

				Dim that As AggregateMemoryBlock = DirectCast(o, AggregateMemoryBlock)

				Return opNum = that.opNum
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return opNum
			End Function
		End Class

		Public Overrides ReadOnly Property CustomOperations As IDictionary(Of String, CustomOpDescriptor)
			Get
				SyncLock Me
					If customOps Is Nothing Then
						Dim list As String = [loop].getAllCustomOps()
            
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


		Private Function getPointerPointerFrom(ByVal map As ThreadLocal(Of IDictionary(Of Integer, PointerPointer)), ByVal numArguments As Integer) As PointerPointer
			If map.get() Is Nothing Then
				Dim store As IDictionary(Of Integer, PointerPointer) = New Dictionary(Of Integer, PointerPointer)()
				store(numArguments) = New PointerPointer(numArguments)
				map.set(store)
				Return map.get().get(numArguments)
			ElseIf map.get().get(numArguments) Is Nothing Then
				Dim pointerPointer As New PointerPointer(numArguments)
				map.get().put(numArguments,pointerPointer)
				Return pointerPointer
			End If

			Return map.get().get(numArguments)
		End Function




		Private Function getShortPointerFrom(ByVal map As ThreadLocal(Of IDictionary(Of Integer, ShortPointer)), ByVal numArguments As Integer) As ShortPointer
			If map.get() Is Nothing Then
				Dim store As IDictionary(Of Integer, ShortPointer) = New Dictionary(Of Integer, ShortPointer)()
				store(numArguments) = New ShortPointer(numArguments)
				map.set(store)
				Return map.get().get(numArguments)
			ElseIf map.get().get(numArguments) Is Nothing Then
				Dim pointerPointer As New ShortPointer(numArguments)
				map.get().put(numArguments,pointerPointer)
				Return pointerPointer
			End If

			Return map.get().get(numArguments)
		End Function


		Private Function getLongPointerFrom(ByVal map As ThreadLocal(Of IDictionary(Of Integer, LongPointer)), ByVal numArguments As Integer) As LongPointer
			If map.get() Is Nothing Then
				Dim store As IDictionary(Of Integer, LongPointer) = New Dictionary(Of Integer, LongPointer)()
				store(numArguments) = New LongPointer(numArguments)
				map.set(store)
				Return map.get().get(numArguments)
			ElseIf map.get().get(numArguments) Is Nothing Then
				Dim pointerPointer As val = New LongPointer(numArguments)
				map.get().put(numArguments,pointerPointer)
				Return pointerPointer
			End If

			Return map.get().get(numArguments)
		End Function

		Private Function getDoublePointerFrom(ByVal map As ThreadLocal(Of IDictionary(Of Integer, DoublePointer)), ByVal numArguments As Integer) As DoublePointer
			If map.get() Is Nothing Then
				Dim store As IDictionary(Of Integer, DoublePointer) = New Dictionary(Of Integer, DoublePointer)()
				store(numArguments) = New DoublePointer(numArguments)
				map.set(store)
				Return map.get().get(numArguments)
			ElseIf map.get().get(numArguments) Is Nothing Then
				Dim pointerPointer As New DoublePointer(numArguments)
				map.get().put(numArguments,pointerPointer)
				Return pointerPointer
			End If

			Return map.get().get(numArguments)
		End Function


		Private Function getBooleanPointerFrom(ByVal map As ThreadLocal(Of IDictionary(Of Integer, BooleanPointer)), ByVal numArguments As Integer) As BooleanPointer
			If map.get() Is Nothing Then
				Dim store As IDictionary(Of Integer, BooleanPointer) = New Dictionary(Of Integer, BooleanPointer)()
				store(numArguments) = New BooleanPointer(numArguments)
				map.set(store)
				Return map.get().get(numArguments)
			ElseIf map.get().get(numArguments) Is Nothing Then
				Dim pointerPointer As val = New BooleanPointer(numArguments)
				map.get().put(numArguments,pointerPointer)
				Return pointerPointer
			End If

			Return map.get().get(numArguments)
		End Function


		Private Function getInputShapes(ByVal numArguments As Integer) As PointerPointer
			Return getPointerPointerFrom(inputShapes,numArguments)
		End Function

		Private Function getInputBuffers(ByVal numArguments As Integer) As PointerPointer
			Return getPointerPointerFrom(inputBuffers,numArguments)

		End Function

		Private Function getOutputShapes(ByVal numArguments As Integer) As PointerPointer
			Return getPointerPointerFrom(outputShapes,numArguments)

		End Function

		Private Function getOutputBuffers(ByVal numArguments As Integer) As PointerPointer
			Return getPointerPointerFrom(outputBuffers,numArguments)

		End Function

		''' <summary>
		''' This method executes given CustomOp
		''' 
		''' PLEASE NOTE: You're responsible for input/output validation </summary>
		''' <param name="op"> Operation to execute </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray[] exec(@NonNull CustomOp op)
		Public Overrides Function exec(ByVal op As CustomOp) As INDArray()

			Dim shapeOverride As Boolean = False
			If op.numOutputArguments() = 0 AndAlso Not op.InplaceCall Then
				Try
					Dim list As val = Me.calculateOutputShape(op)
					If list.isEmpty() Then
						Throw New ND4JIllegalStateException("Op name " & op.opName() & " failed to calculate output datatypes")
					End If

					For Each shape As LongShapeDescriptor In list
						op.addOutputArgument(Nd4j.create(shape, False))
					Next shape

					shapeOverride = True
				Catch e As ND4JIllegalStateException
					Throw e
				Catch e As Exception
					Throw New ND4JIllegalStateException("Op name " & op.opName() & " - no output arrays were provided and calculateOutputShape failed to execute", e)
					'throw new RuntimeException(e);
				End Try
			End If

			Dim name As val = op.opName()
			Try
					Using context As val = buildContext()
        
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
        
					' check if input & output needs update
					For Each [in] As val In op.inputArguments()
						If Not [in].isEmpty() Then
							CType([in].data(), BaseCpuDataBuffer).actualizePointerAndIndexer()
						End If
					Next [in]
        
					For Each [out] As val In op.outputArguments()
						If Not [out].isEmpty() Then
							CType([out].data(), BaseCpuDataBuffer).actualizePointerAndIndexer()
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

			Dim inputBuffers As val = New PointerPointer(Of )(nIn)
			Dim inputShapes As val = New PointerPointer(Of )(nIn)
			Dim inputArgs As val = If(opContext IsNot Nothing, opContext.getInputArrays(), op.inputArguments())
			Dim cnt As Integer= 0
			For Each [in] As val In inputArgs
				If Not [in].isEmpty() Then
					inputBuffers.put(cnt, [in].data().addressPointer())
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


			Dim ptrptr As OpaqueShapeList
			Try
				ptrptr = [loop].calculateOutputShapes2(Nothing, hash, inputBuffers, inputShapes, nIn, tArgs, nTArgs, iArgs, nIArgs, bArgs, nBArgs, dArgs, nDArgs)

				If [loop].lastErrorCode() <> 0 Then
					Dim differentialFunction As DifferentialFunction = DirectCast(op, DifferentialFunction)
					Throw New Exception("Op " & op.opName() & " with name " & differentialFunction.getOwnName() & " failed to execute." & opContext.ToString() & " Here is the error from c++: " & [loop].lastErrorMessage())

				End If
			Catch t As Exception
				Dim sb As New StringBuilder()
				sb.Append("Inputs: [(")
				For i As Integer = 0 To inputArgs.size() - 1
					If i > 0 Then
						sb.Append("), (")
					End If
					sb.Append(Shape.shapeToStringShort(inputArgs.get(i)))
				Next i
				sb.Append(")]")
				If TypeOf op Is DifferentialFunction AndAlso DirectCast(op, DifferentialFunction).getSameDiff() IsNot Nothing Then
					appendSameDiffInfo(sb, DirectCast(op, DifferentialFunction))
				End If

				Dim nOut As Integer = If(opContext IsNot Nothing, opContext.numOutputArguments(), op.numOutputArguments())
				log.error("Failed to calculate output shapes for op {}. Attempted to execute with {} inputs, {} outputs, " & "{} targs, {} iargs, {} bargs and {} dargs. {} - Please see above message (printed out from c++) for a possible cause of error.", op.opName(), nIn, nOut, nTArgs, nIArgs, nBArgs, nDArgs, sb.ToString())
				Throw t
			End Try

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			If ptrptr Is Nothing Then
				Throw New Exception()
			End If

			Dim e As Integer = 0
			Do While e < [loop].getShapeListSize(ptrptr)
				result.add(getShapeFromPointer((New PagedPointer([loop].getShape(ptrptr, e))).asLongPointer()))
				e += 1
			Loop


			[loop].deleteShapeList(ptrptr)

			If log.isTraceEnabled() Then
				Dim arr(result.size() - 1) As String
				For i As Integer = 0 To result.size() - 1
					arr(i) = result.get(i).ToString()
				Next i

				Dim differentialFunction As DifferentialFunction = DirectCast(op, DifferentialFunction)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				log.trace("Calculated output shapes for op  of name {} and type {} - {}",differentialFunction.getOwnName(), op.GetType().FullName, java.util.Arrays.toString(arr))
			End If
			Return result
		End Function


		Public Overrides Sub enableDebugMode(ByVal reallyEnable As Boolean)
			debug_Conflict.set(reallyEnable)
			[loop].enableDebugMode(reallyEnable)
		End Sub

		Public Overrides Sub enableVerboseMode(ByVal reallyEnable As Boolean)
			verbose_Conflict.set(reallyEnable)
			[loop].enableVerboseMode(reallyEnable)
		End Sub


		Public Overrides Sub registerGraph(ByVal id As Long, ByVal graph As Pointer)
			[loop].registerGraph(Nothing, id, graph)

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Map<String, org.nd4j.linalg.api.ndarray.INDArray> executeGraph(long id, @NonNull Map<String, org.nd4j.linalg.api.ndarray.INDArray> map, @NonNull Map<String, Integer> reverseMap)
		Public Overrides Function executeGraph(ByVal id As Long, ByVal map As IDictionary(Of String, INDArray), ByVal reverseMap As IDictionary(Of String, Integer)) As IDictionary(Of String, INDArray)

			Dim ptrBuffers As val = New PointerPointer(map.Count)
			Dim ptrShapes As val = New PointerPointer(map.Count)
			Dim ptrIndices As val = New IntPointer(map.Count)

			Dim cnt As Integer = 0
			Dim keySet As val = New List(Of String)(map.Keys)
			For Each key As val In keySet
				Dim array As val = map(key)

				ptrBuffers.put(cnt, array.data().addressPointer())
				ptrShapes.put(cnt, array.shapeInfoDataBuffer().addressPointer())
				ptrIndices.put(cnt, reverseMap(key))

				cnt += 1
			Next key

			Dim newMap As val = New LinkedHashMap(Of String, INDArray)()

			Dim result As OpaqueVariablesSet = [loop].executeStoredGraph(Nothing, id, ptrBuffers, ptrShapes, ptrIndices, map.Count)

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			Dim status As OpStatus = OpStatus.byNumber([loop].getVariablesSetStatus(result))

			If status <> OpStatus.ND4J_STATUS_OK Then
				Throw New ND4JIllegalStateException("Op execution failed: " & status)
			End If

			Dim e As Integer = 0
			Do While e < [loop].getVariablesSetSize(result)
				Dim var As OpaqueVariable = [loop].getVariable(result, e)
				Dim nodeId As Integer = [loop].getVariableId(var)
				Dim index As Integer = [loop].getVariableIndex(var)
				Dim shapeInfo As LongPointer = [loop].getVariableShape(var)
				Dim buffer As Pointer = [loop].getVariableBuffer(var)

				Dim rank As val = CInt(Math.Truncate(shapeInfo.get(0)))
				Dim jshape As val = New Long(rank * 2 + 3){}
				For i As Integer = 0 To jshape.length - 1
					jshape(i) = shapeInfo.get(i)
				Next i

				Dim shapeOf As val = Shape.shapeOf(jshape)
				Dim stridesOf As val = Shape.stridesOf(jshape)
				Dim order As val = Shape.order(jshape)
				Dim array As val = Nd4j.create(shapeOf, stridesOf, 0, order)

				Dim perfX As val = PerformanceTracker.Instance.helperStartTransaction()

				Pointer.memcpy(array.data().addressPointer(), buffer, Shape.lengthOf(shapeOf) * Nd4j.sizeOfDataType(array.dataType()))

				PerformanceTracker.Instance.helperRegisterTransaction(0, perfX, Shape.lengthOf(shapeOf) * Nd4j.sizeOfDataType(array.dataType()), MemcpyDirection.HOST_TO_HOST)

				'newMap.put(keySet.get(nodeId), array);
				Dim nodeName As String = [loop].getVariableName(var)
				newMap.put(nodeName, array)
				e += 1
			Loop

			[loop].deleteVariablesSet(result)

			Return newMap
		End Function

		Public Overrides Sub forgetGraph(ByVal id As Long)
			[loop].unregisterGraph(Nothing, id)
			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
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
				[loop].setElementThreshold(threshold)
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
				[loop].setTADThreshold(threshold)
			End Set
		End Property

		Public Overrides Function getString(ByVal buffer As DataBuffer, ByVal index As Long) As String
			Preconditions.checkArgument(TypeOf buffer Is Utf8Buffer, "Expected Utf8Buffer")

			Dim addr As val = CType(buffer.indexer(), LongIndexer).get(index)
			Dim ptr As val = New PagedPointer(addr)
			Dim str As val = New Nd4jCpu.utf8string(ptr)
			Return str._buffer().capacity(str._length()).getString()
		End Function

		Public Overrides Function type() As ExecutionerType
			Return ExecutionerType.NATIVE_CPU
		End Function

		Public Overrides ReadOnly Property ExperimentalMode As Boolean
			Get
				Return experimentalMode_Conflict.get()
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void scatterUpdate(org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate.UpdateOp op, @NonNull INDArray array, @NonNull INDArray indices, @NonNull INDArray updates, @NonNull int[] axis)
		Public Overrides Sub scatterUpdate(ByVal op As ScatterUpdate.UpdateOp, ByVal array As INDArray, ByVal indices As INDArray, ByVal updates As INDArray, ByVal axis() As Integer)
			Dim tadX As val = tadManager_Conflict.getTADOnlyShapeInfo(array, axis)
			Dim tadY As val = tadManager_Conflict.getTADOnlyShapeInfo(updates, axis)

			If tadY.getSecond().length() <> indices.length() Then
				Throw New System.InvalidOperationException("Number of updates doesn't match number of indices. Bad dimensions used?")
			End If

			[loop].scatterUpdate(Nothing, op.ordinal(), CInt(indices.length()), array.data().addressPointer(), CType(tadX.getFirst().addressPointer(), LongPointer), CType(tadX.getSecond().addressPointer(), LongPointer), Nothing, Nothing, Nothing, updates.data().addressPointer(), CType(tadY.getFirst().addressPointer(), LongPointer), CType(tadY.getSecond().addressPointer(), LongPointer), Nothing, Nothing, Nothing, indices.data().addressPointer(), CType(indices.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing)

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If
		End Sub

		Public Overrides Function buildContext() As OpContext
			Return New CpuOpContext()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray[] exec(CustomOp op, @NonNull OpContext context)
		Public Overrides Function exec(ByVal op As CustomOp, ByVal context As OpContext) As INDArray()
			Dim st As Long = profilingConfigurableHookIn(op, context)
			Dim mklOverride As Boolean = False
			Try
				If Nd4jCpu.Environment.getInstance().isUseMKLDNN() Then
					Dim opName As val = op.opName()
					Dim state As val = mklOverrides(op)
					If state IsNot Nothing AndAlso state = True Then
						mklOverride = True
						Nd4jCpu.Environment.getInstance().setUseMKLDNN(True)
					End If
				End If


				Dim status As val = [loop].execCustomOp2(Nothing, op.opHash(), context.contextPointer())

				If [loop].lastErrorCode() <> 0 Then
					Throw New Exception([loop].lastErrorMessage())
				End If

				If status <> 0 Then
					Dim differentialFunction As DifferentialFunction = DirectCast(op, DifferentialFunction)
					Throw New Exception("Op with name " & differentialFunction.getOwnName() & " and op type [" & op.opName() & "] execution failed")
				End If
				If context.getOutputArrays().Count = 0 Then
					Return New INDArray(){}
				Else
					Return CType(context.getOutputArrays(), List(Of INDArray)).ToArray()
				End If
			Catch e As Exception
				Dim sb As val = New StringBuilder()
				sb.append("Inputs: [(")
				Dim nIn As Integer = (If(context.getInputArrays() Is Nothing, 0, context.getInputArrays().Count))
				For i As Integer = 0 To nIn - 1
					If i > 0 Then
						sb.append("), (")
					End If
					sb.append(Shape.shapeToStringShort(context.getInputArrays()(i)))
				Next i
				sb.append(")]. Outputs: [(")
				Dim nOut As Integer = (If(context.getOutputArrays() Is Nothing, 0, context.getOutputArrays().Count))
				For i As Integer = 0 To nOut - 1
					If i > 0 Then
						sb.append("), (")
					End If
					sb.append(Shape.shapeToStringShort(context.getOutputArrays()(i)))
				Next i
				sb.append(")]. tArgs: ")
				Dim nT As Integer = (If(context.getTArguments() Is Nothing, 0, context.getTArguments().Count))
				If nT > 0 Then
					sb.append(context.getTArguments())
				Else
					sb.append("-")
				End If
				sb.append(". iArgs: ")
				Dim nI As Integer = (If(context.getIArguments() Is Nothing, 0, context.getIArguments().Count))
				If nI > 0 Then
					sb.append(context.getIArguments())
				Else
					sb.append("-")
				End If
				sb.append(". bArgs: ")
				Dim nB As Integer = (If(context.getBArguments() Is Nothing, 0, context.getBArguments().Count))
				If nB > 0 Then
					sb.append(context.getBArguments())
				Else
					sb.append("-")
				End If
				If TypeOf op Is DifferentialFunction Then
					Dim n As String = DirectCast(op, DifferentialFunction).getOwnName()
					If n IsNot Nothing AndAlso Not n.Equals(op.opName()) Then
						sb.append(". Op own name: """).append(n).append("""")
					End If
				End If

				If TypeOf op Is DifferentialFunction AndAlso DirectCast(op, DifferentialFunction).getSameDiff() IsNot Nothing Then
					appendSameDiffInfo(sb, DirectCast(op, DifferentialFunction))
				End If

				log.error("Failed to execute op " & op.opName() & ". Attempted to execute with " & nIn & " inputs, " & nOut & " outputs, " & nT & " targs," & nB & " bargs and " & nI & " iargs. " & sb.ToString() & " - Please see above message (printed out from c++) for a possible cause of error.")
				Throw e
			Finally
				If mklOverride Then
					Nd4jCpu.Environment.getInstance().setUseMKLDNN(True)
				End If
				profilingConfigurableHookOut(op, context, st)
			End Try
		End Function

		Public Overrides Function inspectArray(ByVal array As INDArray) As INDArrayStatistics
			Dim debugInfo As val = New Nd4jCpu.DebugInfo()

			[loop].inspectArray(Nothing, array.data().addressPointer(), CType(array.shapeInfoDataBuffer().addressPointer(), LongPointer), Nothing, Nothing, debugInfo)

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			Return INDArrayStatistics.builder().minValue(debugInfo._minValue()).maxValue(debugInfo._maxValue()).meanValue(debugInfo._meanValue()).stdDevValue(debugInfo._stdDevValue()).countInf(debugInfo._infCount()).countNaN(debugInfo._nanCount()).countNegative(debugInfo._negativeCount()).countPositive(debugInfo._positiveCount()).countZero(debugInfo._zeroCount()).build()
		End Function

		Public Overrides Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal empty As Boolean) As DataBuffer
			Dim dbf As val = [loop].shapeBuffer(shape.Length, New LongPointer(shape), New LongPointer(stride), dtype.toInt(), order, elementWiseStride, empty)
			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			Dim result As val = New LongBuffer([loop].getConstantShapeBufferPrimary(dbf), Shape.shapeInfoLength(shape.Length))

			[loop].deleteConstantShapeBuffer(dbf)

			Return result
		End Function

		Public Overrides Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal extras As Long) As DataBuffer
			Dim dbf As OpaqueConstantShapeBuffer = [loop].shapeBufferEx(shape.Length, New LongPointer(shape), New LongPointer(stride), dtype.toInt(), order, elementWiseStride, extras)
			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			Dim result As val = New LongBuffer([loop].getConstantShapeBufferPrimary(dbf), Shape.shapeInfoLength(shape.Length))

			[loop].deleteConstantShapeBuffer(dbf)

			Return result
		End Function

		Public Overrides Function tadShapeInfoAndOffsets(ByVal array As INDArray, ByVal dimension() As Integer) As TadPack
			Dim pack As OpaqueTadPack = [loop].tadOnlyShapeInfo(CType(array.shapeInfoDataBuffer().addressPointer(), LongPointer), New IntPointer(dimension), dimension.Length)

			If [loop].lastErrorCode() <> 0 Then
				Throw New Exception([loop].lastErrorMessage())
			End If

			Dim tadShape As val = New LongBuffer([loop].getPrimaryShapeInfo(pack), [loop].getShapeInfoLength(pack))
			Dim tadOffsets As val = New LongBuffer([loop].getPrimaryOffsets(pack), [loop].getNumberOfTads(pack))

			[loop].deleteTadPack(pack)

			Return New TadPack(tadShape, tadOffsets)
		End Function

		Protected Friend Overridable Sub appendSameDiffInfo(ByVal sb As StringBuilder, ByVal df As DifferentialFunction)
			Dim inNames() As String = df.argNames()
			Dim outNames() As String = df.outputVariablesNames()
			If inNames IsNot Nothing Then
				sb.Append(". Input var names: ").Append(java.util.Arrays.toString(inNames))
			End If
			If outNames IsNot Nothing Then
				sb.Append(". Output var names: ").Append(java.util.Arrays.toString(outNames))
			End If
		End Sub



	End Class

End Namespace