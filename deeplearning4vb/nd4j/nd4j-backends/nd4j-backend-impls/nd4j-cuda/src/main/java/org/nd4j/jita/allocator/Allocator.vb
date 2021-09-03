Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports Configuration = org.nd4j.jita.conf.Configuration
Imports FlowController = org.nd4j.jita.flow.FlowController
Imports MemoryHandler = org.nd4j.jita.handler.MemoryHandler
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

Namespace org.nd4j.jita.allocator

	''' 
	''' <summary>
	''' Allocator interface provides methods for transparent memory management
	''' 
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Interface Allocator

		''' <summary>
		''' Consume and apply configuration passed in as argument
		''' </summary>
		''' <param name="configuration"> configuration bean to be applied </param>
		Sub applyConfiguration(ByVal configuration As Configuration)

		''' <summary>
		''' This method returns CudaContext for current thread
		''' 
		''' @return
		''' </summary>
		ReadOnly Property DeviceContext As CudaContext

		''' <summary>
		''' This methods specifies Mover implementation to be used internally
		''' </summary>
		''' <param name="memoryHandler"> </param>
		Property MemoryHandler As MemoryHandler

		''' <summary>
		''' Returns current Allocator configuration
		''' </summary>
		''' <returns> current configuration </returns>
		ReadOnly Property Configuration As Configuration

		''' <summary>
		''' This method returns actual device pointer valid for current object
		''' </summary>
		''' <param name="buffer"> </param>
		Function getPointer(ByVal buffer As DataBuffer, ByVal context As CudaContext) As Pointer

		''' <summary>
		''' This method returns actual host pointer valid for current object
		''' </summary>
		''' <param name="buffer"> </param>
		Function getHostPointer(ByVal buffer As DataBuffer) As Pointer

		''' <summary>
		''' This method returns actual host pointer valid for current object
		''' </summary>
		''' <param name="array"> </param>
		Function getHostPointer(ByVal array As INDArray) As Pointer

		''' <summary>
		''' This method returns actual device pointer valid for specified shape of current object
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		Function getPointer(ByVal buffer As DataBuffer, ByVal shape As AllocationShape, ByVal isView As Boolean, ByVal context As CudaContext) As Pointer


		''' <summary>
		''' This method returns actual device pointer valid for specified INDArray
		''' </summary>
		Function getPointer(ByVal array As INDArray, ByVal context As CudaContext) As Pointer


		''' <summary>
		''' This method should be callsd to make sure that data on host side is actualized
		''' </summary>
		''' <param name="array"> </param>
		Sub synchronizeHostData(ByVal array As INDArray)

		''' <summary>
		''' This method should be calls to make sure that data on host side is actualized
		''' </summary>
		''' <param name="buffer"> </param>
		Sub synchronizeHostData(ByVal buffer As DataBuffer)

		''' <summary>
		''' This method returns deviceId for current thread
		''' All values >= 0 are considered valid device IDs, all values < 0 are considered stubs.
		''' 
		''' @return
		''' </summary>
		ReadOnly Property DeviceId As Integer?

		''' <summary>
		''' Returns <seealso cref="getDeviceId()"/> wrapped as a <seealso cref="Pointer"/>. </summary>
		ReadOnly Property DeviceIdPointer As Pointer

		''' <summary>
		'''  This method allocates required chunk of memory
		''' </summary>
		''' <param name="requiredMemory"> </param>
		Function allocateMemory(ByVal buffer As DataBuffer, ByVal requiredMemory As AllocationShape, ByVal initialize As Boolean) As AllocationPoint

		''' <summary>
		''' This method allocates required chunk of memory in specific location
		''' 
		''' PLEASE NOTE: Do not use this method, unless you're 100% sure what you're doing
		''' </summary>
		''' <param name="requiredMemory"> </param>
		''' <param name="location"> </param>
		Function allocateMemory(ByVal buffer As DataBuffer, ByVal requiredMemory As AllocationShape, ByVal location As AllocationStatus, ByVal initialize As Boolean) As AllocationPoint


		Sub memcpyBlocking(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long)

		Sub memcpyAsync(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long)

		Sub memcpySpecial(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long)

		Sub memcpyDevice(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long, ByVal context As CudaContext)

		Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer)

		Sub tickHostWrite(ByVal buffer As DataBuffer)

		Sub tickHostWrite(ByVal array As INDArray)

		Sub tickDeviceWrite(ByVal array As INDArray)

		Function getAllocationPoint(ByVal array As INDArray) As AllocationPoint

		Function getAllocationPoint(ByVal buffer As DataBuffer) As AllocationPoint

		Sub registerAction(ByVal context As CudaContext, ByVal result As INDArray, ParamArray ByVal operands() As INDArray)

		ReadOnly Property FlowController As FlowController

		Function getConstantBuffer(ByVal array() As Integer) As DataBuffer

		Function getConstantBuffer(ByVal array() As Single) As DataBuffer

		Function getConstantBuffer(ByVal array() As Double) As DataBuffer

		Function moveToConstant(ByVal dataBuffer As DataBuffer) As DataBuffer

	End Interface

End Namespace