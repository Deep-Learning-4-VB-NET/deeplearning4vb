Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.split
Imports DeepLearning4VB.org.datavec.api.util.files

Namespace org.datavec.api.split

    ''' <summary>Ported from the DataVec JUnit test of the same name.</summary>
    <TestClass>
    Public Class NumberedFileInputSplitTests

        <TestMethod>
        Public Sub testNumberedFileInputSplitBasic()
            Dim baseString As String = "/path/to/files/prefix%d.suffix"
            Dim minIdx As Integer = 0
            Dim maxIdx As Integer = 10
            runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitVaryIndeces()
            Dim baseString As String = "/path/to/files/prefix-%d.suffix"
            Dim minIdx As Integer = 3
            Dim maxIdx As Integer = 27
            runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitBasicNoPrefix()
            Dim baseString As String = "/path/to/files/%d.suffix"
            Dim minIdx As Integer = 0
            Dim maxIdx As Integer = 10
            runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithLeadingZeroes()
            Dim baseString As String = "/path/to/files/prefix-%07d.suffix"
            Dim minIdx As Integer = 0
            Dim maxIdx As Integer = 10
            runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithLeadingZeroesNoSuffix()
            Dim baseString As String = "/path/to/files/prefix-%d"
            Dim minIdx As Integer = 0
            Dim maxIdx As Integer = 10
            runNumberedFileInputSplitTest(baseString, minIdx, maxIdx)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithLeadingSpaces()
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim baseString As String = "/path/to/files/prefix-%5d.suffix"
                                                   runNumberedFileInputSplitTest(baseString, 0, 10)
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithNoLeadingZeroInPadding()
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim baseString As String = "/path/to/files/prefix%5d.suffix"
                                                   runNumberedFileInputSplitTest(baseString, 0, 10)
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithLeadingPlusInPadding()
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim baseString As String = "/path/to/files/prefix%+5d.suffix"
                                                   runNumberedFileInputSplitTest(baseString, 0, 10)
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithLeadingMinusInPadding()
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim baseString As String = "/path/to/files/prefix%-5d.suffix"
                                                   runNumberedFileInputSplitTest(baseString, 0, 10)
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithTwoDigitsInPadding()
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim baseString As String = "/path/to/files/prefix%011d.suffix"
                                                   runNumberedFileInputSplitTest(baseString, 0, 10)
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithInnerZerosInPadding()
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim baseString As String = "/path/to/files/prefix%101d.suffix"
                                                   runNumberedFileInputSplitTest(baseString, 0, 10)
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testNumberedFileInputSplitWithRepeatInnerZerosInPadding()
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim baseString As String = "/path/to/files/prefix%0505d.suffix"
                                                   runNumberedFileInputSplitTest(baseString, 0, 10)
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testNullBaseString()
            assertThrows(Of ArgumentException)(Sub() runNumberedFileInputSplitTest(Nothing, 0, 10))
        End Sub

        <TestMethod>
        Public Sub testPathIteratorAndUriScheme()
            Dim split As New NumberedFileInputSplit("/path/to/files/prefix-%03d.csv", 8, 10)
            assertEquals(3L, split.length())
            Dim paths As New List(Of String)()
            Dim iter As IEnumerator(Of String) = split.locationsPathIterator()
            Do While iter.MoveNext()
                paths.Add(iter.Current)
            Loop
            assertEquals(Arrays.asList("/path/to/files/prefix-008.csv", "/path/to/files/prefix-009.csv", "/path/to/files/prefix-010.csv"), paths)

            Dim uris As New List(Of Uri)()
            Dim uriIter As IEnumerator(Of Uri) = split.locationsIterator()
            Do While uriIter.MoveNext()
                uris.Add(uriIter.Current)
            Loop
            assertEquals(3, uris.Count)
            assertTrue(uris(0).IsFile)

            'A base string with a scheme is kept as a URI
            Dim withScheme As New NumberedFileInputSplit("hdfs://host/data/part-%05d", 0, 1)
            Dim locs() As Uri = withScheme.locations()
            assertEquals("hdfs://host/data/part-00000", locs(0).AbsoluteUri)
            assertEquals("hdfs://host/data/part-00001", locs(1).AbsoluteUri)
            assertTrue(withScheme.resetSupported())
            withScheme.reset()
        End Sub

        Private Shared Sub runNumberedFileInputSplitTest(ByVal baseString As String, ByVal minIdx As Integer, ByVal maxIdx As Integer)
            Dim split As New NumberedFileInputSplit(baseString, minIdx, maxIdx)
            Dim locs() As Uri = split.locations()
            assertEquals(locs.Length, (maxIdx - minIdx) + 1)
            Dim j As Integer = 0
            For i As Integer = minIdx To maxIdx
                Dim path As String = URIUtil.getPath(locs(j))
                j += 1
                Dim exp As String = JavaString.format(baseString, i)
                Dim msg As String = exp & " vs " & path
                assertTrue(path.EndsWith(exp, StringComparison.Ordinal), msg) 'Note: on Windows the drive is prepended to the path - "/C:/"
            Next i
        End Sub

    End Class

End Namespace
