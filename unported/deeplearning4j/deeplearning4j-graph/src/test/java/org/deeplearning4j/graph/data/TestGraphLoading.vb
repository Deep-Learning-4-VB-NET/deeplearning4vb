Imports System.Collections.Generic
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports DelimitedEdgeLineProcessor = org.deeplearning4j.graph.data.impl.DelimitedEdgeLineProcessor
Imports DelimitedVertexLoader = org.deeplearning4j.graph.data.impl.DelimitedVertexLoader
Imports org.deeplearning4j.graph.graph
Imports StringVertexFactory = org.deeplearning4j.graph.vertexfactory.StringVertexFactory
Imports org.deeplearning4j.graph.vertexfactory
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.deeplearning4j.graph.data


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("Permissions issues on CI") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestGraphLoading extends org.deeplearning4j.BaseDL4JTest
	Public Class TestGraphLoading
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testEdgeListGraphLoading() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEdgeListGraphLoading()
			Dim cpr As New ClassPathResource("deeplearning4j-graph/testgraph_7vertices.txt")

			Dim graph As IGraph(Of String, String) = GraphLoader.loadUndirectedGraphEdgeListFile(cpr.TempFileFromArchive.getAbsolutePath(), 7, ",")
	'        System.out.println(graph);

			assertEquals(graph.numVertices(), 7)
			Dim edges()() As Integer = {
				New Integer() {1, 2},
				New Integer() {0, 2, 4},
				New Integer() {0, 1, 3, 4},
				New Integer() {2, 4, 5},
				New Integer() {1, 2, 3, 5, 6},
				New Integer() {3, 4, 6},
				New Integer() {4, 5}
			}

			For i As Integer = 0 To 6
				assertEquals(edges(i).Length, graph.getVertexDegree(i))
				Dim connectedVertices() As Integer = graph.getConnectedVertexIndices(i)
				Dim j As Integer = 0
				Do While j < edges(i).Length
					assertTrue(ArrayUtils.contains(connectedVertices, edges(i)(j)))
					j += 1
				Loop
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testGraphLoading() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphLoading()

			Dim cpr As New ClassPathResource("deeplearning4j-graph/simplegraph.txt")

			Dim edgeLineProcessor As EdgeLineProcessor(Of String) = New DelimitedEdgeLineProcessor(",", False, "//")
			Dim vertexFactory As VertexFactory(Of String) = New StringVertexFactory("v_%d")
			Dim graph As Graph(Of String, String) = GraphLoader.loadGraph(cpr.TempFileFromArchive.getAbsolutePath(), edgeLineProcessor, vertexFactory, 10, False)


	'        System.out.println(graph);

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testGraphLoadingWithVertices() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGraphLoadingWithVertices()

			Dim verticesCPR As New ClassPathResource("deeplearning4j-graph/test_graph_vertices.txt")
			Dim edgesCPR As New ClassPathResource("deeplearning4j-graph/test_graph_edges.txt")


			Dim edgeLineProcessor As EdgeLineProcessor(Of String) = New DelimitedEdgeLineProcessor(",", False, "//")
			Dim vertexLoader As VertexLoader(Of String) = New DelimitedVertexLoader(":", "//")

			Dim graph As Graph(Of String, String) = GraphLoader.loadGraph(verticesCPR.TempFileFromArchive.getAbsolutePath(), edgesCPR.TempFileFromArchive.getAbsolutePath(), vertexLoader, edgeLineProcessor, False)

	'        System.out.println(graph);

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


	End Class

End Namespace