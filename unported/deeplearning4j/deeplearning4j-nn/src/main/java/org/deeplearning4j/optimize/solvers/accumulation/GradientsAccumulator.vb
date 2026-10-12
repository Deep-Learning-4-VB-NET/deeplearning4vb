Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
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

Namespace org.deeplearning4j.optimize.solvers.accumulation


	Public Interface GradientsAccumulator

		''' <summary>
		''' This method allows to pass external updates to accumulator, they will be populated across all workers using this GradientsAccumulator instance
		''' </summary>
		''' <param name="source"> </param>
		Property ExternalSource As IndexedTail



		''' <summary>
		''' This method applies accumulated updates via given StepFunction
		''' </summary>
		''' <param name="function"> </param>
		''' <param name="params"> </param>
		Sub applyUpdate(ByVal [function] As StepFunction, ByVal params As INDArray, ByVal updates As INDArray, ByVal isFinalStep As Boolean)

		''' <summary>
		''' This method applies accumulated updates via given StepFunction
		''' </summary>
		''' <param name="function"> </param>
		''' <param name="params"> </param>
		Sub applyUpdate(ByVal [function] As StepFunction, ByVal params As INDArray, ByVal updates As INDArray, ByVal alpha As Double)

		''' <summary>
		''' This method accepts updates suitable for StepFunction, and accumulates/propagates it across all workers
		''' </summary>
		''' <param name="array"> </param>
		Sub storeUpdate(ByVal array As INDArray, ByVal iterationNumber As Integer, ByVal epochNumber As Integer)

		''' <summary>
		''' This method accepts updates suitable for StepFunction and puts them to the queue, which is used in backpropagation loop
		''' 
		''' PLEASE NOTE: array is expected to be ready for use and match params dimensionality
		''' </summary>
		''' <param name="array"> </param>
		Sub receiveUpdate(ByVal array As INDArray)

		''' <summary>
		''' This method allows to highlight early availability of updates
		''' </summary>
		''' <param name="updatesAvailable"> </param>
		Sub markExternalUpdates(ByVal updatesAvailable As Boolean)

		''' <summary>
		''' This method resets all accumulated updates (if any)
		''' </summary>
		Sub reset()

		''' <summary>
		''' This method does initialization of given worker wrt Thread-Device Affinity
		''' </summary>
		Sub touch()

		''' <summary>
		''' This method checks if there are any (probably external) updates available
		''' @return
		''' </summary>
		Function hasAnything() As Boolean
	End Interface

End Namespace