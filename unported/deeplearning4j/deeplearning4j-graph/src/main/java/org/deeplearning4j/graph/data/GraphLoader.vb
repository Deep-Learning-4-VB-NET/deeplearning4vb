Imports System.Collections.Generic
Imports System.IO
Imports org.deeplearning4j.graph.api
Imports org.deeplearning4j.graph.api
Imports DelimitedEdgeLineProcessor = org.deeplearning4j.graph.data.impl.DelimitedEdgeLineProcessor
Imports WeightedEdgeLineProcessor = org.deeplearning4j.graph.data.impl.WeightedEdgeLineProcessor
Imports org.deeplearning4j.graph.graph
Imports StringVertexFactory = org.deeplearning4j.graph.vertexfactory.StringVertexFactory
Imports org.deeplearning4j.graph.vertexfactory

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


	''' <summary>
	''' Utility methods for loading graphs
	''' 
	''' </summary>
	Public Class GraphLoader

		Private Sub New()
		End Sub

		''' <summary>
		''' Simple method for loading an undirected graph, where the graph is represented by a edge list with one edge
		''' per line with a delimiter in between<br>
		''' This method assumes that all lines in the file are of the form {@code i<delim>j} where i and j are integers
		''' in range 0 to numVertices inclusive, and "<delim>" is the user-provided delimiter
		''' <b>Note</b>: this method calls <seealso cref="loadUndirectedGraphEdgeListFile(String, Integer, String, Boolean)"/> with allowMultipleEdges = true. </summary>
		''' <param name="path"> Path to the edge list file </param>
		''' <param name="numVertices"> number of vertices in the graph </param>
		''' <returns> graph </returns>
		''' <exception cref="IOException"> if file cannot be read </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.graph.graph.Graph<String, String> loadUndirectedGraphEdgeListFile(String path, int numVertices, String delim) throws java.io.IOException
		Public Shared Function loadUndirectedGraphEdgeListFile(ByVal path As String, ByVal numVertices As Integer, ByVal delim As String) As Graph(Of String, String)
			Return loadUndirectedGraphEdgeListFile(path, numVertices, delim, True)
		End Function

		''' <summary>
		''' Simple method for loading an undirected graph, where the graph is represented by a edge list with one edge
		''' per line with a delimiter in between<br>
		''' This method assumes that all lines in the file are of the form {@code i<delim>j} where i and j are integers
		''' in range 0 to numVertices inclusive, and "<delim>" is the user-provided delimiter </summary>
		''' <param name="path"> Path to the edge list file </param>
		''' <param name="numVertices"> number of vertices in the graph </param>
		''' <param name="allowMultipleEdges"> If set to false, the graph will not allow multiple edges between any two vertices to exist. However,
		'''                           checking for duplicates during graph loading can be costly, so use allowMultipleEdges=true when
		'''                           possible. </param>
		''' <returns> graph </returns>
		''' <exception cref="IOException"> if file cannot be read </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.graph.graph.Graph<String, String> loadUndirectedGraphEdgeListFile(String path, int numVertices, String delim, boolean allowMultipleEdges) throws java.io.IOException
		Public Shared Function loadUndirectedGraphEdgeListFile(ByVal path As String, ByVal numVertices As Integer, ByVal delim As String, ByVal allowMultipleEdges As Boolean) As Graph(Of String, String)
			Dim graph As New Graph(Of String, String)(numVertices, allowMultipleEdges, New StringVertexFactory())
			Dim lineProcessor As EdgeLineProcessor(Of String) = New DelimitedEdgeLineProcessor(delim, False)

			Using br As New StreamReader(New java.io.File(path))
				Dim line As String
				line = br.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = br.readLine()) != null)
				Do While line IsNot Nothing
					Dim edge As Edge(Of String) = lineProcessor.processLine(line)
					If edge IsNot Nothing Then
						graph.addEdge(edge)
					End If
						line = br.ReadLine()
				Loop
			End Using
			Return graph
		End Function

		''' <summary>
		'''Method for loading a weighted graph from an edge list file, where each edge (inc. weight) is represented by a
		''' single line. Graph may be directed or undirected<br>
		''' This method assumes that edges are of the format: {@code fromIndex<delim>toIndex<delim>edgeWeight} where {@code <delim>}
		''' is the delimiter.
		''' <b>Note</b>: this method calls <seealso cref="loadWeightedEdgeListFile(String, Integer, String, Boolean, Boolean, String...)"/> with allowMultipleEdges = true. </summary>
		''' <param name="path"> Path to the edge list file </param>
		''' <param name="numVertices"> The number of vertices in the graph </param>
		''' <param name="delim"> The delimiter used in the file (typically: "," or " " etc) </param>
		''' <param name="directed"> whether the edges should be treated as directed (true) or undirected (false) </param>
		''' <param name="ignoreLinesStartingWith"> Starting characters for comment lines. May be null. For example: "//" or "#" </param>
		''' <returns> The graph </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.graph.graph.Graph<String, Double> loadWeightedEdgeListFile(String path, int numVertices, String delim, boolean directed, String... ignoreLinesStartingWith) throws java.io.IOException
		Public Shared Function loadWeightedEdgeListFile(ByVal path As String, ByVal numVertices As Integer, ByVal delim As String, ByVal directed As Boolean, ParamArray ByVal ignoreLinesStartingWith() As String) As Graph(Of String, Double)
			Return loadWeightedEdgeListFile(path, numVertices, delim, directed, True, ignoreLinesStartingWith)
		End Function

		''' <summary>
		'''Method for loading a weighted graph from an edge list file, where each edge (inc. weight) is represented by a
		''' single line. Graph may be directed or undirected<br>
		''' This method assumes that edges are of the format: {@code fromIndex<delim>toIndex<delim>edgeWeight} where {@code <delim>}
		''' is the delimiter. </summary>
		''' <param name="path"> Path to the edge list file </param>
		''' <param name="numVertices"> The number of vertices in the graph </param>
		''' <param name="delim"> The delimiter used in the file (typically: "," or " " etc) </param>
		''' <param name="directed"> whether the edges should be treated as directed (true) or undirected (false) </param>
		''' <param name="allowMultipleEdges"> If set to false, the graph will not allow multiple edges between any two vertices to exist. However,
		'''                           checking for duplicates during graph loading can be costly, so use allowMultipleEdges=true when
		'''                           possible. </param>
		''' <param name="ignoreLinesStartingWith"> Starting characters for comment lines. May be null. For example: "//" or "#" </param>
		''' <returns> The graph </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.graph.graph.Graph<String, Double> loadWeightedEdgeListFile(String path, int numVertices, String delim, boolean directed, boolean allowMultipleEdges, String... ignoreLinesStartingWith) throws java.io.IOException
		Public Shared Function loadWeightedEdgeListFile(ByVal path As String, ByVal numVertices As Integer, ByVal delim As String, ByVal directed As Boolean, ByVal allowMultipleEdges As Boolean, ParamArray ByVal ignoreLinesStartingWith() As String) As Graph(Of String, Double)
			Dim graph As New Graph(Of String, Double)(numVertices, allowMultipleEdges, New StringVertexFactory())
			Dim lineProcessor As EdgeLineProcessor(Of Double) = New WeightedEdgeLineProcessor(delim, directed, ignoreLinesStartingWith)

			Using br As New StreamReader(New java.io.File(path))
				Dim line As String
				line = br.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = br.readLine()) != null)
				Do While line IsNot Nothing
					Dim edge As Edge(Of Double) = lineProcessor.processLine(line)
					If edge IsNot Nothing Then
						graph.addEdge(edge)
					End If
						line = br.ReadLine()
				Loop
			End Using
			Return graph
		End Function

		''' <summary>
		''' Load a graph into memory, using a given EdgeLineProcessor.
		''' Assume one edge per line </summary>
		''' <param name="path"> Path to the file containing the edges, one per line </param>
		''' <param name="lineProcessor"> EdgeLineProcessor used to convert lines of text into a graph (or null for comment lines etc) </param>
		''' <param name="vertexFactory"> Used to create vertices </param>
		''' <param name="numVertices"> number of vertices in the graph </param>
		''' <param name="allowMultipleEdges"> whether the graph should allow multiple edges between a given pair of vertices or not </param>
		''' <returns> IGraph </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <V, E> org.deeplearning4j.graph.graph.Graph<V, E> loadGraph(String path, EdgeLineProcessor<E> lineProcessor, org.deeplearning4j.graph.vertexfactory.VertexFactory<V> vertexFactory, int numVertices, boolean allowMultipleEdges) throws java.io.IOException
		Public Shared Function loadGraph(Of V, E)(ByVal path As String, ByVal lineProcessor As EdgeLineProcessor(Of E), ByVal vertexFactory As VertexFactory(Of V), ByVal numVertices As Integer, ByVal allowMultipleEdges As Boolean) As Graph(Of V, E)
			Dim graph As New Graph(Of V, E)(numVertices, allowMultipleEdges, vertexFactory)

			Using br As New StreamReader(New java.io.File(path))
				Dim line As String
				line = br.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = br.readLine()) != null)
				Do While line IsNot Nothing
					Dim edge As Edge(Of E) = lineProcessor.processLine(line)
					If edge IsNot Nothing Then
						graph.addEdge(edge)
					End If
						line = br.ReadLine()
				Loop
			End Using

			Return graph
		End Function

		''' <summary>
		''' Load graph, assuming vertices are in one file and edges are in another file.
		''' </summary>
		''' <param name="vertexFilePath"> Path to file containing vertices, one per line </param>
		''' <param name="edgeFilePath"> Path to the file containing edges, one per line </param>
		''' <param name="vertexLoader"> VertexLoader, for loading vertices from the file </param>
		''' <param name="edgeLineProcessor"> EdgeLineProcessor, converts text lines into edges </param>
		''' <param name="allowMultipleEdges"> whether the graph should allow (or filter out) multiple edges </param>
		''' <returns> IGraph loaded from files </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <V, E> org.deeplearning4j.graph.graph.Graph<V, E> loadGraph(String vertexFilePath, String edgeFilePath, VertexLoader<V> vertexLoader, EdgeLineProcessor<E> edgeLineProcessor, boolean allowMultipleEdges) throws java.io.IOException
		Public Shared Function loadGraph(Of V, E)(ByVal vertexFilePath As String, ByVal edgeFilePath As String, ByVal vertexLoader As VertexLoader(Of V), ByVal edgeLineProcessor As EdgeLineProcessor(Of E), ByVal allowMultipleEdges As Boolean) As Graph(Of V, E)
			'Assume vertices are in one file
			'And edges are in another file

			Dim vertices As IList(Of Vertex(Of V)) = vertexLoader.loadVertices(vertexFilePath)
			Dim graph As New Graph(Of V, E)(vertices, allowMultipleEdges)

			Using br As New StreamReader(New java.io.File(edgeFilePath))
				Dim line As String
				line = br.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = br.readLine()) != null)
				Do While line IsNot Nothing
					Dim edge As Edge(Of E) = edgeLineProcessor.processLine(line)
					If edge IsNot Nothing Then
						graph.addEdge(edge)
					End If
						line = br.ReadLine()
				Loop
			End Using

			Return graph
		End Function
	End Class

End Namespace