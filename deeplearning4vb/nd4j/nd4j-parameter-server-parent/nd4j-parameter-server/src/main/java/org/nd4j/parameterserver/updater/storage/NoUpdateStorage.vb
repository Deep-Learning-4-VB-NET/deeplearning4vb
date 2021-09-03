Imports Slf4j = lombok.extern.slf4j.Slf4j
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NoUpdateStorage extends BaseUpdateStorage
	Public Class NoUpdateStorage
		Inherits BaseUpdateStorage

		Private updateCount As New AtomicInteger(0)

		''' <summary>
		''' Add an ndarray to the storage
		''' </summary>
		''' <param name="array"> the array to add </param>
		Public Overrides Sub addUpdate(ByVal array As NDArrayMessage)
			log.info("Adding array " & updateCount.get())
			updateCount.incrementAndGet()
		End Sub

		''' <summary>
		''' The number of updates added
		''' to the update storage
		''' 
		''' @return
		''' </summary>
		Public Overrides Function numUpdates() As Integer
			Return updateCount.get()
		End Function

		''' <summary>
		''' Clear the array storage
		''' </summary>
		Public Overrides Sub clear()
			updateCount.set(0)
		End Sub

		''' <summary>
		''' A method for actually performing the implementation
		''' of retrieving the ndarray
		''' </summary>
		''' <param name="index"> the index of the <seealso cref="INDArray"/> to get </param>
		''' <returns> the ndarray at the specified index </returns>
		Public Overrides Function doGetUpdate(ByVal index As Integer) As NDArrayMessage
			Throw New System.NotSupportedException("Nothing is being stored in this implementation")
		End Function
	End Class

End Namespace