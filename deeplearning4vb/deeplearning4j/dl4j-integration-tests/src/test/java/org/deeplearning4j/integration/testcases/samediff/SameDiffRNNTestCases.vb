Imports System.Collections.Generic
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports SequenceRecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator
Imports ModelType = org.deeplearning4j.integration.ModelType
Imports TestCase = org.deeplearning4j.integration.TestCase
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports EvaluationCalibration = org.nd4j.evaluation.classification.EvaluationCalibration
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LSTMActivations = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMActivations
Imports LSTMDataFormat = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMDataFormat
Imports LSTMDirectionMode = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMDirectionMode
Imports LSTMLayerConfig = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig
Imports LSTMLayerOutputs = org.nd4j.linalg.api.ops.impl.layers.recurrent.outputs.LSTMLayerOutputs
Imports LSTMLayerWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports CompositeMultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.preprocessor.CompositeMultiDataSetPreProcessor
Imports MultiDataNormalization = org.nd4j.linalg.dataset.api.preprocessor.MultiDataNormalization
Imports MultiNormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Resources = org.nd4j.common.resources.Resources
Imports Files = org.nd4j.shade.guava.io.Files

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
Namespace org.deeplearning4j.integration.testcases.samediff


	Public Class SameDiffRNNTestCases

		Public Shared ReadOnly Property RnnCsvSequenceClassificationTestCase1 As TestCase
			Get
				Return New SameDiffRNNTestCases.RnnCsvSequenceClassificationTestCase1()
			End Get
		End Property

		Protected Friend Class RnnCsvSequenceClassificationTestCase1
			Inherits TestCase

			Protected Friend Sub New()
				testName = "RnnCsvSequenceClassification1"
				testType = TestType.RANDOM_INIT
				testPredictions = True
				testTrainingCurves = False
				testGradients = False
				testParamsPostTraining = False
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
				Return ModelType.SAMEDIFF
			End Function


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Nd4j.Random.setSeed(12345)
    
    
					Dim miniBatchSize As Integer = 10
					Dim numLabelClasses As Integer = 6
					Dim nIn As Integer = 60
					Dim numUnits As Integer = 7
					Dim timeSteps As Integer = 3
    
    
					Dim sd As SameDiff = SameDiff.create()
    
					Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, miniBatchSize, timeSteps, nIn)
					Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, miniBatchSize, numLabelClasses)
    
    
					Dim cLast As SDVariable = sd.var("cLast", Nd4j.zeros(DataType.FLOAT, miniBatchSize, numUnits))
					Dim yLast As SDVariable = sd.var("yLast", Nd4j.zeros(DataType.FLOAT, miniBatchSize, numUnits))
    
					Dim c As LSTMLayerConfig = LSTMLayerConfig.builder().lstmdataformat(LSTMDataFormat.NTS).directionMode(LSTMDirectionMode.FWD).gateAct(LSTMActivations.SIGMOID).cellAct(LSTMActivations.TANH).outAct(LSTMActivations.TANH).retFullSequence(True).retLastC(True).retLastH(True).build()
    
					Dim outputs As New LSTMLayerOutputs(sd.rnn_Conflict.lstmLayer([in], cLast, yLast, Nothing, LSTMLayerWeights.builder().weights(sd.var("weights", Nd4j.rand(DataType.FLOAT, nIn, 4 * numUnits))).rWeights(sd.var("rWeights", Nd4j.rand(DataType.FLOAT, numUnits, 4 * numUnits))).peepholeWeights(sd.var("inputPeepholeWeights", Nd4j.rand(DataType.FLOAT, 3 * numUnits))).bias(sd.var("bias", Nd4j.rand(DataType.FLOAT, 4 * numUnits))).build(), c), c)
    
    
		'           Behaviour with default settings: 3d (time series) input with shape
		'          [miniBatchSize, vectorSize, timeSeriesLength] -> 2d output [miniBatchSize, vectorSize]
					Dim layer0 As SDVariable = outputs.Output
    
					Dim layer1 As SDVariable = layer0.mean(1)
    
					Dim w1 As SDVariable = sd.var("w1", Nd4j.rand(DataType.FLOAT, numUnits, numLabelClasses))
					Dim b1 As SDVariable = sd.var("b1", Nd4j.rand(DataType.FLOAT, numLabelClasses))
    
    
					Dim [out] As SDVariable = sd.nn_Conflict.softmax("out", layer1.mmul(w1).add(b1))
					Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, [out])
    
					'Also set the training configuration:
					sd.TrainingConfig = TrainingConfig.builder().updater(New Adam(5e-2)).l1(1e-3).l2(1e-3).dataSetFeatureMapping("in").dataSetLabelMapping("label").build()
    
					Return sd
    
				End Get
			End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray>> getPredictionsTestDataSameDiff() throws Exception
			Public Overrides ReadOnly Property PredictionsTestDataSameDiff As IList(Of IDictionary(Of String, INDArray))
				Get
    
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim mds As MultiDataSet = TrainingData.next()
    
					Dim list As IList(Of IDictionary(Of String, INDArray)) = New List(Of IDictionary(Of String, INDArray))()
    
					list.Add(Collections.singletonMap("in", mds.Features(0).reshape(ChrW(10), 1, 60)))
					'[batchsize, insize]
    
					Return list
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<String> getPredictionsNamesSameDiff() throws Exception
			Public Overrides ReadOnly Property PredictionsNamesSameDiff As IList(Of String)
				Get
					Return Collections.singletonList("out")
				End Get
			End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim iter As MultiDataSetIterator = TrainingDataUnnormalized
					Dim pp As MultiDataSetPreProcessor = Sub(multiDataSet)
					Dim l As INDArray = multiDataSet.getLabels(0)
					l = l.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(l.size(2) - 1))
					multiDataSet.setLabels(0, l)
					multiDataSet.setLabelsMaskArray(0, Nothing)
					End Sub
    
    
					iter.PreProcessor = New CompositeMultiDataSetPreProcessor(Normalizer, pp)
    
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
					Resources.copyDirectory("dl4j-integration-tests/data/uci_seq/train/features/", featuresDirTrain)
					Resources.copyDirectory("dl4j-integration-tests/data/uci_seq/train/labels/", labelsDirTrain)
    
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
					Resources.copyDirectory("dl4j-integration-tests/data/uci_seq/test/features/", featuresDirTest)
					Resources.copyDirectory("dl4j-integration-tests/data/uci_seq/test/labels/", labelsDirTest)
    
					Dim trainFeatures As SequenceRecordReader = New CSVSequenceRecordReader()
					trainFeatures.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresDirTest.getAbsolutePath() & "/%d.csv", 0, 149))
					Dim trainLabels As SequenceRecordReader = New CSVSequenceRecordReader()
					trainLabels.initialize(New org.datavec.api.Split.NumberedFileInputSplit(labelsDirTest.getAbsolutePath() & "/%d.csv", 0, 149))
    
					Dim testData As DataSetIterator = New SequenceRecordReaderDataSetIterator(trainFeatures, trainLabels, miniBatchSize, numLabelClasses, False, SequenceRecordReaderDataSetIterator.AlignmentMode.ALIGN_END)
    
					Dim iter As MultiDataSetIterator = New MultiDataSetIteratorAdapter(testData)
    
					Dim pp As MultiDataSetPreProcessor = Sub(multiDataSet)
					Dim l As INDArray = multiDataSet.getLabels(0)
					l = l.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(l.size(2) - 1))
					multiDataSet.setLabels(0, l)
					multiDataSet.setLabelsMaskArray(0, Nothing)
					End Sub
    
    
					iter.PreProcessor = New CompositeMultiDataSetPreProcessor(Normalizer, pp)
    
					Return iter
				End Get
			End Property

			Public Overrides Function doEvaluationSameDiff(ByVal sd As SameDiff, ByVal iter As MultiDataSetIterator, ByVal evaluations() As IEvaluation) As IEvaluation()
				sd.evaluate(iter, "out", 0, evaluations)
				Return evaluations
			End Function
		End Class


	End Class














End Namespace