Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic
import static org.nd4j.systeminfo.GPUInfo.fGpu
Imports BinaryByteUnit = com.jakewharton.byteunits.BinaryByteUnit
Imports FileUtils = org.apache.commons.io.FileUtils
Imports SystemUtils = org.apache.commons.lang3.SystemUtils
Imports ExceptionUtils = org.apache.commons.lang3.exception.ExceptionUtils
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports Nd4jEnvironment = org.nd4j.linalg.api.environment.Nd4jEnvironment
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports VersionCheck = org.nd4j.versioncheck.VersionCheck
Imports VersionInfo = org.nd4j.versioncheck.VersionInfo
Imports OperatingSystem = oshi.software.os.OperatingSystem

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

Namespace org.nd4j.systeminfo

	Public Class SystemInfo

		Private Shared Sub appendField(ByVal sb As StringBuilder, ByVal name As String, ByVal value As Object)
			sb.Append(name).Append(": ").Append(value.ToString()).Append(vbLf)
		End Sub

		Private Shared Sub appendProperty(ByVal sb As StringBuilder, ByVal name As String, ByVal [property] As String)
			appendField(sb, name, System.getProperty([property]))
		End Sub

		Private Shared Sub appendHeader(ByVal sb As StringBuilder, ByVal name As String)
			sb.Append(vbLf & vbLf & "---------------").Append(name).Append("---------------" & vbLf & vbLf)
		End Sub

		Private Const FORMAT As String = "%-40s%s"

		Public Shared Function f(ByVal s1 As String, ByVal o As Object) As String
			Return String.format(FORMAT, s1, (If(o Is Nothing, "null", o.ToString()))) & vbLf
		End Function

		Public Shared Function fBytes(ByVal bytes As Long) As String
			Dim s As String = BinaryByteUnit.format(bytes, "#.00")
'JAVA TO VB CONVERTER NOTE: The variable format was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim format_Conflict As String = "%10s"
			s = String.format(format_Conflict, s)
			If bytes >= 1024 Then
				s &= " (" & bytes & ")"
			End If
			Return s
		End Function

		Public Shared Function fBytes(ByVal s1 As String, ByVal bytes As Long) As String
			Dim s As String = fBytes(bytes)
			Return f(s1, s)
		End Function


		Private Shared Sub appendCUDAInfo(ByVal sb As StringBuilder, ByVal isWindows As Boolean)


			sb.Append("Nvidia-smi:" & vbLf)

			Try
				Dim pb As New ProcessBuilder("nvidia-smi")
				appendOutput(sb, pb)
			Catch e As IOException
				sb.Append("nvidia-smi run failed.")

				If isWindows Then
					sb.Append("  Trying in C:\Program Files\NVIDIA Corporation\NVSMI" & vbLf)

					Try
						Dim pb As New ProcessBuilder("C:\Program Files\NVIDIA Corporation\NVSMI\nvidia-smi.exe")
						appendOutput(sb, pb)
					Catch e1 As IOException
						sb.Append("C:\Program Files\NVIDIA Corporation\NVSMI\nvidia-smi run failed" & vbLf)
						sb.Append(e1.Message)
					End Try
				Else
					sb.Append(vbLf)
				End If

			End Try

			sb.Append(vbLf & "nvcc --version:" & vbLf)
			Try

				Dim pb As New ProcessBuilder("nvcc", "--version")
				appendOutput(sb, pb)
			Catch e As IOException
				sb.Append("nvcc --version run failed.")
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void appendOutput(StringBuilder sb, ProcessBuilder pb) throws java.io.IOException
		Private Shared Sub appendOutput(ByVal sb As StringBuilder, ByVal pb As ProcessBuilder)
			pb.redirectErrorStream(True)
			pb.redirectOutput()

			Dim p As Process = pb.start()
			Using isr As New StreamReader(p.getInputStream())
				Using reader As New StreamReader(isr)
					Dim line As String = Nothing
					line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
					Do While line IsNot Nothing
						sb.Append(line)
						sb.Append(System.getProperty("line.separator"))
							line = reader.ReadLine()
					Loop
				End Using
			End Using
			sb.Append(vbLf)
		End Sub


		''' <summary>
		''' Gets system info in a string
		''' </summary>
		Public Shared ReadOnly Property SystemInfo As String
			Get
				Dim sb As New StringBuilder()
    
				'nd4j info
				appendHeader(sb, "ND4J Info")
    
				Dim pair As Pair(Of String, String) = inferVersion()
				sb.Append(f("Deeplearning4j Version", (If(pair.First Is Nothing, "<could not determine>", pair.First))))
				sb.Append(f("Deeplearning4j CUDA", (If(pair.Second Is Nothing, "<not present>", pair.Second))))
    
				sb.Append(vbLf)
    
				Dim isCUDA As Boolean = False
    
				Try
					appendField(sb, "Nd4j Backend", Nd4j.Backend.GetType().Name)
    
					Dim props As Properties = Nd4j.Executioner.EnvironmentInformation
    
	'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim memory As Double = (CType(props.get("memory.available"), Long?)).Value / CDbl(1024) / 1024 / 1024
					Dim fm As String = String.Format("{0:F1}", memory)
					sb.Append("Backend used: [").Append(props.get("backend")).Append("]; OS: [").Append(props.get("os")).Append("]" & vbLf)
					sb.Append("Cores: [").Append(props.get("cores")).Append("]; Memory: [").Append(fm).Append("GB];" & vbLf)
					sb.Append("Blas vendor: [").Append(props.get("blas.vendor")).Append("]" & vbLf)
    
					If Nd4j.Executioner.GetType().Name.Equals("CudaExecutioner") Then
						isCUDA = True
    
						Dim devicesList As IList(Of IDictionary(Of String, Object)) = CType(props.get(Nd4jEnvironment.CUDA_DEVICE_INFORMATION_KEY), IList(Of IDictionary(Of String, Object)))
						For Each dev As IDictionary(Of String, Object) In devicesList
							sb.Append("Device Name: [").Append(dev(Nd4jEnvironment.CUDA_DEVICE_NAME_KEY)).Append("]; ").Append("CC: [").Append(dev(Nd4jEnvironment.CUDA_DEVICE_MAJOR_VERSION_KEY)).Append(".").Append(dev(Nd4jEnvironment.CUDA_DEVICE_MINOR_VERSION_KEY)).Append("]; Total/free memory: [").Append(dev(Nd4jEnvironment.CUDA_TOTAL_MEMORY_KEY)).Append("]").Append(vbLf)
						Next dev
					End If
    
					sb.Append(vbLf & "Executor Properties:" & vbLf)
    
					For Each prop As KeyValuePair(Of Object, Object) In props.entrySet()
						sb.Append(prop.Key.ToString()).Append("=").Append(prop.Value).Append(vbLf)
					Next prop
				Catch e As Exception
					sb.Append("Could not get ND4J info" & vbLf)
					sb.Append("Exception: ").Append(e.Message).Append(vbLf)
					sb.Append(ExceptionUtils.getStackTrace(e)).Append(vbLf & vbLf)
				End Try
    
				'hardware info
				appendHeader(sb, "Hardware Info")
    
				appendField(sb, "Available processors (cores)", Runtime.getRuntime().availableProcessors())
    
				Dim sys As New oshi.SystemInfo()
				Dim os As OperatingSystem = sys.getOperatingSystem()
				Dim procName As String = sys.getHardware().getProcessor().getName()
				Dim totalMem As Long = sys.getHardware().getMemory().getTotal()
    
				sb.Append(f("Operating System", os.getManufacturer() & " " & os.getFamily() & " " & os.getVersion().getVersion()))
				sb.Append(f("CPU", procName))
				sb.Append(f("CPU Cores - Physical", sys.getHardware().getProcessor().getPhysicalProcessorCount()))
				sb.Append(f("CPU Cores - Logical", sys.getHardware().getProcessor().getLogicalProcessorCount()))
				sb.Append(fBytes("Total System Memory", totalMem))
    
				sb.Append(vbLf)
    
				Dim hasGPUs As Boolean = False
    
				Dim loader As ServiceLoader(Of GPUInfoProvider) = ND4JClassLoading.loadService(GetType(GPUInfoProvider))
				Dim iter As IEnumerator(Of GPUInfoProvider) = loader.GetEnumerator()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If iter.hasNext() Then
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim gpus As IList(Of GPUInfo) = iter.next().getGPUs()
    
					sb.Append(f("Number of GPUs Detected", gpus.Count))
    
					If gpus.Count > 0 Then
						hasGPUs = True
					End If
    
					sb.Append(String.format(fGpu, "Name", "CC", "Total Memory", "Used Memory", "Free Memory")).Append(vbLf)
    
					For Each gpuInfo As GPUInfo In gpus
						sb.Append(gpuInfo).Append(vbLf)
					Next gpuInfo
				Else
					sb.Append("GPU Provider not found (are you missing nd4j-native?)")
				End If
    
				appendHeader(sb, "CUDA Info")
    
				If Not isCUDA Then
					sb.Append("NOT USING CUDA Nd4j" & vbLf)
    
					If hasGPUs Then
						sb.Append("GPUs detected, trying to list CUDA info anyways" & vbLf)
					End If
				End If
    
				If isCUDA OrElse hasGPUs Then
					appendCUDAInfo(sb, SystemUtils.IS_OS_WINDOWS)
				End If
    
				'OS info
				appendHeader(sb, "OS Info")
    
				appendProperty(sb, "OS", "os.name")
				appendProperty(sb, "Version","os.version")
				appendProperty(sb, "Arch","os.arch")
    
				'memory settings
				appendHeader(sb, "Memory Settings")
    
				appendField(sb, "Free memory (bytes)", Runtime.getRuntime().freeMemory())
    
				Dim maxMemory As Long = Runtime.getRuntime().maxMemory()
				appendField(sb, "Maximum memory (bytes)", (If(maxMemory = Long.MaxValue, "No Limit", maxMemory)))
    
				appendField(sb, "Total memory available to JVM (bytes)", Runtime.getRuntime().totalMemory())
    
				sb.Append(vbLf)
    
				Dim xmx As Long = Runtime.getRuntime().maxMemory()
				Dim jvmTotal As Long = Runtime.getRuntime().totalMemory()
				Dim javacppMaxPhys As Long = Pointer.maxPhysicalBytes()
				Dim javacppMaxBytes As Long = Pointer.maxBytes()
				Dim javacppCurrPhys As Long = Pointer.physicalBytes()
				Dim javacppCurrBytes As Long = Pointer.totalBytes()
				sb.Append(fBytes("JVM Memory: XMX", xmx)).Append(fBytes("JVM Memory: current", jvmTotal)).Append(fBytes("JavaCPP Memory: Max Bytes", javacppMaxBytes)).Append(fBytes("JavaCPP Memory: Max Physical", javacppMaxPhys)).Append(fBytes("JavaCPP Memory: Current Bytes", javacppCurrBytes)).Append(fBytes("JavaCPP Memory: Current Physical", javacppCurrPhys))
				Dim periodicGcEnabled As Boolean = Nd4j.MemoryManager.PeriodicGcActive
				Dim autoGcWindow As Long = Nd4j.MemoryManager.AutoGcWindow
				sb.Append(f("Periodic GC Enabled", periodicGcEnabled))
				If periodicGcEnabled Then
					sb.Append(f("Periodic GC Frequency", autoGcWindow & " ms"))
				End If
    
				' Workspaces info
    
				appendHeader(sb, "Workspace Information")
				Dim allWs As IList(Of MemoryWorkspace) = Nd4j.WorkspaceManager.getAllWorkspacesForCurrentThread()
				sb.Append(f("Workspaces: # for current thread", (If(allWs Is Nothing, 0, allWs.Count))))
				'sb.append(f("Workspaces: # for all threads", allWs.size()));      //TODO
				Dim totalWsSize As Long = 0
				If allWs IsNot Nothing AndAlso allWs.Count > 0 Then
					sb.Append("Current thread workspaces:" & vbLf)
					'Name, open, size, currently allocated
					Dim wsFormat As String = "  %-26s%-12s%-30s%-20s"
					sb.Append(String.format(wsFormat, "Name", "State", "Size", "# Cycles")).Append(vbLf)
					For Each ws As MemoryWorkspace In allWs
						totalWsSize += ws.CurrentSize
						Dim numCycles As Long = ws.GenerationId
						sb.Append(String.format(wsFormat, ws.Id, (If(ws.ScopeActive, "OPEN", "CLOSED")), fBytes(ws.CurrentSize), numCycles.ToString())).Append(vbLf)
					Next ws
				End If
				sb.Append(fBytes("Workspaces total size", totalWsSize))
    
				'JVM info
				appendHeader(sb, "JVM Info")
    
				appendProperty(sb, "Runtime Name", "java.runtime.name")
				appendProperty(sb, "Java Version", "java.version")
				appendProperty(sb, "Runtime Version", "java.runtime.version")
				appendProperty(sb, "Vendor", "java.vm.vendor")
				appendProperty(sb, "Vendor Url", "java.vendor.url")
    
				sb.Append(vbLf)
    
				appendProperty(sb, "VM Name", "java.vm.name")
				appendProperty(sb, "VM Version", "java.vm.version")
				appendProperty(sb, "VM Specification Name", "java.vm.specification.name")
    
				sb.Append(vbLf)
    
				appendProperty(sb, "Library Path", "java.library.path")
    
				appendHeader(sb, "Classpath")
    
				Dim urlClassLoader As URLClassLoader = Nothing
    
				If TypeOf ND4JClassLoading.Nd4jClassloader Is URLClassLoader Then
					urlClassLoader = CType(ND4JClassLoading.Nd4jClassloader, URLClassLoader)
				ElseIf TypeOf ClassLoader.getSystemClassLoader() Is URLClassLoader Then
					urlClassLoader = CType(ClassLoader.getSystemClassLoader(), URLClassLoader)
				ElseIf TypeOf GetType(SystemInfo).getClassLoader() Is URLClassLoader Then
					urlClassLoader = CType(GetType(SystemInfo).getClassLoader(), URLClassLoader)
				ElseIf TypeOf Thread.CurrentThread.getContextClassLoader() Is URLClassLoader Then
					urlClassLoader = CType(Thread.CurrentThread.getContextClassLoader(), URLClassLoader)
				Else
					sb.Append("Can't cast class loader to URLClassLoader" & vbLf)
				End If
    
				If urlClassLoader IsNot Nothing Then
					For Each url As URL In urlClassLoader.getURLs()
						sb.Append(url.getFile()).Append(vbLf)
					Next url
				Else
					sb.Append("Using System property java.class.path" & vbLf)
					Dim cps() As String = System.getProperty("java.class.path").Split(";")
					For Each c As String In cps
						sb.Append(c).Append(vbLf)
					Next c
				End If
    
				'launch command
				appendHeader(sb, "Launch Command")
    
				Try
					' only works on Oracle JVMs
					appendProperty(sb, "Launch Command", "sun.java.command")
				Catch e As Exception
					appendField(sb, "Launch Command", "Not available on this JVM")
				End Try
    
				Dim inputArguments As IList(Of String) = ManagementFactory.getRuntimeMXBean().getInputArguments()
				appendField(sb, "JVM Arguments", inputArguments)
    
    
				'system properties
				appendHeader(sb, "System Properties")
    
				Dim props As Properties = System.getProperties()
				For Each prop As KeyValuePair(Of Object, Object) In props.entrySet()
					If prop.Key.ToString().Equals("line.separator") Then
						sb.Append(prop.Key.ToString()).Append("=").Append(prop.Value.ToString().Replace("\", "\\")).Append(vbLf)
					Else
						sb.Append(prop.Key.ToString()).Append("=").Append(prop.Value).Append(vbLf)
					End If
				Next prop
    
    
				'enviroment variables
				appendHeader(sb, "Environment Variables")
    
				Dim env As IDictionary(Of String, String) = System.getenv()
    
				For Each key As String In env.Keys
					sb.Append(key).Append("=").Append(env(key)).Append(vbLf)
				Next key
    
				Return sb.ToString()
			End Get
		End Property

		''' <summary>
		''' Writes system info to the given file
		''' </summary>
		Public Shared Sub writeSystemInfo(ByVal file As File)
			Try
				file.createNewFile()
				FileUtils.writeStringToFile(file, SystemInfo)
			Catch e As IOException
				Throw New Exception("IOException:" & e.Message, e)
			End Try
		End Sub

		''' <summary>
		''' Prints system info
		''' </summary>
		Public Shared Sub printSystemInfo()
			Console.WriteLine(SystemInfo)
		End Sub

		Public Shared Function inferVersion() As Pair(Of String, String)
			Dim vi As IList(Of VersionInfo) = VersionCheck.getVersionInfos()

			Dim dl4jVersion As String = Nothing
			Dim dl4jCudaArtifact As String = Nothing
			For Each v As VersionInfo In vi
				If "org.deeplearning4j".Equals(v.getGroupId()) AndAlso "deeplearning4j-core".Equals(v.getArtifactId()) Then
					Dim version As String = v.getBuildVersion()
					If version.Contains("SNAPSHOT") Then
						dl4jVersion = version & " (" & v.getCommitIdAbbrev() & ")"
					End If
					dl4jVersion = version
				ElseIf "org.deeplearning4j".Equals(v.getGroupId()) AndAlso v.getArtifactId() IsNot Nothing AndAlso v.getArtifactId().contains("deeplearning4j-cuda") Then
					dl4jCudaArtifact = v.getArtifactId()
				End If

			Next v

			Return New Pair(Of String, String)(dl4jVersion, dl4jCudaArtifact)
		End Function
	End Class

End Namespace