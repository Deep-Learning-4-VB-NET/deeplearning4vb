Imports System
Imports System.Collections.Generic
Imports System.Text.Json
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.writable
Imports DeepLearning4VB.org.datavec.api.writable.comparator
Imports DeepLearning4VB.org.nd4vb.common.holder
Imports DVW = DeepLearning4VB.org.datavec.api.writable

Namespace org.datavec.api.writable.comparator

    ''' <summary>The writable comparers used to sort records: factory, ordering, reversal, equality and JSON form.</summary>
    <TestClass>
    Public Class ComparatorsTest

        Private Shared Function Sorted(ByVal cmp As IComparer(Of DVW.Writable), ByVal ParamArray items() As DVW.Writable) As List(Of DVW.Writable)
            Dim list As New List(Of DVW.Writable)(items)
            list.Sort(cmp)
            Return list
        End Function

        <TestMethod>
        Public Sub testForTypeAscending()
            assertTrue(TypeOf Comparators.forType(WritableType.Int) Is IntWritableComparator)
            assertTrue(TypeOf Comparators.forType(WritableType.Byte) Is IntWritableComparator)
            assertTrue(TypeOf Comparators.forType(WritableType.Double) Is DoubleWritableComparator)
            assertTrue(TypeOf Comparators.forType(WritableType.Float) Is FloatWritableComparator)
            assertTrue(TypeOf Comparators.forType(WritableType.Long) Is LongWritableComparator)
            assertTrue(TypeOf Comparators.forType(WritableType.Text) Is TextWritableComparator)
            assertThrows(Of NotSupportedException)(Sub() Comparators.forType(WritableType.Boolean))
            assertThrows(Of NotSupportedException)(Sub() Comparators.forType(WritableType.NDArray))
            assertThrows(Of NotSupportedException)(Sub() Comparators.forType(WritableType.Null, False))
        End Sub

        <TestMethod>
        Public Sub testSortingWithBuiltInComparators()
            Dim ints As List(Of DVW.Writable) = Sorted(Comparators.forType(WritableType.Int), New IntWritable(3), New IntWritable(-1), New IntWritable(2))
            assertEquals(New List(Of DVW.Writable) From {New IntWritable(-1), New IntWritable(2), New IntWritable(3)}, ints)

            Dim desc As List(Of DVW.Writable) = Sorted(Comparators.forType(WritableType.Int, False), New IntWritable(3), New IntWritable(-1), New IntWritable(2))
            assertEquals(New List(Of DVW.Writable) From {New IntWritable(3), New IntWritable(2), New IntWritable(-1)}, desc)

            Dim doubles As List(Of DVW.Writable) = Sorted(Comparators.forType(WritableType.Double), New DoubleWritable(0.5), New Text("-2.5"), New IntWritable(0))
            assertEquals("-2.5", doubles(0).ToString())
            assertEquals(New IntWritable(0), doubles(1))
            assertEquals(New DoubleWritable(0.5), doubles(2))

            Dim longs As List(Of DVW.Writable) = Sorted(Comparators.forType(WritableType.Long), New LongWritable(Long.MaxValue), New LongWritable(Long.MinValue), New LongWritable(0))
            assertEquals(New LongWritable(Long.MinValue), longs(0))
            assertEquals(New LongWritable(Long.MaxValue), longs(2))

            Dim floats As List(Of DVW.Writable) = Sorted(Comparators.forType(WritableType.Float, False), New FloatWritable(1), New FloatWritable(3), New FloatWritable(2))
            assertEquals(New FloatWritable(3), floats(0))
            assertEquals(New FloatWritable(1), floats(2))

            ' text ordering is ordinal: upper case before lower case, like Java's String.compareTo
            Dim texts As List(Of DVW.Writable) = Sorted(Comparators.forType(WritableType.Text), New Text("b"), New Text("B"), New Text("a"), New Text("aa"))
            assertEquals(New List(Of DVW.Writable) From {New Text("B"), New Text("a"), New Text("aa"), New Text("b")}, texts)
            ' Text compares the string form of any writable
            assertTrue(New TextWritableComparator().Compare(New IntWritable(10), New IntWritable(9)) < 0)
        End Sub

        <TestMethod>
        Public Sub testReverseComparator()
            Dim rev As New ReverseComparator(Of DVW.Writable)(New IntWritableComparator())
            assertTrue(rev.Compare(New IntWritable(1), New IntWritable(2)) > 0)
            assertTrue(rev.Compare(New IntWritable(2), New IntWritable(1)) < 0)
            assertEquals(0, rev.Compare(New IntWritable(1), New IntWritable(1)))
            assertTrue(TypeOf rev.getComparator() Is IntWritableComparator)
            Dim revInts As New ReverseComparator(Of Integer)(Comparer(Of Integer).Default)
            Dim l As New List(Of Integer) From {1, 3, 2}
            l.Sort(revInts)
            assertEquals(New List(Of Integer) From {3, 2, 1}, l)
        End Sub

        <TestMethod>
        Public Sub testComparatorEqualityAndToString()
            assertEquals(New IntWritableComparator(), New IntWritableComparator())
            assertEquals(New DoubleWritableComparator().GetHashCode(), New DoubleWritableComparator().GetHashCode())
            assertNotEquals(New IntWritableComparator(), New LongWritableComparator())
            assertEquals("IntWritableComparator()", New IntWritableComparator().ToString())
            assertEquals("DoubleWritableComparator()", New DoubleWritableComparator().ToString())
            assertEquals("FloatWritableComparator()", New FloatWritableComparator().ToString())
            assertEquals("LongWritableComparator()", New LongWritableComparator().ToString())
            assertEquals("TextWritableComparator()", New TextWritableComparator().ToString())
        End Sub

        <TestMethod>
        Public Sub testComparatorJson()
            Dim opts As JsonSerializerOptions = ObjectMapperHolder.JsonMapper
            Dim json As String = JsonSerializer.Serialize(Of WritableComparator)(New LongWritableComparator(), opts)
            assertTrue(json.Contains("""@class"":""org.datavec.api.writable.comparator.LongWritableComparator"""), json)
            Dim back As WritableComparator = JsonSerializer.Deserialize(Of WritableComparator)(json, opts)
            assertTrue(TypeOf back Is LongWritableComparator)
            assertTrue(back.Compare(New LongWritable(1), New LongWritable(2)) < 0)
        End Sub

    End Class

End Namespace
