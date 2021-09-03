Imports System.Collections
Imports System.Collections.Generic
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.nn.transferlearning


	Public Class TransferLearningHelper

		Private isGraph As Boolean = True
		Private applyFrozen As Boolean = False
		Private origGraph As ComputationGraph
		Private origMLN As MultiLayerNetwork
		Private frozenTill As Integer
		Private frozenOutputAt() As String
		Private unFrozenSubsetGraph As ComputationGraph
		Private unFrozenSubsetMLN As MultiLayerNetwork
		Friend frozenInputVertices As ISet(Of String) = New HashSet(Of String)() 'name map so no problem
		Friend graphInputs As IList(Of String)
		Friend frozenInputLayer As Integer = 0

		''' <summary>
		''' Will modify the given comp graph (in place!) to freeze vertices from input to the vertex specified.
		''' </summary>
		''' <param name="orig">           Comp graph </param>
		''' <param name="frozenOutputAt"> vertex to freeze at (hold params constant during training) </param>
		Public Sub New(ByVal orig As ComputationGraph, ParamArray ByVal frozenOutputAt() As String)
			origGraph = orig
			Me.frozenOutputAt = frozenOutputAt
			applyFrozen = True
			initHelperGraph()
		End Sub

		''' <summary>
		''' Expects a computation graph where some vertices are frozen
		''' </summary>
		''' <param name="orig"> </param>
		Public Sub New(ByVal orig As ComputationGraph)
			origGraph = orig
			initHelperGraph()
		End Sub

		''' <summary>
		''' Will modify the given MLN (in place!) to freeze layers (hold params constant during training) specified and below
		''' </summary>
		''' <param name="orig">       MLN to freeze </param>
		''' <param name="frozenTill"> integer indicating the index of the layer and below to freeze </param>
		Public Sub New(ByVal orig As MultiLayerNetwork, ByVal frozenTill As Integer)
			isGraph = False
			Me.frozenTill = frozenTill
			applyFrozen = True
			origMLN = orig
			initHelperMLN()
		End Sub

		''' <summary>
		''' Expects a MLN where some layers are frozen
		''' </summary>
		''' <param name="orig"> </param>
		Public Sub New(ByVal orig As MultiLayerNetwork)
			isGraph = False
			origMLN = orig
			initHelperMLN()
		End Sub

		Public Overridable Sub errorIfGraphIfMLN()
			If isGraph Then
				Throw New System.ArgumentException("This instance was initialized with a computation graph. Cannot apply methods related to MLN")
			Else
				Throw New System.ArgumentException("This instance was initialized with a MultiLayerNetwork. Cannot apply methods related to computation graphs")
			End If

		End Sub

		''' <summary>
		''' Returns the unfrozen subset of the original computation graph as a computation graph
		''' Note that with each call to featurizedFit the parameters to the original computation graph are also updated
		''' </summary>
		Public Overridable Function unfrozenGraph() As ComputationGraph
			If Not isGraph Then
				errorIfGraphIfMLN()
			End If
			Return unFrozenSubsetGraph
		End Function

		''' <summary>
		''' Returns the unfrozen layers of the MultiLayerNetwork as a multilayernetwork
		''' Note that with each call to featurizedFit the parameters to the original MLN are also updated
		''' </summary>
		Public Overridable Function unfrozenMLN() As MultiLayerNetwork
			If isGraph Then
				errorIfGraphIfMLN()
			End If
			Return unFrozenSubsetMLN
		End Function

		''' <summary>
		''' Use to get the output from a featurized input
		''' </summary>
		''' <param name="input"> featurized data </param>
		''' <returns> output </returns>
		Public Overridable Function outputFromFeaturized(ByVal input() As INDArray) As INDArray()
			If Not isGraph Then
				errorIfGraphIfMLN()
			End If
			Return unFrozenSubsetGraph.output(input)
		End Function

		''' <summary>
		''' Use to get the output from a featurized input
		''' </summary>
		''' <param name="input"> featurized data </param>
		''' <returns> output </returns>
		Public Overridable Function outputFromFeaturized(ByVal input As INDArray) As INDArray
			If isGraph Then
				If unFrozenSubsetGraph.NumOutputArrays > 1 Then
					Throw New System.ArgumentException("Graph has more than one output. Expecting an input array with outputFromFeaturized method call")
				End If
				Return unFrozenSubsetGraph.output(input)(0)
			Else
				Return unFrozenSubsetMLN.output(input)
			End If
		End Function

		''' <summary>
		''' Runs through the comp graph and saves off a new model that is simply the "unfrozen" part of the origModel
		''' This "unfrozen" model is then used for training with featurized data
		''' </summary>
		Private Sub initHelperGraph()

			Dim backPropOrder() As Integer = CType(origGraph.topologicalSortOrder().Clone(), Integer())
			ArrayUtils.reverse(backPropOrder)

			Dim allFrozen As ISet(Of String) = New HashSet(Of String)()
			If applyFrozen Then
				Collections.addAll(allFrozen, frozenOutputAt)
			End If
			For i As Integer = 0 To backPropOrder.Length - 1
				Dim gv As GraphVertex = origGraph.Vertices(backPropOrder(i))
				If applyFrozen AndAlso allFrozen.Contains(gv.VertexName) Then
					If gv.hasLayer() Then
						'Need to freeze this layer
						Dim l As org.deeplearning4j.nn.api.Layer = gv.Layer
						gv.setLayerAsFrozen()

						'We also need to place the layer in the CompGraph Layer[] (replacing the old one)
						'This could no doubt be done more efficiently
						Dim layers() As org.deeplearning4j.nn.api.Layer = origGraph.Layers
						For j As Integer = 0 To layers.Length - 1
							If layers(j) Is l Then
								layers(j) = gv.Layer 'Place the new frozen layer to replace the original layer
								Exit For
							End If
						Next j
					End If

					'Also: mark any inputs as to be frozen also
					Dim inputs() As VertexIndices = gv.InputVertices
					If inputs IsNot Nothing AndAlso inputs.Length > 0 Then
						For j As Integer = 0 To inputs.Length - 1
							Dim inputVertexIdx As Integer = inputs(j).VertexIndex
							Dim alsoFreeze As String = origGraph.Vertices(inputVertexIdx).VertexName
							allFrozen.Add(alsoFreeze)
						Next j
					End If
				Else
					If gv.hasLayer() Then
						If TypeOf gv.Layer Is FrozenLayer Then
							allFrozen.Add(gv.VertexName)
							'also need to add parents to list of allFrozen
							Dim inputs() As VertexIndices = gv.InputVertices
							If inputs IsNot Nothing AndAlso inputs.Length > 0 Then
								For j As Integer = 0 To inputs.Length - 1
									Dim inputVertexIdx As Integer = inputs(j).VertexIndex
									Dim alsoFrozen As String = origGraph.Vertices(inputVertexIdx).VertexName
									allFrozen.Add(alsoFrozen)
								Next j
							End If
						End If
					End If
				End If
			Next i
			For i As Integer = 0 To backPropOrder.Length - 1
				Dim gv As GraphVertex = origGraph.Vertices(backPropOrder(i))
				Dim gvName As String = gv.VertexName
				'is it an unfrozen vertex that has an input vertex that is frozen?
				If Not allFrozen.Contains(gvName) AndAlso Not gv.InputVertex Then
					Dim inputs() As VertexIndices = gv.InputVertices
					For j As Integer = 0 To inputs.Length - 1
						Dim inputVertexIdx As Integer = inputs(j).VertexIndex
						Dim inputVertex As String = origGraph.Vertices(inputVertexIdx).VertexName
						If allFrozen.Contains(inputVertex) Then
							frozenInputVertices.Add(inputVertex)
						End If
					Next j
				End If
			Next i

			Dim builder As New TransferLearning.GraphBuilder(origGraph)
			For Each toRemove As String In allFrozen
				If frozenInputVertices.Contains(toRemove) Then
					builder.removeVertexKeepConnections(toRemove)
				Else
					builder.removeVertexAndConnections(toRemove)
				End If
			Next toRemove

			Dim frozenInputVerticesSorted As ISet(Of String) = New HashSet(Of String)()
			frozenInputVerticesSorted.addAll(origGraph.Configuration.getNetworkInputs())
			frozenInputVerticesSorted.RemoveAll(allFrozen)
			'remove input vertices - just to add back in a predictable order
			For Each existingInput As String In frozenInputVerticesSorted
				builder.removeVertexKeepConnections(existingInput)
			Next existingInput
			frozenInputVerticesSorted.addAll(frozenInputVertices)
			'Sort all inputs to the computation graph - in order to have a predictable order
			graphInputs = New ArrayList(frozenInputVerticesSorted)
			graphInputs.Sort()
			For Each asInput As String In frozenInputVerticesSorted
				'add back in the right order
				builder.addInputs(asInput)
			Next asInput
			unFrozenSubsetGraph = builder.build()
			copyOrigParamsToSubsetGraph()
			'unFrozenSubsetGraph.setListeners(origGraph.getListeners());

			If frozenInputVertices.Count = 0 Then
				Throw New System.ArgumentException("No frozen layers found")
			End If

		End Sub

		Private Sub initHelperMLN()
			If applyFrozen Then
				Dim layers() As org.deeplearning4j.nn.api.Layer = origMLN.Layers
				For i As Integer = frozenTill To 0 Step -1
					'unchecked?
					layers(i) = New FrozenLayer(layers(i))
				Next i
				origMLN.Layers = layers
			End If
			Dim i As Integer = 0
			Do While i < origMLN.getnLayers()
				If TypeOf origMLN.getLayer(i) Is FrozenLayer Then
					frozenInputLayer = i
				End If
				i += 1
			Loop
			Dim allConfs As IList(Of NeuralNetConfiguration) = New List(Of NeuralNetConfiguration)()
			i = frozenInputLayer + 1
			Do While i < origMLN.getnLayers()
				allConfs.Add(origMLN.getLayer(i).conf())
				i += 1
			Loop

			Dim c As MultiLayerConfiguration = origMLN.LayerWiseConfigurations

			unFrozenSubsetMLN = New MultiLayerNetwork((New MultiLayerConfiguration.Builder()).inputPreProcessors(c.getInputPreProcessors()).backpropType(c.getBackpropType()).tBPTTForwardLength(c.getTbpttFwdLength()).tBPTTBackwardLength(c.getTbpttBackLength()).confs(allConfs).dataType(origMLN.LayerWiseConfigurations.getDataType()).build())
			unFrozenSubsetMLN.init()
			'copy over params
			i = frozenInputLayer + 1
			Do While i < origMLN.getnLayers()
				unFrozenSubsetMLN.getLayer(i - frozenInputLayer - 1).Params = origMLN.getLayer(i).params()
				i += 1
			Loop
			'unFrozenSubsetMLN.setListeners(origMLN.getListeners());
		End Sub

		''' <summary>
		''' During training frozen vertices/layers can be treated as "featurizing" the input
		''' The forward pass through these frozen layer/vertices can be done in advance and the dataset saved to disk to iterate
		''' quickly on the smaller unfrozen part of the model
		''' Currently does not support datasets with feature masks
		''' </summary>
		''' <param name="input"> multidataset to feed into the computation graph with frozen layer vertices </param>
		''' <returns> a multidataset with input features that are the outputs of the frozen layer vertices and the original labels. </returns>
		Public Overridable Function featurize(ByVal input As MultiDataSet) As MultiDataSet
			If Not isGraph Then
				Throw New System.ArgumentException("Cannot use multidatasets with MultiLayerNetworks.")
			End If
			Dim labels() As INDArray = input.Labels
			Dim features() As INDArray = input.Features
			If input.FeaturesMaskArrays IsNot Nothing Then
				Throw New System.ArgumentException("Currently cannot support featurizing datasets with feature masks")
			End If
			Dim featureMasks() As INDArray = Nothing
			Dim labelMasks() As INDArray = input.LabelsMaskArrays

			Dim featuresNow(graphInputs.Count - 1) As INDArray
			Dim activationsNow As IDictionary(Of String, INDArray) = origGraph.feedForward(features, False)
			For i As Integer = 0 To graphInputs.Count - 1
				Dim anInput As String = graphInputs(i)
				If origGraph.getVertex(anInput).InputVertex Then
					'was an original input to the graph
					Dim inputIndex As Integer = origGraph.Configuration.getNetworkInputs().IndexOf(anInput)
					featuresNow(i) = origGraph.getInput(inputIndex)
				Else
					'needs to be grabbed from the internal activations
					featuresNow(i) = activationsNow(anInput)
				End If
			Next i

			Return New MultiDataSet(featuresNow, labels, featureMasks, labelMasks)
		End Function

		''' <summary>
		''' During training frozen vertices/layers can be treated as "featurizing" the input
		''' The forward pass through these frozen layer/vertices can be done in advance and the dataset saved to disk to iterate
		''' quickly on the smaller unfrozen part of the model
		''' Currently does not support datasets with feature masks
		''' </summary>
		''' <param name="input"> multidataset to feed into the computation graph with frozen layer vertices </param>
		''' <returns> a multidataset with input features that are the outputs of the frozen layer vertices and the original labels. </returns>
		Public Overridable Function featurize(ByVal input As DataSet) As DataSet
			If isGraph Then
				'trying to featurize for a computation graph
				If origGraph.NumInputArrays > 1 OrElse origGraph.NumOutputArrays > 1 Then
					Throw New System.ArgumentException("Input or output size to a computation graph is greater than one. Requires use of a MultiDataSet.")
				Else
					If input.FeaturesMaskArray IsNot Nothing Then
						Throw New System.ArgumentException("Currently cannot support featurizing datasets with feature masks")
					End If
					Dim inbW As New MultiDataSet(New INDArray() {input.Features}, New INDArray() {input.Labels}, Nothing, New INDArray() {input.LabelsMaskArray})
					Dim ret As MultiDataSet = featurize(inbW)
					Return New DataSet(ret.Features(0), input.Labels, ret.LabelsMaskArrays(0), input.LabelsMaskArray)
				End If
			Else
				If input.FeaturesMaskArray IsNot Nothing Then
					Throw New System.NotSupportedException("Feature masks not supported with featurizing currently")
				End If
				Return New DataSet(origMLN.feedForwardToLayer(frozenInputLayer + 1, input.Features, False)(frozenInputLayer + 1), input.Labels, Nothing, input.LabelsMaskArray)
			End If
		End Function

		''' <summary>
		''' Fit from a featurized dataset.
		''' The fit is conducted on an internally instantiated subset model that is representative of the unfrozen part of the original model.
		''' After each call on fit the parameters for the original model are updated
		''' </summary>
		''' <param name="iter"> </param>
		Public Overridable Sub fitFeaturized(ByVal iter As MultiDataSetIterator)
			unFrozenSubsetGraph.fit(iter)
			copyParamsFromSubsetGraphToOrig()
		End Sub

		Public Overridable Sub fitFeaturized(ByVal input As MultiDataSet)
			unFrozenSubsetGraph.fit(input)
			copyParamsFromSubsetGraphToOrig()
		End Sub

		Public Overridable Sub fitFeaturized(ByVal input As DataSet)
			If isGraph Then
				unFrozenSubsetGraph.fit(input)
				copyParamsFromSubsetGraphToOrig()
			Else
				unFrozenSubsetMLN.fit(input)
				copyParamsFromSubsetMLNToOrig()
			End If
		End Sub

		Public Overridable Sub fitFeaturized(ByVal iter As DataSetIterator)
			If isGraph Then
				unFrozenSubsetGraph.fit(iter)
				copyParamsFromSubsetGraphToOrig()
			Else
				unFrozenSubsetMLN.fit(iter)
				copyParamsFromSubsetMLNToOrig()
			End If
		End Sub

		Private Sub copyParamsFromSubsetGraphToOrig()
			For Each aVertex As GraphVertex In unFrozenSubsetGraph.Vertices
				If Not aVertex.hasLayer() Then
					Continue For
				End If
				origGraph.getVertex(aVertex.VertexName).Layer.Params = aVertex.Layer.params()
			Next aVertex
		End Sub

		Private Sub copyOrigParamsToSubsetGraph()
			For Each aVertex As GraphVertex In unFrozenSubsetGraph.Vertices
				If Not aVertex.hasLayer() Then
					Continue For
				End If
				aVertex.Layer.Params = origGraph.getLayer(aVertex.VertexName).params()
			Next aVertex
		End Sub

		Private Sub copyParamsFromSubsetMLNToOrig()
			Dim i As Integer = frozenInputLayer + 1
			Do While i < origMLN.getnLayers()
				origMLN.getLayer(i).Params = unFrozenSubsetMLN.getLayer(i - frozenInputLayer - 1).params()
				i += 1
			Loop
		End Sub

	End Class

End Namespace