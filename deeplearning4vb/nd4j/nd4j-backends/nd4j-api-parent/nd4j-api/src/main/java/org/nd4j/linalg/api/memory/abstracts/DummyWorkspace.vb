Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
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

Namespace org.nd4j.linalg.api.memory.abstracts

	Public Class DummyWorkspace
		Implements MemoryWorkspace

'JAVA TO VB CONVERTER NOTE: The field parentWorkspace was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend parentWorkspace_Conflict As MemoryWorkspace

		''' <summary>
		''' This method returns WorkspaceConfiguration bean that was used for given Workspace instance
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property WorkspaceConfiguration As WorkspaceConfiguration Implements MemoryWorkspace.getWorkspaceConfiguration
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' This method returns Id of this workspace
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Id As String Implements MemoryWorkspace.getId
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property ThreadId As Long? Implements MemoryWorkspace.getThreadId
			Get
				Return -1L
			End Get
		End Property

		Public Overridable ReadOnly Property DeviceId As Integer Implements MemoryWorkspace.getDeviceId
			Get
				Return 0
			End Get
		End Property

		Public Overridable ReadOnly Property WorkspaceType As Type Implements MemoryWorkspace.getWorkspaceType
			Get
				Return Type.DUMMY
			End Get
		End Property

		''' <summary>
		''' This method does allocation from a given Workspace
		''' </summary>
		''' <param name="requiredMemory"> allocation size, in bytes </param>
		''' <param name="dataType">       dataType that is going to be used </param>
		''' <param name="initialize">
		''' @return </param>
		Public Overridable Function alloc(ByVal requiredMemory As Long, ByVal dataType As DataType, ByVal initialize As Boolean) As PagedPointer Implements MemoryWorkspace.alloc
			Throw New System.NotSupportedException("DummyWorkspace shouldn't be used for allocation")
		End Function

		''' <summary>
		''' This method does allocation from a given Workspace
		''' </summary>
		''' <param name="requiredMemory"> allocation size, in bytes </param>
		''' <param name="kind">           MemoryKind for allocation </param>
		''' <param name="dataType">       dataType that is going to be used </param>
		''' <param name="initialize">
		''' @return </param>
		Public Overridable Function alloc(ByVal requiredMemory As Long, ByVal kind As MemoryKind, ByVal dataType As DataType, ByVal initialize As Boolean) As PagedPointer Implements MemoryWorkspace.alloc
			Throw New System.NotSupportedException("DummyWorkspace shouldn't be used for allocation")
		End Function

		Public Overridable ReadOnly Property GenerationId As Long Implements MemoryWorkspace.getGenerationId
			Get
				Return 0L
			End Get
		End Property

		''' <summary>
		''' This method notifies given Workspace that new use cycle is starting now
		''' 
		''' @return
		''' </summary>
		Public Overridable Function notifyScopeEntered() As MemoryWorkspace
			parentWorkspace_Conflict = Nd4j.MemoryManager.CurrentWorkspace

			Nd4j.MemoryManager.CurrentWorkspace = Nothing
			Return Me
		End Function

		''' <summary>
		''' This method TEMPORARY enters this workspace, without reset applied
		''' 
		''' @return
		''' </summary>
		Public Overridable Function notifyScopeBorrowed() As MemoryWorkspace
			Return Nothing
		End Function

		''' <summary>
		''' This method notifies given Workspace that use cycle just ended
		''' 
		''' @return
		''' </summary>
		Public Overridable Function notifyScopeLeft() As MemoryWorkspace
			close()
			Return Me
		End Function

		''' <summary>
		''' This method returns True if scope was opened, and not closed yet.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ScopeActive As Boolean Implements MemoryWorkspace.isScopeActive
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' This method causes Workspace initialization
		''' <para>
		''' PLEASE NOTE: This call will have no effect on previously initialized Workspace
		''' </para>
		''' </summary>
		Public Overridable Sub initializeWorkspace() Implements MemoryWorkspace.initializeWorkspace

		End Sub

		''' <summary>
		''' This method causes Workspace destruction: all memory allocations are released after this call.
		''' </summary>
		Public Overridable Sub destroyWorkspace() Implements MemoryWorkspace.destroyWorkspace

		End Sub

		Public Overridable Sub destroyWorkspace(ByVal extended As Boolean) Implements MemoryWorkspace.destroyWorkspace

		End Sub

		''' <summary>
		''' This method allows you to temporary disable/enable given Workspace use.
		''' If turned off - direct memory allocations will be used.
		''' </summary>
		''' <param name="isEnabled"> </param>
		Public Overridable Sub toggleWorkspaceUse(ByVal isEnabled As Boolean) Implements MemoryWorkspace.toggleWorkspaceUse

		End Sub

		''' <summary>
		''' This method returns amount of memory consumed in last successful cycle, in bytes
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ThisCycleAllocations As Long Implements MemoryWorkspace.getThisCycleAllocations
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' This method enabled debugging mode for this workspace
		''' </summary>
		''' <param name="reallyEnable"> </param>
		Public Overridable Sub enableDebug(ByVal reallyEnable As Boolean) Implements MemoryWorkspace.enableDebug

		End Sub


		''' <summary>
		''' This method returns amount of memory consumed in last successful cycle, in bytes
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property LastCycleAllocations As Long Implements MemoryWorkspace.getLastCycleAllocations
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' This method returns amount of memory consumed by largest successful cycle, in bytes
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property MaxCycleAllocations As Long Implements MemoryWorkspace.getMaxCycleAllocations
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' This methos returns current allocated size of this workspace
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property CurrentSize As Long Implements MemoryWorkspace.getCurrentSize
			Get
				Return 0
			End Get
		End Property

		Public Overridable Sub close() Implements MemoryWorkspace.close
			Nd4j.MemoryManager.CurrentWorkspace = parentWorkspace_Conflict
		End Sub

		''' <summary>
		''' This method returns parent Workspace, if any. Null if there's none.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ParentWorkspace As MemoryWorkspace
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Function tagOutOfScopeUse() As MemoryWorkspace
			Return Me
		End Function

		Public Overridable WriteOnly Property PreviousWorkspace As MemoryWorkspace
			Set(ByVal memoryWorkspace As MemoryWorkspace)
				parentWorkspace_Conflict = memoryWorkspace
			End Set
		End Property

		Public Overridable ReadOnly Property CurrentOffset As Long Implements MemoryWorkspace.getCurrentOffset
			Get
				Return 0
			End Get
		End Property

		Public Overridable ReadOnly Property UniqueId As String
			Get
				Return System.Guid.randomUUID().ToString()
			End Get
		End Property

		Public Overridable Function deallocator() As Deallocator
			Return New DeallocatorAnonymousInnerClass(Me)
		End Function

		Private Class DeallocatorAnonymousInnerClass
			Implements Deallocator

			Private ReadOnly outerInstance As DummyWorkspace

			Public Sub New(ByVal outerInstance As DummyWorkspace)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub deallocate() Implements Deallocator.deallocate
				' no-op
			End Sub
		End Class

		Public Overridable Function targetDevice() As Integer
			Return 0
		End Function

		Public Overridable ReadOnly Property PrimaryOffset As Long Implements MemoryWorkspace.getPrimaryOffset
			Get
				Return 0
			End Get
		End Property
	End Class

End Namespace