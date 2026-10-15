Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordMetaDataLine = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLine
Imports SVMLightRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>
    ''' Ported from the DL4J JUnit test of the same name. Java's NoSuchElementException is InvalidOperationException,
    ''' UnsupportedOperationException is NotSupportedException and IndexOutOfBoundsException is IndexOutOfRangeException.
    ''' </summary>
    <TestClass>
    Public Class SVMLightRecordReaderTest

        Private Shared ReadOnly ZERO As WritableT = SVMLightRecordReader.ZERO
        Private Shared ReadOnly ONE As WritableT = SVMLightRecordReader.ONE
        Private Shared ReadOnly LABEL_ZERO As WritableT = SVMLightRecordReader.LABEL_ZERO
        Private Shared ReadOnly LABEL_ONE As WritableT = SVMLightRecordReader.LABEL_ONE

        Private Shared Function resource(ByVal name As String) As FileSplit
            Return New FileSplit(New ClassPathResource("datavec-api/svmlight/" & name).File)
        End Function

        Private Shared Sub assertRecords(ByVal correct As IList(Of IList(Of WritableT)), ByVal rr As SVMLightRecordReader)
            Dim i As Integer = 0
            Do While rr.hasNext()
                Dim record As IList(Of WritableT) = rr.next()
                assertEquals(correct(i), record)
                i += 1
            Loop
            assertEquals(correct.Count, i)
        End Sub

        <TestMethod>
        Public Sub testBasicRecord()
            Dim correct As New List(Of IList(Of WritableT))()
            ' 7 2:1 4:2 6:3 8:4 10:5
            correct.Add(Arrays.asList(ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5), New IntWritable(7)))
            ' 2 qid:42 1:0.1 2:2 6:6.6 8:80
            correct.Add(Arrays.asList(New DoubleWritable(0.1), New DoubleWritable(2), ZERO, ZERO, ZERO, New DoubleWritable(6.6), ZERO, New DoubleWritable(80), ZERO, ZERO, New IntWritable(2)))
            ' 33
            correct.Add(Arrays.asList(ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, New IntWritable(33)))

            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
            rr.initialize(config, resource("basic.txt"))
            assertRecords(correct, rr)
        End Sub

        <TestMethod>
        Public Sub testNoAppendLabel()
            Dim correct As New List(Of IList(Of WritableT))()
            ' 7 2:1 4:2 6:3 8:4 10:5
            correct.Add(Arrays.asList(ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5)))
            ' 2 qid:42 1:0.1 2:2 6:6.6 8:80
            correct.Add(Arrays.asList(New DoubleWritable(0.1), New DoubleWritable(2), ZERO, ZERO, ZERO, New DoubleWritable(6.6), ZERO, New DoubleWritable(80), ZERO, ZERO))
            ' 33
            correct.Add(Arrays.asList(ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO))

            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
            config.setBoolean(SVMLightRecordReader.APPEND_LABEL, False)
            rr.initialize(config, resource("basic.txt"))
            assertRecords(correct, rr)
        End Sub

        <TestMethod>
        Public Sub testNoLabel()
            Dim correct As New List(Of IList(Of WritableT))()
            '  2:1 4:2 6:3 8:4 10:5
            correct.Add(Arrays.asList(ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5)))
            '  qid:42 1:0.1 2:2 6:6.6 8:80
            correct.Add(Arrays.asList(New DoubleWritable(0.1), New DoubleWritable(2), ZERO, ZERO, ZERO, New DoubleWritable(6.6), ZERO, New DoubleWritable(80), ZERO, ZERO))
            '  1:1.0
            correct.Add(Arrays.asList(New DoubleWritable(1.0), ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO))
            ' (empty line)
            correct.Add(Arrays.asList(ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO))

            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
            config.setBoolean(SVMLightRecordReader.APPEND_LABEL, True)
            rr.initialize(config, resource("noLabels.txt"))
            assertRecords(correct, rr)
        End Sub

        <TestMethod>
        Public Sub testMultioutputRecord()
            Dim correct As New List(Of IList(Of WritableT))()
            ' 7 2.45,9 2:1 4:2 6:3 8:4 10:5
            correct.Add(Arrays.asList(ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5), New IntWritable(7), New DoubleWritable(2.45), New IntWritable(9)))
            ' 2,3,4 qid:42 1:0.1 2:2 6:6.6 8:80
            correct.Add(Arrays.asList(New DoubleWritable(0.1), New DoubleWritable(2), ZERO, ZERO, ZERO, New DoubleWritable(6.6), ZERO, New DoubleWritable(80), ZERO, ZERO, New IntWritable(2), New IntWritable(3), New IntWritable(4)))
            ' 33,32.0,31.9
            correct.Add(Arrays.asList(ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, New IntWritable(33), New DoubleWritable(32.0), New DoubleWritable(31.9)))

            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
            rr.initialize(config, resource("multioutput.txt"))
            assertRecords(correct, rr)
        End Sub

        <TestMethod>
        Public Sub testMultilabelRecord()
            Dim correct As New List(Of IList(Of WritableT))()
            ' 1,3 2:1 4:2 6:3 8:4 10:5
            correct.Add(Arrays.asList(ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5), LABEL_ONE, LABEL_ZERO, LABEL_ONE, LABEL_ZERO))
            ' 2 qid:42 1:0.1 2:2 6:6.6 8:80
            correct.Add(Arrays.asList(New DoubleWritable(0.1), New DoubleWritable(2), ZERO, ZERO, ZERO, New DoubleWritable(6.6), ZERO, New DoubleWritable(80), ZERO, ZERO, LABEL_ZERO, LABEL_ONE, LABEL_ZERO, LABEL_ZERO))
            ' 1,2,4
            correct.Add(Arrays.asList(ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ONE, LABEL_ONE, LABEL_ZERO, LABEL_ONE))
            '  1:1.0
            correct.Add(Arrays.asList(New DoubleWritable(1.0), ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO))
            ' (empty line)
            correct.Add(Arrays.asList(ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO))

            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
            config.setBoolean(SVMLightRecordReader.MULTILABEL, True)
            config.setInt(SVMLightRecordReader.NUM_LABELS, 4)
            rr.initialize(config, resource("multilabel.txt"))
            assertRecords(correct, rr)
        End Sub

        <TestMethod>
        Public Sub testZeroBasedIndexing()
            Dim correct As New List(Of IList(Of WritableT))()
            ' 1,3 2:1 4:2 6:3 8:4 10:5
            correct.Add(Arrays.asList(ZERO, ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5), LABEL_ZERO, LABEL_ONE, LABEL_ZERO, LABEL_ONE, LABEL_ZERO))
            ' 2 qid:42 1:0.1 2:2 6:6.6 8:80
            correct.Add(Arrays.asList(ZERO, New DoubleWritable(0.1), New DoubleWritable(2), ZERO, ZERO, ZERO, New DoubleWritable(6.6), ZERO, New DoubleWritable(80), ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ONE, LABEL_ZERO, LABEL_ZERO))
            ' 1,2,4
            correct.Add(Arrays.asList(ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ONE, LABEL_ONE, LABEL_ZERO, LABEL_ONE))
            '  1:1.0
            correct.Add(Arrays.asList(ZERO, New DoubleWritable(1.0), ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO))
            ' (empty line)
            correct.Add(Arrays.asList(ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO))

            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            'Zero-based indexing is default
            'NOT STANDARD!
            config.setBoolean(SVMLightRecordReader.ZERO_BASED_LABEL_INDEXING, True)
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 11)
            config.setBoolean(SVMLightRecordReader.MULTILABEL, True)
            config.setInt(SVMLightRecordReader.NUM_LABELS, 5)
            rr.initialize(config, resource("multilabel.txt"))
            assertRecords(correct, rr)
        End Sub

        <TestMethod>
        Public Sub testNextRecord()
            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
            config.setBoolean(SVMLightRecordReader.APPEND_LABEL, False)
            rr.initialize(config, resource("basic.txt"))

            Dim record As Record = rr.nextRecord()
            Dim recordList As IList(Of WritableT) = record.getRecord()
            assertEquals(New DoubleWritable(1.0), recordList(1))
            assertEquals(New DoubleWritable(3.0), recordList(5))
            assertEquals(New DoubleWritable(4.0), recordList(7))
            'the metadata is the line of the record (line 0 is the comment)
            assertEquals(1, DirectCast(record.getMetaData(), RecordMetaDataLine).getLineNumber())
            assertEquals(GetType(SVMLightRecordReader), record.getMetaData().getReaderClass())

            record = rr.nextRecord()
            recordList = record.getRecord()
            assertEquals(New DoubleWritable(0.1), recordList(0))
            assertEquals(New DoubleWritable(6.6), recordList(5))
            assertEquals(New DoubleWritable(80.0), recordList(7))
            assertEquals(2, DirectCast(record.getMetaData(), RecordMetaDataLine).getLineNumber())

            'reset re-reads from the start
            rr.reset()
            assertTrue(rr.hasNext())
            assertEquals(New DoubleWritable(1.0), rr.next()(1))
        End Sub

        <TestMethod>
        Public Sub testNoSuchElementException()
            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 11)
            rr.initialize(config, resource("basic.txt"))
            Do While rr.hasNext()
                rr.next()
            Loop
            assertThrows(Of InvalidOperationException)(Sub() rr.next())
        End Sub

        <TestMethod>
        Public Sub failedToSetNumFeaturesException()
            assertThrows(Of NotSupportedException)(Sub()
                                                       Dim rr As New SVMLightRecordReader()
                                                       Dim config As New Configuration()
                                                       rr.initialize(config, resource("basic.txt"))
                                                       Do While rr.hasNext()
                                                           rr.next()
                                                       Loop
                                                   End Sub)
        End Sub

        <TestMethod>
        Public Sub testInconsistentNumLabelsException()
            assertThrows(Of NotSupportedException)(Sub()
                                                       Dim rr As New SVMLightRecordReader()
                                                       Dim config As New Configuration()
                                                       config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
                                                       rr.initialize(config, resource("inconsistentNumLabels.txt"))
                                                       Do While rr.hasNext()
                                                           rr.next()
                                                       Loop
                                                   End Sub)
        End Sub

        <TestMethod>
        Public Sub failedToSetNumMultiabelsException()
            assertThrows(Of NotSupportedException)(Sub()
                                                       Dim rr As New SVMLightRecordReader()
                                                       Dim config As New Configuration()
                                                       rr.initialize(config, resource("multilabel.txt"))
                                                       Do While rr.hasNext()
                                                           rr.next()
                                                       Loop
                                                   End Sub)
        End Sub

        <TestMethod>
        Public Sub testFeatureIndexExceedsNumFeatures()
            assertThrows(Of IndexOutOfRangeException)(Sub()
                                                          Dim rr As New SVMLightRecordReader()
                                                          Dim config As New Configuration()
                                                          config.setInt(SVMLightRecordReader.NUM_FEATURES, 9)
                                                          rr.initialize(config, resource("basic.txt"))
                                                          rr.next()
                                                      End Sub)
        End Sub

        <TestMethod>
        Public Sub testLabelIndexExceedsNumLabels()
            assertThrows(Of IndexOutOfRangeException)(Sub()
                                                          Dim rr As New SVMLightRecordReader()
                                                          Dim config As New Configuration()
                                                          config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
                                                          config.setInt(SVMLightRecordReader.NUM_LABELS, 6)
                                                          rr.initialize(config, resource("basic.txt"))
                                                          rr.next()
                                                      End Sub)
        End Sub

        <TestMethod>
        Public Sub testZeroIndexFeatureWithoutUsingZeroIndexing()
            assertThrows(Of IndexOutOfRangeException)(Sub()
                                                          Dim rr As New SVMLightRecordReader()
                                                          Dim config As New Configuration()
                                                          config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
                                                          config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
                                                          rr.initialize(config, resource("zeroIndexFeature.txt"))
                                                          rr.next()
                                                      End Sub)
        End Sub

        <TestMethod>
        Public Sub testZeroIndexLabelWithoutUsingZeroIndexing()
            assertThrows(Of IndexOutOfRangeException)(Sub()
                                                          Dim rr As New SVMLightRecordReader()
                                                          Dim config As New Configuration()
                                                          config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
                                                          config.setBoolean(SVMLightRecordReader.MULTILABEL, True)
                                                          config.setInt(SVMLightRecordReader.NUM_LABELS, 2)
                                                          rr.initialize(config, resource("zeroIndexLabel.txt"))
                                                          rr.next()
                                                      End Sub)
        End Sub

        ''' <summary>The inconsistent-label-count check itself, which the JUnit tests never reach (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testInconsistentNumLabelsIsDetected()
            Dim rr As New SVMLightRecordReader()
            Dim config As New Configuration()
            config.setBoolean(SVMLightRecordReader.ZERO_BASED_INDEXING, False)
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 10)
            rr.initialize(config, resource("inconsistentNumLabels.txt"))
            'the first record sets the number of labels (1); the second has 2
            assertEquals(11, rr.next().Count)
            assertThrows(Of IndexOutOfRangeException)(Sub() rr.next())
            'a bad feature index is a NumberFormatException
            Dim bad As New SVMLightRecordReader()
            bad.initialize(config, New DeepLearning4VB.org.datavec.api.split.StringSplit("1 x:2.0"))
            assertThrows(Of NumberFormatException)(Sub() bad.next())
        End Sub

    End Class

End Namespace
