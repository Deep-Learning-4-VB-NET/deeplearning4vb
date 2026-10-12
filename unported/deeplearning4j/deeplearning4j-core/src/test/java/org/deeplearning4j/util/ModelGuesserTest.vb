Imports System.IO
Imports IOUtils = org.apache.commons.compress.utils.IOUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ModelGuesser = org.deeplearning4j.core.util.ModelGuesser
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports org.nd4j.linalg.dataset.api.preprocessor
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Resources = org.nd4j.common.resources.Resources
Imports org.junit.jupiter.api.Assertions
Imports org.junit.jupiter.api.Assumptions
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
Namespace org.deeplearning4j.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @DisplayName("Model Guesser Test") @NativeTag @Tag(TagNames.FILE_IO) class ModelGuesserTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ModelGuesserTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Model Guess File") void testModelGuessFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testModelGuessFile()
			Dim f As File = Resources.asFile("modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5")
			assertTrue(f.exists())
			Dim guess1 As Model = ModelGuesser.loadModelGuess(f.getAbsolutePath())
			assertNotNull(guess1)
			f = Resources.asFile("modelimport/keras/examples/mnist_cnn/mnist_cnn_tf_keras_1_model.h5")
			assertTrue(f.exists())
			Dim guess2 As Model = ModelGuesser.loadModelGuess(f.getAbsolutePath())
			assertNotNull(guess2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Model Guess Input Stream") void testModelGuessInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testModelGuessInputStream()
			Dim f As File = Resources.asFile("modelimport/keras/examples/mnist_mlp/mnist_mlp_tf_keras_1_model.h5")
			assertTrue(f.exists())
			Using inputStream As Stream = New FileStream(f, FileMode.Open, FileAccess.Read)
				Dim guess1 As Model = ModelGuesser.loadModelGuess(inputStream)
				assertNotNull(guess1)
			End Using
			f = Resources.asFile("modelimport/keras/examples/mnist_cnn/mnist_cnn_tf_keras_1_model.h5")
			assertTrue(f.exists())
			Using inputStream As Stream = New FileStream(f, FileMode.Open, FileAccess.Read)
				Dim guess1 As Model = ModelGuesser.loadModelGuess(inputStream)
				assertNotNull(guess1)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Load Normalizers File") void testLoadNormalizersFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLoadNormalizersFile()
			Dim net As MultiLayerNetwork = Network
			Dim tempFile As File = testDir.resolve("testLoadNormalizersFile.bin").toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			Dim normalizer As New NormalizerMinMaxScaler(0, 1)
			normalizer.fit(New DataSet(Nd4j.rand(New Integer() { 2, 2 }), Nd4j.rand(New Integer() { 2, 2 })))
			ModelSerializer.addNormalizerToModel(tempFile, normalizer)
			Dim model As Model = ModelGuesser.loadModelGuess(tempFile.getAbsolutePath())
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: org.nd4j.linalg.dataset.api.preprocessor.Normalizer<?> normalizer1 = org.deeplearning4j.core.util.ModelGuesser.loadNormalizer(tempFile.getAbsolutePath());
			Dim normalizer1 As Normalizer(Of Object) = ModelGuesser.loadNormalizer(tempFile.getAbsolutePath())
			assertEquals(model, net)
			assertEquals(normalizer, normalizer1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Normalizer In Place") void testNormalizerInPlace() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNormalizerInPlace()
			Dim net As MultiLayerNetwork = Network
			Dim tempFile As File = testDir.resolve("testNormalizerInPlace.bin").toFile()
			Dim normalizer As New NormalizerMinMaxScaler(0, 1)
			normalizer.fit(New DataSet(Nd4j.rand(New Integer() { 2, 2 }), Nd4j.rand(New Integer() { 2, 2 })))
			ModelSerializer.writeModel(net, tempFile, True, normalizer)
			Dim model As Model = ModelGuesser.loadModelGuess(tempFile.getAbsolutePath())
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: org.nd4j.linalg.dataset.api.preprocessor.Normalizer<?> normalizer1 = org.deeplearning4j.core.util.ModelGuesser.loadNormalizer(tempFile.getAbsolutePath());
			Dim normalizer1 As Normalizer(Of Object) = ModelGuesser.loadNormalizer(tempFile.getAbsolutePath())
			assertEquals(model, net)
			assertEquals(normalizer, normalizer1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Load Normalizers Input Stream") void testLoadNormalizersInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLoadNormalizersInputStream()
			Dim net As MultiLayerNetwork = Network
			Dim tempFile As File = testDir.resolve("testLoadNormalizersInputStream.bin").toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			Dim normalizer As New NormalizerMinMaxScaler(0, 1)
			normalizer.fit(New DataSet(Nd4j.rand(New Integer() { 2, 2 }), Nd4j.rand(New Integer() { 2, 2 })))
			ModelSerializer.addNormalizerToModel(tempFile, normalizer)
			Dim model As Model = ModelGuesser.loadModelGuess(tempFile.getAbsolutePath())
			Using inputStream As Stream = New FileStream(tempFile, FileMode.Open, FileAccess.Read)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: org.nd4j.linalg.dataset.api.preprocessor.Normalizer<?> normalizer1 = org.deeplearning4j.core.util.ModelGuesser.loadNormalizer(inputStream);
				Dim normalizer1 As Normalizer(Of Object) = ModelGuesser.loadNormalizer(inputStream)
				assertEquals(model, net)
				assertEquals(normalizer, normalizer1)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Model Guesser Dl 4 j Model File") void testModelGuesserDl4jModelFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testModelGuesserDl4jModelFile()
			Dim net As MultiLayerNetwork = Network
			Dim tempFile As File = testDir.resolve("testModelGuesserDl4jModelFile.bin").toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			Dim network As MultiLayerNetwork = CType(ModelGuesser.loadModelGuess(tempFile.getAbsolutePath()), MultiLayerNetwork)
			assertEquals(network.LayerWiseConfigurations.toJson(), net.LayerWiseConfigurations.toJson())
			assertEquals(net.params(), network.params())
			assertEquals(net.Updater.StateViewArray, network.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Model Guesser Dl 4 j Model Input Stream") void testModelGuesserDl4jModelInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testModelGuesserDl4jModelInputStream()
			Dim net As MultiLayerNetwork = Network
			Dim tempFile As File = testDir.resolve("testModelGuesserDl4jModelInputStream.bin").toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			Using inputStream As Stream = New FileStream(tempFile, FileMode.Open, FileAccess.Read)
				Dim network As MultiLayerNetwork = DirectCast(ModelGuesser.loadModelGuess(inputStream), MultiLayerNetwork)
				assertNotNull(network)
				assertEquals(network.LayerWiseConfigurations.toJson(), net.LayerWiseConfigurations.toJson())
				assertEquals(net.params(), network.params())
				assertEquals(net.Updater.StateViewArray, network.Updater.StateViewArray)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Model Guess Config File") void testModelGuessConfigFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testModelGuessConfigFile()
			Dim resource As New ClassPathResource("modelimport/keras/configs/cnn_tf_config.json", GetType(ModelGuesserTest).getClassLoader())
			Dim f As File = getTempFile(resource)
			Dim configFilename As String = f.getAbsolutePath()
			Dim conf As Object = ModelGuesser.loadConfigGuess(configFilename)
			assertTrue(TypeOf conf Is MultiLayerConfiguration)
			Dim sequenceResource As New ClassPathResource("/keras/simple/mlp_fapi_multiloss_config.json")
			Dim f2 As File = getTempFile(sequenceResource)
			Dim sequenceConf As Object = ModelGuesser.loadConfigGuess(f2.getAbsolutePath())
			assertTrue(TypeOf sequenceConf Is ComputationGraphConfiguration)
			Dim resourceDl4j As New ClassPathResource("model.json")
			Dim fDl4j As File = getTempFile(resourceDl4j)
			Dim configFilenameDl4j As String = fDl4j.getAbsolutePath()
			Dim confDl4j As Object = ModelGuesser.loadConfigGuess(configFilenameDl4j)
			assertTrue(TypeOf confDl4j Is ComputationGraphConfiguration)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Model Guess Config Input Stream") void testModelGuessConfigInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testModelGuessConfigInputStream()
			Dim resource As New ClassPathResource("modelimport/keras/configs/cnn_tf_config.json", GetType(ModelGuesserTest).getClassLoader())
			Dim f As File = getTempFile(resource)
			Using inputStream As Stream = New FileStream(f, FileMode.Open, FileAccess.Read)
				Dim conf As Object = ModelGuesser.loadConfigGuess(inputStream)
				assertTrue(TypeOf conf Is MultiLayerConfiguration)
			End Using
			Dim sequenceResource As New ClassPathResource("/keras/simple/mlp_fapi_multiloss_config.json")
			Dim f2 As File = getTempFile(sequenceResource)
			Using inputStream As Stream = New FileStream(f2, FileMode.Open, FileAccess.Read)
				Dim sequenceConf As Object = ModelGuesser.loadConfigGuess(inputStream)
				assertTrue(TypeOf sequenceConf Is ComputationGraphConfiguration)
			End Using
			Dim resourceDl4j As New ClassPathResource("model.json")
			Dim fDl4j As File = getTempFile(resourceDl4j)
			Using inputStream As Stream = New FileStream(fDl4j, FileMode.Open, FileAccess.Read)
				Dim confDl4j As Object = ModelGuesser.loadConfigGuess(inputStream)
				assertTrue(TypeOf confDl4j Is ComputationGraphConfiguration)
			End Using
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private File getTempFile(org.nd4j.common.io.ClassPathResource classPathResource) throws Exception
		Private Function getTempFile(ByVal classPathResource As ClassPathResource) As File
			Dim [is] As Stream = classPathResource.InputStream
			Dim f As File = testDir.toFile()
			Dim bos As New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write))
			IOUtils.copy([is], bos)
			bos.flush()
			bos.close()
			Return f
		End Function

		Private ReadOnly Property Network As MultiLayerNetwork
			Get
				Dim nIn As Integer = 5
				Dim nOut As Integer = 6
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).l2(0.01).updater(New Sgd(0.1)).activation(Activation.TANH).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(20).build()).layer(1, (New DenseLayer.Builder()).nIn(20).nOut(30).build()).layer(2, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(30).nOut(nOut).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Return net
			End Get
		End Property
	End Class

End Namespace