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
	Public Class StringHistogramCounter
		Implements HistogramCounter

		Private ReadOnly minLength As Integer
		Private ReadOnly maxLength As Integer
		Private ReadOnly nBins As Integer
'JAVA TO VB CONVERTER NOTE: The field bins was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly bins_Conflict() As Double
		Private ReadOnly binCounts() As Long

		Public Sub New(ByVal minLength As Integer, ByVal maxLength As Integer, ByVal nBins As Integer)
			Me.minLength = minLength
			Me.maxLength = maxLength
			Me.nBins = nBins

			bins_Conflict = New Double(nBins){} '+1 because bins are defined by a range of values: bins[i] to bins[i+1]
			Dim [step] As Double = (CDbl(maxLength - minLength)) / nBins
			For i As Integer = 0 To bins_Conflict.Length - 1
				If i = bins_Conflict.Length - 1 Then
					bins_Conflict(i) = maxLength
				Else
					bins_Conflict(i) = i * [step]
				End If
			Next i

			binCounts = New Long(nBins - 1){}
		End Sub


		Public Overridable Function add(ByVal w As Writable) As HistogramCounter Implements HistogramCounter.add
			Dim d As Double = w.ToString().Length

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

		Public Overridable Function merge(ByVal other As HistogramCounter) As StringHistogramCounter
			If other Is Nothing Then
				Return Me
			End If
			If Not (TypeOf other Is StringHistogramCounter) Then
				Throw New System.ArgumentException("Cannot merge " & other)
			End If

			Dim o As StringHistogramCounter = DirectCast(other, StringHistogramCounter)

			If minLength <> o.minLength OrElse maxLength <> o.maxLength Then
				Throw New System.InvalidOperationException("Min/max values differ: (" & minLength & "," & maxLength & ") " & " vs. (" & o.minLength & "," & o.maxLength & ")")
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