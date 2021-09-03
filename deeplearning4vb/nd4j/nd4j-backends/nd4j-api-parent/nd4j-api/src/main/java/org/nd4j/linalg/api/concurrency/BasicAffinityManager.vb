Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
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

Namespace org.nd4j.linalg.api.concurrency

	Public MustInherit Class BasicAffinityManager
		Implements AffinityManager

		Public MustOverride Sub touch(ByVal buffer As DataBuffer) Implements AffinityManager.touch
		Public MustOverride Sub touch(ByVal array As INDArray) Implements AffinityManager.touch
		Public Overridable ReadOnly Property DeviceForCurrentThread As Integer? Implements AffinityManager.getDeviceForCurrentThread
			Get
				Return 0
			End Get
		End Property

		Public Overridable Function getDeviceForThread(ByVal threadId As Long) As Integer? Implements AffinityManager.getDeviceForThread
			Return 0
		End Function

		Public Overridable Function getDeviceForArray(ByVal array As INDArray) As Integer? Implements AffinityManager.getDeviceForArray
			Return 0
		End Function

		Public Overridable ReadOnly Property NumberOfDevices As Integer Implements AffinityManager.getNumberOfDevices
			Get
				Return 1
			End Get
		End Property

		''' <summary>
		''' This method replicates given INDArray, and places it to target device.
		''' </summary>
		''' <param name="deviceId"> target deviceId </param>
		''' <param name="array">    INDArray to replicate
		''' @return </param>
		Public Overridable Function replicateToDevice(ByVal deviceId As Integer?, ByVal array As INDArray) As INDArray Implements AffinityManager.replicateToDevice
			Return Nothing
		End Function

		''' <summary>
		''' This method replicates given DataBuffer, and places it to target device.
		''' </summary>
		''' <param name="deviceId"> target deviceId </param>
		''' <param name="buffer">
		''' @return </param>
		Public Overridable Function replicateToDevice(ByVal deviceId As Integer?, ByVal buffer As DataBuffer) As DataBuffer Implements AffinityManager.replicateToDevice
			Return Nothing
		End Function

		Public Overridable Sub tagLocation(ByVal array As INDArray, ByVal location As Location) Implements AffinityManager.tagLocation
			' no-op
		End Sub

		Public Overridable Sub tagLocation(ByVal buffer As DataBuffer, ByVal location As Location) Implements AffinityManager.tagLocation
			' no-op
		End Sub

		Public Overridable Sub unsafeSetDevice(ByVal deviceId As Integer?) Implements AffinityManager.unsafeSetDevice
			' no-op
		End Sub

		Public Overridable Sub ensureLocation(ByVal array As INDArray, ByVal location As Location) Implements AffinityManager.ensureLocation
			' no-op
		End Sub

		Public Overridable ReadOnly Property CrossDeviceAccessSupported As Boolean Implements AffinityManager.isCrossDeviceAccessSupported
			Get
				Return True
			End Get
		End Property

		Public Overridable Sub allowCrossDeviceAccess(ByVal reallyAllow As Boolean) Implements AffinityManager.allowCrossDeviceAccess
			' no-op
		End Sub

		Public Overridable Function getActiveLocation(ByVal array As INDArray) As Location Implements AffinityManager.getActiveLocation
			Return Location.EVERYWHERE
		End Function
	End Class

End Namespace