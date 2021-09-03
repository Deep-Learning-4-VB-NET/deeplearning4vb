Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports KerasTestUtils = org.deeplearning4j.nn.modelimport.keras.KerasTestUtils
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasConvolution3D = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasConvolution3D
Imports ReshapePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @DisplayName("Keras Convolution 3 D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasConvolution3DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasConvolution3DTest
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

		Private ReadOnly KERNEL_SIZE() As Integer = { 1, 2, 3 }

		Private ReadOnly STRIDE() As Integer = { 3, 4, 5 }

		Private ReadOnly N_OUT As Integer = 13

		Private ReadOnly BORDER_MODE_VALID As String = "valid"

		Private ReadOnly VALID_PADDING() As Integer = { 0, 0, 0 }

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Convolution 3 D Layer") void testConvolution3DLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testConvolution3DLayer()
			buildConvolution3DLayer(conf1, keras1)
			buildConvolution3DLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildConvolution3DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildConvolution3DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_CONVOLUTION_3D()
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
			If kerasVersion = 1 Then
				config(conf.getLAYER_FIELD_3D_KERNEL_1()) = KERNEL_SIZE(0)
				config(conf.getLAYER_FIELD_3D_KERNEL_2()) = KERNEL_SIZE(1)
				config(conf.getLAYER_FIELD_3D_KERNEL_3()) = KERNEL_SIZE(2)
			Else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				java.util.ArrayList kernel = New java.util.ArrayList<Integer>()
				config(conf.getLAYER_FIELD_KERNEL_SIZE()) = kernel
			End If
			Dim subsampleList As IList(Of Integer) = New List(Of Integer)()
			subsampleList.Add(STRIDE(0))
			subsampleList.Add(STRIDE(1))
			subsampleList.Add(STRIDE(2))
			config(conf.getLAYER_FIELD_CONVOLUTION_STRIDES()) = subsampleList
			config(conf.getLAYER_FIELD_NB_FILTER()) = N_OUT
			config(conf.getLAYER_FIELD_BORDER_MODE()) = BORDER_MODE_VALID
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As ConvolutionLayer = (New KerasConvolution3D(layerConfig)).Convolution3DLayer
			assertEquals(ACTIVATION_DL4J, layer.getActivationFn().ToString())
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(INIT_DL4J, layer.getWeightInitFn())
			assertEquals(L1_REGULARIZATION, KerasTestUtils.getL1(layer), 0.0)
			assertEquals(L2_REGULARIZATION, KerasTestUtils.getL2(layer), 0.0)
			assertEquals(New Dropout(DROPOUT_DL4J), layer.getIDropout())
			assertArrayEquals(KERNEL_SIZE, layer.getKernelSize())
			assertArrayEquals(STRIDE, layer.getStride())
			assertEquals(N_OUT, layer.getNOut())
			assertEquals(ConvolutionMode.Truncate, layer.getConvolutionMode())
			assertArrayEquals(VALID_PADDING, layer.getPadding())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDefaultLayout(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDefaultLayout(ByVal testDir As Path)
			Dim config As String = "{""class_name"": ""Sequential"", ""config"": {""name"": ""sequential_1"", ""layers"": [{""class_name"": ""InputLayer"", ""config"": {""batch_input_shape"": [null, 32], ""dtype"": ""float32"", ""sparse"": false, ""ragged"": false, ""name"": ""input_2""}}, {""class_name"": ""Dense"", ""config"": {""name"": ""dense_4"", ""trainable"": true, ""dtype"": ""float32"", ""units"": 720, ""activation"": ""linear"", ""use_bias"": true, ""kernel_initializer"": {""class_name"": ""GlorotUniform"", ""config"": {""seed"": null}}, ""bias_initializer"": {""class_name"": ""Zeros"", ""config"": {}}, ""kernel_regularizer"": null, ""bias_regularizer"": null, ""activity_regularizer"": null, ""kernel_constraint"": null, ""bias_constraint"": null}}, {""class_name"": ""LeakyReLU"", ""config"": {""name"": ""leaky_re_lu"", ""trainable"": true, ""dtype"": ""float32"", ""alpha"": 0.10000000149011612}}, {""class_name"": ""BatchNormalization"", ""config"": {""name"": ""batch_normalization_3"", ""trainable"": true, ""dtype"": ""float32"", ""axis"": [1], ""momentum"": 0.99, ""epsilon"": 0.001, ""center"": true, ""scale"": true, ""beta_initializer"": {""class_name"": ""Zeros"", ""config"": {}}, ""gamma_initializer"": {""class_name"": ""Ones"", ""config"": {}}, ""moving_mean_initializer"": {""class_name"": ""Zeros"", ""config"": {}}, ""moving_variance_initializer"": {""class_name"": ""Ones"", ""config"": {}}, ""beta_regularizer"": null, ""gamma_regularizer"": null, ""beta_constraint"": null, ""gamma_constraint"": null}}, {""class_name"": ""Dropout"", ""config"": {""name"": ""dropout_1"", ""trainable"": true, ""dtype"": ""float32"", ""rate"": 0.2, ""noise_shape"": null, ""seed"": null}}, {""class_name"": ""Reshape"", ""config"": {""name"": ""reshape"", ""trainable"": true, ""dtype"": ""float32"", ""target_shape"": [60, 1, 3, 4]}}, {""class_name"": ""Conv3D"", ""config"": {""name"": ""conv3d_4"", ""trainable"": true, ""dtype"": ""float32"", ""filters"": 256, ""kernel_size"": [3, 3, 3], ""strides"": [1, 1, 1], ""padding"": ""same"", ""data_format"": ""channels_last"", ""dilation_rate"": [1, 1, 1], ""groups"": 1, ""activation"": ""linear"", ""use_bias"": true, ""kernel_initializer"": {""class_name"": ""GlorotUniform"", ""config"": {""seed"": null}}, ""bias_initializer"": {""class_name"": ""Zeros"", ""config"": {}}, ""kernel_regularizer"": null, ""bias_regularizer"": null, ""activity_regularizer"": null, ""kernel_constraint"": null, ""bias_constraint"": null}}, {""class_name"": ""LeakyReLU"", ""config"": {""name"": ""leaky_re_lu_1"", ""trainable"": true, ""dtype"": ""float32"", ""alpha"": 0.10000000149011612}}, {""class_name"": ""UpSampling3D"", ""config"": {""name"": ""up_sampling3d"", ""trainable"": true, ""dtype"": ""float32"", ""size"": [2, 2, 2], ""data_format"": ""channels_last""}}, {""class_name"": ""Conv3D"", ""config"": {""name"": ""conv3d_5"", ""trainable"": true, ""dtype"": ""float32"", ""filters"": 128, ""kernel_size"": [3, 3, 3], ""strides"": [1, 1, 1], ""padding"": ""same"", ""data_format"": ""channels_last"", ""dilation_rate"": [1, 1, 1], ""groups"": 1, ""activation"": ""linear"", ""use_bias"": true, ""kernel_initializer"": {""class_name"": ""GlorotUniform"", ""config"": {""seed"": null}}, ""bias_initializer"": {""class_name"": ""Zeros"", ""config"": {}}, ""kernel_regularizer"": null, ""bias_regularizer"": null, ""activity_regularizer"": null, ""kernel_constraint"": null, ""bias_constraint"": null}}, {""class_name"": ""LeakyReLU"", ""config"": {""name"": ""leaky_re_lu_2"", ""trainable"": true, ""dtype"": ""float32"", ""alpha"": 0.10000000149011612}}, {""class_name"": ""UpSampling3D"", ""config"": {""name"": ""up_sampling3d_1"", ""trainable"": true, ""dtype"": ""float32"", ""size"": [2, 2, 2], ""data_format"": ""channels_last""}}, {""class_name"": ""Conv3D"", ""config"": {""name"": ""conv3d_6"", ""trainable"": true, ""dtype"": ""float32"", ""filters"": 16, ""kernel_size"": [3, 3, 3], ""strides"": [1, 1, 1], ""padding"": ""same"", ""data_format"": ""channels_last"", ""dilation_rate"": [1, 1, 1], ""groups"": 1, ""activation"": ""linear"", ""use_bias"": true, ""kernel_initializer"": {""class_name"": ""GlorotUniform"", ""config"": {""seed"": null}}, ""bias_initializer"": {""class_name"": ""Zeros"", ""config"": {}}, ""kernel_regularizer"": null, ""bias_regularizer"": null, ""activity_regularizer"": null, ""kernel_constraint"": null, ""bias_constraint"": null}}, {""class_name"": ""LeakyReLU"", ""config"": {""name"": ""leaky_re_lu_3"", ""trainable"": true, ""dtype"": ""float32"", ""alpha"": 0.10000000149011612}}, {""class_name"": ""UpSampling3D"", ""config"": {""name"": ""up_sampling3d_2"", ""trainable"": true, ""dtype"": ""float32"", ""size"": [2, 2, 2], ""data_format"": ""channels_last""}}, {""class_name"": ""Conv3D"", ""config"": {""name"": ""conv3d_7"", ""trainable"": true, ""dtype"": ""float32"", ""filters"": 8, ""kernel_size"": [3, 3, 3], ""strides"": [1, 1, 1], ""padding"": ""same"", ""data_format"": ""channels_last"", ""dilation_rate"": [1, 1, 1], ""groups"": 1, ""activation"": ""linear"", ""use_bias"": true, ""kernel_initializer"": {""class_name"": ""GlorotUniform"", ""config"": {""seed"": null}}, ""bias_initializer"": {""class_name"": ""Zeros"", ""config"": {}}, ""kernel_regularizer"": null, ""bias_regularizer"": null, ""activity_regularizer"": null, ""kernel_constraint"": null, ""bias_constraint"": null}}, {""class_name"": ""LeakyReLU"", ""config"": {""name"": ""leaky_re_lu_4"", ""trainable"": true, ""dtype"": ""float32"", ""alpha"": 0.10000000149011612}}, {""class_name"": ""Conv3D"", ""config"": {""name"": ""conv3d_8"", ""trainable"": true, ""dtype"": ""float32"", ""filters"": 1, ""kernel_size"": [3, 3, 3], ""strides"": [1, 1, 1], ""padding"": ""same"", ""data_format"": ""channels_last"", ""dilation_rate"": [1, 1, 1], ""groups"": 1, ""activation"": ""linear"", ""use_bias"": true, ""kernel_initializer"": {""class_name"": ""GlorotUniform"", ""config"": {""seed"": null}}, ""bias_initializer"": {""class_name"": ""Zeros"", ""config"": {}}, ""kernel_regularizer"": null, ""bias_regularizer"": null, ""activity_regularizer"": null, ""kernel_constraint"": null, ""bias_constraint"": null}}]}, ""keras_version"": ""2.4.0"", ""backend"": ""tensorflow""}" & vbLf
			Dim tempFile As File = testDir.resolve("temp.json").toFile()
			FileUtils.writeStringToFile(tempFile,config, Charset.defaultCharset())
			Dim multiLayerConfiguration As MultiLayerConfiguration = KerasModelImport.importKerasSequentialConfiguration(tempFile.getAbsolutePath())
			assertNotNull(multiLayerConfiguration)
			'null pre processor should still work and default to channels last
			Dim reshapePreprocessor As ReshapePreprocessor = DirectCast(multiLayerConfiguration.getInputPreProcess(4), ReshapePreprocessor)
			assertNull(reshapePreprocessor.getFormat())
			Console.WriteLine(multiLayerConfiguration)
		End Sub

	End Class

End Namespace