Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.split

Namespace org.datavec.api.split

    ''' <summary>Ported from the DataVec JUnit test of the same name (author Ede Meijer).</summary>
    <TestClass>
    Public Class TransformSplitTest

        <TestMethod>
        Public Sub testTransform()
            Dim inputFiles As ICollection(Of Uri) = Arrays.asList(New Uri("file:///foo/bar/../0.csv"), New Uri("file:///foo/1.csv"))
            Dim SUT As InputSplit = New TransformSplit(New CollectionInputSplit(inputFiles), New NormalizeTransform())
            assertArrayEquals(New Uri() {
                New Uri("file:///foo/0.csv"),
                New Uri("file:///foo/1.csv")
            }, SUT.locations())
        End Sub

        ''' <summary>Java: <c>uri.normalize()</c>; System.Uri removes dot segments when the URI is built.</summary>
        Private Class NormalizeTransform
            Implements TransformSplit.URITransform

            Public Function apply(ByVal uri As Uri) As Uri Implements TransformSplit.URITransform.apply
                Return New Uri(uri.AbsoluteUri)
            End Function

        End Class

        <TestMethod>
        Public Sub testSearchReplace()
            Dim inputFiles As ICollection(Of Uri) = Arrays.asList(New Uri("file:///foo/1-in.csv"), New Uri("file:///foo/2-in.csv"))
            Dim SUT As InputSplit = TransformSplit.ofSearchReplace(New CollectionInputSplit(inputFiles), "-in.csv", "-out.csv")
            assertArrayEquals(New Uri() {
                New Uri("file:///foo/1-out.csv"),
                New Uri("file:///foo/2-out.csv")
            }, SUT.locations())
        End Sub

        <TestMethod>
        Public Sub testDelegationAndLength()
            Dim inputFiles As ICollection(Of Uri) = Arrays.asList(New Uri("file:///foo/1-in.csv"), New Uri("file:///foo/2-in.csv"))
            Dim source As New CollectionInputSplit(inputFiles)
            Dim SUT As TransformSplit = TransformSplit.ofSearchReplace(source, "-in", "-out")
            assertEquals(2L, SUT.length())
            assertFalse(SUT.needsBootstrapForWrite())
            assertTrue(SUT.resetSupported())
            SUT.reset()
            assertNull(SUT.openInputStreamFor("file:///foo/1-out.csv"))
            Dim iter As IEnumerator(Of Uri) = SUT.locationsIterator()
            Dim seen As New List(Of Uri)()
            Do While iter.MoveNext()
                seen.Add(iter.Current)
            Loop
            assertEquals(Arrays.asList(New Uri("file:///foo/1-out.csv"), New Uri("file:///foo/2-out.csv")), seen)
            assertThrows(Of ArgumentNullException)(Sub() TransformSplit.ofSearchReplace(source, Nothing, "x"))
            assertThrows(Of ArgumentNullException)(Sub()
                                                       Dim ignored As New TransformSplit(Nothing, New NormalizeTransform())
                                                   End Sub)
        End Sub

    End Class

End Namespace
