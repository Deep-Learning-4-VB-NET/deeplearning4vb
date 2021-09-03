Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer

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

Namespace org.nd4j.linalg.api.buffer.allocation


	Public Interface MemoryStrategy


		''' <summary>
		''' Set the data for the buffer </summary>
		''' <param name="buffer"> the buffer to set </param>
		''' <param name="offset"> the offset to start at </param>
		''' <param name="stride"> the stride to sue </param>
		''' <param name="length"> the length to go till </param>
		Sub setData(ByVal buffer As DataBuffer, ByVal offset As Integer, ByVal stride As Integer, ByVal length As Integer)

		''' 
		''' <param name="buffer"> </param>
		''' <param name="offset"> </param>
		Sub setData(ByVal buffer As DataBuffer, ByVal offset As Integer)

		''' <summary>
		''' Copy data to native or gpu </summary>
		''' <param name="copy"> the buffer to copy </param>
		''' <returns> a pointer representing
		''' the copied data </returns>
		Function copyToHost(ByVal copy As DataBuffer, ByVal offset As Integer) As Object

		''' <summary>
		''' Allocate memory for the given buffer </summary>
		''' <param name="buffer"> the buffer to allocate for </param>
		''' <param name="stride"> the stride </param>
		''' <param name="offset"> the offset used for the buffer
		'''               on allocation </param>
		''' <param name="length"> length </param>
		Function alloc(ByVal buffer As DataBuffer, ByVal stride As Integer, ByVal offset As Integer, ByVal length As Integer) As Object

		''' <summary>
		''' Free the buffer wrt the
		''' allocation strategy </summary>
		''' <param name="buffer"> the buffer to free </param>
		''' <param name="offset"> the offset to free </param>
		''' <param name="length"> the length to free </param>
		Sub free(ByVal buffer As DataBuffer, ByVal offset As Integer, ByVal length As Integer)
	End Interface

End Namespace