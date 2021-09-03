Imports System.Collections.Generic
Imports EmbeddingSequenceLayer = org.deeplearning4j.nn.conf.layers.EmbeddingSequenceLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.embeddings

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Embedding Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasEmbeddingTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasEmbeddingTest
		Inherits BaseDL4JTest

		Private ReadOnly LAYER_NAME As String = "embedding_sequence_layer"

		Private ReadOnly INIT_KERAS As String = "glorot_normal"

		Private ReadOnly INPUT_SHAPE() As Integer = { 100, 20 }

		Private Shared ReadOnly MASK_ZERO() As Boolean = { False, True }

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Layer") void testEmbeddingLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testEmbeddingLayer()
			For Each mz As Boolean In MASK_ZERO
				buildEmbeddingLayer(conf1, keras1, mz)
				buildEmbeddingLayer(conf2, keras2, mz)
			Next mz
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Layer Set Weights Mask Zero") void testEmbeddingLayerSetWeightsMaskZero() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testEmbeddingLayerSetWeightsMaskZero()
			' GIVEN keras embedding with mask zero true
			Dim embedding As KerasEmbedding = buildEmbeddingLayer(conf1, keras1, True)
			' WHEN
			embedding.Weights = Collections.singletonMap(conf1.getLAYER_FIELD_EMBEDDING_WEIGHTS(), Nd4j.ones(INPUT_SHAPE))
			' THEN first row is set to zeros
			Dim weights As INDArray = embedding.getWeights()(DefaultParamInitializer.WEIGHT_KEY)
			assertEquals(embedding.getWeights()(DefaultParamInitializer.WEIGHT_KEY).columns(), INPUT_SHAPE(1))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private KerasEmbedding buildEmbeddingLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion, boolean maskZero) throws Exception
		Private Function buildEmbeddingLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?, ByVal maskZero As Boolean) As KerasEmbedding
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_EMBEDDING()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim inputDim As Integer? = 10
			Dim inputLength As Integer? = 1
			Dim outputDim As Integer? = 10
			config(conf.getLAYER_FIELD_INPUT_DIM()) = inputDim
			config(conf.getLAYER_FIELD_INPUT_LENGTH()) = inputLength
			config(conf.getLAYER_FIELD_OUTPUT_DIM()) = outputDim
			Dim inputShape As IList(Of Integer) = New List(Of Integer)(INPUT_SHAPE.Length)
			For Each i As Integer In INPUT_SHAPE
				inputShape.Add(i)
			Next i
			config(conf.getLAYER_FIELD_BATCH_INPUT_SHAPE()) = inputShape
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			If kerasVersion = 1 Then
				config(conf.getLAYER_FIELD_EMBEDDING_INIT()) = INIT_KERAS
			Else
				Dim init As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
				init("class_name") = conf.getINIT_GLOROT_NORMAL()
				config(conf.getLAYER_FIELD_EMBEDDING_INIT()) = init
			End If
			config(conf.getLAYER_FIELD_MASK_ZERO()) = maskZero
			Dim kerasEmbedding As New KerasEmbedding(layerConfig, False)
			assertEquals(kerasEmbedding.NumParams, 1)
			assertEquals(kerasEmbedding.isZeroMasking(), maskZero)
			Dim layer As EmbeddingSequenceLayer = kerasEmbedding.EmbeddingLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			Return kerasEmbedding
		End Function
	End Class

End Namespace