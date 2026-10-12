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

Namespace org.datavec.spark.transform.analysis.seqlength

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class SequenceLengthAnalysisCounter implements org.datavec.api.transform.analysis.AnalysisCounter<SequenceLengthAnalysisCounter>
	Public Class SequenceLengthAnalysisCounter
		Implements AnalysisCounter(Of SequenceLengthAnalysisCounter)

		Private countZeroLength As Long
		Private countOneLength As Long
		Private countMinLength As Long
		Private minLengthSeen As Integer = Integer.MaxValue
		Private countMaxLength As Long
		Private maxLengthSeen As Integer = Integer.MinValue
		Private countTotal As Long
		Private mean As Double


		Public Sub New()

		End Sub

		Public Overridable Function add(ByVal writable As Writable) As SequenceLengthAnalysisCounter
			Return Me
		End Function

		Public Overridable Function merge(ByVal other As SequenceLengthAnalysisCounter) As SequenceLengthAnalysisCounter
			Dim otherMin As Integer = other.getMinLengthSeen()
			Dim newMinLengthSeen As Integer
			Dim newCountMinValue As Long
			If minLengthSeen = otherMin Then
				newMinLengthSeen = minLengthSeen
				newCountMinValue = countMinLength + other.countMinLength
			ElseIf minLengthSeen > otherMin Then
				'Keep other, take count from other
				newMinLengthSeen = otherMin
				newCountMinValue = other.countMinLength
			Else
				'Keep this min, no change to count
				newMinLengthSeen = minLengthSeen
				newCountMinValue = countMinLength
			End If

			Dim otherMax As Integer = other.getMaxLengthSeen()
			Dim newMaxLengthSeen As Integer
			Dim newCountMaxValue As Long
			If maxLengthSeen = otherMax Then
				newMaxLengthSeen = maxLengthSeen
				newCountMaxValue = countMaxLength + other.countMaxLength
			ElseIf maxLengthSeen < otherMax Then
				'Keep other, take count from other
				newMaxLengthSeen = otherMax
				newCountMaxValue = other.countMaxLength
			Else
				'Keep this max, no change to count
				newMaxLengthSeen = maxLengthSeen
				newCountMaxValue = countMaxLength
			End If

			'Calculate the new mean, in an online fashion:
			Dim newCountTotal As Long = countTotal + other.countTotal
			Dim sum As Double = countTotal * mean + other.countTotal * other.mean
			Dim newMean As Double = sum / newCountTotal


			Return New SequenceLengthAnalysisCounter(countZeroLength + other.countZeroLength, countOneLength + other.countOneLength, newCountMinValue, newMinLengthSeen, newCountMaxValue, newMaxLengthSeen, newCountTotal, newMean)
		End Function

	End Class

End Namespace