Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports BasicConstantHandler = org.nd4j.linalg.cache.BasicConstantHandler
Imports ConstantHandler = org.nd4j.linalg.cache.ConstantHandler
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.jita.constant

	''' <summary>
	''' ConstantHandler implementation for CUDA backend.
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class CudaConstantHandler
		Inherits BasicConstantHandler

		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(CudaConstantHandler))

		Protected Friend Shared ReadOnly wrappedHandler As ConstantHandler = ProtectedCudaConstantHandler.Instance

		Public Sub New()

		End Sub

		Public Overrides Function moveToConstantSpace(ByVal dataBuffer As DataBuffer) As Long
			Return wrappedHandler.moveToConstantSpace(dataBuffer)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Integer, ByVal type As DataType) As DataBuffer
			Return wrappedHandler.getConstantBuffer(array, type)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Single, ByVal type As DataType) As DataBuffer
			Return wrappedHandler.getConstantBuffer(array, type)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Double, ByVal type As DataType) As DataBuffer
			Return wrappedHandler.getConstantBuffer(array, type)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Long, ByVal type As DataType) As DataBuffer
			Return wrappedHandler.getConstantBuffer(array, type)
		End Function

		Public Overrides Function relocateConstantSpace(ByVal dataBuffer As DataBuffer) As DataBuffer
			Return wrappedHandler.relocateConstantSpace(dataBuffer)
		End Function

		Public Overrides Function getConstantBuffer(ByVal array() As Boolean, ByVal dataType As DataType) As DataBuffer
			Return wrappedHandler.getConstantBuffer(array, dataType)
		End Function

		''' <summary>
		''' This method removes all cached constants
		''' </summary>
		Public Overrides Sub purgeConstants()
			wrappedHandler.purgeConstants()
		End Sub

		Public Overrides ReadOnly Property CachedBytes As Long
			Get
				Return wrappedHandler.CachedBytes
			End Get
		End Property
	End Class

End Namespace