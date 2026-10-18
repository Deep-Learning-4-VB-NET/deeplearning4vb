Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.broadcast
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.indexaccum
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.same
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.scalar
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.scalar.comparison
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.summarystats
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.strict
Imports SameSign = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.same.Sign
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports OpType = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op.Type

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' <see cref="BaseOp"/>: equality, hash codes and <c>ToString</c>, the own name, <c>getOpType(Op)</c>,
    ''' <c>FinalResult</c> and the TensorFlow/ONNX names.
    ''' </summary>
    <TestClass>
    Public Class BaseOpTests

        <TestMethod>
        Public Sub EqualOpsOfTheSameClassHaveEqualHashCodes()
            Dim a As New Sigmoid()
            Dim b As New Sigmoid()
            assertTrue(a.Equals(b))
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertTrue(a.Equals(a))
            assertFalse(a.Equals(Nothing))
            assertFalse(a.Equals(New Tanh()))
            assertFalse(a.Equals("sigmoid"))
        End Sub

        <TestMethod>
        Public Sub EqualityComparesTheArraysByIdentity()
            Dim x1 As INDArray = FakeArray.Simple()
            Dim x2 As INDArray = FakeArray.Simple()
            Dim a As New Sigmoid(x1)
            Dim b As New Sigmoid(x1)
            assertTrue(a.Equals(b))
            assertEquals(a.GetHashCode(), b.GetHashCode())
            b.setX(x2)
            assertFalse(a.Equals(b))
            b.setX(x1)
            b.setZ(x2)
            assertFalse(a.Equals(b))
            b.setZ(x1)
            b.setY(x2)
            assertFalse(a.Equals(b))
        End Sub

        <TestMethod>
        Public Sub EqualityComparesTheExtraArgumentsByValue()
            Dim a As New Sigmoid()
            Dim b As New Sigmoid()
            a.setExtraArgs(New Object() {1.0, 2.0})
            assertFalse(a.Equals(b))
            b.setExtraArgs(New Object() {1.0, 2.0})
            assertTrue(a.Equals(b))
            assertEquals(a.GetHashCode(), b.GetHashCode())
            b.setExtraArgs(New Object() {1.0, 3.0})
            assertFalse(a.Equals(b))
        End Sub

        <TestMethod>
        Public Sub EqualityIgnoresTheOwnName()
            ' Java's equals ignores the random own name; the hash code does too here, so equal ops hash alike
            Dim a As New Sigmoid()
            Dim b As New Sigmoid()
            assertNotEquals(a.getOwnName(), b.getOwnName())
            assertTrue(a.Equals(b))
            assertEquals(a.GetHashCode(), b.GetHashCode())
        End Sub

        <TestMethod>
        Public Sub ToStringIsTheOpName()
            assertEquals("sigmoid", New Sigmoid().ToString())
            assertEquals("reduce_sum", New Sum().ToString())
            assertEquals("broadcastadd", New BroadcastAddOp().ToString())
            assertEquals("greaterthan_scalar", New ScalarGreaterThan().ToString())
            assertEquals("var", New Variance().ToString())
        End Sub

        <TestMethod>
        Public Sub ParameterlessOpsGetARandomOwnNameArrayOpsNone()
            Dim op As New Sigmoid()
            assertNotNull(op.getOwnName())
            assertEquals(36, op.getOwnName().Length)
            assertTrue(Guid.TryParse(op.getOwnName(), Nothing))
            assertTrue(op.isOwnNameSetWithDefault())
            ' the array constructors pass DifferentialFunction(sameDiff:=False): no instance id, as in Java
            Dim withArray As New Sigmoid(FakeArray.Simple())
            assertNull(withArray.getOwnName())
            assertFalse(withArray.isOwnNameSetWithDefault())
        End Sub

        <TestMethod>
        Public Sub GetOpTypeOfAnOpFollowsJava()
            Dim y As INDArray = FakeArray.Simple()
            Dim sigmoid As New Sigmoid()
            assertEquals(OpType.TRANSFORM_FLOAT, BaseOp.getOpType(sigmoid).Value)
            sigmoid.setY(y)
            assertEquals(OpType.PAIRWISE, BaseOp.getOpType(sigmoid).Value)

            Dim sum As New Sum()
            assertEquals(OpType.REDUCE_SAME, BaseOp.getOpType(sum).Value)
            sum.setY(y)
            assertEquals(OpType.REDUCE3, BaseOp.getOpType(sum).Value)

            assertEquals(OpType.VARIANCE, BaseOp.getOpType(New Variance()).Value)
            assertEquals(OpType.SCALAR, BaseOp.getOpType(CreateOp(Of ScalarAdd)()).Value)
            assertEquals(OpType.SCALAR, BaseOp.getOpType(New ScalarGreaterThan()).Value)
            assertEquals(OpType.BROADCAST, BaseOp.getOpType(New BroadcastAddOp()).Value)
            assertEquals(OpType.INDEXREDUCE, BaseOp.getOpType(New FirstIndex()).Value)
        End Sub

        <TestMethod>
        Public Sub FinalResultReadsTheScalarZ()
            Dim op As New Sum()
            assertThrows(Of ND4VBIllegalStateException)(Function() op.FinalResult)

            op.setZ(ScalarZ(DataType.DOUBLE, 2.5, 0))
            assertEquals(2.5, op.FinalResult)
            op.setZ(ScalarZ(DataType.LONG, 0.0, 7))
            assertEquals(7L, op.FinalResult)
            op.setZ(ScalarZ(DataType.BOOL, 0.0, 1))
            assertEquals(1, op.FinalResult)

            Dim notScalar As INDArray = FakeArray.Create(New Dictionary(Of String, Func(Of Object(), Object)) From {
                {"get_Empty", Function(a) False}, {"get_Scalar", Function(a) False}})
            op.setZ(notScalar)
            assertThrows(Of ND4VBIllegalStateException)(Function() op.FinalResult)
            Dim empty As INDArray = FakeArray.Create(New Dictionary(Of String, Func(Of Object(), Object)) From {{"get_Empty", Function(a) True}})
            op.setZ(empty)
            assertThrows(Of ND4VBIllegalStateException)(Function() op.FinalResult)
        End Sub

        Private Shared Function ScalarZ(ByVal dtype As DataType, ByVal d As Double, ByVal i As Integer) As INDArray
            Return FakeArray.Create(New Dictionary(Of String, Func(Of Object(), Object)) From {
                {"get_Empty", Function(a) False},
                {"get_Scalar", Function(a) True},
                {"get_R", Function(a) dtype = DataType.DOUBLE OrElse dtype = DataType.FLOAT},
                {"get_Z", Function(a) dtype = DataType.LONG OrElse dtype = DataType.INT},
                {"get_B", Function(a) dtype = DataType.BOOL},
                {"getDouble", Function(a) d},
                {"getInt", Function(a) i}
            })
        End Function

        <TestMethod>
        Public Sub LegacyOpsHaveOneOutputAndNoImportNamesByDefault()
            Dim op As New ScalarGreaterThan()
            assertEquals(1, op.NumOutputs)
            assertThrows(Of NoOpNameFoundException)(Function() op.onnxName())
            assertThrows(Of NoOpNameFoundException)(Function() op.tensorflowName())
            Dim sigmoid As New Sigmoid()
            assertEquals("Sigmoid", sigmoid.onnxName())
            assertEquals("Sigmoid", sigmoid.tensorflowName())
            assertArrayEquals(New String() {"Sigmoid"}, sigmoid.tensorflowNames())
            assertEquals("Sign", New SameSign().tensorflowName())
        End Sub

        <TestMethod>
        Public Sub GetInputArgumentReturnsXOrY()
            Dim x As INDArray = FakeArray.Simple()
            Dim y As INDArray = FakeArray.Simple()
            Dim op As New Sigmoid(x)
            op.setY(y)
            assertTrue(op.getInputArgument(0) Is x)
            assertTrue(op.getInputArgument(1) Is y)
            assertThrows(Of InvalidOperationException)(Function() op.getInputArgument(2))
        End Sub

    End Class

End Namespace
