Imports System
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.analysis.histogram

	<Serializable>
	Public Class DoubleHistogramCounter
		Implements HistogramCounter

		Private ReadOnly minValue As Double
		Private ReadOnly maxValue As Double
		Private ReadOnly nBins As Integer
'JAVA TO VB CONVERTER NOTE: The field bins was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly bins_Conflict() As Double
		Private ReadOnly binCounts() As Long

		Public Sub New(ByVal minValue As Double, ByVal maxValue As Double, ByVal nBins As Integer)
			Me.minValue = minValue
			Me.maxValue = maxValue
			Me.nBins = nBins

			bins_Conflict = New Double(nBins){} '+1 because bins are defined by a range of values: bins[i] to bins[i+1]
			Dim [step] As Double = (maxValue - minValue) / nBins
			For i As Integer = 0 To bins_Conflict.Length - 1
				If i = bins_Conflict.Length - 1 Then
					bins_Conflict(i) = maxValue
				Else
					bins_Conflict(i) = minValue + i * [step]
				End If
			Next i

			binCounts = New Long(nBins - 1){}
		End Sub


		Public Overridable Function add(ByVal w As Writable) As HistogramCounter Implements HistogramCounter.add
			Dim d As Double = w.toDouble()

			'Not super efficient, but linear search on 20-50 items should be good enough
			Dim idx As Integer = -1
			For i As Integer = 0 To nBins - 1
				If d >= bins_Conflict(i) AndAlso d < bins_Conflict(i + 1) Then
					idx = i
					Exit For
				End If
			Next i
			If idx = -1 Then
				idx = nBins - 1
			End If

			binCounts(idx) += 1

			Return Me
		End Function

		Public Overridable Function merge(ByVal other As HistogramCounter) As DoubleHistogramCounter
			If other Is Nothing Then
				Return Me
			End If
			If Not (TypeOf other Is DoubleHistogramCounter) Then
				Throw New System.ArgumentException("Cannot merge " & other)
			End If

			Dim o As DoubleHistogramCounter = DirectCast(other, DoubleHistogramCounter)

			If minValue <> o.minValue OrElse maxValue <> o.maxValue Then
				Throw New System.InvalidOperationException("Min/max values differ: (" & minValue & "," & maxValue & ") " & " vs. (" & o.minValue & "," & o.maxValue & ")")
			End If
			If nBins <> o.nBins Then
				Throw New System.InvalidOperationException("Different number of bins: " & nBins & " vs " & o.nBins)
			End If

			For i As Integer = 0 To nBins - 1
				binCounts(i) += o.binCounts(i)
			Next i

			Return Me
		End Function

		Public Overridable ReadOnly Property Bins As Double() Implements HistogramCounter.getBins
			Get
				Return bins_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Counts As Long() Implements HistogramCounter.getCounts
			Get
				Return binCounts
			End Get
		End Property
	End Class

End Namespace