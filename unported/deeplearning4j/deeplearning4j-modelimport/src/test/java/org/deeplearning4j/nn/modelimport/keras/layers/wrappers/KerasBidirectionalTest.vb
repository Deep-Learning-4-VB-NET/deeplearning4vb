Imports System.Collections.Generic
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.wrappers

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Bidirectional Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasBidirectionalTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasBidirectionalTest
		Inherits BaseDL4JTest

		Private ReadOnly ACTIVATION_KERAS As String = "linear"

		Private ReadOnly ACTIVATION_DL4J As String = "identity"

		Private ReadOnly LAYER_NAME As String = "bidirectional_layer"

		Private ReadOnly INIT_KERAS As String = "glorot_normal"

		Private ReadOnly INIT_DL4J As WeightInit = WeightInit.XAVIER

		Private ReadOnly L1_REGULARIZATION As Double = 0.01

		Private ReadOnly L2_REGULARIZATION As Double = 0.02

		Private ReadOnly DROPOUT_KERAS As Double = 0.3

		Private ReadOnly DROPOUT_DL4J As Double = 1 - DROPOUT_KERAS

		Private ReadOnly N_OUT As Integer = 13

		Private ReadOnly mode As String = "sum"

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Lstm Layer") void testLstmLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLstmLayer()
			buildLstmLayer(conf1, keras1)
			buildLstmLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildLstmLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildLstmLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim innerActivation As String = "hard_sigmoid"
			Dim lstmForgetBiasString As String = "one"
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_LSTM()
			Dim lstmConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			' keras linear -> dl4j identity
			lstmConfig(conf.getLAYER_FIELD_ACTIVATION()) = ACTIVATION_KERAS
			' keras linear -> dl4j identity
			lstmConfig(conf.getLAYER_FIELD_INNER_ACTIVATION()) = innerActivation
			lstmConfig(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			If kerasVersion = 1 Then
				lstmConfig(conf.getLAYER_FIELD_INNER_INIT()) = INIT_KERAS
				lstmConfig(conf.getLAYER_FIELD_INIT()) = INIT_KERAS
			Else
				Dim init As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				init("class_name") = conf.getINIT_GLOROT_NORMAL()
				lstmConfig(conf.getLAYER_FIELD_INNER_INIT()) = init
				lstmConfig(conf.getLAYER_FIELD_INIT()) = init
			End If
			Dim W_reg As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			W_reg(conf.getREGULARIZATION_TYPE_L1()) = L1_REGULARIZATION
			W_reg(conf.getREGULARIZATION_TYPE_L2()) = L2_REGULARIZATION
			lstmConfig(conf.getLAYER_FIELD_W_REGULARIZER()) = W_reg
			lstmConfig(conf.getLAYER_FIELD_RETURN_SEQUENCES()) = True
			lstmConfig(conf.getLAYER_FIELD_DROPOUT_W()) = DROPOUT_KERAS
			lstmConfig(conf.getLAYER_FIELD_DROPOUT_U()) = 0.0
			lstmConfig(conf.getLAYER_FIELD_FORGET_BIAS_INIT()) = lstmForgetBiasString
			lstmConfig(conf.getLAYER_FIELD_OUTPUT_DIM()) = N_OUT
			lstmConfig(conf.getLAYER_FIELD_UNROLL()) = True
			Dim innerRnnConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			innerRnnConfig("class_name") = "LSTM"
			innerRnnConfig("config") = lstmConfig
			Dim innerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			innerConfig("merge_mode") = mode
			innerConfig("layer") = innerRnnConfig
			innerConfig(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			layerConfig("config") = innerConfig
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim kerasBidirectional As New KerasBidirectional(layerConfig)
			Dim layer As Bidirectional = kerasBidirectional.BidirectionalLayer
			assertEquals(Bidirectional.Mode.ADD, layer.getMode())
			assertEquals(Activation.HARDSIGMOID.ToString().ToLower(), DirectCast(kerasBidirectional.UnderlyingRecurrentLayer, LSTM).getGateActivationFn().ToString())
		End Sub
	End Class

End Namespace