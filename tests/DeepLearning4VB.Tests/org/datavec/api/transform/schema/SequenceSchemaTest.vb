Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports DeepLearning4VB.org.datavec.api.writable
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DeepLearning4VB.org.datavec.api.transform.schema
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.schema

    ''' <summary><see cref="SequenceSchema"/>: the builder, the sequence length restrictions, equality, the text layout and inference.</summary>
    <TestClass>
    Public Class SequenceSchemaTest

        ''' <summary>Builds a sequence schema: the inherited addColumn* methods return the base Schema.Builder.</summary>
        Private Shared Function BuildSeq(ByVal builder As SchemaT.Builder) As SequenceSchema
            Return DirectCast(builder.build(), SequenceSchema)
        End Function

        <TestMethod>
        Public Sub testBuilderAndLengths()
            Dim builder As New SequenceSchema.Builder()
            assertTrue(builder.minSequenceLength(1) Is builder)
            assertTrue(builder.maxSequenceLength(5) Is builder)
            builder.addColumnInteger("a").addColumnString("b")
            Dim built As SchemaT = builder.build()
            assertTrue(TypeOf built Is SequenceSchema)
            Dim seq As SequenceSchema = DirectCast(built, SequenceSchema)
            assertEquals(1, seq.getMinSequenceLength().Value)
            assertEquals(5, seq.getMaxSequenceLength().Value)
            assertEquals(1, seq.MinSequenceLength.Value)
            assertEquals(5, seq.MaxSequenceLength.Value)
            assertEquals(2, seq.numColumns())
            assertEquals(Arrays.asList(ColumnType.[Integer], ColumnType.[String]), seq.getColumnTypes())
            'buildSequence gives the static type directly
            Dim typed As SequenceSchema = BuildSeq(New SequenceSchema.Builder().addColumnDouble("d"))
            assertNull(typed.getMinSequenceLength())
            assertNull(typed.getMaxSequenceLength())
            assertEquals(1, typed.numColumns())
            'constructors
            Dim meta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(New ColumnMetaData() {New IntegerMetaData("a"), New StringMetaData("b")})
            Dim fromMeta As New SequenceSchema(meta)
            assertNull(fromMeta.getMinSequenceLength())
            assertEquals(2, fromMeta.numColumns())
            Dim bounded As New SequenceSchema(meta, 1, 5)
            assertEquals(seq, bounded)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New SequenceSchema(New List(Of ColumnMetaData)(), 1, 2)
                                               End Sub)
        End Sub

        <TestMethod>
        Public Sub testEqualsHashAndNewSchema()
            Dim a As SequenceSchema = BuildSeq(New SequenceSchema.Builder().minSequenceLength(2).maxSequenceLength(4).addColumnInteger("x"))
            Dim b As SequenceSchema = BuildSeq(New SequenceSchema.Builder().minSequenceLength(2).maxSequenceLength(4).addColumnInteger("x"))
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertNotEquals(a, BuildSeq(New SequenceSchema.Builder().minSequenceLength(2).addColumnInteger("x")))
            assertNotEquals(a, BuildSeq(New SequenceSchema.Builder().minSequenceLength(2).maxSequenceLength(5).addColumnInteger("x")))
            assertNotEquals(a, BuildSeq(New SequenceSchema.Builder().minSequenceLength(2).maxSequenceLength(4).addColumnLong("x")))
            assertNotEquals(a, New SchemaT.Builder().addColumnInteger("x").build())
            assertTrue(a.sameTypes(New SchemaT.Builder().addColumnInteger("y").build()))
            'newSchema keeps the length restrictions
            Dim renamed As SchemaT = a.newSchema(New List(Of ColumnMetaData)(New ColumnMetaData() {New IntegerMetaData("y"), New DoubleMetaData("z")}))
            assertTrue(TypeOf renamed Is SequenceSchema)
            assertEquals(2, DirectCast(renamed, SequenceSchema).getMinSequenceLength().Value)
            assertEquals(4, DirectCast(renamed, SequenceSchema).getMaxSequenceLength().Value)
            assertEquals(Arrays.asList("y", "z"), renamed.getColumnNames())
            assertEquals(1, a.numColumns())
        End Sub

        <TestMethod>
        Public Sub testToStringLayout()
            Dim seq As SequenceSchema = BuildSeq(New SequenceSchema.Builder().minSequenceLength(1).maxSequenceLength(5).addColumnInteger("a").addColumnString("bb"))
            Dim expected As String = "SequenceSchema(minSequenceLength=1,maxSequenceLength=5)" & vbLf &
                "idx   name      type           meta data" & vbLf &
                "0     ""a""       Integer        IntegerMetaData(name=""a"",)" & vbLf &
                "1     ""bb""      String         StringMetaData(name=""bb"",)" & vbLf
            assertEquals(expected, seq.ToString())
            assertTrue(BuildSeq(New SequenceSchema.Builder().addColumnInteger("a")).ToString().StartsWith("SequenceSchema()" & vbLf, StringComparison.Ordinal))
            assertTrue(BuildSeq(New SequenceSchema.Builder().maxSequenceLength(3).addColumnInteger("a")).ToString().StartsWith("SequenceSchema(maxSequenceLength=3)" & vbLf, StringComparison.Ordinal))
            assertTrue(BuildSeq(New SequenceSchema.Builder().minSequenceLength(3).addColumnInteger("a")).ToString().StartsWith("SequenceSchema(minSequenceLength=3)" & vbLf, StringComparison.Ordinal))
        End Sub

        <TestMethod>
        Public Sub testInferSequence()
            'as in Java, the inference tests the time steps (lists) against the writable types, so any non-empty
            'sequence is rejected; an empty one yields an empty schema with zero length restrictions
            Dim empty As SequenceSchema = SequenceSchema.inferSequence(New List(Of IList(Of WritableT))())
            assertEquals(0, empty.numColumns())
            assertEquals(0, empty.getMinSequenceLength().Value)
            assertEquals(0, empty.getMaxSequenceLength().Value)
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            sequence.Add(Arrays.asList(Of WritableT)(New IntWritable(1)))
            Dim ex As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() SequenceSchema.inferSequence(sequence))
            assertTrue(ex.Message.Contains("Illegal writable"), ex.Message)
            Dim multi As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            multi.Add(sequence)
            assertThrows(Of InvalidOperationException)(Sub() SequenceSchema.inferSequenceMulti(multi))
        End Sub

    End Class

End Namespace
