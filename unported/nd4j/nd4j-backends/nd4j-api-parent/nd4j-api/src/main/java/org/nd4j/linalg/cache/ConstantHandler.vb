Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.linalg.cache

	Public Interface ConstantHandler

		''' <summary>
		''' If specific hardware supports dedicated constant memory,
		''' this method forces DataBuffer passed in to be moved
		''' to that constant memory.
		''' 
		''' PLEASE NOTE: This method implementation is hardware-dependant.
		''' </summary>
		''' <param name="dataBuffer">
		''' @return </param>
		Function moveToConstantSpace(ByVal dataBuffer As DataBuffer) As Long

		''' 
		''' <summary>
		''' PLEASE NOTE: This method implementation is hardware-dependant.
		''' PLEASE NOTE: This method does NOT allow concurrent use of any array
		''' </summary>
		''' <param name="dataBuffer">
		''' @return </param>
		Function relocateConstantSpace(ByVal dataBuffer As DataBuffer) As DataBuffer

		''' <summary>
		''' This method returns DataBuffer with
		''' constant equal to input array.
		''' 
		''' PLEASE NOTE: This method assumes that
		''' you'll never ever change values
		''' within result DataBuffer
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Function getConstantBuffer(ByVal array() As Boolean, ByVal dataType As DataType) As DataBuffer

		Function getConstantBuffer(ByVal array() As Integer, ByVal dataType As DataType) As DataBuffer

		''' <summary>
		''' This method returns DataBuffer with
		''' constant equal to input array.
		''' 
		''' PLEASE NOTE: This method assumes that
		''' you'll never ever change values
		''' within result DataBuffer
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Function getConstantBuffer(ByVal array() As Long, ByVal dataType As DataType) As DataBuffer

		''' <summary>
		''' This method returns DataBuffer with contant equal to input array.
		''' 
		''' PLEASE NOTE: This method assumes that you'll never ever change values within result DataBuffer
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Function getConstantBuffer(ByVal array() As Double, ByVal dataType As DataType) As DataBuffer

		''' <summary>
		''' This method returns DataBuffer with contant equal to input array.
		''' 
		''' PLEASE NOTE: This method assumes that you'll never ever change values within result DataBuffer
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Function getConstantBuffer(ByVal array() As Single, ByVal dataType As DataType) As DataBuffer

		''' <summary>
		''' This method removes all cached constants
		''' </summary>
		Sub purgeConstants()

		''' <summary>
		''' This method returns memory used for cache, in bytes
		''' 
		''' @return
		''' </summary>
		ReadOnly Property CachedBytes As Long
	End Interface

End Namespace