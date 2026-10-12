Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Model = org.deeplearning4j.nn.api.Model
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports org.nd4j.evaluation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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

Namespace org.deeplearning4j.integration


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public abstract class TestCase
	Public MustInherit Class TestCase

		Public Enum TestType
			PRETRAINED
			RANDOM_INIT
		End Enum

		'See: readme.md for more details
		Protected Friend testName As String 'Name of the test, for display purposes
		Protected Friend testType As TestType 'Type of model - from a pretrained model, or a randomly initialized model
		Protected Friend testPredictions As Boolean = True 'If true: check the predictions/output. Requires getPredictionsTestData() to be implemented
		Protected Friend testGradients As Boolean = True 'If true: check the gradients. Requires getGradientsTestData() to be implemented
		Protected Friend testUnsupervisedTraining As Boolean = False 'If true: perform unsupervised training. Only applies to layers like autoencoders, VAEs, etc. Requires getUnsupervisedTrainData() to be implemented
		Protected Friend testTrainingCurves As Boolean = True 'If true: perform training, and compare loss vs. iteration. Requires getTrainingData() method
		Protected Friend testParamsPostTraining As Boolean = True 'If true: perform training, and compare parameters after training. Requires getTrainingData() method
		Protected Friend testEvaluation As Boolean = True 'If true: perform evaluation. Requires getNewEvaluations() and getEvaluationTestData() methods implemented
		Protected Friend testParallelInference As Boolean = True 'If true: run the model through ParallelInference. Requires getPredictionsTestData() method. Only applies to DL4J models, NOT SameDiff models
		Protected Friend testOverfitting As Boolean = True 'If true: perform overfitting, and ensure the predictions match the training data. Requires both getOverfittingData() and getOverfitNumIterations()

		Protected Friend unsupervisedTrainLayersMLN() As Integer = Nothing
		Protected Friend unsupervisedTrainLayersCG() As String = Nothing

		'Relative errors for this test case:
		Protected Friend maxRelativeErrorOutput As Double = 1e-4
		Protected Friend minAbsErrorOutput As Double = 1e-4
		Protected Friend maxRelativeErrorGradients As Double = 1e-4
		Protected Friend minAbsErrorGradients As Double = 1e-4
		Protected Friend maxRelativeErrorPretrainParams As Double = 1e-5
		Protected Friend minAbsErrorPretrainParams As Double = 1e-5
		Protected Friend maxRelativeErrorScores As Double = 1e-6
		Protected Friend minAbsErrorScores As Double = 1e-5
		Protected Friend maxRelativeErrorParamsPostTraining As Double = 1e-4
		Protected Friend minAbsErrorParamsPostTraining As Double = 1e-4
		Protected Friend maxRelativeErrorOverfit As Double = 1e-2
		Protected Friend minAbsErrorOverfit As Double = 1e-2

		Public MustOverride Function modelType() As ModelType

		''' <summary>
		''' Initialize the test case... many tests don't need this; others may use it to download or create data </summary>
		''' <param name="testWorkingDir"> Working directory to use for test </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void initialize(java.io.File testWorkingDir) throws Exception
		Public Overridable Sub initialize(ByVal testWorkingDir As File)
			'No op by default
		End Sub

		''' <summary>
		''' Required if NOT a pretrained model (testType == TestType.RANDOM_INIT)
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Object getConfiguration() throws Exception
		Public Overridable ReadOnly Property Configuration As Object
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required for pretrained models (testType == TestType.PRETRAINED)
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.api.Model getPretrainedModel() throws Exception
		Public Overridable ReadOnly Property PretrainedModel As Model
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required if testPredictions == true && DL4J model (MultiLayerNetwork or ComputationGraph)
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.util.List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[],org.nd4j.linalg.api.ndarray.INDArray[]>> getPredictionsTestData() throws Exception
		Public Overridable ReadOnly Property PredictionsTestData As IList(Of Pair(Of INDArray(), INDArray()))
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required if testPredictions == true && SameDiff model
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.util.List<java.util.Map<String,org.nd4j.linalg.api.ndarray.INDArray>> getPredictionsTestDataSameDiff() throws Exception
		Public Overridable ReadOnly Property PredictionsTestDataSameDiff As IList(Of IDictionary(Of String, INDArray))
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.util.List<String> getPredictionsNamesSameDiff() throws Exception
		Public Overridable ReadOnly Property PredictionsNamesSameDiff As IList(Of String)
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required if testGradients == true && DL4J model
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.MultiDataSet getGradientsTestData() throws Exception
		Public Overridable ReadOnly Property GradientsTestData As MultiDataSet
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required if testGradients == true && SameDiff model
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.util.Map<String,org.nd4j.linalg.api.ndarray.INDArray> getGradientsTestDataSameDiff() throws Exception
		Public Overridable ReadOnly Property GradientsTestDataSameDiff As IDictionary(Of String, INDArray)
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required when testUnsupervisedTraining == true
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getUnsupervisedTrainData() throws Exception
		Public Overridable ReadOnly Property UnsupervisedTrainData As MultiDataSetIterator
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <returns> Training data - DataSetIterator or MultiDataSetIterator </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getTrainingData() throws Exception
		Public Overridable ReadOnly Property TrainingData As MultiDataSetIterator
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required if testEvaluation == true
		''' </summary>
		Public Overridable ReadOnly Property NewEvaluations As IEvaluation()
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		Public Overridable Function doEvaluationSameDiff(ByVal sd As SameDiff, ByVal iter As MultiDataSetIterator, ByVal evaluations() As IEvaluation) As IEvaluation()
			Throw New Exception("Implementations must override this method if used")
		End Function

		''' <summary>
		''' Required if testEvaluation == true
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator getEvaluationTestData() throws Exception
		Public Overridable ReadOnly Property EvaluationTestData As MultiDataSetIterator
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required if testOverfitting == true && DL4J model
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.nd4j.linalg.dataset.api.MultiDataSet getOverfittingData() throws Exception
		Public Overridable ReadOnly Property OverfittingData As MultiDataSet
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required if testOverfitting == true && SameDiff model
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public java.util.Map<String,org.nd4j.linalg.api.ndarray.INDArray> getOverfittingDataSameDiff() throws Exception
		Public Overridable ReadOnly Property OverfittingDataSameDiff As IDictionary(Of String, INDArray)
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property

		''' <summary>
		''' Required if testOverfitting == true
		''' </summary>
		Public Overridable ReadOnly Property OverfitNumIterations As Integer
			Get
				Throw New Exception("Implementations must override this method if used")
			End Get
		End Property


	End Class

End Namespace