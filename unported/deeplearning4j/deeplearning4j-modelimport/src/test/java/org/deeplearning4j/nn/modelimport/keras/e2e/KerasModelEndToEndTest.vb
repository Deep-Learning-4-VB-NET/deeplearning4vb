Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FileUtils = org.apache.commons.io.FileUtils
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ROCMultiClass = org.deeplearning4j.eval.ROCMultiClass
Imports GradientCheckUtil = org.deeplearning4j.gradientcheck.GradientCheckUtil
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports Convolution1DLayer = org.deeplearning4j.nn.conf.layers.Convolution1DLayer
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Hdf5Archive = org.deeplearning4j.nn.modelimport.keras.Hdf5Archive
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports KerasSequentialModel = org.deeplearning4j.nn.modelimport.keras.KerasSequentialModel
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports KerasModelBuilder = org.deeplearning4j.nn.modelimport.keras.utils.KerasModelBuilder
Imports KerasModelUtils = org.deeplearning4j.nn.modelimport.keras.utils.KerasModelUtils
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports FineTuneConfiguration = org.deeplearning4j.nn.transferlearning.FineTuneConfiguration
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports org.nd4j.linalg.activations.impl
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.function
Imports org.nd4j.common.function
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossSparseMCXENT = org.nd4j.linalg.lossfunctions.impl.LossSparseMCXENT
Imports Resources = org.nd4j.common.resources.Resources
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
import static org.junit.jupiter.api.Assertions.assertThrows
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
	''' Unit tests for end-to-end Keras model import.
	''' 
	''' @author dave@skymind.io, Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Keras Model End To End Test") @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag class KerasModelEndToEndTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class KerasModelEndToEndTest
		Inherits BaseDL4JTest

		Private Const GROUP_ATTR_INPUTS As String = "inputs"

		Private Const GROUP_ATTR_OUTPUTS As String = "outputs"

		Private Const GROUP_PREDICTIONS As String = "predictions"

		Private Const GROUP_ACTIVATIONS As String = "activations"

		Private Const TEMP_OUTPUTS_FILENAME As String = "tempOutputs"

		Private Const TEMP_MODEL_FILENAME As String = "tempModel"

		Private Const H5_EXTENSION As String = ".h5"

		Private Const EPS As Double = 1E-5

		Private Const SKIP_GRAD_CHECKS As Boolean = True



		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				' Most benchmarks should run very quickly; large timeout is to avoid issues with unusually slow download of test resources
				Return 900000000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("File Not Found End To End") void fileNotFoundEndToEnd(@TempDir Path tempDir)
		Friend Overridable Sub fileNotFoundEndToEnd(ByVal tempDir As Path)
			assertThrows(GetType(System.InvalidOperationException), Sub()
			Dim modelPath As String = "modelimport/keras/examples/foo/bar.h5"
			importEndModelTest(tempDir,modelPath, Nothing, True, True, False, False)
			End Sub)
		End Sub

		''' <summary>
		''' MNIST MLP tests
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Mnist Mlp Tf Keras 1") void importMnistMlpTfKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMnistMlpTfKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Mnist Mlp Th Keras 1") void importMnistMlpThKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMnistMlpThKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/mnist_mlp/mnist_mlp_th_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/mnist_mlp/mnist_mlp_th_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, False, True, False, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Mnist Mlp Tf Keras 2") void importMnistMlpTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMnistMlpTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Mnist Mlp Reshape Tf Keras 1") void importMnistMlpReshapeTfKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMnistMlpReshapeTfKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/mnist_mlp_reshape/mnist_mlp_reshape_tf_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/mnist_mlp_reshape/mnist_mlp_reshape_tf_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, True, False)
		End Sub

		''' <summary>
		''' MNIST CNN tests
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Mnist Cnn Tf Keras 1") void importMnistCnnTfKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMnistCnnTfKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/mnist_cnn/mnist_cnn_tf_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/mnist_cnn/mnist_cnn_tf_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, False, False, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Mnist Cnn Th Keras 1") void importMnistCnnThKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMnistCnnThKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/mnist_cnn/mnist_cnn_th_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/mnist_cnn/mnist_cnn_th_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, False, True, True, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Mnist Cnn Tf Keras 2") void importMnistCnnTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMnistCnnTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/mnist_cnn/mnist_cnn_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/mnist_cnn/mnist_cnn_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, True, False)
		End Sub

		''' <summary>
		''' IMDB Embedding and LSTM test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Imdb Lstm Tf Keras 1") void importImdbLstmTfKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importImdbLstmTfKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/imdb_lstm/imdb_lstm_tf_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/imdb_lstm/imdb_lstm_tf_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False, True, Nothing, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Imdb Lstm Th Keras 1") void importImdbLstmThKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importImdbLstmThKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/imdb_lstm/imdb_lstm_th_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/imdb_lstm/imdb_lstm_th_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False, True, Nothing, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Imdb Lstm Tf Keras 2") void importImdbLstmTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importImdbLstmTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/imdb_lstm/imdb_lstm_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/imdb_lstm/imdb_lstm_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False, True, Nothing, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Imdb Lstm Th Keras 2") void importImdbLstmThKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importImdbLstmThKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/imdb_lstm/imdb_lstm_th_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/imdb_lstm/imdb_lstm_th_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, False, True, False, False, True, Nothing, Nothing)
		End Sub

		''' <summary>
		''' IMDB LSTM fasttext
		''' </summary>
		' TODO: prediction checks fail due to globalpooling for fasttext, very few grads fail as well
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Imdb Fasttext Tf Keras 1") void importImdbFasttextTfKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importImdbFasttextTfKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/imdb_fasttext/imdb_fasttext_tf_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/imdb_fasttext/imdb_fasttext_tf_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, False, False, False, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Imdb Fasttext Th Keras 1") void importImdbFasttextThKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importImdbFasttextThKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/imdb_fasttext/imdb_fasttext_th_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/imdb_fasttext/imdb_fasttext_th_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, False, False, False, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Imdb Fasttext Tf Keras 2") void importImdbFasttextTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importImdbFasttextTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/imdb_fasttext/imdb_fasttext_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/imdb_fasttext/imdb_fasttext_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, False, False, False)
		End Sub

		''' <summary>
		''' Simple LSTM (return sequences = false) into Dense layer test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Simple Lstm Tf Keras 1") void importSimpleLstmTfKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSimpleLstmTfKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/simple_lstm/simple_lstm_tf_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/simple_lstm/simple_lstm_tf_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Simple Lstm Th Keras 1") void importSimpleLstmThKeras1(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSimpleLstmThKeras1(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/simple_lstm/simple_lstm_th_keras_1_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/simple_lstm/simple_lstm_th_keras_1_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Simple Lstm Tf Keras 2") void importSimpleLstmTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSimpleLstmTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/simple_lstm/simple_lstm_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/simple_lstm/simple_lstm_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, False, False, False)
		End Sub

		''' <summary>
		''' Simple LSTM (return sequences = true) into flatten into Dense layer test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Simple Flatten Lstm Tf Keras 2") void importSimpleFlattenLstmTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSimpleFlattenLstmTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/simple_flatten_lstm/simple_flatten_lstm_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/simple_flatten_lstm/" & "simple_flatten_lstm_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False)
		End Sub

		''' <summary>
		''' Simple RNN (return sequences = true) into flatten into Dense layer test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Simple Flatten Rnn Tf Keras 2") void importSimpleFlattenRnnTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSimpleFlattenRnnTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/simple_flatten_rnn/simple_flatten_rnn_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/simple_flatten_rnn/" & "simple_flatten_rnn_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False, True, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Simple RNN (return sequences = false) into Dense layer test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Simple Rnn Tf Keras 2") void importSimpleRnnTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSimpleRnnTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/simple_rnn/simple_rnn_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/simple_rnn/" & "simple_rnn_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, False, False)
		End Sub

		''' <summary>
		''' CNN without bias test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Cnn No Bias Tf Keras 2") void importCnnNoBiasTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importCnnNoBiasTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/cnn_no_bias/mnist_cnn_no_bias_tf_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/cnn_no_bias/mnist_cnn_no_bias_tf_keras_2_inputs_and_outputs.h5"
			importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, True, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Sparse Xent") void importSparseXent(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSparseXent(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/simple_sparse_xent/simple_sparse_xent_mlp_keras_2_model.h5"
			Dim inputsOutputPath As String = "modelimport/keras/examples/simple_sparse_xent/simple_sparse_xent_mlp_keras_2_inputs_and_outputs.h5"
			Dim net As MultiLayerNetwork = importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, True, True)
			Dim outLayer As Layer = net.OutputLayer
			assertTrue(TypeOf outLayer Is org.deeplearning4j.nn.layers.LossLayer)
			Dim llConf As LossLayer = DirectCast(outLayer.Config, LossLayer)
			assertEquals(New LossSparseMCXENT(), llConf.getLossFn())
		End Sub

		''' <summary>
		''' GAN import tests
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Dcgan Mnist Discriminator") void importDcganMnistDiscriminator(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importDcganMnistDiscriminator(ByVal tempDir As Path)
			importSequentialModelH5Test(tempDir,"modelimport/keras/examples/mnist_dcgan/dcgan_discriminator_epoch_50.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Neither keras or tfkeras can load this.") @DisplayName("Import Dcgan Mnist Generator") void importDcganMnistGenerator(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importDcganMnistGenerator(ByVal tempDir As Path)
			importSequentialModelH5Test(tempDir,"modelimport/keras/examples/mnist_dcgan/dcgan_generator_epoch_50.h5")
		End Sub

		''' <summary>
		''' Auxillary classifier GAN import test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Acgan Discriminator") void importAcganDiscriminator(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importAcganDiscriminator(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/acgan/acgan_discriminator_1_epochs.h5")
			' NHWC
			Dim input As INDArray = Nd4j.create(10, 28, 28, 1)
			Dim output() As INDArray = model.output(input)
		End Sub

		' AB 2020/04/22 Ignored until Keras model import updated to use NHWC support
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Acgan Generator") void importAcganGenerator(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importAcganGenerator(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/acgan/acgan_generator_1_epochs.h5")
			' System.out.println(model.summary()) ;
			Dim latent As INDArray = Nd4j.create(10, 100)
			Dim label As INDArray = Nd4j.create(10, 1)
			Dim output() As INDArray = model.output(latent, label)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Acgan Combined") void importAcganCombined(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importAcganCombined(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/acgan/acgan_combined_1_epochs.h5")
			' TODO: imports, but incorrectly. Has only one input, should have two.
		End Sub

		''' <summary>
		''' Deep convolutional GAN import test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Dcgan Discriminator") void importDcganDiscriminator(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importDcganDiscriminator(ByVal tempDir As Path)
			importSequentialModelH5Test(tempDir,"modelimport/keras/examples/gans/dcgan_discriminator.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Dcgan Generator") void importDcganGenerator(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importDcganGenerator(ByVal tempDir As Path)
			importSequentialModelH5Test(tempDir,"modelimport/keras/examples/gans/dcgan_generator.h5")
		End Sub

		''' <summary>
		''' Wasserstein GAN import test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Wgan Discriminator") void importWganDiscriminator(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importWganDiscriminator(ByVal tempDir As Path)
			For i As Integer = 0 To 99
				' run a few times to make sure HDF5 doesn't crash
				importSequentialModelH5Test(tempDir,"modelimport/keras/examples/gans/wgan_discriminator.h5")
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Wgan Generator") void importWganGenerator(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importWganGenerator(ByVal tempDir As Path)
			importSequentialModelH5Test(tempDir,"modelimport/keras/examples/gans/wgan_generator.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Cnn 1 d") void importCnn1d(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importCnn1d(ByVal tempDir As Path)
			importSequentialModelH5Test(tempDir,"modelimport/keras/examples/cnn1d/cnn1d_flatten_tf_keras2.h5")
		End Sub

		''' <summary>
		''' DGA classifier test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Dga Classifier") void importDgaClassifier(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importDgaClassifier(ByVal tempDir As Path)
			importSequentialModelH5Test(tempDir,"modelimport/keras/examples/dga_classifier/keras2_dga_classifier_tf_model.h5")
		End Sub

		''' <summary>
		''' Reshape flat input into 3D to fit into an LSTM model
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Flat Into LSTM") void importFlatIntoLSTM(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importFlatIntoLSTM(ByVal tempDir As Path)
			importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/reshape_to_rnn/reshape_model.h5")
		End Sub

		''' <summary>
		''' Functional LSTM test
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Functional Lstm Tf Keras 2") void importFunctionalLstmTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importFunctionalLstmTfKeras2(ByVal tempDir As Path)
			Dim modelPath As String = "modelimport/keras/examples/functional_lstm/lstm_functional_tf_keras_2.h5"
			' No training enabled
			Dim graphNoTrain As ComputationGraph = importFunctionalModelH5Test(tempDir,modelPath, Nothing, False)
			Console.WriteLine(graphNoTrain.summary())
			' Training enabled
			Dim graph As ComputationGraph = importFunctionalModelH5Test(tempDir,modelPath, Nothing, True)
			Console.WriteLine(graph.summary())
			' Make predictions
			Dim miniBatch As Integer = 32
			' NWC format - with nIn=4, seqLength = 10
			Dim input As INDArray = Nd4j.ones(miniBatch, 10, 4)
			Dim [out]() As INDArray = graph.output(input)
			' Fit model
			graph.fit(New INDArray() { input }, [out])
		End Sub

		''' <summary>
		''' U-Net
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Unet Tf Keras 2") void importUnetTfKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importUnetTfKeras2(ByVal tempDir As Path)
			importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/unet/unet_keras_2_tf.h5", Nothing, True)
		End Sub

		''' <summary>
		''' ResNet50
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Resnet 50") void importResnet50(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importResnet50(ByVal tempDir As Path)
			importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/resnet/resnet50_weights_tf_dim_ordering_tf_kernels.h5")
		End Sub

		''' <summary>
		''' DenseNet
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Dense Net") void importDenseNet(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importDenseNet(ByVal tempDir As Path)
			importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/densenet/densenet121_tf_keras_2.h5")
		End Sub

		''' <summary>
		''' SqueezeNet
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Squeeze Net") void importSqueezeNet(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSqueezeNet(ByVal tempDir As Path)
			importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/squeezenet/squeezenet.h5")
		End Sub

		''' <summary>
		''' MobileNet
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Mobile Net") void importMobileNet(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMobileNet(ByVal tempDir As Path)
			Dim graph As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/mobilenet/alternative.hdf5")
			Dim input As INDArray = Nd4j.ones(10, 299, 299, 3)
			graph.output(input)
		End Sub

		''' <summary>
		''' InceptionV3 Keras 2 no top
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Inception Keras 2") void importInceptionKeras2(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importInceptionKeras2(ByVal tempDir As Path)
			Dim inputShape() As Integer = { 299, 299, 3 }
			Dim graph As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/inception/inception_tf_keras_2.h5", inputShape, False)
			' TF = channels last = NHWC
			Dim input As INDArray = Nd4j.ones(10, 299, 299, 3)
			graph.output(input)
			Console.WriteLine(graph.summary())
		End Sub

		''' <summary>
		''' InceptionV3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Inception") void importInception(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importInception(ByVal tempDir As Path)
			Dim graph As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/inception/inception_v3_complete.h5")
			' TH = channels first = NCHW
			Dim input As INDArray = Nd4j.ones(10, 3, 299, 299)
			graph.output(input)
			Console.WriteLine(graph.summary())
		End Sub

		''' <summary>
		''' Inception V4
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @DisplayName("Import Inception V 4") void importInceptionV4(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importInceptionV4(ByVal testDir As Path)
			Dim modelUrl As String = DL4JResources.getURLString("models/inceptionv4_keras_imagenet_weightsandconfig.h5")
			Dim kerasFile As File = testDir.resolve("inceptionv4_keras_imagenet_weightsandconfig.h5").toFile()
			If Not kerasFile.exists() Then
				FileUtils.copyURLToFile(New URL(modelUrl), kerasFile)
				kerasFile.deleteOnExit()
			End If
			Dim inputShape() As Integer = { 299, 299, 3 }
			Dim graph As ComputationGraph = importFunctionalModelH5Test(testDir,kerasFile.getAbsolutePath(), inputShape, False)
			' System.out.println(graph.summary());
		End Sub

		''' <summary>
		''' Xception
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Xception") void importXception(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importXception(ByVal tempDir As Path)
			Dim inputShape() As Integer = { 299, 299, 3 }
			Dim graph As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/xception/xception_tf_keras_2.h5", inputShape, False)
		End Sub

		''' <summary>
		''' Seq2seq model
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import Seq 2 Seq") void importSeq2Seq(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSeq2Seq(ByVal tempDir As Path)
			importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/seq2seq/full_model_seq2seq_5549.h5")
		End Sub

		''' <summary>
		''' Import all AlphaGo Zero model variants, i.e.
		''' - Dual residual architecture
		''' - Dual convolutional architecture
		''' - Separate (policy and value) residual architecture
		''' - Separate (policy and value) convolutional architecture
		''' </summary>
		' AB 20200427 Bad keras model - Keras JSON has input shape [null, 10, 19, 19] (i.e., NCHW) but all layers are set to channels_last
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") @DisplayName("Import Sep Conv Policy") void importSepConvPolicy(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSepConvPolicy(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/agz/sep_conv_policy.h5")
			Dim input As INDArray = Nd4j.create(32, 19, 19, 10)
			model.output(input)
		End Sub

		' AB 20200427 Bad keras model - Keras JSON has input shape [null, 10, 19, 19] (i.e., NCHW) but all layers are set to channels_last
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") @DisplayName("Import Sep Res Policy") void importSepResPolicy(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSepResPolicy(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/agz/sep_res_policy.h5")
			Dim input As INDArray = Nd4j.create(32, 19, 19, 10)
			model.output(input)
		End Sub

		' AB 20200427 Bad keras model - Keras JSON has input shape [null, 10, 19, 19] (i.e., NCHW) but all layers are set to channels_last
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") @DisplayName("Import Sep Conv Value") void importSepConvValue(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSepConvValue(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/agz/sep_conv_value.h5")
			Dim input As INDArray = Nd4j.create(32, 19, 19, 10)
			model.output(input)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") @DisplayName("Import Sep Res Value") void importSepResValue(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importSepResValue(ByVal tempDir As Path)
			Dim filePath As String = "C:\Users\agibs\Documents\GitHub\keras1-import-test\sep_res_value.h5"
			Dim builder As KerasModelBuilder = (New KerasModel()).modelBuilder().modelHdf5Filename(filePath).enforceTrainingConfig(False)
			Dim model As KerasModel = builder.buildModel()
			Dim compGraph As ComputationGraph = model.ComputationGraph
			' ComputationGraph model = importFunctionalModelH5Test("modelimport/keras/examples/agz/sep_res_value.h5");
			Dim input As INDArray = Nd4j.create(32, 19, 19, 10)
			compGraph.output(input)
		End Sub

		' AB 20200427 Bad keras model - Keras JSON has input shape [null, 10, 19, 19] (i.e., NCHW) but all layers are set to channels_last
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") @DisplayName("Import Dual Res") void importDualRes(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importDualRes(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/agz/dual_res.h5")
			Dim input As INDArray = Nd4j.create(32, 19, 19, 10)
			model.output(input)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") @DisplayName("Import Dual Conv") void importDualConv(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importDualConv(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/agz/dual_conv.h5")
			Dim input As INDArray = Nd4j.create(32, 19, 19, 10)
			model.output(input)
		End Sub

		''' <summary>
		''' MTCNN
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import MTCNN") void importMTCNN(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMTCNN(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/48net_complete.h5")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") @DisplayName("Test NCHWNWHC Change Import Model") void testNCHWNWHCChangeImportModel(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNCHWNWHCChangeImportModel(ByVal tempDir As Path)
			Dim computationGraph As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/weights/simpleconv2d_model.hdf5")
			computationGraph.output(Nd4j.zeros(1, 1, 28, 28))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Import MTCNN 2 D") void importMTCNN2D(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub importMTCNN2D(ByVal tempDir As Path)
			Dim model As ComputationGraph = importFunctionalModelH5Test(tempDir,"modelimport/keras/examples/12net.h5", New Integer() { 24, 24, 3 }, False)
			Dim input As INDArray = Nd4j.create(10, 24, 24, 3)
			model.output(input)
			' System.out.println(model.summary());
		End Sub

		''' <summary>
		''' Masking layers (simple Masking into LSTM)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Masking Zero Value") void testMaskingZeroValue(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMaskingZeroValue(ByVal tempDir As Path)
			Dim model As MultiLayerNetwork = importSequentialModelH5Test(tempDir,"modelimport/keras/examples/masking/masking_zero_lstm.h5")
			model.summary()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Masking Two Value") void testMaskingTwoValue(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMaskingTwoValue(ByVal tempDir As Path)
			Dim model As MultiLayerNetwork = importSequentialModelH5Test(tempDir,"modelimport/keras/examples/masking/masking_two_lstm.h5")
			model.summary()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Causal Conv 1 D") void testCausalConv1D(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCausalConv1D(ByVal tempDir As Path)
			Dim names() As String = { "causal_conv1d_k2_s1_d1_cl_model.h5", "causal_conv1d_k2_s1_d2_cl_model.h5", "causal_conv1d_k2_s2_d1_cl_model.h5", "causal_conv1d_k2_s3_d1_cl_model.h5", "causal_conv1d_k3_s1_d1_cl_model.h5", "causal_conv1d_k3_s1_d2_cl_model.h5", "causal_conv1d_k3_s2_d1_cl_model.h5", "causal_conv1d_k3_s3_d1_cl_model.h5", "causal_conv1d_k4_s1_d1_cl_model.h5", "causal_conv1d_k4_s1_d2_cl_model.h5", "causal_conv1d_k4_s2_d1_cl_model.h5", "causal_conv1d_k4_s3_d1_cl_model.h5" }
			For Each name As String In names
				Console.WriteLine("Starting test: " & name)
				Dim modelPath As String = "modelimport/keras/examples/causal_conv1d/" & name
				Dim inputsOutputPath As String = "modelimport/keras/examples/causal_conv1d/" & (name.Substring(0, name.Length - "model.h5".Length) & "inputs_and_outputs.h5")
				' TODO:
				''' <summary>
				''' Difference in weights. Same elements, but loaded differently. Likely acceptable difference. Need to confirm though.
				''' </summary>
				Dim net As MultiLayerNetwork = importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, True, True, False, Nothing, Nothing)
				Dim l As Layer = net.getLayer(0)
				Dim c1d As Convolution1DLayer = DirectCast(l.Config, Convolution1DLayer)
				assertEquals(ConvolutionMode.Causal, c1d.getConvolutionMode())
			Next name
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Conv 1 D") void testConv1D(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testConv1D(ByVal tempDir As Path)
			Dim names() As String = { "conv1d_k2_s1_d1_cf_same_model.h5", "conv1d_k2_s1_d1_cf_valid_model.h5", "conv1d_k2_s1_d1_cl_same_model.h5", "conv1d_k2_s1_d1_cl_valid_model.h5", "conv1d_k2_s1_d2_cf_same_model.h5", "conv1d_k2_s1_d2_cf_valid_model.h5", "conv1d_k2_s1_d2_cl_same_model.h5", "conv1d_k2_s1_d2_cl_valid_model.h5", "conv1d_k2_s2_d1_cf_same_model.h5", "conv1d_k2_s2_d1_cf_valid_model.h5", "conv1d_k2_s2_d1_cl_same_model.h5", "conv1d_k2_s2_d1_cl_valid_model.h5", "conv1d_k2_s3_d1_cf_same_model.h5", "conv1d_k2_s3_d1_cf_valid_model.h5", "conv1d_k2_s3_d1_cl_same_model.h5", "conv1d_k2_s3_d1_cl_valid_model.h5", "conv1d_k3_s1_d1_cf_same_model.h5", "conv1d_k3_s1_d1_cf_valid_model.h5", "conv1d_k3_s1_d1_cl_same_model.h5", "conv1d_k3_s1_d1_cl_valid_model.h5", "conv1d_k3_s1_d2_cf_same_model.h5", "conv1d_k3_s1_d2_cf_valid_model.h5", "conv1d_k3_s1_d2_cl_same_model.h5", "conv1d_k3_s1_d2_cl_valid_model.h5", "conv1d_k3_s2_d1_cf_same_model.h5", "conv1d_k3_s2_d1_cf_valid_model.h5", "conv1d_k3_s2_d1_cl_same_model.h5", "conv1d_k3_s2_d1_cl_valid_model.h5", "conv1d_k3_s3_d1_cf_same_model.h5", "conv1d_k3_s3_d1_cf_valid_model.h5", "conv1d_k3_s3_d1_cl_same_model.h5", "conv1d_k3_s3_d1_cl_valid_model.h5", "conv1d_k4_s1_d1_cf_same_model.h5", "conv1d_k4_s1_d1_cf_valid_model.h5", "conv1d_k4_s1_d1_cl_same_model.h5", "conv1d_k4_s1_d1_cl_valid_model.h5", "conv1d_k4_s1_d2_cf_same_model.h5", "conv1d_k4_s1_d2_cf_valid_model.h5", "conv1d_k4_s1_d2_cl_same_model.h5", "conv1d_k4_s1_d2_cl_valid_model.h5", "conv1d_k4_s2_d1_cf_same_model.h5", "conv1d_k4_s2_d1_cf_valid_model.h5", "conv1d_k4_s2_d1_cl_same_model.h5", "conv1d_k4_s2_d1_cl_valid_model.h5", "conv1d_k4_s3_d1_cf_same_model.h5", "conv1d_k4_s3_d1_cf_valid_model.h5", "conv1d_k4_s3_d1_cl_same_model.h5", "conv1d_k4_s3_d1_cl_valid_model.h5" }
			For Each name As String In names
				Console.WriteLine("Starting test: " & name)
				Dim modelPath As String = "modelimport/keras/examples/conv1d/" & name
				Dim inputsOutputPath As String = "modelimport/keras/examples/conv1d/" & (name.Substring(0, name.Length - "model.h5".Length) & "inputs_and_outputs.h5")
				importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, True, True, False, Nothing, Nothing)
			Next name
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Activation Layers") void testActivationLayers(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testActivationLayers(ByVal tempDir As Path)
			Dim names() As String = { "ELU_0_model.h5", "LeakyReLU_0_model.h5", "ReLU_0_model.h5", "ReLU_1_model.h5", "ReLU_2_model.h5", "ReLU_3_model.h5", "Softmax_0_model.h5", "ThresholdReLU_0_model.h5" }
			For Each name As String In names
				Console.WriteLine("Starting test: " & name)
				Dim modelPath As String = "modelimport/keras/examples/activations/" & name
				Dim inputsOutputPath As String = "modelimport/keras/examples/activations/" & (name.Substring(0, name.Length - "model.h5".Length) & "inputs_and_outputs.h5")
				importEndModelTest(tempDir,modelPath, inputsOutputPath, True, True, True, True, False, Nothing, Nothing)
			Next name
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.graph.ComputationGraph importFunctionalModelH5Test(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Function importFunctionalModelH5Test(ByVal tempDir As Path, ByVal modelPath As String) As ComputationGraph
			Return importFunctionalModelH5Test(tempDir,modelPath, Nothing, False)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.graph.ComputationGraph importFunctionalModelH5Test(java.nio.file.Path tempDir,String modelPath, int[] inputShape, boolean train) throws Exception
		Private Function importFunctionalModelH5Test(ByVal tempDir As Path, ByVal modelPath As String, ByVal inputShape() As Integer, ByVal train As Boolean) As ComputationGraph
			Dim modelFile As File
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(modelPath)
				modelFile = createTempFile(tempDir,TEMP_MODEL_FILENAME, H5_EXTENSION)
				Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
			End Using
			Dim builder As KerasModelBuilder = (New KerasModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(train)
			If inputShape IsNot Nothing Then
				builder.inputShape(inputShape)
			End If
			Dim model As KerasModel = builder.buildModel()
			Return model.ComputationGraph
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.multilayer.MultiLayerNetwork importSequentialModelH5Test(java.nio.file.Path tempDir,String modelPath) throws Exception
		Private Function importSequentialModelH5Test(ByVal tempDir As Path, ByVal modelPath As String) As MultiLayerNetwork
			Return importSequentialModelH5Test(tempDir,modelPath, Nothing)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.multilayer.MultiLayerNetwork importSequentialModelH5Test(java.nio.file.Path tempDir,String modelPath, int[] inputShape) throws Exception
		Private Function importSequentialModelH5Test(ByVal tempDir As Path, ByVal modelPath As String, ByVal inputShape() As Integer) As MultiLayerNetwork
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(modelPath)
				Dim modelFile As File = createTempFile(tempDir,TEMP_MODEL_FILENAME, H5_EXTENSION)
				Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
				Dim builder As KerasModelBuilder = (New KerasModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(False)
				If inputShape IsNot Nothing Then
					builder.inputShape(inputShape)
				End If
				Dim model As KerasSequentialModel = builder.buildSequential()
				Return model.MultiLayerNetwork
			End Using
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.multilayer.MultiLayerNetwork importEndModelTest(java.nio.file.Path tempDir,String modelPath, String inputsOutputsPath, boolean tfOrdering, boolean checkPredictions, boolean checkGradients, boolean enforceTrainingConfig) throws Exception
		Public Overridable Function importEndModelTest(ByVal tempDir As Path, ByVal modelPath As String, ByVal inputsOutputsPath As String, ByVal tfOrdering As Boolean, ByVal checkPredictions As Boolean, ByVal checkGradients As Boolean, ByVal enforceTrainingConfig As Boolean) As MultiLayerNetwork
			Return importEndModelTest(tempDir,modelPath, inputsOutputsPath, tfOrdering, checkPredictions, checkGradients, True, enforceTrainingConfig, Nothing, Nothing)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.multilayer.MultiLayerNetwork importEndModelTest(java.nio.file.Path tempDir,String modelPath, String inputsOutputsPath, boolean tfOrdering, boolean checkPredictions, boolean checkGradients, boolean enforceTrainingConfig, boolean checkAuc, org.nd4j.common.function.@Function<org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.linalg.api.ndarray.INDArray> inputPreProc, org.nd4j.common.function.BiFunction<String, org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.linalg.api.ndarray.INDArray> expectedPreProc) throws Exception
		Public Overridable Function importEndModelTest(ByVal tempDir As Path, ByVal modelPath As String, ByVal inputsOutputsPath As String, ByVal tfOrdering As Boolean, ByVal checkPredictions As Boolean, ByVal checkGradients As Boolean, ByVal enforceTrainingConfig As Boolean, ByVal checkAuc As Boolean, ByVal inputPreProc As [Function](Of INDArray, INDArray), ByVal expectedPreProc As BiFunction(Of String, INDArray, INDArray)) As MultiLayerNetwork
			Dim model As MultiLayerNetwork
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(modelPath)
				Dim modelFile As File = createTempFile(tempDir,TEMP_MODEL_FILENAME, H5_EXTENSION)
				Files.copy([is], modelFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
				Dim kerasModel As KerasSequentialModel = (New KerasModel()).modelBuilder().modelHdf5Filename(modelFile.getAbsolutePath()).enforceTrainingConfig(enforceTrainingConfig).buildSequential()
				model = kerasModel.MultiLayerNetwork
			End Using
			Dim outputsFile As File = createTempFile(tempDir,TEMP_OUTPUTS_FILENAME, H5_EXTENSION)
			Using [is] As Stream = org.nd4j.common.resources.Resources.asStream(inputsOutputsPath)
				Files.copy([is], outputsFile.toPath(), StandardCopyOption.REPLACE_EXISTING)
			End Using
			Using outputsArchive As New org.deeplearning4j.nn.modelimport.keras.Hdf5Archive(outputsFile.getAbsolutePath())
				If checkPredictions Then
					Dim input As INDArray = getInputs(outputsArchive, tfOrdering)(0)
					If inputPreProc IsNot Nothing Then
						input = inputPreProc.apply(input)
					End If
					Dim activationsKeras As IDictionary(Of String, INDArray) = getActivations(outputsArchive, tfOrdering)
					Dim i As Integer = 0
					Do While i < model.Layers.Length
						Dim layerName As String = model.getLayerNames()(i)
						If activationsKeras.ContainsKey(layerName) Then
							Dim activationsDl4j As INDArray = model.feedForwardToLayer(i, input, False)(i + 1)
							Dim shape() As Long = activationsDl4j.shape()
							Dim exp As INDArray = activationsKeras(layerName)
							Nd4j.Executioner.enableDebugMode(True)
							Nd4j.Executioner.enableVerboseMode(True)
							If expectedPreProc IsNot Nothing Then
								exp = expectedPreProc.apply(layerName, exp)
							End If
							compareINDArrays(layerName, exp, activationsDl4j, EPS)
						End If
						i += 1
					Loop
					Dim predictionsKeras As INDArray = getPredictions(outputsArchive, tfOrdering)(0)
					Dim predictionsDl4j As INDArray = model.output(input, False)
					If expectedPreProc IsNot Nothing Then
						predictionsKeras = expectedPreProc.apply("output", predictionsKeras)
					End If
					compareINDArrays("predictions", predictionsKeras, predictionsDl4j, EPS)
					Dim outputs As INDArray = getOutputs(outputsArchive, True)(0)
					If outputs.rank() = 1 Then
						outputs = outputs.reshape(ChrW(outputs.length()), 1)
					End If
					Dim nOut As val = CInt(outputs.size(-1))
					If checkAuc Then
						compareMulticlassAUC("predictions", outputs, predictionsKeras, predictionsDl4j, nOut, EPS)
					End If
				End If
				If checkGradients AndAlso Not SKIP_GRAD_CHECKS Then
					Dim r As New Random(12345)
					Dim input As INDArray = getInputs(outputsArchive, tfOrdering)(0)
					Dim predictionsDl4j As INDArray = model.output(input, False)
					' Infer one-hot labels... this probably won't work for all
					Dim testLabels As INDArray = Nd4j.create(predictionsDl4j.shape())
					If testLabels.rank() = 2 Then
						Dim i As Integer = 0
						Do While i < testLabels.size(0)
							testLabels.putScalar(i, r.Next(CInt(testLabels.size(1))), 1.0)
							i += 1
						Loop
					ElseIf testLabels.rank() = 3 Then
						Dim i As Integer = 0
						Do While i < testLabels.size(0)
							Dim j As Integer = 0
							Do While j < testLabels.size(1)
								testLabels.putScalar(i, j, r.Next(CInt(testLabels.size(1))), 1.0)
								j += 1
							Loop
							i += 1
						Loop
					Else
						Throw New Exception("Cannot gradient check 4d output array")
					End If
					KerasModelEndToEndTest.checkGradients(model, input, testLabels)
				End If
			End Using
			Return model
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.nd4j.linalg.api.ndarray.INDArray[] getInputs(org.deeplearning4j.nn.modelimport.keras.Hdf5Archive archive, boolean tensorFlowImageDimOrdering) throws Exception
		Private Shared Function getInputs(ByVal archive As Hdf5Archive, ByVal tensorFlowImageDimOrdering As Boolean) As INDArray()
			Dim inputNames As IList(Of String) = DirectCast(KerasModelUtils.parseJsonString(archive.readAttributeAsJson(GROUP_ATTR_INPUTS))(GROUP_ATTR_INPUTS), IList(Of String))
			Dim inputs(inputNames.Count - 1) As INDArray
			For i As Integer = 0 To inputNames.Count - 1
				inputs(i) = archive.readDataSet(inputNames(i), GROUP_ATTR_INPUTS)
			Next i
			Return inputs
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static Map<String, org.nd4j.linalg.api.ndarray.INDArray> getActivations(org.deeplearning4j.nn.modelimport.keras.Hdf5Archive archive, boolean tensorFlowImageDimOrdering) throws Exception
		Private Shared Function getActivations(ByVal archive As Hdf5Archive, ByVal tensorFlowImageDimOrdering As Boolean) As IDictionary(Of String, INDArray)
			Dim activations As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each layerName As String In archive.getDataSets(GROUP_ACTIVATIONS)
				Dim activation As INDArray = archive.readDataSet(layerName, GROUP_ACTIVATIONS)
				activations(layerName) = activation
			Next layerName
			Return activations
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.nd4j.linalg.api.ndarray.INDArray[] getOutputs(org.deeplearning4j.nn.modelimport.keras.Hdf5Archive archive, boolean tensorFlowImageDimOrdering) throws Exception
		Private Shared Function getOutputs(ByVal archive As Hdf5Archive, ByVal tensorFlowImageDimOrdering As Boolean) As INDArray()
			Dim outputNames As IList(Of String) = DirectCast(KerasModelUtils.parseJsonString(archive.readAttributeAsJson(GROUP_ATTR_OUTPUTS))(GROUP_ATTR_OUTPUTS), IList(Of String))
			Dim outputs(outputNames.Count - 1) As INDArray
			For i As Integer = 0 To outputNames.Count - 1
				outputs(i) = archive.readDataSet(outputNames(i), GROUP_ATTR_OUTPUTS)
			Next i
			Return outputs
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.nd4j.linalg.api.ndarray.INDArray[] getPredictions(org.deeplearning4j.nn.modelimport.keras.Hdf5Archive archive, boolean tensorFlowImageDimOrdering) throws Exception
		Private Shared Function getPredictions(ByVal archive As Hdf5Archive, ByVal tensorFlowImageDimOrdering As Boolean) As INDArray()
			Dim outputNames As IList(Of String) = DirectCast(KerasModelUtils.parseJsonString(archive.readAttributeAsJson(GROUP_ATTR_OUTPUTS))(GROUP_ATTR_OUTPUTS), IList(Of String))
			Dim predictions(outputNames.Count - 1) As INDArray
			For i As Integer = 0 To outputNames.Count - 1
				predictions(i) = archive.readDataSet(outputNames(i), GROUP_PREDICTIONS)
			Next i
			Return predictions
		End Function

		Private Shared Sub compareINDArrays(ByVal label As String, ByVal expected As INDArray, ByVal actual As INDArray, ByVal eps As Double)
			If Not expected.equalShapes(actual) Then
				Throw New System.InvalidOperationException("Shapes do not match for """ & label & """: got " & java.util.Arrays.toString(expected.shape()) & " vs " & java.util.Arrays.toString(actual.shape()))
			End If
			Dim diff As INDArray = expected.sub(actual.castTo(expected.dataType()))
			Dim min As Double = diff.minNumber().doubleValue()
			Dim max As Double = diff.maxNumber().doubleValue()
			log.info(label & ": " & expected.equalsWithEps(actual, eps) & ", " & min & ", " & max)
			Dim threshold As Double = 1e-7
			Dim aAbsMax As Double = Math.Max(Math.Abs(expected.minNumber().doubleValue()), Math.Abs(expected.maxNumber().doubleValue()))
			Dim bAbsMax As Double = Math.Max(Math.Abs(actual.minNumber().doubleValue()), Math.Abs(actual.maxNumber().doubleValue()))
			' skip too small absolute inputs
			If Math.Abs(aAbsMax) > threshold AndAlso Math.Abs(bAbsMax) > threshold Then
				Dim eq As Boolean = expected.equalsWithEps(actual.castTo(expected.dataType()), eps)
				If Not eq Then
					Console.WriteLine("Expected: " & java.util.Arrays.toString(expected.shape()) & ", actual: " & java.util.Arrays.toString(actual.shape()))
					Console.WriteLine("Expected:" & vbLf & expected)
					Console.WriteLine("Actual: " & vbLf & actual)
				End If
				assertTrue(eq,"Output differs: " & label)
			End If
		End Sub

		Private Shared Sub compareMulticlassAUC(ByVal label As String, ByVal target As INDArray, ByVal a As INDArray, ByVal b As INDArray, ByVal nbClasses As Integer, ByVal eps As Double)
			Dim evalA As New ROCMultiClass(100)
			evalA.eval(target, a)
			Dim avgAucA As Double = evalA.calculateAverageAUC()
			Dim evalB As New ROCMultiClass(100)
			evalB.eval(target, b)
			Dim avgAucB As Double = evalB.calculateAverageAUC()
			assertEquals(avgAucA, avgAucB, KerasModelEndToEndTest.EPS)
			Dim aucA(nbClasses - 1) As Double
			Dim aucB(nbClasses - 1) As Double
			If nbClasses > 1 Then
				For i As Integer = 0 To nbClasses - 1
					aucA(i) = evalA.calculateAUC(i)
					aucB(i) = evalB.calculateAUC(i)
				Next i
				assertArrayEquals(aucA, aucB, KerasModelEndToEndTest.EPS)
			End If
		End Sub

		Public Shared Sub checkGradients(ByVal net As MultiLayerNetwork, ByVal input As INDArray, ByVal labels As INDArray)
'JAVA TO VB CONVERTER NOTE: The variable eps was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim eps_Conflict As Double = 1e-6
			Dim max_rel_error As Double = 1e-3
			Dim min_abs_error As Double = 1e-8
			Dim netToTest As MultiLayerNetwork
			If TypeOf net.OutputLayer Is IOutputLayer Then
				netToTest = net
			Else
				Dim l As org.deeplearning4j.nn.conf.layers.Layer
				If labels.rank() = 2 Then
					l = (New LossLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.IDENTITY).build()
				Else
					' Rank 3
					l = (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.IDENTITY).nIn(labels.size(1)).nOut(labels.size(1)).build()
				End If
				netToTest = (New TransferLearning.Builder(net)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).updater(New NoOp()).dropOut(0.0).build()).addLayer(l).build()
			End If
			log.info("Num params: " & net.numParams())
			For Each l As Layer In netToTest.Layers
				' Remove any dropout manually - until this is fixed:
				' https://github.com/eclipse/deeplearning4j/issues/4368
				l.conf().getLayer().setIDropout(Nothing)
				' Also swap out activation functions... this is a bit of a hack, but should make the net gradient checkable...
				If TypeOf l.conf().getLayer() Is FeedForwardLayer Then
					Dim ffl As FeedForwardLayer = CType(l.conf().getLayer(), FeedForwardLayer)
					Dim activation As IActivation = ffl.getActivationFn()
					If TypeOf activation Is ActivationReLU OrElse TypeOf activation Is ActivationLReLU Then
						ffl.setActivationFn(New ActivationSoftPlus())
					ElseIf TypeOf activation Is ActivationHardTanH Then
						ffl.setActivationFn(New ActivationTanH())
					End If
				End If
			Next l
			Nd4j.DataType = DataType.DOUBLE
			Dim passed As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(netToTest).input(input).labels(labels).subset(True).maxPerParam(9))
			assertTrue(passed, "Gradient check failed")
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.io.File createTempFile(java.nio.file.Path testDir,String prefix, String suffix) throws java.io.IOException
		Private Function createTempFile(ByVal testDir As Path, ByVal prefix As String, ByVal suffix As String) As File
			Dim ret As New File(testDir.toFile(),prefix & "-" & System.nanoTime() & suffix)
			ret.createNewFile()
			ret.deleteOnExit()
			Return ret
		End Function
	End Class

End Namespace