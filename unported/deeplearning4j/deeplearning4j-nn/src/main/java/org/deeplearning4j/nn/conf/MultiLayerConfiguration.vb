Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports NetworkMemoryReport = org.deeplearning4j.nn.conf.memory.NetworkMemoryReport
Imports JsonMappers = org.deeplearning4j.nn.conf.serde.JsonMappers
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports OutputLayerUtil = org.deeplearning4j.util.OutputLayerUtil
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossBinaryXENT = org.nd4j.linalg.lossfunctions.impl.LossBinaryXENT
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
Imports LossNegativeLogLikelihood = org.nd4j.linalg.lossfunctions.impl.LossNegativeLogLikelihood
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports InvalidTypeIdException = org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException
Imports ArrayNode = org.nd4j.shade.jackson.databind.node.ArrayNode

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
'ORIGINAL LINE: @Data @AllArgsConstructor(access = AccessLevel.@PRIVATE) @NoArgsConstructor @Slf4j public class MultiLayerConfiguration implements java.io.Serializable, Cloneable
	<Serializable>
	Public Class MultiLayerConfiguration
		Implements ICloneable

		Protected Friend confs As IList(Of NeuralNetConfiguration)
		Protected Friend inputPreProcessors As IDictionary(Of Integer, InputPreProcessor) = New Dictionary(Of Integer, InputPreProcessor)()
		Protected Friend backpropType As BackpropType = BackpropType.Standard
		Protected Friend tbpttFwdLength As Integer = 20
		Protected Friend tbpttBackLength As Integer = 20
		Protected Friend validateOutputLayerConfig As Boolean = True 'Default to legacy for pre 1.0.0-beta3 networks on deserialization

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
		Protected Friend dataType As DataType = DataType.FLOAT 'Default to float for deserialization of beta3 and earlier nets

		'Counter for the number of parameter updates so far
		' This is important for learning rate schedules, for example, and is stored here to ensure it is persisted
		' for Spark and model serialization
		Protected Friend iterationCount As Integer = 0

		'Counter for the number of epochs completed so far. Used for per-epoch schedules
'JAVA TO VB CONVERTER NOTE: The field epochCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend epochCount_Conflict As Integer = 0

		Public Overridable Property EpochCount As Integer
			Get
				Return epochCount_Conflict
			End Get
			Set(ByVal epochCount As Integer)
				Me.epochCount_Conflict = epochCount
				For i As Integer = 0 To confs.Count - 1
					getConf(i).setEpochCount(epochCount)
				Next i
			End Set
		End Property


		''' <returns> JSON representation of NN configuration </returns>
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
		''' Create a neural net configuration from json
		''' </summary>
		''' <param name="json"> the neural net configuration from json </param>
		''' <returns> <seealso cref="MultiLayerConfiguration"/> </returns>
		Public Shared Function fromYaml(ByVal json As String) As MultiLayerConfiguration
			Dim mapper As ObjectMapper = NeuralNetConfiguration.mapperYaml()
			Try
				Return mapper.readValue(json, GetType(MultiLayerConfiguration))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function


		''' <returns> JSON representation of NN configuration </returns>
		Public Overridable Function toJson() As String
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
		''' Create a neural net configuration from json
		''' </summary>
		''' <param name="json"> the neural net configuration from json </param>
		''' <returns> <seealso cref="MultiLayerConfiguration"/> </returns>
		Public Shared Function fromJson(ByVal json As String) As MultiLayerConfiguration
			Dim conf As MultiLayerConfiguration
			Dim mapper As ObjectMapper = NeuralNetConfiguration.mapper()
			Try
				conf = mapper.readValue(json, GetType(MultiLayerConfiguration))
			Catch e As InvalidTypeIdException
				If e.Message.contains("@class") Then
					Try
						'JSON may be legacy (1.0.0-alpha or earlier), attempt to load it using old format
						Return JsonMappers.LegacyMapper.readValue(json, GetType(MultiLayerConfiguration))
					Catch e2 As InvalidTypeIdException
						'Check for legacy custom layers: "Could not resolve type id 'CustomLayer' as a subtype of [simple type, class org.deeplearning4j.nn.conf.layers.Layer]: known type ids = [Bidirectional, CenterLossOutputLayer, CnnLossLayer, ..."
						'1.0.0-beta5: dropping support for custom layers defined in pre-1.0.0-beta format. Built-in layers from these formats still work
						Dim msg As String = e2.Message
						If msg IsNot Nothing AndAlso msg.Contains("Could not resolve type id") Then
							Throw New Exception("Error deserializing MultiLayerConfiguration - configuration may have a custom " & "layer, vertex or preprocessor, in pre version 1.0.0-beta JSON format." & vbLf & "Models in legacy format with custom" & " layers should be loaded in 1.0.0-beta to 1.0.0-beta4 and saved again, before loading in the current version of DL4J", e)
						End If
						Throw New Exception(e2)
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try
				End If
				Throw New Exception(e)
			Catch e As IOException
				'Check if this exception came from legacy deserializer...
				Dim msg As String = e.Message
				If msg IsNot Nothing AndAlso msg.Contains("legacy") Then
					Throw New Exception("Error deserializing MultiLayerConfiguration - configuration may have a custom " & "layer, vertex or preprocessor, in pre version 1.0.0-alpha JSON format. These layers can be " & "deserialized by first registering them with NeuralNetConfiguration.registerLegacyCustomClassesForJSON(Class...)", e)
				End If
				Throw New Exception(e)
			End Try


			'To maintain backward compatibility after loss function refactoring (configs generated with v0.5.0 or earlier)
			' Previously: enumeration used for loss functions. Now: use classes
			' IN the past, could have only been an OutputLayer or RnnOutputLayer using these enums
			Dim layerCount As Integer = 0
			Dim confs As JsonNode = Nothing
			For Each nnc As NeuralNetConfiguration In conf.getConfs()
				Dim l As Layer = nnc.getLayer()
				If TypeOf l Is BaseOutputLayer AndAlso DirectCast(l, BaseOutputLayer).getLossFn() Is Nothing Then
					'lossFn field null -> may be an old config format, with lossFunction field being for the enum
					'if so, try walking the JSON graph to extract out the appropriate enum value

					Dim ol As BaseOutputLayer = DirectCast(l, BaseOutputLayer)
					Try
						Dim jsonNode As JsonNode = mapper.readTree(json)
						If confs Is Nothing Then
							confs = jsonNode.get("confs")
						End If
						If TypeOf confs Is ArrayNode Then
							Dim layerConfs As ArrayNode = CType(confs, ArrayNode)
							Dim outputLayerNNCNode As JsonNode = layerConfs.get(layerCount)
							If outputLayerNNCNode Is Nothing Then
								Return conf 'Should never happen...
							End If
							Dim outputLayerNode As JsonNode = outputLayerNNCNode.get("layer")

							Dim lossFunctionNode As JsonNode = Nothing
							If outputLayerNode.has("output") Then
								lossFunctionNode = outputLayerNode.get("output").get("lossFunction")
							ElseIf outputLayerNode.has("rnnoutput") Then
								lossFunctionNode = outputLayerNode.get("rnnoutput").get("lossFunction")
							End If

							If lossFunctionNode IsNot Nothing Then
								Dim lossFunctionEnumStr As String = lossFunctionNode.asText()
								Dim lossFunction As LossFunctions.LossFunction = Nothing
								Try
									lossFunction = LossFunctions.LossFunction.valueOf(lossFunctionEnumStr)
								Catch e As Exception
									log.warn("OutputLayer with null LossFunction or pre-0.6.0 loss function configuration detected: could not parse JSON", e)
								End Try

								If lossFunction <> Nothing Then
									Select Case lossFunction.innerEnumValue
										Case LossFunctions.LossFunction.InnerEnum.MSE
											ol.setLossFn(New LossMSE())
										Case LossFunctions.LossFunction.InnerEnum.XENT
											ol.setLossFn(New LossBinaryXENT())
										Case LossFunctions.LossFunction.InnerEnum.NEGATIVELOGLIKELIHOOD
											ol.setLossFn(New LossNegativeLogLikelihood())
										Case LossFunctions.LossFunction.InnerEnum.MCXENT
											ol.setLossFn(New LossMCXENT())

										'Remaining: TODO
										Case Else
											log.warn("OutputLayer with null LossFunction or pre-0.6.0 loss function configuration detected: could not set loss function for {}", lossFunction)
									End Select
								End If
							End If

						Else
							log.warn("OutputLayer with null LossFunction or pre-0.6.0 loss function configuration detected: could not parse JSON: layer 'confs' field is not an ArrayNode (is: {})", (If(confs IsNot Nothing, confs.GetType(), Nothing)))
						End If
					Catch e As IOException
						log.warn("OutputLayer with null LossFunction or pre-0.6.0 loss function configuration detected: could not parse JSON", e)
						Exit For
					End Try
				End If

				'Also, pre 0.7.2: activation functions were Strings ("activationFunction" field), not classes ("activationFn")
				'Try to load the old format if necessary, and create the appropriate IActivation instance
				If (TypeOf l Is BaseLayer) AndAlso DirectCast(l, BaseLayer).getActivationFn() Is Nothing Then
					Try
						Dim jsonNode As JsonNode = mapper.readTree(json)
						If confs Is Nothing Then
							confs = jsonNode.get("confs")
						End If
						If TypeOf confs Is ArrayNode Then
							Dim layerConfs As ArrayNode = CType(confs, ArrayNode)
							Dim outputLayerNNCNode As JsonNode = layerConfs.get(layerCount)
							If outputLayerNNCNode Is Nothing Then
								Return conf 'Should never happen...
							End If
							Dim layerWrapperNode As JsonNode = outputLayerNNCNode.get("layer")

							If layerWrapperNode Is Nothing OrElse layerWrapperNode.size() <> 1 Then
								Continue For
							End If

							Dim layerNode As JsonNode = layerWrapperNode.elements().next()
							Dim activationFunction As JsonNode = layerNode.get("activationFunction") 'Should only have 1 element: "dense", "output", etc

							If activationFunction IsNot Nothing Then
								Dim ia As IActivation = Activation.fromString(activationFunction.asText()).getActivationFunction()
								DirectCast(l, BaseLayer).setActivationFn(ia)
							End If
						End If

					Catch e As IOException
						log.warn("Layer with null ActivationFn field or pre-0.7.2 activation function detected: could not parse JSON", e)
					End Try
				End If

				If Not handleLegacyWeightInitFromJson(json, l, mapper, confs, layerCount) Then
					Return conf
				End If

				layerCount += 1
			Next nnc
			Return conf
		End Function

		''' <summary>
		''' Handle <seealso cref="WeightInit"/> and <seealso cref="Distribution"/> from legacy configs in Json format. Copied from handling of <seealso cref="Activation"/>
		''' above. </summary>
		''' <returns> True if all is well and layer iteration shall continue. False else-wise. </returns>
		Private Shared Function handleLegacyWeightInitFromJson(ByVal json As String, ByVal l As Layer, ByVal mapper As ObjectMapper, ByVal confs As JsonNode, ByVal layerCount As Integer) As Boolean
			If (TypeOf l Is BaseLayer) AndAlso DirectCast(l, BaseLayer).getWeightInitFn() Is Nothing Then
				Try
					Dim jsonNode As JsonNode = mapper.readTree(json)
					If confs Is Nothing Then
						confs = jsonNode.get("confs")
					End If
					If TypeOf confs Is ArrayNode Then
						Dim layerConfs As ArrayNode = CType(confs, ArrayNode)
						Dim outputLayerNNCNode As JsonNode = layerConfs.get(layerCount)
						If outputLayerNNCNode Is Nothing Then
							Return False 'Should never happen...
						End If
						Dim layerWrapperNode As JsonNode = outputLayerNNCNode.get("layer")

						If layerWrapperNode Is Nothing OrElse layerWrapperNode.size() <> 1 Then
							Return True
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
							DirectCast(l, BaseLayer).setWeightInitFn(wi)
						End If
					End If

				Catch e As IOException
					log.warn("Layer with null WeightInit detected: " & l.LayerName & ", could not parse JSON", e)
				End Try
			End If
			Return True

		End Function

		Public Overrides Function ToString() As String
			Return toJson()
		End Function

		Public Overridable Function getConf(ByVal i As Integer) As NeuralNetConfiguration
			Return confs(i)
		End Function

		Public Overrides Function clone() As MultiLayerConfiguration
			Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim clone_Conflict As MultiLayerConfiguration = CType(MyBase.clone(), MultiLayerConfiguration)

				If clone_Conflict.confs IsNot Nothing Then
					Dim list As IList(Of NeuralNetConfiguration) = New List(Of NeuralNetConfiguration)()
					For Each conf As NeuralNetConfiguration In clone_Conflict.confs
						list.Add(conf.clone())
					Next conf
					clone_Conflict.confs = list
				End If

				If clone_Conflict.inputPreProcessors IsNot Nothing Then
					Dim map As IDictionary(Of Integer, InputPreProcessor) = New Dictionary(Of Integer, InputPreProcessor)()
					For Each entry As KeyValuePair(Of Integer, InputPreProcessor) In clone_Conflict.inputPreProcessors.SetOfKeyValuePairs()
						map(entry.Key) = entry.Value.clone()
					Next entry
					clone_Conflict.inputPreProcessors = map
				End If

				clone_Conflict.inferenceWorkspaceMode = Me.inferenceWorkspaceMode
				clone_Conflict.trainingWorkspaceMode = Me.trainingWorkspaceMode
				clone_Conflict.cacheMode = Me.cacheMode
				clone_Conflict.validateOutputLayerConfig = Me.validateOutputLayerConfig
				clone_Conflict.dataType = Me.dataType

				Return clone_Conflict

			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function getInputPreProcess(ByVal curr As Integer) As InputPreProcessor
			Return inputPreProcessors(curr)
		End Function

		''' <summary>
		''' Get a <seealso cref="MemoryReport"/> for the given MultiLayerConfiguration. This is used to estimate the
		''' memory requirements for the given network configuration and input
		''' </summary>
		''' <param name="inputType"> Input types for the network </param>
		''' <returns> Memory report for the network </returns>
		Public Overridable Function getMemoryReport(ByVal inputType As InputType) As NetworkMemoryReport

			Dim memoryReportMap As IDictionary(Of String, MemoryReport) = New LinkedHashMap(Of String, MemoryReport)()
			Dim nLayers As Integer = confs.Count
			For i As Integer = 0 To nLayers - 1
				Dim layerName As String = confs(i).getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = i.ToString()
				End If

				'Pass input type through preprocessor, if necessary
				Dim preproc As InputPreProcessor = getInputPreProcess(i)
				'TODO memory requirements for preprocessor
				If preproc IsNot Nothing Then
					inputType = preproc.getOutputType(inputType)
				End If

				Dim report As LayerMemoryReport = confs(i).getLayer().getMemoryReport(inputType)
				memoryReportMap(layerName) = report

				inputType = confs(i).getLayer().getOutputType(i, inputType)
			Next i

			Return New NetworkMemoryReport(memoryReportMap, GetType(MultiLayerConfiguration), "MultiLayerNetwork", inputType)
		End Function

		''' <summary>
		''' For the given input shape/type for the network, return a list of activation sizes for each layer in the network.<br>
		''' i.e., list.get(i) is the output activation sizes for layer i
		''' </summary>
		''' <param name="inputType"> Input type for the network </param>
		''' <returns> A lits of activation types for the network, indexed by layer number </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public List<org.deeplearning4j.nn.conf.inputs.InputType> getLayerActivationTypes(@NonNull InputType inputType)
		Public Overridable Function getLayerActivationTypes(ByVal inputType As InputType) As IList(Of InputType)
			Dim [out] As IList(Of InputType) = New List(Of InputType)()
			Dim nLayers As Integer = confs.Count
			For i As Integer = 0 To nLayers - 1
				Dim preproc As InputPreProcessor = getInputPreProcess(i)
				If preproc IsNot Nothing Then
					inputType = preproc.getOutputType(inputType)
				End If

				inputType = confs(i).getLayer().getOutputType(i, inputType)
				[out].Add(inputType)
			Next i
			Return [out]
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class Builder
		Public Class Builder

			Friend Const DEFAULT_TBPTT_LENGTH As Integer = 20

'JAVA TO VB CONVERTER NOTE: The field confs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend confs_Conflict As IList(Of NeuralNetConfiguration) = New List(Of NeuralNetConfiguration)()
			Protected Friend dampingFactor As Double = 100
'JAVA TO VB CONVERTER NOTE: The field inputPreProcessors was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend inputPreProcessors_Conflict As IDictionary(Of Integer, InputPreProcessor) = New Dictionary(Of Integer, InputPreProcessor)()
'JAVA TO VB CONVERTER NOTE: The field backpropType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend backpropType_Conflict As BackpropType = BackpropType.Standard
			Protected Friend tbpttFwdLength As Integer = DEFAULT_TBPTT_LENGTH
			Protected Friend tbpttBackLength As Integer = DEFAULT_TBPTT_LENGTH
'JAVA TO VB CONVERTER NOTE: The field inputType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend inputType_Conflict As InputType

'JAVA TO VB CONVERTER NOTE: The field trainingWorkspaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend trainingWorkspaceMode_Conflict As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER NOTE: The field inferenceWorkspaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend inferenceWorkspaceMode_Conflict As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER NOTE: The field cacheMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cacheMode_Conflict As CacheMode = CacheMode.NONE
			Protected Friend validateOutputConfig As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field validateTbpttConfig was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend validateTbpttConfig_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field dataType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dataType_Conflict As DataType
'JAVA TO VB CONVERTER NOTE: The field overrideNinUponBuild was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend overrideNinUponBuild_Conflict As Boolean = True


			''' <summary>
			''' Whether to over ride the nIn
			''' configuration forcibly upon construction.
			''' Default value is true </summary>
			''' <param name="overrideNinUponBuild"> Whether to over ride the nIn
			'''           configuration forcibly upon construction. </param>
			''' <returns> builder pattern </returns>
'JAVA TO VB CONVERTER NOTE: The parameter overrideNinUponBuild was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function overrideNinUponBuild(ByVal overrideNinUponBuild_Conflict As Boolean) As Builder
				Me.overrideNinUponBuild_Conflict = overrideNinUponBuild_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify the processors.
			''' These are used at each layer for doing things like normalization and
			''' shaping of input.
			''' </summary>
			''' <param name="processor"> what to use to preProcess the data. </param>
			''' <returns> builder pattern </returns>
			Public Overridable Function inputPreProcessor(ByVal layer As Integer?, ByVal processor As InputPreProcessor) As Builder
				inputPreProcessors(layer) = processor
				Return Me
			End Function

			Public Overridable Function inputPreProcessors(ByVal processors As IDictionary(Of Integer, InputPreProcessor)) As Builder
				Me.inputPreProcessors_Conflict = processors
				Return Me
			End Function

			''' @deprecated Use <seealso cref="NeuralNetConfiguration.Builder.trainingWorkspaceMode(WorkspaceMode)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""NeuralNetConfiguration.Builder.trainingWorkspaceMode(WorkspaceMode)""/>") public Builder trainingWorkspaceMode(@NonNull WorkspaceMode workspaceMode)
			<Obsolete("Use <seealso cref=""NeuralNetConfiguration.Builder.trainingWorkspaceMode(WorkspaceMode)""/>")>
			Public Overridable Function trainingWorkspaceMode(ByVal workspaceMode As WorkspaceMode) As Builder
				Me.trainingWorkspaceMode_Conflict = workspaceMode
				Return Me
			End Function

			''' @deprecated Use <seealso cref="NeuralNetConfiguration.Builder.inferenceWorkspaceMode(WorkspaceMode)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""NeuralNetConfiguration.Builder.inferenceWorkspaceMode(WorkspaceMode)""/>") public Builder inferenceWorkspaceMode(@NonNull WorkspaceMode workspaceMode)
			<Obsolete("Use <seealso cref=""NeuralNetConfiguration.Builder.inferenceWorkspaceMode(WorkspaceMode)""/>")>
			Public Overridable Function inferenceWorkspaceMode(ByVal workspaceMode As WorkspaceMode) As Builder
				Me.inferenceWorkspaceMode_Conflict = workspaceMode
				Return Me
			End Function

			''' <summary>
			''' This method defines how/if preOutput cache is handled:
			''' NONE: cache disabled (default value)
			''' HOST: Host memory will be used
			''' DEVICE: GPU memory will be used (on CPU backends effect will be the same as for HOST)
			''' </summary>
			''' <param name="cacheMode">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder cacheMode(@NonNull CacheMode cacheMode)
'JAVA TO VB CONVERTER NOTE: The parameter cacheMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function cacheMode(ByVal cacheMode_Conflict As CacheMode) As Builder
				Me.cacheMode_Conflict = cacheMode_Conflict
				Return Me
			End Function

			''' <summary>
			''' The type of backprop. Default setting is used for most networks (MLP, CNN etc),
			''' but optionally truncated BPTT can be used for training recurrent neural networks.
			''' If using TruncatedBPTT make sure you set both tBPTTForwardLength() and tBPTTBackwardLength()
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder backpropType(@NonNull BackpropType type)
			Public Overridable Function backpropType(ByVal type As BackpropType) As Builder
				Me.backpropType_Conflict = type
				Return Me
			End Function

			''' <summary>
			''' When doing truncated BPTT: how many steps should we do?<br>
			''' Only applicable when doing backpropType(BackpropType.TruncatedBPTT)<br>
			''' See: <a href="http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf">http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf</a>
			''' </summary>
			''' <param name="bpttLength"> length > 0 </param>
			Public Overridable Function tBPTTLength(ByVal bpttLength As Integer) As Builder
				tBPTTForwardLength(bpttLength)
				Return tBPTTBackwardLength(bpttLength)
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
			Public Overridable Function tBPTTForwardLength(ByVal forwardLength As Integer) As Builder
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
			Public Overridable Function tBPTTBackwardLength(ByVal backwardLength As Integer) As Builder
				Me.tbpttBackLength = backwardLength
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter confs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function confs(ByVal confs_Conflict As IList(Of NeuralNetConfiguration)) As Builder
				Me.confs_Conflict = confs_Conflict
				Return Me
			End Function

			Public Overridable Function setInputType(ByVal inputType As InputType) As Builder
				Me.inputType_Conflict = inputType
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
			Public Overridable Function validateOutputLayerConfig(ByVal validate As Boolean) As Builder
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
			Public Overridable Function validateTbpttConfig(ByVal validate As Boolean) As Builder
				Me.validateTbpttConfig_Conflict = validate
				Return Me
			End Function

			''' <summary>
			''' Set the DataType for the network parameters and activations for all layers in the network. Default: Float </summary>
			''' <param name="dataType"> Datatype to use for parameters and activations </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder dataType(@NonNull DataType dataType)
'JAVA TO VB CONVERTER NOTE: The parameter dataType was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataType(ByVal dataType_Conflict As DataType) As Builder
				Me.dataType_Conflict = dataType_Conflict
				Return Me
			End Function


			Public Overridable Function build() As MultiLayerConfiguration
				'Validate BackpropType setting
				If (tbpttBackLength <> DEFAULT_TBPTT_LENGTH OrElse tbpttFwdLength <> DEFAULT_TBPTT_LENGTH) AndAlso backpropType_Conflict <> BackpropType.TruncatedBPTT Then
					log.warn("Truncated backpropagation through time lengths have been configured with values " & tbpttFwdLength & " and " & tbpttBackLength & " but backprop type is set to " & backpropType_Conflict & ". TBPTT configuration" & " settings will only take effect if backprop type is set to BackpropType.TruncatedBPTT")
				End If

				If backpropType_Conflict = BackpropType.TruncatedBPTT AndAlso validateTbpttConfig_Conflict Then
					'Check for invalid combination - tbptt plus LastTimeStepLayer or
					For i As Integer = 0 To confs_Conflict.Count - 1
						Dim l As Layer = confs(i).getLayer()
						If TypeOf l Is LastTimeStep OrElse TypeOf l Is GlobalPoolingLayer Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							Throw New System.InvalidOperationException("Invalid network configuration detected: Truncated backpropagation through time (TBPTT)" & " cannot be used with layer " & i & " of type " & l.GetType().FullName & ": TBPTT is incompatible with this layer type (which is designed " & "to process entire sequences at once, and does support the type of sequence segments that TPBTT uses)." & vbLf & "This check can be disabled using validateTbpttConfig(false) but this is not recommended.")
						End If
					Next i
				End If


				If inputType_Conflict Is Nothing AndAlso inputPreProcessors(0) Is Nothing Then
					'User hasn't set the InputType. Sometimes we can infer it...
					' For example, Dense/RNN layers, where preprocessor isn't set -> user is *probably* going to feed in
					' standard feedforward or RNN data
					'This isn't the most elegant implementation, but should avoid breaking backward compatibility here
					'Can't infer InputType for CNN layers, however (don't know image dimensions/depth)
					Dim firstLayer As Layer = confs(0).getLayer()
					If TypeOf firstLayer Is BaseRecurrentLayer Then
						Dim brl As BaseRecurrentLayer = DirectCast(firstLayer, BaseRecurrentLayer)
						Dim nIn As val = brl.getNIn()
						If nIn > 0 Then
							inputType_Conflict = InputType.recurrent(nIn, brl.getRnnDataFormat())
						End If
					ElseIf TypeOf firstLayer Is DenseLayer OrElse TypeOf firstLayer Is EmbeddingLayer OrElse TypeOf firstLayer Is OutputLayer Then
						'Can't just use "instanceof FeedForwardLayer" here. ConvolutionLayer is also a FeedForwardLayer
						Dim ffl As FeedForwardLayer = DirectCast(firstLayer, FeedForwardLayer)
						Dim nIn As val = ffl.getNIn()
						If nIn > 0 Then
							inputType_Conflict = InputType.feedForward(nIn)
						End If
					End If
				End If


				'Add preprocessors and set nIns, if InputType has been set
				' Builder.inputType field can be set in 1 of 4 ways:
				' 1. User calls setInputType directly
				' 2. Via ConvolutionLayerSetup -> internally calls setInputType(InputType.convolutional(...))
				' 3. Via the above code: i.e., assume input is as expected  by the RNN or dense layer -> sets the inputType field
				If inputType_Conflict IsNot Nothing Then
					Dim currentInputType As InputType = inputType_Conflict
					For i As Integer = 0 To confs_Conflict.Count - 1
						Dim l As Layer = confs(i).getLayer()
						If inputPreProcessors(i) Is Nothing Then
							'Don't override preprocessor setting, but set preprocessor if required...
							Dim inputPreProcessor As InputPreProcessor = l.getPreProcessorForInputType(currentInputType)
							If inputPreProcessor IsNot Nothing Then
								inputPreProcessors(i) = inputPreProcessor
							End If
						End If

						Dim inputPreProcessor As InputPreProcessor = inputPreProcessors(i)
						If inputPreProcessor IsNot Nothing Then
							currentInputType = inputPreProcessor.getOutputType(currentInputType)
						End If
						If i > 0 Then
							Dim layer As Layer = confs(i - 1).getLayer()
							'convolution 1d is an edge case where it has rnn input type but the filters
							'should be the output
							If TypeOf layer Is Convolution1DLayer Then
								If TypeOf l Is DenseLayer AndAlso TypeOf inputType_Conflict Is InputType.InputTypeRecurrent Then
									Dim feedForwardLayer As FeedForwardLayer = DirectCast(l, FeedForwardLayer)
									 If TypeOf inputType_Conflict Is InputType.InputTypeRecurrent Then
										Dim recurrent As InputType.InputTypeRecurrent = DirectCast(inputType_Conflict, InputType.InputTypeRecurrent)
										feedForwardLayer.setNIn(recurrent.getTimeSeriesLength())
									 End If
								Else
									l.setNIn(currentInputType, overrideNinUponBuild_Conflict) 'Don't override the nIn setting, if it's manually set by the user
								End If
							Else
								l.setNIn(currentInputType, overrideNinUponBuild_Conflict) 'Don't override the nIn setting, if it's manually set by the user
							End If

						Else
							l.setNIn(currentInputType, overrideNinUponBuild_Conflict) 'Don't override the nIn setting, if it's manually set by the user
						End If


						currentInputType = l.getOutputType(i, currentInputType)
					Next i

				End If

				Dim conf As New MultiLayerConfiguration()
				conf.confs = Me.confs_Conflict
				conf.inputPreProcessors = inputPreProcessors_Conflict
				conf.backpropType = backpropType_Conflict
				conf.tbpttFwdLength = tbpttFwdLength
				conf.tbpttBackLength = tbpttBackLength
				conf.trainingWorkspaceMode = trainingWorkspaceMode_Conflict
				conf.inferenceWorkspaceMode = inferenceWorkspaceMode_Conflict
				conf.cacheMode = cacheMode_Conflict
				conf.dataType = dataType_Conflict

				Nd4j.Random.setSeed(conf.getConf(0).getSeed())

				'Validate output layer configuration
				If validateOutputConfig Then
					'Validate output layer configurations...
					For Each n As NeuralNetConfiguration In conf.getConfs()
						Dim l As Layer = n.getLayer()
						OutputLayerUtil.validateOutputLayer(l.LayerName, l) 'No-op for non output/loss layers
					Next n
				End If

				Return conf

			End Function
		End Class
	End Class

End Namespace