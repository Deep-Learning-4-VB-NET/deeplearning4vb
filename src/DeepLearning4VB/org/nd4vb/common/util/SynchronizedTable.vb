Imports System
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

    ''' <summary>A <see cref="Table(Of R, C, V)"/> wrapper that serialises every operation on a lock.</summary>
    Public Class SynchronizedTable(Of R, C, V)
        Implements Table(Of R, C, V)

        Private ReadOnly wrapped As Table(Of R, C, V)
        Private ReadOnly gate As New Object()

        Public Sub New(ByVal wrapped As Table(Of R, C, V))
            If wrapped Is Nothing Then Throw New ArgumentNullException(NameOf(wrapped))
            Me.wrapped = wrapped
        End Sub

        Public Function contains(ByVal rowKey As R, ByVal columnKey As C) As Boolean Implements Table(Of R, C, V).contains
            SyncLock gate
                Return wrapped.contains(rowKey, columnKey)
            End SyncLock
        End Function

        Public Function containsRow(ByVal rowKey As R) As Boolean Implements Table(Of R, C, V).containsRow
            SyncLock gate
                Return wrapped.containsRow(rowKey)
            End SyncLock
        End Function

        Public Function containsColumn(ByVal columnKey As C) As Boolean Implements Table(Of R, C, V).containsColumn
            SyncLock gate
                Return wrapped.containsColumn(columnKey)
            End SyncLock
        End Function

        Public Function containsValue(ByVal value As V) As Boolean Implements Table(Of R, C, V).containsValue
            SyncLock gate
                Return wrapped.containsValue(value)
            End SyncLock
        End Function

        Public Function [get](ByVal rowKey As R, ByVal columnKey As C) As V Implements Table(Of R, C, V).get
            SyncLock gate
                ' the Java original recursed into itself here (an infinite loop); delegate to the wrapped table
                Return wrapped.get(rowKey, columnKey)
            End SyncLock
        End Function

        Public Function isEmpty() As Boolean Implements Table(Of R, C, V).isEmpty
            SyncLock gate
                Return wrapped.isEmpty()
            End SyncLock
        End Function

        Public Function size() As Integer Implements Table(Of R, C, V).size
            SyncLock gate
                Return wrapped.size()
            End SyncLock
        End Function

        Public Sub clear() Implements Table(Of R, C, V).clear
            SyncLock gate
                wrapped.clear()
            End SyncLock
        End Sub

        Public Function put(ByVal rowKey As R, ByVal columnKey As C, ByVal value As V) As V Implements Table(Of R, C, V).put
            SyncLock gate
                Return wrapped.put(rowKey, columnKey, value)
            End SyncLock
        End Function

        Public Sub putAll(ByVal table As Table(Of R, C, V)) Implements Table(Of R, C, V).putAll
            SyncLock gate
                wrapped.putAll(table)
            End SyncLock
        End Sub

        Public Function remove(ByVal rowKey As R, ByVal columnKey As C) As V Implements Table(Of R, C, V).remove
            SyncLock gate
                Return wrapped.remove(rowKey, columnKey)
            End SyncLock
        End Function

        Public Function row(ByVal rowKey As R) As IDictionary(Of C, V) Implements Table(Of R, C, V).row
            SyncLock gate
                Return wrapped.row(rowKey)
            End SyncLock
        End Function

        Public Function column(ByVal columnKey As C) As IDictionary(Of R, V) Implements Table(Of R, C, V).column
            SyncLock gate
                Return wrapped.column(columnKey)
            End SyncLock
        End Function

        Public Function cellSet() As ISet(Of Cell(Of R, C, V)) Implements Table(Of R, C, V).cellSet
            SyncLock gate
                Return wrapped.cellSet()
            End SyncLock
        End Function

        Public Function rowKeySet() As ISet(Of R) Implements Table(Of R, C, V).rowKeySet
            SyncLock gate
                Return wrapped.rowKeySet()
            End SyncLock
        End Function

        Public Function columnKeySet() As ISet(Of C) Implements Table(Of R, C, V).columnKeySet
            SyncLock gate
                Return wrapped.columnKeySet()
            End SyncLock
        End Function

        Public Function values() As ICollection(Of V) Implements Table(Of R, C, V).values
            SyncLock gate
                Return wrapped.values()
            End SyncLock
        End Function

        Public Function rowMap() As IDictionary(Of R, IDictionary(Of C, V)) Implements Table(Of R, C, V).rowMap
            SyncLock gate
                Return wrapped.rowMap()
            End SyncLock
        End Function

        Public Function columnMap() As IDictionary(Of C, IDictionary(Of R, V)) Implements Table(Of R, C, V).columnMap
            SyncLock gate
                Return wrapped.columnMap()
            End SyncLock
        End Function
    End Class

End Namespace
