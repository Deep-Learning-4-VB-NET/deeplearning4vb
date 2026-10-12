Imports System
Imports System.Collections.Generic
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


	Public Class TimeDelayedParameterUpdater
		Inherits BaseParameterUpdater

		Private syncTime As Long
		Private lastSynced As Long


		''' <summary>
		''' Returns the number of required
		''' updates for a new pass
		''' </summary>
		''' <returns> the number of required updates for a new pass </returns>
		Public Overrides Function requiredUpdatesForPass() As Integer
			Return 0
		End Function

		''' <summary>
		''' Returns the current status of this parameter server
		''' updater
		''' 
		''' @return
		''' </summary>
		Public Overrides Function status() As IDictionary(Of String, Number)
			Return Nothing
		End Function

		''' <summary>
		''' Serialize this updater as json
		''' 
		''' @return
		''' </summary>
		Public Overrides Function toJson() As String
			Return Nothing
		End Function

		''' <summary>
		''' Reset internal counters
		''' such as number of updates accumulated.
		''' </summary>
		Public Overrides Sub reset()

		End Sub

		''' <summary>
		''' Returns true if
		''' the updater has accumulated enough ndarrays to
		''' replicate to the workers
		''' </summary>
		''' <returns> true if replication should happen,false otherwise </returns>
		Public Overrides Function shouldReplicate() As Boolean
			Dim now As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim diff As Long = Math.Abs(now - lastSynced)
			Return diff > syncTime
		End Function

		''' <summary>
		''' Do an update based on the ndarray message.
		''' </summary>
		''' <param name="message"> </param>
		Public Overrides Sub update(ByVal message As NDArrayMessage)

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

		End Sub

		''' <summary>
		''' Updates result
		''' based on arr
		''' </summary>
		''' <param name="arr">    the array to update </param>
		''' <param name="result"> the result ndarray to update </param>
		Public Overrides Sub update(ByVal arr As INDArray, ByVal result As INDArray)

		End Sub
	End Class

End Namespace