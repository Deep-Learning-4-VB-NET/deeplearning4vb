Imports System
Imports System.Threading
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports OpProfiler = DeepLearning4VB.org.nd4vb.linalg.profiler.OpProfiler

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
    ''' Keeps a copy of an array on every device. In immediate mode <see cref="broadcast"/> copies the array to all devices
    ''' at once; in delayed mode the other devices copy the most recent array the first time they read it.
    ''' Java's <c>synchronized</c> methods lock on the instance.
    ''' </summary>
    ''' @author raver119@gmail.com
    Public Class DeviceLocalNDArray
        Inherits DeviceLocal(Of INDArray)

        Public Sub New()
            Me.New(False)
        End Sub

        ''' <param name="delayedMode"> whether the other devices copy updates lazily </param>
        Public Sub New(ByVal delayedMode As Boolean)
            MyBase.New(delayedMode)
        End Sub

        ''' <param name="array"> the array to put on every device </param>
        Public Sub New(ByVal array As INDArray)
            Me.New(array, False)
        End Sub

        ''' <param name="array"> the array to put on every device </param>
        ''' <param name="delayedMode"> whether the other devices copy updates lazily </param>
        Public Sub New(ByVal array As INDArray, ByVal delayedMode As Boolean)
            MyBase.New(delayedMode)

            broadcast(array)
        End Sub

        ''' <summary>
        ''' The array of the current thread's device, copied from the device holding the most recent array first when
        ''' this device is stale (delayed mode).
        ''' </summary>
        Public Overrides Function [get]() As INDArray
            SyncLock Me
                Dim deviceId As Integer = currentDevice()
                Dim numDevices As Integer = Nd4vbT.AffinityManager.NumberOfDevices
                Dim sourceId As Integer = Volatile.Read(updatesMap(deviceId))
                If sourceId >= 0 AndAlso sourceId <> deviceId Then
                    ' if updates map contains some deviceId - we should take updated array from there
                    Dim newArray As INDArray = Nd4vbT.create(delayedArray.dataType(), delayedArray.shape(), delayedArray.stride(), delayedArray.ordering())
                    Nd4vbT.MemoryManager.memcpy(newArray.data(), delayedArray.data())
                    backingMap(deviceId) = newArray

                    ' reset updates flag
                    Volatile.Write(updatesMap(deviceId), deviceId)

                    ' also check if all updates were consumed
                    Dim allUpdated As Boolean = True
                    For e As Integer = 0 To numDevices - 1
                        If Volatile.Read(updatesMap(e)) <> e Then
                            allUpdated = False
                            Exit For
                        End If
                    Next e

                    If allUpdated Then
                        delayedArray = Nothing
                    End If
                End If
                Return [get](deviceId)
            End SyncLock
        End Function

        ''' <summary>
        ''' Puts the array on every device (immediate mode), or on the current device while marking the others stale
        ''' (delayed mode). <c>Nothing</c> is ignored.
        ''' </summary>
        ''' <param name="array"> the array to broadcast; views are rejected unless their element-wise stride is not 1, as in Java </param>
        Public Overridable Sub broadcast(ByVal array As INDArray)
            SyncLock Me
                If array Is Nothing Then
                    Return
                End If

                Preconditions.checkArgument(Not array.View OrElse array.elementWiseStride() <> 1, "View can't be used in DeviceLocalNDArray")

                Nd4vbT.Executioner.commit()

                Dim config = OpProfiler.Instance.Config
                Dim locality As Boolean = config.isCheckLocality()

                If locality Then
                    config.setCheckLocality(False)
                End If
                Dim numDevices As Integer = Nd4vbT.AffinityManager.NumberOfDevices
                Dim deviceId As Integer = currentDevice()

                If Not delayedMode Then
                    ' in immediate mode we put data in
                    For i As Integer = 0 To numDevices - 1
                        ' if current thread equal to this device - we just save it, without duplication
                        If deviceId = i Then
                            [set](i, array.detach())
                        Else
                            [set](i, Nd4vbT.AffinityManager.replicateToDevice(i, array))
                        End If
                    Next i
                Else
                    ' we're only updating this device
                    [set](currentDevice(), array)
                    delayedArray = array.dup(array.ordering()).detach()

                    ' and marking all other devices as stale, and provide id of device with the most recent array
                    For i As Integer = 0 To numDevices - 1
                        If i <> deviceId Then
                            Volatile.Write(updatesMap(i), deviceId)
                        End If
                    Next i
                End If

                config.setCheckLocality(locality)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Copies new values into the arrays of all devices when the shape information matches (devices that hold
        ''' nothing yet are marked stale instead), and otherwise broadcasts the array.
        ''' </summary>
        ''' <param name="array"> the new values; views are rejected unless their element-wise stride is not 1, as in Java </param>
        Public Overridable Sub update(ByVal array As INDArray)
            If array Is Nothing Then
                Throw New ArgumentNullException(NameOf(array), "array is marked non-null but is null")
            End If
            SyncLock Me
                Preconditions.checkArgument(Not array.View OrElse array.elementWiseStride() <> 1, "View can't be used in DeviceLocalNDArray")

                Dim numDevices As Integer = Nd4vbT.AffinityManager.NumberOfDevices
                Dim device As Integer = currentDevice()
                Dim currentArray As INDArray = Nothing
                backingMap.TryGetValue(device, currentArray)
                Dim wasDelayed As Boolean = False

                If currentArray IsNot Nothing AndAlso currentArray.shapeInfoJava().SequenceEqual(array.shapeInfoJava()) Then
                    ' if arrays are the same - we'll just issue memcpy
                    For k As Integer = 0 To numDevices - 1
                        Dim lock As ReaderWriterLockSlim = locksMap(k)
                        lock.EnterWriteLock()
                        Try
                            Dim v As INDArray = Nothing
                            backingMap.TryGetValue(k, v)
                            If v Is Nothing Then
                                If Not wasDelayed Then
                                    delayedArray = array.dup(array.ordering()).detach()
                                    wasDelayed = True
                                End If
                                Volatile.Write(updatesMap(k), device)
                                Continue For
                            End If

                            Nd4vbT.MemoryManager.memcpy(v.data(), array.data())
                            Nd4vbT.Executioner.commit()
                        Finally
                            lock.ExitWriteLock()
                        End Try
                    Next k
                Else
                    ' if arrays are not the same - we'll issue broadcast call
                    broadcast(array)
                End If
            End SyncLock
        End Sub

    End Class

End Namespace
