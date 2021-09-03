Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports DropConnect = org.deeplearning4j.nn.conf.weightnoise.DropConnect
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports ExponentialSchedule = org.nd4j.linalg.schedule.ExponentialSchedule
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.nn.misc

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.WORKSPACES) public class TestLrChanges extends org.deeplearning4j.BaseDL4JTest
	Public Class TestLrChanges
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testChangeLrMLN()
		Public Overridable Sub testChangeLrMLN()
			'First: Set LR for a *single* layer and compare vs. equivalent net config
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).updater(New Adam(0.1)).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).updater(New RmsProp(0.01)).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).updater(New NoOp()).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			For i As Integer = 0 To 9
				net.fit(Nd4j.rand(10,10), Nd4j.rand(10,10))
			Next i


			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).updater(New Adam(0.5)).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).updater(New RmsProp(0.01)).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).updater(New NoOp()).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()
			net2.Updater.StateViewArray.assign(net.Updater.StateViewArray)
			conf2.setIterationCount(conf.getIterationCount())
			net2.Params = net.params().dup()

			assertEquals(0.1, net.getLearningRate(0).Value, 0.0)
			net.setLearningRate(0, 0.5) 'Set LR for layer 0 to 0.5
			assertEquals(0.5, net.getLearningRate(0).Value, 0.0)

			assertEquals(conf, conf2)
			assertEquals(conf.toJson(), conf2.toJson())

			assertEquals(net.Updater.StateViewArray, net2.Updater.StateViewArray)

			'Perform some parameter updates - check things are actually in sync...
			For i As Integer = 0 To 2
				Dim [in] As INDArray = Nd4j.rand(10, 10)
				Dim l As INDArray = Nd4j.rand(10, 10)

				net.fit([in], l)
				net2.fit([in], l)
			Next i

			assertEquals(net.params(), net2.params())
			assertEquals(net.Updater.StateViewArray, net2.Updater.StateViewArray)

			Dim in1 As INDArray = Nd4j.rand(10, 10)
			Dim l1 As INDArray = Nd4j.rand(10, 10)

			net.Input = in1
			net.Labels = l1
			net.computeGradientAndScore()

			net2.Input = in1
			net2.Labels = l1
			net2.computeGradientAndScore()

			assertEquals(net.score(), net2.score(), 1e-8)


			'Now: Set *all* LRs to say 0.3...
			Dim conf3 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).updater(New Adam(0.3)).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).updater(New RmsProp(0.3)).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).updater(New NoOp()).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
			Dim net3 As New MultiLayerNetwork(conf3)
			net3.init()
			net3.Updater.StateViewArray.assign(net.Updater.StateViewArray)
			conf3.setIterationCount(conf.getIterationCount())
			net3.Params = net.params().dup()

			net.setLearningRate(0.3)

			'Perform some parameter updates - check things are actually in sync...
			For i As Integer = 0 To 2
				Dim [in] As INDArray = Nd4j.rand(10, 10)
				Dim l As INDArray = Nd4j.rand(10, 10)

				net.fit([in], l)
				net3.fit([in], l)
			Next i

			assertEquals(net.params(), net3.params())
			assertEquals(net.Updater.StateViewArray, net3.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testChangeLSGD()
		Public Overridable Sub testChangeLSGD()
			'Simple test for no updater nets
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).updater(New Sgd(0.1)).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.setLearningRate(1.0)
			net.setLearningRate(1, 0.5)
			assertEquals(1.0, net.getLearningRate(0), 0.0)
			assertEquals(0.5, net.getLearningRate(1), 0.0)


			Dim cg As ComputationGraph = net.toComputationGraph()
			cg.setLearningRate(2.0)
			cg.setLearningRate("1", 2.5)
			assertEquals(2.0, cg.getLearningRate("0"), 0.0)
			assertEquals(2.5, cg.getLearningRate("1"), 0.0)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testChangeLrMLNSchedule()
		Public Overridable Sub testChangeLrMLNSchedule()
			'First: Set LR for a *single* layer and compare vs. equivalent net config
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).updater(New Adam(0.1)).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			For i As Integer = 0 To 9
				net.fit(Nd4j.rand(10,10), Nd4j.rand(10,10))
			Next i


			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).updater(New Adam(New ExponentialSchedule(ScheduleType.ITERATION, 0.5, 0.8))).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()
			net2.Updater.StateViewArray.assign(net.Updater.StateViewArray)
			conf2.setIterationCount(conf.getIterationCount())
			net2.Params = net.params().dup()

			net.setLearningRate(New ExponentialSchedule(ScheduleType.ITERATION, 0.5, 0.8)) 'Set LR for layer 0 to 0.5

			assertEquals(conf, conf2)
			assertEquals(conf.toJson(), conf2.toJson())

			assertEquals(net.Updater.StateViewArray, net2.Updater.StateViewArray)

			'Perform some parameter updates - check things are actually in sync...
			For i As Integer = 0 To 2
				Dim [in] As INDArray = Nd4j.rand(10, 10)
				Dim l As INDArray = Nd4j.rand(10, 10)

				net.fit([in], l)
				net2.fit([in], l)
			Next i

			assertEquals(net.params(), net2.params())
			assertEquals(net.Updater.StateViewArray, net2.Updater.StateViewArray)
		End Sub







'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testChangeLrCompGraph()
		Public Overridable Sub testChangeLrCompGraph()
			'First: Set LR for a *single* layer and compare vs. equivalent net config
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).updater(New Adam(0.1)).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).updater(New RmsProp(0.01)).build(), "0").addLayer("2", (New OutputLayer.Builder()).nIn(10).nOut(10).updater(New NoOp()).lossFunction(LossFunctions.LossFunction.MSE).build(), "1").setOutputs("2").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			For i As Integer = 0 To 9
				net.fit(New DataSet(Nd4j.rand(10,10), Nd4j.rand(10,10)))
			Next i


			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).updater(New Adam(0.5)).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).updater(New RmsProp(0.01)).build(), "0").addLayer("2", (New OutputLayer.Builder()).nIn(10).nOut(10).updater(New NoOp()).lossFunction(LossFunctions.LossFunction.MSE).build(), "1").setOutputs("2").build()
			Dim net2 As New ComputationGraph(conf2)
			net2.init()
			net2.Updater.StateViewArray.assign(net.Updater.StateViewArray)
			conf2.setIterationCount(conf.getIterationCount())
			net2.Params = net.params().dup()

			assertEquals(0.1, net.getLearningRate("0").Value, 0.0)
			net.setLearningRate("0", 0.5) 'Set LR for layer 0 to 0.5
			assertEquals(0.5, net.getLearningRate("0").Value, 0.0)

			assertEquals(conf, conf2)
			assertEquals(conf.toJson(), conf2.toJson())

			assertEquals(net.Updater.StateViewArray, net2.Updater.StateViewArray)

			'Perform some parameter updates - check things are actually in sync...
			For i As Integer = 0 To 2
				Dim [in] As INDArray = Nd4j.rand(10, 10)
				Dim l As INDArray = Nd4j.rand(10, 10)

				net.fit(New DataSet([in], l))
				net2.fit(New DataSet([in], l))
			Next i

			assertEquals(net.params(), net2.params())
			assertEquals(net.Updater.StateViewArray, net2.Updater.StateViewArray)

			Dim in1 As INDArray = Nd4j.rand(10, 10)
			Dim l1 As INDArray = Nd4j.rand(10, 10)

			net.Inputs = in1
			net.Labels = l1
			net.computeGradientAndScore()

			net2.Inputs = in1
			net2.Labels = l1
			net2.computeGradientAndScore()

			assertEquals(net.score(), net2.score(), 1e-8)


			'Now: Set *all* LRs to say 0.3...
			Dim conf3 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).updater(New Adam(0.3)).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).updater(New RmsProp(0.3)).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).updater(New NoOp()).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
			Dim net3 As New MultiLayerNetwork(conf3)
			net3.init()
			net3.Updater.StateViewArray.assign(net.Updater.StateViewArray)
			conf3.setIterationCount(conf.getIterationCount())
			net3.Params = net.params().dup()

			net.setLearningRate(0.3)

			'Perform some parameter updates - check things are actually in sync...
			For i As Integer = 0 To 2
				Dim [in] As INDArray = Nd4j.rand(10, 10)
				Dim l As INDArray = Nd4j.rand(10, 10)

				net.fit(New DataSet([in], l))
				net3.fit(New DataSet([in], l))
			Next i

			assertEquals(net.params(), net3.params())
			assertEquals(net.Updater.StateViewArray, net3.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testChangeLrCompGraphSchedule()
		Public Overridable Sub testChangeLrCompGraphSchedule()
			'First: Set LR for a *single* layer and compare vs. equivalent net config
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).updater(New Adam(0.1)).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "0").addLayer("2", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "1").setOutputs("2").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			For i As Integer = 0 To 9
				net.fit(New DataSet(Nd4j.rand(10,10), Nd4j.rand(10,10)))
			Next i


			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).seed(12345).updater(New Adam(New ExponentialSchedule(ScheduleType.ITERATION, 0.5, 0.8))).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "0").layer("2", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "1").setOutputs("2").build()
			Dim net2 As New ComputationGraph(conf2)
			net2.init()
			net2.Updater.StateViewArray.assign(net.Updater.StateViewArray)
			conf2.setIterationCount(conf.getIterationCount())
			net2.Params = net.params().dup()

			net.setLearningRate(New ExponentialSchedule(ScheduleType.ITERATION, 0.5, 0.8)) 'Set LR for layer 0 to 0.5

			assertEquals(conf, conf2)
			assertEquals(conf.toJson(), conf2.toJson())

			assertEquals(net.Updater.StateViewArray, net2.Updater.StateViewArray)

			'Perform some parameter updates - check things are actually in sync...
			For i As Integer = 0 To 2
				Dim [in] As INDArray = Nd4j.rand(10, 10)
				Dim l As INDArray = Nd4j.rand(10, 10)

				net.fit(New DataSet([in], l))
				net2.fit(New DataSet([in], l))
			Next i

			assertEquals(net.params(), net2.params())
			assertEquals(net.Updater.StateViewArray, net2.Updater.StateViewArray)
		End Sub

	End Class

End Namespace