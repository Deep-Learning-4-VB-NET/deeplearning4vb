Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports GPUInfo = org.nd4j.systeminfo.GPUInfo
Imports GPUInfoProvider = org.nd4j.systeminfo.GPUInfoProvider

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

Namespace org.nd4j.nativeblas

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NativeOpsGPUInfoProvider implements org.nd4j.systeminfo.GPUInfoProvider
	Public Class NativeOpsGPUInfoProvider
		Implements GPUInfoProvider

		Public Overridable ReadOnly Property GPUs As IList(Of GPUInfo) Implements GPUInfoProvider.getGPUs
			Get
				Dim nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
    
				Dim gpus As IList(Of GPUInfo) = New List(Of GPUInfo)()
    
    
				Dim nDevices As Integer = nativeOps.AvailableDevices
				If nDevices > 0 Then
					For i As Integer = 0 To nDevices - 1
						Try
							Dim name As String = nativeOps.getDeviceName(i)
							Dim total As Long = nativeOps.getDeviceTotalMemory(i)
							Dim free As Long = nativeOps.getDeviceFreeMemory(i)
							Dim major As Integer = nativeOps.getDeviceMajor(i)
							Dim minor As Integer = nativeOps.getDeviceMinor(i)
    
							gpus.Add(New GPUInfo(name, total, free, major, minor))
						Catch e As Exception
							log.warn("Can't add GPU", e)
						End Try
					Next i
				End If
    
				Return gpus
			End Get
		End Property

	End Class

End Namespace