﻿Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DepthwiseConvolution2D = org.deeplearning4j.nn.conf.layers.DepthwiseConvolution2D
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports KerasRegularizerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasRegularizerUtils
Imports SeparableConvolutionParamInitializer = org.deeplearning4j.nn.params.SeparableConvolutionParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolutionUtils
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasActivationUtils.getIActivationFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasInitilizationUtils.getWeightInitFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils.getHasBiasFromConfig

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
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class KerasDepthwiseConvolution2D extends KerasConvolution
	Public Class KerasDepthwiseConvolution2D
		Inherits KerasConvolution


		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasDepthwiseConvolution2D(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal kerasVersion As Integer?)
			MyBase.New(kerasVersion)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras configuration </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasDepthwiseConvolution2D(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, Enumerable.Empty(Of String, KerasLayer)(), True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras configuration </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasDepthwiseConvolution2D(java.util.Map<String, Object> layerConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal previousLayers As IDictionary(Of String, KerasLayer))
			Me.New(layerConfig, previousLayers, True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras configuration </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasDepthwiseConvolution2D(java.util.Map<String, Object> layerConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal previousLayers As IDictionary(Of String, KerasLayer), ByVal enforceTrainingConfig As Boolean)
			Me.New(layerConfig, previousLayers, Nothing, enforceTrainingConfig)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras configuration </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasDepthwiseConvolution2D(java.util.Map<String, Object> layerConfig, java.util.Map<String, ? extends org.deeplearning4j.nn.modelimport.keras.KerasLayer> previousLayers, java.util.List<String> layerNamesToCheck, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal previousLayers As IDictionary(Of String, KerasLayer), ByVal layerNamesToCheck As IList(Of String), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)

			If layerNamesToCheck IsNot Nothing Then
				CType(inboundLayerNames_Conflict, List(Of String)).AddRange(layerNamesToCheck)
			End If
			hasBias = getHasBiasFromConfig(layerConfig, conf)
			numTrainableParams = If(hasBias, 2, 1)
			Dim dilationRate() As Integer = getDilationRate(layerConfig, 2, conf, False)

			Dim depthWiseInit As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_DEPTH_WISE_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)

			Dim nIn As val = getNInFromConfig(previousLayers)

			Dim depthMultiplier As Integer = getDepthMultiplier(layerConfig, conf)

			Me.weightL1Regularization = KerasRegularizerUtils.getWeightRegularizerFromConfig(layerConfig, conf, conf.getLAYER_FIELD_DEPTH_WISE_REGULARIZER(), conf.getREGULARIZATION_TYPE_L1())
			Me.weightL2Regularization = KerasRegularizerUtils.getWeightRegularizerFromConfig(layerConfig, conf, conf.getLAYER_FIELD_DEPTH_WISE_REGULARIZER(), conf.getREGULARIZATION_TYPE_L2())


			Dim biasConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_B_CONSTRAINT(), conf, kerasMajorVersion)
			Dim depthWiseWeightConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_DEPTH_WISE_CONSTRAINT(), conf, kerasMajorVersion)


			Dim builder As DepthwiseConvolution2D.Builder = (New DepthwiseConvolution2D.Builder()).name(Me.layerName_Conflict).dropOut(Me.dropout).nIn(nIn).nOut(nIn * depthMultiplier).activation(getIActivationFromConfig(layerConfig, conf)).weightInit(depthWiseInit).depthMultiplier(depthMultiplier).l1(Me.weightL1Regularization).l2(Me.weightL2Regularization).convolutionMode(getConvolutionModeFromConfig(layerConfig, conf)).kernelSize(getKernelSizeFromConfig(layerConfig, 2, conf, kerasMajorVersion)).hasBias(hasBias).dataFormat(If(dimOrder_Conflict = DimOrder.TENSORFLOW, CNN2DFormat.NHWC, CNN2DFormat.NCHW)).stride(getStrideFromConfig(layerConfig, 2, conf))
			Dim padding() As Integer = getPaddingFromBorderModeConfig(layerConfig, 2, conf, kerasMajorVersion)
			If hasBias Then
				builder.biasInit(0.0)
			End If
			If padding IsNot Nothing Then
				builder.padding(padding)
			End If
			If dilationRate IsNot Nothing Then
				builder.dilation(dilationRate)
			End If
			If biasConstraint IsNot Nothing Then
				builder.constrainBias(biasConstraint)
			End If
			If depthWiseWeightConstraint IsNot Nothing Then
				builder.constrainWeights(depthWiseWeightConstraint)
			End If
			Me.layer_Conflict = builder.build()
			Dim depthwiseConvolution2D As DepthwiseConvolution2D = DirectCast(layer_Conflict, DepthwiseConvolution2D)
			depthwiseConvolution2D.setDefaultValueOverriden(True)
		End Sub

		''' <summary>
		''' Set weights for layer.
		''' </summary>
		''' <param name="weights"> Map of weights </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void setWeights(java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> weights) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides WriteOnly Property Weights As IDictionary(Of String, INDArray)
			Set(ByVal weights As IDictionary(Of String, INDArray))
				Me.weights_Conflict = New Dictionary(Of String, INDArray)()
    
				Dim dW As INDArray
				If weights.ContainsKey(conf.getLAYER_PARAM_NAME_DEPTH_WISE_KERNEL()) Then
					dW = weights(conf.getLAYER_PARAM_NAME_DEPTH_WISE_KERNEL())
				Else
					Throw New InvalidKerasConfigurationException("Keras DepthwiseConvolution2D layer does not contain parameter " & conf.getLAYER_PARAM_NAME_DEPTH_WISE_KERNEL())
				End If
    
				Me.weights_Conflict(SeparableConvolutionParamInitializer.DEPTH_WISE_WEIGHT_KEY) = dW
				If hasBias Then
					Dim bias As INDArray
					If kerasMajorVersion = 2 AndAlso weights.ContainsKey("bias") Then
						bias = weights("bias")
					ElseIf kerasMajorVersion = 1 AndAlso weights.ContainsKey("b") Then
						bias = weights("b")
					Else
						Throw New InvalidKerasConfigurationException("Keras DepthwiseConvolution2D layer does not contain bias parameter")
					End If
					Me.weights_Conflict(SeparableConvolutionParamInitializer.BIAS_KEY) = bias
    
				End If
    
			End Set
		End Property

		''' <summary>
		''' Get DL4J DepthwiseConvolution2D.
		''' </summary>
		''' <returns> DepthwiseConvolution2D </returns>
		Public Overridable ReadOnly Property DepthwiseConvolution2DLayer As DepthwiseConvolution2D
			Get
				Return DirectCast(Me.layer_Conflict, DepthwiseConvolution2D)
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
				Throw New InvalidKerasConfigurationException("Keras depth-wise convolution 2D layer accepts only one input (received " & inputType.Length & ")")
			End If
			Return Me.DepthwiseConvolution2DLayer.getOutputType(-1, inputType(0))
		End Function

	End Class
End Namespace