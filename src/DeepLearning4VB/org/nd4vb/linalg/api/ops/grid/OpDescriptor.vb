Imports System
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op

Namespace org.nd4vb.linalg.api.ops.grid

    ''' <summary>
    ''' An op and the dimensions it runs along (Java <c>OpDescriptor</c>; Lombok's <c>@Data @NoArgsConstructor
    ''' @AllArgsConstructor</c> members are written out).
    ''' </summary>
    Public Class OpDescriptor

        Private op As Op
        Private dimensions() As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal op As Op)
            Me.New(op, Nothing)
        End Sub

        Public Sub New(ByVal op As Op, ByVal dimensions() As Integer)
            Me.op = op
            Me.dimensions = dimensions
        End Sub

        Public Overridable Function getOp() As Op
            Return op
        End Function

        Public Overridable Sub setOp(ByVal op As Op)
            Me.op = op
        End Sub

        Public Overridable Function getDimensions() As Integer()
            Return dimensions
        End Function

        Public Overridable Sub setDimensions(ByVal dimensions() As Integer)
            Me.dimensions = dimensions
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As OpDescriptor = TryCast(obj, OpDescriptor)
            If o Is Nothing Then Return False
            Return Object.Equals(op, o.op) AndAlso Arrays.equals(dimensions, o.dimensions)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + If(op Is Nothing, 43, op.GetHashCode())
            h = h * 59 + Arrays.hashCode(dimensions)
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "OpDescriptor(op=" & Objects.toString(op) & ", dimensions=" & Arrays.toString(dimensions) & ")"
        End Function

    End Class

End Namespace
