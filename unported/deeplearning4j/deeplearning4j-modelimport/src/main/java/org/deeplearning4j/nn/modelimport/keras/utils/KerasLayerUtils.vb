Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports SameDiffLambdaLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasInput = org.deeplearning4j.nn.modelimport.keras.layers.KerasInput
Imports KerasTFOpLayer = org.deeplearning4j.nn.modelimport.keras.layers.KerasTFOpLayer
Imports org.deeplearning4j.nn.modelimport.keras.layers.advanced.activations
Imports org.deeplearning4j.nn.modelimport.keras.layers.convolutional
Imports org.deeplearning4j.nn.modelimport.keras.layers.core
Imports Keras2DEmbedding = org.deeplearning4j.nn.modelimport.keras.layers.embeddings.Keras2DEmbedding
Imports KerasEmbedding = org.deeplearning4j.nn.modelimport.keras.layers.embeddings.KerasEmbedding
Imports KerasLocallyConnected1D = org.deeplearning4j.nn.modelimport.keras.layers.local.KerasLocallyConnected1D
Imports KerasAlphaDropout = org.deeplearning4j.nn.modelimport.keras.layers.noise.KerasAlphaDropout
Imports KerasGaussianDropout = org.deeplearning4j.nn.modelimport.keras.layers.noise.KerasGaussianDropout
Imports KerasGaussianNoise = org.deeplearning4j.nn.modelimport.keras.layers.noise.KerasGaussianNoise
Imports KerasBatchNormalization = org.deeplearning4j.nn.modelimport.keras.layers.normalization.KerasBatchNormalization
Imports KerasGlobalPooling = org.deeplearning4j.nn.modelimport.keras.layers.pooling.KerasGlobalPooling
Imports KerasPooling1D = org.deeplearning4j.nn.modelimport.keras.layers.pooling.KerasPooling1D
Imports KerasPooling2D = org.deeplearning4j.nn.modelimport.keras.layers.pooling.KerasPooling2D
Imports KerasPooling3D = org.deeplearning4j.nn.modelimport.keras.layers.pooling.KerasPooling3D
Imports KerasLSTM = org.deeplearning4j.nn.modelimport.keras.layers.recurrent.KerasLSTM
Imports KerasSimpleRnn = org.deeplearning4j.nn.modelimport.keras.layers.recurrent.KerasSimpleRnn
Imports KerasBidirectional = org.deeplearning4j.nn.modelimport.keras.layers.wrappers.KerasBidirectional
Imports org.nd4j.common.primitives
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives

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
'ORIGINAL LINE: @Slf4j public class KerasLayerUtils
	Public Class KerasLayerUtils

		''' <summary>
		''' Checks whether layer config contains unsupported options.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to use Keras training configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void checkForUnsupportedConfigurations(Map<String, Object> layerConfig, boolean enforceTrainingConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Sub checkForUnsupportedConfigurations(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal conf As KerasLayerConfiguration)
			getBiasL1RegularizationFromConfig(layerConfig, enforceTrainingConfig, conf)
			getBiasL2RegularizationFromConfig(layerConfig, enforceTrainingConfig, conf)
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_W_REGULARIZER()) Then
				checkForUnknownRegularizer(DirectCast(innerConfig(conf.getLAYER_FIELD_W_REGULARIZER()), IDictionary(Of String, Object)), enforceTrainingConfig, conf)
			End If
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_B_REGULARIZER()) Then
				checkForUnknownRegularizer(DirectCast(innerConfig(conf.getLAYER_FIELD_B_REGULARIZER()), IDictionary(Of String, Object)), enforceTrainingConfig, conf)
			End If
		End Sub

		''' <summary>
		''' Get L1 bias regularization (if any) from Keras bias regularization configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> L1 regularization strength (0.0 if none) </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static double getBiasL1RegularizationFromConfig(Map<String, Object> layerConfig, boolean enforceTrainingConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getBiasL1RegularizationFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal conf As KerasLayerConfiguration) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_B_REGULARIZER()) Then
				Dim regularizerConfig As IDictionary(Of String, Object) = DirectCast(innerConfig(conf.getLAYER_FIELD_B_REGULARIZER()), IDictionary(Of String, Object))
				If regularizerConfig IsNot Nothing AndAlso regularizerConfig.ContainsKey(conf.getREGULARIZATION_TYPE_L1()) Then
					Throw New UnsupportedKerasConfigurationException("L1 regularization for bias parameter not supported")
				End If
			End If
			Return 0.0
		End Function

		''' <summary>
		''' Get L2 bias regularization (if any) from Keras bias regularization configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> L1 regularization strength (0.0 if none) </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static double getBiasL2RegularizationFromConfig(Map<String, Object> layerConfig, boolean enforceTrainingConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Private Shared Function getBiasL2RegularizationFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal conf As KerasLayerConfiguration) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_B_REGULARIZER()) Then
				Dim regularizerConfig As IDictionary(Of String, Object) = DirectCast(innerConfig(conf.getLAYER_FIELD_B_REGULARIZER()), IDictionary(Of String, Object))
				If regularizerConfig IsNot Nothing AndAlso regularizerConfig.ContainsKey(conf.getREGULARIZATION_TYPE_L2()) Then
					Throw New UnsupportedKerasConfigurationException("L2 regularization for bias parameter not supported")
				End If
			End If
			Return 0.0
		End Function

		''' <summary>
		''' Check whether Keras weight regularization is of unknown type. Currently prints a warning
		''' since main use case for model import is inference, not further training. Unlikely since
		''' standard Keras weight regularizers are L1 and L2.
		''' </summary>
		''' <param name="regularizerConfig"> Map containing Keras weight reguarlization configuration </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void checkForUnknownRegularizer(Map<String, Object> regularizerConfig, boolean enforceTrainingConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Shared Sub checkForUnknownRegularizer(ByVal regularizerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal conf As KerasLayerConfiguration)
			If regularizerConfig IsNot Nothing Then
				For Each field As String In regularizerConfig.Keys
					If Not field.Equals(conf.getREGULARIZATION_TYPE_L1()) AndAlso Not field.Equals(conf.getREGULARIZATION_TYPE_L2()) AndAlso Not field.Equals(conf.getLAYER_FIELD_NAME()) AndAlso Not field.Equals(conf.getLAYER_FIELD_CLASS_NAME()) AndAlso Not field.Equals(conf.getLAYER_FIELD_CONFIG()) Then
						If enforceTrainingConfig Then
							Throw New UnsupportedKerasConfigurationException("Unknown regularization field " & field)
						Else
							log.warn("Ignoring unknown regularization field " & field)
						End If
					End If
				Next field
			End If
		End Sub


		''' <summary>
		''' Build KerasLayer from a Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> map containing Keras layer properties </param>
		''' <returns> KerasLayer </returns>
		''' <seealso cref= Layer </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.modelimport.keras.KerasLayer getKerasLayerFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, Map<String, @Class> customLayers, Map<String, org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer> lambdaLayers, Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function getKerasLayerFromConfig(Of T1 As KerasLayer)(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration, ByVal customLayers As IDictionary(Of String, [Class]), ByVal lambdaLayers As IDictionary(Of String, SameDiffLambdaLayer), ByVal previousLayers As IDictionary(Of T1)) As KerasLayer
			Return getKerasLayerFromConfig(layerConfig, False, conf, customLayers, lambdaLayers, previousLayers)
		End Function

		''' <summary>
		''' Build KerasLayer from a Keras layer configuration. Building layer with
		''' enforceTrainingConfig=true will throw exceptions for unsupported Keras
		''' options related to training (e.g., unknown regularizers). Otherwise
		''' we only generate warnings.
		''' </summary>
		''' <param name="layerConfig">           map containing Keras layer properties </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-only configurations </param>
		''' <returns> KerasLayer </returns>
		''' <seealso cref= Layer </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.modelimport.keras.KerasLayer getKerasLayerFromConfig(Map<String, Object> layerConfig, boolean enforceTrainingConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, Map<String, @Class> customLayers, Map<String, org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer> lambdaLayers, Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function getKerasLayerFromConfig(Of T1 As KerasLayer)(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal conf As KerasLayerConfiguration, ByVal customLayers As IDictionary(Of String, [Class]), ByVal lambdaLayers As IDictionary(Of String, SameDiffLambdaLayer), ByVal previousLayers As IDictionary(Of T1)) As KerasLayer
			Dim layerClassName As String = getClassNameFromConfig(layerConfig, conf)
			If layerClassName.Equals(conf.getLAYER_CLASS_NAME_TIME_DISTRIBUTED()) Then
				layerConfig = getTimeDistributedLayerConfig(layerConfig, conf)
				layerClassName = getClassNameFromConfig(layerConfig, conf)
			End If
			Dim layer As KerasLayer = Nothing
			If layerClassName.Equals(conf.getLAYER_CLASS_NAME_ACTIVATION()) Then
				layer = New KerasActivation(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_LEAKY_RELU()) Then
				layer = New KerasLeakyReLU(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_MASKING()) Then
				layer = New KerasMasking(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_THRESHOLDED_RELU()) Then
				layer = New KerasThresholdedReLU(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_PRELU()) Then
				layer = New KerasPReLU(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_DROPOUT()) Then
				layer = New KerasDropout(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_SPATIAL_DROPOUT_1D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_SPATIAL_DROPOUT_2D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_SPATIAL_DROPOUT_3D()) Then
				layer = New KerasSpatialDropout(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_ALPHA_DROPOUT()) Then
				layer = New KerasAlphaDropout(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_GAUSSIAN_DROPOUT()) Then
				layer = New KerasGaussianDropout(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_GAUSSIAN_NOISE()) Then
				layer = New KerasGaussianNoise(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_DENSE()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_TIME_DISTRIBUTED_DENSE()) Then
				layer = New KerasDense(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_BIDIRECTIONAL()) Then
				layer = New KerasBidirectional(layerConfig, enforceTrainingConfig, previousLayers)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_LSTM()) Then
				layer = New KerasLSTM(layerConfig, enforceTrainingConfig, previousLayers)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_SIMPLE_RNN()) Then
				layer = New KerasSimpleRnn(layerConfig, enforceTrainingConfig, previousLayers)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_CONVOLUTION_3D()) Then
				layer = New KerasConvolution3D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_CONVOLUTION_2D()) Then
				layer = New KerasConvolution2D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_DECONVOLUTION_2D()) Then
				layer = New KerasDeconvolution2D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_DECONVOLUTION_3D()) Then
				layer = New KerasDeconvolution3D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_CONVOLUTION_1D()) Then
				layer = New KerasConvolution1D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_ATROUS_CONVOLUTION_2D()) Then
				layer = New KerasAtrousConvolution2D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_ATROUS_CONVOLUTION_1D()) Then
				layer = New KerasAtrousConvolution1D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_DEPTHWISE_CONVOLUTION_2D()) Then
				layer = New KerasDepthwiseConvolution2D(layerConfig, previousLayers, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_SEPARABLE_CONVOLUTION_2D()) Then
				layer = New KerasSeparableConvolution2D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_MAX_POOLING_3D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_AVERAGE_POOLING_3D()) Then
				layer = New KerasPooling3D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_MAX_POOLING_2D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_AVERAGE_POOLING_2D()) Then
				layer = New KerasPooling2D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_MAX_POOLING_1D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_AVERAGE_POOLING_1D()) Then
				layer = New KerasPooling1D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_1D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_2D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_3D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_MAX_POOLING_1D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_MAX_POOLING_2D()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_GLOBAL_MAX_POOLING_3D()) Then
				layer = New KerasGlobalPooling(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_BATCHNORMALIZATION()) Then
				layer = New KerasBatchNormalization(layerConfig, enforceTrainingConfig, previousLayers)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_EMBEDDING()) Then
				layer = New KerasEmbedding(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_INPUT()) Then
				layer = New KerasInput(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_REPEAT()) Then
				layer = New KerasRepeatVector(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_PERMUTE()) Then
				layer = New KerasPermute(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_MERGE()) Then
				layer = New KerasMerge(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_ADD()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_ADD()) Then
				layer = New KerasMerge(layerConfig, ElementWiseVertex.Op.Add, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_SUBTRACT()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_FUNCTIONAL_SUBTRACT()) Then
				layer = New KerasMerge(layerConfig, ElementWiseVertex.Op.Subtract, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_AVERAGE()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_FUNCTIONAL_AVERAGE()) Then
				layer = New KerasMerge(layerConfig, ElementWiseVertex.Op.Average, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_MULTIPLY()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_FUNCTIONAL_MULTIPLY()) Then
				layer = New KerasMerge(layerConfig, ElementWiseVertex.Op.Product, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_MAXIMUM()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_FUNCTIONAL_MAXIMUM()) Then
				layer = New KerasMerge(layerConfig, ElementWiseVertex.Op.Max, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_CONCATENATE()) OrElse layerClassName.Equals(conf.getLAYER_CLASS_NAME_FUNCTIONAL_CONCATENATE()) Then
				layer = New KerasMerge(layerConfig, Nothing, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_FLATTEN()) Then
				layer = New KerasFlatten(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_RESHAPE()) Then
				layer = New KerasReshape(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_ZERO_PADDING_1D()) Then
				layer = New KerasZeroPadding1D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_ZERO_PADDING_2D()) Then
				layer = New KerasZeroPadding2D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_ZERO_PADDING_3D()) Then
				layer = New KerasZeroPadding3D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_UPSAMPLING_1D()) Then
				layer = New KerasUpsampling1D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_UPSAMPLING_2D()) Then
				layer = New KerasUpsampling2D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_UPSAMPLING_3D()) Then
				layer = New KerasUpsampling3D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_CROPPING_3D()) Then
				layer = New KerasCropping3D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_CROPPING_2D()) Then
				layer = New KerasCropping2D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_CROPPING_1D()) Then
				layer = New KerasCropping1D(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_LAMBDA()) Then
				Dim lambdaLayerName As String = KerasLayerUtils.getLayerNameFromConfig(layerConfig, conf)
				If Not lambdaLayers.ContainsKey(lambdaLayerName) AndAlso Not customLayers.ContainsKey(layerClassName) Then
					Throw New UnsupportedKerasConfigurationException("No SameDiff Lambda layer found for Lambda " & "layer " & lambdaLayerName & ". You can register a SameDiff Lambda layer using KerasLayer." & "registerLambdaLayer(lambdaLayerName, sameDiffLambdaLayer);")
				End If

				Dim lambdaLayer As SameDiffLambdaLayer = lambdaLayers(lambdaLayerName)
				If lambdaLayer IsNot Nothing Then
					layer = New KerasLambda(layerConfig, enforceTrainingConfig, lambdaLayer)
				End If
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_RELU()) Then
				layer = New KerasReLU(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_ELU()) Then
				layer = New KerasELU(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_SOFTMAX()) Then
				layer = New KerasSoftmax(layerConfig, enforceTrainingConfig)
			ElseIf layerClassName.Equals(conf.getLAYER_CLASS_NAME_LOCALLY_CONNECTED_1D()) Then
				layer = New KerasLocallyConnected1D(layerConfig, enforceTrainingConfig)
			ElseIf TypeOf conf Is Keras2LayerConfiguration Then
				Dim k2conf As Keras2LayerConfiguration = DirectCast(conf, Keras2LayerConfiguration)
				If layerClassName.Equals(k2conf.getTENSORFLOW_OP_LAYER()) Then
					layer = New KerasTFOpLayer(layerConfig, enforceTrainingConfig)
				End If
			End If
			If layer Is Nothing Then
				Dim customConfig As Type = customLayers(layerClassName)
				If customConfig Is Nothing Then
					Throw New UnsupportedKerasConfigurationException("Unsupported keras layer type " & layerClassName)
				End If
				Try
					Dim constructor As System.Reflection.ConstructorInfo = customConfig.GetConstructor(GetType(System.Collections.IDictionary))
					layer = CType(constructor.Invoke(layerConfig), KerasLayer)
				Catch e As Exception
					Throw New Exception("The keras custom class " & layerClassName & " needs to have a constructor with only Map<String,Object> as the argument. Please ensure this is defined.", e)
				End Try
			End If
			Return layer
		End Function

		''' <summary>
		''' Get Keras layer class name from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Keras layer class name </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String getClassNameFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getClassNameFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As String
			If Not layerConfig.ContainsKey(conf.getLAYER_FIELD_CLASS_NAME()) Then
				Throw New InvalidKerasConfigurationException("Field " & conf.getLAYER_FIELD_CLASS_NAME() & " missing from layer config")
			End If
			Return DirectCast(layerConfig(conf.getLAYER_FIELD_CLASS_NAME()), String)
		End Function

		''' <summary>
		''' Extract inner layer config from TimeDistributed configuration and merge
		''' it into the outer config.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras TimeDistributed configuration </param>
		''' <returns> Time distributed layer config </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Map<String, Object> getTimeDistributedLayerConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getTimeDistributedLayerConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As IDictionary(Of String, Object)
			If Not layerConfig.ContainsKey(conf.getLAYER_FIELD_CLASS_NAME()) Then
				Throw New InvalidKerasConfigurationException("Field " & conf.getLAYER_FIELD_CLASS_NAME() & " missing from layer config")
			End If
			If Not layerConfig(conf.getLAYER_FIELD_CLASS_NAME()).Equals(conf.getLAYER_CLASS_NAME_TIME_DISTRIBUTED()) Then
				Throw New InvalidKerasConfigurationException("Expected " & conf.getLAYER_CLASS_NAME_TIME_DISTRIBUTED() & " layer, found " & layerConfig(conf.getLAYER_FIELD_CLASS_NAME()))
			End If
			If Not layerConfig.ContainsKey(conf.getLAYER_FIELD_CONFIG()) Then
				Throw New InvalidKerasConfigurationException("Field " & conf.getLAYER_FIELD_CONFIG() & " missing from layer config")
			End If
			Dim outerConfig As IDictionary(Of String, Object) = getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim innerLayer As IDictionary(Of String, Object) = DirectCast(outerConfig(conf.getLAYER_FIELD_LAYER()), IDictionary(Of String, Object))
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = innerLayer(conf.getLAYER_FIELD_CLASS_NAME())
			Dim innerConfig As IDictionary(Of String, Object) = getInnerLayerConfigFromConfig(innerLayer, conf)
			innerConfig(conf.getLAYER_FIELD_NAME()) = outerConfig(conf.getLAYER_FIELD_NAME())
			outerConfig.PutAll(innerConfig)
			outerConfig.Remove(conf.getLAYER_FIELD_LAYER())
			Return layerConfig
		End Function

		''' <summary>
		''' Get inner layer config from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Inner layer config for a nested Keras layer configuration </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Map<String, Object> getInnerLayerConfigFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getInnerLayerConfigFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As IDictionary(Of String, Object)
			If Not layerConfig.ContainsKey(conf.getLAYER_FIELD_CONFIG()) Then
				Throw New InvalidKerasConfigurationException("Field " & conf.getLAYER_FIELD_CONFIG() & " missing from layer config")
			End If
			Return DirectCast(layerConfig(conf.getLAYER_FIELD_CONFIG()), IDictionary(Of String, Object))
		End Function

		''' <summary>
		''' Get layer name from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Keras layer name </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String getLayerNameFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getLayerNameFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As String
			If TypeOf conf Is Keras2LayerConfiguration Then
				Dim k2conf As Keras2LayerConfiguration = DirectCast(conf, Keras2LayerConfiguration)
				If getClassNameFromConfig(layerConfig, conf).Equals(DirectCast(conf, Keras2LayerConfiguration).getTENSORFLOW_OP_LAYER()) Then
					If Not layerConfig.ContainsKey(conf.getLAYER_FIELD_NAME()) Then
						Throw New InvalidKerasConfigurationException("Field " & conf.getLAYER_FIELD_NAME() & " missing from layer config")
					End If
					Return DirectCast(layerConfig(conf.getLAYER_FIELD_NAME()), String)
				End If
			End If

			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_NAME()) Then
				Throw New InvalidKerasConfigurationException("Field " & conf.getLAYER_FIELD_NAME() & " missing from layer config")
			End If
			Return DirectCast(innerConfig(conf.getLAYER_FIELD_NAME()), String)
		End Function

		''' <summary>
		''' Get Keras input shape from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> input shape array </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int[] getInputShapeFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getInputShapeFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Integer()
			' TODO: validate this. shouldn't we also have INPUT_SHAPE checked?
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_BATCH_INPUT_SHAPE()) Then
				Return Nothing
			End If
			Dim batchInputShape As IList(Of Integer) = DirectCast(innerConfig(conf.getLAYER_FIELD_BATCH_INPUT_SHAPE()), IList(Of Integer))
			Dim inputShape(batchInputShape.Count - 2) As Integer
			For i As Integer = 1 To batchInputShape.Count - 1
				inputShape(i - 1) = If(batchInputShape(i) <> Nothing, batchInputShape(i), 0)
			Next i
			Return inputShape
		End Function

		''' <summary>
		''' Get Keras (backend) dimension order from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Dimension order </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.modelimport.keras.KerasLayer.DimOrder getDimOrderFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getDimOrderFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As KerasLayer.DimOrder
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim dimOrder As KerasLayer.DimOrder = KerasLayer.DimOrder.NONE
			If layerConfig.ContainsKey(conf.getLAYER_FIELD_BACKEND()) Then
				Dim backend As String = DirectCast(layerConfig(conf.getLAYER_FIELD_BACKEND()), String)
				If backend.Equals("tensorflow") OrElse backend.Equals("cntk") Then
					dimOrder = KerasLayer.DimOrder.TENSORFLOW
				ElseIf backend.Equals("theano") Then
					dimOrder = KerasLayer.DimOrder.THEANO
				End If
			End If
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_DIM_ORDERING()) Then
				Dim dimOrderStr As String = DirectCast(innerConfig(conf.getLAYER_FIELD_DIM_ORDERING()), String)
				If dimOrderStr.Equals(conf.getDIM_ORDERING_TENSORFLOW()) Then
					dimOrder = KerasLayer.DimOrder.TENSORFLOW
				ElseIf dimOrderStr.Equals(conf.getDIM_ORDERING_THEANO()) Then
					dimOrder = KerasLayer.DimOrder.THEANO
				Else
					log.warn("Keras layer has unknown Keras dimension order: " & dimOrder)
				End If
			End If
			Return dimOrder
		End Function

		''' <summary>
		''' Get list of inbound layers from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> List of inbound layer names </returns>
		Public Shared Function getInboundLayerNamesFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As IList(Of String)
			Dim inboundLayerNames As IList(Of String) = New List(Of String)()
			If layerConfig.ContainsKey(conf.getLAYER_FIELD_INBOUND_NODES()) Then
				Dim inboundNodes As IList(Of Object) = DirectCast(layerConfig(conf.getLAYER_FIELD_INBOUND_NODES()), IList(Of Object))
				If inboundNodes.Count > 0 Then
					For Each nodeName As Object In inboundNodes
						Dim list As IList(Of Object) = DirectCast(nodeName, IList(Of Object))
						For Each o As Object In list
							Dim list2 As IList(Of Object) = DirectCast(o, IList(Of Object))
							inboundLayerNames.Add(list2(0).ToString())

						Next o
					Next nodeName


				End If


			End If
			Return inboundLayerNames
		End Function

		''' <summary>
		''' Get list of inbound layers from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> List of inbound layer names </returns>
		Public Shared Function getOutboundLayerNamesFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As IList(Of String)
			Dim outputLayerNames As IList(Of String) = New List(Of String)()
			If layerConfig.ContainsKey(conf.getLAYER_FIELD_OUTBOUND_NODES()) Then
				Dim outboundNodes As IList(Of Object) = DirectCast(layerConfig(conf.getLAYER_FIELD_OUTBOUND_NODES()), IList(Of Object))
				If outboundNodes.Count > 0 Then
					outboundNodes = DirectCast(outboundNodes(0), IList(Of Object))
					For Each o As Object In outboundNodes
						Dim nodeName As String = CStr(DirectCast(o, IList(Of Object))(0))
						outputLayerNames.Add(nodeName)
					Next o
				End If
			End If
			Return outputLayerNames
		End Function

		''' <summary>
		''' Get number of outputs from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> Number of output neurons of the Keras layer </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int getNOutFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getNOutFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Integer
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim nOut As Integer
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_OUTPUT_DIM()) Then
				' Most feedforward layers: Dense, RNN, etc. 
				nOut = DirectCast(innerConfig(conf.getLAYER_FIELD_OUTPUT_DIM()), Integer)
			ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_EMBEDDING_OUTPUT_DIM()) Then
				' Embedding layers. 
				nOut = DirectCast(innerConfig(conf.getLAYER_FIELD_EMBEDDING_OUTPUT_DIM()), Integer)
			ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_NB_FILTER()) Then
				' Convolutional layers. 
				nOut = DirectCast(innerConfig(conf.getLAYER_FIELD_NB_FILTER()), Integer)
			Else
				Throw New InvalidKerasConfigurationException("Could not determine number of outputs for layer: no " & conf.getLAYER_FIELD_OUTPUT_DIM() & " or " & conf.getLAYER_FIELD_NB_FILTER() & " field found")
			End If
			Return nOut
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static System.Nullable<Integer> getNInFromInputDim(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getNInFromInputDim(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Integer?
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_INPUT_DIM()) Then
				Dim id As Object = innerConfig(conf.getLAYER_FIELD_INPUT_DIM())
				If TypeOf id Is Number Then
					Return DirectCast(id, Number).intValue()
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Get dropout from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> get dropout value from Keras config </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static double getDropoutFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getDropoutFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
	'         NOTE: Keras "dropout" parameter determines dropout probability,
	'         * while DL4J "dropout" parameter determines retention probability.
	'         
			Dim dropout As Double = 1.0
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_DROPOUT()) Then
				' For most feedforward layers. 
				Try
					dropout = 1.0 - DirectCast(innerConfig(conf.getLAYER_FIELD_DROPOUT()), Double)
				Catch e As Exception
					Dim kerasDropout As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_DROPOUT()), Integer)
					dropout = 1.0 - kerasDropout
				End Try
			ElseIf innerConfig.ContainsKey(conf.getLAYER_FIELD_DROPOUT_W()) Then
				' For LSTMs. 
				Try
					dropout = 1.0 - DirectCast(innerConfig(conf.getLAYER_FIELD_DROPOUT_W()), Double)
				Catch e As Exception
					Dim kerasDropout As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_DROPOUT_W()), Integer)
					dropout = 1.0 - kerasDropout
				End Try
			End If
			Return dropout
		End Function

		''' <summary>
		''' Determine if layer should be instantiated with bias
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> whether layer has a bias term </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static boolean getHasBiasFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getHasBiasFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Boolean
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim hasBias As Boolean = True
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_USE_BIAS()) Then
				hasBias = DirectCast(innerConfig(conf.getLAYER_FIELD_USE_BIAS()), Boolean)
			End If
			Return hasBias
		End Function

		''' <summary>
		''' Get zero masking flag
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> if masking zeros or not </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static boolean getZeroMaskingFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getZeroMaskingFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Boolean
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim hasZeroMasking As Boolean = True
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_MASK_ZERO()) Then
				hasZeroMasking = DirectCast(innerConfig(conf.getLAYER_FIELD_MASK_ZERO()), Boolean)
			End If
			Return hasZeroMasking
		End Function

		''' <summary>
		''' Get mask value
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> mask value, defaults to 0.0 </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static double getMaskingValueFromConfig(Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getMaskingValueFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim maskValue As Double = 0.0
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_MASK_VALUE()) Then
				Try
					maskValue = DirectCast(innerConfig(conf.getLAYER_FIELD_MASK_VALUE()), Double)
				Catch e As Exception
					log.warn("Couldn't read masking value, default to 0.0")
				End Try
			Else
				Throw New InvalidKerasConfigurationException("No mask value found, field " & conf.getLAYER_FIELD_MASK_VALUE())
			End If
			Return maskValue
		End Function


		''' <summary>
		''' Remove weights from config after weight setting.
		''' </summary>
		''' <param name="weights"> layer weights </param>
		''' <param name="conf">    Keras layer configuration </param>
		Public Shared Sub removeDefaultWeights(ByVal weights As IDictionary(Of String, INDArray), ByVal conf As KerasLayerConfiguration)
			If weights.Count > 2 Then
				Dim paramNames As ISet(Of String) = weights.Keys
				paramNames.remove(conf.getKERAS_PARAM_NAME_W())
				paramNames.remove(conf.getKERAS_PARAM_NAME_B())
				Dim unknownParamNames As String = paramNames.ToString()
				log.warn("Attemping to set weights for unknown parameters: " & unknownParamNames.Substring(1, (unknownParamNames.Length - 1) - 1))
			End If
		End Sub

		Public Shared Function getMaskingConfiguration(Of T1 As KerasLayer)(ByVal inboundLayerNames As IList(Of String), ByVal previousLayers As IDictionary(Of T1)) As Pair(Of Boolean, Double)
			Dim hasMasking As Boolean? = False
			Dim maskingValue As Double? = 0.0
			For Each inboundLayerName As String In inboundLayerNames
				If previousLayers.ContainsKey(inboundLayerName) Then
					Dim inbound As KerasLayer = previousLayers(inboundLayerName)
					If TypeOf inbound Is KerasEmbedding AndAlso DirectCast(inbound, KerasEmbedding).isZeroMasking() Then
						hasMasking = True
					ElseIf TypeOf inbound Is KerasMasking Then
						hasMasking = True
						maskingValue = DirectCast(inbound, KerasMasking).getMaskingValue()
					End If
				End If
			Next inboundLayerName
			Return New Pair(Of Boolean, Double)(hasMasking, maskingValue)
		End Function

	End Class

End Namespace