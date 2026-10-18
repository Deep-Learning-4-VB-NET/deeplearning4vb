Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports OpType = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op.Type

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' Table-driven reflection tests over the 173 legacy op classes (docs/phase2/op-inventory.md): each class has a
    ''' public parameterless constructor, keeps its Java <c>opName()</c> and <c>opNum()</c>, and its op name is in the
    ''' managed op math registry under the <see cref="ManagedOpKind"/> that matches its op type
    ''' (<see cref="KindsFor"/>, the mapping the executioner uses).
    ''' </summary>
    <TestClass>
    Public Class LegacyOpTableTests

        Private Const ImplNamespace As String = "DeepLearning4VB.org.nd4vb.linalg.api.ops.impl."

        ''' <summary>Class (relative to <c>...ops.impl</c>), Java <c>opName()</c>, Java <c>opNum()</c> (Nothing: Java's <c>opNum()</c> throws).</summary>
        Private Shared ReadOnly Table As Object(,) = {
            {"broadcast.bool.BroadcastEqualTo", "broadcast_equalto", 0},
            {"broadcast.bool.BroadcastGreaterThan", "broadcast_greaterthan", 1},
            {"broadcast.bool.BroadcastGreaterThanOrEqual", "broadcast_greaterthanorequal", 4},
            {"broadcast.bool.BroadcastLessThan", "broadcast_lessthan", 2},
            {"broadcast.bool.BroadcastLessThanOrEqual", "broadcast_lessthanorequal", 11},
            {"broadcast.bool.BroadcastNotEqual", "broadcast_notequal", 6},
            {"broadcast.BroadcastAddOp", "broadcastadd", 0},
            {"broadcast.BroadcastAMax", "broadcast_amax", 16},
            {"broadcast.BroadcastAMin", "broadcast_amin", 15},
            {"broadcast.BroadcastCopyOp", "broadcastcopy", 6},
            {"broadcast.BroadcastDivOp", "broadcastdiv", 3},
            {"broadcast.BroadcastGradientArgs", "broadcastgradientargs", 0},
            {"broadcast.BroadcastMax", "broadcast_max", 14},
            {"broadcast.BroadcastMin", "broadcast_min", 13},
            {"broadcast.BroadcastMulOp", "broadcastmul", 2},
            {"broadcast.BroadcastRDivOp", "broadcastrdiv", 4},
            {"broadcast.BroadcastRSubOp", "broadcastrsub", 5},
            {"broadcast.BroadcastSubOp", "broadcastsub", 1},
            {"indexaccum.FirstIndex", "first_index", 4},
            {"indexaccum.LastIndex", "last_index", 5},
            {"reduce.bool.All", "all", 1},
            {"reduce.bool.Any", "any", 0},
            {"reduce.bool.IsInf", "hasinf", 5},
            {"reduce.bool.IsNaN", "hasnan", 4},
            {"reduce.floating.AMean", "amean", 1},
            {"reduce.floating.Entropy", "entropy", 8},
            {"reduce.floating.LogEntropy", "logentropy", 9},
            {"reduce.floating.Mean", "reduce_mean", 0},
            {"reduce.floating.Norm1", "reduce_norm1", 2},
            {"reduce.floating.Norm2", "reduce_norm2", 3},
            {"reduce.floating.NormMax", "reduce_normmax", 4},
            {"reduce.floating.ShannonEntropy", "shannonentropy", 10},
            {"reduce.floating.SquaredNorm", "reduce_sqnorm", 7},
            {"reduce.longer.CountNonZero", "countNonZero", 0},
            {"reduce.longer.CountZero", "countZero", 1},
            {"reduce.longer.MatchCondition", "match_condition", 2},
            {"reduce.same.AMax", "amax", 5},
            {"reduce.same.AMin", "amin", 6},
            {"reduce.same.ASum", "asum", 4},
            {"reduce.same.Max", "reduce_max", 1},
            {"reduce.same.Min", "reduce_min", 2},
            {"reduce.same.Prod", "reduce_prod", 3},
            {"reduce.same.Sum", "reduce_sum", 0},
            {"reduce3.CosineDistance", "cosinedistance", 5},
            {"reduce3.CosineSimilarity", "cosinesimilarity", 2},
            {"reduce3.Dot", "dot", 3},
            {"reduce3.EqualsWithEps", "equals_with_eps", 4},
            {"reduce3.EuclideanDistance", "euclidean", 1},
            {"reduce3.HammingDistance", "hammingdistance", 7},
            {"reduce3.JaccardDistance", "jaccarddistance", 6},
            {"reduce3.ManhattanDistance", "manhattan", 0},
            {"scalar.comparison.ScalarAnd", "and_scalar", 7},
            {"scalar.comparison.ScalarEps", "eps_scalar", 3},
            {"scalar.comparison.ScalarEquals", "equals_scalar", 0},
            {"scalar.comparison.ScalarGreaterThan", "greaterthan_scalar", 1},
            {"scalar.comparison.ScalarGreaterThanOrEqual", "greaterthanorequal_scalar", 4},
            {"scalar.comparison.ScalarLessThan", "lessthan_scalar", 2},
            {"scalar.comparison.ScalarLessThanOrEqual", "lessthanorequal_scalar", 11},
            {"scalar.comparison.ScalarNot", "not_scalar", 10},
            {"scalar.comparison.ScalarNotEquals", "notequals_scalar", 6},
            {"scalar.comparison.ScalarOr", "or_scalar", 8},
            {"scalar.comparison.ScalarSetValue", "setvalorless_scalar", 14},
            {"scalar.comparison.ScalarXor", "xor_scalar", 9},
            {"scalar.LeakyReLU", "leakyrelu", 35},
            {"scalar.LogX", "log_x", 38},
            {"scalar.Pow", "pow", 31},
            {"scalar.PowDerivative", "_powderivative", 32},
            {"scalar.RectifiedLinear", "relu", 39},
            {"scalar.Relu6", "relu6", 40},
            {"scalar.ReplaceNans", "replace_nans", 37},
            {"scalar.ScalarAdd", "add_scalar", 0},
            {"scalar.ScalarDivision", "div_scalar", 3},
            {"scalar.ScalarFMod", "fmod_scalar", 18},
            {"scalar.ScalarMax", "max_scalar", 6},
            {"scalar.ScalarMin", "scalar_min", 13},
            {"scalar.ScalarMultiplication", "mul_scalar", 2},
            {"scalar.ScalarRemainder", "remainder_scalar", 17},
            {"scalar.ScalarReverseDivision", "rdiv_scalar", 4},
            {"scalar.ScalarReverseSubtraction", "rsub_scalar", 5},
            {"scalar.ScalarSet", "set_scalar", 14},
            {"scalar.ScalarSubtraction", "sub_scalar", 1},
            {"scalar.Step", "step", 41},
            {"summarystats.StandardDeviation", "std", 1},
            {"summarystats.Variance", "var", 0},
            {"transforms.any.Assign", "old_assign", 0},
            {"transforms.bool.BooleanNot", "bool_not", 7},
            {"transforms.bool.IsFinite", "isfinite", 3},
            {"transforms.bool.IsInf", "isinf", 1},
            {"transforms.bool.IsNaN", "isnan", 2},
            {"transforms.bool.MatchConditionTransform", "match_condition_transform", 5},
            {"transforms.comparison.CompareAndReplace", "car", 13},
            {"transforms.comparison.CompareAndSet", "cas", 13},
            {"transforms.comparison.Eps", "eps", 3},
            {"transforms.floating.RSqrt", "rsqrt", 3},
            {"transforms.floating.Sqrt", "sqrt", 1},
            {"transforms.gradient.CubeDerivative", "cubederivative", 21},
            {"transforms.gradient.HardSigmoidDerivative", "hard_sigmoidderivative", 10},
            {"transforms.gradient.HardTanhDerivative", "hardtanhderivative", 5},
            {"transforms.gradient.LeakyReLUDerivative", "leakyreluderivative", 36},
            {"transforms.gradient.RationalTanhDerivative", "rational_tanh_derivative", 11},
            {"transforms.gradient.RectifiedTanhDerivative", "rectified_tanh_derivative", 12},
            {"transforms.gradient.SELUDerivative", "seluderivative", 9},
            {"transforms.gradient.SoftSignDerivative", "softsignderivative", 7},
            {"transforms.MaxOut", "maxout", Nothing},
            {"transforms.pairwise.arithmetic.Axpy", "axpy", 10},
            {"transforms.pairwise.arithmetic.CopyOp", "copy", 1},
            {"transforms.pairwise.arithmetic.FModOp", "fmod", 15},
            {"transforms.pairwise.arithmetic.PowPairwise", "pow_pairwise", 4},
            {"transforms.pairwise.arithmetic.RemainderOp", "remainder", 14},
            {"transforms.pairwise.BinaryMinimalRelativeError", "BinaryMinimalRelativeError", 28},
            {"transforms.pairwise.BinaryRelativeError", "BinaryRelativeError", 27},
            {"transforms.pairwise.bool.And", "and", 7},
            {"transforms.pairwise.bool.Not", "not", 10},
            {"transforms.pairwise.bool.Or", "or", 8},
            {"transforms.pairwise.bool.Xor", "xor", 9},
            {"transforms.pairwise.RelativeError", "RelativeError", 26},
            {"transforms.pairwise.Set", "set", 1},
            {"transforms.same.Abs", "abs", 0},
            {"transforms.same.AMax", "amax_pairwise", 34},
            {"transforms.same.AMin", "amin_pairwise", 35},
            {"transforms.same.Ceil", "ceil", 17},
            {"transforms.same.Cube", "cube", 6},
            {"transforms.same.Floor", "floor", 18},
            {"transforms.same.Max", "max_pairwise", 7},
            {"transforms.same.Min", "min_pairwise", 8},
            {"transforms.same.Negative", "neg", 3},
            {"transforms.same.OneMinus", "oneminus", 7},
            {"transforms.same.Reciprocal", "Reciprocal", 11},
            {"transforms.same.Round", "round", 4},
            {"transforms.same.Sign", "sign", 1},
            {"transforms.same.Square", "square", 12},
            {"transforms.same.TimesOneMinus", "timesoneminus", 5},
            {"transforms.strict.ACos", "acos", 30},
            {"transforms.strict.ACosh", "acosh", 46},
            {"transforms.strict.ASin", "asin", 31},
            {"transforms.strict.ASinh", "asinh", 47},
            {"transforms.strict.ATan", "atan", 32},
            {"transforms.strict.ATanh", "atanh", 52},
            {"transforms.strict.Cos", "cos", 22},
            {"transforms.strict.Cosh", "cosh", 40},
            {"transforms.strict.Erf", "erf", 45},
            {"transforms.strict.Erfc", "erfc", 50},
            {"transforms.strict.Exp", "exp", 23},
            {"transforms.strict.Expm1", "expm1", 51},
            {"transforms.strict.GELU", "gelu", 53},
            {"transforms.strict.GELUDerivative", "_geluderivative", 54},
            {"transforms.strict.HardSigmoid", "hard_sigmoid", 36},
            {"transforms.strict.HardTanh", "hardtanh", 33},
            {"transforms.strict.Log", "log", 24},
            {"transforms.strict.Log1p", "log1p", 44},
            {"transforms.strict.LogSigmoid", "logsigmoid", 49},
            {"transforms.strict.Mish", "mish", 57},
            {"transforms.strict.MishDerivative", "_mishderivative", 58},
            {"transforms.strict.PreciseGELU", "precise_gelu", 55},
            {"transforms.strict.PreciseGELUDerivative", "_precise_geluderivative", 56},
            {"transforms.strict.RationalTanh", "rational_tanh", 37},
            {"transforms.strict.RectifiedTanh", "rectified_tanh", 38},
            {"transforms.strict.Rint", "rint", 48},
            {"transforms.strict.SELU", "selu", 42},
            {"transforms.strict.SetRange", "setrange", 25},
            {"transforms.strict.Sigmoid", "sigmoid", 26},
            {"transforms.strict.SigmoidDerivative", "_sigmoidderivative", 6},
            {"transforms.strict.Sin", "sin", 27},
            {"transforms.strict.Sinh", "sinh", 39},
            {"transforms.strict.SoftPlus", "softplus", 28},
            {"transforms.strict.SoftSign", "softsign", 34},
            {"transforms.strict.Stabilize", "stabilize", 19},
            {"transforms.strict.Swish", "swish", 43},
            {"transforms.strict.SwishDerivative", "_swishderivative", 13},
            {"transforms.strict.Tan", "tan", 41},
            {"transforms.strict.TanDerivative", "tanderivative", 8},
            {"transforms.strict.Tanh", "tanh", 29},
            {"transforms.strict.TanhDerivative", "_tanhderivative", 4}
        }

        ''' <summary>The two legacy classes without registry math (see the inventory): MaxOut is a placeholder whose Java
        ''' <c>opNum()</c> throws, BroadcastGradientArgs computes reduction axes (custom op, M3).</summary>
        Private Shared ReadOnly NotInRegistry As String() = {"transforms.MaxOut", "broadcast.BroadcastGradientArgs"}

        ''' <summary>
        ''' The transform ops that the executioner runs as pairwise ops only (they need a y array). And, Or and Xor are
        ''' transform bool classes too, but report <c>PAIRWISE_BOOL</c> as their op type, as in Java.
        ''' </summary>
        Private Shared ReadOnly RunsAsPairwise As String() = {"car", "BinaryMinimalRelativeError", "BinaryRelativeError",
            "RelativeError", "axpy", "fmod", "amax_pairwise", "amin_pairwise", "max_pairwise", "min_pairwise", "eps",
            "pow_pairwise", "remainder"}

        ''' <summary>
        ''' The registry kinds of an op type: the transform kinds also run as pairwise ops when the op has a y array
        ''' (pairwise bool for bool transforms); variance and summary stats are both summary stats.
        ''' </summary>
        Friend Shared Function KindsFor(ByVal type As OpType) As ManagedOpKind()
            Select Case type
                Case OpType.TRANSFORM_SAME : Return {ManagedOpKind.TransformSame, ManagedOpKind.Pairwise}
                Case OpType.TRANSFORM_FLOAT : Return {ManagedOpKind.TransformFloat, ManagedOpKind.Pairwise}
                Case OpType.TRANSFORM_STRICT : Return {ManagedOpKind.TransformStrict, ManagedOpKind.Pairwise}
                Case OpType.TRANSFORM_BOOL : Return {ManagedOpKind.TransformBool, ManagedOpKind.PairwiseBool}
                Case OpType.TRANSFORM_ANY : Return {ManagedOpKind.TransformAny, ManagedOpKind.Pairwise}
                Case OpType.PAIRWISE : Return {ManagedOpKind.Pairwise}
                Case OpType.PAIRWISE_BOOL : Return {ManagedOpKind.PairwiseBool}
                Case OpType.SCALAR : Return {ManagedOpKind.Scalar}
                Case OpType.SCALAR_BOOL : Return {ManagedOpKind.ScalarBool}
                Case OpType.BROADCAST : Return {ManagedOpKind.Broadcast}
                Case OpType.BROADCAST_BOOL : Return {ManagedOpKind.BroadcastBool}
                Case OpType.REDUCE_FLOAT : Return {ManagedOpKind.ReduceFloat}
                Case OpType.REDUCE_SAME : Return {ManagedOpKind.ReduceSame}
                Case OpType.REDUCE_LONG : Return {ManagedOpKind.ReduceLong}
                Case OpType.REDUCE_BOOL : Return {ManagedOpKind.ReduceBool}
                Case OpType.REDUCE3 : Return {ManagedOpKind.Reduce3}
                Case OpType.INDEXREDUCE : Return {ManagedOpKind.IndexReduce}
                Case OpType.VARIANCE, OpType.SUMMARYSTATS : Return {ManagedOpKind.SummaryStats}
                Case OpType.RANDOM : Return {ManagedOpKind.Random}
                Case OpType.CUSTOM : Return {ManagedOpKind.Custom}
                Case Else : Return Array.Empty(Of ManagedOpKind)()
            End Select
        End Function

        ''' <summary>The op type the executioner dispatches on: the family interface's <c>getOpType()</c>, <c>opType()</c> for index reductions.</summary>
        Friend Shared Function OpTypeOf(ByVal op As Op) As OpType
            If TypeOf op Is TransformOp Then Return DirectCast(op, TransformOp).getOpType()
            If TypeOf op Is ScalarOp Then Return DirectCast(op, ScalarOp).getOpType()
            If TypeOf op Is BroadcastOp Then Return DirectCast(op, BroadcastOp).getOpType()
            If TypeOf op Is ReduceOp Then Return DirectCast(op, ReduceOp).getOpType()
            Return DirectCast(op, BaseOp).opType()
        End Function

        Private Shared Function TypeOfRow(ByVal row As Integer) As Type
            Dim name As String = ImplNamespace & CStr(Table(row, 0))
            Dim t As Type = GetType(BaseOp).Assembly.GetType(name, False)
            assertNotNull(t)
            Return t
        End Function

        <TestMethod>
        Public Sub TheTableListsEveryConcreteLegacyOpClass()
            Dim found As List(Of String) = GetType(BaseOp).Assembly.GetTypes().
                Where(Function(t) t.IsClass AndAlso Not t.IsAbstract AndAlso GetType(BaseOp).IsAssignableFrom(t) AndAlso
                                   t.FullName.StartsWith(ImplNamespace, StringComparison.Ordinal)).
                Select(Function(t) t.FullName.Substring(ImplNamespace.Length)).OrderBy(Function(s) s, StringComparer.Ordinal).ToList()
            Dim listed As New List(Of String)()
            For row As Integer = 0 To Table.GetLength(0) - 1
                listed.Add(CStr(Table(row, 0)))
            Next
            listed.Sort(StringComparer.Ordinal)
            assertEquals(173, listed.Count)
            assertEquals(listed, found)
        End Sub

        <TestMethod>
        Public Sub EveryLegacyOpHasAPublicParameterlessConstructor()
            Dim standIns As Integer = 0
            For row As Integer = 0 To Table.GetLength(0) - 1
                Dim t As Type = TypeOfRow(row)
                assertTrue(t.GetConstructor(Type.EmptyTypes) IsNot Nothing, CStr(Table(row, 0)))
                Dim constructed As Boolean
                Dim op As Object = CreateOp(t, constructed)
                assertTrue(TypeOf op Is BaseOp, CStr(Table(row, 0)))
                If Not constructed Then
                    standIns += 1
                    ' only the scalar ops' constructors need Nd4vb (Nd4vb.scalar) before wave 3
                    assertTrue(TypeOf op Is BaseScalarOp, CStr(Table(row, 0)) & " needs a stand-in in its constructor")
                End If
            Next
            assertTrue(standIns <= 21, "stand-in constructions: " & standIns)
        End Sub

        <TestMethod>
        Public Sub EveryLegacyOpKeepsItsJavaNameAndNumber()
            For row As Integer = 0 To Table.GetLength(0) - 1
                Dim op As BaseOp = DirectCast(CreateOp(TypeOfRow(row), Nothing), BaseOp)
                Dim name As String = CStr(Table(row, 0))
                assertEquals(Table(row, 1), op.opName(), name)
                If Not TypeOf op Is BaseIndexAccumulation Then
                    ' the index reductions' Lombok @Data ToString lists their fields instead
                    assertEquals(Table(row, 1), op.ToString(), name & " ToString")
                End If
                If Table(row, 2) Is Nothing Then
                    assertThrows(Of NotSupportedException)(Sub() op.opNum())
                Else
                    assertEquals(CInt(Table(row, 2)), op.opNum(), name)
                End If
            Next
        End Sub

        <TestMethod>
        Public Sub EveryLegacyOpNameIsInTheRegistryUnderItsKind()
            Dim pairwiseOnly As New List(Of String)()
            For row As Integer = 0 To Table.GetLength(0) - 1
                Dim name As String = CStr(Table(row, 0))
                Dim op As Op = DirectCast(CreateOp(TypeOfRow(row), Nothing), Op)
                Dim kinds As ManagedOpKind() = KindsFor(OpTypeOf(op))
                assertTrue(kinds.Length > 0, name & ": no kind for " & OpTypeOf(op).ToString())
                Dim supported As Boolean = ManagedOpMath.Supports(kinds(0), op.opName())
                Dim viaPairwise As Boolean = kinds.Length > 1 AndAlso ManagedOpMath.Supports(kinds(1), op.opName())
                If NotInRegistry.Contains(name) Then
                    assertFalse(supported OrElse viaPairwise, name & " is in the registry now: update the inventory and this test")
                    Continue For
                End If
                assertTrue(supported OrElse viaPairwise, name & " (" & op.opName() & ") is not registered under " & kinds(0).ToString())
                If Not supported Then
                    pairwiseOnly.Add(op.opName())
                End If
            Next
            pairwiseOnly.Sort(StringComparer.Ordinal)
            Dim expected As List(Of String) = RunsAsPairwise.OrderBy(Function(s) s, StringComparer.Ordinal).ToList()
            assertEquals(expected, pairwiseOnly)
        End Sub

        <TestMethod>
        Public Sub OpTypesFollowTheBaseClasses()
            Dim counts As New Dictionary(Of OpType, Integer)()
            For row As Integer = 0 To Table.GetLength(0) - 1
                Dim op As Op = DirectCast(CreateOp(TypeOfRow(row), Nothing), Op)
                Dim type As OpType = OpTypeOf(op)
                Dim n As Integer = 0
                counts.TryGetValue(type, n)
                counts(type) = n + 1
            Next
            ' the inventory's family counts (transform: MaxOut reports TRANSFORM_STRICT as in Java; Variance reports
            ' VARIANCE, StandardDeviation SUMMARYSTATS)
            assertEquals(23, counts(OpType.TRANSFORM_SAME))
            assertEquals(2, counts(OpType.TRANSFORM_FLOAT))
            assertEquals(49, counts(OpType.TRANSFORM_STRICT))
            assertEquals(7, counts(OpType.TRANSFORM_BOOL))
            assertEquals(3, counts(OpType.PAIRWISE_BOOL))
            assertEquals(4, counts(OpType.TRANSFORM_ANY))
            assertEquals(21, counts(OpType.SCALAR))
            assertEquals(11, counts(OpType.SCALAR_BOOL))
            assertEquals(12, counts(OpType.BROADCAST))
            assertEquals(6, counts(OpType.BROADCAST_BOOL))
            assertEquals(9, counts(OpType.REDUCE_FLOAT))
            assertEquals(7, counts(OpType.REDUCE_SAME))
            assertEquals(3, counts(OpType.REDUCE_LONG))
            assertEquals(4, counts(OpType.REDUCE_BOOL))
            assertEquals(8, counts(OpType.REDUCE3))
            assertEquals(2, counts(OpType.INDEXREDUCE))
            assertEquals(1, counts(OpType.VARIANCE))
            assertEquals(1, counts(OpType.SUMMARYSTATS))
            assertEquals(173, counts.Values.Sum())
        End Sub

    End Class

End Namespace
