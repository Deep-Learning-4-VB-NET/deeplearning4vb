Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DummyWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.DummyWorkspace
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

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

Namespace org.nd4vb.linalg.api.memory

    ''' <summary>
    ''' The shared part of memory managers (Java <c>BasicMemoryManager</c>): the current workspace of each thread, the
    ''' occasional and periodic garbage-collection settings, and loop-time averaging.
    ''' </summary>
    ''' <remarks>
    ''' Differences from Java: <c>allocate(bytes, kind, initialize)</c> and <c>release(Pointer, kind)</c> are native-only and
    ''' not ported (managed memory comes from the garbage-collected heap); <see cref="memcpy"/> copies elements between
    ''' buffers of the same type instead of raw bytes, and records no <c>PerformanceTracker</c> transaction;
    ''' <see cref="scopeOutOfWorkspaces"/> uses this memory manager (Java: <c>Nd4j.getMemoryManager()</c>, the same object).
    ''' </remarks>
    Public MustInherit Class BasicMemoryManager
        Implements MemoryManager

        ''' <summary>Memory allocated on a device (implementation-specific).</summary>
        Public MustOverride Function allocatedMemory(ByVal deviceId As Integer?) As Long Implements MemoryManager.allocatedMemory
        ''' <summary>Bandwidth use per device, or Nothing.</summary>
        Public MustOverride ReadOnly Property BandwidthUse As IDictionary(Of Integer, Long) Implements MemoryManager.BandwidthUse
        ''' <summary>Sets every element of the array to zero.</summary>
        Public MustOverride Sub memset(ByVal array As INDArray) Implements MemoryManager.memset

        Protected Friend frequency As Integer = 0
        Protected Friend freqCounter As Long = 0
        Protected Friend lastGcTimeField As Long = JavaSystem.currentTimeMillis()
        Protected Friend periodicEnabled As Boolean = False
        Protected Friend averageLoopTimeField As Integer = 0
        Protected Friend noGcWindow As Integer = 100
        Protected Friend averagingEnabled As Boolean = False

        Protected Friend Const intervalTail As Integer = 100

        Protected Friend intervals As New ConcurrentQueue(Of Integer)()

        Private ReadOnly workspace As New ThreadLocal(Of MemoryWorkspace)()

        ''' <summary>Not implemented in Java either: throws <see cref="NotSupportedException"/>.</summary>
        Public Overridable Sub collect(ParamArray ByVal arrays() As INDArray) Implements MemoryManager.collect
            Throw New NotSupportedException("This method isn't implemented yet")
        End Sub

        ''' <summary>Enables or disables averaging of the time between garbage collections.</summary>
        Public Overridable Sub toggleAveraging(ByVal enabled As Boolean) Implements MemoryManager.toggleAveraging
            averagingEnabled = enabled
        End Sub

        ''' <summary>This method purges all cached memory chunks (no cache: nothing to do).</summary>
        Public Overridable Sub purgeCaches() Implements MemoryManager.purgeCaches
        End Sub

        ''' <summary>
        ''' Copies the <c>srcBuffer.length()</c> elements of <paramref name="srcBuffer"/> to <paramref name="dstBuffer"/>
        ''' (Java copied the raw bytes; here both buffers must have the same type).
        ''' </summary>
        Public Overridable Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer) Implements MemoryManager.memcpy
            If dstBuffer Is Nothing Then
                Throw New ArgumentNullException(NameOf(dstBuffer))
            End If
            If srcBuffer Is Nothing Then
                Throw New ArgumentNullException(NameOf(srcBuffer))
            End If
            If dstBuffer.dataType() <> srcBuffer.dataType() Then
                Throw New NotSupportedException("memcpy between buffers of different types (" & srcBuffer.dataType().ToString() & " to " & dstBuffer.dataType().ToString() & ") is not supported")
            End If
            Dim n As Long = srcBuffer.length()
            Select Case srcBuffer.dataType()
                Case DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.BFLOAT16
                    For i As Long = 0 To n - 1
                        dstBuffer.put(i, srcBuffer.getDouble(i))
                    Next
                Case DataType.BOOL
                    For i As Long = 0 To n - 1
                        dstBuffer.put(i, srcBuffer.getInt(i) <> 0)
                    Next
                Case DataType.UTF8, DataType.COMPRESSED, DataType.UNKNOWN
                    Throw New NotSupportedException("memcpy is not supported for " & srcBuffer.dataType().ToString() & " buffers")
                Case Else
                    For i As Long = 0 To n - 1
                        dstBuffer.put(i, srcBuffer.getLong(i))
                    Next
            End Select
        End Sub

        ''' <summary>Notification that a scope was entered (to be implemented in Java too: nothing to do).</summary>
        Public Overridable Sub notifyScopeEntered() Implements MemoryManager.notifyScopeEntered
        End Sub

        ''' <summary>Notification that a scope was left (to be implemented in Java too: nothing to do).</summary>
        Public Overridable Sub notifyScopeLeft() Implements MemoryManager.notifyScopeLeft
        End Sub

        ''' <summary>
        ''' Counts a loop iteration and runs a garbage collection every <see cref="OccasionalGcFrequency"/> iterations, when
        ''' the last collection is older than <see cref="AutoGcWindow"/> milliseconds.
        ''' </summary>
        Public Overridable Sub invokeGcOccasionally() Implements MemoryManager.invokeGcOccasionally
            Dim currentTime As Long = JavaSystem.currentTimeMillis()

            If averagingEnabled Then
                intervals.Enqueue(CInt(currentTime - lastGcTimeField))
            End If

            ' not sure if we want to conform autoGcWindow here...
            If frequency > 0 Then
                freqCounter += 1
                If freqCounter Mod frequency = 0 AndAlso currentTime > LastGcTime + AutoGcWindow Then
                    GC.Collect()
                    lastGcTimeField = JavaSystem.currentTimeMillis()
                End If
            End If

            If averagingEnabled Then
                Dim dropped As Integer
                If intervals.Count > intervalTail Then
                    intervals.TryDequeue(dropped)
                End If
            End If
        End Sub

        ''' <summary>Runs a garbage collection and records its time.</summary>
        Public Overridable Sub invokeGc() Implements MemoryManager.invokeGc
            GC.Collect()
            lastGcTimeField = JavaSystem.currentTimeMillis()
        End Sub

        ''' <summary>True if periodic garbage collection is enabled.</summary>
        Public Overridable ReadOnly Property PeriodicGcActive As Boolean Implements MemoryManager.PeriodicGcActive
            Get
                Return periodicEnabled
            End Get
        End Property

        ''' <summary>Every how many loop iterations <see cref="invokeGcOccasionally"/> collects (0: never).</summary>
        Public Overridable Property OccasionalGcFrequency As Integer Implements MemoryManager.OccasionalGcFrequency
            Get
                Return frequency
            End Get
            Set(ByVal value As Integer)
                frequency = value
            End Set
        End Property

        ''' <summary>The minimum time between automatic garbage collections, in milliseconds.</summary>
        Public Overridable Property AutoGcWindow As Integer Implements MemoryManager.AutoGcWindow
            Get
                Return noGcWindow
            End Get
            Set(ByVal windowMillis As Integer)
                noGcWindow = windowMillis
            End Set
        End Property

        ''' <summary>The time of the last garbage collection (milliseconds since the epoch).</summary>
        Public Overridable ReadOnly Property LastGcTime As Long Implements MemoryManager.LastGcTime
            Get
                Return lastGcTimeField
            End Get
        End Property

        ''' <summary>Enables or disables periodic garbage collection.</summary>
        Public Overridable Sub togglePeriodicGc(ByVal enabled As Boolean) Implements MemoryManager.togglePeriodicGc
            periodicEnabled = enabled
        End Sub

        ''' <summary>The average time between collections over the last 100 loops when averaging is enabled, otherwise 0.</summary>
        Public Overridable ReadOnly Property AverageLoopTime As Integer Implements MemoryManager.AverageLoopTime
            Get
                If averagingEnabled Then
                    Dim snapshot As Integer() = intervals.ToArray()
                    If snapshot.Length = 0 Then
                        Return 0
                    End If
                    Dim cnt As Long = 0
                    For Each value As Integer In snapshot
                        cnt += value
                    Next value
                    Return CInt(cnt \ snapshot.Length)
                Else
                    Return 0
                End If
            End Get
        End Property

        ''' <summary>The workspace arrays of the current thread are allocated in (Nothing: outside any workspace).</summary>
        Public Overridable Property CurrentWorkspace As MemoryWorkspace Implements MemoryManager.CurrentWorkspace
            Get
                Return workspace.Value
            End Get
            Set(ByVal value As MemoryWorkspace)
                workspace.Value = value
            End Set
        End Property

        ''' <summary>
        ''' Opens a block outside any workspace: a <see cref="DummyWorkspace"/> that, when a workspace is current, detaches the
        ''' thread from it until closed.
        ''' </summary>
        Public Overridable Function scopeOutOfWorkspaces() As MemoryWorkspace Implements MemoryManager.scopeOutOfWorkspaces
            Dim current As MemoryWorkspace = CurrentWorkspace
            If current Is Nothing Then
                Return New DummyWorkspace(Me)
            Else
                Return (New DummyWorkspace(Me)).notifyScopeEntered()
            End If
        End Function

        ''' <summary>Nothing to release for the managed backend.</summary>
        Public Overridable Sub releaseCurrentContext() Implements MemoryManager.releaseCurrentContext
        End Sub

    End Class

End Namespace
