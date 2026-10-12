Imports System.Collections.Generic
Imports NDArrayHolder = org.nd4j.aeron.ipc.NDArrayHolder
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
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

Namespace org.nd4j.parameterserver.updater


	''' <summary>
	''' A parameter server updater
	''' for applying updates on the parameter server
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Interface ParameterServerUpdater

		''' <summary>
		''' Returns the number of required
		''' updates for a new pass </summary>
		''' <returns> the number of required updates for a new pass </returns>
		Function requiredUpdatesForPass() As Integer

		''' <summary>
		''' Returns true if the updater is
		''' ready for a new array
		''' @return
		''' </summary>
		ReadOnly Property Ready As Boolean

		''' <summary>
		''' Returns true if the
		''' given updater is async
		''' or synchronous
		''' updates </summary>
		''' <returns> true if the given updater
		''' is async or synchronous updates </returns>
		ReadOnly Property Async As Boolean

		''' <summary>
		''' Get the ndarray holder for this
		''' updater </summary>
		''' <returns> the ndarray holder for this updater </returns>
		Function ndArrayHolder() As NDArrayHolder

		''' <summary>
		''' Num updates passed through
		''' the updater </summary>
		''' <returns> the number of updates
		'''  </returns>
		Function numUpdates() As Integer


		''' <summary>
		''' Returns the current status of this parameter server
		''' updater
		''' @return
		''' </summary>
		Function status() As IDictionary(Of String, Number)

		''' <summary>
		''' Serialize this updater as json
		''' @return
		''' </summary>
		Function toJson() As String

		''' <summary>
		''' Reset internal counters
		''' such as number of updates accumulated.
		''' </summary>
		Sub reset()

		''' <summary>
		''' Returns true if
		''' the updater has accumulated enough ndarrays to
		''' replicate to the workers </summary>
		''' <returns> true if replication should happen,false otherwise </returns>
		Function shouldReplicate() As Boolean

		''' <summary>
		''' Do an update based on the ndarray message. </summary>
		''' <param name="message"> </param>
		Sub update(ByVal message As NDArrayMessage)

		''' <summary>
		''' Updates result
		''' based on arr along a particular
		''' <seealso cref="INDArray.tensorAlongDimension(Integer, Integer...)"/> </summary>
		''' <param name="arr"> the array to update </param>
		''' <param name="result"> the result ndarray to update </param>
		''' <param name="idx"> the index to update </param>
		''' <param name="dimensions"> the dimensions to update </param>
		Sub partialUpdate(ByVal arr As INDArray, ByVal result As INDArray, ByVal idx As Long, ParamArray ByVal dimensions() As Integer)

		''' <summary>
		''' Updates result
		''' based on arr </summary>
		''' <param name="arr"> the array to update </param>
		''' <param name="result"> the result ndarray to update </param>
		Sub update(ByVal arr As INDArray, ByVal result As INDArray)
	End Interface

End Namespace