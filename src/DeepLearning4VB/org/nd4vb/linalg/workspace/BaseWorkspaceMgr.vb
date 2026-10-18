Imports System
Imports System.Collections.Generic
Imports DeepLearning4VB.org.slf4j
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports MemoryManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryManager
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports MemoryWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspaceManager
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.workspace

    ''' <summary>
    ''' A base implementation of <see cref="WorkspaceMgr(Of T)"/> with the workspace names, configurations and scoped-out
    ''' array types kept in maps (Java <c>BaseWorkspaceMgr</c>).
    ''' </summary>
    ''' <remarks>
    ''' Java constrains <c>T</c> to an enum; VB cannot, so <c>T</c> is a <c>Structure</c> and should be an enum. The workspace
    ''' and memory managers come from <see cref="currentWorkspaceManager"/> and <see cref="currentMemoryManager"/>, which
    ''' return <c>Nd4vb.WorkspaceManager</c> and <c>Nd4vb.MemoryManager</c> (Java: <c>Nd4j</c>) and can be overridden.
    ''' <see cref="getConfiguration"/> and <see cref="getWorkspaceName"/> return Nothing for unknown array types, as Java's
    ''' map lookups did.
    ''' </remarks>
    Public MustInherit Class BaseWorkspaceMgr(Of T As Structure)
        Implements WorkspaceMgr(Of T)

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BaseWorkspaceMgr(Of T)))

        Private Const DISABLE_LEVERAGE As Boolean = False 'Mainly for debugging/optimization purposes

        Protected Friend ReadOnly scopeOutOfWs As ISet(Of T)
        Protected Friend ReadOnly configMap As IDictionary(Of T, WorkspaceConfiguration)
        Protected Friend ReadOnly workspaceNames As IDictionary(Of T, String)

        ''' <summary>A manager over the given sets and maps.</summary>
        Protected Sub New(ByVal scopeOutOfWs As ISet(Of T), ByVal configMap As IDictionary(Of T, WorkspaceConfiguration), ByVal workspaceNames As IDictionary(Of T, String))
            Me.scopeOutOfWs = scopeOutOfWs
            Me.configMap = configMap
            Me.workspaceNames = workspaceNames
        End Sub

        ''' <summary>A manager with empty sets and maps.</summary>
        Protected Sub New()
            scopeOutOfWs = New HashSet(Of T)()
            configMap = New Dictionary(Of T, WorkspaceConfiguration)()
            workspaceNames = New Dictionary(Of T, String)()
        End Sub

        ''' <summary>The workspace manager used (Java: <c>Nd4j.getWorkspaceManager()</c>).</summary>
        Protected Overridable Function currentWorkspaceManager() As MemoryWorkspaceManager
            Return Nd4vbT.WorkspaceManager
        End Function

        ''' <summary>The memory manager used (Java: <c>Nd4j.getMemoryManager()</c>).</summary>
        Protected Overridable Function currentMemoryManager() As MemoryManager
            Return Nd4vbT.MemoryManager
        End Function

        ''' <summary>Set the workspace configuration for the specified array type.</summary>
        Public Overridable Sub setConfiguration(ByVal arrayType As T, ByVal configuration As WorkspaceConfiguration) Implements WorkspaceMgr(Of T).setConfiguration
            configMap(arrayType) = configuration
        End Sub

        ''' <summary>The workspace configuration for the specified array type, or Nothing.</summary>
        Public Overridable Function getConfiguration(ByVal arrayType As T) As WorkspaceConfiguration Implements WorkspaceMgr(Of T).getConfiguration
            Dim configuration As WorkspaceConfiguration = Nothing
            configMap.TryGetValue(arrayType, configuration)
            Return configuration
        End Function

        ''' <summary>Scopes the array type out of workspaces (and forgets its configuration and workspace name).</summary>
        Public Overridable WriteOnly Property ScopedOutFor As T Implements WorkspaceMgr(Of T).ScopedOutFor
            Set(ByVal arrayType As T)
                scopeOutOfWs.Add(arrayType)
                configMap.Remove(arrayType)
                workspaceNames.Remove(arrayType)
            End Set
        End Property

        ''' <summary>Java <c>setScopedOutFor</c>.</summary>
        Public Overridable Sub setScopedOutFor(ByVal arrayType As T)
            ScopedOutFor = arrayType
        End Sub

        ''' <summary>True if the specified array type is set to be scoped out.</summary>
        Public Overridable Function isScopedOut(ByVal arrayType As T) As Boolean Implements WorkspaceMgr(Of T).isScopedOut
            Return scopeOutOfWs.Contains(arrayType)
        End Function

        ''' <summary>True if the array type is scoped out or has a workspace name.</summary>
        Public Overridable Function hasConfiguration(ByVal arrayType As T) As Boolean Implements WorkspaceMgr(Of T).hasConfiguration
            Return scopeOutOfWs.Contains(arrayType) OrElse workspaceNames.ContainsKey(arrayType)
        End Function

        ''' <summary>Enters the workspace of the array type (or a scope out of workspaces when it is scoped out).</summary>
        Public Overridable Function notifyScopeEntered(ByVal arrayType As T) As MemoryWorkspace Implements WorkspaceMgr(Of T).notifyScopeEntered
            validateConfig(arrayType)

            If isScopedOut(arrayType) Then
                Return currentWorkspaceManager().scopeOutOfWorkspaces()
            Else
                Dim ws As MemoryWorkspace = currentWorkspaceManager().getWorkspaceForCurrentThread(getConfiguration(arrayType), getWorkspaceName(arrayType))
                Return ws.notifyScopeEntered()
            End If
        End Function

        ''' <summary>Enters the workspaces of several array types; closing the result closes them in reverse order.</summary>
        Public Overridable Function notifyScopeEntered(ParamArray ByVal arrayTypes() As T) As WorkspacesCloseable Implements WorkspaceMgr(Of T).notifyScopeEntered
            Dim ws(arrayTypes.Length - 1) As MemoryWorkspace
            For i As Integer = 0 To arrayTypes.Length - 1
                ws(i) = notifyScopeEntered(arrayTypes(i))
            Next i
            Return New WorkspacesCloseable(ws)
        End Function

        ''' <summary>Borrows the (open) workspace of the array type (or scopes out when it is scoped out).</summary>
        Public Overridable Function notifyScopeBorrowed(ByVal arrayType As T) As MemoryWorkspace Implements WorkspaceMgr(Of T).notifyScopeBorrowed
            validateConfig(arrayType)
            enforceExistsAndActive(arrayType)

            If scopeOutOfWs.Contains(arrayType) Then
                Return currentWorkspaceManager().scopeOutOfWorkspaces()
            Else
                Dim ws As MemoryWorkspace = currentWorkspaceManager().getWorkspaceForCurrentThread(getConfiguration(arrayType), getWorkspaceName(arrayType))
                Return ws.notifyScopeBorrowed()
            End If
        End Function

        ''' <summary>Set the workspace name for the specified array type.</summary>
        Public Overridable Sub setWorkspaceName(ByVal arrayType As T, ByVal name As String) Implements WorkspaceMgr(Of T).setWorkspaceName
            workspaceNames(arrayType) = name
        End Sub

        ''' <summary>The workspace name of the array type, or Nothing.</summary>
        Public Overridable Function getWorkspaceName(ByVal arrayType As T) As String Implements WorkspaceMgr(Of T).getWorkspaceName
            Dim name As String = Nothing
            workspaceNames.TryGetValue(arrayType, name)
            Return name
        End Function

        ''' <summary>Sets the workspace name and configuration of the array type (no longer scoped out).</summary>
        Public Overridable Sub setWorkspace(ByVal forEnum As T, ByVal wsName As String, ByVal configuration As WorkspaceConfiguration) Implements WorkspaceMgr(Of T).setWorkspace
            If scopeOutOfWs.Contains(forEnum) Then
                scopeOutOfWs.Remove(forEnum)
            End If
            setWorkspaceName(forEnum, wsName)
            setConfiguration(forEnum, configuration)
        End Sub

        ''' <summary>True if the workspace of the array type is open (always True when it is scoped out).</summary>
        Public Overridable Function isWorkspaceOpen(ByVal arrayType As T) As Boolean Implements WorkspaceMgr(Of T).isWorkspaceOpen
            validateConfig(arrayType)
            If Not scopeOutOfWs.Contains(arrayType) Then
                Return currentWorkspaceManager().checkIfWorkspaceExistsAndActive(getWorkspaceName(arrayType))
            End If
            Return True
        End Function

        ''' <summary>Throws <see cref="ND4VBWorkspaceException"/> unless the workspace of the array type is open.</summary>
        Public Overridable Sub assertOpen(ByVal arrayType As T, ByVal msg As String) Implements WorkspaceMgr(Of T).assertOpen
            If Not scopeOutOfWs.Contains(arrayType) AndAlso Not isWorkspaceOpen(arrayType) Then
                Throw New ND4VBWorkspaceException("Assertion failed: expected workspace for array type " & arrayType.ToString() & " to be open: " & msg)
            End If
        End Sub

        ''' <summary>Throws <see cref="ND4VBWorkspaceException"/> if the workspace of the array type is open.</summary>
        Public Overridable Sub assertNotOpen(ByVal arrayType As T, ByVal msg As String) Implements WorkspaceMgr(Of T).assertNotOpen
            If Not scopeOutOfWs.Contains(arrayType) AndAlso isWorkspaceOpen(arrayType) Then
                Throw New ND4VBWorkspaceException("Assertion failed: expected workspace for array type " & arrayType.ToString() & " to not be open: " & msg)
            End If
        End Sub

        ''' <summary>Throws <see cref="ND4VBWorkspaceException"/> unless the workspace of the array type is the current workspace.</summary>
        Public Overridable Sub assertCurrentWorkspace(ByVal arrayType As T, ByVal msg As String) Implements WorkspaceMgr(Of T).assertCurrentWorkspace
            validateConfig(arrayType)
            Dim curr As MemoryWorkspace = currentMemoryManager().CurrentWorkspace
            If Not scopeOutOfWs.Contains(arrayType) AndAlso (curr Is Nothing OrElse Not getWorkspaceName(arrayType).Equals(curr.Id)) Then
                Throw New ND4VBWorkspaceException("Assertion failed: expected current workspace to be """ & getWorkspaceName(arrayType) & """ (for array type " & arrayType.ToString() & ") - actual current workspace is " & (If(curr Is Nothing, "null", curr.Id)) & (If(msg Is Nothing, "", ": " & msg)))
            End If
        End Sub

        ''' <summary>
        ''' Moves an attached array to the workspace of the array type (<c>leverageTo</c>), or detaches it when the type is
        ''' scoped out; arrays outside workspaces are returned as they are.
        ''' </summary>
        Public Overridable Function leverageTo(ByVal arrayType As T, ByVal array As INDArray) As INDArray Implements WorkspaceMgr(Of T).leverageTo
            If array Is Nothing OrElse Not array.Attached Then
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
                If array.Attached Then
                    If Not array.data().ParentWorkspace.Id.Equals(getWorkspaceName(arrayType)) Then
                        Throw New InvalidOperationException("Array of type " & arrayType.ToString() & " is leveraged from " & array.data().ParentWorkspace.Id & " to " & getWorkspaceName(arrayType) & " but WorkspaceMgn.leverageTo() is currently disabled")
                    End If
                End If
                Return array
            End If
        End Function

        ''' <summary>
        ''' Checks that the array is in the workspace of its type (detached for scoped-out types); migrates it when
        ''' <paramref name="migrateIfInvalid"/>, otherwise throws <see cref="ND4VBWorkspaceException"/>.
        ''' </summary>
        Public Overridable Function validateArrayLocation(ByVal arrayType As T, ByVal array As INDArray, ByVal migrateIfInvalid As Boolean, ByVal exceptionIfDetached As Boolean) As INDArray Implements WorkspaceMgr(Of T).validateArrayLocation
            validateConfig(arrayType)

            If scopeOutOfWs.Contains(arrayType) Then
                'Array is supposed to be detached (no workspace)
                Dim ok As Boolean = Not array.Attached
                If Not ok Then
                    If migrateIfInvalid Then
                        log.trace("Migrating array of type " & arrayType.ToString() & " to workspace " & getWorkspaceName(arrayType))
                        Return leverageTo(arrayType, array)
                    Else
                        Throw New ND4VBWorkspaceException("Array workspace validation failed: Array of type " & arrayType.ToString() & " should be detached (no workspace) but is in workspace: " & array.data().ParentWorkspace.Id)
                    End If
                Else
                    'Detached array, as expected
                    Return array
                End If
            End If

            'At this point: we expect the array to be in a workspace
            Dim wsNameExpected As String = getWorkspaceName(arrayType)
            If Not array.Attached Then
                If exceptionIfDetached Then
                    Throw New ND4VBWorkspaceException("Array workspace validation failed: Array of type " & arrayType.ToString() & " should be in workspace """ & wsNameExpected & """ but is detached")
                Else
                    Return array
                End If
            End If

            Dim wsNameAct As String = array.data().ParentWorkspace.Id
            If Not wsNameExpected.Equals(wsNameAct) Then
                If migrateIfInvalid Then
                    Return leverageTo(arrayType, array)
                Else
                    Throw New ND4VBWorkspaceException("Array workspace validation failed: Array of type " & arrayType.ToString() & " should be in workspace """ & wsNameExpected & """ but is in workspace """ & wsNameAct & """")
                End If
            End If

            'OK - return as-is
            Return array
        End Function

        ''' <summary>Creates an array in the (open) workspace of the array type, in the default order.</summary>
        Public Overridable Function create(ByVal arrayType As T, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray Implements WorkspaceMgr(Of T).create
            enforceExistsAndActive(arrayType)
            Return create(arrayType, dataType, shape, Nd4vbT.order())
        End Function

        ''' <summary>Creates an array in the (open) workspace of the array type.</summary>
        Public Overridable Function create(ByVal arrayType As T, ByVal dataType As DataType, ByVal shape() As Long, ByVal order As Char) As INDArray Implements WorkspaceMgr(Of T).create
            enforceExistsAndActive(arrayType)
            Using ws As MemoryWorkspace = notifyScopeBorrowed(arrayType)
                Return Nd4vbT.create(dataType, shape, order)
            End Using
        End Function

        ''' <summary>Creates an uninitialized array in the workspace of the array type, in the default order.</summary>
        Public Overridable Function createUninitialized(ByVal arrayType As T, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray Implements WorkspaceMgr(Of T).createUninitialized
            Return createUninitialized(arrayType, dataType, shape, Nd4vbT.order())
        End Function

        ''' <summary>Creates an uninitialized array in the (open) workspace of the array type.</summary>
        Public Overridable Function createUninitialized(ByVal arrayType As T, ByVal dataType As DataType, ByVal shape() As Long, ByVal order As Char) As INDArray Implements WorkspaceMgr(Of T).createUninitialized
            enforceExistsAndActive(arrayType)
            Using ws As MemoryWorkspace = notifyScopeBorrowed(arrayType)
                Return Nd4vbT.createUninitialized(dataType, shape, order)
            End Using
        End Function

        ''' <summary>Duplicates an array into the (open) workspace of the array type, in the given order.</summary>
        Public Overridable Function dup(ByVal arrayType As T, ByVal toDup As INDArray, ByVal order As Char) As INDArray Implements WorkspaceMgr(Of T).dup
            enforceExistsAndActive(arrayType)
            Using ws As MemoryWorkspace = notifyScopeBorrowed(arrayType)
                Return toDup.dup(order)
            End Using
        End Function

        ''' <summary>Duplicates an array into the (open) workspace of the array type, in its own order.</summary>
        Public Overridable Function dup(ByVal arrayType As T, ByVal toDup As INDArray) As INDArray Implements WorkspaceMgr(Of T).dup
            Return dup(arrayType, toDup, toDup.ordering())
        End Function

        ''' <summary>
        ''' Casts an array into the workspace of the array type; an array of the right type is returned as it is unless
        ''' <paramref name="dupIfCorrectType"/> or it is in another workspace.
        ''' </summary>
        Public Overridable Function castTo(ByVal arrayType As T, ByVal dataType As DataType, ByVal toCast As INDArray, ByVal dupIfCorrectType As Boolean) As INDArray Implements WorkspaceMgr(Of T).castTo
            If toCast.dataType() = dataType Then
                If Not dupIfCorrectType Then
                    'Check if we can avoid duping... if not in workspace, or already in correct workspace
                    If Not toCast.Attached OrElse toCast.data().ParentWorkspace.Id.Equals(getWorkspaceName(arrayType)) Then
                        Return toCast
                    End If
                End If
                Return dup(arrayType, toCast)
            Else
                Using ws As MemoryWorkspace = notifyScopeBorrowed(arrayType)
                    Return toCast.castTo(dataType)
                End Using
            End If
        End Function

        Private Sub validateConfig(ByVal arrayType As T)
            If scopeOutOfWs.Contains(arrayType) Then
                Return
            End If

            If Not configMap.ContainsKey(arrayType) Then
                Throw New ND4VBWorkspaceException("No workspace configuration has been provided for arrayType: " & arrayType.ToString())
            End If
            If Not workspaceNames.ContainsKey(arrayType) Then
                Throw New ND4VBWorkspaceException("No workspace name has been provided for arrayType: " & arrayType.ToString())
            End If
        End Sub

        Private Sub enforceExistsAndActive(ByVal arrayType As T)
            validateConfig(arrayType)
            If scopeOutOfWs.Contains(arrayType) Then
                Return
            End If

            If Not currentWorkspaceManager().checkIfWorkspaceExistsAndActive(workspaceNames(arrayType)) Then
                Throw New ND4VBWorkspaceException("Workspace """ & workspaceNames(arrayType) & """ for array type " & arrayType.ToString() & " is not open")
            End If
        End Sub

    End Class

End Namespace
