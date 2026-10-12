Imports System
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NDArrayHolder = org.nd4j.aeron.ipc.NDArrayHolder
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.aeron.ndarrayholder


	''' <summary>
	''' An in memory ndarray holder
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class InMemoryNDArrayHolder implements org.nd4j.aeron.ipc.NDArrayHolder
	<Serializable>
	Public Class InMemoryNDArrayHolder
		Implements NDArrayHolder

		Private arr As New AtomicReference(Of INDArray)()
'JAVA TO VB CONVERTER NOTE: The field totalUpdates was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private totalUpdates_Conflict As New AtomicInteger(0)


		Public Sub New(ByVal shape() As Integer)
			Array = Nd4j.zeros(shape)
		End Sub


		Public Sub New(ByVal arr As INDArray)
			Array = arr
		End Sub


		''' <summary>
		''' Set the ndarray
		''' </summary>
		''' <param name="arr"> the ndarray for this holder
		'''            to use </param>
		Public Overridable WriteOnly Property Array Implements NDArrayHolder.setArray As INDArray
			Set(ByVal arr As INDArray)
				If Me.arr.get() Is Nothing Then
					Me.arr.set(arr)
				End If
			End Set
		End Property

		''' <summary>
		''' The number of updates
		''' that have been sent to this older.
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalUpdates() As Integer Implements NDArrayHolder.totalUpdates
			Return totalUpdates_Conflict.get()
		End Function

		''' <summary>
		''' Retrieve an ndarray
		''' 
		''' @return
		''' </summary>
		Public Overridable Function get() As INDArray Implements NDArrayHolder.get
			Return arr.get()
		End Function

		''' <summary>
		''' Retrieve a partial view of the ndarray.
		''' This method uses tensor along dimension internally
		''' Note this will call dup()
		''' </summary>
		''' <param name="idx">        the index of the tad to get </param>
		''' <param name="dimensions"> the dimensions to use </param>
		''' <returns> the tensor along dimension based on the index and dimensions
		''' from the master array. </returns>
		Public Overridable Function getTad(ByVal idx As Integer, ParamArray ByVal dimensions() As Integer) As INDArray Implements NDArrayHolder.getTad
			Return arr.get().tensorAlongDimension(idx, dimensions)
		End Function
	End Class

End Namespace