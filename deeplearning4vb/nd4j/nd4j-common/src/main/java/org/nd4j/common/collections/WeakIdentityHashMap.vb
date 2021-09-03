Imports System.Collections.Generic
Imports lombok

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

Namespace org.nd4j.common.collections


	Public Class WeakIdentityHashMap(Of K, V)
		Implements IDictionary(Of K, V)

		Protected Friend ReadOnly map As IDictionary(Of KeyRef(Of K), V)
		Protected Friend ReadOnly refQueue As ReferenceQueue(Of K)

		Public Sub New()
			map = New Dictionary(Of KeyRef(Of K), V)()
			refQueue = New ReferenceQueue(Of K)()
		End Sub

		'Clear references to any map keys that have been GC'd
		Protected Friend Overridable Sub clearReferences()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.lang.ref.Reference<? extends K> r;
			Dim r As Reference(Of K)
			r = refQueue.poll()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while((r = refQueue.poll()) != null)
			Do While r IsNot Nothing
				map.Remove(r)
					r = refQueue.poll()
			Loop
		End Sub

		Public Overridable ReadOnly Property Count As Integer Implements ICollection(Of K, V).Count
			Get
				clearReferences()
				Return map.Count
			End Get
		End Property

		Public Overrides ReadOnly Property Empty As Boolean
			Get
				clearReferences()
				Return map.Count = 0
			End Get
		End Property

		Public Overridable Function ContainsKey(ByVal key As Object) As Boolean Implements IDictionary(Of K, V).ContainsKey
			clearReferences()
			Return map.ContainsKey(New KeyRef(Of )(key))
		End Function

		Public Overrides Function containsValue(ByVal value As Object) As Boolean
			clearReferences()
			Return map.ContainsValue(value)
		End Function

		Public Overrides Function get(ByVal key As Object) As V
			clearReferences()
			Return map(New KeyRef(Of )(key))
		End Function

		Public Overrides Function put(ByVal key As K, ByVal value As V) As V
			clearReferences()
			map(New KeyRef(Of )(key)) = value
			Return value
		End Function

		Public Overrides Function remove(ByVal key As Object) As V
			clearReferences()
			Return map.Remove(New KeyRef(Of )(key))
		End Function

		Public Overrides Sub putAll(Of T1 As K, T2 As V)(ByVal m As IDictionary(Of T1, T2))
			clearReferences()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for(Map.Entry<? extends K, ? extends V> e : m.entrySet())
			For Each e As KeyValuePair(Of K, V) In m.SetOfKeyValuePairs()
				map(New KeyRef(Of )(e.Key)) = e.Value
			Next e
		End Sub

		Public Overridable Sub Clear() Implements ICollection(Of K, V).Clear
			map.Clear()
			clearReferences()
		End Sub

		Public Overrides Function keySet() As ISet(Of K)
			clearReferences()
			Dim ret As ISet(Of K) = New HashSet(Of K)()
			For Each k As KeyRef(Of K) In map.Keys
				Dim key As K = k.get()
				If key IsNot Nothing Then
					ret.Add(key)
				End If
			Next k
			Return ret
		End Function

		Public Overridable Function getValues() As ICollection(Of V) Implements IDictionary(Of K, V).Values
			clearReferences()
			Return map.Values
		End Function

		Public Overrides Function entrySet() As ISet(Of KeyValuePair(Of K, V))
			clearReferences()
			Dim ret As ISet(Of KeyValuePair(Of K, V)) = New HashSet(Of KeyValuePair(Of K, V))()
			For Each e As KeyValuePair(Of KeyRef(Of K), V) In map.SetOfKeyValuePairs()
				Dim k As K = e.Key.get()
				If k IsNot Nothing Then
					ret.Add(New Entry(Of K, V)(k, e.Value))
				End If
			Next e
			Return ret
		End Function


		Protected Friend Class KeyRef(Of K)
			Inherits WeakReference(Of K)

			Friend ReadOnly hash As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public KeyRef(@NonNull K referent)
			Public Sub New(ByVal referent As K)
				MyBase.New(referent)
				Me.hash = System.identityHashCode(referent)
			End Sub

			Public Overrides Function GetHashCode() As Integer
				Return hash
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Me Is o Then
					Return True
				End If
				If TypeOf o Is WeakReference Then
					Return Me.get() = DirectCast(o, WeakReference).get()
				End If
				Return False
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor protected static class Entry<K,V> implements Map.Entry<K, V>
		Protected Friend Class Entry(Of K, V)
			Implements KeyValuePair(Of K, V)

			Protected Friend key As K
'JAVA TO VB CONVERTER NOTE: The field value was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend value_Conflict As V

			Public Overrides Function setValue(ByVal value As V) As V
				Me.value_Conflict = value
				Return value
			End Function
		End Class
	End Class

End Namespace