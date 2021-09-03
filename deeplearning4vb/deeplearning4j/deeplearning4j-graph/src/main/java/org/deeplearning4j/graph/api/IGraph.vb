Imports System
Imports System.Collections.Generic
Imports NoEdgesException = org.deeplearning4j.graph.exception.NoEdgesException

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

Namespace org.deeplearning4j.graph.api


	Public Interface IGraph(Of V, E)

		''' <summary>
		''' Number of vertices in the graph </summary>
		Function numVertices() As Integer

		''' <summary>
		'''Get a vertex in the graph for a given index </summary>
		''' <param name="idx"> integer index of the vertex to get. must be in range 0 to numVertices() </param>
		''' <returns> vertex </returns>
		Function getVertex(ByVal idx As Integer) As Vertex(Of V)

		''' <summary>
		''' Get multiple vertices in the graph </summary>
		''' <param name="indexes"> the indexes of the vertices to retrieve </param>
		''' <returns> list of vertices </returns>
		Function getVertices(ByVal indexes() As Integer) As IList(Of Vertex(Of V))

		''' <summary>
		''' Get multiple vertices in the graph, with secified indices </summary>
		''' <param name="from"> first vertex to get, inclusive </param>
		''' <param name="to"> last vertex to get, inclusive </param>
		''' <returns> list of vertices </returns>
		Function getVertices(ByVal from As Integer, ByVal [to] As Integer) As IList(Of Vertex(Of V))

		''' <summary>
		''' Add an edge to the graph.
		''' </summary>
		Sub addEdge(ByVal edge As Edge(Of E))

		''' <summary>
		''' Convenience method for adding an edge (directed or undirected) to graph </summary>
		Sub addEdge(ByVal from As Integer, ByVal [to] As Integer, ByVal value As E, ByVal directed As Boolean)

		''' <summary>
		''' Returns a list of edges for a vertex with a given index
		''' For undirected graphs, returns all edges incident on the vertex
		''' For directed graphs, only returns outward directed edges </summary>
		''' <param name="vertex"> index of the vertex to </param>
		''' <returns> list of edges for this vertex </returns>
		Function getEdgesOut(ByVal vertex As Integer) As IList(Of Edge(Of E))

		''' <summary>
		''' Returns the degree of the vertex.<br>
		''' For undirected graphs, this is just the degree.<br>
		''' For directed graphs, this returns the outdegree </summary>
		''' <param name="vertex"> vertex to get degree for </param>
		''' <returns> vertex degree </returns>
		Function getVertexDegree(ByVal vertex As Integer) As Integer

		''' <summary>
		''' Randomly sample a vertex connected to a given vertex. Sampling is done uniformly at random.
		''' Specifically, returns a random X such that either a directed edge (vertex -> X) exists,
		''' or an undirected edge (vertex -- X) exists<br>
		''' Can be used for example to implement a random walk on the graph (specifically: a unweighted random walk) </summary>
		''' <param name="vertex"> vertex to randomly sample from </param>
		''' <param name="rng"> Random number generator to use </param>
		''' <returns> A vertex connected to the specified vertex, </returns>
		''' <exception cref="NoEdgesException"> thrown if the specified vertex has no edges, or no outgoing edges (in the case
		''' of a directed graph). </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Vertex<V> getRandomConnectedVertex(int vertex, java.util.Random rng) throws org.deeplearning4j.graph.exception.NoEdgesException;
		Function getRandomConnectedVertex(ByVal vertex As Integer, ByVal rng As Random) As Vertex(Of V)

		''' <summary>
		'''Get a list of all of the vertices that the specified vertex is connected to<br>
		''' Specifically, for undirected graphs return list of all X such that (vertex -- X) exists<br>
		''' For directed graphs, return list of all X such that (vertex -> X) exists </summary>
		''' <param name="vertex"> Index of the vertex </param>
		''' <returns> list of vertices that the specified vertex is connected to </returns>
		Function getConnectedVertices(ByVal vertex As Integer) As IList(Of Vertex(Of V))

		''' <summary>
		'''Return an array of indexes of vertices that the specified vertex is connected to.<br>
		''' Specifically, for undirected graphs return int[] of all X.vertexID() such that (vertex -- X) exists<br>
		''' For directed graphs, return int[] of all X.vertexID() such that (vertex -> X) exists </summary>
		''' <param name="vertex"> index of the vertex </param>
		''' <returns> list of vertices that the specified vertex is connected to </returns>
		''' <seealso cref= #getConnectedVertices(int) </seealso>
		Function getConnectedVertexIndices(ByVal vertex As Integer) As Integer()
	End Interface

End Namespace