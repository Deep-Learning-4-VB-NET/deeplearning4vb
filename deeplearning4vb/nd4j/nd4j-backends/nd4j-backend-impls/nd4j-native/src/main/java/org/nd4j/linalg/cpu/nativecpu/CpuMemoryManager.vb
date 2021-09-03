Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AllocationsTracker = org.nd4j.linalg.api.memory.AllocationsTracker
Imports AllocationKind = org.nd4j.linalg.api.memory.enums.AllocationKind
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BasicMemoryManager = org.nd4j.linalg.api.memory.BasicMemoryManager
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
'ORIGINAL LINE: @Slf4j public class CpuMemoryManager extends org.nd4j.linalg.api.memory.BasicMemoryManager
	Public Class CpuMemoryManager
		Inherits BasicMemoryManager

		Public Overrides Function allocate(ByVal bytes As Long, ByVal kind As MemoryKind, ByVal initialize As Boolean) As Pointer
			Dim ptr As Pointer = NativeOpsHolder.Instance.getDeviceNativeOps().mallocHost(bytes, 0)

			If ptr Is Nothing OrElse ptr.address() = 0L Then
				Throw New System.OutOfMemoryException("Failed to allocate [" & bytes & "] bytes")
			End If

			'log.info("Allocating {} bytes at MemoryManager", bytes);

			If initialize Then
				Pointer.memset(ptr, 0, bytes)
			End If

			Return ptr
		End Function

		''' <summary>
		''' This method releases previously allocated memory chunk
		''' </summary>
		''' <param name="pointer"> </param>
		''' <param name="kind">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void release(@NonNull Pointer pointer, org.nd4j.linalg.api.memory.enums.MemoryKind kind)
		Public Overrides Sub release(ByVal pointer As Pointer, ByVal kind As MemoryKind)
			NativeOpsHolder.Instance.getDeviceNativeOps().freeHost(pointer)
			pointer.setNull()
		End Sub

		''' <summary>
		''' This method detaches off-heap memory from passed INDArray instances, and optionally stores them in cache for future reuse
		''' PLEASE NOTE: Cache options depend on specific implementations
		''' </summary>
		''' <param name="arrays"> </param>
		Public Overrides Sub collect(ParamArray ByVal arrays() As INDArray)
			MyBase.collect(arrays)
		End Sub

		''' <summary>
		''' Nd4j-native backend doesn't use periodic GC. This method will always return false.
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property PeriodicGcActive As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub memset(ByVal array As INDArray)
			If array.View Then
				array.assign(0.0)
				Return
			End If

			Pointer.memset(array.data().addressPointer(), 0, array.data().length() * Nd4j.sizeOfDataType(array.data().dataType()))
		End Sub

		Public Overrides ReadOnly Property BandwidthUse As IDictionary(Of Integer, Long)
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function allocatedMemory(ByVal deviceId As Integer?) As Long
			Return Pointer.totalBytes() + AllocationsTracker.Instance.bytesOnDevice(AllocationKind.GENERAL, deviceId) + AllocationsTracker.Instance.bytesOnDevice(AllocationKind.WORKSPACE, deviceId)
		End Function
	End Class

End Namespace