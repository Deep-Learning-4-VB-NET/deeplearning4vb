Imports System.Collections.Generic
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.linalg.api.memory


	Public Interface MemoryManager

		Property CurrentWorkspace As MemoryWorkspace


		''' <summary>
		''' PLEASE NOTE: This method is under development yet. Do not use it.
		''' </summary>
		Sub notifyScopeEntered()

		''' <summary>
		''' PLEASE NOTE: This method is under development yet. Do not use it.
		''' </summary>
		Sub notifyScopeLeft()

		''' <summary>
		''' This method calls for GC, and if frequency is met - System.gc() will be called
		''' </summary>
		Sub invokeGcOccasionally()

		''' <summary>
		''' This method calls for GC.
		''' </summary>
		Sub invokeGc()

		''' <summary>
		''' This method enables/disables periodic GC
		''' </summary>
		''' <param name="enabled"> </param>
		Sub togglePeriodicGc(ByVal enabled As Boolean)

		''' <summary>
		''' This method enables/disables calculation of average time spent within loops
		''' 
		''' Default: false
		''' </summary>
		''' <param name="enabled"> </param>
		Sub toggleAveraging(ByVal enabled As Boolean)

		''' <summary>
		''' This method returns true, if periodic GC is active. False otherwise.
		''' 
		''' @return
		''' </summary>
		ReadOnly Property PeriodicGcActive As Boolean

		''' <summary>
		''' This method returns time (in milliseconds) of the las System.gc() call
		''' 
		''' @return
		''' </summary>
		ReadOnly Property LastGcTime As Long

		''' <summary>
		''' Sets manual GC invocation frequency. If you set it to 5, only 1/5 of calls will result in GC invocation
		''' If 0 is used as frequency, it'll disable all manual invocation hooks.
		''' 
		''' default value: 5 </summary>
		''' <param name="frequency"> </param>
		Property OccasionalGcFrequency As Integer


		''' <summary>
		''' This method returns average time between invokeGCOccasionally() calls
		''' @return
		''' </summary>
		ReadOnly Property AverageLoopTime As Integer

		''' <summary>
		''' This method enables/disables periodic System.gc() calls.
		''' Set to 0 to disable this option.
		''' </summary>
		''' <param name="windowMillis"> minimal time milliseconds between calls. </param>
		Property AutoGcWindow As Integer


		''' <summary>
		''' This method returns pointer to allocated memory
		''' 
		''' PLEASE NOTE: Cache options depend on specific implementations
		''' </summary>
		''' <param name="bytes"> </param>
		Function allocate(ByVal bytes As Long, ByVal kind As MemoryKind, ByVal initialize As Boolean) As Pointer


		''' <summary>
		''' This method releases previously allocated memory chunk
		''' </summary>
		''' <param name="pointer"> </param>
		''' <param name="kind">
		''' @return </param>
		Sub release(ByVal pointer As Pointer, ByVal kind As MemoryKind)

		''' <summary>
		''' This method detaches off-heap memory from passed INDArray instances, and optionally stores them in cache for future reuse
		''' PLEASE NOTE: Cache options depend on specific implementations
		''' </summary>
		''' <param name="arrays"> </param>
		Sub collect(ParamArray ByVal arrays() As INDArray)


		''' <summary>
		''' This method purges all cached memory chunks
		''' 
		''' </summary>
		Sub purgeCaches()

		''' <summary>
		''' This method does memcpy  from source buffer to destination buffer
		''' 
		''' PLEASE NOTE: This method is NOT safe.
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcBuffer"> </param>
		Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer)


		''' <summary>
		''' This method fills given INDArray with zeroes.
		''' 
		''' PLEASE NOTE: Can't be efficiently used on views, .assign(0.0) will be used instead
		''' </summary>
		''' <param name="array"> </param>
		Sub memset(ByVal array As INDArray)

		''' <summary>
		''' This method temporary opens block out of any workspace scope.
		''' 
		''' PLEASE NOTE: Do not forget to close this block.
		''' 
		''' @return
		''' </summary>
		Function scopeOutOfWorkspaces() As MemoryWorkspace

		''' <summary>
		'''  This method returns per-device bandwidth use for memory transfers
		''' </summary>
		ReadOnly Property BandwidthUse As IDictionary(Of Integer, Long)

		''' <summary>
		''' This method returns number of bytes allocated on specified device </summary>
		''' <param name="deviceId">
		''' @return </param>
		Function allocatedMemory(ByVal deviceId As Integer?) As Long

		''' <summary>
		''' This method releases Context (if current backend has one, sure)
		''' </summary>
		Sub releaseCurrentContext()
	End Interface

End Namespace