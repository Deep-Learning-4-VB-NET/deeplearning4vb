Imports AbstractMemoryMgr = org.nd4j.autodiff.samediff.internal.memory.AbstractMemoryMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
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

Namespace org.deeplearning4j.nn.layers.samediff

	Public Class DL4JSameDiffMemoryMgr
		Inherits AbstractMemoryMgr

		Private ReadOnly workingMemoryWs As String
		Private ReadOnly outputWs As String
		Private ReadOnly confWorking As WorkspaceConfiguration
		Private ReadOnly confOutput As WorkspaceConfiguration

		'Note: if the working memory or output workspace names are null -> detached memory
		Public Sub New(ByVal workingMemoryWs As String, ByVal outputWs As String, ByVal confWorking As WorkspaceConfiguration, ByVal confOutput As WorkspaceConfiguration)
			Me.workingMemoryWs = workingMemoryWs
			Me.outputWs = outputWs
			Me.confWorking = confWorking
			Me.confOutput = confOutput
		End Sub


		Public Overrides Function allocate(ByVal detached As Boolean, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray
			Dim wsName As String = If(detached, outputWs, workingMemoryWs)
			Dim wsConf As WorkspaceConfiguration = If(detached, confOutput, confWorking)

			If wsName Is Nothing Then
				'Scoped out
				Dim ret As INDArray = Nd4j.createUninitializedDetached(dataType, shape)
				Preconditions.checkState(Not ret.Attached, "Returned array should be detached")
				Return ret
			Else
				Dim ws As MemoryWorkspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(wsConf, wsName)
				Using mw As org.nd4j.linalg.api.memory.MemoryWorkspace = ws.notifyScopeBorrowed()
					Return Nd4j.createUninitialized(dataType, shape)
				End Using
			End If
		End Function

		Public Overrides Function allocate(ByVal detached As Boolean, ByVal descriptor As LongShapeDescriptor) As INDArray
			If descriptor.Empty Then
				Dim ret As INDArray = Nd4j.create(descriptor)
				If detached Then
					ret = ret.detach()
				End If

				Return ret
			End If

			Return allocate(detached, descriptor.dataType(), descriptor.getShape())
		End Function

		Public Overrides Sub release(ByVal array As INDArray)
			'No-op - DL4J workspaces handles this
		End Sub

		Public Overrides Sub Dispose()
			'No-op - DL4J workspaces handles this
		End Sub
	End Class

End Namespace