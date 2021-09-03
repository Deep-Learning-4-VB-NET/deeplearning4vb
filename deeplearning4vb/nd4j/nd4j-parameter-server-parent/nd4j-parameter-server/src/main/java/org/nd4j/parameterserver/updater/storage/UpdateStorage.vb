Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage

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

Namespace org.nd4j.parameterserver.updater.storage

	Public Interface UpdateStorage

		''' <summary>
		''' Add an ndarray to the storage </summary>
		''' <param name="array"> the array to add </param>
		Sub addUpdate(ByVal array As NDArrayMessage)

		''' <summary>
		''' The number of updates added
		''' to the update storage
		''' @return
		''' </summary>
		Function numUpdates() As Integer

		''' <summary>
		''' Clear the array storage
		''' </summary>
		Sub clear()

		''' <summary>
		''' Get the update at the specified index </summary>
		''' <param name="index"> the update to get </param>
		''' <returns> the update at the specified index </returns>
		Function getUpdate(ByVal index As Integer) As NDArrayMessage

		''' <summary>
		''' Close the database
		''' </summary>
		Sub close()

	End Interface

End Namespace