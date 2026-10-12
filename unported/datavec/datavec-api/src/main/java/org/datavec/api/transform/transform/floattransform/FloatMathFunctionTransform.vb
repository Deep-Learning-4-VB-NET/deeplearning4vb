Imports System
Imports Data = lombok.Data
Imports MathFunction = org.datavec.api.transform.MathFunction
Imports FloatWritable = org.datavec.api.writable.FloatWritable
Imports Writable = org.datavec.api.writable.Writable
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.transform.floattransform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class FloatMathFunctionTransform extends BaseFloatTransform
	<Serializable>
	Public Class FloatMathFunctionTransform
		Inherits BaseFloatTransform

		Private mathFunction As MathFunction

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FloatMathFunctionTransform(@JsonProperty("columnName") String columnName, @JsonProperty("mathFunction") org.datavec.api.transform.MathFunction mathFunction)
		Public Sub New(ByVal columnName As String, ByVal mathFunction As MathFunction)
			MyBase.New(columnName)
			Me.mathFunction = mathFunction
		End Sub

		Public Overrides Function map(ByVal w As Writable) As Writable
			Select Case mathFunction
				Case MathFunction.ABS
					Return New FloatWritable(Math.Abs(w.toFloat()))
				Case MathFunction.ACOS
					Return New FloatWritable(CSng(Math.Acos(w.toFloat())))
				Case MathFunction.ASIN
					Return New FloatWritable(CSng(Math.Asin(w.toFloat())))
				Case MathFunction.ATAN
					Return New FloatWritable(CSng(Math.Atan(w.toFloat())))
				Case MathFunction.CEIL
					Return New FloatWritable(CSng(Math.Ceiling(w.toFloat())))
				Case MathFunction.COS
					Return New FloatWritable(CSng(Math.Cos(w.toFloat())))
				Case MathFunction.COSH
					Return New FloatWritable(CSng(Math.Cosh(w.toFloat())))
				Case MathFunction.EXP
					Return New FloatWritable(CSng(Math.Exp(w.toFloat())))
				Case MathFunction.FLOOR
					Return New FloatWritable(CSng(Math.Floor(w.toFloat())))
				Case MathFunction.LOG
					Return New FloatWritable(CSng(Math.Log(w.toFloat())))
				Case MathFunction.LOG10
					Return New FloatWritable(CSng(Math.Log10(w.toFloat())))
				Case MathFunction.SIGNUM
					Return New FloatWritable(Math.Sign(w.toFloat()))
				Case MathFunction.SIN
					Return New FloatWritable(CSng(Math.Sin(w.toFloat())))
				Case MathFunction.SINH
					Return New FloatWritable(CSng(Math.Sinh(w.toFloat())))
				Case MathFunction.SQRT
					Return New FloatWritable(CSng(Math.Sqrt(w.toFloat())))
				Case MathFunction.TAN
					Return New FloatWritable(CSng(Math.Tan(w.toFloat())))
				Case MathFunction.TANH
					Return New FloatWritable(CSng(Math.Tanh(w.toFloat())))
				Case Else
					Throw New Exception("Unknown function: " & mathFunction)
			End Select
		End Function

		Public Overrides Function map(ByVal input As Object) As Object
			Dim d As Single? = DirectCast(input, Number).floatValue()
			Select Case mathFunction
				Case MathFunction.ABS
					Return Math.Abs(d)
				Case MathFunction.ACOS
					Return Math.Acos(d)
				Case MathFunction.ASIN
					Return Math.Asin(d)
				Case MathFunction.ATAN
					Return Math.Atan(d)
				Case MathFunction.CEIL
					Return Math.Ceiling(d)
				Case MathFunction.COS
					Return Math.Cos(d)
				Case MathFunction.COSH
					Return Math.Cosh(d)
				Case MathFunction.EXP
					Return Math.Exp(d)
				Case MathFunction.FLOOR
					Return Math.Floor(d)
				Case MathFunction.LOG
					Return Math.Log(d)
				Case MathFunction.LOG10
					Return Math.Log10(d)
				Case MathFunction.SIGNUM
					Return Math.Sign(d)
				Case MathFunction.SIN
					Return Math.Sin(d)
				Case MathFunction.SINH
					Return Math.Sinh(d)
				Case MathFunction.SQRT
					Return Math.Sqrt(d)
				Case MathFunction.TAN
					Return Math.Tan(d)
				Case MathFunction.TANH
					Return Math.Tanh(d)
				Case Else
					Throw New Exception("Unknown function: " & mathFunction)
			End Select
		End Function
	End Class

End Namespace