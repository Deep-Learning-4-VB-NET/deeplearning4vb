Imports System
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
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.linalg.dimensionalityreduction

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag public class TestPCA extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestPCA
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFactorDims(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFactorDims(ByVal backend As Nd4jBackend)
			Dim m As Integer = 13
			Dim n As Integer = 4

			Dim f() As Double = {7, 1, 11, 11, 7, 11, 3, 1, 2, 21, 1, 11, 10, 26, 29, 56, 31, 52, 55, 71, 31, 54, 47, 40, 66, 68, 6, 15, 8, 8, 6, 9, 17, 22, 18, 4, 23, 9, 8, 60, 52, 20, 47, 33, 22, 6, 44, 22, 26, 34, 12, 12}

			Dim A As INDArray = Nd4j.create(f, New Integer() {m, n}, "f"c)

			Dim A1 As INDArray = A.dup("f"c)
			Dim Factor As INDArray = PCA.pca_factor(A1, 3, True)
			A1 = A.subiRowVector(A.mean(0))

			Dim Reduced As INDArray = A1.mmul(Factor)
			Dim Reconstructed As INDArray = Reduced.mmul(Factor.transpose())
			Dim Diff As INDArray = Reconstructed.sub(A1)
			Dim i As Integer = 0
			Do While i < m * n
				assertEquals(0.0, Diff.getDouble(i), 1.0,"Reconstructed matrix is very different from the original.")
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFactorSVDTransposed(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFactorSVDTransposed(ByVal backend As Nd4jBackend)
			Dim m As Integer = 4
			Dim n As Integer = 13

			Dim f() As Double = {7, 1, 11, 11, 7, 11, 3, 1, 2, 21, 1, 11, 10, 26, 29, 56, 31, 52, 55, 71, 31, 54, 47, 40, 66, 68, 6, 15, 8, 8, 6, 9, 17, 22, 18, 4, 23, 9, 8, 60, 52, 20, 47, 33, 22, 6, 44, 22, 26, 34, 12, 12}

			Dim A As INDArray = Nd4j.create(f, New Long() {m, n}, "f"c)

			Dim A1 As INDArray = A.dup("f"c)
			Dim factor As INDArray = PCA.pca_factor(A1, 3, True)
			A1 = A.subiRowVector(A.mean(0))

			Dim reduced As INDArray = A1.mmul(factor)
			Dim reconstructed As INDArray = reduced.mmul(factor.transpose())
			Dim diff As INDArray = reconstructed.sub(A1)
			Dim i As Integer = 0
			Do While i < m * n
				assertEquals(0.0, diff.getDouble(i), 1.0,"Reconstructed matrix is very different from the original.")
				i += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFactorVariance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFactorVariance(ByVal backend As Nd4jBackend)
			Dim m As Integer = 13
			Dim n As Integer = 4

			Dim f() As Double = {7, 1, 11, 11, 7, 11, 3, 1, 2, 21, 1, 11, 10, 26, 29, 56, 31, 52, 55, 71, 31, 54, 47, 40, 66, 68, 6, 15, 8, 8, 6, 9, 17, 22, 18, 4, 23, 9, 8, 60, 52, 20, 47, 33, 22, 6, 44, 22, 26, 34, 12, 12}

			Dim A As INDArray = Nd4j.create(f, New Integer() {m, n}, "f"c)

			Dim A1 As INDArray = A.dup("f"c)
			Dim Factor1 As INDArray = PCA.pca_factor(A1, 0.95, True)
			A1 = A.subiRowVector(A.mean(0))
			Dim Reduced1 As INDArray = A1.mmul(Factor1)
			Dim Reconstructed1 As INDArray = Reduced1.mmul(Factor1.transpose())
			Dim Diff1 As INDArray = Reconstructed1.sub(A1)
			Dim i As Integer = 0
			Do While i < m * n
				assertEquals(0.0, Diff1.getDouble(i), 0.1,"Reconstructed matrix is very different from the original.")
				i += 1
			Loop
			Dim A2 As INDArray = A.dup("f"c)
			Dim Factor2 As INDArray = PCA.pca_factor(A2, 0.50, True)
			assertTrue(Factor1.columns() > Factor2.columns(),"Variance differences should change factor sizes.")
		End Sub


		''' <summary>
		''' Test new PCA routines, added by Luke Czapla
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPCA(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPCA(ByVal backend As Nd4jBackend)
			Dim m As INDArray = Nd4j.randn(10000, 16)
			' 10000 random correlated samples of 16 features to analyze
			m.getColumn(0).muli(4.84)
			m.getColumn(1).muli(4.84)
			m.getColumn(2).muli(4.09)
			m.getColumn(1).addi(m.getColumn(2).div(2.0))
			m.getColumn(2).addi(34.286)
			m.getColumn(1).addi(m.getColumn(4))
			m.getColumn(4).subi(m.getColumn(5).div(2.0))
			m.getColumn(5).addi(3.4)
			m.getColumn(6).muli(6.0)
			m.getColumn(7).muli(0.2)
			m.getColumn(8).muli(2.0)
			m.getColumn(9).muli(6.0)
			m.getColumn(9).addi(m.getColumn(6).mul(1.0))
			m.getColumn(10).muli(0.2)
			m.getColumn(11).muli(2.0)
			m.getColumn(12).muli(0.2)
			m.getColumn(13).muli(4.0)
			m.getColumn(14).muli(3.2)
			m.getColumn(14).addi(m.getColumn(2).mul(1.0)).subi(m.getColumn(13).div(2.0))
			m.getColumn(15).muli(1.0)
			m.getColumn(13).subi(12.0)
			m.getColumn(15).addi(30.0)

			Dim myPCA As New PCA(m)
			Dim reduced70 As INDArray = myPCA.reducedBasis(0.70)
			Dim reduced99 As INDArray = myPCA.reducedBasis(0.99)
			assertTrue(reduced99.columns() > reduced70.columns(),"Major variance differences should change number of basis vectors")
			Dim reduced100 As INDArray = myPCA.reducedBasis(1.0)
			assertTrue(reduced100.columns() = m.columns(),"100% variance coverage should include all eigenvectors")
			Dim ns As New NDArrayStrings(5)
	'        System.out.println("Eigenvectors:\n" + ns.format(myPCA.getEigenvectors()));
	'        System.out.println("Eigenvalues:\n" + ns.format(myPCA.getEigenvalues()));
			Dim variance As Double = 0.0

			' sample 1000 of the randomly generated samples with the reduced basis set
			For i As Long = 0 To 999
				variance += myPCA.estimateVariance(m.getRow(i), reduced70.columns())
			Next i
			variance /= 1000.0
			Console.WriteLine("Fraction of variance using 70% variance with " & reduced70.columns() & " columns: " & variance)
			assertTrue(variance > 0.70,"Variance does not cover intended 70% variance")
			' create "dummy" data with the same exact trends
			Dim testSample As INDArray = myPCA.generateGaussianSamples(10000)
			Dim analyzePCA As New PCA(testSample)
			assertTrue(myPCA.Mean.equalsWithEps(analyzePCA.Mean, 0.2 * myPCA.Mean.columns()),"Means do not agree accurately enough")
			assertTrue(myPCA.CovarianceMatrix.equalsWithEps(analyzePCA.CovarianceMatrix, 1.0 * analyzePCA.CovarianceMatrix.length()),"Covariance is not reproduced accurately enough")
			assertTrue(myPCA.Eigenvalues.equalsWithEps(analyzePCA.Eigenvalues, 0.5 * myPCA.Eigenvalues.columns()),"Eigenvalues are not close enough")
			assertTrue(myPCA.Eigenvectors.equalsWithEps(analyzePCA.Eigenvectors, 0.1 * analyzePCA.Eigenvectors.length()),"Eigenvectors are not close enough")
	'        System.out.println("Original cov:\n" + ns.format(myPCA.getCovarianceMatrix()) + "\nDummy cov:\n"
	'                        + ns.format(analyzePCA.getCovarianceMatrix()));
			Dim testSample2 As INDArray = analyzePCA.convertBackToFeatures(analyzePCA.convertToComponents(testSample))
			assertTrue(testSample.equalsWithEps(testSample2, 1e-5 * testSample.length()),"Transformation does not work.")
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function

	End Class


End Namespace