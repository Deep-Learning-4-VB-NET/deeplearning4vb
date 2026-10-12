Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nullable = edu.umd.cs.findbugs.annotations.Nullable

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


	Public MustInherit Class DeviceLocal(Of T As Object)
		Protected Friend backingMap As IDictionary(Of Integer, T) = New ConcurrentDictionary(Of Integer, T)()
		Protected Friend locksMap As IList(Of ReentrantReadWriteLock) = New List(Of ReentrantReadWriteLock)()
		Protected Friend updatesMap As IList(Of AtomicInteger) = New List(Of AtomicInteger)()
		Protected Friend ReadOnly delayedMode As Boolean

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile org.nd4j.linalg.api.ndarray.INDArray delayedArray;
		Protected Friend delayedArray As INDArray

		Protected Friend lastSettledDevice As Integer = -1

		Public Sub New(ByVal delayedMode As Boolean)
			Me.delayedMode = delayedMode

			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices
			For i As Integer = 0 To numDevices - 1
				locksMap.Add(New ReentrantReadWriteLock())
				updatesMap.Add(New AtomicInteger(-1))
			Next i
		End Sub

		''' <summary>
		''' This method returns object local to current deviceId
		''' 
		''' @return
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Nullable public T get()
		Public Overridable Function get() As T
			Return get(Nd4j.AffinityManager.getDeviceForCurrentThread())
		End Function

		''' <summary>
		''' This method returns object local to target device
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Nullable public T get(int deviceId)
		Public Overridable Function get(ByVal deviceId As Integer) As T
			Try
				locksMap(deviceId).readLock().lock()
				Return backingMap(deviceId)
			Finally
				locksMap(deviceId).readLock().unlock()
			End Try
		End Function

		''' <summary>
		''' This method sets object for specific device
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="object"> </param>
		Public Overridable Sub set(ByVal deviceId As Integer, ByVal [object] As T)
			Try
				locksMap(deviceId).writeLock().lock()
				backingMap(deviceId) = [object]
			Finally
				locksMap(deviceId).writeLock().unlock()
			End Try
		End Sub

		''' <summary>
		''' This method sets object for current device
		''' </summary>
		''' <param name="object"> </param>
		Public Overridable Sub set(ByVal [object] As T)
			set(Nd4j.AffinityManager.getDeviceForCurrentThread(), [object])
		End Sub


		''' <summary>
		''' This method removes object stored for current device
		''' 
		''' </summary>
		Public Overridable Sub clear()
			Dim deviceId As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()
			Try
				locksMap(deviceId).writeLock().lock()
				backingMap.Remove(deviceId)
			Finally
				locksMap(deviceId).writeLock().unlock()
			End Try
		End Sub
	End Class

End Namespace