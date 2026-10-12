Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports PointersPair = org.nd4j.jita.allocator.pointers.PointersPair

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

Namespace org.nd4j.jita.memory

	''' <summary>
	''' This interface describes 2 basic methods to work with memory: malloc and free.
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Interface MemoryProvider
		''' <summary>
		''' This method provides PointersPair to memory chunk specified by AllocationShape
		''' </summary>
		''' <param name="shape"> shape of desired memory chunk </param>
		''' <param name="point"> target AllocationPoint structure </param>
		''' <param name="location"> either HOST or DEVICE
		''' @return </param>
		Function malloc(ByVal shape As AllocationShape, ByVal point As AllocationPoint, ByVal location As AllocationStatus) As PointersPair

		''' <summary>
		''' This method frees specific chunk of memory, described by AllocationPoint passed in
		''' </summary>
		''' <param name="point"> </param>
		Sub free(ByVal point As AllocationPoint)

		''' <summary>
		''' This method checks specified device for specified amount of memory
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="requiredMemory">
		''' @return </param>
		Function pingDeviceForFreeMemory(ByVal deviceId As Integer?, ByVal requiredMemory As Long) As Boolean


		Sub purgeCache()
	End Interface

End Namespace