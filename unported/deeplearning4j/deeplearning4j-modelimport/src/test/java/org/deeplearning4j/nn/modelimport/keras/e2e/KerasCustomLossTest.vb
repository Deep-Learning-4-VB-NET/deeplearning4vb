Imports System
Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasSequentialModel = org.deeplearning4j.nn.modelimport.keras.KerasSequentialModel
Imports KerasLossUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasLossUtils
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports SameDiffLoss = org.nd4j.linalg.lossfunctions.SameDiffLoss
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
Namespace org.deeplearning4j.nn.modelimport.keras.e2e

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Custom Loss Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.LOSS_FUNCTIONS) class KerasCustomLossTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasCustomLossTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Log Cosh") class LogCosh extends org.nd4j.linalg.lossfunctions.SameDiffLoss
		<Serializable>
		Friend Class LogCosh
			Inherits SameDiffLoss

			Private ReadOnly outerInstance As KerasCustomLossTest

			Public Sub New(ByVal outerInstance As KerasCustomLossTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function defineLoss(ByVal sd As SameDiff, ByVal layerInput As SDVariable, ByVal labels As SDVariable) As SDVariable
				Return sd.math_Conflict.log(sd.math_Conflict.cosh(labels.sub(layerInput)))
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sequential Lambda Layer Import") void testSequentialLambdaLayerImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequentialLambdaLayerImport()
			KerasLossUtils.registerCustomLoss("logcosh", New LogCosh(Me))
			Dim modelPath As String = "modelimport/keras/examples/custom_loss.h5"
			Try
					Using [is] As Stream = Resources.asStream(modelPath)
					Dim modelFile As File = testDir.resolve("tempModel" & DateTimeHelper.CurrentUnixTimeMillis() & ".h5").toFile()
					Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
					Dim model As MultiLayerNetwork = (New KerasSequentialModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(True).buildSequential().MultiLayerNetwork
					Console.WriteLine(model.summary())
					Dim input As INDArray = Nd4j.create(New Integer() { 10, 3 })
					model.output(input)
					End Using
			Finally
				KerasLossUtils.clearCustomLoss()
			End Try
		End Sub
	End Class

End Namespace