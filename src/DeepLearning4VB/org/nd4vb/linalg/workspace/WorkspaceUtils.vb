Imports System
Imports System.Collections.Generic
Imports DummyWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.DummyWorkspace
Imports MemoryManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryManager
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports MemoryWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspaceManager
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
    ''' Workspace utility methods, mainly the out-of-scope checks (Java <c>WorkspaceUtils</c>).
    ''' </summary>
    ''' <remarks>
    ''' The public methods use <c>Nd4vb.WorkspaceManager</c> and <c>Nd4vb.MemoryManager</c> (Java: <c>Nd4j</c>); the Friend
    ''' overloads take the managers explicitly. Java's list of open workspaces renders as <c>[a, b]</c>.
    ''' </remarks>
    Public Class WorkspaceUtils

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Assert that no workspaces are currently open.
        ''' </summary>
        ''' <param name="msg"> Message to include in the exception, if required </param>
        ''' <exception cref="ND4VBWorkspaceException">A workspace is open.</exception>
        Public Shared Sub assertNoWorkspacesOpen(ByVal msg As String)
            assertNoWorkspacesOpen(msg, False)
        End Sub

        ''' <summary>
        ''' Assert that no workspaces are currently open.
        ''' </summary>
        ''' <param name="msg"> Message to include in the exception, if required </param>
        ''' <param name="allowScopedOut"> If true: don't fail if we have an open workspace but are currently scoped out </param>
        Public Shared Sub assertNoWorkspacesOpen(ByVal msg As String, ByVal allowScopedOut As Boolean)
            assertNoWorkspacesOpen(msg, allowScopedOut, Nd4vbT.WorkspaceManager, Nd4vbT.MemoryManager)
        End Sub

        Friend Shared Sub assertNoWorkspacesOpen(ByVal msg As String, ByVal allowScopedOut As Boolean, ByVal workspaceManager As MemoryWorkspaceManager, ByVal memoryManager As MemoryManager)
            If workspaceManager.anyWorkspaceActiveForCurrentThread() Then
                Dim currWs As MemoryWorkspace = memoryManager.CurrentWorkspace
                If allowScopedOut AndAlso (currWs Is Nothing OrElse TypeOf currWs Is DummyWorkspace) Then
                    Return 'Open WS but we've scoped out
                End If

                Throw New ND4VBWorkspaceException(msg & " - Open/active workspaces: " & render(allOpenWorkspaces(workspaceManager)))
            End If
        End Sub

        ''' <summary>
        ''' Assert that the specified workspace is open and active.
        ''' </summary>
        ''' <param name="ws">       Name of the workspace to assert open and active </param>
        ''' <param name="errorMsg"> Message to include in the exception, if required </param>
        Public Shared Sub assertOpenAndActive(ByVal ws As String, ByVal errorMsg As String)
            assertOpenAndActive(ws, errorMsg, Nd4vbT.WorkspaceManager)
        End Sub

        Friend Shared Sub assertOpenAndActive(ByVal ws As String, ByVal errorMsg As String, ByVal workspaceManager As MemoryWorkspaceManager)
            If ws Is Nothing Then
                Throw New ArgumentNullException(NameOf(ws))
            End If
            If Not workspaceManager.checkIfWorkspaceExistsAndActive(ws) Then
                Throw New ND4VBWorkspaceException(errorMsg)
            End If
        End Sub

        ''' <summary>
        ''' Assert that the specified workspace is open, active, and is the current workspace.
        ''' </summary>
        ''' <param name="ws">       Name of the workspace to assert open/active/current </param>
        ''' <param name="errorMsg"> Message to include in the exception, if required </param>
        Public Shared Sub assertOpenActiveAndCurrent(ByVal ws As String, ByVal errorMsg As String)
            assertOpenActiveAndCurrent(ws, errorMsg, Nd4vbT.WorkspaceManager, Nd4vbT.MemoryManager)
        End Sub

        Friend Shared Sub assertOpenActiveAndCurrent(ByVal ws As String, ByVal errorMsg As String, ByVal workspaceManager As MemoryWorkspaceManager, ByVal memoryManager As MemoryManager)
            If ws Is Nothing Then
                Throw New ArgumentNullException(NameOf(ws))
            End If
            If Not workspaceManager.checkIfWorkspaceExistsAndActive(ws) Then
                Throw New ND4VBWorkspaceException(errorMsg & " - workspace is not open and active")
            End If
            Dim currWs As MemoryWorkspace = memoryManager.CurrentWorkspace
            If currWs Is Nothing OrElse Not ws.Equals(currWs.Id) Then
                Throw New ND4VBWorkspaceException(errorMsg & " - not the current workspace (current workspace: " & (If(currWs Is Nothing, "null", currWs.Id)))
            End If
        End Sub

        ''' <summary>
        ''' Assert that the specified array is valid, in terms of workspaces: i.e., if it is attached (and not in a circular
        ''' workspace), assert that the workspace is open, and that the data is not from an old generation.
        ''' </summary>
        ''' <param name="array"> Array to check </param>
        ''' <param name="msg">   Message (prefix) to include in the exception, if required. May be null </param>
        Public Shared Sub assertValidArray(ByVal array As INDArray, ByVal msg As String)
            assertValidArray(array, msg, Nothing)
        End Sub

        Friend Shared Sub assertValidArray(ByVal array As INDArray, ByVal msg As String, ByVal workspaceManager As MemoryWorkspaceManager)
            If array Is Nothing OrElse Not array.Attached Then
                Return
            End If

            Dim ws As MemoryWorkspace = array.data().ParentWorkspace

            If ws.WorkspaceType <> MemoryWorkspace.Type.CIRCULAR Then

                If Not ws.ScopeActive Then
                    Throw New ND4VBWorkspaceException((If(msg Is Nothing, "", msg & ": ")) & "Array uses leaked workspace pointer " & "from workspace " & ws.Id & vbLf & "All open workspaces: " & render(allOpenWorkspaces(If(workspaceManager, Nd4vbT.WorkspaceManager))))
                End If

                If ws.GenerationId <> array.data().GenerationId Then
                    Throw New ND4VBWorkspaceException((If(msg Is Nothing, "", msg & ": ")) & "Array outdated workspace pointer " & "from workspace " & ws.Id & " (array generation " & array.data().GenerationId & ", current workspace generation " & ws.GenerationId & ")" & vbLf & "All open workspaces: " & render(allOpenWorkspaces(If(workspaceManager, Nd4vbT.WorkspaceManager))))
                End If
            End If
        End Sub

        Private Shared Function allOpenWorkspaces(ByVal workspaceManager As MemoryWorkspaceManager) As IList(Of String)
            Dim l As IList(Of MemoryWorkspace) = workspaceManager.AllWorkspacesForCurrentThread
            Dim workspaces As IList(Of String) = New List(Of String)(l.Count)
            For Each ws As MemoryWorkspace In l
                If ws.ScopeActive Then
                    workspaces.Add(ws.Id)
                End If
            Next ws
            Return workspaces
        End Function

        Private Shared Function render(ByVal names As IList(Of String)) As String
            Return "[" & String.Join(", ", names) & "]"
        End Function

    End Class

End Namespace
