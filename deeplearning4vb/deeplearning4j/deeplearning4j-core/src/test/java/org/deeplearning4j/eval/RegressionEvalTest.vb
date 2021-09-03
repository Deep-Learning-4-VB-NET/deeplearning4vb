Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.interval
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
Namespace org.deeplearning4j.eval

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Regression Eval Test") @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.JACKSON_SERDE) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) class RegressionEvalTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class RegressionEvalTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regression Eval Methods") void testRegressionEvalMethods()
		Friend Overridable Sub testRegressionEvalMethods()
			' Basic sanity check
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.ZERO).list().layer(0, (New OutputLayer.Builder()).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(5).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim f As INDArray = Nd4j.zeros(4, 10)
			Dim l As INDArray = Nd4j.ones(4, 5)
			Dim ds As New DataSet(f, l)
			Dim iter As DataSetIterator = New ExistingDataSetIterator(Collections.singletonList(ds))
			Dim re As org.nd4j.evaluation.regression.RegressionEvaluation = net.evaluateRegression(iter)
			For i As Integer = 0 To 4
				assertEquals(1.0, re.meanSquaredError(i), 1e-6)
				assertEquals(1.0, re.meanAbsoluteError(i), 1e-6)
			Next i
			Dim graphConf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.ZERO).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.TANH).nIn(10).nOut(5).build(), "in").setOutputs("0").build()
			Dim cg As New ComputationGraph(graphConf)
			cg.init()
			Dim re2 As RegressionEvaluation = cg.evaluateRegression(iter)
			For i As Integer = 0 To 4
				assertEquals(1.0, re2.meanSquaredError(i), 1e-6)
				assertEquals(1.0, re2.meanAbsoluteError(i), 1e-6)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regression Eval Per Output Masking") void testRegressionEvalPerOutputMasking()
		Friend Overridable Sub testRegressionEvalPerOutputMasking()
			Dim l As INDArray = Nd4j.create(New Double()() {
				New Double() { 1, 2, 3 },
				New Double() { 10, 20, 30 },
				New Double() { -5, -10, -20 }
			})
			Dim predictions As INDArray = Nd4j.zeros(l.shape())
			Dim mask As INDArray = Nd4j.create(New Double()() {
				New Double() { 0, 1, 1 },
				New Double() { 1, 1, 0 },
				New Double() { 0, 1, 0 }
			})
			Dim re As New RegressionEvaluation()
			re.eval(l, predictions, mask)
			Dim mse() As Double = { (10 * 10) / 1.0, (2 * 2 + 20 * 20 + 10 * 10) \ 3, (3 * 3) / 1.0 }
			Dim mae() As Double = { 10.0, (2 + 20 + 10) / 3.0, 3.0 }
			Dim rmse() As Double = { 10.0, Math.Sqrt((2 * 2 + 20 * 20 + 10 * 10) / 3.0), 3.0 }
			For i As Integer = 0 To 2
				assertEquals(mse(i), re.meanSquaredError(i), 1e-6)
				assertEquals(mae(i), re.meanAbsoluteError(i), 1e-6)
				assertEquals(rmse(i), re.rootMeanSquaredError(i), 1e-6)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regression Eval Time Series Split") void testRegressionEvalTimeSeriesSplit()
		Friend Overridable Sub testRegressionEvalTimeSeriesSplit()
			Dim out1 As INDArray = Nd4j.rand(New Integer() { 3, 5, 20 })
			Dim outSub1 As INDArray = out1.get(all(), all(), interval(0, 10))
			Dim outSub2 As INDArray = out1.get(all(), all(), interval(10, 20))
			Dim label1 As INDArray = Nd4j.rand(New Integer() { 3, 5, 20 })
			Dim labelSub1 As INDArray = label1.get(all(), all(), interval(0, 10))
			Dim labelSub2 As INDArray = label1.get(all(), all(), interval(10, 20))
			Dim e1 As New RegressionEvaluation()
			Dim e2 As New RegressionEvaluation()
			e1.eval(label1, out1)
			e2.eval(labelSub1, outSub1)
			e2.eval(labelSub2, outSub2)
			assertEquals(e1, e2)
		End Sub
	End Class

End Namespace