Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports BaseRecurrentLayer = org.deeplearning4j.nn.conf.layers.BaseRecurrentLayer
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports TimeDistributed = org.deeplearning4j.nn.conf.layers.recurrent.TimeDistributed
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
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
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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

Namespace org.deeplearning4j.nn.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestTimeDistributed extends org.deeplearning4j.BaseDL4JTest
	Public Class TestTimeDistributed
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestTimeDistributed#params") public void testTimeDistributed(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTimeDistributed(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.ENABLED, WorkspaceMode.NONE}

				Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).seed(12345).updater(New Adam(0.1)).list().layer((New LSTM.Builder()).nIn(3).nOut(3).dataFormat(rnnDataFormat).build()).layer((New DenseLayer.Builder()).nIn(3).nOut(3).activation(Activation.TANH).build()).layer((New RnnOutputLayer.Builder()).nIn(3).nOut(3).activation(Activation.SOFTMAX).dataFormat(rnnDataFormat).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(3, rnnDataFormat)).build()

				Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).seed(12345).updater(New Adam(0.1)).list().layer((New LSTM.Builder()).nIn(3).nOut(3).dataFormat(rnnDataFormat).build()).layer(New TimeDistributed((New DenseLayer.Builder()).nIn(3).nOut(3).activation(Activation.TANH).build(), rnnDataFormat)).layer((New RnnOutputLayer.Builder()).nIn(3).nOut(3).activation(Activation.SOFTMAX).dataFormat(rnnDataFormat).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(3, rnnDataFormat)).build()

				Dim net1 As New MultiLayerNetwork(conf1)
				Dim net2 As New MultiLayerNetwork(conf2)
				net1.init()
				net2.init()

				For Each mb As Integer In New Integer(){1, 5}
					For Each inLabelOrder As Char In New Char(){"c"c, "f"c}
						Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, mb, 3, 5).dup(inLabelOrder)
						If rnnDataFormat = RNNFormat.NWC Then
							[in] = [in].permute(0, 2, 1)
						End If
						Dim out1 As INDArray = net1.output([in])
						Dim out2 As INDArray = net2.output([in])
						assertEquals(out1, out2)

						Dim labels As INDArray
						If rnnDataFormat = RNNFormat.NCW Then
							labels = TestUtils.randomOneHotTimeSeries(mb, 3, 5).dup(inLabelOrder)
						Else
							labels = TestUtils.randomOneHotTimeSeries(mb, 5, 3).dup(inLabelOrder)
						End If



						Dim ds As New DataSet([in], labels)
						net1.fit(ds)
						net2.fit(ds)

						assertEquals(net1.params(), net2.params())

						Dim net3 As MultiLayerNetwork = TestUtils.testModelSerialization(net2)
						out2 = net2.output([in])
						Dim out3 As INDArray = net3.output([in])

						assertEquals(out2, out3)
					Next inLabelOrder
				Next mb
			Next wsm
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestTimeDistributed#params") @ParameterizedTest public void testTimeDistributedDense(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTimeDistributedDense(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)

			For rnnType As Integer = 0 To 2
				For ffType As Integer = 0 To 2

					Dim l0, l2 As Layer
					Select Case rnnType
						Case 0
							l0 = (New LSTM.Builder()).nOut(5).build()
							l2 = (New LSTM.Builder()).nOut(5).build()
						Case 1
							l0 = (New SimpleRnn.Builder()).nOut(5).build()
							l2 = (New SimpleRnn.Builder()).nOut(5).build()
						Case 2
							l0 = New Bidirectional((New LSTM.Builder()).nOut(5).build())
							l2 = New Bidirectional((New LSTM.Builder()).nOut(5).build())
						Case Else
							Throw New Exception("Not implemented: " & rnnType)
					End Select

					Dim l1 As Layer
					Select Case ffType
						Case 0
							l1 = (New DenseLayer.Builder()).nOut(5).build()
						Case 1
							l1 = (New VariationalAutoencoder.Builder()).nOut(5).encoderLayerSizes(5).decoderLayerSizes(5).build()
						Case 2
							l1 = (New AutoEncoder.Builder()).nOut(5).build()
						Case Else
							Throw New Exception("Not implemented: " & ffType)
					End Select

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer(l0).layer(l1).layer(l2).setInputType(InputType.recurrent(5, 9, rnnDataFormat)).build()

					Dim l0a As BaseRecurrentLayer
					Dim l2a As BaseRecurrentLayer
					If rnnType < 2 Then
						l0a = DirectCast(l0, BaseRecurrentLayer)
						l2a = DirectCast(l2, BaseRecurrentLayer)
					Else
						l0a = CType(DirectCast(l0, Bidirectional).getFwd(), BaseRecurrentLayer)
						l2a = CType(DirectCast(l2, Bidirectional).getFwd(), BaseRecurrentLayer)
					End If
					assertEquals(rnnDataFormat, l0a.getRnnDataFormat())
					assertEquals(rnnDataFormat, l2a.getRnnDataFormat())

					Dim net As New MultiLayerNetwork(conf)
					net.init()
					Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT,If(rnnDataFormat = RNNFormat.NCW, New Long(){2, 5, 9}, New Long()){2, 9, 5})
					net.output([in])
				Next ffType
			Next rnnType
		End Sub
	End Class

End Namespace