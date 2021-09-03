﻿Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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

Namespace org.nd4j.linalg.shape.ones

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class LeadingAndTrailingOnesC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LeadingAndTrailingOnesC
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateLeadingAndTrailingOnes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateLeadingAndTrailingOnes(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(1, 10, 1, 1)
			arr.assign(1)
	'        System.out.println(arr);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrix(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrix(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			Dim slice1 As INDArray = arr.slice(1)
	'        System.out.println(arr.slice(1));
			Dim oneInMiddle As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 1, 2)
			Dim otherSlice As INDArray = oneInMiddle.slice(1)
			assertEquals(2, otherSlice.offset())
	'        System.out.println(otherSlice);
			Dim twoOnesInMiddle As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 1, 1, 2)
			Dim [sub] As INDArray = twoOnesInMiddle.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
			assertEquals(2, [sub].offset())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultipleOnesInMiddle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultipleOnesInMiddle(ByVal backend As Nd4jBackend)
			Dim tensor As INDArray = Nd4j.linspace(1, 144, 144).reshape(ChrW(2), 2, 1, 1, 6, 6)
			Dim tensorSlice1 As INDArray = tensor.slice(1)
			Dim tensorSlice1Slice1 As INDArray = tensorSlice1.slice(1)
	'        System.out.println(tensor);
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace