﻿Imports System.Collections.Generic
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasConstraintUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasConstraintUtils
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolutionUtils
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasActivationUtils.getIActivationFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasInitilizationUtils.getWeightInitFromConfig
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils.getHasBiasFromConfig
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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.convolutional


	Public Class KerasAtrousConvolution2D
		Inherits KerasConvolution

		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasAtrousConvolution2D(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasAtrousConvolution2D(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasAtrousConvolution2D(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)

			hasBias = getHasBiasFromConfig(layerConfig, conf)
			numTrainableParams = If(hasBias, 2, 1)

			Dim biasConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_B_CONSTRAINT(), conf, kerasMajorVersion)
			Dim weightConstraint As LayerConstraint = KerasConstraintUtils.getConstraintsFromConfig(layerConfig, conf.getLAYER_FIELD_W_CONSTRAINT(), conf, kerasMajorVersion)

			Dim init As IWeightInit = getWeightInitFromConfig(layerConfig, conf.getLAYER_FIELD_INIT(), enforceTrainingConfig, conf, kerasMajorVersion)

			Dim builder As ConvolutionLayer.Builder = (New ConvolutionLayer.Builder()).name(Me.layerName_Conflict).nOut(getNOutFromConfig(layerConfig, conf)).dropOut(Me.dropout).activation(getIActivationFromConfig(layerConfig, conf)).weightInit(init).dilation(getDilationRate(layerConfig, 2, conf, True)).l1(Me.weightL1Regularization).l2(Me.weightL2Regularization).convolutionMode(getConvolutionModeFromConfig(layerConfig, conf)).kernelSize(getKernelSizeFromConfig(layerConfig, 2, conf, kerasMajorVersion)).dataFormat(If(dimOrder_Conflict = DimOrder.TENSORFLOW, CNN2DFormat.NHWC, CNN2DFormat.NCHW)).hasBias(hasBias).stride(getStrideFromConfig(layerConfig, 2, conf))
			Dim padding() As Integer = getPaddingFromBorderModeConfig(layerConfig, 2, conf, kerasMajorVersion)

			If hasBias Then
				builder.biasInit(0.0)
			End If
			If padding IsNot Nothing Then
				builder.padding(padding)
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
		''' Get DL4J ConvolutionLayer.
		''' </summary>
		''' <returns> ConvolutionLayer </returns>
		Public Overridable ReadOnly Property AtrousConvolution2D As ConvolutionLayer
			Get
				Return DirectCast(Me.layer_Conflict, ConvolutionLayer)
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
			Return Me.AtrousConvolution2D.getOutputType(-1, inputType(0))
		End Function

	End Class
End Namespace