Imports System.Collections.Generic
Imports SpatialDropout = org.deeplearning4j.nn.conf.dropout.SpatialDropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DropoutLayer = org.deeplearning4j.nn.conf.layers.DropoutLayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasLayerUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLayerUtils

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



	''' <summary>
	''' Keras wrapper for DL4J dropout layer with SpatialDropout, works 1D-3D.
	''' 
	''' @author Max Pumperla
	''' </summary>
	Public Class KerasSpatialDropout
		Inherits KerasLayer

		''' <summary>
		''' Pass-through constructor from KerasLayer
		''' </summary>
		''' <param name="kerasVersion"> major keras version </param>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSpatialDropout(System.Nullable<Integer> kerasVersion) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal kerasVersion As Integer?)
			MyBase.New(kerasVersion)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration. </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSpatialDropout(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
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
'ORIGINAL LINE: public KerasSpatialDropout(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_RATE()) Then
				Throw New InvalidKerasConfigurationException("Keras configuration does not contain " & "parameter" & conf.getLAYER_FIELD_RATE() & "needed for spatial dropout")
			End If
			Dim rate As Double = DirectCast(innerConfig(conf.getLAYER_FIELD_RATE()), Double) ' Keras stores drop rates
			Dim retainRate As Double = 1 - rate

			Me.layer_Conflict = (New DropoutLayer.Builder()).name(Me.layerName_Conflict).dropOut(New SpatialDropout(retainRate)).build()
		End Sub

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
				Throw New InvalidKerasConfigurationException("Keras SpatialDropout layer accepts only one input (received " & inputType.Length & ")")
			End If
			Return Me.SpatialDropoutLayer.getOutputType(-1, inputType(0))
		End Function

		''' <summary>
		''' Get DL4J DropoutLayer with spatial dropout.
		''' </summary>
		''' <returns> DropoutLayer </returns>
		Public Overridable ReadOnly Property SpatialDropoutLayer As DropoutLayer
			Get
				Return DirectCast(Me.layer_Conflict, DropoutLayer)
			End Get
		End Property

	End Class

End Namespace