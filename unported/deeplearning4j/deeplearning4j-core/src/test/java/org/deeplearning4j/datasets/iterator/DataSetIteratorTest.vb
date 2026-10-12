Imports System
Imports System.Collections.Generic
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports CifarLoader = org.datavec.image.loader.CifarLoader
Imports LFWLoader = org.datavec.image.loader.LFWLoader
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports DataSetType = org.deeplearning4j.datasets.fetchers.DataSetType
Imports org.deeplearning4j.datasets.iterator.impl
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports CollectScoresIterationListener = org.deeplearning4j.optimize.listeners.CollectScoresIterationListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions
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
Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Data Set Iterator Test") @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) class DataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class DataSetIteratorTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				' Should run quickly; increased to large timeout due to occasonal slow CI downloads
				Return 360000
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Batch Size Of One Iris") void testBatchSizeOfOneIris() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBatchSizeOfOneIris()
			' Test for (a) iterators returning correct number of examples, and
			' (b) Labels are a proper one-hot vector (i.e., sum is 1.0)
			' Iris:
			Dim iris As DataSetIterator = New IrisDataSetIterator(1, 5)
			Dim irisC As Integer = 0
			Do While iris.MoveNext()
				irisC += 1
				Dim ds As DataSet = iris.Current
				assertTrue(ds.Labels.sum(Integer.MaxValue).getDouble(0) = 1.0)
			Loop
			assertEquals(5, irisC)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Batch Size Of One Mnist") void testBatchSizeOfOneMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBatchSizeOfOneMnist()
			' MNIST:
			Dim mnist As DataSetIterator = New MnistDataSetIterator(1, 5)
			Dim mnistC As Integer = 0
			Do While mnist.MoveNext()
				mnistC += 1
				Dim ds As DataSet = mnist.Current
				assertTrue(ds.Labels.sum(Integer.MaxValue).getDouble(0) = 1.0)
			Loop
			assertEquals(5, mnistC)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Mnist") void testMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMnist()
			Dim cpr As New ClassPathResource("mnist_first_200.txt")
			Dim rr As New CSVRecordReader(0, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit(cpr.TempFileFromArchive))
			Dim dsi As New RecordReaderDataSetIterator(rr, 10, 0, 10)
			Dim iter As New MnistDataSetIterator(10, 200, False, True, False, 0)
			Do While dsi.MoveNext()
				Dim dsExp As DataSet = dsi.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim dsAct As DataSet = iter.next()
				Dim fExp As INDArray = dsExp.Features
				fExp.divi(255)
				Dim lExp As INDArray = dsExp.Labels
				Dim fAct As INDArray = dsAct.Features
				Dim lAct As INDArray = dsAct.Labels
				assertEquals(fExp, fAct.castTo(fExp.dataType()))
				assertEquals(lExp, lAct.castTo(lExp.dataType()))
			Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(iter.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Lfw Iterator") void testLfwIterator() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLfwIterator()
			Dim numExamples As Integer = 1
			Dim row As Integer = 28
			Dim col As Integer = 28
			Dim channels As Integer = 1
			Dim iter As New LFWDataSetIterator(numExamples, New Integer() { row, col, channels }, True)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(iter.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim data As DataSet = iter.next()
			assertEquals(numExamples, data.Labels.size(0))
			assertEquals(row, data.Features.size(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Tiny Image Net Iterator") void testTinyImageNetIterator() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testTinyImageNetIterator()
			Dim numClasses As Integer = 200
			Dim row As Integer = 64
			Dim col As Integer = 64
			Dim channels As Integer = 3
			Dim iter As New TinyImageNetDataSetIterator(1, DataSetType.TEST)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(iter.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim data As DataSet = iter.next()
			assertEquals(numClasses, data.Labels.size(1))
			assertArrayEquals(New Long() { 1, channels, row, col }, data.Features.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Tiny Image Net Iterator 2") void testTinyImageNetIterator2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testTinyImageNetIterator2()
			Dim numClasses As Integer = 200
			Dim row As Integer = 224
			Dim col As Integer = 224
			Dim channels As Integer = 3
			Dim iter As New TinyImageNetDataSetIterator(1, New Integer() { row, col }, DataSetType.TEST)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(iter.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim data As DataSet = iter.next()
			assertEquals(numClasses, data.Labels.size(1))
			assertArrayEquals(New Long() { 1, channels, row, col }, data.Features.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Lfw Model") void testLfwModel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLfwModel()
			Const numRows As Integer = 28
			Const numColumns As Integer = 28
			Dim numChannels As Integer = 3
			Dim outputNum As Integer = LFWLoader.NUM_LABELS
			Dim numSamples As Integer = LFWLoader.NUM_IMAGES
			Dim batchSize As Integer = 2
			Dim seed As Integer = 123
			Dim listenerFreq As Integer = 1
			Dim lfw As New LFWDataSetIterator(batchSize, numSamples, New Integer() { numRows, numColumns, numChannels }, outputNum, False, True, 1.0, New Random(seed))
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(numChannels).nOut(6).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).stride(1, 1).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(numRows, numColumns, numChannels))
			Dim model As New MultiLayerNetwork(builder.build())
			model.init()
			model.setListeners(New ScoreIterationListener(listenerFreq))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			model.fit(lfw.next())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim dataTest As DataSet = lfw.next()
			Dim output As INDArray = model.output(dataTest.Features)
			Dim eval As New Evaluation(outputNum)
			eval.eval(dataTest.Labels, output)
			' System.out.println(eval.stats());
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Cifar 10 Iterator") void testCifar10Iterator() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCifar10Iterator()
			Dim numExamples As Integer = 1
			Dim row As Integer = 32
			Dim col As Integer = 32
			Dim channels As Integer = 3
			Dim iter As New Cifar10DataSetIterator(numExamples)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(iter.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim data As DataSet = iter.next()
			assertEquals(numExamples, data.Labels.size(0))
			assertEquals(channels * row * col, data.Features.ravel().length())
		End Sub

		' Ignored for now - CIFAR iterator needs work - https://github.com/eclipse/deeplearning4j/issues/4673
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @DisplayName("Test Cifar Model") void testCifarModel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCifarModel()
			' Streaming
			runCifar(False)
			' Preprocess
			runCifar(True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void runCifar(boolean preProcessCifar) throws Exception
		Public Overridable Sub runCifar(ByVal preProcessCifar As Boolean)
			Const height As Integer = 32
			Const width As Integer = 32
			Dim channels As Integer = 3
			Dim outputNum As Integer = CifarLoader.NUM_LABELS
			Dim batchSize As Integer = 5
			Dim seed As Integer = 123
			Dim listenerFreq As Integer = 1
			Dim cifar As New Cifar10DataSetIterator(batchSize)
			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(seed).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(channels).nOut(6).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX, New Integer() { 2, 2 })).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(height, width, channels))
			Dim model As New MultiLayerNetwork(builder.build())
			model.init()
			' model.setListeners(Arrays.asList((TrainingListener) new ScoreIterationListener(listenerFreq)));
			Dim listener As New CollectScoresIterationListener(listenerFreq)
			model.setListeners(listener)
			model.fit(cifar)
			cifar = New Cifar10DataSetIterator(batchSize)
			Dim eval As New Evaluation(cifar.getLabels())
			Do While cifar.MoveNext()
				Dim testDS As DataSet = cifar.next(batchSize)
				Dim output As INDArray = model.output(testDS.Features)
				eval.eval(testDS.Labels, output)
			Loop
			' System.out.println(eval.stats(true));
			listener.exportScores(System.out)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Iterator Data Set Iterator Combining") void testIteratorDataSetIteratorCombining()
		Friend Overridable Sub testIteratorDataSetIteratorCombining()
			' Test combining of a bunch of small (size 1) data sets together
			Dim batchSize As Integer = 3
			Dim numBatches As Integer = 4
			Dim featureSize As Integer = 5
			Dim labelSize As Integer = 6
			Nd4j.Random.setSeed(12345)
			Dim orig As IList(Of DataSet) = New List(Of DataSet)()
			Dim i As Integer = 0
			Do While i < batchSize * numBatches
				Dim features As INDArray = Nd4j.rand(1, featureSize)
				Dim labels As INDArray = Nd4j.rand(1, labelSize)
				orig.Add(New DataSet(features, labels))
				i += 1
			Loop
			Dim iter As DataSetIterator = New IteratorDataSetIterator(orig.GetEnumerator(), batchSize)
			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				assertArrayEquals(New Long() { batchSize, featureSize }, ds.Features.shape())
				assertArrayEquals(New Long() { batchSize, labelSize }, ds.Labels.shape())
				Dim fList As IList(Of INDArray) = New List(Of INDArray)()
				Dim lList As IList(Of INDArray) = New List(Of INDArray)()
				For i As Integer = 0 To batchSize - 1
					Dim dsOrig As DataSet = orig(count * batchSize + i)
					fList.Add(dsOrig.Features)
					lList.Add(dsOrig.Labels)
				Next i
				Dim fExp As INDArray = Nd4j.vstack(fList)
				Dim lExp As INDArray = Nd4j.vstack(lList)
				assertEquals(fExp, ds.Features)
				assertEquals(lExp, ds.Labels)
				count += 1
			Loop
			assertEquals(count, numBatches)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Iterator Data Set Iterator Splitting") void testIteratorDataSetIteratorSplitting()
		Friend Overridable Sub testIteratorDataSetIteratorSplitting()
			' Test splitting large data sets into smaller ones
			Dim origBatchSize As Integer = 4
			Dim origNumDSs As Integer = 3
			Dim batchSize As Integer = 3
			Dim numBatches As Integer = 4
			Dim featureSize As Integer = 5
			Dim labelSize As Integer = 6
			Nd4j.Random.setSeed(12345)
			Dim orig As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To origNumDSs - 1
				Dim features As INDArray = Nd4j.rand(origBatchSize, featureSize)
				Dim labels As INDArray = Nd4j.rand(origBatchSize, labelSize)
				orig.Add(New DataSet(features, labels))
			Next i
			Dim expected As IList(Of DataSet) = New List(Of DataSet)()
			expected.Add(New DataSet(orig(0).getFeatures().getRows(0, 1, 2), orig(0).getLabels().getRows(0, 1, 2)))
			expected.Add(New DataSet(Nd4j.vstack(orig(0).getFeatures().getRows(3), orig(1).getFeatures().getRows(0, 1)), Nd4j.vstack(orig(0).getLabels().getRows(3), orig(1).getLabels().getRows(0, 1))))
			expected.Add(New DataSet(Nd4j.vstack(orig(1).getFeatures().getRows(2, 3), orig(2).getFeatures().getRows(0)), Nd4j.vstack(orig(1).getLabels().getRows(2, 3), orig(2).getLabels().getRows(0))))
			expected.Add(New DataSet(orig(2).getFeatures().getRows(1, 2, 3), orig(2).getLabels().getRows(1, 2, 3)))
			Dim iter As DataSetIterator = New IteratorDataSetIterator(orig.GetEnumerator(), batchSize)
			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				assertEquals(expected(count), ds)
				count += 1
			Loop
			assertEquals(count, numBatches)
		End Sub
	End Class

End Namespace