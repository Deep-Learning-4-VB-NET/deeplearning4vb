Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.python4j
Imports Test = org.junit.jupiter.api.Test
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

'
' *
' *  *  ******************************************************************************
' *  *  *
' *  *  *
' *  *  * This program and the accompanying materials are made available under the
' *  *  * terms of the Apache License, Version 2.0 which is available at
' *  *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *  *
' *  *  *  See the NOTICE file distributed with this work for additional
' *  *  *  information regarding copyright ownership.
' *  *  * Unless required by applicable law or agreed to in writing, software
' *  *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  *  * License for the specific language governing permissions and limitations
' *  *  * under the License.
' *  *  *
' *  *  * SPDX-License-Identifier: Apache-2.0
' *  *  *****************************************************************************
' *
' *
' 

Namespace org.nd4j.python4j
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
'ORIGINAL LINE: @NotThreadSafe @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.PYTHON) public class PythonNumpyMultiThreadTest
	Public Class PythonNumpyMultiThreadTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void init()
		Public Shared Sub init()
			Call (New NumpyArray()).init()
		End Sub

		Public Shared Function params() As Stream(Of Arguments)
			Return Arrays.asList(New DataType(){ DataType.FLOAT, DataType.DOUBLE, DataType.INT32, DataType.INT64}).Select(AddressOf Arguments.of)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.nd4j.python4j.PythonNumpyMultiThreadTest#params") @ParameterizedTest public void testMultiThreading1(org.nd4j.linalg.api.buffer.DataType dataType) throws Throwable
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiThreading1(ByVal dataType As DataType)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Throwable> exceptions = java.util.Collections.synchronizedList(new java.util.ArrayList<Throwable>());
			Dim exceptions As IList(Of Exception) = Collections.synchronizedList(New List(Of Exception)())
			Dim runnable As ThreadStart = Sub()
			Try
				Using gil As PythonGIL = PythonGIL.lock()
					Using gc As PythonGC = PythonGC.watch()
						Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
						inputs.Add(New PythonVariable(Of )("x", NumpyArray.INSTANCE, Nd4j.ones(dataType, 2, 3).mul(3)))
						inputs.Add(New PythonVariable(Of )("y", NumpyArray.INSTANCE, Nd4j.ones(dataType, 2, 3).mul(4)))
						Dim [out] As PythonVariable = New PythonVariable(Of )("z", NumpyArray.INSTANCE)
						Dim code As String = "z = x + y"
						PythonExecutioner.exec(code, inputs, Collections.singletonList([out]))
						assertEquals(Nd4j.ones(dataType, 2, 3).mul(7), [out].getValue())
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
'ORIGINAL LINE: @MethodSource("org.nd4j.python4j.PythonNumpyMultiThreadTest#params") @ParameterizedTest public void testMultiThreading2(org.nd4j.linalg.api.buffer.DataType dataType) throws Throwable
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiThreading2(ByVal dataType As DataType)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Throwable> exceptions = java.util.Collections.synchronizedList(new java.util.ArrayList<>());
			Dim exceptions As IList(Of Exception) = Collections.synchronizedList(New List(Of Exception)())
			Dim runnable As ThreadStart = Sub()
			Try
				Using gil As PythonGIL = PythonGIL.lock()
					Using gc As PythonGC = PythonGC.watch()
						PythonContextManager.reset()
						Dim inputs As IList(Of PythonVariable) = New List(Of PythonVariable)()
						inputs.Add(New PythonVariable(Of )("x", NumpyArray.INSTANCE, Nd4j.ones(dataType, 2, 3).mul(3)))
						inputs.Add(New PythonVariable(Of )("y", NumpyArray.INSTANCE, Nd4j.ones(dataType, 2, 3).mul(4)))
						Dim code As String = "z = x + y"
						Dim outputs As IList(Of PythonVariable) = PythonExecutioner.execAndReturnAllVariables(code, inputs)
						assertEquals(Nd4j.ones(dataType, 2, 3).mul(3), outputs(0).getValue())
						assertEquals(Nd4j.ones(dataType, 2, 3).mul(4), outputs(1).getValue())
						assertEquals(Nd4j.ones(dataType, 2, 3).mul(7), outputs(2).getValue())
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


	End Class

End Namespace