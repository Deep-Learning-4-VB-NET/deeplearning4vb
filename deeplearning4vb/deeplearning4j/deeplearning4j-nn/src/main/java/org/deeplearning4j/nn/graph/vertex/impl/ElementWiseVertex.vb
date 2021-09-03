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
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports BroadcastTo = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastTo
Imports MatchConditionTransform = org.nd4j.linalg.api.ops.impl.transforms.bool.MatchConditionTransform
Imports SubOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.SubOp
Imports [Or] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Or
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
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
	Public Class ElementWiseVertex
		Inherits BaseGraphVertex

		Public Enum Op
			Add
			Subtract
			Product
			Average
			Max
		End Enum

		Private op As Op
		Private nInForwardPass As Integer

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal op As Op, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, op, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal op As Op, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.op = op
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

			nInForwardPass = inputs.Length
			If inputs.Length = 1 Then
				Return workspaceMgr.dup(ArrayType.ACTIVATIONS, inputs(0))
			End If

			Dim isBc As Boolean = False
			For i As Integer = 1 To inputs.Length - 1
				If Not inputs(0).equalShapes(inputs(i)) Then
					isBc = True
					Exit For
				End If
			Next i

			Dim outShape() As Long
			If Not isBc Then
				outShape = inputs(0).shape()
			Else
				outShape = Shape.broadcastOutputShape(inputs(0).shape(), inputs(1).shape())
				For i As Integer = 2 To inputs.Length - 1
					outShape = Shape.broadcastOutputShape(outShape, inputs(i).shape())
				Next i
			End If

			Select Case op
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Add
					Dim sum As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, dataType, outShape)
					If isBc AndAlso Not outShape.SequenceEqual(inputs(0).shape()) Then
						Nd4j.exec(New BroadcastTo(inputs(0), outShape, sum))
					Else
						sum.assign(inputs(0))
					End If

					For i As Integer = 1 To inputs.Length - 1
						sum.addi(inputs(i).castTo(dataType))
					Next i
					Return sum
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Average
					Dim average As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, dataType, outShape)
					If isBc AndAlso Not outShape.SequenceEqual(inputs(0).shape()) Then
						Nd4j.exec(New BroadcastTo(inputs(0), outShape, average))
					Else
						average.assign(inputs(0))
					End If
					For i As Integer = 1 To inputs.Length - 1
						average.addi(inputs(i).castTo(dataType))
					Next i
					Return average.divi(inputs.Length)
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Subtract
					If inputs.Length <> 2 Then
						Throw New System.ArgumentException("ElementWise subtraction only supports 2 inputs")
					End If
					Return Nd4j.exec(New SubOp(inputs, New INDArray(){workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, inputs(0).dataType(), outShape)}))(0)
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Product
					Dim product As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, dataType, outShape)

					If isBc AndAlso Not outShape.SequenceEqual(inputs(0).shape()) Then
						Nd4j.exec(New BroadcastTo(inputs(0), outShape, product))
					Else
						product.assign(inputs(0))
					End If

					For i As Integer = 1 To inputs.Length - 1
						product.muli(inputs(i).castTo(dataType))
					Next i
					Return product
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Max
					Dim isBroadcast As Boolean = False
					For i As Integer = 1 To inputs.Length - 1
						isBroadcast = isBroadcast Or Not inputs(0).equalShapes(inputs(i))
						If isBroadcast Then
							Exit For
						End If
					Next i
					If Not isBroadcast Then
						Dim max As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, inputs(0).dataType(), inputs(0).shape(), inputs(0).ordering())
						Dim op As CustomOp = DynamicCustomOp.builder("mergemax").addInputs(inputs).addOutputs(max).callInplace(False).build()
						Nd4j.Executioner.exec(op)
						Return max
					Else
						'AB 20190729 mergemax doesn't support broadcast at this point
						If inputs.Length = 1 Then
							Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, inputs(0))
						Else
							Dim max As INDArray = Transforms.max(inputs(0), inputs(1), True)
							For i As Integer = 2 To inputs.Length - 1
								max = Transforms.max(max, inputs(i), False)
							Next i
							Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, max)
						End If
					End If

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case Else
					Throw New System.NotSupportedException("Unknown op: " & Me.op)
			End Select
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				Throw New System.InvalidOperationException("Cannot do backward pass: errors not set")
			End If

			If nInForwardPass = 1 Then
				Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict)})
			End If

			Dim broadcastCase As Boolean = False
			For i As Integer = 1 To nInForwardPass - 1
				broadcastCase = broadcastCase Or Not inputs(0).equalShapes(inputs(i))
			Next i

			Select Case op
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Add
					'If x=sum_i a_i then dL/da_i = dL/dx * dx/da_i = dL/dx
					Dim [out](nInForwardPass - 1) As INDArray
					For i As Integer = 0 To nInForwardPass - 1
						If Not broadcastCase Then
							'Standard case
							[out](i) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict)
						Else
							'For broadcast case, we need to sum along the broadcast dimensions
							'So if [mb,3]+[mb,1] -> input 0 backprops epsilon, input 1 backprops epsilon.sum(1,keepDim=true)
							If inputs(i).equalShapes(epsilon_Conflict) Then
								[out](i) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict)
							Else
								Dim bcDim() As Integer = Shape.getBroadcastDimensions(inputs(i).shape(), epsilon_Conflict.shape())
								Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
									[out](i) = epsilon_Conflict.sum(True, bcDim)
								End Using
							End If
						End If
					Next i
					Return New Pair(Of Gradient, INDArray())(Nothing, [out])
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Average
					Dim outAverage(nInForwardPass - 1) As INDArray
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
						Dim i As Integer = 0
						Do While i < nInForwardPass
							If inputs(i).equalShapes(epsilon_Conflict) Then
								outAverage(i) = epsilon_Conflict.div(nInForwardPass)
							Else
								Dim bcDim() As Integer = Shape.getBroadcastDimensions(inputs(i).shape(), epsilon_Conflict.shape())
								outAverage(i) = epsilon_Conflict.div(nInForwardPass).sum(True, bcDim)
							End If
							i += 1
						Loop
					End Using
					Return New Pair(Of Gradient, INDArray())(Nothing, outAverage)
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Subtract
					Dim out2(1) As INDArray
					If Not broadcastCase Then
						out2(0) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict)
						out2(1) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict).negi()
					Else
						If inputs(0).equalShapes(epsilon_Conflict) Then
							'Second input is smaller/broadcast
							out2(0) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict)
							Dim bcDim() As Integer = Shape.getBroadcastDimensions(inputs(1).shape(), epsilon_Conflict.shape())
							Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
								out2(1) = epsilon_Conflict.sum(True, bcDim).negi()
							End Using
						Else
							'First input is smaller/broadcast
							Dim bcDim() As Integer = Shape.getBroadcastDimensions(inputs(0).shape(), epsilon_Conflict.shape())
							Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
								out2(0) = epsilon_Conflict.sum(True, bcDim)
							End Using
							out2(1) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict).negi()
						End If
					End If
					Return New Pair(Of Gradient, INDArray())(Nothing, out2)
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Product
					Dim out_product(nInForwardPass - 1) As INDArray
					Dim inBc() As INDArray = inputs
					If broadcastCase Then
						inBc = New INDArray(inputs.Length - 1){}
						For i As Integer = 0 To inputs.Length - 1
							If inputs(i).equalShapes(epsilon_Conflict) Then
								inBc(i) = inputs(i)
							Else
								inBc(i) = epsilon_Conflict.ulike()
								Nd4j.exec(New BroadcastTo(inputs(i), epsilon_Conflict.shape(), inBc(i)))
							End If
						Next i
					End If

					For i As Integer = 0 To nInForwardPass - 1
						out_product(i) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon_Conflict)
						For j As Integer = 0 To nInForwardPass - 1
							If i <> j Then
								out_product(i).muli(inBc(j))
							End If
						Next j

						If Not inputs(i).equalShapes(epsilon_Conflict) Then
							Dim bcDim() As Integer = Shape.getBroadcastDimensions(inputs(i).shape(), epsilon_Conflict.shape())
							Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
								out_product(i) = out_product(i).sum(True, bcDim)
							End Using
						End If
					Next i
					Return New Pair(Of Gradient, INDArray())(Nothing, out_product)
				Case org.deeplearning4j.nn.graph.vertex.impl.ElementWiseVertex.Op.Max
					Dim outMax(nInForwardPass - 1) As INDArray
					Dim maxIndices As INDArray = workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, DataType.INT, epsilon_Conflict.shape(), epsilon_Conflict.ordering())

					Dim bcIn() As INDArray = inputs
					If broadcastCase Then
						'Broadcast to right shape...
						bcIn = New INDArray(inputs.Length - 1){}
						For i As Integer = 0 To inputs.Length - 1
							If inputs(i).equalShapes(epsilon_Conflict) Then
								bcIn(i) = inputs(i)
							Else
								bcIn(i) = epsilon_Conflict.ulike()
								Nd4j.exec(New BroadcastTo(inputs(i), epsilon_Conflict.shape(), bcIn(i)))
							End If
						Next i
					End If

					Dim op As CustomOp = DynamicCustomOp.builder("mergemaxindex").addInputs(bcIn).addOutputs(maxIndices).callInplace(False).build()
					Nd4j.Executioner.exec(op)
					For i As Integer = 0 To nInForwardPass - 1
						'gradient is epsilon where the max index is the same as i and zero elsewhere
						outMax(i) = workspaceMgr.create(ArrayType.BP_WORKING_MEM, DataType.BOOL, maxIndices.shape()) 'workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, maxIndices);
						'generate a mask with 1s and 0s in the right places and muli with epsilon
						Dim nd4jop As New MatchConditionTransform(maxIndices, outMax(i), Conditions.equals(i))
						Nd4j.Executioner.exec(nd4jop)
						If broadcastCase AndAlso Not epsilon_Conflict.equalShapes(inputs(i)) Then
							'Broadcast  for ths input
							outMax(i) = outMax(i).castTo(epsilon_Conflict.dataType()).mul(epsilon_Conflict)
							Dim bcDim() As Integer = Shape.getBroadcastDimensions(inputs(i).shape(), epsilon_Conflict.shape())
							Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATION_GRAD)
								outMax(i) = outMax(i).sum(True, bcDim)
							End Using
						Else
							'Standard case
							outMax(i) = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, outMax(i).castTo(epsilon_Conflict.dataType()).muli(epsilon_Conflict))
						End If
					Next i
					Return New Pair(Of Gradient, INDArray())(Nothing, outMax)
				Case Else
					Throw New System.NotSupportedException("Unknown op: " & Me.op)
			End Select
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
				Dim ret As INDArray = Nd4j.createUninitialized(DataType.BOOL, maskArrays(0).shape()) 'maskArrays[0].dup(maskArrays[0].ordering());
				Nd4j.Executioner.exec(New [Or](maskArrays(0).castTo(DataType.BOOL), maskArrays(1).castTo(DataType.BOOL), ret))
				For i As Integer = 2 To maskArrays.Length - 1
					Nd4j.Executioner.exec(New [Or](maskArrays(i).castTo(DataType.BOOL), ret, ret))
				Next i
				Return New Pair(Of INDArray, MaskState)(ret.castTo(Nd4j.defaultFloatingPointType()), currentMaskState)
			End If
		End Function

		Public Overrides Function ToString() As String
			Return "ElementWiseVertex(id=" & Me.VertexIndex & ",name=""" & Me.VertexName & """,op=" & op & ")"
		End Function
	End Class

End Namespace