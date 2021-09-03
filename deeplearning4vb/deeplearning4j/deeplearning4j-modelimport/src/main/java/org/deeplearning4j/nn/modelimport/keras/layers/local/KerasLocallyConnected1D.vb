Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LocallyConnected1D = org.deeplearning4j.nn.conf.layers.LocallyConnected1D
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConvolution = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolution
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolutionUtils
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasActivationUtils.getActivationFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasInitilizationUtils.getWeightInitFromConfig
Imports org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.local



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class KerasLocallyConnected1D extends org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolution
	Public Class KerasLocallyConnected1D
		Inherits KerasConvolution

		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLocallyConnected1D(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal kerasVersion As Integer?)
			MyBase.New(kerasVersion)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLocallyConnected1D(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasLocallyConnected1D(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)

			hasBias = getHasBiasFromConfig(layerConfig, conf)
			numTrainableParams = If(hasBias, 2, 1)
			Dim dilationRate() As Integer = getDilationRate(layerConfig, 1, conf, False)

			Dim init As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)

			Dim biasConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_B_CONSTRAINT(), conf, kerasMajorVersion)
			Dim weightConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_W_CONSTRAINT(), conf, kerasMajorVersion)

			Dim builder As LocallyConnected1D.Builder = (New LocallyConnected1D.Builder()).name(Me.layerName_Conflict).nOut(getNOutFromConfig(layerConfig, conf)).dropOut(Me.dropout).activation(getActivationFromConfig(layerConfig, conf)).weightInit(conf.getKERAS_PARAM_NAME_W(), init).l1(Me.weightL1Regularization).l2(Me.weightL2Regularization).convolutionMode(getConvolutionModeFromConfig(layerConfig, conf)).kernelSize(getKernelSizeFromConfig(layerConfig, 1, conf, kerasMajorVersion)(0)).hasBias(hasBias).stride(getStrideFromConfig(layerConfig, 1, conf)(0))
			Dim padding() As Integer = getPaddingFromBorderModeConfig(layerConfig, 1, conf, kerasMajorVersion)
			If padding IsNot Nothing Then
				builder.padding(padding(0))
			End If
			If dilationRate IsNot Nothing Then
				builder.dilation(dilationRate(0))
			End If
			If biasConstraint IsNot Nothing Then
				builder.constrainBias(biasConstraint)
			End If
			If weightConstraint IsNot Nothing Then
				builder.constrainWeights(weightConstraint)
			End If
			Me.layer_Conflict = builder.build()

		End Sub

		''' <summary>
		''' Get DL4J LocallyConnected1D layer.
		''' </summary>
		''' <returns> Locally connected 1D layer. </returns>
		Public Overridable ReadOnly Property LocallyConnected1DLayer As LocallyConnected1D
			Get
				Return DirectCast(Me.layer_Conflict, LocallyConnected1D)
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
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Convolution layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim rnnType As InputType.InputTypeRecurrent = DirectCast(inputType(0), InputType.InputTypeRecurrent)

			' Override input/output shape and input channels dynamically. This works since getOutputType will always
			' be called when initializing the model.
			DirectCast(Me.layer_Conflict, LocallyConnected1D).setInputSize(CInt(Math.Truncate(rnnType.getTimeSeriesLength())))
			DirectCast(Me.layer_Conflict, LocallyConnected1D).setNIn(rnnType.getSize())
			DirectCast(Me.layer_Conflict, LocallyConnected1D).computeOutputSize()

			Dim preprocessor As InputPreProcessor = getInputPreprocessor(inputType(0))
			If preprocessor IsNot Nothing Then
				Return Me.LocallyConnected1DLayer.getOutputType(-1, preprocessor.getOutputType(inputType(0)))
			End If
			Return Me.LocallyConnected1DLayer.getOutputType(-1, inputType(0))
		End Function

		''' <summary>
		''' Set weights for 1D locally connected layer.
		''' </summary>
		''' <param name="weights"> Map from parameter name to INDArray. </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
				If weights.ContainsKey(conf.getKERAS_PARAM_NAME_W()) Then
					Dim kerasParamValue As INDArray = weights(conf.getKERAS_PARAM_NAME_W())
					Me.weights_Conflict(ConvolutionParamInitializer.WEIGHT_KEY) = kerasParamValue
				Else
					Throw New InvalidKerasConfigurationException("Parameter " & conf.getKERAS_PARAM_NAME_W() & " does not exist in weights")
				End If
    
				If hasBias Then
					If weights.ContainsKey(conf.getKERAS_PARAM_NAME_B()) Then
						Me.weights_Conflict(ConvolutionParamInitializer.BIAS_KEY) = weights(conf.getKERAS_PARAM_NAME_B())
					Else
						Throw New InvalidKerasConfigurationException("Parameter " & conf.getKERAS_PARAM_NAME_B() & " does not exist in weights")
					End If
				End If
				removeDefaultWeights(weights, conf)
			End Set
		End Property

	End Class

End Namespace