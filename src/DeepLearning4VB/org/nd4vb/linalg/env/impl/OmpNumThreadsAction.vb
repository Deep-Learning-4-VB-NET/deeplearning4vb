Imports System
Imports System.Globalization
Imports ND4VBEnvironmentVars = DeepLearning4VB.org.nd4vb.common.config.ND4VBEnvironmentVars
Imports EnvironmentalAction = DeepLearning4VB.org.nd4vb.linalg.env.EnvironmentalAction

Namespace org.nd4vb.linalg.env.impl

    ''' <summary>
    ''' Handles the <c>OMP_NUM_THREADS</c> environment variable. As in Java the value is parsed (so a malformed value
    ''' fails) but nothing else happens: <c>Nd4j.setNumThreads</c> no longer exists, and the managed backend sizes
    ''' its parallel loops itself.
    ''' </summary>
    Public Class OmpNumThreadsAction
        Implements EnvironmentalAction

        Public Overridable Function targetVariable() As String Implements EnvironmentalAction.targetVariable
            Return ND4VBEnvironmentVars.OMP_NUM_THREADS
        End Function

        Public Overridable Sub process(ByVal value As String) Implements EnvironmentalAction.process
            ' Java Integer.valueOf: an optional sign and decimal digits only
            Dim v As Integer
            If value Is Nothing OrElse Not Integer.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, v) Then
                Throw New JavaCompat.NumberFormatException(If(value Is Nothing, "null", "For input string: """ & value & """"))
            End If

            Dim skipper As String = System.Environment.GetEnvironmentVariable(ND4VBEnvironmentVars.ND4VB_SKIP_BLAS_THREADS)
            If skipper Is Nothing Then
                ' we infer num threads only if skipper undefined
                ' Nd4j.setNumThreads(v);
                ' method does not do anything anymore and was removed
            End If
        End Sub

    End Class

End Namespace
