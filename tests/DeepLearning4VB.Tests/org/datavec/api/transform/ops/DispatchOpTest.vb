Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.ops

    ''' <summary>Port of the DataVec <c>DispatchOpTest</c> JUnit class.</summary>
    <TestClass>
    Public Class DispatchOpTest

        Private intList As IList(Of Integer) = New List(Of Integer) From {1, 2, 3, 4, 5, 6, 7, 8, 9}

        Private stringList As IList(Of String) = New List(Of String) From {"arakoa", "abracadabra", "blast", "acceptance"}

        <TestMethod>
        Public Sub testDispatchSimple()
            Dim af As New AggregatorImpls.AggregableFirst(Of Integer)()
            Dim [as] As New AggregatorImpls.AggregableSum(Of Integer)()
            Dim multiaf As New AggregableMultiOp(Of Integer)(JavaCollections.singletonList(Of IAggregableReduceOp(Of Integer, WritableT))(af))
            Dim multias As New AggregableMultiOp(Of Integer)(JavaCollections.singletonList(Of IAggregableReduceOp(Of Integer, WritableT))([as]))

            Dim parallel As New DispatchOp(Of Integer, WritableT)(New List(Of IAggregableReduceOp(Of Integer, IList(Of WritableT))) From {multiaf, multias})

            assertTrue(multiaf.getOperations().Count = 1)
            assertTrue(multias.getOperations().Count = 1)
            assertTrue(parallel.getOperations().Count = 2)
            For i As Integer = 0 To intList.Count - 1
                parallel.accept(New List(Of Integer) From {intList(i), intList(i)})
            Next i

            Dim res As IList(Of WritableT) = parallel.get()
            assertTrue(res(1).toDouble() = 45.0)
            assertTrue(res(0).toInt() = 1)
        End Sub

        <TestMethod>
        Public Sub testDispatchFlatMap()
            Dim af As New AggregatorImpls.AggregableFirst(Of Integer)()
            Dim [as] As New AggregatorImpls.AggregableSum(Of Integer)()
            Dim multi As New AggregableMultiOp(Of Integer)(New List(Of IAggregableReduceOp(Of Integer, WritableT)) From {af, [as]})

            Dim al As New AggregatorImpls.AggregableLast(Of Integer)()
            Dim amax As New AggregatorImpls.AggregableMax(Of Integer)()
            Dim otherMulti As New AggregableMultiOp(Of Integer)(New List(Of IAggregableReduceOp(Of Integer, WritableT)) From {al, amax})

            Dim parallel As New DispatchOp(Of Integer, WritableT)(New List(Of IAggregableReduceOp(Of Integer, IList(Of WritableT))) From {multi, otherMulti})

            assertTrue(multi.getOperations().Count = 2)
            assertTrue(otherMulti.getOperations().Count = 2)
            assertTrue(parallel.getOperations().Count = 2)
            For i As Integer = 0 To intList.Count - 1
                parallel.accept(New List(Of Integer) From {intList(i), intList(i)})
            Next i

            Dim res As IList(Of WritableT) = parallel.get()
            assertTrue(res(1).toDouble() = 45.0)
            assertTrue(res(0).toInt() = 1)
            assertTrue(res(3).toDouble() = 9)
            assertTrue(res(2).toInt() = 9)
        End Sub

    End Class

End Namespace
