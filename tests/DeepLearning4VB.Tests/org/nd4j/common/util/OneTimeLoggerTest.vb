Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.util
Imports DeepLearning4VB.org.slf4j

Namespace org.nd4j.common.util

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class OneTimeLoggerTest

        ''' <summary>Captures log output so the tests can verify what was written.</summary>
        Private Class RecordingLogger
            Inherits TraceLogger

            Public ReadOnly Messages As New List(Of String)()

            Public Sub New()
                MyBase.New("test")
            End Sub

            Protected Overrides Sub Write(ByVal level As LogLevel, ByVal message As String)
                Messages.Add(level.ToString() & ": " & message)
            End Sub
        End Class

        <TestInitialize>
        Public Sub Reset()
            OneTimeLogger.reset()
        End Sub

        <TestMethod>
        Public Sub testLogger1()
            Dim log As New RecordingLogger()
            OneTimeLogger.info(log, "Format: {}; Pew: {};", 1, 2)
            OneTimeLogger.info(log, "Format: {}; Pew: {};", 3, 4)
            OneTimeLogger.warn(log, "Something else")
            assertEquals(2, log.Messages.Count)
            assertEquals("Info: Format: 1; Pew: 2;", log.Messages(0))
            assertEquals("Warn: Something else", log.Messages(1))
        End Sub

        <TestMethod>
        Public Sub testBuffer1()
            assertTrue(OneTimeLogger.isEligible("Message here"))
            assertFalse(OneTimeLogger.isEligible("Message here"))
            assertTrue(OneTimeLogger.isEligible("Message here 23"))
        End Sub

        <TestMethod>
        Public Sub testBufferEvictsOldMessages()
            For i As Integer = 0 To 99
                assertTrue(OneTimeLogger.isEligible("m" & i))
            Next i
            assertFalse(OneTimeLogger.isEligible("m0"))
            ' the 101st distinct message evicts the oldest one, which becomes eligible again
            assertTrue(OneTimeLogger.isEligible("m100"))
            assertTrue(OneTimeLogger.isEligible("m0"))
        End Sub

        <TestMethod>
        Public Sub testResetForgetsMessages()
            assertTrue(OneTimeLogger.isEligible("again"))
            assertFalse(OneTimeLogger.isEligible("again"))
            OneTimeLogger.reset()
            assertTrue(OneTimeLogger.isEligible("again"))
        End Sub

    End Class

End Namespace
