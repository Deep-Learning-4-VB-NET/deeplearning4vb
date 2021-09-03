Imports System
Imports System.Collections.Generic
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
	Public Class CategoricalHistogramCounter
		Implements HistogramCounter

'JAVA TO VB CONVERTER NOTE: The field counts was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private counts_Conflict As New Dictionary(Of String, Integer)()

		Private stateNames As IList(Of String)

		Public Sub New(ByVal stateNames As IList(Of String))
			Me.stateNames = stateNames
		End Sub

		Public Overridable Function add(ByVal w As Writable) As HistogramCounter Implements HistogramCounter.add
			Dim value As String = w.ToString()
			If counts_Conflict.ContainsKey(value) Then
				counts(value) = counts(value) + 1
			Else
				counts(value) = 1
			End If
			Return Me
		End Function

		Public Overridable Function merge(ByVal other As HistogramCounter) As HistogramCounter Implements HistogramCounter.merge
			If Not (TypeOf other Is CategoricalHistogramCounter) Then
				Throw New System.ArgumentException("Input must be CategoricalHistogramCounter; got " & other)
			End If

			Dim o As CategoricalHistogramCounter = DirectCast(other, CategoricalHistogramCounter)

			For Each entry As KeyValuePair(Of String, Integer) In o.counts_Conflict.SetOfKeyValuePairs()
				Dim key As String = entry.Key
				If counts_Conflict.ContainsKey(key) Then
					counts(key) = counts(key) + entry.Value
				Else
					counts(key) = entry.Value
				End If
			Next entry

			Return Me
		End Function

		Public Overridable ReadOnly Property Bins As Double() Implements HistogramCounter.getBins
			Get
				Dim bins(stateNames.Count) As Double
				For i As Integer = 0 To bins.Length - 1
					bins(i) = i
				Next i
				Return bins
			End Get
		End Property

		Public Overridable ReadOnly Property Counts As Long() Implements HistogramCounter.getCounts
			Get
				Dim ret(stateNames.Count - 1) As Long
				Dim i As Integer = 0
				For Each s As String In stateNames
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: ret[i++] = counts.containsKey(s) ? counts.get(s) : 0;
					ret(i) = If(counts_Conflict.ContainsKey(s), counts(s), 0)
						i += 1
				Next s
				Return ret
			End Get
		End Property
	End Class

End Namespace