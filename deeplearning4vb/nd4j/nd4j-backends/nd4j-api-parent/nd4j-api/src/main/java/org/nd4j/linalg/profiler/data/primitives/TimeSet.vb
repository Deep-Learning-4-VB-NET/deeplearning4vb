Imports System
Imports System.Collections.Generic

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

Namespace org.nd4j.linalg.profiler.data.primitives



	Public Class TimeSet
		Implements IComparable(Of TimeSet)

		Private times As IList(Of ComparableAtomicLong) = New List(Of ComparableAtomicLong)()
'JAVA TO VB CONVERTER NOTE: The field sum was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private sum_Conflict As Long = 0

		Public Overridable Sub addTime(ByVal time As Long)
			times.Add(New ComparableAtomicLong(time))
			sum_Conflict = 0
		End Sub

		Public Overridable ReadOnly Property Sum As Long
			Get
				If sum_Conflict = 0 Then
					For Each time As ComparableAtomicLong In times
						sum_Conflict += time.get()
					Next time
				End If
    
				Return sum_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Average As Long
			Get
				If times.Count = 0 Then
					Return 0L
				End If
    
				Dim tSum As Long = Sum
				Return tSum \ times.Count
			End Get
		End Property

		Public Overridable ReadOnly Property Median As Long
			Get
				If times.Count = 0 Then
					Return 0L
				End If
    
				Return times(times.Count \ 2).longValue()
			End Get
		End Property

		Public Overridable ReadOnly Property Minimum As Long
			Get
				Dim min As Long = Long.MaxValue
				For Each time As ComparableAtomicLong In times
					If time.get() < min Then
						min = time.get()
					End If
				Next time
    
				Return min
			End Get
		End Property

		Public Overridable ReadOnly Property Maximum As Long
			Get
				Dim max As Long = Long.MinValue
				For Each time As ComparableAtomicLong In times
					If time.get() > max Then
						max = time.get()
					End If
				Next time
    
				Return max
			End Get
		End Property

		Public Overridable Function size() As Integer
			Return times.Count
		End Function


		Public Overridable Function CompareTo(ByVal o As TimeSet) As Integer Implements IComparable(Of TimeSet).CompareTo
			Return Long.compare(o.Sum, Me.Sum)
		End Function
	End Class

End Namespace