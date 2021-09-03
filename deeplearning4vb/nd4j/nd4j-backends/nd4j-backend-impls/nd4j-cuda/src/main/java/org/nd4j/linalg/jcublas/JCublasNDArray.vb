Imports System
Imports System.Collections.Generic
Imports System.IO
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports FlatArray = org.nd4j.graph.FlatArray
Imports CudaConstants = org.nd4j.jita.allocator.enums.CudaConstants
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports BaseNDArray = org.nd4j.linalg.api.ndarray.BaseNDArray
Imports BaseNDArrayProxy = org.nd4j.linalg.api.ndarray.BaseNDArrayProxy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JvmShapeInfo = org.nd4j.linalg.api.ndarray.JvmShapeInfo
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CudaLongDataBuffer = org.nd4j.linalg.jcublas.buffer.CudaLongDataBuffer
Imports CudaUtf8Buffer = org.nd4j.linalg.jcublas.buffer.CudaUtf8Buffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports WorkspaceUtils = org.nd4j.linalg.workspace.WorkspaceUtils
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

Namespace org.nd4j.linalg.jcublas



	''' 
	''' 
	''' <summary>
	''' Created by mjk on 8/23/14.
	''' 
	''' @author mjk
	''' @author Adam Gibson
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JCublasNDArray extends org.nd4j.linalg.api.ndarray.BaseNDArray
	<Serializable>
	Public Class JCublasNDArray
		Inherits BaseNDArray


		Public Sub New(ByVal buffer As DataBuffer, ByVal shapeInfo As CudaLongDataBuffer, ByVal javaShapeInfo() As Long)
			Me.jvmShapeInfo = New JvmShapeInfo(javaShapeInfo)
			Me.shapeInformation_Conflict = shapeInfo
			Me.data_Conflict = buffer
		End Sub

		Public Sub New(ByVal data()() As Double)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data()() As Double, ByVal ordering As Char)
			MyBase.New(data, ordering)
		End Sub

		Public Sub New(ByVal shape() As Integer, ByVal buffer As DataBuffer)
			MyBase.New(shape, buffer)
		End Sub

		''' <summary>
		''' Create this JCublasNDArray with the given data and shape and 0 offset
		''' </summary>
		''' <param name="data">     the data to use </param>
		''' <param name="shape">    the shape of the JCublasNDArray </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(data, shape, ordering)
		End Sub

		''' <param name="data">     the data to use </param>
		''' <param name="shape">    the shape of the JCublasNDArray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the JCublasNDArray </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, offset, ordering)
		End Sub

		''' <summary>
		''' Construct an JCublasNDArray of the specified shape
		''' with an empty data array
		''' </summary>
		''' <param name="shape">    the shape of the JCublasNDArray </param>
		''' <param name="stride">   the stride of the JCublasNDArray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the JCublasNDArray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(shape, stride, offset, ordering)
		End Sub

		''' <summary>
		''' Construct an JCublasNDArray of the specified shape, with optional initialization
		''' </summary>
		''' <param name="shape">    the shape of the JCublasNDArray </param>
		''' <param name="stride">   the stride of the JCublasNDArray </param>
		''' <param name="offset">   the desired offset </param>
		''' <param name="ordering"> the ordering of the JCublasNDArray </param>
		''' <param name="initialize"> Whether to initialize the INDArray. If true: initialize. If false: don't. </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean)
			MyBase.New(shape, stride, offset, ordering, initialize)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal initialize As Boolean)
			MyBase.New(shape, stride, offset, ordering, initialize)
		End Sub

		''' <summary>
		''' Create the JCublasNDArray with
		''' the specified shape and stride and an offset of 0
		''' </summary>
		''' <param name="shape">    the shape of the JCublasNDArray </param>
		''' <param name="stride">   the stride of the JCublasNDArray </param>
		''' <param name="ordering"> the ordering of the JCublasNDArray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)

			MyBase.New(shape, stride, ordering)

		End Sub

		Public Sub New(ByVal shape() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(shape, offset, ordering)
		End Sub

		Public Sub New(ByVal shape() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(shape, offset, ordering)
		End Sub

		Public Sub New(ByVal shape() As Integer)
			MyBase.New(shape)
		End Sub

		Public Sub New(ByVal shape() As Long)
			MyBase.New(shape)
		End Sub

		''' <summary>
		''' Creates a new <i>n</i> times <i>m</i> <tt>DoubleMatrix</tt>.
		''' </summary>
		''' <param name="newRows">    the number of rows (<i>n</i>) of the new matrix. </param>
		''' <param name="newColumns"> the number of columns (<i>m</i>) of the new matrix. </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal newRows As Integer, ByVal newColumns As Integer, ByVal ordering As Char)
			MyBase.New(newRows, newColumns, ordering)

		End Sub

		''' <summary>
		''' Create an JCublasNDArray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one JCublasNDArray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices">   the slices to merge </param>
		''' <param name="shape">    the shape of the JCublasNDArray </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(slices, shape, ordering)
		End Sub

		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long, ByVal ordering As Char)
			MyBase.New(slices, shape, ordering)
		End Sub

		''' <summary>
		''' Create an JCublasNDArray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one JCublasNDArray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices">   the slices to merge </param>
		''' <param name="shape">    the shape of the JCublasNDArray </param>
		''' <param name="stride"> </param>
		''' <param name="ordering"> </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)
			MyBase.New(slices, shape, stride, ordering)

		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal ordering As Char)
			MyBase.New(data, shape, stride, ordering)

		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal shape() As Integer, ByVal strides() As Integer)
			MyBase.New(data, shape, strides)
		End Sub
		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Integer)
			MyBase.New(data, shape)
		End Sub

		Public Sub New(ByVal data As DataBuffer, ByVal shape() As Long)
			MyBase.New(data, shape)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long)
			MyBase.New(buffer, shape, offset)
		End Sub

		''' <summary>
		''' Create this JCublasNDArray with the given data and shape and 0 offset
		''' </summary>
		''' <param name="data">  the data to use </param>
		''' <param name="shape"> the shape of the JCublasNDArray </param>
		Public Sub New(ByVal data() As Single, ByVal shape() As Integer)
			MyBase.New(data, shape)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal offset As Long)

			MyBase.New(data, shape, offset)

		End Sub

		''' <summary>
		''' Construct an JCublasNDArray of the specified shape
		''' with an empty data array
		''' </summary>
		''' <param name="shape">  the shape of the JCublasNDArray </param>
		''' <param name="stride"> the stride of the JCublasNDArray </param>
		''' <param name="offset"> the desired offset </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)

			MyBase.New(shape, stride, offset)
		End Sub

		''' <summary>
		''' Create the JCublasNDArray with
		''' the specified shape and stride and an offset of 0
		''' </summary>
		''' <param name="shape">  the shape of the JCublasNDArray </param>
		''' <param name="stride"> the stride of the JCublasNDArray </param>
		Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer)
			MyBase.New(shape, stride)
		End Sub

		Public Sub New(ByVal shape() As Integer, ByVal offset As Long)
			MyBase.New(shape, offset)
		End Sub

		Public Sub New(ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(shape, ordering)
		End Sub

		''' <summary>
		''' Creates a new <i>n</i> times <i>m</i> <tt>DoubleMatrix</tt>.
		''' </summary>
		''' <param name="newRows">    the number of rows (<i>n</i>) of the new matrix. </param>
		''' <param name="newColumns"> the number of columns (<i>m</i>) of the new matrix. </param>
		Public Sub New(ByVal newRows As Integer, ByVal newColumns As Integer)
			MyBase.New(newRows, newColumns)
		End Sub

		''' <summary>
		''' Create an JCublasNDArray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one JCublasNDArray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices"> the slices to merge </param>
		''' <param name="shape">  the shape of the JCublasNDArray </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer)
			MyBase.New(slices, shape)
		End Sub

		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Long)
			MyBase.New(slices, shape)
		End Sub

		''' <summary>
		''' Create an JCublasNDArray from the specified slices.
		''' This will go through and merge all of the
		''' data from each slice in to one JCublasNDArray
		''' which will then take the specified shape
		''' </summary>
		''' <param name="slices"> the slices to merge </param>
		''' <param name="shape">  the shape of the JCublasNDArray </param>
		''' <param name="stride"> </param>
		Public Sub New(ByVal slices As IList(Of INDArray), ByVal shape() As Integer, ByVal stride() As Integer)
			MyBase.New(slices, shape, stride)

		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer)
			MyBase.New(data, shape, stride)
		End Sub


		Public Sub New(ByVal data() As Single, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			MyBase.New(data, shape, stride, offset)
		End Sub

		Public Sub New(ByVal data() As Single)
			MyBase.New(data)
		End Sub


		Public Sub New(ByVal doubleMatrix As JCublasNDArray)
			Me.New(New Long() {doubleMatrix.rows(), doubleMatrix.columns()})
			Me.data_Conflict = dup().data()
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long)
			MyBase.New(data, shape, stride, offset)
		End Sub

		Public Sub New(ByVal floats()() As Single)
			MyBase.New(floats)
		End Sub

		Public Sub New(ByVal data()() As Single, ByVal ordering As Char)
			MyBase.New(data, ordering)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(buffer, shape, offset, ordering)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal buffer As DataBuffer)
			MyBase.New(buffer)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(buffer, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ordering As Char, ByVal dataType As DataType)
			MyBase.New(buffer, shape, stride, offset, ordering, dataType)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal ordering As Char, ByVal dataType As DataType)
			MyBase.New(buffer, shape, stride, offset, ews, ordering, dataType)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Long, ByVal stride() As Long, ByVal ordering As Char, ByVal dataType As DataType)
			MyBase.New(buffer, shape, stride, ordering, dataType)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal order As Char)
			MyBase.New(data, order)
		End Sub

		Public Sub New(ByVal buffer As DataBuffer, ByVal shape() As Integer, ByVal strides() As Integer)
			MyBase.New(buffer, shape, strides)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal ordering As Char)
			MyBase.New(data, shape, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Long, ByVal ordering As Char)
			MyBase.New(data, shape, ordering)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal shape() As Long, ByVal ordering As Char)
			MyBase.New(data, shape, ordering)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ordering As Char)
			MyBase.New(data, shape, stride, offset, ordering)
		End Sub

		Public Sub New(ByVal dataType As DataType, ByVal shape() As Long, ByVal paddings() As Long, ByVal paddingOffsets() As Long, ByVal ordering As Char, ByVal workspace As MemoryWorkspace)
			MyBase.New(dataType, shape, paddings, paddingOffsets, ordering, workspace)
		End Sub

		Public Overrides Function dup() As INDArray
			If Me.Compressed AndAlso Me.ordering() = Nd4j.order().Value Then
				Dim ret As INDArray = Nd4j.createArrayFromShapeBuffer(data().dup(), Me.shapeInfoDataBuffer())
				ret.markAsCompressed(True)
				Return ret
			End If
	'        
	'            Special case for cuda: if we have not a view, and shapes do match - we
	'        
	'        
	'        if (!isView() && ordering() == Nd4j.order() && Shape.strideDescendingCAscendingF(this)) {
	'            AtomicAllocator allocator = AtomicAllocator.getInstance();
	'            INDArray array = Nd4j.createUninitialized(shape(), ordering());
	'        
	'            CudaContext context = allocator.getFlowController().prepareAction(array, this);
	'        
	'            Configuration configuration = CudaEnvironment.getInstance().getConfiguration();
	'        
	'            if (configuration.getMemoryModel() == Configuration.MemoryModel.IMMEDIATE && configuration.getFirstMemory() == AllocationStatus.DEVICE) {
	'        //                log.info("Path 0");
	'                allocator.memcpyDevice(array.data(), allocator.getPointer(this.data, context), this.data.length() * this.data().getElementSize(), 0, context);
	'            } else if (configuration.getMemoryModel() == Configuration.MemoryModel.DELAYED || configuration.getFirstMemory() == AllocationStatus.HOST) {
	'                AllocationPoint pointSrc = allocator.getAllocationPoint(this);
	'                AllocationPoint pointDst = allocator.getAllocationPoint(array);
	'        
	'                if (pointSrc.getAllocationStatus() == AllocationStatus.HOST) {
	'        //                    log.info("Path A");
	'                    NativeOpsHolder.getInstance().getDeviceNativeOps().memcpyAsync(pointDst.getPointers().getHostPointer(), pointSrc.getPointers().getHostPointer(), length * data.getElementSize(), CudaConstants.cudaMemcpyHostToHost, context.getOldStream());
	'                } else {
	'        //                    log.info("Path B. SRC dId: [{}], DST dId: [{}], cId: [{}]", pointSrc.getDeviceId(), pointDst.getDeviceId(), allocator.getDeviceId());
	'                    // this code branch is possible only with DELAYED memoryModel and src point being allocated on device
	'                    if (pointDst.getAllocationStatus() != AllocationStatus.DEVICE) {
	'                        allocator.getMemoryHandler().alloc(AllocationStatus.DEVICE, pointDst, pointDst.getShape(), false);
	'                    }
	'        
	'                    NativeOpsHolder.getInstance().getDeviceNativeOps().memcpyAsync(pointDst.getPointers().getDevicePointer(), pointSrc.getPointers().getHostPointer(), length * data.getElementSize(), CudaConstants.cudaMemcpyHostToDevice, context.getOldStream());
	'                }
	'            }
	'        
	'            allocator.getFlowController().registerAction(context, array, this);
	'            return array;
	'        } else 

			Dim res As val = MyBase.dup()
			Nd4j.Executioner.commit()
			Return res
		End Function

		Public Overrides Function dup(ByVal order As Char) As INDArray
			If Me.Compressed AndAlso Me.ordering() = order Then
				Dim ret As INDArray = Nd4j.createArrayFromShapeBuffer(data().dup(), Me.shapeInfoDataBuffer())
				ret.markAsCompressed(True)
				Return ret
			End If
	'        
	'        if (!isView() && ordering() == order && Shape.strideDescendingCAscendingF(this)) {
	'            AtomicAllocator allocator = AtomicAllocator.getInstance();
	'            INDArray array = Nd4j.createUninitialized(shape(), order);
	'        
	'            CudaContext context = allocator.getFlowController().prepareAction(array, this);
	'        
	'            Configuration configuration = CudaEnvironment.getInstance().getConfiguration();
	'        
	'            if (configuration.getMemoryModel() == Configuration.MemoryModel.IMMEDIATE && configuration.getFirstMemory() == AllocationStatus.DEVICE) {
	'                allocator.memcpyDevice(array.data(), allocator.getPointer(this.data, context), this.data.length() * this.data().getElementSize(), 0, context);
	'            } else if (configuration.getMemoryModel() == Configuration.MemoryModel.DELAYED || configuration.getFirstMemory() == AllocationStatus.HOST) {
	'                AllocationPoint pointSrc = allocator.getAllocationPoint(this);
	'                AllocationPoint pointDst = allocator.getAllocationPoint(array);
	'        
	'                if (pointSrc.getAllocationStatus() == AllocationStatus.HOST) {
	'                    NativeOpsHolder.getInstance().getDeviceNativeOps().memcpyAsync(pointDst.getPointers().getHostPointer(), pointSrc.getPointers().getHostPointer(), length * data.getElementSize(), CudaConstants.cudaMemcpyHostToHost, context.getOldStream());
	'                } else {
	'                    // this code branch is possible only with DELAYED memoryModel and src point being allocated on device
	'                    if (pointDst.getAllocationStatus() != AllocationStatus.DEVICE) {
	'                        allocator.getMemoryHandler().alloc(AllocationStatus.DEVICE, pointDst, pointDst.getShape(), false);
	'                    }
	'        
	'                    NativeOpsHolder.getInstance().getDeviceNativeOps().memcpyAsync(pointDst.getPointers().getDevicePointer(), pointSrc.getPointers().getDevicePointer(), length * data.getElementSize(), CudaConstants.cudaMemcpyHostToDevice, context.getOldStream());
	'                }
	'            }
	'        
	'            allocator.getFlowController().registerAction(context, array, this);
	'        
	'            return array;
	'        } else 
			Return MyBase.dup(order)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			'if (o != null) AtomicAllocator.getInstance().synchronizeHostData((INDArray) o);
			'AtomicAllocator.getInstance().synchronizeHostData(this);
			Return MyBase.Equals(o)
		End Function

		''' <summary>
		''' Generate string representation of the matrix.
		''' </summary>
		Public Overrides Function ToString() As String
			If Not S Then
				AtomicAllocator.Instance.synchronizeHostData(Me)
			End If
			Return MyBase.ToString()
		End Function

		''' 
		''' <summary>
		''' PLEASE NOTE: Never use this method, unless you 100% have to
		''' </summary>
		''' <param name="buffer"> </param>
		Public Overridable WriteOnly Property ShapeInfoDataBuffer As DataBuffer
			Set(ByVal buffer As DataBuffer)
				Me.shapeInformation_Conflict = buffer
				Me.jvmShapeInfo = New JvmShapeInfo(shapeInformation_Conflict.asLong())
			End Set
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private Object writeReplace() throws java.io.ObjectStreamException
		Private Function writeReplace() As Object
			Return New BaseNDArrayProxy(Me)
		End Function

		Public Overrides Function permutei(ParamArray ByVal rearrange() As Integer) As INDArray
			Nd4j.Executioner.push()

			Return MyBase.permutei(rearrange)
		End Function

		Public Overrides Function shapeDescriptor() As LongShapeDescriptor
			Return LongShapeDescriptor.fromShape(shape(), stride(), elementWiseStride(), ordering(), dataType(), Empty)
		End Function

		Public Overrides Function unsafeDuplication() As INDArray
			Return unsafeDuplication(True)
		End Function

		Public Overrides Function unsafeDuplication(ByVal blocking As Boolean) As INDArray
			WorkspaceUtils.assertValidArray(Me, "Cannot duplicate array")
			Dim rb As DataBuffer = If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, Nd4j.DataBufferFactory.createSame(Me.data_Conflict, False), Nd4j.DataBufferFactory.createSame(Me.data_Conflict, False, Nd4j.MemoryManager.CurrentWorkspace))

			Dim ret As INDArray = Nd4j.createArrayFromShapeBuffer(rb, Me.shapeInfoDataBuffer())


			If blocking Then
				Nd4j.Executioner.push()
			End If


			'Nd4j.getExecutioner().commit();

			Dim allocator As AtomicAllocator = AtomicAllocator.Instance
			Dim context As val = CType(allocator.DeviceContext, CudaContext)

			Dim srcPoint As AllocationPoint = allocator.getAllocationPoint(Me)
			Dim dstPoint As AllocationPoint = allocator.getAllocationPoint(ret)

			Dim route As Integer = 0
	'        long time1 = System.currentTimeMillis();
			Dim direction As MemcpyDirection = MemcpyDirection.HOST_TO_HOST
			Dim prof As val = PerformanceTracker.Instance.helperStartTransaction()

			If srcPoint.ActualOnDeviceSide Then
				route = 1
				NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(dstPoint.DevicePointer, srcPoint.DevicePointer, Me.data_Conflict.length() * Me.data_Conflict.ElementSize, CudaConstants.cudaMemcpyDeviceToDevice,If(blocking, context.getOldStream(), context.getSpecialStream()))
				dstPoint.tickDeviceWrite()
				direction = MemcpyDirection.DEVICE_TO_DEVICE
			Else
				route = 3
				NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(dstPoint.DevicePointer, srcPoint.HostPointer, Me.data_Conflict.length() * Me.data_Conflict.ElementSize, CudaConstants.cudaMemcpyHostToDevice,If(blocking, context.getOldStream(), context.getSpecialStream()))
				dstPoint.tickDeviceWrite()
				direction = MemcpyDirection.HOST_TO_DEVICE
			End If


			'allocator.memcpyDevice(ret.data(), allocator.getAllocationPoint(this.data).getDevicePointer(), this.data.length() * this.data().getElementSize(), 0, context);

			If blocking Then
				context.syncOldStream()
			Else
				context.syncSpecialStream()
			End If

			PerformanceTracker.Instance.helperRegisterTransaction(dstPoint.DeviceId, prof, dstPoint.NumberOfBytes, direction)

	'        AtomicAllocator.getInstance().synchronizeHostData(ret);
	'
	'        long time2 = System.currentTimeMillis();
	'
	'        long bytes = this.data.length() * this.data.getElementSize();
	'        long spent = time2 - time1;
	'
	'        float bw = (1000 * bytes / spent) / 1024 / 1024.0f / 1024; //1000 / spent * bytes / 1024 / 1024 / 1024;
	'
	'        log.info("Route: [{}]; Blocking: {}; {} bytes; {} ms; Bandwidth: {} GB/s", route, blocking, bytes, spent, String.format("%.2f", bw));
	'
			Return ret
		End Function

		Public Overrides Function leverageTo(ByVal id As String) As INDArray
			If Not Attached Then
	'            log.info("Skipping detached");
				Return Me
			End If

			If Not Nd4j.WorkspaceManager.checkIfWorkspaceExists(id) Then
	'            log.info("Skipping non-existent");
				Return Me
			End If

			WorkspaceUtils.assertValidArray(Me, "Cannot leverage INDArray to new workspace")

			Dim current As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace

			Dim target As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(id)

			If current Is target Then
	'            log.info("Skipping equals A");
				Return Me
			End If

			If Me.data_Conflict.ParentWorkspace Is target Then
	'            log.info("Skipping equals B");
				Return Me
			End If

			Nd4j.MemoryManager.CurrentWorkspace = target

			Dim copy As INDArray = Nothing
			If Not Me.View Then
				Nd4j.Executioner.commit()

				Dim buffer As val = Nd4j.createBuffer(Me.length(), False)

				Dim pointDst As val = AtomicAllocator.Instance.getAllocationPoint(buffer)
				Dim pointSrc As val = AtomicAllocator.Instance.getAllocationPoint(Me.data_Conflict)

				Dim context As val = AtomicAllocator.Instance.FlowController.prepareAction(pointDst, pointSrc)

				Dim direction As MemcpyDirection = MemcpyDirection.DEVICE_TO_DEVICE
				Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

				If pointSrc.isActualOnDeviceSide() Then
					If NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(pointDst.getDevicePointer(), pointSrc.getDevicePointer(), Me.length() * Nd4j.sizeOfDataType(buffer.dataType()), CudaConstants.cudaMemcpyDeviceToDevice, context.getOldStream()) = 0 Then
						Throw New ND4JIllegalStateException("memcpyAsync failed")
					End If
				Else
					If NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(pointDst.getDevicePointer(), pointSrc.getHostPointer(), Me.length() * Nd4j.sizeOfDataType(buffer.dataType()), CudaConstants.cudaMemcpyHostToDevice, context.getOldStream()) = 0 Then
						Throw New ND4JIllegalStateException("memcpyAsync failed")
					End If

					direction = MemcpyDirection.HOST_TO_DEVICE
				End If

				context.syncOldStream()

				PerformanceTracker.Instance.helperRegisterTransaction(pointDst.getDeviceId(), perfD, pointSrc.getNumberOfBytes(), direction)

				copy = Nd4j.createArrayFromShapeBuffer(buffer, Me.shapeInfoDataBuffer())

				' tag buffer as valid on device side
				pointDst.tickDeviceWrite()

				AtomicAllocator.Instance.FlowController.registerAction(context, pointDst, pointSrc)
			Else
				copy = Me.dup(Me.ordering())

				Nd4j.Executioner.commit()
			End If

			Nd4j.MemoryManager.CurrentWorkspace = current

			Return copy
		End Function

		Public Overrides Function migrate() As INDArray
			WorkspaceUtils.assertValidArray(Me, "Cannot leverage INDArray to new workspace")
			Dim current As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace

			If current Is Nothing Then
				Return Me
			End If

			Dim copy As INDArray = Nothing

			If Not Me.View Then
				Nd4j.Executioner.commit()

				Dim buffer As val = Nd4j.createBuffer(Me.dataType(), Me.length(), False)

				Dim pointDst As val = AtomicAllocator.Instance.getAllocationPoint(buffer)
				Dim pointSrc As val = AtomicAllocator.Instance.getAllocationPoint(Me.data_Conflict)


				Dim context As val = AtomicAllocator.Instance.FlowController.prepareAction(pointDst, pointSrc)

				Dim direction As MemcpyDirection = MemcpyDirection.DEVICE_TO_DEVICE
				Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

				If pointSrc.isActualOnDeviceSide() Then
					If NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(pointDst.getDevicePointer(), pointSrc.getDevicePointer(), Me.length() * Nd4j.sizeOfDataType(buffer.dataType()), CudaConstants.cudaMemcpyDeviceToDevice, context.getOldStream()) = 0 Then
						Throw New ND4JIllegalStateException("memcpyAsync failed")
					End If
				Else
					If NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(pointDst.getDevicePointer(), pointSrc.getHostPointer(), Me.length() * Nd4j.sizeOfDataType(buffer.dataType()), CudaConstants.cudaMemcpyHostToDevice, context.getOldStream()) = 0 Then
						Throw New ND4JIllegalStateException("memcpyAsync failed")
					End If

					direction = MemcpyDirection.HOST_TO_DEVICE
				End If

				context.syncOldStream()

				PerformanceTracker.Instance.helperRegisterTransaction(pointDst.getDeviceId(), perfD, pointDst.getNumberOfBytes(), direction)

				If pointDst.getDeviceId() <> Nd4j.MemoryManager.CurrentWorkspace.DeviceId Then
					pointDst.setDeviceId(Nd4j.MemoryManager.CurrentWorkspace.DeviceId)
				End If

				copy = Nd4j.createArrayFromShapeBuffer(buffer, Me.shapeInfoDataBuffer())

				' tag buffer as valid on device side
				pointDst.tickDeviceWrite()

				AtomicAllocator.Instance.FlowController.registerAction(context, pointDst, pointSrc)
			Else
				copy = Me.dup(Me.ordering())
			End If

			Return copy
		End Function

		Protected Friend Overrides Function stringBuffer(ByVal builder As FlatBufferBuilder, ByVal buffer As DataBuffer) As Integer
			Preconditions.checkArgument(buffer.dataType() = DataType.UTF8, "This method can be called on UTF8 buffers only")
			Try
				Dim bos As New MemoryStream()
				Dim dos As New DataOutputStream(bos)

				Dim numWords As val = Me.length()
				Dim ub As val = DirectCast(buffer, CudaUtf8Buffer)
				' writing length first
				Dim t As val = length()
				Dim ptr As val = CType(ub.pointer(), BytePointer)

				' now write all strings as bytes
				For i As Integer = 0 To ub.length() - 1
					dos.writeByte(ptr.get(i))
				Next i

				Dim bytes As val = bos.toByteArray()
				Return FlatArray.createBufferVector(builder, bytes)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		Public Overrides Function getString(ByVal index As Long) As String
			If Not S Then
				Throw New System.NotSupportedException("This method is usable only on String dataType, but got [" & Me.dataType() & "]")
			End If

			Return DirectCast(data_Conflict, CudaUtf8Buffer).getString(index)
		End Function

	'
	'    @Override
	'    public INDArray convertToHalfs() {
	'        if (data.dataType() == DataType.HALF)
	'            return this;
	'
	'        val factory = Nd4j.getNDArrayFactory();
	'        val buffer = Nd4j.createBuffer(new long[]{this.length()}, DataType.HALF);
	'
	'        factory.convertDataEx(convertType(data.dataType()), AtomicAllocator.getInstance().getPointer(this.data()),
	'                DataTypeEx.FLOAT16, AtomicAllocator.getInstance().getPointer(buffer), buffer.length());
	'
	'        AtomicAllocator.getInstance().getAllocationPoint(buffer).tickDeviceWrite();
	'
	'        return Nd4j.createArrayFromShapeBuffer(buffer, this.shapeInformation);
	'    }
	'
	'
	'    @Override
	'    public INDArray convertToFloats() {
	'        if (data.dataType() == DataType.FLOAT)
	'            return this;
	'
	'        val factory = Nd4j.getNDArrayFactory();
	'        val buffer = Nd4j.createBuffer(new long[]{this.length()}, DataType.FLOAT);
	'
	'        factory.convertDataEx(convertType(data.dataType()), AtomicAllocator.getInstance().getPointer(this.data()), DataTypeEx.FLOAT, AtomicAllocator.getInstance().getPointer(buffer), buffer.length());
	'
	'        AtomicAllocator.getInstance().getAllocationPoint(buffer).tickDeviceWrite();
	'
	'        return Nd4j.createArrayFromShapeBuffer(buffer, this.shapeInformation);
	'    }
	'
	'    @Override
	'    public INDArray convertToDoubles() {
	'        if (data.dataType() == DataType.DOUBLE)
	'            return this;
	'
	'        val factory = Nd4j.getNDArrayFactory();
	'        val buffer = Nd4j.createBuffer(new long[]{this.length()}, DataType.DOUBLE);
	'
	'        factory.convertDataEx(convertType(data.dataType()), AtomicAllocator.getInstance().getPointer(this.data()), DataTypeEx.DOUBLE, AtomicAllocator.getInstance().getPointer(buffer), buffer.length());
	'
	'        AtomicAllocator.getInstance().getAllocationPoint(buffer).tickDeviceWrite();
	'
	'        return Nd4j.createArrayFromShapeBuffer(buffer, this.shapeInformation);
	'    }
	'
	'
	End Class

End Namespace