Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AllocationsTracker = org.nd4j.linalg.api.memory.AllocationsTracker
Imports AllocationKind = org.nd4j.linalg.api.memory.enums.AllocationKind
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports org.nd4j.common.primitives
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports BaseShapeInfoProvider = org.nd4j.linalg.api.ndarray.BaseShapeInfoProvider
Imports ShapeDescriptor = org.nd4j.linalg.api.shape.ShapeDescriptor

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
'ORIGINAL LINE: @Slf4j public class DirectShapeInfoProvider extends org.nd4j.linalg.api.ndarray.BaseShapeInfoProvider
	Public Class DirectShapeInfoProvider
		Inherits BaseShapeInfoProvider

		' TODO: to be removed
		Private shapeCache As IDictionary(Of ShapeDescriptor, Pair(Of DataBuffer, Long())) = New ConcurrentDictionary(Of ShapeDescriptor, Pair(Of DataBuffer, Long()))()

		Private longCache As IDictionary(Of LongShapeDescriptor, Pair(Of DataBuffer, Long())) = New ConcurrentDictionary(Of LongShapeDescriptor, Pair(Of DataBuffer, Long()))()
		Private counter As New AtomicInteger(0)
		Private Const MAX_ENTRIES As Integer = 1000

		Public Overridable Overloads Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dataType As DataType) As Pair(Of DataBuffer, Long())
			Dim extras As Long = 0
			extras = ArrayOptionsHelper.setOptionBit(extras, dataType)
			Return createShapeInformation(shape, stride, elementWiseStride, order, extras)
		End Function

		Public Overrides Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Pair(Of DataBuffer, Long())
			' We enforce offset to 0 in shapeBuffer, since we need it for cache efficiency + we don't actually use offset value @ native side
			' We also enforce elementWiseStride = 0
			If elementWiseStride < 0 Then
				elementWiseStride = 0
			End If

			Dim descriptor As New LongShapeDescriptor(shape, stride, 0, elementWiseStride, order, extras)
			If Not longCache.ContainsKey(descriptor) Then
				If counter.get() < MAX_ENTRIES Then
					SyncLock Me
						If Not longCache.ContainsKey(descriptor) Then
							counter.incrementAndGet()
							Dim buffer As Pair(Of DataBuffer, Long()) = MyBase.createShapeInformation(shape, stride, elementWiseStride, order, extras)
							longCache(descriptor) = buffer

							bytes.addAndGet(buffer.First.length() * 8 * 2)
							AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT,0, buffer.First.length() * 8 * 2)
							Return buffer
						Else
							Return longCache(descriptor)
						End If
					End SyncLock
				Else
					Return MyBase.createShapeInformation(shape, stride, elementWiseStride, order, extras)
				End If
			End If

			Return longCache(descriptor)
		End Function

		Public Overrides Sub purgeCache()
			shapeCache = New ConcurrentDictionary(Of ShapeDescriptor, Pair(Of DataBuffer, Long()))()
		End Sub
	End Class

End Namespace