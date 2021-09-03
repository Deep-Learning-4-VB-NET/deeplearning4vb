Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports DataSetCallback = org.nd4j.linalg.dataset.callbacks.DataSetCallback

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

Namespace org.deeplearning4j.datasets.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class AsyncMultiDataSetIterator extends org.nd4j.linalg.dataset.AsyncMultiDataSetIterator
	<Obsolete>
	Public Class AsyncMultiDataSetIterator
		Inherits org.nd4j.linalg.dataset.AsyncMultiDataSetIterator

		Public Sub New(ByVal baseIterator As MultiDataSetIterator)
			MyBase.New(baseIterator)
		End Sub

		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of MultiDataSet))
			MyBase.New(iterator, queueSize, queue)
		End Sub

		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal queueSize As Integer)
			MyBase.New(baseIterator, queueSize)
		End Sub

		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean)
			MyBase.New(baseIterator, queueSize, useWorkspace)
		End Sub

		Public Sub New(ByVal baseIterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean, ByVal deviceId As Integer?)
			MyBase.New(baseIterator, queueSize, useWorkspace, deviceId)
		End Sub

		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of MultiDataSet), ByVal useWorkspace As Boolean)
			MyBase.New(iterator, queueSize, queue, useWorkspace)
		End Sub

		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of MultiDataSet), ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback)
			MyBase.New(iterator, queueSize, queue, useWorkspace, callback)
		End Sub

		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of MultiDataSet), ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback, ByVal deviceId As Integer?)
			MyBase.New(iterator, queueSize, queue, useWorkspace, callback, deviceId)
		End Sub
	End Class

End Namespace