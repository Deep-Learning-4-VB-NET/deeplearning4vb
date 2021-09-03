Imports System.Collections.Generic
Imports ModelType = org.deeplearning4j.integration.ModelType
Imports TestCase = org.deeplearning4j.integration.TestCase
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraphUtil = org.deeplearning4j.nn.graph.util.ComputationGraphUtil
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports EvaluationCalibration = org.nd4j.evaluation.classification.EvaluationCalibration
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports MapSchedule = org.nd4j.linalg.schedule.MapSchedule
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType

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


	Public Class MLPTestCases


		''' <summary>
		''' A simple MLP test case using MNIST iterator.
		''' Also has LR schedule built-in
		''' </summary>
		Public Shared ReadOnly Property MLPMnist As TestCase
			Get
				Return New TestCaseAnonymousInnerClass2()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass2
			Inherits TestCase

	'		{
	'			testName = "MLPMnist";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = True;
	'			maxRelativeErrorOverfit = 2e-2;
	'			minAbsErrorOverfit = 1e-2;
	'			maxRelativeErrorGradients = 0.01;
	'			minAbsErrorGradients = 0.05;
	'			maxRelativeErrorParamsPostTraining = 0.01;
	'			minAbsErrorParamsPostTraining = 0.05;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.MLN
			End Function

			Public Overrides ReadOnly Property Configuration As Object
				Get
					Return (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(12345).updater(New Adam((New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 5e-2).add(4, 4e-2).add(8, 3e-2).add(12, 2e-2).add(14, 1e-2).build())).l1(1e-3).l2(1e-3).list().layer((New DenseLayer.Builder()).activation(Activation.TANH).nOut(64).build()).layer((New OutputLayer.Builder()).nOut(10).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28,28,1)).build()
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
			Public Overrides ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
				Get
					Dim iter As New MnistDataSetIterator(1, True, 12345)
					Dim [out] As IList(Of Pair(Of INDArray(), INDArray())) = New List(Of Pair(Of INDArray(), INDArray()))()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					[out].Add(New Pair(Of INDArray(), INDArray())(New INDArray(){iter.next().Features}, Nothing))
    
					iter = New MnistDataSetIterator(10, True, 12345)
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					[out].Add(New Pair(Of INDArray(), INDArray())(New INDArray(){iter.next().Features}, Nothing))
					Return [out]
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getGradientsTestData() throws Exception
			Public Overrides ReadOnly Property GradientsTestData As MultiDataSet
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = (New MnistDataSetIterator(10, True, 12345)).next()
					Return New org.nd4j.linalg.dataset.MultiDataSet(ds.Features, ds.Labels)
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim iter As DataSetIterator = New MnistDataSetIterator(16, True, 12345)
					iter = New EarlyTerminationDataSetIterator(iter, 32)
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
					Dim iter As DataSetIterator = New MnistDataSetIterator(16, True, 12345)
					iter = New EarlyTerminationDataSetIterator(iter, 10)
					Return New MultiDataSetIteratorAdapter(iter)
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getOverfittingData() throws Exception
			Public Overrides ReadOnly Property OverfittingData As MultiDataSet
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = (New MnistDataSetIterator(1, True, 12345)).next()
					Return ComputationGraphUtil.toMultiDataSet(ds)
				End Get
			End Property

			Public Overrides ReadOnly Property OverfitNumIterations As Integer
				Get
					Return 300
				End Get
			End Property
		End Class


		''' <summary>
		''' A test case that mirrors MLP Moon example using CSVRecordReader
		''' </summary>
		Public Shared ReadOnly Property MLPMoon As TestCase
			Get
				Return New TestCaseAnonymousInnerClass3()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass3
			Inherits TestCase

	'		{
	'			testName = "MLPMoon";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = False; 'Not much point here: very simple training data
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.MLN
			End Function

			Public Overrides ReadOnly Property Configuration As Object
				Get
					Dim seed As Integer = 123
					Dim learningRate As Double = 0.005
    
					Dim numInputs As Integer = 2
					Dim numOutputs As Integer = 2
					Dim numHiddenNodes As Integer = 20
    
					'log.info("Build model....");
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(seed).updater(New Nesterovs(learningRate, 0.9)).list().layer(0, (New DenseLayer.Builder()).nIn(numInputs).nOut(numHiddenNodes).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(numHiddenNodes).nOut(numOutputs).build()).build()
					Return conf
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
			Public Overrides ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
				Get
					Dim f As File = (New ClassPathResource("dl4j-integration-tests/data/moon_data_eval.csv")).File
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
					Dim testIter As DataSetIterator = New RecordReaderDataSetIterator(rr,1,0,2)
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim next1 As INDArray = testIter.next().getFeatures()
    
					testIter = New RecordReaderDataSetIterator(rr,10,0,2)
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim next10 As INDArray = testIter.next().getFeatures()
    
					Return New List(Of Pair(Of INDArray(), INDArray())) From {
						New Pair(Of )(New INDArray(){next1}, Nothing),
						New Pair(Of )(New INDArray(){next10}, Nothing)
					}
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getGradientsTestData() throws Exception
			Public Overrides ReadOnly Property GradientsTestData As MultiDataSet
				Get
					Dim f As File = (New ClassPathResource("dl4j-integration-tests/data/moon_data_eval.csv")).File
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = (New RecordReaderDataSetIterator(rr,5,0,2)).next()
					Return ComputationGraphUtil.toMultiDataSet(ds)
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim f As File = (New ClassPathResource("dl4j-integration-tests/data/moon_data_train.csv")).File
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
					Dim trainIter As DataSetIterator = New RecordReaderDataSetIterator(rr,32,0,2)
					Return New MultiDataSetIteratorAdapter(trainIter)
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
					Dim f As File = (New ClassPathResource("dl4j-integration-tests/data/moon_data_eval.csv")).File
					Dim rr As RecordReader = New CSVRecordReader()
					rr.initialize(New org.datavec.api.Split.FileSplit(f))
					Dim testIter As DataSetIterator = New RecordReaderDataSetIterator(rr,32,0,2)
					Return New MultiDataSetIteratorAdapter(testIter)
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