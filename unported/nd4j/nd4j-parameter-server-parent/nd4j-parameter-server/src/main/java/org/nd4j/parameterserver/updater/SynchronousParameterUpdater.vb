Imports System
Imports System.Collections.Generic
Imports NDArrayHolder = org.nd4j.aeron.ipc.NDArrayHolder
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports UpdateStorage = org.nd4j.parameterserver.updater.storage.UpdateStorage
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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


	Public Class SynchronousParameterUpdater
		Inherits BaseParameterUpdater

		Private workers As Integer = Runtime.getRuntime().availableProcessors()
		Private Shared objectMapper As New ObjectMapper()

		''' <summary>
		''' Returns the number of required
		''' updates for a new pass
		''' </summary>
		''' <returns> the number of required updates for a new pass </returns>
		Public Overrides Function requiredUpdatesForPass() As Integer
			Return workers
		End Function

		''' <summary>
		''' Returns true if the
		''' given updater is async
		''' or synchronous
		''' updates
		''' </summary>
		''' <returns> true if the given updater
		''' is async or synchronous updates </returns>
		Public Overrides ReadOnly Property Async As Boolean
			Get
				Return False
			End Get
		End Property

		''' 
		''' <param name="updateStorage"> </param>
		''' <param name="ndArrayHolder"> </param>
		''' <param name="workers"> </param>
		Public Sub New(ByVal updateStorage As UpdateStorage, ByVal ndArrayHolder As NDArrayHolder, ByVal workers As Integer)
			MyBase.New(updateStorage, ndArrayHolder)
			Me.workers = workers
		End Sub

		''' <summary>
		''' Initialize this updater
		''' with a custom update storage
		''' </summary>
		''' <param name="updateStorage"> the update storage to use </param>
		Public Sub New(ByVal updateStorage As UpdateStorage, ByVal workers As Integer)
			MyBase.New(updateStorage)
			Me.workers = workers
		End Sub

		''' <summary>
		''' Initializes this updater
		''' with <seealso cref="org.nd4j.parameterserver.updater.storage.InMemoryUpdateStorage"/>
		''' </summary>
		Public Sub New(ByVal workers As Integer)
			Me.workers = workers
		End Sub


		''' <summary>
		''' Returns the current status of this parameter server
		''' updater
		''' 
		''' @return
		''' </summary>
		Public Overrides Function status() As IDictionary(Of String, Number)
			Dim ret As IDictionary(Of String, Number) = New Dictionary(Of String, Number)()
			ret("workers") = workers
			ret("accumulatedUpdates") = numUpdates()
			Return ret
		End Function

		''' <summary>
		''' Serialize this updater as json
		''' 
		''' @return
		''' </summary>
		Public Overrides Function toJson() As String
			Try
				Return objectMapper.writeValueAsString(status())
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function


		''' <summary>
		''' Returns true if
		''' the updater has accumulated enough ndarrays to
		''' replicate to the workers
		''' </summary>
		''' <returns> true if replication should happen,false otherwise </returns>
		Public Overrides Function shouldReplicate() As Boolean
			Return numUpdates() = workers
		End Function

		''' <summary>
		''' Do an update based on the ndarray message.
		''' </summary>
		''' <param name="message"> </param>
		Public Overrides Sub update(ByVal message As NDArrayMessage)
			updateStorage.addUpdate(message)
			Dim arr As INDArray = message.getArr()
			'of note for ndarrays
			Dim dimensions() As Integer = message.getDimensions()
			Dim whole As Boolean = dimensions.Length = 1 AndAlso dimensions(0) = -1

			If Not whole Then
				partialUpdate(arr, ndArrayHolder_Conflict.get(), message.getIndex(), dimensions)
			Else
				update(arr, ndArrayHolder_Conflict.get())
			End If
		End Sub

		''' <summary>
		''' Updates result
		''' based on arr along a particular
		''' <seealso cref="INDArray.tensorAlongDimension(Integer, Integer...)"/>
		''' </summary>
		''' <param name="arr">        the array to update </param>
		''' <param name="result">     the result ndarray to update </param>
		''' <param name="idx">        the index to update </param>
		''' <param name="dimensions"> the dimensions to update </param>
		Public Overrides Sub partialUpdate(ByVal arr As INDArray, ByVal result As INDArray, ByVal idx As Long, ParamArray ByVal dimensions() As Integer)
			result.tensorAlongDimension(CInt(idx), dimensions).addi(arr)
		End Sub

		''' <summary>
		''' Updates result
		''' based on arr
		''' </summary>
		''' <param name="arr">    the array to update </param>
		''' <param name="result"> the result ndarray to update </param>
		Public Overrides Sub update(ByVal arr As INDArray, ByVal result As INDArray)
			result.addi(arr)
		End Sub
	End Class

End Namespace