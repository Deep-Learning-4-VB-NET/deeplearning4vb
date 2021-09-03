Imports System
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports FlatArray = org.nd4j.graph.FlatArray
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.linalg.mixed

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class StringArrayTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class StringArrayTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicStrings_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicStrings_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.scalar("alpha")

			assertNotNull(array)
			assertEquals(1, array.length())
			assertEquals(0, array.rank())
			assertEquals(DataType.UTF8, array.dataType())

			assertEquals("alpha", array.getString(0))
			Dim s As String = array.ToString()
			assertTrue(s.Contains("alpha"),s)
			Console.WriteLine(s)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicStrings_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicStrings_2(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.create("alpha","beta", "gamma")

			assertNotNull(array)
			assertEquals(3, array.length())
			assertEquals(1, array.rank())
			assertEquals(DataType.UTF8, array.dataType())

			assertEquals("alpha", array.getString(0))
			assertEquals("beta", array.getString(1))
			assertEquals("gamma", array.getString(2))
			Dim s As String = array.ToString()
			assertTrue(s.Contains("alpha"),s)
			assertTrue(s.Contains("beta"),s)
			assertTrue(s.Contains("gamma"),s)
			Console.WriteLine(s)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicStrings_3()
		Public Overridable Sub testBasicStrings_3()
			Dim arrayX As val = Nd4j.create("alpha", "beta", "gamma")
			Dim arrayY As val = Nd4j.create("alpha", "beta", "gamma")
			Dim arrayZ As val = Nd4j.create("Alpha", "bEta", "gamma")

			assertEquals(arrayX, arrayX)
			assertEquals(arrayX, arrayY)
			assertNotEquals(arrayX, arrayZ)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicStrings_4()
		Public Overridable Sub testBasicStrings_4()
			Dim arrayX As val = Nd4j.create("alpha", "beta", "gamma")

			Dim fb As val = New FlatBufferBuilder()
			Dim i As val = arrayX.toFlatArray(fb)
			fb.finish(i)
			Dim db As val = fb.dataBuffer()

			Dim flat As val = FlatArray.getRootAsFlatArray(db)
			Dim restored As val = Nd4j.createFromFlatArray(flat)

			assertEquals(arrayX, restored)
			assertEquals("alpha", restored.getString(0))
			assertEquals("beta", restored.getString(1))
			assertEquals("gamma", restored.getString(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicStrings_4a()
		Public Overridable Sub testBasicStrings_4a()
			Dim arrayX As val = Nd4j.scalar("alpha")

			Dim fb As val = New FlatBufferBuilder()
			Dim i As val = arrayX.toFlatArray(fb)
			fb.finish(i)
			Dim db As val = fb.dataBuffer()

			Dim flat As val = FlatArray.getRootAsFlatArray(db)
			Dim restored As val = Nd4j.createFromFlatArray(flat)

			assertEquals("alpha", arrayX.getString(0))

			assertEquals(arrayX, restored)
			assertEquals("alpha", restored.getString(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicStrings_5()
		Public Overridable Sub testBasicStrings_5()
			Dim arrayX As val = Nd4j.create("alpha", "beta", "gamma")
			Dim arrayZ0 As val = arrayX.dup()
			Dim arrayZ1 As val = arrayX.dup(arrayX.ordering())

			assertEquals(arrayX, arrayZ0)
			assertEquals(arrayX, arrayZ1)
		End Sub
	End Class

End Namespace