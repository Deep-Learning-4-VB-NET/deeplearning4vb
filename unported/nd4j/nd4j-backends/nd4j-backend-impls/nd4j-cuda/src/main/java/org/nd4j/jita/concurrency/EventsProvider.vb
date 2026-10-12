Imports System
Imports System.Collections.Generic
Imports cudaEvent_t = org.nd4j.jita.allocator.pointers.cuda.cudaEvent_t
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.nd4j.jita.concurrency


	''' 
	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	<Obsolete>
	Public Class EventsProvider
		Private queue As IList(Of ConcurrentLinkedQueue(Of cudaEvent_t)) = New List(Of ConcurrentLinkedQueue(Of cudaEvent_t))()
		Private newCounter As New AtomicLong(0)
		Private cacheCounter As New AtomicLong(0)

		Public Sub New()
			Dim numDev As Integer = Nd4j.AffinityManager.NumberOfDevices

			For i As Integer = 0 To numDev - 1
				queue.Add(New ConcurrentLinkedQueue(Of cudaEvent_t)())
			Next i
		End Sub

		Public Overridable ReadOnly Property Event As cudaEvent_t
			Get
				Dim deviceId As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()
				Dim e As cudaEvent_t = queue(deviceId).poll()
				If e Is Nothing Then
					e = New cudaEvent_t(NativeOpsHolder.Instance.getDeviceNativeOps().createEvent())
					e.setDeviceId(deviceId)
					newCounter.incrementAndGet()
				Else
					cacheCounter.incrementAndGet()
				End If
    
				Return e
			End Get
		End Property

		Public Overridable Sub storeEvent(ByVal [event] As cudaEvent_t)
			If [event] IsNot Nothing Then
				'            NativeOpsHolder.getInstance().getDeviceNativeOps().destroyEvent(event);
				queue([event].getDeviceId()).add([event])
			End If
		End Sub

		Public Overridable ReadOnly Property EventsNumber As Long
			Get
				Return newCounter.get()
			End Get
		End Property

		Public Overridable ReadOnly Property CachedNumber As Long
			Get
				Return cacheCounter.get()
			End Get
		End Property

	End Class

End Namespace