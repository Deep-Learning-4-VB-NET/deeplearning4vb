Imports System
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse

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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.SAMEDIFF) @Tag(TagNames.MULTI_THREADED) public class SameDiffMultiThreadTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class SameDiffMultiThreadTests
		Inherits BaseND4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSimple(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSimple(ByVal backend As Nd4jBackend)

			Dim nThreads As Integer = 4
			Dim nRuns As Integer = 1000

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 10)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 10)

			Dim w1 As SDVariable = sd.var("w1", Nd4j.rand(DataType.FLOAT, 10, 10))
			Dim b1 As SDVariable = sd.var("b1", Nd4j.rand(DataType.FLOAT, 10))
			Dim w2 As SDVariable = sd.var("w2", Nd4j.rand(DataType.FLOAT, 10, 10))
			Dim b2 As SDVariable = sd.var("b2", Nd4j.rand(DataType.FLOAT, 10))
			Dim w3 As SDVariable = sd.var("w3", Nd4j.rand(DataType.FLOAT, 10, 10))
			Dim b3 As SDVariable = sd.var("b3", Nd4j.rand(DataType.FLOAT, 10))

			Dim l1 As SDVariable = sd.nn_Conflict.tanh([in].mmul(w1).add(b1))
			Dim l2 As SDVariable = sd.nn_Conflict.sigmoid(l1.mmul(w2).add(b2))
			Dim l3 As SDVariable = sd.nn_Conflict.softmax("out", l2.mmul(w3).add(b3))

			Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, l3)

			Dim inputArrs(nThreads - 1) As INDArray
			Dim expOut(nThreads - 1) As INDArray
			For i As Integer = 0 To nThreads - 1
				inputArrs(i) = Nd4j.rand(DataType.FLOAT, i+1, 10)
				expOut(i) = sd.outputSingle(Collections.singletonMap("in", inputArrs(i)), "out")
			Next i

			Dim s As New Semaphore(nThreads)
			Dim latch As New System.Threading.CountdownEvent(nThreads)

			Dim failuresByThread(nThreads - 1) As AtomicBoolean
			Dim counters(nThreads - 1) As AtomicInteger
			doTest(sd, nThreads, nRuns, inputArrs, expOut, "in", "out", failuresByThread, counters, s, latch)

			s.release(nThreads)
			latch.await()

			For i As Integer = 0 To nThreads - 1
				assertFalse(failuresByThread(i).get(),"Thread " & i & " failed")
			Next i

			For i As Integer = 0 To nThreads - 1
				assertEquals(nRuns, counters(i).get(),"Thread " & i & " number of runs")
			Next i
		End Sub



		Public Shared Sub doTest(ByVal sd As SameDiff, ByVal nThreads As Integer, ByVal nRuns As Integer, ByVal inputArrs() As INDArray, ByVal expOut() As INDArray, ByVal inName As String, ByVal outName As String, ByVal failuresByThread() As AtomicBoolean, ByVal counters() As AtomicInteger, ByVal s As Semaphore, ByVal latch As System.Threading.CountdownEvent)

			For i As Integer = 0 To nThreads - 1
				failuresByThread(i) = New AtomicBoolean(False)
				counters(i) = New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int j=i;
				Dim j As Integer=i
				Dim t As New Thread(Sub()
				Try
					s.acquire(1)
					For i As Integer = 0 To nRuns - 1
						Dim [out] As INDArray = sd.outputSingle(Collections.singletonMap(inName, inputArrs(j)), outName)
						Nd4j.Executioner.commit()
						Dim exp As INDArray = expOut(j)

						If Not exp.Equals([out]) Then
							failuresByThread(j).set(True)
							log.error("Failure in thread: {}/{} - iteration {}" & vbLf & "Expected ={}" & vbLf & "Actual={}", Thread.CurrentThread.getId(), j, i, exp, [out])
							Exit For
						End If

						If [out].closeable() Then
							[out].close()
						End If

'                            if(i % 100 == 0){
'                                log.info("Thread {} at {}", Thread.currentThread().getId(), i);
'                            }
						counters(j).addAndGet(1)
					Next i
				Catch t As Exception
					log.error("Error in thread: {}", Thread.CurrentThread.getId(), t)
				Finally
					latch.Signal()
				End Try
				End Sub)
				t.Start()
			Next i
		End Sub
	End Class

End Namespace