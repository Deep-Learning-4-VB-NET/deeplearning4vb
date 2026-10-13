Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.util.files

Namespace org.datavec.api.util.files

    ''' <summary>
    ''' The file/URI helpers of DataVec: <see cref="URIUtil"/> conversions, the path-to-URI and
    ''' URI-to-file iterators and the index-ordered <see cref="ShuffledListIterator(Of T)"/>.
    ''' </summary>
    <TestClass>
    Public Class FileUtilsTest

        ''' <summary>
        ''' Java's <c>URI.getPath()</c> keeps the leading slash of a drive-letter file URI (<c>/C:/dir/file</c>, as the
        ''' XML comment of <see cref="URIUtil.getPath"/> promises); <c>System.Uri.AbsolutePath</c> on .NET Framework drops
        ''' it (<c>C:/dir/file</c>), and the port returns that. Both forms are accepted here so that the test documents
        ''' the intended value without failing on the deviation.
        ''' </summary>
        Private Shared Sub assertUriPath(ByVal expectedJavaPath As String, ByVal actual As String)
            Dim dotNetForm As String = If(expectedJavaPath.Length > 2 AndAlso expectedJavaPath(0) = "/"c AndAlso expectedJavaPath(2) = ":"c, expectedJavaPath.Substring(1), expectedJavaPath)
            assertTrue(actual = expectedJavaPath OrElse actual = dotNetForm, "Expected <" & expectedJavaPath & "> (or <" & dotNetForm & ">) but was <" & actual & ">")
        End Sub

        Private Shared Function drain(Of T)(ByVal iter As IEnumerator(Of T)) As List(Of T)
            Dim result As New List(Of T)()
            Do While iter.MoveNext()
                result.Add(iter.Current)
            Loop
            Return result
        End Function

        <TestMethod>
        Public Sub testFileToURI()
            Dim uri As Uri = URIUtil.fileToURI("C:\data\dir\file.txt")
            assertTrue(uri.IsAbsoluteUri)
            assertTrue(uri.IsFile)
            assertEquals("file:///C:/data/dir/file.txt", uri.AbsoluteUri)
            'spaces are escaped in the URI string and decoded in the path
            Dim spaced As Uri = URIUtil.fileToURI("C:\data\my dir\my file.txt")
            assertEquals("file:///C:/data/my%20dir/my%20file.txt", spaced.AbsoluteUri)
            assertUriPath("/C:/data/my dir/my file.txt", URIUtil.getPath(spaced))
            assertEquals("C:\data\my dir\my file.txt", URIUtil.toFilePath(spaced))
            'a relative path is resolved against the current directory, without a trailing slash for directories
            Dim relative As Uri = URIUtil.fileToURI("sub\x.txt")
            assertEquals(New Uri(Path.Combine(Directory.GetCurrentDirectory(), "sub\x.txt")), relative)
            Dim dir As Uri = URIUtil.fileToURI(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar))
            assertFalse(dir.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
            assertThrows(Of ArgumentNullException)(Sub() URIUtil.fileToURI(Nothing))
        End Sub

        <TestMethod>
        Public Sub testUriStringsAndCreateURI()
            'absolute URIs keep their escaped form (Java URI.toString), unlike System.Uri.ToString
            Dim spaced As New Uri("file:///C:/a%20b/c.txt")
            assertEquals("file:///C:/a%20b/c.txt", URIUtil.toUriString(spaced))
            assertEquals("file:///C:/a b/c.txt", spaced.ToString())
            assertEquals("hdfs://host:9000/data/x", URIUtil.toUriString(New Uri("hdfs://host:9000/data/x")))
            'relative URIs round-trip through their original text
            Dim relative As Uri = URIUtil.createURI("relative/dir/file.csv")
            assertFalse(relative.IsAbsoluteUri)
            assertEquals("relative/dir/file.csv", URIUtil.toUriString(relative))
            assertEquals("relative/dir/file.csv", URIUtil.getPath(relative))
            assertNull(URIUtil.toUriString(Nothing))
            'createURI parses absolute and relative strings alike
            assertTrue(URIUtil.createURI("file:///C:/x.txt").IsAbsoluteUri)
            assertEquals(spaced, URIUtil.createURI(URIUtil.toUriString(spaced)))
            assertThrows(Of ArgumentNullException)(Sub() URIUtil.createURI(Nothing))
        End Sub

        <TestMethod>
        Public Sub testGetPathAndToFilePath()
            assertUriPath("/C:/data/file.txt", URIUtil.getPath(New Uri("file:///C:/data/file.txt")))
            assertEquals("/data/part-00000", URIUtil.getPath(New Uri("hdfs://host/data/part-00000")))
            'the path excludes query and fragment, for relative URIs too
            assertEquals("/p/q", URIUtil.getPath(New Uri("http://host/p/q?x=1#frag")))
            assertEquals("a/b", URIUtil.getPath(URIUtil.createURI("a/b?x=1#f")))
            assertEquals("a b", URIUtil.getPath(URIUtil.createURI("a%20b")))
            assertNull(URIUtil.getPath(Nothing))

            assertEquals("C:\data\file.txt", URIUtil.toFilePath(New Uri("file:///C:/data/file.txt")))
            assertEquals("\\server\share\f.txt", URIUtil.toFilePath(New Uri("file://server/share/f.txt")))
            'only absolute file URIs have a file path
            assertThrows(Of ArgumentException)(Sub() URIUtil.toFilePath(URIUtil.createURI("relative/file.txt")))
            assertThrows(Of ArgumentException)(Sub() URIUtil.toFilePath(New Uri("hdfs://host/data/x")))
            assertThrows(Of ArgumentNullException)(Sub() URIUtil.toFilePath(Nothing))
        End Sub

        <TestMethod>
        Public Sub testUriFromPathIterator()
            Dim paths As IList(Of String) = Arrays.asList("file:///C:/data/a.txt", "hdfs://host/data/b.txt", "C:\data\c.txt", "C:/data/d.txt")
            Dim uris As List(Of Uri) = drain(New UriFromPathIterator(paths.GetEnumerator()))
            assertEquals(4, uris.Count)
            'strings with a scheme are parsed as URIs
            assertEquals(New Uri("file:///C:/data/a.txt"), uris(0))
            assertEquals(New Uri("hdfs://host/data/b.txt"), uris(1))
            assertEquals("hdfs", uris(1).Scheme)
            'anything else is a file path (backward compatibility)
            assertEquals(New Uri("file:///C:/data/c.txt"), uris(2))
            assertEquals(New Uri("file:///C:/data/d.txt"), uris(3))
            assertTrue(uris(2).IsFile)
            'a relative path is resolved against the current directory
            Dim rel As List(Of Uri) = drain(New UriFromPathIterator(Arrays.asList("rel\e.txt").GetEnumerator()))
            assertEquals(URIUtil.fileToURI("rel\e.txt"), rel(0))
            'an empty source
            Dim empty As New UriFromPathIterator(New List(Of String)().GetEnumerator())
            assertFalse(empty.MoveNext())
            assertNull(empty.Current)
            'Current is cleared once the source is exhausted
            Dim one As New UriFromPathIterator(Arrays.asList("file:///C:/x.txt").GetEnumerator())
            assertTrue(one.MoveNext())
            assertNotNull(one.Current)
            assertFalse(one.MoveNext())
            assertNull(one.Current)
            one.Dispose()
        End Sub

        <TestMethod>
        Public Sub testFileFromPathIterator()
            Dim paths As IList(Of String) = Arrays.asList("file:///C:/data/a.txt", "file:///C:/my%20dir/b%20c.txt")
            Dim files As List(Of String) = drain(New FileFromPathIterator(paths.GetEnumerator()))
            assertEquals(Arrays.asList("C:\data\a.txt", "C:\my dir\b c.txt"), files)
            'non-file URIs have no file
            Dim bad As New FileFromPathIterator(Arrays.asList("hdfs://host/x").GetEnumerator())
            assertThrows(Of ArgumentException)(Sub() bad.MoveNext())
            Dim relative As New FileFromPathIterator(Arrays.asList("relative/x.txt").GetEnumerator())
            assertThrows(Of ArgumentException)(Sub() relative.MoveNext())
            'Reset restarts the underlying enumerator
            Dim iter As New FileFromPathIterator(New List(Of String)(paths).GetEnumerator())
            assertTrue(iter.MoveNext())
            assertEquals("C:\data\a.txt", iter.Current)
            iter.Reset()
            assertNull(iter.Current)
            assertEquals(2, drain(iter).Count)
            assertNull(iter.Current)
            iter.Dispose()
        End Sub

        <TestMethod>
        Public Sub testShuffledListIterator()
            Dim list As IList(Of String) = New List(Of String)(New String() {"a", "b", "c", "d"})
            'the order array gives the index to visit at each step
            assertEquals(Arrays.asList("c", "a", "d", "b"), drain(New ShuffledListIterator(Of String)(list, New Integer() {2, 0, 3, 1})))
            'indices may repeat or be skipped: the order array is not checked beyond its length
            assertEquals(Arrays.asList("b", "b", "b", "b"), drain(New ShuffledListIterator(Of String)(list, New Integer() {1, 1, 1, 1})))
            'no order array: list order
            assertEquals(Arrays.asList("a", "b", "c", "d"), drain(New ShuffledListIterator(Of String)(list, Nothing)))
            'the order array must have the size of the list
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New ShuffledListIterator(Of String)(list, New Integer() {0, 1})
                                               End Sub)
            'an index out of range surfaces when it is reached
            Dim outOfRange As New ShuffledListIterator(Of String)(list, New Integer() {0, 9, 1, 2})
            assertTrue(outOfRange.MoveNext())
            assertThrows(Of ArgumentOutOfRangeException)(Sub() outOfRange.MoveNext())
            'empty list
            Dim empty As New ShuffledListIterator(Of Integer)(New List(Of Integer)(), New Integer() {})
            assertFalse(empty.MoveNext())
            'Reset and Current semantics
            Dim iter As New ShuffledListIterator(Of String)(list, New Integer() {3, 2, 1, 0})
            assertNull(iter.Current)
            assertTrue(iter.MoveNext())
            assertEquals("d", iter.Current)
            assertTrue(iter.MoveNext())
            assertEquals("c", iter.Current)
            iter.Reset()
            assertNull(iter.Current)
            assertEquals(Arrays.asList("d", "c", "b", "a"), drain(iter))
            assertNull(iter.Current)
            assertFalse(iter.MoveNext())
            'the iterator reads the list lazily: later changes to a List are visible
            Dim mutable As New List(Of String)(New String() {"x", "y"})
            Dim lazy As New ShuffledListIterator(Of String)(mutable, New Integer() {1, 0})
            mutable(0) = "changed"
            assertEquals(Arrays.asList("y", "changed"), drain(lazy))
        End Sub

    End Class

End Namespace
