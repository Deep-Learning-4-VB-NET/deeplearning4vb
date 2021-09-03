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

Namespace org.nd4j.linalg.compression

	Public Interface AbstractStorage(Of T As Object)

		''' <summary>
		''' Store object into storage
		''' </summary>
		''' <param name="key"> </param>
		''' <param name="object"> </param>
		Sub store(ByVal key As T, ByVal [object] As INDArray)

		''' <summary>
		''' Store object into storage
		''' </summary>
		''' <param name="key"> </param>
		''' <param name="array"> </param>
		Sub store(ByVal key As T, ByVal array() As Single)

		''' <summary>
		''' Store object into storage
		''' </summary>
		''' <param name="key"> </param>
		''' <param name="array"> </param>
		Sub store(ByVal key As T, ByVal array() As Double)

		''' <summary>
		''' Store object into storage, if it doesn't exist </summary>
		'''  <param name="key"> </param>
		''' <param name="object"> </param>
		Function storeIfAbsent(ByVal key As T, ByVal [object] As INDArray) As Boolean

		''' <summary>
		''' Get object from the storage, by key
		''' </summary>
		''' <param name="key"> </param>
		Function get(ByVal key As T) As INDArray

		''' <summary>
		''' This method checks, if storage contains specified key
		''' </summary>
		''' <param name="key">
		''' @return </param>
		Function containsKey(ByVal key As T) As Boolean

		''' <summary>
		''' This method purges everything from storage
		''' </summary>
		Sub clear()


		''' <summary>
		''' This method removes value by specified key
		''' </summary>
		Sub drop(ByVal key As T)

		''' <summary>
		''' This method returns number of entries available in storage
		''' </summary>
		Function size() As Long
	End Interface

End Namespace