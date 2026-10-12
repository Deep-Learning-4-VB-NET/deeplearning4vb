Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
Namespace org.nd4j.linalg.generated

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.SAMEDIFF) @NativeTag public class SDLinalgTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SDLinalgTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		Private sameDiff As SameDiff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			sameDiff = SameDiff.create()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCholesky(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCholesky(ByVal backend As Nd4jBackend)
			Dim input As INDArray = Nd4j.createFromArray(New Single(){ 10.0f, 14.0f, 14.0f, 20.0f, 74.0f, 86.0f, 86.0f, 100.0f }).reshape(ChrW(2), 2, 2)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 3.1622777f, 0.0f, 4.427189f, 0.6324552f, 8.602325f, 0.0f, 9.997296f, 0.23252854f }).reshape(ChrW(2), 2, 2)

			Dim sdinput As SDVariable = sameDiff.var(input)
			Dim [out] As SDVariable = sameDiff.linalg().cholesky(sdinput)
			assertEquals(expected, [out].eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLstsq()
		Public Overridable Sub testLstsq()
			Dim a As INDArray = Nd4j.createFromArray(New Single(){ 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f }).reshape(ChrW(2), 2, 2)

			Dim b As INDArray = Nd4j.createFromArray(New Single(){ 3.0f, 7.0f, 11.0f, 15.0f }).reshape(ChrW(2), 2, 1)

			Dim expected As INDArray = Nd4j.createFromArray(New Single(){ 0.831169367f, 1.090908766f, 0.920544624f, 1.063016534f }).reshape(ChrW(2), 2, 1)

			Dim sda As SDVariable = sameDiff.var(a)
			Dim sdb As SDVariable = sameDiff.var(b)

			Dim res As SDVariable = sameDiff.linalg().lstsq(sda,sdb,0.5,True)
			assertEquals(expected, res.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLu()
		Public Overridable Sub testLu()
			Dim sdInput As SDVariable = sameDiff.var(Nd4j.createFromArray(New Double(){ 1.0, 2.0, 3.0, 0.0, 2.0, 3.0, 0.0, 0.0, 7.0 }).reshape(ChrW(3), 3))

			Dim expected As INDArray = Nd4j.createFromArray(New Double(){ 1.0, 2.0, 3.0, 0.0, 2.0, 3.0, 0.0, 0.0, 7 }).reshape(ChrW(3), 3)

			Dim [out] As SDVariable = sameDiff.linalg().lu("lu", sdInput)
			assertEquals(expected, [out].eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrixBandPart()
		Public Overridable Sub testMatrixBandPart()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 2*3*3).reshape(ChrW(2), 3, 3)
			Dim expected As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 2*3*3).reshape(ChrW(2), 3, 3)

			Dim sdx As SDVariable = sameDiff.var(x)
			Dim res() As SDVariable = sameDiff.linalg().matrixBandPart(sdx, 1, 1)
			assertArrayEquals(x.shape(), res(0).eval().shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testQr()
		Public Overridable Sub testQr()
			Dim input As INDArray = Nd4j.createFromArray(New Double(){ 12.0, -51.0, 4.0, 6.0, 167.0, -68.0, -4.0, 24.0, -41.0, -1.0, 1.0, 0.0, 2.0, 0.0, 3.0 }).reshape(ChrW(5), 3)

			Dim expectedQ As INDArray = Nd4j.createFromArray(New Double(){ 0.8464147390303179, -0.3912908119746455, 0.34312406418022884, 0.42320736951515897, 0.9040872694197354, -0.02927016186366648, -0.2821382463434393, 0.17042054976392634, 0.9328559865183932, -0.07053456158585983, 0.01404065236547358, -0.00109937201747271, 0.14106912317171966, -0.01665551070074392, -0.10577161246232346 }).reshape(ChrW(5), 3)

			Dim expectedR As INDArray = Nd4j.createFromArray(New Double(){ 14.177446878757824, 20.666626544656932, -13.401566701313369, -0.0000000000000006, 175.04253925050244, -70.0803066408638, 0.00000000000000017, -0.00000000000000881, -35.20154302119086 }).reshape(ChrW(3), 3)

			Dim sdInput As SDVariable = sameDiff.var(input)
			Dim res() As SDVariable = sameDiff.linalg().qr(sdInput)

			Dim mmulResult As SDVariable = sameDiff.mmul(res(0), res(1))

			assertEquals(input, mmulResult.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSolve()
		Public Overridable Sub testSolve()
			Dim a As INDArray = Nd4j.createFromArray(New Single() { 2.0f, -1.0f, -2.0f, -4.0f, 6.0f, 3.0f, -4.0f, -2.0f, 8.0f }).reshape(ChrW(3), 3)

			Dim b As INDArray = Nd4j.createFromArray(New Single() { 2.0f, 4.0f, 3.0f }).reshape(ChrW(3), 1)

			Dim expected As INDArray = Nd4j.createFromArray(New Single() { 7.625f, 3.25f, 5.0f }).reshape(ChrW(3), 1)

			Dim sda As SDVariable = sameDiff.var(a)
			Dim sdb As SDVariable = sameDiff.var(b)

			Dim res As SDVariable = sameDiff.linalg().solve(sda, sdb)
			assertEquals(expected, res.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTriangularSolve()
		Public Overridable Sub testTriangularSolve()
			Dim a As INDArray = Nd4j.createFromArray(New Single() { 0.7788f, 0.8012f, 0.7244f, 0.2309f, 0.7271f, 0.1804f, 0.5056f, 0.8925f, 0.5461f }).reshape(ChrW(3), 3)

			Dim b As INDArray = Nd4j.createFromArray(New Single() { 0.7717f, 0.9281f, 0.9846f, 0.4838f, 0.6433f, 0.6041f, 0.6501f, 0.7612f, 0.7605f }).reshape(ChrW(3), 3)

			Dim expected As INDArray = Nd4j.createFromArray(New Single() { 0.99088347f, 1.1917052f, 1.2642528f, 0.35071516f, 0.50630623f, 0.42935497f, -0.30013534f, -0.53690606f, -0.47959247f }).reshape(ChrW(3), 3)

			Dim sda As SDVariable = sameDiff.var(a)
			Dim sdb As SDVariable = sameDiff.var(b)

			Dim res As SDVariable = sameDiff.linalg().triangularSolve(sda, sdb, True, False)
			assertEquals(expected, res.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCross()
		Public Overridable Sub testCross()
			Dim a As INDArray = Nd4j.createFromArray(New Double(){1, 2, 3})
			Dim b As INDArray = Nd4j.createFromArray(New Double(){6, 7, 8})
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){-5, 10, -5})

			Dim sda As SDVariable = sameDiff.var(a)
			Dim sdb As SDVariable = sameDiff.var(b)

			Dim res As SDVariable = sameDiff.linalg().cross(sda, sdb)
			assertEquals(expected, res.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDiag()
		Public Overridable Sub testDiag()
			Dim x As INDArray = Nd4j.createFromArray(New Double(){1, 2})
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){1, 0, 0, 2}).reshape(ChrW(2), 2)

			Dim sdx As SDVariable = sameDiff.var(x)

			Dim res As SDVariable = sameDiff.linalg().diag(sdx)
			assertEquals(expected, res.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDiagPart()
		Public Overridable Sub testDiagPart()
			Dim x As INDArray = Nd4j.linspace(DataType.DOUBLE, 1.0, 1.0, 4).reshape(ChrW(2), 2)
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){1, 4})

			Dim sdx As SDVariable = sameDiff.var(x)

			Dim res As SDVariable = sameDiff.linalg().diag_part(sdx)
			assertEquals(expected, res.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogdet()
		Public Overridable Sub testLogdet()
			Dim x As INDArray = Nd4j.createFromArray(New Double(){4, 12, -16, 12, 37, -43, -16, -43, 98, 4, 1.2, -1.6, 1.2, 3.7, -4.3, -1.6, -4.3, 9.8}).reshape(ChrW(2), 3, 3)
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){3.5835189, 4.159008})

			Dim sdx As SDVariable = sameDiff.var(x)

			Dim res As SDVariable = sameDiff.linalg().logdet(sdx)
			assertEquals(expected, res.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSvd()
		Public Overridable Sub testSvd()
			Dim x As INDArray = Nd4j.createFromArray(New Double(){0.7787856f, 0.80119777f, 0.72437465f, 0.23089433f, 0.72714126f, 0.18039072f, 0.50563407f, 0.89252293f, 0.5461209f}).reshape(ChrW(3), 3)
			Dim expected As INDArray = Nd4j.createFromArray(New Double(){1.8967269987492157, 0.3709665595850617, 0.05524869852188223})

			Dim sdx As SDVariable = sameDiff.var(x)
			Dim res As SDVariable = sameDiff.linalg().svd(sdx, False, False)
			assertEquals(expected, res.eval())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogdetName()
		Public Overridable Sub testLogdetName()
			Dim x As INDArray = Nd4j.createFromArray(New Double(){4, 12, -16, 12, 37, -43, -16, -43, 98, 4, 1.2, -1.6, 1.2, 3.7, -4.3, -1.6, -4.3, 9.8}).reshape(ChrW(2), 3, 3)

			Dim sdx As SDVariable = sameDiff.var(x)

			Dim res As SDVariable = sameDiff.linalg().logdet("logdet", sdx)
			assertEquals("logdet", res.name())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testQrNames()
		Public Overridable Sub testQrNames()
			Dim input As INDArray = Nd4j.createFromArray(New Double(){ 12.0, -51.0, 4.0, 6.0, 167.0, -68.0, -4.0, 24.0, -41.0, -1.0, 1.0, 0.0, 2.0, 0.0, 3.0 }).reshape(ChrW(5), 3)

			Dim sdInput As SDVariable = sameDiff.var(input)
			Dim res() As SDVariable = sameDiff.linalg().qr(New String(){"ret0", "ret1"}, sdInput)

			assertEquals("ret0", res(0).name())
			assertEquals("ret1", res(1).name())
		End Sub
	End Class

End Namespace