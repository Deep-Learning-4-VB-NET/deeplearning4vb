Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports RecurrentLayer = org.deeplearning4j.nn.api.layers.RecurrentLayer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseGraphVertex = org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports org.deeplearning4j.nn.layers
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports FrozenLayerWithBackprop = org.deeplearning4j.nn.layers.FrozenLayerWithBackprop
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class LayerVertex extends org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
	<Serializable>
	Public Class LayerVertex
		Inherits BaseGraphVertex

'JAVA TO VB CONVERTER NOTE: The field layer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private layer_Conflict As Layer
		Private ReadOnly layerPreProcessor As InputPreProcessor
		Private setLayerInput As Boolean

		''' <summary>
		''' Create a network input vertex:
		''' </summary>
		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal layer As Layer, ByVal layerPreProcessor As InputPreProcessor, ByVal outputVertex As Boolean, ByVal dataType As DataType)
			Me.New(graph, name, vertexIndex, Nothing, Nothing, layer, layerPreProcessor, outputVertex, dataType)
		End Sub

		Public Sub New(ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal inputVertices() As VertexIndices, ByVal outputVertices() As VertexIndices, ByVal layer As Layer, ByVal layerPreProcessor As InputPreProcessor, ByVal outputVertex As Boolean, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, inputVertices, outputVertices, dataType)
			Me.graph = graph
			Me.vertexName_Conflict = name
			Me.vertexIndex_Conflict = vertexIndex
			Me.inputVertices_Conflict = inputVertices
			Me.outputVertices_Conflict = outputVertices
			Me.layer_Conflict = layer
			Me.layerPreProcessor = layerPreProcessor
			Me.outputVertex = outputVertex

			Me.inputs = New INDArray((If(inputVertices IsNot Nothing, inputVertices.Length, 0)) - 1){}
		End Sub

		Public Overrides Function hasLayer() As Boolean
			Return True
		End Function

		Public Overrides Sub setLayerAsFrozen()
			If TypeOf Me.layer_Conflict Is FrozenLayer Then
				Return
			End If

			Me.layer_Conflict = New FrozenLayer(Me.layer_Conflict)
			Me.layer_Conflict.conf().getLayer().setLayerName(vertexName_Conflict)
		End Sub

		Public Overrides Function paramTable(ByVal backpropOnly As Boolean) As IDictionary(Of String, INDArray)
			Return layer_Conflict.paramTable(backpropOnly)
		End Function

		Public Overrides ReadOnly Property OutputVertex As Boolean
			Get
				Return outputVertex OrElse TypeOf layer_Conflict Is BaseOutputLayer
			End Get
		End Property

		Public Overrides ReadOnly Property Layer As Layer
			Get
				Return layer_Conflict
			End Get
		End Property

		Public Overrides Function doForward(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If Not canDoForward() Then
				Throw New System.InvalidOperationException("Cannot do forward pass: all inputs not set")
			End If
			Return layer_Conflict.activate(training, workspaceMgr)
		End Function

		Public Overridable Sub applyPreprocessorAndSetInput(ByVal workspaceMgr As LayerWorkspaceMgr)
			'Apply preprocessor
			Dim currInput As INDArray = inputs(0)
			If layerPreProcessor IsNot Nothing Then
				currInput = layerPreProcessor.preProcess(currInput, graph.batchSize(), workspaceMgr)
			End If
			layer_Conflict.setInput(currInput, workspaceMgr)
			setLayerInput = True
		End Sub

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			If Not canDoBackward() Then
				If inputs Is Nothing OrElse inputs(0) Is Nothing Then
					Throw New System.InvalidOperationException("Cannot do backward pass: inputs not set. Layer: """ & vertexName_Conflict & """ (idx " & vertexIndex_Conflict & "), numInputs: " & NumInputArrays)
				Else
					Throw New System.InvalidOperationException("Cannot do backward pass: all epsilons not set. Layer """ & vertexName_Conflict & """ (idx " & vertexIndex_Conflict & "), numInputs :" & NumInputArrays & "; numOutputs: " & NumOutputConnections)
				End If
			End If

			'Edge case: output layer - never did forward pass hence layer.setInput was never called...
			If Not setLayerInput Then
				applyPreprocessorAndSetInput(workspaceMgr)
			End If

			Dim pair As Pair(Of Gradient, INDArray)
			If tbptt AndAlso TypeOf layer_Conflict Is RecurrentLayer Then
				'Truncated BPTT for recurrent layers
				pair = DirectCast(layer_Conflict, RecurrentLayer).tbpttBackpropGradient(epsilon_Conflict, graph.Configuration.getTbpttBackLength(), workspaceMgr)
			Else
				'Normal backprop
				pair = layer_Conflict.backpropGradient(epsilon_Conflict, workspaceMgr) 'epsTotal may be null for OutputLayers
			End If

			If layerPreProcessor IsNot Nothing Then
				Dim eps As INDArray = pair.Second
				eps = layerPreProcessor.backprop(eps, graph.batchSize(), workspaceMgr)
				pair.Second = eps
			End If

			'Layers always have single activations input -> always have single epsilon output during backprop
			Return New Pair(Of Gradient, INDArray())(pair.First, New INDArray() {pair.Second})
		End Function

		Public Overrides Sub setInput(ByVal inputNumber As Integer, ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			If inputNumber > 0 Then
				Throw New System.ArgumentException("Invalid input number: LayerVertex instances have only 1 input (got inputNumber = " & inputNumber & ")")
			End If
			inputs(inputNumber) = input
			setLayerInput = False
			applyPreprocessorAndSetInput(workspaceMgr)
		End Sub

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				layer_Conflict.BackpropGradientsViewArray = backpropGradientsViewArray
			End Set
		End Property

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			If maskArrays Is Nothing OrElse maskArrays.Length = 0 Then
				Return New Pair(Of INDArray, MaskState)(Nothing, currentMaskState)
			End If

			If layerPreProcessor IsNot Nothing Then
				Dim pair As Pair(Of INDArray, MaskState) = layerPreProcessor.feedForwardMaskArray(maskArrays(0), currentMaskState, minibatchSize)
				If pair Is Nothing Then
					maskArrays(0) = Nothing
					currentMaskState = Nothing
				Else
					maskArrays(0) = pair.First
					currentMaskState = pair.Second
				End If
			End If

			Return layer_Conflict.feedForwardMaskArray(maskArrays(0), currentMaskState, minibatchSize)
		End Function


		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("LayerVertex(id=").Append(vertexIndex_Conflict).Append(",name=""").Append(vertexName_Conflict).Append(""",inputs=").Append(Arrays.toString(inputVertices_Conflict)).Append(",outputs=").Append(Arrays.toString(outputVertices_Conflict)).Append(")")
			Return sb.ToString()
		End Function

		Public Overrides Function canDoBackward() As Boolean
			If Not OutputVertex Then
				'inputs to frozen layer go unchecked, so could be null
				If TypeOf Layer Is FrozenLayer Then
					Return True
				Else
					Return MyBase.canDoBackward()
				End If
			End If

			For Each input As INDArray In inputs
				If input Is Nothing Then
					Return False
				End If
			Next input

			Dim resolvedLayer As Layer = layer_Conflict
			If TypeOf layer_Conflict Is FrozenLayerWithBackprop Then
				resolvedLayer = DirectCast(layer_Conflict, FrozenLayerWithBackprop).InsideLayer
			End If

			If Not (TypeOf resolvedLayer Is IOutputLayer) Then
				If epsilon_Conflict Is Nothing Then
					Return False
				End If
			End If

			Return True
		End Function

		Public Overridable Function computeScore(ByVal r As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double
			If Not (TypeOf layer_Conflict Is IOutputLayer) Then
				Throw New System.NotSupportedException("Cannot compute score: layer is not an output layer (layer class: " & layer_Conflict.GetType().Name)
			End If
			'Edge case: output layer - never did forward pass hence layer.setInput was never called...
			If Not setLayerInput Then
				applyPreprocessorAndSetInput(LayerWorkspaceMgr.noWorkspaces()) 'TODO
			End If

			Dim ol As IOutputLayer = DirectCast(layer_Conflict, IOutputLayer)
			Return ol.computeScore(r, training, workspaceMgr)
		End Function

		Public Overridable Function computeScoreForExamples(ByVal r As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If Not (TypeOf layer_Conflict Is IOutputLayer) Then
				Throw New System.NotSupportedException("Cannot compute score: layer is not an output layer (layer class: " & layer_Conflict.GetType().Name)
			End If
			'Edge case: output layer - never did forward pass hence layer.setInput was never called...
			If Not setLayerInput Then
				applyPreprocessorAndSetInput(workspaceMgr)
			End If

			Dim ol As IOutputLayer = DirectCast(layer_Conflict, IOutputLayer)
			Return ol.computeScoreForExamples(r, workspaceMgr)
		End Function

		Public Overrides ReadOnly Property Config As TrainingConfig
			Get
				Return Layer.Config
			End Get
		End Property

		Public Overrides Function params() As INDArray
			Return layer_Conflict.params()
		End Function

		Public Overrides ReadOnly Property GradientsViewArray As INDArray
			Get
				Return layer_Conflict.GradientsViewArray
			End Get
		End Property
	End Class


End Namespace