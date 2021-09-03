Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.parallelism

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class ParallelWrapperTest extends org.deeplearning4j.BaseDL4JTest
	Public Class ParallelWrapperTest
		Inherits BaseDL4JTest

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ParallelWrapperTest))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParallelWrapperRun() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelWrapperRun()

			Dim nChannels As Integer = 1
			Dim outputNum As Integer = 10

			' for GPU you usually want to have higher batchSize
			Dim batchSize As Integer = 128
			Dim nEpochs As Integer = 5
			Dim seed As Integer = 123

			log.info("Load data....")
			Dim mnistTrain As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(batchSize, True, 12345), 15)
			Dim mnistTest As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(batchSize, False, 12345), 4)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(mnistTrain.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim t0 As val = mnistTrain.next()

			log.info("F: {}; L: {};", t0.getFeatures().shape(), t0.getLabels().shape())

			log.info("Build model....")
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).l2(0.0005).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(nChannels).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build()).layer(3, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(4, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(500).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, nChannels))

			Dim conf As MultiLayerConfiguration = builder.build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()

			' ParallelWrapper will take care of load balancing between GPUs.
			Dim wrapper As ParallelWrapper = (New ParallelWrapper.Builder(model)).prefetchBuffer(24).workers(2).averagingFrequency(3).reportScoreAfterAveraging(True).build()

			log.info("Train model....")
			model.setListeners(New ScoreIterationListener(100))
			Dim timeX As Long = DateTimeHelper.CurrentUnixTimeMillis()

			' optionally you might want to use MultipleEpochsIterator instead of manually iterating/resetting over your iterator
			'MultipleEpochsIterator mnistMultiEpochIterator = new MultipleEpochsIterator(nEpochs, mnistTrain);

			For i As Integer = 0 To nEpochs - 1
				Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()

				' Please note: we're feeding ParallelWrapper with iterator, not model directly
				'            wrapper.fit(mnistMultiEpochIterator);
				wrapper.fit(mnistTrain)
				Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
				log.info("*** Completed epoch {}, time: {} ***", i, (time2 - time1))
			Next i
			Dim timeY As Long = DateTimeHelper.CurrentUnixTimeMillis()
			log.info("*** Training complete, time: {} ***", (timeY - timeX))

			Dim eval As Evaluation = model.evaluate(mnistTest)
			log.info(eval.stats())
			mnistTest.reset()

			Dim acc As Double = eval.accuracy()
			assertTrue(acc > 0.5, acc.ToString())

			wrapper.shutdown()
		End Sub
	End Class

End Namespace