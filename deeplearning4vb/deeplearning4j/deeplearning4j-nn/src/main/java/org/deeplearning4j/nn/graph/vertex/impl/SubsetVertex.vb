Imports System
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseGraphVertex = org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
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
	Public Class SubsetVertex
		Inherits BaseGraphVertex

		Private from As Integer
		Private [to] As Integer 'inclusive
		Private forwardShape() As Long

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal from As Integer, ByVal [to] As Integer, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, from, [to], dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal from As Integer, ByVal [to] As Integer, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.from = from
			Me.to = [to]
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

			forwardShape = Arrays.CopyOf(inputs(0).shape(), inputs(0).rank())

			Dim [out] As INDArray
			Select Case inputs(0).rank()
				Case 2
					[out] = inputs(0).get(NDArrayIndex.all(), NDArrayIndex.interval(from, [to], True))
				Case 3
					[out] = inputs(0).get(NDArrayIndex.all(), NDArrayIndex.interval(from, [to], True), NDArrayIndex.all())
				Case 4
					[out] = inputs(0).get(NDArrayIndex.all(), NDArrayIndex.interval(from, [to], True), NDArrayIndex.all(), NDArrayIndex.all())
				Case Else
					Throw New System.NotSupportedException("Cannot get subset for activations of rank " & inputs(0).rank())
			End Select
			Return workspaceMgr.dup(ArrayType.ACTIVATIONS, [out])
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: error not set")
			End If

			Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, epsilon_Conflict.dataType(), forwardShape)
			Select Case forwardShape.Length
				Case 2
					[out].put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(from, [to], True)}, epsilon_Conflict)
				Case 3
					[out].put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(from, [to], True), NDArrayIndex.all()}, epsilon_Conflict)
				Case 4
					[out].put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(from, [to], True), NDArrayIndex.all(), NDArrayIndex.all()}, epsilon_Conflict)
				Case Else
					Throw New Exception("Invalid activation rank") 'Should never happen
			End Select
			Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {[out]})
		End Function

		Public Overrides Function ToString() As String
			Return "SubsetVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & """,fromIdx=" & from & ",toIdx=" & [to] & ")"
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				If backpropGradientsViewArray IsNot Nothing Then
					Throw New Exception("Vertex does not have gradients; gradients view array cannot be set here")
				End If
			End Set
		End Property

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'No op: subset just provides part of the activations for each example (or time step)
			If maskArrays Is Nothing OrElse maskArrays.Length = 0 Then
				Return Nothing
			End If

			Return New Pair(Of INDArray, MaskState)(maskArrays(0), currentMaskState)
		End Function
	End Class

End Namespace