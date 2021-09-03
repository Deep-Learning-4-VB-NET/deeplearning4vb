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
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports EuclideanDistance = org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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
	Public Class L2Vertex
		Inherits BaseGraphVertex

		Private eps As Double

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal eps As Double, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, eps, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal eps As Double, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.eps = eps
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
				Throw New System.InvalidOperationException("Cannot do forward pass: input not set")
			End If

			Dim a As INDArray = inputs(0)
			Dim b As INDArray = inputs(1)

			Dim dimensions(a.rank() - 2) As Integer
			Dim i As Integer = 1
			Do While i < a.rank()
				dimensions(i - 1) = i
				i += 1
			Loop

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
				Dim arr As INDArray = Nd4j.Executioner.exec(New EuclideanDistance(a, b, dimensions))
				Return arr.reshape(ChrW(arr.size(0)), 1)
			End Using
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: error not set")
			End If

			Dim a As INDArray = inputs(0)
			Dim b As INDArray = inputs(1)
			Dim [out] As INDArray = doForward(tbptt, workspaceMgr)
			Transforms.max([out], eps, False) ' in case of 0

			Dim dLdlambda As INDArray = epsilon_Conflict 'dL/dlambda aka 'epsilon' - from layer above

			Dim sNegHalf As INDArray = [out].rdiv(1.0) 's^(-1/2) = 1.0 / s^(1/2) = 1.0 / out

			Dim diff As INDArray
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
				diff = a.sub(b)
			End Using

			Dim first As INDArray = dLdlambda.mul(sNegHalf) 'Column vector for all cases

			Dim dLda As INDArray
			Dim dLdb As INDArray
			If a.rank() = 2 Then
				'2d case (MLPs etc)
				dLda = diff.muliColumnVector(first)
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
					dLdb = dLda.neg()
				End Using
			Else
				'RNN and CNN case - Broadcast along dimension 0
				dLda = Nd4j.Executioner.exec(New BroadcastMulOp(diff, first, diff, 0))
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
					dLdb = dLda.neg()
				End Using
			End If

			Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {dLda, dLdb})
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				If backpropGradientsViewArray IsNot Nothing Then
					Throw New Exception("Vertex does not have gradients; gradients view array cannot be set here")
				End If
			End Set
		End Property

		Public Overrides Function ToString() As String
			Return "L2Vertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & ")"
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