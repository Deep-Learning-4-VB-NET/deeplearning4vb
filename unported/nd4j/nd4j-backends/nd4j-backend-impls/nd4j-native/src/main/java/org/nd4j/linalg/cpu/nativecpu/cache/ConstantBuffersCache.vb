Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AllocationsTracker = org.nd4j.linalg.api.memory.AllocationsTracker
Imports AllocationKind = org.nd4j.linalg.api.memory.enums.AllocationKind
Imports ArrayDescriptor = org.nd4j.linalg.cache.ArrayDescriptor
Imports BasicConstantHandler = org.nd4j.linalg.cache.BasicConstantHandler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.cpu.nativecpu.cache


	Public Class ConstantBuffersCache
		Inherits BasicConstantHandler

		Protected Friend buffersCache As IDictionary(Of ArrayDescriptor, DataBuffer) = New ConcurrentDictionary(Of ArrayDescriptor, DataBuffer)()
		Private counter As New AtomicInteger(0)
		Private bytes As New AtomicLong(0)
		Private Const MAX_ENTRIES As Integer = 1000

		''' <summary>
		''' This method removes all cached constants
		''' </summary>
		Public Overrides Sub purgeConstants()
			buffersCache = New ConcurrentDictionary(Of ArrayDescriptor, DataBuffer)()
		End Sub

		Public Overrides Function getConstantBuffer(ByVal array() As Integer, ByVal dataType As DataType) As DataBuffer
			Dim descriptor As New ArrayDescriptor(array, dataType)

			If Not buffersCache.ContainsKey(descriptor) Then
				Dim buffer As DataBuffer = Nd4j.createTypedBufferDetached(array, dataType)

				If counter.get() < MAX_ENTRIES Then
					counter.incrementAndGet()
					buffersCache(descriptor) = buffer

					bytes.addAndGet(array.Length * Nd4j.sizeOfDataType(dataType))
					AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT, 0, array.Length * Nd4j.sizeOfDataType(dataType))
				End If
				Return buffer
			End If

			Return buffersCache(descriptor)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Boolean, ByVal dataType As DataType) As DataBuffer
			Dim descriptor As New ArrayDescriptor(array, dataType)

			If Not buffersCache.ContainsKey(descriptor) Then
				Dim buffer As DataBuffer = Nd4j.createTypedBufferDetached(array, dataType)

				If counter.get() < MAX_ENTRIES Then
					counter.incrementAndGet()
					buffersCache(descriptor) = buffer

					bytes.addAndGet(array.Length * Nd4j.sizeOfDataType(dataType))
					AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT, 0, array.Length * Nd4j.sizeOfDataType(dataType))
				End If
				Return buffer
			End If

			Return buffersCache(descriptor)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Double, ByVal dataType As DataType) As DataBuffer
			Dim descriptor As New ArrayDescriptor(array, dataType)

			If Not buffersCache.ContainsKey(descriptor) Then
				Dim buffer As DataBuffer = Nd4j.createTypedBufferDetached(array, dataType)

				If counter.get() < MAX_ENTRIES Then
					counter.incrementAndGet()
					buffersCache(descriptor) = buffer

					bytes.addAndGet(array.Length * Nd4j.sizeOfDataType(dataType))
					AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT, 0, array.Length * Nd4j.sizeOfDataType(dataType))
				End If
				Return buffer
			End If

			Return buffersCache(descriptor)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Single, ByVal dataType As DataType) As DataBuffer
			Dim descriptor As New ArrayDescriptor(array, dataType)

			If Not buffersCache.ContainsKey(descriptor) Then
				Dim buffer As DataBuffer = Nd4j.createTypedBufferDetached(array, dataType)

				If counter.get() < MAX_ENTRIES Then
					counter.incrementAndGet()
					buffersCache(descriptor) = buffer

					bytes.addAndGet(array.Length * Nd4j.sizeOfDataType(dataType))
					AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT, 0, array.Length * Nd4j.sizeOfDataType(dataType))
				End If
				Return buffer
			End If

			Return buffersCache(descriptor)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Long, ByVal dataType As DataType) As DataBuffer
			Dim descriptor As New ArrayDescriptor(array, dataType)

			If Not buffersCache.ContainsKey(descriptor) Then
				Dim buffer As DataBuffer = Nd4j.createBufferDetached(array)

				If counter.get() < MAX_ENTRIES Then
					counter.incrementAndGet()
					buffersCache(descriptor) = buffer

					bytes.addAndGet(array.Length * Nd4j.sizeOfDataType(dataType))
					AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT, 0, array.Length * Nd4j.sizeOfDataType(dataType))
				End If
				Return buffer
			End If

			Return buffersCache(descriptor)
		End Function

		Public Overrides ReadOnly Property CachedBytes As Long
			Get
				Return bytes.get()
			End Get
		End Property
	End Class

End Namespace