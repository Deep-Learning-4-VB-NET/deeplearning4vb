Imports System.Linq
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
import static org.junit.jupiter.api.Assertions.assertEquals
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
Namespace org.deeplearning4j.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Array Util Test") @Tag(TagNames.JAVA_ONLY) class ArrayUtilTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ArrayUtilTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Range") void testRange()
		Friend Overridable Sub testRange()
			Dim range() As Integer = ArrayUtil.range(0, 2)
			Dim test() As Integer = { 0, 1 }
			assertEquals(True, test.SequenceEqual(range))
			Dim test2() As Integer = { -1, 0 }
			Dim range2() As Integer = ArrayUtil.range(-1, 1)
			assertEquals(True, test2.SequenceEqual(range2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Strides") void testStrides()
		Friend Overridable Sub testStrides()
			Dim shape() As Integer = { 5, 4, 3 }
			Dim cStyleStride() As Integer = { 12, 3, 1 }
			Dim fortranStyleStride() As Integer = { 1, 5, 20 }
			Dim fortranStyleTest() As Integer = ArrayUtil.calcStridesFortran(shape)
			Dim cStyleTest() As Integer = ArrayUtil.calcStrides(shape)
			assertEquals(True, cStyleStride.SequenceEqual(cStyleTest))
			assertEquals(True, fortranStyleStride.SequenceEqual(fortranStyleTest))
			Dim shape2() As Integer = { 2, 2 }
			Dim cStyleStride2() As Integer = { 2, 1 }
			Dim fortranStyleStride2() As Integer = { 1, 2 }
			Dim cStyleTest2() As Integer = ArrayUtil.calcStrides(shape2)
			Dim fortranStyleTest2() As Integer = ArrayUtil.calcStridesFortran(shape2)
			assertEquals(True, cStyleStride2.SequenceEqual(cStyleTest2))
			assertEquals(True, fortranStyleStride2.SequenceEqual(fortranStyleTest2))
		End Sub
	End Class

End Namespace