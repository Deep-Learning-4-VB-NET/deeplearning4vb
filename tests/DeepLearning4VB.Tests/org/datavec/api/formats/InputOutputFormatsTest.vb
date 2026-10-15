Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports InputFormat = DeepLearning4VB.org.datavec.api.formats.input.InputFormat
Imports CSVInputFormat = DeepLearning4VB.org.datavec.api.formats.input.impl.CSVInputFormat
Imports LibSvmInputFormat = DeepLearning4VB.org.datavec.api.formats.input.impl.LibSvmInputFormat
Imports LineInputFormat = DeepLearning4VB.org.datavec.api.formats.input.impl.LineInputFormat
Imports ListStringInputFormat = DeepLearning4VB.org.datavec.api.formats.input.impl.ListStringInputFormat
Imports MatlabInputFormat = DeepLearning4VB.org.datavec.api.formats.input.impl.MatlabInputFormat
Imports SVMLightInputFormat = DeepLearning4VB.org.datavec.api.formats.input.impl.SVMLightInputFormat
Imports OutputFormat = DeepLearning4VB.org.datavec.api.formats.output.OutputFormat
Imports OutputFormatConstants = DeepLearning4VB.org.datavec.api.formats.output.OutputFormatConstants
Imports CSVOutputFormat = DeepLearning4VB.org.datavec.api.formats.output.impl.CSVOutputFormat
Imports LibSvmOutputFormat = DeepLearning4VB.org.datavec.api.formats.output.impl.LibSvmOutputFormat
Imports LineOutputFormat = DeepLearning4VB.org.datavec.api.formats.output.impl.LineOutputFormat
Imports SVMLightOutputFormat = DeepLearning4VB.org.datavec.api.formats.output.impl.SVMLightOutputFormat
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports LineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.LineRecordReader
Imports ListStringRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.ListStringRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports LibSvmRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.LibSvmRecordReader
Imports MatlabRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.MatlabRecordReader
Imports SVMLightRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter
Imports LineRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.LineRecordWriter
Imports CSVRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.csv.CSVRecordWriter
Imports LibSvmRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.misc.LibSvmRecordWriter
Imports MatlabRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.misc.MatlabRecordWriter
Imports SVMLightRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.misc.SVMLightRecordWriter
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports ListStringSplit = DeepLearning4VB.org.datavec.api.split.ListStringSplit
Imports NumberOfRecordsPartitioner = DeepLearning4VB.org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource

Namespace org.datavec.api.formats

    ''' <summary>The input formats (reader factories) and output formats (writer factories).</summary>
    <TestClass>
    Public Class InputOutputFormatsTest

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

        Private Shared Function irisSplit() As FileSplit
            Return New FileSplit(New ClassPathResource("datavec-api/iris.dat").File)
        End Function

        Private Shared Function countRecords(ByVal rr As RecordReader, ByVal expectedSize As Integer) As Integer
            Dim count As Integer = 0
            Do While rr.hasNext()
                assertEquals(expectedSize, rr.next().Count)
                count += 1
            Loop
            rr.Dispose()
            Return count
        End Function

        <TestMethod>
        Public Sub testCsvAndLineInputFormats()
            Dim csv As InputFormat = New CSVInputFormat()
            Dim rr As RecordReader = csv.createReader(irisSplit())
            assertTrue(TypeOf rr Is CSVRecordReader)
            assertEquals(150, countRecords(rr, 5))
            'the configuration overload ignores the configuration, as in Java
            assertEquals(150, countRecords(csv.createReader(irisSplit(), New Configuration()), 5))

            Dim lines As InputFormat = New LineInputFormat()
            Dim lr As RecordReader = lines.createReader(irisSplit())
            assertTrue(TypeOf lr Is LineRecordReader)
            assertFalse(TypeOf lr Is CSVRecordReader)
            assertEquals(150, countRecords(lr, 1))
            assertEquals(150, countRecords(lines.createReader(irisSplit(), Nothing), 1))
        End Sub

        <TestMethod>
        Public Sub testListStringInputFormat()
            Dim data As IList(Of IList(Of String)) = New List(Of IList(Of String))()
            data.Add(Arrays.asList("a", "b"))
            data.Add(Arrays.asList("c", "d"))

            Dim format As New ListStringInputFormat()
            Dim rr As RecordReader = format.createReader(New ListStringSplit(data))
            assertTrue(TypeOf rr Is ListStringRecordReader)
            assertEquals(Arrays.asList(Of WritableT)(New Text("a"), New Text("b")), rr.next())
            assertEquals(2, countRecords(format.createReader(New ListStringSplit(data), New Configuration()), 2))

            'the Writable members are no-ops / zeros
            assertEquals(0.0, format.toDouble(), 0.0)
            assertEquals(0, format.toInt())
            assertEquals(0L, format.toLong())
            assertThrows(Of NotSupportedException)(Sub() format.getType())
        End Sub

        <TestMethod>
        Public Sub testSvmLightAndLibSvmInputFormats()
            Dim conf As New Configuration()
            conf.setInt(SVMLightRecordReader.NUM_FEATURES, 4)
            conf.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
            Dim split As New FileSplit(New ClassPathResource("data/irisSvmLight.txt").File)

            'LibSvmInputFormat passes the configuration on
            Dim libsvm As RecordReader = New LibSvmInputFormat().createReader(split, conf)
            assertTrue(TypeOf libsvm Is LibSvmRecordReader)
            Dim first As IList(Of WritableT) = libsvm.next()
            assertEquals(Arrays.asList(Of WritableT)(New DoubleWritable(5.1), New DoubleWritable(3.5), New DoubleWritable(1.4), New DoubleWritable(0.2), New IntWritable(0)), first)
            assertEquals(11, countRecords(libsvm, 5)) 'the test fixture holds 12 iris rows, one already read

            'SVMLightInputFormat does not (as in Java): the reader needs its configuration before reading
            Dim svm As RecordReader = New SVMLightInputFormat().createReader(split, conf)
            assertTrue(TypeOf svm Is SVMLightRecordReader)
            assertThrows(Of InvalidOperationException)(Sub() svm.next())
            svm.Conf = conf
            assertEquals(first, svm.next())
            assertEquals(11, countRecords(svm, 5)) 'the test fixture holds 12 iris rows, one already read

            'the base input format's Writable members
            Dim format As New SVMLightInputFormat()
            assertThrows(Of NotSupportedException)(Sub() format.toDouble())
            assertThrows(Of NotSupportedException)(Sub() format.toInt())
            assertThrows(Of NotSupportedException)(Sub() format.getType())
            format.write(Nothing)
            format.readFields(Nothing)
        End Sub

        <TestMethod>
        Public Sub testMatlabInputFormatAndWriter()
            Dim f As String = Path.Combine(tmpDir, "matrix.txt")
            File.WriteAllText(f, "1 2 3" & vbLf & "% a comment line" & vbLf & "4.5" & vbTab & "6 7" & vbLf, New UTF8Encoding(False))

            Dim rr As RecordReader = New MatlabInputFormat().createReader(New FileSplit(f))
            assertTrue(TypeOf rr Is MatlabRecordReader)
            assertTrue(rr.hasNext())
            assertEquals(Arrays.asList(Of WritableT)(New DoubleWritable(1), New DoubleWritable(2), New DoubleWritable(3)), rr.next())
            assertEquals(Arrays.asList(Of WritableT)(New DoubleWritable(4.5), New DoubleWritable(6), New DoubleWritable(7)), rr.next())
            assertFalse(rr.hasNext())
            rr.Dispose()

            'the Matlab writer round-trips through the reader
            Dim out As String = Path.Combine(tmpDir, "written.txt")
            Using writer As New MatlabRecordWriter()
                writer.initialize(New FileSplit(out), New NumberOfRecordsPartitioner())
                writer.write(Arrays.asList(Of WritableT)(New IntWritable(1), New DoubleWritable(2.5)))
                writer.write(Arrays.asList(Of WritableT)(New IntWritable(3), New DoubleWritable(4)))
                assertFalse(writer.supportsBatch())
            End Using
            assertEquals("1 2.5" & vbLf & "3 4.0" & vbLf, File.ReadAllText(out))
            Dim back As RecordReader = New MatlabInputFormat().createReader(New FileSplit(out), Nothing)
            assertEquals(Arrays.asList(Of WritableT)(New DoubleWritable(1), New DoubleWritable(2.5)), back.next())
            assertEquals(1, countRecords(back, 2))
        End Sub

        <TestMethod>
        Public Sub testOutputFormats()
            Dim conf As New Configuration()
            conf.set(OutputFormatConstants.OUTPUT_PATH, tmpDir)
            assertEquals("org.nd4j.outputpath", OutputFormatConstants.OUTPUT_PATH)

            Dim csv As OutputFormat = New CSVOutputFormat()
            assertTrue(TypeOf csv.createWriter(conf) Is CSVRecordWriter)
            Dim line As OutputFormat = New LineOutputFormat()
            assertTrue(TypeOf line.createWriter(conf) Is LineRecordWriter)
            Dim svm As OutputFormat = New SVMLightOutputFormat()
            assertTrue(TypeOf svm.createWriter(conf) Is SVMLightRecordWriter)
            'the LibSVM format configures its writer
            Dim libsvm As RecordWriter = New LibSvmOutputFormat().createWriter(conf)
            assertTrue(TypeOf libsvm Is LibSvmRecordWriter)
            assertTrue(libsvm.Conf Is conf)
            assertTrue(libsvm.getConf() Is conf)

            'a CSV writer from the format writes a readable file
            Dim outFile As String = Path.Combine(tmpDir, "out.csv")
            Using writer As RecordWriter = csv.createWriter(conf)
                writer.initialize(New FileSplit(outFile), New NumberOfRecordsPartitioner())
                writer.write(Arrays.asList(Of WritableT)(New IntWritable(1), New Text("one")))
                writer.write(Arrays.asList(Of WritableT)(New IntWritable(2), New Text("two")))
            End Using
            Dim rr As RecordReader = New CSVInputFormat().createReader(New FileSplit(outFile))
            assertEquals(Arrays.asList(Of WritableT)(New Text("1"), New Text("one")), rr.next())
            assertEquals(1, countRecords(rr, 2))
        End Sub

        <TestMethod>
        Public Sub testLineRecordWriter()
            'the line writer writes the first Text of each record with Text.write (a length prefix), then a new line
            Dim outFile As String = Path.Combine(tmpDir, "lines.txt")
            Using writer As RecordWriter = New LineOutputFormat().createWriter(New Configuration())
                assertFalse(writer.supportsBatch())
                writer.initialize(New FileSplit(outFile), New NumberOfRecordsPartitioner())
                assertEquals(1, writer.write(Arrays.asList(Of WritableT)(New Text("hello"), New Text("ignored"))).getNumRecordsUpdated())
                writer.write(New List(Of WritableT)())
                writer.write(Arrays.asList(Of WritableT)(New Text("hi")))
                assertThrows(Of NotImplementedException)(Sub() writer.writeBatch(New List(Of IList(Of WritableT))()))
            End Using
            Dim bytes() As Byte = File.ReadAllBytes(outFile)
            Dim expected As New List(Of Byte)()
            expected.Add(5)
            expected.AddRange(Encoding.UTF8.GetBytes("hello" & vbLf))
            expected.Add(2)
            expected.AddRange(Encoding.UTF8.GetBytes("hi" & vbLf))
            assertArrayEquals(expected.ToArray(), bytes)
        End Sub

    End Class

End Namespace
