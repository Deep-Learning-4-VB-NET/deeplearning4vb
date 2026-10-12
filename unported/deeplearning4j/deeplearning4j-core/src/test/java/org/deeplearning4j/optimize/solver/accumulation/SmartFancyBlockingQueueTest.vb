Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports SmartFancyBlockingQueue = org.deeplearning4j.optimize.solvers.accumulation.SmartFancyBlockingQueue
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ThreadUtils = org.nd4j.common.util.ThreadUtils
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
import static org.junit.jupiter.api.Assertions.assertTimeout
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
'ORIGINAL LINE: @Slf4j @Disabled("AB 2019/05/21 - Failing (stuck, causing timeouts) - Issue #7657") @DisplayName("Smart Fancy Blocking Queue Test") class SmartFancyBlockingQueueTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class SmartFancyBlockingQueueTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test SFBQ _ 1") void testSFBQ_1()
		Friend Overridable Sub testSFBQ_1()
			assertTimeout(ofMillis(120000), Sub()
			Dim queue As val = New SmartFancyBlockingQueue(8, Nd4j.create(5, 5))
			Dim array As val = Nd4j.create(5, 5)
			For e As Integer = 0 To 5
				queue.put(Nd4j.create(5, 5).assign(e))
			Next e
			assertEquals(6, queue.size())
			For e As Integer = 6 To 9
				queue.put(Nd4j.create(5, 5).assign(e))
			Next e
			assertEquals(1, queue.size())
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test SFBQ _ 2") void testSFBQ_2()
		Friend Overridable Sub testSFBQ_2()
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: assertTimeout(ofMillis(120000), () -> { final lombok.val queue = new org.deeplearning4j.optimize.solvers.accumulation.SmartFancyBlockingQueue(1285601, org.nd4j.linalg.factory.Nd4j.create(5, 5)); final lombok.val barrier = new java.util.concurrent.CyclicBarrier(4); lombok.val threads = new java.util.ArrayList<Thread>(); for(int e = 0; e < 4; e++) { lombok.val f = e; lombok.val t = new Thread(new Runnable()
			assertTimeout(ofMillis(120000), Function() { final val queue = New SmartFancyBlockingQueue(1285601, Nd4j.create(5, 5)); final val barrier = New CyclicBarrier(4); val threads = New List(Of Thread)(); for(Integer e = 0; e < 4; e++) { val f = e; val t = New Thread(Sub()
					Dim cnt As Integer = 0
					Do
						Do While cnt < 1000
							If Not queue.isEmpty() Then
								If cnt Mod 50 = 0 Then
									log.info("Thread {}: [{}]", f, cnt)
								End If
								Dim arr As val = queue.poll()
								assertNotNull(arr)
								Dim local As val = arr.unsafeDuplication(True)
								assertEquals(cnt, local.meanNumber().intValue())
								cnt += 1
							End If
							Try
								barrier.await()
								If f = 0 Then
									queue.registerConsumers(4)
								End If
								barrier.await()
							Catch e1 As InterruptedException
								Console.WriteLine(e1.ToString())
								Console.Write(e1.StackTrace)
							Catch e1 As BrokenBarrierException
								Console.WriteLine(e1.ToString())
								Console.Write(e1.StackTrace)
							End Try
						Loop
						Exit Do
					Loop
			End Sub)
					t.setName("reader thread " & f)
					t.start()
					threads.add(t)
		End Sub
				Function for(ByVal e As Integer) As Friend Overridable
					queue.put(Nd4j.create(5, 5).assign(e))
					Nd4j.Executioner.commit()
				End Function
				For Each t As val In threads
					t.join()
				Next t
	End Class
			)
		}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test SFBQ _ 3") void testSFBQ_3()
		void testSFBQ_3()
		If True Then
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: assertTimeout(ofMillis(120000), () -> { final lombok.val queue = new org.deeplearning4j.optimize.solvers.accumulation.SmartFancyBlockingQueue(1285601, org.nd4j.linalg.factory.Nd4j.create(5, 5)); lombok.val threads = new java.util.ArrayList<Thread>(); for(int e = 0; e < 4; e++) { lombok.val f = e; lombok.val t = new Thread(new Runnable()
			assertTimeout(ofMillis(120000), Function() { final val queue = New SmartFancyBlockingQueue(1285601, Nd4j.create(5, 5)); val threads = New List(Of Thread)(); for(Integer e = 0; e < 4; e++) { val f = e; val t = New Thread(Sub()
					Dim cnt As Integer = 0
					Do
						Do While cnt < 1000
							If Not queue.isEmpty() Then
								If cnt Mod 50 = 0 Then
									log.info("Thread {}: [{}]", f, cnt)
								End If
								Dim arr As val = queue.poll()
								assertNotNull(arr)
								Dim local As val = arr.unsafeDuplication(True)
								cnt += 1
							End If
						Loop
						Exit Do
					Loop
			End Sub)
					t.start()
					threads.add(t)
		End If
				Dim b As val = New Thread(Sub()
				Do
					queue.registerConsumers(4)
					ThreadUtils.uncheckedSleep(30)
				Loop
				End Sub)
				b.setDaemon(True)
				b.start()
				Dim writers As val = New List(Of Thread)()
				For e As Integer = 0 To 3
					Dim t As val = New Thread(Sub()
					For e As Integer = 0 To 249
						Try
							queue.put(Nd4j.createUninitialized(5, 5).assign(e))
							Thread.Sleep(30)
						Catch ex As Exception
							Throw New Exception(ex)
						End Try
					Next e
					End Sub)
					writers.add(t)
					t.start()
				Next e
				For Each t As val In writers
					t.join()
				Next t
				For Each t As val In threads
					t.join()
				Next t
			}
			)
		}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test SFBQ _ 4") void testSFBQ_4()
		void testSFBQ_4()
		If True Then
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: assertTimeout(ofMillis(120000), () -> { final lombok.val queue = new org.deeplearning4j.optimize.solvers.accumulation.SmartFancyBlockingQueue(16, org.nd4j.linalg.factory.Nd4j.create(5, 5)); final lombok.val barrier = new java.util.concurrent.CyclicBarrier(4); lombok.val threads = new java.util.ArrayList<Thread>(); for(int e = 0; e < 4; e++) { lombok.val f = e; lombok.val t = new Thread(new Runnable()
			assertTimeout(ofMillis(120000), Function() { final val queue = New SmartFancyBlockingQueue(16, Nd4j.create(5, 5)); final val barrier = New CyclicBarrier(4); val threads = New List(Of Thread)(); for(Integer e = 0; e < 4; e++) { val f = e; val t = New Thread(Sub()
					Try
						For e As Integer = 0 To 99
							log.info("[Thread {}]: fill phase {}", f, e)
							Dim numUpdates As val = RandomUtils.nextInt(8, 128)
							For p As Integer = 0 To numUpdates - 1
								queue.put(Nd4j.createUninitialized(5, 5))
							Next p
							If f = 0 Then
								queue.registerConsumers(4)
							End If
							barrier.await()
							log.info("[Thread {}]: read phase {}", f, e)
							Do While Not queue.isEmpty()
								Dim arr As val = queue.poll()
								assertNotNull(arr)
							Loop
							barrier.await()
						Next e
					Catch e As InterruptedException
						Throw New Exception(e)
					Catch e As BrokenBarrierException
						Throw New Exception(e)
					End Try
			End Sub)
					t.setName("worker thread " & f)
					t.start()
					threads.add(t)
		End If
				For Each t As val In threads
					t.join()
				Next t
			}
			)
		}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test SFBQ _ 5") void testSFBQ_5()
		void testSFBQ_5()
		If True Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertTimeout(ofMillis(120000), () -> { final lombok.val queue = new org.deeplearning4j.optimize.solvers.accumulation.SmartFancyBlockingQueue(16, org.nd4j.linalg.factory.Nd4j.create(5, 5)); final lombok.val barrier = new java.util.concurrent.CyclicBarrier(4); lombok.val writers = new java.util.ArrayList<Thread>(); for(int e = 0; e < 4; e++) { lombok.val w = new Thread(new Runnable()
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
			assertTimeout(ofMillis(120000), Function() { final val queue = New SmartFancyBlockingQueue(16, Nd4j.create(5, 5)); final val barrier = New CyclicBarrier(4); val writers = New List(Of Thread)(); for(Integer e = 0; e < 4; e) { val w = New Thread(Sub()
					Do
						Try
							Dim n As val = RandomUtils.nextInt(8, 64)
							Dim i As Integer = 1
							Do While i < n + 1
								Dim arr As val = Nd4j.createUninitialized(5, 5).assign(i)
								Nd4j.Executioner.commit()
								queue.put(arr)
								i += 1
							Loop
							ThreadUtils.uncheckedSleep(10)
						Catch e As InterruptedException
							Throw New Exception(e)
						End Try
					Loop
			End Sub)
				e += 1
					w.setName("writer thread " & e)
					w.setDaemon(True)
					w.start()
					writers.add(w)
		End If
				' each reader will read 250 updates. supposedly equal :)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] means = new long[4];
				Dim means(3) As Long
				Dim readers As val = New List(Of Thread)()
				For e As Integer = 0 To 3
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int f = e;
					Dim f As Integer = e
					means(f) = 0
					Dim t As val = New Thread(Sub()
					Try
						Dim cnt As Integer = 0
						Dim fnt As Integer = 0
						Do While cnt < 1000
							If Not queue.isEmpty() Then
								Do While Not queue.isEmpty()
									Dim m As val = queue.poll()
									Dim arr As val = m.unsafeDuplication(True)
									Dim mean As val = arr.meanNumber().longValue()
									assertNotEquals(0, mean,"Failed at cycle: " & cnt)
									means(f) += mean
									cnt += 1
								Loop
								barrier.await()
							End If
							barrier.await()
							If f = 0 Then
								log.info("Read cycle finished")
								queue.registerConsumers(4)
							End If
							barrier.await()
						Loop
					Catch e As InterruptedException
						Throw New Exception(e)
					Catch e As BrokenBarrierException
						Throw New Exception(e)
					End Try
					End Sub)
					t.setName("reader thread " & f)
					t.start()
					readers.add(t)
				Next e
				For Each t As val In readers
					t.join()
				Next t
				' all messages should be the same
				assertEquals(means(0), means(1))
				assertEquals(means(0), means(2))
				assertEquals(means(0), means(3))
			}
			)
		}
	}

End Namespace