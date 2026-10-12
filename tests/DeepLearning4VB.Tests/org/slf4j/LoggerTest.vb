Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.slf4j

Namespace org.slf4j

    ''' <summary>Tests for the slf4j-style logging shim.</summary>
    <TestClass>
    Public Class LoggerTest

        Private Class Capture
            Inherits TraceLogger

            Public ReadOnly Lines As New List(Of String)()

            Public Sub New(ByVal name As String)
                MyBase.New(name)
            End Sub

            Protected Overrides Sub Write(ByVal level As LogLevel, ByVal message As String)
                Lines.Add(level.ToString() & "|" & Name & "|" & message)
            End Sub
        End Class

        <TestCleanup>
        Public Sub RestoreDefaults()
            LoggerFactory.Provider = Nothing
            LoggerFactory.Level = LogLevel.Info
        End Sub

        <TestMethod>
        Public Sub LevelThresholdFiltersMessages()
            Dim log As New Capture("t")
            LoggerFactory.Level = LogLevel.Warn
            assertFalse(log.IsInfoEnabled)
            assertTrue(log.IsWarnEnabled)
            log.info("hidden {}", 1)
            log.warn("shown {}", 2)
            log.error("err {}", 3)
            assertEquals(New String() {"Warn|t|shown 2", "Error|t|err 3"}, log.Lines.ToArray())

            LoggerFactory.Level = LogLevel.Trace
            log.trace("t")
            log.debug("d")
            assertEquals(4, log.Lines.Count)

            LoggerFactory.Level = LogLevel.Off
            log.error("nothing")
            assertEquals(4, log.Lines.Count)
        End Sub

        <TestMethod>
        Public Sub ErrorWithExceptionAppendsIt()
            Dim log As New Capture("t")
            log.error("failed", New InvalidOperationException("boom"))
            assertEquals(1, log.Lines.Count)
            assertTrue(log.Lines(0).StartsWith("Error|t|failed"))
            assertTrue(log.Lines(0).Contains("boom"))
        End Sub

        <TestMethod>
        Public Sub FactoryUsesProviderAndTypeNames()
            Dim created As New List(Of String)()
            LoggerFactory.Provider = Function(name)
                                         created.Add(name)
                                         Return New Capture(name)
                                     End Function
            Dim l1 = LoggerFactory.getLogger(GetType(LoggerTest))
            Dim l2 = LoggerFactory.getLogger("custom")
            assertEquals(GetType(LoggerTest).FullName, l1.Name)
            assertEquals("custom", l2.Name)
            assertEquals(2, created.Count)
            LoggerFactory.Provider = Nothing
            assertTrue(TypeOf LoggerFactory.getLogger("x") Is TraceLogger)
            assertEquals("root", LoggerFactory.getLogger(DirectCast(Nothing, String)).Name)
        End Sub

    End Class

End Namespace
