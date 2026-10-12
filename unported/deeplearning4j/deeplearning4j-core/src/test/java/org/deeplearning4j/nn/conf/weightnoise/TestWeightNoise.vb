Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType
Imports SigmoidSchedule = org.nd4j.linalg.schedule.SigmoidSchedule
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.nn.conf.weightnoise


	Public Class TestWeightNoise
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWeightNoiseConfigJson()
		Public Overridable Sub testWeightNoiseConfigJson()
			Dim weightNoises() As IWeightNoise = {
				New DropConnect(0.5),
				New DropConnect(New SigmoidSchedule(ScheduleType.ITERATION, 0.5, 0.5, 100)),
				New WeightNoise(New NormalDistribution(0, 0.1))
			}

			For Each wn As IWeightNoise In weightNoises
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightNoise(wn).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).weightNoise(New DropConnect(0.25)).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				assertEquals(wn, CType(net.getLayer(0).conf().getLayer(), BaseLayer).getWeightNoise())
				assertEquals(New DropConnect(0.25), CType(net.getLayer(1).conf().getLayer(), BaseLayer).getWeightNoise())
				assertEquals(wn, CType(net.getLayer(2).conf().getLayer(), BaseLayer).getWeightNoise())

				TestUtils.testModelSerialization(net)


				Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightNoise(wn).graphBuilder().addInputs("in").layer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").layer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).weightNoise(New DropConnect(0.25)).build(), "0").layer("2", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build(), "1").setOutputs("2").build()

				Dim graph As New ComputationGraph(conf2)
				graph.init()

				assertEquals(wn, CType(graph.getLayer(0).conf().getLayer(), BaseLayer).getWeightNoise())
				assertEquals(New DropConnect(0.25), CType(graph.getLayer(1).conf().getLayer(), BaseLayer).getWeightNoise())
				assertEquals(wn, CType(graph.getLayer(2).conf().getLayer(), BaseLayer).getWeightNoise())

				TestUtils.testModelSerialization(graph)

				graph.fit(New DataSet(Nd4j.create(1,10), Nd4j.create(1,10)))
			Next wn
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCalls()
		Public Overridable Sub testCalls()

			Dim trainData As IList(Of DataSet) = New List(Of DataSet)()
			trainData.Add(New DataSet(Nd4j.rand(5, 10), Nd4j.rand(5, 10)))
			trainData.Add(New DataSet(Nd4j.rand(5, 10), Nd4j.rand(5, 10)))
			trainData.Add(New DataSet(Nd4j.rand(5, 10), Nd4j.rand(5, 10)))

			Dim expCalls As IList(Of IList(Of WeightNoiseCall)) = New List(Of IList(Of WeightNoiseCall))()
			For i As Integer = 0 To 2
				Dim expCallsForLayer As IList(Of WeightNoiseCall) = New List(Of WeightNoiseCall)()
				expCallsForLayer.Add(New WeightNoiseCall(i, "W", 0, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "b", 0, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "W", 1, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "b", 1, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "W", 2, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "b", 2, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "W", 3, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "b", 3, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "W", 4, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "b", 4, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "W", 5, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(i, "b", 5, 1, True))

				'2 test calls
				expCallsForLayer.Add(New WeightNoiseCall(i, "W", 6, 2, False))
				expCallsForLayer.Add(New WeightNoiseCall(i, "b", 6, 2, False))

				expCalls.Add(expCallsForLayer)
			Next i


			Dim wn1 As New CustomWeightNoise()
			Dim wn2 As New CustomWeightNoise()
			Dim wn3 As New CustomWeightNoise()

			Dim list As IList(Of CustomWeightNoise) = New List(Of CustomWeightNoise) From {wn1, wn2, wn3}

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).weightNoise(wn1).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).weightNoise(wn2).build()).layer((New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).weightNoise(wn3).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			net.fit(New ExistingDataSetIterator(trainData.GetEnumerator()))
			net.fit(New ExistingDataSetIterator(trainData.GetEnumerator()))
			net.output(trainData(0).getFeatures())

			For i As Integer = 0 To 2
				assertEquals(expCalls(i), list(i).getAllCalls())
			Next i


			wn1 = New CustomWeightNoise()
			wn2 = New CustomWeightNoise()
			wn3 = New CustomWeightNoise()
			list = New List(Of CustomWeightNoise) From {wn1, wn2, wn3}

			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).weightNoise(wn1).build(), "in").layer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).weightNoise(wn2).build(), "0").layer("2", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).weightNoise(wn3).build(), "1").setOutputs("2").build()

			Dim graph As New ComputationGraph(conf2)
			graph.init()

			Dim layerIdxs() As Integer = {graph.getLayer(0).Index, graph.getLayer(1).Index, graph.getLayer(2).Index}

			expCalls.Clear()
			For i As Integer = 0 To 2
				Dim expCallsForLayer As IList(Of WeightNoiseCall) = New List(Of WeightNoiseCall)()
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "W", 0, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "b", 0, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "W", 1, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "b", 1, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "W", 2, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "b", 2, 0, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "W", 3, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "b", 3, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "W", 4, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "b", 4, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "W", 5, 1, True))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "b", 5, 1, True))

				'2 test calls
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "W", 6, 2, False))
				expCallsForLayer.Add(New WeightNoiseCall(layerIdxs(i), "b", 6, 2, False))

				expCalls.Add(expCallsForLayer)
			Next i

			graph.fit(New ExistingDataSetIterator(trainData.GetEnumerator()))
			graph.fit(New ExistingDataSetIterator(trainData.GetEnumerator()))
			graph.output(trainData(0).getFeatures())

			For i As Integer = 0 To 2
				assertEquals(expCalls(i), list(i).getAllCalls(), i.ToString())
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data private static class CustomWeightNoise implements IWeightNoise
		<Serializable>
		Private Class CustomWeightNoise
			Implements IWeightNoise

			Friend allCalls As IList(Of WeightNoiseCall) = New List(Of WeightNoiseCall)()

			Public Overridable Function getParameter(ByVal layer As Layer, ByVal paramKey As String, ByVal iteration As Integer, ByVal epoch As Integer, ByVal train As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IWeightNoise.getParameter
				allCalls.Add(New WeightNoiseCall(layer.Index, paramKey, iteration, epoch, train))
				Return layer.getParam(paramKey)
			End Function

			Public Overridable Function clone() As IWeightNoise Implements IWeightNoise.clone
				Return New CustomWeightNoise()
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class WeightNoiseCall
		Private Class WeightNoiseCall
			Friend layerIdx As Integer
			Friend paramKey As String
			Friend iter As Integer
			Friend epoch As Integer
			Friend train As Boolean
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDropConnectValues()
		Public Overridable Sub testDropConnectValues()
			Nd4j.Random.setSeed(12345)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.ONES).list().layer((New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim l As Layer = net.getLayer(0)
			Dim d As New DropConnect(0.5)

			Dim outTest As INDArray = d.getParameter(l, "W", 0, 0, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(l.getParam("W") Is outTest) 'Should be same object
			Dim outTrain As INDArray = d.getParameter(l, "W", 0, 0, True, LayerWorkspaceMgr.noWorkspaces())
			assertNotEquals(l.getParam("W"), outTrain)

			assertEquals(l.getParam("W"), Nd4j.ones(DataType.FLOAT, 10, 10))

			Dim countZeros As Integer = Nd4j.Executioner.exec(New MatchCondition(outTrain, Conditions.equals(0))).getInt(0)
			Dim countOnes As Integer = Nd4j.Executioner.exec(New MatchCondition(outTrain, Conditions.equals(1))).getInt(0)

			assertEquals(100, countZeros + countOnes) 'Should only be 0 or 2
			'Stochastic, but this should hold for most cases
			assertTrue(countZeros >= 25 AndAlso countZeros <= 75)
			assertTrue(countOnes >= 25 AndAlso countOnes <= 75)
		End Sub

	End Class

End Namespace