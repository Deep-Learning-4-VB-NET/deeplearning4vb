Imports System.Collections.Generic
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports Subsampling1DLayer = org.deeplearning4j.nn.conf.layers.Subsampling1DLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports Resources = org.nd4j.common.resources.Resources
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
'ORIGINAL LINE: @DisplayName("Keras Pooling 1 D Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasPooling1DTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasPooling1DTest
		Inherits BaseDL4JTest

		Private ReadOnly LAYER_NAME As String = "test_layer"

		Private ReadOnly KERNEL_SIZE() As Integer = { 2 }

		Private ReadOnly STRIDE() As Integer = { 4 }

		Private ReadOnly POOLING_TYPE As PoolingType = PoolingType.MAX

		Private ReadOnly BORDER_MODE_VALID As String = "valid"

		Private ReadOnly VALID_PADDING() As Integer = { 0, 0 }

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Pooling 1 D Layer") void testPooling1DLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPooling1DLayer()
			buildPooling1DLayer(conf1, keras1)
			buildPooling1DLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildPooling1DLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildPooling1DLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_MAX_POOLING_1D()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			If kerasVersion = 2 Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				java.util.ArrayList kernel = New java.util.ArrayList<Integer>()
				config(conf.getLAYER_FIELD_POOL_1D_SIZE()) = kernel
			Else
				config(conf.getLAYER_FIELD_POOL_1D_SIZE()) = KERNEL_SIZE(0)
			End If
			If kerasVersion = 2 Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				java.util.ArrayList stride = New java.util.ArrayList<Integer>()
				config(conf.getLAYER_FIELD_POOL_1D_STRIDES()) = stride
			Else
				config(conf.getLAYER_FIELD_POOL_1D_STRIDES()) = STRIDE(0)
			End If
			config(conf.getLAYER_FIELD_BORDER_MODE()) = BORDER_MODE_VALID
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As Subsampling1DLayer = (New KerasPooling1D(layerConfig)).Subsampling1DLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(KERNEL_SIZE(0), layer.getKernelSize()(0))
			assertEquals(STRIDE(0), layer.getStride()(0))
			assertEquals(POOLING_TYPE, layer.getPoolingType())
			assertEquals(ConvolutionMode.Truncate, layer.getConvolutionMode())
			assertEquals(VALID_PADDING(0), layer.getPadding()(0))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPooling1dNWHC() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPooling1dNWHC()
			Dim file As File = Resources.asFile("modelimport/keras/tfkeras/issue_9349.hdf5")
			Dim computationGraph As ComputationGraph = KerasModelImport.importKerasModelAndWeights(file.getAbsolutePath())
			Dim maxpooling1d As GraphVertex = computationGraph.getVertex("max_pooling1d")
			assertNotNull(maxpooling1d)
			Dim layer As Layer = maxpooling1d.Layer
			Dim subsampling1DLayer As org.deeplearning4j.nn.layers.convolution.subsampling.Subsampling1DLayer = DirectCast(layer, org.deeplearning4j.nn.layers.convolution.subsampling.Subsampling1DLayer)
			assertEquals(CNN2DFormat.NHWC,subsampling1DLayer.layerConf().getCnn2dDataFormat())
		End Sub


	End Class

End Namespace