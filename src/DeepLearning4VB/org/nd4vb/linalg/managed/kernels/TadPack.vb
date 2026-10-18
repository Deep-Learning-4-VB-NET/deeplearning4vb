' DeepLearning4VB: tensor-along-dimension (TAD) shape infos and offsets, as libnd4j's TAD helper computes them.

Imports System
Imports System.Collections.Generic

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' The tensors along dimensions (TADs) of an array: for a shape info and a set of dimensions, the
    ''' shape info shared by every TAD and the element offset of each TAD, as libnd4j's
    ''' <c>ConstantTadHelper</c>/<c>shape::calcSubArrsShapeInfoAndOffsets</c> compute them.
    ''' </summary>
    ''' <remarks>
    ''' <para>Dimensions are normalized: negative values count from the end, duplicates are removed and
    ''' the list is sorted. The TAD shape is the array's shape restricted to those dimensions (in
    ''' ascending dimension order, unit dimensions kept only if they were asked for) with the array's
    ''' strides; its order flag is the array's and its element-wise stride is recomputed. An empty
    ''' dimension list, or one naming every dimension, gives a single TAD that is the whole array.</para>
    ''' <para>TADs are numbered in row-major order over the remaining ("outer") dimensions, so TAD
    ''' <c>i</c> fills element <c>i</c> (row-major logical index) of a reduction result whose shape is the
    ''' outer shape, with or without kept unit dimensions. <see cref="TadOffsets"/> are relative to the
    ''' array's own first element. Instances are immutable and may be cached and shared.</para>
    ''' </remarks>
    Public NotInheritable Class TadPack

        Private ReadOnly _dimensions As Integer()
        Private ReadOnly _outerDimensions As Integer()
        Private ReadOnly _tadShapeInfo As Long()
        Private ReadOnly _tadOffsets As Long()
        Private ReadOnly _tadLength As Long

        Private Sub New(ByVal dimensions As Integer(), ByVal outerDimensions As Integer(), ByVal tadShapeInfo As Long(), ByVal tadOffsets As Long(), ByVal tadLength As Long)
            _dimensions = dimensions
            _outerDimensions = outerDimensions
            _tadShapeInfo = tadShapeInfo
            _tadOffsets = tadOffsets
            _tadLength = tadLength
        End Sub

        ''' <summary>The normalized TAD dimensions (sorted, non-negative, distinct).</summary>
        Public ReadOnly Property Dimensions As Integer()
            Get
                Return CType(_dimensions.Clone(), Integer())
            End Get
        End Property

        ''' <summary>The dimensions not in the TAD, ascending; TADs are numbered row-major over these.</summary>
        Public ReadOnly Property OuterDimensions As Integer()
            Get
                Return CType(_outerDimensions.Clone(), Integer())
            End Get
        End Property

        ''' <summary>The shape info of every TAD (shared; do not modify).</summary>
        Public ReadOnly Property TadShapeInfo As Long()
            Get
                Return _tadShapeInfo
            End Get
        End Property

        ''' <summary>Element offset of each TAD's first element, relative to the array's first element (shared; do not modify).</summary>
        Public ReadOnly Property TadOffsets As Long()
            Get
                Return _tadOffsets
            End Get
        End Property

        ''' <summary>Number of TADs (the product of the outer dimensions).</summary>
        Public ReadOnly Property NumTads As Long
            Get
                Return _tadOffsets.LongLength
            End Get
        End Property

        ''' <summary>Number of elements in each TAD.</summary>
        Public ReadOnly Property TadLength As Long
            Get
                Return _tadLength
            End Get
        End Property

        ''' <summary>Normalizes a dimension list for an array of rank <paramref name="rank"/>: negative values count from the end, duplicates go, the result is sorted.</summary>
        ''' <exception cref="ArgumentOutOfRangeException">A dimension is outside [-rank, rank).</exception>
        Public Shared Function NormalizeDimensions(ByVal rank As Integer, ByVal dimensions As Integer()) As Integer()
            Dim result As New SortedSet(Of Integer)()
            If dimensions IsNot Nothing Then
                For Each d As Integer In dimensions
                    Dim nd As Integer = If(d < 0, d + rank, d)
                    If nd < 0 OrElse nd >= rank Then
                        Throw New ArgumentOutOfRangeException(NameOf(dimensions), "Dimension " & d & " is out of range for rank " & rank)
                    End If
                    result.Add(nd)
                Next
            End If
            Dim arr As Integer() = New Integer(result.Count - 1) {}
            result.CopyTo(arr)
            Return arr
        End Function

        ''' <summary>Computes the TADs of the array described by <paramref name="shapeInfo"/> along <paramref name="dimensions"/>.</summary>
        Public Shared Function Create(ByVal shapeInfo As Long(), ByVal ParamArray dimensions As Integer()) As TadPack
            ShapeInfoHelper.Validate(shapeInfo, NameOf(shapeInfo))
            Dim rank As Integer = CInt(shapeInfo(0))
            Dim dims As Integer() = NormalizeDimensions(rank, dimensions)
            Dim shape As Long() = ShapeInfoHelper.GetShape(shapeInfo)
            Dim strides As Long() = ShapeInfoHelper.GetStrides(shapeInfo)
            Dim order As Char = ShapeInfoHelper.GetOrder(shapeInfo)
            Dim extras As Long = ShapeInfoHelper.Extras(shapeInfo)

            If dims.Length = 0 OrElse dims.Length = rank Then
                Dim all As Integer() = New Integer(rank - 1) {}
                For d As Integer = 0 To rank - 1
                    all(d) = d
                Next
                Dim whole As Long() = ShapeInfoHelper.Create(shape, strides, order, extras)
                Return New TadPack(all, Array.Empty(Of Integer)(), whole, New Long() {0L}, ShapeInfoHelper.Length(shapeInfo))
            End If

            Dim inTad As Boolean() = New Boolean(rank - 1) {}
            For Each d As Integer In dims
                inTad(d) = True
            Next
            Dim tadShape As Long() = New Long(dims.Length - 1) {}
            Dim tadStrides As Long() = New Long(dims.Length - 1) {}
            For i As Integer = 0 To dims.Length - 1
                tadShape(i) = shape(dims(i))
                tadStrides(i) = strides(dims(i))
            Next
            Dim outer As New List(Of Integer)()
            For d As Integer = 0 To rank - 1
                If Not inTad(d) Then
                    outer.Add(d)
                End If
            Next
            Dim outerDims As Integer() = outer.ToArray()
            Dim numTads As Long = 1
            For Each d As Integer In outerDims
                numTads *= shape(d)
            Next
            Dim tadLength As Long = ShapeInfoHelper.ShapeLength(tadShape)

            ' offsets of the TADs in row-major order over the outer dimensions
            Dim offsets As Long() = New Long(CInt(numTads) - 1) {}
            If numTads > 0 Then
                Dim m As Integer = outerDims.Length
                Dim coords As Long() = New Long(m - 1) {}
                Dim offset As Long = 0
                For t As Integer = 0 To CInt(numTads) - 1
                    offsets(t) = offset
                    Dim k As Integer = m - 1
                    Do While k >= 0
                        Dim d As Integer = outerDims(k)
                        coords(k) += 1
                        offset += strides(d)
                        If coords(k) < shape(d) Then
                            Exit Do
                        End If
                        offset -= coords(k) * strides(d)
                        coords(k) = 0
                        k -= 1
                    Loop
                Next
            End If
            Dim tadInfo As Long() = ShapeInfoHelper.Create(tadShape, tadStrides, order, extras)
            Return New TadPack(dims, outerDims, tadInfo, offsets, tadLength)
        End Function

        ''' <summary>The shape of the outer dimensions (the shape of a reduction result without kept unit dimensions).</summary>
        Public Shared Function OuterShape(ByVal shapeInfo As Long(), ByVal ParamArray dimensions As Integer()) As Long()
            Dim rank As Integer = CInt(shapeInfo(0))
            Dim dims As Integer() = NormalizeDimensions(rank, dimensions)
            If dims.Length = 0 Then
                Return Array.Empty(Of Long)()
            End If
            Dim result As New List(Of Long)()
            For d As Integer = 0 To rank - 1
                If Array.IndexOf(dims, d) < 0 Then
                    result.Add(shapeInfo(1 + d))
                End If
            Next
            Return result.ToArray()
        End Function

    End Class

End Namespace
