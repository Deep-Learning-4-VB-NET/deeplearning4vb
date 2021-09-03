Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
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

Namespace org.nd4j.linalg.api.blas

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class LapackTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LapackTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testQRSquare(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testQRSquare(ByVal backend As Nd4jBackend)
			Dim A As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7, 8, 9})
			A = A.reshape("c"c, 3, 3)
			Dim O As INDArray = Nd4j.create(A.dataType(), A.shape())
			Nd4j.copy(A, O)
			Dim R As INDArray = Nd4j.create(A.dataType(), A.columns(), A.columns())

			Nd4j.BlasWrapper.lapack().geqrf(A, R)

			A.mmuli(R)
			O.subi(A)
			Dim db As DataBuffer = O.data()
			For i As Integer = 0 To db.length() - 1
				assertEquals(0, db.getFloat(i), 1e-5)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testQRRect(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testQRRect(ByVal backend As Nd4jBackend)
			Dim A As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12})
			A = A.reshape("f"c, 4, 3)
			Dim O As INDArray = Nd4j.create(A.dataType(), A.shape())
			Nd4j.copy(A, O)

			Dim R As INDArray = Nd4j.create(A.dataType(), A.columns(), A.columns())
			Nd4j.BlasWrapper.lapack().geqrf(A, R)

			A.mmuli(R)
			O.subi(A)
			Dim db As DataBuffer = O.data()
			For i As Integer = 0 To db.length() - 1
				assertEquals(0, db.getFloat(i), 1e-5)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCholeskyL(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCholeskyL(ByVal backend As Nd4jBackend)
			Dim A As INDArray = Nd4j.create(New Double() {2, -1, 1, -1, 2, -1, 1, -1, 2})
			A = A.reshape("c"c, 3, 3)
			Dim O As INDArray = Nd4j.create(A.dataType(), A.shape())
			Nd4j.copy(A, O)

			Nd4j.BlasWrapper.lapack().potrf(A, True)

			A.mmuli(A.transpose())
			O.subi(A)
			Dim db As DataBuffer = O.data()
			For i As Integer = 0 To db.length() - 1
				assertEquals(0, db.getFloat(i), 1e-5)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCholeskyU(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCholeskyU(ByVal backend As Nd4jBackend)
			Dim A As INDArray = Nd4j.create(New Double() {3, -1, 2, -1, 3, -1, 2, -1, 3})
			A = A.reshape("f"c, 3, 3)
			Dim O As INDArray = Nd4j.create(A.dataType(), A.shape())
			Nd4j.copy(A, O)

			Nd4j.BlasWrapper.lapack().potrf(A, False)
			A = A.transpose().mmul(A)
			O.subi(A)
			Dim db As DataBuffer = O.data()
			For i As Integer = 0 To db.length() - 1
				assertEquals(0, db.getFloat(i), 1e-5)
			Next i
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace