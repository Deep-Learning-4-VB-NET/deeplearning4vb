﻿Imports System.Collections.Generic
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports SeparableConvolution2D = org.deeplearning4j.nn.conf.layers.SeparableConvolution2D
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasTestUtils = org.deeplearning4j.nn.modelimport.keras.KerasTestUtils
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasSeparableConvolution2D = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasSeparableConvolution2D
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.convolution

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Separable Convolution 2 D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasSeparableConvolution2DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasSeparableConvolution2DTest
		Inherits BaseDL4JTest

		Private ReadOnly ACTIVATION_KERAS As String = "linear"

		Private ReadOnly ACTIVATION_DL4J As String = "identity"

		Private ReadOnly LAYER_NAME As String = "test_layer"

		Private ReadOnly INIT_KERAS As String = "glorot_normal"

		Private ReadOnly INIT_DL4J As IWeightInit = New WeightInitXavier()

		Private ReadOnly L1_REGULARIZATION As Double = 0.01

		Private ReadOnly L2_REGULARIZATION As Double = 0.02

		Private ReadOnly DROPOUT_KERAS As Double = 0.3

		Private ReadOnly DROPOUT_DL4J As Double = 1 - DROPOUT_KERAS

		Private ReadOnly KERNEL_SIZE() As Integer = { 1, 2 }

		Private ReadOnly DILATION() As Integer = { 2, 2 }

		Private ReadOnly DEPTH_MULTIPLIER As Integer = 4

		Private ReadOnly STRIDE() As Integer = { 3, 4 }

		Private ReadOnly N_OUT As Integer = 13

		Private ReadOnly BORDER_MODE_VALID As String = "valid"

		Private ReadOnly VALID_PADDING() As Integer = { 0, 0 }

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Separable Convolution 2 D Layer") void testSeparableConvolution2DLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSeparableConvolution2DLayer()
			buildSeparableConvolution2DLayer(conf1, keras1, False)
			buildSeparableConvolution2DLayer(conf2, keras2, False)
			buildSeparableConvolution2DLayer(conf2, keras2, True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildSeparableConvolution2DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion, boolean withDilation) throws Exception
		Private Sub buildSeparableConvolution2DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?, ByVal withDilation As Boolean)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_SEPARABLE_CONVOLUTION_2D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_ACTIVATION()) = ACTIVATION_KERAS
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			If kerasVersion = 1 Then
				config(conf.getLAYER_FIELD_DEPTH_WISE_INIT()) = INIT_KERAS
				config(conf.getLAYER_FIELD_POINT_WISE_INIT()) = INIT_KERAS
			Else
				Dim init As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				init("class_name") = conf.getINIT_GLOROT_NORMAL()
				config(conf.getLAYER_FIELD_DEPTH_WISE_INIT()) = init
				config(conf.getLAYER_FIELD_POINT_WISE_INIT()) = init
			End If
			Dim W_reg As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			W_reg(conf.getREGULARIZATION_TYPE_L1()) = L1_REGULARIZATION
			W_reg(conf.getREGULARIZATION_TYPE_L2()) = L2_REGULARIZATION
			config(conf.getLAYER_FIELD_DEPTH_WISE_REGULARIZER()) = W_reg
			config(conf.getLAYER_FIELD_DROPOUT()) = DROPOUT_KERAS
			config(conf.getLAYER_FIELD_DEPTH_MULTIPLIER()) = DEPTH_MULTIPLIER
			If kerasVersion = 1 Then
				config(conf.getLAYER_FIELD_NB_ROW()) = KERNEL_SIZE(0)
				config(conf.getLAYER_FIELD_NB_COL()) = KERNEL_SIZE(1)
			Else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				java.util.ArrayList kernel = New java.util.ArrayList<Integer>()
				config(conf.getLAYER_FIELD_KERNEL_SIZE()) = kernel
			End If
			If withDilation Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				java.util.ArrayList dilation = New java.util.ArrayList<Integer>()
				config(conf.getLAYER_FIELD_DILATION_RATE()) = dilation
			End If
			Dim subsampleList As IList(Of Integer) = New List(Of Integer)()
			subsampleList.Add(STRIDE(0))
			subsampleList.Add(STRIDE(1))
			config(conf.getLAYER_FIELD_CONVOLUTION_STRIDES()) = subsampleList
			config(conf.getLAYER_FIELD_NB_FILTER()) = N_OUT
			config(conf.getLAYER_FIELD_BORDER_MODE()) = BORDER_MODE_VALID
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As SeparableConvolution2D = (New KerasSeparableConvolution2D(layerConfig)).SeparableConvolution2DLayer
			assertEquals(ACTIVATION_DL4J, layer.getActivationFn().ToString())
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(INIT_DL4J, layer.getWeightInitFn())
			assertEquals(L1_REGULARIZATION, KerasTestUtils.getL1(layer), 0.0)
			assertEquals(L2_REGULARIZATION, KerasTestUtils.getL2(layer), 0.0)
			assertEquals(DEPTH_MULTIPLIER, layer.getDepthMultiplier())
			assertEquals(New Dropout(DROPOUT_DL4J), layer.getIDropout())
			assertArrayEquals(KERNEL_SIZE, layer.getKernelSize())
			assertArrayEquals(STRIDE, layer.getStride())
			assertEquals(N_OUT, layer.getNOut())
			assertEquals(ConvolutionMode.Truncate, layer.getConvolutionMode())
			assertArrayEquals(VALID_PADDING, layer.getPadding())
			If withDilation Then
				assertEquals(DILATION(0), layer.getDilation()(0))
				assertEquals(DILATION(1), layer.getDilation()(1))
			End If
		End Sub
	End Class

End Namespace