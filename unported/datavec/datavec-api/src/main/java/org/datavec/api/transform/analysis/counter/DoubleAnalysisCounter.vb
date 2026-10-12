Imports TDigest = com.tdunning.math.stats.TDigest
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
'ORIGINAL LINE: @AllArgsConstructor @Data public class DoubleAnalysisCounter implements org.datavec.api.transform.analysis.AnalysisCounter<DoubleAnalysisCounter>
	Public Class DoubleAnalysisCounter
		Implements AnalysisCounter(Of DoubleAnalysisCounter)

		Private counter As New StatCounter()
		Private countZero As Long = 0
		Private countMinValue As Long = 0
		Private countMaxValue As Long = 0
		Private countPositive As Long = 0
		Private countNegative As Long = 0
		Private countNaN As Long = 0
	  ''' <summary>
	  ''' A histogram structure that will record a sketch of a distribution.
	  ''' 
	  ''' The compression argument regulates how accuracy should be traded for size? A value of N here
	  ''' will give quantile errors almost always less than 3/N with considerably smaller errors expected
	  ''' for extreme quantiles. Conversely, you should expect to track about 5 N centroids for this
	  ''' accuracy.
	  ''' </summary>
	  Private digest As TDigest = TDigest.createDigest(100)


		Public Sub New()
		End Sub

		Public Overridable ReadOnly Property MinValueSeen As Double
			Get
				Return counter.Min
			End Get
		End Property

		Public Overridable ReadOnly Property MaxValueSeen As Double
			Get
				Return counter.Max
			End Get
		End Property

		Public Overridable ReadOnly Property Sum As Double
			Get
				Return counter.Sum
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

		Public Overridable Function add(ByVal writable As Writable) As DoubleAnalysisCounter
			Dim value As Double = writable.toDouble()

			If value = 0 Then
				countZero += 1
			End If

			If value = Double.NaN Then
				countNaN += 1
			End If

			If value = MinValueSeen Then
				countMinValue += 1
			ElseIf value < MinValueSeen Then
				countMinValue = 1
			End If

			If value = MaxValueSeen Then
				countMaxValue += 1
			ElseIf value > MaxValueSeen Then
				countMaxValue = 1
			End If

			If value >= 0 Then
				countPositive += 1
			Else
				countNegative += 1
			End If

			digest.add(value)
			counter.add(value)

			Return Me
		End Function

		Public Overridable Function merge(ByVal other As DoubleAnalysisCounter) As DoubleAnalysisCounter
			Dim otherMin As Double = other.MinValueSeen
			Dim newCountMinValue As Long
			If MinValueSeen = otherMin Then
				newCountMinValue = countMinValue + other.getCountMinValue()
			ElseIf MinValueSeen > otherMin Then
				'Keep other, take count from othergetSampleStdDev
				newCountMinValue = other.getCountMinValue()
			Else
				'Keep this min, no change to count
				newCountMinValue = countMinValue
			End If

			Dim otherMax As Double = other.MaxValueSeen
			Dim newCountMaxValue As Long
			If MaxValueSeen = otherMax Then
				newCountMaxValue = countMaxValue + other.getCountMaxValue()
			ElseIf MaxValueSeen < otherMax Then
				'Keep other, take count from other
				newCountMaxValue = other.getCountMaxValue()
			Else
				'Keep this max, no change to count
				newCountMaxValue = countMaxValue
			End If

			digest.add(other.getDigest())

			Return New DoubleAnalysisCounter(counter.merge(other.getCounter()), countZero + other.getCountZero(), newCountMinValue, newCountMaxValue, countPositive + other.getCountPositive(), countNegative + other.getCountNegative(), countNaN + other.getCountNaN(), digest)
		End Function
	End Class

End Namespace