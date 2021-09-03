Imports System
Imports val = lombok.val
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

Namespace org.deeplearning4j.nn.graph.vertex.impl.rnn

	<Serializable>
	Public Class DuplicateToTimeSeriesVertex
		Inherits BaseGraphVertex

		Private inputName As String
		Private inputVertexIndex As Integer

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertexName As String, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, inputVertexName, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal inputName As String, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.inputName = inputName
			Me.inputVertexIndex = graph.Configuration.getNetworkInputs().IndexOf(inputName)
			If inputVertexIndex = -1 Then
				Throw New System.ArgumentException("Invalid input name: """ & inputName & """ not found in list " & "of network inputs (" & graph.Configuration.getNetworkInputs() & ")")
			End If
		End Sub

		Public Overrides Function hasLayer() As Boolean
			Return False
		End Function

		Public Overrides ReadOnly Property OutputVertex As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Layer As Layer
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function doForward(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

			'First: work out the time series length
			Dim tsLength As val = graph.getInput(inputVertexIndex).size(2)
			Dim outShape As val = New Long() {inputs(0).size(0), inputs(0).size(1), tsLength}

			Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, inputs(0).dataType(), outShape, "f"c)
			For i As Integer = 0 To tsLength - 1
				[out].put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i)}, inputs(0))
			Next i
			Return [out]
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			'Because we duplicated for each time step: simply need to sum along time for errors/epsilons
			Dim ret As INDArray = epsilon_Conflict.sum(workspaceMgr.create(ArrayType.ACTIVATION_GRAD, epsilon_Conflict.dataType(), epsilon_Conflict.size(0), epsilon_Conflict.size(1)), 2)
			Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {ret})
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				If backpropGradientsViewArray IsNot Nothing Then
					Throw New Exception("Vertex does not have gradients; gradients view array cannot be set here")
				End If
			End Set
		End Property

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'Present for all time steps, or as per the corresponding input mask (if present)
			Dim allMasks() As INDArray = graph.InputMaskArrays
			If allMasks Is Nothing OrElse allMasks(inputVertexIndex) Is Nothing Then
				'No mask
				Return Nothing
			End If
			Return New Pair(Of INDArray, MaskState)(allMasks(inputVertexIndex), MaskState.Active)
		End Function

		Public Overrides Function ToString() As String
			Return "DuplicateToTimeSeriesVertex(inputName=" & inputName & ")"
		End Function
	End Class

End Namespace