Namespace org.deeplearning4j.common.config.dummies

    ''' <summary>Dummy type whose constructor takes another dummy, for the class-loading tests.</summary>
    Public Class TestRectangle
        Inherits TestAbstract

        Public ReadOnly Property Width As Integer
        Public ReadOnly Property Height As Integer
        Public ReadOnly Property Color As TestAbstract

        Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal color As TestAbstract)
            Me.Width = width
            Me.Height = height
            Me.Color = color
        End Sub
    End Class

End Namespace
