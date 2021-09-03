Imports System.Collections.Generic
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
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

Namespace org.nd4j.linalg



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class Nd4jTestsComparisonC extends BaseNd4jTestWithBackends
	Public Class Nd4jTestsComparisonC
		Inherits BaseNd4jTestWithBackends

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(Nd4jTestsComparisonC))

		Public Const SEED As Integer = 123

		Friend initialType As DataType = Nd4j.dataType()



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub before()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub after()
			DataTypeUtil.setDTypeForContext(initialType)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemmWithOpsCommonsMath(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemmWithOpsCommonsMath(ByVal backend As Nd4jBackend)
			Dim first As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(3, 5, SEED, DataType.DOUBLE)
			Dim firstT As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(5, 3, SEED, DataType.DOUBLE)
			Dim second As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(5, 4, SEED, DataType.DOUBLE)
			Dim secondT As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape(4, 5, SEED, DataType.DOUBLE)
			Dim alpha() As Double = {1.0, -0.5, 2.5}
			Dim beta() As Double = {0.0, -0.25, 1.5}
			Dim cOrig As INDArray = Nd4j.linspace(1, 12, 12).reshape(ChrW(3), 4)

			For i As Integer = 0 To first.Count - 1
				For j As Integer = 0 To second.Count - 1
					For k As Integer = 0 To alpha.Length - 1
						For m As Integer = 0 To beta.Length - 1
							Dim cff As INDArray = Nd4j.create(cOrig.shape(), "f"c)
							cff.assign(cOrig)
							Dim cft As INDArray = Nd4j.create(cOrig.shape(), "f"c)
							cft.assign(cOrig)
							Dim ctf As INDArray = Nd4j.create(cOrig.shape(), "f"c)
							ctf.assign(cOrig)
							Dim ctt As INDArray = Nd4j.create(cOrig.shape(), "f"c)
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
							'System.out.println((String.format("Running iteration %d %d %d %d", i, j, k, m)));
							assertTrue(CheckUtil.checkGemm(p1.First, p2.First, cff, False, False, a, b, 1e-4, 1e-6),errorMsgff)
							assertTrue(CheckUtil.checkGemm(p1.First, p2T.First, cft, False, True, a, b, 1e-4, 1e-6),errorMsgft)
							assertTrue(CheckUtil.checkGemm(p1T.First, p2.First, ctf, True, False, a, b, 1e-4, 1e-6),errorMsgtf)
							assertTrue(CheckUtil.checkGemm(p1T.First, p2T.First, ctt, True, True, a, b, 1e-4, 1e-6),errorMsgtt)

							'Also: Confirm that if the C array is uninitialized and beta is 0.0, we don't have issues like 0*NaN = NaN
							If b = 0.0 Then
								cff.assign(Double.NaN)
								cft.assign(Double.NaN)
								ctf.assign(Double.NaN)
								ctt.assign(Double.NaN)

								assertTrue(CheckUtil.checkGemm(p1.First, p2.First, cff, False, False, a, b, 1e-4, 1e-6),errorMsgff)
								assertTrue(CheckUtil.checkGemm(p1.First, p2T.First, cft, False, True, a, b, 1e-4, 1e-6),errorMsgft)
								assertTrue(CheckUtil.checkGemm(p1T.First, p2.First, ctf, True, False, a, b, 1e-4, 1e-6),errorMsgtf)
								assertTrue(CheckUtil.checkGemm(p1T.First, p2T.First, ctt, True, True, a, b, 1e-4, 1e-6),errorMsgtt)
							End If

						Next m
					Next k
				Next j
			Next i
		End Sub


		Private Shared Function getTestWithOpsErrorMsg(ByVal i As Integer, ByVal j As Integer, ByVal op As String, ByVal first As Pair(Of INDArray, String), ByVal second As Pair(Of INDArray, String)) As String
			Return i & "," & j & " - " & first.Second & "." & op & "(" & second.Second & ")"
		End Function

		Private Shared Function getGemmErrorMsg(ByVal i As Integer, ByVal j As Integer, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal alpha As Double, ByVal beta As Double, ByVal first As Pair(Of INDArray, String), ByVal second As Pair(Of INDArray, String)) As String
			Return i & "," & j & " - gemm(tA=" & transposeA & ",tB=" & transposeB & ",alpha=" & alpha & ",beta=" & beta & "). A=" & first.Second & ", B=" & second.Second
		End Function
	End Class

End Namespace