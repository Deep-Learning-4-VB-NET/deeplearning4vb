Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.ops

    ''' <summary>Port of the DataVec <c>AggregableMultiOpTest</c> JUnit class.</summary>
    <TestClass>
    Public Class AggregableMultiOpTest

        Private intList As IList(Of Integer) = New List(Of Integer) From {1, 2, 3, 4, 5, 6, 7, 8, 9}

        <TestMethod>
        Public Sub testMulti()
            Dim af As New AggregatorImpls.AggregableFirst(Of Integer)()
            Dim [as] As New AggregatorImpls.AggregableSum(Of Integer)()
            Dim multi As New AggregableMultiOp(Of Integer)(New List(Of IAggregableReduceOp(Of Integer, WritableT)) From {af, [as]})

            assertTrue(multi.getOperations().Count = 2)
            For i As Integer = 0 To intList.Count - 1
                multi.accept(intList(i))
            Next i
            ' mutablility
            assertTrue([as].get().toDouble() = 45.0)
            assertTrue(af.get().toInt() = 1)

            Dim res As IList(Of WritableT) = multi.get()
            assertTrue(res(1).toDouble() = 45.0)
            assertTrue(res(0).toInt() = 1)

            Dim rf As New AggregatorImpls.AggregableFirst(Of Integer)()
            Dim rs As New AggregatorImpls.AggregableSum(Of Integer)()
            Dim reverse As New AggregableMultiOp(Of Integer)(New List(Of IAggregableReduceOp(Of Integer, WritableT)) From {rf, rs})

            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i

            Dim revRes As IList(Of WritableT) = reverse.get()
            assertTrue(revRes(1).toDouble() = 45.0)
            assertTrue(revRes(0).toInt() = 9)

            multi.combine(reverse)
            Dim combinedRes As IList(Of WritableT) = multi.get()
            assertTrue(combinedRes(1).toDouble() = 90.0)
            assertTrue(combinedRes(0).toInt() = 1)
        End Sub

    End Class

End Namespace
