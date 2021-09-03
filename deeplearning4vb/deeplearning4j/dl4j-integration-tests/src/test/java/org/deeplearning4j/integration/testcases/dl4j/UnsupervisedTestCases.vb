Imports System.Collections.Generic
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports ModelType = org.deeplearning4j.integration.ModelType
Imports TestCase = org.deeplearning4j.integration.TestCase
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BernoulliReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.BernoulliReconstructionDistribution
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Adam = org.nd4j.linalg.learning.config.Adam
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



	Public Class UnsupervisedTestCases

		''' <summary>
		''' Basically: the MNIST VAE anomaly example
		''' </summary>
		Public Shared ReadOnly Property VAEMnistAnomaly As TestCase
			Get
				Return New TestCaseAnonymousInnerClass()
			End Get
		End Property

		Private Class TestCaseAnonymousInnerClass
			Inherits TestCase

	'		{
	'			testName = "VAEMnistAnomaly";
	'			testType = TestType.RANDOM_INIT;
	'			testPredictions = True;
	'			testUnsupervisedTraining = True;
	'			testTrainingCurves = False;
	'			testParamsPostTraining = False;
	'			testGradients = False;
	'			testEvaluation = False;
	'			testOverfitting = False;
	'			unsupervisedTrainLayersMLN = New int[]{0};
	'			maxRelativeErrorParamsPostTraining = 1e-4;
	'			minAbsErrorParamsPostTraining = 5e-4;
	'			maxRelativeErrorPretrainParams = 1e-4;
	'			minAbsErrorPretrainParams = 5e-4;
	'		}

			Public Overrides Function modelType() As ModelType
				Return ModelType.MLN
			End Function

			Public Overrides ReadOnly Property Configuration As Object
				Get
					Return (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).seed(12345).updater(New Adam(1e-3)).weightInit(WeightInit.XAVIER).l2(1e-4).list().layer(0, (New VariationalAutoencoder.Builder()).activation(Activation.TANH).encoderLayerSizes(256, 256).decoderLayerSizes(256, 256).pzxActivationFunction(Activation.IDENTITY).reconstructionDistribution(New BernoulliReconstructionDistribution(Activation.SIGMOID)).nIn(28 * 28).nOut(32).build()).build()
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
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getUnsupervisedTrainData() throws Exception
			Public Overrides ReadOnly Property UnsupervisedTrainData As MultiDataSetIterator
				Get
					Dim iter As DataSetIterator = New MnistDataSetIterator(16, True, 12345)
					iter = New EarlyTerminationDataSetIterator(iter, 32)
					Return New MultiDataSetIteratorAdapter(iter)
				End Get
			End Property
		End Class

	End Class

End Namespace