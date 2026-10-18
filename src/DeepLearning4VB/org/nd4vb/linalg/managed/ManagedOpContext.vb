' DeepLearning4VB: the op context of the managed ND4VB backend (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.ops.CpuOpContext.

Imports System
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports BaseOpContext = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseOpContext

Namespace org.nd4vb.linalg.managed

    ''' <summary>
    ''' The op context of the managed backend (Java <c>CpuOpContext</c>): the input and output arrays and the arguments of
    ''' a custom op call (kept by <see cref="BaseOpContext"/>), the RNG states the op starts from and ends with, and the
    ''' execution flags. An array index that was never set reads as Nothing.
    ''' </summary>
    ''' <remarks>
    ''' <para>Java kept the flags and the RNG state in a libnd4j graph context; here they are fields: <see cref="Inplace"/>,
    ''' <see cref="HelpersAllowed"/> (True by default, as in libnd4j), <see cref="ShapeFunctionOverridden"/> and
    ''' <see cref="RngStates"/> (0 and 0 until set). <see cref="Dispose"/> forgets the arrays; the context can be used
    ''' again afterwards.</para>
    ''' <para>Difference from Java: setting an empty argument array clears the arguments, as <see cref="BaseOpContext"/>
    ''' does (<c>CpuOpContext</c> ignored empty arrays and kept the previous arguments).</para>
    ''' <para>Native-only members not ported: <c>contextPointer()</c>, the <c>Deallocatable</c> members
    ''' (<c>getUniqueId</c>, <c>deallocator</c>, <c>targetDevice</c>) and <c>CpuOpContextDeallocator</c>.</para>
    ''' </remarks>
    Public Class ManagedOpContext
        Inherits BaseOpContext

        Private ReadOnly gate As New Object()
        Private rootStateField As Long
        Private nodeStateField As Long
        Private inplaceField As Boolean
        Private helpersAllowedField As Boolean = True
        Private shapeFunctionOverrideField As Boolean

        ''' <summary>Marks the call as in place: a custom op without output 0 writes into input 0.</summary>
        Public Overrides Sub markInplace(ByVal reallyInplace As Boolean)
            inplaceField = reallyInplace
        End Sub

        ''' <summary>True when the call was marked in place.</summary>
        Public ReadOnly Property Inplace As Boolean
            Get
                Return inplaceField
            End Get
        End Property

        ''' <summary>Allows or forbids platform helpers (libnd4j's MKL-DNN/cuDNN switch; the managed kernels have none).</summary>
        Public Overrides Sub allowHelpers(ByVal reallyAllow As Boolean)
            helpersAllowedField = reallyAllow
        End Sub

        ''' <summary>True unless helpers were forbidden with <see cref="allowHelpers"/>.</summary>
        Public ReadOnly Property HelpersAllowed As Boolean
            Get
                Return helpersAllowedField
            End Get
        End Property

        ''' <summary>Skips the shape checks of the op's shape function (the outputs were allocated from it).</summary>
        Public Overrides Sub shapeFunctionOverride(ByVal reallyOverride As Boolean)
            shapeFunctionOverrideField = reallyOverride
        End Sub

        ''' <summary>True when <see cref="shapeFunctionOverride"/> was set.</summary>
        Public ReadOnly Property ShapeFunctionOverridden As Boolean
            Get
                Return shapeFunctionOverrideField
            End Get
        End Property

        ''' <summary>The root and node states of the context's random generator.</summary>
        Public Overrides ReadOnly Property RngStates As Pair(Of Long, Long)
            Get
                SyncLock gate
                    Return New Pair(Of Long, Long)(rootStateField, nodeStateField)
                End SyncLock
            End Get
        End Property

        ''' <summary>Sets the root and node states of the context's random generator.</summary>
        Public Overrides Sub setRngStates(ByVal rootState As Long, ByVal nodeState As Long)
            SyncLock gate
                rootStateField = rootState
                nodeStateField = nodeState
            End SyncLock
        End Sub

        ''' <summary>Forgets the input and output arrays (Java's <c>close()</c> did nothing; the native context was freed by the deallocator).</summary>
        Public Overrides Sub Dispose()
            purge()
        End Sub

    End Class

End Namespace
