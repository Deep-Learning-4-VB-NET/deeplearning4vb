Imports System
Imports System.Collections.Generic
Imports BlockRealMatrix = org.apache.commons.math3.linear.BlockRealMatrix
Imports RealMatrix = org.apache.commons.math3.linear.RealMatrix
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CheckUtil = org.nd4j.linalg.checkutil.CheckUtil
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.primitives
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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

Namespace org.nd4j.linalg


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class Nd4jTestsComparisonFortran extends BaseNd4jTestWithBackends
	Public Class Nd4jTestsComparisonFortran
		Inherits BaseNd4jTestWithBackends

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(Nd4jTestsComparisonFortran))

		Public Const SEED As Integer = 123

		Friend initialType As DataType = Nd4j.dataType()



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub before()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
			Nd4j.Random.setSeed(SEED)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub after()
			DataTypeUtil.setDTypeForContext(initialType)
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCrash(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCrash(ByVal backend As Nd4jBackend)
			Dim array3d As INDArray = Nd4j.ones(1, 10, 10)
			Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(array3d, 0)
			Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(array3d, 1)

			Dim array4d As INDArray = Nd4j.ones(1, 10, 10, 10)
			Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(array4d, 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMmulWithOpsCommonsMath(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMmulWithOpsCommonsMath(ByVal backend As Nd4jBackend)
			Dim first As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 5, SEED, DataType.DOUBLE)
			Dim second As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(5, 4, SEED, DataType.DOUBLE)

			For i As Integer = 0 To first.Count - 1
				For j As Integer = 0 To second.Count - 1
					Dim p1 As Pair(Of INDArray, String) = first(i)
					Dim p2 As Pair(Of INDArray, String) = second(j)
					Dim errorMsg As String = getTestWithOpsErrorMsg(i, j, "mmul", p1, p2)
					assertTrue(CheckUtil.checkMmul(p1.First, p2.First, 1e-4, 1e-6),errorMsg)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemmWithOpsCommonsMath(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemmWithOpsCommonsMath(ByVal backend As Nd4jBackend)
			Dim first As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 5, SEED, DataType.DOUBLE)
			Dim firstT As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(5, 3, SEED, DataType.DOUBLE)
			Dim second As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(5, 4, SEED, DataType.DOUBLE)
			Dim secondT As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(4, 5, SEED, DataType.DOUBLE)
			Dim alpha() As Double = {1.0, -0.5, 2.5}
			Dim beta() As Double = {0.0, -0.25, 1.5}
			Dim cOrig As INDArray = Nd4j.create(New Integer() {3, 4})
			Dim r As New Random(12345)
			Dim i As Integer = 0
			Do While i < cOrig.size(0)
				Dim j As Integer = 0
				Do While j < cOrig.size(1)
					cOrig.putScalar(New Integer() {i, j}, r.NextDouble())
					j += 1
				Loop
				i += 1
			Loop

			For i As Integer = 0 To first.Count - 1
				For j As Integer = 0 To second.Count - 1
					For k As Integer = 0 To alpha.Length - 1
						For m As Integer = 0 To beta.Length - 1
							'System.out.println((String.format("Running iteration %d %d %d %d", i, j, k, m)));

							Dim cff As INDArray = Nd4j.create(cOrig.shape(), "f"c).castTo(DataType.DOUBLE)
							cff.assign(cOrig)
							Dim cft As INDArray = Nd4j.create(cOrig.shape(), "f"c).castTo(DataType.DOUBLE)
							cft.assign(cOrig)
							Dim ctf As INDArray = Nd4j.create(cOrig.shape(), "f"c).castTo(DataType.DOUBLE)
							ctf.assign(cOrig)
							Dim ctt As INDArray = Nd4j.create(cOrig.shape(), "f"c).castTo(DataType.DOUBLE)
							ctt.assign(cOrig)

							Dim a As Double = alpha(k)
							Dim b As Double = beta(k)
							Dim p1 As Pair(Of INDArray, String) = first(i)
							Dim p1T As Pair(Of INDArray, String) = firstT(i)
							Dim p2 As Pair(Of INDArray, String) = second(j)
							Dim p2T As Pair(Of INDArray, String) = secondT(j)
							Dim errorMsgff As String = getGemmErrorMsg(i, j, False, False, a, b, p1, p2)
							Dim errorMsgft As String = getGemmErrorMsg(i, j, False, True, a, b, p1, p2T)
							Dim errorMsgtf As String = getGemmErrorMsg(i, j, True, False, a, b, p1T, p2)
							Dim errorMsgtt As String = getGemmErrorMsg(i, j, True, True, a, b, p1T, p2T)

							assertTrue(CheckUtil.checkGemm(p1.First, p2.First, cff, False, False, a, b, 1e-4, 1e-6),errorMsgff)
							assertTrue(CheckUtil.checkGemm(p1.First, p2T.First, cft, False, True, a, b, 1e-4, 1e-6),errorMsgft)
							assertTrue(CheckUtil.checkGemm(p1T.First, p2.First, ctf, True, False, a, b, 1e-4, 1e-6),errorMsgtf)
							assertTrue(CheckUtil.checkGemm(p1T.First, p2T.First, ctt, True, True, a, b, 1e-4, 1e-6),errorMsgtt)
						Next m
					Next k
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemvApacheCommons(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemvApacheCommons(ByVal backend As Nd4jBackend)

			Dim rowsArr() As Integer = {4, 4, 4, 8, 8, 8}
			Dim colsArr() As Integer = {2, 1, 10, 2, 1, 10}

			For x As Integer = 0 To rowsArr.Length - 1
				Dim rows As Integer = rowsArr(x)
				Dim cols As Integer = colsArr(x)

				Dim matrices As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(rows, cols, 12345, DataType.DOUBLE)
				Dim vectors As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(cols, 1, 12345, DataType.DOUBLE)

				For i As Integer = 0 To matrices.Count - 1
					For j As Integer = 0 To vectors.Count - 1

						Dim p1 As Pair(Of INDArray, String) = matrices(i)
						Dim p2 As Pair(Of INDArray, String) = vectors(j)
						Dim errorMsg As String = getTestWithOpsErrorMsg(i, j, "mmul", p1, p2)

						Dim m As INDArray = p1.First
						Dim v As INDArray = p2.First

						Dim rm As RealMatrix = New BlockRealMatrix(m.rows(), m.columns())
						Dim r As Integer = 0
						Do While r < m.rows()
							Dim c As Integer = 0
							Do While c < m.columns()
								Dim d As Double = m.getDouble(r, c)
								rm.setEntry(r, c, d)
								c += 1
							Loop
							r += 1
						Loop

						Dim rv As RealMatrix = New BlockRealMatrix(cols, 1)
						r = 0
						Do While r < v.rows()
							Dim d As Double = v.getDouble(r, 0)
							rv.setEntry(r, 0, d)
							r += 1
						Loop

						Dim gemv As INDArray = m.mmul(v)
						Dim gemv2 As RealMatrix = rm.multiply(rv)

						assertArrayEquals(New Long() {rows, 1}, gemv.shape())
						assertArrayEquals(New Integer() {rows, 1}, New Integer() {gemv2.getRowDimension(), gemv2.getColumnDimension()})

						'Check entries:
						For r As Integer = 0 To rows - 1
							Dim exp As Double = gemv2.getEntry(r, 0)
							Dim act As Double = gemv.getDouble(r, 0)
							assertEquals(exp, act, 1e-5,errorMsg)
						Next r
					Next j
				Next i
			Next x
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddSubtractWithOpsCommonsMath(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddSubtractWithOpsCommonsMath(ByVal backend As Nd4jBackend)
			Dim first As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 5, SEED, DataType.DOUBLE)
			Dim second As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 5, SEED, DataType.DOUBLE)
			For i As Integer = 0 To first.Count - 1
				For j As Integer = 0 To second.Count - 1
					Dim p1 As Pair(Of INDArray, String) = first(i)
					Dim p2 As Pair(Of INDArray, String) = second(j)
					Dim errorMsg1 As String = getTestWithOpsErrorMsg(i, j, "add", p1, p2)
					Dim errorMsg2 As String = getTestWithOpsErrorMsg(i, j, "sub", p1, p2)
					Dim addFail As Boolean = CheckUtil.checkAdd(p1.First, p2.First, 1e-4, 1e-6)
					assertTrue(addFail,errorMsg1)
					Dim subFail As Boolean = CheckUtil.checkSubtract(p1.First, p2.First, 1e-4, 1e-6)
					assertTrue(subFail,errorMsg2)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMulDivOnCheckUtilMatrices(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMulDivOnCheckUtilMatrices(ByVal backend As Nd4jBackend)
			Dim first As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 5, SEED, DataType.DOUBLE)
			Dim second As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 5, SEED, DataType.DOUBLE)
			For i As Integer = 0 To first.Count - 1
				For j As Integer = 0 To second.Count - 1
					Dim p1 As Pair(Of INDArray, String) = first(i)
					Dim p2 As Pair(Of INDArray, String) = second(j)
					Dim errorMsg1 As String = getTestWithOpsErrorMsg(i, j, "mul", p1, p2)
					Dim errorMsg2 As String = getTestWithOpsErrorMsg(i, j, "div", p1, p2)
					assertTrue(CheckUtil.checkMulManually(p1.First, p2.First, 1e-4, 1e-6),errorMsg1)
					assertTrue(CheckUtil.checkDivManually(p1.First, p2.First, 1e-4, 1e-6),errorMsg2)
				Next j
			Next i
		End Sub

		Private Shared Function getTestWithOpsErrorMsg(ByVal i As Integer, ByVal j As Integer, ByVal op As String, ByVal first As Pair(Of INDArray, String), ByVal second As Pair(Of INDArray, String)) As String
			Return i & "," & j & " - " & first.Second & "." & op & "(" & second.Second & ")"
		End Function

		Private Shared Function getGemmErrorMsg(ByVal i As Integer, ByVal j As Integer, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal alpha As Double, ByVal beta As Double, ByVal first As Pair(Of INDArray, String), ByVal second As Pair(Of INDArray, String)) As String
			Return i & "," & j & " - gemm(tA=" & transposeA & ",tB= " & transposeB & ",alpha=" & alpha & ",beta= " & beta & "). A=" & first.Second & ", B=" & second.Second
		End Function
	End Class

End Namespace