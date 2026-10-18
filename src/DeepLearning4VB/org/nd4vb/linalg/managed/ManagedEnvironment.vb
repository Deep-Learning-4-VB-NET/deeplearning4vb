' DeepLearning4VB: the managed backend's environment settings (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.CpuEnvironment.

Imports System
Imports System.Collections.Concurrent
Imports System.Globalization
Imports AllocationsTracker = DeepLearning4VB.org.nd4vb.linalg.api.memory.AllocationsTracker
Imports ND4VBEnvironmentVars = DeepLearning4VB.org.nd4vb.common.config.ND4VBEnvironmentVars
Imports EnvironmentT = DeepLearning4VB.org.nd4vb.linalg.factory.Environment

Namespace org.nd4vb.linalg.managed

    ''' <summary>
    ''' The environment of the managed backend (Java <c>CpuEnvironment</c>, which wrapped libnd4j's native
    ''' <c>Environment</c> singleton). The settings are kept here with libnd4j's defaults: not verbose, no debug, no
    ''' profiling, helpers allowed (unless <c>SD_FORBID_HELPERS</c> is set), TAD threshold 1, element-wise threshold 1024,
    ''' and as many threads as <c>SD_MAX_THREADS</c>, <c>OMP_NUM_THREADS</c> or the processor count say
    ''' (<c>SD_MASTER_THREADS</c> for master threads).
    ''' </summary>
    ''' <remarks>
    ''' Differences from Java: there is no native BLAS, so the BLAS version is 0.0.0; the memory limits and group/device
    ''' limits are stored but not enforced; <see cref="getDeviceCouner"/> returns the bytes the
    ''' <see cref="AllocationsTracker"/> records on the device. The managed kernels read <see cref="maxThreads"/> and the
    ''' thresholds when they choose parallel loops (wave 3).
    ''' </remarks>
    Public Class ManagedEnvironment
        Implements EnvironmentT

        Private Shared ReadOnly INSTANCE_Field As New ManagedEnvironment()

        Private verboseField As Boolean = False
        Private debugField As Boolean = False
        Private profilingField As Boolean = False
        Private leaksField As Boolean = False
        Private helpersField As Boolean = True
        Private tadThresholdField As Integer = 1
        Private elementwiseThresholdField As Integer = 1024
        Private maxThreadsField As Integer
        Private maxMasterThreadsField As Integer
        Private maxPrimaryMemoryField As Long = 0
        Private maxSpecialMemoryField As Long = 0
        Private maxDeviceMemoryField As Long = 0
        Private ReadOnly groupLimits As New ConcurrentDictionary(Of Integer, Long)()
        Private ReadOnly deviceLimits As New ConcurrentDictionary(Of Integer, Long)()

        ''' <summary>The process-wide environment.</summary>
        Public Shared ReadOnly Property Instance As ManagedEnvironment
            Get
                Return INSTANCE_Field
            End Get
        End Property

        ''' <summary>A new environment with the defaults (normally use <see cref="Instance"/>).</summary>
        Protected Friend Sub New()
            maxThreadsField = positiveFromEnvironment(ND4VBEnvironmentVars.SD_MAX_THREADS, positiveFromEnvironment(ND4VBEnvironmentVars.OMP_NUM_THREADS, System.Environment.ProcessorCount))
            maxMasterThreadsField = positiveFromEnvironment(ND4VBEnvironmentVars.SD_MASTER_THREADS, maxThreadsField)
            Dim forbid As String = System.Environment.GetEnvironmentVariable(ND4VBEnvironmentVars.SD_FORBID_HELPERS)
            If forbid IsNot Nothing AndAlso (forbid = "1" OrElse String.Equals(forbid, "true", StringComparison.OrdinalIgnoreCase)) Then
                helpersField = False
            End If
            maxPrimaryMemoryField = longFromEnvironment(ND4VBEnvironmentVars.SD_MAX_PRIMARY_BYTES)
            maxSpecialMemoryField = longFromEnvironment(ND4VBEnvironmentVars.SD_MAX_SPECIAL_BYTES)
            maxDeviceMemoryField = longFromEnvironment(ND4VBEnvironmentVars.SD_MAX_DEVICE_BYTES)
        End Sub

        Private Shared Function positiveFromEnvironment(ByVal name As String, ByVal defaultValue As Integer) As Integer
            Dim v As Integer
            Dim s As String = System.Environment.GetEnvironmentVariable(name)
            If s IsNot Nothing AndAlso Integer.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, v) AndAlso v > 0 Then
                Return v
            End If
            Return defaultValue
        End Function

        Private Shared Function longFromEnvironment(ByVal name As String) As Long
            Dim v As Long
            Dim s As String = System.Environment.GetEnvironmentVariable(name)
            If s IsNot Nothing AndAlso Long.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, v) AndAlso v > 0 Then
                Return v
            End If
            Return 0
        End Function

        ''' <summary>BLAS major version: 0 (the managed BLAS has no version).</summary>
        Public Overridable Function blasMajorVersion() As Integer Implements EnvironmentT.blasMajorVersion
            Return 0
        End Function

        ''' <summary>BLAS minor version: 0.</summary>
        Public Overridable Function blasMinorVersion() As Integer Implements EnvironmentT.blasMinorVersion
            Return 0
        End Function

        ''' <summary>BLAS patch version: 0.</summary>
        Public Overridable Function blasPatchVersion() As Integer Implements EnvironmentT.blasPatchVersion
            Return 0
        End Function

        ''' <summary>Verbose mode.</summary>
        Public Overridable Property Verbose As Boolean Implements EnvironmentT.Verbose
            Get
                Return verboseField
            End Get
            Set(ByVal reallyVerbose As Boolean)
                verboseField = reallyVerbose
            End Set
        End Property

        ''' <summary>Debug mode.</summary>
        Public Overridable Property Debug As Boolean Implements EnvironmentT.Debug
            Get
                Return debugField
            End Get
            Set(ByVal reallyDebug As Boolean)
                debugField = reallyDebug
            End Set
        End Property

        ''' <summary>Profiling mode.</summary>
        Public Overridable Property Profiling As Boolean Implements EnvironmentT.Profiling
            Get
                Return profilingField
            End Get
            Set(ByVal reallyProfile As Boolean)
                profilingField = reallyProfile
            End Set
        End Property

        ''' <summary>True if the leaks detector is enabled.</summary>
        Public Overridable ReadOnly Property DetectingLeaks As Boolean Implements EnvironmentT.DetectingLeaks
            Get
                Return leaksField
            End Get
        End Property

        ''' <summary>True if both debug and verbose modes are on.</summary>
        Public Overridable ReadOnly Property DebugAndVerbose As Boolean Implements EnvironmentT.DebugAndVerbose
            Get
                Return debugField AndAlso verboseField
            End Get
        End Property

        ''' <summary>Enables or disables the leaks detector.</summary>
        Public Overridable WriteOnly Property LeaksDetector As Boolean Implements EnvironmentT.LeaksDetector
            Set(ByVal reallyDetect As Boolean)
                leaksField = reallyDetect
            End Set
        End Property

        ''' <summary>True if platform helpers may be used.</summary>
        Public Overridable Function helpersAllowed() As Boolean Implements EnvironmentT.helpersAllowed
            Return helpersField
        End Function

        ''' <summary>Allows or forbids platform helpers.</summary>
        Public Overridable Sub allowHelpers(ByVal reallyAllow As Boolean) Implements EnvironmentT.allowHelpers
            helpersField = reallyAllow
        End Sub

        ''' <summary>The TAD count above which TAD loops run in parallel.</summary>
        Public Overridable Function tadThreshold() As Integer Implements EnvironmentT.tadThreshold
            Return tadThresholdField
        End Function

        ''' <summary>Sets the TAD threshold.</summary>
        Public Overridable Sub setTadThreshold(ByVal value As Integer) Implements EnvironmentT.setTadThreshold
            tadThresholdField = value
        End Sub

        ''' <summary>The element count above which element-wise loops run in parallel.</summary>
        Public Overridable Function elementwiseThreshold() As Integer Implements EnvironmentT.elementwiseThreshold
            Return elementwiseThresholdField
        End Function

        ''' <summary>Sets the element-wise threshold.</summary>
        Public Overridable Sub setElementwiseThreshold(ByVal value As Integer) Implements EnvironmentT.setElementwiseThreshold
            elementwiseThresholdField = value
        End Sub

        ''' <summary>The maximum number of threads of parallel loops.</summary>
        Public Overridable Function maxThreads() As Integer Implements EnvironmentT.maxThreads
            Return maxThreadsField
        End Function

        ''' <summary>Sets the maximum number of threads.</summary>
        Public Overridable Sub setMaxThreads(ByVal value As Integer) Implements EnvironmentT.setMaxThreads
            maxThreadsField = value
        End Sub

        ''' <summary>The maximum number of master threads.</summary>
        Public Overridable Function maxMasterThreads() As Integer Implements EnvironmentT.maxMasterThreads
            Return maxMasterThreadsField
        End Function

        ''' <summary>Sets the maximum number of master threads.</summary>
        Public Overridable Sub setMaxMasterThreads(ByVal value As Integer) Implements EnvironmentT.setMaxMasterThreads
            maxMasterThreadsField = value
        End Sub

        ''' <summary>The primary memory limit (stored, not enforced).</summary>
        Public Overridable WriteOnly Property MaxPrimaryMemory As Long Implements EnvironmentT.MaxPrimaryMemory
            Set(ByVal maxBytes As Long)
                maxPrimaryMemoryField = maxBytes
            End Set
        End Property

        ''' <summary>The special memory limit (stored, not enforced).</summary>
        Public Overridable WriteOnly Property MaxSpecialMemory As Long Implements EnvironmentT.MaxSpecialMemory
            Set(ByVal maxBytes As Long)
                maxSpecialMemoryField = maxBytes
            End Set
        End Property

        ''' <summary>The device memory limit (stored, not enforced).</summary>
        Public Overridable WriteOnly Property MaxDeviceMemory As Long Implements EnvironmentT.MaxDeviceMemory
            Set(ByVal maxBytes As Long)
                maxDeviceMemoryField = maxBytes
            End Set
        End Property

        ''' <summary>The memory limits last set (primary, special, device; 0 when unset).</summary>
        Public Overridable Function getMemoryLimits() As Long()
            Return New Long() {maxPrimaryMemoryField, maxSpecialMemoryField, maxDeviceMemoryField}
        End Function

        ''' <summary>Always True: the managed backend runs on the CPU.</summary>
        Public Overridable ReadOnly Property CPU As Boolean Implements EnvironmentT.CPU
            Get
                Return True
            End Get
        End Property

        ''' <summary>Stores a memory limit for a group (not enforced).</summary>
        Public Overridable Sub setGroupLimit(ByVal group As Integer, ByVal numBytes As Long) Implements EnvironmentT.setGroupLimit
            groupLimits(group) = numBytes
        End Sub

        ''' <summary>Stores a memory limit for a device (not enforced).</summary>
        Public Overridable Sub setDeviceLimit(ByVal deviceId As Integer, ByVal numBytes As Long) Implements EnvironmentT.setDeviceLimit
            deviceLimits(deviceId) = numBytes
        End Sub

        ''' <summary>The memory limit of a group (0 when unset).</summary>
        Public Overridable Function getGroupLimit(ByVal group As Integer) As Long Implements EnvironmentT.getGroupLimit
            Dim v As Long
            Return If(groupLimits.TryGetValue(group, v), v, 0L)
        End Function

        ''' <summary>The memory limit of a device (0 when unset).</summary>
        Public Overridable Function getDeviceLimit(ByVal deviceId As Integer) As Long Implements EnvironmentT.getDeviceLimit
            Dim v As Long
            Return If(deviceLimits.TryGetValue(deviceId, v), v, 0L)
        End Function

        ''' <summary>The bytes the allocation tracker records on the device (Java: libnd4j's memory counter).</summary>
        Public Overridable Function getDeviceCouner(ByVal deviceId As Integer) As Long Implements EnvironmentT.getDeviceCouner
            Return AllocationsTracker.Instance.bytesOnDevice(deviceId)
        End Function

    End Class

End Namespace
