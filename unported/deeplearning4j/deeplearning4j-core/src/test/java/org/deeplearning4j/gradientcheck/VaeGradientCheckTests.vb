Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports org.deeplearning4j.nn.conf.layers.variational
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class VaeGradientCheckTests extends org.deeplearning4j.BaseDL4JTest
	Public Class VaeGradientCheckTests
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = False
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
'ORIGINAL LINE: @Test public void testVaeAsMLP()
		Public Overridable Sub testVaeAsMLP()
			'Post pre-training: a VAE can be used as a MLP, by taking the mean value from p(z|x) as the output
			'This gradient check tests this part

			Dim activFns() As Activation = {Activation.IDENTITY, Activation.TANH, Activation.IDENTITY, Activation.TANH, Activation.IDENTITY, Activation.TANH}

			Dim lossFunctions() As LossFunction = {LossFunction.MCXENT, LossFunction.MCXENT, LossFunction.MSE, LossFunction.MSE, LossFunction.MCXENT, LossFunction.MSE}
			Dim outputActivations() As Activation = {Activation.SOFTMAX, Activation.SOFTMAX, Activation.TANH, Activation.TANH, Activation.SOFTMAX, Activation.TANH}

			'use l2vals[i] with l1vals[i]
			Dim l2vals() As Double = {0.4, 0.0, 0.4, 0.4, 0.0, 0.0}
			Dim l1vals() As Double = {0.0, 0.0, 0.5, 0.0, 0.0, 0.5}
			Dim biasL2() As Double = {0.0, 0.0, 0.0, 0.2, 0.0, 0.4}
			Dim biasL1() As Double = {0.0, 0.0, 0.6, 0.0, 0.0, 0.0}

			Dim encoderLayerSizes()() As Integer = {
				New Integer() {5},
				New Integer() {5},
				New Integer() {5, 6},
				New Integer() {5, 6},
				New Integer() {5},
				New Integer() {5, 6}
			}
			Dim decoderLayerSizes()() As Integer = {
				New Integer() {6},
				New Integer() {7, 8},
				New Integer() {6},
				New Integer() {7, 8},
				New Integer() {6},
				New Integer() {7, 8}
			}

			Dim minibatches() As Integer = {1, 5, 4, 3, 1, 4}

			Nd4j.Random.setSeed(12345)
			For i As Integer = 0 To activFns.Length - 1
				Dim lf As LossFunction = lossFunctions(i)
				Dim outputActivation As Activation = outputActivations(i)
				Dim l2 As Double = l2vals(i)
				Dim l1 As Double = l1vals(i)
				Dim encoderSizes() As Integer = encoderLayerSizes(i)
				Dim decoderSizes() As Integer = decoderLayerSizes(i)
				Dim minibatch As Integer = minibatches(i)
				Dim input As INDArray = Nd4j.rand(minibatch, 4)
				Dim labels As INDArray = Nd4j.create(minibatch, 3)
				For j As Integer = 0 To minibatch - 1
					labels.putScalar(j, j Mod 3, 1.0)
				Next j
				Dim afn As Activation = activFns(i)

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(l2).l1(l1).dataType(DataType.DOUBLE).updater(New NoOp()).l2Bias(biasL2(i)).l1Bias(biasL1(i)).updater(New NoOp()).seed(12345L).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(4).nOut(3).encoderLayerSizes(encoderSizes).decoderLayerSizes(decoderSizes).dist(New NormalDistribution(0, 1)).activation(afn).build()).layer(1, (New OutputLayer.Builder(lf)).activation(outputActivation).nIn(3).nOut(3).dist(New NormalDistribution(0, 1)).build()).build()

				Dim mln As New MultiLayerNetwork(conf)
				mln.init()

				Dim msg As String = "testVaeAsMLP() - activationFn=" & afn & ", lossFn=" & lf & ", outputActivation=" & outputActivation & ", encLayerSizes = " & Arrays.toString(encoderSizes) & ", decLayerSizes = " & Arrays.toString(decoderSizes) & ", l2=" & l2 & ", l1=" & l1
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'                for (int j = 0; j < mln.getnLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, labels)
				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(mln)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVaePretrain()
		Public Overridable Sub testVaePretrain()
			Nd4j.Random.setSeed(12345)
			Dim activFns() As Activation = {Activation.IDENTITY, Activation.TANH, Activation.SOFTSIGN}
			Dim pzxAfns() As Activation = {Activation.IDENTITY, Activation.IDENTITY, Activation.TANH}
			Dim pxzAfns() As Activation = {Activation.TANH, Activation.TANH, Activation.IDENTITY}

			'use l2vals[i] with l1vals[i]
			Dim l2vals() As Double = {0.0, 0.4, 0.4}
			Dim l1vals() As Double = {0.0, 0.5, 0.0}
			Dim biasL2() As Double = {0.0, 0.0, 0.2}
			Dim biasL1() As Double = {0.0, 0.6, 0.0}

			Dim encoderLayerSizes()() As Integer = {
				New Integer() {5},
				New Integer() {3, 4},
				New Integer() {3, 4}
			}
			Dim decoderLayerSizes()() As Integer = {
				New Integer() {4},
				New Integer() {2},
				New Integer() {4, 3}
			}

			Dim minibatches() As Integer = {1, 3, 2, 3}

			Nd4j.Random.setSeed(12345)
			For i As Integer = 0 To activFns.Length - 1
				Dim l2 As Double = l2vals(i)
				Dim l1 As Double = l1vals(i)
				Dim encoderSizes() As Integer = encoderLayerSizes(i)
				Dim decoderSizes() As Integer = decoderLayerSizes(i)
				Dim minibatch As Integer = minibatches(i)
				Dim input As INDArray = Nd4j.rand(minibatch, 4)
				Dim labels As INDArray = Nd4j.create(minibatch, 3)
				For j As Integer = 0 To minibatch - 1
					labels.putScalar(j, j Mod 3, 1.0)
				Next j
				Dim afn As Activation = activFns(i)
				Dim pzxAfn As Activation = pzxAfns(i)
				Dim pxzAfn As Activation = pxzAfns(i)

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(l2).dataType(DataType.DOUBLE).l1(l1).l2Bias(biasL2(i)).l1Bias(biasL1(i)).updater(New NoOp()).seed(12345L).weightInit(WeightInit.XAVIER).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(4).nOut(3).encoderLayerSizes(encoderSizes).decoderLayerSizes(decoderSizes).pzxActivationFunction(pzxAfn).reconstructionDistribution(New GaussianReconstructionDistribution(pxzAfn)).activation(afn).build()).build()

				Dim mln As New MultiLayerNetwork(conf)
				mln.init()
				mln.initGradientsView()

				Dim layer As org.deeplearning4j.nn.api.Layer = mln.getLayer(0)

				Dim msg As String = "testVaePretrain() - activationFn=" & afn & ", p(z|x) afn = " & pzxAfn & ", p(x|z) afn = " & pxzAfn & ", encLayerSizes = " & Arrays.toString(encoderSizes) & ", decLayerSizes = " & Arrays.toString(decoderSizes) & ", l2=" & l2 & ", l1=" & l1
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'                for (int l = 0; l < mln.getnLayers(); l++)
	'                    System.out.println("Layer " + l + " # params: " + mln.getLayer(l).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradientsPretrainLayer(layer, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, input, 12345)

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(mln)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVaePretrainReconstructionDistributions()
		Public Overridable Sub testVaePretrainReconstructionDistributions()

			Dim inOutSize As Integer = 3

			Dim reconstructionDistributions() As ReconstructionDistribution = {
				New GaussianReconstructionDistribution(Activation.IDENTITY),
				New GaussianReconstructionDistribution(Activation.TANH),
				New BernoulliReconstructionDistribution(Activation.SIGMOID),
				(New CompositeReconstructionDistribution.Builder()).addDistribution(1, New GaussianReconstructionDistribution(Activation.IDENTITY)).addDistribution(1, New BernoulliReconstructionDistribution()).addDistribution(1, New GaussianReconstructionDistribution(Activation.TANH)).build(),
				New ExponentialReconstructionDistribution(Activation.TANH),
				New LossFunctionWrapper(New ActivationTanH(), New LossMSE())
			}

			Nd4j.Random.setSeed(12345)
			For i As Integer = 0 To reconstructionDistributions.Length - 1
				Dim minibatch As Integer = (If(i Mod 2 = 0, 1, 3))

				Dim data As INDArray
				Select Case i
					Case 0, 1 'Gaussian + identity
						data = Nd4j.rand(minibatch, inOutSize)
					Case 2 'Bernoulli
						data = Nd4j.create(minibatch, inOutSize)
						Nd4j.Executioner.exec(New BernoulliDistribution(data, 0.5), Nd4j.Random)
					Case 3 'Composite
						data = Nd4j.create(minibatch, inOutSize)
						data.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 1)).assign(Nd4j.rand(minibatch, 1))
						Nd4j.Executioner.exec(New BernoulliDistribution(data.get(NDArrayIndex.all(), NDArrayIndex.interval(1, 2)), 0.5), Nd4j.Random)
						data.get(NDArrayIndex.all(), NDArrayIndex.interval(2, 3)).assign(Nd4j.rand(minibatch, 1))
					Case 4, 5
						data = Nd4j.rand(minibatch, inOutSize)
					Case Else
						Throw New Exception()
				End Select

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.2).l1(0.3).dataType(DataType.DOUBLE).updater(New NoOp()).seed(12345L).dist(New NormalDistribution(0, 1)).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(inOutSize).nOut(3).encoderLayerSizes(4).decoderLayerSizes(3).pzxActivationFunction(Activation.TANH).reconstructionDistribution(reconstructionDistributions(i)).activation(Activation.TANH).build()).build()

				Dim mln As New MultiLayerNetwork(conf)
				mln.init()
				mln.initGradientsView()

				Dim layer As org.deeplearning4j.nn.api.Layer = mln.getLayer(0)

				Dim msg As String = "testVaePretrainReconstructionDistributions() - " & reconstructionDistributions(i)
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'                for (int j = 0; j < mln.getnLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradientsPretrainLayer(layer, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, data, 12345)

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(mln)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVaePretrainMultipleSamples()
		Public Overridable Sub testVaePretrainMultipleSamples()

			Dim minibatch As Integer = 2
			Nd4j.Random.setSeed(12345)
			For Each numSamples As Integer In New Integer(){1, 2}
				Dim features As INDArray = Nd4j.rand(DataType.DOUBLE, minibatch, 4)

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.2).l1(0.3).dataType(DataType.DOUBLE).updater(New NoOp()).seed(12345L).weightInit(WeightInit.XAVIER).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(4).nOut(3).encoderLayerSizes(2, 3).decoderLayerSizes(4, 3).pzxActivationFunction(Activation.TANH).reconstructionDistribution(New GaussianReconstructionDistribution(Activation.TANH)).numSamples(numSamples).activation(Activation.TANH).build()).build()

				Dim mln As New MultiLayerNetwork(conf)
				mln.init()
				mln.initGradientsView()

				Dim layer As org.deeplearning4j.nn.api.Layer = mln.getLayer(0)

				Dim msg As String = "testVaePretrainMultipleSamples() - numSamples = " & numSamples
				If PRINT_RESULTS Then
					Console.WriteLine(msg)
	'                for (int j = 0; j < mln.getnLayers(); j++)
	'                    System.out.println("Layer " + j + " # params: " + mln.getLayer(j).numParams());
				End If

				Dim gradOK As Boolean = GradientCheckUtil.checkGradientsPretrainLayer(layer, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, features, 12345)

				assertTrue(gradOK, msg)
				TestUtils.testModelSerialization(mln)
			Next numSamples
		End Sub
	End Class

End Namespace