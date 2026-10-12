Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class Nd4jTestsF extends BaseNd4jTestWithBackends
	Public Class Nd4jTestsF
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConcat3D_Vstack_F(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConcat3D_Vstack_F(ByVal backend As Nd4jBackend)
			'Nd4j.getExecutioner().enableVerboseMode(true);
			'Nd4j.getExecutioner().enableDebugMode(true);

			Dim shape() As Integer = {1, 1000, 150}
			'INDArray cOrder =  Nd4j.rand(shape,123);


			Dim cArrays As IList(Of INDArray) = New List(Of INDArray)()
			Dim fArrays As IList(Of INDArray) = New List(Of INDArray)()

			For e As Integer = 0 To 31
				cArrays.Add(Nd4j.create(shape, "f"c).assign(e))
				'            fArrays.add(cOrder.dup('f'));
			Next e

			Nd4j.Executioner.commit()

			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim res As INDArray = Nd4j.vstack(cArrays)
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Time spent: {} ms", time2 - time1)

			For e As Integer = 0 To 31
				Dim tad As INDArray = res.tensorAlongDimension(e, 1, 2)
				assertEquals(CDbl(e), tad.meanNumber().doubleValue(), 1e-5)
			Next e
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSlice_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSlice_1(ByVal backend As Nd4jBackend)
			Dim arr As val = Nd4j.linspace(1,4, 4, DataType.DOUBLE).reshape(ChrW(2), 2, 1)
			Dim exp0 As val = Nd4j.create(New Double(){1, 3}, New Integer() {2, 1})
			Dim exp1 As val = Nd4j.create(New Double(){2, 4}, New Integer() {2, 1})

			Dim slice0 As val = arr.slice(0).dup("f"c)
			assertEquals(exp0, slice0)
			assertEquals(exp0, arr.slice(0))

			Dim slice1 As val = arr.slice(1).dup("f"c)
			assertEquals(exp1, slice1)
			assertEquals(exp1, arr.slice(1))
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace