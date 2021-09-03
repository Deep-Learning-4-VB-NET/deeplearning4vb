Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer

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

	Public Interface MemoryWorkspace
		Inherits AutoCloseable, Deallocatable

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		String DEFAULT_ID = "DefaultWorkspace";

		Friend Enum Type
			''' <summary>
			''' This mode means you have dummy workspace here. It doesn't provide any functionality.
			''' </summary>
			DUMMY

			''' <summary>
			''' Most regular workspace. It starts somewhere, and ends somewhere. It has limits, aka scope of use.
			''' </summary>
			SCOPED

			''' <summary>
			''' Special workspace mode: circular buffer. Workspace is never closed, and gets reset only once end reached.
			''' </summary>
			CIRCULAR
		End Enum

		''' <summary>
		''' This method returns WorkspaceConfiguration bean that was used for given Workspace instance
		''' 
		''' @return
		''' </summary>
		ReadOnly Property WorkspaceConfiguration As WorkspaceConfiguration

		''' <summary>
		''' This method returns Type of this workspace
		''' 
		''' @return
		''' </summary>
		ReadOnly Property WorkspaceType As Type

		''' <summary>
		''' This method returns Id of this workspace
		''' 
		''' @return
		''' </summary>
		ReadOnly Property Id As String

		''' <summary>
		''' Returns deviceId for this workspace
		''' 
		''' @return
		''' </summary>
		ReadOnly Property DeviceId As Integer

		''' <summary>
		''' This method returns threadId where this workspace was created
		''' 
		''' @return
		''' </summary>
		ReadOnly Property ThreadId As Long?

		''' <summary>
		''' This method returns current generation Id
		''' @return
		''' </summary>
		ReadOnly Property GenerationId As Long

		''' <summary>
		''' This method does allocation from a given Workspace
		''' </summary>
		''' <param name="requiredMemory"> allocation size, in bytes </param>
		''' <param name="dataType"> dataType that is going to be used
		''' @return </param>
		Function alloc(ByVal requiredMemory As Long, ByVal dataType As DataType, ByVal initialize As Boolean) As PagedPointer


		ReadOnly Property PrimaryOffset As Long

		''' <summary>
		''' This method does allocation from a given Workspace
		''' </summary>
		''' <param name="requiredMemory"> allocation size, in bytes </param>
		''' <param name="kind"> MemoryKind for allocation </param>
		''' <param name="dataType"> dataType that is going to be used
		''' @return </param>
		Function alloc(ByVal requiredMemory As Long, ByVal kind As MemoryKind, ByVal dataType As DataType, ByVal initialize As Boolean) As PagedPointer

		''' <summary>
		''' This method notifies given Workspace that new use cycle is starting now
		''' 
		''' @return
		''' </summary>
		Function notifyScopeEntered() As MemoryWorkspace

		''' <summary>
		''' This method TEMPORARY enters this workspace, without reset applied
		''' 
		''' @return
		''' </summary>
		Function notifyScopeBorrowed() As MemoryWorkspace

		''' <summary>
		''' This method notifies given Workspace that use cycle just ended
		''' 
		''' @return
		''' </summary>
		Function notifyScopeLeft() As MemoryWorkspace

		''' <summary>
		''' This method returns True if scope was opened, and not closed yet.
		''' 
		''' @return
		''' </summary>
		ReadOnly Property ScopeActive As Boolean

		''' <summary>
		''' This method causes Workspace initialization
		''' 
		''' PLEASE NOTE: This call will have no effect on previously initialized Workspace
		''' </summary>
		Sub initializeWorkspace()

		''' <summary>
		''' This method causes Workspace destruction: all memory allocations are released after this call.
		''' </summary>
		Sub destroyWorkspace()

		Sub destroyWorkspace(ByVal extended As Boolean)

		''' <summary>
		''' This method allows you to temporary disable/enable given Workspace use.
		''' If turned off - direct memory allocations will be used.
		''' </summary>
		''' <param name="isEnabled"> </param>
		Sub toggleWorkspaceUse(ByVal isEnabled As Boolean)

		''' <summary>
		''' This method returns amount of memory consumed in current cycle, in bytes
		''' 
		''' @return
		''' </summary>
		ReadOnly Property ThisCycleAllocations As Long

		''' <summary>
		''' This method enabled debugging mode for this workspace
		''' </summary>
		''' <param name="reallyEnable"> </param>
		Sub enableDebug(ByVal reallyEnable As Boolean)

		''' <summary>
		''' This method returns amount of memory consumed in last successful cycle, in bytes
		''' 
		''' @return
		''' </summary>
		ReadOnly Property LastCycleAllocations As Long

		''' <summary>
		''' This method returns amount of memory consumed by largest successful cycle, in bytes
		''' @return
		''' </summary>
		ReadOnly Property MaxCycleAllocations As Long

		''' <summary>
		''' This methos returns current allocated size of this workspace
		''' 
		''' @return
		''' </summary>
		ReadOnly Property CurrentSize As Long

		''' <summary>
		''' This method is for compatibility with "try-with-resources" java blocks.
		''' Internally it should be equal to notifyScopeLeft() method
		''' 
		''' </summary>
		Overrides Sub close()

		''' <summary>
		''' This method returns parent Workspace, if any. Null if there's none.
		''' @return
		''' </summary>
		ReadOnly Property ParentWorkspace As MemoryWorkspace

		''' <summary>
		''' This method temporary disables this workspace
		''' 
		''' @return
		''' </summary>
		Function tagOutOfScopeUse() As MemoryWorkspace

		''' <summary>
		''' Set the previous workspace, if any<br>
		''' NOTE: this method should only be used if you are fully aware of the consequences of doing so. Incorrect use
		''' of this method may leave workspace management in an invalid/indeterminant state!
		''' </summary>
		''' <param name="memoryWorkspace"> Workspace to set as the previous workspace. This is the workspace that will become active
		'''                        when this workspace is closed. </param>
		WriteOnly Property PreviousWorkspace As MemoryWorkspace

		''' <summary>
		''' This mehtod returns current offset within buffer
		''' @return
		''' </summary>
		ReadOnly Property CurrentOffset As Long
	End Interface

End Namespace