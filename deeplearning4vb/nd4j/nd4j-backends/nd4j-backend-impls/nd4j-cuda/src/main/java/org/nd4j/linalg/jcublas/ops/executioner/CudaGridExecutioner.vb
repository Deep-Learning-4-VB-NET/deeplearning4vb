Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Microsoft.VisualBasic
Imports System.Linq
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports org.nd4j.common.primitives
Imports org.bytedeco.javacpp
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.api.ops
Imports Aggregate = org.nd4j.linalg.api.ops.aggregates.Aggregate
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports GridPointers = org.nd4j.linalg.api.ops.grid.GridPointers
Imports OpDescriptor = org.nd4j.linalg.api.ops.grid.OpDescriptor
Imports InvertedPredicateMetaOp = org.nd4j.linalg.api.ops.impl.meta.InvertedPredicateMetaOp
Imports PostulateMetaOp = org.nd4j.linalg.api.ops.impl.meta.PostulateMetaOp
Imports PredicateMetaOp = org.nd4j.linalg.api.ops.impl.meta.PredicateMetaOp
Imports ScalarMax = org.nd4j.linalg.api.ops.impl.scalar.ScalarMax
Imports ScalarMin = org.nd4j.linalg.api.ops.impl.scalar.ScalarMin
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports AggregateDescriptor = org.nd4j.linalg.jcublas.ops.executioner.aggregates.AggregateDescriptor
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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
	''' mGRID implementation for CUDA
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	<Obsolete>
	Public Class CudaGridExecutioner
		Inherits CudaExecutioner
		Implements GridExecutioner

		Protected Friend Enum MetaType
			NOT_APPLICABLE
			PREDICATE
			INVERTED_PREDICATE
			POSTULATE
		End Enum

		' general queues
		'private List<Deque<OpDescriptor>> deviceQueues = new ArrayList<>();

		' last op
		Private Shadows lastOp_Conflict As New ThreadLocal(Of OpDescriptor)()
		'    private ThreadLocal<PointerPointer> extraz = new ThreadLocal<>();
		Private deviceQueues As New ThreadLocal(Of LinkedList(Of OpDescriptor))()

		Private opCounter As New ThreadLocal(Of AtomicLong)()

'JAVA TO VB CONVERTER NOTE: The field metaCounter was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private metaCounter_Conflict As New AtomicLong(0)
		Private execCounter As New AtomicLong(0)

		Private watchdog As IList(Of WatchdogPair) = New CopyOnWriteArrayList(Of WatchdogPair)()

		Private aggregates As IList(Of LinkedList(Of AggregateDescriptor)) = New List(Of LinkedList(Of AggregateDescriptor))()

		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(CudaGridExecutioner))

		Private experimental As New AtomicBoolean(False)

		Public Sub New()
			'        extraz.set(new PointerPointer(10));
			deviceQueues.set(New LinkedList(Of OpDescriptor)())

			Dim numDevices As Integer = nativeOps_Conflict.getAvailableDevices()

			For x As Integer = 0 To numDevices - 1
				aggregates.Add(New ConcurrentLinkedQueue(Of AggregateDescriptor)())
			Next x

			experimental.set(nativeOps_Conflict.isExperimentalEnabled())
		End Sub

		''' <summary>
		''' This is one of the main entry points for ops that are executed without respect to dimension.
		''' 
		''' Developers note: For CudaGridExecutioner that's also the MetaOp/GridOp creation point.
		''' </summary>
		''' <param name="op">
		''' @return </param>
		Public Overrides Function exec(ByVal op As Op) As INDArray
	'        
	'            We pass this op to GridProcessor through check for possible MetaOp concatenation
	'            Also, it's the GriOp entry point
	'         
			checkForCompression(op)

			invokeWatchdog(op)

			If TypeOf op Is ReduceOp Then
				exec(DirectCast(op, ReduceOp), New Integer() {Integer.MaxValue})
			ElseIf TypeOf op Is IndexAccumulation Then
				exec(DirectCast(op, IndexAccumulation), New Integer() {Integer.MaxValue})
			ElseIf TypeOf op Is ScalarOp OrElse TypeOf op Is TransformOp Then
				' the only entry place for TADless ops
				processAsGridOp(op)
			ElseIf TypeOf op Is BroadcastOp Then
				invoke(DirectCast(op, BroadcastOp), Nothing)
			Else
				'logger.info("Random op: {}", op.getClass().getSimpleName());
				pushToGrid(New OpDescriptor(op))
			End If

			Return op.z()
		End Function


		Protected Friend Overridable Sub pushToGrid(ByVal descriptor As OpDescriptor)
			pushToGrid(descriptor, True)
		End Sub


		Protected Friend Overridable Sub invokeWatchdog(ByVal op As Op)

			If watchdog.Count > 0 Then
				For Each pair As WatchdogPair In watchdog
					If compareArrays(pair.getArray(), op) Then
						'    logger.info("WATCHDOG: Invoked {} op on {} using JVM eq", op.getClass().getSimpleName(), pair.getTag());
						Continue For
					End If

					If compareDevicePointers(pair.getArray(), op) Then
						'  logger.info("WATCHDOG: Invoked {} op on {} using device PTR; Thread ID: {}; deviceId: {}", op.getClass().getSimpleName(), pair.getTag(), Thread.currentThread().getId(), Nd4j.getAffinityManager().getDeviceForCurrentThread());
						Throw New Exception()
					End If

					If compareHostPointers(pair.getArray(), op) Then
						'    logger.info("WATCHDOG: Invoked {} op on {} using host PTR", op.getClass().getSimpleName(), pair.getTag());
						Continue For
					End If
				Next pair
			End If
		End Sub

		Protected Friend Overridable Function compareDevicePointers(ByVal array As INDArray, ByVal op As Op) As Boolean
			Dim context As val = CType(AtomicAllocator.Instance.DeviceContext, CudaContext)

			Dim pointer As val = AtomicAllocator.Instance.getPointer(array, context)

			Dim opZ As Long = AtomicAllocator.Instance.getPointer(op.z(), context).address()
			Dim opX As Long = AtomicAllocator.Instance.getPointer(op.x(), context).address()

			Dim opY As Long = If(op.y() Is Nothing, 0, AtomicAllocator.Instance.getPointer(op.y(), context).address())

			If opZ = pointer.address() Then
				'logger.error("op.Z matched: {}", pointer.address());
				Return True
			End If

			If opY = pointer.address() Then
				'logger.error("op.Y matched: {}", pointer.address());
				Return True
			End If

			If opX = pointer.address() Then
				'logger.error("op.X matched: {}", pointer.address());
				Return True
			End If

			Return False
		End Function


		Protected Friend Overridable Function compareHostPointers(ByVal array As INDArray, ByVal op As Op) As Boolean
			Dim context As val = CType(AtomicAllocator.Instance.DeviceContext, CudaContext)

			Dim pointer As Pointer = AtomicAllocator.Instance.getPointer(array, context)

			Dim opZ As Long = AtomicAllocator.Instance.getHostPointer(op.z()).address()
			Dim opX As Long = AtomicAllocator.Instance.getHostPointer(op.x()).address()

			Dim opY As Long = If(op.y() Is Nothing, 0, AtomicAllocator.Instance.getHostPointer(op.y()).address())

			If opZ = pointer.address() OrElse opY = pointer.address() OrElse opX = pointer.address() Then
				Return True
			End If

			Return False
		End Function

		Protected Friend Overridable Function compareArrays(ByVal array As INDArray, ByVal op As Op) As Boolean
			If op.x() Is array OrElse op.y() Is array OrElse op.z() Is array Then
				Return True
			End If

			Return False
		End Function

		''' <summary>
		''' This method adds op into GridOp queue
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable Sub pushToGrid(ByVal descriptor As OpDescriptor, ByVal flush As Boolean)

			' we should just add op to queue here
			'deviceQueues.get().add(descriptor);

			' FIXME: following code should be removed, since it's just executing supers instead of batching

			execCounter.incrementAndGet()

			Dim op As Op = descriptor.getOp()
			Dim dimensions() As Integer = descriptor.getDimensions()

			If TypeOf op Is TransformOp Then
				Dim t As TransformOp = DirectCast(op, TransformOp)
				If flush Then
					flushQueue()
				End If

				'logger.info("Sending TransformOp to CudaExecutioner");
				MyBase.invoke(t, Nothing)
			ElseIf TypeOf op Is Variance Then
				Dim acc As Variance = DirectCast(op, Variance)
				If flush Then
					flushQueue()
				End If

				MyBase.naiveExec(acc, dimensions)
			ElseIf TypeOf op Is ReduceOp Then
				Dim acc As ReduceOp = DirectCast(op, ReduceOp)
				If flush Then
					flushQueue()
				End If

				'logger.info("Sending AccumulationOp to CudaExecutioner: {}", Arrays.toString(dimensions));
				MyBase.naiveExec(acc, dimensions)
			ElseIf TypeOf op Is ScalarOp Then
				Dim sc As ScalarOp = DirectCast(op, ScalarOp)
				If flush Then
					flushQueue()
				End If

				'logger.info("Sending ScalarOp to CudaExecutioner");
				MyBase.invoke(sc, Nothing)
			ElseIf TypeOf op Is BroadcastOp Then
				Dim broadcastOp As BroadcastOp = DirectCast(op, BroadcastOp)
				If flush Then
					flushQueue()
				End If

				'logger.info("Sending BroadcastOp to CudaExecutioner");
				If dimensions IsNot Nothing Then
					MyBase.exec(broadcastOp)
				Else
					MyBase.invoke(broadcastOp, Nothing)
				End If
			ElseIf TypeOf op Is IndexAccumulation Then
				Dim indexAccumulation As IndexAccumulation = DirectCast(op, IndexAccumulation)
				If flush Then
					flushQueue()
				End If

				'logger.info("Sending IndexAccumulationOp to CudaExecutioner");
				'super.exec(indexAccumulation, dimensions);
			ElseIf TypeOf op Is MetaOp Then
				'     logger.info("Executing MetaOp");
				metaCounter_Conflict.incrementAndGet()
				exec(DirectCast(op, MetaOp))
			ElseIf TypeOf op Is GridOp Then
				'    logger.info("Executing GridOp");
				exec(DirectCast(op, GridOp))
			End If
		End Sub



		Public Overridable ReadOnly Property MetaCounter As Long
			Get
				Return metaCounter_Conflict.get()
			End Get
		End Property

		Public Overridable ReadOnly Property ExecutionCounter As Long
			Get
				Return execCounter.get()
			End Get
		End Property

		Protected Friend Overridable Sub processAsGridOp(ByVal op As Op, ParamArray ByVal dimension() As Integer)
	'        
	'            We have multiple options here:
	'                1) Op has no relation to lastOp
	'                2) Op has SOME relation to lastOp
	'                3) Op is supposed to blocking
	'        
	'            So we either should append this op to future GridOp, form MetaOp, or immediately execute everything
	'            But we don't expect this method called for blocking ops ever, so it's either
	'        
			' CudaContext context = AtomicAllocator.getInstance().getFlowController().prepareAction(op.z(), op.x(), op.y());


			Dim last As OpDescriptor = lastOp_Conflict.get()
			If last IsNot Nothing Then
				Dim type As MetaType = getMetaOpType(op, dimension)
				lastOp_Conflict.remove()
				Try
					Select Case type
						Case org.nd4j.linalg.jcublas.ops.executioner.CudaGridExecutioner.MetaType.NOT_APPLICABLE
	'                    
	'                        If we can't form MetaOp with new Op here, we should move lastOp to GridOp queue, and update lastOp with current Op
	'                    
							dequeueOp(last)
							pushToGrid(last, False)

							'|| op instanceof ScalarOp
							If (TypeOf op Is TransformOp AndAlso op.y() IsNot Nothing) AndAlso onCurrentDeviceXYZ(op) Then
								enqueueOp(New OpDescriptor(op, dimension))
							Else
								pushToGrid(New OpDescriptor(op, dimension), False)
							End If
						Case org.nd4j.linalg.jcublas.ops.executioner.CudaGridExecutioner.MetaType.PREDICATE
							Dim metaOp As MetaOp = New PredicateMetaOp(last, New OpDescriptor(op, dimension))
							pushToGrid(New OpDescriptor(metaOp), False)
						Case org.nd4j.linalg.jcublas.ops.executioner.CudaGridExecutioner.MetaType.INVERTED_PREDICATE
							Dim currentOp As New OpDescriptor(op, dimension)

							'          logger.info("Calling for Meta: {}+{}", last.getOp().getClass().getSimpleName(), currentOp.getOp().getClass().getSimpleName());
							dequeueOp(last)
							dequeueOp(currentOp)

							Dim metaOp As MetaOp = New InvertedPredicateMetaOp(last, currentOp)
							pushToGrid(New OpDescriptor(metaOp), False)
						Case org.nd4j.linalg.jcublas.ops.executioner.CudaGridExecutioner.MetaType.POSTULATE
							Dim metaOp As MetaOp = New PostulateMetaOp(last, New OpDescriptor(op, dimension))
							pushToGrid(New OpDescriptor(metaOp), False)
						Case Else
							Throw New System.NotSupportedException("Not supported MetaType: [" & type & "]")
					End Select
				Catch t As Exception
					'Try to provide a more useful exception. Because of the async nature of grid execution, sometimes the
					' exception doesn't make sense - i.e., the exception stack trace points to Nd4j.create() but the exception
					' was actually caused by some problem at an earlier line.
					'In practice, we should be catching these sorts of problems earlier with input validation checks
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New Exception("Error executing previous op: " & last.getOp().GetType().FullName & " - note that in some cases the error/" & "stack trace may be delayed by 1 operation due to the asynchronous nature of ND4J's CUDA grid executioner." & vbLf & "To obtain the original error stack trace for debugging purposes, use nd4j-native backend, Nd4j.getExecutioner().commit() calls after ops, " & "or set the following system property: set ""opexec"" to org.nd4j.linalg.jcublas.ops.executioner.CudaExecutioner", t)
				End Try
			Else
				'&& Nd4j.dataType() != DataType.HALF
				If (TypeOf op Is TransformOp AndAlso op.y() IsNot Nothing AndAlso onCurrentDeviceXYZ(op)) Then
					enqueueOp(New OpDescriptor(op, dimension))
				Else
					pushToGrid(New OpDescriptor(op, dimension), False)
				End If
			End If

			'   AtomicAllocator.getInstance().getFlowController().registerAction(context, op.z(), op.x(), op.y());

			'return op;
		End Sub

		Protected Friend Overridable Function onCurrentDeviceXYZ(ByVal op As Op) As Boolean
			Dim deviceId As Integer = AtomicAllocator.Instance.getDeviceId()
			Dim deviceX As Integer = AtomicAllocator.Instance.getDeviceId(op.x())
			Dim deviceY As Integer = AtomicAllocator.Instance.getDeviceId(op.y())
			Dim deviceZ As Integer = AtomicAllocator.Instance.getDeviceId(op.y())

			Return deviceId = deviceX AndAlso deviceY = deviceZ AndAlso deviceZ = deviceX
		End Function

		Protected Friend Overridable Sub enqueueOp(ByVal descriptor As OpDescriptor)
			AtomicAllocator.Instance.getAllocationPoint(descriptor.getOp().x()).markEnqueued(True)
			AtomicAllocator.Instance.getAllocationPoint(descriptor.getOp().z()).markEnqueued(True)

			If descriptor.getOp().y() IsNot Nothing Then
				AtomicAllocator.Instance.getAllocationPoint(descriptor.getOp().y()).markEnqueued(True)
			End If

			'   logger.info("Enqueued op: " + descriptor.getOp().getClass().getSimpleName());

			lastOp_Conflict.set(descriptor)
		End Sub

		Protected Friend Overridable Sub dequeueOp(ByVal descriptor As OpDescriptor)

			AtomicAllocator.Instance.getAllocationPoint(descriptor.getOp().x()).markEnqueued(False)
			AtomicAllocator.Instance.getAllocationPoint(descriptor.getOp().z()).markEnqueued(False)

			If descriptor.getOp().y() IsNot Nothing Then
				AtomicAllocator.Instance.getAllocationPoint(descriptor.getOp().y()).markEnqueued(False)
			End If

			'   logger.info("Dequeued op: " + descriptor.getOp().getClass().getSimpleName());
		End Sub

		Protected Friend Overridable Function getMetaOpType(ByVal op As Op, ParamArray ByVal dimension() As Integer) As MetaType

			'if (1 > 0) return MetaType.NOT_APPLICABLE;

			Dim last As OpDescriptor = lastOp_Conflict.get()

			If last Is Nothing Then
				Return MetaType.NOT_APPLICABLE
			Else
				' Experimental native compilation required for full MIMD support
				If experimental.get() Then
					logger.info("Experimental hook")
					If TypeOf last.getOp() Is ScalarOp OrElse TypeOf last.getOp() Is TransformOp Then
	'                    
	'                    Predicate logic is simple:
	'                        1) LastOp is one of following op types: Scalar, Transform, PairwiseTransform
	'                        2) LastOp isn't specialOp
	'                        3) LastOp op.x() == op.z()
	'                        4) currentOp op.x() == op.z(), and matches lastOp op.z()
	'                    

						Return If(isMatchingZX(last.getOp(), op), MetaType.PREDICATE, MetaType.NOT_APPLICABLE)
					ElseIf TypeOf last.getOp() Is ReduceOp Then
	'                    
	'                    InvertedMetaOp, aka Postulate logic
	'                    
	'                    Postulate logic is simple too:
	'                        1) LastOp is opType of Reduce or Reduce3
	'                        2) LastOp op.z() isn't scalar
	'                        3) currentOp is one of the following op types: Scalar, Transform
	'                     
						If (TypeOf op Is ScalarOp OrElse TypeOf op Is TransformOp) AndAlso op.y() Is Nothing Then
							Return If(isMatchingZX(last.getOp(), op), MetaType.POSTULATE, MetaType.NOT_APPLICABLE)
						End If
					End If
				Else
					' TODO: extend non-experimental support for MetaOps
					' we enable this only for PairwisetTransforms.Set followed by scalar
					If TypeOf last.getOp() Is TransformOp AndAlso last.getOp().y() IsNot Nothing Then
						' FIXME: get rid of those instanceof
						If TypeOf op Is ScalarOp AndAlso DirectCast(op, ScalarOp).Dimension Is Nothing AndAlso Not (TypeOf op Is ScalarMax) AndAlso Not (TypeOf op Is ScalarMin) AndAlso Not (op.opNum() >= 7 AndAlso op.opNum() <= 11) AndAlso op.opNum() <> 16 AndAlso op.opNum() <> 13 AndAlso Not (op.opNum() >= 56 AndAlso op.opNum() <= 59) Then
							Return If(isMatchingZX(last.getOp(), op), MetaType.INVERTED_PREDICATE, MetaType.NOT_APPLICABLE)
						End If
					End If
				End If
			End If

			Return MetaType.NOT_APPLICABLE
		End Function

		''' <summary>
		''' This method checks, if opA and opB are sharing the same operands
		''' </summary>
		''' <param name="opA"> </param>
		''' <param name="opB">
		''' @return </param>
		Protected Friend Overridable Function isMatchingZX(ByVal opA As Op, ByVal opB As Op) As Boolean
			If opA.x() Is opB.x() AndAlso opA.z() Is opB.z() AndAlso opA.x() Is opB.z() Then
				Return True
			End If

			Return False
		End Function

		''' <summary>
		''' This method is additional check, basically it qualifies possibility of InvertedPredicate MetaOp
		''' </summary>
		''' <param name="opA"> </param>
		''' <param name="opB">
		''' @return </param>
		Protected Friend Overridable Function isMatchingZXY(ByVal opA As Op, ByVal opB As Op) As Boolean
			If opA.z() Is opB.x() OrElse opA.z() Is opB.y() Then
				Return True
			End If

			Return False
		End Function

		Protected Friend Overridable Function pointerizeOp(ByVal descriptor As OpDescriptor) As GridPointers
			Return pointerizeOp(descriptor.getOp(), descriptor.getDimensions())
		End Function

		''' <summary>
		''' This method returns Op as set of required pointers for it </summary>
		''' <param name="op"> </param>
		''' <param name="dimensions">
		''' @return </param>
		Protected Friend Overridable Function pointerizeOp(ByVal op As Op, ParamArray ByVal dimensions() As Integer) As GridPointers
			Dim pointers As New GridPointers(op, dimensions)

			Dim allocator As AtomicAllocator = AtomicAllocator.Instance

			Dim context As val = allocator.DeviceContext

			pointers.setX(allocator.getPointer(op.x(), context))
			pointers.setXShapeInfo(allocator.getPointer(op.x().shapeInfoDataBuffer(), context))
			pointers.setZ(allocator.getPointer(op.z(), context))
			pointers.setZShapeInfo(allocator.getPointer(op.z().shapeInfoDataBuffer(), context))
			pointers.setZLength(op.z().length())

			If op.y() IsNot Nothing Then
				pointers.setY(allocator.getPointer(op.y(), context))
				pointers.setYShapeInfo(allocator.getPointer(op.y().shapeInfoDataBuffer(), context))
			End If

			If dimensions IsNot Nothing AndAlso dimensions.Length > 0 Then
				Dim dimensionBuffer As DataBuffer = Nd4j.ConstantHandler.getConstantBuffer(dimensions, DataType.INT)
				pointers.setDimensions(allocator.getPointer(dimensionBuffer, context))
				pointers.setDimensionsLength(dimensions.Length)
			End If


			' we build TADs
			If dimensions IsNot Nothing AndAlso dimensions.Length > 0 Then
				Dim tadBuffers As Pair(Of DataBuffer, DataBuffer) = tadManager_Conflict.getTADOnlyShapeInfo(op.x(), dimensions)

				Dim devTadShapeInfo As Pointer = AtomicAllocator.Instance.getPointer(tadBuffers.First, context)
				Dim devTadOffsets As Pointer = If(tadBuffers.Second Is Nothing, Nothing, AtomicAllocator.Instance.getPointer(tadBuffers.Second, context))

				' we don't really care, if tadOffsets will be nulls
				pointers.setTadShape(devTadShapeInfo)
				pointers.setTadOffsets(devTadOffsets)
			End If


			Return pointers
		End Function

		''' <summary>
		''' This method returns Op queue lengths for current device
		''' 
		''' PLEASE NOTE: This value also includes variative lastOp
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property QueueLength As Integer Implements GridExecutioner.getQueueLength
			Get
				'return deviceQueues.get().size() + (lastOp.get() == null ? 0 : 1);
				Return (If(lastOp_Conflict.get() Is Nothing, 0, 1))
			End Get
		End Property

		''' <summary>
		''' This method returns Op queue length for specified device
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
		<Obsolete>
		Protected Friend Overridable Function getQueueLength(ByVal deviceId As Integer) As Integer
			Return -1
		End Function

		''' <summary>
		''' This method bundless all ops available in queue into single GridOp
		''' @return
		''' </summary>
		Protected Friend Overridable Function buildGrid() As GridOp
			Return Nothing
		End Function

		Protected Friend Overridable Sub buildZ(ByVal op As IndexAccumulation, ParamArray ByVal dimension() As Integer)
			Array.Sort(dimension)

			For i As Integer = 0 To dimension.Length - 1
				If dimension(i) < 0 Then
					dimension(i) += op.x().rank()
				End If
			Next i

			'do op along all dimensions
			If dimension.Length = op.x().rank() Then
				dimension = New Integer() {Integer.MaxValue}
			End If


			Dim retShape() As Long = If(Shape.wholeArrayDimension(dimension), New Long() {1, 1}, ArrayUtil.removeIndex(op.x().shape(), dimension))
			'ensure vector is proper shape
			If retShape.Length = 1 Then
				If dimension(0) = 0 Then
					retShape = New Long() {1, retShape(0)}
				Else
					retShape = New Long() {retShape(0), 1}
				End If
			ElseIf retShape.Length = 0 Then
				retShape = New Long() {1, 1}
			End If

			If op.z() Is Nothing OrElse op.z() Is op.x() Then
				Dim ret As INDArray = Nothing
				ret = Nd4j.createUninitialized(retShape)


				op.Z = ret
			ElseIf Not retShape.SequenceEqual(op.z().shape()) Then
				Throw New System.InvalidOperationException("Z array shape does not match expected return type for op " & op & ": expected shape " & java.util.Arrays.toString(retShape) & ", z.shape()=" & java.util.Arrays.toString(op.z().shape()))
			End If
		End Sub

		Protected Friend Overridable Sub buildZ(ByVal op As ReduceOp, ParamArray ByVal dimension() As Integer)
			Array.Sort(dimension)

			For i As Integer = 0 To dimension.Length - 1
				If dimension(i) < 0 Then
					dimension(i) += op.x().rank()
				End If
			Next i

			'do op along all dimensions
			If dimension.Length = op.x().rank() Then
				dimension = New Integer() {Integer.MaxValue}
			End If


			Dim retShape() As Long = If(Shape.wholeArrayDimension(dimension), New Long() {1, 1}, ArrayUtil.removeIndex(op.x().shape(), dimension))
			'ensure vector is proper shape
			If retShape.Length = 1 Then
				If dimension(0) = 0 Then
					retShape = New Long() {1, retShape(0)}
				Else
					retShape = New Long() {retShape(0), 1}
				End If
			ElseIf retShape.Length = 0 Then
				retShape = New Long() {1, 1}
			End If

	'        
	'        if(op.x().isVector() && op.x().length() == ArrayUtil.prod(retShape))
	'            return op.noOp();
	'        

			Dim ret As INDArray = Nothing
			If op.z() Is Nothing OrElse op.z() Is op.x() Then
				If op.ComplexAccumulation Then
					Dim xT As val = op.x().tensorsAlongDimension(dimension)
					Dim yT As val = op.y().tensorsAlongDimension(dimension)

					ret = Nd4j.create(xT, yT)
				Else
						ret = Nd4j.zeros(retShape)
				End If

				op.Z = ret
			Else
				' compare length
				If op.z().length() <> ArrayUtil.prodLong(retShape) Then
					Throw New ND4JIllegalStateException("Shape of target array for reduction [" & java.util.Arrays.toString(op.z().shape()) & "] doesn't match expected [" & java.util.Arrays.toString(retShape) & "]")
				End If

				ret = op.z()
			End If
		End Sub

		Public Overridable Overloads Function exec(ByVal op As ReduceOp, ParamArray ByVal dimension() As Integer) As INDArray


			' we should check, if this op returns scalar or not
			' if op.Z is scalar, we can't use GridOp here
			If dimension Is Nothing OrElse dimension.Length = 0 OrElse dimension(0) = Integer.MaxValue Then
				' So, that's scalar. We'll have to flush queue
				' processAsGridOp(op, dimension);
				flushQueue()

				'super.exec(op, new int[] {Integer.MAX_VALUE});
			Else
				buildZ(op, dimension)
				processAsGridOp(op, dimension)
			End If

			Return op.z()
		End Function


		Public Overridable Overloads Function exec(ByVal op As IndexAccumulation, ParamArray ByVal dimension() As Integer) As INDArray
			'        buildZ(op, dimension);

			If dimension Is Nothing OrElse dimension.Length = 0 OrElse dimension(0) = Integer.MaxValue Then
				' So, that's scalar. We'll have to flush queue
				flushQueue()

				buildZ(op, New Integer() {Integer.MaxValue})
				MyBase.invoke(op, Nothing, New Integer() {Integer.MaxValue})
			Else
				buildZ(op, dimension)
				processAsGridOp(op, dimension)
			End If

			Return op.z()
		End Function


		Public Overridable Overloads Function exec(ByVal op As BroadcastOp, ParamArray ByVal dimension() As Integer) As INDArray
			processAsGridOp(op, dimension)

			Return op.z()
		End Function

		' FIXME: remove CudaContext return opType. We just don't need it
		Protected Friend Overrides Function invoke(ByVal op As BroadcastOp, ByVal oc As OpContext) As CudaContext
			Preconditions.checkState(oc Is Nothing)
			processAsGridOp(op, op.Dimension)

			Return Nothing
		End Function

		' FIXME: remove CudaContext return opType. We just don't need it
		Protected Friend Overrides Function invoke(ByVal op As ScalarOp, ByVal oc As OpContext) As CudaContext
			Preconditions.checkState(oc Is Nothing)
			processAsGridOp(op, Nothing)

			Return Nothing
		End Function

		' FIXME: remove CudaContext return opType. We just don't need it
		Protected Friend Overrides Function invoke(ByVal op As TransformOp, ByVal oc As OpContext) As CudaContext
			Preconditions.checkState(oc Is Nothing)
			processAsGridOp(op, Nothing)
			Return Nothing
		End Function

		Protected Friend Overridable Sub prepareGrid(ByVal op As MetaOp)
			Dim ptrA As GridPointers = pointerizeOp(op.FirstOpDescriptor)
			Dim ptrB As GridPointers = pointerizeOp(op.SecondOpDescriptor)



			op.FirstPointers = ptrA
			op.SecondPointers = ptrB
		End Sub

		Public Overrides Sub exec(ByVal op As MetaOp)
			'
		End Sub

	'    
	'    @Override
	'    public void exec(MetaOp op) {
	'        if (extraz.get() == null)
	'            extraz.set(new PointerPointer(32));
	'
	'        prepareGrid(op);
	'
	'        GridPointers first = op.getGridDescriptor().getGridPointers().get(0);
	'        GridPointers second = op.getGridDescriptor().getGridPointers().get(1);
	'
	'        // we need to use it only for first op, since for MetaOps second op shares the same X & Z by definition
	'        CudaContext context =
	'                AtomicAllocator.getInstance().getFlowController().prepareAction(first.getOpZ(), first.getOpY());
	'
	'        //        AtomicAllocator.getInstance().getFlowController().prepareAction(second.getOpX(), second.getOpY(), second.getOpZ());
	'
	'
	'        //CudaContext context = (CudaContext) AtomicAllocator.getInstance().getDeviceContext().getContext();
	'
	'
	'        PointerPointer extras = extraz.get().put(null, context.getOldStream());
	'
	'        double scalarA = 0.0;
	'        double scalarB = 0.0;
	'
	'        if (op.getFirstOp() instanceof ScalarOp)
	'            scalarA = ((ScalarOp) op.getFirstOp()).scalar().doubleValue();
	'
	'        if (op.getSecondOp() instanceof ScalarOp)
	'            scalarB = ((ScalarOp) op.getSecondOp()).scalar().doubleValue();
	'
	'
	'
	'        //logger.info("FirstOp: {}, SecondOp: {}", op.getFirstOp().getClass().getSimpleName(), op.getSecondOp().getClass().getSimpleName());
	'
	'        // FIXME: this is bad hack, reconsider this one
	'        GridPointers yGrid = first;
	'
	'        if (op.getSecondOp().y() != null) {
	'            yGrid = second;
	'        }
	'
	'
	'        if (op instanceof PredicateMetaOp || op instanceof InvertedPredicateMetaOp) {
	'            if (first.getDtype() == DataType.FLOAT) {
	'                if (yGrid.getYOrder() == yGrid.getXOrder() && yGrid.getXStride() >= 1 && yGrid.getYStride() >= 1) {
	'                    nativeOps.execMetaPredicateStridedFloat(extras, first.getType().ordinal(), first.getOpNum(),
	'                            second.getType().ordinal(), second.getOpNum(), first.getXLength(),
	'                            (FloatPointer) first.getX(), first.getXStride(), (FloatPointer) yGrid.getY(), // can be null
	'                            yGrid.getYStride(), // cane be -1
	'                            (FloatPointer) second.getZ(), second.getZStride(),
	'                            (FloatPointer) first.getExtraArgs(), (FloatPointer) second.getExtraArgs(),
	'                            (float) scalarA, (float) scalarB);
	'                } else {
	'                    nativeOps.execMetaPredicateShapeFloat(extras, first.getType().ordinal(), first.getOpNum(),
	'                            second.getType().ordinal(), second.getOpNum(), first.getXLength(),
	'                            (FloatPointer) first.getX(), (LongPointer) first.getXShapeInfo(),
	'                            (FloatPointer) yGrid.getY(), // can be null
	'                            (LongPointer) yGrid.getYShapeInfo(), // cane be -1
	'                            (FloatPointer) second.getZ(), (LongPointer) second.getZShapeInfo(),
	'                            (FloatPointer) first.getExtraArgs(), (FloatPointer) second.getExtraArgs(),
	'                            (float) scalarA, (float) scalarB);
	'                }
	'            } else if (first.getDtype() == DataType.DOUBLE) {
	'                if (yGrid.getYOrder() == yGrid.getXOrder() && yGrid.getXStride() >= 1 && yGrid.getYStride() >= 1) {
	'                    nativeOps.execMetaPredicateStridedDouble(extras, first.getType().ordinal(), first.getOpNum(),
	'                            second.getType().ordinal(), second.getOpNum(), first.getXLength(),
	'                            (DoublePointer) first.getX(), first.getXStride(), (DoublePointer) yGrid.getY(), // can be null
	'                            yGrid.getYStride(), // cane be -1
	'                            (DoublePointer) second.getZ(), second.getZStride(),
	'                            (DoublePointer) first.getExtraArgs(), (DoublePointer) second.getExtraArgs(),
	'                            scalarA, scalarB);
	'                } else {
	'                    nativeOps.execMetaPredicateShapeDouble(extras, first.getType().ordinal(), first.getOpNum(),
	'                            second.getType().ordinal(), second.getOpNum(), first.getXLength(),
	'                            (DoublePointer) first.getX(), (LongPointer) first.getXShapeInfo(),
	'                            (DoublePointer) yGrid.getY(), // can be null
	'                            (LongPointer) yGrid.getYShapeInfo(), // cane be -1
	'                            (DoublePointer) second.getZ(), (LongPointer) second.getZShapeInfo(),
	'                            (DoublePointer) first.getExtraArgs(), (DoublePointer) second.getExtraArgs(),
	'                            scalarA, scalarB);
	'                }
	'            } else {
	'                if (yGrid.getYOrder() == yGrid.getXOrder() && yGrid.getXStride() >= 1 && yGrid.getYStride() >= 1) {
	'                    nativeOps.execMetaPredicateStridedHalf(extras, first.getType().ordinal(), first.getOpNum(),
	'                            second.getType().ordinal(), second.getOpNum(), first.getXLength(),
	'                            (ShortPointer) first.getX(), first.getXStride(), (ShortPointer) yGrid.getY(), // can be null
	'                            yGrid.getYStride(), // cane be -1
	'                            (ShortPointer) second.getZ(), second.getZStride(),
	'                            (ShortPointer) first.getExtraArgs(), (ShortPointer) second.getExtraArgs(),
	'                            (float) scalarA, (float) scalarB);
	'                } else {
	'                    nativeOps.execMetaPredicateShapeHalf(extras, first.getType().ordinal(), first.getOpNum(),
	'                            second.getType().ordinal(), second.getOpNum(), first.getXLength(),
	'                            (ShortPointer) first.getX(), (LongPointer) first.getXShapeInfo(),
	'                            (ShortPointer) yGrid.getY(), // can be null
	'                            (LongPointer) yGrid.getYShapeInfo(), // cane be -1
	'                            (ShortPointer) second.getZ(), (LongPointer) second.getZShapeInfo(),
	'                            (ShortPointer) first.getExtraArgs(), (ShortPointer) second.getExtraArgs(),
	'                            (float) scalarA, (float) scalarB);
	'                }
	'            }
	'        } else if (op instanceof ReduceMetaOp) {
	'            if (first.getDtype() == DataType.FLOAT) {
	'
	'                nativeOps.execMetaPredicateReduceFloat(extras, first.getType().ordinal(), first.getOpNum(),
	'                        second.getType().ordinal(), second.getOpNum(), (FloatPointer) first.getX(),
	'                        (LongPointer) first.getXShapeInfo(), (FloatPointer) second.getY(),
	'                        (LongPointer) second.getYShapeInfo(), (FloatPointer) second.getZ(),
	'                        (LongPointer) second.getZShapeInfo(), (IntPointer) second.getDimensions(),
	'                        second.getDimensionsLength(), (LongPointer) second.getTadShape(),
	'                        new LongPointerWrapper(second.getTadOffsets()), (FloatPointer) first.getExtraArgs(),
	'                        (FloatPointer) second.getExtraArgs(), (float) scalarA, 0.0f, false);
	'            }
	'        }
	'
	'        AtomicAllocator.getInstance().getFlowController().registerAction(context, first.getOpZ(), first.getOpY());
	'        //        AtomicAllocator.getInstance().getFlowController().registerAction(context, second.getOpX(), second.getOpY(), second.getOpZ());
	'    }
	'    

		Public Overrides Sub exec(ByVal op As GridOp)
			' TODO: to be implemented
		End Sub

		Protected Friend Overridable Sub purgeQueue()
			lastOp_Conflict.remove()
		End Sub

		''' <summary>
		''' This method forces all currently enqueued ops to be executed immediately
		''' 
		''' PLEASE NOTE: This call IS non-blocking
		''' </summary>
		Public Overridable Sub flushQueue() Implements GridExecutioner.flushQueue
	'        
	'            Basically we just want to form GridOp and pass it to native executioner
	'            But since we don't have GridOp interface yet, we'll send everything to underlying CudaExecutioner.
	'         
			'    logger.info("Non-Blocking flush");
			' TODO: proper implementation for GridOp creation required here
	'        
	'        Deque<OpDescriptor> currentQueue = deviceQueues.get();
	'        if (currentQueue == null)
	'            return;
	'        
	'        OpDescriptor op = currentQueue.pollFirst();
	'        while (op != null) {
	'            pushToGrid(op, false);
	'        
	'            op = currentQueue.pollFirst();
	'        }
	'        

			' we need to check,
			Dim op As OpDescriptor = lastOp_Conflict.get()
			If op IsNot Nothing Then
				If Not experimental.get() Then
					'if (!nativeOps.isExperimentalEnabled()) {
					' it might be only pairwise transform here for now
					'          logger.info("Flushing existing lastOp");
					lastOp_Conflict.remove()
					dequeueOp(op)
					pushToGrid(op, False)
				Else
					Throw New System.NotSupportedException("Experimental flush isn't supported yet")
				End If
			Else
				'      logger.info("Queue is empty");

			End If
		End Sub

		''' <summary>
		''' This method forces all currently enqueued ops to be executed immediately
		''' 
		''' PLEASE NOTE: This call is always blocking, until all queued operations are finished
		''' </summary>
		Public Overridable Sub flushQueueBlocking() Implements GridExecutioner.flushQueueBlocking
			flushQueue()

			Dim context As val = AtomicAllocator.Instance.DeviceContext

			context.syncSpecialStream()
			context.syncOldStream()
		End Sub

		Public Overridable Sub addToWatchdog(ByVal array As INDArray, ByVal tag As String)
			watchdog.Add(New WatchdogPair(array, tag))
		End Sub


		''' <summary>
		''' This method executes specified RandomOp using default RNG available via Nd4j.getRandom()
		''' </summary>
		''' <param name="op"> </param>
		Public Overrides Function exec(ByVal op As RandomOp) As INDArray
			Return exec(op, Nd4j.Random)
		End Function


		Public Overridable Overloads Sub exec(ByVal batch As IList(Of Aggregate))
			flushQueue()

			MyBase.exec(batch)
		End Sub

		Public Overrides Sub exec(ByVal op As Aggregate)
			flushQueue()

			MyBase.exec(op)
		End Sub

		''' <summary>
		''' This method enqueues aggregate op for future invocation with respect to thread and op order
		''' This method uses current thread Id as aggregation key.
		''' </summary>
		''' <param name="op"> </param>
		Public Overridable Sub aggregate(ByVal op As Aggregate) Implements GridExecutioner.aggregate
			aggregate(op, Thread.CurrentThread.getId())
		End Sub

		''' <summary>
		''' This method enqueues aggregate op for future invocation.
		''' Key value will be used to batch individual ops
		''' </summary>
		''' <param name="op"> </param>
		''' <param name="key"> </param>
		Public Overridable Sub aggregate(ByVal op As Aggregate, ByVal key As Long) Implements GridExecutioner.aggregate
			Dim deviceId As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()
			If opCounter.get() Is Nothing Then
				opCounter.set(New AtomicLong(0))
			End If

			' we enqueue op for specific device here
			aggregates(deviceId).AddLast(New AggregateDescriptor(op, key, opCounter.get().getAndIncrement()))
		End Sub

		Public Overrides Function exec(ByVal op As RandomOp, ByVal rng As Random) As INDArray
			flushQueue()

			Return MyBase.exec(op, rng)
		End Function

		Protected Friend Overridable Sub buildAggregation()

		End Sub

	'    
	'    @Override
	'    public INDArray execAndReturn(BroadcastOp op) {
	'        flushQueue();
	'        execCounter.incrementAndGet();
	'    
	'        return super.execAndReturn(op);
	'    }
	'    
	'    @Override
	'    public INDArray execAndReturn(Op op) {
	'        flushQueue();
	'        execCounter.incrementAndGet();
	'    
	'        return super.execAndReturn(op);
	'    }
	'    
	'    @Override
	'    public INDArray execAndReturn(ScalarOp op) {
	'        flushQueue();
	'        execCounter.incrementAndGet();
	'    
	'        super.invoke(op);
	'        return op.z();
	'    }
	'    
	'    @Override
	'    public INDArray execAndReturn(TransformOp op) {
	'        flushQueue();
	'        execCounter.incrementAndGet();
	'    
	'        super.invoke(op);
	'    
	'        return op.z();
	'    }
	'    



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor private static class WatchdogPair
		Private Class WatchdogPair
			Friend array As INDArray
			Friend tag As String
		End Class


		Public Overrides Sub push()
			flushQueue()
		End Sub

		Public Overrides Sub commit()
			flushQueueBlocking()
		End Sub
	End Class

End Namespace