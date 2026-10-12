Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports TaskContext = org.apache.spark.TaskContext
Imports TaskContextHelper = org.apache.spark.TaskContextHelper
Imports AsyncMultiDataSetIterator = org.nd4j.linalg.dataset.AsyncMultiDataSetIterator
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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
'ORIGINAL LINE: @Slf4j public class SparkAMDSI extends org.nd4j.linalg.dataset.AsyncMultiDataSetIterator
	<Serializable>
	Public Class SparkAMDSI
		Inherits AsyncMultiDataSetIterator

		Protected Friend context As TaskContext

		Protected Friend Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal baseIterator As MultiDataSetIterator)
			Me.New(baseIterator, 8)
		End Sub

		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of MultiDataSet))
			Me.New(iterator, queueSize, queue, True)
		End Sub

		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal queueSize As Integer)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of MultiDataSet)(queueSize))
		End Sub

		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of MultiDataSet)(queueSize), useWorkspace)
		End Sub

		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean, ByVal deviceId As Integer?)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of MultiDataSet)(queueSize), useWorkspace, New DefaultCallback(), deviceId)
		End Sub

		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of MultiDataSet)(queueSize), useWorkspace, callback)
		End Sub

		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of MultiDataSet), ByVal useWorkspace As Boolean)
			Me.New(iterator, queueSize, queue, useWorkspace, Nothing)
		End Sub

		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of MultiDataSet), ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback)
			Me.New(iterator, queueSize, queue, useWorkspace, callback, Nd4j.AffinityManager.getDeviceForCurrentThread())
		End Sub

		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of MultiDataSet), ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback, ByVal deviceId As Integer?)
			Me.New()

			If queueSize < 2 Then
				queueSize = 2
			End If

			Me.callback = callback
			Me.buffer = queue
			Me.backedIterator = iterator
			Me.useWorkspaces = useWorkspace
			Me.prefetchSize = queueSize
			Me.workspaceId = "SAMDSI_ITER-" & System.Guid.randomUUID().ToString()
			Me.deviceId = deviceId

			If iterator.resetSupported() Then
				Me.backedIterator.reset()
			End If

			Me.thread = New SparkPrefetchThread(Me, buffer, iterator, terminator, Nd4j.AffinityManager.getDeviceForCurrentThread())

			context = TaskContext.get()

			thread.setDaemon(True)
			thread.Start()
		End Sub

		Protected Friend Overrides Sub externalCall()
			TaskContextHelper.setTaskContext(context)
		End Sub

		Protected Friend Class SparkPrefetchThread
			Inherits AsyncPrefetchThread

			Private ReadOnly outerInstance As SparkAMDSI


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected SparkPrefetchThread(@NonNull BlockingQueue<org.nd4j.linalg.dataset.api.MultiDataSet> queue, @NonNull MultiDataSetIterator iterator, @NonNull MultiDataSet terminator, int deviceId)
			Protected Friend Sub New(ByVal outerInstance As SparkAMDSI, ByVal queue As BlockingQueue(Of MultiDataSet), ByVal iterator As MultiDataSetIterator, ByVal terminator As MultiDataSet, ByVal deviceId As Integer)
				MyBase.New(queue, iterator, terminator, deviceId)
				Me.outerInstance = outerInstance
			End Sub
		End Class
	End Class

End Namespace