Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LossReduce = DeepLearning4VB.org.nd4vb.autodiff.loss.LossReduce
Imports WeightsFormat = DeepLearning4VB.org.nd4vb.enums.WeightsFormat
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.broadcast
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.indexaccum.custom
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.config
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.loss
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.scatter
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.shape
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.custom
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.dtype
Imports PadT = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.Pad
Imports PadMode = DeepLearning4VB.org.nd4vb.enums.PadMode

Namespace org.nd4vb.linalg.api.ops.impl

    ''' <summary>
    ''' The arguments that the constructors of representative custom ops give the executioner: input and output
    ''' arrays, iArgs, tArgs, bArgs and dArgs, in the order of the Java constructors (shape, transform, convolution,
    ''' pooling, recurrent, loss, scatter, reduction and data type ops).
    ''' </summary>
    <TestClass>
    Public Class CustomOpArgsTests

        Private Shared Function Arr(Optional ByVal dtype As DataType = DataType.FLOAT) As INDArray
            Return FakeArray.Simple(dtype:=dtype)
        End Function

        Private Shared Sub AssertInputs(ByVal op As DynamicCustomOp, ParamArray ByVal expected() As INDArray)
            assertEquals(expected.Length, op.numInputArguments())
            For i As Integer = 0 To expected.Length - 1
                assertTrue(op.getInputArgument(i) Is expected(i), "input " & i)
            Next
        End Sub

        <TestMethod>
        Public Sub ConcatTakesTheDimensionAsItsOnlyIntegerArgument()
            Dim a As INDArray = Arr()
            Dim b As INDArray = Arr()
            Dim op As New Concat(1, a, b)
            AssertInputs(op, a, b)
            assertEquals(0, op.numOutputArguments())
            assertArrayEquals(New Long() {1}, op.iArgs())
            assertEquals(0, op.tArgs().Length)
            assertEquals(1, CInt(op.propertiesForFunction()("concatDimension")))
            Dim op2 As New Concat(New INDArray() {a, b}, -1)
            assertArrayEquals(New Long() {-1}, op2.iArgs())
        End Sub

        <TestMethod>
        Public Sub ReshapeMarksTheOrderWithMinus99AndAppendsTheShape()
            Dim x As INDArray = Arr()
            Dim op As New Reshape(x, 2L, 3L, 4L)
            AssertInputs(op, x)
            assertArrayEquals(New Long() {-99, 2, 3, 4}, op.iArgs())
            ' the shape as an array input: no integer arguments
            Dim shape As INDArray = Arr(DataType.LONG)
            Dim op2 As New Reshape(x, shape)
            AssertInputs(op2, x, shape)
            assertEquals(0, op2.iArgs().Length)
        End Sub

        <TestMethod>
        Public Sub PermuteAndTransposeTakeThePermutation()
            Dim x As INDArray = Arr()
            Dim z As INDArray = Arr()
            Dim op As New Permute(x, z, 2, 0, 1)
            AssertInputs(op, x)
            assertTrue(op.getOutputArgument(0) Is z)
            assertArrayEquals(New Long() {2, 0, 1}, op.iArgs())
            assertEquals("permute", op.opName())
            Dim t As New Transpose(x)
            AssertInputs(t, x)
            assertEquals(0, t.iArgs().Length)
        End Sub

        <TestMethod>
        Public Sub TileTakesTheRepeatsOrARepeatArray()
            Dim x As INDArray = Arr()
            Dim op As New Tile(x, 2, 3)
            AssertInputs(op, x)
            assertArrayEquals(New Long() {2, 3}, op.iArgs())
            Dim reps As INDArray = Arr(DataType.INT)
            Dim op2 As New Tile(x, reps)
            AssertInputs(op2, x, reps)
            assertEquals(0, op2.iArgs().Length)
        End Sub

        <TestMethod>
        Public Sub PadTakesTheModeOrdinalAndThePadValue()
            Dim x As INDArray = Arr()
            Dim padding As INDArray = Arr(DataType.INT)
            Dim z As INDArray = Arr()
            Dim op As New PadT(x, padding, z, PadT.Mode.REFLECT, 2.5)
            AssertInputs(op, x, padding)
            assertTrue(op.getOutputArgument(0) Is z)
            assertArrayEquals(New Long() {1}, op.iArgs())
            assertArrayEquals(New Double() {2.5}, op.tArgs(), 0.0)
            ' the PadMode overloads map to the legacy Mode
            assertArrayEquals(New Long() {2}, New PadT(x, padding, PadMode.SYMMETRIC, 0.0).iArgs())
            assertArrayEquals(New Long() {0}, New PadT(x, padding, 1.5).iArgs())
            ' the padding must be an integer array (Preconditions.checkState)
            assertThrows(Of InvalidOperationException)(Sub()
                                                           Dim bad As New PadT(x, Arr(DataType.FLOAT), 0.0)
                                                       End Sub)
        End Sub

        <TestMethod>
        Public Sub OneHotTakesAxisDepthOnOffAndTheOutputType()
            Dim indices As INDArray = Arr(DataType.INT)
            Dim op As New OneHot(indices, 5, -1, 1.0, 0.0, DataType.INT)
            AssertInputs(op, indices)
            assertArrayEquals(New Long() {-1, 5}, op.iArgs())
            assertArrayEquals(New Double() {1.0, 0.0}, op.tArgs(), 0.0)
            assertArrayEquals(New DataType() {DataType.INT}, op.dArgs())
            assertEquals(New List(Of DataType) From {DataType.INT}, op.calculateOutputDataTypes(New List(Of DataType) From {DataType.INT}))
            ' no output type: the default float, no data type argument; DOUBLE is a real choice, not "no type"
            Dim op2 As New OneHot(indices, 3)
            assertEquals(0, op2.dArgs().Length)
            assertEquals(New List(Of DataType) From {OneHot.DEFAULT_DTYPE}, op2.calculateOutputDataTypes(New List(Of DataType) From {DataType.INT}))
            Dim op3 As New OneHot(indices, 3, 0, 1.0, 0.0, DataType.DOUBLE)
            assertArrayEquals(New DataType() {DataType.DOUBLE}, op3.dArgs())
        End Sub

        <TestMethod>
        Public Sub Conv2DTakesTheConfigurationAsIntegerArguments()
            Dim input As INDArray = Arr()
            Dim weights As INDArray = Arr()
            Dim bias As INDArray = Arr()
            Dim config As Conv2DConfig = Conv2DConfig.builder().kH(3).kW(2).sH(2).sW(1).pH(1).pW(0).dH(1).dW(2).
                isSameMode(True).dataFormat(Conv2DConfig.NHWC).weightsFormat(WeightsFormat.OIYX).build()
            Dim op As New Conv2D(input, weights, bias, config)
            AssertInputs(op, input, weights, bias)
            assertArrayEquals(New Long() {3, 2, 2, 1, 1, 0, 1, 2, 1, 1, CInt(WeightsFormat.OIYX)}, op.iArgs())
            assertTrue(op.getConfig() Is config)
            assertTrue(op.ConfigProperties)
            assertEquals("config", op.configFieldName())
            assertTrue(op.propertiesForFunction()("isSameMode").Equals(True))
            ' no bias: two inputs
            Dim op2 As New Conv2D(input, weights, Nothing, Conv2DConfig.builder().kH(1).kW(1).build())
            AssertInputs(op2, input, weights)
            assertArrayEquals(New Long() {1, 1, 1, 1, 0, 0, 1, 1, 0, 0, CInt(WeightsFormat.YXIO)}, op2.iArgs())
        End Sub

        <TestMethod>
        Public Sub PoolingAndConv1DArguments()
            Dim input As INDArray = Arr()
            Dim pc As Pooling2DConfig = Pooling2DConfig.builder().kH(2).kW(2).sH(2).sW(2).build()
            Dim op As New MaxPooling2D(input, pc)
            AssertInputs(op, input)
            assertEquals(Pooling2D.Pooling2DType.MAX, pc.getType())
            ' kH kW sH sW pH pW dH dW sameMode extra isNHWC
            assertArrayEquals(New Long() {2, 2, 2, 2, 0, 0, 1, 1, 0, 0, 0}, op.iArgs())
            assertEquals("maxpool2d", op.opName())
            Dim c1 As Conv1DConfig = Conv1DConfig.builder().k(3).s(1).p(0).d(1).paddingMode(PaddingMode.CAUSAL).dataFormat(Conv1DConfig.NWC).build()
            Dim conv1 As New Conv1D(input, Arr(), Nothing, c1)
            assertArrayEquals(New Long() {3, 1, 0, 1, CInt(PaddingMode.CAUSAL), 1}, conv1.iArgs())
        End Sub

        <TestMethod>
        Public Sub LstmLayerTakesTheConfigurationAndTheWeights()
            Dim x As INDArray = Arr()
            Dim w As INDArray = Arr()
            Dim rw As INDArray = Arr()
            Dim b As INDArray = Arr()
            Dim weights As LSTMLayerWeights = LSTMLayerWeights.builder().iWeights(w).irWeights(rw).iBias(b).build()
            Dim config As LSTMLayerConfig = LSTMLayerConfig.builder().lstmdataformat(LSTMDataFormat.NST).
                directionMode(LSTMDirectionMode.BIDIR_CONCAT).gateAct(LSTMActivations.HARD_SIGMOID).
                retFullSequence(True).retLastH(True).cellClip(0.5).build()
            Dim op As New LSTMLayer(x, Nothing, Nothing, Nothing, weights, config)
            ' format, direction, gate, out, cell activations
            assertArrayEquals(New Long() {CInt(LSTMDataFormat.NST), CInt(LSTMDirectionMode.BIDIR_CONCAT), CInt(LSTMActivations.HARD_SIGMOID),
                                          CInt(LSTMActivations.TANH), CInt(LSTMActivations.TANH)}, op.iArgs())
            assertArrayEquals(New Double() {0.5}, op.tArgs(), 0.0)
            ' hasBias, seqLen, yLast, cLast, hasPH, retFullSequence, retLastH, retLastC
            assertArrayEquals(New Boolean() {True, False, False, False, False, True, True, False}, op.bArgs())
            assertEquals(2, op.NumOutputs)
            ' as in Java the arrays (Wx, Wr, b, x: LSTMLayerWeights.argsWithInputs) are passed as the op's outputs
            assertEquals(0, op.numInputArguments())
            assertEquals(4, op.numOutputArguments())
            assertTrue(op.getOutputArgument(0) Is w)
            assertTrue(op.getOutputArgument(3) Is x)
        End Sub

        <TestMethod>
        Public Sub LossesTakeTheReductionAndTheirScalars()
            Dim labels As INDArray = Arr()
            Dim predictions As INDArray = Arr()
            Dim weights As INDArray = Arr()
            Dim mse As New MeanSquaredErrorLoss(labels, predictions, weights, LossReduce.MEAN_BY_WEIGHT)
            ' predictions, weights, labels
            AssertInputs(mse, predictions, weights, labels)
            assertArrayEquals(New Long() {CInt(LossReduce.MEAN_BY_WEIGHT)}, mse.iArgs())
            assertEquals(0, mse.tArgs().Length)
            Dim sce As New SoftmaxCrossEntropyLoss(labels, predictions, weights, LossReduce.SUM, 0.1)
            assertArrayEquals(New Long() {CInt(LossReduce.SUM)}, sce.iArgs())
            assertArrayEquals(New Double() {0.1}, sce.tArgs(), 0.0)
            Dim huber As New HuberLoss(labels, predictions, weights, LossReduce.NONE, 2.0)
            assertArrayEquals(New Long() {0}, huber.iArgs())
            assertArrayEquals(New Double() {2.0}, huber.tArgs(), 0.0)
            Dim sig As New SigmoidCrossEntropyLoss(labels, predictions, weights, LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT, 0.2)
            assertArrayEquals(New Long() {3}, sig.iArgs())
            assertArrayEquals(New Double() {0.2}, sig.tArgs(), 0.0)
            assertEquals(New List(Of DataType) From {DataType.HALF}, mse.calculateOutputDataTypes(New List(Of DataType) From {DataType.HALF, DataType.FLOAT, DataType.FLOAT}))
        End Sub

        <TestMethod>
        Public Sub ScatterOpsTakeReferenceIndicesAndUpdates()
            Dim ref As INDArray = Arr()
            Dim indices As INDArray = Arr(DataType.INT)
            Dim updates As INDArray = Arr()
            For Each op As DynamicCustomOp In New DynamicCustomOp() {New ScatterAdd(ref, indices, updates), New ScatterSub(ref, indices, updates),
                                                                    New ScatterMul(ref, indices, updates), New ScatterDiv(ref, indices, updates),
                                                                    New ScatterMax(ref, indices, updates), New ScatterMin(ref, indices, updates),
                                                                    New ScatterUpdate(ref, indices, updates)}
                AssertInputs(op, ref, indices, updates)
                assertEquals(0, op.iArgs().Length, op.opName())
                assertEquals(New List(Of DataType) From {DataType.FLOAT}, op.calculateOutputDataTypes(New List(Of DataType) From {DataType.FLOAT, DataType.INT, DataType.FLOAT}))
            Next
            assertThrows(Of InvalidOperationException)(Sub() Call New ScatterAdd().calculateOutputDataTypes(New List(Of DataType) From {DataType.FLOAT, DataType.INT, DataType.DOUBLE}))
        End Sub

        <TestMethod>
        Public Sub ReductionsTakeKeepDimsAsBooleanAndTheDimensions()
            Dim x As INDArray = Arr()
            Dim op As New ArgMax(x, True, New Integer() {1, 2})
            AssertInputs(op, x)
            assertArrayEquals(New Boolean() {True}, op.bArgs())
            assertArrayEquals(New Long() {1, 2}, op.iArgs())
            assertArrayEquals(New Integer() {1, 2}, op.getDimensions())
            assertTrue(op.isKeepDims())
            Dim all As New ArgMax(x)
            assertArrayEquals(New Boolean() {False}, all.bArgs())
            assertEquals(0, all.iArgs().Length)
        End Sub

        <TestMethod>
        Public Sub CastBiasAddAndGatherArguments()
            Dim x As INDArray = Arr()
            ' Cast: the target type's libnd4j number (FlatBuffers DType)
            Dim cast As New Cast(x, DataType.INT)
            assertArrayEquals(New Long() {9}, cast.iArgs())
            assertEquals(New List(Of DataType) From {DataType.INT}, cast.calculateOutputDataTypes(New List(Of DataType) From {DataType.FLOAT}))
            Dim bias As INDArray = Arr()
            Dim ba As New BiasAdd(x, bias, False)
            AssertInputs(ba, x, bias)
            assertArrayEquals(New Boolean() {False}, ba.bArgs())
            Dim g As New Gather(x, New Integer() {4, 5}, 1)
            AssertInputs(g, x)
            assertArrayEquals(New Long() {1, 4, 5}, g.iArgs())
        End Sub

    End Class

End Namespace
