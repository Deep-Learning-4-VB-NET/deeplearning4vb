Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports CnnLossLayer = org.deeplearning4j.nn.conf.layers.CnnLossLayer
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports RnnLossLayer = org.deeplearning4j.nn.conf.layers.RnnLossLayer
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasLossUtils.mapLossFunction

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = false) public class KerasLoss extends org.deeplearning4j.nn.modelimport.keras.KerasLayer
	Public Class KerasLoss
		Inherits KerasLayer

		Private ReadOnly KERAS_CLASS_NAME_LOSS As String = "Loss"
		Private loss As ILossFunction


		''' <summary>
		''' Constructor from layer name and input shape.
		''' </summary>
		''' <param name="layerName">        layer name </param>
		''' <param name="inboundLayerName"> name of inbound layer </param>
		''' <param name="kerasLoss">        name of Keras loss function </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLoss(String layerName, String inboundLayerName, String kerasLoss) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Sub New(ByVal layerName As String, ByVal inboundLayerName As String, ByVal kerasLoss As String)
			Me.New(layerName, inboundLayerName, kerasLoss, True)
		End Sub

		''' <summary>
		''' Constructor from layer name and input shape.
		''' </summary>
		''' <param name="layerName">             layer name </param>
		''' <param name="inboundLayerName">      name of inbound layer </param>
		''' <param name="kerasLoss">             name of Keras loss function </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training-related configuration options </param>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasLoss(String layerName, String inboundLayerName, String kerasLoss, boolean enforceTrainingConfig) throws UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Sub New(ByVal layerName As String, ByVal inboundLayerName As String, ByVal kerasLoss As String, ByVal enforceTrainingConfig As Boolean)
			Me.className_Conflict = KERAS_CLASS_NAME_LOSS
			Me.layerName_Conflict = layerName
			Me.inputShape_Conflict = Nothing
			Me.dimOrder_Conflict = DimOrder.NONE
			Me.inboundLayerNames_Conflict = New List(Of String)()
			Me.inboundLayerNames_Conflict.Add(inboundLayerName)
			Try
				loss = mapLossFunction(kerasLoss, conf)
			Catch e As UnsupportedKerasConfigurationException
				If enforceTrainingConfig Then
					Throw e
				End If
				log.warn("Unsupported Keras loss function. Replacing with MSE.")
				loss = LossFunctions.LossFunction.SQUARED_LOSS.getILossFunction()
			End Try
		End Sub

		''' <summary>
		''' Get DL4J LossLayer.
		''' </summary>
		''' <returns> LossLayer </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.conf.layers.FeedForwardLayer getLossLayer(org.deeplearning4j.nn.conf.inputs.InputType type) throws org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function getLossLayer(ByVal type As InputType) As FeedForwardLayer
			If TypeOf type Is InputType.InputTypeFeedForward Then
				Me.layer_Conflict = (New LossLayer.Builder(loss)).name(Me.layerName_Conflict).activation(Activation.IDENTITY).build()
			ElseIf TypeOf type Is InputType.InputTypeRecurrent Then
				Me.layer_Conflict = (New RnnLossLayer.Builder(loss)).name(Me.layerName_Conflict).activation(Activation.IDENTITY).build()
			ElseIf TypeOf type Is InputType.InputTypeConvolutional Then
				Me.layer_Conflict = (New CnnLossLayer.Builder(loss)).name(Me.layerName_Conflict).activation(Activation.IDENTITY).build()
			Else
				Throw New UnsupportedKerasConfigurationException("Unsupported output layer type" & "got : " & type.ToString())
			End If
			Return DirectCast(Me.layer_Conflict, FeedForwardLayer)
		End Function

		''' <summary>
		''' Get layer output type.
		''' </summary>
		''' <param name="inputType"> Array of InputTypes </param>
		''' <returns> output type as InputType </returns>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras config </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(org.deeplearning4j.nn.conf.inputs.InputType... inputType) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overrides Function getOutputType(ParamArray ByVal inputType() As InputType) As InputType
			If inputType.Length > 1 Then
				Throw New InvalidKerasConfigurationException("Keras Loss layer accepts only one input (received " & inputType.Length & ")")
			End If
			Return Me.getLossLayer(inputType(0)).getOutputType(-1, inputType(0))
		End Function
	End Class

End Namespace