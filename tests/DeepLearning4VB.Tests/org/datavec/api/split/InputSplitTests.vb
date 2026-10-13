Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.split
Imports BalancedPathFilter = DeepLearning4VB.org.datavec.api.io.filters.BalancedPathFilter
Imports RandomPathFilter = DeepLearning4VB.org.datavec.api.io.filters.RandomPathFilter
Imports ParentPathLabelGenerator = DeepLearning4VB.org.datavec.api.io.labels.ParentPathLabelGenerator
Imports PatternPathLabelGenerator = DeepLearning4VB.org.datavec.api.io.labels.PatternPathLabelGenerator

Namespace org.datavec.api.split

    ''' <summary>Ported from the DataVec JUnit test of the same name (author saudet).</summary>
    <TestClass>
    Public Class InputSplitTests

        <TestMethod>
        Public Sub testSample()
            Dim split As BaseInputSplit = New LabelledPathsInputSplit()

            Dim random As New Random(42)
            Dim extensions() As String = {"tif", "jpg", "png", "jpeg", "bmp", "JPEG", "JPG", "TIF", "PNG"}
            Dim parentPathLabelGenerator As New ParentPathLabelGenerator()
            Dim patternPathLabelGenerator As New PatternPathLabelGenerator("_", 0)
            Dim randomPathFilter As New RandomPathFilter(random, extensions)
            Dim randomPathFilter2 As New RandomPathFilter(random, extensions, 7)
            Dim balancedPathFilter As New BalancedPathFilter(random, extensions, parentPathLabelGenerator, 0, 4, 0, 1)
            Dim balancedPathFilter2 As New BalancedPathFilter(random, extensions, patternPathLabelGenerator, 0, 4, 0, 1)

            'the .wtf file is dropped, everything else is kept
            Dim samples() As InputSplit = split.sample(randomPathFilter)
            assertEquals(1, samples.Length)
            assertEquals(11L, samples(0).length())

            Dim samples2() As InputSplit = split.sample(randomPathFilter2)
            assertEquals(1, samples2.Length)
            assertEquals(7L, samples2(0).length())

            '6 directory labels, at most 4 labels with 1 path each: 4 paths split 80/20
            Dim samples3() As InputSplit = split.sample(balancedPathFilter, 80, 20)
            assertEquals(2, samples3.Length)
            assertEquals(3L, samples3(0).length())
            assertEquals(1L, samples3(1).length())

            '2 file-name labels (group1, group2) with 1 path each: 2 paths split 50/50
            Dim samples4() As InputSplit = split.sample(balancedPathFilter2, 50, 50)
            assertEquals(2, samples4.Length)
            assertEquals(1L, samples4(0).length())
            assertEquals(1L, samples4(1).length())
        End Sub

        ''' <summary>The anonymous <c>BaseInputSplit</c> subclass of the Java test.</summary>
        Private Class LabelledPathsInputSplit
            Inherits BaseInputSplit

            Public Sub New()
                Dim paths() As String = {"label0/group1_img.tif", "label1/group1_img.jpg", "label2/group1_img.png", "label3/group1_img.jpeg", "label4/group1_img.bmp", "label5/group1_img.JPEG", "label0/group2_img.JPG", "label1/group2_img.TIF", "label2/group2_img.PNG", "label3/group2_img.jpg", "label4/group2_img.jpg", "label5/group2_img.wtf"}

                uriStrings = New List(Of String)(paths.Length)
                For Each s As String In paths
                    uriStrings.Add("file:///" & s)
                Next s
            End Sub

            Public Overrides Sub updateSplitLocations(ByVal reset As Boolean)
            End Sub

            Public Overrides Function needsBootstrapForWrite() As Boolean
                Return False
            End Function

            Public Overrides Sub bootStrapForWrite()
            End Sub

            Public Overrides Function openOutputStreamFor(ByVal location As String) As Stream
                Return Nothing
            End Function

            Public Overrides Function openInputStreamFor(ByVal location As String) As Stream
                Return Nothing
            End Function

            Public Overrides Sub reset()
                'No op
            End Sub

            Public Overrides Function resetSupported() As Boolean
                Return True
            End Function

        End Class

        <TestMethod>
        Public Sub testFileSplitBootstrap()
            Dim tmpDir As String = Path.Combine(Path.GetTempPath(), "dl4vb-tests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tmpDir)
            Try
                Dim boostrap As New FileSplit(tmpDir)
                'an empty directory has no location to write to
                assertTrue(boostrap.needsBootstrapForWrite())
                boostrap.bootStrapForWrite()
                assertTrue(Directory.Exists(tmpDir)) 'Java: tmpDir.listFiles() != null
                'once a location has been added the split can be written to
                Dim added As String = boostrap.addNewLocation()
                assertTrue(added.StartsWith("file:", StringComparison.Ordinal))
                assertEquals(1, Directory.GetFiles(tmpDir).Length)
                assertFalse(boostrap.needsBootstrapForWrite())
                assertEquals(1L, boostrap.length())
            Finally
                If Directory.Exists(tmpDir) Then Directory.Delete(tmpDir, True)
            End Try
        End Sub

        <TestMethod>
        Public Sub testSampleNoBias()
            Dim random As New Random(7)
            Dim randomPathFilter As New RandomPathFilter(random, Nothing, 100)

            Dim paths(999) As Uri
            For i As Integer = 0 To paths.Length - 1
                paths(i) = New Uri("file:///label" & (i \ 100) & "/image" & i)
            Next i

            Dim notOnlyFirstLabel As Boolean = False
            Dim paths2() As Uri = randomPathFilter.filter(paths)
            assertEquals(100, paths2.Length)
            For i As Integer = 0 To paths2.Length - 1
                If Not paths2(i).ToString().StartsWith("file:///label0/", StringComparison.Ordinal) Then
                    notOnlyFirstLabel = True
                End If
            Next i
            assertTrue(notOnlyFirstLabel)
        End Sub

    End Class

End Namespace
