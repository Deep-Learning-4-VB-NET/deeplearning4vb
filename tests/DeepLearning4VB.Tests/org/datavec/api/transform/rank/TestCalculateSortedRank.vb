Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports CalculateSortedRank = DeepLearning4VB.org.datavec.api.transform.rank.CalculateSortedRank
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DoubleWritableComparator = DeepLearning4VB.org.datavec.api.writable.comparator.DoubleWritableComparator
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports LongWritableComparator = DeepLearning4VB.org.datavec.api.writable.comparator.LongWritableComparator
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema

Namespace org.datavec.api.transform.rank

    ''' <summary>
    ''' Tests of <see cref="CalculateSortedRank"/> (the sorting itself is done by the executor, which is not ported;
    ''' the Java test of the rank values lives in datavec-local).
    ''' </summary>
    <TestClass>
    Public Class TestCalculateSortedRank

        Private Shared Function schema() As SchemaT
            Return (New SchemaT.Builder()).addColumnString("text").addColumnDouble("value").build()
        End Function

        <TestMethod>
        Public Sub testTransformAddsRankColumn()
            Dim csr As New CalculateSortedRank("rank", "value", New DoubleWritableComparator())
            Dim outSchema As SchemaT = csr.transform(schema())
            assertEquals(New List(Of String) From {"text", "value", "rank"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.String, ColumnType.Double, ColumnType.Long}, outSchema.getColumnTypes())
            Dim rankMeta As LongMetaData = DirectCast(outSchema.getMetaData("rank"), LongMetaData)
            assertEquals(CType(0L, Long?), rankMeta.getMinAllowedValue())
            assertNull(rankMeta.getMaxAllowedValue())
            'The input schema is not modified
            assertEquals(2, schema().numColumns())
        End Sub

        <TestMethod>
        Public Sub testSequenceSchemaNotSupported()
            Dim csr As New CalculateSortedRank("rank", "value", New DoubleWritableComparator())
            Dim seq As SchemaT = (New SequenceSchemaT.Builder()).addColumnDouble("value").build()
            assertThrows(Of InvalidOperationException)(Sub() csr.transform(seq))
        End Sub

        <TestMethod>
        Public Sub testColumnOpMembers()
            Dim s As SchemaT = schema()
            Dim csr As New CalculateSortedRank("rank", "value", New DoubleWritableComparator(), False)
            csr.InputSchema = s
            assertTrue(s Is csr.InputSchema)
            assertTrue(s Is csr.getInputSchema())
            assertEquals("rank", csr.outputColumnName())
            assertEquals(New String() {"text", "value", "rank"}, csr.outputColumnNames())
            'outputColumnNames works on a copy of the column names (the Java list was the schema's own copy too)
            assertEquals(New List(Of String) From {"text", "value"}, s.getColumnNames())
            assertEquals(New String() {"text", "value"}, csr.columnNames())
            assertEquals("text", csr.columnName())
            assertEquals("rank", csr.getNewColumnName())
            assertEquals("value", csr.getSortOnColumn())
            assertFalse(csr.isAscending())
            assertTrue(TypeOf csr.getComparator() Is DoubleWritableComparator)
            assertTrue((New CalculateSortedRank("rank", "value", New DoubleWritableComparator())).isAscending())
        End Sub

        <TestMethod>
        Public Sub testEqualsAndToString()
            Dim a As New CalculateSortedRank("rank", "value", New DoubleWritableComparator())
            Dim b As New CalculateSortedRank("rank", "value", New DoubleWritableComparator(), True)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            'The input schema is not part of the equality
            a.InputSchema = schema()
            assertEquals(a, b)
            assertFalse(a.Equals(New CalculateSortedRank("rank", "value", New DoubleWritableComparator(), False)))
            assertFalse(a.Equals(New CalculateSortedRank("rank", "value", New LongWritableComparator())))
            assertFalse(a.Equals(New CalculateSortedRank("rank2", "value", New DoubleWritableComparator())))
            assertFalse(a.Equals(New CalculateSortedRank("rank", "text", New DoubleWritableComparator())))
            assertEquals("CalculateSortedRank(newColumnName=""rank"", comparator=DoubleWritableComparator())", a.ToString())
        End Sub

    End Class

End Namespace
