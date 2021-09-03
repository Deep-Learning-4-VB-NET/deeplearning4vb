Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.python4j
import static org.bytedeco.cpython.global.python.PyGILState_Check
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) @Tag(TagNames.MULTI_THREADED) public class PythonMultiThreadTest
Public Class PythonMultiThreadTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiThreading1()throws Throwable
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testMultiThreading1()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Throwable> exceptions = java.util.Collections.synchronizedList(new java.util.ArrayList<Throwable>());
		Dim exceptions As IList(Of Exception) = Collections.synchronizedList(New List(Of Exception)())
		Dim runnable As ThreadStart = Sub()
			Try
				Using gil As PythonGIL = PythonGIL.lock()
					Using gc As PythonGC = PythonGC.watch()
						Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
						inputs.Add(New PythonVariable(Of )("x", PythonTypes.STR, "Hello "))
						inputs.Add(New PythonVariable(Of )("y", PythonTypes.STR, "World"))
						Dim [out] As PythonVariable = New PythonVariable(Of )("z", PythonTypes.STR)
						Dim code As String = "z = x + y"
						PythonExecutioner.exec(code, inputs, Collections.singletonList([out]))
						assertEquals("Hello World", [out].getValue())
						Console.WriteLine([out].getValue() & " From thread " & Thread.CurrentThread.getId())
					End Using
				End Using
			Catch e As Exception
				exceptions.Add(e)
			End Try
		End Sub

		Dim numThreads As Integer = 10
		Dim threads(numThreads - 1) As Thread
		For i As Integer = 0 To threads.Length - 1
			threads(i) = New Thread(runnable)
		Next i
		For i As Integer = 0 To threads.Length - 1
			threads(i).Start()
		Next i
		Thread.Sleep(100)
		For i As Integer = 0 To threads.Length - 1
			threads(i).Join()
		Next i
		If exceptions.Count > 0 Then
			Throw (exceptions(0))
		End If

	End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiThreading2()throws Throwable
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testMultiThreading2()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Throwable> exceptions = java.util.Collections.synchronizedList(new java.util.ArrayList<Throwable>());
		Dim exceptions As IList(Of Exception) = Collections.synchronizedList(New List(Of Exception)())
		Dim runnable As ThreadStart = Sub()
			Try
				Using gil As PythonGIL = PythonGIL.lock()
					Using gc As PythonGC = PythonGC.watch()
						PythonContextManager.reset()
						PythonContextManager.reset()
						Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
						inputs.Add(New PythonVariable(Of )("a", PythonTypes.INT, 5))
						Dim code As String = "b = '10'" & vbLf & "c = 20.0 + a"
						Dim vars As IList(Of PythonVariable) = PythonExecutioner.execAndReturnAllVariables(code, inputs)
    
						assertEquals("a", vars(0).getName())
						assertEquals(PythonTypes.INT, vars(0).getType())
						assertEquals(5L, CLng(vars(0).getValue()))
    
						assertEquals("b", vars(1).getName())
						assertEquals(PythonTypes.STR, vars(1).getType())
						assertEquals("10", vars(1).getValue().ToString())
    
						assertEquals("c", vars(2).getName())
						assertEquals(PythonTypes.FLOAT, vars(2).getType())
						assertEquals(25.0, CDbl(vars(2).getValue()), 1e-5)
					End Using
				End Using
			Catch e As Exception
				exceptions.Add(e)
			End Try
		End Sub

		Dim numThreads As Integer = 10
		Dim threads(numThreads - 1) As Thread
		For i As Integer = 0 To threads.Length - 1
			threads(i) = New Thread(runnable)
		Next i
		For i As Integer = 0 To threads.Length - 1
			threads(i).Start()
		Next i
		Thread.Sleep(100)
		For i As Integer = 0 To threads.Length - 1
			threads(i).Join()
		Next i
		If exceptions.Count > 0 Then
			Throw (exceptions(0))
		End If
	End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWorkerThreadLongRunning() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
	Public Overridable Sub testWorkerThreadLongRunning()
		Dim numThreads As Integer = 8
		Dim executorService As ExecutorService = Executors.newFixedThreadPool(numThreads)
		Dim tempVar As New PythonExecutioner()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger finishedExecutionCount = new java.util.concurrent.atomic.AtomicInteger(0);
		Dim finishedExecutionCount As New AtomicInteger(0)
		Dim i As Integer = 0
		Do While i < numThreads * 2
			executorService.submit(Sub()
				Using pythonGIL_Conflict As PythonGIL = PythonGIL.lock()
					Console.WriteLine("Using thread " & Thread.CurrentThread.getId() & " to invoke python")
					assertTrue(PyGILState_Check() > 0,"Thread " & Thread.CurrentThread.getId() & " does not hold the gil.")
					PythonExecutioner.exec("import time; time.sleep(10)")
					Console.WriteLine("Finished execution on thread " & Thread.CurrentThread.getId())
					finishedExecutionCount.incrementAndGet()
				End Using
			End Sub)

			i += 1
		Loop

		executorService.awaitTermination(3, TimeUnit.MINUTES)
		assertEquals(numThreads * 2,finishedExecutionCount.get())


	End Sub


End Class
