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

Namespace org.nd4j.aeron.ipc

	''' <summary>
	''' An ndarray listener
	''' @author Adam Gibson
	''' </summary>
	Public Interface NDArrayCallback


		''' <summary>
		''' A listener for ndarray message </summary>
		''' <param name="message"> the message for the callback </param>
		Sub onNDArrayMessage(ByVal message As NDArrayMessage)

		''' <summary>
		''' Used for partial updates using tensor along
		''' dimension </summary>
		''' <param name="arr"> the array to count as an update </param>
		''' <param name="idx"> the index for the tensor along dimension </param>
		''' <param name="dimensions"> the dimensions to act on for the tensor along dimension </param>
		Sub onNDArrayPartial(ByVal arr As INDArray, ByVal idx As Long, ParamArray ByVal dimensions() As Integer)

		''' <summary>
		''' Setup an ndarray </summary>
		''' <param name="arr"> </param>
		Sub onNDArray(ByVal arr As INDArray)

	End Interface

End Namespace