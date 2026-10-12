Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports KerasSpaceToDepth = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasSpaceToDepth
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Resources = org.nd4j.common.resources.Resources
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
'ORIGINAL LINE: @Slf4j @DisplayName("Keras Yolo 9000 Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasYolo9000Test extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasYolo9000Test
		Inherits BaseDL4JTest

		Private Const TEMP_MODEL_FILENAME As String = "tempModel"

		Private Const H5_EXTENSION As String = ".h5"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test @DisplayName("Test Custom Layer Yolo Import") void testCustomLayerYoloImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCustomLayerYoloImport()
			KerasLayer.registerCustomLayer("Lambda", GetType(KerasSpaceToDepth))
			Dim modelPath As String = "modelimport/keras/examples/yolo/yolo.h5"
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(modelPath)
				Dim modelFile As File = testDir.resolve(TEMP_MODEL_FILENAME & DateTimeHelper.CurrentUnixTimeMillis() & H5_EXTENSION).toFile()
				Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
				Dim model As ComputationGraph = (New KerasModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(False).buildModel().ComputationGraph
				Console.WriteLine(model.summary())
			End Using
		End Sub
	End Class

End Namespace