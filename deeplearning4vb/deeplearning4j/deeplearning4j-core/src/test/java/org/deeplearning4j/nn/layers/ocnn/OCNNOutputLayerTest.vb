Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports GradientCheckUtil = org.deeplearning4j.gradientcheck.GradientCheckUtil
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports ActivationReLU = org.nd4j.linalg.activations.impl.ActivationReLU
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType
Imports StepSchedule = org.nd4j.linalg.schedule.StepSchedule
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.layers.ocnn

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Ocnn Output Layer Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.FILE_IO) class OCNNOutputLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class OCNNOutputLayerTest
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True

		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False

		Private Const DEFAULT_EPS As Double = 1e-6

		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3

		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Layer") void testLayer()
		Friend Overridable Sub testLayer()
			Dim dataSetIterator As DataSetIterator = NormalizedIterator
			Dim doLearningFirst As Boolean = True
			Dim network As MultiLayerNetwork = getGradientCheckNetwork(2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = dataSetIterator.next()
			Dim arr As INDArray = ds.Features
			network.Input = arr
			If doLearningFirst Then
				' Run a number of iterations of learning
				network.Input = arr
				network.setListeners(New ScoreIterationListener(1))
				network.computeGradientAndScore()
				Dim scoreBefore As Double = network.score()
				For j As Integer = 0 To 9
					network.fit(ds)
				Next j
				network.computeGradientAndScore()
				Dim scoreAfter As Double = network.score()
				' Can't test in 'characteristic mode of operation' if not learning
				Dim msg As String = "testLayer() - score did not (sufficiently) decrease during learning - activationFn=" & "relu" & ", lossFn=" & "ocnn" & ", " & "sigmoid" & ", doLearningFirst=" & doLearningFirst & " (before=" & scoreBefore & ", scoreAfter=" & scoreAfter & ")"
				' assertTrue(msg, scoreAfter <  scoreBefore);
			End If
			If PRINT_RESULTS Then
				Console.WriteLine("testLayer() - activationFn=" & "relu" & ", lossFn=" & "ocnn" & "sigmoid" & ", doLearningFirst=" & doLearningFirst)
				Dim j As Integer = 0
				Do While j < network.getnLayers()
					Console.WriteLine("Layer " & j & " # params: " & network.getLayer(j).numParams())
					j += 1
				Loop
			End If
			Dim gradOK As Boolean = GradientCheckUtil.checkGradients(network, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, ds.Features, ds.Labels)
			Dim msg As String = "testLayer() - activationFn=" & "relu" & ", lossFn=" & "ocnn" & ",=" & "sigmoid" & ", doLearningFirst=" & doLearningFirst
			assertTrue(gradOK,msg)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Label Probabilities") void testLabelProbabilities() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLabelProbabilities()
			Nd4j.Random.setSeed(42)
			Dim dataSetIterator As DataSetIterator = NormalizedIterator
			Dim network As MultiLayerNetwork = SingleLayer
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = dataSetIterator.next()
			Dim filtered As DataSet = [next].filterBy(New Integer() { 0, 1 })
			For i As Integer = 0 To 9
				network.EpochCount = i
				network.LayerWiseConfigurations.EpochCount = i
				network.fit(filtered)
			Next i
			Dim anomalies As DataSet = [next].filterBy(New Integer() { 2 })
			Dim output As INDArray = network.output(anomalies.Features)
			Dim normalOutput As INDArray = network.output(anomalies.Features, False)
			assertEquals(output.lt(0.0).castTo(Nd4j.defaultFloatingPointType()).sumNumber().doubleValue(), normalOutput.eq(0.0).castTo(Nd4j.defaultFloatingPointType()).sumNumber().doubleValue(), 1e-1)
			' System.out.println("Labels " + anomalies.getLabels());
			' System.out.println("Anomaly output " + normalOutput);
			' System.out.println(output);
			Dim normalProbs As INDArray = network.output(filtered.Features)
			Dim outputForNormalSamples As INDArray = network.output(filtered.Features, False)
			Console.WriteLine("Normal probabilities " & normalProbs)
			Console.WriteLine("Normal raw output " & outputForNormalSamples)
			Dim tmpFile As New File(testDir.toFile(), "tmp-file-" & System.Guid.randomUUID().ToString())
			ModelSerializer.writeModel(network, tmpFile, True)
			tmpFile.deleteOnExit()
			Dim multiLayerNetwork As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(tmpFile)
			assertEquals(network.params(), multiLayerNetwork.params())
			assertEquals(network.numParams(), multiLayerNetwork.numParams())
		End Sub

		Public Overridable ReadOnly Property NormalizedIterator As DataSetIterator
			Get
				Dim dataSetIterator As DataSetIterator = New IrisDataSetIterator(150, 150)
				Dim normalizerStandardize As New NormalizerStandardize()
				normalizerStandardize.fit(dataSetIterator)
				dataSetIterator.reset()
				dataSetIterator.PreProcessor = normalizerStandardize
				Return dataSetIterator
			End Get
		End Property

		Private ReadOnly Property SingleLayer As MultiLayerNetwork
			Get
				Dim numHidden As Integer = 2
				Dim configuration As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).miniBatch(True).updater(New Adam(0.1)).list((New DenseLayer.Builder()).activation(New ActivationReLU()).nIn(4).nOut(2).build(), (New org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer.Builder()).nIn(2).activation(New ActivationSigmoid()).initialRValue(0.1).nu(0.1).hiddenLayerSize(numHidden).build()).build()
				Dim network As New MultiLayerNetwork(configuration)
				network.init()
				network.setListeners(New ScoreIterationListener(1))
				Return network
			End Get
		End Property

		Public Overridable Function getGradientCheckNetwork(ByVal numHidden As Integer) As MultiLayerNetwork
			Dim configuration As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(42).updater(New NoOp()).miniBatch(False).list((New DenseLayer.Builder()).activation(New ActivationIdentity()).nIn(4).nOut(4).build(), (New org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer.Builder()).nIn(4).nu(0.002).activation(New ActivationSigmoid()).hiddenLayerSize(numHidden).build()).build()
			Dim network As New MultiLayerNetwork(configuration)
			network.init()
			Return network
		End Function
	End Class

End Namespace