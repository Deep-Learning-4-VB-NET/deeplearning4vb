﻿Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InputTypeConvolutional = org.deeplearning4j.nn.conf.inputs.InputType.InputTypeConvolutional
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports Cnn3DToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.Cnn3DToFeedForwardPreProcessor
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasFlattenRnnPreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.KerasFlattenRnnPreprocessor
Imports ReshapePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.core


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasFlatten extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasFlatten
		Inherits KerasLayer

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasFlatten(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasFlatten(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
		End Sub

		''' <summary>
		''' Whether this Keras layer maps to a DL4J InputPreProcessor.
		''' </summary>
		''' <returns> true </returns>
		Public Overrides ReadOnly Property InputPreProcessor As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Gets appropriate DL4J InputPreProcessor for given InputTypes.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> DL4J InputPreProcessor </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
		''' <seealso cref= org.deeplearning4j.nn.conf.InputPreProcessor </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.InputPreProcessor getInputPreprocessor(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getInputPreprocessor(ParamArray ByVal inputType() As InputType) As InputPreProcessor
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Flatten layer accepts only one input (received " & inputType.Length & ")")
			End If
			''' <summary>
			''' TODO: On layer name dropout_2 as input the flatten layer seems to be outputting 20 instead of 80.
			''' Likely due to needing to multiply the final outputs totaled to 80, but only getting to 20.
			''' </summary>
			Dim preprocessor As InputPreProcessor = Nothing
			If TypeOf inputType(0) Is InputType.InputTypeConvolutional Then
				Dim it As InputType.InputTypeConvolutional = DirectCast(inputType(0), InputType.InputTypeConvolutional)
				Select Case Me.DimOrder
					Case NONE, THEANO
						preprocessor = New CnnToFeedForwardPreProcessor(it.getHeight(), it.getWidth(), it.getChannels(), CNN2DFormat.NCHW)
					Case TENSORFLOW
						preprocessor = New CnnToFeedForwardPreProcessor(it.getHeight(), it.getWidth(), it.getChannels(), CNN2DFormat.NHWC)
					Case Else
						Throw New InvalidKerasConfigurationException("Unknown Keras backend " & Me.DimOrder)
				End Select
			ElseIf TypeOf inputType(0) Is InputType.InputTypeRecurrent Then
				Dim it As InputType.InputTypeRecurrent = DirectCast(inputType(0), InputType.InputTypeRecurrent)
				preprocessor = New KerasFlattenRnnPreprocessor(it.getSize(), it.getTimeSeriesLength())
			ElseIf TypeOf inputType(0) Is InputType.InputTypeFeedForward Then
				' NOTE: The output of an embedding layer in DL4J is of feed-forward type. Only if an FF to RNN input
				' preprocessor is set or we explicitly provide 3D input data to start with, will the its output be set
				' to RNN type. Otherwise we add this trivial preprocessor (since there's nothing to flatten).
				Dim it As InputType.InputTypeFeedForward = DirectCast(inputType(0), InputType.InputTypeFeedForward)
				Dim inputShape As val = New Long(){it.getSize()}
				preprocessor = New ReshapePreprocessor(inputShape, inputShape, False, Nothing)
			ElseIf TypeOf inputType(0) Is InputType.InputTypeConvolutional3D Then
				Dim it As InputType.InputTypeConvolutional3D = DirectCast(inputType(0), InputType.InputTypeConvolutional3D)
				Select Case Me.DimOrder
					Case NONE, THEANO
						preprocessor = New Cnn3DToFeedForwardPreProcessor(it.getDepth(),it.getHeight(),it.getWidth(), it.getChannels(),it.getDataFormat() = Convolution3D.DataFormat.NCDHW)
					Case TENSORFLOW
						preprocessor = New Cnn3DToFeedForwardPreProcessor(it.getDepth(),it.getHeight(),it.getWidth(), it.getChannels(),it.getDataFormat() <> Convolution3D.DataFormat.NCDHW)
					Case Else
						Throw New InvalidKerasConfigurationException("Unknown Keras backend " & Me.DimOrder)
				End Select
			End If
			Return preprocessor
		End Function

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
				Throw New InvalidKerasConfigurationException("Keras Flatten layer accepts only one input (received " & inputType.Length & ")")
			End If
			Dim preprocessor As InputPreProcessor = getInputPreprocessor(inputType)
			If preprocessor IsNot Nothing Then
				Return preprocessor.getOutputType(inputType(0))
			End If
			Return inputType(0)
		End Function
	End Class

End Namespace