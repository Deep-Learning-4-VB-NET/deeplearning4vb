Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports ScaleVertex = org.deeplearning4j.nn.conf.graph.ScaleVertex
Imports ShiftVertex = org.deeplearning4j.nn.conf.graph.ShiftVertex
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports SameDiffSimpleLambdaLayer = org.deeplearning4j.nn.layers.samediff.testlayers.SameDiffSimpleLambdaLayer
Imports SameDiffSimpleLambdaVertex = org.deeplearning4j.nn.layers.samediff.testlayers.SameDiffSimpleLambdaVertex
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.nn.layers.samediff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.SAMEDIFF) @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) public class TestSameDiffLambda extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSameDiffLambda
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffLamdaLayerBasic()
		Public Overridable Sub testSameDiffLamdaLayerBasic()
			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.ENABLED, WorkspaceMode.NONE}
				log.info("--- Workspace Mode: {} ---", wsm)


				Nd4j.Random.setSeed(12345)
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).seed(12345).updater(New Adam(0.01)).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build(), "in").addLayer("1", New SameDiffSimpleLambdaLayer(), "0").addLayer("2", (New OutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "1").setOutputs("2").build()

				'Equavalent, not using SameDiff Lambda:
				Dim confStd As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).seed(12345).updater(New Adam(0.01)).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build(), "in").addVertex("1", New ShiftVertex(1.0), "0").addVertex("2", New ScaleVertex(2.0), "1").addLayer("3", (New OutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "2").setOutputs("3").build()

				Dim lambda As New ComputationGraph(conf)
				lambda.init()

				Dim std As New ComputationGraph(confStd)
				std.init()

				lambda.Params = std.params()

				Dim [in] As INDArray = Nd4j.rand(3, 5)
				Dim labels As INDArray = TestUtils.randomOneHot(3, 5)
				Dim ds As New DataSet([in], labels)

				Dim outLambda As INDArray = lambda.outputSingle([in])
				Dim outStd As INDArray = std.outputSingle([in])

				assertEquals(outLambda, outStd)

				Dim scoreLambda As Double = lambda.score(ds)
				Dim scoreStd As Double = std.score(ds)

				assertEquals(scoreStd, scoreLambda, 1e-6)

				For i As Integer = 0 To 2
					lambda.fit(ds)
					std.fit(ds)

					Dim s As String = i.ToString()
					assertEquals(std.params(), lambda.params(), s)
					assertEquals(std.getFlattenedGradients(), lambda.getFlattenedGradients(), s)
				Next i

				Dim loaded As ComputationGraph = TestUtils.testModelSerialization(lambda)
				outLambda = loaded.outputSingle([in])
				outStd = std.outputSingle([in])

				assertEquals(outStd, outLambda)

				'Sanity check on different minibatch sizes:
				Dim newIn As INDArray = Nd4j.vstack([in], [in])
				Dim outMbsd As INDArray = lambda.output(newIn)(0)
				Dim outMb As INDArray = std.output(newIn)(0)
				assertEquals(outMb, outMbsd)
			Next wsm
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameDiffLamdaVertexBasic()
		Public Overridable Sub testSameDiffLamdaVertexBasic()
			For Each wsm As WorkspaceMode In New WorkspaceMode(){WorkspaceMode.ENABLED, WorkspaceMode.NONE}
				log.info("--- Workspace Mode: {} ---", wsm)

				Nd4j.Random.setSeed(12345)
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).dataType(DataType.DOUBLE).seed(12345).updater(New Adam(0.01)).graphBuilder().addInputs("in1", "in2").addLayer("0", (New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build(), "in1").addLayer("1", (New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build(), "in2").addVertex("lambda", New SameDiffSimpleLambdaVertex(), "0", "1").addLayer("2", (New OutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "lambda").setOutputs("2").build()

				'Equavalent, not using SameDiff Lambda:
				Dim confStd As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).trainingWorkspaceMode(wsm).inferenceWorkspaceMode(wsm).dataType(DataType.DOUBLE).seed(12345).updater(New Adam(0.01)).graphBuilder().addInputs("in1", "in2").addLayer("0", (New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build(), "in1").addLayer("1", (New DenseLayer.Builder()).nIn(5).nOut(5).activation(Activation.TANH).build(), "in2").addVertex("elementwise", New ElementWiseVertex(ElementWiseVertex.Op.Product), "0", "1").addLayer("3", (New OutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "elementwise").setOutputs("3").build()

				Dim lambda As New ComputationGraph(conf)
				lambda.init()

				Dim std As New ComputationGraph(confStd)
				std.init()

				lambda.Params = std.params()

				Dim in1 As INDArray = Nd4j.rand(3, 5)
				Dim in2 As INDArray = Nd4j.rand(3, 5)
				Dim labels As INDArray = TestUtils.randomOneHot(3, 5)
				Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(New INDArray(){in1, in2}, New INDArray(){labels})

				Dim outLambda As INDArray = lambda.output(in1, in2)(0)
				Dim outStd As INDArray = std.output(in1, in2)(0)

				assertEquals(outLambda, outStd)

				Dim scoreLambda As Double = lambda.score(mds)
				Dim scoreStd As Double = std.score(mds)

				assertEquals(scoreStd, scoreLambda, 1e-6)

				For i As Integer = 0 To 2
					lambda.fit(mds)
					std.fit(mds)

					Dim s As String = i.ToString()
					assertEquals(std.params(), lambda.params(), s)
					assertEquals(std.getFlattenedGradients(), lambda.getFlattenedGradients(), s)
				Next i

				Dim loaded As ComputationGraph = TestUtils.testModelSerialization(lambda)
				outLambda = loaded.output(in1, in2)(0)
				outStd = std.output(in1, in2)(0)

				assertEquals(outStd, outLambda)

				'Sanity check on different minibatch sizes:
				Dim newIn1 As INDArray = Nd4j.vstack(in1, in1)
				Dim newIn2 As INDArray = Nd4j.vstack(in2, in2)
				Dim outMbsd As INDArray = lambda.output(newIn1, newIn2)(0)
				Dim outMb As INDArray = std.output(newIn1, newIn2)(0)
				assertEquals(outMb, outMbsd)
			Next wsm
		End Sub
	End Class

End Namespace