Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.interval
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestSimpleRnn extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSimpleRnn
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestRnnLayers#params") public void testSimpleRnn(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat, org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSimpleRnn(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim m As Integer = 3
			Dim nIn As Integer = 5
			Dim layerSize As Integer = 6
			Dim tsLength As Integer = 7
			Dim [in] As INDArray
			If rnnDataFormat = RNNFormat.NCW Then
				[in] = Nd4j.rand(DataType.FLOAT, m, nIn, tsLength)
			Else
				[in] = Nd4j.rand(DataType.FLOAT, m, tsLength, nIn)
			End If


			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).list().layer((New SimpleRnn.Builder()).nIn(nIn).nOut(layerSize).dataFormat(rnnDataFormat).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim [out] As INDArray = net.output([in])

			Dim w As INDArray = net.getParam("0_W")
			Dim rw As INDArray = net.getParam("0_RW")
			Dim b As INDArray = net.getParam("0_b")

			Dim outLast As INDArray = Nothing
			For i As Integer = 0 To tsLength - 1
				Dim inCurrent As INDArray
				If rnnDataFormat = RNNFormat.NCW Then
					inCurrent = [in].get(all(), all(), point(i))
				Else
					inCurrent = [in].get(all(), point(i), all())
				End If

				Dim outExpCurrent As INDArray = inCurrent.mmul(w)
				If outLast IsNot Nothing Then
					outExpCurrent.addi(outLast.mmul(rw))
				End If

				outExpCurrent.addiRowVector(b)

				Transforms.tanh(outExpCurrent, False)

				Dim outActCurrent As INDArray
				If rnnDataFormat = RNNFormat.NCW Then
					outActCurrent = [out].get(all(), all(), point(i))
				Else
					outActCurrent = [out].get(all(), point(i), all())
				End If
				assertEquals(outExpCurrent, outActCurrent, i.ToString())

				outLast = outExpCurrent
			Next i


			TestUtils.testModelSerialization(net)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestRnnLayers#params") public void testBiasInit(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBiasInit(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim nIn As Integer = 5
			Dim layerSize As Integer = 6

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).weightInit(WeightInit.XAVIER).activation(Activation.TANH).list().layer((New SimpleRnn.Builder()).nIn(nIn).nOut(layerSize).dataFormat(rnnDataFormat).biasInit(100).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim bArr As INDArray = net.getParam("0_b")
			assertEquals(Nd4j.valueArrayOf(New Long(){1, layerSize}, 100.0f), bArr)
		End Sub
	End Class

End Namespace