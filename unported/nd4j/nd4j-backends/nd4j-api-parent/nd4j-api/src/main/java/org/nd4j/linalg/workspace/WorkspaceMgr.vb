Imports NonNull = lombok.NonNull
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

	Public Interface WorkspaceMgr(Of T As [Enum](Of T))

		''' <summary>
		''' Set the workspace name for the specified array type
		''' </summary>
		''' <param name="arrayType"> Array type to set the workspace name for </param>
		''' <param name="wsName">    Workspace name to set </param>
		Sub setWorkspaceName(ByVal arrayType As T, ByVal wsName As String)

		''' <param name="arrayType"> Array type to get the workspace name for (if set) </param>
		''' <returns> The workspace name for the specified array type (or null, if none has been set) </returns>
		Function getWorkspaceName(ByVal arrayType As T) As String

		''' <summary>
		''' Seth the workspace name and configuration for the specified array type
		''' </summary>
		''' <param name="arrayType">     Array type </param>
		''' <param name="wsName">        Workspace name </param>
		''' <param name="configuration"> Workspace configuration </param>
		Sub setWorkspace(ByVal arrayType As T, ByVal wsName As String, ByVal configuration As WorkspaceConfiguration)

		''' <summary>
		''' Set the workspace configuration for the specified array type
		''' </summary>
		''' <param name="arrayType">     Type of array to set the configuration for </param>
		''' <param name="configuration"> Configuration for the specified array type </param>
		Sub setConfiguration(ByVal arrayType As T, ByVal configuration As WorkspaceConfiguration)

		''' <param name="arrayType"> Array type to get the workspace configuration for </param>
		''' <returns> Workspace configuration for the specified array type (or note, if no configuration has been set) </returns>
		Function getConfiguration(ByVal arrayType As T) As WorkspaceConfiguration

		''' <summary>
		''' Set arrays to be scoped out (not in any workspace) for the specified array type.
		''' This means that create, dup, leverage etc methods will return result arrays that are not attached to any workspace
		''' </summary>
		''' <param name="arrayType"> Array type to set scoped out for </param>
		WriteOnly Property ScopedOutFor As T

		''' <param name="arrayType"> Array type </param>
		''' <returns> True if the specified array type is set to be scoped out </returns>
		Function isScopedOut(ByVal arrayType As T) As Boolean

		''' <summary>
		''' Has the specified array type been configured in this workspace manager?
		''' </summary>
		''' <param name="arrayType"> Array type to check </param>
		''' <returns> True if the array type has been configured (either scoped out, or a workspace has been set for this
		'''  array type) </returns>
		Function hasConfiguration(ByVal arrayType As T) As Boolean

		''' <param name="arrayType"> Array type to enter the scope for </param>
		''' <returns> Workspace for the specified array type </returns>
		Function notifyScopeEntered(ByVal arrayType As T) As MemoryWorkspace

		''' <summary>
		''' Open/enter multiple workspaces. This is equivalent to nested opening of the specified workspaces
		''' </summary>
		''' <param name="arrayTypes"> Open the specified workspaces </param>
		''' <returns> Closeable for the specified workspaces </returns>
		Function notifyScopeEntered(ParamArray ByVal arrayTypes() As T) As WorkspacesCloseable

		''' <summary>
		''' Borrow the scope for the specified array type
		''' </summary>
		''' <param name="arrayType"> Array type to borrow the scope for </param>
		''' <returns> Workspace </returns>
		Function notifyScopeBorrowed(ByVal arrayType As T) As MemoryWorkspace

		''' <summary>
		''' Check if the workspace for the specified array type is open. If the array type is set to be scoped out,
		''' this will return true
		''' </summary>
		''' <param name="arrayType"> Array type </param>
		''' <returns> True if the workspace is open (or array type is set to scoped out) </returns>
		Function isWorkspaceOpen(ByVal arrayType As T) As Boolean

		''' <summary>
		''' Assert thath the workspace for the specified array type is open.
		''' For array types that are set to scoped out, this will be treated as a no-op </summary>
		''' <param name="arrayType"> Array type to check </param>
		''' <param name="msg">       May be null. If non-null: include this in the exception </param>
		''' <exception cref="ND4JWorkspaceException"> If the specified workspace is not open </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void assertOpen(T arrayType, String msg) throws ND4JWorkspaceException;
		Sub assertOpen(ByVal arrayType As T, ByVal msg As String)

		''' <summary>
		''' Assert thath the workspace for the specified array type is not open.
		''' For array types that are set to scoped out, this will be treated as a no-op </summary>
		''' <param name="arrayType"> Array type to check </param>
		''' <param name="msg">       May be null. If non-null: include this in the exception </param>
		''' <exception cref="ND4JWorkspaceException"> If the specified workspace is open </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void assertNotOpen(T arrayType, String msg) throws ND4JWorkspaceException;
		Sub assertNotOpen(ByVal arrayType As T, ByVal msg As String)

		''' <summary>
		''' Assert that the current workspace is the one for the specified array type.
		''' As per <seealso cref="isWorkspaceOpen(Enum)"/> scoped out array types are ignored here.
		''' </summary>
		''' <param name="arrayType"> Array type to check </param>
		''' <param name="msg">       May be null. Message to include in the exception </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void assertCurrentWorkspace(T arrayType, String msg) throws ND4JWorkspaceException;
		Sub assertCurrentWorkspace(ByVal arrayType As T, ByVal msg As String)

		''' <summary>
		''' Leverage the array to the specified array type's workspace (or detach if required).
		''' If the array is not attached (not defined in a workspace) - array is returned unmodified
		''' </summary>
		''' <param name="toWorkspace"> Array type's workspace to move the array to </param>
		''' <param name="array">       Array to leverage </param>
		''' <returns> Leveraged array (if leveraged, or original array otherwise) </returns>
		Function leverageTo(ByVal toWorkspace As T, ByVal array As INDArray) As INDArray

		''' <summary>
		''' Validate that the specified array type is actually in the workspace it's supposed to be in
		''' </summary>
		''' <param name="arrayType">           Array type of the array </param>
		''' <param name="array">               Array to check </param>
		''' <param name="migrateIfInvalid">    if true, and array is in the wrong WS: migrate the array and return. If false and in
		'''                            the wrong WS: exception </param>
		''' <param name="exceptionIfDetached"> If true: if the workspace is detached, but is expected to be in a workspace: should an
		'''                            exception be thrown? </param>
		''' <returns> The original array, or (if required, and if migrateIfInvalid == true) the migrated array </returns>
		''' <exception cref="ND4JWorkspaceException"> If the array is in the incorrect workspace (and migrateIfInvalid == false) </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: org.nd4j.linalg.api.ndarray.INDArray validateArrayLocation(T arrayType, org.nd4j.linalg.api.ndarray.INDArray array, boolean migrateIfInvalid, boolean exceptionIfDetached) throws ND4JWorkspaceException;
		Function validateArrayLocation(ByVal arrayType As T, ByVal array As INDArray, ByVal migrateIfInvalid As Boolean, ByVal exceptionIfDetached As Boolean) As INDArray

		''' <summary>
		''' Create an array in the specified array type's workspace (or detached if none is specified).
		''' Equivalent to <seealso cref="org.nd4j.linalg.factory.Nd4j.create(Integer...)"/>, other than the array location </summary>
		''' <param name="arrayType"> Array type </param>
		''' <param name="dataType">  Data type for the created array </param>
		''' <param name="shape">     Shape </param>
		''' <returns> Created arary </returns>
		Function create(ByVal arrayType As T, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray

		''' <summary>
		''' Create an array in the specified array type's workspace (or detached if none is specified).
		''' Equivalent to <seealso cref="org.nd4j.linalg.factory.Nd4j.create(Integer[],Char)"/>, other than the array location </summary>
		''' <param name="arrayType"> Array type </param>
		''' <param name="dataType">  Data type for the created array </param>
		''' <param name="shape">     Shape </param>
		''' <param name="ordering"> Order of the array </param>
		''' <returns> Created arary </returns>
		Function create(ByVal arrayType As T, ByVal dataType As DataType, ByVal shape() As Long, ByVal ordering As Char) As INDArray

		''' <summary>
		''' Create an uninitialized array in the specified array type's workspace (or detached if none is specified).
		''' Equivalent to <seealso cref="org.nd4j.linalg.factory.Nd4j.createUninitialized(Integer)"/> (int...)}, other than the array location </summary>
		''' <param name="arrayType"> Array type </param>
		''' <param name="dataType">  Data type of the created array </param>
		''' <param name="shape">     Shape </param>
		''' <returns> Created array </returns>
		Function createUninitialized(ByVal arrayType As T, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray

		''' <summary>
		''' Create an uninitialized array in the specified array type's workspace (or detached if none is specified).
		''' Equivalent to <seealso cref="org.nd4j.linalg.factory.Nd4j.createUninitialized(Integer[], Char)"/>}, other than the array location </summary>
		''' <param name="arrayType"> Array type </param>
		''' <param name="dataType">  Data type of the returned array </param>
		''' <param name="shape">     Shape </param>
		''' <param name="order"> Order of the array </param>
		''' <returns> Created array </returns>
		Function createUninitialized(ByVal arrayType As T, ByVal dataType As DataType, ByVal shape() As Long, ByVal order As Char) As INDArray

		''' <summary>
		''' Duplicate the array, where the array is put into the specified array type's workspace (if applicable) </summary>
		''' <param name="arrayType"> Array type for the result </param>
		''' <param name="toDup">     Array to duplicate </param>
		''' <returns> Duplicated array in the specified array type's workspace </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: org.nd4j.linalg.api.ndarray.INDArray dup(@NonNull T arrayType, @NonNull INDArray toDup);
		Function dup(ByVal arrayType As T, ByVal toDup As INDArray) As INDArray

		''' <summary>
		''' Duplicate the array, where the array is put into the specified array type's workspace (if applicable) </summary>
		''' <param name="arrayType"> Array type for the result </param>
		''' <param name="toDup">     Array to duplicate </param>
		''' <param name="order">     Order for the duplicated array </param>
		''' <returns> Duplicated array in the specified array type's workspace </returns>
		Function dup(ByVal arrayType As T, ByVal toDup As INDArray, ByVal order As Char) As INDArray

		''' <summary>
		''' Cast the specified array to the specified datatype.<br>
		''' If the array is already the correct type, the bahaviour depends on the 'dupIfCorrectType' argument.<br>
		''' dupIfCorrectType = false && toCast.dataType() == dataType: return input array as-is (unless workspace is wrong)<br>
		''' dupIfCorrectType = true && toCast.dataType() == dataType: duplicate the array into the specified workspace<br> </summary>
		''' <param name="arrayType">        Array type </param>
		''' <param name="dataType">         Data type </param>
		''' <param name="toCast">           Array to cast </param>
		''' <param name="dupIfCorrectType"> False: return array as-is if correct type and in the correct workspace. True: dup if already correct type </param>
		''' <returns> Cast (or duplicated) array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: org.nd4j.linalg.api.ndarray.INDArray castTo(@NonNull T arrayType, @NonNull DataType dataType, @NonNull INDArray toCast, boolean dupIfCorrectType);
		Function castTo(ByVal arrayType As T, ByVal dataType As DataType, ByVal toCast As INDArray, ByVal dupIfCorrectType As Boolean) As INDArray


	End Interface

End Namespace