' DeepLearning4VB: reading and building ND4J shape-info arrays for the managed kernels.

Imports System

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Helpers for ND4J's shape information <c>Long()</c>:
    ''' <c>[rank, shape(0..rank-1), stride(0..rank-1), extras, elementWiseStride, order]</c>, of length
    ''' <c>2 * rank + 4</c>. Strides are in elements, <c>order</c> is <c>AscW("c"c)</c> or <c>AscW("f"c)</c>,
    ''' and rank 0 is a scalar (<c>[0, extras, 1, order]</c>). <c>extras</c> carries ND4J's data-type
    ''' flags; the kernels copy it but never interpret it.
    ''' </summary>
    ''' <remarks>
    ''' Every kernel matches elements of different arrays by their <b>row-major logical index</b>: element
    ''' <c>i</c> is the one whose coordinates are <c>i</c> written in mixed radix over the shape, last
    ''' dimension fastest, whatever the strides and the order flag. <see cref="OffsetOf(Long(), Long)"/> gives
    ''' its element offset relative to the array's first element.
    ''' </remarks>
    Public NotInheritable Class ShapeInfoHelper

        Private Sub New()
        End Sub

        ''' <summary>The order value of a C-ordered (row-major) array: <c>AscW("c"c)</c>.</summary>
        Public Const OrderC As Long = 99L

        ''' <summary>The order value of an F-ordered (column-major) array: <c>AscW("f"c)</c>.</summary>
        Public Const OrderF As Long = 102L

        ''' <summary>The rank (number of dimensions).</summary>
        Public Shared Function Rank(ByVal info As Long()) As Integer
            Return CInt(info(0))
        End Function

        ''' <summary>Length of a shape-info array for a rank: <c>2 * rank + 4</c>.</summary>
        Public Shared Function InfoLength(ByVal rank As Integer) As Integer
            Return 2 * rank + 4
        End Function

        ''' <summary>Size of one dimension.</summary>
        Public Shared Function Size(ByVal info As Long(), ByVal dimension As Integer) As Long
            Return info(1 + dimension)
        End Function

        ''' <summary>Stride of one dimension, in elements.</summary>
        Public Shared Function Stride(ByVal info As Long(), ByVal dimension As Integer) As Long
            Return info(1 + CInt(info(0)) + dimension)
        End Function

        ''' <summary>A copy of the shape.</summary>
        Public Shared Function GetShape(ByVal info As Long()) As Long()
            Dim r As Integer = CInt(info(0))
            Dim result As Long() = New Long(r - 1) {}
            Array.Copy(info, 1, result, 0, r)
            Return result
        End Function

        ''' <summary>A copy of the strides.</summary>
        Public Shared Function GetStrides(ByVal info As Long()) As Long()
            Dim r As Integer = CInt(info(0))
            Dim result As Long() = New Long(r - 1) {}
            Array.Copy(info, 1 + r, result, 0, r)
            Return result
        End Function

        ''' <summary>The extras word (ND4J's data-type and property flags).</summary>
        Public Shared Function Extras(ByVal info As Long()) As Long
            Return info(InfoLength(CInt(info(0))) - 3)
        End Function

        ''' <summary>The recorded element-wise stride (0 when the elements are not equally spaced in the array's order).</summary>
        Public Shared Function ElementWiseStride(ByVal info As Long()) As Long
            Return info(InfoLength(CInt(info(0))) - 2)
        End Function

        ''' <summary>The order flag: "c"c or "f"c.</summary>
        Public Shared Function GetOrder(ByVal info As Long()) As Char
            Return ChrW(CInt(info(InfoLength(CInt(info(0))) - 1)))
        End Function

        ''' <summary>Number of elements (1 for a scalar, 0 when any dimension is 0).</summary>
        Public Shared Function Length(ByVal info As Long()) As Long
            Dim r As Integer = CInt(info(0))
            Dim n As Long = 1
            For d As Integer = 1 To r
                n *= info(d)
            Next
            Return n
        End Function

        ''' <summary>Number of elements of a shape.</summary>
        Public Shared Function ShapeLength(ByVal shape As Long()) As Long
            Dim n As Long = 1
            For Each s As Long In shape
                n *= s
            Next
            Return n
        End Function

        ''' <summary>Dense strides for a shape in "c"c (row-major) or "f"c (column-major) order.</summary>
        Public Shared Function CalculateStrides(ByVal shape As Long(), ByVal order As Char) As Long()
            Dim r As Integer = shape.Length
            Dim strides As Long() = New Long(r - 1) {}
            Dim s As Long = 1
            If IsFOrder(order) Then
                For d As Integer = 0 To r - 1
                    strides(d) = s
                    s *= Math.Max(1L, shape(d))
                Next
            Else
                For d As Integer = r - 1 To 0 Step -1
                    strides(d) = s
                    s *= Math.Max(1L, shape(d))
                Next
            End If
            Return strides
        End Function

        ''' <summary>A shape info for a dense array of the given shape and order ("c"c or "f"c).</summary>
        Public Shared Function Create(ByVal shape As Long(), ByVal order As Char, Optional ByVal extras As Long = 0L) As Long()
            Return Create(shape, CalculateStrides(shape, order), order, extras)
        End Function

        ''' <summary>A shape info for the given shape, strides and order; the element-wise stride is computed.</summary>
        Public Shared Function Create(ByVal shape As Long(), ByVal strides As Long(), ByVal order As Char, Optional ByVal extras As Long = 0L) As Long()
            If shape Is Nothing Then
                Throw New ArgumentNullException(NameOf(shape))
            End If
            If strides Is Nothing OrElse strides.Length <> shape.Length Then
                Throw New ArgumentException("Shape and strides must have the same rank", NameOf(strides))
            End If
            Dim r As Integer = shape.Length
            Dim info As Long() = New Long(InfoLength(r) - 1) {}
            info(0) = r
            Array.Copy(shape, 0, info, 1, r)
            Array.Copy(strides, 0, info, 1 + r, r)
            info(2 * r + 1) = extras
            info(2 * r + 2) = ComputeElementWiseStride(shape, strides, order)
            info(2 * r + 3) = If(IsFOrder(order), OrderF, OrderC)
            Return info
        End Function

        ''' <summary>
        ''' The element-wise stride ND4J records: the spacing <c>s</c> such that the i-th element in the
        ''' given order is at offset <c>i * s</c>, or 0 when there is none. Dimensions of size 1 are
        ''' ignored; a scalar or single element gives 1.
        ''' </summary>
        Public Shared Function ComputeElementWiseStride(ByVal shape As Long(), ByVal strides As Long(), ByVal order As Char) As Long
            Dim r As Integer = shape.Length
            Dim prevDim As Integer = -1
            Dim result As Long = 1
            If IsFOrder(order) Then
                For d As Integer = 0 To r - 1
                    If shape(d) = 1 Then
                        Continue For
                    End If
                    If prevDim < 0 Then
                        result = strides(d)
                    ElseIf strides(d) <> strides(prevDim) * shape(prevDim) Then
                        Return 0
                    End If
                    prevDim = d
                Next
            Else
                For d As Integer = r - 1 To 0 Step -1
                    If shape(d) = 1 Then
                        Continue For
                    End If
                    If prevDim < 0 Then
                        result = strides(d)
                    ElseIf strides(d) <> strides(prevDim) * shape(prevDim) Then
                        Return 0
                    End If
                    prevDim = d
                Next
            End If
            Return result
        End Function

        ''' <summary>
        ''' The spacing <c>s</c> such that the element with row-major logical index <c>i</c> is at offset
        ''' <c>i * s</c>, or 0 when there is none (1 for C-contiguous arrays and scalars).
        ''' </summary>
        Public Shared Function LinearStride(ByVal info As Long()) As Long
            Return ComputeElementWiseStride(GetShape(info), GetStrides(info), "c"c)
        End Function

        ''' <summary>True when the elements fill one dense block, in C or F layout (dimensions of size 1 ignored).</summary>
        Public Shared Function IsContiguous(ByVal info As Long()) As Boolean
            Dim shape As Long() = GetShape(info)
            Dim strides As Long() = GetStrides(info)
            Return ComputeElementWiseStride(shape, strides, "c"c) = 1 OrElse ComputeElementWiseStride(shape, strides, "f"c) = 1
        End Function

        ''' <summary>True when the elements fill one dense block in the given order ("c"c or "f"c).</summary>
        Public Shared Function IsContiguous(ByVal info As Long(), ByVal order As Char) As Boolean
            Return ComputeElementWiseStride(GetShape(info), GetStrides(info), order) = 1
        End Function

        ''' <summary>Row-major coordinates of the element with logical index <paramref name="index"/>.</summary>
        Public Shared Function IndexToCoordinates(ByVal shape As Long(), ByVal index As Long) As Long()
            Dim r As Integer = shape.Length
            Dim coords As Long() = New Long(r - 1) {}
            Dim rest As Long = index
            For d As Integer = r - 1 To 0 Step -1
                If shape(d) > 0 Then
                    coords(d) = rest Mod shape(d)
                    rest \= shape(d)
                End If
            Next
            Return coords
        End Function

        ''' <summary>Element offset (relative to the array's first element) of the element with row-major logical index <paramref name="index"/>.</summary>
        Public Shared Function OffsetOf(ByVal info As Long(), ByVal index As Long) As Long
            Dim r As Integer = CInt(info(0))
            Dim offset As Long = 0
            Dim rest As Long = index
            For d As Integer = r - 1 To 0 Step -1
                Dim s As Long = info(1 + d)
                If s > 0 Then
                    offset += (rest Mod s) * info(1 + r + d)
                    rest \= s
                End If
            Next
            Return offset
        End Function

        ''' <summary>Element offset (relative to the array's first element) of the element at the given coordinates.</summary>
        Public Shared Function OffsetOf(ByVal info As Long(), ByVal coordinates As Long()) As Long
            Dim r As Integer = CInt(info(0))
            If coordinates.Length <> r Then
                Throw New ArgumentException("Expected " & r & " coordinates, got " & coordinates.Length, NameOf(coordinates))
            End If
            Dim offset As Long = 0
            For d As Integer = 0 To r - 1
                offset += coordinates(d) * info(1 + r + d)
            Next
            Return offset
        End Function

        ''' <summary>The element offsets of all elements in row-major logical order (relative to the array's first element).</summary>
        Public Shared Function Offsets(ByVal info As Long()) As Long()
            Dim n As Long = Length(info)
            Dim result As Long() = New Long(CInt(n) - 1) {}
            If n = 0 Then
                Return result
            End If
            Dim r As Integer = CInt(info(0))
            If r = 0 Then
                result(0) = 0
                Return result
            End If
            Dim coords As Long() = New Long(r - 1) {}
            Dim offset As Long = 0
            For i As Integer = 0 To CInt(n) - 1
                result(i) = offset
                ' advance the odometer, last dimension fastest
                Dim d As Integer = r - 1
                Do While d >= 0
                    coords(d) += 1
                    offset += info(1 + r + d)
                    If coords(d) < info(1 + d) Then
                        Exit Do
                    End If
                    offset -= coords(d) * info(1 + r + d)
                    coords(d) = 0
                    d -= 1
                Loop
            Next
            Return result
        End Function

        ''' <summary>True when the shapes are equal once dimensions of size 1 are removed.</summary>
        Public Shared Function SqueezedShapesEqual(ByVal a As Long(), ByVal b As Long()) As Boolean
            Dim i As Integer = 0
            Dim j As Integer = 0
            Do
                Do While i < a.Length AndAlso a(i) = 1
                    i += 1
                Loop
                Do While j < b.Length AndAlso b(j) = 1
                    j += 1
                Loop
                If i >= a.Length OrElse j >= b.Length Then
                    Return i >= a.Length AndAlso j >= b.Length
                End If
                If a(i) <> b(j) Then
                    Return False
                End If
                i += 1
                j += 1
            Loop
        End Function

        ''' <summary>True for "f"c or "F"c.</summary>
        Friend Shared Function IsFOrder(ByVal order As Char) As Boolean
            Return order = "f"c OrElse order = "F"c
        End Function

        ''' <summary>Throws when <paramref name="info"/> is not a well-formed shape info.</summary>
        Friend Shared Sub Validate(ByVal info As Long(), ByVal name As String)
            If info Is Nothing Then
                Throw New ArgumentNullException(name)
            End If
            If info.Length < 4 OrElse info(0) < 0 OrElse info.Length < InfoLength(CInt(info(0))) Then
                Throw New ArgumentException("Malformed shape info", name)
            End If
        End Sub

    End Class

End Namespace
