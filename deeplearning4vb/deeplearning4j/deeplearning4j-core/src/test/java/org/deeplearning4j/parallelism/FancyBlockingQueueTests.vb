Imports System
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.optimize.solvers.accumulation
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.parallelism


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.DL4J_OLD_API) public class FancyBlockingQueueTests extends org.deeplearning4j.BaseDL4JTest
	Public Class FancyBlockingQueueTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFancyQueue1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFancyQueue1()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.optimize.solvers.accumulation.FancyBlockingQueue<Integer> queue = new org.deeplearning4j.optimize.solvers.accumulation.FancyBlockingQueue<>(new java.util.concurrent.LinkedBlockingQueue<Integer>(512), 4);
			Dim queue As New FancyBlockingQueue(Of Integer)(New LinkedBlockingQueue(Of Integer)(512), 4)
			Dim f As Long = 0
			For x As Integer = 0 To 511
				queue.add(x)
				f += x
			Next x

			assertEquals(512, queue.size())


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong e = new java.util.concurrent.atomic.AtomicLong(0);
			Dim e As New AtomicLong(0)

			queue.registerConsumers(4)
			Dim threads(3) As Thread
			For x As Integer = 0 To 3
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int t = x;
				Dim t As Integer = x
				threads(x) = New Thread(Sub()
				Do While Not queue.Empty
					Dim i As Integer? = queue.poll()
					'log.info("i: {}", i);
					e.addAndGet(i)
				Loop
				End Sub)

				threads(x).Start()
			Next x


			For x As Integer = 0 To 3
				threads(x).Join()
			Next x

			assertEquals(f * 4, e.get())
		End Sub

		''' <summary>
		''' This test is +- the same as the first one, just adds variable consumption time
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFancyQueue2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFancyQueue2()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.optimize.solvers.accumulation.FancyBlockingQueue<Integer> queue = new org.deeplearning4j.optimize.solvers.accumulation.FancyBlockingQueue<>(new java.util.concurrent.LinkedBlockingQueue<Integer>(512), 4);
			Dim queue As New FancyBlockingQueue(Of Integer)(New LinkedBlockingQueue(Of Integer)(512), 4)
			Dim f As Long = 0
			For x As Integer = 0 To 511
				queue.add(x)
				f += x
			Next x

			assertEquals(512, queue.size())


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong e = new java.util.concurrent.atomic.AtomicLong(0);
			Dim e As New AtomicLong(0)
			queue.registerConsumers(4)

			Dim threads(3) As Thread
			For x As Integer = 0 To 3
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int t = x;
				Dim t As Integer = x
				threads(x) = New Thread(Sub()
				Do While Not queue.Empty
					Dim i As Integer? = queue.poll()
					e.addAndGet(i)

					Try
						Thread.Sleep(RandomUtils.nextInt(1, 5))
					Catch e As Exception
						'
					End Try
				Loop
				End Sub)

				threads(x).Start()
			Next x


			For x As Integer = 0 To 3
				threads(x).Join()
			Next x

			assertEquals(f * 4, e.get())
		End Sub


		''' <summary>
		''' This test checks for compatibility with single producer - single consumer model </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFancyQueue3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFancyQueue3()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.optimize.solvers.accumulation.FancyBlockingQueue<Integer> queue = new org.deeplearning4j.optimize.solvers.accumulation.FancyBlockingQueue<>(new java.util.concurrent.LinkedBlockingQueue<Integer>(512), 4);
			Dim queue As New FancyBlockingQueue(Of Integer)(New LinkedBlockingQueue(Of Integer)(512), 4)
			Dim f As Long = 0
			For x As Integer = 0 To 511
				queue.add(x)
				f += x
			Next x

			assertEquals(512, queue.size())


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong e = new java.util.concurrent.atomic.AtomicLong(0);
			Dim e As New AtomicLong(0)
			queue.registerConsumers(1)
			Dim threads(0) As Thread
			For x As Integer = 0 To 0
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int t = x;
				Dim t As Integer = x
				threads(x) = New Thread(Sub()
				Do While Not queue.Empty
					Dim i As Integer? = queue.poll()
					'log.info("i: {}", i);
					e.addAndGet(i)
				Loop
				End Sub)

				threads(x).Start()
			Next x


			For x As Integer = 0 To 0
				threads(x).Join()
			Next x

			assertEquals(f, e.get())
		End Sub

		''' <summary>
		''' This test checks for compatibility with single producer - single consumer model </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFancyQueue4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFancyQueue4()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.optimize.solvers.accumulation.FancyBlockingQueue<Integer> queue = new org.deeplearning4j.optimize.solvers.accumulation.FancyBlockingQueue<>(new java.util.concurrent.LinkedBlockingQueue<Integer>(512), 4);
			Dim queue As New FancyBlockingQueue(Of Integer)(New LinkedBlockingQueue(Of Integer)(512), 4)
			Dim f As Long = 0
			For x As Integer = 0 To 511
				queue.add(x)
				f += x
			Next x

			assertEquals(512, queue.size())


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong e = new java.util.concurrent.atomic.AtomicLong(0);
			Dim e As New AtomicLong(0)
			queue.fallbackToSingleConsumerMode(True)
			Dim threads(0) As Thread
			For x As Integer = 0 To 0
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int t = x;
				Dim t As Integer = x
				threads(x) = New Thread(Sub()
				Do While Not queue.Empty
					Dim i As Integer? = queue.poll()
					'log.info("i: {}", i);
					e.addAndGet(i)
				Loop
				End Sub)

				threads(x).Start()
			Next x


			For x As Integer = 0 To 0
				threads(x).Join()
			Next x

			assertEquals(f, e.get())
		End Sub
	End Class

End Namespace