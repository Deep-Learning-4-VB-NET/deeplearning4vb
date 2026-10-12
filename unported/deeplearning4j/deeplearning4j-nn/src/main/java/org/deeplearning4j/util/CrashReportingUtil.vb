Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic
import static org.deeplearning4j.nn.conf.inputs.InputType.inferInputType
import static org.deeplearning4j.nn.conf.inputs.InputType.inferInputTypes
import static org.nd4j.systeminfo.SystemInfo.inferVersion
Imports BinaryByteUnit = com.jakewharton.byteunits.BinaryByteUnit
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ExceptionUtils = org.apache.commons.lang3.exception.ExceptionUtils
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DL4JSystemProperties = org.deeplearning4j.common.config.DL4JSystemProperties
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Model = org.deeplearning4j.nn.api.Model
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports GraphIndices = org.deeplearning4j.nn.graph.util.GraphIndices
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.graph.vertex.impl.LayerVertex
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports org.deeplearning4j.nn.updater
Imports UpdaterBlock = org.deeplearning4j.nn.updater.UpdaterBlock
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports BaseOptimizer = org.deeplearning4j.optimize.solvers.BaseOptimizer
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports SystemInfo = oshi.SystemInfo
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

Namespace org.deeplearning4j.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CrashReportingUtil
	Public Class CrashReportingUtil
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static boolean crashDumpsEnabled = true;
'JAVA TO VB CONVERTER NOTE: The field crashDumpsEnabled was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared crashDumpsEnabled_Conflict As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static java.io.File crashDumpRootDirectory;
		Private Shared crashDumpRootDirectory As File

		Shared Sub New()
			Dim s As String = System.getProperty(DL4JSystemProperties.CRASH_DUMP_ENABLED_PROPERTY)
			If s IsNot Nothing AndAlso s.Length > 0 Then
				crashDumpsEnabled_Conflict = Boolean.Parse(s)
			End If

			s = System.getProperty(DL4JSystemProperties.CRASH_DUMP_OUTPUT_DIRECTORY_PROPERTY)
			Dim setDir As Boolean = False
			If s IsNot Nothing AndAlso s.Length > 0 Then
				Try
					Dim f As New File(s)
					crashDumpOutputDirectory(f)
					setDir = True
					log.debug("Crash dump output directory set to: {}", f.getAbsolutePath())
				Catch e As Exception
					log.warn("Error setting crash dump output directory to value: {}", s, e)
				End Try
			End If
			If Not setDir Then
				crashDumpOutputDirectory(Nothing)
			End If
		End Sub

		Private Sub New()
		End Sub

		''' <summary>
		''' Method that can be used to enable or disable memory crash reporting. Memory crash reporting is enabled by default.
		''' </summary>
		Public Shared Sub crashDumpsEnabled(ByVal enabled As Boolean)
			crashDumpsEnabled_Conflict = enabled
		End Sub

		''' <summary>
		''' Method that can be use to customize the output directory for memory crash reporting. By default,
		''' the current working directory will be used.
		''' </summary>
		''' <param name="rootDir"> Root directory to use for crash reporting. If null is passed, the current working directory
		'''                will be used </param>
		Public Shared Sub crashDumpOutputDirectory(ByVal rootDir As File)
			If rootDir Is Nothing Then
				Dim userDir As String = System.getProperty("user.dir")
				If userDir Is Nothing Then
					userDir = ""
				End If
				crashDumpRootDirectory = New File(userDir)
				Return
			End If
			crashDumpRootDirectory = rootDir
		End Sub

		''' <summary>
		''' Generate and write the crash dump to the crash dump root directory (by default, the working directory).
		''' Naming convention for crash dump files: "dl4j-memory-crash-dump-<timestamp>_<thread-id>.txt"
		''' 
		''' </summary>
		''' <param name="net">   Net to generate the crash dump for. May not be null </param>
		''' <param name="e">     Throwable/exception. Stack trace will be included in the network output </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeMemoryCrashDump(@NonNull Model net, @NonNull Throwable e)
		Public Shared Sub writeMemoryCrashDump(ByVal net As Model, ByVal e As Exception)
			If Not crashDumpsEnabled_Conflict Then
				Return
			End If

			Dim now As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim tid As Long = Thread.CurrentThread.getId() 'Also add thread ID to avoid name clashes (parallel wrapper, etc)
			Dim threadName As String = Thread.CurrentThread.getName()
			crashDumpRootDirectory.mkdirs()
			Dim f As New File(crashDumpRootDirectory, "dl4j-memory-crash-dump-" & now & "_" & tid & ".txt")
			Dim sb As New StringBuilder()

			Dim sdf As New SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS")
			sb.Append("Deeplearning4j OOM Exception Encountered for ").Append(net.GetType().Name).Append(vbLf).Append(CrashReportingUtil.f("Timestamp: ", sdf.format(now))).Append(CrashReportingUtil.f("Thread ID", tid)).Append(CrashReportingUtil.f("Thread Name", threadName)).Append(vbLf & vbLf)

			sb.Append("Stack Trace:" & vbLf).Append(ExceptionUtils.getStackTrace(e))

			Try
				sb.Append(vbLf & vbLf)
				sb.Append(generateMemoryStatus(net, -1, DirectCast(Nothing, InputType())))
			Catch t As Exception
				sb.Append("<Error generating network memory status information section>").Append(ExceptionUtils.getStackTrace(t))
			End Try

			Dim toWrite As String = sb.ToString()
			Try
				FileUtils.writeStringToFile(f, toWrite)
			Catch e2 As IOException
				log.error("Error writing memory crash dump information to disk: {}", f.getAbsolutePath(), e2)
			End Try

			log.error(">>> Out of Memory Exception Detected. Memory crash dump written to: {}", f.getAbsolutePath())
			log.warn("Memory crash dump reporting can be disabled with CrashUtil.crashDumpsEnabled(false) or using system " & "property -D" & DL4JSystemProperties.CRASH_DUMP_ENABLED_PROPERTY & "=false")
			log.warn("Memory crash dump reporting output location can be set with CrashUtil.crashDumpOutputDirectory(File) or using system " & "property -D" & DL4JSystemProperties.CRASH_DUMP_OUTPUT_DIRECTORY_PROPERTY & "=<path>")
		End Sub

		Private Const FORMAT As String = "%-40s%s"

		''' <summary>
		''' Generate memory/system report as a String, for the specified network.
		''' Includes informatioun about the system, memory configuration, network, etc.
		''' </summary>
		''' <param name="net">   Net to generate the report for </param>
		''' <returns> Report as a String </returns>
		Public Shared Function generateMemoryStatus(ByVal net As Model, ByVal minibatch As Integer, ParamArray ByVal inputTypes() As InputType) As String
			Dim mln As MultiLayerNetwork = Nothing
			Dim cg As ComputationGraph = Nothing
			Dim isMLN As Boolean
			If TypeOf net Is MultiLayerNetwork Then
				mln = DirectCast(net, MultiLayerNetwork)
				isMLN = True
			Else
				cg = DirectCast(net, ComputationGraph)
				isMLN = False
			End If

			Dim sb As StringBuilder = genericMemoryStatus()

			Dim bytesPerElement As Integer
			If(Select Case isMLN, mln.params().dataType(), cg.params().dataType())
				Case [DOUBLE]
					bytesPerElement = 8
				Case FLOAT
					bytesPerElement = 4
				Case HALF
					bytesPerElement = 2
				Case Else
					bytesPerElement = 0 'TODO
			End Select

			sb.Append(vbLf & "----- Workspace Information -----" & vbLf)
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
			Dim helperWorkspaces As IDictionary(Of String, Pointer)
			If isMLN Then
				helperWorkspaces = mln.getHelperWorkspaces()
			Else
				helperWorkspaces = cg.getHelperWorkspaces()
			End If
			If helperWorkspaces IsNot Nothing AndAlso helperWorkspaces.Count > 0 Then
				Dim header As Boolean = False
				For Each e As KeyValuePair(Of String, Pointer) In helperWorkspaces.SetOfKeyValuePairs()
					Dim p As Pointer = e.Value
					If p Is Nothing Then
						Continue For
					End If
					If Not header Then
						sb.Append("Helper Workspaces" & vbLf)
						header = True
					End If
					sb.Append("  ").Append(fBytes(e.Key, p.capacity()))
				Next e
			End If

			Dim sumMem As Long = 0
			Dim nParams As Long = net.params().length()
			sb.Append(vbLf & "----- Network Information -----" & vbLf).Append(f("Network # Parameters", nParams)).Append(fBytes("Parameter Memory", bytesPerElement * nParams))
			Dim flattenedGradients As INDArray
			If isMLN Then
				flattenedGradients = mln.getFlattenedGradients()
			Else
				flattenedGradients = cg.getFlattenedGradients()
			End If
			If flattenedGradients Is Nothing Then
				sb.Append(f("Parameter Gradients Memory", "<not allocated>"))
			Else
				sumMem += (flattenedGradients.length() * bytesPerElement)
				sb.Append(fBytes("Parameter Gradients Memory", bytesPerElement * flattenedGradients.length()))
			End If
				'Updater info
			Dim u As BaseMultiLayerUpdater
			If isMLN Then
				u = DirectCast(mln.getUpdater(False), BaseMultiLayerUpdater)
			Else
				u = cg.getUpdater(False)
			End If
			Dim updaterClasses As ISet(Of String) = New HashSet(Of String)()
			If u Is Nothing Then
				sb.Append(f("Updater","<not initialized>"))
			Else
				Dim stateArr As INDArray = u.getStateViewArray()
				Dim stateArrLength As Long = (If(stateArr Is Nothing, 0, stateArr.length()))
				sb.Append(f("Updater Number of Elements", stateArrLength))
				sb.Append(fBytes("Updater Memory", stateArrLength * bytesPerElement))
				sumMem += stateArrLength * bytesPerElement

				Dim blocks As IList(Of UpdaterBlock) = u.getUpdaterBlocks()
				For Each ub As UpdaterBlock In blocks
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					updaterClasses.Add(ub.GradientUpdater.GetType().FullName)
				Next ub

				sb.Append("Updater Classes:").Append(vbLf)
				For Each s As String In updaterClasses
					sb.Append("  ").Append(s).Append(vbLf)
				Next s
			End If
			sb.Append(fBytes("Params + Gradient + Updater Memory", sumMem))
				'Iter/epoch
			sb.Append(f("Iteration Count", BaseOptimizer.getIterationCount(net)))
			sb.Append(f("Epoch Count", BaseOptimizer.getEpochCount(net)))

				'Workspaces, backprop type, layer info, activation info, helper info
			If isMLN Then
				sb.Append(f("Backprop Type", mln.LayerWiseConfigurations.getBackpropType()))
				If mln.LayerWiseConfigurations.getBackpropType() = BackpropType.TruncatedBPTT Then
					sb.Append(f("TBPTT Length", mln.LayerWiseConfigurations.getTbpttFwdLength() & "/" & mln.LayerWiseConfigurations.getTbpttBackLength()))
				End If
				sb.Append(f("Workspace Mode: Training", mln.LayerWiseConfigurations.getTrainingWorkspaceMode()))
				sb.Append(f("Workspace Mode: Inference", mln.LayerWiseConfigurations.getInferenceWorkspaceMode()))
				appendLayerInformation(sb, mln.Layers, bytesPerElement)
				appendHelperInformation(sb, mln.Layers)
				appendActivationShapes(mln, (If(inputTypes Is Nothing OrElse inputTypes.Length = 0, Nothing, inputTypes(0))), minibatch, sb, bytesPerElement)
			Else
				sb.Append(f("Backprop Type", cg.Configuration.getBackpropType()))
				If cg.Configuration.getBackpropType() = BackpropType.TruncatedBPTT Then
					sb.Append(f("TBPTT Length", cg.Configuration.getTbpttFwdLength() & "/" & cg.Configuration.getTbpttBackLength()))
				End If
				sb.Append(f("Workspace Mode: Training", cg.Configuration.getTrainingWorkspaceMode()))
				sb.Append(f("Workspace Mode: Inference", cg.Configuration.getInferenceWorkspaceMode()))
				appendLayerInformation(sb, cg.Layers, bytesPerElement)
				appendHelperInformation(sb, cg.Layers)
				appendActivationShapes(cg, sb, bytesPerElement)
			End If

			'Listener info:
			Dim listeners As ICollection(Of TrainingListener)
			If isMLN Then
				listeners = mln.getListeners()
			Else
				listeners = cg.getListeners()
			End If

			sb.Append(vbLf & "----- Network Training Listeners -----" & vbLf)
			sb.Append(f("Number of Listeners", (If(listeners Is Nothing, 0, listeners.Count))))
			Dim lCount As Integer = 0
			If listeners IsNot Nothing AndAlso listeners.Count > 0 Then
				For Each tl As TrainingListener In listeners
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: sb.append(f("Listener " + (lCount++), tl));
					sb.Append(f("Listener " & (lCount), tl))
						lCount += 1
				Next tl
			End If

			Return sb.ToString()
		End Function

		Private Shared Function f(ByVal s1 As String, ByVal o As Object) As String
			Return String.format(FORMAT, s1, (If(o Is Nothing, "null", o.ToString()))) & vbLf
		End Function

		Private Shared Function fBytes(ByVal bytes As Long) As String
			Dim s As String = BinaryByteUnit.format(bytes, "#.00")
'JAVA TO VB CONVERTER NOTE: The variable format was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim format_Conflict As String = "%10s"
			s = String.format(format_Conflict, s)
			If bytes >= 1024 Then
				s &= " (" & bytes & ")"
			End If
			Return s
		End Function

		Private Shared Function fBytes(ByVal s1 As String, ByVal bytes As Long) As String
			Dim s As String = fBytes(bytes)
			Return f(s1, s)
		End Function

		Private Shared Function genericMemoryStatus() As StringBuilder

			Dim sb As New StringBuilder()

			sb.Append("========== Memory Information ==========" & vbLf)
			sb.Append("----- Version Information -----" & vbLf)
			Dim pair As Pair(Of String, String) = inferVersion()
			sb.Append(f("Deeplearning4j Version", (If(pair.First Is Nothing, "<could not determine>", pair.First))))
			sb.Append(f("Deeplearning4j CUDA", (If(pair.Second Is Nothing, "<not present>", pair.Second))))

			sb.Append(vbLf & "----- System Information -----" & vbLf)
			Dim sys As New SystemInfo()
			Dim os As OperatingSystem = sys.getOperatingSystem()
			Dim procName As String = sys.getHardware().getProcessor().getName()
			Dim totalMem As Long = sys.getHardware().getMemory().getTotal()

			sb.Append(f("Operating System", os.getManufacturer() & " " & os.getFamily() & " " & os.getVersion().getVersion()))
			sb.Append(f("CPU", procName))
			sb.Append(f("CPU Cores - Physical", sys.getHardware().getProcessor().getPhysicalProcessorCount()))
			sb.Append(f("CPU Cores - Logical", sys.getHardware().getProcessor().getLogicalProcessorCount()))
			sb.Append(fBytes("Total System Memory", totalMem))

			Dim nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
			Dim nDevices As Integer = nativeOps.AvailableDevices
			If nDevices > 0 Then
				sb.Append(f("Number of GPUs Detected", nDevices))
				'Name CC, Total memory, current memory, free memory
				Dim fGpu As String = "  %-30s %-5s %24s %24s %24s"
				sb.Append(String.format(fGpu, "Name", "CC", "Total Memory", "Used Memory", "Free Memory")).Append(vbLf)
				For i As Integer = 0 To nDevices - 1
					Try
						Dim name As String = nativeOps.getDeviceName(i)
						Dim total As Long = nativeOps.getDeviceTotalMemory(i)
						Dim free As Long = nativeOps.getDeviceFreeMemory(i)
						Dim current As Long = total - free
						Dim major As Integer = nativeOps.getDeviceMajor(i)
						Dim minor As Integer = nativeOps.getDeviceMinor(i)

						sb.Append(String.format(fGpu, name, major & "." & minor, fBytes(total), fBytes(current), fBytes(free))).Append(vbLf)
					Catch e As Exception
						sb.Append("  Failed to get device info for device ").Append(i).Append(vbLf)
					End Try
				Next i
			End If

			sb.Append(vbLf & "----- ND4J Environment Information -----" & vbLf)
			sb.Append(f("Data Type", Nd4j.dataType()))
			Dim p As Properties = Nd4j.Executioner.EnvironmentInformation
			For Each s As String In p.stringPropertyNames()
				sb.Append(f(s, p.get(s)))
			Next s

			sb.Append(vbLf & "----- Memory Configuration -----" & vbLf)

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




			Return sb
		End Function

		Private Shared Sub appendLayerInformation(ByVal sb As StringBuilder, ByVal layers() As Layer, ByVal bytesPerElement As Integer)
			Dim layerClasses As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			For Each l As Layer In layers
				If Not layerClasses.ContainsKey(l.GetType().Name) Then
					layerClasses(l.GetType().Name) = 0
				End If
				layerClasses(l.GetType().Name) = layerClasses(l.GetType().Name) + 1
			Next l

			Dim l As IList(Of String) = New List(Of String)(layerClasses.Keys)
			l.Sort()
			sb.Append(f("Number of Layers", layers.Length))
			sb.Append("Layer Counts" & vbLf)
			For Each s As String In l
				sb.Append("  ").Append(f(s, layerClasses(s)))
			Next s
			sb.Append("Layer Parameter Breakdown" & vbLf)
'JAVA TO VB CONVERTER NOTE: The variable format was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim format_Conflict As String = "  %-3s %-20s %-20s %-20s %-20s"
			sb.Append(String.format(format_Conflict, "Idx", "Name", "Layer Type", "Layer # Parameters", "Layer Parameter Memory")).Append(vbLf)
			For Each layer As Layer In layers
				Dim numParams As Long = layer.numParams()
				sb.Append(String.format(format_Conflict, layer.Index, layer.conf().getLayer().getLayerName(), layer.GetType().Name, numParams.ToString(), fBytes(numParams * bytesPerElement))).Append(vbLf)
			Next layer

		End Sub

		Private Shared Sub appendHelperInformation(ByVal sb As StringBuilder, ByVal layers() As Layer)
			sb.Append(vbLf & "----- Layer Helpers - Memory Use -----" & vbLf)

			Dim helperCount As Integer = 0
			Dim helperWithMemCount As Long = 0L
			Dim totalHelperMem As Long = 0L

			'Layer index, layer name, layer class, helper class, total memory, breakdown
'JAVA TO VB CONVERTER NOTE: The variable format was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim format_Conflict As String = "%-3s %-20s %-25s %-30s %-12s %s"
			Dim header As Boolean = False
			For Each l As Layer In layers
				Dim h As LayerHelper = l.Helper
				If h Is Nothing Then
					Continue For
				End If

				helperCount += 1
				Dim mem As IDictionary(Of String, Long) = h.helperMemoryUse()
				If mem Is Nothing OrElse mem.Count = 0 Then
					Continue For
				End If
				helperWithMemCount += 1

				Dim layerTotal As Long = 0
				For Each m As Long? In mem.Values
					layerTotal += m
				Next m

				Dim idx As Integer = l.Index
				Dim layerName As String = l.conf().getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = idx.ToString()
				End If


				If Not header Then
					sb.Append(String.format(format_Conflict, "#", "Layer Name", "Layer Class", "Helper Class", "Total Memory", "Memory Breakdown")).Append(vbLf)
					header = True
				End If

				sb.Append(String.format(format_Conflict, idx, layerName, l.GetType().Name, h.GetType().Name, fBytes(layerTotal), mem.ToString())).Append(vbLf)

				totalHelperMem += layerTotal
			Next l

			sb.Append(f("Total Helper Count", helperCount))
			sb.Append(f("Helper Count w/ Memory", helperWithMemCount))
			sb.Append(fBytes("Total Helper Persistent Memory Use", totalHelperMem))
		End Sub

		Private Shared Sub appendActivationShapes(ByVal net As MultiLayerNetwork, ByVal inputType As InputType, ByVal minibatch As Integer, ByVal sb As StringBuilder, ByVal bytesPerElement As Integer)
			Dim input As INDArray = net.Input
			If input Is Nothing AndAlso inputType Is Nothing Then
				Return
			End If

			sb.Append(vbLf & "----- Network Activations: Inferred Activation Shapes -----" & vbLf)
			If inputType Is Nothing Then
				inputType = inferInputType(input)
				If minibatch <= 0 Then
					minibatch = CInt(input.size(0))
				End If
			End If

			Dim inputShape() As Long
			If input IsNot Nothing Then
				inputShape = input.shape()
			Else
				inputShape = inputType.getShape(True)
				inputShape(0) = minibatch
			End If

			sb.Append(f("Current Minibatch Size", minibatch))
			sb.Append(f("Input Shape", Arrays.toString(inputShape)))
			Dim inputTypes As IList(Of InputType) = net.LayerWiseConfigurations.getLayerActivationTypes(inputType)
'JAVA TO VB CONVERTER NOTE: The variable format was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim format_Conflict As String = "%-3s %-20s %-20s %-42s %-20s %-12s %-12s"
			sb.Append(String.format(format_Conflict, "Idx", "Name", "Layer Type", "Activations Type", "Activations Shape", "# Elements", "Memory")).Append(vbLf)
			Dim layers() As Layer = net.Layers
			Dim totalActivationBytes As Long = 0
			Dim last As Long = 0
			For i As Integer = 0 To inputTypes.Count - 1
				Dim shape() As Long = inputTypes(i).getShape(True)
				If shape(0) <= 0 Then
					shape(0) = minibatch
				End If
				Dim numElements As Long = ArrayUtil.prodLong(shape)
				Dim bytes As Long = numElements*bytesPerElement
				If bytes < 0 Then
					bytes = 0
				End If
				totalActivationBytes += bytes
				sb.Append(String.format(format_Conflict, i.ToString(), layers(i).conf().getLayer().getLayerName(), layers(i).GetType().Name, inputTypes(i), Arrays.toString(shape), (If(numElements < 0, "<variable>", numElements.ToString())), fBytes(bytes))).Append(vbLf)
				last = bytes
			Next i
			sb.Append(fBytes("Total Activations Memory", totalActivationBytes))
			sb.Append(fBytes("Total Activations Memory (per ex)", totalActivationBytes \ minibatch))

			'Exclude output layer, include input
			Dim totalActivationGradMem As Long = totalActivationBytes - last + (ArrayUtil.prodLong(inputShape) * bytesPerElement)
			sb.Append(fBytes("Total Activation Gradient Mem.", totalActivationGradMem))
			sb.Append(fBytes("Total Activation Gradient Mem. (per ex)", totalActivationGradMem \ minibatch))
		End Sub

		Private Shared Sub appendActivationShapes(ByVal net As ComputationGraph, ByVal sb As StringBuilder, ByVal bytesPerElement As Integer)
			Dim input() As INDArray = net.Inputs
			If input Is Nothing Then
				Return
			End If
			For i As Integer = 0 To input.Length - 1
				If input(i) Is Nothing Then
					Return
				End If
			Next i

			sb.Append(vbLf & "----- Network Activations: Inferred Activation Shapes -----" & vbLf)
			Dim inputType() As InputType = inferInputTypes(input)

			sb.Append(f("Current Minibatch Size", input(0).size(0)))
			For i As Integer = 0 To input.Length - 1
				sb.Append(f("Current Input Shape (Input " & i & ")", Arrays.toString(input(i).shape())))
			Next i
			Dim inputTypes As IDictionary(Of String, InputType) = net.Configuration.getLayerActivationTypes(inputType)
			Dim indices As GraphIndices = net.calculateIndices()

'JAVA TO VB CONVERTER NOTE: The variable format was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim format_Conflict As String = "%-3s %-20s %-20s %-42s %-20s %-12s %-12s"
			sb.Append(String.format(format_Conflict, "Idx", "Name", "Layer Type", "Activations Type", "Activations Shape", "# Elements", "Memory")).Append(vbLf)
			Dim layers() As Layer = net.Layers
			Dim totalActivationBytes As Long = 0
			Dim totalExOutput As Long = 0 'Implicitly includes input already due to input vertices
			Dim topo() As Integer = indices.getTopologicalSortOrder()
			For i As Integer = 0 To topo.Length - 1
				Dim layerName As String = indices.getIdxToName().get(i)
				Dim gv As GraphVertex = net.getVertex(layerName)

				Dim it As InputType = inputTypes(layerName)
				Dim shape() As Long = it.getShape(True)
				If shape(0) <= 0 Then
					shape(0) = input(0).size(0)
				End If
				Dim numElements As Long = ArrayUtil.prodLong(shape)
				Dim bytes As Long = numElements*bytesPerElement
				If bytes < 0 Then
					bytes = 0
				End If
				totalActivationBytes += bytes
				Dim className As String
				If TypeOf gv Is LayerVertex Then
					className = gv.Layer.GetType().Name
				Else
					className = gv.GetType().Name
				End If

				sb.Append(String.format(format_Conflict, i.ToString(), layerName, className, it, Arrays.toString(shape), (If(numElements < 0, "<variable>", numElements.ToString())), fBytes(bytes))).Append(vbLf)

				If Not net.Configuration.getNetworkOutputs().contains(layerName) Then
					totalExOutput += bytes
				End If
			Next i
			sb.Append(fBytes("Total Activations Memory", totalActivationBytes))
			sb.Append(fBytes("Total Activation Gradient Memory", totalExOutput))
		End Sub

	End Class

End Namespace