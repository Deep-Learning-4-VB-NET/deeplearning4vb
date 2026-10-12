Imports System.Collections.Generic
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LocallyConnected1D = org.deeplearning4j.nn.conf.layers.LocallyConnected1D
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasTestUtils = org.deeplearning4j.nn.modelimport.keras.KerasTestUtils
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.local

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Locally Connected 1 D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasLocallyConnected1DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasLocallyConnected1DTest
		Inherits BaseDL4JTest

		Private ReadOnly ACTIVATION_KERAS As String = "linear"

		Private ReadOnly ACTIVATION_DL4J As String = "identity"

		Private ReadOnly LAYER_NAME As String = "test_layer"

		Private ReadOnly INIT_KERAS As String = "glorot_normal"

		Private ReadOnly INIT_DL4J As WeightInit = WeightInit.XAVIER

		Private ReadOnly L1_REGULARIZATION As Double = 0.01

		Private ReadOnly L2_REGULARIZATION As Double = 0.02

		Private ReadOnly DROPOUT_KERAS As Double = 0.3

		Private ReadOnly DROPOUT_DL4J As Double = 1 - DROPOUT_KERAS

		Private ReadOnly KERNEL_SIZE As Integer = 2

		Private ReadOnly STRIDE As Integer = 3

		Private ReadOnly N_OUT As Integer = 13

		Private ReadOnly BORDER_MODE_VALID As String = "valid"

		Private ReadOnly VALID_PADDING As Integer = 0

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Locally Connected 2 D Layer") void testLocallyConnected2DLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLocallyConnected2DLayer()
			buildLocallyConnected2DLayer(conf1, keras1)
			buildLocallyConnected2DLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildLocallyConnected2DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildLocallyConnected2DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_LOCALLY_CONNECTED_2D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_ACTIVATION()) = ACTIVATION_KERAS
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			If kerasVersion = 1 Then
				config(conf.getLAYER_FIELD_INIT()) = INIT_KERAS
			Else
				Dim init As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				init("class_name") = conf.getINIT_GLOROT_NORMAL()
				config(conf.getLAYER_FIELD_INIT()) = init
			End If
			Dim W_reg As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			W_reg(conf.getREGULARIZATION_TYPE_L1()) = L1_REGULARIZATION
			W_reg(conf.getREGULARIZATION_TYPE_L2()) = L2_REGULARIZATION
			config(conf.getLAYER_FIELD_W_REGULARIZER()) = W_reg
			config(conf.getLAYER_FIELD_DROPOUT()) = DROPOUT_KERAS
			If kerasVersion = 2 Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				java.util.ArrayList kernel = New java.util.ArrayList<Integer>()
				config(conf.getLAYER_FIELD_FILTER_LENGTH()) = kernel
			Else
				config(conf.getLAYER_FIELD_FILTER_LENGTH()) = KERNEL_SIZE
			End If
			If kerasVersion = 2 Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				java.util.ArrayList stride = New java.util.ArrayList<Integer>()
				config(conf.getLAYER_FIELD_SUBSAMPLE_LENGTH()) = stride
			Else
				config(conf.getLAYER_FIELD_SUBSAMPLE_LENGTH()) = STRIDE
			End If
			config(conf.getLAYER_FIELD_NB_FILTER()) = N_OUT
			config(conf.getLAYER_FIELD_BORDER_MODE()) = BORDER_MODE_VALID
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim kerasLocal As New KerasLocallyConnected1D(layerConfig)
			' once get output type is triggered, inputshape, output shape and input depth are updated
			kerasLocal.getOutputType(InputType.recurrent(3, 4))
			Dim layer As LocallyConnected1D = kerasLocal.LocallyConnected1DLayer
			assertEquals(ACTIVATION_DL4J, layer.getActivation().ToString().ToLower())
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(INIT_DL4J, layer.getWeightInit())
			assertEquals(L1_REGULARIZATION, KerasTestUtils.getL1(layer), 0.0)
			assertEquals(L2_REGULARIZATION, KerasTestUtils.getL2(layer), 0.0)
			assertEquals(New Dropout(DROPOUT_DL4J), layer.getIDropout())
			assertEquals(KERNEL_SIZE, layer.getKernel())
			assertEquals(STRIDE, layer.getStride())
			assertEquals(N_OUT, layer.getNOut())
			assertEquals(ConvolutionMode.Truncate, layer.getCm())
			assertEquals(VALID_PADDING, layer.getPadding())
			assertEquals(layer.getInputSize(), 4)
			assertEquals(layer.getNIn(), 3)
		End Sub
	End Class

End Namespace