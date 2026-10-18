Imports System
Imports ArrayOptionsHelper = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayType

Namespace org.nd4vb.linalg.api.ndarray

    ''' <summary>
    ''' The shape information of an array decoded into fields: shape, stride, length, element-wise stride, extras,
    ''' order and rank. The layout is <c>[rank, shape..., stride..., extras, elementWiseStride, order]</c>.
    ''' <para>
    ''' Java reads the fields with the <c>Shape</c> helpers (<c>Shape.shape</c>, <c>Shape.stride</c>, ...); <c>Shape</c>
    ''' is ported in a later wave, so the private helpers of this class do the same reads (candidates for replacement
    ''' by the <c>Shape</c> calls).
    ''' </para>
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
            Me.shape = shapeOf(javaShapeInformation)
            Me.stride = strideOf(javaShapeInformation)
            Me.length = If(isEmptyShape(javaShapeInformation), 0, lengthOf(javaShapeInformation))
            Me.ews = javaShapeInformation(shapeInfoLength(javaShapeInformation) - 2)
            Me.extras = javaShapeInformation(shapeInfoLength(javaShapeInformation) - 3)
            Me.order = ChrW(CInt(javaShapeInformation(shapeInfoLength(javaShapeInformation) - 1)))
            Me.rank = CInt(javaShapeInformation(0))
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

        ''' <summary>Java <c>Shape.shapeInfoLength(long[])</c>.</summary>
        Private Shared Function shapeInfoLength(ByVal buffer() As Long) As Integer
            Return CInt(buffer(0)) * 2 + 4
        End Function

        ''' <summary>Java <c>Shape.shape(long[])</c>.</summary>
        Private Shared Function shapeOf(ByVal buffer() As Long) As Long()
            Dim ret(CInt(buffer(0)) - 1) As Long
            Array.Copy(buffer, 1, ret, 0, ret.Length)
            Return ret
        End Function

        ''' <summary>Java <c>Shape.stride(long[])</c>.</summary>
        Private Shared Function strideOf(ByVal buffer() As Long) As Long()
            Dim r As Integer = CInt(buffer(0))
            Dim ret(r - 1) As Long
            Array.Copy(buffer, 1 + r, ret, 0, r)
            Return ret
        End Function

        ''' <summary>Java <c>Shape.length(long[])</c>: the product of the shape.</summary>
        Private Shared Function lengthOf(ByVal buffer() As Long) As Long
            Dim ret As Long = 1
            Dim limit As Integer = CInt(buffer(0)) + 1
            For i As Integer = 1 To limit - 1
                ret *= buffer(i)
            Next i
            Return ret
        End Function

        ''' <summary>Java <c>Shape.isEmpty(long[])</c>.</summary>
        Private Shared Function isEmptyShape(ByVal buffer() As Long) As Boolean
            Return ArrayOptionsHelper.arrayType(buffer) = ArrayType.EMPTY
        End Function

    End Class

End Namespace
