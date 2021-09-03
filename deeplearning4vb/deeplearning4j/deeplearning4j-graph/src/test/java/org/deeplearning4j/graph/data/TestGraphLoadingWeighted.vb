Imports System.Collections.Generic
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports WeightedEdgeLineProcessor = org.deeplearning4j.graph.data.impl.WeightedEdgeLineProcessor
Imports org.deeplearning4j.graph.graph
Imports StringVertexFactory = org.deeplearning4j.graph.vertexfactory.StringVertexFactory
Imports org.deeplearning4j.graph.vertexfactory
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.deeplearning4j.graph.data


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("Permissions issues on CI") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestGraphLoadingWeighted extends org.deeplearning4j.BaseDL4JTest
	Public Class TestGraphLoadingWeighted
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testWeightedDirected() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWeightedDirected()

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

			For i As Integer = 0 To numVertices - 1
				Dim edgeList As IList(Of Edge(Of Double)) = graph.getEdgesOut(i)
				assertEquals(edges(i).Length, edgeList.Count)
				For Each e As Edge(Of Double) In edgeList
					Dim from As Integer = e.getFrom()
					Dim [to] As Integer = e.getTo()
					Dim weight As Double = e.getValue()
					assertEquals(i, from)
					assertTrue(ArrayUtils.contains(edges(i), [to]))
					Dim idx As Integer = ArrayUtils.IndexOf(edges(i), [to])
					assertEquals(edgeWeights(i)(idx), weight, 0.0)
				Next e
			Next i

	'        System.out.println(graph);
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000) public void testWeightedDirectedV2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWeightedDirectedV2()

			Dim path As String = (New ClassPathResource("deeplearning4j-graph/WeightedGraph.txt")).TempFileFromArchive.getAbsolutePath()
			Dim numVertices As Integer = 9
			Dim delim As String = ","
			Dim directed As Boolean = True
			Dim ignoreLinesStartingWith() As String = {"//"} 'Comment lines start with "//"

			Dim graph As IGraph(Of String, Double) = GraphLoader.loadWeightedEdgeListFile(path, numVertices, delim, directed, False, ignoreLinesStartingWith)

			assertEquals(numVertices, graph.numVertices())

			'EdgeLineProcessor: used to convert lines -> edges
			Dim edgeLineProcessor As EdgeLineProcessor(Of Double) = New WeightedEdgeLineProcessor(delim, directed, ignoreLinesStartingWith)
			'Vertex factory: used to create vertex objects, given an index for the vertex
			Dim vertexFactory As VertexFactory(Of String) = New StringVertexFactory()

			Dim graph2 As Graph(Of String, Double) = GraphLoader.loadGraph(path, edgeLineProcessor, vertexFactory, numVertices, False)

			assertEquals(graph, graph2)
		End Sub

	End Class

End Namespace