Imports System
Imports System.Linq
Imports System.Text
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports ArrayOptionsHelper = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayType
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

Namespace org.nd4vb.linalg.api.shape

    ''' <summary>
    ''' A value description of an array's shape information: shape, strides, offset, element-wise stride, order and
    ''' the extras (data type and array type bits). Equality and hashing are by value.
    ''' <para>
    ''' Differences from Java: Java's <c>isEmpty()</c> is the method <see cref="isEmpty"/> (an <c>Empty</c> property
    ''' would clash with the static <see cref="empty"/>). <c>fromShape(shape, dataType)</c> and
    ''' <c>fromShape(shape, extras)</c> use the default order <c>Nd4vb.order()</c> and its dense strides
    ''' (<c>Nd4vb.getStrides</c>), as in Java.
    ''' </para>
    ''' </summary>
    Public Class LongShapeDescriptor

        Private order As Char
        Private offset As Long
        Private ews As Long
        Private hashShape As Long = 0
        Private hashStride As Long = 0
        Private shape() As Long
        Private stride() As Long
        Private extras As Long

        Public Sub New(ByVal shape() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal order As Char, ByVal extras As Long)
            Me.shape = CType(shape.Clone(), Long())
            Me.stride = CType(stride.Clone(), Long())
            Me.offset = offset
            Me.ews = ews
            Me.order = order
            Me.extras = extras
        End Sub

        Public Overridable Function getOrder() As Char
            Return order
        End Function

        Public Overridable Function getShape() As Long()
            Return shape
        End Function

        Public Overridable Function getStride() As Long()
            Return stride
        End Function

        Public Overridable Function getExtras() As Long
            Return extras
        End Function

        Public Overridable Sub setExtras(ByVal extras As Long)
            Me.extras = extras
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse CObj(Me).GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim that As LongShapeDescriptor = DirectCast(o, LongShapeDescriptor)

            If extras <> that.extras Then
                Return False
            End If
            If order <> that.order Then
                Return False
            End If
            If offset <> that.offset Then
                Return False
            End If
            If ews <> that.ews Then
                Return False
            End If
            If Not shape.SequenceEqual(that.shape) Then
                Return False
            End If
            Return stride.SequenceEqual(that.stride)
        End Function

        Public Overridable Function rank() As Integer
            Return If(shape Is Nothing, 0, shape.Length)
        End Function

        Public Overridable Function dataType() As DataType
            Return ArrayOptionsHelper.dataType(extras)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = AscW(order)

            result = 31 * result + longHashCode(offset)
            result = 31 * result + longHashCode(ews)
            result = 31 * result + longHashCode(extras)
            result = 31 * result + Arrays.hashCode(shape)
            result = 31 * result + Arrays.hashCode(stride)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Dim builder As New StringBuilder()

            builder.Append(shape.Length).Append(",").Append(Arrays.toString(shape)).Append(",").Append(Arrays.toString(stride)).Append(",").Append(extras).Append(",").Append(ews).Append(",").Append(order)

            Dim result As String = builder.ToString().Replace("]", "").Replace("[", "")
            result = "[" & result & "]"

            Return result
        End Function

        ''' <summary>Java's <c>Long.hashCode</c>: <c>(int)(v ^ (v &gt;&gt;&gt; 32))</c>.</summary>
        Private Shared Function longHashCode(ByVal v As Long) As Integer
            Return Arrays.WrapToInt32(v Xor (v >> 32))
        End Function

        Public Shared Function fromShapeDescriptor(ByVal descriptor As ShapeDescriptor) As LongShapeDescriptor
            If descriptor Is Nothing Then Throw New ArgumentNullException(NameOf(descriptor), "descriptor is marked non-null but is null")
            Return New LongShapeDescriptor(ArrayUtil.toLongArray(descriptor.getShape()), ArrayUtil.toLongArray(descriptor.getStride()), descriptor.getOffset(), descriptor.getEws(), descriptor.getOrder(), descriptor.getExtras())
        End Function

        Public Shared Function fromShape(ByVal shape() As Integer, ByVal dataType As DataType) As LongShapeDescriptor
            Return fromShape(ArrayUtil.toLongArray(shape), dataType)
        End Function

        ''' <summary>A descriptor with the default order (<c>Nd4vb.order()</c>) and its dense strides.</summary>
        Public Shared Function fromShape(ByVal shape() As Long, ByVal dataType As DataType) As LongShapeDescriptor
            Dim order As Char = Nd4vbT.order()
            Return fromShape(shape, Nd4vbT.getStrides(shape, order), 1, order, dataType, False)
        End Function

        ''' <summary>The descriptor of an empty array of the given data type.</summary>
        Public Shared Function empty(ByVal dataType As DataType) As LongShapeDescriptor
            Dim l(-1) As Long
            Return fromShape(l, l, 1, "c"c, dataType, True)
        End Function

        Public Shared Function fromShape(ByVal shape() As Long, ByVal strides() As Long, ByVal ews As Long, ByVal order As Char, ByVal dataType As DataType, ByVal empty As Boolean) As LongShapeDescriptor
            If shape Is Nothing Then Throw New ArgumentNullException(NameOf(shape), "shape is marked non-null but is null")
            If strides Is Nothing Then Throw New ArgumentNullException(NameOf(strides), "strides is marked non-null but is null")
            Dim extras As Long = 0L
            extras = ArrayOptionsHelper.setOptionBit(extras, dataType)
            If empty Then
                extras = ArrayOptionsHelper.setOptionBit(extras, ArrayType.EMPTY)
            End If

            Return New LongShapeDescriptor(shape, strides, 0, ews, order, extras)
        End Function

        ''' <summary>A descriptor with the default order (<c>Nd4vb.order()</c>), its dense strides and the given extras.</summary>
        Public Shared Function fromShape(ByVal shape() As Long, ByVal extras As Long) As LongShapeDescriptor
            Dim order As Char = Nd4vbT.order()
            Return New LongShapeDescriptor(shape, Nd4vbT.getStrides(shape, order), 0, 1, order, extras)
        End Function

        ''' <summary>
        ''' Return a new LongShapeDescriptor with the same shape, strides, order etc but with the specified datatype instead </summary>
        ''' <param name="dataType"> Datatype of the returned descriptor </param>
        Public Overridable Function asDataType(ByVal dataType As DataType) As LongShapeDescriptor
            Dim extras As Long = 0L
            extras = ArrayOptionsHelper.setOptionBit(extras, dataType)
            If isEmpty() Then
                extras = ArrayOptionsHelper.setOptionBit(extras, ArrayType.EMPTY)
            End If
            Return New LongShapeDescriptor(shape, stride, offset, ews, order, extras)
        End Function

        ''' <summary>True if the descriptor describes an empty array (Java <c>isEmpty()</c>).</summary>
        Public Overridable Function isEmpty() As Boolean
            Return ArrayOptionsHelper.hasBitSet(extras, ArrayOptionsHelper.ATYPE_EMPTY_BIT)
        End Function

    End Class

End Namespace
