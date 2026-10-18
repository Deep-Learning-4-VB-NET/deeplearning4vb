Imports System
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer

Namespace org.nd4vb.linalg.api.shape

    ''' <summary>
    ''' The shape information and offsets of the tensors along a dimension (TADs) of an array. Lombok's
    ''' <c>@Data @AllArgsConstructor @NoArgsConstructor @Builder</c> members are written out:
    ''' <c>TadPack.builder().tadShapeInfo(a).tadOffsets(b).build()</c>.
    ''' </summary>
    Public Class TadPack

        Private tadShapeInfo As DataBuffer
        Private tadOffsets As DataBuffer

        Public Sub New()
        End Sub

        Public Sub New(ByVal tadShapeInfo As DataBuffer, ByVal tadOffsets As DataBuffer)
            Me.tadShapeInfo = tadShapeInfo
            Me.tadOffsets = tadOffsets
        End Sub

        Public Overridable Function getTadShapeInfo() As DataBuffer
            Return tadShapeInfo
        End Function

        Public Overridable Sub setTadShapeInfo(ByVal tadShapeInfo As DataBuffer)
            Me.tadShapeInfo = tadShapeInfo
        End Sub

        Public Overridable Function getTadOffsets() As DataBuffer
            Return tadOffsets
        End Function

        Public Overridable Sub setTadOffsets(ByVal tadOffsets As DataBuffer)
            Me.tadOffsets = tadOffsets
        End Sub

        Public Shared Function builder() As TadPackBuilder
            Return New TadPackBuilder()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As TadPack = TryCast(obj, TadPack)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return Object.Equals(tadShapeInfo, o.tadShapeInfo) AndAlso Object.Equals(tadOffsets, o.tadOffsets)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + If(tadShapeInfo Is Nothing, 43, tadShapeInfo.GetHashCode())
            h = h * 59 + If(tadOffsets Is Nothing, 43, tadOffsets.GetHashCode())
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "TadPack(tadShapeInfo=" & Objects.toString(tadShapeInfo) & ", tadOffsets=" & Objects.toString(tadOffsets) & ")"
        End Function

        ''' <summary>Lombok's builder for <see cref="TadPack"/>.</summary>
        Public Class TadPackBuilder
            Private tadShapeInfoValue As DataBuffer
            Private tadOffsetsValue As DataBuffer

            Public Function tadShapeInfo(ByVal value As DataBuffer) As TadPackBuilder
                Me.tadShapeInfoValue = value
                Return Me
            End Function

            Public Function tadOffsets(ByVal value As DataBuffer) As TadPackBuilder
                Me.tadOffsetsValue = value
                Return Me
            End Function

            Public Function build() As TadPack
                Return New TadPack(tadShapeInfoValue, tadOffsetsValue)
            End Function

            Public Overrides Function ToString() As String
                Return "TadPack.TadPackBuilder(tadShapeInfo=" & Objects.toString(tadShapeInfoValue) & ", tadOffsets=" & Objects.toString(tadOffsetsValue) & ")"
            End Function
        End Class

    End Class

End Namespace
