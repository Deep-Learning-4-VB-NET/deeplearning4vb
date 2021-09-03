Imports System.Collections.Generic
Imports Table = org.nd4j.shade.guava.collect.Table

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

Namespace org.nd4j.common.util


	Public Class SynchronizedTable(Of R, C, V)
		Implements Table(Of R, C, V)

		Private wrapped As Table(Of R, C, V)

		Public Sub New(ByVal wrapped As Table(Of R, C, V))
			Me.wrapped = wrapped
		End Sub

		Public Overrides Function contains(ByVal rowKey As Object, ByVal columnKey As Object) As Boolean
			SyncLock Me
				Return wrapped.contains(rowKey, columnKey)
			End SyncLock
		End Function

		Public Overrides Function containsRow(ByVal rowKey As Object) As Boolean
			SyncLock Me
				Return wrapped.containsRow(rowKey)
			End SyncLock
		End Function

		Public Overrides Function containsColumn(ByVal columnKey As Object) As Boolean
			SyncLock Me
				Return wrapped.containsColumn(columnKey)
			End SyncLock
		End Function

		Public Overrides Function containsValue(ByVal value As Object) As Boolean
			SyncLock Me
				Return wrapped.containsValue(value)
			End SyncLock
		End Function

		Public Overrides Function get(ByVal rowKey As Object, ByVal columnKey As Object) As V
			SyncLock Me
				Return get(rowKey, columnKey)
			End SyncLock
		End Function

		Public Overrides ReadOnly Property Empty As Boolean
			Get
				SyncLock Me
					Return wrapped.isEmpty()
				End SyncLock
			End Get
		End Property

		Public Overrides Function size() As Integer
			Return wrapped.size()
		End Function

		Public Overrides Sub clear()
			SyncLock Me
				wrapped.clear()
			End SyncLock
		End Sub

		Public Overrides Function put(ByVal rowKey As R, ByVal columnKey As C, ByVal value As V) As V
			SyncLock Me
				Return wrapped.put(rowKey, columnKey, value)
			End SyncLock
		End Function

		Public Overrides Sub putAll(Of T1 As R, T2 As C, T3 As V)(ByVal table As Table(Of T1, T2, T3))
			SyncLock Me
				wrapped.putAll(table)
			End SyncLock
		End Sub

		Public Overrides Function remove(ByVal rowKey As Object, ByVal columnKey As Object) As V
			SyncLock Me
				Return wrapped.remove(rowKey, columnKey)
			End SyncLock
		End Function

		Public Overrides Function row(ByVal rowKey As R) As IDictionary(Of C, V)
			SyncLock Me
				Return wrapped.row(rowKey)
			End SyncLock
		End Function

		Public Overrides Function column(ByVal columnKey As C) As IDictionary(Of R, V)
			SyncLock Me
				Return wrapped.column(columnKey)
			End SyncLock
		End Function

		Public Overrides Function cellSet() As ISet(Of Cell(Of R, C, V))
			SyncLock Me
				Return wrapped.cellSet()
			End SyncLock
		End Function

		Public Overrides Function rowKeySet() As ISet(Of R)
			SyncLock Me
				Return wrapped.rowKeySet()
			End SyncLock
		End Function

		Public Overrides Function columnKeySet() As ISet(Of C)
			SyncLock Me
				Return wrapped.columnKeySet()
			End SyncLock
		End Function

		Public Overrides Function values() As ICollection(Of V)
			SyncLock Me
				Return wrapped.values()
			End SyncLock
		End Function

		Public Overrides Function rowMap() As IDictionary(Of R, IDictionary(Of C, V))
			SyncLock Me
				Return wrapped.rowMap()
			End SyncLock
		End Function

		Public Overrides Function columnMap() As IDictionary(Of C, IDictionary(Of R, V))
			SyncLock Me
				Return wrapped.columnMap()
			End SyncLock
		End Function
	End Class

End Namespace