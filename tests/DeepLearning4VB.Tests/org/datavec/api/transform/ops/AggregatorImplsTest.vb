Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.ops

Namespace org.datavec.api.transform.ops

    ''' <summary>Port of the DataVec <c>AggregatorImplsTest</c> JUnit class.</summary>
    <TestClass>
    Public Class AggregatorImplsTest

        Private intList As IList(Of Integer) = New List(Of Integer) From {1, 2, 3, 4, 5, 6, 7, 8, 9}

        Private stringList As IList(Of String) = New List(Of String) From {"arakoa", "abracadabra", "blast", "acceptance"}

        <TestMethod>
        Public Sub aggregableFirstTest()
            Dim first As New AggregatorImpls.AggregableFirst(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                first.accept(intList(i))
            Next i
            assertEquals(1, first.get().toInt())

            Dim firstS As New AggregatorImpls.AggregableFirst(Of String)()
            For i As Integer = 0 To stringList.Count - 1
                firstS.accept(stringList(i))
            Next i
            assertTrue(firstS.get().ToString().Equals("arakoa"))

            Dim reverse As New AggregatorImpls.AggregableFirst(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            first.combine(reverse)
            assertEquals(1, first.get().toInt())
        End Sub

        <TestMethod>
        Public Sub aggregableLastTest()
            Dim last As New AggregatorImpls.AggregableLast(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                last.accept(intList(i))
            Next i
            assertEquals(9, last.get().toInt())

            Dim lastS As New AggregatorImpls.AggregableLast(Of String)()
            For i As Integer = 0 To stringList.Count - 1
                lastS.accept(stringList(i))
            Next i
            assertTrue(lastS.get().ToString().Equals("acceptance"))

            Dim reverse As New AggregatorImpls.AggregableLast(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            last.combine(reverse)
            assertEquals(1, last.get().toInt())
        End Sub

        <TestMethod>
        Public Sub aggregableCountTest()
            Dim cnt As New AggregatorImpls.AggregableCount(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                cnt.accept(intList(i))
            Next i
            assertEquals(9, cnt.get().toInt())

            Dim lastS As New AggregatorImpls.AggregableCount(Of String)()
            For i As Integer = 0 To stringList.Count - 1
                lastS.accept(stringList(i))
            Next i
            assertEquals(4, lastS.get().toInt())

            Dim reverse As New AggregatorImpls.AggregableCount(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            cnt.combine(reverse)
            assertEquals(18, cnt.get().toInt())
        End Sub

        <TestMethod>
        Public Sub aggregableMaxTest()
            Dim mx As New AggregatorImpls.AggregableMax(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                mx.accept(intList(i))
            Next i
            assertEquals(9, mx.get().toInt())

            Dim reverse As New AggregatorImpls.AggregableMax(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            mx.combine(reverse)
            assertEquals(9, mx.get().toInt())
        End Sub

        <TestMethod>
        Public Sub aggregableRangeTest()
            Dim mx As New AggregatorImpls.AggregableRange(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                mx.accept(intList(i))
            Next i
            assertEquals(8, mx.get().toInt())

            Dim reverse As New AggregatorImpls.AggregableRange(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1) + 9)
            Next i
            mx.combine(reverse)
            assertEquals(17, mx.get().toInt())
        End Sub

        <TestMethod>
        Public Sub aggregableMinTest()
            Dim mn As New AggregatorImpls.AggregableMin(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                mn.accept(intList(i))
            Next i
            assertEquals(1, mn.get().toInt())

            Dim reverse As New AggregatorImpls.AggregableMin(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            mn.combine(reverse)
            assertEquals(1, mn.get().toInt())
        End Sub

        <TestMethod>
        Public Sub aggregableSumTest()
            Dim sm As New AggregatorImpls.AggregableSum(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                sm.accept(intList(i))
            Next i
            assertEquals(45, sm.get().toInt())

            Dim reverse As New AggregatorImpls.AggregableSum(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            sm.combine(reverse)
            assertEquals(90, sm.get().toInt())
        End Sub

        <TestMethod>
        Public Sub aggregableMeanTest()
            Dim mn As New AggregatorImpls.AggregableMean(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                mn.accept(intList(i))
            Next i
            assertEquals(9L, mn.getCount())
            assertEquals(5.0, mn.get().toDouble(), 0.001)

            Dim reverse As New AggregatorImpls.AggregableMean(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            mn.combine(reverse)
            assertEquals(18L, mn.getCount())
            assertEquals(5.0, mn.get().toDouble(), 0.001)
        End Sub

        <TestMethod>
        Public Sub aggregableStdDevTest()
            Dim sd As New AggregatorImpls.AggregableStdDev(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                sd.accept(intList(i))
            Next i
            assertTrue(Math.Abs(sd.get().toDouble() - 2.7386) < 0.0001)

            Dim reverse As New AggregatorImpls.AggregableStdDev(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            sd.combine(reverse)
            assertTrue(Math.Abs(sd.get().toDouble() - 1.8787) < 0.0001, "" & sd.get().toDouble())
        End Sub

        <TestMethod>
        Public Sub aggregableVariance()
            Dim sd As New AggregatorImpls.AggregableVariance(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                sd.accept(intList(i))
            Next i
            assertTrue(Math.Abs(sd.get().toDouble() - 60.0 / 8) < 0.0001)

            Dim reverse As New AggregatorImpls.AggregableVariance(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            sd.combine(reverse)
            assertTrue(Math.Abs(sd.get().toDouble() - 3.5294) < 0.0001, "" & sd.get().toDouble())
        End Sub

        <TestMethod>
        Public Sub aggregableUncorrectedStdDevTest()
            Dim sd As New AggregatorImpls.AggregableUncorrectedStdDev(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                sd.accept(intList(i))
            Next i
            assertTrue(Math.Abs(sd.get().toDouble() - 2.582) < 0.0001)

            Dim reverse As New AggregatorImpls.AggregableUncorrectedStdDev(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            sd.combine(reverse)
            assertTrue(Math.Abs(sd.get().toDouble() - 1.8257) < 0.0001, "" & sd.get().toDouble())
        End Sub

        <TestMethod>
        Public Sub aggregablePopulationVariance()
            Dim sd As New AggregatorImpls.AggregablePopulationVariance(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                sd.accept(intList(i))
            Next i
            assertTrue(Math.Abs(sd.get().toDouble() - 60.0 / 9) < 0.0001)

            Dim reverse As New AggregatorImpls.AggregablePopulationVariance(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            sd.combine(reverse)
            assertTrue(Math.Abs(sd.get().toDouble() - 30.0 / 9) < 0.0001, "" & sd.get().toDouble())
        End Sub

        <TestMethod>
        Public Sub aggregableCountUniqueTest()
            ' at this low range, it's linear counting
            Dim cu As New AggregatorImpls.AggregableCountUnique(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                cu.accept(intList(i))
            Next i
            assertEquals(9, cu.get().toInt())
            cu.accept(1)
            assertEquals(9, cu.get().toInt())

            Dim reverse As New AggregatorImpls.AggregableCountUnique(Of Integer)()
            For i As Integer = 0 To intList.Count - 1
                reverse.accept(intList(intList.Count - i - 1))
            Next i
            cu.combine(reverse)
            assertEquals(9, cu.get().toInt())
        End Sub

        <TestMethod>
        Public Sub incompatibleAggregatorTest()
            assertThrows(Of NotSupportedException)(
                Sub()
                    Dim sm As New AggregatorImpls.AggregableSum(Of Integer)()
                    For i As Integer = 0 To intList.Count - 1
                        sm.accept(intList(i))
                    Next i
                    assertEquals(45, sm.get().toInt())

                    Dim reverse As New AggregatorImpls.AggregableMean(Of Integer)()
                    For i As Integer = 0 To intList.Count - 1
                        reverse.accept(intList(intList.Count - i - 1))
                    Next i
                    sm.combine(reverse)
                    assertEquals(45, sm.get().toInt())
                End Sub)
        End Sub

    End Class

End Namespace
