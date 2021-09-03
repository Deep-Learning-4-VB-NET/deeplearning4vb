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
Namespace org.nd4j.linalg.factory

	Public Interface Environment

		''' <summary>
		''' BLAS major version number (if applicable) </summary>
		Function blasMajorVersion() As Integer
		''' <summary>
		''' BLAS minor version number (if applicable) </summary>
		Function blasMinorVersion() As Integer
		''' <summary>
		''' BLAS patch version number (if applicable) </summary>
		Function blasPatchVersion() As Integer

		''' <summary>
		''' Returns true if ND4J is set to verbose mode </summary>
		Property Verbose As Boolean
		''' <summary>
		''' Returns true if ND4J is set to debug mode </summary>
		Property Debug As Boolean
		''' <summary>
		''' Returns true if ND4J is set to profiling mode </summary>
		Property Profiling As Boolean
		''' <summary>
		''' Returns true if ND4J is set to detecting leaks mode </summary>
		ReadOnly Property DetectingLeaks As Boolean
		''' <summary>
		''' Returns true if ND4J is set to debug and verbose mode </summary>
		ReadOnly Property DebugAndVerbose As Boolean

		''' <summary>
		''' Set leaks detection mode </summary>
		WriteOnly Property LeaksDetector As Boolean
		''' <summary>
		''' Returns true if helpers (cuDNN, DNNL/MKLDNN etc) are allowed </summary>
		Function helpersAllowed() As Boolean
		''' <summary>
		''' Set whether helpers (cuDNN, DNNL/MKLDNN etc) are allowed </summary>
		Sub allowHelpers(ByVal reallyAllow As Boolean)

		''' <summary>
		''' Returns the TAD (tensor along dimension) threshold for ops </summary>
		Function tadThreshold() As Integer
		''' <summary>
		''' Set the TAD (tensor along dimension) threshold for ops </summary>
		WriteOnly Property TadThreshold As Integer

		''' <summary>
		''' Returns the elementwise threshold for ops </summary>
		Function elementwiseThreshold() As Integer
		''' <summary>
		''' Set the elementwise threshold for ops </summary>
		WriteOnly Property ElementwiseThreshold As Integer

		''' <summary>
		''' Returns the maximum number of threads for C++ op execution (if applicable) </summary>
		Function maxThreads() As Integer
		''' <summary>
		''' Set the maximum number of threads for C++ op execution (if applicable) </summary>
		WriteOnly Property MaxThreads As Integer

		''' <summary>
		''' Returns the maximum number of master threads for C++ op execution (if applicable) </summary>
		Function maxMasterThreads() As Integer
		''' <summary>
		''' Set the maximum number of master threads for C++ op execution (if applicable) </summary>
		WriteOnly Property MaxMasterThreads As Integer

		''' <summary>
		''' Set the maximum primary memory </summary>
		WriteOnly Property MaxPrimaryMemory As Long
		''' <summary>
		''' Set the maximum special memory </summary>
		WriteOnly Property MaxSpecialMemory As Long
		''' <summary>
		''' Set the maximum device memory </summary>
		WriteOnly Property MaxDeviceMemory As Long

		''' <summary>
		''' Return true if the backend is a CPU backend, or false otherwise </summary>
		ReadOnly Property CPU As Boolean

		''' <summary>
		''' This method allows to set memory limit for a specific group of devices. I.e. CUDA or CPU </summary>
		''' <param name="group"> </param>
		''' <param name="numBytes"> </param>
		Sub setGroupLimit(ByVal group As Integer, ByVal numBytes As Long)

		''' <summary>
		''' This method allows to set memory limit for a specific device. I.e. GPU_0 </summary>
		''' <param name="deviceId"> </param>
		''' <param name="numBytes"> </param>
		Sub setDeviceLimit(ByVal deviceId As Integer, ByVal numBytes As Long)

		''' <summary>
		''' This method returns current group limit </summary>
		''' <param name="group">
		''' @return </param>
		Function getGroupLimit(ByVal group As Integer) As Long

		''' <summary>
		''' This method returns current device limit </summary>
		''' <param name="deviceId">
		''' @return </param>
		Function getDeviceLimit(ByVal deviceId As Integer) As Long

		''' <summary>
		''' This method returns current allocated amount for a specific device. I.e. GPU_0 </summary>
		''' <param name="deviceId">
		''' @return </param>
		Function getDeviceCouner(ByVal deviceId As Integer) As Long
	End Interface

End Namespace