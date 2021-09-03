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

Namespace org.nd4j.linalg.schedule

	<Serializable>
	Public Class RampSchedule
		Implements ISchedule

		Protected Friend ReadOnly baseSchedule As ISchedule
		Protected Friend ReadOnly numIter As Integer

		Public Sub New(ByVal baseSchedule As ISchedule, ByVal numIter As Integer)
			Me.baseSchedule = baseSchedule
			Me.numIter = numIter
		End Sub

		Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
			Dim base As Double = baseSchedule.valueAt(iteration, epoch)
			If iteration >= numIter - 1 Then
				Return base
			End If
			Dim frac As Double = (iteration+1) / CDbl(numIter)
			Return frac * base
		End Function

		Public Overridable Function clone() As ISchedule Implements ISchedule.clone
			Return New RampSchedule(baseSchedule.clone(), numIter)
		End Function
	End Class

End Namespace