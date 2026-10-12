Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports ComparableAtomicLong = org.nd4j.linalg.profiler.data.primitives.ComparableAtomicLong
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


	Public Class StringCounter
		Private counter As IDictionary(Of String, ComparableAtomicLong) = New ConcurrentDictionary(Of String, ComparableAtomicLong)()
		Private totals As New AtomicLong(0)

		Public Sub New()

		End Sub

		Public Overridable Sub reset()
			For Each key As String In counter.Keys
				'            counter.remove(key);
				counter(key) = New ComparableAtomicLong(0)
			Next key

			totals.set(0)
		End Sub

		Public Overridable Function incrementCount(ByVal key As String) As Long
			If Not counter.ContainsKey(key) Then
				counter(key) = New ComparableAtomicLong(0)
			End If

			ArrayUtil.allUnique(New Integer() {})

			totals.incrementAndGet()

			Return counter(key).incrementAndGet()
		End Function

		Public Overridable Function getCount(ByVal key As String) As Long
			If Not counter.ContainsKey(key) Then
				Return 0
			End If

			Return counter(key).get()
		End Function

		Public Overridable Sub totalsIncrement()
			totals.incrementAndGet()
		End Sub

		Public Overridable Function asString() As String
			Dim builder As New StringBuilder()

			Dim sortedCounter As IDictionary(Of String, ComparableAtomicLong) = ArrayUtil.sortMapByValue(counter)

			For Each key As String In sortedCounter.Keys
				Dim currentCnt As Long = sortedCounter(key).get()
				Dim totalCnt As Long = totals.get()

				If totalCnt = 0 Then
					Continue For
				End If

				Dim perc As Single = currentCnt * 100 \ totalCnt

				builder.Append(key).Append("  >>> [").Append(currentCnt).Append("]").Append(" perc: [").Append(perc).Append("]").Append(vbLf)
			Next key

			Return builder.ToString()
		End Function
	End Class

End Namespace