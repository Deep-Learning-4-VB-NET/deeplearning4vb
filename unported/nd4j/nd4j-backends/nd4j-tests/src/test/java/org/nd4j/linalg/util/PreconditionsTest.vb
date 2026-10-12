Imports System
Imports Microsoft.VisualBasic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.fail

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

Namespace org.nd4j.linalg.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class PreconditionsTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class PreconditionsTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test(ByVal backend As Nd4jBackend)

			Dim arr As INDArray = Nd4j.linspace(1,60,60).reshape("c"c,3,4,5)

			Try
				Preconditions.checkArgument(False, "Shape is: %ndShape with rank %ndRank", arr, arr)
				fail("Expected exception")
			Catch t As Exception
				assertEquals("Shape is: [3, 4, 5] with rank 3", t.getMessage())
			End Try

			Try

				Preconditions.checkArgument(False, "Stride is: %ndStride with shape info %ndSInfo", arr, arr)
				fail("Expected exception")
			Catch t As Exception
				Dim si As String = arr.shapeInfoToString().replaceAll(vbLf,"")
				assertEquals("Stride is: " & Arrays.toString(arr.stride()) & " with shape info " & si, t.getMessage())
			End Try

			Dim asVector As INDArray = arr.reshape(ChrW(arr.length()))
			Try
				Preconditions.checkArgument(False, "First 10: %nd10", arr)
				fail("Expected exception")
			Catch t As Exception
				Dim get10 As INDArray = asVector.get(NDArrayIndex.interval(0, 10))
				assertEquals("First 10: " & get10, t.getMessage())
			End Try

		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

	End Class

End Namespace