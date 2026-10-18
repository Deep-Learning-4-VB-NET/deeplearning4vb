' DeepLearning4VB: the managed ND4VB backend (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.CpuBackend and its nd4j-native.properties.

Imports System
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports AbstractResource = DeepLearning4VB.org.nd4vb.common.io.AbstractResource
Imports Resource = DeepLearning4VB.org.nd4vb.common.io.Resource
Imports ND4VBSystemProperties = DeepLearning4VB.org.nd4vb.common.config.ND4VBSystemProperties
Imports EnvironmentT = DeepLearning4VB.org.nd4vb.linalg.factory.Environment
Imports Nd4vbBackend = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vbBackend
Imports ManagedBlasWrapper = DeepLearning4VB.org.nd4vb.linalg.managed.blas.ManagedBlasWrapper
Imports ManagedDataBufferFactory = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBufferFactory
Imports ManagedConstantHandler = DeepLearning4VB.org.nd4vb.linalg.managed.cache.ManagedConstantHandler
Imports KernelOptions = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.KernelOptions
Imports ManagedMemoryManager = DeepLearning4VB.org.nd4vb.linalg.managed.memory.ManagedMemoryManager
Imports ManagedWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.managed.memory.ManagedWorkspaceManager
Imports ManagedRandom = DeepLearning4VB.org.nd4vb.linalg.managed.rng.ManagedRandom
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory

Namespace org.nd4vb.linalg.managed

    ''' <summary>
    ''' The managed (pure .NET) backend of ND4VB (Java <c>CpuBackend</c>): always available, CPU priority, and configured
    ''' with the managed components. <see cref="Nd4vbBackend.load"/> finds it in the DeepLearning4VB assembly.
    ''' </summary>
    ''' <remarks>
    ''' <para>The configuration (<see cref="ConfigurationResource"/>, read into <see cref="Nd4vbBackend.Properties"/>) is
    ''' held in memory (Java read <c>nd4j-native.properties</c> from the class path). It names the .NET types
    ''' (<see cref="Type.FullName"/>) that <c>Nd4vb</c> creates, under the keys Java's <c>Nd4j.initWithBackend</c> reads:
    ''' <list type="table">
    ''' <listheader><term>key</term><description>value</description></listheader>
    ''' <item><term><c>ndarrayfactory.class</c></term><description><see cref="NDARRAY_FACTORY_CLASS_NAME"/> (<c>ManagedNDArrayFactory</c>)</description></item>
    ''' <item><term><c>opexec</c></term><description><see cref="ManagedOpExecutioner"/></description></item>
    ''' <item><term><c>blas.ops</c></term><description><see cref="ManagedBlasWrapper"/></description></item>
    ''' <item><term><c>databufferfactory</c></term><description><see cref="ManagedDataBufferFactory"/></description></item>
    ''' <item><term><c>shapeinfoprovider</c></term><description><see cref="ManagedShapeInfoProvider"/></description></item>
    ''' <item><term><c>constantsprovider</c></term><description><see cref="ManagedConstantHandler"/></description></item>
    ''' <item><term><c>affinitymanager</c></term><description><see cref="ManagedAffinityManager"/></description></item>
    ''' <item><term><c>memorymanager</c></term><description><see cref="ManagedMemoryManager"/></description></item>
    ''' <item><term><c>workspacemanager</c></term><description><see cref="ManagedWorkspaceManager"/></description></item>
    ''' <item><term><c>random</c></term><description><see cref="ManagedRandom"/></description></item>
    ''' <item><term><c>dtype</c></term><description><c>float</c></description></item>
    ''' <item><term><c>ndarray.order</c></term><description><c>c</c></description></item>
    ''' <item><term><c>compressiondebug</c></term><description><c>false</c></description></item>
    ''' </list>
    ''' Keys Java's file also had and the managed backend leaves out: <c>native.ops</c> (native-only), <c>alloc</c>,
    ''' <c>opexec.mode</c>, <c>resourcemanager_state</c>, <c>fft</c>; the convolution instance and the distribution factory
    ''' take <c>Nd4vb</c>'s defaults, as in Java.</para>
    ''' <para>As <c>CpuBackend</c>, the backend does not let BLAS calls specify the order (<see cref="allowsOrder"/> is
    ''' False). There are no native libraries to load.</para>
    ''' </remarks>
    Public Class ManagedBackend
        Inherits Nd4vbBackend

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ManagedBackend))

        ''' <summary>The .NET name of the managed backend's array factory (Java <c>CpuNDArrayFactory</c>).</summary>
        Public Const NDARRAY_FACTORY_CLASS_NAME As String = "DeepLearning4VB.org.nd4vb.linalg.managed.ManagedNDArrayFactory"

        ''' <summary>The key of the array factory class (Java <c>Nd4j.NDARRAY_FACTORY_CLASS</c>).</summary>
        Public Const NDARRAY_FACTORY_CLASS_KEY As String = "ndarrayfactory.class"
        ''' <summary>The key of the op executioner class (Java <c>Nd4j.OP_EXECUTIONER</c>).</summary>
        Public Const OP_EXECUTIONER_KEY As String = "opexec"
        ''' <summary>The key of the BLAS wrapper class (Java <c>Nd4j.BLAS_OPS</c>).</summary>
        Public Const BLAS_OPS_KEY As String = "blas.ops"
        ''' <summary>The key of the data buffer factory class (Java <c>Nd4j.DATA_BUFFER_OPS</c>).</summary>
        Public Const DATA_BUFFER_OPS_KEY As String = "databufferfactory"
        ''' <summary>The key of the shape information provider class (Java <c>Nd4j.SHAPEINFO_PROVIDER</c>).</summary>
        Public Const SHAPEINFO_PROVIDER_KEY As String = "shapeinfoprovider"
        ''' <summary>The key of the constant handler class (Java <c>Nd4j.CONSTANT_PROVIDER</c>).</summary>
        Public Const CONSTANT_PROVIDER_KEY As String = "constantsprovider"
        ''' <summary>The key of the affinity manager class (Java <c>Nd4j.AFFINITY_MANAGER</c>).</summary>
        Public Const AFFINITY_MANAGER_KEY As String = "affinitymanager"
        ''' <summary>The key of the memory manager class (Java <c>Nd4j.MEMORY_MANAGER</c>).</summary>
        Public Const MEMORY_MANAGER_KEY As String = "memorymanager"
        ''' <summary>The key of the workspace manager class (Java <c>Nd4j.WORKSPACE_MANAGER</c>).</summary>
        Public Const WORKSPACE_MANAGER_KEY As String = "workspacemanager"
        ''' <summary>The key of the random generator class (Java <c>Nd4j.RANDOM_PROVIDER</c>).</summary>
        Public Const RANDOM_PROVIDER_KEY As String = "random"
        ''' <summary>The key of the default array order (Java <c>Nd4j.ORDER_KEY</c>).</summary>
        Public Const ORDER_KEY As String = "ndarray.order"
        ''' <summary>The key of the compression debug flag (Java <c>Nd4j.COMPRESSION_DEBUG</c>).</summary>
        Public Const COMPRESSION_DEBUG_KEY As String = "compressiondebug"

        ''' <summary>The name the configuration resource reports (Java's file was <c>nd4j-native.properties</c>).</summary>
        Public Const CONFIGURATION_NAME As String = "nd4vb-managed.properties"

        ''' <summary>The backend (cheap: nothing is loaded or created).</summary>
        Public Sub New()
        End Sub

        ''' <summary>Always True: the managed backend runs wherever .NET runs.</summary>
        Public Overrides ReadOnly Property Available As Boolean
            Get
                Return True
            End Get
        End Property

        ''' <summary>Always True.</summary>
        Public Overrides Function canRun() As Boolean
            Return True
        End Function

        ''' <summary>False, as for <c>CpuBackend</c>: BLAS calls are always made in column-major terms.</summary>
        Public Overrides Function allowsOrder() As Boolean
            Return False
        End Function

        ''' <summary>The CPU backend priority (<see cref="Nd4vbBackend.BACKEND_PRIORITY_CPU"/>).</summary>
        Public Overrides ReadOnly Property Priority As Integer
            Get
                Return BACKEND_PRIORITY_CPU
            End Get
        End Property

        ''' <summary>The in-memory configuration (<see cref="configurationText"/>).</summary>
        Public Overrides ReadOnly Property ConfigurationResource As Resource
            Get
                Return New TextResource(configurationText(), CONFIGURATION_NAME)
            End Get
        End Property

        ''' <summary>The array class of the managed backend (<c>ManagedNDArray</c>).</summary>
        Public Overrides ReadOnly Property NDArrayClass As Type
            Get
                Return GetType(ManagedNDArray)
            End Get
        End Property

        ''' <summary>The managed environment (<see cref="ManagedEnvironment.Instance"/>).</summary>
        Public Overrides ReadOnly Property Environment As EnvironmentT
            Get
                Return ManagedEnvironment.Instance
            End Get
        End Property

        ''' <summary>The library version, the .NET runtime, the SIMD state and the kernel thread count.</summary>
        Public Overrides Function buildInfo() As String
            Dim asm As Assembly = GetType(ManagedBackend).Assembly
            Dim sb As New StringBuilder()
            sb.Append("ND4VB managed backend (").Append(asm.GetName().Name).Append(" ").Append(asm.GetName().Version.ToString()).Append(")").Append(vbLf)
            sb.Append("Runtime: ").Append(Global.System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription).Append(", ").Append(If(Global.System.Environment.Is64BitProcess, "64-bit", "32-bit")).Append(vbLf)
            sb.Append("SIMD: ").Append(If(KernelOptions.SimdEnabled, "enabled", "disabled")).Append("; threads: ").Append(KernelOptions.MaxDegreeOfParallelism)
            Return sb.ToString()
        End Function

        ''' <summary>Logs <see cref="buildInfo"/> unless the system property <c>org.nd4vb.log.initialization</c> is not "true".</summary>
        Public Overrides Sub logBackendInit()
            Dim logInitProperty As String = JavaSystem.getProperty(ND4VBSystemProperties.LOG_INITIALIZATION, "true")
            Dim logInit As Boolean = String.Equals(logInitProperty, "true", StringComparison.OrdinalIgnoreCase)

            If logInit Then
                Try
                    log.info("Backend build information:" & vbLf & " {}", buildInfo())
                Catch t As System.Exception
                    log.debug("Error logging managed backend ", t)
                End Try
            End If
        End Sub

        ''' <summary>The configuration properties (see the remarks) in the Java <c>.properties</c> format.</summary>
        Public Shared Function configurationText() As String
            Dim sb As New StringBuilder()
            sb.Append("# ND4VB managed backend configuration (counterpart of nd4j-native.properties)").Append(vbLf)
            sb.Append(ND4VBSystemProperties.DTYPE).Append(" = float").Append(vbLf)
            sb.Append(ORDER_KEY).Append(" = c").Append(vbLf)
            sb.Append(COMPRESSION_DEBUG_KEY).Append(" = false").Append(vbLf)
            sb.Append(NDARRAY_FACTORY_CLASS_KEY).Append(" = ").Append(NDARRAY_FACTORY_CLASS_NAME).Append(vbLf)
            sb.Append(OP_EXECUTIONER_KEY).Append(" = ").Append(GetType(ManagedOpExecutioner).FullName).Append(vbLf)
            sb.Append(BLAS_OPS_KEY).Append(" = ").Append(GetType(ManagedBlasWrapper).FullName).Append(vbLf)
            sb.Append(DATA_BUFFER_OPS_KEY).Append(" = ").Append(GetType(ManagedDataBufferFactory).FullName).Append(vbLf)
            sb.Append(SHAPEINFO_PROVIDER_KEY).Append(" = ").Append(GetType(ManagedShapeInfoProvider).FullName).Append(vbLf)
            sb.Append(CONSTANT_PROVIDER_KEY).Append(" = ").Append(GetType(ManagedConstantHandler).FullName).Append(vbLf)
            sb.Append(AFFINITY_MANAGER_KEY).Append(" = ").Append(GetType(ManagedAffinityManager).FullName).Append(vbLf)
            sb.Append(MEMORY_MANAGER_KEY).Append(" = ").Append(GetType(ManagedMemoryManager).FullName).Append(vbLf)
            sb.Append(WORKSPACE_MANAGER_KEY).Append(" = ").Append(GetType(ManagedWorkspaceManager).FullName).Append(vbLf)
            sb.Append(RANDOM_PROVIDER_KEY).Append(" = ").Append(GetType(ManagedRandom).FullName).Append(vbLf)
            Return sb.ToString()
        End Function

        ''' <summary>A resource whose content is a string (UTF-8).</summary>
        Private NotInheritable Class TextResource
            Inherits AbstractResource

            Private ReadOnly text As String
            Private ReadOnly name As String

            Friend Sub New(ByVal text As String, ByVal name As String)
                Me.text = text
                Me.name = name
            End Sub

            Public Overrides ReadOnly Property InputStream As Stream
                Get
                    Return New MemoryStream(Encoding.UTF8.GetBytes(text), False)
                End Get
            End Property

            Public Overrides ReadOnly Property Description As String
                Get
                    Return "in-memory resource [" & name & "]"
                End Get
            End Property

            Public Overrides ReadOnly Property Filename As String
                Get
                    Return name
                End Get
            End Property

            Public Overrides Function exists() As Boolean
                Return True
            End Function
        End Class

    End Class

End Namespace
