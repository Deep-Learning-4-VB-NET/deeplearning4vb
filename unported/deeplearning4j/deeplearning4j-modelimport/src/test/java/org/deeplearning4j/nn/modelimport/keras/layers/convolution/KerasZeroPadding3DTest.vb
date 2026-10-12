Imports System.Collections.Generic
Imports ZeroPadding3DLayer = org.deeplearning4j.nn.conf.layers.ZeroPadding3DLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports KerasZeroPadding3D = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasZeroPadding3D
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
'ORIGINAL LINE: @DisplayName("Keras Zero Padding 3 D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasZeroPadding3DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasZeroPadding3DTest
		Inherits BaseDL4JTest

		Private ReadOnly LAYER_NAME As String = "zero_padding_3D_layer"

		Private ReadOnly ZERO_PADDING() As Integer = { 2, 3, 4 }

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Zero Padding 3 D Layer") void testZeroPadding3DLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testZeroPadding3DLayer()
			Dim keras1 As Integer? = 1
			buildZeroPadding3DLayer(conf1, keras1)
			Dim keras2 As Integer? = 2
			buildZeroPadding3DLayer(conf2, keras2)
			buildZeroPaddingSingleDim3DLayer(conf1, keras1)
			buildZeroPaddingSingleDim3DLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildZeroPadding3DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildZeroPadding3DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_ZERO_PADDING_3D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.ArrayList padding = New java.util.ArrayList<Integer>()
			config(conf.getLAYER_FIELD_ZERO_PADDING()) = padding
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As ZeroPadding3DLayer = (New KerasZeroPadding3D(layerConfig)).ZeroPadding3DLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(ZERO_PADDING(0), layer.getPadding()(0))
			assertEquals(ZERO_PADDING(0), layer.getPadding()(1))
			assertEquals(ZERO_PADDING(1), layer.getPadding()(2))
			assertEquals(ZERO_PADDING(1), layer.getPadding()(3))
			assertEquals(ZERO_PADDING(2), layer.getPadding()(4))
			assertEquals(ZERO_PADDING(2), layer.getPadding()(5))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildZeroPaddingSingleDim3DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildZeroPaddingSingleDim3DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_ZERO_PADDING_3D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			config(conf.getLAYER_FIELD_ZERO_PADDING()) = ZERO_PADDING(0)
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As ZeroPadding3DLayer = (New KerasZeroPadding3D(layerConfig)).ZeroPadding3DLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(ZERO_PADDING(0), layer.getPadding()(0))
			assertEquals(ZERO_PADDING(0), layer.getPadding()(1))
		End Sub
	End Class

End Namespace