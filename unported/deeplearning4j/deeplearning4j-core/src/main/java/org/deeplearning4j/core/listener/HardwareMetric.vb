Imports System
Imports System.Collections.Generic
Imports lombok
Imports Nd4jEnvironment = org.nd4j.linalg.api.environment.Nd4jEnvironment
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports YAMLFactory = org.nd4j.shade.jackson.dataformat.yaml.YAMLFactory
Imports SystemInfo = oshi.json.SystemInfo
Imports CentralProcessor = oshi.json.hardware.CentralProcessor
Imports GlobalMemory = oshi.json.hardware.GlobalMemory
Imports HWDiskStore = oshi.json.hardware.HWDiskStore
Imports NetworkParams = oshi.json.software.os.NetworkParams
Imports Util = oshi.util.Util

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

Namespace org.deeplearning4j.core.listener


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Data @AllArgsConstructor public class HardwareMetric implements java.io.Serializable
	<Serializable>
	Public Class HardwareMetric

		Private Shared yamlMapper As New ObjectMapper(New YAMLFactory())

		Private perCoreMetrics As IDictionary(Of Integer, DeviceMetric)
		Private physicalProcessorCount, logicalProcessorCount As Long
		Private currentMemoryUse As Long
		Private gpuMetrics As IDictionary(Of Integer, DeviceMetric)
		Private hostName As String
		Private ioWaitTime As Long
		Private averagedCpuLoad As Long
		Private diskInfo As IDictionary(Of Integer, DiskInfo)
		Private name As String

		Private Sub New()
			'No-arg for JSON/YAML
		End Sub


		''' <summary>
		''' Runs <seealso cref="fromSystem(SystemInfo)"/>
		''' with a fresh <seealso cref="SystemInfo"/> </summary>
		''' <returns> the hardware metric based on
		''' the current snapshot of the system this
		''' runs on </returns>
		Public Shared Function fromSystem() As HardwareMetric
			Return fromSystem(New SystemInfo())
		End Function



		''' <summary>
		''' Returns the relevant information
		''' needed for system diagnostics
		''' based on the <seealso cref="SystemInfo"/> </summary>
		''' <param name="systemInfo"> the system info to use </param>
		''' <returns> the <seealso cref="HardwareMetric"/> for the
		''' system this process runs on </returns>
		Public Shared Function fromSystem(ByVal systemInfo As SystemInfo) As HardwareMetric
			Return fromSystem(systemInfo,System.Guid.randomUUID().ToString())
		End Function

		''' <summary>
		''' Returns the relevant information
		''' needed for system diagnostics
		''' based on the <seealso cref="SystemInfo"/> </summary>
		''' <param name="systemInfo"> the system info to use </param>
		''' <returns> the <seealso cref="HardwareMetric"/> for the
		''' system this process runs on </returns>
		Public Shared Function fromSystem(ByVal systemInfo As SystemInfo, ByVal name As String) As HardwareMetric
			Dim builder As HardwareMetricBuilder = HardwareMetric.builder()
			Dim processor As CentralProcessor = systemInfo.getHardware().getProcessor()
			Dim prevTicks() As Long = processor.getSystemCpuLoadTicks()
			' Wait a second...
			Util.sleep(1000)
			Dim ticks() As Long = processor.getSystemCpuLoadTicks()
			Dim iowait As Long = ticks(oshi.hardware.CentralProcessor.TickType.IOWAIT.getIndex()) - prevTicks(oshi.hardware.CentralProcessor.TickType.IOWAIT.getIndex())

			Dim globalMemory As GlobalMemory = systemInfo.getHardware().getMemory()
			Dim networkParams As NetworkParams = systemInfo.getOperatingSystem().getNetworkParams()

			Dim processorCpuLoadBetweenTicks() As Double = processor.getProcessorCpuLoadBetweenTicks()
			Dim cpuMetrics As IDictionary(Of Integer, DeviceMetric) = New LinkedHashMap(Of Integer, DeviceMetric)()
			For i As Integer = 0 To processorCpuLoadBetweenTicks.Length - 1
				cpuMetrics(i) = DeviceMetric.builder().load(processorCpuLoadBetweenTicks(i)).build()
			Next i


			Dim diskInfoMap As IDictionary(Of Integer, DiskInfo) = New LinkedHashMap(Of Integer, DiskInfo)()

			Dim diskStores() As HWDiskStore = systemInfo.getHardware().getDiskStores()
			For i As Integer = 0 To diskStores.Length - 1
				Dim diskStore As HWDiskStore = diskStores(i)
				Dim diskInfo As DiskInfo = DiskInfo.builder().bytesRead(diskStore.getReadBytes()).bytesWritten(diskStore.getWriteBytes()).name(diskStore.getName()).modelName(diskStore.getModel()).transferTime(diskStore.getTransferTime()).build()
				diskInfoMap(i) = diskInfo

			Next i

			Dim gpuMetric As IDictionary(Of Integer, DeviceMetric) = New Dictionary(Of Integer, DeviceMetric)()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			If Nd4j.Backend.GetType().FullName.ToLower().contains("cublas") Then
				Dim info As Properties = Nd4j.Executioner.EnvironmentInformation
				''' 

				Dim devicesList As IList(Of IDictionary(Of String, Object)) = CType(info.get(Nd4jEnvironment.CUDA_DEVICE_INFORMATION_KEY), IList(Of IDictionary(Of String, Object)))
				For i As Integer = 0 To devicesList.Count - 1
					Dim available As Double = Double.Parse(devicesList(i)(Nd4jEnvironment.CUDA_FREE_MEMORY_KEY).ToString())
					Dim memcpyDirectionLongMap As IDictionary(Of MemcpyDirection, Long) = PerformanceTracker.Instance.getCurrentBandwidth()(i)
					Dim deviceMetric As DeviceMetric = DeviceMetric.builder().bandwidthHostToDevice(memcpyDirectionLongMap(MemcpyDirection.HOST_TO_DEVICE)).bandwidthDeviceToHost(memcpyDirectionLongMap(MemcpyDirection.DEVICE_TO_HOST)).bandwidthDeviceToDevice(memcpyDirectionLongMap(MemcpyDirection.DEVICE_TO_DEVICE)).memAvailable(available).totalMemory(Double.Parse(devicesList(i)(Nd4jEnvironment.CUDA_TOTAL_MEMORY_KEY).ToString())).deviceName(devicesList(i)(Nd4jEnvironment.CUDA_DEVICE_NAME_KEY).ToString()).build()
					gpuMetric(i) = deviceMetric

				Next i
			End If

			Return builder.logicalProcessorCount(processor.getLogicalProcessorCount()).physicalProcessorCount(processor.getPhysicalProcessorCount()).name(name).averagedCpuLoad(CLng(Math.Truncate(processor.getSystemCpuLoad() * 100))).ioWaitTime(iowait).gpuMetrics(gpuMetric).hostName(networkParams.getHostName()).diskInfo(diskInfoMap).currentMemoryUse(globalMemory.getTotal() - globalMemory.getAvailable()).perCoreMetrics(cpuMetrics).build()
		End Function

		Public Overridable Function toYaml() As String
			Try
				Return yamlMapper.writeValueAsString(Me)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static HardwareMetric fromYaml(@NonNull String yaml)
		Public Shared Function fromYaml(ByVal yaml As String) As HardwareMetric
			Try
				Return yamlMapper.readValue(yaml, GetType(HardwareMetric))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

	End Class

End Namespace