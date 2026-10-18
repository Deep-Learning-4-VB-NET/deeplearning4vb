Imports System
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

Namespace org.nd4vb.linalg.cache

    ''' <summary>
    ''' A hashable key for a tensor-along-dimension request: the dimensions and the shape information of the array.
    ''' Lombok's <c>@Data</c> members are written out; <c>Shape.shapeInfoLength(rank)</c> (Shape is ported in a later
    ''' wave) is computed inline as <c>rank * 2 + 4</c>.
    ''' </summary>
    Public Class TadDescriptor

        Private dimensionLength As Integer
        Private dimension() As Integer
        Private shape() As Long

        ''' <summary>
        ''' Pass in an ndarray to get the databuffer
        ''' and the appropriate dimensions </summary>
        ''' <param name="array"> the array to pass in
        '''              to get the shape info from </param>
        ''' <param name="dimension"> the dimensions for the TAD </param>
        Public Sub New(ByVal array As INDArray, ByVal dimension() As Integer)
            Me.dimensionLength = If(dimension Is Nothing, 0, dimension.Length)
            Me.dimension = dimension

            ' TODO: change this to fill shapeInfo
            Me.shape = dataBufferToArray(array.shapeInfoDataBuffer())
        End Sub

        ''' <summary>
        ''' Obtain the values from the shape buffer
        ''' for the array </summary>
        ''' <param name="buffer"> the buffer to get the values from </param>
        ''' <returns> the int array version of this data buffer </returns>
        Public Shared Function dataBufferToArray(ByVal buffer As DataBuffer) As Long()
            Dim rank As Integer = buffer.getInt(0)
            Dim length As Integer = rank * 2 + 4
            Dim ret(length - 1) As Long
            ret(0) = rank
            For e As Integer = 1 To length - 1
                ret(e) = buffer.getInt(e)
            Next e

            Return ret
        End Function

        Public Overridable Function getDimensionLength() As Integer
            Return dimensionLength
        End Function

        Public Overridable Sub setDimensionLength(ByVal dimensionLength As Integer)
            Me.dimensionLength = dimensionLength
        End Sub

        Public Overridable Function getDimension() As Integer()
            Return dimension
        End Function

        Public Overridable Sub setDimension(ByVal dimension() As Integer)
            Me.dimension = dimension
        End Sub

        Public Overridable Function getShape() As Long()
            Return shape
        End Function

        Public Overridable Sub setShape(ByVal shape() As Long)
            Me.shape = shape
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As TadDescriptor = TryCast(obj, TadDescriptor)
            If o Is Nothing Then Return False
            Return dimensionLength = o.dimensionLength AndAlso Arrays.equals(dimension, o.dimension) AndAlso Arrays.equals(shape, o.shape)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + dimensionLength
            h = h * 59 + Arrays.hashCode(dimension)
            h = h * 59 + Arrays.hashCode(shape)
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "TadDescriptor(dimensionLength=" & dimensionLength & ", dimension=" & Arrays.toString(dimension) & ", shape=" & Arrays.toString(shape) & ")"
        End Function

    End Class

End Namespace
