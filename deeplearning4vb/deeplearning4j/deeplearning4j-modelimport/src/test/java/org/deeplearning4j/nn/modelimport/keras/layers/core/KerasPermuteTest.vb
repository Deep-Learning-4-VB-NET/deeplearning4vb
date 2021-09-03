Imports System.Collections.Generic
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Keras1LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras1LayerConfiguration
Imports Keras2LayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.Keras2LayerConfiguration
Imports KerasLayerConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports PermutePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.PermutePreprocessor
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
Namespace org.deeplearning4j.nn.modelimport.keras.layers.core

	''' <summary>
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Permute Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasPermuteTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasPermuteTest
		Inherits BaseDL4JTest

		Private keras1 As Integer? = 1

		Private keras2 As Integer? = 2

		Private conf1 As New Keras1LayerConfiguration()

		Private conf2 As New Keras2LayerConfiguration()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Permute Layer") void testPermuteLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPermuteLayer()
			buildPermuteLayer(conf1, keras1)
			buildPermuteLayer(conf2, keras2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void buildPermuteLayer(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion) throws Exception
		Private Sub buildPermuteLayer(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?)
			Dim permuteIndices() As Integer = { 2, 1 }
			Dim permuteList As IList(Of Integer) = New List(Of Integer)()
			permuteList.Add(permuteIndices(0))
			permuteList.Add(permuteIndices(1))
			Dim preProcessor As PermutePreprocessor = getPermutePreProcessor(conf, kerasVersion, permuteList)
			assertEquals(preProcessor.getPermutationIndices()(0), permuteIndices(0))
			assertEquals(preProcessor.getPermutationIndices()(1), permuteIndices(1))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.modelimport.keras.preprocessors.PermutePreprocessor getPermutePreProcessor(org.deeplearning4j.nn.modelimport.keras.config.KerasLayerConfiguration conf, System.Nullable<Integer> kerasVersion, java.util.List<Integer> permuteList) throws InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Private Function getPermutePreProcessor(ByVal conf As KerasLayerConfiguration, ByVal kerasVersion As Integer?, ByVal permuteList As IList(Of Integer)) As PermutePreprocessor
			Dim layerConfig As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			layerConfig(conf.getLAYER_FIELD_CLASS_NAME()) = conf.getLAYER_CLASS_NAME_RESHAPE()
			Dim config As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			config("dims") = permuteList
			config(conf.getLAYER_FIELD_NAME()) = "permute"
			layerConfig(conf.getLAYER_FIELD_CONFIG()) = config
			layerConfig(conf.getLAYER_FIELD_KERAS_VERSION()) = kerasVersion
			Dim inputType As InputType = InputType.InputTypeFeedForward.recurrent(20, 10)
			Return DirectCast((New KerasPermute(layerConfig)).getInputPreprocessor(inputType), PermutePreprocessor)
		End Function
	End Class

End Namespace