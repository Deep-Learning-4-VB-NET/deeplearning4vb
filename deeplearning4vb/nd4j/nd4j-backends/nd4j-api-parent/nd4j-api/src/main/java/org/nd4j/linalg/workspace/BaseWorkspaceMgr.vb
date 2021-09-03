Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
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

Namespace org.nd4j.linalg.workspace


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseWorkspaceMgr<T extends @Enum<T>> implements WorkspaceMgr<T>
	Public MustInherit Class BaseWorkspaceMgr(Of T As [Enum](Of T))
		Implements WorkspaceMgr(Of T)

		Public MustOverride Function castTo(ByVal arrayType As T, ByVal dataType As DataType, ByVal toCast As INDArray, ByVal dupIfCorrectType As Boolean) As INDArray
		Public MustOverride Function dup(ByVal arrayType As T, ByVal toDup As INDArray) As INDArray
		Private Const DISABLE_LEVERAGE As Boolean = False 'Mainly for debugging/optimization purposes

		Protected Friend ReadOnly scopeOutOfWs As ISet(Of T)
		Protected Friend ReadOnly configMap As IDictionary(Of T, WorkspaceConfiguration)
		Protected Friend ReadOnly workspaceNames As IDictionary(Of T, String)

		Protected Friend Sub New(ByVal scopeOutOfWs As ISet(Of T), ByVal configMap As IDictionary(Of T, WorkspaceConfiguration), ByVal workspaceNames As IDictionary(Of T, String))
			Me.scopeOutOfWs = scopeOutOfWs
			Me.configMap = configMap
			Me.workspaceNames = workspaceNames
		End Sub

		Protected Friend Sub New()
			scopeOutOfWs = New HashSet(Of T)()
			configMap = New Dictionary(Of T, WorkspaceConfiguration)()
			workspaceNames = New Dictionary(Of T, String)()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setConfiguration(@NonNull T arrayType, org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration configuration)
		Public Overridable Sub setConfiguration(ByVal arrayType As T, ByVal configuration As WorkspaceConfiguration) Implements WorkspaceMgr(Of T).setConfiguration
			configMap(arrayType) = configuration
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration getConfiguration(@NonNull T arrayType)
		Public Overridable Function getConfiguration(ByVal arrayType As T) As WorkspaceConfiguration Implements WorkspaceMgr(Of T).getConfiguration
			Return configMap(arrayType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setScopedOutFor(@NonNull T arrayType)
		Public Overridable WriteOnly Property ScopedOutFor(ByVal arrayType As T) Implements WorkspaceMgr.setScopedOutFor As T
			Set(ByVal arrayType As T)
				scopeOutOfWs.Add(arrayType)
				configMap.Remove(arrayType)
				workspaceNames.Remove(arrayType)
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean isScopedOut(@NonNull T arrayType)
		Public Overridable Function isScopedOut(ByVal arrayType As T) As Boolean Implements WorkspaceMgr(Of T).isScopedOut
			Return scopeOutOfWs.Contains(arrayType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean hasConfiguration(@NonNull T arrayType)
		Public Overridable Function hasConfiguration(ByVal arrayType As T) As Boolean Implements WorkspaceMgr(Of T).hasConfiguration
			Return scopeOutOfWs.Contains(arrayType) OrElse workspaceNames.ContainsKey(arrayType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.memory.MemoryWorkspace notifyScopeEntered(@NonNull T arrayType)
		Public Overridable Function notifyScopeEntered(ByVal arrayType As T) As MemoryWorkspace Implements WorkspaceMgr(Of T).notifyScopeEntered
			validateConfig(arrayType)

			If isScopedOut(arrayType) Then
				Return Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
			Else
				Dim ws As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(getConfiguration(arrayType), getWorkspaceName(arrayType))
				Return ws.notifyScopeEntered()
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public WorkspacesCloseable notifyScopeEntered(@NonNull T... arrayTypes)
		Public Overridable Function notifyScopeEntered(ParamArray ByVal arrayTypes() As T) As WorkspacesCloseable Implements WorkspaceMgr(Of T).notifyScopeEntered
			Dim ws(arrayTypes.Length - 1) As MemoryWorkspace
			For i As Integer = 0 To arrayTypes.Length - 1
				ws(i) = notifyScopeEntered(arrayTypes(i))
			Next i
			Return New WorkspacesCloseable(ws)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.memory.MemoryWorkspace notifyScopeBorrowed(@NonNull T arrayType)
		Public Overridable Function notifyScopeBorrowed(ByVal arrayType As T) As MemoryWorkspace Implements WorkspaceMgr(Of T).notifyScopeBorrowed
			validateConfig(arrayType)
			enforceExistsAndActive(arrayType)

			If scopeOutOfWs.Contains(arrayType) Then
				Return Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
			Else
				Dim ws As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(getConfiguration(arrayType), getWorkspaceName(arrayType))
				Return ws.notifyScopeBorrowed()
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setWorkspaceName(@NonNull T arrayType, @NonNull String name)
		Public Overridable Sub setWorkspaceName(ByVal arrayType As T, ByVal name As String) Implements WorkspaceMgr(Of T).setWorkspaceName
			workspaceNames(arrayType) = name
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public String getWorkspaceName(@NonNull T arrayType)
		Public Overridable Function getWorkspaceName(ByVal arrayType As T) As String Implements WorkspaceMgr(Of T).getWorkspaceName
			Return workspaceNames(arrayType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setWorkspace(@NonNull T forEnum, @NonNull String wsName, @NonNull WorkspaceConfiguration configuration)
		Public Overridable Sub setWorkspace(ByVal forEnum As T, ByVal wsName As String, ByVal configuration As WorkspaceConfiguration)
			If scopeOutOfWs.Contains(forEnum) Then
				scopeOutOfWs.remove(forEnum)
			End If
			setWorkspaceName(forEnum, wsName)
			setConfiguration(forEnum, configuration)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean isWorkspaceOpen(@NonNull T arrayType)
		Public Overridable Function isWorkspaceOpen(ByVal arrayType As T) As Boolean Implements WorkspaceMgr(Of T).isWorkspaceOpen
			validateConfig(arrayType)
			If Not scopeOutOfWs.Contains(arrayType) Then
				Return Nd4j.WorkspaceManager.checkIfWorkspaceExistsAndActive(getWorkspaceName(arrayType))
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void assertOpen(T arrayType, String msg) throws ND4JWorkspaceException
		Public Overridable Sub assertOpen(ByVal arrayType As T, ByVal msg As String) Implements WorkspaceMgr(Of T).assertOpen
			If Not scopeOutOfWs.Contains(arrayType) AndAlso Not isWorkspaceOpen(arrayType) Then
				Throw New ND4JWorkspaceException("Assertion failed: expected workspace for array type " & arrayType & " to be open: " & msg)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void assertNotOpen(@NonNull T arrayType, @NonNull String msg)
		Public Overridable Sub assertNotOpen(ByVal arrayType As T, ByVal msg As String) Implements WorkspaceMgr(Of T).assertNotOpen
			If Not scopeOutOfWs.Contains(arrayType) AndAlso isWorkspaceOpen(arrayType) Then
				Throw New ND4JWorkspaceException("Assertion failed: expected workspace for array type " & arrayType & " to not be open: " & msg)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void assertCurrentWorkspace(@NonNull T arrayType, String msg)
		Public Overridable Sub assertCurrentWorkspace(ByVal arrayType As T, ByVal msg As String) Implements WorkspaceMgr(Of T).assertCurrentWorkspace
			validateConfig(arrayType)
			Dim curr As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			If Not scopeOutOfWs.Contains(arrayType) AndAlso (curr Is Nothing OrElse Not getWorkspaceName(arrayType).Equals(curr.Id)) Then
				Throw New ND4JWorkspaceException("Assertion failed: expected current workspace to be """ & getWorkspaceName(arrayType) & """ (for array type " & arrayType & ") - actual current workspace is " & (If(curr Is Nothing, Nothing, curr.Id)) + (If(msg Is Nothing, "", ": " & msg)))
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray leverageTo(@NonNull T arrayType, @NonNull INDArray array)
		Public Overridable Function leverageTo(ByVal arrayType As T, ByVal array As INDArray) As INDArray
			If array Is Nothing OrElse Not array.isAttached() Then
				Return array
			End If
			validateConfig(arrayType)
			enforceExistsAndActive(arrayType)

			If Not DISABLE_LEVERAGE Then
				If scopeOutOfWs.Contains(arrayType) Then
					Return array.detach()
				End If
				Return array.leverageTo(getWorkspaceName(arrayType), True)
			Else
				If array.isAttached() Then
					If Not array.data().getParentWorkspace().getId().Equals(getWorkspaceName(arrayType)) Then
						Throw New System.InvalidOperationException("Array of type " & arrayType & " is leveraged from " & array.data().getParentWorkspace().getId() & " to " & getWorkspaceName(arrayType) & " but WorkspaceMgn.leverageTo() is currently disabled")
					End If
				End If
				Return array
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray validateArrayLocation(@NonNull T arrayType, @NonNull INDArray array, boolean migrateIfInvalid, boolean exceptionIfDetached)
		Public Overridable Function validateArrayLocation(ByVal arrayType As T, ByVal array As INDArray, ByVal migrateIfInvalid As Boolean, ByVal exceptionIfDetached As Boolean) As INDArray
			validateConfig(arrayType)

			If scopeOutOfWs.Contains(arrayType) Then
				'Array is supposed to be detached (no workspace)
				Dim ok As Boolean = Not array.isAttached()
				If Not ok Then
					If migrateIfInvalid Then
						log.trace("Migrating array of type " & arrayType & " to workspace " & getWorkspaceName(arrayType))
						Return leverageTo(arrayType, array)
					Else
						Throw New ND4JWorkspaceException("Array workspace validation failed: Array of type " & arrayType & " should be detached (no workspace) but is in workspace: " & array.data().getParentWorkspace().getId())
					End If
				Else
					'Detached array, as expected
					Return array
				End If
			End If

			'At this point: we expect the array to be in a workspace
			Dim wsNameExpected As String = getWorkspaceName(arrayType)
			If Not array.isAttached() Then
				If exceptionIfDetached Then
					Throw New ND4JWorkspaceException("Array workspace validation failed: Array of type " & arrayType & " should be in workspace """ & wsNameExpected & """ but is detached")
				Else
					Return array
				End If
			End If


			Dim wsNameAct As String = array.data().getParentWorkspace().getId()
			If Not wsNameExpected.Equals(wsNameAct) Then
				If migrateIfInvalid Then
					Return leverageTo(arrayType, array)
				Else
					Throw New ND4JWorkspaceException("Array workspace validation failed: Array of type " & arrayType & " should be in workspace """ & wsNameExpected & """ but is in workspace """ & wsNameAct & """")
				End If
			End If

			'OK - return as-is
			Return array
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray create(@NonNull T arrayType, @NonNull DataType dataType, @NonNull long... shape)
		Public Overridable Function create(ByVal arrayType As T, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray
			enforceExistsAndActive(arrayType)
			Return create(arrayType, dataType, shape, Nd4j.order())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray create(@NonNull T arrayType, @NonNull DataType dataType, @NonNull long[] shape, @NonNull char order)
		Public Overridable Function create(ByVal arrayType As T, ByVal dataType As DataType, ByVal shape() As Long, ByVal order As Char) As INDArray
			enforceExistsAndActive(arrayType)
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = notifyScopeBorrowed(arrayType)
				Return Nd4j.create(dataType, shape, order)
			End Using
		End Function

		Public Overridable Function createUninitialized(ByVal arrayType As T, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray Implements WorkspaceMgr(Of T).createUninitialized
			Return createUninitialized(arrayType, dataType, shape, Nd4j.order())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray createUninitialized(@NonNull T arrayType, @NonNull DataType dataType, @NonNull long[] shape, char order)
		Public Overridable Function createUninitialized(ByVal arrayType As T, ByVal dataType As DataType, ByVal shape() As Long, ByVal order As Char) As INDArray
			enforceExistsAndActive(arrayType)
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = notifyScopeBorrowed(arrayType)
				Return Nd4j.createUninitialized(dataType, shape, order)
			End Using
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray dup(@NonNull T arrayType, @NonNull INDArray toDup, char order)
		Public Overridable Function dup(ByVal arrayType As T, ByVal toDup As INDArray, ByVal order As Char) As INDArray
			enforceExistsAndActive(arrayType)
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = notifyScopeBorrowed(arrayType)
				Return toDup.dup(order)
			End Using
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray dup(@NonNull T arrayType, @NonNull INDArray toDup)
		Public Overridable Function dup(ByVal arrayType As T, ByVal toDup As INDArray) As INDArray Implements WorkspaceMgr(Of T).dup
			Return dup(arrayType, toDup, toDup.ordering())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray castTo(@NonNull T arrayType, @NonNull DataType dataType, @NonNull INDArray toCast, boolean dupIfCorrectType)
		Public Overridable Function castTo(ByVal arrayType As T, ByVal dataType As DataType, ByVal toCast As INDArray, ByVal dupIfCorrectType As Boolean) As INDArray Implements WorkspaceMgr(Of T).castTo
			If toCast.dataType() = dataType Then
				If Not dupIfCorrectType Then
					'Check if we can avoid duping... if not in workspace, or already in correct workspace
					If Not toCast.isAttached() OrElse toCast.data().getParentWorkspace().getId().Equals(workspaceNames(arrayType)) Then
						Return toCast
					End If
				End If
				Return dup(arrayType, toCast)
			Else
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = notifyScopeBorrowed(arrayType)
					Return toCast.castTo(dataType)
				End Using
			End If
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private void validateConfig(@NonNull T arrayType)
		Private Sub validateConfig(ByVal arrayType As T)
			If scopeOutOfWs.Contains(arrayType) Then
				Return
			End If

			If Not configMap.ContainsKey(arrayType) Then
				Throw New ND4JWorkspaceException("No workspace configuration has been provided for arrayType: " & arrayType)
			End If
			If Not workspaceNames.ContainsKey(arrayType) Then
				Throw New ND4JWorkspaceException("No workspace name has been provided for arrayType: " & arrayType)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private void enforceExistsAndActive(@NonNull T arrayType)
		Private Sub enforceExistsAndActive(ByVal arrayType As T)
			validateConfig(arrayType)
			If scopeOutOfWs.Contains(arrayType) Then
				Return
			End If

			If Not Nd4j.WorkspaceManager.checkIfWorkspaceExistsAndActive(workspaceNames(arrayType)) Then
				Throw New ND4JWorkspaceException("Workspace """ & workspaceNames(arrayType) & """ for array type " & arrayType & " is not open")
			End If
		End Sub
	End Class

End Namespace