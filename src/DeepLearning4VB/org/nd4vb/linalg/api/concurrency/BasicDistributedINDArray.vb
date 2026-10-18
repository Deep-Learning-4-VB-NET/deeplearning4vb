Imports System
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor

Namespace org.nd4vb.linalg.api.concurrency

    ''' <summary>
    ''' Base class of <see cref="DistributedINDArray"/> implementations: holds the array type.
    ''' </summary>
    Public MustInherit Class BasicDistributedINDArray
        Implements DistributedINDArray

        Private ReadOnly arrayTypeValue As ArrayType

        Public Sub New(ByVal arrayType As ArrayType)
            Me.arrayTypeValue = arrayType
        End Sub

        Public MustOverride Sub allocate(ByVal entry As Integer, ByVal dataType As DataType, ParamArray ByVal shape() As Long) Implements DistributedINDArray.allocate

        Public MustOverride Sub allocate(ByVal entry As Integer, ByVal shapeDescriptor As LongShapeDescriptor) Implements DistributedINDArray.allocate

        Public MustOverride Function numActiveEntries() As Integer Implements DistributedINDArray.numActiveEntries

        Public MustOverride Function numEntries() As Integer Implements DistributedINDArray.numEntries

        Public MustOverride Sub propagate(ByVal array As INDArray) Implements DistributedINDArray.propagate

        Public MustOverride Function entry() As INDArray Implements DistributedINDArray.entry

        Public MustOverride Function entry(ByVal index As Integer) As INDArray Implements DistributedINDArray.entry

        Public Overridable ReadOnly Property INDArrayType As ArrayType Implements DistributedINDArray.INDArrayType
            Get
                Return arrayTypeValue
            End Get
        End Property

    End Class

End Namespace
