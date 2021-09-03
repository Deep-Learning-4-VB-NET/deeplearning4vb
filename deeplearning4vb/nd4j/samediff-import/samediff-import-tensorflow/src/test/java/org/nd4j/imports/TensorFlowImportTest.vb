Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports ExecutionMode = org.nd4j.autodiff.execution.conf.ExecutionMode
Imports ExecutorConfiguration = org.nd4j.autodiff.execution.conf.ExecutorConfiguration
Imports OutputMode = org.nd4j.autodiff.execution.conf.OutputMode
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports FlatGraph = org.nd4j.graph.FlatGraph
Imports FlatNode = org.nd4j.graph.FlatNode
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports RectifiedLinear = org.nd4j.linalg.api.ops.impl.scalar.RectifiedLinear
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports HashUtil = org.nd4j.linalg.util.HashUtil
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports GraphDef = org.tensorflow.framework.GraphDef
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

Namespace org.nd4j.imports



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled public class TensorFlowImportTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TensorFlowImportTest
		Inherits BaseNd4jTestWithBackends

		Private Shared configuration As ExecutorConfiguration = ExecutorConfiguration.builder().executionMode(ExecutionMode.SEQUENTIAL).profilingMode(OpExecutioner.ProfilingMode.DISABLED).gatherTimings(True).outputMode(OutputMode.IMPLICIT).build()



		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable WriteOnly Property Up As Nd4jBackend
			Set(ByVal backend As Nd4jBackend)
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub tearDown(ByVal backend As Nd4jBackend)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(False)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testClassHolder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testClassHolder(ByVal backend As Nd4jBackend)
			DifferentialFunctionClassHolder.Instance
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSingleExample_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSingleExample_1(ByVal backend As Nd4jBackend)
			Dim g As val = TFGraphMapper.importGraph(New File("C:\Users\raver\Downloads\mnist.pb"))

			Dim array As val = Nd4j.ones(1, 28, 28)
			g.associateArrayWithVariable(array, "flatten_1_input")

			'g.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/mnist.fb"), ExecutorConfiguration.builder().outputMode(OutputMode.VARIABLE_SPACE).build());

			g.outputAll(Nothing)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssertImport_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssertImport_1(ByVal backend As Nd4jBackend)
			Dim graph As val = TFGraphMapper.importGraph(New File("C:\Users\raver\Downloads\test.pb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMaxImport_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testArgMaxImport_2()
			Dim graph As val = TFGraphMapper.importGraph((New ClassPathResource("/tf_graphs/examples/reductions/argmax3,4,5_-1/frozen_graph.pbtxt")).InputStream)

			graph.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/argmax_macos.fb"), ExecutorConfiguration.builder().outputMode(OutputMode.IMPLICIT).build(), True)

			log.info(graph.asFlatPrint())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArgMaxImport_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testArgMaxImport_1()
			Dim graph As val = TFGraphMapper.importGraph((New ClassPathResource("/tf_graphs/argmax.pb.txt")).InputStream)

			log.info(graph.asFlatPrint())
			Dim result As val = graph.outputAll(Nothing).get(graph.outputs().get(0))

			Dim exp As val = Nd4j.createFromArray(New Long(){2, 2, 2})

			assertEquals(exp, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHashEquality1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHashEquality1(ByVal backend As Nd4jBackend)
			Dim hash As Long = HashUtil.getLongHash("Conv2D")
			assertEquals(-1637140380760460323L, hash)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHashEquality2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHashEquality2(ByVal backend As Nd4jBackend)
			Dim hash As Long = HashUtil.getLongHash("switch")
			assertEquals(-1988317239813741487L, hash)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCustomOps1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCustomOps1(ByVal backend As Nd4jBackend)
			Dim map As val = Nd4j.Executioner.getCustomOperations()

			assertTrue(map.size() > 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void importGraph1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importGraph1()
			Dim graph As SameDiff = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/max_add_2.pb.txt")).InputStream)

			assertNotNull(graph)

			assertEquals(2, graph.variableMap().Count)

			Dim var0 As SDVariable = graph.variableMap()("zeros")
			Dim var1 As SDVariable = graph.variableMap()("ones")

			assertNotNull(var0)
			assertNotNull(var1)

			assertNotNull(var0.Arr)
			assertNotNull(var1.Arr)

			assertEquals(0.0, var0.Arr.sumNumber().doubleValue(), 1e-5)
			assertEquals(12.0, var1.Arr.sumNumber().doubleValue(), 1e-5)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void importGraph2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importGraph2()
			Dim graph As SameDiff = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensorflow_inception_graph.pb")).InputStream)

			assertNotNull(graph)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void importGraph3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importGraph3()
			Dim graph As SameDiff = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/max_log_reg.pb.txt")).InputStream)

			assertNotNull(graph)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testImportIris() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testImportIris()
			Dim graph As SameDiff = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/train_iris.pb")).InputStream)
			assertNotNull(graph)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void importGraph4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importGraph4()
			Dim graph As SameDiff = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/max_multiply.pb.txt")).InputStream)

			assertNotNull(graph)

			Dim p0 As val = Nd4j.create(10, 10).assign(2.0)
			Dim p1 As val = Nd4j.create(10, 10).assign(3.0)

			graph.associateArrayWithVariable(p0,graph.variableMap()("Placeholder"))
			graph.associateArrayWithVariable(p1, graph.variableMap()("Placeholder_1"))


			graph.var("Placeholder", p0)
			graph.var("Placeholder_1", p1)

			Dim res As val = graph.outputAll(Nothing)(graph.outputs()(0))



			assertEquals(6.0, res.meanNumber().doubleValue(), 1e-5)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLenet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLenet()
			''' <summary>
			''' Produced with:
			''' python  ~/anaconda2/lib/python2.7/site-packages/tensorflow/python/tools/freeze_graph.py  --input_graph=graph2.pb.txt  --input_checkpoint=test3.ckpt  --output_graph=graph_frozen2.pb  --output_node_name=output/BiasAdd --input_binary=False
			''' 
			''' </summary>

			Nd4j.create(1)
			Dim rawGraph As val = GraphDef.parseFrom((New ClassPathResource("tf_graphs/lenet_cnn.pb")).InputStream)
			Dim nodeNames As val = rawGraph.getNodeList().Select(Function(node) node.getName()).ToList()
			Console.WriteLine(nodeNames)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/lenet_cnn.pb")).InputStream)


			Dim convNode As val = tg.getVariable("conv2d/kernel")
			assertNotNull(convNode.getArr())
			Dim shape As val = convNode.getShape()
			Console.WriteLine(Arrays.toString(shape))

			' this is NHWC weights. will be changed soon.
			assertArrayEquals(New Long(){5, 5, 1, 32}, shape)
			Console.WriteLine(convNode)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIntermediate2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIntermediate2()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/max_lstm.pb")).InputStream)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIntermediate1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIntermediate1()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensorflow_inception_graph.pb")).InputStream)

			assertTrue(tg.getVariable("input") IsNot Nothing)
			' assertTrue(tg.getVariableSpace().getVariable("input").isPlaceholder());

			Dim ipod As val = Nd4j.read(New DataInputStream((New ClassPathResource("tf_graphs/ipod.nd4")).InputStream))

			tg.setArrayForVariable("input",ipod)

			Dim buffer As val = tg.asFlatBuffers(True)
			assertNotNull(buffer)

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIntermediateLoop1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIntermediateLoop1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/simple_while.pb.txt")).InputStream)

			assertNotNull(tg)


			Dim graph As val = FlatGraph.getRootAsFlatGraph(tg.asFlatBuffers(True))

			assertEquals(6, graph.variablesLength())
	'        assertEquals("alpha/Assign", graph.nodes(0).name());
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testWeirdConvImport(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWeirdConvImport(ByVal backend As Nd4jBackend)
			Dim tg As val = TFGraphMapper.importGraph(New File("/home/agibsonccc/code/raver_tfimport_test1/profiling_conv.pb.txt"))
			assertNotNull(tg)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIntermediateLoop3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIntermediateLoop3()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/nested_while.pb.txt")).InputStream)

			assertNotNull(tg)

			' now converting to FlatBuffer
			Dim fb As val = tg.asFlatBuffers(True)
			assertNotNull(fb)

			Dim graph As val = FlatGraph.getRootAsFlatGraph(fb)
			assertEquals(15, graph.variablesLength())

			'assertEquals("phi/Assign", graph.nodes(0).name());
			'assertEquals("alpha/Assign", graph.nodes(1).name());

			assertEquals(2, graph.nodes(0).inputPairedLength())
			assertEquals(2, graph.nodes(1).inputPairedLength())

			'   tg.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/nested_while.fb"));
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testIntermediateStridedSlice1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIntermediateStridedSlice1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensor_slice.pb.txt")).InputStream)

			assertNotNull(tg)

			Dim constIn As val = tg.getVariable("StridedSlice/input")
			assertNotNull(constIn)

			Dim arr As val = tg.getArrForVarName(constIn.name())
			assertEquals(139.5, arr.sumNumber().doubleValue(), 1e-5)


			' now converting to FlatBuffer
			Dim fb As val = tg.asFlatBuffers(True)
			assertNotNull(fb)

			Dim graph As val = FlatGraph.getRootAsFlatGraph(fb)
			assertEquals(5, graph.variablesLength())

			Dim nodeSlice As val = graph.nodes(0)

			assertEquals(14, nodeSlice.extraIntegerLength())

			Dim begin_mask As val = nodeSlice.extraInteger(0)
			Dim ellipsis_mask As val = nodeSlice.extraInteger(1)
			Dim end_mask As val = nodeSlice.extraInteger(2)
			Dim new_axis_mask As val = nodeSlice.extraInteger(3)
			Dim shrink_axis_mask As val = nodeSlice.extraInteger(4)

			assertEquals(0, begin_mask)
			assertEquals(0, ellipsis_mask)
			assertEquals(0, end_mask)
			assertEquals(0, new_axis_mask)
			assertEquals(0, shrink_axis_mask)

			Dim nodeSum As val = graph.nodes(1)

	'        assertEquals("StridedSlice", nodeSlice.name());
	'        assertEquals("Sum", nodeSum.name());
	'
			assertEquals(4, nodeSlice.inputPairedLength())
			assertEquals(2, nodeSum.inputPairedLength())

			' we expect these inputs to be 5:0 and 6:0 respectively
			' where 5 (or 6) is a graph node id
			' and :0 is graph node output index, which is 0 because that's predefined variables
			' P.s. nodeSlice.id() should be equal to 5 :)
			Dim in0 As val = nodeSum.inputPaired(0)
			Dim in1 As val = nodeSum.inputPaired(1)
	'
	'        assertEquals(5, nodeSlice.id());
	'        assertEquals(7, nodeSum.id());
	'
	'        assertEquals(nodeSlice.id(), in0.first());
	'        assertEquals(5, in0.first());
	'
	'        assertEquals(6, in1.first());
	'        assertEquals(0, in1.second());
	'

	'         tg.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/tensor_slice.fb"), ExecutorConfiguration.builder().outputMode(OutputMode.IMPLICIT).build());
	'
	'        val executioner = new NativeGraphExecutioner();
	'
	'        val exp = Nd4j.create(3, 1).assign(3);
	'
	'        val results = executioner.executeGraph(tg, configuration);
	'
	'        assertNotNull(results);
	'        assertEquals(1, results.length);
	'        assertEquals(73.5f, results[0].getFloat(0), 1e-5f);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testIntermediateTensorArraySimple1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIntermediateTensorArraySimple1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensor_array.pb.txt")).InputStream)
			tg.setArrayForVariable("input_matrix",Nd4j.ones(3,2))

			assertNotNull(tg)

			Dim firstSlice As val = tg.getVariable("strided_slice")


			Dim fb As val = tg.asFlatBuffers(True)
			assertNotNull(fb)

			Dim graph As val = FlatGraph.getRootAsFlatGraph(fb)
			assertEquals(36, graph.variablesLength())

			assertTrue(graph.nodesLength() > 1)
	'        assertEquals("strided_slice", graph.nodes(0).name());
	'        assertEquals("TensorArray", graph.nodes(1).name());
	'
			'   assertEquals(4, graph.nodes(0).inputPairedLength());

			'tg.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/tensor_array.fb"));
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testIntermediateTensorArrayLoop1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIntermediateTensorArrayLoop1()
			Dim input As val = Nd4j.linspace(1, 10, 10, DataType.FLOAT).reshape(ChrW(5), 2)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensor_array_loop.pb.txt")).InputStream)
			tg.setArrayForVariable("input_matrix",input)
			assertNotNull(tg)

			Dim fb As val = tg.asFlatBuffers(True)
			assertNotNull(fb)

			Dim graph As val = FlatGraph.getRootAsFlatGraph(fb)
			assertEquals(12, graph.variablesLength())

			Dim strided_slice As val = graph.nodes(0)

	'        assertEquals("strided_slice", strided_slice.name());
	'        assertEquals("TensorArray", graph.nodes(1).name());
	'
			assertEquals(4, strided_slice.inputPairedLength())


			' we expect these inputs to be 1:0, 2:0, 3:0 and 4:0 respectively
			' where 1 (or 2/3/4) is a graph node id
			' and :0 is graph node output index, which is 0 because that's predefined variables
			Dim in0 As val = strided_slice.inputPaired(0)
			Dim in1 As val = strided_slice.inputPaired(1)
			Dim in2 As val = strided_slice.inputPaired(2)
			Dim in3 As val = strided_slice.inputPaired(3)

			assertEquals(2, in0.first())
			assertEquals(0, in0.second())

			assertEquals(3, in1.first())
			assertEquals(0, in1.second())

			assertEquals(4, in2.first())
			assertEquals(0, in2.second())

			assertEquals(5, in3.first())
			assertEquals(0, in3.second())
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIntermediateReduction() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIntermediateReduction()
			Nd4j.create(1)
			Dim tg As SameDiff = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/reduce_dim.pb.txt")).InputStream)
			Dim sumResultVar As SDVariable = tg.getVariable("Sum")

	'        val func = tg.getFunctionForVertexId(sumResultVar.getVertexId());
	'        assertEquals(0,func.getDimensions()[0]);
	'        assertEquals(3,tg.variables().size());
	'        assertNotNull(sumResultVar);
	'        assertNotNull(tg.getFunctionForVertexId(sumResultVar.getVertexId()));
	'        System.out.println(tg.variables());
	'
	'        assertNotNull(func.getDimensions());
	'        assertEquals(0,func.getDimensions()[0]);

			Dim fb As ByteBuffer = tg.asFlatBuffers(True)
			assertNotNull(fb)

			Dim graph As FlatGraph = FlatGraph.getRootAsFlatGraph(fb)
			assertEquals(1, graph.nodesLength())
			assertEquals(2, graph.variablesLength())

			assertEquals("Sum", graph.nodes(0).name())

			Dim nodeSum As FlatNode = graph.nodes(0)
			assertEquals(2, nodeSum.inputPairedLength())


			' we expect these inputs to be 1:0 and 2:0 respectively
			' where 1 (or 2) is a graph node id
			' and :0 is graph node output index, which is 0 because that's predefined variables
			Dim in0 As val = nodeSum.inputPaired(0)
			Dim in1 As val = nodeSum.inputPaired(1)

			assertEquals(1, in0.first())
			assertEquals(0, in0.second())

			assertEquals(2, in1.first())
			assertEquals(0, in1.second())

	'        System.out.println(tg.summary());
			tg.summary()

			Dim dimensionsLength As Integer = nodeSum.dimensionsLength()
			assertEquals(1, dimensionsLength)
			Dim d As Integer = nodeSum.dimensions(0)
			assertEquals(1, d)


			'log.info("nodeSum inputs length: {}; inputPaired length: {}",nodeSum.inputLength(), nodeSum.inputPairedLength());

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/reduce_dim.fb"))

	'        val executioner = new NativeGraphExecutioner();
	'
	'        val exp = Nd4j.create(3, 1).assign(3);
	'
	'        val results = executioner.executeGraph(tg, configuration);
	'
	'        assertNotNull(results);
	'        assertEquals(1, results.length);
	'        assertEquals(exp, results[0]);
	'        
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDefaultArgs(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDefaultArgs(ByVal backend As Nd4jBackend)
			Dim op As val = New RectifiedLinear()

			Dim extras As val = op.extraArgs()
			assertTrue(extras.length = 1)
			Dim value As val = CType(extras(0), Double?)

			assertEquals(0.0f, value.floatValue(), 1e-5f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInferShape() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInferShape()
			''' <summary>
			''' node {
			''' name: "input"
			''' op: "Placeholder"
			''' attr {
			''' key: "dtype"
			''' value {
			''' type: DT_FLOAT
			''' }
			''' }
			''' attr {
			''' key: "shape"
			''' value {
			''' shape {
			''' dim {
			''' size: -1
			''' }
			''' dim {
			''' size: 4
			''' }
			''' }
			''' }
			''' }
			''' }
			''' node {
			''' name: "bias"
			''' op: "Const"
			''' attr {
			''' key: "dtype"
			''' value {
			''' type: DT_FLOAT
			''' }
			''' }
			''' attr {
			''' key: "value"
			''' value {
			''' tensor {
			''' dtype: DT_FLOAT
			''' tensor_shape {
			''' dim {
			''' size: 4
			''' }
			''' }
			''' tensor_content: "\000\000\200?\000\000\000@\000\000@@\000\000\200@"
			''' }
			''' }
			''' }
			''' }
			''' node {
			''' name: "bias/read"
			''' op: "Identity"
			''' input: "bias"
			''' attr {
			''' key: "_class"
			''' value {
			''' list {
			''' s: "loc:@bias"
			''' }
			''' }
			''' }
			''' attr {
			''' key: "T"
			''' value {
			''' type: DT_FLOAT
			''' }
			''' }
			''' }
			''' node {
			''' name: "output"
			''' op: "BiasAdd"
			''' input: "input"
			''' input: "bias/read"
			''' attr {
			''' key: "data_format"
			''' value {
			''' s: "NHWC"
			''' }
			''' }
			''' attr {
			''' key: "T"
			''' value {
			''' type: DT_FLOAT
			''' }
			''' }
			''' }
			''' library {
			''' }
			''' 
			''' </summary>
			Dim graph As SameDiff = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/bias_add/frozen_model.pb")).InputStream)
			assertNotNull(graph)

			Dim input As INDArray = Nd4j.linspace(1,40,40, DataType.FLOAT).reshape(ChrW(10), 4)
			Dim expectedOutput As INDArray = Nd4j.linspace(1,40,40, DataType.FLOAT).reshape(ChrW(10), 4).addRowVector(Nd4j.linspace(1,4,4, DataType.FLOAT))
			Dim actual As INDArray = graph.outputSingle(Collections.singletonMap("input",input), graph.outputs()(0))
			assertEquals(input,graph.getVariable("input").Arr)
			assertEquals(expectedOutput,actual)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testImportMapping1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testImportMapping1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/ae_00/frozen_model.pb")).InputStream)

			Dim variables As val = New Dictionary(Of String, SDVariable)()
			For Each var As val In tg.variables()
				variables.put(var.name(), var)
			Next var

			Dim functions As val = New Dictionary(Of String, DifferentialFunction)()
			For Each func As val In tg.ops()
				Dim ownName As val = func.getOwnName()
				Dim outName As String = func.outputVariables()(0).name()

				assertTrue(variables.containsKey(ownName),"Missing ownName: [" & ownName & "]")
				assertEquals(ownName, outName)
			Next func
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCondMapping1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCondMapping1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simpleif_0/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/simpleif_0_1.fb"))
	'
	'        //log.info("{}", tg.asFlatPrint());
	'        val array = tg.execAndEndResult();
	'        val exp = Nd4j.create(2, 2).assign(-2);
	'        assertNotNull(array);
	'        assertEquals(exp, array);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCondMapping2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCondMapping2()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simpleif_0/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			Dim input As val = Nd4j.create(2, 2).assign(-1)
			tg.associateArrayWithVariable(input, tg.getVariable("input_0"))
			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/simpleif_0.fb"))

			'log.info("{}", tg.asFlatPrint());
			Dim array As val = tg.outputAll(Nothing).get(tg.outputs().get(0))
			Dim exp As val = Nd4j.create(2, 2).assign(1)
			assertNotNull(array)
			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhileMapping1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWhileMapping1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simplewhile_0/frozen_model.pb")).InputStream)
			assertNotNull(tg)
			Dim input As val = Nd4j.create(2, 2).assign(1)
			tg.associateArrayWithVariable(input, tg.getVariable("input_0"))

			'tg.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/simplewhile_0_3.fb"));

			'log.info("{}", tg.asFlatPrint());


			Dim array As val = tg.outputAll(Nothing).get(tg.outputs().get(0))
			Dim exp As val = Nd4j.create(2, 2).assign(1)
			assertNotNull(array)
			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhileMapping2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWhileMapping2()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simplewhile_0/frozen_model.pb")).InputStream)
			assertNotNull(tg)
			Dim input As val = Nd4j.scalar(4.0)
			tg.associateArrayWithVariable(input, tg.getVariable("input_1"))

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/simplewhile_0_4.fb"))

			'log.info("{}", tg.asFlatPrint());
	'
	'        val array = tg.outputAll(null).get(tg.outputs().get(0));
	'        val exp = Nd4j.create(2, 2).assign(2);
	'        assertNotNull(array);
	'        assertEquals(exp, array);
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhileMapping3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWhileMapping3()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simplewhile_0/frozen_model.pb")).InputStream)
			assertNotNull(tg)
			Dim input As val = Nd4j.scalar(9.0)
			tg.associateArrayWithVariable(input, tg.getVariable("input_1"))

			'tg.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/simplewhile_0.fb"));

			'log.info("{}", tg.asFlatPrint());

			Dim array As val = tg.outputAll(Nothing).get(tg.outputs().get(0))
			Dim exp As val = Nd4j.create(2, 2).assign(4)
			assertNotNull(array)
			assertEquals(exp, array)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhileDualMapping1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWhileDualMapping1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simplewhile_1/frozen_model.pb")).InputStream)
			assertNotNull(tg)
			Dim input0 As val = Nd4j.create(2, 2).assign(-4.0)
			Dim input1 As val = Nd4j.scalar(1.0)
			tg.associateArrayWithVariable(input0, tg.getVariable("input_0"))
			tg.associateArrayWithVariable(input1, tg.getVariable("input_1"))

			'tg.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/simplewhile_1.fb"));

			'log.info("{}", tg.asFlatPrint());

			Dim array As INDArray = tg.outputAll(Nothing).get(tg.outputs().get(0))
			Dim exp As val = Nd4j.create(2, 2).assign(-1)
			assertNotNull(array)
			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWhileDualMapping2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWhileDualMapping2()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simplewhile_1/frozen_model.pb")).InputStream)
			assertNotNull(tg)
			Dim input0 As val = Nd4j.create(2, 2).assign(-9.0)
			Dim input1 As val = Nd4j.scalar(1.0)
			tg.associateArrayWithVariable(input0, tg.getVariable("input_0"))
			tg.associateArrayWithVariable(input1, tg.getVariable("input_1"))

			'tg.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/simplewhile_1.fb"));

			'log.info("{}", tg.asFlatPrint());

			Dim array As val = tg.outputAll(Nothing).get(tg.outputs().get(0))
			Dim exp As val = Nd4j.create(2, 2).assign(-3)
			assertNotNull(array)
			assertEquals(exp, array)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMixedWhileCond1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMixedWhileCond1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simplewhile_nested/frozen_model.pb")).InputStream)
			assertNotNull(tg)
			Dim input0 As val = Nd4j.create(2, 2).assign(1.0)
			Dim input1 As val = Nd4j.create(3, 3).assign(2.0)
			tg.associateArrayWithVariable(input0, tg.getVariable("input_0"))
			tg.associateArrayWithVariable(input1, tg.getVariable("input_1"))

			'tg.asFlatFile(new File("../../../libnd4j/tests_cpu/resources/simplewhile_nested.fb"));


			'log.info("{}", tg.asFlatPrint());

			Dim m As IDictionary(Of String, INDArray) = tg.outputAll(Nothing)
			Dim array As val = m(tg.outputs().get(0))
			'val array = tg.getVariable("output").getArr();
			Dim exp As val = Nd4j.create(2, 2).assign(15.0)
			assertNotNull(array)
			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testProfConv() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testProfConv()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph(New File("/home/raver119/develop/workspace/models/profiling_conv.pb.txt"))
			assertNotNull(tg)

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/profiling_conv.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testCrash_119_matrix_diag() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCrash_119_matrix_diag()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/partition_stitch_misc/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			Dim input0 As val = Nd4j.create(2, 5, 4).assign(1.0)
			Dim input1 As val = Nd4j.create(2, 3, 5, 4).assign(2.0)
			Dim input2 As val = Nd4j.create(3, 1, 5, 4).assign(3.0)
			tg.associateArrayWithVariable(input0, tg.getVariable("input_0"))
			tg.associateArrayWithVariable(input1, tg.getVariable("input_1"))
			tg.associateArrayWithVariable(input2, tg.getVariable("input_2"))


			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/partition_stitch_misc.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testCrash_119_tensor_dot_misc() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCrash_119_tensor_dot_misc()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/tensor_dot_misc/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			Dim input0 As val = Nd4j.create(36, 3, 4, 5).assign(1.0)
			Dim input1 As val = Nd4j.create(5, 5, 3, 4).assign(2.0)

			tg.associateArrayWithVariable(input0, tg.getVariable("input_a"))
			tg.associateArrayWithVariable(input1, tg.getVariable("input_b"))

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/tensor_dot_misc.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testCrash_119_transpose() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCrash_119_transpose()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/transpose/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			Dim input0 As val = Nd4j.create(New Double(){0.98114507, 0.96400015, 0.58669623, 0.60073098, 0.75425418, 0.44258752, 0.76373084, 0.96593234, 0.34067846}, New Integer() {3, 3})
			Dim input1 As val = Nd4j.create(New Double(){0.98114507, 0.60073098, 0.76373084, 0.96400015, 0.75425418, 0.96593234, 0.58669623, 0.44258752, 0.34067846}, New Integer() {3, 3})

			tg.associateArrayWithVariable(input0, tg.getVariable("input"))
			tg.associateArrayWithVariable(input1, tg.getVariable("input_1"))

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/transpose.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCrash_119_simpleif_0() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCrash_119_simpleif_0()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/simpleif_0/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			Dim input0 As val = Nd4j.create(New Single() {1, 2, 3, 4}, New Integer() {2, 2})
			Dim input1 As val = Nd4j.scalar(11f)

			tg.associateArrayWithVariable(input0, tg.getVariable("input_0"))
			tg.associateArrayWithVariable(input1, tg.getVariable("input_1"))

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/simpleif_0.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testCrash_119_ae_00() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCrash_119_ae_00()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/ae_00/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			Dim input0 As val = Nd4j.create(New Double() {0.98174960, 0.44406342, 0.50100771, 1.00000000, -0.94038386, 0.46501783, -0.49040590, 0.98153842, -0.00198260, 0.49108310, -0.06085236, 0.93523693, -0.05857396, -0.46633510, -0.02806635, -0.96879626, -0.03938015, -0.51578135, -0.06333921, -1.00000000}, New Integer() {5, 4})

			tg.associateArrayWithVariable(input0, tg.getVariable("input"))

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/ae_00.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testCrash_119_expand_dim() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCrash_119_expand_dim()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/expand_dim/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			Dim input0 As val = Nd4j.create(New Double() {0.09753360, 0.76124972, 0.24693797, 0.13813169, 0.33144656, 0.08299957, 0.67197708, 0.80659380, 0.98274191, 0.63566073, 0.21592326, 0.54902743}, New Integer() {3, 4})

			tg.associateArrayWithVariable(input0, tg.getVariable("input_0"))

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/expand_dim.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCrash_119_reduce_dim_false() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCrash_119_reduce_dim_false()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/reduce_dim.pb.txt")).InputStream)
			assertNotNull(tg)


			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/reduce_dim_false.fb"), ExecutorConfiguration.builder().outputMode(OutputMode.IMPLICIT).build(), True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCrash_119_reduce_dim_true() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCrash_119_reduce_dim_true()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/reduce_dim_true.pb.txt")).InputStream)
			assertNotNull(tg)

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/reduce_dim_true.fb"), ExecutorConfiguration.builder().outputMode(OutputMode.IMPLICIT).build(), True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorArray_119_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTensorArray_119_1()
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensor_array.pb.txt")).InputStream)
			assertNotNull(tg)

			Dim input_matrix As val = Nd4j.ones(3, 2)
			Dim array As val = tg.outputSingle(Collections.singletonMap("input_matrix", input_matrix), tg.outputs().get(0))

			Dim exp As val = Nd4j.create(New Single() {1, 1, 2, 2, 3, 3}, New Integer(){3, 2})

			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorArray_119_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTensorArray_119_2()
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensor_array_read.pb.txt")).InputStream)
			assertNotNull(tg)

			Dim input_matrix As val = Nd4j.ones(3, 2)

			Dim array As val = tg.output(Collections.singletonMap("input_matrix", input_matrix), tg.outputs().get(0)).get(tg.outputs().get(0))

			Dim exp As val = Nd4j.create(New Single() {2, 2}, New Integer(){2})

			assertEquals(exp, array)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorArray_119_3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTensorArray_119_3()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensor_array_unstack.pb.txt")).InputStream)
			assertNotNull(tg)

			Dim array As val = tg.outputSingle(Collections.emptyMap(), tg.outputs().get(0))

			Dim exp As val = Nd4j.create(New Single() {5, 6, 7, 8}, New Integer(){4})

			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTensorArray_119_4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTensorArray_119_4()
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/tensor_array_loop.pb.txt")).InputStream)
			assertNotNull(tg)

			Dim input_matrix As val = Nd4j.linspace(1, 10, 10, DataType.FLOAT).reshape(ChrW(5), 2)
			log.info("Graph: {}", tg.asFlatPrint())
			Dim array As val = tg.outputSingle(Collections.singletonMap("input_matrix", input_matrix), tg.outputs().get(0))

			Dim exp As val = Nd4j.create(New Single() {3, 6, 9, 12, 15, 18, 21, 24, 27, 30}, New Integer(){5, 2})

			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLossImport_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLossImport_1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/losses/log_loss_rank2_axis1_SUM_OVER_BATCH_SIZE/frozen_model.pb")).InputStream)

			tg.outputAll(Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testG_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testG_1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/g_08/frozen_model.pb")).InputStream)

			Dim g As val = tg.asFlatBuffers(True)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBoolImport_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBoolImport_1()
			Nd4j.create(1)
			For e As Integer = 0 To 999
				Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/reduce_any/rank0/frozen_model.pb")).InputStream)

				Dim result As IDictionary(Of String, INDArray) = tg.output(Collections.emptyMap(), tg.outputs())

				assertNotNull(result)
				assertTrue(result.Count > 0)
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogical_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLogical_1()
			Nd4j.create(1)
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/transforms/logicalxor_3,4_3,4/frozen_model.pb")).InputStream)

			tg.outputAll(Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSSD_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSSD_1()
			' tf_graphs/examples/ssd_inception_v2_coco_2018_01_28/frozen_inference_graph.pb
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/ssd_inception_v2_coco_2018_01_28/frozen_inference_graph.pb")).InputStream)
			assertNotNull(tg)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testNonFrozenGraph1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNonFrozenGraph1()
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/unfrozen_simple_ae.pb")).InputStream)
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomGraph() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRandomGraph()
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/assert_equal/scalar_float32/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/scalar_float32.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomGraph2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRandomGraph2()
			Dim tg As val = TFGraphMapper.importGraph(New File("c:\develop\mobilenet_v2_1.0_224_frozen.pb"))
			assertNotNull(tg)

			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/mobilenet_v2.fb"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testRandomGraph3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRandomGraph3()
			Dim tg As val = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/assert_equal/3,4_3,4_float32/frozen_model.pb")).InputStream)
			assertNotNull(tg)

			log.info("{}", tg.asFlatPrint())
			tg.asFlatFile(New File("../../../libnd4j/tests_cpu/resources/assertsomething.fb"))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testControlDependencies1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testControlDependencies1()
			Dim sd As SameDiff = TFGraphMapper.importGraph((New ClassPathResource("tf_graphs/examples/cond/cond_true/frozen_model.pb")).InputStream)



	'        
	'        Control dependencies:
	'        variables:
	'            - cond/LinSpace/start - depends on cond/switch_t
	'            - cond/LinSpace/stop - depends on cond/switch_t
	'            - cond/LinSpace/num - depends on cond/switch_t
	'            - cond/ones - depends on cond/switch_f
	'         

			Dim variables As IDictionary(Of String, Variable) = sd.getVariables()

			assertEquals(variables("cond/LinSpace/start").getControlDeps(), Collections.singletonList("cond/switch_t"))
			assertEquals(variables("cond/LinSpace/stop"), Collections.singletonList("cond/switch_t"))
			assertEquals(variables("cond/LinSpace/num"), Collections.singletonList("cond/switch_t"))
			assertEquals(variables("cond/ones"), Collections.singletonList("cond/switch_f"))
		End Sub
	End Class
End Namespace