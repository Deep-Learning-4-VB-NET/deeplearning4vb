Namespace org.deeplearning4j.common.config.dummies

    ''' <summary>Dummy type with several constructors for the class-loading tests.</summary>
    Public Class TestColor
        Inherits TestAbstract

        Public ReadOnly Property Name As String

        Public Sub New(ByVal color As String)
            Name = color
        End Sub

        Public Sub New(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
            Name = "rgb(" & r & "," & g & "," & b & ")"
        End Sub
    End Class

End Namespace
