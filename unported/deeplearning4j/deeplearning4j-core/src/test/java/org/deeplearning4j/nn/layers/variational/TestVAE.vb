Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports org.deeplearning4j.nn.conf.layers.variational
Imports WeightNoise = org.deeplearning4j.nn.conf.weightnoise.WeightNoise
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LossMAE = org.nd4j.linalg.lossfunctions.impl.LossMAE
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
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

Namespace org.deeplearning4j.nn.layers.variational


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.RNG) @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) public class TestVAE extends org.deeplearning4j.BaseDL4JTest
	Public Class TestVAE
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitialization()
		Public Overridable Sub testInitialization()

			Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(10).nOut(5).encoderLayerSizes(12).decoderLayerSizes(13).build()).build()

			Dim c As NeuralNetConfiguration = mlc.getConf(0)
			Dim vae As VariationalAutoencoder = CType(c.getLayer(), VariationalAutoencoder)

			Dim allParams As Long = vae.initializer().numParams(c)

			'                  Encoder         Encoder -> p(z|x)       Decoder         //p(x|z)
			Dim expNumParams As Integer = (10 * 12 + 12) + (12 * (2 * 5) + (2 * 5)) + (5 * 13 + 13) + (13 * (2 * 10) + (2 * 10))
			assertEquals(expNumParams, allParams)

			Dim net As New MultiLayerNetwork(mlc)
			net.init()

			Console.WriteLine("Exp num params: " & expNumParams)
			assertEquals(expNumParams, net.getLayer(0).params().length())
			Dim paramTable As IDictionary(Of String, INDArray) = net.getLayer(0).paramTable()
			Dim count As Integer = 0
			For Each arr As INDArray In paramTable.Values
				count += arr.length()
			Next arr
			assertEquals(expNumParams, count)

			assertEquals(expNumParams, net.getLayer(0).numParams())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testForwardPass()
		Public Overridable Sub testForwardPass()

			Dim encLayerSizes()() As Integer = {
				New Integer() {12},
				New Integer() {12, 13},
				New Integer() {12, 13, 14}
			}
			For i As Integer = 0 To encLayerSizes.Length - 1

				Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(10).nOut(5).encoderLayerSizes(encLayerSizes(i)).decoderLayerSizes(13).build()).build()

				Dim c As NeuralNetConfiguration = mlc.getConf(0)
				Dim vae As VariationalAutoencoder = CType(c.getLayer(), VariationalAutoencoder)

				Dim net As New MultiLayerNetwork(mlc)
				net.init()

				Dim [in] As INDArray = Nd4j.rand(1, 10)

				'        net.output(in);
				Dim [out] As IList(Of INDArray) = net.feedForward([in])
				assertArrayEquals(New Long() {1, 10}, [out](0).shape())
				assertArrayEquals(New Long() {1, 5}, [out](1).shape())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPretrainSimple()
		Public Overridable Sub testPretrainSimple()

			Dim inputSize As Integer = 3

			Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(inputSize).nOut(4).encoderLayerSizes(5).decoderLayerSizes(6).build()).build()

			Dim c As NeuralNetConfiguration = mlc.getConf(0)
			Dim vae As VariationalAutoencoder = CType(c.getLayer(), VariationalAutoencoder)

			Dim allParams As Long = vae.initializer().numParams(c)

			Dim net As New MultiLayerNetwork(mlc)
			net.init()
			net.initGradientsView() 'TODO this should happen automatically

			Dim paramTable As IDictionary(Of String, INDArray) = net.getLayer(0).paramTable()
			Dim gradTable As IDictionary(Of String, INDArray) = DirectCast(net.getLayer(0), org.deeplearning4j.nn.layers.variational.VariationalAutoencoder).getGradientViews()

			assertEquals(paramTable.Keys, gradTable.Keys)
			For Each s As String In paramTable.Keys
				assertEquals(paramTable(s).length(), gradTable(s).length())
				assertArrayEquals(paramTable(s).shape(), gradTable(s).shape())
			Next s

			Console.WriteLine("Num params: " & net.numParams())

			Dim data As INDArray = Nd4j.rand(1, inputSize)


			net.pretrainLayer(0, data)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParamGradientOrderAndViews()
		Public Overridable Sub testParamGradientOrderAndViews()
			Nd4j.Random.setSeed(12345)
			Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(10).nOut(5).encoderLayerSizes(12, 13).decoderLayerSizes(14, 15).build()).build()

			Dim c As NeuralNetConfiguration = mlc.getConf(0)
			Dim vae As VariationalAutoencoder = CType(c.getLayer(), VariationalAutoencoder)

			Dim net As New MultiLayerNetwork(mlc)
			net.init()

			net.initGradientsView()

			Dim layer As org.deeplearning4j.nn.layers.variational.VariationalAutoencoder = DirectCast(net.getLayer(0), org.deeplearning4j.nn.layers.variational.VariationalAutoencoder)

			Dim layerParams As IDictionary(Of String, INDArray) = layer.paramTable()
			Dim layerGradViews As IDictionary(Of String, INDArray) = layer.getGradientViews()

			layer.setInput(Nd4j.rand(3, 10), LayerWorkspaceMgr.noWorkspaces())
			layer.computeGradientAndScore(LayerWorkspaceMgr.noWorkspaces())
			Dim g As Gradient = layer.gradient()
			Dim grads As IDictionary(Of String, INDArray) = g.gradientForVariable()

			assertEquals(layerParams.Count, layerGradViews.Count)
			assertEquals(layerParams.Count, grads.Count)

			'Iteration order should be consistent due to linked hashmaps
			Dim pIter As IEnumerator(Of String) = layerParams.Keys.GetEnumerator()
			Dim gvIter As IEnumerator(Of String) = layerGradViews.Keys.GetEnumerator()
			Dim gIter As IEnumerator(Of String) = grads.Keys.GetEnumerator()

			Do While pIter.MoveNext()
				Dim p As String = pIter.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim gv As String = gvIter.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim gr As String = gIter.next()

				'            System.out.println(p + "\t" + gv + "\t" + gr);

				assertEquals(p, gv)
				assertEquals(p, gr)

				Dim pArr As INDArray = layerParams(p)
				Dim gvArr As INDArray = layerGradViews(p)
				Dim gArr As INDArray = grads(p)

				assertArrayEquals(pArr.shape(), gvArr.shape())
				assertTrue(gvArr Is gArr) 'Should be the exact same object due to view mechanics
			Loop
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPretrainParamsDuringBackprop()
		Public Overridable Sub testPretrainParamsDuringBackprop()
			'Idea: pretrain-specific parameters shouldn't change during backprop

			Nd4j.Random.setSeed(12345)
			Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(10).nOut(5).encoderLayerSizes(12, 13).decoderLayerSizes(14, 15).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(5).nOut(6).activation(New ActivationTanH()).build()).build()

			Dim c As NeuralNetConfiguration = mlc.getConf(0)
			Dim vae As VariationalAutoencoder = CType(c.getLayer(), VariationalAutoencoder)

			Dim net As New MultiLayerNetwork(mlc)
			net.init()

			net.initGradientsView()

			Dim layer As org.deeplearning4j.nn.layers.variational.VariationalAutoencoder = DirectCast(net.getLayer(0), org.deeplearning4j.nn.layers.variational.VariationalAutoencoder)

			Dim input As INDArray = Nd4j.rand(3, 10)
			net.pretrainLayer(0, input)

			'Get a snapshot of the pretrain params after fitting:
			Dim layerParams As IDictionary(Of String, INDArray) = layer.paramTable()
			Dim pretrainParamsBefore As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each s As String In layerParams.Keys
				If layer.isPretrainParam(s) Then
					pretrainParamsBefore(s) = layerParams(s).dup()
				End If
			Next s


			Dim features As INDArray = Nd4j.rand(3, 10)
			Dim labels As INDArray = Nd4j.rand(3, 6)

			For i As Integer = 0 To 2
				net.fit(features, labels)
			Next i

			Dim layerParamsAfter As IDictionary(Of String, INDArray) = layer.paramTable()

			For Each s As String In pretrainParamsBefore.Keys
				Dim before As INDArray = pretrainParamsBefore(s)
				Dim after As INDArray = layerParamsAfter(s)
				assertEquals(before, after)
			Next s
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJsonYaml()
		Public Overridable Sub testJsonYaml()

			Dim config As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New VariationalAutoencoder.Builder()).reconstructionDistribution(New GaussianReconstructionDistribution(Activation.IDENTITY)).nIn(3).nOut(4).encoderLayerSizes(5).decoderLayerSizes(6).build()).layer(1, (New VariationalAutoencoder.Builder()).reconstructionDistribution(New GaussianReconstructionDistribution(Activation.TANH)).nIn(7).nOut(8).encoderLayerSizes(9).decoderLayerSizes(10).build()).layer(2, (New VariationalAutoencoder.Builder()).reconstructionDistribution(New BernoulliReconstructionDistribution()).nIn(11).nOut(12).encoderLayerSizes(13).decoderLayerSizes(14).build()).layer(3, (New VariationalAutoencoder.Builder()).reconstructionDistribution(New ExponentialReconstructionDistribution(Activation.TANH)).nIn(11).nOut(12).encoderLayerSizes(13).decoderLayerSizes(14).build()).layer(4, (New VariationalAutoencoder.Builder()).lossFunction(New ActivationTanH(), LossFunctions.LossFunction.MSE).nIn(11).nOut(12).encoderLayerSizes(13).decoderLayerSizes(14).build()).layer(5, (New VariationalAutoencoder.Builder()).reconstructionDistribution((New CompositeReconstructionDistribution.Builder()).addDistribution(5, New GaussianReconstructionDistribution()).addDistribution(5, New GaussianReconstructionDistribution(Activation.TANH)).addDistribution(5, New BernoulliReconstructionDistribution()).build()).nIn(15).nOut(16).encoderLayerSizes(17).decoderLayerSizes(18).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(18).nOut(19).activation(New ActivationTanH()).build()).build()

			Dim asJson As String = config.toJson()
			Dim asYaml As String = config.toYaml()

			Dim fromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(asJson)
			Dim fromYaml As MultiLayerConfiguration = MultiLayerConfiguration.fromYaml(asYaml)

			assertEquals(config, fromJson)
			assertEquals(config, fromYaml)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReconstructionDistributionsSimple()
		Public Overridable Sub testReconstructionDistributionsSimple()

			Dim inOutSize As Integer = 6

			Dim reconstructionDistributions() As ReconstructionDistribution = {
				New GaussianReconstructionDistribution(Activation.IDENTITY),
				New GaussianReconstructionDistribution(Activation.TANH),
				New BernoulliReconstructionDistribution(Activation.SIGMOID),
				(New CompositeReconstructionDistribution.Builder()).addDistribution(2, New GaussianReconstructionDistribution(Activation.IDENTITY)).addDistribution(2, New BernoulliReconstructionDistribution()).addDistribution(2, New GaussianReconstructionDistribution(Activation.TANH)).build()
			}

			Nd4j.Random.setSeed(12345)
			For Each minibatch As Integer In New Integer() {1, 5}
				For i As Integer = 0 To reconstructionDistributions.Length - 1
					Dim data As INDArray
					Select Case i
						Case 0, 1 'Gaussian + identity
							data = Nd4j.rand(minibatch, inOutSize)
						Case 2 'Bernoulli
							data = Nd4j.create(minibatch, inOutSize)
							Nd4j.Executioner.exec(New BernoulliDistribution(data, 0.5), Nd4j.Random)
						Case 3 'Composite
							data = Nd4j.create(minibatch, inOutSize)
							data.get(NDArrayIndex.all(), NDArrayIndex.interval(0, 2)).assign(Nd4j.rand(minibatch, 2))
							Nd4j.Executioner.exec(New BernoulliDistribution(data.get(NDArrayIndex.all(), NDArrayIndex.interval(2, 4)), 0.5), Nd4j.Random)
							data.get(NDArrayIndex.all(), NDArrayIndex.interval(4, 6)).assign(Nd4j.rand(minibatch, 2))
						Case Else
							Throw New Exception()
					End Select

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.2).l1(0.3).updater(New Sgd(1.0)).seed(12345L).dist(New NormalDistribution(0, 1)).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(inOutSize).nOut(3).encoderLayerSizes(5).decoderLayerSizes(6).pzxActivationFunction(Activation.TANH).reconstructionDistribution(reconstructionDistributions(i)).activation(New ActivationTanH()).build()).build()

					Dim mln As New MultiLayerNetwork(conf)
					mln.init()
					mln.initGradientsView()
					mln.pretrainLayer(0, data)

					Dim layer As org.deeplearning4j.nn.layers.variational.VariationalAutoencoder = DirectCast(mln.getLayer(0), org.deeplearning4j.nn.layers.variational.VariationalAutoencoder)
					assertFalse(layer.hasLossFunction())

					Nd4j.Random.setSeed(12345)
					Dim reconstructionProb As INDArray = layer.reconstructionProbability(data, 50)
					assertArrayEquals(New Long() {minibatch, 1}, reconstructionProb.shape())

					Nd4j.Random.setSeed(12345)
					Dim reconstructionLogProb As INDArray = layer.reconstructionLogProbability(data, 50)
					assertArrayEquals(New Long() {minibatch, 1}, reconstructionLogProb.shape())

					'                System.out.println(reconstructionDistributions[i]);
					For j As Integer = 0 To minibatch - 1
						Dim p As Double = reconstructionProb.getDouble(j)
						Dim logp As Double = reconstructionLogProb.getDouble(j)
						assertTrue(p >= 0.0 AndAlso p <= 1.0)
						assertTrue(logp <= 0.0)

						Dim pFromLogP As Double = Math.Exp(logp)
						assertEquals(p, pFromLogP, 1e-6)
					Next j
				Next i
			Next minibatch
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReconstructionErrorSimple()
		Public Overridable Sub testReconstructionErrorSimple()

			Dim inOutSize As Integer = 6

			Dim reconstructionDistributions() As ReconstructionDistribution = {
				New LossFunctionWrapper(Activation.TANH, New LossMSE()),
				New LossFunctionWrapper(Activation.IDENTITY, New LossMAE()),
				(New CompositeReconstructionDistribution.Builder()).addDistribution(3, New LossFunctionWrapper(Activation.TANH, New LossMSE())).addDistribution(3, New LossFunctionWrapper(Activation.IDENTITY, New LossMAE())).build()
			}

			Nd4j.Random.setSeed(12345)
			For Each minibatch As Integer In New Integer() {1, 5}
				For i As Integer = 0 To reconstructionDistributions.Length - 1
					Dim data As INDArray = Nd4j.rand(minibatch, inOutSize).muli(2).subi(1)

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l2(0.2).l1(0.3).updater(New Sgd(1.0)).seed(12345L).dist(New NormalDistribution(0, 1)).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(inOutSize).nOut(3).encoderLayerSizes(5).decoderLayerSizes(6).pzxActivationFunction(Activation.TANH).reconstructionDistribution(reconstructionDistributions(i)).activation(New ActivationTanH()).build()).build()

					Dim mln As New MultiLayerNetwork(conf)
					mln.init()
					mln.initGradientsView()
					mln.pretrainLayer(0, data)

					Dim layer As org.deeplearning4j.nn.layers.variational.VariationalAutoencoder = DirectCast(mln.getLayer(0), org.deeplearning4j.nn.layers.variational.VariationalAutoencoder)
					assertTrue(layer.hasLossFunction())

					Nd4j.Random.setSeed(12345)
					Dim reconstructionError As INDArray = layer.reconstructionError(data)
					assertArrayEquals(New Long() {minibatch, 1}, reconstructionError.shape())

					For j As Integer = 0 To minibatch - 1
						Dim re As Double = reconstructionError.getDouble(j)
						assertTrue(re >= 0.0)
					Next j
				Next i
			Next minibatch
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVaeWeightNoise()
		Public Overridable Sub testVaeWeightNoise()

			For Each ws As Boolean In New Boolean(){False, True}

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345L).trainingWorkspaceMode(If(ws, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).inferenceWorkspaceMode(If(ws, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).weightNoise(New WeightNoise(New NormalDistribution(0.1, 0.3))).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(10).nOut(3).encoderLayerSizes(5).decoderLayerSizes(6).pzxActivationFunction(Activation.TANH).reconstructionDistribution(New GaussianReconstructionDistribution()).activation(New ActivationTanH()).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim arr As INDArray = Nd4j.rand(3, 10)
				net.pretrainLayer(0, arr)

			Next ws


		End Sub
	End Class

End Namespace