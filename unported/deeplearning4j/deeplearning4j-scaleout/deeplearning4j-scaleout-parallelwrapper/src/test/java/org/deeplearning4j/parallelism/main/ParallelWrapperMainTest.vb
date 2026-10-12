Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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

Namespace org.deeplearning4j.parallelism.main


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled("Permissions issues on CI") @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class ParallelWrapperMainTest extends org.deeplearning4j.BaseDL4JTest
	Public Class ParallelWrapperMainTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void runParallelWrapperMain(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub runParallelWrapperMain(ByVal testDir As Path)

			Dim nChannels As Integer = 1
			Dim outputNum As Integer = 10

			' for GPU you usually want to have higher batchSize
			Dim batchSize As Integer = 128
			Dim seed As Integer = 123
			Dim uiPort As Integer = 9500
			System.setProperty("org.deeplearning4j.ui.port", uiPort.ToString())
			log.info("Load data....")
			Dim mnistTrain As DataSetIterator = New MnistDataSetIterator(batchSize, True, 12345)
			Dim mnistTest As DataSetIterator = New MnistDataSetIterator(batchSize, False, 12345)

			log.info("Build model....")
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).l2(0.0005).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(nChannels).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build()).layer(3, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(4, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(500).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, nChannels))

			Dim conf As MultiLayerConfiguration = builder.build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			Dim tempModel As File = Files.createTempFile(testDir,"tmpmodel","zip").toFile()
			tempModel.deleteOnExit()
			ModelSerializer.writeModel(model, tempModel, False)
			Dim tmp As File = Files.createTempFile(testDir,"tmpmodel","bin").toFile()
			tmp.deleteOnExit()
'JAVA TO VB CONVERTER NOTE: The variable parallelWrapperMain was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim parallelWrapperMain_Conflict As New ParallelWrapperMain()
			Try
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				parallelWrapperMain_Conflict.runMain(New String(){"--modelPath", tempModel.getAbsolutePath(), "--dataSetIteratorFactoryClazz", GetType(MnistDataSetIteratorProviderFactory).FullName, "--modelOutputPath", tmp.getAbsolutePath(), "--uiUrl", "localhost:" & uiPort})
			Finally
				parallelWrapperMain_Conflict.stop()
			End Try
		End Sub
	End Class

End Namespace