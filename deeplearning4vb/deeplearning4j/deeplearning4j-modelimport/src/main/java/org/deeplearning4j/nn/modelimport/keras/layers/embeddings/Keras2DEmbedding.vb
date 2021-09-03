Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports EmbeddingLayer = org.deeplearning4j.nn.conf.layers.EmbeddingLayer
Imports EmbeddingSequenceLayer = org.deeplearning4j.nn.conf.layers.EmbeddingSequenceLayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasInitilizationUtils.getWeightInitFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils.getNOutFromConfig

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.embeddings


	''' <summary>
	''' Imports an Embedding layer from Keras.
	''' 
	''' @author dave@skymind.io
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class Keras2DEmbedding extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class Keras2DEmbedding
		Inherits KerasLayer

		Private ReadOnly NUM_TRAINABLE_PARAMS As Integer = 1
		Private zeroMasking As Boolean
		Private inputDim As Integer
		Private inputLength As Integer
		Private inferInputLength As Boolean


		''' <summary>
		''' Pass through constructor for unit tests
		''' </summary>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Keras2DEmbedding() throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Keras2DEmbedding(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Keras2DEmbedding(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)

			Me.inputDim = getInputDimFromConfig(layerConfig)
			Me.inputLength = getInputLengthFromConfig(layerConfig)
			Me.inferInputLength = Me.inputLength = 0
			If Me.inferInputLength Then
				Me.inputLength = 1 ' set dummy value, so shape inference works
			End If

			Me.zeroMasking = KerasLayerUtils.getZeroMaskingFromConfig(layerConfig, conf)
			If zeroMasking Then
				log.warn("Masking in keras and DL4J work differently. We do not completely support mask_zero flag " & "on Embedding layers. Zero Masking for the Embedding layer only works with unidirectional LSTM for now." & " If you want to have this behaviour for your imported model " & "in DL4J, apply masking as a pre-processing step to your input." & "See https://deeplearning4j.konduit.ai/models/recurrent#masking-one-to-many-many-to-one-and-sequence-classification for more on this.")
			End If

			Dim init As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_EMBEDDING_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)

			Dim embeddingConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_EMBEDDINGS_CONSTRAINT(), conf, kerasMajorVersion)
			Dim nOutFromConfig As Integer = getNOutFromConfig(layerConfig, conf)
			Dim builder As EmbeddingLayer.Builder = (New EmbeddingLayer.Builder()).name(Me.layerName_Conflict).nIn(inputDim).nOut(nOutFromConfig).dropOut(Me.dropout).activation(Activation.IDENTITY).weightInit(init).biasInit(0.0).l1(Me.weightL1Regularization).l2(Me.weightL2Regularization).hasBias(False)
			If embeddingConstraint IsNot Nothing Then
				builder.constrainWeights(embeddingConstraint)
			End If
			Me.layer_Conflict = builder.build()

			Me.inputShape_Conflict = New Integer(){inputDim, 1}
		End Sub

		''' <summary>
		''' Get DL4J Embedding Sequence layer.
		''' </summary>
		''' <returns> Embedding Sequence layer </returns>
		Public Overridable ReadOnly Property EmbeddingLayer As EmbeddingLayer
			Get
				Return DirectCast(Me.layer_Conflict, EmbeddingLayer)
			End Get
		End Property

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			' Check whether layer requires a preprocessor for this InputType. 
			Dim preprocessor As InputPreProcessor = getInputPreprocessor(inputType(0))
			If preprocessor IsNot Nothing Then
				Return Me.EmbeddingLayer.getOutputType(-1, preprocessor.getOutputType(inputType(0)))
			End If
			Return Me.EmbeddingLayer.getOutputType(-1, inputType(0))
		End Function

		''' <summary>
		''' Returns number of trainable parameters in layer.
		''' </summary>
		''' <returns> number of trainable parameters (1) </returns>
		Public Overrides ReadOnly Property NumParams As Integer
			Get
				Return NUM_TRAINABLE_PARAMS
			End Get
		End Property

		''' <summary>
		''' Set weights for layer.
		''' </summary>
		''' <param name="weights"> Embedding layer weights </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
				' TODO: "embeddings" is incorrectly read as "s" for some applications
				If weights.ContainsKey("s") Then
					Dim kernel As INDArray = weights("s")
					weights.Remove("s")
					weights(conf.getLAYER_FIELD_EMBEDDING_WEIGHTS()) = kernel
				End If
    
				If Not weights.ContainsKey(conf.getLAYER_FIELD_EMBEDDING_WEIGHTS()) Then
					Throw New InvalidKerasConfigurationException("Parameter " & conf.getLAYER_FIELD_EMBEDDING_WEIGHTS() & " does not exist in weights")
				End If
				Dim kernel As INDArray = weights(conf.getLAYER_FIELD_EMBEDDING_WEIGHTS())
				If Me.zeroMasking Then
					kernel.putRow(0, Nd4j.zeros(kernel.columns()))
				End If
				Me.weights_Conflict(DefaultParamInitializer.WEIGHT_KEY) = kernel
    
				If weights.Count > 2 Then
					Dim paramNames As ISet(Of String) = weights.Keys
					paramNames.remove(conf.getLAYER_FIELD_EMBEDDING_WEIGHTS())
					Dim unknownParamNames As String = paramNames.ToString()
					log.warn("Attempting to set weights for unknown parameters: " & unknownParamNames.Substring(1, (unknownParamNames.Length - 1) - 1))
				End If
			End Set
		End Property

		''' <summary>
		''' Get Keras input length from Keras layer configuration. In Keras input_length, if present, denotes
		''' the number of indices to embed per mini-batch, i.e. input will be of shape (mb, input_length)
		''' and (mb, 1) else.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> input length as int </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private int getInputLengthFromConfig(java.util.Map<String, Object> layerConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getInputLengthFromConfig(ByVal layerConfig As IDictionary(Of String, Object)) As Integer
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_INPUT_LENGTH()) Then
				Throw New InvalidKerasConfigurationException("Keras Embedding layer config missing " & conf.getLAYER_FIELD_INPUT_LENGTH() & " field")
			End If
			If innerConfig(conf.getLAYER_FIELD_INPUT_LENGTH()) Is Nothing Then
				Return 0
			Else
				Return DirectCast(innerConfig(conf.getLAYER_FIELD_INPUT_LENGTH()), Integer)
			End If
		End Function

		''' <summary>
		''' Get Keras input dimension from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> input dim as int </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private int getInputDimFromConfig(java.util.Map<String, Object> layerConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Function getInputDimFromConfig(ByVal layerConfig As IDictionary(Of String, Object)) As Integer
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_INPUT_DIM()) Then
				Throw New InvalidKerasConfigurationException("Keras Embedding layer config missing " & conf.getLAYER_FIELD_INPUT_DIM() & " field")
			End If
			Return DirectCast(innerConfig(conf.getLAYER_FIELD_INPUT_DIM()), Integer)
		End Function
	End Class

End Namespace