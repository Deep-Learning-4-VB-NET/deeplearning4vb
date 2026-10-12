Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CollectionSequenceRecordReader = org.datavec.api.records.reader.impl.collection.CollectionSequenceRecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports FloatWritable = org.datavec.api.writable.FloatWritable
Imports Writable = org.datavec.api.writable.Writable
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports SequenceRecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports IteratorMultiDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorMultiDataSetIterator
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports org.deeplearning4j.datasets.iterator.impl
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports EvaluativeListener = org.deeplearning4j.optimize.listeners.EvaluativeListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports SplitTestAndTrain = org.nd4j.linalg.dataset.SplitTestAndTrain
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Resources = org.nd4j.common.resources.Resources
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
Namespace org.deeplearning4j.eval

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Eval Test") @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.JACKSON_SERDE) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) class EvalTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class EvalTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Iris") void testIris()
		Friend Overridable Sub testIris()
			' Network config
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).seed(42).updater(New Sgd(1e-6)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(2).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(2).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).build()
			' Instantiate model
			Dim model As New MultiLayerNetwork(conf)
			model.init()
			model.addListeners(New ScoreIterationListener(1))
			' Train-test split
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			[next].shuffle()
			Dim trainTest As SplitTestAndTrain = [next].splitTestAndTrain(5, New Random(42))
			' Train
			Dim train As DataSet = trainTest.Train
			train.normalizeZeroMeanZeroUnitVariance()
			' Test
			Dim test As DataSet = trainTest.Test
			test.normalizeZeroMeanZeroUnitVariance()
			Dim testFeature As INDArray = test.Features
			Dim testLabel As INDArray = test.Labels
			' Fitting model
			model.fit(train)
			' Get predictions from test feature
			Dim testPredictedLabel As INDArray = model.output(testFeature)
			' Eval with class number
			' // Specify class num here
			Dim eval As New org.nd4j.evaluation.classification.Evaluation(3)
			eval.eval(testLabel, testPredictedLabel)
			Dim eval1F1 As Double = eval.f1()
			Dim eval1Acc As Double = eval.accuracy()
			' Eval without class number
			' // No class num
			Dim eval2 As New org.nd4j.evaluation.classification.Evaluation()
			eval2.eval(testLabel, testPredictedLabel)
			Dim eval2F1 As Double = eval2.f1()
			Dim eval2Acc As Double = eval2.accuracy()
			' Assert the two implementations give same f1 and accuracy (since one batch)
			assertTrue(eval1F1 = eval2F1 AndAlso eval1Acc = eval2Acc)
			Dim evalViaMethod As org.nd4j.evaluation.classification.Evaluation = model.evaluate(New ListDataSetIterator(Of )(Collections.singletonList(test)))
			checkEvaluationEquality(eval, evalViaMethod)
			' System.out.println(eval.getConfusionMatrix().toString());
			' System.out.println(eval.getConfusionMatrix().toCSV());
			' System.out.println(eval.getConfusionMatrix().toHTML());
			' System.out.println(eval.confusionToString());
			eval.getConfusionMatrix().ToString()
			eval.getConfusionMatrix().toCSV()
			eval.getConfusionMatrix().toHTML()
			eval.confusionToString()
		End Sub

		Private Shared Sub assertMapEquals(ByVal first As IDictionary(Of Integer, Integer), ByVal second As IDictionary(Of Integer, Integer))
			assertEquals(first.Keys, second.Keys)
			For Each i As Integer? In first.Keys
				assertEquals(first(i), second(i))
			Next i
		End Sub

		Private Shared Sub checkEvaluationEquality(ByVal evalExpected As org.nd4j.evaluation.classification.Evaluation, ByVal evalActual As org.nd4j.evaluation.classification.Evaluation)
			assertEquals(evalExpected.accuracy(), evalActual.accuracy(), 1e-3)
			assertEquals(evalExpected.f1(), evalActual.f1(), 1e-3)
			assertEquals(evalExpected.NumRowCounter, evalActual.NumRowCounter, 1e-3)
			assertMapEquals(evalExpected.falseNegatives(), evalActual.falseNegatives())
			assertMapEquals(evalExpected.falsePositives(), evalActual.falsePositives())
			assertMapEquals(evalExpected.trueNegatives(), evalActual.trueNegatives())
			assertMapEquals(evalExpected.truePositives(), evalActual.truePositives())
			assertEquals(evalExpected.precision(), evalActual.precision(), 1e-3)
			assertEquals(evalExpected.recall(), evalActual.recall(), 1e-3)
			assertEquals(evalExpected.falsePositiveRate(), evalActual.falsePositiveRate(), 1e-3)
			assertEquals(evalExpected.falseNegativeRate(), evalActual.falseNegativeRate(), 1e-3)
			assertEquals(evalExpected.falseAlarmRate(), evalActual.falseAlarmRate(), 1e-3)
			assertEquals(evalExpected.getConfusionMatrix(), evalActual.getConfusionMatrix())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Evaluation With Meta Data") void testEvaluationWithMetaData() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testEvaluationWithMetaData()
			Dim csv As RecordReader = New CSVRecordReader()
			csv.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim batchSize As Integer = 10
			Dim labelIdx As Integer = 4
			Dim numClasses As Integer = 3
			Dim rrdsi As New RecordReaderDataSetIterator(csv, batchSize, labelIdx, numClasses)
			Dim ns As New NormalizerStandardize()
			ns.fit(rrdsi)
			rrdsi.PreProcessor = ns
			rrdsi.reset()
			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).list().layer(0, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(4).nOut(3).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			For i As Integer = 0 To 3
				net.fit(rrdsi)
				rrdsi.reset()
			Next i
			Dim e As New org.nd4j.evaluation.classification.Evaluation()
			' *** New: Enable collection of metadata (stored in the DataSets) ***
			rrdsi.CollectMetaData = True
			Do While rrdsi.MoveNext()
				Dim ds As DataSet = rrdsi.Current
				' *** New - cross dependencies here make types difficult, usid Object internally in DataSet for this***
				Dim meta As IList(Of RecordMetaData) = ds.getExampleMetaData(GetType(RecordMetaData))
				Dim [out] As INDArray = net.output(ds.Features)
				' *** New - evaluate and also store metadata ***
				e.eval(ds.Labels, [out], meta)
			Loop
			' System.out.println(e.stats());
			e.stats()
			' System.out.println("\n\n*** Prediction Errors: ***");
			' *** New - get list of prediction errors from evaluation ***
			Dim errors As IList(Of org.nd4j.evaluation.meta.Prediction) = e.getPredictionErrors()
			Dim metaForErrors As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			For Each p As org.nd4j.evaluation.meta.Prediction In errors
				metaForErrors.Add(DirectCast(p.getRecordMetaData(), RecordMetaData))
			Next p
			' *** New - dynamically load a subset of the data, just for prediction errors ***
			Dim ds As DataSet = rrdsi.loadFromMetaData(metaForErrors)
			Dim output As INDArray = net.output(ds.Features)
			Dim count As Integer = 0
			For Each t As org.nd4j.evaluation.meta.Prediction In errors
				Dim s As String = t & vbTab & vbTab & "Raw Data: " & csv.loadFromMetaData(DirectCast(t.getRecordMetaData(), RecordMetaData)).getRecord() & vbTab & "Normalized: " & ds.Features.getRow(count) & vbTab & "Labels: " & ds.Labels.getRow(count) & vbTab & "Network predictions: " & output.getRow(count)
				' System.out.println(s);
				count += 1
			Next t
			Dim errorCount As Integer = errors.Count
			Dim expAcc As Double = 1.0 - errorCount / 150.0
			assertEquals(expAcc, e.accuracy(), 1e-5)
			Dim confusion As org.nd4j.evaluation.classification.ConfusionMatrix(Of Integer) = e.getConfusionMatrix()
			Dim actualCounts(2) As Integer
			Dim predictedCounts(2) As Integer
			For i As Integer = 0 To 2
				For j As Integer = 0 To 2
					' (actual,predicted)
					Dim entry As Integer = confusion.getCount(i, j)
					Dim list As IList(Of org.nd4j.evaluation.meta.Prediction) = e.getPredictions(i, j)
					assertEquals(entry, list.Count)
					actualCounts(i) += entry
					predictedCounts(j) += entry
				Next j
			Next i
			For i As Integer = 0 To 2
				Dim actualClassI As IList(Of org.nd4j.evaluation.meta.Prediction) = e.getPredictionsByActualClass(i)
				Dim predictedClassI As IList(Of org.nd4j.evaluation.meta.Prediction) = e.getPredictionByPredictedClass(i)
				assertEquals(actualCounts(i), actualClassI.Count)
				assertEquals(predictedCounts(i), predictedClassI.Count)
			Next i
			' Finally: test doEvaluation methods
			rrdsi.reset()
			Dim e2 As New org.nd4j.evaluation.classification.Evaluation()
			net.doEvaluation(rrdsi, e2)
			For i As Integer = 0 To 2
				Dim actualClassI As IList(Of org.nd4j.evaluation.meta.Prediction) = e2.getPredictionsByActualClass(i)
				Dim predictedClassI As IList(Of org.nd4j.evaluation.meta.Prediction) = e2.getPredictionByPredictedClass(i)
				assertEquals(actualCounts(i), actualClassI.Count)
				assertEquals(predictedCounts(i), predictedClassI.Count)
			Next i
			Dim cg As ComputationGraph = net.toComputationGraph()
			rrdsi.reset()
			e2 = New org.nd4j.evaluation.classification.Evaluation()
			cg.doEvaluation(rrdsi, e2)
			For i As Integer = 0 To 2
				Dim actualClassI As IList(Of org.nd4j.evaluation.meta.Prediction) = e2.getPredictionsByActualClass(i)
				Dim predictedClassI As IList(Of org.nd4j.evaluation.meta.Prediction) = e2.getPredictionByPredictedClass(i)
				assertEquals(actualCounts(i), actualClassI.Count)
				assertEquals(predictedCounts(i), predictedClassI.Count)
			Next i
		End Sub

		Private Shared Sub apply(ByVal e As org.nd4j.evaluation.classification.Evaluation, ByVal nTimes As Integer, ByVal predicted As INDArray, ByVal actual As INDArray)
			For i As Integer = 0 To nTimes - 1
				e.eval(actual, predicted)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Eval Splitting") void testEvalSplitting()
		Friend Overridable Sub testEvalSplitting()
			' Test for "tbptt-like" functionality
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				Console.WriteLine("Starting test for workspace mode: " & ws)
				Dim nIn As Integer = 4
				Dim layerSize As Integer = 5
				Dim nOut As Integer = 6
				Dim tbpttLength As Integer = 10
				Dim tsLength As Integer = 5 * tbpttLength + tbpttLength \ 2
				Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).list().layer((New LSTM.Builder()).nIn(nIn).nOut(layerSize).build()).layer((New RnnOutputLayer.Builder()).nIn(layerSize).nOut(nOut).activation(Activation.SOFTMAX).build()).build()
				Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).list().layer((New LSTM.Builder()).nIn(nIn).nOut(layerSize).build()).layer((New RnnOutputLayer.Builder()).nIn(layerSize).nOut(nOut).activation(Activation.SOFTMAX).build()).tBPTTLength(10).backpropType(BackpropType.TruncatedBPTT).build()
				Dim net1 As New MultiLayerNetwork(conf1)
				net1.init()
				Dim net2 As New MultiLayerNetwork(conf2)
				net2.init()
				net2.Params = net1.params()
				For Each useMask As Boolean In New Boolean() { False, True }
					Dim in1 As INDArray = Nd4j.rand(New Integer() { 3, nIn, tsLength })
					Dim out1 As INDArray = TestUtils.randomOneHotTimeSeries(3, nOut, tsLength)
					Dim in2 As INDArray = Nd4j.rand(New Integer() { 5, nIn, tsLength })
					Dim out2 As INDArray = TestUtils.randomOneHotTimeSeries(5, nOut, tsLength)
					Dim lMask1 As INDArray = Nothing
					Dim lMask2 As INDArray = Nothing
					If useMask Then
						lMask1 = Nd4j.create(3, tsLength)
						lMask2 = Nd4j.create(5, tsLength)
						Nd4j.Executioner.exec(New BernoulliDistribution(lMask1, 0.5))
						Nd4j.Executioner.exec(New BernoulliDistribution(lMask2, 0.5))
					End If
					Dim l As IList(Of DataSet) = New List(Of DataSet) From {
						New DataSet(in1, out1, Nothing, lMask1),
						New DataSet(in2, out2, Nothing, lMask2)
					}
					Dim iter As DataSetIterator = New ExistingDataSetIterator(l)
					' System.out.println("Net 1 eval");
					Dim e1() As org.nd4j.evaluation.IEvaluation = net1.doEvaluation(iter, New org.nd4j.evaluation.classification.Evaluation(), New org.nd4j.evaluation.classification.ROCMultiClass(), New org.nd4j.evaluation.regression.RegressionEvaluation())
					' System.out.println("Net 2 eval");
					Dim e2() As org.nd4j.evaluation.IEvaluation = net2.doEvaluation(iter, New org.nd4j.evaluation.classification.Evaluation(), New org.nd4j.evaluation.classification.ROCMultiClass(), New org.nd4j.evaluation.regression.RegressionEvaluation())
					assertEquals(e1(0), e2(0))
					assertEquals(e1(1), e2(1))
					assertEquals(e1(2), e2(2))
				Next useMask
			Next ws
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Eval Splitting Comp Graph") void testEvalSplittingCompGraph()
		Friend Overridable Sub testEvalSplittingCompGraph()
			' Test for "tbptt-like" functionality
			For Each ws As WorkspaceMode In System.Enum.GetValues(GetType(WorkspaceMode))
				Console.WriteLine("Starting test for workspace mode: " & ws)
				Dim nIn As Integer = 4
				Dim layerSize As Integer = 5
				Dim nOut As Integer = 6
				Dim tbpttLength As Integer = 10
				Dim tsLength As Integer = 5 * tbpttLength + tbpttLength \ 2
				Dim conf1 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).graphBuilder().addInputs("in").addLayer("0", (New LSTM.Builder()).nIn(nIn).nOut(layerSize).build(), "in").addLayer("1", (New RnnOutputLayer.Builder()).nIn(layerSize).nOut(nOut).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()
				Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(ws).inferenceWorkspaceMode(ws).graphBuilder().addInputs("in").addLayer("0", (New LSTM.Builder()).nIn(nIn).nOut(layerSize).build(), "in").addLayer("1", (New RnnOutputLayer.Builder()).nIn(layerSize).nOut(nOut).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").tBPTTLength(10).backpropType(BackpropType.TruncatedBPTT).build()
				Dim net1 As New ComputationGraph(conf1)
				net1.init()
				Dim net2 As New ComputationGraph(conf2)
				net2.init()
				net2.Params = net1.params()
				For Each useMask As Boolean In New Boolean() { False, True }
					Dim in1 As INDArray = Nd4j.rand(New Integer() { 3, nIn, tsLength })
					Dim out1 As INDArray = TestUtils.randomOneHotTimeSeries(3, nOut, tsLength)
					Dim in2 As INDArray = Nd4j.rand(New Integer() { 5, nIn, tsLength })
					Dim out2 As INDArray = TestUtils.randomOneHotTimeSeries(5, nOut, tsLength)
					Dim lMask1 As INDArray = Nothing
					Dim lMask2 As INDArray = Nothing
					If useMask Then
						lMask1 = Nd4j.create(3, tsLength)
						lMask2 = Nd4j.create(5, tsLength)
						Nd4j.Executioner.exec(New BernoulliDistribution(lMask1, 0.5))
						Nd4j.Executioner.exec(New BernoulliDistribution(lMask2, 0.5))
					End If
					Dim l As IList(Of DataSet) = New List(Of DataSet) From {
						New DataSet(in1, out1),
						New DataSet(in2, out2)
					}
					Dim iter As DataSetIterator = New ExistingDataSetIterator(l)
					' System.out.println("Eval net 1");
					Dim e1() As org.nd4j.evaluation.IEvaluation = net1.doEvaluation(iter, New org.nd4j.evaluation.classification.Evaluation(), New org.nd4j.evaluation.classification.ROCMultiClass(), New org.nd4j.evaluation.regression.RegressionEvaluation())
					' System.out.println("Eval net 2");
					Dim e2() As org.nd4j.evaluation.IEvaluation = net2.doEvaluation(iter, New org.nd4j.evaluation.classification.Evaluation(), New org.nd4j.evaluation.classification.ROCMultiClass(), New org.nd4j.evaluation.regression.RegressionEvaluation())
					assertEquals(e1(0), e2(0))
					assertEquals(e1(1), e2(1))
					assertEquals(e1(2), e2(2))
				Next useMask
			Next ws
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Eval Splitting 2") void testEvalSplitting2()
		Friend Overridable Sub testEvalSplitting2()
			Dim seqFeatures As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim [step] As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			For i As Integer = 0 To 29
				seqFeatures.Add([step])
			Next i
			Dim seqLabels As IList(Of IList(Of Writable)) = Collections.singletonList(Collections.singletonList(Of Writable)(New FloatWritable(0)))
			Dim fsr As SequenceRecordReader = New CollectionSequenceRecordReader(Collections.singletonList(seqFeatures))
			Dim lsr As SequenceRecordReader = New CollectionSequenceRecordReader(Collections.singletonList(seqLabels))
			Dim testData As DataSetIterator = New SequenceRecordReaderDataSetIterator(fsr, lsr, 1, -1, True, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_END)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New LSTM.Builder()).activation(Activation.TANH).nIn(3).nOut(3).build()).layer(1, (New RnnOutputLayer.Builder()).activation(Activation.SIGMOID).lossFunction(LossFunctions.LossFunction.XENT).nIn(3).nOut(1).build()).backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(10).tBPTTBackwardLength(10).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.evaluate(testData)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Evaluative Listener Simple") void testEvaluativeListenerSimple()
		Friend Overridable Sub testEvaluativeListenerSimple()
			' Sanity check: https://github.com/eclipse/deeplearning4j/issues/5351
			' Network config
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).seed(42).updater(New Sgd(1e-6)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(2).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(2).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).build()
			' Instantiate model
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			' Train-test split
			Dim iter As DataSetIterator = New IrisDataSetIterator(30, 150)
			Dim iterTest As DataSetIterator = New IrisDataSetIterator(30, 150)
			net.setListeners(New EvaluativeListener(iterTest, 3))
			For i As Integer = 0 To 2
				net.fit(iter)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Output Eval Simple") void testMultiOutputEvalSimple()
		Friend Overridable Sub testMultiOutputEvalSimple()
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("out1", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).build(), "in").addLayer("out2", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).build(), "in").setOutputs("out1", "out2").build()
			Dim cg As New ComputationGraph(conf)
			cg.init()
			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)()
			Dim iter As DataSetIterator = New IrisDataSetIterator(30, 150)
			Do While iter.MoveNext()
				Dim ds As DataSet = iter.Current
				list.Add(New org.nd4j.linalg.dataset.MultiDataSet(New INDArray() { ds.Features }, New INDArray() { ds.Labels, ds.Labels }))
			Loop
			Dim e As New org.nd4j.evaluation.classification.Evaluation()
			Dim e2 As New org.nd4j.evaluation.regression.RegressionEvaluation()
			Dim evals As IDictionary(Of Integer, org.nd4j.evaluation.IEvaluation()) = New Dictionary(Of Integer, org.nd4j.evaluation.IEvaluation())()
			evals(0) = New org.nd4j.evaluation.IEvaluation() { e }
			evals(1) = New org.nd4j.evaluation.IEvaluation() { e2 }
			cg.evaluate(New IteratorMultiDataSetIterator(list.GetEnumerator(), 30), evals)
			assertEquals(150, e.NumRowCounter)
			assertEquals(150, e2.getExampleCountPerColumn().getInt(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Output Eval CG") void testMultiOutputEvalCG()
		Friend Overridable Sub testMultiOutputEvalCG()
			' Simple sanity check on evaluation
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New EmbeddingSequenceLayer.Builder()).nIn(10).nOut(10).build(), "in").layer("1", (New LSTM.Builder()).nIn(10).nOut(10).build(), "0").layer("2", (New LSTM.Builder()).nIn(10).nOut(10).build(), "0").layer("out1", (New RnnOutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "1").layer("out2", (New RnnOutputLayer.Builder()).nIn(10).nOut(20).activation(Activation.SOFTMAX).build(), "2").setOutputs("out1", "out2").build()
			Dim cg As New ComputationGraph(conf)
			cg.init()
			Dim mds As New org.nd4j.linalg.dataset.MultiDataSet(New INDArray() { Nd4j.create(10, 1, 10) }, New INDArray() { Nd4j.create(10, 10, 10), Nd4j.create(10, 20, 10) })
			Dim m As IDictionary(Of Integer, org.nd4j.evaluation.IEvaluation()) = New Dictionary(Of Integer, org.nd4j.evaluation.IEvaluation())()
			m(0) = New org.nd4j.evaluation.IEvaluation() { New org.nd4j.evaluation.classification.Evaluation() }
			m(1) = New org.nd4j.evaluation.IEvaluation() { New org.nd4j.evaluation.classification.Evaluation() }
			cg.evaluate(New SingletonMultiDataSetIterator(mds), m)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Evaluation") void testInvalidEvaluation()
		Friend Overridable Sub testInvalidEvaluation()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).nIn(4).nOut(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(3).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.RELU).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Try
				net.evaluate(iter)
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("Classifier") AndAlso e.Message.contains("Evaluation"))
			End Try
			Try
				net.evaluateROC(iter, 0)
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("Classifier") AndAlso e.Message.contains("ROC"))
			End Try
			Try
				net.evaluateROCMultiClass(iter, 0)
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("Classifier") AndAlso e.Message.contains("ROCMultiClass"))
			End Try
			Dim cg As ComputationGraph = net.toComputationGraph()
			Try
				cg.evaluate(iter)
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("Classifier") AndAlso e.Message.contains("Evaluation"))
			End Try
			Try
				cg.evaluateROC(iter, 0)
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("Classifier") AndAlso e.Message.contains("ROC"))
			End Try
			Try
				cg.evaluateROCMultiClass(iter, 0)
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.contains("Classifier") AndAlso e.Message.contains("ROCMultiClass"))
			End Try
			' Disable validation, and check same thing:
			net.LayerWiseConfigurations.setValidateOutputLayerConfig(False)
			net.evaluate(iter)
			net.evaluateROCMultiClass(iter, 0)
			cg.Configuration.setValidateOutputLayerConfig(False)
			cg.evaluate(iter)
			cg.evaluateROCMultiClass(iter, 0)
		End Sub
	End Class

End Namespace