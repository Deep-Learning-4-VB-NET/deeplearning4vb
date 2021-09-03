Imports System.Collections.Generic
Imports ModelType = org.deeplearning4j.integration.ModelType
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports EvaluationCalibration = org.nd4j.evaluation.classification.EvaluationCalibration
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports CompositeMultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.preprocessor.CompositeMultiDataSetPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Files = org.nd4j.shade.guava.io.Files
Imports TestCase = org.deeplearning4j.integration.TestCase
Imports CharacterIterator = org.deeplearning4j.integration.testcases.dl4j.misc.CharacterIterator
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports SequenceRecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports MultiDataNormalization = org.nd4j.linalg.dataset.api.preprocessor.MultiDataNormalization
Imports MultiNormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerMinMaxScaler
Imports MultiNormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerStandardize
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.integration.testcases.dl4j


	Public Class RNNTestCases

		''' <summary>
		''' RNN + global pooling + CSV + normalizer
		''' </summary>
		Public Shared ReadOnly Property RnnCsvSequenceClassificationTestCase1 As TestCase
			Get
				Return New RnnCsvSequenceClassificationTestCase1()
			End Get
		End Property

		Public Shared ReadOnly Property RnnCsvSequenceClassificationTestCase2 As TestCase
			Get
				Return New RnnCsvSequenceClassificationTestCase2()
			End Get
		End Property

		Public Shared ReadOnly Property RnnCharacterTestCase As TestCase
			Get
				Return New TestCaseAnonymousInnerClass()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass
			Inherits TestCase

	'		{
	'			testName = "RnnCharacterTestCase";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = False; 'Not much point on this one - it already fits very well...
	'			'Gradients depend on a lot of chained steps, numerical differences can accumulate
	'			maxRelativeErrorGradients = 5e-4;
	'			minAbsErrorGradients = 2e-4;
	'			maxRelativeErrorParamsPostTraining = 1e-4;
	'			minAbsErrorParamsPostTraining = 2e-3;
	'		}

			Private miniBatchSize As Integer = 32
			Private exampleLength As Integer = 200


			Public Overrides Function modelType() As ModelType
				Return ModelType.MLN
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Nd4j.Random.setSeed(12345)
    
					Dim iter As CharacterIterator = CharacterIterator.getShakespeareIterator(miniBatchSize,exampleLength)
					Dim nOut As Integer = iter.totalOutcomes()
    
					Dim lstmLayerSize As Integer = 200 'Number of units in each GravesLSTM layer
					Dim tbpttLength As Integer = 50 'Length for truncated backpropagation through time. i.e., do parameter updates ever 50 characters
    
					Return (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(12345).l2(0.001).weightInit(WeightInit.XAVIER).updater(New Adam(1e-3)).list().layer(0, (New LSTM.Builder()).nIn(iter.inputColumns()).nOut(lstmLayerSize).activation(Activation.TANH).build()).layer(1, (New LSTM.Builder()).nIn(lstmLayerSize).nOut(lstmLayerSize).activation(Activation.TANH).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(lstmLayerSize).nOut(nOut).build()).backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(tbpttLength).tBPTTBackwardLength(tbpttLength).build()
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
			Public Overrides ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim mds As MultiDataSet = outerInstance.TrainingData.next()
					Return Collections.singletonList(New Pair(Of )(mds.Features, mds.FeaturesMaskArrays))
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getGradientsTestData() throws Exception
			Public Overrides ReadOnly Property GradientsTestData As MultiDataSet
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Return outerInstance.TrainingData.next()
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim iter As DataSetIterator = CharacterIterator.getShakespeareIterator(miniBatchSize,exampleLength)
					iter = New EarlyTerminationDataSetIterator(iter, 2) '2 minibatches, 200/50 = 4 updates per minibatch
					Return New MultiDataSetIteratorAdapter(iter)
				End Get
			End Property

			Public Overrides ReadOnly Property NewEvaluations As IEvaluation()
				Get
					Return New IEvaluation(){
						New Evaluation(),
						New ROCMultiClass()
					}
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getEvaluationTestData() throws Exception
			Public Overrides ReadOnly Property EvaluationTestData As MultiDataSetIterator
				Get
					Return outerInstance.TrainingData
				End Get
			End Property
		End Class

		Protected Friend Class RnnCsvSequenceClassificationTestCase1
			Inherits TestCase

			Protected Friend Sub New()
				testName = "RnnCsvSequenceClassification1"
				testType = TestType.RANDOM_INIT
				testPredictions = True
				testTrainingCurves = True
				testGradients = True
				testParamsPostTraining = True
				testEvaluation = True
				testOverfitting = False 'Not much point on this one - it already fits very well...
			End Sub


'JAVA TO VB CONVERTER NOTE: The field normalizer was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend normalizer_Conflict As MultiDataNormalization

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.nd4j.linalg.dataset.api.preprocessor.MultiDataNormalization getNormalizer() throws Exception
			Protected Friend Overridable ReadOnly Property Normalizer As MultiDataNormalization
				Get
					If normalizer_Conflict IsNot Nothing Then
						Return normalizer_Conflict
					End If
    
					normalizer_Conflict = New MultiNormalizerStandardize()
					normalizer_Conflict.fit(TrainingDataUnnormalized)
    
					Return normalizer_Conflict
				End Get
			End Property

			Public Overrides Function modelType() As ModelType
				Return ModelType.MLN
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Return (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(12345).updater(New Adam(5e-2)).l1(1e-3).l2(1e-3).list().layer(0, (New LSTM.Builder()).activation(Activation.TANH).nOut(10).build()).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.AVG).build()).layer((New OutputLayer.Builder()).nOut(6).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).build()).setInputType(InputType.recurrent(1)).build()
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
			Public Overrides ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim mds As MultiDataSet = TrainingData.next()
					Return Collections.singletonList(New Pair(Of )(mds.Features, mds.FeaturesMaskArrays))
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getGradientsTestData() throws Exception
			Public Overrides ReadOnly Property GradientsTestData As MultiDataSet
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Return TrainingData.next()
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim iter As MultiDataSetIterator = TrainingDataUnnormalized
    
					Dim pp As MultiDataSetPreProcessor = Sub(multiDataSet)
					Dim l As INDArray = multiDataSet.getLabels(0)
					l = l.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(l.size(2)-1))
					multiDataSet.setLabels(0, l)
					multiDataSet.setLabelsMaskArray(0, Nothing)
					End Sub
    
    
					iter.PreProcessor = New CompositeMultiDataSetPreProcessor(Normalizer,pp)
    
					Return iter
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingDataUnnormalized() throws Exception
			Protected Friend Overridable ReadOnly Property TrainingDataUnnormalized As MultiDataSetIterator
				Get
					Dim miniBatchSize As Integer = 10
					Dim numLabelClasses As Integer = 6
    
					Dim featuresDirTrain As File = Files.createTempDir()
					Dim labelsDirTrain As File = Files.createTempDir()
					Call (New ClassPathResource("dl4j-integration-tests/data/uci_seq/train/features/")).copyDirectory(featuresDirTrain)
					Call (New ClassPathResource("dl4j-integration-tests/data/uci_seq/train/labels/")).copyDirectory(labelsDirTrain)
    
					Dim trainFeatures As SequenceRecordReader = New CSVSequenceRecordReader()
					trainFeatures.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresDirTrain.getAbsolutePath() & "/%d.csv", 0, 449))
					Dim trainLabels As SequenceRecordReader = New CSVSequenceRecordReader()
					trainLabels.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsDirTrain.getAbsolutePath() & "/%d.csv", 0, 449))
    
					Dim trainData As DataSetIterator = New SequenceRecordReaderDataSetIterator(trainFeatures, trainLabels, miniBatchSize, numLabelClasses, False, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_END)
    
					Dim iter As MultiDataSetIterator = New MultiDataSetIteratorAdapter(trainData)
					Return iter
				End Get
			End Property

			Public Overrides ReadOnly Property NewEvaluations As IEvaluation()
				Get
					Return New IEvaluation(){
						New Evaluation(),
						New ROCMultiClass(),
						New EvaluationCalibration()
					}
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getEvaluationTestData() throws Exception
			Public Overrides ReadOnly Property EvaluationTestData As MultiDataSetIterator
				Get
					Dim miniBatchSize As Integer = 10
					Dim numLabelClasses As Integer = 6
    
		'            File featuresDirTest = new ClassPathResource("/RnnCsvSequenceClassification/uci_seq/test/features/").getFile();
		'            File labelsDirTest = new ClassPathResource("/RnnCsvSequenceClassification/uci_seq/test/labels/").getFile();
					Dim featuresDirTest As File = Files.createTempDir()
					Dim labelsDirTest As File = Files.createTempDir()
					Call (New ClassPathResource("dl4j-integration-tests/data/uci_seq/test/features/")).copyDirectory(featuresDirTest)
					Call (New ClassPathResource("dl4j-integration-tests/data/uci_seq/test/labels/")).copyDirectory(labelsDirTest)
    
					Dim trainFeatures As SequenceRecordReader = New CSVSequenceRecordReader()
					trainFeatures.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresDirTest.getAbsolutePath() & "/%d.csv", 0, 149))
					Dim trainLabels As SequenceRecordReader = New CSVSequenceRecordReader()
					trainLabels.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsDirTest.getAbsolutePath() & "/%d.csv", 0, 149))
    
					Dim testData As DataSetIterator = New SequenceRecordReaderDataSetIterator(trainFeatures, trainLabels, miniBatchSize, numLabelClasses, False, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_END)
    
					Dim iter As MultiDataSetIterator = New MultiDataSetIteratorAdapter(testData)
    
					Dim pp As MultiDataSetPreProcessor = Sub(multiDataSet)
					Dim l As INDArray = multiDataSet.getLabels(0)
					l = l.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(l.size(2)-1))
					multiDataSet.setLabels(0, l)
					multiDataSet.setLabelsMaskArray(0, Nothing)
					End Sub
    
    
					iter.PreProcessor = New CompositeMultiDataSetPreProcessor(Normalizer,pp)
    
					Return iter
				End Get
			End Property
		End Class

		''' <summary>
		''' Similar to test case 1 - but using GravesLSTM + bidirectional wrapper + min/max scaler normalizer
		''' </summary>
		Protected Friend Class RnnCsvSequenceClassificationTestCase2
			Inherits RnnCsvSequenceClassificationTestCase1

			Protected Friend Sub New()
				MyBase.New()
				testName = "RnnCsvSequenceClassification2"
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Return (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(12345).updater(New Adam(5e-2)).l1(1e-3).l2(1e-3).list().layer(0, New Bidirectional((New LSTM.Builder()).activation(Activation.TANH).nOut(10).build())).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.AVG).build()).layer((New OutputLayer.Builder()).nOut(6).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).build()).setInputType(InputType.recurrent(1)).build()
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.nd4j.linalg.dataset.api.preprocessor.MultiDataNormalization getNormalizer() throws Exception
			Protected Friend Overrides ReadOnly Property Normalizer As MultiDataNormalization
				Get
					If normalizer_Conflict IsNot Nothing Then
						Return normalizer_Conflict
					End If
    
					normalizer_Conflict = New MultiNormalizerMinMaxScaler()
					normalizer_Conflict.fit(TrainingDataUnnormalized)
    
					Return normalizer_Conflict
				End Get
			End Property
		End Class


	End Class

End Namespace