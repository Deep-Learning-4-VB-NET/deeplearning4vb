Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.env
Imports DeepLearning4VB.org.nd4vb.linalg.env.impl

Namespace org.nd4vb.linalg.env.impl

    ''' <summary>
    ''' Tests for the environment-variable handlers that drive the <c>Nd4vb</c> factory. The factory arrives in wave 3, so
    ''' these tests check the variable names and the value parsing that happens before the factory is touched.
    ''' </summary>
    <TestClass>
    Public Class EnvironmentalActionsTests

        <TestMethod>
        Public Sub testTargetVariables()
            assertEquals("ND4VB_DEBUG", New DebugAction().targetVariable())
            assertEquals("ND4VB_FALLBACK", New FallbackAction().targetVariable())
            assertEquals("ND4VB_PREVENT_UNPACK", New NDArrayUnpackAction().targetVariable())
            assertEquals("ND4VB_VERBOSE", New VerboseAction().targetVariable())
            assertEquals("ND4VB_BYPASS_WS", New WorkspacesBypassAction().targetVariable())
            assertEquals("ND4VB_WORKSPACES_DEBUG", New WorkspacesDebugAction().targetVariable())
            assertEquals("ND4VB_SPILL_WS", New WorkspacesSpillAction().targetVariable())
            For Each a As EnvironmentalAction In New EnvironmentalAction() {New DebugAction(), New FallbackAction(), New NDArrayUnpackAction(), New VerboseAction(), New WorkspacesBypassAction(), New WorkspacesDebugAction(), New WorkspacesSpillAction()}
                assertTrue(a.targetVariable().StartsWith("ND4VB_"))
            Next
        End Sub

        <TestMethod>
        Public Sub testJavaBooleanParsing()
            ' Java Boolean.valueOf: only "true" (any case) is true; nothing throws
            assertTrue(DebugAction.javaBoolean("true"))
            assertTrue(DebugAction.javaBoolean("TRUE"))
            assertFalse(DebugAction.javaBoolean("yes"))
            assertFalse(DebugAction.javaBoolean("1"))
            assertFalse(DebugAction.javaBoolean(Nothing))
            assertFalse(WorkspacesSpillAction.javaBoolean(""))
        End Sub

        <TestMethod>
        Public Sub testFalseValuesDoNotTouchTheFactory()
            ' false values and unknown modes leave the workspace manager alone (Nd4vb is not reached)
            Call New WorkspacesBypassAction().process("false")
            Call New WorkspacesSpillAction().process("no")
            Call New WorkspacesDebugAction().process("something else")
            Call New WorkspacesDebugAction().process(Nothing)
        End Sub

    End Class

End Namespace
