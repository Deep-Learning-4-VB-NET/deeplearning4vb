Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>Variance and standard deviation (Welford, Chan merges) against a two-pass reference.</summary>
    <TestClass>
    Public Class SummaryStatsKernelsTest

        Private Shared Function TwoPassVariance(ByVal values As Double(), ByVal biasCorrected As Boolean) As Double
            If values.Length <= 1 Then
                Return 0.0
            End If
            Dim mean As Double = 0
            For Each v As Double In values
                mean += v
            Next
            mean /= values.Length
            Dim ss As Double = 0
            For Each v As Double In values
                ss += (v - mean) * (v - mean)
            Next
            Return ss / If(biasCorrected, values.Length - 1, values.Length)
        End Function

        <TestMethod>
        Public Sub VarianceWithAndWithoutBiasCorrection()
            Dim values As Double() = {2, 4, 4, 4, 5, 5, 7, 9}
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {2, 4}, "f"c, values, 1, 2)
            Assert.AreEqual(4.0, SummaryStatsKernels.Variance(x.Storage, x.Offset, x.Info, False), 1.0E-12)
            Assert.AreEqual(32.0 / 7.0, SummaryStatsKernels.Variance(x.Storage, x.Offset, x.Info, True), 1.0E-12)
            Assert.AreEqual(2.0, SummaryStatsKernels.StandardDeviation(x.Storage, x.Offset, x.Info, False), 1.0E-12)
            Assert.AreEqual(Math.Sqrt(32.0 / 7.0), SummaryStatsKernels.StandardDeviation(x.Storage, x.Offset, x.Info, True), 1.0E-12)
            ' a single element and an empty array give 0, as libnd4j does
            Dim one As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {1}, "c"c, New Double() {3})
            Assert.AreEqual(0.0, SummaryStatsKernels.Variance(one.Storage, one.Offset, one.Info, True), 0.0)
            Assert.AreEqual(0.0, SummaryStatsKernels.Variance(one.Storage, 0, ShapeInfoHelper.Create(New Long() {0}, "c"c), False), 0.0)
        End Sub

        <TestMethod>
        Public Sub VarianceAlongDimensions()
            Dim shape As Long() = {3, 4, 5}
            Dim values As Double() = New Double(59) {}
            For i As Integer = 0 To 59
                values(i) = ((i * 37) Mod 11) * 0.5 - 2.0
            Next
            For Each order As Char In {"c"c, "f"c}
                Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, order, values, 2, 2)
                For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                    Dim outer As Integer() = KernelTestUtil.Outer(3, dims)
                    Dim groups As Integer = 1
                    For Each d As Integer In outer
                        groups *= CInt(shape(d))
                    Next
                    Dim members As New System.Collections.Generic.List(Of Double)()
                    For Each bias As Boolean In {False, True}
                        Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, TadPack.OuterShape(x.Info, dims), "c"c, 1)
                        Dim zs As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, TadPack.OuterShape(x.Info, dims), "f"c)
                        SummaryStatsKernels.VarianceAlongDimensions(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, dims, bias, False)
                        SummaryStatsKernels.VarianceAlongDimensions(x.Storage, x.Offset, x.Info, zs.Storage, zs.Offset, zs.Info, dims, bias, True)
                        For g As Integer = 0 To groups - 1
                            members.Clear()
                            For i As Long = 0 To 59
                                If KernelTestUtil.LinearIndex(shape, KernelTestUtil.Coordinates(shape, i), outer) = g Then
                                    members.Add(values(CInt(i)))
                                End If
                            Next
                            Dim expected As Double = TwoPassVariance(members.ToArray(), bias)
                            Dim label As String = order & " dims " & KernelTestUtil.Describe(dims) & " bias " & bias & " group " & g
                            Assert.AreEqual(expected, z.GetValue(g), 1.0E-12, label)
                            Assert.AreEqual(Math.Sqrt(expected), zs.GetValue(g), 1.0E-5, label & " std")
                        Next
                        z.AssertOutsideUntouched()
                    Next
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub ParallelWelfordMatchesTwoPass()
            Dim n As Integer = 123457
            Dim data As Double() = New Double(n - 1) {}
            Dim rng As New Random(5)
            For i As Integer = 0 To n - 1
                data(i) = 1000.0 + rng.NextDouble() * 3.0   ' large mean, small spread: hard for naive formulas
            Next
            Dim s As ManagedStorage = ManagedStorage.FromArray(data)
            Dim info As Long() = ShapeInfoHelper.Create(New Long() {n}, "c"c)
            Dim expected As Double = TwoPassVariance(data, True)
            Assert.AreEqual(expected, SummaryStatsKernels.Variance(s, 0, info, True), expected * 1.0E-10)
            KernelTestUtil.WithLowThresholds(
                Sub()
                    Assert.AreEqual(expected, SummaryStatsKernels.Variance(s, 0, info, True), expected * 1.0E-10)
                    Dim z As ManagedStorage = ManagedStorage.Create(StorageType.DOUBLE, 1)
                    SummaryStatsKernels.VarianceAlongDimensions(s, 0, info, z, 0, ShapeInfoHelper.Create(New Long() {1}, "c"c), New Integer() {0}, True, False)
                    Assert.AreEqual(expected, z.GetDouble(0), expected * 1.0E-10)
                End Sub)
        End Sub

    End Class

End Namespace
