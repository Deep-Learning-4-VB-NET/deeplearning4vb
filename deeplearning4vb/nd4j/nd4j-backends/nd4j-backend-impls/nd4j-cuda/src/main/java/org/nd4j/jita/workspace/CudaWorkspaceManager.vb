Imports NonNull = lombok.NonNull
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports DebugMode = org.nd4j.linalg.api.memory.enums.DebugMode
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DummyWorkspace = org.nd4j.linalg.api.memory.abstracts.DummyWorkspace
Imports BasicWorkspaceManager = org.nd4j.linalg.api.memory.provider.BasicWorkspaceManager

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

Namespace org.nd4j.jita.workspace

	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class CudaWorkspaceManager
		Inherits BasicWorkspaceManager

		Public Sub New()
			MyBase.New()
		End Sub

		Protected Friend Overridable Function newWorkspace(ByVal configuration As WorkspaceConfiguration) As MemoryWorkspace
			Return If(Nd4j.WorkspaceManager.DebugMode = DebugMode.BYPASS_EVERYTHING, New DummyWorkspace(), New CudaWorkspace(configuration))
		End Function

		Protected Friend Overridable Function newWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
			Return If(Nd4j.WorkspaceManager.DebugMode = DebugMode.BYPASS_EVERYTHING, New DummyWorkspace(), New CudaWorkspace(configuration, id))
		End Function

		Protected Friend Overridable Function newWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String, ByVal deviceId As Integer) As MemoryWorkspace
			Return If(Nd4j.WorkspaceManager.DebugMode = DebugMode.BYPASS_EVERYTHING, New DummyWorkspace(), New CudaWorkspace(configuration, id, deviceId))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.memory.MemoryWorkspace createNewWorkspace(@NonNull WorkspaceConfiguration configuration)
		Public Overridable Overloads Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration) As MemoryWorkspace
			ensureThreadExistense()

			Dim workspace As MemoryWorkspace = newWorkspace(configuration)

			backingMap.get().put(workspace.Id, workspace)

			If Nd4j.WorkspaceManager.DebugMode <> DebugMode.BYPASS_EVERYTHING Then
				pickReference(workspace)
			End If

			Return workspace
		End Function

		Protected Friend Overrides Sub pickReference(ByVal w As MemoryWorkspace)
			Nd4j.DeallocatorService.pickObject(w)
		End Sub

		Public Overrides Function createNewWorkspace() As MemoryWorkspace
			ensureThreadExistense()

			Dim workspace As MemoryWorkspace = newWorkspace(defaultConfiguration)

			backingMap.get().put(workspace.Id, workspace)

			If Nd4j.WorkspaceManager.DebugMode <> DebugMode.BYPASS_EVERYTHING Then
				pickReference(workspace)
			End If

			Return workspace
		End Function


		Public Overrides Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
			ensureThreadExistense()

			Dim workspace As MemoryWorkspace = newWorkspace(configuration, id)

			backingMap.get().put(id, workspace)

			If Nd4j.WorkspaceManager.DebugMode <> DebugMode.BYPASS_EVERYTHING Then
				pickReference(workspace)
			End If

			Return workspace
		End Function

		Public Overridable Overloads Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String, ByVal deviceId As Integer?) As MemoryWorkspace
			ensureThreadExistense()

			Dim workspace As MemoryWorkspace = newWorkspace(configuration, id, deviceId)

			backingMap.get().put(id, workspace)

			If Nd4j.WorkspaceManager.DebugMode <> DebugMode.BYPASS_EVERYTHING Then
				pickReference(workspace)
			End If

			Return workspace
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.memory.MemoryWorkspace getWorkspaceForCurrentThread(@NonNull WorkspaceConfiguration configuration, @NonNull String id)
		Public Overridable Overloads Function getWorkspaceForCurrentThread(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
			ensureThreadExistense()

			Dim workspace As MemoryWorkspace = backingMap.get().get(id)
			If workspace Is Nothing Then
				workspace = newWorkspace(configuration, id)
				backingMap.get().put(id, workspace)

				If Nd4j.WorkspaceManager.DebugMode <> DebugMode.BYPASS_EVERYTHING Then
					pickReference(workspace)
				End If
			End If

			Return workspace
		End Function


	End Class

End Namespace