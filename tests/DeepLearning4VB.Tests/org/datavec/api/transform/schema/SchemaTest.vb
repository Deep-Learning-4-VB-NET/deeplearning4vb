Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports DeepLearning4VB.org.datavec.api.writable
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DeepLearning4VB.org.datavec.api.transform.schema
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone

Namespace org.datavec.api.transform.schema

    ''' <summary>
    ''' <see cref="SchemaT"/> and its builder: every <c>addColumn*</c> overload, the lookups by name and index,
    ''' the copied collections, <c>sameTypes</c>/<c>differences</c>/<c>newSchema</c>, equality, the text layout
    ''' and schema inference from a record.
    ''' </summary>
    <TestClass>
    Public Class SchemaTest

        <TestMethod>
        Public Sub testAddColumnOverloads()
            Dim schema As SchemaT = New SchemaT.Builder() _
                .addColumnFloat("f1") _
                .addColumnFloat("f2", -1.0F, 1.0F) _
                .addColumnFloat("f3", Nothing, 5.0F, True, False) _
                .addColumnDouble("d1") _
                .addColumnDouble("d2", -1.0, 1.0) _
                .addColumnDouble("d3", 0.0, Nothing, False, True) _
                .addColumnInteger("i1") _
                .addColumnInteger("i2", 0, 10) _
                .addColumnLong("l1") _
                .addColumnLong("l2", -100L, Nothing) _
                .addColumnString("s1") _
                .addColumnString("s2", "[a-z]+", 1, 3) _
                .addColumnCategorical("c1", "a", "b") _
                .addColumnCategorical("c2", Arrays.asList("x", "y", "z")) _
                .addColumnTime("t1", DateTimeZone.UTC) _
                .addColumnTime("t2", TimeZoneInfo.Utc) _
                .addColumnTime("t3", DateTimeZone.UTC, 1L, 100L) _
                .addColumnBoolean("b1") _
                .addColumn(New BinaryMetaData("raw")) _
                .build()
            assertEquals(19, schema.numColumns())
            assertEquals(Arrays.asList("f1", "f2", "f3", "d1", "d2", "d3", "i1", "i2", "l1", "l2", "s1", "s2", "c1", "c2", "t1", "t2", "t3", "b1", "raw"), schema.getColumnNames())
            assertEquals(Arrays.asList(ColumnType.Float, ColumnType.Float, ColumnType.Float, ColumnType.[Double], ColumnType.[Double], ColumnType.[Double],
                                       ColumnType.[Integer], ColumnType.[Integer], ColumnType.[Long], ColumnType.[Long], ColumnType.[String], ColumnType.[String],
                                       ColumnType.Categorical, ColumnType.Categorical, ColumnType.Time, ColumnType.Time, ColumnType.Time, ColumnType.[Boolean], ColumnType.Bytes),
                         schema.getColumnTypes())
            'the restrictions end up in the metadata
            assertEquals(New FloatMetaData("f1"), schema.getMetaData("f1"))
            assertEquals(New FloatMetaData("f2", -1.0F, 1.0F, False, False), schema.getMetaData("f2"))
            assertEquals(New FloatMetaData("f3", Nothing, 5.0F, True, False), schema.getMetaData("f3"))
            assertEquals(New DoubleMetaData("d1"), schema.getMetaData("d1"))
            assertEquals(New DoubleMetaData("d2", -1.0, 1.0, False, False), schema.getMetaData("d2"))
            assertEquals(New DoubleMetaData("d3", 0.0, Nothing, False, True), schema.getMetaData("d3"))
            assertEquals(New IntegerMetaData("i1"), schema.getMetaData("i1"))
            assertEquals(New IntegerMetaData("i2", 0, 10), schema.getMetaData("i2"))
            assertEquals(New LongMetaData("l1"), schema.getMetaData("l1"))
            assertEquals(New LongMetaData("l2", -100L, Nothing), schema.getMetaData("l2"))
            assertEquals(New StringMetaData("s1"), schema.getMetaData("s1"))
            assertEquals(New StringMetaData("s2", "[a-z]+", 1, 3), schema.getMetaData("s2"))
            assertEquals(New CategoricalMetaData("c1", "a", "b"), schema.getMetaData("c1"))
            assertEquals(New CategoricalMetaData("c2", "x", "y", "z"), schema.getMetaData("c2"))
            assertEquals(New TimeMetaData("t1", DateTimeZone.UTC), schema.getMetaData("t1"))
            assertEquals(New TimeMetaData("t2", DateTimeZone.UTC), schema.getMetaData("t2"))
            assertEquals(New TimeMetaData("t3", DateTimeZone.UTC, 1L, 100L), schema.getMetaData("t3"))
            assertEquals(New BooleanMetaData("b1"), schema.getMetaData("b1"))
            assertEquals(New BinaryMetaData("raw"), schema.getMetaData("raw"))
            'the builder methods are chainable and the builder returns itself
            Dim builder As New SchemaT.Builder()
            assertTrue(builder.addColumnInteger("x") Is builder)
            assertTrue(builder.addColumnsString("y", "z") Is builder)
            assertEquals(3, builder.build().numColumns())
        End Sub

        <TestMethod>
        Public Sub testAddColumnsOverloads()
            Dim schema As SchemaT = New SchemaT.Builder() _
                .addColumnsFloat("fa", "fb") _
                .addColumnsFloat("f_%d", 0, 1) _
                .addColumnsFloat("fr_%d", 5, 6, -1.0F, 1.0F, True, False) _
                .addColumnsDouble("da", "db") _
                .addColumnsDouble("d_%d", 0, 1) _
                .addColumnsDouble("dr_%d", 2, 2, 0.0, 10.0, False, True) _
                .addColumnsInteger("ia", "ib") _
                .addColumnsInteger("i_%d", 0, 2) _
                .addColumnsInteger("ir_%d", 7, 8, 0, 1) _
                .addColumnsLong("la") _
                .addColumnsLong("l_%d", 1, 2) _
                .addColumnsLong("lr_%d", 3, 3, Nothing, 5L) _
                .addColumnsString("sa", "sb") _
                .addColumnsString("s_%d", 0, 1) _
                .addColumnsString("sr_%d", 4, 5, "\d+", 1, 2) _
                .build()
            assertEquals(Arrays.asList("fa", "fb", "f_0", "f_1", "fr_5", "fr_6", "da", "db", "d_0", "d_1", "dr_2", "ia", "ib", "i_0", "i_1", "i_2", "ir_7", "ir_8",
                                       "la", "l_1", "l_2", "lr_3", "sa", "sb", "s_0", "s_1", "sr_4", "sr_5"), schema.getColumnNames())
            assertEquals(28, schema.numColumns())
            assertEquals(New FloatMetaData("f_1"), schema.getMetaData("f_1"))
            assertEquals(New FloatMetaData("fr_6", -1.0F, 1.0F, True, False), schema.getMetaData("fr_6"))
            assertEquals(New DoubleMetaData("d_0"), schema.getMetaData("d_0"))
            assertEquals(New DoubleMetaData("dr_2", 0.0, 10.0, False, True), schema.getMetaData("dr_2"))
            assertEquals(New IntegerMetaData("i_2"), schema.getMetaData("i_2"))
            assertEquals(New IntegerMetaData("ir_7", 0, 1), schema.getMetaData("ir_7"))
            assertEquals(New LongMetaData("l_2"), schema.getMetaData("l_2"))
            assertEquals(New LongMetaData("lr_3", Nothing, 5L), schema.getMetaData("lr_3"))
            assertEquals(New StringMetaData("s_1"), schema.getMetaData("s_1"))
            assertEquals(New StringMetaData("sr_5", "\d+", 1, 2), schema.getMetaData("sr_5"))
            'an empty index range adds nothing
            assertEquals(1, New SchemaT.Builder().addColumnsInteger("none_%d", 3, 2).addColumnInteger("x").build().numColumns())
        End Sub

        <TestMethod>
        Public Sub testColumnLookup()
            Dim schema As SchemaT = New SchemaT.Builder().addColumnInteger("a").addColumnString("b").addColumnDouble("c").build()
            assertEquals(3, schema.numColumns())
            assertEquals("a", schema.getName(0))
            assertEquals("c", schema.getName(2))
            assertEquals(0, schema.getIndexOfColumn("a"))
            assertEquals(2, schema.getIndexOfColumn("c"))
            assertArrayEquals(New Integer() {2, 0}, schema.getIndexOfColumns("c", "a"))
            assertArrayEquals(New Integer() {1, 2}, schema.getIndexOfColumns(DirectCast(New List(Of String)(New String() {"b", "c"}), ICollection(Of String))))
            assertArrayEquals(New Integer() {}, schema.getIndexOfColumns(New String() {}))
            assertTrue(schema.hasColumn("a"))
            assertTrue(schema.hasColumn("c"))
            assertFalse(schema.hasColumn("A"))
            assertFalse(schema.hasColumn("d"))
            assertFalse(schema.hasColumn(Nothing))
            assertEquals(ColumnType.[Integer], schema.getType(0))
            assertEquals(ColumnType.[Double], schema.getType(2))
            assertEquals(ColumnType.[String], schema.getType("b"))
            assertEquals(New StringMetaData("b"), schema.getMetaData(1))
            assertEquals(New StringMetaData("b"), schema.getMetaData("b"))
            assertTrue(schema.getMetaData(1) Is schema.getMetaData("b"))
            Dim index As IDictionary(Of String, Integer) = schema.getColumnNamesIndex()
            assertEquals(3, index.Count)
            assertEquals(1, index("b"))
            assertTrue(schema.ColumnNamesIndexMap Is index)
        End Sub

        <TestMethod>
        Public Sub testLookupErrors()
            Dim schema As SchemaT = New SchemaT.Builder().addColumnInteger("a").addColumnString("b").build()
            'unknown names: NoSuchElementException in Java
            Dim ex As KeyNotFoundException = assertThrows(Of KeyNotFoundException)(Sub() schema.getIndexOfColumn("nope"))
            assertTrue(ex.Message.Contains("nope"), ex.Message)
            assertThrows(Of KeyNotFoundException)(Sub() schema.getIndexOfColumn(Nothing))
            assertThrows(Of KeyNotFoundException)(Sub() schema.getIndexOfColumns("a", "nope"))
            assertThrows(Of KeyNotFoundException)(Sub() schema.getMetaData("nope"))
            'getType validates the index and the name
            assertThrows(Of ArgumentException)(Sub() schema.getType(-1))
            assertThrows(Of ArgumentException)(Sub() schema.getType(2))
            Dim ex2 As ArgumentException = assertThrows(Of ArgumentException)(Sub() schema.getType("nope"))
            assertTrue(ex2.Message.Contains("does not exist"), ex2.Message)
            'getName / getMetaData by index are plain list accesses
            assertThrows(Of ArgumentOutOfRangeException)(Sub() schema.getName(5))
            assertThrows(Of ArgumentOutOfRangeException)(Sub() schema.getMetaData(5))
        End Sub

        <TestMethod>
        Public Sub testColumnCollectionsAreCopies()
            Dim schema As SchemaT = New SchemaT.Builder().addColumnInteger("a").addColumnString("b").build()
            Dim names As IList(Of String) = schema.getColumnNames()
            assertEquals(Arrays.asList("a", "b"), names)
            names.Add("c")
            assertEquals(2, schema.numColumns())
            assertEquals(Arrays.asList("a", "b"), schema.ColumnNames)
            assertFalse(schema.getColumnNames() Is schema.getColumnNames())
            Dim types As IList(Of ColumnType) = schema.getColumnTypes()
            assertEquals(Arrays.asList(ColumnType.[Integer], ColumnType.[String]), types)
            types.Clear()
            assertEquals(2, schema.getColumnTypes().Count)
            assertEquals(schema.getColumnTypes(), schema.ColumnTypes)
            Dim meta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
            assertEquals(Arrays.asList(Of ColumnMetaData)(New IntegerMetaData("a"), New StringMetaData("b")), meta)
            meta.RemoveAt(0)
            assertEquals(2, schema.getColumnMetaData().Count)
            assertEquals(schema.getColumnMetaData(), schema.ColumnMetaData)
            'the metadata objects themselves are shared, not copied
            assertTrue(schema.getColumnMetaData()(0) Is schema.getMetaData(0))
        End Sub

        <TestMethod>
        Public Sub testSameTypesAndDifferences()
            Dim a As SchemaT = New SchemaT.Builder().addColumnInteger("x").addColumnString("y").build()
            Dim sameTypes As SchemaT = New SchemaT.Builder().addColumnInteger("other", 0, 1).addColumnString("names").build()
            Dim fewer As SchemaT = New SchemaT.Builder().addColumnInteger("x").build()
            Dim swapped As SchemaT = New SchemaT.Builder().addColumnString("y").addColumnInteger("x").build()
            assertTrue(a.sameTypes(a))
            assertTrue(a.sameTypes(sameTypes))
            assertTrue(sameTypes.sameTypes(a))
            assertFalse(a.sameTypes(fewer))
            assertFalse(fewer.sameTypes(a))
            assertFalse(a.sameTypes(swapped))
            'differences: the metadata of the other schema that this schema does not contain (in order)
            assertEquals(0, a.differences(a).Count)
            assertEquals(0, a.differences(swapped).Count)
            assertEquals(0, a.differences(fewer).Count)
            assertEquals(Arrays.asList(Of ColumnMetaData)(New IntegerMetaData("other", 0, 1), New StringMetaData("names")), a.differences(sameTypes))
            Dim extended As SchemaT = New SchemaT.Builder().addColumnInteger("x").addColumnString("y").addColumnDouble("z").build()
            assertEquals(Arrays.asList(Of ColumnMetaData)(New DoubleMetaData("z")), a.differences(extended))
            assertEquals(0, extended.differences(a).Count)
            Dim rebounded As SchemaT = New SchemaT.Builder().addColumnInteger("x", 0, 5).addColumnString("y").build()
            assertEquals(Arrays.asList(Of ColumnMetaData)(New IntegerMetaData("x", 0, 5)), a.differences(rebounded))
        End Sub

        <TestMethod>
        Public Sub testConstructorAndNewSchema()
            Dim meta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(New ColumnMetaData() {New IntegerMetaData("a"), New StringMetaData("b")})
            Dim schema As New SchemaT(meta)
            assertEquals(2, schema.numColumns())
            assertEquals(Arrays.asList("a", "b"), schema.getColumnNames())
            assertTrue(schema.getMetaData(0) Is meta(0))
            assertEquals(schema, New SchemaT.Builder().addColumnInteger("a").addColumnString("b").build())
            'the metadata list is required
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New SchemaT(New List(Of ColumnMetaData)())
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New SchemaT(DirectCast(Nothing, IList(Of ColumnMetaData)))
                                               End Sub)
            'newSchema builds a plain schema from the given metadata
            Dim other As SchemaT = schema.newSchema(New List(Of ColumnMetaData)(New ColumnMetaData() {New DoubleMetaData("d")}))
            assertEquals(GetType(SchemaT), CObj(other).GetType())
            assertEquals(1, other.numColumns())
            assertEquals(ColumnType.[Double], other.getType("d"))
            assertEquals(2, schema.numColumns())
        End Sub

        <TestMethod>
        Public Sub testEqualsAndHashCode()
            Dim a As SchemaT = New SchemaT.Builder().addColumnInteger("x", 0, 10).addColumnCategorical("c", "p", "q").build()
            Dim b As SchemaT = New SchemaT.Builder().addColumnInteger("x", 0, 10).addColumnCategorical("c", "p", "q").build()
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertTrue(a.Equals(a))
            assertFalse(a.Equals(Nothing))
            assertFalse(a.Equals("x"))
            assertNotEquals(a, New SchemaT.Builder().addColumnInteger("x", 0, 11).addColumnCategorical("c", "p", "q").build())
            assertNotEquals(a, New SchemaT.Builder().addColumnInteger("x", 0, 10).addColumnCategorical("c", "q", "p").build())
            assertNotEquals(a, New SchemaT.Builder().addColumnInteger("x", 0, 10).build())
            assertNotEquals(a, New SchemaT.Builder().addColumnCategorical("c", "p", "q").addColumnInteger("x", 0, 10).build())
            assertNotEquals(a, New SchemaT.Builder().addColumnLong("x", 0L, 10L).addColumnCategorical("c", "p", "q").build())
            'a sequence schema with the same columns is a different kind of schema
            Dim seq As SchemaT = New SequenceSchema.Builder().addColumnInteger("x", 0, 10).addColumnCategorical("c", "p", "q").build()
            assertNotEquals(a, seq)
            assertNotEquals(seq, a)
        End Sub

        <TestMethod>
        Public Sub testToStringLayout()
            Dim schema As SchemaT = New SchemaT.Builder().addColumnInteger("a").addColumnString("bb").build()
            Dim expected As String = "Schema():" & vbLf &
                "idx   name      type           meta data" & vbLf &
                "0     ""a""       Integer        IntegerMetaData(name=""a"",)" & vbLf &
                "1     ""bb""      String         StringMetaData(name=""bb"",)" & vbLf
            assertEquals(expected, schema.ToString())
            'the name column is padded to the longest name plus 8
            Dim wide As SchemaT = New SchemaT.Builder().addColumnDouble("averyveryverylongname", 0.0, 1.0).build()
            Dim lines() As String = wide.ToString().Split(New String() {vbLf}, StringSplitOptions.None)
            assertEquals("Schema():", lines(0))
            assertEquals("idx   " & "name".PadRight(21 + 8) & "type           meta data", lines(1))
            assertEquals("0     " & """averyveryverylongname""".PadRight(29) & "Double         DoubleMetaData(name=""averyveryverylongname"",minAllowed=0.0,maxAllowed=1.0,allowNaN=false,allowInfinite=false)", lines(2))
            assertEquals("", lines(3))
            assertEquals(4, lines.Length)
            assertEquals("%-5s", SchemaT.padRight("%-5s", 3))
            assertEquals("ab   ", SchemaT.padRight("ab", 5))
        End Sub

        <TestMethod>
        Public Sub testInfer()
            Dim record As IList(Of WritableT) = Arrays.asList(Of WritableT)(New DoubleWritable(1.5), New IntWritable(2), New LongWritable(3L), New FloatWritable(4.0F), New Text("five"))
            Dim schema As SchemaT = SchemaT.infer(record)
            assertEquals(5, schema.numColumns())
            assertEquals(Arrays.asList("0", "1", "2", "3", "4"), schema.getColumnNames())
            assertEquals(Arrays.asList(ColumnType.[Double], ColumnType.[Integer], ColumnType.[Long], ColumnType.Float, ColumnType.[String]), schema.getColumnTypes())
            assertEquals(New DoubleMetaData("0"), schema.getMetaData(0))
            assertEquals(New StringMetaData("4"), schema.getMetaData(4))
            'inferMultiple only looks at the first record
            Dim second As IList(Of WritableT) = Arrays.asList(Of WritableT)(New Text("x"))
            Dim multi As SchemaT = SchemaT.inferMultiple(New List(Of IList(Of WritableT))(New IList(Of WritableT)() {record, second}))
            assertEquals(schema, multi)
            assertEquals(schema, SchemaT.inferMultiple(New List(Of IList(Of WritableT))(New IList(Of WritableT)() {record})))
            'other writables cannot be inferred
            Dim ex As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() SchemaT.infer(Arrays.asList(Of WritableT)(New IntWritable(1), New BooleanWritable(True))))
            assertTrue(ex.Message.Contains("BooleanWritable"), ex.Message)
            assertThrows(Of InvalidOperationException)(Sub() SchemaT.infer(Arrays.asList(Of WritableT)(NullWritable.INSTANCE)))
        End Sub

        ' Schema.toJson/fromJson/toYaml/fromYaml are not tested: the JSON serde arrives with the serde wave (YAML is not supported).

    End Class

End Namespace
