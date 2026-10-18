' DeepLearning4VB: strided multi-operand iteration shared by the managed kernels.

Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>Processes one run: <paramref name="count"/> elements whose first element of operand k is at <c>bases(k)</c>; the next elements follow at the plan's inner strides.</summary>
    Friend Delegate Sub RunBody(ByVal bases As Long(), ByVal count As Integer)

    ''' <summary>
    ''' Iteration of one or more operands over a common logical shape. Dimensions of size 1 are dropped,
    ''' adjacent dimensions that are contiguous for every operand are merged, and (when the caller allows
    ''' it) dimensions are reordered by descending stride of one operand so that memory is walked in
    ''' order. Without reordering the walk is the row-major logical order. The walk is cut into runs
    ''' along the innermost remaining dimension; each run has constant per-operand strides.
    ''' </summary>
    Friend NotInheritable Class StridedPlan

        Friend ReadOnly Rank As Integer
        Friend ReadOnly Shape As Long()
        ''' <summary>Strides per operand: <c>Strides(k)(d)</c>.</summary>
        Friend ReadOnly Strides As Long()()
        ''' <summary>Base element offset per operand.</summary>
        Friend ReadOnly Offsets As Long()
        Friend ReadOnly Length As Long
        ''' <summary>Size of the innermost dimension (the longest possible run).</summary>
        Friend ReadOnly InnerLength As Long
        ''' <summary>Stride of each operand along the innermost dimension.</summary>
        Friend ReadOnly InnerStrides As Long()

        Private Sub New(ByVal shape As Long(), ByVal strides As Long()(), ByVal offsets As Long())
            Me.Shape = shape
            Me.Strides = strides
            Me.Offsets = offsets
            Rank = shape.Length
            Dim n As Long = 1
            For Each s As Long In shape
                n *= s
            Next
            Length = n
            InnerLength = shape(Rank - 1)
            InnerStrides = New Long(strides.Length - 1) {}
            For k As Integer = 0 To strides.Length - 1
                InnerStrides(k) = strides(k)(Rank - 1)
            Next
        End Sub

        ''' <summary>Number of operands.</summary>
        Friend ReadOnly Property OperandCount As Integer
            Get
                Return Offsets.Length
            End Get
        End Property

        ''' <summary>
        ''' Builds a plan for operands that share <paramref name="shape"/>; <c>strides(k)</c> are the
        ''' strides of operand k in that shape (0 for broadcast dimensions).
        ''' </summary>
        Friend Shared Function Create(ByVal shape As Long(), ByVal strides As Long()(), ByVal offsets As Long(), ByVal reorder As Boolean, ByVal orderOperand As Integer) As StridedPlan
            Dim ops As Integer = strides.Length
            Dim dims As New List(Of Integer)()
            For d As Integer = 0 To shape.Length - 1
                If shape(d) <> 1 Then
                    dims.Add(d)
                End If
            Next
            If dims.Count = 0 Then
                Dim unitStrides As Long()() = New Long(ops - 1)() {}
                For k As Integer = 0 To ops - 1
                    unitStrides(k) = New Long() {0L}
                Next
                Return New StridedPlan(New Long() {1L}, unitStrides, CType(offsets.Clone(), Long()))
            End If
            If reorder AndAlso dims.Count > 1 Then
                ' stable insertion sort by descending |stride| of the order operand
                Dim key As Long() = strides(orderOperand)
                For i As Integer = 1 To dims.Count - 1
                    Dim cur As Integer = dims(i)
                    Dim j As Integer = i - 1
                    Do While j >= 0 AndAlso SysMath.Abs(key(dims(j))) < SysMath.Abs(key(cur))
                        dims(j + 1) = dims(j)
                        j -= 1
                    Loop
                    dims(j + 1) = cur
                Next
            End If
            ' merge adjacent dimensions that are contiguous for every operand
            Dim newShape As New List(Of Long)()
            Dim newStrides As List(Of Long)() = New List(Of Long)(ops - 1) {}
            For k As Integer = 0 To ops - 1
                newStrides(k) = New List(Of Long)()
            Next
            For Each d As Integer In dims
                Dim last As Integer = newShape.Count - 1
                Dim mergeable As Boolean = last >= 0
                If mergeable Then
                    For k As Integer = 0 To ops - 1
                        If newStrides(k)(last) <> strides(k)(d) * shape(d) Then
                            mergeable = False
                            Exit For
                        End If
                    Next
                End If
                If mergeable Then
                    newShape(last) = newShape(last) * shape(d)
                    For k As Integer = 0 To ops - 1
                        newStrides(k)(last) = strides(k)(d)
                    Next
                Else
                    newShape.Add(shape(d))
                    For k As Integer = 0 To ops - 1
                        newStrides(k).Add(strides(k)(d))
                    Next
                End If
            Next
            Dim finalStrides As Long()() = New Long(ops - 1)() {}
            For k As Integer = 0 To ops - 1
                finalStrides(k) = newStrides(k).ToArray()
            Next
            Return New StridedPlan(newShape.ToArray(), finalStrides, CType(offsets.Clone(), Long()))
        End Function

        ''' <summary>
        ''' Builds a plan for operands given by their own shape infos, matched by row-major logical index.
        ''' Returns Nothing when the shapes differ in more than dimensions of size 1 and the operands are
        ''' not all linearly strided (the caller then gathers or scatters through dense copies).
        ''' </summary>
        Friend Shared Function CreateAligned(ByVal infos As Long()(), ByVal offsets As Long(), ByVal reorder As Boolean, ByVal orderOperand As Integer) As StridedPlan
            Dim ops As Integer = infos.Length
            Dim firstShape As Long() = ShapeInfoHelper.GetShape(infos(0))
            Dim aligned As Boolean = True
            For k As Integer = 1 To ops - 1
                If Not ShapeInfoHelper.SqueezedShapesEqual(firstShape, ShapeInfoHelper.GetShape(infos(k))) Then
                    aligned = False
                    Exit For
                End If
            Next
            If aligned Then
                ' common squeezed shape: every operand's non-unit dimensions in order
                Dim common As New List(Of Long)()
                For Each s As Long In firstShape
                    If s <> 1 Then
                        common.Add(s)
                    End If
                Next
                Dim strides As Long()() = New Long(ops - 1)() {}
                For k As Integer = 0 To ops - 1
                    Dim shp As Long() = ShapeInfoHelper.GetShape(infos(k))
                    Dim str As Long() = ShapeInfoHelper.GetStrides(infos(k))
                    Dim list As New List(Of Long)()
                    For d As Integer = 0 To shp.Length - 1
                        If shp(d) <> 1 Then
                            list.Add(str(d))
                        End If
                    Next
                    strides(k) = list.ToArray()
                Next
                Return Create(common.ToArray(), strides, offsets, reorder, orderOperand)
            End If
            ' different shapes of equal length: fine when every operand is linearly strided
            Dim n As Long = ShapeInfoHelper.Length(infos(0))
            Dim linear As Long()() = New Long(ops - 1)() {}
            For k As Integer = 0 To ops - 1
                If ShapeInfoHelper.Length(infos(k)) <> n Then
                    Return Nothing
                End If
                Dim ls As Long = ShapeInfoHelper.LinearStride(infos(k))
                If ls = 0 Then
                    Return Nothing
                End If
                linear(k) = New Long() {ls}
            Next
            Return Create(New Long() {n}, linear, offsets, False, 0)
        End Function

        ''' <summary>Walks elements [start, start + count) of the plan's order from the plan's own base offsets.</summary>
        Friend Sub Run(ByVal start As Long, ByVal count As Long, ByVal body As RunBody)
            Run(start, count, Offsets, body)
        End Sub

        ''' <summary>Walks elements [start, start + count) of the plan's order from the given base offsets (one per operand).</summary>
        Friend Sub Run(ByVal start As Long, ByVal count As Long, ByVal baseOffsets As Long(), ByVal body As RunBody)
            If count <= 0 Then
                Return
            End If
            Dim ops As Integer = InnerStrides.Length
            Dim bases As Long() = New Long(ops - 1) {}
            If Rank = 1 Then
                For k As Integer = 0 To ops - 1
                    bases(k) = baseOffsets(k) + start * InnerStrides(k)
                Next
                body(bases, CInt(count))
                Return
            End If
            Dim inner As Long = InnerLength
            Dim outerRank As Integer = Rank - 1
            Dim row As Long = start \ inner
            Dim col As Long = start - row * inner
            Dim coords As Long() = New Long(outerRank - 1) {}
            Dim rowBase As Long() = New Long(ops - 1) {}
            For k As Integer = 0 To ops - 1
                rowBase(k) = baseOffsets(k)
            Next
            Dim rest As Long = row
            For d As Integer = outerRank - 1 To 0 Step -1
                Dim c As Long = rest Mod Shape(d)
                rest \= Shape(d)
                coords(d) = c
                For k As Integer = 0 To ops - 1
                    rowBase(k) += c * Strides(k)(d)
                Next
            Next
            Dim remaining As Long = count
            Do
                Dim runLength As Long = SysMath.Min(inner - col, remaining)
                For k As Integer = 0 To ops - 1
                    bases(k) = rowBase(k) + col * InnerStrides(k)
                Next
                body(bases, CInt(runLength))
                remaining -= runLength
                If remaining <= 0 Then
                    Exit Do
                End If
                col = 0
                Dim dd As Integer = outerRank - 1
                Do
                    coords(dd) += 1
                    For k As Integer = 0 To ops - 1
                        rowBase(k) += Strides(k)(dd)
                    Next
                    If coords(dd) < Shape(dd) Then
                        Exit Do
                    End If
                    For k As Integer = 0 To ops - 1
                        rowBase(k) -= Shape(dd) * Strides(k)(dd)
                    Next
                    coords(dd) = 0
                    dd -= 1
                Loop While dd >= 0
            Loop
        End Sub

        ''' <summary>Runs <paramref name="body"/> over all elements, in parallel chunks above the threshold. The body must be safe to run concurrently on disjoint ranges.</summary>
        Friend Sub RunAll(ByVal body As RunBody)
            Dim n As Long = Length
            If n <= 0 Then
                Return
            End If
            If Not KernelOptions.ShouldParallelize(n) Then
                Run(0, n, Offsets, body)
                Return
            End If
            Dim chunks As Integer = KernelOptions.ChunkCount(n)
            If chunks <= 1 Then
                Run(0, n, Offsets, body)
                Return
            End If
            Parallel.For(0, chunks, KernelOptions.CreateParallelOptions(),
                Sub(c As Integer)
                    Dim s As Long = n * c \ chunks
                    Dim e As Long = n * (c + 1) \ chunks
                    Run(s, e - s, Offsets, body)
                End Sub)
        End Sub

    End Class

    ''' <summary>Fixed-size chunking of a range for deterministic parallel reductions.</summary>
    Friend NotInheritable Class ReductionChunks

        Private Sub New()
        End Sub

        ''' <summary>Number of chunks of <see cref="KernelOptions.ReductionChunkSize"/> elements covering <paramref name="n"/> elements (1 below the parallel threshold).</summary>
        Friend Shared Function Count(ByVal n As Long) As Integer
            If Not KernelOptions.ShouldParallelize(n) Then
                Return 1
            End If
            Dim size As Long = SysMath.Max(16L, KernelOptions.ReductionChunkSize)
            Return CInt(SysMath.Max(1L, (n + size - 1) \ size))
        End Function

        ''' <summary>First element of chunk <paramref name="c"/> of <paramref name="chunks"/>.</summary>
        Friend Shared Function Start(ByVal n As Long, ByVal c As Integer, ByVal chunks As Integer) As Long
            If chunks = 1 Then
                Return If(c = 0, 0L, n)
            End If
            Dim size As Long = SysMath.Max(16L, KernelOptions.ReductionChunkSize)
            Return SysMath.Min(n, c * size)
        End Function

    End Class

End Namespace
