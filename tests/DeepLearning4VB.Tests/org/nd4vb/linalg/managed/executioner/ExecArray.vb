Imports System
Imports System.Linq
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports INDArrayStub = DeepLearning4VB.org.nd4vb.linalg.cache.INDArrayStub
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports ShapeInfoHelper = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ShapeInfoHelper

Namespace org.nd4vb.linalg.managed

    ''' <summary>
    ''' A small test array for the executioner and BLAS tests (wave 3 has no <c>BaseNDArray</c> yet): a
    ''' <see cref="ManagedDataBuffer"/> whose element 0 is the array's first element (a view buffer for an offset, as in
    ''' ND4J 1.0.0-M2) and a shape information array. It implements the members the executioner, the BLAS classes and
    ''' the TAD manager read; everything else throws (<see cref="INDArrayStub"/>). Values are given and read in row-major
    ''' logical order whatever the layout.
    ''' </summary>
    Friend Class ExecArray
        Inherits INDArrayStub

        Private ReadOnly buffer As DataBuffer
        Private ReadOnly info As Long()

        ''' <summary>An array over <paramref name="buffer"/> with the given shape, strides and order.</summary>
        Friend Sub New(ByVal buffer As DataBuffer, ByVal shape As Long(), ByVal strides As Long(), ByVal order As Char)
            Me.buffer = buffer
            Dim ews As Long = If(shape.Length = 0, 1L, ShapeInfoHelper.ComputeElementWiseStride(shape, strides, order))
            Me.info = ShapeT.createShapeInfoArray(shape, strides, ews, order, buffer.dataType(), False)
        End Sub

        ''' <summary>A dense array of the given type, shape and order holding <paramref name="values"/> (row-major logical order).</summary>
        Friend Shared Function Create(ByVal dtype As DataType, ByVal shape As Long(), ByVal order As Char, ByVal ParamArray values As Double()) As ExecArray
            Dim n As Long = ShapeInfoHelper.ShapeLength(shape)
            Dim buf As New ManagedDataBuffer(dtype, Math.Max(n, 0L))
            Dim arr As New ExecArray(buf, shape, ShapeInfoHelper.CalculateStrides(shape, order), order)
            If values IsNot Nothing AndAlso values.Length > 0 Then
                If values.Length <> n Then
                    Throw New ArgumentException("expected " & n & " values, got " & values.Length)
                End If
                arr.SetValues(values)
            End If
            Return arr
        End Function

        ''' <summary>A dense 'c' array.</summary>
        Friend Shared Function C(ByVal dtype As DataType, ByVal shape As Long(), ByVal ParamArray values As Double()) As ExecArray
            Return Create(dtype, shape, "c"c, values)
        End Function

        ''' <summary>A dense 'f' array (values still in row-major logical order).</summary>
        Friend Shared Function F(ByVal dtype As DataType, ByVal shape As Long(), ByVal ParamArray values As Double()) As ExecArray
            Return Create(dtype, shape, "f"c, values)
        End Function

        ''' <summary>
        ''' A view: <paramref name="storageLength"/> elements of storage (filled with <paramref name="fill"/>), and an array
        ''' starting at element <paramref name="offset"/> with the given shape and strides, holding <paramref name="values"/>.
        ''' </summary>
        Friend Shared Function View(ByVal dtype As DataType, ByVal storageLength As Long, ByVal offset As Long, ByVal shape As Long(), ByVal strides As Long(), ByVal order As Char, ByVal fill As Double, ByVal ParamArray values As Double()) As ExecArray
            Dim root As New ManagedDataBuffer(dtype, storageLength)
            For i As Long = 0 To storageLength - 1
                root.put(i, fill)
            Next i
            Dim buf As DataBuffer = If(offset = 0, CType(root, DataBuffer), New ManagedDataBuffer(root, storageLength - offset, offset))
            Dim arr As New ExecArray(buf, shape, strides, order)
            If values IsNot Nothing AndAlso values.Length > 0 Then
                arr.SetValues(values)
            End If
            Return arr
        End Function

        ''' <summary>The element offset (in the buffer) of the element with row-major logical index i.</summary>
        Private Function offsetOf(ByVal i As Long) As Long
            Return ShapeInfoHelper.OffsetOf(info, i)
        End Function

        ''' <summary>Writes the values in row-major logical order.</summary>
        Friend Sub SetValues(ByVal values As Double())
            For i As Integer = 0 To values.Length - 1
                buffer.put(offsetOf(i), values(i))
            Next i
        End Sub

        ''' <summary>The values in row-major logical order.</summary>
        Friend Function Values() As Double()
            Dim n As Long = length()
            Dim result(CInt(n) - 1) As Double
            For i As Integer = 0 To CInt(n) - 1
                result(i) = buffer.getDouble(offsetOf(i))
            Next i
            Return result
        End Function

        Public Overrides Function data() As DataBuffer
            Return buffer
        End Function

        Public Overrides Function shapeInfoJava() As Long()
            Return info
        End Function

        Public Overrides Function shapeInfoDataBuffer() As DataBuffer
            Return New ManagedDataBuffer(DataType.LONG, info)
        End Function

        Public Overrides Function shapeInfoToString() As String
            Return "Rank: " & rank() & ", Shape: [" & String.Join(", ", shape()) & "], Stride: [" & String.Join(", ", stride()) & "], Order: " & ordering()
        End Function

        Public Overrides Function offset() As Long
            Return buffer.offset()
        End Function

        Public Overrides Function dataType() As DataType
            Return buffer.dataType()
        End Function

        Public Overrides Function length() As Long
            Return ShapeInfoHelper.Length(info)
        End Function

        Public Overrides Function rank() As Integer
            Return CInt(info(0))
        End Function

        Public Overrides Function shape() As Long()
            Return ShapeInfoHelper.GetShape(info)
        End Function

        Public Overrides Function stride() As Long()
            Return ShapeInfoHelper.GetStrides(info)
        End Function

        Public Overrides Function stride(ByVal dimension As Integer) As Integer
            Dim r As Integer = rank()
            Return CInt(info(1 + r + If(dimension < 0, dimension + r, dimension)))
        End Function

        Public Overrides Function size(ByVal dimension As Integer) As Long
            Dim r As Integer = rank()
            Return info(1 + If(dimension < 0, dimension + r, dimension))
        End Function

        Public Overrides Function ordering() As Char
            Return ShapeInfoHelper.GetOrder(info)
        End Function

        Public Overrides Function elementWiseStride() As Integer
            Return CInt(ShapeInfoHelper.ElementWiseStride(info))
        End Function

        Public Overrides Function rows() As Integer
            Select Case rank()
                Case 2
                    Return CInt(size(0))
                Case Else
                    Return 1
            End Select
        End Function

        Public Overrides Function columns() As Integer
            Select Case rank()
                Case 2
                    Return CInt(size(1))
                Case 1
                    Return CInt(size(0))
                Case Else
                    Return 1
            End Select
        End Function

        Public Overrides ReadOnly Property View As Boolean
            Get
                Return buffer.offset() <> 0 OrElse Not ShapeInfoHelper.IsContiguous(info)
            End Get
        End Property

        Public Overrides ReadOnly Property Empty As Boolean
            Get
                Return length() = 0
            End Get
        End Property

        Public Overrides ReadOnly Property Scalar As Boolean
            Get
                Return Not Empty AndAlso (rank() = 0 OrElse (rank() <= 2 AndAlso length() = 1))
            End Get
        End Property

        Public Overrides ReadOnly Property Vector As Boolean
            Get
                Return rank() = 1 OrElse (rank() = 2 AndAlso (size(0) = 1 OrElse size(1) = 1) AndAlso length() > 1)
            End Get
        End Property

        Public Overrides ReadOnly Property Matrix As Boolean
            Get
                Return rank() = 2
            End Get
        End Property

        Public Overrides ReadOnly Property ColumnVector As Boolean
            Get
                Return rank() = 2 AndAlso size(1) = 1
            End Get
        End Property

        Public Overrides ReadOnly Property RowVector As Boolean
            Get
                Return rank() = 1 OrElse (rank() = 2 AndAlso size(0) = 1)
            End Get
        End Property

        Public Overrides ReadOnly Property R As Boolean
            Get
                Return dataType().isFPType()
            End Get
        End Property

        Public Overrides ReadOnly Property Z As Boolean
            Get
                Return dataType().isIntType()
            End Get
        End Property

        Public Overrides ReadOnly Property B As Boolean
            Get
                Return dataType() = DataType.BOOL
            End Get
        End Property

        Public Overrides ReadOnly Property Compressed As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property Attached As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Function wasClosed() As Boolean
            Return False
        End Function

        Public Overrides Function toIntVector() As Integer()
            Return Values().Select(Function(v) CInt(v)).ToArray()
        End Function

        Public Overrides Function toLongVector() As Long()
            Return Values().Select(Function(v) CLng(v)).ToArray()
        End Function

        Public Overrides Function getDouble(ByVal i As Long) As Double
            Return buffer.getDouble(offsetOf(i))
        End Function

        Public Overrides Function getFloat(ByVal i As Long) As Single
            Return CSng(getDouble(i))
        End Function

        Public Overrides Function getDouble(ByVal i As Long, ByVal j As Long) As Double
            Return buffer.getDouble(ShapeInfoHelper.OffsetOf(info, New Long() {i, j}))
        End Function

        Public Overrides Function getFloat(ByVal i As Long, ByVal j As Long) As Single
            Return CSng(getDouble(i, j))
        End Function

        Public Overrides Function getInt(ParamArray ByVal indices() As Integer) As Integer
            If indices.Length = 1 Then
                Return CInt(getDouble(CLng(indices(0))))
            End If
            Return CInt(buffer.getDouble(ShapeInfoHelper.OffsetOf(info, indices.Select(Function(v) CLng(v)).ToArray())))
        End Function

        Public Overrides Function equalShapes(ByVal other As INDArray) As Boolean
            Return shape().SequenceEqual(other.shape())
        End Function

        ''' <summary>A dense copy in the given order.</summary>
        Public Overrides Function dup(ByVal order As Char) As INDArray
            Return Create(dataType(), shape(), order, Values())
        End Function

        Public Overrides Function dup() As INDArray
            Return dup(ordering())
        End Function

        ''' <summary>A zero-filled array of the same type, shape and order (stands in for "uninitialized").</summary>
        Public Overrides Function ulike() As INDArray
            Return Create(dataType(), shape(), ordering())
        End Function

        Public Overrides Function castTo(ByVal dataType As DataType) As INDArray
            Return Create(dataType, shape(), ordering(), Values())
        End Function

        Public Overrides Function assign(ByVal arr As INDArray) As INDArray
            Dim other As ExecArray = DirectCast(arr, ExecArray)
            SetValues(other.Values())
            Return Me
        End Function

        ''' <summary>The transposed view (reversed shape and strides, the other order flag).</summary>
        Public Overrides Function transpose() As INDArray
            Return New ExecArray(buffer, shape().Reverse().ToArray(), stride().Reverse().ToArray(), If(ordering() = "c"c, "f"c, "c"c))
        End Function

    End Class

End Namespace
