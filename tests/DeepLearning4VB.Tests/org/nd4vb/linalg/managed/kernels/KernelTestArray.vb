Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' A test operand: a view (offset, shape info) into a storage whose other elements hold a sentinel,
    ''' so tests can check that kernels touch nothing outside the view.
    ''' </summary>
    Friend NotInheritable Class KernelTestArray

        Public Const Sentinel As Double = -777.0

        Public ReadOnly Storage As ManagedStorage
        Public ReadOnly Offset As Long
        Public ReadOnly Info As Long()
        Private ReadOnly _inView As HashSet(Of Long)

        Private Sub New(ByVal s As ManagedStorage, ByVal offset As Long, ByVal info As Long())
            Storage = s
            Me.Offset = offset
            Me.Info = info
            _inView = New HashSet(Of Long)()
            For Each o As Long In ShapeInfoHelper.Offsets(info)
                _inView.Add(offset + o)
            Next
        End Sub

        Public ReadOnly Property Shape As Long()
            Get
                Return ShapeInfoHelper.GetShape(Info)
            End Get
        End Property

        Public ReadOnly Property Length As Long
            Get
                Return ShapeInfoHelper.Length(Info)
            End Get
        End Property

        ''' <summary>A view with explicit strides; the storage is just large enough plus <paramref name="slack"/> elements, all sentinels.</summary>
        Public Shared Function Strided(ByVal type As StorageType, ByVal shape As Long(), ByVal strides As Long(), ByVal order As Char, ByVal offset As Long, Optional ByVal slack As Long = 3) As KernelTestArray
            Dim maxOffset As Long = 0
            For d As Integer = 0 To shape.Length - 1
                If shape(d) > 0 Then
                    maxOffset += (shape(d) - 1) * strides(d)
                End If
            Next
            Dim s As ManagedStorage = ManagedStorage.Create(type, offset + maxOffset + 1 + slack)
            If type <> StorageType.BOOL Then
                s.Fill(Sentinel)
            End If
            Return New KernelTestArray(s, offset, ShapeInfoHelper.Create(shape, strides, order))
        End Function

        ''' <summary>
        ''' A view of the given shape and order whose elements are <paramref name="spacing"/> apart in
        ''' memory (1 = contiguous), starting at <paramref name="offset"/>.
        ''' </summary>
        Public Shared Function Create(ByVal type As StorageType, ByVal shape As Long(), ByVal order As Char, Optional ByVal offset As Long = 0, Optional ByVal spacing As Long = 1) As KernelTestArray
            Dim strides As Long() = ShapeInfoHelper.CalculateStrides(shape, order)
            For d As Integer = 0 To strides.Length - 1
                strides(d) *= spacing
            Next
            Return Strided(type, shape, strides, order, offset)
        End Function

        ''' <summary>Like <see cref="Create"/>, filled with <paramref name="values"/> in row-major logical order.</summary>
        Public Shared Function FromValues(ByVal type As StorageType, ByVal shape As Long(), ByVal order As Char, ByVal values As Double(), Optional ByVal offset As Long = 0, Optional ByVal spacing As Long = 1) As KernelTestArray
            Dim a As KernelTestArray = Create(type, shape, order, offset, spacing)
            a.SetAll(values)
            Return a
        End Function

        ''' <summary>A row-major sequence 1, 2, 3, ... (times <paramref name="scale"/>) in the given layout.</summary>
        Public Shared Function Sequence(ByVal type As StorageType, ByVal shape As Long(), ByVal order As Char, Optional ByVal offset As Long = 0, Optional ByVal spacing As Long = 1, Optional ByVal scale As Double = 1.0) As KernelTestArray
            Dim n As Long = ShapeInfoHelper.ShapeLength(shape)
            Dim v As Double() = New Double(CInt(n) - 1) {}
            For i As Integer = 0 To v.Length - 1
                v(i) = (i + 1) * scale
            Next
            Return FromValues(type, shape, order, v, offset, spacing)
        End Function

        Public Function GetValue(ByVal index As Long) As Double
            Return Storage.GetDouble(Offset + ShapeInfoHelper.OffsetOf(Info, index))
        End Function

        Public Sub SetValue(ByVal index As Long, ByVal value As Double)
            Storage.SetDouble(Offset + ShapeInfoHelper.OffsetOf(Info, index), value)
        End Sub

        Public Sub SetAll(ByVal values As Double())
            Assert.AreEqual(Length, CLng(values.Length), "value count")
            For i As Integer = 0 To values.Length - 1
                SetValue(i, values(i))
            Next
        End Sub

        Public Function Values() As Double()
            Dim r As Double() = New Double(CInt(Length) - 1) {}
            For i As Integer = 0 To r.Length - 1
                r(i) = GetValue(i)
            Next
            Return r
        End Function

        ''' <summary>Asserts that every storage element outside the view still holds the sentinel.</summary>
        Public Sub AssertOutsideUntouched()
            If Storage.StorageType = StorageType.BOOL Then
                Return
            End If
            ' the sentinel as this storage type holds it
            Dim probe As ManagedStorage = ManagedStorage.Create(Storage.StorageType, 1)
            probe.SetDouble(0, Sentinel)
            Dim expected As Double = probe.GetDouble(0)
            For i As Long = 0 To Storage.Length - 1
                If Not _inView.Contains(i) Then
                    Assert.AreEqual(expected, Storage.GetDouble(i), 0.0, "storage element " & i & " outside the view was modified")
                End If
            Next
        End Sub

        ''' <summary>Asserts the view's values (row-major logical order).</summary>
        Public Sub AssertValues(ByVal expected As Double(), Optional ByVal delta As Double = 0.0, Optional ByVal message As String = "")
            Assert.AreEqual(CLng(expected.Length), Length, message & " length")
            For i As Integer = 0 To expected.Length - 1
                Assert.AreEqual(expected(i), GetValue(i), delta, message & " element " & i)
            Next
        End Sub

    End Class

    ''' <summary>Reference helpers for the kernel tests.</summary>
    Friend NotInheritable Class KernelTestUtil

        Private Sub New()
        End Sub

        ''' <summary>Row-major coordinates of a logical index.</summary>
        Public Shared Function Coordinates(ByVal shape As Long(), ByVal index As Long) As Long()
            Return ShapeInfoHelper.IndexToCoordinates(shape, index)
        End Function

        ''' <summary>Row-major linear index of coordinates restricted to the given dimensions.</summary>
        Public Shared Function LinearIndex(ByVal shape As Long(), ByVal coords As Long(), ByVal dims As Integer()) As Long
            Dim idx As Long = 0
            For Each d As Integer In dims
                idx = idx * shape(d) + coords(d)
            Next
            Return idx
        End Function

        ''' <summary>All subsets of {0 .. rank-1}, each sorted.</summary>
        Public Shared Function AllDimensionSets(ByVal rank As Integer) As List(Of Integer())
            Dim result As New List(Of Integer())()
            For mask As Integer = 0 To (1 << rank) - 1
                Dim dims As New List(Of Integer)()
                For d As Integer = 0 To rank - 1
                    If (mask And (1 << d)) <> 0 Then
                        dims.Add(d)
                    End If
                Next
                result.Add(dims.ToArray())
            Next
            Return result
        End Function

        ''' <summary>Dimensions not in <paramref name="dims"/> (all of them when dims is empty → none).</summary>
        Public Shared Function Outer(ByVal rank As Integer, ByVal dims As Integer()) As Integer()
            If dims.Length = 0 Then
                Return New Integer() {}
            End If
            Dim result As New List(Of Integer)()
            For d As Integer = 0 To rank - 1
                If Array.IndexOf(dims, d) < 0 Then
                    result.Add(d)
                End If
            Next
            Return result.ToArray()
        End Function

        Public Shared Function Describe(ByVal dims As Integer()) As String
            Return "[" & String.Join(",", dims) & "]"
        End Function

        ''' <summary>A simple sum definition.</summary>
        Public Shared Function SumDefinition() As ReduceDefinition
            Dim d As New ReduceDefinition()
            d.Name = "sum"
            d.StartingValue = Function(e As Double()) 0.0
            d.Map = Function(v As Double, e As Double()) v
            d.Update = Function(a As Double, b As Double, e As Double()) a + b
            d.Merge = Function(a As Double, b As Double, e As Double()) a + b
            d.PostProcess = Function(a As Double, n As Long, e As Double()) a
            Return d
        End Function

        ''' <summary>A max definition.</summary>
        Public Shared Function MaxDefinition() As ReduceDefinition
            Dim d As New ReduceDefinition()
            d.Name = "max"
            d.StartingValue = Function(e As Double()) Double.NegativeInfinity
            d.Update = Function(a As Double, b As Double, e As Double()) Math.Max(a, b)
            d.Merge = Function(a As Double, b As Double, e As Double()) Math.Max(a, b)
            Return d
        End Function

        ''' <summary>A mean definition (sum, then divide by n in PostProcess).</summary>
        Public Shared Function MeanDefinition() As ReduceDefinition
            Dim d As ReduceDefinition = SumDefinition()
            d.Name = "mean"
            d.PostProcess = Function(a As Double, n As Long, e As Double()) a / n
            Return d
        End Function

        ''' <summary>A reduce3 dot product.</summary>
        Public Shared Function DotDefinition() As ReduceDefinition
            Dim d As ReduceDefinition = SumDefinition()
            d.Name = "dot"
            d.MapPair = Function(x As Double, y As Double, e As Double()) x * y
            Return d
        End Function

        ''' <summary>argmax: strict comparison, so ties keep the first index.</summary>
        Public Shared Function ArgMaxDefinition() As IndexReduceDefinition
            Dim d As New IndexReduceDefinition()
            d.Name = "argmax"
            d.StartingValue = Function(e As Double()) Double.NegativeInfinity
            d.Prefer = Function(v As Double, i As Long, bv As Double, bi As Long, e As Double()) v > bv
            d.EmptyIndex = 0
            Return d
        End Function

        ''' <summary>argmin: strict comparison, so ties keep the first index.</summary>
        Public Shared Function ArgMinDefinition() As IndexReduceDefinition
            Dim d As New IndexReduceDefinition()
            d.Name = "argmin"
            d.StartingValue = Function(e As Double()) Double.PositiveInfinity
            d.Prefer = Function(v As Double, i As Long, bv As Double, bi As Long, e As Double()) v < bv
            d.EmptyIndex = 0
            Return d
        End Function

        ''' <summary>First index whose value exceeds extraArgs(0); -1 when none.</summary>
        Public Shared Function FirstIndexAboveDefinition() As IndexReduceDefinition
            Dim d As New IndexReduceDefinition()
            d.Name = "firstindex"
            d.StartingValue = Function(e As Double()) 0.0
            d.Prefer = Function(v As Double, i As Long, bv As Double, bi As Long, e As Double()) v > e(0) AndAlso (bi < 0 OrElse i < bi)
            d.EmptyIndex = -1
            Return d
        End Function

        ''' <summary>Last index whose value exceeds extraArgs(0); -1 when none.</summary>
        Public Shared Function LastIndexAboveDefinition() As IndexReduceDefinition
            Dim d As New IndexReduceDefinition()
            d.Name = "lastindex"
            d.StartingValue = Function(e As Double()) 0.0
            d.Prefer = Function(v As Double, i As Long, bv As Double, bi As Long, e As Double()) v > e(0) AndAlso i > bi
            d.EmptyIndex = -1
            Return d
        End Function

        ''' <summary>Runs <paramref name="action"/> with parallelism forced on small sizes (low thresholds), restoring the settings afterwards.</summary>
        Public Shared Sub WithLowThresholds(ByVal action As Action)
            Dim pt As Long = KernelOptions.ParallelThreshold
            Dim cs As Long = KernelOptions.ReductionChunkSize
            Dim gt As Long = KernelOptions.GemmParallelThreshold
            Try
                KernelOptions.ParallelThreshold = 16
                KernelOptions.ReductionChunkSize = 64
                KernelOptions.GemmParallelThreshold = 1
                action()
            Finally
                KernelOptions.ParallelThreshold = pt
                KernelOptions.ReductionChunkSize = cs
                KernelOptions.GemmParallelThreshold = gt
            End Try
        End Sub

    End Class

End Namespace
