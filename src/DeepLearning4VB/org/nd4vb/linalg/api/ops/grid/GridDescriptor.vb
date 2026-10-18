Imports System
Imports System.Collections.Generic

Namespace org.nd4vb.linalg.api.ops.grid

    ''' <summary>
    ''' The ops of a grid op (Java <c>GridDescriptor</c>; Lombok's <c>@Data @NoArgsConstructor @AllArgsConstructor</c>
    ''' members are written out).
    ''' </summary>
    Public Class GridDescriptor

        Private gridDepth As Integer = 0
        Private gridPointers As IList(Of GridPointers) = New List(Of GridPointers)()

        Public Sub New()
        End Sub

        Public Sub New(ByVal gridDepth As Integer, ByVal gridPointers As IList(Of GridPointers))
            Me.gridDepth = gridDepth
            Me.gridPointers = gridPointers
        End Sub

        Public Overridable Function getGridDepth() As Integer
            Return gridDepth
        End Function

        Public Overridable Sub setGridDepth(ByVal gridDepth As Integer)
            Me.gridDepth = gridDepth
        End Sub

        Public Overridable Function getGridPointers() As IList(Of GridPointers)
            Return gridPointers
        End Function

        Public Overridable Sub setGridPointers(ByVal gridPointers As IList(Of GridPointers))
            Me.gridPointers = gridPointers
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As GridDescriptor = TryCast(obj, GridDescriptor)
            If o Is Nothing Then Return False
            If gridDepth <> o.gridDepth Then Return False
            If gridPointers Is Nothing OrElse o.gridPointers Is Nothing Then Return gridPointers Is o.gridPointers
            Return gridPointers.SequenceEqual(o.gridPointers)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + gridDepth
            h = h * 59 + If(gridPointers Is Nothing, 43, gridPointers.Count)
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "GridDescriptor(gridDepth=" & gridDepth & ", gridPointers=" & If(gridPointers Is Nothing, "null", "[" & String.Join(", ", gridPointers) & "]") & ")"
        End Function

    End Class

End Namespace
