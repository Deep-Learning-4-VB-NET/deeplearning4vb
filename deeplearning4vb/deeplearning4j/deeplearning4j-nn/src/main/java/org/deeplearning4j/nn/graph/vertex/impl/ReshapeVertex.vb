Imports System
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseGraphVertex = org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives

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
	Public Class ReshapeVertex
		Inherits BaseGraphVertex

		Private order As Char
		Private newShape() As Integer
		Private maskShape() As Integer


		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal order As Char, ByVal newShape() As Integer, ByVal maskShape() As Integer, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, order, newShape, maskShape, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal order As Char, ByVal newShape() As Integer, ByVal maskShape() As Integer, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.order = order
			Me.newShape = newShape
			Me.maskShape = maskShape
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
				Throw New System.InvalidOperationException("Reshape vertex requires a single input.")
			End If


			Return workspaceMgr.dup(ArrayType.ACTIVATIONS, inputs(0).reshape(order, newShape))
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: errors not set")
			End If

			Dim [out](0) As INDArray
			[out](0) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict.reshape(order, inputs(0).shape()))
			Return New Pair(Of Gradient, INDArray())(Nothing, [out])
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				If backpropGradientsViewArray IsNot Nothing Then
					Throw New Exception("Vertex does not have gradients; gradients view array cannot be set here")
				End If
			End Set
		End Property

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			If maskArrays Is Nothing OrElse maskArrays.Length < 1 OrElse maskArrays(0) Is Nothing Then
				Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
			End If

			If maskShape IsNot Nothing Then
				Return New Pair(Of INDArray, MaskState)(maskArrays(0).reshape(order, maskShape), currentMaskState)
			End If

			'Mask array is an input mask. Therefore: 2 possible cases
			'(a) column vector mask (MLP, CNN), and
			'  i. output is rank 2 or 4 (MLP, CNN) -> no change
			' ii. output is rank 3 (RNN) -> to 2d
			'(b) 2d mask (RNN), and
			'  i. output is rank 2 or 4 (MLP, CNN) -> mask to column vector
			' ii. output is rank 3 (RNN) -> no change


			If maskArrays(0).ColumnVectorOrScalar Then
				If newShape.Length = 2 OrElse newShape.Length = 4 Then
					Return New Pair(Of INDArray, MaskState)(maskArrays(0), currentMaskState)
				ElseIf newShape.Length = 3 Then
					'Column vector -> 2d (FF -> RNN etc)
					Dim newMaskShape() As Integer = {newShape(0), newShape(2)}
					Return New Pair(Of INDArray, MaskState)(maskArrays(0).reshape(order, newMaskShape), currentMaskState)
				End If
			Else
				If newShape.Length = 3 Then
					Return New Pair(Of INDArray, MaskState)(maskArrays(0), currentMaskState)
				Else
					'RNN -> FF/CNN
					Dim newMaskShape() As Integer = {newShape(0)*newShape(2), 1}
					Return New Pair(Of INDArray, MaskState)(maskArrays(0).reshape(order, newMaskShape), currentMaskState)
				End If
			End If

			'Other unknown case - shouldn't happen...
			Return New Pair(Of INDArray, MaskState)(maskArrays(0), currentMaskState)
		End Function

		Public Overrides Function ToString() As String
			Return "ReshapeVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & """,shape=" & newShape.ToString() & ")"
		End Function
	End Class

End Namespace