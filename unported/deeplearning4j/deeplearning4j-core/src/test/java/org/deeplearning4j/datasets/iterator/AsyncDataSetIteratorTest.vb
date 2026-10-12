Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports InterleavedDataSetCallback = org.deeplearning4j.datasets.iterator.callbacks.InterleavedDataSetCallback
Imports VariableTimeseriesGenerator = org.deeplearning4j.datasets.iterator.tools.VariableTimeseriesGenerator
Imports TestDataSetConsumer = org.deeplearning4j.nn.util.TestDataSetConsumer
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
import static org.junit.jupiter.api.Assertions.assertThrows

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 
Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Async Data Set Iterator Test") @NativeTag class AsyncDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class AsyncDataSetIteratorTest
		Inherits BaseDL4JTest

		Private backIterator As ExistingDataSetIterator

		Private Const TEST_SIZE As Integer = 100

		Private Const ITERATIONS As Integer = 10

		' time spent in consumer thread, milliseconds
		Private Const EXECUTION_TIME As Long = 5

		Private Const EXECUTION_SMALL As Long = 1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub setUp()
			Dim iterable As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To TEST_SIZE - 1
				iterable.Add(New DataSet(Nd4j.create(New Single(99){}), Nd4j.create(New Single(9){})))
			Next i
			backIterator = New ExistingDataSetIterator(iterable)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Has Next 1") void hasNext1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub hasNext1()
			For iter As Integer = 0 To ITERATIONS - 1
				For prefetchSize As Integer = 2 To 8
					Dim iterator As New AsyncDataSetIterator(backIterator, prefetchSize)
					Dim cnt As Integer = 0
					Do While iterator.MoveNext()
						Dim ds As DataSet = iterator.Current
						assertNotEquals(Nothing, ds)
						cnt += 1
					Loop
					assertEquals(TEST_SIZE, cnt,"Failed on iteration: " & iter & ", prefetchSize: " & prefetchSize)
					iterator.shutdown()
				Next prefetchSize
			Next iter
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Has Next With Reset And Load") void hasNextWithResetAndLoad() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub hasNextWithResetAndLoad()
			Dim prefetchSizes() As Integer
			If IntegrationTests Then
				prefetchSizes = New Integer() { 2, 3, 4, 5, 6, 7, 8 }
			Else
				prefetchSizes = New Integer() { 2, 3, 8 }
			End If
			For iter As Integer = 0 To ITERATIONS - 1
				For Each prefetchSize As Integer In prefetchSizes
					Dim iterator As New AsyncDataSetIterator(backIterator, prefetchSize)
					Dim consumer As New TestDataSetConsumer(EXECUTION_SMALL)
					Dim cnt As Integer = 0
					Do While iterator.MoveNext()
						Dim ds As DataSet = iterator.Current
						consumer.consumeOnce(ds, False)
						cnt += 1
						If cnt = TEST_SIZE \ 2 Then
							iterator.reset()
						End If
					Loop
					assertEquals(TEST_SIZE + (TEST_SIZE \ 2), cnt)
					iterator.shutdown()
				Next prefetchSize
			Next iter
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test With Load") void testWithLoad()
		Friend Overridable Sub testWithLoad()
			For iter As Integer = 0 To ITERATIONS - 1
				Dim iterator As New AsyncDataSetIterator(backIterator, 8)
				Dim consumer As New TestDataSetConsumer(iterator, EXECUTION_TIME)
				consumer.consumeWhileHasNext(True)
				assertEquals(TEST_SIZE, consumer.Count)
				iterator.shutdown()
			Next iter
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test With Exception") void testWithException()
		Friend Overridable Sub testWithException()
			assertThrows(GetType(System.IndexOutOfRangeException), Sub()
			Dim crashingIterator As New ExistingDataSetIterator(New IterableWithException(Me, 100))
			Dim iterator As New AsyncDataSetIterator(crashingIterator, 8)
			Dim consumer As New TestDataSetConsumer(iterator, EXECUTION_SMALL)
			consumer.consumeWhileHasNext(True)
			iterator.shutdown()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Iterable With Exception") private class IterableWithException implements Iterable<org.nd4j.linalg.dataset.DataSet>
		Private Class IterableWithException
			Implements IEnumerable(Of DataSet)

			Private ReadOnly outerInstance As AsyncDataSetIteratorTest


			Friend ReadOnly counter As New AtomicLong(0)

			Friend ReadOnly crashIteration As Integer

			Public Sub New(ByVal outerInstance As AsyncDataSetIteratorTest, ByVal iteration As Integer)
				Me.outerInstance = outerInstance
				crashIteration = iteration
			End Sub

			Public Overridable Function GetEnumerator() As IEnumerator(Of DataSet) Implements IEnumerator(Of DataSet).GetEnumerator
				counter.set(0)
				Return New IteratorAnonymousInnerClass(Me)
			End Function

			Private Class IteratorAnonymousInnerClass
				Implements IEnumerator(Of DataSet)

				Private ReadOnly outerInstance As IterableWithException

				Public Sub New(ByVal outerInstance As IterableWithException)
					Me.outerInstance = outerInstance
				End Sub


				Public Function hasNext() As Boolean
					Return True
				End Function

				Public Function [next]() As DataSet
					If outerInstance.counter.incrementAndGet() >= outerInstance.crashIteration Then
						Throw New System.IndexOutOfRangeException("Thrown as expected")
					End If
					Return New DataSet(Nd4j.create(10), Nd4j.create(10))
				End Function

				Public Sub remove()
				End Sub
			End Class
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Variable Time Series 1") void testVariableTimeSeries1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testVariableTimeSeries1()
			Dim numBatches As Integer = If(IntegrationTests, 1000, 100)
			Dim batchSize As Integer = If(IntegrationTests, 32, 8)
			Dim timeStepsMin As Integer = 10
			Dim timeStepsMax As Integer = If(IntegrationTests, 500, 100)
			Dim valuesPerTimestep As Integer = If(IntegrationTests, 128, 16)
			Dim adsi As New AsyncDataSetIterator(New VariableTimeseriesGenerator(1192, numBatches, batchSize, valuesPerTimestep, timeStepsMin, timeStepsMax, 10), 2, True)
			For e As Integer = 0 To 9
				Dim cnt As Integer = 0
				Do While adsi.MoveNext()
					Dim ds As DataSet = adsi.Current
					' log.info("Features ptr: {}", AtomicAllocator.getInstance().getPointer(mds.getFeatures()[0].data()).address());
					assertEquals(CDbl(cnt), ds.Features.meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.25, ds.Labels.meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.5, ds.FeaturesMaskArray.meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.75, ds.LabelsMaskArray.meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					cnt += 1
				Loop
				adsi.reset()
				' log.info("Epoch {} finished...", e);
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Variable Time Series 2") void testVariableTimeSeries2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testVariableTimeSeries2()
			Dim adsi As New AsyncDataSetIterator(New VariableTimeseriesGenerator(1192, 100, 32, 128, 100, 100, 100), 2, True, New InterleavedDataSetCallback(2 * 2))
			For e As Integer = 0 To 4
				Dim cnt As Integer = 0
				Do While adsi.MoveNext()
					Dim ds As DataSet = adsi.Current
					ds.detach()
					' log.info("Features ptr: {}", AtomicAllocator.getInstance().getPointer(mds.getFeatures()[0].data()).address());
					assertEquals(CDbl(cnt), ds.Features.meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.25, ds.Labels.meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.5, ds.FeaturesMaskArray.meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					assertEquals(CDbl(cnt) + 0.75, ds.LabelsMaskArray.meanNumber().doubleValue(), 1e-10,"Failed on epoch " & e & "; iteration: " & cnt & ";")
					cnt += 1
				Loop
				adsi.reset()
				' log.info("Epoch {} finished...", e);
			Next e
		End Sub
	End Class

End Namespace