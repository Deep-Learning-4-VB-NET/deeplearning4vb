Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ListOrderedSet = org.apache.commons.collections4.set.ListOrderedSet
Imports org.deeplearning4j.nn.conf
Imports PreprocessorVertex = org.deeplearning4j.nn.conf.graph.PreprocessorVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports SameDiffLambdaLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasModelConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasModelConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasInput = org.deeplearning4j.nn.modelimport.keras.layers.KerasInput
Imports KerasLoss = org.deeplearning4j.nn.modelimport.keras.layers.KerasLoss
Imports KerasLambda = org.deeplearning4j.nn.modelimport.keras.layers.core.KerasLambda
Imports KerasLSTM = org.deeplearning4j.nn.modelimport.keras.layers.recurrent.KerasLSTM
Imports KerasRnnUtils = org.deeplearning4j.nn.modelimport.keras.layers.recurrent.KerasRnnUtils
Imports KerasSimpleRnn = org.deeplearning4j.nn.modelimport.keras.layers.recurrent.KerasSimpleRnn
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports KerasModelBuilder = org.deeplearning4j.nn.modelimport.keras.utils.KerasModelBuilder
Imports KerasModelUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasModelUtils
Imports KerasOptimizerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasOptimizerUtils
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports org.nd4j.autodiff.samediff.internal
Imports org.nd4j.autodiff.samediff.internal
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports NodeDef = org.tensorflow.framework.NodeDef
import static org.deeplearning4j.nn.modelimport.keras.KerasLayer.customLayers
import static org.deeplearning4j.nn.modelimport.keras.KerasLayer.lambdaLayers

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

Namespace org.deeplearning4j.nn.modelimport.keras


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data public class KerasModel
	Public Class KerasModel

		Protected Friend Shared config As New KerasModelConfiguration()
'JAVA TO VB CONVERTER NOTE: The field modelBuilder was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend modelBuilder_Conflict As New KerasModelBuilder(config)

		Protected Friend className As String ' Keras model class name
		Protected Friend enforceTrainingConfig As Boolean ' whether to build model in training mode
		Protected Friend layers As IDictionary(Of String, KerasLayer) ' map from layer name to KerasLayer
		Protected Friend layersOrdered As IList(Of KerasLayer) ' ordered list of layers
		Protected Friend outputTypes As IDictionary(Of String, InputType) ' inferred output types for all layers
		Protected Friend inputLayerNames As List(Of String) ' list of input layers
		Protected Friend outputLayerNames As List(Of String) ' list of output layers
		Protected Friend useTruncatedBPTT As Boolean = False ' whether to use truncated BPTT
		Protected Friend truncatedBPTT As Integer = 0 ' truncated BPTT value
		Protected Friend kerasMajorVersion As Integer
		Protected Friend kerasBackend As String
		Protected Friend dimOrder As KerasLayer.DimOrder = Nothing
		Protected Friend optimizer As IUpdater = Nothing

		Public Sub New()
		End Sub

		Public Overridable Function modelBuilder() As KerasModelBuilder
			Return Me.modelBuilder_Conflict
		End Function

		''' <summary>
		''' (Recommended) Builder-pattern constructor for (Functional API) Model.
		''' </summary>
		''' <param name="modelBuilder"> builder object </param>
		''' <exception cref="IOException">                            IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModel(org.deeplearning4j.nn.modelimport.keras.utils.KerasModelBuilder modelBuilder) throws UnsupportedKerasConfigurationException, IOException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Sub New(ByVal modelBuilder As KerasModelBuilder)
			Me.New(modelBuilder.getModelJson(), modelBuilder.getModelYaml(), modelBuilder.getWeightsArchive(), modelBuilder.getWeightsRoot(), modelBuilder.getTrainingJson(), modelBuilder.getTrainingArchive(), modelBuilder.isEnforceTrainingConfig(), modelBuilder.getInputShape(), modelBuilder.getDimOrder())
		End Sub

		''' <summary>
		''' (Not recommended) Constructor for (Functional API) Model from model configuration
		''' (JSON or YAML), training configuration (JSON), weights, and "training mode"
		''' boolean indicator. When built in training mode, certain unsupported configurations
		''' (e.g., unknown regularizers) will throw Exceptions. When enforceTrainingConfig=false, these
		''' will generate warnings but will be otherwise ignored.
		''' </summary>
		''' <param name="modelJson">             model configuration JSON string </param>
		''' <param name="modelYaml">             model configuration YAML string </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configurations </param>
		''' <exception cref="IOException">                            IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected KerasModel(String modelJson, String modelYaml, Hdf5Archive weightsArchive, String weightsRoot, String trainingJson, Hdf5Archive trainingArchive, boolean enforceTrainingConfig, int[] inputShape, KerasLayer.DimOrder dimOrder) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Protected Friend Sub New(ByVal modelJson As String, ByVal modelYaml As String, ByVal weightsArchive As Hdf5Archive, ByVal weightsRoot As String, ByVal trainingJson As String, ByVal trainingArchive As Hdf5Archive, ByVal enforceTrainingConfig As Boolean, ByVal inputShape() As Integer, ByVal dimOrder As KerasLayer.DimOrder)

			Dim modelConfig As IDictionary(Of String, Object) = KerasModelUtils.parseModelConfig(modelJson, modelYaml)
			Me.kerasMajorVersion = KerasModelUtils.determineKerasMajorVersion(modelConfig, config)
			Me.kerasBackend = KerasModelUtils.determineKerasBackend(modelConfig, config)
			Me.enforceTrainingConfig = enforceTrainingConfig
			Me.dimOrder = dimOrder

			' Determine model configuration type. 
			If Not modelConfig.ContainsKey(config.getFieldClassName()) Then
				Throw New InvalidKerasConfigurationException("Could not determine Keras model class (no " & config.getFieldClassName() & " field found)")
			End If
			Me.className = DirectCast(modelConfig(config.getFieldClassName()), String)
			If Not Me.className.Equals(config.getFieldClassNameModel()) AndAlso Not Me.className.Equals(config.getFieldNameClassFunctional()) Then
				Throw New InvalidKerasConfigurationException("Expected model class name " & config.getFieldClassNameModel() & " or " & config.getFieldNameClassFunctional() & " (found " & Me.className & ")")
			End If


			' Retrieve lists of input and output layers, layer configurations. 
			If Not modelConfig.ContainsKey(config.getModelFieldConfig()) Then
				Throw New InvalidKerasConfigurationException("Could not find model configuration details (no " & config.getModelFieldConfig() & " in model config)")
			End If
			Dim layerLists As IDictionary(Of String, Object) = DirectCast(modelConfig(config.getModelFieldConfig()), IDictionary(Of String, Object))


			' Construct list of input layers. 
			If Not layerLists.ContainsKey(config.getModelFieldInputLayers()) Then
				Throw New InvalidKerasConfigurationException("Could not find list of input layers (no " & config.getModelFieldInputLayers() & " field found)")
			End If
			Me.inputLayerNames = New List(Of String)()
			For Each inputLayerNameObj As Object In DirectCast(layerLists(config.getModelFieldInputLayers()), IList(Of Object))
				Me.inputLayerNames.Add(CStr(DirectCast(inputLayerNameObj, IList(Of Object))(0)))
			Next inputLayerNameObj

			' Construct list of output layers. 
			If Not layerLists.ContainsKey(config.getModelFieldOutputLayers()) Then
				Throw New InvalidKerasConfigurationException("Could not find list of output layers (no " & config.getModelFieldOutputLayers() & " field found)")
			End If
			Me.outputLayerNames = New List(Of String)()
			For Each outputLayerNameObj As Object In DirectCast(layerLists(config.getModelFieldOutputLayers()), IList(Of Object))
				Me.outputLayerNames.Add(CStr(DirectCast(outputLayerNameObj, IList(Of Object))(0)))
			Next outputLayerNameObj

			' Process layer configurations. 
			If Not layerLists.ContainsKey(config.getModelFieldLayers()) Then
				Throw New InvalidKerasConfigurationException("Could not find layer configurations (no " & (config.getModelFieldLayers() & " field found)"))
			End If
			Dim layerPair As Pair(Of IDictionary(Of String, KerasLayer), IList(Of KerasLayer)) = prepareLayers(DirectCast(layerLists((config.getModelFieldLayers())), IList(Of Object)))
			Me.layers = layerPair.First
			Me.layersOrdered = layerPair.Second

			' Import training configuration. 
			If enforceTrainingConfig Then
				If trainingJson IsNot Nothing Then
					importTrainingConfiguration(trainingJson)
				Else
					log.warn("If enforceTrainingConfig is true, a training " & "configuration object has to be provided. Usually the only practical way to do this is to store" & " your keras model with `model.save('model_path.h5')`. If you store model config and weights" & " separately no training configuration is attached.")
				End If
			End If

			If inputShape Is Nothing Then
				inputShape = layersOrdered(0).inputShape
			End If

			' Infer output types for each layer. 
			Me.outputTypes = inferOutputTypes(inputShape)

			' Store weights in layers. 
			If weightsArchive IsNot Nothing Then
				KerasModelUtils.importWeights(weightsArchive, weightsRoot, layers, kerasMajorVersion, kerasBackend)
			End If
		End Sub

		''' <summary>
		''' Helper method called from constructor. Converts layer configuration
		''' JSON into KerasLayer objects.
		''' </summary>
		''' <param name="layerConfigs"> List of Keras layer configurations </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: org.nd4j.common.primitives.Pair<Map<String, KerasLayer>, List<KerasLayer>> prepareLayers(List<Object> layerConfigs) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Friend Overridable Function prepareLayers(ByVal layerConfigs As IList(Of Object)) As Pair(Of IDictionary(Of String, KerasLayer), IList(Of KerasLayer))
			Dim layers As IDictionary(Of String, KerasLayer) = New Dictionary(Of String, KerasLayer)() ' map from layer name to KerasLayer
			Dim layersOrdered As IList(Of KerasLayer) = New List(Of KerasLayer)()

			For Each layerConfig As Object In layerConfigs
				Dim layerConfigMap As IDictionary(Of String, Object) = DirectCast(layerConfig, IDictionary(Of String, Object))
				' Append major keras version and backend to each layer config.
				layerConfigMap(config.getFieldKerasVersion()) = Me.kerasMajorVersion
				If kerasMajorVersion = 2 AndAlso Me.kerasBackend IsNot Nothing Then
					layerConfigMap(config.getFieldBackend()) = Me.kerasBackend
				End If

				Dim kerasLayerConf As KerasLayerConfiguration = (New KerasLayer(Me.kerasMajorVersion)).conf

				If dimOrder <> Nothing Then ' Force override of dim ordering with value from model builder
					Dim dimOrderString As String
					If dimOrder = KerasLayer.DimOrder.TENSORFLOW Then
						dimOrderString = kerasLayerConf.getDIM_ORDERING_TENSORFLOW()
					ElseIf dimOrder = KerasLayer.DimOrder.THEANO Then
						dimOrderString = kerasLayerConf.getDIM_ORDERING_THEANO()
					Else
						Throw New InvalidKerasConfigurationException("Invalid data format / dim ordering")
					End If
					layerConfigMap(kerasLayerConf.getLAYER_FIELD_DIM_ORDERING()) = dimOrderString
				End If


				Dim layer As KerasLayer = KerasLayerUtils.getKerasLayerFromConfig(layerConfigMap, Me.enforceTrainingConfig, kerasLayerConf, customLayers, lambdaLayers, layers)
				layersOrdered.Add(layer)
				layers(layer.LayerName) = layer
				If TypeOf layer Is KerasLSTM Then
					Me.useTruncatedBPTT = Me.useTruncatedBPTT OrElse DirectCast(layer, KerasLSTM).Unroll
				End If
				If TypeOf layer Is KerasSimpleRnn Then
					Me.useTruncatedBPTT = Me.useTruncatedBPTT OrElse DirectCast(layer, KerasSimpleRnn).Unroll
				End If
			Next layerConfig

			Dim names As IList(Of String) = New List(Of String)()
			'set of names of lambda nodes
			Dim lambdaNames As ISet(Of String) = New HashSet(Of String)()

			'node inputs by name for looking up which nodes to do replacements for (useful since indices of nodes can change)
			Dim nodesOutputToForLambdas As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))()
			For i As Integer = 0 To layers.Count - 1
				names.Add(layersOrdered(i).getLayerName())
				If TypeOf layersOrdered(i) Is KerasLambda Then
					lambdaNames.Add(layersOrdered(i).getLayerName())
				End If
			Next i

			Dim replacementNamesForLambda As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))()
			Dim updatedOrders As IDictionary(Of Integer, KerasLayer) = New Dictionary(Of Integer, KerasLayer)()
			For i As Integer = 0 To layersOrdered.Count - 1
'JAVA TO VB CONVERTER NOTE: The variable kerasLayer was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim kerasLayer_Conflict As KerasLayer = layers(names(i))
				Dim tempCopyNames As IList(Of String) = New List(Of String)(kerasLayer_Conflict.getInboundLayerNames())
				Dim removed As IList(Of String) = New List(Of String)()

				For Each input As String In tempCopyNames
					'found a lambda where an input occurs, record the index for input
					If lambdaNames.Contains(input) Then
						If Not nodesOutputToForLambdas.ContainsKey(input) Then
							nodesOutputToForLambdas(input) = New List(Of String)()
						End If

						nodesOutputToForLambdas(input).Add(kerasLayer_Conflict.LayerName)
					End If
					'potential loop found
					Dim indexOfInput As Integer = names.IndexOf(input)
					If indexOfInput > i Then
						Dim originalLambda As KerasLambda = DirectCast(kerasLayer_Conflict, KerasLambda)
						Dim configCopy As IDictionary(Of String, Object) = New Dictionary(Of String, Object)(kerasLayer_Conflict.originalLayerConfig)
						Dim newName As String = kerasLayer_Conflict.LayerName & "-" & input
						If Not replacementNamesForLambda.ContainsKey(originalLambda.layerName_Conflict) Then
							replacementNamesForLambda(originalLambda.layerName_Conflict) = New List(Of String)()
						End If
						configCopy(kerasLayer_Conflict.conf.getLAYER_FIELD_NAME()) = newName
						replacementNamesForLambda(originalLambda.layerName_Conflict).Add(newName)
						Dim sameDiffLambdaLayer As SameDiffLambdaLayer = DirectCast(originalLambda.SameDiffLayer.clone(), SameDiffLambdaLayer)
						sameDiffLambdaLayer.setLayerName(newName)
						Dim kerasLambda As New KerasLambda(configCopy,sameDiffLambdaLayer)
						kerasLambda.layerName_Conflict = newName
						kerasLambda.InboundLayerNames = New List(Of String)(java.util.Arrays.asList(input))
						layers(newName) = kerasLambda
						Dim indexOfNewLayer As Integer = names.IndexOf(input) + 1
						updatedOrders(indexOfNewLayer) = kerasLambda
						names.Insert(indexOfNewLayer,newName)
						removed.Add(input)
						Console.WriteLine("Found input " & input & " at keras node " & names(i) & " with potential cycle.")

					End If
				Next input

				kerasLayer_Conflict.getInboundLayerNames().RemoveAll(removed)
			Next i




			'update the list with all the new layers
			For Each newLayers As KeyValuePair(Of Integer, KerasLayer) In updatedOrders.SetOfKeyValuePairs()
				layersOrdered.Insert(newLayers.Key,newLayers.Value)
			Next newLayers

			Dim oldNames As IList(Of String) = New List(Of String)(names)

			names.Clear()
			'old names are used for checking distance from old nodes to new ones
			'node inputs by name for looking up which nodes to do replacements for (useful since indices of nodes can change)
			If replacementNamesForLambda.Count > 0 Then
				For Each replacementEntry As KeyValuePair(Of String, IList(Of String)) In replacementNamesForLambda.SetOfKeyValuePairs()
					Dim nodesToReplaceInputNamesWith As IList(Of String) = nodesOutputToForLambdas(replacementEntry.Key)
					Dim processed As ISet(Of String) = New HashSet(Of String)()
					For Each nodeName As String In nodesToReplaceInputNamesWith
'JAVA TO VB CONVERTER NOTE: The variable kerasLayer was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
						Dim kerasLayer_Conflict As KerasLayer = layers(nodeName)
						Dim shouldBeOriginal As Boolean = True
						If processed.Count > 0 Then
							For Each process As String In processed
								If kerasLayer_Conflict.getInboundLayerNames().Contains(process) Then
									shouldBeOriginal = False
									Exit For
								End If
							Next process
						End If

						Dim nearestNodes As IList(Of String) = findNearestNodesTo(replacementEntry.Key, nodeName, replacementEntry.Value, oldNames, 2)
						'if the original isn't in the closest top 2 nodes, then we shouldn't replace it
						If nodesToReplaceInputNamesWith.Count > 1 Then
							If Not nearestNodes.Contains(replacementEntry.Key) Then
								shouldBeOriginal = False
							End If
						End If

						'layers that contain an already processed
						'node as an input need modification
						If shouldBeOriginal Then
							processed.Add(nodeName)
							Continue For
						End If

						'replace whatever the final input name is that was last
						kerasLayer_Conflict.getInboundLayerNames()(kerasLayer_Conflict.getInboundLayerNames().IndexOf(replacementEntry.Key)) = nearestNodes(0)

						processed.Add(nodeName)


					Next nodeName
				Next replacementEntry
			End If


			layers.Clear()
'JAVA TO VB CONVERTER NOTE: The variable kerasLayer was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			For Each kerasLayer_Conflict As KerasLayer In layersOrdered
				layers(kerasLayer_Conflict.LayerName) = kerasLayer_Conflict
			Next kerasLayer_Conflict

			Return New Pair(Of IDictionary(Of String, KerasLayer), IList(Of KerasLayer))(layers, layersOrdered)
		End Function

		Friend Overridable Function findNearestNodesTo(ByVal original As String, ByVal target As String, ByVal targetedNodes As IList(Of String), ByVal topoSortNodes As IList(Of String), ByVal k As Integer) As IList(Of String)
			Dim idx As Integer = topoSortNodes.IndexOf(target)
			Dim rankByDistance As New Counter(Of String)()

			For i As Integer = 0 To targetedNodes.Count - 1
				Dim currIdx As Integer = topoSortNodes.IndexOf(targetedNodes(i))
				Dim diff As Integer = Math.Abs(currIdx - idx)
				'note we want the top k ranked by the least
				rankByDistance.incrementCount(targetedNodes(i),-diff)
			Next i

			Dim currIdx As Integer = topoSortNodes.IndexOf(original)
			Dim diff As Integer = Math.Abs(currIdx - idx)
			'note we want the top k ranked by the least
			rankByDistance.incrementCount(original,-diff)
			rankByDistance.keepTopNElements(k)
			Return rankByDistance.keySetSorted()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: Map<String, Object> getOptimizerConfig(Map<String, Object> trainingConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Friend Overridable Function getOptimizerConfig(ByVal trainingConfig As IDictionary(Of String, Object)) As IDictionary(Of String, Object)
			If Not trainingConfig.ContainsKey(config.getOptimizerConfig()) Then
				Throw New InvalidKerasConfigurationException("Field " & config.getOptimizerConfig() & " missing from layer config")
			End If
			Return DirectCast(trainingConfig(config.getOptimizerConfig()), IDictionary(Of String, Object))
		End Function

		''' <summary>
		''' Helper method called from constructor. Incorporate training configuration details into model.
		''' Includes loss function, optimization details, etc.
		''' </summary>
		''' <param name="trainingConfigJson"> JSON containing Keras training configuration </param>
		''' <exception cref="IOException">                            IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void importTrainingConfiguration(String trainingConfigJson) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Friend Overridable Sub importTrainingConfiguration(ByVal trainingConfigJson As String)
			Dim trainingConfig As IDictionary(Of String, Object) = KerasModelUtils.parseJsonString(trainingConfigJson)

			Dim optimizerConfig As IDictionary(Of String, Object) = getOptimizerConfig(trainingConfig)
			Me.optimizer = KerasOptimizerUtils.mapOptimizer(optimizerConfig)

			' Add loss layers for each loss function. 
			Dim lossLayers As IList(Of KerasLayer) = New List(Of KerasLayer)()
			If Not trainingConfig.ContainsKey(config.getTrainingLoss()) Then
				Throw New InvalidKerasConfigurationException("Could not determine training loss function (no " & config.getTrainingLoss() & " field found in training config)")
			End If
			Dim kerasLossObj As Object = trainingConfig(config.getTrainingLoss())

			If TypeOf kerasLossObj Is String Then
				Dim kerasLoss As String = DirectCast(kerasLossObj, String)
				For Each outputLayerName As String In Me.outputLayerNames
					lossLayers.Add(New KerasLoss(outputLayerName & "_loss", outputLayerName, kerasLoss))
				Next outputLayerName
			ElseIf TypeOf kerasLossObj Is System.Collections.IDictionary Then
				Dim kerasLossMap As IDictionary(Of String, Object) = DirectCast(kerasLossObj, IDictionary(Of String, Object))
				For Each outputLayerName As String In kerasLossMap.Keys
					Dim kerasLoss As Object = kerasLossMap(outputLayerName)
					If TypeOf kerasLoss Is String Then
						lossLayers.Add(New KerasLoss(outputLayerName & "_loss", outputLayerName, DirectCast(kerasLoss, String)))
					Else
						Throw New InvalidKerasConfigurationException("Unknown Keras loss " & kerasLoss.ToString())
					End If
				Next outputLayerName
			End If
			Me.outputLayerNames.Clear()

			' Add loss layers to output layer list and layer graph. 
			For Each lossLayer As KerasLayer In lossLayers
				Me.layersOrdered.Add(lossLayer)
				Me.layers(lossLayer.LayerName) = lossLayer
				Me.outputLayerNames.Add(lossLayer.LayerName)
			Next lossLayer
		End Sub

		''' <summary>
		''' Helper method called from constructor. Infers and records output type
		''' for every layer.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: Map<String, org.deeplearning4j.nn.conf.inputs.InputType> inferOutputTypes(int[] inputShape) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Friend Overridable Function inferOutputTypes(ByVal inputShape() As Integer) As IDictionary(Of String, InputType)
			Dim outputTypes As IDictionary(Of String, InputType) = New Dictionary(Of String, InputType)()
			Dim kerasLayerIdx As Integer = 0
			For Each layer As KerasLayer In Me.layersOrdered
				Dim outputType As InputType
				If TypeOf layer Is KerasInput Then
					If inputShape IsNot Nothing AndAlso layer.inputShape_Conflict Is Nothing Then
						layer.inputShape_Conflict = inputShape
					End If

					Dim kerasInput As KerasInput = DirectCast(layer, KerasInput)
					Dim layer1 As Layer = layersOrdered(kerasLayerIdx + 1).layer
					'no dim order, try to pull it from the next layer if there is one
					If ConvolutionUtils.layerHasConvolutionLayout(layer1) Then
						Dim formatForLayer As CNN2DFormat = ConvolutionUtils.getFormatForLayer(layer1)
						If formatForLayer = CNN2DFormat.NCHW Then
							dimOrder = KerasLayer.DimOrder.THEANO
						ElseIf formatForLayer = CNN2DFormat.NHWC Then
							dimOrder = KerasLayer.DimOrder.TENSORFLOW
						Else
							dimOrder = KerasLayer.DimOrder.NONE
						End If
					ElseIf KerasRnnUtils.isRnnLayer(layersOrdered(kerasLayerIdx + 1)) Then
						If kerasInput.inputShape_Conflict Is Nothing Then
							kerasInput.inputShape_Conflict = layersOrdered(kerasLayerIdx + 1).inputShape
						End If
					End If

					If dimOrder <> Nothing Then
						layer.DimOrder = dimOrder
					End If
					outputType = layer.getOutputType()
					Me.truncatedBPTT = DirectCast(layer, KerasInput).TruncatedBptt
				Else
					Dim inputTypes As IList(Of InputType) = New List(Of InputType)()
					Dim i As Integer = 0
					For Each inboundLayerName As String In layer.getInboundLayerNames()
						If outputTypes.ContainsKey(inboundLayerName) Then
							inputTypes.Add(outputTypes(inboundLayerName))
						End If
					Next inboundLayerName
					outputType = layer.getOutputType(CType(inputTypes, List(Of InputType)).ToArray())
				End If
				outputTypes(layer.LayerName) = outputType
				kerasLayerIdx += 1
			Next layer

			Return outputTypes
		End Function

		''' <summary>
		''' Configure a ComputationGraph from this Keras Model configuration.
		''' </summary>
		''' <returns> ComputationGraph </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public ComputationGraphConfiguration getComputationGraphConfiguration() throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable ReadOnly Property ComputationGraphConfiguration As ComputationGraphConfiguration
			Get
				If Not Me.className.Equals(config.getFieldClassNameModel()) AndAlso Not Me.className.Equals(config.getFieldClassNameSequential()) AndAlso Not Me.className.Equals(config.getFieldNameClassFunctional()) Then
					Throw New InvalidKerasConfigurationException("Keras model class name " & Me.className & " incompatible with ComputationGraph")
				End If
				Dim modelBuilder As New NeuralNetConfiguration.Builder()
    
				If optimizer IsNot Nothing Then
					modelBuilder.updater(optimizer)
				End If
    
				Dim outputs As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))()
				For Each layer As KerasLayer In Lists.reverse(Me.layersOrdered)
					For Each input As String In layer.getInboundLayerNames()
						If Not outputs.ContainsKey(input) Then
							outputs(input) = New List(Of String)()
						End If
    
						outputs(input).Add(layer.LayerName)
					Next input
				Next layer
    
				Dim graphBuilder As ComputationGraphConfiguration.GraphBuilder = modelBuilder.graphBuilder()
				' NOTE: normally this is disallowed in DL4J. However, in Keras you can create disconnected graph vertices.
				' The responsibility for doing this correctly is that of the Keras user.
				graphBuilder.allowDisconnected(True)
    
    
				' Build String array of input layer names, add to ComputationGraph. 
				Dim inputLayerNameArray(Me.inputLayerNames.Count - 1) As String
				Me.inputLayerNames.toArray(inputLayerNameArray)
				graphBuilder.addInputs(inputLayerNameArray)
    
				' Build InputType array of input layer types, add to ComputationGraph. 
				Dim inputTypeList As IList(Of InputType) = New List(Of InputType)()
				Dim initialInputTypes As IList(Of InputType) = New List(Of InputType)()
				For Each inputLayerName As String In Me.inputLayerNames
					Me.layers(inputLayerName)
					inputTypeList.Add(Me.layers(inputLayerName).getOutputType())
    
				Next inputLayerName
    
    
				' Build String array of output layer names, add to ComputationGraph. 
				Dim outputLayerNameArray(Me.outputLayerNames.Count - 1) As String
				Me.outputLayerNames.toArray(outputLayerNameArray)
				graphBuilder.Outputs = outputLayerNameArray
    
				Dim preprocessors As IDictionary(Of String, InputPreProcessor) = New Dictionary(Of String, InputPreProcessor)()
				Dim idx As Integer = 0
				' Add layersOrdered one at a time. 
				For Each layer As KerasLayer In Me.layersOrdered
					' Get inbound layer names. 
					Dim inboundLayerNames As IList(Of String) = layer.getInboundLayerNames()
					Dim inboundLayerNamesArray(inboundLayerNames.Count - 1) As String
					inboundLayerNames.toArray(inboundLayerNamesArray)
    
					Dim inboundTypeList As IList(Of InputType) = New List(Of InputType)()
    
					' Get inbound InputTypes and InputPreProcessor, if necessary. 
					If inboundLayerNames.Count > 0 Then
						Dim inputTypes2(inboundLayerNames.Count - 1) As InputType
						Dim inboundIdx As Integer = 0
						For Each layerName As String In inboundLayerNames
							Dim prevLayer As KerasLayer = layers(layerName)
							If prevLayer.InputPreProcessor Then
								Dim inputType As InputType = Me.outputTypes(layerName)
								Dim preprocessor As InputPreProcessor = prevLayer.getInputPreprocessor(inputType)
								KerasModelUtils.setDataFormatIfNeeded(preprocessor,layer)
								Dim outputType As InputType = preprocessor.getOutputType(inputType)
								inputTypes2(inboundIdx) = outputType
								inboundIdx += 1
							Else
								Dim inputType As InputType = Me.outputTypes(layerName)
								inputTypes2(inboundIdx) = inputType
								inboundIdx += 1
							End If
    
							If outputTypes.ContainsKey(layerName) Then
								inboundTypeList.Add(Me.outputTypes(layerName))
							End If
						Next layerName
    
					End If
    
					Dim inboundTypeArray(inboundTypeList.Count - 1) As InputType
					inboundTypeList.toArray(inboundTypeArray)
					Dim preprocessor As InputPreProcessor = layer.getInputPreprocessor(inboundTypeArray)
					'don't add pre processor if there isn't anymore output, edge case for final layer
					If idx = layersOrdered.Count - 1 Then
						preprocessor = Nothing
					End If
					If layer.isLayer() Then
						If preprocessor IsNot Nothing Then
							preprocessors(layer.LayerName) = preprocessor
						End If
						graphBuilder.addLayer(layer.LayerName, layer.Layer, inboundLayerNamesArray)
					ElseIf layer.Vertex Then ' Ignore "preprocessor" layers for now
						If preprocessor IsNot Nothing Then
							preprocessors(layer.LayerName) = preprocessor
						End If
						graphBuilder.addVertex(layer.LayerName, layer.Vertex, inboundLayerNamesArray)
					ElseIf layer.InputPreProcessor Then
						If preprocessor Is Nothing Then
							Throw New UnsupportedKerasConfigurationException("Layer " & layer.LayerName & " could not be mapped to Layer, Vertex, or InputPreProcessor")
						End If
						graphBuilder.addVertex(layer.LayerName, New PreprocessorVertex(preprocessor), inboundLayerNamesArray)
					End If
    
					If TypeOf layer Is KerasInput Then
						initialInputTypes.Add(Me.outputTypes(layer.layerName_Conflict))
					End If
    
					idx += 1
				Next layer
				graphBuilder.setInputPreProcessors(preprocessors)
    
				' Whether to use standard backprop (or BPTT) or truncated BPTT. 
				If Me.useTruncatedBPTT AndAlso Me.truncatedBPTT > 0 Then
					graphBuilder.backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(truncatedBPTT).tBPTTBackwardLength(truncatedBPTT)
				Else
					graphBuilder.backpropType(BackpropType.Standard)
				End If
    
				Dim build As ComputationGraphConfiguration = graphBuilder.build()
				'note we don't forcibly over ride inputs when doing keras import. They are already set.
				build.addPreProcessors(False,False,CType(initialInputTypes, List(Of InputType)).ToArray())
				Return build
			End Get
		End Property

		''' <summary>
		''' Build a ComputationGraph from this Keras Model configuration and import weights.
		''' </summary>
		''' <returns> ComputationGraph </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.graph.ComputationGraph getComputationGraph() throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable ReadOnly Property ComputationGraph As ComputationGraph
			Get
				Return getComputationGraph(True)
			End Get
		End Property

		''' <summary>
		''' Build a ComputationGraph from this Keras Model configuration and (optionally) import weights.
		''' </summary>
		''' <param name="importWeights"> whether to import weights </param>
		''' <returns> ComputationGraph </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.graph.ComputationGraph getComputationGraph(boolean importWeights) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function getComputationGraph(ByVal importWeights As Boolean) As ComputationGraph
			Dim model As New ComputationGraph(ComputationGraphConfiguration)
			model.init()
			If importWeights Then
				model = DirectCast(KerasModelUtils.copyWeightsToModel(model, Me.layers), ComputationGraph)
			End If
			Return model
		End Function
	End Class

End Namespace