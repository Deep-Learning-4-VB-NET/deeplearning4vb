Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports ComparableAtomicLong = org.nd4j.linalg.profiler.data.primitives.ComparableAtomicLong
Imports TimeSet = org.nd4j.linalg.profiler.data.primitives.TimeSet
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.profiler.data


	Public Class StringAggregator

		Private times As IDictionary(Of String, TimeSet) = New ConcurrentDictionary(Of String, TimeSet)()
		Private longCalls As IDictionary(Of String, ComparableAtomicLong) = New ConcurrentDictionary(Of String, ComparableAtomicLong)()

		Private Const THRESHOLD As Long = 100000

		Public Sub New()

		End Sub

		Public Overridable Sub reset()
			For Each key As String In times.Keys
				'            times.remove(key);
				times(key) = New TimeSet()
			Next key

			For Each key As String In longCalls.Keys
				'          longCalls.remove(key);
				longCalls(key) = New ComparableAtomicLong(0)
			Next key
		End Sub


		Public Overridable Sub putTime(ByVal key As String, ByVal op As Op, ByVal timeSpent As Long)
			If Not times.ContainsKey(key) Then
				times(key) = New TimeSet()
			End If

			times(key).addTime(timeSpent)

			If timeSpent > THRESHOLD Then
				Dim keyExt As String = key & " " & op.opName() & " (" & op.opNum() & ")"
				If Not longCalls.ContainsKey(keyExt) Then
					longCalls(keyExt) = New ComparableAtomicLong(0)
				End If

				longCalls(keyExt).incrementAndGet()
			End If
		End Sub

		Public Overridable Sub putTime(ByVal key As String, ByVal op As CustomOp, ByVal timeSpent As Long)
			If Not times.ContainsKey(key) Then
				times(key) = New TimeSet()
			End If

			times(key).addTime(timeSpent)

			If timeSpent > THRESHOLD Then
				Dim keyExt As String = key & " " & op.opName() & " (" & op.opHash() & ")"
				If Not longCalls.ContainsKey(keyExt) Then
					longCalls(keyExt) = New ComparableAtomicLong(0)
				End If

				longCalls(keyExt).incrementAndGet()
			End If
		End Sub

		Public Overridable Sub putTime(ByVal key As String, ByVal timeSpent As Long)
			If Not times.ContainsKey(key) Then
				times(key) = New TimeSet()
			End If

			times(key).addTime(timeSpent)
		End Sub

		Protected Friend Overridable Function getMedian(ByVal key As String) As Long
			Return times(key).getMedian()
		End Function

		Protected Friend Overridable Function getAverage(ByVal key As String) As Long
			Return times(key).getAverage()
		End Function

		Protected Friend Overridable Function getMaximum(ByVal key As String) As Long
			Return times(key).getMaximum()
		End Function

		Protected Friend Overridable Function getMinimum(ByVal key As String) As Long
			Return times(key).getMinimum()
		End Function

		Protected Friend Overridable Function getSum(ByVal key As String) As Long
			Return times(key).getSum()
		End Function

		Public Overridable Function asPercentageString() As String
			Dim builder As New StringBuilder()

			Dim sortedTimes As IDictionary(Of String, TimeSet) = ArrayUtil.sortMapByValue(times)

			Dim sum As New AtomicLong(0)
			For Each key As String In sortedTimes.Keys
				sum.addAndGet(getSum(key))
			Next key
			Dim lSum As Long = sum.get()
			builder.Append("Total time spent: ").Append(lSum \ 1000000).Append(" ms.").Append(vbLf)

			For Each key As String In sortedTimes.Keys
				Dim currentSum As Long = getSum(key)
				Dim perc As Single
				If lSum = 0 Then
					perc = 0.0f
				Else
					perc = currentSum * 100.0f / sum.get()
				End If

				Dim sumMs As Long = currentSum \ 1000000

				builder.Append(key).Append("  >>> ").Append(" perc: ").Append(perc).Append(" ").Append("Time spent: ").Append(sumMs).Append(" ms")

				builder.Append(vbLf)
			Next key

			Return builder.ToString()
		End Function

		Public Overridable Function asString() As String
			Dim builder As New StringBuilder()

			Dim sortedTimes As IDictionary(Of String, TimeSet) = ArrayUtil.sortMapByValue(times)

			For Each key As String In sortedTimes.Keys
				Dim currentMax As Long = getMaximum(key)
				Dim currentMin As Long = getMinimum(key)
				Dim currentAvg As Long = getAverage(key)
				Dim currentMed As Long = getMedian(key)

				builder.Append(key).Append("  >>> ")

				If longCalls.Count = 0 Then
					builder.Append(" ").Append(sortedTimes(key).size()).Append(" calls; ")
				End If

				builder.Append("Min: ").Append(currentMin).Append(" ns; ").Append("Max: ").Append(currentMax).Append(" ns; ").Append("Average: ").Append(currentAvg).Append(" ns; ").Append("Median: ").Append(currentMed).Append(" ns; ")

				builder.Append(vbLf)
			Next key

			builder.Append(vbLf)

			Dim sortedCalls As IDictionary(Of String, ComparableAtomicLong) = ArrayUtil.sortMapByValue(longCalls)

			For Each key As String In sortedCalls.Keys
				Dim numCalls As Long = sortedCalls(key).get()
				builder.Append(key).Append("  >>> ").Append(numCalls)

				builder.Append(vbLf)
			Next key
			builder.Append(vbLf)

			Return builder.ToString()
		End Function
	End Class

End Namespace