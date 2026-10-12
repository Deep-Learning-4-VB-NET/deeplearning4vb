Imports System
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayStrings = org.nd4j.linalg.string.NDArrayStrings
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

Namespace org.nd4j.linalg.api.string


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_SERDE) public class TestFormatting extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestFormatting
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTwoByTwo(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTwoByTwo(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(2, 2, 2, 2)
			Console.WriteLine((New NDArrayStrings()).format(arr))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNd4jArrayString(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNd4jArrayString(ByVal backend As Nd4jBackend)

			Dim arr As INDArray = Nd4j.create(New Single(){1f, 20000000f, 40.838383f, 3f}, New Integer(){2, 2})

			Dim serializedData1 As String = (New NDArrayStrings(",", 3)).format(arr)
			log.info(vbLf & serializedData1)
			Dim expected1 As String = "[[1.000,40.838]," & vbLf & " [2e7,3.000]]"
			assertEquals(expected1.replaceAll(" ", ""), serializedData1.replaceAll(" ", ""))

			Dim serializedData2 As String = (New NDArrayStrings()).format(arr)
			log.info(vbLf & serializedData2)
			Dim expected2 As String = "[[1.0000,40.8384]," & vbLf & " [2e7,3.0000]]"
			assertEquals(expected2.replaceAll(" ", ""), serializedData2.replaceAll(" ", ""))

			Dim serializedData3 As String = (New NDArrayStrings(",", "000.00##E0")).format(arr)
			Dim expected3 As String = "[[100.00E-2,408.3838E-1]," & vbLf & " [200.00E5,300.00E-2]]"
			log.info(vbLf & serializedData3)
			assertEquals(expected3.replaceAll(" ", ""), serializedData3.replaceAll(" ", ""))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRange(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRange(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(New Double()(){
				New Double() {-1, 0, 1, 0},
				New Double() {-0.1, 0.1, -10, 10},
				New Double() {-1e-2, 1e-2, -1e2, 1e2},
				New Double() {-1e-3, 1e-3, -1e3, 1e3},
				New Double() {-1e-4, 1e-4, -1e4, 1e4},
				New Double() {-1e-8, 1e-8, -1e8, 1e8},
				New Double() {-1e-30, 1e-30, -1e30, 1e30},
				New Double() {-1e-50, 1e-50, -1e50, 1e50}
			})
			log.info(vbLf & arr.ToString())

			arr = Nd4j.create(New Double()(){
				New Double() {1.0001e4, 1e5},
				New Double() {0.11, 0.269}
			})
			arr = arr.reshape(ChrW(2), 2, 1)
			log.info(vbLf & arr.ToString())

		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace