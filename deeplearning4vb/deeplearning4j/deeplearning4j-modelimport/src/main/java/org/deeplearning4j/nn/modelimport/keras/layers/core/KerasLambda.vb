Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
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

Namespace org.deeplearning4j.nn.modelimport.keras.layers.core



	''' <summary>
	''' Wraps a DL4J SameDiffLambda into a KerasLayer
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasLambda extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasLambda
		Inherits KerasLayer

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig"> dictionary containing Keras layer configuration </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLambda(java.util.Map<String, Object> layerConfig, org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer sameDiffLayer) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal sameDiffLayer As SameDiffLayer)
			Me.New(layerConfig, True, sameDiffLayer)
		End Sub

		''' <summary>
		''' Constructor from parsed Keras layer configuration dictionary.
		''' </summary>
		''' <param name="layerConfig">           dictionary containing Keras layer configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLambda(java.util.Map<String, Object> layerConfig, boolean enforceTrainingConfig, org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer sameDiffLayer) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Sub New(ByVal layerConfig As IDictionary(Of String, Object), ByVal enforceTrainingConfig As Boolean, ByVal sameDiffLayer As SameDiffLayer)
			MyBase.New(layerConfig, enforceTrainingConfig)
			sameDiffLayer.setLayerName(Me.layerName_Conflict)
			Me.layer_Conflict = sameDiffLayer
		End Sub

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			If inputType.Length > 1 Then
				log.warn("Note: only first input type will be counted for lambda on layer with name " & layerName_Conflict)
			End If
			Return Me.SameDiffLayer.getOutputType(-1, inputType(0))
		End Function

		''' <summary>
		''' Get DL4J SameDiffLayer.
		''' </summary>
		''' <returns> SameDiffLayer </returns>
		Public Overridable ReadOnly Property SameDiffLayer As SameDiffLayer
			Get
				Return DirectCast(Me.layer_Conflict, SameDiffLayer)
			End Get
		End Property

	End Class

End Namespace