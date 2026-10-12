Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolutionUtils

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.pooling


	''' <summary>
	''' Imports a Keras 2D Pooling layer as a DL4J Subsampling layer.
	''' 
	''' @author dave@skymind.io
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasPooling2D extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasPooling2D
		Inherits KerasLayer

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration. </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasPooling2D(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasPooling2D(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
			Dim builder As SubsamplingLayer.Builder = (New SubsamplingLayer.Builder(KerasPoolingUtils.mapPoolingType(Me.className_Conflict, conf))).name(Me.layerName_Conflict).dropOut(Me.dropout).dataFormat(If(dimOrder_Conflict = DimOrder.TENSORFLOW, CNN2DFormat.NHWC, CNN2DFormat.NCHW)).convolutionMode(getConvolutionModeFromConfig(layerConfig, conf)).kernelSize(getKernelSizeFromConfig(layerConfig, 2, conf, kerasMajorVersion)).stride(getStrideFromConfig(layerConfig, 2, conf))
			Dim padding() As Integer = getPaddingFromBorderModeConfig(layerConfig, 2, conf, kerasMajorVersion)
			If padding IsNot Nothing Then
				builder.padding(padding)
			End If
			Me.layer_Conflict = builder.build()
			Dim subsamplingLayer As SubsamplingLayer = DirectCast(layer_Conflict, SubsamplingLayer)
			'ensure the default value stays
			subsamplingLayer.setDefaultValueOverridden(True)
			Me.vertex_Conflict = Nothing
		End Sub

		''' <summary>
		''' Get DL4J SubsamplingLayer.
		''' </summary>
		''' <returns> SubsamplingLayer </returns>
		Public Overridable ReadOnly Property Subsampling2DLayer As SubsamplingLayer
			Get
				Return DirectCast(Me.layer_Conflict, SubsamplingLayer)
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
				Throw New InvalidKerasConfigurationException("Keras Subsampling 2D layer accepts only one input (received " & inputType.Length & ")")
			End If
			Return Me.Subsampling2DLayer.getOutputType(-1, inputType(0))
		End Function
	End Class

End Namespace