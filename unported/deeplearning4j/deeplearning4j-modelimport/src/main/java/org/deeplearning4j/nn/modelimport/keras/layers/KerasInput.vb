Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers




	''' <summary>
	''' Imports an Input layer from Keras. Used to set InputType of DL4J model.
	''' 
	''' @author dave@skymind.io
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class KerasInput extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasInput
		Inherits KerasLayer

		Private ReadOnly NO_TRUNCATED_BPTT As Integer = 0

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration. </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasInput(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasInput(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
			If Me.inputShape_Conflict.Length > 4 Then
				Throw New UnsupportedKerasConfigurationException("Inputs with " & Me.inputShape_Conflict.Length & " dimensions not supported")
			End If
		End Sub

		''' <summary>
		''' Constructor from layer name and input shape.
		''' </summary>
		''' <param name="layerName">  layer name </param>
		''' <param name="inputShape"> input shape as array </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasInput(String layerName, int[] inputShape) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Sub New(ByVal layerName As String, ByVal inputShape() As Integer)
			Me.New(layerName, inputShape, True)
		End Sub

		''' <summary>
		''' Constructor from layer name and input shape.
		''' </summary>
		''' <param name="layerName">             layer name </param>
		''' <param name="inputShape">            input shape as array </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasInput(String layerName, int[] inputShape, boolean enforceTrainingConfig) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Sub New(ByVal layerName As String, ByVal inputShape() As Integer, ByVal enforceTrainingConfig As Boolean)
			Me.className_Conflict = conf.getLAYER_CLASS_NAME_INPUT()
			Me.layerName_Conflict = layerName
			Me.inputShape_Conflict = inputShape
			Me.inboundLayerNames_Conflict = New List(Of String)()
			Me.layer_Conflict = Nothing
			Me.vertex_Conflict = Nothing

			If Me.inputShape_Conflict.Length > 4 Then
				Throw New UnsupportedKerasConfigurationException("Inputs with " & Me.inputShape_Conflict.Length & " dimensions not supported")
			End If
		End Sub

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			If inputType.Length > 0 Then
				log.warn("Keras Input layer does not accept inputs (received " & inputType.Length & "). Ignoring.")
			End If
			Dim myInputType As InputType
			Select Case Me.inputShape_Conflict.Length
				Case 1
					myInputType = New InputType.InputTypeFeedForward(Me.inputShape_Conflict(0), Nothing)
				Case 2
					If Me.dimOrder_Conflict <> Nothing Then
						Select Case Me.dimOrder_Conflict
							Case KerasLayer.DimOrder.TENSORFLOW 'NWC == channels_last
								myInputType = New InputType.InputTypeRecurrent(Me.inputShape_Conflict(1), Me.inputShape_Conflict(0), RNNFormat.NWC)
							Case KerasLayer.DimOrder.THEANO 'NCW == channels_first
								myInputType = New InputType.InputTypeRecurrent(Me.inputShape_Conflict(0), Me.inputShape_Conflict(1), RNNFormat.NCW)
							Case KerasLayer.DimOrder.NONE
								'Assume RNN in [mb, seqLen, size] format
								myInputType = New InputType.InputTypeRecurrent(Me.inputShape_Conflict(1), Me.inputShape_Conflict(0), RNNFormat.NWC)
							Case Else
								Throw New System.InvalidOperationException("Unknown/not supported dimension ordering: " & Me.dimOrder_Conflict)
						End Select
					Else
						'Assume RNN in [mb, seqLen, size] format
						myInputType = New InputType.InputTypeRecurrent(Me.inputShape_Conflict(1), Me.inputShape_Conflict(0), RNNFormat.NWC)
					End If

				Case 3
					Select Case Me.dimOrder_Conflict
						Case KerasLayer.DimOrder.TENSORFLOW
							' TensorFlow convolutional input: # rows, # cols, # channels 
							myInputType = New InputType.InputTypeConvolutional(Me.inputShape_Conflict(0), Me.inputShape_Conflict(1), Me.inputShape_Conflict(2), CNN2DFormat.NHWC)
						Case KerasLayer.DimOrder.THEANO
							' Theano convolutional input:     # channels, # rows, # cols 
							myInputType = New InputType.InputTypeConvolutional(Me.inputShape_Conflict(1), Me.inputShape_Conflict(2), Me.inputShape_Conflict(0), CNN2DFormat.NCHW)
						Case Else
							Me.dimOrder_Conflict = DimOrder.THEANO
							myInputType = New InputType.InputTypeConvolutional(Me.inputShape_Conflict(1), Me.inputShape_Conflict(2), Me.inputShape_Conflict(0), CNN2DFormat.NCHW)
							log.warn("Couldn't determine dim ordering / data format from model file. Older Keras " & "versions may come without specified backend, in which case we assume the model was " & "built with theano.")
					End Select
				Case 4
					Select Case Me.dimOrder_Conflict
						Case KerasLayer.DimOrder.TENSORFLOW
							myInputType = New InputType.InputTypeConvolutional3D(Convolution3D.DataFormat.NDHWC, Me.inputShape_Conflict(0), Me.inputShape_Conflict(1), Me.inputShape_Conflict(2),Me.inputShape_Conflict(3))
						Case KerasLayer.DimOrder.THEANO
							myInputType = New InputType.InputTypeConvolutional3D(Convolution3D.DataFormat.NCDHW, Me.inputShape_Conflict(3), Me.inputShape_Conflict(0), Me.inputShape_Conflict(1),Me.inputShape_Conflict(2))
						Case Else
							Me.dimOrder_Conflict = DimOrder.THEANO
							myInputType = New InputType.InputTypeConvolutional3D(Convolution3D.DataFormat.NCDHW, Me.inputShape_Conflict(3), Me.inputShape_Conflict(0), Me.inputShape_Conflict(1),Me.inputShape_Conflict(2))
							log.warn("Couldn't determine dim ordering / data format from model file. Older Keras " & "versions may come without specified backend, in which case we assume the model was " & "built with theano.")
					End Select
				Case Else
					Throw New UnsupportedKerasConfigurationException("Inputs with " & Me.inputShape_Conflict.Length & " dimensions not supported")
			End Select
			Return myInputType
		End Function

		''' <summary>
		''' Returns value of truncated BPTT, if any found.
		''' </summary>
		''' <returns> value of truncated BPTT </returns>
		Public Overridable ReadOnly Property TruncatedBptt As Integer
			Get
				If Me.inputShape_Conflict.Length = 2 AndAlso Me.inputShape_Conflict(0) > 0 Then
					Return Me.inputShape_Conflict(0)
				End If
				Return NO_TRUNCATED_BPTT
			End Get
		End Property
	End Class

End Namespace