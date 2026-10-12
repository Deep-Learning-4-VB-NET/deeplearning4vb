Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports cudaStream_t = org.nd4j.jita.allocator.pointers.cuda.cudaStream_t
Imports EventsProvider = org.nd4j.jita.concurrency.EventsProvider
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

Namespace org.nd4j.jita.flow

	''' <summary>
	''' Interface describing flow controller.
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Interface FlowController

		Sub init(ByVal allocator As Allocator)

		''' <summary>
		''' This method ensures, that all asynchronous operations on referenced AllocationPoint are finished, and host memory state is up-to-date
		''' </summary>
		''' <param name="point"> </param>
		Sub synchronizeToHost(ByVal point As AllocationPoint)

		''' <summary>
		''' This method ensures, that all asynchronous operations on referenced AllocationPoint are finished, and device memory state is up-to-date
		''' </summary>
		''' <param name="point"> </param>
		Sub synchronizeToDevice(ByVal point As AllocationPoint)

		''' <summary>
		''' This method ensures, that all asynchronous operations on referenced AllocationPoint are finished </summary>
		''' <param name="point"> </param>
		Sub waitTillFinished(ByVal point As AllocationPoint)


		''' <summary>
		''' This method is called after operation was executed
		''' </summary>
		''' <param name="result"> </param>
		''' <param name="operands"> </param>
		Sub registerAction(ByVal context As CudaContext, ByVal result As INDArray, ParamArray ByVal operands() As INDArray)

		Sub registerActionAllWrite(ByVal context As CudaContext, ParamArray ByVal operands() As INDArray)

		''' <summary>
		''' This method is called before operation was executed
		''' </summary>
		''' <param name="result"> </param>
		''' <param name="operands"> </param>
		Function prepareAction(ByVal result As INDArray, ParamArray ByVal operands() As INDArray) As CudaContext

		''' 
		''' 
		''' <param name="operands">
		''' @return </param>
		Function prepareActionAllWrite(ParamArray ByVal operands() As INDArray) As CudaContext

		Function prepareAction(ByVal result As AllocationPoint, ParamArray ByVal operands() As AllocationPoint) As CudaContext

		Sub registerAction(ByVal context As CudaContext, ByVal result As AllocationPoint, ParamArray ByVal operands() As AllocationPoint)

		Sub waitTillReleased(ByVal point As AllocationPoint)

		''' <summary>
		''' This method should be called after memcpy operations, to control their flow.
		''' </summary>
		Sub commitTransfer(ByVal streamUsed As cudaStream_t)

		ReadOnly Property EventsProvider As EventsProvider
	End Interface

End Namespace