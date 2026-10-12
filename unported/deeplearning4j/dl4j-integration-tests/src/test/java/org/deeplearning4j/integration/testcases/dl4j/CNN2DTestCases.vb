Imports System
Imports System.Collections.Generic
Imports FileSplit = org.datavec.api.split.FileSplit
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports ObjectDetectionRecordReader = org.datavec.image.recordreader.objdetect.ObjectDetectionRecordReader
Imports SvhnLabelProvider = org.datavec.image.recordreader.objdetect.impl.SvhnLabelProvider
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports SvhnDataFetcher = org.deeplearning4j.datasets.fetchers.SvhnDataFetcher
Imports ModelType = org.deeplearning4j.integration.ModelType
Imports TestCase = org.deeplearning4j.integration.TestCase
Imports DataSetType = org.deeplearning4j.datasets.fetchers.DataSetType
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports TinyImageNetDataSetIterator = org.deeplearning4j.datasets.iterator.impl.TinyImageNetDataSetIterator
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Yolo2OutputLayer = org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports ComputationGraphUtil = org.deeplearning4j.nn.graph.util.ComputationGraphUtil
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports FineTuneConfiguration = org.deeplearning4j.nn.transferlearning.FineTuneConfiguration
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports PretrainedType = org.deeplearning4j.zoo.PretrainedType
Imports TinyYOLO = org.deeplearning4j.zoo.model.TinyYOLO
Imports VGG16 = org.deeplearning4j.zoo.model.VGG16
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
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler
Imports VGG16ImagePreProcessor = org.nd4j.linalg.dataset.api.preprocessor.VGG16ImagePreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
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


	Public Class CNN2DTestCases

		''' <summary>
		''' Essentially: LeNet MNIST example
		''' </summary>
		Public Shared ReadOnly Property LenetMnist As TestCase
			Get
				Return New TestCaseAnonymousInnerClass()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass
			Inherits TestCase

	'		{
	'			testName = "LenetMnist";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = False;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.MLN
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Dim nChannels As Integer = 1 ' Number of input channels
					Dim outputNum As Integer = 10 ' The number of possible outcomes
					Dim seed As Integer = 123
    
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(seed).l2(0.0005).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(nChannels).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build()).layer(1, (New SubsamplingLayer.Builder(PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build()).layer(3, (New SubsamplingLayer.Builder(PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(4, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(500).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
    
					Return conf
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getGradientsTestData() throws Exception
			Public Overrides ReadOnly Property GradientsTestData As MultiDataSet
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = (New MnistDataSetIterator(8, False, 12345)).next()
					Return New org.nd4j.linalg.dataset.MultiDataSet(ds.Features, ds.Labels)
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim iter As DataSetIterator = New MnistDataSetIterator(16, True, 12345)
    
					iter = New EarlyTerminationDataSetIterator(iter, 60)
					Return New MultiDataSetIteratorAdapter(iter)
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getEvaluationTestData() throws Exception
			Public Overrides ReadOnly Property EvaluationTestData As MultiDataSetIterator
				Get
					Return New MultiDataSetIteratorAdapter(New EarlyTerminationDataSetIterator(New MnistDataSetIterator(32, False, 12345), 10))
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[],org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
			Public Overrides ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
				Get
					Dim iter As DataSetIterator = New MnistDataSetIterator(8, True, 12345)
					Dim list As IList(Of Pair(Of INDArray(), INDArray())) = New List(Of Pair(Of INDArray(), INDArray()))()
    
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					ds = ds.asList()(0)
					list.Add(New Pair(Of INDArray(), INDArray())(New INDArray(){ds.Features}, Nothing))
    
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					ds = iter.next()
					list.Add(New Pair(Of INDArray(), INDArray())(New INDArray(){ds.Features}, Nothing))
					Return list
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

		End Class


		''' <summary>
		''' VGG16 + transfer learning + tiny imagenet
		''' </summary>
		Public Shared ReadOnly Property VGG16TransferTinyImagenet As TestCase
			Get
				Return New TestCaseAnonymousInnerClass2()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass2
			Inherits TestCase

	'		{
	'			testName = "VGG16TransferTinyImagenet_224";
	'			testType = TestType.PRETRAINED;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = False; 'Skip - requires saving approx 1GB of data (gradients x2)
	'			testParamsPostTraining = False; 'Skip - requires saving all params (approx 500mb)
	'			testEvaluation = False;
	'			testOverfitting = False;
	'			maxRelativeErrorOutput = 0.2;
	'			minAbsErrorOutput = 0.05; 'Max value is around 0.22
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.CG
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.api.Model getPretrainedModel() throws Exception
			Public Overrides ReadOnly Property PretrainedModel As Model
				Get
					Dim vgg16 As VGG16 = VGG16.builder().seed(12345).build()
    
					Dim pretrained As ComputationGraph = CType(vgg16.initPretrained(PretrainedType.IMAGENET), ComputationGraph)
    
					'Transfer learning
					Dim newGraph As ComputationGraph = (New TransferLearning.GraphBuilder(pretrained)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).updater(New Adam(1e-3)).seed(12345).build()).removeVertexKeepConnections("predictions").addLayer("predictions", (New OutputLayer.Builder()).nIn(4096).nOut(200).build(), "fc2").build()
    
					Return newGraph
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
			Public Overrides ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
				Get
					Dim [out] As IList(Of Pair(Of INDArray(), INDArray())) = New List(Of Pair(Of INDArray(), INDArray()))()
    
					Dim iter As DataSetIterator = New TinyImageNetDataSetIterator(1, New Integer(){224, 224}, DataSetType.TRAIN, Nothing, 12345)
					iter.PreProcessor = New VGG16ImagePreProcessor()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					[out].Add(New Pair(Of INDArray(), INDArray())(New INDArray(){ds.Features}, Nothing))
    
					iter = New TinyImageNetDataSetIterator(3, New Integer(){224, 224}, DataSetType.TRAIN, Nothing, 54321)
					iter.PreProcessor = New VGG16ImagePreProcessor()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					ds = iter.next()
					[out].Add(New Pair(Of INDArray(), INDArray())(New INDArray(){ds.Features}, Nothing))
    
					Return [out]
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getGradientsTestData() throws Exception
			Public Overrides ReadOnly Property GradientsTestData As MultiDataSet
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = (New TinyImageNetDataSetIterator(8, New Integer(){224, 224}, DataSetType.TRAIN, Nothing, 12345)).next()
					Return New org.nd4j.linalg.dataset.MultiDataSet(ds.Features, ds.Labels)
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Dim iter As DataSetIterator = New TinyImageNetDataSetIterator(4, New Integer(){224, 224}, DataSetType.TRAIN, Nothing, 12345)
					iter.PreProcessor = New VGG16ImagePreProcessor()
    
					iter = New EarlyTerminationDataSetIterator(iter, 2)
					Return New MultiDataSetIteratorAdapter(iter)
				End Get
			End Property
		End Class


		''' <summary>
		''' Basically a cut-down version of the YOLO house numbers example
		''' </summary>
		Public Shared ReadOnly Property YoloHouseNumbers As TestCase
			Get
				Return New TestCaseAnonymousInnerClass3()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass3
			Inherits TestCase

			Private width As Integer = 416
			Private height As Integer = 416
			Private nChannels As Integer = 3
			Private gridWidth As Integer = 13
			Private gridHeight As Integer = 13

	'		{
	'			testName = "YOLOHouseNumbers";
	'			testType = TestType.PRETRAINED;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = False; 'Skip - requires saving approx 1GB of data (gradients x2)
	'			testParamsPostTraining = False; 'Skip - requires saving all params (approx 500mb)
	'			testEvaluation = False;
	'			testOverfitting = False;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.CG
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.api.Model getPretrainedModel() throws Exception
			Public Overrides ReadOnly Property PretrainedModel As Model
				Get
					Dim nClasses As Integer = 10
					Dim nBoxes As Integer = 5
					Dim lambdaNoObj As Double = 0.5
					Dim lambdaCoord As Double = 1.0
					Dim priorBoxes()() As Double = {
						New Double() {2, 5},
						New Double() {2.5, 6},
						New Double() {3, 7},
						New Double() {3.5, 8},
						New Double() {4, 9}
					}
					Dim learningRate As Double = 1e-4
					Dim pretrained As ComputationGraph = CType(TinyYOLO.builder().build().initPretrained(), ComputationGraph)
					Dim priors As INDArray = Nd4j.create(priorBoxes)
    
					Dim fineTuneConf As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).gradientNormalizationThreshold(1.0).updater(New Adam(learningRate)).l2(0.00001).activation(Activation.IDENTITY).trainingWorkspaceMode(WorkspaceMode.ENABLED).inferenceWorkspaceMode(WorkspaceMode.ENABLED).build()
    
					Dim model As ComputationGraph = (New TransferLearning.GraphBuilder(pretrained)).fineTuneConfiguration(fineTuneConf).removeVertexKeepConnections("conv2d_9").removeVertexAndConnections("outputs").addLayer("convolution2d_9", (New ConvolutionLayer.Builder(1,1)).nIn(1024).nOut(nBoxes * (5 + nClasses)).stride(1,1).convolutionMode(ConvolutionMode.Same).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).build(), "leaky_re_lu_8").addLayer("outputs", (New Yolo2OutputLayer.Builder()).lambdaNoObj(lambdaNoObj).lambdaCoord(lambdaCoord).boundingBoxPriors(priors).build(), "convolution2d_9").setOutputs("outputs").build()
    
					Return model
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
			Public Overrides ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim mds As MultiDataSet = outerInstance.TrainingData.next()
					Return Collections.singletonList(New Pair(Of )(mds.Features, Nothing))
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
					Dim fetcher As New SvhnDataFetcher()
					Dim testDir As File = fetcher.getDataSetPath(DataSetType.TEST)
    
					Dim testData As New org.datavec.api.Split.FileSplit(testDir, NativeImageLoader.ALLOWED_FORMATS, New Random(12345))
					Dim recordReaderTest As New ObjectDetectionRecordReader(height, width, nChannels, gridHeight, gridWidth, New SvhnLabelProvider(testDir))
					recordReaderTest.initialize(testData)
					Dim test As New RecordReaderDataSetIterator(recordReaderTest, 2, 1, 1, True)
					test.PreProcessor = New ImagePreProcessingScaler(0, 1)
    
					Return New MultiDataSetIteratorAdapter(New EarlyTerminationDataSetIterator(test, 2))
				End Get
			End Property
		End Class


		''' <summary>
		''' A synthetic 2D CNN that uses all layers:
		''' Convolution, Subsampling, Upsampling, Cropping, Depthwise conv, separable conv, deconv, space to batch,
		''' space to depth, zero padding, batch norm, LRN
		''' </summary>
		Public Shared ReadOnly Property Cnn2DSynthetic As TestCase
			Get
    
				Throw New System.NotSupportedException("Not yet implemented")
			End Get
		End Property


		Public Shared Function testLenetTransferDropoutRepeatability() As TestCase
			Return New TestCaseAnonymousInnerClass4()
		End Function

		Private Class TestCaseAnonymousInnerClass4
			Inherits TestCase

	'		{
	'			testName = "LenetDropoutRepeatability";
	'			testType = TestType.PRETRAINED;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = True;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.MLN
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.api.Model getPretrainedModel() throws Exception
			Public Overrides ReadOnly Property PretrainedModel As Model
				Get
    
					Dim lrSchedule As IDictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()
					lrSchedule(0) = 0.01
					lrSchedule(1000) = 0.005
					lrSchedule(3000) = 0.001
    
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(12345).l2(0.0005).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(1).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build()).layer(1, (New SubsamplingLayer.Builder(PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).dropOut(0.5).build()).layer(3, (New SubsamplingLayer.Builder(PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(4, (New DenseLayer.Builder()).activation(Activation.RELU).dropOut(0.5).nOut(500).build()).layer(5, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
    
    
					Dim net As New MultiLayerNetwork(conf)
					net.init()
    
					Dim iter As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(16, True, 12345), 10)
					net.fit(iter)
    
					Dim pretrained As MultiLayerNetwork = (New TransferLearning.Builder(net)).fineTuneConfiguration(FineTuneConfiguration.builder().updater(New Nesterovs(0.01, 0.9)).seed(98765).build()).nOutReplace(5, 10, WeightInit.XAVIER).build()
    
					Return pretrained
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
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
						New ROCMultiClass(),
						New EvaluationCalibration()
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
					Return 200
				End Get
			End Property
		End Class
	End Class

End Namespace