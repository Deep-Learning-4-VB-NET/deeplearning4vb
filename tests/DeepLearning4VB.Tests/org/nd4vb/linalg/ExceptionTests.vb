Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.exception
Imports BlasException = DeepLearning4VB.org.nd4vb.linalg.api.blas.BlasException
Imports ND4VBWorkspaceException = DeepLearning4VB.org.nd4vb.linalg.workspace.ND4VBWorkspaceException
Imports Nd4vbBackend = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vbBackend

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg

    ''' <summary>
    ''' Tests for the messages, causes and hierarchy of the ND4VB exceptions (<c>org.nd4vb.linalg.exception</c>,
    ''' <c>ND4VBWorkspaceException</c>, <c>BlasException</c>, <c>Nd4vbBackend.NoAvailableBackendException</c>).
    ''' <para>
    ''' This class is in <c>org.nd4vb.linalg</c> rather than in a test namespace <c>org.nd4vb.linalg.exception</c>:
    ''' such a namespace would make the plain name <c>Exception</c> mean that namespace in every test under
    ''' <c>org.nd4vb.linalg</c> (VB looks names up in the enclosing namespaces before the imports).
    ''' </para>
    ''' </summary>
    <TestClass>
    Public Class ExceptionTests

        <TestMethod>
        Public Sub testHierarchy()
            ' Java: ND4JException is a RuntimeException; the specific exceptions extend it
            assertTrue(GetType(System.Exception).IsAssignableFrom(GetType(ND4VBException)))
            assertTrue(GetType(ND4VBException).IsAssignableFrom(GetType(ND4VBIllegalStateException)))
            assertTrue(GetType(ND4VBException).IsAssignableFrom(GetType(ND4VBIllegalArgumentException)))
            assertTrue(GetType(ND4VBException).IsAssignableFrom(GetType(ND4VBIllegalAccessException)))
            assertTrue(GetType(ND4VBException).IsAssignableFrom(GetType(ND4VBUnknownDataTypeException)))
            assertTrue(GetType(ND4VBException).IsAssignableFrom(GetType(ND4VBArraySizeException)))
            assertTrue(GetType(ND4VBException).IsAssignableFrom(GetType(ND4UnresolvedOutputVariables)))
            assertTrue(GetType(ND4VBIllegalStateException).IsAssignableFrom(GetType(ND4VBOpProfilerException)))
            assertFalse(GetType(ND4VBException).IsAssignableFrom(GetType(Nd4vbNoSuchWorkspaceException)))
            assertFalse(GetType(ND4VBException).IsAssignableFrom(GetType(ND4VBWorkspaceException)))
        End Sub

        <TestMethod>
        Public Sub testMessagesAndCauses()
            Dim cause As New InvalidOperationException("inner")
            Dim all As System.Exception() = {
                New ND4VBException("m", cause),
                New ND4VBIllegalStateException("m", cause),
                New ND4VBIllegalArgumentException("m", cause),
                New ND4VBIllegalAccessException("m", cause),
                New ND4VBUnknownDataTypeException("m", cause),
                New ND4VBOpProfilerException("m", cause),
                New ND4UnresolvedOutputVariables("m", cause),
                New Nd4vbNoSuchWorkspaceException("m", cause),
                New ND4VBWorkspaceException("m", cause)
            }
            For Each e As System.Exception In all
                assertEquals("m", e.Message, e.GetType().Name)
                Assert.AreSame(cause, e.InnerException, e.GetType().Name)
            Next e

            assertEquals("only a message", New ND4VBIllegalStateException("only a message").Message)
            assertNull(New ND4VBIllegalStateException("only a message").InnerException)
            assertEquals("m", New Nd4vbNoSuchWorkspaceException("m").Message)
            assertEquals("m", New ND4VBWorkspaceException("m").Message)
        End Sub

        <TestMethod>
        Public Sub testCauseOnlyConstructorsUseJavasMessage()
            ' Java: new RuntimeException(cause) has the message cause.toString() ("class name: message")
            Dim cause As New InvalidOperationException("inner")
            Dim expected As String = "System.InvalidOperationException: inner"
            Dim all As System.Exception() = {
                New ND4VBException(cause),
                New ND4VBIllegalStateException(cause),
                New ND4VBIllegalArgumentException(cause),
                New ND4VBIllegalAccessException(cause),
                New ND4VBUnknownDataTypeException(cause),
                New ND4VBOpProfilerException(cause),
                New ND4VBWorkspaceException(cause)
            }
            For Each e As System.Exception In all
                assertEquals(expected, e.Message, e.GetType().Name)
                Assert.AreSame(cause, e.InnerException, e.GetType().Name)
            Next e
            assertNull(New ND4VBException(DirectCast(Nothing, System.Exception)).InnerException)
        End Sub

        <TestMethod>
        Public Sub testSpecialMessages()
            assertEquals("INDArray length is too big to fit into a .NET array", New ND4VBArraySizeException().Message)

            Dim blas As New BlasException("gemm failed", 3)
            assertEquals("gemm failed: 3", blas.Message)
            assertEquals(3, blas.ErrorCode)
            Dim unknown As New BlasException("unknown")
            assertEquals("unknown: -200", unknown.Message)
            assertEquals(BlasException.UNKNOWN_ERROR, unknown.ErrorCode)

            assertEquals("no backend", New Nd4vbBackend.NoAvailableBackendException("no backend").Message)
            Dim inner As New System.IO.FileNotFoundException("missing.dll")
            Dim wrapped As New Nd4vbBackend.NoAvailableBackendException(inner)
            assertEquals("System.IO.FileNotFoundException: missing.dll", wrapped.Message)
            Assert.AreSame(inner, wrapped.InnerException)
        End Sub

    End Class

End Namespace
