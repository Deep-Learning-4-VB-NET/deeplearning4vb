Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports TaskContext = org.apache.spark.TaskContext
Imports TaskContextHelper = org.apache.spark.TaskContextHelper
Imports AsyncDataSetIterator = org.nd4j.linalg.dataset.AsyncDataSetIterator
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports DataSetCallback = org.nd4j.linalg.dataset.callbacks.DataSetCallback
Imports DefaultCallback = org.nd4j.linalg.dataset.callbacks.DefaultCallback
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.spark.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SparkADSI extends org.nd4j.linalg.dataset.AsyncDataSetIterator
	<Serializable>
	Public Class SparkADSI
		Inherits AsyncDataSetIterator

		Protected Friend context As TaskContext

		Protected Friend Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator)
			Me.New(baseIterator, 8)
		End Sub

		Public Sub New(ByVal iterator As DataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of DataSet))
			Me.New(iterator, queueSize, queue, True)
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal queueSize As Integer)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of DataSet)(queueSize))
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of DataSet)(queueSize), useWorkspace)
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean, ByVal deviceId As Integer?)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of DataSet)(queueSize), useWorkspace, New DefaultCallback(), deviceId)
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of DataSet)(queueSize), useWorkspace, callback)
		End Sub

		Public Sub New(ByVal iterator As DataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of DataSet), ByVal useWorkspace As Boolean)
			Me.New(iterator, queueSize, queue, useWorkspace, New DefaultCallback())
		End Sub

		Public Sub New(ByVal iterator As DataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of DataSet), ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback)
			Me.New(iterator, queueSize, queue, useWorkspace, callback, Nd4j.AffinityManager.getDeviceForCurrentThread())
		End Sub

		Public Sub New(ByVal iterator As DataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of DataSet), ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback, ByVal deviceId As Integer?)
			Me.New()

			If queueSize < 2 Then
				queueSize = 2
			End If

			Me.deviceId = deviceId
			Me.callback = callback
			Me.useWorkspace = useWorkspace
			Me.buffer = queue
			Me.prefetchSize = queueSize
			Me.backedIterator = iterator
			Me.workspaceId = "SADSI_ITER-" & System.Guid.randomUUID().ToString()

			If iterator.resetSupported() Then
				Me.backedIterator.reset()
			End If

			context = TaskContext.get()

			Me.thread = New SparkPrefetchThread(Me, buffer, iterator, terminator, Nothing, Nd4j.AffinityManager.getDeviceForCurrentThread())

			''' <summary>
			''' We want to ensure, that background thread will have the same thread->device affinity, as master thread
			''' </summary>

			thread.setDaemon(True)
			thread.Start()
		End Sub

		Protected Friend Overrides Sub externalCall()
			TaskContextHelper.setTaskContext(context)

		End Sub

		Public Class SparkPrefetchThread
			Inherits AsyncPrefetchThread

			Private ReadOnly outerInstance As SparkADSI


			Protected Friend Sub New(ByVal outerInstance As SparkADSI, ByVal queue As BlockingQueue(Of DataSet), ByVal iterator As DataSetIterator, ByVal terminator As DataSet, ByVal workspace As MemoryWorkspace, ByVal deviceId As Integer)
				MyBase.New(queue, iterator, terminator, workspace, deviceId)
				Me.outerInstance = outerInstance
			End Sub


		End Class
	End Class

End Namespace