Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DeepLearning4VB.org.nd4vb.common.primitives

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' This interface describes OpContext, abstraction used to setup op for execution.
    ''' <para>
    ''' Differences from Java: <c>AutoCloseable</c> is <see cref="IDisposable"/>; the getters and setters of the
    ''' arguments and arrays keep their Java names and types (<c>setIArguments(long...)</c> /
    ''' <c>List&lt;Long&gt; getIArguments()</c>, ...) because their types differ; the native-only
    ''' <c>contextPointer()</c> is deleted.
    ''' </para>
    ''' </summary>
    Public Interface OpContext
        Inherits IDisposable

        ''' <summary>
        ''' This method sets integer arguments required for operation
        ''' </summary>
        ''' <param name="arguments"> </param>
        Sub setIArguments(ParamArray ByVal arguments() As Long)

        Function getIArguments() As IList(Of Long)

        Function numIArguments() As Integer

        ''' <summary>
        ''' This method sets floating point arguments required for operation
        ''' </summary>
        ''' <param name="arguments"> </param>
        Sub setTArguments(ParamArray ByVal arguments() As Double)

        Function getTArguments() As IList(Of Double)

        Function numTArguments() As Integer

        ''' <summary>
        ''' This method sets data type arguments required for operation
        ''' </summary>
        ''' <param name="arguments"> </param>
        Sub setDArguments(ParamArray ByVal arguments() As DataType)

        Function getDArguments() As IList(Of DataType)

        Function numDArguments() As Integer

        ''' <summary>
        ''' This method sets boolean arguments required for operation
        ''' </summary>
        ''' <param name="arguments"> </param>
        Sub setBArguments(ParamArray ByVal arguments() As Boolean)

        Function getBArguments() As IList(Of Boolean)

        Function numBArguments() As Integer

        ''' <summary>
        ''' This method sets root-level seed for rng </summary>
        ''' <param name="seed"> </param>
        Sub setRngStates(ByVal rootState As Long, ByVal nodeState As Long)

        ''' <summary>
        ''' This method returns RNG states, root first node second
        ''' </summary>
        ReadOnly Property RngStates As Pair(Of Long, Long)

        ''' <summary>
        ''' This method adds INDArray as input argument for future op call
        ''' </summary>
        ''' <param name="index"> </param>
        ''' <param name="array"> </param>
        Sub setInputArray(ByVal index As Integer, ByVal array As INDArray)

        ''' <summary>
        ''' This method sets provided arrays as input arrays </summary>
        ''' <param name="arrays"> </param>
        Sub setInputArrays(ByVal arrays As IList(Of INDArray))

        ''' <summary>
        ''' This method sets provided arrays as input arrays </summary>
        ''' <param name="arrays"> </param>
        Sub setInputArrays(ParamArray ByVal arrays() As INDArray)

        ''' <summary>
        ''' This method returns List of input arrays defined within this context
        ''' </summary>
        Function getInputArrays() As IList(Of INDArray)

        Function numInputArguments() As Integer

        ''' <summary>
        ''' This method adds INDArray as output for future op call </summary>
        ''' <param name="idx"> </param>
        Function getInputArray(ByVal idx As Integer) As INDArray

        ''' <summary>
        ''' This method adds INDArray as output for future op call </summary>
        ''' <param name="index"> </param>
        ''' <param name="array"> </param>
        Sub setOutputArray(ByVal index As Integer, ByVal array As INDArray)

        ''' <summary>
        ''' This method sets provided arrays as output arrays </summary>
        ''' <param name="arrays"> </param>
        Sub setOutputArrays(ByVal arrays As IList(Of INDArray))

        ''' <summary>
        ''' This method sets provided arrays as output arrays </summary>
        ''' <param name="arrays"> </param>
        Sub setOutputArrays(ParamArray ByVal arrays() As INDArray)

        ''' <summary>
        ''' This method returns List of output arrays defined within this context
        ''' </summary>
        Function getOutputArrays() As IList(Of INDArray)

        Function getOutputArray(ByVal i As Integer) As INDArray

        Function numOutputArguments() As Integer

        ''' <summary>
        ''' This method allows to set op as inplace </summary>
        ''' <param name="reallyInplace"> </param>
        Sub markInplace(ByVal reallyInplace As Boolean)

        ''' <summary>
        ''' This method allows to enable/disable use of platform helpers within ops. I.e. mkldnn or cuDNN.
        ''' PLEASE NOTE: default value is True
        ''' </summary>
        ''' <param name="reallyAllow"> </param>
        Sub allowHelpers(ByVal reallyAllow As Boolean)

        ''' <summary>
        ''' This methos allows to disape outputs validation via shape function
        ''' PLEASE NOTE: this is very special case, for not-very-correctly implemented ops only
        ''' </summary>
        ''' <param name="reallyOverride"> </param>
        Sub shapeFunctionOverride(ByVal reallyOverride As Boolean)

        ''' <summary>
        ''' Returns the execution mode for this context (Java <c>getExecutionMode()</c>).
        ''' </summary>
        Function getExecutionMode() As ExecutionMode

        ''' <summary>
        ''' Set the execution mode for the Op. Note that this is only relevant for some ops, such as Dropout. </summary>
        ''' <param name="executionMode"> Execution mode </param>
        Sub setExecutionMode(ByVal executionMode As ExecutionMode)

        ''' <summary>
        ''' This method removes all in/out arrays from this OpContext
        ''' </summary>
        Sub purge()

    End Interface

End Namespace
