Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
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

Namespace org.nd4j.linalg.api.environment


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor @AllArgsConstructor public class Nd4jEnvironment implements java.io.Serializable
	<Serializable>
	Public Class Nd4jEnvironment
		Private ram As Long
		Private numCores As Integer
		Private os As String
		Private numGpus As Integer
		Private gpuRam As IList(Of Long)
		Private blasVendor As String
		Private blasThreads As Long
		Private ompThreads As Integer

		Public Const MEMORY_BANDWIDTH_KEY As String = "memoryBandwidth"

		Public Const CUDA_DEVICE_NAME_KEY As String = "cuda.deviceName"
		Public Const CUDA_FREE_MEMORY_KEY As String = "cuda.freeMemory"
		Public Const CUDA_TOTAL_MEMORY_KEY As String = "cuda.totalMemory"
		Public Const CUDA_DEVICE_MAJOR_VERSION_KEY As String = "cuda.deviceMajor"
		Public Const CUDA_DEVICE_MINOR_VERSION_KEY As String = "cuda.deviceMinor"

		Public Const BACKEND_KEY As String = "backend"
		Public Const CUDA_NUM_GPUS_KEY As String = "cuda.availableDevices"
		Public Const CUDA_DEVICE_INFORMATION_KEY As String = "cuda.devicesInformation"
		Public Const BLAS_VENDOR_KEY As String = "blas.vendor"

		Public Const OS_KEY As String = "os"
		Public Const HOST_FREE_MEMORY_KEY As String = "memory.free"
		Public Const HOST_TOTAL_MEMORY_KEY As String = "memory.available"
		Public Const CPU_CORES_KEY As String = "cores"

		Public Const OMP_THREADS_KEY As String = "omp.threads"
		Public Const BLAS_THREADS_KEY As String = "blas.threads"

		''' <summary>
		''' Load an <seealso cref="Nd4jEnvironment"/> from
		''' the properties returned from <seealso cref="org.nd4j.linalg.api.ops.executioner.OpExecutioner.getEnvironmentInformation()"/>
		''' derived from <seealso cref="Nd4j.getExecutioner()"/> </summary>
		''' <returns> the environment representing the system the nd4j
		''' backend is running on. </returns>
		Public Shared ReadOnly Property Environment As Nd4jEnvironment
			Get
				Dim envInfo As Properties = Nd4j.Executioner.EnvironmentInformation
				Dim ret As Nd4jEnvironment = Nd4jEnvironment.builder().numCores(getIntOrZero(CPU_CORES_KEY, envInfo)).ram(getLongOrZero(HOST_TOTAL_MEMORY_KEY, envInfo)).os(envInfo.get(OS_KEY).ToString()).blasVendor(envInfo.get(BLAS_VENDOR_KEY).ToString()).blasThreads(getLongOrZero(BLAS_THREADS_KEY, envInfo)).ompThreads(getIntOrZero(OMP_THREADS_KEY, envInfo)).numGpus(getIntOrZero(CUDA_NUM_GPUS_KEY, envInfo)).build()
				If envInfo.containsKey(CUDA_DEVICE_INFORMATION_KEY) Then
					Dim deviceInfo As IList(Of IDictionary(Of String, Object)) = CType(envInfo.get(CUDA_DEVICE_INFORMATION_KEY), IList(Of IDictionary(Of String, Object)))
					Dim gpuRam As IList(Of Long) = New List(Of Long)()
					For Each info As IDictionary(Of String, Object) In deviceInfo
						gpuRam.Add(Long.Parse(info(Nd4jEnvironment.CUDA_TOTAL_MEMORY_KEY).ToString()))
					Next info
    
					ret.setGpuRam(gpuRam)
				End If
    
    
				Return ret
    
			End Get
		End Property


		Private Shared Function getLongOrZero(ByVal key As String, ByVal properties As Properties) As Long
			If properties.get(key) Is Nothing Then
				Return 0
			End If
			Return Long.Parse(properties.get(key).ToString())
		End Function

		Private Shared Function getIntOrZero(ByVal key As String, ByVal properties As Properties) As Integer
			If properties.get(key) Is Nothing Then
				Return 0
			End If
			Return Integer.Parse(properties.get(key).ToString())
		End Function


	End Class

End Namespace