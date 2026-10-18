Imports System
Imports System.Linq
Imports System.Text

Namespace org.nd4vb.linalg.api.shape

    ''' <summary>
    ''' The <c>Integer</c> version of <see cref="LongShapeDescriptor"/>: shape, strides, offset, element-wise stride,
    ''' order and extras, compared and hashed by value.
    ''' </summary>
    Public Class ShapeDescriptor

        Private order As Char
        Private offset As Long
        Private ews As Integer
        Private hashShape As Long = 0
        Private hashStride As Long = 0
        Private shape() As Integer
        Private stride() As Integer
        Private extras As Long

        Public Sub New(ByVal shape() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ews As Integer, ByVal order As Char, ByVal extras As Long)
            Me.shape = CType(shape.Clone(), Integer())
            Me.stride = CType(stride.Clone(), Integer())
            Me.offset = offset
            Me.ews = ews
            Me.order = order
            Me.extras = extras
        End Sub

        Public Overridable Function getOrder() As Char
            Return order
        End Function

        Public Overridable Function getOffset() As Long
            Return offset
        End Function

        Public Overridable Function getEws() As Integer
            Return ews
        End Function

        Public Overridable Function getShape() As Integer()
            Return shape
        End Function

        Public Overridable Function getStride() As Integer()
            Return stride
        End Function

        Public Overridable Function getExtras() As Long
            Return extras
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse CObj(Me).GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim that As ShapeDescriptor = DirectCast(o, ShapeDescriptor)

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

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = AscW(order)

            result = 31 * result + longHashCode(offset)
            result = 31 * result + longHashCode(extras)
            result = 31 * result + ews
            result = 31 * result + Arrays.hashCode(shape)
            result = 31 * result + Arrays.hashCode(stride)
            Return result
        End Function

        ''' <summary>Java's <c>Long.hashCode</c>: <c>(int)(v ^ (v &gt;&gt;&gt; 32))</c>.</summary>
        Private Shared Function longHashCode(ByVal v As Long) As Integer
            Return Arrays.WrapToInt32(v Xor (v >> 32))
        End Function

        Public Overrides Function ToString() As String
            Dim builder As New StringBuilder()

            builder.Append(shape.Length).Append(",").Append(Arrays.toString(shape)).Append(",").Append(Arrays.toString(stride)).Append(",").Append(offset).Append(",").Append(ews).Append(",").Append(order)

            Dim result As String = builder.ToString().Replace("]", "").Replace("[", "")
            result = "[" & result & "]"

            Return result
        End Function

    End Class

End Namespace
