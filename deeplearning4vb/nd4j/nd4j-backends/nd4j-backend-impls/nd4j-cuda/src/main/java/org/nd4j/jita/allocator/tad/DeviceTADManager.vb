Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.nd4j.common.primitives
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports TadDescriptor = org.nd4j.linalg.cache.TadDescriptor
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

Namespace org.nd4j.jita.allocator.tad


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DeviceTADManager extends BasicTADManager
	Public Class DeviceTADManager
		Inherits BasicTADManager

		Protected Friend tadCache As IList(Of IDictionary(Of TadDescriptor, Pair(Of DataBuffer, DataBuffer))) = New List(Of IDictionary(Of TadDescriptor, Pair(Of DataBuffer, DataBuffer)))()
		Private lock As New Semaphore(1)

		Public Sub New()
			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices

			For i As Integer = 0 To numDevices - 1
				tadCache.Insert(i, New ConcurrentDictionary(Of TadDescriptor, Pair(Of DataBuffer, DataBuffer))())
			Next i
		End Sub

		''' <summary>
		''' This method removes all cached shape buffers
		''' </summary>
		Public Overrides Sub purgeBuffers()
			log.info("Purging TAD buffers...")

			tadCache = New List(Of IDictionary(Of TadDescriptor, Pair(Of DataBuffer, DataBuffer)))()

			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices

			For i As Integer = 0 To numDevices - 1
				log.info("Resetting device: [{}]", i)
				tadCache.Insert(i, New ConcurrentDictionary(Of TadDescriptor, Pair(Of DataBuffer, DataBuffer))())
			Next i

			MyBase.purgeBuffers()
		End Sub

		Public Overrides Function getTADOnlyShapeInfo(ByVal array As INDArray, ByVal dimension() As Integer) As Pair(Of DataBuffer, DataBuffer)
	'        
	'            so, we check, if we have things cached.
	'            If we don't - we just create new TAD shape, and push it to constant memory
	'        
			If dimension IsNot Nothing AndAlso dimension.Length > 1 Then
				Array.Sort(dimension)
			End If

			Dim deviceId As Integer? = AtomicAllocator.Instance.getDeviceId()

			'log.info("Requested TAD for device [{}], dimensions: [{}]", deviceId, Arrays.toString(dimension));

			'extract the dimensions and shape buffer for comparison
			Dim descriptor As New TadDescriptor(array, dimension)

			If Not tadCache(deviceId).ContainsKey(descriptor) Then
				log.trace("Creating new TAD...")
				'create the TAD with the shape information and corresponding offsets
				'note that we use native code to get access to the shape information.
				Dim buffers As Pair(Of DataBuffer, DataBuffer) = MyBase.getTADOnlyShapeInfo(array, dimension)
				''' <summary>
				''' Store the buffers in constant memory.
				''' The main implementation of this is cuda right now.
				''' 
				''' Explanation from: http://cuda-programming.blogspot.jp/2013/01/what-is-constant-memory-in-cuda.html
				''' The CUDA language makes available another kind of memory known as constant memory. As the opName may indicate, we use constant memory for data that will not change over the course of a kernel execution.
				''' 
				''' Why Constant Memory?
				''' 
				''' NVIDIA hardware provides 64KB of constant memory that
				''' it treats differently than it treats standard global memory. In some situations,
				''' using constant memory rather than global memory will reduce the required memory bandwidth.
				''' 
				''' NOTE HERE FOR US: We use 48kb of it using these methods.
				''' 
				''' Note also that we use the <seealso cref="AtomicAllocator"/> which is the cuda memory manager
				''' for moving the current host space data buffer to constant memory.
				''' 
				''' We do this for device access to shape information.
				''' </summary>
				'if (buffers.getFirst() != array.shapeInfoDataBuffer())
					'AtomicAllocator.getInstance().moveToConstant(buffers.getFirst());
				''' <seealso cref= <seealso cref="org.nd4j.jita.constant.ProtectedCudaConstantHandler"/> </seealso>
				'if (buffers.getSecond() != null)
			   '     AtomicAllocator.getInstance().moveToConstant(buffers.getSecond());

				' so, at this point we have buffer valid on host side.
				' And we just need to replace DevicePointer with constant pointer
				tadCache(deviceId)(descriptor) = buffers

				bytes.addAndGet((buffers.First.length() * 4))

				If buffers.Second IsNot Nothing Then
					bytes.addAndGet(buffers.Second.length() * 8)
				End If

				log.trace("Using TAD from cache...")
			End If

			Return tadCache(deviceId)(descriptor)
		End Function
	End Class

End Namespace