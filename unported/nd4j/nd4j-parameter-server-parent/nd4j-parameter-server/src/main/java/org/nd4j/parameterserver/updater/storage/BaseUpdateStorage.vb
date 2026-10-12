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

Namespace org.nd4j.parameterserver.updater.storage


	Public MustInherit Class BaseUpdateStorage
		Implements UpdateStorage

		Public MustOverride Sub clear() Implements UpdateStorage.clear
		Public MustOverride Function numUpdates() As Integer Implements UpdateStorage.numUpdates
		Public MustOverride Sub addUpdate(ByVal array As NDArrayMessage) Implements UpdateStorage.addUpdate
		''' <summary>
		''' Get the update at the specified index
		''' </summary>
		''' <param name="index"> the update to get </param>
		''' <returns> the update at the specified index </returns>
		Public Overridable Function getUpdate(ByVal index As Integer) As NDArrayMessage Implements UpdateStorage.getUpdate
			If index >= numUpdates() Then
				Throw New System.IndexOutOfRangeException("Index passed in " & index & " was >= current number of updates " & numUpdates())
			End If
			Return doGetUpdate(index)
		End Function

		''' <summary>
		''' A method for actually performing the implementation
		''' of retrieving the ndarray </summary>
		''' <param name="index"> the index of the <seealso cref="INDArray"/> to get </param>
		''' <returns> the ndarray at the specified index </returns>
		Public MustOverride Function doGetUpdate(ByVal index As Integer) As NDArrayMessage

		''' <summary>
		''' Close the database
		''' </summary>
		Public Overridable Sub close() Implements UpdateStorage.close
			'default no op
		End Sub
	End Class

End Namespace