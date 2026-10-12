Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
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
Namespace org.deeplearning4j.nn.modelimport.keras.configurations

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Keras 1 Model Configuration Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class Keras1ModelConfigurationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class Keras1ModelConfigurationTest
		Inherits BaseDL4JTest

		Private classLoader As ClassLoader = Me.GetType().getClassLoader()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Imdb Lstm Tf Sequential Config Test") void imdbLstmTfSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub imdbLstmTfSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/imdb_lstm_tf_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Imdb Lstm Th Sequential Config Test") void imdbLstmThSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub imdbLstmThSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/imdb_lstm_th_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Mlp Tf Sequential Config Test") void mnistMlpTfSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistMlpTfSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_mlp_tf_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Mlp Th Sequential Config Test") void mnistMlpThSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistMlpThSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_mlp_th_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Cnn Tf Sequential Config Test") void mnistCnnTfSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistCnnTfSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_cnn_tf_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Cnn No Bias Tf Sequential Config Test") void mnistCnnNoBiasTfSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistCnnNoBiasTfSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_cnn_no_bias_tf_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Cnn Th Sequential Config Test") void mnistCnnThSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistCnnThSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_cnn_th_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mlp Sequential Config Test") void mlpSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mlpSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mlp_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mlp Constraints Config Test") void mlpConstraintsConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mlpConstraintsConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_mlp_constraint_tf_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Reshape Mlp Config Test") void reshapeMlpConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub reshapeMlpConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_mlp_reshape_tf_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Reshape Cnn Config Test") void reshapeCnnConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub reshapeCnnConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_cnn_reshape_tf_keras_1_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mlp Fapi Config Test") void mlpFapiConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mlpFapiConfigTest()
			runModelConfigTest("modelimport/keras/configs/keras1/mlp_fapi_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mlp Fapi Multi Loss Config Test") void mlpFapiMultiLossConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mlpFapiMultiLossConfigTest()
			runModelConfigTest("modelimport/keras/configs/keras1/mlp_fapi_multiloss_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Yolo Config Test") void yoloConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub yoloConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/yolo_model.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Cnn Tf Test") void cnnTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub cnnTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/cnn_tf_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Cnn Th Test") void cnnThTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub cnnThTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/cnn_th_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Lstm Fixed Len Test") void lstmFixedLenTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub lstmFixedLenTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/lstm_tddense_config.json", False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Cnn Tf Test") void mnistCnnTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistCnnTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_cnn_tf_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Mlp Tf Test") void mnistMlpTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistMlpTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/mnist_mlp_tf_config.json", True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Embedding Conv 1 D Tf Test") void embeddingConv1DTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub embeddingConv1DTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras1/keras1_tf_embedding_conv1d_config.json", True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void runSequentialConfigTest(String path, boolean training) throws Exception
		Private Sub runSequentialConfigTest(ByVal path As String, ByVal training As Boolean)
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(path)
				Dim config As MultiLayerConfiguration = (New KerasModel()).modelBuilder().modelJsonInputStream([is]).enforceTrainingConfig(training).buildSequential().MultiLayerConfiguration
				Dim model As New MultiLayerNetwork(config)
				model.init()
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void runModelConfigTest(String path) throws Exception
		Private Sub runModelConfigTest(ByVal path As String)
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(path)
				Dim config As ComputationGraphConfiguration = (New KerasModel()).modelBuilder().modelJsonInputStream([is]).enforceTrainingConfig(True).buildModel().ComputationGraphConfiguration
				Dim model As New ComputationGraph(config)
				model.init()
			End Using
		End Sub
	End Class

End Namespace