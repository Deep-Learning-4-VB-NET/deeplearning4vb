Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports SubsetVertex = org.deeplearning4j.nn.conf.graph.SubsetVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports InvalidInputTypeException = org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
Imports org.deeplearning4j.nn.conf.layers
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports TestGraphVertex = org.deeplearning4j.nn.conf.misc.TestGraphVertex
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports FeedForwardToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnnPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.conf

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Computation Graph Configuration Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class ComputationGraphConfigurationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ComputationGraphConfigurationTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test JSON Basic") void testJSONBasic()
		Friend Overridable Sub testJSONBasic()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).dist(New NormalDistribution(0, 1)).updater(New NoOp()).graphBuilder().addInputs("input").appendLayer("firstLayer", (New DenseLayer.Builder()).nIn(4).nOut(5).activation(Activation.TANH).build()).addLayer("outputLayer", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(5).nOut(3).build(), "firstLayer").setOutputs("outputLayer").build()
			Dim json As String = conf.toJson()
			Dim conf2 As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(json, conf2.toJson())
			assertEquals(conf, conf2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test JSON Basic 2") void testJSONBasic2()
		Friend Overridable Sub testJSONBasic2()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input").addLayer("cnn1", (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).nIn(1).nOut(5).build(), "input").addLayer("cnn2", (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).nIn(1).nOut(5).build(), "input").addLayer("max1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).build(), "cnn1", "cnn2").addLayer("dnn1", (New DenseLayer.Builder()).nOut(7).build(), "max1").addLayer("max2", (New SubsamplingLayer.Builder()).build(), "max1").addLayer("output", (New OutputLayer.Builder()).nIn(7).nOut(10).activation(Activation.SOFTMAX).build(), "dnn1", "max2").setOutputs("output").inputPreProcessor("cnn1", New FeedForwardToCnnPreProcessor(32, 32, 3)).inputPreProcessor("cnn2", New FeedForwardToCnnPreProcessor(32, 32, 3)).inputPreProcessor("dnn1", New CnnToFeedForwardPreProcessor(8, 8, 5)).build()
			Dim json As String = conf.toJson()
			Dim conf2 As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(json, conf2.toJson())
			assertEquals(conf, conf2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test JSON With Graph Nodes") void testJSONWithGraphNodes()
		Friend Overridable Sub testJSONWithGraphNodes()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("input1", "input2").addLayer("cnn1", (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).nIn(1).nOut(5).build(), "input1").addLayer("cnn2", (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).nIn(1).nOut(5).build(), "input2").addVertex("merge1", New MergeVertex(), "cnn1", "cnn2").addVertex("subset1", New SubsetVertex(0, 1), "merge1").addLayer("dense1", (New DenseLayer.Builder()).nIn(20).nOut(5).build(), "subset1").addLayer("dense2", (New DenseLayer.Builder()).nIn(20).nOut(5).build(), "subset1").addVertex("add", New ElementWiseVertex(ElementWiseVertex.Op.Add), "dense1", "dense2").addLayer("out", (New OutputLayer.Builder()).nIn(1).nOut(1).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build(), "add").setOutputs("out").build()
			Dim json As String = conf.toJson()
			' System.out.println(json);
			Dim conf2 As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(json, conf2.toJson())
			assertEquals(conf, conf2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Configurations") void testInvalidConfigurations()
		Friend Overridable Sub testInvalidConfigurations()
			' Test no inputs for a layer:
			Try
				Call (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input1").addLayer("dense1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "input1").addLayer("out", (New OutputLayer.Builder()).nIn(2).nOut(2).build()).setOutputs("out").build()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK - exception is good
				log.info(e.ToString())
			End Try
			' Use appendLayer on first layer
			Try
				Call (New NeuralNetConfiguration.Builder()).graphBuilder().appendLayer("dense1", (New DenseLayer.Builder()).nIn(2).nOut(2).build()).addLayer("out", (New OutputLayer.Builder()).nIn(2).nOut(2).build()).setOutputs("out").build()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK - exception is good
				log.info(e.ToString())
			End Try
			' Test no network inputs
			Try
				Call (New NeuralNetConfiguration.Builder()).graphBuilder().addLayer("dense1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "input1").addLayer("out", (New OutputLayer.Builder()).nIn(2).nOut(2).build(), "dense1").setOutputs("out").build()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK - exception is good
				log.info(e.ToString())
			End Try
			' Test no network outputs
			Try
				Call (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input1").addLayer("dense1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "input1").addLayer("out", (New OutputLayer.Builder()).nIn(2).nOut(2).build(), "dense1").build()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK - exception is good
				log.info(e.ToString())
			End Try
			' Test: invalid input
			Try
				Call (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input1").addLayer("dense1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "input1").addLayer("out", (New OutputLayer.Builder()).nIn(2).nOut(2).build(), "thisDoesntExist").setOutputs("out").build()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK - exception is good
				log.info(e.ToString())
			End Try
			' Test: graph with cycles
			Try
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input1").addLayer("dense1", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "input1", "dense3").addLayer("dense2", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "dense1").addLayer("dense3", (New DenseLayer.Builder()).nIn(2).nOut(2).build(), "dense2").addLayer("out", (New OutputLayer.Builder()).nIn(2).nOut(2).lossFunction(LossFunctions.LossFunction.MSE).build(), "dense1").setOutputs("out").build()
				' Cycle detection happens in ComputationGraph.init()
				Dim graph As New ComputationGraph(conf)
				graph.init()
				fail("No exception thrown for invalid configuration")
			Catch e As System.InvalidOperationException
				' OK - exception is good
				log.info(e.ToString())
			End Try
			' Test: input != inputType count mismatch
			Try
				Call (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("input1", "input2").setInputTypes(New InputType.InputTypeRecurrent(10, 12)).addLayer("cnn1", (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).nIn(1).nOut(5).build(), "input1").addLayer("cnn2", (New ConvolutionLayer.Builder(2, 2)).stride(2, 2).nIn(1).nOut(5).build(), "input2").addVertex("merge1", New MergeVertex(), "cnn1", "cnn2").addVertex("subset1", New SubsetVertex(0, 1), "merge1").addLayer("dense1", (New DenseLayer.Builder()).nIn(20).nOut(5).build(), "subset1").addLayer("dense2", (New DenseLayer.Builder()).nIn(20).nOut(5).build(), "subset1").addVertex("add", New ElementWiseVertex(ElementWiseVertex.Op.Add), "dense1", "dense2").addLayer("out", (New OutputLayer.Builder()).nIn(1).nOut(1).activation(Activation.TANH).lossFunction(LossFunctions.LossFunction.MSE).build(), "add").setOutputs("out").build()
				fail("No exception thrown for invalid configuration")
			Catch e As System.ArgumentException
				' OK - exception is good
				log.info(e.ToString())
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Configuration With Runtime JSON Subtypes") void testConfigurationWithRuntimeJSONSubtypes()
		Friend Overridable Sub testConfigurationWithRuntimeJSONSubtypes()
			' Idea: suppose someone wants to use a ComputationGraph with a custom GraphVertex
			' (i.e., one not built into DL4J). Check that this works for JSON serialization
			' using runtime/reflection subtype mechanism in ComputationGraphConfiguration.fromJson()
			' Check a standard GraphVertex implementation, plus a static inner graph vertex
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addVertex("test", New TestGraphVertex(3, 7), "in").addVertex("test2", New StaticInnerGraphVertex(4, 5), "in").setOutputs("test", "test2").build()
			Dim json As String = conf.toJson()
			' System.out.println(json);
			Dim conf2 As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(conf, conf2)
			assertEquals(json, conf2.toJson())
			Dim tgv As TestGraphVertex = CType(conf2.getVertices().get("test"), TestGraphVertex)
			assertEquals(3, tgv.getFirstVal())
			assertEquals(7, tgv.getSecondVal())
			Dim sigv As StaticInnerGraphVertex = CType(conf.getVertices().get("test2"), StaticInnerGraphVertex)
			assertEquals(4, sigv.getFirstVal())
			assertEquals(5, sigv.getSecondVal())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Output Order Doesnt Change When Cloning") void testOutputOrderDoesntChangeWhenCloning()
		Friend Overridable Sub testOutputOrderDoesntChangeWhenCloning()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("out1", (New OutputLayer.Builder()).nIn(1).nOut(1).build(), "in").addLayer("out2", (New OutputLayer.Builder()).nIn(1).nOut(1).build(), "in").addLayer("out3", (New OutputLayer.Builder()).nIn(1).nOut(1).build(), "in").validateOutputLayerConfig(False).setOutputs("out1", "out2", "out3").build()
			Dim cloned As ComputationGraphConfiguration = conf.clone()
			Dim json As String = conf.toJson()
			Dim jsonCloned As String = cloned.toJson()
			assertEquals(json, jsonCloned)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Allow Disconnected Layers") void testAllowDisconnectedLayers()
		Friend Overridable Sub testAllowDisconnectedLayers()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("bidirectional", New Bidirectional((New LSTM.Builder()).activation(Activation.TANH).nOut(10).build()), "in").addLayer("out", (New RnnOutputLayer.Builder()).nOut(6).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).build(), "bidirectional").addLayer("disconnected_layer", New Bidirectional((New LSTM.Builder()).activation(Activation.TANH).nOut(10).build()), "in").setOutputs("out").setInputTypes(New InputType.InputTypeRecurrent(10, 12)).allowDisconnected(True).build()
			Dim graph As New ComputationGraph(conf)
			graph.init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Bidirectional Graph Summary") void testBidirectionalGraphSummary()
		Friend Overridable Sub testBidirectionalGraphSummary()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("bidirectional", New Bidirectional((New LSTM.Builder()).activation(Activation.TANH).nOut(10).build()), "in").addLayer("out", (New RnnOutputLayer.Builder()).nOut(6).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).build(), "bidirectional").setOutputs("out").setInputTypes(New InputType.InputTypeRecurrent(10, 12)).build()
			Dim graph As New ComputationGraph(conf)
			graph.init()
			graph.summary()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Data @EqualsAndHashCode(callSuper = false) @DisplayName("Static Inner Graph Vertex") static class StaticInnerGraphVertex extends org.deeplearning4j.nn.conf.graph.GraphVertex
		<Serializable>
		Friend Class StaticInnerGraphVertex
			Inherits GraphVertex

			Friend firstVal As Integer

			Friend secondVal As Integer

			Public Overrides Function clone() As GraphVertex
				Return New TestGraphVertex(firstVal, secondVal)
			End Function

			Public Overrides Function numParams(ByVal backprop As Boolean) As Long
				Return 0
			End Function

			Public Overrides Function minVertexInputs() As Integer
				Return 1
			End Function

			Public Overrides Function maxVertexInputs() As Integer
				Return 1
			End Function

			Public Overrides Function instantiate(ByVal graph As ComputationGraph, ByVal name As String, ByVal idx As Integer, ByVal paramsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDatatype As DataType) As org.deeplearning4j.nn.graph.vertex.GraphVertex
				Throw New System.NotSupportedException("Not supported")
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.conf.inputs.InputType getOutputType(int layerIndex, org.deeplearning4j.nn.conf.inputs.InputType... vertexInputs) throws org.deeplearning4j.nn.conf.inputs.InvalidInputTypeException
			Public Overrides Function getOutputType(ByVal layerIndex As Integer, ParamArray ByVal vertexInputs() As InputType) As InputType
				Throw New System.NotSupportedException()
			End Function

			Public Overrides Function getMemoryReport(ParamArray ByVal inputTypes() As InputType) As MemoryReport
				Throw New System.NotSupportedException()
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Output Layer") void testInvalidOutputLayer()
		Friend Overridable Sub testInvalidOutputLayer()
	'        
	'        Test case (invalid configs)
	'        1. nOut=1 + softmax
	'        2. mcxent + tanh
	'        3. xent + softmax
	'        4. xent + relu
	'        5. mcxent + sigmoid
	'         
			Dim lf() As LossFunctions.LossFunction = { LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.MCXENT, LossFunctions.LossFunction.XENT, LossFunctions.LossFunction.XENT, LossFunctions.LossFunction.MCXENT }
			Dim nOut() As Integer = { 1, 3, 3, 3, 3 }
			Dim activations() As Activation = { Activation.SOFTMAX, Activation.TANH, Activation.SOFTMAX, Activation.RELU, Activation.SIGMOID }
			For i As Integer = 0 To lf.Length - 1
				For Each lossLayer As Boolean In New Boolean() { False, True }
					For Each validate As Boolean In New Boolean() { True, False }
						Dim s As String = "nOut=" & nOut(i) & ",lossFn=" & lf(i) & ",lossLayer=" & lossLayer & ",validate=" & validate
						If nOut(i) = 1 AndAlso lossLayer Then
							' nOuts are not availabel in loss layer, can't expect it to detect this case
							Continue For
						End If
						Try
							Call (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").layer("1",If(Not lossLayer, (New OutputLayer.Builder()).nIn(10).nOut(nOut(i)).activation(activations(i)).lossFunction(lf(i)).build(), (New LossLayer.Builder()).activation(activations(i)).lossFunction(lf(i)).build()), "0").setOutputs("1").validateOutputLayerConfig(validate).build()
							If validate Then
								fail("Expected exception: " & s)
							End If
						Catch e As DL4JInvalidConfigException
							If validate Then
								assertTrue(e.Message.ToLower().Contains("invalid output"),s)
							Else
								fail("Validation should not be enabled")
							End If
						End Try
					Next validate
				Next lossLayer
			Next i
		End Sub
	End Class

End Namespace