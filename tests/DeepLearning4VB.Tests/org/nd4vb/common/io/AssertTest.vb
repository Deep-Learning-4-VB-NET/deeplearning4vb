Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports IoAssert = DeepLearning4VB.org.nd4vb.common.io.Assert

Namespace org.nd4vb.common.io

    <TestClass>
    Public Class AssertTest

        <TestMethod>
        Public Sub BooleanAndNullChecks()
            IoAssert.isTrue(True)
            Dim ex = assertThrows(Of ArgumentException)(Sub() IoAssert.isTrue(False, "must hold"))
            assertEquals("must hold", ex.Message)
            assertThrows(Of ArgumentException)(Sub() IoAssert.isTrue(False))
            IoAssert.isNull(Nothing)
            assertThrows(Of ArgumentException)(Sub() IoAssert.isNull("x"))
            IoAssert.notNull("x")
            assertThrows(Of ArgumentException)(Sub() IoAssert.notNull(Nothing, "required"))
            assertThrows(Of InvalidOperationException)(Sub() IoAssert.state(False))
            IoAssert.state(True, "ok")
        End Sub

        <TestMethod>
        Public Sub StringChecks()
            IoAssert.hasLength(" ")
            assertThrows(Of ArgumentException)(Sub() IoAssert.hasLength(""))
            assertThrows(Of ArgumentException)(Sub() IoAssert.hasLength(Nothing, "needs length"))
            IoAssert.hasText(" a ")
            assertThrows(Of ArgumentException)(Sub() IoAssert.hasText("   "))
            IoAssert.doesNotContain("hello", "xyz")
            IoAssert.doesNotContain(Nothing, "x")
            assertThrows(Of ArgumentException)(Sub() IoAssert.doesNotContain("hello", "ell"))
        End Sub

        <TestMethod>
        Public Sub CollectionChecks()
            IoAssert.notEmpty(New Object() {1})
            assertThrows(Of ArgumentException)(Sub() IoAssert.notEmpty(New Object() {}))
            assertThrows(Of ArgumentException)(Sub() IoAssert.notEmpty(DirectCast(Nothing, Object()), "empty array"))
            IoAssert.noNullElements(New Object() {1, "a"})
            IoAssert.noNullElements(DirectCast(Nothing, Object()))
            assertThrows(Of ArgumentException)(Sub() IoAssert.noNullElements(New Object() {1, Nothing}))
            IoAssert.notEmpty(New List(Of Integer) From {1})
            assertThrows(Of ArgumentException)(Sub() IoAssert.notEmpty(New List(Of Integer)()))
            assertThrows(Of ArgumentException)(Sub() IoAssert.notEmpty(DirectCast(Nothing, IEnumerable)))
            IoAssert.notEmpty(New Dictionary(Of String, Integer) From {{"a", 1}})
            assertThrows(Of ArgumentException)(Sub() IoAssert.notEmpty(New Dictionary(Of String, Integer)()))
        End Sub

        <TestMethod>
        Public Sub TypeChecks()
            IoAssert.isInstanceOf(GetType(String), "x")
            IoAssert.isInstanceOf(GetType(Object), 1, "boxed")
            Dim ex = assertThrows(Of ArgumentException)(Sub() IoAssert.isInstanceOf(GetType(String), 1, "Wrong type."))
            assertEquals("Wrong type. Object of class [System.Int32] must be an instance of System.String", ex.Message)
            assertThrows(Of ArgumentException)(Sub() IoAssert.isInstanceOf(GetType(String), Nothing))
            assertThrows(Of ArgumentException)(Sub() IoAssert.isInstanceOf(Nothing, "x"))
            IoAssert.isAssignable(GetType(IEnumerable), GetType(List(Of Integer)))
            assertThrows(Of ArgumentException)(Sub() IoAssert.isAssignable(GetType(String), GetType(Integer)))
            assertThrows(Of ArgumentException)(Sub() IoAssert.isAssignable(GetType(String), Nothing))
        End Sub

    End Class

End Namespace
