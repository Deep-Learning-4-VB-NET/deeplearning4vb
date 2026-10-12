Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports BasicAffinityManager = org.nd4j.linalg.api.concurrency.BasicAffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BaseCudaDataBuffer = org.nd4j.linalg.jcublas.buffer.BaseCudaDataBuffer
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
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

Namespace org.nd4j.jita.concurrency


	''' <summary>
	''' AffinityManager implementation for CUDA
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class CudaAffinityManager
		Inherits BasicAffinityManager

		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(CudaAffinityManager))

		Private affinityMap As IDictionary(Of Long, Integer) = New ConcurrentDictionary(Of Long, Integer)()
		Private devPtr As New AtomicInteger(0)
		Private affiliated As New ThreadLocal(Of AtomicBoolean)()

'JAVA TO VB CONVERTER NOTE: The field numberOfDevices was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numberOfDevices_Conflict As New AtomicInteger(-1)

		Public Sub New()
			MyBase.New()

		End Sub

		''' <summary>
		''' This method returns deviceId for current thread.
		''' 
		''' If no device was assigned to this thread before this call, it'll be assinged here.
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property DeviceForCurrentThread As Integer?
			Get
				Return NativeOpsHolder.Instance.getDeviceNativeOps().getDevice()
			End Get
		End Property

		''' <summary>
		''' This method returns deviceId for a given thread
		''' @return
		''' </summary>
		Public Overrides Function getDeviceForThread(ByVal threadId As Long) As Integer?
			Dim id As Integer? = affinityMap(threadId)
			If id Is Nothing Then
				' if this is current thread - we're still able to fetch id from native side, and update map
				If threadId = Thread.CurrentThread.getId() Then
					id = NativeOpsHolder.Instance.getDeviceNativeOps().getDevice()
					affinityMap(Convert.ToInt64(threadId)) = id
				Else
					' TODO: we should get rid of this method, and forbid such kind of queries
					Throw New Exception("Affinity for thread [" & threadId & "] wasn't defined yet")
				End If
			End If

			Return id
		End Function


		''' <summary>
		''' This method returns device id available. Round-robin balancing used here.
		''' </summary>
		''' <param name="threadId"> this parameter can be anything, it's used for logging only.
		''' @return </param>
		Protected Friend Overridable Function getNextDevice(ByVal threadId As Long) As Integer?
			Dim device As Integer? = Nothing
			If Not CudaEnvironment.Instance.Configuration.ForcedSingleGPU AndAlso NumberOfDevices > 0 Then
				' simple round-robin here
				SyncLock Me
					device = CudaEnvironment.Instance.Configuration.getAvailableDevices().get(devPtr.getAndIncrement())

					' We check only for number of entries here, not their actual values
					If devPtr.get() >= CudaEnvironment.Instance.Configuration.getAvailableDevices().size() Then
						devPtr.set(0)
					End If

					Dim t As val = Thread.CurrentThread
					Dim n As val = If(t.getId() = threadId, t.getName(), "N/A")

					logger.debug("Mapping thread [{} - {}] to device [{}], out of [{}] devices...", threadId, n, device, CudaEnvironment.Instance.Configuration.getAvailableDevices().size())
				End SyncLock
			Else
				device = CudaEnvironment.Instance.Configuration.getAvailableDevices().get(0)
				logger.debug("Single device is forced, mapping to device [{}]", device)
			End If

			Return device
		End Function

		''' <summary>
		''' This method returns number of available devices in system.
		''' 
		''' Please note: returned value might be different from actual number of used devices.
		''' </summary>
		''' <returns> total number of devices </returns>
		Public Overrides ReadOnly Property NumberOfDevices As Integer
			Get
				If numberOfDevices_Conflict.get() < 0 Then
					SyncLock Me
						If numberOfDevices_Conflict.get() < 1 Then
							numberOfDevices_Conflict.set(NativeOpsHolder.Instance.getDeviceNativeOps().getAvailableDevices())
						End If
					End SyncLock
				End If
    
				Return numberOfDevices_Conflict.get()
			End Get
		End Property

		''' <summary>
		''' Utility method, to associate INDArray with specific device (backend-specific)
		''' </summary>
		''' <param name="array"> </param>
		Public Overrides Sub touch(ByVal array As INDArray)
			If array Is Nothing Then
				Return
			End If

			touch(array.data())
			touch(array.shapeInfoDataBuffer())
		End Sub

		''' <summary>
		''' Utility method, to associate INDArray with specific device (backend-specific)
		''' </summary>
		''' <param name="buffer"> </param>
		Public Overrides Sub touch(ByVal buffer As DataBuffer)
			If buffer Is Nothing Then
				Return
			End If

			Dim point As AllocationPoint = AtomicAllocator.Instance.getAllocationPoint(buffer)

			If point.isConstant() Then
				Nd4j.ConstantHandler.relocateConstantSpace(buffer)
			Else
				AtomicAllocator.Instance.MemoryHandler.relocateObject(buffer)
			End If
		End Sub

		''' <summary>
		''' This method replicates given INDArray, and places it to target device.
		''' </summary>
		''' <param name="deviceId"> target deviceId </param>
		''' <param name="array">    INDArray to replicate
		''' @return </param>
		Public Overrides Function replicateToDevice(ByVal deviceId As Integer?, ByVal array As INDArray) As INDArray
			SyncLock Me
				If array Is Nothing Then
					Return Nothing
				End If
        
				' string arrays are stored in host memory only atm
				If array.S Then
					Return array.dup(array.ordering())
				End If
        
				If array.View Then
					Throw New System.NotSupportedException("It's impossible to replicate View")
				End If
        
				Dim shape As val = array.shape()
				Dim stride As val = array.stride()
				Dim elementWiseStride As val = array.elementWiseStride()
				Dim ordering As val = array.ordering()
				Dim length As val = array.length()
				Dim dtype As val = array.dataType()
				Dim empty As val = array.Empty
        
				' we use this call to get device memory updated
				AtomicAllocator.Instance.getPointer(array, AtomicAllocator.Instance.DeviceContext)
        
				Dim currentDeviceId As Integer = getDeviceForCurrentThread()
        
				If currentDeviceId <> deviceId.Value Then
					unsafeSetDevice(deviceId)
				End If
        
        
				Dim newDataBuffer As DataBuffer = replicateToDevice(deviceId, array.data())
				Dim newShapeBuffer As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(shape, stride, elementWiseStride, ordering, dtype, empty).First
				Dim result As INDArray = Nd4j.createArrayFromShapeBuffer(newDataBuffer, newShapeBuffer)
        
				If currentDeviceId <> deviceId.Value Then
					unsafeSetDevice(currentDeviceId)
				End If
        
        
				Return result
			End SyncLock
		End Function

		''' <summary>
		''' This method replicates given DataBuffer, and places it to target device.
		''' </summary>
		''' <param name="deviceId"> target deviceId </param>
		''' <param name="buffer">
		''' @return </param>
		Public Overrides Function replicateToDevice(ByVal deviceId As Integer?, ByVal buffer As DataBuffer) As DataBuffer
			If buffer Is Nothing Then
				Return Nothing
			End If

			Dim currentDeviceId As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()

			If currentDeviceId <> deviceId Then
				Nd4j.AffinityManager.unsafeSetDevice(deviceId)
			End If

			Dim dstBuffer As DataBuffer = Nd4j.createBuffer(buffer.dataType(), buffer.length(), False)
			AtomicAllocator.Instance.memcpy(dstBuffer, buffer)

			If currentDeviceId <> deviceId Then
				Nd4j.AffinityManager.unsafeSetDevice(currentDeviceId)
			End If

			Return dstBuffer
		End Function

		''' <summary>
		''' This method marks given INDArray as actual in specific location (either host, device, or both)
		''' </summary>
		''' <param name="array"> </param>
		''' <param name="location"> </param>
		Public Overrides Sub tagLocation(ByVal array As INDArray, ByVal location As Location)
			' we can't tag empty arrays.
			If array.Empty Then
				Return
			End If

			If location = Location.HOST Then
				AtomicAllocator.Instance.getAllocationPoint(array).tickHostWrite()
			ElseIf location = Location.DEVICE Then
				AtomicAllocator.Instance.getAllocationPoint(array).tickDeviceWrite()
			ElseIf location = Location.EVERYWHERE Then
				AtomicAllocator.Instance.getAllocationPoint(array).tickDeviceWrite()
				AtomicAllocator.Instance.getAllocationPoint(array).tickHostRead()
			End If
		End Sub

		''' <summary>
		''' This method marks given DataBuffer as actual in specific location (either host, device, or both)
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="location"> </param>
		Public Overrides Sub tagLocation(ByVal buffer As DataBuffer, ByVal location As Location)
			If location = Location.HOST Then
				AtomicAllocator.Instance.getAllocationPoint(buffer).tickHostWrite()
			ElseIf location = Location.DEVICE Then
				AtomicAllocator.Instance.getAllocationPoint(buffer).tickDeviceWrite()
			ElseIf location = Location.EVERYWHERE Then
				AtomicAllocator.Instance.getAllocationPoint(buffer).tickDeviceWrite()
				AtomicAllocator.Instance.getAllocationPoint(buffer).tickHostRead()
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public System.Nullable<Integer> getDeviceForArray(@NonNull INDArray array)
		Public Overrides Function getDeviceForArray(ByVal array As INDArray) As Integer?
			Return AtomicAllocator.Instance.getDeviceId(array)
		End Function

		Public Overrides Sub unsafeSetDevice(ByVal deviceId As Integer?)
			' actually set device
			NativeOpsHolder.Instance.getDeviceNativeOps().setDevice(deviceId)

			' reset saved context, so it will be recreated on first call
			AtomicAllocator.Instance.MemoryHandler.resetCachedContext()
		End Sub

		Public Overrides Sub ensureLocation(ByVal array As INDArray, ByVal location As Location)
			' to location to ensure for empty array
			If array Is Nothing OrElse array.Empty OrElse array.S Then
				Return
			End If

			' let's make sure host pointer actually exists
			DirectCast(array.data(), BaseCudaDataBuffer).lazyAllocateHostPointer()

			Dim point As val = AtomicAllocator.Instance.getAllocationPoint(array)
			Select Case location
				Case HOST
						AtomicAllocator.Instance.synchronizeHostData(array)
				Case DEVICE
						AtomicAllocator.Instance.FlowController.synchronizeToDevice(point)
				Case Else
					AtomicAllocator.Instance.synchronizeHostData(array)
					AtomicAllocator.Instance.FlowController.synchronizeToDevice(point)
			End Select
		End Sub

		Public Overrides Function getActiveLocation(ByVal array As INDArray) As Location
			If array.Empty Then
				Return Location.EVERYWHERE
			End If

			Dim point As val = AtomicAllocator.Instance.getAllocationPoint(array)

			If point.isActualOnDeviceSide() AndAlso point.isActualOnHostSide() Then
				Return Location.EVERYWHERE
			ElseIf point.isActualOnDeviceSide() Then
				Return Location.DEVICE
			Else
				Return Location.HOST
			End If
		End Function

		Public Overrides ReadOnly Property CrossDeviceAccessSupported As Boolean
			Get
				Return NativeOpsHolder.Instance.getDeviceNativeOps().isP2PAvailable() AndAlso CudaEnvironment.Instance.Configuration.isCrossDeviceAccessAllowed()
			End Get
		End Property

		Public Overrides Sub allowCrossDeviceAccess(ByVal reallyAllow As Boolean)
			CudaEnvironment.Instance.Configuration.allowCrossDeviceAccess(reallyAllow)
		End Sub
	End Class

End Namespace