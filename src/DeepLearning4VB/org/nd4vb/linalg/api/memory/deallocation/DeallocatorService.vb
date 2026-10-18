Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports DeepLearning4VB.org.slf4j
Imports Deallocatable = DeepLearning4VB.org.nd4vb.linalg.api.memory.Deallocatable

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

Namespace org.nd4vb.linalg.api.memory.deallocation

    ''' <summary>
    ''' Tracks <see cref="Deallocatable"/> objects through weak references and runs their <see cref="Deallocator"/> once the
    ''' garbage collector has reclaimed them (Java <c>DeallocatorService</c>).
    ''' </summary>
    ''' <remarks>
    ''' Java had a <c>ReferenceQueue</c> per device and daemon threads that blocked on them. .NET has no reference queues, so
    ''' this service has no threads: <see cref="purge"/> finds the references whose objects were collected and runs their
    ''' deallocators; <see cref="pickObject"/> purges every <see cref="PurgeInterval"/> registrations, and the managed memory
    ''' manager purges after <c>invokeGc()</c>. The managed backend allocates from the garbage-collected heap, so deallocators
    ''' only release bookkeeping (for example the allocation tracker's workspace bytes).
    ''' </remarks>
    Public Class DeallocatorService

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DeallocatorService))

        ''' <summary>Every how many registrations <see cref="pickObject"/> purges collected objects.</summary>
        Public Const PurgeInterval As Integer = 256

        Private ReadOnly referenceMap As New ConcurrentDictionary(Of String, DeallocatableReference)()
        Private counter As Long = 0
        Private registrations As Long = 0

        ''' <summary>Creates the service (Java also started its deallocator threads here).</summary>
        Public Sub New()
        End Sub

        ''' <summary>Returns a new value of a process-wide counter (used for unique ids).</summary>
        Public Overridable Function nextValue() As Long
            Return Interlocked.Increment(counter)
        End Function

        ''' <summary>This method adds Deallocatable object instance to tracking system.</summary>
        ''' <param name="deallocatable"> object to track </param>
        Public Overridable Sub pickObject(ByVal deallocatable As Deallocatable)
            If deallocatable Is Nothing Then
                Throw New ArgumentNullException(NameOf(deallocatable))
            End If
            Dim reference As New DeallocatableReference(deallocatable)
            referenceMap(deallocatable.UniqueId) = reference
            If Interlocked.Increment(registrations) Mod PurgeInterval = 0 Then
                purge()
            End If
        End Sub

        ''' <summary>
        ''' Runs the deallocator of every tracked object the garbage collector has reclaimed and stops tracking it; returns
        ''' how many were released.
        ''' </summary>
        Public Overridable Function purge() As Integer
            Dim released As Integer = 0
            For Each entry As KeyValuePair(Of String, DeallocatableReference) In referenceMap
                If entry.Value.get() Is Nothing Then
                    Dim removed As DeallocatableReference = Nothing
                    If referenceMap.TryRemove(entry.Key, removed) Then
                        Try
                            removed.getDeallocator().deallocate()
                        Catch e As System.Exception
                            log.warn("Deallocator of {} failed: {}", removed.getId(), e.Message)
                        End Try
                        released += 1
                    End If
                End If
            Next
            Return released
        End Function

        ''' <summary>The number of objects tracked (alive or not purged yet).</summary>
        Public Overridable ReadOnly Property TrackedObjects As Integer
            Get
                Return referenceMap.Count
            End Get
        End Property

    End Class

End Namespace
