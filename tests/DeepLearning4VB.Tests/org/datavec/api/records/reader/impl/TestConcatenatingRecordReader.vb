Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports ComposableRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.ComposableRecordReader
Imports ConcatenatingRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.ConcatenatingRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports StringSplit = DeepLearning4VB.org.datavec.api.split.StringSplit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name, plus the ComposableRecordReader.</summary>
    <TestClass>
    Public Class TestConcatenatingRecordReader

        <TestMethod>
        Public Sub test()
            Dim rr As New CSVRecordReader(0, ","c)
            rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim rr2 As New CSVRecordReader(0, ","c)
            rr2.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Using rrC As RecordReader = New ConcatenatingRecordReader(rr, rr2)
                Dim count As Integer = 0
                Do While rrC.hasNext()
                    Dim r As IList(Of WritableT) = rrC.next()
                    assertEquals(5, r.Count)
                    count += 1
                Loop

                assertEquals(300, count)
                assertTrue(rrC.resetSupported())
                rrC.reset()
                assertEquals("5.1", rrC.nextRecord().getRecord()(0).ToString())
            End Using
        End Sub

        ''' <summary>Each composed record is the concatenation of one record per reader (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testComposable()
            Dim rr As New CSVRecordReader(0, ","c)
            rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))
            Dim rr2 As New CSVRecordReader(0, ","c)
            rr2.initialize(New StringSplit("x,y"))

            Using composed As RecordReader = New ComposableRecordReader(rr, rr2)
                assertTrue(composed.hasNext())
                Dim first As IList(Of WritableT) = composed.next()
                assertEquals(7, first.Count)
                assertEquals("5.1", first(0).ToString())
                assertEquals("x", first(5).ToString())
                assertEquals("y", first(6).ToString())
                'the second reader is exhausted, so the composition is too
                assertFalse(composed.hasNext())
                assertTrue(rr.hasNext())
                composed.reset()
                assertTrue(composed.hasNext())
                assertEquals(7, composed.nextRecord().getRecord().Count)
                assertNull(composed.Labels)
                assertThrows(Of NotSupportedException)(Sub() composed.record(Nothing, Nothing))
            End Using
        End Sub

    End Class

End Namespace
