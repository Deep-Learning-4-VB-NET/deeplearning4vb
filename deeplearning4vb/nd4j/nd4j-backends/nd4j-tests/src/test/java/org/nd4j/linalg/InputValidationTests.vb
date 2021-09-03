Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.fail

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
'ORIGINAL LINE: @NativeTag public class InputValidationTests extends BaseNd4jTestWithBackends
	Public Class InputValidationTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		'///////////////////////////////////////////////////////////
		'/////////////////// Broadcast Tests ///////////////////////

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvalidColVectorOp1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInvalidColVectorOp1(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.create(10, 10)
			Dim col As INDArray = Nd4j.create(5, 1)
			Try
				first.muliColumnVector(col)
				fail("Should have thrown IllegalStateException")
			Catch e As System.InvalidOperationException
				'OK
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvalidColVectorOp2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInvalidColVectorOp2(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.create(10, 10)
			Dim col As INDArray = Nd4j.create(5, 1)
			Try
				first.addColumnVector(col)
				fail("Should have thrown IllegalStateException")
			Catch e As System.InvalidOperationException
				'OK
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvalidRowVectorOp1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInvalidRowVectorOp1(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.create(10, 10)
			Dim row As INDArray = Nd4j.create(1, 5)
			Try
				first.addiRowVector(row)
				fail("Should have thrown IllegalStateException")
			Catch e As System.InvalidOperationException
				'OK
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInvalidRowVectorOp2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInvalidRowVectorOp2(ByVal backend As Nd4jBackend)
			Dim first As INDArray = Nd4j.create(10, 10)
			Dim row As INDArray = Nd4j.create(1, 5)
			Try
				first.subRowVector(row)
				fail("Should have thrown IllegalStateException")
			Catch e As System.InvalidOperationException
				'OK
			End Try
		End Sub



	End Class

End Namespace