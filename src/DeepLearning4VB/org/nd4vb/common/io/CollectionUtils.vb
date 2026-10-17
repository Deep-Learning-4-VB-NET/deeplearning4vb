Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4vb.common.util

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

Namespace org.nd4vb.common.io

    ''' <summary>
    ''' Collection utilities, a copy of Spring's <c>org.springframework.util.CollectionUtils</c>.
    ''' <para>Differences from the Java original: Java's raw <c>Collection</c>/<c>Map</c> parameters are
    ''' <see cref="IEnumerable"/>/<see cref="IDictionary"/>, or the generic interfaces where elements are
    ''' added or looked up; <c>Iterator</c> and <c>Enumeration</c> are both <see cref="IEnumerator"/>, so the
    ''' two <c>contains</c> overloads collapse into one; <see cref="hasUniqueObject"/> compares boxed value
    ''' types by value; <see cref="unmodifiableMultiValueMap"/> returns a read-only view built from a
    ''' snapshot of the keys, like <c>Collections.unmodifiableMap</c> did.</para>
    ''' </summary>
    Public MustInherit Class CollectionUtils

        Public Sub New()
        End Sub

        ''' <summary>True if the collection is Nothing or has no elements (Java: <c>isEmpty(Collection)</c>).</summary>
        Public Shared Function isEmpty(ByVal collection As IEnumerable) As Boolean
            If collection Is Nothing Then
                Return True
            End If
            Dim counted As ICollection = TryCast(collection, ICollection)
            If counted IsNot Nothing Then
                Return counted.Count = 0
            End If
            Dim e As IEnumerator = collection.GetEnumerator()
            Try
                Return Not e.MoveNext()
            Finally
                Dim disposable As IDisposable = TryCast(e, IDisposable)
                If disposable IsNot Nothing Then
                    disposable.Dispose()
                End If
            End Try
        End Function

        ''' <summary>True if the map is Nothing or has no entries (Java: <c>isEmpty(Map)</c>).</summary>
        Public Shared Function isEmpty(ByVal map As IDictionary) As Boolean
            Return map Is Nothing OrElse map.Count = 0
        End Function

        Public Shared Function arrayToList(ByVal source As Object) As IList(Of Object)
            Return Arrays.asList(ObjectUtils.toObjectArray(source))
        End Function

        ''' <summary>Adds every element of the array (reference or value-type array) to the collection.</summary>
        Public Shared Sub mergeArrayIntoCollection(Of T)(ByVal array As Object, ByVal collection As ICollection(Of T))
            If collection Is Nothing Then
                Throw New ArgumentException("Collection must not be null")
            End If
            For Each elem As Object In ObjectUtils.toObjectArray(array)
                collection.Add(CType(elem, T))
            Next elem
        End Sub

        ''' <summary>Copies every property (including defaults) into the map (Java: <c>mergePropertiesIntoMap(Properties, Map)</c>).</summary>
        Public Shared Sub mergePropertiesIntoMap(Of V)(ByVal props As Properties, ByVal map As IDictionary(Of String, V))
            If map Is Nothing Then
                Throw New ArgumentException("Map must not be null")
            End If
            If props IsNot Nothing Then
                For Each key As String In props.stringPropertyNames()
                    map(key) = CType(CObj(props.getProperty(key)), V)
                Next key
            End If
        End Sub

        ''' <summary>
        ''' True if the remaining elements of the enumerator contain the element
        ''' (Java: <c>contains(Iterator, Object)</c> and <c>contains(Enumeration, Object)</c>).
        ''' </summary>
        Public Shared Function contains(ByVal iterator As IEnumerator, ByVal element As Object) As Boolean
            If iterator IsNot Nothing Then
                Do While iterator.MoveNext()
                    If ObjectUtils.nullSafeEquals(iterator.Current, element) Then
                        Return True
                    End If
                Loop
            End If
            Return False
        End Function

        ''' <summary>True if the collection contains the very same instance (reference equality).</summary>
        Public Shared Function containsInstance(ByVal collection As IEnumerable, ByVal element As Object) As Boolean
            If collection IsNot Nothing Then
                For Each candidate As Object In collection
                    If candidate Is element Then
                        Return True
                    End If
                Next candidate
            End If
            Return False
        End Function

        ''' <summary>True if any candidate is contained in the source (using the source's own <c>Contains</c>, like Java's <c>Collection.contains</c>).</summary>
        Public Shared Function containsAny(Of T)(ByVal source As ICollection(Of T), ByVal candidates As IEnumerable(Of T)) As Boolean
            If source Is Nothing OrElse source.Count = 0 OrElse candidates Is Nothing Then
                Return False
            End If
            For Each candidate As T In candidates
                If source.Contains(candidate) Then
                    Return True
                End If
            Next candidate
            Return False
        End Function

        ''' <summary>The first candidate contained in the source, or the default value of <typeparamref name="T"/> (Java: <c>null</c>).</summary>
        Public Shared Function findFirstMatch(Of T)(ByVal source As ICollection(Of T), ByVal candidates As IEnumerable(Of T)) As T
            If source Is Nothing OrElse source.Count = 0 OrElse candidates Is Nothing Then
                Return Nothing
            End If
            For Each candidate As T In candidates
                If source.Contains(candidate) Then
                    Return candidate
                End If
            Next candidate
            Return Nothing
        End Function

        ''' <summary>
        ''' The single element that is an instance of <paramref name="type"/> (any element when the type
        ''' is Nothing); Nothing when there is no such element or more than one.
        ''' </summary>
        Public Shared Function findValueOfType(ByVal collection As IEnumerable, ByVal type As Type) As Object
            If isEmpty(collection) Then
                Return Nothing
            End If
            Dim value As Object = Nothing
            For Each element As Object In collection
                If type Is Nothing OrElse type.IsInstanceOfType(element) Then
                    If value IsNot Nothing Then
                        Return Nothing
                    End If
                    value = element
                End If
            Next element
            Return value
        End Function

        ''' <summary>Generic form of <see cref="findValueOfType(IEnumerable, Type)"/> (Java: <c>Class&lt;T&gt;</c> argument).</summary>
        Public Shared Function findValueOfType(Of T)(ByVal collection As IEnumerable) As T
            Return CType(findValueOfType(collection, GetType(T)), T)
        End Function

        ''' <summary>The single value of the first type in <paramref name="types"/> that has exactly one instance in the collection.</summary>
        Public Shared Function findValueOfType(ByVal collection As IEnumerable, ByVal types() As Type) As Object
            If isEmpty(collection) OrElse ObjectUtils.isEmpty(types) Then
                Return Nothing
            End If
            For Each type As Type In types
                Dim value As Object = findValueOfType(collection, type)
                If value IsNot Nothing Then
                    Return value
                End If
            Next type
            Return Nothing
        End Function

        ''' <summary>
        ''' True if the collection holds exactly one distinct instance. Elements are compared by
        ''' reference like Java's <c>==</c>, except boxed value types which are compared by value
        ''' (every enumeration boxes them anew, so references would never match).
        ''' </summary>
        Public Shared Function hasUniqueObject(ByVal collection As IEnumerable) As Boolean
            If isEmpty(collection) Then
                Return False
            End If
            Dim hasCandidate As Boolean = False
            Dim candidate As Object = Nothing
            For Each elem As Object In collection
                If Not hasCandidate Then
                    hasCandidate = True
                    candidate = elem
                ElseIf Not sameInstance(candidate, elem) Then
                    Return False
                End If
            Next elem
            Return True
        End Function

        Private Shared Function sameInstance(ByVal a As Object, ByVal b As Object) As Boolean
            If a Is b Then
                Return True
            End If
            If a Is Nothing OrElse b Is Nothing Then
                Return False
            End If
            Return a.GetType().IsValueType AndAlso a.Equals(b)
        End Function

        ''' <summary>The runtime type shared by all non-Nothing elements, or Nothing if they differ or the collection is empty.</summary>
        Public Shared Function findCommonElementType(ByVal collection As IEnumerable) As Type
            If isEmpty(collection) Then
                Return Nothing
            End If
            Dim candidate As Type = Nothing
            For Each val As Object In collection
                If val IsNot Nothing Then
                    If candidate Is Nothing Then
                        candidate = val.GetType()
                    ElseIf candidate IsNot val.GetType() Then
                        Return Nothing
                    End If
                End If
            Next val
            Return candidate
        End Function

        ''' <summary>
        ''' Drains the enumerator (Java: <c>Enumeration</c>) into <paramref name="array"/> when it is large
        ''' enough (the element after the last one is set to Nothing, like Java's <c>Collection.toArray(T[])</c>),
        ''' otherwise into a new array.
        ''' </summary>
        Public Shared Function toArray(Of A, E As A)(ByVal enumeration As IEnumerator(Of E), ByVal array() As A) As A()
            Dim elements As New List(Of A)()
            Do While enumeration.MoveNext()
                elements.Add(enumeration.Current)
            Loop
            If array IsNot Nothing AndAlso array.Length >= elements.Count Then
                elements.CopyTo(array)
                If array.Length > elements.Count Then
                    array(elements.Count) = Nothing
                End If
                Return array
            End If
            Return elements.ToArray()
        End Function

        ''' <summary>
        ''' Adapts an enumerator (Java: <c>Enumeration</c>) to the Java <c>Iterator</c> protocol
        ''' (<c>hasNext</c>/<c>next</c>/<c>remove</c>); the result is itself an <see cref="IEnumerator(Of T)"/>.
        ''' </summary>
        Public Shared Function toIterator(Of E)(ByVal enumeration As IEnumerator(Of E)) As IEnumerator(Of E)
            Return New EnumerationIterator(Of E)(enumeration)
        End Function

        ''' <summary>Wraps the map in a <see cref="MultiValueMap(Of K, V)"/> view; changes go through to the map.</summary>
        Public Shared Function toMultiValueMap(Of K, V)(ByVal map As IDictionary(Of K, IList(Of V))) As MultiValueMap(Of K, V)
            Return New MultiValueMapAdapter(Of K, V)(map)
        End Function

        ''' <summary>
        ''' A read-only <see cref="MultiValueMap(Of K, V)"/>: the keys are copied into a new insertion-ordered
        ''' map (Java: <c>Collections.unmodifiableMap(new LinkedHashMap(...))</c>) and each value list is a
        ''' read-only view of the original list (Java: <c>Collections.unmodifiableList</c>). Mutating
        ''' operations throw <see cref="NotSupportedException"/>.
        ''' </summary>
        Public Shared Function unmodifiableMultiValueMap(Of K, V)(ByVal map As MultiValueMap(Of K, V)) As MultiValueMap(Of K, V)
            Assert.notNull(map, "'map' must not be null")
            Dim result As New LinkedHashMap(Of K, IList(Of V))(map.Count)
            For Each entry As KeyValuePair(Of K, IList(Of V)) In map
                Dim values As IList(Of V) = Nothing
                If entry.Value IsNot Nothing Then
                    values = New ReadOnlyCollection(Of V)(entry.Value)
                End If
                result.put(entry.Key, values)
            Next entry
            Return toMultiValueMap(New ReadOnlyDictionary(Of K, IList(Of V))(result))
        End Function

        ''' <summary>A <see cref="MultiValueMap(Of K, V)"/> backed by an arbitrary dictionary of lists.</summary>
        <Serializable>
        Private Class MultiValueMapAdapter(Of K, V)
            Implements MultiValueMap(Of K, V)

            Private ReadOnly map As IDictionary(Of K, IList(Of V))

            Public Sub New(ByVal map As IDictionary(Of K, IList(Of V)))
                Assert.notNull(map, "'map' must not be null")
                Me.map = map
            End Sub

            Public Overloads Sub add(ByVal key As K, ByVal value As V) Implements MultiValueMap(Of K, V).add
                Dim values As IList(Of V) = map.GetValueOrNull(key)
                If values Is Nothing Then
                    values = New List(Of V)()
                    map(key) = values
                End If
                values.Add(value)
            End Sub

            Public Function getFirst(ByVal key As K) As V Implements MultiValueMap(Of K, V).getFirst
                Dim values As IList(Of V) = map.GetValueOrNull(key)
                Return If(values IsNot Nothing AndAlso values.Count > 0, values(0), Nothing)
            End Function

            Public Sub [set](ByVal key As K, ByVal value As V) Implements MultiValueMap(Of K, V).[set]
                Dim values As IList(Of V) = New List(Of V)()
                values.Add(value)
                map(key) = values
            End Sub

            Public Sub setAll(ByVal values As IDictionary(Of K, V)) Implements MultiValueMap(Of K, V).setAll
                For Each entry As KeyValuePair(Of K, V) In values
                    [set](entry.Key, entry.Value)
                Next entry
            End Sub

            Public Function toSingleValueMap() As IDictionary(Of K, V) Implements MultiValueMap(Of K, V).toSingleValueMap
                Dim singleValueMap As New LinkedHashMap(Of K, V)(map.Count)
                For Each entry As KeyValuePair(Of K, IList(Of V)) In map
                    If entry.Value IsNot Nothing AndAlso entry.Value.Count > 0 Then
                        singleValueMap.put(entry.Key, entry.Value(0))
                    End If
                Next entry
                Return singleValueMap
            End Function

            ' ---- Java-style accessors ----

            Public Function size() As Integer
                Return map.Count
            End Function

            Public Function isEmpty() As Boolean
                Return map.Count = 0
            End Function

            Public Function [get](ByVal key As K) As IList(Of V)
                Return map.GetValueOrNull(key)
            End Function

            Public Function put(ByVal key As K, ByVal value As IList(Of V)) As IList(Of V)
                Dim previous As IList(Of V) = map.GetValueOrNull(key)
                map(key) = value
                Return previous
            End Function

            Public Function removeAndGet(ByVal key As K) As IList(Of V)
                Dim previous As IList(Of V) = map.GetValueOrNull(key)
                map.Remove(key)
                Return previous
            End Function

            Public Sub putAll(ByVal m As IDictionary(Of K, IList(Of V)))
                map.PutAll(m)
            End Sub

            Public Function containsValue(ByVal value As IList(Of V)) As Boolean
                Return map.Values.Contains(value)
            End Function

            Public Function keySet() As ISet(Of K)
                Return New LinkedHashSet(Of K)(map.Keys)
            End Function

            Public Function entrySet() As ISet(Of KeyValuePair(Of K, IList(Of V)))
                Return New LinkedHashSet(Of KeyValuePair(Of K, IList(Of V)))(map)
            End Function

            ' ---- IDictionary(Of K, IList(Of V)) ----

            Default Public Property Item(ByVal key As K) As IList(Of V) Implements IDictionary(Of K, IList(Of V)).Item
                Get
                    Return map(key)
                End Get
                Set(ByVal value As IList(Of V))
                    map(key) = value
                End Set
            End Property

            Public ReadOnly Property Keys As ICollection(Of K) Implements IDictionary(Of K, IList(Of V)).Keys
                Get
                    Return map.Keys
                End Get
            End Property

            Public ReadOnly Property Values As ICollection(Of IList(Of V)) Implements IDictionary(Of K, IList(Of V)).Values
                Get
                    Return map.Values
                End Get
            End Property

            Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Count
                Get
                    Return map.Count
                End Get
            End Property

            Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).IsReadOnly
                Get
                    Return map.IsReadOnly
                End Get
            End Property

            Public Function ContainsKey(ByVal key As K) As Boolean Implements IDictionary(Of K, IList(Of V)).ContainsKey
                Return map.ContainsKey(key)
            End Function

            Public Overloads Sub Add(ByVal key As K, ByVal value As IList(Of V)) Implements IDictionary(Of K, IList(Of V)).Add
                map.Add(key, value)
            End Sub

            Public Function Remove(ByVal key As K) As Boolean Implements IDictionary(Of K, IList(Of V)).Remove
                Return map.Remove(key)
            End Function

            Public Function TryGetValue(ByVal key As K, ByRef value As IList(Of V)) As Boolean Implements IDictionary(Of K, IList(Of V)).TryGetValue
                Return map.TryGetValue(key, value)
            End Function

            Private Sub AddPair(ByVal item As KeyValuePair(Of K, IList(Of V))) Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Add
                map.Add(item)
            End Sub

            Public Sub Clear() Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Clear
                map.Clear()
            End Sub

            Private Function ContainsPair(ByVal item As KeyValuePair(Of K, IList(Of V))) As Boolean Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Contains
                Return map.Contains(item)
            End Function

            Private Sub CopyTo(ByVal array() As KeyValuePair(Of K, IList(Of V)), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).CopyTo
                map.CopyTo(array, arrayIndex)
            End Sub

            Private Function RemovePair(ByVal item As KeyValuePair(Of K, IList(Of V))) As Boolean Implements ICollection(Of KeyValuePair(Of K, IList(Of V))).Remove
                Return map.Remove(item)
            End Function

            Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of K, IList(Of V))) Implements IEnumerable(Of KeyValuePair(Of K, IList(Of V))).GetEnumerator
                Return map.GetEnumerator()
            End Function

            Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
                Return map.GetEnumerator()
            End Function

            Public Overrides Function Equals(ByVal other As Object) As Boolean
                If Me Is other Then
                    Return True
                End If
                Dim adapter As MultiValueMapAdapter(Of K, V) = TryCast(other, MultiValueMapAdapter(Of K, V))
                If adapter IsNot Nothing Then
                    Return map.Equals(adapter.map)
                End If
                Return map.Equals(other)
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return map.GetHashCode()
            End Function

            Public Overrides Function ToString() As String
                Return map.ToString()
            End Function
        End Class

        ''' <summary>
        ''' Java <c>Iterator</c> protocol (<c>hasNext</c>/<c>next</c>/<c>remove</c>) over an enumerator;
        ''' it is also an enumerator itself so it can be used in <c>For Each</c>-style loops.
        ''' </summary>
        Private Class EnumerationIterator(Of E)
            Implements IEnumerator(Of E)

            Private ReadOnly enumeration As IEnumerator(Of E)
            Private peeked As Boolean
            Private peekedHasValue As Boolean

            Public Sub New(ByVal enumeration As IEnumerator(Of E))
                Me.enumeration = enumeration
            End Sub

            Public Function hasNext() As Boolean
                If Not peeked Then
                    peekedHasValue = enumeration.MoveNext()
                    peeked = True
                End If
                Return peekedHasValue
            End Function

            Public Function [next]() As E
                If Not hasNext() Then
                    Throw New InvalidOperationException("No more elements")
                End If
                peeked = False
                Return enumeration.Current
            End Function

            Public Sub remove()
                Throw New NotSupportedException("Not supported")
            End Sub

            Public ReadOnly Property Current As E Implements IEnumerator(Of E).Current
                Get
                    Return enumeration.Current
                End Get
            End Property

            Private ReadOnly Property CurrentNonGeneric As Object Implements IEnumerator.Current
                Get
                    Return enumeration.Current
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                If peeked Then
                    peeked = False
                    Return peekedHasValue
                End If
                Return enumeration.MoveNext()
            End Function

            Public Sub Reset() Implements IEnumerator.Reset
                peeked = False
                enumeration.Reset()
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                enumeration.Dispose()
            End Sub
        End Class
    End Class

End Namespace
