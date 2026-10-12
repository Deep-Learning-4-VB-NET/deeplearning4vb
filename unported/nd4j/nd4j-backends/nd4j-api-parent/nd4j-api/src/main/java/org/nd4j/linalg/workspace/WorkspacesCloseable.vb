Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace

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
'ORIGINAL LINE: @Data public class WorkspacesCloseable implements AutoCloseable
	Public Class WorkspacesCloseable
		Implements AutoCloseable

		Private workspaces() As MemoryWorkspace

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public WorkspacesCloseable(@NonNull MemoryWorkspace... workspaces)
		Public Sub New(ParamArray ByVal workspaces() As MemoryWorkspace)
			Me.workspaces = workspaces
		End Sub

		Public Overrides Sub close()
			For i As Integer = workspaces.Length-1 To 0 Step -1
				workspaces(i).close()
			Next i
		End Sub
	End Class

End Namespace