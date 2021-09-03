Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports At = org.nd4j.autodiff.listeners.At
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports org.nd4j.autodiff.samediff.internal
Imports FrameIter = org.nd4j.autodiff.samediff.internal.FrameIter
Imports InferenceSession = org.nd4j.autodiff.samediff.internal.InferenceSession
Imports NoOpMemoryMgr = org.nd4j.autodiff.samediff.internal.memory.NoOpMemoryMgr
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TensorflowFrameworkImporter = org.nd4j.samediff.frameworkimport.tensorflow.importer.TensorflowFrameworkImporter
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

Namespace org.nd4j.autodiff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.SAMEDIFF) public class TestSessions extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestSessions
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInferenceSessionBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInferenceSessionBasic(ByVal backend As Nd4jBackend)
			'So far: trivial test to check execution order

			Dim sd As SameDiff = SameDiff.create()

			Dim ph1 As SDVariable = sd.placeHolder("x", DataType.FLOAT, 3,4)
			Dim ph2 As SDVariable = sd.placeHolder("y", DataType.FLOAT, 1,4)

			Dim [out] As SDVariable = ph1.add("out", ph2)

			'NOTE: normally sessions are internal and completely hidden from users

			Dim [is] As New InferenceSession(sd)

			Dim x As INDArray = Nd4j.linspace(1, 12, 12).castTo(DataType.FLOAT).reshape(ChrW(3), 4)
			Dim y As INDArray = Nd4j.linspace(0.1, 0.4, 4, DataType.DOUBLE).castTo(DataType.FLOAT).reshape(ChrW(1), 4)

			Dim outExp As INDArray = x.addRowVector(y)

			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("x") = x
			m("y") = y

			Dim outMap As IDictionary(Of String, INDArray) = [is].output(Collections.singletonList("out"), m, Nothing, Collections.emptyList(), Nothing, At.defaultAt(Operation.TRAINING))

			assertEquals(1, outMap.Count)
			assertEquals(outExp, outMap("out"))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testInferenceSessionBasic2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testInferenceSessionBasic2(ByVal backend As Nd4jBackend)
			'So far: trivial test to check execution order

			Dim sd As SameDiff = SameDiff.create()
			Dim ph1 As SDVariable = sd.placeHolder("x", DataType.FLOAT, 3,3)
			Dim ph2 As SDVariable = sd.placeHolder("y", DataType.FLOAT, 3,3)

			Dim a As SDVariable = ph1.add("a", ph2)
			Dim b As SDVariable = ph1.mmul("b", ph2)
			Dim c As SDVariable = ph1.sub("c", ph2)
			Dim d As SDVariable = a.add("d", b)

			'To get array d - need to execute: a, b, d - NOT the sub op (c)

			'NOTE: normally sessions are internal and completely hidden from users

			Dim [is] As New InferenceSession(sd)
			Dim x As INDArray = Nd4j.linspace(1, 9, 9).castTo(DataType.FLOAT).reshape(ChrW(3), 3)
			Dim y As INDArray = Nd4j.linspace(0.0, 0.9, 9, DataType.DOUBLE).castTo(DataType.FLOAT).reshape(ChrW(3), 3)

			Dim aExp As INDArray = x.add(y)
			Dim bExp As INDArray = x.mmul(y)
			Dim dExp As INDArray = aExp.add(bExp)

			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("x") = x
			m("y") = y

			Dim outMap As IDictionary(Of String, INDArray) = [is].output(Collections.singletonList("d"), m, Nothing, Collections.emptyList(), Nothing, At.defaultAt(Operation.TRAINING))

			assertEquals(1, outMap.Count)
			assertEquals(dExp, outMap("d"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergeSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergeSimple(ByVal backend As Nd4jBackend)
			'This isn't really a sensible graph, as merge op behaviour is undefined when multiple inputs are available...

			Dim sd As SameDiff = SameDiff.create()
			Dim ph1 As SDVariable = sd.placeHolder("x", DataType.FLOAT, 3,3)
			Dim ph2 As SDVariable = sd.placeHolder("y", DataType.FLOAT, 3,3)

			Dim merge As SDVariable = sd.merge(ph1, ph2)

			Dim outVar As SDVariable = sd.identity(merge)

			Dim x As INDArray = Nd4j.linspace(1, 9, 9).castTo(DataType.FLOAT).reshape(ChrW(3), 3)
			Dim y As INDArray = Nd4j.linspace(0.0, 0.9, 9, DataType.DOUBLE).castTo(DataType.FLOAT).reshape(ChrW(3), 3)
	'        ph1.setArray(x);
	'        ph2.setArray(y);
	'        INDArray out = sd.execAndEndResult();
	'        System.out.println(out);


			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("x") = x
			m("y") = y

			Dim [is] As New InferenceSession(sd)
	'        String outName = merge.name();
			Dim outName As String = outVar.name()
			Dim outMap As IDictionary(Of String, INDArray) = [is].output(Collections.singletonList(outName), m, Nothing, Collections.emptyList(), Nothing, At.defaultAt(Operation.TRAINING))

			assertEquals(1, outMap.Count)
			Dim [out] As INDArray = outMap(outName)
			assertTrue(x.Equals([out]) OrElse y.Equals([out]))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSwitchSimple(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSwitchSimple(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim x As SDVariable = sd.placeHolder("x", DataType.FLOAT, 3,3)
			Dim b As SDVariable = sd.placeHolder("b", DataType.BOOL)

			Dim switchOut() As SDVariable = sd.switchOp(x,b) 'Order: false then true
			Dim falsePlusOne As SDVariable = switchOut(0).add("addFalseBranch", 1)
			Dim truePlusTen As SDVariable = switchOut(1).add("addTrueBranch", 10.0)

			Dim merge As SDVariable = sd.merge(falsePlusOne, truePlusTen)

			Dim xArr As INDArray = Nd4j.create(DataType.FLOAT, 3,3)
			Dim bArr As INDArray = Nd4j.scalar(True)

			Dim expTrue As INDArray = xArr.add(10.0)
			Dim expFalse As INDArray = xArr.add(1.0)

			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("x") = xArr
			m("b") = bArr

			Dim [is] As New InferenceSession(sd)
			Dim n As String = merge.name()

	'        System.out.println("----------------------------------");
			Dim outMap As IDictionary(Of String, INDArray) = [is].output(Collections.singletonList(n), m, Nothing, Collections.emptyList(), Nothing, At.defaultAt(Operation.TRAINING))
			assertEquals(1, outMap.Count)
			assertEquals(expTrue, outMap(n))


	'        System.out.println("----------------------------------");
			'Check false case:
			bArr.assign(0)
			[is] = New InferenceSession(sd)
			outMap = [is].output(Collections.singletonList(n), m, Nothing, Collections.emptyList(), Nothing, At.defaultAt(Operation.TRAINING))
			assertEquals(1, outMap.Count)
			assertEquals(expFalse, outMap(n))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Timeout(20000L) @Tag(org.nd4j.common.tests.tags.TagNames.FILE_IO) @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testSwitchWhile(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSwitchWhile(ByVal backend As Nd4jBackend)

	'        
	'        Test case:
	'        i=0, j=numIter
	'        while(i<j){
	'            i++
	'        }
	'        return (i,j)
	'
	'        Therefore, expected output for 2 nodes is (numIter, numIter)
	'         

			For Each numIter As Integer In New Integer(){1, 3}
				Dim f As File = (New ClassPathResource("tf_graphs/examples/while1/iter_" & numIter & "/frozen_model.pb")).File
				Dim tensorflowFrameworkImporter As New TensorflowFrameworkImporter()
				Dim sd As SameDiff = tensorflowFrameworkImporter.runImport(f.getAbsolutePath(),Collections.emptyMap())



	'            System.out.println("----------------------------------");
				'This particular test/graph doesn't use placeholders
				Dim [is] As New InferenceSession(sd)
				[is].setMmgr(New NoOpMemoryMgr()) 'So arrays aren't deallocated during execution
				Dim n As String = "while/Exit"
				Dim n2 As String = "while/Exit_1"

				Dim m As IDictionary(Of String, INDArray) = [is].output(New List(Of String) From {n, n2}, Collections.emptyMap(), Nothing, Collections.emptyList(), Nothing, At.defaultAt(Operation.TRAINING))
				assertEquals(2, m.Count)

				Dim exp As INDArray = Nd4j.scalar(CSng(numIter))

				assertEquals(exp, m(n))
				assertEquals(exp, m(n2))

				Dim outputs As IDictionary(Of AbstractSession.VarId, INDArray) = [is].getNodeOutputs()
				'Some sanity checks on the internal state:
				'Check 1: "while/Less" should be executed numIter+1 times... i.e., numIter times through the loop, plus once to exit
				Dim i As Integer = 0
				Do While i < numIter + 1
					Dim expVarId As New AbstractSession.VarId("while/Less","while/while_context", i, New FrameIter(AbstractSession.OUTER_FRAME, 0, Nothing))
					Dim expLessVal As INDArray = Nd4j.scalar(i <> numIter)
					assertTrue(outputs.ContainsKey(expVarId))
					assertEquals(expLessVal, outputs(expVarId))
					i += 1
				Loop
				Dim expVarId As New AbstractSession.VarId("while/Less","while/while_context", numIter+1, New FrameIter(AbstractSession.OUTER_FRAME, 0, Nothing))
				assertFalse(outputs.ContainsKey(expVarId))

				'Check 2: Add should be executed numIter times...
				For i As Integer = 0 To numIter - 1
					expVarId = New AbstractSession.VarId("while/add","while/while_context", i, New FrameIter(AbstractSession.OUTER_FRAME, 0, Nothing))
					Dim expAddVal As INDArray = Nd4j.scalar(CSng(i+1)) 'Starts at 0, so post exec it's 1 higher than iter number
					assertTrue(outputs.ContainsKey(expVarId))
					assertEquals(expAddVal, outputs(expVarId))
				Next i
			Next numIter

		End Sub
	End Class

End Namespace