Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.function
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

Namespace org.nd4j.autodiff.samediff.internal


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class AbstractDependencyTracker<T, D>
	Public MustInherit Class AbstractDependencyTracker(Of T, D)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Map<T, @Set<D>> dependencies;
		Private ReadOnly dependencies As IDictionary(Of T, ISet(Of D)) 'Key: the dependent. Value: all things that the key depends on
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Map<T, @Set<org.nd4j.common.primitives.Pair<D, D>>> orDependencies;
		Private ReadOnly orDependencies As IDictionary(Of T, ISet(Of Pair(Of D, D))) 'Key: the dependent. Value: the set of OR dependencies
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Map<D, @Set<T>> reverseDependencies = new HashMap<>();
		Private ReadOnly reverseDependencies As IDictionary(Of D, ISet(Of T)) = New Dictionary(Of D, ISet(Of T))() 'Key: the dependee. Value: The set of all dependents that depend on this value
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Map<D, @Set<T>> reverseOrDependencies = new HashMap<>();
		Private ReadOnly reverseOrDependencies As IDictionary(Of D, ISet(Of T)) = New Dictionary(Of D, ISet(Of T))()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final @Set<D> satisfiedDependencies = new HashSet<>();
		Private ReadOnly satisfiedDependencies As ISet(Of D) = New HashSet(Of D)() 'Mark the dependency as satisfied. If not in set: assumed to not be satisfied
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final @Set<T> allSatisfied;
		Private ReadOnly allSatisfied As ISet(Of T) 'Set of all dependent values (Ys) that have all dependencies satisfied
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Queue<T> allSatisfiedQueue = new LinkedList<>();
		Private ReadOnly allSatisfiedQueue As New LinkedList(Of T)() 'Queue for *new* "all satisfied" values. Values are removed using the "new all satisfied" methods


		Protected Friend Sub New()
			dependencies = CType(newTMap(), IDictionary(Of T, ISet(Of D)))
			orDependencies = CType(newTMap(), IDictionary(Of T, ISet(Of Pair(Of D, D))))
			allSatisfied = newTSet()
		End Sub

		''' <returns> A new map where the dependents (i.e., Y in "X -> Y") are the key </returns>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected abstract Map<T, ?> newTMap();
		Protected Friend MustOverride Function newTMap() As IDictionary(Of T, Object)

		''' <returns> A new set where the dependents (i.e., Y in "X -> Y") are the key </returns>
		Protected Friend MustOverride Function newTSet() As ISet(Of T)

		''' <returns> A String representation of the dependent object </returns>
		Protected Friend MustOverride Function toStringT(ByVal t As T) As String

		''' <returns> A String representation of the dependee object </returns>
		Protected Friend MustOverride Function toStringD(ByVal d As D) As String

		''' <summary>
		''' Clear all internal state for the dependency tracker
		''' </summary>
		Public Overridable Sub clear()
			dependencies.Clear()
			orDependencies.Clear()
			reverseDependencies.Clear()
			reverseOrDependencies.Clear()
			satisfiedDependencies.Clear()
			allSatisfied.Clear()
			allSatisfiedQueue.Clear()
		End Sub

		''' <returns> True if no dependencies have been defined </returns>
		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return dependencies.Count = 0 AndAlso orDependencies.Count = 0 AndAlso allSatisfiedQueue.Count = 0
			End Get
		End Property

		''' <returns> True if the dependency has been marked as satisfied using <seealso cref="markSatisfied(Object, Boolean)"/> </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public boolean isSatisfied(@NonNull D x)
		Public Overridable Function isSatisfied(ByVal x As D) As Boolean
			Return satisfiedDependencies.Contains(x)
		End Function

		''' <summary>
		''' Mark the specified value as satisfied.
		''' For example, if two dependencies have been previously added (X -> Y) and (X -> A) then after the markSatisfied(X, true)
		''' call, both of these dependencies are considered satisfied.
		''' </summary>
		''' <param name="x">         Value to mark </param>
		''' <param name="satisfied"> Whether to mark as satisfied (true) or unsatisfied (false) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void markSatisfied(@NonNull D x, boolean satisfied)
		Public Overridable Sub markSatisfied(ByVal x As D, ByVal satisfied As Boolean)
			If satisfied Then
				Dim alreadySatisfied As Boolean = satisfiedDependencies.Contains(x)

				If Not alreadySatisfied Then
					satisfiedDependencies.Add(x)

					'Check if any Y's exist that have dependencies that are all satisfied, for X -> Y
					Dim s As ISet(Of T) = reverseDependencies(x)
					Dim s2 As ISet(Of T) = reverseOrDependencies(x)

					Dim set As ISet(Of T)
					If s IsNot Nothing AndAlso s2 IsNot Nothing Then
						set = newTSet()
						set.addAll(s)
						set.addAll(s2)
					ElseIf s IsNot Nothing Then
						set = s
					ElseIf s2 IsNot Nothing Then
						set = s2
					Else
						If log.isTraceEnabled() Then
							log.trace("No values depend on: {}", toStringD(x))
						End If
						Return
					End If

					For Each t As T In set
						Dim required As ISet(Of D) = dependencies(t)
						Dim requiredOr As ISet(Of Pair(Of D, D)) = orDependencies(t)
						Dim allSatisfied As Boolean = True
						If required IsNot Nothing Then
							For Each d As D In required
								If Not isSatisfied(d) Then
									allSatisfied = False
									Exit For
								End If
							Next d
						End If
						If allSatisfied AndAlso requiredOr IsNot Nothing Then
							For Each p As Pair(Of D, D) In requiredOr
								If Not isSatisfied(p.First) AndAlso Not isSatisfied(p.Second) Then
									allSatisfied = False
									Exit For
								End If
							Next p
						End If

						If allSatisfied AndAlso Not Me.allSatisfied.Contains(t) Then
							Me.allSatisfied.Add(t)
							Me.allSatisfiedQueue.AddLast(t)
						End If
					Next t
				End If

			Else
				satisfiedDependencies.remove(x)
				If allSatisfied.Count > 0 Then

					Dim reverse As ISet(Of T) = reverseDependencies(x)
					If reverse IsNot Nothing Then
						For Each y As T In reverse
							If allSatisfied.Contains(y) Then
								allSatisfied.remove(y)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
								allSatisfiedQueue.remove(y)
							End If
						Next y
					End If
					Dim orReverse As ISet(Of T) = reverseOrDependencies(x)
					If orReverse IsNot Nothing Then
						For Each y As T In orReverse
							If allSatisfied.Contains(y) AndAlso Not isAllSatisfied(y) Then
								allSatisfied.remove(y)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
								allSatisfiedQueue.remove(y)
							End If
						Next y
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Check whether any dependencies x -> y exist, for y (i.e., anything previously added by <seealso cref="addDependency(Object, Object)"/>
		''' or <seealso cref="addOrDependency(Object, Object, Object)"/>
		''' </summary>
		''' <param name="y"> Dependent to check </param>
		''' <returns> True if Y depends on any values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public boolean hasDependency(@NonNull T y)
		Public Overridable Function hasDependency(ByVal y As T) As Boolean
			Dim s1 As ISet(Of D) = dependencies(y)
			If s1 IsNot Nothing AndAlso s1.Count > 0 Then
				Return True
			End If

			Dim s2 As ISet(Of Pair(Of D, D)) = orDependencies(y)
			Return s2 IsNot Nothing AndAlso s2.Count > 0
		End Function

		''' <summary>
		''' Get all dependencies x, for x -> y, and (x1 or x2) -> y
		''' </summary>
		''' <param name="y"> Dependent to get dependencies for </param>
		''' <returns> List of dependencies </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DependencyList<T, D> getDependencies(@NonNull T y)
		Public Overridable Function getDependencies(ByVal y As T) As DependencyList(Of T, D)
			Dim s1 As ISet(Of D) = dependencies(y)
			Dim s2 As ISet(Of Pair(Of D, D)) = orDependencies(y)

			Dim l1 As IList(Of D) = (If(s1 Is Nothing, Nothing, New List(Of D)(s1)))
			Dim l2 As IList(Of Pair(Of D, D)) = (If(s2 Is Nothing, Nothing, New List(Of Pair(Of D, D))(s2)))

			Return New DependencyList(Of T, D)(y, l1, l2)
		End Function

		''' <summary>
		''' Add a dependency: y depends on x, as in x -> y
		''' </summary>
		''' <param name="y"> The dependent </param>
		''' <param name="x"> The dependee that is required for Y </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addDependency(@NonNull T y, @NonNull D x)
		Public Overridable Sub addDependency(ByVal y As T, ByVal x As D)
			If Not dependencies.ContainsKey(y) Then
				dependencies(y) = New HashSet(Of D)()
			End If

			If Not reverseDependencies.ContainsKey(x) Then
				reverseDependencies(x) = newTSet()
			End If

			dependencies(y).Add(x)
			reverseDependencies(x).Add(y)

			checkAndUpdateIfAllSatisfied(y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void checkAndUpdateIfAllSatisfied(@NonNull T y)
		Protected Friend Overridable Sub checkAndUpdateIfAllSatisfied(ByVal y As T)
			Dim allSat As Boolean = isAllSatisfied(y)
			If allSat Then
				'Case where "x is satisfied" happened before x->y added
				If Not allSatisfied.Contains(y) Then
					allSatisfied.Add(y)
					allSatisfiedQueue.AddLast(y)
				End If
			ElseIf allSatisfied.Contains(y) Then
				If Not allSatisfiedQueue.Contains(y) Then
					Dim sb As New StringBuilder()
					sb.Append("Dependent object """).Append(toStringT(y)).Append(""" was previously processed after all dependencies").Append(" were marked satisfied, but is now additional dependencies have been added." & vbLf)
					Dim dl As DependencyList(Of T, D) = getDependencies(y)
					If dl.getDependencies() IsNot Nothing Then
						sb.Append("Dependencies:" & vbLf)
						For Each d As D In dl.getDependencies()
							sb.Append(d).Append(" - ").Append(If(isSatisfied(d), "Satisfied", "Not satisfied")).Append(vbLf)
						Next d
					End If
					If dl.getOrDependencies() IsNot Nothing Then
						sb.Append("Or dependencies:" & vbLf)
						For Each p As Pair(Of D, D) In dl.getOrDependencies()
							sb.Append(p).Append(" - satisfied=(").Append(isSatisfied(p.First)).Append(",").Append(isSatisfied(p.Second)).Append(")")
						Next p
					End If
					Throw New System.InvalidOperationException(sb.ToString())
				End If

				'Not satisfied, but is in the queue -> needs to be removed
				allSatisfied.remove(y)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
				allSatisfiedQueue.remove(y)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected boolean isAllSatisfied(@NonNull T y)
		Protected Friend Overridable Function isAllSatisfied(ByVal y As T) As Boolean
			Dim set1 As ISet(Of D) = dependencies(y)

			Dim retVal As Boolean = True
			If set1 IsNot Nothing Then
				For Each d As D In set1
					retVal = isSatisfied(d)
					If Not retVal Then
						Exit For
					End If
				Next d
			End If
			If retVal Then
				Dim set2 As ISet(Of Pair(Of D, D)) = orDependencies(y)
				If set2 IsNot Nothing Then
					For Each p As Pair(Of D, D) In set2
						retVal = isSatisfied(p.First) OrElse isSatisfied(p.Second)
						If Not retVal Then
							Exit For
						End If
					Next p
				End If
			End If
			Return retVal
		End Function


		''' <summary>
		''' Remove a dependency (x -> y)
		''' </summary>
		''' <param name="y"> The dependent that currently requires X </param>
		''' <param name="x"> The dependee that is no longer required for Y </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void removeDependency(@NonNull T y, @NonNull D x)
		Public Overridable Sub removeDependency(ByVal y As T, ByVal x As D)
			If Not dependencies.ContainsKey(y) AndAlso Not orDependencies.ContainsKey(y) Then
				Return
			End If

			Dim s As ISet(Of D) = dependencies(y)
			If s IsNot Nothing Then
				s.remove(x)
				If s.Count = 0 Then
					dependencies.Remove(y)
				End If
			End If

			Dim s2 As ISet(Of T) = reverseDependencies(x)
			If s2 IsNot Nothing Then
				s2.remove(y)
				If s2.Count = 0 Then
					reverseDependencies.Remove(x)
				End If
			End If


			Dim s3 As ISet(Of Pair(Of D, D)) = orDependencies(y)
			If s3 IsNot Nothing Then
				Dim removedReverse As Boolean = False
				Dim iter As IEnumerator(Of Pair(Of D, D)) = s3.GetEnumerator()
				Do While iter.MoveNext()
					Dim p As Pair(Of D, D) = iter.Current
					If x.Equals(p.First) OrElse x.Equals(p.Second) Then
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
						iter.remove()

						If Not removedReverse Then
							Dim set1 As ISet(Of T) = reverseOrDependencies(p.First)
							Dim set2 As ISet(Of T) = reverseOrDependencies(p.Second)

							set1.remove(y)
							set2.remove(y)

							If set1.Count = 0 Then
								reverseOrDependencies.Remove(p.First)
							End If
							If set2.Count = 0 Then
								reverseOrDependencies.Remove(p.Second)
							End If

							removedReverse = True
						End If
					End If
				Loop
			End If
			If s3 IsNot Nothing AndAlso s3.Count = 0 Then
				orDependencies.Remove(y)
			End If
		End Sub

		''' <summary>
		''' Add an "Or" dependency: Y requires either x1 OR x2 - i.e., (x1 or x2) -> Y<br>
		''' If either x1 or x2 (or both) are marked satisfied via <seealso cref="markSatisfied(Object, Boolean)"/> then the
		''' dependency is considered satisfied
		''' </summary>
		''' <param name="y">  Dependent </param>
		''' <param name="x1"> Dependee 1 </param>
		''' <param name="x2"> Dependee 2 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addOrDependency(@NonNull T y, @NonNull D x1, @NonNull D x2)
		Public Overridable Sub addOrDependency(ByVal y As T, ByVal x1 As D, ByVal x2 As D)
			If Not orDependencies.ContainsKey(y) Then
				orDependencies(y) = New HashSet(Of Pair(Of D, D))()
			End If

			If Not reverseOrDependencies.ContainsKey(x1) Then
				reverseOrDependencies(x1) = newTSet()
			End If
			If Not reverseOrDependencies.ContainsKey(x2) Then
				reverseOrDependencies(x2) = newTSet()
			End If

			orDependencies(y).Add(New Pair(Of )(x1, x2))
			reverseOrDependencies(x1).Add(y)
			reverseOrDependencies(x2).Add(y)

			checkAndUpdateIfAllSatisfied(y)
		End Sub

		''' <returns> True if there are any new/unprocessed "all satisfied dependents" (Ys in X->Y) </returns>
		Public Overridable Function hasNewAllSatisfied() As Boolean
			Return allSatisfiedQueue.Count > 0
		End Function

		''' <summary>
		''' Returns the next new dependent (Y in X->Y) that has all dependees (Xs) marked as satisfied via <seealso cref="markSatisfied(Object, Boolean)"/>
		''' Throws an exception if <seealso cref="hasNewAllSatisfied()"/> returns false.<br>
		''' Note that once a value has been retrieved from here, no new dependencies of the form (X -> Y) can be added for this value;
		''' the value is considered "processed" at this point.
		''' </summary>
		''' <returns> The next new "all satisfied dependent" </returns>
		Public Overridable ReadOnly Property NewAllSatisfied As T
			Get
				Preconditions.checkState(hasNewAllSatisfied(), "No new/unprocessed dependents that are all satisfied")
				Return allSatisfiedQueue.RemoveFirst()
			End Get
		End Property

		''' <returns> As per <seealso cref="getNewAllSatisfied()"/> but returns all values </returns>
		Public Overridable ReadOnly Property NewAllSatisfiedList As IList(Of T)
			Get
				Preconditions.checkState(hasNewAllSatisfied(), "No new/unprocessed dependents that are all satisfied")
				Dim ret As IList(Of T) = New List(Of T)(allSatisfiedQueue)
				allSatisfiedQueue.Clear()
				Return ret
			End Get
		End Property

		''' <summary>
		''' As per <seealso cref="getNewAllSatisfied()"/> but instead of returning the first dependee, it returns the first that matches
		''' the provided predicate. If no value matches the predicate, null is returned
		''' </summary>
		''' <param name="predicate"> Predicate gor checking </param>
		''' <returns> The first value matching the predicate, or null if no values match the predicate </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public T getFirstNewAllSatisfiedMatching(@NonNull Predicate<T> predicate)
		Public Overridable Function getFirstNewAllSatisfiedMatching(ByVal predicate As Predicate(Of T)) As T
			Preconditions.checkState(hasNewAllSatisfied(), "No new/unprocessed dependents that are all satisfied")

			Dim t As T = allSatisfiedQueue.First.Value
			If predicate.test(t) Then
				t = allSatisfiedQueue.RemoveFirst()
				allSatisfied.remove(t)
				Return t
			End If

			If allSatisfiedQueue.Count > 1 Then
				Dim iter As IEnumerator(Of T) = allSatisfiedQueue.GetEnumerator()
				Do While iter.MoveNext()
					t = iter.Current
					If predicate.test(t) Then
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
						iter.remove()
						allSatisfied.remove(t)
						Return t
					End If
				Loop
			End If

			Return Nothing 'None match predicate
		End Function
	End Class

End Namespace