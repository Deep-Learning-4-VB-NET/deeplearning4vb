Imports System.Collections.Generic
Imports CircularFifoQueue = org.apache.commons.collections4.queue.CircularFifoQueue
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.rl4j.support


	Public Class MockHistoryProcessor
		Implements IHistoryProcessor

		Public startMonitorCallCount As Integer = 0
		Public stopMonitorCallCount As Integer = 0

		Private ReadOnly config As Configuration
'JAVA TO VB CONVERTER NOTE: The field history was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly history_Conflict As CircularFifoQueue(Of INDArray)

		Public ReadOnly recordCalls As List(Of INDArray)
		Public ReadOnly addCalls As List(Of INDArray)

		Public Sub New(ByVal config As Configuration)

			Me.config = config
			history_Conflict = New CircularFifoQueue(Of INDArray)(config.getHistoryLength())
			recordCalls = New List(Of INDArray)()
			addCalls = New List(Of INDArray)()
		End Sub

		Public Overridable ReadOnly Property Conf As Configuration Implements IHistoryProcessor.getConf
			Get
				Return config
			End Get
		End Property

		Public Overridable ReadOnly Property History As INDArray() Implements IHistoryProcessor.getHistory
			Get
				Dim array(Conf.getHistoryLength() - 1) As INDArray
				Dim i As Integer = 0
				Do While i < config.getHistoryLength()
					array(i) = history_Conflict.get(i).castTo(Nd4j.dataType())
					i += 1
				Loop
				Return array
			End Get
		End Property

		Public Overridable Sub record(ByVal image As INDArray) Implements IHistoryProcessor.record
			recordCalls.Add(image)
		End Sub

		Public Overridable Sub add(ByVal image As INDArray) Implements IHistoryProcessor.add
			addCalls.Add(image)
			history_Conflict.add(image)
		End Sub

		Public Overridable Sub startMonitor(ByVal filename As String, ByVal shape() As Integer) Implements IHistoryProcessor.startMonitor
			startMonitorCallCount += 1
		End Sub

		Public Overridable Sub stopMonitor() Implements IHistoryProcessor.stopMonitor
			stopMonitorCallCount += 1
		End Sub

		Public Overridable ReadOnly Property Monitoring As Boolean Implements IHistoryProcessor.isMonitoring
			Get
				Return False
			End Get
		End Property

		Public Overridable ReadOnly Property Scale As Double Implements IHistoryProcessor.getScale
			Get
				Return 255.0
			End Get
		End Property
	End Class

End Namespace