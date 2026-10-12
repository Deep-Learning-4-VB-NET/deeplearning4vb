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

Namespace org.nd4j.linalg.indexing

	Public Interface INDArrayIndex
		''' <summary>
		''' The ending for this index
		''' @return
		''' </summary>
		Function [end]() As Long

		''' <summary>
		''' The start of this index
		''' @return
		''' </summary>
		Function offset() As Long

		''' <summary>
		''' The total length of this index (end - start)
		''' @return
		''' </summary>
		Function length() As Long

		''' <summary>
		''' The stride for the index (most of the time will be 1)
		''' @return
		''' </summary>
		Function stride() As Long

		''' <summary>
		''' Reverse the indexes
		''' </summary>
		Sub reverse()

		''' <summary>
		''' Returns true
		''' if the index is an interval
		''' @return
		''' </summary>
		ReadOnly Property Interval As Boolean

		''' <summary>
		''' Init the index wrt
		''' the dimension and the given nd array </summary>
		''' <param name="arr"> the array to initialize on </param>
		''' <param name="begin"> the beginning index </param>
		''' <param name="dimension"> the dimension to initialize on </param>
		Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer)

		''' <summary>
		''' Init the index wrt
		''' the dimension and the given nd array </summary>
		''' <param name="arr"> the array to initialize on </param>
		''' <param name="dimension"> the dimension to initialize on </param>
		Sub init(ByVal arr As INDArray, ByVal dimension As Integer)

		Sub init(ByVal begin As Long, ByVal [end] As Long, ByVal max As Long)

		''' <summary>
		''' Initiailize based on the specified begin and end </summary>
		''' <param name="begin"> </param>
		''' <param name="end"> </param>
		Sub init(ByVal begin As Long, ByVal [end] As Long)
	End Interface

End Namespace