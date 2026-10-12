Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.nd4j.linalg.schedule


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode @JsonIgnoreProperties({"allKeysSorted"}) public class MapSchedule implements ISchedule
	<Serializable>
	Public Class MapSchedule
		Implements ISchedule

		Private scheduleType As ScheduleType
		Private values As IDictionary(Of Integer, Double)

		Private allKeysSorted() As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MapSchedule(@JsonProperty("scheduleType") @NonNull ScheduleType scheduleType, @JsonProperty("values") @NonNull Map<Integer, Double> values)
		Public Sub New(ByVal scheduleType As ScheduleType, ByVal values As IDictionary(Of Integer, Double))
			If Not values.containsKey(0) Then
				Throw New System.ArgumentException("Invalid set of values: must contain initial value (position 0)")
			End If
			Me.scheduleType = scheduleType
			Me.values = values

			Me.allKeysSorted = New Integer(values.size() - 1){}
			Dim pos As Integer = 0
			For Each i As Integer? In values.keySet()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: allKeysSorted[pos++] = i;
				allKeysSorted(pos) = i
					pos += 1
			Next i
			Array.Sort(allKeysSorted)
		End Sub

		Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
			Dim i As Integer = (If(scheduleType = ScheduleType.ITERATION, iteration, epoch))

			If values.ContainsKey(i) Then
				Return values(i)
			Else
				'Key doesn't exist - find nearest key...
				If i >= allKeysSorted(allKeysSorted.Length - 1) Then
					Return values(allKeysSorted(allKeysSorted.Length - 1))
				Else
	'                
	'                Returned:
	'                index of the search key, if it is contained in the array; otherwise, (-(insertion point) - 1). The
	'                 insertion point is defined as the point at which the key would be inserted into the array: the index
	'                  of the first element greater than the key
	'                 
					Dim pt As Integer = Array.BinarySearch(allKeysSorted, i)
					Dim iPt As Integer = -(pt + 1)
					Dim d As Double = values(allKeysSorted(iPt-1))
					Return d
				End If
			End If
		End Function

		Public Overridable Function clone() As ISchedule Implements ISchedule.clone
			Return New MapSchedule(scheduleType, values)
		End Function

		''' <summary>
		''' DynamicCustomOpsBuilder for conveniently constructing map schedules
		''' </summary>
		Public Class Builder

			Friend scheduleType As ScheduleType
			Friend values As IDictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()

			''' <param name="scheduleType"> Schedule opType to use </param>
			Public Sub New(ByVal scheduleType As ScheduleType)
				Me.scheduleType = scheduleType
			End Sub

			''' <summary>
			''' Add a single point to the map schedule. Indexes start at 0
			''' </summary>
			''' <param name="position"> Position to add (iteration or epoch index, depending on setting) </param>
			''' <param name="value">    Value for that iteraiton/epoch </param>
			Public Overridable Function add(ByVal position As Integer, ByVal value As Double) As Builder
				values(position) = value
				Return Me
			End Function

			Public Overridable Function build() As MapSchedule
				Return New MapSchedule(scheduleType, values)
			End Function
		End Class
	End Class

End Namespace