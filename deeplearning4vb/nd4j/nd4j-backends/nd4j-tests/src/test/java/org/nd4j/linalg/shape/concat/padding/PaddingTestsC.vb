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
Imports Convolution = org.nd4j.linalg.convolution.Convolution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.shape.concat.padding

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class PaddingTestsC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class PaddingTestsC
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPrepend(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPrepend(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 1, 1, 1, 2},
				New Double() {1, 1, 1, 3, 4}
			})

			Dim prepend As INDArray = Nd4j.prepend(linspace, 3, 1.0, -1)
			assertEquals(assertion, prepend)


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPaddingOneThrougFour(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPaddingOneThrougFour(ByVal backend As Nd4jBackend)
			Dim ph As Integer = 0
			Dim pw As Integer = 0
			Dim sy As Integer = 2
			Dim sx As Integer = 2
			Dim ret As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4}, New Integer() {1, 1, 8, 8})
			Dim padded As INDArray = Nd4j.pad(ret, New Integer()() {
				New Integer() {0, 0},
				New Integer() {0, 0},
				New Integer() {ph, ph + sy - 1},
				New Integer() {pw, pw + sx - 1}
			})

			Dim assertion As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, New Integer() {1, 1, 9, 9})
			assertArrayEquals(assertion.shape(), padded.shape())
			assertEquals(assertion, padded)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAppend2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAppend2(ByVal backend As Nd4jBackend)
			Dim ret As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4}, New Integer() {1, 1, 8, 8})

			Dim appendAssertion As INDArray = Nd4j.create(New Double() {1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0}, New Integer() {1, 1, 9, 8})

			Dim appended As INDArray = Nd4j.append(ret, 1, 0, 2)
			assertArrayEquals(appendAssertion.shape(), appended.shape())
			assertEquals(appendAssertion, appended)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPaddingTensor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPaddingTensor(ByVal backend As Nd4jBackend)
			',1,1,1,1,2,2,0
			Dim kh As Integer = 1, kw As Integer = 1, sy As Integer = 1, sx As Integer = 1, ph As Integer = 2, pw As Integer = 2
			Dim linspaced As INDArray = Nd4j.linspace(1, 16, 16, DataType.DOUBLE).reshape(ChrW(2), 2, 2, 2)
			Dim n As val = linspaced.size(0)
			Dim c As val = linspaced.size(1)
			Dim h As val = linspaced.size(2)
			Dim w As val = linspaced.size(3)

			Dim outWidth As Long = Convolution.outSize(h, kh, sy, ph, 1, True)
			Dim outHeight As Long = Convolution.outSize(w, kw, sx, pw, 1, True)
			Dim padded As INDArray = Nd4j.pad(linspaced, New Integer()() {
				New Integer() {0, 0},
				New Integer() {0, 0},
				New Integer() {ph, ph + sy - 1},
				New Integer() {pw, pw + sx - 1}
			})
	'        System.out.println(padded);
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAppend(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAppend(ByVal backend As Nd4jBackend)
			Dim linspace As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim otherAppend As INDArray = Nd4j.append(linspace, 3, 1.0, -1)
			Dim assertion As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2, 1, 1, 1},
				New Double() {3, 4, 1, 1, 1}
			})

			assertEquals(assertion, otherAppend)
		End Sub
	End Class

End Namespace