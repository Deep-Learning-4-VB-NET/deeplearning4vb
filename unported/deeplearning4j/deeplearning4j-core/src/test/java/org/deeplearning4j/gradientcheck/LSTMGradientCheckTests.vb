Imports System
Imports Microsoft.VisualBasic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Updater = org.deeplearning4j.nn.conf.Updater
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports RnnToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToCnnPreProcessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
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

Namespace org.deeplearning4j.gradientcheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class LSTMGradientCheckTests extends org.deeplearning4j.BaseDL4JTest
	Public Class LSTMGradientCheckTests
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLSTMBasicMultiLayer()
		Public Overridable Sub testLSTMBasicMultiLayer()
			'Basic test of GravesLSTM layer
			Nd4j.Random.Seed = 12345L

			Dim timeSeriesLength As Integer = 4
			Dim nIn As Integer = 2
			Dim layerSize As Integer = 2
			Dim nOut As Integer = 2
			Dim miniBatchSize As Integer = 5

			Dim gravesLSTM() As Boolean = {True, False}

			For Each graves As Boolean In gravesLSTM

				Dim l0 As Layer
				Dim l1 As Layer
				If graves Then
					l0 = (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.SIGMOID).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()
					l1 = (New GravesLSTM.Builder()).nIn(layerSize).nOut(layerSize).activation(Activation.SIGMOID).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()
				Else
					l0 = (New LSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.SIGMOID).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()
					l1 = (New LSTM.Builder()).nIn(layerSize).nOut(layerSize).activation(Activation.SIGMOID).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()
				End If

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).dataType(DataType.DOUBLE).list().layer(0, l0).layer(1, l1).layer(2, (New RnnOutputLayer.Builder(LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1.0)).updater(New NoOp()).build()).build()

				Dim mln As New MultiLayerNetwork(conf)
				mln.init()

				Dim r As New Random(12345L)
				Dim input As INDArray = Nd4j.zeros(miniBatchSize, nIn, timeSeriesLength)
				For i As Integer = 0 To miniBatchSize - 1
					For j As Integer = 0 To nIn - 1
						For k As Integer = 0 To timeSeriesLength - 1
							input.putScalar(New Integer() {i, j, k}, r.NextDouble() - 0.5)
						Next k
					Next j
				Next i

				Dim labels As INDArray = Nd4j.zeros(miniBatchSize, nOut, timeSeriesLength)
				For i As Integer = 0 To miniBatchSize - 1
					For j As Integer = 0 To timeSeriesLength - 1
						Dim idx As Integer = r.Next(nOut)
						labels.putScalar(New Integer() {i, idx, j}, 1.0)
					Next j
				Next i

				Dim testName As String = "testLSTMBasic(" & (If(graves, "GravesLSTM", "LSTM")) & ")"
				If PRINT_RESULTS Then
					Console.WriteLine(testName)
	'                for (int j = 0; j < mln.getnLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

				assertTrue(gradOK, testName)
				TestUtils.testModelSerialization(mln)
			Next graves
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientLSTMFull()
		Public Overridable Sub testGradientLSTMFull()

			Dim timeSeriesLength As Integer = 4
			Dim nIn As Integer = 3
			Dim layerSize As Integer = 4
			Dim nOut As Integer = 2
			Dim miniBatchSize As Integer = 2

			Dim gravesLSTM() As Boolean = {True, False}

			For Each graves As Boolean In gravesLSTM

				Dim r As New Random(12345L)
				Dim input As INDArray = Nd4j.rand(New Integer(){miniBatchSize, nIn, timeSeriesLength}, "f"c).subi(0.5)

				Dim labels As INDArray = Nd4j.zeros(miniBatchSize, nOut, timeSeriesLength)
				For i As Integer = 0 To miniBatchSize - 1
					For j As Integer = 0 To timeSeriesLength - 1
						Dim idx As Integer = r.Next(nOut)
						labels.putScalar(New Integer() {i, idx, j}, 1.0f)
					Next j
				Next i


				'use l2vals[i] with l1vals[i]
				Dim l2vals() As Double = {0.4, 0.0}
				Dim l1vals() As Double = {0.0, 0.5}
				Dim biasL2() As Double = {0.3, 0.0}
				Dim biasL1() As Double = {0.0, 0.6}
				Dim activFns() As Activation = {Activation.TANH, Activation.SOFTSIGN}
				Dim lossFunctions() As LossFunction = {LossFunction.MCXENT, LossFunction.MSE}
				Dim outputActivations() As Activation = {Activation.SOFTMAX, Activation.TANH}

				For i As Integer = 0 To l2vals.Length - 1

					Dim lf As LossFunction = lossFunctions(i)
					Dim outputActivation As Activation = outputActivations(i)
					Dim l2 As Double = l2vals(i)
					Dim l1 As Double = l1vals(i)
					Dim afn As Activation = activFns(i)

					Dim conf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345L).dist(New NormalDistribution(0, 1)).updater(New NoOp())

					If l1 > 0.0 Then
						conf.l1(l1)
					End If
					If l2 > 0.0 Then
						conf.l2(l2)
					End If
					If biasL2(i) > 0 Then
						conf.l2Bias(biasL2(i))
					End If
					If biasL1(i) > 0 Then
						conf.l1Bias(biasL1(i))
					End If

					Dim layer As Layer
					If graves Then
						layer = (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).activation(afn).build()
					Else
						layer = (New LSTM.Builder()).nIn(nIn).nOut(layerSize).activation(afn).build()
					End If

					Dim conf2 As NeuralNetConfiguration.ListBuilder = conf.list().layer(0, layer).layer(1, (New RnnOutputLayer.Builder(lf)).activation(outputActivation).nIn(layerSize).nOut(nOut).build())

					Dim mln As New MultiLayerNetwork(conf2.build())
					mln.init()

					Dim testName As String = "testGradientLSTMFull(" & (If(graves, "GravesLSTM", "LSTM")) & " - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", l2=" & l2 & ", l1=" & l1
					If PRINT_RESULTS Then
						Console.WriteLine(testName)
	'                    for (int j = 0; j < mln.getnLayers(); j++)
	'                        System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).subset(True).maxPerParam(128))

					assertTrue(gradOK, testName)
					TestUtils.testModelSerialization(mln)
				Next i
			Next graves
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientLSTMEdgeCases()
		Public Overridable Sub testGradientLSTMEdgeCases()
			'Edge cases: T=1, miniBatchSize=1, both
			Dim timeSeriesLength() As Integer = {1, 5, 1}
			Dim miniBatchSize() As Integer = {7, 1, 1}

			Dim nIn As Integer = 3
			Dim layerSize As Integer = 4
			Dim nOut As Integer = 2

			Dim gravesLSTM() As Boolean = {True, False}

			For Each graves As Boolean In gravesLSTM

				For i As Integer = 0 To timeSeriesLength.Length - 1

					Dim r As New Random(12345L)
					Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, miniBatchSize(i), nIn, timeSeriesLength(i))

					Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(miniBatchSize(i), nOut, timeSeriesLength(i))

					Dim layer As Layer
					If graves Then
						layer = (New GravesLSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build()
					Else
						layer = (New LSTM.Builder()).nIn(nIn).nOut(layerSize).activation(Activation.TANH).build()
					End If

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).dataType(DataType.DOUBLE).dist(New NormalDistribution(0, 1)).updater(New NoOp()).list().layer(0, layer).layer(1, (New RnnOutputLayer.Builder(LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).build()).build()
					Dim mln As New MultiLayerNetwork(conf)
					mln.init()

					Dim msg As String = "testGradientLSTMEdgeCases(" & (If(graves, "GravesLSTM", "LSTM")) & " - timeSeriesLength=" & timeSeriesLength(i) & ", miniBatchSize=" & miniBatchSize(i)
					Console.WriteLine(msg)
					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
					assertTrue(gradOK, msg)
					TestUtils.testModelSerialization(mln)
				Next i
			Next graves
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientGravesBidirectionalLSTMFull()
		Public Overridable Sub testGradientGravesBidirectionalLSTMFull()
			Dim activFns() As Activation = {Activation.TANH, Activation.SOFTSIGN}

			Dim lossFunctions() As LossFunction = {LossFunction.MCXENT, LossFunction.MSE}
			Dim outputActivations() As Activation = {Activation.SOFTMAX, Activation.TANH} 'i.e., lossFunctions[i] used with outputActivations[i] here

			Dim timeSeriesLength As Integer = 3
			Dim nIn As Integer = 2
			Dim layerSize As Integer = 2
			Dim nOut As Integer = 2
			Dim miniBatchSize As Integer = 3

			Dim r As New Random(12345L)
			Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, miniBatchSize, nIn, timeSeriesLength).subi(0.5)

			Dim labels As INDArray = TestUtils.randomOneHotTimeSeries(miniBatchSize, nOut, timeSeriesLength)

			'use l2vals[i] with l1vals[i]
			Dim l2vals() As Double = {0.4, 0.0}
			Dim l1vals() As Double = {0.5, 0.0}
			Dim biasL2() As Double = {0.0, 0.2}
			Dim biasL1() As Double = {0.0, 0.6}

			For i As Integer = 0 To lossFunctions.Length - 1
				For k As Integer = 0 To l2vals.Length - 1
					Dim afn As Activation = activFns(i)
					Dim lf As LossFunction = lossFunctions(i)
					Dim outputActivation As Activation = outputActivations(i)
					Dim l2 As Double = l2vals(k)
					Dim l1 As Double = l1vals(k)

					Dim conf As New NeuralNetConfiguration.Builder()
					If l1 > 0.0 Then
						conf.l1(l1)
					End If
					If l2 > 0.0 Then
						conf.l2(l2)
					End If
					If biasL2(k) > 0 Then
						conf.l2Bias(biasL2(k))
					End If
					If biasL1(k) > 0 Then
						conf.l1Bias(biasL1(k))
					End If

					Dim mlc As MultiLayerConfiguration = conf.seed(12345L).dataType(DataType.DOUBLE).updater(New NoOp()).list().layer(0, (New GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(layerSize).weightInit(New NormalDistribution(0, 1)).activation(afn).build()).layer(1, (New RnnOutputLayer.Builder(lf)).activation(outputActivation).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).build()


					Dim mln As New MultiLayerNetwork(mlc)

					mln.init()

					If PRINT_RESULTS Then
						Console.WriteLine("testGradientGravesBidirectionalLSTMFull() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", l2=" & l2 & ", l1=" & l1)
	'                    for (int j = 0; j < mln.getnLayers(); j++)
	'                        System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
					End If

					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)

					Dim msg As String = "testGradientGravesLSTMFull() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", l2=" & l2 & ", l1=" & l1
					assertTrue(gradOK, msg)
					TestUtils.testModelSerialization(mln)
				Next k
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientGravesBidirectionalLSTMEdgeCases()
		Public Overridable Sub testGradientGravesBidirectionalLSTMEdgeCases()
			'Edge cases: T=1, miniBatchSize=1, both
			Dim timeSeriesLength() As Integer = {1, 5, 1}
			Dim miniBatchSize() As Integer = {7, 1, 1}

			Dim nIn As Integer = 3
			Dim layerSize As Integer = 4
			Dim nOut As Integer = 2

			For i As Integer = 0 To timeSeriesLength.Length - 1

				Dim r As New Random(12345L)
				Dim input As INDArray = Nd4j.rand(DataType.DOUBLE, miniBatchSize(i), nIn, timeSeriesLength(i)).subi(0.5)

				Dim labels As INDArray = Nd4j.zeros(miniBatchSize(i), nOut, timeSeriesLength(i))
				Dim m As Integer = 0
				Do While m < miniBatchSize(i)
					Dim j As Integer = 0
					Do While j < timeSeriesLength(i)
						Dim idx As Integer = r.Next(nOut)
						labels.putScalar(New Integer() {m, idx, j}, 1.0f)
						j += 1
					Loop
					m += 1
				Loop

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).dataType(DataType.DOUBLE).list().layer(0, (New GravesBidirectionalLSTM.Builder()).nIn(nIn).nOut(layerSize).dist(New NormalDistribution(0, 1)).updater(Updater.NONE).build()).layer(1, (New RnnOutputLayer.Builder(LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).dist(New NormalDistribution(0, 1)).updater(New NoOp()).build()).build()
				Dim mln As New MultiLayerNetwork(conf)
				mln.init()

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).subset(True).maxPerParam(128))

				Dim msg As String = "testGradientGravesLSTMEdgeCases() - timeSeriesLength=" & timeSeriesLength(i) & ", miniBatchSize=" & miniBatchSize(i)
				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(mln)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientCnnFfRnn()
		Public Overridable Sub testGradientCnnFfRnn()
			'Test gradients with CNN -> FF -> LSTM -> RnnOutputLayer
			'time series input/output (i.e., video classification or similar)

			Dim nChannelsIn As Integer = 2
			Dim inputSize As Integer = 6 * 6 * nChannelsIn '10px x 10px x 3 channels
			Dim miniBatchSize As Integer = 2
			Dim timeSeriesLength As Integer = 4
			Dim nClasses As Integer = 2

			'Generate
			Nd4j.Random.setSeed(12345)
			Dim input As INDArray = Nd4j.rand(New Integer() {miniBatchSize, inputSize, timeSeriesLength})
			Dim labels As INDArray = Nd4j.zeros(miniBatchSize, nClasses, timeSeriesLength)
			Dim r As New Random(12345)
			For i As Integer = 0 To miniBatchSize - 1
				For j As Integer = 0 To timeSeriesLength - 1
					Dim idx As Integer = r.Next(nClasses)
					labels.putScalar(New Integer() {i, idx, j}, 1.0)
				Next j
			Next i


			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).seed(12345).dataType(DataType.DOUBLE).dist(New UniformDistribution(-2, 2)).list().layer(0, (New ConvolutionLayer.Builder(3, 3)).nIn(2).nOut(3).stride(1, 1).activation(Activation.TANH).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(1, 1).build()).layer(2, (New DenseLayer.Builder()).nIn(27).nOut(4).activation(Activation.TANH).build()).layer(3, (New GravesLSTM.Builder()).nIn(4).nOut(3).activation(Activation.TANH).build()).layer(4, (New RnnOutputLayer.Builder()).lossFunction(LossFunction.MCXENT).nIn(3).nOut(nClasses).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(6, 6, 2)).build()

			'Here: ConvolutionLayerSetup in config builder doesn't know that we are expecting time series input, not standard FF input -> override it here
			conf.getInputPreProcessors().put(0, New RnnToCnnPreProcessor(6, 6, 2))

			Dim mln As New MultiLayerNetwork(conf)
			mln.init()

			Console.WriteLine("Params per layer:")
			Dim i As Integer = 0
			Do While i < mln.getnLayers()
				Console.WriteLine("layer " & i & vbTab & mln.getLayer(i).numParams())
				i += 1
			Loop

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(mln).input(input).labels(labels).subset(True).maxPerParam(32))
			assertTrue(gradOK)
			TestUtils.testModelSerialization(mln)
		End Sub
	End Class

End Namespace