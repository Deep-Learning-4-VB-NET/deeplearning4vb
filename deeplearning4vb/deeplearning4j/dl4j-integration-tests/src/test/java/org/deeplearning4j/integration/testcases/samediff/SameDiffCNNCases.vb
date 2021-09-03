Imports System.Collections.Generic
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
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
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Conv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
Imports Pooling3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs

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


	Public Class SameDiffCNNCases


		Public Shared ReadOnly Property LenetMnist As TestCase
			Get
				Return New TestCaseAnonymousInnerClass()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass
			Inherits TestCase

	'		{
	'			testName = "LenetMnistSD";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = False;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.SAMEDIFF
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Nd4j.Random.setSeed(12345)
    
					Dim nChannels As Integer = 1 ' Number of input channels
					Dim outputNum As Integer = 10 ' The number of possible outcomes
    
					Dim sd As SameDiff = SameDiff.create()
					Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 784)
					Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, outputNum)
    
					'input [minibatch, channels=1, Height = 28, Width = 28]
					Dim in4d As SDVariable = [in].reshape(-1, nChannels, 28, 28)
    
					Dim kernelHeight As Integer = 5
					Dim kernelWidth As Integer = 5
    
    
					' w0 [kernelHeight = 5, kernelWidth = 5 , inputChannels = 1, outputChannels = 20]
					' b0 [20]
					Dim w0 As SDVariable = sd.var("w0", Nd4j.rand(DataType.FLOAT, kernelHeight, kernelWidth, nChannels, 20).muli(0.01))
					Dim b0 As SDVariable = sd.var("b0", Nd4j.rand(DataType.FLOAT, 20).muli(0.01))
    
    
					Dim layer0 As SDVariable = sd.nn_Conflict.relu(sd.cnn_Conflict.conv2d("layer0", in4d, w0, b0, Conv2DConfig.builder().kH(kernelHeight).kW(kernelWidth).sH(1).sW(1).dataFormat("NCHW").build()), 0)
    
					' outputSize = (inputSize - kernelSize + 2*padding) / stride + 1
					' outputsize_H(W) = ( 28 - 5 + 2*0 ) / 1 + 1 = 24
					' [minibatch,20,24,24]
    
    
					Dim layer1 As SDVariable = sd.cnn_Conflict.maxPooling2d("layer1", layer0, Pooling2DConfig.builder().kH(2).kW(2).sH(2).sW(2).isNHWC(False).build())
    
					' outputSize = (inputSize - kernelSize + 2*padding) / stride + 1
					' outputsize_H(W) = ( 24 - 2 + 2*0 ) / 2 + 1 = 12
					' [minibatch,12,12,20]
    
    
					' w2 [kernelHeight = 5, kernelWidth = 5 , inputChannels = 20, outputChannels = 50]
					' b0 [50]
					Dim w2 As SDVariable = sd.var("w2", Nd4j.rand(DataType.FLOAT, kernelHeight, kernelWidth, 20, 50).muli(0.01))
					Dim b2 As SDVariable = sd.var("b2", Nd4j.rand(DataType.FLOAT, 50).muli(0.01))
    
    
					Dim layer2 As SDVariable = sd.nn_Conflict.relu(sd.cnn_Conflict.conv2d("layer2", layer1, w2, b2, Conv2DConfig.builder().kH(kernelHeight).kW(kernelWidth).sH(1).sW(1).dataFormat("NCHW").build()), 0)
    
					' outputSize = (inputSize - kernelSize + 2*padding) / stride + 1
					' outputsize_H(W) = ( 12 - 5 + 2*0 ) / 1 + 1 = 8
					' [minibatch,8,8,50]
    
    
					Dim layer3 As SDVariable = sd.cnn_Conflict.maxPooling2d("layer3", layer2, Pooling2DConfig.builder().kH(2).kW(2).sH(2).sW(2).isNHWC(False).build())
    
    
					' outputSize = (inputSize - kernelSize + 2*padding) / stride + 1
					' outputsize_H(W) = ( 8 - 2 + 2*0 ) / 2 + 1 = 4
					' [minibatch,4,4,50]
    
					Dim channels_height_width As Integer = 4 * 4 * 50
					Dim layer3_reshaped As SDVariable = layer3.reshape(-1, channels_height_width)
    
					Dim w4 As SDVariable = sd.var("w4", Nd4j.rand(DataType.FLOAT, channels_height_width, 500).muli(0.01))
					Dim b4 As SDVariable = sd.var("b4", Nd4j.rand(DataType.FLOAT, 500).muli(0.01))
    
    
					Dim layer4 As SDVariable = sd.nn_Conflict.relu("layer4", layer3_reshaped.mmul(w4).add(b4), 0)
    
					Dim w5 As SDVariable = sd.var("w5", Nd4j.rand(DataType.FLOAT, 500, outputNum))
					Dim b5 As SDVariable = sd.var("b5", Nd4j.rand(DataType.FLOAT, outputNum))
    
					Dim [out] As SDVariable = sd.nn_Conflict.softmax("out", layer4.mmul(w5).add(b5))
					Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, [out])
    
					'Also set the training configuration:
					sd.TrainingConfig = TrainingConfig.builder().updater(New Adam(1e-3)).l2(1e-3).dataSetFeatureMapping("in").dataSetLabelMapping("label").build()
    
    
					Return sd
    
    
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
'ORIGINAL LINE: @Override public List<Map<String, org.nd4j.linalg.api.ndarray.INDArray>> getPredictionsTestDataSameDiff() throws Exception
			Public Overrides ReadOnly Property PredictionsTestDataSameDiff As IList(Of IDictionary(Of String, INDArray))
				Get
					Dim iter As DataSetIterator = New MnistDataSetIterator(8, True, 12345)
    
					Dim list As IList(Of IDictionary(Of String, INDArray)) = New List(Of IDictionary(Of String, INDArray))()
    
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As org.nd4j.linalg.dataset.DataSet = iter.next()
					ds = ds.asList()(0)
    
					list.Add(Collections.singletonMap("in", ds.Features))
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					ds = iter.next()
					list.Add(Collections.singletonMap("in", ds.Features))
					Return list
				End Get
			End Property

			Public Overrides ReadOnly Property PredictionsNamesSameDiff As IList(Of String)
				Get
					Return Collections.singletonList("out")
    
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



			Public Overrides Function doEvaluationSameDiff(ByVal sd As SameDiff, ByVal iter As MultiDataSetIterator, ByVal evaluations() As IEvaluation) As IEvaluation()
				sd.evaluate(iter, "out", 0, evaluations)
				Return evaluations
			End Function

		End Class


		Public Shared ReadOnly Property Cnn3dSynthetic As TestCase
			Get
				Return New TestCaseAnonymousInnerClass2()
    
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass2
			Inherits TestCase

	'		{
	'			testName = "Cnn3dSynthetic";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = False;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.SAMEDIFF
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Nd4j.Random.setSeed(12345)
    
					Dim nChannels As Integer = 3 ' Number of input channels
					Dim outputNum As Integer = 10 ' The number of possible outcomes
    
					Dim sd As SameDiff = SameDiff.create()
    
    
					'input in NCDHW [minibatch, channels=3, Height = 8, Width = 8, Depth = 8]
					Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, nChannels, 8, 8, 8)
    
					Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, nChannels, outputNum)
    
					'input in NCDHW [minibatch, channels=3, Height = 8, Width = 8, Depth = 8]
    
					' Weights for conv3d. Rank 5 with shape [kernelDepth, kernelHeight, kernelWidth, inputChannels, outputChannels]
					' [kernelDepth = 3, kernelHeight = 3, kernelWidth = 3, inputChannels = 3, outputChannels = 8]
					Dim w0 As SDVariable = sd.var("w0", Nd4j.rand(DataType.FLOAT, 3, 3, 3, nChannels, 8))
					' Optional 1D bias array with shape [outputChannels]. May be null.
					Dim b0 As SDVariable = sd.var("b0", Nd4j.rand(DataType.FLOAT, 8))
    
    
					Dim layer0 As SDVariable = sd.nn_Conflict.relu(sd.cnn_Conflict.conv3d("layer0", [in], w0, b0, Conv3DConfig.builder().kH(3).kW(3).kD(3).sH(2).sW(2).sD(2).dataFormat("NCDHW").build()), 0)
    
					' outputSize = (inputSize - kernelSize + 2*padding) / stride + 1
					' outputsize_H(W)(D) = (8 - 3 + 2*0 ) / 2 + 1 = 3
					' [minibatch,8,3,3,3]
    
    
					Dim layer1 As SDVariable = sd.cnn_Conflict.maxPooling3d("layer1", layer0, Pooling3DConfig.builder().kH(2).kW(2).kD(2).sH(2).sW(2).sD(2).isNCDHW(True).build())
    
					' outputSize = (inputSize - kernelSize + 2*padding) / stride + 1
					' outputsize_H(W)(D) = ( 3 - 2 + 2*0 ) / 2 + 1 = 1
					' [minibatch,8,1,1,1]
    
    
					Dim channels_height_width_depth As Integer = 8 * 1 * 1 * 1
    
					Dim layer1_reshaped As SDVariable = layer1.reshape(-1, channels_height_width_depth)
    
					Dim w1 As SDVariable = sd.var("w4", Nd4j.rand(DataType.FLOAT, channels_height_width_depth, 10))
					Dim b1 As SDVariable = sd.var("b4", Nd4j.rand(DataType.FLOAT, 10))
    
    
					Dim [out] As SDVariable = sd.nn_Conflict.softmax("out", layer1_reshaped.mmul(w1).add(b1))
					Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, [out])
    
					'Also set the training configuration:
					sd.TrainingConfig = TrainingConfig.builder().updater(New Nesterovs(0.01, 0.9)).dataSetFeatureMapping("in").dataSetLabelMapping("label").build()
    
					Return sd
    
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Map<String,org.nd4j.linalg.api.ndarray.INDArray> getGradientsTestDataSameDiff() throws Exception
			Public Overrides ReadOnly Property GradientsTestDataSameDiff As IDictionary(Of String, INDArray)
				Get
					Nd4j.Random.setSeed(12345)
					'NCDHW format
					Dim arr As INDArray = Nd4j.rand(New Integer(){2, 3, 8, 8, 8})
					Dim labels As INDArray = org.deeplearning4j.integration.TestUtils.randomOneHot(2, 10)
    
					Dim map As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
					map("in") = arr
					map("label") = labels
					Return map
    
				End Get
			End Property



			Public Overrides ReadOnly Property PredictionsNamesSameDiff As IList(Of String)
				Get
    
					Return Collections.singletonList("out")
    
				End Get
			End Property



'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<Map<String, org.nd4j.linalg.api.ndarray.INDArray>> getPredictionsTestDataSameDiff() throws Exception
			Public Overrides ReadOnly Property PredictionsTestDataSameDiff As IList(Of IDictionary(Of String, INDArray))
				Get
					Nd4j.Random.setSeed(12345)
    
					Dim list As IList(Of IDictionary(Of String, INDArray)) = New List(Of IDictionary(Of String, INDArray))()
					Dim arr As INDArray = Nd4j.rand(New Integer(){2, 3, 8, 8, 8})
    
					list.Add(Collections.singletonMap("in", arr))
    
					Return list
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.MultiDataSet getGradientsTestData() throws Exception
			Public Overrides ReadOnly Property GradientsTestData As MultiDataSet
				Get
					Nd4j.Random.setSeed(12345)
					'NCDHW format
					Dim arr As INDArray = Nd4j.rand(New Integer(){2, 3, 8, 8, 8})
					Dim labels As INDArray = org.deeplearning4j.integration.TestUtils.randomOneHot(2, 10)
					Return New org.nd4j.linalg.dataset.MultiDataSet(arr, labels)
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
			Public Overrides ReadOnly Property TrainingData As MultiDataSetIterator
				Get
					Return New SingletonMultiDataSetIterator(outerInstance.GradientsTestData)
				End Get
			End Property


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getEvaluationTestData() throws Exception
			Public Overrides ReadOnly Property EvaluationTestData As MultiDataSetIterator
				Get
					Return outerInstance.TrainingData
				End Get
			End Property

			Public Overrides Function doEvaluationSameDiff(ByVal sd As SameDiff, ByVal iter As MultiDataSetIterator, ByVal evaluations() As IEvaluation) As IEvaluation()
				sd.evaluate(iter, "out", 0, evaluations)
				Return evaluations
			End Function

			Public Overrides ReadOnly Property NewEvaluations As IEvaluation()
				Get
					Return New IEvaluation(){New Evaluation()}
				End Get
			End Property


		End Class
	End Class
End Namespace