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

	Public Interface AffinityManager

		Friend Enum Location
			HOST
			DEVICE
			EVERYWHERE
		End Enum

		''' <summary>
		''' This method returns deviceId for current thread
		''' @return
		''' </summary>
		ReadOnly Property DeviceForCurrentThread As Integer?

		''' <summary>
		''' This method returns deviceId for a given thread
		''' @return
		''' </summary>
		Function getDeviceForThread(ByVal threadId As Long) As Integer?


		''' <summary>
		''' This method returns id of current device for a given INDArray
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Function getDeviceForArray(ByVal array As INDArray) As Integer?

		''' <summary>
		''' This method returns number of available devices
		''' @return
		''' </summary>
		ReadOnly Property NumberOfDevices As Integer

		''' <summary>
		''' Utility method, to associate INDArray with specific device (backend-specific)
		''' </summary>
		''' <param name="array"> </param>
		Sub touch(ByVal array As INDArray)

		''' <summary>
		''' Utility method, to associate INDArray with specific device (backend-specific)
		''' </summary>
		''' <param name="buffer"> </param>
		Sub touch(ByVal buffer As DataBuffer)

		''' <summary>
		''' This method replicates given INDArray, and places it to target device.
		''' </summary>
		''' <param name="deviceId">  target deviceId </param>
		''' <param name="array"> INDArray to replicate
		''' @return </param>
		Function replicateToDevice(ByVal deviceId As Integer?, ByVal array As INDArray) As INDArray

		''' <summary>
		''' This method replicates given DataBuffer, and places it to target device.
		''' </summary>
		''' <param name="deviceId">  target deviceId </param>
		''' <param name="buffer">
		''' @return </param>
		Function replicateToDevice(ByVal deviceId As Integer?, ByVal buffer As DataBuffer) As DataBuffer

		''' <summary>
		''' This method tags specific INDArray as "recent" on specified location
		''' </summary>
		''' <param name="location"> </param>
		Sub tagLocation(ByVal array As INDArray, ByVal location As Location)

		''' <summary>
		''' This method tags specific DataBuffer as "recent" on specified location
		''' </summary>
		''' <param name="location"> </param>
		Sub tagLocation(ByVal buffer As DataBuffer, ByVal location As Location)


		''' <summary>
		''' This method propagates given INDArray to specified location
		''' </summary>
		''' <param name="array"> </param>
		''' <param name="location"> </param>
		Sub ensureLocation(ByVal array As INDArray, ByVal location As Location)

		''' <summary>
		''' This method returns last-updated location for the given INDArray </summary>
		''' <param name="array">
		''' @return </param>
		Function getActiveLocation(ByVal array As INDArray) As Location

		''' <summary>
		''' This method forces specific device for current thread.
		''' 
		''' PLEASE NOTE: This method is UNSAFE and should NOT be used with 100% clearance about it.
		''' </summary>
		''' <param name="deviceId"> </param>
		Sub unsafeSetDevice(ByVal deviceId As Integer?)


		''' <summary>
		''' This method returns TRUE if cross-device access is allowed on this system
		''' </summary>
		ReadOnly Property CrossDeviceAccessSupported As Boolean

		''' <summary>
		''' This method allows to block cross-device access. Mostly suitable for debugging/testing purposes
		''' </summary>
		''' <param name="reallyAllow"> </param>
		Sub allowCrossDeviceAccess(ByVal reallyAllow As Boolean)
	End Interface

End Namespace