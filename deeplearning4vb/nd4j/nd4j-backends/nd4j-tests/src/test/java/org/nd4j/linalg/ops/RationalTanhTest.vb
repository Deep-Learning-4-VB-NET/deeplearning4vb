Imports System
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports RationalTanhDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.RationalTanhDerivative
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.ops

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class RationalTanhTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class RationalTanhTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void gradientCheck(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub gradientCheck(ByVal backend As Nd4jBackend)

			Dim eps As Double = 1e-6
			Dim A As INDArray = Nd4j.linspace(-3, 3, 10).reshape(ChrW(2), 5)
			Dim ADer As INDArray = Nd4j.Executioner.exec(New RationalTanhDerivative(A.dup()))

'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict() As Double = A.data().asDouble()
'JAVA TO VB CONVERTER NOTE: The variable aDer was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim aDer_Conflict() As Double = ADer.data().asDouble()

			For i As Integer = 0 To 9
				Dim empirical As Double = (f(a_Conflict(i) + eps) - f(a_Conflict(i) - eps)) / (2 * eps)
				Dim analytic As Double = aDer_Conflict(i)
				assertTrue(Math.Abs(empirical - analytic) / (Math.Abs(empirical) + Math.Abs(analytic)) < 0.001)
			Next i

		End Sub

		Public Shared Function f(ByVal x As Double) As Double
			Return 1.7159 * tanhApprox(2.0 / 3 * x)
		End Function

	'    
	'    public static INDArray fDeriv(double x){
	'        //return C1 * 2.0/3 * tanhDeriv(2.0 / 3 * x);
	'    }
	'    

		Public Shared Function tanhApprox(ByVal y As Double) As Double
			Return Math.Sign(y) * (1.0 - 1.0 / (1 + Math.Abs(y) + y * y + 1.41645 * Math.Pow(y, 4.0)))
		End Function

	'    
	'    public static double tanhDeriv(double y){
	'        double a = 1 + Math.abs(y) + y*y + C * Math.pow(y,4);
	'        return (1 + Math.signum(y) * (2*y + 4*C*Math.pow(y,3))) / (a * a);
	'    }
	'    

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace