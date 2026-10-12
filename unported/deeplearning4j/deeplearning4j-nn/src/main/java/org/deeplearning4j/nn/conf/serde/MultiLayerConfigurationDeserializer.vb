Imports System.Collections.Generic
Imports IOUtils = org.apache.commons.io.IOUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
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
Imports ArrayNode = org.nd4j.shade.jackson.databind.node.ArrayNode
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


	Public Class MultiLayerConfigurationDeserializer
		Inherits BaseNetConfigDeserializer(Of MultiLayerConfiguration)

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public MultiLayerConfigurationDeserializer(org.nd4j.shade.jackson.databind.JsonDeserializer<?> defaultDeserializer)
		Public Sub New(ByVal defaultDeserializer As JsonDeserializer(Of T1))
			MyBase.New(defaultDeserializer, GetType(MultiLayerConfiguration))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.MultiLayerConfiguration deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext ctxt) throws java.io.IOException
		Public Overrides Function deserialize(ByVal jp As JsonParser, ByVal ctxt As DeserializationContext) As MultiLayerConfiguration
			Dim charOffsetStart As Long = jp.getCurrentLocation().getCharOffset()

			Dim conf As MultiLayerConfiguration = CType(defaultDeserializer.deserialize(jp, ctxt), MultiLayerConfiguration)
			Dim layers((conf.getConfs().size()) - 1) As Layer
			For i As Integer = 0 To layers.Length - 1
				layers(i) = conf.getConf(i).getLayer()
			Next i

			'Now, check if we need to manually handle IUpdater deserialization from legacy format
			Dim attemptIUpdaterFromLegacy As Boolean = requiresIUpdaterFromLegacy(layers)

			Dim requiresLegacyRegularizationHandling As Boolean = requiresRegularizationFromLegacy(layers)
			Dim requiresLegacyWeightInitHandling As Boolean = requiresWeightInitFromLegacy(layers)
			Dim requiresLegacyActivationHandling As Boolean = requiresActivationFromLegacy(layers)
			Dim requiresLegacyLossHandling As Boolean = Me.requiresLegacyLossHandling(layers)

			If attemptIUpdaterFromLegacy OrElse requiresLegacyRegularizationHandling OrElse requiresLegacyWeightInitHandling Then
				Dim endLocation As JsonLocation = jp.getCurrentLocation()
				Dim charOffsetEnd As Long = endLocation.getCharOffset()
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
				Dim jsonSubString As String = s.Substring(tempVar, CInt(charOffsetEnd) - (tempVar))

				Dim om As ObjectMapper = NeuralNetConfiguration.mapper()
				Dim rootNode As JsonNode = om.readTree(jsonSubString)

				Dim confsNode As ArrayNode = CType(rootNode.get("confs"), ArrayNode)

				For i As Integer = 0 To layers.Length - 1
					Dim [on] As ObjectNode = CType(confsNode.get(i), ObjectNode)
					Dim confNode As ObjectNode = Nothing
					If TypeOf layers(i) Is BaseLayer AndAlso DirectCast(layers(i), BaseLayer).getIUpdater() Is Nothing Then
						'layer -> (first/only child) -> updater
						If [on].has("layer") Then
							confNode = [on]
							[on] = CType([on].get("layer"), ObjectNode)
						Else
							Continue For
						End If
						[on] = CType([on].elements().next(), ObjectNode)

						handleUpdaterBackwardCompatibility(DirectCast(layers(i), BaseLayer), [on])
					End If

					If attemptIUpdaterFromLegacy Then
						If layers(i).getIDropout() Is Nothing Then
							'Check for legacy dropout/dropconnect
							If [on].has("dropOut") Then
								Dim d As Double = [on].get("dropOut").asDouble()
								If Not Double.IsNaN(d) Then
									'Might be dropout or dropconnect...
									If confNode IsNot Nothing AndAlso TypeOf layers(i) Is BaseLayer AndAlso confNode.has("useDropConnect") AndAlso confNode.get("useDropConnect").asBoolean(False) Then
										DirectCast(layers(i), BaseLayer).setWeightNoise(New DropConnect(d))
									Else
										If d > 0.0 Then
											layers(i).setIDropout(New Dropout(d))
										End If
									End If
								End If
							End If
						End If
					End If

					If requiresLegacyRegularizationHandling OrElse requiresLegacyWeightInitHandling OrElse requiresLegacyActivationHandling Then
						If [on].has("layer") Then
							'Legacy format
							Dim layerNode As ObjectNode = CType([on].get("layer"), ObjectNode)
							If layerNode.has("@class") Then
								'Later legacy format: class field for JSON subclass
								[on] = layerNode
							Else
								'Early legacy format: wrapper object for JSON subclass
								[on] = CType([on].get("layer").elements().next(), ObjectNode)
							End If
						End If
					End If

					If requiresLegacyRegularizationHandling AndAlso TypeOf layers(i) Is BaseLayer AndAlso DirectCast(layers(i), BaseLayer).getRegularization() Is Nothing Then
						handleL1L2BackwardCompatibility(DirectCast(layers(i), BaseLayer), [on])
					End If

					If requiresLegacyWeightInitHandling AndAlso TypeOf layers(i) Is BaseLayer AndAlso DirectCast(layers(i), BaseLayer).getWeightInitFn() Is Nothing Then
						handleWeightInitBackwardCompatibility(DirectCast(layers(i), BaseLayer), [on])
					End If

					If requiresLegacyActivationHandling AndAlso TypeOf layers(i) Is BaseLayer AndAlso DirectCast(layers(i), BaseLayer).getActivationFn() Is Nothing Then
						handleActivationBackwardCompatibility(DirectCast(layers(i), BaseLayer), [on])
					End If

					If requiresLegacyLossHandling AndAlso TypeOf layers(i) Is BaseOutputLayer AndAlso DirectCast(layers(i), BaseOutputLayer).getLossFn() Is Nothing Then
						handleLossBackwardCompatibility(DirectCast(layers(i), BaseOutputLayer), [on])
					End If
				Next i
			End If





			'After 1.0.0-beta3, batchnorm reparameterized to support both variance and log10stdev
			'JSON deserialization uses public BatchNormalization() constructor which defaults to log10stdev now
			' but, as there is no useLogStdev=false property for legacy batchnorm JSON, the 'real' value (useLogStdev=false)
			' is not set to override the default, unless we do it manually here
			For Each nnc As NeuralNetConfiguration In conf.getConfs()
				Dim l As Layer = nnc.getLayer()
				If TypeOf l Is BatchNormalization Then
					Dim bn As BatchNormalization = DirectCast(l, BatchNormalization)
					Dim vars As IList(Of String) = nnc.getVariables()
					Dim isVariance As Boolean = vars.Contains(BatchNormalizationParamInitializer.GLOBAL_VAR)
					bn.setUseLogStd(Not isVariance)
				End If
			Next nnc

			Return conf
		End Function
	End Class

End Namespace