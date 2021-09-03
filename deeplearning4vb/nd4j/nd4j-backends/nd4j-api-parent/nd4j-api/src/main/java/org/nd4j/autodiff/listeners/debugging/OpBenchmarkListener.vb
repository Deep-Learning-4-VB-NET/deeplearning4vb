Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports lombok
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class OpBenchmarkListener extends org.nd4j.autodiff.listeners.BaseListener
	Public Class OpBenchmarkListener
		Inherits BaseListener

		Public Enum Mode
			SINGLE_ITER_PRINT
			AGGREGATE

		End Enum
		Private ReadOnly operation As Operation
		Private ReadOnly mode As Mode
		Private ReadOnly minRuntime As Long
		Private aggregateModeMap As IDictionary(Of String, OpExec)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.@PRIVATE) private long start;
		Private start As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.@PRIVATE) private boolean printActive;
		Private printActive As Boolean
		Private printDone As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OpBenchmarkListener(org.nd4j.autodiff.listeners.Operation operation, @NonNull Mode mode)
		Public Sub New(ByVal operation As Operation, ByVal mode As Mode)
			Me.New(operation, mode, 0)
		End Sub

		''' <param name="operation">  Operation to collect stats for </param>
		''' <param name="mode">       Mode - see <seealso cref="OpBenchmarkListener"/> </param>
		''' <param name="minRuntime"> Minimum runtime - only applies to Mode.SINGLE_ITER_PRINT. If op runtime below this: don't print </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OpBenchmarkListener(org.nd4j.autodiff.listeners.Operation operation, @NonNull Mode mode, long minRuntime)
		Public Sub New(ByVal operation As Operation, ByVal mode As Mode, ByVal minRuntime As Long)
			Me.operation = operation
			Me.mode = mode
			Me.minRuntime = minRuntime
		End Sub

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return Me.operation = Nothing OrElse Me.operation = operation
		End Function

		Public Overrides Sub operationStart(ByVal sd As SameDiff, ByVal op As Operation)
			If printDone Then
				Return
			End If
			If Me.operation = Nothing OrElse Me.operation = op Then
				printActive = True
			End If
		End Sub

		Public Overrides Sub operationEnd(ByVal sd As SameDiff, ByVal op As Operation)
			If printDone Then
				Return
			End If
			If Me.operation = Nothing OrElse Me.operation = op Then
				printActive = False
				printDone = True
			End If
		End Sub

		Public Overrides Sub preOpExecution(ByVal sd As SameDiff, ByVal at As At, ByVal op As SameDiffOp, ByVal opContext As OpContext)
			start = DateTimeHelper.CurrentUnixTimeMillis()
		End Sub

		Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
			Dim now As Long = DateTimeHelper.CurrentUnixTimeMillis()

			If mode = Mode.SINGLE_ITER_PRINT AndAlso printActive AndAlso (now-start) > Me.minRuntime Then
				Console.WriteLine(getOpString(op, now))
			ElseIf mode = Mode.AGGREGATE Then
				If aggregateModeMap Is Nothing Then
					aggregateModeMap = New LinkedHashMap(Of String, OpExec)()
				End If

				If Not aggregateModeMap.ContainsKey(op.Name) Then
					Dim s As String = getOpString(op, Nothing)
					Dim oe As New OpExec(op.Name, op.Op.opName(), op.Op.GetType(), New List(Of Long)(), s)
					aggregateModeMap(op.Name) = oe
				End If

				aggregateModeMap(op.Name).getRuntimeMs().add(now-start)
			End If
		End Sub

		Private Function getOpString(ByVal op As SameDiffOp, ByVal now As Long?) As String
			Dim sb As New StringBuilder()
			sb.Append(op.Name).Append(" - ").Append(op.Op.GetType().Name).Append("(").Append(op.Op.opName()).Append(") - ")
			If now IsNot Nothing Then
				sb.Append(now.Value - start).Append(" ms" & vbLf)
			End If

			If TypeOf op.Op Is DynamicCustomOp Then
				Dim dco As DynamicCustomOp = DirectCast(op.Op, DynamicCustomOp)
				Dim x As Integer = 0

				For Each i As INDArray In dco.inputArguments()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: sb.append("  in ").append(x++).append(": ").append(i.shapeInfoToString()).append(vbLf);
					sb.Append("  in ").Append(x).Append(": ").Append(i.shapeInfoToString()).Append(vbLf)
						x += 1
				Next i
				x = 0
				For Each o As INDArray In dco.outputArguments()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: sb.append("  out ").append(x++).append(": ").append(o.shapeInfoToString()).append(vbLf);
					sb.Append("  out ").Append(x).Append(": ").Append(o.shapeInfoToString()).Append(vbLf)
						x += 1
				Next o
				Dim iargs() As Long = dco.iArgs()
				Dim bargs() As Boolean = dco.bArgs()
				Dim targs() As Double = dco.tArgs()
				If iargs IsNot Nothing AndAlso iargs.Length > 0 Then
					sb.Append("  iargs: ").Append(java.util.Arrays.toString(iargs)).Append(vbLf)
				End If
				If bargs IsNot Nothing AndAlso bargs.Length > 0 Then
					sb.Append("  bargs: ").Append(java.util.Arrays.toString(bargs)).Append(vbLf)
				End If
				If targs IsNot Nothing AndAlso targs.Length > 0 Then
					sb.Append("  targs: ").Append(java.util.Arrays.toString(targs)).Append(vbLf)
				End If
			Else
				Dim o As Op = DirectCast(op.Op, Op)
				If o.x() IsNot Nothing Then
					sb.Append("  x: ").Append(o.x().shapeInfoToString())
				End If
				If o.y() IsNot Nothing Then
					sb.Append("  y: ").Append(o.y().shapeInfoToString())
				End If
				If o.z() IsNot Nothing Then
					sb.Append("  z: ").Append(o.z().shapeInfoToString())
				End If
			End If
			Return sb.ToString()
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class OpExec
		Public Class OpExec
			Friend ReadOnly opOwnName As String
			Friend ReadOnly opName As String
			Friend ReadOnly opClass As Type
			Friend runtimeMs As IList(Of Long)
			Friend firstIter As String

			Public Overrides Function ToString() As String
				Dim df As New DecimalFormat("0.000")

				Return opOwnName & " - op class: " & opClass.Name & " (op name: " & opName & ")" & vbLf & "count: " & runtimeMs.Count & ", mean: " & df.format(avgMs()) & "ms, std: " & df.format(stdMs()) & "ms, min: " & minMs() & "ms, max: " & maxMs() & "ms" & vbLf & firstIter
			End Function

			Public Overridable Function avgMs() As Double
				Dim sum As Long = 0
				For Each l As Long? In runtimeMs
					sum += l
				Next l
				Return sum / CDbl(runtimeMs.Count)
			End Function

			Public Overridable Function stdMs() As Double
				Return Nd4j.createFromArray(ArrayUtil.toArrayLong(runtimeMs)).stdNumber().doubleValue()
			End Function

			Public Overridable Function minMs() As Long
				Return Nd4j.createFromArray(ArrayUtil.toArrayLong(runtimeMs)).minNumber().longValue()
			End Function

			Public Overridable Function maxMs() As Long
				Return Nd4j.createFromArray(ArrayUtil.toArrayLong(runtimeMs)).maxNumber().longValue()
			End Function
		End Class
	End Class

End Namespace