Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.split
Imports FileStreamCreatorFunction = DeepLearning4VB.org.datavec.api.split.streams.FileStreamCreatorFunction
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports RandomPathFilter = DeepLearning4VB.org.datavec.api.io.filters.RandomPathFilter

Namespace org.datavec.api.split

    ''' <summary>
    ''' The input splits not covered by the ported JUnit tests: <see cref="FileSplit"/> over files and
    ''' directories, <see cref="StringSplit"/>, <see cref="ListStringSplit"/>, <see cref="CollectionInputSplit"/>,
    ''' <see cref="InputStreamInputSplit"/>, <see cref="OutputStreamInputSplit"/> and <see cref="StreamInputSplit"/>.
    ''' </summary>
    <TestClass>
    Public Class InputSplitsTest

        Private tmpDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            tmpDir = Path.Combine(Path.GetTempPath(), "dl4vb-tests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tmpDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(tmpDir) Then Directory.Delete(tmpDir, True)
        End Sub

        Private Function writeFile(ByVal relativePath As String, ByVal content As String) As String
            Dim target As String = Path.Combine(tmpDir, relativePath)
            Directory.CreateDirectory(Path.GetDirectoryName(target))
            System.IO.File.WriteAllText(target, content, New UTF8Encoding(False))
            Return target
        End Function

        ''' <summary>The local file paths of a split's locations, sorted.</summary>
        Private Shared Function filePaths(ByVal split As InputSplit) As List(Of String)
            Dim result As New List(Of String)()
            For Each u As Uri In split.locations()
                result.Add(URIUtil.toFilePath(u))
            Next u
            result.Sort(StringComparer.OrdinalIgnoreCase)
            Return result
        End Function

        Private Shared Function drain(Of T)(ByVal iter As IEnumerator(Of T)) As List(Of T)
            Dim result As New List(Of T)()
            Do While iter.MoveNext()
                result.Add(iter.Current)
            Loop
            Return result
        End Function

        ' ---- FileSplit ----

        <TestMethod>
        Public Sub testFileSplitOverDirectory()
            Dim a As String = writeFile("a.txt", "aaa")
            Dim b As String = writeFile("b.csv", "b")
            Dim c As String = writeFile("sub\c.txt", "cc")
            Dim d As String = writeFile("sub\deep\d.csv", "dddd")

            'recursive by default: every file under the directory, one unit of length per file
            Dim split As New FileSplit(tmpDir)
            assertEquals(4L, split.length())
            assertEquals(4, split.locations().Length)
            Dim expected As New List(Of String)(New String() {a, b, c, d})
            expected.Sort(StringComparer.OrdinalIgnoreCase)
            assertEquals(expected, filePaths(split))
            For Each u As Uri In split.locations()
                assertTrue(u.IsAbsoluteUri AndAlso u.IsFile, u.ToString())
            Next u
            assertEquals(tmpDir, split.getRootDir())
            assertEquals(tmpDir, split.RootDir)
            assertFalse(split.needsBootstrapForWrite())
            assertTrue(split.resetSupported())
            split.reset()

            'the path iterator yields the URI strings, the location iterator the URIs
            Dim pathStrings As List(Of String) = drain(split.locationsPathIterator())
            assertEquals(4, pathStrings.Count)
            assertTrue(pathStrings.All(Function(s) s.StartsWith("file:///", StringComparison.Ordinal)))
            Dim uris As List(Of Uri) = drain(split.locationsIterator())
            assertEquals(split.locations().ToList(), uris)

            'non-recursive: only the top-level files
            Dim flat As New FileSplit(tmpDir, Nothing, False)
            assertEquals(2L, flat.length())
            assertEquals(New List(Of String)(New String() {a, b}), filePaths(flat))
        End Sub

        <TestMethod>
        Public Sub testFileSplitAllowedFormats()
            Dim a As String = writeFile("a.txt", "a")
            writeFile("b.csv", "b")
            Dim c As String = writeFile("sub\c.txt", "c")
            Dim upper As String = writeFile("sub\D.TXT", "d")
            Dim noDot As String = writeFile("sub\notes-txt", "e")

            'formats are suffixes matched case-sensitively, with no dot implied (commons-io SuffixFileFilter)
            Dim txt As New FileSplit(tmpDir, New String() {"txt"})
            assertEquals(New List(Of String)(New String() {a, c, noDot}), filePaths(txt))
            assertEquals(3L, txt.length())
            Dim txtAndCsv As New FileSplit(tmpDir, New String() {".txt", ".csv"})
            assertEquals(3L, txtAndCsv.length())
            Dim upperOnly As New FileSplit(tmpDir, New String() {"TXT"})
            assertEquals(New List(Of String)(New String() {upper}), filePaths(upperOnly))
            Dim none As New FileSplit(tmpDir, New String() {"png"})
            assertEquals(0L, none.length())
            assertEquals(0, none.locations().Length)
            assertTrue(none.needsBootstrapForWrite())
            'formats combined with no recursion
            Dim topTxt As New FileSplit(tmpDir, New String() {"txt"}, False)
            assertEquals(New List(Of String)(New String() {a}), filePaths(topTxt))
            'an empty format list matches nothing
            assertEquals(0L, New FileSplit(tmpDir, New String() {}).length())
        End Sub

        <TestMethod>
        Public Sub testFileSplitOverSingleFile()
            Dim csv As String = writeFile("single.csv", "0123456789")
            Dim split As New FileSplit(csv)
            'a single file: one location and the length is the file size in bytes
            assertEquals(1, split.locations().Length)
            assertEquals(URIUtil.fileToURI(csv), split.locations()(0))
            assertEquals(10L, split.length())
            assertEquals(csv, split.getRootDir())
            assertFalse(split.needsBootstrapForWrite())
            assertEquals(Arrays.asList(URIUtil.fileToURI(csv).AbsoluteUri), drain(split.locationsPathIterator()))
            'the file split can read its own location, as a URI string or a plain path
            Using s As Stream = split.openInputStreamFor(URIUtil.fileToURI(csv).AbsoluteUri)
                assertEquals("0123456789", New StreamReader(s).ReadToEnd())
            End Using
            Using s As Stream = split.openInputStreamFor(csv)
                assertEquals("0123456789", New StreamReader(s).ReadToEnd())
            End Using
        End Sub

        <TestMethod>
        Public Sub testFileSplitCreatesMissingAbsoluteFile()
            Dim missing As String = Path.Combine(tmpDir, "new-file.txt")
            assertFalse(File.Exists(missing))
            'a rooted path that does not exist is created as an empty file (java.io.File.createNewFile)
            Dim split As New FileSplit(missing)
            assertTrue(File.Exists(missing))
            assertEquals(0L, New FileInfo(missing).Length)
            assertEquals(1, split.locations().Length)
            assertEquals(URIUtil.fileToURI(missing), split.locations()(0))
            'the length is not updated in that case (as in Java)
            assertEquals(0L, split.length())
            assertFalse(split.needsBootstrapForWrite())

            'a relative path that does not exist is an error, and so is a null path
            Dim ex As ArgumentException = assertThrows(Of ArgumentException)(Sub()
                                                                                Dim ignored As New FileSplit("no-such-dir-" & Guid.NewGuid().ToString("N") & "\file.txt")
                                                                            End Sub)
            assertTrue(ex.Message.StartsWith("No such file or directory: ", StringComparison.Ordinal), ex.Message)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New FileSplit(DirectCast(Nothing, String))
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testFileSplitRandomization()
            For i As Integer = 0 To 19
                writeFile("f" & i.ToString("00") & ".txt", i.ToString())
            Next i
            Dim ordered As New FileSplit(tmpDir)
            Dim plain As List(Of String) = drain(ordered.locationsPathIterator())

            Dim shuffled As New FileSplit(tmpDir, New Random(42))
            Dim order1 As List(Of String) = drain(shuffled.locationsPathIterator())
            'same files, different order
            assertEquals(20, order1.Count)
            assertEquals(New HashSet(Of String)(plain), New HashSet(Of String)(order1))
            assertNotEquals(plain, order1)
            'the iteration order is stable until reset() reshuffles it
            assertEquals(order1, drain(shuffled.locationsPathIterator()))
            shuffled.reset()
            Dim order2 As List(Of String) = drain(shuffled.locationsPathIterator())
            assertEquals(New HashSet(Of String)(plain), New HashSet(Of String)(order2))
            assertNotEquals(order1, order2)
            'locations() are not affected by the shuffle
            assertEquals(plain, drain(ordered.locationsPathIterator()))
            assertEquals(ordered.locations().ToList(), shuffled.locations().ToList())
            'the same seed gives the same order
            Dim again As New FileSplit(tmpDir, New Random(42))
            assertEquals(order1, drain(again.locationsPathIterator()))
            'the URI iterator follows the shuffled order as well
            Dim uris As List(Of Uri) = drain(shuffled.locationsIterator())
            assertEquals(order2, uris.Select(Function(u) u.AbsoluteUri).ToList())
            'randomization with formats
            Dim withFormats As New FileSplit(tmpDir, New String() {"txt"}, New Random(1))
            assertEquals(20L, withFormats.length())
            assertEquals(New HashSet(Of String)(plain), New HashSet(Of String)(drain(withFormats.locationsPathIterator())))
        End Sub

        <TestMethod>
        Public Sub testFileSplitAddNewLocation()
            Dim split As New FileSplit(tmpDir)
            assertEquals(0L, split.length())
            assertTrue(split.needsBootstrapForWrite())

            'a generated location is a new empty file inside the directory
            Dim generated As String = split.addNewLocation()
            Dim generatedPath As String = URIUtil.toFilePath(New Uri(generated))
            assertTrue(File.Exists(generatedPath))
            assertEquals(tmpDir, Path.GetDirectoryName(generatedPath))
            assertEquals(1L, split.length())
            assertEquals(generated, split.locations()(0).AbsoluteUri)
            assertFalse(split.needsBootstrapForWrite())

            'an explicit location is created too, and an existing one is kept as it is
            Dim explicitPath As String = Path.Combine(tmpDir, "explicit.txt")
            Dim explicitUri As String = URIUtil.fileToURI(explicitPath).AbsoluteUri
            assertEquals(explicitUri, split.addNewLocation(explicitUri))
            assertTrue(File.Exists(explicitPath))
            assertEquals(2L, split.length())
            File.WriteAllText(explicitPath, "kept")
            split.addNewLocation(explicitUri)
            assertEquals("kept", File.ReadAllText(explicitPath))
            assertEquals(3L, split.length())

            'the new locations can be written and read through the split
            Using s As Stream = split.openOutputStreamFor(generated)
                Dim bytes() As Byte = Encoding.UTF8.GetBytes("written")
                s.Write(bytes, 0, bytes.Length)
            End Using
            assertEquals("written", File.ReadAllText(generatedPath))
            Using s As Stream = split.openInputStreamFor(generated)
                assertEquals("written", New StreamReader(s).ReadToEnd())
            End Using

            'for a single-file split the new file gets the same extension, next to the original
            Dim orig As String = writeFile("orig.csv", "x")
            Dim singleSplit As New FileSplit(orig)
            Dim sibling As String = URIUtil.toFilePath(New Uri(singleSplit.addNewLocation()))
            assertTrue(File.Exists(sibling))
            assertEquals(".csv", Path.GetExtension(sibling))
            assertEquals(tmpDir, Path.GetDirectoryName(sibling))
            assertNotEquals(orig, sibling)
            assertEquals(2, singleSplit.locations().Length)
        End Sub

        <TestMethod>
        Public Sub testFileSplitUpdateSplitLocations()
            writeFile("one.txt", "1")
            Dim split As New FileSplit(tmpDir)
            assertEquals(1, split.locations().Length)
            writeFile("two.txt", "2")
            'without reset nothing changes; with reset the directory is listed again
            split.updateSplitLocations(False)
            assertEquals(1, split.locations().Length)
            split.updateSplitLocations(True)
            assertEquals(2, split.locations().Length)
            'sampling a file split goes through the path filter and partitions the locations
            Dim samples() As InputSplit = split.sample(New RandomPathFilter(New Random(3), "txt"), 50, 50)
            assertEquals(2, samples.Length)
            assertEquals(1L, samples(0).length())
            assertEquals(1L, samples(1).length())
            Dim all() As InputSplit = split.sample(Nothing)
            assertEquals(1, all.Length)
            assertEquals(2L, all(0).length())
        End Sub

        ' ---- StringSplit and ListStringSplit ----

        <TestMethod>
        Public Sub testStringSplit()
            Dim split As New StringSplit("1,2,3,4,5")
            assertEquals("1,2,3,4,5", split.getData())
            assertEquals("1,2,3,4,5", split.Data)
            'the length is the length of the string
            assertEquals(9L, split.length())
            assertEquals(0L, New StringSplit("").length())
            'a string split has no locations
            assertEquals(0, split.locations().Length)
            assertFalse(split.locationsIterator().MoveNext())
            assertFalse(split.locationsPathIterator().MoveNext())
            assertTrue(split.canWriteToLocation(New Uri("file:///C:/x")))
            assertNull(split.addNewLocation())
            assertNull(split.addNewLocation("x"))
            assertFalse(split.needsBootstrapForWrite())
            split.bootStrapForWrite()
            split.updateSplitLocations(True)
            assertNull(split.openInputStreamFor("x"))
            assertNull(split.openOutputStreamFor("x"))
            assertTrue(split.resetSupported())
            split.reset()
            assertEquals("1,2,3,4,5", split.getData())
        End Sub

        <TestMethod>
        Public Sub testListStringSplit()
            Dim data As IList(Of IList(Of String)) = New List(Of IList(Of String))()
            data.Add(Arrays.asList("a", "b", "c"))
            data.Add(Arrays.asList("d", "e"))
            data.Add(Arrays.asList("f"))
            Dim split As New ListStringSplit(data)
            assertTrue(split.getData() Is data)
            assertTrue(split.Data Is data)
            'the length is the number of rows
            assertEquals(3L, split.length())
            assertEquals(0L, New ListStringSplit(New List(Of IList(Of String))()).length())
            assertEquals(0, split.locations().Length)
            assertFalse(split.locationsIterator().MoveNext())
            assertFalse(split.locationsPathIterator().MoveNext())
            'unlike a StringSplit it cannot be written to
            assertFalse(split.canWriteToLocation(New Uri("file:///C:/x")))
            assertNull(split.addNewLocation())
            assertNull(split.addNewLocation("x"))
            assertFalse(split.needsBootstrapForWrite())
            assertNull(split.openInputStreamFor("x"))
            assertNull(split.openOutputStreamFor("x"))
            assertTrue(split.resetSupported())
            split.reset()
            assertEquals(Arrays.asList("d", "e"), split.getData()(1))
        End Sub

        ' ---- CollectionInputSplit ----

        <TestMethod>
        Public Sub testCollectionInputSplit()
            Dim u1 As New Uri("file:///C:/data/a%20b.csv")
            Dim u2 As New Uri("hdfs://host/data/c.csv")
            Dim relative As Uri = URIUtil.createURI("relative/d.csv")
            Dim fromArray As New CollectionInputSplit(New Uri() {u1, u2, relative})
            Dim fromList As New CollectionInputSplit(DirectCast(New List(Of Uri)(New Uri() {u1, u2, relative}), ICollection(Of Uri)))

            For Each split As CollectionInputSplit In New CollectionInputSplit() {fromArray, fromList}
                assertEquals(3L, split.length())
                'the URIs round-trip through their string form, escapes and relative URIs included
                assertArrayEquals(New Uri() {u1, u2, relative}, split.locations())
                'the URI iterator treats a scheme-less string as a file path (resolved against the current directory)
                assertEquals(Arrays.asList(u1, u2, URIUtil.fileToURI("relative/d.csv")), drain(split.locationsIterator()))
                assertEquals(Arrays.asList("file:///C:/data/a%20b.csv", "hdfs://host/data/c.csv", "relative/d.csv"), drain(split.locationsPathIterator()))
                assertFalse(split.needsBootstrapForWrite())
                split.bootStrapForWrite()
                split.updateSplitLocations(True)
                assertTrue(split.resetSupported())
                split.reset()
                assertNull(split.openInputStreamFor("x"))
                assertNull(split.openOutputStreamFor("x"))
                'BaseInputSplit: only absolute locations can be written to, and locations cannot be added
                assertTrue(split.canWriteToLocation(u1))
                assertFalse(split.canWriteToLocation(relative))
                assertThrows(Of NotSupportedException)(Sub() split.addNewLocation())
                assertThrows(Of NotSupportedException)(Sub() split.addNewLocation("x"))
            Next split

            Dim empty As New CollectionInputSplit(New Uri() {})
            assertEquals(0L, empty.length())
            assertEquals(0, empty.locations().Length)
            assertFalse(empty.locationsIterator().MoveNext())
        End Sub

        <TestMethod>
        Public Sub testBaseInputSplitSampleWeights()
            Dim uris(9) As Uri
            For i As Integer = 0 To 9
                uris(i) = New Uri("file:///C:/data/" & i & ".csv")
            Next i
            Dim split As New CollectionInputSplit(uris)

            'no filter and no weights: one split with everything, in order
            Dim all() As InputSplit = split.sample(Nothing)
            assertEquals(1, all.Length)
            assertArrayEquals(uris, all(0).locations())
            'a first weight of exactly 1.0 means "no split" (Java checks weights[0] != 1.0 only), whatever follows it
            assertEquals(1, split.sample(Nothing, 1.0).Length)
            assertEquals(10L, split.sample(Nothing, 1.0)(0).length())
            assertEquals(1, split.sample(Nothing, 1, 1, 1).Length)
            assertEquals(10L, split.sample(Nothing, 1, 1, 1)(0).length())

            'weights are relative; the boundaries are rounded (Math.round) and the splits are contiguous
            Dim splits() As InputSplit = split.sample(Nothing, 60, 40)
            assertEquals(2, splits.Length)
            assertEquals(6L, splits(0).length())
            assertEquals(4L, splits(1).length())
            assertArrayEquals(uris.Take(6).ToArray(), splits(0).locations())
            assertArrayEquals(uris.Skip(6).ToArray(), splits(1).locations())
            Dim thirds() As InputSplit = split.sample(Nothing, 2, 2, 2)
            assertEquals(3, thirds.Length)
            assertEquals(3L, thirds(0).length()) 'round(3.33) = 3
            assertEquals(4L, thirds(1).length()) 'round(6.67) = 7, so 3..6
            assertEquals(3L, thirds(2).length())
            Dim quarter() As InputSplit = split.sample(Nothing, 0.25, 0.75)
            assertEquals(3L, quarter(0).length()) 'round(2.5) = 3
            assertEquals(7L, quarter(1).length())
            'a zero weight gives an empty split
            Dim withEmpty() As InputSplit = split.sample(Nothing, 0, 1)
            assertEquals(0L, withEmpty(0).length())
            assertEquals(10L, withEmpty(1).length())

            'a filter is applied first
            Dim filtered() As InputSplit = split.sample(New RandomPathFilter(New Random(5), New String() {"csv"}, 4), 50, 50)
            assertEquals(2L, filtered(0).length())
            assertEquals(2L, filtered(1).length())
            Dim allFiltered() As InputSplit = split.sample(New RandomPathFilter(New Random(5), New String() {"csv"}, 4))
            assertEquals(1, allFiltered.Length)
            assertEquals(4L, allFiltered(0).length())
        End Sub

        ' ---- InputStreamInputSplit and OutputStreamInputSplit ----

        <TestMethod>
        Public Sub testInputStreamInputSplit()
            Dim stream As New MemoryStream(Encoding.UTF8.GetBytes("a,b,c"))
            Dim split As New InputStreamInputSplit(stream, "file:///C:/data/in.csv")
            assertTrue(split.getIs() Is stream)
            assertTrue(split.InputStream Is stream)
            'the one location, as a URI and as a path
            assertEquals(1, split.locations().Length)
            assertEquals(New Uri("file:///C:/data/in.csv"), split.locations()(0))
            assertEquals(Arrays.asList(New Uri("file:///C:/data/in.csv")), drain(split.locationsIterator()))
            'the path form of the location (see FileUtilsTest for the exact string)
            assertEquals(Arrays.asList(URIUtil.getPath(New Uri("file:///C:/data/in.csv"))), drain(split.locationsPathIterator()))
            'the stream is served for any location
            assertTrue(split.openInputStreamFor("anything") Is stream)
            assertTrue(split.resetSupported())
            split.reset()
            assertTrue(split.getIs() Is stream)
            'writing is not supported
            assertFalse(split.canWriteToLocation(New Uri("file:///C:/x")))
            assertNull(split.addNewLocation())
            assertNull(split.addNewLocation("x"))
            split.updateSplitLocations(True)
            assertThrows(Of NotSupportedException)(Sub() split.needsBootstrapForWrite())
            assertThrows(Of NotSupportedException)(Sub() split.bootStrapForWrite())
            assertThrows(Of NotSupportedException)(Sub() split.openOutputStreamFor("x"))
            assertThrows(Of NotSupportedException)(Sub() split.length())
            'the stream can be replaced
            Dim other As New MemoryStream()
            split.setIs(other)
            assertTrue(split.openInputStreamFor("x") Is other)
            split.InputStream = stream
            assertTrue(split.getIs() Is stream)

            'the URI constructor
            Dim uri As Uri = URIUtil.fileToURI(Path.Combine(tmpDir, "x.csv"))
            Dim fromUri As New InputStreamInputSplit(stream, uri)
            assertEquals(uri, fromUri.locations()(0))
            assertEquals(Arrays.asList(URIUtil.getPath(uri)), drain(fromUri.locationsPathIterator()))

            'without a location: no reset, an empty path, no URI
            Dim noLocation As New InputStreamInputSplit(stream)
            assertEquals(0, noLocation.locations().Length)
            assertFalse(noLocation.resetSupported())
            assertThrows(Of NotSupportedException)(Sub() noLocation.reset())
            assertEquals(Arrays.asList(""), drain(noLocation.locationsPathIterator()))
            assertTrue(noLocation.openInputStreamFor("x") Is stream)
        End Sub

        <TestMethod>
        Public Sub testOutputStreamInputSplit()
            Dim stream As New MemoryStream()
            Dim split As New OutputStreamInputSplit(stream)
            assertTrue(split.getOutputStream() Is stream)
            assertTrue(split.OutputStream Is stream)
            'the stream is served for any location
            assertTrue(split.openOutputStreamFor("anything") Is stream)
            assertTrue(split.canWriteToLocation(New Uri("file:///C:/x")))
            assertFalse(split.needsBootstrapForWrite())
            split.bootStrapForWrite()
            assertNull(split.addNewLocation())
            assertEquals(0, split.locations().Length)
            'reading, iteration, length, reset and named locations are not supported
            assertThrows(Of NotSupportedException)(Sub() split.openInputStreamFor("x"))
            assertThrows(Of NotSupportedException)(Sub() split.addNewLocation("x"))
            assertThrows(Of NotSupportedException)(Sub() split.updateSplitLocations(True))
            assertThrows(Of NotSupportedException)(Sub() split.length())
            assertThrows(Of NotSupportedException)(Sub() split.locationsIterator())
            assertThrows(Of NotSupportedException)(Sub() split.locationsPathIterator())
            assertFalse(split.resetSupported())
            assertThrows(Of NotSupportedException)(Sub() split.reset())
            'the stream can be replaced
            Dim other As New MemoryStream()
            split.setOutputStream(other)
            assertTrue(split.openOutputStreamFor("x") Is other)
            split.OutputStream = stream
            assertTrue(split.getOutputStream() Is stream)
        End Sub

        ' ---- StreamInputSplit ----

        <TestMethod>
        Public Sub testStreamInputSplitWithFileStreams()
            Dim f1 As String = writeFile("file1.txt", "a,b,c" & vbLf & "d,e,f")
            Dim f2 As String = writeFile("file2.txt", "1,2,3")
            Dim uris As IList(Of Uri) = Arrays.asList(URIUtil.fileToURI(f1), URIUtil.fileToURI(f2))
            Dim fn As New FileStreamCreatorFunction()
            Dim split As New StreamInputSplit(uris, fn)

            assertEquals(2L, split.length())
            assertArrayEquals(New Uri() {URIUtil.fileToURI(f1), URIUtil.fileToURI(f2)}, split.locations())
            assertTrue(split.getUris() Is uris)
            assertTrue(split.getStreamCreatorFn() Is fn)
            assertNull(split.getRng())
            assertNull(split.getOrder())
            'without a random source the URIs are iterated in list order, every time
            assertEquals(uris, drain(split.locationsIterator()))
            assertEquals(uris, drain(split.locationsIterator()))
            assertTrue(split.resetSupported())
            split.reset()
            'the stream creator opens the content of each location
            Dim contents As New List(Of String)()
            Dim iter As IEnumerator(Of Uri) = split.locationsIterator()
            Do While iter.MoveNext()
                Using s As Stream = split.getStreamCreatorFn().apply(iter.Current)
                    contents.Add(New StreamReader(s).ReadToEnd())
                End Using
            Loop
            assertEquals(Arrays.asList("a,b,c" & vbLf & "d,e,f", "1,2,3"), contents)
            'everything else is unsupported
            assertThrows(Of NotSupportedException)(Sub() split.locationsPathIterator())
            assertThrows(Of NotSupportedException)(Sub() split.canWriteToLocation(uris(0)))
            assertThrows(Of NotSupportedException)(Sub() split.addNewLocation())
            assertThrows(Of NotSupportedException)(Sub() split.addNewLocation("x"))
            assertThrows(Of NotSupportedException)(Sub() split.updateSplitLocations(True))
            assertThrows(Of NotSupportedException)(Sub() split.needsBootstrapForWrite())
            assertThrows(Of NotSupportedException)(Sub() split.bootStrapForWrite())
            assertThrows(Of NotSupportedException)(Sub() split.openInputStreamFor("x"))
            assertThrows(Of NotSupportedException)(Sub() split.openOutputStreamFor("x"))
            'both arguments are required
            assertThrows(Of ArgumentNullException)(Sub()
                                                       Dim ignored As New StreamInputSplit(Nothing, fn)
                                                   End Sub)
            assertThrows(Of ArgumentNullException)(Sub()
                                                       Dim ignored As New StreamInputSplit(uris, Nothing)
                                                   End Sub)
        End Sub

        <TestMethod>
        Public Sub testStreamInputSplitRandomization()
            Dim uris As New List(Of Uri)()
            For i As Integer = 0 To 19
                uris.Add(New Uri("file:///C:/data/" & i & ".csv"))
            Next i
            Dim split As New StreamInputSplit(uris, New FileStreamCreatorFunction(), New Random(42))
            assertNotNull(split.getRng())
            'every iterator is a fresh permutation of all the URIs
            Dim first As List(Of Uri) = drain(split.locationsIterator())
            assertEquals(20, first.Count)
            assertEquals(New HashSet(Of Uri)(uris), New HashSet(Of Uri)(first))
            assertNotEquals(uris, first)
            Dim second As List(Of Uri) = drain(split.locationsIterator())
            assertEquals(New HashSet(Of Uri)(uris), New HashSet(Of Uri)(second))
            assertNotEquals(first, second)
            'the order array is created on first use and reused
            assertNotNull(split.getOrder())
            assertEquals(20, split.getOrder().Length)
            assertEquals(Enumerable.Range(0, 20).ToList(), split.getOrder().OrderBy(Function(x) x).ToList())
            'locations() and length() are unaffected
            assertArrayEquals(uris.ToArray(), split.locations())
            assertEquals(20L, split.length())
            'the same seed gives the same permutation
            Dim again As New StreamInputSplit(uris, New FileStreamCreatorFunction(), New Random(42))
            assertEquals(first, drain(again.locationsIterator()))
            'removing the random source restores list order
            split.setRng(Nothing)
            assertEquals(uris, drain(split.locationsIterator()))
        End Sub

        <TestMethod>
        Public Sub testStreamInputSplitDataMethods()
            Dim u1 As New Uri("file:///C:/data/1.csv")
            Dim u2 As New Uri("file:///C:/data/2.csv")
            Dim fn As New FileStreamCreatorFunction()
            Dim a As New StreamInputSplit(Arrays.asList(u1, u2), fn)
            Dim b As New StreamInputSplit(Arrays.asList(u1, u2), fn)
            'Lombok @Data: equality over the URIs, the function, the rng and the order
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertTrue(a.Equals(a))
            assertFalse(a.Equals(Nothing))
            assertFalse(a.Equals("x"))
            assertNotEquals(a, New StreamInputSplit(Arrays.asList(u2, u1), fn))
            assertNotEquals(a, New StreamInputSplit(Arrays.asList(u1), fn))
            assertNotEquals(a, New StreamInputSplit(Arrays.asList(u1, u2), New FileStreamCreatorFunction()))
            Dim rng As New Random(1)
            assertNotEquals(a, New StreamInputSplit(Arrays.asList(u1, u2), fn, rng))
            b.setRng(rng)
            assertNotEquals(a, b)
            a.setRng(rng)
            assertEquals(a, b)
            b.setOrder(New Integer() {1, 0})
            assertNotEquals(a, b)
            a.setOrder(New Integer() {1, 0})
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            'the explicit order is used by the iterator
            assertEquals(Arrays.asList(u2, u1).Count, drain(a.locationsIterator()).Count)
            'setters
            Dim other As IList(Of Uri) = Arrays.asList(u2)
            a.setUris(other)
            assertTrue(a.getUris() Is other)
            assertEquals(1L, a.length())
            Dim fn2 As New FileStreamCreatorFunction()
            a.setStreamCreatorFn(fn2)
            assertTrue(a.getStreamCreatorFn() Is fn2)
            'toString lists the fields
            Dim text As String = New StreamInputSplit(Arrays.asList(u1, u2), fn).ToString()
            assertTrue(text.StartsWith("StreamInputSplit(uris=[file:///C:/data/1.csv, file:///C:/data/2.csv], streamCreatorFn=", StringComparison.Ordinal), text)
            assertTrue(text.EndsWith(", rng=null, order=null)", StringComparison.Ordinal), text)
            assertTrue(New StreamInputSplit(Arrays.asList(u1), fn, rng).ToString().Contains(", rng=System.Random, order=null)"))
            b.setOrder(New Integer() {1, 0})
            assertTrue(b.ToString().EndsWith("order=[1, 0])", StringComparison.Ordinal), b.ToString())
        End Sub

    End Class

End Namespace
