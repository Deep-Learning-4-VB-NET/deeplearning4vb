' DeepLearning4VB: broadcasting kernels (ND4J broadcast along dimensions and NumPy-style broadcasting).

Imports System
Imports System.Collections.Generic
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Broadcasting element-wise kernels. Two flavours:
    ''' <list type="bullet">
    ''' <item><description><see cref="BroadcastAlongDimensions"/>: ND4J's <c>BroadcastOp</c> (libnd4j
    ''' <c>Broadcast::exec</c>): y has the shape of one tensor along the given dimensions of x, and is
    ''' applied to every such tensor; z has the shape of x.</description></item>
    ''' <item><description><see cref="BroadcastPairwise"/> and <see cref="BroadcastCopy"/>: NumPy rules
    ''' (shapes aligned at the last dimension; a dimension of size 1 or a missing leading dimension
    ''' stretches); z has the broadcast shape.</description></item>
    ''' </list>
    ''' Values, types, threading and the extra-argument contract are as in <see cref="ElementWiseKernels"/>.
    ''' </summary>
    Public NotInheritable Class BroadcastKernels

        Private Sub New()
        End Sub

#Region "Broadcast along dimensions"

        ''' <summary>
        ''' z = f(x, y) where y is matched with every tensor of x along <paramref name="dimensions"/>:
        ''' element j (row-major over the TAD shape) of each TAD of x pairs with element j (row-major logical
        ''' index) of y. y needs as many elements as one TAD (its shape may be the TAD shape, with or without
        ''' unit dimensions, or any shape of that length); z must have the shape of x (unit dimensions may
        ''' differ). An empty dimension list means all dimensions (y then has the length of x).
        ''' </summary>
        Public Shared Sub BroadcastAlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                   ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                                   ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                   ByVal dimensions As Integer(), ByVal f As PairwiseFunction, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(y, yOffset, yShapeInfo, "y")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            If f Is Nothing Then
                Throw New ArgumentNullException(NameOf(f))
            End If
            Dim xShape As Long() = ShapeInfoHelper.GetShape(xShapeInfo)
            If Not ShapeInfoHelper.SqueezedShapesEqual(xShape, ShapeInfoHelper.GetShape(zShapeInfo)) Then
                Throw New ArgumentException("z must have the shape of x for a broadcast along dimensions", NameOf(zShapeInfo))
            End If
            Dim rank As Integer = xShape.Length
            Dim dims As Integer() = TadPack.NormalizeDimensions(rank, dimensions)
            If dims.Length = 0 Then
                dims = New Integer(rank - 1) {}
                For d As Integer = 0 To rank - 1
                    dims(d) = d
                Next
            End If
            Dim tadShape As Long() = New Long(dims.Length - 1) {}
            For i As Integer = 0 To dims.Length - 1
                tadShape(i) = xShape(dims(i))
            Next
            Dim tadLength As Long = ShapeInfoHelper.ShapeLength(tadShape)
            If ShapeInfoHelper.Length(yShapeInfo) <> tadLength Then
                Throw New ArgumentException("y has " & ShapeInfoHelper.Length(yShapeInfo) & " elements but a tensor along dimensions [" & String.Join(", ", dims) & "] of x has " & tadLength, NameOf(yShapeInfo))
            End If
            If ShapeInfoHelper.Length(xShapeInfo) = 0 Then
                Return
            End If
            ' y's strides over the TAD shape (densified when its shape cannot be mapped onto it)
            Dim ys As ManagedStorage = y
            Dim yo As Long = yOffset
            Dim yTadStrides As Long() = StridesInShape(yShapeInfo, tadShape)
            If yTadStrides Is Nothing Then
                Dim yi As Long() = Nothing
                KernelUtil.Densify(y, yOffset, yShapeInfo, ys, yo, yi)
                yTadStrides = ShapeInfoHelper.CalculateStrides(tadShape, "c"c)
            End If
            Dim yFull As Long() = New Long(rank - 1) {}
            For i As Integer = 0 To dims.Length - 1
                yFull(dims(i)) = yTadStrides(i)
            Next
            Dim xStrides As Long() = ShapeInfoHelper.GetStrides(xShapeInfo)
            Dim zStrides As Long() = StridesInShape(zShapeInfo, xShape)
            Dim plan As StridedPlan = StridedPlan.Create(xShape, New Long()() {xStrides, yFull, zStrides}, New Long() {xOffset, yo, zOffset}, True, 2)
            ElementWiseKernels.PairwisePlan(x, ys, z, plan, f, KernelUtil.Args(extraArgs))
        End Sub

#End Region

#Region "NumPy-style broadcasting"

        ''' <summary>True when the two shapes broadcast together under NumPy rules.</summary>
        Public Shared Function CanBroadcast(ByVal a As Long(), ByVal b As Long()) As Boolean
            If a Is Nothing OrElse b Is Nothing Then
                Return False
            End If
            Dim r As Integer = Math.Max(a.Length, b.Length)
            For i As Integer = 1 To r
                Dim da As Long = If(i <= a.Length, a(a.Length - i), 1L)
                Dim db As Long = If(i <= b.Length, b(b.Length - i), 1L)
                If da <> db AndAlso da <> 1 AndAlso db <> 1 Then
                    Return False
                End If
            Next
            Return True
        End Function

        ''' <summary>
        ''' The NumPy broadcast shape of two shapes: aligned at the last dimension, each dimension is the
        ''' larger of the two sizes where one of them is 1 (so a 0 against a 1 gives 0).
        ''' </summary>
        ''' <exception cref="ArgumentException">The shapes are not compatible.</exception>
        Public Shared Function BroadcastShape(ByVal a As Long(), ByVal b As Long()) As Long()
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If
            If b Is Nothing Then
                Throw New ArgumentNullException(NameOf(b))
            End If
            Dim r As Integer = Math.Max(a.Length, b.Length)
            Dim result As Long() = New Long(r - 1) {}
            For i As Integer = 1 To r
                Dim da As Long = If(i <= a.Length, a(a.Length - i), 1L)
                Dim db As Long = If(i <= b.Length, b(b.Length - i), 1L)
                If da = db Then
                    result(r - i) = da
                ElseIf da = 1 Then
                    result(r - i) = db
                ElseIf db = 1 Then
                    result(r - i) = da
                Else
                    Throw New ArgumentException("Shapes [" & String.Join(", ", a) & "] and [" & String.Join(", ", b) & "] cannot be broadcast together")
                End If
            Next
            Return result
        End Function

        ''' <summary>
        ''' z = f(x, y) with NumPy broadcasting: x and y must each broadcast to z's shape (normally z has
        ''' <see cref="BroadcastShape"/> of the two; z may also be x itself when y broadcasts to x).
        ''' </summary>
        Public Shared Sub BroadcastPairwise(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                            ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                            ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                            ByVal f As PairwiseFunction, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(y, yOffset, yShapeInfo, "y")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            If f Is Nothing Then
                Throw New ArgumentNullException(NameOf(f))
            End If
            Dim zShape As Long() = ShapeInfoHelper.GetShape(zShapeInfo)
            Dim xs As Long() = BroadcastStrides(xShapeInfo, zShape, "x")
            Dim ys As Long() = BroadcastStrides(yShapeInfo, zShape, "y")
            If ShapeInfoHelper.Length(zShapeInfo) = 0 Then
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.Create(zShape, New Long()() {xs, ys, ShapeInfoHelper.GetStrides(zShapeInfo)}, New Long() {xOffset, yOffset, zOffset}, True, 2)
            ElementWiseKernels.PairwisePlan(x, y, z, plan, f, KernelUtil.Args(extraArgs))
        End Sub

        ''' <summary>z = x stretched to z's shape with NumPy rules (converting to z's type like <see cref="ElementWiseKernels.Copy"/>).</summary>
        Public Shared Sub BroadcastCopy(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                        ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long())
            KernelUtil.CheckOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckOperand(z, zOffset, zShapeInfo, "z")
            Dim zShape As Long() = ShapeInfoHelper.GetShape(zShapeInfo)
            Dim xs As Long() = BroadcastStrides(xShapeInfo, zShape, "x")
            If ShapeInfoHelper.Length(zShapeInfo) = 0 Then
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.Create(zShape, New Long()() {xs, ShapeInfoHelper.GetStrides(zShapeInfo)}, New Long() {xOffset, zOffset}, True, 1)
            ElementWiseKernels.CopyPlan(x, z, plan)
        End Sub

        ''' <summary>
        ''' The strides that read the operand as if it had <paramref name="targetShape"/> under NumPy rules
        ''' (0 along stretched or missing dimensions).
        ''' </summary>
        ''' <exception cref="ArgumentException">The operand does not broadcast to the target shape.</exception>
        Public Shared Function BroadcastStrides(ByVal operandShapeInfo As Long(), ByVal targetShape As Long(), Optional ByVal name As String = "operand") As Long()
            Dim r As Integer = targetShape.Length
            Dim oShape As Long() = ShapeInfoHelper.GetShape(operandShapeInfo)
            Dim oStrides As Long() = ShapeInfoHelper.GetStrides(operandShapeInfo)
            If oShape.Length > r Then
                Throw New ArgumentException(name & " of rank " & oShape.Length & " cannot broadcast to rank " & r)
            End If
            Dim result As Long() = New Long(r - 1) {}
            Dim lead As Integer = r - oShape.Length
            For i As Integer = 0 To r - 1
                Dim oi As Integer = i - lead
                If oi < 0 Then
                    result(i) = 0
                ElseIf oShape(oi) = targetShape(i) Then
                    result(i) = If(oShape(oi) = 1, 0L, oStrides(oi))
                ElseIf oShape(oi) = 1 Then
                    result(i) = 0
                Else
                    Throw New ArgumentException(name & " of shape [" & String.Join(", ", oShape) & "] cannot broadcast to [" & String.Join(", ", targetShape) & "]")
                End If
            Next
            Return result
        End Function

#End Region

        ''' <summary>
        ''' Strides that read an operand of the same length as <paramref name="targetShape"/> in the target's
        ''' row-major logical order: its own strides when the shapes agree up to unit dimensions, scaled
        ''' dense strides when it is linearly strided, otherwise Nothing.
        ''' </summary>
        Friend Shared Function StridesInShape(ByVal info As Long(), ByVal targetShape As Long()) As Long()
            Dim shp As Long() = ShapeInfoHelper.GetShape(info)
            Dim str As Long() = ShapeInfoHelper.GetStrides(info)
            Dim result As Long() = New Long(targetShape.Length - 1) {}
            If ShapeInfoHelper.SqueezedShapesEqual(shp, targetShape) Then
                Dim own As New List(Of Long)()
                For d As Integer = 0 To shp.Length - 1
                    If shp(d) <> 1 Then
                        own.Add(str(d))
                    End If
                Next
                Dim k As Integer = 0
                For d As Integer = 0 To targetShape.Length - 1
                    If targetShape(d) <> 1 Then
                        result(d) = own(k)
                        k += 1
                    End If
                Next
                Return result
            End If
            If ShapeInfoHelper.ShapeLength(shp) <> ShapeInfoHelper.ShapeLength(targetShape) Then
                Return Nothing
            End If
            Dim ls As Long = ShapeInfoHelper.LinearStride(info)
            If ls = 0 Then
                Return Nothing
            End If
            Dim dense As Long() = ShapeInfoHelper.CalculateStrides(targetShape, "c"c)
            For d As Integer = 0 To targetShape.Length - 1
                result(d) = dense(d) * ls
            Next
            Return result
        End Function

    End Class

End Namespace
