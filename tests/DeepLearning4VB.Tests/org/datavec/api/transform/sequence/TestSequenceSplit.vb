Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit
Imports SequenceSplitTimeSeparation = DeepLearning4VB.org.datavec.api.transform.sequence.split.SequenceSplitTimeSeparation
Imports SplitMaxLengthSequence = DeepLearning4VB.org.datavec.api.transform.sequence.split.SplitMaxLengthSequence
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TimeUnit = DeepLearning4VB.JavaCompat.TimeUnit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Port of the DataVec <c>TestSequenceSplit</c> JUnit class, plus tests of <see cref="SplitMaxLengthSequence"/> and
    ''' the rest of <see cref="SequenceSplitTimeSeparation"/>.
    ''' </summary>
    <TestClass>
    Public Class TestSequenceSplit

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub testSequenceSplitTimeSeparation()

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnTime("time", DateTimeZone.UTC).addColumnString("text").build()

            Dim inputSequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputSequence.Add(row(New LongWritable(0), New Text("t0")))
            inputSequence.Add(row(New LongWritable(1000), New Text("t1")))
            'Second split: 74 seconds later
            inputSequence.Add(row(New LongWritable(75000), New Text("t2")))
            inputSequence.Add(row(New LongWritable(100000), New Text("t3")))
            'Third split: 1 minute and 1 milliseconds later
            inputSequence.Add(row(New LongWritable(160001), New Text("t4")))

            Dim seqSplit As SequenceSplitT = New SequenceSplitTimeSeparation("time", 1, TimeUnit.MINUTES)
            seqSplit.InputSchema = schema

            Dim splits As IList(Of IList(Of IList(Of WritableT))) = seqSplit.split(inputSequence)
            assertEquals(3, splits.Count)

            Dim exp0 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp0.Add(row(New LongWritable(0), New Text("t0")))
            exp0.Add(row(New LongWritable(1000), New Text("t1")))
            Dim exp1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp1.Add(row(New LongWritable(75000), New Text("t2")))
            exp1.Add(row(New LongWritable(100000), New Text("t3")))
            Dim exp2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp2.Add(row(New LongWritable(160001), New Text("t4")))

            assertEquals(exp0, splits(0))
            assertEquals(exp1, splits(1))
            assertEquals(exp2, splits(2))
        End Sub

        ' ---- additional tests ----

        <TestMethod>
        Public Sub testSequenceSplitTimeSeparationSchemaChecksAndAccessors()
            Dim split As New SequenceSplitTimeSeparation("time", 30, TimeUnit.SECONDS)
            assertEquals(30000L, split.getSeparationMilliseconds())
            assertEquals("time", split.getTimeColumn())
            assertEquals(30L, split.getTimeQuantity())
            assertEquals(TimeUnit.SECONDS, split.getTimeUnit())
            assertEquals(-1, split.getTimeColumnIdx())

            'Missing column / not a time column
            Dim noTime As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("other").build()
            assertThrows(Of InvalidOperationException)(Sub() split.InputSchema = noTime)
            Dim notTime As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("other").addColumnLong("time").build()
            assertThrows(Of InvalidOperationException)(Sub() split.InputSchema = notTime)

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("other").addColumnTime("time", DateTimeZone.UTC).build()
            split.InputSchema = schema
            assertEquals(1, split.getTimeColumnIdx())
            assertTrue(schema Is split.InputSchema)

            'A separation of exactly the threshold does not split
            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("a"), New LongWritable(0)),
                row(New Text("b"), New LongWritable(30000)),
                row(New Text("c"), New LongWritable(60001))}
            Dim splits As IList(Of IList(Of IList(Of WritableT))) = split.split(seq)
            assertEquals(2, splits.Count)
            assertEquals(2, splits(0).Count)
            assertEquals(1, splits(1).Count)

            assertEquals("SequenceSplitTimeSeparation(timeColumn=""time"",timeQuantity=30,timeUnit=SECONDS)", split.ToString())
            'Equality ignores the schema and the derived values
            assertEquals(New SequenceSplitTimeSeparation("time", 30, TimeUnit.SECONDS), split)
            assertEquals(New SequenceSplitTimeSeparation("time", 30, TimeUnit.SECONDS).GetHashCode(), split.GetHashCode())
            assertFalse(split.Equals(New SequenceSplitTimeSeparation("time", 30000, TimeUnit.MILLISECONDS)))
        End Sub

        Private Shared Function sequenceOfLength(ByVal n As Integer) As IList(Of IList(Of WritableT))
            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For i As Integer = 0 To n - 1
                seq.Add(row(New LongWritable(i)))
            Next i
            Return seq
        End Function

        Private Shared Function lengths(ByVal splits As IList(Of IList(Of IList(Of WritableT)))) As IList(Of Integer)
            Dim result As New List(Of Integer)()
            For Each s As IList(Of IList(Of WritableT)) In splits
                result.Add(s.Count)
            Next s
            Return result
        End Function

        ''' <summary>The splits, concatenated, must give back the original sequence.</summary>
        Private Shared Sub assertConcatenation(ByVal seq As IList(Of IList(Of WritableT)), ByVal splits As IList(Of IList(Of IList(Of WritableT))))
            Dim all As New List(Of IList(Of WritableT))()
            For Each s As IList(Of IList(Of WritableT)) In splits
                all.AddRange(s)
            Next s
            assertEquals(seq, all)
        End Sub

        <TestMethod>
        Public Sub testSplitMaxLengthSequence()
            Dim split As SequenceSplitT = New SplitMaxLengthSequence(3, False)

            'Not longer than the maximum: unchanged
            Dim short3 As IList(Of IList(Of WritableT)) = sequenceOfLength(3)
            Dim splits As IList(Of IList(Of IList(Of WritableT))) = split.split(short3)
            assertEquals(1, splits.Count)
            assertTrue(splits(0) Is short3)

            Dim seq As IList(Of IList(Of WritableT)) = sequenceOfLength(10)
            splits = split.split(seq)
            assertEquals(New List(Of Integer) From {3, 3, 3, 1}, lengths(splits))
            assertConcatenation(seq, splits)

            splits = split.split(sequenceOfLength(9))
            assertEquals(New List(Of Integer) From {3, 3, 3}, lengths(splits))
        End Sub

        <TestMethod>
        Public Sub testSplitMaxLengthSequenceEqualSplits()
            Dim split As SequenceSplitT = New SplitMaxLengthSequence(3, True)

            Dim seq As IList(Of IList(Of WritableT)) = sequenceOfLength(10)
            Dim splits As IList(Of IList(Of IList(Of WritableT))) = split.split(seq)
            assertEquals(New List(Of Integer) From {3, 3, 2, 2}, lengths(splits))
            assertConcatenation(seq, splits)

            assertEquals(New List(Of Integer) From {3, 3, 3}, lengths(split.split(sequenceOfLength(9))))
            assertEquals(New List(Of Integer) From {3, 3, 3, 3}, lengths(split.split(sequenceOfLength(12))))
            assertEquals(New List(Of Integer) From {2, 2}, lengths(split.split(sequenceOfLength(4))))
            assertEquals(New List(Of Integer) From {2}, lengths(split.split(sequenceOfLength(2))))

            Dim longSeq As IList(Of IList(Of WritableT)) = sequenceOfLength(100)
            Dim splits10 As IList(Of IList(Of IList(Of WritableT))) = (New SplitMaxLengthSequence(10, True)).split(longSeq)
            assertEquals(10, splits10.Count)
            For Each s As IList(Of IList(Of WritableT)) In splits10
                assertEquals(10, s.Count)
            Next s
            assertConcatenation(longSeq, splits10)
        End Sub

        <TestMethod>
        Public Sub testSplitMaxLengthSequenceAccessors()
            Dim split As New SplitMaxLengthSequence(5, True)
            assertEquals(5, split.getMaxSequenceLength())
            assertTrue(split.isEqualSplits())
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnLong("x").build()
            split.InputSchema = schema
            assertTrue(schema Is split.InputSchema)
            assertEquals("SplitMaxLengthSequence(maxSequenceLength=5,equalSplits=true)", split.ToString())
            assertEquals(New SplitMaxLengthSequence(5, True), split)
            assertEquals(New SplitMaxLengthSequence(5, True).GetHashCode(), split.GetHashCode())
            assertFalse(split.Equals(New SplitMaxLengthSequence(5, False)))
            assertFalse(split.Equals(New SplitMaxLengthSequence(4, True)))
        End Sub

    End Class

End Namespace
