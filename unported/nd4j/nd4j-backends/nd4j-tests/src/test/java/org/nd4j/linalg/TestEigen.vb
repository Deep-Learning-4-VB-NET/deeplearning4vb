Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Eigen = org.nd4j.linalg.eigen.Eigen
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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

Namespace org.nd4j.linalg


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class TestEigen extends BaseNd4jTestWithBackends
	Public Class TestEigen
		Inherits BaseNd4jTestWithBackends

		Protected Friend initialType As DataType = Nd4j.dataType()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			Nd4j.DataType = initialType
		End Sub

		' test of functions added by Luke Czapla
		' Compares solution of A x = L x  to solution to A x = L B x when it is simple
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test2Syev(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test2Syev(ByVal backend As Nd4jBackend)
			For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(dt, dt)

				Dim matrix()() As Double = {
					New Double() {0.0427, -0.04, 0, 0, 0, 0},
					New Double() {-0.04, 0.0427, 0, 0, 0, 0},
					New Double() {0, 0.00, 0.0597, 0, 0, 0},
					New Double() {0, 0, 0, 50, 0, 0},
					New Double() {0, 0, 0, 0, 50, 0},
					New Double() {0, 0, 0, 0, 0, 50}
				}
				Dim m As INDArray = Nd4j.create(ArrayUtil.flattenDoubleArray(matrix), New Integer(){6, 6})
				Dim res As INDArray = Eigen.symmetricGeneralizedEigenvalues(m, True)

				Dim n As INDArray = Nd4j.create(ArrayUtil.flattenDoubleArray(matrix), New Integer(){6, 6})
				Dim res2 As INDArray = Eigen.symmetricGeneralizedEigenvalues(n, Nd4j.eye(6).mul(2.0).castTo(DataType.DOUBLE), True)

				For i As Integer = 0 To 5
					assertEquals(res.getDouble(i), 2 * res2.getDouble(i), 1e-6)
				Next i
			Next dt
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSyev(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSyev(ByVal backend As Nd4jBackend)
			For Each dt As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				'log.info("Datatype: {}", dt);
				Nd4j.setDefaultDataTypes(dt, dt)

				Dim A As INDArray = Nd4j.create(New Single()(){
					New Single() {1.96f, -6.49f, -0.47f, -7.20f, -0.65f},
					New Single() {-6.49f, 3.80f, -6.39f, 1.50f, -6.34f},
					New Single() {-0.47f, -6.39f, 4.17f, -1.51f, 2.67f},
					New Single() {-7.20f, 1.50f, -1.51f, 5.70f, 1.80f},
					New Single() {-0.65f, -6.34f, 2.67f, 1.80f, -7.10f}
				})

				Dim B As INDArray = A.dup()
				Dim e As INDArray = Eigen.symmetricGeneralizedEigenvalues(A)

				Dim i As Integer = 0
				Do While i < A.rows()
					Dim LHS As INDArray = B.mmul(A.slice(i, 1).reshape(ChrW(-1), 1))
					Dim RHS As INDArray = A.slice(i, 1).mul(e.getFloat(i))

					For j As Integer = 0 To LHS.length() - 1
						assertEquals(LHS.getFloat(j), RHS.getFloat(j), 0.001f)
					Next j
					i += 1
				Loop
			Next dt
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace