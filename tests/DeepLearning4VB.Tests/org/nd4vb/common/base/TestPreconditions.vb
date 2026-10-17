Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.base

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.common.base

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class TestPreconditions

        <TestMethod>
        Public Sub testPreconditions()
            Preconditions.checkArgument(True)
            ' Java checks that the message is null; .NET exceptions always carry a default message.
            assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False))

            Preconditions.checkArgument(True, "Message %s here", 10)
            assertEquals("Message 10 here", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "Message %s here", 10)).Message)

            Preconditions.checkArgument(True, "Message %s here %s there", 10, 20)
            assertEquals("Message 10 here 20 there", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "Message %s here %s there", 10, 20)).Message)

            Preconditions.checkArgument(True, "Message %s here %s there %s more", 10, 20, 30)
            assertEquals("Message 10 here 20 there 30 more", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "Message %s here %s there %s more", 10, 20, 30)).Message)

            Preconditions.checkArgument(True, "Message %s here", 10L)
            assertEquals("Message 10 here", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "Message %s here", 10L)).Message)

            Preconditions.checkArgument(True, "Message %s here %s there", 10L, 20L)
            assertEquals("Message 10 here 20 there", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "Message %s here %s there", 10L, 20L)).Message)

            Preconditions.checkArgument(True, "Message %s here %s there %s more", 10L, 20L, 30L)
            assertEquals("Message 10 here 20 there 30 more", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "Message %s here %s there %s more", 10L, 20L, 30L)).Message)

            Preconditions.checkArgument(True, "Message %s here %s there %s more", "A", "B", "C")
            assertEquals("Message A here B there C more", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "Message %s here %s there %s more", "A", "B", "C")).Message)
        End Sub

        <TestMethod>
        Public Sub testPreconditionsMalformed()
            'No %s:
            Preconditions.checkArgument(True, "This is malformed", "A", "B", "C")
            assertEquals("This is malformed [A,B,C]", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "This is malformed", "A", "B", "C")).Message)

            'More args than %s:
            Preconditions.checkArgument(True, "This is %s malformed", "A", "B", "C")
            assertEquals("This is A malformed [B,C]", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "This is %s malformed", "A", "B", "C")).Message)

            'No args
            Preconditions.checkArgument(True, "This is %s %s malformed")
            assertEquals("This is %s %s malformed", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "This is %s %s malformed")).Message)

            'More %s than args
            Preconditions.checkArgument(True, "This is %s %s malformed", "A")
            assertEquals("This is A %s malformed", assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "This is %s %s malformed", "A")).Message)
        End Sub

        <TestMethod>
        Public Sub testPreconditionsState()
            Preconditions.checkState(True)
            assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False))

            Preconditions.checkState(True, "Message %s here", 10)
            assertEquals("Message 10 here", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "Message %s here", 10)).Message)

            Preconditions.checkState(True, "Message %s here %s there", 10, 20)
            assertEquals("Message 10 here 20 there", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "Message %s here %s there", 10, 20)).Message)

            Preconditions.checkState(True, "Message %s here %s there %s more", 10, 20, 30)
            assertEquals("Message 10 here 20 there 30 more", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "Message %s here %s there %s more", 10, 20, 30)).Message)

            Preconditions.checkState(True, "Message %s here", 10L)
            assertEquals("Message 10 here", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "Message %s here", 10L)).Message)

            Preconditions.checkState(True, "Message %s here %s there", 10L, 20L)
            assertEquals("Message 10 here 20 there", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "Message %s here %s there", 10L, 20L)).Message)

            Preconditions.checkState(True, "Message %s here %s there %s more", 10L, 20L, 30L)
            assertEquals("Message 10 here 20 there 30 more", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "Message %s here %s there %s more", 10L, 20L, 30L)).Message)

            Preconditions.checkState(True, "Message %s here %s there %s more", "A", "B", "C")
            assertEquals("Message A here B there C more", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "Message %s here %s there %s more", "A", "B", "C")).Message)
        End Sub

        <TestMethod>
        Public Sub testPreconditionsMalformedState()
            Preconditions.checkState(True, "This is malformed", "A", "B", "C")
            assertEquals("This is malformed [A,B,C]", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "This is malformed", "A", "B", "C")).Message)

            Preconditions.checkState(True, "This is %s malformed", "A", "B", "C")
            assertEquals("This is A malformed [B,C]", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "This is %s malformed", "A", "B", "C")).Message)

            Preconditions.checkState(True, "This is %s %s malformed")
            assertEquals("This is %s %s malformed", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "This is %s %s malformed")).Message)

            Preconditions.checkState(True, "This is %s %s malformed", "A")
            assertEquals("This is A %s malformed", assertThrows(Of InvalidOperationException)(Sub() Preconditions.checkState(False, "This is %s %s malformed", "A")).Message)
        End Sub

        <TestMethod>
        Public Sub testPreconditionsNull()
            Preconditions.checkNotNull("")
            assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing))

            Preconditions.checkNotNull("", "Message %s here", 10)
            assertEquals("Message 10 here", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here", 10)).Message)

            Preconditions.checkNotNull("", "Message %s here %s there", 10, 20)
            assertEquals("Message 10 here 20 there", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here %s there", 10, 20)).Message)

            Preconditions.checkNotNull("", "Message %s here %s there %s more", 10, 20, 30)
            assertEquals("Message 10 here 20 there 30 more", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here %s there %s more", 10, 20, 30)).Message)

            Preconditions.checkNotNull("", "Message %s here", 10L)
            assertEquals("Message 10 here", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here", 10L)).Message)

            Preconditions.checkNotNull("", "Message %s here %s there", 10L, 20L)
            assertEquals("Message 10 here 20 there", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here %s there", 10L, 20L)).Message)

            Preconditions.checkNotNull("", "Message %s here %s there %s more", 10L, 20L, 30L)
            assertEquals("Message 10 here 20 there 30 more", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here %s there %s more", 10L, 20L, 30L)).Message)

            Preconditions.checkNotNull("", "Message %s here %s there %s more", "A", "B", "C")
            assertEquals("Message A here B there C more", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here %s there %s more", "A", "B", "C")).Message)

            Preconditions.checkNotNull("", "Message %s here %s there %s more", New Integer() {0, 1}, New Double() {2.0, 3.0}, New Boolean() {True, False})
            assertEquals("Message [0, 1] here [2.0, 3.0] there [true, false] more", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here %s there %s more", New Integer() {0, 1}, New Double() {2.0, 3.0}, New Boolean() {True, False})).Message)

            Preconditions.checkNotNull("", "Message %s here %s there", New String() {"A", "B"}, New Object() {1.0, "C"})
            assertEquals("Message [A, B] here [1.0, C] there", assertThrows(Of NullReferenceException)(Sub() Preconditions.checkNotNull(Nothing, "Message %s here %s there", New String() {"A", "B"}, New Object() {1.0, "C"})).Message)
        End Sub

        ''' <summary>Custom formatters are registered explicitly in .NET (Java used ServiceLoader).</summary>
        <TestMethod>
        Public Sub testCustomFormatter()
            Preconditions.registerFormat(New UpperCaseFormat())
            Dim ex = assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "%up and %s", "shout", "quiet"))
            assertEquals("SHOUT and quiet", ex.Message)
            ' A custom tag at the very start of the message must be honoured too.
            ex = assertThrows(Of ArgumentException)(Sub() Preconditions.checkArgument(False, "%up first", "a"))
            assertEquals("A first", ex.Message)
        End Sub

        Private Class UpperCaseFormat
            Implements PreconditionsFormat

            Public Function formatTags() As System.Collections.Generic.IList(Of String) Implements PreconditionsFormat.formatTags
                Return New System.Collections.Generic.List(Of String) From {"%up"}
            End Function

            Public Function format(ByVal tag As String, ByVal arg As Object) As String Implements PreconditionsFormat.format
                Return arg.ToString().ToUpperInvariant()
            End Function
        End Class

    End Class

End Namespace
