Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IndexedTail = org.deeplearning4j.optimize.solvers.accumulation.IndexedTail
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.optimize.solver.accumulation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Indexed Tail Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Disabled("IndexedTail is only used in a few places in the v1 of spark, multi threaded usage fails on gpu. ") class IndexedTailTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class IndexedTailTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Deltas _ 1") void testDeltas_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testDeltas_1()
			Dim tail As val = New IndexedTail(2)
			assertFalse(tail.hasAnything(11))
			assertFalse(tail.hasAnything(22))
			' 3 updates in queue
			tail.put(Nd4j.create(5, 5))
			tail.put(Nd4j.create(5, 5))
			tail.put(Nd4j.create(5, 5))
			assertEquals(3, tail.getDelta(11))
			assertEquals(3, tail.getDelta(22))
			tail.drainTo(22, Nd4j.create(5, 5))
			assertEquals(3, tail.getDelta(11))
			assertEquals(0, tail.getDelta(22))
			tail.put(Nd4j.create(5, 5))
			assertEquals(4, tail.getDelta(11))
			assertEquals(1, tail.getDelta(22))
			tail.drainTo(22, Nd4j.create(5, 5))
			tail.drainTo(11, Nd4j.create(5, 5))
			assertEquals(0, tail.getDelta(11))
			assertEquals(0, tail.getDelta(22))
			tail.put(Nd4j.create(5, 5))
			tail.put(Nd4j.create(5, 5))
			assertEquals(2, tail.getDelta(11))
			assertEquals(2, tail.getDelta(22))
			tail.drainTo(22, Nd4j.create(5, 5))
			assertEquals(2, tail.getDelta(11))
			assertEquals(0, tail.getDelta(22))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Max Applied Index _ 1") void testMaxAppliedIndex_1()
		Friend Overridable Sub testMaxAppliedIndex_1()
			Dim tail As val = New IndexedTail(3)
			' "registering" 3 consumers
			assertFalse(tail.hasAnything(11))
			assertFalse(tail.hasAnything(22))
			assertFalse(tail.hasAnything(33))
			' putting 10 updates in
			For e As Integer = 0 To 9
				tail.put(Nd4j.create(5, 5))
			Next e
			assertEquals(10, tail.updatesSize())
			assertEquals(-1, tail.maxAppliedIndexEverywhere())
			' 2 consumers consumed 2 elements, and 1 consumer consumed 3 elements
			tail.getPositions().get(11L).set(2)
			tail.getPositions().get(22L).set(2)
			tail.getPositions().get(33L).set(3)
			' all elements including this index are safe to remove, because they were consumed everywhere
			assertEquals(2, tail.maxAppliedIndexEverywhere())
			' only updates starting from 4 are safe to collapse, because 3 was consumed by one consumer
			assertEquals(4, tail.firstNotAppliedIndexEverywhere())
			' truncating stuff
			tail.maintenance()
			assertEquals(8, tail.updatesSize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test First Not Applied _ 1") void testFirstNotApplied_1()
		Friend Overridable Sub testFirstNotApplied_1()
			Dim tail As val = New IndexedTail(1)
			tail.hasAnything()
			assertEquals(-1, tail.firstNotAppliedIndexEverywhere())
			tail.put(Nd4j.createUninitialized(5, 5))
			assertEquals(0, tail.firstNotAppliedIndexEverywhere())
			tail.put(Nd4j.createUninitialized(5, 5))
			tail.put(Nd4j.createUninitialized(5, 5))
			assertEquals(0, tail.firstNotAppliedIndexEverywhere())
			assertTrue(tail.drainTo(Nd4j.create(5, 5)))
			assertEquals(4, tail.firstNotAppliedIndexEverywhere())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Single Threaded _ 1") void testSingleThreaded_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSingleThreaded_1()
			Dim tail As val = New IndexedTail(1)
			For e As Integer = 0 To 99
				Dim orig As val = Nd4j.create(5, 5).assign(e)
				tail.put(orig)
				Nd4j.Executioner.commit()
				assertTrue(tail.hasAnything())
				Dim temp As val = Nd4j.create(5, 5)
				Dim status As val = tail.drainTo(temp)
				assertTrue(status)
				assertArrayEquals(orig.shape(), temp.shape())
				assertEquals(orig, temp)
			Next e
			assertEquals(0, tail.updatesSize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Single Threaded _ 2") void testSingleThreaded_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSingleThreaded_2()
			Dim tail As val = New IndexedTail(1)
			For e As Integer = 0 To 99
				Dim numUpdates As Integer = RandomUtils.nextInt(1, 10)
				Dim sum As Integer = 0
				For f As Integer = 1 To numUpdates
					sum += f
					Dim orig As val = Nd4j.create(5, 5).assign(f)
					tail.put(orig)
				Next f
				Nd4j.Executioner.commit()
				assertTrue(tail.hasAnything())
				Dim temp As val = Nd4j.create(5, 5)
				Dim status As val = tail.drainTo(temp)
				assertTrue(status)
				assertEquals(sum, temp.meanNumber().intValue())
			Next e
			assertEquals(0, tail.updatesSize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Single Threaded _ 3") void testSingleThreaded_3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSingleThreaded_3()
			Dim tail As val = New IndexedTail(2, True, New Long() { 5, 5 })
			assertFalse(tail.hasAnything())
			assertFalse(tail.hasAnything(11))
			Dim sum As Integer = 0
			For e As Integer = 0 To 63
				sum += (e + 1)
				tail.put(Nd4j.createUninitialized(5, 5).assign(e + 1))
				Nd4j.Executioner.commit()
			Next e
			assertTrue(tail.getCollapsedMode().get())
			assertEquals(1, tail.updatesSize())
			Dim array As val = tail.getUpdates().get(32L)
			assertNotNull(array)
			assertEquals(sum, CInt(Math.Truncate(array.getDouble(0))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Pseudo Multi Threaded _ 1") void testPseudoMultiThreaded_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPseudoMultiThreaded_1()
			Dim tail As val = New IndexedTail(2)
			For e As Integer = 0 To 99
				' putting in one thread
				Dim orig As val = Nd4j.create(5, 5).assign(e)
				tail.put(orig)
				Nd4j.Executioner.commit()
				For t As Integer = 0 To 1
					assertTrue(tail.hasAnything(t))
					Dim temp As val = Nd4j.create(5, 5)
					Dim status As val = tail.drainTo(t, temp)
					assertTrue(status)
					assertArrayEquals(orig.shape(), temp.shape())
					assertEquals(orig, temp)
				Next t
			Next e
			assertEquals(0, tail.updatesSize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Threaded _ 1") void testMultiThreaded_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultiThreaded_1()
			Dim numReaders As val = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final lombok.val tail = new org.deeplearning4j.optimize.solvers.accumulation.IndexedTail(numReaders);
			Dim tail As val = New IndexedTail(numReaders)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] sums = new long[numReaders];
			Dim sums(numReaders - 1) As Long
			Dim readers As val = New List(Of Thread)()
			For e As Integer = 0 To numReaders - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int f = e;
				Dim f As Integer = e
				Dim t As val = New Thread(Sub()
				sums(f) = 0
				Do While Not tail.isDead()
					Do While tail.hasAnything()
						Dim updates As val = Nd4j.create(5, 5)
						tail.drainTo(updates)
						Dim mean As val = CInt(Math.Truncate(updates.getDouble(0)))
						sums(f) += mean
					Loop
				Loop
				End Sub)
				t.setName("reader thread " & f)
				t.start()
				readers.add(t)
			Next e
			Dim sum As Integer = 0
			For e As Integer = 0 To 9999
				Dim array As val = Nd4j.create(5, 5).assign(e + 1)
				Nd4j.Executioner.commit()
				sum += (e + 1)
				tail.put(array)
			Next e
			' just wait till everything consumed
			Thread.Sleep(2000)
			tail.notifyDead()
			For Each t As val In readers
				t.join()
			Next t
			For e As Integer = 0 To numReaders - 1
				assertEquals(sum, sums(e),"Failed for reader [" & e & "]")
			Next e
			assertEquals(0, tail.updatesSize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Threaded _ 2") void testMultiThreaded_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultiThreaded_2()
			Dim numReaders As val = 4
			Dim numWriters As val = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final lombok.val tail = new org.deeplearning4j.optimize.solvers.accumulation.IndexedTail(numReaders);
			Dim tail As val = New IndexedTail(numReaders)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] sums = new long[numReaders];
			Dim sums(numReaders - 1) As Long
			Dim readers As val = New List(Of Thread)()
			For e As Integer = 0 To numReaders - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int f = e;
				Dim f As Integer = e
				Dim t As val = New Thread(Sub()
				sums(f) = 0
				Do While Not tail.isDead()
					Do While tail.hasAnything()
						Dim updates As val = Nd4j.create(5, 5)
						tail.drainTo(updates)
						Dim mean As val = CInt(Math.Truncate(updates.getDouble(0)))
						sums(f) += mean
					Loop
				Loop
				End Sub)
				t.setName("reader thread " & f)
				t.start()
				readers.add(t)
			Next e
			Dim writers As val = New List(Of Thread)()
			For e As Integer = 0 To numWriters - 1
				Dim f As val = e
				Dim t As val = New Thread(Sub()
				Dim sum As Integer = 0
				For e1 As Integer = 0 To 999
					Dim array As val = Nd4j.create(5, 5).assign(e1 + 1)
					Nd4j.Executioner.commit()
					sum += (e1 + 1)
					tail.put(array)
				Next e1
				End Sub)
				t.setName("writer thread " & f)
				t.start()
				writers.add(t)
			Next e
			For Each t As val In writers
				t.join()
			Next t
			' just wait till everything consumed
			Thread.Sleep(2000)
			tail.notifyDead()
			For Each t As val In readers
				t.join()
			Next t
			For e As Integer = 0 To numReaders - 1
				assertEquals(500500 * numWriters, sums(e),"Failed for reader [" & e & "]")
			Next e
			assertEquals(0, tail.updatesSize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Threaded _ 3") void testMultiThreaded_3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultiThreaded_3()
			Dim numReaders As val = 4
			Dim numWriters As val = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final lombok.val tail = new org.deeplearning4j.optimize.solvers.accumulation.IndexedTail(numReaders, true, new long[] { 5, 5 });
			Dim tail As val = New IndexedTail(numReaders, True, New Long() { 5, 5 })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] sums = new long[numReaders];
			Dim sums(numReaders - 1) As Long
			Dim readers As val = New List(Of Thread)()
			For e As Integer = 0 To numReaders - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int f = e;
				Dim f As Integer = e
				Dim t As val = New Thread(Sub()
				sums(f) = 0
				Do While Not tail.isDead()
					Do While tail.hasAnything()
						Dim updates As val = Nd4j.create(5, 5)
						tail.drainTo(updates)
						Dim mean As val = CInt(Math.Truncate(updates.getDouble(0)))
						sums(f) += mean
					Loop
				Loop
				End Sub)
				t.setName("reader thread " & f)
				t.start()
				readers.add(t)
			Next e
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger sum = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim sum As New AtomicInteger(0)
			Dim writers As val = New List(Of Thread)()
			For e As Integer = 0 To numWriters - 1
				Dim f As val = e
				Dim t As val = New Thread(Sub()
				For i As Integer = 0 To 255
					Dim array As val = Nd4j.create(5, 5).assign(i + 1)
					Nd4j.Executioner.commit()
					sum.addAndGet(i + 1)
					tail.put(array)
				Next i
				End Sub)
				t.setName("writer thread " & f)
				t.start()
				writers.add(t)
			Next e
			For Each t As val In writers
				t.join()
			Next t
			' just wait till everything consumed
			Thread.Sleep(3000)
			tail.notifyDead()
			For Each t As val In readers
				t.join()
			Next t
			log.info("Readers results: {}", sums)
			For e As Integer = 0 To numReaders - 1
				assertEquals(sum.get(), sums(e),"Failed for reader [" & e & "]")
			Next e
			assertEquals(0, tail.updatesSize())
		End Sub
	End Class

End Namespace