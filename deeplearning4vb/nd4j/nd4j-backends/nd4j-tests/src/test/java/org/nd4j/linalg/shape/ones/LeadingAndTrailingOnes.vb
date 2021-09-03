Imports System.Collections.Generic
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
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class LeadingAndTrailingOnes extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LeadingAndTrailingOnes
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSliceConstructor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSliceConstructor(ByVal backend As Nd4jBackend)
			Dim testList As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 4
				testList.Add(Nd4j.scalar(DataType.DOUBLE, i + 1))
			Next i

			Dim test As INDArray = Nd4j.create(testList, New Integer() {1, testList.Count})
			Dim expected As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5}, New Integer() {1, 5})
			assertEquals(expected, test)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeadAndTrail(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeadAndTrail(ByVal backend As Nd4jBackend)
			Dim fourD As INDArray = Nd4j.create(1, 2, 1, 1)
			assertEquals(2, fourD.length())
			For i As Integer = 0 To fourD.length() - 1
				assertEquals(0.0, fourD.getDouble(i), 1e-1)
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateLeadingAndTrailingOnes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCreateLeadingAndTrailingOnes(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(1, 10, 1, 1)
			arr.assign(1)
			arr.ToString()
	'        System.out.println(arr);
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace