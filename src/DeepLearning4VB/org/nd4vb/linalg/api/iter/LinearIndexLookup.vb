Imports System
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape

Namespace org.nd4vb.linalg.api.iter

    ''' <summary>
    ''' Caches the conversion of linear indexes to subscripts for one shape and order. The subscripts are computed
    ''' lazily by <c>Shape.ind2sub</c>/<c>Shape.ind2subC</c>.
    ''' </summary>
    <Serializable>
    Public Class LinearIndexLookup

        Private ordering As Char
        Private indexes()() As Long
        Private shape() As Long
        Private exists() As Boolean
        Private numIndexes As Long

        ''' <param name="shape">    the shape of the linear index </param>
        ''' <param name="ordering"> the ordering of the linear index </param>
        Public Sub New(ByVal shape() As Integer, ByVal ordering As Char)
            Me.New(ArrayUtil.toLongArray(shape), ordering)
        End Sub

        Public Sub New(ByVal shape() As Long, ByVal ordering As Char)
            Me.shape = shape
            Me.ordering = ordering
            numIndexes = ArrayUtil.prodLong(shape)

            ' FIXME: long!
            indexes = New Long(CInt(numIndexes) - 1)() {}
            exists = New Boolean(CInt(numIndexes) - 1) {}
        End Sub

        ''' <summary>
        ''' Give back a sub
        ''' wrt the given linear index </summary>
        ''' <param name="index"> the index </param>
        ''' <returns> the sub for the given index </returns>
        Public Overridable Function lookup(ByVal index As Integer) As Long()
            If exists(index) Then
                Return indexes(index)
            Else
                exists(index) = True
                indexes(index) = If(ordering = "c"c, ShapeT.ind2subC(shape, index, numIndexes), ShapeT.ind2sub(shape, index, numIndexes))
                Return indexes(index)
            End If
        End Function

    End Class

End Namespace
