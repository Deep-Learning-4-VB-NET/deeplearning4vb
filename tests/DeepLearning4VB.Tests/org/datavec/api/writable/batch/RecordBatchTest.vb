Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.writable
Imports DeepLearning4VB.org.datavec.api.writable.batch
Imports DVW = DeepLearning4VB.org.datavec.api.writable

Namespace org.datavec.api.writable.batch

    ''' <summary>The read-only list views of record batches, exercised through a simple list-backed subclass.</summary>
    <TestClass>
    Public Class RecordBatchTest

        Private Class ListRecordBatch
            Inherits AbstractWritableRecordBatch

            Private ReadOnly records As IList(Of IList(Of DVW.Writable))

            Public Sub New(ByVal records As IList(Of IList(Of DVW.Writable)))
                Me.records = records
            End Sub

            Public Overrides Function size() As Integer
                Return records.Count
            End Function

            Public Overrides Function [get](ByVal index As Integer) As IList(Of DVW.Writable)
                Return records(index)
            End Function
        End Class

        Private Class ListTimeSeriesBatch
            Inherits AbstractTimeSeriesWritableRecordBatch

            Private ReadOnly sequences As IList(Of IList(Of IList(Of DVW.Writable)))

            Public Sub New(ByVal sequences As IList(Of IList(Of IList(Of DVW.Writable))))
                Me.sequences = sequences
            End Sub

            Public Overrides Function size() As Integer
                Return sequences.Count
            End Function

            Public Overrides Function [get](ByVal index As Integer) As IList(Of IList(Of DVW.Writable))
                Return sequences(index)
            End Function
        End Class

        Private Shared Function Record(ByVal ParamArray values() As Integer) As IList(Of DVW.Writable)
            Dim r As New List(Of DVW.Writable)()
            For Each v As Integer In values
                r.Add(New IntWritable(v))
            Next v
            Return r
        End Function

        <TestMethod>
        Public Sub testRecordBatchReadAccess()
            Dim batch As New ListRecordBatch(New List(Of IList(Of DVW.Writable)) From {Record(1, 2), Record(3, 4), Record(5, 6)})
            assertEquals(3, batch.Count)
            assertEquals(3, batch.size())
            assertFalse(batch.isEmpty())
            assertTrue(batch.IsReadOnly)
            assertEquals(Record(3, 4), batch(1))
            assertEquals(Record(5, 6), batch.get(2))

            ' For Each goes through the list iterator
            Dim seen As Integer = 0
            For Each r As IList(Of DVW.Writable) In batch
                assertEquals(Record(2 * seen + 1, 2 * seen + 2), r)
                seen += 1
            Next r
            assertEquals(3, seen)

            ' Java-style ListIterator methods
            Dim it As AbstractWritableRecordBatch.RecordBatchListIterator = batch.listIterator()
            assertTrue(it.hasNext())
            assertFalse(it.hasPrevious())
            assertEquals(Record(1, 2), it.next())
            assertTrue(it.hasPrevious())
            assertEquals(Record(1, 2), it.previous())
            assertEquals(2, it.nextIndex())
            assertEquals(0, it.previousIndex())
            assertEquals(Record(3, 4), it.next())
            assertEquals(Record(5, 6), it.next())
            assertFalse(it.hasNext())
            assertThrows(Of NotSupportedException)(Sub() it.remove())
            assertThrows(Of NotSupportedException)(Sub() it.add(Record(0)))
            assertThrows(Of NotSupportedException)(Sub() it.set(Record(0)))

            ' .NET collection consumers work through CopyTo
            Dim copy As New List(Of IList(Of DVW.Writable))(batch)
            assertEquals(3, copy.Count)
            assertEquals(Record(1, 2), copy(0))
            assertTrue(new ListRecordBatch(New List(Of IList(Of DVW.Writable))()).isEmpty())
        End Sub

        <TestMethod>
        Public Sub testRecordBatchIsReadOnly()
            Dim batch As New ListRecordBatch(New List(Of IList(Of DVW.Writable)) From {Record(1)})
            assertThrows(Of NotSupportedException)(Sub() batch.Add(Record(2)))
            assertThrows(Of NotSupportedException)(Sub() batch.Insert(0, Record(2)))
            assertThrows(Of NotSupportedException)(Sub() batch.Remove(Record(1)))
            assertThrows(Of NotSupportedException)(Sub() batch.RemoveAt(0))
            assertThrows(Of NotSupportedException)(Sub() batch(0) = Record(2))
            assertThrows(Of NotSupportedException)(Sub() batch.set(0, Record(2)))
            assertThrows(Of NotSupportedException)(Sub() batch.IndexOf(Record(1)))
            assertThrows(Of NotSupportedException)(Sub() batch.lastIndexOf(Record(1)))
            assertThrows(Of NotSupportedException)(Sub() batch.subList(0, 1))
            assertThrows(Of NotSupportedException)(Sub() batch.toArray())
            assertThrows(Of NotSupportedException)(Sub() batch.listIterator(0))
            ' Contains/containsAll always return false and Clear is a no-op, like the Java original
            assertFalse(batch.Contains(Record(1)))
            assertFalse(batch.containsAll(New List(Of IList(Of DVW.Writable)) From {Record(1)}))
            batch.Clear()
            assertEquals(1, batch.Count)
        End Sub

        <TestMethod>
        Public Sub testTimeSeriesBatch()
            Dim seq1 As IList(Of IList(Of DVW.Writable)) = New List(Of IList(Of DVW.Writable)) From {Record(1), Record(2)}
            Dim seq2 As IList(Of IList(Of DVW.Writable)) = New List(Of IList(Of DVW.Writable)) From {Record(3)}
            Dim batch As New ListTimeSeriesBatch(New List(Of IList(Of IList(Of DVW.Writable))) From {seq1, seq2})
            assertEquals(2, batch.Count)
            assertFalse(batch.isEmpty())
            assertTrue(seq2 Is batch(1))
            assertTrue(seq1 Is batch.get(0))
            assertTrue(batch.IsReadOnly)
            assertFalse(batch.Contains(seq1))
            ' iteration is unsupported in the Java original
            assertThrows(Of NotSupportedException)(Sub() batch.GetEnumerator())
            assertThrows(Of NotSupportedException)(Sub() batch.Add(seq1))
            assertThrows(Of NotSupportedException)(Sub() batch.RemoveAt(0))
            assertThrows(Of NotSupportedException)(Sub() batch.IndexOf(seq1))
            Dim arr(1) As IList(Of IList(Of DVW.Writable))
            batch.CopyTo(arr, 0)
            assertTrue(arr(0) Is seq1 AndAlso arr(1) Is seq2)
        End Sub

    End Class

End Namespace
