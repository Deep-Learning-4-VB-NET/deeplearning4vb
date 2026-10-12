Imports System.Collections.Generic
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.graph.api
Imports GraphLoader = org.deeplearning4j.graph.data.GraphLoader
Imports org.deeplearning4j.graph.iterator
Imports org.deeplearning4j.graph.iterator
Imports org.deeplearning4j.graph.vertexfactory
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.graph.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.FILE_IO) public class TestGraph extends org.deeplearning4j.BaseDL4JTest
	Public Class TestGraph
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testSimpleGraph()
		Public Overridable Sub testSimpleGraph()

			Dim graph As New Graph(Of String, String)(10, False, New VFactory())

			assertEquals(10, graph.numVertices())

			For i As Integer = 0 To 9
				'Add some undirected edges
				Dim str As String = i & "--" & (i + 1) Mod 10
				Dim edge As New Edge(Of String)(i, (i + 1) Mod 10, str, False)

				graph.addEdge(edge)
			Next i

			For i As Integer = 0 To 9
				Dim edges As IList(Of Edge(Of String)) = graph.getEdgesOut(i)
				assertEquals(2, edges.Count)

				'expect for example 0->1 and 9->0
				Dim first As Edge(Of String) = edges(0)
				If first.getFrom() = i Then
					'undirected edge: i -> i+1 (or 9 -> 0)
					assertEquals(i, first.getFrom())
					assertEquals((i + 1) Mod 10, first.getTo())
				Else
					'undirected edge: i-1 -> i (or 9 -> 0)
					assertEquals((i + 10 - 1) Mod 10, first.getFrom())
					assertEquals(i, first.getTo())
				End If

				Dim second As Edge(Of String) = edges(1)
				assertNotEquals(first.getFrom(), second.getFrom())
				If second.getFrom() = i Then
					'undirected edge: i -> i+1 (or 9 -> 0)
					assertEquals(i, second.getFrom())
					assertEquals((i + 1) Mod 10, second.getTo())
				Else
					'undirected edge: i-1 -> i (or 9 -> 0)
					assertEquals((i + 10 - 1) Mod 10, second.getFrom())
					assertEquals(i, second.getTo())
				End If
			Next i
		End Sub

		Private Class VFactory
			Implements VertexFactory(Of String)

			Public Overridable Function create(ByVal vertexIdx As Integer) As Vertex(Of String)
				Return New Vertex(Of String)(vertexIdx, vertexIdx.ToString())
			End Function
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testRandomWalkIterator()
		Public Overridable Sub testRandomWalkIterator()
			Dim graph As New Graph(Of String, String)(10, False, New VFactory())
			assertEquals(10, graph.numVertices())

			For i As Integer = 0 To 9
				'Add some undirected edges
				Dim str As String = i & "--" & (i + 1) Mod 10
				Dim edge As New Edge(Of String)(i, (i + 1) Mod 10, str, False)

				graph.addEdge(edge)
			Next i

			Dim walkLength As Integer = 4
			Dim iter As New RandomWalkIterator(Of String)(graph, walkLength, 1235, NoEdgeHandling.EXCEPTION_ON_DISCONNECTED)

			Dim count As Integer = 0
			Dim startIdxSet As ISet(Of Integer) = New HashSet(Of Integer)()
			Do While iter.hasNext()
				count += 1
				Dim sequence As IVertexSequence(Of String) = iter.next()
				Dim seqCount As Integer = 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim first As Integer = sequence.next().vertexID()
				Dim previous As Integer = first
				Do While sequence.MoveNext()
					'Possible next vertices for this particular graph: (previous+1)%10 or (previous-1+10)%10
					Dim left As Integer = (previous - 1 + 10) Mod 10
					Dim right As Integer = (previous + 1) Mod 10
					Dim current As Integer = sequence.Current.vertexID()
					assertTrue(current = left OrElse current = right, "expected: " & left & " or " & right & ", got " & current)
					seqCount += 1
					previous = current
				Loop
				assertEquals(seqCount, walkLength + 1) 'walk of 0 -> 1 element, walk of 2 -> 3 elements etc
				assertFalse(startIdxSet.Contains(first)) 'Expect to see each node exactly once
				startIdxSet.Add(first)
			Loop
			assertEquals(10, count) 'Expect exactly 10 starting nodes
			assertEquals(10, startIdxSet.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testWeightedRandomWalkIterator() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWeightedRandomWalkIterator()

			'Load a directed, weighted graph from file
			Dim path As String = (New ClassPathResource("deeplearning4j-graph/WeightedGraph.txt")).TempFileFromArchive.getAbsolutePath()
			Dim numVertices As Integer = 9
			Dim delim As String = ","
			Dim ignoreLinesStartingWith() As String = {"//"} 'Comment lines start with "//"

			Dim graph As IGraph(Of String, Double) = GraphLoader.loadWeightedEdgeListFile(path, numVertices, delim, True, ignoreLinesStartingWith)

			assertEquals(numVertices, graph.numVertices())

			Dim vertexOutDegrees() As Integer = {2, 2, 1, 2, 2, 1, 1, 1, 1}
			For i As Integer = 0 To numVertices - 1
				assertEquals(vertexOutDegrees(i), graph.getVertexDegree(i))
			Next i
			Dim edges()() As Integer = {
				New Integer() {1, 3},
				New Integer() {2, 4},
				New Integer() {5},
				New Integer() {4, 6},
				New Integer() {5, 7},
				New Integer() {8},
				New Integer() {7},
				New Integer() {8},
				New Integer() {0}
			}
			Dim edgeWeights()() As Double = {
				New Double() {1, 3},
				New Double() {12, 14},
				New Double() {25},
				New Double() {34, 36},
				New Double() {45, 47},
				New Double() {58},
				New Double() {67},
				New Double() {78},
				New Double() {80}
			}
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim edgeWeightsNormalized[][] As Double = new Double[edgeWeights.Length][0]
			Dim edgeWeightsNormalized()() As Double = RectangularArrays.RectangularDoubleArray(edgeWeights.Length, 0)
			For i As Integer = 0 To edgeWeights.Length - 1
				Dim sum As Double = 0.0
				Dim j As Integer = 0
				Do While j < edgeWeights(i).Length
					sum += edgeWeights(i)(j)
					j += 1
				Loop
				edgeWeightsNormalized(i) = New Double((edgeWeights(i).Length) - 1){}
				j = 0
				Do While j < edgeWeights(i).Length
					edgeWeightsNormalized(i)(j) = edgeWeights(i)(j) / sum
					j += 1
				Loop
			Next i

			Dim walkLength As Integer = 5
			Dim iterator As New WeightedRandomWalkIterator(Of String)(graph, walkLength, 12345)

			Dim walkCount As Integer = 0
			Dim set As ISet(Of Integer) = New HashSet(Of Integer)()
			Do While iterator.hasNext()
				Dim walk As IVertexSequence(Of String) = iterator.next()
				assertEquals(walkLength + 1, walk.sequenceLength()) 'Walk length of 5 -> 6 vertices (inc starting point)

				Dim thisWalkCount As Integer = 0
				Dim first As Boolean = True
				Dim lastVertex As Integer = -1
				Do While walk.MoveNext()
					Dim vertex As Vertex(Of String) = walk.Current
					If first Then
						assertFalse(set.Contains(vertex.vertexID()))
						set.Add(vertex.vertexID())
						lastVertex = vertex.vertexID()
						first = False
					Else
						'Ensure that a directed edge exists from lastVertex -> vertex
						Dim currVertex As Integer = vertex.vertexID()
						assertTrue(ArrayUtils.contains(edges(lastVertex), currVertex))
						lastVertex = currVertex
					End If

					thisWalkCount += 1
				Loop
				assertEquals(walkLength + 1, thisWalkCount) 'Walk length of 5 -> 6 vertices (inc starting point)
				walkCount += 1
			Loop

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim transitionProb[][] As Double = new Double[numVertices][numVertices]
			Dim transitionProb()() As Double = RectangularArrays.RectangularDoubleArray(numVertices, numVertices)
			Dim nWalks As Integer = 2000
			For i As Integer = 0 To nWalks - 1
				iterator.reset()
				Do While iterator.hasNext()
					Dim seq As IVertexSequence(Of String) = iterator.next()
					Dim last As Integer = -1
					Do While seq.MoveNext()
						Dim curr As Integer = seq.Current.vertexID()
						If last <> -1 Then
							transitionProb(last)(curr) += 1.0
						End If
						last = curr
					Loop
				Loop
			Next i
			For i As Integer = 0 To transitionProb.Length - 1
				Dim sum As Double = 0.0
				Dim j As Integer = 0
				Do While j < transitionProb(i).Length
					sum += transitionProb(i)(j)
					j += 1
				Loop
				j = 0
				Do While j < transitionProb(i).Length
					transitionProb(i)(j) /= sum
					j += 1
				Loop
	'            System.out.println(Arrays.toString(transitionProb[i]));
			Next i

			'Check that transition probs are essentially correct (within bounds of random variation)
			For i As Integer = 0 To numVertices - 1
				For j As Integer = 0 To numVertices - 1
					If Not ArrayUtils.contains(edges(i), j) Then
						assertEquals(0.0, transitionProb(i)(j), 0.0)
					Else
						Dim idx As Integer = ArrayUtils.IndexOf(edges(i), j)
						assertEquals(edgeWeightsNormalized(i)(idx), transitionProb(i)(j), 0.01)
					End If
				Next j
			Next i


			For i As Integer = 0 To numVertices - 1
				assertTrue(set.Contains(i))
			Next i
			assertEquals(numVertices, walkCount)
		End Sub
	End Class

End Namespace