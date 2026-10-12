Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports KerasLRN = org.deeplearning4j.nn.modelimport.keras.layers.custom.KerasLRN
Imports KerasPoolHelper = org.deeplearning4j.nn.modelimport.keras.layers.custom.KerasPoolHelper
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
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
Namespace org.deeplearning4j.nn.modelimport.keras.e2e

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Keras Custom Layer Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) class KerasCustomLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasCustomLayerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

		' run manually, might take a long time to load (too long for unit tests)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test @DisplayName("Test Custom Layer Import") void testCustomLayerImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCustomLayerImport()
			' file paths
			Dim kerasWeightsAndConfigUrl As String = DL4JResources.getURLString("googlenet_keras_weightsandconfig.h5")
			Dim cachedKerasFile As File = testDir.resolve("googlenet_keras_weightsandconfig.h5").toFile()
			Dim newFile As New File(testDir.toFile(),"googlenet_dl4j_inference.zip")
			Dim outputPath As String = newFile.getAbsolutePath()
			KerasLayer.registerCustomLayer("PoolHelper", GetType(KerasPoolHelper))
			KerasLayer.registerCustomLayer("LRN", GetType(KerasLRN))
			' download file
			If Not cachedKerasFile.exists() Then
				log.info("Downloading model to " & cachedKerasFile.ToString())
				FileUtils.copyURLToFile(New URL(kerasWeightsAndConfigUrl), cachedKerasFile)
				cachedKerasFile.deleteOnExit()
			End If
			Dim importedModel As org.deeplearning4j.nn.api.Model = KerasModelImport.importKerasModelAndWeights(cachedKerasFile.getAbsolutePath())
			ModelSerializer.writeModel(importedModel, outputPath, False)
			Dim serializedModel As ComputationGraph = ModelSerializer.restoreComputationGraph(outputPath)
			log.info(serializedModel.summary())
		End Sub
	End Class

End Namespace