Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports KerasSpaceToDepth = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasSpaceToDepth
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
'ORIGINAL LINE: @Slf4j @DisplayName("Keras Yolo 9000 Predict Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasYolo9000PredictTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasYolo9000PredictTest
		Inherits BaseDL4JTest

		Private Const DL4J_MODEL_FILE_NAME As String = "."

		Private Shared IMAGE_PREPROCESSING_SCALER As New ImagePreProcessingScaler(0, 1)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Need to manually download file for ylo.") @DisplayName("Test Yolo Prediction Import") void testYoloPredictionImport() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testYoloPredictionImport()
			Dim HEIGHT As Integer = 416
			Dim WIDTH As Integer = 416
			Dim indArray As INDArray = Nd4j.create(HEIGHT, WIDTH, 3)
			IMAGE_PREPROCESSING_SCALER.transform(indArray)
			KerasLayer.registerCustomLayer("Lambda", GetType(KerasSpaceToDepth))
			Dim h5_FILENAME As String = "modelimport/keras/examples/yolo/yolo-voc.h5"
			Dim graph As ComputationGraph = KerasModelImport.importKerasModelAndWeights(h5_FILENAME, False)
			Dim priorBoxes()() As Double = {
				New Double() { 1.3221, 1.73145 },
				New Double() { 3.19275, 4.00944 },
				New Double() { 5.05587, 8.09892 },
				New Double() { 9.47112, 4.84053 },
				New Double() { 11.2364, 10.0071 }
			}
			Dim priors As INDArray = Nd4j.create(priorBoxes)
			Dim model As ComputationGraph = (New TransferLearning.GraphBuilder(graph)).addLayer("outputs", (New org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer.Builder()).boundingBoxPriors(priors).build(), "conv2d_23").setOutputs("outputs").build()
			ModelSerializer.writeModel(model, DL4J_MODEL_FILE_NAME, False)
			Dim computationGraph As ComputationGraph = ModelSerializer.restoreComputationGraph(New File(DL4J_MODEL_FILE_NAME))
			Console.WriteLine(computationGraph.summary(InputType.convolutional(416, 416, 3)))
			Dim results As INDArray = computationGraph.outputSingle(indArray)
		End Sub
	End Class

End Namespace