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


	Public Class MultiDimensionalSet(Of K, V)
		Implements ISet(Of Pair(Of K, V))


		Private backedSet As ISet(Of Pair(Of K, V))

		Public Sub New(ByVal backedSet As ISet(Of Pair(Of K, V)))
			Me.backedSet = backedSet
		End Sub

		Public Shared Function hashSet(Of K, V)() As MultiDimensionalSet(Of K, V)
			Return New MultiDimensionalSet(Of K, V)(New HashSet(Of Pair(Of K, V))())
		End Function


		Public Shared Function treeSet(Of K, V)() As MultiDimensionalSet(Of K, V)
			Return New MultiDimensionalSet(Of K, V)(New SortedSet(Of Pair(Of K, V))())
		End Function



		Public Shared Function concurrentSkipListSet(Of K, V)() As MultiDimensionalSet(Of K, V)
			Return New MultiDimensionalSet(Of K, V)(New ConcurrentSkipListSet(Of Pair(Of K, V))())
		End Function

		''' <summary>
		''' Returns the number of elements in this applyTransformToDestination (its cardinality).  If this
		''' applyTransformToDestination contains more than <tt>Integer.MAX_VALUE</tt> elements, returns
		''' <tt>Integer.MAX_VALUE</tt>.
		''' </summary>
		''' <returns> the number of elements in this applyTransformToDestination (its cardinality) </returns>
		Public Overrides Function size() As Integer
			Return backedSet.Count
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this applyTransformToDestination contains no elements.
		''' </summary>
		''' <returns> <tt>true</tt> if this applyTransformToDestination contains no elements </returns>
		Public Overrides ReadOnly Property Empty As Boolean
			Get
				Return backedSet.Count = 0
			End Get
		End Property

		''' <summary>
		''' Returns <tt>true</tt> if this applyTransformToDestination contains the specified element.
		''' More formally, returns <tt>true</tt> if and only if this applyTransformToDestination
		''' contains an element <tt>e</tt> such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		''' </summary>
		''' <param name="o"> element whose presence in this applyTransformToDestination is to be tested </param>
		''' <returns> <tt>true</tt> if this applyTransformToDestination contains the specified element </returns>
		''' <exception cref="ClassCastException">   if the type of the specified element
		'''                              is incompatible with this applyTransformToDestination
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if the specified element is null and this
		'''                              applyTransformToDestination does not permit null elements
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		Public Overrides Function contains(ByVal o As Object) As Boolean
			Return backedSet.Contains(o)
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this applyTransformToDestination.  The elements are
		''' returned in no particular order (unless this applyTransformToDestination is an instance of some
		''' class that provides a guarantee).
		''' </summary>
		''' <returns> an iterator over the elements in this applyTransformToDestination </returns>
		Public Overrides Function iterator() As IEnumerator(Of Pair(Of K, V))
			Return backedSet.GetEnumerator()
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this applyTransformToDestination.
		''' If this applyTransformToDestination makes any guarantees as to what order its elements
		''' are returned by its iterator, this method must return the
		''' elements in the same order.
		''' <p/>
		''' <para>The returned array will be "safe" in that no references to it
		''' are maintained by this applyTransformToDestination.  (In other words, this method must
		''' allocate a new array even if this applyTransformToDestination is backed by an array).
		''' The caller is thus free to modify the returned array.
		''' <p/>
		''' </para>
		''' <para>This method acts as bridge between array-based and collection-based
		''' APIs.
		''' 
		''' </para>
		''' </summary>
		''' <returns> an array containing all the elements in this applyTransformToDestination </returns>
		Public Overrides Function toArray() As Object()
			Return backedSet.ToArray()
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this applyTransformToDestination; the
		''' runtime type of the returned array is that of the specified array.
		''' If the applyTransformToDestination fits in the specified array, it is returned therein.
		''' Otherwise, a new array is allocated with the runtime type of the
		''' specified array and the size of this applyTransformToDestination.
		''' <p/>
		''' <para>If this applyTransformToDestination fits in the specified array with room to spare
		''' (i.e., the array has more elements than this applyTransformToDestination), the element in
		''' the array immediately following the end of the applyTransformToDestination is applyTransformToDestination to
		''' <tt>null</tt>.  (This is useful in determining the length of this
		''' applyTransformToDestination <i>only</i> if the caller knows that this applyTransformToDestination does not contain
		''' any null elements.)
		''' <p/>
		''' </para>
		''' <para>If this applyTransformToDestination makes any guarantees as to what order its elements
		''' are returned by its iterator, this method must return the elements
		''' in the same order.
		''' <p/>
		''' </para>
		''' <para>Like the <seealso cref="toArray()"/> method, this method acts as bridge between
		''' array-based and collection-based APIs.  Further, this method allows
		''' precise control over the runtime type of the output array, and may,
		''' under certain circumstances, be used to save allocation costs.
		''' <p/>
		''' </para>
		''' <para>Suppose <tt>x</tt> is a applyTransformToDestination known to contain only strings.
		''' The following code can be used to dump the applyTransformToDestination into a newly allocated
		''' array of <tt>String</tt>:
		''' <p/>
		''' <pre>
		'''     String[] y = x.toArray(new String[0]);</pre>
		''' 
		''' Note that <tt>toArray(new Object[0])</tt> is identical in function to
		''' <tt>toArray()</tt>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="a"> the array into which the elements of this applyTransformToDestination are to be
		'''          stored, if it is big enough; otherwise, a new array of the same
		'''          runtime type is allocated for this purpose. </param>
		''' <returns> an array containing all the elements in this applyTransformToDestination </returns>
		''' <exception cref="ArrayStoreException">  if the runtime type of the specified array
		'''                              is not a supertype of the runtime type of every element in this
		'''                              applyTransformToDestination </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
		Public Overrides Function toArray(Of T)(ByVal a() As T) As T()
			Return backedSet.toArray(a)
		End Function

		''' <summary>
		''' Adds the specified element to this applyTransformToDestination if it is not already present
		''' (optional operation).  More formally, adds the specified element
		''' <tt>e</tt> to this applyTransformToDestination if the applyTransformToDestination contains no element <tt>e2</tt>
		''' such that
		''' <tt>(e==null&nbsp;?&nbsp;e2==null&nbsp;:&nbsp;e.equals(e2))</tt>.
		''' If this applyTransformToDestination already contains the element, the call leaves the applyTransformToDestination
		''' unchanged and returns <tt>false</tt>.  In combination with the
		''' restriction on constructors, this ensures that sets never contain
		''' duplicate elements.
		''' <p/>
		''' <para>The stipulation above does not imply that sets must accept all
		''' elements; sets may refuse to add any particular element, including
		''' <tt>null</tt>, and throw an exception, as described in the
		''' specification for <seealso cref="Collection.add Collection.add"/>.
		''' Individual applyTransformToDestination implementations should clearly document any
		''' restrictions on the elements that they may contain.
		''' 
		''' </para>
		''' </summary>
		''' <param name="kvPair"> element to be added to this applyTransformToDestination </param>
		''' <returns> <tt>true</tt> if this applyTransformToDestination did not already contain the specified
		''' element </returns>
		''' <exception cref="UnsupportedOperationException"> if the <tt>add</tt> operation
		'''                                       is not supported by this applyTransformToDestination </exception>
		''' <exception cref="ClassCastException">            if the class of the specified element
		'''                                       prevents it from being added to this applyTransformToDestination </exception>
		''' <exception cref="NullPointerException">          if the specified element is null and this
		'''                                       applyTransformToDestination does not permit null elements </exception>
		''' <exception cref="IllegalArgumentException">      if some property of the specified element
		'''                                       prevents it from being added to this applyTransformToDestination </exception>
		Public Overrides Function add(ByVal kvPair As Pair(Of K, V)) As Boolean
			Return backedSet.Add(kvPair)
		End Function

		''' <summary>
		''' Removes the specified element from this applyTransformToDestination if it is present
		''' (optional operation).  More formally, removes an element <tt>e</tt>
		''' such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>, if
		''' this applyTransformToDestination contains such an element.  Returns <tt>true</tt> if this applyTransformToDestination
		''' contained the element (or equivalently, if this applyTransformToDestination changed as a
		''' result of the call).  (This applyTransformToDestination will not contain the element once the
		''' call returns.)
		''' </summary>
		''' <param name="o"> object to be removed from this applyTransformToDestination, if present </param>
		''' <returns> <tt>true</tt> if this applyTransformToDestination contained the specified element </returns>
		''' <exception cref="ClassCastException">            if the type of the specified element
		'''                                       is incompatible with this applyTransformToDestination
		'''                                       (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException">          if the specified element is null and this
		'''                                       applyTransformToDestination does not permit null elements
		'''                                       (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="UnsupportedOperationException"> if the <tt>remove</tt> operation
		'''                                       is not supported by this applyTransformToDestination </exception>
		Public Overrides Function remove(ByVal o As Object) As Boolean
			Return backedSet.remove(o)
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this applyTransformToDestination contains all of the elements of the
		''' specified collection.  If the specified collection is also a applyTransformToDestination, this
		''' method returns <tt>true</tt> if it is a <i>subset</i> of this applyTransformToDestination.
		''' </summary>
		''' <param name="c"> collection to be checked for containment in this applyTransformToDestination </param>
		''' <returns> <tt>true</tt> if this applyTransformToDestination contains all of the elements of the
		''' specified collection </returns>
		''' <exception cref="ClassCastException">   if the types of one or more elements
		'''                              in the specified collection are incompatible with this
		'''                              applyTransformToDestination
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if the specified collection contains one
		'''                              or more null elements and this applyTransformToDestination does not permit null
		'''                              elements
		'''                              (<a href="Collection.html#optional-restrictions">optional</a>),
		'''                              or if the specified collection is null </exception>
		''' <seealso cref= #contains(Object) </seealso>
		Public Overrides Function containsAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Return backedSet.ContainsAll(c)
		End Function

		''' <summary>
		''' Adds all of the elements in the specified collection to this applyTransformToDestination if
		''' they're not already present (optional operation).  If the specified
		''' collection is also a applyTransformToDestination, the <tt>addAll</tt> operation effectively
		''' modifies this applyTransformToDestination so that its value is the <i>union</i> of the two
		''' sets.  The behavior of this operation is undefined if the specified
		''' collection is modified while the operation is in progress.
		''' </summary>
		''' <param name="c"> collection containing elements to be added to this applyTransformToDestination </param>
		''' <returns> <tt>true</tt> if this applyTransformToDestination changed as a result of the call </returns>
		''' <exception cref="UnsupportedOperationException"> if the <tt>addAll</tt> operation
		'''                                       is not supported by this applyTransformToDestination </exception>
		''' <exception cref="ClassCastException">            if the class of an element of the
		'''                                       specified collection prevents it from being added to this applyTransformToDestination </exception>
		''' <exception cref="NullPointerException">          if the specified collection contains one
		'''                                       or more null elements and this applyTransformToDestination does not permit null
		'''                                       elements, or if the specified collection is null </exception>
		''' <exception cref="IllegalArgumentException">      if some property of an element of the
		'''                                       specified collection prevents it from being added to this applyTransformToDestination </exception>
		''' <seealso cref= #add(Object) </seealso>
		Public Overrides Function addAll(Of T1 As Pair(Of K)(ByVal c As ICollection(Of T1)) As Boolean
			Return backedSet.addAll(c)
		End Function

		''' <summary>
		''' Retains only the elements in this applyTransformToDestination that are contained in the
		''' specified collection (optional operation).  In other words, removes
		''' from this applyTransformToDestination all of its elements that are not contained in the
		''' specified collection.  If the specified collection is also a applyTransformToDestination, this
		''' operation effectively modifies this applyTransformToDestination so that its value is the
		''' <i>intersection</i> of the two sets.
		''' </summary>
		''' <param name="c"> collection containing elements to be retained in this applyTransformToDestination </param>
		''' <returns> <tt>true</tt> if this applyTransformToDestination changed as a result of the call </returns>
		''' <exception cref="UnsupportedOperationException"> if the <tt>retainAll</tt> operation
		'''                                       is not supported by this applyTransformToDestination </exception>
		''' <exception cref="ClassCastException">            if the class of an element of this applyTransformToDestination
		'''                                       is incompatible with the specified collection
		'''                                       (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException">          if this applyTransformToDestination contains a null element and the
		'''                                       specified collection does not permit null elements
		'''                                       (<a href="Collection.html#optional-restrictions">optional</a>),
		'''                                       or if the specified collection is null </exception>
		''' <seealso cref= #remove(Object) </seealso>
		Public Overrides Function retainAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Return backedSet.RetainAll(c)
		End Function

		''' <summary>
		''' Removes from this applyTransformToDestination all of its elements that are contained in the
		''' specified collection (optional operation).  If the specified
		''' collection is also a applyTransformToDestination, this operation effectively modifies this
		''' applyTransformToDestination so that its value is the <i>asymmetric applyTransformToDestination difference</i> of
		''' the two sets.
		''' </summary>
		''' <param name="c"> collection containing elements to be removed from this applyTransformToDestination </param>
		''' <returns> <tt>true</tt> if this applyTransformToDestination changed as a result of the call </returns>
		''' <exception cref="UnsupportedOperationException"> if the <tt>removeAll</tt> operation
		'''                                       is not supported by this applyTransformToDestination </exception>
		''' <exception cref="ClassCastException">            if the class of an element of this applyTransformToDestination
		'''                                       is incompatible with the specified collection
		'''                                       (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException">          if this applyTransformToDestination contains a null element and the
		'''                                       specified collection does not permit null elements
		'''                                       (<a href="Collection.html#optional-restrictions">optional</a>),
		'''                                       or if the specified collection is null </exception>
		''' <seealso cref= #remove(Object) </seealso>
		''' <seealso cref= #contains(Object) </seealso>
		Public Overrides Function removeAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Return backedSet.RemoveAll(c)
		End Function

		''' <summary>
		''' Removes all of the elements from this applyTransformToDestination (optional operation).
		''' The applyTransformToDestination will be empty after this call returns.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the <tt>clear</tt> method
		'''                                       is not supported by this applyTransformToDestination </exception>
		Public Overrides Sub clear()
			backedSet.Clear()
		End Sub



		Public Overridable Function contains(ByVal k As K, ByVal v As V) As Boolean
			Return contains(New Pair(Of )(k, v))
		End Function

		Public Overridable Sub add(ByVal k As K, ByVal v As V)
			add(New Pair(Of )(k, v))
		End Sub

	End Class

End Namespace