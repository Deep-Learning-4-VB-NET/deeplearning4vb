Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
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

Namespace org.deeplearning4j.nn.transferlearning

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TransferLearningComplex extends org.deeplearning4j.BaseDL4JTest
	Public Class TransferLearningComplex
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMergeAndFreeze()
		Public Overridable Sub testMergeAndFreeze()
			' in1 -> A -> B -> merge, in2 -> C -> merge -> D -> out
			'Goal here: test a number of things...
			' (a) Ensure that freezing C doesn't impact A and B. Only C should be frozen in this config
			' (b) Test global override (should be selective)


			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1e-4)).activation(Activation.LEAKYRELU).graphBuilder().addInputs("in1", "in2").addLayer("A", (New DenseLayer.Builder()).nIn(10).nOut(9).build(), "in1").addLayer("B", (New DenseLayer.Builder()).nIn(9).nOut(8).build(), "A").addLayer("C", (New DenseLayer.Builder()).nIn(7).nOut(6).build(), "in2").addLayer("D", (New DenseLayer.Builder()).nIn(8 + 7).nOut(5).build(), "B", "C").addLayer("out", (New OutputLayer.Builder()).nIn(5).nOut(4).activation(Activation.LEAKYRELU).build(), "D").setOutputs("out").validateOutputLayerConfig(False).build()

			Dim graph As New ComputationGraph(conf)
			graph.init()


			Dim topologicalOrder() As Integer = graph.topologicalSortOrder()
			Dim vertices() As org.deeplearning4j.nn.graph.vertex.GraphVertex = graph.Vertices

			For i As Integer = 0 To topologicalOrder.Length - 1
				Dim v As org.deeplearning4j.nn.graph.vertex.GraphVertex = vertices(topologicalOrder(i))
				log.info(i & vbTab & v.VertexName)
			Next i

			Dim graph2 As ComputationGraph = (New TransferLearning.GraphBuilder(graph)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).updater(New Adam(2e-2)).build()).setFeatureExtractor("C").validateOutputLayerConfig(False).build()

			Dim cFound As Boolean = False
			Dim layers() As Layer = graph2.Layers

			For Each l As Layer In layers
				Dim name As String = l.conf().getLayer().getLayerName()
				log.info(name & vbTab & " frozen: " & (TypeOf l Is FrozenLayer))
				If "C".Equals(l.conf().getLayer().getLayerName()) Then
					'Only C should be frozen in this config
					cFound = True
					assertTrue(TypeOf l Is FrozenLayer, name)
				Else
					assertFalse(TypeOf l Is FrozenLayer, name)
				End If

				'Also check config:
				Dim bl As BaseLayer = (CType(l.conf().getLayer(), BaseLayer))
				assertEquals(New Adam(2e-2), bl.getIUpdater())
				assertEquals(Activation.LEAKYRELU.getActivationFunction(), bl.getActivationFn())
			Next l
			assertTrue(cFound)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSimplerMergeBackProp()
		Public Overridable Sub testSimplerMergeBackProp()

			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.9)).activation(Activation.IDENTITY).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)

	'        
	'                inCentre                inRight
	'                   |                        |
	'             denseCentre0               denseRight0
	'                   |                        |
	'                   |------ mergeRight ------|
	'                                |
	'                              outRight
	'        
	'        

			Dim conf As ComputationGraphConfiguration = overallConf.graphBuilder().addInputs("inCentre", "inRight").addLayer("denseCentre0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "inCentre").addLayer("denseRight0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "inRight").addVertex("mergeRight", New MergeVertex(), "denseCentre0", "denseRight0").addLayer("outRight", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(4).nOut(2).build(), "mergeRight").setOutputs("outRight").build()
			Dim modelToTune As New ComputationGraph(conf)
			modelToTune.init()

			Dim randData As New MultiDataSet(New INDArray() {Nd4j.rand(2, 2), Nd4j.rand(2, 2)}, New INDArray() {Nd4j.rand(2, 2)})
			Dim denseCentre0 As INDArray = modelToTune.feedForward(randData.Features, False)("denseCentre0")
			Dim otherRandData As New MultiDataSet(New INDArray() {denseCentre0, randData.getFeatures(1)}, randData.Labels)

			Dim otherConf As ComputationGraphConfiguration = overallConf.graphBuilder().addInputs("denseCentre0", "inRight").addLayer("denseRight0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "inRight").addVertex("mergeRight", New MergeVertex(), "denseCentre0", "denseRight0").addLayer("outRight", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(4).nOut(2).build(), "mergeRight").setOutputs("outRight").build()
			Dim modelOther As New ComputationGraph(otherConf)
			modelOther.init()
			modelOther.getLayer("denseRight0").Params = modelToTune.getLayer("denseRight0").params()
			modelOther.getLayer("outRight").Params = modelToTune.getLayer("outRight").params()

			modelToTune.getVertex("denseCentre0").setLayerAsFrozen()
			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToTune)).setFeatureExtractor("denseCentre0").build()
			Dim n As Integer = 0
			Do While n < 5
				If n = 0 Then
					'confirm activations out of the merge are equivalent
					assertEquals(modelToTune.feedForward(randData.Features, False)("mergeRight"), modelOther.feedForward(otherRandData.Features, False)("mergeRight"))
					assertEquals(modelNow.feedForward(randData.Features, False)("mergeRight"), modelOther.feedForward(otherRandData.Features, False)("mergeRight"))
				End If
				'confirm activations out of frozen vertex is the same as the input to the other model
				modelOther.fit(otherRandData)
				modelToTune.fit(randData)
				modelNow.fit(randData)

				assertEquals(otherRandData.getFeatures(0), modelNow.feedForward(randData.Features, False)("denseCentre0"))
				assertEquals(otherRandData.getFeatures(0), modelToTune.feedForward(randData.Features, False)("denseCentre0"))

				assertEquals(modelOther.getLayer("denseRight0").params(), modelNow.getLayer("denseRight0").params())
				assertEquals(modelOther.getLayer("denseRight0").params(), modelToTune.getLayer("denseRight0").params())

				assertEquals(modelOther.getLayer("outRight").params(), modelNow.getLayer("outRight").params())
				assertEquals(modelOther.getLayer("outRight").params(), modelToTune.getLayer("outRight").params())
				n += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLessSimpleMergeBackProp()
		Public Overridable Sub testLessSimpleMergeBackProp()

			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.9)).activation(Activation.IDENTITY)

	'        
	'                inCentre                inRight
	'                   |                        |
	'             denseCentre0               denseRight0
	'                   |                        |
	'                   |------ mergeRight ------|
	'                   |            |
	'                 outCentre     outRight
	'        
	'        

			Dim conf As ComputationGraphConfiguration = overallConf.graphBuilder().addInputs("inCentre", "inRight").addLayer("denseCentre0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "inCentre").addLayer("outCentre", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(2).nOut(2).build(),"denseCentre0").addLayer("denseRight0", (New DenseLayer.Builder()).nIn(3).nOut(2).build(), "inRight").addVertex("mergeRight", New MergeVertex(), "denseCentre0", "denseRight0").addLayer("outRight", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(4).nOut(2).build(),"mergeRight").setOutputs("outCentre", "outRight").build()
			Dim modelToTune As New ComputationGraph(conf)
			modelToTune.init()
			modelToTune.getVertex("denseCentre0").setLayerAsFrozen()

			Dim randData As New MultiDataSet(New INDArray() {Nd4j.rand(2, 2), Nd4j.rand(2, 3)}, New INDArray() {Nd4j.rand(2, 2), Nd4j.rand(2, 2)})
			Dim denseCentre0 As INDArray = modelToTune.feedForward(randData.Features, False)("denseCentre0")
			Dim otherRandData As New MultiDataSet(New INDArray() {denseCentre0, randData.getFeatures(1)}, randData.Labels)

			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToTune)).setFeatureExtractor("denseCentre0").build()
			assertTrue(TypeOf modelNow.getLayer("denseCentre0") Is FrozenLayer)
			Dim n As Integer = 0
			Do While n < 5
				'confirm activations out of frozen vertex is the same as the input to the other model
				modelToTune.fit(randData)
				modelNow.fit(randData)

				assertEquals(otherRandData.getFeatures(0), modelNow.feedForward(randData.Features, False)("denseCentre0"))
				assertEquals(otherRandData.getFeatures(0), modelToTune.feedForward(randData.Features, False)("denseCentre0"))

				assertEquals(modelToTune.getLayer("denseRight0").params(), modelNow.getLayer("denseRight0").params())

				assertEquals(modelToTune.getLayer("outRight").params(), modelNow.getLayer("outRight").params())

				assertEquals(modelToTune.getLayer("outCentre").params(), modelNow.getLayer("outCentre").params())
				n += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAddOutput()
		Public Overridable Sub testAddOutput()
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.9)).activation(Activation.IDENTITY)

			Dim conf As ComputationGraphConfiguration = overallConf.graphBuilder().addInputs("inCentre", "inRight").addLayer("denseCentre0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "inCentre").addLayer("denseRight0", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "inRight").addVertex("mergeRight", New MergeVertex(), "denseCentre0", "denseRight0").addLayer("outRight", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(4).nOut(2).build(), "mergeRight").setOutputs("outRight").build()
			Dim modelToTune As New ComputationGraph(conf)
			modelToTune.init()

			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToTune)).addLayer("outCentre", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(2).nOut(3).build(), "denseCentre0").setOutputs("outRight", "outCentre").build()

			assertEquals(2, modelNow.NumOutputArrays)
			Dim rand As New MultiDataSet(New INDArray() {Nd4j.rand(2, 2), Nd4j.rand(2, 2)}, New INDArray() {Nd4j.rand(2, 2), Nd4j.rand(2, 3)})
			modelNow.fit(rand)
	'        log.info(modelNow.summary());
	'        log.info(modelNow.summary(InputType.feedForward(2),InputType.feedForward(2)));
			modelNow.summary()
			modelNow.summary(InputType.feedForward(2),InputType.feedForward(2))
		End Sub
	End Class

End Namespace