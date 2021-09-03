Imports System
Imports System.Collections.Generic
Imports Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix
Imports LUDecomposition = org.apache.commons.math3.linear.LUDecomposition
Imports MatrixUtils = org.apache.commons.math3.linear.MatrixUtils
Imports RealMatrix = org.apache.commons.math3.linear.RealMatrix
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CheckUtil = org.nd4j.linalg.checkutil.CheckUtil
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.primitives
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.linalg.inverse


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class TestInvertMatrices extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestInvertMatrices
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInverse(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInverse(ByVal backend As Nd4jBackend)
			Dim matrix As RealMatrix = New Array2DRowRealMatrix(New Double()() {
				New Double() {1, 2},
				New Double() {3, 4}
			})

			Dim inverse As RealMatrix = MatrixUtils.inverse(matrix)
			Dim arr As INDArray = InvertMatrix.invert(Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2), False)
			Dim i As Integer = 0
			Do While i < inverse.getRowDimension()
				Dim j As Integer = 0
				Do While j < inverse.getColumnDimension()
					assertEquals(arr.getDouble(i, j), inverse.getEntry(i, j), 1e-1)
					j += 1
				Loop
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInverseComparison(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInverseComparison(ByVal backend As Nd4jBackend)

			Dim list As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(10, 10, 12345, DataType.DOUBLE)

			For Each p As Pair(Of INDArray, String) In list
				Dim orig As INDArray = p.First
				orig.assign(Nd4j.rand(orig.shape()))
				Dim inverse As INDArray = InvertMatrix.invert(orig, False)
				Dim rm As RealMatrix = CheckUtil.convertToApacheMatrix(orig)
				Dim rmInverse As RealMatrix = (New LUDecomposition(rm)).getSolver().getInverse()

				Dim expected As INDArray = CheckUtil.convertFromApacheMatrix(rmInverse, orig.dataType())
				assertTrue(CheckUtil.checkEntries(expected, inverse, 1e-3, 1e-4),p.Second)
			Next p
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvalidMatrixInversion(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInvalidMatrixInversion(ByVal backend As Nd4jBackend)
			Try
				InvertMatrix.invert(Nd4j.create(5, 4), False)
				fail("No exception thrown for invalid input")
			Catch e As Exception
			End Try

			Try
				InvertMatrix.invert(Nd4j.create(5, 5, 5), False)
				fail("No exception thrown for invalid input")
			Catch e As Exception
			End Try

			Try
				InvertMatrix.invert(Nd4j.create(1, 5), False)
				fail("No exception thrown for invalid input")
			Catch e As Exception
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvertMatrixScalar()
		Public Overridable Sub testInvertMatrixScalar()
			Dim [in] As INDArray = Nd4j.valueArrayOf(New Integer(){1, 1}, 2)
			Dim out1 As INDArray = InvertMatrix.invert([in], False)
			assertEquals(Nd4j.valueArrayOf(New Integer(){1, 1}, 0.5), out1)
			assertEquals(Nd4j.valueArrayOf(New Integer(){1, 1}, 2), [in])

			Dim out2 As INDArray = InvertMatrix.invert([in], True)
			assertTrue(out2 Is [in])
			assertEquals(Nd4j.valueArrayOf(New Integer(){1, 1}, 0.5), out2)
		End Sub

		''' <summary>
		''' Example from: <a href="https://www.wolframalpha.com/input/?i=invert+matrix+((1,2),(3,4),(5,6))">here</a>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeftPseudoInvert(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeftPseudoInvert(ByVal backend As Nd4jBackend)
			Dim X As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2},
				New Double() {3, 4},
				New Double() {5, 6}
			})
			Dim expectedLeftInverse As INDArray = Nd4j.create(New Double()(){
				New Double() {-16, -4, 8},
				New Double() {13, 4, -5}
			}).mul(1 / 12R)
			Dim leftInverse As INDArray = InvertMatrix.pLeftInvert(X, False)
			assertEquals(expectedLeftInverse, leftInverse)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray identity3x3 = org.nd4j.linalg.factory.Nd4j.create(new double[][]{{1, 0, 0}, {0, 1, 0}, {0, 0, 1}});
			Dim identity3x3 As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 0, 0},
				New Double() {0, 1, 0},
				New Double() {0, 0, 1}
			})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray identity2x2 = org.nd4j.linalg.factory.Nd4j.create(new double[][]{{1, 0}, {0, 1}});
			Dim identity2x2 As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 0},
				New Double() {0, 1}
			})
			Const precision As Double = 1e-5

			' right inverse
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray rightInverseCheck = X.mmul(leftInverse);
			Dim rightInverseCheck As INDArray = X.mmul(leftInverse)
			' right inverse must not hold since X rows are not linear independent (x_3 + x_1 = 2*x_2)
			assertFalse(rightInverseCheck.equalsWithEps(identity3x3, precision))

			' left inverse must hold since X columns are linear independent
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray leftInverseCheck = leftInverse.mmul(X);
			Dim leftInverseCheck As INDArray = leftInverse.mmul(X)
			assertTrue(leftInverseCheck.equalsWithEps(identity2x2, precision))

			' general condition X = X * X^-1 * X
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray generalCond = X.mmul(leftInverse).mmul(X);
			Dim generalCond As INDArray = X.mmul(leftInverse).mmul(X)
			assertTrue(X.equalsWithEps(generalCond, precision))
			checkMoorePenroseConditions(X, leftInverse, precision)
		End Sub

		''' <summary>
		''' Check the Moore-Penrose conditions for pseudo-matrices.
		''' </summary>
		''' <param name="A"> Initial matrix </param>
		''' <param name="B"> Pseudo-Inverse of {@code A} </param>
		''' <param name="precision"> Precision when comparing matrix elements </param>
		Private Sub checkMoorePenroseConditions(ByVal A As INDArray, ByVal B As INDArray, ByVal precision As Double)
			' ABA=A (AB need not be the general identity matrix, but it maps all column vectors of A to themselves)
			assertTrue(A.equalsWithEps(A.mmul(B).mmul(A), precision))
			' BAB=B (B is a weak inverse for the multiplicative semigroup)
			assertTrue(B.equalsWithEps(B.mmul(A).mmul(B), precision))
			' (AB)^T=AB (AB is Hermitian)
			assertTrue((A.mmul(B)).transpose().equalsWithEps(A.mmul(B), precision))
			' (BA)^T=BA (BA is also Hermitian)
			assertTrue((B.mmul(A)).transpose().equalsWithEps(B.mmul(A), precision))
		End Sub

		''' <summary>
		''' Example from: <a href="https://www.wolframalpha.com/input/?i=invert+matrix+((1,2),(3,4),(5,6))^T">here</a>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRightPseudoInvert(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRightPseudoInvert(ByVal backend As Nd4jBackend)
			Dim X As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2},
				New Double() {3, 4},
				New Double() {5, 6}
			}).transpose()
			Dim expectedRightInverse As INDArray = Nd4j.create(New Double()(){
				New Double() {-16, 13},
				New Double() {-4, 4},
				New Double() {8, -5}
			}).mul(1 / 12R)
			Dim rightInverse As INDArray = InvertMatrix.pRightInvert(X, False)
			assertEquals(expectedRightInverse, rightInverse)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray identity3x3 = org.nd4j.linalg.factory.Nd4j.create(new double[][]{{1, 0, 0}, {0, 1, 0}, {0, 0, 1}});
			Dim identity3x3 As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 0, 0},
				New Double() {0, 1, 0},
				New Double() {0, 0, 1}
			})
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray identity2x2 = org.nd4j.linalg.factory.Nd4j.create(new double[][]{{1, 0}, {0, 1}});
			Dim identity2x2 As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 0},
				New Double() {0, 1}
			})
			Const precision As Double = 1e-5

			' left inverse
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray leftInverseCheck = rightInverse.mmul(X);
			Dim leftInverseCheck As INDArray = rightInverse.mmul(X)
			' left inverse must not hold since X columns are not linear independent (x_3 + x_1 = 2*x_2)
			assertFalse(leftInverseCheck.equalsWithEps(identity3x3, precision))

			' left inverse must hold since X rows are linear independent
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray rightInverseCheck = X.mmul(rightInverse);
			Dim rightInverseCheck As INDArray = X.mmul(rightInverse)
			assertTrue(rightInverseCheck.equalsWithEps(identity2x2, precision))

			' general condition X = X * X^-1 * X
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray generalCond = X.mmul(rightInverse).mmul(X);
			Dim generalCond As INDArray = X.mmul(rightInverse).mmul(X)
			assertTrue(X.equalsWithEps(generalCond, precision))
			checkMoorePenroseConditions(X, rightInverse, precision)
		End Sub

		''' <summary>
		''' Try to compute the right pseudo inverse of a matrix without full row rank (x1 = 2*x2)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRightPseudoInvertWithNonFullRowRank(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRightPseudoInvertWithNonFullRowRank(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim X As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2},
				New Double() {3, 6},
				New Double() {5, 10}
			}).transpose()
			Dim rightInverse As INDArray = InvertMatrix.pRightInvert(X, False)
			End Sub)

		End Sub

		''' <summary>
		''' Try to compute the left pseudo inverse of a matrix without full column rank (x1 = 2*x2)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeftPseudoInvertWithNonFullColumnRank(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeftPseudoInvertWithNonFullColumnRank(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim X As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 2},
				New Double() {3, 6},
				New Double() {5, 10}
			})
			Dim leftInverse As INDArray = InvertMatrix.pLeftInvert(X, False)
			End Sub)

		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace