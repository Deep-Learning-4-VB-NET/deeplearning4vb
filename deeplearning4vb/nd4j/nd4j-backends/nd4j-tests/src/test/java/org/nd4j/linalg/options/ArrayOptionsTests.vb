﻿Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = org.nd4j.linalg.api.shape.options.ArrayType
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.nd4j.linalg.options

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.JAVA_ONLY) public class ArrayOptionsTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ArrayOptionsTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrayType_0(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrayType_0(ByVal backend As Nd4jBackend)
			Dim shapeInfo() As Long = {2, 2, 2, 2, 1, 0, 1, 99}
			assertEquals(ArrayType.DENSE, ArrayOptionsHelper.arrayType(shapeInfo))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrayType_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrayType_1(ByVal backend As Nd4jBackend)
			Dim shapeInfo() As Long = {2, 2, 2, 2, 1, 0, 1, 99}
			ArrayOptionsHelper.setOptionBit(shapeInfo, ArrayType.EMPTY)

			assertEquals(ArrayType.EMPTY, ArrayOptionsHelper.arrayType(shapeInfo))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrayType_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrayType_2(ByVal backend As Nd4jBackend)
			Dim shapeInfo() As Long = {2, 2, 2, 2, 1, 0, 1, 99}
			ArrayOptionsHelper.setOptionBit(shapeInfo, ArrayType.SPARSE)

			assertEquals(ArrayType.SPARSE, ArrayOptionsHelper.arrayType(shapeInfo))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrayType_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrayType_3(ByVal backend As Nd4jBackend)
			Dim shapeInfo() As Long = {2, 2, 2, 2, 1, 0, 1, 99}
			ArrayOptionsHelper.setOptionBit(shapeInfo, ArrayType.COMPRESSED)

			assertEquals(ArrayType.COMPRESSED, ArrayOptionsHelper.arrayType(shapeInfo))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDataTypesToFromLong(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDataTypesToFromLong(ByVal backend As Nd4jBackend)

			For Each dt As DataType In DataType.values()
				If dt = DataType.UNKNOWN Then
					Continue For
				End If
				Dim s As String = dt.ToString()
				Dim l As Long = 0
				l = ArrayOptionsHelper.setOptionBit(l, dt)
				assertNotEquals(0, l,s)
				Dim dt2 As DataType = ArrayOptionsHelper.dataType(l)
				assertEquals(dt, dt2,s)
			Next dt

		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace