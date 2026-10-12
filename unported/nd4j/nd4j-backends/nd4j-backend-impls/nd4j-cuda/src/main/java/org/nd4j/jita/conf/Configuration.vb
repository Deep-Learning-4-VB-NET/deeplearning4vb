Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports Aggressiveness = org.nd4j.jita.allocator.enums.Aggressiveness
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.nd4j.jita.conf


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Configuration implements java.io.Serializable
	<Serializable>
	Public Class Configuration

		Public Enum ExecutionModel
			SEQUENTIAL
			ASYNCHRONOUS
			OPTIMIZED
		End Enum

		Public Enum AllocationModel
			DIRECT
			CACHE_HOST
			CACHE_ALL
		End Enum

		Public Enum MemoryModel
			IMMEDIATE
			DELAYED
		End Enum

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Deprecated private ExecutionModel executionModel = ExecutionModel.SEQUENTIAL;
'JAVA TO VB CONVERTER NOTE: The field executionModel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<Obsolete>
		Private executionModel_Conflict As ExecutionModel = ExecutionModel.SEQUENTIAL

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private AllocationModel allocationModel = AllocationModel.CACHE_ALL;
'JAVA TO VB CONVERTER NOTE: The field allocationModel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private allocationModel_Conflict As AllocationModel = AllocationModel.CACHE_ALL

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.jita.allocator.enums.AllocationStatus firstMemory = org.nd4j.jita.allocator.enums.AllocationStatus.DEVICE;
'JAVA TO VB CONVERTER NOTE: The field firstMemory was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private firstMemory_Conflict As AllocationStatus = AllocationStatus.DEVICE

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private MemoryModel memoryModel = MemoryModel.IMMEDIATE;
'JAVA TO VB CONVERTER NOTE: The field memoryModel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private memoryModel_Conflict As MemoryModel = MemoryModel.IMMEDIATE

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean debug = false;
		Private debug As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean verbose = false;
'JAVA TO VB CONVERTER NOTE: The field verbose was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private verbose_Conflict As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean fillDashboard = false;
		Private fillDashboard As Boolean = False

		Private forceSingleGPU As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long noGcWindowMs = 100;
'JAVA TO VB CONVERTER NOTE: The field noGcWindowMs was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private noGcWindowMs_Conflict As Long = 100

		''' <summary>
		''' Keep this value between 0.01 and 0.95 please
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private double maximumDeviceMemoryUsed = 0.85;
'JAVA TO VB CONVERTER NOTE: The field maximumDeviceMemoryUsed was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumDeviceMemoryUsed_Conflict As Double = 0.85

		''' <summary>
		''' Minimal number of activations for relocation threshold
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int minimumRelocationThreshold = 5;
'JAVA TO VB CONVERTER NOTE: The field minimumRelocationThreshold was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private minimumRelocationThreshold_Conflict As Integer = 5

		''' <summary>
		''' Minimal guaranteed TTL for memory chunk
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long minimumTTLMilliseconds = 10 * 1000L;
		Private minimumTTLMilliseconds As Long = 10 * 1000L

		''' <summary>
		''' Number of buckets/garbage collectors for host memory
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int numberOfGcThreads = 6;
'JAVA TO VB CONVERTER NOTE: The field numberOfGcThreads was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numberOfGcThreads_Conflict As Integer = 6

		''' <summary>
		''' Deallocation aggressiveness
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated @Getter private org.nd4j.jita.allocator.enums.Aggressiveness hostDeallocAggressiveness = org.nd4j.jita.allocator.enums.Aggressiveness.REASONABLE;
		<Obsolete>
		Private hostDeallocAggressiveness As Aggressiveness = Aggressiveness.REASONABLE

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated @Getter private org.nd4j.jita.allocator.enums.Aggressiveness gpuDeallocAggressiveness = org.nd4j.jita.allocator.enums.Aggressiveness.REASONABLE;
		<Obsolete>
		Private gpuDeallocAggressiveness As Aggressiveness = Aggressiveness.REASONABLE

		''' <summary>
		''' Allocation aggressiveness
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated @Getter private org.nd4j.jita.allocator.enums.Aggressiveness gpuAllocAggressiveness = org.nd4j.jita.allocator.enums.Aggressiveness.REASONABLE;
		<Obsolete>
		Private gpuAllocAggressiveness As Aggressiveness = Aggressiveness.REASONABLE


		''' <summary>
		''' Maximum allocated per-device memory, in bytes
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long maximumDeviceAllocation = 4 * 1024 * 1024 * 1024L;
'JAVA TO VB CONVERTER NOTE: The field maximumDeviceAllocation was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumDeviceAllocation_Conflict As Long = 4 * 1024 * 1024 * 1024L


		''' <summary>
		''' Maximum allocatable zero-copy/pinned/pageable memory
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long maximumZeroAllocation = Runtime.getRuntime().maxMemory() + (500 * 1024 * 1024L);
'JAVA TO VB CONVERTER NOTE: The field maximumZeroAllocation was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumZeroAllocation_Conflict As Long = Runtime.getRuntime().maxMemory() + (500 * 1024 * 1024L)

		''' <summary>
		''' True if allowed, false if relocation required
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean crossDeviceAccessAllowed = true;
		Private crossDeviceAccessAllowed As Boolean = True

		''' <summary>
		''' True, if allowed, false otherwise
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean zeroCopyFallbackAllowed = false;
		Private zeroCopyFallbackAllowed As Boolean = False

		''' <summary>
		''' Maximum length of single memory chunk
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long maximumSingleHostAllocation = Long.MAX_VALUE;
'JAVA TO VB CONVERTER NOTE: The field maximumSingleHostAllocation was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumSingleHostAllocation_Conflict As Long = Long.MaxValue

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long maximumSingleDeviceAllocation = 1024 * 1024 * 1024L;
'JAVA TO VB CONVERTER NOTE: The field maximumSingleDeviceAllocation was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumSingleDeviceAllocation_Conflict As Long = 1024 * 1024 * 1024L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private java.util.List<Integer> availableDevices = new java.util.ArrayList<>();
		Private availableDevices As IList(Of Integer) = New List(Of Integer)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private java.util.List<Integer> bannedDevices = new java.util.ArrayList<>();
		Private bannedDevices As IList(Of Integer) = New List(Of Integer)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int maximumGridSize = 4096;
'JAVA TO VB CONVERTER NOTE: The field maximumGridSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumGridSize_Conflict As Integer = 4096

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int maximumBlockSize = 256;
'JAVA TO VB CONVERTER NOTE: The field maximumBlockSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumBlockSize_Conflict As Integer = 256

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int minimumBlockSize = 32;
'JAVA TO VB CONVERTER NOTE: The field minimumBlockSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private minimumBlockSize_Conflict As Integer = 32

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long maximumHostCache = 1024 * 1024 * 1024L;
'JAVA TO VB CONVERTER NOTE: The field maximumHostCache was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumHostCache_Conflict As Long = 1024 * 1024 * 1024L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long maximumDeviceCache = 512L * 1024L * 1024L;
'JAVA TO VB CONVERTER NOTE: The field maximumDeviceCache was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumDeviceCache_Conflict As Long = 512L * 1024L * 1024L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean usePreallocation = false;
		Private usePreallocation As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int preallocationCalls = 10;
'JAVA TO VB CONVERTER NOTE: The field preallocationCalls was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preallocationCalls_Conflict As Integer = 10

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long maximumHostCacheableLength = 100663296;
'JAVA TO VB CONVERTER NOTE: The field maximumHostCacheableLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumHostCacheableLength_Conflict As Long = 100663296

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long maximumDeviceCacheableLength = 16L * 1024L * 1024L;
'JAVA TO VB CONVERTER NOTE: The field maximumDeviceCacheableLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private maximumDeviceCacheableLength_Conflict As Long = 16L * 1024L * 1024L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int commandQueueLength = 3;
'JAVA TO VB CONVERTER NOTE: The field commandQueueLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private commandQueueLength_Conflict As Integer = 3

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int commandLanesNumber = 4;
'JAVA TO VB CONVERTER NOTE: The field commandLanesNumber was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private commandLanesNumber_Conflict As Integer = 4

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int debugTriggered = 0;
		Private debugTriggered As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int poolSize = 32;
'JAVA TO VB CONVERTER NOTE: The field poolSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private poolSize_Conflict As Integer = 32

'JAVA TO VB CONVERTER NOTE: The field initialized was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly initialized_Conflict As New AtomicBoolean(False)

		Public Overridable ReadOnly Property Initialized As Boolean
			Get
				Return initialized_Conflict.get()
			End Get
		End Property

		Public Overridable Sub setInitialized()
			Me.initialized_Conflict.compareAndSet(False, True)
		End Sub


		Private Sub parseEnvironmentVariables()

			' Do not call System.getenv(): Accessing all variables requires higher security privileges
			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_BLOCK_SIZE) IsNot Nothing Then
				Try
					Dim var As Integer = Integer.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_BLOCK_SIZE))
					MaximumBlockSize = var
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_MAX_BLOCK_SIZE, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_BLOCK_SIZE))
				End Try
			End If

			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MIN_BLOCK_SIZE) IsNot Nothing Then
				Try
					Dim var As Integer = Integer.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MIN_BLOCK_SIZE))
					MinimumBlockSize = var
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_MIN_BLOCK_SIZE, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MIN_BLOCK_SIZE))
				End Try
			End If

			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_GRID_SIZE) IsNot Nothing Then
				Try
					Dim var As Integer = Integer.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_GRID_SIZE))
					MaximumGridSize = var
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_MAX_GRID_SIZE, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_GRID_SIZE))
				End Try
			End If

			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_CONTEXTS) IsNot Nothing Then
				Try
					Dim var As Integer = Integer.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_CONTEXTS))
					PoolSize = var
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_MAX_CONTEXTS, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_CONTEXTS))
				End Try
			End If

			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_FORCE_SINGLE_GPU) IsNot Nothing Then
				Try
					Dim var As Boolean = Boolean.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_FORCE_SINGLE_GPU))
					allowMultiGPU(Not var)
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_FORCE_SINGLE_GPU, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_FORCE_SINGLE_GPU))
				End Try
			End If

			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_USE_PREALLOCATION) IsNot Nothing Then
				Try
					Dim var As Boolean = Boolean.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_USE_PREALLOCATION))
					allowPreallocation(var)
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_USE_PREALLOCATION, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_USE_PREALLOCATION))
				End Try
			End If

			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_DEVICE_CACHE) IsNot Nothing Then
				Try
					Dim var As Long = Long.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_DEVICE_CACHE))
					MaximumDeviceCache = var
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_MAX_DEVICE_CACHE, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_DEVICE_CACHE))
				End Try
			End If


			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_HOST_CACHE) IsNot Nothing Then
				Try
					Dim var As Long = Long.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_HOST_CACHE))
					MaximumHostCache = var
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_MAX_HOST_CACHE, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_HOST_CACHE))
				End Try
			End If

			If Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_DEVICE_ALLOCATION) IsNot Nothing Then
				Try
					Dim var As Long = Long.Parse(Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_DEVICE_ALLOCATION))
					MaximumSingleDeviceAllocation = var
				Catch e As Exception
					log.error("Can't parse {}: [{}]", ND4JEnvironmentVars.ND4J_CUDA_MAX_DEVICE_ALLOCATION, Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_CUDA_MAX_DEVICE_ALLOCATION))
				End Try
			End If

		End Sub

		''' <summary>
		''' This method enables/disables
		''' </summary>
		''' <param name="reallyEnable">
		''' @return </param>
		Public Overridable Function enableDashboard(ByVal reallyEnable As Boolean) As Configuration
			fillDashboard = reallyEnable
			Return Me
		End Function

		''' <summary>
		''' Per-device resources pool size. Streams, utility memory
		''' </summary>
		''' <param name="poolSize">
		''' @return </param>
		Public Overridable Function setPoolSize(ByVal poolSize As Integer) As Configuration
			If poolSize < 8 Then
				Throw New System.InvalidOperationException("poolSize can't be lower then 8")
			End If
			Me.poolSize_Conflict = poolSize
			Return Me
		End Function

		Public Overridable Function triggerDebug(ByVal code As Integer) As Configuration
			Me.debugTriggered = code
			Return Me
		End Function

		Public Overridable Function setMinimumRelocationThreshold(ByVal threshold As Integer) As Configuration
			Me.maximumDeviceAllocation_Conflict = Math.Max(2, threshold)

			Return Me
		End Function

		''' <summary>
		''' This method allows you to specify maximum memory cache for host memory
		''' </summary>
		''' <param name="maxCache">
		''' @return </param>
		Public Overridable Function setMaximumHostCache(ByVal maxCache As Long) As Configuration
			Me.maximumHostCache_Conflict = maxCache
			Return Me
		End Function

		''' <summary>
		''' This method allows you to specify maximum memory cache per device
		''' </summary>
		''' <param name="maxCache">
		''' @return </param>
		Public Overridable Function setMaximumDeviceCache(ByVal maxCache As Long) As Configuration
			Me.maximumDeviceCache_Conflict = maxCache
			Return Me
		End Function

		''' <summary>
		''' This method allows you to specify max per-device memory use.
		''' 
		''' PLEASE NOTE: Accepted value range is 0.01 > x < 0.95
		''' </summary>
		''' <param name="percentage"> </param>
		Public Overridable Function setMaximumDeviceMemoryUsed(ByVal percentage As Double) As Configuration
			If percentage < 0.02 OrElse percentage > 0.95 Then
				Me.maximumDeviceMemoryUsed_Conflict = 0.85
			Else
				Me.maximumDeviceMemoryUsed_Conflict = percentage
			End If

			Return Me
		End Function

		Public Sub New()
			parseEnvironmentVariables()
		End Sub


		Friend Overridable Sub updateDevice()
			Dim cnt As Integer = Nd4j.AffinityManager.NumberOfDevices

			If cnt = 0 Then
				Throw New Exception("No CUDA devices were found in system")
			End If

			For i As Integer = 0 To cnt - 1
				availableDevices.Add(i)
			Next i
		End Sub



		''' <summary>
		''' This method checks, if GPU subsystem supports cross-device P2P access over PCIe.
		''' 
		''' PLEASE NOTE: This method also returns TRUE if system has only one device. This is done to guarantee reallocation avoidance within same device.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property P2PSupported As Boolean
			Get
				Return NativeOpsHolder.Instance.getDeviceNativeOps().isP2PAvailable()
			End Get
		End Property

		''' <summary>
		''' This method allows you to ban specific device.
		''' 
		''' PLEASE NOTE: This method
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Configuration banDevice(@NonNull Integer deviceId)
		Public Overridable Function banDevice(ByVal deviceId As Integer) As Configuration
			If Not availableDevices.Contains(deviceId) Then
				Return Me
			End If

			If Not bannedDevices.Contains(deviceId) Then
				bannedDevices.Add(deviceId)
			End If

			availableDevices.RemoveAt(deviceId)

			Return Me
		End Function

		''' <summary>
		''' This method forces specific device to be used. All other devices present in system will be ignored.
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Configuration useDevice(@NonNull Integer deviceId)
		Public Overridable Function useDevice(ByVal deviceId As Integer) As Configuration
			Return useDevices(deviceId)
		End Function

		''' <summary>
		''' This method forces specific devices to be used. All other devices present in system will be ignored.
		''' </summary>
		''' <param name="devices">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Configuration useDevices(@NonNull int... devices)
		Public Overridable Function useDevices(ParamArray ByVal devices() As Integer) As Configuration
			Dim usableDevices As IList(Of Integer) = New List(Of Integer)()
			For Each device As Integer In devices
				If Not availableDevices.Contains(device) Then
					log.warn("Non-existent device [{}] requested, ignoring...", device)
				Else
					If Not usableDevices.Contains(device) Then
						usableDevices.Add(device)
					End If
				End If

			Next device

			If usableDevices.Count > 0 Then
				availableDevices.Clear()
				CType(availableDevices, List(Of Integer)).AddRange(usableDevices)
			End If

			Return Me
		End Function

		''' <summary>
		''' This method allows you to set maximum host allocation. However, it's recommended to leave it as default: Xmx + something.
		''' </summary>
		''' <param name="max"> amount of memory in bytes </param>
		Public Overridable Function setMaximumZeroAllocation(ByVal max As Long) As Configuration
			Dim xmx As Long = Runtime.getRuntime().maxMemory()
			If max < xmx Then
				log.warn("Setting maximum memory below -Xmx value can cause problems")
			End If

			If max <= 0 Then
				Throw New System.InvalidOperationException("You can't set maximum host memory <= 0")
			End If

			maximumZeroAllocation_Conflict = max

			Return Me
		End Function

		''' <summary>
		''' This method allows you to set maximum device allocation. It's recommended to keep it equal to MaximumZeroAllocation </summary>
		''' <param name="max"> </param>
		Public Overridable Function setMaximumDeviceAllocation(ByVal max As Long) As Configuration
			If max < 0 Then
				Throw New System.InvalidOperationException("You can't set maximum device memory < 0")
			End If

			Return Me
		End Function

		''' <summary>
		''' This method allows to specify maximum single allocation on host.
		''' 
		''' Default value: Long.MAX_VALUE
		''' </summary>
		''' <param name="max">
		''' @return </param>
		Public Overridable Function setMaximumSingleHostAllocation(ByVal max As Long) As Configuration
			Me.maximumSingleHostAllocation_Conflict = max

			Return Me
		End Function

		''' <summary>
		''' This method allows to specify maximum single allocation on device.
		''' 
		''' Default value: Long.MAX_VALUE
		''' </summary>
		''' <param name="max">
		''' @return </param>
		Public Overridable Function setMaximumSingleDeviceAllocation(ByVal max As Long) As Configuration
			Me.maximumSingleDeviceAllocation_Conflict = max

			Return Me
		End Function

		''' <summary>
		''' This method allows to specify max gridDim for kernel launches.
		''' 
		''' Default value: 128
		''' </summary>
		''' <param name="gridDim">
		''' @return </param>
		Public Overridable Function setMaximumGridSize(ByVal gridDim As Integer) As Configuration
			If gridDim <= 7 OrElse gridDim > 8192 Then
				Throw New System.InvalidOperationException("Please keep gridDim in range [8...8192]")
			End If

			Me.maximumGridSize_Conflict = gridDim

			Return Me
		End Function

		''' <summary>
		''' This methos allows to specify max blockSize for kernel launches
		''' 
		''' Default value: -1 (that means pick value automatically, device occupancy dependent)
		''' </summary>
		''' <param name="blockDim">
		''' @return </param>
		Public Overridable Function setMaximumBlockSize(ByVal blockDim As Integer) As Configuration
			If blockDim < 32 OrElse blockDim > 768 Then
				Throw New System.InvalidOperationException("Please keep blockDim in range [32...768]")
			End If


			Me.maximumBlockSize_Conflict = blockDim

			Return Me
		End Function

		Public Overridable Function setMinimumBlockSize(ByVal blockDim As Integer) As Configuration
			If blockDim < 32 OrElse blockDim > 768 Then
				Throw New System.InvalidOperationException("Please keep blockDim in range [32...768]")
			End If


			Me.minimumBlockSize_Conflict = blockDim

			Return Me
		End Function

		''' <summary>
		''' With debug enabled all CUDA launches will become synchronous, with forced stream synchronizations after calls.
		''' 
		''' Default value: false;
		''' 
		''' @return
		''' </summary>
		Public Overridable Function enableDebug(ByVal debug As Boolean) As Configuration
			Me.debug = debug
			Return Me
		End Function

		Public Overridable Function setVerbose(ByVal verbose As Boolean) As Configuration
			Me.verbose_Conflict = verbose
			Return Me
		End Function

		''' <summary>
		''' Enables/disables P2P memory access for multi-gpu
		''' </summary>
		''' <param name="reallyAllow">
		''' @return </param>
		Public Overridable Function allowCrossDeviceAccess(ByVal reallyAllow As Boolean) As Configuration
			Me.crossDeviceAccessAllowed = reallyAllow

			Return Me
		End Function

		''' <summary>
		''' This method allows to specify execution model for matrix/blas operations
		''' 
		''' SEQUENTIAL: Issue commands in order Java compiler sees them.
		''' ASYNCHRONOUS: Issue commands asynchronously, if that's possible.
		''' OPTIMIZED: Not implemented yet. Equals to asynchronous for now.
		''' 
		''' Default value: SEQUENTIAL
		''' </summary>
		''' <param name="executionModel">
		''' @return </param>
		''' @deprecated Only ExecutionModel.SEQUENTIAL is supported 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Only ExecutionModel.SEQUENTIAL is supported") public Configuration setExecutionModel(@NonNull ExecutionModel executionModel)
		<Obsolete("Only ExecutionModel.SEQUENTIAL is supported")>
		Public Overridable Function setExecutionModel(ByVal executionModel As ExecutionModel) As Configuration
			If executionModel <> ExecutionModel.SEQUENTIAL Then
				Throw New System.ArgumentException("Only ExecutionModel.SEQUENTIAL is supported")
			End If
			Me.executionModel_Conflict = ExecutionModel.SEQUENTIAL
			Return Me
		End Function

		''' <summary>
		''' This method allows to specify allocation model for memory.
		''' 
		''' DIRECT: Do not cache anything, release memory as soon as it's not used.
		''' CACHE_HOST: Cache host memory only, Device memory (if any) will use DIRECT mode.
		''' CACHE_ALL: All memory will be cached.
		''' 
		''' Defailt value: CACHE_ALL
		''' </summary>
		''' <param name="allocationModel">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Configuration setAllocationModel(@NonNull AllocationModel allocationModel)
		Public Overridable Function setAllocationModel(ByVal allocationModel As AllocationModel) As Configuration
			Me.allocationModel_Conflict = allocationModel

			Return Me
		End Function

		''' <summary>
		''' This method allows to specify initial memory to be used within system.
		''' HOST: all data is located on host memory initially, and gets into DEVICE, if used frequent enough
		''' DEVICE: all memory is located on device.
		''' DELAYED: memory allocated on HOST first, and on first use gets moved to DEVICE
		''' 
		''' PLEASE NOTE: For device memory all data still retains on host side as well.
		''' 
		''' Default value: DEVICE </summary>
		''' <param name="initialMemory">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Configuration setFirstMemory(@NonNull AllocationStatus initialMemory)
		Public Overridable Function setFirstMemory(ByVal initialMemory As AllocationStatus) As Configuration
			If initialMemory <> AllocationStatus.DEVICE AndAlso initialMemory <> AllocationStatus.HOST AndAlso initialMemory <> AllocationStatus.DELAYED Then
				Throw New System.InvalidOperationException("First memory should be either [HOST], [DEVICE] or [DELAYED]")
			End If

			Me.firstMemory_Conflict = initialMemory

			Return Me
		End Function

		''' <summary>
		''' NOT IMPLEMENTED YET </summary>
		''' <param name="reallyAllow">
		''' @return </param>
		Public Overridable Function allowFallbackFromDevice(ByVal reallyAllow As Boolean) As Configuration
			Me.zeroCopyFallbackAllowed = reallyAllow
			Return Me
		End Function

		''' <summary>
		''' This method allows you to set number of threads that'll handle memory releases on native side.
		''' 
		''' Default value: 4
		''' @return
		''' </summary>
		Public Overridable Function setNumberOfGcThreads(ByVal numThreads As Integer) As Configuration
			If numThreads <= 0 OrElse numThreads > 20 Then
				Throw New System.InvalidOperationException("Please, use something in range of [1..20] as number of GC threads")
			End If

			If Not Initialized Then
				Me.numberOfGcThreads_Conflict = numThreads
			End If

			Return Me
		End Function

		''' <summary>
		''' This method allows to specify maximum length of single memory chunk that's allowed to be cached.
		''' Please note: -1 value totally disables limits here.
		''' 
		''' Default value: 96 MB </summary>
		''' <param name="maxLen">
		''' @return </param>
		Public Overridable Function setMaximumHostCacheableLength(ByVal maxLen As Long) As Configuration
			Me.maximumHostCacheableLength_Conflict = maxLen

			Return Me
		End Function

		''' <summary>
		''' This method allows to specify maximum length of single memory chunk that's allowed to be cached.
		''' Please note: -1 value totally disables limits here.
		''' 
		''' Default value: 96 MB </summary>
		''' <param name="maxLen">
		''' @return </param>
		Public Overridable Function setMaximumDeviceCacheableLength(ByVal maxLen As Long) As Configuration
			Me.maximumDeviceCacheableLength_Conflict = maxLen

			Return Me
		End Function

		''' <summary>
		''' If set to true, each non-cached allocation request will cause few additional allocations,
		''' 
		''' Default value: true
		''' </summary>
		''' <param name="reallyAllow">
		''' @return </param>
		Public Overridable Function allowPreallocation(ByVal reallyAllow As Boolean) As Configuration
			Me.usePreallocation = reallyAllow

			Return Me
		End Function

		''' <summary>
		''' This method allows to specify number of preallocation calls done by cache subsystem in parallel, to serve later requests.
		''' 
		''' Default value: 25
		''' </summary>
		''' <param name="numCalls">
		''' @return </param>
		Public Overridable Function setPreallocationCalls(ByVal numCalls As Integer) As Configuration
			If numCalls < 0 OrElse numCalls > 100 Then
				Throw New System.InvalidOperationException("Please use preallocation calls in range of [1..100]")
			End If
			Me.preallocationCalls_Conflict = numCalls

			Return Me
		End Function

		''' <summary>
		''' This method allows you to specify command queue length, as primary argument for asynchronous execution controller
		''' 
		''' Default value: 3
		''' </summary>
		''' <param name="length">
		''' @return </param>
		Public Overridable Function setCommandQueueLength(ByVal length As Integer) As Configuration
			If length <= 0 Then
				Throw New System.InvalidOperationException("Command queue length can't be <= 0")
			End If
			Me.commandQueueLength_Conflict = length

			Return Me
		End Function

		''' <summary>
		''' This option specifies minimal time gap between two subsequent System.gc() calls
		''' Set to 0 to disable this option.
		''' </summary>
		''' <param name="windowMs">
		''' @return </param>
		Public Overridable Function setNoGcWindowMs(ByVal windowMs As Long) As Configuration
			If windowMs < 1 Then
				Throw New System.InvalidOperationException("No-GC window should have positive value")
			End If

			Me.noGcWindowMs_Conflict = windowMs
			Return Me
		End Function

		''' <summary>
		''' This method allows you to specify maximum number of probable parallel cuda processes
		''' 
		''' Default value: 4
		''' 
		''' PLEASE NOTE: This parameter has effect only for ASYNCHRONOUS execution model
		''' </summary>
		''' <param name="length">
		''' @return </param>
		Public Overridable Function setCommandLanesNumber(ByVal length As Integer) As Configuration
			If length < 1 Then
				Throw New System.InvalidOperationException("Command Lanes number can't be < 1")
			End If
			If length > 8 Then
				length = 8
			End If
			Me.commandLanesNumber_Conflict = length

			Return Me
		End Function

		Public Overridable ReadOnly Property ForcedSingleGPU As Boolean
			Get
				Return forceSingleGPU
			End Get
		End Property

		''' <summary>
		''' This method allows you to enable or disable multi-GPU mode.
		''' 
		''' PLEASE NOTE: This is NOT magic method, that will automatically scale your application performance.
		''' </summary>
		''' <param name="reallyAllow">
		''' @return </param>
		Public Overridable Function allowMultiGPU(ByVal reallyAllow As Boolean) As Configuration
			forceSingleGPU = Not reallyAllow
			Return Me
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Configuration setMemoryModel(@NonNull MemoryModel model)
		Public Overridable Function setMemoryModel(ByVal model As MemoryModel) As Configuration
			memoryModel_Conflict = model
			Return Me
		End Function
	End Class

End Namespace