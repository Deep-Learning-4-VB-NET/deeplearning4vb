Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports org.datavec.api.transform.analysis
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

Namespace org.datavec.api.transform.analysis.counter

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class StringAnalysisCounter implements org.datavec.api.transform.analysis.AnalysisCounter<StringAnalysisCounter>
	Public Class StringAnalysisCounter
		Implements AnalysisCounter(Of StringAnalysisCounter)

		Private counter As New StatCounter()
		Private countZeroLength As Long = 0
		Private countMinLength As Long = 0
		Private countMaxLength As Long = 0

		Public Sub New()
		End Sub

		Public Overridable ReadOnly Property MinLengthSeen As Integer
			Get
				Return CInt(Math.Truncate(counter.Min))
			End Get
		End Property

		Public Overridable ReadOnly Property MaxLengthSeen As Integer
			Get
				Return CInt(Math.Truncate(counter.Max))
			End Get
		End Property

		Public Overridable ReadOnly Property SumLength As Long
			Get
				Return CLng(Math.Truncate(counter.Sum))
			End Get
		End Property

		Public Overridable ReadOnly Property CountTotal As Long
			Get
				Return counter.Count
			End Get
		End Property

		Public Overridable ReadOnly Property SampleStdev As Double
			Get
				Return counter.getStddev(False)
			End Get
		End Property

		Public Overridable ReadOnly Property Mean As Double
			Get
				Return counter.Mean
			End Get
		End Property

		Public Overridable ReadOnly Property SampleVariance As Double
			Get
				Return counter.getVariance(False)
			End Get
		End Property

		Public Overridable Function add(ByVal writable As Writable) As StringAnalysisCounter
			Dim length As Integer = writable.ToString().Length

			If length = 0 Then
				countZeroLength += 1
			End If

			If length = MinLengthSeen Then
				countMinLength += 1
			ElseIf length < MinLengthSeen Then
				countMinLength = 1
			End If

			If length = MaxLengthSeen Then
				countMaxLength += 1
			ElseIf length > MaxLengthSeen Then
				countMaxLength = 1
			End If

			counter.add(CDbl(length))

			Return Me
		End Function

		Public Overridable Function merge(ByVal other As StringAnalysisCounter) As StringAnalysisCounter
			Dim otherMin As Integer = other.MinLengthSeen
			Dim newCountMinLength As Long
			If MinLengthSeen = otherMin Then
				newCountMinLength = countMinLength + other.getCountMinLength()
			ElseIf MinLengthSeen > otherMin Then
				'Keep other, take count from other
				newCountMinLength = other.getCountMinLength()
			Else
				'Keep this min, no change to count
				newCountMinLength = countMinLength
			End If

			Dim otherMax As Integer = other.MaxLengthSeen
			Dim newCountMaxLength As Long
			If MaxLengthSeen = otherMax Then
				newCountMaxLength = countMaxLength + other.getCountMaxLength()
			ElseIf MaxLengthSeen < otherMax Then
				'Keep other, take count from other
				newCountMaxLength = other.getCountMaxLength()
			Else
				'Keep this max, no change to count
				newCountMaxLength = countMaxLength
			End If

			Return New StringAnalysisCounter(counter.merge(other.getCounter()), countZeroLength + other.getCountZeroLength(), newCountMinLength, newCountMaxLength)
		End Function
	End Class

End Namespace