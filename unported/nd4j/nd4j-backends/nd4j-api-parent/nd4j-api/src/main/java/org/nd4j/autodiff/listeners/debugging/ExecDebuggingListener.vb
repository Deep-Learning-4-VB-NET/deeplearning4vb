Imports System
Imports System.Text
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports ScalarOp = org.nd4j.linalg.api.ops.ScalarOp

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

Namespace org.nd4j.autodiff.listeners.debugging


	Public Class ExecDebuggingListener
		Inherits BaseListener

		Public Enum PrintMode
			OPS_ONLY
			SHAPES_ONLY
			REPRODUCE

		End Enum
		Private ReadOnly printMode As PrintMode
		Private ReadOnly maxIterations As Integer
		Private ReadOnly logIter As Boolean

		Private printIterations As Long = 0
		Private lastIter As Integer = -1
		Private stepThisIter As Integer = 0

		''' <param name="printMode">     Print mode, see <seealso cref="PrintMode"/> </param>
		''' <param name="maxIterations"> Maximum number of iterations to print. <= 0 for "all iterations" </param>
		''' <param name="logIter">       If true: prefix iteration/epoch, such as "(iter=1,epoch=0,op=3)" to the output </param>
		Public Sub New(ByVal printMode As PrintMode, ByVal maxIterations As Integer, ByVal logIter As Boolean)
			Me.printMode = printMode
			Me.maxIterations = maxIterations
			Me.logIter = logIter
		End Sub

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return True
		End Function

		Public Overrides Sub preOpExecution(ByVal sd As SameDiff, ByVal at As At, ByVal op As SameDiffOp, ByVal opContext As OpContext)
			If lastIter <> at.iteration() Then
				lastIter = at.iteration()
				stepThisIter = 0
				printIterations += 1
			End If

			If maxIterations > 0 AndAlso printIterations > maxIterations Then
				Return
			End If

			Dim sb As New StringBuilder()
			If logIter Then
				sb.Append("(iter=").Append(at.iteration()).Append(",epoch=").Append(at.epoch()).Append(",")
			End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: sb.append("op=").append(stepThisIter++).append(logIter ? ") " : " - ");
			sb.Append("op=").Append(stepThisIter).Append(If(logIter, ") ", " - "))
				stepThisIter += 1

			Dim df As DifferentialFunction = op.Op
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			sb.Append(op.Op.GetType().FullName)
			Dim co As CustomOp = If(TypeOf df Is CustomOp, DirectCast(df, CustomOp), Nothing)
			Dim lOp As Op = If(TypeOf df Is Op, DirectCast(df, Op), Nothing)
			If printMode = PrintMode.OPS_ONLY Then
				sb.Append(vbLf)
			ElseIf printMode = PrintMode.SHAPES_ONLY Then
				If co IsNot Nothing Then
					If co.iArgs() IsNot Nothing AndAlso co.iArgs().Length > 0 Then
						sb.Append(vbLf & vbTab & "iArgs=").Append(Arrays.toString(co.iArgs()))
					End If
					If co.bArgs() IsNot Nothing AndAlso co.bArgs().Length > 0 Then
						sb.Append(vbLf & vbTab & "bArgs=").Append(Arrays.toString(co.bArgs()))
					End If
					If co.tArgs() IsNot Nothing AndAlso co.tArgs().Length > 0 Then
						sb.Append(vbLf & vbTab & "tArgs=").Append(Arrays.toString(co.tArgs()))
					End If
					Dim inputs As val = co.inputArguments()
					Dim outputs As val = co.outputArguments()
					If inputs IsNot Nothing Then
						For i As Integer = 0 To inputs.size() - 1
							sb.Append(vbLf & vbTab & "Input[").Append(i).Append("]=").Append(inputs.get(i).shapeInfoToString())
						Next i
					End If
					If outputs IsNot Nothing Then
						For i As Integer = 0 To outputs.size() - 1
							sb.Append(vbLf & vbTab & "Outputs[").Append(i).Append("]=").Append(outputs.get(i).shapeInfoToString())
						Next i
					End If
				Else
					If lOp.x() IsNot Nothing Then
						sb.Append(vbLf & vbTab & "x: ").Append(lOp.x().shapeInfoToString())
					End If
					If lOp.y() IsNot Nothing Then
						sb.Append(vbLf & vbTab & "y: ").Append(lOp.y().shapeInfoToString())
					End If
					If lOp.z() IsNot Nothing Then
						sb.Append(vbLf & vbTab & "z: ").Append(lOp.z().shapeInfoToString())
					End If
					If TypeOf lOp Is ScalarOp Then
						Dim scalar As INDArray = DirectCast(lOp, ScalarOp).scalar()
						If scalar IsNot Nothing Then
							sb.Append(vbLf & vbTab & "scalar: ").Append(scalar.shapeInfoToString())
						End If
					End If
				End If
				sb.Append(vbLf)
			ElseIf printMode = PrintMode.REPRODUCE Then
				sb.Append(vbLf)
				If co IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					sb.Append("DynamicCustomOp op = new ").Append(co.GetType().FullName).Append("();" & vbLf)
					If co.iArgs() IsNot Nothing AndAlso co.iArgs().Length > 0 Then
						sb.Append("op.addIArgument(").Append(Arrays.toString(co.iArgs()).replaceAll("[\[\]]", "")).Append(");" & vbLf)
					End If
					If co.bArgs() IsNot Nothing AndAlso co.bArgs().Length > 0 Then
						sb.Append("op.addBArgument(").Append(Arrays.toString(co.bArgs()).replaceAll("[\[\]]", "")).Append(");" & vbLf)
					End If
					If co.tArgs() IsNot Nothing AndAlso co.tArgs().Length > 0 Then
						sb.Append("op.addTArgument(").Append(Arrays.toString(co.tArgs()).replaceAll("[\[\]]", "")).Append(");" & vbLf)
					End If
					Dim inputs As val = co.inputArguments()
					Dim outputs As val = co.outputArguments()
					If inputs IsNot Nothing Then
						sb.Append("INDArray[] inputs = new INDArray[").Append(inputs.size()).Append("];" & vbLf)
						For i As Integer = 0 To inputs.size() - 1
							sb.Append("inputs[").Append(i).Append("] = ")
							sb.Append(createString(inputs.get(i))).Append(";" & vbLf)
						Next i
						sb.Append("op.addInputArgument(inputs);" & vbLf)
					End If
					If outputs IsNot Nothing Then
						sb.Append("INDArray[] outputs = new INDArray[").Append(outputs.size()).Append("];" & vbLf)
						For i As Integer = 0 To outputs.size() - 1
							sb.Append("outputs[").Append(i).Append("] = ")
							sb.Append(createString(outputs.get(i))).Append(";" & vbLf)
						Next i
						sb.Append("op.addOutputArgument(outputs);" & vbLf)
					End If
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					sb.Append("Op op = new ").Append(op.GetType().FullName).Append("();" & vbLf)
					If lOp.x() IsNot Nothing Then
						sb.Append("op.setX(").Append(createString(lOp.x())).Append(");" & vbLf)
					End If
					If lOp.y() IsNot Nothing Then
						sb.Append("op.setY(").Append(createString(lOp.y())).Append(");" & vbLf)
					End If
					If lOp.z() IsNot Nothing Then
						sb.Append("op.setZ").Append(createString(lOp.z())).Append(");" & vbLf)
					End If
					If TypeOf lOp Is ScalarOp Then
						Dim scalar As INDArray = DirectCast(lOp, ScalarOp).scalar()
						If scalar IsNot Nothing Then
							sb.Append("((ScalarOp)op).setScalar(").Append(createString(scalar)).Append(");" & vbLf)
						End If
					End If
				End If
				sb.Append("Nd4j.exec(op);" & vbLf)
			End If

			Console.Write(sb.ToString())
		End Sub

		Private Shared Function createString(ByVal arr As INDArray) As String
			Dim sb As New StringBuilder()

			If arr.Empty Then
				sb.Append("Nd4j.empty(DataType.").Append(arr.dataType()).Append(");")
			Else
				sb.Append("Nd4j.createFromArray(")

				Dim dt As DataType = arr.dataType()
				Select Case dt.innerEnumValue
					Case DataType.InnerEnum.DOUBLE
						Dim dArr() As Double = arr.dup().data().asDouble()
						sb.Append(Arrays.toString(dArr).replaceAll("[\[\]]", ""))
					Case DataType.InnerEnum.FLOAT, HALF, BFLOAT16
						Dim fArr() As Single = arr.dup().data().asFloat()
						sb.Append(Arrays.toString(fArr).replaceAll(",", "f,").replaceAll("]", "f").replaceAll("[\[\]]", ""))
					Case DataType.InnerEnum.LONG, UINT32, UINT64
						Dim lArr() As Long = arr.dup().data().asLong()
						sb.Append(Arrays.toString(lArr).replaceAll(",", "L,").replaceAll("]", "L").replaceAll("[\[\]]", ""))
					Case DataType.InnerEnum.INT, [SHORT], UBYTE, [BYTE], UINT16, BOOL
						Dim iArr() As Integer = arr.dup().data().asInt()
						sb.Append(Arrays.toString(iArr).replaceAll("[\[\]]", ""))
					Case DataType.InnerEnum.UTF8
					Case DataType.InnerEnum.COMPRESSED, UNKNOWN
				End Select

				sb.Append(").reshape(").Append(Arrays.toString(arr.shape()).replaceAll("[\[\]]", "")).Append(")")

				If dt = DataType.HALF OrElse dt = DataType.BFLOAT16 OrElse dt = DataType.UINT32 OrElse dt = DataType.UINT64 OrElse dt = DataType.SHORT OrElse dt = DataType.UBYTE OrElse dt = DataType.BYTE OrElse dt = DataType.UINT16 OrElse dt = DataType.BOOL Then
					sb.Append(".cast(DataType.").Append(arr.dataType()).Append(")")
				End If
			End If

			Return sb.ToString()
		End Function

	End Class

End Namespace