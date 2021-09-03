Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = org.nd4j.linalg.api.shape.options.ArrayType
Imports org.nd4j.common.primitives
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports BaseShapeInfoProvider = org.nd4j.linalg.api.ndarray.BaseShapeInfoProvider

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

Namespace org.nd4j.jita.constant


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ProtectedCudaShapeInfoProvider extends org.nd4j.linalg.api.ndarray.BaseShapeInfoProvider
	Public Class ProtectedCudaShapeInfoProvider
		Inherits BaseShapeInfoProvider

		Private allocator As AtomicAllocator

		Private cacheHit As New AtomicLong(1)
		Private cacheMiss As New AtomicLong(1)

		Private lock As New Semaphore(1)

		Protected Friend Shared ReadOnly protector As ConstantProtector = ConstantProtector.Instance

		Private Shared ourInstance As New ProtectedCudaShapeInfoProvider()


		Private Sub New()

		End Sub

		''' <summary>
		''' This method forces cache purge, if cache is available for specific implementation
		''' </summary>
		Public Overrides Sub purgeCache()
			protector.purgeProtector()
		End Sub

		Public Shared ReadOnly Property Instance As ProtectedCudaShapeInfoProvider
			Get
				Return ourInstance
			End Get
		End Property


		Public Overrides Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal type As DataType, ByVal empty As Boolean) As Pair(Of DataBuffer, Long())
			Dim extras As Long = ArrayOptionsHelper.setOptionBit(0L, type)
			If empty Then
				extras = ArrayOptionsHelper.setOptionBit(extras, ArrayType.EMPTY)
			End If

			Return createShapeInformation(shape, stride, elementWiseStride, order, extras)
		End Function

		Public Overrides Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Pair(Of DataBuffer, Long())
			' We enforce offset to 0 in shapeBuffer, since we need it for cache efficiency + we don't actually use offset value @ native side
			Dim offset As Long = 0
			If elementWiseStride < 0 Then
				elementWiseStride = 0
			End If

			Dim deviceId As Integer? = AtomicAllocator.Instance.getDeviceId()

			Dim descriptor As New LongShapeDescriptor(shape, stride, offset, elementWiseStride, order, extras)

			If Not protector.containsDataBuffer(deviceId, descriptor) Then
				Dim buffer As Pair(Of DataBuffer, Long()) = Nothing
				SyncLock Me
					If Not protector.containsDataBuffer(deviceId, descriptor) Then
						'log.info("Cache miss: {}", descriptor);
						buffer = MyBase.createShapeInformation(shape, stride, elementWiseStride, order, extras)
						buffer.First.Constant = True

	'                    
	'                    // constant space is allocated at cpp level
	'                    if (CudaEnvironment.getInstance().getConfiguration().getMemoryModel() == Configuration.MemoryModel.IMMEDIATE) {
	'                        Nd4j.getConstantHandler().moveToConstantSpace(buffer.getFirst());
	'                    }
	'                     

						'deviceCache.get(deviceId).put(descriptor, buffer);
						protector.persistDataBuffer(deviceId, descriptor, buffer)

						bytes.addAndGet(buffer.First.length() * 8 * 2)

						cacheMiss.incrementAndGet()
					Else
						buffer = protector.getDataBuffer(deviceId, descriptor)
					End If
				End SyncLock
				Return buffer
			Else
				'       log.info("Cache hit: {}", descriptor);
				cacheHit.incrementAndGet()
			End If

			Return protector.getDataBuffer(deviceId, descriptor) 'deviceCache.get(deviceId).get(descriptor);
		End Function
	End Class

End Namespace