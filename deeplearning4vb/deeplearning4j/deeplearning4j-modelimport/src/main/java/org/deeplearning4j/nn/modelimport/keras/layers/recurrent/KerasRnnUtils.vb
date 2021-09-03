Imports System
Imports System.Collections.Generic
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasEmbedding = org.deeplearning4j.nn.modelimport.keras.layers.embeddings.KerasEmbedding
Imports KerasBidirectional = org.deeplearning4j.nn.modelimport.keras.layers.wrappers.KerasBidirectional
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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.recurrent


	Public Class KerasRnnUtils

		''' <summary>
		''' Returns true if the given layer is an
		''' <seealso cref="KerasLSTM"/>, <seealso cref="KerasSimpleRnn"/>,
		''' <seealso cref="KerasBidirectional"/> </summary>
		''' <param name="kerasLayer"> the input layer
		''' @return </param>
		Public Shared Function isRnnLayer(ByVal kerasLayer As KerasLayer) As Boolean
			Return TypeOf kerasLayer Is KerasLSTM OrElse TypeOf kerasLayer Is KerasSimpleRnn OrElse TypeOf kerasLayer Is KerasBidirectional OrElse TypeOf kerasLayer Is KerasEmbedding
		End Function

		''' <summary>
		''' Get unroll parameter to decide whether to unroll RNN with BPTT or not.
		''' </summary>
		''' <param name="conf">        KerasLayerConfiguration </param>
		''' <param name="layerConfig"> dictionary containing Keras layer properties </param>
		''' <returns> boolean unroll parameter </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static boolean getUnrollRecurrentLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, java.util.Map<String, Object> layerConfig) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getUnrollRecurrentLayer(ByVal conf As KerasLayerConfiguration, ByVal layerConfig As IDictionary(Of String, Object)) As Boolean
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If Not innerConfig.ContainsKey(conf.getLAYER_FIELD_UNROLL()) Then
				Throw New InvalidKerasConfigurationException("Keras LSTM layer config missing " & conf.getLAYER_FIELD_UNROLL() & " field")
			End If
			Return DirectCast(innerConfig(conf.getLAYER_FIELD_UNROLL()), Boolean)
		End Function

		''' <summary>
		''' Get recurrent weight dropout from Keras layer configuration.
		''' Non-zero dropout rates are currently not supported.
		''' </summary>
		''' <param name="conf">        KerasLayerConfiguration </param>
		''' <param name="layerConfig"> dictionary containing Keras layer properties </param>
		''' <returns> recurrent dropout rate </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static double getRecurrentDropout(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, java.util.Map<String, Object> layerConfig) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getRecurrentDropout(ByVal conf As KerasLayerConfiguration, ByVal layerConfig As IDictionary(Of String, Object)) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			Dim dropout As Double = 1.0
			If innerConfig.ContainsKey(conf.getLAYER_FIELD_DROPOUT_U()) Then
				Try
					dropout = 1.0 - DirectCast(innerConfig(conf.getLAYER_FIELD_DROPOUT_U()), Double)
				Catch e As Exception
					Dim kerasDropout As Integer = DirectCast(innerConfig(conf.getLAYER_FIELD_DROPOUT_U()), Integer)
					dropout = 1.0 - CDbl(kerasDropout)
				End Try
			End If
			If dropout < 1.0 Then
				Throw New UnsupportedKerasConfigurationException("Dropout > 0 on recurrent connections not supported.")
			End If
			Return dropout
		End Function
	End Class

End Namespace