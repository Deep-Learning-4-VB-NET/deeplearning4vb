Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace org.nd4vb.linalg

    ''' <summary>
    ''' Test support for code that reaches the <c>Nd4vb</c> factory (Java's <c>Nd4j</c>), which phase 2 wave 3 ports.
    ''' While only the wave-2 stand-in exists (its members throw <see cref="NotImplementedException"/> with a message
    ''' naming the stand-in), <see cref="runWithNd4vb"/> reports the test as inconclusive instead of failed; once the
    ''' factory exists the test runs normally.
    ''' </summary>
    Friend Module Nd4vbStandIn

        ''' <summary>Runs <paramref name="action"/>; inconclusive when it reaches the <c>Nd4vb</c> stand-in.</summary>
        Public Sub runWithNd4vb(ByVal action As Action)
            Try
                action()
            Catch e As NotImplementedException When e.Message IsNot Nothing AndAlso e.Message.Contains("stand-in")
                Assert.Inconclusive("Needs the Nd4vb factory (phase 2 wave 3): " & e.Message)
            End Try
        End Sub

    End Module

End Namespace
