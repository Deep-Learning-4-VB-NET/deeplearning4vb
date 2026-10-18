Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports MemcpyDirection = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemcpyDirection

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

Namespace org.nd4vb.linalg.api.ops.performance.primitives

    ''' <summary>
    ''' Utility class that holds values for each <see cref="MemcpyDirection"/> and returns their average, guarded by
    ''' one read/write lock per direction (Java's <c>ReentrantReadWriteLock</c> is <see cref="ReaderWriterLockSlim"/>).
    ''' </summary>
    Public Class AveragingTransactionsHolder

        Private ReadOnly storage As IList(Of IList(Of Long)) = New List(Of IList(Of Long))([Enum].GetValues(GetType(MemcpyDirection)).Length)
        Private ReadOnly locks As IList(Of ReaderWriterLockSlim) = New List(Of ReaderWriterLockSlim)([Enum].GetValues(GetType(MemcpyDirection)).Length)

        Public Sub New()
            init()
        End Sub

        Protected Friend Overridable Sub init()
            ' filling map withi initial keys
            For Each v As MemcpyDirection In [Enum].GetValues(GetType(MemcpyDirection))
                Dim o As Integer = CInt(v)
                storage.Insert(o, New List(Of Long)())

                locks.Insert(o, New ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion))
            Next v
        End Sub

        Public Overridable Sub clear()
            For Each v As MemcpyDirection In [Enum].GetValues(GetType(MemcpyDirection))
                Dim o As Integer = CInt(v)
                locks(o).EnterWriteLock()
                Try
                    storage(o).Clear()
                Finally
                    locks(o).ExitWriteLock()
                End Try
            Next v
        End Sub

        Public Overridable Sub addValue(ByVal direction As MemcpyDirection, ByVal value As Long?)
            Dim o As Integer = CInt(direction)
            locks(o).EnterWriteLock()
            Try
                storage(o).Add(value.Value)
            Finally
                locks(o).ExitWriteLock()
            End Try
        End Sub

        ''' <summary>The average of the values of a direction (integer division, as Java's long arithmetic), 0 when there are none.</summary>
        Public Overridable Function getAverageValue(ByVal direction As MemcpyDirection) As Long?
            Dim o As Integer = CInt(direction)
            locks(o).EnterReadLock()
            Try
                Dim r As Long = 0L
                Dim list As IList(Of Long) = storage(o)

                If list.Count = 0 Then
                    Return 0L
                End If

                For Each v As Long In list
                    r += v
                Next v

                Return r \ list.Count
            Finally
                locks(o).ExitReadLock()
            End Try
        End Function

    End Class

End Namespace
