Imports Data = lombok.Data
Imports NDArrayCallback = org.nd4j.aeron.ipc.NDArrayCallback
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
Imports InMemoryNDArrayHolder = org.nd4j.aeron.ndarrayholder.InMemoryNDArrayHolder
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ParameterServerUpdater = org.nd4j.parameterserver.updater.ParameterServerUpdater
Imports SynchronousParameterUpdater = org.nd4j.parameterserver.updater.SynchronousParameterUpdater
Imports NoUpdateStorage = org.nd4j.parameterserver.updater.storage.NoUpdateStorage

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

Namespace org.nd4j.parameterserver


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ParameterServerListener implements org.nd4j.aeron.ipc.NDArrayCallback
	Public Class ParameterServerListener
		Implements NDArrayCallback

		Private updater As ParameterServerUpdater
		Private master As Boolean
		Private shape() As Integer

		''' <summary>
		''' Shape of the ndarray </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="updatesPerEpoch">  the number of updates per epoch
		'''                         for synchronization </param>
		Public Sub New(ByVal shape() As Integer, ByVal updatesPerEpoch As Integer)
			updater = New SynchronousParameterUpdater(New NoUpdateStorage(), New InMemoryNDArrayHolder(shape), updatesPerEpoch)
		End Sub

		''' <summary>
		''' Shape of the ndarray </summary>
		''' <param name="shape"> the shape of the array </param>
		Public Sub New(ByVal shape() As Integer)
			Me.New(shape, Runtime.getRuntime().availableProcessors())
		End Sub


		''' 
		''' <param name="shape"> the shape of the array </param>
		''' <param name="updater"> the updater to use for this server </param>
		Public Sub New(ByVal shape() As Integer, ByVal updater As ParameterServerUpdater)
			Me.updater = updater
			Me.shape = shape

		End Sub

		''' <summary>
		''' A listener for ndarray message
		''' </summary>
		''' <param name="message"> the message for the callback </param>
		Public Overridable Sub onNDArrayMessage(ByVal message As NDArrayMessage)
			updater.update(message)
		End Sub

		''' <summary>
		''' Used for partial updates using tensor along
		''' dimension </summary>
		'''  <param name="arr">        the array to count as an update </param>
		''' <param name="idx">        the index for the tensor along dimension </param>
		''' <param name="dimensions"> the dimensions to act on for the tensor along dimension </param>
		Public Overridable Sub onNDArrayPartial(ByVal arr As INDArray, ByVal idx As Long, ParamArray ByVal dimensions() As Integer) Implements NDArrayCallback.onNDArrayPartial
			SyncLock Me
				updater.partialUpdate(arr, updater.ndArrayHolder().get(), idx, dimensions)
			End SyncLock
		End Sub

		''' <summary>
		''' Setup an ndarray
		''' </summary>
		''' <param name="arr"> </param>
		Public Overridable Sub onNDArray(ByVal arr As INDArray) Implements NDArrayCallback.onNDArray
			SyncLock Me
				If shape Is Nothing Then
					updater.update(arr.reshape(ChrW(1), arr.length()), updater.ndArrayHolder().get())
				Else
					updater.update(arr, updater.ndArrayHolder().get())
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Do a final divide for averaging
		''' </summary>
		Public Overridable Sub finish()
			SyncLock Me
				updater.ndArrayHolder().get().divi(updater.numUpdates())
			End SyncLock
		End Sub


	End Class

End Namespace