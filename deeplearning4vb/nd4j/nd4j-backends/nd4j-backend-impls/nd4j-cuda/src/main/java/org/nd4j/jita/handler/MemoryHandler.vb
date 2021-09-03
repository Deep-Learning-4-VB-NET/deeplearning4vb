Imports System.Collections.Generic
Imports Table = org.nd4j.shade.guava.collect.Table
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports PointersPair = org.nd4j.jita.allocator.pointers.PointersPair
Imports Configuration = org.nd4j.jita.conf.Configuration
Imports FlowController = org.nd4j.jita.flow.FlowController
Imports MemoryProvider = org.nd4j.jita.memory.MemoryProvider
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext

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

Namespace org.nd4j.jita.handler


	''' <summary>
	''' MemoryHandler interface describes methods for data access
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Interface MemoryHandler

		''' <summary>
		''' This method gets called from Allocator, during Allocator/MemoryHandler initialization
		''' </summary>
		''' <param name="configuration"> </param>
		''' <param name="allocator"> </param>
		Sub init(ByVal configuration As Configuration, ByVal allocator As Allocator)

		''' <summary>
		''' This method returns if this MemoryHandler instance is device-dependant (i.e. CUDA)
		''' </summary>
		''' <returns> TRUE if dependant, FALSE otherwise </returns>
		ReadOnly Property DeviceDependant As Boolean


		ReadOnly Property CudaContext As CudaContext


		''' <summary>
		''' This method removes AllocationPoint from corresponding device/host trackers </summary>
		''' <param name="point"> </param>
		''' <param name="location"> </param>
		Sub forget(ByVal point As AllocationPoint, ByVal location As AllocationStatus)

		''' <summary>
		''' This method causes memory synchronization on host side.
		'''  Viable only for Device-dependant MemoryHandlers
		''' </summary>
		''' <param name="threadId"> </param>
		''' <param name="deviceId"> </param>
		''' <param name="point"> </param>
		Sub synchronizeThreadDevice(ByVal threadId As Long?, ByVal deviceId As Integer?, ByVal point As AllocationPoint)

		''' <summary>
		''' Allocate specified memory chunk on specified device/host
		''' </summary>
		''' <param name="targetMode"> valid arguments are DEVICE, ZERO
		''' @return </param>
		Function alloc(ByVal targetMode As AllocationStatus, ByVal point As AllocationPoint, ByVal shape As AllocationShape, ByVal initialize As Boolean) As PointersPair

		''' <summary>
		''' This method checks if specified device has free memory
		''' 
		''' @return
		''' </summary>
		Function pingDeviceForFreeMemory(ByVal deviceId As Integer?, ByVal requiredMemory As Long) As Boolean

		''' <summary>
		'''  Relocates specific chunk of memory from one storage to another
		''' </summary>
		''' <param name="currentStatus"> </param>
		''' <param name="targetStatus"> </param>
		''' <param name="point"> </param>
		Sub relocate(ByVal currentStatus As AllocationStatus, ByVal targetStatus As AllocationStatus, ByVal point As AllocationPoint, ByVal shape As AllocationShape, ByVal context As CudaContext)

		''' <summary>
		''' Copies memory from device to host, if needed.
		''' Device copy is preserved as is.
		''' </summary>
		''' <param name="point"> </param>
		Sub copyback(ByVal point As AllocationPoint, ByVal shape As AllocationShape)


		''' <summary>
		''' Copies memory from host buffer to device.
		''' Host copy is preserved as is.
		''' </summary>
		''' <param name="point"> </param>
		Sub copyforward(ByVal point As AllocationPoint, ByVal shape As AllocationShape)

		''' <summary>
		''' Copies memory from device to zero-copy memory
		''' </summary>
		''' <param name="point"> </param>
		''' <param name="shape"> </param>
		Sub fallback(ByVal point As AllocationPoint, ByVal shape As AllocationShape)

		''' <summary>
		''' This method frees memory chunk specified by pointer
		''' </summary>
		''' <param name="point"> </param>
		Sub free(ByVal point As AllocationPoint, ByVal target As AllocationStatus)

		''' <summary>
		''' This method returns initial allocation location. So, it can be HOST, or DEVICE if environment allows that.
		''' 
		''' @return
		''' </summary>
		ReadOnly Property InitialLocation As AllocationStatus

		''' <summary>
		''' This method initializes specific device for current thread
		''' </summary>
		Sub initializeDevice(ByVal threadId As Long?, ByVal deviceId As Integer?)

		''' <summary>
		'''  Synchronous version of memcpy.
		''' 
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcPointer"> </param>
		''' <param name="length"> </param>
		''' <param name="dstOffset"> </param>
		Sub memcpyBlocking(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long)

		''' <summary>
		''' Asynchronous version of memcpy
		''' 
		''' PLEASE NOTE: This is device-dependent method, if it's not supported in your environment, blocking call will be used instead.
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcPointer"> </param>
		''' <param name="length"> </param>
		''' <param name="dstOffset"> </param>
		Sub memcpyAsync(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long)

		Sub memcpySpecial(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long)

		Sub memcpyDevice(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long, ByVal context As CudaContext)

		''' <summary>
		''' Synchronous version of memcpy
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcBuffer"> </param>
		Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer)

		''' <summary>
		''' PLEASE NOTE: Specific implementation, on systems without special devices can return HostPointer here
		''' @return
		''' </summary>
		Function getDevicePointer(ByVal buffer As DataBuffer, ByVal context As CudaContext) As Pointer

		''' <summary>
		''' PLEASE NOTE: This method always returns pointer valid within OS memory space
		''' @return
		''' </summary>
		Function getHostPointer(ByVal buffer As DataBuffer) As Pointer


		''' <summary>
		''' This method returns total amount of memory allocated within system
		''' 
		''' @return
		''' </summary>
		ReadOnly Property AllocationStatistics As Table(Of AllocationStatus, Integer, Long)

		''' <summary>
		''' This method returns total amount of host memory allocated within this MemoryHandler
		''' 
		''' @return
		''' </summary>
		ReadOnly Property AllocatedHostMemory As Long

		''' <summary>
		''' This method returns total amount of memory allocated at specified device
		''' </summary>
		''' <param name="device">
		''' @return </param>
		Function getAllocatedDeviceMemory(ByVal device As Integer?) As Long

		''' <summary>
		''' This method returns number of allocated objects within specific bucket
		''' </summary>
		''' <param name="bucketId">
		''' @return </param>
		Function getAllocatedHostObjects(ByVal bucketId As Long?) As Long

		''' <summary>
		''' This method returns total number of allocated objects in host memory
		''' @return
		''' </summary>
		ReadOnly Property AllocatedHostObjects As Long

		''' <summary>
		''' This method returns total number of object allocated on specified device
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
		Function getAllocatedDeviceObjects(ByVal deviceId As Integer?) As Long

		''' <summary>
		''' This method returns set of allocation tracking IDs for specific device
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
		Function getDeviceTrackingPoints(ByVal deviceId As Integer?) As ISet(Of Long)

		''' <summary>
		''' This method returns sets of allocation tracking IDs for specific bucket
		''' </summary>
		''' <param name="bucketId">
		''' @return </param>
		Function getHostTrackingPoints(ByVal bucketId As Long?) As ISet(Of Long)

		''' <summary>
		''' This method removes specific previously allocated object from device memory
		''' </summary>
		''' <param name="threadId"> </param>
		''' <param name="deviceId"> </param>
		''' <param name="objectId"> </param>
		''' <param name="point"> </param>
		''' <param name="copyback"> </param>
		Sub purgeDeviceObject(ByVal threadId As Long?, ByVal deviceId As Integer?, ByVal objectId As Long?, ByVal point As AllocationPoint, ByVal copyback As Boolean)

		''' <summary>
		''' This method removes specific previously allocated object from host memory
		''' </summary>
		''' <param name="bucketId"> </param>
		''' <param name="objectId"> </param>
		''' <param name="point"> </param>
		''' <param name="copyback"> </param>
		Sub purgeZeroObject(ByVal bucketId As Long?, ByVal objectId As Long?, ByVal point As AllocationPoint, ByVal copyback As Boolean)

		''' <summary>
		''' This method returns set of available devices
		''' @return
		''' </summary>
		ReadOnly Property AvailableDevices As ISet(Of Integer)

		''' <summary>
		''' This method returns device ID for current thread
		''' 
		''' @return
		''' </summary>
		ReadOnly Property DeviceId As Integer?

		''' <summary>
		''' Returns <seealso cref="getDeviceId()"/> wrapped as a <seealso cref="Pointer"/>. </summary>
		ReadOnly Property DeviceIdPointer As Pointer

		''' <summary>
		''' This method returns ExternalContext wrapper (if applicable)
		''' @return
		''' </summary>
		ReadOnly Property DeviceContext As CudaContext

		Sub registerAction(ByVal context As CudaContext, ByVal result As INDArray, ParamArray ByVal operands() As INDArray)

		ReadOnly Property FlowController As FlowController

		ReadOnly Property MemoryProvider As MemoryProvider

		Function promoteObject(ByVal buffer As DataBuffer) As Boolean

		Sub relocateObject(ByVal buffer As DataBuffer)

		Sub resetCachedContext()
	End Interface

End Namespace