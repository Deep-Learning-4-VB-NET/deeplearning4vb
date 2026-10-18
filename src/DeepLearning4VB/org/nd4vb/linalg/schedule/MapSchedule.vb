Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.Json.Serialization

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

Namespace org.nd4vb.linalg.schedule

    ''' <summary>
    ''' A schedule given by a map from iteration (or epoch) to value: the value of the largest key not greater than
    ''' the position applies. The map must contain the key 0. Use <see cref="MapSchedule.Builder"/> to build one.
    ''' <c>allKeysSorted</c> is not part of the JSON form (Java <c>@JsonIgnoreProperties({"allKeysSorted"})</c>).
    ''' </summary>
    <Serializable>
    Public Class MapSchedule
        Implements ISchedule

        Private ReadOnly scheduleType As ScheduleType
        Private values As IDictionary(Of Integer, Double)
        Private allKeysSorted As Integer()

        <JsonConstructor>
        Public Sub New(ByVal scheduleType As ScheduleType, ByVal values As IDictionary(Of Integer, Double))
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values), "values is marked non-null but is null")
            If Not values.ContainsKey(0) Then
                Throw New ArgumentException("Invalid set of values: must contain initial value (position 0)")
            End If
            Me.scheduleType = scheduleType
            Me.values = values

            Me.allKeysSorted = values.Keys.ToArray()
            Array.Sort(allKeysSorted)
        End Sub

        Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
            Dim i As Integer = If(scheduleType = ScheduleType.ITERATION, iteration, epoch)

            Dim found As Double
            If values.TryGetValue(i, found) Then
                Return found
            End If

            ' Key doesn't exist - find nearest key...
            If i >= allKeysSorted(allKeysSorted.Length - 1) Then
                Return values(allKeysSorted(allKeysSorted.Length - 1))
            End If
            ' BinarySearch returns (-(insertion point) - 1) for a missing key; the insertion point is the index of the
            ' first element greater than the key
            Dim pt As Integer = Array.BinarySearch(allKeysSorted, i)
            Dim iPt As Integer = -(pt + 1)
            Return values(allKeysSorted(iPt - 1))
        End Function

        Public Overridable Function clone() As ISchedule Implements ISchedule.clone
            Return New MapSchedule(scheduleType, values)
        End Function

        Public Overridable Function getScheduleType() As ScheduleType
            Return scheduleType
        End Function

        Public Overridable Function getValues() As IDictionary(Of Integer, Double)
            Return values
        End Function

        Public Overridable Sub setValues(ByVal values As IDictionary(Of Integer, Double))
            Me.values = values
        End Sub

        Public Overridable Function getAllKeysSorted() As Integer()
            Return allKeysSorted
        End Function

        Public Overridable Sub setAllKeysSorted(ByVal allKeysSorted As Integer())
            Me.allKeysSorted = allKeysSorted
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As MapSchedule = TryCast(obj, MapSchedule)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return scheduleType.Equals(o.scheduleType) AndAlso mapEquals(values, o.values) AndAlso Arrays.equals(allKeysSorted, o.allKeysSorted)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + scheduleType.GetHashCode()
            h = h * 59 + If(values Is Nothing, 43, values.Count)
            h = h * 59 + Arrays.hashCode(allKeysSorted)
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "MapSchedule(" & "scheduleType=" & scheduleType.ToString() & ", values=" & If(values Is Nothing, "null", "{" & String.Join(", ", values.Select(Function(kv) Arrays.FormatElement(kv.Key) & "=" & Arrays.FormatElement(kv.Value))) & "}") & ", allKeysSorted=" & Arrays.toString(allKeysSorted) & ")"
        End Function

        Private Shared Function mapEquals(ByVal a As IDictionary(Of Integer, Double), ByVal b As IDictionary(Of Integer, Double)) As Boolean
            If a Is b Then Return True
            If a Is Nothing OrElse b Is Nothing OrElse a.Count <> b.Count Then Return False
            For Each kv As KeyValuePair(Of Integer, Double) In a
                Dim other As Double
                If Not b.TryGetValue(kv.Key, other) OrElse Not kv.Value.Equals(other) Then Return False
            Next
            Return True
        End Function

        ''' <summary>Builder for conveniently constructing map schedules.</summary>
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
            ''' <param name="value">    Value for that iteration/epoch </param>
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
