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

Namespace org.nd4j.linalg.cache

	Public MustInherit Class BasicConstantHandler
		Implements ConstantHandler

		Public MustOverride ReadOnly Property CachedBytes As Long Implements ConstantHandler.getCachedBytes
		Public MustOverride Sub purgeConstants() Implements ConstantHandler.purgeConstants
		Public MustOverride Function getConstantBuffer(ByVal array() As Single, ByVal dataType As org.nd4j.linalg.api.buffer.DataType) As DataBuffer
		Public MustOverride Function getConstantBuffer(ByVal array() As Double, ByVal dataType As org.nd4j.linalg.api.buffer.DataType) As DataBuffer
		Public MustOverride Function getConstantBuffer(ByVal array() As Long, ByVal dataType As org.nd4j.linalg.api.buffer.DataType) As DataBuffer
		Public MustOverride Function getConstantBuffer(ByVal array() As Integer, ByVal dataType As org.nd4j.linalg.api.buffer.DataType) As DataBuffer
		Public MustOverride Function getConstantBuffer(ByVal array() As Boolean, ByVal dataType As org.nd4j.linalg.api.buffer.DataType) As DataBuffer
		Public Overridable Function moveToConstantSpace(ByVal dataBuffer As DataBuffer) As Long Implements ConstantHandler.moveToConstantSpace
			' no-op
			Return 0L
		End Function

		Public Overridable Function relocateConstantSpace(ByVal dataBuffer As DataBuffer) As DataBuffer Implements ConstantHandler.relocateConstantSpace
			Return dataBuffer
		End Function


	End Class

End Namespace