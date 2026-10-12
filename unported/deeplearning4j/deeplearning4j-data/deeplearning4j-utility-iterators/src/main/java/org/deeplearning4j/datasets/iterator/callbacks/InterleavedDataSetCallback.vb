Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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

Namespace org.deeplearning4j.datasets.iterator.callbacks


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class InterleavedDataSetCallback implements DataSetCallback
	Public Class InterleavedDataSetCallback
		Implements DataSetCallback

		Private workspaces As IList(Of MemoryWorkspace) = New List(Of MemoryWorkspace)()
		Private bufferSize As Integer
		Private numWorkspaces As Integer

		Private isInitialized As Boolean = False

		Private counterInput As New AtomicLong(0)

		Public Sub New(ByVal bufferSize As Integer)
			Me.bufferSize = bufferSize
		End Sub

		Protected Friend Overridable Sub initializeWorkspaces(ByVal size As Long)
			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(size).overallocationLimit(bufferSize).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.NONE).build()

			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices
			Dim cDevice As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()
			For i As Integer = 0 To numDevices - 1
				Nd4j.AffinityManager.unsafeSetDevice(i)
				workspaces.Add(Nd4j.WorkspaceManager.createNewWorkspace(configuration, "IDSC-" & i, i))
			Next i

			Nd4j.AffinityManager.unsafeSetDevice(cDevice)
			numWorkspaces = numDevices
			isInitialized = True
		End Sub

		Public Overridable Sub [call](ByVal dataSet As DataSet)
			If Not isInitialized Then
				initializeWorkspaces(dataSet.MemoryFootprint)
			End If

			Nd4j.Executioner.commit()

			Dim currIdx As Integer = CInt(Math.Truncate(counterInput.getAndIncrement() Mod numWorkspaces))
			Dim currWs As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			Nd4j.MemoryManager.CurrentWorkspace = workspaces(currIdx)

			dataSet.migrate()

			Nd4j.MemoryManager.CurrentWorkspace = currWs
		End Sub

		Public Overridable Sub [call](ByVal multiDataSet As MultiDataSet)
			If Not isInitialized Then
				initializeWorkspaces(multiDataSet.MemoryFootprint)
			End If

			Nd4j.Executioner.commit()

			Dim currIdx As Integer = CInt(Math.Truncate(counterInput.getAndIncrement() Mod numWorkspaces))
			Dim currWs As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			Nd4j.MemoryManager.CurrentWorkspace = workspaces(currIdx)

			multiDataSet.migrate()

			Nd4j.MemoryManager.CurrentWorkspace = currWs
		End Sub

		Public Overridable Sub reset()
			counterInput.set(0)
		End Sub
	End Class

End Namespace