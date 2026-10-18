Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil

Namespace org.nd4vb.linalg.api.iter

    ''' <summary>
    ''' Private copies of the <c>Shape.ind2sub</c>/<c>Shape.ind2subC</c> conversions (linear index to subscripts, in
    ''' Fortran and C order) used by the iterators of this package. <c>Shape</c> is ported in a later wave; these
    ''' helpers should then be replaced by the <c>Shape</c> calls.
    ''' </summary>
    Friend Module IterShapeHelper

        ''' <summary>Java <c>Shape.ind2sub(long[] shape, long index, long numIndices)</c> (Fortran order).</summary>
        Public Function ind2sub(ByVal shape() As Long, ByVal index As Long, ByVal numIndices As Long) As Long()
            Dim denom As Long = numIndices
            Dim ret(shape.Length - 1) As Long
            For i As Integer = ret.Length - 1 To 0 Step -1
                denom \= shape(i)
                If index \ denom >= Integer.MaxValue Then
                    Throw New ArgumentException("Dimension can not be >= Integer.MAX_VALUE")
                End If
                ret(i) = index \ denom
                index = index Mod denom
            Next i
            Return ret
        End Function

        ''' <summary>Java <c>Shape.ind2sub(long[] shape, long index)</c>.</summary>
        Public Function ind2sub(ByVal shape() As Long, ByVal index As Long) As Long()
            Return ind2sub(shape, index, ArrayUtil.prodLong(shape))
        End Function

        ''' <summary>Java <c>Shape.ind2sub(INDArray arr, long index)</c>.</summary>
        Public Function ind2sub(ByVal arr As INDArray, ByVal index As Long) As Long()
            If arr.rank() = 1 Then
                Return New Long() {index}
            End If
            Return ind2sub(arr.shape(), index, ArrayUtil.prodLong(arr.shape()))
        End Function

        ''' <summary>Java <c>Shape.ind2subC(long[] shape, long index, long numIndices)</c> (C order).</summary>
        Public Function ind2subC(ByVal shape() As Long, ByVal index As Long, ByVal numIndices As Long) As Long()
            Dim denom As Long = numIndices
            Dim ret(shape.Length - 1) As Long
            For i As Integer = 0 To shape.Length - 1
                denom \= shape(i)
                If index \ denom >= Integer.MaxValue Then
                    Throw New ArgumentException("Dimension can not be >= Integer.MAX_VALUE")
                End If
                ret(i) = index \ denom
                index = index Mod denom
            Next i
            Return ret
        End Function

        ''' <summary>Java <c>Shape.ind2subC(long[] shape, long index)</c>.</summary>
        Public Function ind2subC(ByVal shape() As Long, ByVal index As Long) As Long()
            Return ind2subC(shape, index, ArrayUtil.prodLong(shape))
        End Function

        ''' <summary>Java <c>Shape.ind2subC(INDArray arr, long index)</c>.</summary>
        Public Function ind2subC(ByVal arr As INDArray, ByVal index As Long) As Long()
            If arr.rank() = 1 Then
                Return New Long() {index}
            End If
            Return ind2subC(arr.shape(), index, ArrayUtil.prodLong(arr.shape()))
        End Function

    End Module

End Namespace
