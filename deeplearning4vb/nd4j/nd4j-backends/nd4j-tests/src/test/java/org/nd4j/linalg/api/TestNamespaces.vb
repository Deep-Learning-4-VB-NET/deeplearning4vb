Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend

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

Namespace org.nd4j.linalg.api

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class TestNamespaces extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestNamespaces
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitwiseSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitwiseSimple(ByVal backend As Nd4jBackend)

			Dim x As INDArray = Nd4j.rand(DataType.FLOAT, 1, 5).muli(100000).castTo(DataType.INT)
			Dim y As INDArray = Nd4j.rand(DataType.FLOAT, 1, 5).muli(100000).castTo(DataType.INT)

			Dim [and] As INDArray = Nd4j.bitwise_Conflict.and(x, y)
			Dim [or] As INDArray = Nd4j.bitwise_Conflict.or(x, y)
			Dim [xor] As INDArray = Nd4j.bitwise_Conflict.xor(x, y)

	'        System.out.println(and);
	'        System.out.println(or);
	'        System.out.println(xor);

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMathSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMathSimple(ByVal backend As Nd4jBackend)
			Dim x As INDArray = Nd4j.rand(DataType.FLOAT, 1, 5).muli(2).subi(1)
			Dim abs As INDArray = Nd4j.math_Conflict.abs(x)
	'        System.out.println(x);
	'        System.out.println(abs);


			Dim c1 As INDArray = Nd4j.createFromArray(0, 2, 1)
			Dim c2 As INDArray = Nd4j.createFromArray(1, 2, 1)

			Dim cm As INDArray = Nd4j.math_Conflict.confusionMatrix(c1, c2, 3)
	'        System.out.println(cm);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomSimple(ByVal backend As Nd4jBackend)
			Dim normal As INDArray = Nd4j.random_Conflict.normal(0, 1, DataType.FLOAT, 10)
	'        System.out.println(normal);
			Dim uniform As INDArray = Nd4j.random_Conflict.uniform(0, 1, DataType.FLOAT, 10)
	'        System.out.println(uniform);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNeuralNetworkSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNeuralNetworkSimple(ByVal backend As Nd4jBackend)
			Dim [out] As INDArray = Nd4j.nn_Conflict.elu(Nd4j.random_Conflict.normal(0, 1, DataType.FLOAT, 10))
	'        System.out.println(out);
			Dim out2 As INDArray = Nd4j.nn_Conflict.softmax(Nd4j.random_Conflict.normal(0, 1, DataType.FLOAT, 4, 5), 1)
	'        System.out.println(out2);
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

	End Class

End Namespace