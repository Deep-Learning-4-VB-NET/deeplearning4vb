Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Condition = DeepLearning4VB.org.nd4vb.linalg.indexing.conditions.Condition
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.broadcast
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.broadcast.bool
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.indexaccum
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.bool
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.floating
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.longer
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.same
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce3
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.scalar
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.scalar.comparison
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.summarystats
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.comparison
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.strict
Imports BoolIsNaN = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.bool.IsNaN
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports OpType = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op.Type

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' How representative ops of each family store x, y, z, dimensions and extra arguments, and pick their result
    ''' data type. Arrays are <see cref="FakeArray"/> instances; calls that need a wave-3 class go through
    ''' <see cref="TolerateStandIn"/>.
    ''' </summary>
    <TestClass>
    Public Class LegacyOpStateTests

        ''' <summary>A condition with fixed values (the ops read <c>Value</c>, <c>condtionNum()</c> and <c>epsThreshold()</c>).</summary>
        Private NotInheritable Class FixedCondition
            Implements Condition

            Private ReadOnly v As Double
            Private ReadOnly num As Integer
            Private ReadOnly eps As Double

            Public Sub New(ByVal v As Double, ByVal num As Integer, ByVal eps As Double)
                Me.v = v
                Me.num = num
                Me.eps = eps
            End Sub

            Public Function apply(ByVal input As Object) As Boolean Implements Global.DeepLearning4VB.org.nd4vb.common.function.[Function](Of Object, Boolean).apply
                Return False
            End Function

            Public Sub setValue(ByVal value As Object) Implements Condition.setValue
            End Sub

            Public Function condtionNum() As Integer Implements Condition.condtionNum
                Return num
            End Function

            Public ReadOnly Property Value As Double Implements Condition.Value
                Get
                    Return v
                End Get
            End Property

            Public Function epsThreshold() As Double Implements Condition.epsThreshold
                Return eps
            End Function
        End Class

        <TestMethod>
        Public Sub TransformOpsStoreXAsXAndZ()
            Dim op As New Sigmoid()
            assertNull(op.x())
            assertNull(op.y())
            assertNull(op.z())
            assertNull(op.extraArgs())

            Dim x As INDArray = FakeArray.Simple()
            Dim inPlace As New Sigmoid(x)
            assertTrue(inPlace.x() Is x)
            assertTrue(inPlace.z() Is x)
            assertNull(inPlace.y())

            Dim y As INDArray = FakeArray.Simple()
            Dim z As INDArray = FakeArray.Simple()
            op.setX(x)
            op.setY(y)
            op.setZ(z)
            assertTrue(op.x() Is x)
            assertTrue(op.y() Is y)
            assertTrue(op.z() Is z)
            op.clearArrays()
            assertNull(op.x())
            assertNull(op.y())
            assertNull(op.z())
        End Sub

        <TestMethod>
        Public Sub TransformOpsKeepTheirExtraArguments()
            Dim x As INDArray = FakeArray.Simple()
            Dim setRange As New SetRange(x, 0.5, 2.0)
            assertArrayEquals(New Object() {0.5, 2.0}, setRange.extraArgs())
            assertTrue(setRange.x() Is x)
            assertTrue(setRange.z() Is x)

            Dim cas As New CompareAndSet(x, 1.5, 9.0, 0.001, New FixedCondition(1.5, 10, 0.001))
            assertArrayEquals(New Object() {1.5, 9.0, 0.001, 10.0}, cas.extraArgs())
            assertEquals(13, cas.opNum())
            cas.setY(FakeArray.Simple())
            assertEquals(12, cas.opNum())

            Dim op As New Sigmoid()
            op.setExtraArgs(New Object() {3.0})
            assertArrayEquals(New Object() {3.0}, op.extraArgs())
            assertArrayEquals(New Object() {3.0}, op.getExtraArgs())
        End Sub

        <TestMethod>
        Public Sub TransformResultTypesFollowTheKind()
            Dim x As INDArray = FakeArray.Simple(dtype:=DataType.DOUBLE)
            assertEquals(DataType.DOUBLE, New Sigmoid(x).resultType())
            assertEquals(DataType.BOOL, New BoolIsNaN(x).resultType())
            assertEquals(OpType.TRANSFORM_STRICT, New Sigmoid().getOpType())
            assertEquals(OpType.TRANSFORM_BOOL, New BoolIsNaN().getOpType())
        End Sub

        <TestMethod>
        Public Sub ScalarOpsTakeTheScalarFromYOrTheStoredValue()
            Dim op As ScalarAdd = CreateOp(Of ScalarAdd)()
            Dim scalarArray As INDArray = FakeArray.Simple(isScalar:=True)
            op.setScalar(scalarArray)
            assertTrue(op.scalar() Is scalarArray)
            assertTrue(op.getScalarValue() Is scalarArray)
            ' BaseScalarOp: a scalar y wins over the stored value
            Dim y As INDArray = FakeArray.Simple(isScalar:=True)
            op.setY(y)
            assertTrue(op.scalar() Is y)
            op.setY(FakeArray.Simple(isScalar:=False))
            assertTrue(op.scalar() Is scalarArray)
            assertEquals(OpType.SCALAR, op.getOpType())
            assertEquals(OpType.SCALAR, op.opType())

            ' BaseScalarBoolOp: y only when no value is stored
            Dim cmp As New ScalarGreaterThan()
            assertNull(cmp.scalar())
            cmp.setY(y)
            assertTrue(cmp.scalar() Is y)
            cmp.setScalar(scalarArray)
            assertTrue(cmp.scalar() Is scalarArray)
            assertEquals(OpType.SCALAR_BOOL, cmp.getOpType())
        End Sub

        <TestMethod>
        Public Sub ScalarDimensionsGoToTheDimensionArrayOnly()
            ' Java's setDimension calls defineDimensions, which fills the dimension array but not the int[] field
            Dim op As New ScalarGreaterThan()
            TolerateStandIn(Sub() op.Dimension = New Integer() {1})
            assertNull(op.Dimension)
        End Sub

        <TestMethod>
        Public Sub ScalarConstructorsStoreTheValue()
            ' runs completely once Nd4vb exists (wave 3); until then the constructor stops at Nd4vb.scalar
            Dim x As INDArray = FakeArray.Simple()
            TolerateStandIn(Sub()
                                Dim op As New ScalarAdd(x, 2.0)
                                assertTrue(op.x() Is x)
                                assertTrue(op.z() Is x)
                                assertNotNull(op.scalar())
                            End Sub)
        End Sub

        <TestMethod>
        Public Sub BroadcastOpsStoreTheirDimensions()
            Dim op As New BroadcastAddOp()
            assertNull(op.dimensions())
            op.Dimension = New Integer() {0, 2}
            assertArrayEquals(New Integer() {0, 2}, op.Dimension)
            Dim x As INDArray = FakeArray.Simple()
            Dim y As INDArray = FakeArray.Simple()
            op.setX(x)
            op.setY(y)
            assertTrue(op.x() Is x)
            assertTrue(op.y() Is y)
            assertEquals(OpType.BROADCAST, op.getOpType())

            Dim cmp As New BroadcastEqualTo()
            assertEquals(OpType.BROADCAST_BOOL, cmp.getOpType())
            assertEquals(OpType.BROADCAST, cmp.opType())
            ' without dimensions, the shapes of x and y give them (Shape.getBroadcastDimensions)
            cmp.setX(FakeArray.Create(New Dictionary(Of String, Func(Of Object(), Object)) From {{"shape", Function(a) New Long() {2, 3}}}))
            cmp.setY(FakeArray.Create(New Dictionary(Of String, Func(Of Object(), Object)) From {{"shape", Function(a) New Long() {1, 3}}}))
            assertArrayEquals(New Integer() {0}, cmp.Dimension)
        End Sub

        <TestMethod>
        Public Sub ReduceOpsStoreDimensionsAndFlags()
            Dim op As New Sum()
            assertNull(op.getDimensions())
            assertFalse(op.KeepDims)
            TolerateStandIn(Sub() op.setDimensions(1, 2))
            assertArrayEquals(New Integer() {1, 2}, op.getDimensions())
            op.setKeepDims(True)
            assertTrue(op.KeepDims)
            assertFalse(op.ComplexAccumulation)
            op.setComplex(True)
            assertTrue(op.ComplexAccumulation)
            assertTrue(op.isComplex())
            op.setEmptyReduce(True)
            assertTrue(op.isEmptyReduce())
            assertEquals(OpType.REDUCE_SAME, op.getOpType())
        End Sub

        <TestMethod>
        Public Sub ReduceResultTypesFollowTheKind()
            Dim x As INDArray = FakeArray.Simple(dtype:=DataType.DOUBLE)
            Dim sum As New Sum()
            sum.setX(x)
            assertEquals(DataType.DOUBLE, sum.resultType())
            Dim mean As New Mean()
            mean.setX(x)
            assertEquals(DataType.DOUBLE, mean.resultType())
            assertEquals(OpType.REDUCE_FLOAT, mean.getOpType())
            assertEquals(DataType.LONG, New CountNonZero().resultType())
            assertEquals(OpType.REDUCE_LONG, New CountNonZero().getOpType())
            assertEquals(DataType.BOOL, New All().resultType())
            assertEquals(OpType.REDUCE_BOOL, New All().getOpType())
            assertTrue(New All().emptyValue())
            assertFalse(New Any().emptyValue())
        End Sub

        <TestMethod>
        Public Sub Reduce3OpsUseAccumulatorSlots()
            Dim op As New CosineSimilarity()
            assertArrayEquals(New Object() {0.0F, 0.0F}, op.extraArgs())
            assertEquals(OpType.REDUCE3, op.getOpType())
            assertEquals(OpType.REDUCE3, New EuclideanDistance().getOpType())
        End Sub

        <TestMethod>
        Public Sub IndexReductionsStoreKeepDimsAndTheCondition()
            Dim op As New FirstIndex()
            assertFalse(op.KeepDims)
            op.setKeepDims(True)
            assertTrue(op.KeepDims)
            assertEquals(OpType.INDEXREDUCE, op.opType())
            op.setCompare(2.0)
            op.setEps(0.5)
            op.setMode(3)
            assertEquals(2.0, op.getCompare(), 0.0)
            assertEquals(0.5, op.getEps(), 0.0)
            assertEquals(3, op.getMode())
            ' Lombok @Data: equals/hashCode/toString over condition, compare, eps and mode
            Dim other As New FirstIndex()
            other.setCompare(2.0)
            other.setEps(0.5)
            other.setMode(3)
            assertTrue(op.Equals(other))
            assertEquals(op.GetHashCode(), other.GetHashCode())
            assertEquals("FirstIndex(condition=null, compare=2.0, eps=0.5, mode=3)", op.ToString())
            other.setMode(4)
            assertFalse(op.Equals(other))
            assertFalse(op.Equals(New LastIndex()))
            assertEquals("LastIndex(condition=null, compare=0.0, eps=0.0, mode=0)", New LastIndex().ToString())
        End Sub

        <TestMethod>
        Public Sub SummaryStatsAreBiasCorrectedByDefault()
            Dim op As New Variance()
            assertTrue(op.BiasCorrected)
            op.BiasCorrected = False
            assertFalse(op.BiasCorrected)
            assertEquals(OpType.VARIANCE, op.getOpType())
            Dim std As New StandardDeviation()
            assertEquals("std", std.opName())
            assertEquals(OpType.SUMMARYSTATS, std.getOpType())
        End Sub

    End Class

End Namespace
