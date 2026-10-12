Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports PreprocessorVertex = org.deeplearning4j.nn.conf.graph.PreprocessorVertex
Imports DuplicateToTimeSeriesVertex = org.deeplearning4j.nn.conf.graph.rnn.DuplicateToTimeSeriesVertex
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports org.deeplearning4j.nn.graph.vertex.impl
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.nn.graph.graphnodes


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestGraphNodes extends org.deeplearning4j.BaseDL4JTest
	Public Class TestGraphNodes
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMergeNode()
		Public Overridable Sub testMergeNode()
			Nd4j.Random.setSeed(12345)
			Dim mergeNode As GraphVertex = New MergeVertex(Nothing, "", -1, Nd4j.dataType(), 1)

			Dim first As INDArray = Nd4j.linspace(0, 11, 12, Nd4j.dataType()).reshape(ChrW(3), 4)
			Dim second As INDArray = Nd4j.linspace(0, 17, 18, Nd4j.dataType()).reshape(ChrW(3), 6).addi(100)

			mergeNode.setInputs(first, second)
			Dim [out] As INDArray = mergeNode.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(New Long() {3, 10}, [out].shape())

			assertEquals(first, [out].get(NDArrayIndex.all(), NDArrayIndex.interval(0, 4)))
			assertEquals(second, [out].get(NDArrayIndex.all(), NDArrayIndex.interval(4, 10)))

			mergeNode.Epsilon = [out]
			Dim backward() As INDArray = mergeNode.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second
			assertEquals(first, backward(0))
			assertEquals(second, backward(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMergeNodeRNN()
		Public Overridable Sub testMergeNodeRNN()

			Nd4j.Random.setSeed(12345)
			Dim mergeNode As GraphVertex = New MergeVertex(Nothing, "", -1, Nd4j.dataType(), 1)

			Dim first As INDArray = Nd4j.linspace(0, 59, 60, Nd4j.dataType()).reshape(ChrW(3), 4, 5)
			Dim second As INDArray = Nd4j.linspace(0, 89, 90, Nd4j.dataType()).reshape(ChrW(3), 6, 5).addi(100)

			mergeNode.setInputs(first, second)
			Dim [out] As INDArray = mergeNode.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(New Long() {3, 10, 5}, [out].shape())

			assertEquals(first, [out].get(NDArrayIndex.all(), NDArrayIndex.interval(0, 4), NDArrayIndex.all()))
			assertEquals(second, [out].get(NDArrayIndex.all(), NDArrayIndex.interval(4, 10), NDArrayIndex.all()))

			mergeNode.Epsilon = [out]
			Dim backward() As INDArray = mergeNode.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second
			assertEquals(first, backward(0))
			assertEquals(second, backward(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnDepthMerge()
		Public Overridable Sub testCnnDepthMerge()
			Nd4j.Random.setSeed(12345)
			Dim mergeNode As GraphVertex = New MergeVertex(Nothing, "", -1, Nd4j.dataType(), 1)

			Dim first As INDArray = Nd4j.linspace(0, 3, 4, Nd4j.dataType()).reshape(ChrW(1), 1, 2, 2)
			Dim second As INDArray = Nd4j.linspace(0, 3, 4, Nd4j.dataType()).reshape(ChrW(1), 1, 2, 2).addi(10)

			mergeNode.setInputs(first, second)
			Dim [out] As INDArray = mergeNode.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(New Long() {1, 2, 2, 2}, [out].shape())

			For i As Integer = 0 To 1
				For j As Integer = 0 To 1
					assertEquals(first.getDouble(0, 0, i, j), [out].getDouble(0, 0, i, j), 1e-6)
					assertEquals(second.getDouble(0, 0, i, j), [out].getDouble(0, 1, i, j), 1e-6)
				Next j
			Next i

			mergeNode.Epsilon = [out]
			Dim backward() As INDArray = mergeNode.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second
			assertEquals(first, backward(0))
			assertEquals(second, backward(1))


			'Slightly more complicated test:
			first = Nd4j.linspace(0, 17, 18, Nd4j.dataType()).reshape(ChrW(1), 2, 3, 3)
			second = Nd4j.linspace(0, 17, 18, Nd4j.dataType()).reshape(ChrW(1), 2, 3, 3).addi(100)

			mergeNode.setInputs(first, second)
			[out] = mergeNode.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertArrayEquals(New Long() {1, 4, 3, 3}, [out].shape())

			For i As Integer = 0 To 2
				For j As Integer = 0 To 2
					assertEquals(first.getDouble(0, 0, i, j), [out].getDouble(0, 0, i, j), 1e-6)
					assertEquals(first.getDouble(0, 1, i, j), [out].getDouble(0, 1, i, j), 1e-6)

					assertEquals(second.getDouble(0, 0, i, j), [out].getDouble(0, 2, i, j), 1e-6)
					assertEquals(second.getDouble(0, 1, i, j), [out].getDouble(0, 3, i, j), 1e-6)
				Next j
			Next i

			mergeNode.Epsilon = [out]
			backward = mergeNode.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second
			assertEquals(first, backward(0))
			assertEquals(second, backward(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSubsetNode()
		Public Overridable Sub testSubsetNode()
			Nd4j.Random.setSeed(12345)
			Dim subset As GraphVertex = New SubsetVertex(Nothing, "", -1, 4, 7, Nd4j.dataType())

			Dim [in] As INDArray = Nd4j.rand(5, 10)
			subset.Inputs = [in]
			Dim [out] As INDArray = subset.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals([in].get(NDArrayIndex.all(), NDArrayIndex.interval(4, 7, True)), [out])

			subset.Epsilon = [out]
			Dim backward As INDArray = subset.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			assertEquals(Nd4j.zeros(5, 4), backward.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 3, True)))
			assertEquals([out], backward.get(NDArrayIndex.all(), NDArrayIndex.interval(4, 7, True)))
			assertEquals(Nd4j.zeros(5, 2), backward.get(NDArrayIndex.all(), NDArrayIndex.interval(8, 9, True)))

			'Test same for CNNs:
			[in] = Nd4j.rand(New Integer() {5, 10, 3, 3})
			subset.Inputs = [in]
			[out] = subset.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals([in].get(NDArrayIndex.all(), NDArrayIndex.interval(4, 7, True), NDArrayIndex.all(), NDArrayIndex.all()), [out])

			subset.Epsilon = [out]
			backward = subset.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			assertEquals(Nd4j.zeros(5, 4, 3, 3), backward.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 3, True), NDArrayIndex.all(), NDArrayIndex.all()))
			assertEquals([out], backward.get(NDArrayIndex.all(), NDArrayIndex.interval(4, 7, True), NDArrayIndex.all(), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 2, 3, 3), backward.get(NDArrayIndex.all(), NDArrayIndex.interval(8, 9, True), NDArrayIndex.all(), NDArrayIndex.all()))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLastTimeStepVertex()
		Public Overridable Sub testLastTimeStepVertex()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addVertex("lastTS", New LastTimeStepVertex("in"), "in").addLayer("out", (New OutputLayer.Builder()).nIn(1).nOut(1).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build(), "lastTS").setOutputs("out").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			'First: test without input mask array
			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.rand(New Integer() {3, 5, 6})
			Dim expOut As INDArray = [in].get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(5))

			Dim gv As GraphVertex = graph.getVertex("lastTS")
			gv.Inputs = [in]
			'Forward pass:
			Dim outFwd As INDArray = gv.doForward(True, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(expOut, outFwd)
			'Backward pass:
			gv.Epsilon = expOut
			Dim pair As Pair(Of Gradient, INDArray()) = gv.doBackward(False, LayerWorkspaceMgr.noWorkspaces())
			Dim eps As INDArray = pair.Second(0)
			assertArrayEquals([in].shape(), eps.shape())
			assertEquals(Nd4j.zeros(3, 5, 5), eps.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 4, True)))
			assertEquals(expOut, eps.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(5)))

			'Second: test with input mask array
			Dim inMask As INDArray = Nd4j.zeros(3, 6)
			inMask.putRow(0, Nd4j.create(New Double() {1, 1, 1, 0, 0, 0}))
			inMask.putRow(1, Nd4j.create(New Double() {1, 1, 1, 1, 0, 0}))
			inMask.putRow(2, Nd4j.create(New Double() {1, 1, 1, 1, 1, 0}))
			graph.setLayerMaskArrays(New INDArray() {inMask}, Nothing)

			expOut = Nd4j.zeros(3, 5)
			expOut.putRow(0, [in].get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(2)))
			expOut.putRow(1, [in].get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.point(3)))
			expOut.putRow(2, [in].get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.point(4)))

			gv.Inputs = [in]
			outFwd = gv.doForward(True, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(expOut, outFwd)

			Dim json As String = conf.toJson()
			Dim conf2 As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(conf, conf2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDuplicateToTimeSeriesVertex()
		Public Overridable Sub testDuplicateToTimeSeriesVertex()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in2d", "in3d").addVertex("duplicateTS", New DuplicateToTimeSeriesVertex("in3d"), "in2d").addLayer("out", (New OutputLayer.Builder()).nIn(1).nOut(1).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build(), "duplicateTS").addLayer("out3d", (New RnnOutputLayer.Builder()).nIn(1).nOut(1).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build(), "in3d").setOutputs("out", "out3d").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			Dim in2d As INDArray = Nd4j.rand(3, 5)
			Dim in3d As INDArray = Nd4j.rand(New Integer() {3, 2, 7})

			graph.setInputs(in2d, in3d)

			Dim expOut As INDArray = Nd4j.zeros(3, 5, 7)
			For i As Integer = 0 To 6
				expOut.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(i)}, in2d)
			Next i

			Dim gv As GraphVertex = graph.getVertex("duplicateTS")
			gv.Inputs = in2d
			Dim outFwd As INDArray = gv.doForward(True, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(expOut, outFwd)

			Dim expOutBackward As INDArray = expOut.sum(2)
			gv.Epsilon = expOut
			Dim outBwd As INDArray = gv.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			assertEquals(expOutBackward, outBwd)

			Dim json As String = conf.toJson()
			Dim conf2 As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(conf, conf2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStackNode()
		Public Overridable Sub testStackNode()
			Nd4j.Random.setSeed(12345)
			Dim unstack As GraphVertex = New StackVertex(Nothing, "", -1, Nd4j.dataType())

			Dim in1 As INDArray = Nd4j.rand(5, 2)
			Dim in2 As INDArray = Nd4j.rand(5, 2)
			Dim in3 As INDArray = Nd4j.rand(5, 2)
			unstack.setInputs(in1, in2, in3)
			Dim [out] As INDArray = unstack.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(in1, [out].get(NDArrayIndex.interval(0, 5), NDArrayIndex.all()))
			assertEquals(in2, [out].get(NDArrayIndex.interval(5, 10), NDArrayIndex.all()))
			assertEquals(in3, [out].get(NDArrayIndex.interval(10, 15), NDArrayIndex.all()))

			unstack.Epsilon = [out]
			Dim b As Pair(Of Gradient, INDArray()) = unstack.doBackward(False, LayerWorkspaceMgr.noWorkspaces())

			assertEquals(in1, b.Second(0))
			assertEquals(in2, b.Second(1))
			assertEquals(in3, b.Second(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStackVertexEmbedding()
		Public Overridable Sub testStackVertexEmbedding()
			Nd4j.Random.setSeed(12345)
			Dim unstack As GraphVertex = New StackVertex(Nothing, "", -1, Nd4j.dataType())

			Dim in1 As INDArray = Nd4j.zeros(5, 1)
			Dim in2 As INDArray = Nd4j.zeros(5, 1)
			For i As Integer = 0 To 4
				in1.putScalar(i, 0, i)
				in2.putScalar(i, 0, i)
			Next i

			Dim l As INDArray = Nd4j.rand(5, 5)
			Dim ds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(New INDArray() {in1, in2}, New INDArray() {l, l}, Nothing, Nothing)


			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in1", "in2").addVertex("stack", New org.deeplearning4j.nn.conf.graph.StackVertex(), "in1", "in2").addLayer("1", (New EmbeddingLayer.Builder()).nIn(5).nOut(5).build(), "stack").addVertex("unstack1", New org.deeplearning4j.nn.conf.graph.UnstackVertex(0, 2), "1").addVertex("unstack2", New org.deeplearning4j.nn.conf.graph.UnstackVertex(0, 2), "1").addLayer("out1", (New OutputLayer.Builder()).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.L2).nIn(5).nOut(5).build(), "unstack1").addLayer("out2", (New OutputLayer.Builder()).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.L2).nIn(5).nOut(5).build(), "unstack2").setOutputs("out1", "out2").build()

			Dim g As New ComputationGraph(conf)
			g.init()

			g.feedForward(New INDArray() {in1, in2}, False)

			g.fit(ds)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStackUnstackNodeVariableLength()
		Public Overridable Sub testStackUnstackNodeVariableLength()
			Nd4j.Random.setSeed(12345)
			Dim stack As GraphVertex = New StackVertex(Nothing, "", -1, Nd4j.dataType())

			'Test stack with variable length + mask arrays
			Dim in0 As INDArray = Nd4j.rand(New Integer() {5, 2, 5})
			Dim in1 As INDArray = Nd4j.rand(New Integer() {5, 2, 6})
			Dim in2 As INDArray = Nd4j.rand(New Integer() {5, 2, 7})

			Dim mask0 As INDArray = Nd4j.ones(5, 5)
			Dim mask1 As INDArray = Nd4j.ones(5, 6)
			Dim mask2 As INDArray = Nd4j.ones(5, 7)

			stack.setInputs(in0, in1, in2)
			Dim p As Pair(Of INDArray, MaskState) = stack.feedForwardMaskArrays(New INDArray() {mask0, mask1, mask2}, MaskState.Active, 5)
			assertArrayEquals(New Long() {15, 7}, p.First.shape())
			assertEquals(MaskState.Active, p.Second)

			Dim [out] As INDArray = stack.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(in0, [out].get(NDArrayIndex.interval(0, 5), NDArrayIndex.all(), NDArrayIndex.interval(0, 5)))
			assertEquals(in1, [out].get(NDArrayIndex.interval(5, 10), NDArrayIndex.all(), NDArrayIndex.interval(0, 6)))
			assertEquals(in2, [out].get(NDArrayIndex.interval(10, 15), NDArrayIndex.all(), NDArrayIndex.interval(0, 7)))

			stack.Epsilon = [out]
			Dim b As Pair(Of Gradient, INDArray()) = stack.doBackward(False, LayerWorkspaceMgr.noWorkspaces())

			assertEquals(in0, b.Second(0))
			assertEquals(in1, b.Second(1))
			assertEquals(in2, b.Second(2))

			'Test unstack with variable length + mask arrays
			'Note that we don't actually need changes here - unstack has a single input, and the unstacked mask
			'might be a bit longer than we really need, but it'll still be correct
			Dim unstack0 As GraphVertex = New UnstackVertex(Nothing, "u0", 0, 0, 3, Nd4j.dataType())
			Dim unstack1 As GraphVertex = New UnstackVertex(Nothing, "u1", 0, 1, 3, Nd4j.dataType())
			Dim unstack2 As GraphVertex = New UnstackVertex(Nothing, "u2", 0, 2, 3, Nd4j.dataType())

			unstack0.Inputs = [out]
			unstack1.Inputs = [out]
			unstack2.Inputs = [out]
			Dim f0 As INDArray = unstack0.doForward(True, LayerWorkspaceMgr.noWorkspaces())
			Dim f1 As INDArray = unstack1.doForward(True, LayerWorkspaceMgr.noWorkspaces())
			Dim f2 As INDArray = unstack2.doForward(True, LayerWorkspaceMgr.noWorkspaces())

			assertEquals(in0, f0.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 5)))
			assertEquals(in1, f1.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 6)))
			assertEquals(in2, f2.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(0, 7)))

			Dim p0 As Pair(Of INDArray, MaskState) = unstack0.feedForwardMaskArrays(New INDArray() {p.First}, MaskState.Active, 5)
			Dim p1 As Pair(Of INDArray, MaskState) = unstack1.feedForwardMaskArrays(New INDArray() {p.First}, MaskState.Active, 5)
			Dim p2 As Pair(Of INDArray, MaskState) = unstack2.feedForwardMaskArrays(New INDArray() {p.First}, MaskState.Active, 5)

			assertEquals(mask0, p0.First.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 5)))
			assertEquals(mask1, p1.First.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 6)))
			assertEquals(mask2, p2.First.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 7)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUnstackNode()
		Public Overridable Sub testUnstackNode()
			Nd4j.Random.setSeed(12345)
			Dim unstack0 As GraphVertex = New UnstackVertex(Nothing, "", -1, 0, 3, Nd4j.dataType())
			Dim unstack1 As GraphVertex = New UnstackVertex(Nothing, "", -1, 1, 3, Nd4j.dataType())
			Dim unstack2 As GraphVertex = New UnstackVertex(Nothing, "", -1, 2, 3, Nd4j.dataType())

			Dim [in] As INDArray = Nd4j.rand(15, 2)
			unstack0.Inputs = [in]
			unstack1.Inputs = [in]
			unstack2.Inputs = [in]
			Dim out0 As INDArray = unstack0.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			Dim out1 As INDArray = unstack1.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			Dim out2 As INDArray = unstack2.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals([in].get(NDArrayIndex.interval(0, 5), NDArrayIndex.all()), out0)
			assertEquals([in].get(NDArrayIndex.interval(5, 10), NDArrayIndex.all()), out1)
			assertEquals([in].get(NDArrayIndex.interval(10, 15), NDArrayIndex.all()), out2)

			unstack0.Epsilon = out0
			unstack1.Epsilon = out1
			unstack2.Epsilon = out2
			Dim backward0 As INDArray = unstack0.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			Dim backward1 As INDArray = unstack1.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			Dim backward2 As INDArray = unstack2.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			assertEquals(out0, backward0.get(NDArrayIndex.interval(0, 5), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 2), backward0.get(NDArrayIndex.interval(5, 10), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 2), backward0.get(NDArrayIndex.interval(10, 15), NDArrayIndex.all()))

			assertEquals(Nd4j.zeros(5, 2), backward1.get(NDArrayIndex.interval(0, 5), NDArrayIndex.all()))
			assertEquals(out1, backward1.get(NDArrayIndex.interval(5, 10), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 2), backward1.get(NDArrayIndex.interval(10, 15), NDArrayIndex.all()))

			assertEquals(Nd4j.zeros(5, 2), backward2.get(NDArrayIndex.interval(0, 5), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 2), backward2.get(NDArrayIndex.interval(5, 10), NDArrayIndex.all()))
			assertEquals(out2, backward2.get(NDArrayIndex.interval(10, 15), NDArrayIndex.all()))



			'Test same for CNNs:
			[in] = Nd4j.rand(New Integer() {15, 10, 3, 3})
			unstack0.Inputs = [in]
			unstack1.Inputs = [in]
			unstack2.Inputs = [in]
			out0 = unstack0.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			out1 = unstack1.doForward(False, LayerWorkspaceMgr.noWorkspaces())
			out2 = unstack2.doForward(False, LayerWorkspaceMgr.noWorkspaces())

			assertEquals([in].get(NDArrayIndex.interval(0, 5), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()), out0)
			assertEquals([in].get(NDArrayIndex.interval(5, 10), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()), out1)
			assertEquals([in].get(NDArrayIndex.interval(10, 15), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()), out2)

			unstack0.Epsilon = out0
			unstack1.Epsilon = out1
			unstack2.Epsilon = out2
			backward0 = unstack0.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			backward1 = unstack1.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			backward2 = unstack2.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second(0)
			assertEquals(out0, backward0.get(NDArrayIndex.interval(0, 5), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 10, 3, 3), backward0.get(NDArrayIndex.interval(5, 10), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 10, 3, 3), backward0.get(NDArrayIndex.interval(10, 15), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))

			assertEquals(Nd4j.zeros(5, 10, 3, 3), backward1.get(NDArrayIndex.interval(0, 5), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))
			assertEquals(out1, backward1.get(NDArrayIndex.interval(5, 10), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 10, 3, 3), backward1.get(NDArrayIndex.interval(10, 15), NDArrayIndex.all()))

			assertEquals(Nd4j.zeros(5, 10, 3, 3), backward2.get(NDArrayIndex.interval(0, 5), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))
			assertEquals(Nd4j.zeros(5, 10, 3, 3), backward2.get(NDArrayIndex.interval(5, 10), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))
			assertEquals(out2, backward2.get(NDArrayIndex.interval(10, 15), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testL2Node()
		Public Overridable Sub testL2Node()
			Nd4j.Random.setSeed(12345)
			Dim l2 As GraphVertex = New L2Vertex(Nothing, "", -1, 1e-8, Nd4j.dataType())

			Dim in1 As INDArray = Nd4j.rand(5, 2)
			Dim in2 As INDArray = Nd4j.rand(5, 2)

			l2.setInputs(in1, in2)
			Dim [out] As INDArray = l2.doForward(False, LayerWorkspaceMgr.noWorkspaces())

			Dim expOut As INDArray = Nd4j.create(5, 1)
			For i As Integer = 0 To 4
				Dim d2 As Double = 0.0
				Dim j As Integer = 0
				Do While j < in1.size(1)
					Dim temp As Double = (in1.getDouble(i, j) - in2.getDouble(i, j))
					d2 += temp * temp
					j += 1
				Loop
				d2 = Math.Sqrt(d2)
				expOut.putScalar(i, 0, d2)
			Next i

			assertEquals(expOut, [out])



			Dim epsilon As INDArray = Nd4j.rand(5, 1) 'dL/dlambda
			Dim diff As INDArray = in1.sub(in2)
			'Out == sqrt(s) = s^1/2. Therefore: s^(-1/2) = 1/out
			Dim sNegHalf As INDArray = [out].rdiv(1.0)

			Dim dLda As INDArray = diff.mulColumnVector(epsilon.mul(sNegHalf))
			Dim dLdb As INDArray = diff.mulColumnVector(epsilon.mul(sNegHalf)).neg()



			l2.Epsilon = epsilon
			Dim p As Pair(Of Gradient, INDArray()) = l2.doBackward(False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(dLda, p.Second(0))
			assertEquals(dLdb, p.Second(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReshapeNode()
		Public Overridable Sub testReshapeNode()
			Nd4j.Random.setSeed(12345)
			Dim reshapeVertex As GraphVertex = New ReshapeVertex(Nothing, "", -1, "c"c, New Integer() {-1, 736}, Nothing, Nd4j.dataType())

			Dim inputShape As val = New Long() {1, 1, 1, 736}
			Dim input As INDArray = Nd4j.create(inputShape)

			reshapeVertex.Inputs = input
			Dim [out] As INDArray = reshapeVertex.doForward(False, LayerWorkspaceMgr.noWorkspaces())

			assertArrayEquals(New Long() {1, 736}, [out].shape())

			reshapeVertex.Epsilon = [out]
			Dim backward() As INDArray = reshapeVertex.doBackward(False, LayerWorkspaceMgr.noWorkspaces()).Second
			assertTrue(backward(0).shape().SequenceEqual(inputShape))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJSON()
		Public Overridable Sub testJSON()
			'The config here is non-sense, but that doesn't matter for config -> json -> config test
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addVertex("v1", New ElementWiseVertex(ElementWiseVertex.Op.Add), "in").addVertex("v2", New org.deeplearning4j.nn.conf.graph.MergeVertex(), "in", "in").addVertex("v3", New PreprocessorVertex(New CnnToFeedForwardPreProcessor(1, 2, 1)), "in").addVertex("v4", New org.deeplearning4j.nn.conf.graph.SubsetVertex(0, 1), "in").addVertex("v5", New DuplicateToTimeSeriesVertex("in"), "in").addVertex("v6", New LastTimeStepVertex("in"), "in").addVertex("v7", New org.deeplearning4j.nn.conf.graph.StackVertex(), "in").addVertex("v8", New org.deeplearning4j.nn.conf.graph.UnstackVertex(0, 1), "in").addLayer("out", (New OutputLayer.Builder()).nIn(1).nOut(1).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build(), "in").setOutputs("out", "v1", "v2", "v3", "v4", "v5", "v6", "v7", "v8").build()

			Dim json As String = conf.toJson()
			Dim conf2 As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(conf, conf2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLastTimeStepWithTransfer()
		Public Overridable Sub testLastTimeStepWithTransfer()
			Dim lstmLayerSize As Integer = 16
			Dim numLabelClasses As Integer = 10
			Dim numInputs As Integer = 5

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(WorkspaceMode.NONE).inferenceWorkspaceMode(WorkspaceMode.NONE).seed(123).updater(New AdaDelta()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("rr").addLayer("1", (New LSTM.Builder()).activation(Activation.TANH).nIn(numInputs).nOut(lstmLayerSize).dropOut(0.9).build(), "rr").addLayer("2", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(numLabelClasses).build(), "1").setOutputs("2").setInputTypes(InputType.recurrent(numInputs,16, RNNFormat.NCW)).build()


			Dim net As New ComputationGraph(conf)
			net.init()

			Dim updatedModel As ComputationGraph = (New TransferLearning.GraphBuilder(net)).addVertex("laststepoutput", New LastTimeStepVertex("rr"), "2").setOutputs("laststepoutput").build()


			Dim input As INDArray = Nd4j.rand(New Integer(){10, numInputs, 16})

			Dim [out]() As INDArray = updatedModel.output(input)

			assertNotNull([out])
			assertEquals(1, [out].Length)
			assertNotNull([out](0))

			assertArrayEquals(New Long(){10, numLabelClasses}, [out](0).shape())

			Dim acts As IDictionary(Of String, INDArray) = updatedModel.feedForward(input, False)

			assertEquals(4, acts.Count) '2 layers + input + vertex output
			assertNotNull(acts("laststepoutput"))
			assertArrayEquals(New Long(){10, numLabelClasses}, acts("laststepoutput").shape())

			Dim toString As String = [out](0).ToString()
		End Sub
	End Class

End Namespace