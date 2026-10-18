Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' Holds one object per device, keyed by device id (the managed backend has one device, id 0).
    ''' <para>
    ''' Differences from Java: the per-device <c>ReentrantReadWriteLock</c>s are <see cref="ReaderWriterLockSlim"/>s and
    ''' the per-device <c>AtomicInteger</c> update markers are the elements of <see cref="updatesMap"/>, read and written
    ''' with <see cref="Volatile"/>; Java's <c>get()</c>/<c>set()</c> are <see cref="[get]()"/>/<see cref="[set](T)"/>.
    ''' <see cref="[get](Integer)"/> returns <c>Nothing</c> (the default of <typeparamref name="T"/>) for a device that
    ''' holds nothing, like Java's <c>Map.get</c>.
    ''' </para>
    ''' </summary>
    ''' @author raver119@gmail.com
    Public MustInherit Class DeviceLocal(Of T)

        ''' <summary>The object of each device.</summary>
        Protected Friend backingMap As New ConcurrentDictionary(Of Integer, T)()
        ''' <summary>One lock per device.</summary>
        Protected Friend locksMap As IList(Of ReaderWriterLockSlim) = New List(Of ReaderWriterLockSlim)()
        ''' <summary>
        ''' For each device, the id of the device holding a newer value (delayed mode), the device's own id once it is
        ''' up to date, or -1.
        ''' </summary>
        Protected Friend updatesMap() As Integer
        ''' <summary>Whether updates are copied to the other devices lazily.</summary>
        Protected Friend ReadOnly delayedMode As Boolean
        ''' <summary>The most recent value in delayed mode (Java <c>volatile</c>).</summary>
        Protected Friend delayedArray As INDArray
        ''' <summary>Unused, as in Java.</summary>
        Protected Friend lastSettledDevice As Integer = -1

        ''' <param name="delayedMode"> whether updates reach the other devices lazily </param>
        Public Sub New(ByVal delayedMode As Boolean)
            Me.delayedMode = delayedMode

            Dim numDevices As Integer = Nd4vbT.AffinityManager.NumberOfDevices
            ReDim updatesMap(numDevices - 1)
            For i As Integer = 0 To numDevices - 1
                locksMap.Add(New ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion))
                updatesMap(i) = -1
            Next i
        End Sub

        ''' <summary>
        ''' The object of the current thread's device, or <c>Nothing</c>.
        ''' </summary>
        Public Overridable Function [get]() As T
            Return [get](currentDevice())
        End Function

        ''' <summary>
        ''' The object of the given device, or <c>Nothing</c>.
        ''' </summary>
        Public Overridable Function [get](ByVal deviceId As Integer) As T
            locksMap(deviceId).EnterReadLock()
            Try
                Dim value As T = Nothing
                backingMap.TryGetValue(deviceId, value)
                Return value
            Finally
                locksMap(deviceId).ExitReadLock()
            End Try
        End Function

        ''' <summary>
        ''' Stores the object of the given device.
        ''' </summary>
        Public Overridable Sub [set](ByVal deviceId As Integer, ByVal [object] As T)
            locksMap(deviceId).EnterWriteLock()
            Try
                backingMap(deviceId) = [object]
            Finally
                locksMap(deviceId).ExitWriteLock()
            End Try
        End Sub

        ''' <summary>
        ''' Stores the object of the current thread's device.
        ''' </summary>
        Public Overridable Sub [set](ByVal [object] As T)
            [set](currentDevice(), [object])
        End Sub

        ''' <summary>
        ''' Removes the object of the current thread's device.
        ''' </summary>
        Public Overridable Sub clear()
            Dim deviceId As Integer = currentDevice()
            locksMap(deviceId).EnterWriteLock()
            Try
                Dim removed As T = Nothing
                backingMap.TryRemove(deviceId, removed)
            Finally
                locksMap(deviceId).ExitWriteLock()
            End Try
        End Sub

        ''' <summary>The device of the current thread (Java <c>Nd4j.getAffinityManager().getDeviceForCurrentThread()</c>).</summary>
        Protected Friend Shared Function currentDevice() As Integer
            Return Nd4vbT.AffinityManager.DeviceForCurrentThread.GetValueOrDefault()
        End Function

    End Class

End Namespace
