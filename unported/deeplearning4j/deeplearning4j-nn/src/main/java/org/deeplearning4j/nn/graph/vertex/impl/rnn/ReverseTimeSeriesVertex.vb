Imports System
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseGraphVertex = org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
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
	Public Class ReverseTimeSeriesVertex
		Inherits BaseGraphVertex

		Private ReadOnly inputName As String
		Private ReadOnly inputIdx As Integer

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputName As String, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, Nothing, Nothing, dataType)
			Me.inputName = inputName


			If inputName Is Nothing Then
				' Don't use masks
				Me.inputIdx = -1
			Else
				' Find the given input
				Me.inputIdx = graph.Configuration.getNetworkInputs().IndexOf(inputName)
				If inputIdx = -1 Then
					Throw New System.ArgumentException("Invalid input name: """ & inputName & """ not found in list " & "of network inputs (" & graph.Configuration.getNetworkInputs() & ")")
				End If
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
			' Get the mask arrays for the given input, if any
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray mask = getMask();
			Dim mask As INDArray = Me.Mask

			' Store the input
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = inputs[0];
			Dim input As INDArray = inputs(0)

			' Compute the output
			Return revertTimeSeries(input, mask, workspaceMgr, ArrayType.ACTIVATIONS)
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())

			' Get the mask arrays for the given input, if any
			Dim mask As INDArray = Me.Mask

			' Backpropagate the output error (epsilon) to the input variables:
			'      Just undo the revert (which can be done by another revert)
			Dim epsilonsOut As INDArray = revertTimeSeries(epsilon_Conflict, mask, workspaceMgr, ArrayType.ACTIVATION_GRAD)

			Return New Pair(Of Gradient, INDArray())(Nothing, New INDArray() {epsilonsOut})
		End Function

		''' <summary>
		''' Gets the current mask array from the provided input </summary>
		''' <returns> The mask or null, if no input was provided </returns>
		Private ReadOnly Property Mask As INDArray
			Get
				' If no input is provided, no mask is used and null is returned
				If inputIdx < 0 Then
					Return Nothing
				End If
    
	'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
	'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[] inputMaskArrays = graph.getInputMaskArrays();
				Dim inputMaskArrays() As INDArray = graph.InputMaskArrays
				Return (If(inputMaskArrays IsNot Nothing, inputMaskArrays(inputIdx), Nothing))
			End Get
		End Property

		''' <summary>
		''' Reverts the element order of a tensor along the 3rd axis (time series axis).
		''' A masking tensor is used to restrict the revert to meaningful elements and keep the padding in place.
		''' 
		''' This method is self-inverse in the following sense:
		''' {@code revertTensor( revertTensor (input, mask), mask )}
		''' equals
		''' {@code input} </summary>
		''' <param name="input"> The input tensor </param>
		''' <param name="mask"> The masking tensor (1 for meaningful entries, 0 for padding) </param>
		''' <returns> The reverted mask. </returns>
		Private Shared Function revertTimeSeries(ByVal input As INDArray, ByVal mask As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal type As ArrayType) As INDArray
			' Get number of samples
			Dim n As val = input.size(0)

			' Get maximal length of a time series
			Dim m As val = input.size(2)

			' Create empty output
			Dim [out] As INDArray = workspaceMgr.create(type, input.dataType(), input.shape(), "f"c)

			' Iterate over all samples
			For s As Integer = 0 To n - 1
				Dim t1 As Long = 0 ' Original time step
				Dim t2 As Long = m - 1 ' Destination time step

				' Revert Sample: Copy from origin (t1) to destination (t2)
				Do While t1 < m AndAlso t2 >= 0

					' If mask is set: ignore padding
					If mask IsNot Nothing Then
						' Origin: find next time step
						Do While t1 < m AndAlso mask.getDouble(s, t1) = 0
							t1 += 1
						Loop
						' Destination: find next time step
						Do While t2 >= 0 AndAlso mask.getDouble(s, t2) = 0
							t2 -= 1
						Loop
					End If

					' Get the feature vector for the given sample and origin time step
					' The vector contains features (forward pass) or errors (backward pass)
					Dim vec As INDArray = input.get(NDArrayIndex.point(s), NDArrayIndex.all(), NDArrayIndex.point(t1))

					' Put the feature vector to the given destination in the output
					[out].put(New INDArrayIndex(){ NDArrayIndex.point(s), NDArrayIndex.all(), NDArrayIndex.point(t2) }, vec)

					' Move on
					t1 += 1
					t2 -= 1
				Loop
			Next s

			' Return the output
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
			If maskArrays.Length > 1 Then
				Throw New System.ArgumentException("This vertex can only handle one input and hence only one mask")
			End If

			' The mask does not change.
			Return New Pair(Of INDArray, MaskState)(maskArrays(0), currentMaskState)
		End Function

		Public Overrides Function ToString() As String
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String paramStr = (inputName == null) ? "" : "inputName=" + inputName;
			Dim paramStr As String = If(inputName Is Nothing, "", "inputName=" & inputName)
			Return "ReverseTimeSeriesVertex(" & paramStr & ")"
		End Function


	End Class

End Namespace