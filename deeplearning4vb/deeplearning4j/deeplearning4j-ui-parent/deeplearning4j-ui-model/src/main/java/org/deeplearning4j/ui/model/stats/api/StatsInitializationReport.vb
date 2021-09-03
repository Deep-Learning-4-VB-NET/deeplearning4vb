Imports System.Collections.Generic
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener

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

Namespace org.deeplearning4j.ui.model.stats.api


	Public Interface StatsInitializationReport
		Inherits Persistable

		Sub reportIDs(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamp As Long)

		''' <param name="arch">             Operating system architecture, as reported by JVM </param>
		''' <param name="osName">           Operating system name </param>
		''' <param name="jvmName">          JVM name </param>
		''' <param name="jvmVersion">       JVM version </param>
		''' <param name="jvmSpecVersion">   JVM Specification version (for example, 1.8) </param>
		''' <param name="nd4jBackendClass"> ND4J backend Factory class </param>
		''' <param name="nd4jDataTypeName"> ND4J datatype name </param>
		''' <param name="hostname">         Hostname for the machine, if available </param>
		''' <param name="jvmUID">           A unique identified for the current JVM. Should be shared by all instances in the same JVM.
		'''                         Should vary for different JVMs on the same machine. </param>
		''' <param name="swEnvironmentInfo"> Environment information: Usually from Nd4j.getExecutioner().getEnvironmentInformation() </param>
		Sub reportSoftwareInfo(ByVal arch As String, ByVal osName As String, ByVal jvmName As String, ByVal jvmVersion As String, ByVal jvmSpecVersion As String, ByVal nd4jBackendClass As String, ByVal nd4jDataTypeName As String, ByVal hostname As String, ByVal jvmUID As String, ByVal swEnvironmentInfo As IDictionary(Of String, String))

		''' <param name="jvmAvailableProcessors"> Number of available processor cores according to the JVM </param>
		''' <param name="numDevices">             Number of compute devices (GPUs) </param>
		''' <param name="jvmMaxMemory">           Maximum memory for the JVM </param>
		''' <param name="offHeapMaxMemory">       Maximum off-heap memory </param>
		''' <param name="deviceTotalMemory">      GPU memory by device: same length as numDevices. May be null, if numDevices is 0 </param>
		''' <param name="deviceDescription">      Description of each device. May be null, if numDevices is 0 </param>
		''' <param name="hardwareUID">            A unique identifier for the machine. Should be shared by all instances running on
		'''                               the same machine, including in different JVMs
		'''  </param>
		Sub reportHardwareInfo(ByVal jvmAvailableProcessors As Integer, ByVal numDevices As Integer, ByVal jvmMaxMemory As Long, ByVal offHeapMaxMemory As Long, ByVal deviceTotalMemory() As Long, ByVal deviceDescription() As String, ByVal hardwareUID As String)


		''' <summary>
		''' Report the model information
		''' </summary>
		''' <param name="modelClassName">  Model class name: i.e., type of model </param>
		''' <param name="modelConfigJson"> Model configuration, as JSON string </param>
		''' <param name="numLayers">       Number of layers in the model </param>
		''' <param name="numParams">       Number of parameters in the model </param>
		Sub reportModelInfo(ByVal modelClassName As String, ByVal modelConfigJson As String, ByVal paramNames() As String, ByVal numLayers As Integer, ByVal numParams As Long)


		Function hasSoftwareInfo() As Boolean

		Function hasHardwareInfo() As Boolean

		Function hasModelInfo() As Boolean

		ReadOnly Property SwArch As String

		ReadOnly Property SwOsName As String

		ReadOnly Property SwJvmName As String

		ReadOnly Property SwJvmVersion As String

		ReadOnly Property SwJvmSpecVersion As String

		ReadOnly Property SwNd4jBackendClass As String

		ReadOnly Property SwNd4jDataTypeName As String

		ReadOnly Property SwHostName As String

		ReadOnly Property SwJvmUID As String

		ReadOnly Property SwEnvironmentInfo As IDictionary(Of String, String)

		ReadOnly Property HwJvmAvailableProcessors As Integer

		ReadOnly Property HwNumDevices As Integer

		ReadOnly Property HwJvmMaxMemory As Long

		ReadOnly Property HwOffHeapMaxMemory As Long

		ReadOnly Property HwDeviceTotalMemory As Long()

		ReadOnly Property HwDeviceDescription As String()

		ReadOnly Property HwHardwareUID As String

		ReadOnly Property ModelClassName As String

		ReadOnly Property ModelConfigJson As String

		ReadOnly Property ModelParamNames As String()

		ReadOnly Property ModelNumLayers As Integer

		ReadOnly Property ModelNumParams As Long

	End Interface

End Namespace