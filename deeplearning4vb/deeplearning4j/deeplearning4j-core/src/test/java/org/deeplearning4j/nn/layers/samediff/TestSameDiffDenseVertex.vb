Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports SameDiffDenseVertex = org.deeplearning4j.nn.layers.samediff.testlayers.SameDiffDenseVertex
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
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

Namespace org.deeplearning4j.nn.layers.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) public class TestSameDiffDenseVertex extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSameDiffDenseVertex
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffDenseVertex()
		Public Overridable Sub testSameDiffDenseVertex()

			Dim nIn As Integer = 3
			Dim nOut As Integer = 4

			For Each workspaces As Boolean In New Boolean(){False, True}

				For Each minibatch As Integer In New Integer(){5, 1}

					Dim afns() As Activation = { Activation.TANH, Activation.SIGMOID }

					For Each a As Activation In afns
						log.info("Starting test - " & a & " - minibatch " & minibatch & ", workspaces: " & workspaces)
						Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).trainingWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).inferenceWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).updater(New Sgd(0.0)).graphBuilder().addInputs("in").addVertex("0", New SameDiffDenseVertex(nIn, nOut, a, WeightInit.XAVIER), "in").addVertex("1", New SameDiffDenseVertex(nOut, nOut, a, WeightInit.XAVIER), "0").layer("2", (New OutputLayer.Builder()).nIn(nOut).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "1").setOutputs("2").build()

						Dim netSD As New ComputationGraph(conf)
						netSD.init()

						Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).trainingWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).inferenceWorkspaceMode(If(workspaces, WorkspaceMode.ENABLED, WorkspaceMode.NONE)).updater(New Sgd(0.0)).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(nOut).activation(a).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(nOut).nOut(nOut).activation(a).build(), "0").layer("2", (New OutputLayer.Builder()).nIn(nOut).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "1").setOutputs("2").build()

						Dim netStandard As New ComputationGraph(conf2)
						netStandard.init()

						netSD.params().assign(netStandard.params())

						'Check params:
						assertEquals(netStandard.params(), netSD.params())
						assertEquals(netStandard.paramTable(), netSD.paramTable())

						Dim [in] As INDArray = Nd4j.rand(minibatch, nIn)
						Dim l As INDArray = TestUtils.randomOneHot(minibatch, nOut, 12345)

						Dim outSD As INDArray = netSD.outputSingle([in])
						Dim outStd As INDArray = netStandard.outputSingle([in])

						assertEquals(outStd, outSD)

						netSD.setInput(0, [in])
						netStandard.setInput(0, [in])
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

	'                    System.out.println("========================================================================");

						'Sanity check: different minibatch size
						[in] = Nd4j.rand(2 * minibatch, nIn)
						l = TestUtils.randomOneHot(2 * minibatch, nOut, 12345)
						netSD.Inputs = [in]
						netStandard.Inputs = [in]
						netSD.Labels = l
						netStandard.Labels = l

						netSD.computeGradientAndScore()
						netStandard.computeGradientAndScore()
						assertEquals(netStandard.gradient().gradient(), netSD.gradient().gradient())

						'Check training:
						Dim ds As New DataSet([in], l)
						For i As Integer = 0 To 2
							netSD.fit(ds)
							netStandard.fit(ds)

							assertEquals(netStandard.paramTable(), netSD.paramTable())
							assertEquals(netStandard.params(), netSD.params())
							assertEquals(netStandard.getFlattenedGradients(), netSD.getFlattenedGradients())
						Next i

						'Check serialization:
						Dim loaded As ComputationGraph = TestUtils.testModelSerialization(netSD)

						outSD = loaded.outputSingle([in])
						outStd = netStandard.outputSingle([in])
						assertEquals(outStd, outSD)

						'Sanity check on different minibatch sizes:
						Dim newIn As INDArray = Nd4j.vstack([in], [in])
						Dim outMbsd As INDArray = netSD.output(newIn)(0)
						Dim outMb As INDArray = netStandard.output(newIn)(0)
						assertEquals(outMb, outMbsd)
					Next a
				Next minibatch
			Next workspaces
		End Sub
	End Class

End Namespace