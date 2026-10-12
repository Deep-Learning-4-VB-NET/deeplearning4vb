Imports System
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports [Step] = org.nd4j.linalg.api.ops.impl.scalar.Step
Imports CubeDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.CubeDerivative
Imports HardSigmoidDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.HardSigmoidDerivative
Imports HardTanhDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.HardTanhDerivative
Imports LeakyReLUDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUDerivative
Imports SoftSignDerivative = org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftSignDerivative
Imports Sigmoid = org.nd4j.linalg.api.ops.impl.transforms.strict.Sigmoid
Imports SigmoidDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.SigmoidDerivative
Imports TanhDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.TanhDerivative
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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

Namespace org.nd4j.linalg.ops


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class DerivativeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DerivativeTests
		Inherits BaseNd4jTestWithBackends

		Public Const REL_ERROR_TOLERANCE As Double = 1e-3


		Friend initialType As DataType = Nd4j.dataType()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			Nd4j.DataType = Me.initialType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHardTanhDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHardTanhDerivative(ByVal backend As Nd4jBackend)
				'HardTanh:
			'f(x) = 1 if x > 1
			'f(x) = -1 if x < -1
			'f(x) = x otherwise
			'This is piecewise differentiable.
			'f'(x) = 0 if |x|>1
			'f'(x) = 1 otherwise
			'Note for x= +/- 1, HardTanh is not differentiable. Choose f'(+/- 1) = 1

			Dim z As INDArray = Nd4j.zeros(100)
			Dim expOut(99) As Double
			For i As Integer = 0 To 99
				Dim x As Double = 0.1 * (i - 50)
				z.putScalar(i, x)
				expOut(i) = (If(Math.Abs(x) <= 1.0, 1, 0))
			Next i

			Dim zPrime As INDArray = Nd4j.Executioner.exec(New HardTanhDerivative(z))

			For i As Integer = 0 To 99
				assertEquals(expOut(i), zPrime.getDouble(i), 1e-1)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRectifiedLinearDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRectifiedLinearDerivative(ByVal backend As Nd4jBackend)
			'ReLU:
			'f(x) = max(0,x)
			'Piecewise differentiable; choose f'(0) = 0
			'f'(x) = 1 if x > 0
			'f'(x) = 0 if x <= 0

			Dim z As INDArray = Nd4j.zeros(100).castTo(DataType.DOUBLE)
			Dim expOut(99) As Double
			For i As Integer = 0 To 99
				Dim x As Double = 0.1 * (i - 50)
				z.putScalar(i, x)
				expOut(i) = (If(x > 0, 1, 0))
			Next i

			Dim zPrime As INDArray = Nd4j.Executioner.exec(New [Step](z))

			For i As Integer = 0 To 99
				assertTrue(expOut(i) = zPrime.getDouble(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSigmoidDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSigmoidDerivative(ByVal backend As Nd4jBackend)
			'Derivative of sigmoid: ds(x)/dx = s(x)*(1-s(x))
			's(x) = 1 / (exp(-x) + 1)
			Dim z As INDArray = Nd4j.zeros(100)
			Dim expOut(99) As Double
			For i As Integer = 0 To 99
				Dim x As Double = 0.1 * (i - 50)
				z.putScalar(i, x)
				Dim sigmoid As Double = 1.0 / (FastMath.exp(-x) + 1)
				expOut(i) = sigmoid * (1 - sigmoid)
			Next i

			Dim zPrime As INDArray = Nd4j.Executioner.exec(New SigmoidDerivative(z))

			For i As Integer = 0 To 99
				Dim relError As Double = Math.Abs(expOut(i) - zPrime.getDouble(i)) / (Math.Abs(expOut(i)) + Math.Abs(zPrime.getDouble(i)))
				assertTrue(relError < REL_ERROR_TOLERANCE)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHardSigmoidDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHardSigmoidDerivative(ByVal backend As Nd4jBackend)
	'        
	'        f(x) = min(1, max(0, 0.2*x + 0.5))
	'        or equivalently: clip 0.2*x+0.5 to range 0 to 1
	'        where clipping bounds are -2.5 and 2.5
	'        
	'        Hard sigmoid derivative:
	'        f'(x) =
	'        0 if x < -2.5 or x > 2.5
	'        0.2 otherwise
	'         

			Dim expHSOut(299) As Double
			Dim expDerivOut(299) As Double
			Dim xArr As INDArray = Nd4j.linspace(-3, 3, 300, Nd4j.dataType())
			For i As Integer = 0 To xArr.length() - 1
				Dim x As Double = xArr.getDouble(i)
				Dim hs As Double = 0.2 * x + 0.5
				If hs < 0 Then
					hs = 0
				End If
				If hs > 1 Then
					hs = 1
				End If
				expHSOut(i) = hs

				Dim hsDeriv As Double
				If x < -2.5 OrElse x > 2.5 Then
					hsDeriv = 0
				Else
					hsDeriv = 0.2
				End If

				expDerivOut(i) = hsDeriv
			Next i

			Dim z As INDArray = Transforms.hardSigmoid(xArr, True)
			Dim zPrime As INDArray = Nd4j.Executioner.exec(New HardSigmoidDerivative(xArr.dup()))

			For i As Integer = 0 To expHSOut.Length - 1
				Dim relErrorHS As Double = Math.Abs(expHSOut(i) - z.getDouble(i)) / (Math.Abs(expHSOut(i)) + Math.Abs(z.getDouble(i)))
				If Not (expHSOut(i) = 0 AndAlso z.getDouble(i) = 0) Then
					assertTrue(relErrorHS < REL_ERROR_TOLERANCE)
				End If
				Dim relErrorDeriv As Double = Math.Abs(expDerivOut(i) - zPrime.getDouble(i)) / (Math.Abs(expDerivOut(i)) + Math.Abs(zPrime.getDouble(i)))
				If Not (expDerivOut(i) = 0 AndAlso zPrime.getDouble(i) = 0) Then
					assertTrue(relErrorDeriv < REL_ERROR_TOLERANCE)
				End If
			Next i

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftPlusDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftPlusDerivative(ByVal backend As Nd4jBackend)
			's(x) = 1 / (exp(-x) + 1)
			Dim z As INDArray = Nd4j.zeros(100)
			Dim expOut(99) As Double
			For i As Integer = 0 To 99
				Dim x As Double = 0.1 * (i - 50)
				z.putScalar(i, x)
				expOut(i) = 1.0 / (1.0 + FastMath.exp(-x))
			Next i

			Dim zPrime As INDArray = Nd4j.Executioner.exec(New Sigmoid(z))

			For i As Integer = 0 To 99
				Dim relError As Double = Math.Abs(expOut(i) - zPrime.getDouble(i)) / (Math.Abs(expOut(i)) + Math.Abs(zPrime.getDouble(i)))
				assertTrue(relError < REL_ERROR_TOLERANCE)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTanhDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTanhDerivative(ByVal backend As Nd4jBackend)

			'Derivative of sigmoid: ds(x)/dx = s(x)*(1-s(x))
			's(x) = 1 / (exp(-x) + 1)
			Dim z As INDArray = Nd4j.zeros(100)
			Dim expOut(99) As Double
			For i As Integer = 0 To 99
				Dim x As Double = 0.1 * (i - 50)
				z.putScalar(i, x)
				Dim tanh As Double = FastMath.tanh(x)
				expOut(i) = 1.0 - tanh * tanh
			Next i

			Dim zPrime As INDArray = Nd4j.Executioner.exec(New TanhDerivative(z))

			For i As Integer = 0 To 99
				Dim relError As Double = Math.Abs(expOut(i) - zPrime.getDouble(i)) / (Math.Abs(expOut(i)) + Math.Abs(zPrime.getDouble(i)))
				assertTrue(relError < REL_ERROR_TOLERANCE)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCubeDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCubeDerivative(ByVal backend As Nd4jBackend)

			'Derivative of cube: 3*x^2
			Dim z As INDArray = Nd4j.zeros(100)
			Dim expOut(99) As Double
			For i As Integer = 0 To 99
				Dim x As Double = 0.1 * (i - 50)
				z.putScalar(i, x)
				expOut(i) = 3 * x * x
			Next i

			Dim zPrime As INDArray = Nd4j.Executioner.exec(New CubeDerivative(z))

			For i As Integer = 0 To 99
				Dim d1 As Double = expOut(i)
				Dim d2 As Double = zPrime.getDouble(i)
				Dim relError As Double = Math.Abs(d1 - d1) / (Math.Abs(d1) + Math.Abs(d2))
				If d1 = 0.0 AndAlso d2 = 0.0 Then
					relError = 0.0
				End If
				Dim str As String = "exp=" & expOut(i) & ", act=" & zPrime.getDouble(i) & "; relError = " & relError
				assertTrue(relError < REL_ERROR_TOLERANCE,str)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeakyReLUDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeakyReLUDerivative(ByVal backend As Nd4jBackend)
			'Derivative: 0.01 if x<0, 1 otherwise
			Dim z As INDArray = Nd4j.zeros(100)
			Dim expOut(99) As Double
			For i As Integer = 0 To 99
				Dim x As Double = 0.1 * (i - 50)
				z.putScalar(i, x)
				expOut(i) = (If(x >= 0, 1, 0.25))
			Next i

			Dim zPrime As INDArray = Nd4j.Executioner.exec(New LeakyReLUDerivative(z, 0.25))

			For i As Integer = 0 To 99
				Dim relError As Double = Math.Abs(expOut(i) - zPrime.getDouble(i)) / (Math.Abs(expOut(i)) + Math.Abs(zPrime.getDouble(i)))
				assertTrue(relError < REL_ERROR_TOLERANCE)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftSignDerivative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftSignDerivative(ByVal backend As Nd4jBackend)
			'Derivative: 1 / (1+abs(x))^2
			Dim z As INDArray = Nd4j.zeros(100).castTo(DataType.DOUBLE)
			Dim expOut(99) As Double
			For i As Integer = 0 To 99
				Dim x As Double = 0.1 * (i - 50)
				z.putScalar(i, x)
				Dim temp As Double = 1 + Math.Abs(x)
				expOut(i) = 1.0 / (temp * temp)
			Next i

			Dim zPrime As INDArray = Nd4j.Executioner.exec(New SoftSignDerivative(z))

			For i As Integer = 0 To 99
				Dim relError As Double = Math.Abs(expOut(i) - zPrime.getDouble(i)) / (Math.Abs(expOut(i)) + Math.Abs(zPrime.getDouble(i)))
				assertTrue(relError < REL_ERROR_TOLERANCE)
			Next i
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace