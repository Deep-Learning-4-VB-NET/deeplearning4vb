Imports System.Collections.Generic
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation

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


	Public Class KerasActivationUtils

		''' <summary>
		''' Map Keras to DL4J activation functions.
		''' </summary>
		''' <param name="conf"> Keras layer configuration </param>
		''' <param name="kerasActivation"> String containing Keras activation function name </param>
		''' <returns> Activation enum value containing DL4J activation function name </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.activations.Activation mapToActivation(String kerasActivation, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function mapToActivation(ByVal kerasActivation As String, ByVal conf As KerasLayerConfiguration) As Activation
			Dim dl4jActivation As Activation
			If kerasActivation.Equals(conf.getKERAS_ACTIVATION_SOFTMAX()) Then
				dl4jActivation = Activation.SOFTMAX
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_SOFTPLUS()) Then
				dl4jActivation = Activation.SOFTPLUS
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_SOFTSIGN()) Then
				dl4jActivation = Activation.SOFTSIGN
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_RELU()) Then
				dl4jActivation = Activation.RELU
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_RELU6()) Then
				dl4jActivation = Activation.RELU6
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_ELU()) Then
				dl4jActivation = Activation.ELU
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_SELU()) Then
				dl4jActivation = Activation.SELU
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_TANH()) Then
				dl4jActivation = Activation.TANH
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_SIGMOID()) Then
				dl4jActivation = Activation.SIGMOID
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_HARD_SIGMOID()) Then
				dl4jActivation = Activation.HARDSIGMOID
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_LINEAR()) Then
				dl4jActivation = Activation.IDENTITY
			ElseIf kerasActivation.Equals(conf.getKERAS_ACTIVATION_SWISH()) Then
				dl4jActivation = Activation.SWISH
			Else
				Throw New UnsupportedKerasConfigurationException("Unknown Keras activation function " & kerasActivation)
			End If
			Return dl4jActivation
		End Function


		''' <summary>
		''' Map Keras to DL4J activation functions.
		''' </summary>
		''' <param name="kerasActivation"> String containing Keras activation function name </param>
		''' <returns> DL4J activation function </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.activations.IActivation mapToIActivation(String kerasActivation, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function mapToIActivation(ByVal kerasActivation As String, ByVal conf As KerasLayerConfiguration) As IActivation
			Dim activation As Activation = mapToActivation(kerasActivation, conf)
			Return activation.getActivationFunction()
		End Function

		''' <summary>
		''' Get activation function from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> DL4J activation function </returns>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.activations.IActivation getIActivationFromConfig(java.util.Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function getIActivationFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As IActivation
			Return getActivationFromConfig(layerConfig, conf).getActivationFunction()
		End Function

		''' <summary>
		''' Get activation enum value from Keras layer configuration.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <returns> DL4J activation enum value </returns>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.activations.Activation getActivationFromConfig(java.util.Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function getActivationFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration) As Activation
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_ACTIVATION()) Then
				Throw New InvalidKerasConfigurationException("Keras layer is missing " & conf.getLAYER_FIELD_ACTIVATION() & " field")
			End If
			Return mapToActivation(DirectCast(innerConfig(conf.getLAYER_FIELD_ACTIVATION()), String), conf)
		End Function
	End Class

End Namespace