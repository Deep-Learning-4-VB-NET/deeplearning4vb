Imports System.Linq
Imports Nullable = edu.umd.cs.findbugs.annotations.Nullable
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler

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

Namespace org.nd4j.linalg.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DeviceLocalNDArray extends DeviceLocal<org.nd4j.linalg.api.ndarray.INDArray>
	Public Class DeviceLocalNDArray
		Inherits DeviceLocal(Of INDArray)

		Public Sub New()
			Me.New(False)
		End Sub

		Public Sub New(ByVal delayedMode As Boolean)
			MyBase.New(delayedMode)
		End Sub

		Public Sub New(ByVal array As INDArray)
			Me.New(array, False)
		End Sub

		Public Sub New(ByVal array As INDArray, ByVal delayedMode As Boolean)
			MyBase.New(delayedMode)

			broadcast(array)
		End Sub

		''' <summary>
		''' This method returns object local to current deviceId
		''' 
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Nullable @Override public synchronized org.nd4j.linalg.api.ndarray.INDArray get()
		Public Overrides Function get() As INDArray
			SyncLock Me
				Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()
				Dim numDevices As val = Nd4j.AffinityManager.NumberOfDevices
				Dim sourceId As val = updatesMap(deviceId).get()
				If sourceId >= 0 AndAlso sourceId <> deviceId Then
					' if updates map contains some deviceId - we should take updated array from there
					Dim newArray As val = Nd4j.create(delayedArray.dataType(), delayedArray.shape(), delayedArray.stride(), delayedArray.ordering())
					Nd4j.MemoryManager.memcpy(newArray.data(), delayedArray.data())
					backingMap(deviceId) = newArray
        
					' reset updates flag
					updatesMap(deviceId).set(deviceId)
        
        
					' also check if all updates were consumed
					Dim allUpdated As Boolean = True
					For e As Integer = 0 To numDevices - 1
						If updatesMap(e).get() <> e Then
							allUpdated = False
							Exit For
						End If
					Next e
        
					If allUpdated Then
						delayedArray = Nothing
					End If
				End If
				Return get(deviceId)
			End SyncLock
		End Function

		''' <summary>
		''' This method duplicates array, and stores it to all devices
		''' 
		''' PLEASE NOTE: this method is NOT atomic, so you must be sure no other threads are using this instance during the update </summary>
		''' <param name="array"> </param>
		Public Overridable Sub broadcast(ByVal array As INDArray)
			SyncLock Me
				If array Is Nothing Then
					Return
				End If
        
				Preconditions.checkArgument(Not array.View OrElse array.elementWiseStride() <> 1, "View can't be used in DeviceLocalNDArray")
        
				Nd4j.Executioner.commit()
        
				Dim config As val = OpProfiler.Instance.getConfig()
				Dim locality As val = config.isCheckLocality()
        
				If locality Then
					config.setCheckLocality(False)
				End If
				Dim numDevices As val = Nd4j.AffinityManager.NumberOfDevices
				Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()
        
				If Not delayedMode Then
					' in immediate mode we put data in
        
					For i As Integer = 0 To numDevices - 1
						' if current thread equal to this device - we just save it, without duplication
						If deviceId = i Then
							set(i, array.detach())
						Else
							set(i, Nd4j.AffinityManager.replicateToDevice(i, array))
						End If
        
					Next i
				Else
					' we're only updating this device
					set(Nd4j.AffinityManager.getDeviceForCurrentThread(), array)
					delayedArray = array.dup(array.ordering()).detach()
        
					' and marking all other devices as stale, and provide id of device with the most recent array
					For i As Integer = 0 To numDevices - 1
						If i <> deviceId Then
							updatesMap(i).set(deviceId)
						End If
					Next i
				End If
        
				config.setCheckLocality(locality)
			End SyncLock
		End Sub

		''' <summary>
		''' This method updates
		''' 
		''' PLEASE NOTE: this method is NOT atomic, so you must be sure no other threads are using this instance during the update </summary>
		''' <param name="array"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void update(@NonNull INDArray array)
		Public Overridable Sub update(ByVal array As INDArray)
			SyncLock Me
				Preconditions.checkArgument(Not array.isView() OrElse array.elementWiseStride() <> 1, "View can't be used in DeviceLocalNDArray")
        
				Dim numDevices As val = Nd4j.AffinityManager.NumberOfDevices
				Dim device As val = Nd4j.AffinityManager.getDeviceForCurrentThread()
				Dim currentArray As val = backingMap(device)
				Dim wasDelayed As Boolean = False
        
				If currentArray.shapeInfoJava().SequenceEqual(array.shapeInfoJava()) Then
					' if arrays are the same - we'll just issue memcpy
					For k As Integer = 0 To numDevices - 1
						Dim lock As val = locksMap(k)
						Try
							lock.writeLock().lock()
							Dim v As val = backingMap(k)
							If v Is Nothing Then
								If Not wasDelayed Then
									delayedArray = array.dup(array.ordering()).detach()
									wasDelayed = True
								End If
								updatesMap(k).set(device)
								Continue For
							End If
        
							Nd4j.MemoryManager.memcpy(v.data(), array.data())
							Nd4j.Executioner.commit()
						Finally
							lock.writeLock().unlock()
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