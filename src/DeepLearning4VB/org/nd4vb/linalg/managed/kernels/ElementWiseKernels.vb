' DeepLearning4VB: element-wise kernels (transform, scalar, pairwise, copy) of the managed ND4VB engine.

Imports System
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Element-wise loops over (storage, element offset, shape info) operands: z = f(x), z = f(x, scalar),
    ''' z = f(x, y), and converting copies.
    ''' </summary>
    ''' <remarks>
    ''' <para>Operands must have the same number of elements; elements are matched by their row-major
    ''' logical index (see <see cref="ShapeInfoHelper"/>), so any strides, either order flag and
    ''' different shapes of equal length work. Storages may be of any numeric type and may differ: values
    ''' are read as doubles, the function is evaluated in double, and the result is stored with the
    ''' target's conversion (Java casts for integers, "f() &lt;&gt; 0" for BOOL). FLOAT-to-FLOAT and
    ''' DOUBLE-to-DOUBLE operands use typed loops.</para>
    ''' <para>x (or y) and z may be the same storage and view (in place). Overlapping but different views of
    ''' one storage give unspecified results, as in ND4J. Large operands are processed in parallel chunks
    ''' (<see cref="KernelOptions.ParallelThreshold"/>), so the functions must be thread-safe; they are
    ''' called with the same <c>extraArgs</c> array (never Nothing) and must not modify it.</para>
    ''' </remarks>
    Public NotInheritable Class ElementWiseKernels

        Private Sub New()
        End Sub

#Region "Transform"

        ''' <summary>z = f(x) for every element.</summary>
        Public Shared Sub Transform(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                    ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                    ByVal f As TransformFunction, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            KernelUtil.CheckSameLength(xShapeInfo, "x", zShapeInfo, "z")
            If f Is Nothing Then
                Throw New ArgumentNullException(NameOf(f))
            End If
            If ShapeInfoHelper.Length(xShapeInfo) = 0 Then
                Return
            End If
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo, zShapeInfo}, New Long() {xOffset, zOffset}, True, 1)
            If plan Is Nothing Then
                Dim dx As ManagedStorage = Nothing, dxo As Long, dxi As Long() = Nothing
                KernelUtil.Densify(x, xOffset, xShapeInfo, dx, dxo, dxi)
                WithDenseOutput(z, zOffset, zShapeInfo,
                    Sub(tz As ManagedStorage, tzo As Long, tzi As Long())
                        TransformPlan(dx, tz, StridedPlan.CreateAligned(New Long()() {dxi, tzi}, New Long() {dxo, tzo}, True, 1), f, extra)
                    End Sub)
                Return
            End If
            TransformPlan(x, z, plan, f, extra)
        End Sub

        Private Shared Sub TransformPlan(ByVal x As ManagedStorage, ByVal z As ManagedStorage, ByVal plan As StridedPlan, ByVal f As TransformFunction, ByVal extra As Double())
            Dim xInc As Integer = CInt(plan.InnerStrides(0))
            Dim zInc As Integer = CInt(plan.InnerStrides(1))
            Dim xf As Single() = x.Floats
            Dim zf As Single() = z.Floats
            Dim xd As Double() = x.Doubles
            Dim zd As Double() = z.Doubles
            If xf IsNot Nothing AndAlso zf IsNot Nothing Then
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xa As Single() = xf, za As Single() = zf, fn As TransformFunction = f, ea As Double() = extra
                        Dim xi As Integer = CInt(b(0)), zi As Integer = CInt(b(1))
                        For j As Integer = 0 To count - 1
                            za(zi) = CSng(fn(xa(xi), ea))
                            xi += xInc
                            zi += zInc
                        Next
                    End Sub)
            ElseIf xd IsNot Nothing AndAlso zd IsNot Nothing Then
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xa As Double() = xd, za As Double() = zd, fn As TransformFunction = f, ea As Double() = extra
                        Dim xi As Integer = CInt(b(0)), zi As Integer = CInt(b(1))
                        For j As Integer = 0 To count - 1
                            za(zi) = fn(xa(xi), ea)
                            xi += xInc
                            zi += zInc
                        Next
                    End Sub)
            Else
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xs As ManagedStorage = x, zs As ManagedStorage = z, fn As TransformFunction = f, ea As Double() = extra
                        Dim xi As Long = b(0), zi As Long = b(1)
                        For j As Integer = 0 To count - 1
                            zs.SetDouble(zi, fn(xs.GetDouble(xi), ea))
                            xi += xInc
                            zi += zInc
                        Next
                    End Sub)
            End If
        End Sub

#End Region

#Region "Scalar"

        ''' <summary>z = f(x, scalarValue) for every element (ND4J's scalar ops).</summary>
        Public Shared Sub Scalar(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                 ByVal scalarValue As Double,
                                 ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                 ByVal f As PairwiseFunction, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            KernelUtil.CheckSameLength(xShapeInfo, "x", zShapeInfo, "z")
            If f Is Nothing Then
                Throw New ArgumentNullException(NameOf(f))
            End If
            If ShapeInfoHelper.Length(xShapeInfo) = 0 Then
                Return
            End If
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo, zShapeInfo}, New Long() {xOffset, zOffset}, True, 1)
            If plan Is Nothing Then
                Dim dx As ManagedStorage = Nothing, dxo As Long, dxi As Long() = Nothing
                KernelUtil.Densify(x, xOffset, xShapeInfo, dx, dxo, dxi)
                WithDenseOutput(z, zOffset, zShapeInfo,
                    Sub(tz As ManagedStorage, tzo As Long, tzi As Long())
                        ScalarPlan(dx, tz, StridedPlan.CreateAligned(New Long()() {dxi, tzi}, New Long() {dxo, tzo}, True, 1), scalarValue, f, extra)
                    End Sub)
                Return
            End If
            ScalarPlan(x, z, plan, scalarValue, f, extra)
        End Sub

        Private Shared Sub ScalarPlan(ByVal x As ManagedStorage, ByVal z As ManagedStorage, ByVal plan As StridedPlan, ByVal s As Double, ByVal f As PairwiseFunction, ByVal extra As Double())
            Dim xInc As Integer = CInt(plan.InnerStrides(0))
            Dim zInc As Integer = CInt(plan.InnerStrides(1))
            Dim xf As Single() = x.Floats
            Dim zf As Single() = z.Floats
            Dim xd As Double() = x.Doubles
            Dim zd As Double() = z.Doubles
            If xf IsNot Nothing AndAlso zf IsNot Nothing Then
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xa As Single() = xf, za As Single() = zf, fn As PairwiseFunction = f, ea As Double() = extra, sv As Double = s
                        Dim xi As Integer = CInt(b(0)), zi As Integer = CInt(b(1))
                        For j As Integer = 0 To count - 1
                            za(zi) = CSng(fn(xa(xi), sv, ea))
                            xi += xInc
                            zi += zInc
                        Next
                    End Sub)
            ElseIf xd IsNot Nothing AndAlso zd IsNot Nothing Then
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xa As Double() = xd, za As Double() = zd, fn As PairwiseFunction = f, ea As Double() = extra, sv As Double = s
                        Dim xi As Integer = CInt(b(0)), zi As Integer = CInt(b(1))
                        For j As Integer = 0 To count - 1
                            za(zi) = fn(xa(xi), sv, ea)
                            xi += xInc
                            zi += zInc
                        Next
                    End Sub)
            Else
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xs As ManagedStorage = x, zs As ManagedStorage = z, fn As PairwiseFunction = f, ea As Double() = extra, sv As Double = s
                        Dim xi As Long = b(0), zi As Long = b(1)
                        For j As Integer = 0 To count - 1
                            zs.SetDouble(zi, fn(xs.GetDouble(xi), sv, ea))
                            xi += xInc
                            zi += zInc
                        Next
                    End Sub)
            End If
        End Sub

#End Region

#Region "Pairwise"

        ''' <summary>z = f(x, y) for every element; x, y and z have the same number of elements (ND4J's pairwise ops).</summary>
        Public Shared Sub Pairwise(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                   ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                   ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                   ByVal f As PairwiseFunction, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(y, yOffset, yShapeInfo, "y")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            KernelUtil.CheckSameLength(xShapeInfo, "x", yShapeInfo, "y")
            KernelUtil.CheckSameLength(xShapeInfo, "x", zShapeInfo, "z")
            If f Is Nothing Then
                Throw New ArgumentNullException(NameOf(f))
            End If
            If ShapeInfoHelper.Length(xShapeInfo) = 0 Then
                Return
            End If
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo, yShapeInfo, zShapeInfo}, New Long() {xOffset, yOffset, zOffset}, True, 2)
            If plan Is Nothing Then
                Dim dx As ManagedStorage = Nothing, dxo As Long, dxi As Long() = Nothing
                Dim dy As ManagedStorage = Nothing, dyo As Long, dyi As Long() = Nothing
                KernelUtil.Densify(x, xOffset, xShapeInfo, dx, dxo, dxi)
                KernelUtil.Densify(y, yOffset, yShapeInfo, dy, dyo, dyi)
                WithDenseOutput(z, zOffset, zShapeInfo,
                    Sub(tz As ManagedStorage, tzo As Long, tzi As Long())
                        PairwisePlan(dx, dy, tz, StridedPlan.CreateAligned(New Long()() {dxi, dyi, tzi}, New Long() {dxo, dyo, tzo}, True, 2), f, extra)
                    End Sub)
                Return
            End If
            PairwisePlan(x, y, z, plan, f, extra)
        End Sub

        ''' <summary>The three-operand loop shared with the broadcast kernels (operand order x, y, z).</summary>
        Friend Shared Sub PairwisePlan(ByVal x As ManagedStorage, ByVal y As ManagedStorage, ByVal z As ManagedStorage, ByVal plan As StridedPlan, ByVal f As PairwiseFunction, ByVal extra As Double())
            Dim xInc As Integer = CInt(plan.InnerStrides(0))
            Dim yInc As Integer = CInt(plan.InnerStrides(1))
            Dim zInc As Integer = CInt(plan.InnerStrides(2))
            Dim xf As Single() = x.Floats
            Dim yf As Single() = y.Floats
            Dim zf As Single() = z.Floats
            Dim xd As Double() = x.Doubles
            Dim yd As Double() = y.Doubles
            Dim zd As Double() = z.Doubles
            If xf IsNot Nothing AndAlso yf IsNot Nothing AndAlso zf IsNot Nothing Then
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xa As Single() = xf, ya As Single() = yf, za As Single() = zf, fn As PairwiseFunction = f, ea As Double() = extra
                        Dim xi As Integer = CInt(b(0)), yi As Integer = CInt(b(1)), zi As Integer = CInt(b(2))
                        For j As Integer = 0 To count - 1
                            za(zi) = CSng(fn(xa(xi), ya(yi), ea))
                            xi += xInc
                            yi += yInc
                            zi += zInc
                        Next
                    End Sub)
            ElseIf xd IsNot Nothing AndAlso yd IsNot Nothing AndAlso zd IsNot Nothing Then
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xa As Double() = xd, ya As Double() = yd, za As Double() = zd, fn As PairwiseFunction = f, ea As Double() = extra
                        Dim xi As Integer = CInt(b(0)), yi As Integer = CInt(b(1)), zi As Integer = CInt(b(2))
                        For j As Integer = 0 To count - 1
                            za(zi) = fn(xa(xi), ya(yi), ea)
                            xi += xInc
                            yi += yInc
                            zi += zInc
                        Next
                    End Sub)
            Else
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xs As ManagedStorage = x, ys As ManagedStorage = y, zs As ManagedStorage = z, fn As PairwiseFunction = f, ea As Double() = extra
                        Dim xi As Long = b(0), yi As Long = b(1), zi As Long = b(2)
                        For j As Integer = 0 To count - 1
                            zs.SetDouble(zi, fn(xs.GetDouble(xi), ys.GetDouble(yi), ea))
                            xi += xInc
                            yi += yInc
                            zi += zInc
                        Next
                    End Sub)
            End If
        End Sub

#End Region

#Region "Copy and fill"

        ''' <summary>
        ''' z = x converted to z's type (ND4J's assign): same-type copies move raw values (HALF and
        ''' BFLOAT16 bit patterns, strings included); other copies convert like
        ''' <see cref="ManagedStorage.CopyTo(Long, ManagedStorage, Long, Long)"/>.
        ''' </summary>
        Public Shared Sub Copy(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                               ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long())
            KernelUtil.CheckOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckOperand(z, zOffset, zShapeInfo, "z")
            KernelUtil.CheckSameLength(xShapeInfo, "x", zShapeInfo, "z")
            Dim n As Long = ShapeInfoHelper.Length(xShapeInfo)
            If n = 0 Then
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo, zShapeInfo}, New Long() {xOffset, zOffset}, True, 1)
            If plan Is Nothing Then
                ' shapes differ and an operand is not linearly strided: walk both in logical order
                Dim xo As Long() = ShapeInfoHelper.Offsets(xShapeInfo)
                Dim zo As Long() = ShapeInfoHelper.Offsets(zShapeInfo)
                For i As Integer = 0 To CInt(n) - 1
                    ManagedStorage.ConvertElement(x, xOffset + xo(i), z, zOffset + zo(i))
                Next
                Return
            End If
            CopyPlan(x, z, plan)
        End Sub

        ''' <summary>The two-operand copy loop (operand order x, z), shared with the broadcast kernels.</summary>
        Friend Shared Sub CopyPlan(ByVal x As ManagedStorage, ByVal z As ManagedStorage, ByVal plan As StridedPlan)
            If x.StorageType <> z.StorageType Then
                Dim xInc As Long = plan.InnerStrides(0)
                Dim zInc As Long = plan.InnerStrides(1)
                plan.RunAll(
                    Sub(b As Long(), count As Integer)
                        Dim xi As Long = b(0), zi As Long = b(1)
                        For j As Integer = 0 To count - 1
                            ManagedStorage.ConvertElement(x, xi, z, zi)
                            xi += xInc
                            zi += zInc
                        Next
                    End Sub)
                Return
            End If
            Select Case x.StorageType
                Case StorageType.FLOAT
                    CopyRuns(x.Floats, z.Floats, plan)
                Case StorageType.DOUBLE
                    CopyRuns(x.Doubles, z.Doubles, plan)
                Case StorageType.HALF, StorageType.BFLOAT16
                    CopyRuns(x.HalfBits, z.HalfBits, plan)
                Case StorageType.LONG
                    CopyRuns(x.Longs, z.Longs, plan)
                Case StorageType.INT
                    CopyRuns(x.Ints, z.Ints, plan)
                Case StorageType.SHORT
                    CopyRuns(x.Shorts, z.Shorts, plan)
                Case StorageType.BYTE
                    CopyRuns(x.SBytes, z.SBytes, plan)
                Case StorageType.UBYTE
                    CopyRuns(x.UBytes, z.UBytes, plan)
                Case StorageType.UINT16
                    CopyRuns(x.UShorts, z.UShorts, plan)
                Case StorageType.UINT32
                    CopyRuns(x.UInts, z.UInts, plan)
                Case StorageType.UINT64
                    CopyRuns(x.ULongs, z.ULongs, plan)
                Case StorageType.BOOL
                    CopyRuns(x.Booleans, z.Booleans, plan)
                Case StorageType.UTF8
                    CopyRuns(x.Strings, z.Strings, plan)
                Case Else
                    Throw New NotSupportedException(x.StorageType.ToString())
            End Select
        End Sub

        Private Shared Sub CopyRuns(Of T)(ByVal src As T(), ByVal dst As T(), ByVal plan As StridedPlan)
            Dim xInc As Integer = CInt(plan.InnerStrides(0))
            Dim zInc As Integer = CInt(plan.InnerStrides(1))
            plan.RunAll(
                Sub(b As Long(), count As Integer)
                    Dim s As T() = src, d As T() = dst
                    Dim xi As Integer = CInt(b(0)), zi As Integer = CInt(b(1))
                    If xInc = 1 AndAlso zInc = 1 Then
                        Array.Copy(s, xi, d, zi, count)
                        Return
                    End If
                    For j As Integer = 0 To count - 1
                        d(zi) = s(xi)
                        xi += xInc
                        zi += zInc
                    Next
                End Sub)
        End Sub

        ''' <summary>Sets every element of the view to <paramref name="value"/> (converted as <see cref="ManagedStorage.SetDouble"/> does).</summary>
        Public Shared Sub Fill(ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(), ByVal value As Double)
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            If ShapeInfoHelper.Length(zShapeInfo) = 0 Then
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {zShapeInfo}, New Long() {zOffset}, True, 0)
            Dim zInc As Long = plan.InnerStrides(0)
            plan.RunAll(
                Sub(b As Long(), count As Integer)
                    If zInc = 1 Then
                        z.Fill(value, b(0), count)
                        Return
                    End If
                    Dim zi As Long = b(0)
                    For j As Integer = 0 To count - 1
                        z.SetDouble(zi, value)
                        zi += zInc
                    Next
                End Sub)
        End Sub

#End Region

        ''' <summary>
        ''' Runs <paramref name="compute"/> on z directly when z is linearly strided; otherwise on a dense
        ''' temporary of z's type, which is then copied into z.
        ''' </summary>
        Friend Shared Sub WithDenseOutput(ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(), ByVal compute As Action(Of ManagedStorage, Long, Long()))
            If ShapeInfoHelper.LinearStride(zShapeInfo) <> 0 Then
                compute(z, zOffset, zShapeInfo)
                Return
            End If
            Dim n As Long = ShapeInfoHelper.Length(zShapeInfo)
            Dim tz As ManagedStorage = ManagedStorage.Create(z.StorageType, n)
            Dim ti As Long() = KernelUtil.VectorInfo(n)
            compute(tz, 0, ti)
            Copy(tz, 0, ti, z, zOffset, zShapeInfo)
        End Sub

    End Class

End Namespace
