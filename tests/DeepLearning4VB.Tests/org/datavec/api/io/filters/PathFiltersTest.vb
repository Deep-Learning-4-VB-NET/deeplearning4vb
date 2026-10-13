Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.io.filters
Imports DeepLearning4VB.org.datavec.api.io.labels
Imports DeepLearning4VB.org.datavec.api.writable
Imports DVW = DeepLearning4VB.org.datavec.api.writable

Namespace org.datavec.api.io.filters

    ''' <summary>Path filters (random, balanced) and the label generators they rely on.</summary>
    <TestClass>
    Public Class PathFiltersTest

        Private Shared Function U(ByVal path As String) As Uri
            Return New Uri("file:///C:/data/" & path)
        End Function

        Private Shared ReadOnly SamplePaths() As Uri = {
            U("cat/1.jpg"), U("cat/2.jpg"), U("cat/3.png"),
            U("dog/1.jpg"), U("dog/2.jpg"), U("dog/3.jpg"), U("dog/4.jpg"), U("dog/5.txt"),
            U("fish/1.jpg")}

        <TestMethod>
        Public Sub testRandomPathFilterIsAPermutation()
            Dim filter As New RandomPathFilter(New Random(12345))
            Dim result() As Uri = filter.filter(SamplePaths)
            assertEquals(SamplePaths.Length, result.Length)
            assertEquals(New HashSet(Of Uri)(SamplePaths), New HashSet(Of Uri)(result))
            ' the same seed gives the same order
            Dim again() As Uri = New RandomPathFilter(New Random(12345)).filter(SamplePaths)
            assertArrayEquals(result, again)
        End Sub

        <TestMethod>
        Public Sub testRandomPathFilterExtensionsAndMaxPaths()
            Dim jpgOnly() As Uri = New RandomPathFilter(New Random(1), "jpg").filter(SamplePaths)
            assertEquals(7, jpgOnly.Length)
            assertTrue(jpgOnly.All(Function(p) p.ToString().EndsWith(".jpg")))

            Dim jpgOrPng() As Uri = New RandomPathFilter(New Random(1), New String() {"jpg", "png"}, 0).filter(SamplePaths)
            assertEquals(8, jpgOrPng.Length)

            Dim capped() As Uri = New RandomPathFilter(New Random(1), New String() {"jpg"}, 3).filter(SamplePaths)
            assertEquals(3, capped.Length)
            assertTrue(capped.All(Function(p) p.ToString().EndsWith(".jpg")))

            Dim all() As Uri = New RandomPathFilter(New Random(1), New String() {}).filter(SamplePaths)
            assertEquals(SamplePaths.Length, all.Length)
        End Sub

        <TestMethod>
        Public Sub testBalancedPathFilterBalancesLabels()
            Dim filter As New BalancedPathFilter(New Random(42), New String() {"jpg", "png"}, New ParentPathLabelGenerator())
            Dim result() As Uri = filter.filter(SamplePaths)
            ' the smallest label (fish) has one path, so one path per label is kept, interleaved by label
            assertEquals(3, result.Length)
            Dim labels As List(Of String) = result.Select(Function(p) New ParentPathLabelGenerator().getLabelForPath(p).ToString()).ToList()
            assertEquals(New HashSet(Of String) From {"cat", "dog", "fish"}, New HashSet(Of String)(labels))
        End Sub

        <TestMethod>
        Public Sub testBalancedPathFilterLimits()
            Dim gen As New ParentPathLabelGenerator()
            ' only the requested labels
            Dim catsAndDogs() As Uri = New BalancedPathFilter(New Random(7), New String() {"jpg"}, gen, 0, 0, 0, 0, "cat", "dog").filter(SamplePaths)
            assertEquals(4, catsAndDogs.Length) ' 2 cat jpgs -> 2 per label
            assertEquals(2, catsAndDogs.Count(Function(p) gen.getLabelForPath(p).ToString() = "cat"))
            assertEquals(2, catsAndDogs.Count(Function(p) gen.getLabelForPath(p).ToString() = "dog"))

            ' maxPathsPerLabel caps every label
            Dim capped() As Uri = New BalancedPathFilter(New Random(7), New String() {"jpg"}, gen, 1).filter(New Uri() {U("cat/1.jpg"), U("cat/2.jpg"), U("dog/1.jpg"), U("dog/2.jpg")})
            assertEquals(2, capped.Length)

            ' maxLabels keeps only the first labels seen (after the shuffle)
            Dim oneLabel() As Uri = New BalancedPathFilter(New Random(7), New String() {"jpg"}, gen, 1, 1).filter(SamplePaths)
            assertEquals(1, oneLabel.Length)

            ' minPathsPerLabel can produce an unbalanced result when a label has too few paths
            Dim unbalanced() As Uri = New BalancedPathFilter(New Random(7), New String() {"jpg"}, gen, 0, 0, 2, 0).filter(New Uri() {U("cat/1.jpg"), U("dog/1.jpg"), U("dog/2.jpg")})
            assertEquals(3, unbalanced.Length)

            ' no label generator: the parent directory is the label
            Dim defaultGen() As Uri = New BalancedPathFilter(New Random(7), Nothing, 2).filter(SamplePaths)
            assertEquals(3, defaultGen.Length)
        End Sub

        <TestMethod>
        Public Sub testParentPathLabelGenerator()
            Dim gen As New ParentPathLabelGenerator()
            assertEquals(New Text("cat"), gen.getLabelForPath("C:\data\cat\1.jpg"))
            assertEquals(New Text("cat"), gen.getLabelForPath("/data/cat/1.jpg"))
            assertEquals(New Text("cat"), gen.getLabelForPath(U("cat/1.jpg")))
            assertTrue(gen.inferLabelClasses())
            assertEquals(WritableType.Text, gen.getLabelForPath("a/b/c.txt").getType())
        End Sub

        <TestMethod>
        Public Sub testPatternPathLabelGenerator()
            Dim byPos0 As New PatternPathLabelGenerator("_")
            assertEquals(New Text("dog"), byPos0.getLabelForPath("C:\x\dog_12.png"))
            Dim byPos1 As New PatternPathLabelGenerator("_", 1)
            assertEquals(New Text("12"), byPos1.getLabelForPath("C:\x\dog_12.png"))
            assertEquals(New Text("12"), byPos1.getLabelForPath(U("x/dog_12.png")))
            ' the pattern is a regex
            Dim regex As New PatternPathLabelGenerator("[-_]", 2)
            assertEquals(New Text("c"), regex.getLabelForPath("a-b_c.wav"))
            assertTrue(regex.inferLabelClasses())
            assertThrows(Of IndexOutOfRangeException)(Sub() byPos1.getLabelForPath("nounderscore.png"))
        End Sub

    End Class

End Namespace
