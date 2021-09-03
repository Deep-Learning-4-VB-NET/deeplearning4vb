Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports TestDropout = org.deeplearning4j.nn.conf.dropout.TestDropout
Imports GravesLSTM = org.deeplearning4j.nn.conf.layers.GravesLSTM
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports RnnLossLayer = org.deeplearning4j.nn.conf.layers.RnnLossLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports RnnDataFormat = org.nd4j.enums.RnnDataFormat
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals
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

Namespace org.deeplearning4j.nn.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestRnnLayers extends org.deeplearning4j.BaseDL4JTest
	Public Class TestRnnLayers
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestRnnLayers#params") public void testTimeStepIs3Dimensional(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTimeStepIs3Dimensional(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)

			Dim nIn As Integer = 12
			Dim nOut As Integer = 3

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).weightInit(WeightInit.XAVIER).list().layer((New SimpleRnn.Builder()).nIn(nIn).nOut(3).dataFormat(rnnDataFormat).build()).layer((New LSTM.Builder()).nIn(3).nOut(5).dataFormat(rnnDataFormat).build()).layer((New RnnOutputLayer.Builder()).nOut(nOut).activation(Activation.SOFTMAX).build()).build()


			Dim net As New MultiLayerNetwork(conf)
			net.init()

'JAVA TO VB CONVERTER NOTE: The variable simpleRnn was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim simpleRnn_Conflict As org.deeplearning4j.nn.layers.recurrent.SimpleRnn = DirectCast(net.getLayer(0), org.deeplearning4j.nn.layers.recurrent.SimpleRnn)

			Dim rnnInput3d As INDArray = If(rnnDataFormat=RNNFormat.NCW, Nd4j.create(10,12, 1), Nd4j.create(10, 1, 12))
			Dim simpleOut As INDArray = simpleRnn_Conflict.rnnTimeStep(rnnInput3d, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(simpleOut.shape().SequenceEqual(If(rnnDataFormat=RNNFormat.NCW, New Long() {10, 3, 1}, New Long()){10, 1, 3}))

			Dim rnnInput2d As INDArray = Nd4j.create(10, 12)
			Try
				simpleRnn_Conflict.rnnTimeStep(rnnInput2d, LayerWorkspaceMgr.noWorkspaces())
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.Equals("3D input expected to RNN layer expected, got 2"))
			End Try

			Dim lstm As org.deeplearning4j.nn.layers.recurrent.LSTM = DirectCast(net.getLayer(1), org.deeplearning4j.nn.layers.recurrent.LSTM)

			Dim lstmInput3d As INDArray = If(rnnDataFormat=RNNFormat.NCW, Nd4j.create(10, 3, 1), Nd4j.create(10, 1, 3))
			Dim lstmOut As INDArray = lstm.rnnTimeStep(lstmInput3d, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(lstmOut.shape().SequenceEqual(If(rnnDataFormat=RNNFormat.NCW, New Long() {10, 5, 1}, New Long()){10, 1, 5}))

			Dim lstmInput2d As INDArray = Nd4j.create(10, 3)
			Try
				lstm.rnnTimeStep(lstmInput2d, LayerWorkspaceMgr.noWorkspaces())
			Catch e As System.InvalidOperationException
				assertTrue(e.Message.Equals("3D input expected to RNN layer expected, got 2"))
			End Try


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestRnnLayers#params") public void testDropoutRecurrentLayers(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDropoutRecurrentLayers(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)

			Dim layerTypes() As String = {"graves", "lstm", "simple"}

			For Each s As String In layerTypes

				Dim layer As Layer
				Dim layerD As Layer
				Dim layerD2 As Layer
				Dim cd As New TestDropout.CustomDropout()
				Select Case s
					Case "graves"
						layer = (New GravesLSTM.Builder()).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
						layerD = (New GravesLSTM.Builder()).dropOut(0.5).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
						layerD2 = (New GravesLSTM.Builder()).dropOut(cd).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
					Case "lstm"
						layer = (New LSTM.Builder()).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
						layerD = (New LSTM.Builder()).dropOut(0.5).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
						layerD2 = (New LSTM.Builder()).dropOut(cd).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
					Case "simple"
						layer = (New SimpleRnn.Builder()).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
						layerD = (New SimpleRnn.Builder()).dropOut(0.5).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
						layerD2 = (New SimpleRnn.Builder()).dropOut(cd).activation(Activation.TANH).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()
					Case Else
						Throw New Exception(s)
				End Select

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(layer).layer((New RnnOutputLayer.Builder()).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()).build()

				Dim confD As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(layerD).layer((New RnnOutputLayer.Builder()).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()).build()

				Dim confD2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(layerD2).layer((New RnnOutputLayer.Builder()).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).dataFormat(rnnDataFormat).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim netD As New MultiLayerNetwork(confD)
				netD.init()

				Dim netD2 As New MultiLayerNetwork(confD2)
				netD2.init()

				assertEquals(net.params(), netD.params(), s)
				assertEquals(net.params(), netD2.params(), s)

				Dim f As INDArray = Nd4j.rand(DataType.FLOAT, New Integer(){3, 10, 10})

				'Output: test mode -> no dropout
				Dim out1 As INDArray = net.output(f)
				Dim out1D As INDArray = netD.output(f)
				Dim out1D2 As INDArray = netD2.output(f)
				assertEquals(out1, out1D, s)
				assertEquals(out1, out1D2, s)


				Dim out2 As INDArray = net.output(f, True)
				Dim out2D As INDArray = netD.output(f, True)
				assertNotEquals(out2, out2D, s)

				Dim l As INDArray = TestUtils.randomOneHotTimeSeries(3, 10, 10, 12345)
				net.fit(f.dup(), l)
				netD.fit(f.dup(), l)
				assertNotEquals(net.params(), netD.params(), s)

				netD2.fit(f.dup(), l)
				netD2.fit(f.dup(), l)
				netD2.fit(f.dup(), l)


				Dim expected As IList(Of Pair(Of Integer, Integer)) = New List(Of Pair(Of Integer, Integer)) From {
					New Pair(Of Pair(Of Integer, Integer))(0, 0),
					New Pair(Of )(1, 0),
					New Pair(Of )(2, 0)
				}

				assertEquals(expected, cd.getAllCalls(), s)
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.nn.layers.recurrent.TestRnnLayers#params") public void testMismatchedInputLabelLength(org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMismatchedInputLabelLength(ByVal rnnDataFormat As RNNFormat, ByVal backend As Nd4jBackend)

			For i As Integer = 0 To 1

				Dim lb As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).list().layer((New SimpleRnn.Builder()).nIn(5).nOut(5).dataFormat(rnnDataFormat).build())

				Select Case i
					Case 0
						lb.layer((New RnnOutputLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(5).nOut(5).dataFormat(rnnDataFormat).build())
					Case 1
						lb.layer((New RnnLossLayer.Builder()).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).dataFormat(rnnDataFormat).build())
					Case Else
						Throw New Exception()
				End Select

				Dim conf As MultiLayerConfiguration = lb.build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, 3, 5, 5)
				Dim l As INDArray = TestUtils.randomOneHotTimeSeries(rnnDataFormat, 3, 5, 10, New Random(12345))
				Try
					net.fit([in],l)
				Catch t As Exception
					Dim msg As String = t.getMessage()
					If msg Is Nothing Then
						t.printStackTrace()
					End If
					Console.WriteLine(i)
					assertTrue(msg IsNot Nothing AndAlso msg.Contains("sequence length") AndAlso msg.Contains("input") AndAlso msg.Contains("label"), msg)
				End Try

			Next i


		End Sub
	End Class

End Namespace