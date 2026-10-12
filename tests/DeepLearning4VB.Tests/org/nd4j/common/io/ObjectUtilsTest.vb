Imports System
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.io

Namespace org.nd4j.common.io

    <TestClass>
    Public Class ObjectUtilsTest

        Private Enum Color
            Red
            Green
        End Enum

        <TestMethod>
        Public Sub EmptinessAndContainment()
            assertTrue(ObjectUtils.isEmpty(Nothing))
            assertTrue(ObjectUtils.isEmpty(New Object() {}))
            assertFalse(ObjectUtils.isEmpty(New String() {"a"}))
            assertTrue(ObjectUtils.isArray(New Integer() {1}))
            assertFalse(ObjectUtils.isArray("x"))
            assertFalse(ObjectUtils.isArray(Nothing))
            assertTrue(ObjectUtils.containsElement(New Object() {1, "a", Nothing}, "a"))
            assertTrue(ObjectUtils.containsElement(New Object() {1, Nothing}, Nothing))
            assertFalse(ObjectUtils.containsElement(Nothing, "a"))
            Dim colors() As Color = DirectCast([Enum].GetValues(GetType(Color)), Color())
            assertTrue(ObjectUtils.containsConstant(colors, "red"))
            assertFalse(ObjectUtils.containsConstant(colors, "red", True))
            assertTrue(ObjectUtils.containsConstant(colors, "Red", True))
            assertEquals(Color.Green, ObjectUtils.caseInsensitiveValueOf(colors, "GREEN"))
            assertThrows(Of ArgumentException)(Function() ObjectUtils.caseInsensitiveValueOf(colors, "blue"))
        End Sub

        <TestMethod>
        Public Sub ArrayConversions()
            Dim grown() As String = ObjectUtils.addObjectToArray(New String() {"a"}, "b")
            assertEquals(New String() {"a", "b"}, grown)
            Dim fromNothing() As Object = ObjectUtils.addObjectToArray(DirectCast(Nothing, Object()), "x")
            assertEquals(New Object() {"x"}, fromNothing)
            assertEquals(GetType(String), fromNothing.GetType().GetElementType())
            Dim boxed() As Object = ObjectUtils.toObjectArray(New Integer() {1, 2})
            assertEquals(New Object() {1, 2}, boxed)
            assertEquals(0, ObjectUtils.toObjectArray(Nothing).Length)
            Dim same() As String = {"s"}
            assertTrue(Object.ReferenceEquals(same, ObjectUtils.toObjectArray(same)))
            assertThrows(Of ArgumentException)(Function() ObjectUtils.toObjectArray("not an array"))
        End Sub

        <TestMethod>
        Public Sub NullSafeEqualsAndHashCode()
            assertTrue(ObjectUtils.nullSafeEquals(Nothing, Nothing))
            assertFalse(ObjectUtils.nullSafeEquals(Nothing, "a"))
            assertTrue(ObjectUtils.nullSafeEquals("a", "a"))
            assertTrue(ObjectUtils.nullSafeEquals(New Integer() {1, 2}, New Integer() {1, 2}))
            assertFalse(ObjectUtils.nullSafeEquals(New Integer() {1, 2}, New Integer() {2, 1}))
            assertTrue(ObjectUtils.nullSafeEquals(New String() {"a", Nothing}, New String() {"a", Nothing}))
            assertFalse(ObjectUtils.nullSafeEquals(New Integer() {1}, New Long() {1}))
            assertEquals(0, ObjectUtils.nullSafeHashCode(DirectCast(Nothing, Object)))
            assertEquals(1231, ObjectUtils.hashCode(True))
            assertEquals(1237, ObjectUtils.hashCode(False))
            assertEquals(1072693248, ObjectUtils.hashCode(1.0))
            assertEquals(1065353216, ObjectUtils.hashCode(1.0F))
            assertEquals(1, ObjectUtils.hashCode(4294967296L))
            ' Spring's array hash: 31 * (31 * 7 + 1) + 2
            assertEquals(6760, ObjectUtils.nullSafeHashCode(New Integer() {1, 2}))
            assertEquals(6760, ObjectUtils.nullSafeHashCode(DirectCast(New Integer() {1, 2}, Object)))
            assertEquals(6760, ObjectUtils.nullSafeHashCode(New Object() {1, 2}))
            assertEquals(31 * 7 + 1231, ObjectUtils.nullSafeHashCode(New Boolean() {True}))
            assertEquals(217, ObjectUtils.nullSafeHashCode(New Object() {Nothing}))
            assertEquals(0, ObjectUtils.nullSafeHashCode(DirectCast(Nothing, Integer())))
        End Sub

        <TestMethod>
        Public Sub StringRendering()
            assertEquals("null", ObjectUtils.nullSafeToString(DirectCast(Nothing, Object)))
            assertEquals("abc", ObjectUtils.nullSafeToString("abc"))
            assertEquals("{1, 2}", ObjectUtils.nullSafeToString(New Integer() {1, 2}))
            assertEquals("{'a', 'b'}", ObjectUtils.nullSafeToString(New Char() {"a"c, "b"c}))
            assertEquals("{true, false}", ObjectUtils.nullSafeToString(New Boolean() {True, False}))
            assertEquals("{1.5, null}", ObjectUtils.nullSafeToString(New Object() {1.5, Nothing}))
            assertEquals("{}", ObjectUtils.nullSafeToString(New Long() {}))
            assertEquals("{1.0}", ObjectUtils.nullSafeToString(DirectCast(New Double() {1.0}, Object)))
            assertEquals("true", ObjectUtils.nullSafeToString(True))
            assertEquals("", ObjectUtils.getDisplayString(Nothing))
            assertEquals("x", ObjectUtils.getDisplayString("x"))
            assertEquals("null", ObjectUtils.nullSafeClassName(Nothing))
            assertEquals("System.String", ObjectUtils.nullSafeClassName("x"))
            Dim o As New Object()
            assertEquals("System.Object@" & ObjectUtils.getIdentityHexString(o), ObjectUtils.identityToString(o))
            assertEquals("", ObjectUtils.identityToString(Nothing))
            assertEquals(ObjectUtils.getIdentityHexString(o), ObjectUtils.getIdentityHexString(o))
        End Sub

        <TestMethod>
        Public Sub CheckedExceptionsDoNotExist()
            assertFalse(ObjectUtils.isCheckedException(New IOException()))
            assertTrue(ObjectUtils.isCompatibleWithThrowsClause(New IOException(), GetType(ArgumentException)))
            assertTrue(ObjectUtils.isCompatibleWithThrowsClause(New InvalidOperationException()))
        End Sub

    End Class

End Namespace
