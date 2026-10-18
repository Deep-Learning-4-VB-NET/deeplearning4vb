Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports JoinT = DeepLearning4VB.org.datavec.api.transform.join.Join
Imports LocalTransformExecutorT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformExecutor
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform.join

    ''' <summary>Port of the DataVec (datavec-local) <c>TestJoin</c> JUnit class.</summary>
    <TestClass>
    Public Class TestJoin

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub testJoinOneToMany_ManyToOne()

            Dim customerInfoSchema As SchemaT = (New SchemaT.Builder()).addColumnLong("customerID").addColumnString("customerName").build()

            Dim purchasesSchema As SchemaT = (New SchemaT.Builder()).addColumnLong("purchaseID").addColumnLong("customerID").addColumnDouble("amount").build()

            Dim infoList As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            infoList.Add(row(New LongWritable(12345), New Text("Customer12345")))
            infoList.Add(row(New LongWritable(98765), New Text("Customer98765")))
            infoList.Add(row(New LongWritable(50000), New Text("Customer50000")))

            Dim purchaseList As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            purchaseList.Add(row(New LongWritable(1000000), New LongWritable(12345), New DoubleWritable(10.0)))
            purchaseList.Add(row(New LongWritable(1000001), New LongWritable(12345), New DoubleWritable(20.0)))
            purchaseList.Add(row(New LongWritable(1000002), New LongWritable(98765), New DoubleWritable(30.0)))

            Dim join As JoinT = (New JoinT.Builder(JoinT.JoinType.RightOuter)).setJoinColumns("customerID").setSchemas(customerInfoSchema, purchasesSchema).build()

            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expected.Add(row(New LongWritable(12345), New Text("Customer12345"), New LongWritable(1000000), New DoubleWritable(10.0)))
            expected.Add(row(New LongWritable(12345), New Text("Customer12345"), New LongWritable(1000001), New DoubleWritable(20.0)))
            expected.Add(row(New LongWritable(98765), New Text("Customer98765"), New LongWritable(1000002), New DoubleWritable(30.0)))

            Dim info As IList(Of IList(Of WritableT)) = infoList
            Dim purchases As IList(Of IList(Of WritableT)) = purchaseList

            Dim joined As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.executeJoin(join, info, purchases)
            'Sort by order ID (column 3, index 2)
            Dim joinedList As IList(Of IList(Of WritableT)) = joined.OrderBy(Function(o) o(2).toLong()).ToList()

            assertEquals(expected, joinedList)

            assertEquals(3, joinedList.Count)

            Dim expectedColNames As IList(Of String) = New List(Of String) From {"customerID", "customerName", "purchaseID", "amount"}
            assertEquals(expectedColNames, join.OutputSchema.getColumnNames())

            Dim expectedColTypes As IList(Of ColumnType) = New List(Of ColumnType) From {ColumnType.Long, ColumnType.String, ColumnType.Long, ColumnType.Double}
            assertEquals(expectedColTypes, join.OutputSchema.getColumnTypes())


            'Test Many to one: same thing, but swap the order...
            Dim join2 As JoinT = (New JoinT.Builder(JoinT.JoinType.LeftOuter)).setJoinColumns("customerID").setSchemas(purchasesSchema, customerInfoSchema).build()

            Dim expectedManyToOne As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expectedManyToOne.Add(row(New LongWritable(1000000), New LongWritable(12345), New DoubleWritable(10.0), New Text("Customer12345")))
            expectedManyToOne.Add(row(New LongWritable(1000001), New LongWritable(12345), New DoubleWritable(20.0), New Text("Customer12345")))
            expectedManyToOne.Add(row(New LongWritable(1000002), New LongWritable(98765), New DoubleWritable(30.0), New Text("Customer98765")))

            Dim joined2 As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.executeJoin(join2, purchases, info)
            'Sort by order ID (column 0)
            Dim joinedList2 As IList(Of IList(Of WritableT)) = joined2.OrderBy(Function(o) o(0).toLong()).ToList()
            assertEquals(3, joinedList2.Count)

            assertEquals(expectedManyToOne, joinedList2)

            Dim expectedColNames2 As IList(Of String) = New List(Of String) From {"purchaseID", "customerID", "amount", "customerName"}
            assertEquals(expectedColNames2, join2.OutputSchema.getColumnNames())

            Dim expectedColTypes2 As IList(Of ColumnType) = New List(Of ColumnType) From {ColumnType.Long, ColumnType.Long, ColumnType.Double, ColumnType.String}
            assertEquals(expectedColTypes2, join2.OutputSchema.getColumnTypes())
        End Sub

        <TestMethod>
        Public Sub testJoinManyToMany()
            Dim schema1 As SchemaT = (New SchemaT.Builder()).addColumnLong("id").addColumnCategorical("category", New List(Of String) From {"cat0", "cat1", "cat2"}).build()

            Dim schema2 As SchemaT = (New SchemaT.Builder()).addColumnLong("otherId").addColumnCategorical("otherCategory", New List(Of String) From {"cat0", "cat1", "cat2"}).build()

            Dim first As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            first.Add(row(New LongWritable(0), New Text("cat0")))
            first.Add(row(New LongWritable(1), New Text("cat0")))
            first.Add(row(New LongWritable(2), New Text("cat1")))

            Dim second As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            second.Add(row(New LongWritable(100), New Text("cat0")))
            second.Add(row(New LongWritable(101), New Text("cat0")))
            second.Add(row(New LongWritable(102), New Text("cat2")))

            Dim expOuterJoin As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expOuterJoin.Add(row(New LongWritable(0), New Text("cat0"), New LongWritable(100)))
            expOuterJoin.Add(row(New LongWritable(0), New Text("cat0"), New LongWritable(101)))
            expOuterJoin.Add(row(New LongWritable(1), New Text("cat0"), New LongWritable(100)))
            expOuterJoin.Add(row(New LongWritable(1), New Text("cat0"), New LongWritable(101)))
            expOuterJoin.Add(row(New LongWritable(2), New Text("cat1"), New NullWritable()))
            expOuterJoin.Add(row(New NullWritable(), New Text("cat2"), New LongWritable(102)))

            Dim expLeftJoin As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expLeftJoin.Add(row(New LongWritable(0), New Text("cat0"), New LongWritable(100)))
            expLeftJoin.Add(row(New LongWritable(0), New Text("cat0"), New LongWritable(101)))
            expLeftJoin.Add(row(New LongWritable(1), New Text("cat0"), New LongWritable(100)))
            expLeftJoin.Add(row(New LongWritable(1), New Text("cat0"), New LongWritable(101)))
            expLeftJoin.Add(row(New LongWritable(2), New Text("cat1"), New NullWritable()))

            Dim expRightJoin As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expRightJoin.Add(row(New LongWritable(0), New Text("cat0"), New LongWritable(100)))
            expRightJoin.Add(row(New LongWritable(0), New Text("cat0"), New LongWritable(101)))
            expRightJoin.Add(row(New LongWritable(1), New Text("cat0"), New LongWritable(100)))
            expRightJoin.Add(row(New LongWritable(1), New Text("cat0"), New LongWritable(101)))
            expRightJoin.Add(row(New NullWritable(), New Text("cat2"), New LongWritable(102)))

            Dim expInnerJoin As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expInnerJoin.Add(row(New LongWritable(0), New Text("cat0"), New LongWritable(100)))
            expInnerJoin.Add(row(New LongWritable(0), New Text("cat0"), New LongWritable(101)))
            expInnerJoin.Add(row(New LongWritable(1), New Text("cat0"), New LongWritable(100)))
            expInnerJoin.Add(row(New LongWritable(1), New Text("cat0"), New LongWritable(101)))

            Dim firstRDD As IList(Of IList(Of WritableT)) = first
            Dim secondRDD As IList(Of IList(Of WritableT)) = second

            Dim count As Integer = 0
            For Each jt As JoinT.JoinType In [Enum].GetValues(GetType(JoinT.JoinType))
                Dim join As JoinT = (New JoinT.Builder(jt)).setJoinColumnsLeft("category").setJoinColumnsRight("otherCategory").setSchemas(schema1, schema2).build()
                Dim [out] As List(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(LocalTransformExecutorT.executeJoin(join, firstRDD, secondRDD))

                'Sort output by column 0, then column 1, then column 2 for comparison to expected...
                [out] = [out].OrderBy(Function(o) o, New JoinOutputComparer()).ToList()

                Select Case jt
                    Case JoinT.JoinType.Inner
                        assertEquals(expInnerJoin, [out])
                    Case JoinT.JoinType.LeftOuter
                        assertEquals(expLeftJoin, [out])
                    Case JoinT.JoinType.RightOuter
                        assertEquals(expRightJoin, [out])
                    Case JoinT.JoinType.FullOuter
                        assertEquals(expOuterJoin, [out])
                End Select
                count += 1
            Next jt

            assertEquals(4, count)
        End Sub

        ''' <summary>The comparator of the Java test: column 0 (nulls last), then column 1, then column 2 (nulls last).</summary>
        Private NotInheritable Class JoinOutputComparer
            Implements IComparer(Of IList(Of WritableT))

            Public Function Compare(ByVal o1 As IList(Of WritableT), ByVal o2 As IList(Of WritableT)) As Integer Implements IComparer(Of IList(Of WritableT)).Compare
                Dim w1 As WritableT = o1(0)
                Dim w2 As WritableT = o2(0)
                If TypeOf w1 Is NullWritable Then
                    Return 1
                ElseIf TypeOf w2 Is NullWritable Then
                    Return -1
                End If
                Dim c As Integer = w1.toLong().CompareTo(w2.toLong())
                If c <> 0 Then
                    Return c
                End If
                c = String.CompareOrdinal(o1(1).ToString(), o2(1).ToString())
                If c <> 0 Then
                    Return c
                End If
                w1 = o1(2)
                w2 = o2(2)
                If TypeOf w1 Is NullWritable Then
                    Return 1
                ElseIf TypeOf w2 Is NullWritable Then
                    Return -1
                End If
                Return w1.toLong().CompareTo(w2.toLong())
            End Function

        End Class

    End Class

End Namespace
