Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.primitives

Namespace org.nd4j.common.primitives

    ''' <summary>Tests for Pair/Triple/Quad (and immutable variants) and Optional.</summary>
    <TestClass>
    Public Class TupleTest

        <TestMethod>
        Public Sub Pair_EqualityHashCodeAndAccessors()
            Dim p = Pair.of("k", 1)
            Dim q = New Pair(Of String, Integer)("k", 1)
            assertEquals(p, q)
            assertEquals(p.GetHashCode(), q.GetHashCode())
            assertNotEquals(p, Pair.of("k", 2))
            assertNotEquals(p, Nothing)
            assertEquals("k", p.Key)
            assertEquals("k", p.First)
            assertEquals("k", p.Left)
            assertEquals("k", p.getKey())
            assertEquals(1, p.Value)
            assertEquals(1, p.Second)
            assertEquals(1, p.Right)
            assertEquals(1, p.getValue())
            p.Second = 5
            assertEquals(5, p.getSecond())
            p.setFirst("j")
            assertEquals("j", p.Key)
            assertEquals("Pair{key=j, value=5}", p.ToString())
            assertEquals("Pair{key=[1, 2], value=null}", Pair.of(New Integer() {1, 2}, DirectCast(Nothing, String)).ToString())
            Dim empty As New Pair(Of String, String)()
            assertNull(empty.First)
            assertEquals(Pair.of(1, 2), Pair.fromArray(New Integer() {1, 2}))
            assertThrows(Of ArgumentException)(Sub() Pair.fromArray(New Integer() {1, 2, 3}))
            assertEquals(Pair.makePair(1, "a"), Pair.create(1, "a"))
            assertEquals(Pair.pairOf(1, "a"), Pair.create(1, "a"))
            ' pairs work as dictionary keys
            Dim d As New Dictionary(Of Pair(Of String, Integer), String)()
            d(Pair.of("a", 1)) = "x"
            assertEquals("x", d(Pair.of("a", 1)))
        End Sub

        <TestMethod>
        Public Sub TripleAndQuad()
            Dim t = Triple.of(1, "two", 3.0)
            assertEquals(1, t.First)
            assertEquals("two", t.Middle)
            assertEquals(3.0, t.Right)
            assertEquals(t, Triple.tripleOf(1, "two", 3.0))
            assertEquals(t.GetHashCode(), New Triple(Of Integer, String, Double)(1, "two", 3.0).GetHashCode())
            assertNotEquals(t, Triple.of(1, "two", 4.0))
            t.setThird(9.0)
            assertEquals(9.0, t.getThird())
            assertEquals("Triple(first=1, second=two, third=9.0)", t.ToString())

            Dim q = Quad.of("a", 2, 3L, True)
            assertEquals("a", q.getFirst())
            assertEquals(2, q.Second)
            assertEquals(3L, q.Third)
            assertEquals(True, q.Fourth)
            assertEquals(q, Quad.quadOf("a", 2, 3L, True))
            assertNotEquals(q, Quad.of("a", 2, 3L, False))
            q.Fourth = False
            assertEquals(Quad.of("a", 2, 3L, False), q)
        End Sub

        <TestMethod>
        Public Sub ImmutableVariants()
            Dim p = ImmutablePair.of("k", 1)
            assertEquals("k", p.Key)
            assertEquals(1, p.Second)
            assertEquals(p, ImmutablePair.makePair("k", 1))
            assertEquals(p, ImmutablePair.create("k", 1))
            assertNotEquals(p, Pair.of("k", 1)) ' different types are not equal, as in Java
            assertEquals("ImmutablePair(key=k, value=1)", p.ToString())

            Dim t = ImmutableTriple.of(1, 2, 3)
            assertEquals(2, t.Middle)
            assertEquals(t, ImmutableTriple.tripleOf(1, 2, 3))
            assertEquals(t.GetHashCode(), ImmutableTriple.of(1, 2, 3).GetHashCode())

            Dim q = ImmutableQuad.of(1, 2, 3, 4)
            assertEquals(4, q.getFourth())
            assertEquals(q, ImmutableQuad.quadOf(1, 2, 3, 4))
            assertNotEquals(q, ImmutableQuad.of(1, 2, 3, 5))
        End Sub

        <TestMethod>
        Public Sub Optional_Semantics()
            Dim empty = [Optional](Of String).empty()
            assertFalse(empty.Present)
            assertFalse(empty.isPresent())
            assertEquals("dflt", empty.orElse("dflt"))
            assertThrows(Of InvalidOperationException)(Function() empty.[get]())
            assertEquals("Optional()", empty.ToString())
            assertTrue([Optional](Of String).empty() Is empty)

            Dim some = [Optional](Of String).of("x")
            assertTrue(some.Present)
            assertEquals("x", some.[get]())
            assertEquals("x", some.orElse("dflt"))
            assertEquals("Optional(x)", some.ToString())
            assertEquals(some, [Optional](Of String).ofNullable("x"))
            assertEquals(some.GetHashCode(), [Optional](Of String).of("x").GetHashCode())
            assertNotEquals(some, empty)
            assertEquals(empty, [Optional](Of String).ofNullable(Nothing))
            assertThrows(Of NullReferenceException)(Function() [Optional](Of String).of(Nothing))
        End Sub

        <TestMethod>
        Public Sub SynchronizedObject_GetSet()
            Dim so As New SynchronizedObject(Of String)("a")
            assertEquals("a", so.[get]())
            so.[set]("b")
            assertEquals("b", so.Value)
            so.Value = "c"
            assertEquals("c", so.[get]())
            assertNull(New SynchronizedObject(Of String)().[get]())
        End Sub

    End Class

End Namespace
