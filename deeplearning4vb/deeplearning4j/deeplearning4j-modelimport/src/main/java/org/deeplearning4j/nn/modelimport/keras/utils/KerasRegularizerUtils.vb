Imports System
Imports System.Collections.Generic
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
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

Namespace org.deeplearning4j.nn.modelimport.keras.utils


	Public Class KerasRegularizerUtils

		''' <summary>
		''' Get weight regularization from Keras weight regularization configuration.
		''' </summary>
		''' <param name="layerConfig">     Map containing Keras weight regularization configuration </param>
		''' <param name="conf">            Keras layer configuration </param>
		''' <param name="configField">     regularization config field to use </param>
		''' <param name="regularizerType"> type of regularization as string (e.g. "l2") </param>
		''' <returns> L1 or L2 regularization strength (0.0 if none) </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static double getWeightRegularizerFromConfig(java.util.Map<String, Object> layerConfig, org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, String configField, String regularizerType) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function getWeightRegularizerFromConfig(ByVal layerConfig As IDictionary(Of String, Object), ByVal conf As KerasLayerConfiguration, ByVal configField As String, ByVal regularizerType As String) As Double
			Dim innerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(layerConfig, conf)
			If innerConfig.ContainsKey(configField) Then
				Dim regularizerConfig As IDictionary(Of String, Object) = DirectCast(innerConfig(configField), IDictionary(Of String, Object))
				If regularizerConfig IsNot Nothing Then
					If regularizerConfig.ContainsKey(regularizerType) Then
						Return DirectCast(regularizerConfig(regularizerType), Double)
					End If
					If regularizerConfig.ContainsKey(conf.getLAYER_FIELD_CLASS_NAME()) AndAlso regularizerConfig(conf.getLAYER_FIELD_CLASS_NAME()).Equals("L1L2") Then
						Dim innerRegularizerConfig As IDictionary(Of String, Object) = KerasLayerUtils.getInnerLayerConfigFromConfig(regularizerConfig, conf)
						Try
							Return DirectCast(innerRegularizerConfig(regularizerType), Double)
						Catch e As Exception
							Return CDbl(DirectCast(innerRegularizerConfig(regularizerType), Integer))
						End Try


					End If
				End If
			End If
			Return 0.0
		End Function
	End Class

End Namespace