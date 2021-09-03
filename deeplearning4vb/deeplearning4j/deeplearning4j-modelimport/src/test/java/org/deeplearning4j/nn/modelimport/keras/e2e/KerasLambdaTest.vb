Imports System
Imports System.IO
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SameDiffLambdaLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports KerasSequentialModel = org.deeplearning4j.nn.modelimport.keras.KerasSequentialModel
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Resources = org.nd4j.common.resources.Resources
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

	''' <summary>
	''' Test importing Keras models with multiple Lamdba layers.
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Keras Lambda Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag @Tag(TagNames.SAMEDIFF) class KerasLambdaTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasLambdaTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Exponential Lambda") class ExponentialLambda extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
		<Serializable>
		Friend Class ExponentialLambda
			Inherits SameDiffLambdaLayer

			Private ReadOnly outerInstance As KerasLambdaTest

			Public Sub New(ByVal outerInstance As KerasLambdaTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function defineLayer(ByVal sd As SameDiff, ByVal x As SDVariable) As SDVariable
				Return x.mul(x)
			End Function

			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
				Return inputType
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Times Three Lambda") class TimesThreeLambda extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaLayer
		<Serializable>
		Friend Class TimesThreeLambda
			Inherits SameDiffLambdaLayer

			Private ReadOnly outerInstance As KerasLambdaTest

			Public Sub New(ByVal outerInstance As KerasLambdaTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function defineLayer(ByVal sd As SameDiff, ByVal x As SDVariable) As SDVariable
				Return x.mul(3)
			End Function

			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
				Return inputType
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Sequential Lambda Layer Import") void testSequentialLambdaLayerImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSequentialLambdaLayerImport()
			KerasLayer.registerLambdaLayer("lambda_1", New ExponentialLambda(Me))
			KerasLayer.registerLambdaLayer("lambda_2", New TimesThreeLambda(Me))
			Dim modelPath As String = "modelimport/keras/examples/lambda/sequential_lambda.h5"
			Try
					Using [is] As Stream = Resources.asStream(modelPath)
					Dim modelFile As File = testDir.resolve("tempModel" & DateTimeHelper.CurrentUnixTimeMillis() & ".h5").toFile()
					Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
					Dim model As MultiLayerNetwork = (New KerasSequentialModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(False).buildSequential().MultiLayerNetwork
					Console.WriteLine(model.summary())
					Dim input As INDArray = Nd4j.create(New Integer() { 10, 100 })
					model.output(input)
					End Using
			Finally
				KerasLayer.clearLambdaLayers()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Model Lambda Layer Import") void testModelLambdaLayerImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testModelLambdaLayerImport()
			KerasLayer.registerLambdaLayer("lambda_3", New ExponentialLambda(Me))
			KerasLayer.registerLambdaLayer("lambda_4", New TimesThreeLambda(Me))
			Dim modelPath As String = "modelimport/keras/examples/lambda/model_lambda.h5"
			Try
					Using [is] As Stream = Resources.asStream(modelPath)
					Dim modelFile As File = testDir.resolve("tempModel" & DateTimeHelper.CurrentUnixTimeMillis() & ".h5").toFile()
					Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
					Dim model As ComputationGraph = (New KerasModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(False).buildModel().ComputationGraph
					Console.WriteLine(model.summary())
					Dim input As INDArray = Nd4j.create(New Integer() { 10, 784 })
					model.output(input)
					End Using
			Finally
				' Clear all lambdas, so other tests aren't affected.
				KerasLayer.clearLambdaLayers()
			End Try
		End Sub
	End Class

End Namespace