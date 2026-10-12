Imports System.Collections.Generic
Imports SingletonMultiDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonMultiDataSetIterator
Imports ModelType = org.deeplearning4j.integration.ModelType
Imports TestCase = org.deeplearning4j.integration.TestCase
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports Subsampling3DLayer = org.deeplearning4j.nn.conf.layers.Subsampling3DLayer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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


	Public Class CNN3DTestCases


		''' <summary>
		''' A simple synthetic CNN 3d test case using all CNN 3d layers:
		''' Subsampling, Upsampling, Convolution, Cropping, Zero padding
		''' </summary>
		Public Shared ReadOnly Property Cnn3dTestCaseSynthetic As TestCase
			Get
				Return New TestCaseAnonymousInnerClass();}
    
			End Get
		End Property

	Private Class TestCaseAnonymousInnerClass
		Inherits TestCase

	'	{
	'		testName = "Cnn3dSynthetic";
	'		testType = TestType.RANDOM_INIT;
	'		testPredictions = True;
	'		testTrainingCurves = True;
	'		testGradients = True;
	'		testParamsPostTraining = True;
	'		testEvaluation = True;
	'		testOverfitting = False;
	'	}

		Public Overrides Function modelType() As ModelType
			Return ModelType.MLN
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Object getConfiguration() throws Exception
		Public Overrides ReadOnly Property Configuration As Object
			Get
				Dim nChannels As Integer = 3 ' Number of input channels
				Dim outputNum As Integer = 10 ' The number of possible outcomes
				Dim seed As Integer = 123
    
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).l2(0.0005).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).convolutionMode(ConvolutionMode.Same).list().layer((New Convolution3D.Builder(3,3,3)).dataFormat(Convolution3D.DataFormat.NCDHW).nIn(nChannels).stride(2, 2, 2).nOut(8).activation(Activation.IDENTITY).build()).layer((New Subsampling3DLayer.Builder(PoolingType.MAX)).kernelSize(2, 2, 2).stride(2, 2, 2).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(outputNum).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional3D(8,8,8,nChannels)).build()
    
				Return conf
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

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[],org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
		Public Overrides ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
			Get
				Dim mds As MultiDataSet = outerInstance.GradientsTestData
				Return Collections.singletonList(New Pair(Of )(mds.Features, Nothing))
			End Get
		End Property

		Public Overrides ReadOnly Property NewEvaluations As IEvaluation()
			Get
				Return New IEvaluation(){New Evaluation()}
			End Get
		End Property

	End Class

End Namespace