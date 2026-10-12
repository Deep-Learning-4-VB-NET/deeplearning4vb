Imports System.Collections.Generic
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.pooling

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Pooling 2 D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasPooling2DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasPooling2DTest
		Inherits BaseDL4JTest

		Private ReadOnly LAYER_NAME As String = "test_layer"

		Private ReadOnly KERNEL_SIZE() As Integer = { 1, 2 }

		Private ReadOnly STRIDE() As Integer = { 3, 4 }

		Private ReadOnly POOLING_TYPE As PoolingType = PoolingType.MAX

		Private ReadOnly BORDER_MODE_VALID As String = "valid"

		Private ReadOnly VALID_PADDING() As Integer = { 0, 0 }

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Pooling 2 D Layer") void testPooling2DLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPooling2DLayer()
			buildPooling2DLayer(conf1, keras1)
			buildPooling2DLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildPooling2DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildPooling2DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_MAX_POOLING_2D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			Dim kernelSizeList As IList(Of Integer) = New List(Of Integer)()
			kernelSizeList.Add(KERNEL_SIZE(0))
			kernelSizeList.Add(KERNEL_SIZE(1))
			config(conf.getLAYER_FIELD_POOL_SIZE()) = kernelSizeList
			Dim subsampleList As IList(Of Integer) = New List(Of Integer)()
			subsampleList.Add(STRIDE(0))
			subsampleList.Add(STRIDE(1))
			config(conf.getLAYER_FIELD_POOL_STRIDES()) = subsampleList
			config(conf.getLAYER_FIELD_BORDER_MODE()) = BORDER_MODE_VALID
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As SubsamplingLayer = (New KerasPooling2D(layerConfig)).Subsampling2DLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			assertArrayEquals(KERNEL_SIZE, layer.getKernelSize())
			assertArrayEquals(STRIDE, layer.getStride())
			assertEquals(POOLING_TYPE, layer.getPoolingType())
			assertEquals(ConvolutionMode.Truncate, layer.getConvolutionMode())
			assertArrayEquals(VALID_PADDING, layer.getPadding())
		End Sub
	End Class

End Namespace