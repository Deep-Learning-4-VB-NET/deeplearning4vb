Imports System.Collections.Generic
Imports System.Linq
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.workspace
Imports org.nd4j.linalg.workspace

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

Namespace org.deeplearning4j.nn.workspace


	Public Class LayerWorkspaceMgr
		Inherits BaseWorkspaceMgr(Of ArrayType)

		Public Shared CUDNN_WORKSPACE_KEY As String = "CUDNN_WORKSPACE"

		Private Shared NO_WS_IMMUTABLE As LayerWorkspaceMgr
		Shared Sub New()
			Dim all As ISet(Of ArrayType) = New HashSet(Of ArrayType)()
			Collections.addAll(all, System.Enum.GetValues(GetType(ArrayType)))
			NO_WS_IMMUTABLE = New LayerWorkspaceMgr(all, Enumerable.Empty(Of ArrayType, WorkspaceConfiguration)(), Enumerable.Empty(Of ArrayType, String)())
		End Sub

'JAVA TO VB CONVERTER NOTE: The field noLeverageOverride was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend noLeverageOverride_Conflict As ISet(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected Map<String,org.bytedeco.javacpp.Pointer> helperWorkspacePointers;
		Protected Friend helperWorkspacePointers As IDictionary(Of String, Pointer)

		Private Sub New()

		End Sub

		Public Sub New(ByVal scopeOutOfWs As ISet(Of ArrayType), ByVal configMap As IDictionary(Of ArrayType, WorkspaceConfiguration), ByVal workspaceNames As IDictionary(Of ArrayType, String))
			MyBase.New(scopeOutOfWs, configMap, workspaceNames)
			If configMap IsNot Nothing Then
				Preconditions.checkArgument(configMap.Keys.Equals(workspaceNames.Keys), "Keys for config may and workspace names must match")
			End If
		End Sub

		Public Overridable WriteOnly Property NoLeverageOverride As String
			Set(ByVal wsName As String)
				If noLeverageOverride_Conflict Is Nothing Then
					noLeverageOverride_Conflict = New HashSet(Of String)()
				End If
				noLeverageOverride_Conflict.Add(wsName)
			End Set
		End Property

		Public Overrides Function leverageTo(ByVal arrayType As ArrayType, ByVal array As INDArray) As INDArray
			If noLeverageOverride_Conflict IsNot Nothing AndAlso array.Attached AndAlso noLeverageOverride_Conflict.Contains(array.data().ParentWorkspace.Id) Then
				Return array
			End If
			Return MyBase.leverageTo(arrayType, array)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray validateArrayLocation(@NonNull ArrayType arrayType, @NonNull INDArray array, boolean migrateIfInvalid, boolean exceptionIfDetached)
		Public Overrides Function validateArrayLocation(ByVal arrayType As ArrayType, ByVal array As INDArray, ByVal migrateIfInvalid As Boolean, ByVal exceptionIfDetached As Boolean) As INDArray
			If noLeverageOverride_Conflict IsNot Nothing AndAlso array.isAttached() AndAlso noLeverageOverride_Conflict.Contains(array.data().getParentWorkspace().getId()) Then
				Return array 'OK - leverage override
			End If
			Return MyBase.validateArrayLocation(arrayType, array, migrateIfInvalid, exceptionIfDetached)
		End Function

		''' <summary>
		''' Get the pointer to the helper memory. Usually used for CUDNN workspace memory sharing.
		''' NOTE: Don't use this method unless you are fully aware of how it is used to manage CuDNN memory!
		''' Will (by design) throw a NPE if the underlying map (set from MultiLayerNetwork or ComputationGraph) is not set.
		''' </summary>
		''' <param name="key"> Key for the helper workspace pointer </param>
		''' @param <T> Pointer type </param>
		''' <returns> Pointer for that key, or null if none exists </returns>
		Public Overridable Function getHelperWorkspace(Of T As Pointer)(ByVal key As String) As T
			Return If(helperWorkspacePointers Is Nothing, Nothing, CType(helperWorkspacePointers(key), T))
		End Function

		''' <summary>
		''' Set the pointer to the helper memory. Usually used for CuDNN workspace memory sharing.
		''' NOTE: Don't use this method unless you are fully aware of how it is used to manage CuDNN memory!
		''' Will (by design) throw a NPE if the underlying map (set from MultiLayerNetwork or ComputationGraph) is not set.
		''' </summary>
		''' <param name="key">   Key for the helper workspace pointer </param>
		''' <param name="value"> Pointer </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setHelperWorkspace(@NonNull String key, org.bytedeco.javacpp.Pointer value)
		Public Overridable Sub setHelperWorkspace(ByVal key As String, ByVal value As Pointer)
			If helperWorkspacePointers Is Nothing Then
				helperWorkspacePointers = New Dictionary(Of String, Pointer)()
			End If
			helperWorkspacePointers(key) = value
		End Sub

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		''' <param name="helperWorkspacePointers"> Helper pointers - see <seealso cref="getHelperWorkspace(String)"/> for details </param>
		''' <returns> Workspace manager </returns>
		Public Shared Function noWorkspaces(ByVal helperWorkspacePointers As IDictionary(Of String, Pointer)) As LayerWorkspaceMgr
			Dim wsm As LayerWorkspaceMgr = noWorkspaces()
			wsm.setHelperWorkspacePointers(helperWorkspacePointers)
			Return wsm
		End Function

		Public Shared Function noWorkspaces() As LayerWorkspaceMgr
			Return builder().defaultNoWorkspace().build()
		End Function

		Public Shared Function noWorkspacesImmutable() As LayerWorkspaceMgr
			Return NO_WS_IMMUTABLE
		End Function

		Public Class Builder

			Friend mgr As LayerWorkspaceMgr

			Public Sub New()
				mgr = New LayerWorkspaceMgr()
			End Sub

			''' <summary>
			''' Set the default to be scoped out for all array types.
			''' NOTE: Will not override the configuration for any array types that have already been configured </summary>
			''' <returns> Builder </returns>
			Public Overridable Function defaultNoWorkspace() As Builder
				For Each t As ArrayType In System.Enum.GetValues(GetType(ArrayType))
					If Not mgr.configMap.ContainsKey(t) Then
						mgr.ScopedOutFor = t
					End If
				Next t
				Return Me
			End Function

			''' <summary>
			''' Specify that no workspace should be used for array of the specified type - i.e., these arrays should all
			''' be scoped out.
			''' </summary>
			''' <param name="type"> Array type to set scoped out for </param>
			''' <returns> Builder </returns>
			Public Overridable Function noWorkspaceFor(ByVal type As ArrayType) As Builder
				mgr.ScopedOutFor = type
				Return Me
			End Function

			''' <summary>
			''' Set the default workspace for all array types to the specified workspace name/configuration
			''' NOTE: This will NOT override any settings previously set.
			''' </summary>
			''' <param name="workspaceName"> Name of the workspace to use for all (not set) arrray types </param>
			''' <param name="configuration"> Configuration to use for all (not set) arrray types </param>
			''' <returns> Builder </returns>
			Public Overridable Function defaultWorkspace(ByVal workspaceName As String, ByVal configuration As WorkspaceConfiguration) As Builder
				For Each t As ArrayType In System.Enum.GetValues(GetType(ArrayType))
					If Not mgr.configMap.ContainsKey(t) AndAlso Not mgr.isScopedOut(t) Then
						[with](t, workspaceName, configuration)
					End If
				Next t
				Return Me
			End Function

			''' <summary>
			''' Configure the workspace (name, configuration) for the specified array type
			''' </summary>
			''' <param name="type">          Array type </param>
			''' <param name="workspaceName"> Workspace name for the specified array type </param>
			''' <param name="configuration"> Configuration for the specified array type </param>
			''' <returns> Builder </returns>
			Public Overridable Function [with](ByVal type As ArrayType, ByVal workspaceName As String, ByVal configuration As WorkspaceConfiguration) As Builder
				mgr.setConfiguration(type, configuration)
				mgr.setWorkspaceName(type, workspaceName)
				Return Me
			End Function

			Public Overridable Function build() As LayerWorkspaceMgr
				Return mgr
			End Function

		End Class

	End Class

End Namespace