Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat

Namespace JavaCompat

    ''' <summary>Tests for the Java collection equivalents: LinkedHashMap/Set, PriorityQueue, synchronized wrappers, Properties.</summary>
    <TestClass>
    Public Class CollectionsCompatTest

        <TestMethod>
        Public Sub LinkedHashMap_KeepsInsertionOrder()
            Dim m As New LinkedHashMap(Of String, Integer)()
            assertEquals(0, m.put("c", 3)) ' V is a value type, so "no previous value" is the default 0
            m.put("a", 1)
            m.put("b", 2)
            assertEquals(3, m.put("c", 30))
            assertEquals(New String() {"c", "a", "b"}, m.Keys.ToArray())
            assertEquals(New Integer() {30, 1, 2}, m.Values.ToArray())
            assertEquals(30, m("c"))
            assertEquals(0, m.[get]("missing"))
            assertThrows(Of KeyNotFoundException)(Function() m("missing"))
            assertTrue(m.ContainsKey("a"))
            assertTrue(m.containsValue(2))
            assertEquals(1, m.removeAndGet("a"))
            assertFalse(m.Remove("a"))
            assertEquals(New String() {"c", "b"}, m.keySet().ToArray())
            Dim v As Integer
            assertTrue(m.TryGetValue("b", v))
            assertEquals(2, v)
            assertEquals("{c=30, b=2}", m.ToString())
            Dim copy As New LinkedHashMap(Of String, Integer)(m)
            assertEquals(m, copy)
            assertEquals(m.GetHashCode(), copy.GetHashCode())
            m.Clear()
            assertTrue(m.isEmpty())
        End Sub

        <TestMethod>
        Public Sub LinkedHashSet_KeepsInsertionOrderAndSetSemantics()
            Dim s As New LinkedHashSet(Of Integer)(New Integer() {3, 1, 2, 3, 1})
            assertEquals(New Integer() {3, 1, 2}, s.ToArray())
            assertFalse(s.Add(1))
            assertTrue(s.Add(9))
            assertTrue(s.Remove(3))
            assertEquals(New Integer() {1, 2, 9}, s.ToArray())
            assertTrue(s.IsSupersetOf(New Integer() {1, 9}))
            assertTrue(s.SetEquals(New Integer() {9, 2, 1}))
            s.IntersectWith(New Integer() {2, 9, 100})
            assertEquals(New Integer() {2, 9}, s.ToArray())
            s.UnionWith(New Integer() {5})
            s.SymmetricExceptWith(New Integer() {5, 6})
            assertEquals(New Integer() {2, 9, 6}, s.ToArray())
        End Sub

        <TestMethod>
        Public Sub PriorityQueue_PollsInComparerOrder()
            Dim pq As New PriorityQueue(Of Integer)()
            For Each x In New Integer() {5, 1, 4, 1, 3, 9, 2}
                pq.add(x)
            Next
            assertEquals(7, pq.size())
            assertEquals(1, pq.peek())
            Dim drained As New List(Of Integer)()
            Do While Not pq.isEmpty()
                drained.Add(pq.poll())
            Loop
            assertEquals(New Integer() {1, 1, 2, 3, 4, 5, 9}, drained.ToArray())
            assertEquals(0, pq.poll())
            assertThrows(Of InvalidOperationException)(Function() pq.remove())

            Dim reversed As New PriorityQueue(Of String)(Comparer(Of String).Create(Function(a, b) String.CompareOrdinal(b, a)))
            reversed.add("b")
            reversed.add("c")
            reversed.add("a")
            assertEquals("c", reversed.poll())
            assertEquals("b", reversed.poll())
            assertThrows(Of ArgumentNullException)(Sub() reversed.add(Nothing))
            assertThrows(Of ArgumentException)(Function() New PriorityQueue(Of Integer)(0))
        End Sub

        <TestMethod>
        Public Sub SynchronizedWrappers_DelegateAndSnapshotEnumeration()
            Dim d As New SynchronizedDictionary(Of String, Integer)(New SortedDictionary(Of String, Integer)())
            d("b") = 2
            d("a") = 1
            assertEquals(2, d.Count)
            assertEquals(New String() {"a", "b"}, d.Keys.ToArray())
            For Each kv In d
                d(kv.Key & "x") = kv.Value ' modifying while enumerating is safe: enumeration is a snapshot
            Next
            assertEquals(4, d.Count)
            assertTrue(d.Remove("ax"))

            Dim s As New SynchronizedSet(Of Integer)(New HashSet(Of Integer)())
            assertTrue(s.Add(1))
            assertFalse(s.Add(1))
            assertTrue(s.Contains(1))
            s.UnionWith(New Integer() {2, 3})
            assertEquals(3, s.Count)
            assertTrue(s.SetEquals(New Integer() {1, 2, 3}))
        End Sub

        <TestMethod>
        Public Sub Properties_LoadParsesJavaPropertiesFormat()
            Dim text As String = String.Join(ControlChars.Lf, {
                "# a comment",
                "! another comment",
                "key1=value1",
                "key2 : value 2",
                "key3 value3",
                "   spaced   =   trimmed   ",
                "multi=first \",
                "      second",
                "esc\ key=tab\there",
                "unicode=AB",
                "empty=",
                ""})
            Dim p As New Properties()
            p.load(New StringReader(text))
            assertEquals(8, p.Count)
            assertEquals("value1", p.getProperty("key1"))
            assertEquals("value 2", p.getProperty("key2"))
            assertEquals("value3", p.getProperty("key3"))
            assertEquals("trimmed   ", p.getProperty("spaced"))
            assertEquals("first second", p.getProperty("multi"))
            assertEquals("tab" & ControlChars.Tab & "here", p.getProperty("esc key"))
            assertEquals("AB", p.getProperty("unicode"))
            assertEquals("", p.getProperty("empty"))
            assertNull(p.getProperty("missing"))
            assertEquals("dflt", p.getProperty("missing", "dflt"))
        End Sub

        <TestMethod>
        Public Sub Properties_StoreThenLoad_RoundTrips()
            Dim p As New Properties()
            p.setProperty("plain", "value")
            p.setProperty("needs=escaping:here", "a=b # c")
            p.setProperty("tabs", "x" & ControlChars.Tab & "y")
            p.setProperty("unicode", "é")
            Dim sw As New StringWriter()
            p.store(sw, "header")
            Dim text As String = sw.ToString()
            assertTrue(text.StartsWith("#header"))
            Dim q As New Properties()
            q.load(New StringReader(text))
            assertEquals(p.toDictionary(), q.toDictionary())
            assertEquals(4, q.stringPropertyNames().Count)
        End Sub

        <TestMethod>
        Public Sub Properties_DefaultsAreConsulted()
            Dim defaults As New Properties()
            defaults.setProperty("a", "1")
            Dim p As New Properties(defaults)
            p.setProperty("b", "2")
            assertEquals("1", p.getProperty("a"))
            assertEquals("2", p.getProperty("b"))
            assertFalse(p.containsKey("a"))
            assertEquals(New String() {"a", "b"}, p.stringPropertyNames().OrderBy(Function(x) x).ToArray())
            assertEquals("2", p.remove("b"))
            assertNull(p.remove("b"))
            assertThrows(Of ArgumentNullException)(Sub() p.setProperty("x", Nothing))
        End Sub

    End Class

End Namespace
