Imports System
Imports System.Collections.Generic
Imports Properties = DeepLearning4VB.JavaCompat.Properties
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports INDArrayStatistics = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArrayStatistics
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports Aggregate = DeepLearning4VB.org.nd4vb.linalg.api.ops.aggregates.Aggregate
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.aggregates
Imports RandomT = DeepLearning4VB.org.nd4vb.linalg.api.rng.Random
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports TadPack = DeepLearning4VB.org.nd4vb.linalg.api.shape.TadPack
Imports TADManagerT = DeepLearning4VB.org.nd4vb.linalg.cache.TADManager
Imports ProfilerConfig = DeepLearning4VB.org.nd4vb.linalg.profiler.ProfilerConfig

Namespace org.nd4vb.linalg.api.ops.executioner

    ''' <summary>
    ''' An operation executioner handles storage specific details of
    ''' executing an operation
    ''' <para>
    ''' Differences from the Java interface:
    ''' <list type="bullet">
    ''' <item><see cref="ExecutionerType"/> has the extra value <c>MANAGED</c> for the managed backend of ND4VB.</item>
    ''' <item>The deprecated <c>getProfilingMode()</c>/<c>setProfilingMode()</c> stay methods (a <c>ProfilingMode</c>
    ''' property would clash with the nested enum).</item>
    ''' <item>The <c>exec(Variance)</c> and <c>execAndReturn(Variance)</c> overloads are left out: <c>Variance</c> is
    ''' a reduce op of a later milestone (M2), and a <c>Variance</c> passes to <see cref="exec(ReduceOp)"/> and
    ''' <see cref="execAndReturn(ReduceOp)"/>, which implementations must handle like Java's Variance overloads
    ''' (bias-corrected summary statistics).</item>
    ''' <item>Deleted as native-only: <c>registerGraph(long, Pointer)</c> (a JavaCPP graph pointer) and the deprecated
    ''' "temporary hook" <c>scatterUpdate(ScatterUpdate.UpdateOp, ...)</c>, which called NativeOps directly.</item>
    ''' <item><c>Properties</c> is <c>JavaCompat.Properties</c>.</item>
    ''' </list>
    ''' </para>
    ''' </summary>
    Public Interface OpExecutioner

        ''' <summary>The kinds of executioner (in case of adding new executioner - list it here).</summary>
        Enum ExecutionerType
            NATIVE_CPU
            CUDA
            ''' <summary>The managed (pure .NET) executioner of ND4VB.</summary>
            MANAGED
        End Enum

        Enum ProfilingMode
            DISABLED
            NAN_PANIC
            INF_PANIC
            ANY_PANIC
            OPERATIONS
            METHODS
            ALL
            SCOPE_PANIC
            BANDWIDTH
        End Enum

        ''' <summary>
        ''' This method returns true if verbose mode is enabled, false otherwise
        ''' </summary>
        ReadOnly Property Verbose As Boolean

        ''' <summary>
        ''' This method returns true if debug mode is enabled, false otherwise
        ''' </summary>
        ReadOnly Property Debug As Boolean

        ''' <summary>
        ''' This method returns type for this executioner instance
        ''' </summary>
        Function type() As ExecutionerType

        ''' <summary>
        ''' This method returns opName of the last invoked op
        ''' </summary>
        ReadOnly Property LastOp As String

        ''' <summary>
        ''' Execute the operation
        ''' </summary>
        ''' <param name="op"> the operation to execute </param>
        Function exec(ByVal op As Op) As INDArray

        ''' <summary>
        ''' Execute the operation
        ''' </summary>
        ''' <param name="op"> the operation to execute </param>
        Function exec(ByVal op As Op, ByVal opContext As OpContext) As INDArray

        ''' <summary>
        ''' Execute a TransformOp and return the result </summary>
        ''' <param name="op"> the operation to execute </param>
        Function execAndReturn(ByVal op As TransformOp) As TransformOp

        ''' <summary>
        ''' Execute and return the result from an accumulation
        ''' </summary>
        ''' <param name="op"> the operation to execute </param>
        ''' <returns> the accumulated result </returns>
        Function execAndReturn(ByVal op As ReduceOp) As ReduceOp

        ''' <summary>
        ''' Execute and return the result from an index accumulation </summary>
        ''' <param name="op"> the index accumulation operation to execute </param>
        ''' <returns> the accumulated index </returns>
        Function execAndReturn(ByVal op As IndexAccumulation) As IndexAccumulation

        ''' <summary>
        ''' Execute and return the result from a scalar op
        ''' </summary>
        ''' <param name="op"> the operation to execute </param>
        ''' <returns> the accumulated result </returns>
        Function execAndReturn(ByVal op As ScalarOp) As ScalarOp

        ''' <summary>
        ''' Execute and return the result from a vector op </summary>
        ''' <param name="op"> </param>
        Function execAndReturn(ByVal op As BroadcastOp) As BroadcastOp

        ''' <summary>
        ''' Execute a reduceOp, possibly along one or more dimensions </summary>
        ''' <param name="reduceOp"> the reduceOp </param>
        ''' <returns> the reduceOp op </returns>
        Function exec(ByVal reduceOp As ReduceOp) As INDArray

        ''' <summary>
        ''' Execute a broadcast op, possibly along one or more dimensions </summary>
        ''' <param name="broadcast"> the accumulation </param>
        ''' <returns> the broadcast op </returns>
        Function exec(ByVal broadcast As BroadcastOp) As INDArray

        ''' <summary>
        ''' Execute ScalarOp </summary>
        ''' <param name="broadcast"> </param>
        Function exec(ByVal broadcast As ScalarOp) As INDArray

        ''' <summary>
        ''' Execute an index accumulation along one or more dimensions </summary>
        ''' <param name="indexAccum"> the index accumulation operation </param>
        ''' <returns> result </returns>
        Function exec(ByVal indexAccum As IndexAccumulation) As INDArray

        ''' <summary>
        ''' Execute and return  a result
        ''' ndarray from the given op </summary>
        ''' <param name="op"> the operation to execute </param>
        ''' <returns> the result from the operation </returns>
        Function execAndReturn(ByVal op As Op) As Op

        ''' <summary>
        ''' Execute MetaOp
        ''' </summary>
        ''' <param name="op"> </param>
        Sub exec(ByVal op As MetaOp)

        ''' <summary>
        ''' Execute GridOp </summary>
        ''' <param name="op"> </param>
        Sub exec(ByVal op As GridOp)

        ''' <param name="op"> </param>
        Sub exec(ByVal op As Aggregate)

        ''' <summary>
        ''' This method executes previously built batch
        ''' </summary>
        ''' <param name="batch"> </param>
        Sub exec(Of T As Aggregate)(ByVal batch As Batch(Of T))

        ''' <summary>
        ''' This method takes arbitrary sized list of aggregates,
        ''' and packs them into batches
        ''' </summary>
        ''' <param name="batch"> </param>
        Sub exec(ByVal batch As IList(Of Aggregate))

        ''' <summary>
        ''' This method executes specified RandomOp using default RNG available via Nd4j.getRandom()
        ''' </summary>
        ''' <param name="op"> </param>
        Function exec(ByVal op As RandomOp) As INDArray

        ''' <summary>
        ''' This method executes specific RandomOp against specified RNG
        ''' </summary>
        ''' <param name="op"> </param>
        ''' <param name="rng"> </param>
        Function exec(ByVal op As RandomOp, ByVal rng As RandomT) As INDArray

        ''' <summary>
        ''' This method return set of key/value and
        ''' key/key/value objects,
        ''' describing current environment
        ''' </summary>
        ReadOnly Property EnvironmentInformation As Properties

        ''' <summary>
        ''' This method specifies desired profiling mode (deprecated in Java: use <see cref="ProfilingConfig"/>).
        ''' </summary>
        ''' <param name="mode"> </param>
        Sub setProfilingMode(ByVal mode As ProfilingMode)

        ''' <summary>
        ''' Returns the profiling mode (deprecated in Java).
        ''' </summary>
        Function getProfilingMode() As ProfilingMode

        ''' <summary>
        ''' This method stores specified configuration.
        ''' </summary>
        ''' <param name="config"> </param>
        WriteOnly Property ProfilingConfig As ProfilerConfig

        ''' <summary>
        ''' This method returns TADManager instance used for this OpExecutioner
        ''' </summary>
        ReadOnly Property TADManager As TADManagerT

        ''' <summary>
        ''' This method prints out environmental information returned by getEnvironmentInformation() method
        ''' </summary>
        Sub printEnvironmentInformation()

        ''' <summary>
        ''' This method ensures all operations that supposed to be executed at this moment, are executed.
        ''' </summary>
        Sub push()

        ''' <summary>
        ''' This method ensures all operations that supposed to be executed at this moment, are executed and finished.
        ''' </summary>
        Sub commit()

        ''' <summary>
        ''' This method encodes array as thresholds, updating input array at the same time
        ''' </summary>
        ''' <param name="input"> </param>
        ''' <returns> encoded array is returned </returns>
        Function thresholdEncode(ByVal input As INDArray, ByVal threshold As Double) As INDArray

        ''' <summary>
        ''' This method encodes array as thresholds, updating input array at the same time
        ''' </summary>
        ''' <param name="input"> </param>
        ''' <returns> encoded array is returned </returns>
        Function thresholdEncode(ByVal input As INDArray, ByVal threshold As Double, ByVal boundary As Integer?) As INDArray

        ''' <summary>
        ''' This method decodes thresholds array, and puts it into target array
        ''' </summary>
        ''' <param name="encoded"> </param>
        ''' <param name="target"> </param>
        ''' <returns> target is returned </returns>
        Function thresholdDecode(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray

        ''' <summary>
        ''' This method returns number of elements affected by encoder </summary>
        ''' <param name="indArray"> </param>
        ''' <param name="target"> </param>
        ''' <param name="threshold"> </param>
        Function bitmapEncode(ByVal indArray As INDArray, ByVal target As INDArray, ByVal threshold As Double) As Long

        ''' <param name="indArray"> </param>
        ''' <param name="threshold"> </param>
        Function bitmapEncode(ByVal indArray As INDArray, ByVal threshold As Double) As INDArray

        ''' <param name="encoded"> </param>
        ''' <param name="target"> </param>
        Function bitmapDecode(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray

        ''' <summary>
        ''' This method returns names of all custom operations available in current backend, and their number of input/output arguments
        ''' </summary>
        ReadOnly Property CustomOperations As IDictionary(Of String, CustomOpDescriptor)

        ''' <summary>
        ''' This method executes given CustomOp
        '''
        ''' PLEASE NOTE: You're responsible for input/output validation </summary>
        ''' <param name="op"> </param>
        Function execAndReturn(ByVal op As CustomOp) As CustomOp

        Function exec(ByVal op As CustomOp) As INDArray()

        ''' <summary>
        ''' This method executes op with given context </summary>
        ''' <param name="op"> </param>
        ''' <param name="context"> </param>
        ''' <returns> method returns output arrays defined within context </returns>
        Function exec(ByVal op As CustomOp, ByVal context As OpContext) As INDArray()

        Function calculateOutputShape(ByVal op As CustomOp) As IList(Of LongShapeDescriptor)

        Function calculateOutputShape(ByVal op As CustomOp, ByVal opContext As OpContext) As IList(Of LongShapeDescriptor)

        ''' <summary>
        ''' Equivalent to calli
        ''' </summary>
        Function allocateOutputArrays(ByVal op As CustomOp) As INDArray()

        Sub enableDebugMode(ByVal reallyEnable As Boolean)

        Sub enableVerboseMode(ByVal reallyEnable As Boolean)

        ReadOnly Property ExperimentalMode As Boolean

        Function executeGraph(ByVal id As Long, ByVal map As IDictionary(Of String, INDArray), ByVal reverseMap As IDictionary(Of String, Integer)) As IDictionary(Of String, INDArray)

        Sub forgetGraph(ByVal id As Long)

        ''' <summary>
        ''' This method allows to set desired number of elements per thread, for performance optimization purposes.
        ''' I.e. if array contains 2048 elements, and threshold is set to 1024, 2 threads will be used for given op execution.
        '''
        ''' Default value: 1024
        ''' </summary>
        ''' <param name="threshold"> </param>
        WriteOnly Property ElementsThreshold As Integer

        ''' <summary>
        ''' This method allows to set desired number of sub-arrays per thread, for performance optimization purposes.
        ''' I.e. if matrix has shape of 64 x 128, and threshold is set to 8, each thread will be processing 8 sub-arrays (sure, if you have 8 core cpu).
        ''' If your cpu has, say, 4, cores, only 4 threads will be spawned, and each will process 16 sub-arrays
        '''
        ''' Default value: 8 </summary>
        ''' <param name="threshold"> </param>
        WriteOnly Property TadThreshold As Integer

        ''' <summary>
        ''' This method extracts String from Utf8Buffer </summary>
        ''' <param name="buffer"> </param>
        ''' <param name="index"> </param>
        Function getString(ByVal buffer As DataBuffer, ByVal index As Long) As String

        ''' <summary>
        ''' This method returns OpContext which can be used (and reused) to execute custom ops
        ''' </summary>
        Function buildContext() As OpContext

        ''' <param name="array"> </param>
        Function inspectArray(ByVal array As INDArray) As INDArrayStatistics

        ''' <summary>
        ''' This method returns shapeInfo DataBuffer
        ''' </summary>
        ''' <param name="shape"> </param>
        ''' <param name="stride"> </param>
        ''' <param name="elementWiseStride"> </param>
        ''' <param name="order"> </param>
        ''' <param name="dtype"> </param>
        Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal empty As Boolean) As DataBuffer

        Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal extra As Long) As DataBuffer

        ''' <summary>
        ''' This method returns host/device tad buffers
        ''' </summary>
        Function tadShapeInfoAndOffsets(ByVal array As INDArray, ByVal dimension() As Integer) As TadPack

        ''' <summary>
        ''' This method returns constant buffer for the given jvm array </summary>
        ''' <param name="values"> </param>
        Function createConstantBuffer(ByVal values() As Long, ByVal desiredType As DataType) As DataBuffer

        Function createConstantBuffer(ByVal values() As Integer, ByVal desiredType As DataType) As DataBuffer

        Function createConstantBuffer(ByVal values() As Single, ByVal desiredType As DataType) As DataBuffer

        Function createConstantBuffer(ByVal values() As Double, ByVal desiredType As DataType) As DataBuffer

    End Interface

End Namespace
