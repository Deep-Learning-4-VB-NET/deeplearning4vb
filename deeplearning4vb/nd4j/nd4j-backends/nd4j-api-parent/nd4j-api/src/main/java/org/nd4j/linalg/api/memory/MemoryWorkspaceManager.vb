Imports System.Collections.Generic
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports DebugMode = org.nd4j.linalg.api.memory.enums.DebugMode

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

Namespace org.nd4j.linalg.api.memory



	Public Interface MemoryWorkspaceManager

		''' <summary>
		''' Returns globally unique ID
		''' 
		''' @return
		''' </summary>
		ReadOnly Property UUID As String

		''' <summary>
		''' This method returns current debug mode active in this JVM
		''' @return
		''' </summary>
		Property DebugMode As DebugMode


		''' <summary>
		''' This method sets default workspace configuration for this provider instance
		''' </summary>
		''' <param name="configuration"> </param>
		WriteOnly Property DefaultWorkspaceConfiguration As WorkspaceConfiguration

		''' <summary>
		''' This method builds new Workspace with given configuration
		''' </summary>
		''' <param name="configuration">
		''' @return </param>
		Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration) As MemoryWorkspace

		''' <summary>
		''' This method builds new Workspace with default configuration
		''' 
		''' @return
		''' </summary>
		Function createNewWorkspace() As MemoryWorkspace

		''' <summary>
		''' This method builds new Workspace with given configuration
		''' </summary>
		''' <param name="configuration">
		''' @return </param>
		Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace


		''' <summary>
		''' This method builds new Workspace with given configuration
		''' </summary>
		''' <param name="configuration">
		''' @return </param>
		Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String, ByVal deviceId As Integer?) As MemoryWorkspace

		''' <summary>
		''' This method returns you current default Workspace for current Thread
		''' 
		''' PLEASE NOTE: If Workspace wasn't defined, new Workspace will be created using current default configuration
		''' 
		''' @return
		''' </summary>
		Property WorkspaceForCurrentThread As MemoryWorkspace

		''' <summary>
		''' This method returns you Workspace for a given Id for current Thread
		''' 
		''' PLEASE NOTE: If Workspace wasn't defined, new Workspace will be created using current default configuration
		''' 
		''' @return
		''' </summary>
		Function getWorkspaceForCurrentThread(ByVal id As String) As MemoryWorkspace

		''' <summary>
		''' This method returns you Workspace for a given Id for current Thread
		''' 
		''' PLEASE NOTE: If Workspace wasn't defined, new Workspace will be created using given configuration
		''' 
		''' @return
		''' </summary>
		Function getWorkspaceForCurrentThread(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace


		''' <summary>
		''' This method allows you to set given Workspace for spacific Id for current Thread
		''' </summary>
		''' <param name="workspace"> </param>
		Sub setWorkspaceForCurrentThread(ByVal workspace As MemoryWorkspace, ByVal id As String)

		''' <summary>
		''' This method allows you to destroy given Workspace
		''' </summary>
		''' <param name="workspace"> </param>
		Sub destroyWorkspace(ByVal workspace As MemoryWorkspace)

		''' <summary>
		''' This method destroys & deallocates all Workspaces for a calling Thread
		''' 
		''' PLEASE NOTE: This method is NOT safe
		''' </summary>
		Sub destroyAllWorkspacesForCurrentThread()

		''' <summary>
		''' This method destroys current Workspace for current Thread
		''' </summary>
		Sub destroyWorkspace()

		''' <summary>
		''' This method gets & activates default workspace
		''' 
		''' @return
		''' </summary>
		ReadOnly Property AndActivateWorkspace As MemoryWorkspace

		''' <summary>
		''' This method gets & activates workspace with a given Id
		''' 
		''' @return
		''' </summary>
		Function getAndActivateWorkspace(ByVal id As String) As MemoryWorkspace

		''' <summary>
		''' This method gets & activates default with a given configuration and Id
		''' 
		''' @return
		''' </summary>
		Function getAndActivateWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace

		''' <summary>
		''' This method checks, if Workspace with a given Id was created before this call
		''' </summary>
		''' <param name="id">
		''' @return </param>
		Function checkIfWorkspaceExists(ByVal id As String) As Boolean

		''' <summary>
		''' This method checks, if Workspace with a given Id was created before this call, AND is active at the moment of call
		''' </summary>
		''' <param name="id">
		''' @return </param>
		Function checkIfWorkspaceExistsAndActive(ByVal id As String) As Boolean

		''' <summary>
		''' This method temporary opens block out of any workspace scope.
		''' 
		''' PLEASE NOTE: Do not forget to close this block.
		''' 
		''' @return
		''' </summary>
		Function scopeOutOfWorkspaces() As MemoryWorkspace

		''' <summary>
		''' This method prints out allocation statistics for current thread
		''' </summary>
		Sub printAllocationStatisticsForCurrentThread()

		''' <summary>
		''' This method returns list of workspace IDs for current thread
		''' 
		''' @return
		''' </summary>
		ReadOnly Property AllWorkspacesIdsForCurrentThread As IList(Of String)

		''' <summary>
		''' This method returns all workspaces for current thread
		''' </summary>
		ReadOnly Property AllWorkspacesForCurrentThread As IList(Of MemoryWorkspace)

		''' <summary>
		''' Determine if there are any workspaces open for the current thread.
		''' </summary>
		''' <returns> True if any workspaces are open for this thread, false otherwise </returns>
		Function anyWorkspaceActiveForCurrentThread() As Boolean
	End Interface

End Namespace