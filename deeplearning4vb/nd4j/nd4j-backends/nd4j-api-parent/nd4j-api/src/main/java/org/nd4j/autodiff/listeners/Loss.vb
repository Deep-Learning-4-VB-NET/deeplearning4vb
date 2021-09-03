Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.nd4j.autodiff.listeners



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class Loss
	Public Class Loss

'JAVA TO VB CONVERTER NOTE: The field lossNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly lossNames_Conflict As IList(Of String)
		Private ReadOnly losses() As Double

		''' <param name="lossNames"> Names of the losses </param>
		''' <param name="losses">    Values for each loss. Must be same length as lossNames </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Loss(@NonNull List<String> lossNames, @NonNull double[] losses)
		Public Sub New(ByVal lossNames As IList(Of String), ByVal losses() As Double)
			Preconditions.checkState(lossNames.size() = losses.Length, "Expected equal number of loss names and loss values")
			Me.lossNames_Conflict = lossNames
			Me.losses = losses
		End Sub

		''' <returns> Number of loss values (i.e., length of lossNames and losses) </returns>
		Public Overridable Function numLosses() As Integer
			Return lossNames_Conflict.Count
		End Function

		''' <returns> Names of all of the loss components </returns>
		Public Overridable Function lossNames() As IList(Of String)
			Return lossNames_Conflict
		End Function

		''' <returns> Values corresponding to each of the losses (same order as lossNames()) </returns>
		Public Overridable Function lossValues() As Double()
			Return losses
		End Function

		''' <summary>
		''' Get the specified loss by name
		''' </summary>
		''' <param name="lossName"> Name of the loss (must exist) </param>
		''' <returns> Specified loss value </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public double getLoss(@NonNull String lossName)
		Public Overridable Function getLoss(ByVal lossName As String) As Double
			Dim idx As Integer = lossNames_Conflict.IndexOf(lossName)
			Preconditions.checkState(idx >= 0, "No loss with name ""%s"" exists. All loss names: %s", lossName, lossNames_Conflict)
			Return losses(idx)
		End Function

		''' <returns> The total loss (sum of all loss components) </returns>
		Public Overridable Function totalLoss() As Double
			Dim sum As Double = 0.0
			For Each d As Double In losses
				sum += d
			Next d
			Return sum
		End Function

		Public Overridable Function copy() As Loss
			Return New Loss(lossNames_Conflict, losses)
		End Function

		Public Shared Function sum(ByVal losses As IList(Of Loss)) As Loss

			If losses.Count = 0 Then
				Return New Loss(Enumerable.Empty(Of String)(), New Double(){})
			End If

			Dim lossValues((losses(0).losses.Length) - 1) As Double
			Dim lossNames As IList(Of String) = New List(Of String)(losses(0).lossNames)

			For i As Integer = 0 To losses.Count - 1
				Dim l As Loss = losses(i)
				Preconditions.checkState(l.losses.Length = lossValues.Length, "Loss %s has %s losses, the others before it had %s.", i, l.losses.Length, lossValues.Length)

'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(l.lossNames.equals(lossNames), "Loss %s has different loss names from the others before it.  Expected %s, got %s.", i, lossNames, l.lossNames);
				Preconditions.checkState(l.lossNames_Conflict.SequenceEqual(lossNames), "Loss %s has different loss names from the others before it.  Expected %s, got %s.", i, lossNames, l.lossNames_Conflict)

				For j As Integer = 0 To lossValues.Length - 1
					lossValues(j) += l.losses(j)
				Next j

			Next i

			Return New Loss(lossNames, lossValues)
		End Function

		Public Shared Function average(ByVal losses As IList(Of Loss)) As Loss
			Dim sum As Loss = Loss.sum(losses)

			For i As Integer = 0 To sum.losses.Length - 1
				sum.losses(i) /= losses.Count
			Next i

			Return sum
		End Function

		Public Shared Function add(ByVal a As Loss, ByVal b As Loss) As Loss
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(a.lossNames.equals(b.lossNames), "Loss names differ.  First loss has names %s, second has names %s.", a.lossNames, b.lossNames);
			Preconditions.checkState(a.lossNames_Conflict.SequenceEqual(b.lossNames_Conflict), "Loss names differ.  First loss has names %s, second has names %s.", a.lossNames_Conflict, b.lossNames_Conflict)

			Dim lossValues(a.losses.Length - 1) As Double
			For i As Integer = 0 To lossValues.Length - 1
				lossValues(i) = a.losses(i) + b.losses(i)
			Next i

			Return New Loss(a.lossNames_Conflict, lossValues)
		End Function

		Public Shared Function [sub](ByVal a As Loss, ByVal b As Loss) As Loss
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(a.lossNames.equals(b.lossNames), "Loss names differ.  First loss has names %s, second has names %s.", a.lossNames, b.lossNames);
			Preconditions.checkState(a.lossNames_Conflict.SequenceEqual(b.lossNames_Conflict), "Loss names differ.  First loss has names %s, second has names %s.", a.lossNames_Conflict, b.lossNames_Conflict)

			Dim lossValues(a.losses.Length - 1) As Double
			For i As Integer = 0 To lossValues.Length - 1
				lossValues(i) = a.losses(i) - b.losses(i)
			Next i

			Return New Loss(a.lossNames_Conflict, lossValues)
		End Function

		Public Shared Function div(ByVal a As Loss, ByVal b As Number) As Loss
			Dim lossValues(a.losses.Length - 1) As Double
			For i As Integer = 0 To lossValues.Length - 1
				lossValues(i) = a.losses(i) / b.doubleValue()
			Next i

			Return New Loss(a.lossNames_Conflict, lossValues)
		End Function

		Public Overridable Function add(ByVal other As Loss) As Loss
			Return add(Me, other)
		End Function

		Public Overridable Function [sub](ByVal other As Loss) As Loss
			Return [sub](Me, other)
		End Function

		Public Overridable Function plus(ByVal other As Loss) As Loss
			Return add(Me, other)
		End Function

		Public Overridable Function minus(ByVal other As Loss) As Loss
			Return [sub](Me, other)
		End Function

		Public Overridable Function div(ByVal other As Number) As Loss
			Return div(Me, other)
		End Function

	End Class

End Namespace