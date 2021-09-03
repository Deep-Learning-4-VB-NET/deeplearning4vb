Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports SneakyThrows = lombok.SneakyThrows
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports org.junit.jupiter.api
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig
Imports ILoggerFactory = org.slf4j.ILoggerFactory
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
import static org.junit.jupiter.api.Assumptions.assumeTrue

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
Namespace org.deeplearning4j


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Base DL 4 J Test") public abstract class BaseDL4JTest
	Public MustInherit Class BaseDL4JTest

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	   Private Shared log As Logger = LoggerFactory.getLogger(GetType(BaseDL4JTest).FullName)

		Protected Friend startTime As Long

		Protected Friend threadCountBefore As Integer

		Private ReadOnly DEFAULT_THREADS As Integer = Runtime.getRuntime().availableProcessors()

		''' <summary>
		''' Override this to specify the number of threads for C++ execution, via
		''' <seealso cref="org.nd4j.linalg.factory.Environment.setMaxMasterThreads(Integer)"/> </summary>
		''' <returns> Number of threads to use for C++ op execution </returns>
		Public Overridable Function numThreads() As Integer
			Return DEFAULT_THREADS
		End Function

		''' <summary>
		''' Override this method to set the default timeout for methods in the test class
		''' </summary>
		Public Overridable ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90_000
			End Get
		End Property

		''' <summary>
		''' Override this to set the profiling mode for the tests defined in the child class
		''' </summary>
		Public Overridable ReadOnly Property ProfilingMode As OpExecutioner.ProfilingMode
			Get
				Return OpExecutioner.ProfilingMode.SCOPE_PANIC
			End Get
		End Property

		''' <summary>
		''' Override this to set the datatype of the tests defined in the child class
		''' </summary>
		Public Overridable ReadOnly Property DataType As DataType
			Get
				Return DataType.DOUBLE
			End Get
		End Property

		Public Overridable ReadOnly Property DefaultFPDataType As DataType
			Get
				Return DataType
			End Get
		End Property

		Protected Friend Shared integrationTest As Boolean?

		''' <returns> True if integration tests maven profile is enabled, false otherwise. </returns>
		Public Shared ReadOnly Property IntegrationTests As Boolean
			Get
				If integrationTest Is Nothing Then
					Dim prop As String = Environment.GetEnvironmentVariable("DL4J_INTEGRATION_TESTS")
					integrationTest = Boolean.Parse(prop)
				End If
				Return integrationTest
			End Get
		End Property

		''' <summary>
		''' Call this as the first line of a test in order to skip that test, only when the integration tests maven profile is not enabled.
		''' This can be used to dynamically skip integration tests when the integration test profile is not enabled.
		''' Note that the integration test profile is not enabled by default - "integration-tests" profile
		''' </summary>
		Public Shared Sub skipUnlessIntegrationTests()
			assumeTrue(IntegrationTests, "Skipping integration test - integration profile is not enabled")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach @Timeout(90000L) void beforeTest(TestInfo testInfo)
		Friend Overridable Sub beforeTest(ByVal testInfo As TestInfo)
			log.info("{}.{}", Me.GetType().Name, testInfo.getTestMethod().get().getName())
			' Suppress ND4J initialization - don't need this logged for every test...
			System.setProperty(ND4JSystemProperties.LOG_INITIALIZATION, "false")
			System.setProperty(ND4JSystemProperties.ND4J_IGNORE_AVX, "true")
			Nd4j.Executioner.ProfilingMode = ProfilingMode
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().build()
			Nd4j.setDefaultDataTypes(DataType, DefaultFPDataType)
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().build()
			Nd4j.Executioner.enableDebugMode(False)
			Nd4j.Executioner.enableVerboseMode(False)
			Dim numThreads As Integer = Me.numThreads()
			Preconditions.checkState(numThreads > 0, "Number of threads must be > 0")
			If numThreads <> Nd4j.Environment.maxMasterThreads() Then
				Nd4j.Environment.MaxMasterThreads = numThreads
			End If
			startTime = DateTimeHelper.CurrentUnixTimeMillis()
			threadCountBefore = ManagementFactory.getThreadMXBean().getThreadCount()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SneakyThrows @AfterEach void afterTest(TestInfo testInfo)
		Friend Overridable Sub afterTest(ByVal testInfo As TestInfo)
			' Attempt to keep workspaces isolated between tests
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			Dim currWS As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			Nd4j.MemoryManager.CurrentWorkspace = Nothing
			If currWS IsNot Nothing Then
				' Not really safe to continue testing under this situation... other tests will likely fail with obscure
				' errors that are hard to track back to this
				log.error("Open workspace leaked from test! Exiting - {}, isOpen = {} - {}", currWS.Id, currWS.ScopeActive, currWS)
				Console.WriteLine("Open workspace leaked from test! Exiting - " & currWS.Id & ", isOpen = " & currWS.ScopeActive & " - " & currWS)
				System.out.flush()
				' Try to flush logs also:
				Try
					Thread.Sleep(1000)
				Catch e As InterruptedException
				End Try
				Dim lf As ILoggerFactory = LoggerFactory.getILoggerFactory()
				'work around to remove explicit dependency on logback
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				If lf.GetType().FullName.Equals("ch.qos.logback.classic.LoggerContext") Then
					Dim method As System.Reflection.MethodInfo = lf.GetType().GetMethod("stop")
					method.setAccessible(True)
					method.invoke(lf)
				End If
				Try
					Thread.Sleep(1000)
				Catch e As InterruptedException
				End Try
				Environment.Exit(1)
			End If
			Dim sb As New StringBuilder()
			Dim maxPhys As Long = Pointer.maxPhysicalBytes()
			Dim maxBytes As Long = Pointer.maxBytes()
			Dim currPhys As Long = Pointer.physicalBytes()
			Dim currBytes As Long = Pointer.totalBytes()
			Dim jvmTotal As Long = Runtime.getRuntime().totalMemory()
			Dim jvmMax As Long = Runtime.getRuntime().maxMemory()
			Dim threadsAfter As Integer = ManagementFactory.getThreadMXBean().getThreadCount()
			Dim duration As Long = DateTimeHelper.CurrentUnixTimeMillis() - startTime
			sb.Append(Me.GetType().Name).Append(".").Append(testInfo.getTestMethod().get().getName()).Append(": ").Append(duration).Append(" ms").Append(", threadCount: (").Append(threadCountBefore).Append("->").Append(threadsAfter).Append(")").Append(", jvmTotal=").Append(jvmTotal).Append(", jvmMax=").Append(jvmMax).Append(", totalBytes=").Append(currBytes).Append(", maxBytes=").Append(maxBytes).Append(", currPhys=").Append(currPhys).Append(", maxPhys=").Append(maxPhys)
			Dim ws As IList(Of MemoryWorkspace) = Nd4j.WorkspaceManager.getAllWorkspacesForCurrentThread()
			If ws IsNot Nothing AndAlso ws.Count > 0 Then
				Dim currSize As Long = 0
				For Each w As MemoryWorkspace In ws
					currSize += w.CurrentSize
				Next w
				If currSize > 0 Then
					sb.Append(", threadWSSize=").Append(currSize).Append(" (").Append(ws.Count).Append(" WSs)")
				End If
			End If
			Dim p As Properties = Nd4j.Executioner.EnvironmentInformation
			Dim o As Object = p.get("cuda.devicesInformation")
			If TypeOf o Is System.Collections.IList Then
				Dim l As IList(Of IDictionary(Of String, Object)) = DirectCast(o, IList(Of IDictionary(Of String, Object)))
				If l.Count > 0 Then
					sb.Append(" [").Append(l.Count).Append(" GPUs: ")
					For i As Integer = 0 To l.Count - 1
						Dim m As IDictionary(Of String, Object) = l(i)
						If i > 0 Then
							sb.Append(",")
						End If
						sb.Append("(").Append(m("cuda.freeMemory")).Append(" free, ").Append(m("cuda.totalMemory")).Append(" total)")
					Next i
					sb.Append("]")
				End If
			End If
			log.info(sb.ToString())
		End Sub
	End Class

End Namespace