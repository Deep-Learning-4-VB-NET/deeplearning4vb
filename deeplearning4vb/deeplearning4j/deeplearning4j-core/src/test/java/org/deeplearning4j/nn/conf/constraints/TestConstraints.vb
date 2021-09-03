Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports MaxNormConstraint = org.deeplearning4j.nn.conf.constraint.MaxNormConstraint
Imports MinMaxNormConstraint = org.deeplearning4j.nn.conf.constraint.MinMaxNormConstraint
Imports NonNegativeConstraint = org.deeplearning4j.nn.conf.constraint.NonNegativeConstraint
Imports UnitNormConstraint = org.deeplearning4j.nn.conf.constraint.UnitNormConstraint
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
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
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.nn.conf.constraints


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestConstraints extends org.deeplearning4j.BaseDL4JTest
	Public Class TestConstraints
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLayerRecurrentConstraints() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLayerRecurrentConstraints()

			Dim constraints() As LayerConstraint = {
				New MaxNormConstraint(0.5, 1),
				New MinMaxNormConstraint(0.3, 0.4, 1.0, 1),
				New NonNegativeConstraint(),
				New UnitNormConstraint(1)
			}

			For Each lc As LayerConstraint In constraints

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.0)).dist(New NormalDistribution(0, 5)).list().layer((New LSTM.Builder()).nIn(12).nOut(10).constrainRecurrent(lc).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(8).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim exp As LayerConstraint = lc.clone()
				assertEquals(exp.ToString(), net.getLayer(0).conf().getLayer().getConstraints().get(0).ToString())

				Dim input As INDArray = Nd4j.rand(3, 12)
				Dim labels As INDArray = Nd4j.rand(3, 8)

				net.fit(input.reshape(ChrW(3), 12, 1), labels)

				Dim RW0 As INDArray = net.getParam("0_RW")


				If TypeOf lc Is MaxNormConstraint Then
					assertTrue(RW0.norm2(1).maxNumber().doubleValue() <= 0.5)

				ElseIf TypeOf lc Is MinMaxNormConstraint Then
					assertTrue(RW0.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(RW0.norm2(1).maxNumber().doubleValue() <= 0.4)
				ElseIf TypeOf lc Is NonNegativeConstraint Then
					assertTrue(RW0.minNumber().doubleValue() >= 0.0)
				ElseIf TypeOf lc Is UnitNormConstraint Then
					assertEquals(1.0, RW0.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, RW0.norm2(1).maxNumber().doubleValue(), 1e-6)
				End If

				TestUtils.testModelSerialization(net)
			Next lc
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLayerBiasConstraints() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLayerBiasConstraints()

			Dim constraints() As LayerConstraint = {
				New MaxNormConstraint(0.5, 1),
				New MinMaxNormConstraint(0.3, 0.4, 1.0, 1),
				New NonNegativeConstraint(),
				New UnitNormConstraint(1)
			}

			For Each lc As LayerConstraint In constraints

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.0)).dist(New NormalDistribution(0, 5)).biasInit(10.0).list().layer((New DenseLayer.Builder()).nIn(12).nOut(10).constrainBias(lc).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(8).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim exp As LayerConstraint = lc.clone()
				assertEquals(exp.ToString(), net.getLayer(0).conf().getLayer().getConstraints().get(0).ToString())

				Dim input As INDArray = Nd4j.rand(3, 12)
				Dim labels As INDArray = Nd4j.rand(3, 8)

				net.fit(input, labels)

				Dim b0 As INDArray = net.getParam("0_b")


				If TypeOf lc Is MaxNormConstraint Then
					assertTrue(b0.norm2(1).maxNumber().doubleValue() <= 0.5)

				ElseIf TypeOf lc Is MinMaxNormConstraint Then
					assertTrue(b0.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(b0.norm2(1).maxNumber().doubleValue() <= 0.4)
				ElseIf TypeOf lc Is NonNegativeConstraint Then
					assertTrue(b0.minNumber().doubleValue() >= 0.0)
				ElseIf TypeOf lc Is UnitNormConstraint Then
					assertEquals(1.0, b0.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, b0.norm2(1).maxNumber().doubleValue(), 1e-6)
				End If

				TestUtils.testModelSerialization(net)
			Next lc
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLayerWeightsConstraints() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLayerWeightsConstraints()

			Dim constraints() As LayerConstraint = {
				New MaxNormConstraint(0.5, 1),
				New MinMaxNormConstraint(0.3, 0.4, 1.0, 1),
				New NonNegativeConstraint(),
				New UnitNormConstraint(1)
			}

			For Each lc As LayerConstraint In constraints

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.0)).dist(New NormalDistribution(0, 5)).list().layer((New DenseLayer.Builder()).nIn(12).nOut(10).constrainWeights(lc).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(8).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim exp As LayerConstraint = lc.clone()
				assertEquals(exp.ToString(), net.getLayer(0).conf().getLayer().getConstraints().get(0).ToString())

				Dim input As INDArray = Nd4j.rand(3, 12)
				Dim labels As INDArray = Nd4j.rand(3, 8)

				net.fit(input, labels)

				Dim w0 As INDArray = net.getParam("0_W")


				If TypeOf lc Is MaxNormConstraint Then
					assertTrue(w0.norm2(1).maxNumber().doubleValue() <= 0.5)

				ElseIf TypeOf lc Is MinMaxNormConstraint Then
					assertTrue(w0.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(w0.norm2(1).maxNumber().doubleValue() <= 0.4)
				ElseIf TypeOf lc Is NonNegativeConstraint Then
					assertTrue(w0.minNumber().doubleValue() >= 0.0)
				ElseIf TypeOf lc Is UnitNormConstraint Then
					assertEquals(1.0, w0.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, w0.norm2(1).maxNumber().doubleValue(), 1e-6)
				End If

				TestUtils.testModelSerialization(net)
			Next lc
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLayerWeightsAndBiasConstraints() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLayerWeightsAndBiasConstraints()

			Dim constraints() As LayerConstraint = {
				New MaxNormConstraint(0.5, 1),
				New MinMaxNormConstraint(0.3, 0.4, 1.0, 1),
				New NonNegativeConstraint(),
				New UnitNormConstraint(1)
			}

			For Each lc As LayerConstraint In constraints

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.0)).dist(New NormalDistribution(0, 5)).biasInit(0.2).list().layer((New DenseLayer.Builder()).nIn(12).nOut(10).constrainAllParameters(lc).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(8).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim exp As LayerConstraint = lc.clone()
				assertEquals(exp.ToString(), net.getLayer(0).conf().getLayer().getConstraints().get(0).ToString())

				Dim input As INDArray = Nd4j.rand(3, 12)
				Dim labels As INDArray = Nd4j.rand(3, 8)

				net.fit(input, labels)

				Dim w0 As INDArray = net.getParam("0_W")
				Dim b0 As INDArray = net.getParam("0_b")


				If TypeOf lc Is MaxNormConstraint Then
					assertTrue(w0.norm2(1).maxNumber().doubleValue() <= 0.5)
					assertTrue(b0.norm2(1).maxNumber().doubleValue() <= 0.5)

				ElseIf TypeOf lc Is MinMaxNormConstraint Then
					assertTrue(w0.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(w0.norm2(1).maxNumber().doubleValue() <= 0.4)
					assertTrue(b0.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(b0.norm2(1).maxNumber().doubleValue() <= 0.4)
				ElseIf TypeOf lc Is NonNegativeConstraint Then
					assertTrue(w0.minNumber().doubleValue() >= 0.0)
					assertTrue(b0.minNumber().doubleValue() >= 0.0)
				ElseIf TypeOf lc Is UnitNormConstraint Then
					assertEquals(1.0, w0.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, w0.norm2(1).maxNumber().doubleValue(), 1e-6)
					assertEquals(1.0, b0.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, b0.norm2(1).maxNumber().doubleValue(), 1e-6)
				End If

				TestUtils.testModelSerialization(net)
			Next lc
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLayerWeightsAndBiasSeparateConstraints() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLayerWeightsAndBiasSeparateConstraints()

			Dim constraints() As LayerConstraint = {
				New MaxNormConstraint(0.5, 1),
				New MinMaxNormConstraint(0.3, 0.4, 1.0, 1),
				New NonNegativeConstraint(),
				New UnitNormConstraint(1)
			}

			For Each lc As LayerConstraint In constraints

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.0)).dist(New NormalDistribution(0, 5)).biasInit(0.2).list().layer((New DenseLayer.Builder()).nIn(12).nOut(10).constrainWeights(lc).constrainBias(lc).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(8).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim exp As LayerConstraint = lc.clone()
				assertEquals(exp.ToString(), net.getLayer(0).conf().getLayer().getConstraints().get(0).ToString())

				Dim input As INDArray = Nd4j.rand(3, 12)
				Dim labels As INDArray = Nd4j.rand(3, 8)

				net.fit(input, labels)

				Dim w0 As INDArray = net.getParam("0_W")
				Dim b0 As INDArray = net.getParam("0_b")


				If TypeOf lc Is MaxNormConstraint Then
					assertTrue(w0.norm2(1).maxNumber().doubleValue() <= 0.5)
					assertTrue(b0.norm2(1).maxNumber().doubleValue() <= 0.5)

				ElseIf TypeOf lc Is MinMaxNormConstraint Then
					assertTrue(w0.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(w0.norm2(1).maxNumber().doubleValue() <= 0.4)
					assertTrue(b0.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(b0.norm2(1).maxNumber().doubleValue() <= 0.4)
				ElseIf TypeOf lc Is NonNegativeConstraint Then
					assertTrue(w0.minNumber().doubleValue() >= 0.0)
					assertTrue(b0.minNumber().doubleValue() >= 0.0)
				ElseIf TypeOf lc Is UnitNormConstraint Then
					assertEquals(1.0, w0.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, w0.norm2(1).maxNumber().doubleValue(), 1e-6)
					assertEquals(1.0, b0.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, b0.norm2(1).maxNumber().doubleValue(), 1e-6)
				End If

				TestUtils.testModelSerialization(net)
			Next lc
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testModelConstraints() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
			Public Overridable Sub testModelConstraints()

			Dim constraints() As LayerConstraint = {
				New MaxNormConstraint(0.5, 1),
				New MinMaxNormConstraint(0.3, 0.4, 1.0, 1),
				New NonNegativeConstraint(),
				New UnitNormConstraint(1)
			}

			For Each lc As LayerConstraint In constraints

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).constrainWeights(lc).updater(New Sgd(0.0)).dist(New NormalDistribution(0,5)).biasInit(1).list().layer((New DenseLayer.Builder()).nIn(12).nOut(10).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(8).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim exp As LayerConstraint = lc.clone()
				assertEquals(exp.ToString(), net.getLayer(0).conf().getLayer().getConstraints().get(0).ToString())
				assertEquals(exp.ToString(), net.getLayer(1).conf().getLayer().getConstraints().get(0).ToString())

				Dim input As INDArray = Nd4j.rand(3, 12)
				Dim labels As INDArray = Nd4j.rand(3, 8)

				net.fit(input, labels)

				Dim w0 As INDArray = net.getParam("0_W")
				Dim w1 As INDArray = net.getParam("1_W")

				If TypeOf lc Is MaxNormConstraint Then
					assertTrue(w0.norm2(1).maxNumber().doubleValue() <= 0.5)
					assertTrue(w1.norm2(1).maxNumber().doubleValue() <= 0.5)
				ElseIf TypeOf lc Is MinMaxNormConstraint Then
					assertTrue(w0.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(w0.norm2(1).maxNumber().doubleValue() <= 0.4)
					assertTrue(w1.norm2(1).minNumber().doubleValue() >= 0.3)
					assertTrue(w1.norm2(1).maxNumber().doubleValue() <= 0.4)
				ElseIf TypeOf lc Is NonNegativeConstraint Then
					assertTrue(w0.minNumber().doubleValue() >= 0.0)
				ElseIf TypeOf lc Is UnitNormConstraint Then
					assertEquals(1.0, w0.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, w0.norm2(1).maxNumber().doubleValue(), 1e-6)
					assertEquals(1.0, w1.norm2(1).minNumber().doubleValue(), 1e-6)
					assertEquals(1.0, w1.norm2(1).maxNumber().doubleValue(), 1e-6)
				End If

				TestUtils.testModelSerialization(net)
			Next lc
			End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConstraints()
		Public Overridable Sub testConstraints()

			Dim learningRate As Double = 0.001
			Dim nIn As Integer = 10
			Dim lstmLayerSize As Integer = 32

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.RELU_UNIFORM).updater(New RmsProp(learningRate)).graphBuilder().addInputs("input_lstm", "input_cpc").addLayer("first_lstm_layer", (New LSTM.Builder()).nIn(nIn).nOut(lstmLayerSize).activation(Activation.RELU).constrainWeights(New NonNegativeConstraint()).build(), "input_lstm").addVertex("lastTimeStep", New LastTimeStepVertex("input_lstm"), "first_lstm_layer").addVertex("merge", New MergeVertex(), "lastTimeStep", "input_cpc").addLayer("dense", (New DenseLayer.Builder()).constrainWeights(New NonNegativeConstraint()).nIn(lstmLayerSize + 1).nOut(lstmLayerSize\2).activation(Activation.RELU).build(), "merge").addLayer("second_dense", (New DenseLayer.Builder()).constrainWeights(New NonNegativeConstraint()).nIn(lstmLayerSize\2).nOut(lstmLayerSize\8).activation(Activation.RELU).build(), "dense").addLayer("output_layer", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).constrainWeights(New NonNegativeConstraint()).nIn(lstmLayerSize\8).nOut(1).activation(Activation.IDENTITY).build(), "second_dense").setOutputs("output_layer").backpropType(BackpropType.Standard).build()

			Dim g As New ComputationGraph(conf)
			g.init()


			For i As Integer = 0 To 99
				Dim in1 As INDArray = Nd4j.rand(New Integer(){1, nIn, 5})
				Dim in2 As INDArray = Nd4j.rand(New Integer(){1, 1})
				Dim label As INDArray = Nd4j.rand(New Integer(){1, 1})
				g.fit(New INDArray(){in1, in2}, New INDArray(){label})

				For Each e As KeyValuePair(Of String, INDArray) In g.paramTable().SetOfKeyValuePairs()
					If Not e.Key.contains("W") Then
						Continue For
					End If

					Dim min As Double = e.Value.minNumber().doubleValue()
					assertTrue(min >= 0.0)
				Next e
			Next i
		End Sub


	End Class

End Namespace