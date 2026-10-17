Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ConditionFilter = DeepLearning4VB.org.datavec.api.transform.filter.ConditionFilter
Imports ConditionOp = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports FilterInvalidValues = DeepLearning4VB.org.datavec.api.transform.filter.FilterInvalidValues
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports IntegerColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports InvalidNumColumns = DeepLearning4VB.org.datavec.api.transform.filter.InvalidNumColumns
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.filter

    ''' <summary>Port of the DataVec <c>TestFilters</c> JUnit class.</summary>
    <TestClass>
    Public Class TestFilters

        ''' <summary>Java <c>Collections.singletonList((Writable) w)</c>.</summary>
        Private Shared Function row(ByVal w As WritableT) As IList(Of WritableT)
            Return JavaCollections.singletonList(Of WritableT)(w)
        End Function

        ''' <summary>Java <c>Arrays.&lt;Writable&gt;asList(...)</c>.</summary>
        Private Shared Function row(ByVal ParamArray ws() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(ws)
        End Function

        <TestMethod>
        Public Sub testFilterNumColumns()
            Dim list As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            list.Add(row(New IntWritable(-1)))
            list.Add(row(New IntWritable(0)))
            list.Add(row(New IntWritable(2)))

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("intCol", 0, 10).addColumnDouble("doubleCol", -100.0, 100.0).build() 'Only values in the range 0 to 10 are ok; -100 to 100 only, no NaN or infinite
            Dim numColumns As FilterT = New InvalidNumColumns(schema)
            For i As Integer = 0 To list.Count - 1
                assertTrue(numColumns.removeExample(list(i)))
            Next i

            Dim correct As IList(Of WritableT) = row(New IntWritable(0), New DoubleWritable(2))
            assertFalse(numColumns.removeExample(correct))
        End Sub

        <TestMethod>
        Public Sub testFilterInvalidValues()
            Dim list As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            list.Add(row(New IntWritable(-1)))
            list.Add(row(New IntWritable(0)))
            list.Add(row(New IntWritable(2)))

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("intCol", 0, 10).addColumnDouble("doubleCol", -100.0, 100.0).build() 'Only values in the range 0 to 10 are ok; -100 to 100 only, no NaN or infinite

            Dim filter As FilterT = New FilterInvalidValues("intCol", "doubleCol")
            filter.InputSchema = schema

            'Test valid examples:
            assertFalse(filter.removeExample(row(New IntWritable(0), New DoubleWritable(0))))
            assertFalse(filter.removeExample(row(New IntWritable(10), New DoubleWritable(0))))
            assertFalse(filter.removeExample(row(New IntWritable(0), New DoubleWritable(-100))))
            assertFalse(filter.removeExample(row(New IntWritable(0), New DoubleWritable(100))))

            'Test invalid:
            assertTrue(filter.removeExample(row(New IntWritable(-1), New DoubleWritable(0))))
            assertTrue(filter.removeExample(row(New IntWritable(11), New DoubleWritable(0))))
            assertTrue(filter.removeExample(row(New IntWritable(0), New DoubleWritable(-101))))
            assertTrue(filter.removeExample(row(New IntWritable(0), New DoubleWritable(101))))
        End Sub

        <TestMethod>
        Public Sub testConditionFilter()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("column").build()

            Dim condition As ConditionT = New IntegerColumnCondition("column", ConditionOp.LessThan, 0)
            condition.InputSchema = schema

            Dim filter As FilterT = New ConditionFilter(condition)

            assertFalse(filter.removeExample(row(New IntWritable(10))))
            assertFalse(filter.removeExample(row(New IntWritable(1))))
            assertFalse(filter.removeExample(row(New IntWritable(0))))
            assertTrue(filter.removeExample(row(New IntWritable(-1))))
            assertTrue(filter.removeExample(row(New IntWritable(-10))))
        End Sub

    End Class

End Namespace
