Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports org.nd4j.common.primitives

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

Namespace org.nd4j.common.collection


	''' <summary>
	''' Multiple key map
	''' </summary>
	<Serializable>
	Public Class MultiDimensionalMap(Of K, T, V)

		Private backedMap As IDictionary(Of Pair(Of K, T), V)

		''' <summary>
		''' Thread safe sorted map implementation </summary>
		''' @param <K> </param>
		''' @param <T> </param>
		''' @param <V>
		''' @return </param>
		Public Shared Function newThreadSafeTreeBackedMap(Of K, T, V)() As MultiDimensionalMap(Of K, T, V)
			Return New MultiDimensionalMap(Of K, T, V)(New ConcurrentSkipListMap(Of Pair(Of K, T), V)())
		End Function

		''' <summary>
		''' Thread safe hash map implementation </summary>
		''' @param <K> </param>
		''' @param <T> </param>
		''' @param <V>
		''' @return </param>
		Public Shared Function newThreadSafeHashBackedMap(Of K, T, V)() As MultiDimensionalMap(Of K, T, V)
			Return New MultiDimensionalMap(Of K, T, V)(New ConcurrentDictionary(Of Pair(Of K, T), V)())
		End Function

		''' <summary>
		''' Thread safe hash map impl </summary>
		''' @param <K> </param>
		''' @param <T> </param>
		''' @param <V>
		''' @return </param>
		Public Shared Function newHashBackedMap(Of K, T, V)() As MultiDimensionalMap(Of K, T, V)
			Return New MultiDimensionalMap(Of K, T, V)(New Dictionary(Of Pair(Of K, T), V)())
		End Function

		''' <summary>
		''' Tree map implementation </summary>
		''' @param <K> </param>
		''' @param <T> </param>
		''' @param <V>
		''' @return </param>
		Public Shared Function newTreeBackedMap(Of K, T, V)() As MultiDimensionalMap(Of K, T, V)
			Return New MultiDimensionalMap(Of K, T, V)(New SortedDictionary(Of Pair(Of K, T), V)())
		End Function

		Public Sub New(ByVal backedMap As IDictionary(Of Pair(Of K, T), V))
			Me.backedMap = backedMap
		End Sub

		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the number of key-value mappings in this map.  If the
		''' map contains more than <tt>Integer.MAX_VALUE</tt> elements, returns
		''' <tt>Integer.MAX_VALUE</tt>.
		''' </summary>
		''' <returns> the number of key-value mappings in this map </returns>
		Public Overridable Function size() As Integer
			Return backedMap.Count
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this map contains no key-value mappings.
		''' </summary>
		''' <returns> <tt>true</tt> if this map contains no key-value mappings </returns>
		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return backedMap.Count = 0
			End Get
		End Property

		''' <summary>
		''' Returns <tt>true</tt> if this map contains a mapping for the specified
		''' key.  More formally, returns <tt>true</tt> if and only if
		''' this map contains a mapping for a key <tt>k</tt> such that
		''' <tt>(key==null ? k==null : key.equals(k))</tt>.  (There can be
		''' at most one such mapping.)
		''' </summary>
		''' <param name="key"> key whose presence in this map is to be tested </param>
		''' <returns> <tt>true</tt> if this map contains a mapping for the specified
		''' key </returns>
		''' <exception cref="ClassCastException">   if the key is of an inappropriate type for
		'''                              this map
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if the specified key is null and this map
		'''                              does not permit null keys
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>

		Public Overridable Function containsKey(ByVal key As Object) As Boolean
			Return backedMap.ContainsKey(key)
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this map maps one or more keys to the
		''' specified value.  More formally, returns <tt>true</tt> if and only if
		''' this map contains at least one mapping to a value <tt>v</tt> such that
		''' <tt>(value==null ? v==null : value.equals(v))</tt>.  This operation
		''' will probably require time linear in the map size for most
		''' implementations of the <tt>Map</tt> interface.
		''' </summary>
		''' <param name="value"> value whose presence in this map is to be tested </param>
		''' <returns> <tt>true</tt> if this map maps one or more keys to the
		''' specified value </returns>
		''' <exception cref="ClassCastException">   if the value is of an inappropriate type for
		'''                              this map
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if the specified value is null and this
		'''                              map does not permit null values
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>

		Public Overridable Function containsValue(ByVal value As Object) As Boolean
			Return backedMap.ContainsValue(value)
		End Function

		''' <summary>
		''' Returns the value to which the specified key is mapped,
		''' or {@code null} if this map contains no mapping for the key.
		''' <p/>
		''' <para>More formally, if this map contains a mapping from a key
		''' {@code k} to a value {@code v} such that {@code (key==null ? k==null :
		''' key.equals(k))}, then this method returns {@code v}; otherwise
		''' it returns {@code null}.  (There can be at most one such mapping.)
		''' <p/>
		''' </para>
		''' <para>If this map permits null values, then a return value of
		''' {@code null} does not <i>necessarily</i> indicate that the map
		''' contains no mapping for the key; it's also possible that the map
		''' explicitly maps the key to {@code null}.  The {@link #containsKey
		''' containsKey} operation may be used to distinguish these two cases.
		''' 
		''' </para>
		''' </summary>
		''' <param name="key"> the key whose associated value is to be returned </param>
		''' <returns> the value to which the specified key is mapped, or
		''' {@code null} if this map contains no mapping for the key </returns>
		''' <exception cref="ClassCastException">   if the key is of an inappropriate type for
		'''                              this map
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if the specified key is null and this map
		'''                              does not permit null keys
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>

		Public Overridable Function get(ByVal key As Object) As V
			Return backedMap(key)
		End Function

		''' <summary>
		''' Associates the specified value with the specified key in this map
		''' (optional operation).  If the map previously contained a mapping for
		''' the key, the old value is replaced by the specified value.  (A map
		''' <tt>m</tt> is said to contain a mapping for a key <tt>k</tt> if and only
		''' if <seealso cref="containsKey(Object) m.containsKey(k)"/> would return
		''' <tt>true</tt>.)
		''' </summary>
		''' <param name="key">   key with which the specified value is to be associated </param>
		''' <param name="value"> value to be associated with the specified key </param>
		''' <returns> the previous value associated with <tt>key</tt>, or
		''' <tt>null</tt> if there was no mapping for <tt>key</tt>.
		''' (A <tt>null</tt> return can also indicate that the map
		''' previously associated <tt>null</tt> with <tt>key</tt>,
		''' if the implementation supports <tt>null</tt> values.) </returns>
		''' <exception cref="UnsupportedOperationException"> if the <tt>put</tt> operation
		'''                                       is not supported by this map </exception>
		''' <exception cref="ClassCastException">            if the class of the specified key or value
		'''                                       prevents it from being stored in this map </exception>
		''' <exception cref="NullPointerException">          if the specified key or value is null
		'''                                       and this map does not permit null keys or values </exception>
		''' <exception cref="IllegalArgumentException">      if some property of the specified key
		'''                                       or value prevents it from being stored in this map </exception>

		Public Overridable Function put(ByVal key As Pair(Of K, T), ByVal value As V) As V
				backedMap(key) = value
				Return backedMap(key)
		End Function

		''' <summary>
		''' Removes the mapping for a key from this map if it is present
		''' (optional operation).   More formally, if this map contains a mapping
		''' from key <tt>k</tt> to value <tt>v</tt> such that
		''' <code>(key==null ?  k==null : key.equals(k))</code>, that mapping
		''' is removed.  (The map can contain at most one such mapping.)
		''' <p/>
		''' <para>Returns the value to which this map previously associated the key,
		''' or <tt>null</tt> if the map contained no mapping for the key.
		''' <p/>
		''' </para>
		''' <para>If this map permits null values, then a return value of
		''' <tt>null</tt> does not <i>necessarily</i> indicate that the map
		''' contained no mapping for the key; it's also possible that the map
		''' explicitly mapped the key to <tt>null</tt>.
		''' <p/>
		''' </para>
		''' <para>The map will not contain a mapping for the specified key once the
		''' call returns.
		''' 
		''' </para>
		''' </summary>
		''' <param name="key"> key whose mapping is to be removed from the map </param>
		''' <returns> the previous value associated with <tt>key</tt>, or
		''' <tt>null</tt> if there was no mapping for <tt>key</tt>. </returns>
		''' <exception cref="UnsupportedOperationException"> if the <tt>remove</tt> operation
		'''                                       is not supported by this map </exception>
		''' <exception cref="ClassCastException">            if the key is of an inappropriate type for
		'''                                       this map
		'''                                       (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException">          if the specified key is null and this
		'''                                       map does not permit null keys
		'''                                       (<a href="Collection.html#optional-restrictions">optional</a>) </exception>

		Public Overridable Function remove(ByVal key As Object) As V
			Return backedMap.Remove(key)
		End Function

		''' <summary>
		''' Copies all of the mappings from the specified map to this map
		''' (optional operation).  The effect of this call is equivalent to that
		''' of calling <seealso cref="System.Collections.IDictionary<>.put(k, v)"/> on this map once
		''' for each mapping from key <tt>k</tt> to value <tt>v</tt> in the
		''' specified map.  The behavior of this operation is undefined if the
		''' specified map is modified while the operation is in progress.
		''' </summary>
		''' <param name="m"> mappings to be stored in this map </param>
		''' <exception cref="UnsupportedOperationException"> if the <tt>putAll</tt> operation
		'''                                       is not supported by this map </exception>
		''' <exception cref="ClassCastException">            if the class of a key or value in the
		'''                                       specified map prevents it from being stored in this map </exception>
		''' <exception cref="NullPointerException">          if the specified map is null, or if
		'''                                       this map does not permit null keys or values, and the
		'''                                       specified map contains null keys or values </exception>
		''' <exception cref="IllegalArgumentException">      if some property of a key or value in
		'''                                       the specified map prevents it from being stored in this map </exception>

		Public Overridable Sub putAll(Of T1 As Pair(Of K, T2 As V)(ByVal m As IDictionary(Of T1, T2))
			backedMap.PutAll(m)
		End Sub

		''' <summary>
		''' Removes all of the mappings from this map (optional operation).
		''' The map will be empty after this call returns.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the <tt>clear</tt> operation
		'''                                       is not supported by this map </exception>

		Public Overridable Sub clear()
			backedMap.Clear()
		End Sub

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the keys contained in this map.
		''' The applyTransformToDestination is backed by the map, so changes to the map are
		''' reflected in the applyTransformToDestination, and vice-versa.  If the map is modified
		''' while an iteration over the applyTransformToDestination is in progress (except through
		''' the iterator's own <tt>remove</tt> operation), the results of
		''' the iteration are undefined.  The applyTransformToDestination supports element removal,
		''' which removes the corresponding mapping from the map, via the
		''' <tt>Iterator.remove</tt>, <tt>Set.remove</tt>,
		''' <tt>removeAll</tt>, <tt>retainAll</tt>, and <tt>clear</tt>
		''' operations.  It does not support the <tt>add</tt> or <tt>addAll</tt>
		''' operations.
		''' </summary>
		''' <returns> a applyTransformToDestination view of the keys contained in this map </returns>

		Public Overridable Function keySet() As ISet(Of Pair(Of K, T))
			Return backedMap.Keys
		End Function

		''' <summary>
		''' Returns a <seealso cref="System.Collections.ICollection"/> view of the values contained in this map.
		''' The collection is backed by the map, so changes to the map are
		''' reflected in the collection, and vice-versa.  If the map is
		''' modified while an iteration over the collection is in progress
		''' (except through the iterator's own <tt>remove</tt> operation),
		''' the results of the iteration are undefined.  The collection
		''' supports element removal, which removes the corresponding
		''' mapping from the map, via the <tt>Iterator.remove</tt>,
		''' <tt>Collection.remove</tt>, <tt>removeAll</tt>,
		''' <tt>retainAll</tt> and <tt>clear</tt> operations.  It does not
		''' support the <tt>add</tt> or <tt>addAll</tt> operations.
		''' </summary>
		''' <returns> a collection view of the values contained in this map </returns>

		Public Overridable Function values() As ICollection(Of V)
			Return backedMap.Values
		End Function

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		''' The applyTransformToDestination is backed by the map, so changes to the map are
		''' reflected in the applyTransformToDestination, and vice-versa.  If the map is modified
		''' while an iteration over the applyTransformToDestination is in progress (except through
		''' the iterator's own <tt>remove</tt> operation, or through the
		''' <tt>setValue</tt> operation on a map entry returned by the
		''' iterator) the results of the iteration are undefined.  The applyTransformToDestination
		''' supports element removal, which removes the corresponding
		''' mapping from the map, via the <tt>Iterator.remove</tt>,
		''' <tt>Set.remove</tt>, <tt>removeAll</tt>, <tt>retainAll</tt> and
		''' <tt>clear</tt> operations.  It does not support the
		''' <tt>add</tt> or <tt>addAll</tt> operations.
		''' </summary>
		''' <returns> a applyTransformToDestination view of the mappings contained in this map </returns>

		Public Overridable Function entrySet() As ISet(Of Entry(Of K, T, V))
			Dim ret As ISet(Of Entry(Of K, T, V)) = New HashSet(Of Entry(Of K, T, V))()
			For Each pair As Pair(Of K, T) In backedMap.Keys
				ret.Add(New Entry(Of )(pair.First, pair.Second, backedMap(pair)))
			Next pair
			Return ret
		End Function

		Public Overridable Function get(ByVal k As K, ByVal t As T) As V
			Return get(New Pair(Of )(k, t))
		End Function

		Public Overridable Sub put(ByVal k As K, ByVal t As T, ByVal v As V)
			put(New Pair(Of )(k, t), v)
		End Sub


		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If Not (TypeOf o Is MultiDimensionalMap) Then
				Return False
			End If

			Dim that As MultiDimensionalMap = DirectCast(o, MultiDimensionalMap)

			Return Not (If(backedMap IsNot Nothing, Not backedMap.Equals(that.backedMap), that.backedMap IsNot Nothing))

		End Function


		Public Overrides Function GetHashCode() As Integer
			Return If(backedMap IsNot Nothing, backedMap.GetHashCode(), 0)
		End Function


		Public Overrides Function ToString() As String
			Return "MultiDimensionalMap{" & "backedMap=" & backedMap & "}"c
		End Function


		Public Overridable Function contains(ByVal k As K, ByVal t As T) As Boolean
			Return containsKey(New Pair(Of )(k, t))
		End Function


		Public Class Entry(Of K, T, V)
			Implements KeyValuePair(Of Pair(Of K, T), V)

'JAVA TO VB CONVERTER NOTE: The field firstKey was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend firstKey_Conflict As K
'JAVA TO VB CONVERTER NOTE: The field secondKey was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend secondKey_Conflict As T
'JAVA TO VB CONVERTER NOTE: The field value was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend value_Conflict As V

			Public Sub New(ByVal firstKey As K, ByVal secondKey As T, ByVal value As V)
				Me.firstKey_Conflict = firstKey
				Me.secondKey_Conflict = secondKey
				Me.value_Conflict = value
			End Sub

			Public Overridable Property FirstKey As K
				Get
					Return firstKey_Conflict
				End Get
				Set(ByVal firstKey As K)
					Me.firstKey_Conflict = firstKey
				End Set
			End Property


			Public Overridable Property SecondKey As T
				Get
					Return secondKey_Conflict
				End Get
				Set(ByVal secondKey As T)
					Me.secondKey_Conflict = secondKey
				End Set
			End Property


			Public Overridable ReadOnly Property Value As V
				Get
					Return value_Conflict
				End Get
			End Property

			''' <summary>
			''' Replaces the value corresponding to this entry with the specified
			''' value (optional operation).  (Writes through to the map.)  The
			''' behavior of this call is undefined if the mapping has already been
			''' removed from the map (by the iterator's <tt>remove</tt> operation).
			''' </summary>
			''' <param name="value"> new value to be stored in this entry </param>
			''' <returns> old value corresponding to the entry </returns>
			''' <exception cref="UnsupportedOperationException"> if the <tt>put</tt> operation
			'''                                       is not supported by the backing map </exception>
			''' <exception cref="ClassCastException">            if the class of the specified value
			'''                                       prevents it from being stored in the backing map </exception>
			''' <exception cref="NullPointerException">          if the backing map does not permit
			'''                                       null values, and the specified value is null </exception>
			''' <exception cref="IllegalArgumentException">      if some property of this value
			'''                                       prevents it from being stored in the backing map </exception>
			''' <exception cref="IllegalStateException">         implementations may, but are not
			'''                                       required to, throw this exception if the entry has been
			'''                                       removed from the backing map. </exception>

			Public Overridable Function setValue(ByVal value As V) As V
				Dim old As V = Me.value_Conflict
				Me.value_Conflict = value
				Return old
			End Function


			''' <summary>
			''' Returns the key corresponding to this entry.
			''' </summary>
			''' <returns> the key corresponding to this entry </returns>
			''' <exception cref="IllegalStateException"> implementations may, but are not
			'''                               required to, throw this exception if the entry has been
			'''                               removed from the backing map. </exception>

			Public Overridable ReadOnly Property Key As Pair(Of K, T)
				Get
					Return New Pair(Of K, T)(firstKey_Conflict, secondKey_Conflict)
				End Get
			End Property
		End Class



	End Class

End Namespace