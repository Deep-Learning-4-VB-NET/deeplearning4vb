Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports Model = org.deeplearning4j.nn.api.Model
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Hdf5Archive = org.deeplearning4j.nn.modelimport.keras.Hdf5Archive
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModelConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasModelConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasBidirectional = org.deeplearning4j.nn.modelimport.keras.layers.wrappers.KerasBidirectional
Imports ReshapePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports TypeReference = org.nd4j.shade.jackson.core.type.TypeReference
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports YAMLFactory = org.nd4j.shade.jackson.dataformat.yaml.YAMLFactory

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

Namespace org.deeplearning4j.nn.modelimport.keras.utils



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasModelUtils
	Public Class KerasModelUtils


		''' <summary>
		''' Set the <seealso cref="org.deeplearning4j.nn.conf.DataFormat"/>
		''' for certain input preprocessors to ensure that
		''' model import propagates properly for cases like reshapes.
		''' </summary>
		''' <param name="inputPreProcessor"> </param>
		''' <param name="currLayer"> </param>
		Public Shared Sub setDataFormatIfNeeded(ByVal inputPreProcessor As InputPreProcessor, ByVal currLayer As KerasLayer)
			If TypeOf inputPreProcessor Is ReshapePreprocessor Then
				Dim reshapePreprocessor As ReshapePreprocessor = DirectCast(inputPreProcessor, ReshapePreprocessor)
				If currLayer.isLayer() Then
					If currLayer.DimOrder <> Nothing Then
						Dim layer As Layer = currLayer.Layer
						If TypeOf layer Is ConvolutionLayer Then
							Dim convolutionLayer As ConvolutionLayer = DirectCast(layer, ConvolutionLayer)
							If TypeOf convolutionLayer Is Convolution3D Then
								Dim convolution3D As Convolution3D = DirectCast(convolutionLayer, Convolution3D)
								reshapePreprocessor.setFormat(convolution3D.getDataFormat())
							ElseIf TypeOf convolutionLayer Is Deconvolution3D Then
							   Dim deconvolution3D As Deconvolution3D = DirectCast(convolutionLayer, Deconvolution3D)
							   reshapePreprocessor.setFormat(deconvolution3D.getDataFormat())
							Else
								reshapePreprocessor.setFormat(convolutionLayer.getCnn2dDataFormat())
							End If
						ElseIf TypeOf layer Is BaseRecurrentLayer Then
							Dim baseRecurrentLayer As BaseRecurrentLayer = DirectCast(layer, BaseRecurrentLayer)
							reshapePreprocessor.setFormat(baseRecurrentLayer.getRnnDataFormat())
						End If
					End If
				End If
			End If

		End Sub


		''' <summary>
		''' Helper function to import weights from nested Map into existing model. Depends critically
		''' on matched layer and parameter names. In general this seems to be straightforward for most
		''' Keras models and layersOrdered, but there may be edge cases.
		''' </summary>
		''' <param name="model"> DL4J Model interface </param>
		''' <returns> DL4J Model interface </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.api.Model copyWeightsToModel(org.deeplearning4j.nn.api.Model model, Map<String, org.deeplearning4j.nn.modelimport.keras.KerasLayer> kerasLayers) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function copyWeightsToModel(ByVal model As Model, ByVal kerasLayers As IDictionary(Of String, KerasLayer)) As Model
			' Get list if layers from model. 
			Dim layersFromModel() As org.deeplearning4j.nn.api.Layer
			If TypeOf model Is MultiLayerNetwork Then
				layersFromModel = DirectCast(model, MultiLayerNetwork).Layers
			Else
				layersFromModel = DirectCast(model, ComputationGraph).Layers
			End If

			' Iterate over layers in model, setting weights when relevant. 
			Dim layerNames As ISet(Of String) = New HashSet(Of String)(kerasLayers.Keys)
			For Each layer As org.deeplearning4j.nn.api.Layer In layersFromModel
				Dim layerName As String = layer.conf().getLayer().getLayerName()
				If Not kerasLayers.ContainsKey(layerName) Then
					Throw New InvalidKerasConfigurationException("No weights found for layer in model (named " & layerName & ")")
				End If
				kerasLayers(layerName).copyWeightsToLayer(layer)
				layerNames.remove(layerName)
			Next layer

			For Each layerName As String In layerNames
				If kerasLayers(layerName).getNumParams() > 0 Then
					Throw New InvalidKerasConfigurationException("Attempting to copy weights for layer not in model (named " & layerName & ")")
				End If
			Next layerName
			Return model
		End Function

		''' <summary>
		''' Determine Keras major version
		''' </summary>
		''' <param name="modelConfig"> parsed model configuration for keras model </param>
		''' <param name="config">      basic model configuration (KerasModelConfiguration) </param>
		''' <returns> Major Keras version (1 or 2) </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int determineKerasMajorVersion(Map<String, Object> modelConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasModelConfiguration config) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function determineKerasMajorVersion(ByVal modelConfig As IDictionary(Of String, Object), ByVal config As KerasModelConfiguration) As Integer
			Dim kerasMajorVersion As Integer
			If Not modelConfig.ContainsKey(config.getFieldKerasVersion()) Then
				log.warn("Could not read keras version used (no " & config.getFieldKerasVersion() & " field found) " & vbLf & "assuming keras version is 1.0.7 or earlier.")
				kerasMajorVersion = 1
			Else
				Dim kerasVersionString As String = DirectCast(modelConfig(config.getFieldKerasVersion()), String)
				If Char.IsDigit(kerasVersionString.Chars(0)) Then
					kerasMajorVersion = CInt(Math.Truncate(Char.GetNumericValue(kerasVersionString.Chars(0))))
				Else
					Throw New InvalidKerasConfigurationException("Keras version was not readable (" & config.getFieldKerasVersion() & " provided)")
				End If
			End If
			Return kerasMajorVersion
		End Function

		''' <summary>
		''' Determine Keras backend
		''' </summary>
		''' <param name="modelConfig"> parsed model configuration for keras model </param>
		''' <param name="config">      basic model configuration (KerasModelConfiguration) </param>
		''' <returns> Keras backend string </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
		Public Shared Function determineKerasBackend(ByVal modelConfig As IDictionary(Of String, Object), ByVal config As KerasModelConfiguration) As String
			Dim kerasBackend As String = Nothing
			If Not modelConfig.ContainsKey(config.getFieldBackend()) Then
				' TODO: H5 files unfortunately do not seem to have this property in keras 1.
				log.warn("Could not read keras backend used (no " & config.getFieldBackend() & " field found) " & vbLf)
			Else
				kerasBackend = DirectCast(modelConfig(config.getFieldBackend()), String)
			End If
			Return kerasBackend
		End Function

		Private Shared Function findParameterName(ByVal parameter As String, ByVal fragmentList() As String) As String
			Dim layerNameMatcher As Matcher = Pattern.compile(fragmentList(fragmentList.Length - 1)).matcher(parameter)
			Dim parameterNameFound As String = layerNameMatcher.replaceFirst("")

			' Usually layer name is separated from parameter name by an underscore. 
			Dim paramNameMatcher As Matcher = Pattern.compile("^_(.+)$").matcher(parameterNameFound)
			If paramNameMatcher.find() Then
				parameterNameFound = paramNameMatcher.group(1)
			End If

			' TensorFlow backend often appends ":" followed by one or more digits to parameter names. 
			Dim tfSuffixMatcher As Matcher = Pattern.compile(":\d+?$").matcher(parameterNameFound)
			If tfSuffixMatcher.find() Then
				parameterNameFound = tfSuffixMatcher.replaceFirst("")
			End If

			' TensorFlow backend also may append "_" followed by one or more digits to parameter names.
			Dim tfParamNbMatcher As Matcher = Pattern.compile("_\d+$").matcher(parameterNameFound)
			If tfParamNbMatcher.find() Then
				parameterNameFound = tfParamNbMatcher.replaceFirst("")
			End If

			Return parameterNameFound
		End Function

		''' <summary>
		''' Store weights to import with each associated Keras layer.
		''' </summary>
		''' <param name="weightsArchive"> Hdf5Archive </param>
		''' <param name="weightsRoot">    root of weights in HDF5 archive </param>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void importWeights(org.deeplearning4j.nn.modelimport.keras.Hdf5Archive weightsArchive, String weightsRoot, Map<String, org.deeplearning4j.nn.modelimport.keras.KerasLayer> layers, int kerasVersion, String backend) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Sub importWeights(ByVal weightsArchive As Hdf5Archive, ByVal weightsRoot As String, ByVal layers As IDictionary(Of String, KerasLayer), ByVal kerasVersion As Integer, ByVal backend As String)
			' check to ensure naming scheme doesn't include forward slash
			Dim includesSlash As Boolean = False
			For Each layerName As String In layers.Keys
				If layerName.Contains("/") Then
					includesSlash = True
				End If
			Next layerName
			SyncLock GetType(KerasModelUtils)
				Dim layerGroups As IList(Of String)
				If Not includesSlash Then
					layerGroups = If(weightsRoot IsNot Nothing, weightsArchive.getGroups(weightsRoot), weightsArchive.getGroups())
				Else
					layerGroups = New List(Of String)(layers.Keys)
				End If
				' Set weights in KerasLayer for each entry in weights map. 
				For Each layerName As String In layerGroups
					Dim layerParamNames As IList(Of String)

					' there's a bug where if a layer name contains a forward slash, the first fragment must be appended
					' to the name of the dataset; it appears h5 interprets the forward slash as a data group
					Dim layerFragments() As String = layerName.Split("/", True)

					' Find nested groups when using Tensorflow
					Dim rootPrefix As String = If(weightsRoot IsNot Nothing, weightsRoot & "/", "")
					Dim attributeStrParts As IList(Of String) = New List(Of String)()
					Dim attributeStr As String = weightsArchive.readAttributeAsString("weight_names", rootPrefix & layerName)
					Dim attributeJoinStr As String
					Dim attributeMatcher As Matcher = Pattern.compile(":\d+").matcher(attributeStr)
					Dim foundTfGroups As Boolean? = attributeMatcher.find()

					If foundTfGroups Then
						For Each part As String In attributeStr.Split("/", True)
							part = part.Trim()
							If part.Length = 0 Then
								Exit For
							End If
							Dim tfSuffixMatcher As Matcher = Pattern.compile(":\d+").matcher(part)
							If tfSuffixMatcher.find() Then
								Exit For
							End If
							attributeStrParts.Add(part)
						Next part
						attributeJoinStr = StringUtils.join(attributeStrParts, "/")
					Else
						attributeJoinStr = layerFragments(0)
					End If

					Dim baseAttributes As String = layerName & "/" & attributeJoinStr
					If layerFragments.Length > 1 Then
						Try
							layerParamNames = weightsArchive.getDataSets(rootPrefix & baseAttributes)
						Catch e As Exception
							layerParamNames = weightsArchive.getDataSets(rootPrefix & layerName)
						End Try
					Else
						If foundTfGroups Then
							layerParamNames = weightsArchive.getDataSets(rootPrefix & baseAttributes)
						Else
							If kerasVersion = 2 Then
								If backend.Equals("theano") AndAlso layerName.Contains("bidirectional") Then
									For Each part As String In attributeStr.Split("/", True)
										If part.Contains("forward") Then
											baseAttributes = baseAttributes & "/" & part
										End If
									Next part

								End If
								If layers(layerName).getNumParams() > 0 Then
									Try
										layerParamNames = weightsArchive.getDataSets(rootPrefix & baseAttributes)
									Catch e As Exception
										log.warn("No HDF5 group with weights found for layer with name " & layerName & ", continuing import.")
										layerParamNames = java.util.Collections.emptyList()
									End Try
								Else
									layerParamNames = weightsArchive.getDataSets(rootPrefix & layerName)
								End If

							Else
								layerParamNames = weightsArchive.getDataSets(rootPrefix & layerName)
							End If

						End If
					End If
					If layerParamNames.Count = 0 Then
						Continue For
					End If
					If Not layers.ContainsKey(layerName) Then
						Throw New InvalidKerasConfigurationException("Found weights for layer not in model (named " & layerName & ")")
					End If
					Dim layer As KerasLayer = layers(layerName)


					If layerParamNames.Count <> layer.NumParams Then
						If kerasVersion = 2 AndAlso TypeOf layer Is KerasBidirectional AndAlso 2 * layerParamNames.Count <> layer.NumParams Then
							Throw New InvalidKerasConfigurationException("Found " & layerParamNames.Count & " weights for layer with " & layer.NumParams & " trainable params (named " & layerName & ")")
						End If
					End If
					Dim weights As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()


					For Each layerParamName As String In layerParamNames
						Dim paramName As String = KerasModelUtils.findParameterName(layerParamName, layerFragments)
						Dim paramValue As INDArray

						If kerasVersion = 2 AndAlso TypeOf layer Is KerasBidirectional Then
							Dim backwardAttributes As String = baseAttributes.Replace("forward", "backward")
							Dim forwardParamValue As INDArray = weightsArchive.readDataSet(layerParamName, rootPrefix & baseAttributes)
							Dim backwardParamValue As INDArray = weightsArchive.readDataSet(layerParamName, rootPrefix & backwardAttributes)
							weights("forward_" & paramName) = forwardParamValue
							weights("backward_" & paramName) = backwardParamValue
						Else
							If foundTfGroups Then
								paramValue = weightsArchive.readDataSet(layerParamName, rootPrefix & baseAttributes)
							Else
								If layerFragments.Length > 1 Then
									paramValue = weightsArchive.readDataSet(layerFragments(0) & "/" & layerParamName, rootPrefix, layerName)
								Else
									If kerasVersion = 2 Then
										paramValue = weightsArchive.readDataSet(layerParamName, rootPrefix & baseAttributes)
									Else
										paramValue = weightsArchive.readDataSet(layerParamName, rootPrefix, layerName)
									End If
								End If
							End If
							weights(paramName) = paramValue
						End If
					Next layerParamName
					layer.Weights = weights
				Next layerName

				' Look for layers in model with no corresponding entries in weights map. 
				Dim layerNames As ISet(Of String) = New HashSet(Of String)(layers.Keys)
				layerNames.RemoveAll(layerGroups)
				For Each layerName As String In layerNames
					If layers(layerName).getNumParams() > 0 Then
						Throw New InvalidKerasConfigurationException("Could not find weights required for layer " & layerName)
					End If
				Next layerName
			End SyncLock
		End Sub

		''' <summary>
		''' Parse Keras model configuration from JSON or YAML string representation
		''' </summary>
		''' <param name="modelJson"> JSON string representing model (potentially null) </param>
		''' <param name="modelYaml"> YAML string representing model (potentially null) </param>
		''' <returns> Model configuration as Map<String, Object> </returns>
		''' <exception cref="IOException">                        IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Map<String, Object> parseModelConfig(String modelJson, String modelYaml) throws IOException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function parseModelConfig(ByVal modelJson As String, ByVal modelYaml As String) As IDictionary(Of String, Object)
			Dim modelConfig As IDictionary(Of String, Object)
			If modelJson IsNot Nothing Then
				modelConfig = parseJsonString(modelJson)
			ElseIf modelYaml IsNot Nothing Then
				modelConfig = parseYamlString(modelYaml)
			Else
				Throw New InvalidKerasConfigurationException("Requires model configuration as either JSON or YAML string.")
			End If
			Return modelConfig
		End Function


		''' <summary>
		''' Convenience function for parsing JSON strings.
		''' </summary>
		''' <param name="json"> String containing valid JSON </param>
		''' <returns> Nested (key,value) map of arbitrary depth </returns>
		''' <exception cref="IOException"> IO exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Map<String, Object> parseJsonString(String json) throws java.io.IOException
		Public Shared Function parseJsonString(ByVal json As String) As IDictionary(Of String, Object)
			Dim mapper As New ObjectMapper()
			Dim typeRef As TypeReference(Of Dictionary(Of String, Object)) = New TypeReferenceAnonymousInnerClass()
			Return mapper.readValue(json, typeRef)
		End Function

		Private Class TypeReferenceAnonymousInnerClass
			Inherits TypeReference(Of Dictionary(Of String, Object))

		End Class

		''' <summary>
		''' Convenience function for parsing YAML strings.
		''' </summary>
		''' <param name="yaml"> String containing valid YAML </param>
		''' <returns> Nested (key,value) map of arbitrary depth </returns>
		''' <exception cref="IOException"> IO exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Map<String, Object> parseYamlString(String yaml) throws java.io.IOException
		Public Shared Function parseYamlString(ByVal yaml As String) As IDictionary(Of String, Object)
			Dim mapper As New ObjectMapper(New YAMLFactory())
			Dim typeRef As TypeReference(Of Dictionary(Of String, Object)) = New TypeReferenceAnonymousInnerClass2()
			Return mapper.readValue(yaml, typeRef)
		End Function

		Private Class TypeReferenceAnonymousInnerClass2
			Inherits TypeReference(Of Dictionary(Of String, Object))

		End Class

	End Class

End Namespace