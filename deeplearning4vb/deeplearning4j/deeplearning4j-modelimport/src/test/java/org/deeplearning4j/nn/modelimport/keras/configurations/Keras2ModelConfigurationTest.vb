Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports KerasSpaceToDepth = org.deeplearning4j.nn.modelimport.keras.layers.convolutional.KerasSpaceToDepth
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @Slf4j @DisplayName("Keras 2 Model Configuration Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class Keras2ModelConfigurationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class Keras2ModelConfigurationTest
		Inherits BaseDL4JTest

		Friend classLoader As ClassLoader = Me.GetType().getClassLoader()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("File Not Found Test") void fileNotFoundTest()
		Friend Overridable Sub fileNotFoundTest()
			assertThrows(GetType(System.InvalidOperationException), Sub()
			runModelConfigTest("modelimport/keras/foo/bar.json")
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Not A File Test") void notAFileTest()
		Friend Overridable Sub notAFileTest()
			assertThrows(GetType(IOException), Sub()
			runModelConfigTest("modelimport/keras/")
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Simple 222 Config Test") void simple222ConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub simple222ConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/model_2_2_2.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Simple 224 Config Test") void simple224ConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub simple224ConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/model_2_2_4.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Yolo 9000 Config Test") void yolo9000ConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub yolo9000ConfigTest()
			KerasLayer.registerCustomLayer("Lambda", GetType(KerasSpaceToDepth))
			runModelConfigTest("modelimport/keras/configs/keras2/yolo9000_tf_keras_2.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("L 1 l 2 Regularizer Dense Tf Config Test") void l1l2RegularizerDenseTfConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub l1l2RegularizerDenseTfConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/l1l2_regularizer_dense_tf_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Dga Classifier Tf Config Test") void dgaClassifierTfConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub dgaClassifierTfConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_dga_classifier_tf_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Conv Pooling 1 d Tf Config Test") void convPooling1dTfConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub convPooling1dTfConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_conv1d_pooling1d_tf_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Bidirectional Lstm Config Test") void bidirectionalLstmConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub bidirectionalLstmConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/bidirectional_lstm_tf_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Imdb Lstm Tf Sequential Config Test") void imdbLstmTfSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub imdbLstmTfSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/imdb_lstm_tf_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Imdb Lstm Th Sequential Config Test") void imdbLstmThSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub imdbLstmThSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/imdb_lstm_th_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Simple Rnn Config Test") void simpleRnnConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub simpleRnnConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/simple_rnn_tf_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Simple Prelu Config Test") void simplePreluConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub simplePreluConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/prelu_config_tf_keras_2.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Mlp Tf Sequential Config Test") void mnistMlpTfSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistMlpTfSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/mnist_mlp_tf_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Mlp Th Sequential Config Test") void mnistMlpThSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistMlpThSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/mnist_mlp_th_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Cnn Tf Sequential Config Test") void mnistCnnTfSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistCnnTfSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/mnist_cnn_tf_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Cnn Th Sequential Config Test") void mnistCnnThSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistCnnThSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/mnist_cnn_th_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Cnn No Bias Tf Sequential Config Test") void mnistCnnNoBiasTfSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistCnnNoBiasTfSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_mnist_cnn_no_bias_tf_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mlp Sequential Config Test") void mlpSequentialConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mlpSequentialConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_mlp_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mlp Constraints Config Test") void mlpConstraintsConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mlpConstraintsConfigTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/mnist_mlp_constraint_tf_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Embedding Flatten Th Test") void embeddingFlattenThTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub embeddingFlattenThTest()
			runModelConfigTest("modelimport/keras/configs/keras2/embedding_flatten_graph_th_keras_2.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mlp Fapi Config Test") void mlpFapiConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mlpFapiConfigTest()
			runModelConfigTest("modelimport/keras/configs/keras2/keras2_mlp_fapi_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mlp Fapi Multi Loss Config Test") void mlpFapiMultiLossConfigTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mlpFapiMultiLossConfigTest()
			runModelConfigTest("modelimport/keras/configs/keras2/keras2_mlp_fapi_multiloss_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Cnn Tf Test") void cnnTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub cnnTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_cnn_tf_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Cnn Th Test") void cnnThTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub cnnThTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_cnn_th_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Cnn Tf Test") void mnistCnnTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistCnnTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_mnist_cnn_tf_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Mnist Mlp Tf Test") void mnistMlpTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub mnistMlpTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_mnist_mlp_tf_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Embedding Conv 1 D Tf Test") void embeddingConv1DTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub embeddingConv1DTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/keras2_tf_embedding_conv1d_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Flatten Conv 1 D Tf Test") void flattenConv1DTfTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub flattenConv1DTfTest()
			runSequentialConfigTest("modelimport/keras/configs/keras2/flatten_conv1d_tf_keras_2.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Embedding LSTM Mask Zero Test") void embeddingLSTMMaskZeroTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub embeddingLSTMMaskZeroTest()
			Dim path As String = "modelimport/keras/configs/keras2/embedding_lstm_calculator.json"
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(path)
				Dim config As ComputationGraphConfiguration = (New KerasModel()).modelBuilder().modelJsonInputStream([is]).enforceTrainingConfig(False).buildModel().ComputationGraphConfiguration
				Dim model As New ComputationGraph(config)
				model.init()
				Dim output As INDArray = model.outputSingle(Nd4j.zeros(1, 3))
				Console.WriteLine(output.shapeInfoToString())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Permute Retina Unet") void permuteRetinaUnet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub permuteRetinaUnet()
			runModelConfigTest("modelimport/keras/configs/keras2/permute_retina_unet.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Simple Add Layer Test") void simpleAddLayerTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub simpleAddLayerTest()
			runModelConfigTest("modelimport/keras/configs/keras2/simple_add_tf_keras_2.json")
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 999999999L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Embedding Concat Test") void embeddingConcatTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub embeddingConcatTest()
			runModelConfigTest("/modelimport/keras/configs/keras2/model_concat_embedding_sequences_tf_keras_2.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Conv 1 d Dilation Test") void conv1dDilationTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub conv1dDilationTest()
			runModelConfigTest("/modelimport/keras/configs/keras2/conv1d_dilation_tf_keras_2_config.json")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test 5982") void test5982() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub test5982()
			Dim jsonFile As File = Resources.asFile("modelimport/keras/configs/bidirectional_last_timeStep.json")
			Dim modelGraphConf As val = KerasModelImport.importKerasSequentialConfiguration(jsonFile.getAbsolutePath())
			Dim model As New MultiLayerNetwork(modelGraphConf)
			Dim features As INDArray = Nd4j.create(New Double() { 1, 3, 1, 2, 2, 1, 82, 2, 10, 1, 3, 1, 2, 1, 82, 3, 1, 10, 1, 2, 1, 3, 1, 10, 82, 2, 1, 1, 10, 82, 2, 3, 1, 2, 1, 10, 1, 2, 3, 82, 2, 1, 10, 3, 82, 1, 2, 1, 10, 1 }, New Integer() { 1, 1, 50 })
			model.init()
			Dim [out] As INDArray = model.output(features)
			assertArrayEquals(New Long() { 1, 14 }, [out].shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("One Lstm Layer Test") @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) void oneLstmLayerTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub oneLstmLayerTest()
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream("/modelimport/keras/configs/keras2/one_lstm_no_sequences_tf_keras_2.json")
				Dim config As MultiLayerConfiguration = (New KerasModel()).modelBuilder().modelJsonInputStream([is]).enforceTrainingConfig(False).buildSequential().MultiLayerConfiguration
				Dim model As New MultiLayerNetwork(config)
				model.init()
				' NWC format - [Minibatch, seqLength, channels]
				Dim input As INDArray = Nd4j.create(DataType.FLOAT, 50, 1500, 500)
				Dim [out] As INDArray = model.output(input)
				assertTrue([out].shape().SequenceEqual(New Long() { 50, 64 }))
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Reshape Embedding Concat Test") void ReshapeEmbeddingConcatTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub ReshapeEmbeddingConcatTest()
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream("/modelimport/keras/configs/keras2/reshape_embedding_concat.json")
				Dim config As ComputationGraphConfiguration = (New KerasModel()).modelBuilder().modelJsonInputStream([is]).enforceTrainingConfig(False).buildModel().ComputationGraphConfiguration
				Dim model As New ComputationGraph(config)
				model.init()
				' System.out.println(model.summary());
				model.outputSingle(Nd4j.zeros(1, 1), Nd4j.zeros(1, 1), Nd4j.zeros(1, 1))
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void runSequentialConfigTest(String path) throws Exception
		Private Sub runSequentialConfigTest(ByVal path As String)
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(path)
				Dim config As MultiLayerConfiguration = (New KerasModel()).modelBuilder().modelJsonInputStream([is]).enforceTrainingConfig(False).buildSequential().MultiLayerConfiguration
				Dim model As New MultiLayerNetwork(config)
				model.init()
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void runModelConfigTest(String path) throws Exception
		Private Sub runModelConfigTest(ByVal path As String)
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(path)
				Dim config As ComputationGraphConfiguration = (New KerasModel()).modelBuilder().modelJsonInputStream([is]).enforceTrainingConfig(False).buildModel().ComputationGraphConfiguration
				Dim model As New ComputationGraph(config)
				model.init()
			End Using
		End Sub
	End Class

End Namespace