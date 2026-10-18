Imports System
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape

Namespace org.nd4vb.linalg.api.ndarray

    ''' <summary>
    ''' The shape information of an array decoded into fields: shape, stride, length, element-wise stride, extras,
    ''' order and rank, read with the <c>Shape</c> helpers. The layout is
    ''' <c>[rank, shape..., stride..., extras, elementWiseStride, order]</c>; the length of an empty array is 0.
    ''' </summary>
    Public Class JvmShapeInfo

        Protected Friend ReadOnly javaShapeInformation() As Long
        Protected Friend ReadOnly shape() As Long
        Protected Friend ReadOnly stride() As Long
        Protected Friend ReadOnly length As Long
        Protected Friend ReadOnly ews As Long
        Protected Friend ReadOnly extras As Long
        Protected Friend ReadOnly order As Char
        Protected Friend ReadOnly rank As Integer

        Public Sub New(ByVal javaShapeInformation() As Long)
            If javaShapeInformation Is Nothing Then Throw New ArgumentNullException(NameOf(javaShapeInformation), "javaShapeInformation is marked non-null but is null")
            Me.javaShapeInformation = javaShapeInformation
            Me.shape = ShapeT.shape(javaShapeInformation)
            Me.stride = ShapeT.stride(javaShapeInformation)
            Me.length = If(ShapeT.isEmpty(javaShapeInformation), 0, ShapeT.length(javaShapeInformation))
            Me.ews = ShapeT.elementWiseStride(javaShapeInformation)
            Me.extras = ShapeT.extras(javaShapeInformation)
            Me.order = ShapeT.order(javaShapeInformation)
            Me.rank = ShapeT.rank(javaShapeInformation)
        End Sub

        Public Overridable Function getJavaShapeInformation() As Long()
            Return javaShapeInformation
        End Function

        Public Overridable Function getShape() As Long()
            Return shape
        End Function

        Public Overridable Function getStride() As Long()
            Return stride
        End Function

        Public Overridable Function getLength() As Long
            Return length
        End Function

        Public Overridable Function getEws() As Long
            Return ews
        End Function

        Public Overridable Function getExtras() As Long
            Return extras
        End Function

        Public Overridable Function getOrder() As Char
            Return order
        End Function

        Public Overridable Function getRank() As Integer
            Return rank
        End Function

    End Class

End Namespace
