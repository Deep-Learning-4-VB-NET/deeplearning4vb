Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports JoinT = DeepLearning4VB.org.datavec.api.transform.join.Join
Imports JoinTypeExtensions = DeepLearning4VB.org.datavec.api.transform.join.JoinTypeExtensions
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.join

    ''' <summary>Port of the DataVec <c>TestJoin</c> JUnit class, plus tests of every join type and the null handling.</summary>
    <TestClass>
    Public Class TestJoin

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        ' Java: testJoin(@TempDir Path testDir); the directory was not used.
        <TestMethod>
        Public Sub testJoin()

            Dim firstSchema As SchemaT = (New SchemaT.Builder()).addColumnString("keyColumn").addColumnsInteger("first0", "first1").build()

            Dim secondSchema As SchemaT = (New SchemaT.Builder()).addColumnString("keyColumn").addColumnsInteger("second0").build()

            Dim first As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            first.Add(row(New Text("key0"), New IntWritable(0), New IntWritable(1)))
            first.Add(row(New Text("key1"), New IntWritable(10), New IntWritable(11)))

            Dim second As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            second.Add(row(New Text("key0"), New IntWritable(100)))
            second.Add(row(New Text("key1"), New IntWritable(110)))

            Dim join As JoinT = (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumns("keyColumn").setSchemas(firstSchema, secondSchema).build()

            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expected.Add(row(New Text("key0"), New IntWritable(0), New IntWritable(1), New IntWritable(100)))
            expected.Add(row(New Text("key1"), New IntWritable(10), New IntWritable(11), New IntWritable(110)))

            'Check schema:
            Dim joinedSchema As SchemaT = join.OutputSchema
            assertEquals(4, joinedSchema.numColumns())
            assertEquals(New List(Of String) From {"keyColumn", "first0", "first1", "second0"}, joinedSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.String, ColumnType.Integer, ColumnType.Integer, ColumnType.Integer}, joinedSchema.getColumnTypes())

            'Check joining with null values:
            expected = New List(Of IList(Of WritableT))()
            expected.Add(row(New Text("key0"), New IntWritable(0), New IntWritable(1), NullWritable.INSTANCE))
            expected.Add(row(New Text("key1"), New IntWritable(10), New IntWritable(11), NullWritable.INSTANCE))
            For i As Integer = 0 To first.Count - 1
                Dim [out] As IList(Of WritableT) = join.joinExamples(first(i), Nothing)
                assertEquals(expected(i), [out])
            Next i

            expected = New List(Of IList(Of WritableT))()
            expected.Add(row(New Text("key0"), NullWritable.INSTANCE, NullWritable.INSTANCE, New IntWritable(100)))
            expected.Add(row(New Text("key1"), NullWritable.INSTANCE, NullWritable.INSTANCE, New IntWritable(110)))
            For i As Integer = 0 To first.Count - 1
                Dim [out] As IList(Of WritableT) = join.joinExamples(Nothing, second(i))
                assertEquals(expected(i), [out])
            Next i
        End Sub

        <TestMethod>
        Public Sub testJoinValidation()
            assertThrows(Of ArgumentException)(
                Sub()
                    Dim firstSchema As SchemaT = (New SchemaT.Builder()).addColumnString("keyColumn1").addColumnsInteger("first0", "first1").build()
                    Dim secondSchema As SchemaT = (New SchemaT.Builder()).addColumnString("keyColumn2").addColumnsInteger("second0").build()
                    Call (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumns("keyColumn1", "thisDoesntExist").setSchemas(firstSchema, secondSchema).build()
                End Sub)
        End Sub

        <TestMethod>
        Public Sub testJoinValidation2()
            assertThrows(Of ArgumentException)(
                Sub()
                    Dim firstSchema As SchemaT = (New SchemaT.Builder()).addColumnString("keyColumn1").addColumnsInteger("first0", "first1").build()
                    Dim secondSchema As SchemaT = (New SchemaT.Builder()).addColumnString("keyColumn2").addColumnsInteger("second0").build()
                    Call (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumns("keyColumn1").setSchemas(firstSchema, secondSchema).build()
                End Sub)
        End Sub

        ' ---- additional tests ----

        Private Shared Function leftSchema() As SchemaT
            Return (New SchemaT.Builder()).addColumnInteger("id").addColumnString("name").addColumnString("region").build()
        End Function

        Private Shared Function rightSchema() As SchemaT
            Return (New SchemaT.Builder()).addColumnDouble("amount").addColumnInteger("customerId").addColumnString("area").build()
        End Function

        <TestMethod>
        Public Sub testEveryJoinTypeSchemaAndExamples()
            For Each type As JoinT.JoinType In JoinTypeExtensions.JoinTypeValues()
                Dim join As JoinT = (New JoinT.Builder(type)).setJoinColumnsLeft("id").setJoinColumnsRight("customerId").setSchemas(leftSchema(), rightSchema()).build()
                assertEquals(type, join.getJoinType())

                'Left schema plus the non-key right columns
                Dim outSchema As SchemaT = join.OutputSchema
                assertEquals(New List(Of String) From {"id", "name", "region", "amount", "area"}, outSchema.getColumnNames())
                assertEquals(New List(Of ColumnType) From {ColumnType.Integer, ColumnType.String, ColumnType.String, ColumnType.Double, ColumnType.String}, outSchema.getColumnTypes())
                assertEquals(outSchema, join.getOutputSchema())

                Dim l As IList(Of WritableT) = row(New IntWritable(7), New Text("ann"), New Text("north"))
                Dim r As IList(Of WritableT) = row(New Text("12.5"), New IntWritable(7), New Text("n1"))

                'Both sides present
                assertEquals(row(New IntWritable(7), New Text("ann"), New Text("north"), New Text("12.5"), New Text("n1")), join.joinExamples(l, r))
                'Right missing (left outer / full outer)
                assertEquals(row(New IntWritable(7), New Text("ann"), New Text("north"), NullWritable.INSTANCE, NullWritable.INSTANCE), join.joinExamples(l, Nothing))
                'Left missing (right outer / full outer): the key comes from the right example
                assertEquals(row(New IntWritable(7), NullWritable.INSTANCE, NullWritable.INSTANCE, New Text("12.5"), New Text("n1")), join.joinExamples(Nothing, r))
                'Both missing
                assertThrows(Of ArgumentException)(Sub() join.joinExamples(Nothing, Nothing))
            Next type
        End Sub

        <TestMethod>
        Public Sub testCompoundKeyWithLeftExampleMissing()
            Dim left As SchemaT = (New SchemaT.Builder()).addColumnString("a").addColumnInteger("v").addColumnString("b").build()
            Dim right As SchemaT = (New SchemaT.Builder()).addColumnString("rb").addColumnString("ra").addColumnInteger("w").build()
            Dim join As JoinT = (New JoinT.Builder(JoinT.JoinType.FullOuter)).setJoinColumnsLeft("a", "b").setJoinColumnsRight("ra", "rb").setSchemas(left, right).build()

            assertEquals(New List(Of String) From {"a", "v", "b", "w"}, join.OutputSchema.getColumnNames())

            'Left key column a takes the value of ra, b the value of rb
            Dim r As IList(Of WritableT) = row(New Text("B"), New Text("A"), New IntWritable(5))
            assertEquals(row(New Text("A"), NullWritable.INSTANCE, New Text("B"), New IntWritable(5)), join.joinExamples(Nothing, r))

            Dim l As IList(Of WritableT) = row(New Text("A"), New IntWritable(1), New Text("B"))
            assertEquals(row(New Text("A"), New IntWritable(1), New Text("B"), NullWritable.INSTANCE), join.joinExamples(l, Nothing))
            assertEquals(row(New Text("A"), New IntWritable(1), New Text("B"), New IntWritable(5)), join.joinExamples(l, r))
        End Sub

        <TestMethod>
        Public Sub testBuilderValidation()
            'Schemas missing
            assertThrows(Of InvalidOperationException)(Sub() Call (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumns("id").build())
            'Join columns missing
            assertThrows(Of ArgumentException)(Sub() Call (New JoinT.Builder(JoinT.JoinType.Inner)).setSchemas(leftSchema(), rightSchema()).build())
            assertThrows(Of ArgumentException)(Sub() Call (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumnsLeft("id").setSchemas(leftSchema(), rightSchema()).build())
            assertThrows(Of ArgumentException)(Sub() Call (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumnsLeft().setJoinColumnsRight("customerId").setSchemas(leftSchema(), rightSchema()).build())
            'Right join column not in the right schema
            assertThrows(Of ArgumentException)(Sub() Call (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumnsLeft("id").setJoinColumnsRight("id").setSchemas(leftSchema(), rightSchema()).build())
        End Sub

#Disable Warning BC40000 ' the deprecated setKeyColumns* methods are tested on purpose
        <TestMethod>
        Public Sub testDeprecatedKeyColumnSetters()
            Dim a As JoinT = (New JoinT.Builder(JoinT.JoinType.LeftOuter)).setKeyColumns("keyColumn").setSchemas(leftSchemaWithKey(), rightSchemaWithKey()).build()
            Dim b As JoinT = (New JoinT.Builder(JoinT.JoinType.LeftOuter)).setKeyColumnsLeft("keyColumn").setKeyColumnsRight("keyColumn").setSchemas(leftSchemaWithKey(), rightSchemaWithKey()).build()
            Dim c As JoinT = (New JoinT.Builder(JoinT.JoinType.LeftOuter)).setJoinColumns("keyColumn").setSchemas(leftSchemaWithKey(), rightSchemaWithKey()).build()
            assertEquals(c, a)
            assertEquals(c, b)
            assertEquals(c.GetHashCode(), a.GetHashCode())
        End Sub
#Enable Warning BC40000

        Private Shared Function leftSchemaWithKey() As SchemaT
            Return (New SchemaT.Builder()).addColumnString("keyColumn").addColumnInteger("x").build()
        End Function

        Private Shared Function rightSchemaWithKey() As SchemaT
            Return (New SchemaT.Builder()).addColumnString("keyColumn").addColumnInteger("y").build()
        End Function

        <TestMethod>
        Public Sub testEqualsAccessorsAndToString()
            Dim a As JoinT = (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumns("keyColumn").setSchemas(leftSchemaWithKey(), rightSchemaWithKey()).build()
            Dim b As JoinT = (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumns("keyColumn").setSchemas(leftSchemaWithKey(), rightSchemaWithKey()).build()
            Dim c As JoinT = (New JoinT.Builder(JoinT.JoinType.RightOuter)).setJoinColumns("keyColumn").setSchemas(leftSchemaWithKey(), rightSchemaWithKey()).build()
            assertEquals(a, b)
            assertFalse(a.Equals(c))
            assertEquals(leftSchemaWithKey(), a.getLeftSchema())
            assertEquals(rightSchemaWithKey(), a.getRightSchema())
            assertArrayEquals(New String() {"keyColumn"}, a.getJoinColumnsLeft())
            assertArrayEquals(New String() {"keyColumn"}, a.getJoinColumnsRight())
            assertTrue(a.ToString().StartsWith("Join(joinType=Inner, leftSchema=", StringComparison.Ordinal))
            assertTrue(a.ToString().EndsWith("joinColumnsLeft=[keyColumn], joinColumnsRight=[keyColumn])", StringComparison.Ordinal))

            c.setJoinType(JoinT.JoinType.Inner)
            assertEquals(a, c)
        End Sub

        <TestMethod>
        Public Sub testJoinTypeValueOf()
            assertEquals(JoinT.JoinType.Inner, JoinTypeExtensions.JoinTypeValueOf("Inner"))
            assertEquals(JoinT.JoinType.FullOuter, JoinTypeExtensions.JoinTypeValueOf("FullOuter"))
            assertEquals(4, JoinTypeExtensions.JoinTypeValues().Length)
            assertThrows(Of ArgumentException)(Sub() JoinTypeExtensions.JoinTypeValueOf("inner"))
            assertThrows(Of ArgumentException)(Sub() JoinTypeExtensions.JoinTypeValueOf("1"))
            assertThrows(Of ArgumentException)(Sub() JoinTypeExtensions.JoinTypeValueOf(""))
            assertThrows(Of ArgumentException)(Sub() JoinTypeExtensions.JoinTypeValueOf(Nothing))
        End Sub

    End Class

End Namespace
