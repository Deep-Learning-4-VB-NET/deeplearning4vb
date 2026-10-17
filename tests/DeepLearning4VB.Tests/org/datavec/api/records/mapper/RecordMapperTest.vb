Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports RecordReaderConverter = DeepLearning4VB.org.datavec.api.records.converter.RecordReaderConverter
Imports RecordMapper = DeepLearning4VB.org.datavec.api.records.mapper.RecordMapper
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports CollectionSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.CollectionSequenceRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports SequenceRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.SequenceRecordWriter
Imports CSVRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.csv.CSVRecordWriter
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports NumberOfRecordsPartitioner = DeepLearning4VB.org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports PartitionMetaData = DeepLearning4VB.org.datavec.api.split.partition.PartitionMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource

Namespace org.datavec.api.records.mapper

    ''' <summary>
    ''' <see cref="RecordMapper"/> (reader to writer copies, one record at a time and in batches, and
    ''' concatenated readers) and <see cref="RecordReaderConverter"/>.
    ''' </summary>
    <TestClass>
    Public Class RecordMapperTest

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

        ''' <summary>All records of a CSV file.</summary>
        Private Shared Function readCsv(ByVal file As String) As IList(Of IList(Of WritableT))
            Dim result As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Using rr As New CSVRecordReader()
                rr.initialize(New FileSplit(file))
                Do While rr.hasNext()
                    result.Add(rr.next())
                Loop
            End Using
            Return result
        End Function

        <TestMethod>
        Public Sub testCopyOneRecordAtATime()
            Dim outFile As String = Path.Combine(tmpDir, "out.csv")
            Dim writer As New CSVRecordWriter()
            Dim partitioner As New NumberOfRecordsPartitioner()
            Dim mapper As RecordMapper = RecordMapper.newBuilder().recordReader(New CSVRecordReader()).recordWriter(writer).inputUrl(irisSplit()).outputUrl(New FileSplit(outFile)).partitioner(partitioner).build()
            assertTrue(mapper.getPartitioner() Is partitioner)
            assertTrue(mapper.Partitioner Is partitioner)
            mapper.copy()
            'without batching the writer is left open (as in Java)
            writer.close()

            assertEquals(150, partitioner.totalRecordsWritten())
            Dim expected As IList(Of IList(Of WritableT)) = readCsv(New ClassPathResource("datavec-api/iris.dat").File)
            assertEquals(expected, readCsv(outFile))
        End Sub

        <TestMethod>
        Public Sub testCopyInBatches()
            Dim outFile As String = Path.Combine(tmpDir, "out.csv")
            Dim writer As New CSVRecordWriter()
            Dim partitioner As New NumberOfRecordsPartitioner()
            Dim mapper As RecordMapper = RecordMapper.newBuilder().recordReader(New CSVRecordReader()).recordWriter(writer).inputUrl(irisSplit()).outputUrl(New FileSplit(outFile)).partitioner(partitioner).batchSize(40).build()
            'CSV reader and writer both support batches, so the batch path is taken and closes the writer itself
            mapper.copy()

            assertEquals(150, partitioner.totalRecordsWritten())
            Dim expected As IList(Of IList(Of WritableT)) = readCsv(New ClassPathResource("datavec-api/iris.dat").File)
            assertEquals(expected, readCsv(outFile))
        End Sub

        <TestMethod>
        Public Sub testCopyConcatenatedReaders()
            Dim outFile As String = Path.Combine(tmpDir, "out.csv")
            Dim writer As New CSVRecordWriter()
            Dim readers() As RecordReader = {New CSVRecordReader(), New CSVRecordReader(10, ","c)}
            Dim splits() As InputSplit = {irisSplit(), irisSplit()}
            Dim mapper As RecordMapper = RecordMapper.newBuilder().readersToConcat(readers).splitPerReader(splits).recordWriter(writer).outputUrl(New FileSplit(outFile)).build()
            'the concatenating copy closes the writer
            mapper.copy()

            Dim copied As IList(Of IList(Of WritableT)) = readCsv(outFile)
            assertEquals(290, copied.Count)
            Dim expected As IList(Of IList(Of WritableT)) = readCsv(New ClassPathResource("datavec-api/iris.dat").File)
            assertEquals(expected(0), copied(0))
            assertEquals(expected(10), copied(150))
            assertEquals(expected(149), copied(289))
        End Sub

        <TestMethod>
        Public Sub testBuilderValidation()
            Dim outFile As String = Path.Combine(tmpDir, "out.csv")
            'neither a reader nor readers to concatenate
            Dim none As RecordMapper = RecordMapper.newBuilder().recordWriter(New CSVRecordWriter()).outputUrl(New FileSplit(outFile)).build()
            assertThrows(Of ArgumentException)(Sub() none.copy())
            'one split per reader
            Dim mismatched As RecordMapper = RecordMapper.newBuilder().readersToConcat(New RecordReader() {New CSVRecordReader()}).splitPerReader(New InputSplit() {irisSplit(), irisSplit()}).recordWriter(New CSVRecordWriter()).outputUrl(New FileSplit(outFile)).build()
            assertThrows(Of ArgumentException)(Sub() mismatched.copy())
            'a null reader
            Dim nullReader As RecordMapper = RecordMapper.newBuilder().readersToConcat(New RecordReader() {Nothing}).splitPerReader(New InputSplit() {irisSplit()}).recordWriter(New CSVRecordWriter()).outputUrl(New FileSplit(outFile)).build()
            assertThrows(Of InvalidOperationException)(Sub() nullReader.copy())
            'the builder's Lombok string form
            Dim text As String = RecordMapper.newBuilder().batchSize(7).ToString()
            assertTrue(text.StartsWith("RecordMapper.RecordMapperBuilder(recordReader=null", StringComparison.Ordinal), text)
            assertTrue(text.EndsWith("batchSize=7)", StringComparison.Ordinal), text)
        End Sub

        <TestMethod>
        Public Sub testRecordReaderConverter()
            Dim outFile As String = Path.Combine(tmpDir, "converted.csv")
            Dim reader As New CSVRecordReader(140, ","c)
            reader.initialize(irisSplit())
            Dim writer As New CSVRecordWriter()
            writer.initialize(New FileSplit(outFile), New NumberOfRecordsPartitioner())
            'closes the writer on completion
            RecordReaderConverter.convert(reader, writer)
            Dim copied As IList(Of IList(Of WritableT)) = readCsv(outFile)
            assertEquals(10, copied.Count)
            assertEquals(readCsv(New ClassPathResource("datavec-api/iris.dat").File)(140), copied(0))

            'an exhausted reader cannot be converted
            assertThrows(Of NotSupportedException)(Sub() RecordReaderConverter.convert(reader, New CSVRecordWriter()))
            reader.close()

            'the sequence form, with an in-memory sequence writer
            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            sequences.Add(Arrays.asList(Of IList(Of WritableT))(Arrays.asList(Of WritableT)(New IntWritable(1)), Arrays.asList(Of WritableT)(New IntWritable(2))))
            sequences.Add(Arrays.asList(Of IList(Of WritableT))(Arrays.asList(Of WritableT)(New IntWritable(3))))
            Dim seqWriter As New InMemorySequenceWriter()
            RecordReaderConverter.convert(New CollectionSequenceRecordReader(sequences), seqWriter, False)
            assertEquals(sequences, seqWriter.sequences)
            assertFalse(seqWriter.closed)
            RecordReaderConverter.convert(New CollectionSequenceRecordReader(sequences), seqWriter)
            assertEquals(4, seqWriter.sequences.Count)
            assertTrue(seqWriter.closed)
        End Sub

        ''' <summary>Collects the written sequences.</summary>
        Private Class InMemorySequenceWriter
            Implements SequenceRecordWriter

            Public ReadOnly sequences As New List(Of IList(Of IList(Of WritableT)))()
            Public closed As Boolean
            Private confValue As Configuration

            Public Function write(ByVal sequence As IList(Of IList(Of WritableT))) As PartitionMetaData Implements SequenceRecordWriter.write
                sequences.Add(sequence)
                Return PartitionMetaData.builder().numRecordsUpdated(1).build()
            End Function

            Public Sub close() Implements SequenceRecordWriter.close
                closed = True
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                close()
            End Sub

            Public Property Conf As Configuration Implements Global.DeepLearning4VB.org.datavec.api.conf.Configurable.Conf
                Get
                    Return confValue
                End Get
                Set(ByVal value As Configuration)
                    confValue = value
                End Set
            End Property

            Public Sub setConf(ByVal conf As Configuration) Implements Global.DeepLearning4VB.org.datavec.api.conf.Configurable.setConf
                confValue = conf
            End Sub

            Public Function getConf() As Configuration Implements Global.DeepLearning4VB.org.datavec.api.conf.Configurable.getConf
                Return confValue
            End Function
        End Class

    End Class

End Namespace
