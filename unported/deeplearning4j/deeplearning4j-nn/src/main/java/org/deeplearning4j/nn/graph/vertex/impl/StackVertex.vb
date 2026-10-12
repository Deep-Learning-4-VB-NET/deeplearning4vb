Imports System
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseGraphVertex = org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
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

Namespace org.deeplearning4j.nn.graph.vertex.impl

	<Serializable>
	Public Class StackVertex
		Inherits BaseGraphVertex

		Private lastInputShapes()() As Long

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
			' stacking along dimension 0
			' inputs[] is an array of INDArray (e.g.: shape of 3 x [nExamples, nSize])
			' what we want to do is make a stacked output (e.g.: [3 x nExamples, nSize])
			lastInputShapes = Nothing
			Dim nStack As Integer = inputs.Length
			Dim inShape As val = inputs(0).shape()
			Dim outShape As val = New Long(inShape.length - 1){}

			' create the new shape
			outShape(0) = nStack * inShape(0)
			For i As Integer = 1 To inShape.length - 1
				outShape(i) = inShape(i)
			Next i

			Dim variableLengthTS As Boolean = False
			If inShape.length = 3 Then
				'RNN data - check for variable length time series
				Dim minLength As Long = inputs(0).size(2)
				Dim maxLength As Long = minLength
				For i As Integer = 1 To inputs.Length - 1
					Dim thisLength As Long = inputs(i).size(2)
					minLength = Math.Min(minLength, thisLength)
					maxLength = Math.Max(maxLength, thisLength)
				Next i
				variableLengthTS = (minLength <> maxLength)

				If Not variableLengthTS Then
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
						Return Nd4j.concat(0, inputs)
					End Using
				End If

				outShape(2) = maxLength
				Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, inputs(0).dataType(), outShape)
				Dim numExamples As Long = inputs(0).size(0)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: lastInputShapes = new Long[inputs.Length][0]
				lastInputShapes = RectangularArrays.RectangularLongArray(inputs.Length, 0)
				For i As Integer = 0 To inputs.Length - 1
					[out].put(New INDArrayIndex() {NDArrayIndex.interval(i * numExamples, (i + 1) * numExamples), NDArrayIndex.all(), NDArrayIndex.interval(0, inputs(i).size(2))}, inputs(i))
					lastInputShapes(i) = inputs(i).shape()
				Next i

				Return [out]
			Else
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
					Return Nd4j.concat(0, inputs)
				End Using
			End If
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			' this is basically doForward on UnstackVertex
			If Not canDoForward() Then
				Throw New System.InvalidOperationException("Cannot do forward pass: input not set")
			End If

			If epsilon_Conflict Is Nothing Then
				'Edge case for stack vertex: stack -> embedding
				'If the null epsilons are a problem in practice, this should be picked up by other layers
				Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray(inputs.Length - 1){})
			End If

			Dim nStack As Integer = inputs.Length
			Dim [out](nStack - 1) As INDArray

			Dim [step] As Long = epsilon_Conflict.size(0) \ nStack

			For i As Integer = 0 To nStack - 1
				Select Case epsilon_Conflict.rank()
					Case 2
						[out](i) = epsilon_Conflict.get(NDArrayIndex.interval(i * [step], (i + 1) * [step]), NDArrayIndex.all())
					Case 3
						If lastInputShapes IsNot Nothing Then
							'Variable length time series case
							[out](i) = epsilon_Conflict.get(NDArrayIndex.interval(i * [step], (i + 1) * [step]), NDArrayIndex.all(), NDArrayIndex.interval(0, lastInputShapes(i)(2)))
						Else
							[out](i) = epsilon_Conflict.get(NDArrayIndex.interval(i * [step], (i + 1) * [step]), NDArrayIndex.all(), NDArrayIndex.all())
						End If
					Case 4
						[out](i) = epsilon_Conflict.get(NDArrayIndex.interval(i * [step], (i + 1) * [step]), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all())
					Case Else
						Throw New System.NotSupportedException("Cannot get subset for activations of rank " & inputs(0).rank())
				End Select
			Next i

			For i As Integer = 0 To nStack - 1
				[out](i) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, [out](i))
			Next i

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
			'Cases here: no mask arrays, or all mask arrays - all of the same size
			If maskArrays Is Nothing Then
				Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
			End If

			Dim allNull As Boolean = True
			For Each i As INDArray In maskArrays
				If i IsNot Nothing Then
					allNull = False
					Exit For
				End If
			Next i
			If allNull Then
				Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
			End If

			' stacking along dimension 0
			'Given masks are all either 1d (column vector) or 2d (examples, timeSeriesLength) we can just vStack the masks
			'However: variable length TS might have different length masks...
			Dim allSameLength As Boolean = True
			Dim size1_ex0 As Long = maskArrays(0).size(1)
			Dim maxLength As Long = size1_ex0
			For i As Integer = 1 To maskArrays.Length - 1
				allSameLength = allSameLength And (size1_ex0 = maskArrays(i).size(1))
				maxLength = Math.Max(maxLength, maskArrays(i).size(1))
			Next i

			If allSameLength Then
				Return New Pair(Of INDArray, MaskState)(Nd4j.vstack(maskArrays), currentMaskState)
			Else
				Dim numExamples As Long = maskArrays(0).size(0)
				Dim outMask As INDArray = Nd4j.create(maskArrays.Length * numExamples, maxLength)
				For i As Integer = 0 To maskArrays.Length - 1
					outMask.put(New INDArrayIndex() {NDArrayIndex.interval(i * numExamples, (i + 1) * numExamples), NDArrayIndex.interval(0, maskArrays(i).size(1))}, maskArrays(i))
				Next i

				Return New Pair(Of INDArray, MaskState)(outMask, currentMaskState)
			End If
		End Function

		Public Overrides Function ToString() As String
			Return "StackVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & ")"
		End Function
	End Class

End Namespace