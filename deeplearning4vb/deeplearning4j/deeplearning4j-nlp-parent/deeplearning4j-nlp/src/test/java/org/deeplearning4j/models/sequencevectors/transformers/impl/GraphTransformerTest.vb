Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports NoEdgeHandling = org.deeplearning4j.models.sequencevectors.graph.enums.NoEdgeHandling
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.vertex
Imports org.deeplearning4j.models.sequencevectors.graph.walkers
Imports org.deeplearning4j.models.sequencevectors.graph.walkers.impl
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.models.sequencevectors.transformers.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class GraphTransformerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class GraphTransformerTest
		Inherits BaseDL4JTest

		Private Shared graph As IGraph(Of VocabWord, Double)

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
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGraphTransformer1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphTransformer1()
			Dim walker As GraphWalker(Of VocabWord) = (New RandomWalker.Builder(Of VocabWord)(graph)).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).build()

			Dim transformer As GraphTransformer(Of VocabWord) = (New GraphTransformer.Builder(Of VocabWord)(graph)).setGraphWalker(walker).build()

			Dim iterator As IEnumerator(Of Sequence(Of VocabWord)) = transformer.GetEnumerator()
			Dim cnt As Integer = 0
			Do While iterator.MoveNext()
				Dim sequence As Sequence(Of VocabWord) = iterator.Current
				Console.WriteLine(sequence)
				cnt += 1
			Loop

			assertEquals(10, cnt)
		End Sub
	End Class

End Namespace