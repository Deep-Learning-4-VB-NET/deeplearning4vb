Imports System.Collections.Generic
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
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
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports Resources = org.nd4j.common.resources.Resources

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


	Public Class SameDiffMLPTestCases


		Public Shared ReadOnly Property MLPMnist As TestCase
			Get
				Return New TestCaseAnonymousInnerClass()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass
			Inherits TestCase

	'		{
	'			testName = "MLPMnistSD";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = True;
	'			maxRelativeErrorOverfit = 2e-2;
	'			minAbsErrorOverfit = 1e-2;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.SAMEDIFF
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Nd4j.Random.setSeed(12345)
    
					'Define the network structure:
					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 784)
					Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 10)
    
					Dim w0 As SDVariable = sd.var("w0", Nd4j.rand(DataType.FLOAT, 784, 256).muli(0.1))
					Dim b0 As SDVariable = sd.var("b0", Nd4j.rand(DataType.FLOAT, 256).muli(0.1))
					Dim w1 As SDVariable = sd.var("w1", Nd4j.rand(DataType.FLOAT, 256, 10).muli(0.1))
					Dim b1 As SDVariable = sd.var("b1", Nd4j.rand(DataType.FLOAT, 10).muli(0.1))
    
					Dim a0 As SDVariable = sd.nn_Conflict.tanh([in].mmul(w0).add(b0))
					Dim [out] As SDVariable = sd.nn_Conflict.softmax("out", a0.mmul(w1).add(b1))
					Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, [out])
    
					'Also set the training configuration:
					sd.TrainingConfig = TrainingConfig.builder().updater(New Adam(0.01)).weightDecay(1e-3, True).dataSetFeatureMapping("in").dataSetLabelMapping("label").build()
    
					Return sd
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<Map<String, org.nd4j.linalg.api.ndarray.INDArray>> getPredictionsTestDataSameDiff() throws Exception
			Public Overrides ReadOnly Property PredictionsTestDataSameDiff As IList(Of IDictionary(Of String, INDArray))
				Get
					Dim [out] As IList(Of IDictionary(Of String, INDArray)) = New List(Of IDictionary(Of String, INDArray))()
    
					Dim iter As DataSetIterator = New MnistDataSetIterator(1, True, 12345)
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					[out].Add(Collections.singletonMap("in", iter.next().getFeatures()))
    
					iter = New MnistDataSetIterator(8, True, 12345)
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					[out].Add(Collections.singletonMap("in", iter.next().getFeatures()))
    
					Return [out]
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<String> getPredictionsNamesSameDiff() throws Exception
			Public Overrides ReadOnly Property PredictionsNamesSameDiff As IList(Of String)
				Get
					Return Collections.singletonList("out")
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Map<String, org.nd4j.linalg.api.ndarray.INDArray> getGradientsTestDataSameDiff() throws Exception
			Public Overrides ReadOnly Property GradientsTestDataSameDiff As IDictionary(Of String, INDArray)
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = (New MnistDataSetIterator(8, True, 12345)).next()
					Dim map As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
					map("in") = ds.Features
					map("label") = ds.Labels
					Return map
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim iter As DataSetIterator = New MnistDataSetIterator(8, True, 12345)
					iter = New EarlyTerminationDataSetIterator(iter, 32)
					Return New MultiDataSetIteratorAdapter(iter)
				End Get
			End Property

			Public Overrides ReadOnly Property NewEvaluations As IEvaluation()
				Get
					Return New IEvaluation(){New Evaluation()}
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getEvaluationTestData() throws Exception
			Public Overrides ReadOnly Property EvaluationTestData As MultiDataSetIterator
				Get
					Dim iter As DataSetIterator = New MnistDataSetIterator(8, False, 12345)
					iter = New EarlyTerminationDataSetIterator(iter, 32)
					Return New MultiDataSetIteratorAdapter(iter)
				End Get
			End Property

			Public Overrides Function doEvaluationSameDiff(ByVal sd As SameDiff, ByVal iter As MultiDataSetIterator, ByVal evaluations() As IEvaluation) As IEvaluation()
				sd.evaluate(iter, "out", 0, evaluations)
				Return evaluations
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getOverfittingData() throws Exception
			Public Overrides ReadOnly Property OverfittingData As MultiDataSet
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Return (New MnistDataSetIterator(1, True, 12345)).next().toMultiDataSet()
				End Get
			End Property

			Public Overrides ReadOnly Property OverfitNumIterations As Integer
				Get
					Return 100
				End Get
			End Property
		End Class


		Public Shared ReadOnly Property MLPMoon As TestCase
			Get
				Return New TestCaseAnonymousInnerClass2()
    
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass2
			Inherits TestCase

	'		{
	'			testName = "MLPMoonSD";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = True;
	'			maxRelativeErrorOverfit = 2e-2;
	'			minAbsErrorOverfit = 1e-2;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.SAMEDIFF
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
    
					Dim numInputs As Integer = 2
					Dim numOutputs As Integer = 2
					Dim numHiddenNodes As Integer = 20
					Dim learningRate As Double = 0.005
    
    
					Nd4j.Random.setSeed(12345)
    
					'Define the network structure:
					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, numInputs)
					Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, numOutputs)
    
					Dim w0 As SDVariable = sd.var("w0", Nd4j.rand(DataType.FLOAT, numInputs, numHiddenNodes))
					Dim b0 As SDVariable = sd.var("b0", Nd4j.rand(DataType.FLOAT, numHiddenNodes))
					Dim w1 As SDVariable = sd.var("w1", Nd4j.rand(DataType.FLOAT, numHiddenNodes, numOutputs))
					Dim b1 As SDVariable = sd.var("b1", Nd4j.rand(DataType.FLOAT, numOutputs))
    
					Dim a0 As SDVariable = sd.nn_Conflict.relu([in].mmul(w0).add(b0), 0)
					Dim [out] As SDVariable = sd.nn_Conflict.softmax("out", a0.mmul(w1).add(b1))
					Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, [out])
    
					'Also set the training configuration:
					sd.TrainingConfig = TrainingConfig.builder().updater(New Nesterovs(learningRate, 0.9)).weightDecay(1e-3, True).dataSetFeatureMapping("in").dataSetLabelMapping("label").build()
    
					Return sd
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<Map<String, org.nd4j.linalg.api.ndarray.INDArray>> getPredictionsTestDataSameDiff() throws Exception
			Public Overrides ReadOnly Property PredictionsTestDataSameDiff As IList(Of IDictionary(Of String, INDArray))
				Get
					Dim [out] As IList(Of IDictionary(Of String, INDArray)) = New List(Of IDictionary(Of String, INDArray))()
    
					Dim f As File = Resources.asFile("dl4j-integration-tests/data/moon_data_eval.csv")
    
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
					Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 1, 0, 2)
    
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					[out].Add(Collections.singletonMap("in", iter.next().getFeatures()))
    
    
					Return [out]
				End Get
			End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<String> getPredictionsNamesSameDiff() throws Exception
			Public Overrides ReadOnly Property PredictionsNamesSameDiff As IList(Of String)
				Get
					Return Collections.singletonList("out")
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Map<String, org.nd4j.linalg.api.ndarray.INDArray> getGradientsTestDataSameDiff() throws Exception
			Public Overrides ReadOnly Property GradientsTestDataSameDiff As IDictionary(Of String, INDArray)
				Get
    
					Dim f As File = Resources.asFile("dl4j-integration-tests/data/moon_data_eval.csv")
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As org.nd4j.linalg.dataset.DataSet = (New RecordReaderDataSetIterator(rr, 5, 0, 2)).next()
    
					Dim map As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
					map("in") = ds.Features
					map("label") = ds.Labels
					Return map
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim f As File = Resources.asFile("dl4j-integration-tests/data/moon_data_train.csv")
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
					Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 32, 0, 2)
    
					iter = New EarlyTerminationDataSetIterator(iter, 32)
					Return New MultiDataSetIteratorAdapter(iter)
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
					Dim f As File = Resources.asFile("dl4j-integration-tests/data/moon_data_eval.csv")
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
					Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 32, 0, 2)
					Return New MultiDataSetIteratorAdapter(iter)
				End Get
			End Property


			Public Overrides Function doEvaluationSameDiff(ByVal sd As SameDiff, ByVal iter As MultiDataSetIterator, ByVal evaluations() As IEvaluation) As IEvaluation()
				sd.evaluate(iter, "out", 0, evaluations)
				Return evaluations
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getOverfittingData() throws Exception
			Public Overrides ReadOnly Property OverfittingData As MultiDataSet
				Get
    
					Dim f As File = Resources.asFile("dl4j-integration-tests/data/moon_data_eval.csv")
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Return (New RecordReaderDataSetIterator(rr, 1, 0, 2)).next().toMultiDataSet()
				End Get
			End Property

			Public Overrides ReadOnly Property OverfitNumIterations As Integer
				Get
					Return 200
				End Get
			End Property
		End Class
	End Class













End Namespace