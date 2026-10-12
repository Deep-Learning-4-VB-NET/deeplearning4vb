Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasInput = org.deeplearning4j.nn.modelimport.keras.layers.KerasInput
Imports KerasModelBuilder = org.deeplearning4j.nn.modelimport.keras.utils.KerasModelBuilder
Imports KerasModelUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasModelUtils
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasModelUtils.importWeights

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
'ORIGINAL LINE: @Slf4j public class KerasSequentialModel extends KerasModel
	Public Class KerasSequentialModel
		Inherits KerasModel

		''' <summary>
		''' (Recommended) Builder-pattern constructor for Sequential model.
		''' </summary>
		''' <param name="modelBuilder"> builder object </param>
		''' <exception cref="IOException">                            I/O exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras configuration </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSequentialModel(org.deeplearning4j.nn.modelimport.keras.utils.KerasModelBuilder modelBuilder) throws UnsupportedKerasConfigurationException, IOException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Sub New(ByVal modelBuilder As KerasModelBuilder)
			Me.New(modelBuilder.getModelJson(), modelBuilder.getModelYaml(), modelBuilder.getWeightsArchive(), modelBuilder.getWeightsRoot(), modelBuilder.getTrainingJson(), modelBuilder.getTrainingArchive(), modelBuilder.isEnforceTrainingConfig(), modelBuilder.getInputShape())
		End Sub

		''' <summary>
		''' (Not recommended) Constructor for Sequential model from model configuration
		''' (JSON or YAML), training configuration (JSON), weights, and "training mode"
		''' boolean indicator. When built in training mode, certain unsupported configurations
		''' (e.g., unknown regularizers) will throw Exceptions. When enforceTrainingConfig=false, these
		''' will generate warnings but will be otherwise ignored.
		''' </summary>
		''' <param name="modelJson">    model configuration JSON string </param>
		''' <param name="modelYaml">    model configuration YAML string </param>
		''' <param name="trainingJson"> training configuration JSON string </param>
		''' <exception cref="IOException"> I/O exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSequentialModel(String modelJson, String modelYaml, Hdf5Archive weightsArchive, String weightsRoot, String trainingJson, Hdf5Archive trainingArchive, boolean enforceTrainingConfig, int[] inputShape) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal modelJson As String, ByVal modelYaml As String, ByVal weightsArchive As Hdf5Archive, ByVal weightsRoot As String, ByVal trainingJson As String, ByVal trainingArchive As Hdf5Archive, ByVal enforceTrainingConfig As Boolean, ByVal inputShape() As Integer)

			Dim modelConfig As IDictionary(Of String, Object) = KerasModelUtils.parseModelConfig(modelJson, modelYaml)
			Me.kerasMajorVersion = KerasModelUtils.determineKerasMajorVersion(modelConfig, config)
			Me.kerasBackend = KerasModelUtils.determineKerasBackend(modelConfig, config)
			Me.enforceTrainingConfig = enforceTrainingConfig

			' Determine model configuration type. 
			If Not modelConfig.ContainsKey(config.getFieldClassName()) Then
				Throw New InvalidKerasConfigurationException("Could not determine Keras model class (no " & config.getFieldClassName() & " field found)")
			End If
			Me.className = DirectCast(modelConfig(config.getFieldClassName()), String)
			If Not Me.className.Equals(config.getFieldClassNameSequential()) Then
				Throw New InvalidKerasConfigurationException("Model class name must be " & config.getFieldClassNameSequential() & " (found " & Me.className & ")")
			End If

			' Process layer configurations. 
			If Not modelConfig.ContainsKey(config.getModelFieldConfig()) Then
				Throw New InvalidKerasConfigurationException("Could not find layer configurations (no " & config.getModelFieldConfig() & " field found)")
			End If

			' Prior to Keras 2.2.3 the "config" of a Sequential model was a list of layer configurations. For consistency
			' "config" is now an object containing a "name" and "layers", the latter contain the same data as before.
			' This change only affects Sequential models.
			Dim layerList As IList(Of Object)
			Try
				layerList = DirectCast(modelConfig(config.getModelFieldConfig()), IList(Of Object))
			Catch e As Exception
				Dim layerMap As Hashtable = DirectCast(modelConfig(config.getModelFieldConfig()), Dictionary(Of String, Object))
				layerList = CType(layerMap("layers"), IList(Of Object))
			End Try

			Dim layerPair As Pair(Of IDictionary(Of String, KerasLayer), IList(Of KerasLayer)) = prepareLayers(layerList)
			Me.layers = layerPair.First
			Me.layersOrdered = layerPair.Second

			Dim inputLayer As KerasLayer
			If TypeOf Me.layersOrdered(0) Is KerasInput Then
				inputLayer = Me.layersOrdered(0)
			Else
				' Add placeholder input layer and update lists of input and output layers. 
				Dim firstLayerInputShape() As Integer = Me.layersOrdered(0).getInputShape()
				Preconditions.checkState(ArrayUtil.prod(firstLayerInputShape) > 0,"Input shape must not be zero!")
				inputLayer = New KerasInput("input1", firstLayerInputShape)
				inputLayer.DimOrder = Me.layersOrdered(0).getDimOrder()
				Me.layers(inputLayer.LayerName) = inputLayer
				Me.layersOrdered.Insert(0, inputLayer)
			End If
			Me.inputLayerNames = New List(Of String)(Collections.singletonList(inputLayer.LayerName))
			Me.outputLayerNames = New List(Of String)(Collections.singletonList(Me.layersOrdered(Me.layersOrdered.Count - 1).getLayerName()))

			' Update each layer's inbound layer list to include (only) previous layer. 
			Dim prevLayer As KerasLayer = Nothing
			For Each layer As KerasLayer In Me.layersOrdered
				If prevLayer IsNot Nothing Then
					layer.InboundLayerNames = Collections.singletonList(prevLayer.LayerName)
				End If
				prevLayer = layer
			Next layer

			' Import training configuration. 
			If enforceTrainingConfig Then
				If trainingJson IsNot Nothing Then
					importTrainingConfiguration(trainingJson)
				Else
					log.warn("If enforceTrainingConfig is true, a training " & "configuration object has to be provided. Usually the only practical way to do this is to store" & " your keras model with `model.save('model_path.h5'. If you store model config and weights" & " separately no training configuration is attached.")
				End If
			End If

			Me.outputTypes = inferOutputTypes(inputShape)

			If weightsArchive IsNot Nothing Then
				importWeights(weightsArchive, weightsRoot, layers, kerasMajorVersion, kerasBackend)
			End If
		End Sub

		''' <summary>
		''' Default constructor
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Configure a MultiLayerConfiguration from this Keras Sequential model configuration.
		''' </summary>
		''' <returns> MultiLayerConfiguration </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.conf.MultiLayerConfiguration getMultiLayerConfiguration() throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable ReadOnly Property MultiLayerConfiguration As MultiLayerConfiguration
			Get
				If Not Me.className.Equals(config.getFieldClassNameSequential()) Then
					Throw New InvalidKerasConfigurationException("Keras model class name " & Me.className & " incompatible with MultiLayerNetwork")
				End If
				If Me.inputLayerNames.Count <> 1 Then
					Throw New InvalidKerasConfigurationException("MultiLayerNetwork expects only 1 input (found " & Me.inputLayerNames.Count & ")")
				End If
				If Me.outputLayerNames.Count <> 1 Then
					Throw New InvalidKerasConfigurationException("MultiLayerNetwork expects only 1 output (found " & Me.outputLayerNames.Count & ")")
				End If
    
				Dim modelBuilder As New NeuralNetConfiguration.Builder()
    
				If optimizer IsNot Nothing Then
					modelBuilder.updater(optimizer)
				End If
    
				Dim listBuilder As NeuralNetConfiguration.ListBuilder = modelBuilder.list()
				'don't forcibly over ride for keras import
				listBuilder.overrideNinUponBuild(False)
				' Add layers one at a time. 
				Dim prevLayer As KerasLayer = Nothing
				Dim layerIndex As Integer = 0
				For Each layer As KerasLayer In Me.layersOrdered
					If layer.isLayer() Then
						Dim nbInbound As Integer = layer.getInboundLayerNames().Count
						If nbInbound <> 1 Then
							Throw New InvalidKerasConfigurationException("Layers in MultiLayerConfiguration must have exactly one inbound layer (found " & nbInbound & " for layer " & layer.LayerName & ")")
						End If
						If prevLayer IsNot Nothing Then
							Dim inputTypes(0) As InputType
							Dim preprocessor As InputPreProcessor
							If prevLayer.InputPreProcessor Then
								inputTypes(0) = Me.outputTypes(prevLayer.getInboundLayerNames()(0))
								preprocessor = prevLayer.getInputPreprocessor(inputTypes)
								KerasModelUtils.setDataFormatIfNeeded(preprocessor,layer)
								Dim outputType As InputType = preprocessor.getOutputType(inputTypes(0))
								layer.Layer.setNIn(outputType,listBuilder.isOverrideNinUponBuild())
							Else
								inputTypes(0) = Me.outputTypes(prevLayer.LayerName)
								preprocessor = layer.getInputPreprocessor(inputTypes)
								If preprocessor IsNot Nothing Then
									Dim outputType As InputType = preprocessor.getOutputType(inputTypes(0))
									layer.Layer.setNIn(outputType,listBuilder.isOverrideNinUponBuild())
								Else
									layer.Layer.setNIn(inputTypes(0),listBuilder.isOverrideNinUponBuild())
								End If
    
								KerasModelUtils.setDataFormatIfNeeded(preprocessor,layer)
    
							End If
							If preprocessor IsNot Nothing Then
								listBuilder.inputPreProcessor(layerIndex, preprocessor)
							End If
    
    
						End If
    
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: listBuilder.layer(layerIndex++, layer.getLayer());
						listBuilder.layer(layerIndex, layer.Layer)
							layerIndex += 1
					ElseIf layer.Vertex IsNot Nothing Then
						Throw New InvalidKerasConfigurationException("Cannot add vertex to MultiLayerConfiguration (class name " & layer.ClassName & ", layer name " & layer.LayerName & ")")
					End If
					prevLayer = layer
				Next layer
    
				' Whether to use standard backprop (or BPTT) or truncated BPTT. 
				If Me.useTruncatedBPTT AndAlso Me.truncatedBPTT > 0 Then
					listBuilder.backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(truncatedBPTT).tBPTTBackwardLength(truncatedBPTT)
				Else
					listBuilder.backpropType(BackpropType.Standard)
				End If
    
				Dim build As MultiLayerConfiguration = listBuilder.build()
    
    
				Return build
			End Get
		End Property

		''' <summary>
		''' Build a MultiLayerNetwork from this Keras Sequential model configuration.
		''' </summary>
		''' <returns> MultiLayerNetwork </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.multilayer.MultiLayerNetwork getMultiLayerNetwork() throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable ReadOnly Property MultiLayerNetwork As MultiLayerNetwork
			Get
				Return getMultiLayerNetwork(True)
			End Get
		End Property

		''' <summary>
		''' Build a MultiLayerNetwork from this Keras Sequential model configuration and import weights.
		''' </summary>
		''' <returns> MultiLayerNetwork </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.multilayer.MultiLayerNetwork getMultiLayerNetwork(boolean importWeights) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function getMultiLayerNetwork(ByVal importWeights As Boolean) As MultiLayerNetwork
			Dim model As New MultiLayerNetwork(MultiLayerConfiguration)
			model.init()
			If importWeights Then
				model = DirectCast(KerasModelUtils.copyWeightsToModel(model, Me.layers), MultiLayerNetwork)
			End If
			Return model
		End Function
	End Class

End Namespace