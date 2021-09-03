﻿Imports System.Collections.Generic
Imports Upsampling1D = org.deeplearning4j.nn.conf.layers.Upsampling1D
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasUpsampling1D = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasUpsampling1D
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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.convolution

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Upsampling 1 D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasUpsampling1DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasUpsampling1DTest
		Inherits BaseDL4JTest

		Private ReadOnly LAYER_NAME As String = "upsampling_1D_layer"

		Private size As Integer = 4

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Upsampling 1 D Layer") void testUpsampling1DLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testUpsampling1DLayer()
			buildUpsampling1DLayer(conf1, keras1)
			buildUpsampling1DLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildUpsampling1DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildUpsampling1DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_UPSAMPLING_1D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_UPSAMPLING_1D_SIZE()) = size
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As Upsampling1D = (New KerasUpsampling1D(layerConfig)).Upsampling1DLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(size, layer.getSize()(0))
		End Sub
	End Class

End Namespace