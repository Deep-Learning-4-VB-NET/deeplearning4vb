Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports LibSvmRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.LibSvmRecordReader
Imports LibSvmRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports NumberOfRecordsPartitioner = DeepLearning4VB.org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource

Namespace org.datavec.api.records.writer.impl

    ''' <summary>
    ''' Ported from the DL4J JUnit test of the same name. The NDArrayWritable tests (testNDArrayWritables,
    ''' testNDArrayWritablesMultilabel, testNDArrayWritablesZeroIndex) are not ported: they need Nd4vb/INDArray.
    ''' </summary>
    <TestClass>
    Public Class LibSvmRecordWriterTest

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

        Private Shared Function resource(ByVal name As String) As String
            Return New ClassPathResource("datavec-api/svmlight/" & name).File
        End Function

        <TestMethod>
        Public Sub testBasic()
            Dim configWriter As New Configuration()

            Dim configReader As New Configuration()
            configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 10)
            configReader.setBoolean(LibSvmRecordReader.ZERO_BASED_INDEXING, False)

            executeTest(configWriter, configReader, resource("basic.txt"))
        End Sub

        <TestMethod>
        Public Sub testNoLabel()
            Dim configWriter As New Configuration()
            configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
            configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 9)

            Dim configReader As New Configuration()
            configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 10)
            configReader.setBoolean(LibSvmRecordReader.ZERO_BASED_INDEXING, False)

            executeTest(configWriter, configReader, resource("basic.txt"))
        End Sub

        <TestMethod>
        Public Sub testMultioutputRecord()
            Dim configWriter As New Configuration()
            configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
            configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 9)

            Dim configReader As New Configuration()
            configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 10)
            configReader.setBoolean(LibSvmRecordReader.ZERO_BASED_INDEXING, False)

            executeTest(configWriter, configReader, resource("multioutput.txt"))
        End Sub

        <TestMethod>
        Public Sub testMultilabelRecord()
            Dim configWriter As New Configuration()
            configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
            configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 9)
            configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)

            Dim configReader As New Configuration()
            configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 10)
            configReader.setBoolean(LibSvmRecordReader.MULTILABEL, True)
            configReader.setInt(LibSvmRecordReader.NUM_LABELS, 4)
            configReader.setBoolean(LibSvmRecordReader.ZERO_BASED_INDEXING, False)

            executeTest(configWriter, configReader, resource("multilabel.txt"))
        End Sub

        <TestMethod>
        Public Sub testZeroBasedIndexing()
            Dim configWriter As New Configuration()
            configWriter.setBoolean(LibSvmRecordWriter.ZERO_BASED_INDEXING, True)
            configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
            configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 10)
            configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)

            Dim configReader As New Configuration()
            configReader.setInt(LibSvmRecordReader.NUM_FEATURES, 11)
            configReader.setBoolean(LibSvmRecordReader.MULTILABEL, True)
            configReader.setInt(LibSvmRecordReader.NUM_LABELS, 5)

            executeTest(configWriter, configReader, resource("multilabel.txt"))
        End Sub

        Public Sub executeTest(ByVal configWriter As Configuration, ByVal configReader As Configuration, ByVal inputFile As String)
            Dim tempFile As String = Path.Combine(tmpDir, "LibSvmRecordWriter.txt")

            Using writer As New LibSvmRecordWriter()
                Dim outputSplit As New FileSplit(tempFile)
                writer.initialize(configWriter, outputSplit, New NumberOfRecordsPartitioner())
                Dim rr As New LibSvmRecordReader()
                rr.initialize(configReader, New FileSplit(inputFile))
                Do While rr.hasNext()
                    Dim record As IList(Of WritableT) = rr.next()
                    writer.write(record)
                Loop
            End Using

            Dim p As New Regex(LibSvmRecordReader.QID_PREFIX & ":\d+ ")
            Dim linesOriginal As IList(Of String) = New List(Of String)()
            For Each line As String In File.ReadAllLines(inputFile)
                If Not line.StartsWith(LibSvmRecordReader.COMMENT_CHAR, StringComparison.Ordinal) Then
                    Dim lineClean As String = line.Split(New Char() {"#"c}, 2)(0)
                    If lineClean.StartsWith(" ", StringComparison.Ordinal) Then
                        lineClean = " " & lineClean.Trim()
                    Else
                        lineClean = lineClean.Trim()
                    End If
                    lineClean = p.Replace(lineClean, "")
                    linesOriginal.Add(lineClean)
                End If
            Next line
            Dim linesNew As IList(Of String) = File.ReadAllLines(tempFile)
            assertEquals(linesOriginal, linesNew)
        End Sub

        <TestMethod>
        Public Sub testNonIntegerButValidMultilabel()
            Dim record As IList(Of WritableT) = Arrays.asList(Of WritableT)(New IntWritable(3), New IntWritable(2), New DoubleWritable(1.0))

            Dim tempFile As String = Path.Combine(tmpDir, "LibSvmRecordWriter.txt")

            Using writer As New LibSvmRecordWriter()
                Dim configWriter As New Configuration()
                configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
                configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 1)
                configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
                Dim outputSplit As New FileSplit(tempFile)
                writer.initialize(configWriter, outputSplit, New NumberOfRecordsPartitioner())
                writer.write(record)
            End Using
            assertEquals(Arrays.asList("1 1:3.0 2:2.0"), File.ReadAllLines(tempFile))
        End Sub

        ''' <summary>Java: NumberFormatException; the port's JavaCompat.NumberFormatException.</summary>
        <TestMethod>
        Public Sub nonIntegerMultilabel()
            assertThrows(Of NumberFormatException)(Sub()
                                                       Dim record As IList(Of WritableT) = Arrays.asList(Of WritableT)(New IntWritable(3), New IntWritable(2), New DoubleWritable(1.2))
                                                       Dim tempFile As String = Path.Combine(tmpDir, "LibSvmRecordWriter.txt")
                                                       Using writer As New LibSvmRecordWriter()
                                                           Dim configWriter As New Configuration()
                                                           configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
                                                           configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 1)
                                                           configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
                                                           Dim outputSplit As New FileSplit(tempFile)
                                                           writer.initialize(configWriter, outputSplit, New NumberOfRecordsPartitioner())
                                                           writer.write(record)
                                                       End Using
                                                   End Sub)
        End Sub

        <TestMethod>
        Public Sub nonBinaryMultilabel()
            assertThrows(Of NumberFormatException)(Sub()
                                                       Dim record As IList(Of WritableT) = Arrays.asList(Of WritableT)(New IntWritable(0), New IntWritable(1), New IntWritable(2))
                                                       Dim tempFile As String = Path.Combine(tmpDir, "LibSvmRecordWriter.txt")
                                                       Using writer As New LibSvmRecordWriter()
                                                           Dim configWriter As New Configuration()
                                                           configWriter.setInt(LibSvmRecordWriter.FEATURE_FIRST_COLUMN, 0)
                                                           configWriter.setInt(LibSvmRecordWriter.FEATURE_LAST_COLUMN, 1)
                                                           configWriter.setBoolean(LibSvmRecordWriter.MULTILABEL, True)
                                                           Dim outputSplit As New FileSplit(tempFile)
                                                           writer.initialize(configWriter, outputSplit, New NumberOfRecordsPartitioner())
                                                           writer.write(record)
                                                       End Using
                                                   End Sub)
        End Sub

    End Class

End Namespace
