Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
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

Namespace org.nd4j.linalg.ops.copy

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class CopyTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CopyTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCopy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCopy(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			Dim dup As INDArray = arr.dup()
			assertEquals(arr, dup)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDup(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDup(ByVal backend As Nd4jBackend)

			For x As Integer = 0 To 99
				Dim orig As INDArray = Nd4j.linspace(1, 4, 4)
				Dim dup As INDArray = orig.dup()
				assertEquals(orig, dup)

				Dim matrix As INDArray = Nd4j.create(New Single() {1, 2, 3, 4}, New Integer() {2, 2})
				Dim dup2 As INDArray = matrix.dup()
				assertEquals(matrix, dup2)

				Dim row1 As INDArray = matrix.getRow(1)
				Dim dupRow As INDArray = row1.dup()
				assertEquals(row1, dupRow)


				Dim columnSorted As INDArray = Nd4j.create(New Single() {2, 1, 4, 3}, New Integer() {2, 2})
				Dim dup3 As INDArray = columnSorted.dup()
				assertEquals(columnSorted, dup3)
			Next x
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace