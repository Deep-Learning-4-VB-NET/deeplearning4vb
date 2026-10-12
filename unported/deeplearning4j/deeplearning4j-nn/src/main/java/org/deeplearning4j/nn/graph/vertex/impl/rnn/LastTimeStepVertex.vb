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
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
	Public Class LastTimeStepVertex
		Inherits BaseGraphVertex

		Private inputName As String
		Private inputIdx As Integer
		''' <summary>
		''' Shape of the forward pass activations </summary>
		Private fwdPassShape() As Long
		''' <summary>
		''' Indexes of the time steps that were extracted, for each example </summary>
		Private fwdPassTimeSteps() As Integer

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputName As String, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, inputName, dataType)
		End Sub


		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal inputName As String, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.inputName = inputName
			Me.inputIdx = graph.Configuration.getNetworkInputs().IndexOf(inputName)
			If inputIdx = -1 Then
				Throw New System.ArgumentException("Invalid input name: """ & inputName & """ not found in list " & "of network inputs (" & graph.Configuration.getNetworkInputs() & ")")
			End If
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
			'First: get the mask arrays for the given input, if any
			Dim inputMaskArrays() As INDArray = graph.InputMaskArrays
			Dim mask As INDArray = (If(inputMaskArrays IsNot Nothing, inputMaskArrays(inputIdx), Nothing))

			'Then: work out, from the mask array, which time step of activations we want, extract activations
			'Also: record where they came from (so we can do errors later)
			fwdPassShape = inputs(0).shape()

			Dim [out] As INDArray
			If mask Is Nothing Then
				'No mask array -> extract same (last) column for all
				Dim lastTS As Long = inputs(0).size(2) - 1
				[out] = inputs(0).get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(lastTS))
				[out] = workspaceMgr.dup(ArrayType.ACTIVATIONS, [out])
				fwdPassTimeSteps = Nothing 'Null -> last time step for all examples
			Else
				Dim outShape As val = New Long() {inputs(0).size(0), inputs(0).size(1)}
				[out] = workspaceMgr.create(ArrayType.ACTIVATIONS, inputs(0).dataType(), outShape)

				'Want the index of the last non-zero entry in the mask array.
				'Check a little here by using mulRowVector([0,1,2,3,...]) and argmax
				Dim maxTsLength As Long = fwdPassShape(2)
				Dim row As INDArray = Nd4j.linspace(0, maxTsLength - 1, maxTsLength, mask.dataType())
				Dim temp As INDArray = mask.mulRowVector(row)
				Dim lastElementIdx As INDArray = Nd4j.argMax(temp, 1)
				fwdPassTimeSteps = New Integer(CInt(fwdPassShape(0)) - 1){}
				For i As Integer = 0 To fwdPassTimeSteps.Length - 1
					fwdPassTimeSteps(i) = CInt(Math.Truncate(lastElementIdx.getDouble(i)))
				Next i

				'Now, get and assign the corresponding subsets of 3d activations:
				For i As Integer = 0 To fwdPassTimeSteps.Length - 1
					[out].putRow(i, inputs(0).get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(fwdPassTimeSteps(i))))
				Next i
			End If

			Return [out]
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())

			'Allocate the appropriate sized array:
			Dim epsilonsOut As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, epsilon_Conflict.dataType(), fwdPassShape, "f"c)

			If fwdPassTimeSteps Is Nothing Then
				'Last time step for all examples
				epsilonsOut.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(fwdPassShape(2) - 1)}, epsilon_Conflict)
			Else
				'Different time steps were extracted for each example
				For i As Integer = 0 To fwdPassTimeSteps.Length - 1
					epsilonsOut.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(fwdPassTimeSteps(i))}, epsilon_Conflict.getRow(i))
				Next i
			End If
			Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {epsilonsOut})
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				If backpropGradientsViewArray IsNot Nothing Then
					Throw New Exception("Vertex does not have gradients; gradients view array cannot be set here")
				End If
			End Set
		End Property

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'Input: 2d mask array, for masking a time series. After extracting out the last time step, we no longer need the mask array

			Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
		End Function

		Public Overrides Function ToString() As String
			Return "LastTimeStepVertex(inputName=" & inputName & ")"
		End Function
	End Class

End Namespace