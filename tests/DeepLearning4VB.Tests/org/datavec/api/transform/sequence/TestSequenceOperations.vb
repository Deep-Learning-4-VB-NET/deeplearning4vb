Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BaseSequenceExpansionTransform = DeepLearning4VB.org.datavec.api.transform.sequence.expansion.BaseSequenceExpansionTransform
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConvertFromSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertFromSequence
Imports ConvertToSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertToSequence
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NumericalColumnComparator = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceComparator
Imports SequenceMerge = DeepLearning4VB.org.datavec.api.transform.sequence.merge.SequenceMerge
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports SequenceTrimToLengthTransform = DeepLearning4VB.org.datavec.api.transform.sequence.trim.SequenceTrimToLengthTransform
Imports SequenceTrimToLengthTransformExtensions = DeepLearning4VB.org.datavec.api.transform.sequence.trim.SequenceTrimToLengthTransformExtensions
Imports SequenceTrimTransform = DeepLearning4VB.org.datavec.api.transform.sequence.trim.SequenceTrimTransform
Imports StringComparator = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.StringComparator
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Tests of the sequence comparators, <c>ConvertToSequence</c>/<c>ConvertFromSequence</c>, the trim transforms
    ''' (including <c>TestTransforms.testTrimSequenceTransform</c>), the expansion base class and <c>SequenceMerge</c>.
    ''' </summary>
    <TestClass>
    Public Class TestSequenceOperations

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        Private Shared Function numericSchema() As SchemaT
            Return (New SequenceSchemaT.Builder()).addColumnInteger("int").addColumnLong("long").addColumnDouble("double").addColumnTime("time", DateTimeZone.UTC).addColumnString("str").addColumnFloat("float").build()
        End Function

        ' ---- comparators ----

        <TestMethod>
        Public Sub testNumericalColumnComparator()
            Dim schema As SchemaT = numericSchema()
            Dim r1 As IList(Of WritableT) = row(New IntWritable(1), New LongWritable(20), New DoubleWritable(-0.5), New LongWritable(1000), New Text("b"), New Text("0"))
            Dim r2 As IList(Of WritableT) = row(New IntWritable(2), New LongWritable(10), New DoubleWritable(0.5), New LongWritable(500), New Text("a"), New Text("0"))

            For Each col As String In New String() {"int", "double"}
                Dim asc As New NumericalColumnComparator(col)
                asc.Schema = schema
                assertTrue(asc.Compare(r1, r2) < 0)
                assertTrue(asc.Compare(r2, r1) > 0)
                assertEquals(0, asc.Compare(r1, r1))

                Dim desc As New NumericalColumnComparator(col, False)
                desc.Schema = schema
                assertTrue(desc.Compare(r1, r2) > 0)
                assertTrue(desc.Compare(r2, r1) < 0)
            Next col

            For Each col As String In New String() {"long", "time"}
                Dim asc As New NumericalColumnComparator(col)
                asc.Schema = schema
                assertTrue(asc.Compare(r1, r2) > 0)
                assertTrue(asc.Compare(r2, r1) < 0)
            Next col

            Dim timeComparator As New NumericalColumnComparator("time")
            timeComparator.Schema = schema
            assertEquals(ColumnType.Time, timeComparator.getColumnType())
            assertEquals(3, timeComparator.getColumnIdx())
            assertTrue(timeComparator.isAscending())
        End Sub

        <TestMethod>
        Public Sub testNumericalColumnComparatorDoubleSemantics()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnDouble("d", Nothing, Nothing, True, True).build()
            Dim cmp As New NumericalColumnComparator("d")
            cmp.Schema = schema
            'Java Double.compare: NaN is greater than everything and equal to itself; -0.0 < 0.0
            assertTrue(cmp.Compare(row(New DoubleWritable(Double.NaN)), row(New DoubleWritable(Double.PositiveInfinity))) > 0)
            assertEquals(0, cmp.Compare(row(New DoubleWritable(Double.NaN)), row(New DoubleWritable(Double.NaN))))
            assertTrue(cmp.Compare(row(New DoubleWritable(-0.0)), row(New DoubleWritable(0.0))) < 0)
        End Sub

        <TestMethod>
        Public Sub testNumericalColumnComparatorRejectsOtherTypes()
            Dim schema As SchemaT = numericSchema()
            For Each col As String In New String() {"str", "float"}
                Dim cmp As New NumericalColumnComparator(col)
                assertThrows(Of InvalidOperationException)(Sub() cmp.Schema = schema)
            Next col
            Dim missing As New NumericalColumnComparator("missing")
            assertThrows(Of KeyNotFoundException)(Sub() missing.Schema = schema)
        End Sub

        <TestMethod>
        Public Sub testStringComparator()
            Dim schema As SchemaT = numericSchema()
            Dim cmp As SequenceComparatorT = New StringComparator("str")
            cmp.Schema = schema
            Dim r1 As IList(Of WritableT) = row(New IntWritable(1), New LongWritable(20), New DoubleWritable(-0.5), New LongWritable(1000), New Text("b"), New Text("0"))
            Dim r2 As IList(Of WritableT) = row(New IntWritable(2), New LongWritable(10), New DoubleWritable(0.5), New LongWritable(500), New Text("a"), New Text("0"))
            Dim r3 As IList(Of WritableT) = row(New IntWritable(2), New LongWritable(10), New DoubleWritable(0.5), New LongWritable(500), New Text("B"), New Text("0"))
            assertTrue(cmp.Compare(r1, r2) > 0)
            assertTrue(cmp.Compare(r2, r1) < 0)
            assertEquals(0, cmp.Compare(r1, r1))
            'Ordinal comparison: upper case before lower case
            assertTrue(cmp.Compare(r3, r1) < 0)
        End Sub

        <TestMethod>
        Public Sub testComparatorColumnOpMembersEqualityAndToString()
            Dim schema As SchemaT = numericSchema()
            Dim cmp As New NumericalColumnComparator("long", False)
            'Setting the input schema only stores it; setting the schema also resolves the index
            cmp.InputSchema = schema
            assertTrue(schema Is cmp.InputSchema)
            assertEquals(-1, cmp.getColumnIdx())
            cmp.Schema = schema
            assertEquals(1, cmp.getColumnIdx())
            assertTrue(schema Is cmp.getSchema())
            assertTrue(schema Is cmp.transform(schema))
            assertEquals("long", cmp.getColumnName())
            assertEquals("long", cmp.columnName())
            assertEquals("long", cmp.outputColumnName())
            assertEquals(New String() {"long"}, cmp.columnNames())
            assertEquals(New String() {"long"}, cmp.outputColumnNames())
            assertEquals("NumericalColumnComparator(columnName=""long"",ascending=false)", cmp.ToString())

            'Equality: column name and order, not the schema
            assertEquals(New NumericalColumnComparator("long", False), cmp)
            assertEquals(New NumericalColumnComparator("long", False).GetHashCode(), cmp.GetHashCode())
            assertFalse(cmp.Equals(New NumericalColumnComparator("long", True)))
            assertFalse(cmp.Equals(New NumericalColumnComparator("int", False)))

            Dim s As New StringComparator("str")
            assertEquals("StringComparator()", s.ToString())
            assertEquals(New StringComparator("str"), s)
            assertFalse(s.Equals(New StringComparator("other")))
            assertFalse(s.Equals(New NumericalColumnComparator("str")))
        End Sub

        ' ---- ConvertToSequence / ConvertFromSequence ----

        <TestMethod>
        Public Sub testConvertToSequence()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnLong("time").addColumnDouble("value").build()
            Dim comparator As New NumericalColumnComparator("time")
            Dim cts As New ConvertToSequenceT("key", comparator)

            Dim seqSchema As SequenceSchemaT = cts.transform(schema)
            assertEquals(schema.getColumnMetaData(), seqSchema.getColumnMetaData())
            assertEquals(New SequenceSchemaT(schema.getColumnMetaData()), seqSchema)

            'Setting the input schema hands the sequence schema to the comparator
            cts.InputSchema = schema
            assertTrue(schema Is cts.InputSchema)
            assertTrue(schema Is cts.getInputSchema())
            assertEquals(1, comparator.getColumnIdx())
            assertTrue(TypeOf comparator.getSchema() Is SequenceSchemaT)

            assertFalse(cts.isSingleStepSequencesMode())
            assertEquals(New String() {"key"}, cts.getKeyColumns())
            assertTrue(comparator Is cts.getComparator())
            assertEquals("ConvertToSequence(keyColumn=""key"",comparator=NumericalColumnComparator(columnName=""time"",ascending=true))", cts.ToString())

            'Compound keys
            Dim compound As New ConvertToSequenceT(New List(Of String) From {"key", "value"}, New NumericalColumnComparator("time"))
            assertEquals(New String() {"key", "value"}, compound.getKeyColumns())
            assertEquals("ConvertToSequence(keyColumns=""[key, value]"",comparator=NumericalColumnComparator(columnName=""time"",ascending=true))", compound.ToString())

            'Single step sequences mode: no key, no comparator
            Dim singleStep As New ConvertToSequenceT(True, New String() {}, Nothing)
            singleStep.InputSchema = schema
            assertTrue(singleStep.isSingleStepSequencesMode())
            assertEquals("ConvertToSequence()", singleStep.ToString())

            'Equality ignores the input schema
            Dim other As New ConvertToSequenceT("key", New NumericalColumnComparator("time"))
            assertEquals(other, cts)
            assertEquals(other.GetHashCode(), cts.GetHashCode())
            assertFalse(cts.Equals(compound))
            assertFalse(cts.Equals(New ConvertToSequenceT("key", New NumericalColumnComparator("time", False))))
        End Sub

        <TestMethod>
        Public Sub testConvertFromSequence()
            Dim seqSchema As SequenceSchemaT = DirectCast((New SequenceSchemaT.Builder()).minSequenceLength(1).addColumnString("key").addColumnInteger("value").build(), SequenceSchemaT)
            Dim cfs As New ConvertFromSequenceT()
            Dim outSchema As SchemaT = cfs.transform(seqSchema)
            assertFalse(TypeOf outSchema Is SequenceSchemaT)
            assertEquals(seqSchema.getColumnMetaData(), outSchema.getColumnMetaData())

            cfs.InputSchema = seqSchema
            assertTrue(seqSchema Is cfs.getInputSchema())
            cfs.setInputSchema(Nothing)
            assertNull(cfs.InputSchema)

            assertEquals("ConvertFromSequence()", cfs.ToString())
            Dim other As New ConvertFromSequenceT()
            other.InputSchema = seqSchema
            assertEquals(other, cfs)
            assertEquals(other.GetHashCode(), cfs.GetHashCode())
        End Sub

        ' ---- trims ----

        Private Shared Function sequence4() As IList(Of IList(Of WritableT))
            Return New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)),
                row(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)),
                row(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}
        End Function

        <TestMethod>
        Public Sub testTrimSequenceTransform()
            ' Java: TestTransforms.testTrimSequenceTransform
            Dim seq As IList(Of IList(Of WritableT)) = sequence4()

            Dim expTrimFirst As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)),
                row(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}

            Dim expTrimLast As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5))}

            Dim tFirst As New SequenceTrimTransform(2, True)
            Dim tLast As New SequenceTrimTransform(2, False)

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnsDouble("col%d", 0, 2).build()
            tFirst.InputSchema = schema
            tLast.InputSchema = schema

            assertEquals(expTrimFirst, tFirst.mapSequence(seq))
            assertEquals(expTrimLast, tLast.mapSequence(seq))
        End Sub

        <TestMethod>
        Public Sub testTrimSequenceTransformOtherMembers()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnsDouble("col%d", 0, 2).build()
            Dim t As New SequenceTrimTransform(4, True)
            t.InputSchema = schema
            'Trimming everything (or more) gives an empty sequence
            assertEquals(0, t.mapSequence(sequence4()).Count)
            assertEquals(0, (New SequenceTrimTransform(10, False)).mapSequence(sequence4()).Count)
            assertEquals(4, (New SequenceTrimTransform(0, False)).mapSequence(sequence4()).Count)

            assertTrue(schema Is t.transform(schema))
            assertEquals(New String() {"col0", "col1", "col2"}, t.outputColumnNames())
            assertEquals(New String() {"col0", "col1", "col2"}, t.columnNames())
            assertNull(t.outputColumnName())
            assertNull(t.columnName())
            assertEquals(4, t.getNumStepsToTrim())
            assertTrue(t.isTrimFromStart())
            assertThrows(Of NotSupportedException)(Sub() t.map(row(New DoubleWritable(1))))
            assertThrows(Of NotSupportedException)(Sub() t.map(CObj("x")))
            assertThrows(Of NotSupportedException)(Sub() t.mapSequence(CObj("x")))

            assertEquals(New SequenceTrimTransform(4, True), t)
            assertFalse(t.Equals(New SequenceTrimTransform(4, False)))
            assertEquals("SequenceTrimTransform(numStepsToTrim=4, trimFromStart=true)", t.ToString())
        End Sub

        <TestMethod>
        Public Sub testSequenceTrimToLengthTransform()
            ' Java: TestTransforms.testSequenceTrimToLengthTransform (without the TransformProcess and JSON parts)
            Dim seq As IList(Of IList(Of WritableT)) = sequence4()
            Dim expTrimLength3 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {seq(0), seq(1), seq(2)}

            Dim t As TransformT = New SequenceTrimToLengthTransform(3, SequenceTrimToLengthTransform.Mode.TRIM, Nothing)
            assertEquals(expTrimLength3, t.mapSequence(seq))

            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {seq(0), seq(1)}
            assertEquals(seq2, t.mapSequence(seq2))
            assertEquals(expTrimLength3, t.mapSequence(expTrimLength3))
        End Sub

        <TestMethod>
        Public Sub testSequenceTrimToLengthTransformTrimOrPad()
            ' Java: TestTransforms.testSequenceTrimToLengthTransformTrimOrPad (without the TransformProcess and JSON parts)
            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(sequence4())
            seq.Add(row(New DoubleWritable(12), New DoubleWritable(13), New DoubleWritable(14)))

            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {seq(0), seq(1)}

            Dim expTrimLength4 As IList(Of IList(Of WritableT)) = sequence4()

            Dim pad As IList(Of WritableT) = row(New DoubleWritable(900), New DoubleWritable(901), New DoubleWritable(902))
            Dim t As TransformT = New SequenceTrimToLengthTransform(4, SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, pad)

            assertEquals(expTrimLength4, t.mapSequence(seq))

            Dim exp2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {seq(0), seq(1), pad, pad}
            assertEquals(exp2, t.mapSequence(seq2))

            'An empty sequence is padded completely
            assertEquals(New List(Of IList(Of WritableT)) From {pad, pad, pad, pad}, t.mapSequence(New List(Of IList(Of WritableT))()))

            'Padding with the wrong number of values
            Dim wrongPad As TransformT = New SequenceTrimToLengthTransform(4, SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, row(New DoubleWritable(0)))
            assertThrows(Of InvalidOperationException)(Sub() wrongPad.mapSequence(seq2))
        End Sub

        <TestMethod>
        Public Sub testSequenceTrimToLengthTransformOtherMembers()
            assertThrows(Of InvalidOperationException)(Sub()
                                   Dim created As Object = New SequenceTrimToLengthTransform(0, SequenceTrimToLengthTransform.Mode.TRIM, Nothing)
                               End Sub)
            assertThrows(Of InvalidOperationException)(Sub()
                                   Dim created As Object = New SequenceTrimToLengthTransform(3, SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, Nothing)
                               End Sub)

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnsDouble("col%d", 0, 1).build()
            Dim pad As IList(Of WritableT) = row(New DoubleWritable(0), New DoubleWritable(0))
            Dim t As New SequenceTrimToLengthTransform(3, SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, pad)
            t.InputSchema = schema
            assertTrue(schema Is t.InputSchema)
            assertTrue(schema Is t.transform(schema))
            assertEquals(New String() {"col0", "col1"}, t.outputColumnNames())
            assertEquals(New String() {"col0", "col1"}, t.columnNames())
            assertNull(t.outputColumnName())
            assertNull(t.columnName())
            assertEquals(3, t.getMaxLength())
            assertEquals(SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, t.getMode())
            assertTrue(pad Is t.getPad())
            assertThrows(Of NotSupportedException)(Sub() t.map(pad))

            assertEquals(New SequenceTrimToLengthTransform(3, SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, row(New DoubleWritable(0), New DoubleWritable(0))), t)
            assertEquals(New SequenceTrimToLengthTransform(3, SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, row(New DoubleWritable(0), New DoubleWritable(0))).GetHashCode(), t.GetHashCode())
            assertFalse(t.Equals(New SequenceTrimToLengthTransform(3, SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, row(New DoubleWritable(1), New DoubleWritable(0)))))
            assertFalse(t.Equals(New SequenceTrimToLengthTransform(3, SequenceTrimToLengthTransform.Mode.TRIM, pad)))
            assertEquals("SequenceTrimToLengthTransform(maxLength=3, mode=TRIM, pad=null)", (New SequenceTrimToLengthTransform(3, SequenceTrimToLengthTransform.Mode.TRIM, Nothing)).ToString())

            assertEquals(SequenceTrimToLengthTransform.Mode.TRIM_OR_PAD, SequenceTrimToLengthTransformExtensions.ModeValueOf("TRIM_OR_PAD"))
            assertEquals(2, SequenceTrimToLengthTransformExtensions.ModeValues().Length)
            assertThrows(Of ArgumentException)(Sub() SequenceTrimToLengthTransformExtensions.ModeValueOf("trim"))
            assertThrows(Of ArgumentException)(Sub() SequenceTrimToLengthTransformExtensions.ModeValueOf("0"))
        End Sub

        ' ---- expansion ----

        ''' <summary>Expands a text column into one time step per character (the character code as an int).</summary>
        <Serializable>
        Private Class CharacterExpansion
            Inherits BaseSequenceExpansionTransform

            Public Sub New(ByVal columnName As String, ByVal newColumnName As String)
                MyBase.New(New List(Of String) From {columnName}, New List(Of String) From {newColumnName})
            End Sub

            Protected Overrides Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)
                Return New List(Of ColumnMetaData) From {New IntegerMetaData(expandedColumnNames(0), 0, 65535)}
            End Function

            Protected Overrides Function expandTimeStep(ByVal currentStepValues As IList(Of WritableT)) As IList(Of IList(Of WritableT))
                Dim [out] As New List(Of IList(Of WritableT))()
                For Each c As Char In currentStepValues(0).ToString()
                    [out].Add(New List(Of WritableT) From {New IntWritable(AscW(c))})
                Next c
                Return [out]
            End Function
        End Class

        ''' <summary>Expands two columns at once: a count n and a text t into n steps of (i, t + i).</summary>
        <Serializable>
        Private Class PairExpansion
            Inherits BaseSequenceExpansionTransform

            Public Sub New()
                MyBase.New(New List(Of String) From {"n", "t"}, New List(Of String) From {"i", "ti"})
            End Sub

            Protected Overrides Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)
                Dim t As ColumnMetaData = origColumnMeta(1).clone()
                t.Name = expandedColumnNames(1)
                Return New List(Of ColumnMetaData) From {New IntegerMetaData(expandedColumnNames(0)), t}
            End Function

            Protected Overrides Function expandTimeStep(ByVal currentStepValues As IList(Of WritableT)) As IList(Of IList(Of WritableT))
                Dim [out] As New List(Of IList(Of WritableT))()
                For i As Integer = 0 To currentStepValues(0).toInt() - 1
                    [out].Add(New List(Of WritableT) From {New IntWritable(i), New Text(currentStepValues(1).ToString() & i)})
                Next i
                Return [out]
            End Function
        End Class

        <TestMethod>
        Public Sub testSequenceExpansion()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnLong("time").addColumnString("text").addColumnDouble("value").build()
            Dim t As New CharacterExpansion("text", "char")
            t.InputSchema = schema

            Dim outSchema As SchemaT = t.transform(schema)
            assertTrue(TypeOf outSchema Is SequenceSchemaT)
            assertEquals(New List(Of String) From {"time", "char", "value"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.Long, ColumnType.Integer, ColumnType.Double}, outSchema.getColumnTypes())

            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(1), New Text("ab"), New DoubleWritable(0.5)),
                row(New LongWritable(2), New Text(""), New DoubleWritable(1.5)),
                row(New LongWritable(3), New Text("c"), New DoubleWritable(2.5))}
            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(1), New IntWritable(AscW("a"c)), New DoubleWritable(0.5)),
                row(New LongWritable(1), New IntWritable(AscW("b"c)), New DoubleWritable(0.5)),
                row(New LongWritable(3), New IntWritable(AscW("c"c)), New DoubleWritable(2.5))}
            assertEquals(expected, t.mapSequence(seq))

            assertEquals(New String() {"text"}, t.columnNames())
            assertEquals("text", t.columnName())
            assertEquals(New String() {"char"}, t.outputColumnNames())
            assertEquals("char", t.outputColumnName())
            assertThrows(Of NotSupportedException)(Sub() t.map(seq(0)))
            assertThrows(Of NotSupportedException)(Sub() t.map(CObj("x")))
            assertThrows(Of NotSupportedException)(Sub() t.mapSequence(CObj("x")))

            assertEquals(New CharacterExpansion("text", "char"), t)
            assertEquals(New CharacterExpansion("text", "char").GetHashCode(), t.GetHashCode())
            assertFalse(t.Equals(New CharacterExpansion("text", "chars")))
        End Sub

        <TestMethod>
        Public Sub testSequenceExpansionOfSeveralColumns()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("t").addColumnDouble("x").addColumnInteger("n").build()
            Dim t As New PairExpansion()
            t.InputSchema = schema
            'The expanded columns keep their positions (in schema order)
            assertEquals(New List(Of String) From {"ti", "x", "i"}, t.transform(schema).getColumnNames())

            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("p"), New DoubleWritable(1), New IntWritable(2)),
                row(New Text("q"), New DoubleWritable(2), New IntWritable(1))}
            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("p0"), New DoubleWritable(1), New IntWritable(0)),
                row(New Text("p1"), New DoubleWritable(1), New IntWritable(1)),
                row(New Text("q0"), New DoubleWritable(2), New IntWritable(0))}
            assertEquals(expected, t.mapSequence(seq))
        End Sub

        <TestMethod>
        Public Sub testSequenceExpansionArguments()
            assertThrows(Of ArgumentException)(Sub()
                                   Dim created As Object = New CharacterExpansionWithLists(New List(Of String)(), New List(Of String)())
                               End Sub)
            assertThrows(Of ArgumentNullException)(Sub()
                                   Dim created As Object = New CharacterExpansionWithLists(Nothing, New List(Of String)())
                               End Sub)
            assertThrows(Of ArgumentNullException)(Sub()
                                   Dim created As Object = New CharacterExpansionWithLists(New List(Of String) From {"a"}, Nothing)
                               End Sub)
        End Sub

        ''' <summary>Exposes the base constructor for the argument checks.</summary>
        Private Class CharacterExpansionWithLists
            Inherits BaseSequenceExpansionTransform

            Public Sub New(ByVal required As IList(Of String), ByVal expanded As IList(Of String))
                MyBase.New(required, expanded)
            End Sub

            Protected Overrides Function expandedColumnMetaDatas(ByVal origColumnMeta As IList(Of ColumnMetaData), ByVal expandedColumnNames As IList(Of String)) As IList(Of ColumnMetaData)
                Return origColumnMeta
            End Function

            Protected Overrides Function expandTimeStep(ByVal currentStepValues As IList(Of WritableT)) As IList(Of IList(Of WritableT))
                Return New List(Of IList(Of WritableT)) From {currentStepValues}
            End Function
        End Class

        ' ---- merge ----

        <TestMethod>
        Public Sub testSequenceMerge()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnLong("time").addColumnString("source").build()
            Dim comparator As New NumericalColumnComparator("time")
            comparator.Schema = schema
            Dim merge As New SequenceMerge(comparator)

            Dim seq1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(1), New Text("a1")), row(New LongWritable(5), New Text("a5")), row(New LongWritable(7), New Text("a7"))}
            Dim seq2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(5), New Text("b5")), row(New LongWritable(2), New Text("b2")), row(New LongWritable(7), New Text("b7"))}
            Dim seq3 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New LongWritable(5), New Text("c5"))}

            Dim merged As IList(Of IList(Of WritableT)) = merge.mergeSequences(New List(Of IList(Of IList(Of WritableT))) From {seq1, seq2, seq3})
            Dim sources As New List(Of String)()
            For Each [step] As IList(Of WritableT) In merged
                sources.Add([step](1).ToString())
            Next [step]
            'Sorted by time; equal times keep the order of the input sequences (stable sort, like Java)
            assertEquals(New List(Of String) From {"a1", "b2", "a5", "b5", "c5", "a7", "b7"}, sources)
            'The inputs are not modified
            assertEquals(3, seq1.Count)
            assertEquals("b5", seq2(0)(1).ToString())

            assertTrue(comparator Is merge.getComparator())
            assertEquals(New SequenceMerge(New NumericalColumnComparator("time")), merge)
            assertEquals("SequenceMerge(comparator=NumericalColumnComparator(columnName=""time"",ascending=true))", merge.ToString())
        End Sub

    End Class

End Namespace
