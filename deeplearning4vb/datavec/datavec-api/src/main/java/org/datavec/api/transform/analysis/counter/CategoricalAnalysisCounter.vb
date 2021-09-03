Imports System.Collections.Generic
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
'ORIGINAL LINE: @AllArgsConstructor @Data public class CategoricalAnalysisCounter implements org.datavec.api.transform.analysis.AnalysisCounter<CategoricalAnalysisCounter>
	Public Class CategoricalAnalysisCounter
		Implements AnalysisCounter(Of CategoricalAnalysisCounter)

		Private counts As IDictionary(Of String, Long) = New Dictionary(Of String, Long)()
		Private countTotal As Long = 0



		Public Sub New()

		End Sub


		Public Overridable Function add(ByVal writable As Writable) As CategoricalAnalysisCounter
			Dim value As String = writable.ToString()

			Dim newCount As Long = 0
			If counts.ContainsKey(value) Then
				newCount = counts(value)
			End If
			newCount += 1
			counts(value) = newCount

			countTotal += 1

			Return Me
		End Function

		Public Overridable Function merge(ByVal other As CategoricalAnalysisCounter) As CategoricalAnalysisCounter
			Dim combinedKeySet As ISet(Of String) = New HashSet(Of String)(counts.Keys)
			combinedKeySet.addAll(other.counts.Keys)

			For Each s As String In combinedKeySet
				Dim count As Long = 0
				If counts.ContainsKey(s) Then
					count += counts(s)
				End If
				If other.counts.ContainsKey(s) Then
					count += other.counts(s)
				End If
				counts(s) = count
			Next s

			countTotal += other.countTotal

			Return Me
		End Function

	End Class

End Namespace