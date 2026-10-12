Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports NoEdgeHandling = org.deeplearning4j.models.sequencevectors.graph.enums.NoEdgeHandling
Imports PopularityMode = org.deeplearning4j.models.sequencevectors.graph.enums.PopularityMode
Imports SpreadSpectrum = org.deeplearning4j.models.sequencevectors.graph.enums.SpreadSpectrum
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
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class PopularityWalkerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class PopularityWalkerTest
		Inherits BaseDL4JTest

		Private Shared graph As Graph(Of VocabWord, Double)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
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

				graph.addEdge(0, 4, 1.0, False)
				graph.addEdge(0, 4, 1.0, False)
				graph.addEdge(0, 4, 1.0, False)
				graph.addEdge(4, 5, 1.0, False)
				graph.addEdge(1, 3, 1.0, False)
				graph.addEdge(9, 7, 1.0, False)
				graph.addEdge(5, 6, 1.0, False)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPopularityWalkerCreation() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPopularityWalkerCreation()
			Dim walker As GraphWalker(Of VocabWord) = (New PopularityWalker.Builder(Of VocabWord)(graph)).setWalkDirection(WalkDirection.FORWARD_ONLY).setWalkLength(10).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).build()

			assertEquals("PopularityWalker", walker.GetType().Name)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPopularityWalker1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPopularityWalker1()
			Dim walker As GraphWalker(Of VocabWord) = (New PopularityWalker.Builder(Of VocabWord)(graph)).setWalkDirection(WalkDirection.FORWARD_ONLY).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).setWalkLength(10).setPopularityMode(PopularityMode.MAXIMUM).setPopularitySpread(3).setSpreadSpectrum(SpreadSpectrum.PLAIN).build()

			Console.WriteLine("Connected [3] size: " & graph.getConnectedVertices(3).Count)
			Console.WriteLine("Connected [4] size: " & graph.getConnectedVertices(4).Count)

			Dim sequence As Sequence(Of VocabWord) = walker.next()
			assertEquals("0", sequence.getElements().get(0).getLabel())

			Console.WriteLine("Position at 1: [" & sequence.getElements().get(1).getLabel() & "]")

			assertTrue(sequence.getElements().get(1).getLabel().Equals("4") OrElse sequence.getElements().get(1).getLabel().Equals("7") OrElse sequence.getElements().get(1).getLabel().Equals("9"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPopularityWalker2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPopularityWalker2()
			Dim walker As GraphWalker(Of VocabWord) = (New PopularityWalker.Builder(Of VocabWord)(graph)).setWalkDirection(WalkDirection.FORWARD_ONLY).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).setWalkLength(10).setPopularityMode(PopularityMode.MINIMUM).setPopularitySpread(3).build()

			Console.WriteLine("Connected [3] size: " & graph.getConnectedVertices(3).Count)
			Console.WriteLine("Connected [4] size: " & graph.getConnectedVertices(4).Count)

			Dim sequence As Sequence(Of VocabWord) = walker.next()
			assertEquals("0", sequence.getElements().get(0).getLabel())

			Console.WriteLine("Position at 1: [" & sequence.getElements().get(1).getLabel() & "]")

			assertTrue(sequence.getElements().get(1).getLabel().Equals("8") OrElse sequence.getElements().get(1).getLabel().Equals("3") OrElse sequence.getElements().get(1).getLabel().Equals("9") OrElse sequence.getElements().get(1).getLabel().Equals("7"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPopularityWalker3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPopularityWalker3()
			Dim walker As GraphWalker(Of VocabWord) = (New PopularityWalker.Builder(Of VocabWord)(graph)).setWalkDirection(WalkDirection.FORWARD_ONLY).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).setWalkLength(10).setPopularityMode(PopularityMode.MAXIMUM).setPopularitySpread(3).setSpreadSpectrum(SpreadSpectrum.PROPORTIONAL).build()

			Console.WriteLine("Connected [3] size: " & graph.getConnectedVertices(3).Count)
			Console.WriteLine("Connected [4] size: " & graph.getConnectedVertices(4).Count)

			Dim got4 As New AtomicBoolean(False)
			Dim got7 As New AtomicBoolean(False)
			Dim got9 As New AtomicBoolean(False)

			For i As Integer = 0 To 49
				Dim sequence As Sequence(Of VocabWord) = walker.next()
				assertEquals("0", sequence.getElements().get(0).getLabel())
				Console.WriteLine("Position at 1: [" & sequence.getElements().get(1).getLabel() & "]")

				got4.compareAndSet(False, sequence.getElements().get(1).getLabel().Equals("4"))
				got7.compareAndSet(False, sequence.getElements().get(1).getLabel().Equals("7"))
				got9.compareAndSet(False, sequence.getElements().get(1).getLabel().Equals("9"))

				assertTrue(sequence.getElements().get(1).getLabel().Equals("4") OrElse sequence.getElements().get(1).getLabel().Equals("7") OrElse sequence.getElements().get(1).getLabel().Equals("9"))

				walker.reset(False)
			Next i

			assertTrue(got4.get())
			assertTrue(got7.get())
			assertTrue(got9.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPopularityWalker4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPopularityWalker4()
			Dim walker As GraphWalker(Of VocabWord) = (New PopularityWalker.Builder(Of VocabWord)(graph)).setWalkDirection(WalkDirection.FORWARD_ONLY).setNoEdgeHandling(NoEdgeHandling.CUTOFF_ON_DISCONNECTED).setWalkLength(10).setPopularityMode(PopularityMode.MINIMUM).setPopularitySpread(3).setSpreadSpectrum(SpreadSpectrum.PROPORTIONAL).build()

			Console.WriteLine("Connected [3] size: " & graph.getConnectedVertices(3).Count)
			Console.WriteLine("Connected [4] size: " & graph.getConnectedVertices(4).Count)

			Dim got3 As New AtomicBoolean(False)
			Dim got8 As New AtomicBoolean(False)
			Dim got9 As New AtomicBoolean(False)

			For i As Integer = 0 To 49
				Dim sequence As Sequence(Of VocabWord) = walker.next()
				assertEquals("0", sequence.getElements().get(0).getLabel())
				Console.WriteLine("Position at 1: [" & sequence.getElements().get(1).getLabel() & "]")

				got3.compareAndSet(False, sequence.getElements().get(1).getLabel().Equals("3"))
				got8.compareAndSet(False, sequence.getElements().get(1).getLabel().Equals("8"))
				got9.compareAndSet(False, sequence.getElements().get(1).getLabel().Equals("9"))

				assertTrue(sequence.getElements().get(1).getLabel().Equals("8") OrElse sequence.getElements().get(1).getLabel().Equals("3") OrElse sequence.getElements().get(1).getLabel().Equals("9"))

				walker.reset(False)
			Next i

			assertTrue(got3.get())
			assertTrue(got8.get())
			assertTrue(got9.get())
		End Sub
	End Class

End Namespace