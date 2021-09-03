Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports NoEdgeHandling = org.deeplearning4j.models.sequencevectors.graph.enums.NoEdgeHandling
Imports WalkDirection = org.deeplearning4j.models.sequencevectors.graph.enums.WalkDirection
Imports NoEdgesException = org.deeplearning4j.models.sequencevectors.graph.exception.NoEdgesException
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.vertex
Imports org.deeplearning4j.models.sequencevectors.graph.walkers
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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

Namespace org.deeplearning4j.models.sequencevectors.graph.walkers.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class RandomWalkerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class RandomWalkerTest
		Inherits BaseDL4JTest

		Private Shared graph As IGraph(Of VocabWord, Double)
		Private Shared graphBig As IGraph(Of VocabWord, Double)
		Private Shared graphDirected As IGraph(Of VocabWord, Double)

		Protected Friend Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(RandomWalkerTest))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			If graph Is Nothing Then
				graph = New Graph(Of VocabWord, Double)(10, False, New AbstractVertexFactory(Of VocabWord)())

				For i As Integer = 0 To 9
					graph.getVertex(i).setValue(New VocabWord(i, i.ToString()))

					Dim x As Integer = i + 3
					If x >= 10 Then
						x = 0
					End If
					graph.addEdge(i, x, 1.0, False)
				Next i

				graphDirected = New Graph(Of VocabWord, Double)(10, False, New AbstractVertexFactory(Of VocabWord)())

				For i As Integer = 0 To 9
					graphDirected.getVertex(i).setValue(New VocabWord(i, i.ToString()))

					Dim x As Integer = i + 3
					If x >= 10 Then
						x = 0
					End If
					graphDirected.addEdge(i, x, 1.0, True)
				Next i

				graphBig = New Graph(Of VocabWord, Double)(1000, False, New AbstractVertexFactory(Of VocabWord)())

				For i As Integer = 0 To 999
					graphBig.getVertex(i).setValue(New VocabWord(i, i.ToString()))

					Dim x As Integer = i + 3
					If x >= 1000 Then
						x = 0
					End If
					graphBig.addEdge(i, x, 1.0, False)
				Next i
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphCreation() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphCreation()
			Dim graph As New Graph(Of VocabWord, Double)(10, False, New AbstractVertexFactory(Of VocabWord)())

			' we have 10 elements
			assertEquals(10, graph.numVertices())

			For i As Integer = 0 To 9
				Dim vertex As Vertex(Of VocabWord) = graph.getVertex(i)
				assertEquals(Nothing, vertex.getValue())
				assertEquals(i, vertex.vertexID())
			Next i
			assertEquals(10, graph.numVertices())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphTraverseRandom1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphTraverseRandom1()
			Dim walker As RandomWalker(Of VocabWord) = CType((New RandomWalker.Builder(Of VocabWord)(graph)).setNoEdgeHandling(NoEdgeHandling.SELF_LOOP_ON_DISCONNECTED).setWalkLength(3).build(), RandomWalker(Of VocabWord))

			Dim cnt As Integer = 0
			Do While walker.hasNext()
				Dim sequence As Sequence(Of VocabWord) = walker.next()

				assertEquals(3, sequence.getElements().size())
				assertNotEquals(Nothing, sequence)

				For Each word As VocabWord In sequence.getElements()
					assertNotEquals(Nothing, word)
				Next word

				cnt += 1
			Loop

			assertEquals(10, cnt)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphTraverseRandom2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphTraverseRandom2()
			Dim walker As RandomWalker(Of VocabWord) = CType((New RandomWalker.Builder(Of VocabWord)(graph)).setSeed(12345).setNoEdgeHandling(NoEdgeHandling.EXCEPTION_ON_DISCONNECTED).setWalkLength(20).setWalkDirection(WalkDirection.FORWARD_UNIQUE).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).build(), RandomWalker(Of VocabWord))

			Dim cnt As Integer = 0
			Do While walker.hasNext()
				Dim sequence As Sequence(Of VocabWord) = walker.next()

				assertTrue(sequence.getElements().size() <= 10)
				assertNotEquals(Nothing, sequence)

				For Each word As VocabWord In sequence.getElements()
					assertNotEquals(Nothing, word)
				Next word

				cnt += 1
			Loop

			assertEquals(10, cnt)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphTraverseRandom3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphTraverseRandom3()
			Dim walker As RandomWalker(Of VocabWord) = CType((New RandomWalker.Builder(Of VocabWord)(graph)).setNoEdgeHandling(NoEdgeHandling.EXCEPTION_ON_DISCONNECTED).setWalkLength(20).setWalkDirection(WalkDirection.FORWARD_UNIQUE).setNoEdgeHandling(NoEdgeHandling.EXCEPTION_ON_DISCONNECTED).build(), RandomWalker(Of VocabWord))

			Try
				Do While walker.hasNext()
					Dim sequence As Sequence(Of VocabWord) = walker.next()
					logger.info("Sequence: " & sequence)
				Loop

				' if cycle passed without exception - something went bad
				assertTrue(False)
			Catch e As NoEdgesException
				' this cycle should throw exception
			Catch e As Exception
				assertTrue(False)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphTraverseRandom4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphTraverseRandom4()
			Dim walker As RandomWalker(Of VocabWord) = CType((New RandomWalker.Builder(Of VocabWord)(graphBig)).setSeed(12345).setNoEdgeHandling(NoEdgeHandling.EXCEPTION_ON_DISCONNECTED).setWalkLength(20).setWalkDirection(WalkDirection.FORWARD_UNIQUE).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).build(), RandomWalker(Of VocabWord))

			Dim sequence1 As Sequence(Of VocabWord) = walker.next()

			walker.reset(True)

			Dim sequence2 As Sequence(Of VocabWord) = walker.next()

			assertNotEquals(sequence1.getElements().get(0), sequence2.getElements().get(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphTraverseRandom5() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphTraverseRandom5()
			Dim walker As RandomWalker(Of VocabWord) = CType((New RandomWalker.Builder(Of VocabWord)(graphBig)).setWalkLength(20).setWalkDirection(WalkDirection.FORWARD_UNIQUE).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).build(), RandomWalker(Of VocabWord))

			Dim sequence1 As Sequence(Of VocabWord) = walker.next()

			walker.reset(False)

			Dim sequence2 As Sequence(Of VocabWord) = walker.next()

			assertEquals(sequence1.getElements().get(0), sequence2.getElements().get(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphTraverseRandom6() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphTraverseRandom6()
			Dim walker As GraphWalker(Of VocabWord) = (New RandomWalker.Builder(Of VocabWord)(graphDirected)).setWalkLength(20).setWalkDirection(WalkDirection.FORWARD_UNIQUE).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).build()

			Dim sequence As Sequence(Of VocabWord) = walker.next()
			assertEquals("0", sequence.getElements().get(0).getLabel())
			assertEquals("3", sequence.getElements().get(1).getLabel())
			assertEquals("6", sequence.getElements().get(2).getLabel())
			assertEquals("9", sequence.getElements().get(3).getLabel())

			assertEquals(4, sequence.getElements().size())
		End Sub
	End Class

End Namespace