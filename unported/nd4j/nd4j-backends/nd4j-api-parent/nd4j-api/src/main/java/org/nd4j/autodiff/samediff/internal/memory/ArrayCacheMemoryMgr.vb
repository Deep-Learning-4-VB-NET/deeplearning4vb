Imports System
Imports System.Collections.Generic
Imports lombok
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.autodiff.samediff.internal.memory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class ArrayCacheMemoryMgr extends AbstractMemoryMgr
	Public Class ArrayCacheMemoryMgr
		Inherits AbstractMemoryMgr

		Private ReadOnly maxMemFrac As Double
		Private ReadOnly smallArrayThreshold As Long
		Private ReadOnly largerArrayMaxMultiple As Double

		Private ReadOnly maxCacheBytes As Long
		Private ReadOnly totalMemBytes As Long

		Private currentCacheSize As Long = 0
		Private arrayStores As IDictionary(Of DataType, ArrayStore) = New Dictionary(Of DataType, ArrayStore)()

		Private lruCache As New LinkedHashSet(Of Long)()
		Private lruCacheValues As IDictionary(Of Long, INDArray) = New Dictionary(Of Long, INDArray)()

		''' <summary>
		''' Create an ArrayCacheMemoryMgr with default settings as per <seealso cref="ArrayCacheMemoryMgr"/>
		''' </summary>
		Public Sub New()
			Me.New(0.25, 1024, 2.0)
		End Sub

		''' <param name="maxMemFrac">             Maximum memory fraciton to use as cache </param>
		''' <param name="smallArrayThreshold">    Below this size (elements), don't apply the "largerArrayMaxMultiple" rule </param>
		''' <param name="largerArrayMaxMultiple"> Maximum multiple of the requested size to return from the cache. If an array of size
		'''                               1024 is requested, and largerArrayMaxMultiple is 2.0, then we'll return from the cache
		'''                               the array with the smallest data buffer up to 2.0*1024 elements; otherwise we'll return
		'''                               a new array </param>
		Public Sub New(ByVal maxMemFrac As Double, ByVal smallArrayThreshold As Long, ByVal largerArrayMaxMultiple As Double)
			Preconditions.checkArgument(maxMemFrac > 0 AndAlso maxMemFrac < 1, "Maximum memory fraction for cache must be between 0.0 and 1.0, got %s", maxMemFrac)
			Preconditions.checkArgument(smallArrayThreshold >= 0, "Small array threshold must be >= 0, got %s", smallArrayThreshold)
			Preconditions.checkArgument(largerArrayMaxMultiple >= 1.0, "Larger array max multiple must be >= 1.0, got %s", largerArrayMaxMultiple)
			Me.maxMemFrac = maxMemFrac
			Me.smallArrayThreshold = smallArrayThreshold
			Me.largerArrayMaxMultiple = largerArrayMaxMultiple

			If Cpu Then
				totalMemBytes = Pointer.maxBytes()
			Else
				Dim p As Properties = Nd4j.Executioner.EnvironmentInformation
				Dim devList As System.Collections.IList = CType(p.get("cuda.devicesInformation"), System.Collections.IList)
				Dim m As System.Collections.IDictionary = CType(devList(0), System.Collections.IDictionary)
				totalMemBytes = CType(m("cuda.totalMemory"), Long?)
			End If
			maxCacheBytes = CLng(Math.Truncate(maxMemFrac * totalMemBytes))
		End Sub

		Private ReadOnly Property Cpu As Boolean
			Get
				Dim backend As String = Nd4j.Executioner.EnvironmentInformation.getProperty("backend")
				Return Not "CUDA".Equals(backend, StringComparison.OrdinalIgnoreCase)
			End Get
		End Property

		Public Overrides Function allocate(ByVal detached As Boolean, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray
			If arrayStores.ContainsKey(dataType) Then
				Dim arr As INDArray = arrayStores(dataType).get(shape)
				If arr IsNot Nothing Then
					'Decrement cache size
					currentCacheSize -= dataType.width() * arr.data().length()

					Return arr 'Allocated from cache
				End If
			End If

			'Allocation failed, allocate new array
			Return Nd4j.createUninitializedDetached(dataType, shape)
		End Function

		Public Overrides Function allocate(ByVal detached As Boolean, ByVal descriptor As LongShapeDescriptor) As INDArray
			If descriptor.Empty Then
				Dim ret As INDArray = Nd4j.create(descriptor)
				If detached Then
					ret = ret.detach()
				End If

				Return ret
			End If

			Dim dataType As DataType = descriptor.dataType()
			Dim shape() As Long = descriptor.getShape()
			If arrayStores.ContainsKey(dataType) Then
				Dim arr As INDArray = arrayStores(dataType).get(shape)
				If arr IsNot Nothing AndAlso arr.ordering() <> descriptor.getOrder() Then
					arr.Order = descriptor.getOrder()
				End If


				If arr IsNot Nothing Then
					'Decrement cache size
					currentCacheSize -= dataType.width() * arr.data().length()

					Return arr 'Allocated from cache
				End If
			End If

			'Allocation failed, allocate new array
			Return Nd4j.createUninitializedDetached(dataType, shape)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void release(@NonNull INDArray array)
		Public Overridable Overloads Sub release(ByVal array As INDArray)
			'Check for multiple releases of the array
			Dim id As Long = array.getId()
			Preconditions.checkState(Not lruCache.contains(id), "Array was released multiple times: id=%s, shape=%ndShape", id, array)


			Dim dt As DataType = array.dataType()
			If array.data() Is Nothing AndAlso array.closeable() Then
				array.close()
				Return
			End If

			Dim thisBytes As Long = array.data().length() * dt.width()
			If array.dataType() = DataType.UTF8 Then
				'Don't cache string arrays due to variable length buffers
				If array.closeable() Then
					array.close()
				End If
			ElseIf currentCacheSize + thisBytes > maxCacheBytes Then
				If thisBytes > maxCacheBytes Then
					'Can't store even if we clear everything - too large
					If array.closeable() Then
						array.close()
					End If
					Return
				End If

				'Need to deallocate some arrays to stay under limit - do in "oldest first" order
				Dim iter As IEnumerator(Of Long) = lruCache.GetEnumerator()
				Do While currentCacheSize + thisBytes > maxCacheBytes
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim [next] As Long = iter.next()
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
					iter.remove()
					Dim nextOldest As INDArray = lruCacheValues.Remove([next])
					Dim ndt As DataType = nextOldest.dataType()
					Dim nextBytes As Long = ndt.width() * nextOldest.data().length()
					arrayStores(ndt).removeObject(nextOldest)
					currentCacheSize -= nextBytes

					If nextOldest.closeable() Then
						nextOldest.close()
					End If
				Loop

				'After clearing space - can now cache
				cacheArray(array)
			Else
				'OK to cache
				cacheArray(array)
			End If

			'Store in LRU cache for "last used" removal if we exceed cache size
			lruCache.add(array.getId())
			lruCacheValues(array.getId()) = array
		End Sub

		Private Sub cacheArray(ByVal array As INDArray)
			Dim dt As DataType = array.dataType()
			If Not arrayStores.ContainsKey(dt) Then
				arrayStores(dt) = New ArrayStore(Me)
			End If
			arrayStores(dt).add(array)
			currentCacheSize += array.data().length() * dt.width()

			lruCache.add(array.Id)
			lruCacheValues(array.Id) = array
		End Sub

		Public Overrides Sub Dispose()
			For Each [as] As ArrayStore In arrayStores.Values
				[as].close()
			Next [as]
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class ArrayStore
		Public Class ArrayStore
			Private ReadOnly outerInstance As ArrayCacheMemoryMgr

			Public Sub New(ByVal outerInstance As ArrayCacheMemoryMgr)
				Me.outerInstance = outerInstance
			End Sub

			Friend sorted(999) As INDArray 'TODO resizing, don't hardcode
			Friend lengths(999) As Long
			Friend lengthSum As Long
			Friend bytesSum As Long
			Friend size As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private void add(@NonNull INDArray array)
			Friend Overridable Sub add(ByVal array As INDArray)
				'Resize arrays
				If size = sorted.Length Then
					sorted = Arrays.CopyOf(sorted, 2*sorted.Length)
					lengths = Arrays.CopyOf(lengths, 2*lengths.Length)
				End If

				Dim length As Long = array.data().length()
				Dim idx As Integer = Array.BinarySearch(lengths, 0, size, length)
				If idx < 0 Then
					idx = -idx - 1 'See binarySearch javadoc
				End If
				For i As Integer = size - 1 To idx Step -1
					sorted(i + 1) = sorted(i)
					lengths(i + 1) = lengths(i)
				Next i
				sorted(idx) = array
				lengths(idx) = length
				size += 1
				lengthSum += length
				bytesSum += length * array.dataType().width()
			End Sub

			Friend Overridable Function get(ByVal shape() As Long) As INDArray
				If size = 0 Then
					Return Nothing
				End If

				Dim length As Long = If(shape.Length = 0, 1, ArrayUtil.prod(shape))

				Dim idx As Integer = Array.BinarySearch(lengths, 0, size, length)
				If idx < 0 Then
					idx = -idx - 1
					If idx >= size Then
						'Largest array is smaller than required -> can't return from cache
						Return Nothing
					End If
					Dim nextSmallest As INDArray = sorted(idx)
					Dim nextSmallestLength As Long = nextSmallest.data().length()
					Dim nextSmallestLengthBytes As Long = nextSmallestLength * nextSmallest.dataType().width()

					Dim tooLarge As Boolean = (length > CLng(Math.Truncate(nextSmallestLength * outerInstance.largerArrayMaxMultiple)))

					If nextSmallestLengthBytes > outerInstance.smallArrayThreshold AndAlso tooLarge Then
						Return Nothing
					End If ' If less than smallArrayThreshold, ok, return as is
				End If

				'Remove
				Dim arr As INDArray = removeIdx(idx)

				outerInstance.lruCache.remove(arr.Id)
				outerInstance.lruCacheValues.Remove(arr.Id)

				'Create a new array with the specified buffer. This is for 2 reasons:
				'(a) the cached array and requested array sizes may differ (though this is easy to check for)
				'(b) Some SameDiff array use tracking uses *object identity* - so we want different objects when reusing arrays
				'    to avoid issues there
				Return Nd4j.create(arr.data(), shape)
			End Function

			Friend Overridable Sub removeObject(ByVal array As INDArray)
				Dim length As Long = array.data().length()
				Dim idx As Integer = Array.BinarySearch(lengths, 0, size, length)
				Preconditions.checkState(idx >= 0, "Cannot remove array from ArrayStore: no array with this length exists in the cache")
				Dim found As Boolean = False
				Dim i As Integer = 0
				Do While Not found AndAlso i < size
					found = sorted(i) Is array AndAlso lengths(i) = length 'Object and length equality
					i += 1
				Loop
				Preconditions.checkState(found, "Cannot remove array: not found in ArrayCache")
				removeIdx(i - 1)
			End Sub

			Friend Overridable Function removeIdx(ByVal idx As Integer) As INDArray
				Dim arr As INDArray = sorted(idx)
				For i As Integer = idx To size - 1
					sorted(i) = sorted(i + 1)
					lengths(i) = lengths(i + 1)
				Next i
				sorted(size) = Nothing
				lengths(size) = 0
				size -= 1

				bytesSum -= (arr.data().length() * arr.dataType().width())
				lengthSum -= arr.data().length()

				Return arr
			End Function

			Friend Overridable Sub close()
				For i As Integer = 0 To size - 1
					If sorted(i).closeable() Then
						sorted(i).close()
					End If
					lengths(i) = 0
				Next i
				lengthSum = 0
				bytesSum = 0
				size = 0
			End Sub
		End Class
	End Class

End Namespace