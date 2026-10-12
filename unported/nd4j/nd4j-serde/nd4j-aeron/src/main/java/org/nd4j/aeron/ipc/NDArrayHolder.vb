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


	Public Interface NDArrayHolder


		''' <summary>
		''' Set the ndarray </summary>
		''' <param name="arr"> the ndarray for this holder
		'''            to use </param>
		WriteOnly Property Array As INDArray


		''' <summary>
		''' The number of updates
		''' that have been sent to this older.
		''' @return
		''' </summary>
		Function totalUpdates() As Integer

		''' <summary>
		''' Retrieve an ndarray
		''' @return
		''' </summary>
		Function get() As INDArray

		''' <summary>
		''' Retrieve a partial view of the ndarray.
		''' This method uses tensor along dimension internally
		''' Note this will call dup() </summary>
		''' <param name="idx"> the index of the tad to get </param>
		''' <param name="dimensions"> the dimensions to use </param>
		''' <returns> the tensor along dimension based on the index and dimensions
		''' from the master array. </returns>
		Function getTad(ByVal idx As Integer, ParamArray ByVal dimensions() As Integer) As INDArray
	End Interface

End Namespace