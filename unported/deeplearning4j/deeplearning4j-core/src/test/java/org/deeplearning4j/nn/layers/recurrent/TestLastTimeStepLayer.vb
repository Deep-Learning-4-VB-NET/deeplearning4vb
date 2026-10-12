Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports RnnDataFormat = org.nd4j.enums.RnnDataFormat
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
import static org.deeplearning4j.nn.api.OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT
import static org.deeplearning4j.nn.weights.WeightInit.XAVIER_UNIFORM
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.activations.Activation.IDENTITY
import static org.nd4j.linalg.activations.Activation.TANH
import static org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE

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

Namespace org.deeplearning4j.nn.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestLastTimeStepLayer extends org.deeplearning4j.BaseDL4JTest
	Public Class TestLastTimeStepLayer
		Inherits BaseDL4JTest

		Public Shared Function params() As Stream(Of Arguments)
			Dim args As IList(Of Arguments) = New List(Of Arguments)()
			For Each nd4jBackend As Nd4jBackend In BaseNd4jTestWithBackends.BACKENDS
				For Each rnnFormat As RNNFormat In System.Enum.GetValues(GetType(RNNFormat))
					args.Add(Arguments.of(rnnFormat,nd4jBackend))
				Next rnnFormat
			Next nd4jBackend
			Return args.stream()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestLastTimeStepLayer#params") public void testLastTimeStepVertex(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLastTimeStepVertex(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("lastTS", New LastTimeStep((New SimpleRnn.Builder()).nIn(5).nOut(6).dataFormat(rnnDataFormat).build()), "in").setOutputs("lastTS").build()

			Dim graph As New ComputationGraph(conf)
			graph.init()

			'First: test without input mask array
			Nd4j.Random.setSeed(12345)
			Dim l As Layer = graph.getLayer("lastTS")
			Dim [in] As INDArray
			If rnnDataFormat = RNNFormat.NCW Then
				[in] = Nd4j.rand(3, 5, 6)
			Else
				[in] = Nd4j.rand(3, 6, 5)
			End If
			Dim outUnderlying As INDArray = DirectCast(l, LastTimeStepLayer).getUnderlying().activate([in], False, LayerWorkspaceMgr.noWorkspaces())
			Dim expOut As INDArray
			If rnnDataFormat = RNNFormat.NCW Then
				expOut = outUnderlying.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(5))
			Else
				expOut = outUnderlying.get(NDArrayIndex.all(), NDArrayIndex.point(5), NDArrayIndex.all())
			End If



			'Forward pass:
			Dim outFwd As INDArray = l.activate([in], False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(expOut, outFwd)

			'Second: test with input mask array
			Dim inMask As INDArray = Nd4j.zeros(3, 6)
			inMask.putRow(0, Nd4j.create(New Double(){1, 1, 1, 0, 0, 0}))
			inMask.putRow(1, Nd4j.create(New Double(){1, 1, 1, 1, 0, 0}))
			inMask.putRow(2, Nd4j.create(New Double(){1, 1, 1, 1, 1, 0}))
			graph.setLayerMaskArrays(New INDArray(){inMask}, Nothing)

			expOut = Nd4j.zeros(3, 6)
			If rnnDataFormat = RNNFormat.NCW Then
				expOut.putRow(0, outUnderlying.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(2)))
				expOut.putRow(1, outUnderlying.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.point(3)))
				expOut.putRow(2, outUnderlying.get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.point(4)))
			Else
				expOut.putRow(0, outUnderlying.get(NDArrayIndex.point(0), NDArrayIndex.point(2), NDArrayIndex.all()))
				expOut.putRow(1, outUnderlying.get(NDArrayIndex.point(1), NDArrayIndex.point(3), NDArrayIndex.all()))
				expOut.putRow(2, outUnderlying.get(NDArrayIndex.point(2), NDArrayIndex.point(4), NDArrayIndex.all()))
			End If


			outFwd = l.activate([in], False, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(expOut, outFwd)

			TestUtils.testModelSerialization(graph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestLastTimeStepLayer#params") public void testMaskingAndAllMasked(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMaskingAndAllMasked(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			Dim builder As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).optimizationAlgo(STOCHASTIC_GRADIENT_DESCENT).weightInit(XAVIER_UNIFORM).activation(TANH).updater(New AdaGrad(0.01)).l2(0.0001).seed(1234).graphBuilder().addInputs("in").setInputTypes(InputType.recurrent(1, rnnDataFormat)).addLayer("RNN", New LastTimeStep((New LSTM.Builder()).nOut(10).dataFormat(rnnDataFormat).build()), "in").addLayer("dense", (New DenseLayer.Builder()).nOut(10).build(), "RNN").addLayer("out", (New OutputLayer.Builder()).activation(IDENTITY).lossFunction(MSE).nOut(10).build(), "dense").setOutputs("out")

			Dim conf As ComputationGraphConfiguration = builder.build()
			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim f As INDArray = Nd4j.rand(New Long(){1, 1, 24})
			Dim fm1 As INDArray = Nd4j.ones(1,24)
			Dim fm2 As INDArray = Nd4j.zeros(1,24)
			Dim fm3 As INDArray = Nd4j.zeros(1,24)
			fm3.get(NDArrayIndex.point(0), NDArrayIndex.interval(0,5)).assign(1)
			If rnnDataFormat = RNNFormat.NWC Then
				f = f.permute(0, 2, 1)
			End If
			Dim out1() As INDArray = cg.output(False, New INDArray(){f}, New INDArray(){fm1})
			Try
				cg.output(False, New INDArray(){f}, New INDArray(){fm2})
				fail("Expected exception")
			Catch e As Exception
				assertTrue(e.Message.contains("mask is all 0s"))
			End Try

			Dim out3() As INDArray = cg.output(False, New INDArray(){f}, New INDArray(){fm3})

			Console.WriteLine(out1(0))
			Console.WriteLine(out3(0))

			assertNotEquals(out1(0), out3(0))
		End Sub
	End Class

End Namespace