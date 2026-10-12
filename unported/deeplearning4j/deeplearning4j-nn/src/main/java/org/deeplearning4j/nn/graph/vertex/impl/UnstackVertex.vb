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
	Public Class UnstackVertex
		Inherits BaseGraphVertex

		Private from As Long
		Private stackSize As Integer
		Private forwardShape() As Long
		Private [step] As Long

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal from As Integer, ByVal stackSize As Integer, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, from, stackSize, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal from As Integer, ByVal stackSize As Integer, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.from = from
			Me.stackSize = stackSize
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

			' once we know the inputs, save the shape and interval size for doBackward
			Me.forwardShape = Arrays.CopyOf(inputs(0).shape(), inputs(0).rank())

			Me.step = inputs(0).size(0) \ stackSize
			Dim start As Long = from * [step]
			Dim [end] As Long = (from + 1) * [step]

			Dim ret As INDArray
			Select Case inputs(0).rank() 'TODO remove the dups here if/when possible (gradient checks must pass)
				Case 2
					ret = inputs(0).get(NDArrayIndex.interval(start, [end]), NDArrayIndex.all())
				Case 3
					ret = inputs(0).get(NDArrayIndex.interval(start, [end]), NDArrayIndex.all(), NDArrayIndex.all())
				Case 4
					ret = inputs(0).get(NDArrayIndex.interval(start, [end]), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
				Case Else
					Throw New System.NotSupportedException("Cannot get subset for activations of rank " & inputs(0).rank())
			End Select

			Return workspaceMgr.dup(ArrayType.ACTIVATIONS, ret)
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: error not set")
			End If

			Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, inputs(0).dataType(), forwardShape)
			Dim start As Long = from * [step]
			Dim [end] As Long = (from + 1) * [step]

			Select Case forwardShape.Length
				Case 2
					[out].put(New INDArrayIndex() {NDArrayIndex.interval(start, [end]), NDArrayIndex.all()}, epsilon_Conflict)
				Case 3
					[out].put(New INDArrayIndex() {NDArrayIndex.interval(start, [end]), NDArrayIndex.all(), NDArrayIndex.all()}, epsilon_Conflict)
				Case 4
					[out].put(New INDArrayIndex() {NDArrayIndex.interval(start, [end]), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()}, epsilon_Conflict)
				Case Else
					Throw New Exception("Invalid activation rank") 'Should never happen
			End Select
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
			If maskArrays Is Nothing OrElse maskArrays.Length = 0 Then
				Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
			End If

			Dim allNull As Boolean = True
			For i As Integer = 0 To maskArrays.Length - 1
				If maskArrays(i) IsNot Nothing Then
					allNull = False
					Exit For
				End If
			Next i
			If allNull Then
				Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
			End If

			'Mask arrays are either 1d (column vector) or 2d...
			Dim start As Long = from * minibatchSize
			Dim [end] As Long = (from + 1) * minibatchSize
			Dim outMask As INDArray = maskArrays(0).get(NDArrayIndex.interval(start, [end]), NDArrayIndex.all())
			Return New Pair(Of INDArray, MaskState)(outMask, currentMaskState)
		End Function

		Public Overrides Function ToString() As String
			Return "UnstackVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & """,fromIdx=" & from & ",forwardShape=" & forwardShape & ")"
		End Function
	End Class

End Namespace