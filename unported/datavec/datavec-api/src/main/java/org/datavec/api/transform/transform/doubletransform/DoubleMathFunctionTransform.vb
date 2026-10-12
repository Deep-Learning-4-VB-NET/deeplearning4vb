Imports System
Imports Data = lombok.Data
Imports MathFunction = org.datavec.api.transform.MathFunction
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
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

Namespace org.datavec.api.transform.transform.doubletransform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class DoubleMathFunctionTransform extends BaseDoubleTransform
	<Serializable>
	Public Class DoubleMathFunctionTransform
		Inherits BaseDoubleTransform

		Private mathFunction As MathFunction

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DoubleMathFunctionTransform(@JsonProperty("columnName") String columnName, @JsonProperty("mathFunction") org.datavec.api.transform.MathFunction mathFunction)
		Public Sub New(ByVal columnName As String, ByVal mathFunction As MathFunction)
			MyBase.New(columnName)
			Me.mathFunction = mathFunction
		End Sub

		Public Overrides Function map(ByVal w As Writable) As Writable
			Select Case mathFunction
				Case MathFunction.ABS
					Return New DoubleWritable(Math.Abs(w.toDouble()))
				Case MathFunction.ACOS
					Return New DoubleWritable(Math.Acos(w.toDouble()))
				Case MathFunction.ASIN
					Return New DoubleWritable(Math.Asin(w.toDouble()))
				Case MathFunction.ATAN
					Return New DoubleWritable(Math.Atan(w.toDouble()))
				Case MathFunction.CEIL
					Return New DoubleWritable(Math.Ceiling(w.toDouble()))
				Case MathFunction.COS
					Return New DoubleWritable(Math.Cos(w.toDouble()))
				Case MathFunction.COSH
					Return New DoubleWritable(Math.Cosh(w.toDouble()))
				Case MathFunction.EXP
					Return New DoubleWritable(Math.Exp(w.toDouble()))
				Case MathFunction.FLOOR
					Return New DoubleWritable(Math.Floor(w.toDouble()))
				Case MathFunction.LOG
					Return New DoubleWritable(Math.Log(w.toDouble()))
				Case MathFunction.LOG10
					Return New DoubleWritable(Math.Log10(w.toDouble()))
				Case MathFunction.SIGNUM
					Return New DoubleWritable(Math.Sign(w.toDouble()))
				Case MathFunction.SIN
					Return New DoubleWritable(Math.Sin(w.toDouble()))
				Case MathFunction.SINH
					Return New DoubleWritable(Math.Sinh(w.toDouble()))
				Case MathFunction.SQRT
					Return New DoubleWritable(Math.Sqrt(w.toDouble()))
				Case MathFunction.TAN
					Return New DoubleWritable(Math.Tan(w.toDouble()))
				Case MathFunction.TANH
					Return New DoubleWritable(Math.Tanh(w.toDouble()))
				Case Else
					Throw New Exception("Unknown function: " & mathFunction)
			End Select
		End Function

		Public Overrides Function map(ByVal input As Object) As Object
			Dim d As Double = DirectCast(input, Number).doubleValue()
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