Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.collection
Imports DeepLearning4VB.org.nd4j.common.primitives

Namespace org.nd4j.common.collection

    <TestClass>
    Public Class IntArrayKeyMapTest

        <TestMethod>
        Public Sub KeysAreComparedByContent()
            Dim m As New IntArrayKeyMap(Of String)()
            assertNull(m.put(New Integer() {1, 2, 3}, "a"))
            assertEquals("a", m.put(New Integer() {1, 2, 3}, "b"))
            assertEquals(1, m.Count)
            assertEquals("b", m.[get](New Integer() {1, 2, 3}))
            assertNull(m.[get](New Integer() {3, 2, 1}))
            assertTrue(m.ContainsKey(New Integer() {1, 2, 3}))
            m(New Integer() {7}) = "seven"
            assertEquals(2, m.size())
            assertEquals(New Integer() {7}, m.Keys.Last())
            Dim v As String = Nothing
            assertTrue(m.TryGetValue(New Integer() {7}, v))
            assertEquals("seven", v)
            assertEquals("seven", m.removeAndGet(New Integer() {7}))
            assertFalse(m.Remove(New Integer() {7}))
            assertTrue(m.containsValue("b"))
            For Each kv In m
                assertEquals(New Integer() {1, 2, 3}, kv.Key)
                assertEquals("b", kv.Value)
            Next
            Dim other As New IntArrayKeyMap(Of String)()
            other.putAll(m)
            assertEquals(m, other)
            m.Clear()
            assertTrue(m.IsEmpty)
        End Sub

        <TestMethod>
        Public Sub IntArray_DeduplicatesAndHashesByContent()
            Dim a As New IntArray(New Integer() {1, 1, 2})
            Dim b As New IntArray(New Integer() {1, 2})
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertEquals(New Integer() {1, 2}, a.getBackingArray())
            assertEquals("[1, 2]", a.ToString())
            assertThrows(Of NullReferenceException)(Function() New IntArray(Nothing))
        End Sub

        <TestMethod>
        Public Sub IntArrayKeySet_IsAnOrderedSetOfArrays()
            Dim s As New IntArrayKeySet()
            assertTrue(s.Add(New Integer() {1, 2}))
            assertFalse(s.Add(New Integer() {1, 2}))
            assertTrue(s.Add(New Integer() {3}))
            assertEquals(2, s.Count)
            assertTrue(s.Contains(New Integer() {3}))
            assertFalse(s.Contains(New Integer() {2, 1}))
            assertEquals(New Integer()() {New Integer() {1, 2}, New Integer() {3}}, s.ToArray())
            assertTrue(s.IsSupersetOf(New Integer()() {New Integer() {3}}))
            s.ExceptWith(New Integer()() {New Integer() {1, 2}})
            assertEquals(1, s.Count)
            assertTrue(s.Remove(New Integer() {3}))
            assertTrue(s.IsEmpty)
        End Sub

    End Class

    <TestClass>
    Public Class MultiDimensionalMapTest

        <TestMethod>
        Public Sub PutGetByTwoKeys()
            Dim m = MultiDimensionalMap(Of String, Integer, String).newHashBackedMap()
            m.put("a", 1, "a1")
            m.put("a", 2, "a2")
            m.put("b", 1, "b1")
            assertEquals(3, m.size())
            assertEquals("a2", m.[get]("a", 2))
            assertNull(m.[get]("z", 9))
            assertTrue(m.contains("b", 1))
            assertFalse(m.contains("b", 2))
            assertEquals("a1", m(New Pair(Of String, Integer)("a", 1)))
            assertEquals("a1", m.put(New Pair(Of String, Integer)("a", 1), "A1"))
            assertTrue(m.remove("b", 1))
            assertFalse(m.remove("b", 1))
            assertEquals(2, m.Count)
            Dim entries = m.entrySet()
            assertEquals(2, entries.Count)
            Dim first = entries.First()
            assertEquals(first.Key, New Pair(Of String, Integer)(first.FirstKey, first.SecondKey))
            assertEquals("A1", m.removeAndGet(New Pair(Of String, Integer)("a", 1)))
            assertTrue(m.containsValue("a2"))
        End Sub

        <TestMethod>
        Public Sub TreeBackedMapIsSorted_AndThreadSafeVariantsWork()
            Dim m = MultiDimensionalMap(Of Integer, Integer, String).newTreeBackedMap()
            m.put(2, 1, "c")
            m.put(1, 2, "b")
            m.put(1, 1, "a")
            assertEquals(New String() {"a", "b", "c"}, m.Values.ToArray())

            Dim ts = MultiDimensionalMap(Of Integer, Integer, String).newThreadSafeTreeBackedMap()
            ts.put(9, 9, "z")
            ts.put(0, 0, "a")
            assertEquals(New String() {"a", "z"}, ts.Values.ToArray())

            Dim th = MultiDimensionalMap(Of Integer, Integer, String).newThreadSafeHashBackedMap()
            th.put(1, 1, "x")
            assertEquals("x", th.[get](1, 1))
            Dim copy As New MultiDimensionalMap(Of Integer, Integer, String)()
            copy.putAll(th)
            assertEquals(th, copy)
        End Sub

        <TestMethod>
        Public Sub MultiDimensionalSet_AddContainsByTwoValues()
            Dim s = MultiDimensionalSet(Of String, Integer).hashSet()
            assertTrue(s.add("a", 1))
            assertFalse(s.add("a", 1))
            assertTrue(s.Add(New Pair(Of String, Integer)("b", 2)))
            assertTrue(s.contains("a", 1))
            assertFalse(s.contains("a", 2))
            assertEquals(2, s.size())
            assertTrue(s.remove("a", 1))
            assertEquals(1, s.Count)
            assertTrue(s.containsAll(New Pair(Of String, Integer)() {New Pair(Of String, Integer)("b", 2)}))
            assertTrue(s.addAll(New Pair(Of String, Integer)() {New Pair(Of String, Integer)("c", 3), New Pair(Of String, Integer)("b", 2)}))
            assertEquals(2, s.Count)

            Dim sorted = MultiDimensionalSet(Of Integer, Integer).treeSet()
            sorted.add(2, 0)
            sorted.add(1, 5)
            sorted.add(1, 2)
            assertEquals(New Integer() {1, 1, 2}, sorted.Select(Function(p) p.First).ToArray())
            assertEquals(New Integer() {2, 5, 0}, sorted.Select(Function(p) p.Second).ToArray())

            Dim concurrent = MultiDimensionalSet(Of Integer, Integer).concurrentSkipListSet()
            concurrent.add(1, 1)
            assertTrue(concurrent.contains(1, 1))
        End Sub

    End Class

    <TestClass>
    Public Class CompactHeapStringListTest

        <TestMethod>
        Public Sub AddGetIndexOfAcrossReallocations()
            ' tiny block sizes force the internal arrays to grow repeatedly
            Dim list As New CompactHeapStringList(16, 16)
            For i As Integer = 0 To 999
                assertTrue(list.add("string number " & i))
            Next i
            assertEquals(1000, list.Count)
            assertEquals("string number 0", list(0))
            assertEquals("string number 999", list.[get](999))
            assertEquals(500, list.IndexOf("string number 500"))
            assertEquals(500, list.lastIndexOf("string number 500"))
            assertEquals(-1, list.IndexOf("nope"))
            assertTrue(list.Contains("string number 7"))
            assertFalse(list.Contains("string number 1000"))
            Dim viaEnumeration As Integer = 0
            For Each s In list
                viaEnumeration += 1
            Next
            assertEquals(1000, viaEnumeration)
            assertEquals(New String() {"string number 10", "string number 11"}, list.subList(10, 12).ToArray())
            assertThrows(Of ArgumentOutOfRangeException)(Function() list(1000))
            assertThrows(Of NotSupportedException)(Sub() list.Insert(0, "x"))
            assertThrows(Of NotSupportedException)(Sub() list.RemoveAt(0))
            assertThrows(Of NotSupportedException)(Sub() list(0) = "x")
            Dim other As New CompactHeapStringList()
            other.addAll(list)
            assertEquals(list, other)
            assertEquals(list.GetHashCode(), other.GetHashCode())
            list.Clear()
            assertTrue(list.isEmpty())
            assertEquals(1000, other.size())
        End Sub

        <TestMethod>
        Public Sub EmptyStringsAndDefaultsWork()
            Dim list As New CompactHeapStringList()
            list.add("")
            list.add("a")
            list.add("")
            assertEquals(3, list.Count)
            assertEquals("", list(2))
            assertEquals(0, list.IndexOf(""))
            assertEquals(2, list.lastIndexOf(""))
            assertEquals("[, a, ]", list.ToString())
        End Sub

    End Class

End Namespace
