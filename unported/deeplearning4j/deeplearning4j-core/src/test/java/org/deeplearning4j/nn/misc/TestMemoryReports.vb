Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.conf.graph
Imports DuplicateToTimeSeriesVertex = org.deeplearning4j.nn.conf.graph.rnn.DuplicateToTimeSeriesVertex
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports MemoryType = org.deeplearning4j.nn.conf.memory.MemoryType
Imports MemoryUseMode = org.deeplearning4j.nn.conf.memory.MemoryUseMode
Imports FeedForwardToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnnPreProcessor
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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

Namespace org.deeplearning4j.nn.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.WORKSPACES) public class TestMemoryReports extends org.deeplearning4j.BaseDL4JTest
	Public Class TestMemoryReports
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public static java.util.List<org.nd4j.common.primitives.Pair<? extends Layer, org.deeplearning4j.nn.conf.inputs.InputType>> getTestLayers()
		Public Shared ReadOnly Property TestLayers As IList(Of Pair(Of Layer, InputType))
			Get
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: java.util.List<org.nd4j.common.primitives.Pair<? extends Layer, org.deeplearning4j.nn.conf.inputs.InputType>> l = new java.util.ArrayList<>();
				Dim l As IList(Of Pair(Of Layer, InputType)) = New List(Of Pair(Of Layer, InputType))()
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new ActivationLayer.Builder().activation(org.nd4j.linalg.activations.Activation.TANH).build(), org.deeplearning4j.nn.conf.inputs.InputType.feedForward(20)));
				l.Add(New Pair(Of Layer, InputType)((New ActivationLayer.Builder()).activation(Activation.TANH).build(), InputType.feedForward(20)))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new DenseLayer.Builder().nIn(20).nOut(20).build(), org.deeplearning4j.nn.conf.inputs.InputType.feedForward(20)));
				l.Add(New Pair(Of Layer, InputType)((New DenseLayer.Builder()).nIn(20).nOut(20).build(), InputType.feedForward(20)))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new DropoutLayer.Builder().nIn(20).nOut(20).build(), org.deeplearning4j.nn.conf.inputs.InputType.feedForward(20)));
				l.Add(New Pair(Of Layer, InputType)((New DropoutLayer.Builder()).nIn(20).nOut(20).build(), InputType.feedForward(20)))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new EmbeddingLayer.Builder().nIn(1).nOut(20).build(), org.deeplearning4j.nn.conf.inputs.InputType.feedForward(20)));
				l.Add(New Pair(Of Layer, InputType)((New EmbeddingLayer.Builder()).nIn(1).nOut(20).build(), InputType.feedForward(20)))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new OutputLayer.Builder().nIn(20).nOut(20).build(), org.deeplearning4j.nn.conf.inputs.InputType.feedForward(20)));
				l.Add(New Pair(Of Layer, InputType)((New OutputLayer.Builder()).nIn(20).nOut(20).build(), InputType.feedForward(20)))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new LossLayer.Builder().build(), org.deeplearning4j.nn.conf.inputs.InputType.feedForward(20)));
				l.Add(New Pair(Of Layer, InputType)((New LossLayer.Builder()).build(), InputType.feedForward(20)))
    
				'RNN layers:
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new GravesLSTM.Builder().nIn(20).nOut(20).build(), org.deeplearning4j.nn.conf.inputs.InputType.recurrent(20, 30)));
				l.Add(New Pair(Of Layer, InputType)((New GravesLSTM.Builder()).nIn(20).nOut(20).build(), InputType.recurrent(20, 30)))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new LSTM.Builder().nIn(20).nOut(20).build(), org.deeplearning4j.nn.conf.inputs.InputType.recurrent(20, 30)));
				l.Add(New Pair(Of Layer, InputType)((New LSTM.Builder()).nIn(20).nOut(20).build(), InputType.recurrent(20, 30)))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new GravesBidirectionalLSTM.Builder().nIn(20).nOut(20).build(), org.deeplearning4j.nn.conf.inputs.InputType.recurrent(20, 30)));
				l.Add(New Pair(Of Layer, InputType)((New GravesBidirectionalLSTM.Builder()).nIn(20).nOut(20).build(), InputType.recurrent(20, 30)))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: l.add(new org.nd4j.common.primitives.Pair<>(new RnnOutputLayer.Builder().nIn(20).nOut(20).build(), org.deeplearning4j.nn.conf.inputs.InputType.recurrent(20, 30)));
				l.Add(New Pair(Of Layer, InputType)((New RnnOutputLayer.Builder()).nIn(20).nOut(20).build(), InputType.recurrent(20, 30)))
    
				Return l
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public static java.util.List<org.nd4j.common.primitives.Pair<? extends GraphVertex, org.deeplearning4j.nn.conf.inputs.InputType[]>> getTestVertices()
		Public Shared ReadOnly Property TestVertices As IList(Of Pair(Of GraphVertex, InputType()))
			Get
    
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: java.util.List<org.nd4j.common.primitives.Pair<? extends GraphVertex, org.deeplearning4j.nn.conf.inputs.InputType[]>> out = new java.util.ArrayList<>();
				Dim [out] As IList(Of Pair(Of GraphVertex, InputType())) = New List(Of Pair(Of GraphVertex, InputType()))()
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new ElementWiseVertex(ElementWiseVertex.Op.Add), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.feedForward(10), org.deeplearning4j.nn.conf.inputs.InputType.feedForward(10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New ElementWiseVertex(ElementWiseVertex.Op.Add), New InputType() {InputType.feedForward(10), InputType.feedForward(10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new ElementWiseVertex(ElementWiseVertex.Op.Add), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10), org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New ElementWiseVertex(ElementWiseVertex.Op.Add), New InputType() {InputType.recurrent(10, 10), InputType.recurrent(10, 10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new L2NormalizeVertex(), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.feedForward(10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New L2NormalizeVertex(), New InputType() {InputType.feedForward(10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new L2Vertex(), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10), org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New L2Vertex(), New InputType() {InputType.recurrent(10, 10), InputType.recurrent(10, 10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new MergeVertex(), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10), org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New MergeVertex(), New InputType() {InputType.recurrent(10, 10), InputType.recurrent(10, 10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new PreprocessorVertex(new org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnnPreProcessor(1, 10, 1)), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.convolutional(1, 10, 1)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New PreprocessorVertex(New FeedForwardToCnnPreProcessor(1, 10, 1)), New InputType() {InputType.convolutional(1, 10, 1)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new ScaleVertex(1.0), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New ScaleVertex(1.0), New InputType() {InputType.recurrent(10, 10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new ShiftVertex(1.0), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New ShiftVertex(1.0), New InputType() {InputType.recurrent(10, 10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new StackVertex(), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10), org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New StackVertex(), New InputType() {InputType.recurrent(10, 10), InputType.recurrent(10, 10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new UnstackVertex(0, 2), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New UnstackVertex(0, 2), New InputType() {InputType.recurrent(10, 10)}))
    
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new org.deeplearning4j.nn.conf.graph.rnn.DuplicateToTimeSeriesVertex("0"), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10), org.deeplearning4j.nn.conf.inputs.InputType.feedForward(10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New DuplicateToTimeSeriesVertex("0"), New InputType() {InputType.recurrent(10, 10), InputType.feedForward(10)}))
	'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	'ORIGINAL LINE: out.add(new org.nd4j.common.primitives.Pair<>(new org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex("0"), new org.deeplearning4j.nn.conf.inputs.InputType[] {org.deeplearning4j.nn.conf.inputs.InputType.recurrent(10, 10)}));
				[out].Add(New Pair(Of GraphVertex, InputType())(New LastTimeStepVertex("0"), New InputType() {InputType.recurrent(10, 10)}))
    
				Return [out]
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMemoryReportSimple()
		Public Overridable Sub testMemoryReportSimple()

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<org.nd4j.common.primitives.Pair<? extends Layer, org.deeplearning4j.nn.conf.inputs.InputType>> l = getTestLayers();
			Dim l As IList(Of Pair(Of Layer, InputType)) = getTestLayers()


'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (org.nd4j.common.primitives.Pair<? extends Layer, org.deeplearning4j.nn.conf.inputs.InputType> p : l)
			For Each p As Pair(Of Layer, InputType) In l

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, p.First.clone()).layer(1, p.First.clone()).validateOutputLayerConfig(False).build()

				Dim mr As MemoryReport = conf.getMemoryReport(p.Second)
				'            System.out.println(mr.toString());
				'            System.out.println("\n\n");

				'Test to/from JSON + YAML
				Dim json As String = mr.toJson()
				Dim yaml As String = mr.toYaml()

				Dim fromJson As MemoryReport = MemoryReport.fromJson(json)
				Dim fromYaml As MemoryReport = MemoryReport.fromYaml(yaml)

				assertEquals(mr, fromJson)
				assertEquals(mr, fromYaml)
			Next p
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMemoryReportSimpleCG()
		Public Overridable Sub testMemoryReportSimpleCG()

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<org.nd4j.common.primitives.Pair<? extends Layer, org.deeplearning4j.nn.conf.inputs.InputType>> l = getTestLayers();
			Dim l As IList(Of Pair(Of Layer, InputType)) = getTestLayers()


'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (org.nd4j.common.primitives.Pair<? extends Layer, org.deeplearning4j.nn.conf.inputs.InputType> p : l)
			For Each p As Pair(Of Layer, InputType) In l

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", p.First.clone(), "in").addLayer("1", p.First.clone(), "0").setOutputs("1").validateOutputLayerConfig(False).build()

				Dim mr As MemoryReport = conf.getMemoryReport(p.Second)
				'            System.out.println(mr.toString());
				'            System.out.println("\n\n");

				'Test to/from JSON + YAML
				Dim json As String = mr.toJson()
				Dim yaml As String = mr.toYaml()

				Dim fromJson As MemoryReport = MemoryReport.fromJson(json)
				Dim fromYaml As MemoryReport = MemoryReport.fromYaml(yaml)

				assertEquals(mr, fromJson)
				assertEquals(mr, fromYaml)
			Next p
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMemoryReportsVerticesCG()
		Public Overridable Sub testMemoryReportsVerticesCG()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<org.nd4j.common.primitives.Pair<? extends GraphVertex, org.deeplearning4j.nn.conf.inputs.InputType[]>> l = getTestVertices();
			Dim l As IList(Of Pair(Of GraphVertex, InputType())) = getTestVertices()

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: for (org.nd4j.common.primitives.Pair<? extends GraphVertex, org.deeplearning4j.nn.conf.inputs.InputType[]> p : l)
			For Each p As Pair(Of GraphVertex, InputType()) In l
				Dim inputs As IList(Of String) = New List(Of String)()
				Dim i As Integer = 0
				Do While i < p.Second.Length
					inputs.Add(i.ToString())
					i += 1
				Loop

				Dim layerInputs() As String = CType(inputs, List(Of String)).ToArray()
				If TypeOf p.First Is DuplicateToTimeSeriesVertex Then
					layerInputs = New String() {"1"}
				End If

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs(inputs).allowDisconnected(True).addVertex("gv", p.First, layerInputs).setOutputs("gv").build()

				Dim mr As MemoryReport = conf.getMemoryReport(p.Second)
				'            System.out.println(mr.toString());
				'            System.out.println("\n\n");

				'Test to/from JSON + YAML
				Dim json As String = mr.toJson()
				Dim yaml As String = mr.toYaml()

				Dim fromJson As MemoryReport = MemoryReport.fromJson(json)
				Dim fromYaml As MemoryReport = MemoryReport.fromYaml(yaml)

				assertEquals(mr, fromJson)
				assertEquals(mr, fromYaml)
			Next p
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInferInputType()
		Public Overridable Sub testInferInputType()
			Dim l As IList(Of Pair(Of INDArray(), InputType())) = New List(Of Pair(Of INDArray(), InputType()))()
			l.Add(New Pair(Of INDArray(), InputType())(New INDArray() {Nd4j.create(10, 8)}, New InputType() {InputType.feedForward(8)}))
			l.Add(New Pair(Of INDArray(), InputType())(New INDArray() {Nd4j.create(10, 8), Nd4j.create(10, 20)}, New InputType() {InputType.feedForward(8), InputType.feedForward(20)}))
			l.Add(New Pair(Of INDArray(), InputType())(New INDArray() {Nd4j.create(10, 8, 7)}, New InputType() {InputType.recurrent(8, 7)}))
			l.Add(New Pair(Of INDArray(), InputType())(New INDArray() {Nd4j.create(10, 8, 7), Nd4j.create(10, 20, 6)}, New InputType() {InputType.recurrent(8, 7), InputType.recurrent(20, 6)}))

			'Activations order: [m,d,h,w]
			l.Add(New Pair(Of INDArray(), InputType())(New INDArray() {Nd4j.create(10, 8, 7, 6)}, New InputType() {InputType.convolutional(7, 6, 8)}))
			l.Add(New Pair(Of INDArray(), InputType())(New INDArray() {Nd4j.create(10, 8, 7, 6), Nd4j.create(10, 4, 3, 2)}, New InputType() {InputType.convolutional(7, 6, 8), InputType.convolutional(3, 2, 4)}))

			For Each p As Pair(Of INDArray(), InputType()) In l
				Dim act() As InputType = InputType.inferInputTypes(p.First)

				assertArrayEquals(p.Second, act)
			Next p
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateSimple()
		Public Overridable Sub validateSimple()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(20).build()).layer(1, (New DenseLayer.Builder()).nIn(20).nOut(27).build()).build()

			Dim mr As MemoryReport = conf.getMemoryReport(InputType.feedForward(10))

			Dim numParams As Integer = (10 * 20 + 20) + (20 * 27 + 27) '787 -> 3148 bytes
			Dim actSize As Integer = 20 + 27 '47 -> 188 bytes
			Dim total15Minibatch As Integer = numParams + 15 * actSize

			'Fixed: should be just params
			Dim fixedBytes As Long = mr.getTotalMemoryBytes(0, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT)
			Dim varBytes As Long = mr.getTotalMemoryBytes(1, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT) - fixedBytes

			assertEquals(numParams * 4, fixedBytes)
			assertEquals(actSize * 4, varBytes)

			Dim minibatch15 As Long = mr.getTotalMemoryBytes(15, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT)
			assertEquals(total15Minibatch * 4, minibatch15)

			'        System.out.println(fixedBytes + "\t" + varBytes);
			'        System.out.println(mr.toString());

			assertEquals(actSize * 4, mr.getMemoryBytes(MemoryType.ACTIVATIONS, 1, MemoryUseMode.TRAINING, CacheMode.NONE, DataType.FLOAT))
			assertEquals(actSize * 4, mr.getMemoryBytes(MemoryType.ACTIVATIONS, 1, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT))

			Dim inputActSize As Integer = 10 + 20
			assertEquals(inputActSize * 4, mr.getMemoryBytes(MemoryType.ACTIVATION_GRADIENTS, 1, MemoryUseMode.TRAINING, CacheMode.NONE, DataType.FLOAT))
			assertEquals(0, mr.getMemoryBytes(MemoryType.ACTIVATION_GRADIENTS, 1, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT))

			'Variable working memory - due to preout during backprop. But not it's the MAX value, as it can be GC'd or workspaced
			Dim workingMemVariable As Integer = 27
			assertEquals(workingMemVariable * 4, mr.getMemoryBytes(MemoryType.WORKING_MEMORY_VARIABLE, 1, MemoryUseMode.TRAINING, CacheMode.NONE, DataType.FLOAT))
			assertEquals(0, mr.getMemoryBytes(MemoryType.WORKING_MEMORY_VARIABLE, 1, MemoryUseMode.INFERENCE, CacheMode.NONE, DataType.FLOAT))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPreprocessors() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPreprocessors()
			'https://github.com/eclipse/deeplearning4j/issues/4223
			Dim f As File = (New ClassPathResource("4223/CompGraphConfig.json")).TempFileFromArchive
			Dim s As String = FileUtils.readFileToString(f, Charset.defaultCharset())

			Dim conf As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(s)

			conf.getMemoryReport(InputType.convolutional(17,19,19))
		End Sub
	End Class

End Namespace