Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DummyWorkspace = org.nd4j.linalg.api.memory.abstracts.DummyWorkspace

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


	Public Class WorkspaceUtils

		Private Sub New()
		End Sub

		''' <summary>
		''' Assert that no workspaces are currently open
		''' </summary>
		''' <param name="msg"> Message to include in the exception, if required </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void assertNoWorkspacesOpen(String msg) throws ND4JWorkspaceException
		Public Shared Sub assertNoWorkspacesOpen(ByVal msg As String)
			assertNoWorkspacesOpen(msg, False)
		End Sub

		''' <summary>
		''' Assert that no workspaces are currently open
		''' </summary>
		''' <param name="msg"> Message to include in the exception, if required </param>
		''' <param name="allowScopedOut"> If true: don't fail if we have an open workspace but are currently scoped out </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void assertNoWorkspacesOpen(String msg, boolean allowScopedOut) throws ND4JWorkspaceException
		Public Shared Sub assertNoWorkspacesOpen(ByVal msg As String, ByVal allowScopedOut As Boolean)
			If Nd4j.WorkspaceManager.anyWorkspaceActiveForCurrentThread() Then

				Dim currWs As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
				If allowScopedOut AndAlso (currWs Is Nothing OrElse TypeOf currWs Is DummyWorkspace) Then
					Return 'Open WS but we've scoped out
				End If

				Dim l As IList(Of MemoryWorkspace) = Nd4j.WorkspaceManager.getAllWorkspacesForCurrentThread()
				Dim workspaces As IList(Of String) = New List(Of String)(l.Count)
				For Each ws As MemoryWorkspace In l
					If ws.ScopeActive Then
						workspaces.Add(ws.Id)
					End If
				Next ws
				Throw New ND4JWorkspaceException(msg & " - Open/active workspaces: " & workspaces)
			End If
		End Sub

		''' <summary>
		''' Assert that the specified workspace is open and active
		''' </summary>
		''' <param name="ws">       Name of the workspace to assert open and active </param>
		''' <param name="errorMsg"> Message to include in the exception, if required </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void assertOpenAndActive(@NonNull String ws, @NonNull String errorMsg) throws ND4JWorkspaceException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub assertOpenAndActive(ByVal ws As String, ByVal errorMsg As String)
			If Not Nd4j.WorkspaceManager.checkIfWorkspaceExistsAndActive(ws) Then
				Throw New ND4JWorkspaceException(errorMsg)
			End If
		End Sub

		''' <summary>
		''' Assert that the specified workspace is open, active, and is the current workspace
		''' </summary>
		''' <param name="ws">       Name of the workspace to assert open/active/current </param>
		''' <param name="errorMsg"> Message to include in the exception, if required </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void assertOpenActiveAndCurrent(@NonNull String ws, @NonNull String errorMsg) throws ND4JWorkspaceException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub assertOpenActiveAndCurrent(ByVal ws As String, ByVal errorMsg As String)
			If Not Nd4j.WorkspaceManager.checkIfWorkspaceExistsAndActive(ws) Then
				Throw New ND4JWorkspaceException(errorMsg & " - workspace is not open and active")
			End If
			Dim currWs As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			If currWs Is Nothing OrElse Not ws.Equals(currWs.Id) Then
				Throw New ND4JWorkspaceException(errorMsg & " - not the current workspace (current workspace: " & (If(currWs Is Nothing, Nothing, currWs.Id)))
			End If
		End Sub

		''' <summary>
		''' Assert that the specified array is valid, in terms of workspaces: i.e., if it is attached (and not in a circular
		''' workspace), assert that the workspace is open, and that the data is not from an old generation. </summary>
		''' <param name="array"> Array to check </param>
		''' <param name="msg">   Message (prefix) to include in the exception, if required. May be null </param>
		Public Shared Sub assertValidArray(ByVal array As INDArray, ByVal msg As String)
			If array Is Nothing OrElse Not array.Attached Then
				Return
			End If

			Dim ws As val = array.data().ParentWorkspace

			If ws.getWorkspaceType() <> MemoryWorkspace.Type.CIRCULAR Then

				If Not ws.isScopeActive() Then
					Throw New ND4JWorkspaceException((If(msg Is Nothing, "", msg & ": ")) & "Array uses leaked workspace pointer " & "from workspace " & ws.getId() & vbLf & "All open workspaces: " & allOpenWorkspaces())
				End If

				If ws.getGenerationId() <> array.data().GenerationId Then
					Throw New ND4JWorkspaceException((If(msg Is Nothing, "", msg & ": ")) & "Array outdated workspace pointer " & "from workspace " & ws.getId() & " (array generation " & array.data().GenerationId & ", current workspace generation " & ws.getGenerationId() & ")" & vbLf & "All open workspaces: " & allOpenWorkspaces())
				End If
			End If
		End Sub

		Private Shared Function allOpenWorkspaces() As IList(Of String)
			Dim l As IList(Of MemoryWorkspace) = Nd4j.WorkspaceManager.getAllWorkspacesForCurrentThread()
			Dim workspaces As IList(Of String) = New List(Of String)(l.Count)
			For Each ws As MemoryWorkspace In l
				If ws.ScopeActive Then
					workspaces.Add(ws.Id)
				End If
			Next ws
			Return workspaces
		End Function
	End Class

End Namespace