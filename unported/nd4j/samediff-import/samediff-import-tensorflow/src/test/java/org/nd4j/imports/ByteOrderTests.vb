Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports FlatArray = org.nd4j.graph.FlatArray
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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

Namespace org.nd4j.imports


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ByteOrderTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ByteOrderTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(False)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(False)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testByteArrayOrder1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testByteArrayOrder1(ByVal backend As Nd4jBackend)
			Dim ndarray As val = Nd4j.create(DataType.FLOAT, 2).assign(1)

			assertEquals(DataType.FLOAT, ndarray.data().dataType())

			Dim array As val = ndarray.data().asBytes()

			assertEquals(8, array.length)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testByteArrayOrder2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testByteArrayOrder2(ByVal backend As Nd4jBackend)
			Dim original As val = Nd4j.linspace(1, 25, 25, DataType.FLOAT).reshape(ChrW(5), 5)
			Dim bufferBuilder As val = New FlatBufferBuilder(0)

			Dim array As Integer = original.toFlatArray(bufferBuilder)
			bufferBuilder.finish(array)

			Dim flatArray As val = FlatArray.getRootAsFlatArray(bufferBuilder.dataBuffer())

			Dim restored As val = Nd4j.createFromFlatArray(flatArray)

			assertEquals(original, restored)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testByteArrayOrder3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testByteArrayOrder3(ByVal backend As Nd4jBackend)
			Dim original As val = Nd4j.linspace(1, 25, 25, DataType.FLOAT).reshape("f"c, 5, 5)
			Dim bufferBuilder As val = New FlatBufferBuilder(0)

			Dim array As Integer = original.toFlatArray(bufferBuilder)
			bufferBuilder.finish(array)

			Dim flatArray As val = FlatArray.getRootAsFlatArray(bufferBuilder.dataBuffer())

			Dim restored As val = Nd4j.createFromFlatArray(flatArray)

			assertEquals(original, restored)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeStridesOf1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShapeStridesOf1(ByVal backend As Nd4jBackend)
			Dim buffer As val = New Integer(){2, 5, 5, 5, 1, 0, 1, 99}

			Dim shape As val = Shape.shapeOf(buffer)
			Dim strides As val = Shape.stridesOf(buffer)

			assertArrayEquals(New Integer(){5, 5}, shape)
			assertArrayEquals(New Integer(){5, 1}, strides)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeStridesOf2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShapeStridesOf2(ByVal backend As Nd4jBackend)
			Dim buffer As val = New Integer(){3, 5, 5, 5, 25, 5, 1, 0, 1, 99}

			Dim shape As val = Shape.shapeOf(buffer)
			Dim strides As val = Shape.stridesOf(buffer)

			assertArrayEquals(New Integer(){5, 5, 5}, shape)
			assertArrayEquals(New Integer(){25, 5, 1}, strides)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScalarEncoding(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScalarEncoding(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.scalar(2.0f)

			Dim bufferBuilder As New FlatBufferBuilder(0)
			Dim fb As val = scalar.toFlatArray(bufferBuilder)
			bufferBuilder.finish(fb)
			Dim db As val = bufferBuilder.dataBuffer()

			Dim flat As val = FlatArray.getRootAsFlatArray(db)


			Dim restored As val = Nd4j.createFromFlatArray(flat)

			assertEquals(scalar, restored)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorEncoding_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorEncoding_1(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.createFromArray(New Single(){1, 2, 3, 4, 5})

			Dim bufferBuilder As New FlatBufferBuilder(0)
			Dim fb As val = scalar.toFlatArray(bufferBuilder)
			bufferBuilder.finish(fb)
			Dim db As val = bufferBuilder.dataBuffer()

			Dim flat As val = FlatArray.getRootAsFlatArray(db)

			Dim restored As val = Nd4j.createFromFlatArray(flat)

			assertEquals(scalar, restored)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorEncoding_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorEncoding_2(ByVal backend As Nd4jBackend)
			Dim scalar As val = Nd4j.createFromArray(New Double(){1, 2, 3, 4, 5})

			Dim bufferBuilder As New FlatBufferBuilder(0)
			Dim fb As val = scalar.toFlatArray(bufferBuilder)
			bufferBuilder.finish(fb)
			Dim db As val = bufferBuilder.dataBuffer()

			Dim flat As val = FlatArray.getRootAsFlatArray(db)

			Dim restored As val = Nd4j.createFromFlatArray(flat)

			assertEquals(scalar, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStringEncoding_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStringEncoding_1(ByVal backend As Nd4jBackend)
			Dim strings As val = Arrays.asList("alpha", "beta", "gamma")
			Dim vector As val = Nd4j.create(strings, 3)

			Dim bufferBuilder As val = New FlatBufferBuilder(0)

			Dim fb As val = vector.toFlatArray(bufferBuilder)
			bufferBuilder.finish(fb)
			Dim db As val = bufferBuilder.dataBuffer()

			Dim flat As val = FlatArray.getRootAsFlatArray(db)

			Dim restored As val = Nd4j.createFromFlatArray(flat)

			assertEquals(vector, restored)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace