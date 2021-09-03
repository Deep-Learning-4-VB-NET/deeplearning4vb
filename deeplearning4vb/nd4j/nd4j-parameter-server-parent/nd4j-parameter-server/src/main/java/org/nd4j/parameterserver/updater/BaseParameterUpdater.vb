Imports System.Collections.Generic
Imports NDArrayHolder = org.nd4j.aeron.ipc.NDArrayHolder
Imports InMemoryUpdateStorage = org.nd4j.parameterserver.updater.storage.InMemoryUpdateStorage
Imports UpdateStorage = org.nd4j.parameterserver.updater.storage.UpdateStorage

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

	Public MustInherit Class BaseParameterUpdater
		Implements ParameterServerUpdater

		Public MustOverride Sub update(ByVal arr As org.nd4j.linalg.api.ndarray.INDArray, ByVal result As org.nd4j.linalg.api.ndarray.INDArray) Implements ParameterServerUpdater.update
		Public MustOverride Sub partialUpdate(ByVal arr As org.nd4j.linalg.api.ndarray.INDArray, ByVal result As org.nd4j.linalg.api.ndarray.INDArray, ByVal idx As Long, ByVal dimensions() As Integer)
		Public MustOverride Sub update(ByVal message As org.nd4j.aeron.ipc.NDArrayMessage) Implements ParameterServerUpdater.update
		Public MustOverride Function shouldReplicate() As Boolean Implements ParameterServerUpdater.shouldReplicate
		Public MustOverride Function toJson() As String Implements ParameterServerUpdater.toJson
		Public MustOverride Function status() As IDictionary(Of String, Number) Implements ParameterServerUpdater.status
		Public MustOverride Function requiredUpdatesForPass() As Integer Implements ParameterServerUpdater.requiredUpdatesForPass
		Protected Friend updateStorage As UpdateStorage
'JAVA TO VB CONVERTER NOTE: The field ndArrayHolder was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ndArrayHolder_Conflict As NDArrayHolder

		Public Sub New(ByVal updateStorage As UpdateStorage, ByVal ndArrayHolder As NDArrayHolder)
			Me.updateStorage = updateStorage
			Me.ndArrayHolder_Conflict = ndArrayHolder
		End Sub

		''' <summary>
		''' Returns true if the updater is
		''' ready for a new array
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Ready As Boolean Implements ParameterServerUpdater.isReady
			Get
				Return numUpdates() = requiredUpdatesForPass()
			End Get
		End Property

		''' <summary>
		''' Returns true if the
		''' given updater is async
		''' or synchronous
		''' updates
		''' </summary>
		''' <returns> true if the given updater
		''' is async or synchronous updates </returns>
		Public Overridable ReadOnly Property Async As Boolean Implements ParameterServerUpdater.isAsync
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Get the ndarray holder for this
		''' updater
		''' </summary>
		''' <returns> the ndarray holder for this updater </returns>
		Public Overridable Function ndArrayHolder() As NDArrayHolder Implements ParameterServerUpdater.ndArrayHolder
			Return ndArrayHolder_Conflict
		End Function

		''' <summary>
		''' Initialize this updater
		''' with a custom update storage </summary>
		''' <param name="updateStorage"> the update storage to use </param>
		Public Sub New(ByVal updateStorage As UpdateStorage)
			Me.updateStorage = updateStorage
		End Sub

		''' <summary>
		''' Initializes this updater
		''' with <seealso cref="InMemoryUpdateStorage"/>
		''' </summary>
		Public Sub New()
			Me.New(New InMemoryUpdateStorage())
		End Sub



		''' <summary>
		''' Reset internal counters
		''' such as number of updates accumulated.
		''' </summary>
		Public Overridable Sub reset() Implements ParameterServerUpdater.reset
			updateStorage.clear()
		End Sub


		''' <summary>
		''' Num updates passed through
		''' the updater
		''' </summary>
		''' <returns> the number of updates </returns>
		Public Overridable Function numUpdates() As Integer Implements ParameterServerUpdater.numUpdates
			Return updateStorage.numUpdates()
		End Function
	End Class

End Namespace