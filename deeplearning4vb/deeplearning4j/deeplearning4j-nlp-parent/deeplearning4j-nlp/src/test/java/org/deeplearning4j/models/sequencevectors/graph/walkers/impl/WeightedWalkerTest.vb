Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports NoEdgeHandling = org.deeplearning4j.models.sequencevectors.graph.enums.NoEdgeHandling
Imports WalkDirection = org.deeplearning4j.models.sequencevectors.graph.enums.WalkDirection
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

Namespace org.deeplearning4j.models.sequencevectors.graph.walkers.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class WeightedWalkerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class WeightedWalkerTest
		Inherits BaseDL4JTest

		Private Shared basicGraph As Graph(Of VocabWord, Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			If basicGraph Is Nothing Then
				' we don't really care about this graph, since it's just basic graph for iteration checks
				basicGraph = New Graph(Of VocabWord, Integer)(10, False, New AbstractVertexFactory(Of VocabWord)())

				For i As Integer = 0 To 9
					basicGraph.getVertex(i).setValue(New VocabWord(i, i.ToString()))

					Dim x As Integer = i + 3
					If x >= 10 Then
						x = 0
					End If
					basicGraph.addEdge(i, x, 1, False)
				Next i

				basicGraph.addEdge(0, 4, 2, False)
				basicGraph.addEdge(0, 4, 4, False)
				basicGraph.addEdge(0, 4, 6, False)
				basicGraph.addEdge(4, 5, 8, False)
				basicGraph.addEdge(1, 3, 6, False)
				basicGraph.addEdge(9, 7, 4, False)
				basicGraph.addEdge(5, 6, 2, False)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicIterator1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasicIterator1()
			Dim walker As GraphWalker(Of VocabWord) = (New WeightedWalker.Builder(Of VocabWord)(basicGraph)).setWalkDirection(WalkDirection.FORWARD_PREFERRED).setWalkLength(10).setNoEdgeHandling(NoEdgeHandling.RESTART_ON_DISCONNECTED).build()

			Dim cnt As Integer = 0
			Do While walker.hasNext()
				Dim sequence As Sequence(Of VocabWord) = walker.next()

				assertNotEquals(Nothing, sequence)
				assertEquals(10, sequence.getElements().size())
				cnt += 1
			Loop

			assertEquals(basicGraph.numVertices(), cnt)
		End Sub

	End Class

End Namespace