Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Convolution = org.nd4j.linalg.convolution.Convolution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
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
Namespace org.deeplearning4j.nn.modelimport.keras.configurations

	''' <summary>
	''' Test import of Keras models.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Keras Model Import Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasModelImportTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasModelImportTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 9999999999999L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test H 5 Without Tensorflow Scope") void testH5WithoutTensorflowScope() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testH5WithoutTensorflowScope()
			Dim model As MultiLayerNetwork = loadModel("modelimport/keras/tfscope/model.h5")
			assertNotNull(model)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @DisplayName("Test NCHWNWHC Change Import") void testNCHWNWHCChangeImport()
		Friend Overridable Sub testNCHWNWHCChangeImport()
			Dim model As MultiLayerNetwork = loadModel("modelimport/keras/weights/conv2dnchw/simpleconv2d.hdf5")
			Dim multiLayerConfiguration As MultiLayerConfiguration = model.LayerWiseConfigurations
			Dim convolutionLayer As ConvolutionLayer = CType(multiLayerConfiguration.getConf(0).getLayer(), ConvolutionLayer)
			assertEquals(CNN2DFormat.NCHW, convolutionLayer.getCnn2dDataFormat())
			Dim subsamplingLayer As SubsamplingLayer = CType(multiLayerConfiguration.getConf(1).getLayer(), SubsamplingLayer)
			assertEquals(CNN2DFormat.NHWC, subsamplingLayer.getCnn2dDataFormat())
			Dim convolutionLayer1 As ConvolutionLayer = CType(multiLayerConfiguration.getConf(2).getLayer(), ConvolutionLayer)
			assertEquals(CNN2DFormat.NHWC, convolutionLayer1.getCnn2dDataFormat())
			model.output(Nd4j.zeros(1, 1, 28, 28))
			assertNotNull(model)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test H 5 With Tensorflow Scope") void testH5WithTensorflowScope() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testH5WithTensorflowScope()
			Dim model As MultiLayerNetwork = loadModel("modelimport/keras/tfscope/model.h5.with.tensorflow.scope")
			assertNotNull(model)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Weight And Json Without Tensorflow Scope") void testWeightAndJsonWithoutTensorflowScope() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWeightAndJsonWithoutTensorflowScope()
			Dim model As MultiLayerNetwork = loadModel("modelimport/keras/tfscope/model.json", "modelimport/keras/tfscope/model.weight")
			assertNotNull(model)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Weight And Json With Tensorflow Scope") void testWeightAndJsonWithTensorflowScope() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWeightAndJsonWithTensorflowScope()
			Dim model As MultiLayerNetwork = loadModel("modelimport/keras/tfscope/model.json.with.tensorflow.scope", "modelimport/keras/tfscope/model.weight.with.tensorflow.scope")
			assertNotNull(model)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.multilayer.MultiLayerNetwork loadModel(String modelJsonFilename, String modelWeightFilename) throws NullPointerException
		Private Function loadModel(ByVal modelJsonFilename As String, ByVal modelWeightFilename As String) As MultiLayerNetwork
			Dim network As MultiLayerNetwork = Nothing
			Try
				network = KerasModelImport.importKerasSequentialModelAndWeights(Resources.asFile(modelJsonFilename).getAbsolutePath(), Resources.asFile(modelWeightFilename).getAbsolutePath(), False)
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is InvalidKerasConfigurationException OrElse TypeOf e Is UnsupportedKerasConfigurationException
				log.error("", e)
			End Try
			Return network
		End Function

		Private Function loadModel(ByVal modelFilename As String) As MultiLayerNetwork
			Dim model As MultiLayerNetwork = Nothing
			Try
				model = KerasModelImport.importKerasSequentialModelAndWeights(Resources.asFile(modelFilename).getAbsolutePath())
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is InvalidKerasConfigurationException OrElse TypeOf e Is UnsupportedKerasConfigurationException
				log.error("", e)
			End Try
			Return model
		End Function
	End Class

End Namespace