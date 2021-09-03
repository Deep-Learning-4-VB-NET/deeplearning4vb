Imports System.Collections.Generic
Imports IOUtils = org.apache.commons.io.IOUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports BaseOutputLayer = org.deeplearning4j.nn.conf.layers.BaseOutputLayer
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports DropConnect = org.deeplearning4j.nn.conf.weightnoise.DropConnect
Imports BatchNormalizationParamInitializer = org.deeplearning4j.nn.params.BatchNormalizationParamInitializer
Imports JsonLocation = org.nd4j.shade.jackson.core.JsonLocation
Imports JsonParser = org.nd4j.shade.jackson.core.JsonParser
Imports DeserializationContext = org.nd4j.shade.jackson.databind.DeserializationContext
Imports JsonDeserializer = org.nd4j.shade.jackson.databind.JsonDeserializer
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports ObjectNode = org.nd4j.shade.jackson.databind.node.ObjectNode

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

Namespace org.deeplearning4j.nn.conf.serde



	Public Class ComputationGraphConfigurationDeserializer
		Inherits BaseNetConfigDeserializer(Of ComputationGraphConfiguration)

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public ComputationGraphConfigurationDeserializer(org.nd4j.shade.jackson.databind.JsonDeserializer<?> defaultDeserializer)
		Public Sub New(ByVal defaultDeserializer As JsonDeserializer(Of T1))
			MyBase.New(defaultDeserializer, GetType(ComputationGraphConfiguration))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.ComputationGraphConfiguration deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext ctxt) throws java.io.IOException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal ctxt As DeserializationContext) As ComputationGraphConfiguration
			Dim charOffsetStart As Long = jp.getCurrentLocation().getCharOffset()
			Dim conf As ComputationGraphConfiguration = CType(defaultDeserializer.deserialize(jp, ctxt), ComputationGraphConfiguration)


			'Updater configuration changed after 0.8.0 release
			'Previously: enumerations and fields. Now: classes
			'Here, we manually create the appropriate Updater instances, if the IUpdater field is empty

			Dim layerList As IList(Of Layer) = New List(Of Layer)()
			Dim vertices As IDictionary(Of String, GraphVertex) = conf.getVertices()
			For Each entry As KeyValuePair(Of String, GraphVertex) In vertices.SetOfKeyValuePairs()
				If TypeOf entry.Value Is LayerVertex Then
					Dim lv As LayerVertex = CType(entry.Value, LayerVertex)
					layerList.Add(lv.getLayerConf().getLayer())
				End If
			Next entry

			Dim layers() As Layer = CType(layerList, List(Of Layer)).ToArray()
			'Now, check if we need to manually handle IUpdater deserialization from legacy format
			Dim attemptIUpdaterFromLegacy As Boolean = requiresIUpdaterFromLegacy(layers)
			Dim requireLegacyRegularizationHandling As Boolean = requiresRegularizationFromLegacy(layers)
			Dim requiresLegacyWeightInitHandling As Boolean = requiresWeightInitFromLegacy(layers)
			Dim requiresLegacyActivationHandling As Boolean = requiresActivationFromLegacy(layers)
			Dim requiresLegacyLossHandling As Boolean = Me.requiresLegacyLossHandling(layers)

			Dim charOffsetEnd As Long? = Nothing
			Dim endLocation As JsonLocation = Nothing
			Dim jsonSubString As String = Nothing
			If attemptIUpdaterFromLegacy OrElse requireLegacyRegularizationHandling OrElse requiresLegacyWeightInitHandling Then
				endLocation = jp.getCurrentLocation()
				charOffsetEnd = endLocation.getCharOffset()
				Dim sourceRef As Object = endLocation.getSourceRef()
				Dim s As String
				If TypeOf sourceRef Is StringReader Then
					'Workaround: sometimes sourceRef is a String, sometimes a StringReader
					DirectCast(sourceRef, StringReader).reset()
					s = IOUtils.toString(DirectCast(sourceRef, StringReader))
				Else
					s = sourceRef.ToString()
				End If
								Dim tempVar = (int) charOffsetStart - 1
				jsonSubString = s.Substring(tempVar, charOffsetEnd.Value - (tempVar))

				Dim om As ObjectMapper = NeuralNetConfiguration.mapper()
				Dim rootNode As JsonNode = om.readTree(jsonSubString)

				Dim verticesNode As ObjectNode = CType(rootNode.get("vertices"), ObjectNode)
				Dim iter As IEnumerator(Of JsonNode) = verticesNode.elements()
				Dim layerIdx As Integer = 0
				Do While iter.MoveNext()
					Dim [next] As JsonNode = iter.Current
					Dim confNode As ObjectNode = Nothing
					Dim cls As String = If([next].has("@class"), [next].get("@class").asText(), Nothing)
					If [next].has("LayerVertex") Then
						[next] = [next].get("LayerVertex")
						If [next].has("layerConf") Then
							confNode = CType([next].get("layerConf"), ObjectNode)
							[next] = confNode.get("layer").elements().next()
						Else
							Continue Do
						End If

						If attemptIUpdaterFromLegacy AndAlso TypeOf layers(layerIdx) Is BaseLayer AndAlso DirectCast(layers(layerIdx), BaseLayer).getIUpdater() Is Nothing Then
							handleUpdaterBackwardCompatibility(DirectCast(layers(layerIdx), BaseLayer), CType([next], ObjectNode))
						End If

						If requireLegacyRegularizationHandling AndAlso TypeOf layers(layerIdx) Is BaseLayer AndAlso DirectCast(layers(layerIdx), BaseLayer).getRegularization() Is Nothing Then
							handleL1L2BackwardCompatibility(DirectCast(layers(layerIdx), BaseLayer), CType([next], ObjectNode))
						End If

						If requiresLegacyWeightInitHandling AndAlso TypeOf layers(layerIdx) Is BaseLayer AndAlso DirectCast(layers(layerIdx), BaseLayer).getWeightInitFn() Is Nothing Then
							handleWeightInitBackwardCompatibility(DirectCast(layers(layerIdx), BaseLayer), CType([next], ObjectNode))
						End If

						If requiresLegacyActivationHandling AndAlso TypeOf layers(layerIdx) Is BaseLayer AndAlso DirectCast(layers(layerIdx), BaseLayer).getActivationFn() Is Nothing Then
							handleActivationBackwardCompatibility(DirectCast(layers(layerIdx), BaseLayer), CType([next], ObjectNode))
						End If

						If requiresLegacyLossHandling AndAlso TypeOf layers(layerIdx) Is BaseOutputLayer AndAlso DirectCast(layers(layerIdx), BaseOutputLayer).getLossFn() Is Nothing Then
							handleLossBackwardCompatibility(DirectCast(layers(layerIdx), BaseOutputLayer), CType([next], ObjectNode))
						End If

						If layers(layerIdx).getIDropout() Is Nothing Then
							'Check for legacy dropout
							If [next].has("dropOut") Then
								Dim d As Double = [next].get("dropOut").asDouble()
								If Not Double.IsNaN(d) Then
									'Might be dropout or dropconnect...
									If TypeOf layers(layerIdx) Is BaseLayer AndAlso confNode.has("useDropConnect") AndAlso confNode.get("useDropConnect").asBoolean(False) Then
										DirectCast(layers(layerIdx), BaseLayer).setWeightNoise(New DropConnect(d))
									Else
										layers(layerIdx).setIDropout(New Dropout(d))
									End If
								End If
							End If
						End If
						layerIdx += 1
					ElseIf "org.deeplearning4j.nn.conf.graph.LayerVertex".Equals(cls) Then
						If requiresLegacyWeightInitHandling AndAlso TypeOf layers(layerIdx) Is BaseLayer AndAlso DirectCast(layers(layerIdx), BaseLayer).getWeightInitFn() Is Nothing Then
							'Post JSON format change for subclasses, but before WeightInit was made a class
							confNode = CType([next].get("layerConf"), ObjectNode)
							[next] = confNode.get("layer")
							handleWeightInitBackwardCompatibility(DirectCast(layers(layerIdx), BaseLayer), CType([next], ObjectNode))
						End If
						layerIdx += 1
					End If
				Loop
			End If

			'After 1.0.0-beta3, batchnorm reparameterized to support both variance and log10stdev
			'JSON deserialization uses public BatchNormalization() constructor which defaults to log10stdev now
			' but, as there is no useLogStdev=false property for legacy batchnorm JSON, the 'real' value (useLogStdev=false)
			' is not set to override the default, unless we do it manually here
			For Each gv As GraphVertex In conf.getVertices().values()
				If TypeOf gv Is LayerVertex AndAlso TypeOf (DirectCast(gv, LayerVertex)).getLayerConf().getLayer() Is BatchNormalization Then
					Dim bn As BatchNormalization = CType(DirectCast(gv, LayerVertex).getLayerConf().getLayer(), BatchNormalization)
					Dim vars As IList(Of String) = DirectCast(gv, LayerVertex).getLayerConf().getVariables()
					Dim isVariance As Boolean = vars.Contains(BatchNormalizationParamInitializer.GLOBAL_VAR)
					bn.setUseLogStd(Not isVariance)
				End If
			Next gv

			Return conf
		End Function
	End Class

End Namespace