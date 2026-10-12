Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports lombok
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports GlobalPoolingLayer = org.deeplearning4j.nn.conf.layers.GlobalPoolingLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports SameDiffVertex = org.deeplearning4j.nn.conf.layers.samediff.SameDiffVertex
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports NetworkMemoryReport = org.deeplearning4j.nn.conf.memory.NetworkMemoryReport
Imports JsonMappers = org.deeplearning4j.nn.conf.serde.JsonMappers
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports OutputLayerUtil = org.deeplearning4j.util.OutputLayerUtil
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports InvalidTypeIdException = org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.nn.conf


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"trainingWorkspaceMode", "inferenceWorkspaceMode", "cacheMode", "topologicalOrder", "topologicalOrderStr"}) @AllArgsConstructor(access = AccessLevel.@PRIVATE) @NoArgsConstructor public class ComputationGraphConfiguration implements java.io.Serializable, Cloneable
	<Serializable>
	Public Class ComputationGraphConfiguration
		Implements ICloneable

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(ComputationGraphConfiguration))

		Protected Friend vertices As IDictionary(Of String, GraphVertex) = New LinkedHashMap(Of String, GraphVertex)()
		Protected Friend vertexInputs As IDictionary(Of String, IList(Of String)) = New LinkedHashMap(Of String, IList(Of String))()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected WorkspaceMode trainingWorkspaceMode = WorkspaceMode.ENABLED;
		Protected Friend trainingWorkspaceMode As WorkspaceMode = WorkspaceMode.ENABLED

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected WorkspaceMode inferenceWorkspaceMode = WorkspaceMode.ENABLED;
		Protected Friend inferenceWorkspaceMode As WorkspaceMode = WorkspaceMode.ENABLED

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected CacheMode cacheMode;
		Protected Friend cacheMode As CacheMode

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected org.nd4j.linalg.api.buffer.DataType dataType = org.nd4j.linalg.api.buffer.DataType.FLOAT;
		Protected Friend dataType As DataType = DataType.FLOAT 'Default to float for 1.0.0-beta3 and earlier nets

		Protected Friend validateOutputLayerConfig As Boolean = True 'Default for 1.0.0-beta3 and earlier nets

		''' <summary>
		''' List of inputs to the network, by name
		''' </summary>
		Protected Friend networkInputs As IList(Of String)

		''' <summary>
		''' List of network outputs, by name
		''' </summary>
		Protected Friend networkOutputs As IList(Of String)
		Protected Friend backpropType As BackpropType = BackpropType.Standard
		Protected Friend tbpttFwdLength As Integer = 20
		Protected Friend tbpttBackLength As Integer = 20

		Protected Friend defaultConfiguration As NeuralNetConfiguration

		'Counter for the number of parameter updates so far
		' This is important for learning rate schedules, for example, and is stored here to ensure it is persisted
		' for Spark and model serialization
		Protected Friend iterationCount As Integer = 0

		'Counter for the number of epochs completed so far. Used for per-epoch schedules
		Protected Friend epochCount As Integer = 0

		Protected Friend topologicalOrder() As Integer
		Protected Friend topologicalOrderStr As IList(Of String)

		''' <returns> YAML representation of configuration </returns>
		Public Overridable Function toYaml() As String
			Dim mapper As ObjectMapper = NeuralNetConfiguration.mapperYaml()
			SyncLock mapper
				Try
					Return mapper.writeValueAsString(Me)
				Catch e As org.nd4j.shade.jackson.core.JsonProcessingException
					Throw New Exception(e)
				End Try
			End SyncLock
		End Function

		''' <summary>
		''' Create a neural net configuration from YAML
		''' </summary>
		''' <param name="json"> the neural net configuration from YAML </param>
		''' <returns> <seealso cref="ComputationGraphConfiguration"/> </returns>
		Public Shared Function fromYaml(ByVal json As String) As ComputationGraphConfiguration
			Dim mapper As ObjectMapper = NeuralNetConfiguration.mapperYaml()
			Try
				Return mapper.readValue(json, GetType(ComputationGraphConfiguration))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <returns> JSON representation of computation graph configuration </returns>
		Public Overridable Function toJson() As String
			'As per MultiLayerConfiguration.toJson()
			Dim mapper As ObjectMapper = NeuralNetConfiguration.mapper()
			SyncLock mapper
				'JSON mappers are supposed to be thread safe: however, in practice they seem to miss fields occasionally
				'when writeValueAsString is used by multiple threads. This results in invalid JSON. See issue #3243
				Try
					Return mapper.writeValueAsString(Me)
				Catch e As org.nd4j.shade.jackson.core.JsonProcessingException
					Throw New Exception(e)
				End Try
			End SyncLock
		End Function

		''' <summary>
		''' Create a computation graph configuration from json
		''' </summary>
		''' <param name="json"> the neural net configuration from json </param>
		''' <returns> <seealso cref="ComputationGraphConfiguration"/> </returns>
		Public Shared Function fromJson(ByVal json As String) As ComputationGraphConfiguration
			'As per MultiLayerConfiguration.fromJson()
			Dim mapper As ObjectMapper = NeuralNetConfiguration.mapper()
			Dim conf As ComputationGraphConfiguration
			Try
				conf = mapper.readValue(json, GetType(ComputationGraphConfiguration))
			Catch e As InvalidTypeIdException
				If e.Message.contains("@class") Then
					Try
						'JSON may be legacy (1.0.0-alpha or earlier), attempt to load it using old format
						Return JsonMappers.LegacyMapper.readValue(json, GetType(ComputationGraphConfiguration))
					Catch e2 As InvalidTypeIdException
						'Check for legacy custom layers: "Could not resolve type id 'CustomLayer' as a subtype of [simple type, class org.deeplearning4j.nn.conf.layers.Layer]: known type ids = [Bidirectional, CenterLossOutputLayer, CnnLossLayer, ..."
						'1.0.0-beta5: dropping support for custom layers defined in pre-1.0.0-beta format. Built-in layers from these formats still work
						Dim msg As String = e2.Message
						If msg IsNot Nothing AndAlso msg.Contains("Could not resolve type id") Then
							Throw New Exception("Error deserializing ComputationGraphConfiguration - configuration may have a custom " & "layer, vertex or preprocessor, in pre version 1.0.0-beta JSON format." & vbLf & "Models in legacy format with custom" & " layers should be loaded in 1.0.0-beta to 1.0.0-beta4 and saved again, before loading in the current version of DL4J", e)
						End If
						Throw New Exception(e2)
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try
				End If
				Throw New Exception(e)
			Catch e As Exception
				'Check if this exception came from legacy deserializer...
				Dim msg As String = e.Message
				If msg IsNot Nothing AndAlso msg.Contains("legacy") Then
					Throw New Exception("Error deserializing ComputationGraphConfiguration - configuration may have a custom " & "layer, vertex or preprocessor, in pre version 1.0.0-alpha JSON format. These layers can be " & "deserialized by first registering them with NeuralNetConfiguration.registerLegacyCustomClassesForJSON(Class...)", e)
				End If
				Throw New Exception(e)
			End Try

			'To maintain backward compatibility after activation function refactoring (configs generated with v0.7.1 or earlier)
			' Previously: enumeration used for activation functions. Now: use classes
			Dim layerCount As Integer = 0
			Dim vertexMap As IDictionary(Of String, GraphVertex) = conf.getVertices()
			Dim vertices As JsonNode = Nothing
			For Each entry As KeyValuePair(Of String, GraphVertex) In vertexMap.SetOfKeyValuePairs()
				If Not (TypeOf entry.Value Is LayerVertex) Then
					Continue For
				End If

				Dim lv As LayerVertex = CType(entry.Value, LayerVertex)
				If lv.getLayerConf() IsNot Nothing AndAlso lv.getLayerConf().getLayer() IsNot Nothing Then
					Dim layer As Layer = lv.getLayerConf().getLayer()

					If TypeOf layer Is BaseLayer AndAlso DirectCast(layer, BaseLayer).getActivationFn() Is Nothing Then
						Dim layerName As String = layer.LayerName

						Try
							If vertices Is Nothing Then
								Dim jsonNode As JsonNode = mapper.readTree(json)
								vertices = jsonNode.get("vertices")
							End If

							Dim vertexNode As JsonNode = vertices.get(layerName)
							Dim layerVertexNode As JsonNode = vertexNode.get("LayerVertex")
							If layerVertexNode Is Nothing OrElse Not layerVertexNode.has("layerConf") OrElse Not layerVertexNode.get("layerConf").has("layer") Then
								Continue For
							End If
							Dim layerWrapperNode As JsonNode = layerVertexNode.get("layerConf").get("layer")

							If layerWrapperNode Is Nothing OrElse layerWrapperNode.size() <> 1 Then
								Continue For
							End If

							Dim layerNode As JsonNode = layerWrapperNode.elements().next()
							Dim activationFunction As JsonNode = layerNode.get("activationFunction") 'Should only have 1 element: "dense", "output", etc

							If activationFunction IsNot Nothing Then
								Dim ia As IActivation = Activation.fromString(activationFunction.asText()).getActivationFunction()
								DirectCast(layer, BaseLayer).setActivationFn(ia)
							End If

						Catch e As IOException
							log.warn("Layer with null ActivationFn field or pre-0.7.2 activation function detected: could not parse JSON", e)
						End Try
					End If

					handleLegacyWeightInitFromJson(json, layer, mapper, vertices)
				End If
			Next entry

			Return conf
		End Function

		''' <summary>
		''' Handle <seealso cref="WeightInit"/> and <seealso cref="Distribution"/> from legacy configs in Json format. Copied from handling of <seealso cref="Activation"/>
		''' above. </summary>
		''' <returns> True if all is well and layer iteration shall continue. False else-wise. </returns>
		Private Shared Sub handleLegacyWeightInitFromJson(ByVal json As String, ByVal layer As Layer, ByVal mapper As ObjectMapper, ByVal vertices As JsonNode)
			If TypeOf layer Is BaseLayer AndAlso DirectCast(layer, BaseLayer).getWeightInitFn() Is Nothing Then
				Dim layerName As String = layer.LayerName

				Try
					If vertices Is Nothing Then
						Dim jsonNode As JsonNode = mapper.readTree(json)
						vertices = jsonNode.get("vertices")
					End If

					Dim vertexNode As JsonNode = vertices.get(layerName)
					Dim layerVertexNode As JsonNode = vertexNode.get("LayerVertex")
					If layerVertexNode Is Nothing OrElse Not layerVertexNode.has("layerConf") OrElse Not layerVertexNode.get("layerConf").has("layer") Then
						Return
					End If
					Dim layerWrapperNode As JsonNode = layerVertexNode.get("layerConf").get("layer")

					If layerWrapperNode Is Nothing OrElse layerWrapperNode.size() <> 1 Then
						Return
					End If

					Dim layerNode As JsonNode = layerWrapperNode.elements().next()
					Dim weightInit As JsonNode = layerNode.get("weightInit") 'Should only have 1 element: "dense", "output", etc
					Dim distribution As JsonNode = layerNode.get("dist")

					Dim dist As Distribution = Nothing
					If distribution IsNot Nothing Then
						dist = mapper.treeToValue(distribution, GetType(Distribution))
					End If

					If weightInit IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.weights.IWeightInit wi = org.deeplearning4j.nn.weights.WeightInit.valueOf(weightInit.asText()).getWeightInitFunction(dist);
						Dim wi As IWeightInit = WeightInit.valueOf(weightInit.asText()).getWeightInitFunction(dist)
						DirectCast(layer, BaseLayer).setWeightInitFn(wi)
					End If

				Catch e As IOException
					log.warn("Layer with null ActivationFn field or pre-0.7.2 activation function detected: could not parse JSON", e)
				End Try
			End If
		End Sub

		Public Overrides Function ToString() As String
			Return toJson()
		End Function

		Public Overrides Function clone() As ComputationGraphConfiguration
			Dim conf As New ComputationGraphConfiguration()

			conf.vertices = New LinkedHashMap(Of String, GraphVertex)()
			For Each entry As KeyValuePair(Of String, GraphVertex) In Me.vertices.SetOfKeyValuePairs()
				conf.vertices(entry.Key) = entry.Value.clone()
			Next entry

			conf.vertexInputs = New LinkedHashMap(Of String, IList(Of String))()
			For Each entry As KeyValuePair(Of String, IList(Of String)) In Me.vertexInputs.SetOfKeyValuePairs()
				conf.vertexInputs(entry.Key) = New List(Of String)(entry.Value)
			Next entry

			conf.networkInputs = New List(Of String)()

			conf.networkInputs = New List(Of String)(Me.networkInputs)
			conf.networkOutputs = New List(Of String)(Me.networkOutputs)

			conf.backpropType = backpropType
			conf.tbpttFwdLength = tbpttFwdLength
			conf.tbpttBackLength = tbpttBackLength
			conf.defaultConfiguration = defaultConfiguration.clone()
			conf.trainingWorkspaceMode = trainingWorkspaceMode
			conf.inferenceWorkspaceMode = inferenceWorkspaceMode
			conf.cacheMode = Me.cacheMode
			conf.defaultConfiguration.cacheMode = Me.cacheMode
			conf.validateOutputLayerConfig = Me.validateOutputLayerConfig
			conf.dataType = Me.dataType

			Return conf
		End Function


		''' <summary>
		''' Check the configuration, make sure it is valid
		''' </summary>
		''' <exception cref="IllegalStateException"> if configuration is not valid </exception>
		Public Overridable Sub validate()
			validate(False, False)
		End Sub

		''' <summary>
		''' Check the configuration, make sure it is valid
		''' </summary>
		''' <param name="allowDisconnected"> If true: don't throw an exception on vertices that are 'disconnected'. A disconnected
		'''                          vertex is one that is not an output, and doesn't connect to any other vertices. i.e.,
		'''                          it's output activations don't go anywhere </param>
		''' <exception cref="IllegalStateException"> if configuration is not valid </exception>
		Public Overridable Sub validate(ByVal allowDisconnected As Boolean, ByVal allowNoOutput As Boolean)

			If networkInputs Is Nothing OrElse networkInputs.Count = 0 Then
				Throw New System.InvalidOperationException("Invalid configuration: network has no inputs. " & "Use .addInputs(String...) to label (and give an ordering to) the network inputs")
			End If
			If (networkOutputs Is Nothing OrElse networkOutputs.Count = 0) AndAlso Not allowNoOutput Then
				Throw New System.InvalidOperationException("Invalid configuration: network has no outputs. " & "Use .setOutput(String...) to specify (and give an ordering to) the output vertices, " & "or use allowNoOutputs(true) to disable this check")
			End If

			'Check uniqueness of names for inputs, layers, GraphNodes
			For Each s As String In networkInputs
				If vertices.ContainsKey(s) Then
					Throw New System.InvalidOperationException("Invalid configuration: name """ & s & """ is present in both network inputs and graph vertices/layers")
				End If
			Next s

			'Check: each layer & node has at least one input
			For Each e As KeyValuePair(Of String, IList(Of String)) In vertexInputs.SetOfKeyValuePairs()
				Dim nodeName As String = e.Key
				If e.Value Is Nothing OrElse e.Value.isEmpty() Then
					Throw New System.InvalidOperationException("Invalid configuration: vertex """ & nodeName & """ has no inputs")
				End If
				For Each inputName As String In e.Value
					If Not vertices.ContainsKey(inputName) AndAlso Not networkInputs.Contains(inputName) Then
						Throw New System.InvalidOperationException("Invalid configuration: Vertex """ & nodeName & """ has input """ & inputName & """ that does not exist")
					End If
				Next inputName
			Next e

			'Check output names:
			If networkOutputs IsNot Nothing Then
				For Each s As String In networkOutputs
					If Not vertices.ContainsKey(s) Then
						Throw New System.InvalidOperationException("Invalid configuration: Output name """ & s & """ is not a valid vertex")
					End If
				Next s
			End If

			'Check that there aren't any disconnected vertices
			If Not allowDisconnected Then
				'A vertex is considered disconnected if it is (a) not an output vertex, and (b) isn't used an as input
				' to another layer

				Dim seenAsInput As ISet(Of String) = New HashSet(Of String)()
				seenAsInput.addAll(networkOutputs)
				For Each e As KeyValuePair(Of String, IList(Of String)) In vertexInputs.SetOfKeyValuePairs()
					seenAsInput.addAll(e.Value)
				Next e

				Dim disconnected As ISet(Of String) = New HashSet(Of String)()
				disconnected.addAll(networkInputs)
				disconnected.addAll(vertices.Keys)
				disconnected.RemoveAll(seenAsInput)
				If disconnected.Count > 0 AndAlso Not allowNoOutput Then 'If allowing no output: by definition we have disconnected vertices
					Throw New System.InvalidOperationException("Invalid configuration: disconnected vertices found - " & disconnected & ". Disconnected vertices are those that do not connect to either another vertex, and are also" & " not a network output. This vertex can be set as an output using setOutputs(String...). " & "To disable this error (i.e., allow network configurations with" & " disconnected vertices) use GraphBuilder.allowDisconnected(true)")
				End If
			End If

			'Check for no graph cycles: done in ComputationGraph.init()
		End Sub

		''' <summary>
		''' Add preprocessors automatically, given the specified types of inputs for the network. Inputs are specified using the
		''' <seealso cref="InputType"/> class, in the same order in which the inputs were defined in the original configuration.<br>
		''' For example, in a network with two inputs: a convolutional input (28x28x1 images) and feed forward inputs, use
		''' {@code .addPreProcessors(InputType.convolutional(28,28,1),InputType.feedForward())}.<br>
		''' For the CNN->Dense and CNN->RNN transitions, the nIns on the Dense/RNN layers will also be added automatically.
		''' <b>NOTE</b>: This method will be called automatically when using the
		''' <seealso cref="GraphBuilder.setInputTypes(InputType...)"/> functionality.
		''' See that method for details.
		''' </summary>
		Public Overridable Sub addPreProcessors(ParamArray ByVal inputTypes() As InputType)
			getLayerActivationTypes(True, inputTypes)
		End Sub

		''' <summary>
		''' Add preprocessors automatically, given the specified types of inputs for the network. Inputs are specified using the
		''' <seealso cref="InputType"/> class, in the same order in which the inputs were defined in the original configuration.<br>
		''' For example, in a network with two inputs: a convolutional input (28x28x1 images) and feed forward inputs, use
		''' {@code .addPreProcessors(InputType.convolutional(28,28,1),InputType.feedForward())}.<br>
		''' For the CNN->Dense and CNN->RNN transitions, the nIns on the Dense/RNN layers will also be added automatically.
		''' <b>NOTE</b>: This method will be called automatically when using the
		''' <seealso cref="GraphBuilder.setInputTypes(InputType...)"/> functionality.
		''' See that method for details. </summary>
		''' <param name="forceOverrideInputs">  whether to forcibly over ride inputs or not
		'''                             when setting up pre processing </param>
		''' <param name="inputTypes">  the input types to set </param>
		Public Overridable Sub addPreProcessors(ByVal addPreprocIfNecessary As Boolean, ByVal forceOverrideInputs As Boolean, ParamArray ByVal inputTypes() As InputType)
			getLayerActivationTypes(addPreprocIfNecessary,forceOverrideInputs, inputTypes)
		End Sub

		''' <summary>
		''' Add preprocessors automatically, given the specified types of inputs for the network. Inputs are specified using the
		''' <seealso cref="InputType"/> class, in the same order in which the inputs were defined in the original configuration.<br>
		''' For example, in a network with two inputs: a convolutional input (28x28x1 images) and feed forward inputs, use
		''' {@code .addPreProcessors(InputType.convolutional(28,28,1),InputType.feedForward())}.<br>
		''' For the CNN->Dense and CNN->RNN transitions, the nIns on the Dense/RNN layers will also be added automatically.
		''' <b>NOTE</b>: This method will be called automatically when using the
		''' <seealso cref="GraphBuilder.setInputTypes(InputType...)"/> functionality.
		''' See that method for details. </summary>
		''' <param name="forceOverrideInputs">  whether to forcibly over ride inputs or not
		'''                             when setting up pre processing </param>
		''' <param name="inputTypes">  the input types to set </param>
		Public Overridable Sub addPreProcessors(ByVal forceOverrideInputs As Boolean, ParamArray ByVal inputTypes() As InputType)
			getLayerActivationTypes(True,forceOverrideInputs, inputTypes)
		End Sub



		''' <summary>
		''' For the given input shape/type for the network, return a map of activation sizes for each layer and vertex
		''' in the graph. Note that this method will automatically add preprocessors if required, to handle (for example)
		''' the transition between CNN and dense layers. </summary>
		''' <param name="inputTypes">                Input types for the network </param>
		''' <returns> A map of activation types for the graph (key: vertex name. value: type of activations out of that vertex) </returns>
		Public Overridable Function getLayerActivationTypes(ParamArray ByVal inputTypes() As InputType) As IDictionary(Of String, InputType)
			Return getLayerActivationTypes(True, inputTypes)
		End Function

		''' <summary>
		''' For the given input shape/type for the network, return a map of activation sizes for each layer and vertex
		''' in the graph. Note that this method can also add preprocessors if required (to handle transitions between some
		''' layer types such as convolutional -> dense, for example) </summary>
		''' <param name="addPreprocIfNecessary">     If true: add any required preprocessors, in the process of calculating the layer
		'''                                  activation sizes </param>
		''' <param name="overrideInputs">            whether to forcibly over ride inputs when
		'''                                  setting inputs </param>
		''' <param name="inputTypes">                Input types for the network </param>
		''' <returns> A map of activation types for the graph (key: vertex name. value: type of activations out of that vertex) </returns>
		Public Overridable Function getLayerActivationTypes(ByVal addPreprocIfNecessary As Boolean, ByVal overrideInputs As Boolean, ParamArray ByVal inputTypes() As InputType) As IDictionary(Of String, InputType)

			If inputTypes Is Nothing OrElse inputTypes.Length <> networkInputs.Count Then
				Throw New System.ArgumentException("Invalid number of InputTypes: cannot add preprocessors if number of InputType " & "objects differs from number of network inputs")
			End If

			'Now: need to do essentially a forward pass through the network, to work out what type of preprocessors to add
			'To do this: need to know what the output types are for each GraphVertex.

			'Do topological sort
			Dim topologicalOrdering As IList(Of String) = Me.topologicalOrdering()

			'Now, given the topological sort: do equivalent of forward pass
			Dim vertexOutputs As IDictionary(Of String, InputType) = New LinkedHashMap(Of String, InputType)()
			Dim currLayerIdx As Integer = -1
			For Each s As String In topologicalOrdering
				Dim inputIdx As Integer = networkInputs.IndexOf(s)
				If inputIdx <> -1 Then
					vertexOutputs(s) = inputTypes(inputIdx)
					Continue For
				End If

				Dim gv As GraphVertex = vertices(s)

				Dim inputTypeList As IList(Of InputType) = New List(Of InputType)()

				If TypeOf gv Is LayerVertex Then
					'Add preprocessor, if necessary:
					Dim [in] As String = vertexInputs(s)(0)
					Dim layerInput As InputType = vertexOutputs([in])
					inputTypeList.Add(layerInput)

					Dim lv As LayerVertex = DirectCast(gv, LayerVertex)
					Dim l As Layer = lv.getLayerConf().getLayer()

					'Preprocessors - add if necessary
					If lv.PreProcessor Is Nothing Then
						'But don't override preprocessors that are manually defined; if none has been defined,
						'add the appropriate preprocessor for this input type/layer combination
						Dim preproc As InputPreProcessor = l.getPreProcessorForInputType(layerInput)
						lv.setPreProcessor(preproc)
					End If

					'Set nIn value for layer (if not already set)
					Dim afterPreproc As InputType = layerInput
					If lv.PreProcessor IsNot Nothing AndAlso addPreprocIfNecessary Then
						Dim ip As InputPreProcessor = lv.PreProcessor
						afterPreproc = ip.getOutputType(layerInput)
					End If

					l.setNIn(afterPreproc, overrideInputs)

					currLayerIdx += 1
				Else
					Dim inputs As IList(Of String) = vertexInputs(s)
					If inputs IsNot Nothing Then
						For Each inputVertexName As String In inputs
							inputTypeList.Add(vertexOutputs(inputVertexName))
						Next inputVertexName
					End If
				End If

				Dim outputFromVertex As InputType = gv.getOutputType(currLayerIdx, CType(inputTypeList, List(Of InputType)).ToArray())
				vertexOutputs(s) = outputFromVertex
			Next s

			Return vertexOutputs
		End Function

		''' <summary>
		''' For the given input shape/type for the network, return a map of activation sizes for each layer and vertex
		''' in the graph. Note that this method can also add preprocessors if required (to handle transitions between some
		''' layer types such as convolutional -> dense, for example) </summary>
		''' <param name="addPreprocIfNecessary">     If true: add any required preprocessors, in the process of calculating the layer
		'''                                  activation sizes </param>
		''' <param name="inputTypes">                Input types for the network </param>
		''' <returns> A map of activation types for the graph (key: vertex name. value: type of activations out of that vertex) </returns>
		Public Overridable Function getLayerActivationTypes(ByVal addPreprocIfNecessary As Boolean, ParamArray ByVal inputTypes() As InputType) As IDictionary(Of String, InputType)
			Return getLayerActivationTypes(addPreprocIfNecessary,True,inputTypes)
		End Function

		Private Function verticesOutputTo() As IDictionary(Of String, IList(Of String))
'JAVA TO VB CONVERTER NOTE: The local variable verticesOutputTo was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim verticesOutputTo_Conflict As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))() 'Key: vertex. Values: vertices that this node is an input for
			For Each entry As KeyValuePair(Of String, GraphVertex) In vertices.SetOfKeyValuePairs()
				Dim vertexName As String = entry.Key
				Dim vertexInputNames As IList(Of String)
				vertexInputNames = vertexInputs(vertexName)

				If vertexInputNames Is Nothing Then
					Continue For
				End If

				'Build reverse network structure:
				For Each s As String In vertexInputNames
					Dim list As IList(Of String) = verticesOutputTo(s)
					If list Is Nothing Then
						list = New List(Of String)()
						verticesOutputTo(s) = list
					End If
					list.Add(vertexName) 'Edge: s -> vertexName
				Next s
			Next entry

			Return verticesOutputTo_Conflict
		End Function

		Private Function topologicalOrdering() As IList(Of String)
			'First step: build network in reverse order (i.e., define map of a -> list(b) instead of list(a) -> b)
			Dim verticesOutputTo As IDictionary(Of String, IList(Of String)) = Me.verticesOutputTo()
			Dim noIncomingEdges As New LinkedList(Of String)(networkInputs) 'Set of all nodes with no incoming edges
'JAVA TO VB CONVERTER NOTE: The local variable topologicalOrdering was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim topologicalOrdering_Conflict As IList(Of String) = New List(Of String)()

			Dim inputEdges As IDictionary(Of String, ISet(Of String)) = New Dictionary(Of String, ISet(Of String))()
			For Each entry As KeyValuePair(Of String, IList(Of String)) In vertexInputs.SetOfKeyValuePairs()
				inputEdges(entry.Key) = New HashSet(Of String)(entry.Value)
			Next entry

			Do While noIncomingEdges.Count > 0
				Dim [next] As String = noIncomingEdges.RemoveFirst()
				topologicalOrdering_Conflict.Add([next])

				'Remove edges next -> vertexOuputsTo[...] from graph;
				Dim nextEdges As IList(Of String) = verticesOutputTo([next])

				If nextEdges IsNot Nothing AndAlso nextEdges.Count > 0 Then
					For Each s As String In nextEdges
						Dim set As ISet(Of String) = inputEdges(s)
						set.remove([next])
						If set.Count = 0 Then
							noIncomingEdges.AddLast(s) 'No remaining edges for vertex i -> add to list for processing
						End If
					Next s
				End If
			Loop

			'If any edges remain in the graph: graph has cycles:
			For Each entry As KeyValuePair(Of String, ISet(Of String)) In inputEdges.SetOfKeyValuePairs()
				Dim set As ISet(Of String) = entry.Value
				If set Is Nothing Then
					Continue For
				End If
				If set.Count > 0 Then
					Throw New System.InvalidOperationException("Invalid configuration: cycle detected in graph. Cannot calculate topological ordering with graph cycle (" & "cycle includes vertex """ & entry.Key & """)")
				End If
			Next entry

			Return topologicalOrdering_Conflict
		End Function

		''' <summary>
		''' Get a <seealso cref="MemoryReport"/> for the given computation graph configuration. This is used to estimate the
		''' memory requirements for the given network configuration and input
		''' </summary>
		''' <param name="inputTypes"> Input types for the network </param>
		''' <returns> Memory report for the network </returns>
		Public Overridable Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As NetworkMemoryReport


			Dim memoryReportMap As IDictionary(Of String, MemoryReport) = New LinkedHashMap(Of String, MemoryReport)()
			Dim topologicalOrdering As IList(Of String) = Me.topologicalOrdering()

			Dim vertexOutputs As IDictionary(Of String, InputType) = New Dictionary(Of String, InputType)()
			Dim currLayerIdx As Integer = -1
			For Each s As String In topologicalOrdering
				Dim inputIdx As Integer = networkInputs.IndexOf(s)
				If inputIdx <> -1 Then
					vertexOutputs(s) = inputTypes(inputIdx)
					Continue For
				End If

				Dim gv As GraphVertex = vertices(s)

				Dim inputTypeList As IList(Of InputType) = New List(Of InputType)()

				If TypeOf gv Is LayerVertex Then
					'Add preprocessor, if necessary:
					Dim [in] As String = vertexInputs(s)(0)
					Dim layerInput As InputType = vertexOutputs([in])
					inputTypeList.Add(layerInput)
					currLayerIdx += 1
				Else
					Dim inputs As IList(Of String) = vertexInputs(s)
					If inputs IsNot Nothing Then
						For Each inputVertexName As String In inputs
							inputTypeList.Add(vertexOutputs(inputVertexName))
						Next inputVertexName
					End If
				End If



				Dim outputFromVertex As InputType = gv.getOutputType(currLayerIdx, CType(inputTypeList, List(Of InputType)).ToArray())
				vertexOutputs(s) = outputFromVertex

				Dim mr As MemoryReport = gv.getMemoryReport(CType(inputTypeList, List(Of InputType)).ToArray())

				memoryReportMap(s) = mr
			Next s

			Return New NetworkMemoryReport(memoryReportMap, GetType(ComputationGraphConfiguration), "ComputationGraph", inputTypes)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class GraphBuilder
		Public Class GraphBuilder
			Friend Const DEFAULT_TBPTT_LENGTH As Integer = 20

			Protected Friend vertices As IDictionary(Of String, GraphVertex) = New LinkedHashMap(Of String, GraphVertex)()

			''' <summary>
			''' Key: graph node. Values: input to that node
			''' </summary>
			Protected Friend vertexInputs As IDictionary(Of String, IList(Of String)) = New LinkedHashMap(Of String, IList(Of String))()

			Protected Friend networkInputs As IList(Of String) = New List(Of String)()
			Protected Friend networkInputTypes As IList(Of InputType) = New List(Of InputType)()
			Protected Friend networkOutputs As IList(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER NOTE: The field backpropType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend backpropType_Conflict As BackpropType = BackpropType.Standard
			Protected Friend tbpttFwdLength As Integer = DEFAULT_TBPTT_LENGTH
			Protected Friend tbpttBackLength As Integer = DEFAULT_TBPTT_LENGTH

			Protected Friend inputPreProcessors As IDictionary(Of String, InputPreProcessor) = New LinkedHashMap(Of String, InputPreProcessor)()

			Protected Friend globalConfiguration As NeuralNetConfiguration.Builder

'JAVA TO VB CONVERTER NOTE: The field allowDisconnected was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend allowDisconnected_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field allowNoOutput was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend allowNoOutput_Conflict As Boolean = False
			Protected Friend validateOutputConfig As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field validateTbpttConfig was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend validateTbpttConfig_Conflict As Boolean = True

			Protected Friend lastAdded As String = Nothing

			Public Sub New(ByVal globalConfiguration As NeuralNetConfiguration.Builder)
				Me.globalConfiguration = globalConfiguration
			End Sub

			Public Sub New(ByVal newConf As ComputationGraphConfiguration, ByVal globalConfiguration As NeuralNetConfiguration.Builder)

				Dim clonedConf As ComputationGraphConfiguration = newConf.clone()

				Me.vertices = clonedConf.getVertices()
				Me.vertexInputs = clonedConf.getVertexInputs()

				Me.networkInputs = clonedConf.getNetworkInputs()
				Me.networkOutputs = clonedConf.getNetworkOutputs()
				Me.backpropType_Conflict = clonedConf.getBackpropType()
				Me.tbpttFwdLength = clonedConf.getTbpttFwdLength()
				Me.tbpttBackLength = clonedConf.getTbpttBackLength()
				Me.globalConfiguration = globalConfiguration
				'this.getGlobalConfiguration().setSeed(clonedConf.getDefaultConfiguration().getSeed());
			End Sub

			''' <summary>
			''' Specify the processors for a given layer
			''' These are used at each layer for doing things like normalization and shaping of input.<br>
			''' <b>Note</b>: preprocessors can also be defined using the <seealso cref="addLayer(String, Layer, InputPreProcessor, String...)"/> method.
			''' </summary>
			''' <param name="layer">     the name of the layer that this preprocessor will be used with </param>
			''' <param name="processor"> the preprocessor to use for the specified layer </param>
			Public Overridable Function inputPreProcessor(ByVal layer As String, ByVal processor As InputPreProcessor) As GraphBuilder
				inputPreProcessors(layer) = processor
				Return Me
			End Function

			''' <summary>
			''' The type of backprop. Default setting is used for most networks (MLP, CNN etc),
			''' but optionally truncated BPTT can be used for training recurrent neural networks.
			''' If using TruncatedBPTT make sure you set both tBPTTForwardLength() and tBPTTBackwardLength()
			''' </summary>
			''' <param name="type"> Type of backprop. Default: BackpropType.Standard </param>
			Public Overridable Function backpropType(ByVal type As BackpropType) As GraphBuilder
				Me.backpropType_Conflict = type
				Return Me
			End Function

			''' <summary>
			''' When doing truncated BPTT: how many steps of forward pass should we do
			''' before doing (truncated) backprop?<br>
			''' Only applicable when doing backpropType(BackpropType.TruncatedBPTT)<br>
			''' Typically tBPTTForwardLength parameter is same as the tBPTTBackwardLength parameter,
			''' but may be larger than it in some circumstances (but never smaller)<br>
			''' Ideally your training data time series length should be divisible by this
			''' This is the k1 parameter on pg23 of
			''' <a href="http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf">http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf</a>
			''' </summary>
			''' <param name="forwardLength"> Forward length > 0, >= backwardLength </param>
			Public Overridable Function tBPTTForwardLength(ByVal forwardLength As Integer) As GraphBuilder
				Me.tbpttFwdLength = forwardLength
				Return Me
			End Function

			''' <summary>
			''' When doing truncated BPTT: how many steps of backward should we do?<br>
			''' Only applicable when doing backpropType(BackpropType.TruncatedBPTT)<br>
			''' This is the k2 parameter on pg23 of
			''' <a href="http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf">http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf</a>
			''' </summary>
			''' <param name="backwardLength"> <= forwardLength </param>
			Public Overridable Function tBPTTBackwardLength(ByVal backwardLength As Integer) As GraphBuilder
				Me.tbpttBackLength = backwardLength
				Return Me
			End Function

			''' <summary>
			''' When doing truncated backpropagation through time (tBPTT): how many steps should we do?<br>
			''' Only applicable when doing backpropType(BackpropType.TruncatedBPTT)<br>
			''' See: <a href="http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf">http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf</a>
			''' </summary>
			''' <param name="tbpttLength"> length > 0 </param>
'JAVA TO VB CONVERTER NOTE: The parameter tbpttLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function tBPTTLength(ByVal tbpttLength_Conflict As Integer) As GraphBuilder
				tBPTTForwardLength(tbpttLength_Conflict)
				Return tBPTTBackwardLength(tbpttLength_Conflict)
			End Function

			''' <summary>
			''' Add a layer, with no <seealso cref="InputPreProcessor"/>, with the specified name and specified inputs.
			''' </summary>
			''' <param name="layerName">   Name/label of the layer to add </param>
			''' <param name="layer">       The layer configuration </param>
			''' <param name="layerInputs"> Inputs to this layer. Inputs may be other layers, GraphVertex objects,
			'''                    on a combination of the two. </param>
			''' <seealso cref= #addLayer(String, Layer, InputPreProcessor, String...) </seealso>
			Public Overridable Function addLayer(ByVal layerName As String, ByVal layer As Layer, ParamArray ByVal layerInputs() As String) As GraphBuilder
				Return addLayer(layerName, layer, Nothing, layerInputs)
			End Function

			''' <summary>
			''' Add a layer, with no <seealso cref="InputPreProcessor"/>, with the specified name
			''' and input from the last added layer/vertex.
			''' </summary>
			''' <param name="layerName">   Name/label of the layer to add </param>
			''' <param name="layer">       The layer configuration </param>
			''' <seealso cref= #addLayer(String, Layer, InputPreProcessor, String...) </seealso>
			Public Overridable Function appendLayer(ByVal layerName As String, ByVal layer As Layer) As GraphBuilder
				Return appendLayer(layerName, layer, Nothing)
			End Function

			''' <summary>
			''' Add a layer, with no <seealso cref="InputPreProcessor"/>, with the specified name and specified inputs.
			''' </summary>
			''' <param name="layerName">   Name/label of the layer to add </param>
			''' <param name="layer">       The layer configuration </param>
			''' <param name="layerInputs"> Inputs to this layer. Inputs may be other layers, GraphVertex objects,
			'''                    on a combination of the two. </param>
			''' <seealso cref= #addLayer(String, Layer, InputPreProcessor, String...) </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter layer was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layer(ByVal layerName As Integer, ByVal layer_Conflict As Layer, ParamArray ByVal layerInputs() As String) As GraphBuilder
				Return addLayer(layerName.ToString(), layer_Conflict, Nothing, layerInputs)
			End Function

			''' <summary>
			''' Add a layer, with no <seealso cref="InputPreProcessor"/>, with the specified name and specified inputs.
			''' </summary>
			''' <param name="layerName">   Name/label of the layer to add </param>
			''' <param name="layer">       The layer configuration </param>
			''' <param name="layerInputs"> Inputs to this layer. Inputs may be other layers, GraphVertex objects,
			'''                    on a combination of the two. </param>
			''' <seealso cref= #addLayer(String, Layer, InputPreProcessor, String...) </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter layer was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layer(ByVal layerName As String, ByVal layer_Conflict As Layer, ParamArray ByVal layerInputs() As String) As GraphBuilder
				Return addLayer(layerName, layer_Conflict, Nothing, layerInputs)
			End Function

			''' <summary>
			''' Add a layer and an <seealso cref="InputPreProcessor"/>, with the specified name and specified inputs.
			''' </summary>
			''' <param name="layerName">    Name/label of the layer to add </param>
			''' <param name="layer">        The layer configuration </param>
			''' <param name="preProcessor"> The InputPreProcessor to use with this layer. </param>
			''' <param name="layerInputs">  Inputs to this layer. Inputs may be other layers, GraphVertex objects,
			'''                     on a combination of the two. </param>
			Public Overridable Function addLayer(ByVal layerName As String, ByVal layer As Layer, ByVal preProcessor As InputPreProcessor, ParamArray ByVal layerInputs() As String) As GraphBuilder
				Dim builder As NeuralNetConfiguration.Builder = globalConfiguration.clone()
				builder.layer(layer)
				addVertex(layerName, New LayerVertex(builder.build(), preProcessor), layerInputs)
				layer.setLayerName(layerName)
				Return Me
			End Function

			''' <summary>
			''' Add a layer and an <seealso cref="InputPreProcessor"/>, with the specified name
			''' and input from the last added layer/vertex.
			''' </summary>
			''' <param name="layerName">    Name/label of the layer to add </param>
			''' <param name="layer">        The layer configuration </param>
			''' <param name="preProcessor"> The InputPreProcessor to use with this layer. </param>
			Public Overridable Function appendLayer(ByVal layerName As String, ByVal layer As Layer, ByVal preProcessor As InputPreProcessor) As GraphBuilder

				If lastAdded Is Nothing Then
					Throw New System.InvalidOperationException("Can not use appendLayer with no previous layers")
				End If

				addLayer(layerName, layer, preProcessor, lastAdded)
				Return Me
			End Function

			''' <summary>
			''' Add a layer and an <seealso cref="InputPreProcessor"/>, with the specified name and specified inputs.
			''' </summary>
			''' <param name="layerName">    Name/label of the layer to add </param>
			''' <param name="layer">        The layer configuration </param>
			''' <param name="preProcessor"> The InputPreProcessor to use with this layer. </param>
			''' <param name="layerInputs">  Inputs to this layer. Inputs may be other layers, GraphVertex objects,
			'''                     on a combination of the two. </param>
'JAVA TO VB CONVERTER NOTE: The parameter layer was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layer(ByVal layerName As String, ByVal layer_Conflict As Layer, ByVal preProcessor As InputPreProcessor, ParamArray ByVal layerInputs() As String) As GraphBuilder
				Return addLayer(layerName, layer_Conflict, preProcessor, layerInputs)
			End Function

			''' <summary>
			''' Intended for use with the transfer learning API. Users discouraged from employing it directly.
			''' Removes the specified vertex from the vertices list, it's connections and associated preprocessor
			''' If the vertex removed is an output vertex it will also be removed from the list of outputs </summary>
			''' <param name="vertexName"> Name of the vertex to remove </param>
			Public Overridable Function removeVertex(ByVal vertexName As String) As GraphBuilder
				removeVertex(vertexName, True)
				Return Me
			End Function

			''' <summary>
			''' Intended for use with the transfer learning API. Users discouraged from employing it directly.
			''' Removes the specified vertex from the vertices list,
			''' Removes it's connections (associated preprocessor and if an output also removes it from list of outputs) if "removeConnections" is specified as true
			''' Specifying as false can leave the graph in an invalid state with references to vertices that donot exist unless a new vertex is added back in with the same name </summary>
			''' <param name="removeConnections"> Specify true to remove connections </param>
			''' <param name="vertexName"> Name of the vertex to remove </param>
			Public Overridable Function removeVertex(ByVal vertexName As String, ByVal removeConnections As Boolean) As GraphBuilder
				vertices.Remove(vertexName)
				vertexInputs.Remove(vertexName)
				If networkInputs.Contains(vertexName) Then
					networkInputs.Remove(vertexName)
				End If
				If removeConnections Then
					If networkOutputs.Contains(vertexName) Then
						networkOutputs.Remove(vertexName)
					End If
					Dim newVertexInputs As IDictionary(Of String, IList(Of String)) = New LinkedHashMap(Of String, IList(Of String))()
					For Each entry As KeyValuePair(Of String, IList(Of String)) In Me.vertexInputs.SetOfKeyValuePairs()
						Dim inputs As IList(Of String) = entry.Value
						If inputs.Contains(vertexName) Then
							'Some lists are not modifiable. So we'll make a new copy, minus the one to be removed
							Dim newList As IList(Of String) = New List(Of String)(inputs.Count - 1)
							For Each s As String In inputs
								If Not vertexName.Equals(s) Then
									newList.Add(s)
								End If
							Next s
							newVertexInputs(entry.Key) = newList
						Else
							newVertexInputs(entry.Key) = entry.Value
						End If
					Next entry
					Me.vertexInputs = newVertexInputs

					If inputPreProcessors.ContainsKey(vertexName) Then
						inputPreProcessors.Remove(vertexName)
					End If
				End If
				Return Me
			End Function

			''' <summary>
			''' Specify the inputs to the network, and their associated labels.
			''' </summary>
			''' <param name="inputNames"> The names of the inputs. This also defines their order </param>
			Public Overridable Function addInputs(ParamArray ByVal inputNames() As String) As GraphBuilder
				Collections.addAll(networkInputs, inputNames)
				lastAdded = networkInputs(networkInputs.Count - 1)
				Return Me
			End Function

			''' <summary>
			''' Specify the inputs to the network, and their associated labels.
			''' </summary>
			''' <param name="inputNames"> The names of the inputs. This also defines their order </param>
			Public Overridable Function addInputs(ByVal inputNames As ICollection(Of String)) As GraphBuilder
				CType(networkInputs, List(Of String)).AddRange(inputNames)
				lastAdded = networkInputs(networkInputs.Count - 1)
				Return Me
			End Function

			''' <summary>
			'''Specify the types of inputs to the network, so that:<br>
			''' (a) preprocessors can be automatically added, and<br>
			''' (b) the nIns (input size) for each layer can be automatically calculated and set<br>
			''' The order here is the same order as .addInputs(). Thus, if you do .addInputs("a","b") and .setInputTypes(InputType.feedForward(),
			''' InputType.convolutional(28,28,1)) then the input labelled "a" is a feed forward input, whereas the input labelled "b" in a CNN
			''' input, with 28x28x1 images as input.<br>
			''' <b>Note</b>: Using setInputTypes is not always necessary, but can be especially helpful for example with CNNs such that
			''' the calculations on input/output sizes (width, height, channels, etc) don't need to be done manually.<br>
			''' <b>Note 2</b>: If a preprocessor is manually added for a given layer, it will not be overridden by the automatic
			''' addition of preprocessors.
			''' <b>Note 3</b>: If a layer has an nIn set manually, this will not be overridden
			''' </summary>
			Public Overridable Function setInputTypes(ParamArray ByVal inputTypes() As InputType) As GraphBuilder
				If inputTypes IsNot Nothing AndAlso inputTypes.Length > 0 Then
					If networkInputs.Count > 0 AndAlso networkInputTypes.Count + inputTypes.Length <> networkInputs.Count Then
						Throw New System.ArgumentException("Invalid number of InputTypes: " & "existing inputTypes (" & networkInputTypes.Count & ") + additional inputTypes (" & inputTypes.Length & ")" & " != number of network inputs (" & networkInputs.Count & ")")
					End If
					Collections.addAll(networkInputTypes, inputTypes)
				End If
				Return Me
			End Function


			''' <summary>
			''' Set the network output labels. These should be the names of the OutputLayer instances in the network
			''' </summary>
			''' <param name="outputNames"> The names of the output layers. This also defines their order. </param>
			Public Overridable Function setOutputs(ParamArray ByVal outputNames() As String) As GraphBuilder
				networkOutputs.Clear()
				Collections.addAll(networkOutputs, outputNames)

				Return Me
			End Function

			''' <summary>
			''' Add a <seealso cref="GraphVertex"/> to the network configuration. A GraphVertex defines forward and backward pass methods,
			''' and can contain a <seealso cref="LayerVertex"/>, a <seealso cref="org.deeplearning4j.nn.conf.graph.ElementWiseVertex"/> to do element-wise
			''' addition/subtraction, a <seealso cref="MergeVertex"/> to combine/concatenate the activations out of multiple layers or vertices,
			''' a <seealso cref="org.deeplearning4j.nn.conf.graph.SubsetVertex"/> to select a subset of the activations out of another layer/GraphVertex.<br>
			''' Custom GraphVertex objects (that extend the abstract <seealso cref="GraphVertex"/> class) may also be used.
			''' </summary>
			''' <param name="vertexName">   The name of the GraphVertex to add </param>
			''' <param name="vertex">       The GraphVertex to add </param>
			''' <param name="vertexInputs"> The inputs/activations to this GraphVertex. </param>
			Public Overridable Function addVertex(ByVal vertexName As String, ByVal vertex As GraphVertex, ParamArray ByVal vertexInputs() As String) As GraphBuilder

				Preconditions.checkState(Not vertices.ContainsKey(vertexName), "Cannot add vertex: a vertex with name ""%s"" already exists", vertexName)
				vertices(vertexName) = vertex

				'Automatically insert a MergeNode if this vertex can only take 1 input (layer vertices, etc)
				If vertex.maxVertexInputs() = 1 AndAlso vertexInputs IsNot Nothing AndAlso vertexInputs.Length > 1 Then
					Dim mergeName As String = vertexName & "-merge"
					addVertex(mergeName, New MergeVertex(), vertexInputs)
					Me.vertexInputs(vertexName) = Collections.singletonList(mergeName)
				ElseIf vertexInputs IsNot Nothing Then
					Me.vertexInputs(vertexName) = New List(Of String) From {vertexInputs}
				End If

				Me.lastAdded = vertexName

				Return Me
			End Function

			''' <summary>
			''' Add a <seealso cref="GraphVertex"/> to the network configuration, with input from the last added vertex/layer. A GraphVertex defines forward and backward pass methods,
			''' and can contain a <seealso cref="LayerVertex"/>, a <seealso cref="org.deeplearning4j.nn.conf.graph.ElementWiseVertex"/> to do element-wise
			''' addition/subtraction, a <seealso cref="MergeVertex"/> to combine/concatenate the activations out of multiple layers or vertices,
			''' a <seealso cref="org.deeplearning4j.nn.conf.graph.SubsetVertex"/> to select a subset of the activations out of another layer/GraphVertex.<br>
			''' Custom GraphVertex objects (that extend the abstract <seealso cref="GraphVertex"/> class) may also be used.
			''' </summary>
			''' <param name="vertexName">   The name of the GraphVertex to add </param>
			''' <param name="vertex">       The GraphVertex to add </param>
			Public Overridable Function appendVertex(ByVal vertexName As String, ByVal vertex As GraphVertex) As GraphBuilder

				If lastAdded Is Nothing Then
					Throw New System.InvalidOperationException("Can not use appendLayer with no previous layers")
				End If

				addVertex(vertexName, vertex, lastAdded)
				Return Me
			End Function

			''' <summary>
			''' Used only during validation after building.<br>
			''' If true: don't throw an exception on configurations containing vertices that are 'disconnected'. A disconnected
			''' vertex is one that is not an output, and doesn't connect to any other vertices. i.e., it's output activations
			''' don't go anywhere. Most users can (and should) leave this as the default value of false.
			''' </summary>
			''' <param name="allowDisconnected"> Whether to allow disconnected vertices, during validation </param>
'JAVA TO VB CONVERTER NOTE: The parameter allowDisconnected was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function allowDisconnected(ByVal allowDisconnected_Conflict As Boolean) As GraphBuilder
				Me.allowDisconnected_Conflict = allowDisconnected_Conflict
				Return Me
			End Function

			''' <summary>
			''' Used only during validation after building.<br>
			''' If true: don't throw an exception on configurations without any outputs. This is enabled by default
			''' to avoid creating invalid graphs, but can be disabled if required.<br>
			''' Most users can (and should) leave this as the default value of false.
			''' </summary>
			''' <param name="allowNoOutput"> Whether to allow no outputs, during validation </param>
'JAVA TO VB CONVERTER NOTE: The parameter allowNoOutput was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function allowNoOutput(ByVal allowNoOutput_Conflict As Boolean) As GraphBuilder
				Me.allowNoOutput_Conflict = allowNoOutput_Conflict
				Return Me
			End Function

			''' <summary>
			''' Enabled by default. If enabled, the output layer configuration will be validated, to throw an exception on
			''' likely invalid outputs - such as softmax + nOut=1, or LossMCXENT + Tanh.<br>
			''' If disabled (false) no output layer validation will be performed.<br>
			''' Disabling this validation is not recommended, as the configurations that fail validation usually will
			''' not be able to learn correctly. However, the option to disable this validation is provided for advanced users
			''' when creating non-standard architectures.
			''' </summary>
			''' <param name="validate"> If true: validate output layer configuration. False: don't validate </param>
			Public Overridable Function validateOutputLayerConfig(ByVal validate As Boolean) As GraphBuilder
				Me.validateOutputConfig = validate
				Return Me
			End Function

			''' <summary>
			''' Enabled by default. If enabled, an exception will be throw when using the (invalid) combination of truncated
			''' backpropagation through time (TBPTT) with either a GlobalPoolingLayer or LastTimeStepLayer.<br>
			''' It is possible to disable this validation to allow what is almost certainly an invalid configuration to be used,
			''' however this is not recommended.
			''' </summary>
			''' <param name="validate"> Whether TBPTT validation should be performed </param>
			Public Overridable Function validateTbpttConfig(ByVal validate As Boolean) As GraphBuilder
				Me.validateTbpttConfig_Conflict = validate
				Return Me
			End Function

			''' <summary>
			''' For the (perhaps partially constructed) network configuration, return a map of activation sizes for each
			''' layer and vertex in the graph.<br>
			''' Note 1: The network configuration may be incomplete, but the inputs have been added to the layer already.<br>
			''' Note 2: To use this method, the network input types must have been set using <seealso cref="setInputTypes(InputType...)"/>
			''' first </summary>
			''' <returns> A map of activation types for the graph (key: vertex name. value: type of activations out of that vertex) </returns>
			Public Overridable ReadOnly Property LayerActivationTypes As IDictionary(Of String, InputType)
				Get
					Preconditions.checkArgument(networkInputs IsNot Nothing AndAlso networkInputs.Count > 0, "Cannot calculate activation types if no inputs have been set (use addInputs(String...))")
					Preconditions.checkArgument(networkInputTypes IsNot Nothing AndAlso networkInputTypes.Count = networkInputs.Count, "Cannot calculate layer activation types if network if network input types have not" & "been set (use ")
    
					'Instantiate temporary ComputationGraphConfiguration and calculate output shapes
					Dim conf As ComputationGraphConfiguration
					Try
						conf = buildConfig()
					Catch e As Exception
						Throw New Exception("Error calculating activation types for layers: error occured when constructing " & "temporary ComputationGraphConfiguration)", e)
					End Try
    
					Try
						conf.validate(True, True)
					Catch e As Exception
						Throw New Exception("Error calculating activation types for layers: validation of temporary" & " ComputationGraphConfiguration failed", e)
					End Try
    
					Return conf.getLayerActivationTypes(True, CType(networkInputTypes, List(Of InputType)).ToArray())
				End Get
			End Property


			Friend Overridable Function buildConfig() As ComputationGraphConfiguration
				'Validate BackpropType setting
				If (tbpttBackLength <> DEFAULT_TBPTT_LENGTH OrElse tbpttFwdLength <> DEFAULT_TBPTT_LENGTH) AndAlso backpropType_Conflict <> BackpropType.TruncatedBPTT Then
					log.warn("Truncated backpropagation through time lengths have been configured with values " & tbpttFwdLength & " and " & tbpttBackLength & " but backprop type is set to " & backpropType_Conflict & ". TBPTT configuration" & " settings will only take effect if backprop type is set to BackpropType.TruncatedBPTT")
				End If

				Dim conf As New ComputationGraphConfiguration()
				conf.backpropType = backpropType_Conflict
				conf.tbpttBackLength = tbpttBackLength
				conf.tbpttFwdLength = tbpttFwdLength

				conf.networkInputs = networkInputs
				conf.networkOutputs = networkOutputs

				conf.vertices = Me.vertices
				conf.vertexInputs = Me.vertexInputs
				conf.trainingWorkspaceMode = globalConfiguration.trainingWorkspaceMode_Conflict
				conf.inferenceWorkspaceMode = globalConfiguration.inferenceWorkspaceMode_Conflict
				conf.cacheMode = globalConfiguration.cacheMode_Conflict
				conf.validateOutputLayerConfig = validateOutputConfig
				conf.dataType = globalConfiguration.dataType_Conflict

				conf.defaultConfiguration = globalConfiguration.build()

				'Add preprocessors that were defined separately to the Layers to which they belong
				For Each entry As KeyValuePair(Of String, InputPreProcessor) In inputPreProcessors.SetOfKeyValuePairs()
					Dim gv As GraphVertex = vertices(entry.Key)
					If TypeOf gv Is LayerVertex Then
						Dim lv As LayerVertex = DirectCast(gv, LayerVertex)
						lv.setPreProcessor(entry.Value)
					Else
						Throw New System.InvalidOperationException("Invalid configuration: InputPreProcessor defined for GraphVertex """ & entry.Key & """, but this vertex is not a LayerVertex")
					End If

				Next entry

				For Each gv As KeyValuePair(Of String, GraphVertex) In vertices.SetOfKeyValuePairs()
					If TypeOf gv.Value Is LayerVertex Then
						Dim lv As LayerVertex = CType(gv.Value, LayerVertex)
						Dim l As Layer = lv.getLayerConf().getLayer()
					End If
					If TypeOf gv.Value Is SameDiffVertex Then
						CType(gv.Value, SameDiffVertex).applyGlobalConfig(globalConfiguration)
					End If

				Next gv

				Return conf
			End Function


			''' <summary>
			''' Create the ComputationGraphConfiguration from the Builder pattern
			''' </summary>
			Public Overridable Function build() As ComputationGraphConfiguration

				Dim conf As ComputationGraphConfiguration = buildConfig()
				conf.validate(allowDisconnected_Conflict, allowNoOutput_Conflict) 'throws exception for invalid configuration

				'Automatically add preprocessors, set nIns for CNN->dense transitions, etc
				If networkInputTypes.Count > 0 Then
					conf.addPreProcessors(CType(networkInputTypes, List(Of InputType)).ToArray())
				End If

				If validateOutputConfig Then
					'Validate output layer configurations...
					For Each e As KeyValuePair(Of String, GraphVertex) In conf.getVertices().entrySet()
						If TypeOf e.Value Is LayerVertex Then
							Dim l As Layer = CType(e.Value, LayerVertex).getLayerConf().getLayer()
							OutputLayerUtil.validateOutputLayer(e.Key, l) 'No-op for non output/loss layers
						End If
					Next e
				End If

				If backpropType_Conflict = BackpropType.TruncatedBPTT AndAlso validateTbpttConfig_Conflict Then
					'Check for invalid combination - tbptt plus LastTimeStepLayer or
					For Each e As KeyValuePair(Of String, GraphVertex) In vertices.SetOfKeyValuePairs()
						Dim gv As GraphVertex = e.Value
						Dim l As Layer = (If(TypeOf gv Is LayerVertex, DirectCast(gv, LayerVertex).getLayerConf().getLayer(), Nothing))
						If TypeOf gv Is LastTimeStepVertex OrElse (l IsNot Nothing AndAlso (TypeOf l Is LastTimeStep OrElse TypeOf l Is GlobalPoolingLayer)) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							Dim s As String = (If(l Is Nothing, gv.GetType().FullName, l.GetType().FullName))
							Dim n As String = e.Key
							Throw New System.InvalidOperationException("Invalid network configuration detected: Truncated backpropagation through time (TBPTT)" & " cannot be used with layer """ & n & """ of type " & s & ": TBPTT is incompatible with this layer type (which is designed " & "to process entire sequences at once, and does support the type of sequence segments that TPBTT uses)." & vbLf & "This check can be disabled using validateTbpttConfig(false) but this is not recommended.")
						End If
					Next e
				End If

				Return conf
			End Function
		End Class
	End Class

End Namespace