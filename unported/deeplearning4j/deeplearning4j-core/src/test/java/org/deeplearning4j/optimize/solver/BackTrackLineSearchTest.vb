Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports BackTrackLineSearch = org.deeplearning4j.optimize.solvers.BackTrackLineSearch
Imports DefaultStepFunction = org.deeplearning4j.optimize.stepfunctions.DefaultStepFunction
Imports NegativeDefaultStepFunction = org.deeplearning4j.optimize.stepfunctions.NegativeDefaultStepFunction
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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
Namespace org.deeplearning4j.optimize.solver

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Back Track Line Search Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.FILE_IO) class BackTrackLineSearchTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class BackTrackLineSearchTest
		Inherits BaseDL4JTest

		Private irisIter As DataSetIterator

		Private irisData As DataSet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void before()
		Friend Overridable Sub before()
			If irisIter Is Nothing Then
				irisIter = New IrisDataSetIterator(5, 5)
			End If
			If irisData Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				irisData = irisIter.next()
				irisData.normalizeZeroMeanZeroUnitVariance()
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Single Min Line Search") void testSingleMinLineSearch() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSingleMinLineSearch()
			Dim layer As OutputLayer = getIrisLogisticLayerConfig(Activation.SOFTMAX, 100, LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)
			Dim nParams As Integer = CInt(layer.numParams())
			layer.BackpropGradientsViewArray = Nd4j.create(1, nParams)
			layer.setInput(irisData.Features, LayerWorkspaceMgr.noWorkspaces())
			layer.Labels = irisData.Labels
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			Dim lineSearch As New BackTrackLineSearch(layer, layer.Optimizer)
			Dim [step] As Double = lineSearch.optimize(layer.params(), layer.gradient().gradient(), layer.gradient().gradient(), LayerWorkspaceMgr.noWorkspacesImmutable())
			assertEquals(1.0, [step], 1e-3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Single Max Line Search") void testSingleMaxLineSearch() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSingleMaxLineSearch()
			Dim score1, score2 As Double
			Dim layer As OutputLayer = getIrisLogisticLayerConfig(Activation.SOFTMAX, 100, LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)
			Dim nParams As Integer = CInt(layer.numParams())
			layer.BackpropGradientsViewArray = Nd4j.create(1, nParams)
			layer.setInput(irisData.Features, LayerWorkspaceMgr.noWorkspaces())
			layer.Labels = irisData.Labels
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			score1 = layer.score()
			Dim lineSearch As New BackTrackLineSearch(layer, New NegativeDefaultStepFunction(), layer.Optimizer)
			Dim [step] As Double = lineSearch.optimize(layer.params(), layer.gradient().gradient(), layer.gradient().gradient(), LayerWorkspaceMgr.noWorkspacesImmutable())
			assertEquals(1.0, [step], 1e-3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Mult Min Line Search") void testMultMinLineSearch() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultMinLineSearch()
			Dim score1, score2 As Double
			Dim layer As OutputLayer = getIrisLogisticLayerConfig(Activation.SOFTMAX, 100, LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)
			Dim nParams As Integer = CInt(layer.numParams())
			layer.BackpropGradientsViewArray = Nd4j.create(1, nParams)
			layer.setInput(irisData.Features, LayerWorkspaceMgr.noWorkspaces())
			layer.Labels = irisData.Labels
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			score1 = layer.score()
			Dim origGradient As INDArray = layer.gradient().gradient().dup()
			Dim sf As New NegativeDefaultStepFunction()
			Dim lineSearch As New BackTrackLineSearch(layer, sf, layer.Optimizer)
			Dim [step] As Double = lineSearch.optimize(layer.params(), layer.gradient().gradient(), layer.gradient().gradient(), LayerWorkspaceMgr.noWorkspacesImmutable())
			Dim currParams As INDArray = layer.params()
			sf.step(currParams, origGradient, [step])
			layer.Params = currParams
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			score2 = layer.score()
			assertTrue(score1 > score2,"score1=" & score1 & ", score2=" & score2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Mult Max Line Search") void testMultMaxLineSearch() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultMaxLineSearch()
			Dim score1, score2 As Double
			irisData.normalizeZeroMeanZeroUnitVariance()
			Dim layer As OutputLayer = getIrisLogisticLayerConfig(Activation.SOFTMAX, 100, LossFunctions.LossFunction.MCXENT)
			Dim nParams As Integer = CInt(layer.numParams())
			layer.BackpropGradientsViewArray = Nd4j.create(1, nParams)
			layer.setInput(irisData.Features, LayerWorkspaceMgr.noWorkspaces())
			layer.Labels = irisData.Labels
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			score1 = layer.score()
			Dim origGradient As INDArray = layer.gradient().gradient().dup()
			Dim sf As New DefaultStepFunction()
			Dim lineSearch As New BackTrackLineSearch(layer, sf, layer.Optimizer)
			Dim [step] As Double = lineSearch.optimize(layer.params().dup(), layer.gradient().gradient().dup(), layer.gradient().gradient().dup(), LayerWorkspaceMgr.noWorkspacesImmutable())
			Dim currParams As INDArray = layer.params()
			sf.step(currParams, origGradient, [step])
			layer.Params = currParams
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			score2 = layer.score()
			assertTrue(score1 < score2,"score1 = " & score1 & ", score2 = " & score2)
		End Sub

		Private Shared Function getIrisLogisticLayerConfig(ByVal activationFunction As Activation, ByVal maxIterations As Integer, ByVal lossFunction As LossFunctions.LossFunction) As OutputLayer
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).miniBatch(True).maxNumLineSearchIterations(maxIterations).layer((New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(lossFunction)).nIn(4).nOut(3).activation(activationFunction).weightInit(WeightInit.XAVIER).build()).build()
			Dim numParams As val = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Return CType(conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType()), OutputLayer)
		End Function

		' /////////////////////////////////////////////////////////////////////////
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Back Track Line Gradient Descent") void testBackTrackLineGradientDescent()
		Friend Overridable Sub testBackTrackLineGradientDescent()
			Dim optimizer As OptimizationAlgorithm = OptimizationAlgorithm.LINE_GRADIENT_DESCENT
			Dim irisIter As DataSetIterator = New IrisDataSetIterator(1, 1)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim data As DataSet = irisIter.next()
			Dim network As New MultiLayerNetwork(getIrisMultiLayerConfig(Activation.SIGMOID, optimizer))
			network.init()
			Dim listener As TrainingListener = New ScoreIterationListener(10)
			network.setListeners(Collections.singletonList(listener))
			Dim oldScore As Double = network.score(data)
			For i As Integer = 0 To 99
				network.fit(data.Features, data.Labels)
			Next i
			Dim score As Double = network.score()
			assertTrue(score < oldScore)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Back Track Line CG") void testBackTrackLineCG()
		Friend Overridable Sub testBackTrackLineCG()
			Dim optimizer As OptimizationAlgorithm = OptimizationAlgorithm.CONJUGATE_GRADIENT
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim data As DataSet = irisIter.next()
			data.normalizeZeroMeanZeroUnitVariance()
			Dim network As New MultiLayerNetwork(getIrisMultiLayerConfig(Activation.RELU, optimizer))
			network.init()
			Dim listener As TrainingListener = New ScoreIterationListener(10)
			network.setListeners(Collections.singletonList(listener))
			Dim firstScore As Double = network.score(data)
			For i As Integer = 0 To 4
				network.fit(data.Features, data.Labels)
			Next i
			Dim score As Double = network.score()
			assertTrue(score < firstScore)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Back Track Line LBFGS") void testBackTrackLineLBFGS()
		Friend Overridable Sub testBackTrackLineLBFGS()
			Dim optimizer As OptimizationAlgorithm = OptimizationAlgorithm.LBFGS
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim data As DataSet = irisIter.next()
			data.normalizeZeroMeanZeroUnitVariance()
			Dim network As New MultiLayerNetwork(getIrisMultiLayerConfig(Activation.RELU, optimizer))
			network.init()
			Dim listener As TrainingListener = New ScoreIterationListener(10)
			network.setListeners(Collections.singletonList(listener))
			Dim oldScore As Double = network.score(data)
			For i As Integer = 0 To 4
				network.fit(data.Features, data.Labels)
			Next i
			Dim score As Double = network.score()
			assertTrue(score < oldScore)
		End Sub

		Private Shared Function getIrisMultiLayerConfig(ByVal activationFunction As Activation, ByVal optimizer As OptimizationAlgorithm) As MultiLayerConfiguration
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(optimizer).updater(New Adam(0.01)).seed(12345L).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(100).weightInit(WeightInit.XAVIER).activation(activationFunction).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(100).nOut(3).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).build()).build()
			Return conf
		End Function
	End Class

End Namespace