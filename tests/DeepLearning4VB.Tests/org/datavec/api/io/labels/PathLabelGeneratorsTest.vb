Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.io.labels
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports DVW = DeepLearning4VB.org.datavec.api.writable
Imports WritableType = DeepLearning4VB.org.datavec.api.writable.WritableType

Namespace org.datavec.api.io.labels

    ''' <summary>
    ''' The label generators: the parent directory name (<see cref="ParentPathLabelGenerator"/>) and a
    ''' regex-split segment of the base name (<see cref="PatternPathLabelGenerator"/>), from paths and URIs.
    ''' </summary>
    <TestClass>
    Public Class PathLabelGeneratorsTest

        <TestMethod>
        Public Sub testParentPathLabelFromPathsAndUris()
            Dim gen As PathLabelGenerator = New ParentPathLabelGenerator()
            'only the immediate parent directory is the label
            assertEquals(New Text("cat"), gen.getLabelForPath("C:\data\animals\cat\img_01.jpg"))
            assertEquals(New Text("cat"), gen.getLabelForPath("/data/animals/cat/img_01.jpg"))
            assertEquals(New Text("cat"), gen.getLabelForPath("data/cat/img_01.jpg"))
            assertEquals(New Text("cat"), gen.getLabelForPath(New Uri("file:///C:/data/animals/cat/img_01.jpg")))
            assertEquals(New Text("cat"), gen.getLabelForPath(New Uri("file:///data/cat/img_01.jpg")))
            'URI escapes are decoded before the directory name is taken
            assertEquals(New Text("my label"), gen.getLabelForPath(New Uri("file:///C:/data/my%20label/img.png")))
            'the file name and extension play no part
            assertEquals(New Text("dog"), gen.getLabelForPath("C:\data\dog\noextension"))
            assertEquals(New Text("dog"), gen.getLabelForPath("C:\data\dog\a.b.c.png"))
            'the label is a Text writable and label classes are inferred from the labels
            Dim label As DVW.Writable = gen.getLabelForPath("C:\data\dog\x.png")
            assertEquals(WritableType.Text, label.[getType]())
            assertEquals("dog", label.ToString())
            assertTrue(gen.inferLabelClasses())
        End Sub

        <TestMethod>
        Public Sub testPatternPathLabelPositionsAndRegex()
            'default position: the first segment
            Dim first As New PatternPathLabelGenerator("_")
            assertEquals(New Text("cat"), first.getLabelForPath("C:\data\cat_01.jpg"))
            assertEquals(New Text("cat"), first.getLabelForPath("/data/cat_01.jpg"))
            assertEquals(New Text("cat"), first.getLabelForPath(New Uri("file:///C:/data/cat_01.jpg")))
            'other positions
            assertEquals(New Text("01"), New PatternPathLabelGenerator("_", 1).getLabelForPath("C:\data\cat_01.jpg"))
            assertEquals(New Text("c"), New PatternPathLabelGenerator("_", 2).getLabelForPath("a_b_c.jpg"))
            'the pattern is a regular expression
            assertEquals(New Text("cat"), New PatternPathLabelGenerator("\d+", 1).getLabelForPath("img12cat.jpg"))
            assertEquals(New Text("b"), New PatternPathLabelGenerator("[-_.]", 1).getLabelForPath("a-b_c.jpg"))
            assertEquals(New Text("mid"), New PatternPathLabelGenerator("\s*,\s*", 1).getLabelForPath("x , mid ,y.txt"))
            'the directory never takes part, only the base name
            assertEquals(New Text("cat"), New PatternPathLabelGenerator("_", 0).getLabelForPath("C:\under_scored\dir\cat_01.jpg"))
            assertTrue(first.inferLabelClasses())
            assertEquals(WritableType.Text, first.getLabelForPath("a_b.png").[getType]())
        End Sub

        ''' <summary>Java's <c>String.split</c> keeps leading empty strings but drops trailing ones.</summary>
        <TestMethod>
        Public Sub testPatternPathLabelSplitSemantics()
            'leading empty segment is kept
            assertEquals(New Text(""), New PatternPathLabelGenerator("_", 0).getLabelForPath("_cat.jpg"))
            assertEquals(New Text("cat"), New PatternPathLabelGenerator("_", 1).getLabelForPath("_cat.jpg"))
            'trailing empty segments are dropped, so position 1 does not exist for "cat__.jpg"
            assertEquals(New Text("cat"), New PatternPathLabelGenerator("_", 0).getLabelForPath("cat__.jpg"))
            assertThrows(Of IndexOutOfRangeException)(Sub() Call New PatternPathLabelGenerator("_", 1).getLabelForPath("cat__.jpg"))
            'an inner empty segment is kept
            assertEquals(New Text(""), New PatternPathLabelGenerator("_", 1).getLabelForPath("cat__01.jpg"))
            assertEquals(New Text("01"), New PatternPathLabelGenerator("_", 2).getLabelForPath("cat__01.jpg"))
            'no match: the whole base name is the only segment
            assertEquals(New Text("cat01"), New PatternPathLabelGenerator("_", 0).getLabelForPath("C:\x\cat01.jpg"))
            assertThrows(Of IndexOutOfRangeException)(Sub() Call New PatternPathLabelGenerator("_", 1).getLabelForPath("C:\x\cat01.jpg"))
            'only the last extension is removed (FilenameUtils.getBaseName)
            assertEquals(New Text("b.tar"), New PatternPathLabelGenerator("_", 1).getLabelForPath("a_b.tar.gz"))
            'the extension itself cannot be used as a label
            assertEquals(New Text("cat"), New PatternPathLabelGenerator("\.", 0).getLabelForPath("cat.jpg"))
            assertThrows(Of IndexOutOfRangeException)(Sub() Call New PatternPathLabelGenerator("\.", 1).getLabelForPath("cat.jpg"))
        End Sub

        ''' <summary>Both generators are serializable (they are shipped to Spark workers in Java).</summary>
        <TestMethod>
        Public Sub testGeneratorsAreSerializable()
            assertTrue(isSerializableType(GetType(ParentPathLabelGenerator)))
            assertTrue(isSerializableType(GetType(PatternPathLabelGenerator)))
            Dim formatter As New ObjectFormatter()
            Using ms As New System.IO.MemoryStream()
                formatter.Serialize(ms, New PatternPathLabelGenerator("-", 1))
                ms.Position = 0
                Dim copy As PatternPathLabelGenerator = DirectCast(formatter.Deserialize(ms), PatternPathLabelGenerator)
                assertEquals(New Text("two"), copy.getLabelForPath("one-two-three.png"))
            End Using
        End Sub

        ''' <summary>A PathMultiLabelGenerator is just a function from a path to several writables.</summary>
        <TestMethod>
        Public Sub testPathMultiLabelGenerator()
            Dim gen As PathMultiLabelGenerator = New DirectoryAndNameLabels()
            Dim labels As IList(Of DVW.Writable) = gen.getLabels("C:\data\cat\cat_01.jpg")
            assertEquals(2, labels.Count)
            assertEquals(New Text("cat"), labels(0))
            assertEquals(New Text("01"), labels(1))
            assertEquals(Arrays.asList(New Text("dog"), New Text("7")), gen.getLabels(New Uri("file:///C:/data/dog/dog_7.jpg").LocalPath))
        End Sub

        Private Class DirectoryAndNameLabels
            Implements PathMultiLabelGenerator

            Private ReadOnly parent As New ParentPathLabelGenerator()
            Private ReadOnly pattern As New PatternPathLabelGenerator("_", 1)

            Public Function getLabels(ByVal uriPath As String) As IList(Of DVW.Writable) Implements PathMultiLabelGenerator.getLabels
                Return Arrays.asList(parent.getLabelForPath(uriPath), pattern.getLabelForPath(uriPath))
            End Function

        End Class

    End Class

End Namespace
