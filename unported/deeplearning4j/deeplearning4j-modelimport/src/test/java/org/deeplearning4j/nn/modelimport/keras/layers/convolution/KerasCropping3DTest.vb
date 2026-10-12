Imports System.Collections.Generic
Imports Cropping3D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping3D
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasCropping3D = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasCropping3D
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
'ORIGINAL LINE: @DisplayName("Keras Cropping 3 D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasCropping3DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasCropping3DTest
		Inherits BaseDL4JTest

		Private ReadOnly LAYER_NAME As String = "cropping_3D_layer"

		Private ReadOnly CROPPING() As Integer = { 2, 3, 5 }

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cropping 3 D Layer") void testCropping3DLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCropping3DLayer()
			Dim keras1 As Integer? = 1
			buildCropping3DLayer(conf1, keras1)
			Dim keras2 As Integer? = 2
			buildCropping3DLayer(conf2, keras2)
			buildCroppingSingleDim3DLayer(conf1, keras1)
			buildCroppingSingleDim3DLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildCropping3DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildCropping3DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_CROPPING_3D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.ArrayList padding = New java.util.ArrayList<Integer>()
			config(conf.getLAYER_FIELD_CROPPING()) = padding
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As Cropping3D = (New KerasCropping3D(layerConfig)).Cropping3DLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(CROPPING(0), layer.getCropping()(0))
			assertEquals(CROPPING(0), layer.getCropping()(1))
			assertEquals(CROPPING(1), layer.getCropping()(2))
			assertEquals(CROPPING(1), layer.getCropping()(3))
			assertEquals(CROPPING(2), layer.getCropping()(4))
			assertEquals(CROPPING(2), layer.getCropping()(5))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildCroppingSingleDim3DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildCroppingSingleDim3DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_CROPPING_3D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			config(conf.getLAYER_FIELD_CROPPING()) = CROPPING(0)
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As Cropping3D = (New KerasCropping3D(layerConfig)).Cropping3DLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(CROPPING(0), layer.getCropping()(0))
			assertEquals(CROPPING(0), layer.getCropping()(1))
		End Sub
	End Class

End Namespace