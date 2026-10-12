Imports System
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseGraphVertex = org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.nn.graph.vertex.impl

	<Serializable>
	Public Class ScaleVertex
		Inherits BaseGraphVertex

		Private scaleFactor As Double

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal scaleFactor As Double, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, scaleFactor, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal scaleFactor As Double, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.scaleFactor = scaleFactor
		End Sub

		Public Overrides Function hasLayer() As Boolean
			Return False
		End Function

		Public Overrides ReadOnly Property Layer As Layer
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function doForward(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If Not canDoForward() Then
				Throw New System.InvalidOperationException("Cannot do forward pass: inputs not set (ScaleVertex " & vertexName_Conflict & " idx " & vertexIndex_Conflict & ")")
			End If

			If inputs.Length > 1 Then
				Throw New System.ArgumentException("ScaleVertex (name " & vertexName_Conflict & " idx " & vertexIndex_Conflict & ") only supports 1 input.")
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
				Return inputs(0).mul(scaleFactor)
			End Using
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: errors not set (ScaleVertex " & vertexName_Conflict & " idx " & vertexIndex_Conflict & ")")
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
				Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {epsilon_Conflict.mul(scaleFactor)})
			End Using
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				If backpropGradientsViewArray IsNot Nothing Then
					Throw New Exception("Vertex does not have gradients; gradients view array cannot be set here (ScaleVertex " & vertexName_Conflict & " idx " & vertexIndex_Conflict & ")")
				End If
			End Set
		End Property

		Public Overrides Function ToString() As String
			Return "ScaleVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & """,scaleFactor=" & scaleFactor & ")"
		End Function

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'No op
			If maskArrays Is Nothing OrElse maskArrays.Length = 0 Then
				Return Nothing
			End If

			Return New Pair(Of INDArray, MaskState)(maskArrays(0), currentMaskState)
		End Function
	End Class

End Namespace