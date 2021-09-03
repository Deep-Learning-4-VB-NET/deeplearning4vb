Imports System.Collections.Generic
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports MaskZeroLayer = org.deeplearning4j.nn.conf.layers.util.MaskZeroLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasTestUtils = org.deeplearning4j.nn.modelimport.keras.KerasTestUtils
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports KerasEmbedding = org.deeplearning4j.nn.modelimport.keras.layers.embeddings.KerasEmbedding
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports Assertions = org.junit.jupiter.api.Assertions
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras LSTM Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasLSTMTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasLSTMTest
		Inherits BaseDL4JTest

		Private ReadOnly ACTIVATION_KERAS As String = "linear"

		Private ReadOnly ACTIVATION_DL4J As String = "identity"

		Private ReadOnly LAYER_NAME As String = "lstm_layer"

		Private ReadOnly INIT_KERAS As String = "glorot_normal"

		Private ReadOnly INIT_DL4J As IWeightInit = New WeightInitXavier()

		Private ReadOnly L1_REGULARIZATION As Double = 0.01

		Private ReadOnly L2_REGULARIZATION As Double = 0.02

		Private ReadOnly DROPOUT_KERAS As Double = 0.3

		Private ReadOnly DROPOUT_DL4J As Double = 1 - DROPOUT_KERAS

		Private ReadOnly N_OUT As Integer = 13

		Private returnSequences() As Boolean? = { True, False }

		Private maskZero() As Boolean? = { True, False }

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Lstm Layer") void testLstmLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLstmLayer()
			For Each rs As Boolean? In returnSequences
				buildLstmLayer(conf1, keras1, rs)
				buildLstmLayer(conf2, keras2, rs)
			Next rs
			For Each mz As Boolean? In maskZero
				buildMaskZeroLstmLayer(conf1, keras1, mz)
				buildMaskZeroLstmLayer(conf2, keras2, mz)
			Next mz
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void buildLstmLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion, System.Nullable<Boolean> rs) throws Exception
		Friend Overridable Sub buildLstmLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?, ByVal rs As Boolean?)
			Dim innerActivation As String = "hard_sigmoid"
			Dim lstmForgetBiasDouble As Double = 1.0
			Dim lstmForgetBiasString As String = "one"
			Dim lstmUnroll As Boolean = True
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_LSTM()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_ACTIVATION()) = ACTIVATION_KERAS
			config(conf.getLAYER_FIELD_INNER_ACTIVATION()) = innerActivation
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			If kerasVersion = 1 Then
				config(conf.getLAYER_FIELD_INNER_INIT()) = INIT_KERAS
				config(conf.getLAYER_FIELD_INIT()) = INIT_KERAS
			Else
				Dim init As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				init("class_name") = conf.getINIT_GLOROT_NORMAL()
				config(conf.getLAYER_FIELD_INNER_INIT()) = init
				config(conf.getLAYER_FIELD_INIT()) = init
			End If
			Dim W_reg As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			W_reg(conf.getREGULARIZATION_TYPE_L1()) = L1_REGULARIZATION
			W_reg(conf.getREGULARIZATION_TYPE_L2()) = L2_REGULARIZATION
			config(conf.getLAYER_FIELD_W_REGULARIZER()) = W_reg
			config(conf.getLAYER_FIELD_RETURN_SEQUENCES()) = rs
			config(conf.getLAYER_FIELD_DROPOUT_W()) = DROPOUT_KERAS
			config(conf.getLAYER_FIELD_DROPOUT_U()) = 0.0
			config(conf.getLAYER_FIELD_FORGET_BIAS_INIT()) = lstmForgetBiasString
			config(conf.getLAYER_FIELD_OUTPUT_DIM()) = N_OUT
			config(conf.getLAYER_FIELD_UNROLL()) = lstmUnroll
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As LSTM
			Dim lts As LastTimeStep
			Dim kerasLstm As New KerasLSTM(layerConfig)
			If rs Then
				Dim outputType As InputType = kerasLstm.getOutputType(InputType.recurrent(1337))
				assertEquals(outputType, InputType.recurrent(N_OUT))
				layer = DirectCast(kerasLstm.LSTMLayer, LSTM)
			Else
				lts = DirectCast(kerasLstm.LSTMLayer, LastTimeStep)
				Dim outputType As InputType = kerasLstm.getOutputType(InputType.feedForward(1337))
				assertEquals(outputType, InputType.feedForward(N_OUT))
				layer = DirectCast(lts.Underlying, LSTM)
			End If
			assertEquals(ACTIVATION_DL4J, layer.getActivationFn().ToString())
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(INIT_DL4J, layer.getWeightInitFn())
			assertEquals(L1_REGULARIZATION, KerasTestUtils.getL1(layer), 0.0)
			assertEquals(L2_REGULARIZATION, KerasTestUtils.getL2(layer), 0.0)
			assertEquals(New Dropout(DROPOUT_DL4J), layer.getIDropout())
			assertEquals(lstmForgetBiasDouble, layer.getForgetGateBiasInit(), 0.0)
			assertEquals(N_OUT, layer.getNOut())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildMaskZeroLstmLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion, System.Nullable<Boolean> maskZero) throws Exception
		Private Sub buildMaskZeroLstmLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?, ByVal maskZero As Boolean?)
			Dim innerActivation As String = "hard_sigmoid"
			Dim lstmForgetBiasString As String = "one"
			Dim lstmUnroll As Boolean = True
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_LSTM()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_ACTIVATION()) = ACTIVATION_KERAS
			config(conf.getLAYER_FIELD_INNER_ACTIVATION()) = innerActivation
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			If kerasVersion = 1 Then
				config(conf.getLAYER_FIELD_INNER_INIT()) = INIT_KERAS
				config(conf.getLAYER_FIELD_INIT()) = INIT_KERAS
			Else
				Dim init As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				init("class_name") = conf.getINIT_GLOROT_NORMAL()
				config(conf.getLAYER_FIELD_INNER_INIT()) = init
				config(conf.getLAYER_FIELD_INIT()) = init
			End If
			Dim W_reg As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			W_reg(conf.getREGULARIZATION_TYPE_L1()) = L1_REGULARIZATION
			W_reg(conf.getREGULARIZATION_TYPE_L2()) = L2_REGULARIZATION
			config(conf.getLAYER_FIELD_W_REGULARIZER()) = W_reg
			config(conf.getLAYER_FIELD_DROPOUT_W()) = DROPOUT_KERAS
			config(conf.getLAYER_FIELD_DROPOUT_U()) = 0.0
			config(conf.getLAYER_FIELD_FORGET_BIAS_INIT()) = lstmForgetBiasString
			config(conf.getLAYER_FIELD_OUTPUT_DIM()) = N_OUT
			config(conf.getLAYER_FIELD_UNROLL()) = lstmUnroll
			config(conf.getLAYER_FIELD_RETURN_SEQUENCES()) = True
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			layerConfig(conf.getLAYER_FIELD_INBOUND_NODES()) = Arrays.asList(Arrays.asList(Arrays.asList("embedding")))
			Dim embedding As KerasEmbedding = getEmbedding(maskZero)
			Dim previousLayers As IDictionary(Of String, KerasEmbedding) = Collections.singletonMap("embedding", embedding)
			Dim kerasLstm As New KerasLSTM(layerConfig, previousLayers)
			Assertions.assertEquals(TypeOf kerasLstm.Layer Is MaskZeroLayer, maskZero)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.modelimport.keras.layers.embeddings.KerasEmbedding getEmbedding(boolean maskZero) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function getEmbedding(ByVal maskZero As Boolean) As KerasEmbedding
			Dim embedding As New KerasEmbedding()
			embedding.setZeroMasking(maskZero)
			Return embedding
		End Function
	End Class

End Namespace