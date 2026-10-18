Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.env
Imports DeepLearning4VB.org.nd4vb.linalg.env.impl
Imports ND4VBEnvironmentVars = DeepLearning4VB.org.nd4vb.common.config.ND4VBEnvironmentVars
Imports NumberFormatException = DeepLearning4VB.JavaCompat.NumberFormatException

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.env.impl

    ''' <summary>
    ''' Tests for the environment-variable handler ported in this wave (<c>OmpNumThreadsAction</c>). The other handlers
    ''' (<c>ND4J_DEBUG</c>, <c>ND4J_VERBOSE</c>, workspace modes, ...) drive the <c>Nd4j</c> factory and arrive with it.
    ''' </summary>
    <TestClass>
    Public Class OmpNumThreadsActionTests

        <TestMethod>
        Public Sub testTargetVariable()
            Dim action As EnvironmentalAction = New OmpNumThreadsAction()
            assertEquals("OMP_NUM_THREADS", action.targetVariable())
            assertEquals(ND4VBEnvironmentVars.OMP_NUM_THREADS, action.targetVariable())
        End Sub

        <TestMethod>
        Public Sub testAcceptsJavaIntegers()
            Dim action As New OmpNumThreadsAction()
            action.process("4")
            action.process("+8")
            action.process("-1")
            action.process(Integer.MaxValue.ToString(Globalization.CultureInfo.InvariantCulture))
        End Sub

        <TestMethod>
        Public Sub testRejectsMalformedValuesLikeInteger_valueOf()
            Dim action As New OmpNumThreadsAction()
            For Each bad As String In New String() {"", " 4", "4 ", "four", "4.0", "1e3", "2147483648", "0x10"}
                Dim value As String = bad
                Dim ex = assertThrows(Of NumberFormatException)(Sub() action.process(value))
                assertEquals("For input string: """ & value & """", ex.Message)
            Next bad
            Dim nullEx = assertThrows(Of NumberFormatException)(Sub() action.process(Nothing))
            assertEquals("null", nullEx.Message)
            ' NumberFormatException derives from ArgumentException, as Java's derives from IllegalArgumentException
            assertThrows(Of ArgumentException)(Sub() action.process("x"))
        End Sub

    End Class

End Namespace
