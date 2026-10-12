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
Imports BroadcastDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastDivOp
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
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
	Public Class L2NormalizeVertex
		Inherits BaseGraphVertex

		Private Shared ReadOnly DEFAULT_RANK2_DIMS() As Integer = {1}
		Private Shared ReadOnly DEFAULT_RANK3_DIMS() As Integer = {1, 2}
		Private Shared ReadOnly DEFAULT_RANK4_DIMS() As Integer = {1, 2, 3}

		Private dimension() As Integer
		Private eps As Double

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal dimension() As Integer, ByVal eps As Double, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, dimension, eps, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal dimension() As Integer, ByVal eps As Double, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.dimension = dimension
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
				Throw New System.InvalidOperationException("Cannot do forward pass: inputs not set (L2NormalizeVertex " & vertexName_Conflict & " idx " & vertexIndex_Conflict & ")")
			End If

			' L2 norm along all dimensions except 0, unless user-specified
			' x / |x|2
			Dim x As INDArray = inputs(0)
			Dim dimensions() As Integer = getDimensions(x)

			Dim xNorm2 As INDArray = x.norm2(True,dimensions)
			Transforms.max(xNorm2, eps, False)
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
				If x.rank() = 2 Then
					Return x.divColumnVector(xNorm2)
				Else
					Dim [out] As INDArray = Nd4j.createUninitialized(x.shape(), x.ordering())
					Return Nd4j.Executioner.exec(New BroadcastDivOp(x, xNorm2, [out], 0))
				End If
			End Using
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: errors not set (L2NormalizeVertex " & vertexName_Conflict & " idx " & vertexIndex_Conflict & ")")
			End If

			Dim x As INDArray = inputs(0)
			Dim dimensions() As Integer = getDimensions(x)

			Dim norm As INDArray = x.norm2(dimensions)
			Dim norm3 As INDArray = Transforms.pow(norm, 3.0, True)
			Transforms.max(norm, eps, False) ' in case of div/0
			Transforms.max(norm3, eps, False)

			Dim dLdx As INDArray
			If x.rank() = 2 Then
				' 2D case
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
					dLdx = epsilon_Conflict.divColumnVector(norm)
				End Using
				Dim xDivNorm3 As INDArray = x.divColumnVector(norm3)
				dLdx.subi(xDivNorm3.muliColumnVector(epsilon_Conflict.mul(x).sum(1)))
			Else
				'RNN and CNN case - Broadcast along dimension 0
				Dim dx As INDArray = epsilon_Conflict.mul(x).sum(dimensions)

				'x / |x|_2^3 * sum_k (dLda*x)
				Dim xDivNorm3 As INDArray = Nd4j.createUninitialized(x.shape(), x.ordering())
				Nd4j.Executioner.exec(New BroadcastDivOp(x, norm3, xDivNorm3, 0))
				Nd4j.Executioner.exec(New BroadcastMulOp(xDivNorm3, dx, xDivNorm3, 0))

				'1/|x|_2 * dLda - above
				dLdx = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, epsilon_Conflict.dataType(), epsilon_Conflict.shape(), epsilon_Conflict.ordering())
				Nd4j.Executioner.exec(New BroadcastDivOp(epsilon_Conflict, norm, dLdx, 0))
				dLdx.subi(xDivNorm3)
			End If

			Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {dLdx})
		End Function

		Private Function getDimensions(ByVal x As INDArray) As Integer()
			If dimension Is Nothing OrElse dimension.Length < 1 Then
				Select Case x.rank()
					Case 2
						Return DEFAULT_RANK2_DIMS
					Case 3
						Return DEFAULT_RANK3_DIMS
					Case 4
						Return DEFAULT_RANK4_DIMS
					Case Else
						Throw New Exception()
				End Select
			End If
			Return dimension
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				If backpropGradientsViewArray IsNot Nothing Then
					Throw New Exception("Vertex does not have gradients; gradients view array cannot be set here")
				End If
			End Set
		End Property

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'No op
			If maskArrays Is Nothing OrElse maskArrays.Length = 0 Then
				Return Nothing
			End If

			Return New Pair(Of INDArray, MaskState)(maskArrays(0), currentMaskState)
		End Function

		Public Overrides Function ToString() As String
			Return "L2NormalizeVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & ",dim=""" & dimension & """)"
		End Function
	End Class

End Namespace