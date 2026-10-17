Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports DeepLearning4VB.JavaCompat

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

Namespace org.nd4vb.common.util

    ''' <summary>
    ''' A <see cref="MultiValueMap(Of K, V)"/> that keeps keys in insertion order.
    ''' </summary>
    <Serializable>
    Public Class LinkedMultiValueMap(Of K, V)
        Implements MultiValueMap(Of K, V)

        Private Const serialVersionUID As Long = 3801124242820219131L
        Private ReadOnly targetMap As LinkedHashMap(Of K, IList(Of V))

        Public Sub New()
            targetMap = New LinkedHashMap(Of K, IList(Of V))()
        End Sub

        Public Sub New(ByVal initialCapacity As Integer)
            targetMap = New LinkedHashMap(Of K, IList(Of V))(initialCapacity)
        End Sub

        Public Sub New(ByVal otherMap As IDictionary(Of K, IList(Of V)))
            targetMap = New LinkedHashMap(Of K, IList(Of V))(otherMap)
        End Sub

        Public Overloads Overridable Sub add(ByVal key As K, ByVal value As V) Implements MultiValueMap(Of K, V).add
            Dim values As IList(Of V) = targetMap.[get](key)
            If values Is Nothing Then
                values = New List(Of V)()
                targetMap.put(key, values)
            End If
            values.Add(value)
        End Sub

        Public Overridable Function getFirst(ByVal key As K) As V Implements MultiValueMap(Of K, V).getFirst
            Dim values As IList(Of V) = targetMap.[get](key)
            Return If(values IsNot Nothing AndAlso values.Count > 0, values(0), Nothing)
        End Function

        Public Overridable Sub [set](ByVal key As K, ByVal value As V) Implements MultiValueMap(Of K, V).[set]
            Dim values As IList(Of V) = New List(Of V)()
            values.Add(value)
            targetMap.put(key, values)
        End Sub

        Public Overridable Sub setAll(ByVal values As IDictionary(Of K, V)) Implements MultiValueMap(Of K, V).setAll
            For Each entry As KeyValuePair(Of K, V) In values
                [set](entry.Key, entry.Value)
            Next entry
        End Sub

        Public Overridable Function toSingleValueMap() As IDictionary(Of K, V) Implements MultiValueMap(Of K, V).toSingleValueMap
            Dim singleValueMap As New LinkedHashMap(Of K, V)(targetMap.Count)
            For Each entry As KeyValuePair(Of K, IList(Of V)) In targetMap
                If entry.Value IsNot Nothing AndAlso entry.Value.Count > 0 Then
                    singleValueMap.put(entry.Key, entry.Value(0))
                End If
            Next entry
            Return singleValueMap
        End Function

        ' ---- Java-style accessors ----

        Public Overridable Function size() As Integer
            Return targetMap.Count
        End Function

        Public Overridable Function isEmpty() As Boolean
            Return targetMap.Count = 0
        End Function

        Public Overridable Function [get](ByVal key As K) As IList(Of V)
            Return targetMap.[get](key)
        End Function

        Public Overridable Function put(ByVal key As K, ByVal value As IList(Of V)) As IList(Of V)
            Return targetMap.put(key, value)
        End Function

        Public Overridable Function removeAndGet(ByVal key As K) As IList(Of V)
            Return targetMap.removeAndGet(key)
        End Function

        Public Overridable Sub putAll(ByVal m As IDictionary(Of K, IList(Of V)))
            targetMap.putAll(m)
        End Sub

        Public Overridable Function containsValue(ByVal value As IList(Of V)) As Boolean
            Return targetMap.containsValue(value)
        End Function

        Public Overridable Function keySet() As ISet(Of K)
            Return targetMap.keySet()
        End Function

        Public Overridable Function entrySet() As ISet(Of KeyValuePair(Of K, IList(Of V)))
            Return targetMap.entrySet()
        End Function

        ' ---- IDictionary(Of K, IList(Of V)) ----

        Default Public Property Item(ByVal key As K) As IList(Of V) Implements IDictionary(Of K, IList(Of V)).Item
            Get
                Return targetMap(key)
            End Get
            Set(ByVal value As IList(Of V))
                targetMap(key) = value
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of K) Implements IDictionary(Of K, IList(Of V)).Keys
            Get
                Return targetMap.Keys
            End Get
        End Property

        Public ReadOnly Property Values As ICollection(Of IList(Of V)) Implements IDictionary(Of K, IList(Of V)).Values
            Get
                Return targetMap.Values
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Count
            Get
                Return targetMap.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function ContainsKey(ByVal key As K) As Boolean Implements IDictionary(Of K, IList(Of V)).ContainsKey
            Return targetMap.ContainsKey(key)
        End Function

        Public Overloads Sub Add(ByVal key As K, ByVal value As IList(Of V)) Implements IDictionary(Of K, IList(Of V)).Add
            targetMap.Add(key, value)
        End Sub

        Public Function Remove(ByVal key As K) As Boolean Implements IDictionary(Of K, IList(Of V)).Remove
            Return targetMap.Remove(key)
        End Function

        Public Function TryGetValue(ByVal key As K, ByRef value As IList(Of V)) As Boolean Implements IDictionary(Of K, IList(Of V)).TryGetValue
            Return targetMap.TryGetValue(key, value)
        End Function

        Private Sub AddPair(ByVal item As KeyValuePair(Of K, IList(Of V))) Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Add
            targetMap.Add(item.Key, item.Value)
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Clear
            targetMap.Clear()
        End Sub

        Private Function ContainsPair(ByVal item As KeyValuePair(Of K, IList(Of V))) As Boolean Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Contains
            Return DirectCast(targetMap, ICollection(Of KeyValuePair(Of K, IList(Of V)))).Contains(item)
        End Function

        Private Sub CopyTo(ByVal array() As KeyValuePair(Of K, IList(Of V)), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).CopyTo
            DirectCast(targetMap, ICollection(Of KeyValuePair(Of K, IList(Of V)))).CopyTo(array, arrayIndex)
        End Sub

        Private Function RemovePair(ByVal item As KeyValuePair(Of K, IList(Of V))) As Boolean Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Remove
            Return DirectCast(targetMap, ICollection(Of KeyValuePair(Of K, IList(Of V)))).Remove(item)
        End Function

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of K, IList(Of V))) Implements IEnumerable(Of KeyValuePair(Of K, IList(Of V))).GetEnumerator
            Return targetMap.GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return targetMap.GetEnumerator()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return targetMap.Equals(obj)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return targetMap.GetHashCode()
        End Function

        Public Overrides Function ToString() As String
            Return targetMap.ToString()
        End Function
    End Class

End Namespace
