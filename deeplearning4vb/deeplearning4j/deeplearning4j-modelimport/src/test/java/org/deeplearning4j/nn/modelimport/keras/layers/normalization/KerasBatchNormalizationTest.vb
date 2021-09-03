Imports System
Imports System.Collections.Generic
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports StrumpfResolver = org.nd4j.common.resources.strumpf.StrumpfResolver
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.normalization


	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Batch Normalization Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasBatchNormalizationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasBatchNormalizationTest
		Inherits BaseDL4JTest

		Public Const PARAM_NAME_BETA As String = "beta"

		Private ReadOnly LAYER_NAME As String = "batch_norm_layer"

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Batchnorm Layer") void testBatchnormLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBatchnormLayer()
			buildBatchNormalizationLayer(conf1, keras1)
			buildBatchNormalizationLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildBatchNormalizationLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildBatchNormalizationLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim epsilon As Double = 1E-5
			Dim momentum As Double = 0.99
			Dim batchNormalization As New KerasBatchNormalization(kerasVersion)
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_BATCHNORMALIZATION()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config(conf.getLAYER_FIELD_NAME()) = LAYER_NAME
			config(batchNormalization.getLAYER_FIELD_EPSILON()) = epsilon
			config(batchNormalization.getLAYER_FIELD_MOMENTUM()) = momentum
			config(batchNormalization.getLAYER_FIELD_GAMMA_REGULARIZER()) = Nothing
			config(batchNormalization.getLAYER_FIELD_BETA_REGULARIZER()) = Nothing
			config(batchNormalization.getLAYER_FIELD_MODE()) = 0
			config(batchNormalization.getLAYER_FIELD_AXIS()) = 3
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim layer As BatchNormalization = (New KerasBatchNormalization(layerConfig)).BatchNormalizationLayer
			assertEquals(LAYER_NAME, layer.LayerName)
			assertEquals(epsilon, layer.getEps(), 0.0)
			assertEquals(momentum, layer.getDecay(), 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Weights") void testSetWeights() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSetWeights()
			Dim weights As IDictionary(Of String, INDArray) = weightsWithoutGamma()
			Dim batchNormalization As New KerasBatchNormalization(keras2)
			batchNormalization.setScale(False)
			batchNormalization.Weights = weights
			Dim size As Integer = batchNormalization.getWeights().Count
			assertEquals(4, size)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNN1d with batch norm") public void testWithCnn1d() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWithCnn1d()
			Dim absolutePath As String = Resources.asFile("modelimport/keras/tfkeras/batchNormError.h5").getAbsolutePath()
			Dim computationGraph As ComputationGraph = KerasModelImport.importKerasModelAndWeights(absolutePath)
			Dim sampleInput As INDArray = Nd4j.ones(25,25,25)
			Dim output() As INDArray = computationGraph.output(sampleInput)
			assertArrayEquals(New Long(){25, 24, 10},output(0).shape())

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNN1d with batch norm") public void testWithCnn1d2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWithCnn1d2()
			Dim absolutePath As String = Resources.asFile("modelimport/keras/tfkeras/batchNormError2.h5").getAbsolutePath()
			Dim computationGraph As ComputationGraph = KerasModelImport.importKerasModelAndWeights(absolutePath)
			Console.WriteLine(computationGraph.summary())
			Dim sampleInput As INDArray = Nd4j.ones(25,25,25)
			Dim output() As INDArray = computationGraph.output(sampleInput)
			assertArrayEquals(New Long(){25, 24, 512},output(0).shape())

		End Sub
		Private Function weightsWithoutGamma() As IDictionary(Of String, INDArray)
			Dim weights As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			weights(conf2.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_VARIANCE()) = Nd4j.ones(1L)
			weights(conf2.getLAYER_FIELD_BATCHNORMALIZATION_MOVING_MEAN()) = Nd4j.ones(1L)
			weights(PARAM_NAME_BETA) = Nd4j.ones(1)
			Return weights
		End Function
	End Class

End Namespace