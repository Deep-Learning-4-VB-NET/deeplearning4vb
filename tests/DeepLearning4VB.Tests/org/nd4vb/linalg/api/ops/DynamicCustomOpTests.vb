Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports MemcpyDirection = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemcpyDirection
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.aggregates.impl
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.performance.primitives
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports OpType = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op.Type

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' <see cref="DynamicCustomOp"/> and its builder (op name, inputs, outputs, iArgs, tArgs, bArgs, dArgs, sArgs),
    ''' <see cref="NoOp"/>, <see cref="BaseOpContext"/>, the aggregates and the bandwidth averages.
    ''' </summary>
    <TestClass>
    Public Class DynamicCustomOpTests

        ''' <summary>A builder as <c>DynamicCustomOp.builder(name)</c> makes it from a descriptor (the custom op table is the executioner's).</summary>
        Private Shared Function NewBuilder(ByVal numInputs As Integer, ByVal numOutputs As Integer, ByVal inplaceAllowed As Boolean,
                                           ByVal numT As Integer, ByVal numI As Integer) As DynamicCustomOp.DynamicCustomOpsBuilder
            Return New DynamicCustomOp.DynamicCustomOpsBuilder("my_op", 12345L, numInputs, numOutputs, inplaceAllowed, numT, numI)
        End Function

        <TestMethod>
        Public Sub BuilderCollectsTheArguments()
            Dim x As INDArray = FakeArray.Simple()
            Dim y As INDArray = FakeArray.Simple()
            Dim z As INDArray = FakeArray.Simple()
            Dim op As DynamicCustomOp = NewBuilder(-1, -1, True, -1, -1).
                addInputs(x, y).
                addOutputs(z).
                addIntegerArguments(1, 2).
                addIntegerArguments(3L).
                addIntegerArguments(New List(Of Integer) From {4}).
                addFloatingPointArguments(0.5, 1.5).
                addBooleanArguments(True, False).
                addStringArguments("a", "b").
                callInplace(True).
                build()

            assertEquals("my_op", op.opName())
            assertEquals("my_op", op.ToString())
            assertEquals(12345L, op.getHash())
            assertEquals(12345L, op.opHash())
            assertEquals(12345, op.opNum())
            assertEquals(2, op.numInputArguments())
            assertTrue(op.getInputArgument(0) Is x)
            assertTrue(op.getInputArgument(1) Is y)
            assertNull(op.getInputArgument(2))
            assertEquals(1, op.numOutputArguments())
            assertTrue(op.getOutputArgument(0) Is z)
            assertArrayEquals(New Long() {1, 2, 3, 4}, op.iArgs())
            assertArrayEquals(New Double() {0.5, 1.5}, op.tArgs(), 0.0)
            assertArrayEquals(New Boolean() {True, False}, op.bArgs())
            assertArrayEquals(New String() {"a", "b"}, op.sArgs())
            assertEquals(0, op.dArgs().Length)
            assertTrue(op.InplaceCall)
            assertEquals(OpType.CUSTOM, op.opType())
        End Sub

        <TestMethod>
        Public Sub BuilderChecksTheDescriptorCounts()
            assertThrows(Of ND4VBIllegalStateException)(Function() NewBuilder(2, -1, False, -1, -1).addInputs(FakeArray.Simple()))
            assertThrows(Of ND4VBIllegalStateException)(Function() NewBuilder(-1, 1, False, -1, -1).addOutputs())
            assertThrows(Of ND4VBIllegalStateException)(Function() NewBuilder(-1, -1, False, -1, 2).addIntegerArguments(1))
            assertThrows(Of ND4VBIllegalStateException)(Function() NewBuilder(-1, -1, False, -1, 3).addIntegerArguments(1, 2))
            assertThrows(Of ND4VBIllegalStateException)(Function() NewBuilder(-1, -1, False, 2, -1).addFloatingPointArguments(1.0))
            assertThrows(Of ND4VBIllegalStateException)(Function() NewBuilder(-1, -1, False, -1, -1).callInplace(True))
            ' exactly the descriptor counts are fine
            Dim op As DynamicCustomOp = NewBuilder(1, 1, False, 1, 1).addInputs(FakeArray.Simple()).addOutputs(FakeArray.Simple()).
                addIntegerArguments(7L).addFloatingPointArguments(2.0).build()
            assertArrayEquals(New Long() {7}, op.iArgs())
            assertFalse(op.InplaceCall)
        End Sub

        <TestMethod>
        Public Sub ArgumentsCanBeAddedAndRemoved()
            Dim op As New DynamicCustomOp("my_op", New INDArray() {}, New INDArray() {})
            op.addIArgument(5, 6)
            op.addIArgument(7L)
            op.addTArgument(1.0, 2.0)
            op.addBArgument(True)
            op.addDArgument(DataType.FLOAT, DataType.INT)
            op.addSArgument("s")
            assertArrayEquals(New Long() {5, 6, 7}, op.iArgs())
            assertEquals(6L, op.getIArgument(1).Value)
            assertEquals(3, op.numIArguments())
            assertEquals(2.0, op.getTArgument(1).Value, 0.0)
            assertTrue(op.getBArgument(0).Value)
            assertArrayEquals(New DataType() {DataType.FLOAT, DataType.INT}, op.dArgs())
            assertEquals(2, op.numDArguments())
            assertEquals("s", op.getSArgument(0))
            assertEquals(1, op.numSArguments())

            op.removeIArgument(6)
            assertArrayEquals(New Long() {5, 7}, op.iArgs())
            op.removeTArgument(1.0)
            assertArrayEquals(New Double() {2.0}, op.tArgs(), 0.0)
            op.removeSArgument("s")
            assertEquals(0, op.numSArguments())
        End Sub

        <TestMethod>
        Public Sub ConstructorsStoreInputsOutputsAndArguments()
            Dim x As INDArray = FakeArray.Simple()
            Dim z As INDArray = FakeArray.Simple()
            Dim op As New DynamicCustomOp("my_op", x, z, New List(Of Double) From {0.25}, New Integer() {3, 4})
            assertTrue(op.inputArguments()(0) Is x)
            assertTrue(op.outputArguments()(0) Is z)
            assertArrayEquals(New Double() {0.25}, op.tArgs(), 0.0)
            assertArrayEquals(New Long() {3, 4}, op.iArgs())
            assertEquals(0, op.bArgs().Length)

            Dim noArrays As New DynamicCustomOp("my_op", CType(Nothing, INDArray), CType(Nothing, INDArray), Nothing, CType(Nothing, Integer()))
            assertEquals(0, noArrays.numInputArguments())
            assertEquals(0, noArrays.numOutputArguments())
            assertEquals(0, noArrays.numTArguments())
        End Sub

        <TestMethod>
        Public Sub InputAndOutputSlotsGrowOnDemand()
            Dim op As New DynamicCustomOp()
            Dim a As INDArray = FakeArray.Simple()
            Dim b As INDArray = FakeArray.Simple()
            op.setInputArgument(2, a)
            assertEquals(3, op.numInputArguments())
            assertNull(op.getInputArgument(0))
            assertTrue(op.getInputArgument(2) Is a)
            op.setOutputArgument(1, b)
            assertEquals(2, op.numOutputArguments())
            assertTrue(op.getOutputArgument(1) Is b)
            op.setInputArguments(a, b)
            assertEquals(2, op.numInputArguments())
            assertTrue(op.getInputArgument(1) Is b)
            op.removeInputArgument(a)
            assertTrue(op.getInputArgument(0) Is b)
            assertThrows(Of ND4VBIllegalStateException)(Sub() op.addInputArgument(CType(Nothing, INDArray)))
            assertThrows(Of ND4VBIllegalStateException)(Sub() op.addOutputArgument(CType(Nothing, INDArray)))
            op.clearArrays()
            assertEquals(0, op.numInputArguments())
            assertEquals(0, op.numOutputArguments())
        End Sub

        <TestMethod>
        Public Sub WrapFilterNullDropsNothing()
            Dim a As INDArray = FakeArray.Simple()
            Dim filtered As INDArray() = DynamicCustomOp.wrapFilterNull(a, Nothing, a)
            assertEquals(2, filtered.Length)
            assertNull(DynamicCustomOp.wrapOrNull(CType(Nothing, INDArray)))
            assertEquals(1, DynamicCustomOp.wrapOrNull(a).Length)
        End Sub

        <TestMethod>
        Public Sub NoOpIsACustomOpWithABoolOutput()
            Dim op As New NoOp()
            assertEquals("noop", op.opName())
            assertEquals("NoOp", op.tensorflowName())
            assertEquals("NoOp", op.onnxName())
            assertEquals(1, op.NumOutputs)
            assertArrayEquals(New DataType() {DataType.BOOL}, New List(Of DataType)(op.calculateOutputDataTypes(New List(Of DataType) From {DataType.FLOAT})).ToArray())
            Dim x As INDArray = FakeArray.Simple()
            Dim withInput As New NoOp(x)
            assertTrue(withInput.getInputArgument(0) Is x)
            assertThrows(Of NoOpNameFoundException)(Function() New DynamicCustomOp("my_op").tensorflowName())
        End Sub

        ''' <summary>The parts a backend context adds, as no-ops.</summary>
        Private NotInheritable Class TestContext
            Inherits BaseOpContext

            Public Overrides Sub shapeFunctionOverride(ByVal reallyOverride As Boolean)
            End Sub

            Public Overrides Sub allowHelpers(ByVal reallyAllow As Boolean)
            End Sub

            Public Overrides Sub markInplace(ByVal reallyInplace As Boolean)
            End Sub

            Public Overrides ReadOnly Property RngStates As Pair(Of Long, Long)
                Get
                    Return Nothing
                End Get
            End Property

            Public Overrides Sub setRngStates(ByVal rootState As Long, ByVal nodeState As Long)
            End Sub

            Public Overrides Sub Dispose()
            End Sub
        End Class

        <TestMethod>
        Public Sub OpContextKeepsArgumentsAndArrays()
            Using ctx As New TestContext()
                ctx.setIArguments(1, 2)
                ctx.setTArguments(0.5)
                ctx.setBArguments(True, True, False)
                ctx.setDArguments(DataType.DOUBLE)
                assertEquals(New List(Of Long) From {1, 2}, ctx.getIArguments())
                assertEquals(2, ctx.numIArguments())
                assertEquals(1, ctx.numTArguments())
                assertEquals(3, ctx.numBArguments())
                assertEquals(1, ctx.numDArguments())
                ctx.setIArguments(9)
                assertEquals(New List(Of Long) From {9}, ctx.getIArguments())

                Dim a As INDArray = FakeArray.Simple()
                Dim b As INDArray = FakeArray.Simple()
                ctx.setInputArrays(a, b)
                assertEquals(2, ctx.numInputArguments())
                assertTrue(ctx.getInputArray(1) Is b)
                assertNull(ctx.getInputArray(5))
                ctx.setOutputArray(0, a)
                assertTrue(ctx.getOutputArray(0) Is a)
                assertNull(ctx.getOutputArray(1))
                assertEquals(1, ctx.getOutputArrays().Count)
                ' the array lists stop at the first gap
                ctx.setInputArray(3, a)
                assertEquals(2, ctx.getInputArrays().Count)
                assertEquals(ExecutionMode.UNDEFINED, ctx.getExecutionMode())
                ctx.setExecutionMode(ExecutionMode.INFERENCE)
                assertEquals(ExecutionMode.INFERENCE, ctx.getExecutionMode())
                ctx.purge()
                assertEquals(0, ctx.numInputArguments())
                assertEquals(0, ctx.numOutputArguments())
            End Using
        End Sub

        <TestMethod>
        Public Sub AggregatesKeepTheirArguments()
            Dim a As INDArray = FakeArray.Simple()
            Dim b As INDArray = FakeArray.Simple()
            Dim c As INDArray = FakeArray.Simple()
            Dim gemm As New AggregateGEMM(101, 111, 112, 2, 3, 4, 1.5, a, 2, b, 4, 0.5, c, 2)
            assertEquals("aggregate_gemm", gemm.name())
            assertEquals(5, gemm.opNum())
            assertEquals(3, gemm.Arguments.Count)
            assertTrue(gemm.Arguments(2) Is c)
            assertEquals(New List(Of Integer) From {2, 3, 4, 2, 4, 2, 111, 112, 101}, gemm.IndexingArguments)
            assertEquals(New List(Of Object) From {1.5, 0.5}, gemm.RealArguments)
            assertEquals(9, gemm.maxIndexArguments())
            assertEquals(0.0, gemm.FinalResult)
            gemm.FinalResult = 3.0
            assertEquals(3.0, gemm.FinalResult)

            Dim x As INDArray = FakeArray.Create(New Dictionary(Of String, Func(Of Object(), Object)) From {{"length", Function(args) 1000L}})
            Dim axpy As New AggregateAxpy(x, b, 2.0)
            assertEquals("aggregate_axpy", axpy.name())
            assertEquals(New List(Of Integer) From {1000}, axpy.IndexingArguments)
            assertEquals(768, axpy.ThreadsPerInstance)
        End Sub

        <TestMethod>
        Public Sub BandwidthAveragesUseIntegerDivision()
            Dim holder As New AveragingTransactionsHolder()
            assertEquals(0L, holder.getAverageValue(MemcpyDirection.HOST_TO_DEVICE).Value)
            holder.addValue(MemcpyDirection.HOST_TO_DEVICE, 10)
            holder.addValue(MemcpyDirection.HOST_TO_DEVICE, 15)
            holder.addValue(MemcpyDirection.DEVICE_TO_HOST, 100)
            assertEquals(12L, holder.getAverageValue(MemcpyDirection.HOST_TO_DEVICE).Value)
            assertEquals(100L, holder.getAverageValue(MemcpyDirection.DEVICE_TO_HOST).Value)
            holder.clear()
            assertEquals(0L, holder.getAverageValue(MemcpyDirection.DEVICE_TO_HOST).Value)
        End Sub

    End Class

End Namespace
