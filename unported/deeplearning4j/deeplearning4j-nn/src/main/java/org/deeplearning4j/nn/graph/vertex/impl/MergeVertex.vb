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
Imports [Or] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Or
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
	Public Class MergeVertex
		Inherits BaseGraphVertex

		Private forwardPassShapes()() As Long
		Private fwdPassRank As Integer
		Private mergeAxis As Integer

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal dataType As DataType, ByVal mergeAxis As Integer)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, dataType, mergeAxis)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal dataType As DataType, ByVal mergeAxis As Integer)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.mergeAxis = mergeAxis
		End Sub

		Public Overrides Function ToString() As String
			Return "MergeVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & """)"
		End Function

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

			If inputs.Length = 1 Then
				'No-op case
				Dim shape As val = inputs(0).shape()
				forwardPassShapes = New Long()() {Arrays.CopyOf(shape, shape.length)}
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, inputs(0))
			End If

			Dim [in](inputs.Length - 1) As INDArray
			For i As Integer = 0 To [in].Length - 1
				[in](i) = inputs(i).castTo(dataType) 'No-op if correct type
			Next i

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: forwardPassShapes = new Long[in.Length][0]
			forwardPassShapes = RectangularArrays.RectangularLongArray([in].Length, 0)
			Dim nExamples As val = [in](0).size(0)
			fwdPassRank = [in](0).rank()
			For i As Integer = 0 To [in].Length - 1
				Dim currShape As val = [in](i).shape()
				If fwdPassRank <> currShape.length Then
					Throw New System.InvalidOperationException("Cannot merge activations with different ranks: first activations have rank " & fwdPassRank & ", activations[" & i & "] have rank " & currShape.length & " (shape=" & Arrays.toString(currShape) & ")")
				End If
				forwardPassShapes(i) = Arrays.CopyOf(currShape, currShape.length)
				If currShape(0) <> nExamples Then
					Throw New System.InvalidOperationException("Cannot merge activations with different number of examples (activations[0] shape: " & Arrays.toString([in](0).shape()) & ", activations[" & i & "] shape: " & Arrays.toString([in](i).shape()))
				End If
			Next i

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
				Dim [out] As INDArray = Nd4j.concat(mergeAxis, [in])
				Return [out]
			End Using
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: errors not set")
			End If

			If forwardPassShapes.Length = 1 Then
				'No op case
				Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilon_Conflict)})
			End If

			'Split the epsilons in the opposite way that the activations were merged
			Dim [out](forwardPassShapes.Length - 1) As INDArray
			For i As Integer = 0 To [out].Length - 1
				[out](i) = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, epsilon_Conflict.dataType(), forwardPassShapes(i))
			Next i

			Dim cumulative As Integer = 0
			Select Case fwdPassRank
				Case 2
					'Standard
					For i As Integer = 0 To forwardPassShapes.Length - 1
						[out](i).assign(epsilon_Conflict.get(NDArrayIndex.all(), NDArrayIndex.interval(cumulative, cumulative + forwardPassShapes(i)(1)))) 'subset of columns
						cumulative += forwardPassShapes(i)(1)
					Next i
				Case 3
					For i As Integer = 0 To forwardPassShapes.Length - 1
						[out](i).assign(epsilon_Conflict.get(indices(3, mergeAxis, cumulative, cumulative + forwardPassShapes(i)(mergeAxis)))) 'All time steps

						cumulative += forwardPassShapes(i)(mergeAxis)
					Next i
				Case 4
					For i As Integer = 0 To forwardPassShapes.Length - 1
						[out](i).assign(epsilon_Conflict.get(indices(4, mergeAxis, cumulative, cumulative + forwardPassShapes(i)(mergeAxis)))) 'height

						cumulative += forwardPassShapes(i)(mergeAxis)
					Next i
				Case Else
					Throw New Exception("Invalid rank during forward pass (not 2, 3, 4)") 'Should never happen
			End Select

			Return New Pair(Of Gradient, INDArray())(Nothing, [out])
		End Function

		Private Function indices(ByVal num As Integer, ByVal axis As Integer, ByVal from As Long, ByVal [to] As Long) As INDArrayIndex()
			Dim [out](num - 1) As INDArrayIndex
			For i As Integer = 0 To num - 1
				If i = axis Then
					[out](i) = NDArrayIndex.interval(from, [to])
				Else
					[out](i) = NDArrayIndex.all()
				End If
			Next i
			Return [out]
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
			'Which means: if any masks are missing, output null (equivalent to no mask)
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
				Dim ret As INDArray
				If maskArrays(0).dataType() = DataType.BOOL Then
					ret = maskArrays(0).dup(maskArrays(0).ordering())
				Else
					ret = maskArrays(0).castTo(DataType.BOOL)
				End If
				Nd4j.Executioner.exec(New [Or](ret, maskArrays(1).castTo(DataType.BOOL), ret))
				For i As Integer = 2 To maskArrays.Length - 1
					Nd4j.Executioner.exec(New [Or](maskArrays(i).castTo(DataType.BOOL), ret, ret))
				Next i
				Return New Pair(Of INDArray, MaskState)(ret, currentMaskState)
			End If
		End Function
	End Class

End Namespace