Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4vb.common.io

Namespace org.nd4vb.common.io

    <TestClass>
    Public Class CollectionUtilsTest

        <TestMethod>
        Public Sub Emptiness()
            assertTrue(CollectionUtils.isEmpty(DirectCast(Nothing, IEnumerable)))
            assertTrue(CollectionUtils.isEmpty(New List(Of Integer)()))
            assertFalse(CollectionUtils.isEmpty(New HashSet(Of Integer) From {1}))
            assertTrue(CollectionUtils.isEmpty(New Dictionary(Of String, String)()))
            assertFalse(CollectionUtils.isEmpty(New Dictionary(Of String, String) From {{"a", "b"}}))
            assertTrue(CollectionUtils.isEmpty(New LinkedHashMap(Of String, String)()))
        End Sub

        <TestMethod>
        Public Sub MergingAndConversion()
            assertEquals(New Object() {1, 2}, CollectionUtils.arrayToList(New Integer() {1, 2}))
            Dim target As New List(Of Integer) From {0}
            CollectionUtils.mergeArrayIntoCollection(New Integer() {1, 2}, target)
            assertEquals(New Integer() {0, 1, 2}, target)
            Dim strings As New List(Of String)()
            CollectionUtils.mergeArrayIntoCollection(New String() {"a"}, strings)
            assertEquals(New String() {"a"}, strings)
            assertThrows(Of ArgumentException)(Sub() CollectionUtils.mergeArrayIntoCollection(New Integer() {1}, DirectCast(Nothing, ICollection(Of Integer))))
            Dim props As New Properties()
            props.setProperty("k", "v")
            Dim map As New Dictionary(Of String, String)()
            CollectionUtils.mergePropertiesIntoMap(props, map)
            assertEquals("v", map("k"))
            Dim objectMap As New Dictionary(Of String, Object)()
            CollectionUtils.mergePropertiesIntoMap(props, objectMap)
            assertEquals("v", objectMap("k"))
            assertThrows(Of ArgumentException)(Sub() CollectionUtils.mergePropertiesIntoMap(props, DirectCast(Nothing, IDictionary(Of String, String))))
        End Sub

        <TestMethod>
        Public Sub ContainsHelpers()
            Dim list As New List(Of String) From {"a", "b"}
            assertTrue(CollectionUtils.contains(list.GetEnumerator(), "b"))
            assertFalse(CollectionUtils.contains(list.GetEnumerator(), "z"))
            assertFalse(CollectionUtils.contains(Nothing, "a"))
            Dim instance As New Object()
            assertTrue(CollectionUtils.containsInstance(New Object() {1, instance}, instance))
            assertFalse(CollectionUtils.containsInstance(New Object() {New Object()}, instance))
            assertTrue(CollectionUtils.containsAny(list, New String() {"z", "b"}))
            assertFalse(CollectionUtils.containsAny(list, New String() {"z"}))
            assertFalse(CollectionUtils.containsAny(list, New String() {}))
            assertEquals("b", CollectionUtils.findFirstMatch(list, New String() {"z", "b", "a"}))
            assertNull(CollectionUtils.findFirstMatch(list, New String() {"z"}))
        End Sub

        <TestMethod>
        Public Sub TypeQueries()
            Dim mixed As New List(Of Object) From {"a", 1, "b"}
            assertEquals(1, CollectionUtils.findValueOfType(mixed, GetType(Integer)))
            assertNull(CollectionUtils.findValueOfType(mixed, GetType(String)))
            assertEquals(1, CollectionUtils.findValueOfType(Of Integer)(mixed))
            assertEquals(1, CollectionUtils.findValueOfType(mixed, New Type() {GetType(Double), GetType(Integer)}))
            assertNull(CollectionUtils.findValueOfType(mixed, New Type() {}))
            assertNull(CollectionUtils.findCommonElementType(mixed))
            assertEquals(GetType(String), CollectionUtils.findCommonElementType(New List(Of Object) From {"a", Nothing, "b"}))
            assertNull(CollectionUtils.findCommonElementType(New List(Of String)()))
            Dim same As New Object()
            assertTrue(CollectionUtils.hasUniqueObject(New Object() {same, same}))
            assertFalse(CollectionUtils.hasUniqueObject(New Object() {same, New Object()}))
            assertTrue(CollectionUtils.hasUniqueObject(New Integer() {7, 7}))
            assertFalse(CollectionUtils.hasUniqueObject(New Integer() {}))
        End Sub

        <TestMethod>
        Public Sub EnumerationsAndIterators()
            Dim source As New List(Of String) From {"a", "b", "c"}
            Dim filled() As String = CollectionUtils.toArray(source.GetEnumerator(), New String(4) {})
            assertEquals(New String() {"a", "b", "c", Nothing, Nothing}, filled)
            Dim grown() As Object = CollectionUtils.toArray(source.GetEnumerator(), New Object() {})
            assertEquals(New Object() {"a", "b", "c"}, grown)
            Dim iterator As IEnumerator(Of String) = CollectionUtils.toIterator(source.GetEnumerator())
            Dim collected As New List(Of String)()
            Do While iterator.MoveNext()
                collected.Add(iterator.Current)
            Loop
            assertEquals(source, collected)
        End Sub

        <TestMethod>
        Public Sub MultiValueMaps()
            Dim backing As New Dictionary(Of String, IList(Of Integer))()
            Dim mvm = CollectionUtils.toMultiValueMap(backing)
            mvm.add("a", 1)
            mvm.add("a", 2)
            mvm.[set]("b", 3)
            assertEquals(1, mvm.getFirst("a"))
            assertEquals(0, mvm.getFirst("missing"))
            assertEquals(New Integer() {1, 2}, backing("a"))
            assertEquals(2, mvm.Count)
            assertTrue(mvm.ContainsKey("b"))
            assertEquals(New Integer() {3}, mvm("b"))
            Dim firstValues = mvm.toSingleValueMap()
            assertEquals(1, firstValues("a"))
            assertEquals(3, firstValues("b"))
            mvm.setAll(New Dictionary(Of String, Integer) From {{"c", 4}})
            assertEquals(4, mvm.getFirst("c"))
            assertTrue(mvm.Remove("c"))
            assertFalse(backing.ContainsKey("c"))

            Dim frozen = CollectionUtils.unmodifiableMultiValueMap(mvm)
            assertEquals(2, frozen.Count)
            assertEquals(1, frozen.getFirst("a"))
            assertEquals(New Integer() {1, 2}, frozen("a"))
            assertThrows(Of NotSupportedException)(Sub() frozen.add("a", 9))
            assertThrows(Of NotSupportedException)(Sub() frozen("a").Add(9))
            assertThrows(Of NotSupportedException)(Sub() frozen.Remove("a"))
            mvm.add("a", 5)
            assertEquals(New Integer() {1, 2, 5}, frozen("a"))
            assertFalse(frozen.ContainsKey("d"))
        End Sub

    End Class

End Namespace
