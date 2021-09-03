Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.deeplearning4j.nn.rl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestMultiModelGradientApplication extends org.deeplearning4j.BaseDL4JTest
	Public Class TestMultiModelGradientApplication
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientApplyMultiLayerNetwork()
		Public Overridable Sub testGradientApplyMultiLayerNetwork()
			Dim minibatch As Integer = 7
			Dim nIn As Integer = 10
			Dim nOut As Integer = 10

			For Each regularization As Boolean In New Boolean() {False, True}
				For Each u As IUpdater In New IUpdater() {
					New Sgd(0.1),
					New Nesterovs(0.1),
					New Adam(0.1)
				}

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(u).l1(If(regularization, 0.2, 0.0)).l2(If(regularization, 0.3, 0.0)).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(10).build()).layer(1, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(nOut).build()).build()


					Nd4j.Random.setSeed(12345)
					Dim net1GradCalc As New MultiLayerNetwork(conf)
					net1GradCalc.init()

					Nd4j.Random.setSeed(12345)
					Dim net2GradUpd As New MultiLayerNetwork(conf.clone())
					net2GradUpd.init()

					assertEquals(net1GradCalc.params(), net2GradUpd.params())

					Dim f As INDArray = Nd4j.rand(minibatch, nIn)
					Dim l As INDArray = Nd4j.create(minibatch, nOut)
					For i As Integer = 0 To minibatch - 1
						l.putScalar(i, i Mod nOut, 1.0)
					Next i
					net1GradCalc.Input = f
					net1GradCalc.Labels = l

					net2GradUpd.Input = f
					net2GradUpd.Labels = l

					'Calculate gradient in first net, update and apply it in the second
					'Also: calculate gradient in the second net, just to be sure it isn't modified while doing updating on
					' the other net's gradient
					net1GradCalc.computeGradientAndScore()
					net2GradUpd.computeGradientAndScore()

					Dim g As Gradient = net1GradCalc.gradient()
					Dim gBefore As INDArray = g.gradient().dup() 'Net 1 gradient should be modified
					Dim net2GradBefore As INDArray = net2GradUpd.gradient().gradient().dup() 'But net 2 gradient should not be
					net2GradUpd.Updater.update(net2GradUpd, g, 0, 0, minibatch, LayerWorkspaceMgr.noWorkspaces())
					Dim gAfter As INDArray = g.gradient().dup()
					Dim net2GradAfter As INDArray = net2GradUpd.gradient().gradient().dup()

					assertNotEquals(gBefore, gAfter) 'Net 1 gradient should be modified
					assertEquals(net2GradBefore, net2GradAfter) 'But net 2 gradient should not be


					'Also: if we apply the gradient using a subi op, we should get the same final params as if we did a fit op
					' on the original network
					net2GradUpd.params().subi(g.gradient())

					net1GradCalc.fit(f, l)
					assertEquals(net1GradCalc.params(), net2GradUpd.params())


					'=============================
					If Not (TypeOf u Is Sgd) Then
						net2GradUpd.Updater.StateViewArray.assign(net1GradCalc.Updater.StateViewArray)
					End If
					assertEquals(net1GradCalc.params(), net2GradUpd.params())
					assertEquals(net1GradCalc.Updater.StateViewArray, net2GradUpd.Updater.StateViewArray)

					'Remove the next 2 lines: fails - as net 1 is 1 iteration ahead
					net1GradCalc.LayerWiseConfigurations.setIterationCount(0)
					net2GradUpd.LayerWiseConfigurations.setIterationCount(0)

					For i As Integer = 0 To 99
						net1GradCalc.fit(f, l)
						net2GradUpd.fit(f, l)
						assertEquals(net1GradCalc.params(), net2GradUpd.params())
					Next i
				Next u
			Next regularization
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientApplyComputationGraph()
		Public Overridable Sub testGradientApplyComputationGraph()
			Dim minibatch As Integer = 7
			Dim nIn As Integer = 10
			Dim nOut As Integer = 10

			For Each regularization As Boolean In New Boolean() {False, True}
				For Each u As IUpdater In New IUpdater() {
					New Sgd(0.1),
					New Adam(0.1)
				}

					Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(u).l1(If(regularization, 0.2, 0.0)).l2(If(regularization, 0.3, 0.0)).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(10).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "0").addLayer("2", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(nOut).build(), "1").setOutputs("2").build()


					Nd4j.Random.setSeed(12345)
					Dim net1GradCalc As New ComputationGraph(conf)
					net1GradCalc.init()

					Nd4j.Random.setSeed(12345)
					Dim net2GradUpd As New ComputationGraph(conf.clone())
					net2GradUpd.init()

					assertEquals(net1GradCalc.params(), net2GradUpd.params())

					Dim f As INDArray = Nd4j.rand(minibatch, nIn)
					Dim l As INDArray = Nd4j.create(minibatch, nOut)
					For i As Integer = 0 To minibatch - 1
						l.putScalar(i, i Mod nOut, 1.0)
					Next i
					net1GradCalc.Inputs = f
					net1GradCalc.Labels = l

					net2GradUpd.Inputs = f
					net2GradUpd.Labels = l

					'Calculate gradient in first net, update and apply it in the second
					'Also: calculate gradient in the second net, just to be sure it isn't modified while doing updating on
					' the other net's gradient
					net1GradCalc.computeGradientAndScore()
					net2GradUpd.computeGradientAndScore()

					Dim g As Gradient = net1GradCalc.gradient()
					Dim gBefore As INDArray = g.gradient().dup() 'Net 1 gradient should be modified
					Dim net2GradBefore As INDArray = net2GradUpd.gradient().gradient().dup() 'But net 2 gradient should not be
					net2GradUpd.Updater.update(g, 0, 0, minibatch, LayerWorkspaceMgr.noWorkspaces())
					Dim gAfter As INDArray = g.gradient().dup()
					Dim net2GradAfter As INDArray = net2GradUpd.gradient().gradient().dup()

					assertNotEquals(gBefore, gAfter) 'Net 1 gradient should be modified
					assertEquals(net2GradBefore, net2GradAfter) 'But net 2 gradient should not be


					'Also: if we apply the gradient using a subi op, we should get the same final params as if we did a fit op
					' on the original network
					net2GradUpd.params().subi(g.gradient())

					net1GradCalc.fit(New INDArray() {f}, New INDArray() {l})
					assertEquals(net1GradCalc.params(), net2GradUpd.params())

					'=============================
					If Not (TypeOf u Is Sgd) Then
						net2GradUpd.Updater.StateViewArray.assign(net1GradCalc.Updater.StateViewArray)
					End If
					assertEquals(net1GradCalc.params(), net2GradUpd.params())
					assertEquals(net1GradCalc.Updater.StateViewArray, net2GradUpd.Updater.StateViewArray)

					'Remove the next 2 lines: fails - as net 1 is 1 iteration ahead
					net1GradCalc.Configuration.setIterationCount(0)
					net2GradUpd.Configuration.setIterationCount(0)


					For i As Integer = 0 To 99
						net1GradCalc.fit(New INDArray() {f}, New INDArray() {l})
						net2GradUpd.fit(New INDArray() {f}, New INDArray() {l})
						assertEquals(net1GradCalc.params(), net2GradUpd.params())
					Next i
				Next u
			Next regularization
		End Sub

	End Class

End Namespace