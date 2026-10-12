Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.parallelism.inference.observers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class BatchedInferenceObservableTest extends org.deeplearning4j.BaseDL4JTest
	Public Class BatchedInferenceObservableTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub tearDown()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVerticalBatch1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVerticalBatch1()
			Dim observable As New BatchedInferenceObservable()

			For i As Integer = 0 To 31
				observable.addInput(New INDArray(){Nd4j.create(1,100).assign(i)}, Nothing)
			Next i

			assertEquals(1, observable.getInputBatches().Count)

			Dim array As INDArray = observable.getInputBatches()(0).getFirst()(0)
			assertEquals(2, array.rank())

			log.info("Array shape: {}", Arrays.toString(array.shapeInfoDataBuffer().asInt()))

			For i As Integer = 0 To 31
				assertEquals(CSng(i), array.tensorAlongDimension(i, 1).meanNumber().floatValue(), 0.001f)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVerticalBatch2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVerticalBatch2()
			Dim observable As New BatchedInferenceObservable()

			For i As Integer = 0 To 31
				observable.addInput(New INDArray(){Nd4j.create(1,3, 72, 72).assign(i)}, Nothing)
			Next i

			assertEquals(1, observable.getInputBatches().Count)

			Dim array As INDArray = observable.getInputBatches()(0).getFirst()(0)
			assertEquals(4, array.rank())
			assertEquals(32, array.size(0))

			log.info("Array shape: {}", Arrays.toString(array.shapeInfoDataBuffer().asInt()))

			For i As Integer = 0 To 31
				assertEquals(CSng(i), array.tensorAlongDimension(i, 1, 2, 3).meanNumber().floatValue(), 0.001f)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHorizontalBatch1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHorizontalBatch1()
			Dim observable As New BatchedInferenceObservable()

			For i As Integer = 0 To 31
				observable.addInput(New INDArray(){Nd4j.create(3, 72, 72).assign(i), Nd4j.create(3, 100).assign(100 + i)}, Nothing)
			Next i

			assertEquals(1, observable.getInputBatches().Count)

			Dim inputs() As INDArray = observable.getInputBatches()(0).getFirst()

			Dim features0 As INDArray = inputs(0)
			Dim features1 As INDArray = inputs(1)

			assertArrayEquals(New Long() {32*3, 72, 72}, features0.shape())
			assertArrayEquals(New Long() {32*3, 100}, features1.shape())

			For i As Integer = 0 To 31
				For j As Integer = 0 To 2
					assertEquals(CSng(i), features0.tensorAlongDimension(3*i + j, 1, 2).meanNumber().floatValue(), 0.001f)
					assertEquals(CSng(100) + i, features1.tensorAlongDimension(3*i + j, 1).meanNumber().floatValue(), 0.001f)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTearsBatch1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTearsBatch1()
			Dim observable As New BatchedInferenceObservable()
			Dim output0 As INDArray = Nd4j.create(32, 10)
			Dim output1 As INDArray = Nd4j.create(32, 15)
			For i As Integer = 0 To 31
				Dim t0 As INDArray = output0.tensorAlongDimension(i, 1).assign(i)
				Dim t1 As INDArray = output1.tensorAlongDimension(i, 1).assign(i)
				observable.addInput(New INDArray(){t0.reshape(ChrW(1), t0.length()), t1.reshape(ChrW(1), t1.length())}, Nothing)
			Next i

			Dim f As System.Reflection.FieldInfo = GetType(BatchedInferenceObservable).getDeclaredField("outputBatchInputArrays")
			f.setAccessible(True)
			Dim l As IList(Of Integer()) = New List(Of Integer())()
			l.Add(New Integer(){0, 31})
			f.set(observable, l)

			observable.Counter = 32
			observable.OutputBatches = Collections.singletonList(New INDArray(){output0, output1})

			Dim outputs As IList(Of INDArray()) = observable.getOutputs()

			For i As Integer = 0 To 31
				assertEquals(2, outputs(i).Length)

				assertEquals(CSng(i), outputs(i)(0).meanNumber().floatValue(), 0.001f)
				assertEquals(CSng(i), outputs(i)(1).meanNumber().floatValue(), 0.001f)
			Next i
		End Sub
	End Class

End Namespace