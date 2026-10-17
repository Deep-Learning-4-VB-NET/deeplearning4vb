Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports GeographicMidpointReduction = DeepLearning4VB.org.datavec.api.transform.reduce.impl.GeographicMidpointReduction
Imports Reducer = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports ReduceOp = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.reduce

    ''' <summary>
    ''' Port of the DataVec <c>TestReductions</c> JUnit class, plus tests of the other members of
    ''' <see cref="GeographicMidpointReduction"/>.
    ''' </summary>
    <TestClass>
    Public Class TestReductions

        Private Shared Function parse(ByVal s As String) As Double
            Return Double.Parse(s, CultureInfo.InvariantCulture)
        End Function

        <TestMethod>
        Public Sub testGeographicMidPointReduction()

            'http://www.geomidpoint.com/example.html
            'That particular example is weighted - have 3x weight for t1, 2x weight for t2, 1x weight for t1
            Dim t1 As New Text("40.7143528,-74.0059731")
            Dim t2 As New Text("41.8781136,-87.6297982")
            Dim t3 As New Text("33.7489954,-84.3879824")

            Dim list As IList(Of WritableT) = New List(Of WritableT) From {t1, t1, t1, t2, t2, t3}

            Dim reduction As New GeographicMidpointReduction(",")

            Dim reduceOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = reduction.reduceOp()
            For Each w As WritableT In list
                reduceOp.accept(w)
            Next w

            Dim listOut As IList(Of WritableT) = reduceOp.get()
            assertEquals(1, listOut.Count)
            Dim wOut As WritableT = listOut(0)

            Dim split() As String = wOut.ToString().Split(","c)
            Dim lat As Double = parse(split(0))
            Dim lng As Double = parse(split(1))

            Dim expLat As Double = 40.11568861
            Dim expLong As Double = -80.2996028

            assertEquals(expLat, lat, 0.000001)
            assertEquals(expLong, lng, 0.000001)

            'Test multiple reductions
            list = New List(Of WritableT) From {t1, t1, t2}
            Dim list2 As IList(Of WritableT) = New List(Of WritableT) From {t1, t2, t3}
            reduceOp = reduction.reduceOp()
            For Each w As WritableT In list
                reduceOp.accept(w)
            Next w

            Dim reduceOp2 As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = reduction.reduceOp()
            For Each w As WritableT In list2
                reduceOp2.accept(w)
            Next w

            reduceOp.combine(reduceOp2)

            listOut = reduceOp.get()
            assertEquals(1, listOut.Count)
            wOut = listOut(0)

            split = wOut.ToString().Split(","c)
            lat = parse(split(0))
            lng = parse(split(1))

            assertEquals(expLat, lat, 0.000001)
            assertEquals(expLong, lng, 0.000001)
        End Sub

        <TestMethod>
        Public Sub testGeographicMidPointOtherDelimiterAndSinglePoint()
            Dim reduction As New GeographicMidpointReduction("\|")
            Dim op As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = reduction.reduceOp()
            op.accept(New Text("10.5|-20.25"))
            ' the delimiter is a regular expression for parsing, but the output uses the delimiter string as given (Java)
            Dim outStr As String = op.get()(0).ToString()
            Dim idx As Integer = outStr.IndexOf("\|", StringComparison.Ordinal)
            assertTrue(idx > 0)
            assertEquals(10.5, parse(outStr.Substring(0, idx)), 0.000000001)
            assertEquals(-20.25, parse(outStr.Substring(idx + 2)), 0.000000001)
        End Sub

        <TestMethod>
        Public Sub testGeographicMidPointErrors()
            Dim reduction As New GeographicMidpointReduction(",")

            'No data points
            Dim empty As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = reduction.reduceOp()
            assertThrows(Of InvalidOperationException)(Sub() empty.get())

            'Unparsable coordinates
            Dim op As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = reduction.reduceOp()
            assertThrows(Of InvalidOperationException)(Sub() op.accept(New Text("1.0,2.0,3.0")))
            assertThrows(Of InvalidOperationException)(Sub() op.accept(New Text("1.0")))
            assertThrows(Of Global.DeepLearning4VB.JavaCompat.NumberFormatException)(Sub() op.accept(New Text("north,south")))

            'Latitude out of range
            assertThrows(Of InvalidOperationException)(Sub() op.accept(New Text("91.0,0.0")))

            'Antipodal points: the midpoint is the centre of the earth
            Dim antipodal As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = reduction.reduceOp()
            antipodal.accept(New Text("0.0,0.0"))
            antipodal.accept(New Text("0.0,180.0"))
            assertThrows(Of InvalidOperationException)(Sub() antipodal.get())

            'Combining with another kind of accumulator
            Dim other As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = New AggregableMultiOp(Of WritableT)(New List(Of IAggregableReduceOp(Of WritableT, WritableT))())
            Dim op2 As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = reduction.reduceOp()
            assertThrows(Of InvalidOperationException)(Sub() op2.combine(other))
        End Sub

        <TestMethod>
        Public Sub testGeographicMidPointSchemaAndEquality()
            Dim reduction As New GeographicMidpointReduction(",")
            assertEquals(New List(Of String) From {"midpoint(coords)"}, reduction.getColumnsOutputName("coords"))
            Dim named As New GeographicMidpointReduction(",", "centre")
            assertEquals(New List(Of String) From {"centre"}, named.getColumnsOutputName("coords"))

            Dim meta As IList(Of ColumnMetaData) = reduction.getColumnOutputMetaData(New List(Of String) From {"m"}, New StringMetaData("coords"))
            assertEquals(1, meta.Count)
            assertEquals(ColumnType.String, meta(0).ColumnType)
            assertEquals("m", meta(0).Name)

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("coords").build()
            assertTrue(schema Is reduction.transform(schema))
            reduction.InputSchema = schema
            assertNull(reduction.InputSchema)
            assertNull(reduction.outputColumnName())
            assertEquals(0, reduction.outputColumnNames().Length)
            assertEquals(0, reduction.columnNames().Length)
            assertNull(reduction.columnName())

            assertEquals(New GeographicMidpointReduction(","), reduction)
            assertEquals(New GeographicMidpointReduction(",").GetHashCode(), reduction.GetHashCode())
            assertFalse(reduction.Equals(named))
            assertFalse(reduction.Equals(New GeographicMidpointReduction(";")))
        End Sub

        <TestMethod>
        Public Sub testGeographicMidPointInReducer()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnString("coords").build()
            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.TakeFirst)).keyColumns("key").customReduction("coords", New GeographicMidpointReduction(",")).build()
            reducer.InputSchema = schema

            Dim outSchema As SchemaT = reducer.transform(schema)
            assertEquals(New List(Of String) From {"key", "midpoint(coords)"}, outSchema.getColumnNames())

            Dim accu As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()
            accu.accept(New List(Of WritableT) From {New Text("k"), New Text("10.0,20.0")})
            accu.accept(New List(Of WritableT) From {New Text("k"), New Text("10.0,20.0")})
            Dim result As IList(Of WritableT) = accu.get()
            assertEquals(2, result.Count)
            assertEquals(New Text("k"), result(0))
            Dim parts() As String = result(1).ToString().Split(","c)
            assertEquals(10.0, parse(parts(0)), 0.000000001)
            assertEquals(20.0, parse(parts(1)), 0.000000001)
        End Sub

    End Class

End Namespace
