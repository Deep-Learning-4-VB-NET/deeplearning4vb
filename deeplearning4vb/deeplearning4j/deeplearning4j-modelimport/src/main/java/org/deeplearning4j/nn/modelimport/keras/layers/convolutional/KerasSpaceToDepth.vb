Imports System.Collections.Generic
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SpaceToDepthLayer = org.deeplearning4j.nn.conf.layers.SpaceToDepthLayer
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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.convolutional


	Public Class KerasSpaceToDepth
		Inherits KerasLayer

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration. </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras configuration exception </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSpaceToDepth(java.util.Map<String, Object> layerConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object))
			Me.New(layerConfig, True)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras configuration exception </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras configuration exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasSpaceToDepth(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean)
			MyBase.New(layerConfig, enforceTrainingConfig)

			' TODO: we hard-code block size here to import YOLO9000. This size is not available as property
			' in the hdf5 file outside of the serialized lambda function (that we can't really well deserialize).
			Dim builder As SpaceToDepthLayer.Builder = (New SpaceToDepthLayer.Builder()).blocks(2).dataFormat(SpaceToDepthLayer.DataFormat.NHWC).name(layerName_Conflict)

			Me.layer_Conflict = builder.build()
			Me.vertex_Conflict = Nothing
		End Sub

		''' <summary>
		''' Get DL4J SpaceToDepth layer.
		''' </summary>
		''' <returns> SpaceToDepth layer </returns>
		Public Overridable ReadOnly Property SpaceToDepthLayer As SpaceToDepthLayer
			Get
				Return DirectCast(Me.layer_Conflict, SpaceToDepthLayer)
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
				Throw New InvalidKerasConfigurationException("Keras Space to Depth layer accepts only one input (received " & inputType.Length & ")")
			End If
			Return Me.SpaceToDepthLayer.getOutputType(-1, inputType(0))
		End Function

	End Class
End Namespace