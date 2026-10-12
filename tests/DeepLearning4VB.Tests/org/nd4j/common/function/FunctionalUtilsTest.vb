Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.function
Imports DeepLearning4VB.org.nd4j.common.primitives

Namespace org.nd4j.common.function

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class FunctionalUtilsTest

        <TestMethod>
        Public Sub testCoGroup()
            Dim leftMap As IList(Of Pair(Of String, String)) = New List(Of Pair(Of String, String))()
            Dim rightMap As IList(Of Pair(Of String, String)) = New List(Of Pair(Of String, String))()

            leftMap.Add(Pair.of("cat", "adam"))
            leftMap.Add(Pair.of("dog", "adam"))

            rightMap.Add(Pair.of("fish", "alex"))
            rightMap.Add(Pair.of("cat", "alice"))
            rightMap.Add(Pair.of("dog", "steve"))

            '[(fish,([],[alex])), (dog,([adam],[steve])), (cat,([adam],[alice]))]
            Dim assertion As IDictionary(Of String, Pair(Of IList(Of String), IList(Of String))) = New Dictionary(Of String, Pair(Of IList(Of String), IList(Of String)))()
            assertion("cat") = New Pair(Of IList(Of String), IList(Of String))(New List(Of String) From {"adam"}, New List(Of String) From {"alice"})
            assertion("dog") = New Pair(Of IList(Of String), IList(Of String))(New List(Of String) From {"adam"}, New List(Of String) From {"steve"})
            assertion("fish") = New Pair(Of IList(Of String), IList(Of String))(New List(Of String)(), New List(Of String) From {"alex"})

            Dim cogroup As IDictionary(Of String, Pair(Of IList(Of String), IList(Of String))) = FunctionalUtils.cogroup(leftMap, rightMap)
            assertEquals(3, cogroup.Count)
            For Each key In assertion.Keys
                assertTrue(cogroup.ContainsKey(key), key)
                assertEquals(assertion(key).First, cogroup(key).First, key & " left")
                assertEquals(assertion(key).Second, cogroup(key).Second, key & " right")
            Next
        End Sub

        <TestMethod>
        Public Sub testGroupBy()
            Dim list As IList(Of Pair(Of Integer, Integer)) = New List(Of Pair(Of Integer, Integer))()
            For i As Integer = 0 To 9
                For j As Integer = 0 To 4
                    list.Add(Pair.of(i, j))
                Next j
            Next i

            Dim integerIterableMap As IDictionary(Of Integer, IList(Of Integer)) = FunctionalUtils.groupByKey(list)
            assertEquals(10, integerIterableMap.Keys.Count)
            assertEquals(5, integerIterableMap(0).Count)
        End Sub

        <TestMethod>
        Public Sub testMapToPair()
            Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
            For i As Integer = 0 To 4
                map(i.ToString()) = i.ToString()
            Next i

            Dim pairs As IList(Of Pair(Of String, String)) = FunctionalUtils.mapToPair(map)
            assertEquals(map.Count, pairs.Count)
            For Each p In pairs
                assertEquals(map(p.First), p.Second)
            Next
        End Sub

    End Class

End Namespace
