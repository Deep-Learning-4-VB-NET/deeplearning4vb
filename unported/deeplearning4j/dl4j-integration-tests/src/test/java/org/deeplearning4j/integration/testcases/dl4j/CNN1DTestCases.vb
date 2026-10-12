Imports System.Collections.Generic
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports ModelType = org.deeplearning4j.integration.ModelType
Imports TestCase = org.deeplearning4j.integration.TestCase
Imports CharacterIterator = org.deeplearning4j.integration.testcases.dl4j.misc.CharacterIterator
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Cropping1D = org.deeplearning4j.nn.conf.layers.convolutional.Cropping1D
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports org.nd4j.common.primitives
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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


	Public Class CNN1DTestCases


		''' <summary>
		''' A simple CNN 1d test case using most CNN 1d layers:
		''' Subsampling, Upsampling, Convolution, Cropping, Zero padding
		''' </summary>
		Public Shared ReadOnly Property Cnn1dTestCaseCharRNN As TestCase
			Get
				Return New TestCaseAnonymousInnerClass()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass
			Inherits TestCase

	'		{
	'			testName = "CNN1dCharacterTestCase";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testTrainingCurves = True;
	'			testGradients = True;
	'			testParamsPostTraining = True;
	'			testEvaluation = True;
	'			testOverfitting = False;
	'		}

			Friend miniBatchSize As Integer = 16
			Friend exampleLength As Integer = 128

			Public Overrides Function modelType() As ModelType
				Return ModelType.CG
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Object getConfiguration() throws Exception
			Public Overrides ReadOnly Property Configuration As Object
				Get
					Dim iter As CharacterIterator = CharacterIterator.getShakespeareIterator(miniBatchSize,exampleLength)
					Dim nOut As Integer = iter.totalOutcomes()
    
					Return (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(12345).weightInit(WeightInit.XAVIER).updater(New Adam(0.01)).convolutionMode(ConvolutionMode.Same).graphBuilder().addInputs("in").layer("0", (New Convolution1DLayer.Builder()).nOut(32).activation(Activation.TANH).kernelSize(3).stride(1).build(), "in").layer("1", (New Subsampling1DLayer.Builder()).kernelSize(2).stride(1).poolingType(SubsamplingLayer.PoolingType.MAX).build(), "0").layer("2", New Cropping1D(1), "1").layer("3", New ZeroPadding1DLayer(1), "2").layer("out", (New RnnOutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nOut(nOut).build(), "3").setInputTypes(InputType.recurrent(nOut)).setOutputs("out").build()
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
					iter = New EarlyTerminationDataSetIterator(iter, 2) '3 minibatches, 1000/200 = 5 updates per minibatch
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

	End Class

End Namespace