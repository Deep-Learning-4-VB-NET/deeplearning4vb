Imports System.Collections.Generic
Imports System.Threading
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.multithreading


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.WORKSPACES) @Tag(TagNames.MULTI_THREADED) public class MultithreadedTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class MultithreadedTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void basicMigrationTest_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub basicMigrationTest_1()
			If Nd4j.AffinityManager.NumberOfDevices < 2 Then
				Return
			End If

			Dim exp As val = Nd4j.create(DataType.INT32, 5, 5).assign(2)

			Dim hash As val = New HashSet(Of Integer)()

			' we're creating bunch of arrays on different devices
			Dim list As val = New List(Of INDArray)()
			Dim e As Integer = 0
			Do While e < Nd4j.AffinityManager.NumberOfDevices
				Dim t As val = e
				Dim thread As val = New Thread(Sub()
				For f As Integer = 0 To 9
					Dim array As val = Nd4j.create(DataType.INT32, 5, 5).assign(1)
					hash.add(Nd4j.AffinityManager.getDeviceForCurrentThread())
					assertEquals(Nd4j.AffinityManager.getDeviceForCurrentThread(), Nd4j.AffinityManager.getDeviceForArray(array))
					list.add(array)
				Next f
				End Sub)

				thread.start()
				thread.join()
				e += 1
			Loop

			' lets make sure all devices covered
			assertEquals(Nd4j.AffinityManager.NumberOfDevices, hash.size())

			' make sure nothing failed in threads
			assertEquals(10 * Nd4j.AffinityManager.NumberOfDevices, list.size())

			' now we're going to use arrays on current device, so data will be migrated
			For Each arr As val In list
				arr.addi(1)

				assertEquals(exp, arr)
			Next arr
		End Sub
	End Class

End Namespace