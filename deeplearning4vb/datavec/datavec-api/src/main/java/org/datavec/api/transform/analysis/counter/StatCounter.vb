Imports System

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

Namespace org.datavec.api.transform.analysis.counter


	<Serializable>
	Public Class StatCounter

'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private count_Conflict As Long = 0
		Private runningMean As Double
		Private runningM2 As Double ' Running variance numerator (sum of (x - mean)^2)
'JAVA TO VB CONVERTER NOTE: The field max was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private max_Conflict As Double = -Double.MaxValue
'JAVA TO VB CONVERTER NOTE: The field min was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private min_Conflict As Double = Double.MaxValue


		Public Overridable ReadOnly Property Mean As Double
			Get
				Return runningMean
			End Get
		End Property

		Public Overridable ReadOnly Property Sum As Double
			Get
				Return runningMean * count_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Min As Double
			Get
				Return min_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Max As Double
			Get
				Return max_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Count As Long
			Get
				Return count_Conflict
			End Get
		End Property

		Public Overridable Function getVariance(ByVal population As Boolean) As Double
			Dim divisor As Long = (If(population, count_Conflict, count_Conflict-1))
			If (population AndAlso count_Conflict = 0) OrElse (Not population AndAlso count_Conflict = 1) Then
				Return Double.NaN
			End If
			Return runningM2 / divisor
		End Function

		Public Overridable Function getStddev(ByVal population As Boolean) As Double
			Return Math.Sqrt(getVariance(population))
		End Function

		Public Overridable Sub add(ByVal x As Double)
			Dim d As Double = x - runningMean
			count_Conflict += 1
			runningMean += (d / count_Conflict)
			runningM2 += (d * (x - runningMean))
			max_Conflict = Math.Max(max_Conflict, x)
			min_Conflict = Math.Min(min_Conflict, x)
		End Sub

		Public Overridable Function merge(ByVal o As StatCounter) As StatCounter
			If o Is Nothing OrElse o.count_Conflict = 0 Then
				Return Me
			End If
			If o Is Me Then
				Return merge(o.clone())
			End If
			If Me.count_Conflict = 0 Then
				count_Conflict = o.count_Conflict
				runningMean = o.runningMean
				runningMean = o.runningM2
				max_Conflict = o.max_Conflict
				min_Conflict = o.min_Conflict
			Else
				min_Conflict = Math.Min(min_Conflict, o.min_Conflict)
				max_Conflict = Math.Max(max_Conflict, o.max_Conflict)

				Dim d As Double = o.runningMean - runningMean
				If o.count_Conflict * 10 < count_Conflict Then
					runningMean = runningMean + (d * o.count_Conflict) / (count_Conflict + o.count_Conflict)
				ElseIf count_Conflict * 10 < o.count_Conflict Then
					runningMean = o.runningMean - (d * count_Conflict) / (count_Conflict + o.count_Conflict)
				Else
					runningMean = (runningMean * count_Conflict + o.runningMean * o.count_Conflict) / (count_Conflict + o.count_Conflict)
				End If
				runningM2 += o.runningM2 + (d * d * count_Conflict * o.count_Conflict) / (count_Conflict + o.count_Conflict)
				count_Conflict += o.count_Conflict
			End If

			Return Me
		End Function

		Public Overridable Function clone() As StatCounter
			Dim ret As New StatCounter()
			ret.count_Conflict = count_Conflict
			ret.runningMean = runningMean
			ret.runningM2 = runningM2
			ret.max_Conflict = max_Conflict
			ret.min_Conflict = min_Conflict
			Return ret
		End Function

		Public Overrides Function ToString() As String
			Return "StatCounter(count=" & count_Conflict & ",mean=" & runningMean & ",stdev=" & getStddev(False) & ",min=" & min_Conflict & ",max=" & max_Conflict & ")"
		End Function
	End Class

End Namespace