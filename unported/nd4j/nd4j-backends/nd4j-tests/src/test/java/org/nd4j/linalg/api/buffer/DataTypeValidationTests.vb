Imports System
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertThrows

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

Namespace org.nd4j.linalg.api.buffer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class DataTypeValidationTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DataTypeValidationTests
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			initialType = Nd4j.dataType()
			Nd4j.DataType = DataType.FLOAT
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void reset()
		Public Overridable Sub reset()
			Nd4j.DataType = initialType
		End Sub

		''' <summary>
		''' Testing basic assign
		''' </summary>
	'    
	'    @Test(expected = ND4JIllegalStateException.class)
	'    public void testOpValidation1() {
	'        INDArray x = Nd4j.create(10);
	'
	'        Nd4j.setDataType(DataType.DOUBLE);
	'
	'        INDArray y = Nd4j.create(10);
	'
	'        x.addi(y);
	'
	'        Nd4j.getExecutioner().commit();
	'    }
	'
		''' <summary>
		''' Testing level1 blas
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasValidation1(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testBlasValidation1(ByVal backend As Nd4jBackend)
		   assertThrows(GetType(ND4JIllegalStateException),Sub()
		   Dim x As INDArray = Nd4j.create(10)
		   Nd4j.DataType = DataType.DOUBLE
		   Dim y As INDArray = Nd4j.create(10)
		   Nd4j.BlasWrapper.dot(x, y)
		   End Sub)

		 End Sub

		''' <summary>
		''' Testing level2 blas
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasValidation2(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testBlasValidation2(ByVal backend As Nd4jBackend)
			assertThrows(GetType(Exception),Sub()
			Dim a As INDArray = Nd4j.create(100, 10)
			Dim x As INDArray = Nd4j.create(100)
			Nd4j.DataType = DataType.DOUBLE
			Dim y As INDArray = Nd4j.create(100)
			Nd4j.BlasWrapper.gemv(1.0, a, x, 1.0, y)
			End Sub)

		 End Sub

		''' <summary>
		''' Testing level3 blas
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBlasValidation3(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub testBlasValidation3(ByVal backend As Nd4jBackend)
		   assertThrows(GetType(System.InvalidOperationException),Sub()
		   Dim x As INDArray = Nd4j.create(100, 100)
		   Nd4j.DataType = DataType.DOUBLE
		   Dim y As INDArray = Nd4j.create(100, 100)
		   x.mmul(y)
		   End Sub)

		 End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace