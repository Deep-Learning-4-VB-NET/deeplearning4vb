Imports System
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseGraphVertex = org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports [Or] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Or
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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
	Public Class PoolHelperVertex
		Inherits BaseGraphVertex

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
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
				Throw New System.InvalidOperationException("Cannot do forward pass: inputs not set")
			End If

			If inputs.Length > 1 Then
				Throw New System.InvalidOperationException("PoolHelper vertex requires a single input.")
			End If

			Dim strippedInput As INDArray = inputs(0).get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(1, inputs(0).size(2)), NDArrayIndex.interval(1, inputs(0).size(3)))
			Return workspaceMgr.dup(ArrayType.ACTIVATIONS, strippedInput)
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: errors not set")
			End If

			Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, epsilon_Conflict.dataType(), epsilon_Conflict.size(0), epsilon_Conflict.size(1), 1+epsilon_Conflict.size(2), 1+epsilon_Conflict.size(2))
			[out].get(NDArrayIndex.all(), NDArrayIndex.all(),NDArrayIndex.interval(1, inputs(0).size(2)), NDArrayIndex.interval(1, inputs(0).size(3))).assign(epsilon_Conflict)

			Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {[out]})
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				If backpropGradientsViewArray IsNot Nothing Then
					Throw New Exception("Vertex does not have gradients; gradients view array cannot be set here")
				End If
			End Set
		End Property

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			If maskArrays Is Nothing Then
				Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
			End If

			'Most common case: all or none.
			'If there's only *some* mask arrays: assume the others (missing) are equivalent to all 1s
			'And for handling multiple masks: best strategy seems to be an OR operation
			'i.e., output is 1 if any of the input are 1s
			'Which means: if any masks are missing, output null (equivalent to no mask, or all steps present)
			'Otherwise do an element-wise OR operation

			For Each arr As INDArray In maskArrays
				If arr Is Nothing Then
					Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
				End If
			Next arr

			'At this point: all present. Do OR operation
			If maskArrays.Length = 1 Then
				Return New Pair(Of INDArray, MaskState)(maskArrays(0), currentMaskState)
			Else
				Dim ret As INDArray = maskArrays(0).dup(maskArrays(0).ordering())
				Nd4j.Executioner.exec(New [Or](maskArrays(0), maskArrays(1), ret))
				For i As Integer = 2 To maskArrays.Length - 1
					Nd4j.Executioner.exec(New [Or](maskArrays(i), ret, ret))
				Next i
				Return New Pair(Of INDArray, MaskState)(ret, currentMaskState)
			End If
		End Function

		Public Overrides Function ToString() As String
			Return "PoolHelperVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & """)"
		End Function
	End Class

End Namespace