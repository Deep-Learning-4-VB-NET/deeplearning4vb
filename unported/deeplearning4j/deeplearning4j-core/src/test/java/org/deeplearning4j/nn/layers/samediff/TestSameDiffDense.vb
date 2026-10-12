Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports GradientCheckUtil = org.deeplearning4j.gradientcheck.GradientCheckUtil
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports SameDiffDense = org.deeplearning4j.nn.layers.samediff.testlayers.SameDiffDense
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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

Namespace org.deeplearning4j.nn.layers.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) public class TestSameDiffDense extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSameDiffDense
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffDenseBasic()
		Public Overridable Sub testSameDiffDenseBasic()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New SameDiffDense.Builder()).nIn(nIn).nOut(nOut).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim pt1 As IDictionary(Of String, INDArray) = net.getLayer(0).paramTable()
			assertNotNull(pt1)
			assertEquals(2, pt1.Count)
			assertNotNull(pt1(DefaultParamInitializer.WEIGHT_KEY))
			assertNotNull(pt1(DefaultParamInitializer.BIAS_KEY))

			assertArrayEquals(New Long(){nIn, nOut}, pt1(DefaultParamInitializer.WEIGHT_KEY).shape())
			assertArrayEquals(New Long(){1, nOut}, pt1(DefaultParamInitializer.BIAS_KEY).shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffDenseForward()
		Public Overridable Sub testSameDiffDenseForward()
			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.ENABLED, WorkspaceMode.NONE}
				For Each minibatch As Integer In New Integer(){5, 1}
					Dim nIn As Integer = 3
					Dim nOut As Integer = 4

					Dim afns() As Activation = { Activation.TANH, Activation.SIGMOID, Activation.ELU, Activation.IDENTITY, Activation.SOFTPLUS, Activation.SOFTSIGN, Activation.CUBE, Activation.HARDTANH, Activation.RELU }

					For Each a As Activation In afns
						log.info("Starting test - " & a & ", workspace = " & wsm)
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).inferenceWorkspaceMode(wsm).trainingWorkspaceMode(wsm).list().layer((New SameDiffDense.Builder()).nIn(nIn).nOut(nOut).activation(a).build()).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						assertNotNull(net.paramTable())

						Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).activation(a).nIn(nIn).nOut(nOut).build()).build()

						Dim net2 As New MultiLayerNetwork(conf2)
						net2.init()

						net.params().assign(net2.params())

						'Check params:
						assertEquals(net2.params(), net.params())
						Dim params1 As IDictionary(Of String, INDArray) = net.paramTable()
						Dim params2 As IDictionary(Of String, INDArray) = net2.paramTable()
						assertEquals(params2, params1)

						Dim [in] As INDArray = Nd4j.rand(minibatch, nIn)
						Dim [out] As INDArray = net.output([in])
						Dim outExp As INDArray = net2.output([in])

						assertEquals(outExp, [out])

						'Also check serialization:
						Dim netLoaded As MultiLayerNetwork = TestUtils.testModelSerialization(net)
						Dim outLoaded As INDArray = netLoaded.output([in])

						assertEquals(outExp, outLoaded)

						'Sanity check on different minibatch sizes:
						Dim newIn As INDArray = Nd4j.vstack([in], [in])
						Dim outMbsd As INDArray = net.output(newIn)
						Dim outMb As INDArray = net2.output(newIn)
						assertEquals(outMb, outMbsd)
					Next a
				Next minibatch
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffDenseForwardMultiLayer()
		Public Overridable Sub testSameDiffDenseForwardMultiLayer()
			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.ENABLED, WorkspaceMode.NONE}
				For Each minibatch As Integer In New Integer(){5, 1}
					Dim nIn As Integer = 3
					Dim nOut As Integer = 4

					Dim afns() As Activation = { Activation.TANH, Activation.SIGMOID, Activation.ELU, Activation.IDENTITY, Activation.SOFTPLUS, Activation.SOFTSIGN, Activation.CUBE, Activation.HARDTANH, Activation.RELU }

					For Each a As Activation In afns
						log.info("Starting test - " & a & " - workspace=" & wsm)
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer((New SameDiffDense.Builder()).nIn(nIn).nOut(nOut).weightInit(WeightInit.XAVIER).activation(a).build()).layer((New SameDiffDense.Builder()).nIn(nOut).nOut(nOut).weightInit(WeightInit.XAVIER).activation(a).build()).layer((New OutputLayer.Builder()).nIn(nOut).nOut(nOut).weightInit(WeightInit.XAVIER).activation(a).build()).validateOutputLayerConfig(False).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						assertNotNull(net.paramTable())

						Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).list().layer((New DenseLayer.Builder()).activation(a).nIn(nIn).nOut(nOut).build()).layer((New DenseLayer.Builder()).activation(a).nIn(nOut).nOut(nOut).build()).layer((New OutputLayer.Builder()).nIn(nOut).nOut(nOut).activation(a).build()).validateOutputLayerConfig(False).build()

						Dim net2 As New MultiLayerNetwork(conf2)
						net2.init()

						assertEquals(net2.params(), net.params())

						'Check params:
						assertEquals(net2.params(), net.params())
						Dim params1 As IDictionary(Of String, INDArray) = net.paramTable()
						Dim params2 As IDictionary(Of String, INDArray) = net2.paramTable()
						assertEquals(params2, params1)

						Dim [in] As INDArray = Nd4j.rand(minibatch, nIn)
						Dim [out] As INDArray = net.output([in])
						Dim outExp As INDArray = net2.output([in])

						assertEquals(outExp, [out])

						'Also check serialization:
						Dim netLoaded As MultiLayerNetwork = TestUtils.testModelSerialization(net)
						Dim outLoaded As INDArray = netLoaded.output([in])

						assertEquals(outExp, outLoaded)


						'Sanity check different minibatch sizes
						[in] = Nd4j.rand(2 * minibatch, nIn)
						[out] = net.output([in])
						outExp = net2.output([in])
						assertEquals(outExp, [out])
					Next a
				Next minibatch
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffDenseBackward()
		Public Overridable Sub testSameDiffDenseBackward()
			Dim nIn As Integer = 3
			Dim nOut As Integer = 4

			For Each workspaces As Boolean In New Boolean(){False, True}

				For Each minibatch As Integer In New Integer(){5, 1}

					Dim afns() As Activation = { Activation.TANH, Activation.SIGMOID, Activation.ELU, Activation.IDENTITY, Activation.SOFTPLUS, Activation.SOFTSIGN, Activation.HARDTANH, Activation.CUBE, Activation.RELU }

					For Each a As Activation In afns
						log.info("Starting test - " & a & " - minibatch " & minibatch & ", workspaces: " & workspaces)
						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).inferenceWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).list().layer((New SameDiffDense.Builder()).nIn(nIn).nOut(nOut).activation(a).build()).layer((New OutputLayer.Builder()).nIn(nOut).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

						Dim netSD As New MultiLayerNetwork(conf)
						netSD.init()

						Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).activation(a).nIn(nIn).nOut(nOut).build()).layer((New OutputLayer.Builder()).nIn(nOut).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

						Dim netStandard As New MultiLayerNetwork(conf2)
						netStandard.init()

						netSD.params().assign(netStandard.params())

						'Check params:
						assertEquals(netStandard.params(), netSD.params())
						assertEquals(netStandard.paramTable(), netSD.paramTable())

						Dim [in] As INDArray = Nd4j.rand(minibatch, nIn)
						Dim l As INDArray = TestUtils.randomOneHot(minibatch, nOut, 12345)
						netSD.Input = [in]
						netStandard.Input = [in]
						netSD.Labels = l
						netStandard.Labels = l

						netSD.computeGradientAndScore()
						netStandard.computeGradientAndScore()

						Dim gSD As Gradient = netSD.gradient()
						Dim gStd As Gradient = netStandard.gradient()

						Dim m1 As IDictionary(Of String, INDArray) = gSD.gradientForVariable()
						Dim m2 As IDictionary(Of String, INDArray) = gStd.gradientForVariable()

						assertEquals(m2.Keys, m1.Keys)

						For Each s As String In m1.Keys
							Dim i1 As INDArray = m1(s)
							Dim i2 As INDArray = m2(s)

							assertEquals(i2, i1, s)
						Next s

						assertEquals(gStd.gradient(), gSD.gradient())

						'Sanity check: different minibatch size
						[in] = Nd4j.rand(2 * minibatch, nIn)
						l = TestUtils.randomOneHot(2 * minibatch, nOut, 12345)
						netSD.Input = [in]
						netStandard.Input = [in]
						netSD.Labels = l
						netStandard.Labels = l

						netSD.computeGradientAndScore()
	'                    netStandard.computeGradientAndScore();
	'                    assertEquals(netStandard.gradient().gradient(), netSD.gradient().gradient());

						'Sanity check on different minibatch sizes:
						Dim newIn As INDArray = Nd4j.vstack([in], [in])
						Dim outMbsd As INDArray = netSD.output(newIn)
						Dim outMb As INDArray = netStandard.output(newIn)
						assertEquals(outMb, outMbsd)
					Next a
				Next minibatch
			Next workspaces
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffDenseTraining()
		Public Overridable Sub testSameDiffDenseTraining()
			Nd4j.Random.setSeed(12345)

			Dim nIn As Integer = 4
			Dim nOut As Integer = 3

			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.ENABLED, WorkspaceMode.NONE}

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).updater(New Adam(0.1)).list().layer((New SameDiffDense.Builder()).nIn(nIn).nOut(5).activation(Activation.TANH).build()).layer((New SameDiffDense.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build()).layer((New OutputLayer.Builder()).nIn(5).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

				Dim netSD As New MultiLayerNetwork(conf)
				netSD.init()

				Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(0.1)).list().layer((New DenseLayer.Builder()).activation(Activation.TANH).nIn(nIn).nOut(5).build()).layer((New DenseLayer.Builder()).activation(Activation.TANH).nIn(5).nOut(5).build()).layer((New OutputLayer.Builder()).nIn(5).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

				Dim netStandard As New MultiLayerNetwork(conf2)
				netStandard.init()

				netSD.params().assign(netStandard.params())

				'Check params:
				assertEquals(netStandard.params(), netSD.params())
				assertEquals(netStandard.paramTable(), netSD.paramTable())

				Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = iter.next()

				Dim outSD As INDArray = netSD.output(ds.Features)
				Dim outStd As INDArray = netStandard.output(ds.Features)

				assertEquals(outStd, outSD)

				For i As Integer = 0 To 2
					netSD.fit(ds)
					netStandard.fit(ds)
					Dim s As String = i.ToString()
					assertEquals(netStandard.getFlattenedGradients(), netSD.getFlattenedGradients(), s)
					assertEquals(netStandard.params(), netSD.params(), s)
					assertEquals(netStandard.Updater.StateViewArray, netSD.Updater.StateViewArray, s)
				Next i

				'Sanity check on different minibatch sizes:
				Dim newIn As INDArray = Nd4j.vstack(ds.Features, ds.Features)
				Dim outMbsd As INDArray = netSD.output(newIn)
				Dim outMb As INDArray = netStandard.output(newIn)
				assertEquals(outMb, outMbsd)
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void gradientCheck()
		Public Overridable Sub gradientCheck()
			Dim nIn As Integer = 4
			Dim nOut As Integer = 4

			For Each workspaces As Boolean In New Boolean(){True, False}
				For Each a As Activation In New Activation(){Activation.TANH, Activation.IDENTITY}

					Dim msg As String = "workspaces: " & workspaces & ", " & a
					Nd4j.Random.setSeed(12345)

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).updater(New NoOp()).trainingWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).inferenceWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).list().layer((New SameDiffDense.Builder()).nIn(nIn).nOut(nOut).activation(a).build()).layer((New SameDiffDense.Builder()).nIn(nOut).nOut(nOut).activation(a).build()).layer((New OutputLayer.Builder()).nIn(nOut).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

					Dim net As New MultiLayerNetwork(conf)
					net.init()

					Dim f As INDArray = Nd4j.rand(3, nIn)
					Dim l As INDArray = TestUtils.randomOneHot(3, nOut)

					log.info("Starting: " & msg)
					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(net, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, f, l)

					assertTrue(gradOK, msg)

					TestUtils.testModelSerialization(net)

					'Sanity check on different minibatch sizes:
					Dim newIn As INDArray = Nd4j.vstack(f, f)
					net.output(newIn)
				Next a
			Next workspaces
		End Sub
	End Class

End Namespace