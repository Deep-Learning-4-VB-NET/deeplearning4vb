Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Convolution1DLayer = org.deeplearning4j.nn.conf.layers.Convolution1DLayer
Imports InputTypeUtil = org.deeplearning4j.nn.conf.layers.InputTypeUtil
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolutionUtils
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasActivationUtils.getIActivationFromConfig
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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.convolutional


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class KerasConvolution1D extends KerasConvolution
	Public Class KerasConvolution1D
		Inherits KerasConvolution

		''' <summary>
		''' Pass-through constructor from KerasLayer </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasConvolution1D(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal kerasVersion As Integer?)
			MyBase.New(kerasVersion)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">       dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException"> </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasConvolution1D(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">               dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig">     whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException"> </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasConvolution1D(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
			hasBias = getHasBiasFromConfig(layerConfig, conf)
			'dl4j weights are 128,20,3,1 keras are 128,100,3,1
			numTrainableParams = If(hasBias, 2, 1)
			Dim dilationRate() As Integer = getDilationRate(layerConfig, 1, conf, False)
			Dim biasConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_B_CONSTRAINT(), conf, kerasMajorVersion)
			Dim weightConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_W_CONSTRAINT(), conf, kerasMajorVersion)

			Dim init As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)
			Dim builder As Convolution1DLayer.Builder = (New Convolution1DLayer.Builder()).name(Me.layerName_Conflict).nOut(getNOutFromConfig(layerConfig, conf)).dropOut(Me.dropout).activation(getIActivationFromConfig(layerConfig, conf)).weightInit(init).l1(Me.weightL1Regularization).l2(Me.weightL2Regularization).convolutionMode(getConvolutionModeFromConfig(layerConfig, conf)).kernelSize(getKernelSizeFromConfig(layerConfig, 1, conf, kerasMajorVersion)(0)).hasBias(hasBias).stride(getStrideFromConfig(layerConfig, 1, conf)(0)).rnnDataFormat(If(dimOrder_Conflict = DimOrder.TENSORFLOW, RNNFormat.NWC, RNNFormat.NCW))
			Dim padding() As Integer = getPaddingFromBorderModeConfig(layerConfig, 1, conf, kerasMajorVersion)
			If hasBias Then
				builder.biasInit(0.0)
			End If
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
			If inputShape_Conflict IsNot Nothing Then
				If dimOrder_Conflict = DimOrder.THEANO Then
					builder.nIn(inputShape_Conflict(0))
				Else
					builder.nIn(inputShape_Conflict(1))
				End If
			End If

			Me.layer_Conflict = builder.build()
			'set this in order to infer the dimensional format
			Dim convolution1DLayer As Convolution1DLayer = DirectCast(Me.layer_Conflict, Convolution1DLayer)
			convolution1DLayer.setCnn2dDataFormat(If(dimOrder_Conflict = DimOrder.TENSORFLOW, CNN2DFormat.NHWC, CNN2DFormat.NCHW))
			convolution1DLayer.setDefaultValueOverriden(True)
		End Sub

		''' <summary>
		''' Get DL4J ConvolutionLayer.
		''' </summary>
		''' <returns>  ConvolutionLayer </returns>
		Public Overridable ReadOnly Property Convolution1DLayer As Convolution1DLayer
			Get
				Return DirectCast(Me.layer_Conflict, Convolution1DLayer)
			End Get
		End Property


		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Convolution layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim preprocessor As InputPreProcessor = getInputPreprocessor(inputType(0))
			If preprocessor IsNot Nothing Then
				Return Me.Convolution1DLayer.getOutputType(-1, preprocessor.getOutputType(inputType(0)))
			End If
			Return Me.Convolution1DLayer.getOutputType(-1, inputType(0))
		End Function


		''' <summary>
		''' Gets appropriate DL4J InputPreProcessor for given InputTypes.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> DL4J InputPreProcessor </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration exception </exception>
		''' <seealso cref= org.deeplearning4j.nn.conf.InputPreProcessor </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.InputPreProcessor getInputPreprocessor(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getInputPreprocessor(ParamArray ByVal inputType() As InputType) As InputPreProcessor
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Conv1D layer accepts only one input (received " & inputType.Length & ")")
			End If
			If inputType(0) IsNot Nothing AndAlso inputType(0).getType() <> InputType.Type.RNN OrElse inputType(0) Is Nothing Then
				Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType(0), RNNFormat.NCW,layerName_Conflict)
			Else
				Dim inputTypeRecurrent As InputType.InputTypeRecurrent = DirectCast(inputType(0), InputType.InputTypeRecurrent)
				Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType(0),inputTypeRecurrent.getFormat(),layerName_Conflict)

			End If
		End Function


		''' <summary>
		''' Set weights for layer.
		''' </summary>
		''' <param name="weights">   Map from parameter name to INDArray. </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
				If weights.ContainsKey(conf.getKERAS_PARAM_NAME_W()) Then
					Dim kerasParamValue As INDArray = weights(conf.getKERAS_PARAM_NAME_W())
					Dim paramValue As INDArray
					Select Case Me.DimOrder
						Case TENSORFLOW
							paramValue = kerasParamValue
							paramValue = paramValue.reshape(ChrW(paramValue.size(0)), paramValue.size(1), paramValue.size(2), 1)
    
						Case THEANO
							'Convert from keras [k,nIn,nOut] to DL4J conv2d [nOut, nIn, k, 1]
							Dim k As Long = kerasParamValue.size(0)
							Dim nIn As Long = kerasParamValue.size(1)
							Dim nOut As Long = kerasParamValue.size(2)
							paramValue = kerasParamValue.dup("c"c).reshape(ChrW(nOut), nIn, k, 1)
						Case Else
							Throw New InvalidKerasConfigurationException("Unknown keras backend " & Me.DimOrder)
					End Select
    
					Me.weights_Conflict(ConvolutionParamInitializer.WEIGHT_KEY) = paramValue
    
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